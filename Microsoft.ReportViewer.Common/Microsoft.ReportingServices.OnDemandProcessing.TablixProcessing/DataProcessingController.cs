using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class DataProcessingController
	{
		private readonly OnDemandProcessingContext m_odpContext;

		private readonly Microsoft.ReportingServices.ReportIntermediateFormat.DataSet m_dataSet;

		private readonly Microsoft.ReportingServices.ReportIntermediateFormat.Report m_report;

		private readonly DataSetInstance m_dataSetInstance;

		private RuntimeOnDemandDataSetObj m_dataSetObj;

		private bool m_hasSortFilterInfo;

		public IOnDemandScopeInstance GroupTreeRoot => m_dataSetObj;

		public DataProcessingController(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, DataSetInstance dataSetInstance)
		{
			m_odpContext = odpContext;
			m_dataSet = dataSet;
			m_dataSetInstance = dataSetInstance;
			m_report = odpContext.ReportDefinition;
			m_odpContext.EnsureScalabilitySetup();
			UserSortFilterContext userSortFilterContext = m_odpContext.UserSortFilterContext;
			if (!m_odpContext.InSubreportInDataRegion)
			{
				userSortFilterContext.ResetContextForTopLevelDataSet();
			}
			m_hasSortFilterInfo = m_odpContext.PopulateRuntimeSortFilterEventInfo(m_dataSet);
			if (-1 == userSortFilterContext.DataSetGlobalId)
			{
				userSortFilterContext.DataSetGlobalId = m_dataSet.GlobalID;
			}
			Global.Tracer.Assert(m_odpContext.ReportObjectModel != null && m_odpContext.ReportRuntime != null);
			m_odpContext.SetupFieldsForNewDataSet(m_dataSet, m_dataSetInstance, addRowIndex: true, noRows: true);
			m_dataSet.SetFilterExprHost(m_odpContext.ReportObjectModel);
			m_dataSetObj = new RuntimeOnDemandDataSetObj(m_odpContext, m_dataSet, m_dataSetInstance);
		}

		public void InitializeDataProcessing()
		{
			m_dataSetObj.Initialize();
			Global.Tracer.Assert(m_odpContext.CurrentDataSetIndex == m_dataSet.IndexInCollection, "(m_odpContext.CurrentDataSetIndex == m_dataSet.IndexInCollection)");
		}

		public void TeardownDataProcessing()
		{
			m_dataSetObj.Teardown();
			m_odpContext.EnsureScalabilityCleanup();
		}

		public void NextRow(Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow row, int rowNumber, bool useRowOffset, bool readerExtensionsSupported)
		{
			FieldsImpl fieldsImplForUpdate = m_odpContext.ReportObjectModel.GetFieldsImplForUpdate(m_dataSet);
			if (useRowOffset)
			{
				fieldsImplForUpdate.NewRow();
			}
			else
			{
				fieldsImplForUpdate.NewRow(row.StreamPosition);
			}
			if (fieldsImplForUpdate.AddRowIndex)
			{
				fieldsImplForUpdate.SetRowIndex(rowNumber);
			}
			m_odpContext.ReportObjectModel.UpdateFieldValues(reuseFieldObjects: false, row, m_dataSetInstance, readerExtensionsSupported);
			m_dataSetObj.NextRow();
		}

		public void AllRowsRead()
		{
			m_dataSetObj.FinishReadingRows();
			m_odpContext.FirstPassPostProcess();
			Global.Tracer.Trace(TraceLevel.Verbose, "TablixProcessing: FirstPass Complete");
			if (m_report.HasAggregatesOfAggregatesInUserSort && m_odpContext.RuntimeSortFilterInfo != null && m_odpContext.RuntimeSortFilterInfo.Count > 0)
			{
				PreComputeAggregatesOfAggregates();
			}
			m_odpContext.ApplyUserSorts();
			Global.Tracer.Trace(TraceLevel.Verbose, "TablixProcessing: ApplyUserSorts Complete");
			SortAndFilter();
			Global.Tracer.Trace(TraceLevel.Verbose, "TablixProcessing: SortAndFilter Complete");
			m_odpContext.CheckAndThrowIfAborted();
			PostSortOperations();
			Global.Tracer.Trace(TraceLevel.Verbose, "TablixProcessing: PostSortOperations Complete");
			StoreDataSetLevelAggregates();
		}

		private void PreComputeAggregatesOfAggregates()
		{
			if (m_report.NeedPostGroupProcessing)
			{
				m_odpContext.SecondPassOperation = SecondPassOperations.FilteringOrAggregatesOrDomainScope;
				AggregateUpdateContext aggContext = new AggregateUpdateContext(m_odpContext, AggregateMode.Aggregates);
				m_odpContext.DomainScopeContext = new DomainScopeContext();
				m_dataSetObj.SortAndFilter(aggContext);
				m_odpContext.DomainScopeContext = null;
			}
		}

		private void SortAndFilter()
		{
			if (m_report.NeedPostGroupProcessing)
			{
				m_odpContext.SecondPassOperation = (m_report.HasVariables ? SecondPassOperations.Variables : SecondPassOperations.None);
				if (m_report.HasSpecialRecursiveAggregates)
				{
					m_odpContext.SecondPassOperation |= SecondPassOperations.FilteringOrAggregatesOrDomainScope;
				}
				else
				{
					m_odpContext.SecondPassOperation |= (SecondPassOperations.Sorting | SecondPassOperations.FilteringOrAggregatesOrDomainScope);
				}
				AggregateUpdateContext aggContext = new AggregateUpdateContext(m_odpContext, AggregateMode.Aggregates);
				m_dataSetObj.EnterProcessUserSortPhase();
				m_odpContext.DomainScopeContext = new DomainScopeContext();
				m_dataSetObj.SortAndFilter(aggContext);
				m_odpContext.DomainScopeContext = null;
				if (m_report.HasSpecialRecursiveAggregates)
				{
					m_odpContext.SecondPassOperation = SecondPassOperations.Sorting;
					m_dataSetObj.SortAndFilter(aggContext);
				}
				m_dataSetObj.LeaveProcessUserSortPhase();
			}
		}

		private void PostSortOperations()
		{
			if (m_report.HasPostSortAggregates)
			{
				Dictionary<string, IReference<RuntimeGroupRootObj>> groupCollection = new Dictionary<string, IReference<RuntimeGroupRootObj>>();
				AggregateUpdateContext aggContext = new AggregateUpdateContext(m_odpContext, AggregateMode.PostSortAggregates);
				m_dataSetObj.CalculateRunningValues(groupCollection, aggContext);
			}
		}

		private void StoreDataSetLevelAggregates()
		{
			if (m_dataSet.Aggregates != null || m_dataSet.PostSortAggregates != null)
			{
				DataSetInstance dataSetInstance = m_odpContext.CurrentReportInstance.GetDataSetInstance(m_dataSet.IndexInCollection, m_odpContext);
				if (m_dataSet.Aggregates != null)
				{
					dataSetInstance.StoreAggregates(m_odpContext, m_dataSet.Aggregates);
				}
				if (m_dataSet.PostSortAggregates != null)
				{
					dataSetInstance.StoreAggregates(m_odpContext, m_dataSet.PostSortAggregates);
				}
			}
		}

		public void GenerateGroupTree()
		{
			Global.Tracer.Trace(TraceLevel.Verbose, "Data processing complete.  Beginning group tree creation");
			OnDemandMetadata odpMetadata = m_odpContext.OdpMetadata;
			Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.EnsureGroupTreeStorageSetup(odpMetadata, m_odpContext.ChunkFactory, odpMetadata.GlobalIDOwnerCollection, openExisting: false, m_odpContext.GetActiveCompatibilityVersion(), m_odpContext.ProhibitSerializableValues);
			CreateInstances();
			if (!m_odpContext.InSubreportInDataRegion)
			{
				m_odpContext.TopLevelContext.MergeNewUserSortFilterInformation();
				odpMetadata.ResetUserSortFilterContexts();
			}
			m_dataSetObj.CompleteLookupProcessing();
		}

		private void CreateInstances()
		{
			m_odpContext.ReportRuntime.CurrentScope = m_dataSetObj;
			if (m_hasSortFilterInfo && m_odpContext.RuntimeSortFilterInfo != null)
			{
				if (m_odpContext.TopLevelContext.ReportRuntimeUserSortFilterInfo == null)
				{
					m_odpContext.TopLevelContext.ReportRuntimeUserSortFilterInfo = new List<IReference<RuntimeSortFilterEventInfo>>();
				}
				m_odpContext.TopLevelContext.ReportRuntimeUserSortFilterInfo.AddRange(m_odpContext.RuntimeSortFilterInfo);
			}
			m_dataSetObj.CreateInstances();
			if (m_odpContext.ReportDefinition.InScopeEventSources != null)
			{
				int count = m_odpContext.ReportDefinition.InScopeEventSources.Count;
				List<IInScopeEventSource> list = null;
				for (int i = 0; i < count; i++)
				{
					IInScopeEventSource inScopeEventSource = m_odpContext.ReportDefinition.InScopeEventSources[i];
					if (inScopeEventSource.UserSort.DataSet == m_dataSet)
					{
						if (list == null)
						{
							list = new List<IInScopeEventSource>(count - i);
						}
						list.Add(inScopeEventSource);
					}
				}
				if (list != null)
				{
					UserSortFilterContext.ProcessEventSources(m_odpContext, m_dataSetObj, list);
				}
			}
			m_odpContext.ReportRuntime.CurrentScope = null;
		}
	}
}

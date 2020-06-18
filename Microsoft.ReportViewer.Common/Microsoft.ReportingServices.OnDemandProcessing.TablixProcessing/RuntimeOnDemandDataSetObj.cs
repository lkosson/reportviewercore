using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class RuntimeOnDemandDataSetObj : Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner, IHierarchyObj, IStorable, IPersistable, IScope, ISelfReferential, IOnDemandScopeInstance
	{
		private readonly OnDemandProcessingContext m_odpContext;

		private readonly Microsoft.ReportingServices.ReportIntermediateFormat.DataSet m_dataSet;

		private readonly DataSetInstance m_dataSetInstance;

		private ScalableList<DataFieldRow> m_dataRows;

		private RuntimeUserSortTargetInfo m_userSortTargetInfo;

		private RuntimeOnDemandDataSetObjReference m_selfReference;

		private int[] m_sortFilterExpressionScopeInfoIndices;

		private RuntimeRICollection m_runtimeDataRegions;

		private bool m_firstNonAggregateRow = true;

		private Filters m_filters;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_nonCustomAggregates;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_customAggregates;

		private RuntimeLookupProcessing m_lookupProcessor;

		public bool IsNoRows => false;

		public bool IsMostRecentlyCreatedScopeInstance => false;

		public bool HasUnProcessedServerAggregate => false;

		IReference<IHierarchyObj> IHierarchyObj.HierarchyRoot => m_selfReference;

		OnDemandProcessingContext IHierarchyObj.OdpContext => m_odpContext;

		BTree IHierarchyObj.SortTree
		{
			get
			{
				if (m_userSortTargetInfo != null)
				{
					return m_userSortTargetInfo.SortTree;
				}
				return null;
			}
		}

		int IHierarchyObj.ExpressionIndex => 0;

		List<int> IHierarchyObj.SortFilterInfoIndices
		{
			get
			{
				if (m_userSortTargetInfo != null)
				{
					return m_userSortTargetInfo.SortFilterInfoIndices;
				}
				return null;
			}
		}

		bool IHierarchyObj.IsDetail => false;

		bool IHierarchyObj.InDataRowSortPhase => false;

		int IStorable.Size => 0;

		public IReference<IHierarchyObj> SelfReference => m_selfReference;

		public int Depth => 0;

		bool IScope.TargetForNonDetailSort
		{
			get
			{
				if (m_userSortTargetInfo != null)
				{
					return m_userSortTargetInfo.TargetForNonDetailSort;
				}
				return false;
			}
		}

		int[] IScope.SortFilterExpressionScopeInfoIndices
		{
			get
			{
				if (m_sortFilterExpressionScopeInfoIndices == null)
				{
					m_sortFilterExpressionScopeInfoIndices = new int[m_odpContext.RuntimeSortFilterInfo.Count];
					for (int i = 0; i < m_odpContext.RuntimeSortFilterInfo.Count; i++)
					{
						m_sortFilterExpressionScopeInfoIndices[i] = -1;
					}
				}
				return m_sortFilterExpressionScopeInfoIndices;
			}
		}

		IRIFReportScope IScope.RIFReportScope => null;

		public RuntimeOnDemandDataSetObj(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, DataSetInstance dataSetInstance)
		{
			m_odpContext = odpContext;
			m_dataSet = dataSet;
			m_dataSetInstance = dataSetInstance;
			m_odpContext.TablixProcessingScalabilityCache.GenerateFixedReference(this);
			UserSortFilterContext userSortFilterContext = odpContext.UserSortFilterContext;
			if (m_odpContext.IsSortFilterTarget(dataSet.IsSortFilterTarget, userSortFilterContext.CurrentContainingScope, SelfReference, ref m_userSortTargetInfo) && m_userSortTargetInfo.TargetForNonDetailSort)
			{
				m_dataRows = new ScalableList<DataFieldRow>(0, odpContext.TablixProcessingScalabilityCache, 100, 10);
			}
			if (!m_odpContext.StreamingMode)
			{
				CreateRuntimeStructure();
			}
			m_dataSet.SetupRuntimeEnvironment(m_odpContext);
			if (m_dataSet.Filters != null)
			{
				m_filters = new Filters(Filters.FilterTypes.DataSetFilter, (IReference<Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>)SelfReference, m_dataSet.Filters, m_dataSet.ObjectType, m_dataSet.Name, m_odpContext, 0);
			}
			RegisterAggregates();
		}

		public void Initialize()
		{
			if (m_dataSet.LookupDestinationInfos != null)
			{
				m_lookupProcessor = new RuntimeLookupProcessing(m_odpContext, m_dataSet, m_dataSetInstance, this);
			}
		}

		public void NextRow()
		{
			if (m_odpContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
			{
				NextAggregateRow();
				return;
			}
			bool flag = true;
			if (m_filters != null)
			{
				flag = m_filters.PassFilters(new DataFieldRow(m_odpContext.ReportObjectModel.FieldsImpl, getAndSave: false));
			}
			if (flag)
			{
				PostFilterNextRow();
			}
		}

		private void NextAggregateRow()
		{
			if (m_odpContext.ReportObjectModel.FieldsImpl.AggregationFieldCount == 0 && m_customAggregates != null)
			{
				for (int i = 0; i < m_customAggregates.Count; i++)
				{
					m_customAggregates[i].Update();
				}
			}
			if (m_userSortTargetInfo != null && m_userSortTargetInfo.SortTree != null)
			{
				if (m_userSortTargetInfo.AggregateRows == null)
				{
					m_userSortTargetInfo.AggregateRows = new List<AggregateRow>();
				}
				AggregateRow item = new AggregateRow(m_odpContext.ReportObjectModel.FieldsImpl, getAndSave: true);
				m_userSortTargetInfo.AggregateRows.Add(item);
				if (!m_userSortTargetInfo.TargetForNonDetailSort)
				{
					return;
				}
			}
			SendToInner();
		}

		public void PostFilterNextRow()
		{
			if (m_nonCustomAggregates != null)
			{
				for (int i = 0; i < m_nonCustomAggregates.Count; i++)
				{
					m_nonCustomAggregates[i].Update();
				}
			}
			if (m_dataRows != null)
			{
				RuntimeDataTablixObj.SaveData(m_dataRows, m_odpContext);
			}
			if (m_firstNonAggregateRow)
			{
				m_firstNonAggregateRow = false;
				m_dataSetInstance.FirstRowOffset = m_odpContext.ReportObjectModel.FieldsImpl.StreamOffset;
			}
			if (m_lookupProcessor != null)
			{
				m_lookupProcessor.NextRow();
			}
			else
			{
				PostLookupNextRow();
			}
		}

		internal void PostLookupNextRow()
		{
			SendToInner();
		}

		private void SendToInner()
		{
			if (m_runtimeDataRegions == null)
			{
				CreateRuntimeStructure();
			}
			m_runtimeDataRegions.FirstPassNextDataRow(m_odpContext);
		}

		internal void CompleteLookupProcessing()
		{
			if (m_lookupProcessor != null)
			{
				m_lookupProcessor.CompleteLookupProcessing();
				m_lookupProcessor = null;
			}
		}

		public void FinishReadingRows()
		{
			if (m_filters != null)
			{
				m_filters.FinishReadingRows();
			}
			m_odpContext.CheckAndThrowIfAborted();
			if (m_lookupProcessor != null && m_lookupProcessor.MustBufferAllRows)
			{
				m_lookupProcessor.FinishReadingRows();
			}
		}

		public void SortAndFilter(AggregateUpdateContext aggContext)
		{
			if (m_runtimeDataRegions != null)
			{
				m_runtimeDataRegions.SortAndFilter(aggContext);
			}
		}

		public void EnterProcessUserSortPhase()
		{
			if (m_userSortTargetInfo != null)
			{
				m_userSortTargetInfo.EnterProcessUserSortPhase(m_odpContext);
			}
		}

		public void LeaveProcessUserSortPhase()
		{
			if (m_userSortTargetInfo != null)
			{
				m_userSortTargetInfo.LeaveProcessUserSortPhase(m_odpContext);
			}
		}

		public void CalculateRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCollection, AggregateUpdateContext aggContext)
		{
			if (m_runtimeDataRegions != null)
			{
				m_runtimeDataRegions.CalculateRunningValues(groupCollection, null, aggContext);
			}
		}

		public void CreateInstances()
		{
			if (m_runtimeDataRegions != null)
			{
				m_runtimeDataRegions.CreateAllDataRegionInstances(m_odpContext.CurrentReportInstance, m_odpContext, m_selfReference);
			}
		}

		public void Teardown()
		{
			m_selfReference = null;
		}

		public IOnDemandMemberOwnerInstanceReference GetDataRegionInstance(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion)
		{
			if (m_runtimeDataRegions == null)
			{
				return null;
			}
			return m_runtimeDataRegions.GetDataRegionObj(rifDataRegion);
		}

		public IReference<IDataCorrelation> GetIdcReceiver(IRIFReportDataScope scope)
		{
			return null;
		}

		private void CreateRuntimeStructure()
		{
			m_runtimeDataRegions = new RuntimeRICollection(m_selfReference, m_dataSet.DataRegions, m_odpContext, m_odpContext.ReportDefinition.MergeOnePass);
		}

		private void RegisterAggregates()
		{
			if (m_odpContext.InSubreport)
			{
				m_odpContext.ReportObjectModel.AggregatesImpl.ClearAll();
			}
			CreateAggregates(m_dataSet.Aggregates);
			CreateAggregates(m_dataSet.PostSortAggregates);
		}

		private void CreateAggregates(List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> aggDefs)
		{
			if (aggDefs == null || 0 >= aggDefs.Count)
			{
				return;
			}
			AggregatesImpl aggregatesImpl = m_odpContext.ReportObjectModel.AggregatesImpl;
			for (int i = 0; i < aggDefs.Count; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo dataAggregateInfo = aggDefs[i];
				Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj dataAggregateObj = aggregatesImpl.GetAggregateObj(dataAggregateInfo.Name);
				if (dataAggregateObj == null)
				{
					dataAggregateObj = new Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj(dataAggregateInfo, m_odpContext);
					aggregatesImpl.Add(dataAggregateObj);
				}
				if (Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Previous != dataAggregateInfo.AggregateType)
				{
					if (Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Aggregate == dataAggregateInfo.AggregateType)
					{
						RuntimeDataRegionObj.AddAggregate(ref m_customAggregates, dataAggregateObj);
					}
					else
					{
						RuntimeDataRegionObj.AddAggregate(ref m_nonCustomAggregates, dataAggregateObj);
					}
				}
			}
		}

		IHierarchyObj IHierarchyObj.CreateHierarchyObjForSortTree()
		{
			return new RuntimeSortHierarchyObj(this, 1);
		}

		ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
		{
			return m_odpContext.RegisterComparisonErrorForSortFilterEvent(propertyName);
		}

		internal ProcessingMessageList RegisterSpatialElementComparisonError(string type)
		{
			m_odpContext.ErrorContext.Register(ProcessingErrorCode.rsCannotCompareSpatialType, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.DataSet, m_dataSet.Name, type);
			return m_odpContext.ErrorContext.Messages;
		}

		void IHierarchyObj.NextRow(IHierarchyObj owner)
		{
			Global.Tracer.Assert(condition: false);
		}

		void IHierarchyObj.Traverse(ProcessingStages operation, ITraversalContext traversalContext)
		{
			Global.Tracer.Assert(condition: false);
		}

		void IHierarchyObj.ReadRow()
		{
			SendToInner();
		}

		void IHierarchyObj.ProcessUserSort()
		{
			Global.Tracer.Assert(m_userSortTargetInfo != null, "(null != m_userSortTargetInfo)");
			m_odpContext.ProcessUserSortForTarget(SelfReference, ref m_dataRows, m_userSortTargetInfo.TargetForNonDetailSort);
			if (!m_userSortTargetInfo.TargetForNonDetailSort)
			{
				return;
			}
			m_userSortTargetInfo.ResetTargetForNonDetailSort();
			m_userSortTargetInfo.EnterProcessUserSortPhase(m_odpContext);
			CreateRuntimeStructure();
			m_userSortTargetInfo.SortTree.Traverse(ProcessingStages.UserSortFilter, ascending: true, null);
			m_userSortTargetInfo.SortTree.Dispose();
			m_userSortTargetInfo.SortTree = null;
			if (m_userSortTargetInfo.AggregateRows != null)
			{
				for (int i = 0; i < m_userSortTargetInfo.AggregateRows.Count; i++)
				{
					m_userSortTargetInfo.AggregateRows[i].SetFields(m_odpContext.ReportObjectModel.FieldsImpl);
					SendToInner();
				}
				m_userSortTargetInfo.AggregateRows = null;
			}
			m_userSortTargetInfo.LeaveProcessUserSortPhase(m_odpContext);
		}

		void IHierarchyObj.MarkSortInfoProcessed(List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo)
		{
			if (m_userSortTargetInfo != null)
			{
				m_userSortTargetInfo.MarkSortInfoProcessed(runtimeSortFilterInfo, SelfReference);
			}
		}

		void IHierarchyObj.AddSortInfoIndex(int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
		{
			if (m_userSortTargetInfo != null)
			{
				m_userSortTargetInfo.AddSortInfoIndex(sortInfoIndex, sortInfo);
			}
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			Global.Tracer.Assert(condition: false, "RuntimeOnDemandDataSetObj should not be serialized");
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(condition: false, "RuntimeOnDemandDataSetObj should not be de-serialized");
		}

		void IPersistable.ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false, "RuntimeOnDemandDataSetObj should not need references resolved");
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeOnDemandDataSetObj;
		}

		public void SetReference(IReference selfRef)
		{
			m_selfReference = (RuntimeOnDemandDataSetObjReference)selfRef;
		}

		bool IScope.IsTargetForSort(int index, bool detailSort)
		{
			if (m_userSortTargetInfo != null)
			{
				return m_userSortTargetInfo.IsTargetForSort(index, detailSort);
			}
			return false;
		}

		bool IScope.InScope(string scope)
		{
			if (Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(m_dataSet.Name, scope, ignoreCase: false) == 0)
			{
				return true;
			}
			return false;
		}

		void IScope.ReadRow(DataActions dataAction, ITraversalContext context)
		{
			Global.Tracer.Assert(condition: false);
		}

		IReference<IScope> IScope.GetOuterScope(bool includeSubReportContainingScope)
		{
			if (includeSubReportContainingScope)
			{
				return m_odpContext.UserSortFilterContext.CurrentContainingScope;
			}
			return null;
		}

		void IScope.CalculatePreviousAggregates()
		{
		}

		string IScope.GetScopeName()
		{
			return m_dataSet.Name;
		}

		int IScope.RecursiveLevel(string scope)
		{
			return 0;
		}

		bool IScope.TargetScopeMatched(int index, bool detailSort)
		{
			if (m_odpContext.UserSortFilterContext.CurrentContainingScope != null)
			{
				return m_odpContext.UserSortFilterContext.CurrentContainingScope.Value().TargetScopeMatched(index, detailSort);
			}
			if (m_odpContext.RuntimeSortFilterInfo != null)
			{
				return true;
			}
			return false;
		}

		void IScope.GetScopeValues(IReference<IHierarchyObj> targetScopeObj, List<object>[] scopeValues, ref int index)
		{
			IReference<IScope> currentContainingScope = m_odpContext.UserSortFilterContext.CurrentContainingScope;
			if (currentContainingScope != null && (targetScopeObj == null || this != targetScopeObj.Value()))
			{
				currentContainingScope.Value().GetScopeValues(null, scopeValues, ref index);
			}
		}

		void IScope.GetGroupNameValuePairs(Dictionary<string, object> pairs)
		{
		}

		public void SetupEnvironment()
		{
		}

		void IScope.UpdateAggregates(AggregateUpdateContext context)
		{
		}
	}
}

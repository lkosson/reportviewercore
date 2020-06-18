using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class OnDemandStateManagerStreaming : OnDemandStateManager
	{
		private class DataPipelineThrottle
		{
			private bool m_stoppingScopeInstanceCreated;

			private bool m_anyScopeInstanceCreated;

			private PipelineAdvanceMode m_pipelineMode;

			private IRIFReportDataScope m_targetScopeForDataProcessing;

			private bool m_inUse;

			private bool m_metStoppingCondition;

			public bool InUse => m_inUse;

			internal bool ShouldStopPipelineAdvance(bool rowAccepted)
			{
				switch (m_pipelineMode)
				{
				case PipelineAdvanceMode.ByOneRow:
					m_metStoppingCondition = rowAccepted;
					break;
				case PipelineAdvanceMode.ToStoppingScopeInstance:
					m_metStoppingCondition = (rowAccepted && m_stoppingScopeInstanceCreated);
					break;
				case PipelineAdvanceMode.ToFulfillServerAggregate:
					m_metStoppingCondition = (m_stoppingScopeInstanceCreated || !NeedsDataForServerAggregate(m_targetScopeForDataProcessing));
					break;
				default:
					Global.Tracer.Assert(false, "Unknown pipeline mode: {0}", m_pipelineMode);
					throw new InvalidOperationException();
				}
				return m_metStoppingCondition;
			}

			internal void CreatedScopeInstance(IRIFReportDataScope scope)
			{
				m_anyScopeInstanceCreated = true;
				if (CanBindOrProcessIndividually(scope) && IsTargetScopeForDataProcessing(scope))
				{
					m_stoppingScopeInstanceCreated = true;
				}
			}

			private bool IsTargetScopeForDataProcessing(IRIFReportDataScope candidateScope)
			{
				return m_targetScopeForDataProcessing.IsSameOrChildScopeOf(candidateScope);
			}

			public void StartUsingContext(PipelineAdvanceMode mode, IRIFReportDataScope targetScope)
			{
				m_inUse = true;
				m_metStoppingCondition = false;
				m_anyScopeInstanceCreated = false;
				m_stoppingScopeInstanceCreated = false;
				m_pipelineMode = mode;
				m_targetScopeForDataProcessing = targetScope;
			}

			public bool StopUsingContext()
			{
				m_inUse = false;
				if (m_pipelineMode == PipelineAdvanceMode.ToStoppingScopeInstance)
				{
					return m_anyScopeInstanceCreated;
				}
				return m_metStoppingCondition;
			}
		}

		private enum PipelineAdvanceMode
		{
			ToStoppingScopeInstance,
			ByOneRow,
			ToFulfillServerAggregate
		}

		private IReportScopeInstance m_lastROMInstance;

		private IReference<IOnDemandScopeInstance> m_lastOnDemandScopeInstance;

		private IRIFReportDataScope m_lastRIFObject;

		private DataPipelineManager m_pipelineManager;

		private QueryRestartInfo m_queryRestartInfo;

		private ExecutedQueryCache m_executedQueryCache;

		private EventHandler m_abortProcessor;

		private DataPipelineThrottle m_pipelineThrottle;

		private DataPipelineThrottle m_pipelineThrottle2;

		internal override IReportScopeInstance LastROMInstance => m_lastROMInstance;

		internal override IRIFReportScope LastTablixProcessingReportScope
		{
			get
			{
				return m_lastRIFObject;
			}
			set
			{
				Global.Tracer.Assert(condition: false, "Set LastTablixProcessingReportScope not supported in this execution mode");
				throw new NotImplementedException();
			}
		}

		internal override IInstancePath LastRIFObject
		{
			get
			{
				return m_lastRIFObject;
			}
			set
			{
				Global.Tracer.Assert(condition: false, "Set LastRIFObject not supported in this execution mode");
				throw new NotImplementedException();
			}
		}

		internal override QueryRestartInfo QueryRestartInfo => m_queryRestartInfo;

		internal override ExecutedQueryCache ExecutedQueryCache => m_executedQueryCache;

		public OnDemandStateManagerStreaming(OnDemandProcessingContext odpContext)
			: base(odpContext)
		{
			m_queryRestartInfo = new QueryRestartInfo();
			if (m_odpContext.AbortInfo != null)
			{
				m_abortProcessor = AbortHandler;
				m_odpContext.AbortInfo.ProcessingAbortEvent += m_abortProcessor;
			}
		}

		internal override ExecutedQueryCache SetupExecutedQueryCache()
		{
			Global.Tracer.Assert(m_executedQueryCache == null, "Cannot SetupExecutedQueryCache twice");
			m_executedQueryCache = new ExecutedQueryCache();
			return ExecutedQueryCache;
		}

		internal override void ResetOnDemandState()
		{
		}

		internal override int RecursiveLevel(string scopeName)
		{
			Global.Tracer.Assert(condition: false, "The Level function is not supported in this execution mode.");
			throw new NotImplementedException();
		}

		internal override bool InScope(string scopeName)
		{
			Global.Tracer.Assert(condition: false, "The InScope function is not supported in this execution mode.");
			throw new NotImplementedException();
		}

		internal override Dictionary<string, object> GetCurrentSpecialGroupingValues()
		{
			Global.Tracer.Assert(condition: false, "The CreateDrillthroughContext function is not supported in this execution mode.");
			throw new NotImplementedException();
		}

		internal override bool CalculateAggregate(string aggregateName)
		{
			Global.Tracer.Assert(!m_odpContext.IsPageHeaderFooter, "Not supported for page header/footer in streaming mode");
			OnDemandProcessingContext odpWorkerContextForTablixProcessing = GetOdpWorkerContextForTablixProcessing();
			odpWorkerContextForTablixProcessing.ReportAggregates.TryGetValue(aggregateName, out Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo value);
			if (value == null)
			{
				return false;
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = m_odpContext.ReportDefinition.MappingDataSetIndexToDataSet[value.DataSetIndexInCollection];
			if (!odpWorkerContextForTablixProcessing.IsTablixProcessingComplete(dataSet.IndexInCollection))
			{
				if (odpWorkerContextForTablixProcessing.IsTablixProcessingMode)
				{
					return false;
				}
				DataSetAggregateDataPipelineManager pipeline = new DataSetAggregateDataPipelineManager(odpWorkerContextForTablixProcessing, dataSet);
				odpWorkerContextForTablixProcessing.OnDemandProcessDataPipelineWithRestore(pipeline);
			}
			return true;
		}

		internal override bool CalculateLookup(LookupInfo lookup)
		{
			Global.Tracer.Assert(condition: false, "Lookup functions are not supported in this execution mode.");
			throw new NotImplementedException();
		}

		internal override bool PrepareFieldsCollectionForDirectFields()
		{
			Global.Tracer.Assert(condition: false, "The fields collection should already be setup for Streaming ODP Mode");
			throw new NotImplementedException();
		}

		internal override void EvaluateScopedFieldReference(string scopeName, int fieldIndex, ref Microsoft.ReportingServices.RdlExpressions.VariantResult result)
		{
			Global.Tracer.Assert(m_lastRIFObject != null, "The RIF object for the current scope should be present.");
			try
			{
				if (!m_odpContext.ReportDefinition.MappingNameToDataSet.TryGetValue(scopeName, out Microsoft.ReportingServices.ReportIntermediateFormat.DataSet value))
				{
					throw new ReportProcessingException_NonExistingScopeReference(scopeName);
				}
				if (!TryGetNonStructuralIdcDataManager(value, out NonStructuralIdcDataManager nsIdcDataManager))
				{
					nsIdcDataManager = CreateNonStructuralIdcDataManager(scopeName, value);
				}
				if (nsIdcDataManager.SourceDataScope.CurrentStreamingScopeInstance != nsIdcDataManager.LastParentScopeInstance)
				{
					nsIdcDataManager.RegisterActiveParent(nsIdcDataManager.SourceDataScope.CurrentStreamingScopeInstance);
					nsIdcDataManager.Advance();
				}
				else
				{
					nsIdcDataManager.SetupEnvironment();
				}
				m_odpContext.ReportRuntime.EvaluateSimpleFieldReference(fieldIndex, ref result);
			}
			finally
			{
				SetupEnvironment(m_lastRIFObject, m_lastOnDemandScopeInstance.Value(), m_lastOnDemandScopeInstance);
			}
		}

		private NonStructuralIdcDataManager CreateNonStructuralIdcDataManager(string scopeName, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet targetDataSet)
		{
			if (!DataScopeInfo.TryGetInnermostParentScopeRelatedToTargetDataSet(targetDataSet, m_lastRIFObject, out IRIFReportDataScope targetScope))
			{
				throw new ReportProcessingException_InvalidScopeReference(scopeName);
			}
			NonStructuralIdcDataManager nonStructuralIdcDataManager = new NonStructuralIdcDataManager(m_odpContext, targetDataSet, targetScope);
			RegisterDisposableDataReaderOrIdcDataManager(nonStructuralIdcDataManager);
			AddNonStructuralIdcDataManager(targetDataSet, nonStructuralIdcDataManager);
			return nonStructuralIdcDataManager;
		}

		internal override void RestoreContext(IInstancePath originalObject)
		{
			if (originalObject != null && m_odpContext.ReportRuntime.ContextUpdated && m_lastRIFObject != originalObject)
			{
				SetupObjectModels((IRIFReportDataScope)originalObject, -1);
			}
		}

		internal override void SetupContext(IInstancePath rifObject, IReportScopeInstance romInstance)
		{
			SetupContext(rifObject, romInstance, -1);
		}

		internal override void SetupContext(IInstancePath rifObject, IReportScopeInstance romInstance, int moveNextInstanceIndex)
		{
			m_lastROMInstance = romInstance;
			IRIFReportDataScope iRIFReportDataScope = romInstance.ReportScope.RIFReportScope as IRIFReportDataScope;
			if (iRIFReportDataScope != null && (m_lastOnDemandScopeInstance == null || iRIFReportDataScope.CurrentStreamingScopeInstance != m_lastOnDemandScopeInstance))
			{
				SetupObjectModels(iRIFReportDataScope, moveNextInstanceIndex);
			}
		}

		private void SetupObjectModels(IRIFReportDataScope reportDataScope, int moveNextInstanceIndex)
		{
			_ = m_odpContext.ReportDefinition;
			m_odpContext.EnsureCultureIsSetOnCurrentThread();
			EnsureScopeIsBound(reportDataScope);
			if (m_lastOnDemandScopeInstance != reportDataScope.CurrentStreamingScopeInstance)
			{
				SetupEnvironment(reportDataScope, reportDataScope.CurrentStreamingScopeInstance.Value(), reportDataScope.CurrentStreamingScopeInstance);
			}
		}

		private void EnsureScopeIsBound(IRIFReportDataScope reportDataScope)
		{
			BindScopeToInstance(reportDataScope);
			if (!reportDataScope.IsBoundToStreamingScopeInstance && CanBindOrProcessIndividually(reportDataScope) && TryProcessToNextScopeInstance(reportDataScope))
			{
				BindScopeToInstance(reportDataScope);
			}
			if (!reportDataScope.IsBoundToStreamingScopeInstance)
			{
				reportDataScope.BindToNoRowsScopeInstance(m_odpContext);
			}
		}

		private void SetupEnvironment(IRIFReportDataScope reportDataScope, IOnDemandScopeInstance scopeInst, IReference<IOnDemandScopeInstance> scopeInstRef)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = reportDataScope.DataScopeInfo.DataSet;
			if (m_odpContext.CurrentDataSetIndex != dataSet.IndexInCollection)
			{
				m_odpContext.SetupFieldsForNewDataSet(dataSet, m_odpContext.GetDataSetInstance(dataSet), addRowIndex: true, noRows: false);
			}
			scopeInst.SetupEnvironment();
			m_lastOnDemandScopeInstance = scopeInstRef;
			m_lastRIFObject = reportDataScope;
		}

		private void BindScopeToInstance(IRIFReportDataScope reportDataScope)
		{
			if (reportDataScope.IsBoundToStreamingScopeInstance)
			{
				return;
			}
			if (!reportDataScope.IsScope)
			{
				IRIFReportDataScope parentReportScope = reportDataScope.ParentReportScope;
				EnsureScopeIsBound(parentReportScope);
				reportDataScope.BindToStreamingScopeInstance(parentReportScope.CurrentStreamingScopeInstance);
				return;
			}
			switch (reportDataScope.InstancePathItem.Type)
			{
			case InstancePathItemType.Cell:
				if (reportDataScope.IsDataIntersectionScope)
				{
					IRIFReportIntersectionScope iRIFReportIntersectionScope = (IRIFReportIntersectionScope)reportDataScope;
					IRIFReportDataScope parentRowReportScope = iRIFReportIntersectionScope.ParentRowReportScope;
					if (!TryBindParentScope(reportDataScope, parentRowReportScope, out IReference<IOnDemandMemberInstance> parentScopeInst2))
					{
						break;
					}
					IRIFReportDataScope parentColumnReportScope = iRIFReportIntersectionScope.ParentColumnReportScope;
					if (!TryBindParentScope(reportDataScope, parentColumnReportScope, out IReference<IOnDemandMemberInstance> parentScopeInst3))
					{
						break;
					}
					IReference<IOnDemandMemberInstance> reference;
					IReference<IOnDemandMemberInstance> reference2;
					if (!iRIFReportIntersectionScope.IsColumnOuterGrouping)
					{
						reference = parentScopeInst2;
						reference2 = parentScopeInst3;
					}
					else
					{
						reference = parentScopeInst3;
						reference2 = parentScopeInst2;
					}
					CheckForPrematureScopeInstance(reportDataScope);
					IReference<IOnDemandScopeInstance> cellRef;
					IOnDemandScopeInstance cellInstance = SyntheticTriangulatedCellReference.GetCellInstance(reference, reference2, out cellRef);
					if (cellInstance == null && iRIFReportIntersectionScope.DataScopeInfo.NeedsIDC && TryProcessToCreateCell(iRIFReportIntersectionScope, (RuntimeDataTablixGroupLeafObjReference)reference2, (RuntimeDataTablixGroupLeafObjReference)reference))
					{
						cellInstance = SyntheticTriangulatedCellReference.GetCellInstance(reference, reference2, out cellRef);
					}
					if (cellInstance != null)
					{
						if (cellRef == null)
						{
							iRIFReportIntersectionScope.BindToStreamingScopeInstance(reference, reference2);
							SetupEnvironment(reportDataScope, cellInstance, iRIFReportIntersectionScope.CurrentStreamingScopeInstance);
						}
						else
						{
							reportDataScope.BindToStreamingScopeInstance(cellRef);
						}
					}
				}
				else
				{
					Global.Tracer.Assert(condition: false, "Non-intersection cell scopes are not yet supported by streaming ODP.");
				}
				break;
			case InstancePathItemType.ColumnMemberInstanceIndexTopMost:
			case InstancePathItemType.ColumnMemberInstanceIndex:
			case InstancePathItemType.RowMemberInstanceIndex:
			{
				IRIFReportDataScope parentReportScope3 = reportDataScope.ParentReportScope;
				if (!TryBindParentScope(reportDataScope, parentReportScope3, out IReference<IOnDemandMemberOwnerInstance> parentScopeInst4))
				{
					break;
				}
				CheckForPrematureScopeInstance(reportDataScope);
				using (parentScopeInst4.PinValue())
				{
					IOnDemandMemberOwnerInstance onDemandMemberOwnerInstance = parentScopeInst4.Value();
					Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode rifMember = (Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode)reportDataScope;
					IOnDemandMemberInstanceReference firstMemberInstance = onDemandMemberOwnerInstance.GetFirstMemberInstance(rifMember);
					if (RequiresIdcProcessing(reportDataScope, firstMemberInstance, (IReference<IOnDemandScopeInstance>)parentScopeInst4))
					{
						firstMemberInstance = onDemandMemberOwnerInstance.GetFirstMemberInstance(rifMember);
					}
					reportDataScope.BindToStreamingScopeInstance(firstMemberInstance);
				}
				break;
			}
			case InstancePathItemType.DataRegion:
			{
				IRIFReportDataScope parentReportScope2 = reportDataScope.ParentReportScope;
				Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = (Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion)reportDataScope;
				if (parentReportScope2 == null)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = dataRegion.DataScopeInfo.DataSet;
					DataPipelineManager orCreatePipelineManager = GetOrCreatePipelineManager(dataSet, dataRegion);
					reportDataScope.BindToStreamingScopeInstance(orCreatePipelineManager.GroupTreeRoot.GetDataRegionInstance(dataRegion));
				}
				else
				{
					if (!TryBindParentScope(reportDataScope, parentReportScope2, out IReference<IOnDemandScopeInstance> parentScopeInst))
					{
						break;
					}
					CheckForPrematureScopeInstance(reportDataScope);
					using (parentScopeInst.PinValue())
					{
						IOnDemandScopeInstance onDemandScopeInstance = parentScopeInst.Value();
						IReference<IOnDemandScopeInstance> dataRegionInstance = onDemandScopeInstance.GetDataRegionInstance(dataRegion);
						if (RequiresIdcProcessing(reportDataScope, dataRegionInstance, parentScopeInst))
						{
							dataRegionInstance = onDemandScopeInstance.GetDataRegionInstance(dataRegion);
						}
						reportDataScope.BindToStreamingScopeInstance(dataRegionInstance);
					}
				}
				break;
			}
			default:
				Global.Tracer.Assert(false, "SetupObjectModels cannot handle IRIFReportDataScope of type: {0}", Enum.GetName(typeof(InstancePathItemType), reportDataScope.InstancePathItem.Type));
				break;
			}
		}

		private bool RequiresIdcProcessing(IRIFReportDataScope reportDataScope, IReference<IOnDemandScopeInstance> scopeInstanceRef, IReference<IOnDemandScopeInstance> parentScopeInstanceRef)
		{
			if (reportDataScope.DataScopeInfo.NeedsIDC)
			{
				if (scopeInstanceRef == null)
				{
					RegisterParentForIdc(reportDataScope, parentScopeInstanceRef);
					return TryProcessToNextScopeInstance(reportDataScope);
				}
				IOnDemandScopeInstance onDemandScopeInstance = scopeInstanceRef.Value();
				if (onDemandScopeInstance.IsNoRows && onDemandScopeInstance.IsMostRecentlyCreatedScopeInstance)
				{
					RegisterParentForIdc(reportDataScope, parentScopeInstanceRef);
					return ProcessOneRow(reportDataScope);
				}
			}
			return false;
		}

		private void RegisterParentForIdc(IRIFReportDataScope reportDataScope, IReference<IOnDemandScopeInstance> parentScopeInstanceRef)
		{
			((IdcDataManager)GetOrCreateIdcDataManager(reportDataScope)).RegisterActiveParent(parentScopeInstanceRef);
		}

		internal override bool CheckForPrematureServerAggregate(string aggregateName)
		{
			IRIFReportDataScope iRIFReportDataScope = m_lastRIFObject;
			while (iRIFReportDataScope != null && !iRIFReportDataScope.IsScope)
			{
				iRIFReportDataScope = iRIFReportDataScope.ParentReportScope;
			}
			if (iRIFReportDataScope == null || !iRIFReportDataScope.IsBoundToStreamingScopeInstance)
			{
				return false;
			}
			if (NeedsDataForServerAggregate(iRIFReportDataScope))
			{
				AdvanceDataPipeline(iRIFReportDataScope, PipelineAdvanceMode.ToFulfillServerAggregate);
				SetupEnvironment(iRIFReportDataScope, iRIFReportDataScope.CurrentStreamingScopeInstance.Value(), iRIFReportDataScope.CurrentStreamingScopeInstance);
				return true;
			}
			return false;
		}

		internal static bool NeedsDataForServerAggregate(IRIFReportDataScope reportDataScope)
		{
			IOnDemandScopeInstance onDemandScopeInstance = reportDataScope.CurrentStreamingScopeInstance.Value();
			if (!onDemandScopeInstance.IsNoRows && onDemandScopeInstance.IsMostRecentlyCreatedScopeInstance)
			{
				return onDemandScopeInstance.HasUnProcessedServerAggregate;
			}
			return false;
		}

		private void CheckForPrematureScopeInstance(IRIFReportDataScope reportDataScope)
		{
			if (!CanBindOrProcessIndividually(reportDataScope) && !reportDataScope.DataScopeInfo.NeedsIDC && !reportDataScope.IsDataIntersectionScope && !DataScopeInfo.HasDecomposableAncestorWithNonLatestInstanceBinding(reportDataScope))
			{
				TryProcessToNextScopeInstance(reportDataScope);
			}
		}

		private bool TryBindParentScope<T>(IRIFReportDataScope reportDataScope, IRIFReportDataScope parentReportDataScope, out IReference<T> parentScopeInst) where T : IOnDemandScopeInstance
		{
			EnsureScopeIsBound(parentReportDataScope);
			parentScopeInst = (IReference<T>)parentReportDataScope.CurrentStreamingScopeInstance;
			if (parentScopeInst.Value().IsNoRows)
			{
				reportDataScope.BindToNoRowsScopeInstance(m_odpContext);
				return false;
			}
			return true;
		}

		private DataPipelineManager GetOrCreatePipelineManager(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, IRIFReportDataScope targetScope)
		{
			if (m_pipelineManager != null)
			{
				if (m_pipelineManager.DataSetIndex == dataSet.IndexInCollection)
				{
					return m_pipelineManager;
				}
				if (m_odpContext.IsTablixProcessingComplete(dataSet.IndexInCollection))
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Performance: While rendering the report: '{0}' the data set {1} was processed multiple times due to rendering traversal order.", m_odpContext.ReportContext.ItemPathAsString.MarkAsPrivate(), dataSet.Name.MarkAsPrivate());
				}
				CleanupPipelineManager();
				ShutdownSequentialReadersAndIdcDataManagers();
			}
			if (dataSet.AllowIncrementalProcessing)
			{
				m_pipelineManager = new IncrementalDataPipelineManager(m_odpContext, dataSet);
			}
			else
			{
				m_pipelineManager = new StreamingAtomicDataPipelineManager(m_odpContext, dataSet);
			}
			m_pipelineThrottle = new DataPipelineThrottle();
			m_pipelineThrottle.StartUsingContext(PipelineAdvanceMode.ToStoppingScopeInstance, targetScope);
			m_pipelineManager.StartProcessing();
			m_pipelineThrottle.StopUsingContext();
			TryProcessToNextScopeInstance(targetScope);
			return m_pipelineManager;
		}

		private void CleanupPipelineManager()
		{
			if (m_pipelineManager != null)
			{
				m_pipelineManager.StopProcessing();
				m_pipelineManager = null;
			}
		}

		internal override IRecordRowReader CreateSequentialDataReader(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, out Microsoft.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance)
		{
			LiveRecordRowReader liveRecordRowReader = new LiveRecordRowReader(dataSet, m_odpContext);
			dataSetInstance = liveRecordRowReader.DataSetInstance;
			RegisterDisposableDataReaderOrIdcDataManager(liveRecordRowReader);
			return liveRecordRowReader;
		}

		private bool TryProcessToCreateCell(IRIFReportDataScope reportDataScope, RuntimeDataTablixGroupLeafObjReference columnGroupLeafRef, RuntimeDataTablixGroupLeafObjReference rowGroupLeafRef)
		{
			((CellIdcDataManager)GetOrCreateIdcDataManager(reportDataScope)).RegisterActiveIntersection(columnGroupLeafRef, rowGroupLeafRef);
			return TryProcessToNextScopeInstance(reportDataScope);
		}

		private bool TryProcessToNextScopeInstance(IRIFReportDataScope reportDataScope)
		{
			return AdvanceDataPipeline(reportDataScope, PipelineAdvanceMode.ToStoppingScopeInstance);
		}

		private bool AdvanceDataPipeline(IRIFReportDataScope reportDataScope, PipelineAdvanceMode pipelineMode)
		{
			m_lastOnDemandScopeInstance = null;
			DataPipelineThrottle pipelineThrottle = SetupUsablePipelineContextWithBackup();
			m_pipelineThrottle.StartUsingContext(pipelineMode, reportDataScope);
			bool isTablixProcessingMode = m_odpContext.IsTablixProcessingMode;
			bool num = m_odpContext.ExecutionLogContext.TryStartTablixProcessingTimer();
			m_odpContext.IsTablixProcessingMode = true;
			if (reportDataScope.DataScopeInfo.DataPipelineID != m_pipelineManager.DataSetIndex)
			{
				m_odpContext.StateManager.GetIdcDataManager(reportDataScope).Advance();
			}
			else
			{
				m_pipelineManager.Advance();
			}
			m_odpContext.IsTablixProcessingMode = isTablixProcessingMode;
			if (num)
			{
				m_odpContext.ExecutionLogContext.StopTablixProcessingTimer();
			}
			bool result = m_pipelineThrottle.StopUsingContext();
			m_pipelineThrottle = pipelineThrottle;
			return result;
		}

		private DataPipelineThrottle SetupUsablePipelineContextWithBackup()
		{
			if (m_pipelineThrottle == null)
			{
				m_pipelineThrottle = new DataPipelineThrottle();
			}
			DataPipelineThrottle pipelineThrottle = m_pipelineThrottle;
			if (m_pipelineThrottle.InUse)
			{
				if (m_pipelineThrottle2 == null)
				{
					m_pipelineThrottle2 = new DataPipelineThrottle();
				}
				if (m_pipelineThrottle2.InUse)
				{
					m_pipelineThrottle = new DataPipelineThrottle();
					return pipelineThrottle;
				}
				m_pipelineThrottle = m_pipelineThrottle2;
			}
			return pipelineThrottle;
		}

		internal override bool ProcessOneRow(IRIFReportDataScope scope)
		{
			return AdvanceDataPipeline(scope, PipelineAdvanceMode.ByOneRow);
		}

		private void AbortHandler(object sender, EventArgs e)
		{
			if (m_pipelineManager != null)
			{
				m_pipelineManager.Abort();
			}
		}

		internal override void FreeResources()
		{
			if (m_abortProcessor != null)
			{
				m_odpContext.AbortInfo.ProcessingAbortEvent -= m_abortProcessor;
				m_abortProcessor = null;
			}
			base.FreeResources();
			CleanupPipelineManager();
			CleanupQueryCache();
		}

		private void CleanupQueryCache()
		{
			if (m_executedQueryCache != null)
			{
				m_executedQueryCache.Close();
			}
		}

		internal override void BindNextMemberInstance(IInstancePath rifObject, IReportScopeInstance romInstance, int moveNextInstanceIndex)
		{
			IRIFReportDataScope iRIFReportDataScope = romInstance.ReportScope.RIFReportScope as IRIFReportDataScope;
			IReference<IOnDemandMemberInstance> reference = iRIFReportDataScope.CurrentStreamingScopeInstance as IReference<IOnDemandMemberInstance>;
			if (reference.Value().IsNoRows)
			{
				return;
			}
			IDisposable disposable = reference.PinValue();
			IOnDemandMemberInstance onDemandMemberInstance = reference.Value();
			iRIFReportDataScope.BindToStreamingScopeInstance(onDemandMemberInstance.GetNextMemberInstance());
			if (!iRIFReportDataScope.IsBoundToStreamingScopeInstance && CanBindOrProcessIndividually(iRIFReportDataScope) && onDemandMemberInstance.IsMostRecentlyCreatedScopeInstance)
			{
				IdcDataManager idcDataManager = null;
				if (iRIFReportDataScope.DataScopeInfo.NeedsIDC)
				{
					idcDataManager = (IdcDataManager)GetIdcDataManager(iRIFReportDataScope);
					List<object> groupExprValues = onDemandMemberInstance.GroupExprValues;
					List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo> groupExpressions = ((Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode)iRIFReportDataScope).Grouping.GroupExpressions;
					idcDataManager.SetSkippingFilter(groupExpressions, groupExprValues);
				}
				if (TryProcessToNextScopeInstance(iRIFReportDataScope))
				{
					iRIFReportDataScope.BindToStreamingScopeInstance(onDemandMemberInstance.GetNextMemberInstance());
				}
				idcDataManager?.ClearSkippingFilter();
			}
			if (!iRIFReportDataScope.IsBoundToStreamingScopeInstance)
			{
				iRIFReportDataScope.BindToNoRowsScopeInstance(m_odpContext);
			}
			SetupEnvironment(iRIFReportDataScope, iRIFReportDataScope.CurrentStreamingScopeInstance.Value(), iRIFReportDataScope.CurrentStreamingScopeInstance);
			disposable.Dispose();
		}

		internal override bool ShouldStopPipelineAdvance(bool rowAccepted)
		{
			return m_pipelineThrottle.ShouldStopPipelineAdvance(rowAccepted);
		}

		internal override void CreatedScopeInstance(IRIFReportDataScope scope)
		{
			m_pipelineThrottle.CreatedScopeInstance(scope);
		}

		internal static bool CanBindOrProcessIndividually(IRIFReportDataScope scope)
		{
			return scope.DataScopeInfo.IsDecomposable;
		}
	}
}

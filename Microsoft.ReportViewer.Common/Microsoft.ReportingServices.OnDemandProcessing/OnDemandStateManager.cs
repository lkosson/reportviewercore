using Microsoft.ReportingServices.Common;
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
	internal abstract class OnDemandStateManager
	{
		protected readonly OnDemandProcessingContext m_odpContext;

		private List<IDisposable> m_sequentialDataReadersAndIdcDataManagers;

		private BaseIdcDataManager[] m_idcDataManagers;

		internal abstract IReportScopeInstance LastROMInstance
		{
			get;
		}

		internal abstract IRIFReportScope LastTablixProcessingReportScope
		{
			get;
			set;
		}

		internal abstract IInstancePath LastRIFObject
		{
			get;
			set;
		}

		internal abstract QueryRestartInfo QueryRestartInfo
		{
			get;
		}

		internal abstract ExecutedQueryCache ExecutedQueryCache
		{
			get;
		}

		public OnDemandStateManager(OnDemandProcessingContext odpContext)
		{
			m_odpContext = odpContext;
		}

		internal abstract ExecutedQueryCache SetupExecutedQueryCache();

		internal abstract void ResetOnDemandState();

		internal abstract int RecursiveLevel(string scopeName);

		internal abstract bool InScope(string scopeName);

		internal abstract Dictionary<string, object> GetCurrentSpecialGroupingValues();

		internal abstract void RestoreContext(IInstancePath originalObject);

		internal abstract void SetupContext(IInstancePath rifObject, IReportScopeInstance romInstance);

		internal abstract void SetupContext(IInstancePath rifObject, IReportScopeInstance romInstance, int moveNextInstanceIndex);

		internal abstract void BindNextMemberInstance(IInstancePath rifObject, IReportScopeInstance romInstance, int moveNextInstanceIndex);

		internal abstract bool CalculateAggregate(string aggregateName);

		internal abstract bool CalculateLookup(LookupInfo lookup);

		internal abstract bool PrepareFieldsCollectionForDirectFields();

		internal abstract void EvaluateScopedFieldReference(string scopeName, int fieldIndex, ref Microsoft.ReportingServices.RdlExpressions.VariantResult result);

		internal abstract IRecordRowReader CreateSequentialDataReader(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, out Microsoft.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance);

		internal abstract bool ShouldStopPipelineAdvance(bool rowAccepted);

		internal abstract void CreatedScopeInstance(IRIFReportDataScope scope);

		internal virtual void FreeResources()
		{
			ShutdownSequentialReadersAndIdcDataManagers();
		}

		protected OnDemandProcessingContext GetOdpWorkerContextForTablixProcessing()
		{
			if (m_odpContext.IsPageHeaderFooter)
			{
				return m_odpContext.ParentContext;
			}
			return m_odpContext;
		}

		protected void ShutdownSequentialReadersAndIdcDataManagers()
		{
			if (m_sequentialDataReadersAndIdcDataManagers == null)
			{
				return;
			}
			for (int i = 0; i < m_sequentialDataReadersAndIdcDataManagers.Count; i++)
			{
				try
				{
					m_sequentialDataReadersAndIdcDataManagers[i].Dispose();
				}
				catch (ReportProcessingException ex)
				{
					if (ex.InnerException != null && AsynchronousExceptionDetection.IsStoppingException(ex.InnerException))
					{
						throw;
					}
					Global.Tracer.Trace(TraceLevel.Error, "Error cleaning up request: {0}", ex);
				}
			}
			m_sequentialDataReadersAndIdcDataManagers = null;
			m_idcDataManagers = null;
		}

		protected void RegisterDisposableDataReaderOrIdcDataManager(IDisposable dataReaderOrIdcDataManager)
		{
			if (m_sequentialDataReadersAndIdcDataManagers == null)
			{
				m_sequentialDataReadersAndIdcDataManagers = new List<IDisposable>();
			}
			m_sequentialDataReadersAndIdcDataManagers.Add(dataReaderOrIdcDataManager);
		}

		internal abstract bool CheckForPrematureServerAggregate(string aggregateName);

		internal abstract bool ProcessOneRow(IRIFReportDataScope scope);

		protected BaseIdcDataManager GetOrCreateIdcDataManager(IRIFReportDataScope scope)
		{
			if (!TryGetIdcDataManager(scope, out BaseIdcDataManager idcDataManager))
			{
				idcDataManager = ((!scope.IsDataIntersectionScope) ? ((BaseIdcDataManager)new IdcDataManager(m_odpContext, scope)) : ((BaseIdcDataManager)new CellIdcDataManager(m_odpContext, scope)));
				RegisterDisposableDataReaderOrIdcDataManager(idcDataManager);
				AddIdcDataManager(scope, idcDataManager);
			}
			return idcDataManager;
		}

		private bool TryGetIdcDataManager(IRIFReportDataScope scope, out BaseIdcDataManager idcDataManager)
		{
			return TryGetIdcDataManager(scope.DataScopeInfo.DataPipelineID, out idcDataManager);
		}

		protected bool TryGetNonStructuralIdcDataManager(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet targetDataSet, out NonStructuralIdcDataManager nsIdcDataManager)
		{
			if (TryGetIdcDataManager(targetDataSet.IndexInCollection, out BaseIdcDataManager idcDataManager))
			{
				nsIdcDataManager = (NonStructuralIdcDataManager)idcDataManager;
				return true;
			}
			nsIdcDataManager = null;
			return false;
		}

		private bool TryGetIdcDataManager(int dataPipelineId, out BaseIdcDataManager idcDataManager)
		{
			if (m_idcDataManagers == null)
			{
				idcDataManager = null;
				return false;
			}
			idcDataManager = m_idcDataManagers[dataPipelineId];
			return idcDataManager != null;
		}

		protected void AddNonStructuralIdcDataManager(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet targetDataSet, NonStructuralIdcDataManager idcDataManager)
		{
			AddIdcDataManager(targetDataSet.IndexInCollection, idcDataManager);
		}

		private void AddIdcDataManager(IRIFReportDataScope scope, BaseIdcDataManager idcDataManager)
		{
			AddIdcDataManager(scope.DataScopeInfo.DataPipelineID, idcDataManager);
		}

		private void AddIdcDataManager(int dataPipelineId, BaseIdcDataManager idcDataManager)
		{
			if (m_idcDataManagers == null)
			{
				m_idcDataManagers = new BaseIdcDataManager[m_odpContext.ReportDefinition.DataPipelineCount];
			}
			m_idcDataManagers[dataPipelineId] = idcDataManager;
		}

		internal BaseIdcDataManager GetIdcDataManager(IRIFReportDataScope scope)
		{
			if (!TryGetIdcDataManager(scope, out BaseIdcDataManager idcDataManager))
			{
				Global.Tracer.Assert(condition: false, "Missing expected IDCDataManager.");
			}
			return idcDataManager;
		}
	}
}

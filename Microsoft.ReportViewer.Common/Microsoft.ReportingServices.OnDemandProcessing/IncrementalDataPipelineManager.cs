using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class IncrementalDataPipelineManager : DataPipelineManager
	{
		private RuntimeDataSourceIncrementalDataProcessing m_runtimeDataSource;

		public override IOnDemandScopeInstance GroupTreeRoot => m_runtimeDataSource.GroupTreeRoot;

		protected override RuntimeDataSource RuntimeDataSource => m_runtimeDataSource;

		public IncrementalDataPipelineManager(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
			: base(odpContext, dataSet)
		{
		}

		protected override void InternalStartProcessing()
		{
			Global.Tracer.Assert(m_runtimeDataSource == null, "Cannot StartProcessing twice for the same pipeline manager");
			m_runtimeDataSource = new RuntimeDataSourceIncrementalDataProcessing(m_dataSet, m_odpContext);
			m_runtimeDataSource.Initialize();
		}

		protected override void InternalStopProcessing()
		{
			if (m_runtimeDataSource != null)
			{
				m_runtimeDataSource.Teardown();
				m_odpContext.ReportRuntime.CurrentScope = null;
			}
		}

		public override void Abort()
		{
			base.Abort();
			if (m_runtimeDataSource != null)
			{
				m_runtimeDataSource.Abort();
			}
		}

		public override void Advance()
		{
			m_runtimeDataSource.Advance();
		}
	}
}

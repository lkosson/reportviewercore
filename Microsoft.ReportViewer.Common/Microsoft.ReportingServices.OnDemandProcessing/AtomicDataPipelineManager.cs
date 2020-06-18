using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal abstract class AtomicDataPipelineManager : DataPipelineManager
	{
		private RuntimeDataSourceDataProcessing m_runtimeDataSource;

		public override IOnDemandScopeInstance GroupTreeRoot => m_runtimeDataSource.RuntimeDataSet.GroupTreeRoot;

		protected override RuntimeDataSource RuntimeDataSource => m_runtimeDataSource;

		public AtomicDataPipelineManager(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
			: base(odpContext, dataSet)
		{
		}

		protected override void InternalStartProcessing()
		{
			Global.Tracer.Assert(m_runtimeDataSource == null, "Cannot StartProcessing twice for the same pipeline manager");
			m_runtimeDataSource = CreateDataSource();
			m_runtimeDataSource.ProcessSingleOdp();
			m_odpContext.CheckAndThrowIfAborted();
		}

		protected abstract RuntimeDataSourceDataProcessing CreateDataSource();

		protected override void InternalStopProcessing()
		{
			m_runtimeDataSource = null;
		}

		public override void Advance()
		{
		}
	}
}

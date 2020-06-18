using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class FullAtomicDataPipelineManager : AtomicDataPipelineManager
	{
		public FullAtomicDataPipelineManager(OnDemandProcessingContext odpContext, DataSet dataSet)
			: base(odpContext, dataSet)
		{
		}

		protected override RuntimeDataSourceDataProcessing CreateDataSource()
		{
			return new RuntimeDataSourceFullDataProcessing(m_dataSet, m_odpContext);
		}
	}
}

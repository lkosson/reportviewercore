using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class StreamingAtomicDataPipelineManager : AtomicDataPipelineManager
	{
		public StreamingAtomicDataPipelineManager(OnDemandProcessingContext odpContext, DataSet dataSet)
			: base(odpContext, dataSet)
		{
		}

		protected override RuntimeDataSourceDataProcessing CreateDataSource()
		{
			return new RuntimeDataSourceStreamingDataProcessing(m_dataSet, m_odpContext);
		}

		public override void Abort()
		{
			base.Abort();
			if (RuntimeDataSource != null)
			{
				RuntimeDataSource.Abort();
			}
		}
	}
}

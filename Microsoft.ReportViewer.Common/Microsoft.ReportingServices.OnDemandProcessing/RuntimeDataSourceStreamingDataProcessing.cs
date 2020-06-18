using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeDataSourceStreamingDataProcessing : RuntimeDataSourceDataProcessing
	{
		internal RuntimeDataSourceStreamingDataProcessing(DataSet dataSet, OnDemandProcessingContext processingContext)
			: base(dataSet, processingContext)
		{
		}

		protected override RuntimeOnDemandDataSet CreateRuntimeDataSet()
		{
			DataSetInstance dataSetInstance = new DataSetInstance(m_dataSet);
			m_odpContext.CurrentReportInstance.SetDataSetInstance(dataSetInstance);
			return new RuntimeOnDemandDataSet(base.DataSourceDefinition, m_dataSet, dataSetInstance, m_odpContext, processFromLiveDataReader: true, generateGroupTree: false, canWriteDataChunk: false);
		}
	}
}

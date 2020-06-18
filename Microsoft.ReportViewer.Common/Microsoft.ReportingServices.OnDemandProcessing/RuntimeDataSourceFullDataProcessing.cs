using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeDataSourceFullDataProcessing : RuntimeDataSourceDataProcessing
	{
		protected override bool NeedsExecutionLogging => false;

		internal RuntimeDataSourceFullDataProcessing(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, OnDemandProcessingContext processingContext)
			: base(dataSet, processingContext)
		{
		}

		protected override RuntimeOnDemandDataSet CreateRuntimeDataSet()
		{
			OnDemandProcessingContext odpContext = base.OdpContext;
			DataSetInstance dataSetInstance = odpContext.CurrentReportInstance.GetDataSetInstance(m_dataSet, odpContext);
			if (odpContext.IsTablixProcessingComplete(m_dataSet.IndexInCollection))
			{
				Global.Tracer.Trace(TraceLevel.Warning, "Tablix processing is being attempted multiple times on DataSet '{0}'.", m_dataSet.Name.MarkAsPrivate());
			}
			return new RuntimeOnDemandDataSet(base.DataSourceDefinition, m_dataSet, dataSetInstance, odpContext, processFromLiveDataReader: false, generateGroupTree: true, canWriteDataChunk: true);
		}

		protected override void OpenInitialConnectionAndTransaction()
		{
		}
	}
}

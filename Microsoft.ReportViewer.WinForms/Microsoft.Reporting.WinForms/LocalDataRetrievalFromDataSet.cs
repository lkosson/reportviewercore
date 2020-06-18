using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;

namespace Microsoft.Reporting.WinForms
{
	internal class LocalDataRetrievalFromDataSet : LocalDataRetrieval
	{
		public delegate IEnumerable GetSubReportDataSetCallback(PreviewItemContext subReportContext, ParameterInfoCollection initialParameters);

		private GetSubReportDataSetCallback m_subreportDataCallback;

		public GetSubReportDataSetCallback SubReportDataSetCallback
		{
			set
			{
				m_subreportDataCallback = value;
			}
		}

		public override bool SupportsQueries => false;

		public override ProcessingContext CreateProcessingContext(PreviewItemContext itemContext, ParameterInfoCollection parameters, IEnumerable dataSources, RuntimeDataSourceInfoCollection dataSourceInfoColl, RuntimeDataSetInfoCollection dataSetInfoColl, SharedDataSetCompiler sharedDataSetCompiler, DatasourceCredentialsCollection credentials, ReportProcessing.OnDemandSubReportCallback subReportCallback, IGetResource getResourceFunction, IChunkFactory chunkFactory, ReportRuntimeSetup runtimeSetup, CreateAndRegisterStream createStreamCallback)
		{
			return new ProcessingContextForDataSets(itemContext, parameters, new DataSourceCollectionWrapper((ReportDataSourceCollection)dataSources), subReportCallback, m_subreportDataCallback, getResourceFunction, chunkFactory, runtimeSetup, createStreamCallback);
		}
	}
}

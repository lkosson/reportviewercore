using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;

namespace Microsoft.Reporting
{
	internal abstract class LocalDataRetrieval
	{
		public abstract bool SupportsQueries
		{
			get;
		}

		public abstract ProcessingContext CreateProcessingContext(PreviewItemContext itemContext, ParameterInfoCollection parameters, IEnumerable dataSources, RuntimeDataSourceInfoCollection dataSourceInfoColl, RuntimeDataSetInfoCollection dataSetInfoColl, SharedDataSetCompiler sharedDataSetCompiler, DatasourceCredentialsCollection credentials, ReportProcessing.OnDemandSubReportCallback subReportCallback, IGetResource getResourceFunction, IChunkFactory chunkFactory, ReportRuntimeSetup runtimeSetup, CreateAndRegisterStream createStreamCallback);
	}
}

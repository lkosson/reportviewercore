using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.Reporting
{
	internal delegate DataSetPublishingResult SharedDataSetCompiler(DataSetInfo dataSetInfo, ICatalogItemContext dataSetContext);
}

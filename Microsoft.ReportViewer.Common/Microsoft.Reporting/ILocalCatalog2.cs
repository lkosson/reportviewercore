using Microsoft.ReportingServices.DataExtensions;

namespace Microsoft.Reporting
{
	internal interface ILocalCatalog2
	{
		bool GetReportDataSourceCredentials(PreviewItemContext itemContext, DataSourceInfo dataSourceInfo, out string userName, out string password);
	}
}

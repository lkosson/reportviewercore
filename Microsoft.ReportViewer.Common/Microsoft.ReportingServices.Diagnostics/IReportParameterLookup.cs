using System.Collections.Specialized;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal interface IReportParameterLookup
	{
		string GetReportParamsInstanceId(NameValueCollection reportParameters);
	}
}

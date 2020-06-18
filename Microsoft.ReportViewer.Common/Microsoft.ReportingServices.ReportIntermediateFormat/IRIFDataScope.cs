using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal interface IRIFDataScope
	{
		string Name
		{
			get;
		}

		DataScopeInfo DataScopeInfo
		{
			get;
		}

		ObjectType DataScopeObjectType
		{
			get;
		}
	}
}

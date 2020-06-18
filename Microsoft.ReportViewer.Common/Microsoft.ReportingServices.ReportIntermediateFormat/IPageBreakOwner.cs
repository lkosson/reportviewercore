using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal interface IPageBreakOwner
	{
		PageBreak PageBreak
		{
			get;
			set;
		}

		ObjectType ObjectType
		{
			get;
		}

		string ObjectName
		{
			get;
		}

		IInstancePath InstancePath
		{
			get;
		}
	}
}

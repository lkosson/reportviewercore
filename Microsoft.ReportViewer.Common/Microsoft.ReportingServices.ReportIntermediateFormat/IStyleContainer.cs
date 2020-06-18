using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal interface IStyleContainer
	{
		Style StyleClass
		{
			get;
		}

		IInstancePath InstancePath
		{
			get;
		}

		ObjectType ObjectType
		{
			get;
		}

		string Name
		{
			get;
		}
	}
}

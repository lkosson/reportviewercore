using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface IROMActionOwner
	{
		string UniqueName
		{
			get;
		}

		List<string> FieldsUsedInValueExpression
		{
			get;
		}
	}
}

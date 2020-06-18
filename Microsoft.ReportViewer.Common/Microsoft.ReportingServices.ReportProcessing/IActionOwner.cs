using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal interface IActionOwner
	{
		Action Action
		{
			get;
		}

		List<string> FieldsUsedInValueExpression
		{
			get;
			set;
		}
	}
}

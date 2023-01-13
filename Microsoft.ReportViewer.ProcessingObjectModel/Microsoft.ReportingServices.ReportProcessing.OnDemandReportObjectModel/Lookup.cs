using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public abstract class Lookup : MarshalByRefObject
	{
		public abstract object Value
		{
			get;
		}

		public abstract object[] Values
		{
			get;
		}
	}
}

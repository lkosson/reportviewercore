using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public abstract class Variables : MarshalByRefObject
	{
		public abstract Variable this[string key]
		{
			get;
		}
	}
}

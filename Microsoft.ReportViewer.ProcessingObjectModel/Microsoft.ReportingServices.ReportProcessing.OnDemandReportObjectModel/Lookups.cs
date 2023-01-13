using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public abstract class Lookups : MarshalByRefObject
	{
		public abstract Lookup this[string key]
		{
			get;
		}
	}
}

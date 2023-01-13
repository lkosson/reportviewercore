using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public abstract class Scopes : MarshalByRefObject
	{
		public abstract Scope this[string key]
		{
			get;
		}
	}
}

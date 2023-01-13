using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	public abstract class ReportItems : MarshalByRefObject
	{
		public abstract ReportItem this[string key]
		{
			get;
		}
	}
}

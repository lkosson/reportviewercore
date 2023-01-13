using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	public abstract class Parameters : MarshalByRefObject
	{
		public abstract Parameter this[string key]
		{
			get;
		}
	}
}

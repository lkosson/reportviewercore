using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	public abstract class Aggregates : MarshalByRefObject
	{
		public abstract object this[string key]
		{
			get;
		}
	}
}

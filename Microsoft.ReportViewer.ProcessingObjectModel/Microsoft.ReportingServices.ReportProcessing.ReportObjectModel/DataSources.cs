using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	public abstract class DataSources : MarshalByRefObject
	{
		public abstract DataSource this[string key]
		{
			get;
		}
	}
}

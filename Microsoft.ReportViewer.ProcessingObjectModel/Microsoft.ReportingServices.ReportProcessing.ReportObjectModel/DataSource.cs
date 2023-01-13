using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	public abstract class DataSource : MarshalByRefObject
	{
		public abstract string DataSourceReference
		{
			get;
		}

		public abstract string Type
		{
			get;
		}
	}
}

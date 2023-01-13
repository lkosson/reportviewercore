using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	public abstract class DataSets : MarshalByRefObject
	{
		public abstract DataSet this[string key]
		{
			get;
		}
	}
}

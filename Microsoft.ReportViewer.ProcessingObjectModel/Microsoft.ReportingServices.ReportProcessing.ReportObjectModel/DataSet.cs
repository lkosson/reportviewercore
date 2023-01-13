using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	public abstract class DataSet : MarshalByRefObject
	{
		public abstract string CommandText
		{
			get;
		}

		public abstract string RewrittenCommandText
		{
			get;
		}

		public abstract DateTime ExecutionTime
		{
			get;
		}
	}
}

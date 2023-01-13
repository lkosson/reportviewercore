using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public abstract class Variable : MarshalByRefObject
	{
		public abstract object Value
		{
			get;
			set;
		}

		public abstract bool Writable
		{
			get;
		}

		public abstract bool SetValue(object value);
	}
}

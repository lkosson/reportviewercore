using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	public abstract class ReportItem : MarshalByRefObject
	{
		public abstract object Value
		{
			get;
		}
	}
}

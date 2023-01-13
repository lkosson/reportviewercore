using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public abstract class TextRun : MarshalByRefObject
	{
		public abstract object Value
		{
			get;
		}
	}
}

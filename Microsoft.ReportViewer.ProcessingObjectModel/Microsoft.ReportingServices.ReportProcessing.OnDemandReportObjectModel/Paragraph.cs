using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public abstract class Paragraph : MarshalByRefObject
	{
		public abstract TextRuns TextRuns
		{
			get;
		}
	}
}

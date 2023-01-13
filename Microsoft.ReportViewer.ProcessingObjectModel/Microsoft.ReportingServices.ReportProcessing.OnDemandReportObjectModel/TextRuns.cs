using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public abstract class TextRuns : MarshalByRefObject
	{
		public abstract TextRun this[int index]
		{
			get;
		}
	}
}

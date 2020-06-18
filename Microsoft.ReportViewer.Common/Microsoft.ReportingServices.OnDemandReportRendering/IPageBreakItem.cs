using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface IPageBreakItem
	{
		[Obsolete("Use PageBreak.BreakLocation instead.")]
		PageBreakLocation PageBreakLocation
		{
			get;
		}

		PageBreak PageBreak
		{
			get;
		}
	}
}

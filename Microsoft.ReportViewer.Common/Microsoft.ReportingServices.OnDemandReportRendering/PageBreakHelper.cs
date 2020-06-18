namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class PageBreakHelper
	{
		internal static PageBreakLocation GetPageBreakLocation(bool pageBreakAtStart, bool pageBreakAtEnd)
		{
			if (pageBreakAtStart && pageBreakAtEnd)
			{
				return PageBreakLocation.StartAndEnd;
			}
			if (pageBreakAtStart)
			{
				return PageBreakLocation.Start;
			}
			if (pageBreakAtEnd)
			{
				return PageBreakLocation.End;
			}
			return PageBreakLocation.None;
		}

		internal static PageBreakLocation MergePageBreakLocations(PageBreakLocation outer, PageBreakLocation inner)
		{
			if (outer == inner)
			{
				return outer;
			}
			if (PageBreakLocation.StartAndEnd == outer || inner == PageBreakLocation.None)
			{
				return outer;
			}
			if (PageBreakLocation.StartAndEnd == inner || outer == PageBreakLocation.None)
			{
				return inner;
			}
			if (PageBreakLocation.End == outer && PageBreakLocation.Start == inner)
			{
				return PageBreakLocation.StartAndEnd;
			}
			if (PageBreakLocation.Start == outer && PageBreakLocation.End == inner)
			{
				return PageBreakLocation.StartAndEnd;
			}
			return PageBreakLocation.None;
		}

		internal static bool HasPageBreakAtStart(PageBreakLocation pageBreakLoc)
		{
			if (pageBreakLoc != PageBreakLocation.Start)
			{
				return pageBreakLoc == PageBreakLocation.StartAndEnd;
			}
			return true;
		}

		internal static bool HasPageBreakAtEnd(PageBreakLocation pageBreakLoc)
		{
			if (pageBreakLoc != PageBreakLocation.End)
			{
				return pageBreakLoc == PageBreakLocation.StartAndEnd;
			}
			return true;
		}
	}
}

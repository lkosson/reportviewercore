using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ParagraphFilteredStyle : Style
	{
		internal ParagraphFilteredStyle(Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem, RenderingContext renderingContext, bool useRenderStyle)
			: base(renderReportItem, renderingContext, useRenderStyle)
		{
		}

		protected override bool IsAvailableStyle(StyleAttributeNames styleName)
		{
			if (styleName == StyleAttributeNames.TextAlign || styleName == StyleAttributeNames.LineHeight)
			{
				return true;
			}
			return false;
		}
	}
}

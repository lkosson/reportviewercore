using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class TextBoxFilteredStyle : Style
	{
		internal TextBoxFilteredStyle(Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem, RenderingContext renderingContext, bool useRenderStyle)
			: base(renderReportItem, renderingContext, useRenderStyle)
		{
		}

		protected override bool IsAvailableStyle(StyleAttributeNames styleName)
		{
			switch (styleName)
			{
			case StyleAttributeNames.BorderColor:
			case StyleAttributeNames.BorderColorTop:
			case StyleAttributeNames.BorderColorLeft:
			case StyleAttributeNames.BorderColorRight:
			case StyleAttributeNames.BorderColorBottom:
			case StyleAttributeNames.BorderStyle:
			case StyleAttributeNames.BorderStyleTop:
			case StyleAttributeNames.BorderStyleLeft:
			case StyleAttributeNames.BorderStyleRight:
			case StyleAttributeNames.BorderStyleBottom:
			case StyleAttributeNames.BorderWidth:
			case StyleAttributeNames.BorderWidthTop:
			case StyleAttributeNames.BorderWidthLeft:
			case StyleAttributeNames.BorderWidthRight:
			case StyleAttributeNames.BorderWidthBottom:
			case StyleAttributeNames.BackgroundColor:
			case StyleAttributeNames.VerticalAlign:
			case StyleAttributeNames.PaddingLeft:
			case StyleAttributeNames.PaddingRight:
			case StyleAttributeNames.PaddingTop:
			case StyleAttributeNames.PaddingBottom:
			case StyleAttributeNames.Direction:
			case StyleAttributeNames.WritingMode:
			case StyleAttributeNames.BackgroundImage:
			case StyleAttributeNames.BackgroundImageRepeat:
				return true;
			default:
				return false;
			}
		}
	}
}

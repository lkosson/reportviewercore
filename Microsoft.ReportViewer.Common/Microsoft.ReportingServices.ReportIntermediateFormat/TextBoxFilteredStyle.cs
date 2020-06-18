namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class TextBoxFilteredStyle : Style
	{
		internal TextBoxFilteredStyle(Style style)
			: base(ConstructionPhase.Deserializing)
		{
			m_styleAttributes = style.StyleAttributes;
			m_expressionList = style.ExpressionList;
		}

		internal override bool GetAttributeInfo(string styleAttributeName, out AttributeInfo styleAttribute)
		{
			switch (styleAttributeName)
			{
			case "BorderColor":
			case "BorderColorTop":
			case "BorderColorLeft":
			case "BorderColorRight":
			case "BorderColorBottom":
			case "BorderStyle":
			case "BorderStyleTop":
			case "BorderStyleLeft":
			case "BorderStyleRight":
			case "BorderStyleBottom":
			case "BorderWidth":
			case "BorderWidthTop":
			case "BorderWidthLeft":
			case "BorderWidthRight":
			case "BorderWidthBottom":
			case "BackgroundColor":
			case "Direction":
			case "VerticalAlign":
			case "PaddingLeft":
			case "PaddingRight":
			case "PaddingTop":
			case "PaddingBottom":
			case "WritingMode":
			case "BackgroundImage":
			case "BackgroundRepeat":
			case "BackgroundImageSource":
			case "BackgroundImageValue":
			case "BackgroundImageMIMEType":
				return base.GetAttributeInfo(styleAttributeName, out styleAttribute);
			default:
				styleAttribute = null;
				return false;
			}
		}
	}
}

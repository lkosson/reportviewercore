namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class ParagraphFilteredStyle : Style
	{
		internal ParagraphFilteredStyle(Style style)
			: base(ConstructionPhase.Deserializing)
		{
			m_styleAttributes = style.StyleAttributes;
			m_expressionList = style.ExpressionList;
		}

		internal override bool GetAttributeInfo(string styleAttributeName, out AttributeInfo styleAttribute)
		{
			if (styleAttributeName == "TextAlign" || styleAttributeName == "LineHeight")
			{
				return base.GetAttributeInfo(styleAttributeName, out styleAttribute);
			}
			styleAttribute = null;
			return false;
		}
	}
}

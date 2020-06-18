namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class CompiledStyleInfo
	{
		private HtmlElement.HtmlElementType m_elementType;

		private ReportColor m_color;

		private FontStyles m_fontStyle;

		private string m_fontFamily;

		private ReportSize m_fontSize;

		private TextAlignments m_textAlign;

		private TextDecorations m_textDecoration;

		private FontWeights m_fontWeight;

		private bool m_colorSet;

		private bool m_fontStyleSet;

		private bool m_fontFamilySet;

		private bool m_fontSizeSet;

		private bool m_textAlignSet;

		private bool m_textDecorationSet;

		private bool m_fontWeightSet;

		private CompiledStyleInfo m_parentStyle;

		private CompiledStyleInfo m_childStyle;

		internal HtmlElement.HtmlElementType ElementType
		{
			get
			{
				return m_elementType;
			}
			set
			{
				m_elementType = value;
			}
		}

		internal ReportColor Color
		{
			get
			{
				if (m_colorSet)
				{
					return m_color;
				}
				if (m_parentStyle != null)
				{
					return m_parentStyle.Color;
				}
				return null;
			}
			set
			{
				m_colorSet = true;
				m_color = value;
			}
		}

		internal FontStyles FontStyle
		{
			get
			{
				if (m_fontStyleSet)
				{
					return m_fontStyle;
				}
				if (m_parentStyle != null)
				{
					return m_parentStyle.FontStyle;
				}
				return FontStyles.Default;
			}
			set
			{
				m_fontStyleSet = true;
				m_fontStyle = value;
			}
		}

		internal string FontFamily
		{
			get
			{
				if (m_fontFamilySet)
				{
					return m_fontFamily;
				}
				if (m_parentStyle != null)
				{
					return m_parentStyle.FontFamily;
				}
				return null;
			}
			set
			{
				m_fontFamilySet = true;
				m_fontFamily = value;
			}
		}

		internal ReportSize FontSize
		{
			get
			{
				if (m_fontSizeSet)
				{
					return m_fontSize;
				}
				if (m_parentStyle != null)
				{
					return m_parentStyle.FontSize;
				}
				return null;
			}
			set
			{
				m_fontSizeSet = true;
				m_fontSize = value;
			}
		}

		internal TextAlignments TextAlign
		{
			get
			{
				if (m_textAlignSet)
				{
					return m_textAlign;
				}
				if (m_parentStyle != null)
				{
					return m_parentStyle.TextAlign;
				}
				return TextAlignments.Default;
			}
			set
			{
				m_textAlignSet = true;
				m_textAlign = value;
			}
		}

		internal FontWeights FontWeight
		{
			get
			{
				if (m_fontWeightSet)
				{
					return m_fontWeight;
				}
				if (m_parentStyle != null)
				{
					return m_parentStyle.FontWeight;
				}
				return FontWeights.Default;
			}
			set
			{
				m_fontWeightSet = true;
				m_fontWeight = value;
			}
		}

		internal TextDecorations TextDecoration
		{
			get
			{
				if (m_textDecorationSet)
				{
					return m_textDecoration;
				}
				if (m_parentStyle != null)
				{
					return m_parentStyle.TextDecoration;
				}
				return TextDecorations.Default;
			}
			set
			{
				m_textDecorationSet = true;
				m_textDecoration = value;
			}
		}

		internal CompiledStyleInfo CreateChildStyle(HtmlElement.HtmlElementType elementType)
		{
			CompiledStyleInfo compiledStyleInfo = new CompiledStyleInfo();
			compiledStyleInfo.m_elementType = elementType;
			compiledStyleInfo.m_parentStyle = this;
			m_childStyle = compiledStyleInfo;
			return compiledStyleInfo;
		}

		internal CompiledStyleInfo RemoveStyle(HtmlElement.HtmlElementType elementType)
		{
			if (m_elementType == elementType)
			{
				if (m_parentStyle != null)
				{
					m_parentStyle.m_childStyle = null;
					return m_parentStyle;
				}
				ResetStyle();
			}
			else if (m_parentStyle != null)
			{
				m_parentStyle.InternalRemoveStyle(elementType);
			}
			return this;
		}

		internal void InternalRemoveStyle(HtmlElement.HtmlElementType elementType)
		{
			if (m_elementType == elementType)
			{
				if (m_parentStyle != null)
				{
					m_parentStyle.m_childStyle = m_childStyle;
					m_childStyle.m_parentStyle = m_parentStyle;
				}
				else if (m_parentStyle == null)
				{
					m_childStyle.m_parentStyle = null;
				}
			}
			else if (m_parentStyle != null)
			{
				m_parentStyle.InternalRemoveStyle(elementType);
			}
		}

		private void ResetStyle()
		{
			m_colorSet = false;
			m_fontFamilySet = false;
			m_fontSizeSet = false;
			m_fontStyleSet = false;
			m_fontWeightSet = false;
			m_textAlignSet = false;
			m_textDecorationSet = false;
		}

		internal void PopulateStyleInstance(ICompiledStyleInstance styleInstance, bool isParagraphStyle)
		{
			if (isParagraphStyle)
			{
				TextAlignments textAlign = TextAlign;
				if (textAlign != 0)
				{
					styleInstance.TextAlign = textAlign;
				}
				return;
			}
			ReportColor color = Color;
			if (color != null)
			{
				styleInstance.Color = color;
			}
			string fontFamily = FontFamily;
			if (!string.IsNullOrEmpty(fontFamily))
			{
				styleInstance.FontFamily = fontFamily;
			}
			ReportSize fontSize = FontSize;
			if (fontSize != null)
			{
				styleInstance.FontSize = fontSize;
			}
			FontStyles fontStyle = FontStyle;
			if (fontStyle != 0)
			{
				styleInstance.FontStyle = fontStyle;
			}
			FontWeights fontWeight = FontWeight;
			if (fontWeight != 0)
			{
				styleInstance.FontWeight = fontWeight;
			}
			TextDecorations textDecoration = TextDecoration;
			if (textDecoration != 0)
			{
				styleInstance.TextDecoration = textDecoration;
			}
		}
	}
}

using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class HtmlParser : RichTextParser
	{
		internal sealed class Constants
		{
			internal const string HtmlSize = "size";

			internal const string HtmlColor = "color";

			internal const string HtmlAlign = "align";

			internal const string CssFontSize = "font-size";

			internal const string CssFontStyle = "font-style";

			internal const string CssFontWeight = "font-weight";

			internal const string CssTextAlign = "text-align";

			internal const string CssTextIndent = "text-indent";

			internal const string CssPadding = "padding";

			internal const string CssColor = "color";

			internal const char NonBreakingSpace = '\u00a0';
		}

		internal static class StyleDefaults
		{
			internal static ReportSize H1FontSize = new ReportSize("24pt");

			internal static ReportSize H2FontSize = new ReportSize("18pt");

			internal static ReportSize H3FontSize = new ReportSize("14pt");

			internal static ReportSize H4FontSize = new ReportSize("12pt");

			internal static ReportSize H5FontSize = new ReportSize("10pt");

			internal static ReportSize H6FontSize = new ReportSize("8pt");

			internal static ReportSize PFontSize = new ReportSize("10pt");

			internal static ReportSize H1Margin = H1FontSize;

			internal static ReportSize H2Margin = H2FontSize;

			internal static ReportSize H3Margin = H3FontSize;

			internal static ReportSize H4Margin = H4FontSize;

			internal static ReportSize H5Margin = H5FontSize;

			internal static ReportSize H6Margin = H6FontSize;

			internal static ReportSize PMargin = PFontSize;
		}

		private HtmlElement m_currentHtmlElement;

		private string m_currentHyperlinkText;

		private HtmlLexer m_htmlLexer;

		internal HtmlParser(bool multipleParagraphsAllowed, IRichTextInstanceCreator iRichTextInstanceCreator, IRichTextLogger richTextLogger)
			: base(multipleParagraphsAllowed, iRichTextInstanceCreator, richTextLogger)
		{
		}

		private string HtmlTrimStart(string input)
		{
			for (int i = 0; i < input.Length; i++)
			{
				char c = input[i];
				if (!char.IsWhiteSpace(c) || c == '\u00a0')
				{
					return input.Substring(i);
				}
			}
			return string.Empty;
		}

		protected override void InternalParse(string richText)
		{
			m_htmlLexer = new HtmlLexer(richText);
			int num = 0;
			FunctionalList<ListStyle> functionalList = FunctionalList<ListStyle>.Empty;
			HtmlElement.HtmlNodeType htmlNodeType = HtmlElement.HtmlNodeType.Element;
			HtmlElement.HtmlNodeType htmlNodeType2 = HtmlElement.HtmlNodeType.Element;
			HtmlElement.HtmlElementType htmlElementType = HtmlElement.HtmlElementType.None;
			HtmlElement.HtmlElementType htmlElementType2 = HtmlElement.HtmlElementType.None;
			while (m_htmlLexer.Read())
			{
				m_currentHtmlElement = m_htmlLexer.CurrentElement;
				htmlElementType2 = m_currentHtmlElement.ElementType;
				htmlNodeType2 = m_currentHtmlElement.NodeType;
				switch (htmlNodeType2)
				{
				case HtmlElement.HtmlNodeType.Element:
					if (num != 0 && htmlElementType2 != HtmlElement.HtmlElementType.TITLE)
					{
						break;
					}
					switch (htmlElementType2)
					{
					case HtmlElement.HtmlElementType.TITLE:
						if (!m_currentHtmlElement.IsEmptyElement)
						{
							num++;
						}
						htmlElementType2 = htmlElementType;
						htmlNodeType2 = htmlNodeType;
						break;
					case HtmlElement.HtmlElementType.P:
					case HtmlElement.HtmlElementType.DIV:
					case HtmlElement.HtmlElementType.LI:
					case HtmlElement.HtmlElementType.H1:
					case HtmlElement.HtmlElementType.H2:
					case HtmlElement.HtmlElementType.H3:
					case HtmlElement.HtmlElementType.H4:
					case HtmlElement.HtmlElementType.H5:
					case HtmlElement.HtmlElementType.H6:
						ParseParagraphElement(htmlElementType2, functionalList);
						break;
					case HtmlElement.HtmlElementType.UL:
					case HtmlElement.HtmlElementType.OL:
					{
						FlushPendingLI();
						CloseParagraph();
						ListStyle listStyleForElement2 = GetListStyleForElement(htmlElementType2);
						functionalList = functionalList.Add(listStyleForElement2);
						m_currentParagraph.ListLevel = functionalList.Count;
						break;
					}
					case HtmlElement.HtmlElementType.SPAN:
					case HtmlElement.HtmlElementType.FONT:
					case HtmlElement.HtmlElementType.STRONG:
					case HtmlElement.HtmlElementType.STRIKE:
					case HtmlElement.HtmlElementType.B:
					case HtmlElement.HtmlElementType.I:
					case HtmlElement.HtmlElementType.U:
					case HtmlElement.HtmlElementType.S:
					case HtmlElement.HtmlElementType.EM:
						ParseTextRunElement(htmlElementType2);
						break;
					case HtmlElement.HtmlElementType.A:
						ParseActionElement(functionalList.Count);
						break;
					case HtmlElement.HtmlElementType.BR:
						if (htmlNodeType != HtmlElement.HtmlNodeType.EndElement)
						{
							AppendText(Environment.NewLine);
						}
						else
						{
							SetTextRunValue(Environment.NewLine);
						}
						break;
					default:
						htmlElementType2 = htmlElementType;
						htmlNodeType2 = htmlNodeType;
						break;
					}
					break;
				case HtmlElement.HtmlNodeType.Text:
				{
					if (num != 0)
					{
						break;
					}
					string text = m_currentHtmlElement.Value;
					if (htmlNodeType == HtmlElement.HtmlNodeType.Text)
					{
						AppendText(text);
						break;
					}
					if (htmlElementType == HtmlElement.HtmlElementType.BR)
					{
						AppendText(HtmlTrimStart(text));
						break;
					}
					if (m_currentParagraphInstance == null)
					{
						text = HtmlTrimStart(text);
					}
					if (!string.IsNullOrEmpty(text))
					{
						SetTextRunValue(text);
						break;
					}
					htmlElementType2 = htmlElementType;
					htmlNodeType2 = htmlNodeType;
					break;
				}
				case HtmlElement.HtmlNodeType.EndElement:
					if (num != 0 && htmlElementType2 != HtmlElement.HtmlElementType.TITLE)
					{
						break;
					}
					switch (htmlElementType2)
					{
					case HtmlElement.HtmlElementType.TITLE:
						if (num > 0)
						{
							num--;
						}
						htmlElementType2 = htmlElementType;
						htmlNodeType2 = htmlNodeType;
						break;
					case HtmlElement.HtmlElementType.UL:
					case HtmlElement.HtmlElementType.OL:
						FlushPendingLI();
						CloseParagraph();
						if (functionalList.Count > 0)
						{
							ListStyle listStyleForElement = GetListStyleForElement(htmlElementType2);
							bool flag = false;
							FunctionalList<ListStyle> functionalList2 = functionalList;
							do
							{
								flag = (functionalList2.First == listStyleForElement);
								functionalList2 = functionalList2.Rest;
							}
							while (!flag && functionalList2.Count > 0);
							if (flag)
							{
								functionalList = functionalList2;
								m_currentParagraph.ListLevel = functionalList.Count;
							}
						}
						break;
					case HtmlElement.HtmlElementType.LI:
						CloseParagraph();
						break;
					case HtmlElement.HtmlElementType.P:
					case HtmlElement.HtmlElementType.DIV:
					case HtmlElement.HtmlElementType.H1:
					case HtmlElement.HtmlElementType.H2:
					case HtmlElement.HtmlElementType.H3:
					case HtmlElement.HtmlElementType.H4:
					case HtmlElement.HtmlElementType.H5:
					case HtmlElement.HtmlElementType.H6:
						CloseParagraph();
						m_currentParagraph = m_currentParagraph.RemoveParagraph(htmlElementType2);
						goto case HtmlElement.HtmlElementType.SPAN;
					case HtmlElement.HtmlElementType.SPAN:
					case HtmlElement.HtmlElementType.FONT:
					case HtmlElement.HtmlElementType.STRONG:
					case HtmlElement.HtmlElementType.STRIKE:
					case HtmlElement.HtmlElementType.B:
					case HtmlElement.HtmlElementType.I:
					case HtmlElement.HtmlElementType.U:
					case HtmlElement.HtmlElementType.S:
					case HtmlElement.HtmlElementType.EM:
						m_currentStyle = m_currentStyle.RemoveStyle(htmlElementType2);
						break;
					case HtmlElement.HtmlElementType.A:
						RevertActionElement(htmlElementType2);
						break;
					default:
						htmlElementType2 = htmlElementType;
						htmlNodeType2 = htmlNodeType;
						break;
					}
					break;
				}
				htmlNodeType = htmlNodeType2;
				htmlElementType = htmlElementType2;
			}
			if (m_paragraphInstanceCollection.Count == 0)
			{
				CreateTextRunInstance();
			}
			m_currentParagraph = m_currentParagraph.RemoveAll();
		}

		private ListStyle GetListStyleForElement(HtmlElement.HtmlElementType elementType)
		{
			if (elementType == HtmlElement.HtmlElementType.OL)
			{
				return ListStyle.Numbered;
			}
			return ListStyle.Bulleted;
		}

		private void ParseParagraphElement(HtmlElement.HtmlElementType elementType, FunctionalList<ListStyle> listStyles)
		{
			CloseParagraph();
			if (m_currentParagraph.ElementType == HtmlElement.HtmlElementType.P)
			{
				m_currentParagraph = m_currentParagraph.RemoveParagraph(HtmlElement.HtmlElementType.P);
				m_currentStyle = m_currentStyle.RemoveStyle(HtmlElement.HtmlElementType.P);
			}
			if (elementType == HtmlElement.HtmlElementType.LI)
			{
				FlushPendingLI();
				if (listStyles.Count > 0)
				{
					m_currentParagraph.ListStyle = listStyles.First;
				}
				else
				{
					m_currentParagraph.ListStyle = ListStyle.Bulleted;
				}
			}
			else
			{
				m_currentStyle = m_currentStyle.CreateChildStyle(elementType);
				m_currentParagraph = m_currentParagraph.CreateChildParagraph(elementType);
				switch (elementType)
				{
				case HtmlElement.HtmlElementType.H1:
					m_currentStyle.FontSize = StyleDefaults.H1FontSize;
					m_currentStyle.FontWeight = FontWeights.Bold;
					SetMarginTopAndBottom(StyleDefaults.H1Margin);
					break;
				case HtmlElement.HtmlElementType.H2:
					m_currentStyle.FontSize = StyleDefaults.H2FontSize;
					m_currentStyle.FontWeight = FontWeights.Bold;
					SetMarginTopAndBottom(StyleDefaults.H2Margin);
					break;
				case HtmlElement.HtmlElementType.H3:
					m_currentStyle.FontSize = StyleDefaults.H3FontSize;
					m_currentStyle.FontWeight = FontWeights.Bold;
					SetMarginTopAndBottom(StyleDefaults.H3Margin);
					break;
				case HtmlElement.HtmlElementType.H4:
					m_currentStyle.FontSize = StyleDefaults.H4FontSize;
					m_currentStyle.FontWeight = FontWeights.Bold;
					SetMarginTopAndBottom(StyleDefaults.H4Margin);
					break;
				case HtmlElement.HtmlElementType.H5:
					m_currentStyle.FontSize = StyleDefaults.H5FontSize;
					m_currentStyle.FontWeight = FontWeights.Bold;
					SetMarginTopAndBottom(StyleDefaults.H5Margin);
					break;
				case HtmlElement.HtmlElementType.H6:
					m_currentStyle.FontSize = StyleDefaults.H6FontSize;
					m_currentStyle.FontWeight = FontWeights.Bold;
					SetMarginTopAndBottom(StyleDefaults.H6Margin);
					break;
				case HtmlElement.HtmlElementType.P:
					SetMarginTopAndBottom(StyleDefaults.PMargin);
					break;
				}
				if (!m_currentHtmlElement.IsEmptyElement && m_currentHtmlElement.HasAttributes && m_allowMultipleParagraphs && m_currentHtmlElement.Attributes.TryGetValue("align", out string value))
				{
					if (RichTextStyleTranslator.TranslateTextAlign(value, out TextAlignments textAlignment))
					{
						m_currentStyle.TextAlign = textAlignment;
					}
					else
					{
						m_richTextLogger.RegisterInvalidValueWarning("align", value, m_currentHtmlElement.CharacterPosition);
					}
				}
			}
			SetStyleValues(isParagraph: true);
		}

		private void FlushPendingLI()
		{
			if (m_allowMultipleParagraphs && m_currentParagraph.ListStyle != 0)
			{
				CreateParagraphInstance();
				CloseParagraph();
			}
		}

		private void SetMarginTopAndBottom(ReportSize marginValue)
		{
			m_currentParagraph.UpdateMarginTop(marginValue);
			m_currentParagraph.AddMarginBottom(marginValue);
		}

		private void ParseTextRunElement(HtmlElement.HtmlElementType elementType)
		{
			m_currentStyle = m_currentStyle.CreateChildStyle(elementType);
			bool flag = false;
			switch (m_currentHtmlElement.ElementType)
			{
			case HtmlElement.HtmlElementType.I:
			case HtmlElement.HtmlElementType.EM:
				m_currentStyle.FontStyle = FontStyles.Italic;
				break;
			case HtmlElement.HtmlElementType.U:
				m_currentStyle.TextDecoration = TextDecorations.Underline;
				break;
			case HtmlElement.HtmlElementType.STRONG:
			case HtmlElement.HtmlElementType.B:
				m_currentStyle.FontWeight = FontWeights.Bold;
				break;
			case HtmlElement.HtmlElementType.STRIKE:
			case HtmlElement.HtmlElementType.S:
				m_currentStyle.TextDecoration = TextDecorations.LineThrough;
				break;
			case HtmlElement.HtmlElementType.SPAN:
			case HtmlElement.HtmlElementType.FONT:
				flag = true;
				break;
			}
			if (!flag || m_currentHtmlElement.IsEmptyElement || !m_currentHtmlElement.HasAttributes)
			{
				return;
			}
			if (m_currentHtmlElement.ElementType == HtmlElement.HtmlElementType.FONT)
			{
				if (m_currentHtmlElement.Attributes.TryGetValue("size", out string value))
				{
					if (RichTextStyleTranslator.TranslateHtmlFontSize(value, out string translatedSize))
					{
						m_currentStyle.FontSize = new ReportSize(translatedSize);
					}
					else
					{
						m_richTextLogger.RegisterInvalidSizeWarning("size", value, m_currentHtmlElement.CharacterPosition);
					}
				}
				if (m_currentHtmlElement.Attributes.TryGetValue("face", out value))
				{
					m_currentStyle.FontFamily = value;
				}
				if (m_currentHtmlElement.Attributes.TryGetValue("color", out value))
				{
					if (ReportColor.TryParse(RichTextStyleTranslator.TranslateHtmlColor(value), out ReportColor reportColor))
					{
						m_currentStyle.Color = reportColor;
					}
					else
					{
						m_richTextLogger.RegisterInvalidColorWarning("color", value, m_currentHtmlElement.CharacterPosition);
					}
				}
			}
			else if (m_currentHtmlElement.ElementType == HtmlElement.HtmlElementType.SPAN)
			{
				SetStyleValues(isParagraph: false);
			}
		}

		private void RevertActionElement(HtmlElement.HtmlElementType elementType)
		{
			if (m_currentHyperlinkText != null)
			{
				m_currentHyperlinkText = null;
				m_currentStyle = m_currentStyle.RemoveStyle(elementType);
			}
		}

		private void ParseActionElement(int listLevel)
		{
			RevertActionElement(HtmlElement.HtmlElementType.A);
			if (!m_currentHtmlElement.IsEmptyElement && m_currentHtmlElement.HasAttributes && m_currentHtmlElement.Attributes.TryGetValue("href", out string value))
			{
				IActionInstance actionInstance = m_IRichTextInstanceCreator.CreateActionInstance();
				actionInstance.SetHyperlinkText(value);
				if (actionInstance.HyperlinkText != null)
				{
					m_currentStyle = m_currentStyle.CreateChildStyle(HtmlElement.HtmlElementType.A);
					m_currentStyle.Color = new ReportColor("Blue");
					m_currentStyle.TextDecoration = TextDecorations.Underline;
					m_currentHyperlinkText = value;
				}
			}
		}

		protected override ICompiledTextRunInstance CreateTextRunInstance()
		{
			ICompiledTextRunInstance compiledTextRunInstance = base.CreateTextRunInstance();
			compiledTextRunInstance.MarkupType = MarkupType.HTML;
			if (m_currentHyperlinkText != null)
			{
				IActionInstance actionInstance = m_IRichTextInstanceCreator.CreateActionInstance();
				actionInstance.SetHyperlinkText(m_currentHyperlinkText);
				compiledTextRunInstance.ActionInstance = actionInstance;
			}
			return compiledTextRunInstance;
		}

		private void SetStyleValues(bool isParagraph)
		{
			if (m_currentHtmlElement.CssStyle == null)
			{
				return;
			}
			string value;
			ReportSize reportSize;
			if (isParagraph && m_allowMultipleParagraphs)
			{
				if (m_currentHtmlElement.CssStyle.TryGetValue("text-align", out value))
				{
					if (RichTextStyleTranslator.TranslateTextAlign(value, out TextAlignments textAlignment))
					{
						m_currentStyle.TextAlign = textAlignment;
					}
					else
					{
						m_richTextLogger.RegisterInvalidValueWarning("text-align", value, m_currentHtmlElement.CharacterPosition);
					}
				}
				if (m_currentHtmlElement.CssStyle.TryGetValue("text-indent", out value))
				{
					if (ReportSize.TryParse(value, allowNegative: true, out reportSize))
					{
						m_currentParagraph.HangingIndent = reportSize;
					}
					else
					{
						m_richTextLogger.RegisterInvalidSizeWarning("text-indent", value, m_currentHtmlElement.CharacterPosition);
					}
				}
				ReportSize generalPadding = null;
				if (m_currentHtmlElement.CssStyle.TryGetValue("padding", out value))
				{
					if (ReportSize.TryParse(value, out reportSize))
					{
						generalPadding = reportSize;
					}
					else
					{
						m_richTextLogger.RegisterInvalidSizeWarning("padding", value, m_currentHtmlElement.CharacterPosition);
					}
				}
				if (HasPaddingValue("padding-top", generalPadding, out ReportSize effectivePadding))
				{
					m_currentParagraph.AddSpaceBefore(effectivePadding);
				}
				if (HasPaddingValue("padding-bottom", generalPadding, out effectivePadding))
				{
					m_currentParagraph.AddSpaceAfter(effectivePadding);
				}
				if (HasPaddingValue("padding-left", generalPadding, out effectivePadding))
				{
					m_currentParagraph.AddLeftIndent(effectivePadding);
				}
				if (HasPaddingValue("padding-right", generalPadding, out effectivePadding))
				{
					m_currentParagraph.AddRightIndent(effectivePadding);
				}
			}
			if (m_currentHtmlElement.CssStyle.TryGetValue("font-family", out value))
			{
				m_currentStyle.FontFamily = value;
			}
			if (m_currentHtmlElement.CssStyle.TryGetValue("font-size", out value))
			{
				if (ReportSize.TryParse(value, out reportSize))
				{
					m_currentStyle.FontSize = reportSize;
				}
				else
				{
					m_richTextLogger.RegisterInvalidSizeWarning("font-size", value, m_currentHtmlElement.CharacterPosition);
				}
			}
			if (m_currentHtmlElement.CssStyle.TryGetValue("font-weight", out value))
			{
				if (RichTextStyleTranslator.TranslateFontWeight(value, out FontWeights fontWieght))
				{
					m_currentStyle.FontWeight = fontWieght;
				}
				else
				{
					m_richTextLogger.RegisterInvalidValueWarning("font-weight", value, m_currentHtmlElement.CharacterPosition);
				}
			}
			if (m_currentHtmlElement.CssStyle.TryGetValue("color", out value))
			{
				if (ReportColor.TryParse(RichTextStyleTranslator.TranslateHtmlColor(value), out ReportColor reportColor))
				{
					m_currentStyle.Color = reportColor;
				}
				else
				{
					m_richTextLogger.RegisterInvalidColorWarning("color", value, m_currentHtmlElement.CharacterPosition);
				}
			}
		}

		private bool HasPaddingValue(string attrName, ReportSize generalPadding, out ReportSize effectivePadding)
		{
			if (m_currentHtmlElement.CssStyle.TryGetValue(attrName, out string value))
			{
				if (ReportSize.TryParse(value, out ReportSize reportSize))
				{
					effectivePadding = reportSize;
					return true;
				}
				m_richTextLogger.RegisterInvalidSizeWarning("padding", value, m_currentHtmlElement.CharacterPosition);
			}
			if (generalPadding != null)
			{
				effectivePadding = generalPadding;
				return true;
			}
			effectivePadding = null;
			return false;
		}
	}
}

using Microsoft.ReportingServices.Rendering.RPLProcessing;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class ParagraphStyleWriter : ElementStyleWriter
	{
		internal enum Mode
		{
			ListOnly = 1,
			ParagraphOnly,
			All
		}

		private RPLParagraph m_paragraph;

		private RPLTextBox m_textBox;

		private bool m_outputSharedInNonShared;

		private Mode m_mode = Mode.All;

		private int m_currentListLevel;

		internal RPLParagraph Paragraph
		{
			get
			{
				return m_paragraph;
			}
			set
			{
				m_paragraph = value;
			}
		}

		internal Mode ParagraphMode
		{
			get
			{
				return m_mode;
			}
			set
			{
				m_mode = value;
			}
		}

		internal int CurrentListLevel
		{
			get
			{
				return m_currentListLevel;
			}
			set
			{
				m_currentListLevel = value;
			}
		}

		internal bool OutputSharedInNonShared
		{
			get
			{
				return m_outputSharedInNonShared;
			}
			set
			{
				m_outputSharedInNonShared = value;
			}
		}

		internal ParagraphStyleWriter(IHtmlReportWriter renderer, RPLTextBox textBox)
			: base(renderer)
		{
			m_textBox = textBox;
		}

		internal override bool NeedsToWriteNullStyle(StyleWriterMode mode)
		{
			RPLParagraph paragraph = m_paragraph;
			switch (mode)
			{
			case StyleWriterMode.NonShared:
			{
				RPLParagraphProps rPLParagraphProps = paragraph.ElementProps as RPLParagraphProps;
				if (rPLParagraphProps.LeftIndent != null || rPLParagraphProps.RightIndent != null || rPLParagraphProps.SpaceBefore != null || rPLParagraphProps.SpaceAfter != null || rPLParagraphProps.HangingIndent != null)
				{
					return true;
				}
				IRPLStyle nonSharedStyle = m_textBox.ElementProps.NonSharedStyle;
				if (m_outputSharedInNonShared)
				{
					return true;
				}
				break;
			}
			case StyleWriterMode.Shared:
			{
				if (m_outputSharedInNonShared)
				{
					return false;
				}
				RPLParagraphPropsDef rPLParagraphPropsDef = paragraph.ElementProps.Definition as RPLParagraphPropsDef;
				if (rPLParagraphPropsDef.LeftIndent != null || rPLParagraphPropsDef.RightIndent != null || rPLParagraphPropsDef.SpaceBefore != null || rPLParagraphPropsDef.SpaceAfter != null || rPLParagraphPropsDef.HangingIndent != null)
				{
					return true;
				}
				IRPLStyle sharedStyle = m_textBox.ElementPropsDef.SharedStyle;
				if (sharedStyle != null && HTML4Renderer.IsDirectionRTL(sharedStyle))
				{
					return true;
				}
				break;
			}
			}
			return false;
		}

		internal override void WriteStyles(StyleWriterMode mode, IRPLStyle style)
		{
			RPLParagraph paragraph = m_paragraph;
			RPLTextBox textBox = m_textBox;
			RPLTextBoxProps rPLTextBoxProps = textBox.ElementProps as RPLTextBoxProps;
			if (paragraph != null)
			{
				RPLParagraphProps rPLParagraphProps = paragraph.ElementProps as RPLParagraphProps;
				RPLParagraphPropsDef rPLParagraphPropsDef = rPLParagraphProps.Definition as RPLParagraphPropsDef;
				RPLReportSize rPLReportSize = null;
				RPLReportSize leftIndent = null;
				RPLReportSize rightIndent = null;
				RPLReportSize spaceBefore = null;
				RPLReportSize spaceAfter = null;
				IRPLStyle iRPLStyle = null;
				switch (mode)
				{
				case StyleWriterMode.All:
					rPLReportSize = rPLParagraphProps.HangingIndent;
					if (rPLReportSize == null)
					{
						rPLReportSize = rPLParagraphPropsDef.HangingIndent;
					}
					leftIndent = rPLParagraphProps.LeftIndent;
					if (leftIndent == null)
					{
						leftIndent = rPLParagraphPropsDef.LeftIndent;
					}
					rightIndent = rPLParagraphProps.RightIndent;
					if (rightIndent == null)
					{
						rightIndent = rPLParagraphPropsDef.RightIndent;
					}
					spaceBefore = rPLParagraphProps.SpaceBefore;
					if (spaceBefore == null)
					{
						spaceBefore = rPLParagraphPropsDef.SpaceBefore;
					}
					spaceAfter = rPLParagraphProps.SpaceAfter;
					if (spaceAfter == null)
					{
						spaceAfter = rPLParagraphPropsDef.SpaceAfter;
					}
					break;
				case StyleWriterMode.NonShared:
				{
					iRPLStyle = m_textBox.ElementProps.NonSharedStyle;
					rPLReportSize = rPLParagraphProps.HangingIndent;
					rightIndent = rPLParagraphProps.RightIndent;
					leftIndent = rPLParagraphProps.LeftIndent;
					spaceAfter = rPLParagraphProps.SpaceAfter;
					spaceBefore = rPLParagraphProps.SpaceBefore;
					if (m_outputSharedInNonShared)
					{
						if (rPLReportSize == null)
						{
							rPLReportSize = rPLParagraphPropsDef.HangingIndent;
						}
						if (rightIndent == null)
						{
							rightIndent = rPLParagraphPropsDef.RightIndent;
						}
						if (leftIndent == null)
						{
							leftIndent = rPLParagraphPropsDef.LeftIndent;
						}
						if (spaceAfter == null)
						{
							spaceAfter = rPLParagraphPropsDef.SpaceAfter;
						}
						if (spaceBefore == null)
						{
							spaceBefore = rPLParagraphPropsDef.SpaceBefore;
						}
						break;
					}
					bool flag = HTML4Renderer.IsDirectionRTL(m_textBox.ElementProps.Style);
					if (rPLReportSize == null)
					{
						if (flag)
						{
							if (rightIndent != null)
							{
								rPLReportSize = rPLParagraphPropsDef.HangingIndent;
							}
						}
						else if (leftIndent != null)
						{
							rPLReportSize = rPLParagraphPropsDef.HangingIndent;
						}
					}
					else if (flag)
					{
						if (rightIndent == null)
						{
							rightIndent = rPLParagraphPropsDef.RightIndent;
						}
					}
					else if (leftIndent == null)
					{
						leftIndent = rPLParagraphPropsDef.LeftIndent;
					}
					break;
				}
				case StyleWriterMode.Shared:
					iRPLStyle = m_textBox.ElementPropsDef.SharedStyle;
					rPLReportSize = rPLParagraphPropsDef.HangingIndent;
					leftIndent = rPLParagraphPropsDef.LeftIndent;
					rightIndent = rPLParagraphPropsDef.RightIndent;
					spaceBefore = rPLParagraphPropsDef.SpaceBefore;
					spaceAfter = rPLParagraphPropsDef.SpaceAfter;
					break;
				}
				if (m_currentListLevel > 0 && rPLReportSize != null && rPLReportSize.ToMillimeters() < 0.0 && !m_renderer.IsBrowserIE)
				{
					rPLReportSize = null;
				}
				if (m_mode != Mode.ParagraphOnly)
				{
					FixIndents(ref leftIndent, ref rightIndent, ref spaceBefore, ref spaceAfter, rPLReportSize);
					bool flag2 = HTML4Renderer.IsWritingModeVertical(rPLTextBoxProps.Style);
					if (flag2 && m_renderer.IsBrowserIE)
					{
						WriteStyle(HTML4Renderer.m_paddingLeft, leftIndent);
					}
					else
					{
						WriteStyle(HTML4Renderer.m_marginLeft, leftIndent);
					}
					WriteStyle(HTML4Renderer.m_marginRight, rightIndent);
					WriteStyle(HTML4Renderer.m_marginTop, spaceBefore);
					if (flag2 && m_renderer.IsBrowserIE)
					{
						WriteStyle(HTML4Renderer.m_marginBottom, spaceAfter);
					}
					else
					{
						WriteStyle(HTML4Renderer.m_paddingBottom, spaceAfter);
					}
				}
				if (m_mode == Mode.ListOnly)
				{
					WriteStyle(HTML4Renderer.m_fontFamily, "Arial");
					WriteStyle(HTML4Renderer.m_fontSize, "10pt");
				}
				else if (rPLReportSize != null && rPLReportSize.ToMillimeters() < 0.0)
				{
					WriteStyle(HTML4Renderer.m_textIndent, rPLReportSize);
				}
			}
			if (style == null || (m_mode != Mode.All && m_mode != Mode.ParagraphOnly))
			{
				return;
			}
			object obj = style[25];
			if (obj != null || mode != 0)
			{
				RPLFormat.TextAlignments textAlignments = RPLFormat.TextAlignments.General;
				if (obj != null)
				{
					textAlignments = (RPLFormat.TextAlignments)obj;
				}
				if (textAlignments == RPLFormat.TextAlignments.General)
				{
					bool flag3 = HTML4Renderer.GetTextAlignForType(rPLTextBoxProps);
					if (HTML4Renderer.IsDirectionRTL(rPLTextBoxProps.Style))
					{
						flag3 = ((!flag3) ? true : false);
					}
					WriteStream(HTML4Renderer.m_textAlign);
					if (flag3)
					{
						WriteStream(HTML4Renderer.m_rightValue);
					}
					else
					{
						WriteStream(HTML4Renderer.m_leftValue);
					}
					WriteStream(HTML4Renderer.m_semiColon);
				}
				else
				{
					WriteStyle(HTML4Renderer.m_textAlign, EnumStrings.GetValue(textAlignments), null);
				}
			}
			WriteStyle(HTML4Renderer.m_lineHeight, style[28]);
		}

		internal void FixIndents(ref RPLReportSize leftIndent, ref RPLReportSize rightIndent, ref RPLReportSize spaceBefore, ref RPLReportSize spaceAfter, RPLReportSize hangingIndent)
		{
			RPLTextBoxProps rPLTextBoxProps = m_textBox.ElementProps as RPLTextBoxProps;
			if (HTML4Renderer.IsDirectionRTL(rPLTextBoxProps.Style))
			{
				rightIndent = FixHangingIndent(rightIndent, hangingIndent);
			}
			else
			{
				leftIndent = FixHangingIndent(leftIndent, hangingIndent);
			}
			object obj = rPLTextBoxProps.Style[30];
			if (m_renderer.IsBrowserIE && obj != null && HTML4Renderer.IsWritingModeVertical((RPLFormat.WritingModes)obj))
			{
				RPLReportSize rPLReportSize = leftIndent;
				leftIndent = spaceAfter;
				spaceAfter = rightIndent;
				rightIndent = spaceBefore;
				spaceBefore = rPLReportSize;
			}
		}

		internal RPLReportSize FixHangingIndent(RPLReportSize leftIndent, RPLReportSize hangingIndent)
		{
			if (hangingIndent == null)
			{
				return leftIndent;
			}
			double num = hangingIndent.ToMillimeters();
			if (num < 0.0)
			{
				double num2 = 0.0;
				if (leftIndent != null)
				{
					num2 = leftIndent.ToMillimeters();
				}
				num2 -= num;
				leftIndent = new RPLReportSize(num2);
			}
			return leftIndent;
		}
	}
}

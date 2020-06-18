using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal class RichTextRenderer : IDisposable
	{
		private float m_width;

		private float m_flowHeight = float.MaxValue;

		private float m_height;

		private bool m_lineLimit = true;

		private bool m_charTrimLastLine = true;

		private float m_dpi;

		private FontCache m_fontCache;

		private TextBox m_rttextbox;

		private List<Paragraph> m_cachedrtparagraphs;

		internal bool LineLimit
		{
			get
			{
				return m_lineLimit;
			}
			set
			{
				if (m_lineLimit != value)
				{
					m_lineLimit = value;
					ResetCachedObjects();
				}
			}
		}

		internal bool CharTrimLastLine
		{
			get
			{
				return m_charTrimLastLine;
			}
			set
			{
				if (m_charTrimLastLine != value)
				{
					m_charTrimLastLine = value;
					ResetCachedObjects();
				}
			}
		}

		internal float Width
		{
			get
			{
				return m_width;
			}
			set
			{
				if (m_width != value)
				{
					m_width = value;
					ResetCachedObjects();
				}
			}
		}

		internal int WidthInPX
		{
			get
			{
				return TextBox.ConvertToPixels(Width, Dpi);
			}
			set
			{
				Width = TextBox.ConvertToMillimeters(value, Dpi);
			}
		}

		internal float FlowHeight
		{
			get
			{
				return m_flowHeight;
			}
			set
			{
				if (m_flowHeight != value)
				{
					m_flowHeight = value;
					ResetCachedObjects();
				}
			}
		}

		internal int FlowHeightInPX
		{
			get
			{
				return TextBox.ConvertToPixels(FlowHeight, Dpi);
			}
			set
			{
				FlowHeight = TextBox.ConvertToMillimeters(value, Dpi);
			}
		}

		internal float Dpi
		{
			get
			{
				if (m_dpi == 0f)
				{
					using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
					{
						m_dpi = graphics.DpiX;
					}
				}
				return m_dpi;
			}
			set
			{
				m_dpi = value;
			}
		}

		internal FontCache FontCache
		{
			get
			{
				if (m_fontCache == null)
				{
					m_fontCache = new FontCache(Dpi);
				}
				return m_fontCache;
			}
			set
			{
				m_fontCache = value;
			}
		}

		internal List<Paragraph> RTParagraphs
		{
			get
			{
				if (m_cachedrtparagraphs == null)
				{
					UpdateCachedObjects();
				}
				return m_cachedrtparagraphs;
			}
			set
			{
				m_cachedrtparagraphs = value;
			}
		}

		internal TextBox RTTextbox => m_rttextbox;

		internal RichTextRenderer()
		{
		}

		internal void Render(Graphics g, RectangleF rectangle)
		{
			Render(g, rectangle, null, unitsInMM: true);
		}

		internal void Render(Graphics g, RectangleF rectangle, bool unitsInMM)
		{
			Render(g, rectangle, null, unitsInMM);
		}

		internal void Render(Graphics g, RectangleF rectangle, IEnumerable<RTSelectionHighlight> highlights)
		{
			Render(g, rectangle, highlights, unitsInMM: true);
		}

		internal void Render(Graphics g, RectangleF rectangle, IEnumerable<RTSelectionHighlight> highlights, bool unitsInMM)
		{
			Render(g, rectangle, PointF.Empty, highlights, unitsInMM);
		}

		internal void Render(Graphics g, RectangleF rectangle, PointF offset, IEnumerable<RTSelectionHighlight> highlights, bool unitsInMM)
		{
			List<Paragraph> rTParagraphs = RTParagraphs;
			if (rTParagraphs == null || rTParagraphs.Count == 0)
			{
				return;
			}
			using (RevertingDeviceContext revertingDeviceContext = new RevertingDeviceContext(g, Dpi))
			{
				Win32DCSafeHandle hdc = revertingDeviceContext.Hdc;
				if (highlights != null)
				{
					_ = RTTextbox.TextBoxProps.Direction;
					foreach (RTSelectionHighlight highlight in highlights)
					{
						if (!HighlightStartLessThanOrEqualToEnd(highlight.SelectionStart, highlight.SelectionEnd))
						{
							TextBoxContext selectionStart = highlight.SelectionStart;
							highlight.SelectionStart = highlight.SelectionEnd;
							highlight.SelectionEnd = selectionStart;
						}
						TextRun run;
						CaretInfo caretInfo = MapLocation(hdc, highlight.SelectionStart, relativeToRun: true, moveCaretToNextLine: true, out run);
						TextRun run2;
						CaretInfo caretInfo2 = MapLocation(hdc, highlight.SelectionEnd, relativeToRun: true, moveCaretToNextLine: true, out run2);
						if (caretInfo != null && caretInfo2 != null && run != null && run2 != null)
						{
							SetHighlighting(rTParagraphs, hdc, highlight, run, run2, caretInfo.Position.X, caretInfo2.Position.X);
						}
					}
				}
				Rectangle rect = (!unitsInMM) ? Rectangle.Round(rectangle) : new Rectangle(TextBox.ConvertToPixels(rectangle.X, m_dpi), TextBox.ConvertToPixels(rectangle.Y, m_dpi), TextBox.ConvertToPixels(rectangle.Width, m_dpi), TextBox.ConvertToPixels(rectangle.Height, m_dpi));
				revertingDeviceContext.XForm.Transform(ref rect);
				Win32ObjectSafeHandle win32ObjectSafeHandle = Win32.CreateRectRgn(rect.Left - 1, rect.Top - 1, rect.Right + 1, rect.Bottom + 1);
				if (!win32ObjectSafeHandle.IsInvalid)
				{
					try
					{
						if (Win32.SelectClipRgn(hdc, win32ObjectSafeHandle) == 0)
						{
							Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
						}
					}
					finally
					{
						win32ObjectSafeHandle.Close();
					}
				}
				TextBox.Render(RTTextbox, rTParagraphs, hdc, FontCache, offset, rectangle, m_dpi, unitsInMM);
				if (Win32.SelectClipRgn(hdc, Win32ObjectSafeHandle.Zero) == 0)
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
			}
		}

		private void SetHighlighting(List<Paragraph> paragraphs, Win32DCSafeHandle hdc, RTSelectionHighlight highlight, TextRun runStart, TextRun runEnd, int x1, int x2)
		{
			if (runStart == runEnd)
			{
				if (x1 != x2)
				{
					if (x1 < x2)
					{
						runStart.HighlightStart = x1;
						runStart.HighlightEnd = x2;
					}
					else
					{
						runStart.HighlightStart = x2;
						runStart.HighlightEnd = x1;
					}
					runStart.HighlightColor = highlight.Color;
					runStart.AllowColorInversion = highlight.AllowColorInversion;
				}
				return;
			}
			bool flag = false;
			bool flag2 = false;
			Color color = highlight.Color;
			bool allowColorInversion = highlight.AllowColorInversion;
			for (int i = highlight.SelectionStart.ParagraphIndex; i <= highlight.SelectionEnd.ParagraphIndex; i++)
			{
				int count = paragraphs[i].TextLines.Count;
				for (int j = 0; j < count; j++)
				{
					List<TextRun> logicalRuns = paragraphs[i].TextLines[j].LogicalRuns;
					int count2 = logicalRuns.Count;
					for (int k = 0; k < count2; k++)
					{
						TextRun textRun = logicalRuns[k];
						bool flag3 = textRun.ScriptAnalysis.fLayoutRTL == 0;
						if (textRun == runStart)
						{
							flag2 = true;
							if (flag3 && x1 < textRun.GetWidth(hdc, FontCache))
							{
								textRun.HighlightStart = x1;
								textRun.HighlightEnd = -1;
								textRun.HighlightColor = color;
								textRun.AllowColorInversion = allowColorInversion;
							}
							else if (!flag3 && x1 > 0)
							{
								textRun.HighlightStart = -1;
								textRun.HighlightEnd = x1;
								textRun.HighlightColor = color;
								textRun.AllowColorInversion = allowColorInversion;
							}
							continue;
						}
						if (textRun == runEnd)
						{
							flag2 = false;
							if (flag3 && x2 > 0)
							{
								textRun.HighlightStart = -1;
								textRun.HighlightEnd = x2;
								textRun.HighlightColor = color;
								textRun.AllowColorInversion = allowColorInversion;
							}
							else if (!flag3 && x2 < textRun.GetWidth(hdc, FontCache))
							{
								textRun.HighlightStart = x2;
								textRun.HighlightEnd = -1;
								textRun.HighlightColor = color;
								textRun.AllowColorInversion = allowColorInversion;
							}
							flag = true;
							break;
						}
						if (flag2)
						{
							textRun.HighlightStart = -1;
							textRun.HighlightEnd = -1;
							textRun.HighlightColor = color;
							textRun.AllowColorInversion = allowColorInversion;
						}
					}
					if (flag)
					{
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}

		private static bool HighlightStartLessThanOrEqualToEnd(TextBoxContext start, TextBoxContext end)
		{
			if (start.ParagraphIndex == end.ParagraphIndex)
			{
				if (start.TextRunIndex == end.TextRunIndex)
				{
					return start.TextRunCharacterIndex <= end.TextRunCharacterIndex;
				}
				return start.TextRunIndex <= end.TextRunIndex;
			}
			return start.ParagraphIndex <= end.ParagraphIndex;
		}

		internal Rectangle GetTextBoundingBoxPx(Rectangle rect, RPLFormat.VerticalAlignments vAlign)
		{
			if (RTParagraphs == null)
			{
				return new Rectangle(rect.Location, Size.Empty);
			}
			Rectangle rectangle = Rectangle.Empty;
			Size size = default(Size);
			int num = rect.Top;
			Size size2 = rect.Size;
			if (RTTextbox.VerticalText)
			{
				size2 = new Size(size2.Height, size2.Width);
			}
			foreach (Paragraph rTParagraph in RTParagraphs)
			{
				int num2 = 0;
				if (rTParagraph.TextLines.Count == 1 && rTParagraph.ParagraphProps.HangingIndent > 0f)
				{
					num2 += TextBox.ConvertToPixels(rTParagraph.ParagraphProps.HangingIndent, Dpi);
				}
				foreach (TextLine textLine in rTParagraph.TextLines)
				{
					size.Width = Math.Max(Math.Max(size.Width, textLine.GetWidth(Win32DCSafeHandle.Zero, FontCache)), 10);
					size.Height += textLine.GetHeight(Win32DCSafeHandle.Zero, FontCache);
				}
				switch (rTParagraph.ParagraphProps.Alignment)
				{
				case RPLFormat.TextAlignments.General:
					if (RTTextbox.TextBoxProps.Direction != RPLFormat.Directions.RTL)
					{
						goto case RPLFormat.TextAlignments.Left;
					}
					goto case RPLFormat.TextAlignments.Right;
				case RPLFormat.TextAlignments.Left:
					num2 += rect.Left;
					break;
				case RPLFormat.TextAlignments.Center:
					num2 += rect.Left + (size2.Width - size.Width) / 2;
					break;
				case RPLFormat.TextAlignments.Right:
					num2 += rect.Left + size2.Width - size.Width;
					break;
				default:
					throw new ArgumentException("Unknown TextAlignment: " + rTParagraph.ParagraphProps.Alignment);
				}
				num2 = ((RTTextbox.TextBoxProps.Direction != 0) ? (num2 - TextBox.ConvertToPixels(rTParagraph.ParagraphProps.RightIndent + 10.583333f * (float)rTParagraph.ParagraphProps.ListLevel, Dpi)) : (num2 + TextBox.ConvertToPixels(rTParagraph.ParagraphProps.LeftIndent + 10.583333f * (float)rTParagraph.ParagraphProps.ListLevel, Dpi)));
				Rectangle rectangle2 = new Rectangle(new Point(num2, num), size);
				num += size.Height;
				rectangle = ((!rectangle.IsEmpty) ? Rectangle.Union(rectangle, rectangle2) : rectangle2);
			}
			if (rectangle.Height < size2.Height)
			{
				switch (vAlign)
				{
				case RPLFormat.VerticalAlignments.Middle:
					rectangle.Y += (size2.Height - rectangle.Height) / 2;
					break;
				case RPLFormat.VerticalAlignments.Bottom:
					rectangle.Y = rect.Top + size2.Height - rectangle.Height;
					break;
				}
			}
			if (RTTextbox.VerticalText)
			{
				Point location = new Point(rect.Right - (rectangle.Top - rect.Top) - rectangle.Height, rectangle.Left);
				Size size3 = new Size(rectangle.Height, rectangle.Width);
				rectangle = new Rectangle(location, size3);
			}
			return Rectangle.Intersect(rect, rectangle);
		}

		internal TextBoxContext MapPoint(PointF pt)
		{
			bool atEndOfLine;
			return MapPoint(null, pt, out atEndOfLine);
		}

		internal TextBoxContext MapPoint(PointF pt, out bool atEndOfLine)
		{
			return MapPoint(null, pt, out atEndOfLine);
		}

		internal TextBoxContext MapPoint(Graphics g, PointF pt)
		{
			bool atEndOfLine;
			return MapPoint(g, pt, out atEndOfLine);
		}

		internal TextBoxContext MapPoint(Graphics g, PointF pt, out bool atEndOfLine)
		{
			TextBoxContext textBoxContext = null;
			atEndOfLine = false;
			textBoxContext = GetParagraphAndRunIndex(g, (int)pt.X, (int)pt.Y, out TextRun run, out int runX, out atEndOfLine);
			if (run != null)
			{
				GlyphData glyphData = run.GlyphData;
				GlyphShapeData glyphScriptShapeData = glyphData.GlyphScriptShapeData;
				if (glyphData != null && run.CharacterCount > 0)
				{
					int piCP = 0;
					int piTrailing = 0;
					int num = Win32.ScriptXtoCP(runX, run.CharacterCount, glyphScriptShapeData.GlyphCount, glyphScriptShapeData.Clusters, glyphScriptShapeData.VisAttrs, glyphData.Advances, ref run.SCRIPT_ANALYSIS, ref piCP, ref piTrailing);
					if (Win32.Failed(num))
					{
						Marshal.ThrowExceptionForHR(num);
					}
					if (run.ScriptAnalysis.fLayoutRTL == 1)
					{
						if (piCP == -1)
						{
							textBoxContext.TextRunCharacterIndex += run.CharacterCount;
						}
						else if (pt.X <= 0f)
						{
							TextBoxContext textBoxContext2 = textBoxContext;
							textBoxContext2.TextRunCharacterIndex = textBoxContext2.TextRunCharacterIndex;
						}
						else
						{
							textBoxContext.TextRunCharacterIndex += piCP + piTrailing;
						}
					}
					else
					{
						textBoxContext.TextRunCharacterIndex += piCP + piTrailing;
					}
				}
			}
			if (textBoxContext == null)
			{
				textBoxContext = new TextBoxContext();
			}
			return textBoxContext;
		}

		private TextBoxContext GetParagraphAndRunIndex(Graphics g, int x, int y, out TextRun run, out int runX, out bool atEndOfLine)
		{
			List<Paragraph> rTParagraphs = RTParagraphs;
			Paragraph paragraph = null;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int i = 0;
			bool flag = true;
			run = null;
			runX = 0;
			atEndOfLine = false;
			if (rTParagraphs == null || rTParagraphs.Count == 0)
			{
				return null;
			}
			if (y < 0)
			{
				y = 0;
			}
			for (; i < rTParagraphs.Count; i++)
			{
				paragraph = rTParagraphs[i];
				if ((y < paragraph.OffsetY || y >= paragraph.OffsetY + paragraph.Height) && i + 1 != rTParagraphs.Count)
				{
					continue;
				}
				y -= paragraph.OffsetY;
				num = TextBox.ConvertToPixels(paragraph.ParagraphProps.LeftIndent, Dpi);
				num2 = TextBox.ConvertToPixels(paragraph.ParagraphProps.RightIndent, Dpi);
				num3 = TextBox.ConvertToPixels(paragraph.ParagraphProps.HangingIndent, Dpi);
				flag = (m_rttextbox.TextBoxProps.Direction == RPLFormat.Directions.LTR);
				if (num3 < 0)
				{
					if (flag)
					{
						num -= num3;
					}
					else
					{
						num2 -= num3;
					}
				}
				int listLevel = paragraph.ParagraphProps.ListLevel;
				if (listLevel > 0)
				{
					int num4 = listLevel * TextBox.ConvertToPixels(10.583333f, Dpi);
					if (flag)
					{
						num += num4;
					}
					else
					{
						num2 += num4;
					}
				}
				break;
			}
			TextBoxContext paragraphAndRunIndex = GetParagraphAndRunIndex(g, paragraph, num, num2, num3, x, y, flag, i == rTParagraphs.Count - 1, out run, out runX, out atEndOfLine);
			if (paragraphAndRunIndex != null)
			{
				paragraphAndRunIndex.ParagraphIndex = i;
			}
			return paragraphAndRunIndex;
		}

		private TextBoxContext GetParagraphAndRunIndex(Graphics g, Paragraph paragraph, int leftIndent, int rightIndent, int hangingIndent, int x, int y, bool isLTR, bool lastParagraph, out TextRun run, out int runX, out bool atEndOfLine)
		{
			Win32DCSafeHandle win32DCSafeHandle = null;
			bool flag = false;
			try
			{
				if (g == null)
				{
					flag = true;
					g = Graphics.FromHwnd(IntPtr.Zero);
				}
				win32DCSafeHandle = new Win32DCSafeHandle(g.GetHdc(), ownsHandle: false);
				runX = x;
				run = null;
				atEndOfLine = false;
				for (int i = 0; i < paragraph.TextLines.Count; i++)
				{
					TextLine textLine = paragraph.TextLines[i];
					y -= textLine.GetHeight(win32DCSafeHandle, FontCache);
					if (textLine.FirstLine)
					{
						y -= TextBox.ConvertToPixels(paragraph.ParagraphProps.SpaceBefore, Dpi);
					}
					else if (textLine.LastLine)
					{
						y -= TextBox.ConvertToPixels(paragraph.ParagraphProps.SpaceAfter, Dpi);
					}
					if (y >= 0 && i + 1 != paragraph.TextLines.Count)
					{
						continue;
					}
					int width = textLine.GetWidth(win32DCSafeHandle, FontCache);
					if (x > WidthInPX)
					{
						atEndOfLine = (width > 0);
						run = GetLastNonLineBreakRun(textLine.LogicalRuns, out int charIndex, out bool hasNonCRLFChars);
						TextBoxContext textBoxContext = new TextBoxContext();
						textBoxContext.TextRunIndex = run.TextRunProperties.IndexInParagraph;
						if (hasNonCRLFChars && run.CharacterCount > 0)
						{
							textBoxContext.TextRunCharacterIndex = run.CharacterIndexInOriginal + charIndex + 1;
						}
						else
						{
							textBoxContext.TextRunCharacterIndex = run.CharacterIndexInOriginal;
						}
						run = null;
						return textBoxContext;
					}
					if (x < 0)
					{
						atEndOfLine = false;
						run = GetFirstNonLineBreakRun(textLine.LogicalRuns, out int charIndex2);
						TextBoxContext result = new TextBoxContext
						{
							TextRunIndex = run.TextRunProperties.IndexInParagraph,
							TextRunCharacterIndex = run.CharacterIndexInOriginal + charIndex2
						};
						run = null;
						return result;
					}
					RPLFormat.TextAlignments textAlignments = paragraph.ParagraphProps.Alignment;
					if (textAlignments == RPLFormat.TextAlignments.General)
					{
						textAlignments = m_rttextbox.TextBoxProps.DefaultAlignment;
						if (!isLTR)
						{
							switch (textAlignments)
							{
							case RPLFormat.TextAlignments.Left:
								textAlignments = RPLFormat.TextAlignments.Right;
								break;
							case RPLFormat.TextAlignments.Right:
								textAlignments = RPLFormat.TextAlignments.Left;
								break;
							}
						}
					}
					switch (textAlignments)
					{
					case RPLFormat.TextAlignments.Center:
						runX = x - (leftIndent + (WidthInPX - leftIndent - rightIndent) / 2 - width / 2);
						break;
					case RPLFormat.TextAlignments.Right:
						runX = x - (WidthInPX - width - rightIndent);
						break;
					default:
						runX = x - leftIndent;
						break;
					}
					if (textLine.FirstLine && hangingIndent != 0)
					{
						if (isLTR)
						{
							switch (textAlignments)
							{
							case RPLFormat.TextAlignments.Left:
								runX -= hangingIndent;
								break;
							case RPLFormat.TextAlignments.Center:
								runX -= hangingIndent / 2;
								break;
							}
						}
						else
						{
							switch (textAlignments)
							{
							case RPLFormat.TextAlignments.Right:
								runX += hangingIndent;
								break;
							case RPLFormat.TextAlignments.Center:
								runX += hangingIndent / 2;
								break;
							}
						}
					}
					runX = Math.Max(0, runX);
					return GetParagraphAndRunIndex(win32DCSafeHandle, paragraph, textLine, x, width, lastParagraph, out run, ref runX, out atEndOfLine);
				}
				return null;
			}
			finally
			{
				if (!win32DCSafeHandle.IsInvalid)
				{
					g.ReleaseHdc();
				}
				if (flag)
				{
					g.Dispose();
					g = null;
				}
			}
		}

		private TextRun GetLastNonLineBreakRun(List<TextRun> runs, out int charIndex, out bool hasNonCRLFChars)
		{
			for (int num = runs.Count - 1; num >= 0; num--)
			{
				TextRun textRun = runs[num];
				if (textRun.CharacterCount > 0)
				{
					string text = textRun.Text;
					for (charIndex = textRun.CharacterCount - 1; charIndex >= 0; charIndex--)
					{
						char c = text[charIndex];
						if (c != '\n' && c != '\r')
						{
							hasNonCRLFChars = true;
							return textRun;
						}
					}
				}
			}
			charIndex = 0;
			hasNonCRLFChars = false;
			return runs[runs.Count - 1];
		}

		private TextRun GetFirstNonLineBreakRun(List<TextRun> runs, out int charIndex)
		{
			TextRun textRun;
			for (int i = 0; i < runs.Count; i++)
			{
				textRun = runs[i];
				if (textRun.CharacterCount <= 0)
				{
					continue;
				}
				string text = textRun.Text;
				for (charIndex = 0; charIndex < textRun.CharacterCount; charIndex++)
				{
					char c = text[charIndex];
					if (c != '\n' && c != '\r')
					{
						return textRun;
					}
				}
			}
			textRun = runs[runs.Count - 1];
			if (textRun.CharacterCount > 0)
			{
				charIndex = textRun.CharacterCount - 1;
			}
			else
			{
				charIndex = 0;
			}
			return textRun;
		}

		private TextBoxContext GetParagraphAndRunIndex(Win32DCSafeHandle hdc, Paragraph paragraph, TextLine line, int x, int lineWidth, bool lastParagraph, out TextRun run, ref int runX, out bool atEndOfLine)
		{
			atEndOfLine = false;
			run = null;
			int count = line.VisualRuns.Count;
			for (int i = 0; i < count; i++)
			{
				run = line.VisualRuns[i];
				int width = run.GetWidth(hdc, FontCache, i == count - 1);
				if (runX - width <= 0 || i + 1 == count)
				{
					if (runX - width > 0)
					{
						atEndOfLine = true;
						if (run.ScriptAnalysis.fLayoutRTL == 1 && x >= lineWidth)
						{
							run = line.VisualRuns[count - 1];
							return new TextBoxContext
							{
								TextRunIndex = run.TextRunProperties.IndexInParagraph,
								TextRunCharacterIndex = run.CharacterIndexInOriginal
							};
						}
					}
					int num = 0;
					int characterCount = run.CharacterCount;
					if (characterCount > 0)
					{
						if ((run.ScriptAnalysis.fLayoutRTL == 0 && i + 1 == count) || (run.ScriptAnalysis.fLayoutRTL == 1 && i == 0))
						{
							if (width <= 0)
							{
								string text = run.Text;
								if (characterCount > 1 && text[characterCount - 2] == '\r' && text[characterCount - 1] == '\n')
								{
									num = ((run.ScriptAnalysis.fLayoutRTL != 0) ? (-1) : (-2));
								}
								else if (text[characterCount - 1] == '\n' && run.ScriptAnalysis.fLayoutRTL == 0)
								{
									if (i > 0)
									{
										run = line.VisualRuns[i - 1];
										if (run.CharacterCount > 0 && run.Text[run.CharacterCount - 1] == '\r')
										{
											num = -1;
										}
									}
									else
									{
										num = -1;
									}
								}
							}
							else if (i + 1 == count && (run.Text[characterCount - 1] == '\r' || run.Text[characterCount - 1] == '\n'))
							{
								num = -1;
							}
							else if (i == 0 && (run.Text[0] == '\r' || run.Text[0] == '\n'))
							{
								num = -1;
							}
						}
						else if (i == 0 && (run.Text[0] == '\r' || run.Text[0] == '\n'))
						{
							num = -1;
						}
					}
					else if (lastParagraph && line.LastLine && !line.FirstLine)
					{
						num = 1;
					}
					return new TextBoxContext
					{
						TextRunIndex = run.TextRunProperties.IndexInParagraph,
						TextRunCharacterIndex = run.CharacterIndexInOriginal + num
					};
				}
				runX -= width;
			}
			return null;
		}

		internal CaretInfo MapLocation(TextBoxContext location)
		{
			return MapLocation(null, location, moveCaretToNextLine: false);
		}

		internal CaretInfo MapLocation(Graphics g, TextBoxContext location)
		{
			return MapLocation(g, location, moveCaretToNextLine: false);
		}

		internal CaretInfo MapLocation(TextBoxContext location, bool moveCaretToNextLine)
		{
			return MapLocation(null, location, moveCaretToNextLine);
		}

		internal CaretInfo MapLocation(Graphics g, TextBoxContext location, bool moveCaretToNextLine)
		{
			TextRun run;
			return MapLocation(g, location, relativeToRun: false, moveCaretToNextLine, out run);
		}

		private CaretInfo MapLocation(Graphics g, TextBoxContext location, bool relativeToRun, bool moveCaretToNextLine, out TextRun run)
		{
			Win32DCSafeHandle win32DCSafeHandle = null;
			bool flag = false;
			try
			{
				if (g == null)
				{
					flag = true;
					g = Graphics.FromHwnd(IntPtr.Zero);
				}
				win32DCSafeHandle = new Win32DCSafeHandle(g.GetHdc(), ownsHandle: false);
				return MapLocation(win32DCSafeHandle, location, relativeToRun, moveCaretToNextLine, out run);
			}
			finally
			{
				if (!win32DCSafeHandle.IsInvalid)
				{
					g.ReleaseHdc();
				}
				if (flag)
				{
					g.Dispose();
					g = null;
				}
			}
		}

		private CaretInfo MapLocation(Win32DCSafeHandle hdc, TextBoxContext location, bool relativeToRun, bool moveCaretToNextLine, out TextRun run)
		{
			CaretInfo caretInfo = null;
			run = null;
			int lineYOffset;
			int lineHeight;
			int textRunCharacterIndex;
			bool isFirstLine;
			bool isLastLine;
			Point paragraphAndRunCoordinates = GetParagraphAndRunCoordinates(hdc, location, moveCaretToNextLine, out lineYOffset, out lineHeight, out run, out textRunCharacterIndex, out isFirstLine, out isLastLine);
			if (run != null)
			{
				GlyphData glyphData = run.GlyphData;
				GlyphShapeData glyphScriptShapeData = glyphData.GlyphScriptShapeData;
				int piX = 0;
				if (glyphData != null && run.CharacterCount > 0)
				{
					int num = Win32.ScriptCPtoX(textRunCharacterIndex, fTrailing: false, run.CharacterCount, glyphScriptShapeData.GlyphCount, glyphScriptShapeData.Clusters, glyphScriptShapeData.VisAttrs, glyphData.Advances, ref run.SCRIPT_ANALYSIS, ref piX);
					if (Win32.Failed(num))
					{
						Marshal.ThrowExceptionForHR(num);
					}
				}
				caretInfo = new CaretInfo();
				CachedFont cachedFont = run.GetCachedFont(hdc, FontCache);
				caretInfo.Height = cachedFont.GetHeight(hdc, FontCache);
				caretInfo.Ascent = cachedFont.GetAscent(hdc, FontCache);
				caretInfo.Descent = cachedFont.GetDescent(hdc, FontCache);
				caretInfo.LineHeight = lineHeight;
				caretInfo.LineYOffset = lineYOffset;
				caretInfo.IsFirstLine = isFirstLine;
				caretInfo.IsLastLine = isLastLine;
				_ = RTParagraphs;
				int y = paragraphAndRunCoordinates.Y - caretInfo.Ascent;
				if (relativeToRun)
				{
					caretInfo.Position = new Point(piX, y);
				}
				else
				{
					caretInfo.Position = new Point(paragraphAndRunCoordinates.X + piX, y);
				}
			}
			return caretInfo;
		}

		private Point GetParagraphAndRunCoordinates(Win32DCSafeHandle hdc, TextBoxContext location, bool moveCaretToNextLine, out int lineYOffset, out int lineHeight, out TextRun textRun, out int textRunCharacterIndex, out bool isFirstLine, out bool isLastLine)
		{
			int num = 0;
			int textRunIndex = location.TextRunIndex;
			textRunCharacterIndex = location.TextRunCharacterIndex;
			lineYOffset = 0;
			lineHeight = 0;
			textRun = null;
			isFirstLine = true;
			isLastLine = true;
			List<Paragraph> rTParagraphs = RTParagraphs;
			if (rTParagraphs == null || location.ParagraphIndex >= rTParagraphs.Count)
			{
				return Point.Empty;
			}
			Paragraph paragraph = rTParagraphs[location.ParagraphIndex];
			int num2 = paragraph.OffsetY + TextBox.ConvertToPixels(paragraph.ParagraphProps.SpaceBefore, Dpi);
			int num3 = TextBox.ConvertToPixels(paragraph.ParagraphProps.LeftIndent, Dpi);
			int num4 = TextBox.ConvertToPixels(paragraph.ParagraphProps.RightIndent, Dpi);
			int num5 = TextBox.ConvertToPixels(paragraph.ParagraphProps.HangingIndent, Dpi);
			bool flag = m_rttextbox.TextBoxProps.Direction == RPLFormat.Directions.LTR;
			if (num5 < 0)
			{
				if (flag)
				{
					num3 -= num5;
				}
				else
				{
					num4 -= num5;
				}
			}
			int listLevel = paragraph.ParagraphProps.ListLevel;
			if (listLevel > 0)
			{
				int num6 = listLevel * TextBox.ConvertToPixels(10.583333f, Dpi);
				if (flag)
				{
					num3 += num6;
				}
				else
				{
					num4 += num6;
				}
			}
			for (int i = 0; i < paragraph.TextLines.Count; i++)
			{
				TextLine textLine = paragraph.TextLines[i];
				int descent = textLine.GetDescent(hdc, FontCache);
				lineHeight = textLine.GetHeight(hdc, FontCache);
				lineYOffset = num2;
				num2 += lineHeight;
				RPLFormat.TextAlignments textAlignments = paragraph.ParagraphProps.Alignment;
				if (textAlignments == RPLFormat.TextAlignments.General)
				{
					textAlignments = m_rttextbox.TextBoxProps.DefaultAlignment;
					if (!flag)
					{
						switch (textAlignments)
						{
						case RPLFormat.TextAlignments.Left:
							textAlignments = RPLFormat.TextAlignments.Right;
							break;
						case RPLFormat.TextAlignments.Right:
							textAlignments = RPLFormat.TextAlignments.Left;
							break;
						}
					}
				}
				switch (textAlignments)
				{
				case RPLFormat.TextAlignments.Center:
					num = num3 + (WidthInPX - num3 - num4) / 2 - textLine.GetWidth(hdc, FontCache) / 2;
					break;
				case RPLFormat.TextAlignments.Right:
					num = WidthInPX - textLine.GetWidth(hdc, FontCache) - num4;
					break;
				default:
					num = num3;
					break;
				}
				if (textLine.FirstLine && num5 != 0)
				{
					if (flag)
					{
						switch (textAlignments)
						{
						case RPLFormat.TextAlignments.Left:
							num += num5;
							break;
						case RPLFormat.TextAlignments.Center:
							num += num5 / 2;
							break;
						}
					}
					else
					{
						switch (textAlignments)
						{
						case RPLFormat.TextAlignments.Right:
							num -= num5;
							break;
						case RPLFormat.TextAlignments.Center:
							num -= num5 / 2;
							break;
						}
					}
				}
				int count = textLine.VisualRuns.Count;
				for (int j = 0; j < count; j++)
				{
					textRun = textLine.VisualRuns[j];
					if (textRun.TextRunProperties.IndexInParagraph == textRunIndex && textRunCharacterIndex >= textRun.CharacterIndexInOriginal)
					{
						bool flag2 = (moveCaretToNextLine || textRun.CharacterCount <= 0 || textRun.Text[textRun.CharacterCount - 1] != '\n') ? (textRunCharacterIndex <= textRun.CharacterIndexInOriginal + textRun.CharacterCount) : (textRunCharacterIndex < textRun.CharacterIndexInOriginal + textRun.CharacterCount);
						if (flag2 || (i + 1 == paragraph.TextLines.Count && j + 1 == count))
						{
							if (moveCaretToNextLine && textRunCharacterIndex == textRun.CharacterIndexInOriginal + textRun.CharacterCount && j + 1 == count && i + 1 < paragraph.TextLines.Count)
							{
								location = location.Clone();
								if (paragraph.TextLines[i + 1].VisualRuns[0].TextRunProperties.IndexInParagraph != textRunIndex)
								{
									location.TextRunIndex++;
									location.TextRunCharacterIndex = 0;
								}
								Point paragraphAndRunCoordinates = GetParagraphAndRunCoordinates(hdc, location, moveCaretToNextLine: false, out lineYOffset, out lineHeight, out textRun, out textRunCharacterIndex, out isFirstLine, out isLastLine);
								textRunCharacterIndex = Math.Max(textRunCharacterIndex - 1, 0);
								return paragraphAndRunCoordinates;
							}
							textRunCharacterIndex -= textRun.CharacterIndexInOriginal;
							isFirstLine = textLine.FirstLine;
							isLastLine = textLine.LastLine;
							return new Point(num, num2 - descent);
						}
					}
					num += textRun.GetWidth(hdc, FontCache);
				}
			}
			textRun = null;
			return Point.Empty;
		}

		internal float GetHeight()
		{
			if (m_height == 0f)
			{
				UpdateCachedObjects();
			}
			return m_height;
		}

		internal int GetHeightInPX()
		{
			return TextBox.ConvertToPixels(GetHeight(), Dpi);
		}

		internal void SetTextbox(TextBox textbox)
		{
			m_rttextbox = textbox;
			ResetCachedObjects();
		}

		private void ResetCachedObjects()
		{
			m_cachedrtparagraphs = null;
			m_height = 0f;
		}

		private void UpdateCachedObjects()
		{
			using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
			{
				UpdateCachedObjects(g);
			}
		}

		private void UpdateCachedObjects(Graphics g)
		{
			using (new PerfCheck("UpdateCachedObjects"))
			{
				using (new PerfCheck("ScriptItemize"))
				{
					RTTextbox.ScriptItemize();
				}
				using (new PerfCheck("Flow"))
				{
					FlowContext flowContext = new FlowContext(Width, FlowHeight);
					flowContext.LineLimit = LineLimit;
					flowContext.CharTrimLastLine = CharTrimLastLine;
					m_cachedrtparagraphs = LineBreaker.Flow(RTTextbox, g, FontCache, flowContext, keepLines: true, out m_height);
				}
			}
		}

		public void Dispose()
		{
			if (m_fontCache != null)
			{
				m_fontCache.Dispose();
				m_fontCache = null;
			}
			GC.SuppressFinalize(this);
		}
	}
}

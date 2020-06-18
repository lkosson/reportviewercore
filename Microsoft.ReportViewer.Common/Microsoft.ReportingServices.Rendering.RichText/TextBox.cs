using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal sealed class TextBox
	{
		internal const float PrefixIndent = 10.583333f;

		internal const float PrefixSpace = 4.233333f;

		internal const float INCH_TO_MILLIMETER = 25.4f;

		internal const float RoundDelta = 0.1f;

		private List<Paragraph> m_paragraphs;

		private ITextBoxProps m_textBoxProps;

		internal List<Paragraph> Paragraphs
		{
			get
			{
				return m_paragraphs;
			}
			set
			{
				m_paragraphs = value;
			}
		}

		internal ITextBoxProps TextBoxProps => m_textBoxProps;

		internal bool VerticalText
		{
			get
			{
				if (m_textBoxProps.WritingMode != RPLFormat.WritingModes.Vertical)
				{
					return m_textBoxProps.WritingMode == RPLFormat.WritingModes.Rotate270;
				}
				return true;
			}
		}

		internal bool HorizontalText => m_textBoxProps.WritingMode == RPLFormat.WritingModes.Horizontal;

		internal string Value
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < m_paragraphs.Count; i++)
				{
					Paragraph paragraph = m_paragraphs[i];
					for (int j = 0; j < paragraph.Runs.Count; j++)
					{
						TextRun textRun = paragraph.Runs[j];
						stringBuilder.Append(textRun.Text);
					}
				}
				return stringBuilder.ToString();
			}
		}

		private TextBox()
		{
		}

		internal TextBox(ITextBoxProps textBoxProps)
		{
			m_textBoxProps = textBoxProps;
		}

		internal static float MeasureFullHeight(TextBox textBox, Graphics g, FontCache fontCache, FlowContext flowContext, out float contentHeight)
		{
			if (flowContext.Width <= 0f)
			{
				contentHeight = 0f;
				return 0f;
			}
			FlowContext flowContext2 = flowContext.Clone();
			flowContext2.Reset();
			flowContext2.Height = float.MaxValue;
			float num = 0f;
			if (textBox.VerticalText)
			{
				flowContext2.LineLimit = false;
				float width = 0f;
				float nextWidth = 0f;
				float num2 = 0f;
				float num3 = flowContext.Height;
				float num4 = 0f;
				if (flowContext.Height == float.MaxValue)
				{
					num3 = 0f;
				}
				num2 = (contentHeight = LineBreaker.FlowVertical(textBox, g, fontCache, flowContext2, out width, out nextWidth));
				num = width;
				num4 = width;
				bool flag = flowContext2.CharTrimmedRunWidth > 0;
				float num5 = 0f;
				while (num2 < flowContext.Width && width > num3)
				{
					flowContext2.Reset();
					flowContext2.Height = Math.Max(nextWidth, num3);
					flowContext2.Width = float.MaxValue;
					num2 = LineBreaker.FlowVertical(textBox, g, fontCache, flowContext2, out width, out nextWidth);
					if (num2 < flowContext.Width)
					{
						num5 = num2 - contentHeight;
						if (num2 <= contentHeight || num5 <= 0.1f)
						{
							if (flag)
							{
								contentHeight = num2;
								num = flowContext2.Height;
								break;
							}
							if (flowContext2.CharTrimmedRunWidth <= 0)
							{
								break;
							}
						}
						contentHeight = num2;
						num = flowContext2.Height;
						num4 = width;
					}
					else
					{
						num5 = num - flowContext2.Height;
						if (!(num5 > 0.1f))
						{
							break;
						}
						nextWidth = flowContext2.Height + (num - flowContext2.Height) / 2f;
						contentHeight = num2;
						width = num4;
					}
					flag = (flowContext2.CharTrimmedRunWidth > 0);
				}
			}
			else
			{
				LineBreaker.Flow(textBox, g, fontCache, flowContext2, keepLines: false, out contentHeight);
				num = contentHeight;
			}
			return num;
		}

		internal static void Render(TextBox textBox, List<Paragraph> paragraphs, Graphics g, FontCache fontCache, PointF offset, RectangleF layoutRectangle)
		{
			float dpiX = g.DpiX;
			Win32DCSafeHandle win32DCSafeHandle = new Win32DCSafeHandle(g.GetHdc(), ownsHandle: false);
			try
			{
				Render(textBox, paragraphs, win32DCSafeHandle, fontCache, offset, layoutRectangle, dpiX);
			}
			finally
			{
				fontCache.ResetGraphics();
				if (!win32DCSafeHandle.IsInvalid)
				{
					g.ReleaseHdc();
				}
			}
		}

		internal static void Render(TextBox textBox, List<Paragraph> paragraphs, Win32DCSafeHandle hdc, FontCache fontCache, PointF offset, RectangleF layoutRectangle, float dpiX)
		{
			Render(textBox, paragraphs, hdc, fontCache, offset, layoutRectangle, dpiX, unitsInMM: true);
		}

		internal static void Render(TextBox textBox, List<Paragraph> paragraphs, Win32DCSafeHandle hdc, FontCache fontCache, PointF offset, RectangleF layoutRectangle, float dpiX, bool unitsInMM)
		{
			if (paragraphs == null || paragraphs.Count == 0)
			{
				return;
			}
			Rectangle layoutRectangle2;
			Point point;
			if (!unitsInMM)
			{
				layoutRectangle2 = new Rectangle((int)layoutRectangle.X, (int)layoutRectangle.Y, (int)layoutRectangle.Width, (int)layoutRectangle.Height);
				point = new Point((int)offset.X, (int)offset.Y);
			}
			else
			{
				layoutRectangle2 = new Rectangle(ConvertToPixels(layoutRectangle.X, dpiX), ConvertToPixels(layoutRectangle.Y, dpiX), ConvertToPixels(layoutRectangle.Width, dpiX), ConvertToPixels(layoutRectangle.Height, dpiX));
				point = new Point(ConvertToPixels(offset.X, dpiX), ConvertToPixels(offset.Y, dpiX));
			}
			uint fMode = Win32.SetTextAlign(hdc, 24u);
			int iBkMode = Win32.SetBkMode(hdc, 1);
			Win32ObjectSafeHandle win32ObjectSafeHandle = Win32.SelectObject(hdc, Win32ObjectSafeHandle.Zero);
			try
			{
				fontCache.WritingMode = textBox.TextBoxProps.WritingMode;
				int offsetY = point.Y;
				for (int i = 0; i < paragraphs.Count; i++)
				{
					RenderParagraph(textBox, paragraphs[i], hdc, fontCache, point.X, ref offsetY, layoutRectangle2, dpiX);
				}
			}
			finally
			{
				fMode = Win32.SetTextAlign(hdc, fMode);
				iBkMode = Win32.SetBkMode(hdc, iBkMode);
				if (!win32ObjectSafeHandle.IsInvalid)
				{
					Win32.SelectObject(hdc, win32ObjectSafeHandle).SetHandleAsInvalid();
					win32ObjectSafeHandle.SetHandleAsInvalid();
				}
			}
		}

		private static void RenderParagraph(TextBox textBox, Paragraph paragraph, Win32DCSafeHandle hdc, FontCache fontCache, int offsetX, ref int offsetY, Rectangle layoutRectangle, float dpiX)
		{
			List<TextLine> textLines = paragraph.TextLines;
			IParagraphProps paragraphProps = paragraph.ParagraphProps;
			bool flag = textBox.TextBoxProps.Direction == RPLFormat.Directions.LTR;
			RPLFormat.TextAlignments textAlignments = paragraphProps.Alignment;
			if (textAlignments == RPLFormat.TextAlignments.General)
			{
				textAlignments = textBox.TextBoxProps.DefaultAlignment;
				if (!flag)
				{
					switch (textAlignments)
					{
					case RPLFormat.TextAlignments.Right:
						textAlignments = RPLFormat.TextAlignments.Left;
						break;
					case RPLFormat.TextAlignments.Left:
						textAlignments = RPLFormat.TextAlignments.Right;
						break;
					}
				}
			}
			int num = ConvertToPixels(paragraphProps.LeftIndent, dpiX);
			int num2 = ConvertToPixels(paragraphProps.RightIndent, dpiX);
			int num3 = ConvertToPixels(paragraphProps.HangingIndent, dpiX);
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
			if (paragraphProps.ListLevel > 0)
			{
				int num4 = paragraphProps.ListLevel * ConvertToPixels(10.583333f, dpiX);
				if (flag)
				{
					num += num4;
				}
				else
				{
					num2 += num4;
				}
			}
			if (textLines == null || textLines.Count == 0)
			{
				offsetY += ConvertToPixels(paragraphProps.SpaceBefore, dpiX);
				offsetY += ConvertToPixels(paragraphProps.SpaceAfter, dpiX);
				return;
			}
			Graphics graphics = null;
			try
			{
				for (int i = 0; i < textLines.Count; i++)
				{
					TextLine textLine = textLines[i];
					int ascent = textLine.GetAscent(hdc, fontCache);
					textLine.GetDescent(hdc, fontCache);
					int height = textLine.GetHeight(hdc, fontCache);
					if (textLine.FirstLine)
					{
						offsetY += ConvertToPixels(paragraphProps.SpaceBefore, dpiX);
					}
					int baselineY = offsetY + ascent;
					offsetY += height;
					int num5 = offsetX;
					switch (textAlignments)
					{
					case RPLFormat.TextAlignments.Left:
						num5 = num;
						break;
					case RPLFormat.TextAlignments.Center:
					{
						int num6 = 0;
						num6 = ((!textBox.HorizontalText) ? layoutRectangle.Height : layoutRectangle.Width);
						num5 = num + (num6 - num - num2) / 2 - textLine.GetWidth(hdc, fontCache) / 2;
						break;
					}
					default:
						num5 = ((!textBox.HorizontalText) ? (layoutRectangle.Height - num2 - textLine.GetWidth(hdc, fontCache)) : (layoutRectangle.Width - num2 - textLine.GetWidth(hdc, fontCache)));
						break;
					}
					if (textLine.Prefix != null && textLine.Prefix.Count > 0)
					{
						int num7 = (!flag) ? (num5 + textLine.GetWidth(hdc, fontCache) + ConvertToPixels(4.233333f, dpiX)) : (num5 - ConvertToPixels(4.233333f, dpiX) - textLine.GetPrefixWidth(hdc, fontCache));
						if (num3 < 0)
						{
							if (flag && textAlignments == RPLFormat.TextAlignments.Left)
							{
								num7 += num3;
							}
							else if (!flag && textAlignments == RPLFormat.TextAlignments.Right)
							{
								num7 -= num3;
							}
						}
						for (int j = 0; j < textLine.Prefix.Count; j++)
						{
							TextRun textRun = textLine.Prefix[j];
							textBox.TextBoxProps.DrawTextRun(textRun, paragraph, hdc, dpiX, fontCache, num7, offsetY, baselineY, height, layoutRectangle);
							num7 += textRun.GetWidth(hdc, fontCache);
						}
					}
					if (textLine.FirstLine && num3 != 0)
					{
						if (flag)
						{
							switch (textAlignments)
							{
							case RPLFormat.TextAlignments.Left:
								num5 += num3;
								break;
							case RPLFormat.TextAlignments.Center:
								num5 += num3 / 2;
								break;
							}
						}
						else
						{
							switch (textAlignments)
							{
							case RPLFormat.TextAlignments.Right:
								num5 -= num3;
								break;
							case RPLFormat.TextAlignments.Center:
								num5 -= num3 / 2;
								break;
							}
						}
					}
					int prevRunWidth = 0;
					int prevRunX = 0;
					TextRun prevRun = null;
					int count = textLine.VisualRuns.Count;
					for (int k = 0; k < count; k++)
					{
						TextRun textRun2 = textLine.VisualRuns[k];
						int width = textRun2.GetWidth(hdc, fontCache, k == count - 1);
						if (!textRun2.IsHighlightTextRun)
						{
							if (width > 0)
							{
								textBox.TextBoxProps.DrawTextRun(textRun2, paragraph, hdc, dpiX, fontCache, num5, offsetY, baselineY, height, layoutRectangle);
							}
						}
						else
						{
							bool flag2 = (flag && k + 1 == count) || (!flag && k == 0);
							if (width > 0 || flag2)
							{
								if (graphics == null)
								{
									graphics = Graphics.FromHdc(hdc.Handle);
								}
								RenderHighlightedTextRun(textBox, paragraph, textRun2, prevRun, hdc, graphics, fontCache, dpiX, num5, offsetY, baselineY, height, layoutRectangle, width, prevRunWidth, prevRunX, flag2, textLine.LastLine);
							}
						}
						prevRunX = num5;
						prevRunWidth = width;
						num5 += width;
						prevRun = textRun2;
					}
					if (textLine.LastLine)
					{
						offsetY += ConvertToPixels(paragraphProps.SpaceAfter, dpiX);
					}
				}
			}
			finally
			{
				if (graphics != null)
				{
					graphics.Dispose();
					graphics = null;
				}
			}
		}

		private static void RenderHighlightedTextRun(TextBox textBox, Paragraph paragraph, TextRun run, TextRun prevRun, Win32DCSafeHandle hdc, Graphics g, FontCache fontCache, float dpiX, int x, int offsetY, int baselineY, int lineHeight, Rectangle layoutRectangle, int runWidth, int prevRunWidth, int prevRunX, bool lastRunInLine, bool lastLineInParagraph)
		{
			uint? num = null;
			Rectangle? rectangle = null;
			bool flag = false;
			Color color = run.HighlightColor;
			if (!color.IsEmpty)
			{
				int num2 = (run.HighlightStart >= 0) ? run.HighlightStart : 0;
				int num3 = (run.HighlightEnd >= 0) ? run.HighlightEnd : runWidth;
				if (lastRunInLine)
				{
					bool flag2 = run.ScriptAnalysis.fLayoutRTL == 1;
					if (lastLineInParagraph && runWidth != 0)
					{
						if (num2 != num3 && ((flag2 && run.HighlightStart < 0) || (!flag2 && run.HighlightEnd < 0)))
						{
							if (flag2)
							{
								num2 -= 5;
								int abcA = run.GetGlyphData(hdc, fontCache).ABC.abcA;
								if (abcA < 0)
								{
									num2 += abcA;
								}
							}
							else
							{
								num3 += 5;
								int abcC = run.GetGlyphData(hdc, fontCache).ABC.abcC;
								if (abcC < 0)
								{
									num3 -= abcC;
								}
							}
						}
					}
					else if (runWidth == 0)
					{
						if (flag2)
						{
							num2 -= 5;
						}
						else
						{
							num3 += 5;
						}
					}
				}
				if (num2 != num3)
				{
					if (num2 == 0 && prevRun != null && (prevRun.GetGlyphData(hdc, fontCache).ABC.abcC <= 0 || run.GetGlyphData(hdc, fontCache).ABC.abcA < 0))
					{
						flag = true;
					}
					if (run.AllowColorInversion && NeedsColorInversion(color, textBox.TextBoxProps.BackgroundColor))
					{
						color = InvertColor(textBox.TextBoxProps.BackgroundColor);
					}
					using (Brush brush = new SolidBrush(color))
					{
						rectangle = ((!textBox.HorizontalText) ? new Rectangle?(new Rectangle(layoutRectangle.Right - offsetY, layoutRectangle.Y + x + num2, lineHeight, num3 - num2)) : new Rectangle?(new Rectangle(layoutRectangle.X + x + num2, layoutRectangle.Y + offsetY - lineHeight, num3 - num2, lineHeight)));
						g.FillRectangle(brush, rectangle.Value);
					}
					if (run.AllowColorInversion && NeedsColorInversion(color, run.TextRunProperties.Color))
					{
						Color color2 = InvertColor(run.TextRunProperties.Color);
						num = (uint)((color2.B << 16) | (color2.G << 8) | color2.R);
					}
				}
				run.HighlightColor = Color.Empty;
			}
			if (runWidth > 0)
			{
				textBox.TextBoxProps.DrawTextRun(run, paragraph, hdc, dpiX, fontCache, x, offsetY, baselineY, lineHeight, layoutRectangle);
				if (num.HasValue)
				{
					textBox.TextBoxProps.DrawClippedTextRun(run, paragraph, hdc, dpiX, fontCache, x, offsetY, baselineY, lineHeight, layoutRectangle, num.Value, rectangle.Value);
				}
			}
			if (flag)
			{
				Rectangle empty = Rectangle.Empty;
				empty = (textBox.HorizontalText ? new Rectangle(layoutRectangle.X + x, layoutRectangle.Y + offsetY - lineHeight, prevRunWidth, lineHeight) : new Rectangle(layoutRectangle.Right - offsetY, layoutRectangle.Y + x, lineHeight, prevRunWidth));
				Color color3 = prevRun.TextRunProperties.Color;
				if (!run.AllowColorInversion || !NeedsColorInversion(color, color3))
				{
					num = prevRun.ColorInt;
				}
				else
				{
					color3 = InvertColor(color3);
					num = (uint)((color3.B << 16) | (color3.G << 8) | color3.R);
				}
				textBox.TextBoxProps.DrawClippedTextRun(prevRun, paragraph, hdc, dpiX, fontCache, prevRunX, offsetY, baselineY, lineHeight, layoutRectangle, num.Value, empty);
			}
		}

		private static bool NeedsColorInversion(Color color1, Color color2)
		{
			if (color2.IsEmpty || color2 == Color.Transparent)
			{
				return false;
			}
			return Math.Abs(color1.R - color2.R) + Math.Abs(color1.G - color2.G) + Math.Abs(color1.B - color2.B) < 192;
		}

		private static Color InvertColor(Color color)
		{
			int num = 255 - color.R;
			if (num >= 118 && num <= 138)
			{
				num = 117;
			}
			int num2 = 255 - color.G;
			if (num2 >= 118 && num2 <= 138)
			{
				num2 = 117;
			}
			int num3 = 255 - color.B;
			if (num3 >= 118 && num3 <= 138)
			{
				num3 = 117;
			}
			return Color.FromArgb(color.A, num, num2, num3);
		}

		internal static void DrawTextRun(TextRun run, Win32DCSafeHandle hdc, FontCache fontCache, int x, int baselineY, Underline underline)
		{
			uint crColor = 0u;
			try
			{
				uint colorInt = run.ColorInt;
				underline?.Draw(hdc, (int)((double)run.UnderlineHeight * 0.085), colorInt);
				crColor = Win32.SetTextColor(hdc, colorInt);
				GlyphData glyphData = run.GetGlyphData(hdc, fontCache);
				GlyphShapeData glyphScriptShapeData = glyphData.GlyphScriptShapeData;
				CachedFont cachedFont = run.GetCachedFont(hdc, fontCache);
				fontCache.SelectFontObject(hdc, cachedFont.Hfont);
				int num = Win32.ScriptTextOut(hdc, ref cachedFont.ScriptCache, x, baselineY, 4u, IntPtr.Zero, ref run.SCRIPT_ANALYSIS, IntPtr.Zero, 0, glyphScriptShapeData.Glyphs, glyphScriptShapeData.GlyphCount, glyphData.Advances, null, glyphData.GOffsets);
				if (Win32.Failed(num))
				{
					Marshal.ThrowExceptionForHR(num);
				}
			}
			finally
			{
				crColor = Win32.SetTextColor(hdc, crColor);
			}
		}

		internal static void ExtDrawTextRun(TextRun run, Win32DCSafeHandle hdc, FontCache fontCache, int x, int baselineY, Underline underline)
		{
			uint crColor = 0u;
			try
			{
				uint colorInt = run.ColorInt;
				underline?.Draw(hdc, (int)((double)run.UnderlineHeight * 0.085), colorInt);
				crColor = Win32.SetTextColor(hdc, colorInt);
				CachedFont cachedFont = run.GetCachedFont(hdc, fontCache);
				fontCache.SelectFontObject(hdc, cachedFont.Hfont);
				int[] lpDx = null;
				uint fuOptions = 0u;
				if (run.ScriptAnalysis.fRTL == 1)
				{
					fuOptions = 128u;
				}
				if (!Win32.ExtTextOut(hdc, x, baselineY, fuOptions, IntPtr.Zero, run.Text, (uint)run.Text.Length, lpDx))
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
			}
			finally
			{
				crColor = Win32.SetTextColor(hdc, crColor);
			}
		}

		internal static void DrawClippedTextRun(TextRun run, Win32DCSafeHandle hdc, FontCache fontCache, int x, int baselineY, uint fontColorOverride, Rectangle clipRect, Underline underline)
		{
			uint crColor = 0u;
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				underline?.Draw(hdc, (int)((double)run.UnderlineHeight * 0.085), fontColorOverride);
				RECT structure = default(RECT);
				structure.left = clipRect.Left;
				structure.right = clipRect.Right;
				structure.top = clipRect.Top;
				structure.bottom = clipRect.Bottom;
				crColor = Win32.SetTextColor(hdc, fontColorOverride);
				GlyphData glyphData = run.GetGlyphData(hdc, fontCache);
				GlyphShapeData glyphScriptShapeData = glyphData.GlyphScriptShapeData;
				CachedFont cachedFont = run.GetCachedFont(hdc, fontCache);
				fontCache.SelectFontObject(hdc, cachedFont.Hfont);
				intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(structure));
				Marshal.StructureToPtr(structure, intPtr, fDeleteOld: false);
				int num = Win32.ScriptTextOut(hdc, ref cachedFont.ScriptCache, x, baselineY, 4u, intPtr, ref run.SCRIPT_ANALYSIS, IntPtr.Zero, 0, glyphScriptShapeData.Glyphs, glyphScriptShapeData.GlyphCount, glyphData.Advances, null, glyphData.GOffsets);
				if (Win32.Failed(num))
				{
					Marshal.ThrowExceptionForHR(num);
				}
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
				crColor = Win32.SetTextColor(hdc, crColor);
			}
		}

		internal static int ConvertToPixels(float mm, float dpi)
		{
			if (mm == float.MaxValue)
			{
				return int.MaxValue;
			}
			return Convert.ToInt32((double)dpi * 0.03937007874 * (double)mm);
		}

		internal static float ConvertToMillimeters(int pixels, float dpi)
		{
			if (dpi == 0f)
			{
				return float.MaxValue;
			}
			return 1f / dpi * (float)pixels * 25.4f;
		}

		internal static float ConvertToPoints(float pixels, float dpi)
		{
			if (dpi == 0f)
			{
				return float.MaxValue;
			}
			return pixels * 72f / dpi;
		}

		internal static bool IsWhitespaceControlChar(char c)
		{
			switch (c)
			{
			case '\n':
			case '\v':
			case '\f':
			case '\r':
			case '\u0085':
				return true;
			default:
				return false;
			}
		}

		internal void ScriptItemize()
		{
			RPLFormat.Directions direction = m_textBoxProps.Direction;
			for (int i = 0; i < m_paragraphs.Count; i++)
			{
				m_paragraphs[i].ScriptItemize(direction);
			}
		}
	}
}

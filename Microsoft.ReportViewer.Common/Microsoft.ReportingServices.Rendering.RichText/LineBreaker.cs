using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal sealed class LineBreaker
	{
		internal static readonly char[] BulletChars = new char[3]
		{
			'•',
			'◦',
			'▪'
		};

		private static readonly char[,] RomanNumerals = new char[3, 3]
		{
			{
				'i',
				'v',
				'x'
			},
			{
				'x',
				'l',
				'c'
			},
			{
				'c',
				'd',
				'm'
			}
		};

		private LineBreaker()
		{
		}

		internal static List<Paragraph> Flow(TextBox textBox, Graphics g, FontCache fontCache, FlowContext flowContext, bool keepLines)
		{
			float height;
			return Flow(textBox, g, fontCache, flowContext, keepLines, out height);
		}

		internal static List<Paragraph> Flow(TextBox textBox, Graphics g, FontCache fontCache, FlowContext flowContext, bool keepLines, out float height)
		{
			List<Paragraph> list = null;
			float dpiX = g.DpiX;
			Win32DCSafeHandle win32DCSafeHandle = new Win32DCSafeHandle(g.GetHdc(), ownsHandle: false);
			try
			{
				return Flow(textBox, win32DCSafeHandle, dpiX, fontCache, flowContext, keepLines, out height);
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

		internal static List<Paragraph> Flow(TextBox textBox, Win32DCSafeHandle hdc, float dpiX, FontCache fontCache, FlowContext flowContext, bool keepLines, out float height)
		{
			if (flowContext.Height <= 0f || flowContext.Width <= 0f)
			{
				height = 0f;
				return null;
			}
			int num = TextBox.ConvertToPixels(flowContext.ContentOffset, dpiX);
			int contentOffset = num;
			TextBoxContext context = flowContext.Context;
			RPLFormat.Directions direction = textBox.TextBoxProps.Direction;
			List<Paragraph> list = new List<Paragraph>();
			Win32ObjectSafeHandle win32ObjectSafeHandle = Win32.SelectObject(hdc, Win32ObjectSafeHandle.Zero);
			SizeF flowContextSize;
			if (!textBox.VerticalText)
			{
				flowContextSize = new SizeF(TextBox.ConvertToPixels(flowContext.Width, dpiX), TextBox.ConvertToPixels(flowContext.Height, dpiX));
			}
			else
			{
				flowContext.VerticalCanGrow = textBox.TextBoxProps.CanGrow;
				flowContextSize = new SizeF(TextBox.ConvertToPixels(flowContext.Height, dpiX), TextBox.ConvertToPixels(flowContext.Width, dpiX));
			}
			fontCache.WritingMode = textBox.TextBoxProps.WritingMode;
			try
			{
				while (context.ParagraphIndex < textBox.Paragraphs.Count)
				{
					Paragraph paragraph = textBox.Paragraphs[context.ParagraphIndex];
					paragraph.OffsetY = num;
					bool num2 = FlowParagraph(paragraph, direction, hdc, dpiX, fontCache, flowContext, keepLines, flowContextSize, ref contentOffset);
					num += paragraph.Height;
					if (!keepLines)
					{
						paragraph.TextLines = null;
					}
					if (!num2)
					{
						break;
					}
					list.Add(paragraph);
					if ((float)num >= flowContextSize.Height)
					{
						if (paragraph.AtEndOfParagraph(context))
						{
							context.IncrementParagraph();
						}
						break;
					}
					context.IncrementParagraph();
				}
				if ((float)num < flowContextSize.Height)
				{
					flowContext.AtEndOfTextBox = true;
				}
			}
			finally
			{
				if (!win32ObjectSafeHandle.IsInvalid)
				{
					Win32.SelectObject(hdc, win32ObjectSafeHandle).SetHandleAsInvalid();
					win32ObjectSafeHandle.SetHandleAsInvalid();
				}
			}
			height = TextBox.ConvertToMillimeters(num, dpiX);
			flowContext.ContentOffset = TextBox.ConvertToMillimeters(contentOffset, dpiX);
			return list;
		}

		internal static float FlowVertical(TextBox textBox, Graphics g, FontCache fontCache, FlowContext flowContext, out float width, out float nextWidth)
		{
			width = (nextWidth = 0f);
			List<Paragraph> list = null;
			int num = 0;
			int val = int.MaxValue;
			int num2 = 0;
			float height = 0f;
			int num3 = 0;
			List<TextLine> list2 = null;
			float dpiX = g.DpiX;
			Win32DCSafeHandle win32DCSafeHandle = new Win32DCSafeHandle(g.GetHdc(), ownsHandle: false);
			if (textBox.VerticalText)
			{
				flowContext.VerticalCanGrow = textBox.TextBoxProps.CanGrow;
				num3 = TextBox.ConvertToPixels(flowContext.Height, dpiX);
			}
			else
			{
				num3 = TextBox.ConvertToPixels(flowContext.Width, dpiX);
			}
			try
			{
				list = Flow(textBox, win32DCSafeHandle, dpiX, fontCache, flowContext, keepLines: true, out height);
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						list2 = list[i].TextLines;
						if (list2 != null)
						{
							float leftIndent = 0f;
							float rightIndent = 0f;
							float hangingIndent = 0f;
							float num4 = 0f;
							RPLFormat.Directions direction = textBox.TextBoxProps.Direction;
							list[i].GetParagraphIndents(direction, dpiX, out leftIndent, out rightIndent, out hangingIndent);
							num4 = leftIndent + rightIndent;
							if (list2[0].FirstLine)
							{
								num4 += hangingIndent;
							}
							num2 = Math.Max(num2, (int)num4);
							for (int j = 0; j < list2.Count; j++)
							{
								num = Math.Max(num, list2[j].GetWidth(win32DCSafeHandle, fontCache, useVisualRunsIfAvailable: false) + (int)num4);
								val = Math.Min(val, list2[j].GetWidth(win32DCSafeHandle, fontCache, useVisualRunsIfAvailable: false) + (int)num4);
							}
							list[i].TextLines = null;
						}
					}
				}
				val = Math.Max(val, num2);
				if (num > 0)
				{
					int num5 = 0;
					if (flowContext.VerticalCanGrow && flowContext.ForcedCharTrim)
					{
						num5 = flowContext.CharTrimmedRunWidth;
					}
					else
					{
						num5 = (num - val) / 2;
						if (num5 == 0)
						{
							num5 = num / 2;
						}
						else
						{
							num5 += val;
							if (num5 >= num3)
							{
								num5 /= 2;
							}
						}
					}
					width = TextBox.ConvertToMillimeters(num, dpiX);
					nextWidth = TextBox.ConvertToMillimeters(num5, dpiX);
					return height;
				}
				return height;
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

		private static bool FlowParagraph(Paragraph paragraph, RPLFormat.Directions direction, Win32DCSafeHandle hdc, float dpiX, FontCache fontCache, FlowContext flowContext, bool keepLines, SizeF flowContextSize, ref int contentOffset)
		{
			List<TextLine> list = new List<TextLine>();
			TextBoxContext context = flowContext.Context;
			int num = paragraph.OffsetY;
			paragraph.ProcessedEmptyParagraph = false;
			bool flag = false;
			IParagraphProps paragraphProps = paragraph.ParagraphProps;
			if (!flowContext.Updatable || !paragraph.Updated)
			{
				flag = (context.TextRunIndex == 0 && context.TextRunCharacterIndex == 0);
			}
			if (flag)
			{
				num += TextBox.ConvertToPixels(paragraphProps.SpaceBefore, dpiX);
				if ((float)num >= flowContextSize.Height)
				{
					paragraph.Height = num - paragraph.OffsetY;
					return false;
				}
			}
			int num2 = contentOffset;
			float leftIndent = 0f;
			float rightIndent = 0f;
			float hangingIndent = 0f;
			paragraph.GetParagraphIndents(direction, dpiX, out leftIndent, out rightIndent, out hangingIndent);
			Stack<int> stack = null;
			Stack<int> stack2 = null;
			TextLine textLine = null;
			while (true)
			{
				float num3 = leftIndent;
				float num4 = rightIndent;
				if (flag)
				{
					if (direction == RPLFormat.Directions.LTR)
					{
						num3 += hangingIndent;
					}
					else
					{
						num4 += hangingIndent;
					}
				}
				stack = new Stack<int>();
				stack2 = new Stack<int>();
				textLine = new TextLine();
				if (flag)
				{
					textLine.Prefix = CreateLinePrefix(paragraph, direction);
					textLine.FirstLine = true;
				}
				if (!GetLine(paragraph, textLine, hdc, fontCache, flowContext, num3, flowContextSize.Width - num4, flowContextSize.Height - (float)num, stack, stack2, (int)(num3 + num4)))
				{
					if (list.Count > 0)
					{
						list[list.Count - 1].LastLine = true;
					}
					num += TextBox.ConvertToPixels(paragraphProps.SpaceAfter, dpiX);
					break;
				}
				if (keepLines)
				{
					textLine.ScriptLayout(hdc, fontCache);
				}
				num += textLine.GetHeight(hdc, fontCache);
				list.Add(textLine);
				num2 = num;
				if ((float)num >= flowContextSize.Height)
				{
					break;
				}
				flag = false;
			}
			paragraph.Height = num - paragraph.OffsetY;
			if ((float)num > flowContextSize.Height)
			{
				if (list.Count > 0 && (float)num2 > flowContextSize.Height)
				{
					TextLine textLine2 = list[list.Count - 1];
					int textRunIndex = 0;
					int textRunCharacterIndex = 0;
					while (stack.Count > 0)
					{
						textRunIndex = stack.Pop();
					}
					while (stack2.Count > 0)
					{
						textRunCharacterIndex = stack2.Pop();
					}
					if (flowContext.LineLimit)
					{
						context.TextRunIndex = textRunIndex;
						context.TextRunCharacterIndex = textRunCharacterIndex;
						list.RemoveAt(list.Count - 1);
						flowContext.OmittedLineHeight = TextBox.ConvertToMillimeters(textLine2.GetHeight(hdc, fontCache), dpiX);
						if (textLine2.FirstLine)
						{
							num2 -= TextBox.ConvertToPixels(paragraphProps.SpaceBefore, dpiX);
						}
					}
					else
					{
						TextBoxContext textBoxContext = context.Clone();
						textBoxContext.TextRunIndex = textRunIndex;
						textBoxContext.TextRunCharacterIndex = textRunCharacterIndex;
						flowContext.ClipContext = textBoxContext;
					}
					num2 -= textLine2.GetHeight(hdc, fontCache);
				}
				paragraph.AdvanceToNextRun(context, skipEmptyRuns: false);
				paragraph.AdvanceToNextRun(flowContext.ClipContext, skipEmptyRuns: false);
				if (list.Count > 0)
				{
					if (flowContext.ClipContext == null && paragraph.AtEndOfParagraph(context))
					{
						contentOffset = num;
					}
					else
					{
						contentOffset = num2;
					}
					paragraph.TextLines = list;
					return true;
				}
				paragraph.TextLines = null;
				if (paragraph.AtEndOfParagraph(context))
				{
					contentOffset = num;
				}
				return false;
			}
			paragraph.AdvanceToNextRun(context);
			paragraph.TextLines = list;
			contentOffset = num;
			return true;
		}

		private static bool GetLine(Paragraph paragraph, TextLine line, Win32DCSafeHandle hdc, FontCache fontCache, FlowContext flowContext, float left, float right, float height, Stack<int> lineRunsIndex, Stack<int> lineRunsCharIndex, int lineIndents)
		{
			float num = right - left;
			float num2 = left;
			bool newLine = false;
			int num3 = 0;
			do
			{
				lineRunsIndex.Push(flowContext.Context.TextRunIndex);
				lineRunsCharIndex.Push(flowContext.Context.TextRunCharacterIndex);
				TextRun subRunForLine = paragraph.GetSubRunForLine(flowContext.Context, ref newLine);
				if (subRunForLine == null)
				{
					break;
				}
				line.LogicalRuns.Add(subRunForLine);
				int width = subRunForLine.GetWidth(hdc, fontCache);
				num3 = Math.Max(subRunForLine.GetWidth(hdc, fontCache, isAtLineEnd: true) - width, num3);
				num2 += (float)width;
				if (!(num2 + (float)num3 > right))
				{
					continue;
				}
				bool flag = flowContext.WordTrim;
				if (!flowContext.LineLimit && flag)
				{
					if ((float)line.GetHeight(hdc, fontCache) >= height)
					{
						flag = false;
					}
					line.ResetHeight();
				}
				if (!subRunForLine.IsPlaceholderTextRun)
				{
					FoldLine(paragraph, line, hdc, fontCache, flowContext, flag, lineRunsIndex, lineRunsCharIndex, num - (float)num3, lineIndents);
					break;
				}
				lineRunsIndex.Push(flowContext.Context.TextRunIndex);
				lineRunsCharIndex.Push(flowContext.Context.TextRunCharacterIndex);
				bool flag2 = true;
				TextRun subRunForLine2 = paragraph.GetSubRunForLine(flowContext.Context, ref newLine);
				if (subRunForLine2 != null)
				{
					flag2 = (subRunForLine.TextRunProperties.IndexInParagraph < subRunForLine2.TextRunProperties.IndexInParagraph);
				}
				flowContext.Context.TextRunIndex = lineRunsIndex.Pop();
				flowContext.Context.TextRunCharacterIndex = lineRunsCharIndex.Pop();
				if (flag2)
				{
					FoldLine(paragraph, line, hdc, fontCache, flowContext, flag, lineRunsIndex, lineRunsCharIndex, num - (float)num3, lineIndents);
					break;
				}
			}
			while (!newLine);
			if (line.LogicalRuns.Count == 0)
			{
				return false;
			}
			return true;
		}

		private static void FoldLine(Paragraph paragraph, TextLine line, Win32DCSafeHandle hdc, FontCache fontCache, FlowContext flowContext, bool wordTrim, Stack<int> lineRunsIndex, Stack<int> lineRunsCharIndex, float maxWidth, int lineIndents)
		{
			int count = line.LogicalRuns.Count;
			float num = 0f;
			for (int i = 0; i < count; i++)
			{
				num += (float)line.LogicalRuns[i].GetWidth(hdc, fontCache);
			}
			int num2 = -1;
			bool flag = false;
			for (int num3 = count - 1; num3 >= 0; num3--)
			{
				TextRun textRun = line.LogicalRuns[num3];
				num -= (float)textRun.GetWidth(hdc, fontCache);
				SCRIPT_LOGATTR? nextCharLogAttr = null;
				if (num3 != count - 1)
				{
					TextRun textRun2 = line.LogicalRuns[num3 + 1];
					if (textRun2.CharacterCount > 0)
					{
						nextCharLogAttr = textRun2.ScriptLogAttr[0];
					}
				}
				if (!textRun.IsPlaceholderTextRun)
				{
					flag = true;
					int num4 = 0;
					num4 = ((!wordTrim) ? FindFoldTextPosition_CharacterTrim(textRun, hdc, fontCache, maxWidth - num) : FindFoldTextPosition_TextRunTrim(textRun, hdc, fontCache, maxWidth - num, nextCharLogAttr));
					if (num4 > 0)
					{
						FoldLineAt(line, num3, num4, flowContext.Context, lineRunsIndex, lineRunsCharIndex);
						return;
					}
				}
				else if (num2 > 0 && textRun.TextRunProperties.IndexInParagraph != num2)
				{
					flag = true;
				}
				num2 = textRun.TextRunProperties.IndexInParagraph;
			}
			if (line.LogicalRuns.Count > 1)
			{
				if (!flag)
				{
					return;
				}
				int num5;
				for (num5 = line.LogicalRuns.Count - 1; num5 > 0; num5--)
				{
					flowContext.Context.TextRunIndex = lineRunsIndex.Pop();
					flowContext.Context.TextRunCharacterIndex = lineRunsCharIndex.Pop();
					TextRun textRun3 = line.LogicalRuns[num5];
					if (!textRun3.IsPlaceholderTextRun || textRun3.CharacterIndexInOriginal == 0)
					{
						break;
					}
				}
				line.LogicalRuns.RemoveRange(num5, line.LogicalRuns.Count - num5);
			}
			else
			{
				if (!flag)
				{
					return;
				}
				int num6 = 1;
				if (maxWidth > 0f)
				{
					flowContext.ForcedCharTrim = true;
					if (flowContext.VerticalCanGrow)
					{
						num6 = FindWidthToBreakPosition(line.LogicalRuns[0], hdc, fontCache, maxWidth, out int width);
						flowContext.CharTrimmedRunWidth = Math.Max(flowContext.CharTrimmedRunWidth, width + lineIndents);
					}
					else
					{
						num6 = FindFoldTextPosition_CharacterTrim(line.LogicalRuns[0], hdc, fontCache, maxWidth);
					}
					if (num6 == 0)
					{
						num6 = 1;
					}
				}
				else if (line.FirstLine && paragraph.ParagraphProps.HangingIndent > 0f)
				{
					num6 = 0;
					if (flowContext.Updatable)
					{
						paragraph.Updated = true;
					}
				}
				FoldLineAt(line, 0, num6, flowContext.Context, lineRunsIndex, lineRunsCharIndex);
			}
		}

		private static void FoldLineAt(TextLine line, int runIndex, int runCharIndex, TextBoxContext context, Stack<int> lineRunsIndex, Stack<int> lineRunsCharIndex)
		{
			TextRun textRun = line.LogicalRuns[runIndex];
			for (int num = line.LogicalRuns.Count - 1; num > runIndex; num--)
			{
				context.TextRunIndex = lineRunsIndex.Pop();
				context.TextRunCharacterIndex = lineRunsCharIndex.Pop();
				line.LogicalRuns.RemoveAt(num);
			}
			string text = textRun.Text;
			if (runCharIndex < text.Length)
			{
				context.TextRunCharacterIndex -= text.Length - runCharIndex;
				if (textRun.Clone)
				{
					textRun.TerminateAt(runCharIndex);
					return;
				}
				SCRIPT_LOGATTR[] array = new SCRIPT_LOGATTR[runCharIndex];
				Array.Copy(textRun.ScriptLogAttr, 0, array, 0, array.Length);
				string text2 = text.Substring(0, runCharIndex);
				TextRun textRun2 = textRun.Split(text2, array);
				textRun2.Clone = true;
				line.LogicalRuns[runIndex] = textRun2;
			}
		}

		private static int FindWidthToBreakPosition(TextRun run, Win32DCSafeHandle hdc, FontCache fontCache, float maxWidth, out int width)
		{
			string text = run.Text;
			int[] logicalWidths = run.GetLogicalWidths(hdc, fontCache);
			int i = 0;
			for (width = 0; i < text.Length && ((float)(width + logicalWidths[i]) <= maxWidth || (!run.ScriptLogAttr[i].IsWhiteSpace && !run.ScriptLogAttr[i].IsSoftBreak)); i++)
			{
				width += logicalWidths[i];
			}
			return i;
		}

		private static int FindFoldTextPosition_CharacterTrim(TextRun run, Win32DCSafeHandle hdc, FontCache fontCache, float maxWidth)
		{
			string text = run.Text;
			int[] logicalWidths = run.GetLogicalWidths(hdc, fontCache);
			int i = 0;
			for (int num = 0; i < text.Length && (float)(num + logicalWidths[i]) <= maxWidth; i++)
			{
				num += logicalWidths[i];
			}
			return i;
		}

		private static int FindFoldTextPosition_TextRunTrim(TextRun run, Win32DCSafeHandle hdc, FontCache fontCache, float maxWidth, SCRIPT_LOGATTR? nextCharLogAttr)
		{
			string text = run.Text;
			int i = FindFoldTextPosition_CharacterTrim(run, hdc, fontCache, maxWidth);
			if (i == text.Length && i > 0 && !run.ScriptLogAttr[i - 1].IsWhiteSpace && nextCharLogAttr.HasValue && !nextCharLogAttr.Value.IsWhiteSpace && !nextCharLogAttr.Value.IsSoftBreak)
			{
				i--;
			}
			int num = i;
			if (i > 0)
			{
				if (i < text.Length)
				{
					while (i > 0 && !run.ScriptLogAttr[i].IsWhiteSpace && !run.ScriptLogAttr[i].IsSoftBreak)
					{
						i--;
					}
					if (i <= 0)
					{
						return -1;
					}
					for (; i < num + 1 && run.ScriptLogAttr[i].IsWhiteSpace; i++)
					{
					}
				}
				return i;
			}
			return -1;
		}

		private static List<TextRun> CreateLinePrefix(Paragraph paragraph, RPLFormat.Directions direction)
		{
			IParagraphProps paragraphProps = paragraph.ParagraphProps;
			if (paragraphProps.ListStyle == RPLFormat.ListStyles.None || paragraphProps.ListLevel <= 0)
			{
				return null;
			}
			Paragraph paragraph2 = new Paragraph();
			StringBuilder stringBuilder = new StringBuilder();
			PrefixRun prefixRun = null;
			if (paragraphProps.ListStyle == RPLFormat.ListStyles.Bulleted)
			{
				prefixRun = new BulletPrefixRun();
				stringBuilder.Append(BulletChars[(paragraphProps.ListLevel - 1) % 3]);
			}
			else
			{
				prefixRun = new NumberPrefixRun();
				switch ((paragraphProps.ListLevel - 1) % 3)
				{
				case 0:
					stringBuilder.Append(GetAsDecimalString(paragraph.ParagraphNumber));
					break;
				case 1:
					stringBuilder.Append(GetAsRomanNumeralString(paragraph.ParagraphNumber));
					break;
				case 2:
					stringBuilder.Append(GetAsLatinAlphaString(paragraph.ParagraphNumber));
					break;
				}
				stringBuilder.Append('.');
			}
			paragraph2.Runs.Add(new TextRun(stringBuilder.ToString(), prefixRun));
			paragraph2.ScriptItemize(direction);
			TextLine textLine = new TextLine();
			for (int i = 0; i < paragraph2.Runs.Count; i++)
			{
				textLine.LogicalRuns.Add(paragraph2.Runs[i]);
			}
			textLine.ScriptLayout(Win32DCSafeHandle.Zero, null);
			return textLine.LogicalRuns;
		}

		internal static string GetAsDecimalString(int value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (value > 0)
			{
				stringBuilder.Append((char)(48 + value % 10));
				value /= 10;
			}
			int num = stringBuilder.Length / 2;
			for (int i = 0; i < num; i++)
			{
				char value2 = stringBuilder[i];
				stringBuilder[i] = stringBuilder[stringBuilder.Length - i - 1];
				stringBuilder[stringBuilder.Length - i - 1] = value2;
			}
			return stringBuilder.ToString();
		}

		internal static string GetAsLatinAlphaString(int value)
		{
			value--;
			return new string((char)(97 + value % 26), value / 26 + 1);
		}

		internal static string GetAsRomanNumeralString(int value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (value >= 1000)
			{
				stringBuilder.Append('m', value / 1000);
				value %= 1000;
			}
			for (int num = 2; num >= 0; num--)
			{
				int num2 = (int)Math.Pow(10.0, num);
				int num3 = 9 * num2;
				if (value >= num3)
				{
					stringBuilder.Append(RomanNumerals[num, 0]);
					stringBuilder.Append(RomanNumerals[num, 2]);
					value -= num3;
				}
				num3 = 5 * num2;
				if (value >= num3)
				{
					stringBuilder.Append(RomanNumerals[num, 1]);
					value -= num3;
				}
				num3 = 4 * num2;
				if (value >= num3)
				{
					stringBuilder.Append(RomanNumerals[num, 0]);
					stringBuilder.Append(RomanNumerals[num, 1]);
					value -= num3;
				}
				if (value >= num2)
				{
					stringBuilder.Append(RomanNumerals[num, 0], value / num2);
					value %= num2;
				}
			}
			return stringBuilder.ToString();
		}
	}
}

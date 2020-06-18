using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal sealed class Paragraph
	{
		private List<TextRun> m_runs = new List<TextRun>();

		private IParagraphProps m_paragraphProps;

		private List<TextLine> m_lines;

		private bool m_updated;

		private bool m_processedEmptyParagraph;

		private int m_offsetY = -1;

		private int m_height = -1;

		private int m_lastRunIndex;

		private int m_lastCharIndexInRun;

		private const int m_maxRunLength = 21845;

		internal IParagraphProps ParagraphProps => m_paragraphProps;

		internal int ParagraphNumber => m_paragraphProps.ParagraphNumber;

		internal List<TextRun> Runs
		{
			get
			{
				return m_runs;
			}
			set
			{
				m_runs = value;
			}
		}

		internal List<TextLine> TextLines
		{
			get
			{
				return m_lines;
			}
			set
			{
				m_lines = value;
			}
		}

		internal bool Updated
		{
			get
			{
				return m_updated;
			}
			set
			{
				m_updated = value;
			}
		}

		internal bool ProcessedEmptyParagraph
		{
			get
			{
				return m_processedEmptyParagraph;
			}
			set
			{
				m_processedEmptyParagraph = value;
			}
		}

		internal int OffsetY
		{
			get
			{
				return m_offsetY;
			}
			set
			{
				m_offsetY = value;
			}
		}

		internal int Height
		{
			get
			{
				return m_height;
			}
			set
			{
				m_height = value;
			}
		}

		internal Paragraph()
		{
			m_runs = new List<TextRun>();
		}

		internal Paragraph(IParagraphProps paraProps)
		{
			m_paragraphProps = paraProps;
			m_runs = new List<TextRun>();
		}

		internal Paragraph(IParagraphProps paraProps, int textRunCount)
		{
			m_paragraphProps = paraProps;
			m_runs = new List<TextRun>(textRunCount);
		}

		private List<TextRun> ExtractRuns(int startingParaCharIndex, int endingParaCharIndex, SCRIPT_ANALYSIS analysis)
		{
			List<TextRun> list = new List<TextRun>();
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			int num2 = 0;
			TextRun textRun = null;
			for (int i = 0; i < m_runs.Count; i++)
			{
				TextRun textRun2 = m_runs[i];
				num2 = num + textRun2.CharacterCount - 1;
				int num3 = -1;
				if (startingParaCharIndex <= num)
				{
					num3 = 0;
				}
				else if (startingParaCharIndex <= num2)
				{
					num3 = startingParaCharIndex - num;
				}
				if (num3 >= 0)
				{
					if (endingParaCharIndex <= num2)
					{
						int num4 = endingParaCharIndex - num - num3 + 1;
						textRun = textRun2.GetSubRun(num3, num4);
						textRun.SCRIPT_ANALYSIS = analysis;
						stringBuilder.Append(textRun.Text);
						list.Add(textRun);
						if (endingParaCharIndex == num2 && num4 == 1 && i == m_runs.Count - 1 && textRun.Text[0] == '\n')
						{
							textRun = textRun2.GetSubRun(num3 + 1, 0);
							textRun.SCRIPT_ANALYSIS = analysis;
							list.Add(textRun);
						}
						AnalyzeForBreakPositions(list, stringBuilder.ToString());
						return list;
					}
					textRun = textRun2.GetSubRun(num3);
					textRun.SCRIPT_ANALYSIS = analysis;
					stringBuilder.Append(textRun.Text);
					list.Add(textRun);
				}
				num = num2 + 1;
			}
			AnalyzeForBreakPositions(list, stringBuilder.ToString());
			return list;
		}

		private static void AnalyzeForBreakPositions(List<TextRun> itemRuns, string itemsText)
		{
			if (itemsText.Length != 0)
			{
				SCRIPT_ANALYSIS psa = itemRuns[0].SCRIPT_ANALYSIS;
				SCRIPT_LOGATTR[] array = new SCRIPT_LOGATTR[itemsText.Length];
				int num = Win32.ScriptBreak(itemsText, itemsText.Length, ref psa, array);
				if (Win32.Failed(num))
				{
					Marshal.ThrowExceptionForHR(num);
				}
				int num2 = 0;
				for (int i = 0; i < itemRuns.Count; i++)
				{
					TextRun textRun = itemRuns[i];
					int length = textRun.Text.Length;
					SCRIPT_LOGATTR[] array2 = new SCRIPT_LOGATTR[length];
					Array.Copy(array, num2, array2, 0, length);
					textRun.ScriptLogAttr = array2;
					num2 += length;
				}
			}
		}

		internal void ScriptItemize(RPLFormat.Directions direction)
		{
			SCRIPT_ITEM[] array = null;
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			string nextTextBlock;
			while ((nextTextBlock = GetNextTextBlock()) != null)
			{
				if (nextTextBlock.Length <= 0)
				{
					continue;
				}
				SCRIPT_CONTROL psControl = default(SCRIPT_CONTROL);
				psControl.dword1 = 16777216u;
				SCRIPT_STATE psState = new ScriptState
				{
					uBidiLevel = ((direction != 0) ? 1 : 0)
				}.GetAs_SCRIPT_STATE();
				SCRIPT_ITEM[] array2 = new SCRIPT_ITEM[nextTextBlock.Length + 1];
				int pcItems = 0;
				int num2 = Win32.ScriptItemize(nextTextBlock, nextTextBlock.Length, array2.Length, ref psControl, ref psState, array2, ref pcItems);
				if (Win32.Failed(num2))
				{
					Marshal.ThrowExceptionForHR(num2);
				}
				if (array == null)
				{
					array = array2;
					num = pcItems;
				}
				else
				{
					SCRIPT_ITEM[] array3 = new SCRIPT_ITEM[num + array2.Length];
					Array.Copy(array, 0, array3, 0, num);
					for (int i = 0; i < array2.Length; i++)
					{
						SCRIPT_ITEM sCRIPT_ITEM = array2[i];
						sCRIPT_ITEM.iCharPos += stringBuilder.Length;
						array3[num + i] = sCRIPT_ITEM;
					}
					array = array3;
					num += pcItems;
				}
				stringBuilder.Append(nextTextBlock);
			}
			if (num <= 0)
			{
				return;
			}
			List<SCRIPT_ITEM> list = new List<SCRIPT_ITEM>();
			int num3 = -1;
			for (int j = 0; j < num; j++)
			{
				int bidiLevel = ScriptState.GetBidiLevel(array[j].analysis.state.word1);
				int bidiLevel2 = ScriptState.GetBidiLevel(array[j + 1].analysis.state.word1);
				if (bidiLevel == bidiLevel2 && CanMergeItemizedRuns(array[j].iCharPos, array[j + 1].iCharPos - 1, stringBuilder))
				{
					if (num3 < 0)
					{
						num3 = j;
					}
					continue;
				}
				if (num3 >= 0)
				{
					list.Add(array[num3]);
				}
				list.Add(array[j]);
				num3 = -1;
			}
			if (num3 >= 0)
			{
				list.Add(array[num3]);
			}
			list.Add(array[num]);
			array = null;
			num = list.Count - 1;
			if (num == 1)
			{
				for (int k = 0; k < m_runs.Count; k++)
				{
					m_runs[k].SCRIPT_ANALYSIS = list[0].analysis;
				}
				AnalyzeForBreakPositions(m_runs, stringBuilder.ToString());
				return;
			}
			List<TextRun> list2 = new List<TextRun>();
			for (int l = 0; l < num; l++)
			{
				int iCharPos = list[l].iCharPos;
				int endingParaCharIndex = list[l + 1].iCharPos - 1;
				List<TextRun> collection = ExtractRuns(iCharPos, endingParaCharIndex, list[l].analysis);
				list2.AddRange(collection);
			}
			m_runs = list2;
		}

		private bool CanMergeItemizedRuns(int start, int end, StringBuilder paragraphText)
		{
			bool flag = true;
			for (int i = start; i <= end; i++)
			{
				char c = paragraphText[i];
				if (c > '\u007f' || char.IsLetter(c))
				{
					flag = false;
					break;
				}
				switch (c)
				{
				case '\n':
				case '\v':
				case '\f':
				case '\r':
				case '[':
				case ']':
				case '\u0085':
					flag = false;
					break;
				}
				if (!flag)
				{
					break;
				}
			}
			return flag;
		}

		internal bool AtEndOfParagraph(TextBoxContext context)
		{
			if (context == null)
			{
				return false;
			}
			if (context.TextRunIndex < m_runs.Count)
			{
				return false;
			}
			return true;
		}

		internal void AdvanceToNextRun(TextBoxContext context)
		{
			AdvanceToNextRun(context, skipEmptyRuns: true);
		}

		internal void AdvanceToNextRun(TextBoxContext context, bool skipEmptyRuns)
		{
			if (context == null || context.TextRunIndex >= m_runs.Count)
			{
				return;
			}
			TextRun textRun = m_runs[context.TextRunIndex];
			int num = (textRun.Text != null) ? textRun.Text.Length : 0;
			if (m_runs.Count == 1 && num == 0)
			{
				if (m_processedEmptyParagraph)
				{
					context.TextRunIndex++;
					context.TextRunCharacterIndex = 0;
				}
				else
				{
					m_processedEmptyParagraph = true;
				}
			}
			else if (context.TextRunCharacterIndex == num && (skipEmptyRuns || num > 0))
			{
				context.TextRunIndex++;
				context.TextRunCharacterIndex = 0;
			}
		}

		internal TextRun GetSubRunForLine(TextBoxContext context, ref bool newLine)
		{
			AdvanceToNextRun(context);
			if (context.TextRunIndex < m_runs.Count)
			{
				TextRun textRun = m_runs[context.TextRunIndex];
				string text = null;
				bool flag = false;
				if (context.TextRunCharacterIndex == 0)
				{
					text = (textRun.Text ?? "");
				}
				else
				{
					flag = true;
					text = textRun.Text.Substring(context.TextRunCharacterIndex);
				}
				int num = text.IndexOf('\n');
				if (num != -1)
				{
					if (num + 1 < text.Length)
					{
						text = text.Substring(0, num + 1);
						flag = true;
					}
					newLine = true;
				}
				if (flag)
				{
					SCRIPT_LOGATTR[] array = new SCRIPT_LOGATTR[text.Length];
					Array.Copy(textRun.ScriptLogAttr, context.TextRunCharacterIndex, array, 0, array.Length);
					TextRun textRun2 = textRun.Split(text, array);
					textRun2.CharacterIndexInOriginal += context.TextRunCharacterIndex;
					textRun2.Clone = true;
					context.TextRunCharacterIndex += text.Length;
					return textRun2;
				}
				context.TextRunCharacterIndex += text.Length;
				return textRun;
			}
			return null;
		}

		internal string GetNextTextBlock()
		{
			if (m_lastRunIndex >= m_runs.Count)
			{
				m_lastCharIndexInRun = 0;
				m_lastRunIndex = 0;
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			bool flag2 = false;
			while (m_lastRunIndex < m_runs.Count)
			{
				TextRun textRun = m_runs[m_lastRunIndex];
				if (!flag && (textRun.IsPlaceholderTextRun || flag2))
				{
					break;
				}
				int length = textRun.Text.Length;
				if (stringBuilder.Length + length - m_lastCharIndexInRun > 21845)
				{
					int num = 21845 - stringBuilder.Length + m_lastCharIndexInRun - 1;
					int num2 = Math.Max(num - 100, m_lastCharIndexInRun);
					for (int num3 = num; num3 >= num2; num3--)
					{
						char c = textRun.Text[num3];
						if (!char.IsHighSurrogate(c) && !char.IsLetterOrDigit(c))
						{
							num = ((c != '\r' || num3 - 1 <= m_lastCharIndexInRun) ? num3 : (num3 - 1));
							break;
						}
					}
					stringBuilder.Append(textRun.Text, m_lastCharIndexInRun, num - m_lastCharIndexInRun + 1);
					m_lastCharIndexInRun = num + 1;
					flag2 = textRun.IsPlaceholderTextRun;
					return stringBuilder.ToString();
				}
				stringBuilder.Append(textRun.Text, m_lastCharIndexInRun, length - m_lastCharIndexInRun);
				m_lastCharIndexInRun = 0;
				flag = false;
				flag2 = textRun.IsPlaceholderTextRun;
				m_lastRunIndex++;
			}
			return stringBuilder.ToString();
		}

		internal void GetParagraphIndents(RPLFormat.Directions direction, float dpiX, out float leftIndent, out float rightIndent, out float hangingIndent)
		{
			leftIndent = TextBox.ConvertToPixels(m_paragraphProps.LeftIndent, dpiX);
			rightIndent = TextBox.ConvertToPixels(m_paragraphProps.RightIndent, dpiX);
			hangingIndent = TextBox.ConvertToPixels(m_paragraphProps.HangingIndent, dpiX);
			if (hangingIndent < 0f)
			{
				if (direction == RPLFormat.Directions.LTR)
				{
					leftIndent -= hangingIndent;
				}
				else
				{
					rightIndent -= hangingIndent;
				}
			}
			if (m_paragraphProps.ListLevel > 0)
			{
				float num = m_paragraphProps.ListLevel * TextBox.ConvertToPixels(10.583333f, dpiX);
				if (direction == RPLFormat.Directions.LTR)
				{
					leftIndent += num;
				}
				else
				{
					rightIndent += num;
				}
			}
		}
	}
}

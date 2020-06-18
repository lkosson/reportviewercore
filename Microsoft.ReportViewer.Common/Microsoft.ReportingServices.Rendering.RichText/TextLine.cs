using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal sealed class TextLine
	{
		private List<TextRun> m_prefix;

		private List<TextRun> m_visualRuns;

		private List<TextRun> m_logicalRuns = new List<TextRun>();

		private int m_prefixWidth;

		private int m_width;

		private int m_ascent;

		private int m_descent;

		private bool m_firstLine;

		private bool m_lastLine;

		private bool m_calculatedDimensions;

		private bool m_calculatedHeight;

		internal string Text
		{
			get
			{
				if (m_logicalRuns.Count == 0)
				{
					return null;
				}
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < m_visualRuns.Count; i++)
				{
					stringBuilder.Append(m_visualRuns[i].Text);
				}
				return stringBuilder.ToString();
			}
		}

		internal List<TextRun> VisualRuns => m_visualRuns;

		internal List<TextRun> LogicalRuns => m_logicalRuns;

		internal List<TextRun> Prefix
		{
			get
			{
				return m_prefix;
			}
			set
			{
				m_prefix = value;
			}
		}

		internal bool LastLine
		{
			get
			{
				return m_lastLine;
			}
			set
			{
				m_lastLine = value;
			}
		}

		internal bool FirstLine
		{
			get
			{
				return m_firstLine;
			}
			set
			{
				m_firstLine = value;
			}
		}

		internal TextLine()
		{
		}

		internal void ResetHeight()
		{
			m_calculatedHeight = false;
			m_ascent = 0;
			m_descent = 0;
		}

		internal int GetHeight(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			if (!m_calculatedHeight)
			{
				m_calculatedHeight = true;
				for (int i = 0; i < m_logicalRuns.Count; i++)
				{
					TextRun textRun = m_logicalRuns[i];
					int ascent = textRun.GetAscent(hdc, fontCache);
					if (ascent > m_ascent)
					{
						m_ascent = ascent;
					}
					int descent = textRun.GetDescent(hdc, fontCache);
					if (descent > m_descent)
					{
						m_descent = descent;
					}
				}
				if (m_prefix != null)
				{
					for (int j = 0; j < m_prefix.Count; j++)
					{
						TextRun textRun2 = m_prefix[j];
						int ascent2 = textRun2.GetAscent(hdc, fontCache);
						if (ascent2 > m_ascent)
						{
							m_ascent = ascent2;
						}
						int descent2 = textRun2.GetDescent(hdc, fontCache);
						if (descent2 > m_descent)
						{
							m_descent = descent2;
						}
					}
				}
			}
			return m_ascent + m_descent;
		}

		internal int GetAscent(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			CalculateDimensions(hdc, fontCache, useVisualRunsIfAvailable: true);
			return m_ascent;
		}

		internal int GetDescent(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			CalculateDimensions(hdc, fontCache, useVisualRunsIfAvailable: true);
			return m_descent;
		}

		internal int GetWidth(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			return GetWidth(hdc, fontCache, useVisualRunsIfAvailable: true);
		}

		internal int GetWidth(Win32DCSafeHandle hdc, FontCache fontCache, bool useVisualRunsIfAvailable)
		{
			CalculateDimensions(hdc, fontCache, useVisualRunsIfAvailable);
			return m_width;
		}

		internal int GetPrefixWidth(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			if (m_prefix == null)
			{
				return 0;
			}
			CalculateDimensions(hdc, fontCache, useVisualRunsIfAvailable: true);
			return m_prefixWidth;
		}

		private void CalculateDimensions(Win32DCSafeHandle hdc, FontCache fontCache, bool useVisualRunsIfAvailable)
		{
			if (m_calculatedDimensions)
			{
				return;
			}
			bool flag = useVisualRunsIfAvailable && m_visualRuns != null;
			List<TextRun> list = flag ? m_visualRuns : m_logicalRuns;
			m_width = 0;
			m_prefixWidth = 0;
			int count = list.Count;
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				TextRun textRun = list[i];
				int width = textRun.GetWidth(hdc, fontCache);
				m_width += width;
				if (!flag)
				{
					num = Math.Max(textRun.GetWidth(hdc, fontCache, isAtLineEnd: true) - width, num);
				}
				else if (i == count - 1)
				{
					num = textRun.GetWidth(hdc, fontCache, isAtLineEnd: true) - width;
				}
				if (!m_calculatedHeight)
				{
					int ascent = textRun.GetAscent(hdc, fontCache);
					if (ascent > m_ascent)
					{
						m_ascent = ascent;
					}
					int descent = textRun.GetDescent(hdc, fontCache);
					if (descent > m_descent)
					{
						m_descent = descent;
					}
				}
			}
			m_width += num;
			if (m_prefix != null)
			{
				for (int j = 0; j < m_prefix.Count; j++)
				{
					TextRun textRun2 = m_prefix[j];
					if (!m_calculatedHeight)
					{
						int ascent2 = textRun2.GetAscent(hdc, fontCache);
						if (ascent2 > m_ascent)
						{
							m_ascent = ascent2;
						}
						int descent2 = textRun2.GetDescent(hdc, fontCache);
						if (descent2 > m_descent)
						{
							m_descent = descent2;
						}
					}
					m_prefixWidth += textRun2.GetWidth(hdc, fontCache);
				}
			}
			m_calculatedDimensions = true;
			m_calculatedHeight = true;
		}

		internal void ScriptLayout(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			if (m_visualRuns != null)
			{
				return;
			}
			int count = m_logicalRuns.Count;
			if (count == 0)
			{
				return;
			}
			byte[] array = new byte[count];
			int[] array2 = new int[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = (byte)m_logicalRuns[i].ScriptAnalysis.s.uBidiLevel;
			}
			int num = Win32.ScriptLayout(count, array, null, array2);
			if (Win32.Failed(num))
			{
				Marshal.ThrowExceptionForHR(num);
			}
			m_visualRuns = new List<TextRun>(count);
			for (int j = 0; j < count; j++)
			{
				m_visualRuns.Add(null);
			}
			for (int k = 0; k < count; k++)
			{
				m_visualRuns[array2[k]] = m_logicalRuns[k];
			}
			int num2 = 0;
			int l = -1;
			for (int m = 0; m < count; m++)
			{
				TextRun textRun = m_visualRuns[m];
				textRun.UnderlineHeight = 0;
				if (textRun.TextRunProperties.TextDecoration == RPLFormat.TextDecorations.Underline)
				{
					if (l < 0)
					{
						l = m;
					}
					int height = textRun.GetHeight(hdc, fontCache);
					if (height > num2)
					{
						num2 = height;
					}
				}
				else if (l >= 0)
				{
					for (; l < m; l++)
					{
						m_visualRuns[l].UnderlineHeight = num2;
					}
					num2 = 0;
					l = -1;
				}
			}
			if (l >= 0)
			{
				for (; l < count; l++)
				{
					m_visualRuns[l].UnderlineHeight = num2;
				}
			}
		}
	}
}

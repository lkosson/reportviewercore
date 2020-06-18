using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8.Records;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class RichTextInfo : IRichTextInfo
	{
		private List<Pair<int, int>> m_fontList;

		private BIFF8Font m_font;

		private StyleContainer m_styleContainer;

		private StringBuilder m_stringBuilder;

		private int m_startIndex = -1;

		private int m_maxFontIndex;

		private bool m_firstRun;

		private bool m_checkForRotatedFarEastChars;

		private bool m_foundRotatedFarEastChar;

		internal int MaxFontIndex => m_maxFontIndex;

		internal bool FoundRotatedFarEastChar => m_foundRotatedFarEastChar;

		public bool CheckForRotatedFarEastChars
		{
			set
			{
				m_checkForRotatedFarEastChars = value;
			}
		}

		internal RichTextInfo(StyleContainer styleContainer)
		{
			m_styleContainer = styleContainer;
			m_startIndex = 0;
			m_font = null;
			m_fontList = new List<Pair<int, int>>();
			m_stringBuilder = new StringBuilder();
			m_maxFontIndex = 0;
			m_firstRun = true;
		}

		public IFont AppendTextRun(string value)
		{
			return AppendTextRun(value, replaceInvalidWhiteSpace: true);
		}

		public IFont AppendTextRun(string value, bool replaceInvalidWhiteSpace)
		{
			if (m_font != null)
			{
				int num = m_styleContainer.AddFont(m_font);
				m_maxFontIndex = Math.Max(m_maxFontIndex, num);
				m_fontList.Add(new Pair<int, int>(m_startIndex, num));
			}
			m_startIndex = m_stringBuilder.Length;
			if (replaceInvalidWhiteSpace)
			{
				AppendWithChecking(value);
			}
			else
			{
				m_stringBuilder.Append(value);
			}
			if (m_firstRun)
			{
				m_firstRun = false;
				return m_styleContainer;
			}
			m_font = new BIFF8Font();
			return m_font;
		}

		public void AppendText(string value)
		{
			AppendText(value, replaceInvalidWhiteSpace: true);
		}

		public void AppendText(string value, bool replaceInvalidWhiteSpace)
		{
			if (m_startIndex >= 0)
			{
				if (replaceInvalidWhiteSpace)
				{
					AppendWithChecking(value);
				}
				else
				{
					m_stringBuilder.Append(value);
				}
			}
		}

		public void AppendText(char value)
		{
			if (m_startIndex >= 0)
			{
				m_stringBuilder.Append(value);
			}
		}

		private void AppendWithChecking(string value)
		{
			ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(value, m_stringBuilder, m_checkForRotatedFarEastChars, out bool foundEastAsianChar);
			if (m_checkForRotatedFarEastChars && foundEastAsianChar)
			{
				m_checkForRotatedFarEastChars = false;
				m_foundRotatedFarEastChar = true;
			}
		}

		internal StringWrapperBIFF8 CompleteRun()
		{
			if (m_font != null)
			{
				int num = m_styleContainer.AddFont(m_font);
				m_maxFontIndex = Math.Max(m_maxFontIndex, num);
				m_fontList.Add(new Pair<int, int>(m_startIndex, num));
			}
			StringWrapperBIFF8 stringWrapperBIFF = new StringWrapperBIFF8(m_stringBuilder.ToString());
			stringWrapperBIFF.SetRunsList(m_fontList);
			m_fontList = null;
			m_font = null;
			m_stringBuilder = null;
			return stringWrapperBIFF;
		}
	}
}

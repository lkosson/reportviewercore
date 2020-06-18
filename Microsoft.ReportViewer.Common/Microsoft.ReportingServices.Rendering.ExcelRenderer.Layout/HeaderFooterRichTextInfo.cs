using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Layout
{
	internal sealed class HeaderFooterRichTextInfo : IRichTextInfo
	{
		private HeaderFooterRichTextFont m_font;

		private StringBuilder m_stringBuilder;

		private StringBuilder m_valueBuilder;

		internal string LastFontName => m_font.LastFontName;

		internal double LastFontSize => m_font.LastFontSize;

		public bool CheckForRotatedFarEastChars
		{
			set
			{
			}
		}

		internal HeaderFooterRichTextInfo(StringBuilder builder)
		{
			m_stringBuilder = builder;
			m_font = new HeaderFooterRichTextFont(m_stringBuilder);
			m_valueBuilder = new StringBuilder();
		}

		public IFont AppendTextRun(string value)
		{
			return AppendTextRun(value, replaceInvalidWhiteSpace: true);
		}

		public IFont AppendTextRun(string value, bool replaceInvalidWhiteSpace)
		{
			if (m_valueBuilder.Length > 0)
			{
				m_stringBuilder.Append(m_valueBuilder.ToString());
				m_valueBuilder.Remove(0, m_valueBuilder.Length);
			}
			if (replaceInvalidWhiteSpace)
			{
				FormulaHandler.EncodeHeaderFooterString(m_valueBuilder, value);
			}
			else
			{
				m_valueBuilder.Append(value);
			}
			return m_font;
		}

		public void AppendText(string value)
		{
			AppendText(value, replaceInvalidWhiteSpace: true);
		}

		public void AppendText(string value, bool replaceInvalidWhiteSpace)
		{
			if (replaceInvalidWhiteSpace)
			{
				FormulaHandler.EncodeHeaderFooterString(m_valueBuilder, value);
			}
			else
			{
				m_valueBuilder.Append(value);
			}
		}

		public void AppendText(char value)
		{
			m_valueBuilder.Append(value);
		}

		internal void CompleteRun()
		{
			if (m_valueBuilder.Length > 0)
			{
				m_stringBuilder.Append(m_valueBuilder.ToString());
				m_valueBuilder.Remove(0, m_valueBuilder.Length);
			}
			m_stringBuilder.Append(' ');
		}

		internal void CompleteCurrentFormatting()
		{
			m_font.Bold = 0;
			m_font.Italic = false;
			m_font.ScriptStyle = ScriptStyle.None;
			m_font.Strikethrough = false;
			m_font.Underline = Underline.None;
		}
	}
}

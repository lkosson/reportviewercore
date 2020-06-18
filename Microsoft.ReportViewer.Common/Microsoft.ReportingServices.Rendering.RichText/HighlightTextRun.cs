using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal class HighlightTextRun : TextRun
	{
		private int m_highlightStart = -1;

		private int m_highlightEnd = -1;

		private Color m_highlightColor = Color.Empty;

		private int m_charIndexInOriginal;

		internal override int HighlightStart
		{
			get
			{
				return m_highlightStart;
			}
			set
			{
				m_highlightStart = value;
			}
		}

		internal override int HighlightEnd
		{
			get
			{
				return m_highlightEnd;
			}
			set
			{
				m_highlightEnd = value;
			}
		}

		internal override Color HighlightColor
		{
			get
			{
				return m_highlightColor;
			}
			set
			{
				m_highlightColor = value;
			}
		}

		internal override bool IsHighlightTextRun => true;

		internal override int CharacterIndexInOriginal
		{
			get
			{
				return m_charIndexInOriginal;
			}
			set
			{
				m_charIndexInOriginal = value;
			}
		}

		internal HighlightTextRun(string text, ITextRunProps props)
			: base(text, props)
		{
		}

		internal HighlightTextRun(string text, TextRun textRun)
			: base(text, textRun.TextRunProperties)
		{
			m_charIndexInOriginal = textRun.CharacterIndexInOriginal;
		}

		internal HighlightTextRun(string text, HighlightTextRun textRun)
			: base(text, textRun)
		{
			m_highlightStart = textRun.m_highlightStart;
			m_highlightEnd = textRun.m_highlightEnd;
			m_highlightColor = textRun.m_highlightColor;
			m_charIndexInOriginal = textRun.CharacterIndexInOriginal;
		}

		internal HighlightTextRun(string text, HighlightTextRun textRun, SCRIPT_LOGATTR[] scriptLogAttr)
			: base(text, textRun, scriptLogAttr)
		{
			m_highlightStart = textRun.m_highlightStart;
			m_highlightEnd = textRun.m_highlightEnd;
			m_highlightColor = textRun.m_highlightColor;
			m_charIndexInOriginal = textRun.CharacterIndexInOriginal;
		}

		internal override TextRun Split(string text, SCRIPT_LOGATTR[] scriptLogAttr)
		{
			return new HighlightTextRun(text, this, scriptLogAttr);
		}

		internal override TextRun GetSubRun(int startIndex, int length)
		{
			if (length == m_text.Length)
			{
				return this;
			}
			if (startIndex > 0)
			{
				m_textRunProps.AddSplitIndex(startIndex);
			}
			return new HighlightTextRun(m_text.Substring(startIndex, length), this)
			{
				CharacterIndexInOriginal = startIndex
			};
		}
	}
}

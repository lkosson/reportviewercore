using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal sealed class PlaceholderTextRun : HighlightTextRun
	{
		private Color m_placeholderBorderColor = Color.Empty;

		private bool m_allowColorInversion = true;

		internal override bool IsPlaceholderTextRun => true;

		internal Color PlaceholderBorderColor
		{
			get
			{
				return m_placeholderBorderColor;
			}
			set
			{
				m_placeholderBorderColor = value;
			}
		}

		internal override bool AllowColorInversion
		{
			get
			{
				return m_allowColorInversion;
			}
			set
			{
				m_allowColorInversion = value;
			}
		}

		internal PlaceholderTextRun(string text, ITextRunProps props)
			: base(text, props)
		{
		}

		internal PlaceholderTextRun(string text, TextRun textRun)
			: base(text, textRun.TextRunProperties)
		{
		}

		internal PlaceholderTextRun(string text, PlaceholderTextRun textRun)
			: base(text, textRun)
		{
			m_placeholderBorderColor = textRun.PlaceholderBorderColor;
		}

		internal PlaceholderTextRun(string text, PlaceholderTextRun textRun, SCRIPT_LOGATTR[] scriptLogAttr)
			: base(text, textRun, scriptLogAttr)
		{
			m_placeholderBorderColor = textRun.PlaceholderBorderColor;
		}

		internal override TextRun Split(string text, SCRIPT_LOGATTR[] scriptLogAttr)
		{
			return new PlaceholderTextRun(text, this, scriptLogAttr);
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
			return new PlaceholderTextRun(m_text.Substring(startIndex, length), this)
			{
				CharacterIndexInOriginal = startIndex
			};
		}
	}
}

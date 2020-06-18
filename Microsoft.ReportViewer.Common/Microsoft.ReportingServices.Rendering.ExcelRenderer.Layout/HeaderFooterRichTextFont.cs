using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Layout
{
	internal sealed class HeaderFooterRichTextFont : IFont
	{
		private StringBuilder m_builder;

		private bool m_boldSet;

		private bool m_italicSet;

		private bool m_strikethroughSet;

		private bool m_underlineSet;

		private string m_fontName;

		private double m_fontSize;

		public int Bold
		{
			set
			{
				bool flag = value >= 600;
				if (m_boldSet != flag)
				{
					m_builder.Append("&B");
				}
				m_boldSet = flag;
			}
		}

		public bool Italic
		{
			set
			{
				if (m_italicSet != value)
				{
					m_builder.Append("&I");
				}
				m_italicSet = value;
			}
		}

		public bool Strikethrough
		{
			set
			{
				if (m_strikethroughSet != value)
				{
					m_builder.Append("&s");
				}
				m_strikethroughSet = value;
			}
		}

		public ScriptStyle ScriptStyle
		{
			set
			{
			}
		}

		public IColor Color
		{
			set
			{
			}
		}

		public Underline Underline
		{
			set
			{
				bool flag = value != Underline.None;
				if (m_underlineSet != flag)
				{
					m_builder.Append("&u");
				}
				m_underlineSet = flag;
			}
		}

		public string Name
		{
			set
			{
				if (!string.IsNullOrEmpty(value) && value != m_fontName)
				{
					m_builder.Append("&").Append('"');
					FormulaHandler.EncodeHeaderFooterString(m_builder, value);
					m_builder.Append('"');
					m_fontName = value;
				}
			}
		}

		public double Size
		{
			set
			{
				if (value != 0.0 && m_fontSize != value)
				{
					m_builder.Append("&").Append((int)value);
				}
				m_fontSize = value;
			}
		}

		internal string LastFontName => m_fontName;

		internal double LastFontSize => m_fontSize;

		internal HeaderFooterRichTextFont(StringBuilder builder)
		{
			m_builder = builder;
		}
	}
}

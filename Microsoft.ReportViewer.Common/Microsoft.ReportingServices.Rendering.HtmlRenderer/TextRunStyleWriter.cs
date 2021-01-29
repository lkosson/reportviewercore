using Microsoft.ReportingServices.Rendering.RPLProcessing;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class TextRunStyleWriter : ElementStyleWriter
	{
		internal TextRunStyleWriter(IHtmlReportWriter renderer)
			: base(renderer)
		{
		}

		internal override bool NeedsToWriteNullStyle(StyleWriterMode mode)
		{
			return false;
		}

		internal override void WriteStyles(StyleWriterMode mode, IRPLStyle style)
		{
			string text = style[20] as string;
			if (text != null)
			{
				text = HTML4Renderer.HandleSpecialFontCharacters(text);
			}
			WriteStyle(HTML4Renderer.m_fontFamily, text);
			WriteStyle(HTML4Renderer.m_fontSize, style[21]);
			object obj = style[22];
			string text2 = null;
			if (obj != null)
			{
				text2 = EnumStrings.GetValue((RPLFormat.FontWeights)obj);
				WriteStyle(HTML4Renderer.m_fontWeight, text2);
			}
			obj = style[19];
			if (obj != null)
			{
				text2 = EnumStrings.GetValue((RPLFormat.FontStyles)obj);
				WriteStyle(HTML4Renderer.m_fontStyle, text2);
			}
			obj = style[24];
			if (obj != null)
			{
				text2 = EnumStrings.GetValue((RPLFormat.TextDecorations)obj);
				WriteStyle(HTML4Renderer.m_textDecoration, text2);
			}
			WriteStyle(HTML4Renderer.m_color, style[27]);
		}
	}
}

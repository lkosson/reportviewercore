using Microsoft.ReportingServices.Rendering.RPLProcessing;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal abstract class ElementStyleWriter
	{
		protected IHtmlReportWriter m_renderer;

		internal ElementStyleWriter(IHtmlReportWriter renderer)
		{
			m_renderer = renderer;
		}

		internal abstract bool NeedsToWriteNullStyle(StyleWriterMode mode);

		internal abstract void WriteStyles(StyleWriterMode mode, IRPLStyle style);

		protected void WriteStream(string s)
		{
			m_renderer.WriteStream(s);
		}

		protected void WriteStream(byte[] value)
		{
			m_renderer.WriteStream(value);
		}

		protected void WriteStyle(byte[] text, object value)
		{
			if (value != null)
			{
				WriteStream(text);
				WriteStream(value.ToString());
				WriteStream(HTML4Renderer.m_semiColon);
			}
		}

		protected void WriteStyle(byte[] text, object nonShared, object shared)
		{
			object obj = nonShared;
			if (obj == null)
			{
				obj = shared;
			}
			WriteStyle(text, obj);
		}
	}
}

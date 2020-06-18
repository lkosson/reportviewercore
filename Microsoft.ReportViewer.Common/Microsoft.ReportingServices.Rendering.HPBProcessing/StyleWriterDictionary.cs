using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal class StyleWriterDictionary : StyleWriter
	{
		private Dictionary<byte, object> m_styles;

		public StyleWriterDictionary(Dictionary<byte, object> styles)
		{
			m_styles = styles;
		}

		public void Write(byte rplId, string value)
		{
			m_styles.Add(rplId, value);
		}

		public void Write(byte rplId, byte value)
		{
			m_styles.Add(rplId, value);
		}

		public void Write(byte rplId, int value)
		{
			m_styles.Add(rplId, value);
		}

		public void WriteAll(Dictionary<byte, object> styles)
		{
			foreach (KeyValuePair<byte, object> style in styles)
			{
				m_styles.Add(style.Key, style.Value);
			}
		}
	}
}

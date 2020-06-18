using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal class StyleWriterOM : StyleWriter
	{
		private RPLStyleProps m_styleProps;

		public StyleWriterOM(RPLStyleProps styleProps)
		{
			m_styleProps = styleProps;
		}

		public void Write(byte rplId, string value)
		{
			m_styleProps.Add(rplId, value);
		}

		public void Write(byte rplId, byte value)
		{
			m_styleProps.Add(rplId, value);
		}

		public void Write(byte rplId, int value)
		{
			m_styleProps.Add(rplId, value);
		}

		public void WriteAll(Dictionary<byte, object> styles)
		{
			foreach (KeyValuePair<byte, object> style in styles)
			{
				m_styleProps.Add(style.Key, style.Value);
			}
		}
	}
}

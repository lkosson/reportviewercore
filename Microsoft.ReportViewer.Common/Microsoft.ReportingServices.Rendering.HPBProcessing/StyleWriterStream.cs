using Microsoft.ReportingServices.OnDemandReportRendering;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal class StyleWriterStream : StyleWriter
	{
		private BinaryWriter m_writer;

		public StyleWriterStream(BinaryWriter writer)
		{
			m_writer = writer;
		}

		public void WriteNotNull(byte rplId, string value)
		{
			if (value != null)
			{
				Write(rplId, value);
			}
		}

		public void WriteNotNull(byte rplId, byte? value)
		{
			if (value.HasValue)
			{
				Write(rplId, value.Value);
			}
		}

		public void Write(byte rplId, string value)
		{
			m_writer.Write(rplId);
			m_writer.Write(value);
		}

		public void Write(byte rplId, byte value)
		{
			m_writer.Write(rplId);
			m_writer.Write(value);
		}

		public void Write(byte rplId, int value)
		{
			m_writer.Write(rplId);
			m_writer.Write(value);
		}

		public void Write(byte rplId, float value)
		{
			m_writer.Write(rplId);
			m_writer.Write(value);
		}

		public void Write(byte rplId, bool value)
		{
			m_writer.Write(rplId);
			m_writer.Write(value);
		}

		public void WriteAll(Dictionary<byte, object> styles)
		{
			foreach (KeyValuePair<byte, object> style in styles)
			{
				string text = style.Value as string;
				if (text != null)
				{
					Write(style.Key, text);
					continue;
				}
				byte? b = style.Value as byte?;
				if (b.HasValue)
				{
					Write(style.Key, b.Value);
					continue;
				}
				int? num = style.Value as int?;
				if (num.HasValue)
				{
					Write(style.Key, num.Value);
				}
			}
		}

		public void WriteSharedProperty(byte rplId, ReportStringProperty prop)
		{
			if (prop != null && !prop.IsExpression && prop.Value != null)
			{
				Write(rplId, prop.Value);
			}
		}

		public void WriteSharedProperty(byte rplId, ReportSizeProperty prop)
		{
			if (prop != null && !prop.IsExpression && prop.Value != null)
			{
				Write(rplId, prop.Value.ToString());
			}
		}

		public void WriteSharedProperty(byte rplId, ReportIntProperty prop)
		{
			if (prop != null && !prop.IsExpression)
			{
				Write(rplId, prop.Value);
			}
		}
	}
}

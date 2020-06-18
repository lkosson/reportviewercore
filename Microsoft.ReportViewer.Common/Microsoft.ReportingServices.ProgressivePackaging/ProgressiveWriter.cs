using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.ProgressivePackaging
{
	internal class ProgressiveWriter : IMessageWriter, IDisposable
	{
		internal const string Format = "Progressive";

		internal const int MajorVersion = 1;

		internal const int MinorVersion = 0;

		private BinaryWriter m_writer;

		private bool m_disposed;

		private LengthEncodedWritableStream m_lastStream;

		internal ProgressiveWriter(BinaryWriter writer)
		{
			m_writer = writer;
		}

		public void WriteMessage(string name, object value)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name");
			}
			if (StringComparer.OrdinalIgnoreCase.Compare(name, ".") == 0)
			{
				m_writer.Write(name);
				return;
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			Type type = VerifyType(name, value.GetType());
			VerifyLastStream();
			m_writer.Write(name);
			InternalWriteValue(value, type);
		}

		public Stream CreateWritableStream(string name)
		{
			VerifyType(name, typeof(Stream));
			VerifyLastStream();
			m_lastStream = new LengthEncodedWritableStream(m_writer, name);
			return m_lastStream;
		}

		private Type VerifyType(string name, Type type)
		{
			Type type2 = ProgressiveTypeDictionary.GetType(name);
			if (type2 == null)
			{
				throw new NotImplementedException();
			}
			if (!type2.IsAssignableFrom(type))
			{
				throw new ArgumentException("wrong type", "value");
			}
			return type2;
		}

		private void InternalWriteValue(object value, Type type)
		{
			if (type == typeof(string))
			{
				m_writer.Write(value as string);
				return;
			}
			if (type == typeof(string[]))
			{
				MessageUtil.WriteStringArray(m_writer, value as string[]);
				return;
			}
			if (type == typeof(bool))
			{
				m_writer.Write((bool)value);
				return;
			}
			if (type == typeof(int))
			{
				m_writer.Write((int)value);
				return;
			}
			if (type == typeof(Stream))
			{
				throw new InvalidOperationException("stream");
			}
			if (type == typeof(Dictionary<string, object>))
			{
				Write((Dictionary<string, object>)value);
				return;
			}
			throw new NotImplementedException();
		}

		private void Write(Dictionary<string, object> value)
		{
			m_writer.Write(value.Count);
			foreach (KeyValuePair<string, object> item in value)
			{
				if (item.Value is bool)
				{
					m_writer.Write(3);
					m_writer.Write(item.Key);
					m_writer.Write((bool)item.Value);
					continue;
				}
				throw new NotImplementedException();
			}
		}

		public void Dispose()
		{
			if (!m_disposed)
			{
				m_writer.Write(string.Empty);
				m_disposed = true;
				if (m_lastStream != null)
				{
					m_lastStream.Dispose();
					m_lastStream = null;
				}
			}
		}

		private void VerifyLastStream()
		{
			if (m_lastStream != null)
			{
				if (!m_lastStream.Closed)
				{
					throw new InvalidOperationException("last stream");
				}
				m_lastStream = null;
			}
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.ProgressivePackaging
{
	internal class ProgressiveReader : IMessageReader, IEnumerable<MessageElement>, IEnumerable, IDisposable
	{
		internal const string Format = "Progressive";

		internal const int MajorVersion = 1;

		internal const int MinorVersion = 0;

		private BinaryReader m_reader;

		private LengthEncodedReadableStream m_lastStream;

		private bool m_enumeratorCreated;

		internal ProgressiveReader(BinaryReader reader)
		{
			m_reader = reader;
		}

		private object InternalReadValue(Type type)
		{
			if (type == typeof(string))
			{
				return m_reader.ReadString();
			}
			if (type == typeof(string[]))
			{
				return MessageUtil.ReadStringArray(m_reader);
			}
			if (type == typeof(bool))
			{
				return m_reader.ReadBoolean();
			}
			if (type == typeof(int))
			{
				return m_reader.ReadInt32();
			}
			if (type == typeof(Stream))
			{
				m_lastStream = new LengthEncodedReadableStream(m_reader);
				return m_lastStream;
			}
			if (type == typeof(Dictionary<string, object>))
			{
				return ReadDictionary();
			}
			throw new NotImplementedException();
		}

		private Dictionary<string, object> ReadDictionary()
		{
			int num = m_reader.ReadInt32();
			Dictionary<string, object> dictionary = new Dictionary<string, object>(num);
			for (int i = 0; i < num; i++)
			{
				if (m_reader.ReadInt32() == 3)
				{
					dictionary.Add(m_reader.ReadString(), m_reader.ReadBoolean());
					continue;
				}
				throw new NotImplementedException();
			}
			return dictionary;
		}

		public IEnumerator<MessageElement> GetEnumerator()
		{
			if (m_enumeratorCreated)
			{
				throw new InvalidOperationException();
			}
			m_enumeratorCreated = true;
			while (m_reader.BaseStream.CanRead)
			{
				string name;
				object value;
				try
				{
					VerifyLastStream();
					name = m_reader.ReadString();
					if (string.IsNullOrEmpty(name))
					{
						yield break;
					}
					if (StringComparer.OrdinalIgnoreCase.Compare(name, ".") == 0)
					{
						continue;
					}
					Type type = ProgressiveTypeDictionary.GetType(name);
					if (type == null)
					{
						throw new NotImplementedException();
					}
					value = InternalReadValue(type);
					goto IL_00c9;
				}
				catch (EndOfStreamException innerException)
				{
					throw new IOException("end of stream", innerException);
				}
				catch (IOException)
				{
					throw;
				}
				catch (Exception innerException2)
				{
					throw new IOException("reader", innerException2);
				}
				IL_00c9:
				yield return new MessageElement(name, value);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<MessageElement>)this).GetEnumerator();
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

		public void Dispose()
		{
			if (m_lastStream != null)
			{
				m_lastStream.Dispose();
				m_lastStream = null;
			}
		}
	}
}

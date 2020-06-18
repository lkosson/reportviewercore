using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class Bookmarks
	{
		private List<int> m_offsets;

		private List<string> m_names;

		internal int Count => m_names.Count;

		internal Bookmarks()
		{
			m_offsets = new List<int>();
			m_names = new List<string>();
		}

		internal static char[] EscapeText(string text, out int length)
		{
			char[] array = new char[text.Length];
			length = 0;
			for (int i = 0; i < text.Length; i++)
			{
				switch (text[i])
				{
				case '\r':
					if (i < text.Length - 1 && text[i + 1] == '\n')
					{
						array[length++] = '_';
						i++;
					}
					else
					{
						array[length++] = '_';
					}
					break;
				case '\u0001':
				case '\u0002':
				case '\u0003':
				case '\u0004':
				case '\u0005':
				case '\u0006':
				case '\a':
				case '\b':
				case '\t':
				case '\n':
				case '\v':
				case '\f':
				case '\u000e':
				case '\u000f':
				case '\u0010':
				case '\u0011':
				case '\u0012':
				case '\u0013':
				case '\u0014':
				case '\u0015':
				case '\u0016':
				case '\u0017':
				case '\u0018':
				case '\u0019':
				case '\u001a':
				case '\u001b':
				case '\u001c':
				case '\u001d':
				case '\u001e':
				case '\u001f':
				case ' ':
					array[length++] = '_';
					break;
				default:
					array[length++] = text[i];
					break;
				}
			}
			return array;
		}

		internal static string CleanseName(string name)
		{
			int length = 0;
			name = new string(EscapeText(name, out length), 0, length);
			if (name.Length > 40)
			{
				return name.Substring(0, 40);
			}
			return name;
		}

		internal void AddBookmark(string name, int cp)
		{
			m_names.Add(CleanseName(name));
			m_offsets.Add(cp);
		}

		internal void SerializeStarts(BinaryWriter writer, int cpEnd)
		{
			for (int i = 0; i < m_offsets.Count; i++)
			{
				writer.Write(m_offsets[i]);
			}
			writer.Write(cpEnd);
			for (int j = 0; j < m_offsets.Count; j++)
			{
				writer.Write(j);
			}
			writer.Flush();
		}

		internal void SerializeEnds(BinaryWriter writer, int cpEnd)
		{
			for (int i = 0; i < m_offsets.Count; i++)
			{
				writer.Write(m_offsets[i]);
			}
			writer.Write(cpEnd);
			writer.Flush();
		}

		internal void SerializeNames(BinaryWriter writer)
		{
			writer.Write((short)(-1));
			writer.Write((short)m_names.Count);
			writer.Write((short)0);
			for (int i = 0; i < m_names.Count; i++)
			{
				string text = m_names[i];
				writer.Write((short)text.Length);
				writer.Flush();
				byte[] bytes = Encoding.Unicode.GetBytes(text);
				writer.Write(bytes, 0, bytes.Length);
				writer.Flush();
			}
		}
	}
}

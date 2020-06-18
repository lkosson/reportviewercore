using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class SSTHandler
	{
		private List<int> m_recordSize;

		internal int m_count;

		private Dictionary<StringWrapperBIFF8, int> m_stringTable;

		private List<StringWrapperBIFF8> m_stringOrder;

		private int m_offset;

		private int m_totalStrings;

		private int m_baseUniqueStrings;

		internal int TotalStrings => m_totalStrings;

		internal int UniqueStrings
		{
			get
			{
				if (m_stringTable.Count < m_baseUniqueStrings)
				{
					return m_baseUniqueStrings;
				}
				return m_stringTable.Count;
			}
		}

		internal SSTHandler()
		{
			m_stringTable = new Dictionary<StringWrapperBIFF8, int>();
			m_stringOrder = new List<StringWrapperBIFF8>();
			m_offset = 0;
		}

		private int CalcSize()
		{
			m_recordSize = new List<int>();
			int num = 8228;
			int num2 = 12;
			int num3 = 0;
			int num4 = num - num2;
			StringChunkInfo stringChunkInfo = new StringChunkInfo();
			foreach (StringWrapperBIFF8 item in m_stringOrder)
			{
				stringChunkInfo.CharsTotal = item.Cch;
				int headerSize = item.HeaderSize;
				int num5 = num4 - headerSize;
				if (num5 < 0 || (num5 == 0 && item.Cch != 0))
				{
					m_recordSize.Add(num2);
					num3 += num2;
					num2 = 4;
					num4 = num - num2;
				}
				int aBytesAvailable = num4 - headerSize;
				num2 += headerSize;
				item.PrepareWriteCharacterData(stringChunkInfo, aBytesAvailable);
				num2 += item.GetCharacterDataSize(stringChunkInfo, aBytesAvailable);
				num4 = ((!stringChunkInfo.HasMore) ? (num - num2) : 0);
				while (stringChunkInfo.HasMore)
				{
					aBytesAvailable = num4;
					item.PrepareWriteCharacterData(stringChunkInfo, aBytesAvailable);
					int characterDataSize = item.GetCharacterDataSize(stringChunkInfo, aBytesAvailable);
					num2 += characterDataSize;
					if (characterDataSize == 0)
					{
						m_recordSize.Add(num2);
						num3 += num2;
						num2 = 5;
						aBytesAvailable = 8223;
						item.PrepareWriteCharacterData(stringChunkInfo, aBytesAvailable);
						num2 += item.GetCharacterDataSize(stringChunkInfo, aBytesAvailable);
					}
					num4 = ((!stringChunkInfo.HasMore) ? (num - num2) : 0);
				}
				int num6 = item.FormatRunsDataSize;
				if (num6 <= 0)
				{
					continue;
				}
				while (num6 > 0)
				{
					if (num4 == 0)
					{
						m_recordSize.Add(num2);
						num3 += num2;
						num2 = 4;
						num4 = num - num2;
					}
					int num7 = (num4 < num6) ? num4 : num6;
					num7 -= num7 % 4;
					if (num7 > 0)
					{
						num4 -= num7;
						num6 -= num7;
						num2 += num7;
					}
					else
					{
						num4 = 0;
					}
				}
			}
			m_recordSize.Add(num2);
			return num3 + num2;
		}

		private int InternalAddString(StringWrapperBIFF8 aWrapper)
		{
			m_totalStrings++;
			m_stringTable[aWrapper] = m_stringOrder.Count;
			m_stringOrder.Add(aWrapper);
			if (m_stringTable.Count < m_baseUniqueStrings)
			{
				m_baseUniqueStrings++;
			}
			m_offset += aWrapper.Size;
			return m_stringOrder.Count - 1;
		}

		private void StartRecord(Stream aOut, short aRecordType)
		{
			LittleEndianHelper.WriteShortU(aOut, aRecordType);
			int aVal = m_recordSize[m_count++] - 4;
			LittleEndianHelper.WriteShortU(aOut, aVal);
		}

		internal string GetString(int aOffset)
		{
			if (aOffset < m_stringOrder.Count)
			{
				return m_stringOrder[aOffset].String;
			}
			return null;
		}

		internal StringWrapperBIFF8 GetStringWrapper(int aOffset)
		{
			if (aOffset < m_stringOrder.Count)
			{
				return m_stringOrder[aOffset];
			}
			return null;
		}

		internal int AddString(string aStr)
		{
			return AddString(new StringWrapperBIFF8(aStr));
		}

		internal int AddString(StringWrapperBIFF8 aWrapper)
		{
			if (m_stringTable.ContainsKey(aWrapper))
			{
				return m_stringTable[aWrapper];
			}
			return InternalAddString(aWrapper);
		}

		internal void Write(Stream aOut)
		{
			CalcSize();
			m_count = 0;
			int num = 8228;
			int num2 = 12;
			StartRecord(aOut, 252);
			LittleEndianHelper.WriteIntU(aOut, m_totalStrings);
			LittleEndianHelper.WriteIntU(aOut, UniqueStrings);
			int num3 = num - num2;
			StringChunkInfo stringChunkInfo = new StringChunkInfo();
			foreach (StringWrapperBIFF8 item in m_stringOrder)
			{
				stringChunkInfo.CharsTotal = item.Cch;
				int headerSize = item.HeaderSize;
				int num4 = num3 - headerSize;
				if (num4 < 0 || (num4 == 0 && item.Cch != 0))
				{
					StartRecord(aOut, 60);
					num2 = 4;
					num3 = num - num2;
				}
				int aBytesAvailable = num3 - headerSize;
				num2 += headerSize;
				item.PrepareWriteCharacterData(stringChunkInfo, aBytesAvailable);
				item.WriteHeaderData(aOut, stringChunkInfo.Compressed);
				num2 += item.WriteCharacterData(aOut, stringChunkInfo, aBytesAvailable);
				num3 = ((!stringChunkInfo.HasMore) ? (num - num2) : 0);
				while (stringChunkInfo.HasMore)
				{
					aBytesAvailable = num3;
					int charPos = stringChunkInfo.CharPos;
					int characterDataSize = item.GetCharacterDataSize(stringChunkInfo, aBytesAvailable);
					stringChunkInfo.CharPos = charPos;
					if (characterDataSize == 0)
					{
						StartRecord(aOut, 60);
						num2 = 5;
						aBytesAvailable = 8223;
						item.PrepareWriteCharacterData(stringChunkInfo, aBytesAvailable);
						aOut.WriteByte((byte)((!stringChunkInfo.Compressed) ? 1 : 0));
					}
					else
					{
						item.PrepareWriteCharacterData(stringChunkInfo, aBytesAvailable);
					}
					num2 += item.WriteCharacterData(aOut, stringChunkInfo, aBytesAvailable);
					num3 = ((!stringChunkInfo.HasMore) ? (num - num2) : 0);
				}
				int num5 = item.FormatRunsDataSize;
				if (num5 <= 0)
				{
					continue;
				}
				byte[] formatRunsData = item.FormatRunsData;
				while (num5 > 0)
				{
					if (num3 == 0)
					{
						StartRecord(aOut, 60);
						num2 = 4;
						num3 = num - num2;
					}
					int num6 = (num3 < num5) ? num3 : num5;
					num6 -= num6 % 4;
					if (num6 > 0)
					{
						aOut.Write(formatRunsData, formatRunsData.Length - num5, num6);
						num3 -= num6;
						num5 -= num6;
						num2 += num6;
					}
					else
					{
						num3 = 0;
					}
				}
			}
		}
	}
}

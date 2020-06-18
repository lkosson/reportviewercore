using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8.Records;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal class StringWrapperBIFF8
	{
		private byte m_grbit = 1;

		private string m_rgb;

		private List<Pair<int, int>> m_runsList;

		private int m_hash;

		private const byte m_fHighByte = 1;

		private const byte m_fExtSt = 4;

		private const byte m_fRichSt = 8;

		internal int DataSize => CharacterSize + FinalDataSize;

		internal int CharacterSize
		{
			get
			{
				if ((m_grbit & 1) == 0)
				{
					return m_rgb.Length;
				}
				return m_rgb.Length * 2;
			}
		}

		internal int FinalDataSize => FormatRunsDataSize;

		internal int FormatRunsDataSize
		{
			get
			{
				int result = 0;
				if (m_runsList != null)
				{
					result = (((m_grbit & 8) != 0) ? (m_runsList.Count * 4) : 0);
				}
				return result;
			}
		}

		internal int Size => HeaderSize + DataSize;

		internal int HeaderSize
		{
			get
			{
				int num = 3;
				if ((m_grbit & 8) != 0)
				{
					num += 2;
				}
				return num;
			}
		}

		internal byte[] FinalData
		{
			get
			{
				byte[] array = new byte[FinalDataSize];
				byte[] formatRunsData = FormatRunsData;
				Array.Copy(formatRunsData, array, formatRunsData.Length);
				return array;
			}
		}

		internal byte[] FormatRunsData
		{
			get
			{
				int formatRunsDataSize = FormatRunsDataSize;
				if (formatRunsDataSize > 0)
				{
					byte[] array = new byte[formatRunsDataSize];
					int num = 0;
					for (int i = 0; i < m_runsList.Count; i++)
					{
						Pair<int, int> pair = m_runsList[i];
						LittleEndianHelper.WriteShortU(pair.First, array, num);
						LittleEndianHelper.WriteShortU(pair.Second, array, num + 2);
						num += 4;
					}
					return array;
				}
				return new byte[0];
			}
		}

		internal string String => m_rgb;

		internal StringChunkInfo ChunkInfo => new StringChunkInfo
		{
			CharsTotal = m_rgb.Length
		};

		internal bool FirstChunkCompressed => (m_grbit & 1) == 0;

		internal int RunCount
		{
			get
			{
				if (m_runsList == null)
				{
					return 0;
				}
				return m_runsList.Count;
			}
		}

		internal int Cch => m_rgb.Length;

		internal StringWrapperBIFF8(string aStr)
		{
			m_rgb = aStr;
		}

		public override int GetHashCode()
		{
			if (m_hash == 0)
			{
				m_hash = m_rgb.GetHashCode();
				m_hash <<= 3;
				if (m_runsList != null)
				{
					m_hash |= m_runsList.Count;
				}
			}
			return m_hash;
		}

		public override bool Equals(object o)
		{
			StringWrapperBIFF8 stringWrapperBIFF = (StringWrapperBIFF8)o;
			if (m_runsList == null != (stringWrapperBIFF.m_runsList == null))
			{
				return false;
			}
			if ((stringWrapperBIFF.m_grbit & 0xFE) != (m_grbit & 0xFE))
			{
				return false;
			}
			if (stringWrapperBIFF.m_rgb.Length != m_rgb.Length)
			{
				return false;
			}
			if (string.Compare(stringWrapperBIFF.m_rgb, m_rgb, StringComparison.Ordinal) != 0)
			{
				return false;
			}
			if (m_runsList != null && m_runsList.Count > 0)
			{
				for (int i = 0; i < m_runsList.Count; i++)
				{
					Pair<int, int> pair = stringWrapperBIFF.m_runsList[i];
					Pair<int, int> pair2 = m_runsList[i];
					if (pair.First != pair2.First || pair.Second != pair2.Second)
					{
						return false;
					}
				}
			}
			return true;
		}

		public override string ToString()
		{
			return String;
		}

		internal void SetRunsList(List<Pair<int, int>> value)
		{
			m_hash = 0;
			m_runsList = value;
			if (m_runsList != null && m_runsList.Count > 0)
			{
				m_grbit |= 8;
			}
			else
			{
				m_grbit &= 247;
			}
		}

		internal void WriteHeaderData(Stream aOut, bool aCompressed)
		{
			if (aCompressed)
			{
				m_grbit &= 254;
			}
			else
			{
				m_grbit |= 1;
			}
			LittleEndianHelper.WriteShortU(aOut, m_rgb.Length);
			aOut.WriteByte(m_grbit);
			if (m_runsList != null)
			{
				if ((m_grbit & 8) != 0)
				{
					LittleEndianHelper.WriteShortU(aOut, m_runsList.Count);
				}
			}
			else if ((m_grbit & 8) != 0)
			{
				LittleEndianHelper.WriteShortU(aOut, 0);
			}
		}

		internal byte[] GetCharacterData(StringChunkInfo aChunkInfo, int aBytesAvailable)
		{
			if (aBytesAvailable <= 0)
			{
				aChunkInfo.Compressed = true;
				aChunkInfo.Data = null;
				return aChunkInfo.Data;
			}
			int num = Math.Min(Cch - aChunkInfo.CharPos, aBytesAvailable);
			if (num == 0)
			{
				aChunkInfo.Compressed = true;
				aChunkInfo.Data = null;
				return aChunkInfo.Data;
			}
			string text = String.Substring(aChunkInfo.CharPos, aChunkInfo.CharPos + num - aChunkInfo.CharPos);
			if (StringUtil.CanCompress(text))
			{
				aChunkInfo.Compressed = true;
				aChunkInfo.Data = StringUtil.DecodeTo1Byte(text);
				aChunkInfo.CharPos += num;
				return aChunkInfo.Data;
			}
			int num2 = Math.Min(Cch - aChunkInfo.CharPos, aBytesAvailable / 2);
			byte[] data;
			if (num2 != 0)
			{
				text = text.Substring(0, num2);
				data = StringUtil.DecodeTo2ByteLE(text);
			}
			else
			{
				data = new byte[0];
			}
			aChunkInfo.Compressed = false;
			aChunkInfo.Data = data;
			aChunkInfo.CharPos += num2;
			return aChunkInfo.Data;
		}

		internal void PrepareWriteCharacterData(StringChunkInfo aChunkInfo, int aBytesAvailable)
		{
			if (aBytesAvailable <= 0)
			{
				aChunkInfo.Compressed = true;
				return;
			}
			int num = Math.Min(Cch - aChunkInfo.CharPos, aBytesAvailable);
			if (num == 0)
			{
				aChunkInfo.Compressed = true;
			}
			else
			{
				aChunkInfo.Compressed = StringUtil.CanCompress(m_rgb, aChunkInfo.CharPos, num);
			}
		}

		internal int WriteCharacterData(Stream aOut, StringChunkInfo aChunkInfo, int aBytesAvailable)
		{
			if (aBytesAvailable <= 0)
			{
				aChunkInfo.Compressed = true;
				aChunkInfo.Data = null;
				return 0;
			}
			int num = Math.Min(Cch - aChunkInfo.CharPos, aBytesAvailable);
			if (num == 0)
			{
				aChunkInfo.Compressed = true;
				aChunkInfo.Data = null;
				return 0;
			}
			if (aChunkInfo.Compressed)
			{
				StringUtil.DecodeTo1Byte(aOut, m_rgb, aChunkInfo.CharPos, num);
				aChunkInfo.CharPos += num;
				return num;
			}
			int num2 = Math.Min(Cch - aChunkInfo.CharPos, aBytesAvailable / 2);
			if (num2 != 0)
			{
				StringUtil.DecodeTo2ByteLE(aOut, m_rgb, aChunkInfo.CharPos, num2);
			}
			aChunkInfo.CharPos += num2;
			return num2 * 2;
		}

		internal int GetCharacterDataSize(StringChunkInfo aChunkInfo, int aBytesAvailable)
		{
			if (aBytesAvailable <= 0)
			{
				aChunkInfo.Compressed = true;
				aChunkInfo.Data = null;
				return 0;
			}
			int num = Math.Min(Cch - aChunkInfo.CharPos, aBytesAvailable);
			if (num == 0)
			{
				aChunkInfo.Compressed = true;
				aChunkInfo.Data = null;
				return 0;
			}
			if (aChunkInfo.Compressed)
			{
				aChunkInfo.CharPos += num;
				return num;
			}
			int num2 = Math.Min(Cch - aChunkInfo.CharPos, aBytesAvailable / 2);
			aChunkInfo.CharPos += num2;
			return num2 * 2;
		}
	}
}

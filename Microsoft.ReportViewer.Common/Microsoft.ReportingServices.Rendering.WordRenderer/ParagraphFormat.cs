using Microsoft.ReportingServices.Rendering.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class ParagraphFormat : Format
	{
		private const int MaxPapSprmSize = 493;

		private List<int> m_papOffsets;

		private byte[] m_papFkp;

		private Stream m_papTable;

		private int m_papOffsetOffset;

		private int m_papGrpprlOffset;

		private int m_papFcOffset;

		private int m_parStart;

		private int m_fcStart;

		internal short StyleIndex
		{
			get
			{
				return m_grpprl.StyleIndex;
			}
			set
			{
				m_grpprl.StyleIndex = value;
			}
		}

		internal Stream Stream => m_papTable;

		internal List<int> Offsets => m_papOffsets;

		internal ParagraphFormat(Stream papTable, int fcStart)
			: base(493, 2)
		{
			m_papTable = papTable;
			m_fcStart = fcStart;
			m_papFkp = new byte[512];
			m_papGrpprlOffset = 510;
			m_papFcOffset = 4;
			m_papOffsetOffset = m_papFcOffset;
			m_papOffsets = new List<int>();
			m_papOffsets.Add(m_fcStart);
			LittleEndian.PutInt(m_papFkp, m_fcStart);
		}

		internal void CommitParagraph(int cpEnd, TableData currentRow, Stream dataStream)
		{
			byte[] array = m_grpprl.Buf;
			int num = m_grpprl.Offset;
			if (currentRow != null)
			{
				array = currentRow.Tapx;
				num = array.Length;
				if (num >= 488)
				{
					int param = (int)dataStream.Length;
					BinaryWriter binaryWriter = new BinaryWriter(dataStream);
					binaryWriter.Write((ushort)(array.Length - 2));
					binaryWriter.Write(array, 2, array.Length - 2);
					binaryWriter.Flush();
					SprmBuffer sprmBuffer = new SprmBuffer(8, 2);
					sprmBuffer.AddSprm(26182, param, null);
					array = sprmBuffer.Buf;
					num = sprmBuffer.Offset;
				}
			}
			if (!AddPropToPapFkp(m_papFkp, cpEnd, m_papFcOffset, new byte[13], m_papOffsetOffset, array, num, m_papGrpprlOffset))
			{
				m_papTable.Write(m_papFkp, 0, m_papFkp.Length);
				m_papOffsets.Add(m_parStart * 2 + m_fcStart);
				Array.Clear(m_papFkp, 0, m_papFkp.Length);
				LittleEndian.PutInt(m_papFkp, m_parStart * 2 + m_fcStart);
				m_papFcOffset = 4;
				m_papOffsetOffset = m_papFcOffset;
				m_papGrpprlOffset = 510;
				AddPropToPapFkp(m_papFkp, cpEnd, m_papFcOffset, new byte[13], m_papOffsetOffset, array, num, m_papGrpprlOffset);
			}
			m_papFkp[511]++;
			m_papFcOffset += 4;
			m_papOffsetOffset += 17;
			int num2 = num + 1;
			m_papGrpprlOffset = m_papGrpprlOffset - num2 - num2 % 2;
			m_parStart = cpEnd;
			m_grpprl.ClearStyle();
			m_grpprl.Reset();
		}

		private bool AddPropToPapFkp(byte[] fkp, int cpEnd, int fcOffset, byte[] midEntry, int offsetOffset, byte[] grpprl, int grpprlEnd, int grpprlOffset)
		{
			int num = 0;
			byte[] array = null;
			int num2 = grpprlEnd;
			if (grpprlEnd > 0)
			{
				if (grpprlEnd % 2 == 1)
				{
					num = grpprlEnd + 1;
					array = new byte[1];
					num2 = grpprlEnd + 1;
				}
				else
				{
					num = grpprlEnd + 2;
					array = new byte[2];
				}
			}
			int num3 = grpprlOffset - offsetOffset;
			if (4 + midEntry.Length + num > num3)
			{
				return false;
			}
			int num4 = (grpprlEnd > 0) ? (grpprlOffset - num) : 0;
			Array.Copy(fkp, fcOffset, fkp, fcOffset + 4, offsetOffset - fcOffset);
			LittleEndian.PutInt(fkp, fcOffset, cpEnd * 2 + m_fcStart);
			midEntry[0] = (byte)(num4 / 2);
			Array.Copy(midEntry, 0, fkp, offsetOffset + 4, midEntry.Length);
			if (grpprlEnd > 0)
			{
				Array.Copy(grpprl, 0, fkp, num4 + array.Length, grpprlEnd);
				array[array.Length - 1] = (byte)(num2 / 2);
				Array.Copy(array, 0, fkp, num4, array.Length);
			}
			return true;
		}

		internal void Finish(int lastCp)
		{
			m_papOffsets.Add(lastCp * 2 + m_fcStart);
			m_papTable.Write(m_papFkp, 0, m_papFkp.Length);
		}

		internal void SetIsInTable(int m_nestingLevel)
		{
			m_grpprl.AddSprm(9238, 1, null);
			m_grpprl.AddSprm(26185, m_nestingLevel, null);
		}

		internal void WriteBinTableTo(BinaryWriter tableWriter, ref int pageStart)
		{
			foreach (int papOffset in m_papOffsets)
			{
				tableWriter.Write(papOffset);
			}
			for (int i = 0; i < m_papOffsets.Count - 1; i++)
			{
				tableWriter.Write(pageStart++);
			}
		}
	}
}

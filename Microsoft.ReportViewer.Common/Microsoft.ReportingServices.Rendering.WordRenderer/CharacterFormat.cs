using Microsoft.ReportingServices.Rendering.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class CharacterFormat : Format
	{
		private const int MaxChpSprmSize = 503;

		private int m_chpOffsetOffset;

		private int m_chpGrpprlOffset;

		private Stream m_chpTable;

		private List<int> m_chpOffsets;

		private byte[] m_chpFkp;

		private Stack<SprmBuffer> m_formatStack;

		private int m_chpFcOffset;

		private int m_fcStart;

		internal Stream Stream => m_chpTable;

		internal List<int> Offsets => m_chpOffsets;

		internal CharacterFormat(Stream chpTable, int fcStart)
			: base(503, 0)
		{
			m_chpTable = chpTable;
			m_fcStart = fcStart;
			m_chpFkp = new byte[512];
			m_chpGrpprlOffset = 510;
			m_chpFcOffset = 4;
			m_chpOffsetOffset = m_chpFcOffset;
			m_chpOffsets = new List<int>();
			m_chpOffsets.Add(m_fcStart);
			m_formatStack = new Stack<SprmBuffer>(1);
			LittleEndian.PutInt(m_chpFkp, m_fcStart);
		}

		internal void CommitLastCharacterRun(int cpStart, int cpEnd)
		{
			byte[] buf = m_grpprl.Buf;
			int offset = m_grpprl.Offset;
			if (!AddPropToChpFkp(m_chpFkp, cpEnd, m_chpFcOffset, new byte[1], m_chpOffsetOffset, buf, offset, m_chpGrpprlOffset))
			{
				m_chpTable.Write(m_chpFkp, 0, m_chpFkp.Length);
				m_chpOffsets.Add(cpStart * 2 + m_fcStart);
				Array.Clear(m_chpFkp, 0, m_chpFkp.Length);
				LittleEndian.PutInt(m_chpFkp, cpStart * 2 + m_fcStart);
				m_chpFcOffset = 4;
				m_chpOffsetOffset = m_chpFcOffset;
				m_chpGrpprlOffset = 510;
				AddPropToChpFkp(m_chpFkp, cpEnd, m_chpFcOffset, new byte[1], m_chpOffsetOffset, buf, offset, m_chpGrpprlOffset);
			}
			m_chpFkp[511]++;
			m_chpFcOffset += 4;
			m_chpOffsetOffset += 5;
			int num = m_grpprl.Offset + 1;
			m_chpGrpprlOffset = m_chpGrpprlOffset - num - num % 2;
			m_grpprl.Reset();
		}

		private bool AddPropToChpFkp(byte[] fkp, int cpEnd, int fcOffset, byte[] midEntry, int offsetOffset, byte[] grpprl, int grpprlEnd, int grpprlOffset)
		{
			int num = (grpprlEnd > 0) ? (grpprlEnd + 1) : 0;
			int num2 = grpprlOffset - offsetOffset;
			if (4 + midEntry.Length + num + num % 2 > num2)
			{
				return false;
			}
			int num3 = (grpprlEnd > 0) ? (grpprlOffset - num - num % 2) : 0;
			Array.Copy(fkp, fcOffset, fkp, fcOffset + 4, offsetOffset - fcOffset);
			LittleEndian.PutInt(fkp, fcOffset, cpEnd * 2 + m_fcStart);
			midEntry[0] = (byte)(num3 / 2);
			Array.Copy(midEntry, 0, fkp, offsetOffset + 4, midEntry.Length);
			if (grpprlEnd > 0)
			{
				Array.Copy(grpprl, 0, fkp, num3 + 1, grpprlEnd);
				fkp[num3] = (byte)grpprlEnd;
			}
			return true;
		}

		internal void Finish(int lastCp)
		{
			m_chpOffsets.Add(lastCp * 2 + m_fcStart);
			m_chpTable.Write(m_chpFkp, 0, m_chpFkp.Length);
		}

		internal void Push(int bufSize)
		{
			m_formatStack.Push(m_grpprl);
			m_grpprl = new SprmBuffer(bufSize, 0);
		}

		internal void CopyAndPush()
		{
			m_formatStack.Push(m_grpprl);
			m_grpprl = (SprmBuffer)m_grpprl.Clone();
		}

		internal void Pop()
		{
			if (m_formatStack.Peek() != null)
			{
				m_grpprl = m_formatStack.Pop();
			}
		}

		internal void SetIsInlineImage(int position)
		{
			m_grpprl.AddSprm(2133, 1, null);
			m_grpprl.AddSprm(27139, position, null);
		}

		internal void WriteBinTableTo(BinaryWriter tableWriter, ref int pageStart)
		{
			foreach (int chpOffset in m_chpOffsets)
			{
				tableWriter.Write(chpOffset);
			}
			for (int i = 0; i < m_chpOffsets.Count - 1; i++)
			{
				tableWriter.Write(pageStart++);
			}
		}
	}
}

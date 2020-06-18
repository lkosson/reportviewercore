using Microsoft.ReportingServices.Rendering.Utilities;
using System;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class SprmBuffer : ICloneable
	{
		private byte[] m_grpprl;

		private int m_offset;

		private int m_initialOffset;

		internal byte[] Buf => m_grpprl;

		internal int Offset => m_offset;

		internal short StyleIndex
		{
			get
			{
				return LittleEndian.getShort(m_grpprl, 0);
			}
			set
			{
				LittleEndian.PutShort(m_grpprl, value);
			}
		}

		internal SprmBuffer(int initialSize, int initialOffset)
		{
			m_grpprl = new byte[initialSize];
			m_offset = initialOffset;
			m_initialOffset = initialOffset;
		}

		internal void AddSprm(ushort instruction, int param, byte[] varParam)
		{
			int num = (instruction & 0xE000) >> 13;
			byte[] array = null;
			switch (num)
			{
			case 0:
			case 1:
				array = new byte[3]
				{
					0,
					0,
					(byte)param
				};
				break;
			case 2:
				array = new byte[4];
				LittleEndian.PutUShort(array, 2, (ushort)param);
				break;
			case 3:
				array = new byte[6];
				LittleEndian.PutInt(array, 2, param);
				break;
			case 4:
			case 5:
				array = new byte[4];
				LittleEndian.PutUShort(array, 2, (ushort)param);
				break;
			case 6:
				array = new byte[3 + varParam.Length];
				array[2] = (byte)varParam.Length;
				Array.Copy(varParam, 0, array, 3, varParam.Length);
				break;
			case 7:
			{
				array = new byte[5];
				byte[] array2 = new byte[4];
				LittleEndian.PutInt(array2, 0, param);
				Array.Copy(array2, 0, array, 2, 3);
				break;
			}
			}
			LittleEndian.PutUShort(array, 0, instruction);
			Array.Copy(array, 0, m_grpprl, m_offset, array.Length);
			m_offset += array.Length;
		}

		internal void AddRawSprmData(byte[] buf)
		{
			Array.Copy(buf, 0, m_grpprl, m_offset, buf.Length);
			m_offset += buf.Length;
		}

		internal void Clear(int start, int length)
		{
			Array.Clear(m_grpprl, start, length);
		}

		internal void Reset(int offset)
		{
			m_offset = offset;
		}

		internal void Reset()
		{
			Reset(m_initialOffset);
		}

		internal void ClearStyle()
		{
			Array.Clear(m_grpprl, 0, 2);
		}

		public object Clone()
		{
			SprmBuffer sprmBuffer = new SprmBuffer(m_grpprl.Length, m_initialOffset);
			sprmBuffer.m_offset = m_offset;
			Array.Copy(m_grpprl, sprmBuffer.m_grpprl, m_offset);
			return sprmBuffer;
		}
	}
}

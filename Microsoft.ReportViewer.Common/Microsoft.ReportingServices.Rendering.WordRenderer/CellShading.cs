using Microsoft.ReportingServices.Rendering.Utilities;
using System;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class CellShading
	{
		internal const int ShdSize = 10;

		private byte[] m_cellShading;

		private byte[] m_cellShading2;

		private byte[] m_cellShading3;

		private byte[] m_tableShd;

		internal int SprmSize => 3 + m_cellShading.Length + ((m_cellShading2 != null) ? (3 + m_cellShading2.Length) : 0) + ((m_cellShading3 != null) ? (3 + m_cellShading3.Length) : 0);

		internal CellShading(int numColumns, byte[] tableShd)
		{
			m_cellShading = new byte[10 * Math.Min(numColumns, 22)];
			if (numColumns > 22)
			{
				m_cellShading2 = new byte[10 * Math.Min(numColumns - 22, 22)];
				if (numColumns > 44)
				{
					m_cellShading3 = new byte[10 * Math.Min(numColumns - 44, 22)];
				}
			}
			m_tableShd = tableShd;
			InitShading();
		}

		internal byte[] ToByteArray()
		{
			byte[] array = new byte[SprmSize];
			int num = 0;
			num += Word97Writer.AddSprm(array, num, 54802, 0, m_cellShading);
			if (m_cellShading2 != null)
			{
				num += Word97Writer.AddSprm(array, num, 54806, 0, m_cellShading2);
				if (m_cellShading3 != null)
				{
					num += Word97Writer.AddSprm(array, num, 54796, 0, m_cellShading3);
				}
			}
			return array;
		}

		internal void SetCellShading(int index, int ico24)
		{
			int num = index * 10;
			byte[] data = m_cellShading;
			if (index >= 22)
			{
				if (index >= 44)
				{
					num = (index - 44) * 10;
					data = m_cellShading3;
				}
				else
				{
					num = (index - 22) * 10;
					data = m_cellShading2;
				}
			}
			LittleEndian.PutInt(data, num + 4, ico24);
		}

		internal void Reset()
		{
			Array.Clear(m_cellShading, 0, m_cellShading.Length);
			if (m_cellShading2 != null)
			{
				Array.Clear(m_cellShading2, 0, m_cellShading2.Length);
				if (m_cellShading3 != null)
				{
					Array.Clear(m_cellShading3, 0, m_cellShading3.Length);
				}
			}
			InitShading();
		}

		private void InitShading()
		{
			if (m_tableShd == null)
			{
				for (int i = 3; i < m_cellShading.Length; i += 10)
				{
					m_cellShading[i] = byte.MaxValue;
					m_cellShading[i + 4] = byte.MaxValue;
				}
				if (m_cellShading2 != null)
				{
					for (int j = 3; j < m_cellShading2.Length; j += 10)
					{
						m_cellShading2[j] = byte.MaxValue;
						m_cellShading2[j + 4] = byte.MaxValue;
					}
				}
				if (m_cellShading3 != null)
				{
					for (int k = 3; k < m_cellShading3.Length; k += 10)
					{
						m_cellShading3[k] = byte.MaxValue;
						m_cellShading3[k + 4] = byte.MaxValue;
					}
				}
				return;
			}
			for (int l = 0; l < m_cellShading.Length; l += 10)
			{
				Array.Copy(m_tableShd, 0, m_cellShading, l, 10);
			}
			if (m_cellShading2 != null)
			{
				for (int m = 0; m < m_cellShading2.Length; m += 10)
				{
					Array.Copy(m_tableShd, 0, m_cellShading2, m, 10);
				}
			}
			if (m_cellShading3 != null)
			{
				for (int n = 0; n < m_cellShading3.Length; n += 10)
				{
					Array.Copy(m_tableShd, 0, m_cellShading3, n, 10);
				}
			}
		}
	}
}

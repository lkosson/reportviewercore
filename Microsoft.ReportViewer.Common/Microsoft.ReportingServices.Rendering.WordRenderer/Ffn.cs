using Microsoft.ReportingServices.Rendering.Utilities;
using System;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class Ffn
	{
		private int m_cbFfnM1;

		private byte m_info;

		private ushort m_wWeight;

		private byte m_chs;

		private byte m_ixchSzAlt;

		private byte[] m_panose = new byte[10];

		private byte[] m_fontSig = new byte[24];

		private char[] m_xszFfn;

		private int m_xszFfnLength;

		internal Ffn(int size, byte info, short wWeight, byte chs, byte ixchSzAlt, byte[] panose, byte[] fontSig, char[] xszFfn)
		{
			m_cbFfnM1 = size;
			m_info = info;
			m_wWeight = (ushort)wWeight;
			m_chs = chs;
			m_ixchSzAlt = ixchSzAlt;
			m_panose = panose;
			m_fontSig = fontSig;
			m_xszFfn = xszFfn;
			m_xszFfnLength = m_xszFfn.Length;
		}

		internal virtual byte[] toByteArray()
		{
			int num = 0;
			byte[] array = new byte[m_cbFfnM1 + 1];
			array[num] = (byte)m_cbFfnM1;
			num++;
			array[num] = m_info;
			num++;
			LittleEndian.PutUShort(array, num, m_wWeight);
			num += 2;
			array[num] = m_chs;
			num++;
			array[num] = m_ixchSzAlt;
			num++;
			Array.Copy(m_panose, 0, array, num, m_panose.Length);
			num += m_panose.Length;
			Array.Copy(m_fontSig, 0, array, num, m_fontSig.Length);
			num += m_fontSig.Length;
			for (int i = 0; i < m_xszFfn.Length; i++)
			{
				LittleEndian.PutUShort(array, num, m_xszFfn[i]);
				num += 2;
			}
			return array;
		}
	}
}

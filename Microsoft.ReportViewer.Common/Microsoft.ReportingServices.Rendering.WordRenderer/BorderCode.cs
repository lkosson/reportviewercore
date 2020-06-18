using Microsoft.ReportingServices.Rendering.Utilities;
using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class BorderCode
	{
		private int m_ico24;

		private int m_info;

		private static readonly BitField m_dptLineWidth = new BitField(255);

		private static readonly BitField m_brcType = new BitField(65280);

		private static readonly BitField m_dptSpace = new BitField(2031616);

		private static readonly BitField m_fShadow = new BitField(2097152);

		private static readonly BitField m_fFrame = new BitField(4194304);

		private byte m_97dptLineWidth;

		private byte m_97brcType;

		private ushort m_97info2;

		private static readonly BitField m_97dptSpace = new BitField(7936);

		private static readonly BitField m_97fShadow = new BitField(8192);

		private static readonly BitField _97fFrame = new BitField(16384);

		internal LineStyle Style
		{
			get
			{
				return (LineStyle)m_brcType.GetValue(m_info);
			}
			set
			{
				m_info = m_brcType.SetValue(m_info, (int)value);
				m_97brcType = (byte)value;
			}
		}

		internal int LineWidth
		{
			get
			{
				return m_dptLineWidth.GetValue(m_info);
			}
			set
			{
				m_info = m_dptLineWidth.SetValue(m_info, value);
				m_97dptLineWidth = (byte)value;
			}
		}

		internal bool HasShadow
		{
			set
			{
				m_info = m_fShadow.SetBoolean(m_info, value);
				m_97info2 = (ushort)m_97fShadow.SetBoolean(m_97info2, value);
			}
		}

		internal int Size => 8;

		internal int Ico24
		{
			get
			{
				return m_ico24;
			}
			set
			{
				m_ico24 = value;
			}
		}

		internal bool Empty
		{
			get
			{
				if (LineWidth != 0)
				{
					return Style == LineStyle.None;
				}
				return true;
			}
		}

		internal int Ico97 => 1;

		internal BorderCode()
		{
			m_ico24 = -16777216;
		}

		internal void SetColor(int ico24)
		{
			m_ico24 = ico24;
		}

		internal Color GetColor()
		{
			return WordColor.getColor(m_ico24);
		}

		internal void SetColor(ref Color color)
		{
			m_ico24 = WordColor.GetIco24(color);
		}

		internal void Serialize2K3(byte[] buf, int offset)
		{
			LittleEndian.PutInt(buf, offset, m_ico24);
			int val = m_info;
			if (Style == LineStyle.Double)
			{
				val = m_dptLineWidth.SetValue(m_info, m_dptLineWidth.GetValue(m_info) / 2);
			}
			LittleEndian.PutInt(buf, offset + 4, val);
		}

		internal void Serialize97(byte[] buf, int offset)
		{
			buf[offset + 1] = m_97brcType;
			if (m_97brcType == 3)
			{
				buf[offset] = (byte)((int)m_97dptLineWidth / 2);
			}
			else
			{
				buf[offset] = m_97dptLineWidth;
			}
			LittleEndian.PutUShort(buf, offset + 2, m_97info2);
		}

		internal virtual byte[] toByteArray()
		{
			byte[] array = new byte[8];
			Serialize2K3(array, 0);
			return array;
		}
	}
}

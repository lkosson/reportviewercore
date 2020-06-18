using Microsoft.ReportingServices.Rendering.Utilities;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class CellSpacingStruct
	{
		internal enum Location
		{
			Top = 1,
			Left = 2,
			Bottom = 4,
			Right = 8,
			All = 0xF
		}

		private byte m_itcFirst;

		private byte m_itcLim;

		private byte m_grfbrc;

		private byte m_ftsWidth;

		private ushort m_wWidth;

		internal int ItcFirst
		{
			set
			{
				m_itcFirst = (byte)value;
			}
		}

		internal int ItcLim
		{
			set
			{
				m_itcLim = (byte)value;
			}
		}

		internal int GrfBrc
		{
			set
			{
				m_grfbrc = (byte)value;
			}
		}

		internal int FtsWidth
		{
			set
			{
				m_ftsWidth = (byte)value;
			}
		}

		internal int Width
		{
			get
			{
				return m_wWidth;
			}
			set
			{
				m_wWidth = (ushort)value;
			}
		}

		internal bool Empty => m_wWidth == 0;

		internal CellSpacingStruct(Location location)
		{
			m_grfbrc = (byte)location;
			m_ftsWidth = 3;
		}

		internal void serialize(byte[] buf, int offset)
		{
			buf[offset] = m_itcFirst;
			buf[offset + 1] = m_itcLim;
			buf[offset + 2] = m_grfbrc;
			buf[offset + 3] = m_ftsWidth;
			LittleEndian.PutUShort(buf, offset + 4, m_wWidth);
		}

		internal byte[] ToByteArray()
		{
			byte[] array = new byte[9];
			LittleEndian.PutUShort(array, 54834);
			array[2] = 6;
			serialize(array, 3);
			return array;
		}
	}
}

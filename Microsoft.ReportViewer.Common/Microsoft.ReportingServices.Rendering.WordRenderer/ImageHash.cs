using Microsoft.ReportingServices.Rendering.RPLProcessing;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class ImageHash
	{
		private byte[] m_md4;

		private RPLFormat.Sizings m_sizing;

		private int m_width;

		private int m_height;

		internal ImageHash(byte[] md4, RPLFormat.Sizings sizing, int width, int height)
		{
			m_md4 = md4;
			m_sizing = sizing;
			m_width = width;
			m_height = height;
		}

		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < m_md4.Length; i++)
			{
				num += m_md4[i];
			}
			num += m_width;
			num += m_height;
			return num + (int)m_sizing;
		}

		public override bool Equals(object obj)
		{
			ImageHash imageHash = (ImageHash)obj;
			if (m_sizing != imageHash.m_sizing || m_width != imageHash.m_width || m_height != imageHash.m_height || m_md4.Length != imageHash.m_md4.Length)
			{
				return false;
			}
			for (int i = 0; i < m_md4.Length; i++)
			{
				if (m_md4[i] != imageHash.m_md4[i])
				{
					return false;
				}
			}
			return true;
		}
	}
}

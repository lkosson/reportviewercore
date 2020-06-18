using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class BIFF8Color : IColor
	{
		private Color m_color;

		private int m_paletteIndex;

		internal int PaletteIndex => m_paletteIndex;

		public byte Red => m_color.R;

		public byte Green => m_color.G;

		public byte Blue => m_color.B;

		internal BIFF8Color()
		{
		}

		internal BIFF8Color(Color color, int paletteIndex)
		{
			m_color = color;
			m_paletteIndex = paletteIndex;
		}

		public override int GetHashCode()
		{
			return m_color.GetHashCode();
		}

		public override bool Equals(object val)
		{
			if (val is BIFF8Color)
			{
				return m_color.Equals(((BIFF8Color)val).m_color);
			}
			return m_color.Equals(val);
		}
	}
}

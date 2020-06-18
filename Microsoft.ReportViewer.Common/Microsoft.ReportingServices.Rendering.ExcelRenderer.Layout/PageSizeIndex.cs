using System;
using System.Globalization;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Layout
{
	internal static class PageSizeIndex
	{
		private static int[] m_mmSizeIndex = new int[15]
		{
			8,
			9,
			11,
			12,
			15,
			27,
			28,
			29,
			30,
			31,
			32,
			33,
			34,
			35,
			36
		};

		private static int[] m_mmWidth = new int[15]
		{
			297,
			210,
			148,
			250,
			215,
			110,
			162,
			324,
			229,
			114,
			114,
			250,
			176,
			176,
			110
		};

		private static int[] m_mmHeight = new int[15]
		{
			420,
			297,
			210,
			354,
			275,
			220,
			229,
			458,
			324,
			162,
			229,
			353,
			250,
			125,
			230
		};

		private static int[] m_inchSizeIndex = new int[19]
		{
			1,
			3,
			4,
			5,
			6,
			7,
			14,
			16,
			19,
			20,
			21,
			22,
			23,
			24,
			25,
			26,
			37,
			38,
			39
		};

		private static float[] m_inchWidth = new float[19]
		{
			8.5f,
			11f,
			17f,
			8.5f,
			5.5f,
			7.5f,
			8.5f,
			10f,
			3.875f,
			4.125f,
			4.5f,
			4.5f,
			5f,
			11f,
			22f,
			34f,
			3.875f,
			3.625f,
			14.875f
		};

		private static float[] m_inchHeight = new float[19]
		{
			11f,
			17f,
			11f,
			14f,
			8.5f,
			10.5f,
			13f,
			14f,
			8.875f,
			9.5f,
			10.375f,
			11f,
			11.5f,
			22f,
			34f,
			44f,
			7.5f,
			6.5f,
			11f
		};

		internal static int GetPageSizeIndex(float pageWidth, float pageHeight, out bool isPortrait)
		{
			int num = (int)Math.Round(pageWidth);
			int num2 = (int)Math.Round(pageHeight);
			for (int i = 0; i < m_mmWidth.Length; i++)
			{
				if (num2 == m_mmHeight[i] && num == m_mmWidth[i])
				{
					isPortrait = true;
					return m_mmSizeIndex[i];
				}
			}
			for (int j = 0; j < m_mmWidth.Length; j++)
			{
				if (num2 == m_mmWidth[j] && num == m_mmHeight[j])
				{
					isPortrait = false;
					return m_mmSizeIndex[j];
				}
			}
			float num3 = (float)Math.Round(LayoutConvert.ToInches(pageHeight.ToString(CultureInfo.InvariantCulture) + "mm"), 1);
			float num4 = (float)Math.Round(LayoutConvert.ToInches(pageWidth.ToString(CultureInfo.InvariantCulture) + "mm"), 1);
			for (int k = 0; k < m_inchWidth.Length; k++)
			{
				if (num3 == m_inchHeight[k] && num4 == m_inchWidth[k])
				{
					isPortrait = true;
					return m_inchSizeIndex[k];
				}
			}
			for (int l = 0; l < m_inchWidth.Length; l++)
			{
				if (num4 == m_inchHeight[l] && num3 == m_inchWidth[l])
				{
					isPortrait = false;
					return m_inchSizeIndex[l];
				}
			}
			if (pageHeight >= pageWidth)
			{
				isPortrait = true;
			}
			else
			{
				isPortrait = false;
			}
			return 0;
		}
	}
}

using System;
using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class WordColor
	{
		private static readonly Color darkYellow = Color.FromArgb(32896);

		internal int _ico;

		internal int _ico24;

		internal WordColor(int ico)
		{
			_ico = ico;
		}

		internal WordColor(ref Color color)
		{
			_ico = getIco97(ref color);
		}

		internal virtual Color getColor()
		{
			return get97Color(_ico);
		}

		internal static Color get97Color(int ico97)
		{
			switch (ico97)
			{
			case 0:
			case 1:
				return Color.Black;
			case 2:
				return Color.Blue;
			case 3:
				return Color.Cyan;
			case 4:
				return Color.Green;
			case 5:
				return Color.Magenta;
			case 6:
				return Color.Red;
			case 7:
				return Color.Yellow;
			case 8:
				return Color.White;
			case 9:
				return Color.DarkBlue;
			case 10:
				return Color.DarkCyan;
			case 11:
				return Color.DarkGreen;
			case 12:
				return Color.DarkMagenta;
			case 13:
				return Color.DarkRed;
			case 14:
				return Color.DarkGoldenrod;
			case 15:
				return Color.DarkGray;
			case 16:
				return Color.LightGray;
			default:
				return Color.Empty;
			}
		}

		internal static Color getColor(int ico24)
		{
			return Color.FromArgb(transposeIco(ico24));
		}

		internal static int GetIco24(Color color)
		{
			return (color.B << 16) | (color.G << 8) | color.R;
		}

		internal static int getIco97(ref Color color)
		{
			float[] hSBModel = getHSBModel(ref color);
			float num = 2.14748365E+09f;
			int result = 0;
			for (int i = 1; i < 17; i++)
			{
				Color color2 = get97Color(i);
				float[] hSBModel2 = getHSBModel(ref color2);
				float num2 = Math.Abs(hSBModel[0] - hSBModel2[0]);
				num2 *= 6f;
				num2 += Math.Abs(hSBModel[1] - hSBModel2[1]);
				num2 += Math.Abs(hSBModel[2] - hSBModel2[2]);
				if (num2 < num)
				{
					num = num2;
					result = i;
				}
			}
			return result;
		}

		private static float[] getHSBModel(ref Color color)
		{
			int r = color.R;
			int g = color.G;
			int b = color.B;
			float[] array = new float[3];
			int num = (r <= g) ? g : r;
			if (b > num)
			{
				num = b;
			}
			int num2 = (r >= g) ? g : r;
			if (b < num2)
			{
				num2 = b;
			}
			float num3 = (float)num / 255f;
			float num4 = (num == 0) ? 0f : ((float)(num - num2) / (float)num);
			float num5;
			if (num4 == 0f)
			{
				num5 = 0f;
			}
			else
			{
				float num6 = (float)(num - r) / (float)(num - num2);
				float num7 = (float)(num - g) / (float)(num - num2);
				float num8 = (float)(num - b) / (float)(num - num2);
				num5 = ((r == num) ? (num8 - num7) : ((g != num) ? (4f + num7 - num6) : (2f + num6 - num8)));
				num5 /= 6f;
				if (num5 < 0f)
				{
					num5 += 1f;
				}
			}
			array[0] = num5;
			array[1] = num4;
			array[2] = num3;
			return array;
		}

		internal static int transposeIco(int ico)
		{
			return ((ico & 0xFF) << 16) + (ico & 0xFF00) + ((ico & 0xFF0000) >> 16);
		}
	}
}

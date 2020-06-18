using System;
using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal struct HLSColor
	{
		private const int ShadowAdj = -333;

		private const int HilightAdj = 500;

		private const int WatermarkAdj = -50;

		private const int Range = 240;

		private const int HLSMax = 240;

		private const int RGBMax = 255;

		private const int Undefined = 160;

		private int hue;

		private int saturation;

		private int luminosity;

		public HLSColor(int red, int green, int blue)
		{
			int num = Math.Max(Math.Max(red, green), blue);
			int num2 = Math.Min(Math.Min(red, green), blue);
			int num3 = num + num2;
			luminosity = (num3 * 240 + 255) / 510;
			int num4 = num - num2;
			if (num4 == 0)
			{
				saturation = 0;
				hue = 160;
				return;
			}
			if (luminosity <= 120)
			{
				saturation = (num4 * 240 + num3 / 2) / num3;
			}
			else
			{
				saturation = (num4 * 240 + (510 - num3) / 2) / (510 - num3);
			}
			int num5 = ((num - red) * 40 + num4 / 2) / num4;
			int num6 = ((num - green) * 40 + num4 / 2) / num4;
			int num7 = ((num - blue) * 40 + num4 / 2) / num4;
			if (red == num)
			{
				hue = num7 - num6;
			}
			else if (green == num)
			{
				hue = 80 + num5 - num7;
			}
			else
			{
				hue = 160 + num6 - num5;
			}
			if (hue < 0)
			{
				hue += 240;
			}
			if (hue > 240)
			{
				hue -= 240;
			}
		}

		public Color Lighten(float percLighter)
		{
			int num = luminosity;
			int num2 = NewLuma(500, scale: true);
			return ColorFromHLS(hue, num + (int)((float)(num2 - num) * percLighter), saturation);
		}

		public override bool Equals(object o)
		{
			if (o is HLSColor)
			{
				HLSColor hLSColor = (HLSColor)o;
				if (hue == hLSColor.hue && saturation == hLSColor.saturation && luminosity == hLSColor.luminosity)
				{
					return true;
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (hue << 6) | (saturation << 2) | luminosity;
		}

		public static bool operator ==(HLSColor a, HLSColor b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(HLSColor a, HLSColor b)
		{
			return !a.Equals(b);
		}

		private Color ColorFromHLS(int hue, int luminosity, int saturation)
		{
			byte red;
			byte green;
			byte blue;
			if (saturation == 0)
			{
				red = (green = (blue = (byte)(luminosity * 255 / 240)));
				if (hue != 160)
				{
				}
			}
			else
			{
				int num = (luminosity > 120) ? (luminosity + saturation - (luminosity * saturation + 120) / 240) : ((luminosity * (240 + saturation) + 120) / 240);
				int n = 2 * luminosity - num;
				red = (byte)((HueToRGB(n, num, hue + 80) * 255 + 120) / 240);
				green = (byte)((HueToRGB(n, num, hue) * 255 + 120) / 240);
				blue = (byte)((HueToRGB(n, num, hue - 80) * 255 + 120) / 240);
			}
			return Color.FromArgb(red, green, blue);
		}

		private int HueToRGB(int n1, int n2, int hue)
		{
			if (hue < 0)
			{
				hue += 240;
			}
			if (hue > 240)
			{
				hue -= 240;
			}
			if (hue < 40)
			{
				return n1 + ((n2 - n1) * hue + 20) / 40;
			}
			if (hue < 120)
			{
				return n2;
			}
			if (hue < 160)
			{
				return n1 + ((n2 - n1) * (160 - hue) + 20) / 40;
			}
			return n1;
		}

		private int NewLuma(int n, bool scale)
		{
			return NewLuma(luminosity, n, scale);
		}

		private int NewLuma(int luminosity, int n, bool scale)
		{
			if (n == 0)
			{
				return luminosity;
			}
			if (scale)
			{
				if (n > 0)
				{
					return (int)((long)(luminosity * (1000 - n) + 241 * n) / 1000L);
				}
				return luminosity * (n + 1000) / 1000;
			}
			int num = luminosity;
			num += (int)((long)(n * 240) / 1000L);
			if (num < 0)
			{
				num = 0;
			}
			if (num > 240)
			{
				num = 240;
			}
			return num;
		}
	}
}

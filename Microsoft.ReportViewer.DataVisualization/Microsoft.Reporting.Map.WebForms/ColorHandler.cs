using System;
using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class ColorHandler
	{
		internal static RGB HSVtoRGB(int H, int S, int V)
		{
			return HSVtoRGB(new HSV(H, S, V));
		}

		internal static Color HSVtoColor(HSV hsv)
		{
			RGB rGB = HSVtoRGB(hsv);
			return Color.FromArgb(rGB.Red, rGB.Green, rGB.Blue);
		}

		internal static Color HSVtoColor(int H, int S, int V)
		{
			return HSVtoColor(new HSV(H, S, V));
		}

		internal static HSV ColorToHSV(Color color)
		{
			return RGBtoHSV(new RGB(color.R, color.G, color.B));
		}

		internal static RGB HSVtoRGB(HSV HSV)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = (double)HSV.Hue / 255.0 * 360.0 % 360.0;
			double num5 = (double)HSV.Saturation / 255.0;
			double num6 = (double)HSV.value / 255.0;
			if (num5 == 0.0)
			{
				num = num6;
				num2 = num6;
				num3 = num6;
			}
			else
			{
				double num7 = num4 / 60.0;
				int num8 = (int)Math.Floor(num7);
				double num9 = num7 - (double)num8;
				double num10 = num6 * (1.0 - num5);
				double num11 = num6 * (1.0 - num5 * num9);
				double num12 = num6 * (1.0 - num5 * (1.0 - num9));
				switch (num8)
				{
				case 0:
					num = num6;
					num2 = num12;
					num3 = num10;
					break;
				case 1:
					num = num11;
					num2 = num6;
					num3 = num10;
					break;
				case 2:
					num = num10;
					num2 = num6;
					num3 = num12;
					break;
				case 3:
					num = num10;
					num2 = num11;
					num3 = num6;
					break;
				case 4:
					num = num12;
					num2 = num10;
					num3 = num6;
					break;
				case 5:
					num = num6;
					num2 = num10;
					num3 = num11;
					break;
				}
			}
			return new RGB((int)(num * 255.0), (int)(num2 * 255.0), (int)(num3 * 255.0));
		}

		internal static HSV RGBtoHSV(RGB RGB)
		{
			double num = (double)RGB.Red / 255.0;
			double num2 = (double)RGB.Green / 255.0;
			double num3 = (double)RGB.Blue / 255.0;
			double num4 = Math.Min(Math.Min(num, num2), num3);
			double num5 = Math.Max(Math.Max(num, num2), num3);
			double num6 = num5;
			double num7 = num5 - num4;
			double num8;
			double num9;
			if (num5 == 0.0 || num7 == 0.0)
			{
				num8 = 0.0;
				num9 = 0.0;
			}
			else
			{
				num8 = num7 / num5;
				num9 = ((num == num5) ? ((num2 - num3) / num7) : ((num2 != num5) ? (4.0 + (num - num2) / num7) : (2.0 + (num3 - num) / num7)));
			}
			num9 *= 60.0;
			if (num9 < 0.0)
			{
				num9 += 360.0;
			}
			return new HSV((int)(num9 / 360.0 * 255.0), (int)(num8 * 255.0), (int)(num6 * 255.0));
		}
	}
}

using System;
using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class ColorGenerator
	{
		private static Color[] colorsDundas = new Color[15]
		{
			Color.FromArgb(65, 140, 240),
			Color.FromArgb(252, 180, 65),
			Color.FromArgb(224, 64, 10),
			Color.FromArgb(5, 100, 146),
			Color.FromArgb(191, 191, 191),
			Color.FromArgb(26, 59, 105),
			Color.FromArgb(255, 227, 130),
			Color.FromArgb(18, 156, 221),
			Color.FromArgb(202, 107, 75),
			Color.FromArgb(0, 92, 219),
			Color.FromArgb(243, 210, 136),
			Color.FromArgb(80, 99, 129),
			Color.FromArgb(241, 185, 168),
			Color.FromArgb(224, 131, 10),
			Color.FromArgb(120, 147, 190)
		};

		private static Color[] colorsPacific = new Color[10]
		{
			Color.FromArgb(1, 184, 170),
			Color.FromArgb(55, 70, 73),
			Color.FromArgb(253, 98, 94),
			Color.FromArgb(242, 200, 15),
			Color.FromArgb(95, 107, 109),
			Color.FromArgb(138, 212, 235),
			Color.FromArgb(254, 150, 102),
			Color.FromArgb(166, 105, 153),
			Color.FromArgb(53, 153, 184),
			Color.FromArgb(223, 191, 191)
		};

		internal Color[] GenerateColors(MapColorPalette palette, int colorCount)
		{
			Color[] array = new Color[colorCount];
			if (palette == MapColorPalette.Random)
			{
				Random random = new Random(465804847);
				for (int i = 0; i < colorCount; i++)
				{
					Color color = Color.FromArgb(random.Next(100, 256), random.Next(100, 256), random.Next(100, 256));
					if (CheckDistance(color, array, i, 50 / colorCount))
					{
						array[i] = color;
					}
					else
					{
						i--;
					}
				}
			}
			switch (palette)
			{
			case MapColorPalette.Light:
			{
				Random random5 = new Random(265804847);
				for (int m = 0; m < colorCount; m++)
				{
					Color color5 = Color.FromArgb(random5.Next(160, 256), random5.Next(160, 256), random5.Next(160, 256));
					if (CheckDistance(color5, array, m, 15 / colorCount))
					{
						array[m] = color5;
					}
					else
					{
						m--;
					}
				}
				break;
			}
			case MapColorPalette.SemiTransparent:
			{
				Random random3 = new Random(465889847);
				for (int k = 0; k < colorCount; k++)
				{
					Color color3 = Color.FromArgb(128, random3.Next(256), random3.Next(256), random3.Next(256));
					if (CheckDistance(color3, array, k, 50 / colorCount))
					{
						array[k] = color3;
					}
					else
					{
						k--;
					}
				}
				break;
			}
			case MapColorPalette.Dundas:
			{
				Random random4 = new Random(965889847);
				for (int l = 0; l < colorCount; l++)
				{
					if (l < colorsDundas.Length)
					{
						array[l] = colorsDundas[l];
						continue;
					}
					Color color4 = Color.FromArgb(random4.Next(256), random4.Next(64, 256), random4.Next(64, 256));
					if (CheckDistance(color4, array, l, 50 / colorCount))
					{
						array[l] = color4;
					}
					else
					{
						l--;
					}
				}
				break;
			}
			case MapColorPalette.Pacific:
			{
				Random random2 = new Random(539136961);
				for (int j = 0; j < colorCount; j++)
				{
					if (j < colorsPacific.Length)
					{
						array[j] = colorsPacific[j];
						continue;
					}
					Color color2 = Color.FromArgb(random2.Next(256), random2.Next(64, 256), random2.Next(64, 256));
					if (CheckDistance(color2, array, j, 50 / colorCount))
					{
						array[j] = color2;
					}
					else
					{
						j--;
					}
				}
				break;
			}
			}
			return array;
		}

		private bool CheckDistance(Color color, Color[] colors, int index, int minimumDistance)
		{
			if (minimumDistance < 1)
			{
				return true;
			}
			int num = minimumDistance * minimumDistance * minimumDistance;
			for (int i = 0; i < index; i++)
			{
				int num2 = color.R - colors[i].R;
				int num3 = color.G - colors[i].G;
				int num4 = color.B - colors[i].B;
				if (num2 * num2 + num3 * num3 + num4 * num4 < num)
				{
					return false;
				}
			}
			return true;
		}
	}
}

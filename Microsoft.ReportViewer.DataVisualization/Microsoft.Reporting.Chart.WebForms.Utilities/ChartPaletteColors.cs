using System;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms.Utilities
{
	internal static class ChartPaletteColors
	{
		private static Color[] colorsGrayScale;

		private static Color[] colorsDefault;

		private static Color[] colorsPastel;

		private static Color[] colorsEarth;

		private static Color[] colorsSemiTransparent;

		private static Color[] colorsLight;

		private static Color[] colorsExcel;

		private static Color[] colorsBerry;

		private static Color[] colorsChocolate;

		private static Color[] colorsFire;

		private static Color[] colorsSeaGreen;

		private static Color[] colorsDundas;

		private static Color[] colorsPacific;

		private static Color[] colorsPacificLight;

		private static Color[] colorsPacificSemitransparent;

		static ChartPaletteColors()
		{
			colorsGrayScale = null;
			colorsDefault = new Color[16]
			{
				Color.Green,
				Color.Blue,
				Color.Purple,
				Color.Lime,
				Color.Fuchsia,
				Color.Teal,
				Color.Yellow,
				Color.Gray,
				Color.Aqua,
				Color.Navy,
				Color.Maroon,
				Color.Red,
				Color.Olive,
				Color.Silver,
				Color.Tomato,
				Color.Moccasin
			};
			colorsPastel = new Color[16]
			{
				Color.SkyBlue,
				Color.LimeGreen,
				Color.MediumOrchid,
				Color.LightCoral,
				Color.SteelBlue,
				Color.YellowGreen,
				Color.Turquoise,
				Color.HotPink,
				Color.Khaki,
				Color.Tan,
				Color.DarkSeaGreen,
				Color.CornflowerBlue,
				Color.Plum,
				Color.CadetBlue,
				Color.PeachPuff,
				Color.LightSalmon
			};
			colorsEarth = new Color[16]
			{
				Color.FromArgb(255, 128, 0),
				Color.DarkGoldenrod,
				Color.FromArgb(192, 64, 0),
				Color.OliveDrab,
				Color.Peru,
				Color.FromArgb(192, 192, 0),
				Color.ForestGreen,
				Color.Chocolate,
				Color.Olive,
				Color.LightSeaGreen,
				Color.SandyBrown,
				Color.FromArgb(0, 192, 0),
				Color.DarkSeaGreen,
				Color.Firebrick,
				Color.SaddleBrown,
				Color.FromArgb(192, 0, 0)
			};
			colorsSemiTransparent = new Color[16]
			{
				Color.FromArgb(150, 255, 0, 0),
				Color.FromArgb(150, 0, 255, 0),
				Color.FromArgb(150, 0, 0, 255),
				Color.FromArgb(150, 255, 255, 0),
				Color.FromArgb(150, 0, 255, 255),
				Color.FromArgb(150, 255, 0, 255),
				Color.FromArgb(150, 170, 120, 20),
				Color.FromArgb(80, 255, 0, 0),
				Color.FromArgb(80, 0, 255, 0),
				Color.FromArgb(80, 0, 0, 255),
				Color.FromArgb(80, 255, 255, 0),
				Color.FromArgb(80, 0, 255, 255),
				Color.FromArgb(80, 255, 0, 255),
				Color.FromArgb(80, 170, 120, 20),
				Color.FromArgb(150, 100, 120, 50),
				Color.FromArgb(150, 40, 90, 150)
			};
			colorsLight = new Color[10]
			{
				Color.Lavender,
				Color.LavenderBlush,
				Color.PeachPuff,
				Color.LemonChiffon,
				Color.MistyRose,
				Color.Honeydew,
				Color.AliceBlue,
				Color.WhiteSmoke,
				Color.AntiqueWhite,
				Color.LightCyan
			};
			colorsExcel = new Color[16]
			{
				Color.FromArgb(153, 153, 255),
				Color.FromArgb(153, 51, 102),
				Color.FromArgb(255, 255, 204),
				Color.FromArgb(204, 255, 255),
				Color.FromArgb(102, 0, 102),
				Color.FromArgb(255, 128, 128),
				Color.FromArgb(0, 102, 204),
				Color.FromArgb(204, 204, 255),
				Color.FromArgb(0, 0, 128),
				Color.FromArgb(255, 0, 255),
				Color.FromArgb(255, 255, 0),
				Color.FromArgb(0, 255, 255),
				Color.FromArgb(128, 0, 128),
				Color.FromArgb(128, 0, 0),
				Color.FromArgb(0, 128, 128),
				Color.FromArgb(0, 0, 255)
			};
			colorsBerry = new Color[11]
			{
				Color.BlueViolet,
				Color.MediumOrchid,
				Color.RoyalBlue,
				Color.MediumVioletRed,
				Color.Blue,
				Color.BlueViolet,
				Color.Orchid,
				Color.MediumSlateBlue,
				Color.FromArgb(192, 0, 192),
				Color.MediumBlue,
				Color.Purple
			};
			colorsChocolate = new Color[10]
			{
				Color.Sienna,
				Color.Chocolate,
				Color.DarkRed,
				Color.Peru,
				Color.Brown,
				Color.SandyBrown,
				Color.SaddleBrown,
				Color.FromArgb(192, 64, 0),
				Color.Firebrick,
				Color.FromArgb(182, 92, 58)
			};
			colorsFire = new Color[10]
			{
				Color.Gold,
				Color.Red,
				Color.DeepPink,
				Color.Crimson,
				Color.DarkOrange,
				Color.Magenta,
				Color.Yellow,
				Color.OrangeRed,
				Color.MediumVioletRed,
				Color.FromArgb(221, 226, 33)
			};
			colorsSeaGreen = new Color[10]
			{
				Color.SeaGreen,
				Color.MediumAquamarine,
				Color.SteelBlue,
				Color.DarkCyan,
				Color.CadetBlue,
				Color.MediumSeaGreen,
				Color.MediumTurquoise,
				Color.LightSteelBlue,
				Color.DarkSeaGreen,
				Color.SkyBlue
			};
			colorsDundas = new Color[15]
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
			colorsPacific = new Color[20]
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
				Color.FromArgb(223, 191, 191),
				Color.FromArgb(74, 197, 187),
				Color.FromArgb(95, 107, 109),
				Color.FromArgb(251, 130, 129),
				Color.FromArgb(244, 210, 90),
				Color.FromArgb(127, 137, 138),
				Color.FromArgb(164, 221, 238),
				Color.FromArgb(253, 171, 137),
				Color.FromArgb(182, 135, 172),
				Color.FromArgb(40, 115, 138),
				Color.FromArgb(167, 143, 143)
			};
			colorsPacificLight = new Color[10]
			{
				Color.FromArgb(74, 197, 187),
				Color.FromArgb(95, 107, 109),
				Color.FromArgb(251, 130, 129),
				Color.FromArgb(244, 210, 90),
				Color.FromArgb(127, 137, 138),
				Color.FromArgb(164, 221, 238),
				Color.FromArgb(253, 171, 137),
				Color.FromArgb(182, 135, 172),
				Color.FromArgb(40, 115, 138),
				Color.FromArgb(167, 143, 143)
			};
			colorsPacificSemitransparent = new Color[20]
			{
				Color.FromArgb(153, 1, 184, 170),
				Color.FromArgb(153, 55, 70, 73),
				Color.FromArgb(153, 253, 98, 94),
				Color.FromArgb(153, 242, 200, 15),
				Color.FromArgb(153, 95, 107, 109),
				Color.FromArgb(153, 138, 212, 235),
				Color.FromArgb(153, 254, 150, 102),
				Color.FromArgb(153, 166, 105, 153),
				Color.FromArgb(153, 53, 153, 184),
				Color.FromArgb(153, 223, 191, 191),
				Color.FromArgb(153, 74, 197, 187),
				Color.FromArgb(153, 95, 107, 109),
				Color.FromArgb(153, 251, 130, 129),
				Color.FromArgb(153, 244, 210, 90),
				Color.FromArgb(153, 127, 137, 138),
				Color.FromArgb(153, 164, 221, 238),
				Color.FromArgb(153, 253, 171, 137),
				Color.FromArgb(153, 182, 135, 172),
				Color.FromArgb(153, 40, 115, 138),
				Color.FromArgb(153, 167, 143, 143)
			};
			colorsGrayScale = new Color[16];
			for (int i = 0; i < 16; i++)
			{
				int num = 200 - i * 11;
				colorsGrayScale[i] = Color.FromArgb(num, num, num);
			}
		}

		public static Color[] GetPaletteColors(ChartColorPalette palette)
		{
			switch (palette)
			{
			case ChartColorPalette.None:
				throw new ArgumentException(SR.ExceptionPaletteIsEmpty);
			case ChartColorPalette.Default:
				return colorsDefault;
			case ChartColorPalette.Grayscale:
				return colorsGrayScale;
			case ChartColorPalette.Excel:
				return colorsExcel;
			case ChartColorPalette.Pastel:
				return colorsPastel;
			case ChartColorPalette.Light:
				return colorsLight;
			case ChartColorPalette.EarthTones:
				return colorsEarth;
			case ChartColorPalette.Semitransparent:
				return colorsSemiTransparent;
			case ChartColorPalette.Berry:
				return colorsBerry;
			case ChartColorPalette.Chocolate:
				return colorsChocolate;
			case ChartColorPalette.Fire:
				return colorsFire;
			case ChartColorPalette.SeaGreen:
				return colorsSeaGreen;
			case ChartColorPalette.BrightPastel:
				return colorsDundas;
			case ChartColorPalette.Pacific:
				return colorsPacific;
			case ChartColorPalette.PacificLight:
				return colorsPacificLight;
			case ChartColorPalette.PacificSemiTransparent:
				return colorsPacificSemitransparent;
			default:
				return null;
			}
		}
	}
}

using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLPaletteModel : IPaletteModel
	{
		private string[] _legacyPalette;

		private bool _legacyPaletteModified;

		private readonly Palette _interface;

		private const int NUM_RESERVED_COLORS_IN_PALETTE = 8;

		public bool LegacyPaletteModified => _legacyPaletteModified;

		public Palette Interface => _interface;

		public XMLPaletteModel()
		{
			_interface = new Palette(this);
			_legacyPalette = new string[64]
			{
				"00000000",
				"00FFFFFF",
				"00FF0000",
				"0000FF00",
				"000000FF",
				"00FFFF00",
				"00FF00FF",
				"0000FFFF",
				"00000000",
				"00FFFFFF",
				"00FF0000",
				"0000FF00",
				"000000FF",
				"00FFFF00",
				"00FF00FF",
				"0000FFFF",
				"00800000",
				"00008000",
				"00000080",
				"00808000",
				"00800080",
				"00008080",
				"00C0C0C0",
				"00808080",
				"009999FF",
				"00993366",
				"00FFFFCC",
				"00CCFFFF",
				"00660066",
				"00FF8080",
				"000066CC",
				"00CCCCFF",
				"00000080",
				"00FF00FF",
				"00FFFF00",
				"0000FFFF",
				"00800080",
				"00800000",
				"00008080",
				"000000FF",
				"0000CCFF",
				"00CCFFFF",
				"00CCFFCC",
				"00FFFF99",
				"0099CCFF",
				"00FF99CC",
				"00CC99FF",
				"00FFCC99",
				"003366FF",
				"0033CCCC",
				"0099CC00",
				"00FFCC00",
				"00FF9900",
				"00FF6600",
				"00666699",
				"00969696",
				"00003366",
				"00339966",
				"00003300",
				"00333300",
				"00993300",
				"00993366",
				"00333399",
				"00333333"
			};
		}

		private void CheckValidIndex(int index)
		{
			if (index < 0 || index >= _legacyPalette.Length - 8)
			{
				throw new FatalException();
			}
		}

		public void WriteIndexedColors(CT_Stylesheet stylesheet)
		{
			if (stylesheet.Colors == null)
			{
				stylesheet.Colors = new CT_Colors();
			}
			if (stylesheet.Colors.IndexedColors == null)
			{
				stylesheet.Colors.IndexedColors = new CT_IndexedColors();
			}
			if (stylesheet.Colors.IndexedColors.RgbColor == null)
			{
				stylesheet.Colors.IndexedColors.RgbColor = new List<CT_RgbColor>();
			}
			else
			{
				stylesheet.Colors.IndexedColors.RgbColor.Clear();
			}
			string[] legacyPalette = _legacyPalette;
			foreach (string rgb_Attr in legacyPalette)
			{
				CT_RgbColor cT_RgbColor = new CT_RgbColor();
				cT_RgbColor.Rgb_Attr = rgb_Attr;
				stylesheet.Colors.IndexedColors.RgbColor.Add(cT_RgbColor);
			}
		}

		private string ColorString(int channel)
		{
			return channel.ToString("X", CultureInfo.InvariantCulture).PadLeft(2, '0');
		}

		private string ColorString(int red, int green, int blue)
		{
			return "00" + ColorString(red) + ColorString(green) + ColorString(blue);
		}

		public XMLColorModel GetColorFromCT(CT_Color color)
		{
			return new XMLColorModel(color, this);
		}

		public ColorModel getColor(int red, int green, int blue)
		{
			return new XMLColorModel(red, green, blue);
		}

		public void setColorAt(int index, int red, int green, int blue)
		{
			CheckValidIndex(index);
			_legacyPalette[index + 8] = ColorString(red, green, blue);
			_legacyPaletteModified = true;
		}

		public int GetColorIndex(int red, int green, int blue)
		{
			string value = ColorString(red, green, blue);
			for (int i = 8; i < _legacyPalette.Length; i++)
			{
				if (_legacyPalette[i].Equals(value))
				{
					return i - 8;
				}
			}
			return -1;
		}

		public ColorModel GetColorAt(int index)
		{
			CheckValidIndex(index);
			return new XMLColorModel(_legacyPalette[index + 8]);
		}
	}
}

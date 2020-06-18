using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using System.Globalization;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLColorModel : ColorModel
	{
		private const string Default = "FF000000";

		private readonly CT_Color _color;

		private readonly XMLPaletteModel _palette;

		public CT_Color Data => _color;

		public int Red
		{
			get
			{
				return GetValueAtByte(1);
			}
			set
			{
				SetValueAtByte(1, (byte)value);
			}
		}

		public int Green
		{
			get
			{
				return GetValueAtByte(2);
			}
			set
			{
				SetValueAtByte(2, (byte)value);
			}
		}

		public int Blue
		{
			get
			{
				return GetValueAtByte(3);
			}
			set
			{
				SetValueAtByte(3, (byte)value);
			}
		}

		public XMLColorModel(CT_Color color, XMLPaletteModel palette)
		{
			_palette = palette;
			if (color == null)
			{
				_color = new CT_Color();
				_color.Rgb_Attr = "FF000000";
			}
			else
			{
				_color = color;
			}
		}

		public XMLColorModel(int red, int green, int blue)
		{
			_color = new CT_Color();
			_color.Rgb_Attr = "FF000000";
			Red = red;
			Green = green;
			Blue = blue;
		}

		public XMLColorModel(string argb)
		{
			_color = new CT_Color();
			_color.Rgb_Attr = argb;
		}

		private int GetValueAtByte(int index)
		{
			return int.Parse(_color.Rgb_Attr.Substring(2 * index, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
		}

		private void SetValueAtByte(int index, byte value)
		{
			_color.Rgb_Attr = _color.Rgb_Attr.Substring(0, index * 2) + value.ToString("X", CultureInfo.InvariantCulture).PadLeft(2, '0') + _color.Rgb_Attr.Substring(index * 2 + 2);
		}

		public override int getRed()
		{
			return Red;
		}

		public override int getBlue()
		{
			return Blue;
		}

		public override int getGreen()
		{
			return Green;
		}

		public XMLColorModel Clone()
		{
			CT_Color cT_Color = new CT_Color();
			if (_color.Rgb_Attr != null)
			{
				cT_Color.Rgb_Attr = _color.Rgb_Attr;
			}
			if (_color.Indexed_Attr_Is_Specified)
			{
				cT_Color.Indexed_Attr = _color.Indexed_Attr;
			}
			if (_color.Theme_Attr_Is_Specified)
			{
				cT_Color.Theme_Attr = _color.Theme_Attr;
			}
			cT_Color.Tint_Attr = _color.Tint_Attr;
			cT_Color.Auto_Attr = _color.Auto_Attr;
			return new XMLColorModel(cT_Color, _palette);
		}

		public override bool Equals(object o)
		{
			if (!(o is XMLColorModel))
			{
				return false;
			}
			XMLColorModel xMLColorModel = (XMLColorModel)o;
			if (_color.Rgb_Attr != null && xMLColorModel._color.Rgb_Attr != null)
			{
				return _color.Rgb_Attr == xMLColorModel._color.Rgb_Attr;
			}
			if (_color.Rgb_Attr == null && xMLColorModel._color.Rgb_Attr == null)
			{
				if (_color.Indexed_Attr_Is_Specified ^ xMLColorModel._color.Indexed_Attr_Is_Specified)
				{
					return false;
				}
				if (_color.Indexed_Attr_Is_Specified && _color.Indexed_Attr != xMLColorModel._color.Indexed_Attr)
				{
					return false;
				}
				if (_color.Theme_Attr_Is_Specified ^ xMLColorModel._color.Theme_Attr_Is_Specified)
				{
					return false;
				}
				if (_color.Theme_Attr_Is_Specified && _color.Theme_Attr != xMLColorModel._color.Theme_Attr)
				{
					return false;
				}
				if (_color.Tint_Attr == xMLColorModel._color.Tint_Attr)
				{
					return _color.Auto_Attr == xMLColorModel._color.Auto_Attr;
				}
				return false;
			}
			return false;
		}

		public override int GetHashCode()
		{
			if (_color.Rgb_Attr != null)
			{
				return _color.Rgb_Attr.GetHashCode();
			}
			return (((((_color.Auto_Attr.GetHashCode() * 397) ^ _color.Tint_Attr.GetHashCode()) * 397) ^ _color.Indexed_Attr.GetHashCode()) * 397) ^ _color.Theme_Attr.GetHashCode();
		}
	}
}

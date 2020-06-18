using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.sharedTypes;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLFontModel : IFontModel, ICloneable, IDeepCloneable<XMLFontModel>
	{
		private readonly CT_Font _font;

		private readonly Font _interface;

		private readonly XMLPaletteModel _palette;

		private bool _hasBeenModified;

		public bool HasBeenModified => _hasBeenModified;

		public Font Interface => _interface;

		public CT_Font Data => _font;

		public bool Bold
		{
			get
			{
				if (_font.B == null)
				{
					_font.B = new CT_BooleanProperty();
					_font.B.Val_Attr = false;
				}
				return _font.B.Val_Attr;
			}
			set
			{
				if (_font.B == null)
				{
					_font.B = new CT_BooleanProperty();
				}
				_font.B.Val_Attr = value;
				_hasBeenModified = true;
			}
		}

		public bool Italic
		{
			get
			{
				if (_font.I == null)
				{
					_font.I = new CT_BooleanProperty();
					_font.I.Val_Attr = false;
				}
				return _font.I.Val_Attr;
			}
			set
			{
				if (_font.I == null)
				{
					_font.I = new CT_BooleanProperty();
				}
				_font.I.Val_Attr = value;
				_hasBeenModified = true;
			}
		}

		public bool Strikethrough
		{
			get
			{
				if (_font.Strike == null)
				{
					_font.Strike = new CT_BooleanProperty();
					_font.Strike.Val_Attr = false;
				}
				return _font.Strike.Val_Attr;
			}
			set
			{
				if (_font.Strike == null)
				{
					_font.Strike = new CT_BooleanProperty();
				}
				_font.Strike.Val_Attr = value;
				_hasBeenModified = true;
			}
		}

		public string Name
		{
			get
			{
				if (_font.Name == null)
				{
					_font.Name = new CT_FontName();
					_font.Name.Val_Attr = "Calibri";
				}
				return _font.Name.Val_Attr;
			}
			set
			{
				if (_font.Name == null)
				{
					_font.Name = new CT_FontName();
				}
				_font.Name.Val_Attr = ExcelGeneratorStringUtil.Truncate(value, 31);
				_hasBeenModified = true;
			}
		}

		public double Size
		{
			get
			{
				if (_font.Sz == null)
				{
					_font.Sz = new CT_FontSize();
					_font.Sz.Val_Attr = 11.0;
				}
				return _font.Sz.Val_Attr;
			}
			set
			{
				if (value < 1.0 || value > 409.55)
				{
					throw new FatalException();
				}
				if (_font.Sz == null)
				{
					_font.Sz = new CT_FontSize();
				}
				_font.Sz.Val_Attr = value;
				_hasBeenModified = true;
			}
		}

		public ST_UnderlineValues Underline
		{
			get
			{
				if (_font.U == null)
				{
					return null;
				}
				return _font.U.Val_Attr;
			}
			set
			{
				if (value == null)
				{
					_font.U = null;
				}
				else
				{
					if (_font.U == null)
					{
						_font.U = new CT_UnderlineProperty();
					}
					_font.U.Val_Attr = value;
				}
				_hasBeenModified = true;
			}
		}

		public ST_VerticalAlignRun ScriptStyle
		{
			get
			{
				if (_font.VertAlign == null)
				{
					return null;
				}
				return _font.VertAlign.Val_Attr;
			}
			set
			{
				if (value == null)
				{
					_font.VertAlign = null;
				}
				else
				{
					if (_font.VertAlign == null)
					{
						_font.VertAlign = new CT_VerticalAlignFontProperty();
					}
					_font.VertAlign.Val_Attr = value;
				}
				_hasBeenModified = true;
			}
		}

		public ColorModel Color
		{
			get
			{
				if (_font.Color == null)
				{
					_font.Color = new CT_Color();
					_font.Color.Theme_Attr = 1u;
				}
				return _palette.GetColorFromCT(_font.Color);
			}
			set
			{
				_font.Color = ((XMLColorModel)value).Data;
				_hasBeenModified = true;
			}
		}

		public XMLFontModel(CT_Font font, XMLPaletteModel palette)
		{
			_font = font;
			_interface = new Font(this);
			_palette = palette;
		}

		public XMLFontModel(XMLPaletteModel palette)
			: this(new CT_Font(), palette)
		{
		}

		public void SetFont(IFontModel font)
		{
			copy((XMLFontModel)font);
		}

		public void copy(XMLFontModel srcFont)
		{
			CT_Font font = srcFont._font;
			_hasBeenModified = srcFont.HasBeenModified;
			if (font.B != null)
			{
				Bold = srcFont.Bold;
			}
			if (font.I != null)
			{
				Italic = srcFont.Italic;
			}
			if (font.Strike != null)
			{
				Strikethrough = srcFont.Strikethrough;
			}
			if (font.Name != null)
			{
				Name = srcFont.Name;
			}
			if (font.Sz != null)
			{
				Size = srcFont.Size;
			}
			if (font.U != null)
			{
				Underline = srcFont.Underline;
			}
			if (font.VertAlign != null)
			{
				ScriptStyle = srcFont.ScriptStyle;
			}
			if (font.Family != null)
			{
				_font.Family = font.Family;
			}
			if (font.Scheme != null)
			{
				_font.Scheme = font.Scheme;
			}
			if (font.Color != null)
			{
				Color = ((XMLColorModel)srcFont.Color).Clone();
			}
		}

		public object Clone()
		{
			return DeepClone();
		}

		public XMLFontModel DeepClone()
		{
			XMLFontModel xMLFontModel = new XMLFontModel(_palette);
			xMLFontModel.copy(this);
			return xMLFontModel;
		}

		public override bool Equals(object o)
		{
			if (!(o is XMLFontModel))
			{
				return false;
			}
			XMLFontModel xMLFontModel = (XMLFontModel)o;
			if (_hasBeenModified != xMLFontModel._hasBeenModified)
			{
				return false;
			}
			bool num = ((_font.B == null && xMLFontModel._font.B == null) || Bold == xMLFontModel.Bold) && ((_font.I == null && xMLFontModel._font.I == null) || Italic == xMLFontModel.Italic) && ((_font.Strike == null && xMLFontModel._font.Strike == null) || Strikethrough == xMLFontModel.Strikethrough) && ((_font.U == null && xMLFontModel._font.U == null) || Underline == xMLFontModel.Underline);
			bool flag = _font.Family == xMLFontModel._font.Family && ((_font.Name == null && xMLFontModel._font.Name == null) || Name == xMLFontModel.Name) && ((_font.Sz == null && xMLFontModel._font.Sz == null) || Size == xMLFontModel.Size);
			bool flag2 = ((_font.VertAlign == null && xMLFontModel._font.VertAlign == null) || ScriptStyle == xMLFontModel.ScriptStyle) && ((_font.Color == null && xMLFontModel._font.Color == null) || Color.Equals(xMLFontModel.Color));
			return num && flag && flag2;
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (_font.B != null && (bool)_font.B.Val_Attr)
			{
				num |= 0x10;
			}
			if (_font.I != null && (bool)_font.I.Val_Attr)
			{
				num |= 0x20;
			}
			if (_font.Strike != null && (bool)_font.Strike.Val_Attr)
			{
				num |= 0x40;
			}
			if (_font.Family != null)
			{
				num ^= _font.Family.Val_Attr.GetHashCode();
			}
			num ^= ((_font.Name != null) ? _font.Name.Val_Attr.GetHashCode() : "Calibri".GetHashCode());
			num ^= ((_font.Sz != null) ? _font.Sz.Val_Attr.GetHashCode() : 11.0.GetHashCode());
			num ^= ((_font.U != null) ? _font.U.Val_Attr.GetHashCode() : XMLConstants.Font.Default.Underline.GetHashCode());
			num ^= ((_font.VertAlign != null) ? _font.VertAlign.GetHashCode() : XMLConstants.Font.Default.VerticalAlignment.GetHashCode());
			CT_Color cT_Color = new CT_Color();
			cT_Color.Theme_Attr = 1u;
			return num ^ ((_font.Color != null) ? _palette.GetColorFromCT(_font.Color).GetHashCode() : _palette.GetColorFromCT(cT_Color).GetHashCode());
		}
	}
}

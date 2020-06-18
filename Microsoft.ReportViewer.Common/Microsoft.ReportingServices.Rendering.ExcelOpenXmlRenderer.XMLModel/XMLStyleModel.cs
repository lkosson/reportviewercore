using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLStyleModel : IStyleModel, ICloneable, IDeepCloneable<XMLStyleModel>
	{
		private Style _interface;

		private GlobalStyle _globalInterface;

		private readonly StyleManager _manager;

		private readonly CT_Xf _xf;

		private bool _hasBeenModified;

		private uint _index;

		private string _numberformat;

		private XMLFontModel _font;

		private XMLFillModel _fill;

		private XMLBorderModel _border;

		public virtual Style Interface
		{
			get
			{
				if (_interface == null)
				{
					_interface = new Style(this);
				}
				return _interface;
			}
		}

		public GlobalStyle GlobalInterface
		{
			get
			{
				if (_globalInterface == null)
				{
					_globalInterface = new GlobalStyle(this);
				}
				return _globalInterface;
			}
		}

		public CT_Xf Data => _xf;

		public XMLFontModel Font
		{
			get
			{
				return _font;
			}
			set
			{
				_font = value;
			}
		}

		public string NumberFormat
		{
			get
			{
				return _numberformat;
			}
			set
			{
				_xf.ApplyNumberFormat_Attr = (value == null);
				_numberformat = value;
				_hasBeenModified = true;
			}
		}

		public IBorderModel BorderModel => _border;

		private XMLFillModel Fill
		{
			get
			{
				return _fill;
			}
			set
			{
				_fill = value;
			}
		}

		public ColorModel BackgroundColor
		{
			get
			{
				if (Fill != null)
				{
					return Fill.Color;
				}
				return null;
			}
			set
			{
				Fill.Color = (XMLColorModel)value;
				_hasBeenModified = true;
			}
		}

		private CT_CellAlignment Alignment
		{
			get
			{
				if (_xf.Alignment == null)
				{
					_xf.Alignment = new CT_CellAlignment();
				}
				_xf.ApplyAlignment_Attr = true;
				return _xf.Alignment;
			}
		}

		public ST_VerticalAlignment VerticalAlignment
		{
			get
			{
				if (_xf.Alignment == null || !_xf.Alignment.Vertical_Attr_Is_Specified)
				{
					return null;
				}
				return Alignment.Vertical_Attr;
			}
			set
			{
				if (value == null)
				{
					Alignment.Vertical_Attr_Is_Specified = false;
				}
				else
				{
					Alignment.Vertical_Attr = value;
				}
				_hasBeenModified = true;
			}
		}

		public ST_HorizontalAlignment HorizontalAlignment
		{
			get
			{
				if (_xf.Alignment == null || !_xf.Alignment.Horizontal_Attr_Is_Specified)
				{
					return null;
				}
				return Alignment.Horizontal_Attr;
			}
			set
			{
				if (value == null)
				{
					Alignment.Horizontal_Attr_Is_Specified = false;
				}
				else
				{
					Alignment.Horizontal_Attr = value;
				}
				_hasBeenModified = true;
			}
		}

		public uint? TextDirection
		{
			get
			{
				if (_xf.Alignment == null || !_xf.Alignment.ReadingOrder_Attr_Is_Specified)
				{
					return null;
				}
				return Alignment.ReadingOrder_Attr;
			}
			set
			{
				if (!value.HasValue)
				{
					Alignment.ReadingOrder_Attr_Is_Specified = false;
				}
				else
				{
					Alignment.ReadingOrder_Attr = value.Value;
				}
				_hasBeenModified = true;
			}
		}

		public bool? WrapText
		{
			get
			{
				if (_xf.Alignment == null || !_xf.Alignment.WrapText_Attr_Is_Specified)
				{
					return null;
				}
				return Alignment.WrapText_Attr;
			}
			set
			{
				if (!value.HasValue)
				{
					Alignment.WrapText_Attr_Is_Specified = false;
					return;
				}
				Alignment.WrapText_Attr = value.Value;
				_hasBeenModified = true;
			}
		}

		public int? IndentLevel
		{
			get
			{
				if (_xf.Alignment == null || !_xf.Alignment.WrapText_Attr_Is_Specified)
				{
					return null;
				}
				return (int)Alignment.Indent_Attr;
			}
			set
			{
				if (value < 0 || value > 255)
				{
					throw new FatalException();
				}
				if (!value.HasValue)
				{
					Alignment.Indent_Attr_Is_Specified = false;
					return;
				}
				Alignment.Indent_Attr = (uint)value.Value;
				_hasBeenModified = true;
			}
		}

		public int? Orientation
		{
			get
			{
				if (_xf.Alignment == null || !_xf.Alignment.TextRotation_Attr_Is_Specified)
				{
					return null;
				}
				return (int)Alignment.TextRotation_Attr;
			}
			set
			{
				if (value < 0 || (value > 180 && value < 254) || value > 255)
				{
					throw new FatalException();
				}
				if (!value.HasValue)
				{
					Alignment.TextRotation_Attr_Is_Specified = false;
					return;
				}
				Alignment.TextRotation_Attr = (uint)value.Value;
				_hasBeenModified = true;
			}
		}

		public bool HasBeenModified
		{
			get
			{
				if (!_hasBeenModified && !Font.HasBeenModified)
				{
					return BorderModel.HasBeenModified;
				}
				return true;
			}
		}

		public uint Index
		{
			get
			{
				return _index;
			}
			set
			{
				_index = value;
			}
		}

		public XMLStyleModel(StyleManager manager)
			: this(new CT_Xf(), manager)
		{
		}

		public XMLStyleModel(CT_Xf xf, StyleManager manager)
			: this(xf, manager, setVerticalAlignment: true)
		{
		}

		public XMLStyleModel(CT_Xf xf, StyleManager manager, bool setVerticalAlignment)
		{
			_xf = xf;
			_manager = manager;
			_font = (_xf.ApplyFont_Attr ? _manager.GetFont(_xf.FontId_Attr) : new XMLFontModel(_manager.Palette));
			_border = (_xf.ApplyBorder_Attr ? _manager.GetBorder(_xf.BorderId_Attr) : new XMLBorderModel(_manager.Palette));
			_fill = (_xf.ApplyFill_Attr ? _manager.GetFill(_xf.FillId_Attr).DeepClone() : new XMLFillModel(_manager.Palette));
			if (setVerticalAlignment)
			{
				Alignment.Vertical_Attr = ST_VerticalAlignment.top;
				Alignment.WrapText_Attr = true;
			}
		}

		public IStyleModel cloneStyle(bool cellStyle)
		{
			if (cellStyle)
			{
				return (XMLStyleModel)Clone();
			}
			throw new FatalException();
		}

		private void Copy(IStyleModel srcStyle)
		{
			XMLStyleModel xMLStyleModel = (XMLStyleModel)srcStyle;
			_hasBeenModified = xMLStyleModel._hasBeenModified;
			_xf.ApplyAlignment_Attr = xMLStyleModel._xf.ApplyAlignment_Attr;
			_xf.ApplyBorder_Attr = xMLStyleModel._xf.ApplyBorder_Attr;
			_xf.ApplyFill_Attr = xMLStyleModel._xf.ApplyFill_Attr;
			_xf.ApplyFont_Attr = xMLStyleModel._xf.ApplyFont_Attr;
			_xf.ApplyNumberFormat_Attr = xMLStyleModel._xf.ApplyNumberFormat_Attr;
			_xf.ApplyProtection_Attr = xMLStyleModel._xf.ApplyProtection_Attr;
			_xf.NumFmtId_Attr = xMLStyleModel._xf.NumFmtId_Attr;
			if (xMLStyleModel._xf.BorderId_Attr_Is_Specified)
			{
				_xf.BorderId_Attr = xMLStyleModel._xf.BorderId_Attr;
			}
			if (xMLStyleModel._xf.FillId_Attr_Is_Specified)
			{
				_xf.FillId_Attr = xMLStyleModel._xf.FillId_Attr;
			}
			if (xMLStyleModel._xf.FontId_Attr_Is_Specified)
			{
				_xf.FontId_Attr = xMLStyleModel._xf.FontId_Attr;
			}
			if (xMLStyleModel._xf.Alignment != null)
			{
				if (xMLStyleModel._xf.Alignment.Vertical_Attr_Is_Specified)
				{
					Alignment.Vertical_Attr = xMLStyleModel._xf.Alignment.Vertical_Attr;
				}
				if (xMLStyleModel.Alignment.Horizontal_Attr_Is_Specified)
				{
					Alignment.Horizontal_Attr = xMLStyleModel._xf.Alignment.Horizontal_Attr;
				}
				if (xMLStyleModel.Alignment.ReadingOrder_Attr_Is_Specified)
				{
					Alignment.ReadingOrder_Attr = xMLStyleModel._xf.Alignment.ReadingOrder_Attr;
				}
				if (xMLStyleModel.Alignment.WrapText_Attr_Is_Specified)
				{
					Alignment.WrapText_Attr = xMLStyleModel._xf.Alignment.WrapText_Attr;
				}
				if (xMLStyleModel.Alignment.ShrinkToFit_Attr_Is_Specified)
				{
					Alignment.ShrinkToFit_Attr = xMLStyleModel._xf.Alignment.ShrinkToFit_Attr;
				}
				if (xMLStyleModel.Alignment.Indent_Attr_Is_Specified)
				{
					Alignment.Indent_Attr = xMLStyleModel._xf.Alignment.Indent_Attr;
				}
				if (xMLStyleModel.Alignment.JustifyLastLine_Attr_Is_Specified)
				{
					Alignment.JustifyLastLine_Attr = xMLStyleModel._xf.Alignment.JustifyLastLine_Attr;
				}
				if (xMLStyleModel.Alignment.TextRotation_Attr_Is_Specified)
				{
					Alignment.TextRotation_Attr = xMLStyleModel._xf.Alignment.TextRotation_Attr;
				}
			}
			if (xMLStyleModel._numberformat != null)
			{
				_numberformat = xMLStyleModel._numberformat;
			}
			if (xMLStyleModel._font != null)
			{
				Font = (XMLFontModel)xMLStyleModel._font.Clone();
			}
			if (xMLStyleModel._fill != null)
			{
				Fill = xMLStyleModel._fill.DeepClone();
			}
			if (xMLStyleModel._border != null)
			{
				_border = xMLStyleModel._border.DeepClone();
			}
		}

		public XMLStyleModel DeepClone()
		{
			XMLStyleModel xMLStyleModel = new XMLStyleModel(_manager);
			xMLStyleModel.Copy(this);
			return xMLStyleModel;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is XMLStyleModel))
			{
				return false;
			}
			XMLStyleModel xMLStyleModel = (XMLStyleModel)obj;
			if (_hasBeenModified != xMLStyleModel._hasBeenModified)
			{
				return false;
			}
			if (_xf.Alignment != null)
			{
				if (xMLStyleModel._xf.Alignment == null)
				{
					return false;
				}
				if (_xf.Alignment.Horizontal_Attr_Is_Specified)
				{
					if (!xMLStyleModel._xf.Alignment.Horizontal_Attr_Is_Specified)
					{
						return false;
					}
					if (_xf.Alignment.Horizontal_Attr != xMLStyleModel._xf.Alignment.Horizontal_Attr)
					{
						return false;
					}
				}
				if (_xf.Alignment.Indent_Attr_Is_Specified)
				{
					if (!xMLStyleModel._xf.Alignment.Indent_Attr_Is_Specified)
					{
						return false;
					}
					if (_xf.Alignment.Indent_Attr != xMLStyleModel._xf.Alignment.Indent_Attr)
					{
						return false;
					}
				}
				if (_xf.Alignment.JustifyLastLine_Attr_Is_Specified)
				{
					if (!xMLStyleModel._xf.Alignment.JustifyLastLine_Attr_Is_Specified)
					{
						return false;
					}
					if ((bool)(_xf.Alignment.JustifyLastLine_Attr != xMLStyleModel._xf.Alignment.JustifyLastLine_Attr))
					{
						return false;
					}
				}
				if (_xf.Alignment.ReadingOrder_Attr_Is_Specified)
				{
					if (!xMLStyleModel._xf.Alignment.ReadingOrder_Attr_Is_Specified)
					{
						return false;
					}
					if (_xf.Alignment.ReadingOrder_Attr != xMLStyleModel._xf.Alignment.ReadingOrder_Attr)
					{
						return false;
					}
				}
				if (_xf.Alignment.RelativeIndent_Attr_Is_Specified)
				{
					if (!xMLStyleModel._xf.Alignment.RelativeIndent_Attr_Is_Specified)
					{
						return false;
					}
					if (_xf.Alignment.RelativeIndent_Attr != xMLStyleModel._xf.Alignment.RelativeIndent_Attr)
					{
						return false;
					}
				}
				if (_xf.Alignment.ShrinkToFit_Attr_Is_Specified)
				{
					if (!xMLStyleModel._xf.Alignment.ShrinkToFit_Attr_Is_Specified)
					{
						return false;
					}
					if ((bool)(_xf.Alignment.ShrinkToFit_Attr != xMLStyleModel._xf.Alignment.ShrinkToFit_Attr))
					{
						return false;
					}
				}
				if (_xf.Alignment.TextRotation_Attr_Is_Specified)
				{
					if (!xMLStyleModel._xf.Alignment.TextRotation_Attr_Is_Specified)
					{
						return false;
					}
					if (_xf.Alignment.TextRotation_Attr != xMLStyleModel._xf.Alignment.TextRotation_Attr)
					{
						return false;
					}
				}
				if (_xf.Alignment.Vertical_Attr_Is_Specified)
				{
					if (!xMLStyleModel._xf.Alignment.Vertical_Attr_Is_Specified)
					{
						return false;
					}
					if (_xf.Alignment.Vertical_Attr != xMLStyleModel._xf.Alignment.Vertical_Attr)
					{
						return false;
					}
				}
				if (_xf.Alignment.WrapText_Attr_Is_Specified)
				{
					if (!xMLStyleModel._xf.Alignment.WrapText_Attr_Is_Specified)
					{
						return false;
					}
					if ((bool)(_xf.Alignment.WrapText_Attr != xMLStyleModel._xf.Alignment.WrapText_Attr))
					{
						return false;
					}
				}
			}
			else if (xMLStyleModel._xf.Alignment != null)
			{
				return false;
			}
			if ((bool)(_xf.ApplyAlignment_Attr != xMLStyleModel._xf.ApplyAlignment_Attr))
			{
				return false;
			}
			if ((bool)(_xf.ApplyBorder_Attr != xMLStyleModel._xf.ApplyBorder_Attr))
			{
				return false;
			}
			if ((bool)(_xf.ApplyFill_Attr != xMLStyleModel._xf.ApplyFill_Attr))
			{
				return false;
			}
			if ((bool)(_xf.ApplyFont_Attr != xMLStyleModel._xf.ApplyFont_Attr))
			{
				return false;
			}
			if ((bool)(_xf.ApplyNumberFormat_Attr != xMLStyleModel._xf.ApplyNumberFormat_Attr))
			{
				return false;
			}
			if ((bool)(_xf.ApplyProtection_Attr != xMLStyleModel._xf.ApplyProtection_Attr))
			{
				return false;
			}
			if (_xf.XfId_Attr != xMLStyleModel._xf.XfId_Attr)
			{
				return false;
			}
			if ((bool)(_xf.QuotePrefix_Attr != xMLStyleModel._xf.QuotePrefix_Attr))
			{
				return false;
			}
			if (_border != null && !_border.Equals(xMLStyleModel._border))
			{
				return false;
			}
			if (_border == null && xMLStyleModel._border != null)
			{
				return false;
			}
			if (_fill != null && !_fill.Equals(xMLStyleModel._fill))
			{
				return false;
			}
			if (_fill == null && xMLStyleModel._fill != null)
			{
				return false;
			}
			if (_font != null && !_font.Equals(xMLStyleModel._font))
			{
				return false;
			}
			if (_font == null && xMLStyleModel._font != null)
			{
				return false;
			}
			if (_numberformat == null && xMLStyleModel._numberformat == null && (bool)_xf.ApplyNumberFormat_Attr && ((bool)(!xMLStyleModel._xf.ApplyNumberFormat_Attr) || _xf.NumFmtId_Attr != xMLStyleModel._xf.NumFmtId_Attr))
			{
				return false;
			}
			if (_numberformat != null && !_numberformat.Equals(xMLStyleModel._numberformat))
			{
				return false;
			}
			if (_numberformat == null && xMLStyleModel._numberformat != null)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 0;
			num ^= (_xf.ApplyAlignment_Attr ? 1 : 0);
			num ^= (_xf.ApplyBorder_Attr ? 2 : 0);
			num ^= (_xf.ApplyFill_Attr ? 4 : 0);
			num ^= (_xf.ApplyFont_Attr ? 8 : 0);
			num ^= (_xf.ApplyNumberFormat_Attr ? 16 : 0);
			num ^= (_xf.ApplyProtection_Attr ? 32 : 0);
			if (_xf.Alignment != null)
			{
				num ^= (_xf.Alignment.Horizontal_Attr_Is_Specified ? _xf.Alignment.Horizontal_Attr.GetHashCode() : 0);
				num ^= (_xf.Alignment.Indent_Attr_Is_Specified ? _xf.Alignment.Indent_Attr.GetHashCode() : 0);
				num ^= ((_xf.Alignment.JustifyLastLine_Attr_Is_Specified && (bool)_xf.Alignment.JustifyLastLine_Attr) ? 256 : 0);
				num ^= (_xf.Alignment.ReadingOrder_Attr_Is_Specified ? _xf.Alignment.ReadingOrder_Attr.GetHashCode() : 0);
				num ^= (_xf.Alignment.RelativeIndent_Attr_Is_Specified ? _xf.Alignment.RelativeIndent_Attr : 0);
				num ^= ((_xf.Alignment.ShrinkToFit_Attr_Is_Specified && (bool)_xf.Alignment.ShrinkToFit_Attr) ? 512 : 0);
				num ^= (_xf.Alignment.TextRotation_Attr_Is_Specified ? _xf.Alignment.TextRotation_Attr.GetHashCode() : 0);
				num ^= (_xf.Alignment.Vertical_Attr_Is_Specified ? _xf.Alignment.Vertical_Attr.GetHashCode() : 0);
				num ^= ((_xf.Alignment.WrapText_Attr_Is_Specified && (bool)_xf.Alignment.WrapText_Attr) ? 1024 : 0);
			}
			if (_border != null)
			{
				num ^= _border.GetHashCode();
			}
			if (_fill != null)
			{
				num ^= _fill.GetHashCode();
			}
			if (_font != null)
			{
				num ^= _font.GetHashCode();
			}
			if (_numberformat != null)
			{
				num ^= _numberformat.GetHashCode();
			}
			return num;
		}

		public object Clone()
		{
			return DeepClone();
		}

		public void Cleanup()
		{
			if (_numberformat != null)
			{
				_xf.ApplyNumberFormat_Attr = true;
				_xf.NumFmtId_Attr = _manager.AddNumberFormat(_numberformat);
			}
			if (_font != null)
			{
				_xf.ApplyFont_Attr = true;
				_xf.FontId_Attr = _manager.AddFont(_font);
			}
			if (_fill != null)
			{
				_xf.ApplyFill_Attr = true;
				_xf.FillId_Attr = _manager.AddFill(_fill);
			}
			if (_border != null)
			{
				_xf.ApplyBorder_Attr = true;
				_xf.BorderId_Attr = _manager.AddBorder(_border);
			}
		}
	}
}

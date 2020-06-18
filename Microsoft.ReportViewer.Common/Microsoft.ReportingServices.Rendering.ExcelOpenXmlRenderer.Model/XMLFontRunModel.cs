using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.sharedTypes;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal class XMLFontRunModel : IFontModel, ICloneable
	{
		private readonly Font _interface;

		private readonly List<CT_RPrElt> _props;

		private readonly XMLPaletteModel _palette;

		public Font Interface => _interface;

		public bool Bold
		{
			set
			{
				if (_props[0].B == null)
				{
					foreach (CT_RPrElt prop in _props)
					{
						prop.B = new CT_BooleanProperty();
						prop.B.Val_Attr = value;
					}
					return;
				}
				foreach (CT_RPrElt prop2 in _props)
				{
					prop2.B.Val_Attr = value;
				}
			}
		}

		public bool Italic
		{
			set
			{
				if (_props[0].I == null)
				{
					foreach (CT_RPrElt prop in _props)
					{
						prop.I = new CT_BooleanProperty();
						prop.I.Val_Attr = value;
					}
					return;
				}
				foreach (CT_RPrElt prop2 in _props)
				{
					prop2.I.Val_Attr = value;
				}
			}
		}

		public bool Strikethrough
		{
			set
			{
				if (_props[0].Strike == null)
				{
					foreach (CT_RPrElt prop in _props)
					{
						prop.Strike = new CT_BooleanProperty();
						prop.Strike.Val_Attr = value;
					}
					return;
				}
				foreach (CT_RPrElt prop2 in _props)
				{
					prop2.Strike.Val_Attr = value;
				}
			}
		}

		public string Name
		{
			set
			{
				if (value == null)
				{
					throw new FatalException();
				}
				string val_Attr = ExcelGeneratorStringUtil.Truncate(value, 31);
				if (_props[0].RFont == null)
				{
					foreach (CT_RPrElt prop in _props)
					{
						prop.RFont = new CT_FontName();
						prop.RFont.Val_Attr = val_Attr;
					}
					return;
				}
				foreach (CT_RPrElt prop2 in _props)
				{
					prop2.RFont.Val_Attr = val_Attr;
				}
			}
		}

		public double Size
		{
			set
			{
				if (value < 1.0 || value > 409.55)
				{
					throw new FatalException();
				}
				if (_props[0].Sz != null)
				{
					return;
				}
				foreach (CT_RPrElt prop in _props)
				{
					prop.Sz = new CT_FontSize();
					prop.Sz.Val_Attr = value;
				}
			}
		}

		public ST_UnderlineValues Underline
		{
			set
			{
				if (value == null)
				{
					foreach (CT_RPrElt prop in _props)
					{
						prop.U = null;
					}
					return;
				}
				if (_props[0].U == null)
				{
					foreach (CT_RPrElt prop2 in _props)
					{
						prop2.U = new CT_UnderlineProperty();
						prop2.U.Val_Attr = value;
					}
					return;
				}
				foreach (CT_RPrElt prop3 in _props)
				{
					prop3.U.Val_Attr = value;
				}
			}
		}

		public ST_VerticalAlignRun ScriptStyle
		{
			set
			{
				if (value == null)
				{
					foreach (CT_RPrElt prop in _props)
					{
						prop.VertAlign = null;
					}
					return;
				}
				if (_props[0].VertAlign == null)
				{
					foreach (CT_RPrElt prop2 in _props)
					{
						prop2.VertAlign = new CT_VerticalAlignFontProperty();
						prop2.VertAlign.Val_Attr = value;
					}
					return;
				}
				foreach (CT_RPrElt prop3 in _props)
				{
					prop3.VertAlign.Val_Attr = value;
				}
			}
		}

		public int Charset
		{
			set
			{
				if (value < 0 || value > 255)
				{
					throw new FatalException();
				}
				if (_props[0].Charset == null)
				{
					foreach (CT_RPrElt prop in _props)
					{
						prop.Charset = new CT_IntProperty();
						prop.Charset.Val_Attr = value;
					}
					return;
				}
				foreach (CT_RPrElt prop2 in _props)
				{
					prop2.Charset.Val_Attr = value;
				}
			}
		}

		public ColorModel Color
		{
			set
			{
				if (value == null)
				{
					throw new FatalException();
				}
				if (!(value is XMLColorModel))
				{
					throw new FatalException();
				}
				foreach (CT_RPrElt prop in _props)
				{
					prop.Color = ((XMLColorModel)value).Clone().Data;
				}
			}
		}

		public XMLFontRunModel(XMLPaletteModel palette)
		{
			_interface = new Font(this);
			_props = new List<CT_RPrElt>();
			_palette = palette;
		}

		public object Clone()
		{
			throw new FatalException();
		}

		public void Add(CT_RPrElt prop)
		{
			_props.Add(prop);
		}

		public void SetFont(IFontModel font)
		{
			SetFont((XMLFontModel)font);
		}

		public void SetFont(XMLFontModel font)
		{
			XMLFontModel xMLFontModel = new XMLFontModel(_palette);
			xMLFontModel.copy(font);
			Bold = xMLFontModel.Bold;
			Italic = xMLFontModel.Italic;
			Strikethrough = xMLFontModel.Strikethrough;
			Name = xMLFontModel.Name;
			Size = xMLFontModel.Size;
			Underline = xMLFontModel.Underline;
			ScriptStyle = xMLFontModel.ScriptStyle;
			Color = xMLFontModel.Color;
		}
	}
}

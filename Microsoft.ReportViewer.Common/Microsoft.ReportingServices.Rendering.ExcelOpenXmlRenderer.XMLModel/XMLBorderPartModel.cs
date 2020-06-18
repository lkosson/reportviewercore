using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLBorderPartModel : IBorderPartModel, IDeepCloneable<XMLBorderPartModel>
	{
		private readonly CT_BorderPr _part;

		private readonly XMLPaletteModel _palette;

		private bool _hasBeenModified;

		public bool HasBeenModified => _hasBeenModified;

		public virtual ColorModel Color
		{
			get
			{
				if (Part.Color == null)
				{
					Part.Color = new CT_Color();
					Part.Color.Theme_Attr = 1u;
				}
				return _palette.GetColorFromCT(Part.Color);
			}
			set
			{
				if (value != null)
				{
					Part.Color = ((XMLColorModel)value).Data;
					_hasBeenModified = true;
				}
				else
				{
					Part.Color = null;
				}
			}
		}

		public virtual ST_BorderStyle Style
		{
			get
			{
				return Part.Style_Attr;
			}
			set
			{
				Part.Style_Attr = value;
				_hasBeenModified = true;
			}
		}

		public CT_BorderPr Part => _part;

		public XMLBorderPartModel(CT_BorderPr part, XMLPaletteModel palette)
		{
			_part = part;
			_palette = palette;
		}

		public XMLBorderPartModel DeepClone()
		{
			XMLBorderPartModel xMLBorderPartModel = new XMLBorderPartModel(new CT_BorderPr(), _palette);
			if (Part.Color != null)
			{
				xMLBorderPartModel.Part.Color = _palette.GetColorFromCT(Part.Color).Clone().Data;
			}
			if (Part.Style_Attr != null)
			{
				xMLBorderPartModel.Part.Style_Attr = Part.Style_Attr;
			}
			xMLBorderPartModel._hasBeenModified = _hasBeenModified;
			return xMLBorderPartModel;
		}

		public override bool Equals(object o)
		{
			if (!(o is XMLBorderPartModel))
			{
				return false;
			}
			XMLBorderPartModel xMLBorderPartModel = (XMLBorderPartModel)o;
			if (HasBeenModified != xMLBorderPartModel.HasBeenModified)
			{
				return false;
			}
			if (Part.Style_Attr != xMLBorderPartModel.Part.Style_Attr)
			{
				return false;
			}
			if (!_palette.GetColorFromCT(Part.Color).Equals(_palette.GetColorFromCT(xMLBorderPartModel.Part.Color)))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return 0 ^ Part.Style_Attr.GetHashCode() ^ _palette.GetColorFromCT(Part.Color).GetHashCode();
		}
	}
}

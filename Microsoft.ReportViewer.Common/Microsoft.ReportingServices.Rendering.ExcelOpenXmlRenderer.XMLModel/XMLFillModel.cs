using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLFillModel : IDeepCloneable<XMLFillModel>
	{
		private readonly CT_Fill _fill;

		private XMLColorModel _foreground;

		private XMLColorModel _background;

		private readonly XMLPaletteModel _palette;

		public CT_Fill Data => _fill;

		public XMLColorModel Color
		{
			get
			{
				return _foreground;
			}
			set
			{
				if (_fill.Choice_0 == CT_Fill.ChoiceBucket_0.patternFill)
				{
					_foreground = value;
					_fill.PatternFill.PatternType_Attr = ((value == null) ? ST_PatternType.none : ST_PatternType.solid);
					_background = value;
					return;
				}
				throw new FatalException();
			}
		}

		public XMLFillModel(XMLPaletteModel palette)
		{
			_fill = new CT_Fill();
			_fill.Choice_0 = CT_Fill.ChoiceBucket_0.patternFill;
			_fill.PatternFill = new CT_PatternFill();
			_fill.PatternFill.PatternType_Attr = ST_PatternType.none;
			_palette = palette;
		}

		public XMLFillModel(CT_Fill fill, XMLPaletteModel palette)
		{
			_fill = fill;
			_palette = palette;
			if (_fill.PatternFill != null)
			{
				if (_fill.PatternFill.FgColor != null)
				{
					_foreground = _palette.GetColorFromCT(_fill.PatternFill.FgColor);
				}
				if (_fill.PatternFill.BgColor != null)
				{
					_background = _palette.GetColorFromCT(_fill.PatternFill.BgColor);
				}
			}
		}

		public XMLFillModel DeepClone()
		{
			CT_Fill cT_Fill = new CT_Fill();
			if (_fill.PatternFill != null)
			{
				CT_PatternFill patternFill = _fill.PatternFill;
				CT_PatternFill cT_PatternFill2 = cT_Fill.PatternFill = new CT_PatternFill();
				if (patternFill.BgColor != null)
				{
					cT_PatternFill2.BgColor = _palette.GetColorFromCT(patternFill.BgColor).Clone().Data;
				}
				if (patternFill.FgColor != null)
				{
					cT_PatternFill2.FgColor = _palette.GetColorFromCT(patternFill.FgColor).Clone().Data;
				}
				cT_PatternFill2.PatternType_Attr = patternFill.PatternType_Attr;
			}
			cT_Fill.Choice_0 = _fill.Choice_0;
			XMLFillModel xMLFillModel = new XMLFillModel(cT_Fill, _palette);
			if (_background != null)
			{
				xMLFillModel._background = _background.Clone();
			}
			if (_foreground != null)
			{
				xMLFillModel._foreground = _foreground.Clone();
			}
			return xMLFillModel;
		}

		public override bool Equals(object o)
		{
			if (!(o is XMLFillModel))
			{
				return false;
			}
			XMLFillModel xMLFillModel = (XMLFillModel)o;
			if (_fill.Choice_0 != xMLFillModel._fill.Choice_0)
			{
				return false;
			}
			Cleanup();
			xMLFillModel.Cleanup();
			if (_palette.GetColorFromCT(_fill.PatternFill.BgColor).Equals(_palette.GetColorFromCT(xMLFillModel._fill.PatternFill.BgColor)) && _palette.GetColorFromCT(_fill.PatternFill.FgColor).Equals(_palette.GetColorFromCT(xMLFillModel._fill.PatternFill.FgColor)))
			{
				return _fill.PatternFill.PatternType_Attr == xMLFillModel._fill.PatternFill.PatternType_Attr;
			}
			return false;
		}

		public override int GetHashCode()
		{
			Cleanup();
			return 0 ^ _palette.GetColorFromCT(_fill.PatternFill.BgColor).GetHashCode() ^ _palette.GetColorFromCT(_fill.PatternFill.FgColor).GetHashCode() ^ _fill.PatternFill.PatternType_Attr.GetHashCode();
		}

		public void Cleanup()
		{
			if (_fill.Choice_0 == CT_Fill.ChoiceBucket_0.patternFill)
			{
				if (_foreground != null)
				{
					_fill.PatternFill.FgColor = _foreground.Data;
				}
				if (_background != null)
				{
					_fill.PatternFill.BgColor = _background.Data;
				}
			}
		}
	}
}

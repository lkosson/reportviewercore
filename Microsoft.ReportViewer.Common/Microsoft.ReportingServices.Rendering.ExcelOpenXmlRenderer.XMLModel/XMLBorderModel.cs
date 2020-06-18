using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLBorderModel : IBorderModel, IDeepCloneable<XMLBorderModel>
	{
		private readonly CT_Border _border;

		private readonly XMLPaletteModel _palette;

		private XMLBorderPartModel _topBorder;

		private XMLBorderPartModel _bottomBorder;

		private XMLBorderPartModel _leftBorder;

		private XMLBorderPartModel _rightBorder;

		private XMLBorderPartModel _diagonalBorder;

		public CT_Border Data => _border;

		public bool HasBeenModified
		{
			get
			{
				if (!_topBorder.HasBeenModified && !_bottomBorder.HasBeenModified && !_leftBorder.HasBeenModified && !_rightBorder.HasBeenModified)
				{
					return _diagonalBorder.HasBeenModified;
				}
				return true;
			}
		}

		public XMLBorderPartModel TopBorder => _topBorder;

		public XMLBorderPartModel BottomBorder => _bottomBorder;

		public XMLBorderPartModel LeftBorder => _leftBorder;

		public XMLBorderPartModel RightBorder => _rightBorder;

		public XMLBorderPartModel DiagonalBorder => _diagonalBorder;

		public ExcelBorderPart DiagonalPartDirection
		{
			set
			{
				switch (value)
				{
				case ExcelBorderPart.DiagonalUp:
					_border.DiagonalDown_Attr = false;
					_border.DiagonalUp_Attr = true;
					break;
				case ExcelBorderPart.DiagonalDown:
					_border.DiagonalDown_Attr = true;
					_border.DiagonalUp_Attr = false;
					break;
				case ExcelBorderPart.DiagonalBoth:
					_border.DiagonalDown_Attr = true;
					_border.DiagonalUp_Attr = true;
					break;
				}
			}
		}

		public XMLBorderModel(XMLPaletteModel palette)
		{
			_border = new CT_Border();
			_border.Top = new CT_BorderPr();
			_border.Bottom = new CT_BorderPr();
			_border.Left = new CT_BorderPr();
			_border.Right = new CT_BorderPr();
			_border.Diagonal = new CT_BorderPr();
			_palette = palette;
			InitBorderPartModels();
		}

		public XMLBorderModel(CT_Border border, XMLPaletteModel palette)
		{
			_border = border;
			_palette = palette;
			InitBorderPartModels();
		}

		private void InitBorderPartModels()
		{
			_topBorder = new XMLBorderPartModel(_border.Top, _palette);
			_bottomBorder = new XMLBorderPartModel(_border.Bottom, _palette);
			_leftBorder = new XMLBorderPartModel(_border.Left, _palette);
			_rightBorder = new XMLBorderPartModel(_border.Right, _palette);
			_diagonalBorder = new XMLBorderPartModel(_border.Diagonal, _palette);
		}

		public XMLBorderModel DeepClone()
		{
			XMLBorderModel xMLBorderModel = new XMLBorderModel(_palette);
			CT_Border border = _border;
			CT_Border border2 = xMLBorderModel._border;
			if (border.DiagonalDown_Attr_Is_Specified)
			{
				border2.DiagonalDown_Attr = border.DiagonalDown_Attr;
			}
			if (border.DiagonalUp_Attr_Is_Specified)
			{
				border2.DiagonalUp_Attr = border.DiagonalUp_Attr;
			}
			border2.Outline_Attr = border.Outline_Attr;
			xMLBorderModel._bottomBorder = _bottomBorder.DeepClone();
			border2.Bottom = xMLBorderModel._bottomBorder.Part;
			xMLBorderModel._topBorder = _topBorder.DeepClone();
			border2.Top = xMLBorderModel._topBorder.Part;
			xMLBorderModel._leftBorder = _leftBorder.DeepClone();
			border2.Left = xMLBorderModel._leftBorder.Part;
			xMLBorderModel._rightBorder = _rightBorder.DeepClone();
			border2.Right = xMLBorderModel._rightBorder.Part;
			xMLBorderModel._diagonalBorder = _diagonalBorder.DeepClone();
			border2.Diagonal = xMLBorderModel._diagonalBorder.Part;
			return xMLBorderModel;
		}

		public override bool Equals(object o)
		{
			if (!(o is XMLBorderModel))
			{
				return false;
			}
			XMLBorderModel xMLBorderModel = (XMLBorderModel)o;
			if (new XMLBorderPartModel(_border.Bottom, _palette).Equals(new XMLBorderPartModel(xMLBorderModel._border.Bottom, _palette)) && new XMLBorderPartModel(_border.Diagonal, _palette).Equals(new XMLBorderPartModel(xMLBorderModel._border.Diagonal, _palette)) && new XMLBorderPartModel(_border.Left, _palette).Equals(new XMLBorderPartModel(xMLBorderModel._border.Left, _palette)) && new XMLBorderPartModel(_border.Right, _palette).Equals(new XMLBorderPartModel(xMLBorderModel._border.Right, _palette)) && new XMLBorderPartModel(_border.Top, _palette).Equals(new XMLBorderPartModel(xMLBorderModel._border.Top, _palette)) && (bool)(_border.DiagonalDown_Attr == xMLBorderModel._border.DiagonalDown_Attr) && (bool)(_border.DiagonalUp_Attr == xMLBorderModel._border.DiagonalUp_Attr))
			{
				return _border.Outline_Attr == xMLBorderModel._border.Outline_Attr;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return 0 ^ _border.DiagonalDown_Attr.GetHashCode() ^ _border.DiagonalUp_Attr.GetHashCode() ^ _border.Outline_Attr.GetHashCode() ^ new XMLBorderPartModel(_border.Bottom, _palette).GetHashCode() ^ new XMLBorderPartModel(_border.Top, _palette).GetHashCode() ^ new XMLBorderPartModel(_border.Left, _palette).GetHashCode() ^ new XMLBorderPartModel(_border.Right, _palette).GetHashCode() ^ new XMLBorderPartModel(_border.Diagonal, _palette).GetHashCode();
		}
	}
}

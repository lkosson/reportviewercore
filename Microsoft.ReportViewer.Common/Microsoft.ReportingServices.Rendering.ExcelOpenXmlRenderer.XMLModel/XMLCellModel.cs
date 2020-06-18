using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Models;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLCellModel : ICellModel, IOoxmlCtWrapperModel
	{
		private Cell _interface;

		private readonly CT_Cell _cell;

		private ICellController _controller;

		public Cell Interface
		{
			get
			{
				if (_interface == null)
				{
					_interface = new Cell(this);
				}
				return _interface;
			}
		}

		public CT_Cell Data => _cell;

		public Cell.CellValueType ValueType => _controller.ValueType;

		public object Value
		{
			get
			{
				return _controller.Value;
			}
			set
			{
				_controller.Value = value;
			}
		}

		public IStyleModel Style
		{
			get
			{
				return _controller.Style;
			}
			set
			{
				_controller.Style = value;
			}
		}

		public string Name => _cell.R_Attr;

		public OoxmlComplexType OoxmlTag => _cell;

		public XMLCellModel(XMLWorksheetModel sheet, PartManager manager, CT_Cell cell)
		{
			_cell = cell;
			_controller = new XMLCellController(cell, sheet, manager);
			FixCellValue();
		}

		private void FixCellValue()
		{
			if (_cell.T_Attr != ST_CellType.str)
			{
				_cell.V = null;
				_cell.T_Attr = ST_CellType.str;
			}
		}

		public ICharacterRunModel getCharacters(int startIndex, int length)
		{
			return _controller.CharManager.CreateRun(startIndex, length);
		}

		public void Cleanup()
		{
			_controller.Cleanup();
		}
	}
}

using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Models;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLRowModel : IRowModel, IOoxmlCtWrapperModel
	{
		private Row _rowInterface;

		private readonly XMLWorksheetModel _worksheetModel;

		private readonly CT_Row _row;

		private readonly PartManager _manager;

		private IDictionary<int, ICellModel> _cells;

		public Row RowInterface
		{
			get
			{
				if (_rowInterface == null)
				{
					_rowInterface = new Row(this);
				}
				return _rowInterface;
			}
		}

		public IWorksheetModel WorksheetModel
		{
			get
			{
				if (_worksheetModel == null)
				{
					throw new FatalException();
				}
				return _worksheetModel;
			}
		}

		public IDictionary<int, ICellModel> CellsMap => _cells;

		public int RowNumber
		{
			get
			{
				return (int)(_row.R_Attr - 1);
			}
			set
			{
				_row.R_Attr = (uint)(value + 1);
			}
		}

		public double Height
		{
			set
			{
				_row.Ht_Attr = value;
				_row.CustomHeight_Attr = true;
			}
		}

		public bool CustomHeight
		{
			set
			{
				_row.CustomHeight_Attr = value;
			}
		}

		public int OutlineLevel
		{
			set
			{
				if (value < 0 || value > 7)
				{
					throw new FatalException();
				}
				_row.OutlineLevel_Attr = (byte)value;
			}
		}

		public bool Hidden
		{
			set
			{
				_row.Hidden_Attr = value;
			}
		}

		public bool OutlineCollapsed
		{
			set
			{
				_row.Collapsed_Attr = value;
			}
		}

		public OoxmlComplexType OoxmlTag => _row;

		public XMLRowModel(XMLStreamsheetModel sheet, PartManager manager, int rowNumber)
		{
			_worksheetModel = sheet;
			_manager = manager;
			_row = new CT_Row();
			_row.C = new List<CT_Cell>();
			RowNumber = rowNumber;
			Init();
		}

		public ICellModel getCell(int col)
		{
			if (col < 0 || col > 16383)
			{
				throw new FatalException();
			}
			if (CellsMap.TryGetValue(col, out ICellModel value))
			{
				return value;
			}
			CT_Cell cT_Cell = new CT_Cell();
			cT_Cell.R_Attr = CellPair.Name(RowNumber, col);
			value = new XMLCellModel(_worksheetModel, _manager, cT_Cell);
			CellsMap.Add(col, value);
			return value;
		}

		public void ClearCell(int column)
		{
			if (CellsMap.ContainsKey(column))
			{
				CellsMap.Remove(column);
			}
		}

		private void Init()
		{
			_cells = new Dictionary<int, ICellModel>();
			_row.C.Clear();
		}

		public void Cleanup()
		{
			List<int> list = new List<int>(CellsMap.Keys);
			list.Sort();
			foreach (int item in list)
			{
				XMLCellModel xMLCellModel = (XMLCellModel)CellsMap[item];
				xMLCellModel.Cleanup();
				_row.C.Add(xMLCellModel.Data);
			}
			CellsMap.Clear();
		}
	}
}

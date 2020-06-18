using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class Streamsheet
	{
		private readonly IStreamsheetModel _model;

		public string Name
		{
			get
			{
				return _model.Name;
			}
			set
			{
				_model.Name = value;
			}
		}

		public Pictures Pictures => _model.Pictures.Interface;

		public PageSetup PageSetup => _model.PageSetup.Interface;

		public bool ShowGridlines
		{
			set
			{
				_model.ShowGridlines = value;
			}
		}

		public int MaxRowIndex => _model.MaxRowIndex;

		public int MaxColIndex => _model.MaxColIndex;

		public Streamsheet(IStreamsheetModel model)
		{
			_model = model;
		}

		public Row CreateRow()
		{
			return _model.CreateRow().RowInterface;
		}

		public Row CreateRow(int index)
		{
			return _model.CreateRow(index).RowInterface;
		}

		public ColumnProperties GetColumnProperties(int columnIndex)
		{
			return _model.getColumn(columnIndex).Interface;
		}

		public void CreateHyperlink(string areaFormula, string href, string label)
		{
			_model.CreateHyperlink(areaFormula, href, label);
		}

		public Anchor CreateAnchor(int rowNumber, int columnNumber, double offsetX, double offsetY)
		{
			return _model.createAnchor(rowNumber, columnNumber, offsetX, offsetY).Interface;
		}

		public void MergeCells(int firstRow, int firstCol, int rowCount, int colCount)
		{
			_model.MergeCells(firstRow, firstCol, rowCount, colCount);
		}

		public void SetFreezePanes(int row, int col)
		{
			_model.SetFreezePanes(row, col);
		}

		public void SetBackgroundPicture(string uniqueId, string extension, Stream pictureStream)
		{
			_model.SetBackgroundPicture(uniqueId, extension, pictureStream);
		}

		public void Cleanup()
		{
			_model.Cleanup();
		}
	}
}

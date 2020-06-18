using System;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal interface IStreamsheetModel : IWorksheetModel, ICloneable
	{
		new Streamsheet Interface
		{
			get;
		}

		int MaxRowIndex
		{
			get;
		}

		int MaxColIndex
		{
			get;
		}

		IRowModel CreateRow();

		IRowModel CreateRow(int index);

		void CreateHyperlink(string areaFormula, string href, string label);

		void MergeCells(int firstRow, int firstCol, int rowCount, int colCount);

		void SetFreezePanes(int row, int column);

		void Cleanup();
	}
}

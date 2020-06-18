namespace Microsoft.Reporting.WinForms
{
	internal class RenderingItemBorderTablix
	{
		internal int RowZIndex;

		internal int ColumnZIndex;

		internal int RowIndex;

		internal int ColumnIndex;

		internal bool CompareRowFirst = true;

		internal RenderingItem Item;

		internal RenderingItemBorderTablix(int rowZIndex, int columnZIndex, int rowIndex, int columnIndex, RenderingItem item)
		{
			RowZIndex = rowZIndex;
			ColumnZIndex = columnZIndex;
			RowIndex = rowIndex;
			ColumnIndex = columnIndex;
			Item = item;
		}
	}
}

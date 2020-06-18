namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal static class CellReference
	{
		internal static string CreateExcelReference(string sheetName, int row, int column)
		{
			return "'" + sheetName + "'!" + ConvertToLetter(column) + (row + 1);
		}

		private static string ConvertToLetter(int val)
		{
			string str = "";
			int num = 26;
			if (val / num > 0)
			{
				str += (char)(val / num + 65 - 1);
			}
			return str + (char)(val % num + 65);
		}
	}
}

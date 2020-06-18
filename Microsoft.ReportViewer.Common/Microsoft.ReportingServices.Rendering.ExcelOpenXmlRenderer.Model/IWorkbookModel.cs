namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal interface IWorkbookModel
	{
		IWorksheetsModel Worksheets
		{
			get;
		}

		IPaletteModel Palette
		{
			get;
		}

		IWorksheetModel getWorksheet(int sheetOffset);

		IStyleModel createGlobalStyle();
	}
}

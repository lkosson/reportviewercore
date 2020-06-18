using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal abstract class XMLWorkbookModel : IWorkbookModel
	{
		protected PartManager _manager;

		protected XMLWorksheetsModel _worksheets;

		protected XMLDefinedNamesManager _nameManager;

		public abstract IWorksheetsModel Worksheets
		{
			get;
		}

		public IPaletteModel Palette => _manager.StyleSheet.Palette;

		public XMLDefinedNamesManager NameManager => _nameManager;

		public IWorksheetModel getWorksheet(int sheetOffset)
		{
			return Worksheets.GetWorksheet(sheetOffset);
		}

		public IStyleModel createGlobalStyle()
		{
			return _manager.StyleSheet.CreateStyle();
		}

		public void Cleanup()
		{
			foreach (XMLWorksheetModel worksheet in Worksheets)
			{
				worksheet.Cleanup();
			}
			_nameManager.Cleanup();
		}
	}
}

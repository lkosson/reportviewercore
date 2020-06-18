using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLStreamsheetsModel : XMLWorksheetsModel
	{
		private XMLStreamsheetModel _currentSheet;

		public XMLStreamsheetsModel(XMLWorkbookModel workbook, PartManager manager)
			: base(workbook, manager)
		{
		}

		public override IStreamsheetModel CreateStreamsheet(string sheetName, ExcelGeneratorConstants.CreateTempStream createTempStream)
		{
			return CreateSheet(sheetName, createTempStream);
		}

		private XMLStreamsheetModel CreateSheet(string sheetName, ExcelGeneratorConstants.CreateTempStream createTempStream)
		{
			XMLStreamsheetModel xMLStreamsheetModel = new XMLStreamsheetModel((XMLStreambookModel)Workbook, this, Manager, sheetName, createTempStream);
			Worksheets.Add(xMLStreamsheetModel);
			_currentSheet = xMLStreamsheetModel;
			return xMLStreamsheetModel;
		}
	}
}

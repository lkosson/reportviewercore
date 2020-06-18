using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal interface IWorksheetsModel : IEnumerable<IWorksheetModel>, IEnumerable
	{
		Worksheets Interface
		{
			get;
		}

		int Count
		{
			get;
		}

		IWorksheetModel GetWorksheet(int position);

		int getSheetPosition(string name);

		IStreamsheetModel CreateStreamsheet(string sheetName, ExcelGeneratorConstants.CreateTempStream createTempStream);
	}
}

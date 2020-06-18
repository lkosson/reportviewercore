using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal interface IBorderPartModel
	{
		ColorModel Color
		{
			set;
		}

		ST_BorderStyle Style
		{
			set;
		}

		bool HasBeenModified
		{
			get;
		}
	}
}

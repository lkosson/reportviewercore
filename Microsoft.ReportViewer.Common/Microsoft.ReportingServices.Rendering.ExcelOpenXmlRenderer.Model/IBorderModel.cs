using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal interface IBorderModel
	{
		bool HasBeenModified
		{
			get;
		}

		XMLBorderPartModel TopBorder
		{
			get;
		}

		XMLBorderPartModel BottomBorder
		{
			get;
		}

		XMLBorderPartModel LeftBorder
		{
			get;
		}

		XMLBorderPartModel RightBorder
		{
			get;
		}

		XMLBorderPartModel DiagonalBorder
		{
			get;
		}

		ExcelBorderPart DiagonalPartDirection
		{
			set;
		}
	}
}

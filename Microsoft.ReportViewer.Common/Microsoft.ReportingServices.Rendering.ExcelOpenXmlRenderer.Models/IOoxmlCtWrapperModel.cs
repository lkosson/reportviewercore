using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Models
{
	internal interface IOoxmlCtWrapperModel
	{
		OoxmlComplexType OoxmlTag
		{
			get;
		}

		void Cleanup();
	}
}

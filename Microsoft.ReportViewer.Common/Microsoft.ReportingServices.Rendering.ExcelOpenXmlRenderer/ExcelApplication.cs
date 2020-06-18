using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal class ExcelApplication
	{
		public virtual Workbook CreateStreaming(Stream outputStream)
		{
			return new Workbook(new XMLStreambookModel(outputStream));
		}

		public virtual void Save(Workbook workbook)
		{
			workbook.Model.Save();
		}
	}
}

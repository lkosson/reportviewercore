using System.IO.Packaging;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal interface IStreambookModel : IWorkbookModel
	{
		Package ZipPackage
		{
			get;
		}

		void Save();
	}
}

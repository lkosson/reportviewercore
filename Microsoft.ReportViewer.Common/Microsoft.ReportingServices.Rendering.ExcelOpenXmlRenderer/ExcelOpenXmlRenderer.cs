using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.ExcelRenderer;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using System.IO;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal class ExcelOpenXmlRenderer : Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelRenderer
	{
		public override string LocalizedName => ExcelRenderRes.ExcelOoxmlLocalizedName;

		protected override Stream CreateFinalOutputStream(string name, CreateAndRegisterStream createAndRegisterStream)
		{
			return createAndRegisterStream(name, "xlsx", null, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", willSeek: false, StreamOper.CreateAndRegister);
		}

		internal override IExcelGenerator CreateExcelGenerator(ExcelGeneratorConstants.CreateTempStream createTempStream)
		{
			return new OpenXmlGenerator(createTempStream);
		}
	}
}

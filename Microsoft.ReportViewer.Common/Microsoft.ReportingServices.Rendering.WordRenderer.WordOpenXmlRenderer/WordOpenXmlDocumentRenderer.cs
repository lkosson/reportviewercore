using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.SPBProcessing;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer
{
	internal class WordOpenXmlDocumentRenderer : WordDocumentRendererBase
	{
		public override string LocalizedName => WordRenderRes.WordOpenXmlLocalizedName;

		internal override IWordWriter NewWordWriter()
		{
			return new WordOpenXmlWriter();
		}

		protected override WordRenderer NewWordRenderer(CreateAndRegisterStream createAndRegisterStream, DeviceInfo deviceInfoObj, Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing spbProcessing, IWordWriter writer, string reportName)
		{
			return new WordOpenXmlRenderer(createAndRegisterStream, spbProcessing, writer, deviceInfoObj, reportName);
		}
	}
}

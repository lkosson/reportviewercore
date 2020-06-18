using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.SPBProcessing;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class WordDocumentRenderer : WordDocumentRendererBase
	{
		internal override IWordWriter NewWordWriter()
		{
			return new Word97Writer();
		}

		protected override WordRenderer NewWordRenderer(CreateAndRegisterStream createAndRegisterStream, DeviceInfo deviceInfoObj, Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing spbProcessing, IWordWriter writer, string reportName)
		{
			return new Word97Renderer(createAndRegisterStream, spbProcessing, writer, deviceInfoObj, reportName);
		}
	}
}

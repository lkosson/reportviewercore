using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal interface UrlWriter
	{
		void WriteImage(Stream destinationStream, RPLImageData image);
	}
}

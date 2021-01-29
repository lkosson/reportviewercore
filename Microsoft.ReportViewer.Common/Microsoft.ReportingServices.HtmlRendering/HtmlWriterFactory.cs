using Microsoft.ReportingServices.Interfaces;
using System.IO;
using System.Text;
using System.Web.UI;

namespace Microsoft.ReportingServices.HtmlRendering
{
	internal static class HtmlWriterFactory
	{
		public static HtmlTextWriter CreateWriter(string streamName, string mimeType, CreateAndRegisterStream createStreamCallback, StreamOper streamOper)
		{
			Stream stream = createStreamCallback(streamName, "html", Encoding.UTF8, mimeType, willSeek: false, streamOper);
			HtmlTextWriter htmlTextWriter = new HtmlTextWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
			htmlTextWriter.NewLine = null;
			return htmlTextWriter;
		}
	}
}

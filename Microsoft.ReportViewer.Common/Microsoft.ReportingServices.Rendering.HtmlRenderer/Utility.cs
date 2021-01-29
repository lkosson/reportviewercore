using System;
using System.Globalization;
using System.IO;
using System.Web.UI;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class Utility
	{
		internal const int TextBufferSize = 16384;

		private Utility()
		{
		}

		internal static void CopyStream(Stream source, Stream sink)
		{
			byte[] array = new byte[4096];
			for (int num = source.Read(array, 0, array.Length); num != 0; num = source.Read(array, 0, array.Length))
			{
				sink.Write(array, 0, num);
			}
		}

		internal static string MmToPxAsString(double size)
		{
			return Convert.ToInt64(size * 3.7795275590551185).ToString(CultureInfo.InvariantCulture);
		}

		internal static long MMToPx(double size)
		{
			return Convert.ToInt64(size * 3.7795275590551185);
		}

		internal static BufferedStream CreateBufferedStream(HtmlTextWriter sourceWriter)
		{
			return CreateBufferedStream(sourceWriter.BaseStream);
		}

		internal static BufferedStream CreateBufferedStream(Stream sourceStream)
		{
			return new BufferedStream(sourceStream, 16384);
		}
	}
}

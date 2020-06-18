using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal static class SupportClass
	{
		public static long CopyStream(Stream from, Stream to)
		{
			byte[] buffer = new byte[4096];
			long num = 0L;
			int num2;
			while ((num2 = from.Read(buffer, 0, 4096)) > 0)
			{
				num += num2;
				to.Write(buffer, 0, num2);
			}
			return num;
		}
	}
}

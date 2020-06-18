using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator
{
	internal static class ExcelGeneratorConstants
	{
		internal delegate Stream CreateTempStream(string name);

		internal const int EASTASIACHAR_RANGE1_START = 4352;

		internal const int EASTASIACHAR_RANGE1_END = 4607;

		internal const int EASTASIACHAR_RANGE2_START = 11904;

		internal const int EASTASIACHAR_RANGE2_END = 55215;

		internal const int EASTASIACHAR_RANGE3_START = 63744;

		internal const int EASTASIACHAR_RANGE3_END = 65519;

		internal const int EASTASIACHAR_RANGE4_START = 55296;

		internal const int EASTASIACHAR_RANGE4_END = 56319;

		internal const int MAX_LENGTH_SPREADSHEET_NAME = 31;

		internal const string UNSUPPORTED_WORKSHEETNAME_CHARS = "[]:?*/\\";

		internal const int MAX_COLORS_IN_PALETTE = 56;

		public const string BMP_EXTENSION = "bmp";

		public const string JPEG_EXTENSION = "jpg";

		public const string GIF_EXTENSION = "gif";

		public const string PNG_EXTENSION = "png";

		internal const int MAX_STRING_LENGTH = 32767;

		internal const int STREAM_COPY_BUFFER_SIZE = 1024;

		internal const int CONTROL_CHARACTER_RANGE_END = 31;
	}
}

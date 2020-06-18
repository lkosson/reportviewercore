using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	[CompilerGenerated]
	internal class WordRenderRes
	{
		[CompilerGenerated]
		public class Keys
		{
			private static ResourceManager resourceManager = new ResourceManager(typeof(WordRenderRes).FullName, typeof(WordRenderRes).Module.Assembly);

			private static CultureInfo _culture = null;

			public const string WordLocalizedName = "WordLocalizedName";

			public const string WordOpenXmlLocalizedName = "WordOpenXmlLocalizedName";

			public const string InvalidPNGError = "InvalidPNGError";

			public const string ColumnsErrorRectangle = "ColumnsErrorRectangle";

			public const string ColumnsErrorBody = "ColumnsErrorBody";

			public const string ColumnsErrorHeaderFooter = "ColumnsErrorHeaderFooter";

			public static CultureInfo Culture
			{
				get
				{
					return _culture;
				}
				set
				{
					_culture = value;
				}
			}

			private Keys()
			{
			}

			public static string GetString(string key)
			{
				return resourceManager.GetString(key, _culture);
			}
		}

		public static CultureInfo Culture
		{
			get
			{
				return Keys.Culture;
			}
			set
			{
				Keys.Culture = value;
			}
		}

		public static string WordLocalizedName => Keys.GetString("WordLocalizedName");

		public static string WordOpenXmlLocalizedName => Keys.GetString("WordOpenXmlLocalizedName");

		public static string InvalidPNGError => Keys.GetString("InvalidPNGError");

		public static string ColumnsErrorRectangle => Keys.GetString("ColumnsErrorRectangle");

		public static string ColumnsErrorBody => Keys.GetString("ColumnsErrorBody");

		public static string ColumnsErrorHeaderFooter => Keys.GetString("ColumnsErrorHeaderFooter");

		protected WordRenderRes()
		{
		}
	}
}

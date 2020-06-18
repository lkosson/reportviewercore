using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer
{
	[CompilerGenerated]
	internal class ExcelRenderRes
	{
		[CompilerGenerated]
		public class Keys
		{
			private static ResourceManager resourceManager = new ResourceManager(typeof(ExcelRenderRes).FullName, typeof(ExcelRenderRes).Module.Assembly);

			private static CultureInfo _culture = null;

			public const string ExcelLocalizedName = "ExcelLocalizedName";

			public const string ExcelOoxmlLocalizedName = "ExcelOoxmlLocalizedName";

			public const string DocumentMap = "DocumentMap";

			public const string SheetName = "SheetName";

			public const string SheetNameCounterSuffix = "SheetNameCounterSuffix";

			public const string ArgumentNullException = "ArgumentNullException";

			public const string ArgumentInvalid = "ArgumentInvalid";

			public const string InvalidIndexException = "InvalidIndexException";

			public const string MaxValueExceeded = "MaxValueExceeded";

			public const string ValueOutOfRange = "ValueOutOfRange";

			public const string MaxRowExceededInSheet = "MaxRowExceededInSheet";

			public const string MaxColExceededInSheet = "MaxColExceededInSheet";

			public const string UnknownImageFormat = "UnknownImageFormat";

			public const string MaxStringLengthExceeded = "MaxStringLengthExceeded";

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

			public static string GetString(string key, object arg0)
			{
				return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), arg0);
			}

			public static string GetString(string key, object arg0, object arg1)
			{
				return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), arg0, arg1);
			}

			public static string GetString(string key, object arg0, object arg1, object arg2)
			{
				return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), arg0, arg1, arg2);
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

		public static string ExcelLocalizedName => Keys.GetString("ExcelLocalizedName");

		public static string ExcelOoxmlLocalizedName => Keys.GetString("ExcelOoxmlLocalizedName");

		public static string DocumentMap => Keys.GetString("DocumentMap");

		public static string SheetName => Keys.GetString("SheetName");

		public static string SheetNameCounterSuffix => Keys.GetString("SheetNameCounterSuffix");

		public static string ArgumentNullException => Keys.GetString("ArgumentNullException");

		public static string ArgumentInvalid => Keys.GetString("ArgumentInvalid");

		protected ExcelRenderRes()
		{
		}

		public static string InvalidIndexException(string index)
		{
			return Keys.GetString("InvalidIndexException", index);
		}

		public static string MaxValueExceeded(string max)
		{
			return Keys.GetString("MaxValueExceeded", max);
		}

		public static string ValueOutOfRange(string min, string max, string value)
		{
			return Keys.GetString("ValueOutOfRange", min, max, value);
		}

		public static string MaxRowExceededInSheet(string rows, string maxRows)
		{
			return Keys.GetString("MaxRowExceededInSheet", rows, maxRows);
		}

		public static string MaxColExceededInSheet(string cols, string maxCols)
		{
			return Keys.GetString("MaxColExceededInSheet", cols, maxCols);
		}

		public static string UnknownImageFormat(string format)
		{
			return Keys.GetString("UnknownImageFormat", format);
		}

		public static string MaxStringLengthExceeded(string row, string col)
		{
			return Keys.GetString("MaxStringLengthExceeded", row, col);
		}
	}
}

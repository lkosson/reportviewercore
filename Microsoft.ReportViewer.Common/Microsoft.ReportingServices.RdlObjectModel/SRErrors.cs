using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	[CompilerGenerated]
	internal class SRErrors
	{
		[CompilerGenerated]
		public class Keys
		{
			private static ResourceManager resourceManager = new ResourceManager(typeof(SRErrors).FullName, typeof(SRErrors).Module.Assembly);

			private static CultureInfo _culture = null;

			public const string InvalidParamGreaterThan = "InvalidParamGreaterThan";

			public const string InvalidParamLessThan = "InvalidParamLessThan";

			public const string InvalidParamBetween = "InvalidParamBetween";

			public const string InvalidParam = "InvalidParam";

			public const string InvalidIdentifier = "InvalidIdentifier";

			public const string UnitParseNumericPart = "UnitParseNumericPart";

			public const string UnitParseNoDigits = "UnitParseNoDigits";

			public const string UnitParseNoUnit = "UnitParseNoUnit";

			public const string TextParseFailedFormat = "TextParseFailedFormat";

			public const string InvalidColor = "InvalidColor";

			public const string InvalidUnitType = "InvalidUnitType";

			public const string InvalidValue = "InvalidValue";

			public const string NoRootElement = "NoRootElement";

			public const string DeserializationFailedMethod = "DeserializationFailedMethod";

			public const string DeserializationFailed = "DeserializationFailed";

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

		public static string NoRootElement => Keys.GetString("NoRootElement");

		protected SRErrors()
		{
		}

		public static string InvalidParamGreaterThan(string name, object min)
		{
			return Keys.GetString("InvalidParamGreaterThan", name, min);
		}

		public static string InvalidParamLessThan(string name, object max)
		{
			return Keys.GetString("InvalidParamLessThan", name, max);
		}

		public static string InvalidParamBetween(string name, object min, object max)
		{
			return Keys.GetString("InvalidParamBetween", name, min, max);
		}

		public static string InvalidParam(string name, object value)
		{
			return Keys.GetString("InvalidParam", name, value);
		}

		public static string InvalidIdentifier(string name)
		{
			return Keys.GetString("InvalidIdentifier", name);
		}

		public static string UnitParseNumericPart(string value, string numericPart, string type)
		{
			return Keys.GetString("UnitParseNumericPart", value, numericPart, type);
		}

		public static string UnitParseNoDigits(string value)
		{
			return Keys.GetString("UnitParseNoDigits", value);
		}

		public static string UnitParseNoUnit(string value)
		{
			return Keys.GetString("UnitParseNoUnit", value);
		}

		public static string TextParseFailedFormat(string text, string format)
		{
			return Keys.GetString("TextParseFailedFormat", text, format);
		}

		public static string InvalidColor(string value)
		{
			return Keys.GetString("InvalidColor", value);
		}

		public static string InvalidUnitType(string value)
		{
			return Keys.GetString("InvalidUnitType", value);
		}

		public static string InvalidValue(string value)
		{
			return Keys.GetString("InvalidValue", value);
		}

		public static string DeserializationFailedMethod(string methodName)
		{
			return Keys.GetString("DeserializationFailedMethod", methodName);
		}

		public static string DeserializationFailed(string message)
		{
			return Keys.GetString("DeserializationFailed", message);
		}
	}
}

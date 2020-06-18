using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	[CompilerGenerated]
	internal class RenderErrors
	{
		[CompilerGenerated]
		public class Keys
		{
			private static ResourceManager resourceManager = new ResourceManager(typeof(RenderErrors).FullName, typeof(RenderErrors).Module.Assembly);

			private static CultureInfo _culture = null;

			public const string rrInvalidPageNumber = "rrInvalidPageNumber";

			public const string rrRenderStyleError = "rrRenderStyleError";

			public const string rrRenderSectionInstanceError = "rrRenderSectionInstanceError";

			public const string rrRenderResultNull = "rrRenderResultNull";

			public const string rrRenderStreamNull = "rrRenderStreamNull";

			public const string rrRenderDeviceNull = "rrRenderDeviceNull";

			public const string rrRenderReportNull = "rrRenderReportNull";

			public const string rrRenderReportNameNull = "rrRenderReportNameNull";

			public const string rrRenderUnknownReportItem = "rrRenderUnknownReportItem";

			public const string rrRenderStyleName = "rrRenderStyleName";

			public const string rrRenderTextBox = "rrRenderTextBox";

			public const string rrRenderingError = "rrRenderingError";

			public const string rrUnexpectedError = "rrUnexpectedError";

			public const string rrControlInvalidTag = "rrControlInvalidTag";

			public const string rrPageNamespaceInvalid = "rrPageNamespaceInvalid";

			public const string rrInvalidAttribute = "rrInvalidAttribute";

			public const string rrInvalidProperty = "rrInvalidProperty";

			public const string rrInvalidStyleName = "rrInvalidStyleName";

			public const string rrInvalidControl = "rrInvalidControl";

			public const string rrParameterExpected = "rrParameterExpected";

			public const string rrExpectedTopLevelElement = "rrExpectedTopLevelElement";

			public const string rrInvalidDeviceInfo = "rrInvalidDeviceInfo";

			public const string rrInvalidParamValue = "rrInvalidParamValue";

			public const string rrExpectedEndElement = "rrExpectedEndElement";

			public const string rrReportNameNull = "rrReportNameNull";

			public const string rrReportParamsNull = "rrReportParamsNull";

			public const string rrRendererParamsNull = "rrRendererParamsNull";

			public const string rrMeasurementUnitError = "rrMeasurementUnitError";

			public const string rrInvalidOWCRequest = "rrInvalidOWCRequest";

			public const string rrInvalidColor = "rrInvalidColor";

			public const string rrInvalidSize = "rrInvalidSize";

			public const string rrInvalidMeasurementUnit = "rrInvalidMeasurementUnit";

			public const string rrNegativeSize = "rrNegativeSize";

			public const string rrOutOfRange = "rrOutOfRange";

			public const string rrInvalidStyleArgumentType = "rrInvalidStyleArgumentType";

			public const string rrInvalidBorderStyle = "rrInvalidBorderStyle";

			public const string rrInvalidUniqueName = "rrInvalidUniqueName";

			public const string rrInvalidActionLabel = "rrInvalidActionLabel";

			public const string rrInvalidMimeType = "rrInvalidMimeType";

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

		public static string rrRenderStyleError => Keys.GetString("rrRenderStyleError");

		public static string rrRenderSectionInstanceError => Keys.GetString("rrRenderSectionInstanceError");

		public static string rrRenderResultNull => Keys.GetString("rrRenderResultNull");

		public static string rrRenderStreamNull => Keys.GetString("rrRenderStreamNull");

		public static string rrRenderDeviceNull => Keys.GetString("rrRenderDeviceNull");

		public static string rrRenderReportNull => Keys.GetString("rrRenderReportNull");

		public static string rrRenderReportNameNull => Keys.GetString("rrRenderReportNameNull");

		public static string rrRenderUnknownReportItem => Keys.GetString("rrRenderUnknownReportItem");

		public static string rrRenderStyleName => Keys.GetString("rrRenderStyleName");

		public static string rrRenderTextBox => Keys.GetString("rrRenderTextBox");

		public static string rrRenderingError => Keys.GetString("rrRenderingError");

		public static string rrUnexpectedError => Keys.GetString("rrUnexpectedError");

		public static string rrControlInvalidTag => Keys.GetString("rrControlInvalidTag");

		public static string rrPageNamespaceInvalid => Keys.GetString("rrPageNamespaceInvalid");

		public static string rrInvalidAttribute => Keys.GetString("rrInvalidAttribute");

		public static string rrInvalidProperty => Keys.GetString("rrInvalidProperty");

		public static string rrInvalidStyleName => Keys.GetString("rrInvalidStyleName");

		public static string rrInvalidControl => Keys.GetString("rrInvalidControl");

		public static string rrParameterExpected => Keys.GetString("rrParameterExpected");

		public static string rrReportNameNull => Keys.GetString("rrReportNameNull");

		public static string rrReportParamsNull => Keys.GetString("rrReportParamsNull");

		public static string rrRendererParamsNull => Keys.GetString("rrRendererParamsNull");

		public static string rrMeasurementUnitError => Keys.GetString("rrMeasurementUnitError");

		public static string rrInvalidOWCRequest => Keys.GetString("rrInvalidOWCRequest");

		public static string rrInvalidUniqueName => Keys.GetString("rrInvalidUniqueName");

		public static string rrInvalidActionLabel => Keys.GetString("rrInvalidActionLabel");

		protected RenderErrors()
		{
		}

		public static string rrInvalidPageNumber(int totalNumPages)
		{
			return Keys.GetString("rrInvalidPageNumber", totalNumPages);
		}

		public static string rrExpectedTopLevelElement(string elementName)
		{
			return Keys.GetString("rrExpectedTopLevelElement", elementName);
		}

		public static string rrInvalidDeviceInfo(string detail)
		{
			return Keys.GetString("rrInvalidDeviceInfo", detail);
		}

		public static string rrInvalidParamValue(string paramName)
		{
			return Keys.GetString("rrInvalidParamValue", paramName);
		}

		public static string rrExpectedEndElement(string elementName)
		{
			return Keys.GetString("rrExpectedEndElement", elementName);
		}

		public static string rrInvalidColor(string color)
		{
			return Keys.GetString("rrInvalidColor", color);
		}

		public static string rrInvalidSize(string size)
		{
			return Keys.GetString("rrInvalidSize", size);
		}

		public static string rrInvalidMeasurementUnit(string size)
		{
			return Keys.GetString("rrInvalidMeasurementUnit", size);
		}

		public static string rrNegativeSize(string size)
		{
			return Keys.GetString("rrNegativeSize", size);
		}

		public static string rrOutOfRange(string size)
		{
			return Keys.GetString("rrOutOfRange", size);
		}

		public static string rrInvalidStyleArgumentType(string argumentType)
		{
			return Keys.GetString("rrInvalidStyleArgumentType", argumentType);
		}

		public static string rrInvalidBorderStyle(string style)
		{
			return Keys.GetString("rrInvalidBorderStyle", style);
		}

		public static string rrInvalidMimeType(string mimeType)
		{
			return Keys.GetString("rrInvalidMimeType", mimeType);
		}
	}
}

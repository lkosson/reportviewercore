using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportRendering;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class ProcessingValidator
	{
		private ProcessingValidator()
		{
		}

		internal static string ValidateColor(string color, IErrorContext errorContext)
		{
			return ValidateColor(color, errorContext, allowTransparency: false);
		}

		internal static string ValidateColor(string color, IErrorContext errorContext, bool allowTransparency)
		{
			if (Validator.ValidateColor(color, out string newColor, allowTransparency))
			{
				return newColor;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidColor, Severity.Warning, color);
			return null;
		}

		internal static string ValidateBorderWidth(string size, IErrorContext errorContext)
		{
			return ValidateSize(size, Validator.BorderWidthMin, Validator.BorderWidthMax, errorContext);
		}

		internal static string ValidateFontSize(string size, IErrorContext errorContext)
		{
			return ValidateSize(size, Validator.FontSizeMin, Validator.FontSizeMax, errorContext);
		}

		internal static string ValidatePadding(string size, IErrorContext errorContext)
		{
			return ValidateSize(size, Validator.PaddingMin, Validator.PaddingMax, errorContext);
		}

		internal static string ValidateLineHeight(string size, IErrorContext errorContext)
		{
			return ValidateSize(size, Validator.LineHeightMin, Validator.LineHeightMax, errorContext);
		}

		private static string ValidateSize(string size, double minValue, double maxValue, IErrorContext errorContext)
		{
			if (!Validator.ValidateSizeString(size, out RVUnit sizeValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidSize, Severity.Warning, size);
				return null;
			}
			if (!Validator.ValidateSizeUnitType(sizeValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidMeasurementUnit, Severity.Warning, sizeValue.Type.ToString());
				return null;
			}
			if (!Validator.ValidateSizeIsPositive(sizeValue))
			{
				errorContext.Register(ProcessingErrorCode.rsNegativeSize, Severity.Warning);
				return null;
			}
			if (!Validator.ValidateSizeValue(Converter.ConvertToMM(sizeValue), minValue, maxValue))
			{
				errorContext.Register(ProcessingErrorCode.rsOutOfRangeSize, Severity.Warning, size, Converter.ConvertSize(minValue), Converter.ConvertSize(maxValue));
				return null;
			}
			return size;
		}

		internal static string ValidateEmbeddedImageName(string embeddedImageName, EmbeddedImageHashtable embeddedImages, IErrorContext errorContext)
		{
			if (embeddedImageName == null)
			{
				return null;
			}
			if (!Validator.ValidateEmbeddedImageName(embeddedImageName, embeddedImages))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEmbeddedImageProperty, Severity.Warning, embeddedImageName);
				return null;
			}
			return embeddedImageName;
		}

		internal static string ValidateEmbeddedImageName(string embeddedImageName, EmbeddedImageHashtable embeddedImages, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			if (embeddedImageName == null)
			{
				return null;
			}
			if (!Validator.ValidateEmbeddedImageName(embeddedImageName, embeddedImages))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEmbeddedImageProperty, Severity.Warning, objectType, objectName, propertyName, embeddedImageName);
				return null;
			}
			return embeddedImageName;
		}

		internal static string ValidateLanguage(string language, IErrorContext errorContext, out CultureInfo culture)
		{
			if (Validator.ValidateLanguage(language, out culture))
			{
				return language;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidLanguage, Severity.Warning, language);
			return null;
		}

		internal static string ValidateSpecificLanguage(string language, IErrorContext errorContext, out CultureInfo culture)
		{
			if (Validator.ValidateSpecificLanguage(language, out culture))
			{
				return language;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidLanguage, Severity.Warning, language);
			return null;
		}

		internal static Calendar CreateCalendar(Calendars calendarType)
		{
			Validator.CreateCalendar(calendarType, out Calendar calendar);
			return calendar;
		}

		internal static Calendar CreateCalendar(string calendarName)
		{
			Validator.CreateCalendar(calendarName, out Calendar calendar);
			return calendar;
		}

		internal static bool ValidateCalendar(CultureInfo language, Calendars calendarType, ObjectType objectType, string ObjectName, string propertyName, ErrorContext errorContext)
		{
			if (!Validator.ValidateCalendar(language, calendarType))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidCalendarForLanguage, Severity.Error, objectType, ObjectName, propertyName, calendarType.ToString(), language.Name);
				return false;
			}
			return true;
		}

		internal static bool ValidateCalendar(CultureInfo language, string calendarName, ObjectType objectType, string ObjectName, string propertyName, ErrorContext errorContext)
		{
			if (!Validator.ValidateCalendar(language, calendarName))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidCalendarForLanguage, Severity.Error, objectType, ObjectName, propertyName, calendarName, language.Name);
				return false;
			}
			return true;
		}

		internal static void ValidateNumeralVariant(CultureInfo language, int numVariant, ObjectType objectType, string ObjectName, string propertyName, ErrorContext errorContext)
		{
			if (!Validator.ValidateNumeralVariant(language, numVariant))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidNumeralVariantForLanguage, Severity.Error, objectType, ObjectName, propertyName, numVariant.ToString(CultureInfo.InvariantCulture), language.Name);
			}
		}

		internal static object ValidateNumeralVariant(int numeralVariant, IErrorContext errorContext)
		{
			if (Validator.ValidateNumeralVariant(numeralVariant))
			{
				return numeralVariant;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidNumeralVariant, Severity.Warning, numeralVariant.ToString(CultureInfo.InvariantCulture));
			return null;
		}

		internal static string ValidateMimeType(string mimeType, IErrorContext errorContext)
		{
			if (Validator.ValidateMimeType(mimeType))
			{
				return mimeType;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidMIMEType, Severity.Warning, mimeType);
			return null;
		}

		internal static string ValidateMimeType(string mimeType, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			if (Validator.ValidateMimeType(mimeType))
			{
				return mimeType;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidMIMEType, Severity.Warning, objectType, objectName, propertyName, mimeType);
			return null;
		}

		internal static string ValidateBorderStyle(string borderStyle, ObjectType objectType, IErrorContext errorContext)
		{
			if (Validator.ValidateBorderStyle(borderStyle, out string borderStyleForLine))
			{
				if (ObjectType.Line == objectType)
				{
					return borderStyleForLine;
				}
				return borderStyle;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidBorderStyle, Severity.Warning, borderStyle);
			return null;
		}

		internal static string ValidateBackgroundGradientType(string gradientType, IErrorContext errorContext)
		{
			if (Validator.ValidateBackgroundGradientType(gradientType))
			{
				return gradientType;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidBackgroundGradientType, Severity.Warning, gradientType);
			return null;
		}

		internal static string ValidateBackgroundRepeat(string repeat, IErrorContext errorContext)
		{
			if (Validator.ValidateBackgroundRepeat(repeat))
			{
				return repeat;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidBackgroundRepeat, Severity.Warning, repeat);
			return null;
		}

		internal static string ValidateFontStyle(string fontStyle, IErrorContext errorContext)
		{
			if (Validator.ValidateFontStyle(fontStyle))
			{
				return fontStyle;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidFontStyle, Severity.Warning, fontStyle);
			return null;
		}

		internal static string ValidateFontWeight(string fontWeight, IErrorContext errorContext)
		{
			if (Validator.ValidateFontWeight(fontWeight))
			{
				return fontWeight;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidFontWeight, Severity.Warning, fontWeight);
			return null;
		}

		internal static string ValidateTextDecoration(string textDecoration, IErrorContext errorContext)
		{
			if (Validator.ValidateTextDecoration(textDecoration))
			{
				return textDecoration;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidTextDecoration, Severity.Warning, textDecoration);
			return null;
		}

		internal static string ValidateTextAlign(string textAlign, IErrorContext errorContext)
		{
			if (Validator.ValidateTextAlign(textAlign))
			{
				return textAlign;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidTextAlign, Severity.Warning, textAlign);
			return null;
		}

		internal static string ValidateVerticalAlign(string verticalAlign, IErrorContext errorContext)
		{
			if (Validator.ValidateVerticalAlign(verticalAlign))
			{
				return verticalAlign;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidVerticalAlign, Severity.Warning, verticalAlign);
			return null;
		}

		internal static string ValidateDirection(string direction, IErrorContext errorContext)
		{
			if (Validator.ValidateDirection(direction))
			{
				return direction;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidDirection, Severity.Warning, direction);
			return null;
		}

		internal static string ValidateWritingMode(string writingMode, IErrorContext errorContext)
		{
			if (Validator.ValidateWritingMode(writingMode))
			{
				return writingMode;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidWritingMode, Severity.Warning, writingMode);
			return null;
		}

		internal static string ValidateUnicodeBiDi(string unicodeBiDi, IErrorContext errorContext)
		{
			if (Validator.ValidateUnicodeBiDi(unicodeBiDi))
			{
				return unicodeBiDi;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidUnicodeBiDi, Severity.Warning, unicodeBiDi);
			return null;
		}

		internal static string ValidateCalendar(string calendar, IErrorContext errorContext)
		{
			if (Validator.ValidateCalendar(calendar))
			{
				return calendar;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidCalendar, Severity.Warning, calendar);
			return null;
		}

		internal static object ValidateCustomStyle(string styleName, object styleValue, IErrorContext errorContext)
		{
			return ValidateCustomStyle(styleName, styleValue, ObjectType.Image, errorContext);
		}

		internal static object ValidateCustomStyle(string styleName, object styleValue, ObjectType objectType, IErrorContext errorContext)
		{
			CultureInfo culture;
			switch (styleName)
			{
			case "BorderColor":
			case "BorderColorLeft":
			case "BorderColorRight":
			case "BorderColorTop":
			case "BorderColorBottom":
				return ValidateColor(styleValue as string, errorContext, objectType == ObjectType.Chart);
			case "BorderStyle":
			case "BorderStyleLeft":
			case "BorderStyleRight":
			case "BorderStyleTop":
			case "BorderStyleBottom":
				return ValidateBorderStyle(styleValue as string, objectType, errorContext);
			case "BorderWidth":
			case "BorderWidthLeft":
			case "BorderWidthRight":
			case "BorderWidthTop":
			case "BorderWidthBottom":
				return ValidateSize((styleValue as Microsoft.ReportingServices.ReportRendering.ReportSize).ToString(), Validator.BorderWidthMin, Validator.BorderWidthMax, errorContext);
			case "Color":
			case "BackgroundColor":
			case "BackgroundGradientEndColor":
				return ValidateColor(styleValue as string, errorContext, objectType == ObjectType.Chart);
			case "BackgroundGradientType":
				return ValidateBackgroundGradientType(styleValue as string, errorContext);
			case "FontStyle":
				return ValidateFontStyle(styleValue as string, errorContext);
			case "FontFamily":
				return styleValue as string;
			case "FontSize":
				return ValidateSize((styleValue as Microsoft.ReportingServices.ReportRendering.ReportSize).ToString(), Validator.FontSizeMin, Validator.FontSizeMax, errorContext);
			case "FontWeight":
				return ValidateFontWeight(styleValue as string, errorContext);
			case "Format":
				return styleValue as string;
			case "TextDecoration":
				return ValidateTextDecoration(styleValue as string, errorContext);
			case "TextAlign":
				return ValidateTextAlign(styleValue as string, errorContext);
			case "VerticalAlign":
				return ValidateVerticalAlign(styleValue as string, errorContext);
			case "PaddingLeft":
			case "PaddingRight":
			case "PaddingTop":
			case "PaddingBottom":
				return ValidateSize((styleValue as Microsoft.ReportingServices.ReportRendering.ReportSize).ToString(), Validator.PaddingMin, Validator.PaddingMax, errorContext);
			case "LineHeight":
				return ValidateSize((styleValue as Microsoft.ReportingServices.ReportRendering.ReportSize).ToString(), Validator.LineHeightMin, Validator.LineHeightMax, errorContext);
			case "Direction":
				return ValidateDirection(styleValue as string, errorContext);
			case "WritingMode":
				return ValidateWritingMode(styleValue as string, errorContext);
			case "Language":
				return ValidateSpecificLanguage(styleValue as string, errorContext, out culture);
			case "UnicodeBiDi":
				return ValidateUnicodeBiDi(styleValue as string, errorContext);
			case "Calendar":
				return ValidateCalendar(styleValue as string, errorContext);
			case "NumeralLanguage":
				return ValidateLanguage(styleValue as string, errorContext, out culture);
			case "NumeralVariant":
			{
				if (int.TryParse(styleValue as string, out int result))
				{
					return ValidateNumeralVariant(result, errorContext);
				}
				errorContext.Register(ProcessingErrorCode.rsInvalidNumeralVariant, Severity.Warning, styleValue as string);
				return null;
			}
			default:
				Global.Tracer.Assert(condition: false);
				break;
			case "BackgroundImageSource":
			case "BackgroundImageValue":
			case "BackgroundImageMIMEType":
			case "BackgroundRepeat":
				break;
			}
			return null;
		}
	}
}

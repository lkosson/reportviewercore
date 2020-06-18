using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal sealed class ProcessingValidator
	{
		private ProcessingValidator()
		{
		}

		internal static string ValidateColor(string color, IErrorContext errorContext, bool allowTransparency)
		{
			if (color == null)
			{
				return null;
			}
			if (Validator.ValidateColor(color, out string newColor, allowTransparency))
			{
				return newColor;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidColor, Severity.Warning, color);
			return null;
		}

		internal static string ValidateSize(string size, IErrorContext errorContext)
		{
			return ValidateSize(size, allowNegative: false, errorContext);
		}

		internal static string ValidateSize(string size, bool allowNegative, IErrorContext errorContext)
		{
			return ValidateSize(size, double.MinValue, double.MaxValue, allowNegative, errorContext);
		}

		internal static string ValidateBorderWidth(string size, IErrorContext errorContext)
		{
			return ValidateSize(size, Validator.BorderWidthMin, Validator.BorderWidthMax, allowNegative: false, errorContext);
		}

		internal static string ValidateFontSize(string size, IErrorContext errorContext)
		{
			return ValidateSize(size, Validator.FontSizeMin, Validator.FontSizeMax, allowNegative: false, errorContext);
		}

		internal static string ValidatePadding(string size, IErrorContext errorContext)
		{
			return ValidateSize(size, Validator.PaddingMin, Validator.PaddingMax, allowNegative: false, errorContext);
		}

		internal static string ValidateLineHeight(string size, IErrorContext errorContext)
		{
			return ValidateSize(size, Validator.LineHeightMin, Validator.LineHeightMax, allowNegative: false, errorContext);
		}

		private static string ValidateSize(string size, double minValue, double maxValue, bool allowNegative, IErrorContext errorContext)
		{
			if (size == null)
			{
				return null;
			}
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
			if (!allowNegative && !Validator.ValidateSizeIsPositive(sizeValue))
			{
				errorContext.Register(ProcessingErrorCode.rsNegativeSize, Severity.Warning);
				return null;
			}
			if (!Validator.ValidateSizeValue(Converter.ConvertToMM(sizeValue), minValue, maxValue))
			{
				errorContext.Register(ProcessingErrorCode.rsOutOfRangeSize, Severity.Warning, size, Converter.ConvertSizeFromMM(minValue, sizeValue.Type), Converter.ConvertSizeFromMM(maxValue, sizeValue.Type));
				return null;
			}
			return size;
		}

		internal static string ValidateEmbeddedImageName(string embeddedImageName, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo> embeddedImages, IErrorContext errorContext)
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

		internal static string ValidateEmbeddedImageName(string embeddedImageName, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo> embeddedImages, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
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

		internal static string ValidateBackgroundHatchType(string backgroundHatchType, IErrorContext errorContext)
		{
			if (Validator.ValidateBackgroundHatchType(backgroundHatchType))
			{
				return backgroundHatchType;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidBackgroundHatchType, Severity.Warning, backgroundHatchType);
			return null;
		}

		internal static string ValidateTextEffect(string textEffect, IErrorContext errorContext)
		{
			if (Validator.ValidateTextEffect(textEffect))
			{
				return textEffect;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidTextEffect, Severity.Warning, textEffect);
			return null;
		}

		internal static string ValidateBorderStyle(string borderStyle, ObjectType objectType, bool isDynamicImageSubElement, IErrorContext errorContext, bool isDefaultBorder)
		{
			if (!Validator.ValidateBorderStyle(borderStyle, isDefaultBorder, objectType, isDynamicImageSubElement, out string validatedStyle))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidBorderStyle, Severity.Warning, borderStyle);
				return null;
			}
			return validatedStyle;
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
				return ValidateColor(styleValue as string, errorContext, Validator.IsDynamicImageReportItem(objectType));
			case "BorderStyle":
				return ValidateBorderStyle(styleValue as string, objectType, isDynamicImageSubElement: false, errorContext, isDefaultBorder: true);
			case "BorderStyleLeft":
			case "BorderStyleRight":
			case "BorderStyleTop":
			case "BorderStyleBottom":
				return ValidateBorderStyle(styleValue as string, objectType, isDynamicImageSubElement: false, errorContext, isDefaultBorder: false);
			case "BorderWidth":
			case "BorderWidthLeft":
			case "BorderWidthRight":
			case "BorderWidthTop":
			case "BorderWidthBottom":
				return ValidateSize((styleValue as ReportSize).ToString(), Validator.BorderWidthMin, Validator.BorderWidthMax, allowNegative: false, errorContext);
			case "Color":
			case "BackgroundColor":
			case "BackgroundGradientEndColor":
				return ValidateColor(styleValue as string, errorContext, Validator.IsDynamicImageReportItem(objectType));
			case "BackgroundGradientType":
				return ValidateBackgroundGradientType(styleValue as string, errorContext);
			case "FontStyle":
				return ValidateFontStyle(styleValue as string, errorContext);
			case "FontFamily":
				return styleValue as string;
			case "FontSize":
				return ValidateSize((styleValue as ReportSize).ToString(), Validator.FontSizeMin, Validator.FontSizeMax, allowNegative: false, errorContext);
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
				return ValidateSize((styleValue as ReportSize).ToString(), Validator.PaddingMin, Validator.PaddingMax, allowNegative: false, errorContext);
			case "LineHeight":
				return ValidateSize((styleValue as ReportSize).ToString(), Validator.LineHeightMin, Validator.LineHeightMax, allowNegative: false, errorContext);
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
			case "CurrencyLanguage":
				return ValidateLanguage(styleValue as string, errorContext, out culture);
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

		internal static string ValidateTextRunMarkupType(string value, IErrorContext errorContext)
		{
			if (Validator.ValidateTextRunMarkupType(value))
			{
				return value;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidMarkupType, Severity.Warning, value);
			return "None";
		}

		internal static string ValidateParagraphListStyle(string value, IErrorContext errorContext)
		{
			if (Validator.ValidateParagraphListStyle(value))
			{
				return value;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidListStyle, Severity.Warning, value);
			return "None";
		}

		internal static int? ValidateParagraphListLevel(int value, IErrorContext errorContext)
		{
			if (!Validator.ValidateParagraphListLevel(value, out int? adjustedValue))
			{
				errorContext.Register(ProcessingErrorCode.rsOutOfRangeSize, Severity.Warning, Convert.ToString(value, CultureInfo.InvariantCulture), Convert.ToString(0, CultureInfo.InvariantCulture), Convert.ToString(9, CultureInfo.InvariantCulture));
				return adjustedValue;
			}
			return value;
		}
	}
}

using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal sealed class PublishingValidator
	{
		private static readonly string m_invalidCharacters = ";?:@&=+$,\\*<>|\"";

		private PublishingValidator()
		{
		}

		internal static bool ValidateColor(StyleInformation.StyleInformationAttribute colorAttribute, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			if (colorAttribute.ValueType == Microsoft.ReportingServices.ReportIntermediateFormat.ValueType.ThemeReference)
			{
				string stringValue = colorAttribute.Value.StringValue;
				if (string.IsNullOrEmpty(stringValue))
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidColor, Severity.Error, objectType, objectName, propertyName, stringValue);
					return false;
				}
				return true;
			}
			return ValidateColor(colorAttribute.Value, objectType, objectName, propertyName, errorContext);
		}

		internal static bool ValidateColor(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo color, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(color != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == color.Type)
			{
				if (!Validator.ValidateColor(color.StringValue, out string newColor, Validator.IsDynamicImageReportItem(objectType)))
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidColor, Severity.Error, objectType, objectName, propertyName, color.StringValue);
					return false;
				}
				color.StringValue = newColor;
			}
			return true;
		}

		internal static void ValidateBorderColorNotTransparent(ObjectType objectType, string objectName, Microsoft.ReportingServices.ReportIntermediateFormat.Style styleClass, string styleName, ErrorContext errorContext)
		{
			if (styleClass.GetAttributeInfo(styleName, out Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo styleAttribute) && !styleAttribute.IsExpression && ReportColor.TryParse(styleAttribute.Value, allowTransparency: true, out ReportColor reportColor) && reportColor.ToColor().A != byte.MaxValue)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidColor, Severity.Error, objectType, objectName, styleName, styleAttribute.Value);
			}
		}

		internal static bool ValidateSize(string size, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			double validSizeInMM;
			string newSize;
			return ValidateSize(size, allowNegative: false, Validator.NegativeMin, Validator.NormalMax, objectType, objectName, propertyName, errorContext, out validSizeInMM, out newSize);
		}

		internal static bool ValidateSize(string size, ObjectType objectType, string objectName, string propertyName, bool restrictMaxValue, ErrorContext errorContext, out double sizeInMM, out string roundSize)
		{
			bool allowNegative = ObjectType.Line == objectType;
			return ValidateSize(size, objectType, objectName, propertyName, restrictMaxValue, allowNegative, errorContext, out sizeInMM, out roundSize);
		}

		internal static bool ValidateSize(string size, ObjectType objectType, string objectName, string propertyName, bool restrictMaxValue, bool allowNegative, ErrorContext errorContext, out double sizeInMM, out string roundSize)
		{
			double minValue = allowNegative ? Validator.NegativeMin : Validator.NormalMin;
			double maxValue = restrictMaxValue ? Validator.NormalMax : double.MaxValue;
			return ValidateSize(size, allowNegative, minValue, maxValue, objectType, objectName, propertyName, errorContext, out sizeInMM, out roundSize);
		}

		internal static bool ValidateSize(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo size, double minValue, double maxValue, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(size != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == size.Type)
			{
				bool allowNegative = false;
				double validSizeInMM;
				string newSize;
				return ValidateSize(size.StringValue, allowNegative, minValue, maxValue, objectType, objectName, propertyName, errorContext, out validSizeInMM, out newSize);
			}
			return true;
		}

		internal static bool ValidateSize(string size, bool allowNegative, double minValue, double maxValue, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext, out double validSizeInMM, out string newSize)
		{
			validSizeInMM = minValue;
			newSize = minValue + "mm";
			if (!Validator.ValidateSizeString(size, out RVUnit sizeValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidSize, Severity.Error, objectType, objectName, propertyName, size);
				return false;
			}
			if (!Validator.ValidateSizeUnitType(sizeValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidMeasurementUnit, Severity.Error, objectType, objectName, propertyName, sizeValue.Type.ToString());
				return false;
			}
			if (!allowNegative && !Validator.ValidateSizeIsPositive(sizeValue))
			{
				errorContext.Register(ProcessingErrorCode.rsNegativeSize, Severity.Error, objectType, objectName, propertyName);
				return false;
			}
			double num = Converter.ConvertToMM(sizeValue);
			if (!Validator.ValidateSizeValue(num, minValue, maxValue))
			{
				errorContext.Register(ProcessingErrorCode.rsOutOfRangeSize, Severity.Error, objectType, objectName, propertyName, size, Converter.ConvertSizeFromMM(allowNegative ? minValue : Math.Max(0.0, minValue), sizeValue.Type), Converter.ConvertSizeFromMM(maxValue, sizeValue.Type));
				return false;
			}
			validSizeInMM = num;
			newSize = sizeValue.ToString(CultureInfo.InvariantCulture);
			return true;
		}

		internal static bool ValidateEmbeddedImageName(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo embeddedImageName, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo> embeddedImages, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(embeddedImageName != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == embeddedImageName.Type)
			{
				return ValidateEmbeddedImageName(embeddedImageName.StringValue, embeddedImages, objectType, objectName, propertyName, errorContext);
			}
			return true;
		}

		internal static bool ValidateEmbeddedImageName(Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo embeddedImageName, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo> embeddedImages, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(embeddedImageName != null);
			if (!embeddedImageName.IsExpression)
			{
				return ValidateEmbeddedImageName(embeddedImageName.Value, embeddedImages, objectType, objectName, propertyName, errorContext);
			}
			return true;
		}

		private static bool ValidateEmbeddedImageName(string embeddedImageName, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo> embeddedImages, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			if (!Validator.ValidateEmbeddedImageName(embeddedImageName, embeddedImages))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEmbeddedImageProperty, Severity.Error, objectType, objectName, propertyName, embeddedImageName);
				return false;
			}
			return true;
		}

		internal static bool ValidateLanguage(string languageValue, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext, out CultureInfo culture)
		{
			culture = null;
			Global.Tracer.Assert(languageValue != null);
			if (!Validator.ValidateLanguage(languageValue, out culture))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidLanguage, Severity.Error, objectType, objectName, propertyName, languageValue);
				return false;
			}
			return true;
		}

		internal static bool ValidateLanguage(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo language, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(language != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == language.Type)
			{
				CultureInfo culture = null;
				if (!Validator.ValidateLanguage(language.StringValue, out culture))
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidLanguage, Severity.Error, objectType, objectName, propertyName, language.StringValue);
					return false;
				}
			}
			return true;
		}

		internal static bool ValidateSpecificLanguage(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo language, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext, out CultureInfo culture)
		{
			culture = null;
			Global.Tracer.Assert(language != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == language.Type && !Validator.ValidateSpecificLanguage(language.StringValue, out culture))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidLanguage, Severity.Error, objectType, objectName, propertyName, language.StringValue);
				return false;
			}
			return true;
		}

		internal static bool ValidateColumns(int columns, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext, int sectionNumber)
		{
			if (!Validator.ValidateColumns(columns))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidColumnsInReportSection, Severity.Error, objectType, objectName, propertyName, sectionNumber.ToString(CultureInfo.InvariantCulture));
				return false;
			}
			return true;
		}

		private static bool ValidateNumeralVariant(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo numeralVariant, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(numeralVariant != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == numeralVariant.Type && !Validator.ValidateNumeralVariant(numeralVariant.IntValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidNumeralVariant, Severity.Error, objectType, objectName, propertyName, numeralVariant.IntValue.ToString(CultureInfo.InvariantCulture));
				return false;
			}
			return true;
		}

		internal static bool ValidateMimeType(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo mimeType, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			if (mimeType == null)
			{
				errorContext.Register(ProcessingErrorCode.rsMissingMIMEType, Severity.Error, objectType, objectName, propertyName);
				return false;
			}
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == mimeType.Type)
			{
				return ValidateMimeType(mimeType.StringValue, objectType, objectName, propertyName, errorContext);
			}
			return true;
		}

		internal static bool ValidateMimeType(string mimeType, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			if (!Validator.ValidateMimeType(mimeType))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidMIMEType, Severity.Error, objectType, objectName, propertyName, mimeType);
				return false;
			}
			return true;
		}

		private static bool ValidateTextEffect(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo textEffect, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(textEffect != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == textEffect.Type && !Validator.ValidateTextEffect(textEffect.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidTextEffect, Severity.Error, objectType, objectName, propertyName, textEffect.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateBackgroundHatchType(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo backgroundHatchType, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(backgroundHatchType != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == backgroundHatchType.Type && !Validator.ValidateBackgroundHatchType(backgroundHatchType.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidBackgroundHatchType, Severity.Error, objectType, objectName, propertyName, backgroundHatchType.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidatePosition(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo position, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(position != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == position.Type && !Validator.ValidatePosition(position.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidBackgroundImagePosition, Severity.Error, objectType, objectName, propertyName, position.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateBorderStyle(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo borderStyle, ObjectType objectType, string objectName, bool isDynamicElementSubElement, string propertyName, bool isDefaultBorder, ErrorContext errorContext)
		{
			Global.Tracer.Assert(borderStyle != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == borderStyle.Type)
			{
				if (!Validator.ValidateBorderStyle(borderStyle.StringValue, isDefaultBorder, objectType, isDynamicElementSubElement, out string validatedStyle))
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidBorderStyle, Severity.Error, objectType, objectName, propertyName, borderStyle.StringValue);
					return false;
				}
				borderStyle.StringValue = validatedStyle;
			}
			return true;
		}

		private static bool ValidateBackgroundGradientType(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo repeat, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(repeat != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == repeat.Type && !Validator.ValidateBackgroundGradientType(repeat.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidBackgroundGradientType, Severity.Error, objectType, objectName, propertyName, repeat.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateBackgroundRepeat(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo repeat, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(repeat != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == repeat.Type && !Validator.ValidateBackgroundRepeat(repeat.StringValue, objectType))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidBackgroundRepeat, Severity.Error, objectType, objectName, propertyName, repeat.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateTransparency(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo transparency, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(transparency != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == transparency.Type)
			{
				double floatValue = transparency.FloatValue;
				if (floatValue < 0.0 || floatValue > 100.0)
				{
					errorContext.Register(ProcessingErrorCode.rsOutOfRangeSize, Severity.Error, objectType, objectName, propertyName, transparency.OriginalText, "0", "100");
					return false;
				}
			}
			return true;
		}

		private static bool ValidateFontStyle(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo fontStyle, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(fontStyle != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == fontStyle.Type && !Validator.ValidateFontStyle(fontStyle.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidFontStyle, Severity.Error, objectType, objectName, propertyName, fontStyle.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateFontWeight(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo fontWeight, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(fontWeight != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == fontWeight.Type && !Validator.ValidateFontWeight(fontWeight.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidFontWeight, Severity.Error, objectType, objectName, propertyName, fontWeight.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateTextDecoration(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo textDecoration, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(textDecoration != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == textDecoration.Type && !Validator.ValidateTextDecoration(textDecoration.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidTextDecoration, Severity.Error, objectType, objectName, propertyName, textDecoration.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateTextAlign(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo textAlign, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(textAlign != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == textAlign.Type && !Validator.ValidateTextAlign(textAlign.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidTextAlign, Severity.Error, objectType, objectName, propertyName, textAlign.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateVerticalAlign(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo verticalAlign, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(verticalAlign != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == verticalAlign.Type && !Validator.ValidateVerticalAlign(verticalAlign.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidVerticalAlign, Severity.Error, objectType, objectName, propertyName, verticalAlign.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateDirection(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo direction, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(direction != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == direction.Type && !Validator.ValidateDirection(direction.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidDirection, Severity.Error, objectType, objectName, propertyName, direction.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateWritingMode(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo writingMode, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(writingMode != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == writingMode.Type && !Validator.ValidateWritingMode(writingMode.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidWritingMode, Severity.Error, objectType, objectName, propertyName, writingMode.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateUnicodeBiDi(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo unicodeBiDi, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(unicodeBiDi != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == unicodeBiDi.Type && !Validator.ValidateUnicodeBiDi(unicodeBiDi.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidUnicodeBiDi, Severity.Error, objectType, objectName, propertyName, unicodeBiDi.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateCalendar(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo calendar, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(calendar != null);
			if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == calendar.Type && !Validator.ValidateCalendar(calendar.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidCalendar, Severity.Error, objectType, objectName, propertyName, calendar.StringValue);
				return false;
			}
			return true;
		}

		private static void ValidateBackgroundImage(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo backgroundImageSource, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo backgroundImageValue, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo backgroundImageMIMEType, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo backgroundEmbeddingMode, Microsoft.ReportingServices.ReportIntermediateFormat.Style style, ObjectType objectType, string objectName, ErrorContext errorContext)
		{
			if (backgroundImageSource == null)
			{
				return;
			}
			bool flag = true;
			Global.Tracer.Assert(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == backgroundImageSource.Type);
			Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType intValue = (Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType)backgroundImageSource.IntValue;
			Global.Tracer.Assert(backgroundImageValue != null);
			if (Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.Database == intValue && Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == backgroundImageValue.Type)
			{
				errorContext.Register(ProcessingErrorCode.rsBinaryConstant, Severity.Error, objectType, objectName, "BackgroundImageValue");
				flag = false;
			}
			if (Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.Database == intValue && !ValidateMimeType(backgroundImageMIMEType, objectType, objectName, "BackgroundImageMIMEType", errorContext))
			{
				flag = false;
			}
			if (flag)
			{
				style.AddAttribute("BackgroundImageSource", backgroundImageSource);
				style.AddAttribute("BackgroundImageValue", backgroundImageValue);
				if (backgroundEmbeddingMode != null)
				{
					style.AddAttribute("EmbeddingMode", backgroundEmbeddingMode);
				}
				if (Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.Database == intValue)
				{
					style.AddAttribute("BackgroundImageMIMEType", backgroundImageMIMEType);
				}
			}
		}

		internal static Microsoft.ReportingServices.ReportIntermediateFormat.Style ValidateAndCreateStyle(List<StyleInformation.StyleInformationAttribute> attributes, ObjectType objectType, string objectName, ErrorContext errorContext)
		{
			bool meDotValueReferenced;
			return ValidateAndCreateStyle(attributes, objectType, objectName, isDynamicImageSubElement: true, errorContext, checkForMeDotValue: false, out meDotValueReferenced);
		}

		internal static Microsoft.ReportingServices.ReportIntermediateFormat.Style ValidateAndCreateStyle(List<StyleInformation.StyleInformationAttribute> attributes, ObjectType objectType, string objectName, bool isDynamicImageSubElement, ErrorContext errorContext)
		{
			bool meDotValueReferenced;
			return ValidateAndCreateStyle(attributes, objectType, objectName, isDynamicImageSubElement, errorContext, checkForMeDotValue: false, out meDotValueReferenced);
		}

		internal static Microsoft.ReportingServices.ReportIntermediateFormat.Style ValidateAndCreateStyle(List<StyleInformation.StyleInformationAttribute> attributes, ObjectType objectType, string objectName, ErrorContext errorContext, bool checkForMeDotValue, out bool meDotValueReferenced)
		{
			return ValidateAndCreateStyle(attributes, objectType, objectName, isDynamicImageSubElement: false, errorContext, checkForMeDotValue, out meDotValueReferenced);
		}

		internal static Microsoft.ReportingServices.ReportIntermediateFormat.Style ValidateAndCreateStyle(List<StyleInformation.StyleInformationAttribute> attributes, ObjectType objectType, string objectName, bool isDynamicImageSubElement, ErrorContext errorContext, bool checkForMeDotValue, out bool meDotValueReferenced)
		{
			meDotValueReferenced = false;
			Microsoft.ReportingServices.ReportIntermediateFormat.Style style = new Microsoft.ReportingServices.ReportIntermediateFormat.Style(Microsoft.ReportingServices.ReportIntermediateFormat.ConstructionPhase.Publishing);
			Global.Tracer.Assert(attributes != null);
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo backgroundImageSource = null;
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo backgroundImageValue = null;
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo backgroundImageMIMEType = null;
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo backgroundEmbeddingMode = null;
			for (int i = 0; i < attributes.Count; i++)
			{
				StyleInformation.StyleInformationAttribute styleInformationAttribute = attributes[i];
				if (checkForMeDotValue && styleInformationAttribute.ValueType == Microsoft.ReportingServices.ReportIntermediateFormat.ValueType.Constant && styleInformationAttribute.Value.MeDotValueDetected)
				{
					meDotValueReferenced = true;
				}
				switch (attributes[i].Name)
				{
				case "BackgroundImageSource":
					backgroundImageSource = styleInformationAttribute.Value;
					break;
				case "BackgroundImageValue":
					backgroundImageValue = styleInformationAttribute.Value;
					break;
				case "BackgroundImageMIMEType":
					backgroundImageMIMEType = styleInformationAttribute.Value;
					break;
				case "BackgroundRepeat":
					if (ValidateBackgroundRepeat(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "EmbeddingMode":
					backgroundEmbeddingMode = styleInformationAttribute.Value;
					break;
				case "Transparency":
					if (ValidateTransparency(styleInformationAttribute.Value, objectType, objectName, "Transparency", errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "TransparentColor":
					if (ValidateColor(styleInformationAttribute.Value, objectType, objectName, "TransparentColor", errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "BorderColor":
				case "BorderColorLeft":
				case "BorderColorRight":
				case "BorderColorTop":
				case "BorderColorBottom":
					if (ValidateColor(styleInformationAttribute.Value, objectType, objectName, "BorderColor", errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "BorderStyle":
					if (ValidateBorderStyle(styleInformationAttribute.Value, objectType, objectName, isDynamicImageSubElement, "BorderStyle", isDefaultBorder: true, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "BorderStyleLeft":
				case "BorderStyleRight":
				case "BorderStyleTop":
				case "BorderStyleBottom":
					if (ValidateBorderStyle(styleInformationAttribute.Value, objectType, objectName, isDynamicImageSubElement, "BorderStyle", isDefaultBorder: false, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "BorderWidth":
				case "BorderWidthLeft":
				case "BorderWidthRight":
				case "BorderWidthTop":
				case "BorderWidthBottom":
					if (ValidateSize(styleInformationAttribute.Value, Validator.BorderWidthMin, Validator.BorderWidthMax, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "BackgroundGradientEndColor":
					if (ValidateColor(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "BackgroundGradientType":
					if (ValidateBackgroundGradientType(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "FontStyle":
					if (ValidateFontStyle(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "FontFamily":
					style.AddAttribute(styleInformationAttribute);
					break;
				case "FontSize":
					if (ValidateSize(styleInformationAttribute.Value, Validator.FontSizeMin, Validator.FontSizeMax, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "FontWeight":
					if (ValidateFontWeight(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "Format":
					style.AddAttribute(styleInformationAttribute);
					break;
				case "TextDecoration":
					if (ValidateTextDecoration(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "TextAlign":
					if (ValidateTextAlign(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "VerticalAlign":
					if (ValidateVerticalAlign(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "Color":
				case "BackgroundColor":
					if (ValidateColor(styleInformationAttribute, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "PaddingLeft":
				case "PaddingRight":
				case "PaddingTop":
				case "PaddingBottom":
					if (ValidateSize(styleInformationAttribute.Value, Validator.PaddingMin, Validator.PaddingMax, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "LineHeight":
					if (ValidateSize(styleInformationAttribute.Value, Validator.LineHeightMin, Validator.LineHeightMax, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "Direction":
					if (ValidateDirection(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "WritingMode":
					if (ValidateWritingMode(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "Language":
				{
					if (ValidateSpecificLanguage(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext, out CultureInfo _))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				}
				case "UnicodeBiDi":
					if (ValidateUnicodeBiDi(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "Calendar":
					if (ValidateCalendar(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "CurrencyLanguage":
					if (ValidateLanguage(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "NumeralLanguage":
					if (ValidateLanguage(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "NumeralVariant":
					if (ValidateNumeralVariant(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "ShadowColor":
					if (ValidateColor(styleInformationAttribute.Value, objectType, objectName, "ShadowColor", errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "ShadowOffset":
					if (ValidateSize(styleInformationAttribute.Value, Validator.NormalMin, Validator.NormalMax, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "BackgroundHatchType":
					if (ValidateBackgroundHatchType(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "TextEffect":
					if (ValidateTextEffect(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "Position":
					if (ValidatePosition(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
			ValidateBackgroundImage(backgroundImageSource, backgroundImageValue, backgroundImageMIMEType, backgroundEmbeddingMode, style, objectType, objectName, errorContext);
			if (0 < style.StyleAttributes.Count)
			{
				return style;
			}
			return null;
		}

		internal static void ValidateCalendar(CultureInfo language, string calendar, ObjectType objectType, string ObjectName, string propertyName, ErrorContext errorContext)
		{
			if (!Validator.ValidateCalendar(language, calendar))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidCalendarForLanguage, Severity.Error, objectType, ObjectName, propertyName, calendar, language.Name);
			}
		}

		internal static void ValidateNumeralVariant(CultureInfo language, int numVariant, ObjectType objectType, string ObjectName, string propertyName, ErrorContext errorContext)
		{
			if (!Validator.ValidateNumeralVariant(language, numVariant))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidNumeralVariantForLanguage, Severity.Error, objectType, ObjectName, propertyName, numVariant.ToString(CultureInfo.InvariantCulture), language.Name);
			}
		}

		internal static void ValidateTextRunMarkupType(string value, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			if (!Validator.ValidateTextRunMarkupType(value))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidMarkupType, Severity.Error, objectType, objectName, propertyName, value);
			}
		}

		internal static void ValidateParagraphListStyle(string value, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			if (!Validator.ValidateParagraphListStyle(value))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidListStyle, Severity.Error, objectType, objectName, propertyName, value);
			}
		}

		internal static string ValidateReportName(ICatalogItemContext reportContext, string reportName, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(reportName != null);
			if (reportName.StartsWith(Uri.UriSchemeHttp + Uri.SchemeDelimiter, StringComparison.OrdinalIgnoreCase) || reportName.StartsWith(Uri.UriSchemeHttps + Uri.SchemeDelimiter, StringComparison.OrdinalIgnoreCase))
			{
				try
				{
					new Uri(reportName);
				}
				catch (UriFormatException)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidReportUri, Severity.Error, objectType, objectName, propertyName);
					return reportName;
				}
			}
			else if (reportName.Length > 0 && -1 != reportName.IndexOfAny(m_invalidCharacters.ToCharArray()))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidReportNameCharacters, Severity.Error, objectType, objectName, propertyName, m_invalidCharacters);
				return reportName;
			}
			string text;
			try
			{
				text = reportContext.AdjustSubreportOrDrillthroughReportPath(reportName.Trim());
			}
			catch (RSException)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidReportUri, Severity.Error, objectType, objectName, propertyName);
				return reportName;
			}
			if (text == null || reportName.Length == 0)
			{
				errorContext.Register((reportName.Length == 0) ? ProcessingErrorCode.rsInvalidReportName : ProcessingErrorCode.rsInvalidReportUri, Severity.Error, objectType, objectName, propertyName);
				return reportName;
			}
			return text;
		}
	}
}

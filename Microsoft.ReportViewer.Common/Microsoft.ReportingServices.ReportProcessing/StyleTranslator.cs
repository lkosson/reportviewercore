using Microsoft.ReportingServices.OnDemandReportRendering;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal abstract class StyleTranslator
	{
		private static bool CompareWithInvariantCulture(string strOne, string strTwo)
		{
			if (ReportProcessing.CompareWithInvariantCulture(strOne, strTwo, ignoreCase: false) == 0)
			{
				return true;
			}
			return false;
		}

		internal static int TranslateStyle(StyleAttributeNames styleName, string styleString, IErrorContext errorContext, bool isChartStyle)
		{
			switch (styleName)
			{
			case StyleAttributeNames.BorderStyle:
			case StyleAttributeNames.BorderStyleTop:
			case StyleAttributeNames.BorderStyleLeft:
			case StyleAttributeNames.BorderStyleRight:
			case StyleAttributeNames.BorderStyleBottom:
				return (int)TranslateBorderStyle(styleString, errorContext);
			case StyleAttributeNames.BackgroundGradientType:
				return (int)TranslateBackgroundGradientType(styleString, errorContext);
			case StyleAttributeNames.BackgroundImageRepeat:
				return (int)TranslateBackgroundRepeat(styleString, errorContext, isChartStyle);
			case StyleAttributeNames.FontStyle:
				return (int)TranslateFontStyle(styleString, errorContext);
			case StyleAttributeNames.FontWeight:
				return (int)TranslateFontWeight(styleString, errorContext);
			case StyleAttributeNames.TextDecoration:
				return (int)TranslateTextDecoration(styleString, errorContext);
			case StyleAttributeNames.TextAlign:
				return (int)TranslateTextAlign(styleString, errorContext);
			case StyleAttributeNames.VerticalAlign:
				return (int)TranslateVerticalAlign(styleString, errorContext);
			case StyleAttributeNames.Direction:
				return (int)TranslateDirection(styleString, errorContext);
			case StyleAttributeNames.WritingMode:
				return (int)TranslateWritingMode(styleString, errorContext);
			case StyleAttributeNames.UnicodeBiDi:
				return (int)TranslateUnicodeBiDi(styleString, errorContext);
			case StyleAttributeNames.Calendar:
				return (int)TranslateCalendar(styleString, errorContext);
			case StyleAttributeNames.TextEffect:
				return (int)TranslateTextEffect(styleString, errorContext, isChartStyle);
			case StyleAttributeNames.BackgroundHatchType:
				return (int)TranslateBackgroundHatchType(styleString, errorContext, isChartStyle);
			case StyleAttributeNames.Position:
				return (int)TranslatePosition(styleString, errorContext, isChartStyle);
			default:
				throw new NotImplementedException("cannot translate style: " + styleName);
			}
		}

		internal static BorderStyles TranslateBorderStyle(string styleString, IErrorContext errorContext)
		{
			return TranslateBorderStyle(styleString, BorderStyles.Default, errorContext);
		}

		internal static BorderStyles TranslateBorderStyle(string styleString, BorderStyles defaultStyle, IErrorContext errorContext)
		{
			if (styleString == null)
			{
				return defaultStyle;
			}
			if (CompareWithInvariantCulture("None", styleString))
			{
				return BorderStyles.None;
			}
			if (CompareWithInvariantCulture("Solid", styleString))
			{
				return BorderStyles.Solid;
			}
			if (CompareWithInvariantCulture("Dashed", styleString))
			{
				return BorderStyles.Dashed;
			}
			if (CompareWithInvariantCulture("Dotted", styleString))
			{
				return BorderStyles.Dotted;
			}
			if (CompareWithInvariantCulture("DashDot", styleString))
			{
				return BorderStyles.DashDot;
			}
			if (CompareWithInvariantCulture("DashDotDot", styleString))
			{
				return BorderStyles.DashDotDot;
			}
			if (CompareWithInvariantCulture("Double", styleString))
			{
				return BorderStyles.Double;
			}
			if (CompareWithInvariantCulture("Groove", styleString))
			{
				return BorderStyles.Solid;
			}
			if (CompareWithInvariantCulture("Ridge", styleString))
			{
				return BorderStyles.Solid;
			}
			if (CompareWithInvariantCulture("Inset", styleString))
			{
				return BorderStyles.Solid;
			}
			if (CompareWithInvariantCulture("WindowInset", styleString))
			{
				return BorderStyles.Solid;
			}
			if (CompareWithInvariantCulture("Outset", styleString))
			{
				return BorderStyles.Solid;
			}
			if (CompareWithInvariantCulture("Default", styleString))
			{
				return defaultStyle;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidBorderStyle, Severity.Warning, styleString);
			return defaultStyle;
		}

		internal static BackgroundGradients TranslateBackgroundGradientType(string styleString, IErrorContext errorContext)
		{
			if (styleString == null)
			{
				return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumBackgroundGradient;
			}
			if (CompareWithInvariantCulture("None", styleString))
			{
				return BackgroundGradients.None;
			}
			if (CompareWithInvariantCulture("LeftRight", styleString))
			{
				return BackgroundGradients.LeftRight;
			}
			if (CompareWithInvariantCulture("TopBottom", styleString))
			{
				return BackgroundGradients.TopBottom;
			}
			if (CompareWithInvariantCulture("Center", styleString))
			{
				return BackgroundGradients.Center;
			}
			if (CompareWithInvariantCulture("DiagonalLeft", styleString))
			{
				return BackgroundGradients.DiagonalLeft;
			}
			if (CompareWithInvariantCulture("DiagonalRight", styleString))
			{
				return BackgroundGradients.DiagonalRight;
			}
			if (CompareWithInvariantCulture("HorizontalCenter", styleString))
			{
				return BackgroundGradients.HorizontalCenter;
			}
			if (CompareWithInvariantCulture("VerticalCenter", styleString))
			{
				return BackgroundGradients.VerticalCenter;
			}
			if (CompareWithInvariantCulture("Default", styleString))
			{
				return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumBackgroundGradient;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidBackgroundGradientType, Severity.Warning, styleString);
			return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumBackgroundGradient;
		}

		internal static BackgroundRepeatTypes TranslateBackgroundRepeat(string styleString, IErrorContext errorContext, bool isChartStyle)
		{
			if (styleString != null && !CompareWithInvariantCulture("Default", styleString))
			{
				if (CompareWithInvariantCulture("Repeat", styleString))
				{
					return BackgroundRepeatTypes.Repeat;
				}
				if (CompareWithInvariantCulture("NoRepeat", styleString))
				{
					if (isChartStyle)
					{
						return BackgroundRepeatTypes.Fit;
					}
					return BackgroundRepeatTypes.Clip;
				}
				if (!isChartStyle)
				{
					if (CompareWithInvariantCulture("RepeatX", styleString))
					{
						return BackgroundRepeatTypes.RepeatX;
					}
					if (CompareWithInvariantCulture("RepeatY", styleString))
					{
						return BackgroundRepeatTypes.RepeatY;
					}
				}
				else if (CompareWithInvariantCulture("Fit", styleString))
				{
					return BackgroundRepeatTypes.Fit;
				}
				if (CompareWithInvariantCulture("Clip", styleString))
				{
					return BackgroundRepeatTypes.Clip;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidBackgroundRepeat, Severity.Warning, styleString);
			}
			if (isChartStyle)
			{
				return BackgroundRepeatTypes.Fit;
			}
			return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumBackgroundRepeatType;
		}

		internal static Positions TranslatePosition(string styleString, IErrorContext errorContext, bool isChartStyle)
		{
			if (styleString != null && !CompareWithInvariantCulture("Default", styleString))
			{
				if (CompareWithInvariantCulture(styleString, "Top"))
				{
					return Positions.Top;
				}
				if (CompareWithInvariantCulture(styleString, "TopLeft"))
				{
					return Positions.TopLeft;
				}
				if (CompareWithInvariantCulture(styleString, "TopRight"))
				{
					return Positions.TopRight;
				}
				if (CompareWithInvariantCulture(styleString, "Left"))
				{
					return Positions.Left;
				}
				if (CompareWithInvariantCulture(styleString, "Center"))
				{
					return Positions.Center;
				}
				if (CompareWithInvariantCulture(styleString, "Right"))
				{
					return Positions.Right;
				}
				if (CompareWithInvariantCulture(styleString, "BottomRight"))
				{
					return Positions.BottomRight;
				}
				if (CompareWithInvariantCulture(styleString, "Bottom"))
				{
					return Positions.Bottom;
				}
				if (CompareWithInvariantCulture(styleString, "BottomLeft"))
				{
					return Positions.BottomLeft;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidBackgroundImagePosition, Severity.Warning, styleString);
			}
			if (isChartStyle)
			{
				return Positions.TopLeft;
			}
			return Positions.Center;
		}

		internal static FontStyles TranslateFontStyle(string styleString, IErrorContext errorContext)
		{
			if (styleString == null)
			{
				return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumFontStyle;
			}
			if (CompareWithInvariantCulture("Normal", styleString))
			{
				return FontStyles.Normal;
			}
			if (CompareWithInvariantCulture("Italic", styleString))
			{
				return FontStyles.Italic;
			}
			if (CompareWithInvariantCulture("Default", styleString))
			{
				return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumFontStyle;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidFontStyle, Severity.Warning, styleString);
			return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumFontStyle;
		}

		internal static FontWeights TranslateFontWeight(string styleString, IErrorContext errorContext)
		{
			if (styleString == null)
			{
				return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumFontWeight;
			}
			if (CompareWithInvariantCulture("Normal", styleString))
			{
				return FontWeights.Normal;
			}
			if (CompareWithInvariantCulture("Bold", styleString))
			{
				return FontWeights.Bold;
			}
			if (CompareWithInvariantCulture("Bolder", styleString))
			{
				return FontWeights.Bold;
			}
			if (CompareWithInvariantCulture("100", styleString))
			{
				return FontWeights.Thin;
			}
			if (CompareWithInvariantCulture("200", styleString))
			{
				return FontWeights.ExtraLight;
			}
			if (CompareWithInvariantCulture("300", styleString))
			{
				return FontWeights.Light;
			}
			if (CompareWithInvariantCulture("400", styleString))
			{
				return FontWeights.Normal;
			}
			if (CompareWithInvariantCulture("500", styleString))
			{
				return FontWeights.Medium;
			}
			if (CompareWithInvariantCulture("600", styleString))
			{
				return FontWeights.SemiBold;
			}
			if (CompareWithInvariantCulture("700", styleString))
			{
				return FontWeights.Bold;
			}
			if (CompareWithInvariantCulture("800", styleString))
			{
				return FontWeights.ExtraBold;
			}
			if (CompareWithInvariantCulture("900", styleString))
			{
				return FontWeights.Heavy;
			}
			if (CompareWithInvariantCulture("Thin", styleString))
			{
				return FontWeights.Thin;
			}
			if (CompareWithInvariantCulture("ExtraLight", styleString))
			{
				return FontWeights.ExtraLight;
			}
			if (CompareWithInvariantCulture("Light", styleString))
			{
				return FontWeights.Light;
			}
			if (CompareWithInvariantCulture("Lighter", styleString))
			{
				return FontWeights.Light;
			}
			if (CompareWithInvariantCulture("Medium", styleString))
			{
				return FontWeights.Medium;
			}
			if (CompareWithInvariantCulture("SemiBold", styleString))
			{
				return FontWeights.SemiBold;
			}
			if (CompareWithInvariantCulture("ExtraBold", styleString))
			{
				return FontWeights.ExtraBold;
			}
			if (CompareWithInvariantCulture("Heavy", styleString))
			{
				return FontWeights.Heavy;
			}
			if (CompareWithInvariantCulture("Default", styleString))
			{
				return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumFontWeight;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidFontWeight, Severity.Warning, styleString);
			return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumFontWeight;
		}

		internal static TextDecorations TranslateTextDecoration(string styleString, IErrorContext errorContext)
		{
			if (styleString == null)
			{
				return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumTextDecoration;
			}
			if (CompareWithInvariantCulture("None", styleString))
			{
				return TextDecorations.None;
			}
			if (CompareWithInvariantCulture("Underline", styleString))
			{
				return TextDecorations.Underline;
			}
			if (CompareWithInvariantCulture("Overline", styleString))
			{
				return TextDecorations.Overline;
			}
			if (CompareWithInvariantCulture("LineThrough", styleString))
			{
				return TextDecorations.LineThrough;
			}
			if (CompareWithInvariantCulture("Default", styleString))
			{
				return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumTextDecoration;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidTextDecoration, Severity.Warning, styleString);
			return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumTextDecoration;
		}

		internal static TextAlignments TranslateTextAlign(string styleString, IErrorContext errorContext)
		{
			if (styleString == null)
			{
				return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumTextAlignment;
			}
			if (CompareWithInvariantCulture("General", styleString))
			{
				return TextAlignments.General;
			}
			if (CompareWithInvariantCulture("Left", styleString))
			{
				return TextAlignments.Left;
			}
			if (CompareWithInvariantCulture("Center", styleString))
			{
				return TextAlignments.Center;
			}
			if (CompareWithInvariantCulture("Right", styleString))
			{
				return TextAlignments.Right;
			}
			if (CompareWithInvariantCulture("Default", styleString))
			{
				return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumTextAlignment;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidTextAlign, Severity.Warning, styleString);
			return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumTextAlignment;
		}

		internal static VerticalAlignments TranslateVerticalAlign(string styleString, IErrorContext errorContext)
		{
			if (styleString == null)
			{
				return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumVerticalAlignment;
			}
			if (CompareWithInvariantCulture("Top", styleString))
			{
				return VerticalAlignments.Top;
			}
			if (CompareWithInvariantCulture("Middle", styleString))
			{
				return VerticalAlignments.Middle;
			}
			if (CompareWithInvariantCulture("Bottom", styleString))
			{
				return VerticalAlignments.Bottom;
			}
			if (CompareWithInvariantCulture("Default", styleString))
			{
				return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumVerticalAlignment;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidVerticalAlign, Severity.Warning, styleString);
			return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumVerticalAlignment;
		}

		internal static Directions TranslateDirection(string styleString, IErrorContext errorContext)
		{
			if (styleString == null)
			{
				return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumDirection;
			}
			if (CompareWithInvariantCulture("LTR", styleString))
			{
				return Directions.LTR;
			}
			if (CompareWithInvariantCulture("RTL", styleString))
			{
				return Directions.RTL;
			}
			if (CompareWithInvariantCulture("Default", styleString))
			{
				return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumDirection;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidDirection, Severity.Warning, styleString);
			return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumDirection;
		}

		internal static WritingModes TranslateWritingMode(string styleString, IErrorContext errorContext)
		{
			if (styleString == null)
			{
				return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumWritingMode;
			}
			if (CompareWithInvariantCulture("lr-tb", styleString))
			{
				return WritingModes.Horizontal;
			}
			if (CompareWithInvariantCulture("tb-rl", styleString))
			{
				return WritingModes.Vertical;
			}
			if (CompareWithInvariantCulture("Horizontal", styleString))
			{
				return WritingModes.Horizontal;
			}
			if (CompareWithInvariantCulture("Vertical", styleString))
			{
				return WritingModes.Vertical;
			}
			if (CompareWithInvariantCulture("Rotate270", styleString))
			{
				return WritingModes.Rotate270;
			}
			if (CompareWithInvariantCulture("Default", styleString))
			{
				return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumWritingMode;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidWritingMode, Severity.Warning, styleString);
			return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumWritingMode;
		}

		internal static UnicodeBiDiTypes TranslateUnicodeBiDi(string styleString, IErrorContext errorContext)
		{
			if (styleString == null)
			{
				return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumUnicodeBiDiType;
			}
			if (CompareWithInvariantCulture("Normal", styleString))
			{
				return UnicodeBiDiTypes.Normal;
			}
			if (CompareWithInvariantCulture("Embed", styleString))
			{
				return UnicodeBiDiTypes.Embed;
			}
			if (CompareWithInvariantCulture("BiDi-Override", styleString))
			{
				return UnicodeBiDiTypes.BiDiOverride;
			}
			if (CompareWithInvariantCulture("BiDiOverride", styleString))
			{
				return UnicodeBiDiTypes.BiDiOverride;
			}
			if (CompareWithInvariantCulture("Default", styleString))
			{
				return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumUnicodeBiDiType;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidUnicodeBiDi, Severity.Warning, styleString);
			return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumUnicodeBiDiType;
		}

		internal static string TranslateCalendar(Calendars calendar)
		{
			switch (calendar)
			{
			case Calendars.Default:
				return "Default";
			case Calendars.Gregorian:
				return "Gregorian";
			case Calendars.GregorianArabic:
				return "Gregorian Arabic";
			case Calendars.GregorianMiddleEastFrench:
				return "Gregorian Middle East French";
			case Calendars.GregorianTransliteratedEnglish:
				return "Gregorian Transliterated English";
			case Calendars.GregorianTransliteratedFrench:
				return "Gregorian Transliterated French";
			case Calendars.GregorianUSEnglish:
				return "Gregorian US English";
			case Calendars.Hebrew:
				return "Hebrew";
			case Calendars.Hijri:
				return "Hijri";
			case Calendars.Japanese:
				return "Japanese";
			case Calendars.Korean:
				return "Korean";
			case Calendars.Taiwan:
				return "Taiwan";
			case Calendars.ThaiBuddhist:
				return "Thai Buddhist";
			case Calendars.Julian:
				return "Julian";
			default:
				return "Default";
			}
		}

		internal static Calendars TranslateCalendar(string styleString, IErrorContext errorContext)
		{
			if (styleString != null && !CompareWithInvariantCulture("Default", styleString))
			{
				if (CompareWithInvariantCulture("Gregorian", styleString))
				{
					return Calendars.Gregorian;
				}
				if (CompareWithInvariantCulture("Gregorian Arabic", styleString))
				{
					return Calendars.GregorianArabic;
				}
				if (CompareWithInvariantCulture("Gregorian Middle East French", styleString))
				{
					return Calendars.GregorianMiddleEastFrench;
				}
				if (CompareWithInvariantCulture("Gregorian Transliterated English", styleString))
				{
					return Calendars.GregorianTransliteratedEnglish;
				}
				if (CompareWithInvariantCulture("Gregorian Transliterated French", styleString))
				{
					return Calendars.GregorianTransliteratedFrench;
				}
				if (CompareWithInvariantCulture("Gregorian US English", styleString))
				{
					return Calendars.GregorianUSEnglish;
				}
				if (CompareWithInvariantCulture("Hebrew", styleString))
				{
					return Calendars.Hebrew;
				}
				if (CompareWithInvariantCulture("Hijri", styleString))
				{
					return Calendars.Hijri;
				}
				if (CompareWithInvariantCulture("Japanese", styleString))
				{
					return Calendars.Japanese;
				}
				if (CompareWithInvariantCulture("Korea", styleString))
				{
					return Calendars.Korean;
				}
				if (CompareWithInvariantCulture("Korean", styleString))
				{
					return Calendars.Korean;
				}
				if (CompareWithInvariantCulture("Taiwan", styleString))
				{
					return Calendars.Taiwan;
				}
				if (CompareWithInvariantCulture("Thai Buddhist", styleString))
				{
					return Calendars.ThaiBuddhist;
				}
				if (CompareWithInvariantCulture("Julian", styleString))
				{
					return Calendars.Julian;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidCalendar, Severity.Warning, styleString);
			}
			return Microsoft.ReportingServices.OnDemandReportRendering.Style.DefaultEnumCalendar;
		}

		internal static BackgroundHatchTypes TranslateBackgroundHatchType(string styleValue, IErrorContext errorContext, bool isChartStyle)
		{
			if (styleValue != null)
			{
				if (CompareWithInvariantCulture(styleValue, "None"))
				{
					return BackgroundHatchTypes.None;
				}
				if (CompareWithInvariantCulture(styleValue, "Default"))
				{
					return BackgroundHatchTypes.None;
				}
				if (CompareWithInvariantCulture(styleValue, "BackwardDiagonal"))
				{
					return BackgroundHatchTypes.BackwardDiagonal;
				}
				if (CompareWithInvariantCulture(styleValue, "Cross"))
				{
					return BackgroundHatchTypes.Cross;
				}
				if (CompareWithInvariantCulture(styleValue, "DarkDownwardDiagonal"))
				{
					return BackgroundHatchTypes.DarkDownwardDiagonal;
				}
				if (CompareWithInvariantCulture(styleValue, "DarkHorizontal"))
				{
					return BackgroundHatchTypes.DarkHorizontal;
				}
				if (CompareWithInvariantCulture(styleValue, "DarkUpwardDiagonal"))
				{
					return BackgroundHatchTypes.DarkUpwardDiagonal;
				}
				if (CompareWithInvariantCulture(styleValue, "DarkVertical"))
				{
					return BackgroundHatchTypes.DarkVertical;
				}
				if (CompareWithInvariantCulture(styleValue, "DashedDownwardDiagonal"))
				{
					return BackgroundHatchTypes.DashedDownwardDiagonal;
				}
				if (CompareWithInvariantCulture(styleValue, "DashedHorizontal"))
				{
					return BackgroundHatchTypes.DashedHorizontal;
				}
				if (CompareWithInvariantCulture(styleValue, "DashedUpwardDiagonal"))
				{
					return BackgroundHatchTypes.DashedUpwardDiagonal;
				}
				if (CompareWithInvariantCulture(styleValue, "DashedVertical"))
				{
					return BackgroundHatchTypes.DashedVertical;
				}
				if (CompareWithInvariantCulture(styleValue, "DiagonalBrick"))
				{
					return BackgroundHatchTypes.DiagonalBrick;
				}
				if (CompareWithInvariantCulture(styleValue, "DiagonalCross"))
				{
					return BackgroundHatchTypes.DiagonalCross;
				}
				if (CompareWithInvariantCulture(styleValue, "Divot"))
				{
					return BackgroundHatchTypes.Divot;
				}
				if (CompareWithInvariantCulture(styleValue, "DottedDiamond"))
				{
					return BackgroundHatchTypes.DottedDiamond;
				}
				if (CompareWithInvariantCulture(styleValue, "DottedGrid"))
				{
					return BackgroundHatchTypes.DottedGrid;
				}
				if (CompareWithInvariantCulture(styleValue, "ForwardDiagonal"))
				{
					return BackgroundHatchTypes.ForwardDiagonal;
				}
				if (CompareWithInvariantCulture(styleValue, "Horizontal"))
				{
					return BackgroundHatchTypes.Horizontal;
				}
				if (CompareWithInvariantCulture(styleValue, "HorizontalBrick"))
				{
					return BackgroundHatchTypes.HorizontalBrick;
				}
				if (CompareWithInvariantCulture(styleValue, "LargeCheckerBoard"))
				{
					return BackgroundHatchTypes.LargeCheckerBoard;
				}
				if (CompareWithInvariantCulture(styleValue, "LargeConfetti"))
				{
					return BackgroundHatchTypes.LargeConfetti;
				}
				if (CompareWithInvariantCulture(styleValue, "LargeGrid"))
				{
					return BackgroundHatchTypes.LargeGrid;
				}
				if (CompareWithInvariantCulture(styleValue, "LightDownwardDiagonal"))
				{
					return BackgroundHatchTypes.LightDownwardDiagonal;
				}
				if (CompareWithInvariantCulture(styleValue, "LightHorizontal"))
				{
					return BackgroundHatchTypes.LightHorizontal;
				}
				if (CompareWithInvariantCulture(styleValue, "LightUpwardDiagonal"))
				{
					return BackgroundHatchTypes.LightUpwardDiagonal;
				}
				if (CompareWithInvariantCulture(styleValue, "LightVertical"))
				{
					return BackgroundHatchTypes.LightVertical;
				}
				if (CompareWithInvariantCulture(styleValue, "NarrowHorizontal"))
				{
					return BackgroundHatchTypes.NarrowHorizontal;
				}
				if (CompareWithInvariantCulture(styleValue, "NarrowVertical"))
				{
					return BackgroundHatchTypes.NarrowVertical;
				}
				if (CompareWithInvariantCulture(styleValue, "OutlinedDiamond"))
				{
					return BackgroundHatchTypes.OutlinedDiamond;
				}
				if (CompareWithInvariantCulture(styleValue, "Percent05"))
				{
					return BackgroundHatchTypes.Percent05;
				}
				if (CompareWithInvariantCulture(styleValue, "Percent10"))
				{
					return BackgroundHatchTypes.Percent10;
				}
				if (CompareWithInvariantCulture(styleValue, "Percent20"))
				{
					return BackgroundHatchTypes.Percent20;
				}
				if (CompareWithInvariantCulture(styleValue, "Percent25"))
				{
					return BackgroundHatchTypes.Percent25;
				}
				if (CompareWithInvariantCulture(styleValue, "Percent30"))
				{
					return BackgroundHatchTypes.Percent30;
				}
				if (CompareWithInvariantCulture(styleValue, "Percent40"))
				{
					return BackgroundHatchTypes.Percent40;
				}
				if (CompareWithInvariantCulture(styleValue, "Percent50"))
				{
					return BackgroundHatchTypes.Percent50;
				}
				if (CompareWithInvariantCulture(styleValue, "Percent60"))
				{
					return BackgroundHatchTypes.Percent60;
				}
				if (CompareWithInvariantCulture(styleValue, "Percent70"))
				{
					return BackgroundHatchTypes.Percent70;
				}
				if (CompareWithInvariantCulture(styleValue, "Percent75"))
				{
					return BackgroundHatchTypes.Percent75;
				}
				if (CompareWithInvariantCulture(styleValue, "Percent80"))
				{
					return BackgroundHatchTypes.Percent80;
				}
				if (CompareWithInvariantCulture(styleValue, "Percent90"))
				{
					return BackgroundHatchTypes.Percent90;
				}
				if (CompareWithInvariantCulture(styleValue, "Plaid"))
				{
					return BackgroundHatchTypes.Plaid;
				}
				if (CompareWithInvariantCulture(styleValue, "Shingle"))
				{
					return BackgroundHatchTypes.Shingle;
				}
				if (CompareWithInvariantCulture(styleValue, "SmallCheckerBoard"))
				{
					return BackgroundHatchTypes.SmallCheckerBoard;
				}
				if (CompareWithInvariantCulture(styleValue, "SmallConfetti"))
				{
					return BackgroundHatchTypes.SmallConfetti;
				}
				if (CompareWithInvariantCulture(styleValue, "SmallGrid"))
				{
					return BackgroundHatchTypes.SmallGrid;
				}
				if (CompareWithInvariantCulture(styleValue, "SolidDiamond"))
				{
					return BackgroundHatchTypes.SolidDiamond;
				}
				if (CompareWithInvariantCulture(styleValue, "Sphere"))
				{
					return BackgroundHatchTypes.Sphere;
				}
				if (CompareWithInvariantCulture(styleValue, "Trellis"))
				{
					return BackgroundHatchTypes.Trellis;
				}
				if (CompareWithInvariantCulture(styleValue, "Vertical"))
				{
					return BackgroundHatchTypes.Vertical;
				}
				if (CompareWithInvariantCulture(styleValue, "Wave"))
				{
					return BackgroundHatchTypes.Wave;
				}
				if (CompareWithInvariantCulture(styleValue, "Weave"))
				{
					return BackgroundHatchTypes.Weave;
				}
				if (CompareWithInvariantCulture(styleValue, "WideDownwardDiagonal"))
				{
					return BackgroundHatchTypes.WideDownwardDiagonal;
				}
				if (CompareWithInvariantCulture(styleValue, "WideUpwardDiagonal"))
				{
					return BackgroundHatchTypes.WideUpwardDiagonal;
				}
				if (CompareWithInvariantCulture(styleValue, "ZigZag"))
				{
					return BackgroundHatchTypes.ZigZag;
				}
			}
			else if (styleValue != null)
			{
				errorContext?.Register(ProcessingErrorCode.rsInvalidBackgroundHatchType, Severity.Warning, styleValue);
			}
			return BackgroundHatchTypes.None;
		}

		internal static TextEffects TranslateTextEffect(string styleValue, IErrorContext errorContext, bool isChartStyle)
		{
			if (styleValue != null && !CompareWithInvariantCulture(styleValue, "Default"))
			{
				if (CompareWithInvariantCulture(styleValue, "None"))
				{
					return TextEffects.None;
				}
				if (CompareWithInvariantCulture(styleValue, "Shadow"))
				{
					return TextEffects.Shadow;
				}
				if (CompareWithInvariantCulture(styleValue, "Emboss"))
				{
					return TextEffects.Emboss;
				}
				if (CompareWithInvariantCulture(styleValue, "Embed"))
				{
					return TextEffects.Embed;
				}
				if (CompareWithInvariantCulture(styleValue, "Frame"))
				{
					return TextEffects.Frame;
				}
			}
			if (styleValue != null)
			{
				errorContext?.Register(ProcessingErrorCode.rsInvalidTextEffect, Severity.Warning, styleValue);
			}
			if (isChartStyle)
			{
				return TextEffects.Shadow;
			}
			return TextEffects.None;
		}
	}
}

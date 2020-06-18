using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal sealed class Validator
	{
		internal class DoubleComparer : IComparer<double>
		{
			private static DoubleComparer m_instance;

			public static DoubleComparer Instance
			{
				get
				{
					if (m_instance == null)
					{
						m_instance = new DoubleComparer();
					}
					return m_instance;
				}
			}

			private DoubleComparer()
			{
			}

			public int Compare(double x, double y)
			{
				return CompareDoubles(x, y);
			}
		}

		internal const int DecimalPrecision = 10;

		internal const int RoundingPrecision = 4;

		internal const int TruncatePrecision = 3;

		internal static double NormalMin = 0.0;

		internal static double NegativeMin = -11557.0;

		internal static double NormalMax = 11557.0;

		internal static double BorderWidthMin = 0.08814;

		internal static double BorderWidthMax = 7.0555555555555554;

		internal static double FontSizeMin = 0.35277777777777775;

		internal static double FontSizeMax = 70.555555555555543;

		internal static double PaddingMin = 0.0;

		internal static double PaddingMax = 352.77777777777777;

		internal static double LineHeightMin = 0.35277777777777775;

		internal static double LineHeightMax = 352.77777777777777;

		internal const int ParagraphListLevelMin = 0;

		internal const int ParagraphListLevelMax = 9;

		private static Regex m_colorRegex = new Regex("^#(\\d|a|b|c|d|e|f){6}$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);

		private static Regex m_colorRegexTransparency = new Regex("^#(\\d|a|b|c|d|e|f){8}$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);

		internal static bool ValidateGaugeAntiAliasings(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "All") || CompareWithInvariantCulture(val, "None") || CompareWithInvariantCulture(val, "Text") || CompareWithInvariantCulture(val, "Graphics"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateGaugeBarStarts(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "ScaleStart") || CompareWithInvariantCulture(val, "Zero"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateGaugeCapStyles(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "RoundedDark") || CompareWithInvariantCulture(val, "Rounded") || CompareWithInvariantCulture(val, "RoundedLight") || CompareWithInvariantCulture(val, "RoundedWithAdditionalTop") || CompareWithInvariantCulture(val, "RoundedWithWideIndentation") || CompareWithInvariantCulture(val, "FlattenedWithIndentation") || CompareWithInvariantCulture(val, "FlattenedWithWideIndentation") || CompareWithInvariantCulture(val, "RoundedGlossyWithIndentation") || CompareWithInvariantCulture(val, "RoundedWithIndentation"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateGaugeFrameShapes(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Default") || CompareWithInvariantCulture(val, "Circular") || CompareWithInvariantCulture(val, "Rectangular") || CompareWithInvariantCulture(val, "RoundedRectangular") || CompareWithInvariantCulture(val, "AutoShape") || CompareWithInvariantCulture(val, "CustomCircular1") || CompareWithInvariantCulture(val, "CustomCircular2") || CompareWithInvariantCulture(val, "CustomCircular3") || CompareWithInvariantCulture(val, "CustomCircular4") || CompareWithInvariantCulture(val, "CustomCircular5") || CompareWithInvariantCulture(val, "CustomCircular6") || CompareWithInvariantCulture(val, "CustomCircular7") || CompareWithInvariantCulture(val, "CustomCircular8") || CompareWithInvariantCulture(val, "CustomCircular9") || CompareWithInvariantCulture(val, "CustomCircular10") || CompareWithInvariantCulture(val, "CustomCircular11") || CompareWithInvariantCulture(val, "CustomCircular12") || CompareWithInvariantCulture(val, "CustomCircular13") || CompareWithInvariantCulture(val, "CustomCircular14") || CompareWithInvariantCulture(val, "CustomCircular15") || CompareWithInvariantCulture(val, "CustomSemiCircularN1") || CompareWithInvariantCulture(val, "CustomSemiCircularN2") || CompareWithInvariantCulture(val, "CustomSemiCircularN3") || CompareWithInvariantCulture(val, "CustomSemiCircularN4") || CompareWithInvariantCulture(val, "CustomSemiCircularS1") || CompareWithInvariantCulture(val, "CustomSemiCircularS2") || CompareWithInvariantCulture(val, "CustomSemiCircularS3") || CompareWithInvariantCulture(val, "CustomSemiCircularS4") || CompareWithInvariantCulture(val, "CustomSemiCircularE1") || CompareWithInvariantCulture(val, "CustomSemiCircularE2") || CompareWithInvariantCulture(val, "CustomSemiCircularE3") || CompareWithInvariantCulture(val, "CustomSemiCircularE4") || CompareWithInvariantCulture(val, "CustomSemiCircularW1") || CompareWithInvariantCulture(val, "CustomSemiCircularW2") || CompareWithInvariantCulture(val, "CustomSemiCircularW3") || CompareWithInvariantCulture(val, "CustomSemiCircularW4") || CompareWithInvariantCulture(val, "CustomQuarterCircularNE1") || CompareWithInvariantCulture(val, "CustomQuarterCircularNE2") || CompareWithInvariantCulture(val, "CustomQuarterCircularNE3") || CompareWithInvariantCulture(val, "CustomQuarterCircularNE4") || CompareWithInvariantCulture(val, "CustomQuarterCircularNW1") || CompareWithInvariantCulture(val, "CustomQuarterCircularNW2") || CompareWithInvariantCulture(val, "CustomQuarterCircularNW3") || CompareWithInvariantCulture(val, "CustomQuarterCircularNW4") || CompareWithInvariantCulture(val, "CustomQuarterCircularSE1") || CompareWithInvariantCulture(val, "CustomQuarterCircularSE2") || CompareWithInvariantCulture(val, "CustomQuarterCircularSE3") || CompareWithInvariantCulture(val, "CustomQuarterCircularSE4") || CompareWithInvariantCulture(val, "CustomQuarterCircularSW1") || CompareWithInvariantCulture(val, "CustomQuarterCircularSW2") || CompareWithInvariantCulture(val, "CustomQuarterCircularSW3") || CompareWithInvariantCulture(val, "CustomQuarterCircularSW4"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateGaugeFrameStyles(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "None") || CompareWithInvariantCulture(val, "Simple") || CompareWithInvariantCulture(val, "Edged"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateGaugeGlassEffects(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "None") || CompareWithInvariantCulture(val, "Simple"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateGaugeInputValueFormulas(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "None") || CompareWithInvariantCulture(val, "Average") || CompareWithInvariantCulture(val, "Linear") || CompareWithInvariantCulture(val, "Max") || CompareWithInvariantCulture(val, "Min") || CompareWithInvariantCulture(val, "Median") || CompareWithInvariantCulture(val, "OpenClose") || CompareWithInvariantCulture(val, "Percentile") || CompareWithInvariantCulture(val, "Variance") || CompareWithInvariantCulture(val, "RateOfChange") || CompareWithInvariantCulture(val, "Integral"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateGaugeLabelPlacements(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Inside") || CompareWithInvariantCulture(val, "Outside") || CompareWithInvariantCulture(val, "Cross"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateGaugeMarkerStyles(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Triangle") || CompareWithInvariantCulture(val, "None") || CompareWithInvariantCulture(val, "Rectangle") || CompareWithInvariantCulture(val, "Circle") || CompareWithInvariantCulture(val, "Diamond") || CompareWithInvariantCulture(val, "Trapezoid") || CompareWithInvariantCulture(val, "Star") || CompareWithInvariantCulture(val, "Wedge") || CompareWithInvariantCulture(val, "Pentagon"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateGaugeOrientations(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Auto") || CompareWithInvariantCulture(val, "Horizontal") || CompareWithInvariantCulture(val, "Vertical"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateGaugePointerPlacements(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Cross") || CompareWithInvariantCulture(val, "Outside") || CompareWithInvariantCulture(val, "Inside"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateGaugeThermometerStyles(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Standard") || CompareWithInvariantCulture(val, "Flask"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateGaugeTickMarkShapes(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Rectangle") || CompareWithInvariantCulture(val, "None") || CompareWithInvariantCulture(val, "Triangle") || CompareWithInvariantCulture(val, "Circle") || CompareWithInvariantCulture(val, "Diamond") || CompareWithInvariantCulture(val, "Trapezoid") || CompareWithInvariantCulture(val, "Star") || CompareWithInvariantCulture(val, "Wedge") || CompareWithInvariantCulture(val, "Pentagon"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateLinearPointerTypes(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Marker") || CompareWithInvariantCulture(val, "Bar") || CompareWithInvariantCulture(val, "Thermometer"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateRadialPointerNeedleStyles(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Triangular") || CompareWithInvariantCulture(val, "Rectangular") || CompareWithInvariantCulture(val, "TaperedWithTail") || CompareWithInvariantCulture(val, "Tapered") || CompareWithInvariantCulture(val, "ArrowWithTail") || CompareWithInvariantCulture(val, "Arrow") || CompareWithInvariantCulture(val, "StealthArrowWithTail") || CompareWithInvariantCulture(val, "StealthArrow") || CompareWithInvariantCulture(val, "TaperedWithStealthArrow") || CompareWithInvariantCulture(val, "StealthArrowWithWideTail") || CompareWithInvariantCulture(val, "TaperedWithRoundedPoint"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateRadialPointerTypes(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Needle") || CompareWithInvariantCulture(val, "Marker") || CompareWithInvariantCulture(val, "Bar"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateScaleRangePlacements(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Inside") || CompareWithInvariantCulture(val, "Outside") || CompareWithInvariantCulture(val, "Cross"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateBackgroundGradientTypes(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "StartToEnd") || CompareWithInvariantCulture(val, "None") || CompareWithInvariantCulture(val, "LeftRight") || CompareWithInvariantCulture(val, "TopBottom") || CompareWithInvariantCulture(val, "Center") || CompareWithInvariantCulture(val, "DiagonalLeft") || CompareWithInvariantCulture(val, "DiagonalRight") || CompareWithInvariantCulture(val, "HorizontalCenter") || CompareWithInvariantCulture(val, "VerticalCenter"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateTextAntiAliasingQualities(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "High") || CompareWithInvariantCulture(val, "Normal") || CompareWithInvariantCulture(val, "SystemDefault"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateGaugeResizeModes(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "AutoFit") || CompareWithInvariantCulture(val, "None"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateImageSourceType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (ValidateImageSourceType(val))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateMimeType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (ValidateMimeType(val))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateGaugeIndicatorStyles(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Mechanical") || CompareWithInvariantCulture(val, "Digital7Segment") || CompareWithInvariantCulture(val, "Digital14Segment"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateGaugeShowSigns(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "NegativeOnly") || CompareWithInvariantCulture(val, "Both") || CompareWithInvariantCulture(val, "None"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateGaugeStateIndicatorStyles(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Circle") || CompareWithInvariantCulture(val, "Flag") || CompareWithInvariantCulture(val, "ArrowDown") || CompareWithInvariantCulture(val, "ArrowDownIncline") || CompareWithInvariantCulture(val, "ArrowSide") || CompareWithInvariantCulture(val, "ArrowUp") || CompareWithInvariantCulture(val, "ArrowUpIncline") || CompareWithInvariantCulture(val, "BoxesAllFilled") || CompareWithInvariantCulture(val, "BoxesNoneFilled") || CompareWithInvariantCulture(val, "BoxesOneFilled") || CompareWithInvariantCulture(val, "BoxesTwoFilled") || CompareWithInvariantCulture(val, "BoxesThreeFilled") || CompareWithInvariantCulture(val, "LightArrowDown") || CompareWithInvariantCulture(val, "LightArrowDownIncline") || CompareWithInvariantCulture(val, "LightArrowSide") || CompareWithInvariantCulture(val, "LightArrowUp") || CompareWithInvariantCulture(val, "LightArrowUpIncline") || CompareWithInvariantCulture(val, "QuartersAllFilled") || CompareWithInvariantCulture(val, "QuartersNoneFilled") || CompareWithInvariantCulture(val, "QuartersOneFilled") || CompareWithInvariantCulture(val, "QuartersTwoFilled") || CompareWithInvariantCulture(val, "QuartersThreeFilled") || CompareWithInvariantCulture(val, "SignalMeterFourFilled") || CompareWithInvariantCulture(val, "SignalMeterNoneFilled") || CompareWithInvariantCulture(val, "SignalMeterOneFilled") || CompareWithInvariantCulture(val, "SignalMeterThreeFilled") || CompareWithInvariantCulture(val, "SignalMeterTwoFilled") || CompareWithInvariantCulture(val, "StarQuartersAllFilled") || CompareWithInvariantCulture(val, "StarQuartersNoneFilled") || CompareWithInvariantCulture(val, "StarQuartersOneFilled") || CompareWithInvariantCulture(val, "StarQuartersTwoFilled") || CompareWithInvariantCulture(val, "StarQuartersThreeFilled") || CompareWithInvariantCulture(val, "ThreeSignsCircle") || CompareWithInvariantCulture(val, "ThreeSignsDiamond") || CompareWithInvariantCulture(val, "ThreeSignsTriangle") || CompareWithInvariantCulture(val, "ThreeSymbolCheck") || CompareWithInvariantCulture(val, "ThreeSymbolCross") || CompareWithInvariantCulture(val, "ThreeSymbolExclamation") || CompareWithInvariantCulture(val, "ThreeSymbolUnCircledCheck") || CompareWithInvariantCulture(val, "ThreeSymbolUnCircledCross") || CompareWithInvariantCulture(val, "ThreeSymbolUnCircledExclamation") || CompareWithInvariantCulture(val, "TrafficLight") || CompareWithInvariantCulture(val, "TrafficLightUnrimmed") || CompareWithInvariantCulture(val, "TriangleDash") || CompareWithInvariantCulture(val, "TriangleDown") || CompareWithInvariantCulture(val, "TriangleUp") || CompareWithInvariantCulture(val, "ButtonStop") || CompareWithInvariantCulture(val, "ButtonPlay") || CompareWithInvariantCulture(val, "ButtonPause") || CompareWithInvariantCulture(val, "FaceSmile") || CompareWithInvariantCulture(val, "FaceNeutral") || CompareWithInvariantCulture(val, "FaceFrown") || CompareWithInvariantCulture(val, "Image") || CompareWithInvariantCulture(val, "None"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateGaugeTransformationType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (IsStateIndicatorTransformationTypePercent(val) || CompareWithInvariantCulture(val, "None"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool IsStateIndicatorTransformationTypePercent(string val)
		{
			return CompareWithInvariantCulture(val, "Percentage");
		}

		internal static bool ValidateMapLegendTitleSeparator(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "None") || CompareWithInvariantCulture(val, "Line") || CompareWithInvariantCulture(val, "ThickLine") || CompareWithInvariantCulture(val, "DoubleLine") || CompareWithInvariantCulture(val, "DashLine") || CompareWithInvariantCulture(val, "DotLine") || CompareWithInvariantCulture(val, "GradientLine") || CompareWithInvariantCulture(val, "ThickGradientLine"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateMapLegendLayout(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "AutoTable") || CompareWithInvariantCulture(val, "Column") || CompareWithInvariantCulture(val, "Row") || CompareWithInvariantCulture(val, "WideTable") || CompareWithInvariantCulture(val, "TallTable"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateMapAutoBool(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Auto") || CompareWithInvariantCulture(val, "True") || CompareWithInvariantCulture(val, "False"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateLabelPlacement(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Alternate") || CompareWithInvariantCulture(val, "Top") || CompareWithInvariantCulture(val, "Bottom"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateLabelBehavior(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Auto") || CompareWithInvariantCulture(val, "ShowMiddleValue") || CompareWithInvariantCulture(val, "ShowBorderValue"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateUnit(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Percentage") || CompareWithInvariantCulture(val, "Inch") || CompareWithInvariantCulture(val, "Point") || CompareWithInvariantCulture(val, "Centimeter") || CompareWithInvariantCulture(val, "Millimeter") || CompareWithInvariantCulture(val, "Pica"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateLabelPosition(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Near") || CompareWithInvariantCulture(val, "OneQuarter") || CompareWithInvariantCulture(val, "Center") || CompareWithInvariantCulture(val, "ThreeQuarters") || CompareWithInvariantCulture(val, "Far"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateMapPosition(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "TopCenter") || CompareWithInvariantCulture(val, "TopLeft") || CompareWithInvariantCulture(val, "TopRight") || CompareWithInvariantCulture(val, "LeftTop") || CompareWithInvariantCulture(val, "LeftCenter") || CompareWithInvariantCulture(val, "LeftBottom") || CompareWithInvariantCulture(val, "RightTop") || CompareWithInvariantCulture(val, "RightCenter") || CompareWithInvariantCulture(val, "RightBottom") || CompareWithInvariantCulture(val, "BottomRight") || CompareWithInvariantCulture(val, "BottomCenter") || CompareWithInvariantCulture(val, "BottomLeft"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateMapCoordinateSystem(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Planar") || CompareWithInvariantCulture(val, "Geographic"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateMapProjection(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Equirectangular") || CompareWithInvariantCulture(val, "Mercator") || CompareWithInvariantCulture(val, "Robinson") || CompareWithInvariantCulture(val, "Fahey") || CompareWithInvariantCulture(val, "Eckert1") || CompareWithInvariantCulture(val, "Eckert3") || CompareWithInvariantCulture(val, "HammerAitoff") || CompareWithInvariantCulture(val, "Wagner3") || CompareWithInvariantCulture(val, "Bonne"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateMapPalette(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Random") || CompareWithInvariantCulture(val, "Light") || CompareWithInvariantCulture(val, "SemiTransparent") || CompareWithInvariantCulture(val, "BrightPastel") || CompareWithInvariantCulture(val, "Pacific"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateMapRuleDistributionType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Optimal") || CompareWithInvariantCulture(val, "EqualInterval") || CompareWithInvariantCulture(val, "EqualDistribution") || CompareWithInvariantCulture(val, "Custom"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateMapResizeMode(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "AutoFit") || CompareWithInvariantCulture(val, "None"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateMapMarkerStyle(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "None") || CompareWithInvariantCulture(val, "Rectangle") || CompareWithInvariantCulture(val, "Circle") || CompareWithInvariantCulture(val, "Diamond") || CompareWithInvariantCulture(val, "Triangle") || CompareWithInvariantCulture(val, "Trapezoid") || CompareWithInvariantCulture(val, "Star") || CompareWithInvariantCulture(val, "Wedge") || CompareWithInvariantCulture(val, "Pentagon") || CompareWithInvariantCulture(val, "PushPin") || CompareWithInvariantCulture(val, "Image"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateMapLineLabelPlacement(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Above") || CompareWithInvariantCulture(val, "Center") || CompareWithInvariantCulture(val, "Below"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateMapPolygonLabelPlacement(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "MiddleCenter") || CompareWithInvariantCulture(val, "MiddleLeft") || CompareWithInvariantCulture(val, "MiddleRight") || CompareWithInvariantCulture(val, "TopCenter") || CompareWithInvariantCulture(val, "TopLeft") || CompareWithInvariantCulture(val, "TopRight") || CompareWithInvariantCulture(val, "BottomCenter") || CompareWithInvariantCulture(val, "BottomLeft") || CompareWithInvariantCulture(val, "BottomRight"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateMapPointLabelPlacement(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Bottom") || CompareWithInvariantCulture(val, "Top") || CompareWithInvariantCulture(val, "Left") || CompareWithInvariantCulture(val, "Right") || CompareWithInvariantCulture(val, "Center"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateMapTileStyle(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Road") || CompareWithInvariantCulture(val, "Aerial") || CompareWithInvariantCulture(val, "Hybrid"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateMapVisibilityMode(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Visible") || CompareWithInvariantCulture(val, "Hidden") || CompareWithInvariantCulture(val, "ZoomBased"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateDataType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Boolean") || CompareWithInvariantCulture(val, "DateTime") || CompareWithInvariantCulture(val, "Integer") || CompareWithInvariantCulture(val, "Float") || CompareWithInvariantCulture(val, "String"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateMapBorderSkinType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "None") || CompareWithInvariantCulture(val, "Emboss") || CompareWithInvariantCulture(val, "Raised") || CompareWithInvariantCulture(val, "Sunken") || CompareWithInvariantCulture(val, "FrameThin1") || CompareWithInvariantCulture(val, "FrameThin2") || CompareWithInvariantCulture(val, "FrameThin3") || CompareWithInvariantCulture(val, "FrameThin4") || CompareWithInvariantCulture(val, "FrameThin5") || CompareWithInvariantCulture(val, "FrameThin6") || CompareWithInvariantCulture(val, "FrameTitle1") || CompareWithInvariantCulture(val, "FrameTitle2") || CompareWithInvariantCulture(val, "FrameTitle3") || CompareWithInvariantCulture(val, "FrameTitle4") || CompareWithInvariantCulture(val, "FrameTitle5") || CompareWithInvariantCulture(val, "FrameTitle6") || CompareWithInvariantCulture(val, "FrameTitle7") || CompareWithInvariantCulture(val, "FrameTitle8"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateMapAntiAliasing(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "All") || CompareWithInvariantCulture(val, "None") || CompareWithInvariantCulture(val, "Text") || CompareWithInvariantCulture(val, "Graphics"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateMapTextAntiAliasingQuality(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "High") || CompareWithInvariantCulture(val, "Normal") || CompareWithInvariantCulture(val, "SystemDefault"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		private Validator()
		{
		}

		internal static bool ValidateColor(string color, out string newColor, bool allowTransparency)
		{
			if (color == null || (color.Length == 7 && color[0] == '#' && m_colorRegex.Match(color).Success) || (allowTransparency && color.Length == 9 && color[0] == '#' && m_colorRegexTransparency.Match(color).Success))
			{
				newColor = color;
				return true;
			}
			if (ValidateReportColor(color, out string newColor2, out Color _, allowTransparency))
			{
				if (newColor2 == null)
				{
					newColor = color;
				}
				else
				{
					newColor = newColor2;
				}
				return true;
			}
			newColor = null;
			return false;
		}

		internal static bool ValidateColor(string color, out Color c, bool allowTransparency)
		{
			if (color == null)
			{
				c = Color.Empty;
				return true;
			}
			if ((color.Length == 7 && color[0] == '#' && m_colorRegex.Match(color).Success) || (allowTransparency && color.Length == 9 && color[0] == '#' && m_colorRegexTransparency.Match(color).Success))
			{
				ColorFromArgb(color, out c, allowTransparency);
				return true;
			}
			if (ValidateReportColor(color, out string newColor, out c, allowTransparency))
			{
				if (newColor != null)
				{
					ColorFromArgb(newColor, out c, allowTransparency);
				}
				return true;
			}
			c = Color.Empty;
			return false;
		}

		internal static void ParseColor(string color, out Color c, bool allowTransparency)
		{
			if (color == null)
			{
				c = Color.Empty;
			}
			else if ((color.Length == 7 && color[0] == '#' && m_colorRegex.Match(color).Success) || (allowTransparency && color.Length == 9 && color[0] == '#' && m_colorRegexTransparency.Match(color).Success))
			{
				ColorFromArgb(color, out c, allowTransparency);
			}
			else
			{
				c = Color.FromName(color);
			}
		}

		private static void ColorFromArgb(string color, out Color c, bool allowTransparency)
		{
			try
			{
				if (!allowTransparency && color.Length != 7)
				{
					c = Color.FromArgb(0, 0, 0);
					return;
				}
				c = Color.FromArgb(Convert.ToInt32(color.Substring(1), 16));
				if (color.Length == 7)
				{
					c = Color.FromArgb(255, c);
				}
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				c = Color.FromArgb(0, 0, 0);
			}
		}

		private static bool ValidateReportColor(string color, out string newColor, out Color c, bool allowTransparency)
		{
			c = Color.FromName(color);
			if (c.A == 0 && c.R == 0 && c.G == 0 && c.B == 0)
			{
				if (string.Compare("LightGrey", color, StringComparison.OrdinalIgnoreCase) == 0)
				{
					newColor = "#d3d3d3";
					return true;
				}
				newColor = null;
				return false;
			}
			switch (c.ToKnownColor())
			{
			case KnownColor.ActiveBorder:
			case KnownColor.ActiveCaption:
			case KnownColor.ActiveCaptionText:
			case KnownColor.AppWorkspace:
			case KnownColor.Control:
			case KnownColor.ControlDark:
			case KnownColor.ControlDarkDark:
			case KnownColor.ControlLight:
			case KnownColor.ControlLightLight:
			case KnownColor.ControlText:
			case KnownColor.Desktop:
			case KnownColor.GrayText:
			case KnownColor.Highlight:
			case KnownColor.HighlightText:
			case KnownColor.HotTrack:
			case KnownColor.InactiveBorder:
			case KnownColor.InactiveCaption:
			case KnownColor.InactiveCaptionText:
			case KnownColor.Info:
			case KnownColor.InfoText:
			case KnownColor.Menu:
			case KnownColor.MenuText:
			case KnownColor.ScrollBar:
			case KnownColor.Window:
			case KnownColor.WindowFrame:
			case KnownColor.WindowText:
				newColor = null;
				return false;
			case KnownColor.Transparent:
				newColor = null;
				return allowTransparency;
			default:
				newColor = null;
				return true;
			}
		}

		internal static bool ValidateSize(string size, bool allowNegative, out double sizeInMM)
		{
			return ValidateSize(size, allowNegative ? NegativeMin : NormalMin, NormalMax, out sizeInMM);
		}

		private static bool ValidateSize(string size, double minValue, double maxValue, out double sizeInMM)
		{
			if (ValidateSizeString(size, out RVUnit sizeValue) && ValidateSizeUnitType(sizeValue))
			{
				sizeInMM = Converter.ConvertToMM(sizeValue);
				return ValidateSizeValue(sizeInMM, minValue, maxValue);
			}
			sizeInMM = 0.0;
			return false;
		}

		internal static bool ValidateSizeString(string sizeString, out RVUnit sizeValue)
		{
			try
			{
				sizeValue = new RVUnit(sizeString, CultureInfo.InvariantCulture, RVUnitType.Pixel);
				if (sizeValue.Type == RVUnitType.Pixel)
				{
					return false;
				}
				return true;
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				sizeValue = RVUnit.Empty;
				return false;
			}
		}

		internal static bool ValidateSizeUnitType(RVUnit sizeValue)
		{
			switch (sizeValue.Type)
			{
			case RVUnitType.Cm:
			case RVUnitType.Inch:
			case RVUnitType.Mm:
			case RVUnitType.Pica:
			case RVUnitType.Point:
				return true;
			default:
				return false;
			}
		}

		internal static bool ValidateSizeIsPositive(RVUnit sizeValue)
		{
			if (sizeValue.Value >= 0.0)
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateSizeValue(double sizeInMM, double minValue, double maxValue)
		{
			if (sizeInMM >= minValue && sizeInMM <= maxValue)
			{
				return true;
			}
			return false;
		}

		internal static void ParseSize(string size, out double sizeInMM)
		{
			RVUnit unit = RVUnit.Parse(size, CultureInfo.InvariantCulture);
			sizeInMM = Converter.ConvertToMM(unit);
		}

		internal static int CompareDoubles(double first, double second)
		{
			double num = Math.Round(first, 4);
			double num2 = Math.Round(second, 4);
			long num3 = (long)(num * Math.Pow(10.0, 3.0));
			long num4 = (long)(num2 * Math.Pow(10.0, 3.0));
			if (num3 < num4)
			{
				return -1;
			}
			if (num3 > num4)
			{
				return 1;
			}
			return 0;
		}

		internal static bool ValidateEmbeddedImageName(string embeddedImageName, Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo> embeddedImages)
		{
			if (embeddedImageName == null)
			{
				return false;
			}
			return embeddedImages?.ContainsKey(embeddedImageName) ?? false;
		}

		internal static bool ValidateSpecificLanguage(string language, out CultureInfo culture)
		{
			if (language == null)
			{
				culture = null;
				return true;
			}
			try
			{
				culture = CultureInfo.CreateSpecificCulture(language);
				if (culture.IsNeutralCulture)
				{
					culture = null;
					return false;
				}
				culture = new CultureInfo(culture.Name, useUserOverride: false);
				return true;
			}
			catch (ArgumentException)
			{
				culture = null;
				return false;
			}
		}

		internal static bool ValidateLanguage(string language, out CultureInfo culture)
		{
			if (language == null)
			{
				culture = null;
				return true;
			}
			try
			{
				culture = new CultureInfo(language, useUserOverride: false);
				return true;
			}
			catch (ArgumentException)
			{
				culture = null;
				return false;
			}
		}

		private static bool CreateCalendar(string calendarName, out Calendar calendar)
		{
			calendar = null;
			bool result = false;
			if (CompareWithInvariantCulture(calendarName, "GregorianUSEnglish"))
			{
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
			}
			else if (CompareWithInvariantCulture(calendarName, "GregorianArabic"))
			{
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.Arabic);
			}
			else if (CompareWithInvariantCulture(calendarName, "GregorianMiddleEastFrench"))
			{
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.MiddleEastFrench);
			}
			else if (CompareWithInvariantCulture(calendarName, "GregorianTransliteratedEnglish"))
			{
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.TransliteratedEnglish);
			}
			else if (CompareWithInvariantCulture(calendarName, "GregorianTransliteratedFrench"))
			{
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.TransliteratedFrench);
			}
			else if (CompareWithInvariantCulture(calendarName, "Hebrew"))
			{
				calendar = new HebrewCalendar();
			}
			else if (CompareWithInvariantCulture(calendarName, "Hijri"))
			{
				calendar = new HijriCalendar();
			}
			else if (CompareWithInvariantCulture(calendarName, "Japanese"))
			{
				calendar = new JapaneseCalendar();
			}
			else if (CompareWithInvariantCulture(calendarName, "Korean"))
			{
				calendar = new KoreanCalendar();
			}
			else if (CompareWithInvariantCulture(calendarName, "Taiwan"))
			{
				calendar = new TaiwanCalendar();
			}
			else if (CompareWithInvariantCulture(calendarName, "ThaiBuddhist"))
			{
				calendar = new ThaiBuddhistCalendar();
			}
			return result;
		}

		internal static bool ValidateCalendar(CultureInfo langauge, string calendarName)
		{
			if (CompareWithInvariantCulture(calendarName, "Default") || CompareWithInvariantCulture(calendarName, "Gregorian"))
			{
				return true;
			}
			Calendar calendar;
			bool flag = CreateCalendar(calendarName, out calendar);
			if (calendar == null)
			{
				return false;
			}
			Calendar[] optionalCalendars = langauge.OptionalCalendars;
			if (optionalCalendars != null)
			{
				for (int i = 0; i < optionalCalendars.Length; i++)
				{
					if (optionalCalendars[i].GetType() == calendar.GetType())
					{
						if (!flag)
						{
							return true;
						}
						GregorianCalendarTypes calendarType = ((GregorianCalendar)calendar).CalendarType;
						GregorianCalendarTypes calendarType2 = ((GregorianCalendar)optionalCalendars[i]).CalendarType;
						if (calendarType == calendarType2)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		internal static bool ValidateNumeralVariant(CultureInfo language, int numVariant)
		{
			switch (numVariant)
			{
			default:
				return false;
			case 1:
			case 2:
				return true;
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			{
				string text = language.TwoLetterISOLanguageName;
				if (text == null)
				{
					text = language.ThreeLetterISOLanguageName;
				}
				switch (numVariant)
				{
				case 3:
					if (CompareWithInvariantCulture(text, "ar") || CompareWithInvariantCulture(text, "ur") || CompareWithInvariantCulture(text, "fa") || CompareWithInvariantCulture(text, "hi") || CompareWithInvariantCulture(text, "kok") || CompareWithInvariantCulture(text, "mr") || CompareWithInvariantCulture(text, "sa") || CompareWithInvariantCulture(text, "bn") || CompareWithInvariantCulture(text, "pa") || CompareWithInvariantCulture(text, "gu") || CompareWithInvariantCulture(text, "or") || CompareWithInvariantCulture(text, "ta") || CompareWithInvariantCulture(text, "te") || CompareWithInvariantCulture(text, "kn") || CompareWithInvariantCulture(text, "ms") || CompareWithInvariantCulture(text, "th") || CompareWithInvariantCulture(text, "lo") || CompareWithInvariantCulture(text, "bo"))
					{
						return true;
					}
					break;
				case 7:
					if (CompareWithInvariantCulture(text, "ko"))
					{
						return true;
					}
					break;
				default:
					if (CompareWithInvariantCulture(text, "ko") || CompareWithInvariantCulture(text, "ja"))
					{
						return true;
					}
					text = language.Name;
					if (CompareWithInvariantCulture(text, "zh-CHT") || CompareWithInvariantCulture(text, "zh-CHS"))
					{
						return true;
					}
					break;
				}
				return false;
			}
			}
		}

		internal static bool ValidateColumns(int columns)
		{
			if (columns >= 1 && columns <= 1000)
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateNumeralVariant(int numeralVariant)
		{
			if (numeralVariant >= 1 && numeralVariant <= 7)
			{
				return true;
			}
			return false;
		}

		internal static bool ValidatePalette(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Default") || CompareWithInvariantCulture(val, "EarthTones") || CompareWithInvariantCulture(val, "Excel") || CompareWithInvariantCulture(val, "GrayScale") || CompareWithInvariantCulture(val, "Light") || CompareWithInvariantCulture(val, "Pastel") || CompareWithInvariantCulture(val, "SemiTransparent") || CompareWithInvariantCulture(val, "Berry") || CompareWithInvariantCulture(val, "Chocolate") || CompareWithInvariantCulture(val, "Fire") || CompareWithInvariantCulture(val, "SeaGreen") || CompareWithInvariantCulture(val, "BrightPastel") || CompareWithInvariantCulture(val, "Pacific") || CompareWithInvariantCulture(val, "PacificLight") || CompareWithInvariantCulture(val, "PacificSemiTransparent") || CompareWithInvariantCulture(val, "Custom"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidatePaletteHatchBehavior(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Default") || CompareWithInvariantCulture(val, "None") || CompareWithInvariantCulture(val, "Always"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartIntervalType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Default") || CompareWithInvariantCulture(val, "Auto") || CompareWithInvariantCulture(val, "Number") || CompareWithInvariantCulture(val, "Years") || CompareWithInvariantCulture(val, "Months") || CompareWithInvariantCulture(val, "Weeks") || CompareWithInvariantCulture(val, "Days") || CompareWithInvariantCulture(val, "Hours") || CompareWithInvariantCulture(val, "Minutes") || CompareWithInvariantCulture(val, "Seconds") || CompareWithInvariantCulture(val, "Milliseconds"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartTickMarksType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "None") || CompareWithInvariantCulture(val, "Inside") || CompareWithInvariantCulture(val, "Outside") || CompareWithInvariantCulture(val, "Cross"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartColumnType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Text") || CompareWithInvariantCulture(val, "SeriesSymbol"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartCellType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Text") || CompareWithInvariantCulture(val, "SeriesSymbol") || CompareWithInvariantCulture(val, "Image"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartCellAlignment(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Center") || CompareWithInvariantCulture(val, "Top") || CompareWithInvariantCulture(val, "TopLeft") || CompareWithInvariantCulture(val, "TopRight") || CompareWithInvariantCulture(val, "Left") || CompareWithInvariantCulture(val, "Right") || CompareWithInvariantCulture(val, "BottomRight") || CompareWithInvariantCulture(val, "Bottom") || CompareWithInvariantCulture(val, "BottomLeft"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartAllowOutsideChartArea(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Partial") || CompareWithInvariantCulture(val, "True") || CompareWithInvariantCulture(val, "False"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartCalloutLineAnchor(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Arrow") || CompareWithInvariantCulture(val, "Diamond") || CompareWithInvariantCulture(val, "Square") || CompareWithInvariantCulture(val, "Round") || CompareWithInvariantCulture(val, "None"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartCalloutLineStyle(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Solid") || CompareWithInvariantCulture(val, "Dotted") || CompareWithInvariantCulture(val, "Dashed") || CompareWithInvariantCulture(val, "Double") || CompareWithInvariantCulture(val, "DashDot") || CompareWithInvariantCulture(val, "DashDotDot") || CompareWithInvariantCulture(val, "None"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartCalloutStyle(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Underline") || CompareWithInvariantCulture(val, "Box") || CompareWithInvariantCulture(val, "None"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartCustomItemSeparator(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "None") || CompareWithInvariantCulture(val, "Line") || CompareWithInvariantCulture(val, "ThickLine") || CompareWithInvariantCulture(val, "DoubleLine") || CompareWithInvariantCulture(val, "DashLine") || CompareWithInvariantCulture(val, "DotLine") || CompareWithInvariantCulture(val, "GradientLine") || CompareWithInvariantCulture(val, "ThickGradientLine"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartSeriesFormula(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "BollingerBands") || CompareWithInvariantCulture(val, "MovingAverage") || CompareWithInvariantCulture(val, "ExponentialMovingAverage") || CompareWithInvariantCulture(val, "TriangularMovingAverage") || CompareWithInvariantCulture(val, "WeightedMovingAverage") || CompareWithInvariantCulture(val, "MACD") || CompareWithInvariantCulture(val, "DetrendedPriceOscillator") || CompareWithInvariantCulture(val, "Envelopes") || CompareWithInvariantCulture(val, "Performance") || CompareWithInvariantCulture(val, "RateOfChange") || CompareWithInvariantCulture(val, "RelativeStrengthIndex") || CompareWithInvariantCulture(val, "StandardDeviation") || CompareWithInvariantCulture(val, "TRIX") || CompareWithInvariantCulture(val, "Mean") || CompareWithInvariantCulture(val, "Median") || CompareWithInvariantCulture(val, "Error"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartSeriesType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Column") || CompareWithInvariantCulture(val, "Bar") || CompareWithInvariantCulture(val, "Line") || CompareWithInvariantCulture(val, "Shape") || CompareWithInvariantCulture(val, "Scatter") || CompareWithInvariantCulture(val, "Area") || CompareWithInvariantCulture(val, "Range") || CompareWithInvariantCulture(val, "Polar"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartSeriesSubtype(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName, string elementNamespace)
		{
			if (CompareWithInvariantCulture(val, "Plain") || CompareWithInvariantCulture(val, "Stacked") || CompareWithInvariantCulture(val, "PercentStacked") || CompareWithInvariantCulture(val, "Smooth") || CompareWithInvariantCulture(val, "Stepped") || CompareWithInvariantCulture(val, "Pie") || CompareWithInvariantCulture(val, "ExplodedPie") || CompareWithInvariantCulture(val, "Doughnut") || CompareWithInvariantCulture(val, "ExplodedDoughnut") || CompareWithInvariantCulture(val, "Funnel") || CompareWithInvariantCulture(val, "Pyramid") || CompareWithInvariantCulture(val, "Bubble") || CompareWithInvariantCulture(val, "Candlestick") || CompareWithInvariantCulture(val, "Stock") || CompareWithInvariantCulture(val, "Bar") || CompareWithInvariantCulture(val, "Column") || CompareWithInvariantCulture(val, "BoxPlot") || CompareWithInvariantCulture(val, "ErrorBar") || CompareWithInvariantCulture(val, "Radar"))
			{
				return true;
			}
			if (RdlNamespaceComparer.Instance.Compare(elementNamespace, "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition") >= 0 && CompareWithInvariantCulture(val, "Map"))
			{
				return true;
			}
			if (RdlNamespaceComparer.Instance.Compare(elementNamespace, "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition") >= 0 && (CompareWithInvariantCulture(val, "TreeMap") || CompareWithInvariantCulture(val, "Sunburst")))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool IsValidChartSeriesSubType(string type, string subType)
		{
			if (CompareWithInvariantCulture(subType, "Plain"))
			{
				return true;
			}
			if (CompareWithInvariantCulture(type, "Area") && (CompareWithInvariantCulture(subType, "Smooth") || CompareWithInvariantCulture(subType, "Stacked") || CompareWithInvariantCulture(subType, "PercentStacked")))
			{
				return true;
			}
			if ((CompareWithInvariantCulture(type, "Bar") || CompareWithInvariantCulture(type, "Column")) && (CompareWithInvariantCulture(subType, "Stacked") || CompareWithInvariantCulture(subType, "PercentStacked")))
			{
				return true;
			}
			if (CompareWithInvariantCulture(type, "Line") && (CompareWithInvariantCulture(subType, "Smooth") || CompareWithInvariantCulture(subType, "Stepped")))
			{
				return true;
			}
			if (CompareWithInvariantCulture(type, "Polar") && CompareWithInvariantCulture(subType, "Radar"))
			{
				return true;
			}
			if (CompareWithInvariantCulture(type, "Range") && (CompareWithInvariantCulture(subType, "Candlestick") || CompareWithInvariantCulture(subType, "Stock") || CompareWithInvariantCulture(subType, "Smooth") || CompareWithInvariantCulture(subType, "Bar") || CompareWithInvariantCulture(subType, "Column") || CompareWithInvariantCulture(subType, "BoxPlot") || CompareWithInvariantCulture(subType, "ErrorBar")))
			{
				return true;
			}
			if (CompareWithInvariantCulture(type, "Scatter") && CompareWithInvariantCulture(subType, "Bubble"))
			{
				return true;
			}
			if (CompareWithInvariantCulture(type, "Shape") && (CompareWithInvariantCulture(subType, "Pie") || CompareWithInvariantCulture(subType, "ExplodedPie") || CompareWithInvariantCulture(subType, "Doughnut") || CompareWithInvariantCulture(subType, "ExplodedDoughnut") || CompareWithInvariantCulture(subType, "Funnel") || CompareWithInvariantCulture(subType, "Pyramid") || CompareWithInvariantCulture(subType, "TreeMap") || CompareWithInvariantCulture(subType, "Sunburst")))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateChartAxisLocation(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Default") || CompareWithInvariantCulture(val, "Opposite"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartAxisArrow(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "None") || CompareWithInvariantCulture(val, "Triangle") || CompareWithInvariantCulture(val, "SharpTriangle") || CompareWithInvariantCulture(val, "Lines"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartBorderSkinType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "None") || CompareWithInvariantCulture(val, "Emboss") || CompareWithInvariantCulture(val, "Raised") || CompareWithInvariantCulture(val, "Sunken") || CompareWithInvariantCulture(val, "FrameThin1") || CompareWithInvariantCulture(val, "FrameThin2") || CompareWithInvariantCulture(val, "FrameThin3") || CompareWithInvariantCulture(val, "FrameThin4") || CompareWithInvariantCulture(val, "FrameThin5") || CompareWithInvariantCulture(val, "FrameThin6") || CompareWithInvariantCulture(val, "FrameTitle1") || CompareWithInvariantCulture(val, "FrameTitle2") || CompareWithInvariantCulture(val, "FrameTitle3") || CompareWithInvariantCulture(val, "FrameTitle4") || CompareWithInvariantCulture(val, "FrameTitle5") || CompareWithInvariantCulture(val, "FrameTitle6") || CompareWithInvariantCulture(val, "FrameTitle7") || CompareWithInvariantCulture(val, "FrameTitle8"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartTitlePositions(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "TopCenter") || CompareWithInvariantCulture(val, "TopLeft") || CompareWithInvariantCulture(val, "TopRight") || CompareWithInvariantCulture(val, "LeftTop") || CompareWithInvariantCulture(val, "LeftCenter") || CompareWithInvariantCulture(val, "LeftBottom") || CompareWithInvariantCulture(val, "RightTop") || CompareWithInvariantCulture(val, "RightCenter") || CompareWithInvariantCulture(val, "RightBottom") || CompareWithInvariantCulture(val, "BottomRight") || CompareWithInvariantCulture(val, "BottomCenter") || CompareWithInvariantCulture(val, "BottomLeft"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartAxisTitlePositions(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Center") || CompareWithInvariantCulture(val, "Near") || CompareWithInvariantCulture(val, "Far"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartTitleDockings(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Top") || CompareWithInvariantCulture(val, "Left") || CompareWithInvariantCulture(val, "Right") || CompareWithInvariantCulture(val, "Bottom"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartAxisLabelRotation(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "None") || CompareWithInvariantCulture(val, "Rotate15") || CompareWithInvariantCulture(val, "Rotate30") || CompareWithInvariantCulture(val, "Rotate45") || CompareWithInvariantCulture(val, "Rotate90"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartBreakLineType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Ragged") || CompareWithInvariantCulture(val, "None") || CompareWithInvariantCulture(val, "Straight") || CompareWithInvariantCulture(val, "Wave"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartAutoBool(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Auto") || CompareWithInvariantCulture(val, "True") || CompareWithInvariantCulture(val, "False"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartDataLabelPosition(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Auto") || CompareWithInvariantCulture(val, "Top") || CompareWithInvariantCulture(val, "TopLeft") || CompareWithInvariantCulture(val, "TopRight") || CompareWithInvariantCulture(val, "Left") || CompareWithInvariantCulture(val, "Center") || CompareWithInvariantCulture(val, "Right") || CompareWithInvariantCulture(val, "BottomRight") || CompareWithInvariantCulture(val, "BottomLeft") || CompareWithInvariantCulture(val, "Bottom") || CompareWithInvariantCulture(val, "Outside"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartMarkerType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "None") || CompareWithInvariantCulture(val, "Square") || CompareWithInvariantCulture(val, "Circle") || CompareWithInvariantCulture(val, "Diamond") || CompareWithInvariantCulture(val, "Triangle") || CompareWithInvariantCulture(val, "Cross") || CompareWithInvariantCulture(val, "Star4") || CompareWithInvariantCulture(val, "Star5") || CompareWithInvariantCulture(val, "Star6") || CompareWithInvariantCulture(val, "Star10") || CompareWithInvariantCulture(val, "Auto"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartThreeDProjectionMode(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Oblique") || CompareWithInvariantCulture(val, "Perspective"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateChartThreeDShading(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "None") || CompareWithInvariantCulture(val, "Real") || CompareWithInvariantCulture(val, "Simple"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateBackgroundHatchType(string backgroundHatchType)
		{
			if (CompareWithInvariantCulture(backgroundHatchType, "Default") || CompareWithInvariantCulture(backgroundHatchType, "None") || CompareWithInvariantCulture(backgroundHatchType, "BackwardDiagonal") || CompareWithInvariantCulture(backgroundHatchType, "Cross") || CompareWithInvariantCulture(backgroundHatchType, "DarkDownwardDiagonal") || CompareWithInvariantCulture(backgroundHatchType, "DarkHorizontal") || CompareWithInvariantCulture(backgroundHatchType, "DarkUpwardDiagonal") || CompareWithInvariantCulture(backgroundHatchType, "DarkVertical") || CompareWithInvariantCulture(backgroundHatchType, "DashedDownwardDiagonal") || CompareWithInvariantCulture(backgroundHatchType, "DashedHorizontal") || CompareWithInvariantCulture(backgroundHatchType, "DashedUpwardDiagonal") || CompareWithInvariantCulture(backgroundHatchType, "DashedVertical") || CompareWithInvariantCulture(backgroundHatchType, "DiagonalBrick") || CompareWithInvariantCulture(backgroundHatchType, "DiagonalCross") || CompareWithInvariantCulture(backgroundHatchType, "Divot") || CompareWithInvariantCulture(backgroundHatchType, "DottedDiamond") || CompareWithInvariantCulture(backgroundHatchType, "DottedGrid") || CompareWithInvariantCulture(backgroundHatchType, "ForwardDiagonal") || CompareWithInvariantCulture(backgroundHatchType, "Horizontal") || CompareWithInvariantCulture(backgroundHatchType, "HorizontalBrick") || CompareWithInvariantCulture(backgroundHatchType, "LargeCheckerBoard") || CompareWithInvariantCulture(backgroundHatchType, "LargeConfetti") || CompareWithInvariantCulture(backgroundHatchType, "LargeGrid") || CompareWithInvariantCulture(backgroundHatchType, "LightDownwardDiagonal") || CompareWithInvariantCulture(backgroundHatchType, "LightHorizontal") || CompareWithInvariantCulture(backgroundHatchType, "LightUpwardDiagonal") || CompareWithInvariantCulture(backgroundHatchType, "LightVertical") || CompareWithInvariantCulture(backgroundHatchType, "NarrowHorizontal") || CompareWithInvariantCulture(backgroundHatchType, "NarrowVertical") || CompareWithInvariantCulture(backgroundHatchType, "OutlinedDiamond") || CompareWithInvariantCulture(backgroundHatchType, "Percent05") || CompareWithInvariantCulture(backgroundHatchType, "Percent10") || CompareWithInvariantCulture(backgroundHatchType, "Percent20") || CompareWithInvariantCulture(backgroundHatchType, "Percent25") || CompareWithInvariantCulture(backgroundHatchType, "Percent30") || CompareWithInvariantCulture(backgroundHatchType, "Percent40") || CompareWithInvariantCulture(backgroundHatchType, "Percent50") || CompareWithInvariantCulture(backgroundHatchType, "Percent60") || CompareWithInvariantCulture(backgroundHatchType, "Percent70") || CompareWithInvariantCulture(backgroundHatchType, "Percent75") || CompareWithInvariantCulture(backgroundHatchType, "Percent80") || CompareWithInvariantCulture(backgroundHatchType, "Percent90") || CompareWithInvariantCulture(backgroundHatchType, "Plaid") || CompareWithInvariantCulture(backgroundHatchType, "Shingle") || CompareWithInvariantCulture(backgroundHatchType, "SmallCheckerBoard") || CompareWithInvariantCulture(backgroundHatchType, "SmallConfetti") || CompareWithInvariantCulture(backgroundHatchType, "SmallGrid") || CompareWithInvariantCulture(backgroundHatchType, "SolidDiamond") || CompareWithInvariantCulture(backgroundHatchType, "Sphere") || CompareWithInvariantCulture(backgroundHatchType, "Trellis") || CompareWithInvariantCulture(backgroundHatchType, "Vertical") || CompareWithInvariantCulture(backgroundHatchType, "Wave") || CompareWithInvariantCulture(backgroundHatchType, "Weave") || CompareWithInvariantCulture(backgroundHatchType, "WideDownwardDiagonal") || CompareWithInvariantCulture(backgroundHatchType, "WideUpwardDiagonal") || CompareWithInvariantCulture(backgroundHatchType, "ZigZag"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidatePosition(string position)
		{
			if (CompareWithInvariantCulture(position, "Default") || CompareWithInvariantCulture(position, "Top") || CompareWithInvariantCulture(position, "TopLeft") || CompareWithInvariantCulture(position, "TopRight") || CompareWithInvariantCulture(position, "Left") || CompareWithInvariantCulture(position, "Center") || CompareWithInvariantCulture(position, "Right") || CompareWithInvariantCulture(position, "BottomRight") || CompareWithInvariantCulture(position, "Bottom") || CompareWithInvariantCulture(position, "BottomLeft"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateTextEffect(string textEffect)
		{
			if (CompareWithInvariantCulture(textEffect, "Default") || CompareWithInvariantCulture(textEffect, "None") || CompareWithInvariantCulture(textEffect, "Shadow") || CompareWithInvariantCulture(textEffect, "Emboss") || CompareWithInvariantCulture(textEffect, "Embed") || CompareWithInvariantCulture(textEffect, "Frame"))
			{
				return true;
			}
			return false;
		}

		internal static bool IsDynamicImageReportItem(ObjectType objectType)
		{
			if (objectType != ObjectType.Chart && objectType != ObjectType.GaugePanel)
			{
				return objectType == ObjectType.Map;
			}
			return true;
		}

		internal static bool IsDynamicImageSubElement(IStyleContainer styleContainer)
		{
			if (IsDynamicImageReportItem(styleContainer.ObjectType))
			{
				if (!(styleContainer is Microsoft.ReportingServices.ReportIntermediateFormat.Chart) && !(styleContainer is GaugePanel))
				{
					return !(styleContainer is Map);
				}
				return false;
			}
			return false;
		}

		internal static bool ValidateBorderStyle(string borderStyle, bool isDefaultBorder, ObjectType objectType, bool isDynamicImageSubElement, out string validatedStyle)
		{
			bool flag = IsDynamicImageReportItem(objectType);
			bool flag2 = objectType == ObjectType.Line;
			if (CompareWithInvariantCulture(borderStyle, "Dotted") || CompareWithInvariantCulture(borderStyle, "Dashed"))
			{
				validatedStyle = borderStyle;
				return true;
			}
			if (CompareWithInvariantCulture(borderStyle, "None") || CompareWithInvariantCulture(borderStyle, "Solid") || CompareWithInvariantCulture(borderStyle, "Double"))
			{
				if (flag2)
				{
					validatedStyle = "Solid";
				}
				else
				{
					validatedStyle = borderStyle;
				}
				return true;
			}
			if (CompareWithInvariantCulture(borderStyle, "DashDot") || CompareWithInvariantCulture(borderStyle, "DashDotDot"))
			{
				if (flag)
				{
					if (isDynamicImageSubElement)
					{
						validatedStyle = borderStyle;
					}
					else
					{
						validatedStyle = "Dashed";
					}
				}
				else
				{
					validatedStyle = null;
				}
				return flag;
			}
			if (CompareWithInvariantCulture(borderStyle, "Default"))
			{
				if (isDefaultBorder)
				{
					if (flag2)
					{
						validatedStyle = "Solid";
					}
					else
					{
						validatedStyle = "None";
					}
				}
				else
				{
					validatedStyle = borderStyle;
				}
				return true;
			}
			validatedStyle = null;
			return false;
		}

		internal static bool ValidateImageSourceType(string val)
		{
			if (CompareWithInvariantCulture(val, "External") || CompareWithInvariantCulture(val, "Embedded") || CompareWithInvariantCulture(val, "Database"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateMimeType(string mimeType)
		{
			if (mimeType == null)
			{
				return false;
			}
			if (CompareWithInvariantCultureIgnoreCase(mimeType, "image/bmp") || CompareWithInvariantCultureIgnoreCase(mimeType, "image/jpeg") || CompareWithInvariantCultureIgnoreCase(mimeType, "image/gif") || CompareWithInvariantCultureIgnoreCase(mimeType, "image/png") || CompareWithInvariantCultureIgnoreCase(mimeType, "image/x-png"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateBackgroundGradientType(string gradientType)
		{
			if (CompareWithInvariantCulture(gradientType, "Default") || CompareWithInvariantCulture(gradientType, "None") || CompareWithInvariantCulture(gradientType, "LeftRight") || CompareWithInvariantCulture(gradientType, "TopBottom") || CompareWithInvariantCulture(gradientType, "Center") || CompareWithInvariantCulture(gradientType, "DiagonalLeft") || CompareWithInvariantCulture(gradientType, "DiagonalRight") || CompareWithInvariantCulture(gradientType, "HorizontalCenter") || CompareWithInvariantCulture(gradientType, "VerticalCenter"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateBackgroundRepeatForNamespace(string repeat, string rdlNamespace)
		{
			if (CompareWithInvariantCulture(repeat, "FitProportional"))
			{
				return RdlNamespaceComparer.Instance.Compare(rdlNamespace, "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition") >= 0;
			}
			return true;
		}

		internal static bool ValidateBackgroundRepeat(string repeat, ObjectType objectType)
		{
			bool flag = objectType == ObjectType.Chart;
			bool flag2 = objectType == ObjectType.ReportSection;
			if (CompareWithInvariantCulture(repeat, "Default") || CompareWithInvariantCulture(repeat, "Repeat") || CompareWithInvariantCulture(repeat, "Clip"))
			{
				return true;
			}
			if (CompareWithInvariantCulture(repeat, "RepeatX") || CompareWithInvariantCulture(repeat, "RepeatY"))
			{
				return !flag;
			}
			if (CompareWithInvariantCulture(repeat, "Fit"))
			{
				return flag || flag2;
			}
			if (CompareWithInvariantCulture(repeat, "FitProportional"))
			{
				return flag2;
			}
			return false;
		}

		internal static bool ValidateFontStyle(string fontStyle)
		{
			if (CompareWithInvariantCulture(fontStyle, "Default") || CompareWithInvariantCulture(fontStyle, "Normal") || CompareWithInvariantCulture(fontStyle, "Italic"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateFontWeight(string fontWeight)
		{
			if (CompareWithInvariantCulture(fontWeight, "Thin") || CompareWithInvariantCulture(fontWeight, "ExtraLight") || CompareWithInvariantCulture(fontWeight, "Light") || CompareWithInvariantCulture(fontWeight, "Normal") || CompareWithInvariantCulture(fontWeight, "Default") || CompareWithInvariantCulture(fontWeight, "Medium") || CompareWithInvariantCulture(fontWeight, "SemiBold") || CompareWithInvariantCulture(fontWeight, "Bold") || CompareWithInvariantCulture(fontWeight, "ExtraBold") || CompareWithInvariantCulture(fontWeight, "Heavy"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateTextDecoration(string textDecoration)
		{
			if (CompareWithInvariantCulture(textDecoration, "Default") || CompareWithInvariantCulture(textDecoration, "None") || CompareWithInvariantCulture(textDecoration, "Underline") || CompareWithInvariantCulture(textDecoration, "Overline") || CompareWithInvariantCulture(textDecoration, "LineThrough"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateTextAlign(string textAlign)
		{
			if (CompareWithInvariantCulture(textAlign, "Default") || CompareWithInvariantCulture(textAlign, "General") || CompareWithInvariantCulture(textAlign, "Left") || CompareWithInvariantCulture(textAlign, "Center") || CompareWithInvariantCulture(textAlign, "Right"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateVerticalAlign(string verticalAlign)
		{
			if (CompareWithInvariantCulture(verticalAlign, "Default") || CompareWithInvariantCulture(verticalAlign, "Top") || CompareWithInvariantCulture(verticalAlign, "Middle") || CompareWithInvariantCulture(verticalAlign, "Bottom"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateDirection(string direction)
		{
			if (CompareWithInvariantCulture(direction, "Default") || CompareWithInvariantCulture(direction, "LTR") || CompareWithInvariantCulture(direction, "RTL"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateWritingMode(string writingMode)
		{
			if (CompareWithInvariantCulture(writingMode, "Default") || CompareWithInvariantCulture(writingMode, "Horizontal") || CompareWithInvariantCulture(writingMode, "Vertical") || CompareWithInvariantCulture(writingMode, "Rotate270"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateUnicodeBiDi(string unicodeBiDi)
		{
			if (CompareWithInvariantCulture(unicodeBiDi, "Normal") || CompareWithInvariantCulture(unicodeBiDi, "Embed") || CompareWithInvariantCulture(unicodeBiDi, "BiDiOverride"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateCalendar(string calendar)
		{
			if (CompareWithInvariantCulture(calendar, "Default") || CompareWithInvariantCulture(calendar, "Gregorian") || CompareWithInvariantCulture(calendar, "GregorianUSEnglish") || CompareWithInvariantCulture(calendar, "GregorianArabic") || CompareWithInvariantCulture(calendar, "GregorianMiddleEastFrench") || CompareWithInvariantCulture(calendar, "GregorianTransliteratedEnglish") || CompareWithInvariantCulture(calendar, "GregorianTransliteratedFrench") || CompareWithInvariantCulture(calendar, "Hebrew") || CompareWithInvariantCulture(calendar, "Hijri") || CompareWithInvariantCulture(calendar, "Japanese") || CompareWithInvariantCulture(calendar, "Korean") || CompareWithInvariantCulture(calendar, "Taiwan") || CompareWithInvariantCulture(calendar, "ThaiBuddhist"))
			{
				return true;
			}
			return false;
		}

		internal static bool CompareWithInvariantCulture(string strOne, string strTwo)
		{
			if (Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(strOne, strTwo, ignoreCase: false) == 0)
			{
				return true;
			}
			return false;
		}

		internal static bool CompareWithInvariantCultureIgnoreCase(string strOne, string strTwo)
		{
			if (Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(strOne, strTwo, ignoreCase: true) == 0)
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateTextOrientations(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (CompareWithInvariantCulture(val, "Auto") || CompareWithInvariantCulture(val, "Horizontal") || CompareWithInvariantCulture(val, "Rotated90") || CompareWithInvariantCulture(val, "Rotated270") || CompareWithInvariantCulture(val, "Stacked"))
			{
				return true;
			}
			RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		private static void RegisterInvalidEnumValueError(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Error, context.ObjectType, context.ObjectName, propertyName, val);
		}

		internal static bool ValidateTextRunMarkupType(string value)
		{
			if (CompareWithInvariantCulture(value, "None") || CompareWithInvariantCulture(value, "HTML"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateParagraphListStyle(string value)
		{
			if (CompareWithInvariantCulture(value, "None") || CompareWithInvariantCulture(value, "Numbered") || CompareWithInvariantCulture(value, "Bulleted"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateParagraphListLevel(int value, out int? adjustedValue)
		{
			if (value < 0)
			{
				adjustedValue = null;
				return false;
			}
			if (value > 9)
			{
				adjustedValue = 9;
				return false;
			}
			adjustedValue = value;
			return true;
		}
	}
}

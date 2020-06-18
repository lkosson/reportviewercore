using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class EnumTranslator
	{
		internal static GaugeFrameShapes TranslateGaugeFrameShapes(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeFrameShapes.Default;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Default"))
			{
				return GaugeFrameShapes.Default;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Circular"))
			{
				return GaugeFrameShapes.Circular;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rectangular"))
			{
				return GaugeFrameShapes.Rectangular;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedRectangular"))
			{
				return GaugeFrameShapes.RoundedRectangular;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "AutoShape"))
			{
				return GaugeFrameShapes.AutoShape;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular1"))
			{
				return GaugeFrameShapes.CustomCircular1;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular2"))
			{
				return GaugeFrameShapes.CustomCircular2;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular3"))
			{
				return GaugeFrameShapes.CustomCircular3;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular4"))
			{
				return GaugeFrameShapes.CustomCircular4;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular5"))
			{
				return GaugeFrameShapes.CustomCircular5;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular6"))
			{
				return GaugeFrameShapes.CustomCircular6;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular7"))
			{
				return GaugeFrameShapes.CustomCircular7;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular8"))
			{
				return GaugeFrameShapes.CustomCircular8;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular9"))
			{
				return GaugeFrameShapes.CustomCircular9;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular10"))
			{
				return GaugeFrameShapes.CustomCircular10;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular11"))
			{
				return GaugeFrameShapes.CustomCircular11;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular12"))
			{
				return GaugeFrameShapes.CustomCircular12;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular13"))
			{
				return GaugeFrameShapes.CustomCircular13;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular14"))
			{
				return GaugeFrameShapes.CustomCircular14;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular15"))
			{
				return GaugeFrameShapes.CustomCircular15;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularN1"))
			{
				return GaugeFrameShapes.CustomSemiCircularN1;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularN2"))
			{
				return GaugeFrameShapes.CustomSemiCircularN2;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularN3"))
			{
				return GaugeFrameShapes.CustomSemiCircularN3;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularN4"))
			{
				return GaugeFrameShapes.CustomSemiCircularN4;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularS1"))
			{
				return GaugeFrameShapes.CustomSemiCircularS1;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularS2"))
			{
				return GaugeFrameShapes.CustomSemiCircularS2;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularS3"))
			{
				return GaugeFrameShapes.CustomSemiCircularS3;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularS4"))
			{
				return GaugeFrameShapes.CustomSemiCircularS4;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularE1"))
			{
				return GaugeFrameShapes.CustomSemiCircularE1;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularE2"))
			{
				return GaugeFrameShapes.CustomSemiCircularE2;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularE3"))
			{
				return GaugeFrameShapes.CustomSemiCircularE3;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularE4"))
			{
				return GaugeFrameShapes.CustomSemiCircularE4;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularW1"))
			{
				return GaugeFrameShapes.CustomSemiCircularW1;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularW2"))
			{
				return GaugeFrameShapes.CustomSemiCircularW2;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularW3"))
			{
				return GaugeFrameShapes.CustomSemiCircularW3;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularW4"))
			{
				return GaugeFrameShapes.CustomSemiCircularW4;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNE1"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNE1;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNE2"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNE2;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNE3"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNE3;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNE4"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNE4;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNW1"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNW1;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNW2"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNW2;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNW3"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNW3;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNW4"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNW4;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSE1"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSE1;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSE2"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSE2;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSE3"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSE3;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSE4"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSE4;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSW1"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSW1;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSW2"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSW2;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSW3"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSW3;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSW4"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSW4;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeFrameShapes.Default;
		}

		internal static GaugeFrameStyles TranslateGaugeFrameStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeFrameStyles.None;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeFrameStyles.None;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Simple"))
			{
				return GaugeFrameStyles.Simple;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Edged"))
			{
				return GaugeFrameStyles.Edged;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeFrameStyles.None;
		}

		internal static GaugeAntiAliasings TranslateGaugeAntiAliasings(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeAntiAliasings.All;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "All"))
			{
				return GaugeAntiAliasings.All;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeAntiAliasings.None;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Text"))
			{
				return GaugeAntiAliasings.Text;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Graphics"))
			{
				return GaugeAntiAliasings.Graphics;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeAntiAliasings.All;
		}

		internal static GaugeGlassEffects TranslateGaugeGlassEffects(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeGlassEffects.None;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeGlassEffects.None;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Simple"))
			{
				return GaugeGlassEffects.Simple;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeGlassEffects.None;
		}

		internal static GaugeBarStarts TranslateGaugeBarStarts(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeBarStarts.ScaleStart;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ScaleStart"))
			{
				return GaugeBarStarts.ScaleStart;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Zero"))
			{
				return GaugeBarStarts.Zero;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeBarStarts.ScaleStart;
		}

		internal static GaugeCapStyles TranslateGaugeCapStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeCapStyles.RoundedDark;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedDark"))
			{
				return GaugeCapStyles.RoundedDark;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rounded"))
			{
				return GaugeCapStyles.Rounded;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedLight"))
			{
				return GaugeCapStyles.RoundedLight;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedWithAdditionalTop"))
			{
				return GaugeCapStyles.RoundedWithAdditionalTop;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedWithWideIndentation"))
			{
				return GaugeCapStyles.RoundedWithWideIndentation;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FlattenedWithIndentation"))
			{
				return GaugeCapStyles.FlattenedWithIndentation;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FlattenedWithWideIndentation"))
			{
				return GaugeCapStyles.FlattenedWithWideIndentation;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedGlossyWithIndentation"))
			{
				return GaugeCapStyles.RoundedGlossyWithIndentation;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedWithIndentation"))
			{
				return GaugeCapStyles.RoundedWithIndentation;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeCapStyles.RoundedDark;
		}

		internal static GaugeInputValueFormulas TranslateGaugeInputValueFormulas(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeInputValueFormulas.None;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeInputValueFormulas.None;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Average"))
			{
				return GaugeInputValueFormulas.Average;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Linear"))
			{
				return GaugeInputValueFormulas.Linear;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Max"))
			{
				return GaugeInputValueFormulas.Max;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Min"))
			{
				return GaugeInputValueFormulas.Min;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Median"))
			{
				return GaugeInputValueFormulas.Median;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "OpenClose"))
			{
				return GaugeInputValueFormulas.OpenClose;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Percentile"))
			{
				return GaugeInputValueFormulas.Percentile;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Variance"))
			{
				return GaugeInputValueFormulas.Variance;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RateOfChange"))
			{
				return GaugeInputValueFormulas.RateOfChange;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Integral"))
			{
				return GaugeInputValueFormulas.Integral;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeInputValueFormulas.None;
		}

		internal static GaugeLabelPlacements TranslateGaugeLabelPlacements(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeLabelPlacements.Inside;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Inside"))
			{
				return GaugeLabelPlacements.Inside;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Outside"))
			{
				return GaugeLabelPlacements.Outside;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Cross"))
			{
				return GaugeLabelPlacements.Cross;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeLabelPlacements.Inside;
		}

		internal static GaugeMarkerStyles TranslateGaugeMarkerStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeMarkerStyles.Triangle;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Triangle"))
			{
				return GaugeMarkerStyles.Triangle;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeMarkerStyles.None;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rectangle"))
			{
				return GaugeMarkerStyles.Rectangle;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Circle"))
			{
				return GaugeMarkerStyles.Circle;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Diamond"))
			{
				return GaugeMarkerStyles.Diamond;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Trapezoid"))
			{
				return GaugeMarkerStyles.Trapezoid;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star"))
			{
				return GaugeMarkerStyles.Star;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Wedge"))
			{
				return GaugeMarkerStyles.Wedge;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pentagon"))
			{
				return GaugeMarkerStyles.Pentagon;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeMarkerStyles.Triangle;
		}

		internal static GaugeOrientations TranslateGaugeOrientations(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeOrientations.Auto;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
			{
				return GaugeOrientations.Auto;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Horizontal"))
			{
				return GaugeOrientations.Horizontal;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Vertical"))
			{
				return GaugeOrientations.Vertical;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeOrientations.Auto;
		}

		internal static GaugePointerPlacements TranslateGaugePointerPlacements(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugePointerPlacements.Cross;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Cross"))
			{
				return GaugePointerPlacements.Cross;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Outside"))
			{
				return GaugePointerPlacements.Outside;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Inside"))
			{
				return GaugePointerPlacements.Inside;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugePointerPlacements.Cross;
		}

		internal static GaugeThermometerStyles TranslateGaugeThermometerStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeThermometerStyles.Standard;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Standard"))
			{
				return GaugeThermometerStyles.Standard;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Flask"))
			{
				return GaugeThermometerStyles.Flask;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeThermometerStyles.Standard;
		}

		internal static GaugeTickMarkShapes TranslateGaugeTickMarkShapes(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeTickMarkShapes.Rectangle;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rectangle"))
			{
				return GaugeTickMarkShapes.Rectangle;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeTickMarkShapes.None;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Triangle"))
			{
				return GaugeTickMarkShapes.Triangle;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Circle"))
			{
				return GaugeTickMarkShapes.Circle;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Diamond"))
			{
				return GaugeTickMarkShapes.Diamond;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Trapezoid"))
			{
				return GaugeTickMarkShapes.Trapezoid;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star"))
			{
				return GaugeTickMarkShapes.Star;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Wedge"))
			{
				return GaugeTickMarkShapes.Wedge;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pentagon"))
			{
				return GaugeTickMarkShapes.Pentagon;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeTickMarkShapes.Rectangle;
		}

		internal static LinearPointerTypes TranslateLinearPointerTypes(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return LinearPointerTypes.Marker;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Marker"))
			{
				return LinearPointerTypes.Marker;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bar"))
			{
				return LinearPointerTypes.Bar;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Thermometer"))
			{
				return LinearPointerTypes.Thermometer;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return LinearPointerTypes.Marker;
		}

		internal static RadialPointerNeedleStyles TranslateRadialPointerNeedleStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return RadialPointerNeedleStyles.Triangular;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Triangular"))
			{
				return RadialPointerNeedleStyles.Triangular;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rectangular"))
			{
				return RadialPointerNeedleStyles.Rectangular;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TaperedWithTail"))
			{
				return RadialPointerNeedleStyles.TaperedWithTail;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Tapered"))
			{
				return RadialPointerNeedleStyles.Tapered;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ArrowWithTail"))
			{
				return RadialPointerNeedleStyles.ArrowWithTail;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Arrow"))
			{
				return RadialPointerNeedleStyles.Arrow;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StealthArrowWithTail"))
			{
				return RadialPointerNeedleStyles.StealthArrowWithTail;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StealthArrow"))
			{
				return RadialPointerNeedleStyles.StealthArrow;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TaperedWithStealthArrow"))
			{
				return RadialPointerNeedleStyles.TaperedWithStealthArrow;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StealthArrowWithWideTail"))
			{
				return RadialPointerNeedleStyles.StealthArrowWithWideTail;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TaperedWithRoundedPoint"))
			{
				return RadialPointerNeedleStyles.TaperedWithRoundedPoint;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return RadialPointerNeedleStyles.Triangular;
		}

		internal static RadialPointerTypes TranslateRadialPointerTypes(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return RadialPointerTypes.Needle;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Needle"))
			{
				return RadialPointerTypes.Needle;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Marker"))
			{
				return RadialPointerTypes.Marker;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bar"))
			{
				return RadialPointerTypes.Bar;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return RadialPointerTypes.Needle;
		}

		internal static ScaleRangePlacements TranslateScaleRangePlacements(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return ScaleRangePlacements.Inside;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Inside"))
			{
				return ScaleRangePlacements.Inside;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Outside"))
			{
				return ScaleRangePlacements.Outside;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Cross"))
			{
				return ScaleRangePlacements.Cross;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return ScaleRangePlacements.Inside;
		}

		internal static BackgroundGradientTypes TranslateBackgroundGradientTypes(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return BackgroundGradientTypes.StartToEnd;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StartToEnd"))
			{
				return BackgroundGradientTypes.StartToEnd;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return BackgroundGradientTypes.None;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftRight"))
			{
				return BackgroundGradientTypes.LeftRight;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopBottom"))
			{
				return BackgroundGradientTypes.TopBottom;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
			{
				return BackgroundGradientTypes.Center;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DiagonalLeft"))
			{
				return BackgroundGradientTypes.DiagonalLeft;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DiagonalRight"))
			{
				return BackgroundGradientTypes.DiagonalRight;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "HorizontalCenter"))
			{
				return BackgroundGradientTypes.HorizontalCenter;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "VerticalCenter"))
			{
				return BackgroundGradientTypes.VerticalCenter;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return BackgroundGradientTypes.StartToEnd;
		}

		internal static TextAntiAliasingQualities TranslateTextAntiAliasingQualities(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return TextAntiAliasingQualities.High;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "High"))
			{
				return TextAntiAliasingQualities.High;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Normal"))
			{
				return TextAntiAliasingQualities.Normal;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SystemDefault"))
			{
				return TextAntiAliasingQualities.SystemDefault;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return TextAntiAliasingQualities.High;
		}

		internal static GaugeResizeModes TranslateGaugeResizeModes(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeResizeModes.AutoFit;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "AutoFit"))
			{
				return GaugeResizeModes.AutoFit;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeResizeModes.None;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeResizeModes.AutoFit;
		}

		internal static GaugeIndicatorStyles TranslateGaugeIndicatorStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeIndicatorStyles.Mechanical;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Mechanical"))
			{
				return GaugeIndicatorStyles.Mechanical;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Digital7Segment"))
			{
				return GaugeIndicatorStyles.Digital7Segment;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Digital14Segment"))
			{
				return GaugeIndicatorStyles.Digital14Segment;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeIndicatorStyles.Mechanical;
		}

		internal static GaugeShowSigns TranslateGaugeShowSigns(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeShowSigns.NegativeOnly;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "NegativeOnly"))
			{
				return GaugeShowSigns.NegativeOnly;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Both"))
			{
				return GaugeShowSigns.Both;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeShowSigns.None;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeShowSigns.NegativeOnly;
		}

		internal static GaugeStateIndicatorStyles TranslateGaugeStateIndicatorStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeStateIndicatorStyles.Circle;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Circle"))
			{
				return GaugeStateIndicatorStyles.Circle;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Flag"))
			{
				return GaugeStateIndicatorStyles.Flag;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ArrowDown"))
			{
				return GaugeStateIndicatorStyles.ArrowDown;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ArrowDownIncline"))
			{
				return GaugeStateIndicatorStyles.ArrowDownIncline;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ArrowSide"))
			{
				return GaugeStateIndicatorStyles.ArrowSide;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ArrowUp"))
			{
				return GaugeStateIndicatorStyles.ArrowUp;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ArrowUpIncline"))
			{
				return GaugeStateIndicatorStyles.ArrowUpIncline;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BoxesAllFilled"))
			{
				return GaugeStateIndicatorStyles.BoxesAllFilled;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BoxesNoneFilled"))
			{
				return GaugeStateIndicatorStyles.BoxesNoneFilled;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BoxesOneFilled"))
			{
				return GaugeStateIndicatorStyles.BoxesOneFilled;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BoxesTwoFilled"))
			{
				return GaugeStateIndicatorStyles.BoxesTwoFilled;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BoxesThreeFilled"))
			{
				return GaugeStateIndicatorStyles.BoxesThreeFilled;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LightArrowDown"))
			{
				return GaugeStateIndicatorStyles.LightArrowDown;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LightArrowDownIncline"))
			{
				return GaugeStateIndicatorStyles.LightArrowDownIncline;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LightArrowSide"))
			{
				return GaugeStateIndicatorStyles.LightArrowSide;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LightArrowUp"))
			{
				return GaugeStateIndicatorStyles.LightArrowUp;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LightArrowUpIncline"))
			{
				return GaugeStateIndicatorStyles.LightArrowUpIncline;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "QuartersAllFilled"))
			{
				return GaugeStateIndicatorStyles.QuartersAllFilled;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "QuartersNoneFilled"))
			{
				return GaugeStateIndicatorStyles.QuartersNoneFilled;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "QuartersOneFilled"))
			{
				return GaugeStateIndicatorStyles.QuartersOneFilled;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "QuartersTwoFilled"))
			{
				return GaugeStateIndicatorStyles.QuartersTwoFilled;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "QuartersThreeFilled"))
			{
				return GaugeStateIndicatorStyles.QuartersThreeFilled;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SignalMeterFourFilled"))
			{
				return GaugeStateIndicatorStyles.SignalMeterFourFilled;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SignalMeterNoneFilled"))
			{
				return GaugeStateIndicatorStyles.SignalMeterNoneFilled;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SignalMeterOneFilled"))
			{
				return GaugeStateIndicatorStyles.SignalMeterOneFilled;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SignalMeterThreeFilled"))
			{
				return GaugeStateIndicatorStyles.SignalMeterThreeFilled;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SignalMeterTwoFilled"))
			{
				return GaugeStateIndicatorStyles.SignalMeterTwoFilled;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StarQuartersAllFilled"))
			{
				return GaugeStateIndicatorStyles.StarQuartersAllFilled;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StarQuartersNoneFilled"))
			{
				return GaugeStateIndicatorStyles.StarQuartersNoneFilled;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StarQuartersOneFilled"))
			{
				return GaugeStateIndicatorStyles.StarQuartersOneFilled;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StarQuartersTwoFilled"))
			{
				return GaugeStateIndicatorStyles.StarQuartersTwoFilled;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StarQuartersThreeFilled"))
			{
				return GaugeStateIndicatorStyles.StarQuartersThreeFilled;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSignsCircle"))
			{
				return GaugeStateIndicatorStyles.ThreeSignsCircle;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSignsDiamond"))
			{
				return GaugeStateIndicatorStyles.ThreeSignsDiamond;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSignsTriangle"))
			{
				return GaugeStateIndicatorStyles.ThreeSignsTriangle;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSymbolCheck"))
			{
				return GaugeStateIndicatorStyles.ThreeSymbolCheck;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSymbolCross"))
			{
				return GaugeStateIndicatorStyles.ThreeSymbolCross;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSymbolExclamation"))
			{
				return GaugeStateIndicatorStyles.ThreeSymbolExclamation;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSymbolUnCircledCheck"))
			{
				return GaugeStateIndicatorStyles.ThreeSymbolUnCircledCheck;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSymbolUnCircledCross"))
			{
				return GaugeStateIndicatorStyles.ThreeSymbolUnCircledCross;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSymbolUnCircledExclamation"))
			{
				return GaugeStateIndicatorStyles.ThreeSymbolUnCircledExclamation;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TrafficLight"))
			{
				return GaugeStateIndicatorStyles.TrafficLight;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TrafficLightUnrimmed"))
			{
				return GaugeStateIndicatorStyles.TrafficLightUnrimmed;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TriangleDash"))
			{
				return GaugeStateIndicatorStyles.TriangleDash;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TriangleDown"))
			{
				return GaugeStateIndicatorStyles.TriangleDown;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TriangleUp"))
			{
				return GaugeStateIndicatorStyles.TriangleUp;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ButtonStop"))
			{
				return GaugeStateIndicatorStyles.ButtonStop;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ButtonPlay"))
			{
				return GaugeStateIndicatorStyles.ButtonPlay;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ButtonPause"))
			{
				return GaugeStateIndicatorStyles.ButtonPause;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FaceSmile"))
			{
				return GaugeStateIndicatorStyles.FaceSmile;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FaceNeutral"))
			{
				return GaugeStateIndicatorStyles.FaceNeutral;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FaceFrown"))
			{
				return GaugeStateIndicatorStyles.FaceFrown;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Image"))
			{
				return GaugeStateIndicatorStyles.Image;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeStateIndicatorStyles.None;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeStateIndicatorStyles.Circle;
		}

		internal static GaugeTransformationType TranslateGaugeTransformationType(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeTransformationType.Percentage;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Percentage"))
			{
				return GaugeTransformationType.Percentage;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeTransformationType.None;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return GaugeTransformationType.Percentage;
		}

		internal static MapLegendTitleSeparator TranslateMapLegendTitleSeparator(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapLegendTitleSeparator.None;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return MapLegendTitleSeparator.None;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Line"))
			{
				return MapLegendTitleSeparator.Line;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThickLine"))
			{
				return MapLegendTitleSeparator.ThickLine;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DoubleLine"))
			{
				return MapLegendTitleSeparator.DoubleLine;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DashLine"))
			{
				return MapLegendTitleSeparator.DashLine;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DotLine"))
			{
				return MapLegendTitleSeparator.DotLine;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "GradientLine"))
			{
				return MapLegendTitleSeparator.GradientLine;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThickGradientLine"))
			{
				return MapLegendTitleSeparator.ThickGradientLine;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapLegendTitleSeparator.None;
		}

		internal static MapLegendLayout TranslateMapLegendLayout(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapLegendLayout.AutoTable;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "AutoTable"))
			{
				return MapLegendLayout.AutoTable;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Column"))
			{
				return MapLegendLayout.Column;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Row"))
			{
				return MapLegendLayout.Row;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "WideTable"))
			{
				return MapLegendLayout.WideTable;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TallTable"))
			{
				return MapLegendLayout.TallTable;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapLegendLayout.AutoTable;
		}

		internal static MapAutoBool TranslateMapAutoBool(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapAutoBool.Auto;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
			{
				return MapAutoBool.Auto;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "True"))
			{
				return MapAutoBool.True;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "False"))
			{
				return MapAutoBool.False;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapAutoBool.Auto;
		}

		internal static MapLabelPlacement TranslateLabelPlacement(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapLabelPlacement.Alternate;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Alternate"))
			{
				return MapLabelPlacement.Alternate;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Top"))
			{
				return MapLabelPlacement.Top;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bottom"))
			{
				return MapLabelPlacement.Bottom;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapLabelPlacement.Alternate;
		}

		internal static MapLabelBehavior TranslateLabelBehavior(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapLabelBehavior.Auto;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
			{
				return MapLabelBehavior.Auto;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ShowMiddleValue"))
			{
				return MapLabelBehavior.ShowMiddleValue;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ShowBorderValue"))
			{
				return MapLabelBehavior.ShowBorderValue;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapLabelBehavior.Auto;
		}

		internal static Unit TranslateUnit(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return Unit.Percentage;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Percentage"))
			{
				return Unit.Percentage;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Inch"))
			{
				return Unit.Inch;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Point"))
			{
				return Unit.Point;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Centimeter"))
			{
				return Unit.Centimeter;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Millimeter"))
			{
				return Unit.Millimeter;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pica"))
			{
				return Unit.Pica;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return Unit.Percentage;
		}

		internal static MapLabelPosition TranslateLabelPosition(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapLabelPosition.Near;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Near"))
			{
				return MapLabelPosition.Near;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "OneQuarter"))
			{
				return MapLabelPosition.OneQuarter;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
			{
				return MapLabelPosition.Center;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeQuarters"))
			{
				return MapLabelPosition.ThreeQuarters;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Far"))
			{
				return MapLabelPosition.Far;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapLabelPosition.Near;
		}

		internal static MapPosition TranslateMapPosition(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapPosition.TopCenter;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopCenter"))
			{
				return MapPosition.TopCenter;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopLeft"))
			{
				return MapPosition.TopLeft;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopRight"))
			{
				return MapPosition.TopRight;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftTop"))
			{
				return MapPosition.LeftTop;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftCenter"))
			{
				return MapPosition.LeftCenter;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftBottom"))
			{
				return MapPosition.LeftBottom;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightTop"))
			{
				return MapPosition.RightTop;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightCenter"))
			{
				return MapPosition.RightCenter;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightBottom"))
			{
				return MapPosition.RightBottom;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomRight"))
			{
				return MapPosition.BottomRight;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomCenter"))
			{
				return MapPosition.BottomCenter;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomLeft"))
			{
				return MapPosition.BottomLeft;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapPosition.TopCenter;
		}

		internal static MapCoordinateSystem TranslateMapCoordinateSystem(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapCoordinateSystem.Planar;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Planar"))
			{
				return MapCoordinateSystem.Planar;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Geographic"))
			{
				return MapCoordinateSystem.Geographic;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapCoordinateSystem.Planar;
		}

		internal static MapProjection TranslateMapProjection(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapProjection.Equirectangular;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Equirectangular"))
			{
				return MapProjection.Equirectangular;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Mercator"))
			{
				return MapProjection.Mercator;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Robinson"))
			{
				return MapProjection.Robinson;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Fahey"))
			{
				return MapProjection.Fahey;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Eckert1"))
			{
				return MapProjection.Eckert1;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Eckert3"))
			{
				return MapProjection.Eckert3;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "HammerAitoff"))
			{
				return MapProjection.HammerAitoff;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Wagner3"))
			{
				return MapProjection.Wagner3;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bonne"))
			{
				return MapProjection.Bonne;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapProjection.Equirectangular;
		}

		internal static MapRuleDistributionType TranslateMapRuleDistributionType(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapRuleDistributionType.Optimal;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Optimal"))
			{
				return MapRuleDistributionType.Optimal;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "EqualInterval"))
			{
				return MapRuleDistributionType.EqualInterval;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "EqualDistribution"))
			{
				return MapRuleDistributionType.EqualDistribution;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Custom"))
			{
				return MapRuleDistributionType.Custom;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapRuleDistributionType.Optimal;
		}

		internal static MapMarkerStyle TranslateMapMarkerStyle(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapMarkerStyle.None;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return MapMarkerStyle.None;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rectangle"))
			{
				return MapMarkerStyle.Rectangle;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Circle"))
			{
				return MapMarkerStyle.Circle;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Diamond"))
			{
				return MapMarkerStyle.Diamond;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Triangle"))
			{
				return MapMarkerStyle.Triangle;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Trapezoid"))
			{
				return MapMarkerStyle.Trapezoid;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star"))
			{
				return MapMarkerStyle.Star;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Wedge"))
			{
				return MapMarkerStyle.Wedge;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pentagon"))
			{
				return MapMarkerStyle.Pentagon;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "PushPin"))
			{
				return MapMarkerStyle.PushPin;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Image"))
			{
				return MapMarkerStyle.Image;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapMarkerStyle.None;
		}

		internal static MapResizeMode TranslateMapResizeMode(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapResizeMode.AutoFit;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "AutoFit"))
			{
				return MapResizeMode.AutoFit;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return MapResizeMode.None;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapResizeMode.AutoFit;
		}

		internal static MapPalette TranslateMapPalette(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapPalette.Random;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Random"))
			{
				return MapPalette.Random;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Light"))
			{
				return MapPalette.Light;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SemiTransparent"))
			{
				return MapPalette.SemiTransparent;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BrightPastel"))
			{
				return MapPalette.BrightPastel;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pacific"))
			{
				return MapPalette.Pacific;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapPalette.Random;
		}

		internal static MapLineLabelPlacement TranslateMapLineLabelPlacement(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapLineLabelPlacement.Above;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Above"))
			{
				return MapLineLabelPlacement.Above;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
			{
				return MapLineLabelPlacement.Center;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Below"))
			{
				return MapLineLabelPlacement.Below;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapLineLabelPlacement.Above;
		}

		internal static MapPolygonLabelPlacement TranslateMapPolygonLabelPlacement(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapPolygonLabelPlacement.MiddleCenter;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "MiddleCenter"))
			{
				return MapPolygonLabelPlacement.MiddleCenter;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "MiddleLeft"))
			{
				return MapPolygonLabelPlacement.MiddleLeft;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "MiddleRight"))
			{
				return MapPolygonLabelPlacement.MiddleRight;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopCenter"))
			{
				return MapPolygonLabelPlacement.TopCenter;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopLeft"))
			{
				return MapPolygonLabelPlacement.TopLeft;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopRight"))
			{
				return MapPolygonLabelPlacement.TopRight;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomCenter"))
			{
				return MapPolygonLabelPlacement.BottomCenter;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomLeft"))
			{
				return MapPolygonLabelPlacement.BottomLeft;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomRight"))
			{
				return MapPolygonLabelPlacement.BottomRight;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapPolygonLabelPlacement.MiddleCenter;
		}

		internal static MapPointLabelPlacement TranslateMapPointLabelPlacement(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapPointLabelPlacement.Bottom;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bottom"))
			{
				return MapPointLabelPlacement.Bottom;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Top"))
			{
				return MapPointLabelPlacement.Top;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Left"))
			{
				return MapPointLabelPlacement.Left;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Right"))
			{
				return MapPointLabelPlacement.Right;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
			{
				return MapPointLabelPlacement.Center;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapPointLabelPlacement.Bottom;
		}

		internal static MapTileStyle TranslateMapTileStyle(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapTileStyle.Road;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Road"))
			{
				return MapTileStyle.Road;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Aerial"))
			{
				return MapTileStyle.Aerial;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Hybrid"))
			{
				return MapTileStyle.Hybrid;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapTileStyle.Road;
		}

		internal static MapVisibilityMode TranslateMapVisibilityMode(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapVisibilityMode.Visible;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Visible"))
			{
				return MapVisibilityMode.Visible;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Hidden"))
			{
				return MapVisibilityMode.Hidden;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ZoomBased"))
			{
				return MapVisibilityMode.ZoomBased;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapVisibilityMode.Visible;
		}

		internal static MapDataType TranslateDataType(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapDataType.String;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Boolean"))
			{
				return MapDataType.Boolean;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DateTime"))
			{
				return MapDataType.DateTime;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Integer"))
			{
				return MapDataType.Integer;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Float"))
			{
				return MapDataType.Float;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "String"))
			{
				return MapDataType.String;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Decimal"))
			{
				return MapDataType.Decimal;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapDataType.String;
		}

		internal static MapAntiAliasing TranslateMapAntiAliasing(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapAntiAliasing.All;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "All"))
			{
				return MapAntiAliasing.All;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return MapAntiAliasing.None;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Text"))
			{
				return MapAntiAliasing.Text;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Graphics"))
			{
				return MapAntiAliasing.Graphics;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapAntiAliasing.All;
		}

		internal static MapTextAntiAliasingQuality TranslateMapTextAntiAliasingQuality(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapTextAntiAliasingQuality.High;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "High"))
			{
				return MapTextAntiAliasingQuality.High;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Normal"))
			{
				return MapTextAntiAliasingQuality.Normal;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SystemDefault"))
			{
				return MapTextAntiAliasingQuality.SystemDefault;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapTextAntiAliasingQuality.High;
		}

		internal static MapBorderSkinType TranslateMapBorderSkinType(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapBorderSkinType.None;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return MapBorderSkinType.None;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Emboss"))
			{
				return MapBorderSkinType.Emboss;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Raised"))
			{
				return MapBorderSkinType.Raised;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Sunken"))
			{
				return MapBorderSkinType.Sunken;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin1"))
			{
				return MapBorderSkinType.FrameThin1;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin2"))
			{
				return MapBorderSkinType.FrameThin2;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin3"))
			{
				return MapBorderSkinType.FrameThin3;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin4"))
			{
				return MapBorderSkinType.FrameThin4;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin5"))
			{
				return MapBorderSkinType.FrameThin5;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin6"))
			{
				return MapBorderSkinType.FrameThin6;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle1"))
			{
				return MapBorderSkinType.FrameTitle1;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle2"))
			{
				return MapBorderSkinType.FrameTitle2;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle3"))
			{
				return MapBorderSkinType.FrameTitle3;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle4"))
			{
				return MapBorderSkinType.FrameTitle4;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle5"))
			{
				return MapBorderSkinType.FrameTitle5;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle6"))
			{
				return MapBorderSkinType.FrameTitle6;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle7"))
			{
				return MapBorderSkinType.FrameTitle7;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle8"))
			{
				return MapBorderSkinType.FrameTitle8;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return MapBorderSkinType.None;
		}

		internal static ChartBreakLineType TranslateChartBreakLineType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Ragged"))
				{
					return ChartBreakLineType.Ragged;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartBreakLineType.None;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Straight"))
				{
					return ChartBreakLineType.Straight;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Wave"))
				{
					return ChartBreakLineType.Wave;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartBreakLineType.Ragged;
		}

		internal static ChartIntervalType TranslateChartIntervalType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Default"))
				{
					return ChartIntervalType.Default;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
				{
					return ChartIntervalType.Auto;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Number"))
				{
					return ChartIntervalType.Number;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Years"))
				{
					return ChartIntervalType.Years;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Months"))
				{
					return ChartIntervalType.Months;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Weeks"))
				{
					return ChartIntervalType.Weeks;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Days"))
				{
					return ChartIntervalType.Days;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Hours"))
				{
					return ChartIntervalType.Hours;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Minutes"))
				{
					return ChartIntervalType.Minutes;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Seconds"))
				{
					return ChartIntervalType.Seconds;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Milliseconds"))
				{
					return ChartIntervalType.Milliseconds;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartIntervalType.Default;
		}

		internal static ChartAutoBool TranslateChartAutoBool(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
				{
					return ChartAutoBool.Auto;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "True"))
				{
					return ChartAutoBool.True;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "False"))
				{
					return ChartAutoBool.False;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartAutoBool.Auto;
		}

		internal static ChartAxisLabelRotation TranslateChartAxisLabelRotation(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartAxisLabelRotation.None;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rotate30"))
				{
					return ChartAxisLabelRotation.Rotate30;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rotate45"))
				{
					return ChartAxisLabelRotation.Rotate45;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rotate90"))
				{
					return ChartAxisLabelRotation.Rotate90;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartAxisLabelRotation.None;
		}

		internal static ChartAxisLocation TranslateChartAxisLocation(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Default"))
				{
					return ChartAxisLocation.Default;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Opposite"))
				{
					return ChartAxisLocation.Opposite;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartAxisLocation.Default;
		}

		internal static ChartAxisArrow TranslateChartAxisArrow(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartAxisArrow.None;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Triangle"))
				{
					return ChartAxisArrow.Triangle;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SharpTriangle"))
				{
					return ChartAxisArrow.SharpTriangle;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Lines"))
				{
					return ChartAxisArrow.Lines;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartAxisArrow.None;
		}

		internal static ChartTickMarksType TranslateChartTickMarksType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartTickMarksType.None;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Inside"))
				{
					return ChartTickMarksType.Inside;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Outside"))
				{
					return ChartTickMarksType.Outside;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Cross"))
				{
					return ChartTickMarksType.Cross;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartTickMarksType.None;
		}

		internal static ChartColumnType TranslateChartColumnType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Text"))
				{
					return ChartColumnType.Text;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SeriesSymbol"))
				{
					return ChartColumnType.SeriesSymbol;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartColumnType.Text;
		}

		internal static ChartCellType TranslateChartCellType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Text"))
				{
					return ChartCellType.Text;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SeriesSymbol"))
				{
					return ChartCellType.SeriesSymbol;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Image"))
				{
					return ChartCellType.Image;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartCellType.Text;
		}

		internal static ChartCellAlignment TranslateChartCellAlignment(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
				{
					return ChartCellAlignment.Center;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Top"))
				{
					return ChartCellAlignment.Top;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopLeft"))
				{
					return ChartCellAlignment.TopLeft;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopRight"))
				{
					return ChartCellAlignment.TopRight;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Left"))
				{
					return ChartCellAlignment.Left;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Right"))
				{
					return ChartCellAlignment.Right;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomRight"))
				{
					return ChartCellAlignment.BottomRight;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bottom"))
				{
					return ChartCellAlignment.Bottom;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomLeft"))
				{
					return ChartCellAlignment.BottomLeft;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartCellAlignment.Center;
		}

		internal static ChartAllowOutsideChartArea TranslateChartAllowOutsideChartArea(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Partial"))
				{
					return ChartAllowOutsideChartArea.Partial;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "True"))
				{
					return ChartAllowOutsideChartArea.True;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "False"))
				{
					return ChartAllowOutsideChartArea.False;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartAllowOutsideChartArea.Partial;
		}

		internal static ChartCalloutLineAnchor TranslateChartCalloutLineAnchor(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Arrow"))
				{
					return ChartCalloutLineAnchor.Arrow;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Diamond"))
				{
					return ChartCalloutLineAnchor.Diamond;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Square"))
				{
					return ChartCalloutLineAnchor.Square;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Round"))
				{
					return ChartCalloutLineAnchor.Round;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartCalloutLineAnchor.None;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartCalloutLineAnchor.Arrow;
		}

		internal static ChartCalloutLineStyle TranslateChartCalloutLineStyle(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Solid"))
				{
					return ChartCalloutLineStyle.Solid;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Dotted"))
				{
					return ChartCalloutLineStyle.Dotted;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Dashed"))
				{
					return ChartCalloutLineStyle.Dashed;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Double"))
				{
					return ChartCalloutLineStyle.Double;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DashDot"))
				{
					return ChartCalloutLineStyle.DashDot;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DashDotDot"))
				{
					return ChartCalloutLineStyle.DashDotDot;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartCalloutLineStyle.None;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartCalloutLineStyle.Solid;
		}

		internal static ChartCalloutStyle TranslateChartCalloutStyle(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Underline"))
				{
					return ChartCalloutStyle.Underline;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Box"))
				{
					return ChartCalloutStyle.Box;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartCalloutStyle.None;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartCalloutStyle.Underline;
		}

		internal static ChartSeriesFormula TranslateChartSeriesFormula(string val)
		{
			if (val == null)
			{
				return ChartSeriesFormula.BollingerBands;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BollingerBands"))
			{
				return ChartSeriesFormula.BollingerBands;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "MovingAverage"))
			{
				return ChartSeriesFormula.MovingAverage;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ExponentialMovingAverage"))
			{
				return ChartSeriesFormula.ExponentialMovingAverage;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TriangularMovingAverage"))
			{
				return ChartSeriesFormula.TriangularMovingAverage;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "WeightedMovingAverage"))
			{
				return ChartSeriesFormula.WeightedMovingAverage;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "MACD"))
			{
				return ChartSeriesFormula.MACD;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DetrendedPriceOscillator"))
			{
				return ChartSeriesFormula.DetrendedPriceOscillator;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Envelopes"))
			{
				return ChartSeriesFormula.Envelopes;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Performance"))
			{
				return ChartSeriesFormula.Performance;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RateOfChange"))
			{
				return ChartSeriesFormula.RateOfChange;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RelativeStrengthIndex"))
			{
				return ChartSeriesFormula.RelativeStrengthIndex;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StandardDeviation"))
			{
				return ChartSeriesFormula.StandardDeviation;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TRIX"))
			{
				return ChartSeriesFormula.TRIX;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Mean"))
			{
				return ChartSeriesFormula.Mean;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Median"))
			{
				return ChartSeriesFormula.Median;
			}
			return ChartSeriesFormula.BollingerBands;
		}

		internal static ChartTitlePositions TranslateChartTitlePosition(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopCenter"))
				{
					return ChartTitlePositions.TopCenter;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopLeft"))
				{
					return ChartTitlePositions.TopLeft;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopRight"))
				{
					return ChartTitlePositions.TopRight;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftTop"))
				{
					return ChartTitlePositions.LeftTop;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftCenter"))
				{
					return ChartTitlePositions.LeftCenter;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftBottom"))
				{
					return ChartTitlePositions.LeftBottom;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightTop"))
				{
					return ChartTitlePositions.RightTop;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightCenter"))
				{
					return ChartTitlePositions.RightCenter;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightBottom"))
				{
					return ChartTitlePositions.RightBottom;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomRight"))
				{
					return ChartTitlePositions.BottomRight;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomCenter"))
				{
					return ChartTitlePositions.BottomCenter;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomLeft"))
				{
					return ChartTitlePositions.BottomLeft;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartTitlePositions.TopCenter;
		}

		internal static ChartTitleDockings TranslateChartTitleDocking(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Top"))
				{
					return ChartTitleDockings.Top;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Left"))
				{
					return ChartTitleDockings.Left;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Right"))
				{
					return ChartTitleDockings.Right;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bottom"))
				{
					return ChartTitleDockings.Bottom;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartTitleDockings.Top;
		}

		internal static ChartAxisTitlePositions TranslateChartAxisTitlePosition(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
				{
					return ChartAxisTitlePositions.Center;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Near"))
				{
					return ChartAxisTitlePositions.Near;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Far"))
				{
					return ChartAxisTitlePositions.Far;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartAxisTitlePositions.Center;
		}

		internal static ChartSeparators TranslateChartSeparator(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartSeparators.None;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Line"))
				{
					return ChartSeparators.Line;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThickLine"))
				{
					return ChartSeparators.ThickLine;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DoubleLine"))
				{
					return ChartSeparators.DoubleLine;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DashLine"))
				{
					return ChartSeparators.DashLine;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DotLine"))
				{
					return ChartSeparators.DotLine;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "GradientLine"))
				{
					return ChartSeparators.GradientLine;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThickGradientLine"))
				{
					return ChartSeparators.ThickGradientLine;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartSeparators.None;
		}

		internal static ChartLegendLayouts TranslateChartLegendLayout(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Column"))
				{
					return ChartLegendLayouts.Column;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Row"))
				{
					return ChartLegendLayouts.Row;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "AutoTable"))
				{
					return ChartLegendLayouts.AutoTable;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "WideTable"))
				{
					return ChartLegendLayouts.WideTable;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TallTable"))
				{
					return ChartLegendLayouts.TallTable;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartLegendLayouts.AutoTable;
		}

		internal static ChartLegendPositions TranslateChartLegendPositions(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopCenter"))
				{
					return ChartLegendPositions.TopCenter;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopLeft"))
				{
					return ChartLegendPositions.TopLeft;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopRight"))
				{
					return ChartLegendPositions.TopRight;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftTop"))
				{
					return ChartLegendPositions.LeftTop;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftCenter"))
				{
					return ChartLegendPositions.LeftCenter;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftBottom"))
				{
					return ChartLegendPositions.LeftBottom;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightTop"))
				{
					return ChartLegendPositions.RightTop;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightCenter"))
				{
					return ChartLegendPositions.RightCenter;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightBottom"))
				{
					return ChartLegendPositions.RightBottom;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomRight"))
				{
					return ChartLegendPositions.BottomRight;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomCenter"))
				{
					return ChartLegendPositions.BottomCenter;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomLeft"))
				{
					return ChartLegendPositions.BottomLeft;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartLegendPositions.RightTop;
		}

		internal static ChartAreaAlignOrientations TranslateChartAreaAlignOrientation(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartAreaAlignOrientations.None;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Vertical"))
				{
					return ChartAreaAlignOrientations.Vertical;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Horizontal"))
				{
					return ChartAreaAlignOrientations.Horizontal;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "All"))
				{
					return ChartAreaAlignOrientations.All;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartAreaAlignOrientations.None;
		}

		internal static ChartThreeDProjectionModes TranslateChartThreeDProjectionMode(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Oblique"))
				{
					return ChartThreeDProjectionModes.Oblique;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Perspective"))
				{
					return ChartThreeDProjectionModes.Perspective;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartThreeDProjectionModes.Oblique;
		}

		internal static ChartThreeDShadingTypes TranslateChartThreeDShading(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartThreeDShadingTypes.None;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Real"))
				{
					return ChartThreeDShadingTypes.Real;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Simple"))
				{
					return ChartThreeDShadingTypes.Simple;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartThreeDShadingTypes.Real;
		}

		internal static ChartBorderSkinType TranslateChartBorderSkinType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartBorderSkinType.None;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Emboss"))
				{
					return ChartBorderSkinType.Emboss;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Raised"))
				{
					return ChartBorderSkinType.Raised;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Sunken"))
				{
					return ChartBorderSkinType.Sunken;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin1"))
				{
					return ChartBorderSkinType.FrameThin1;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin2"))
				{
					return ChartBorderSkinType.FrameThin2;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin3"))
				{
					return ChartBorderSkinType.FrameThin3;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin4"))
				{
					return ChartBorderSkinType.FrameThin4;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin5"))
				{
					return ChartBorderSkinType.FrameThin5;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin6"))
				{
					return ChartBorderSkinType.FrameThin6;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle1"))
				{
					return ChartBorderSkinType.FrameTitle1;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle2"))
				{
					return ChartBorderSkinType.FrameTitle2;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle3"))
				{
					return ChartBorderSkinType.FrameTitle3;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle4"))
				{
					return ChartBorderSkinType.FrameTitle4;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle5"))
				{
					return ChartBorderSkinType.FrameTitle5;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle6"))
				{
					return ChartBorderSkinType.FrameTitle6;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle7"))
				{
					return ChartBorderSkinType.FrameTitle7;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle8"))
				{
					return ChartBorderSkinType.FrameTitle8;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartBorderSkinType.None;
		}

		internal static ChartSeriesType TranslateChartSeriesType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Column"))
				{
					return ChartSeriesType.Column;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bar"))
				{
					return ChartSeriesType.Bar;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Line"))
				{
					return ChartSeriesType.Line;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Shape"))
				{
					return ChartSeriesType.Shape;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Scatter"))
				{
					return ChartSeriesType.Scatter;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Area"))
				{
					return ChartSeriesType.Area;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Range"))
				{
					return ChartSeriesType.Range;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Polar"))
				{
					return ChartSeriesType.Polar;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartSeriesType.Column;
		}

		internal static ChartSeriesSubtype TranslateChartSeriesSubtype(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Plain"))
				{
					return ChartSeriesSubtype.Plain;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Stacked"))
				{
					return ChartSeriesSubtype.Stacked;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "PercentStacked"))
				{
					return ChartSeriesSubtype.PercentStacked;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Smooth"))
				{
					return ChartSeriesSubtype.Smooth;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Stepped"))
				{
					return ChartSeriesSubtype.Stepped;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pie"))
				{
					return ChartSeriesSubtype.Pie;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ExplodedPie"))
				{
					return ChartSeriesSubtype.ExplodedPie;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Doughnut"))
				{
					return ChartSeriesSubtype.Doughnut;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ExplodedDoughnut"))
				{
					return ChartSeriesSubtype.ExplodedDoughnut;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Funnel"))
				{
					return ChartSeriesSubtype.Funnel;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pyramid"))
				{
					return ChartSeriesSubtype.Pyramid;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TreeMap"))
				{
					return ChartSeriesSubtype.TreeMap;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Sunburst"))
				{
					return ChartSeriesSubtype.Sunburst;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bubble"))
				{
					return ChartSeriesSubtype.Bubble;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Candlestick"))
				{
					return ChartSeriesSubtype.Candlestick;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Stock"))
				{
					return ChartSeriesSubtype.Stock;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bar"))
				{
					return ChartSeriesSubtype.Bar;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Column"))
				{
					return ChartSeriesSubtype.Column;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BoxPlot"))
				{
					return ChartSeriesSubtype.BoxPlot;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ErrorBar"))
				{
					return ChartSeriesSubtype.ErrorBar;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Radar"))
				{
					return ChartSeriesSubtype.Radar;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartSeriesSubtype.Plain;
		}

		internal static ChartDataLabelPositions TranslateChartDataLabelPosition(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
				{
					return ChartDataLabelPositions.Auto;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Top"))
				{
					return ChartDataLabelPositions.Top;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopLeft"))
				{
					return ChartDataLabelPositions.TopLeft;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopRight"))
				{
					return ChartDataLabelPositions.TopRight;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Left"))
				{
					return ChartDataLabelPositions.Left;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
				{
					return ChartDataLabelPositions.Center;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Right"))
				{
					return ChartDataLabelPositions.Right;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomRight"))
				{
					return ChartDataLabelPositions.BottomRight;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomLeft"))
				{
					return ChartDataLabelPositions.BottomLeft;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bottom"))
				{
					return ChartDataLabelPositions.Bottom;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Outside"))
				{
					return ChartDataLabelPositions.Outside;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartDataLabelPositions.Auto;
		}

		internal static ChartMarkerTypes TranslateChartMarkerType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartMarkerTypes.None;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Square"))
				{
					return ChartMarkerTypes.Square;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Circle"))
				{
					return ChartMarkerTypes.Circle;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Diamond"))
				{
					return ChartMarkerTypes.Diamond;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Triangle"))
				{
					return ChartMarkerTypes.Triangle;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Cross"))
				{
					return ChartMarkerTypes.Cross;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star4"))
				{
					return ChartMarkerTypes.Star4;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star5"))
				{
					return ChartMarkerTypes.Star5;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star6"))
				{
					return ChartMarkerTypes.Star6;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star10"))
				{
					return ChartMarkerTypes.Star10;
				}
				if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
				{
					return ChartMarkerTypes.Auto;
				}
				errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartMarkerTypes.None;
		}

		internal static ChartPalette TranslateChartPalette(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return ChartPalette.Default;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Default"))
			{
				return ChartPalette.Default;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "EarthTones"))
			{
				return ChartPalette.EarthTones;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Excel"))
			{
				return ChartPalette.Excel;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "GrayScale"))
			{
				return ChartPalette.GrayScale;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Light"))
			{
				return ChartPalette.Light;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pastel"))
			{
				return ChartPalette.Pastel;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SemiTransparent"))
			{
				return ChartPalette.SemiTransparent;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Berry"))
			{
				return ChartPalette.Berry;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Chocolate"))
			{
				return ChartPalette.Chocolate;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Fire"))
			{
				return ChartPalette.Fire;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SeaGreen"))
			{
				return ChartPalette.SeaGreen;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BrightPastel"))
			{
				return ChartPalette.BrightPastel;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pacific"))
			{
				return ChartPalette.Pacific;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "PacificLight"))
			{
				return ChartPalette.PacificLight;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "PacificSemiTransparent"))
			{
				return ChartPalette.PacificSemiTransparent;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Custom"))
			{
				return ChartPalette.Custom;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return ChartPalette.Default;
		}

		internal static PaletteHatchBehavior TranslatePaletteHatchBehavior(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return PaletteHatchBehavior.Default;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Default"))
			{
				return PaletteHatchBehavior.Default;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return PaletteHatchBehavior.None;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Always"))
			{
				return PaletteHatchBehavior.Always;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return PaletteHatchBehavior.Default;
		}

		internal static Image.SourceType TranslateImageSourceType(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return Image.SourceType.External;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "External"))
			{
				return Image.SourceType.External;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Embedded"))
			{
				return Image.SourceType.Embedded;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Database"))
			{
				return Image.SourceType.Database;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return Image.SourceType.External;
		}

		internal static TextOrientations TranslateTextOrientations(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return TextOrientations.Auto;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
			{
				return TextOrientations.Auto;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Horizontal"))
			{
				return TextOrientations.Horizontal;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rotated90"))
			{
				return TextOrientations.Rotated90;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rotated270"))
			{
				return TextOrientations.Rotated270;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Stacked"))
			{
				return TextOrientations.Stacked;
			}
			errorContext?.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			return TextOrientations.Auto;
		}
	}
}

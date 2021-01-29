using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class ROMEnumStrings : EnumStrings
	{
		public static string GetValue(Microsoft.ReportingServices.OnDemandReportRendering.FontStyles val)
		{
			switch (val)
			{
			case Microsoft.ReportingServices.OnDemandReportRendering.FontStyles.Normal:
				return "normal";
			case Microsoft.ReportingServices.OnDemandReportRendering.FontStyles.Italic:
				return "italic";
			default:
				return null;
			}
		}

		public static string GetValue(Microsoft.ReportingServices.OnDemandReportRendering.FontWeights val)
		{
			switch (val)
			{
			case Microsoft.ReportingServices.OnDemandReportRendering.FontWeights.Normal:
				return "400";
			case Microsoft.ReportingServices.OnDemandReportRendering.FontWeights.Thin:
				return "100";
			case Microsoft.ReportingServices.OnDemandReportRendering.FontWeights.ExtraLight:
				return "200";
			case Microsoft.ReportingServices.OnDemandReportRendering.FontWeights.Light:
				return "300";
			case Microsoft.ReportingServices.OnDemandReportRendering.FontWeights.Medium:
				return "500";
			case Microsoft.ReportingServices.OnDemandReportRendering.FontWeights.SemiBold:
				return "600";
			case Microsoft.ReportingServices.OnDemandReportRendering.FontWeights.Bold:
				return "700";
			case Microsoft.ReportingServices.OnDemandReportRendering.FontWeights.ExtraBold:
				return "800";
			case Microsoft.ReportingServices.OnDemandReportRendering.FontWeights.Heavy:
				return "900";
			default:
				return null;
			}
		}

		public static string GetValue(Microsoft.ReportingServices.OnDemandReportRendering.TextDecorations val)
		{
			switch (val)
			{
			case Microsoft.ReportingServices.OnDemandReportRendering.TextDecorations.None:
				return "none";
			case Microsoft.ReportingServices.OnDemandReportRendering.TextDecorations.Underline:
				return "underline";
			case Microsoft.ReportingServices.OnDemandReportRendering.TextDecorations.Overline:
				return "overline";
			case Microsoft.ReportingServices.OnDemandReportRendering.TextDecorations.LineThrough:
				return "line-through";
			default:
				return null;
			}
		}

		public static string GetValue(Microsoft.ReportingServices.OnDemandReportRendering.TextAlignments val)
		{
			switch (val)
			{
			case Microsoft.ReportingServices.OnDemandReportRendering.TextAlignments.Left:
				return "left";
			case Microsoft.ReportingServices.OnDemandReportRendering.TextAlignments.Center:
				return "center";
			case Microsoft.ReportingServices.OnDemandReportRendering.TextAlignments.Right:
				return "right";
			default:
				return null;
			}
		}

		public static string GetValue(Microsoft.ReportingServices.OnDemandReportRendering.VerticalAlignments val)
		{
			switch (val)
			{
			case Microsoft.ReportingServices.OnDemandReportRendering.VerticalAlignments.Top:
				return "top";
			case Microsoft.ReportingServices.OnDemandReportRendering.VerticalAlignments.Middle:
				return "middle";
			case Microsoft.ReportingServices.OnDemandReportRendering.VerticalAlignments.Bottom:
				return "bottom";
			default:
				return null;
			}
		}

		public static string GetValue(Microsoft.ReportingServices.OnDemandReportRendering.Directions val)
		{
			switch (val)
			{
			case Microsoft.ReportingServices.OnDemandReportRendering.Directions.LTR:
				return "ltr";
			case Microsoft.ReportingServices.OnDemandReportRendering.Directions.RTL:
				return "rtl";
			default:
				return null;
			}
		}

		public static string GetValue(Microsoft.ReportingServices.OnDemandReportRendering.UnicodeBiDiTypes val)
		{
			switch (val)
			{
			case Microsoft.ReportingServices.OnDemandReportRendering.UnicodeBiDiTypes.Normal:
				return "normal";
			case Microsoft.ReportingServices.OnDemandReportRendering.UnicodeBiDiTypes.Embed:
				return "embed";
			case Microsoft.ReportingServices.OnDemandReportRendering.UnicodeBiDiTypes.BiDiOverride:
				return "bidi-override";
			default:
				return null;
			}
		}

		public static string GetValue(Microsoft.ReportingServices.OnDemandReportRendering.BorderStyles val)
		{
			switch (val)
			{
			case Microsoft.ReportingServices.OnDemandReportRendering.BorderStyles.None:
				return "none";
			case Microsoft.ReportingServices.OnDemandReportRendering.BorderStyles.Dotted:
				return "dotted";
			case Microsoft.ReportingServices.OnDemandReportRendering.BorderStyles.Dashed:
				return "dashed";
			case Microsoft.ReportingServices.OnDemandReportRendering.BorderStyles.Solid:
				return "solid";
			case Microsoft.ReportingServices.OnDemandReportRendering.BorderStyles.Double:
				return "double";
			default:
				return null;
			}
		}

		public static string GetValue(Microsoft.ReportingServices.OnDemandReportRendering.BackgroundRepeatTypes val)
		{
			switch (val)
			{
			case Microsoft.ReportingServices.OnDemandReportRendering.BackgroundRepeatTypes.Repeat:
				return "repeat";
			case Microsoft.ReportingServices.OnDemandReportRendering.BackgroundRepeatTypes.Clip:
				return "no-repeat";
			case Microsoft.ReportingServices.OnDemandReportRendering.BackgroundRepeatTypes.RepeatX:
				return "repeat-x";
			case Microsoft.ReportingServices.OnDemandReportRendering.BackgroundRepeatTypes.RepeatY:
				return "repeat-y";
			default:
				return null;
			}
		}
	}
}

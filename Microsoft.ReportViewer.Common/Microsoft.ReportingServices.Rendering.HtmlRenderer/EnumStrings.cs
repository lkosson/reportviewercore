using Microsoft.ReportingServices.Rendering.RPLProcessing;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class EnumStrings
	{
		public class FontStyles
		{
			public const string Normal = "normal";

			public const string Italic = "italic";
		}

		public class FontWeights
		{
			public const string Normal = "400";

			public const string Thin = "100";

			public const string ExtraLight = "200";

			public const string Light = "300";

			public const string Medium = "500";

			public const string SemiBold = "600";

			public const string Bold = "700";

			public const string ExtraBold = "800";

			public const string Heavy = "900";
		}

		public class TextDecorations
		{
			public const string None = "none";

			public const string Underline = "underline";

			public const string Overline = "overline";

			public const string LineThrough = "line-through";
		}

		public class TextAlignments
		{
			public const string General = "General";

			public const string Left = "left";

			public const string Center = "center";

			public const string Right = "right";
		}

		public class VerticalAlignments
		{
			public const string Top = "top";

			public const string Middle = "middle";

			public const string Bottom = "bottom";
		}

		public class Directions
		{
			public const string LTR = "ltr";

			public const string RTL = "rtl";
		}

		public class WritingModes
		{
			public const string Horizontal = "lr-tb";

			public const string Vertical = "tb-rl";
		}

		public class UnicodeBiDiTypes
		{
			public const string Normal = "normal";

			public const string Embed = "embed";

			public const string BiDiOverride = "bidi-override";
		}

		public class BorderStyles
		{
			public const string None = "none";

			public const string Dotted = "dotted";

			public const string Dashed = "dashed";

			public const string Solid = "solid";

			public const string Double = "double";
		}

		public class BackgroundRepeatTypes
		{
			public const string Repeat = "repeat";

			public const string NoRepeat = "no-repeat";

			public const string RepeatX = "repeat-x";

			public const string RepeatY = "repeat-y";
		}

		public static string GetValue(RPLFormat.FontStyles val)
		{
			switch (val)
			{
			case RPLFormat.FontStyles.Normal:
				return "normal";
			case RPLFormat.FontStyles.Italic:
				return "italic";
			default:
				return null;
			}
		}

		public static string GetValue(RPLFormat.FontWeights val)
		{
			switch (val)
			{
			case RPLFormat.FontWeights.Normal:
				return "400";
			case RPLFormat.FontWeights.Thin:
				return "100";
			case RPLFormat.FontWeights.ExtraLight:
				return "200";
			case RPLFormat.FontWeights.Light:
				return "300";
			case RPLFormat.FontWeights.Medium:
				return "500";
			case RPLFormat.FontWeights.SemiBold:
				return "600";
			case RPLFormat.FontWeights.Bold:
				return "700";
			case RPLFormat.FontWeights.ExtraBold:
				return "800";
			case RPLFormat.FontWeights.Heavy:
				return "900";
			default:
				return null;
			}
		}

		public static string GetValue(RPLFormat.TextDecorations val)
		{
			switch (val)
			{
			case RPLFormat.TextDecorations.None:
				return "none";
			case RPLFormat.TextDecorations.Underline:
				return "underline";
			case RPLFormat.TextDecorations.Overline:
				return "overline";
			case RPLFormat.TextDecorations.LineThrough:
				return "line-through";
			default:
				return null;
			}
		}

		public static string GetValue(RPLFormat.TextAlignments val)
		{
			switch (val)
			{
			case RPLFormat.TextAlignments.Left:
				return "left";
			case RPLFormat.TextAlignments.Center:
				return "center";
			case RPLFormat.TextAlignments.Right:
				return "right";
			default:
				return null;
			}
		}

		public static string GetValue(RPLFormat.VerticalAlignments val)
		{
			switch (val)
			{
			case RPLFormat.VerticalAlignments.Top:
				return "top";
			case RPLFormat.VerticalAlignments.Middle:
				return "middle";
			case RPLFormat.VerticalAlignments.Bottom:
				return "bottom";
			default:
				return null;
			}
		}

		public static string GetValue(RPLFormat.Directions val)
		{
			switch (val)
			{
			case RPLFormat.Directions.LTR:
				return "ltr";
			case RPLFormat.Directions.RTL:
				return "rtl";
			default:
				return null;
			}
		}

		public static string GetValue(RPLFormat.UnicodeBiDiTypes val)
		{
			switch (val)
			{
			case RPLFormat.UnicodeBiDiTypes.Normal:
				return "normal";
			case RPLFormat.UnicodeBiDiTypes.Embed:
				return "embed";
			case RPLFormat.UnicodeBiDiTypes.BiDiOverride:
				return "bidi-override";
			default:
				return null;
			}
		}

		public static string GetValue(RPLFormat.BorderStyles val)
		{
			switch (val)
			{
			case RPLFormat.BorderStyles.None:
				return "none";
			case RPLFormat.BorderStyles.Dotted:
				return "dotted";
			case RPLFormat.BorderStyles.Dashed:
				return "dashed";
			case RPLFormat.BorderStyles.Solid:
				return "solid";
			case RPLFormat.BorderStyles.Double:
				return "double";
			default:
				return null;
			}
		}

		public static string GetValue(RPLFormat.BackgroundRepeatTypes val)
		{
			switch (val)
			{
			case RPLFormat.BackgroundRepeatTypes.Repeat:
				return "repeat";
			case RPLFormat.BackgroundRepeatTypes.Clip:
				return "no-repeat";
			case RPLFormat.BackgroundRepeatTypes.RepeatX:
				return "repeat-x";
			case RPLFormat.BackgroundRepeatTypes.RepeatY:
				return "repeat-y";
			default:
				return null;
			}
		}
	}
}

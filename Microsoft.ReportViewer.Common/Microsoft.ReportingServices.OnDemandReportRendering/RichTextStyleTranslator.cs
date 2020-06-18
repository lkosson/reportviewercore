using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class RichTextStyleTranslator
	{
		internal class StyleEnumConstants
		{
			internal const string Default = "Default";

			internal const string Normal = "Normal";

			internal const string General = "General";

			internal const string Center = "Center";

			internal const string Left = "Left";

			internal const string Right = "Right";

			internal const string Thin = "Thin";

			internal const string ExtraLight = "ExtraLight";

			internal const string Light = "Light";

			internal const string Lighter = "Lighter";

			internal const string Medium = "Medium";

			internal const string SemiBold = "SemiBold";

			internal const string Bold = "Bold";

			internal const string Bolder = "Bolder";

			internal const string ExtraBold = "ExtraBold";

			internal const string Heavy = "Heavy";

			internal const string FontWeight100 = "100";

			internal const string FontWeight200 = "200";

			internal const string FontWeight300 = "300";

			internal const string FontWeight400 = "400";

			internal const string FontWeight500 = "500";

			internal const string FontWeight600 = "600";

			internal const string FontWeight700 = "700";

			internal const string FontWeight800 = "800";

			internal const string FontWeight900 = "900";
		}

		internal static bool CompareWithInvariantCulture(string str1, string str2)
		{
			return string.Compare(str1, str2, StringComparison.OrdinalIgnoreCase) == 0;
		}

		internal static bool TranslateHtmlFontSize(string value, out string translatedSize)
		{
			if (int.TryParse(value, out int result))
			{
				if (result <= 0)
				{
					translatedSize = "7.5pt";
				}
				else
				{
					switch (result)
					{
					case 1:
						translatedSize = "7.5pt";
						break;
					case 2:
						translatedSize = "10pt";
						break;
					case 3:
						translatedSize = "11pt";
						break;
					case 4:
						translatedSize = "13.5pt";
						break;
					case 5:
						translatedSize = "18pt";
						break;
					case 6:
						translatedSize = "24pt";
						break;
					default:
						translatedSize = "36pt";
						break;
					}
				}
				return true;
			}
			translatedSize = null;
			return false;
		}

		internal static string TranslateHtmlColor(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				if (value[0] == '#')
				{
					return value;
				}
				if (char.IsDigit(value[0]))
				{
					return "#" + value;
				}
			}
			return value;
		}

		internal static bool TranslateFontWeight(string styleString, out FontWeights fontWieght)
		{
			fontWieght = FontWeights.Normal;
			if (!string.IsNullOrEmpty(styleString))
			{
				if (CompareWithInvariantCulture("Normal", styleString))
				{
					fontWieght = FontWeights.Normal;
				}
				else if (CompareWithInvariantCulture("Bold", styleString))
				{
					fontWieght = FontWeights.Bold;
				}
				else if (CompareWithInvariantCulture("Bolder", styleString))
				{
					fontWieght = FontWeights.Bold;
				}
				else if (CompareWithInvariantCulture("100", styleString))
				{
					fontWieght = FontWeights.Thin;
				}
				else if (CompareWithInvariantCulture("200", styleString))
				{
					fontWieght = FontWeights.ExtraLight;
				}
				else if (CompareWithInvariantCulture("300", styleString))
				{
					fontWieght = FontWeights.Light;
				}
				else if (CompareWithInvariantCulture("400", styleString))
				{
					fontWieght = FontWeights.Normal;
				}
				else if (CompareWithInvariantCulture("500", styleString))
				{
					fontWieght = FontWeights.Medium;
				}
				else if (CompareWithInvariantCulture("600", styleString))
				{
					fontWieght = FontWeights.SemiBold;
				}
				else if (CompareWithInvariantCulture("700", styleString))
				{
					fontWieght = FontWeights.Bold;
				}
				else if (CompareWithInvariantCulture("800", styleString))
				{
					fontWieght = FontWeights.ExtraBold;
				}
				else if (CompareWithInvariantCulture("900", styleString))
				{
					fontWieght = FontWeights.Heavy;
				}
				else if (CompareWithInvariantCulture("Thin", styleString))
				{
					fontWieght = FontWeights.Thin;
				}
				else if (CompareWithInvariantCulture("ExtraLight", styleString))
				{
					fontWieght = FontWeights.ExtraLight;
				}
				else if (CompareWithInvariantCulture("Light", styleString))
				{
					fontWieght = FontWeights.Light;
				}
				else if (CompareWithInvariantCulture("Lighter", styleString))
				{
					fontWieght = FontWeights.Light;
				}
				else if (CompareWithInvariantCulture("Medium", styleString))
				{
					fontWieght = FontWeights.Medium;
				}
				else if (CompareWithInvariantCulture("SemiBold", styleString))
				{
					fontWieght = FontWeights.SemiBold;
				}
				else if (CompareWithInvariantCulture("ExtraBold", styleString))
				{
					fontWieght = FontWeights.ExtraBold;
				}
				else if (CompareWithInvariantCulture("Heavy", styleString))
				{
					fontWieght = FontWeights.Heavy;
				}
				else
				{
					if (!CompareWithInvariantCulture("Default", styleString))
					{
						return false;
					}
					fontWieght = FontWeights.Normal;
				}
				return true;
			}
			return false;
		}

		internal static bool TranslateTextAlign(string styleString, out TextAlignments textAlignment)
		{
			textAlignment = TextAlignments.General;
			if (!string.IsNullOrEmpty(styleString))
			{
				if (CompareWithInvariantCulture("General", styleString))
				{
					textAlignment = TextAlignments.General;
				}
				else if (CompareWithInvariantCulture("Left", styleString))
				{
					textAlignment = TextAlignments.Left;
				}
				else if (CompareWithInvariantCulture("Center", styleString))
				{
					textAlignment = TextAlignments.Center;
				}
				else if (CompareWithInvariantCulture("Right", styleString))
				{
					textAlignment = TextAlignments.Right;
				}
				else
				{
					if (!CompareWithInvariantCulture("Default", styleString))
					{
						return false;
					}
					textAlignment = TextAlignments.General;
				}
				return true;
			}
			return false;
		}
	}
}

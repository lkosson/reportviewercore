using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.OnDemandReportRendering;
using System;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class Validator
	{
		internal static int DecimalPrecision = 5;

		internal static double NormalMin = 0.0;

		internal static double NegativeMin = 0.0 - Converter.Inches160;

		internal static double NormalMax = Converter.Inches160;

		internal static double BorderWidthMin = Converter.PtPoint25;

		internal static double BorderWidthMax = Converter.Pt20;

		internal static double FontSizeMin = Converter.Pt1;

		internal static double FontSizeMax = Converter.Pt200;

		internal static double PaddingMin = 0.0;

		internal static double PaddingMax = Converter.Pt1000;

		internal static double LineHeightMin = Converter.Pt1;

		internal static double LineHeightMax = Converter.Pt1000;

		private static Regex m_colorRegex = new Regex("^#(\\d|a|b|c|d|e|f){6}$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);

		private static Regex m_colorRegexTransparency = new Regex("^#(\\d|a|b|c|d|e|f){8}$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);

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

		internal static bool ValidateColor(string color, out Color c)
		{
			return ValidateColor(color, out c, allowTransparency: false);
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

		internal static void ParseColor(string color, out Color c)
		{
			ParseColor(color, out c, allowTransparency: false);
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
			catch
			{
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

		internal static bool ValidateSizeString(string sizeString, out RVUnit sizeValue)
		{
			try
			{
				sizeValue = RVUnit.Parse(sizeString, CultureInfo.InvariantCulture);
				if (sizeValue.Type == RVUnitType.Pixel)
				{
					return false;
				}
				return true;
			}
			catch
			{
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

		internal static bool ValidateEmbeddedImageName(string embeddedImageName, EmbeddedImageHashtable embeddedImages)
		{
			if (embeddedImageName == null)
			{
				return false;
			}
			return embeddedImages?.ContainsKey(embeddedImageName) ?? false;
		}

		internal static bool ValidateSpecificLanguage(string language, out CultureInfo culture)
		{
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

		internal static bool CreateCalendar(string calendarName, out Calendar calendar)
		{
			calendar = null;
			bool result = false;
			if (CompareWithInvariantCulture(calendarName, "Gregorian Arabic"))
			{
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.Arabic);
			}
			else if (CompareWithInvariantCulture(calendarName, "Gregorian Middle East French"))
			{
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.MiddleEastFrench);
			}
			else if (CompareWithInvariantCulture(calendarName, "Gregorian Transliterated English"))
			{
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.TransliteratedEnglish);
			}
			else if (CompareWithInvariantCulture(calendarName, "Gregorian Transliterated French"))
			{
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.TransliteratedFrench);
			}
			else if (CompareWithInvariantCulture(calendarName, "Gregorian US English"))
			{
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
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
			else if (CompareWithInvariantCulture(calendarName, "Korea"))
			{
				calendar = new KoreanCalendar();
			}
			else if (CompareWithInvariantCulture(calendarName, "Taiwan"))
			{
				calendar = new TaiwanCalendar();
			}
			else if (CompareWithInvariantCulture(calendarName, "Thai Buddhist"))
			{
				calendar = new ThaiBuddhistCalendar();
			}
			else if (CompareWithInvariantCulture(calendarName, "Gregorian"))
			{
				calendar = new GregorianCalendar();
			}
			return result;
		}

		internal static bool CreateCalendar(Calendars calendarType, out Calendar calendar)
		{
			calendar = null;
			bool result = false;
			switch (calendarType)
			{
			case Calendars.Default:
			case Calendars.Gregorian:
				calendar = new GregorianCalendar();
				break;
			case Calendars.GregorianArabic:
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.Arabic);
				break;
			case Calendars.GregorianMiddleEastFrench:
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.MiddleEastFrench);
				break;
			case Calendars.GregorianTransliteratedEnglish:
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.TransliteratedEnglish);
				break;
			case Calendars.GregorianTransliteratedFrench:
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.TransliteratedFrench);
				break;
			case Calendars.GregorianUSEnglish:
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
				break;
			case Calendars.Hebrew:
				calendar = new HebrewCalendar();
				break;
			case Calendars.Hijri:
				calendar = new HijriCalendar();
				break;
			case Calendars.Japanese:
				calendar = new JapaneseCalendar();
				break;
			case Calendars.Julian:
				calendar = new JulianCalendar();
				break;
			case Calendars.Korean:
				calendar = new KoreanCalendar();
				break;
			case Calendars.Taiwan:
				calendar = new TaiwanCalendar();
				break;
			case Calendars.ThaiBuddhist:
				calendar = new ThaiBuddhistCalendar();
				break;
			}
			return result;
		}

		internal static bool ValidateCalendar(CultureInfo langauge, Calendars calendarType)
		{
			if (calendarType == Calendars.Gregorian)
			{
				return true;
			}
			Calendar calendar;
			bool isGregorianSubType = CreateCalendar(calendarType, out calendar);
			return ValidateCalendar(langauge, isGregorianSubType, calendar);
		}

		internal static bool ValidateCalendar(CultureInfo langauge, string calendarName)
		{
			if (CompareWithInvariantCulture(calendarName, "Gregorian"))
			{
				return true;
			}
			Calendar calendar;
			bool isGregorianSubType = CreateCalendar(calendarName, out calendar);
			return ValidateCalendar(langauge, isGregorianSubType, calendar);
		}

		private static bool ValidateCalendar(CultureInfo langauge, bool isGregorianSubType, Calendar calendar)
		{
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
						if (!isGregorianSubType)
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

		internal static bool ValidateBorderStyle(string borderStyle, out string borderStyleForLine)
		{
			if (CompareWithInvariantCulture(borderStyle, "Dotted") || CompareWithInvariantCulture(borderStyle, "Dashed"))
			{
				borderStyleForLine = borderStyle;
				return true;
			}
			if (CompareWithInvariantCulture(borderStyle, "None") || CompareWithInvariantCulture(borderStyle, "Solid") || CompareWithInvariantCulture(borderStyle, "Double") || CompareWithInvariantCulture(borderStyle, "Groove") || CompareWithInvariantCulture(borderStyle, "Ridge") || CompareWithInvariantCulture(borderStyle, "Inset") || CompareWithInvariantCulture(borderStyle, "WindowInset") || CompareWithInvariantCulture(borderStyle, "Outset"))
			{
				borderStyleForLine = "Solid";
				return true;
			}
			borderStyleForLine = null;
			return false;
		}

		internal static bool ValidateMimeType(string mimeType)
		{
			if (CompareWithInvariantCulture(mimeType, "image/bmp") || CompareWithInvariantCulture(mimeType, "image/jpeg") || CompareWithInvariantCulture(mimeType, "image/gif") || CompareWithInvariantCulture(mimeType, "image/png") || CompareWithInvariantCulture(mimeType, "image/x-png"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateBackgroundGradientType(string gradientType)
		{
			if (CompareWithInvariantCulture(gradientType, "None") || CompareWithInvariantCulture(gradientType, "LeftRight") || CompareWithInvariantCulture(gradientType, "TopBottom") || CompareWithInvariantCulture(gradientType, "Center") || CompareWithInvariantCulture(gradientType, "DiagonalLeft") || CompareWithInvariantCulture(gradientType, "DiagonalRight") || CompareWithInvariantCulture(gradientType, "HorizontalCenter") || CompareWithInvariantCulture(gradientType, "VerticalCenter"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateBackgroundRepeat(string repeat)
		{
			if (CompareWithInvariantCulture(repeat, "Repeat") || CompareWithInvariantCulture(repeat, "NoRepeat") || CompareWithInvariantCulture(repeat, "RepeatX") || CompareWithInvariantCulture(repeat, "RepeatY"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateFontStyle(string fontStyle)
		{
			if (CompareWithInvariantCulture(fontStyle, "Normal") || CompareWithInvariantCulture(fontStyle, "Italic"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateFontWeight(string fontWeight)
		{
			if (CompareWithInvariantCulture(fontWeight, "Lighter") || CompareWithInvariantCulture(fontWeight, "Normal") || CompareWithInvariantCulture(fontWeight, "Bold") || CompareWithInvariantCulture(fontWeight, "Bolder") || CompareWithInvariantCulture(fontWeight, "100") || CompareWithInvariantCulture(fontWeight, "200") || CompareWithInvariantCulture(fontWeight, "300") || CompareWithInvariantCulture(fontWeight, "400") || CompareWithInvariantCulture(fontWeight, "500") || CompareWithInvariantCulture(fontWeight, "600") || CompareWithInvariantCulture(fontWeight, "700") || CompareWithInvariantCulture(fontWeight, "800") || CompareWithInvariantCulture(fontWeight, "900"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateTextDecoration(string textDecoration)
		{
			if (CompareWithInvariantCulture(textDecoration, "None") || CompareWithInvariantCulture(textDecoration, "Underline") || CompareWithInvariantCulture(textDecoration, "Overline") || CompareWithInvariantCulture(textDecoration, "LineThrough"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateTextAlign(string textAlign)
		{
			if (CompareWithInvariantCulture(textAlign, "General") || CompareWithInvariantCulture(textAlign, "Left") || CompareWithInvariantCulture(textAlign, "Center") || CompareWithInvariantCulture(textAlign, "Right"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateVerticalAlign(string verticalAlign)
		{
			if (CompareWithInvariantCulture(verticalAlign, "Top") || CompareWithInvariantCulture(verticalAlign, "Middle") || CompareWithInvariantCulture(verticalAlign, "Bottom"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateDirection(string direction)
		{
			if (CompareWithInvariantCulture(direction, "LTR") || CompareWithInvariantCulture(direction, "RTL"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateWritingMode(string writingMode)
		{
			if (CompareWithInvariantCulture(writingMode, "lr-tb") || CompareWithInvariantCulture(writingMode, "tb-rl"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateUnicodeBiDi(string unicodeBiDi)
		{
			if (CompareWithInvariantCulture(unicodeBiDi, "Normal") || CompareWithInvariantCulture(unicodeBiDi, "Embed") || CompareWithInvariantCulture(unicodeBiDi, "BiDi-Override"))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateCalendar(string calendar)
		{
			if (CompareWithInvariantCulture(calendar, "Gregorian") || CompareWithInvariantCulture(calendar, "Gregorian Arabic") || CompareWithInvariantCulture(calendar, "Gregorian Middle East French") || CompareWithInvariantCulture(calendar, "Gregorian Transliterated English") || CompareWithInvariantCulture(calendar, "Gregorian Transliterated French") || CompareWithInvariantCulture(calendar, "Gregorian US English") || CompareWithInvariantCulture(calendar, "Hebrew") || CompareWithInvariantCulture(calendar, "Hijri") || CompareWithInvariantCulture(calendar, "Japanese") || CompareWithInvariantCulture(calendar, "Korea") || CompareWithInvariantCulture(calendar, "Taiwan") || CompareWithInvariantCulture(calendar, "Thai Buddhist"))
			{
				return true;
			}
			return false;
		}

		internal static bool CompareWithInvariantCulture(string strOne, string strTwo)
		{
			if (ReportProcessing.CompareWithInvariantCulture(strOne, strTwo, ignoreCase: false) == 0)
			{
				return true;
			}
			return false;
		}
	}
}

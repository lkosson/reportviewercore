using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Drawing;
using System.Globalization;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Layout
{
	internal static class LayoutConvert
	{
		private const double MM_PER_INCH = 25.4;

		private const int PTS_PER_INCH = 72;

		private const double ROUNDUP_DELTA = 0.00051;

		private const int OnePixelInTwips = 15;

		private const int TwoPixelsInTwips = 30;

		private const int ThreePixelsInTwips = 45;

		internal static double ToMillimeters(string val)
		{
			return new ReportSize(val).ToMillimeters();
		}

		internal static double ToInches(string val)
		{
			return new ReportSize(val).ToInches();
		}

		internal static double ToPoints(string val)
		{
			return new ReportSize(val).ToPoints();
		}

		internal static Color ToColor(string val)
		{
			return new ReportColor(val).ToColor();
		}

		internal static ExcelBorderStyle ToBorderLineStyle(ExcelBorderStyle excelBorderStyle, double borderWidth)
		{
			switch (excelBorderStyle)
			{
			case ExcelBorderStyle.Dashed:
			case ExcelBorderStyle.MedDashed:
				if (borderWidth <= 1.0)
				{
					return ExcelBorderStyle.Dashed;
				}
				return ExcelBorderStyle.MedDashed;
			case ExcelBorderStyle.Dotted:
				return ExcelBorderStyle.Dotted;
			case ExcelBorderStyle.Double:
				return ExcelBorderStyle.Double;
			case ExcelBorderStyle.None:
				return ExcelBorderStyle.None;
			default:
				if (borderWidth <= 1.0)
				{
					return ExcelBorderStyle.Thin;
				}
				if (borderWidth <= 1.5)
				{
					return ExcelBorderStyle.Medium;
				}
				return ExcelBorderStyle.Thick;
			}
		}

		internal static ExcelBorderStyle ToBorderLineStyle(RPLFormat.BorderStyles val)
		{
			switch (val)
			{
			case RPLFormat.BorderStyles.Dotted:
				return ExcelBorderStyle.Dotted;
			case RPLFormat.BorderStyles.Dashed:
				return ExcelBorderStyle.Dashed;
			case RPLFormat.BorderStyles.Solid:
				return ExcelBorderStyle.Medium;
			case RPLFormat.BorderStyles.Double:
				return ExcelBorderStyle.Double;
			default:
				return ExcelBorderStyle.None;
			}
		}

		internal static int GetBorderWidth(ExcelBorderStyle borderStyle, double borderWidthInPts, bool rightOrBottom)
		{
			if (borderStyle != 0)
			{
				if (borderWidthInPts <= 1.0)
				{
					if (rightOrBottom)
					{
						return 0;
					}
					return 15;
				}
				if (borderWidthInPts <= 1.5)
				{
					if (rightOrBottom)
					{
						return 15;
					}
					return 30;
				}
				if (rightOrBottom)
				{
					return 30;
				}
				return 45;
			}
			return 0;
		}

		internal static int ToFontWeight(RPLFormat.FontWeights val)
		{
			switch (val)
			{
			case RPLFormat.FontWeights.Thin:
				return 100;
			case RPLFormat.FontWeights.ExtraLight:
				return 200;
			case RPLFormat.FontWeights.Light:
				return 300;
			case RPLFormat.FontWeights.Normal:
				return 400;
			case RPLFormat.FontWeights.Medium:
				return 500;
			case RPLFormat.FontWeights.SemiBold:
				return 600;
			case RPLFormat.FontWeights.Bold:
				return 700;
			case RPLFormat.FontWeights.ExtraBold:
				return 800;
			case RPLFormat.FontWeights.Heavy:
				return 900;
			default:
				return 400;
			}
		}

		internal static HorizontalAlignment ToHorizontalAlignEnum(RPLFormat.TextAlignments val)
		{
			switch (val)
			{
			case RPLFormat.TextAlignments.Left:
				return HorizontalAlignment.Left;
			case RPLFormat.TextAlignments.Center:
				return HorizontalAlignment.Center;
			case RPLFormat.TextAlignments.Right:
				return HorizontalAlignment.Right;
			default:
				return HorizontalAlignment.General;
			}
		}

		internal static HorizontalAlignment RotateVerticalToHorizontalAlign(VerticalAlignment val, bool isClockwise)
		{
			switch (val)
			{
			case VerticalAlignment.Bottom:
				if (!isClockwise)
				{
					return HorizontalAlignment.Right;
				}
				return HorizontalAlignment.Left;
			case VerticalAlignment.Center:
				return HorizontalAlignment.Center;
			case VerticalAlignment.Top:
				if (!isClockwise)
				{
					return HorizontalAlignment.Left;
				}
				return HorizontalAlignment.Right;
			case VerticalAlignment.Distributed:
				return HorizontalAlignment.Distributed;
			case VerticalAlignment.Justify:
				return HorizontalAlignment.Justify;
			default:
				return HorizontalAlignment.General;
			}
		}

		internal static VerticalAlignment RotateHorizontalToVerticalAlign(HorizontalAlignment val, bool isClockwise)
		{
			switch (val)
			{
			case HorizontalAlignment.Left:
				if (!isClockwise)
				{
					return VerticalAlignment.Bottom;
				}
				return VerticalAlignment.Top;
			case HorizontalAlignment.Right:
				if (!isClockwise)
				{
					return VerticalAlignment.Top;
				}
				return VerticalAlignment.Bottom;
			case HorizontalAlignment.Justify:
				return VerticalAlignment.Justify;
			case HorizontalAlignment.Distributed:
				return VerticalAlignment.Distributed;
			default:
				return VerticalAlignment.Center;
			}
		}

		internal static VerticalAlignment ToVerticalAlignEnum(RPLFormat.VerticalAlignments val)
		{
			switch (val)
			{
			case RPLFormat.VerticalAlignments.Top:
				return VerticalAlignment.Top;
			case RPLFormat.VerticalAlignments.Bottom:
				return VerticalAlignment.Bottom;
			default:
				return VerticalAlignment.Center;
			}
		}

		internal static Calendar GetCalendarInstance(RPLFormat.Calendars val)
		{
			switch (val)
			{
			case RPLFormat.Calendars.GregorianArabic:
				return new GregorianCalendar(GregorianCalendarTypes.Arabic);
			case RPLFormat.Calendars.GregorianMiddleEastFrench:
				return new GregorianCalendar(GregorianCalendarTypes.MiddleEastFrench);
			case RPLFormat.Calendars.GregorianTransliteratedEnglish:
				return new GregorianCalendar(GregorianCalendarTypes.TransliteratedEnglish);
			case RPLFormat.Calendars.GregorianTransliteratedFrench:
				return new GregorianCalendar(GregorianCalendarTypes.TransliteratedFrench);
			case RPLFormat.Calendars.GregorianUSEnglish:
				return new GregorianCalendar(GregorianCalendarTypes.USEnglish);
			case RPLFormat.Calendars.Hebrew:
				return new HebrewCalendar();
			case RPLFormat.Calendars.Hijri:
				return new HijriCalendar();
			case RPLFormat.Calendars.Japanese:
				return new JapaneseCalendar();
			case RPLFormat.Calendars.Korean:
				return new KoreanCalendar();
			case RPLFormat.Calendars.Taiwan:
				return new TaiwanCalendar();
			case RPLFormat.Calendars.ThaiBuddhist:
				return new ThaiBuddhistCalendar();
			default:
				return new GregorianCalendar();
			}
		}

		internal static int ConvertMMTo20thPoints(double aMm)
		{
			return (int)Math.Round(ConvertMMToPoints(aMm) * 20.0 + 0.00051, 3);
		}

		internal static double ConvertMMTo20thPointsUnrounded(double aMm)
		{
			return ConvertMMToPoints(aMm) * 20.0;
		}

		internal static double ConvertMMToInches(double aMm)
		{
			return aMm / 25.4;
		}

		internal static double ConvertFloatToDouble(float floatValue)
		{
			return Convert.ToDouble((decimal)floatValue);
		}

		internal static double ConvertMMToPoints(double aMm)
		{
			return aMm / 25.4 * 72.0;
		}

		internal static double ConvertPointsToMM(double aPoints)
		{
			return aPoints / 72.0 * 25.4;
		}

		internal static bool ParseBool(string boolValue, bool defaultValue)
		{
			bool result = defaultValue;
			if (boolValue != null && boolValue.Length > 0)
			{
				try
				{
					result = bool.Parse(boolValue);
					return result;
				}
				catch (FormatException)
				{
					return result;
				}
			}
			return result;
		}

		internal static double ParseDouble(string doubleValue, double defaultValue)
		{
			double result = defaultValue;
			if (doubleValue != null && doubleValue.Length > 0)
			{
				try
				{
					result = double.Parse(doubleValue, CultureInfo.InvariantCulture);
					return result;
				}
				catch (FormatException)
				{
					return result;
				}
			}
			return result;
		}
	}
}

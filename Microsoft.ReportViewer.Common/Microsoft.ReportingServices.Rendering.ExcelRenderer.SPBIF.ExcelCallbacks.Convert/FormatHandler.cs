using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Layout;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Globalization;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert
{
	internal static class FormatHandler
	{
		internal const string DEFAULTFORMATFORDATETIME = "G";

		internal const string DEFAULTFORMATFORNUMBER = "General";

		private static string GetShortNumberFormat(char type)
		{
			switch (type)
			{
			case '#':
				return "#";
			case '0':
				return "0";
			case 'D':
			case 'd':
				return "#0";
			case 'E':
			case 'e':
				return "0.000000E+000";
			case 'F':
			case 'f':
				return "0.00";
			case 'P':
			case 'p':
				return "#,##0.00%";
			case 'R':
			case 'r':
				return "General";
			default:
				return string.Empty;
			}
		}

		private static string GetLongNumberFormat(char type, int precision)
		{
			string text = "";
			bool flag = precision > 0;
			while (precision > 0)
			{
				text += "0";
				precision--;
			}
			switch (type)
			{
			case 'D':
			case 'd':
				if (!flag)
				{
					return "0";
				}
				return text;
			case 'E':
			case 'e':
				if (!flag)
				{
					return "0E+000";
				}
				return "0." + text + "E+000";
			case 'F':
			case 'f':
				if (!flag)
				{
					return "0";
				}
				return "0." + text;
			case 'P':
			case 'p':
				if (!flag)
				{
					return "#,##0%";
				}
				return "#,##0." + text + "#%";
			case 'R':
			case 'r':
				return "General";
			default:
				return string.Empty;
			}
		}

		private static string GetExcelPictureNumberFormat(string format)
		{
			string text = format.ToUpperInvariant();
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < format.Length; i++)
			{
				switch (format[i])
				{
				case '"':
				case '\'':
				{
					char c = format[i];
					stringBuilder.Append('"');
					i++;
					bool flag = false;
					for (; i < format.Length; i++)
					{
						if (format[i] != c)
						{
							stringBuilder.Append(format[i]);
							continue;
						}
						flag = true;
						break;
					}
					if (flag)
					{
						stringBuilder.Append('"');
					}
					break;
				}
				case 'E':
				case 'e':
					stringBuilder.Append(format[i]);
					if (i + 1 < format.Length && format[i + 1] == '0')
					{
						stringBuilder.Append('-');
					}
					break;
				case '\\':
					if (i + 1 < format.Length && format[i + 1] == '\'')
					{
						stringBuilder.Append("\\'");
						i++;
					}
					else if (i + 1 < format.Length && format[i + 1] == '"')
					{
						stringBuilder.Append("\\005c\"");
						i++;
					}
					else
					{
						stringBuilder.Append('"');
						stringBuilder.Append(format[i]);
						stringBuilder.Append('"');
					}
					break;
				case '*':
				case '_':
					stringBuilder.Append('"');
					stringBuilder.Append(format[i]);
					stringBuilder.Append('"');
					break;
				default:
					if (NeedNumberEscape(text[i]))
					{
						stringBuilder.Append('"');
						stringBuilder.Append(format[i]);
						stringBuilder.Append('"');
					}
					else
					{
						stringBuilder.Append(format[i]);
					}
					break;
				}
			}
			return stringBuilder.ToString();
		}

		private static bool NeedNumberEscape(char c)
		{
			bool result = false;
			if (c == '|' || c == '?' || c == '@' || c == 'B' || c == 'D' || c == 'G' || c == 'H' || c == 'M' || c == 'S' || c == 'Y' || c == 'N')
			{
				result = true;
			}
			return result;
		}

		private static string GetNumberFormat(NumberFormatInfo numberFormatInfo, int precision)
		{
			string numberDecimalSeparator = NumberFormatInfo.InvariantInfo.NumberDecimalSeparator;
			string numberGroupSeparator = NumberFormatInfo.InvariantInfo.NumberGroupSeparator;
			int[] numberGroupSizes = numberFormatInfo.NumberGroupSizes;
			int numberNegativePattern = numberFormatInfo.NumberNegativePattern;
			_ = numberFormatInfo.NegativeSign;
			int num = 3;
			if (numberGroupSizes.Length != 0)
			{
				num = numberGroupSizes[0];
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("#");
			stringBuilder.Append(numberGroupSeparator);
			for (int i = 0; i < num - 1; i++)
			{
				stringBuilder.Append("#");
			}
			stringBuilder.Append('0');
			if (precision > 0)
			{
				stringBuilder.Append(numberDecimalSeparator);
			}
			for (int j = 0; j < precision; j++)
			{
				stringBuilder.Append('0');
			}
			StringBuilder stringBuilder2 = new StringBuilder(stringBuilder.ToString());
			stringBuilder2.Append(";");
			stringBuilder2.Append(string.Format(CultureInfo.InvariantCulture, GetNegativePattern(numberNegativePattern), new object[2]
			{
				stringBuilder.ToString(),
				numberFormatInfo.NegativeSign
			}));
			return stringBuilder2.ToString();
		}

		private static string GetNegativePattern(int negativePattern)
		{
			switch (negativePattern)
			{
			case 0:
				return "({0})";
			case 1:
				return "{1}{0}";
			case 2:
				return "{1} {0}";
			case 3:
				return "{0}{1}";
			case 4:
				return "{0} {1}";
			default:
				return string.Empty;
			}
		}

		private static string GetCurrencyFormat(NumberFormatInfo numberFormatInfo, int precision)
		{
			string currencyDecimalSeparator = NumberFormatInfo.InvariantInfo.CurrencyDecimalSeparator;
			string currencyGroupSeparator = NumberFormatInfo.InvariantInfo.CurrencyGroupSeparator;
			int[] currencyGroupSizes = numberFormatInfo.CurrencyGroupSizes;
			int currencyNegativePattern = numberFormatInfo.CurrencyNegativePattern;
			int currencyPositivePattern = numberFormatInfo.CurrencyPositivePattern;
			string text = "\"" + numberFormatInfo.CurrencySymbol + "\"";
			string text2 = "\\" + numberFormatInfo.NegativeSign;
			int num = 3;
			if (currencyGroupSizes.Length != 0)
			{
				num = currencyGroupSizes[0];
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("#");
			stringBuilder.Append(currencyGroupSeparator);
			for (int i = 0; i < num - 1; i++)
			{
				stringBuilder.Append("#");
			}
			stringBuilder.Append('0');
			if (0 < precision)
			{
				stringBuilder.Append(currencyDecimalSeparator);
			}
			for (int j = 0; j < precision; j++)
			{
				stringBuilder.Append('0');
			}
			StringBuilder stringBuilder2 = new StringBuilder(string.Format(CultureInfo.InvariantCulture, GetPositiveCurrencyPattern(currencyPositivePattern), new object[2]
			{
				stringBuilder.ToString(),
				text
			}));
			stringBuilder2.Append(";");
			stringBuilder2.Append(string.Format(CultureInfo.InvariantCulture, GetNegativeCurrencyPattern(currencyNegativePattern), new object[3]
			{
				stringBuilder.ToString(),
				text,
				text2
			}));
			return stringBuilder2.ToString();
		}

		private static string GetPositiveCurrencyPattern(int currencyPositivePattern)
		{
			switch (currencyPositivePattern)
			{
			case 0:
				return "{1}{0}";
			case 1:
				return "{0}{1}";
			case 2:
				return "{1} {0}";
			case 3:
				return "{0} {1}";
			default:
				return string.Empty;
			}
		}

		private static string GetNegativeCurrencyPattern(int currencyNegativePattern)
		{
			switch (currencyNegativePattern)
			{
			case 0:
				return "({1}{0})";
			case 1:
				return "{2}{1}{0}";
			case 2:
				return "{1}{2}{0}";
			case 3:
				return "{1}{0}{2}";
			case 4:
				return "({0}{1})";
			case 5:
				return "{2}{0}{1}";
			case 6:
				return "{0}{2}{1}";
			case 7:
				return "{0}{1}{2}";
			case 8:
				return "{2}{0} {1}";
			case 9:
				return "{2}{1} {0}";
			case 10:
				return "{0} {1}{2}";
			case 11:
				return "{1} {0}{2}";
			case 12:
				return "{1} {2}{0}";
			case 13:
				return "{0}{2} {1}";
			case 14:
				return "({1} {0})";
			case 15:
				return "({0} {1})";
			default:
				return string.Empty;
			}
		}

		private static string GetNumberFormat(string format, string language, out bool isHex, TypeCode typeCode, object originalValue, ref bool isGeneral)
		{
			isHex = false;
			if (string.IsNullOrEmpty(format))
			{
				return "General";
			}
			CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture(language);
			cultureInfo = new CultureInfo(cultureInfo.Name, useUserOverride: false);
			NumberFormatInfo numberFormat = cultureInfo.NumberFormat;
			if (format.Length > 3)
			{
				return GetExcelPictureNumberFormat(format);
			}
			if (format.Length == 1)
			{
				switch (format[0])
				{
				case 'x':
					if ((uint)(typeCode - 5) <= 7u)
					{
						isHex = true;
						return "LOWER(DEC2HEX({0}))";
					}
					return string.Empty;
				case 'X':
					if ((uint)(typeCode - 5) <= 7u)
					{
						isHex = true;
						return "DEC2HEX({0})";
					}
					return string.Empty;
				case 'C':
				case 'c':
					return GetCurrencyFormat(numberFormat, numberFormat.CurrencyDecimalDigits);
				case 'N':
				case 'n':
					return GetNumberFormat(numberFormat, numberFormat.NumberDecimalDigits);
				case 'G':
				case 'g':
					return "General";
				default:
					return GetShortNumberFormat(format[0]);
				}
			}
			if (int.TryParse(format.Substring(1), out int result))
			{
				switch (format[0])
				{
				case 'x':
					if ((uint)(typeCode - 5) <= 7u)
					{
						isHex = true;
						return "LOWER(DEC2HEX({0}, " + result + "))";
					}
					return string.Empty;
				case 'X':
					if ((uint)(typeCode - 5) <= 7u)
					{
						isHex = true;
						return "DEC2HEX({0}, " + result + ")";
					}
					return string.Empty;
				case 'C':
				case 'c':
					return GetCurrencyFormat(numberFormat, result);
				case 'N':
				case 'n':
					return GetNumberFormat(numberFormat, result);
				case 'G':
				case 'g':
				{
					if (originalValue == null)
					{
						return string.Empty;
					}
					char generalFormat = GetGeneralFormat(typeCode, originalValue, result, cultureInfo);
					isGeneral = true;
					return GetLongNumberFormat(generalFormat, result);
				}
				default:
					return GetLongNumberFormat(format[0], result);
				}
			}
			return GetExcelPictureNumberFormat(format);
		}

		private static char GetGeneralFormat(TypeCode typeCode, object originalValue, int precision, CultureInfo cultureInfo)
		{
			string text = "E";
			string text2 = "F";
			if (precision != -1)
			{
				text += precision;
				text2 += precision;
			}
			string text3 = string.Empty;
			string text4 = string.Empty;
			switch (typeCode)
			{
			case TypeCode.Byte:
				text3 = ((byte)originalValue).ToString(text, cultureInfo);
				text4 = ((byte)originalValue).ToString(text2, cultureInfo);
				break;
			case TypeCode.Decimal:
				text3 = ((decimal)originalValue).ToString(text, cultureInfo);
				text4 = ((decimal)originalValue).ToString(text2, cultureInfo);
				break;
			case TypeCode.Double:
				text3 = ((double)originalValue).ToString(text, cultureInfo);
				text4 = ((double)originalValue).ToString(text2, cultureInfo);
				break;
			case TypeCode.Int16:
				text3 = ((short)originalValue).ToString(text, cultureInfo);
				text4 = ((short)originalValue).ToString(text2, cultureInfo);
				break;
			case TypeCode.Int32:
				text3 = ((int)originalValue).ToString(text, cultureInfo);
				text4 = ((int)originalValue).ToString(text2, cultureInfo);
				break;
			case TypeCode.Int64:
				text3 = ((long)originalValue).ToString(text, cultureInfo);
				text4 = ((long)originalValue).ToString(text2, cultureInfo);
				break;
			case TypeCode.SByte:
				text3 = ((sbyte)originalValue).ToString(text, cultureInfo);
				text4 = ((sbyte)originalValue).ToString(text2, cultureInfo);
				break;
			case TypeCode.Single:
				text3 = ((float)originalValue).ToString(text, cultureInfo);
				text4 = ((float)originalValue).ToString(text2, cultureInfo);
				break;
			case TypeCode.UInt16:
				text3 = ((ushort)originalValue).ToString(text, cultureInfo);
				text4 = ((ushort)originalValue).ToString(text2, cultureInfo);
				break;
			case TypeCode.UInt32:
				text3 = ((uint)originalValue).ToString(text, cultureInfo);
				text4 = ((uint)originalValue).ToString(text2, cultureInfo);
				break;
			case TypeCode.UInt64:
				text3 = ((ulong)originalValue).ToString(text, cultureInfo);
				text4 = ((ulong)originalValue).ToString(text2, cultureInfo);
				break;
			}
			if (text3.Length < text4.Length)
			{
				return 'E';
			}
			return 'F';
		}

		private static string GetDateTimePattern(char patternCharacter, DateTimeFormatInfo currInfo)
		{
			RSTrace.ExcelRendererTracer.Assert(currInfo != null, "The DateTimeFormatInfo parameter cannot be null");
			switch (patternCharacter)
			{
			case 'd':
				return currInfo.ShortDatePattern;
			case 'D':
				return currInfo.LongDatePattern;
			case 'f':
				return currInfo.LongDatePattern + " " + currInfo.ShortTimePattern;
			case 'F':
				return currInfo.FullDateTimePattern;
			case 'g':
				return currInfo.ShortDatePattern + " " + currInfo.ShortTimePattern;
			case 'G':
				return currInfo.ShortDatePattern + " " + currInfo.LongTimePattern;
			case 'm':
				return currInfo.MonthDayPattern;
			case 'M':
				return currInfo.MonthDayPattern;
			case 'r':
				return currInfo.RFC1123Pattern;
			case 'R':
				return currInfo.RFC1123Pattern;
			case 's':
				return currInfo.SortableDateTimePattern;
			case 't':
				return currInfo.ShortTimePattern;
			case 'T':
				return currInfo.LongTimePattern;
			case 'u':
				return currInfo.UniversalSortableDateTimePattern;
			case 'U':
				return currInfo.FullDateTimePattern;
			case 'y':
				return currInfo.YearMonthPattern;
			case 'Y':
				return currInfo.YearMonthPattern;
			default:
				return string.Empty;
			}
		}

		private static string GetExcelPictureDateTimeFormat(string format, string timeSeparator, string dateSeparator)
		{
			if (format == null || format.Length <= 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int i = 0;
			int num = 0;
			bool flag = false;
			string text = format.ToUpperInvariant();
			while (i < format.Length)
			{
				switch (text[i])
				{
				case ':':
					flag = true;
					stringBuilder.Append(timeSeparator);
					i++;
					break;
				case '/':
					if (!flag)
					{
						stringBuilder.Append('\\');
					}
					stringBuilder.Append(dateSeparator);
					i++;
					break;
				case '%':
					i++;
					break;
				case '"':
				case '\'':
				{
					char c = text[i];
					stringBuilder.Append('"');
					for (i++; i < format.Length && format[i] != c; i++)
					{
						stringBuilder.Append(format[i]);
					}
					stringBuilder.Append('"');
					i++;
					break;
				}
				case '[':
				{
					stringBuilder.Append('"');
					stringBuilder.Append(format[i]);
					i++;
					bool flag2 = false;
					for (; i < format.Length; i++)
					{
						if (flag2)
						{
							break;
						}
						char c = format[i];
						if (c == ']')
						{
							flag2 = true;
						}
						stringBuilder.Append(c);
					}
					stringBuilder.Append('"');
					break;
				}
				case 'D':
					flag = true;
					num = 1;
					for (i++; i < format.Length && format[i] == 'd'; i++)
					{
						num++;
					}
					if (num > 4)
					{
						stringBuilder.Append("dddd");
						break;
					}
					while (num > 0)
					{
						stringBuilder.Append('d');
						num--;
					}
					break;
				case 'F':
				{
					flag = true;
					char value = (format[i] != 'f') ? '#' : '0';
					if (stringBuilder.Length == 0 || '.' != stringBuilder[stringBuilder.Length - 1])
					{
						stringBuilder.Append('.');
					}
					num = 0;
					for (; i < text.Length && text[i] == 'F'; i++)
					{
						if (num < 7)
						{
							stringBuilder.Append(value);
						}
						num++;
					}
					break;
				}
				case 'G':
					i++;
					break;
				case 'H':
					flag = true;
					stringBuilder.Append('h');
					i++;
					num = 1;
					while (i < format.Length && format[i] == 'h')
					{
						i++;
						num++;
					}
					if (num > 1)
					{
						stringBuilder.Append("h");
					}
					break;
				case 'M':
					flag = true;
					i++;
					num = 1;
					while (i < format.Length && format[i] == 'm')
					{
						i++;
						num++;
					}
					if (num > 3)
					{
						stringBuilder.Append("mmm");
						break;
					}
					while (num >= 1)
					{
						stringBuilder.Append('m');
						num--;
					}
					break;
				case 'S':
					flag = true;
					stringBuilder.Append('s');
					num = 1;
					i++;
					while (i < format.Length && format[i] == 's')
					{
						i++;
						num++;
					}
					if (num > 1)
					{
						stringBuilder.Append('s');
					}
					break;
				case 'T':
					flag = true;
					if (i + 1 < format.Length && format[i + 1] == 't')
					{
						stringBuilder.Append("AM/PM");
						for (i++; format.Length > i && format[i] == 't'; i++)
						{
						}
					}
					else
					{
						stringBuilder.Append("A/P");
						i++;
					}
					break;
				case 'Y':
					flag = true;
					i++;
					num = 1;
					for (; i < format.Length && format[i] == 'y'; i++)
					{
						num++;
					}
					if (num <= 2)
					{
						stringBuilder.Append("yy");
					}
					else
					{
						stringBuilder.Append("yyyy");
					}
					break;
				case 'Z':
					i++;
					break;
				case '.':
					stringBuilder.Append(format[i]);
					i++;
					break;
				case '\\':
					stringBuilder.Append(format[i]);
					i++;
					if (i < format.Length)
					{
						stringBuilder.Append(format[i]);
						i++;
						break;
					}
					return string.Empty;
				default:
				{
					char c = format[i];
					if (char.IsDigit(c))
					{
						stringBuilder.Append('\\');
					}
					stringBuilder.Append(c);
					i++;
					break;
				}
				}
			}
			return stringBuilder.ToString();
		}

		private static string GetDateTimeFormat(string format, string language, object rplCalendar)
		{
			if (format == null || format.Length <= 0)
			{
				format = "G";
			}
			CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture(language);
			cultureInfo = new CultureInfo(cultureInfo.Name, useUserOverride: false);
			if (cultureInfo.DateTimeFormat == null)
			{
				cultureInfo.DateTimeFormat = DateTimeFormatInfo.CurrentInfo;
			}
			if (rplCalendar != null)
			{
				bool flag = false;
				Calendar calendarInstance = LayoutConvert.GetCalendarInstance((RPLFormat.Calendars)rplCalendar);
				Type type = calendarInstance.GetType();
				Calendar[] optionalCalendars = cultureInfo.OptionalCalendars;
				foreach (Calendar calendar in optionalCalendars)
				{
					if (type == calendar.GetType())
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					cultureInfo.DateTimeFormat.Calendar = calendarInstance;
				}
				else
				{
					calendarInstance = new GregorianCalendar();
					cultureInfo.DateTimeFormat.Calendar = calendarInstance;
				}
			}
			string empty = string.Empty;
			DateTimeFormatInfo dateTimeFormat = cultureInfo.DateTimeFormat;
			string timeSeparator = dateTimeFormat.TimeSeparator;
			string dateSeparator = dateTimeFormat.DateSeparator;
			empty = ((format.Length != 1) ? format : GetDateTimePattern(format[0], dateTimeFormat));
			return GetExcelPictureDateTimeFormat(empty, timeSeparator, dateSeparator);
		}

		private static int GetLanguageLCID(string language)
		{
			CultureInfo cultureInfo = new CultureInfo(language, useUserOverride: false);
			if (cultureInfo.IsNeutralCulture)
			{
				try
				{
					cultureInfo = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
				}
				catch (ArgumentException)
				{
					if (string.Compare(language, "zh-CHT", StringComparison.Ordinal) == 0)
					{
						cultureInfo = new CultureInfo("zh-TW", useUserOverride: false);
					}
					else if (string.Compare(language, "zh-CHS", StringComparison.Ordinal) == 0)
					{
						cultureInfo = new CultureInfo("zh-CN", useUserOverride: false);
					}
				}
			}
			return cultureInfo.LCID;
		}

		private static string GetFormatStringPrefix(RPLFormat.Calendars? calendar, int numeralVariant, string language, string numeralLanguage)
		{
			if (-1 == numeralVariant || !calendar.HasValue || numeralLanguage == null || language == null)
			{
				return string.Empty;
			}
			string excelCalendarDigits = GetExcelCalendarDigits(calendar.Value);
			string value = string.Empty;
			int languageLCID = GetLanguageLCID(language);
			string value2 = string.Empty;
			if (numeralVariant != 2)
			{
				if (numeralVariant.Equals("3"))
				{
					value2 = GetExcelNumeralVariant(numeralLanguage);
				}
				else
				{
					switch (numeralVariant)
					{
					case 4:
						value = "[DBNUM1]";
						languageLCID = GetLanguageLCID(numeralLanguage);
						value2 = string.Empty;
						break;
					case 5:
						value = "[DBNUM2]";
						languageLCID = GetLanguageLCID(numeralLanguage);
						value2 = string.Empty;
						break;
					case 6:
						value = "[DBNUM3]";
						languageLCID = GetLanguageLCID(numeralLanguage);
						value2 = string.Empty;
						break;
					case 7:
						value = "[DBNUM4]";
						languageLCID = GetLanguageLCID(numeralLanguage);
						value2 = string.Empty;
						break;
					}
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(value);
				stringBuilder.Append("[$-");
				stringBuilder.Append(value2);
				stringBuilder.Append(excelCalendarDigits);
				string text = System.Convert.ToString(languageLCID, 16);
				if (text.Length < 4)
				{
					for (int num = 4 - text.Length; num > 0; num--)
					{
						text = "0" + text;
					}
				}
				stringBuilder.Append(text);
				stringBuilder.Append("]");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		private static string GetExcelNumeralVariant(string numeralLanguagePrefix)
		{
			char[] separator = new char[1]
			{
				'-'
			};
			switch (numeralLanguagePrefix.Split(separator)[0].ToUpperInvariant())
			{
			case "AR":
				return "2";
			case "BN":
				return "5";
			case "BO":
				return "F";
			case "FA":
				return "3";
			case "GU":
				return "7";
			case "HI":
				return "4";
			case "KN":
				return "B";
			case "KOK":
				return "4";
			case "LO":
				return "E";
			case "MR":
				return "4";
			case "MS":
				return "C";
			case "OR":
				return "8";
			case "PA":
				return "6";
			case "SA":
				return "4";
			case "TA":
				return "9";
			case "TE":
				return "A";
			case "TH":
				return "D";
			case "UR":
				return "3";
			default:
				return null;
			}
		}

		private static string GetExcelCalendarDigits(RPLFormat.Calendars calendar)
		{
			switch (calendar)
			{
			case RPLFormat.Calendars.Gregorian:
				return "01";
			case RPLFormat.Calendars.GregorianArabic:
				return "0A";
			case RPLFormat.Calendars.GregorianMiddleEastFrench:
				return "09";
			case RPLFormat.Calendars.GregorianTransliteratedEnglish:
				return "0B";
			case RPLFormat.Calendars.GregorianTransliteratedFrench:
				return "0C";
			case RPLFormat.Calendars.GregorianUSEnglish:
				return "02";
			case RPLFormat.Calendars.Hebrew:
				return "08";
			case RPLFormat.Calendars.Hijri:
				return "06";
			case RPLFormat.Calendars.Japanese:
				return "03";
			case RPLFormat.Calendars.Korean:
				return "05";
			case RPLFormat.Calendars.Taiwan:
				return "04";
			case RPLFormat.Calendars.ThaiBuddhist:
				return "07";
			default:
				return null;
			}
		}

		internal static ExcelDataType GetDataType(TypeCode type)
		{
			switch (type)
			{
			case TypeCode.Boolean:
				return ExcelDataType.Boolean;
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
			case TypeCode.DateTime:
				return ExcelDataType.Number;
			case TypeCode.Char:
			case TypeCode.String:
				return ExcelDataType.String;
			default:
				return ExcelDataType.Blank;
			}
		}

		internal static bool IsExcelNumberDataType(TypeCode type)
		{
			if (GetDataType(type) == ExcelDataType.Number)
			{
				return true;
			}
			return false;
		}

		internal static string GetExcelNumberFormat(string rsNumberFormat, string language, RPLFormat.Calendars rplCalendar, string numeralLanguage, int numeralVariant, TypeCode type, object originalValue, out string hexFormula, out bool invalidFormatCode)
		{
			hexFormula = null;
			string empty = string.Empty;
			string str = string.Empty;
			invalidFormatCode = false;
			if (type != TypeCode.DateTime)
			{
				bool isGeneral = false;
				empty = GetNumberFormat(rsNumberFormat, language, out bool isHex, type, originalValue, ref isGeneral);
				if (string.IsNullOrEmpty(empty))
				{
					invalidFormatCode = true;
				}
				else if (!isHex)
				{
					if (empty != null && empty.Length > 0 && empty != "General")
					{
						str = GetFormatStringPrefix(rplCalendar, numeralVariant, language, numeralLanguage);
					}
				}
				else
				{
					hexFormula = empty;
				}
			}
			else
			{
				empty = GetDateTimeFormat(rsNumberFormat, language, rplCalendar);
				if (string.IsNullOrEmpty(empty))
				{
					invalidFormatCode = true;
				}
				else
				{
					str = GetFormatStringPrefix(rplCalendar, numeralVariant, language, numeralLanguage);
				}
			}
			return str + empty;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.ReportingServices.ReportProcessing.Utils
{
	internal sealed class StringKeyGenerator
	{
		private static class TypePrefix
		{
			public const char Null = '0';

			public const char Int = '0';

			public const char Decimal = '0';

			public const char Double = '0';

			public const char DateTime = '1';

			public const char String = '2';

			public const char Boolean = '3';
		}

		private static class TypeSuffix
		{
			public const char Null = '0';

			public const char Int = '1';

			public const char Decimal = '2';

			public const char Double = '3';
		}

		private const char GroupSeparator = '\u001d';

		private const char UnitSeparator = '\u001f';

		private static string NullAsNumericKey = "0D\u001f0";

		private static string NullKey = ".";

		private readonly CultureInfo _cultureInfo;

		private readonly CompareInfo _compareInfo;

		private readonly CompareOptions _compareOptions;

		private readonly bool _nullAsNumeric;

		private static char[] Sorted64Chars = new char[64]
		{
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F',
			'G',
			'H',
			'I',
			'J',
			'K',
			'L',
			'M',
			'N',
			'O',
			'P',
			'Q',
			'R',
			'S',
			'T',
			'U',
			'V',
			'W',
			'X',
			'Y',
			'Z',
			'a',
			'b',
			'c',
			'd',
			'e',
			'f',
			'g',
			'h',
			'i',
			'j',
			'k',
			'l',
			'm',
			'n',
			'o',
			'p',
			'q',
			'r',
			's',
			't',
			'u',
			'v',
			'w',
			'x',
			'y',
			'z',
			'{',
			'}'
		};

		public StringKeyGenerator(CompareInfo compareInfo, CompareOptions compareOptions, bool nullAsBlank, bool useOrdinalStringComparison)
		{
			_compareInfo = compareInfo;
			_cultureInfo = new CultureInfo(compareInfo.Name);
			_compareOptions = compareOptions;
			_nullAsNumeric = nullAsBlank;
			if (useOrdinalStringComparison)
			{
				_compareOptions = CompareOptions.Ordinal;
			}
		}

		public StringKeyGenerator(string cultureName, CompareOptions compareOptions, bool nullAsBlank, bool useOrdinalStringComparison)
			: this(CompareInfo.GetCompareInfo(cultureName), compareOptions, nullAsBlank, useOrdinalStringComparison)
		{
		}

		public string GetKey(IEnumerable<object> values)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (object value in values)
			{
				if (flag)
				{
					stringBuilder.Append('\u001d');
				}
				else
				{
					flag = true;
				}
				string key = GetKey(value);
				stringBuilder.Append(key);
			}
			return stringBuilder.ToString();
		}

		public string GetKey(object value)
		{
			if (value == null)
			{
				return GetNullSortKey();
			}
			return GetSortKey((IConvertible)value);
		}

		private string GetSortKey(IConvertible value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			switch (value.GetTypeCode())
			{
			case TypeCode.Empty:
				stringBuilder.Append(GetNullSortKey());
				break;
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
				stringBuilder.Append('0');
				stringBuilder.Append(GetIntSortKey(value));
				stringBuilder.Append('\u001f');
				stringBuilder.Append('1');
				break;
			case TypeCode.Decimal:
				stringBuilder.Append('0');
				stringBuilder.Append(GetDecimalSortKey(value));
				stringBuilder.Append('\u001f');
				stringBuilder.Append('2');
				break;
			case TypeCode.Single:
			case TypeCode.Double:
				stringBuilder.Append('0');
				stringBuilder.Append(GetDoubleSortKey(value));
				stringBuilder.Append('\u001f');
				stringBuilder.Append('3');
				break;
			case TypeCode.DateTime:
				stringBuilder.Append('1');
				stringBuilder.Append(GetDateTimeSortKey(value));
				break;
			case TypeCode.Char:
			case TypeCode.String:
				stringBuilder.Append('2');
				stringBuilder.Append(GetStringSortKey(value));
				break;
			case TypeCode.Boolean:
				stringBuilder.Append('3');
				stringBuilder.Append(GetBooleanSortKey(value));
				break;
			default:
				throw new NotSupportedException("This data type doesn't have prefix.");
			}
			return stringBuilder.ToString();
		}

		private string GetNullSortKey()
		{
			if (!_nullAsNumeric)
			{
				return NullKey;
			}
			return NullAsNumericKey;
		}

		private string GetIntSortKey(IConvertible value)
		{
			long num = value.ToInt64(null);
			return GetNumericSortKey(string.Format(CultureInfo.InvariantCulture, "{0:e20}", value), num == 0);
		}

		private string GetDecimalSortKey(IConvertible value)
		{
			decimal d = value.ToDecimal(null);
			return GetNumericSortKey(string.Format(CultureInfo.InvariantCulture, "{0:e28}", value), d == 0m);
		}

		private string GetDoubleSortKey(IConvertible value)
		{
			double num = value.ToDouble(null);
			if (double.IsNegativeInfinity(num))
			{
				return "A";
			}
			if (double.IsPositiveInfinity(num))
			{
				return "G";
			}
			if (double.IsNaN(num))
			{
				return "H";
			}
			return GetNumericSortKey(string.Format(CultureInfo.InvariantCulture, "{0:e28}", value), num == 0.0);
		}

		private string GetDateTimeSortKey(IConvertible value)
		{
			long ticks = value.ToDateTime(null).Ticks;
			return GetNumericSortKey(string.Format(CultureInfo.InvariantCulture, "{0:e20}", ticks), ticks == 0);
		}

		private string GetStringSortKey(IConvertible value)
		{
			string text = value.ToString(null);
			if (_compareOptions.HasFlag(CompareOptions.Ordinal))
			{
				return text;
			}
			if (_compareOptions.HasFlag(CompareOptions.OrdinalIgnoreCase))
			{
				return text.ToUpperInvariant();
			}
			return ToComparableBase64String(_compareInfo.GetSortKey(text, _compareOptions).KeyData);
		}

		private string GetBooleanSortKey(IConvertible value)
		{
			if (!value.ToBoolean(null))
			{
				return "0";
			}
			return "1";
		}

		private static string GetNumericSortKey(string scientific, bool isZero)
		{
			if (isZero)
			{
				return "D";
			}
			char[] array = new char[scientific.Length];
			bool flag = scientific[0] == '-';
			bool flag2 = scientific[scientific.Length - 4] == '-';
			char c = array[0] = ((!flag) ? (flag2 ? 'E' : 'F') : ((!flag2) ? 'B' : 'C'));
			array[1] = ReverseIfNegative(scientific[scientific.Length - 3], flag2 ^ flag);
			array[2] = ReverseIfNegative(scientific[scientific.Length - 2], flag2 ^ flag);
			array[3] = ReverseIfNegative(scientific[scientific.Length - 1], flag2 ^ flag);
			int num = 4;
			int num2 = 0;
			int num3 = 4;
			if (scientific[num2] == '-')
			{
				num2++;
			}
			char c2 = scientific[num2++];
			while (true)
			{
				switch (c2)
				{
				default:
					array[num++] = ReverseIfNegative(c2, flag);
					if (c2 != '0')
					{
						num3 = num;
					}
					break;
				case '.':
					break;
				case 'e':
					if (flag && num3 < array.Length)
					{
						array[num3++] = 'A';
					}
					return new string(array, 0, num3);
				}
				c2 = scientific[num2++];
			}
		}

		private static char ReverseIfNegative(char digit, bool isNegative)
		{
			if (isNegative)
			{
				int num = digit - 48;
				num = 9 - num;
				return (char)(48 + num);
			}
			return digit;
		}

		public static string ToComparableBase64String(byte[] bytes)
		{
			if (bytes == null)
			{
				return null;
			}
			if (bytes.Length == 0)
			{
				return string.Empty;
			}
			char[] array = new char[bytes.Length * 2];
			int length = 0;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 63;
			int i = 0;
			for (int num5 = bytes.Length * 8; i < num5; i += 6)
			{
				if (num2 < 8)
				{
					num3 = ((num >= bytes.Length) ? (num3 << 8) : ((num3 << 8) | bytes[num++]));
					num2 += 8;
				}
				int num6 = num2 - 6;
				int num7 = (num3 & (num4 << num6)) >> num6;
				array[length++] = Sorted64Chars[num7];
				num2 -= 6;
			}
			return new string(array, 0, length);
		}
	}
}

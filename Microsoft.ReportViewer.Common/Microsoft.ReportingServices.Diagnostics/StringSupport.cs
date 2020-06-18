using System;
using System.Globalization;
using System.Text;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal static class StringSupport
	{
		public static bool StartsWith(string str, string prefix, bool ignoreCase, CultureInfo culture)
		{
			if (str == null || prefix == null)
			{
				throw new ArgumentException("StringStartsWith can't accept null parameters");
			}
			if (str.Length < prefix.Length)
			{
				return false;
			}
			return string.Compare(str, 0, prefix, 0, prefix.Length, ignoreCase, culture) == 0;
		}

		public static bool EndsWith(string str, string postfix, bool ignoreCase, CultureInfo culture)
		{
			if (str == null || postfix == null)
			{
				throw new ArgumentException("StringEndsWith can't accept null parameters");
			}
			if (str.Length < postfix.Length)
			{
				return false;
			}
			return string.Compare(str, str.Length - postfix.Length, postfix, 0, postfix.Length, ignoreCase, culture) == 0;
		}

		public static byte[] ToUnicodeArray(string s)
		{
			if (s == null)
			{
				return null;
			}
			return Encoding.Unicode.GetBytes(s);
		}

		public static string FromUnicodeArray(byte[] u)
		{
			if (u == null)
			{
				return null;
			}
			return Encoding.Unicode.GetString(u);
		}
	}
}

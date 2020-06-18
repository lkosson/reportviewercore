using System;

namespace Microsoft.ReportingServices.Common
{
	internal static class UrlUtil
	{
		public static string UrlEncode(string input)
		{
			if (input == null)
			{
				return null;
			}
			return Uri.EscapeDataString(input);
		}

		public static string UrlDecode(string input)
		{
			if (input == null)
			{
				return null;
			}
			input = input.Replace("+", " ");
			return Uri.UnescapeDataString(input);
		}
	}
}

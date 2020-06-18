using System;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator
{
	internal static class ExcelGeneratorStringUtil
	{
		internal static void ConvertWhitespaceAppendString(string value, StringBuilder stringBuilder, bool checkEastAsianChars, out bool foundEastAsianChar)
		{
			foundEastAsianChar = false;
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				switch (c)
				{
				case '\r':
					stringBuilder.Append('\n');
					if (i + 1 < value.Length && value[i + 1] == '\n')
					{
						i++;
					}
					continue;
				case '\n':
					stringBuilder.Append(c);
					continue;
				}
				if (checkEastAsianChars && ((c > 'ᄀ' && c <= 'ᇿ') || (c > '⺀' && c <= '\ud7af') || (c > '豈' && c <= '\uffef') || (c > '\ud800' && c <= '\udbff')))
				{
					foundEastAsianChar = true;
					checkEastAsianChars = false;
				}
				if (c <= '\u001f')
				{
					stringBuilder.Append(' ');
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
		}

		internal static void ConvertWhitespaceAppendString(string value, StringBuilder stringBuilder)
		{
			bool foundEastAsianChar = false;
			ConvertWhitespaceAppendString(value, stringBuilder, checkEastAsianChars: false, out foundEastAsianChar);
		}

		public static StringBuilder SanitizeSheetName(string sheetName)
		{
			int num = 0;
			int val = sheetName.Length;
			if (sheetName.StartsWith("'", StringComparison.Ordinal))
			{
				num = 1;
			}
			if (sheetName.EndsWith("'", StringComparison.Ordinal))
			{
				val = sheetName.Length - 1;
			}
			val = Math.Min(val, 31);
			StringBuilder stringBuilder = new StringBuilder(31);
			for (int i = num; i < val; i++)
			{
				char value = sheetName[i];
				if ("[]:?*/\\".IndexOf(value) < 0)
				{
					stringBuilder.Append(value);
				}
			}
			return stringBuilder;
		}

		internal static string Truncate(string value, int length)
		{
			if (value != null && value.Length > length)
			{
				return value.Substring(0, length);
			}
			return value;
		}
	}
}

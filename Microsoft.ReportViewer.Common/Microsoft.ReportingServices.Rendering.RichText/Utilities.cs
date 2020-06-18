using System.Text;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal class Utilities
	{
		internal const int EASTASIACHAR_RANGE1_START = 4352;

		internal const int EASTASIACHAR_RANGE1_END = 4607;

		internal const int EASTASIACHAR_RANGE2_START = 11904;

		internal const int EASTASIACHAR_RANGE2_END = 55215;

		internal const int EASTASIACHAR_RANGE3_START = 63744;

		internal const int EASTASIACHAR_RANGE3_END = 65519;

		internal const int EASTASIACHAR_RANGE4_START = 55296;

		internal const int EASTASIACHAR_RANGE4_END = 56319;

		internal const int SPACE_MODIFIER_LETTERS_RANGE_START = 688;

		internal const int SPACE_MODIFIER_LETTERS_RANGE_END = 767;

		internal const int SCRIPT_LOOKUP_DEFAULT = 0;

		internal const int SCRIPT_LOOKUP_KANA = 14;

		internal const int SCRIPT_LOOKUP_HIRAGANA = 15;

		internal const int SCRIPT_LOOKUP_KATAKANA = 16;

		internal const int SCRIPT_LOOKUP_HAN = 17;

		internal const int SCRIPT_LOOKUP_HANGUL = 18;

		internal const int SCRIPT_LOOKUP_OLD_HANGUL = 19;

		internal const int SCRIPT_LOOKUP_BOPOMOFO = 20;

		internal const int SCRIPT_LOOKUP_KATAKANA_EXT = -1;

		internal static bool IsEastAsianChar(char c)
		{
			if ((c > 'ᄀ' && c <= 'ᇿ') || (c > '⺀' && c <= '\ud7af') || (c > '豈' && c <= '\uffef') || (c > '\ud800' && c <= '\udbff') || (c >= 'ʰ' && c <= '\u02ff'))
			{
				return true;
			}
			return false;
		}

		internal static string ConvertTabAndCheckEastAsianChars(string value, out bool hasEastAsianChars)
		{
			hasEastAsianChars = false;
			if (string.IsNullOrEmpty(value))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (char c in value)
			{
				if (c == '\t')
				{
					stringBuilder.Append(' ');
					continue;
				}
				if (flag && IsEastAsianChar(c))
				{
					hasEastAsianChars = true;
					flag = false;
				}
				stringBuilder.Append(c);
			}
			return stringBuilder.ToString();
		}

		internal static int GetEastAsianScriptIDFromText(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return 0;
			}
			char c = text[0];
			if (c >= 'ᄀ' && c <= 'ᇿ')
			{
				return 18;
			}
			if (c >= '⺀' && c <= '\u2fdf')
			{
				return 17;
			}
			if (c >= '⿰' && c <= '\u2fff')
			{
				return 17;
			}
			if (c >= '\u3000' && c <= '〿')
			{
				return 17;
			}
			if (c >= '\u3040' && c <= 'ゟ')
			{
				return 15;
			}
			if (c >= '゠' && c <= 'ヿ')
			{
				return 16;
			}
			if (c >= '\u3100' && c <= '\u312f')
			{
				return 20;
			}
			if (c >= '\u3130' && c <= '\u318f')
			{
				return 18;
			}
			if (c >= '㆐' && c <= '㆟')
			{
				return 17;
			}
			if (c >= 'ㆠ' && c <= '\u31bf')
			{
				return 17;
			}
			if (c >= '㇀' && c <= 'ㇿ')
			{
				return -1;
			}
			if (c >= '㈀' && c <= '\u321f')
			{
				return 18;
			}
			if (c >= '㈠' && c <= '㏿')
			{
				return 16;
			}
			if (c >= '㐀' && c <= '䷿')
			{
				return 17;
			}
			if (c >= '一' && c <= '龿')
			{
				return 17;
			}
			if (c >= 'ꀀ' && c <= '\ua4cf')
			{
				return 18;
			}
			if (c >= '가' && c <= '\ud7af')
			{
				return 18;
			}
			if (c >= '豈' && c <= '\ufaff')
			{
				return 17;
			}
			if (c >= '\uff00' && c <= '\uffef')
			{
				return 16;
			}
			return 0;
		}
	}
}

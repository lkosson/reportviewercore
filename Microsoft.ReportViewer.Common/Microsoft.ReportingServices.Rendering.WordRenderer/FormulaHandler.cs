using System.Collections;
using System.Text.RegularExpressions;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal sealed class FormulaHandler
	{
		internal enum GlobalExpressionType
		{
			PageNumber,
			TotalPages,
			ReportName,
			Unknown
		}

		private static Regex m_RegexGlobalOnly;

		private static Regex m_RegexAmpDetection;

		private static RegexOptions m_regexOptions;

		internal const string GLOBALS_PAGENUMBER = "PAGENUMBER";

		internal const string GLOBALS_TOTALPAGES = "TOTALPAGES";

		internal const string GLOBALS_OVERALLPAGENUMBER = "OVERALLPAGENUMBER";

		internal const string GLOBALS_OVERALLTOTALPAGES = "OVERALLTOTALPAGES";

		internal const string GLOBALS_REPORTNAME = "REPORTNAME";

		static FormulaHandler()
		{
			InitRegularExpressions();
		}

		private static void InitRegularExpressions()
		{
			m_regexOptions = (RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);
			string text = "(\"((\"\")|[^\"])*\")";
			string text2 = "Globals";
			string text3 = "Item";
			string text4 = "&";
			string str = Regex.Escape("!");
			string str2 = Regex.Escape(".");
			string text5 = "[" + str + str2 + "]";
			string text6 = Regex.Escape("(");
			string text7 = Regex.Escape(")");
			string str3 = text2 + "(" + text5 + ")?(" + text3 + ")?(" + text6 + ")?(?<GlobalParameterName>(\\s*" + text + "\\s*)|[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Cf}]*)(" + text7 + ")?";
			m_RegexGlobalOnly = new Regex("^\\s*" + str3 + "\\s*$", m_regexOptions);
			m_RegexAmpDetection = new Regex("\\s*" + text + "\\s*|(?<detected>" + text4 + ")", m_regexOptions);
		}

		internal FormulaHandler()
		{
		}

		internal static ArrayList ProcessHeaderFooterFormula(string formulaExpression)
		{
			if (formulaExpression == null)
			{
				return null;
			}
			ArrayList arrayList = new ArrayList();
			string text = null;
			MatchCollection matchCollection = m_RegexAmpDetection.Matches(formulaExpression, 0);
			if (matchCollection == null || matchCollection.Count == 0)
			{
				text = formulaExpression;
			}
			else
			{
				int num = 0;
				string text2 = null;
				string text3 = null;
				for (int i = 0; i <= matchCollection.Count; i++)
				{
					text3 = ((i >= matchCollection.Count) ? formulaExpression.Substring(num, formulaExpression.Length - num) : formulaExpression.Substring(num, matchCollection[i].Index - num));
					text3 = text3.Trim();
					if (text3.Length > 0)
					{
						text = ((text != null) ? (text + "&" + text3) : text3);
					}
					if (i >= matchCollection.Count)
					{
						continue;
					}
					text3 = formulaExpression.Substring(matchCollection[i].Index, matchCollection[i].Length);
					num = matchCollection[i].Index + matchCollection[i].Length;
					if (text3 == "&")
					{
						continue;
					}
					text2 = text3.Trim();
					int length = text2.Length;
					if (length > 1 && text2[0] == '"' && text2[length - 1] == '"')
					{
						text2 = text2.Substring(1, length - 2);
					}
					if (text != null)
					{
						Match match = m_RegexGlobalOnly.Match(text);
						if (!match.Success)
						{
							return null;
						}
						GlobalExpressionType globalExpressionType = WordHeaderFooterFormula(match, text);
						if (globalExpressionType == GlobalExpressionType.Unknown)
						{
							return null;
						}
						arrayList.Add(globalExpressionType);
						text = null;
					}
					arrayList.Add(text2);
				}
			}
			if (text != null)
			{
				Match match2 = m_RegexGlobalOnly.Match(text);
				if (!match2.Success)
				{
					return null;
				}
				GlobalExpressionType globalExpressionType2 = WordHeaderFooterFormula(match2, text);
				if (globalExpressionType2 == GlobalExpressionType.Unknown)
				{
					return null;
				}
				arrayList.Add(globalExpressionType2);
			}
			return arrayList;
		}

		private static GlobalExpressionType WordHeaderFooterFormula(Match match, string formulaExpression)
		{
			GlobalExpressionType result = GlobalExpressionType.Unknown;
			string text = match.Result("${GlobalParameterName}");
			text = text.Replace("\"", string.Empty);
			if (text != null && text.Length != 0)
			{
				switch (text.Trim().ToUpperInvariant())
				{
				case "PAGENUMBER":
				case "OVERALLPAGENUMBER":
					result = GlobalExpressionType.PageNumber;
					break;
				case "TOTALPAGES":
				case "OVERALLTOTALPAGES":
					result = GlobalExpressionType.TotalPages;
					break;
				case "REPORTNAME":
					result = GlobalExpressionType.ReportName;
					break;
				default:
					result = GlobalExpressionType.Unknown;
					break;
				}
			}
			return result;
		}
	}
}

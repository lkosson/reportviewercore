using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert
{
	internal sealed class FormulaHandler
	{
		private int m_reportItemCount;

		private ArrayList m_reportItemsNames;

		private static Hashtable m_FormulaMapping;

		private static Regex m_RegexGlobalOnly;

		private static Regex m_RegexReportItemNameFormat;

		private static Regex m_RegexFunctionName;

		private static Regex m_RegexIdentifier;

		private static Regex m_RegexNonConstant;

		private static Regex m_RegexFieldDetection;

		private static Regex m_RegexReportItemsDetection;

		private static Regex m_RegexGlobalsDetection;

		private static Regex m_RegexAmpDetection;

		private static Regex m_RegexParametersDetection;

		private static Regex m_RegexUserDetection;

		private static Regex m_RegexAggregatesDetection;

		private static Regex m_RegexStringLiteralOnly;

		private static Regex m_RegexNothingOnly;

		private static Regex m_RegexReportItemName;

		private static Regex m_RegexSpecialFunction;

		private static RegexOptions m_regexOptions;

		private static ArrayList m_VBModulePropertiesSupported;

		private static ArrayList m_VBModulePropertiesUnSupported;

		static FormulaHandler()
		{
			InitFormulaMapping();
			InitRegularExpressions();
		}

		private static void InitRegularExpressions()
		{
			m_regexOptions = (RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);
			string text = "(\"((\"\")|[^\"])*\")";
			string text2 = "RunningValue";
			string text3 = "RowNumber";
			string text4 = "Fields";
			string text5 = "ReportItems";
			string text6 = "Globals";
			string text7 = "Parameters";
			string text8 = "Aggregates";
			string text9 = "User";
			string text10 = "Item";
			string text11 = "&";
			string str = Regex.Escape("()!#,.:&*+-/\\^<=>");
			string text12 = "(^|[" + str + "\\s])";
			string text13 = "($|[" + str + "\\s])";
			string text14 = Regex.Escape("!");
			string text15 = Regex.Escape(".");
			string text16 = "[" + text14 + text15 + "]";
			Regex.Escape(":");
			string text17 = Regex.Escape("(");
			string text18 = Regex.Escape(")");
			string str2 = text6 + "(" + text16 + ")?(" + text10 + ")?(" + text17 + ")?(?<GlobalParameterName>(\\s*" + text + "\\s*)|[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Cf}]*)(" + text18 + ")?";
			m_RegexNonConstant = new Regex("^\\s*=", m_regexOptions);
			m_RegexFieldDetection = new Regex(text12 + "(?<detected>" + text4 + ")" + text13, m_regexOptions);
			m_RegexReportItemsDetection = new Regex(text12 + "(?<detected>" + text5 + ")" + text13, m_regexOptions);
			m_RegexGlobalsDetection = new Regex(text12 + "(?<detected>" + text6 + ")" + text13, m_regexOptions);
			m_RegexParametersDetection = new Regex(text12 + "(?<detected>" + text7 + ")" + text13, m_regexOptions);
			m_RegexAggregatesDetection = new Regex(text12 + "(?<detected>" + text8 + ")" + text13, m_regexOptions);
			m_RegexUserDetection = new Regex(text12 + "(?<detected>" + text9 + ")" + text13, m_regexOptions);
			string pattern = text5 + "(" + text15 + text10 + ")?" + text17 + "(?<ReportItemname>" + text + "|[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Cf}]*)" + text18 + "(" + text15 + "value)?";
			m_RegexIdentifier = new Regex("[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Cf}]*", m_regexOptions);
			m_RegexNothingOnly = new Regex("^\\s*Nothing\\s*$", m_regexOptions);
			m_RegexReportItemName = new Regex(text5 + text14 + "(?<reportitemname>[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Cf}]*)(" + text15 + "(value))?", m_regexOptions);
			m_RegexReportItemNameFormat = new Regex(pattern, m_regexOptions);
			m_RegexFunctionName = new Regex("(?<prefix>" + text12 + ")(?<FunctionName>[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Cf}]*)\\s*" + text17, m_regexOptions);
			m_RegexStringLiteralOnly = new Regex("^\\s*\"(?<string>((\"\")|[^\"])*)\"\\s*$", m_regexOptions);
			m_RegexNothingOnly = new Regex("^\\s*Nothing\\s*$", m_regexOptions);
			m_RegexGlobalOnly = new Regex("^\\s*" + str2 + "\\s*$", m_regexOptions);
			m_RegexAmpDetection = new Regex("\\s*" + text + "\\s*|(?<detected>" + text11 + ")", m_regexOptions);
			m_RegexSpecialFunction = new Regex("(?<prefix>" + text12 + ")(?<sfname>" + text2 + "|" + text3 + "|First|Last|Sum|Avg|Max|Min|CountDistinct|Count|StDevP|VarP|StDev|Var|Aggregate)\\s*\\(", m_regexOptions);
		}

		private static void InitFormulaMapping()
		{
			m_FormulaMapping = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			m_FormulaMapping.Add("Asc", "Code");
			m_FormulaMapping.Add("Cdate", "DateValue");
			m_FormulaMapping.Add("Chr", "Char");
			m_FormulaMapping.Add("DateSerial", "Date");
			m_FormulaMapping.Add("Iif", "If");
			m_FormulaMapping.Add("Lcase", "Lower");
			m_FormulaMapping.Add("Ucase", "Upper");
			m_FormulaMapping.Add("Abs", "Abs");
			m_FormulaMapping.Add("Atan", "Atan");
			m_FormulaMapping.Add("Choose", "Choose");
			m_FormulaMapping.Add("Cos", "Cos");
			m_FormulaMapping.Add("DateValue", "DateValue");
			m_FormulaMapping.Add("Day", "Day");
			m_FormulaMapping.Add("DDB", "DDB");
			m_FormulaMapping.Add("Exp", "Exp");
			m_FormulaMapping.Add("FV", "FV");
			m_FormulaMapping.Add("Hour", "Hour");
			m_FormulaMapping.Add("Int", "Int");
			m_FormulaMapping.Add("Ipmt", "Ipmt");
			m_FormulaMapping.Add("Irr", "Irr");
			m_FormulaMapping.Add("Left", "Left");
			m_FormulaMapping.Add("Minute", "Minute");
			m_FormulaMapping.Add("Mirr", "Mirr");
			m_FormulaMapping.Add("Month", "Month");
			m_FormulaMapping.Add("Now", "Now");
			m_FormulaMapping.Add("Nper", "Nper");
			m_FormulaMapping.Add("Npv", "Npv");
			m_FormulaMapping.Add("Pmt", "Pmt");
			m_FormulaMapping.Add("PPmt", "PPmt");
			m_FormulaMapping.Add("Pv", "Pv");
			m_FormulaMapping.Add("Rate", "Rate");
			m_FormulaMapping.Add("Right", "Right");
			m_FormulaMapping.Add("Second", "Second");
			m_FormulaMapping.Add("Sign", "Sign");
			m_FormulaMapping.Add("Sin", "Sin");
			m_FormulaMapping.Add("Sln", "Sln");
			m_FormulaMapping.Add("Sqrt", "Sqrt");
			m_FormulaMapping.Add("Syd", "Syd");
			m_FormulaMapping.Add("Tan", "Tan");
			m_FormulaMapping.Add("Today", "Today");
			m_FormulaMapping.Add("Year", "Year");
			m_VBModulePropertiesSupported = new ArrayList();
			m_VBModulePropertiesSupported.Add("Today");
			m_VBModulePropertiesSupported.Add("Now");
			m_VBModulePropertiesUnSupported = new ArrayList();
			m_VBModulePropertiesUnSupported.Add("DateString");
			m_VBModulePropertiesUnSupported.Add("TimeOfDay");
			m_VBModulePropertiesUnSupported.Add("Timer");
			m_VBModulePropertiesUnSupported.Add("TimeString");
			m_VBModulePropertiesUnSupported.Add("Hex");
			m_VBModulePropertiesUnSupported.Add("Oct");
		}

		internal FormulaHandler()
		{
			m_reportItemCount = 0;
		}

		internal ArrayList ProcessFormula(string formulaExpression, out string excelFormula)
		{
			m_reportItemsNames = new ArrayList();
			if (formulaExpression == null || formulaExpression.Length == 0)
			{
				excelFormula = null;
				m_reportItemsNames = null;
				return m_reportItemsNames;
			}
			string text = CheckValidityforConversion(formulaExpression);
			if (text == null || text.Length == 0)
			{
				excelFormula = null;
				m_reportItemsNames = null;
				return null;
			}
			m_reportItemCount = 0;
			excelFormula = Regex.Replace(text, m_RegexReportItemName.ToString(), MapToExcel, m_regexOptions);
			return m_reportItemsNames;
		}

		internal static string ProcessHeaderFooterFormula(string formulaExpression)
		{
			if (formulaExpression == null)
			{
				return null;
			}
			string text = null;
			StringBuilder stringBuilder = new StringBuilder();
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
						text = ExcelHeaderFooterFormula(text);
						if (text == null || text == string.Empty)
						{
							return null;
						}
						stringBuilder.Append(text);
						text = null;
					}
					EncodeHeaderFooterString(stringBuilder, text2);
				}
			}
			if (text != null)
			{
				text = ExcelHeaderFooterFormula(text);
				if (text == null || text == string.Empty)
				{
					return null;
				}
				stringBuilder.Append(text);
			}
			return stringBuilder.ToString();
		}

		internal static void EncodeHeaderFooterString(StringBuilder output, string input)
		{
			if (output == null || input == null)
			{
				return;
			}
			int i = output.Length;
			if (output.Length > 0 && char.IsDigit(output[i - 1]))
			{
				output.Append(' ');
			}
			output.Append(input);
			for (; i < output.Length; i++)
			{
				if (output[i] == '\\' || output[i] == '"')
				{
					output.Insert(i, '\\');
					i++;
				}
				else if (output[i] == '&')
				{
					output.Insert(i, '&');
					i++;
				}
			}
		}

		internal static string ExcelHeaderFooterFormula(string formulaExpression)
		{
			string result = string.Empty;
			if (formulaExpression != null && formulaExpression.Length != 0)
			{
				Match match = m_RegexGlobalOnly.Match(formulaExpression);
				if (match.Success)
				{
					string text = match.Result("${GlobalParameterName}");
					text = text.Replace("\"", string.Empty);
					if (text != null && text.Length != 0)
					{
						switch (text.Trim().ToUpperInvariant())
						{
						case "PAGENUMBER":
						case "OVERALLPAGENUMBER":
							result = "&P";
							break;
						case "TOTALPAGES":
						case "OVERALLTOTALPAGES":
							result = "&N";
							break;
						case "REPORTNAME":
							result = "&F";
							break;
						default:
							result = string.Empty;
							break;
						}
					}
				}
			}
			return result;
		}

		private string CheckValidityforConversion(string formulaExpression)
		{
			if (Detected(m_RegexFieldDetection, formulaExpression) || Detected(m_RegexGlobalsDetection, formulaExpression) || Detected(m_RegexNothingOnly, formulaExpression) || Detected(m_RegexParametersDetection, formulaExpression) || Detected(m_RegexAggregatesDetection, formulaExpression) || Detected(m_RegexUserDetection, formulaExpression) || Detected(m_RegexSpecialFunction, formulaExpression))
			{
				return null;
			}
			foreach (string item in m_VBModulePropertiesUnSupported)
			{
				Regex detectionRegex = new Regex("(?<FunctionName>" + item + ")(\\s*\\(\\s*\\))?", m_regexOptions);
				if (Detected(detectionRegex, formulaExpression))
				{
					return null;
				}
			}
			string transformedExpression = FormatReportItemReference(formulaExpression);
			if (ValidateFunctionNames(ref transformedExpression))
			{
				foreach (string item2 in m_VBModulePropertiesSupported)
				{
					Regex regex = new Regex("(?<FunctionName>" + item2 + ")(\\s*\\(\\s*\\))?", m_regexOptions);
					transformedExpression = Regex.Replace(transformedExpression, regex.ToString(), MapVbModuleProperty, m_regexOptions);
				}
				return transformedExpression;
			}
			return null;
		}

		private bool ValidateFunctionNames(ref string transformedExpression)
		{
			string text = transformedExpression;
			int num = 0;
			ArrayList arrayList = new ArrayList();
			while (text.Length > 0)
			{
				Match match = m_RegexFunctionName.Match(text, 0);
				if (!match.Success)
				{
					return true;
				}
				string text2 = match.Result("${FunctionName}");
				if (text2 == null || text2.Length == 0)
				{
					num = match.Index + match.Length;
					text = text.Substring(num);
					continue;
				}
				if (!m_FormulaMapping.ContainsKey(text2))
				{
					return false;
				}
				if (arrayList.IndexOf(text2) == -1)
				{
					Regex regex = new Regex("(?<FunctionName>" + text2 + ")\\s*\\(", m_regexOptions);
					transformedExpression = Regex.Replace(transformedExpression, regex.ToString(), MapFunction, m_regexOptions);
					arrayList.Add(text2);
				}
				num = match.Index + match.Length;
				text = text.Substring(num);
			}
			return true;
		}

		private string MapVbModuleProperty(Match match)
		{
			string text = match.Result("${FunctionName}");
			if (text != null && text.Length != 0)
			{
				return text + "()";
			}
			return match.ToString();
		}

		private string MapFunction(Match match)
		{
			_ = string.Empty;
			string text = match.Result("${FunctionName}");
			if (text != null && text.Length != 0)
			{
				return Regex.Replace(Regex.Replace(match.ToString(), m_RegexIdentifier.ToString(), m_FormulaMapping[text].ToString(), m_regexOptions), "\\s+", "");
			}
			return match.ToString();
		}

		private string FormatReportItemReference(string formulaExpression)
		{
			return Regex.Replace(formulaExpression, m_RegexReportItemNameFormat.ToString(), MapReportItemName, m_regexOptions);
		}

		private string MapReportItemName(Match match)
		{
			_ = string.Empty;
			string text = match.Result("${ReportItemname}");
			if (text != null && text.Length != 0)
			{
				text = text.Replace("\"", string.Empty);
				return "ReportItems!" + text;
			}
			return match.ToString();
		}

		private string MapToExcel(Match match)
		{
			string result = string.Empty;
			string text = match.Result("${reportitemname}");
			if (text != null && text.Length != 0)
			{
				if (m_reportItemsNames.Contains(text))
				{
					result = "{" + m_reportItemsNames.IndexOf(text) + "}";
				}
				else
				{
					m_reportItemsNames.Add(text);
					result = "{" + m_reportItemCount + "}";
					m_reportItemCount++;
				}
			}
			return result;
		}

		private bool Detected(Regex detectionRegex, string formulaExpression)
		{
			if (detectionRegex.Match(formulaExpression).Success)
			{
				return true;
			}
			return false;
		}
	}
}

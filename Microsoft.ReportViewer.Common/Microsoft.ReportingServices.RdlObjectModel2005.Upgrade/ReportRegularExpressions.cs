using System.Text.RegularExpressions;

namespace Microsoft.ReportingServices.RdlObjectModel2005.Upgrade
{
	internal sealed class ReportRegularExpressions
	{
		internal Regex Whitespace;

		internal Regex NonConstant;

		internal Regex FieldDetection;

		internal Regex ReportItemsDetection;

		internal Regex ParametersDetection;

		internal Regex PageGlobalsDetection;

		internal Regex AggregatesDetection;

		internal Regex UserDetection;

		internal Regex DataSetsDetection;

		internal Regex DataSourcesDetection;

		internal Regex VariablesDetection;

		internal Regex MeDotValueExpression;

		internal Regex MeDotValueDetection;

		internal Regex IllegalCharacterDetection;

		internal Regex LineTerminatorDetection;

		internal Regex FieldOnly;

		internal Regex ParameterOnly;

		internal Regex StringLiteralOnly;

		internal Regex NothingOnly;

		internal Regex ReportItemName;

		internal Regex FieldName;

		internal Regex ParameterName;

		internal Regex DataSetName;

		internal Regex DataSourceName;

		internal Regex SpecialFunction;

		internal Regex PSAFunction;

		internal Regex Arguments;

		internal Regex DynamicFieldReference;

		internal Regex DynamicFieldPropertyReference;

		internal Regex StaticFieldPropertyReference;

		internal Regex RewrittenCommandText;

		internal Regex SimpleDynamicFieldReference;

		internal Regex SimpleDynamicReportItemReference;

		internal Regex SimpleDynamicVariableReference;

		internal Regex ReportItemValueReference;

		internal Regex VariableName;

		internal Regex ClsIdentifierRegex;

		private const string m_identifierStart = "\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}";

		private const string m_identifierExtend = "\\p{Mn}\\p{Mc}\\p{Nd}\\p{Pc}\\p{Cf}";

		internal const string ClsReplacerPattern = "[^\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Mn}\\p{Mc}\\p{Nd}\\p{Pc}\\p{Cf}]";

		internal static readonly ReportRegularExpressions Value = new ReportRegularExpressions();

		private ReportRegularExpressions()
		{
			RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline;
			NonConstant = new Regex("^\\s*=", options);
			string text = Regex.Escape("-+()#,:&*/\\^<=>");
			string text2 = Regex.Escape("!");
			string text3 = Regex.Escape(".");
			string text4 = "[" + text2 + text3 + "]";
			string text5 = "(^|[" + text + "\\s])";
			string text6 = "($|[" + text + text2 + text3 + "\\s])";
			Whitespace = new Regex("\\s+", options);
			FieldDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>Fields)" + text6, options);
			ReportItemsDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>ReportItems)" + text6, options);
			ParametersDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>Parameters)" + text6, options);
			PageGlobalsDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>(Globals" + text4 + "PageNumber)|(Globals" + text4 + "TotalPages))" + text6, options);
			AggregatesDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>Aggregates)" + text6, options);
			UserDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>User)" + text6, options);
			DataSetsDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>DataSets)" + text6, options);
			DataSourcesDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>DataSources)" + text6, options);
			VariablesDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>Variables)" + text6, options);
			MeDotValueDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>(?:Me.)?Value)" + text6, options);
			MeDotValueExpression = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<medotvalue>(Me" + text3 + ")?Value)*" + text6, options);
			string text7 = Regex.Escape(":");
			string text8 = Regex.Escape("#");
			string text9 = "(" + text8 + "[^" + text8 + "]*" + text8 + ")";
			string text10 = Regex.Escape(":=");
			LineTerminatorDetection = new Regex("(?<detected>(\\u000D\\u000A)|([\\u000D\\u000A\\u2028\\u2029]))", options);
			IllegalCharacterDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text9 + "|" + text10 + "|(?<detected>" + text7 + ")", options);
			string text11 = "[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Cf}]*";
			string text12 = "ReportItems" + text2 + "(?<reportitemname>" + text11 + ")";
			string text13 = "Fields" + text2 + "(?<fieldname>" + text11 + ")";
			string text14 = "Parameters" + text2 + "(?<parametername>" + text11 + ")";
			string text15 = "DataSets" + text2 + "(?<datasetname>" + text11 + ")";
			string str = "DataSources" + text2 + "(?<datasourcename>" + text11 + ")";
			string str2 = "Variables" + text2 + "(?<variablename>" + text11 + ")";
			string text16 = "(?<detected>(ReportItems(" + text3 + "Item)?" + Regex.Escape("(") + "[ \t]*" + Regex.Escape("\"") + "(?<reportitemname>" + text11 + ")" + Regex.Escape("\"") + "[ \t]*" + Regex.Escape(")") + "))";
			SimpleDynamicReportItemReference = new Regex(text5 + text16, options);
			SimpleDynamicVariableReference = new Regex(text5 + "(?<detected>(Variables(" + text3 + "Item)?" + Regex.Escape("(") + "[ \t]*" + Regex.Escape("\"") + "(?<variablename>" + text11 + ")" + Regex.Escape("\"") + "[ \t]*" + Regex.Escape(")") + "))", options);
			SimpleDynamicFieldReference = new Regex(text5 + "(?<detected>(Fields(" + text3 + "Item)?" + Regex.Escape("(") + "[ \t]*" + Regex.Escape("\"") + "(?<fieldname>" + text11 + ")" + Regex.Escape("\"") + "[ \t]*" + Regex.Escape(")") + "))", options);
			DynamicFieldReference = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>(Fields(" + text3 + "Item)?" + Regex.Escape("(") + "))", options);
			DynamicFieldPropertyReference = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + text13 + Regex.Escape("("), options);
			StaticFieldPropertyReference = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + text13 + text3 + "(?<propertyname>" + text11 + ")", options);
			FieldOnly = new Regex("^\\s*" + text13 + text3 + "Value\\s*$", options);
			RewrittenCommandText = new Regex("^\\s*" + text15 + text3 + "RewrittenCommandText\\s*$", options);
			ParameterOnly = new Regex("^\\s*" + text14 + text3 + "Value\\s*$", options);
			StringLiteralOnly = new Regex("^\\s*\"(?<string>((\"\")|[^\"])*)\"\\s*$", options);
			NothingOnly = new Regex("^\\s*Nothing\\s*$", options);
			ReportItemName = new Regex(text5 + text12, options);
			FieldName = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + text13, options);
			ParameterName = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + text14, options);
			DataSetName = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + text15, options);
			DataSourceName = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + str, options);
			VariableName = new Regex(text5 + str2, options);
			SpecialFunction = new Regex("(\"((\"\")|[^\"])*\")|(?<prefix>" + text5 + ")(?<sfname>RunningValue|RowNumber|First|Last|Previous|Sum|Avg|Max|Min|CountDistinct|Count|CountRows|StDevP|VarP|StDev|Var|Aggregate)\\s*\\(", options);
			string text17 = Regex.Escape("(");
			string text18 = Regex.Escape(")");
			PSAFunction = new Regex("(\"((\"\")|[^\"])*\")|(?<prefix>" + text5 + ")(?<psaname>RunningValue|First|Last|Previous)\\s*\\(", options);
			string text19 = Regex.Escape(",");
			Arguments = new Regex("(\"((\"\")|[^\"])*\")|(?<openParen>" + text17 + ")|(?<closeParen>" + text18 + ")|(?<comma>" + text19 + ")", options);
			ReportItemValueReference = new Regex("((" + text12 + ")|" + text16 + ")" + text3 + "Value");
			ClsIdentifierRegex = new Regex("^[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Mn}\\p{Mc}\\p{Nd}\\p{Pc}\\p{Cf}]*$", options);
		}
	}
}

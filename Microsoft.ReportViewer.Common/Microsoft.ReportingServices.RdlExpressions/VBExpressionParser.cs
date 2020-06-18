using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.ReportingServices.RdlExpressions
{
	internal sealed class VBExpressionParser : ExpressionParser
	{
		private sealed class ParserState
		{
			internal bool BodyRefersToReportItems
			{
				get;
				set;
			}

			internal bool PageSectionRefersToReportItems
			{
				get;
				set;
			}

			internal bool PageSectionRefersToOverallTotalPages
			{
				get;
				set;
			}

			internal bool PageSectionRefersToTotalPages
			{
				get;
				set;
			}

			internal int NumberOfAggregates
			{
				get;
				set;
			}

			internal int NumberOfRunningValues
			{
				get;
				set;
			}

			internal int NumberOfLookups
			{
				get;
				set;
			}

			internal int LastID => NumberOfAggregates + NumberOfRunningValues;

			internal int LastLookupID => NumberOfLookups;

			internal bool PreviousAggregateUsed
			{
				get;
				set;
			}

			internal bool AggregateOfAggregatesUsed
			{
				get;
				set;
			}

			internal bool AggregateOfAggregatesUsedInUserSort
			{
				get;
				set;
			}

			internal ParserState Save()
			{
				return (ParserState)MemberwiseClone();
			}
		}

		private sealed class ReportRegularExpressions
		{
			internal Regex NonConstant;

			internal Regex FieldDetection;

			internal Regex ReportItemsDetection;

			internal Regex ParametersDetection;

			internal Regex RenderFormatAnyDetection;

			internal Regex OverallPageGlobalsDetection;

			internal Regex PageGlobalsDetection;

			internal Regex OverallTotalPagesDetection;

			internal Regex TotalPagesDetection;

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

			internal Regex InScopeOrLevel;

			internal Regex InScope;

			internal Regex Level;

			internal Regex CreateDrillthroughContext;

			internal Regex ReportItemName;

			internal Regex FieldName;

			internal Regex ParameterName;

			internal Regex DynamicParameterReference;

			internal Regex DataSetName;

			internal Regex DataSourceName;

			internal Regex SpecialFunction;

			internal Regex Arguments;

			internal Regex DynamicFieldReference;

			internal Regex DynamicFieldPropertyReference;

			internal Regex StaticFieldPropertyReference;

			internal Regex RewrittenCommandText;

			internal Regex SimpleDynamicFieldReference;

			internal Regex SimpleDynamicReportItemReference;

			internal Regex SimpleDynamicVariableReference;

			internal Regex VariableName;

			internal Regex RenderFormatPropertyName;

			internal Regex HasLevelWithNoScope;

			internal Regex RdlFunction;

			internal Regex ScopedFieldReferenceOnly;

			internal Regex ScopesDetection;

			internal Regex SimpleDynamicScopeReference;

			internal Regex ScopeName;

			internal Regex DictionaryOpWithIdentifier;

			internal Regex IndexerWithIdentifier;

			internal static readonly ReportRegularExpressions Value = new ReportRegularExpressions();

			private ReportRegularExpressions()
			{
				RegexOptions options = RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline;
				NonConstant = new Regex("^\\s*=", options);
				string text = Regex.Escape("-+()#,:&*/\\^<=>");
				string text2 = Regex.Escape("!");
				string text3 = Regex.Escape(".");
				string text4 = "[" + text2 + text3 + "]";
				string text5 = "[" + text + "\\s]";
				string text6 = "(^|" + text5 + ")";
				string text7 = "($|[" + text + text2 + text3 + "\\s])";
				string text8 = "($|[" + text + text3 + "\\s])";
				string text9 = CaseInsensitive("Fields");
				string text10 = CaseInsensitive("Value");
				string text11 = CaseInsensitive("Scopes");
				string text12 = CaseInsensitive("ReportItems");
				string text13 = CaseInsensitive("Parameters");
				string text14 = CaseInsensitive("Globals");
				string text15 = CaseInsensitive("RenderFormat");
				string text16 = CaseInsensitive("OverallTotalPages");
				string text17 = CaseInsensitive("TotalPages");
				string text18 = CaseInsensitive("DataSets");
				string text19 = CaseInsensitive("DataSources");
				string text20 = CaseInsensitive("Variables");
				string text21 = CaseInsensitive("Me");
				string text22 = CaseInsensitive("Item");
				string text23 = CaseInsensitive("InScope");
				string text24 = CaseInsensitive("Level");
				FieldDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>" + text9 + ")" + text7, options);
				ScopesDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>" + text11 + ")" + text7, options);
				ReportItemsDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>" + text12 + ")" + text7, options);
				ParametersDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>" + text13 + ")" + text7, options);
				RenderFormatAnyDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>(" + text14 + text4 + text15 + text7 + "))", options);
				OverallPageGlobalsDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>(" + text14 + text4 + CaseInsensitive("OverallPageNumber") + ")|(" + text14 + text4 + text16 + "))" + text7, options);
				PageGlobalsDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>(" + text14 + text4 + CaseInsensitive("PageNumber") + ")|(" + text14 + text4 + text17 + ")|(" + text14 + text4 + CaseInsensitive("PageName") + "))" + text7, options);
				OverallTotalPagesDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>(" + text14 + text4 + text16 + text7 + ")|(" + text14 + text5 + "))", options);
				TotalPagesDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>(" + text14 + text4 + text17 + text7 + ")|(" + text14 + text5 + "))", options);
				AggregatesDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>" + CaseInsensitive("Aggregates") + ")" + text7, options);
				UserDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>" + CaseInsensitive("User") + ")" + text7, options);
				DataSetsDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>" + text18 + ")" + text7, options);
				DataSourcesDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>" + text19 + ")" + text7, options);
				VariablesDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>" + text20 + ")" + text7, options);
				MeDotValueDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>(" + text21 + text3 + ")?" + text10 + ")" + text7, options);
				MeDotValueExpression = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<medotvalue>(" + text21 + text3 + ")?" + text10 + ")*" + text7, options);
				string text25 = Regex.Escape(":");
				string text26 = Regex.Escape("#");
				string text27 = "(" + text26 + "[^" + text26 + "]*" + text26 + ")";
				string text28 = Regex.Escape(":=");
				LineTerminatorDetection = new Regex("(?<detected>(\\u000D\\u000A)|([\\u000D\\u000A\\u2028\\u2029]))", options);
				IllegalCharacterDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text27 + "|" + text28 + "|(?<detected>" + text25 + ")", options);
				string text29 = "[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Cf}]*";
				string str = text12 + text2 + "(?<reportitemname>" + text29 + ")";
				string text30 = text9 + text2 + "(?<fieldname>" + text29 + ")";
				string text31 = text13 + text2 + "(?<parametername>" + text29 + ")";
				string text32 = text18 + text2 + "(?<datasetname>" + text29 + ")";
				string str2 = text19 + text2 + "(?<datasourcename>" + text29 + ")";
				string str3 = text20 + text2 + "(?<variablename>" + text29 + ")";
				string text33 = text11 + text2 + "(?<scopename>" + text29 + ")";
				string str4 = text14 + text4 + text15 + text4 + "(?<propertyname>" + text29 + ")";
				SimpleDynamicReportItemReference = new Regex(text6 + "(?<detected>(" + text12 + "(" + text3 + text22 + ")?" + Regex.Escape("(") + "[ \t]*" + Regex.Escape("\"") + "(?<reportitemname>" + text29 + ")" + Regex.Escape("\"") + "[ \t]*" + Regex.Escape(")") + "))", options);
				SimpleDynamicVariableReference = new Regex(text6 + "(?<detected>(" + text20 + "(" + text3 + text22 + ")?" + Regex.Escape("(") + "[ \t]*" + Regex.Escape("\"") + "(?<variablename>" + text29 + ")" + Regex.Escape("\"") + "[ \t]*" + Regex.Escape(")") + "))", options);
				SimpleDynamicFieldReference = new Regex(text6 + "(?<detected>(" + text9 + "(" + text3 + text22 + ")?" + Regex.Escape("(") + "[ \t]*" + Regex.Escape("\"") + "(?<fieldname>" + text29 + ")" + Regex.Escape("\"") + "[ \t]*" + Regex.Escape(")") + "))", options);
				DynamicFieldReference = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>(" + text9 + "(" + text3 + text22 + ")?" + Regex.Escape("(") + "))", options);
				DynamicFieldPropertyReference = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + text30 + Regex.Escape("("), options);
				StaticFieldPropertyReference = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + text30 + text3 + "(?<propertyname>" + text29 + ")", options);
				FieldOnly = new Regex("^\\s*" + text30 + text3 + text10 + "\\s*$", options);
				string text34 = "(?<hasfields>" + text3 + text9 + ")?";
				SimpleDynamicScopeReference = new Regex(text6 + "(?<detected>(" + text11 + "(" + text3 + text22 + ")?" + Regex.Escape("(") + "[ \t]*" + Regex.Escape("\"") + "(?<scopename>" + text29 + ")" + Regex.Escape("\"") + "[ \t]*" + Regex.Escape(")") + ")" + text34 + ")", options);
				ScopeName = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + text33 + text34, options);
				string text35 = "(?<fieldname>" + text29 + ")";
				DictionaryOpWithIdentifier = new Regex("\\G" + text2 + text35, options);
				IndexerWithIdentifier = new Regex("\\G(" + text3 + text22 + ")?" + Regex.Escape("(") + "[ \t]*" + Regex.Escape("\"") + text35 + Regex.Escape("\"") + "[ \t]*" + Regex.Escape(")"), options);
				ScopedFieldReferenceOnly = new Regex("^\\s*" + text33 + text3 + text30 + text3 + text10 + "\\s*$", options);
				RewrittenCommandText = new Regex("^\\s*" + text32 + text3 + CaseInsensitive("RewrittenCommandText") + "\\s*$", options);
				ParameterOnly = new Regex("^\\s*" + text31 + text3 + text10 + "\\s*$", options);
				StringLiteralOnly = new Regex("^\\s*\"(?<string>((\"\")|[^\"])*)\"\\s*$", options);
				NothingOnly = new Regex("^\\s*" + CaseInsensitive("Nothing") + "\\s*$", options);
				InScopeOrLevel = new Regex("((\"((\"\")|[^\"])*\")|('.*)|" + text6 + ")*(" + text23 + "|" + text24 + ")\\s*\\(", options);
				string str5 = "(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>";
				string str6 = ")\\s*\\(";
				InScope = new Regex(str5 + text23 + str6, options);
				Level = new Regex(str5 + text24 + str6, options);
				CreateDrillthroughContext = new Regex(str5 + CaseInsensitive("CreateDrillthroughContext") + str6, options);
				ReportItemName = new Regex(text6 + str, options);
				FieldName = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + text30, options);
				ParameterName = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + text31, options);
				DynamicParameterReference = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>" + text13 + text8 + ")", options);
				DataSetName = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + text32, options);
				DataSourceName = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + str2, options);
				VariableName = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + str3, options);
				RenderFormatPropertyName = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + str4, options);
				SpecialFunction = new Regex("(\"((\"\")|[^\"])*\")|('.*)|(?<prefix>" + text6 + ")(?<sfname>" + CaseInsensitive("RunningValue") + "|" + CaseInsensitive("RowNumber") + "|" + CaseInsensitive("Lookup") + "|" + CaseInsensitive("LookupSet") + "|" + CaseInsensitive("MultiLookup") + "|" + CaseInsensitive("First") + "|" + CaseInsensitive("Last") + "|" + CaseInsensitive("Previous") + "|" + CaseInsensitive("Sum") + "|" + CaseInsensitive("Avg") + "|" + CaseInsensitive("Max") + "|" + CaseInsensitive("Min") + "|" + CaseInsensitive("CountDistinct") + "|" + CaseInsensitive("Count") + "|" + CaseInsensitive("CountRows") + "|" + CaseInsensitive("StDevP") + "|" + CaseInsensitive("VarP") + "|" + CaseInsensitive("StDev") + "|" + CaseInsensitive("Var") + "|" + CaseInsensitive("Aggregate") + "|" + CaseInsensitive("Union") + ")\\s*\\(", options);
				RdlFunction = new Regex("^\\s*(?<functionName>" + CaseInsensitive("MinValue") + "|" + CaseInsensitive("MaxValue") + ")\\s*\\(", options);
				string text36 = Regex.Escape("(");
				string text37 = Regex.Escape(")");
				string text38 = Regex.Escape(",");
				string text39 = Regex.Escape("{");
				string text40 = Regex.Escape("}");
				Arguments = new Regex("(\"((\"\")|[^\"])*\")|('.*)|(?<openParen>" + text36 + ")|(?<closeParen>" + text37 + ")|(?<openCurly>" + text39 + ")|(?<closeCurly>" + text40 + ")|(?<comma>" + text38 + ")", options);
				HasLevelWithNoScope = new Regex(text24 + "\\s*" + text36 + "\\s*" + text37);
			}

			private static string CaseInsensitive(string input)
			{
				StringBuilder stringBuilder = new StringBuilder(input.Length * 4);
				foreach (char c in input)
				{
					stringBuilder.Append("[");
					stringBuilder.Append(char.ToUpperInvariant(c));
					stringBuilder.Append(char.ToLowerInvariant(c));
					stringBuilder.Append("]");
				}
				return stringBuilder.ToString();
			}
		}

		private const string RunningValue = "RunningValue";

		private const string RowNumber = "RowNumber";

		private const string Previous = "Previous";

		private const string Lookup = "Lookup";

		private const string LookupSet = "LookupSet";

		private const string MultiLookup = "MultiLookup";

		private const string Fields = "Fields";

		private const string ReportItems = "ReportItems";

		private const string Parameters = "Parameters";

		private const string Globals = "Globals";

		private const string User = "User";

		private const string Aggregates = "Aggregates";

		private const string DataSets = "DataSets";

		private const string DataSources = "DataSources";

		private const string Variables = "Variables";

		private const string MinValue = "MinValue";

		private const string MaxValue = "MaxValue";

		private const string Scopes = "Scopes";

		private const string Star = "*";

		private ReportRegularExpressions m_regexes;

		private ExpressionContext m_context;

		private ParserState m_state = new ParserState();

		internal override bool BodyRefersToReportItems => m_state.BodyRefersToReportItems;

		internal override bool PageSectionRefersToReportItems => m_state.PageSectionRefersToReportItems;

		internal override bool PageSectionRefersToOverallTotalPages => m_state.PageSectionRefersToOverallTotalPages;

		internal override bool PageSectionRefersToTotalPages => m_state.PageSectionRefersToTotalPages;

		internal override int NumberOfAggregates => m_state.NumberOfAggregates;

		internal override int LastID => m_state.LastID;

		internal override int LastLookupID => m_state.LastLookupID;

		internal override bool PreviousAggregateUsed => m_state.PreviousAggregateUsed;

		internal override bool AggregateOfAggregatesUsed => m_state.AggregateOfAggregatesUsed;

		internal override bool AggregateOfAggregatesUsedInUserSort => m_state.AggregateOfAggregatesUsedInUserSort;

		internal VBExpressionParser(ErrorContext errorContext)
			: base(errorContext)
		{
			m_regexes = ReportRegularExpressions.Value;
		}

		internal override CodeDomProvider GetCodeCompiler()
		{
			return new VBExpressionCodeProvider();
		}

		internal override string GetCompilerArguments()
		{
			return "/optimize+";
		}

		internal override Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ParseExpression(string expression, ExpressionContext context, EvaluationMode evaluationMode)
		{
			Global.Tracer.Assert(expression != null, "(null != expression)");
			string vbExpression;
			return Lex(expression, context, evaluationMode, out vbExpression);
		}

		internal override Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ParseExpression(string expression, ExpressionContext context, EvaluationMode evaluationMode, out bool userCollectionReferenced)
		{
			string vbExpression;
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = Lex(expression, context, evaluationMode, out vbExpression);
			userCollectionReferenced = false;
			if (expressionInfo.Type == Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression)
			{
				userCollectionReferenced = DetectUserReference(vbExpression);
				expressionInfo.SimpleParameterName = GetSimpleParameterReferenceName(vbExpression);
			}
			return expressionInfo;
		}

		internal override void ConvertField2ComplexExpr(ref Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo info)
		{
			Global.Tracer.Assert(info.Type == Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Field, "(info.Type == ExpressionInfo.Types.Field)");
			info.Type = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression;
			info.TransformedExpression = "Fields!" + info.StringValue + ".Value";
		}

		internal override void ResetPageSectionRefersFlags()
		{
			m_state.PageSectionRefersToReportItems = false;
			m_state.PageSectionRefersToOverallTotalPages = false;
			m_state.PageSectionRefersToTotalPages = false;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo Lex(string expression, ExpressionContext context, EvaluationMode evaluationMode, out string vbExpression)
		{
			vbExpression = null;
			m_context = context;
			if (context.MaxExpressionLength != -1 && expression != null && expression.Length > context.MaxExpressionLength)
			{
				m_errorContext.Register(ProcessingErrorCode.rsSandboxingExpressionExceedsMaximumLength, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, Convert.ToString(context.MaxExpressionLength, CultureInfo.InvariantCulture));
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = new Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo();
			expressionInfo.OriginalText = expression;
			bool flag = evaluationMode == EvaluationMode.Constant;
			if (!flag)
			{
				Match match = m_regexes.NonConstant.Match(expression);
				if (match.Success)
				{
					vbExpression = expression.Substring(match.Length);
				}
				else
				{
					flag = true;
				}
			}
			if (flag)
			{
				ExpressionParser.ParseRDLConstant(expression, expressionInfo, context.ConstantType, m_errorContext, context.ObjectType, context.ObjectName, context.PropertyName);
			}
			else
			{
				GrammarFlags grammarFlags = (GrammarFlags)(((m_context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) == 0) ? (ExpressionParser.ExpressionType2Restrictions(m_context.ExpressionType) | Restrictions.InBody) : (ExpressionParser.ExpressionType2Restrictions(m_context.ExpressionType) | Restrictions.InPageSection));
				if (m_context.ObjectType == ObjectType.Paragraph)
				{
					grammarFlags |= GrammarFlags.DenyMeDotValue;
				}
				if (m_regexes.HasLevelWithNoScope.Match(expression).Success)
				{
					expressionInfo.NullLevelDetected = true;
				}
				VBLex(vbExpression, isParameter: false, grammarFlags, expressionInfo);
			}
			if (m_state.AggregateOfAggregatesUsed && context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.AggregatesOfAggregates))
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlExpressionAggregatesOfAggregates, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
			}
			return expressionInfo;
		}

		private string GetSimpleParameterReferenceName(string expression)
		{
			string result = null;
			Match match = m_regexes.ParameterOnly.Match(expression);
			if (match.Success)
			{
				result = match.Result("${parametername}");
			}
			return result;
		}

		private bool DetectUserReference(string expression)
		{
			return Detected(expression, m_regexes.UserDetection);
		}

		private void VBLex(string expression, bool isParameter, GrammarFlags grammarFlags, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			ParserState state = m_state.Save();
			if (!TryVBLex(expression, isParameter, grammarFlags, expressionInfo, parseSpecialFunctions: true))
			{
				m_state = state;
				TryVBLex(expression, isParameter, grammarFlags, expressionInfo, parseSpecialFunctions: false);
			}
			if (expressionInfo.Type == Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression && m_context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.ComplexExpression))
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidComplexExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
			}
		}

		private bool TryVBLex(string expression, bool isParameter, GrammarFlags grammarFlags, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, bool parseSpecialFunctions)
		{
			if ((grammarFlags & GrammarFlags.DenyFields) == 0)
			{
				Match match = m_regexes.FieldOnly.Match(expression);
				if (match.Success)
				{
					string asSimpleFieldReference = match.Result("${fieldname}");
					expressionInfo.SetAsSimpleFieldReference(asSimpleFieldReference);
					return true;
				}
			}
			if ((grammarFlags & GrammarFlags.DenyDataSets) == 0)
			{
				Match match2 = m_regexes.RewrittenCommandText.Match(expression);
				if (match2.Success)
				{
					string text = match2.Result("${datasetname}");
					expressionInfo.AddReferencedDataSet(text);
					expressionInfo.Type = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Token;
					expressionInfo.StringValue = text;
					return true;
				}
			}
			if ((grammarFlags & GrammarFlags.DenyFields) == 0 && (grammarFlags & GrammarFlags.DenyScopes) == 0 && !m_context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.ScopesCollection))
			{
				Match match3 = m_regexes.ScopedFieldReferenceOnly.Match(expression);
				if (match3.Success)
				{
					string scopeName = match3.Result("${scopename}");
					string fieldName = match3.Result("${fieldname}");
					expressionInfo.SetAsScopedFieldReference(scopeName, fieldName);
					return true;
				}
			}
			EnforceRestrictions(ref expression, isParameter, grammarFlags, expressionInfo);
			StringBuilder stringBuilder = new StringBuilder();
			int newPos = 0;
			bool flag = false;
			while (newPos < expression.Length)
			{
				if (parseSpecialFunctions)
				{
					Match match4 = m_regexes.RdlFunction.Match(expression, newPos);
					if (match4.Success)
					{
						string text2 = match4.Result("${functionName}");
						if (string.IsNullOrEmpty(text2))
						{
							return false;
						}
						newPos = match4.Length;
						if (ParseRdlFunction(expressionInfo, newPos, text2, expression, grammarFlags, out newPos) || expression.Substring(newPos).Trim().Length > 0)
						{
							return false;
						}
						return true;
					}
					parseSpecialFunctions = false;
				}
				Match match5 = m_regexes.SpecialFunction.Match(expression, newPos);
				if (!match5.Success)
				{
					stringBuilder.Append(expression.Substring(newPos));
					newPos = expression.Length;
					continue;
				}
				stringBuilder.Append(expression.Substring(newPos, match5.Index - newPos));
				string text3 = match5.Result("${sfname}");
				if (string.IsNullOrEmpty(text3))
				{
					stringBuilder.Append(match5.Value);
					newPos = match5.Index + match5.Length;
					continue;
				}
				stringBuilder.Append(match5.Result("${prefix}"));
				newPos = match5.Index + match5.Length;
				bool isRunningValue = false;
				Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types types;
				string lookupID;
				if (AreEqualOrdinalIgnoreCase(text3, "Lookup"))
				{
					types = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_OneValue;
					LookupInfo lookup = GetLookup(newPos, text3, expression, isParameter, grammarFlags, LookupType.Lookup, out newPos, out lookupID);
					expressionInfo.AddLookup(lookup);
				}
				else if (AreEqualOrdinalIgnoreCase(text3, "LookupSet"))
				{
					types = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_MultiValue;
					LookupInfo lookup2 = GetLookup(newPos, text3, expression, isParameter, grammarFlags, LookupType.LookupSet, out newPos, out lookupID);
					expressionInfo.AddLookup(lookup2);
				}
				else if (AreEqualOrdinalIgnoreCase(text3, "MultiLookup"))
				{
					types = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_MultiValue;
					LookupInfo lookup3 = GetLookup(newPos, text3, expression, isParameter, grammarFlags, LookupType.MultiLookup, out newPos, out lookupID);
					expressionInfo.AddLookup(lookup3);
				}
				else
				{
					lookupID = CreateAggregateID();
					m_state.NumberOfAggregates++;
					types = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Aggregate;
					if (AreEqualOrdinalIgnoreCase(text3, "Previous"))
					{
						GetPreviousAggregate(newPos, text3, expression, isParameter, grammarFlags, out newPos, out Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValue);
						runningValue.Name = lookupID;
						expressionInfo.AddRunningValue(runningValue);
						isRunningValue = true;
					}
					else if (AreEqualOrdinalIgnoreCase(text3, "RunningValue"))
					{
						GetRunningValue(newPos, text3, expression, isParameter, grammarFlags, out newPos, out Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValue2);
						runningValue2.Name = lookupID;
						expressionInfo.AddRunningValue(runningValue2);
						isRunningValue = true;
					}
					else if (AreEqualOrdinalIgnoreCase(text3, "RowNumber"))
					{
						GetRowNumber(newPos, text3, expression, isParameter, grammarFlags, out newPos, out Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo rowNumber);
						rowNumber.Name = lookupID;
						expressionInfo.AddRunningValue(rowNumber);
						isRunningValue = true;
					}
					else
					{
						GetAggregate(newPos, text3, expression, isParameter, grammarFlags, out newPos, out Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregate);
						aggregate.Name = lookupID;
						expressionInfo.AddAggregate(aggregate);
						isRunningValue = false;
					}
				}
				if (!flag)
				{
					flag = true;
					string text4 = stringBuilder.ToString();
					if (text4.Trim().Length == 0 && expression.Substring(newPos).Trim().Length == 0)
					{
						expressionInfo.Type = types;
						expressionInfo.StringValue = lookupID;
						return true;
					}
				}
				switch (types)
				{
				case Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Aggregate:
					expressionInfo.AddTransformedExpressionAggregateInfo(stringBuilder.Length, lookupID, isRunningValue);
					stringBuilder.Append("Aggregates!");
					stringBuilder.Append(lookupID);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_OneValue:
				case Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_MultiValue:
					expressionInfo.AddTransformedExpressionLookupInfo(stringBuilder.Length, lookupID);
					stringBuilder.Append("Lookups!");
					stringBuilder.Append(lookupID);
					if (types == Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_OneValue)
					{
						stringBuilder.Append(".Value");
					}
					else
					{
						stringBuilder.Append(".Values");
					}
					break;
				default:
					Global.Tracer.Assert(false, "Unexpected special function type {0}", types);
					break;
				}
			}
			string text5 = stringBuilder.ToString();
			GetReferencedFieldNames(text5, expressionInfo);
			GetReferencedParameterNames(text5, expressionInfo);
			GetReferencedDataSetNames(text5, expressionInfo);
			GetReferencedDataSourceNames(text5, expressionInfo);
			GetReferencedVariableNames(text5, expressionInfo);
			GetReferencedScopesAndScopedFields(text5, expressionInfo);
			GetReferencedReportItemNames(text5, expressionInfo);
			GetReferencedReportItemNames(expressionInfo);
			GetMeDotValueReferences(text5, expressionInfo);
			GetMeDotValueReferences(expressionInfo);
			expressionInfo.Type = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression;
			expressionInfo.TransformedExpression = text5;
			if ((m_context.ObjectType == ObjectType.Textbox || m_context.ObjectType == ObjectType.TextRun) && expressionInfo.MeDotValueDetected)
			{
				SetValueReferenced();
			}
			if (expressionInfo.Type == Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression && Detected(text5, m_regexes.ScopesDetection))
			{
				m_errorContext.Register(ProcessingErrorCode.rsScopeReferenceInComplexExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
			}
			return true;
		}

		internal override Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo CreateScopedFirstAggregate(string fieldName, string scopeName)
		{
			string text = CreateAggregateID();
			m_state.NumberOfAggregates++;
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = new Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo();
			expressionInfo.SetAsSimpleFieldReference(fieldName);
			expressionInfo.OriginalText = fieldName;
			Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo dataAggregateInfo = new Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo();
			dataAggregateInfo.AggregateType = Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.First;
			dataAggregateInfo.Name = text;
			dataAggregateInfo.Expressions = new Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo[1]
			{
				expressionInfo
			};
			dataAggregateInfo.SetScope(scopeName);
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo2 = new Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo();
			expressionInfo2.AddAggregate(dataAggregateInfo);
			expressionInfo2.HasAnyFieldReferences = true;
			expressionInfo2.Type = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Aggregate;
			expressionInfo2.StringValue = text;
			return expressionInfo2;
		}

		private bool AreEqualOrdinalIgnoreCase(string str1, string str2)
		{
			return string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
		}

		private void EnforceRestrictions(ref string expression, bool isParameter, GrammarFlags grammarFlags, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			if ((grammarFlags & GrammarFlags.DenyRenderFormatAll) != 0 && Detected(expression, m_regexes.RenderFormatAnyDetection))
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidRenderFormatUsage, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
			}
			if (m_context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.InScope) && Detected(expression, m_regexes.InScope))
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, "InScope");
			}
			if (m_context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.Level) && Detected(expression, m_regexes.Level))
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, "Level");
			}
			if (m_context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.CreateDrillthroughContext) && Detected(expression, m_regexes.CreateDrillthroughContext))
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, "CreateDrillthroughContext");
			}
			bool flag = Detected(expression, m_regexes.ScopesDetection);
			if (flag && m_context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.ScopesCollection))
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, "Scopes");
			}
			if (flag && (grammarFlags & GrammarFlags.DenyScopes) != 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidScopeCollectionReference, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
			}
			if ((grammarFlags & GrammarFlags.DenyFields) != 0 && Detected(expression, m_regexes.FieldDetection))
			{
				if ((m_context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
				{
					m_errorContext.Register(ProcessingErrorCode.rsFieldInPageSectionExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.QueryParameter == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsFieldInQueryParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, m_context.DataSetName);
				}
				else if (ExpressionType.ReportLanguage == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsFieldInReportLanguageExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(ExpressionType.ReportParameter == m_context.ExpressionType, "(ExpressionType.ReportParameter == m_context.ExpressionType)");
					m_errorContext.Register(ProcessingErrorCode.rsFieldInReportParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
			if ((grammarFlags & GrammarFlags.DenyVariables) != 0 && Detected(expression, m_regexes.VariablesDetection))
			{
				if (m_context.InPrevious)
				{
					m_errorContext.Register(ProcessingErrorCode.rsVariableInPreviousAggregate, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (isParameter)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateofVariable, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (m_context.InLookup)
				{
					m_errorContext.Register(ProcessingErrorCode.rsLookupOfVariable, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.QueryParameter == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsVariableInQueryParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, m_context.DataSetName);
				}
				else if (ExpressionType.ReportParameter == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsVariableInReportParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.ReportLanguage == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsVariableInReportLanguageExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.GroupExpression == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsVariableInGroupExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.DataRegionSortExpression == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsVariableInDataRowSortExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.DataRegionFilters == m_context.ExpressionType || ExpressionType.DataSetFilters == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsVariableInDataRegionOrDataSetFilterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.JoinExpression == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsVariableInJoinExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(ExpressionType.FieldValue == m_context.ExpressionType, "(ExpressionType.FieldValue == m_context.ExpressionType)");
					m_errorContext.Register(ProcessingErrorCode.rsVariableInCalculatedFieldExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
			int num = NumberOfTimesDetected(expression, m_regexes.ReportItemsDetection);
			if ((grammarFlags & GrammarFlags.DenyReportItems) != 0 && 0 < num)
			{
				if (isParameter)
				{
					Global.Tracer.Assert((m_context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) == 0, "(0 == (m_context.Location & LocationFlags.InPageSection))");
					m_errorContext.Register(ProcessingErrorCode.rsAggregateReportItemInBody, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.DataSetFilters == m_context.ExpressionType || ExpressionType.DataRegionFilters == m_context.ExpressionType || ExpressionType.GroupingFilters == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsReportItemInFilterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.GroupExpression == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsReportItemInGroupExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.QueryParameter == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsReportItemInQueryParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, m_context.DataSetName);
				}
				else if (ExpressionType.ReportParameter == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsReportItemInReportParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.ReportLanguage == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsReportItemInReportLanguageExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.VariableValue == m_context.ExpressionType || ExpressionType.GroupVariableValue == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsReportItemInVariableExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.SortExpression == m_context.ExpressionType || ExpressionType.UserSortExpression == m_context.ExpressionType || ExpressionType.DataRegionSortExpression == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsReportItemInSortExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.JoinExpression == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsReportItemInJoinExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (m_context.InLookup)
				{
					m_errorContext.Register(ProcessingErrorCode.rsReportItemInLookupDestinationOrResult, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(false, "Unknown ExpressionType: {0} denying ReportItems.", m_context.ExpressionType);
				}
			}
			if ((m_context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0 && 1 < num)
			{
				m_errorContext.Register(ProcessingErrorCode.rsMultiReportItemsInPageSectionExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
			}
			if (0 < num)
			{
				if ((m_context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
				{
					m_state.PageSectionRefersToReportItems = true;
				}
				else
				{
					m_state.BodyRefersToReportItems = true;
				}
			}
			if ((m_context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0 && Detected(expression, m_regexes.OverallTotalPagesDetection))
			{
				m_state.PageSectionRefersToOverallTotalPages = true;
			}
			if ((m_context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0 && Detected(expression, m_regexes.TotalPagesDetection))
			{
				m_state.PageSectionRefersToTotalPages = true;
			}
			if (Detected(expression, m_regexes.OverallPageGlobalsDetection))
			{
				expressionInfo.ReferencedOverallPageGlobals = true;
				if ((grammarFlags & GrammarFlags.DenyOverallPageGlobals) != 0)
				{
					Global.Tracer.Assert((m_context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) == 0, "(0 == (m_context.Location & LocationFlags.InPageSection))");
					m_errorContext.Register(ProcessingErrorCode.rsOverallPageNumberInBody, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
			if (Detected(expression, m_regexes.PageGlobalsDetection))
			{
				expressionInfo.ReferencedPageGlobals = true;
				if ((grammarFlags & GrammarFlags.DenyPageGlobals) != 0)
				{
					Global.Tracer.Assert((m_context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) == 0, "(0 == (m_context.Location & LocationFlags.InPageSection))");
					m_errorContext.Register(ProcessingErrorCode.rsPageNumberInBody, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
			if (Detected(expression, m_regexes.AggregatesDetection))
			{
				m_errorContext.Register(ProcessingErrorCode.rsGlobalNotDefined, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
			}
			if ((grammarFlags & GrammarFlags.DenyDataSets) != 0 && Detected(expression, m_regexes.DataSetsDetection))
			{
				if ((m_context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
				{
					m_errorContext.Register(ProcessingErrorCode.rsDataSetInPageSectionExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.QueryParameter == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsDataSetInQueryParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, m_context.DataSetName);
				}
				else if (ExpressionType.ReportLanguage == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsDataSetInReportLanguageExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(ExpressionType.ReportParameter == m_context.ExpressionType, "(ExpressionType.ReportParameter == m_context.ExpressionType)");
					m_errorContext.Register(ProcessingErrorCode.rsDataSetInReportParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
			if ((grammarFlags & GrammarFlags.DenyDataSources) != 0 && Detected(expression, m_regexes.DataSourcesDetection))
			{
				if ((m_context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
				{
					m_errorContext.Register(ProcessingErrorCode.rsDataSourceInPageSectionExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.QueryParameter == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsDataSourceInQueryParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, m_context.DataSetName);
				}
				else if (ExpressionType.ReportLanguage == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsDataSourceInReportLanguageExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(ExpressionType.ReportParameter == m_context.ExpressionType, "(ExpressionType.ReportParameter == m_context.ExpressionType)");
					m_errorContext.Register(ProcessingErrorCode.rsDataSourceInReportParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
			if ((grammarFlags & GrammarFlags.DenyMeDotValue) != 0 && Detected(expression, m_regexes.MeDotValueDetection))
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidMeDotValueInExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
			}
			RemoveLineTerminators(ref expression, m_regexes.LineTerminatorDetection);
			if (Detected(expression, m_regexes.IllegalCharacterDetection))
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidCharacterInExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
			}
		}

		private void GetMeDotValueReferences(string strTransformedExpression, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			GetMeDotValueReferences(strTransformedExpression, expressionInfo, inTransformedExpression: true);
		}

		private void GetMeDotValueReferences(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			GetMeDotValueReferences(expressionInfo.OriginalText, expressionInfo, inTransformedExpression: false);
		}

		private void GetMeDotValueReferences(string expression, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, bool inTransformedExpression)
		{
			MatchCollection matchCollection = m_regexes.MeDotValueExpression.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				Group group = matchCollection[i].Groups["medotvalue"];
				if (group.Value != null && group.Value.Length > 0)
				{
					if (inTransformedExpression)
					{
						expressionInfo.AddMeDotValueInTransformedExpression(group.Index);
					}
					else
					{
						expressionInfo.AddMeDotValueInOriginalText(group.Index);
					}
				}
			}
		}

		private void GetReferencedReportItemNames(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			GetReferencedReportItemNames(expressionInfo.OriginalText, expressionInfo, inTransformedExpression: false);
		}

		private void GetReferencedReportItemNames(string expression, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			GetReferencedReportItemNames(expression, expressionInfo, inTransformedExpression: true);
		}

		private void GetReferencedReportItemNames(string expression, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, bool inTransformedExpression)
		{
			MatchCollection matchCollection = m_regexes.ReportItemName.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				Group group = matchCollection[i].Groups["reportitemname"];
				if (group.Value != null && group.Value.Length > 0)
				{
					if (inTransformedExpression)
					{
						expressionInfo.AddReferencedReportItemInTransformedExpression(group.Value, group.Index);
					}
					else
					{
						expressionInfo.AddReferencedReportItemInOriginalText(group.Value, group.Index);
					}
				}
			}
			matchCollection = m_regexes.SimpleDynamicReportItemReference.Matches(expression);
			for (int j = 0; j < matchCollection.Count; j++)
			{
				Group group2 = matchCollection[j].Groups["reportitemname"];
				if (group2.Value != null)
				{
					if (inTransformedExpression)
					{
						expressionInfo.AddReferencedReportItemInTransformedExpression(group2.Value, group2.Index);
					}
					else
					{
						expressionInfo.AddReferencedReportItemInOriginalText(group2.Value, group2.Index);
					}
				}
			}
		}

		private void GetReferencedVariableNames(string expression, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			MatchCollection matchCollection = m_regexes.VariableName.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				Group group = matchCollection[i].Groups["variablename"];
				if (group.Value != null && group.Value.Length > 0)
				{
					expressionInfo.AddReferencedVariable(group.Value, group.Index);
				}
			}
			matchCollection = m_regexes.SimpleDynamicVariableReference.Matches(expression);
			for (int j = 0; j < matchCollection.Count; j++)
			{
				Group group2 = matchCollection[j].Groups["variablename"];
				if (group2.Value != null)
				{
					expressionInfo.AddReferencedVariable(group2.Value, group2.Index);
				}
			}
		}

		private void GetReferencedFieldNames(string expression, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			if (Detected(expression, m_regexes.FieldDetection))
			{
				expressionInfo.HasAnyFieldReferences = true;
			}
			if (Detected(expression, m_regexes.DynamicFieldReference))
			{
				expressionInfo.DynamicFieldReferences = true;
			}
			else
			{
				MatchCollection matchCollection = m_regexes.DynamicFieldPropertyReference.Matches(expression);
				for (int i = 0; i < matchCollection.Count; i++)
				{
					string text = matchCollection[i].Result("${fieldname}");
					if (text != null && text.Length != 0)
					{
						expressionInfo.AddDynamicPropertyReference(text);
					}
				}
				matchCollection = m_regexes.StaticFieldPropertyReference.Matches(expression);
				for (int j = 0; j < matchCollection.Count; j++)
				{
					string text2 = matchCollection[j].Result("${fieldname}");
					string text3 = matchCollection[j].Result("${propertyname}");
					if (text2 != null && text2.Length != 0 && text3 != null && text3.Length != 0)
					{
						expressionInfo.AddStaticPropertyReference(text2, text3);
					}
				}
			}
			MatchCollection matchCollection2 = m_regexes.FieldName.Matches(expression);
			for (int k = 0; k < matchCollection2.Count; k++)
			{
				string text4 = matchCollection2[k].Result("${fieldname}");
				if (text4 != null && text4.Length != 0)
				{
					expressionInfo.AddReferencedField(text4);
				}
			}
			matchCollection2 = m_regexes.SimpleDynamicFieldReference.Matches(expression);
			for (int l = 0; l < matchCollection2.Count; l++)
			{
				Group group = matchCollection2[l].Groups["fieldname"];
				if (group.Value != null && group.Value.Length > 0)
				{
					expressionInfo.AddReferencedField(group.Value);
				}
			}
		}

		private void GetReferencedScopesAndScopedFields(string expression, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			MatchCollection matchCollection = m_regexes.ScopeName.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				HandleMatchedScopeReference(expression, expressionInfo, matchCollection[i]);
			}
			matchCollection = m_regexes.SimpleDynamicScopeReference.Matches(expression);
			for (int j = 0; j < matchCollection.Count; j++)
			{
				HandleMatchedScopeReference(expression, expressionInfo, matchCollection[j]);
			}
		}

		private void HandleMatchedScopeReference(string expression, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, Match match)
		{
			Group group = match.Groups["scopename"];
			string value = group.Value;
			if (!group.Success || string.IsNullOrEmpty(value))
			{
				return;
			}
			string text = null;
			if (match.Groups["hasfields"].Success)
			{
				int startat = match.Index + match.Length;
				Match match2 = m_regexes.DictionaryOpWithIdentifier.Match(expression, startat);
				if (!match2.Success)
				{
					match2 = m_regexes.IndexerWithIdentifier.Match(expression, startat);
				}
				if (match2.Success)
				{
					Group group2 = match2.Groups["fieldname"];
					if (group2.Success)
					{
						text = group2.Value;
					}
				}
			}
			ScopeReference scopeReference = null;
			scopeReference = (string.IsNullOrEmpty(text) ? new ScopeReference(value) : new ScopeReference(value, text));
			expressionInfo.AddReferencedScope(scopeReference);
		}

		private void GetReferencedParameterNames(string expression, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			if (Detected(expression, m_regexes.DynamicParameterReference))
			{
				expressionInfo.HasDynamicParameterReference = true;
			}
			MatchCollection matchCollection = m_regexes.ParameterName.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				string text = matchCollection[i].Result("${parametername}");
				if (text != null && text.Length != 0)
				{
					expressionInfo.AddReferencedParameter(text);
				}
			}
		}

		private bool HasRenderFormatNonIsInteractiveReference(string expression, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, out string referencedRenderFormatProperty)
		{
			referencedRenderFormatProperty = null;
			MatchCollection matchCollection = m_regexes.RenderFormatPropertyName.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				referencedRenderFormatProperty = matchCollection[i].Result("${propertyname}");
				if (!string.IsNullOrEmpty(referencedRenderFormatProperty) && Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture("IsInteractive", referencedRenderFormatProperty, ignoreCase: true) != 0)
				{
					return true;
				}
			}
			return false;
		}

		private void GetReferencedDataSetNames(string expression, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			MatchCollection matchCollection = m_regexes.DataSetName.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				string text = matchCollection[i].Result("${datasetname}");
				if (text != null && text.Length != 0)
				{
					expressionInfo.AddReferencedDataSet(text);
				}
			}
		}

		private void GetReferencedDataSourceNames(string expression, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			MatchCollection matchCollection = m_regexes.DataSourceName.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				string text = matchCollection[i].Result("${datasourcename}");
				if (text != null && text.Length != 0)
				{
					expressionInfo.AddReferencedDataSource(text);
				}
			}
		}

		private bool Detected(string expression, Regex detectionRegex)
		{
			return NumberOfTimesDetected(expression, detectionRegex) != 0;
		}

		private int NumberOfTimesDetected(string expression, Regex detectionRegex)
		{
			int num = 0;
			MatchCollection matchCollection = detectionRegex.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				string text = matchCollection[i].Result("${detected}");
				if (text != null && text.Length != 0)
				{
					num++;
				}
			}
			return num;
		}

		private void RemoveLineTerminators(ref string expression, Regex detectionRegex)
		{
			if (expression == null)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(expression, expression.Length);
			MatchCollection matchCollection = detectionRegex.Matches(expression);
			for (int num = matchCollection.Count - 1; num >= 0; num--)
			{
				string text = matchCollection[num].Result("${detected}");
				if (text != null && text.Length != 0)
				{
					stringBuilder.Remove(matchCollection[num].Index, matchCollection[num].Length);
				}
			}
			if (matchCollection.Count != 0)
			{
				expression = stringBuilder.ToString();
			}
		}

		private void GetRunningValue(int currentPos, string functionName, string expression, bool isParameter, GrammarFlags grammarFlags, out int newPos, out Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValue)
		{
			if (m_context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.RunningValue))
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, functionName);
			}
			if ((grammarFlags & GrammarFlags.DenyRunningValue) != 0)
			{
				if (m_context.InPrevious)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRunningValueInPreviousAggregate, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (isParameter)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRunningValueInAggregateExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.DataSetFilters == m_context.ExpressionType || ExpressionType.DataRegionFilters == m_context.ExpressionType || ExpressionType.GroupingFilters == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRunningValueInFilterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.GroupExpression == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRunningValueInGroupExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if ((m_context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRunningValueInPageSectionExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.QueryParameter == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRunningValueInQueryParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, m_context.DataSetName);
				}
				else if (ExpressionType.ReportParameter == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRunningValueInReportParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.ReportLanguage == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRunningValueInReportLanguageExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.VariableValue == m_context.ExpressionType || ExpressionType.GroupVariableValue == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRunningValueInVariableExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.FieldValue == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateInCalculatedFieldExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.JoinExpression == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRunningValueInJoinExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(ExpressionType.SortExpression == m_context.ExpressionType || ExpressionType.UserSortExpression == m_context.ExpressionType || ExpressionType.DataRegionSortExpression == m_context.ExpressionType, "(SortExpression == m_context.ExpressionType)");
					m_errorContext.Register(ProcessingErrorCode.rsRunningValueInSortExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
			GetArguments(currentPos, expression, out newPos, out List<string> arguments);
			if (3 != arguments.Count)
			{
				m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, functionName);
			}
			runningValue = new Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo();
			if (2 <= arguments.Count)
			{
				bool flag;
				try
				{
					runningValue.AggregateType = (Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes)Enum.Parse(typeof(Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes), arguments[1], ignoreCase: true);
					flag = (Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Aggregate != runningValue.AggregateType && Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Previous != runningValue.AggregateType && Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.CountRows != runningValue.AggregateType);
				}
				catch (ArgumentException)
				{
					flag = false;
				}
				if (!flag)
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidRunningValueAggregate, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
			if (1 <= arguments.Count)
			{
				if (Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Count == runningValue.AggregateType && "*" == arguments[0].Trim())
				{
					m_errorContext.Register(ProcessingErrorCode.rsCountStarRVNotSupported, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else
				{
					runningValue.Expressions = new Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo[1];
					runningValue.Expressions[0] = GetParameterExpression(runningValue, arguments[0], grammarFlags);
				}
			}
			if (3 <= arguments.Count)
			{
				runningValue.Scope = GetAggregateScope(arguments[2], allowNothing: true);
			}
			DetectAggregateFieldReferences(runningValue);
			m_state.NumberOfRunningValues++;
		}

		private void GetPreviousAggregate(int currentPos, string functionName, string expression, bool isParameter, GrammarFlags grammarFlags, out int newPos, out Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValue)
		{
			if (m_context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.Previous))
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, functionName);
			}
			if ((grammarFlags & GrammarFlags.DenyPrevious) != 0)
			{
				if (m_context.InPrevious)
				{
					m_errorContext.Register(ProcessingErrorCode.rsPreviousInPreviousAggregate, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (m_context.InLookup)
				{
					m_errorContext.Register(ProcessingErrorCode.rsPreviousInLookupDestinationOrResult, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (isParameter)
				{
					m_errorContext.Register(ProcessingErrorCode.rsPreviousInAggregateExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.DataSetFilters == m_context.ExpressionType || ExpressionType.DataRegionFilters == m_context.ExpressionType || ExpressionType.GroupingFilters == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInFilterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.GroupExpression == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInGroupExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if ((m_context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
				{
					m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInPageSectionExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.QueryParameter == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInQueryParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, m_context.DataSetName);
				}
				else if (ExpressionType.ReportParameter == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInReportParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.ReportLanguage == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInReportLanguageExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.FieldValue == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateInCalculatedFieldExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.VariableValue == m_context.ExpressionType || ExpressionType.GroupVariableValue == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInVariableExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.JoinExpression == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInJoinExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(ExpressionType.SortExpression == m_context.ExpressionType || ExpressionType.UserSortExpression == m_context.ExpressionType || ExpressionType.DataRegionSortExpression == m_context.ExpressionType, "(SortExpression == m_context.ExpressionType)");
					m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInSortExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
			m_state.PreviousAggregateUsed = true;
			m_context.InPrevious = true;
			m_state.NumberOfRunningValues++;
			runningValue = new Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo();
			runningValue.AggregateType = Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Previous;
			GetArguments(currentPos, expression, out newPos, out List<string> arguments);
			if (arguments.Count == 1 || arguments.Count == 2)
			{
				runningValue.Expressions = new Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo[1];
				Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo parameterExpression = GetParameterExpression(runningValue, arguments[0], grammarFlags);
				parameterExpression.InPrevious = true;
				runningValue.Expressions[0] = parameterExpression;
				if (HasInScopeOrLevel(arguments[0]))
				{
					m_errorContext.Register(ProcessingErrorCode.rsInScopeOrLevelInPreviousAggregate, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				if (arguments.Count == 2)
				{
					runningValue.Scope = GetAggregateScope(arguments[1], allowNothing: false);
				}
			}
			else
			{
				m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, functionName);
			}
			m_context.InPrevious = false;
		}

		private bool HasInScopeOrLevel(string expression)
		{
			return m_regexes.InScopeOrLevel.Match(expression).Success;
		}

		private void GetRowNumber(int currentPos, string functionName, string expression, bool isParameter, GrammarFlags grammarFlags, out int newPos, out Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo rowNumber)
		{
			if (m_context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.RowNumber))
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, functionName);
			}
			if ((grammarFlags & GrammarFlags.DenyRowNumber) != 0)
			{
				if (m_context.InPrevious)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRowNumberInPreviousAggregate, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				if (m_context.InLookup)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRowNumberInLookupDestinationOrResult, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (isParameter)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateofAggregate, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.DataSetFilters == m_context.ExpressionType || ExpressionType.DataRegionFilters == m_context.ExpressionType || ExpressionType.GroupingFilters == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRowNumberInFilterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if ((m_context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRowNumberInPageSectionExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.QueryParameter == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRowNumberInQueryParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, m_context.DataSetName);
				}
				else if (ExpressionType.ReportParameter == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRowNumberInReportParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.ReportLanguage == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRowNumberInReportLanguageExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.VariableValue == m_context.ExpressionType || ExpressionType.GroupVariableValue == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRowNumberInVariableExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.FieldValue == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateInCalculatedFieldExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(ExpressionType.SortExpression == m_context.ExpressionType || ExpressionType.UserSortExpression == m_context.ExpressionType || ExpressionType.DataRegionSortExpression == m_context.ExpressionType, "(SortExpression == m_context.ExpressionType)");
					m_errorContext.Register(ProcessingErrorCode.rsRowNumberInSortExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
			GetArguments(currentPos, expression, out newPos, out List<string> arguments);
			if (1 != arguments.Count)
			{
				m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, functionName);
			}
			rowNumber = new Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo();
			rowNumber.AggregateType = Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.CountRows;
			rowNumber.Expressions = new Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo[0];
			if (1 <= arguments.Count)
			{
				rowNumber.Scope = GetAggregateScope(arguments[0], allowNothing: true);
			}
			m_state.NumberOfRunningValues++;
		}

		private string GetAggregateScope(string expression, bool allowNothing)
		{
			return GetScope(expression, allowNothing, ProcessingErrorCode.rsInvalidAggregateScope);
		}

		private string GetLookupScope(string expression)
		{
			return GetScope(expression, allowNothing: false, ProcessingErrorCode.rsInvalidLookupScope);
		}

		private string GetScope(string expression, bool allowNothing, ProcessingErrorCode errorCode)
		{
			if (m_regexes.NothingOnly.Match(expression).Success)
			{
				if (allowNothing)
				{
					return null;
				}
			}
			else
			{
				Match match = m_regexes.StringLiteralOnly.Match(expression);
				if (match.Success)
				{
					return match.Result("${string}");
				}
			}
			m_errorContext.Register(errorCode, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
			return null;
		}

		private bool IsRecursive(string flag)
		{
			RecursiveFlags recursiveFlags = RecursiveFlags.Simple;
			try
			{
				recursiveFlags = (RecursiveFlags)Enum.Parse(typeof(RecursiveFlags), flag, ignoreCase: true);
			}
			catch (Exception)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidAggregateRecursiveFlag, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
			}
			if (RecursiveFlags.Recursive == recursiveFlags)
			{
				return true;
			}
			return false;
		}

		private LookupInfo GetLookup(int currentPos, string functionName, string expression, bool isParameter, GrammarFlags grammarFlags, LookupType lookupType, out int newPos, out string lookupID)
		{
			lookupID = CreateLookupID();
			if (m_context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.Lookup))
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, functionName);
			}
			if ((grammarFlags & GrammarFlags.DenyLookups) != 0)
			{
				if (m_context.InLookup)
				{
					m_errorContext.Register(ProcessingErrorCode.rsNestedLookups, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.DataSetFilters == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsLookupInFilterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.QueryParameter == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateInQueryParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, m_context.DataSetName);
				}
				else if (ExpressionType.ReportLanguage == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateInReportLanguageExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.FieldValue == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateInCalculatedFieldExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.ReportParameter == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateInReportParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(false, "Unknown ExpressionType for Lookup restriction: {0}", m_context.ExpressionType);
				}
			}
			m_context.InLookup = true;
			GetArguments(currentPos, expression, out newPos, out List<string> arguments);
			if (arguments.Count != 4)
			{
				m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, functionName);
			}
			LookupInfo lookupInfo = new LookupInfo();
			lookupInfo.LookupType = lookupType;
			lookupInfo.Name = lookupID;
			lookupInfo.DestinationInfo = new LookupDestinationInfo();
			lookupInfo.DestinationInfo.IsMultiValue = (lookupType == LookupType.LookupSet);
			if (arguments.Count > 3)
			{
				lookupInfo.DestinationInfo.Scope = GetLookupScope(arguments[3]);
			}
			if (arguments.Count > 2)
			{
				lookupInfo.ResultExpr = GetLookupParameterExpr(arguments[2], GrammarFlags.DenyAggregates | GrammarFlags.DenyRunningValue | GrammarFlags.DenyRowNumber | GrammarFlags.DenyReportItems | GrammarFlags.DenyOverallPageGlobals | GrammarFlags.DenyPrevious | GrammarFlags.DenyVariables | GrammarFlags.DenyLookups | GrammarFlags.DenyPageGlobals | GrammarFlags.DenyRenderFormatAll | GrammarFlags.DenyScopes, isParameter);
			}
			if (arguments.Count > 1)
			{
				lookupInfo.DestinationInfo.DestinationExpr = GetLookupParameterExpr(arguments[1], GrammarFlags.DenyAggregates | GrammarFlags.DenyRunningValue | GrammarFlags.DenyRowNumber | GrammarFlags.DenyReportItems | GrammarFlags.DenyOverallPageGlobals | GrammarFlags.DenyPrevious | GrammarFlags.DenyVariables | GrammarFlags.DenyLookups | GrammarFlags.DenyPageGlobals | GrammarFlags.DenyRenderFormatAll | GrammarFlags.DenyScopes, isParameter);
			}
			if (arguments.Count > 0)
			{
				lookupInfo.SourceExpr = GetLookupParameterExpr(arguments[0], grammarFlags | (GrammarFlags.DenyVariables | GrammarFlags.DenyLookups | GrammarFlags.DenyRenderFormatAll), isParameter);
			}
			m_state.NumberOfLookups++;
			return lookupInfo;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo GetLookupParameterExpr(string parameterExpression, GrammarFlags grammarFlags, bool isParameter)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = new Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo();
			expressionInfo.OriginalText = parameterExpression;
			if (m_context.InAggregate)
			{
				grammarFlags |= GrammarFlags.DenyAggregates;
			}
			VBLex(parameterExpression, isParameter, grammarFlags, expressionInfo);
			return expressionInfo;
		}

		private bool ParseRdlFunction(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, int currentPos, string functionName, string expression, GrammarFlags grammarFlags, out int newPos)
		{
			newPos = currentPos;
			RdlFunctionInfo rdlFunctionInfo = new RdlFunctionInfo();
			GetArguments(currentPos, expression, out newPos, out List<string> arguments);
			if (arguments.Count < 2)
			{
				m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, functionName);
			}
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo>(arguments.Count);
			foreach (string item in arguments)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo2 = new Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo();
				list.Add(expressionInfo2);
				expressionInfo2.OriginalText = item;
				VBLex(item, isParameter: false, grammarFlags, expressionInfo2);
				if (expressionInfo2.Type == Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression)
				{
					return true;
				}
			}
			expressionInfo.SetAsRdlFunction(rdlFunctionInfo);
			rdlFunctionInfo.SetFunctionType(functionName);
			rdlFunctionInfo.Expressions = list;
			return false;
		}

		private void GetAggregate(int currentPos, string functionName, string expression, bool isParameter, GrammarFlags grammarFlags, out int newPos, out Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregate)
		{
			if ((grammarFlags & GrammarFlags.DenyAggregates) != 0)
			{
				if (isParameter)
				{
					if (m_context.InLookup)
					{
						m_errorContext.Register(ProcessingErrorCode.rsNestedAggregateViaLookup, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
					}
					else if ((m_context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
					{
						m_errorContext.Register(ProcessingErrorCode.rsNestedAggregateInPageSection, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
					}
					else
					{
						m_errorContext.Register(ProcessingErrorCode.rsAggregateofAggregate, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
					}
				}
				else if (ExpressionType.DataSetFilters == m_context.ExpressionType || ExpressionType.DataRegionFilters == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateInFilterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.GroupExpression == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateInGroupExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.DataRegionSortExpression == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateInDataRowSortExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.QueryParameter == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateInQueryParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, m_context.DataSetName);
				}
				else if (ExpressionType.ReportLanguage == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateInReportLanguageExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.FieldValue == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateInCalculatedFieldExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.ReportParameter == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateInReportParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.JoinExpression == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateInJoinExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (m_context.InLookup)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateInLookupDestinationOrResult, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(m_context.InPrevious, "(m_context.InPrevious)");
				}
			}
			bool inPrevious = m_context.InPrevious;
			m_context.InPrevious = false;
			GetArguments(currentPos, expression, out newPos, out List<string> arguments);
			if (arguments.Count > 3)
			{
				m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, functionName);
			}
			aggregate = new Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo();
			aggregate.AggregateType = (Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes)Enum.Parse(typeof(Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes), functionName, ignoreCase: true);
			if ((grammarFlags & GrammarFlags.DenyPostSortAggregate) != 0 && aggregate.IsPostSortAggregate())
			{
				if (m_context.InAggregate)
				{
					m_errorContext.Register(ProcessingErrorCode.rsPostSortAggregateInAggregateExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.GroupingFilters == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsPostSortAggregateInGroupFilterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.SortExpression == m_context.ExpressionType || ExpressionType.UserSortExpression == m_context.ExpressionType || ExpressionType.DataRegionFilters == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsPostSortAggregateInSortExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.VariableValue == m_context.ExpressionType || ExpressionType.GroupVariableValue == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsPostSortAggregateInVariableExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
			if (Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.CountRows == aggregate.AggregateType)
			{
				aggregate.AggregateType = Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.CountRows;
				aggregate.Expressions = new Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo[0];
				if (1 <= arguments.Count)
				{
					aggregate.SetScope(GetAggregateScope(arguments[0], allowNothing: false));
				}
				if (2 <= arguments.Count)
				{
					if (m_context.InAggregate)
					{
						m_errorContext.Register(ProcessingErrorCode.rsInvalidNestedRecursiveAggregate, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
					}
					else
					{
						aggregate.Recursive = IsRecursive(arguments[1]);
					}
				}
				if (3 <= arguments.Count)
				{
					m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, functionName);
				}
			}
			else
			{
				if (arguments.Count == 0)
				{
					m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, functionName);
				}
				else if (1 <= arguments.Count)
				{
					if (Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Count == aggregate.AggregateType && "*" == arguments[0].Trim())
					{
						m_errorContext.Register(ProcessingErrorCode.rsCountStarNotSupported, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
					}
					else
					{
						aggregate.Expressions = new Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo[1];
						aggregate.Expressions[0] = GetParameterExpression(aggregate, arguments[0], grammarFlags);
						if (Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Aggregate == aggregate.AggregateType)
						{
							if (Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Field != aggregate.Expressions[0].Type)
							{
								m_errorContext.Register(ProcessingErrorCode.rsInvalidCustomAggregateExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
							}
							if (aggregate.AggregateType == Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Aggregate && inPrevious)
							{
								m_errorContext.Register(ProcessingErrorCode.rsAggregateInPreviousAggregate, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
							}
						}
					}
				}
				if (2 <= arguments.Count)
				{
					aggregate.SetScope(GetAggregateScope(arguments[1], allowNothing: false));
				}
				if (3 <= arguments.Count)
				{
					if (aggregate.IsPostSortAggregate() || Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Aggregate == aggregate.AggregateType || inPrevious)
					{
						m_errorContext.Register(ProcessingErrorCode.rsInvalidRecursiveAggregate, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
					}
					else if (m_context.InAggregate)
					{
						m_errorContext.Register(ProcessingErrorCode.rsInvalidNestedRecursiveAggregate, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
					}
					else if (aggregate.IsAggregateOfAggregate)
					{
						m_errorContext.Register(ProcessingErrorCode.rsRecursiveAggregateOfAggregate, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
					}
					else
					{
						aggregate.Recursive = IsRecursive(arguments[2]);
					}
				}
			}
			if (m_context.OuterAggregate != null)
			{
				m_context.OuterAggregate.AddNestedAggregate(aggregate);
			}
			m_state.AggregateOfAggregatesUsed |= aggregate.IsAggregateOfAggregate;
			if (aggregate.IsAggregateOfAggregate && m_context.ExpressionType == ExpressionType.UserSortExpression)
			{
				m_state.AggregateOfAggregatesUsedInUserSort = true;
			}
			DetectAggregateFieldReferences(aggregate);
			if ((grammarFlags & GrammarFlags.DenyAggregatesOfAggregates) != 0 && aggregate.IsAggregateOfAggregate)
			{
				if (ExpressionType.GroupingFilters == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsNestedAggregateInFilterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.GroupVariableValue == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsNestedAggregateInGroupVariable, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
		}

		private void DetectAggregateFieldReferences(Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregate)
		{
			if (aggregate.Expressions == null || aggregate.Expressions.Length == 0)
			{
				return;
			}
			int num = 0;
			while (true)
			{
				if (num >= aggregate.Expressions.Length)
				{
					return;
				}
				Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = aggregate.Expressions[num];
				if (expressionInfo.HasAnyFieldReferences)
				{
					break;
				}
				if (expressionInfo.Lookups != null)
				{
					foreach (LookupInfo lookup in expressionInfo.Lookups)
					{
						if (lookup.SourceExpr.HasAnyFieldReferences)
						{
							aggregate.PublishingInfo.HasAnyFieldReferences = true;
							break;
						}
					}
				}
				num++;
			}
			aggregate.PublishingInfo.HasAnyFieldReferences = true;
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo GetParameterExpression(Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo outerAggregate, string parameterExpression, GrammarFlags grammarFlags)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = new Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo();
			expressionInfo.OriginalText = parameterExpression;
			grammarFlags = (((m_context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0) ? (grammarFlags | (GrammarFlags.DenyAggregates | GrammarFlags.DenyRunningValue | GrammarFlags.DenyRowNumber | GrammarFlags.DenyPostSortAggregate | GrammarFlags.DenyPrevious | GrammarFlags.DenyVariables | GrammarFlags.DenyScopes)) : (m_context.InPrevious ? (grammarFlags | (GrammarFlags.DenyRowNumber | GrammarFlags.DenyReportItems | GrammarFlags.DenyPrevious | GrammarFlags.DenyVariables | GrammarFlags.DenyScopes)) : (grammarFlags | (GrammarFlags.DenyRunningValue | GrammarFlags.DenyRowNumber | GrammarFlags.DenyReportItems | GrammarFlags.DenyPostSortAggregate | GrammarFlags.DenyPrevious | GrammarFlags.DenyVariables | GrammarFlags.DenyScopes))));
			Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo outerAggregate2 = m_context.OuterAggregate;
			m_context.OuterAggregate = outerAggregate;
			VBLex(parameterExpression, isParameter: true, grammarFlags, expressionInfo);
			m_context.OuterAggregate = outerAggregate2;
			return expressionInfo;
		}

		private void GetArguments(int currentPos, string expression, out int newPos, out List<string> arguments)
		{
			int num = 1;
			int num2 = 0;
			arguments = new List<string>();
			string text = string.Empty;
			while (0 < num && currentPos < expression.Length)
			{
				Match match = m_regexes.Arguments.Match(expression, currentPos);
				if (!match.Success)
				{
					text += expression.Substring(currentPos);
					currentPos = expression.Length;
					continue;
				}
				string text2 = match.Result("${openParen}");
				string text3 = match.Result("${closeParen}");
				string text4 = match.Result("${comma}");
				string text5 = match.Result("${openCurly}");
				string text6 = match.Result("${closeCurly}");
				if (text2 != null && text2.Length != 0)
				{
					num++;
					text += expression.Substring(currentPos, match.Index - currentPos + match.Length);
				}
				else if (text3 != null && text3.Length != 0)
				{
					num--;
					if (num == 0)
					{
						text += expression.Substring(currentPos, match.Index - currentPos);
						if (text.Trim().Length != 0)
						{
							arguments.Add(text);
							text = string.Empty;
						}
					}
					else
					{
						text += expression.Substring(currentPos, match.Index - currentPos + match.Length);
					}
				}
				else if (text5 != null && text5.Length != 0)
				{
					num2++;
					text += expression.Substring(currentPos, match.Index - currentPos + match.Length);
				}
				else if (text6 != null && text6.Length != 0)
				{
					num2--;
					text += expression.Substring(currentPos, match.Index - currentPos + match.Length);
				}
				else if (text4 != null && text4.Length != 0)
				{
					if (1 == num && num2 == 0)
					{
						text += expression.Substring(currentPos, match.Index - currentPos);
						arguments.Add(text);
						text = string.Empty;
					}
					else
					{
						text += expression.Substring(currentPos, match.Index - currentPos + match.Length);
					}
				}
				else
				{
					text += expression.Substring(currentPos, match.Index - currentPos + match.Length);
				}
				currentPos = match.Index + match.Length;
			}
			if (num > 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsExpressionMissingCloseParen, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				if (text.Trim().Length != 0)
				{
					arguments.Add(text);
					text = string.Empty;
				}
			}
			newPos = currentPos;
		}

		private string CreateAggregateID()
		{
			return "Aggregate" + m_state.LastID;
		}

		private string CreateLookupID()
		{
			return "Lookup" + m_state.LastLookupID;
		}
	}
}

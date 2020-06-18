using Microsoft.VisualBasic;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class VBExpressionParser : ExpressionParser
	{
		private sealed class ReportRegularExpressions
		{
			internal Regex NonConstant;

			internal Regex FieldDetection;

			internal Regex ReportItemsDetection;

			internal Regex ParametersDetection;

			internal Regex PageGlobalsDetection;

			internal Regex AggregatesDetection;

			internal Regex UserDetection;

			internal Regex DataSetsDetection;

			internal Regex DataSourcesDetection;

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

			internal Regex Arguments;

			internal Regex DynamicFieldReference;

			internal Regex DynamicFieldPropertyReference;

			internal Regex StaticFieldPropertyReference;

			internal Regex RewrittenCommandText;

			internal Regex ExtendedPropertyName;

			internal Regex FieldWithExtendedProperty;

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
				FieldDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>Fields)" + text6, options);
				ReportItemsDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>ReportItems)" + text6, options);
				ParametersDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>Parameters)" + text6, options);
				PageGlobalsDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>(Globals" + text4 + "PageNumber)|(Globals" + text4 + "TotalPages))" + text6, options);
				AggregatesDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>Aggregates)" + text6, options);
				UserDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>User)" + text6, options);
				DataSetsDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>DataSets)" + text6, options);
				DataSourcesDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>DataSources)" + text6, options);
				MeDotValueDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>(?:Me.)?Value)" + text6, options);
				string text7 = Regex.Escape(":");
				string text8 = Regex.Escape("#");
				string text9 = "(" + text8 + "[^" + text8 + "]*" + text8 + ")";
				string text10 = Regex.Escape(":=");
				LineTerminatorDetection = new Regex("(?<detected>(\\u000D\\u000A)|([\\u000D\\u000A\\u2028\\u2029]))", options);
				IllegalCharacterDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text9 + "|" + text10 + "|(?<detected>" + text7 + ")", options);
				string text11 = "[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Cf}]*";
				string str = "ReportItems" + text2 + "(?<reportitemname>" + text11 + ")";
				string text12 = "Fields" + text2 + "(?<fieldname>" + text11 + ")";
				string text13 = "Parameters" + text2 + "(?<parametername>" + text11 + ")";
				string text14 = "DataSets" + text2 + "(?<datasetname>" + text11 + ")";
				string str2 = "DataSources" + text2 + "(?<datasourcename>" + text11 + ")";
				string text15 = "Fields((" + text2 + "(?<fieldname>" + text11 + "))|((" + text3 + "Item)?" + Regex.Escape("(") + "\"(?<fieldname>" + text11 + ")\"" + Regex.Escape(")") + "))";
				ExtendedPropertyName = new Regex("(Value|IsMissing|UniqueName|BackgroundColor|Color|FontFamily|Fontsize|FontWeight|FontStyle|TextDecoration|FormattedValue|Key|LevelNumber|ParentUniqueName)", options);
				string text16 = "(" + text3 + "Properties)?" + Regex.Escape("(") + "\"(?<propertyname>" + text11 + ")\"" + Regex.Escape(")");
				FieldWithExtendedProperty = new Regex(string.Concat("^\\s*", text15, "((", text3, ExtendedPropertyName, ")|(", text16, "))\\s*$"), options);
				DynamicFieldReference = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>(Fields(" + text3 + "Item)?" + Regex.Escape("(") + "))", options);
				DynamicFieldPropertyReference = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + text12 + "(" + text3 + "Properties)?" + Regex.Escape("("), options);
				StaticFieldPropertyReference = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + text12 + text3 + "(?<propertyname>" + text11 + ")", options);
				FieldOnly = new Regex("^\\s*" + text12 + text3 + "Value\\s*$", options);
				RewrittenCommandText = new Regex("^\\s*" + text14 + text3 + "RewrittenCommandText\\s*$", options);
				ParameterOnly = new Regex("^\\s*" + text13 + text3 + "Value\\s*$", options);
				StringLiteralOnly = new Regex("^\\s*\"(?<string>((\"\")|[^\"])*)\"\\s*$", options);
				NothingOnly = new Regex("^\\s*Nothing\\s*$", options);
				ReportItemName = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + str, options);
				FieldName = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + text12, options);
				ParameterName = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + text13, options);
				DataSetName = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + text14, options);
				DataSourceName = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + str2, options);
				SpecialFunction = new Regex("(\"((\"\")|[^\"])*\")|(?<prefix>" + text5 + ")(?<sfname>RunningValue|RowNumber|First|Last|Previous|Sum|Avg|Max|Min|CountDistinct|Count|CountRows|StDevP|VarP|StDev|Var|Aggregate)\\s*\\(", options);
				string text17 = Regex.Escape("(");
				string text18 = Regex.Escape(")");
				string text19 = Regex.Escape(",");
				Arguments = new Regex("(\"((\"\")|[^\"])*\")|(?<openParen>" + text17 + ")|(?<closeParen>" + text18 + ")|(?<comma>" + text19 + ")", options);
			}
		}

		private const string RunningValue = "RunningValue";

		private const string RowNumber = "RowNumber";

		private const string Previous = "Previous";

		private const string Star = "*";

		private ReportRegularExpressions m_regexes;

		private int m_numberOfAggregates;

		private int m_numberOfRunningValues;

		private bool m_bodyRefersToReportItems;

		private bool m_pageSectionRefersToReportItems;

		private ExpressionContext m_context;

		internal override bool BodyRefersToReportItems => m_bodyRefersToReportItems;

		internal override bool PageSectionRefersToReportItems => m_pageSectionRefersToReportItems;

		internal override int NumberOfAggregates => m_numberOfAggregates;

		internal override int LastID => m_numberOfAggregates + m_numberOfRunningValues;

		internal VBExpressionParser(ErrorContext errorContext)
			: base(errorContext)
		{
			m_regexes = ReportRegularExpressions.Value;
			m_numberOfAggregates = 0;
			m_numberOfRunningValues = 0;
			m_bodyRefersToReportItems = false;
			m_pageSectionRefersToReportItems = false;
		}

		internal override CodeDomProvider GetCodeCompiler()
		{
			return new VBCodeProvider();
		}

		internal override string GetCompilerArguments()
		{
			return "/optimize+";
		}

		internal override ExpressionInfo ParseExpression(string expression, ExpressionContext context)
		{
			Global.Tracer.Assert(expression != null);
			string vbExpression;
			return Lex(expression, context, out vbExpression);
		}

		internal override ExpressionInfo ParseExpression(string expression, ExpressionContext context, DetectionFlags flag, out bool reportParameterReferenced, out string reportParameterName, out bool userCollectionReferenced)
		{
			string vbExpression;
			ExpressionInfo expressionInfo = Lex(expression, context, out vbExpression);
			reportParameterReferenced = false;
			reportParameterName = null;
			userCollectionReferenced = false;
			if (expressionInfo.Type == ExpressionInfo.Types.Expression)
			{
				if ((flag & DetectionFlags.ParameterReference) != 0)
				{
					reportParameterReferenced = true;
					reportParameterName = GetReferencedReportParameters(vbExpression);
				}
				if ((flag & DetectionFlags.UserReference) != 0)
				{
					userCollectionReferenced = DetectUserReference(vbExpression);
				}
			}
			return expressionInfo;
		}

		internal override ExpressionInfo ParseExpression(string expression, ExpressionContext context, out bool userCollectionReferenced)
		{
			string vbExpression;
			ExpressionInfo expressionInfo = Lex(expression, context, out vbExpression);
			userCollectionReferenced = false;
			if (expressionInfo.Type == ExpressionInfo.Types.Expression)
			{
				userCollectionReferenced = DetectUserReference(vbExpression);
			}
			return expressionInfo;
		}

		internal override void ConvertField2ComplexExpr(ref ExpressionInfo info)
		{
			Global.Tracer.Assert(info.Type == ExpressionInfo.Types.Field);
			info.Type = ExpressionInfo.Types.Expression;
			info.TransformedExpression = "Fields!" + info.Value + ".Value";
		}

		private ExpressionInfo Lex(string expression, ExpressionContext context, out string vbExpression)
		{
			vbExpression = null;
			m_context = context;
			ExpressionInfo expressionInfo = context.ParseExtended ? new ExpressionInfoExtended() : new ExpressionInfo();
			expressionInfo.OriginalText = expression;
			Match match = m_regexes.NonConstant.Match(expression);
			if (!match.Success)
			{
				expressionInfo.Type = ExpressionInfo.Types.Constant;
				switch (context.ConstantType)
				{
				case ConstantType.String:
					expressionInfo.Value = expression;
					break;
				case ConstantType.Boolean:
				{
					bool boolValue;
					try
					{
						boolValue = XmlConvert.ToBoolean(expression);
					}
					catch
					{
						boolValue = false;
						m_errorContext.Register(ProcessingErrorCode.rsInvalidBooleanConstant, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, expression);
					}
					expressionInfo.BoolValue = boolValue;
					break;
				}
				case ConstantType.Integer:
				{
					int intValue;
					try
					{
						intValue = XmlConvert.ToInt32(expression);
					}
					catch
					{
						intValue = 0;
						m_errorContext.Register(ProcessingErrorCode.rsInvalidIntegerConstant, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, expression);
					}
					expressionInfo.IntValue = intValue;
					break;
				}
				default:
					Global.Tracer.Assert(condition: false);
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
			}
			else
			{
				GrammarFlags grammarFlags = (GrammarFlags)(((m_context.Location & LocationFlags.InPageSection) == 0) ? (ExpressionParser.ExpressionType2Restrictions(m_context.ExpressionType) | Restrictions.InBody) : (ExpressionParser.ExpressionType2Restrictions(m_context.ExpressionType) | Restrictions.InPageSection));
				vbExpression = expression.Substring(match.Length);
				VBLex(vbExpression, isParameter: false, grammarFlags, expressionInfo);
			}
			return expressionInfo;
		}

		private string GetReferencedReportParameters(string expression)
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

		private void VBLex(string expression, bool isParameter, GrammarFlags grammarFlags, ExpressionInfo expressionInfo)
		{
			if ((grammarFlags & GrammarFlags.DenyFields) == 0)
			{
				Match match = m_regexes.FieldOnly.Match(expression);
				if (match.Success)
				{
					string text = match.Result("${fieldname}");
					expressionInfo.AddReferencedField(text);
					expressionInfo.Type = ExpressionInfo.Types.Field;
					expressionInfo.Value = text;
					return;
				}
				if (m_context.ParseExtended)
				{
					match = m_regexes.FieldWithExtendedProperty.Match(expression);
					if (match.Success)
					{
						((ExpressionInfoExtended)expressionInfo).IsExtendedSimpleFieldReference = true;
					}
				}
			}
			if ((grammarFlags & GrammarFlags.DenyDataSets) == 0)
			{
				Match match2 = m_regexes.RewrittenCommandText.Match(expression);
				if (match2.Success)
				{
					string text2 = match2.Result("${datasetname}");
					expressionInfo.AddReferencedDataSet(text2);
					expressionInfo.Type = ExpressionInfo.Types.Token;
					expressionInfo.Value = text2;
					return;
				}
			}
			EnforceRestrictions(ref expression, isParameter, grammarFlags);
			string text3 = string.Empty;
			int newPos = 0;
			bool flag = false;
			while (newPos < expression.Length)
			{
				Match match3 = m_regexes.SpecialFunction.Match(expression, newPos);
				if (!match3.Success)
				{
					text3 += expression.Substring(newPos);
					newPos = expression.Length;
					continue;
				}
				text3 += expression.Substring(newPos, match3.Index - newPos);
				string text4 = match3.Result("${sfname}");
				if (text4 == null || text4.Length == 0)
				{
					text3 += match3.Value;
					newPos = match3.Index + match3.Length;
					continue;
				}
				text3 += match3.Result("${prefix}");
				newPos = match3.Index + match3.Length;
				string text5 = CreateAggregateID();
				if (string.Compare(text4, "Previous", StringComparison.OrdinalIgnoreCase) == 0)
				{
					GetPreviousAggregate(newPos, text4, expression, isParameter, grammarFlags, out newPos, out RunningValueInfo runningValue);
					runningValue.Name = text5;
					expressionInfo.AddRunningValue(runningValue);
				}
				else if (string.Compare(text4, "RunningValue", StringComparison.OrdinalIgnoreCase) == 0)
				{
					GetRunningValue(newPos, text4, expression, isParameter, grammarFlags, out newPos, out RunningValueInfo runningValue2);
					runningValue2.Name = text5;
					expressionInfo.AddRunningValue(runningValue2);
				}
				else if (string.Compare(text4, "RowNumber", StringComparison.OrdinalIgnoreCase) == 0)
				{
					GetRowNumber(newPos, text4, expression, isParameter, grammarFlags, out newPos, out RunningValueInfo rowNumber);
					rowNumber.Name = text5;
					expressionInfo.AddRunningValue(rowNumber);
				}
				else
				{
					GetAggregate(newPos, text4, expression, isParameter, grammarFlags, out newPos, out DataAggregateInfo aggregate);
					aggregate.Name = text5;
					expressionInfo.AddAggregate(aggregate);
				}
				if (!flag)
				{
					flag = true;
					if (text3.Trim().Length == 0 && expression.Substring(newPos).Trim().Length == 0)
					{
						expressionInfo.Type = ExpressionInfo.Types.Aggregate;
						expressionInfo.Value = text5;
						return;
					}
				}
				if (expressionInfo.TransformedExpressionAggregatePositions == null)
				{
					expressionInfo.TransformedExpressionAggregatePositions = new IntList();
					expressionInfo.TransformedExpressionAggregateIDs = new StringList();
				}
				expressionInfo.TransformedExpressionAggregatePositions.Add(text3.Length);
				expressionInfo.TransformedExpressionAggregateIDs.Add(text5);
				text3 = text3 + "Aggregates!" + text5;
			}
			GetReferencedFieldNames(text3, expressionInfo);
			GetReferencedReportItemNames(text3, expressionInfo);
			GetReferencedParameterNames(text3, expressionInfo);
			GetReferencedDataSetNames(text3, expressionInfo);
			GetReferencedDataSourceNames(text3, expressionInfo);
			expressionInfo.Type = ExpressionInfo.Types.Expression;
			expressionInfo.TransformedExpression = text3;
			if (m_context.ObjectType == ObjectType.Textbox && Detected(expressionInfo.TransformedExpression, m_regexes.MeDotValueDetection))
			{
				SetValueReferenced();
			}
		}

		private void EnforceRestrictions(ref string expression, bool isParameter, GrammarFlags grammarFlags)
		{
			if ((grammarFlags & GrammarFlags.DenyFields) != 0 && Detected(expression, m_regexes.FieldDetection))
			{
				if ((m_context.Location & LocationFlags.InPageSection) != 0)
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
					Global.Tracer.Assert(ExpressionType.ReportParameter == m_context.ExpressionType);
					m_errorContext.Register(ProcessingErrorCode.rsFieldInReportParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
			int num = NumberOfTimesDetected(expression, m_regexes.ReportItemsDetection);
			if ((grammarFlags & GrammarFlags.DenyReportItems) != 0 && 0 < num)
			{
				if (isParameter)
				{
					Global.Tracer.Assert((m_context.Location & LocationFlags.InPageSection) == 0);
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
				else
				{
					Global.Tracer.Assert(ExpressionType.SortExpression == m_context.ExpressionType);
					m_errorContext.Register(ProcessingErrorCode.rsReportItemInSortExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
			if ((m_context.Location & LocationFlags.InPageSection) != 0 && 1 < num)
			{
				m_errorContext.Register(ProcessingErrorCode.rsMultiReportItemsInPageSectionExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
			}
			if (0 < num)
			{
				if ((m_context.Location & LocationFlags.InPageSection) != 0)
				{
					m_pageSectionRefersToReportItems = true;
				}
				else
				{
					m_bodyRefersToReportItems = true;
				}
			}
			if ((grammarFlags & GrammarFlags.DenyPageGlobals) != 0 && Detected(expression, m_regexes.PageGlobalsDetection))
			{
				Global.Tracer.Assert((m_context.Location & LocationFlags.InPageSection) == 0);
				m_errorContext.Register(ProcessingErrorCode.rsPageNumberInBody, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
			}
			if (Detected(expression, m_regexes.AggregatesDetection))
			{
				m_errorContext.Register(ProcessingErrorCode.rsGlobalNotDefined, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
			}
			if ((grammarFlags & GrammarFlags.DenyDataSets) != 0 && Detected(expression, m_regexes.DataSetsDetection))
			{
				if ((m_context.Location & LocationFlags.InPageSection) != 0)
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
					Global.Tracer.Assert(ExpressionType.ReportParameter == m_context.ExpressionType);
					m_errorContext.Register(ProcessingErrorCode.rsDataSetInReportParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
			if ((grammarFlags & GrammarFlags.DenyDataSources) != 0 && Detected(expression, m_regexes.DataSourcesDetection))
			{
				if ((m_context.Location & LocationFlags.InPageSection) != 0)
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
					Global.Tracer.Assert(ExpressionType.ReportParameter == m_context.ExpressionType);
					m_errorContext.Register(ProcessingErrorCode.rsDataSourceInReportParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
			RemoveLineTerminators(ref expression, m_regexes.LineTerminatorDetection);
			if (Detected(expression, m_regexes.IllegalCharacterDetection))
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidCharacterInExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
			}
		}

		private void GetReferencedReportItemNames(string expression, ExpressionInfo expressionInfo)
		{
			MatchCollection matchCollection = m_regexes.ReportItemName.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				string text = matchCollection[i].Result("${reportitemname}");
				if (text != null && text.Length != 0)
				{
					expressionInfo.AddReferencedReportItem(text);
				}
			}
		}

		private void GetReferencedFieldNames(string expression, ExpressionInfo expressionInfo)
		{
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
		}

		private void GetReferencedParameterNames(string expression, ExpressionInfo expressionInfo)
		{
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

		private void GetReferencedDataSetNames(string expression, ExpressionInfo expressionInfo)
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

		private void GetReferencedDataSourceNames(string expression, ExpressionInfo expressionInfo)
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

		private void GetRunningValue(int currentPos, string functionName, string expression, bool isParameter, GrammarFlags grammarFlags, out int newPos, out RunningValueInfo runningValue)
		{
			if ((grammarFlags & GrammarFlags.DenyRunningValue) != 0)
			{
				if (isParameter)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateofAggregate, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.DataSetFilters == m_context.ExpressionType || ExpressionType.DataRegionFilters == m_context.ExpressionType || ExpressionType.GroupingFilters == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRunningValueInFilterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.GroupExpression == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRunningValueInGroupExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if ((m_context.Location & LocationFlags.InPageSection) != 0)
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
				else
				{
					Global.Tracer.Assert(ExpressionType.SortExpression == m_context.ExpressionType);
					m_errorContext.Register(ProcessingErrorCode.rsRunningValueInSortExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
			GetArguments(currentPos, expression, out newPos, out List<string> arguments);
			if (3 != arguments.Count)
			{
				m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, functionName);
			}
			runningValue = new RunningValueInfo();
			if (2 <= arguments.Count)
			{
				bool flag;
				try
				{
					runningValue.AggregateType = (DataAggregateInfo.AggregateTypes)Enum.Parse(typeof(DataAggregateInfo.AggregateTypes), arguments[1], ignoreCase: true);
					flag = (DataAggregateInfo.AggregateTypes.Aggregate != runningValue.AggregateType && DataAggregateInfo.AggregateTypes.Previous != runningValue.AggregateType && DataAggregateInfo.AggregateTypes.CountRows != runningValue.AggregateType);
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
				if (DataAggregateInfo.AggregateTypes.Count == runningValue.AggregateType && "*" == arguments[0].Trim())
				{
					m_errorContext.Register(ProcessingErrorCode.rsCountStarRVNotSupported, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else
				{
					runningValue.Expressions = new ExpressionInfo[1];
					runningValue.Expressions[0] = GetParameterExpression(arguments[0], grammarFlags);
				}
			}
			if (3 <= arguments.Count)
			{
				runningValue.Scope = GetScope(arguments[2], allowNothing: true);
			}
			m_numberOfRunningValues++;
		}

		private void GetPreviousAggregate(int currentPos, string functionName, string expression, bool isParameter, GrammarFlags grammarFlags, out int newPos, out RunningValueInfo runningValue)
		{
			if ((grammarFlags & GrammarFlags.DenyPrevious) != 0)
			{
				if (isParameter)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateofAggregate, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.DataSetFilters == m_context.ExpressionType || ExpressionType.DataRegionFilters == m_context.ExpressionType || ExpressionType.GroupingFilters == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInFilterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.GroupExpression == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInGroupExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if ((m_context.Location & LocationFlags.InPageSection) != 0)
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
				else
				{
					Global.Tracer.Assert(ExpressionType.SortExpression == m_context.ExpressionType);
					m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInSortExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
			GetArguments(currentPos, expression, out newPos, out List<string> arguments);
			if (1 != arguments.Count)
			{
				m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, functionName);
			}
			runningValue = new RunningValueInfo();
			runningValue.AggregateType = DataAggregateInfo.AggregateTypes.Previous;
			if (1 <= arguments.Count)
			{
				runningValue.Expressions = new ExpressionInfo[1];
				runningValue.Expressions[0] = GetParameterExpression(arguments[0], grammarFlags);
			}
			m_numberOfRunningValues++;
		}

		private void GetRowNumber(int currentPos, string functionName, string expression, bool isParameter, GrammarFlags grammarFlags, out int newPos, out RunningValueInfo rowNumber)
		{
			if ((grammarFlags & GrammarFlags.DenyRowNumber) != 0)
			{
				if (isParameter)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateofAggregate, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.DataSetFilters == m_context.ExpressionType || ExpressionType.DataRegionFilters == m_context.ExpressionType || ExpressionType.GroupingFilters == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsRowNumberInFilterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if ((m_context.Location & LocationFlags.InPageSection) != 0)
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
				else
				{
					Global.Tracer.Assert(ExpressionType.SortExpression == m_context.ExpressionType);
					m_errorContext.Register(ProcessingErrorCode.rsRowNumberInSortExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
			GetArguments(currentPos, expression, out newPos, out List<string> arguments);
			if (1 != arguments.Count)
			{
				m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, functionName);
			}
			rowNumber = new RunningValueInfo();
			rowNumber.AggregateType = DataAggregateInfo.AggregateTypes.CountRows;
			rowNumber.Expressions = new ExpressionInfo[0];
			if (1 <= arguments.Count)
			{
				rowNumber.Scope = GetScope(arguments[0], allowNothing: true);
			}
			m_numberOfRunningValues++;
		}

		private string GetScope(string expression, bool allowNothing)
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
			m_errorContext.Register(ProcessingErrorCode.rsInvalidAggregateScope, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
			return null;
		}

		private bool IsRecursive(string flag)
		{
			RecursiveFlags recursiveFlags = RecursiveFlags.Simple;
			try
			{
				recursiveFlags = (RecursiveFlags)Enum.Parse(typeof(RecursiveFlags), flag, ignoreCase: true);
			}
			catch
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidAggregateRecursiveFlag, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
			}
			if (RecursiveFlags.Recursive == recursiveFlags)
			{
				return true;
			}
			return false;
		}

		private void GetAggregate(int currentPos, string functionName, string expression, bool isParameter, GrammarFlags grammarFlags, out int newPos, out DataAggregateInfo aggregate)
		{
			if ((grammarFlags & GrammarFlags.DenyAggregates) != 0)
			{
				if (isParameter)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateofAggregate, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.DataSetFilters == m_context.ExpressionType || ExpressionType.DataRegionFilters == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateInFilterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.GroupExpression == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsAggregateInGroupExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
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
				else
				{
					Global.Tracer.Assert(ExpressionType.ReportParameter == m_context.ExpressionType);
					m_errorContext.Register(ProcessingErrorCode.rsAggregateInReportParameterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
			GetArguments(currentPos, expression, out newPos, out List<string> arguments);
			if (arguments.Count != 0 && 1 != arguments.Count && 2 != arguments.Count && 3 != arguments.Count)
			{
				m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, functionName);
			}
			aggregate = new DataAggregateInfo();
			aggregate.AggregateType = (DataAggregateInfo.AggregateTypes)Enum.Parse(typeof(DataAggregateInfo.AggregateTypes), functionName, ignoreCase: true);
			if ((grammarFlags & GrammarFlags.DenyPostSortAggregate) != 0 && aggregate.IsPostSortAggregate())
			{
				if (ExpressionType.GroupingFilters == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsPostSortAggregateInGroupFilterExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
				else if (ExpressionType.SortExpression == m_context.ExpressionType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsPostSortAggregateInSortExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
				}
			}
			if (DataAggregateInfo.AggregateTypes.CountRows == aggregate.AggregateType)
			{
				aggregate.AggregateType = DataAggregateInfo.AggregateTypes.CountRows;
				aggregate.Expressions = new ExpressionInfo[0];
				if (1 == arguments.Count)
				{
					aggregate.SetScope(GetScope(arguments[0], allowNothing: false));
				}
				else if (2 == arguments.Count)
				{
					aggregate.Recursive = IsRecursive(arguments[1]);
				}
				else if (arguments.Count != 0)
				{
					m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName, functionName);
				}
				if (DataAggregateInfo.AggregateTypes.CountRows == aggregate.AggregateType && (m_context.Location & LocationFlags.InPageSection) != 0)
				{
					m_errorContext.Register(ProcessingErrorCode.rsCountRowsInPageSectionExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
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
					if (DataAggregateInfo.AggregateTypes.Count == aggregate.AggregateType && "*" == arguments[0].Trim())
					{
						m_errorContext.Register(ProcessingErrorCode.rsCountStarNotSupported, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
					}
					else
					{
						aggregate.Expressions = new ExpressionInfo[1];
						aggregate.Expressions[0] = GetParameterExpression(arguments[0], grammarFlags);
						if (DataAggregateInfo.AggregateTypes.Aggregate == aggregate.AggregateType && ExpressionInfo.Types.Field != aggregate.Expressions[0].Type)
						{
							m_errorContext.Register(ProcessingErrorCode.rsInvalidCustomAggregateExpression, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
						}
					}
				}
				if (2 <= arguments.Count)
				{
					aggregate.SetScope(GetScope(arguments[1], allowNothing: false));
				}
				if (3 <= arguments.Count)
				{
					if (aggregate.IsPostSortAggregate() || DataAggregateInfo.AggregateTypes.Aggregate == aggregate.AggregateType)
					{
						m_errorContext.Register(ProcessingErrorCode.rsInvalidRecursiveAggregate, Severity.Error, m_context.ObjectType, m_context.ObjectName, m_context.PropertyName);
					}
					else
					{
						aggregate.Recursive = IsRecursive(arguments[2]);
					}
				}
			}
			m_numberOfAggregates++;
		}

		private ExpressionInfo GetParameterExpression(string parameterExpression, GrammarFlags grammarFlags)
		{
			ExpressionInfo expressionInfo = new ExpressionInfo();
			expressionInfo.OriginalText = parameterExpression;
			grammarFlags = (((m_context.Location & LocationFlags.InPageSection) == 0) ? (grammarFlags | (GrammarFlags.DenyAggregates | GrammarFlags.DenyRunningValue | GrammarFlags.DenyRowNumber | GrammarFlags.DenyReportItems | GrammarFlags.DenyPrevious)) : (grammarFlags | (GrammarFlags.DenyAggregates | GrammarFlags.DenyRunningValue | GrammarFlags.DenyRowNumber | GrammarFlags.DenyPrevious)));
			VBLex(parameterExpression, isParameter: true, grammarFlags, expressionInfo);
			return expressionInfo;
		}

		private void GetArguments(int currentPos, string expression, out int newPos, out List<string> arguments)
		{
			int num = 1;
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
				else if (text4 != null && text4.Length != 0)
				{
					if (1 == num)
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
			return "Aggregate" + (m_numberOfAggregates + m_numberOfRunningValues);
		}
	}
}

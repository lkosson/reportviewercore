using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Globalization;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class ExprHostBuilder
	{
		internal enum ErrorSource
		{
			Expression,
			CodeModuleClassInstanceDecl,
			CustomCode,
			Unknown
		}

		private static class Constants
		{
			internal const string ReportObjectModelNS = "Microsoft.ReportingServices.ReportProcessing.ReportObjectModel";

			internal const string ExprHostObjectModelNS = "Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel";

			internal const string ReportExprHost = "ReportExprHost";

			internal const string IndexedExprHost = "IndexedExprHost";

			internal const string ReportParamExprHost = "ReportParamExprHost";

			internal const string CalcFieldExprHost = "CalcFieldExprHost";

			internal const string DataSourceExprHost = "DataSourceExprHost";

			internal const string DataSetExprHost = "DataSetExprHost";

			internal const string ReportItemExprHost = "ReportItemExprHost";

			internal const string ActionExprHost = "ActionExprHost";

			internal const string ActionInfoExprHost = "ActionInfoExprHost";

			internal const string TextBoxExprHost = "TextBoxExprHost";

			internal const string ImageExprHost = "ImageExprHost";

			internal const string ParamExprHost = "ParamExprHost";

			internal const string SubreportExprHost = "SubreportExprHost";

			internal const string ActiveXControlExprHost = "ActiveXControlExprHost";

			internal const string SortingExprHost = "SortingExprHost";

			internal const string FilterExprHost = "FilterExprHost";

			internal const string GroupingExprHost = "GroupingExprHost";

			internal const string ListExprHost = "ListExprHost";

			internal const string TableGroupExprHost = "TableGroupExprHost";

			internal const string TableExprHost = "TableExprHost";

			internal const string MatrixDynamicGroupExprHost = "MatrixDynamicGroupExprHost";

			internal const string MatrixExprHost = "MatrixExprHost";

			internal const string ChartExprHost = "ChartExprHost";

			internal const string OWCChartExprHost = "OWCChartExprHost";

			internal const string StyleExprHost = "StyleExprHost";

			internal const string AggregateParamExprHost = "AggregateParamExprHost";

			internal const string MultiChartExprHost = "MultiChartExprHost";

			internal const string ChartDynamicGroupExprHost = "ChartDynamicGroupExprHost";

			internal const string ChartDataPointExprHost = "ChartDataPointExprHost";

			internal const string ChartTitleExprHost = "ChartTitleExprHost";

			internal const string AxisExprHost = "AxisExprHost";

			internal const string DataValueExprHost = "DataValueExprHost";

			internal const string CustomReportItemExprHost = "CustomReportItemExprHost";

			internal const string DataGroupingExprHost = "DataGroupingExprHost";

			internal const string DataCellExprHost = "DataCellExprHost";

			internal const string ParametersOnlyParam = "parametersOnly";

			internal const string CustomCodeProxy = "CustomCodeProxy";

			internal const string CustomCodeProxyBase = "CustomCodeProxyBase";

			internal const string ReportObjectModelParam = "reportObjectModel";

			internal const string SetReportObjectModel = "SetReportObjectModel";

			internal const string Code = "Code";

			internal const string CodeProxyBase = "m_codeProxyBase";

			internal const string CodeParam = "code";

			internal const string Report = "Report";

			internal const string RemoteArrayWrapper = "RemoteArrayWrapper";

			internal const string LabelExpr = "LabelExpr";

			internal const string ValueExpr = "ValueExpr";

			internal const string NoRowsExpr = "NoRowsExpr";

			internal const string ParameterHosts = "m_parameterHostsRemotable";

			internal const string IndexParam = "index";

			internal const string FilterHosts = "m_filterHostsRemotable";

			internal const string SortingHost = "SortingHost";

			internal const string GroupingHost = "GroupingHost";

			internal const string SubgroupHost = "SubgroupHost";

			internal const string VisibilityHiddenExpr = "VisibilityHiddenExpr";

			internal const string SortDirectionHosts = "SortDirectionHosts";

			internal const string DataValueHosts = "m_dataValueHostsRemotable";

			internal const string CustomPropertyHosts = "m_customPropertyHostsRemotable";

			internal const string ReportLanguageExpr = "ReportLanguageExpr";

			internal const string AggregateParamHosts = "m_aggregateParamHostsRemotable";

			internal const string ReportParameterHosts = "m_reportParameterHostsRemotable";

			internal const string DataSourceHosts = "m_dataSourceHostsRemotable";

			internal const string DataSetHosts = "m_dataSetHostsRemotable";

			internal const string PageSectionHosts = "m_pageSectionHostsRemotable";

			internal const string LineHosts = "m_lineHostsRemotable";

			internal const string RectangleHosts = "m_rectangleHostsRemotable";

			internal const string TextBoxHosts = "m_textBoxHostsRemotable";

			internal const string ImageHosts = "m_imageHostsRemotable";

			internal const string SubreportHosts = "m_subreportHostsRemotable";

			internal const string ActiveXControlHosts = "m_activeXControlHostsRemotable";

			internal const string ListHosts = "m_listHostsRemotable";

			internal const string TableHosts = "m_tableHostsRemotable";

			internal const string MatrixHosts = "m_matrixHostsRemotable";

			internal const string ChartHosts = "m_chartHostsRemotable";

			internal const string OWCChartHosts = "m_OWCChartHostsRemotable";

			internal const string CustomReportItemHosts = "m_customReportItemHostsRemotable";

			internal const string ConnectStringExpr = "ConnectStringExpr";

			internal const string FieldHosts = "m_fieldHostsRemotable";

			internal const string QueryParametersHost = "QueryParametersHost";

			internal const string QueryCommandTextExpr = "QueryCommandTextExpr";

			internal const string ValidValuesHost = "ValidValuesHost";

			internal const string ValidValueLabelsHost = "ValidValueLabelsHost";

			internal const string ValidationExpressionExpr = "ValidationExpressionExpr";

			internal const string ActionInfoHost = "ActionInfoHost";

			internal const string ActionHost = "ActionHost";

			internal const string ActionItemHosts = "m_actionItemHostsRemotable";

			internal const string BookmarkExpr = "BookmarkExpr";

			internal const string ToolTipExpr = "ToolTipExpr";

			internal const string ToggleImageInitialStateExpr = "ToggleImageInitialStateExpr";

			internal const string UserSortExpressionsHost = "UserSortExpressionsHost";

			internal const string MIMETypeExpr = "MIMETypeExpr";

			internal const string OmitExpr = "OmitExpr";

			internal const string HyperlinkExpr = "HyperlinkExpr";

			internal const string DrillThroughReportNameExpr = "DrillThroughReportNameExpr";

			internal const string DrillThroughParameterHosts = "m_drillThroughParameterHostsRemotable";

			internal const string DrillThroughBookmakLinkExpr = "DrillThroughBookmarkLinkExpr";

			internal const string BookmarkLinkExpr = "BookmarkLinkExpr";

			internal const string FilterExpressionExpr = "FilterExpressionExpr";

			internal const string ParentExpressionsHost = "ParentExpressionsHost";

			internal const string SubGroupHost = "SubGroupHost";

			internal const string SubtotalHost = "SubtotalHost";

			internal const string RowGroupingsHost = "RowGroupingsHost";

			internal const string ColumnGroupingsHost = "ColumnGroupingsHost";

			internal const string SeriesGroupingsHost = "SeriesGroupingsHost";

			internal const string CategoryGroupingsHost = "CategoryGroupingsHost";

			internal const string MultiChartHost = "MultiChartHost";

			internal const string HeadingLabelExpr = "HeadingLabelExpr";

			internal const string ChartDataPointHosts = "m_chartDataPointHostsRemotable";

			internal const string DataLabelValueExpr = "DataLabelValueExpr";

			internal const string DataLabelStyleHost = "DataLabelStyleHost";

			internal const string StyleHost = "StyleHost";

			internal const string MarkerStyleHost = "MarkerStyleHost";

			internal const string TitleHost = "TitleHost";

			internal const string CaptionExpr = "CaptionExpr";

			internal const string MajorGridLinesHost = "MajorGridLinesHost";

			internal const string MinorGridLinesHost = "MinorGridLinesHost";

			internal const string StaticRowLabelsHost = "StaticRowLabelsHost";

			internal const string StaticColumnLabelsHost = "StaticColumnLabelsHost";

			internal const string CategoryAxisHost = "CategoryAxisHost";

			internal const string ValueAxisHost = "ValueAxisHost";

			internal const string LegendHost = "LegendHost";

			internal const string PlotAreaHost = "PlotAreaHost";

			internal const string AxisMinExpr = "AxisMinExpr";

			internal const string AxisMaxExpr = "AxisMaxExpr";

			internal const string AxisCrossAtExpr = "AxisCrossAtExpr";

			internal const string AxisMajorIntervalExpr = "AxisMajorIntervalExpr";

			internal const string AxisMinorIntervalExpr = "AxisMinorIntervalExpr";

			internal const string TableGroupsHost = "TableGroupsHost";

			internal const string TableRowVisibilityHiddenExpressions = "TableRowVisibilityHiddenExpressions";

			internal const string TableColumnVisibilityHiddenExpressions = "TableColumnVisibilityHiddenExpressions";

			internal const string OWCChartColumnHosts = "OWCChartColumnHosts";

			internal const string DataValueNameExpr = "DataValueNameExpr";

			internal const string DataValueValueExpr = "DataValueValueExpr";

			internal const string DataGroupingHosts = "m_dataGroupingHostsRemotable";

			internal const string DataCellHosts = "m_dataCellHostsRemotable";
		}

		private abstract class TypeDecl
		{
			internal CodeTypeDeclaration Type;

			internal string BaseTypeName;

			internal TypeDecl Parent;

			internal CodeConstructor Constructor;

			internal bool HasExpressions;

			internal CodeExpressionCollection DataValues;

			protected bool m_setCode;

			internal void NestedTypeAdd(string name, CodeTypeDeclaration nestedType)
			{
				ConstructorCreate();
				Type.Members.Add(nestedType);
				Constructor.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), name), CreateTypeCreateExpression(nestedType.Name)));
			}

			internal int NestedTypeColAdd(string name, string baseTypeName, ref CodeExpressionCollection initializers, CodeTypeDeclaration nestedType)
			{
				Type.Members.Add(nestedType);
				TypeColInit(name, baseTypeName, ref initializers);
				return initializers.Add(CreateTypeCreateExpression(nestedType.Name));
			}

			protected TypeDecl(string typeName, string baseTypeName, TypeDecl parent, bool setCode)
			{
				BaseTypeName = baseTypeName;
				Parent = parent;
				m_setCode = setCode;
				Type = CreateType(typeName, baseTypeName);
			}

			protected void ConstructorCreate()
			{
				if (Constructor == null)
				{
					Constructor = CreateConstructor();
					Type.Members.Add(Constructor);
				}
			}

			protected virtual CodeConstructor CreateConstructor()
			{
				return new CodeConstructor
				{
					Attributes = MemberAttributes.Public
				};
			}

			protected CodeAssignStatement CreateTypeColInitStatement(string name, string baseTypeName, ref CodeExpressionCollection initializers)
			{
				CodeObjectCreateExpression codeObjectCreateExpression = new CodeObjectCreateExpression();
				codeObjectCreateExpression.CreateType = new CodeTypeReference("RemoteArrayWrapper", new CodeTypeReference(baseTypeName));
				if (initializers != null)
				{
					codeObjectCreateExpression.Parameters.AddRange(initializers);
				}
				initializers = codeObjectCreateExpression.Parameters;
				return new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), name), codeObjectCreateExpression);
			}

			protected virtual CodeTypeDeclaration CreateType(string name, string baseType)
			{
				Global.Tracer.Assert(name != null);
				CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(name);
				if (baseType != null)
				{
					codeTypeDeclaration.BaseTypes.Add(new CodeTypeReference(baseType));
				}
				codeTypeDeclaration.Attributes = (MemberAttributes)24578;
				return codeTypeDeclaration;
			}

			private void TypeColInit(string name, string baseTypeName, ref CodeExpressionCollection initializers)
			{
				ConstructorCreate();
				if (initializers == null)
				{
					Constructor.Statements.Add(CreateTypeColInitStatement(name, baseTypeName, ref initializers));
				}
			}

			private CodeObjectCreateExpression CreateTypeCreateExpression(string typeName)
			{
				if (m_setCode)
				{
					return new CodeObjectCreateExpression(typeName, new CodeArgumentReferenceExpression("Code"));
				}
				return new CodeObjectCreateExpression(typeName);
			}
		}

		private sealed class RootTypeDecl : TypeDecl
		{
			internal CodeExpressionCollection Aggregates;

			internal CodeExpressionCollection PageSections;

			internal CodeExpressionCollection ReportParameters;

			internal CodeExpressionCollection DataSources;

			internal CodeExpressionCollection DataSets;

			internal CodeExpressionCollection Lines;

			internal CodeExpressionCollection Rectangles;

			internal CodeExpressionCollection TextBoxes;

			internal CodeExpressionCollection Images;

			internal CodeExpressionCollection Subreports;

			internal CodeExpressionCollection ActiveXControls;

			internal CodeExpressionCollection Lists;

			internal CodeExpressionCollection Tables;

			internal CodeExpressionCollection Matrices;

			internal CodeExpressionCollection Charts;

			internal CodeExpressionCollection OWCCharts;

			internal CodeExpressionCollection CustomReportItems;

			internal RootTypeDecl(bool setCode)
				: base("ReportExprHostImpl", "ReportExprHost", null, setCode)
			{
			}

			protected override CodeConstructor CreateConstructor()
			{
				CodeConstructor codeConstructor = base.CreateConstructor();
				codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "parametersOnly"));
				CodeParameterDeclarationExpression value = new CodeParameterDeclarationExpression(typeof(object), "reportObjectModel");
				codeConstructor.Parameters.Add(value);
				codeConstructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("reportObjectModel"));
				ReportParameters = new CodeExpressionCollection();
				DataSources = new CodeExpressionCollection();
				DataSets = new CodeExpressionCollection();
				return codeConstructor;
			}

			protected override CodeTypeDeclaration CreateType(string name, string baseType)
			{
				CodeTypeDeclaration codeTypeDeclaration = base.CreateType(name, baseType);
				if (m_setCode)
				{
					CodeMemberField codeMemberField = new CodeMemberField("CustomCodeProxy", "Code");
					codeMemberField.Attributes = (MemberAttributes)20482;
					codeTypeDeclaration.Members.Add(codeMemberField);
				}
				return codeTypeDeclaration;
			}

			internal void CompleteConstructorCreation()
			{
				if (!HasExpressions)
				{
					return;
				}
				if (Constructor == null)
				{
					ConstructorCreate();
					return;
				}
				CodeConditionStatement codeConditionStatement = new CodeConditionStatement();
				codeConditionStatement.Condition = new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("parametersOnly"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(true));
				if (ReportParameters.Count > 0)
				{
					codeConditionStatement.TrueStatements.Add(CreateTypeColInitStatement("m_reportParameterHostsRemotable", "ReportParamExprHost", ref ReportParameters));
				}
				codeConditionStatement.TrueStatements.Add(new CodeMethodReturnStatement());
				Constructor.Statements.Insert(0, codeConditionStatement);
				if (DataSources.Count > 0)
				{
					Constructor.Statements.Insert(0, CreateTypeColInitStatement("m_dataSourceHostsRemotable", "DataSourceExprHost", ref DataSources));
				}
				if (DataSets.Count > 0)
				{
					Constructor.Statements.Insert(0, CreateTypeColInitStatement("m_dataSetHostsRemotable", "DataSetExprHost", ref DataSets));
				}
				if (m_setCode)
				{
					Constructor.Statements.Insert(0, new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "m_codeProxyBase"), new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Code")));
					Constructor.Statements.Insert(0, new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Code"), new CodeObjectCreateExpression("CustomCodeProxy", new CodeThisReferenceExpression())));
				}
			}
		}

		private sealed class NonRootTypeDecl : TypeDecl
		{
			internal CodeExpressionCollection Parameters;

			internal CodeExpressionCollection Filters;

			internal CodeExpressionCollection Actions;

			internal CodeExpressionCollection Fields;

			internal CodeExpressionCollection DataPoints;

			internal CodeExpressionCollection DataGroupings;

			internal CodeExpressionCollection DataCells;

			internal ReturnStatementList IndexedExpressions;

			internal NonRootTypeDecl(string typeName, string baseTypeName, TypeDecl parent, bool setCode)
				: base(typeName, baseTypeName, parent, setCode)
			{
				if (setCode)
				{
					ConstructorCreate();
				}
			}

			protected override CodeConstructor CreateConstructor()
			{
				CodeConstructor codeConstructor = base.CreateConstructor();
				if (m_setCode)
				{
					codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression("CustomCodeProxy", "code"));
					codeConstructor.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Code"), new CodeArgumentReferenceExpression("code")));
				}
				return codeConstructor;
			}

			protected override CodeTypeDeclaration CreateType(string name, string baseType)
			{
				CodeTypeDeclaration codeTypeDeclaration = base.CreateType(string.Format(CultureInfo.InvariantCulture, "{0}_{1}", name, baseType), baseType);
				if (m_setCode)
				{
					CodeMemberField codeMemberField = new CodeMemberField("CustomCodeProxy", "Code");
					codeMemberField.Attributes = (MemberAttributes)20482;
					codeTypeDeclaration.Members.Add(codeMemberField);
				}
				return codeTypeDeclaration;
			}
		}

		private sealed class CustomCodeProxyDecl : TypeDecl
		{
			internal CustomCodeProxyDecl(TypeDecl parent)
				: base("CustomCodeProxy", "CustomCodeProxyBase", parent, setCode: false)
			{
				ConstructorCreate();
			}

			protected override CodeConstructor CreateConstructor()
			{
				CodeConstructor codeConstructor = base.CreateConstructor();
				codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IReportObjectModelProxyForCustomCode), "reportObjectModel"));
				codeConstructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("reportObjectModel"));
				return codeConstructor;
			}

			internal void AddClassInstance(string className, string instanceName, int id)
			{
				string fileName = "CMCID" + id.ToString(CultureInfo.InvariantCulture) + "end";
				CodeMemberField codeMemberField = new CodeMemberField(className, "m_" + instanceName);
				codeMemberField.Attributes = (MemberAttributes)20482;
				codeMemberField.InitExpression = new CodeObjectCreateExpression(className);
				codeMemberField.LinePragma = new CodeLinePragma(fileName, 0);
				Type.Members.Add(codeMemberField);
				CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
				codeMemberProperty.Type = new CodeTypeReference(className);
				codeMemberProperty.Name = instanceName;
				codeMemberProperty.Attributes = (MemberAttributes)24578;
				codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), codeMemberField.Name)));
				codeMemberProperty.LinePragma = new CodeLinePragma(fileName, 2);
				Type.Members.Add(codeMemberProperty);
			}

			internal void AddCode(string code)
			{
				CodeTypeMember codeTypeMember = new CodeSnippetTypeMember(code);
				codeTypeMember.LinePragma = new CodeLinePragma("CustomCode", 0);
				Type.Members.Add(codeTypeMember);
			}
		}

		private sealed class ReturnStatementList
		{
			private ArrayList m_list = new ArrayList();

			internal CodeMethodReturnStatement this[int index] => (CodeMethodReturnStatement)m_list[index];

			internal int Count => m_list.Count;

			internal int Add(CodeMethodReturnStatement retStatement)
			{
				return m_list.Add(retStatement);
			}
		}

		internal const string RootType = "ReportExprHostImpl";

		private RootTypeDecl m_rootTypeDecl;

		private TypeDecl m_currentTypeDecl;

		private bool m_setCode;

		private const string EndSrcMarker = "end";

		private const string ExprSrcMarker = "Expr";

		private static readonly Regex m_findExprNumber = new Regex("^Expr([0-9]+)end", RegexOptions.Compiled);

		private const string CustomCodeSrcMarker = "CustomCode";

		private const string CodeModuleClassInstanceDeclSrcMarker = "CMCID";

		private static readonly Regex m_findCodeModuleClassInstanceDeclNumber = new Regex("^CMCID([0-9]+)end", RegexOptions.Compiled);

		internal bool HasExpressions
		{
			get
			{
				if (m_rootTypeDecl != null)
				{
					return m_rootTypeDecl.HasExpressions;
				}
				return false;
			}
		}

		internal bool CustomCode => m_setCode;

		internal ExprHostBuilder()
		{
		}

		internal void SetCustomCode()
		{
			m_setCode = true;
		}

		internal CodeCompileUnit GetExprHost(IntermediateFormatVersion version, bool refusePermissions)
		{
			Global.Tracer.Assert(m_rootTypeDecl != null && m_currentTypeDecl.Parent == null, "(m_rootTypeDecl != null && m_currentTypeDecl.Parent == null)");
			CodeCompileUnit codeCompileUnit = null;
			if (HasExpressions)
			{
				codeCompileUnit = new CodeCompileUnit();
				codeCompileUnit.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration("System.Reflection.AssemblyVersion", new CodeAttributeArgument(new CodePrimitiveExpression(version.ToString()))));
				if (refusePermissions)
				{
					codeCompileUnit.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration("System.Security.Permissions.SecurityPermission", new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SecurityAction)), "RequestMinimum")), new CodeAttributeArgument("Execution", new CodePrimitiveExpression(true))));
					codeCompileUnit.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration("System.Security.Permissions.SecurityPermission", new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SecurityAction)), "RequestOptional")), new CodeAttributeArgument("Execution", new CodePrimitiveExpression(true))));
				}
				CodeNamespace codeNamespace = new CodeNamespace();
				codeCompileUnit.Namespaces.Add(codeNamespace);
				codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
				codeNamespace.Imports.Add(new CodeNamespaceImport("System.Convert"));
				codeNamespace.Imports.Add(new CodeNamespaceImport("System.Math"));
				codeNamespace.Imports.Add(new CodeNamespaceImport("Microsoft.VisualBasic"));
				codeNamespace.Imports.Add(new CodeNamespaceImport("Microsoft.ReportingServices.ReportProcessing.ReportObjectModel"));
				codeNamespace.Imports.Add(new CodeNamespaceImport("Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel"));
				codeNamespace.Types.Add(m_rootTypeDecl.Type);
			}
			m_rootTypeDecl = null;
			return codeCompileUnit;
		}

		internal ErrorSource ParseErrorSource(CompilerError error, out int id)
		{
			Global.Tracer.Assert(error.FileName != null, "(error.FileName != null)");
			id = -1;
			if (error.FileName.StartsWith("CustomCode", StringComparison.Ordinal))
			{
				return ErrorSource.CustomCode;
			}
			Match match = m_findCodeModuleClassInstanceDeclNumber.Match(error.FileName);
			if (match.Success && int.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
			{
				return ErrorSource.CodeModuleClassInstanceDecl;
			}
			match = m_findExprNumber.Match(error.FileName);
			if (match.Success && int.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
			{
				return ErrorSource.Expression;
			}
			return ErrorSource.Unknown;
		}

		internal void ReportStart()
		{
			m_currentTypeDecl = (m_rootTypeDecl = new RootTypeDecl(m_setCode));
		}

		internal void ReportEnd()
		{
			m_rootTypeDecl.CompleteConstructorCreation();
		}

		internal void ReportLanguage(ExpressionInfo expression)
		{
			ExpressionAdd("ReportLanguageExpr", expression);
		}

		internal void GenericLabel(ExpressionInfo expression)
		{
			ExpressionAdd("LabelExpr", expression);
		}

		internal void GenericValue(ExpressionInfo expression)
		{
			ExpressionAdd("ValueExpr", expression);
		}

		internal void GenericNoRows(ExpressionInfo expression)
		{
			ExpressionAdd("NoRowsExpr", expression);
		}

		internal void GenericVisibilityHidden(ExpressionInfo expression)
		{
			ExpressionAdd("VisibilityHiddenExpr", expression);
		}

		internal void AggregateParamExprAdd(ExpressionInfo expression)
		{
			AggregateStart();
			GenericValue(expression);
			expression.ExprHostID = AggregateEnd();
		}

		internal void CustomCodeProxyStart()
		{
			Global.Tracer.Assert(m_setCode, "(m_setCode)");
			m_currentTypeDecl = new CustomCodeProxyDecl(m_currentTypeDecl);
		}

		internal void CustomCodeProxyEnd()
		{
			m_rootTypeDecl.Type.Members.Add(m_currentTypeDecl.Type);
			TypeEnd(m_rootTypeDecl);
		}

		internal void CustomCodeClassInstance(string className, string instanceName, int id)
		{
			((CustomCodeProxyDecl)m_currentTypeDecl).AddClassInstance(className, instanceName, id);
		}

		internal void ReportCode(string code)
		{
			((CustomCodeProxyDecl)m_currentTypeDecl).AddCode(code);
		}

		internal void ReportParameterStart(string name)
		{
			TypeStart(name, "ReportParamExprHost");
		}

		internal int ReportParameterEnd()
		{
			ExprIndexerCreate();
			return TypeEnd(m_rootTypeDecl, "m_reportParameterHostsRemotable", ref m_rootTypeDecl.ReportParameters);
		}

		internal void ReportParameterValidationExpression(ExpressionInfo expression)
		{
			ExpressionAdd("ValidationExpressionExpr", expression);
		}

		internal void ReportParameterDefaultValue(ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void ReportParameterValidValuesStart()
		{
			TypeStart("ReportParameterValidValues", "IndexedExprHost");
		}

		internal void ReportParameterValidValuesEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "ValidValuesHost");
		}

		internal void ReportParameterValidValue(ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void ReportParameterValidValueLabelsStart()
		{
			TypeStart("ReportParameterValidValueLabels", "IndexedExprHost");
		}

		internal void ReportParameterValidValueLabelsEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "ValidValueLabelsHost");
		}

		internal void ReportParameterValidValueLabel(ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void CalcFieldStart(string name)
		{
			TypeStart(name, "CalcFieldExprHost");
		}

		internal int CalcFieldEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_fieldHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).Fields);
		}

		internal void QueryParametersStart()
		{
			TypeStart("QueryParameters", "IndexedExprHost");
		}

		internal void QueryParametersEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "QueryParametersHost");
		}

		internal void QueryParameterValue(ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void DataSourceStart(string name)
		{
			TypeStart(name, "DataSourceExprHost");
		}

		internal int DataSourceEnd()
		{
			return TypeEnd(m_rootTypeDecl, "m_dataSourceHostsRemotable", ref m_rootTypeDecl.DataSources);
		}

		internal void DataSourceConnectString(ExpressionInfo expression)
		{
			ExpressionAdd("ConnectStringExpr", expression);
		}

		internal void DataSetStart(string name)
		{
			TypeStart(name, "DataSetExprHost");
		}

		internal int DataSetEnd()
		{
			return TypeEnd(m_rootTypeDecl, "m_dataSetHostsRemotable", ref m_rootTypeDecl.DataSets);
		}

		internal void DataSetQueryCommandText(ExpressionInfo expression)
		{
			ExpressionAdd("QueryCommandTextExpr", expression);
		}

		internal void PageSectionStart()
		{
			TypeStart(CreateTypeName("PageSection", m_rootTypeDecl.PageSections), "StyleExprHost");
		}

		internal int PageSectionEnd()
		{
			return TypeEnd(m_rootTypeDecl, "m_pageSectionHostsRemotable", ref m_rootTypeDecl.PageSections);
		}

		internal void ParameterOmit(ExpressionInfo expression)
		{
			ExpressionAdd("OmitExpr", expression);
		}

		internal void StyleAttribute(string name, ExpressionInfo expression)
		{
			ExpressionAdd(name + "Expr", expression);
		}

		internal void ActionInfoStart()
		{
			TypeStart("ActionInfo", "ActionInfoExprHost");
		}

		internal void ActionInfoEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "ActionInfoHost");
		}

		internal void ActionStart()
		{
			TypeStart(CreateTypeName("Action", ((NonRootTypeDecl)m_currentTypeDecl).Actions), "ActionExprHost");
		}

		internal int ActionEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_actionItemHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).Actions);
		}

		internal void ActionHyperlink(ExpressionInfo expression)
		{
			ExpressionAdd("HyperlinkExpr", expression);
		}

		internal void ActionDrillThroughReportName(ExpressionInfo expression)
		{
			ExpressionAdd("DrillThroughReportNameExpr", expression);
		}

		internal void ActionDrillThroughBookmarkLink(ExpressionInfo expression)
		{
			ExpressionAdd("DrillThroughBookmarkLinkExpr", expression);
		}

		internal void ActionBookmarkLink(ExpressionInfo expression)
		{
			ExpressionAdd("BookmarkLinkExpr", expression);
		}

		internal void ActionDrillThroughParameterStart()
		{
			ParameterStart();
		}

		internal int ActionDrillThroughParameterEnd()
		{
			return ParameterEnd("m_drillThroughParameterHostsRemotable");
		}

		internal void ReportItemBookmark(ExpressionInfo expression)
		{
			ExpressionAdd("BookmarkExpr", expression);
		}

		internal void ReportItemToolTip(ExpressionInfo expression)
		{
			ExpressionAdd("ToolTipExpr", expression);
		}

		internal void LineStart(string name)
		{
			TypeStart(name, "ReportItemExprHost");
		}

		internal int LineEnd()
		{
			return ReportItemEnd("m_lineHostsRemotable", ref m_rootTypeDecl.Lines);
		}

		internal void RectangleStart(string name)
		{
			TypeStart(name, "ReportItemExprHost");
		}

		internal int RectangleEnd()
		{
			return ReportItemEnd("m_rectangleHostsRemotable", ref m_rootTypeDecl.Rectangles);
		}

		internal void TextBoxStart(string name)
		{
			TypeStart(name, "TextBoxExprHost");
		}

		internal int TextBoxEnd()
		{
			return ReportItemEnd("m_textBoxHostsRemotable", ref m_rootTypeDecl.TextBoxes);
		}

		internal void TextBoxToggleImageInitialState(ExpressionInfo expression)
		{
			ExpressionAdd("ToggleImageInitialStateExpr", expression);
		}

		internal void UserSortExpressionsStart()
		{
			TypeStart("UserSort", "IndexedExprHost");
		}

		internal void UserSortExpressionsEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "UserSortExpressionsHost");
		}

		internal void UserSortExpression(ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void ImageStart(string name)
		{
			TypeStart(name, "ImageExprHost");
		}

		internal int ImageEnd()
		{
			return ReportItemEnd("m_imageHostsRemotable", ref m_rootTypeDecl.Images);
		}

		internal void ImageMIMEType(ExpressionInfo expression)
		{
			ExpressionAdd("MIMETypeExpr", expression);
		}

		internal void SubreportStart(string name)
		{
			TypeStart(name, "SubreportExprHost");
		}

		internal int SubreportEnd()
		{
			return ReportItemEnd("m_subreportHostsRemotable", ref m_rootTypeDecl.Subreports);
		}

		internal void SubreportParameterStart()
		{
			ParameterStart();
		}

		internal int SubreportParameterEnd()
		{
			return ParameterEnd("m_parameterHostsRemotable");
		}

		internal void ActiveXControlStart(string name)
		{
			TypeStart(name, "ActiveXControlExprHost");
		}

		internal int ActiveXControlEnd()
		{
			return ReportItemEnd("m_activeXControlHostsRemotable", ref m_rootTypeDecl.ActiveXControls);
		}

		internal void ActiveXControlParameterStart()
		{
			ParameterStart();
		}

		internal int ActiveXControlParameterEnd()
		{
			return ParameterEnd("m_parameterHostsRemotable");
		}

		internal void SortingStart()
		{
			TypeStart("Sorting", "SortingExprHost");
		}

		internal void SortingEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "SortingHost");
		}

		internal void SortingExpression(ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void SortDirectionsStart()
		{
			TypeStart("SortDirections", "IndexedExprHost");
		}

		internal void SortDirectionsEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "SortDirectionHosts");
		}

		internal void SortDirection(ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void FilterStart()
		{
			TypeStart(CreateTypeName("Filter", ((NonRootTypeDecl)m_currentTypeDecl).Filters), "FilterExprHost");
		}

		internal int FilterEnd()
		{
			ExprIndexerCreate();
			return TypeEnd(m_currentTypeDecl.Parent, "m_filterHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).Filters);
		}

		internal void FilterExpression(ExpressionInfo expression)
		{
			ExpressionAdd("FilterExpressionExpr", expression);
		}

		internal void FilterValue(ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void GroupingStart(string typeName)
		{
			TypeStart(typeName, "GroupingExprHost");
		}

		internal void GroupingEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "GroupingHost");
		}

		internal void GroupingExpression(ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void GroupingParentExpressionsStart()
		{
			TypeStart("Parent", "IndexedExprHost");
		}

		internal void GroupingParentExpressionsEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "ParentExpressionsHost");
		}

		internal void GroupingParentExpression(ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void ListStart(string name)
		{
			TypeStart(name, "ListExprHost");
		}

		internal int ListEnd()
		{
			return ReportItemEnd("m_listHostsRemotable", ref m_rootTypeDecl.Lists);
		}

		internal void MatrixDynamicGroupStart(string name)
		{
			TypeStart("MatrixDynamicGroup_" + name, "MatrixDynamicGroupExprHost");
		}

		internal bool MatrixDynamicGroupEnd(bool column)
		{
			string baseTypeName = m_currentTypeDecl.Parent.BaseTypeName;
			if (!(baseTypeName == "MatrixExprHost"))
			{
				if (baseTypeName == "MatrixDynamicGroupExprHost")
				{
					return TypeEnd(m_currentTypeDecl.Parent, "SubGroupHost");
				}
				Global.Tracer.Assert(condition: false);
				return false;
			}
			if (column)
			{
				return TypeEnd(m_currentTypeDecl.Parent, "ColumnGroupingsHost");
			}
			return TypeEnd(m_currentTypeDecl.Parent, "RowGroupingsHost");
		}

		internal void SubtotalStart()
		{
			TypeStart("Subtotal", "StyleExprHost");
		}

		internal void SubtotalEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "SubtotalHost");
		}

		internal void MatrixStart(string name)
		{
			TypeStart(name, "MatrixExprHost");
		}

		internal int MatrixEnd()
		{
			return ReportItemEnd("m_matrixHostsRemotable", ref m_rootTypeDecl.Matrices);
		}

		internal void MultiChartStart()
		{
			TypeStart("MultiChart", "MultiChartExprHost");
		}

		internal void MultiChartEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "MultiChartHost");
		}

		internal void ChartDynamicGroupStart(string name)
		{
			TypeStart("ChartDynamicGroup_" + name, "ChartDynamicGroupExprHost");
		}

		internal bool ChartDynamicGroupEnd(bool column)
		{
			string baseTypeName = m_currentTypeDecl.Parent.BaseTypeName;
			if (!(baseTypeName == "ChartExprHost"))
			{
				if (baseTypeName == "ChartDynamicGroupExprHost")
				{
					return TypeEnd(m_currentTypeDecl.Parent, "SubGroupHost");
				}
				Global.Tracer.Assert(condition: false);
				return false;
			}
			if (column)
			{
				return TypeEnd(m_currentTypeDecl.Parent, "ColumnGroupingsHost");
			}
			return TypeEnd(m_currentTypeDecl.Parent, "RowGroupingsHost");
		}

		internal void ChartHeadingLabel(ExpressionInfo expression)
		{
			ExpressionAdd("HeadingLabelExpr", expression);
		}

		internal void ChartDataPointStart()
		{
			TypeStart(CreateTypeName("DataPoint", ((NonRootTypeDecl)m_currentTypeDecl).DataPoints), "ChartDataPointExprHost");
		}

		internal int ChartDataPointEnd()
		{
			ExprIndexerCreate();
			return TypeEnd(m_currentTypeDecl.Parent, "m_chartDataPointHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).DataPoints);
		}

		internal void ChartDataPointDataValue(ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void DataLabelValue(ExpressionInfo expression)
		{
			ExpressionAdd("DataLabelValueExpr", expression);
		}

		internal void DataLabelStyleStart()
		{
			StyleStart("DataLabelStyle");
		}

		internal void DataLabelStyleEnd()
		{
			StyleEnd("DataLabelStyleHost");
		}

		internal void DataPointStyleStart()
		{
			StyleStart("Style");
		}

		internal void DataPointStyleEnd()
		{
			StyleEnd("StyleHost");
		}

		internal void DataPointMarkerStyleStart()
		{
			StyleStart("DataPointMarkerStyle");
		}

		internal void DataPointMarkerStyleEnd()
		{
			StyleEnd("MarkerStyleHost");
		}

		internal void ChartTitleStart()
		{
			TypeStart("Title", "ChartTitleExprHost");
		}

		internal void ChartTitleEnd()
		{
			TypeEnd(m_currentTypeDecl.Parent, "TitleHost");
		}

		internal void ChartCaption(ExpressionInfo expression)
		{
			ExpressionAdd("CaptionExpr", expression);
		}

		internal void MajorGridLinesStyleStart()
		{
			StyleStart("MajorGridLinesStyle");
		}

		internal void MajorGridLinesStyleEnd()
		{
			StyleEnd("MajorGridLinesHost");
		}

		internal void MinorGridLinesStyleStart()
		{
			StyleStart("MinorGridLinesStyle");
		}

		internal void MinorGridLinesStyleEnd()
		{
			StyleEnd("MinorGridLinesHost");
		}

		internal void AxisMin(ExpressionInfo expression)
		{
			ExpressionAdd("AxisMinExpr", expression);
		}

		internal void AxisMax(ExpressionInfo expression)
		{
			ExpressionAdd("AxisMaxExpr", expression);
		}

		internal void AxisCrossAt(ExpressionInfo expression)
		{
			ExpressionAdd("AxisCrossAtExpr", expression);
		}

		internal void AxisMajorInterval(ExpressionInfo expression)
		{
			ExpressionAdd("AxisMajorIntervalExpr", expression);
		}

		internal void AxisMinorInterval(ExpressionInfo expression)
		{
			ExpressionAdd("AxisMinorIntervalExpr", expression);
		}

		internal void ChartStaticRowLabelsStart()
		{
			TypeStart("ChartStaticRowLabels", "IndexedExprHost");
		}

		internal void ChartStaticRowLabelsEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "StaticRowLabelsHost");
		}

		internal void ChartStaticColumnLabelsStart()
		{
			TypeStart("ChartStaticColumnLabels", "IndexedExprHost");
		}

		internal void ChartStaticColumnLabelsEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "StaticColumnLabelsHost");
		}

		internal void ChartStaticColumnRowLabel(ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void ChartStart(string name)
		{
			TypeStart(name, "ChartExprHost");
		}

		internal int ChartEnd()
		{
			return ReportItemEnd("m_chartHostsRemotable", ref m_rootTypeDecl.Charts);
		}

		internal void ChartCategoryAxisStart()
		{
			AxisStart("CategoryAxis");
		}

		internal void ChartCategoryAxisEnd()
		{
			AxisEnd("CategoryAxisHost");
		}

		internal void ChartValueAxisStart()
		{
			AxisStart("ValueAxis");
		}

		internal void ChartValueAxisEnd()
		{
			AxisEnd("ValueAxisHost");
		}

		internal void ChartLegendStart()
		{
			StyleStart("Legend");
		}

		internal void ChartLegendEnd()
		{
			StyleEnd("LegendHost");
		}

		internal void ChartPlotAreaStart()
		{
			StyleStart("PlotArea");
		}

		internal void ChartPlotAreaEnd()
		{
			StyleEnd("PlotAreaHost");
		}

		internal void TableGroupStart(string name)
		{
			TypeStart("TableGroup_" + name, "TableGroupExprHost");
		}

		internal bool TableGroupEnd()
		{
			string baseTypeName = m_currentTypeDecl.Parent.BaseTypeName;
			if (!(baseTypeName == "TableExprHost"))
			{
				if (baseTypeName == "TableGroupExprHost")
				{
					return TypeEnd(m_currentTypeDecl.Parent, "SubGroupHost");
				}
				Global.Tracer.Assert(condition: false);
				return false;
			}
			return TypeEnd(m_currentTypeDecl.Parent, "TableGroupsHost");
		}

		internal void TableRowVisibilityHiddenExpressionsStart()
		{
			TypeStart("TableRowVisibilityHiddenExpressionsClass", "IndexedExprHost");
		}

		internal void TableRowVisibilityHiddenExpressionsEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "TableRowVisibilityHiddenExpressions");
		}

		internal void TableRowColVisibilityHiddenExpressionsExpr(ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void TableStart(string name)
		{
			TypeStart(name, "TableExprHost");
		}

		internal int TableEnd()
		{
			return ReportItemEnd("m_tableHostsRemotable", ref m_rootTypeDecl.Tables);
		}

		internal void TableColumnVisibilityHiddenExpressionsStart()
		{
			TypeStart("TableColumnVisibilityHiddenExpressions", "IndexedExprHost");
		}

		internal void TableColumnVisibilityHiddenExpressionsEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "TableColumnVisibilityHiddenExpressions");
		}

		internal void OWCChartStart(string name)
		{
			TypeStart(name, "OWCChartExprHost");
		}

		internal int OWCChartEnd()
		{
			return ReportItemEnd("m_OWCChartHostsRemotable", ref m_rootTypeDecl.OWCCharts);
		}

		internal void OWCChartColumnsStart()
		{
			TypeStart("OWCChartColumns", "IndexedExprHost");
		}

		internal void OWCChartColumnsEnd()
		{
			ExprIndexerCreate();
			TypeEnd(m_currentTypeDecl.Parent, "OWCChartColumnHosts");
		}

		internal void OWCChartColumnsValue(ExpressionInfo expression)
		{
			IndexedExpressionAdd(expression);
		}

		internal void DataValueStart()
		{
			TypeStart(CreateTypeName("DataValue", m_currentTypeDecl.DataValues), "DataValueExprHost");
		}

		internal int DataValueEnd(bool isCustomProperty)
		{
			return TypeEnd(m_currentTypeDecl.Parent, isCustomProperty ? "m_customPropertyHostsRemotable" : "m_dataValueHostsRemotable", ref m_currentTypeDecl.Parent.DataValues);
		}

		internal void DataValueName(ExpressionInfo expression)
		{
			ExpressionAdd("DataValueNameExpr", expression);
		}

		internal void DataValueValue(ExpressionInfo expression)
		{
			ExpressionAdd("DataValueValueExpr", expression);
		}

		internal void CustomReportItemStart(string name)
		{
			TypeStart(name, "CustomReportItemExprHost");
		}

		internal int CustomReportItemEnd()
		{
			return ReportItemEnd("m_customReportItemHostsRemotable", ref m_rootTypeDecl.CustomReportItems);
		}

		internal void DataGroupingStart(bool column)
		{
			string template = "DataGrouping" + (column ? "Column" : "Row");
			TypeStart(CreateTypeName(template, ((NonRootTypeDecl)m_currentTypeDecl).DataGroupings), "DataGroupingExprHost");
		}

		internal int DataGroupingEnd(bool column)
		{
			Global.Tracer.Assert("CustomReportItemExprHost" == m_currentTypeDecl.Parent.BaseTypeName || "DataGroupingExprHost" == m_currentTypeDecl.Parent.BaseTypeName);
			return TypeEnd(m_currentTypeDecl.Parent, "m_dataGroupingHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).DataGroupings);
		}

		internal void DataCellStart()
		{
			TypeStart(CreateTypeName("DataCell", ((NonRootTypeDecl)m_currentTypeDecl).DataCells), "DataCellExprHost");
		}

		internal int DataCellEnd()
		{
			return TypeEnd(m_currentTypeDecl.Parent, "m_dataCellHostsRemotable", ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).DataCells);
		}

		private void TypeStart(string typeName, string baseType)
		{
			m_currentTypeDecl = new NonRootTypeDecl(typeName, baseType, m_currentTypeDecl, m_setCode);
		}

		private int TypeEnd(TypeDecl container, string name, ref CodeExpressionCollection initializers)
		{
			int result = -1;
			if (m_currentTypeDecl.HasExpressions)
			{
				result = container.NestedTypeColAdd(name, m_currentTypeDecl.BaseTypeName, ref initializers, m_currentTypeDecl.Type);
			}
			TypeEnd(container);
			return result;
		}

		private bool TypeEnd(TypeDecl container, string name)
		{
			bool hasExpressions = m_currentTypeDecl.HasExpressions;
			if (hasExpressions)
			{
				container.NestedTypeAdd(name, m_currentTypeDecl.Type);
			}
			TypeEnd(container);
			return hasExpressions;
		}

		private void TypeEnd(TypeDecl container)
		{
			Global.Tracer.Assert(m_currentTypeDecl.Parent != null && container != null, "(m_currentTypeDecl.Parent != null && container != null)");
			container.HasExpressions |= m_currentTypeDecl.HasExpressions;
			m_currentTypeDecl = m_currentTypeDecl.Parent;
		}

		private int ReportItemEnd(string name, ref CodeExpressionCollection initializers)
		{
			return TypeEnd(m_rootTypeDecl, name, ref initializers);
		}

		private void ParameterStart()
		{
			TypeStart(CreateTypeName("Parameter", ((NonRootTypeDecl)m_currentTypeDecl).Parameters), "ParamExprHost");
		}

		private int ParameterEnd(string propName)
		{
			return TypeEnd(m_currentTypeDecl.Parent, propName, ref ((NonRootTypeDecl)m_currentTypeDecl.Parent).Parameters);
		}

		private void StyleStart(string typeName)
		{
			TypeStart(typeName, "StyleExprHost");
		}

		private void StyleEnd(string propName)
		{
			TypeEnd(m_currentTypeDecl.Parent, propName);
		}

		private void AxisStart(string typeName)
		{
			TypeStart(typeName, "AxisExprHost");
		}

		private void AxisEnd(string propName)
		{
			TypeEnd(m_currentTypeDecl.Parent, propName);
		}

		private void AggregateStart()
		{
			TypeStart(CreateTypeName("Aggregate", m_rootTypeDecl.Aggregates), "AggregateParamExprHost");
		}

		private int AggregateEnd()
		{
			return TypeEnd(m_rootTypeDecl, "m_aggregateParamHostsRemotable", ref m_rootTypeDecl.Aggregates);
		}

		private string CreateTypeName(string template, CodeExpressionCollection initializers)
		{
			return template + ((initializers == null) ? "0" : initializers.Count.ToString(CultureInfo.InvariantCulture));
		}

		private void ExprIndexerCreate()
		{
			NonRootTypeDecl nonRootTypeDecl = (NonRootTypeDecl)m_currentTypeDecl;
			if (nonRootTypeDecl.IndexedExpressions == null)
			{
				return;
			}
			Global.Tracer.Assert(nonRootTypeDecl.IndexedExpressions.Count > 0, "(currentTypeDecl.IndexedExpressions.Count > 0)");
			CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
			codeMemberProperty.Name = "Item";
			codeMemberProperty.Attributes = (MemberAttributes)24580;
			codeMemberProperty.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "index"));
			codeMemberProperty.Type = new CodeTypeReference(typeof(object));
			nonRootTypeDecl.Type.Members.Add(codeMemberProperty);
			int count = nonRootTypeDecl.IndexedExpressions.Count;
			if (count == 1)
			{
				codeMemberProperty.GetStatements.Add(nonRootTypeDecl.IndexedExpressions[0]);
				return;
			}
			CodeConditionStatement codeConditionStatement = new CodeConditionStatement();
			codeMemberProperty.GetStatements.Add(codeConditionStatement);
			int num = count - 1;
			int num2 = count - 2;
			for (int i = 0; i < num; i++)
			{
				codeConditionStatement.Condition = new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("index"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(i));
				codeConditionStatement.TrueStatements.Add(nonRootTypeDecl.IndexedExpressions[i]);
				if (i < num2)
				{
					CodeConditionStatement codeConditionStatement2 = new CodeConditionStatement();
					codeConditionStatement.FalseStatements.Add(codeConditionStatement2);
					codeConditionStatement = codeConditionStatement2;
				}
			}
			codeConditionStatement.FalseStatements.Add(nonRootTypeDecl.IndexedExpressions[num]);
		}

		private void IndexedExpressionAdd(ExpressionInfo expression)
		{
			if (expression.Type == ExpressionInfo.Types.Expression)
			{
				NonRootTypeDecl nonRootTypeDecl = (NonRootTypeDecl)m_currentTypeDecl;
				if (nonRootTypeDecl.IndexedExpressions == null)
				{
					nonRootTypeDecl.IndexedExpressions = new ReturnStatementList();
				}
				nonRootTypeDecl.HasExpressions = true;
				expression.ExprHostID = nonRootTypeDecl.IndexedExpressions.Add(CreateExprReturnStatement(expression));
			}
		}

		private void ExpressionAdd(string name, ExpressionInfo expression)
		{
			if (expression.Type == ExpressionInfo.Types.Expression)
			{
				CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
				codeMemberProperty.Name = name;
				codeMemberProperty.Type = new CodeTypeReference(typeof(object));
				codeMemberProperty.Attributes = (MemberAttributes)24580;
				codeMemberProperty.GetStatements.Add(CreateExprReturnStatement(expression));
				m_currentTypeDecl.Type.Members.Add(codeMemberProperty);
				m_currentTypeDecl.HasExpressions = true;
			}
		}

		private CodeMethodReturnStatement CreateExprReturnStatement(ExpressionInfo expression)
		{
			return new CodeMethodReturnStatement(new CodeSnippetExpression(expression.TransformedExpression))
			{
				LinePragma = new CodeLinePragma("Expr" + expression.CompileTimeID.ToString(CultureInfo.InvariantCulture) + "end", 0)
			};
		}
	}
}

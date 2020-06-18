using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TextBox : ReportItem, IActionOwner
	{
		private ExpressionInfo m_value;

		private string m_formula;

		private bool m_canGrow;

		private bool m_canShrink;

		private string m_hideDuplicates;

		private Action m_action;

		private bool m_isToggle;

		private ExpressionInfo m_initialToggleState;

		private bool m_valueReferenced;

		private bool m_recursiveSender;

		private bool m_dataElementStyleAttribute = true;

		[Reference]
		private GroupingList m_containingScopes;

		private EndUserSort m_userSort;

		private bool m_isMatrixCellScope;

		private TypeCode m_valueType = TypeCode.String;

		private bool m_isSubReportTopLevelScope;

		[NonSerialized]
		private bool m_overrideReportDataElementStyle;

		[NonSerialized]
		private string m_textboxScope;

		[NonSerialized]
		private bool m_isDetailScope;

		[NonSerialized]
		private bool m_valueTypeSet;

		[NonSerialized]
		private VariantResult m_oldResult;

		[NonSerialized]
		private bool m_hasOldResult;

		[NonSerialized]
		private string m_formattedValue;

		[NonSerialized]
		private TextBoxExprHost m_exprHost;

		[NonSerialized]
		private bool m_sharedFormatSettings;

		[NonSerialized]
		private bool m_calendarValidated;

		[NonSerialized]
		private Calendar m_calendar;

		[NonSerialized]
		private uint m_languageInstanceId;

		[NonSerialized]
		private int m_tableColumnPosition = -1;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		internal override ObjectType ObjectType => ObjectType.Textbox;

		internal ExpressionInfo Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
			}
		}

		internal bool CanGrow
		{
			get
			{
				return m_canGrow;
			}
			set
			{
				m_canGrow = value;
			}
		}

		internal bool CanShrink
		{
			get
			{
				return m_canShrink;
			}
			set
			{
				m_canShrink = value;
			}
		}

		internal string HideDuplicates
		{
			get
			{
				return m_hideDuplicates;
			}
			set
			{
				m_hideDuplicates = value;
			}
		}

		internal Action Action
		{
			get
			{
				return m_action;
			}
			set
			{
				m_action = value;
			}
		}

		internal bool IsToggle
		{
			get
			{
				return m_isToggle;
			}
			set
			{
				m_isToggle = value;
			}
		}

		internal ExpressionInfo InitialToggleState
		{
			get
			{
				return m_initialToggleState;
			}
			set
			{
				m_initialToggleState = value;
			}
		}

		internal bool RecursiveSender
		{
			get
			{
				return m_recursiveSender;
			}
			set
			{
				m_recursiveSender = value;
			}
		}

		internal TypeCode ValueType
		{
			get
			{
				return m_valueType;
			}
			set
			{
				m_valueType = value;
			}
		}

		internal VariantResult OldResult
		{
			get
			{
				return m_oldResult;
			}
			set
			{
				m_oldResult = value;
				m_hasOldResult = true;
			}
		}

		internal bool IsSubReportTopLevelScope
		{
			get
			{
				return m_isSubReportTopLevelScope;
			}
			set
			{
				m_isSubReportTopLevelScope = value;
			}
		}

		internal bool HasOldResult
		{
			get
			{
				return m_hasOldResult;
			}
			set
			{
				m_hasOldResult = value;
			}
		}

		internal bool SharedFormatSettings
		{
			get
			{
				return m_sharedFormatSettings;
			}
			set
			{
				m_sharedFormatSettings = value;
			}
		}

		internal string FormattedValue
		{
			get
			{
				return m_formattedValue;
			}
			set
			{
				m_formattedValue = value;
			}
		}

		internal string Formula
		{
			get
			{
				return m_formula;
			}
			set
			{
				m_formula = value;
			}
		}

		internal bool ValueReferenced
		{
			get
			{
				return m_valueReferenced;
			}
			set
			{
				m_valueReferenced = value;
			}
		}

		internal bool CalendarValidated
		{
			get
			{
				return m_calendarValidated;
			}
			set
			{
				m_calendarValidated = value;
			}
		}

		internal Calendar Calendar
		{
			get
			{
				return m_calendar;
			}
			set
			{
				m_calendar = value;
			}
		}

		internal uint LanguageInstanceId
		{
			get
			{
				return m_languageInstanceId;
			}
			set
			{
				m_languageInstanceId = value;
			}
		}

		internal TextBoxExprHost TextBoxExprHost => m_exprHost;

		internal bool DataElementStyleAttribute
		{
			get
			{
				return m_dataElementStyleAttribute;
			}
			set
			{
				m_dataElementStyleAttribute = value;
			}
		}

		internal GroupingList ContainingScopes
		{
			get
			{
				return m_containingScopes;
			}
			set
			{
				m_containingScopes = value;
			}
		}

		internal EndUserSort UserSort
		{
			get
			{
				return m_userSort;
			}
			set
			{
				m_userSort = value;
			}
		}

		internal bool IsMatrixCellScope
		{
			get
			{
				return m_isMatrixCellScope;
			}
			set
			{
				m_isMatrixCellScope = value;
			}
		}

		internal bool OverrideReportDataElementStyle
		{
			get
			{
				return m_overrideReportDataElementStyle;
			}
			set
			{
				m_overrideReportDataElementStyle = value;
			}
		}

		internal override DataElementOutputTypes DataElementOutputDefault
		{
			get
			{
				if (DataElementOutputTypesRDL.Auto == m_dataElementOutputRDL && m_value != null && ExpressionInfo.Types.Constant == m_value.Type)
				{
					return DataElementOutputTypes.NoOutput;
				}
				return DataElementOutputTypes.Output;
			}
		}

		internal string TextBoxScope
		{
			get
			{
				return m_textboxScope;
			}
			set
			{
				m_textboxScope = value;
			}
		}

		internal bool IsDetailScope
		{
			get
			{
				return m_isDetailScope;
			}
			set
			{
				m_isDetailScope = value;
			}
		}

		internal int TableColumnPosition
		{
			get
			{
				return m_tableColumnPosition;
			}
			set
			{
				m_tableColumnPosition = value;
			}
		}

		Action IActionOwner.Action => m_action;

		List<string> IActionOwner.FieldsUsedInValueExpression
		{
			get
			{
				return m_fieldsUsedInValueExpression;
			}
			set
			{
				m_fieldsUsedInValueExpression = value;
			}
		}

		internal TextBox(ReportItem parent)
			: base(parent)
		{
		}

		internal TextBox(int id, ReportItem parent)
			: base(id, parent)
		{
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			context.ExprHostBuilder.TextBoxStart(m_name);
			base.Initialize(context);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context, isContainer: false, tableRowCol: false);
			}
			if (m_value != null)
			{
				m_value.Initialize("Value", context);
				context.ExprHostBuilder.GenericValue(m_value);
			}
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_initialToggleState != null)
			{
				m_initialToggleState.Initialize("InitialState", context);
				context.ExprHostBuilder.TextBoxToggleImageInitialState(m_initialToggleState);
			}
			if (m_hideDuplicates != null)
			{
				context.ValidateHideDuplicateScope(m_hideDuplicates, this);
			}
			context.RegisterSender(this);
			if (m_userSort != null)
			{
				context.RegisterSortFilterTextbox(this);
				m_textboxScope = context.GetCurrentScope();
				if ((LocationFlags)0 < (context.Location & LocationFlags.InMatrixCellTopLevelItem))
				{
					m_isMatrixCellScope = true;
				}
				if ((LocationFlags)0 < (context.Location & LocationFlags.InDetail))
				{
					m_isDetailScope = true;
					context.SetDataSetDetailUserSortFilter();
				}
				string sortExpressionScopeString = m_userSort.SortExpressionScopeString;
				if (sortExpressionScopeString == null)
				{
					context.TextboxWithDetailSortExpressionAdd(this);
				}
				else if (context.IsScope(sortExpressionScopeString))
				{
					if (context.IsCurrentScope(sortExpressionScopeString) && !m_isMatrixCellScope)
					{
						m_userSort.SortExpressionScope = context.GetSortFilterScope(sortExpressionScopeString);
						InitializeSortExpression(context, needsExplicitAggregateScope: false);
					}
					else if (context.IsAncestorScope(sortExpressionScopeString, (LocationFlags)0 < (context.Location & LocationFlags.InMatrixGroupHeader), m_isMatrixCellScope))
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidExpressionScope, Severity.Error, context.ObjectType, context.ObjectName, "SortExpressionScope", sortExpressionScopeString);
					}
					else
					{
						context.RegisterUserSortInnerScope(this);
					}
				}
				else
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsNonExistingScope, Severity.Error, context.ObjectType, context.ObjectName, "SortExpressionScope", sortExpressionScopeString);
				}
				string sortTargetString = m_userSort.SortTargetString;
				if (sortTargetString != null)
				{
					if (context.IsScope(sortTargetString))
					{
						if (!context.IsCurrentScope(sortTargetString) && !context.IsAncestorScope(sortTargetString, (LocationFlags)0 < (context.Location & LocationFlags.InMatrixGroupHeader), checkAllGroupingScopes: false) && !context.IsPeerScope(sortTargetString))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTargetScope, Severity.Error, context.ObjectType, context.ObjectName, "SortTarget", sortTargetString);
						}
					}
					else
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsNonExistingScope, Severity.Error, context.ObjectType, context.ObjectName, "SortTarget", sortTargetString);
					}
				}
				else if (context.IsReportTopLevelScope())
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidOmittedTargetScope, Severity.Error, context.ObjectType, context.ObjectName, "SortTarget");
				}
			}
			base.ExprHostID = context.ExprHostBuilder.TextBoxEnd();
			return true;
		}

		internal void InitializeSortExpression(InitializationContext context, bool needsExplicitAggregateScope)
		{
			if (m_userSort == null || m_userSort.SortExpression == null)
			{
				return;
			}
			bool flag = true;
			if (needsExplicitAggregateScope && m_userSort.SortExpression.Aggregates != null)
			{
				int count = m_userSort.SortExpression.Aggregates.Count;
				for (int i = 0; i < count; i++)
				{
					if (!m_userSort.SortExpression.Aggregates[i].GetScope(out string _))
					{
						flag = false;
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidOmittedExpressionScope, Severity.Error, ObjectType.Textbox, m_name, "SortExpression", "SortExpressionScope");
					}
				}
			}
			if (flag)
			{
				m_userSort.SortExpression.Initialize("SortExpression", context);
			}
		}

		internal void AddToScopeSortFilterList()
		{
			IntList peerSortFilters = GetPeerSortFilters(create: true);
			Global.Tracer.Assert(peerSortFilters != null);
			peerSortFilters.Add(m_ID);
		}

		internal IntList GetPeerSortFilters(bool create)
		{
			if (m_userSort == null)
			{
				return null;
			}
			InScopeSortFilterHashtable inScopeSortFilterHashtable = null;
			IntList intList = null;
			if (m_containingScopes == null || m_containingScopes.Count == 0 || m_isSubReportTopLevelScope)
			{
				inScopeSortFilterHashtable = GetSortFiltersInScope(create, inDetail: false);
			}
			else
			{
				Grouping lastEntry = m_containingScopes.LastEntry;
				if (lastEntry == null)
				{
					inScopeSortFilterHashtable = GetSortFiltersInScope(create, inDetail: true);
				}
				else if (m_userSort.SortExpressionScope == null)
				{
					if (lastEntry.DetailSortFiltersInScope == null && create)
					{
						lastEntry.DetailSortFiltersInScope = new InScopeSortFilterHashtable();
					}
					inScopeSortFilterHashtable = lastEntry.DetailSortFiltersInScope;
				}
				else
				{
					if (lastEntry.NonDetailSortFiltersInScope == null && create)
					{
						lastEntry.NonDetailSortFiltersInScope = new InScopeSortFilterHashtable();
					}
					inScopeSortFilterHashtable = lastEntry.NonDetailSortFiltersInScope;
				}
			}
			if (inScopeSortFilterHashtable != null)
			{
				int num = (m_userSort.SortExpressionScope == null) ? m_userSort.SortTarget.ID : m_userSort.SortExpressionScope.ID;
				intList = inScopeSortFilterHashtable[num];
				if (intList == null && create)
				{
					intList = new IntList();
					inScopeSortFilterHashtable.Add(num, intList);
				}
			}
			return intList;
		}

		private InScopeSortFilterHashtable GetSortFiltersInScope(bool create, bool inDetail)
		{
			InScopeSortFilterHashtable inScopeSortFilterHashtable = null;
			ReportItem parent = m_parent;
			if (inDetail)
			{
				while (parent != null && !(parent is DataRegion))
				{
					parent = parent.Parent;
				}
			}
			else
			{
				while (parent != null && !(parent is Report))
				{
					parent = parent.Parent;
				}
			}
			Global.Tracer.Assert(parent is DataRegion || parent is Report);
			if (parent is Report)
			{
				Report report = (Report)parent;
				if (m_userSort.SortExpressionScope == null)
				{
					if (report.DetailSortFiltersInScope == null && create)
					{
						report.DetailSortFiltersInScope = new InScopeSortFilterHashtable();
					}
					return report.DetailSortFiltersInScope;
				}
				if (report.NonDetailSortFiltersInScope == null && create)
				{
					report.NonDetailSortFiltersInScope = new InScopeSortFilterHashtable();
				}
				return report.NonDetailSortFiltersInScope;
			}
			Global.Tracer.Assert(m_userSort.SortExpressionScope == null);
			DataRegion dataRegion = (DataRegion)parent;
			if (dataRegion.DetailSortFiltersInScope == null && create)
			{
				dataRegion.DetailSortFiltersInScope = new InScopeSortFilterHashtable();
			}
			return dataRegion.DetailSortFiltersInScope;
		}

		protected override void DataRendererInitialize(InitializationContext context)
		{
			base.DataRendererInitialize(context);
			if (!m_overrideReportDataElementStyle)
			{
				m_dataElementStyleAttribute = context.ReportDataElementStyleAttribute;
			}
		}

		internal void SetValueType(object textBoxValue)
		{
			if (textBoxValue == null || DBNull.Value == textBoxValue || (m_valueTypeSet && TypeCode.Object == m_valueType))
			{
				return;
			}
			TypeCode typeCode = Type.GetTypeCode(textBoxValue.GetType());
			if (m_valueTypeSet)
			{
				if (m_valueType != typeCode)
				{
					m_valueType = TypeCode.Object;
				}
			}
			else
			{
				m_valueType = typeCode;
			}
			m_valueTypeSet = true;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID < 0)
			{
				return;
			}
			Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
			m_exprHost = reportExprHost.TextBoxHostsRemotable[base.ExprHostID];
			ReportItemSetExprHost(m_exprHost, reportObjectModel);
			if (m_action != null)
			{
				if (m_exprHost.ActionInfoHost != null)
				{
					m_action.SetExprHost(m_exprHost.ActionInfoHost, reportObjectModel);
				}
				else if (m_exprHost.ActionHost != null)
				{
					m_action.SetExprHost(m_exprHost.ActionHost, reportObjectModel);
				}
			}
		}

		internal override void ProcessDrillthroughAction(ReportProcessing.ProcessingContext processingContext, NonComputedUniqueNames nonCompNames)
		{
			if (m_action != null && nonCompNames != null)
			{
				m_action.ProcessDrillthroughAction(processingContext, nonCompNames.UniqueName);
			}
		}

		internal bool IsSimpleTextBox()
		{
			if (m_styleClass != null && m_styleClass.ExpressionList != null && 0 < m_styleClass.ExpressionList.Count)
			{
				return false;
			}
			if (m_initialToggleState != null || m_isToggle || m_visibility != null)
			{
				return false;
			}
			if (m_label != null || m_bookmark != null || m_action != null)
			{
				return false;
			}
			if (m_toolTip != null && ExpressionInfo.Types.Constant != m_toolTip.Type)
			{
				return false;
			}
			if (m_customProperties != null)
			{
				return false;
			}
			if (m_hideDuplicates != null || m_userSort != null)
			{
				return false;
			}
			return true;
		}

		internal bool IsSimpleTextBox(IntermediateFormatVersion intermediateFormatVersion)
		{
			Global.Tracer.Assert(intermediateFormatVersion != null);
			if (intermediateFormatVersion.IsRS2005_WithSimpleTextBoxOptimizations)
			{
				return IsSimpleTextBox();
			}
			return false;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.CanGrow, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.CanShrink, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.HideDuplicates, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Action));
			memberInfoList.Add(new MemberInfo(MemberName.IsToggle, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.InitialToggleState, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.ValueType, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Formula, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.ValueReferenced, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.RecursiveSender, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.DataElementStyleAttribute, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.ContainingScopes, Token.Reference, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.GroupingList));
			memberInfoList.Add(new MemberInfo(MemberName.UserSort, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.EndUserSort));
			memberInfoList.Add(new MemberInfo(MemberName.IsMatrixCellScope, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.IsSubReportTopLevelScope, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItem, memberInfoList);
		}
	}
}

using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Grouping : IAggregateHolder, ISortFilterScope, IPersistable, IReferenceable, IPageBreakOwner
	{
		private string m_name;

		private int m_ID = -1;

		private List<ExpressionInfo> m_groupExpressions;

		private ExpressionInfo m_groupLabel;

		private List<bool> m_sortDirections;

		private PageBreak m_pageBreak;

		private ExpressionInfo m_pageName;

		private List<DataAggregateInfo> m_aggregates;

		private bool m_groupAndSort;

		private List<Filter> m_filters;

		[Reference]
		private List<ReportItem> m_reportItemsWithHideDuplicates;

		private List<ExpressionInfo> m_parent;

		private List<DataAggregateInfo> m_recursiveAggregates;

		private List<DataAggregateInfo> m_postSortAggregates;

		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput;

		private bool m_saveGroupExprValues;

		private List<ExpressionInfo> m_userSortExpressions;

		private InScopeSortFilterHashtable m_nonDetailSortFiltersInScope;

		private InScopeSortFilterHashtable m_detailSortFiltersInScope;

		private List<Variable> m_variables;

		private string m_domainScope;

		private int m_scopeIDForDomainScope = -1;

		private bool m_naturalGroup;

		[NonSerialized]
		private List<int> m_hideDuplicatesReportItemIDs;

		[NonSerialized]
		private GroupExprHost m_exprHost;

		[NonSerialized]
		private Hashtable m_scopeNames;

		[NonSerialized]
		private int m_recursiveLevel;

		[NonSerialized]
		private int[] m_groupExpressionFieldIndices;

		[NonSerialized]
		private bool m_isClone;

		[NonSerialized]
		private List<object> m_currentGroupExprValues;

		[NonSerialized]
		private object[] m_groupInstanceExprValues;

		[NonSerialized]
		private ReportHierarchyNode m_owner;

		[NonSerialized]
		private List<object>[] m_sortFilterScopeInfo;

		[NonSerialized]
		private int[] m_sortFilterScopeIndex;

		[NonSerialized]
		private bool[] m_needScopeInfoForSortFilterExpression;

		[NonSerialized]
		private bool[] m_sortFilterScopeMatched;

		[NonSerialized]
		private bool[] m_isSortFilterTarget;

		[NonSerialized]
		private bool[] m_isSortFilterExpressionScope;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		internal ExpressionInfo GroupLabel
		{
			get
			{
				return m_groupLabel;
			}
			set
			{
				m_groupLabel = value;
			}
		}

		internal List<bool> SortDirections
		{
			get
			{
				return m_sortDirections;
			}
			set
			{
				m_sortDirections = value;
			}
		}

		internal List<ExpressionInfo> GroupExpressions
		{
			get
			{
				return m_groupExpressions;
			}
			set
			{
				m_groupExpressions = value;
			}
		}

		internal string DomainScope
		{
			get
			{
				return m_domainScope;
			}
			set
			{
				m_domainScope = value;
			}
		}

		internal int ScopeIDForDomainScope
		{
			get
			{
				if (m_scopeIDForDomainScope == -1)
				{
					return Owner.DataScopeInfo.ScopeID;
				}
				return m_scopeIDForDomainScope;
			}
			set
			{
				m_scopeIDForDomainScope = value;
			}
		}

		internal bool IsDetail
		{
			get
			{
				if (m_groupExpressions != null)
				{
					return m_groupExpressions.Count == 0;
				}
				return true;
			}
		}

		internal bool IsClone => m_isClone;

		internal ExpressionInfo PageName
		{
			get
			{
				return m_pageName;
			}
			set
			{
				m_pageName = value;
			}
		}

		internal PageBreak PageBreak
		{
			get
			{
				return m_pageBreak;
			}
			set
			{
				m_pageBreak = value;
			}
		}

		PageBreak IPageBreakOwner.PageBreak
		{
			get
			{
				return m_pageBreak;
			}
			set
			{
				m_pageBreak = value;
			}
		}

		Microsoft.ReportingServices.ReportProcessing.ObjectType IPageBreakOwner.ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.Grouping;

		string IPageBreakOwner.ObjectName => m_name;

		IInstancePath IPageBreakOwner.InstancePath => m_owner;

		internal List<DataAggregateInfo> Aggregates
		{
			get
			{
				return m_aggregates;
			}
			set
			{
				m_aggregates = value;
			}
		}

		internal bool GroupAndSort
		{
			get
			{
				return m_groupAndSort;
			}
			set
			{
				m_groupAndSort = value;
			}
		}

		internal List<Filter> Filters
		{
			get
			{
				return m_filters;
			}
			set
			{
				m_filters = value;
			}
		}

		internal bool SimpleGroupExpressions
		{
			get
			{
				if (m_groupExpressions != null)
				{
					for (int i = 0; i < m_groupExpressions.Count; i++)
					{
						ExpressionInfo expressionInfo = m_groupExpressions[i];
						Global.Tracer.Assert(expressionInfo != null, "(null != expression)");
						if (expressionInfo.Type != ExpressionInfo.Types.Field && expressionInfo.Type != ExpressionInfo.Types.Constant)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		internal List<ReportItem> ReportItemsWithHideDuplicates
		{
			get
			{
				return m_reportItemsWithHideDuplicates;
			}
			set
			{
				m_reportItemsWithHideDuplicates = value;
			}
		}

		internal List<ExpressionInfo> Parent
		{
			get
			{
				return m_parent;
			}
			set
			{
				m_parent = value;
			}
		}

		internal IndexedExprHost ParentExprHost
		{
			get
			{
				if (m_exprHost == null)
				{
					return null;
				}
				return m_exprHost.ParentExpressionsHost;
			}
		}

		internal IndexedExprHost VariableValueHosts
		{
			get
			{
				if (m_exprHost == null)
				{
					return null;
				}
				return m_exprHost.VariableValueHosts;
			}
		}

		internal List<DataAggregateInfo> RecursiveAggregates
		{
			get
			{
				return m_recursiveAggregates;
			}
			set
			{
				m_recursiveAggregates = value;
			}
		}

		internal List<DataAggregateInfo> PostSortAggregates
		{
			get
			{
				return m_postSortAggregates;
			}
			set
			{
				m_postSortAggregates = value;
			}
		}

		internal string DataElementName
		{
			get
			{
				return m_dataElementName;
			}
			set
			{
				m_dataElementName = value;
			}
		}

		internal DataElementOutputTypes DataElementOutput
		{
			get
			{
				return m_dataElementOutput;
			}
			set
			{
				m_dataElementOutput = value;
			}
		}

		internal bool SaveGroupExprValues
		{
			get
			{
				return m_saveGroupExprValues;
			}
			set
			{
				m_saveGroupExprValues = value;
			}
		}

		internal List<ExpressionInfo> UserSortExpressions
		{
			get
			{
				return m_userSortExpressions;
			}
			set
			{
				m_userSortExpressions = value;
			}
		}

		internal InScopeSortFilterHashtable NonDetailSortFiltersInScope
		{
			get
			{
				return m_nonDetailSortFiltersInScope;
			}
			set
			{
				m_nonDetailSortFiltersInScope = value;
			}
		}

		internal InScopeSortFilterHashtable DetailSortFiltersInScope
		{
			get
			{
				return m_detailSortFiltersInScope;
			}
			set
			{
				m_detailSortFiltersInScope = value;
			}
		}

		internal List<int> HideDuplicatesReportItemIDs
		{
			get
			{
				return m_hideDuplicatesReportItemIDs;
			}
			set
			{
				m_hideDuplicatesReportItemIDs = value;
			}
		}

		internal GroupExprHost ExprHost => m_exprHost;

		internal Hashtable ScopeNames
		{
			get
			{
				return m_scopeNames;
			}
			set
			{
				m_scopeNames = value;
			}
		}

		internal int RecursiveLevel
		{
			get
			{
				return m_recursiveLevel;
			}
			set
			{
				m_recursiveLevel = value;
			}
		}

		internal List<object> CurrentGroupExpressionValues
		{
			get
			{
				return m_currentGroupExprValues;
			}
			set
			{
				m_currentGroupExprValues = value;
			}
		}

		internal ReportHierarchyNode Owner
		{
			get
			{
				return m_owner;
			}
			set
			{
				m_owner = value;
			}
		}

		internal List<object>[] SortFilterScopeInfo
		{
			get
			{
				return m_sortFilterScopeInfo;
			}
			set
			{
				m_sortFilterScopeInfo = value;
			}
		}

		internal int[] SortFilterScopeIndex
		{
			get
			{
				return m_sortFilterScopeIndex;
			}
			set
			{
				m_sortFilterScopeIndex = value;
			}
		}

		internal bool[] NeedScopeInfoForSortFilterExpression
		{
			get
			{
				return m_needScopeInfoForSortFilterExpression;
			}
			set
			{
				m_needScopeInfoForSortFilterExpression = value;
			}
		}

		internal bool[] IsSortFilterTarget
		{
			get
			{
				return m_isSortFilterTarget;
			}
			set
			{
				m_isSortFilterTarget = value;
			}
		}

		internal bool[] IsSortFilterExpressionScope
		{
			get
			{
				return m_isSortFilterExpressionScope;
			}
			set
			{
				m_isSortFilterExpressionScope = value;
			}
		}

		internal bool[] SortFilterScopeMatched
		{
			get
			{
				return m_sortFilterScopeMatched;
			}
			set
			{
				m_sortFilterScopeMatched = value;
			}
		}

		int IReferenceable.ID => m_ID;

		int ISortFilterScope.ID
		{
			get
			{
				Global.Tracer.Assert(m_owner != null);
				return m_owner.ID;
			}
		}

		string ISortFilterScope.ScopeName => m_name;

		bool[] ISortFilterScope.IsSortFilterTarget
		{
			get
			{
				return m_isSortFilterTarget;
			}
			set
			{
				m_isSortFilterTarget = value;
			}
		}

		bool[] ISortFilterScope.IsSortFilterExpressionScope
		{
			get
			{
				return m_isSortFilterExpressionScope;
			}
			set
			{
				m_isSortFilterExpressionScope = value;
			}
		}

		List<ExpressionInfo> ISortFilterScope.UserSortExpressions
		{
			get
			{
				return m_userSortExpressions;
			}
			set
			{
				m_userSortExpressions = value;
			}
		}

		IndexedExprHost ISortFilterScope.UserSortExpressionsHost
		{
			get
			{
				if (m_exprHost == null)
				{
					return null;
				}
				return m_exprHost.UserSortExpressionsHost;
			}
		}

		internal List<Variable> Variables
		{
			get
			{
				return m_variables;
			}
			set
			{
				m_variables = value;
			}
		}

		internal bool NaturalGroup
		{
			get
			{
				return m_naturalGroup;
			}
			set
			{
				m_naturalGroup = value;
			}
		}

		DataScopeInfo IAggregateHolder.DataScopeInfo => m_owner.DataScopeInfo;

		internal Grouping(ConstructionPhase phase)
			: this(-1, phase)
		{
		}

		internal Grouping(int id, ConstructionPhase phase)
		{
			if (phase == ConstructionPhase.Publishing)
			{
				m_groupExpressions = new List<ExpressionInfo>();
				m_aggregates = new List<DataAggregateInfo>();
				m_postSortAggregates = new List<DataAggregateInfo>();
				m_recursiveAggregates = new List<DataAggregateInfo>();
			}
			m_ID = id;
		}

		internal bool IsAtomic(InitializationContext context)
		{
			if (!context.EvaluateAtomicityCondition(!m_naturalGroup && !IsDetail, m_owner, AtomicityReason.NonNaturalGroup) && !context.EvaluateAtomicityCondition(m_domainScope != null, m_owner, AtomicityReason.DomainScope) && !context.EvaluateAtomicityCondition(m_parent != null, m_owner, AtomicityReason.RecursiveParent))
			{
				return context.EvaluateAtomicityCondition(HasAggregatesForAtomicityCheck(), m_owner, AtomicityReason.Aggregates);
			}
			return true;
		}

		private bool HasAggregatesForAtomicityCheck()
		{
			if (!DataScopeInfo.HasNonServerAggregates(m_aggregates) && !DataScopeInfo.HasAggregates(m_postSortAggregates))
			{
				return DataScopeInfo.HasAggregates(m_recursiveAggregates);
			}
			return true;
		}

		public void ResetAggregates(AggregatesImpl reportOmAggregates)
		{
			reportOmAggregates.ResetAll(m_aggregates);
			reportOmAggregates.ResetAll(m_postSortAggregates);
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.GroupStart(m_name);
			m_saveGroupExprValues = context.HasPreviousAggregates;
			DataRendererInitialize(context);
			if (m_groupExpressions != null)
			{
				for (int i = 0; i < m_groupExpressions.Count; i++)
				{
					ExpressionInfo expressionInfo = m_groupExpressions[i];
					expressionInfo.GroupExpressionInitialize(context);
					context.ExprHostBuilder.GroupExpression(expressionInfo);
				}
			}
			if (m_groupLabel != null)
			{
				m_groupLabel.Initialize("Label", context);
				context.ExprHostBuilder.GenericLabel(m_groupLabel);
			}
			if (m_filters != null)
			{
				for (int j = 0; j < m_filters.Count; j++)
				{
					m_filters[j].Initialize(context);
				}
			}
			if (m_parent != null)
			{
				context.ExprHostBuilder.GroupParentExpressionsStart();
				for (int k = 0; k < m_parent.Count; k++)
				{
					ExpressionInfo expressionInfo2 = m_parent[k];
					expressionInfo2.GroupExpressionInitialize(context);
					context.ExprHostBuilder.GroupParentExpression(expressionInfo2);
				}
				context.ExprHostBuilder.GroupParentExpressionsEnd();
			}
			if (m_userSortExpressions != null)
			{
				context.ExprHostBuilder.UserSortExpressionsStart();
				for (int l = 0; l < m_userSortExpressions.Count; l++)
				{
					ExpressionInfo expression = m_userSortExpressions[l];
					context.ExprHostBuilder.UserSortExpression(expression);
				}
				context.ExprHostBuilder.UserSortExpressionsEnd();
			}
			if (m_variables != null && m_variables.Count != 0)
			{
				context.ExprHostBuilder.VariableValuesStart();
				for (int m = 0; m < m_variables.Count; m++)
				{
					Variable variable = m_variables[m];
					variable.Initialize(context);
					context.ExprHostBuilder.VariableValueExpression(variable.Value);
				}
				context.ExprHostBuilder.VariableValuesEnd();
			}
			if (m_pageBreak != null)
			{
				m_pageBreak.Initialize(context);
			}
			if (m_pageName != null)
			{
				m_pageName.Initialize("PageName", context);
				context.ExprHostBuilder.PageName(m_pageName);
			}
			context.ExprHostBuilder.GroupEnd();
		}

		List<DataAggregateInfo> IAggregateHolder.GetAggregateList()
		{
			return m_aggregates;
		}

		List<DataAggregateInfo> IAggregateHolder.GetPostSortAggregateList()
		{
			return m_postSortAggregates;
		}

		void IAggregateHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(m_aggregates != null, "(null != m_aggregates)");
			if (m_aggregates.Count == 0)
			{
				m_aggregates = null;
			}
			Global.Tracer.Assert(m_postSortAggregates != null, "(null != m_postSortAggregates)");
			if (m_postSortAggregates.Count == 0)
			{
				m_postSortAggregates = null;
			}
			Global.Tracer.Assert(m_recursiveAggregates != null, "(null != m_recursiveAggregates)");
			if (m_recursiveAggregates.Count == 0)
			{
				m_recursiveAggregates = null;
			}
		}

		private void DataRendererInitialize(InitializationContext context)
		{
			if (m_dataElementOutput == DataElementOutputTypes.Auto || m_dataElementOutput == DataElementOutputTypes.ContentsOnly)
			{
				m_dataElementOutput = DataElementOutputTypes.Output;
			}
			Microsoft.ReportingServices.ReportPublishing.CLSNameValidator.ValidateDataElementName(ref m_dataElementName, m_name, context.ObjectType, context.ObjectName, "DataElementName", context.ErrorContext);
		}

		internal void AddReportItemWithHideDuplicates(ReportItem reportItem)
		{
			if (m_reportItemsWithHideDuplicates == null)
			{
				m_reportItemsWithHideDuplicates = new List<ReportItem>();
			}
			m_reportItemsWithHideDuplicates.Add(reportItem);
		}

		internal void ResetReportItemsWithHideDuplicates()
		{
			if (m_reportItemsWithHideDuplicates != null)
			{
				int count = m_reportItemsWithHideDuplicates.Count;
				for (int i = 0; i < count; i++)
				{
					(m_reportItemsWithHideDuplicates[i] as TextBox).ResetDuplicates();
				}
			}
		}

		internal bool IsOnPathToSortFilterSource(int index)
		{
			if (m_sortFilterScopeInfo != null && m_sortFilterScopeIndex != null && -1 != m_sortFilterScopeIndex[index])
			{
				return true;
			}
			return false;
		}

		internal int[] GetGroupExpressionFieldIndices()
		{
			if (m_groupExpressionFieldIndices == null && m_groupExpressions != null && 0 < m_groupExpressions.Count)
			{
				Global.Tracer.Assert(m_groupExpressions != null && 0 < m_groupExpressions.Count, "(null != m_groupExpressions && 0 < m_groupExpressions.Count)");
				m_groupExpressionFieldIndices = new int[m_groupExpressions.Count];
				for (int i = 0; i < m_groupExpressions.Count; i++)
				{
					m_groupExpressionFieldIndices[i] = -2;
					ExpressionInfo expressionInfo = m_groupExpressions[i];
					if (expressionInfo.Type == ExpressionInfo.Types.Field)
					{
						m_groupExpressionFieldIndices[i] = expressionInfo.IntValue;
					}
					else if (expressionInfo.Type == ExpressionInfo.Types.Constant)
					{
						m_groupExpressionFieldIndices[i] = -1;
					}
				}
			}
			return m_groupExpressionFieldIndices;
		}

		internal Grouping CloneForDomainScope(AutomaticSubtotalContext context, ReportHierarchyNode cloneOwner)
		{
			Grouping grouping = new Grouping(ConstructionPhase.Publishing);
			grouping.m_isClone = true;
			grouping.m_ID = context.GenerateID();
			grouping.m_owner = cloneOwner;
			cloneOwner.OriginalScopeID = Owner.DataScopeInfo.ScopeID;
			Global.Tracer.Assert(m_name != null, "Group Name cannot be null");
			grouping.m_name = context.CreateAndRegisterUniqueGroupName(m_name, m_isClone, isDomainScope: true);
			CloneGroupExpressions(context, grouping);
			return grouping;
		}

		internal object PublishClone(AutomaticSubtotalContext context, ReportHierarchyNode owner)
		{
			Grouping grouping = (Grouping)MemberwiseClone();
			grouping.m_isClone = true;
			grouping.m_ID = context.GenerateID();
			grouping.m_owner = owner;
			if (DomainScope != null)
			{
				grouping.DomainScope = context.GetNewScopeName(DomainScope);
				if (string.CompareOrdinal(DomainScope, grouping.DomainScope) != 0)
				{
					context.DomainScopeGroups.Add(grouping);
				}
				else
				{
					grouping.m_scopeIDForDomainScope = Owner.DataScopeInfo.ScopeID;
				}
			}
			context.AddAggregateHolder(grouping);
			Global.Tracer.Assert(m_name != null);
			grouping.m_name = context.CreateAndRegisterUniqueGroupName(m_name, m_isClone);
			context.AddSortTarget(grouping.m_name, grouping);
			CloneGroupExpressions(context, grouping);
			if (m_groupLabel != null)
			{
				grouping.m_groupLabel = (ExpressionInfo)m_groupLabel.PublishClone(context);
			}
			if (m_sortDirections != null)
			{
				grouping.m_sortDirections = new List<bool>(m_sortDirections.Count);
				foreach (bool sortDirection in m_sortDirections)
				{
					grouping.m_sortDirections.Add(sortDirection);
				}
			}
			grouping.m_aggregates = new List<DataAggregateInfo>();
			grouping.m_recursiveAggregates = new List<DataAggregateInfo>();
			grouping.m_postSortAggregates = new List<DataAggregateInfo>();
			if (m_filters != null)
			{
				grouping.m_filters = new List<Filter>(m_filters.Count);
				foreach (Filter filter in m_filters)
				{
					grouping.m_filters.Add((Filter)filter.PublishClone(context));
				}
			}
			if (m_parent != null)
			{
				grouping.m_parent = new List<ExpressionInfo>(m_parent.Count);
				foreach (ExpressionInfo item in m_parent)
				{
					grouping.m_parent.Add((ExpressionInfo)item.PublishClone(context));
				}
			}
			if (m_dataElementName != null)
			{
				grouping.m_dataElementName = (string)m_dataElementName.Clone();
			}
			if (m_userSortExpressions != null)
			{
				grouping.m_userSortExpressions = new List<ExpressionInfo>(m_userSortExpressions.Count);
				foreach (ExpressionInfo userSortExpression in m_userSortExpressions)
				{
					grouping.m_userSortExpressions.Add((ExpressionInfo)userSortExpression.PublishClone(context));
				}
			}
			if (m_variables != null)
			{
				grouping.m_variables = new List<Variable>(m_variables.Count);
				foreach (Variable variable in m_variables)
				{
					grouping.m_variables.Add((Variable)variable.PublishClone(context));
				}
			}
			if (m_nonDetailSortFiltersInScope != null)
			{
				grouping.m_nonDetailSortFiltersInScope = new InScopeSortFilterHashtable(m_nonDetailSortFiltersInScope.Count);
				foreach (DictionaryEntry item2 in m_nonDetailSortFiltersInScope)
				{
					List<int> obj = (List<int>)item2.Value;
					List<int> list = new List<int>(obj.Count);
					foreach (int item3 in obj)
					{
						list.Add(item3);
					}
					grouping.m_nonDetailSortFiltersInScope.Add(item2.Key, list);
				}
			}
			if (m_detailSortFiltersInScope != null)
			{
				grouping.m_detailSortFiltersInScope = new InScopeSortFilterHashtable(m_detailSortFiltersInScope.Count);
				foreach (DictionaryEntry item4 in m_detailSortFiltersInScope)
				{
					List<int> obj2 = (List<int>)item4.Value;
					List<int> list2 = new List<int>(obj2.Count);
					foreach (int item5 in obj2)
					{
						list2.Add(item5);
					}
					grouping.m_detailSortFiltersInScope.Add(item4.Key, list2);
				}
			}
			if (m_pageBreak != null)
			{
				grouping.m_pageBreak = (PageBreak)m_pageBreak.PublishClone(context);
			}
			if (m_pageName != null)
			{
				grouping.m_pageName = (ExpressionInfo)m_pageName.PublishClone(context);
			}
			return grouping;
		}

		private void CloneGroupExpressions(AutomaticSubtotalContext context, Grouping clone)
		{
			if (m_groupExpressions == null)
			{
				return;
			}
			clone.m_groupExpressions = new List<ExpressionInfo>(m_groupExpressions.Count);
			foreach (ExpressionInfo groupExpression in m_groupExpressions)
			{
				clone.m_groupExpressions.Add((ExpressionInfo)groupExpression.PublishClone(context));
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			list.Add(new MemberInfo(MemberName.GroupExpressions, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.GroupLabel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SortDirections, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Boolean));
			list.Add(new ReadOnlyMemberInfo(MemberName.PageBreakLocation, Token.Enum));
			list.Add(new MemberInfo(MemberName.Aggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			list.Add(new MemberInfo(MemberName.GroupAndSort, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Filters, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Filter));
			list.Add(new MemberInfo(MemberName.ReportItemsWithHideDuplicates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem));
			list.Add(new MemberInfo(MemberName.Parent, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.RecursiveAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			list.Add(new MemberInfo(MemberName.PostSortAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			list.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			list.Add(new MemberInfo(MemberName.SaveGroupExprValues, Token.Boolean));
			list.Add(new MemberInfo(MemberName.UserSortExpressions, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.NonDetailSortFiltersInScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Int32PrimitiveListHashtable));
			list.Add(new MemberInfo(MemberName.DetailSortFiltersInScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Int32PrimitiveListHashtable));
			list.Add(new MemberInfo(MemberName.Variables, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Variable));
			list.Add(new MemberInfo(MemberName.PageBreak, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PageBreak));
			list.Add(new MemberInfo(MemberName.PageName, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DomainScope, Token.String));
			list.Add(new MemberInfo(MemberName.ScopeIDForDomainScope, Token.Int32));
			list.Add(new MemberInfo(MemberName.NaturalGroup, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Grouping, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(m_name);
					break;
				case MemberName.ID:
					writer.Write(m_ID);
					break;
				case MemberName.GroupExpressions:
					writer.Write(m_groupExpressions);
					break;
				case MemberName.GroupLabel:
					writer.Write(m_groupLabel);
					break;
				case MemberName.SortDirections:
					writer.WriteListOfPrimitives(m_sortDirections);
					break;
				case MemberName.Aggregates:
					writer.Write(m_aggregates);
					break;
				case MemberName.GroupAndSort:
					writer.Write(m_groupAndSort);
					break;
				case MemberName.Filters:
					writer.Write(m_filters);
					break;
				case MemberName.ReportItemsWithHideDuplicates:
					writer.WriteListOfReferences(m_reportItemsWithHideDuplicates);
					break;
				case MemberName.Parent:
					writer.Write(m_parent);
					break;
				case MemberName.RecursiveAggregates:
					writer.Write(m_recursiveAggregates);
					break;
				case MemberName.PostSortAggregates:
					writer.Write(m_postSortAggregates);
					break;
				case MemberName.DataElementName:
					writer.Write(m_dataElementName);
					break;
				case MemberName.DataElementOutput:
					writer.WriteEnum((int)m_dataElementOutput);
					break;
				case MemberName.SaveGroupExprValues:
					writer.Write(m_saveGroupExprValues);
					break;
				case MemberName.UserSortExpressions:
					writer.Write(m_userSortExpressions);
					break;
				case MemberName.NonDetailSortFiltersInScope:
					writer.WriteInt32PrimitiveListHashtable<int>(m_nonDetailSortFiltersInScope);
					break;
				case MemberName.DetailSortFiltersInScope:
					writer.WriteInt32PrimitiveListHashtable<int>(m_detailSortFiltersInScope);
					break;
				case MemberName.Variables:
					writer.Write(m_variables);
					break;
				case MemberName.PageBreak:
					writer.Write(m_pageBreak);
					break;
				case MemberName.PageName:
					writer.Write(m_pageName);
					break;
				case MemberName.DomainScope:
					writer.Write(m_domainScope);
					break;
				case MemberName.ScopeIDForDomainScope:
					writer.Write(m_scopeIDForDomainScope);
					break;
				case MemberName.NaturalGroup:
					writer.Write(m_naturalGroup);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					m_name = reader.ReadString();
					break;
				case MemberName.ID:
					m_ID = reader.ReadInt32();
					break;
				case MemberName.GroupExpressions:
					m_groupExpressions = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.GroupLabel:
					m_groupLabel = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SortDirections:
					m_sortDirections = reader.ReadListOfPrimitives<bool>();
					break;
				case MemberName.PageBreakLocation:
					m_pageBreak = new PageBreak();
					m_pageBreak.BreakLocation = (PageBreakLocation)reader.ReadEnum();
					break;
				case MemberName.Aggregates:
					m_aggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				case MemberName.GroupAndSort:
					m_groupAndSort = reader.ReadBoolean();
					break;
				case MemberName.Filters:
					m_filters = reader.ReadGenericListOfRIFObjects<Filter>();
					break;
				case MemberName.ReportItemsWithHideDuplicates:
					m_reportItemsWithHideDuplicates = reader.ReadGenericListOfReferences<ReportItem>(this);
					break;
				case MemberName.Parent:
					m_parent = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.RecursiveAggregates:
					m_recursiveAggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				case MemberName.PostSortAggregates:
					m_postSortAggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				case MemberName.DataElementName:
					m_dataElementName = reader.ReadString();
					break;
				case MemberName.DataElementOutput:
					m_dataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
					break;
				case MemberName.SaveGroupExprValues:
					m_saveGroupExprValues = reader.ReadBoolean();
					break;
				case MemberName.UserSortExpressions:
					m_userSortExpressions = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.NonDetailSortFiltersInScope:
					m_nonDetailSortFiltersInScope = reader.ReadInt32PrimitiveListHashtable<InScopeSortFilterHashtable, int>();
					break;
				case MemberName.DetailSortFiltersInScope:
					m_detailSortFiltersInScope = reader.ReadInt32PrimitiveListHashtable<InScopeSortFilterHashtable, int>();
					break;
				case MemberName.Variables:
					m_variables = reader.ReadGenericListOfRIFObjects<Variable>();
					break;
				case MemberName.PageBreak:
					m_pageBreak = (PageBreak)reader.ReadRIFObject();
					break;
				case MemberName.PageName:
					m_pageName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DomainScope:
					m_domainScope = reader.ReadString();
					break;
				case MemberName.ScopeIDForDomainScope:
					m_scopeIDForDomainScope = reader.ReadInt32();
					break;
				case MemberName.NaturalGroup:
					m_naturalGroup = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.ReportItemsWithHideDuplicates)
				{
					if (m_reportItemsWithHideDuplicates == null)
					{
						m_reportItemsWithHideDuplicates = new List<ReportItem>();
					}
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					Global.Tracer.Assert(referenceableItems[item.RefID] is ReportItem);
					Global.Tracer.Assert(!m_reportItemsWithHideDuplicates.Contains((ReportItem)referenceableItems[item.RefID]));
					m_reportItemsWithHideDuplicates.Add((ReportItem)referenceableItems[item.RefID]);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Grouping;
		}

		internal void SetExprHost(GroupExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			if (m_exprHost.FilterHostsRemotable != null)
			{
				Global.Tracer.Assert(m_filters != null, "(m_filters != null)");
				int count = m_filters.Count;
				for (int i = 0; i < count; i++)
				{
					m_filters[i].SetExprHost(m_exprHost.FilterHostsRemotable, reportObjectModel);
				}
			}
			if (m_exprHost.ParentExpressionsHost != null)
			{
				m_exprHost.ParentExpressionsHost.SetReportObjectModel(reportObjectModel);
			}
			if (m_exprHost.VariableValueHosts != null)
			{
				m_exprHost.VariableValueHosts.SetReportObjectModel(reportObjectModel);
			}
			if (m_exprHost.UserSortExpressionsHost != null)
			{
				m_exprHost.UserSortExpressionsHost.SetReportObjectModel(reportObjectModel);
			}
			if (m_pageBreak != null && m_exprHost.PageBreakExprHost != null)
			{
				m_pageBreak.SetExprHost(m_exprHost.PageBreakExprHost, reportObjectModel);
			}
		}

		internal string EvaluateGroupingLabelExpression(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_owner, romInstance);
			return context.ReportRuntime.EvaluateGroupingLabelExpression(this, Microsoft.ReportingServices.ReportProcessing.ObjectType.Tablix, m_name);
		}

		internal int GetRecursiveLevel(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_owner, romInstance);
			return m_recursiveLevel;
		}

		internal void SetGroupInstanceExpressionValues(object[] exprValues)
		{
			m_groupInstanceExprValues = exprValues;
		}

		internal object[] GetGroupInstanceExpressionValues(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_owner, romInstance);
			return m_groupInstanceExprValues;
		}

		internal string EvaluatePageName(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_owner, romInstance);
			return context.ReportRuntime.EvaluateGroupingPageNameExpression(this, m_pageName, m_name);
		}
	}
}

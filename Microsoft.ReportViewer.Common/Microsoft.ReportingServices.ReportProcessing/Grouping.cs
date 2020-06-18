using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Grouping : IAggregateHolder, ISortFilterScope
	{
		private string m_name;

		private ExpressionInfoList m_groupExpressions;

		private ExpressionInfo m_groupLabel;

		private BoolList m_sortDirections;

		private bool m_pageBreakAtEnd;

		private bool m_pageBreakAtStart;

		private string m_custom;

		private DataAggregateInfoList m_aggregates;

		private bool m_groupAndSort;

		private FilterList m_filters;

		[Reference]
		private ReportItemList m_reportItemsWithHideDuplicates;

		private ExpressionInfoList m_parent;

		private DataAggregateInfoList m_recursiveAggregates;

		private DataAggregateInfoList m_postSortAggregates;

		private string m_dataElementName;

		private string m_dataCollectionName;

		private DataElementOutputTypes m_dataElementOutput;

		private DataValueList m_customProperties;

		private bool m_saveGroupExprValues;

		private ExpressionInfoList m_userSortExpressions;

		private InScopeSortFilterHashtable m_nonDetailSortFiltersInScope;

		private InScopeSortFilterHashtable m_detailSortFiltersInScope;

		[NonSerialized]
		private IntList m_hideDuplicatesReportItemIDs;

		[NonSerialized]
		private GroupingExprHost m_exprHost;

		[NonSerialized]
		private Hashtable m_scopeNames;

		[NonSerialized]
		private bool m_inPivotCell;

		[NonSerialized]
		private int m_recursiveLevel;

		[NonSerialized]
		private int[] m_groupExpressionFieldIndices;

		[NonSerialized]
		private bool m_hasInnerFilters;

		[NonSerialized]
		private VariantList m_currentGroupExprValues;

		[NonSerialized]
		private ReportHierarchyNode m_owner;

		[NonSerialized]
		private VariantList[] m_sortFilterScopeInfo;

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

		internal BoolList SortDirections
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

		internal ExpressionInfoList GroupExpressions
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

		internal bool PageBreakAtEnd
		{
			get
			{
				return m_pageBreakAtEnd;
			}
			set
			{
				m_pageBreakAtEnd = value;
			}
		}

		internal bool PageBreakAtStart
		{
			get
			{
				return m_pageBreakAtStart;
			}
			set
			{
				m_pageBreakAtStart = value;
			}
		}

		internal string Custom
		{
			get
			{
				return m_custom;
			}
			set
			{
				m_custom = value;
			}
		}

		internal DataAggregateInfoList Aggregates
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

		internal FilterList Filters
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
						Global.Tracer.Assert(m_groupExpressions[i] != null);
						if (ExpressionInfo.Types.Field != m_groupExpressions[i].Type)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		internal ReportItemList ReportItemsWithHideDuplicates
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

		internal ExpressionInfoList Parent
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

		internal DataAggregateInfoList RecursiveAggregates
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

		internal DataAggregateInfoList PostSortAggregates
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

		internal string DataCollectionName
		{
			get
			{
				return m_dataCollectionName;
			}
			set
			{
				m_dataCollectionName = value;
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

		internal DataValueList CustomProperties
		{
			get
			{
				return m_customProperties;
			}
			set
			{
				m_customProperties = value;
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

		internal ExpressionInfoList UserSortExpressions
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

		internal IntList HideDuplicatesReportItemIDs
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

		internal GroupingExprHost ExprHost => m_exprHost;

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

		internal bool InPivotCell
		{
			get
			{
				return m_inPivotCell;
			}
			set
			{
				m_inPivotCell = value;
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

		internal bool HasInnerFilters
		{
			get
			{
				return m_hasInnerFilters;
			}
			set
			{
				m_hasInnerFilters = value;
			}
		}

		internal VariantList CurrentGroupExpressionValues
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

		internal VariantList[] SortFilterScopeInfo
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

		ExpressionInfoList ISortFilterScope.UserSortExpressions
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

		internal Grouping(ConstructionPhase phase)
		{
			if (phase == ConstructionPhase.Publishing)
			{
				m_groupExpressions = new ExpressionInfoList();
				m_aggregates = new DataAggregateInfoList();
				m_postSortAggregates = new DataAggregateInfoList();
				m_recursiveAggregates = new DataAggregateInfoList();
			}
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.GroupingStart(m_name);
			DataRendererInitialize(context);
			if (m_groupExpressions != null)
			{
				for (int i = 0; i < m_groupExpressions.Count; i++)
				{
					ExpressionInfo expressionInfo = m_groupExpressions[i];
					expressionInfo.GroupExpressionInitialize(context);
					context.ExprHostBuilder.GroupingExpression(expressionInfo);
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
				context.ExprHostBuilder.GroupingParentExpressionsStart();
				for (int k = 0; k < m_parent.Count; k++)
				{
					ExpressionInfo expressionInfo2 = m_parent[k];
					expressionInfo2.GroupExpressionInitialize(context);
					context.ExprHostBuilder.GroupingParentExpression(expressionInfo2);
				}
				context.ExprHostBuilder.GroupingParentExpressionsEnd();
			}
			if (m_customProperties != null)
			{
				m_customProperties.Initialize(null, isCustomProperty: true, context);
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
			context.ExprHostBuilder.GroupingEnd();
		}

		DataAggregateInfoList[] IAggregateHolder.GetAggregateLists()
		{
			return new DataAggregateInfoList[1]
			{
				m_aggregates
			};
		}

		DataAggregateInfoList[] IAggregateHolder.GetPostSortAggregateLists()
		{
			return new DataAggregateInfoList[1]
			{
				m_postSortAggregates
			};
		}

		void IAggregateHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(m_aggregates != null);
			if (m_aggregates.Count == 0)
			{
				m_aggregates = null;
			}
			Global.Tracer.Assert(m_postSortAggregates != null);
			if (m_postSortAggregates.Count == 0)
			{
				m_postSortAggregates = null;
			}
			Global.Tracer.Assert(m_recursiveAggregates != null);
			if (m_recursiveAggregates.Count == 0)
			{
				m_recursiveAggregates = null;
			}
		}

		private void DataRendererInitialize(InitializationContext context)
		{
			CLSNameValidator.ValidateDataElementName(ref m_dataElementName, m_name, context.ObjectType, context.ObjectName, "DataElementName", context.ErrorContext);
			CLSNameValidator.ValidateDataElementName(ref m_dataCollectionName, m_dataElementName + "_Collection", context.ObjectType, context.ObjectName, "DataCollectionName", context.ErrorContext);
		}

		internal void AddReportItemWithHideDuplicates(ReportItem reportItem)
		{
			if (m_reportItemsWithHideDuplicates == null)
			{
				m_reportItemsWithHideDuplicates = new ReportItemList();
			}
			m_reportItemsWithHideDuplicates.Add(reportItem);
		}

		internal void SetExprHost(GroupingExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			if (m_exprHost.FilterHostsRemotable != null)
			{
				Global.Tracer.Assert(m_filters != null);
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
			if (m_exprHost.CustomPropertyHostsRemotable != null)
			{
				Global.Tracer.Assert(m_customProperties != null);
				m_customProperties.SetExprHost(m_exprHost.CustomPropertyHostsRemotable, reportObjectModel);
			}
			if (m_exprHost.UserSortExpressionsHost != null)
			{
				m_exprHost.UserSortExpressionsHost.SetReportObjectModel(reportObjectModel);
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
			if (m_groupExpressionFieldIndices == null)
			{
				Global.Tracer.Assert(m_groupExpressions != null && 0 < m_groupExpressions.Count);
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

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Name, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.GroupExpressions, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.GroupLabel, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.SortDirections, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.BoolList));
			memberInfoList.Add(new MemberInfo(MemberName.PageBreakAtEnd, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.PageBreakAtStart, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Custom, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Aggregates, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataAggregateInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.GroupAndSort, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Filters, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.FilterList));
			memberInfoList.Add(new MemberInfo(MemberName.ReportItemsWithHideDuplicates, Token.Reference, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemList));
			memberInfoList.Add(new MemberInfo(MemberName.Parent, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.RecursiveAggregates, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataAggregateInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.PostSortAggregates, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataAggregateInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DataCollectionName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.CustomProperties, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueList));
			memberInfoList.Add(new MemberInfo(MemberName.SaveGroupExprValues, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.UserSortExpressions, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.NonDetailSortFiltersInScope, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InScopeSortFilterHashtable));
			memberInfoList.Add(new MemberInfo(MemberName.DetailSortFiltersInScope, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InScopeSortFilterHashtable));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}

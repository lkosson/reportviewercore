using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal abstract class DataRegion : ReportItem, IPageBreakItem, IAggregateHolder, ISortFilterScope
	{
		protected string m_dataSetName;

		protected ExpressionInfo m_noRows;

		protected bool m_pageBreakAtEnd;

		protected bool m_pageBreakAtStart;

		protected bool m_keepTogether;

		protected IntList m_repeatSiblings;

		protected FilterList m_filters;

		protected DataAggregateInfoList m_aggregates;

		protected DataAggregateInfoList m_postSortAggregates;

		protected ExpressionInfoList m_userSortExpressions;

		protected InScopeSortFilterHashtable m_detailSortFiltersInScope;

		[NonSerialized]
		protected ReportProcessing.RuntimeDataRegionObj m_runtimeDataRegionObj;

		[NonSerialized]
		protected PageBreakStates m_pagebreakState;

		[NonSerialized]
		protected Hashtable m_scopeNames;

		[NonSerialized]
		protected bool m_inPivotCell;

		[NonSerialized]
		protected bool[] m_isSortFilterTarget;

		[NonSerialized]
		protected bool[] m_isSortFilterExpressionScope;

		[NonSerialized]
		protected int[] m_sortFilterSourceDetailScopeInfo;

		[NonSerialized]
		protected int m_currentDetailRowIndex = -1;

		internal string DataSetName
		{
			get
			{
				return m_dataSetName;
			}
			set
			{
				m_dataSetName = value;
			}
		}

		internal ExpressionInfo NoRows
		{
			get
			{
				return m_noRows;
			}
			set
			{
				m_noRows = value;
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

		internal bool KeepTogether
		{
			get
			{
				return m_keepTogether;
			}
			set
			{
				m_keepTogether = value;
			}
		}

		internal IntList RepeatSiblings
		{
			get
			{
				return m_repeatSiblings;
			}
			set
			{
				m_repeatSiblings = value;
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

		internal ReportProcessing.RuntimeDataRegionObj RuntimeDataRegionObj
		{
			get
			{
				return m_runtimeDataRegionObj;
			}
			set
			{
				m_runtimeDataRegionObj = value;
			}
		}

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

		internal int[] SortFilterSourceDetailScopeInfo
		{
			get
			{
				return m_sortFilterSourceDetailScopeInfo;
			}
			set
			{
				m_sortFilterSourceDetailScopeInfo = value;
			}
		}

		internal int CurrentDetailRowIndex
		{
			get
			{
				return m_currentDetailRowIndex;
			}
			set
			{
				m_currentDetailRowIndex = value;
			}
		}

		protected abstract DataRegionExprHost DataRegionExprHost
		{
			get;
		}

		int ISortFilterScope.ID => m_ID;

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
				if (DataRegionExprHost == null)
				{
					return null;
				}
				return DataRegionExprHost.UserSortExpressionsHost;
			}
		}

		protected DataRegion(ReportItem parent)
			: base(parent)
		{
		}

		protected DataRegion(int id, ReportItem parent)
			: base(id, parent)
		{
			m_aggregates = new DataAggregateInfoList();
			m_postSortAggregates = new DataAggregateInfoList();
		}

		internal override bool Initialize(InitializationContext context)
		{
			base.Initialize(context);
			if (m_filters != null)
			{
				for (int i = 0; i < m_filters.Count; i++)
				{
					m_filters[i].Initialize(context);
				}
			}
			if (m_noRows != null)
			{
				m_noRows.Initialize("NoRows", context);
				context.ExprHostBuilder.GenericNoRows(m_noRows);
			}
			if (m_userSortExpressions != null)
			{
				context.ExprHostBuilder.UserSortExpressionsStart();
				for (int j = 0; j < m_userSortExpressions.Count; j++)
				{
					ExpressionInfo expression = m_userSortExpressions[j];
					context.ExprHostBuilder.UserSortExpression(expression);
				}
				context.ExprHostBuilder.UserSortExpressionsEnd();
			}
			return false;
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
		}

		bool IPageBreakItem.IgnorePageBreaks()
		{
			if (m_pagebreakState == PageBreakStates.Unknown)
			{
				if (SharedHiddenState.Never != Visibility.GetSharedHidden(m_visibility))
				{
					m_pagebreakState = PageBreakStates.CanIgnore;
				}
				else
				{
					m_pagebreakState = PageBreakStates.CannotIgnore;
				}
			}
			if (PageBreakStates.CanIgnore == m_pagebreakState)
			{
				return true;
			}
			return false;
		}

		bool IPageBreakItem.HasPageBreaks(bool atStart)
		{
			if ((atStart && m_pageBreakAtStart) || (!atStart && m_pageBreakAtEnd))
			{
				return true;
			}
			return false;
		}

		protected void DataRegionSetExprHost(DataRegionExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null);
			ReportItemSetExprHost(exprHost, reportObjectModel);
			if (exprHost.FilterHostsRemotable != null)
			{
				Global.Tracer.Assert(m_filters != null);
				int count = m_filters.Count;
				for (int i = 0; i < count; i++)
				{
					m_filters[i].SetExprHost(exprHost.FilterHostsRemotable, reportObjectModel);
				}
			}
			if (exprHost.UserSortExpressionsHost != null)
			{
				exprHost.UserSortExpressionsHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.DataSetName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.NoRows, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.PageBreakAtEnd, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.PageBreakAtStart, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.KeepTogether, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.RepeatSiblings, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.IntList));
			memberInfoList.Add(new MemberInfo(MemberName.Filters, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.FilterList));
			memberInfoList.Add(new MemberInfo(MemberName.Aggregates, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataAggregateInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.PostSortAggregates, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataAggregateInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.UserSortExpressions, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.DetailSortFiltersInScope, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InScopeSortFilterHashtable));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItem, memberInfoList);
		}
	}
}

using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.ReportingServices.ReportRendering;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class ReportHierarchyNode : IDOwner, IPageBreakItem
	{
		protected Grouping m_grouping;

		protected Sorting m_sorting;

		protected ReportHierarchyNode m_innerHierarchy;

		[Reference]
		protected DataRegion m_dataRegionDef;

		[NonSerialized]
		private PageBreakStates m_pagebreakState;

		[NonSerialized]
		private DynamicGroupExprHost m_exprHost;

		internal Grouping Grouping
		{
			get
			{
				return m_grouping;
			}
			set
			{
				m_grouping = value;
				if (m_grouping != null)
				{
					m_grouping.Owner = this;
				}
			}
		}

		internal Sorting Sorting
		{
			get
			{
				return m_sorting;
			}
			set
			{
				m_sorting = value;
			}
		}

		internal ReportHierarchyNode InnerHierarchy
		{
			get
			{
				return m_innerHierarchy;
			}
			set
			{
				m_innerHierarchy = value;
			}
		}

		internal DataRegion DataRegionDef
		{
			get
			{
				return m_dataRegionDef;
			}
			set
			{
				m_dataRegionDef = value;
			}
		}

		internal ReportHierarchyNode()
		{
		}

		internal ReportHierarchyNode(int id, DataRegion dataRegionDef)
			: base(id)
		{
			m_dataRegionDef = dataRegionDef;
		}

		internal void Initialize(InitializationContext context)
		{
			if (m_grouping != null)
			{
				m_grouping.Initialize(context);
			}
			if (m_sorting != null)
			{
				m_sorting.Initialize(context);
			}
		}

		bool IPageBreakItem.IgnorePageBreaks()
		{
			return false;
		}

		protected bool IgnorePageBreaks(Visibility visibility)
		{
			if (m_pagebreakState == PageBreakStates.Unknown)
			{
				if (SharedHiddenState.Never != Visibility.GetSharedHidden(visibility))
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
			if (m_grouping == null)
			{
				return false;
			}
			if ((atStart && m_grouping.PageBreakAtStart) || (!atStart && m_grouping.PageBreakAtEnd))
			{
				return true;
			}
			return false;
		}

		protected void ReportHierarchyNodeSetExprHost(DynamicGroupExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null);
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			ReportHierarchyNodeSetExprHost(m_exprHost.GroupingHost, m_exprHost.SortingHost, reportObjectModel);
		}

		internal void ReportHierarchyNodeSetExprHost(GroupingExprHost groupingExprHost, SortingExprHost sortingExprHost, ObjectModelImpl reportObjectModel)
		{
			if (groupingExprHost != null)
			{
				Global.Tracer.Assert(m_grouping != null);
				m_grouping.SetExprHost(groupingExprHost, reportObjectModel);
			}
			if (sortingExprHost != null)
			{
				Global.Tracer.Assert(m_sorting != null);
				m_sorting.SetExprHost(sortingExprHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Grouping, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Grouping));
			memberInfoList.Add(new MemberInfo(MemberName.Sorting, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Sorting));
			memberInfoList.Add(new MemberInfo(MemberName.InnerHierarchy, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportHierarchyNode));
			memberInfoList.Add(new MemberInfo(MemberName.DataRegionDef, Token.Reference, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataRegion));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.IDOwner, memberInfoList);
		}
	}
}

using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal abstract class PivotHeading : ReportHierarchyNode
	{
		protected Visibility m_visibility;

		protected Subtotal m_subtotal;

		protected int m_level;

		protected bool m_isColumn;

		protected bool m_hasExprHost;

		protected int m_subtotalSpan;

		private IntList m_IDs;

		[NonSerialized]
		protected int m_numberOfStatics;

		[NonSerialized]
		protected DataAggregateInfoList m_aggregates;

		[NonSerialized]
		protected DataAggregateInfoList m_postSortAggregates;

		[NonSerialized]
		protected DataAggregateInfoList m_recursiveAggregates;

		[NonSerialized]
		protected AggregatesImpl m_outermostSTCellRVCol;

		[NonSerialized]
		protected AggregatesImpl m_cellRVCol;

		[NonSerialized]
		protected AggregatesImpl[] m_outermostSTCellScopedRVCollections;

		[NonSerialized]
		protected AggregatesImpl[] m_cellScopedRVCollections;

		[NonSerialized]
		protected Hashtable[] m_cellScopeNames;

		internal PivotHeading SubHeading
		{
			get
			{
				return (PivotHeading)m_innerHierarchy;
			}
			set
			{
				m_innerHierarchy = value;
			}
		}

		internal Visibility Visibility
		{
			get
			{
				return m_visibility;
			}
			set
			{
				m_visibility = value;
			}
		}

		internal Subtotal Subtotal
		{
			get
			{
				return m_subtotal;
			}
			set
			{
				m_subtotal = value;
			}
		}

		internal int Level
		{
			get
			{
				return m_level;
			}
			set
			{
				m_level = value;
			}
		}

		internal bool IsColumn
		{
			get
			{
				return m_isColumn;
			}
			set
			{
				m_isColumn = value;
			}
		}

		internal bool HasExprHost
		{
			get
			{
				return m_hasExprHost;
			}
			set
			{
				m_hasExprHost = value;
			}
		}

		internal int SubtotalSpan
		{
			get
			{
				return m_subtotalSpan;
			}
			set
			{
				m_subtotalSpan = value;
			}
		}

		internal IntList IDs
		{
			get
			{
				return m_IDs;
			}
			set
			{
				m_IDs = value;
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

		internal int NumberOfStatics
		{
			get
			{
				return m_numberOfStatics;
			}
			set
			{
				m_numberOfStatics = value;
			}
		}

		internal AggregatesImpl OutermostSTCellRVCol
		{
			get
			{
				return m_outermostSTCellRVCol;
			}
			set
			{
				m_outermostSTCellRVCol = value;
			}
		}

		internal AggregatesImpl CellRVCol
		{
			get
			{
				return m_cellRVCol;
			}
			set
			{
				m_cellRVCol = value;
			}
		}

		internal AggregatesImpl[] OutermostSTCellScopedRVCollections
		{
			get
			{
				return m_outermostSTCellScopedRVCollections;
			}
			set
			{
				m_outermostSTCellScopedRVCollections = value;
			}
		}

		internal AggregatesImpl[] CellScopedRVCollections
		{
			get
			{
				return m_cellScopedRVCollections;
			}
			set
			{
				m_cellScopedRVCollections = value;
			}
		}

		internal Hashtable[] CellScopeNames
		{
			get
			{
				return m_cellScopeNames;
			}
			set
			{
				m_cellScopeNames = value;
			}
		}

		internal PivotHeading()
		{
		}

		internal PivotHeading(int id, DataRegion matrixDef)
			: base(id, matrixDef)
		{
			m_aggregates = new DataAggregateInfoList();
			m_postSortAggregates = new DataAggregateInfoList();
			m_recursiveAggregates = new DataAggregateInfoList();
		}

		internal void CopySubHeadingAggregates()
		{
			if (SubHeading != null)
			{
				SubHeading.CopySubHeadingAggregates();
				Pivot.CopyAggregates(SubHeading.Aggregates, m_aggregates);
				Pivot.CopyAggregates(SubHeading.PostSortAggregates, m_postSortAggregates);
				Pivot.CopyAggregates(SubHeading.RecursiveAggregates, m_aggregates);
			}
		}

		internal void TransferHeadingAggregates()
		{
			if (SubHeading != null)
			{
				SubHeading.TransferHeadingAggregates();
			}
			if (m_grouping != null)
			{
				for (int i = 0; i < m_aggregates.Count; i++)
				{
					m_grouping.Aggregates.Add(m_aggregates[i]);
				}
			}
			m_aggregates = null;
			if (m_grouping != null)
			{
				for (int j = 0; j < m_postSortAggregates.Count; j++)
				{
					m_grouping.PostSortAggregates.Add(m_postSortAggregates[j]);
				}
			}
			m_postSortAggregates = null;
			if (m_grouping != null)
			{
				for (int k = 0; k < m_recursiveAggregates.Count; k++)
				{
					m_grouping.RecursiveAggregates.Add(m_recursiveAggregates[k]);
				}
			}
			m_recursiveAggregates = null;
		}

		internal PivotHeading GetInnerStaticHeading()
		{
			PivotHeading pivotHeading = null;
			Pivot pivot = (Pivot)m_dataRegionDef;
			pivotHeading = ((!m_isColumn) ? pivot.PivotStaticRows : pivot.PivotStaticColumns);
			if (pivotHeading != null && pivotHeading.Level > m_level)
			{
				return pivotHeading;
			}
			return null;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Visibility, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Visibility));
			memberInfoList.Add(new MemberInfo(MemberName.Subtotal, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Subtotal));
			memberInfoList.Add(new MemberInfo(MemberName.Level, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.IsColumn, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.HasExprHost, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.SubtotalSpan, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.IDs, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.IntList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportHierarchyNode, memberInfoList);
		}
	}
}

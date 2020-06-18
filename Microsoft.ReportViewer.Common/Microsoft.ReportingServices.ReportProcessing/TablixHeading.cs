using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal abstract class TablixHeading : ReportHierarchyNode
	{
		protected bool m_subtotal;

		protected bool m_isColumn;

		protected int m_level;

		protected bool m_hasExprHost;

		protected int m_headingSpan = 1;

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

		internal new ReportHierarchyNode InnerHierarchy
		{
			get
			{
				throw new InvalidOperationException();
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		internal bool Subtotal
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

		internal int HeadingSpan
		{
			get
			{
				return m_headingSpan;
			}
			set
			{
				m_headingSpan = value;
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

		internal TablixHeading()
		{
		}

		internal TablixHeading(int id, DataRegion dataRegionDef)
			: base(id, dataRegionDef)
		{
			m_aggregates = new DataAggregateInfoList();
			m_postSortAggregates = new DataAggregateInfoList();
			m_recursiveAggregates = new DataAggregateInfoList();
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Subtotal, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.IsColumn, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Level, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.HasExprHost, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.HeadingSpan, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportHierarchyNode, memberInfoList);
		}
	}
}

using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal abstract class Tablix : DataRegion, IAggregateHolder, IRunningValueHolder
	{
		private int m_columnCount;

		private int m_rowCount;

		protected DataAggregateInfoList m_cellAggregates;

		protected Pivot.ProcessingInnerGroupings m_processingInnerGrouping;

		protected RunningValueInfoList m_runningValues;

		protected DataAggregateInfoList m_cellPostSortAggregates;

		[NonSerialized]
		protected ReportProcessing.RuntimeTablixGroupRootObj m_currentOuterHeadingGroupRoot;

		[NonSerialized]
		protected int m_innermostRowFilterLevel = -1;

		[NonSerialized]
		protected int m_innermostColumnFilterLevel = -1;

		[NonSerialized]
		protected int[] m_outerGroupingIndexes;

		[NonSerialized]
		protected ReportProcessing.AggregateRowInfo[] m_outerGroupingAggregateRowInfo;

		[NonSerialized]
		protected ReportProcessing.AggregateRowInfo m_tablixAggregateRowInfo;

		[NonSerialized]
		protected bool m_processCellRunningValues;

		[NonSerialized]
		protected bool m_processOutermostSTCellRunningValues;

		internal int ColumnCount
		{
			get
			{
				return m_columnCount;
			}
			set
			{
				m_columnCount = value;
			}
		}

		internal int RowCount
		{
			get
			{
				return m_rowCount;
			}
			set
			{
				m_rowCount = value;
			}
		}

		internal DataAggregateInfoList CellAggregates
		{
			get
			{
				return m_cellAggregates;
			}
			set
			{
				m_cellAggregates = value;
			}
		}

		internal DataAggregateInfoList CellPostSortAggregates
		{
			get
			{
				return m_cellPostSortAggregates;
			}
			set
			{
				m_cellPostSortAggregates = value;
			}
		}

		internal Pivot.ProcessingInnerGroupings ProcessingInnerGrouping
		{
			get
			{
				return m_processingInnerGrouping;
			}
			set
			{
				m_processingInnerGrouping = value;
			}
		}

		internal RunningValueInfoList RunningValues
		{
			get
			{
				return m_runningValues;
			}
			set
			{
				m_runningValues = value;
			}
		}

		internal abstract TablixHeadingList TablixColumns
		{
			get;
		}

		internal abstract TablixHeadingList TablixRows
		{
			get;
		}

		internal abstract RunningValueInfoList TablixCellRunningValues
		{
			get;
		}

		internal ReportProcessing.RuntimeTablixGroupRootObj CurrentOuterHeadingGroupRoot
		{
			get
			{
				return m_currentOuterHeadingGroupRoot;
			}
			set
			{
				m_currentOuterHeadingGroupRoot = value;
			}
		}

		internal int InnermostRowFilterLevel
		{
			get
			{
				return m_innermostRowFilterLevel;
			}
			set
			{
				m_innermostRowFilterLevel = value;
			}
		}

		internal int InnermostColumnFilterLevel
		{
			get
			{
				return m_innermostColumnFilterLevel;
			}
			set
			{
				m_innermostColumnFilterLevel = value;
			}
		}

		internal int[] OuterGroupingIndexes => m_outerGroupingIndexes;

		internal bool ProcessCellRunningValues
		{
			get
			{
				return m_processCellRunningValues;
			}
			set
			{
				m_processCellRunningValues = value;
			}
		}

		internal bool ProcessOutermostSTCellRunningValues
		{
			get
			{
				return m_processOutermostSTCellRunningValues;
			}
			set
			{
				m_processOutermostSTCellRunningValues = value;
			}
		}

		internal Tablix(ReportItem parent)
			: base(parent)
		{
		}

		internal Tablix(int id, ReportItem parent)
			: base(id, parent)
		{
			m_runningValues = new RunningValueInfoList();
			m_cellAggregates = new DataAggregateInfoList();
			m_cellPostSortAggregates = new DataAggregateInfoList();
		}

		internal static void CopyAggregates(DataAggregateInfoList srcAggregates, DataAggregateInfoList targetAggregates)
		{
			for (int i = 0; i < srcAggregates.Count; i++)
			{
				DataAggregateInfo dataAggregateInfo = srcAggregates[i];
				targetAggregates.Add(dataAggregateInfo);
				dataAggregateInfo.IsCopied = true;
			}
		}

		RunningValueInfoList IRunningValueHolder.GetRunningValueList()
		{
			return m_runningValues;
		}

		void IRunningValueHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(m_runningValues != null);
			if (m_runningValues.Count == 0)
			{
				m_runningValues = null;
			}
		}

		DataAggregateInfoList[] IAggregateHolder.GetAggregateLists()
		{
			return new DataAggregateInfoList[2]
			{
				m_aggregates,
				m_cellAggregates
			};
		}

		DataAggregateInfoList[] IAggregateHolder.GetPostSortAggregateLists()
		{
			return new DataAggregateInfoList[2]
			{
				m_postSortAggregates,
				m_cellPostSortAggregates
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
			Global.Tracer.Assert(m_cellAggregates != null);
			if (m_cellAggregates.Count == 0)
			{
				m_cellAggregates = null;
			}
			Global.Tracer.Assert(m_cellPostSortAggregates != null);
			if (m_cellPostSortAggregates.Count == 0)
			{
				m_cellPostSortAggregates = null;
			}
		}

		internal void SkipStaticHeading(ref TablixHeadingList tablixHeading, ref TablixHeadingList staticHeading)
		{
			if (tablixHeading != null && tablixHeading[0].Grouping == null)
			{
				staticHeading = tablixHeading;
				tablixHeading = tablixHeading.InnerHeadings();
			}
			else
			{
				staticHeading = null;
			}
		}

		internal TablixHeadingList GetOuterHeading()
		{
			if (m_processingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
			{
				return TablixRows;
			}
			return TablixColumns;
		}

		internal abstract TablixHeadingList SkipStatics(TablixHeadingList headings);

		internal abstract int GetDynamicHeadingCount(bool outerGroupings);

		internal void GetHeadingDefState(out TablixHeadingList outermostColumns, out TablixHeadingList outermostRows, out TablixHeadingList staticColumns, out TablixHeadingList staticRows)
		{
			outermostColumns = TablixColumns;
			outermostRows = TablixRows;
			staticColumns = null;
			staticRows = null;
			SkipStaticHeading(ref outermostColumns, ref staticColumns);
			SkipStaticHeading(ref outermostRows, ref staticRows);
		}

		internal int CreateOuterGroupingIndexList()
		{
			int dynamicHeadingCount = GetDynamicHeadingCount(outerGroupings: true);
			if (m_outerGroupingIndexes == null)
			{
				m_outerGroupingIndexes = new int[dynamicHeadingCount];
				m_outerGroupingAggregateRowInfo = new ReportProcessing.AggregateRowInfo[dynamicHeadingCount];
			}
			return dynamicHeadingCount;
		}

		internal abstract Hashtable GetOuterScopeNames(int dynamicLevel);

		internal void SaveTablixAggregateRowInfo(ReportProcessing.ProcessingContext pc)
		{
			if (m_tablixAggregateRowInfo == null)
			{
				m_tablixAggregateRowInfo = new ReportProcessing.AggregateRowInfo();
			}
			m_tablixAggregateRowInfo.SaveAggregateInfo(pc);
		}

		internal void RestoreTablixAggregateRowInfo(ReportProcessing.ProcessingContext pc)
		{
			if (m_tablixAggregateRowInfo != null)
			{
				m_tablixAggregateRowInfo.RestoreAggregateInfo(pc);
			}
		}

		internal void SaveOuterGroupingAggregateRowInfo(int headingLevel, ReportProcessing.ProcessingContext pc)
		{
			Global.Tracer.Assert(m_outerGroupingAggregateRowInfo != null);
			if (m_outerGroupingAggregateRowInfo[headingLevel] == null)
			{
				m_outerGroupingAggregateRowInfo[headingLevel] = new ReportProcessing.AggregateRowInfo();
			}
			m_outerGroupingAggregateRowInfo[headingLevel].SaveAggregateInfo(pc);
		}

		internal void SetCellAggregateRowInfo(int headingLevel, ReportProcessing.ProcessingContext pc)
		{
			Global.Tracer.Assert(m_outerGroupingAggregateRowInfo != null && m_tablixAggregateRowInfo != null);
			m_tablixAggregateRowInfo.CombineAggregateInfo(pc, m_outerGroupingAggregateRowInfo[headingLevel]);
		}

		internal void ResetOutergGroupingAggregateRowInfo()
		{
			Global.Tracer.Assert(m_outerGroupingAggregateRowInfo != null);
			for (int i = 0; i < m_outerGroupingAggregateRowInfo.Length; i++)
			{
				m_outerGroupingAggregateRowInfo[i] = null;
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ColumnCount, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.RowCount, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.CellAggregates, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataAggregateInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.ProcessingInnerGrouping, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.RunningValues, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.RunningValueInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.CellPostSortAggregates, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataAggregateInfoList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataRegion, memberInfoList);
		}
	}
}

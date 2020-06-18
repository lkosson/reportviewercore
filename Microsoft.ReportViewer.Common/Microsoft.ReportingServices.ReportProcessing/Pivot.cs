using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal abstract class Pivot : DataRegion, IAggregateHolder, IRunningValueHolder
	{
		internal enum ProcessingInnerGroupings
		{
			Column,
			Row
		}

		private int m_columnCount;

		private int m_rowCount;

		protected DataAggregateInfoList m_cellAggregates;

		protected ProcessingInnerGroupings m_processingInnerGrouping;

		protected RunningValueInfoList m_runningValues;

		protected DataAggregateInfoList m_cellPostSortAggregates;

		private DataElementOutputTypes m_cellDataElementOutput;

		[NonSerialized]
		protected ReportProcessing.RuntimePivotGroupRootObj m_currentOuterHeadingGroupRoot;

		[NonSerialized]
		protected int m_innermostRowFilterLevel = -1;

		[NonSerialized]
		protected int m_innermostColumnFilterLevel = -1;

		[NonSerialized]
		protected int[] m_outerGroupingIndexes;

		[NonSerialized]
		protected ReportProcessing.AggregateRowInfo[] m_outerGroupingAggregateRowInfo;

		[NonSerialized]
		protected ReportProcessing.AggregateRowInfo m_pivotAggregateRowInfo;

		[NonSerialized]
		protected bool m_processCellRunningValues;

		[NonSerialized]
		protected bool m_processOutermostSTCellRunningValues;

		internal abstract PivotHeading PivotColumns
		{
			get;
		}

		internal abstract PivotHeading PivotRows
		{
			get;
		}

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

		internal abstract RunningValueInfoList PivotCellRunningValues
		{
			get;
		}

		internal ProcessingInnerGroupings ProcessingInnerGrouping
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

		internal abstract PivotHeading PivotStaticColumns
		{
			get;
		}

		internal abstract PivotHeading PivotStaticRows
		{
			get;
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

		internal DataElementOutputTypes CellDataElementOutput
		{
			get
			{
				return m_cellDataElementOutput;
			}
			set
			{
				m_cellDataElementOutput = value;
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

		internal ReportProcessing.RuntimePivotGroupRootObj CurrentOuterHeadingGroupRoot
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

		internal Pivot(ReportItem parent)
			: base(parent)
		{
		}

		internal Pivot(int id, ReportItem parent)
			: base(id, parent)
		{
			m_runningValues = new RunningValueInfoList();
			m_cellAggregates = new DataAggregateInfoList();
			m_cellPostSortAggregates = new DataAggregateInfoList();
		}

		internal void CopyHeadingAggregates(PivotHeading heading)
		{
			if (heading != null)
			{
				heading.CopySubHeadingAggregates();
				CopyAggregates(heading.Aggregates, m_aggregates);
				CopyAggregates(heading.PostSortAggregates, m_postSortAggregates);
				CopyAggregates(heading.RecursiveAggregates, m_aggregates);
			}
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

		internal bool SubtotalInInnerHeading(ref PivotHeading innerHeading, ref PivotHeading staticHeading)
		{
			SkipStaticHeading(ref innerHeading, ref staticHeading);
			if (innerHeading != null && innerHeading.Subtotal != null)
			{
				return true;
			}
			return false;
		}

		internal void SkipStaticHeading(ref PivotHeading pivotHeading, ref PivotHeading staticHeading)
		{
			if (pivotHeading != null && pivotHeading.Grouping == null)
			{
				staticHeading = pivotHeading;
				pivotHeading = (PivotHeading)pivotHeading.InnerHierarchy;
			}
			else
			{
				staticHeading = null;
			}
		}

		internal void GetHeadingDefState(out PivotHeading outermostColumn, out bool outermostColumnSubtotal, out PivotHeading staticColumn, out PivotHeading outermostRow, out bool outermostRowSubtotal, out PivotHeading staticRow)
		{
			outermostRowSubtotal = false;
			outermostColumnSubtotal = false;
			staticRow = null;
			outermostRow = PivotRows;
			outermostRowSubtotal = SubtotalInInnerHeading(ref outermostRow, ref staticRow);
			if (outermostRow == null)
			{
				outermostRowSubtotal = true;
			}
			staticColumn = null;
			outermostColumn = PivotColumns;
			outermostColumnSubtotal = SubtotalInInnerHeading(ref outermostColumn, ref staticColumn);
			if (outermostColumn == null)
			{
				outermostColumnSubtotal = true;
			}
		}

		internal PivotHeading GetPivotHeading(bool outerHeading)
		{
			if ((outerHeading && m_processingInnerGrouping == ProcessingInnerGroupings.Column) || (!outerHeading && m_processingInnerGrouping == ProcessingInnerGroupings.Row))
			{
				return PivotRows;
			}
			return PivotColumns;
		}

		internal PivotHeading GetOuterHeading(int level)
		{
			PivotHeading pivotHeading = GetPivotHeading(outerHeading: true);
			PivotHeading staticHeading = null;
			for (int i = 0; i <= level; i++)
			{
				SkipStaticHeading(ref pivotHeading, ref staticHeading);
				if (pivotHeading != null && i < level)
				{
					pivotHeading = (PivotHeading)pivotHeading.InnerHierarchy;
				}
			}
			return pivotHeading;
		}

		internal int GetDynamicHeadingCount(bool outerGroupings)
		{
			int num;
			if ((outerGroupings && m_processingInnerGrouping == ProcessingInnerGroupings.Column) || (!outerGroupings && m_processingInnerGrouping == ProcessingInnerGroupings.Row))
			{
				num = m_rowCount;
				if (PivotStaticRows != null)
				{
					num--;
				}
			}
			else
			{
				num = m_columnCount;
				if (PivotStaticColumns != null)
				{
					num--;
				}
			}
			return num;
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

		internal Hashtable GetOuterScopeNames(int dynamicLevel)
		{
			Hashtable hashtable = new Hashtable();
			PivotHeading pivotHeading = GetPivotHeading(outerHeading: true);
			int num = 0;
			while (num <= dynamicLevel && pivotHeading != null)
			{
				if (pivotHeading.Grouping != null)
				{
					hashtable.Add(pivotHeading.Grouping.Name, pivotHeading.Grouping);
					num++;
				}
				pivotHeading = (PivotHeading)pivotHeading.InnerHierarchy;
			}
			return hashtable;
		}

		internal void SavePivotAggregateRowInfo(ReportProcessing.ProcessingContext pc)
		{
			if (m_pivotAggregateRowInfo == null)
			{
				m_pivotAggregateRowInfo = new ReportProcessing.AggregateRowInfo();
			}
			m_pivotAggregateRowInfo.SaveAggregateInfo(pc);
		}

		internal void RestorePivotAggregateRowInfo(ReportProcessing.ProcessingContext pc)
		{
			if (m_pivotAggregateRowInfo != null)
			{
				m_pivotAggregateRowInfo.RestoreAggregateInfo(pc);
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
			Global.Tracer.Assert(m_outerGroupingAggregateRowInfo != null && m_pivotAggregateRowInfo != null);
			m_pivotAggregateRowInfo.CombineAggregateInfo(pc, m_outerGroupingAggregateRowInfo[headingLevel]);
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
			memberInfoList.Add(new MemberInfo(MemberName.CellDataElementOutput, Token.Enum));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataRegion, memberInfoList);
		}
	}
}

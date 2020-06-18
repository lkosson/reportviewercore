using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class RuntimeUserSortTargetInfo
	{
		private ReportProcessing.BTreeNode m_sortTree;

		private ReportProcessing.AggregateRowList m_aggregateRows;

		private IntList m_sortFilterInfoIndices;

		private Hashtable m_targetForNonDetailSort;

		private Hashtable m_targetForDetailSort;

		internal ReportProcessing.BTreeNode SortTree
		{
			get
			{
				return m_sortTree;
			}
			set
			{
				m_sortTree = value;
			}
		}

		internal ReportProcessing.AggregateRowList AggregateRows
		{
			get
			{
				return m_aggregateRows;
			}
			set
			{
				m_aggregateRows = value;
			}
		}

		internal IntList SortFilterInfoIndices
		{
			get
			{
				return m_sortFilterInfoIndices;
			}
			set
			{
				m_sortFilterInfoIndices = value;
			}
		}

		internal bool TargetForNonDetailSort => m_targetForNonDetailSort != null;

		internal RuntimeUserSortTargetInfo(ReportProcessing.IHierarchyObj owner, int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
		{
			AddSortInfo(owner, sortInfoIndex, sortInfo);
		}

		internal void AddSortInfo(ReportProcessing.IHierarchyObj owner, int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
		{
			if (sortInfo.EventSource.UserSort.SortExpressionScope != null || owner.IsDetail)
			{
				if (sortInfo.EventSource.UserSort.SortExpressionScope == null)
				{
					AddSortInfoIndex(sortInfoIndex, sortInfo);
				}
				if (m_sortTree == null)
				{
					m_sortTree = new ReportProcessing.BTreeNode(owner);
				}
			}
			if (sortInfo.EventSource.UserSort.SortExpressionScope != null)
			{
				if (m_targetForNonDetailSort == null)
				{
					m_targetForNonDetailSort = new Hashtable();
				}
				m_targetForNonDetailSort.Add(sortInfoIndex, null);
			}
			else
			{
				if (m_targetForDetailSort == null)
				{
					m_targetForDetailSort = new Hashtable();
				}
				m_targetForDetailSort.Add(sortInfoIndex, null);
			}
		}

		internal void AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
		{
			Global.Tracer.Assert(sortInfo.EventSource.UserSort.SortExpressionScope == null || !sortInfo.TargetSortFilterInfoAdded);
			if (m_sortFilterInfoIndices == null)
			{
				m_sortFilterInfoIndices = new IntList();
			}
			m_sortFilterInfoIndices.Add(sortInfoIndex);
			sortInfo.TargetSortFilterInfoAdded = true;
		}

		internal void ResetTargetForNonDetailSort()
		{
			m_targetForNonDetailSort = null;
		}

		internal bool IsTargetForSort(int index, bool detailSort)
		{
			Hashtable hashtable = m_targetForNonDetailSort;
			if (detailSort)
			{
				hashtable = m_targetForDetailSort;
			}
			if (hashtable != null && hashtable.Contains(index))
			{
				return true;
			}
			return false;
		}

		internal void MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo, ReportProcessing.IHierarchyObj sortTarget)
		{
			if (m_targetForNonDetailSort != null)
			{
				foreach (int key in m_targetForNonDetailSort.Keys)
				{
					if (runtimeSortFilterInfo[key].EventTarget == sortTarget)
					{
						Global.Tracer.Assert(!runtimeSortFilterInfo[key].Processed, "(!runtimeSortFilterInfo[index].Processed)");
						runtimeSortFilterInfo[key].Processed = true;
					}
				}
			}
			if (m_targetForDetailSort == null)
			{
				return;
			}
			foreach (int key2 in m_targetForDetailSort.Keys)
			{
				if (runtimeSortFilterInfo[key2].EventTarget == sortTarget)
				{
					Global.Tracer.Assert(!runtimeSortFilterInfo[key2].Processed, "(!runtimeSortFilterInfo[index].Processed)");
					runtimeSortFilterInfo[key2].Processed = true;
				}
			}
		}

		internal void EnterProcessUserSortPhase(ReportProcessing.ProcessingContext pc)
		{
			if (m_sortFilterInfoIndices != null)
			{
				for (int i = 0; i < m_sortFilterInfoIndices.Count; i++)
				{
					pc.UserSortFilterContext.EnterProcessUserSortPhase(i);
				}
			}
		}

		internal void LeaveProcessUserSortPhase(ReportProcessing.ProcessingContext pc)
		{
			if (m_sortFilterInfoIndices != null)
			{
				for (int i = 0; i < m_sortFilterInfoIndices.Count; i++)
				{
					pc.UserSortFilterContext.LeaveProcessUserSortPhase(i);
				}
			}
		}
	}
}

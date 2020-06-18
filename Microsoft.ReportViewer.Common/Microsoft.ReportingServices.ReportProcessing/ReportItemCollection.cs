using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportItemCollection : IDOwner, IRunningValueHolder
	{
		private ReportItemList m_nonComputedReportItems;

		private ReportItemList m_computedReportItems;

		private ReportItemIndexerList m_sortedReportItemList;

		private RunningValueInfoList m_runningValues;

		[NonSerialized]
		private bool m_normal;

		[NonSerialized]
		private bool m_unpopulated;

		[NonSerialized]
		private ReportItemList m_entries;

		[NonSerialized]
		private string m_linkToChildName;

		[NonSerialized]
		private bool m_firstInstance = true;

		internal ReportItem this[int index]
		{
			get
			{
				if (m_unpopulated)
				{
					Global.Tracer.Assert(m_entries != null);
					return m_entries[index];
				}
				GetReportItem(index, out bool _, out int _, out ReportItem reportItem);
				return reportItem;
			}
		}

		internal int Count
		{
			get
			{
				if (m_unpopulated)
				{
					Global.Tracer.Assert(m_entries != null);
					return m_entries.Count;
				}
				if (m_sortedReportItemList == null)
				{
					return 0;
				}
				return m_sortedReportItemList.Count;
			}
		}

		internal ReportItemList ComputedReportItems
		{
			get
			{
				Global.Tracer.Assert(!m_unpopulated);
				return m_computedReportItems;
			}
			set
			{
				m_computedReportItems = value;
			}
		}

		internal ReportItemList NonComputedReportItems
		{
			get
			{
				Global.Tracer.Assert(!m_unpopulated);
				return m_nonComputedReportItems;
			}
			set
			{
				m_nonComputedReportItems = value;
			}
		}

		internal ReportItemIndexerList SortedReportItems
		{
			get
			{
				Global.Tracer.Assert(!m_unpopulated);
				return m_sortedReportItemList;
			}
			set
			{
				m_sortedReportItemList = value;
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

		internal bool FirstInstance
		{
			get
			{
				return m_firstInstance;
			}
			set
			{
				m_firstInstance = value;
			}
		}

		internal string LinkToChild
		{
			set
			{
				m_linkToChildName = value;
			}
		}

		internal ReportItemCollection()
		{
		}

		internal ReportItemCollection(int id, bool normal)
			: base(id)
		{
			m_runningValues = new RunningValueInfoList();
			m_normal = normal;
			m_unpopulated = true;
			m_entries = new ReportItemList();
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

		internal void AddReportItem(ReportItem reportItem)
		{
			Global.Tracer.Assert(m_unpopulated);
			Global.Tracer.Assert(reportItem != null);
			Global.Tracer.Assert(m_entries != null);
			m_entries.Add(reportItem);
		}

		internal void AddCustomRenderItem(ReportItem reportItem)
		{
			Global.Tracer.Assert(reportItem != null);
			m_unpopulated = false;
			if (m_sortedReportItemList == null)
			{
				m_nonComputedReportItems = new ReportItemList();
				m_computedReportItems = new ReportItemList();
				m_sortedReportItemList = new ReportItemIndexerList();
			}
			ReportItemIndexer reportItemIndexer = default(ReportItemIndexer);
			if (reportItem.Computed)
			{
				reportItemIndexer.Index = m_computedReportItems.Add(reportItem);
			}
			else
			{
				reportItemIndexer.Index = m_nonComputedReportItems.Add(reportItem);
			}
			reportItemIndexer.IsComputed = reportItem.Computed;
			m_sortedReportItemList.Add(reportItemIndexer);
		}

		internal bool Initialize(InitializationContext context, bool registerRunningValues)
		{
			return Initialize(context, registerRunningValues, null);
		}

		internal bool Initialize(InitializationContext context, bool registerRunningValues, bool[] tableColumnVisiblity)
		{
			Global.Tracer.Assert(m_unpopulated);
			if (registerRunningValues)
			{
				context.RegisterRunningValues(m_runningValues);
			}
			if ((context.Location & LocationFlags.InPageSection) == 0)
			{
				context.RegisterPeerScopes(this);
			}
			Global.Tracer.Assert(m_entries != null);
			int count = m_entries.Count;
			bool flag = true;
			bool flag2 = false;
			SortedReportItemIndexList sortedReportItemIndexList = new SortedReportItemIndexList(count);
			bool result = true;
			bool tableColumnVisible = context.TableColumnVisible;
			for (int i = 0; i < count; i++)
			{
				ReportItem reportItem = m_entries[i];
				Global.Tracer.Assert(reportItem != null);
				if (tableColumnVisiblity != null && i < tableColumnVisiblity.Length && tableColumnVisible)
				{
					context.TableColumnVisible = tableColumnVisiblity[i];
				}
				if (!reportItem.Initialize(context))
				{
					result = false;
				}
				if (i == 0 && reportItem.Parent != null)
				{
					if ((context.Location & LocationFlags.InMatrixOrTable) != 0)
					{
						flag2 = true;
					}
					if (reportItem.Parent.HeightValue < reportItem.Parent.WidthValue)
					{
						flag = false;
					}
				}
				sortedReportItemIndexList.Add(m_entries, i, flag);
			}
			if (registerRunningValues)
			{
				context.UnRegisterRunningValues(m_runningValues);
			}
			if (count > 1 && !flag2)
			{
				RegisterOverlappingItems(context, count, sortedReportItemIndexList, flag);
			}
			return result;
		}

		private void RegisterOverlappingItems(InitializationContext context, int count, SortedReportItemIndexList sortedTop, bool isSortedVertically)
		{
			Hashtable hashtable = new Hashtable(count);
			for (int i = 0; i < count - 1; i++)
			{
				int num = sortedTop[i];
				double num2 = isSortedVertically ? m_entries[num].AbsoluteBottomValue : m_entries[num].AbsoluteRightValue;
				bool flag = true;
				for (int j = i + 1; j < count && flag; j++)
				{
					int num3 = sortedTop[j];
					Global.Tracer.Assert(num != num3, "(currentIndex != peerIndex)");
					double num4 = isSortedVertically ? m_entries[num3].AbsoluteTopValue : m_entries[num3].AbsoluteLeftValue;
					if (num2 > num4)
					{
						int num5 = Math.Min(num, num3);
						int num6 = Math.Max(num, num3);
						IntList intList = hashtable[num5] as IntList;
						if (intList == null)
						{
							intList = new IntList();
							hashtable[num5] = intList;
						}
						intList.Add(num6);
					}
					else
					{
						flag = false;
					}
				}
			}
			foreach (int key in hashtable.Keys)
			{
				IntList intList2 = hashtable[key] as IntList;
				double num8 = isSortedVertically ? m_entries[key].AbsoluteLeftValue : m_entries[key].AbsoluteTopValue;
				double num9 = isSortedVertically ? m_entries[key].AbsoluteRightValue : m_entries[key].AbsoluteBottomValue;
				for (int k = 0; k < intList2.Count; k++)
				{
					int index = intList2[k];
					double num10 = isSortedVertically ? m_entries[index].AbsoluteLeftValue : m_entries[index].AbsoluteTopValue;
					double num11 = isSortedVertically ? m_entries[index].AbsoluteRightValue : m_entries[index].AbsoluteBottomValue;
					if ((num10 > num8 && num10 < num9) || (num11 > num8 && num11 < num9) || (num10 <= num8 && num9 <= num11) || (num8 <= num10 && num11 <= num9))
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsOverlappingReportItems, Severity.Warning, m_entries[key].ObjectType, m_entries[key].Name, null, ErrorContext.GetLocalizedObjectTypeString(m_entries[index].ObjectType), m_entries[index].Name);
					}
				}
			}
		}

		internal void CalculateSizes(InitializationContext context, bool overwrite)
		{
			Global.Tracer.Assert(m_unpopulated);
			Global.Tracer.Assert(m_entries != null);
			for (int i = 0; i < m_entries.Count; i++)
			{
				ReportItem reportItem = m_entries[i];
				Global.Tracer.Assert(reportItem != null);
				reportItem.CalculateSizes(context, overwrite);
			}
		}

		internal void RegisterReceiver(InitializationContext context)
		{
			Global.Tracer.Assert(m_unpopulated);
			Global.Tracer.Assert(m_entries != null);
			for (int i = 0; i < m_entries.Count; i++)
			{
				ReportItem reportItem = m_entries[i];
				Global.Tracer.Assert(reportItem != null);
				reportItem.RegisterReceiver(context);
			}
		}

		internal void MarkChildrenComputed()
		{
			Global.Tracer.Assert(m_unpopulated);
			Global.Tracer.Assert(m_entries != null);
			for (int i = 0; i < m_entries.Count; i++)
			{
				ReportItem reportItem = m_entries[i];
				Global.Tracer.Assert(reportItem != null);
				if (reportItem is TextBox)
				{
					reportItem.Computed = true;
				}
			}
		}

		internal void Populate(ErrorContext errorContext)
		{
			Global.Tracer.Assert(m_unpopulated);
			Global.Tracer.Assert(m_entries != null);
			Hashtable hashtable = new Hashtable();
			int num = -1;
			if (0 < m_entries.Count)
			{
				if (m_normal)
				{
					m_entries.Sort();
				}
				m_nonComputedReportItems = new ReportItemList();
				m_computedReportItems = new ReportItemList();
				m_sortedReportItemList = new ReportItemIndexerList();
				for (int i = 0; i < m_entries.Count; i++)
				{
					ReportItem reportItem = m_entries[i];
					Global.Tracer.Assert(reportItem != null);
					if (reportItem is DataRegion)
					{
						hashtable[reportItem.Name] = reportItem;
					}
					ReportItemIndexer reportItemIndexer = default(ReportItemIndexer);
					if (reportItem.Computed)
					{
						reportItemIndexer.Index = m_computedReportItems.Add(reportItem);
					}
					else
					{
						reportItemIndexer.Index = m_nonComputedReportItems.Add(reportItem);
					}
					reportItemIndexer.IsComputed = reportItem.Computed;
					m_sortedReportItemList.Add(reportItemIndexer);
				}
			}
			m_unpopulated = false;
			m_entries = null;
			for (int j = 0; j < Count; j++)
			{
				ReportItem reportItem2 = this[j];
				Global.Tracer.Assert(reportItem2 != null);
				if (reportItem2.RepeatWith != null)
				{
					if (reportItem2 is DataRegion || reportItem2 is SubReport || (reportItem2 is Rectangle && ((Rectangle)reportItem2).ContainsDataRegionOrSubReport()))
					{
						errorContext.Register(ProcessingErrorCode.rsInvalidRepeatWith, Severity.Error, reportItem2.ObjectType, reportItem2.Name, "RepeatWith");
					}
					if (!m_normal || !hashtable.ContainsKey(reportItem2.RepeatWith))
					{
						errorContext.Register(ProcessingErrorCode.rsRepeatWithNotPeerDataRegion, Severity.Error, reportItem2.ObjectType, reportItem2.Name, "RepeatWith", reportItem2.RepeatWith);
					}
					DataRegion dataRegion = (DataRegion)hashtable[reportItem2.RepeatWith];
					if (dataRegion != null)
					{
						if (dataRegion.RepeatSiblings == null)
						{
							dataRegion.RepeatSiblings = new IntList();
						}
						dataRegion.RepeatSiblings.Add(j);
					}
				}
				if (m_linkToChildName != null && num < 0 && reportItem2.Name.Equals(m_linkToChildName, StringComparison.Ordinal))
				{
					num = j;
					((Rectangle)reportItem2.Parent).LinkToChild = j;
				}
			}
		}

		internal bool IsReportItemComputed(int index)
		{
			Global.Tracer.Assert(!m_unpopulated);
			Global.Tracer.Assert(0 <= index);
			return m_sortedReportItemList[index].IsComputed;
		}

		internal ReportItem GetUnsortedReportItem(int index, bool computed)
		{
			Global.Tracer.Assert(!m_unpopulated);
			Global.Tracer.Assert(0 <= index);
			return InternalGet(index, computed);
		}

		internal void GetReportItem(int index, out bool computed, out int internalIndex, out ReportItem reportItem)
		{
			Global.Tracer.Assert(!m_unpopulated);
			computed = false;
			internalIndex = -1;
			reportItem = null;
			if (m_sortedReportItemList != null && 0 <= index && index < m_sortedReportItemList.Count)
			{
				ReportItemIndexer reportItemIndexer = m_sortedReportItemList[index];
				if (0 <= reportItemIndexer.Index)
				{
					computed = reportItemIndexer.IsComputed;
					internalIndex = reportItemIndexer.Index;
					reportItem = InternalGet(internalIndex, computed);
				}
			}
		}

		private ReportItem InternalGet(int index, bool computed)
		{
			Global.Tracer.Assert(m_computedReportItems != null);
			Global.Tracer.Assert(m_nonComputedReportItems != null);
			if (computed)
			{
				return m_computedReportItems[index];
			}
			return m_nonComputedReportItems[index];
		}

		internal void ProcessDrillthroughAction(ReportProcessing.ProcessingContext processingContext, NonComputedUniqueNames[] nonCompNames)
		{
			if (nonCompNames != null && m_nonComputedReportItems != null && m_nonComputedReportItems.Count != 0)
			{
				NonComputedUniqueNames nonComputedUniqueNames = null;
				for (int i = 0; i < m_nonComputedReportItems.Count; i++)
				{
					nonComputedUniqueNames = nonCompNames[i];
					m_nonComputedReportItems[i].ProcessDrillthroughAction(processingContext, nonComputedUniqueNames);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.NonComputedReportItems, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemList));
			memberInfoList.Add(new MemberInfo(MemberName.ComputedReportItems, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemList));
			memberInfoList.Add(new MemberInfo(MemberName.SortedReportItems, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemIndexerList));
			memberInfoList.Add(new MemberInfo(MemberName.RunningValues, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.RunningValueInfoList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.IDOwner, memberInfoList);
		}
	}
}

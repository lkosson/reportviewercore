using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ReportItemCollection : IDOwner, IPersistable, IStaticReferenceable, IEnumerable<ReportItem>, IEnumerable
	{
		private List<ReportItem> m_nonComputedReportItems;

		private List<ReportItem> m_computedReportItems;

		private List<ReportItemIndexer> m_sortedReportItemList;

		private List<int> m_romIndexMap;

		[NonSerialized]
		private bool m_normal;

		[NonSerialized]
		private bool m_unpopulated;

		[NonSerialized]
		private List<ReportItem> m_entries;

		[NonSerialized]
		private string m_linkToChildName;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private bool m_firstInstance = true;

		[NonSerialized]
		private int m_staticRefId = int.MinValue;

		internal ReportItem this[int index]
		{
			get
			{
				if (m_unpopulated)
				{
					Global.Tracer.Assert(m_entries != null, "(null != m_entries)");
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
					Global.Tracer.Assert(m_entries != null, "(null != m_entries)");
					return m_entries.Count;
				}
				if (m_sortedReportItemList == null)
				{
					return 0;
				}
				return m_sortedReportItemList.Count;
			}
		}

		internal List<ReportItem> ComputedReportItems
		{
			get
			{
				Global.Tracer.Assert(!m_unpopulated, "(!m_unpopulated)");
				return m_computedReportItems;
			}
			set
			{
				m_computedReportItems = value;
			}
		}

		internal List<ReportItem> NonComputedReportItems
		{
			get
			{
				Global.Tracer.Assert(!m_unpopulated, "(!m_unpopulated)");
				return m_nonComputedReportItems;
			}
			set
			{
				m_nonComputedReportItems = value;
			}
		}

		internal List<ReportItemIndexer> SortedReportItems
		{
			get
			{
				Global.Tracer.Assert(!m_unpopulated, "(!m_unpopulated)");
				return m_sortedReportItemList;
			}
			set
			{
				m_sortedReportItemList = value;
			}
		}

		internal List<int> ROMIndexMap => m_romIndexMap;

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

		int IStaticReferenceable.ID => m_staticRefId;

		internal ReportItemCollection()
		{
		}

		internal ReportItemCollection(int id, bool normal)
			: base(id)
		{
			m_normal = normal;
			m_unpopulated = true;
			m_entries = new List<ReportItem>();
		}

		public IEnumerator<ReportItem> GetEnumerator()
		{
			for (int i = 0; i < Count; i++)
			{
				yield return this[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		internal void AddReportItem(ReportItem reportItem)
		{
			Global.Tracer.Assert(m_unpopulated, "(m_unpopulated)");
			Global.Tracer.Assert(reportItem != null, "(null != reportItem)");
			Global.Tracer.Assert(m_entries != null, "(null != m_entries)");
			m_entries.Add(reportItem);
		}

		internal void AddCustomRenderItem(ReportItem reportItem)
		{
			Global.Tracer.Assert(reportItem != null, "(null != reportItem)");
			m_unpopulated = false;
			if (m_sortedReportItemList == null)
			{
				m_nonComputedReportItems = new List<ReportItem>();
				m_computedReportItems = new List<ReportItem>();
				m_sortedReportItemList = new List<ReportItemIndexer>();
			}
			ReportItemIndexer item = default(ReportItemIndexer);
			if (reportItem.Computed)
			{
				m_computedReportItems.Add(reportItem);
				item.Index = m_computedReportItems.Count - 1;
			}
			else
			{
				m_nonComputedReportItems.Add(reportItem);
				item.Index = m_nonComputedReportItems.Count - 1;
			}
			item.IsComputed = reportItem.Computed;
			m_sortedReportItemList.Add(item);
		}

		internal bool Initialize(InitializationContext context)
		{
			Global.Tracer.Assert(m_unpopulated, "(m_unpopulated)");
			if ((context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) == 0)
			{
				context.RegisterPeerScopes(this);
			}
			Global.Tracer.Assert(m_entries != null, "(null != m_entries)");
			int count = m_entries.Count;
			bool flag = true;
			bool flag2 = false;
			SortedReportItemIndexList sortedReportItemIndexList = new SortedReportItemIndexList(count);
			bool result = true;
			for (int i = 0; i < count; i++)
			{
				ReportItem reportItem = m_entries[i];
				Global.Tracer.Assert(reportItem != null, "(null != item)");
				if (!reportItem.Initialize(context))
				{
					result = false;
				}
				if (i == 0 && reportItem.Parent != null)
				{
					if ((context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InTablix) != 0)
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
			if (count > 1 && !flag2 && !context.PublishingContext.IsRdlx)
			{
				RegisterOverlappingItems(context, count, sortedReportItemIndexList, flag);
			}
			return result;
		}

		internal void InitializeRVDirectionDependentItems(InitializationContext context)
		{
			for (int i = 0; i < m_entries.Count; i++)
			{
				m_entries[i].InitializeRVDirectionDependentItems(context);
			}
		}

		internal void DetermineGroupingExprValueCount(InitializationContext context, int groupingExprCount)
		{
			for (int i = 0; i < m_entries.Count; i++)
			{
				m_entries[i].DetermineGroupingExprValueCount(context, groupingExprCount);
			}
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
						int item = Math.Max(num, num3);
						List<int> list = hashtable[num5] as List<int>;
						if (list == null)
						{
							list = new List<int>();
							hashtable[num5] = list;
						}
						list.Add(item);
					}
					else
					{
						flag = false;
					}
				}
			}
			foreach (int key in hashtable.Keys)
			{
				List<int> list2 = hashtable[key] as List<int>;
				double num7 = isSortedVertically ? m_entries[key].AbsoluteLeftValue : m_entries[key].AbsoluteTopValue;
				double num8 = isSortedVertically ? m_entries[key].AbsoluteRightValue : m_entries[key].AbsoluteBottomValue;
				for (int k = 0; k < list2.Count; k++)
				{
					int index = list2[k];
					double num9 = isSortedVertically ? m_entries[index].AbsoluteLeftValue : m_entries[index].AbsoluteTopValue;
					double num10 = isSortedVertically ? m_entries[index].AbsoluteRightValue : m_entries[index].AbsoluteBottomValue;
					if (((num9 > num7 && num9 < num8) || (num10 > num7 && num10 < num8) || (num9 <= num7 && num8 <= num10) || (num7 <= num9 && num10 <= num8)) && (m_entries[key].ObjectType != Microsoft.ReportingServices.ReportProcessing.ObjectType.CustomReportItem || ((CustomReportItem)m_entries[key]).AltReportItem != m_entries[index]) && (m_entries[index].ObjectType != Microsoft.ReportingServices.ReportProcessing.ObjectType.CustomReportItem || ((CustomReportItem)m_entries[index]).AltReportItem != m_entries[key]))
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsOverlappingReportItems, Severity.Warning, m_entries[key].ObjectType, m_entries[key].Name, null, ErrorContext.GetLocalizedObjectTypeString(m_entries[index].ObjectType), m_entries[index].Name);
					}
				}
			}
		}

		internal void CalculateSizes(InitializationContext context, bool overwrite)
		{
			Global.Tracer.Assert(m_unpopulated, "(m_unpopulated)");
			Global.Tracer.Assert(m_entries != null, "(null != m_entries)");
			for (int i = 0; i < m_entries.Count; i++)
			{
				ReportItem reportItem = m_entries[i];
				Global.Tracer.Assert(reportItem != null, "(null != item)");
				reportItem.CalculateSizes(context, overwrite);
			}
		}

		internal void MarkChildrenComputed()
		{
			Global.Tracer.Assert(m_unpopulated, "(m_unpopulated)");
			Global.Tracer.Assert(m_entries != null, "(null != m_entries)");
			for (int i = 0; i < m_entries.Count; i++)
			{
				ReportItem reportItem = m_entries[i];
				Global.Tracer.Assert(reportItem != null, "(null != item)");
				if (reportItem is TextBox)
				{
					reportItem.Computed = true;
				}
			}
		}

		internal void Populate(ErrorContext errorContext)
		{
			Global.Tracer.Assert(m_unpopulated, "(m_unpopulated)");
			Global.Tracer.Assert(m_entries != null, "(null != m_entries)");
			Hashtable hashtable = new Hashtable();
			int num = -1;
			if (0 < m_entries.Count)
			{
				if (m_normal)
				{
					m_entries.Sort();
				}
				m_nonComputedReportItems = new List<ReportItem>();
				m_computedReportItems = new List<ReportItem>();
				m_sortedReportItemList = new List<ReportItemIndexer>();
				List<CustomReportItem> list = new List<CustomReportItem>();
				for (int i = 0; i < m_entries.Count; i++)
				{
					ReportItem reportItem = m_entries[i];
					Global.Tracer.Assert(reportItem != null, "(null != item)");
					if (reportItem.IsDataRegion)
					{
						hashtable[reportItem.Name] = reportItem;
					}
					if (reportItem.ObjectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.CustomReportItem)
					{
						list.Add((CustomReportItem)reportItem);
					}
					ReportItemIndexer item = default(ReportItemIndexer);
					if (reportItem.Computed)
					{
						m_computedReportItems.Add(reportItem);
						item.Index = m_computedReportItems.Count - 1;
					}
					else
					{
						m_nonComputedReportItems.Add(reportItem);
						item.Index = m_nonComputedReportItems.Count - 1;
					}
					item.IsComputed = reportItem.Computed;
					m_sortedReportItemList.Add(item);
				}
				if (list.Count > 0)
				{
					bool[] array = new bool[m_sortedReportItemList.Count];
					foreach (CustomReportItem item2 in list)
					{
						int num3 = item2.AltReportItemIndexInParentCollectionDef = m_entries.IndexOf(item2.AltReportItem);
						array[num3] = true;
					}
					m_romIndexMap = new List<int>(m_sortedReportItemList.Count - list.Count);
					for (int j = 0; j < m_sortedReportItemList.Count; j++)
					{
						if (!array[j])
						{
							m_romIndexMap.Add(j);
						}
					}
					Global.Tracer.Assert(m_romIndexMap.Count + list.Count == m_sortedReportItemList.Count);
				}
			}
			m_unpopulated = false;
			m_entries = null;
			for (int k = 0; k < Count; k++)
			{
				ReportItem reportItem2 = this[k];
				Global.Tracer.Assert(reportItem2 != null, "(null != item)");
				if (reportItem2.RepeatWith != null)
				{
					if (reportItem2.IsDataRegion || reportItem2 is SubReport || (reportItem2 is Rectangle && ((Rectangle)reportItem2).ContainsDataRegionOrSubReport()))
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
							dataRegion.RepeatSiblings = new List<int>();
						}
						dataRegion.RepeatSiblings.Add((m_romIndexMap == null) ? k : m_romIndexMap.IndexOf(k));
					}
				}
				if (m_linkToChildName != null && num < 0 && reportItem2.Name.Equals(m_linkToChildName, StringComparison.Ordinal))
				{
					num = k;
					((Rectangle)reportItem2.Parent).LinkToChild = k;
				}
			}
		}

		internal bool IsReportItemComputed(int index)
		{
			Global.Tracer.Assert(!m_unpopulated, "(!m_unpopulated)");
			Global.Tracer.Assert(0 <= index, "(0 <= index)");
			return m_sortedReportItemList[index].IsComputed;
		}

		internal ReportItem GetUnsortedReportItem(int index, bool computed)
		{
			Global.Tracer.Assert(!m_unpopulated, "(!m_unpopulated)");
			Global.Tracer.Assert(0 <= index, "(0 <= index)");
			return InternalGet(index, computed);
		}

		internal void GetReportItem(int index, out bool computed, out int internalIndex, out ReportItem reportItem)
		{
			Global.Tracer.Assert(!m_unpopulated, "(!m_unpopulated)");
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
			Global.Tracer.Assert(m_computedReportItems != null, "(null != m_computedReportItems)");
			Global.Tracer.Assert(m_nonComputedReportItems != null, "(null != m_nonComputedReportItems)");
			if (computed)
			{
				return m_computedReportItems[index];
			}
			return m_nonComputedReportItems[index];
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ReportItemCollection reportItemCollection = (ReportItemCollection)base.PublishClone(context);
			context.AddReportItemCollection(reportItemCollection);
			if (m_entries != null)
			{
				CustomReportItem customReportItem = null;
				reportItemCollection.m_entries = new List<ReportItem>();
				foreach (ReportItem entry in m_entries)
				{
					ReportItem reportItem = (ReportItem)entry.PublishClone(context);
					reportItemCollection.m_entries.Add(reportItem);
					if (reportItem is CustomReportItem)
					{
						Global.Tracer.Assert(customReportItem == null, "(lastCriPublishClone == null)");
						customReportItem = (CustomReportItem)reportItem;
					}
					else if (customReportItem != null)
					{
						customReportItem.AltReportItem = reportItem;
						customReportItem = null;
					}
				}
				Global.Tracer.Assert(customReportItem == null, "(lastCriPublishClone == null)");
			}
			if (m_linkToChildName != null)
			{
				reportItemCollection.m_linkToChildName = context.GetNewReportItemName(m_linkToChildName);
			}
			return reportItemCollection;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.NonComputedReportItems, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem));
			list.Add(new MemberInfo(MemberName.ComputedReportItems, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem));
			list.Add(new MemberInfo(MemberName.SortedReportItems, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemIndexer));
			list.Add(new MemberInfo(MemberName.ROMIndexMap, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemCollection, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.NonComputedReportItems:
					writer.Write(m_nonComputedReportItems);
					break;
				case MemberName.ComputedReportItems:
					writer.Write(m_computedReportItems);
					break;
				case MemberName.SortedReportItems:
					writer.Write(m_sortedReportItemList);
					break;
				case MemberName.ROMIndexMap:
					writer.WriteListOfPrimitives(m_romIndexMap);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.NonComputedReportItems:
					m_nonComputedReportItems = reader.ReadGenericListOfRIFObjects<ReportItem>();
					break;
				case MemberName.ComputedReportItems:
					m_computedReportItems = reader.ReadGenericListOfRIFObjects<ReportItem>();
					break;
				case MemberName.SortedReportItems:
					m_sortedReportItemList = reader.ReadGenericListOfRIFObjects<ReportItemIndexer>();
					break;
				case MemberName.ROMIndexMap:
					m_romIndexMap = reader.ReadListOfPrimitives<int>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemCollection;
		}

		void IStaticReferenceable.SetID(int id)
		{
			m_staticRefId = id;
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IStaticReferenceable.GetObjectType()
		{
			return GetObjectType();
		}
	}
}

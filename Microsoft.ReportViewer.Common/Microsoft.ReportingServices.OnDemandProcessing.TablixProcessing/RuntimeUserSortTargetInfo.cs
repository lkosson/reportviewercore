using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeUserSortTargetInfo : IStorable, IPersistable
	{
		private BTree m_sortTree;

		private List<AggregateRow> m_aggregateRows;

		private List<int> m_sortFilterInfoIndices;

		private Hashtable m_targetForNonDetailSort;

		private Hashtable m_targetForDetailSort;

		private static Declaration m_declaration = GetDeclaration();

		internal BTree SortTree
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

		internal List<AggregateRow> AggregateRows
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

		internal List<int> SortFilterInfoIndices
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

		public int Size => ItemSizes.SizeOf(m_sortTree) + ItemSizes.SizeOf(m_aggregateRows) + ItemSizes.SizeOf(m_sortFilterInfoIndices) + ItemSizes.SizeOf(m_targetForNonDetailSort) + ItemSizes.SizeOf(m_targetForDetailSort);

		internal RuntimeUserSortTargetInfo()
		{
		}

		internal RuntimeUserSortTargetInfo(IReference<IHierarchyObj> owner, int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
		{
			AddSortInfo(owner, sortInfoIndex, sortInfo);
		}

		internal void AddSortInfo(IReference<IHierarchyObj> owner, int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
		{
			IInScopeEventSource eventSource = sortInfo.Value().EventSource;
			if (eventSource.UserSort.SortExpressionScope != null || owner.Value().IsDetail)
			{
				if (eventSource.UserSort.SortExpressionScope == null)
				{
					AddSortInfoIndex(sortInfoIndex, sortInfo);
				}
				if (m_sortTree == null)
				{
					IHierarchyObj hierarchyObj = owner.Value();
					m_sortTree = new BTree(hierarchyObj, hierarchyObj.OdpContext, hierarchyObj.Depth + 1);
				}
			}
			if (eventSource.UserSort.SortExpressionScope != null)
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

		internal void AddSortInfoIndex(int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfoRef)
		{
			using (sortInfoRef.PinValue())
			{
				RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = sortInfoRef.Value();
				Global.Tracer.Assert(runtimeSortFilterEventInfo.EventSource.UserSort.SortExpressionScope == null || !runtimeSortFilterEventInfo.TargetSortFilterInfoAdded);
				if (m_sortFilterInfoIndices == null)
				{
					m_sortFilterInfoIndices = new List<int>();
				}
				m_sortFilterInfoIndices.Add(sortInfoIndex);
				runtimeSortFilterEventInfo.TargetSortFilterInfoAdded = true;
			}
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

		internal void MarkSortInfoProcessed(List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo, IReference<IHierarchyObj> sortTarget)
		{
			if (m_targetForNonDetailSort != null)
			{
				MarkSortInfoProcessed(runtimeSortFilterInfo, sortTarget, m_targetForNonDetailSort.Keys);
			}
			if (m_targetForDetailSort != null)
			{
				MarkSortInfoProcessed(runtimeSortFilterInfo, sortTarget, m_targetForDetailSort.Keys);
			}
		}

		private void MarkSortInfoProcessed(List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo, IReference<IHierarchyObj> sortTarget, ICollection indices)
		{
			foreach (int index in indices)
			{
				IReference<RuntimeSortFilterEventInfo> reference = runtimeSortFilterInfo[index];
				using (reference.PinValue())
				{
					RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
					if (runtimeSortFilterEventInfo.EventTarget.Equals(sortTarget))
					{
						Global.Tracer.Assert(!runtimeSortFilterEventInfo.Processed, "(!runtimeSortInfo.Processed)");
						runtimeSortFilterEventInfo.Processed = true;
					}
				}
			}
		}

		internal void EnterProcessUserSortPhase(OnDemandProcessingContext odpContext)
		{
			if (m_sortFilterInfoIndices != null)
			{
				int count = m_sortFilterInfoIndices.Count;
				for (int i = 0; i < count; i++)
				{
					odpContext.UserSortFilterContext.EnterProcessUserSortPhase(m_sortFilterInfoIndices[i]);
				}
			}
		}

		internal void LeaveProcessUserSortPhase(OnDemandProcessingContext odpContext)
		{
			if (m_sortFilterInfoIndices != null)
			{
				int count = m_sortFilterInfoIndices.Count;
				for (int i = 0; i < count; i++)
				{
					odpContext.UserSortFilterContext.LeaveProcessUserSortPhase(m_sortFilterInfoIndices[i]);
				}
			}
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.SortTree:
					writer.Write(m_sortTree);
					break;
				case MemberName.AggregateRows:
					writer.Write(m_aggregateRows);
					break;
				case MemberName.SortFilterInfoIndices:
					writer.WriteListOfPrimitives(m_sortFilterInfoIndices);
					break;
				case MemberName.TargetForNonDetailSort:
					writer.WriteVariantVariantHashtable(m_targetForNonDetailSort);
					break;
				case MemberName.TargetForDetailSort:
					writer.WriteVariantVariantHashtable(m_targetForDetailSort);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.SortTree:
					m_sortTree = (BTree)reader.ReadRIFObject();
					break;
				case MemberName.AggregateRows:
					m_aggregateRows = reader.ReadListOfRIFObjects<List<AggregateRow>>();
					break;
				case MemberName.SortFilterInfoIndices:
					m_sortFilterInfoIndices = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.TargetForNonDetailSort:
					m_targetForNonDetailSort = reader.ReadVariantVariantHashtable();
					break;
				case MemberName.TargetForDetailSort:
					m_targetForDetailSort = reader.ReadVariantVariantHashtable();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeUserSortTargetInfo;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.SortTree, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTree));
				list.Add(new MemberInfo(MemberName.AggregateRows, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AggregateRow));
				list.Add(new MemberInfo(MemberName.SortFilterInfoIndices, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
				list.Add(new MemberInfo(MemberName.TargetForNonDetailSort, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VariantVariantHashtable));
				list.Add(new MemberInfo(MemberName.TargetForDetailSort, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VariantVariantHashtable));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeUserSortTargetInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}

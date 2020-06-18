using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class TreePartitionManager : IPersistable
	{
		private List<long> m_treePartitionOffsets;

		[NonSerialized]
		private bool m_treeChanged;

		[NonSerialized]
		internal static readonly long EmptyTreePartitionOffset = -1L;

		[NonSerialized]
		internal static readonly ReferenceID EmptyTreePartitionID = new ReferenceID(hasMultiPart: true, isTemporary: false, -1);

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal bool TreeHasChanged
		{
			get
			{
				return m_treeChanged;
			}
			set
			{
				m_treeChanged = value;
			}
		}

		internal TreePartitionManager()
		{
		}

		internal TreePartitionManager(List<long> partitionTable)
		{
			m_treePartitionOffsets = partitionTable;
		}

		internal ReferenceID AllocateNewTreePartition()
		{
			m_treeChanged = true;
			if (m_treePartitionOffsets == null)
			{
				m_treePartitionOffsets = new List<long>();
			}
			int count = m_treePartitionOffsets.Count;
			m_treePartitionOffsets.Add(EmptyTreePartitionOffset);
			ReferenceID result = default(ReferenceID);
			result.HasMultiPart = true;
			result.IsTemporary = false;
			result.PartitionID = count;
			return result;
		}

		internal void UpdateTreePartitionOffset(ReferenceID id, long offset)
		{
			int partitionIndex = GetPartitionIndex(id);
			Global.Tracer.Assert(offset >= 0, "Invalid offset for Tree partition. ID: {0} Offset: {1}", id, offset);
			Global.Tracer.Assert(m_treePartitionOffsets[partitionIndex] == EmptyTreePartitionOffset, "Cannot update offset for already persisted tree partition");
			m_treeChanged = true;
			m_treePartitionOffsets[partitionIndex] = offset;
		}

		internal long GetTreePartitionOffset(ReferenceID id)
		{
			int partitionIndex = GetPartitionIndex(id);
			return m_treePartitionOffsets[partitionIndex];
		}

		private int GetPartitionIndex(ReferenceID id)
		{
			int partitionID = id.PartitionID;
			Global.Tracer.Assert(partitionID >= 0, "Invalid tree partition id: {0}", partitionID);
			Global.Tracer.Assert(m_treePartitionOffsets != null && partitionID < m_treePartitionOffsets.Count, "Cannot update Tree partition: {0} without first allocating it", partitionID);
			return partitionID;
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.TreePartitionOffsets)
				{
					writer.WriteListOfPrimitives(m_treePartitionOffsets);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.TreePartitionOffsets)
				{
					m_treePartitionOffsets = reader.ReadListOfPrimitives<long>();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TreePartitionManager;
		}

		public static Declaration GetDeclaration()
		{
			if (m_Declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.TreePartitionOffsets, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int64));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TreePartitionManager, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_Declaration;
		}
	}
}

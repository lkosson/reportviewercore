using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class BTreeNodeTupleList : IStorable, IPersistable
	{
		private List<BTreeNodeTuple> m_list;

		private int m_capacity;

		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		internal BTreeNodeTuple this[int index] => m_list[index];

		internal int Count => m_list.Count;

		public int Size => ItemSizes.SizeOf(m_list) + 4;

		internal BTreeNodeTupleList()
		{
		}

		internal BTreeNodeTupleList(int capacity)
		{
			m_list = new List<BTreeNodeTuple>(capacity);
			m_capacity = capacity;
		}

		internal void Add(BTreeNodeTuple tuple, ScalableList<BTreeNode> nodes)
		{
			if (m_list.Count == m_capacity)
			{
				throw new InvalidOperationException();
			}
			m_list.Add(tuple);
			if (-1 != tuple.ChildIndex)
			{
				BTreeNode item;
				using (nodes.GetAndPin(tuple.ChildIndex, out item))
				{
					item.IndexInParent = m_list.Count - 1;
				}
			}
		}

		internal void Insert(int index, BTreeNodeTuple tuple, ScalableList<BTreeNode> nodes)
		{
			if (m_list.Count == m_capacity)
			{
				throw new InvalidOperationException();
			}
			m_list.Insert(index, tuple);
			for (int i = index; i < m_list.Count; i++)
			{
				int childIndex = m_list[i].ChildIndex;
				if (childIndex != -1)
				{
					BTreeNode item;
					using (nodes.GetAndPin(childIndex, out item))
					{
						item.IndexInParent = i;
					}
				}
			}
		}

		internal void RemoveAtEnd()
		{
			m_list.RemoveAt(m_list.Count - 1);
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.List:
					writer.Write(m_list);
					break;
				case MemberName.Capacity:
					writer.Write(m_capacity);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.List:
					m_list = reader.ReadListOfRIFObjects<List<BTreeNodeTuple>>();
					break;
				case MemberName.Capacity:
					m_capacity = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNodeTupleList;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.List, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNodeTuple));
				list.Add(new MemberInfo(MemberName.Capacity, Token.Int32));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNodeTupleList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}

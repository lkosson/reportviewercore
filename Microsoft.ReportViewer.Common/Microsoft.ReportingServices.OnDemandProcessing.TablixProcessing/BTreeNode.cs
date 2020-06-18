using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class BTreeNode : IStorable, IPersistable
	{
		private const int BTreeOrder = 3;

		private BTreeNodeTupleList m_tuples;

		private int m_indexInParent;

		private static Declaration m_declaration = GetDeclaration();

		internal int IndexInParent
		{
			set
			{
				m_indexInParent = value;
			}
		}

		internal BTreeNodeTupleList Tuples => m_tuples;

		public int Size => ItemSizes.SizeOf(m_tuples) + 4;

		internal BTreeNode()
		{
		}

		internal BTreeNode(IHierarchyObj owner)
		{
			m_tuples = new BTreeNodeTupleList(3);
			BTreeNodeTuple tuple = new BTreeNodeTuple(CreateBTreeNode(null, owner), -1);
			m_tuples.Add(tuple, null);
		}

		internal void Traverse(ProcessingStages operation, bool ascending, ScalableList<BTreeNode> nodes, ITraversalContext traversalContext)
		{
			if (ascending)
			{
				for (int i = 0; i < m_tuples.Count; i++)
				{
					m_tuples[i].Traverse(operation, ascending, nodes, traversalContext);
				}
				return;
			}
			for (int num = m_tuples.Count - 1; num >= 0; num--)
			{
				m_tuples[num].Traverse(operation, ascending, nodes, traversalContext);
			}
		}

		internal void SetFirstChild(ScalableList<BTreeNode> nodes, int childIndex)
		{
			Global.Tracer.Assert(1 <= m_tuples.Count, "(1 <= m_tuples.Count)");
			m_tuples[0].ChildIndex = childIndex;
			if (childIndex != -1)
			{
				BTreeNode item;
				using (nodes.GetAndPin(childIndex, out item))
				{
					item.IndexInParent = 0;
				}
			}
		}

		private BTreeNodeValue CreateBTreeNode(object key, IHierarchyObj owner)
		{
			return new BTreeNodeHierarchyObj(key, owner);
		}

		internal bool SearchAndInsert(object keyValue, ScalableList<BTreeNode> nodes, IHierarchyObj owner, out BTreeNodeValue newSiblingValue, out int newSiblingIndex, out int globalNewSiblingIndex)
		{
			int num = -1;
			int i;
			for (i = 1; i < m_tuples.Count; i++)
			{
				num = m_tuples[i].Value.CompareTo(keyValue, owner.OdpContext);
				if (num >= 0)
				{
					break;
				}
			}
			if (num == 0)
			{
				m_tuples[i].Value.AddRow(owner);
			}
			else
			{
				int childIndex = m_tuples[i - 1].ChildIndex;
				if (childIndex == -1)
				{
					return InsertBTreeNode(nodes, CreateBTreeNode(keyValue, owner), i, -1, owner, out newSiblingValue, out newSiblingIndex, out globalNewSiblingIndex);
				}
				BTreeNode item;
				using (nodes.GetAndPin(childIndex, out item))
				{
					if (!item.SearchAndInsert(keyValue, nodes, owner, out BTreeNodeValue newSiblingValue2, out int newSiblingIndex2, out int globalNewSiblingIndex2))
					{
						return InsertBTreeNode(nodes, newSiblingValue2, newSiblingIndex2, globalNewSiblingIndex2, owner, out newSiblingValue, out newSiblingIndex, out globalNewSiblingIndex);
					}
				}
			}
			newSiblingValue = null;
			newSiblingIndex = -1;
			globalNewSiblingIndex = -1;
			return true;
		}

		private bool InsertBTreeNode(ScalableList<BTreeNode> nodes, BTreeNodeValue nodeValueToInsert, int nodeIndexToInsert, int globalNodeIndexToInsert, IHierarchyObj owner, out BTreeNodeValue newSiblingValue, out int newSiblingIndex, out int globalNewSibingIndex)
		{
			if (3 > m_tuples.Count)
			{
				m_tuples.Insert(nodeIndexToInsert, new BTreeNodeTuple(nodeValueToInsert, globalNodeIndexToInsert), nodes);
				newSiblingValue = null;
				newSiblingIndex = -1;
				globalNewSibingIndex = -1;
				return true;
			}
			int num = 2;
			BTreeNode bTreeNode = new BTreeNode(owner);
			BTreeNodeValue bTreeNodeValue;
			if (num < nodeIndexToInsert)
			{
				bTreeNodeValue = m_tuples[num].Value;
				bTreeNode.SetFirstChild(nodes, m_tuples[num].ChildIndex);
				for (int i = num + 1; i < ((m_tuples.Count <= nodeIndexToInsert) ? m_tuples.Count : nodeIndexToInsert); i++)
				{
					bTreeNode.m_tuples.Add(m_tuples[i], nodes);
				}
				bTreeNode.m_tuples.Add(new BTreeNodeTuple(nodeValueToInsert, globalNodeIndexToInsert), nodes);
				for (int j = nodeIndexToInsert; j < m_tuples.Count; j++)
				{
					bTreeNode.m_tuples.Add(m_tuples[j], nodes);
				}
				int count = m_tuples.Count;
				for (int k = num; k < count; k++)
				{
					m_tuples.RemoveAtEnd();
				}
			}
			else if (num > nodeIndexToInsert)
			{
				bTreeNodeValue = m_tuples[num - 1].Value;
				bTreeNode.SetFirstChild(nodes, m_tuples[num - 1].ChildIndex);
				for (int l = num; l < m_tuples.Count; l++)
				{
					bTreeNode.m_tuples.Add(m_tuples[l], nodes);
				}
				int count2 = m_tuples.Count;
				for (int m = num - 1; m < count2; m++)
				{
					m_tuples.RemoveAtEnd();
				}
				m_tuples.Insert(nodeIndexToInsert, new BTreeNodeTuple(nodeValueToInsert, globalNodeIndexToInsert), nodes);
			}
			else
			{
				bTreeNodeValue = nodeValueToInsert;
				bTreeNode.SetFirstChild(nodes, globalNodeIndexToInsert);
				for (int n = num; n < m_tuples.Count; n++)
				{
					bTreeNode.m_tuples.Add(m_tuples[n], nodes);
				}
				int count3 = m_tuples.Count;
				for (int num2 = num; num2 < count3; num2++)
				{
					m_tuples.RemoveAtEnd();
				}
			}
			newSiblingValue = bTreeNodeValue;
			newSiblingIndex = m_indexInParent + 1;
			globalNewSibingIndex = nodes.Count;
			nodes.Add(bTreeNode);
			return false;
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Tuples:
					writer.Write(m_tuples);
					break;
				case MemberName.IndexInParent:
					writer.Write(m_indexInParent);
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
				case MemberName.Tuples:
					m_tuples = (BTreeNodeTupleList)reader.ReadRIFObject();
					break;
				case MemberName.IndexInParent:
					m_indexInParent = reader.ReadInt32();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNode;
		}

		internal static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Tuples, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNodeTupleList));
				list.Add(new MemberInfo(MemberName.IndexInParent, Token.Int32));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNode, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}

using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class BTreeNodeTuple : IStorable, IPersistable
	{
		private BTreeNodeValue m_value;

		private int m_childIndex = -1;

		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		internal BTreeNodeValue Value => m_value;

		internal int ChildIndex
		{
			get
			{
				return m_childIndex;
			}
			set
			{
				m_childIndex = value;
			}
		}

		public int Size => ItemSizes.SizeOf(m_value) + 4;

		internal BTreeNodeTuple()
		{
		}

		internal BTreeNodeTuple(BTreeNodeValue value, int childIndex)
		{
			m_value = value;
			m_childIndex = childIndex;
		}

		internal void Traverse(ProcessingStages operation, bool ascending, ScalableList<BTreeNode> nodeList, ITraversalContext traversalContext)
		{
			if (ascending)
			{
				if (m_value != null)
				{
					m_value.Traverse(operation, traversalContext);
				}
				VisitChild(operation, ascending, nodeList, traversalContext);
			}
			else
			{
				VisitChild(operation, ascending, nodeList, traversalContext);
				if (m_value != null)
				{
					m_value.Traverse(operation, traversalContext);
				}
			}
		}

		internal void VisitChild(ProcessingStages operation, bool ascending, ScalableList<BTreeNode> nodeList, ITraversalContext traversalContext)
		{
			if (-1 != m_childIndex)
			{
				BTreeNode item;
				using (nodeList.GetAndPin(m_childIndex, out item))
				{
					item.Traverse(operation, ascending, nodeList, traversalContext);
				}
			}
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Value:
					writer.Write(m_value);
					break;
				case MemberName.Child:
					writer.Write(m_childIndex);
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
				case MemberName.Value:
					m_value = (BTreeNodeValue)reader.ReadRIFObject();
					break;
				case MemberName.Child:
					m_childIndex = reader.ReadInt32();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNodeTuple;
		}

		internal static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNodeValue));
				list.Add(new MemberInfo(MemberName.Child, Token.Int32));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNodeTuple, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}

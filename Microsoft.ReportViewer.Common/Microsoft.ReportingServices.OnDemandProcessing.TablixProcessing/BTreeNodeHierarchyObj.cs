using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class BTreeNodeHierarchyObj : BTreeNodeValue
	{
		private object m_key;

		private IHierarchyObj m_hierarchyNode;

		private static Declaration m_declaration = GetDeclaration();

		protected override object Key => m_key;

		public override int Size => ItemSizes.SizeOf(m_key) + ItemSizes.SizeOf(m_hierarchyNode);

		internal BTreeNodeHierarchyObj()
		{
		}

		internal BTreeNodeHierarchyObj(object key, IHierarchyObj owner)
		{
			m_key = key;
			if (key != null)
			{
				m_hierarchyNode = owner.CreateHierarchyObjForSortTree();
				m_hierarchyNode.NextRow(owner);
			}
		}

		internal override void AddRow(IHierarchyObj owner)
		{
			m_hierarchyNode.NextRow(owner);
		}

		internal override void Traverse(ProcessingStages operation, ITraversalContext traversalContext)
		{
			if (m_hierarchyNode != null)
			{
				m_hierarchyNode.Traverse(operation, traversalContext);
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Key:
					writer.Write(m_key);
					break;
				case MemberName.HierarchyNode:
					writer.Write(m_hierarchyNode);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Key:
					m_key = reader.ReadVariant();
					break;
				case MemberName.HierarchyNode:
					m_hierarchyNode = (IHierarchyObj)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNodeHierarchyObj;
		}

		internal static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Key, Token.Object));
				list.Add(new MemberInfo(MemberName.HierarchyNode, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IHierarchyObj));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNodeHierarchyObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNodeValue, list);
			}
			return m_declaration;
		}
	}
}

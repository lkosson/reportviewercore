using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeGroupingObj : IStorable, IPersistable
	{
		internal enum GroupingTypes
		{
			None,
			Hash,
			Sort,
			Detail,
			DetailUserSort,
			NaturalGroup
		}

		[NonSerialized]
		protected RuntimeHierarchyObj m_owner;

		protected Microsoft.ReportingServices.ReportProcessing.ObjectType m_objectType;

		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		internal virtual BTree Tree
		{
			get
			{
				Global.Tracer.Assert(condition: false, "Tree is only available for sort based groupings.");
				return null;
			}
		}

		public virtual int Size => ItemSizes.ReferenceSize + 4;

		internal RuntimeGroupingObj()
		{
		}

		internal RuntimeGroupingObj(RuntimeHierarchyObj owner, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType)
		{
			m_owner = owner;
			m_objectType = objectType;
		}

		internal static RuntimeGroupingObj CreateGroupingObj(GroupingTypes type, RuntimeHierarchyObj owner, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType)
		{
			switch (type)
			{
			case GroupingTypes.None:
				return new RuntimeGroupingObjLinkedList(owner, objectType);
			case GroupingTypes.Hash:
				return new RuntimeGroupingObjHash(owner, objectType);
			case GroupingTypes.Sort:
				return new RuntimeGroupingObjTree(owner, objectType);
			case GroupingTypes.Detail:
				return new RuntimeGroupingObjDetail(owner, objectType);
			case GroupingTypes.DetailUserSort:
				return new RuntimeGroupingObjDetailUserSort(owner, objectType);
			case GroupingTypes.NaturalGroup:
				return new RuntimeGroupingObjNaturalGroup(owner, objectType);
			default:
				Global.Tracer.Assert(condition: false, "Unexpected GroupingTypes");
				throw new InvalidOperationException();
			}
		}

		internal abstract void Cleanup();

		internal void NextRow(object keyValue)
		{
			NextRow(keyValue, hasParent: false, null);
		}

		internal abstract void NextRow(object keyValue, bool hasParent, object parentKey);

		internal abstract void Traverse(ProcessingStages operation, bool ascending, ITraversalContext traversalContext);

		internal abstract void CopyDomainScopeGroupInstances(RuntimeGroupRootObj destination);

		internal void SetOwner(RuntimeHierarchyObj owner)
		{
			m_owner = owner;
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.ObjectType)
				{
					writer.WriteEnum((int)m_objectType);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.ObjectType)
				{
					m_objectType = (Microsoft.ReportingServices.ReportProcessing.ObjectType)reader.ReadEnum();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public virtual Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObj;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.ObjectType, Token.Enum));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}

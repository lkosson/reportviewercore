using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal class RuntimeGroupingObjLinkedList : RuntimeGroupingObj
	{
		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		public override int Size => base.Size;

		internal RuntimeGroupingObjLinkedList()
		{
		}

		internal RuntimeGroupingObjLinkedList(RuntimeHierarchyObj owner, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(owner, objectType)
		{
		}

		internal override void Cleanup()
		{
		}

		internal override void NextRow(object keyValue, bool hasParent, object parentKey)
		{
			Global.Tracer.Assert(condition: false, "This implementation of RuntimeGroupingObj does not support NextRow");
		}

		internal override void Traverse(ProcessingStages operation, bool ascending, ITraversalContext traversalContext)
		{
			RuntimeGroupRootObj runtimeGroupRootObj = m_owner as RuntimeGroupRootObj;
			Global.Tracer.Assert(runtimeGroupRootObj != null, "(null != groupRootOwner)");
			runtimeGroupRootObj.TraverseLinkedGroupLeaves(operation, ascending, traversalContext);
		}

		internal override void CopyDomainScopeGroupInstances(RuntimeGroupRootObj destination)
		{
			Global.Tracer.Assert(condition: false, "Domain Scope should only be applied to Hash groups");
		}

		protected RuntimeHierarchyObjReference CreateHierarchyObjAndAddToParent()
		{
			RuntimeHierarchyObjReference runtimeHierarchyObjReference = null;
			try
			{
				RuntimeHierarchyObj runtimeHierarchyObj = new RuntimeHierarchyObj(m_owner, m_objectType, ((IScope)m_owner).Depth + 1);
				runtimeHierarchyObjReference = (RuntimeHierarchyObjReference)runtimeHierarchyObj.SelfReference;
				runtimeHierarchyObj.NextRow();
			}
			finally
			{
				if (null != runtimeHierarchyObjReference)
				{
					runtimeHierarchyObjReference.UnPinValue();
				}
			}
			return runtimeHierarchyObjReference;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				_ = writer.CurrentMember.MemberName;
				Global.Tracer.Assert(condition: false);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				_ = reader.CurrentMember.MemberName;
				Global.Tracer.Assert(condition: false);
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjLinkedList;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjLinkedList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObj, memberInfoList);
			}
			return m_declaration;
		}
	}
}

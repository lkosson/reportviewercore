using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeGroupingObjDetail : RuntimeGroupingObjLinkedList
	{
		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		public override int Size => base.Size;

		internal RuntimeGroupingObjDetail()
		{
		}

		internal RuntimeGroupingObjDetail(RuntimeHierarchyObj owner, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(owner, objectType)
		{
		}

		internal override void NextRow(object keyValue, bool hasParent, object parentKey)
		{
			Global.Tracer.Assert(!hasParent, "(!hasParent)");
			CreateHierarchyObjAndAddToParent();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjDetail;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjDetail, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjLinkedList, memberInfoList);
			}
			return m_declaration;
		}
	}
}

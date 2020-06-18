using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeTablixGroupLeafObj : RuntimeDataTablixWithScopedItemsGroupLeafObj
	{
		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		internal RuntimeTablixGroupLeafObj()
		{
		}

		internal RuntimeTablixGroupLeafObj(RuntimeDataTablixGroupRootObjReference groupRootRef, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(groupRootRef, objectType)
		{
		}

		protected override List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem> GetGroupScopedContents(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode member)
		{
			return ((TablixMember)member).GroupScopedContentsForProcessing;
		}

		protected override List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem> GetCellContents(Cell cell)
		{
			TablixCell tablixCell = cell as TablixCell;
			if (tablixCell != null && tablixCell.HasInnerGroupTreeHierarchy)
			{
				return tablixCell.CellContentCollection;
			}
			return null;
		}

		protected override RuntimeCell CreateCell(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember)
		{
			return new RuntimeTablixCell((RuntimeTablixGroupLeafObjReference)m_selfReference, (TablixMember)outerGroupingMember, (TablixMember)innerGroupingMember, !m_hasInnerHierarchy);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixGroupLeafObj;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				m_declaration = new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixGroupLeafObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixWithScopedItemsGroupLeafObj, memberInfoList);
			}
			return m_declaration;
		}
	}
}

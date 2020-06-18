using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeTablixCell : RuntimeCellWithContents
	{
		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		internal RuntimeTablixCell()
		{
		}

		internal RuntimeTablixCell(RuntimeTablixGroupLeafObjReference owner, TablixMember outerGroupingMember, TablixMember innerGroupingMember, bool innermost)
			: base(owner, outerGroupingMember, innerGroupingMember, innermost)
		{
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

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixCell;
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
			Global.Tracer.Assert(condition: false, GetType().Name + " should not resolve references");
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				m_declaration = new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixCell, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellWithContents, memberInfoList);
			}
			return m_declaration;
		}
	}
}

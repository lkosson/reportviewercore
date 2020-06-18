using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeChartCriCell : RuntimeCell
	{
		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		internal RuntimeChartCriCell()
		{
		}

		internal RuntimeChartCriCell(RuntimeChartCriGroupLeafObjReference owner, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember, bool innermost)
			: base(owner, outerGroupingMember, innerGroupingMember, innermost)
		{
		}

		protected override void ConstructCellContents(Cell cell, ref DataActions dataAction)
		{
		}

		protected override void CreateInstanceCellContents(Cell cell, DataCellInstance cellInstance, OnDemandProcessingContext odpContext)
		{
		}

		public override IOnDemandMemberOwnerInstanceReference GetDataRegionInstance(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion)
		{
			Global.Tracer.Assert(condition: false, "This type of cell does not support nested data regions.");
			throw new InvalidOperationException();
		}

		public override IReference<IDataCorrelation> GetIdcReceiver(IRIFReportDataScope scope)
		{
			Global.Tracer.Assert(condition: false, "This type of cell does not support nested data regions.");
			throw new InvalidOperationException();
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
			Global.Tracer.Assert(condition: false, "RuntimeChartCriCell should not resolve references");
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriCell;
		}

		internal new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriCell, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCell, memberInfoList);
			}
			return m_declaration;
		}
	}
}

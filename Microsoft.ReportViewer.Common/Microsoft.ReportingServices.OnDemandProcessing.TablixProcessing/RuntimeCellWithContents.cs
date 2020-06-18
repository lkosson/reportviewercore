using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeCellWithContents : RuntimeCell
	{
		private RuntimeRICollection m_cellContents;

		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		public override int Size => base.Size + ItemSizes.SizeOf(m_cellContents);

		internal RuntimeCellWithContents()
		{
		}

		internal RuntimeCellWithContents(RuntimeDataTablixGroupLeafObjReference owner, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember, bool innermost)
			: base(owner, outerGroupingMember, innerGroupingMember, innermost)
		{
		}

		protected abstract List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem> GetCellContents(Cell cell);

		protected override void ConstructCellContents(Cell cell, ref DataActions dataAction)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem> cellContents = GetCellContents(cell);
			if (cellContents != null)
			{
				m_owner.Value().OdpContext.TablixProcessingScalabilityCache.AllocateAndPin((RuntimeCell)this, m_outerGroupDynamicIndex);
				if (m_cellContents == null)
				{
					m_cellContents = new RuntimeRICollection(cellContents.Count);
				}
				m_cellContents.AddItems(m_selfReference, cellContents, ref dataAction, m_owner.Value().OdpContext);
			}
		}

		internal override bool NextRow()
		{
			bool result = base.NextRow();
			if (m_cellContents != null)
			{
				OnDemandProcessingContext odpContext = m_owner.Value().OdpContext;
				m_cellContents.FirstPassNextDataRow(odpContext);
			}
			return result;
		}

		protected override void TraverseCellContents(ProcessingStages operation, AggregateUpdateContext context)
		{
			if (m_cellContents != null)
			{
				switch (operation)
				{
				case ProcessingStages.SortAndFilter:
					m_cellContents.SortAndFilter(context);
					break;
				case ProcessingStages.UpdateAggregates:
					m_cellContents.UpdateAggregates(context);
					break;
				default:
					Global.Tracer.Assert(condition: false, "Invalid operation for TraverseCellContents.");
					break;
				}
			}
		}

		protected override void CalculateInnerRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
			if (m_cellContents != null)
			{
				m_cellContents.CalculateRunningValues(groupCol, lastGroup, aggContext);
			}
		}

		protected override void CreateInstanceCellContents(Cell cell, DataCellInstance cellInstance, OnDemandProcessingContext odpContext)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem> cellContents = GetCellContents(cell);
			if (cellContents != null && m_cellContents != null)
			{
				m_cellContents.CreateInstances(cellInstance, odpContext, m_selfReference, cellContents);
			}
		}

		public override IOnDemandMemberOwnerInstanceReference GetDataRegionInstance(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion)
		{
			Global.Tracer.Assert(m_cellContents != null, "Cannot find data region.");
			return m_cellContents.GetDataRegionObj(rifDataRegion);
		}

		public override IReference<IDataCorrelation> GetIdcReceiver(IRIFReportDataScope scope)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = scope as Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion;
			Global.Tracer.Assert(dataRegion != null, "Invalid scope.");
			return m_cellContents.GetDataRegionObj(dataRegion);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellWithContents;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.CellContents)
				{
					writer.Write(m_cellContents);
				}
				else
				{
					Global.Tracer.Assert(condition: false, string.Concat("Unsupported member name: ", writer.CurrentMember.MemberName, "."));
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.CellContents)
				{
					m_cellContents = (RuntimeRICollection)reader.ReadRIFObject();
				}
				else
				{
					Global.Tracer.Assert(condition: false, string.Concat("Unsupported member name: ", reader.CurrentMember.MemberName, "."));
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		internal new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.CellContents, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRICollection));
				m_declaration = new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellWithContents, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCell, list);
			}
			return m_declaration;
		}
	}
}

using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeDataTablixWithScopedItemsGroupLeafObj : RuntimeDataTablixGroupLeafObj
	{
		private RuntimeRICollection m_groupScopedItems;

		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		public override int Size => base.Size + ItemSizes.SizeOf(m_groupScopedItems);

		internal RuntimeDataTablixWithScopedItemsGroupLeafObj()
		{
		}

		internal RuntimeDataTablixWithScopedItemsGroupLeafObj(RuntimeDataTablixGroupRootObjReference groupRootRef, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(groupRootRef, objectType)
		{
		}

		protected abstract List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem> GetGroupScopedContents(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode member);

		protected abstract List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem> GetCellContents(Cell cell);

		protected abstract RuntimeCell CreateCell(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember);

		protected override void InitializeGroupScopedItems(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode member, ref DataActions innerDataAction)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem> groupScopedContents = GetGroupScopedContents(member);
			if (groupScopedContents != null)
			{
				if (m_groupScopedItems == null)
				{
					m_groupScopedItems = new RuntimeRICollection(groupScopedContents.Count);
				}
				m_groupScopedItems.AddItems(m_selfReference, groupScopedContents, ref innerDataAction, m_odpContext);
			}
		}

		protected override void ConstructOutermostCellContents(Cell cell)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem> cellContents = GetCellContents(cell);
			if (cellContents != null)
			{
				DataActions dataAction = DataActions.None;
				if (m_groupScopedItems == null)
				{
					m_groupScopedItems = new RuntimeRICollection(cellContents.Count);
				}
				m_groupScopedItems.AddItems(m_selfReference, cellContents, ref dataAction, m_odpContext);
			}
		}

		internal override void CreateCell(RuntimeCells cellsCollection, int collectionKey, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember, Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef)
		{
			RuntimeCell runtimeCell = CreateCell(outerGroupingMember, innerGroupingMember);
			if (runtimeCell.SelfReference == null)
			{
				cellsCollection.AddCell(collectionKey, runtimeCell);
				return;
			}
			IReference<RuntimeCell> selfReference = runtimeCell.SelfReference;
			selfReference.UnPinValue();
			cellsCollection.AddCell(collectionKey, selfReference);
		}

		protected override void SendToInner()
		{
			base.SendToInner();
			if ((base.IsOuterGrouping || !m_odpContext.PeerOuterGroupProcessing) && m_groupScopedItems != null)
			{
				m_groupScopedItems.FirstPassNextDataRow(m_odpContext);
			}
		}

		protected override void TraverseStaticContents(ProcessingStages operation, AggregateUpdateContext context)
		{
			if (m_groupScopedItems != null)
			{
				switch (operation)
				{
				case ProcessingStages.SortAndFilter:
					m_groupScopedItems.SortAndFilter(context);
					break;
				case ProcessingStages.UpdateAggregates:
					m_groupScopedItems.UpdateAggregates(context);
					break;
				default:
					Global.Tracer.Assert(condition: false, "Unknown operation in TraverseStaticContents.");
					break;
				}
			}
		}

		protected override void CalculateRunningValuesForStaticContents(AggregateUpdateContext aggContext)
		{
			if (!m_processHeading)
			{
				return;
			}
			RuntimeDataTablixGroupRootObjReference runtimeDataTablixGroupRootObjReference = (RuntimeDataTablixGroupRootObjReference)m_hierarchyRoot;
			using (runtimeDataTablixGroupRootObjReference.PinValue())
			{
				Dictionary<string, IReference<RuntimeGroupRootObj>> groupCollection = runtimeDataTablixGroupRootObjReference.Value().GroupCollection;
				RuntimeGroupRootObjReference lastGroup = runtimeDataTablixGroupRootObjReference;
				if (m_groupScopedItems != null)
				{
					m_groupScopedItems.CalculateRunningValues(groupCollection, lastGroup, aggContext);
				}
			}
		}

		protected override void CreateInstanceHeadingContents()
		{
			if (base.MemberDef.InScopeEventSources != null)
			{
				UserSortFilterContext.ProcessEventSources(m_odpContext, this, base.MemberDef.InScopeEventSources);
			}
			if (m_groupScopedItems != null)
			{
				List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem> groupScopedContents = GetGroupScopedContents(base.MemberDef);
				if (groupScopedContents != null)
				{
					m_groupScopedItems.CreateInstances(m_memberInstance, m_odpContext, m_selfReference, groupScopedContents);
				}
			}
		}

		protected override void CreateOutermostStaticCellContents(Cell cell, DataCellInstance cellInstance)
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem> cellContents = GetCellContents(cell);
			if (m_groupScopedItems != null && cellContents != null)
			{
				m_groupScopedItems.CreateInstances(cellInstance, m_odpContext, m_selfReference, cellContents);
			}
		}

		internal override RuntimeDataTablixObjReference GetNestedDataRegion(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion)
		{
			Global.Tracer.Assert(m_groupScopedItems != null, "Cannot find data region.");
			return m_groupScopedItems.GetDataRegionObj(rifDataRegion);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixWithScopedItemsGroupLeafObj;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.GroupScopedItems)
				{
					writer.Write(m_groupScopedItems);
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
				if (memberName == MemberName.GroupScopedItems)
				{
					m_groupScopedItems = (RuntimeRICollection)reader.ReadRIFObject();
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

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.GroupScopedItems, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRICollection));
				m_declaration = new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixWithScopedItemsGroupLeafObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObj, list);
			}
			return m_declaration;
		}
	}
}

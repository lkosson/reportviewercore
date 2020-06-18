using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeDataTablixWithScopedItemsObj : RuntimeDataTablixObj, IStorable, IPersistable
	{
		private RuntimeRICollection m_dataRegionScopedItems;

		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		public override int Size => base.Size + ItemSizes.SizeOf(m_dataRegionScopedItems);

		internal RuntimeDataTablixWithScopedItemsObj()
		{
		}

		internal RuntimeDataTablixWithScopedItemsObj(IReference<IScope> outerScope, Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataTablixDef, ref DataActions dataAction, OnDemandProcessingContext odpContext, bool onePassProcess, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(outerScope, dataTablixDef, ref dataAction, odpContext, onePassProcess, objectType)
		{
		}

		protected abstract List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem> GetDataRegionScopedItems();

		protected override void ConstructRuntimeStructure(ref DataActions innerDataAction, bool onePassProcess)
		{
			m_dataRegionScopedItems = null;
			base.ConstructRuntimeStructure(ref innerDataAction, onePassProcess);
			List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem> dataRegionScopedItems = GetDataRegionScopedItems();
			if (dataRegionScopedItems != null)
			{
				m_dataRegionScopedItems = new RuntimeRICollection(m_selfReference, dataRegionScopedItems, ref innerDataAction, m_odpContext);
			}
		}

		internal override RuntimeDataTablixObjReference GetNestedDataRegion(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion)
		{
			Global.Tracer.Assert(m_dataRegionScopedItems != null, "Cannot find data region.");
			return m_dataRegionScopedItems.GetDataRegionObj(rifDataRegion);
		}

		protected override void SendToInner()
		{
			base.SendToInner();
			if (m_dataRegionScopedItems != null)
			{
				m_dataRegionScopedItems.FirstPassNextDataRow(m_odpContext);
			}
		}

		protected override void CalculateRunningValuesForTopLevelStaticContents(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
			if (m_dataRegionScopedItems != null)
			{
				m_dataRegionScopedItems.CalculateRunningValues(groupCol, lastGroup, aggContext);
			}
		}

		protected override void Traverse(ProcessingStages operation, ITraversalContext context)
		{
			base.Traverse(operation, context);
			if (m_dataRegionScopedItems != null)
			{
				TraverseDataRegionScopedItems(operation, context);
			}
		}

		private void TraverseDataRegionScopedItems(ProcessingStages operation, ITraversalContext context)
		{
			switch (operation)
			{
			case ProcessingStages.SortAndFilter:
				m_dataRegionScopedItems.SortAndFilter((AggregateUpdateContext)context);
				break;
			case ProcessingStages.UpdateAggregates:
				m_dataRegionScopedItems.UpdateAggregates((AggregateUpdateContext)context);
				break;
			default:
				Global.Tracer.Assert(condition: false, "Unknown ProcessingStage for TraverseDataRegionScopedItems");
				break;
			}
		}

		protected override void CreateDataRegionScopedInstance(DataRegionInstance dataRegionInstance)
		{
			base.CreateDataRegionScopedInstance(dataRegionInstance);
			if (m_dataRegionScopedItems != null)
			{
				m_dataRegionScopedItems.CreateInstances(dataRegionInstance, m_odpContext, m_selfReference, GetDataRegionScopedItems());
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixWithScopedItemsObj;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.DataRegionScopedItems)
				{
					writer.Write(m_dataRegionScopedItems);
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
				if (memberName == MemberName.DataRegionScopedItems)
				{
					m_dataRegionScopedItems = (RuntimeRICollection)reader.ReadRIFObject();
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
				list.Add(new MemberInfo(MemberName.DataRegionScopedItems, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRICollection));
				m_declaration = new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixWithScopedItemsObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixObj, list);
			}
			return m_declaration;
		}
	}
}

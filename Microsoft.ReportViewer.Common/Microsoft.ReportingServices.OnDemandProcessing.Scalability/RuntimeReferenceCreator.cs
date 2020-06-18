using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeReferenceCreator : IReferenceCreator
	{
		private static RuntimeReferenceCreator m_instance = new RuntimeReferenceCreator();

		internal static RuntimeReferenceCreator Instance => m_instance;

		private RuntimeReferenceCreator()
		{
		}

		public bool TryCreateReference(IStorable refTarget, out BaseReference newReference)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType objectType = refTarget.GetObjectType();
			if (TryMapObjectTypeToReferenceType(objectType, out Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType referenceType))
			{
				return TryCreateReference(referenceType, out newReference);
			}
			newReference = null;
			return false;
		}

		public bool TryCreateReference(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType referenceObjectType, out BaseReference reference)
		{
			switch (referenceObjectType)
			{
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Null:
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None:
				Global.Tracer.Assert(condition: false, "Cannot create reference to Nothing or Null");
				reference = null;
				return false;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixCellReference:
				reference = new RuntimeTablixCellReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellReference:
				reference = new RuntimeCellReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDetailObjReference:
				reference = new RuntimeDetailObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixObjReference:
				reference = new RuntimeDataTablixObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObjReference:
				reference = new RuntimeHierarchyObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObjReference:
				reference = new RuntimeDataRegionObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupRootObjReference:
				reference = new RuntimeDataTablixGroupRootObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupRootObjReference:
				reference = new RuntimeGroupRootObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObjReference:
				reference = new RuntimeGroupObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixGroupLeafObjReference:
				reference = new RuntimeTablixGroupLeafObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriGroupLeafObjReference:
				reference = new RuntimeChartCriGroupLeafObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObjReference:
				reference = new RuntimeDataTablixGroupLeafObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObjReference:
				reference = new RuntimeGroupLeafObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeOnDemandDataSetObjReference:
				reference = new RuntimeOnDemandDataSetObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortHierarchyObjReference:
				reference = new RuntimeSortHierarchyObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixObjReference:
				reference = new RuntimeTablixObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartObjReference:
				reference = new RuntimeChartObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGaugePanelObjReference:
				reference = new RuntimeGaugePanelObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMapDataRegionObjReference:
				reference = new RuntimeMapDataRegionObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCriObjReference:
				reference = new RuntimeCriObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AggregateRowReference:
				reference = new SimpleReference<AggregateRow>(referenceObjectType);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateReference:
				reference = new SimpleReference<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregate>(referenceObjectType);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjReference:
				reference = new SimpleReference<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj>(referenceObjectType);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRowReference:
				reference = new SimpleReference<DataFieldRow>(referenceObjectType);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IHierarchyObjReference:
				reference = new SimpleReference<IHierarchyObj>(referenceObjectType);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellsReference:
				reference = new SimpleReference<RuntimeCells>(referenceObjectType);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriCellReference:
				reference = new RuntimeChartCriCellReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriObjReference:
				reference = new RuntimeChartCriObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixMemberObjReference:
				reference = new RuntimeDataTablixMemberObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjReference:
				reference = new SimpleReference<RuntimeGroupingObj>(referenceObjectType);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObjReference:
				reference = new RuntimeMemberObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRDLDataRegionObjReference:
				reference = new SimpleReference<RuntimeRDLDataRegionObj>(referenceObjectType);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRICollectionReference:
				reference = new SimpleReference<RuntimeRICollection>(referenceObjectType);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortFilterEventInfoReference:
				reference = new SimpleReference<Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing.RuntimeSortFilterEventInfo>(referenceObjectType);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeUserSortTargetInfoReference:
				reference = new SimpleReference<Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing.RuntimeUserSortTargetInfo>(referenceObjectType);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortExpressionScopeInstanceHolderReference:
				reference = new SortExpressionScopeInstanceHolderReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterExpressionScopeObjReference:
				reference = new SortFilterExpressionScopeObjReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArrayReference:
				reference = new SimpleReference<StorableArray>(referenceObjectType);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference:
				reference = new ScalableDictionaryNodeReference();
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTableReference:
				reference = new SimpleReference<LookupTable>(referenceObjectType);
				break;
			default:
				reference = null;
				return false;
			}
			return true;
		}

		private bool TryMapObjectTypeToReferenceType(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType targetType, out Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType referenceType)
		{
			switch (targetType)
			{
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRowReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ObjectModelImpl:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeOnDemandDataSetObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortHierarchyObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortHierarchyObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IHierarchyObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IHierarchyObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortFilterEventInfo:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortFilterEventInfoReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterExpressionScopeObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterExpressionScopeObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortExpressionScopeInstanceHolder:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortExpressionScopeInstanceHolderReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCell:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRICollection:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRICollectionReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixCell:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixCellReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriCell:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriCellReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRDLDataRegionObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRDLDataRegionObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeUserSortTargetInfo:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeUserSortTargetInfoReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AggregateRow:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AggregateRowReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IScope:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IScopeReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCells:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellsReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDetailObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDetailObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupRootObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupRootObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupRootObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupRootObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixMemberObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixMemberObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGaugePanelObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGaugePanelObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMapDataRegionObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMapDataRegionObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCriObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCriObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixGroupLeafObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixGroupLeafObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriGroupLeafObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriGroupLeafObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeOnDemandDataSetObj:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeOnDemandDataSetObjReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArray:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArrayReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNode:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTable:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTableReference;
				break;
			default:
				referenceType = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None;
				return false;
			}
			return true;
		}
	}
}

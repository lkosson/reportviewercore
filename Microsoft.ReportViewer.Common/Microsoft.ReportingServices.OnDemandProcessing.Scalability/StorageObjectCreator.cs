using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class StorageObjectCreator : IScalabilityObjectCreator
	{
		private static List<Declaration> m_declarations = BuildDeclarations();

		private static StorageObjectCreator m_instance = null;

		internal static StorageObjectCreator Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new StorageObjectCreator();
				}
				return m_instance;
			}
		}

		private StorageObjectCreator()
		{
		}

		public bool TryCreateObject(ObjectType objectType, out IPersistable persistObj)
		{
			switch (objectType)
			{
			case ObjectType.Aggregate:
				persistObj = new Aggregate();
				break;
			case ObjectType.AggregateRow:
				persistObj = new AggregateRow();
				break;
			case ObjectType.Avg:
				persistObj = new Avg();
				break;
			case ObjectType.BTree:
				persistObj = new BTree();
				break;
			case ObjectType.BTreeNode:
				persistObj = new BTreeNode();
				break;
			case ObjectType.BTreeNodeTupleList:
				persistObj = new BTreeNodeTupleList();
				break;
			case ObjectType.BTreeNodeTuple:
				persistObj = new BTreeNodeTuple();
				break;
			case ObjectType.BTreeNodeHierarchyObj:
				persistObj = new BTreeNodeHierarchyObj();
				break;
			case ObjectType.CalculatedFieldWrapperImpl:
				persistObj = new CalculatedFieldWrapperImpl();
				break;
			case ObjectType.ChildLeafInfo:
				persistObj = new ChildLeafInfo();
				break;
			case ObjectType.Count:
				persistObj = new Count();
				break;
			case ObjectType.CountDistinct:
				persistObj = new CountDistinct();
				break;
			case ObjectType.CountRows:
				persistObj = new CountRows();
				break;
			case ObjectType.DataAggregateObj:
				persistObj = new DataAggregateObj();
				break;
			case ObjectType.DataAggregateObjResult:
				persistObj = new DataAggregateObjResult();
				break;
			case ObjectType.DataFieldRow:
				persistObj = new DataFieldRow();
				break;
			case ObjectType.DataRegionMemberInstance:
				persistObj = new DataRegionMemberInstance();
				break;
			case ObjectType.FieldImpl:
				persistObj = new FieldImpl();
				break;
			case ObjectType.First:
				persistObj = new First();
				break;
			case ObjectType.Last:
				persistObj = new Last();
				break;
			case ObjectType.Max:
				persistObj = new Max();
				break;
			case ObjectType.Min:
				persistObj = new Min();
				break;
			case ObjectType.Previous:
				persistObj = new Previous();
				break;
			case ObjectType.RuntimeCells:
				persistObj = new RuntimeCells();
				break;
			case ObjectType.RuntimeChartCriCell:
				persistObj = new RuntimeChartCriCell();
				break;
			case ObjectType.RuntimeChartCriGroupLeafObj:
				persistObj = new RuntimeChartCriGroupLeafObj();
				break;
			case ObjectType.RuntimeChartObj:
				persistObj = new RuntimeChartObj();
				break;
			case ObjectType.RuntimeGaugePanelObj:
				persistObj = new RuntimeGaugePanelObj();
				break;
			case ObjectType.RuntimeCriObj:
				persistObj = new RuntimeCriObj();
				break;
			case ObjectType.RuntimeDataTablixGroupRootObj:
				persistObj = new RuntimeDataTablixGroupRootObj();
				break;
			case ObjectType.RuntimeDataTablixMemberObj:
				persistObj = new RuntimeDataTablixMemberObj();
				break;
			case ObjectType.RuntimeExpressionInfo:
				persistObj = new RuntimeExpressionInfo();
				break;
			case ObjectType.RuntimeHierarchyObj:
				persistObj = new RuntimeHierarchyObj();
				break;
			case ObjectType.RuntimeRICollection:
				persistObj = new RuntimeRICollection();
				break;
			case ObjectType.RuntimeSortDataHolder:
				persistObj = new RuntimeSortDataHolder();
				break;
			case ObjectType.RuntimeSortFilterEventInfo:
				persistObj = new RuntimeSortFilterEventInfo();
				break;
			case ObjectType.RuntimeSortHierarchyObj:
				persistObj = new RuntimeSortHierarchyObj();
				break;
			case ObjectType.RuntimeDataRowSortHierarchyObj:
				persistObj = new RuntimeDataRowSortHierarchyObj();
				break;
			case ObjectType.RuntimeTablixCell:
				persistObj = new RuntimeTablixCell();
				break;
			case ObjectType.RuntimeTablixGroupLeafObj:
				persistObj = new RuntimeTablixGroupLeafObj();
				break;
			case ObjectType.RuntimeTablixObj:
				persistObj = new RuntimeTablixObj();
				break;
			case ObjectType.RuntimeUserSortTargetInfo:
				persistObj = new RuntimeUserSortTargetInfo();
				break;
			case ObjectType.ScopeLookupTable:
				persistObj = new ScopeLookupTable();
				break;
			case ObjectType.SortExpressionScopeInstanceHolder:
				persistObj = new RuntimeSortFilterEventInfo.SortExpressionScopeInstanceHolder();
				break;
			case ObjectType.SortFilterExpressionScopeObj:
				persistObj = new RuntimeSortFilterEventInfo.SortFilterExpressionScopeObj();
				break;
			case ObjectType.SortHierarchyStruct:
				persistObj = new RuntimeSortHierarchyObj.SortHierarchyStructure();
				break;
			case ObjectType.SortScopeValuesHolder:
				persistObj = new RuntimeSortFilterEventInfo.SortScopeValuesHolder();
				break;
			case ObjectType.StDev:
				persistObj = new StDev();
				break;
			case ObjectType.StDevP:
				persistObj = new StDevP();
				break;
			case ObjectType.StorageItem:
				persistObj = new StorageItem();
				break;
			case ObjectType.Sum:
				persistObj = new Sum();
				break;
			case ObjectType.Var:
				persistObj = new Var();
				break;
			case ObjectType.VarP:
				persistObj = new VarP();
				break;
			case ObjectType.FilterKey:
				persistObj = new Filters.FilterKey();
				break;
			case ObjectType.LookupMatches:
				persistObj = new LookupMatches();
				break;
			case ObjectType.LookupMatchesWithRows:
				persistObj = new LookupMatchesWithRows();
				break;
			case ObjectType.LookupTable:
				persistObj = new LookupTable();
				break;
			case ObjectType.Union:
				persistObj = new Union();
				break;
			case ObjectType.RuntimeMapDataRegionObj:
				persistObj = new RuntimeMapDataRegionObj();
				break;
			case ObjectType.DataScopeInfo:
				persistObj = new DataScopeInfo();
				break;
			case ObjectType.BucketedDataAggregateObjs:
				persistObj = new BucketedDataAggregateObjs();
				break;
			case ObjectType.DataAggregateObjBucket:
				persistObj = new DataAggregateObjBucket();
				break;
			case ObjectType.RuntimeGroupingObjHash:
				persistObj = new RuntimeGroupingObjHash();
				break;
			case ObjectType.RuntimeGroupingObjTree:
				persistObj = new RuntimeGroupingObjTree();
				break;
			case ObjectType.RuntimeGroupingObjDetail:
				persistObj = new RuntimeGroupingObjDetail();
				break;
			case ObjectType.RuntimeGroupingObjDetailUserSort:
				persistObj = new RuntimeGroupingObjDetailUserSort();
				break;
			case ObjectType.RuntimeGroupingObjLinkedList:
				persistObj = new RuntimeGroupingObjLinkedList();
				break;
			case ObjectType.RuntimeGroupingObjNaturalGroup:
				persistObj = new RuntimeGroupingObjNaturalGroup();
				break;
			default:
				persistObj = null;
				return false;
			}
			return true;
		}

		public List<Declaration> GetDeclarations()
		{
			return m_declarations;
		}

		private static List<Declaration> BuildDeclarations()
		{
			return new List<Declaration>(83)
			{
				Aggregate.GetDeclaration(),
				AggregateRow.GetDeclaration(),
				Avg.GetDeclaration(),
				BTree.GetDeclaration(),
				BTreeNode.GetDeclaration(),
				BTreeNodeTuple.GetDeclaration(),
				BTreeNodeTupleList.GetDeclaration(),
				BTreeNodeHierarchyObj.GetDeclaration(),
				CalculatedFieldWrapperImpl.GetDeclaration(),
				ChildLeafInfo.GetDeclaration(),
				Count.GetDeclaration(),
				CountDistinct.GetDeclaration(),
				CountRows.GetDeclaration(),
				DataAggregateObj.GetDeclaration(),
				DataAggregateObjResult.GetDeclaration(),
				DataRegionMemberInstance.GetDeclaration(),
				DataFieldRow.GetDeclaration(),
				FieldImpl.GetDeclaration(),
				First.GetDeclaration(),
				Last.GetDeclaration(),
				Max.GetDeclaration(),
				Min.GetDeclaration(),
				Previous.GetDeclaration(),
				RuntimeCell.GetDeclaration(),
				RuntimeCells.GetDeclaration(),
				RuntimeCellWithContents.GetDeclaration(),
				RuntimeChartCriCell.GetDeclaration(),
				RuntimeChartCriGroupLeafObj.GetDeclaration(),
				RuntimeChartCriObj.GetDeclaration(),
				RuntimeChartObj.GetDeclaration(),
				RuntimeCriObj.GetDeclaration(),
				RuntimeDataRegionObj.GetDeclaration(),
				RuntimeDataTablixObj.GetDeclaration(),
				RuntimeDataTablixGroupLeafObj.GetDeclaration(),
				RuntimeDataTablixGroupRootObj.GetDeclaration(),
				RuntimeDataTablixMemberObj.GetDeclaration(),
				RuntimeDataTablixWithScopedItemsObj.GetDeclaration(),
				RuntimeDataTablixWithScopedItemsGroupLeafObj.GetDeclaration(),
				RuntimeDetailObj.GetDeclaration(),
				RuntimeExpressionInfo.GetDeclaration(),
				RuntimeGroupLeafObj.GetDeclaration(),
				RuntimeGroupObj.GetDeclaration(),
				RuntimeGroupRootObj.GetDeclaration(),
				RuntimeGroupingObj.GetDeclaration(),
				RuntimeHierarchyObj.GetDeclaration(),
				RuntimeMemberObj.GetDeclaration(),
				RuntimeRDLDataRegionObj.GetDeclaration(),
				RuntimeRICollection.GetDeclaration(),
				RuntimeSortDataHolder.GetDeclaration(),
				RuntimeSortFilterEventInfo.GetDeclaration(),
				RuntimeSortFilterEventInfo.SortExpressionScopeInstanceHolder.GetDeclaration(),
				RuntimeSortFilterEventInfo.SortFilterExpressionScopeObj.GetDeclaration(),
				RuntimeSortFilterEventInfo.SortScopeValuesHolder.GetDeclaration(),
				RuntimeSortHierarchyObj.GetDeclaration(),
				RuntimeSortHierarchyObj.SortHierarchyStructure.GetDeclaration(),
				RuntimeDataRowSortHierarchyObj.GetDeclaration(),
				RuntimeTablixCell.GetDeclaration(),
				RuntimeTablixGroupLeafObj.GetDeclaration(),
				RuntimeTablixObj.GetDeclaration(),
				RuntimeUserSortTargetInfo.GetDeclaration(),
				ScopeInstance.GetDeclaration(),
				ScopeLookupTable.GetDeclaration(),
				StDev.GetDeclaration(),
				StDevP.GetDeclaration(),
				Sum.GetDeclaration(),
				Var.GetDeclaration(),
				VarBase.GetDeclaration(),
				VarP.GetDeclaration(),
				Filters.FilterKey.GetDeclaration(),
				RuntimeGaugePanelObj.GetDeclaration(),
				LookupMatches.GetDeclaration(),
				LookupMatchesWithRows.GetDeclaration(),
				LookupTable.GetDeclaration(),
				RuntimeMapDataRegionObj.GetDeclaration(),
				DataScopeInfo.GetDeclaration(),
				BucketedDataAggregateObjs.GetDeclaration(),
				DataAggregateObjBucket.GetDeclaration(),
				RuntimeGroupingObjHash.GetDeclaration(),
				RuntimeGroupingObjTree.GetDeclaration(),
				RuntimeGroupingObjDetail.GetDeclaration(),
				RuntimeGroupingObjDetailUserSort.GetDeclaration(),
				RuntimeGroupingObjLinkedList.GetDeclaration(),
				RuntimeGroupingObjNaturalGroup.GetDeclaration()
			};
		}
	}
}

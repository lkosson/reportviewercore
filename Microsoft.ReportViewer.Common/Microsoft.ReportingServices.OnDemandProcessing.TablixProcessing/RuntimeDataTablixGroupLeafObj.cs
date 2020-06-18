using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeDataTablixGroupLeafObj : RuntimeGroupLeafObj, ISortDataHolder, IStorable, IPersistable, IOnDemandMemberInstance, IOnDemandMemberOwnerInstance, IOnDemandScopeInstance
	{
		protected IReference<RuntimeMemberObj>[] m_memberObjs;

		protected bool m_hasInnerHierarchy;

		protected List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_firstPassCellNonCustomAggs;

		protected List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_firstPassCellCustomAggs;

		protected RuntimeCells[] m_cellsList;

		protected List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_cellPostSortAggregates;

		protected int m_groupLeafIndex = -1;

		protected bool m_processHeading = true;

		[NonSerialized]
		protected DataRegionMemberInstance m_memberInstance;

		protected int m_sequentialMemberIndexWithinScopeLevel = -1;

		protected Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] m_runningValueValues;

		protected Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] m_runningValueOfAggregateValues;

		protected Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] m_cellRunningValueValues;

		protected int m_instanceIndex = -1;

		private long m_scopeInstanceNumber;

		[NonSerialized]
		private int m_bufferIndex = -1;

		private const int BeforeFirstRowInBuffer = -1;

		private const int AfterLastRowInBuffer = -2;

		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		internal List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> CellPostSortAggregates => m_cellPostSortAggregates;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion DataRegionDef => base.MemberDef.DataRegionDef;

		internal int HeadingLevel => ((RuntimeDataTablixGroupRootObj)m_hierarchyRoot.Value()).HeadingLevel;

		internal int InstanceIndex => m_instanceIndex;

		internal int GroupLeafIndex => m_groupLeafIndex;

		protected bool HasInnerStaticMembersInSameScope
		{
			get
			{
				if (base.MemberDef.InnerStaticMembersInSameScope != null && base.MemberDef.InnerStaticMembersInSameScope.Count != 0)
				{
					return true;
				}
				return false;
			}
		}

		List<object> IOnDemandMemberInstance.GroupExprValues => m_groupExprValues;

		private List<int> OutermostRowIndexes
		{
			get
			{
				if (base.MemberDef.IsColumn)
				{
					return base.MemberDef.DataRegionDef.OutermostStaticRowIndexes;
				}
				return base.MemberDef.GetCellIndexes();
			}
		}

		private List<int> OutermostColumnIndexes
		{
			get
			{
				if (base.MemberDef.IsColumn)
				{
					return base.MemberDef.GetCellIndexes();
				}
				return base.MemberDef.DataRegionDef.OutermostStaticColumnIndexes;
			}
		}

		public bool IsNoRows => m_firstRow == null;

		public bool IsMostRecentlyCreatedScopeInstance => m_hierarchyDef.DataScopeInfo.IsLastScopeInstanceNumber(m_scopeInstanceNumber);

		public bool HasUnProcessedServerAggregate
		{
			get
			{
				if (m_customAggregates != null && m_customAggregates.Count > 0)
				{
					return !m_hasProcessedAggregateRow;
				}
				return false;
			}
		}

		public override int Size => base.Size + ItemSizes.SizeOf(m_memberObjs) + 1 + ItemSizes.SizeOf(m_firstPassCellNonCustomAggs) + ItemSizes.SizeOf(m_firstPassCellCustomAggs) + ItemSizes.SizeOf(m_cellsList) + ItemSizes.SizeOf(m_cellPostSortAggregates) + 4 + 1 + ItemSizes.ReferenceSize + 4 + 4 + ItemSizes.SizeOf(m_runningValueValues) + ItemSizes.SizeOf(m_runningValueOfAggregateValues) + ItemSizes.SizeOf(m_cellRunningValueValues) + 8;

		internal RuntimeDataTablixGroupLeafObj()
		{
		}

		internal RuntimeDataTablixGroupLeafObj(RuntimeDataTablixGroupRootObjReference groupRootRef, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(groupRootRef, objectType)
		{
			using (groupRootRef.PinValue())
			{
				RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = groupRootRef.Value();
				Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode hierarchyDef = runtimeDataTablixGroupRootObj.HierarchyDef;
				bool handleMyDataAction = false;
				bool num = HandleSortFilterEvent(hierarchyDef.IsColumn);
				ConstructorHelper(runtimeDataTablixGroupRootObj, DataRegionDef, out handleMyDataAction, out DataActions innerDataAction);
				InitializeGroupScopedItems(hierarchyDef, ref innerDataAction);
				if (!handleMyDataAction)
				{
					m_dataAction = innerDataAction;
				}
				if (num)
				{
					m_dataAction |= DataActions.UserSort;
				}
				if (m_dataAction != 0 || DataRegionDef.IsMatrixIDC)
				{
					m_dataRows = new ScalableList<DataFieldRow>(m_depth + 1, m_odpContext.TablixProcessingScalabilityCache, 30);
				}
			}
			m_odpContext.CreatedScopeInstance(m_hierarchyDef);
			m_scopeInstanceNumber = RuntimeDataRegionObj.AssignScopeInstanceNumber(m_hierarchyDef.DataScopeInfo);
		}

		protected virtual void InitializeGroupScopedItems(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode member, ref DataActions innerDataAction)
		{
		}

		void ISortDataHolder.NextRow()
		{
			if (m_detailRowCounter == 0)
			{
				NextRow();
			}
			else
			{
				if (m_detailSortAdditionalGroupLeafs == null)
				{
					m_detailSortAdditionalGroupLeafs = new List<IHierarchyObj>();
				}
				IHierarchyObj hierarchyObj = base.GroupRoot.Value().CreateDetailSortHierarchyObj(this);
				m_detailSortAdditionalGroupLeafs.Add(hierarchyObj);
				hierarchyObj.NextRow(this);
			}
			m_detailRowCounter++;
		}

		void ISortDataHolder.Traverse(ProcessingStages operation, ITraversalContext traversalContext)
		{
			TablixProcessingMoveNext(operation);
			switch (operation)
			{
			case ProcessingStages.SortAndFilter:
				SortAndFilter((AggregateUpdateContext)traversalContext);
				break;
			case ProcessingStages.PreparePeerGroupRunningValues:
				PrepareCalculateRunningValues();
				break;
			case ProcessingStages.RunningValues:
				CalculateDetailSortRunningValues((AggregateUpdateContext)traversalContext);
				break;
			case ProcessingStages.CreateGroupTree:
				CreateInstance((CreateInstancesTraversalContext)traversalContext);
				break;
			case ProcessingStages.UpdateAggregates:
				UpdateAggregates((AggregateUpdateContext)traversalContext);
				break;
			default:
				Global.Tracer.Assert(condition: false);
				break;
			}
			int num = (m_detailSortAdditionalGroupLeafs != null) ? m_detailSortAdditionalGroupLeafs.Count : 0;
			for (int i = 0; i < num; i++)
			{
				m_detailSortAdditionalGroupLeafs[i].Traverse(operation, traversalContext);
			}
		}

		private void CalculateDetailSortRunningValues(AggregateUpdateContext aggContext)
		{
			if (m_dataRows == null || (m_dataAction & DataActions.PostSortAggregates) == 0)
			{
				return;
			}
			_ = m_dataRows.Count;
			using (m_hierarchyRoot.PinValue())
			{
				RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = m_hierarchyRoot.Value() as RuntimeDataTablixGroupRootObj;
				if (runtimeDataTablixGroupRootObj.DetailDataRows == null)
				{
					runtimeDataTablixGroupRootObj.DetailDataRows = new ScalableList<DataFieldRow>(runtimeDataTablixGroupRootObj.Depth, m_odpContext.TablixProcessingScalabilityCache);
				}
				UpdateSortFilterInfo(runtimeDataTablixGroupRootObj, runtimeDataTablixGroupRootObj.HierarchyDef.IsColumn, runtimeDataTablixGroupRootObj.DetailDataRows.Count);
				runtimeDataTablixGroupRootObj.DetailDataRows.AddRange(m_dataRows);
			}
			CalculateRunningValues(aggContext);
		}

		private void UpdateSortFilterInfo(RuntimeGroupRootObj detailRoot, bool isColumnAxis, int rootRowCount)
		{
			List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo = m_odpContext.RuntimeSortFilterInfo;
			if (runtimeSortFilterInfo == null || detailRoot.HierarchyDef.DataRegionDef.SortFilterSourceDetailScopeInfo == null)
			{
				return;
			}
			for (int i = 0; i < runtimeSortFilterInfo.Count; i++)
			{
				IReference<RuntimeSortFilterEventInfo> reference = runtimeSortFilterInfo[i];
				using (reference.PinValue())
				{
					RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
					if (base.SelfReference == runtimeSortFilterEventInfo.GetEventSourceScope(isColumnAxis))
					{
						runtimeSortFilterEventInfo.UpdateEventSourceScope(isColumnAxis, detailRoot.SelfReference, rootRowCount);
					}
					if (runtimeSortFilterEventInfo.HasDetailScopeInfo)
					{
						runtimeSortFilterEventInfo.UpdateDetailScopeInfo(detailRoot, isColumnAxis, rootRowCount, base.SelfReference);
					}
				}
			}
		}

		protected void ConstructorHelper(RuntimeDataTablixGroupRootObj groupRoot, Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, out bool handleMyDataAction, out DataActions innerDataAction)
		{
			m_dataAction = groupRoot.DataAction;
			handleMyDataAction = false;
			if (m_hierarchyDef.DataScopeInfo != null && m_hierarchyDef.DataScopeInfo.HasAggregatesToUpdateAtRowScope)
			{
				m_dataAction |= DataActions.AggregatesOfAggregates;
				handleMyDataAction = true;
			}
			if (groupRoot.ProcessOutermostStaticCells)
			{
				List<int> outermostColumnIndexes = OutermostColumnIndexes;
				foreach (int outermostRowIndex in OutermostRowIndexes)
				{
					foreach (int item in outermostColumnIndexes)
					{
						handleMyDataAction |= CreateCellAggregates(dataRegionDef, outermostRowIndex, item);
					}
				}
			}
			ConstructRuntimeStructure(ref handleMyDataAction, out innerDataAction);
			if (base.IsOuterGrouping)
			{
				RuntimeDataTablixObjReference runtimeDataTablixObjReference = (RuntimeDataTablixObjReference)dataRegionDef.RuntimeDataRegionObj;
				using (runtimeDataTablixObjReference.PinValue())
				{
					RuntimeDataTablixObj runtimeDataTablixObj = runtimeDataTablixObjReference.Value();
					int hierarchyDynamicIndex = groupRoot.HierarchyDef.HierarchyDynamicIndex;
					m_groupLeafIndex = runtimeDataTablixObj.OuterGroupingCounters[hierarchyDynamicIndex] + 1;
					runtimeDataTablixObj.OuterGroupingCounters[hierarchyDynamicIndex] = m_groupLeafIndex;
				}
				dataRegionDef.UpdateOuterGroupingIndexes(m_hierarchyRoot as RuntimeDataTablixGroupRootObjReference, m_groupLeafIndex);
			}
		}

		private bool CreateCellAggregates(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, int rowIndex, int colIndex)
		{
			bool result = false;
			Cell cell = dataRegionDef.Rows[rowIndex].Cells[colIndex];
			if (cell != null && !cell.SimpleGroupTreeCell)
			{
				if (cell.AggregateIndexes != null)
				{
					RuntimeDataRegionObj.CreateAggregates(m_odpContext, dataRegionDef.CellAggregates, cell.AggregateIndexes, ref m_firstPassCellNonCustomAggs, ref m_firstPassCellCustomAggs);
				}
				if (cell.HasInnerGroupTreeHierarchy)
				{
					ConstructOutermostCellContents(cell);
				}
				if (cell.PostSortAggregateIndexes != null)
				{
					result = true;
					RuntimeDataRegionObj.CreateAggregates(m_odpContext, dataRegionDef.CellPostSortAggregates, cell.PostSortAggregateIndexes, ref m_postSortAggregates);
				}
			}
			return result;
		}

		protected abstract void ConstructOutermostCellContents(Cell cell);

		protected override void ConstructRuntimeStructure(ref bool handleMyDataAction, out DataActions innerDataAction)
		{
			using (m_hierarchyRoot.PinValue())
			{
				RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = m_hierarchyRoot.Value() as RuntimeDataTablixGroupRootObj;
				Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode hierarchyDef = runtimeDataTablixGroupRootObj.HierarchyDef;
				Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = hierarchyDef.DataRegionDef;
				base.ConstructRuntimeStructure(ref handleMyDataAction, out innerDataAction);
				if (!base.IsOuterGrouping && (!hierarchyDef.HasInnerDynamic || runtimeDataTablixGroupRootObj.HasLeafCells))
				{
					if (m_cellsList == null)
					{
						m_cellsList = new RuntimeCells[dataRegionDef.OuterGroupingDynamicMemberCount];
						RuntimeDataRegionObj.CreateAggregates(m_odpContext, dataRegionDef.CellPostSortAggregates, ref m_cellPostSortAggregates);
					}
					int outerGroupingDynamicMemberCount = dataRegionDef.OuterGroupingDynamicMemberCount;
					for (int i = 0; i < outerGroupingDynamicMemberCount; i++)
					{
						IReference<RuntimeDataTablixGroupRootObj> reference = dataRegionDef.CurrentOuterGroupRootObjs[i];
						if (reference == null)
						{
							break;
						}
						CreateRuntimeCells(reference.Value());
					}
				}
				int num = hierarchyDef.HasInnerDynamic ? hierarchyDef.InnerDynamicMembers.Count : 0;
				m_hasInnerHierarchy = (num > 0);
				m_memberObjs = new IReference<RuntimeMemberObj>[Math.Max(1, num)];
				if (num == 0)
				{
					IReference<RuntimeMemberObj> reference2 = RuntimeDataTablixMemberObj.CreateRuntimeMemberObject(m_selfReference, null, ref innerDataAction, runtimeDataTablixGroupRootObj.OdpContext, runtimeDataTablixGroupRootObj.InnerGroupings, hierarchyDef.InnerStaticMembersInSameScope, runtimeDataTablixGroupRootObj.OutermostStatics, runtimeDataTablixGroupRootObj.HeadingLevel + 1, base.ObjectType);
					m_memberObjs[0] = reference2;
					return;
				}
				for (int j = 0; j < num; j++)
				{
					IReference<RuntimeMemberObj> reference3 = RuntimeDataTablixMemberObj.CreateRuntimeMemberObject(m_selfReference, hierarchyDef.InnerDynamicMembers[j], ref innerDataAction, runtimeDataTablixGroupRootObj.OdpContext, runtimeDataTablixGroupRootObj.InnerGroupings, (j == 0) ? hierarchyDef.InnerStaticMembersInSameScope : null, runtimeDataTablixGroupRootObj.OutermostStatics, runtimeDataTablixGroupRootObj.HeadingLevel + 1, base.ObjectType);
					m_memberObjs[j] = reference3;
				}
			}
		}

		private void CreateRuntimeCells(RuntimeDataTablixGroupRootObj outerGroupRoot)
		{
			if (outerGroupRoot != null && outerGroupRoot.HasLeafCells)
			{
				int hierarchyDynamicIndex = outerGroupRoot.HierarchyDef.HierarchyDynamicIndex;
				if (m_cellsList[hierarchyDynamicIndex] == null)
				{
					m_cellsList[hierarchyDynamicIndex] = new RuntimeCells(base.Depth, m_odpContext.TablixProcessingScalabilityCache);
				}
			}
		}

		internal abstract void CreateCell(RuntimeCells cellsCollection, int collectionKey, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember, Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef);

		internal override void NextRow()
		{
			DomainScopeContext domainScopeContext = base.OdpContext.DomainScopeContext;
			if (domainScopeContext != null)
			{
				DomainScopeContext.DomainScopeInfo currentDomainScope = domainScopeContext.CurrentDomainScope;
				if (currentDomainScope != null)
				{
					if (m_firstRow == null)
					{
						m_firstRow = currentDomainScope.CurrentRow;
					}
					return;
				}
			}
			if (base.IsOuterGrouping)
			{
				DataRegionDef.CurrentOuterGroupRoot = (RuntimeDataTablixGroupRootObjReference)m_hierarchyRoot;
				DataRegionDef.UpdateOuterGroupingIndexes((RuntimeDataTablixGroupRootObjReference)m_hierarchyRoot, m_groupLeafIndex);
			}
			if (base.IsOuterGrouping || !m_odpContext.PeerOuterGroupProcessing)
			{
				UpdateAggregateInfo();
			}
			if (base.IsOuterGrouping)
			{
				DataRegionDef.SaveOuterGroupingAggregateRowInfo(m_hierarchyDef.HierarchyDynamicIndex, m_odpContext);
			}
			if (base.IsOuterGrouping || !m_odpContext.PeerOuterGroupProcessing)
			{
				FieldsImpl fieldsImpl = m_odpContext.ReportObjectModel.FieldsImpl;
				if (fieldsImpl.AggregationFieldCount == 0 && fieldsImpl.ValidAggregateRow)
				{
					RuntimeDataRegionObj.UpdateAggregates(m_odpContext, m_firstPassCellCustomAggs, updateAndSetup: false);
				}
				if (!m_odpContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					RuntimeDataRegionObj.UpdateAggregates(m_odpContext, m_firstPassCellNonCustomAggs, updateAndSetup: false);
				}
			}
			InternalNextRow();
		}

		internal void PeerOuterGroupProcessCells()
		{
			Global.Tracer.Assert(!base.IsOuterGrouping && m_odpContext.PeerOuterGroupProcessing);
			DataScopeInfo dataScopeInfo = m_hierarchyDef.DataScopeInfo;
			if (dataScopeInfo != null && dataScopeInfo.DataSet != null && dataScopeInfo.DataSet.HasScopeWithCustomAggregates)
			{
				dataScopeInfo.ApplyGroupingFieldsForServerAggregates(m_odpContext.ReportObjectModel.FieldsImpl);
			}
			ProcessCells();
		}

		protected override void SendToInner()
		{
			RuntimeDataTablixGroupRootObjReference runtimeDataTablixGroupRootObjReference = (RuntimeDataTablixGroupRootObjReference)m_hierarchyRoot;
			if (base.IsOuterGrouping)
			{
				DataRegionDef.ResetOuterGroupingIndexesForOuterPeerGroup(runtimeDataTablixGroupRootObjReference.Value().HierarchyDef.HierarchyDynamicIndex);
				DataRegionDef.CurrentOuterGroupRoot = runtimeDataTablixGroupRootObjReference;
				DataRegionDef.UpdateOuterGroupingIndexes(runtimeDataTablixGroupRootObjReference, m_groupLeafIndex);
			}
			base.SendToInner();
			if (!DataRegionDef.IsMatrixIDC)
			{
				ProcessCells();
			}
			int num = (m_memberObjs != null) ? m_memberObjs.Length : 0;
			if (num == 0)
			{
				return;
			}
			bool peerOuterGroupProcessing = m_odpContext.PeerOuterGroupProcessing;
			AggregateRowInfo aggregateRowInfo = null;
			if (base.IsOuterGrouping || num > 1)
			{
				if (aggregateRowInfo == null)
				{
					aggregateRowInfo = new AggregateRowInfo();
				}
				aggregateRowInfo.SaveAggregateInfo(m_odpContext);
			}
			for (int i = 0; i < num; i++)
			{
				if (base.IsOuterGrouping)
				{
					if (i != 0)
					{
						m_odpContext.PeerOuterGroupProcessing = true;
					}
					DataRegionDef.SetDataTablixAggregateRowInfo(aggregateRowInfo);
				}
				IReference<RuntimeMemberObj> reference = m_memberObjs[i];
				using (reference.PinValue())
				{
					reference.Value().NextRow(base.IsOuterGrouping, m_odpContext);
				}
				aggregateRowInfo?.RestoreAggregateInfo(m_odpContext);
			}
			m_odpContext.PeerOuterGroupProcessing = peerOuterGroupProcessing;
		}

		internal RuntimeCell GetOrCreateCell(RuntimeDataTablixGroupLeafObj rowGroupLeaf)
		{
			Global.Tracer.Assert(!base.IsOuterGrouping, "(!IsOuterGrouping)");
			RuntimeCell result = null;
			if (m_cellsList != null)
			{
				IReference<RuntimeDataTablixGroupRootObj> reference = (IReference<RuntimeDataTablixGroupRootObj>)rowGroupLeaf.HierarchyRoot;
				RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = reference.Value();
				int hierarchyDynamicIndex = runtimeDataTablixGroupRootObj.HierarchyDef.HierarchyDynamicIndex;
				if (m_cellsList[hierarchyDynamicIndex] == null)
				{
					CreateRuntimeCells(runtimeDataTablixGroupRootObj);
				}
				RuntimeCells runtimeCells = m_cellsList[hierarchyDynamicIndex];
				if (runtimeCells != null)
				{
					result = runtimeCells.GetOrCreateCell(DataRegionDef, (RuntimeDataTablixGroupLeafObjReference)base.SelfReference, reference, rowGroupLeaf.GroupLeafIndex, out IDisposable _);
				}
			}
			return result;
		}

		private void ProcessCells()
		{
			if (m_cellsList == null)
			{
				return;
			}
			Global.Tracer.Assert(!base.IsOuterGrouping, "(!IsOuterGrouping)");
			if (!m_odpContext.PeerOuterGroupProcessing)
			{
				IReference<RuntimeDataTablixObj> ownerDataTablix = GetOwnerDataTablix();
				using (ownerDataTablix.PinValue())
				{
					RuntimeDataTablixObj runtimeDataTablixObj = ownerDataTablix.Value();
					if (runtimeDataTablixObj.InnerGroupsWithCellsForOuterPeerGroupProcessing != null)
					{
						runtimeDataTablixObj.InnerGroupsWithCellsForOuterPeerGroupProcessing.Add((RuntimeDataTablixGroupLeafObjReference)m_selfReference);
					}
				}
			}
			int[] outerGroupingIndexes = DataRegionDef.OuterGroupingIndexes;
			for (int i = 0; i < outerGroupingIndexes.Length; i++)
			{
				IReference<RuntimeDataTablixGroupRootObj> reference = DataRegionDef.CurrentOuterGroupRootObjs[i];
				if (reference == null)
				{
					continue;
				}
				RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = reference.Value();
				int hierarchyDynamicIndex = runtimeDataTablixGroupRootObj.HierarchyDef.HierarchyDynamicIndex;
				int num = outerGroupingIndexes[i];
				AggregateRowInfo aggregateRowInfo = AggregateRowInfo.CreateAndSaveAggregateInfo(m_odpContext);
				DataRegionDef.SetCellAggregateRowInfo(i, m_odpContext);
				if (m_cellsList[hierarchyDynamicIndex] == null)
				{
					CreateRuntimeCells(runtimeDataTablixGroupRootObj);
				}
				RuntimeCells runtimeCells = m_cellsList[hierarchyDynamicIndex];
				if (runtimeCells != null)
				{
					IDisposable cleanupRef;
					RuntimeCell andPinCell = runtimeCells.GetAndPinCell(num, out cleanupRef);
					if (andPinCell == null)
					{
						CreateCell(runtimeCells, num, runtimeDataTablixGroupRootObj.HierarchyDef, base.MemberDef, DataRegionDef);
						andPinCell = runtimeCells.GetAndPinCell(num, out cleanupRef);
					}
					andPinCell.NextRow();
					cleanupRef?.Dispose();
				}
				aggregateRowInfo.RestoreAggregateInfo(m_odpContext);
			}
		}

		internal override void GetScopeValues(IReference<IHierarchyObj> targetScopeObj, List<object>[] scopeValues, ref int index)
		{
			if (base.GroupRoot.Value().IsDetailGroup && (targetScopeObj == null || this != targetScopeObj.Value()))
			{
				DetailGetScopeValues(OuterScope, targetScopeObj, scopeValues, ref index);
			}
			else
			{
				base.GetScopeValues(targetScopeObj, scopeValues, ref index);
			}
		}

		internal override bool TargetScopeMatched(int index, bool detailSort)
		{
			if (base.GroupRoot.Value().IsDetailGroup)
			{
				return DetailTargetScopeMatched(base.MemberDef.DataRegionDef, OuterScope, base.MemberDef.IsColumn, index);
			}
			if (detailSort && base.GroupingDef.SortFilterScopeInfo == null)
			{
				return true;
			}
			if (m_targetScopeMatched != null && m_targetScopeMatched[index])
			{
				return true;
			}
			return false;
		}

		internal override bool SortAndFilter(AggregateUpdateContext aggContext)
		{
			SetupEnvironment();
			if (m_userSortTargetInfo != null)
			{
				m_userSortTargetInfo.EnterProcessUserSortPhase(m_odpContext);
			}
			AggregateUpdateQueue workQueue = null;
			if (m_odpContext.HasSecondPassOperation(SecondPassOperations.FilteringOrAggregatesOrDomainScope))
			{
				workQueue = RuntimeDataRegionObj.AggregateOfAggregatesStart(aggContext, this, m_hierarchyDef.DataScopeInfo, m_aggregatesOfAggregates, AggregateUpdateFlags.Both, needsSetupEnvironment: false);
			}
			bool result;
			using (m_hierarchyRoot.PinValue())
			{
				RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = (RuntimeDataTablixGroupRootObj)m_hierarchyRoot.Value();
				bool flag = false;
				int innerDomainScopeCount = runtimeDataTablixGroupRootObj.HierarchyDef.InnerDomainScopeCount;
				DomainScopeContext domainScopeContext = base.OdpContext.DomainScopeContext;
				if (m_odpContext.HasSecondPassOperation(SecondPassOperations.FilteringOrAggregatesOrDomainScope) && innerDomainScopeCount > 0)
				{
					domainScopeContext.AddDomainScopes(m_memberObjs, m_memberObjs.Length - innerDomainScopeCount);
				}
				TraverseStaticContents(ProcessingStages.SortAndFilter, aggContext);
				if (m_hasInnerHierarchy)
				{
					bool flag2 = true;
					for (int i = 0; i < m_memberObjs.Length; i++)
					{
						IReference<RuntimeMemberObj> reference = m_memberObjs[i];
						using (reference.PinValue())
						{
							RuntimeMemberObj runtimeMemberObj = reference.Value();
							if (runtimeMemberObj.SortAndFilter(aggContext))
							{
								flag2 = false;
							}
							else if (null == runtimeMemberObj.GroupRoot)
							{
								flag2 = false;
							}
							else if (runtimeMemberObj.GroupRoot.Value().HierarchyDef.InnerStaticMembersInSameScope != null)
							{
								flag2 = false;
							}
						}
					}
					if (flag2)
					{
						Global.Tracer.Assert((SecondPassOperations.FilteringOrAggregatesOrDomainScope & m_odpContext.SecondPassOperation) != 0, "(0 != (SecondPassOperations.Filtering & m_odpContext.SecondPassOperation))");
						Global.Tracer.Assert(runtimeDataTablixGroupRootObj.GroupFilters != null, "(null != groupRoot.GroupFilters)");
						runtimeDataTablixGroupRootObj.GroupFilters.FailFilters = true;
						flag = true;
					}
				}
				if (m_odpContext.HasSecondPassOperation(SecondPassOperations.FilteringOrAggregatesOrDomainScope))
				{
					RuntimeDataRegionObj.AggregatesOfAggregatesEnd(this, aggContext, workQueue, m_hierarchyDef.DataScopeInfo, m_aggregatesOfAggregates, updateAggsIfNeeded: false);
				}
				result = base.SortAndFilter(aggContext);
				if (m_odpContext.HasSecondPassOperation(SecondPassOperations.FilteringOrAggregatesOrDomainScope) && innerDomainScopeCount > 0)
				{
					domainScopeContext.RemoveDomainScopes(m_memberObjs, m_memberObjs.Length - innerDomainScopeCount);
				}
				if (flag)
				{
					runtimeDataTablixGroupRootObj.GroupFilters.FailFilters = false;
				}
			}
			if (m_userSortTargetInfo != null)
			{
				m_userSortTargetInfo.LeaveProcessUserSortPhase(m_odpContext);
			}
			return result;
		}

		internal override void PostFilterNextRow(AggregateUpdateContext context)
		{
			AggregateUpdateQueue workQueue = null;
			if (m_odpContext.HasSecondPassOperation(SecondPassOperations.FilteringOrAggregatesOrDomainScope))
			{
				workQueue = RuntimeDataRegionObj.AggregateOfAggregatesStart(context, this, m_hierarchyDef.DataScopeInfo, m_aggregatesOfAggregates, AggregateUpdateFlags.None, needsSetupEnvironment: false);
			}
			TraverseCellList(ProcessingStages.SortAndFilter, context);
			if (m_odpContext.HasSecondPassOperation(SecondPassOperations.FilteringOrAggregatesOrDomainScope))
			{
				RuntimeDataRegionObj.AggregatesOfAggregatesEnd(this, context, workQueue, m_hierarchyDef.DataScopeInfo, m_aggregatesOfAggregates, updateAggsIfNeeded: true);
			}
			base.PostFilterNextRow(context);
		}

		protected virtual void TraverseStaticContents(ProcessingStages operation, AggregateUpdateContext context)
		{
		}

		private void TraverseCellList(ProcessingStages operation, AggregateUpdateContext aggContext)
		{
			if (m_cellsList == null)
			{
				return;
			}
			for (int i = 0; i < m_cellsList.Length; i++)
			{
				if (m_cellsList[i] != null)
				{
					switch (operation)
					{
					case ProcessingStages.SortAndFilter:
						m_cellsList[i].SortAndFilter(aggContext);
						break;
					case ProcessingStages.UpdateAggregates:
						m_cellsList[i].UpdateAggregates(aggContext);
						break;
					default:
						Global.Tracer.Assert(condition: false, "Unknown operation in TraverseCellList");
						break;
					}
				}
			}
		}

		public override void UpdateAggregates(AggregateUpdateContext aggContext)
		{
			SetupEnvironment();
			if (!RuntimeDataRegionObj.UpdateAggregatesAtScope(aggContext, this, m_hierarchyDef.DataScopeInfo, AggregateUpdateFlags.Both, needsSetupEnvironment: false))
			{
				return;
			}
			TraverseStaticContents(ProcessingStages.UpdateAggregates, aggContext);
			if (m_hasInnerHierarchy)
			{
				for (int i = 0; i < m_memberObjs.Length; i++)
				{
					IReference<RuntimeMemberObj> reference = m_memberObjs[i];
					using (reference.PinValue())
					{
						reference.Value().UpdateAggregates(aggContext);
					}
				}
			}
			TraverseCellList(ProcessingStages.UpdateAggregates, aggContext);
		}

		protected static void CalculateInnerRunningValues(IReference<RuntimeMemberObj>[] memberObjs, Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
			if (memberObjs == null)
			{
				return;
			}
			foreach (IReference<RuntimeMemberObj> reference in memberObjs)
			{
				using (reference.PinValue())
				{
					reference.Value().CalculateRunningValues(groupCol, lastGroup, aggContext);
				}
			}
		}

		protected override void PrepareCalculateRunningValues()
		{
			m_processHeading = true;
			if (!m_hasInnerHierarchy)
			{
				return;
			}
			for (int i = 0; i < m_memberObjs.Length; i++)
			{
				IReference<RuntimeMemberObj> reference = m_memberObjs[i];
				using (reference.PinValue())
				{
					reference.Value().PrepareCalculateRunningValues();
				}
			}
		}

		internal override void CalculateRunningValues(AggregateUpdateContext aggContext)
		{
			SetupEnvironment();
			AggregateUpdateQueue workQueue = null;
			bool flag = false;
			DataActions dataActions = DataActions.PostSortAggregates;
			if (m_processHeading)
			{
				flag = FlagUtils.HasFlag(m_dataAction, DataActions.PostSortAggregates);
				workQueue = RuntimeDataRegionObj.AggregateOfAggregatesStart(aggContext, this, m_hierarchyDef.DataScopeInfo, m_postSortAggregatesOfAggregates, flag ? AggregateUpdateFlags.ScopedAggregates : AggregateUpdateFlags.Both, needsSetupEnvironment: false);
				if (flag && aggContext.LastScopeNeedsRowAggregateProcessing())
				{
					dataActions |= DataActions.PostSortAggregatesOfAggregates;
				}
			}
			bool isOuterGrouping = base.IsOuterGrouping;
			RuntimeDataTablixGroupRootObjReference runtimeDataTablixGroupRootObjReference = (RuntimeDataTablixGroupRootObjReference)m_hierarchyRoot;
			using (runtimeDataTablixGroupRootObjReference.PinValue())
			{
				RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = runtimeDataTablixGroupRootObjReference.Value();
				Dictionary<string, IReference<RuntimeGroupRootObj>> groupCollection = runtimeDataTablixGroupRootObj.GroupCollection;
				if (m_processHeading)
				{
					if (flag)
					{
						ReadRows(dataActions, aggContext);
						if (isOuterGrouping)
						{
							m_dataRows = null;
						}
					}
					CalculateInnerRunningValues(m_memberObjs, groupCollection, runtimeDataTablixGroupRootObjReference, aggContext);
				}
				else if (m_hasInnerHierarchy)
				{
					CalculateInnerRunningValues(m_memberObjs, groupCollection, runtimeDataTablixGroupRootObjReference, aggContext);
				}
				RuntimeGroupRootObjReference lastGroup = runtimeDataTablixGroupRootObjReference;
				if (isOuterGrouping)
				{
					if (!m_hasInnerHierarchy || HasInnerStaticMembersInSameScope)
					{
						DataRegionDef.CurrentOuterGroupRoot = runtimeDataTablixGroupRootObjReference;
						DataRegionDef.OuterGroupingIndexes[runtimeDataTablixGroupRootObj.HierarchyDef.HierarchyDynamicIndex] = m_groupLeafIndex;
						CalculateInnerRunningValues(runtimeDataTablixGroupRootObj.InnerGroupings, groupCollection, lastGroup, aggContext);
					}
				}
				else if (m_cellsList != null)
				{
					IReference<RuntimeDataTablixGroupRootObj> currentOuterGroupRoot = DataRegionDef.CurrentOuterGroupRoot;
					using (currentOuterGroupRoot.PinValue())
					{
						RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj2 = currentOuterGroupRoot.Value();
						int hierarchyDynamicIndex = runtimeDataTablixGroupRootObj2.HierarchyDef.HierarchyDynamicIndex;
						if (m_cellsList[hierarchyDynamicIndex] == null && m_odpContext.PeerOuterGroupProcessing)
						{
							CreateRuntimeCells(runtimeDataTablixGroupRootObj2);
						}
						RuntimeCells runtimeCells = m_cellsList[hierarchyDynamicIndex];
						if (runtimeCells != null)
						{
							DataRegionDef.ProcessCellRunningValues = true;
							runtimeCells.CalculateRunningValues(DataRegionDef, groupCollection, lastGroup, (RuntimeDataTablixGroupLeafObjReference)m_selfReference, aggContext);
							DataRegionDef.ProcessCellRunningValues = false;
						}
					}
				}
			}
			CalculateRunningValuesForStaticContents(aggContext);
			if (m_processHeading)
			{
				RuntimeDataRegionObj.AggregatesOfAggregatesEnd(this, aggContext, workQueue, m_hierarchyDef.DataScopeInfo, m_postSortAggregatesOfAggregates, updateAggsIfNeeded: true);
				if (m_odpContext.HasPreviousAggregates)
				{
					CalculatePreviousAggregates(isOuterGrouping);
				}
			}
			StoreCalculatedRunningValues();
		}

		protected virtual void CalculateRunningValuesForStaticContents(AggregateUpdateContext aggContext)
		{
		}

		protected void StoreCalculatedRunningValues()
		{
			if (m_processHeading)
			{
				using (m_hierarchyRoot.PinValue())
				{
					((RuntimeDataTablixGroupRootObj)m_hierarchyRoot.Value()).DoneReadingRows(ref m_runningValueValues, ref m_runningValueOfAggregateValues, ref m_cellRunningValueValues);
					m_processHeading = false;
				}
			}
			ResetScopedRunningValues();
		}

		protected override void ResetScopedRunningValues()
		{
			m_processHeading = false;
			base.ResetScopedRunningValues();
		}

		public override void ReadRow(DataActions dataAction, ITraversalContext context)
		{
			if (DataActions.UserSort == dataAction)
			{
				RuntimeDataRegionObj.CommonFirstRow(m_odpContext, ref m_firstRowIsAggregate, ref m_firstRow);
				CommonNextRow(m_dataRows);
			}
			else if (DataRegionDef.ProcessCellRunningValues)
			{
				if (FlagUtils.HasFlag(dataAction, DataActions.PostSortAggregates) && m_cellPostSortAggregates != null)
				{
					RuntimeDataRegionObj.UpdateAggregates(m_odpContext, m_cellPostSortAggregates, updateAndSetup: false);
				}
				using (m_hierarchyRoot.PinValue())
				{
					((IScope)m_hierarchyRoot.Value()).ReadRow(dataAction, context);
				}
			}
			else
			{
				base.ReadRow(dataAction, context);
			}
		}

		public override void SetupEnvironment()
		{
			base.SetupEnvironment();
			SetupAggregateValues(m_firstPassCellNonCustomAggs, m_firstPassCellCustomAggs);
		}

		private void SetupAggregateValues(List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> nonCustomAggCollection, List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> customAggCollection)
		{
			SetupAggregates(nonCustomAggCollection);
			SetupAggregates(customAggCollection);
		}

		protected virtual void CreateInstanceHeadingContents()
		{
		}

		internal override void CreateInstance(CreateInstancesTraversalContext traversalContext)
		{
			SetupEnvironment();
			RuntimeDataTablixGroupRootObjReference runtimeDataTablixGroupRootObjReference = (RuntimeDataTablixGroupRootObjReference)m_hierarchyRoot;
			using (runtimeDataTablixGroupRootObjReference.PinValue())
			{
				RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = runtimeDataTablixGroupRootObjReference.Value();
				ScopeInstance scopeInstance = traversalContext.ParentInstance;
				SetupRunningValues(base.MemberDef.RunningValues, m_runningValueValues);
				if (base.MemberDef.DataScopeInfo != null)
				{
					SetupRunningValues(base.MemberDef.DataScopeInfo.RunningValuesOfAggregates, m_runningValueOfAggregateValues);
				}
				if (m_targetScopeMatched != null)
				{
					base.MemberDef.Grouping.SortFilterScopeMatched = m_targetScopeMatched;
				}
				IReference<RuntimeDataTablixGroupRootObj> reference;
				if (base.IsOuterGrouping)
				{
					reference = runtimeDataTablixGroupRootObjReference;
					DataRegionDef.CurrentOuterGroupRoot = reference;
					DataRegionDef.UpdateOuterGroupingIndexes(runtimeDataTablixGroupRootObjReference, m_groupLeafIndex);
					DataRegionDef.NewOuterCells();
				}
				else
				{
					reference = DataRegionDef.CurrentOuterGroupRoot;
				}
				bool flag = false;
				if (m_sequentialMemberIndexWithinScopeLevel == -1)
				{
					m_sequentialMemberIndexWithinScopeLevel = DataRegionDef.AddMemberInstance(base.MemberDef.IsColumn);
					m_memberInstance = DataRegionMemberInstance.CreateInstance((IMemberHierarchy)scopeInstance, m_odpContext, base.MemberDef, m_firstRow.StreamOffset, m_sequentialMemberIndexWithinScopeLevel, m_recursiveLevel, runtimeDataTablixGroupRootObj.IsDetailGroup ? null : m_groupExprValues, m_variableValues, out m_instanceIndex);
					flag = true;
					if (runtimeDataTablixGroupRootObj.HasParent)
					{
						runtimeDataTablixGroupRootObj.SetRecursiveParentIndex(m_instanceIndex, m_recursiveLevel);
						if (m_recursiveLevel > 0)
						{
							m_memberInstance.RecursiveParentIndex = runtimeDataTablixGroupRootObj.GetRecursiveParentIndex(m_recursiveLevel - 1);
						}
						m_memberInstance.HasRecursiveChildren = (m_firstChild != null || m_grouping != null);
					}
					scopeInstance = m_memberInstance;
					CreateInstanceHeadingContents();
				}
				runtimeDataTablixGroupRootObj.CurrentMemberIndexWithinScopeLevel = m_sequentialMemberIndexWithinScopeLevel;
				runtimeDataTablixGroupRootObj.CurrentMemberInstance = m_memberInstance;
				if (m_memberObjs != null)
				{
					for (int i = 0; i < m_memberObjs.Length; i++)
					{
						IReference<RuntimeMemberObj> reference2 = m_memberObjs[i];
						using (reference2.PinValue())
						{
							reference2.Value().CreateInstances(base.SelfReference, m_odpContext, runtimeDataTablixGroupRootObj.DataRegionInstance, base.IsOuterGrouping, reference, scopeInstance, traversalContext.InnerMembers, traversalContext.InnerGroupLeafRef);
						}
					}
				}
				if (flag)
				{
					m_memberInstance.InstanceComplete();
					m_memberInstance = null;
				}
			}
		}

		internal void CreateInnerGroupingsOrCells(DataRegionInstance dataRegionInstance, ScopeInstance parentInstance, IReference<RuntimeDataTablixGroupRootObj> currOuterGroupRoot, IReference<RuntimeMemberObj>[] innerMembers, IReference<RuntimeDataTablixGroupLeafObj> innerGroupLeafRef)
		{
			SetupEnvironment();
			if (innerMembers != null)
			{
				innerGroupLeafRef = ((!base.IsOuterGrouping) ? ((IReference<RuntimeDataTablixGroupLeafObj>)base.SelfReference) : null);
				foreach (IReference<RuntimeMemberObj> reference in innerMembers)
				{
					using (reference.PinValue())
					{
						reference.Value().CreateInstances(base.SelfReference, m_odpContext, dataRegionInstance, !base.IsOuterGrouping, currOuterGroupRoot, dataRegionInstance, null, innerGroupLeafRef);
					}
				}
				return;
			}
			IDisposable disposable2 = null;
			RuntimeDataTablixGroupLeafObj runtimeDataTablixGroupLeafObj;
			if (base.IsOuterGrouping && innerGroupLeafRef != null)
			{
				disposable2 = innerGroupLeafRef.PinValue();
				runtimeDataTablixGroupLeafObj = innerGroupLeafRef.Value();
			}
			else
			{
				runtimeDataTablixGroupLeafObj = this;
			}
			if (currOuterGroupRoot != null)
			{
				runtimeDataTablixGroupLeafObj.CreateCellInstance(dataRegionInstance, currOuterGroupRoot);
			}
			else if (base.MemberDef.IsColumn)
			{
				runtimeDataTablixGroupLeafObj.CreateOutermostStatics(dataRegionInstance, m_sequentialMemberIndexWithinScopeLevel);
			}
			else
			{
				runtimeDataTablixGroupLeafObj.CreateOutermostStatics(m_memberInstance, 0);
			}
			disposable2?.Dispose();
		}

		protected virtual void CreateCellInstance(DataRegionInstance dataRegionInstance, IReference<RuntimeDataTablixGroupRootObj> currOuterGroupRoot)
		{
			SetupEnvironment();
			int hierarchyDynamicIndex = currOuterGroupRoot.Value().HierarchyDef.HierarchyDynamicIndex;
			if (m_cellsList[hierarchyDynamicIndex] == null)
			{
				CreateRuntimeCells(currOuterGroupRoot.Value());
			}
			Global.Tracer.Assert(m_cellsList[hierarchyDynamicIndex] != null, "(null != m_cellsList[index])");
			dataRegionInstance.DataRegionDef.AddCell();
			IDisposable cleanupRef;
			RuntimeCell orCreateCell = m_cellsList[hierarchyDynamicIndex].GetOrCreateCell(DataRegionDef, (RuntimeDataTablixGroupLeafObjReference)m_selfReference, currOuterGroupRoot, out cleanupRef);
			if (orCreateCell != null)
			{
				if (base.MemberDef.IsColumn)
				{
					orCreateCell.CreateInstance(currOuterGroupRoot.Value().CurrentMemberInstance, m_sequentialMemberIndexWithinScopeLevel);
				}
				else
				{
					orCreateCell.CreateInstance(m_memberInstance, currOuterGroupRoot.Value().CurrentMemberIndexWithinScopeLevel);
				}
				cleanupRef?.Dispose();
			}
		}

		internal void CreateStaticCells(DataRegionInstance dataRegionInstance, ScopeInstance parentInstance, IReference<RuntimeDataTablixGroupRootObj> currOuterGroupRoot, bool outerGroupings, List<int> staticLeafCellIndexes, IReference<RuntimeMemberObj>[] innerMembers, IReference<RuntimeDataTablixGroupLeafObj> innerGroupLeafRef)
		{
			if ((base.IsOuterGrouping && !outerGroupings) || (base.IsOuterGrouping && innerMembers == null && innerGroupLeafRef == null))
			{
				if (base.MemberDef.IsColumn)
				{
					CreateOutermostStatics(dataRegionInstance, m_sequentialMemberIndexWithinScopeLevel);
				}
				else
				{
					CreateOutermostStatics(m_memberInstance, 0);
				}
			}
			else
			{
				CreateInnerGroupingsOrCells(dataRegionInstance, parentInstance, currOuterGroupRoot, innerMembers, innerGroupLeafRef);
			}
		}

		protected void CreateOutermostStatics(IMemberHierarchy dataRegionOrRowMemberInstance, int columnMemberSequenceId)
		{
			SetupEnvironment();
			SetupRunningValues(base.MemberDef.RunningValues, m_runningValueValues);
			if (base.MemberDef.DataScopeInfo != null)
			{
				SetupRunningValues(base.MemberDef.DataScopeInfo.RunningValuesOfAggregates, m_runningValueOfAggregateValues);
			}
			using (m_hierarchyRoot.PinValue())
			{
				((RuntimeDataTablixGroupRootObj)m_hierarchyRoot.Value()).SetupCellRunningValues(m_cellRunningValueValues);
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = DataRegionDef;
			long firstRowOffset = (m_firstRow != null) ? m_firstRow.StreamOffset : 0;
			dataRegionDef.AddCell();
			List<int> outermostColumnIndexes = OutermostColumnIndexes;
			foreach (int outermostRowIndex in OutermostRowIndexes)
			{
				foreach (int item in outermostColumnIndexes)
				{
					Cell cell = dataRegionDef.Rows[outermostRowIndex].Cells[item];
					if (cell != null && !cell.SimpleGroupTreeCell)
					{
						DataCellInstance dataCellInstance = DataCellInstance.CreateInstance(dataRegionOrRowMemberInstance, m_odpContext, cell, firstRowOffset, columnMemberSequenceId);
						CreateOutermostStaticCellContents(cell, dataCellInstance);
						dataCellInstance.InstanceComplete();
					}
				}
			}
		}

		protected virtual void CreateOutermostStaticCellContents(Cell cell, DataCellInstance cellInstance)
		{
		}

		internal bool GetCellTargetForNonDetailSort()
		{
			return ((RuntimeDataTablixGroupRootObj)m_hierarchyRoot.Value()).GetCellTargetForNonDetailSort();
		}

		internal bool GetCellTargetForSort(int index, bool detailSort)
		{
			return ((RuntimeDataTablixGroupRootObj)m_hierarchyRoot.Value()).GetCellTargetForSort(index, detailSort);
		}

		internal bool NeedHandleCellSortFilterEvent()
		{
			if (base.GroupingDef.SortFilterScopeMatched == null)
			{
				return base.GroupingDef.NeedScopeInfoForSortFilterExpression != null;
			}
			return true;
		}

		internal IReference<RuntimeDataTablixObj> GetOwnerDataTablix()
		{
			IReference<IScope> outerScope = OuterScope;
			while (!(outerScope is RuntimeDataTablixObjReference))
			{
				outerScope = outerScope.Value().GetOuterScope(includeSubReportContainingScope: false);
			}
			Global.Tracer.Assert(outerScope is RuntimeDataTablixObjReference, "(outerScopeRef is RuntimeDataTablixObjReference)");
			return (RuntimeDataTablixObjReference)outerScope;
		}

		internal bool ReadStreamingModeIdcRowFromBufferOrDataSet(FieldsContext fieldsContext)
		{
			if (m_bufferIndex == -2)
			{
				return false;
			}
			m_bufferIndex++;
			if (m_bufferIndex >= m_dataRows.Count)
			{
				m_odpContext.StateManager.ProcessOneRow(base.GroupingDef.Owner);
				if (m_bufferIndex >= m_dataRows.Count)
				{
					m_bufferIndex = -2;
					return false;
				}
				m_dataRows[m_bufferIndex].SaveAggregateInfo(m_odpContext);
				return true;
			}
			m_dataRows[m_bufferIndex].RestoreDataSetAndSetFields(m_odpContext, fieldsContext);
			return true;
		}

		internal void PushBackStreamingModeIdcRowToBuffer()
		{
			if (m_bufferIndex != -2)
			{
				m_bufferIndex--;
			}
		}

		internal void ResetStreamingModeIdcRowBuffer()
		{
			m_bufferIndex = -1;
		}

		public IOnDemandMemberInstanceReference GetFirstMemberInstance(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode rifMember)
		{
			return RuntimeDataRegionObj.GetFirstMemberInstance(rifMember, m_memberObjs);
		}

		public IOnDemandMemberInstanceReference GetNextMemberInstance()
		{
			IOnDemandMemberInstanceReference result = null;
			if (m_nextLeaf != null)
			{
				result = (IOnDemandMemberInstanceReference)m_nextLeaf;
			}
			return result;
		}

		public IOnDemandScopeInstance GetCellInstance(IOnDemandMemberInstanceReference outerGroupInstanceRef, out IReference<IOnDemandScopeInstance> cellScopeRef)
		{
			RuntimeCell result = null;
			cellScopeRef = null;
			RuntimeDataTablixGroupLeafObj runtimeDataTablixGroupLeafObj = ((RuntimeDataTablixGroupLeafObjReference)outerGroupInstanceRef).Value();
			int hierarchyDynamicIndex = runtimeDataTablixGroupLeafObj.MemberDef.HierarchyDynamicIndex;
			RuntimeCells runtimeCells = m_cellsList[hierarchyDynamicIndex];
			if (runtimeCells != null)
			{
				result = runtimeCells.GetCell(runtimeDataTablixGroupLeafObj.GroupLeafIndex, out RuntimeCellReference cellRef);
				cellScopeRef = cellRef;
			}
			return result;
		}

		public IOnDemandMemberOwnerInstanceReference GetDataRegionInstance(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion)
		{
			return GetNestedDataRegion(rifDataRegion);
		}

		internal abstract RuntimeDataTablixObjReference GetNestedDataRegion(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion);

		public IReference<IDataCorrelation> GetIdcReceiver(IRIFReportDataScope scope)
		{
			if (scope.IsGroup)
			{
				return RuntimeDataRegionObj.GetGroupRoot(scope as Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode, m_memberObjs);
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion = scope as Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion;
			return GetNestedDataRegion(rifDataRegion);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MemberObjs:
					writer.Write(m_memberObjs);
					break;
				case MemberName.HasInnerHierarchy:
					writer.Write(m_hasInnerHierarchy);
					break;
				case MemberName.FirstPassCellNonCustomAggs:
					writer.Write(m_firstPassCellNonCustomAggs);
					break;
				case MemberName.FirstPassCellCustomAggs:
					writer.Write(m_firstPassCellCustomAggs);
					break;
				case MemberName.CellsList:
					writer.Write(m_cellsList);
					break;
				case MemberName.CellPostSortAggregates:
					writer.Write(m_cellPostSortAggregates);
					break;
				case MemberName.GroupLeafIndex:
					writer.Write(m_groupLeafIndex);
					break;
				case MemberName.ProcessHeading:
					writer.Write(m_processHeading);
					break;
				case MemberName.SequentialMemberIndexWithinScopeLevel:
					writer.Write(m_sequentialMemberIndexWithinScopeLevel);
					break;
				case MemberName.RunningValueValues:
					writer.Write(m_runningValueValues);
					break;
				case MemberName.RunningValueOfAggregateValues:
					writer.Write(m_runningValueOfAggregateValues);
					break;
				case MemberName.CellRunningValueValues:
					writer.Write(m_cellRunningValueValues);
					break;
				case MemberName.InstanceIndex:
					writer.Write(m_instanceIndex);
					break;
				case MemberName.ScopeInstanceNumber:
					writer.Write(m_scopeInstanceNumber);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.MemberObjs:
					m_memberObjs = reader.ReadArrayOfRIFObjects<IReference<RuntimeMemberObj>>();
					break;
				case MemberName.HasInnerHierarchy:
					m_hasInnerHierarchy = reader.ReadBoolean();
					break;
				case MemberName.FirstPassCellNonCustomAggs:
					m_firstPassCellNonCustomAggs = reader.ReadListOfRIFObjects<List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.FirstPassCellCustomAggs:
					m_firstPassCellCustomAggs = reader.ReadListOfRIFObjects<List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.CellsList:
					m_cellsList = reader.ReadArrayOfRIFObjects<RuntimeCells>();
					break;
				case MemberName.CellPostSortAggregates:
					m_cellPostSortAggregates = reader.ReadListOfRIFObjects<List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.GroupLeafIndex:
					m_groupLeafIndex = reader.ReadInt32();
					break;
				case MemberName.ProcessHeading:
					m_processHeading = reader.ReadBoolean();
					break;
				case MemberName.SequentialMemberIndexWithinScopeLevel:
					m_sequentialMemberIndexWithinScopeLevel = reader.ReadInt32();
					break;
				case MemberName.RunningValueValues:
					m_runningValueValues = reader.ReadArrayOfRIFObjects<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult>();
					break;
				case MemberName.RunningValueOfAggregateValues:
					m_runningValueOfAggregateValues = reader.ReadArrayOfRIFObjects<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult>();
					break;
				case MemberName.CellRunningValueValues:
					m_cellRunningValueValues = reader.ReadArrayOfRIFObjects<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult>();
					break;
				case MemberName.InstanceIndex:
					m_instanceIndex = reader.ReadInt32();
					break;
				case MemberName.ScopeInstanceNumber:
					m_scopeInstanceNumber = reader.ReadInt64();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.MemberObjs, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObjReference));
				list.Add(new MemberInfo(MemberName.HasInnerHierarchy, Token.Boolean));
				list.Add(new MemberInfo(MemberName.FirstPassCellNonCustomAggs, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.FirstPassCellCustomAggs, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.CellsList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCells));
				list.Add(new MemberInfo(MemberName.CellPostSortAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.GroupLeafIndex, Token.Int32));
				list.Add(new MemberInfo(MemberName.ProcessHeading, Token.Boolean));
				list.Add(new MemberInfo(MemberName.SequentialMemberIndexWithinScopeLevel, Token.Int32));
				list.Add(new MemberInfo(MemberName.RunningValueValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjResult));
				list.Add(new MemberInfo(MemberName.CellRunningValueValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjResult));
				list.Add(new MemberInfo(MemberName.InstanceIndex, Token.Int32));
				list.Add(new MemberInfo(MemberName.RunningValueOfAggregateValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjResult));
				list.Add(new MemberInfo(MemberName.ScopeInstanceNumber, Token.Int64));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObj, list);
			}
			return m_declaration;
		}
	}
}

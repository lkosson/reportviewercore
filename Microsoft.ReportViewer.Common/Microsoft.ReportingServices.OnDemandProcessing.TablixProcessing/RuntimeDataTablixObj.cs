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
	internal abstract class RuntimeDataTablixObj : RuntimeRDLDataRegionObj, IOnDemandMemberOwnerInstance, IOnDemandScopeInstance, IStorable, IPersistable
	{
		protected int[] m_outerGroupingCounters;

		protected IReference<RuntimeMemberObj>[] m_outerGroupings;

		protected IReference<RuntimeMemberObj>[] m_innerGroupings;

		protected List<IReference<RuntimeDataTablixGroupLeafObj>> m_innerGroupsWithCellsForOuterPeerGroupProcessing;

		private long m_scopeInstanceNumber;

		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		internal int[] OuterGroupingCounters => m_outerGroupingCounters;

		internal List<IReference<RuntimeDataTablixGroupLeafObj>> InnerGroupsWithCellsForOuterPeerGroupProcessing => m_innerGroupsWithCellsForOuterPeerGroupProcessing;

		public bool IsNoRows => m_firstRow == null;

		public bool IsMostRecentlyCreatedScopeInstance => m_dataRegionDef.DataScopeInfo.IsLastScopeInstanceNumber(m_scopeInstanceNumber);

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

		public override int Size => base.Size + ItemSizes.SizeOf(m_outerGroupingCounters) + ItemSizes.SizeOf(m_outerGroupings) + ItemSizes.SizeOf(m_innerGroupings) + ItemSizes.SizeOf(m_innerGroupsWithCellsForOuterPeerGroupProcessing) + 8;

		internal RuntimeDataTablixObj()
		{
		}

		internal RuntimeDataTablixObj(IReference<IScope> outerScope, Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataTablixDef, ref DataActions dataAction, OnDemandProcessingContext odpContext, bool onePassProcess, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(outerScope, dataTablixDef, ref dataAction, odpContext, onePassProcess, dataTablixDef.RunningValues, objectType, outerScope.Value().Depth + 1)
		{
			ConstructorHelper(ref dataAction, onePassProcess, out bool handleMyDataAction, out DataActions innerDataAction);
			m_innerDataAction = innerDataAction;
			DataActions userSortDataAction = HandleSortFilterEvent();
			ConstructRuntimeStructure(ref innerDataAction, onePassProcess);
			HandleDataAction(handleMyDataAction, innerDataAction, userSortDataAction);
			m_odpContext.CreatedScopeInstance(m_dataRegionDef);
			m_scopeInstanceNumber = RuntimeDataRegionObj.AssignScopeInstanceNumber(m_dataRegionDef.DataScopeInfo);
		}

		protected void ConstructorHelper(ref DataActions dataAction, bool onePassProcess, out bool handleMyDataAction, out DataActions innerDataAction)
		{
			innerDataAction = m_dataAction;
			handleMyDataAction = false;
			if (onePassProcess)
			{
				if (m_dataRegionDef.RunningValues != null && 0 < m_dataRegionDef.RunningValues.Count)
				{
					RuntimeDataRegionObj.CreateAggregates(m_odpContext, m_dataRegionDef.RunningValues, ref m_nonCustomAggregates);
				}
				RuntimeDataRegionObj.CreateAggregates(m_odpContext, m_dataRegionDef.PostSortAggregates, ref m_nonCustomAggregates);
				RuntimeDataRegionObj.CreateAggregates(m_odpContext, m_dataRegionDef.CellPostSortAggregates, ref m_nonCustomAggregates);
			}
			else
			{
				if (m_dataRegionDef.RunningValues != null && 0 < m_dataRegionDef.RunningValues.Count)
				{
					m_dataAction |= DataActions.PostSortAggregates;
				}
				if (m_dataRegionDef.PostSortAggregates != null && m_dataRegionDef.PostSortAggregates.Count != 0)
				{
					RuntimeDataRegionObj.CreateAggregates(m_odpContext, m_dataRegionDef.PostSortAggregates, ref m_postSortAggregates);
					m_dataAction |= DataActions.PostSortAggregates;
					handleMyDataAction = true;
				}
				if (m_dataRegionDef.DataScopeInfo != null)
				{
					DataScopeInfo dataScopeInfo = m_dataRegionDef.DataScopeInfo;
					if (dataScopeInfo.PostSortAggregatesOfAggregates != null && !dataScopeInfo.PostSortAggregatesOfAggregates.IsEmpty)
					{
						RuntimeDataRegionObj.CreateAggregates(m_odpContext, dataScopeInfo.PostSortAggregatesOfAggregates, ref m_postSortAggregatesOfAggregates);
					}
					if (dataScopeInfo.HasAggregatesToUpdateAtRowScope)
					{
						m_dataAction |= DataActions.AggregatesOfAggregates;
						handleMyDataAction = true;
					}
				}
				if (handleMyDataAction)
				{
					innerDataAction = DataActions.None;
				}
				else
				{
					innerDataAction = m_dataAction;
				}
			}
			m_inDataRowSortPhase = (m_dataRegionDef.Sorting != null && m_dataRegionDef.Sorting.ShouldApplySorting);
			if (m_inDataRowSortPhase)
			{
				m_sortedDataRowTree = new BTree(this, m_odpContext, m_depth);
				m_dataRowSortExpression = new RuntimeExpressionInfo(m_dataRegionDef.Sorting.SortExpressions, m_dataRegionDef.Sorting.ExprHost, m_dataRegionDef.Sorting.SortDirections, 0);
				m_odpContext.AddSpecialDataRowSort((IReference<IDataRowSortOwner>)base.SelfReference);
			}
			m_dataRegionDef.ResetInstanceIndexes();
			m_outerGroupingCounters = new int[m_dataRegionDef.OuterGroupingDynamicMemberCount];
			for (int i = 0; i < m_outerGroupingCounters.Length; i++)
			{
				m_outerGroupingCounters[i] = -1;
			}
		}

		protected override void ConstructRuntimeStructure(ref DataActions innerDataAction, bool onePassProcess)
		{
			CreateRuntimeMemberObjects(m_dataRegionDef.OuterMembers, m_dataRegionDef.InnerMembers, out HierarchyNodeList outerTopLevelStaticMembers, out HierarchyNodeList innerTopLevelStaticMembers, ref innerDataAction);
			if (((outerTopLevelStaticMembers != null && outerTopLevelStaticMembers.Count != 0) || (innerTopLevelStaticMembers != null && innerTopLevelStaticMembers.Count != 0)) && base.DataRegionDef.OutermostStaticColumnIndexes == null && base.DataRegionDef.OutermostStaticRowIndexes == null)
			{
				List<int> list = innerTopLevelStaticMembers?.LeafCellIndexes;
				List<int> list2 = outerTopLevelStaticMembers?.LeafCellIndexes;
				if (base.DataRegionDef.ProcessingInnerGrouping == Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion.ProcessingInnerGroupings.Column)
				{
					base.DataRegionDef.OutermostStaticColumnIndexes = list;
					base.DataRegionDef.OutermostStaticRowIndexes = list2;
				}
				else
				{
					base.DataRegionDef.OutermostStaticColumnIndexes = list2;
					base.DataRegionDef.OutermostStaticRowIndexes = list;
				}
			}
		}

		private void CreateRuntimeMemberObjects(HierarchyNodeList outerMembers, HierarchyNodeList innerMembers, out HierarchyNodeList outerTopLevelStaticMembers, out HierarchyNodeList innerTopLevelStaticMembers, ref DataActions innerDataAction)
		{
			bool hasOppositeStaticLeafMembers = false;
			HierarchyNodeList topLevelDynamicMembers = null;
			outerTopLevelStaticMembers = null;
			if (outerMembers != null)
			{
				hasOppositeStaticLeafMembers = outerMembers.HasStaticLeafMembers;
				topLevelDynamicMembers = outerMembers.DynamicMembersAtScope;
				outerTopLevelStaticMembers = outerMembers.StaticMembersInSameScope;
			}
			bool hasOppositeStaticLeafMembers2 = false;
			HierarchyNodeList topLevelDynamicMembers2 = null;
			innerTopLevelStaticMembers = null;
			if (innerMembers != null)
			{
				hasOppositeStaticLeafMembers2 = innerMembers.HasStaticLeafMembers;
				topLevelDynamicMembers2 = innerMembers.DynamicMembersAtScope;
				innerTopLevelStaticMembers = innerMembers.StaticMembersInSameScope;
			}
			DataActions groupingDataAction = DataActions.None;
			CreateTopLevelRuntimeGroupings(ref groupingDataAction, ref m_innerGroupings, innerTopLevelStaticMembers, topLevelDynamicMembers2, null, hasOppositeStaticLeafMembers);
			Global.Tracer.Assert(m_innerGroupings != null && m_innerGroupings.Length >= 1, "(null != m_innerGroupings && m_innerGroupings.Length >= 1)");
			CreateTopLevelRuntimeGroupings(ref innerDataAction, ref m_outerGroupings, outerTopLevelStaticMembers, topLevelDynamicMembers, m_innerGroupings, hasOppositeStaticLeafMembers2);
		}

		private void CreateTopLevelRuntimeGroupings(ref DataActions groupingDataAction, ref IReference<RuntimeMemberObj>[] groupings, HierarchyNodeList topLevelStaticMembers, HierarchyNodeList topLevelDynamicMembers, IReference<RuntimeMemberObj>[] innerGroupings, bool hasOppositeStaticLeafMembers)
		{
			int num = topLevelDynamicMembers?.Count ?? 0;
			groupings = new IReference<RuntimeMemberObj>[Math.Max(1, num)];
			if (num == 0)
			{
				IReference<RuntimeMemberObj> reference = RuntimeDataTablixMemberObj.CreateRuntimeMemberObject(m_selfReference, null, ref groupingDataAction, m_odpContext, innerGroupings, topLevelStaticMembers, hasOppositeStaticLeafMembers, 0, base.ObjectType);
				groupings[0] = reference;
				return;
			}
			for (int i = 0; i < num; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode dynamicMemberDef = topLevelDynamicMembers[i];
				IReference<RuntimeMemberObj> reference2 = RuntimeDataTablixMemberObj.CreateRuntimeMemberObject(m_selfReference, dynamicMemberDef, ref groupingDataAction, m_odpContext, innerGroupings, (i == 0) ? topLevelStaticMembers : null, hasOppositeStaticLeafMembers, 0, base.ObjectType);
				groupings[i] = reference2;
			}
		}

		protected void HandleDataAction(bool handleMyDataAction, DataActions innerDataAction, DataActions userSortDataAction)
		{
			if (!handleMyDataAction)
			{
				m_dataAction = innerDataAction;
			}
			m_dataAction |= userSortDataAction;
			if (m_dataAction != 0)
			{
				m_dataRows = new ScalableList<DataFieldRow>(m_depth + 1, m_odpContext.TablixProcessingScalabilityCache, 30);
			}
		}

		protected override void SendToInner()
		{
			bool peerOuterGroupProcessing = m_odpContext.PeerOuterGroupProcessing;
			m_dataRegionDef.RuntimeDataRegionObj = m_selfReference;
			int num = (m_outerGroupings != null) ? m_outerGroupings.Length : 0;
			AggregateRowInfo aggregateRowInfo = AggregateRowInfo.CreateAndSaveAggregateInfo(m_odpContext);
			if (m_dataRegionDef.IsMatrixIDC)
			{
				if (m_innerGroupings != null)
				{
					ProcessInnerHierarchy(aggregateRowInfo);
				}
				for (int i = 0; i < num; i++)
				{
					ProcessOuterHierarchy(aggregateRowInfo, i);
				}
				return;
			}
			if (num == 0)
			{
				if (m_innerGroupings != null)
				{
					ProcessInnerHierarchy(aggregateRowInfo);
				}
			}
			else
			{
				if (m_innerGroupsWithCellsForOuterPeerGroupProcessing == null || !peerOuterGroupProcessing)
				{
					m_innerGroupsWithCellsForOuterPeerGroupProcessing = new List<IReference<RuntimeDataTablixGroupLeafObj>>();
				}
				for (int j = 0; j < num; j++)
				{
					ProcessOuterHierarchy(aggregateRowInfo, j);
					if (m_innerGroupings == null)
					{
						continue;
					}
					if (j == 0)
					{
						ProcessInnerHierarchy(aggregateRowInfo);
						continue;
					}
					foreach (IReference<RuntimeDataTablixGroupLeafObj> item in m_innerGroupsWithCellsForOuterPeerGroupProcessing)
					{
						using (item.PinValue())
						{
							item.Value().PeerOuterGroupProcessCells();
						}
						aggregateRowInfo.RestoreAggregateInfo(m_odpContext);
					}
				}
			}
			m_odpContext.PeerOuterGroupProcessing = peerOuterGroupProcessing;
		}

		private void ProcessInnerHierarchy(AggregateRowInfo aggregateRowInfo)
		{
			for (int i = 0; i < m_innerGroupings.Length; i++)
			{
				IReference<RuntimeMemberObj> reference = m_innerGroupings[i];
				using (reference.PinValue())
				{
					reference.Value().NextRow(isOuterGrouping: false, m_odpContext);
				}
				aggregateRowInfo.RestoreAggregateInfo(m_odpContext);
			}
		}

		private void ProcessOuterHierarchy(AggregateRowInfo aggregateRowInfo, int outerGroupingIndex)
		{
			m_odpContext.PeerOuterGroupProcessing = (outerGroupingIndex != 0);
			m_dataRegionDef.ResetOuterGroupingIndexesForOuterPeerGroup(0);
			m_dataRegionDef.ResetOuterGroupingAggregateRowInfo();
			m_dataRegionDef.SetDataTablixAggregateRowInfo(aggregateRowInfo);
			IReference<RuntimeMemberObj> reference = m_outerGroupings[outerGroupingIndex];
			using (reference.PinValue())
			{
				reference.Value().NextRow(isOuterGrouping: true, m_odpContext);
			}
			aggregateRowInfo.RestoreAggregateInfo(m_odpContext);
		}

		internal override bool SortAndFilter(AggregateUpdateContext aggContext)
		{
			SetupEnvironment();
			if (m_userSortTargetInfo != null)
			{
				m_userSortTargetInfo.EnterProcessUserSortPhase(m_odpContext);
			}
			bool num = base.DataRegionDef.ProcessingInnerGrouping == Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion.ProcessingInnerGroupings.Column;
			IReference<RuntimeMemberObj>[] array = num ? m_outerGroupings : m_innerGroupings;
			IReference<RuntimeMemberObj>[] array2 = num ? m_innerGroupings : m_outerGroupings;
			int rowDomainScopeCount = base.DataRegionDef.RowDomainScopeCount;
			int columnDomainScopeCount = base.DataRegionDef.ColumnDomainScopeCount;
			DomainScopeContext domainScopeContext = base.OdpContext.DomainScopeContext;
			AggregateUpdateQueue workQueue = null;
			if (m_odpContext.HasSecondPassOperation(SecondPassOperations.FilteringOrAggregatesOrDomainScope))
			{
				workQueue = RuntimeDataRegionObj.AggregateOfAggregatesStart(aggContext, this, m_dataRegionDef.DataScopeInfo, m_aggregatesOfAggregates, AggregateUpdateFlags.Both, needsSetupEnvironment: false);
				if (rowDomainScopeCount > 0)
				{
					domainScopeContext.AddDomainScopes(array, array.Length - rowDomainScopeCount);
				}
				if (columnDomainScopeCount > 0)
				{
					domainScopeContext.AddDomainScopes(array2, array2.Length - columnDomainScopeCount);
				}
			}
			Traverse(ProcessingStages.SortAndFilter, aggContext);
			base.SortAndFilter(aggContext);
			if (m_odpContext.HasSecondPassOperation(SecondPassOperations.FilteringOrAggregatesOrDomainScope))
			{
				RuntimeDataRegionObj.AggregatesOfAggregatesEnd(this, aggContext, workQueue, m_dataRegionDef.DataScopeInfo, m_aggregatesOfAggregates, updateAggsIfNeeded: true);
				if (rowDomainScopeCount > 0)
				{
					domainScopeContext.RemoveDomainScopes(array, array.Length - rowDomainScopeCount);
				}
				if (columnDomainScopeCount > 0)
				{
					domainScopeContext.RemoveDomainScopes(array2, array2.Length - columnDomainScopeCount);
				}
			}
			if (m_userSortTargetInfo != null)
			{
				m_userSortTargetInfo.LeaveProcessUserSortPhase(m_odpContext);
			}
			return true;
		}

		public override void UpdateAggregates(AggregateUpdateContext context)
		{
			SetupEnvironment();
			if (RuntimeDataRegionObj.UpdateAggregatesAtScope(context, this, m_dataRegionDef.DataScopeInfo, AggregateUpdateFlags.Both, needsSetupEnvironment: false))
			{
				Traverse(ProcessingStages.UpdateAggregates, context);
			}
		}

		protected virtual void Traverse(ProcessingStages operation, ITraversalContext context)
		{
			bool num = base.DataRegionDef.ProcessingInnerGrouping == Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion.ProcessingInnerGroupings.Column;
			IReference<RuntimeMemberObj>[] array = num ? m_outerGroupings : m_innerGroupings;
			IReference<RuntimeMemberObj>[] array2 = num ? m_innerGroupings : m_outerGroupings;
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					TraverseMember(array[i], operation, context);
				}
			}
			if (array2 != null)
			{
				for (int j = 0; j < array2.Length; j++)
				{
					TraverseMember(array2[j], operation, context);
				}
			}
		}

		private void TraverseMember(IReference<RuntimeMemberObj> memberRef, ProcessingStages operation, ITraversalContext context)
		{
			using (memberRef.PinValue())
			{
				switch (operation)
				{
				case ProcessingStages.SortAndFilter:
					memberRef.Value().SortAndFilter((AggregateUpdateContext)context);
					break;
				case ProcessingStages.UpdateAggregates:
					memberRef.Value().UpdateAggregates((AggregateUpdateContext)context);
					break;
				default:
					Global.Tracer.Assert(condition: false, "Unknown ProcessingStage in TraverseMember");
					break;
				}
			}
		}

		internal override void CalculateRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
			if (m_dataRegionDef.RunningValues != null && m_runningValues == null && m_previousValues == null)
			{
				AddRunningValues(m_odpContext, m_dataRegionDef.RunningValues, ref m_runningValues, ref m_previousValues, groupCol, lastGroup);
			}
			if (m_dataRegionDef.DataScopeInfo != null)
			{
				List<string> runningValuesInGroup = null;
				List<string> previousValuesInGroup = null;
				AddRunningValues(m_odpContext, m_dataRegionDef.DataScopeInfo.RunningValuesOfAggregates, ref runningValuesInGroup, ref previousValuesInGroup, groupCol, lastGroup);
			}
			bool flag = m_dataRows != null && FlagUtils.HasFlag(m_dataAction, DataActions.PostSortAggregates);
			AggregateUpdateQueue workQueue = RuntimeDataRegionObj.AggregateOfAggregatesStart(aggContext, this, m_dataRegionDef.DataScopeInfo, m_postSortAggregatesOfAggregates, flag ? AggregateUpdateFlags.ScopedAggregates : AggregateUpdateFlags.Both, needsSetupEnvironment: true);
			if (flag)
			{
				DataActions dataActions = DataActions.PostSortAggregates;
				if (aggContext.LastScopeNeedsRowAggregateProcessing())
				{
					dataActions |= DataActions.PostSortAggregatesOfAggregates;
				}
				ReadRows(dataActions, aggContext);
				m_dataRows = null;
			}
			int num = (m_outerGroupings != null) ? m_outerGroupings.Length : 0;
			if (num == 0)
			{
				if (m_innerGroupings != null)
				{
					for (int i = 0; i < m_innerGroupings.Length; i++)
					{
						IReference<RuntimeMemberObj> reference = m_innerGroupings[i];
						using (reference.PinValue())
						{
							reference.Value().CalculateRunningValues(groupCol, lastGroup, aggContext);
						}
					}
				}
			}
			else
			{
				for (int j = 0; j < num; j++)
				{
					IReference<RuntimeMemberObj> reference2 = m_outerGroupings[j];
					bool flag2;
					using (reference2.PinValue())
					{
						RuntimeMemberObj runtimeMemberObj = reference2.Value();
						runtimeMemberObj.CalculateRunningValues(groupCol, lastGroup, aggContext);
						flag2 = (runtimeMemberObj.GroupRoot == null);
					}
					if (!flag2 || m_innerGroupings == null)
					{
						continue;
					}
					for (int k = 0; k < m_innerGroupings.Length; k++)
					{
						IReference<RuntimeMemberObj> reference3 = m_innerGroupings[k];
						using (reference3.PinValue())
						{
							RuntimeMemberObj runtimeMemberObj2 = reference3.Value();
							runtimeMemberObj2.PrepareCalculateRunningValues();
							runtimeMemberObj2.CalculateRunningValues(groupCol, lastGroup, aggContext);
						}
					}
				}
			}
			CalculateRunningValuesForTopLevelStaticContents(groupCol, lastGroup, aggContext);
			RuntimeDataRegionObj.AggregatesOfAggregatesEnd(this, aggContext, workQueue, m_dataRegionDef.DataScopeInfo, m_postSortAggregatesOfAggregates, updateAggsIfNeeded: true);
			CalculateDRPreviousAggregates();
			RuntimeRICollection.StoreRunningValues(m_odpContext.ReportObjectModel.AggregatesImpl, m_dataRegionDef.RunningValues, ref m_runningValueValues);
			if (m_dataRegionDef.DataScopeInfo != null)
			{
				RuntimeRICollection.StoreRunningValues(m_odpContext.ReportObjectModel.AggregatesImpl, m_dataRegionDef.DataScopeInfo.RunningValuesOfAggregates, ref m_runningValueOfAggregateValues);
			}
		}

		protected virtual void CalculateRunningValuesForTopLevelStaticContents(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
		}

		internal static void AddRunningValues(OnDemandProcessingContext odpContext, List<Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo> runningValues, ref List<string> runningValuesInGroup, ref List<string> previousValuesInGroup, Dictionary<string, IReference<RuntimeGroupRootObj>> groupCollection, IReference<RuntimeGroupRootObj> lastGroup)
		{
			if (runningValues == null || 0 >= runningValues.Count)
			{
				return;
			}
			if (runningValuesInGroup == null)
			{
				runningValuesInGroup = new List<string>();
			}
			if (previousValuesInGroup == null)
			{
				previousValuesInGroup = new List<string>();
			}
			AggregatesImpl aggregatesImpl = odpContext.ReportObjectModel.AggregatesImpl;
			for (int i = 0; i < runningValues.Count; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValueInfo = runningValues[i];
				Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj dataAggregateObj = aggregatesImpl.GetAggregateObj(runningValueInfo.Name);
				if (dataAggregateObj == null)
				{
					dataAggregateObj = new Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj(runningValueInfo, odpContext);
					aggregatesImpl.Add(dataAggregateObj);
				}
				if (runningValueInfo.Scope != null)
				{
					if (groupCollection.TryGetValue(runningValueInfo.Scope, out IReference<RuntimeGroupRootObj> value))
					{
						using (value.PinValue())
						{
							value.Value().AddScopedRunningValue(dataAggregateObj);
						}
					}
					else
					{
						Global.Tracer.Assert(condition: false);
					}
				}
				if (runningValueInfo.AggregateType == Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Previous)
				{
					previousValuesInGroup.Add(dataAggregateObj.Name);
				}
				else
				{
					runningValuesInGroup.Add(dataAggregateObj.Name);
				}
			}
		}

		internal static void UpdateRunningValues(OnDemandProcessingContext odpContext, List<string> runningValueNames)
		{
			AggregatesImpl aggregatesImpl = odpContext.ReportObjectModel.AggregatesImpl;
			for (int i = 0; i < runningValueNames.Count; i++)
			{
				string name = runningValueNames[i];
				aggregatesImpl.GetAggregateObj(name).Update();
			}
		}

		internal static void SaveData(ScalableList<DataFieldRow> dataRows, OnDemandProcessingContext odpContext)
		{
			Global.Tracer.Assert(dataRows != null, "(null != dataRows)");
			dataRows.Add(SaveData(odpContext));
		}

		internal static DataFieldRow SaveData(OnDemandProcessingContext odpContext)
		{
			return new DataFieldRow(odpContext.ReportObjectModel.FieldsImpl, getAndSave: true);
		}

		internal override void CalculatePreviousAggregates()
		{
			if (m_outerScope != null && (DataActions.PostSortAggregates & m_outerDataAction) != 0)
			{
				using (m_outerScope.PinValue())
				{
					m_outerScope.Value().CalculatePreviousAggregates();
				}
			}
		}

		private void CalculateDRPreviousAggregates()
		{
			SetupEnvironment();
			if (m_previousValues != null)
			{
				AggregatesImpl aggregatesImpl = m_odpContext.ReportObjectModel.AggregatesImpl;
				for (int i = 0; i < m_previousValues.Count; i++)
				{
					string text = m_previousValues[i];
					Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = aggregatesImpl.GetAggregateObj(text);
					Global.Tracer.Assert(aggregateObj != null, "Missing expected previous aggregate: {0}", text);
					aggregateObj.Update();
				}
			}
		}

		public override void ReadRow(DataActions dataAction, ITraversalContext context)
		{
			if (DataActions.UserSort == dataAction)
			{
				RuntimeDataRegionObj.CommonFirstRow(m_odpContext, ref m_firstRowIsAggregate, ref m_firstRow);
				CommonNextRow(m_dataRows);
				return;
			}
			if (DataActions.AggregatesOfAggregates == dataAction)
			{
				((AggregateUpdateContext)context).UpdateAggregatesForRow();
				return;
			}
			if (FlagUtils.HasFlag(dataAction, DataActions.PostSortAggregatesOfAggregates))
			{
				((AggregateUpdateContext)context).UpdateAggregatesForRow();
			}
			if (m_dataRegionDef.ProcessCellRunningValues)
			{
				return;
			}
			if (FlagUtils.HasFlag(dataAction, DataActions.PostSortAggregates))
			{
				if (m_postSortAggregates != null)
				{
					RuntimeDataRegionObj.UpdateAggregates(m_odpContext, m_postSortAggregates, updateAndSetup: false);
				}
				if (m_runningValues != null)
				{
					UpdateRunningValues(m_odpContext, m_runningValues);
				}
			}
			if (m_outerScope != null && (dataAction & m_outerDataAction) != 0)
			{
				using (m_outerScope.PinValue())
				{
					m_outerScope.Value().ReadRow(dataAction, context);
				}
			}
		}

		public override void SetupEnvironment()
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = m_dataRegionDef.GetDataSet(m_odpContext.ReportDefinition);
			SetupNewDataSet(dataSet);
			m_odpContext.ReportRuntime.CurrentScope = this;
			SetupEnvironment(m_dataRegionDef.RunningValues);
		}

		internal void CreateOutermostStaticCells(DataRegionInstance dataRegionInstance, bool outerGroupings, IReference<RuntimeMemberObj>[] innerMembers, IReference<RuntimeDataTablixGroupLeafObj> innerGroupLeafRef)
		{
			if (innerMembers != null)
			{
				SetupEnvironment();
				foreach (IReference<RuntimeMemberObj> reference in innerMembers)
				{
					using (reference.PinValue())
					{
						reference.Value().CreateInstances(base.SelfReference, m_odpContext, dataRegionInstance, !outerGroupings, null, dataRegionInstance, null, innerGroupLeafRef);
					}
				}
			}
			else if (base.DataRegionDef.OutermostStaticRowIndexes != null && base.DataRegionDef.OutermostStaticColumnIndexes != null)
			{
				m_dataRegionDef.AddCell();
			}
		}

		private bool OutermostSTCellTargetScopeMatched(int index, IReference<RuntimeSortFilterEventInfo> sortFilterInfo)
		{
			return true;
		}

		internal override bool TargetScopeMatched(int index, bool detailSort)
		{
			if (m_dataRegionDef.InOutermostStaticCells && !OutermostSTCellTargetScopeMatched(index, m_odpContext.RuntimeSortFilterInfo[index]))
			{
				return false;
			}
			return base.TargetScopeMatched(index, detailSort);
		}

		internal void CreateInstances(DataRegionInstance dataRegionInstance)
		{
			m_dataRegionDef.ResetInstanceIndexes();
			m_innerGroupsWithCellsForOuterPeerGroupProcessing = null;
			m_dataRegionDef.CurrentDataRegionInstance = dataRegionInstance;
			dataRegionInstance.StoreAggregates(m_odpContext, m_dataRegionDef.Aggregates);
			dataRegionInstance.StoreAggregates(m_odpContext, m_dataRegionDef.PostSortAggregates);
			dataRegionInstance.StoreAggregates(m_odpContext, m_dataRegionDef.RunningValues);
			if (m_dataRegionDef.DataScopeInfo != null)
			{
				DataScopeInfo dataScopeInfo = m_dataRegionDef.DataScopeInfo;
				dataRegionInstance.StoreAggregates(m_odpContext, dataScopeInfo.AggregatesOfAggregates);
				dataRegionInstance.StoreAggregates(m_odpContext, dataScopeInfo.PostSortAggregatesOfAggregates);
				dataRegionInstance.StoreAggregates(m_odpContext, dataScopeInfo.RunningValuesOfAggregates);
			}
			if (m_firstRow != null)
			{
				dataRegionInstance.FirstRowOffset = m_firstRow.StreamOffset;
			}
			m_dataRegionDef.ResetInstancePathCascade();
			if (m_dataRegionDef.InScopeEventSources != null)
			{
				UserSortFilterContext.ProcessEventSources(m_odpContext, this, m_dataRegionDef.InScopeEventSources);
			}
			CreateDataRegionScopedInstance(dataRegionInstance);
			IReference<RuntimeMemberObj>[] array;
			IReference<RuntimeMemberObj>[] innerMembers;
			bool outerGroupings;
			if (base.DataRegionDef.ProcessingInnerGrouping == Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion.ProcessingInnerGroupings.Column)
			{
				array = m_outerGroupings;
				innerMembers = m_innerGroupings;
				outerGroupings = true;
			}
			else
			{
				array = m_innerGroupings;
				innerMembers = m_outerGroupings;
				outerGroupings = false;
			}
			IReference<RuntimeMemberObj>[] array2 = array;
			foreach (IReference<RuntimeMemberObj> reference in array2)
			{
				using (reference.PinValue())
				{
					reference.Value().CreateInstances(base.SelfReference, m_odpContext, dataRegionInstance, outerGroupings, null, dataRegionInstance, innerMembers, null);
				}
			}
			m_dataRegionDef.ResetInstancePathCascade();
			m_dataRegionDef.ResetInstanceIndexes();
		}

		protected virtual void CreateDataRegionScopedInstance(DataRegionInstance dataRegionInstance)
		{
		}

		public IOnDemandMemberInstanceReference GetFirstMemberInstance(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode rifMember)
		{
			IReference<RuntimeMemberObj>[] memberCollection = GetMemberCollection(rifMember);
			return RuntimeDataRegionObj.GetFirstMemberInstance(rifMember, memberCollection);
		}

		private IReference<RuntimeMemberObj>[] GetMemberCollection(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode rifMember)
		{
			if (m_dataRegionDef.ProcessingInnerGrouping == Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion.ProcessingInnerGroupings.Column)
			{
				if (rifMember.IsColumn)
				{
					return m_innerGroupings;
				}
				return m_outerGroupings;
			}
			if (rifMember.IsColumn)
			{
				return m_outerGroupings;
			}
			return m_innerGroupings;
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
				Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode rifMember = scope as Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode;
				IReference<RuntimeMemberObj>[] memberCollection = GetMemberCollection(rifMember);
				return RuntimeDataRegionObj.GetGroupRoot(rifMember, memberCollection);
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
				case MemberName.OuterGroupingCounters:
					writer.Write(m_outerGroupingCounters);
					break;
				case MemberName.OuterGroupings:
					writer.Write(m_outerGroupings);
					break;
				case MemberName.InnerGroupings:
					writer.Write(m_innerGroupings);
					break;
				case MemberName.InnerGroupsWithCellsForOuterPeerGroupProcessing:
					writer.Write(m_innerGroupsWithCellsForOuterPeerGroupProcessing);
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
				case MemberName.OuterGroupingCounters:
					m_outerGroupingCounters = reader.ReadInt32Array();
					break;
				case MemberName.OuterGroupings:
					m_outerGroupings = reader.ReadArrayOfRIFObjects<IReference<RuntimeMemberObj>>();
					break;
				case MemberName.InnerGroupings:
					m_innerGroupings = reader.ReadArrayOfRIFObjects<IReference<RuntimeMemberObj>>();
					break;
				case MemberName.InnerGroupsWithCellsForOuterPeerGroupProcessing:
					m_innerGroupsWithCellsForOuterPeerGroupProcessing = reader.ReadListOfRIFObjects<List<IReference<RuntimeDataTablixGroupLeafObj>>>();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.OuterGroupingCounters, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Int32));
				list.Add(new MemberInfo(MemberName.OuterGroupings, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObjReference));
				list.Add(new MemberInfo(MemberName.InnerGroupings, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObjReference));
				list.Add(new MemberInfo(MemberName.InnerGroupsWithCellsForOuterPeerGroupProcessing, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObjReference));
				list.Add(new MemberInfo(MemberName.ScopeInstanceNumber, Token.Int64));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRDLDataRegionObj, list);
			}
			return m_declaration;
		}
	}
}

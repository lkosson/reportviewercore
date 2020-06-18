using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeGroupLeafObj : RuntimeGroupObj, IDataRowHolder
	{
		protected List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_nonCustomAggregates;

		protected BucketedDataAggregateObjs m_aggregatesOfAggregates;

		protected BucketedDataAggregateObjs m_postSortAggregatesOfAggregates;

		protected List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_customAggregates;

		protected DataFieldRow m_firstRow;

		protected bool m_firstRowIsAggregate;

		protected RuntimeGroupLeafObjReference m_nextLeaf;

		protected RuntimeGroupLeafObjReference m_prevLeaf;

		protected ScalableList<DataFieldRow> m_dataRows;

		protected IReference<RuntimeGroupObj> m_parent;

		protected List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_recursiveAggregates;

		protected List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_postSortAggregates;

		protected int m_recursiveLevel;

		protected List<object> m_groupExprValues;

		protected bool[] m_targetScopeMatched;

		protected DataActions m_dataAction;

		protected RuntimeUserSortTargetInfo m_userSortTargetInfo;

		protected int[] m_sortFilterExpressionScopeInfoIndices;

		protected object[] m_variableValues;

		protected int m_detailRowCounter;

		protected List<IHierarchyObj> m_detailSortAdditionalGroupLeafs;

		protected Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode m_hierarchyDef;

		protected bool m_isOuterGrouping;

		protected bool m_hasProcessedAggregateRow;

		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		internal bool IsOuterGrouping => m_isOuterGrouping;

		internal RuntimeGroupLeafObjReference NextLeaf
		{
			set
			{
				m_nextLeaf = value;
			}
		}

		internal RuntimeGroupLeafObjReference PrevLeaf
		{
			set
			{
				m_prevLeaf = value;
			}
		}

		internal IReference<RuntimeGroupObj> Parent
		{
			get
			{
				return m_parent;
			}
			set
			{
				m_parent = value;
			}
		}

		protected override IReference<IScope> OuterScope => m_hierarchyRoot;

		internal override int RecursiveLevel => m_recursiveLevel;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode MemberDef => m_hierarchyDef;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.Grouping GroupingDef => m_hierarchyDef.Grouping;

		protected override string ScopeName => m_hierarchyDef.Grouping.Name;

		protected override IReference<IHierarchyObj> HierarchyRoot
		{
			get
			{
				if (ProcessingStages.UserSortFilter == ((RuntimeGroupRootObj)m_hierarchyRoot.Value()).ProcessingStage)
				{
					return (RuntimeGroupLeafObjReference)m_selfReference;
				}
				return m_hierarchyRoot;
			}
		}

		protected override BTree SortTree
		{
			get
			{
				if (ProcessingStages.UserSortFilter == ((RuntimeGroupRootObj)m_hierarchyRoot.Value()).ProcessingStage)
				{
					if (m_userSortTargetInfo != null)
					{
						return m_userSortTargetInfo.SortTree;
					}
					return null;
				}
				return m_grouping.Tree;
			}
		}

		protected override int ExpressionIndex
		{
			get
			{
				if (ProcessingStages.UserSortFilter == ((RuntimeGroupRootObj)m_hierarchyRoot.Value()).ProcessingStage)
				{
					return 0;
				}
				Global.Tracer.Assert(condition: false);
				return -1;
			}
		}

		protected override List<int> SortFilterInfoIndices
		{
			get
			{
				if (m_userSortTargetInfo != null)
				{
					return m_userSortTargetInfo.SortFilterInfoIndices;
				}
				return null;
			}
		}

		protected RuntimeGroupRootObjReference GroupRoot => m_hierarchyRoot as RuntimeGroupRootObjReference;

		internal DataFieldRow FirstRow => m_firstRow;

		internal override bool TargetForNonDetailSort
		{
			get
			{
				if (m_userSortTargetInfo != null && m_userSortTargetInfo.TargetForNonDetailSort)
				{
					return true;
				}
				return m_hierarchyRoot.Value().TargetForNonDetailSort;
			}
		}

		protected override int[] SortFilterExpressionScopeInfoIndices
		{
			get
			{
				if (m_sortFilterExpressionScopeInfoIndices == null)
				{
					m_sortFilterExpressionScopeInfoIndices = new int[m_odpContext.RuntimeSortFilterInfo.Count];
					for (int i = 0; i < m_odpContext.RuntimeSortFilterInfo.Count; i++)
					{
						m_sortFilterExpressionScopeInfoIndices[i] = -1;
					}
				}
				return m_sortFilterExpressionScopeInfoIndices;
			}
		}

		internal override IRIFReportScope RIFReportScope => m_hierarchyDef;

		internal int DetailSortRowCounter => m_detailRowCounter;

		internal ScalableList<DataFieldRow> DataRows => m_dataRows;

		public override int Size => base.Size + ItemSizes.SizeOf(m_nonCustomAggregates) + ItemSizes.SizeOf(m_customAggregates) + ItemSizes.SizeOf(m_firstRow) + 1 + ItemSizes.SizeOf(m_nextLeaf) + ItemSizes.SizeOf(m_prevLeaf) + ItemSizes.SizeOf(m_dataRows) + ItemSizes.SizeOf(m_parent) + ItemSizes.SizeOf(m_recursiveAggregates) + ItemSizes.SizeOf(m_postSortAggregates) + 4 + ItemSizes.SizeOf(m_groupExprValues) + ItemSizes.SizeOf(m_targetScopeMatched) + 4 + ItemSizes.SizeOf(m_userSortTargetInfo) + ItemSizes.SizeOf(m_sortFilterExpressionScopeInfoIndices) + 1 + ItemSizes.SizeOf(m_variableValues) + 4 + ItemSizes.SizeOf(m_detailSortAdditionalGroupLeafs) + ItemSizes.ReferenceSize + ItemSizes.SizeOf(m_aggregatesOfAggregates) + ItemSizes.SizeOf(m_postSortAggregatesOfAggregates) + 1;

		protected RuntimeGroupLeafObj()
		{
		}

		protected RuntimeGroupLeafObj(RuntimeGroupRootObjReference groupRootRef, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(groupRootRef.Value().OdpContext, objectType, ((IScope)groupRootRef.Value()).Depth + 1)
		{
			RuntimeGroupRootObj runtimeGroupRootObj = groupRootRef.Value();
			m_hierarchyDef = runtimeGroupRootObj.HierarchyDef;
			m_hierarchyRoot = groupRootRef;
			Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = m_hierarchyDef.Grouping;
			RuntimeDataRegionObj.CreateAggregates(m_odpContext, grouping.Aggregates, ref m_nonCustomAggregates, ref m_customAggregates);
			RuntimeDataRegionObj.CreateAggregates(m_odpContext, grouping.RecursiveAggregates, ref m_recursiveAggregates);
			RuntimeDataRegionObj.CreateAggregates(m_odpContext, grouping.PostSortAggregates, ref m_postSortAggregates);
			if (m_hierarchyDef.DataScopeInfo != null)
			{
				DataScopeInfo dataScopeInfo = m_hierarchyDef.DataScopeInfo;
				RuntimeDataRegionObj.CreateAggregates(m_odpContext, dataScopeInfo.AggregatesOfAggregates, ref m_aggregatesOfAggregates);
				RuntimeDataRegionObj.CreateAggregates(m_odpContext, dataScopeInfo.PostSortAggregatesOfAggregates, ref m_postSortAggregatesOfAggregates);
			}
			if (runtimeGroupRootObj.SaveGroupExprValues)
			{
				m_groupExprValues = grouping.CurrentGroupExpressionValues;
			}
			RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = runtimeGroupRootObj as RuntimeDataTablixGroupRootObj;
			m_isOuterGrouping = (runtimeDataTablixGroupRootObj != null && runtimeDataTablixGroupRootObj.InnerGroupings != null);
		}

		internal override bool IsTargetForSort(int index, bool detailSort)
		{
			if (m_userSortTargetInfo != null && m_userSortTargetInfo.IsTargetForSort(index, detailSort))
			{
				return true;
			}
			return m_hierarchyRoot.Value().IsTargetForSort(index, detailSort);
		}

		protected virtual void ConstructRuntimeStructure(ref bool handleMyDataAction, out DataActions innerDataAction)
		{
			if (m_postSortAggregates != null)
			{
				handleMyDataAction = true;
			}
			if (m_recursiveAggregates != null && (m_odpContext.SpecialRecursiveAggregates || MemberDef.HasInnerDynamic))
			{
				handleMyDataAction = true;
			}
			if (handleMyDataAction)
			{
				innerDataAction = DataActions.None;
			}
			else
			{
				innerDataAction = ((RuntimeGroupRootObj)m_hierarchyRoot.Value()).DataAction;
			}
		}

		protected bool HandleSortFilterEvent(bool isColumnAxis)
		{
			if (m_odpContext.RuntimeSortFilterInfo == null)
			{
				return false;
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.Grouping groupingDef = GroupingDef;
			if (groupingDef.IsDetail)
			{
				return false;
			}
			int count = m_odpContext.RuntimeSortFilterInfo.Count;
			if (groupingDef.SortFilterScopeMatched != null || groupingDef.NeedScopeInfoForSortFilterExpression != null)
			{
				m_targetScopeMatched = new bool[count];
				for (int i = 0; i < count; i++)
				{
					IReference<RuntimeSortFilterEventInfo> reference = m_odpContext.RuntimeSortFilterInfo[i];
					using (reference.PinValue())
					{
						RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
						if (groupingDef.SortFilterScopeMatched != null && groupingDef.SortFilterScopeIndex != null && -1 != groupingDef.SortFilterScopeIndex[i])
						{
							m_targetScopeMatched[i] = groupingDef.SortFilterScopeMatched[i];
							if (!m_targetScopeMatched[i])
							{
								continue;
							}
							if (groupingDef.IsSortFilterTarget != null && groupingDef.IsSortFilterTarget[i] && !m_hierarchyRoot.Value().TargetForNonDetailSort)
							{
								runtimeSortFilterEventInfo.EventTarget = (IReference<IHierarchyObj>)base.SelfReference;
								if (m_userSortTargetInfo == null)
								{
									m_userSortTargetInfo = new RuntimeUserSortTargetInfo((IReference<IHierarchyObj>)base.SelfReference, i, reference);
								}
								else
								{
									m_userSortTargetInfo.AddSortInfo((IReference<IHierarchyObj>)base.SelfReference, i, reference);
								}
							}
							Global.Tracer.Assert(runtimeSortFilterEventInfo.EventSource.ContainingScopes != null, "(null != sortFilterInfo.EventSource.ContainingScopes)");
							if (groupingDef == runtimeSortFilterEventInfo.EventSource.ContainingScopes.LastEntry && !runtimeSortFilterEventInfo.EventSource.IsTablixCellScope && !m_hierarchyRoot.Value().TargetForNonDetailSort)
							{
								runtimeSortFilterEventInfo.SetEventSourceScope(isColumnAxis, base.SelfReference, -1);
							}
							continue;
						}
						m_targetScopeMatched[i] = ((RuntimeGroupRootObj)m_hierarchyRoot.Value()).TargetScopeMatched(i, detailSort: false);
					}
				}
			}
			m_odpContext.RegisterSortFilterExpressionScope(m_hierarchyRoot, base.SelfReference, groupingDef.IsSortFilterExpressionScope);
			if (m_userSortTargetInfo != null && m_userSortTargetInfo.TargetForNonDetailSort)
			{
				return true;
			}
			return false;
		}

		internal override void GetScopeValues(IReference<IHierarchyObj> targetScopeObj, List<object>[] scopeValues, ref int index)
		{
			if (GroupingDef.IsDetail)
			{
				DetailGetScopeValues(GroupRoot.Value().GroupRootOuterScope, targetScopeObj, scopeValues, ref index);
			}
			else if (targetScopeObj == null || this != targetScopeObj.Value())
			{
				using (m_hierarchyRoot.PinValue())
				{
					m_hierarchyRoot.Value().GetScopeValues(targetScopeObj, scopeValues, ref index);
				}
				Global.Tracer.Assert(m_groupExprValues != null, "(null != m_groupExprValues)");
				Global.Tracer.Assert(index < scopeValues.Length, "(index < scopeValues.Length)");
				scopeValues[index++] = m_groupExprValues;
			}
		}

		internal override bool TargetScopeMatched(int index, bool detailSort)
		{
			if (GroupingDef.IsDetail)
			{
				RuntimeGroupRootObj runtimeGroupRootObj = GroupRoot.Value();
				return DetailTargetScopeMatched(MemberDef.DataRegionDef, runtimeGroupRootObj.GroupRootOuterScope, runtimeGroupRootObj.HierarchyDef.IsColumn, index);
			}
			if (detailSort && GroupingDef.SortFilterScopeInfo == null)
			{
				return true;
			}
			if (m_targetScopeMatched != null)
			{
				return m_targetScopeMatched[index];
			}
			return false;
		}

		protected void UpdateAggregateInfo()
		{
			FieldsImpl fieldsImpl = m_odpContext.ReportObjectModel.FieldsImpl;
			if (!fieldsImpl.ValidAggregateRow)
			{
				return;
			}
			int[] groupExpressionFieldIndices = GroupingDef.GetGroupExpressionFieldIndices();
			if (groupExpressionFieldIndices != null)
			{
				foreach (int num in groupExpressionFieldIndices)
				{
					if (num >= 0)
					{
						fieldsImpl.ConsumeAggregationField(num);
					}
				}
			}
			if (fieldsImpl.AggregationFieldCount == 0 && m_customAggregates != null && (IsOuterGrouping || !m_odpContext.PeerOuterGroupProcessing))
			{
				m_hasProcessedAggregateRow = true;
				RuntimeDataRegionObj.UpdateAggregates(m_odpContext, m_customAggregates, updateAndSetup: false);
			}
		}

		protected void InternalNextRow()
		{
			using (m_hierarchyRoot.PinValue())
			{
				RuntimeGroupRootObj runtimeGroupRootObj = m_hierarchyRoot.Value() as RuntimeGroupRootObj;
				ProcessingStages processingStage = runtimeGroupRootObj.ProcessingStage;
				runtimeGroupRootObj.ProcessingStage = ProcessingStages.UserSortFilter;
				if (runtimeGroupRootObj.IsDetailGroup)
				{
					DetailHandleSortFilterEvent(MemberDef.DataRegionDef, runtimeGroupRootObj.GroupRootOuterScope, runtimeGroupRootObj.HierarchyDef.IsColumn, m_odpContext.ReportObjectModel.FieldsImpl.GetRowIndex());
				}
				RuntimeDataRegionObj.CommonFirstRow(m_odpContext, ref m_firstRowIsAggregate, ref m_firstRow);
				if (IsOuterGrouping || !m_odpContext.PeerOuterGroupProcessing)
				{
					if (m_odpContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
					{
						ScopeNextAggregateRow(m_userSortTargetInfo);
						if (MemberDef.DataRegionDef.IsMatrixIDC)
						{
							AggregateRow aggregateRow = new AggregateRow(m_odpContext.ReportObjectModel.FieldsImpl, getAndSave: true);
							aggregateRow.SaveAggregateInfo(m_odpContext);
							m_dataRows.Add(aggregateRow);
						}
					}
					else
					{
						ScopeNextNonAggregateRow(m_nonCustomAggregates, m_dataRows);
					}
				}
				else
				{
					SendToInner();
				}
				runtimeGroupRootObj.ProcessingStage = processingStage;
			}
		}

		protected override void SendToInner()
		{
			using (m_hierarchyRoot.PinValue())
			{
				((RuntimeGroupRootObj)m_hierarchyRoot.Value()).ProcessingStage = ProcessingStages.Grouping;
			}
		}

		internal void RemoveFromParent(RuntimeGroupObjReference parentRef)
		{
			using (parentRef.PinValue())
			{
				RuntimeGroupObj runtimeGroupObj = parentRef.Value();
				if (null == m_prevLeaf)
				{
					runtimeGroupObj.FirstChild = m_nextLeaf;
				}
				else
				{
					using (m_prevLeaf.PinValue())
					{
						m_prevLeaf.Value().m_nextLeaf = m_nextLeaf;
					}
				}
				if (null == m_nextLeaf)
				{
					runtimeGroupObj.LastChild = m_prevLeaf;
					return;
				}
				using (m_nextLeaf.PinValue())
				{
					m_nextLeaf.Value().m_prevLeaf = m_prevLeaf;
				}
			}
		}

		private IReference<RuntimeGroupLeafObj> Traverse(ProcessingStages operation, ITraversalContext traversalContext)
		{
			IReference<RuntimeGroupLeafObj> nextLeaf = m_nextLeaf;
			if (((RuntimeGroupRootObj)m_hierarchyRoot.Value()).HasParent)
			{
				m_recursiveLevel = m_parent.Value().RecursiveLevel + 1;
			}
			bool flag = IsSpecialFilteringPass(operation);
			if (flag)
			{
				m_lastChild = null;
				ProcessChildren(operation, traversalContext);
			}
			switch (operation)
			{
			case ProcessingStages.SortAndFilter:
				SortAndFilter((AggregateUpdateContext)traversalContext);
				break;
			case ProcessingStages.PreparePeerGroupRunningValues:
				PrepareCalculateRunningValues();
				break;
			case ProcessingStages.RunningValues:
			{
				bool flag2 = false;
				if (m_odpContext.HasPreviousAggregates)
				{
					flag2 = ((RuntimeGroupRootObj)m_hierarchyRoot.Value()).IsDetailGroup;
					if (m_groupExprValues != null && !flag2)
					{
						m_odpContext.GroupExpressionValues.AddRange(m_groupExprValues);
					}
				}
				CalculateRunningValues((AggregateUpdateContext)traversalContext);
				if (m_odpContext.HasPreviousAggregates && m_groupExprValues != null && !flag2)
				{
					m_odpContext.GroupExpressionValues.RemoveRange(m_odpContext.GroupExpressionValues.Count - m_groupExprValues.Count, m_groupExprValues.Count);
				}
				break;
			}
			case ProcessingStages.CreateGroupTree:
				CreateInstance((CreateInstancesTraversalContext)traversalContext);
				break;
			case ProcessingStages.UpdateAggregates:
				UpdateAggregates((AggregateUpdateContext)traversalContext);
				break;
			}
			if (!flag)
			{
				ProcessChildren(operation, traversalContext);
			}
			return nextLeaf;
		}

		internal void TraverseAllLeafNodes(ProcessingStages operation, ITraversalContext traversalContext)
		{
			IReference<RuntimeGroupLeafObj> reference = base.SelfReference as IReference<RuntimeGroupLeafObj>;
			while (reference != null)
			{
				TablixProcessingMoveNext(operation);
				using (reference.PinValue())
				{
					reference = reference.Value().Traverse(operation, traversalContext);
				}
			}
		}

		protected void TablixProcessingMoveNext(ProcessingStages operation)
		{
			if (operation == ProcessingStages.CreateGroupTree && (m_odpContext.ReportDefinition.ReportOrDescendentHasUserSortFilter || m_odpContext.ReportDefinition.HasSubReports))
			{
				MemberDef.MoveNextForUserSort(m_odpContext);
			}
		}

		private void ProcessChildren(ProcessingStages operation, ITraversalContext traversalContext)
		{
			if (!(null != m_firstChild) && m_grouping == null)
			{
				return;
			}
			using (m_hierarchyRoot.PinValue())
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)m_hierarchyRoot.Value();
				if (null != m_firstChild)
				{
					using (m_firstChild.PinValue())
					{
						m_firstChild.Value().TraverseAllLeafNodes(operation, traversalContext);
					}
					if (operation != ProcessingStages.SortAndFilter)
					{
						return;
					}
					if ((SecondPassOperations.FilteringOrAggregatesOrDomainScope & m_odpContext.SecondPassOperation) != 0 && runtimeGroupRootObj.HierarchyDef.Grouping.Filters != null)
					{
						if (null == m_lastChild)
						{
							m_firstChild = null;
						}
					}
					else if (m_grouping != null)
					{
						m_firstChild = null;
					}
					return;
				}
				if (m_grouping != null)
				{
					m_grouping.Traverse(operation, runtimeGroupRootObj.Expression.Direction, traversalContext);
				}
			}
		}

		private bool IsSpecialFilteringPass(ProcessingStages operation)
		{
			if (ProcessingStages.SortAndFilter == operation && m_odpContext.SpecialRecursiveAggregates && (SecondPassOperations.FilteringOrAggregatesOrDomainScope & m_odpContext.SecondPassOperation) != 0)
			{
				return true;
			}
			return false;
		}

		internal override bool SortAndFilter(AggregateUpdateContext aggContext)
		{
			bool flag = true;
			bool specialFilter = false;
			using (m_hierarchyRoot.PinValue())
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)m_hierarchyRoot.Value();
				Global.Tracer.Assert(runtimeGroupRootObj != null, "(null != groupRoot)");
				if (MemberDef.Grouping.Variables != null)
				{
					ScopeInstance.ResetVariables(m_odpContext, MemberDef.Grouping.Variables);
					ScopeInstance.CalculateVariables(m_odpContext, MemberDef.Grouping.Variables, out m_variableValues);
				}
				if (runtimeGroupRootObj.ProcessSecondPassSorting && null != m_firstChild)
				{
					m_expression = runtimeGroupRootObj.Expression;
					m_grouping = new RuntimeGroupingObjTree(this, m_objectType);
				}
				m_lastChild = null;
				if (m_odpContext.HasSecondPassOperation(SecondPassOperations.FilteringOrAggregatesOrDomainScope))
				{
					if (m_odpContext.SpecialRecursiveAggregates && m_recursiveAggregates != null)
					{
						Global.Tracer.Assert(m_dataRows != null, "(null != m_dataRows)");
						ReadRows(sendToParent: false);
					}
					if (runtimeGroupRootObj.GroupFilters != null)
					{
						SetupEnvironment();
						flag = runtimeGroupRootObj.GroupFilters.PassFilters(base.SelfReference, out specialFilter);
					}
				}
				if (flag)
				{
					PostFilterNextRow(aggContext);
					return flag;
				}
				if (!specialFilter)
				{
					FailFilter();
					return flag;
				}
				return flag;
			}
		}

		internal void FailFilter()
		{
			RuntimeGroupLeafObjReference runtimeGroupLeafObjReference = null;
			bool flag = false;
			if (IsSpecialFilteringPass(ProcessingStages.SortAndFilter))
			{
				flag = true;
			}
			if (!(m_firstChild != null))
			{
				return;
			}
			using (m_parent.PinValue())
			{
				RuntimeGroupObj runtimeGroupObj = m_parent.Value();
				RuntimeGroupLeafObjReference runtimeGroupLeafObjReference2 = m_firstChild;
				while (runtimeGroupLeafObjReference2 != null)
				{
					using (runtimeGroupLeafObjReference2.PinValue())
					{
						RuntimeGroupLeafObj runtimeGroupLeafObj = runtimeGroupLeafObjReference2.Value();
						runtimeGroupLeafObjReference = runtimeGroupLeafObj.m_nextLeaf;
						runtimeGroupLeafObj.m_parent = m_parent;
						if (flag)
						{
							runtimeGroupObj.AddChild(runtimeGroupLeafObjReference2);
						}
					}
					runtimeGroupLeafObjReference2 = runtimeGroupLeafObjReference;
				}
			}
		}

		internal virtual void PostFilterNextRow(AggregateUpdateContext context)
		{
			using (m_hierarchyRoot.PinValue())
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)m_hierarchyRoot.Value();
				if ((SecondPassOperations.FilteringOrAggregatesOrDomainScope & m_odpContext.SecondPassOperation) != 0 && m_dataRows != null && (m_dataAction & DataActions.RecursiveAggregates) != 0)
				{
					if (m_odpContext.SpecialRecursiveAggregates)
					{
						ReadRows(sendToParent: true);
					}
					else
					{
						ReadRows(DataActions.RecursiveAggregates, null);
					}
					ReleaseDataRows(DataActions.RecursiveAggregates, ref m_dataAction, ref m_dataRows);
				}
				bool processSecondPassSorting = runtimeGroupRootObj.ProcessSecondPassSorting;
				if (processSecondPassSorting)
				{
					SetupEnvironment();
				}
				if (processSecondPassSorting || runtimeGroupRootObj.GroupFilters != null)
				{
					m_nextLeaf = null;
					using (m_parent.PinValue())
					{
						m_parent.Value().InsertToSortTree((RuntimeGroupLeafObjReference)m_selfReference);
					}
				}
			}
		}

		protected abstract void PrepareCalculateRunningValues();

		internal override void CalculateRunningValues(AggregateUpdateContext aggContext)
		{
			ResetScopedRunningValues();
		}

		public override void ReadRow(DataActions dataAction, ITraversalContext context)
		{
			Global.Tracer.Assert(DataActions.UserSort != dataAction, "(DataActions.UserSort != dataAction)");
			if (FlagUtils.HasFlag(dataAction, DataActions.PostSortAggregatesOfAggregates))
			{
				((AggregateUpdateContext)context).UpdateAggregatesForRow();
			}
			if (FlagUtils.HasFlag(dataAction, DataActions.PostSortAggregates))
			{
				if (m_postSortAggregates != null)
				{
					RuntimeDataRegionObj.UpdateAggregates(m_odpContext, m_postSortAggregates, updateAndSetup: false);
				}
				Global.Tracer.Assert(null != m_hierarchyRoot, "(null != m_hierarchyRoot)");
				using (m_hierarchyRoot.PinValue())
				{
					((IScope)m_hierarchyRoot.Value()).ReadRow(DataActions.PostSortAggregates, context);
				}
			}
			else if (FlagUtils.HasFlag(dataAction, DataActions.AggregatesOfAggregates))
			{
				((AggregateUpdateContext)context).UpdateAggregatesForRow();
			}
			else
			{
				Global.Tracer.Assert(DataActions.RecursiveAggregates == dataAction, "(DataActions.RecursiveAggregates == dataAction)");
				if (m_recursiveAggregates != null)
				{
					RuntimeDataRegionObj.UpdateAggregates(m_odpContext, m_recursiveAggregates, updateAndSetup: false);
				}
				using (m_parent.PinValue())
				{
					((IScope)m_parent.Value()).ReadRow(DataActions.RecursiveAggregates, context);
				}
			}
		}

		private void ReadRow(bool sendToParent)
		{
			if (!sendToParent)
			{
				Global.Tracer.Assert(m_recursiveAggregates != null, "(null != m_recursiveAggregates)");
				RuntimeDataRegionObj.UpdateAggregates(m_odpContext, m_recursiveAggregates, updateAndSetup: false);
			}
			else
			{
				using (m_parent.PinValue())
				{
					((IScope)m_parent.Value()).ReadRow(DataActions.RecursiveAggregates, (ITraversalContext)null);
				}
			}
		}

		public override void SetupEnvironment()
		{
			if (m_hierarchyDef.DataScopeInfo != null && m_hierarchyDef.DataScopeInfo.DataSet != null)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = m_hierarchyDef.DataScopeInfo.DataSet;
				SetupNewDataSet(dataSet);
				if (m_hierarchyDef.DataScopeInfo.DataSet.DataSetCore.FieldsContext != null)
				{
					m_odpContext.ReportObjectModel.RestoreFields(m_hierarchyDef.DataScopeInfo.DataSet.DataSetCore.FieldsContext);
				}
			}
			SetupEnvironment(m_nonCustomAggregates, m_customAggregates, m_firstRow);
			MemberDef.SetUserSortDetailRowIndex(m_odpContext);
			SetupAggregates(m_aggregatesOfAggregates);
			SetupAggregates(m_postSortAggregatesOfAggregates);
			SetupAggregates(m_recursiveAggregates);
			SetupAggregates(m_postSortAggregates);
			RuntimeGroupRootObj obj = (RuntimeGroupRootObj)m_hierarchyRoot.Value();
			if (obj.HasParent)
			{
				GroupingDef.RecursiveLevel = m_recursiveLevel;
			}
			if (obj.SaveGroupExprValues)
			{
				GroupingDef.CurrentGroupExpressionValues = m_groupExprValues;
			}
			SetupGroupVariables();
		}

		internal void SetupGroupVariables()
		{
			if (m_variableValues != null)
			{
				ScopeInstance.SetupVariables(m_odpContext, GroupingDef.Variables, m_variableValues);
			}
		}

		internal void CalculatePreviousAggregates(bool setupEnvironment)
		{
			if (setupEnvironment)
			{
				SetupEnvironment();
			}
			using (m_hierarchyRoot.PinValue())
			{
				((IScope)m_hierarchyRoot.Value()).CalculatePreviousAggregates();
			}
		}

		internal override void CalculatePreviousAggregates()
		{
			CalculatePreviousAggregates(setupEnvironment: true);
		}

		public void ReadRows(DataActions action, ITraversalContext context)
		{
			if (m_dataRows != null)
			{
				for (int i = 0; i < m_dataRows.Count; i++)
				{
					m_dataRows[i].SetFields(m_odpContext.ReportObjectModel.FieldsImpl);
					ReadRow(action, context);
				}
			}
		}

		private void ReadRows(bool sendToParent)
		{
			for (int i = 0; i < m_dataRows.Count; i++)
			{
				m_dataRows[i].SetFields(m_odpContext.ReportObjectModel.FieldsImpl);
				ReadRow(sendToParent);
			}
		}

		protected virtual void ResetScopedRunningValues()
		{
			using (m_hierarchyRoot.PinValue())
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)m_hierarchyRoot.Value();
				if (runtimeGroupRootObj.ScopedRunningValues == null)
				{
					return;
				}
				AggregatesImpl aggregatesImpl = m_odpContext.ReportObjectModel.AggregatesImpl;
				foreach (string scopedRunningValue in runtimeGroupRootObj.ScopedRunningValues)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = aggregatesImpl.GetAggregateObj(scopedRunningValue);
					Global.Tracer.Assert(aggregateObj != null, "Expected aggregate: {0} not in global collection", scopedRunningValue);
					aggregateObj.Init();
				}
			}
		}

		internal override bool InScope(string scope)
		{
			using (m_hierarchyRoot.PinValue())
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = ((RuntimeGroupRootObj)m_hierarchyRoot.Value()).HierarchyDef.Grouping;
				if (grouping.ScopeNames == null)
				{
					grouping.ScopeNames = GetScopeNames(base.SelfReference, scope, out bool inScope);
					return inScope;
				}
				return grouping.ScopeNames.Contains(scope);
			}
		}

		protected override int GetRecursiveLevel(string scope)
		{
			if (scope == null)
			{
				return m_recursiveLevel;
			}
			using (m_hierarchyRoot.PinValue())
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = ((RuntimeGroupRootObj)m_hierarchyRoot.Value()).HierarchyDef.Grouping;
				if (grouping.ScopeNames == null)
				{
					grouping.ScopeNames = GetScopeNames(base.SelfReference, scope, out int level);
					return level;
				}
				return (grouping.ScopeNames[scope] as Microsoft.ReportingServices.ReportIntermediateFormat.Grouping)?.RecursiveLevel ?? (-1);
			}
		}

		protected override void ProcessUserSort()
		{
			using (m_hierarchyRoot.PinValue())
			{
				((RuntimeGroupRootObj)m_hierarchyRoot.Value()).ProcessingStage = ProcessingStages.UserSortFilter;
			}
			m_odpContext.ProcessUserSortForTarget((IReference<IHierarchyObj>)base.SelfReference, ref m_dataRows, m_userSortTargetInfo.TargetForNonDetailSort);
			if (m_userSortTargetInfo.TargetForNonDetailSort)
			{
				m_dataAction &= ~DataActions.UserSort;
				m_userSortTargetInfo.ResetTargetForNonDetailSort();
				m_userSortTargetInfo.EnterProcessUserSortPhase(m_odpContext);
				bool handleMyDataAction = false;
				ConstructRuntimeStructure(ref handleMyDataAction, out DataActions innerDataAction);
				if (!handleMyDataAction)
				{
					Global.Tracer.Assert(innerDataAction == m_dataAction, "(innerDataAction == m_dataAction)");
				}
				if (m_dataAction != 0)
				{
					m_dataRows = new ScalableList<DataFieldRow>(base.Depth, m_odpContext.TablixProcessingScalabilityCache);
				}
				ScopeFinishSorting(ref m_firstRow, m_userSortTargetInfo);
				m_userSortTargetInfo.LeaveProcessUserSortPhase(m_odpContext);
			}
		}

		protected override void MarkSortInfoProcessed(List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo)
		{
			if (m_userSortTargetInfo != null)
			{
				m_userSortTargetInfo.MarkSortInfoProcessed(runtimeSortFilterInfo, (IReference<IHierarchyObj>)base.SelfReference);
			}
		}

		protected override void AddSortInfoIndex(int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
		{
			if (m_userSortTargetInfo != null)
			{
				m_userSortTargetInfo.AddSortInfoIndex(sortInfoIndex, sortInfo);
			}
		}

		public override IHierarchyObj CreateHierarchyObjForSortTree()
		{
			if (ProcessingStages.UserSortFilter == ((RuntimeGroupRootObj)m_hierarchyRoot.Value()).ProcessingStage)
			{
				return new RuntimeSortHierarchyObj(this, m_depth + 1);
			}
			return base.CreateHierarchyObjForSortTree();
		}

		protected override void GetGroupNameValuePairs(Dictionary<string, object> pairs)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = ((RuntimeGroupRootObj)m_hierarchyRoot.Value()).HierarchyDef.Grouping;
			if (grouping.ScopeNames == null)
			{
				grouping.ScopeNames = GetScopeNames(base.SelfReference, pairs);
				return;
			}
			IEnumerator enumerator = grouping.ScopeNames.Values.GetEnumerator();
			while (enumerator.MoveNext())
			{
				RuntimeDataRegionObj.AddGroupNameValuePair(m_odpContext, enumerator.Current as Microsoft.ReportingServices.ReportIntermediateFormat.Grouping, pairs);
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.NonCustomAggregates:
					writer.Write(m_nonCustomAggregates);
					break;
				case MemberName.CustomAggregates:
					writer.Write(m_customAggregates);
					break;
				case MemberName.FirstRow:
					writer.Write(m_firstRow);
					break;
				case MemberName.FirstRowIsAggregate:
					writer.Write(m_firstRowIsAggregate);
					break;
				case MemberName.NextLeaf:
					writer.Write(m_nextLeaf);
					break;
				case MemberName.PrevLeaf:
					writer.Write(m_prevLeaf);
					break;
				case MemberName.DataRows:
					writer.Write(m_dataRows);
					break;
				case MemberName.Parent:
					writer.Write(m_parent);
					break;
				case MemberName.RecursiveAggregates:
					writer.Write(m_recursiveAggregates);
					break;
				case MemberName.PostSortAggregates:
					writer.Write(m_postSortAggregates);
					break;
				case MemberName.RecursiveLevel:
					writer.Write(m_recursiveLevel);
					break;
				case MemberName.GroupExprValues:
					writer.WriteListOfVariant(m_groupExprValues);
					break;
				case MemberName.TargetScopeMatched:
					writer.Write(m_targetScopeMatched);
					break;
				case MemberName.DataAction:
					writer.WriteEnum((int)m_dataAction);
					break;
				case MemberName.UserSortTargetInfo:
					writer.Write(m_userSortTargetInfo);
					break;
				case MemberName.SortFilterExpressionScopeInfoIndices:
					writer.Write(m_sortFilterExpressionScopeInfoIndices);
					break;
				case MemberName.HierarchyDef:
				{
					int value = scalabilityCache.StoreStaticReference(m_hierarchyDef);
					writer.Write(value);
					break;
				}
				case MemberName.IsOuterGrouping:
					writer.Write(m_isOuterGrouping);
					break;
				case MemberName.Variables:
					writer.WriteSerializableArray(m_variableValues);
					break;
				case MemberName.DetailRowCounter:
					writer.Write(m_detailRowCounter);
					break;
				case MemberName.DetailSortAdditionalGroupLeafs:
					writer.Write(m_detailSortAdditionalGroupLeafs);
					break;
				case MemberName.AggregatesOfAggregates:
					writer.Write(m_aggregatesOfAggregates);
					break;
				case MemberName.PostSortAggregatesOfAggregates:
					writer.Write(m_postSortAggregatesOfAggregates);
					break;
				case MemberName.HasProcessedAggregateRow:
					writer.Write(m_hasProcessedAggregateRow);
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
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.NonCustomAggregates:
					m_nonCustomAggregates = reader.ReadListOfRIFObjects<List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.CustomAggregates:
					m_customAggregates = reader.ReadListOfRIFObjects<List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.FirstRow:
					m_firstRow = (DataFieldRow)reader.ReadRIFObject();
					break;
				case MemberName.FirstRowIsAggregate:
					m_firstRowIsAggregate = reader.ReadBoolean();
					break;
				case MemberName.NextLeaf:
					m_nextLeaf = (RuntimeGroupLeafObjReference)reader.ReadRIFObject();
					break;
				case MemberName.PrevLeaf:
					m_prevLeaf = (RuntimeGroupLeafObjReference)reader.ReadRIFObject();
					break;
				case MemberName.DataRows:
					m_dataRows = reader.ReadRIFObject<ScalableList<DataFieldRow>>();
					break;
				case MemberName.Parent:
					m_parent = (IReference<RuntimeGroupObj>)reader.ReadRIFObject();
					break;
				case MemberName.RecursiveAggregates:
					m_recursiveAggregates = reader.ReadListOfRIFObjects<List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.PostSortAggregates:
					m_postSortAggregates = reader.ReadListOfRIFObjects<List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.RecursiveLevel:
					m_recursiveLevel = reader.ReadInt32();
					break;
				case MemberName.GroupExprValues:
					m_groupExprValues = reader.ReadListOfVariant<List<object>>();
					break;
				case MemberName.TargetScopeMatched:
					m_targetScopeMatched = reader.ReadBooleanArray();
					break;
				case MemberName.DataAction:
					m_dataAction = (DataActions)reader.ReadEnum();
					break;
				case MemberName.UserSortTargetInfo:
					m_userSortTargetInfo = (RuntimeUserSortTargetInfo)reader.ReadRIFObject();
					break;
				case MemberName.SortFilterExpressionScopeInfoIndices:
					m_sortFilterExpressionScopeInfoIndices = reader.ReadInt32Array();
					break;
				case MemberName.HierarchyDef:
				{
					int id = reader.ReadInt32();
					m_hierarchyDef = (Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.IsOuterGrouping:
					m_isOuterGrouping = reader.ReadBoolean();
					break;
				case MemberName.Variables:
					m_variableValues = reader.ReadSerializableArray();
					break;
				case MemberName.DetailRowCounter:
					m_detailRowCounter = reader.ReadInt32();
					break;
				case MemberName.DetailSortAdditionalGroupLeafs:
					m_detailSortAdditionalGroupLeafs = reader.ReadListOfRIFObjects<List<IHierarchyObj>>();
					break;
				case MemberName.AggregatesOfAggregates:
					m_aggregatesOfAggregates = (BucketedDataAggregateObjs)reader.ReadRIFObject();
					break;
				case MemberName.PostSortAggregatesOfAggregates:
					m_postSortAggregatesOfAggregates = (BucketedDataAggregateObjs)reader.ReadRIFObject();
					break;
				case MemberName.HasProcessedAggregateRow:
					m_hasProcessedAggregateRow = reader.ReadBoolean();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.NonCustomAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.CustomAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.FirstRow, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow));
				list.Add(new MemberInfo(MemberName.FirstRowIsAggregate, Token.Boolean));
				list.Add(new MemberInfo(MemberName.NextLeaf, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObjReference));
				list.Add(new MemberInfo(MemberName.PrevLeaf, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObjReference));
				list.Add(new MemberInfo(MemberName.DataRows, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
				list.Add(new MemberInfo(MemberName.Parent, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObjReference));
				list.Add(new MemberInfo(MemberName.RecursiveAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.PostSortAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.RecursiveLevel, Token.Int32));
				list.Add(new MemberInfo(MemberName.GroupExprValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VariantList));
				list.Add(new MemberInfo(MemberName.TargetScopeMatched, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Boolean));
				list.Add(new MemberInfo(MemberName.DataAction, Token.Enum));
				list.Add(new MemberInfo(MemberName.UserSortTargetInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeUserSortTargetInfo));
				list.Add(new MemberInfo(MemberName.SortFilterExpressionScopeInfoIndices, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Int32));
				list.Add(new MemberInfo(MemberName.HierarchyDef, Token.Int32));
				list.Add(new MemberInfo(MemberName.IsOuterGrouping, Token.Boolean));
				list.Add(new MemberInfo(MemberName.Variables, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SerializableArray, Token.Serializable));
				list.Add(new MemberInfo(MemberName.DetailRowCounter, Token.Int32));
				list.Add(new MemberInfo(MemberName.DetailSortAdditionalGroupLeafs, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IHierarchyObj));
				list.Add(new MemberInfo(MemberName.AggregatesOfAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BucketedDataAggregateObjs));
				list.Add(new MemberInfo(MemberName.PostSortAggregatesOfAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BucketedDataAggregateObjs));
				list.Add(new MemberInfo(MemberName.HasProcessedAggregateRow, Token.Boolean));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObj, list);
			}
			return m_declaration;
		}
	}
}

using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeGroupRootObj : RuntimeGroupObj, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner, IStorable, IPersistable, IDataCorrelation
	{
		protected Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode m_hierarchyDef;

		protected IReference<IScope> m_outerScope;

		private ProcessingStages m_processingStage = ProcessingStages.Grouping;

		protected List<string> m_scopedRunningValues;

		protected List<string> m_runningValuesInGroup;

		protected List<string> m_previousValuesInGroup;

		protected Dictionary<string, IReference<RuntimeGroupRootObj>> m_groupCollection;

		protected DataActions m_dataAction;

		protected DataActions m_outerDataAction;

		protected RuntimeGroupingObj.GroupingTypes m_groupingType;

		[Reference]
		protected Filters m_groupFilters;

		protected RuntimeExpressionInfo m_parentExpression;

		protected object m_currentGroupExprValue;

		protected bool m_saveGroupExprValues = true;

		protected int[] m_sortFilterExpressionScopeInfoIndices;

		private bool[] m_builtinSortOverridden;

		protected bool m_isDetailGroup;

		protected RuntimeUserSortTargetInfo m_detailUserSortTargetInfo;

		protected ScalableList<DataFieldRow> m_detailDataRows;

		private static Declaration m_declaration = GetDeclaration();

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode HierarchyDef => m_hierarchyDef;

		internal List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo> GroupExpressions => m_hierarchyDef.Grouping.GroupExpressions;

		internal GroupExprHost GroupExpressionHost => m_hierarchyDef.Grouping.ExprHost;

		internal List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo> SortExpressions
		{
			get
			{
				if (m_hierarchyDef.Sorting != null && m_hierarchyDef.Sorting.ShouldApplySorting)
				{
					return m_hierarchyDef.Sorting.SortExpressions;
				}
				return null;
			}
		}

		internal SortExprHost SortExpressionHost => m_hierarchyDef.Sorting.ExprHost;

		internal List<bool> GroupDirections => m_hierarchyDef.Grouping.SortDirections;

		internal List<bool> SortDirections => m_hierarchyDef.Sorting.SortDirections;

		internal RuntimeExpressionInfo Expression => m_expression;

		internal List<string> ScopedRunningValues => m_scopedRunningValues;

		internal Dictionary<string, IReference<RuntimeGroupRootObj>> GroupCollection => m_groupCollection;

		internal DataActions DataAction => m_dataAction;

		internal ProcessingStages ProcessingStage
		{
			get
			{
				return m_processingStage;
			}
			set
			{
				m_processingStage = value;
			}
		}

		internal DataRegionInstance DataRegionInstance => m_hierarchyDef.DataRegionDef.CurrentDataRegionInstance;

		internal RuntimeGroupingObj.GroupingTypes GroupingType => m_groupingType;

		internal Filters GroupFilters => m_groupFilters;

		internal bool HasParent => m_parentExpression != null;

		protected override IReference<IScope> OuterScope => m_outerScope;

		internal IReference<IScope> GroupRootOuterScope => m_outerScope;

		internal bool SaveGroupExprValues => true;

		internal bool IsDetailGroup => m_isDetailGroup;

		protected override bool IsDetail
		{
			get
			{
				if (m_isDetailGroup)
				{
					return true;
				}
				return base.IsDetail;
			}
		}

		protected override int ExpressionIndex => 0;

		protected override List<int> SortFilterInfoIndices
		{
			get
			{
				if (m_detailUserSortTargetInfo != null)
				{
					return m_detailUserSortTargetInfo.SortFilterInfoIndices;
				}
				return null;
			}
		}

		internal BTree GroupOrDetailSortTree => SortTree;

		protected override BTree SortTree
		{
			get
			{
				if (m_detailUserSortTargetInfo != null)
				{
					return m_detailUserSortTargetInfo.SortTree;
				}
				if (m_grouping != null)
				{
					return m_grouping.Tree;
				}
				return null;
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

		private bool BuiltinSortOverridden
		{
			get
			{
				if (m_detailUserSortTargetInfo != null && IsDetailGroup)
				{
					return true;
				}
				if (m_odpContext.RuntimeSortFilterInfo != null && m_builtinSortOverridden != null)
				{
					for (int i = 0; i < m_odpContext.RuntimeSortFilterInfo.Count; i++)
					{
						if (m_odpContext.UserSortFilterContext.InProcessUserSortPhase(i) && m_builtinSortOverridden[i])
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		internal bool ProcessSecondPassSorting
		{
			get
			{
				if ((SecondPassOperations.Sorting & m_odpContext.SecondPassOperation) != 0 && !BuiltinSortOverridden && HierarchyDef.Sorting != null)
				{
					return HierarchyDef.Sorting.ShouldApplySorting;
				}
				return false;
			}
		}

		internal ScalableList<DataFieldRow> DetailDataRows
		{
			get
			{
				return m_detailDataRows;
			}
			set
			{
				m_detailDataRows = value;
			}
		}

		public override int Size => base.Size + ItemSizes.ReferenceSize + ItemSizes.SizeOf(m_outerScope) + 4 + ItemSizes.SizeOf(m_scopedRunningValues) + ItemSizes.SizeOf(m_runningValuesInGroup) + ItemSizes.ReferenceSize + ItemSizes.SizeOf(m_groupCollection) + 4 + 4 + 4 + ItemSizes.ReferenceSize + ItemSizes.SizeOf(m_parentExpression) + ItemSizes.SizeOf(m_currentGroupExprValue) + 1 + ItemSizes.SizeOf(m_sortFilterExpressionScopeInfoIndices) + ItemSizes.SizeOf(m_builtinSortOverridden) + ItemSizes.SizeOf(m_detailUserSortTargetInfo) + ItemSizes.SizeOf(m_detailDataRows) + 1;

		protected RuntimeGroupRootObj()
		{
		}

		protected RuntimeGroupRootObj(IReference<IScope> outerScope, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode hierarchyDef, DataActions dataAction, OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(odpContext, objectType, outerScope.Value().Depth + 1)
		{
			m_hierarchyRoot = (RuntimeHierarchyObjReference)m_selfReference;
			m_outerScope = outerScope;
			m_hierarchyDef = hierarchyDef;
			Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = hierarchyDef.Grouping;
			Global.Tracer.Assert(grouping != null, "(null != groupDef)");
			m_isDetailGroup = grouping.IsDetail;
			if (m_isDetailGroup)
			{
				m_expression = null;
			}
			else
			{
				m_expression = new RuntimeExpressionInfo(grouping.GroupExpressions, grouping.ExprHost, grouping.SortDirections, 0);
			}
			if (m_odpContext.RuntimeSortFilterInfo != null)
			{
				int count = m_odpContext.RuntimeSortFilterInfo.Count;
				using (outerScope.PinValue())
				{
					IScope scope = outerScope.Value();
					for (int i = 0; i < count; i++)
					{
						IReference<RuntimeSortFilterEventInfo> reference = m_odpContext.RuntimeSortFilterInfo[i];
						using (reference.PinValue())
						{
							RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
							if (runtimeSortFilterEventInfo.EventSource.ContainingScopes != null && runtimeSortFilterEventInfo.EventSource.ContainingScopes.Count != 0 && !runtimeSortFilterEventInfo.HasEventSourceScope && (!m_isDetailGroup || !runtimeSortFilterEventInfo.EventSource.IsSubReportTopLevelScope))
							{
								continue;
							}
							bool flag = false;
							if (m_isDetailGroup)
							{
								if (!scope.TargetForNonDetailSort && IsTargetForSort(i, detailSort: true) && runtimeSortFilterEventInfo.EventTarget != base.SelfReference && scope.TargetScopeMatched(i, detailSort: true))
								{
									flag = true;
									if (m_detailUserSortTargetInfo == null)
									{
										m_detailUserSortTargetInfo = new RuntimeUserSortTargetInfo((IReference<IHierarchyObj>)base.SelfReference, i, reference);
									}
									else
									{
										m_detailUserSortTargetInfo.AddSortInfo((IReference<IHierarchyObj>)base.SelfReference, i, reference);
									}
								}
							}
							else if (grouping.IsSortFilterExpressionScope != null)
							{
								flag = (grouping.IsSortFilterExpressionScope[i] && m_odpContext.UserSortFilterContext.InProcessUserSortPhase(i) && TargetScopeMatched(i, detailSort: false));
							}
							if (flag)
							{
								if (m_builtinSortOverridden == null)
								{
									m_builtinSortOverridden = new bool[count];
								}
								m_builtinSortOverridden[i] = true;
							}
						}
					}
				}
			}
			if (m_detailUserSortTargetInfo != null)
			{
				m_groupingType = RuntimeGroupingObj.GroupingTypes.DetailUserSort;
			}
			else if (grouping.GroupAndSort && !BuiltinSortOverridden)
			{
				m_groupingType = RuntimeGroupingObj.GroupingTypes.Sort;
			}
			else if (grouping.IsDetail && grouping.Parent == null && !BuiltinSortOverridden)
			{
				m_groupingType = RuntimeGroupingObj.GroupingTypes.Detail;
			}
			else if (grouping.NaturalGroup)
			{
				m_groupingType = RuntimeGroupingObj.GroupingTypes.NaturalGroup;
			}
			else
			{
				m_groupingType = RuntimeGroupingObj.GroupingTypes.Hash;
			}
			m_grouping = RuntimeGroupingObj.CreateGroupingObj(m_groupingType, this, objectType);
			if (grouping.Filters == null)
			{
				m_dataAction = dataAction;
				m_outerDataAction = dataAction;
			}
			if (grouping.RecursiveAggregates != null)
			{
				m_dataAction |= DataActions.RecursiveAggregates;
			}
			if (grouping.PostSortAggregates != null)
			{
				m_dataAction |= DataActions.PostSortAggregates;
			}
			if (grouping.Parent != null)
			{
				m_parentExpression = new RuntimeExpressionInfo(grouping.Parent, grouping.ParentExprHost, null, 0);
			}
		}

		internal override void GetScopeValues(IReference<IHierarchyObj> targetScopeObj, List<object>[] scopeValues, ref int index)
		{
			if (targetScopeObj == null || this != targetScopeObj.Value())
			{
				if (m_isDetailGroup)
				{
					DetailGetScopeValues(m_outerScope, targetScopeObj, scopeValues, ref index);
				}
				else
				{
					m_outerScope.Value().GetScopeValues(targetScopeObj, scopeValues, ref index);
				}
			}
		}

		internal override bool TargetScopeMatched(int index, bool detailSort)
		{
			if (m_isDetailGroup)
			{
				return DetailTargetScopeMatched(m_hierarchyDef.DataRegionDef, m_outerScope, m_hierarchyDef.IsColumn, index);
			}
			return m_outerScope.Value().TargetScopeMatched(index, detailSort);
		}

		protected abstract void UpdateDataRegionGroupRootInfo();

		internal override void NextRow()
		{
			if (m_hierarchyDef.DataScopeInfo == null || !m_hierarchyDef.DataScopeInfo.NeedsIDC)
			{
				NextRegularRow();
			}
		}

		bool IDataCorrelation.NextCorrelatedRow()
		{
			return NextRegularRow();
		}

		private bool NextRegularRow()
		{
			UpdateDataRegionGroupRootInfo();
			if (!ProcessThisRow())
			{
				return false;
			}
			DomainScopeContext domainScopeContext = base.OdpContext.DomainScopeContext;
			DomainScopeContext.DomainScopeInfo domainScopeInfo = null;
			if (domainScopeContext != null)
			{
				domainScopeInfo = domainScopeContext.CurrentDomainScope;
			}
			if (domainScopeInfo != null)
			{
				domainScopeInfo.MoveNext();
				m_currentGroupExprValue = domainScopeInfo.CurrentKey;
			}
			else if (m_expression != null)
			{
				m_currentGroupExprValue = EvaluateGroupExpression(m_expression, "Group");
			}
			else
			{
				m_currentGroupExprValue = m_odpContext.ReportObjectModel.FieldsImpl.GetRowIndex();
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = m_hierarchyDef.Grouping;
			if (SaveGroupExprValues)
			{
				grouping.CurrentGroupExpressionValues = new List<object>(1);
				grouping.CurrentGroupExpressionValues.Add(m_currentGroupExprValue);
			}
			if (m_isDetailGroup)
			{
				if (m_detailUserSortTargetInfo != null)
				{
					ProcessDetailSort();
				}
				else
				{
					m_grouping.NextRow(m_currentGroupExprValue, hasParent: false, null);
				}
			}
			else
			{
				if (m_odpContext.RuntimeSortFilterInfo != null)
				{
					int count = m_odpContext.RuntimeSortFilterInfo.Count;
					if (grouping.SortFilterScopeMatched == null)
					{
						grouping.SortFilterScopeMatched = new bool[count];
					}
					for (int i = 0; i < count; i++)
					{
						grouping.SortFilterScopeMatched[i] = true;
					}
				}
				MatchSortFilterScope(m_outerScope, grouping, m_currentGroupExprValue, 0);
				object parentKey = null;
				bool flag = m_parentExpression != null;
				if (flag)
				{
					parentKey = EvaluateGroupExpression(m_parentExpression, "Parent");
				}
				m_grouping.NextRow(m_currentGroupExprValue, flag, parentKey);
			}
			domainScopeInfo?.MovePrevious();
			return true;
		}

		protected void ProcessDetailSort()
		{
			if (m_detailUserSortTargetInfo != null && !m_detailUserSortTargetInfo.TargetForNonDetailSort)
			{
				IReference<RuntimeSortFilterEventInfo> reference = m_odpContext.RuntimeSortFilterInfo[m_detailUserSortTargetInfo.SortFilterInfoIndices[0]];
				object sortOrder;
				using (reference.PinValue())
				{
					sortOrder = reference.Value().GetSortOrder(m_odpContext.ReportRuntime);
				}
				m_detailUserSortTargetInfo.SortTree.NextRow(sortOrder, this);
			}
		}

		internal IHierarchyObj CreateDetailSortHierarchyObj(RuntimeGroupLeafObj rootSortDetailLeafObj)
		{
			Global.Tracer.Assert(m_detailUserSortTargetInfo != null, "(null != m_detailUserSortTargetInfo)");
			return new RuntimeSortHierarchyObj(this, base.Depth);
		}

		public override IHierarchyObj CreateHierarchyObjForSortTree()
		{
			if (m_detailUserSortTargetInfo != null)
			{
				return new RuntimeSortHierarchyObj(this, base.Depth);
			}
			return base.CreateHierarchyObjForSortTree();
		}

		protected override void ProcessUserSort()
		{
			if (m_detailUserSortTargetInfo != null)
			{
				m_odpContext.ProcessUserSortForTarget((IReference<IHierarchyObj>)base.SelfReference, ref m_detailDataRows, m_detailUserSortTargetInfo.TargetForNonDetailSort);
			}
			else
			{
				base.ProcessUserSort();
			}
		}

		protected override void MarkSortInfoProcessed(List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo)
		{
			if (m_detailUserSortTargetInfo != null)
			{
				m_detailUserSortTargetInfo.MarkSortInfoProcessed(runtimeSortFilterInfo, (IReference<IHierarchyObj>)base.SelfReference);
			}
			else
			{
				base.MarkSortInfoProcessed(runtimeSortFilterInfo);
			}
		}

		protected override void AddSortInfoIndex(int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
		{
			if (m_detailUserSortTargetInfo != null)
			{
				m_detailUserSortTargetInfo.AddSortInfoIndex(sortInfoIndex, sortInfo);
			}
			else
			{
				base.AddSortInfoIndex(sortInfoIndex, sortInfo);
			}
		}

		private object EvaluateGroupExpression(RuntimeExpressionInfo expression, string propertyName)
		{
			Global.Tracer.Assert(m_hierarchyDef.Grouping != null, "(null != m_hierarchyDef.Grouping)");
			return m_odpContext.ReportRuntime.EvaluateRuntimeExpression(expression, Microsoft.ReportingServices.ReportProcessing.ObjectType.Grouping, m_hierarchyDef.Grouping.Name, propertyName);
		}

		protected bool ProcessThisRow()
		{
			FieldsImpl fieldsImpl = m_odpContext.ReportObjectModel.FieldsImpl;
			if (fieldsImpl.IsAggregateRow && 0 > fieldsImpl.AggregationFieldCount)
			{
				return false;
			}
			int[] groupExpressionFieldIndices = m_hierarchyDef.Grouping.GetGroupExpressionFieldIndices();
			if (groupExpressionFieldIndices == null)
			{
				fieldsImpl.ValidAggregateRow = false;
			}
			else
			{
				foreach (int num in groupExpressionFieldIndices)
				{
					if (-1 > num || (0 <= num && !fieldsImpl[num].IsAggregationField))
					{
						fieldsImpl.ValidAggregateRow = false;
					}
				}
			}
			if (fieldsImpl.IsAggregateRow && !fieldsImpl.ValidAggregateRow)
			{
				return false;
			}
			return true;
		}

		internal void AddChildWithNoParent(RuntimeGroupLeafObjReference child)
		{
			if (RuntimeGroupingObj.GroupingTypes.Sort == m_groupingType)
			{
				using (child.PinValue())
				{
					child.Value().Parent = (RuntimeGroupObjReference)m_selfReference;
				}
			}
			else
			{
				AddChild(child);
			}
		}

		private bool DetermineTraversalDirection()
		{
			bool result = true;
			if (m_detailUserSortTargetInfo != null && IsDetailGroup && GroupOrDetailSortTree != null)
			{
				result = GetDetailSortDirection();
			}
			else if (m_expression != null)
			{
				result = m_expression.Direction;
			}
			return result;
		}

		internal override bool SortAndFilter(AggregateUpdateContext aggContext)
		{
			if (m_odpContext.HasSecondPassOperation(SecondPassOperations.FilteringOrAggregatesOrDomainScope))
			{
				CopyDomainScopeGroupInstancesFromTarget();
			}
			RuntimeGroupingObj grouping = m_grouping;
			bool ascending = DetermineTraversalDirection();
			bool result = true;
			bool processSecondPassSorting = ProcessSecondPassSorting;
			bool flag = (SecondPassOperations.FilteringOrAggregatesOrDomainScope & m_odpContext.SecondPassOperation) != 0 && (m_hierarchyDef.HasFilters || m_hierarchyDef.HasInnerFilters);
			if (processSecondPassSorting)
			{
				m_expression = new RuntimeExpressionInfo(m_hierarchyDef.Sorting.SortExpressions, m_hierarchyDef.Sorting.ExprHost, m_hierarchyDef.Sorting.SortDirections, 0);
				m_groupingType = RuntimeGroupingObj.GroupingTypes.Sort;
				m_grouping = new RuntimeGroupingObjTree(this, m_objectType);
			}
			else if (flag)
			{
				m_groupingType = RuntimeGroupingObj.GroupingTypes.None;
				m_grouping = new RuntimeGroupingObjLinkedList(this, m_objectType);
			}
			if (flag)
			{
				m_groupFilters = new Filters(Filters.FilterTypes.GroupFilter, (IReference<Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>)base.SelfReference, m_hierarchyDef.Grouping.Filters, Microsoft.ReportingServices.ReportProcessing.ObjectType.Grouping, m_hierarchyDef.Grouping.Name, m_odpContext, base.Depth + 1);
			}
			m_processingStage = ProcessingStages.SortAndFilter;
			m_lastChild = null;
			grouping.Traverse(ProcessingStages.SortAndFilter, ascending, aggContext);
			if (flag)
			{
				m_groupFilters.FinishReadingGroups(aggContext);
				if (!processSecondPassSorting && null == m_lastChild)
				{
					if (m_firstChild != null)
					{
						m_firstChild.Free();
					}
					m_firstChild = null;
					result = false;
				}
			}
			if (grouping != m_grouping)
			{
				grouping.Cleanup();
			}
			return result;
		}

		private void CopyDomainScopeGroupInstancesFromTarget()
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = HierarchyDef.Grouping;
			if (grouping != null && grouping.DomainScope != null)
			{
				IReference<RuntimeGroupRootObj> value;
				bool condition = base.OdpContext.DomainScopeContext.DomainScopes.TryGetValue(grouping.ScopeIDForDomainScope, out value);
				Global.Tracer.Assert(condition, "DomainScopes should contain the target group root for the specified group");
				using (value.PinValue())
				{
					RuntimeGroupRootObj runtimeGroupRootObj = value.Value();
					ProcessingStage = ProcessingStages.Grouping;
					runtimeGroupRootObj.m_grouping.CopyDomainScopeGroupInstances(this);
					ProcessingStage = ProcessingStages.SortAndFilter;
				}
			}
		}

		public override void UpdateAggregates(AggregateUpdateContext context)
		{
			m_grouping.Traverse(ProcessingStages.UpdateAggregates, DetermineTraversalDirection(), context);
		}

		void Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner.PostFilterNextRow()
		{
			Global.Tracer.Assert(condition: false);
		}

		internal virtual void AddScopedRunningValue(Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj runningValueObj)
		{
			if (m_scopedRunningValues == null)
			{
				m_scopedRunningValues = new List<string>();
			}
			if (!m_scopedRunningValues.Contains(runningValueObj.Name))
			{
				m_scopedRunningValues.Add(runningValueObj.Name);
			}
		}

		internal override void CalculateRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
			SetupRunningValues(groupCol);
		}

		protected void SetupRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol)
		{
			m_groupCollection = groupCol;
			if (m_hierarchyDef.Grouping.Name != null)
			{
				groupCol[m_hierarchyDef.Grouping.Name] = (RuntimeGroupRootObjReference)m_selfReference;
			}
		}

		protected void AddRunningValues(List<Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo> runningValues)
		{
			AddRunningValues(runningValues, ref m_runningValuesInGroup, ref m_previousValuesInGroup, m_groupCollection, cellRunningValues: false, outermostStatics: false);
		}

		protected void AddRunningValuesOfAggregates()
		{
			if (m_hierarchyDef.DataScopeInfo != null)
			{
				List<string> runningValuesInGroup = null;
				List<string> previousValuesInGroup = null;
				AddRunningValues(m_hierarchyDef.DataScopeInfo.RunningValuesOfAggregates, ref runningValuesInGroup, ref previousValuesInGroup, m_groupCollection, cellRunningValues: false, outermostStatics: false);
			}
		}

		protected bool AddRunningValues(List<Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo> runningValues, ref List<string> runningValuesInGroup, ref List<string> previousValuesInGroup, Dictionary<string, IReference<RuntimeGroupRootObj>> groupCollection, bool cellRunningValues, bool outermostStatics)
		{
			bool result = false;
			if (runningValues == null || 0 >= runningValues.Count)
			{
				return result;
			}
			if (runningValuesInGroup == null)
			{
				runningValuesInGroup = new List<string>();
			}
			if (previousValuesInGroup == null)
			{
				previousValuesInGroup = new List<string>();
			}
			if (cellRunningValues)
			{
				List<int> list = null;
				List<int> list2 = null;
				Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = m_hierarchyDef.DataRegionDef;
				bool isColumn = m_hierarchyDef.IsColumn;
				RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = this as RuntimeDataTablixGroupRootObj;
				if (outermostStatics && ((runtimeDataTablixGroupRootObj != null && runtimeDataTablixGroupRootObj.InnerGroupings != null) || dataRegionDef.CurrentOuterGroupRoot == null))
				{
					if (isColumn)
					{
						list2 = m_hierarchyDef.GetCellIndexes();
						list = dataRegionDef.OutermostStaticRowIndexes;
					}
					else
					{
						list2 = dataRegionDef.OutermostStaticColumnIndexes;
						list = m_hierarchyDef.GetCellIndexes();
					}
				}
				else
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode hierarchyDef = dataRegionDef.CurrentOuterGroupRoot.Value().HierarchyDef;
					if (isColumn)
					{
						list2 = m_hierarchyDef.GetCellIndexes();
						list = ((!outermostStatics) ? hierarchyDef.GetCellIndexes() : dataRegionDef.OutermostStaticRowIndexes);
					}
					else
					{
						list = m_hierarchyDef.GetCellIndexes();
						list2 = ((!outermostStatics) ? hierarchyDef.GetCellIndexes() : dataRegionDef.OutermostStaticColumnIndexes);
					}
				}
				if (list != null && list2 != null)
				{
					foreach (int item in list)
					{
						foreach (int item2 in list2)
						{
							Cell cell = dataRegionDef.Rows[item].Cells[item2];
							if (cell.RunningValueIndexes != null)
							{
								result = true;
								for (int i = 0; i < cell.RunningValueIndexes.Count; i++)
								{
									int index = cell.RunningValueIndexes[i];
									AddRunningValue(runningValues[index], runningValuesInGroup, previousValuesInGroup, groupCollection);
								}
							}
						}
					}
				}
			}
			else
			{
				result = true;
				for (int j = 0; j < runningValues.Count; j++)
				{
					AddRunningValue(runningValues[j], runningValuesInGroup, previousValuesInGroup, groupCollection);
				}
			}
			if (previousValuesInGroup.Count == 0)
			{
				previousValuesInGroup = null;
			}
			if (runningValuesInGroup.Count == 0)
			{
				runningValuesInGroup = null;
			}
			return result;
		}

		private void AddRunningValue(Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValue, List<string> runningValuesInGroup, List<string> previousValuesInGroup, Dictionary<string, IReference<RuntimeGroupRootObj>> groupCollection)
		{
			AggregatesImpl aggregatesImpl = m_odpContext.ReportObjectModel.AggregatesImpl;
			bool flag = runningValue.AggregateType == Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Previous;
			List<string> list = (!flag) ? runningValuesInGroup : previousValuesInGroup;
			Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj dataAggregateObj = aggregatesImpl.GetAggregateObj(runningValue.Name);
			if (dataAggregateObj == null)
			{
				dataAggregateObj = new Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj(runningValue, m_odpContext);
				aggregatesImpl.Add(dataAggregateObj);
			}
			else if (flag && (runningValue.Scope == null || runningValue.IsScopedInEvaluationScope))
			{
				dataAggregateObj.Init();
			}
			if (runningValue.Scope != null)
			{
				if (groupCollection.TryGetValue(runningValue.Scope, out IReference<RuntimeGroupRootObj> value))
				{
					using (value.PinValue())
					{
						value.Value().AddScopedRunningValue(dataAggregateObj);
					}
				}
				else
				{
					Global.Tracer.Assert(condition: false, "RV with runtime scope escalation");
				}
			}
			if (!list.Contains(dataAggregateObj.Name))
			{
				list.Add(dataAggregateObj.Name);
			}
		}

		internal override void CalculatePreviousAggregates()
		{
			if (m_previousValuesInGroup != null)
			{
				AggregatesImpl aggregatesImpl = m_odpContext.ReportObjectModel.AggregatesImpl;
				for (int i = 0; i < m_previousValuesInGroup.Count; i++)
				{
					string text = m_previousValuesInGroup[i];
					Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = aggregatesImpl.GetAggregateObj(text);
					Global.Tracer.Assert(aggregateObj != null, "Missing expected previous aggregate: {0}", text);
					aggregateObj.Update();
				}
			}
			if (m_outerScope != null && (m_outerDataAction & DataActions.PostSortAggregates) != 0)
			{
				using (m_outerScope.PinValue())
				{
					m_outerScope.Value().CalculatePreviousAggregates();
				}
			}
		}

		public override void ReadRow(DataActions dataAction, ITraversalContext context)
		{
			if (DataActions.PostSortAggregates == dataAction && m_runningValuesInGroup != null)
			{
				AggregatesImpl aggregatesImpl = m_odpContext.ReportObjectModel.AggregatesImpl;
				for (int i = 0; i < m_runningValuesInGroup.Count; i++)
				{
					string text = m_runningValuesInGroup[i];
					Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = aggregatesImpl.GetAggregateObj(text);
					Global.Tracer.Assert(aggregateObj != null, "Missing expected running value aggregate: {0}", text);
					aggregateObj.Update();
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

		internal void CreateInstances(ScopeInstance parentInstance, IReference<RuntimeMemberObj>[] innerMembers, IReference<RuntimeDataTablixGroupLeafObj> innerGroupLeafRef)
		{
			CreateInstancesTraversalContext traversalContext = new CreateInstancesTraversalContext(parentInstance, innerMembers, innerGroupLeafRef);
			m_hierarchyDef.ResetInstancePathCascade();
			TraverseGroupOrSortTree(ProcessingStages.CreateGroupTree, traversalContext);
			if (m_detailDataRows != null)
			{
				m_detailDataRows.Dispose();
			}
			m_detailDataRows = null;
			m_detailUserSortTargetInfo = null;
		}

		protected void TraverseGroupOrSortTree(ProcessingStages operation, ITraversalContext traversalContext)
		{
			if (m_detailUserSortTargetInfo != null && m_groupingType != 0)
			{
				m_detailUserSortTargetInfo.SortTree.Traverse(operation, GetDetailSortDirection(), traversalContext);
			}
			else
			{
				m_grouping.Traverse(operation, m_expression == null || m_expression.Direction, traversalContext);
			}
		}

		internal void TraverseLinkedGroupLeaves(ProcessingStages operation, bool ascending, ITraversalContext traversalContext)
		{
			if (null != m_firstChild)
			{
				using (m_firstChild.PinValue())
				{
					m_firstChild.Value().TraverseAllLeafNodes(operation, traversalContext);
				}
			}
		}

		private bool GetDetailSortDirection()
		{
			int index = m_detailUserSortTargetInfo.SortFilterInfoIndices[0];
			return m_odpContext.RuntimeSortFilterInfo[index].Value().SortDirection;
		}

		internal RuntimeGroupLeafObjReference CreateGroupLeaf()
		{
			RuntimeGroupLeafObj runtimeGroupLeafObj = null;
			switch (base.ObjectType)
			{
			case Microsoft.ReportingServices.ReportProcessing.ObjectType.Tablix:
				runtimeGroupLeafObj = new RuntimeTablixGroupLeafObj((RuntimeDataTablixGroupRootObjReference)base.SelfReference, base.ObjectType);
				break;
			case Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel:
			case Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart:
			case Microsoft.ReportingServices.ReportProcessing.ObjectType.CustomReportItem:
			case Microsoft.ReportingServices.ReportProcessing.ObjectType.MapDataRegion:
				runtimeGroupLeafObj = new RuntimeChartCriGroupLeafObj((RuntimeDataTablixGroupRootObjReference)base.SelfReference, base.ObjectType);
				break;
			default:
				Global.Tracer.Assert(condition: false, "Invalid ObjectType");
				break;
			}
			RuntimeGroupLeafObjReference obj = (RuntimeGroupLeafObjReference)runtimeGroupLeafObj.SelfReference;
			obj.UnPinValue();
			return obj;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.HierarchyDef:
				{
					int value2 = scalabilityCache.StoreStaticReference(m_hierarchyDef);
					writer.Write(value2);
					break;
				}
				case MemberName.OuterScope:
					writer.Write(m_outerScope);
					break;
				case MemberName.ProcessingStage:
					writer.WriteEnum((int)m_processingStage);
					break;
				case MemberName.ScopedRunningValues:
					writer.WriteListOfPrimitives(m_scopedRunningValues);
					break;
				case MemberName.RunningValuesInGroup:
					writer.WriteListOfPrimitives(m_runningValuesInGroup);
					break;
				case MemberName.PreviousValuesInGroup:
					writer.WriteListOfPrimitives(m_previousValuesInGroup);
					break;
				case MemberName.GroupCollection:
					writer.WriteStringRIFObjectDictionary(m_groupCollection);
					break;
				case MemberName.DataAction:
					writer.WriteEnum((int)m_dataAction);
					break;
				case MemberName.OuterDataAction:
					writer.WriteEnum((int)m_outerDataAction);
					break;
				case MemberName.GroupingType:
					writer.WriteEnum((int)m_groupingType);
					break;
				case MemberName.Filters:
				{
					int value = scalabilityCache.StoreStaticReference(m_groupFilters);
					writer.Write(value);
					break;
				}
				case MemberName.ParentExpression:
					writer.Write(m_parentExpression);
					break;
				case MemberName.CurrentGroupExprValue:
					writer.Write(m_currentGroupExprValue);
					break;
				case MemberName.SaveGroupExprValues:
					writer.Write(m_saveGroupExprValues);
					break;
				case MemberName.SortFilterExpressionScopeInfoIndices:
					writer.Write(m_sortFilterExpressionScopeInfoIndices);
					break;
				case MemberName.BuiltinSortOverridden:
					writer.Write(m_builtinSortOverridden);
					break;
				case MemberName.IsDetailGroup:
					writer.Write(m_isDetailGroup);
					break;
				case MemberName.DetailUserSortTargetInfo:
					writer.Write(m_detailUserSortTargetInfo);
					break;
				case MemberName.DetailRows:
					writer.Write(m_detailDataRows);
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
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.HierarchyDef:
				{
					int id2 = reader.ReadInt32();
					m_hierarchyDef = (Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode)scalabilityCache.FetchStaticReference(id2);
					break;
				}
				case MemberName.OuterScope:
					m_outerScope = (IReference<IScope>)reader.ReadRIFObject();
					break;
				case MemberName.ProcessingStage:
					m_processingStage = (ProcessingStages)reader.ReadEnum();
					break;
				case MemberName.ScopedRunningValues:
					m_scopedRunningValues = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.RunningValuesInGroup:
					m_runningValuesInGroup = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.PreviousValuesInGroup:
					m_previousValuesInGroup = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.GroupCollection:
					m_groupCollection = reader.ReadStringRIFObjectDictionary<IReference<RuntimeGroupRootObj>>();
					break;
				case MemberName.DataAction:
					m_dataAction = (DataActions)reader.ReadEnum();
					break;
				case MemberName.OuterDataAction:
					m_outerDataAction = (DataActions)reader.ReadEnum();
					break;
				case MemberName.GroupingType:
					m_groupingType = (RuntimeGroupingObj.GroupingTypes)reader.ReadEnum();
					break;
				case MemberName.Filters:
				{
					int id = reader.ReadInt32();
					m_groupFilters = (Filters)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.ParentExpression:
					m_parentExpression = (RuntimeExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CurrentGroupExprValue:
					m_currentGroupExprValue = reader.ReadVariant();
					break;
				case MemberName.SaveGroupExprValues:
					m_saveGroupExprValues = reader.ReadBoolean();
					break;
				case MemberName.SortFilterExpressionScopeInfoIndices:
					m_sortFilterExpressionScopeInfoIndices = reader.ReadInt32Array();
					break;
				case MemberName.BuiltinSortOverridden:
					m_builtinSortOverridden = reader.ReadBooleanArray();
					break;
				case MemberName.IsDetailGroup:
					m_isDetailGroup = reader.ReadBoolean();
					break;
				case MemberName.DetailUserSortTargetInfo:
					m_detailUserSortTargetInfo = (RuntimeUserSortTargetInfo)reader.ReadRIFObject();
					break;
				case MemberName.DetailRows:
					m_detailDataRows = reader.ReadRIFObject<ScalableList<DataFieldRow>>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupRootObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.HierarchyDef, Token.Int32));
				list.Add(new MemberInfo(MemberName.OuterScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IScopeReference));
				list.Add(new MemberInfo(MemberName.ProcessingStage, Token.Enum));
				list.Add(new MemberInfo(MemberName.ScopedRunningValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
				list.Add(new MemberInfo(MemberName.RunningValuesInGroup, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
				list.Add(new MemberInfo(MemberName.PreviousValuesInGroup, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
				list.Add(new MemberInfo(MemberName.GroupCollection, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupRootObjReference));
				list.Add(new MemberInfo(MemberName.DataAction, Token.Enum));
				list.Add(new MemberInfo(MemberName.OuterDataAction, Token.Enum));
				list.Add(new MemberInfo(MemberName.GroupingType, Token.Enum));
				list.Add(new MemberInfo(MemberName.Filters, Token.Int32));
				list.Add(new MemberInfo(MemberName.ParentExpression, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeExpressionInfo));
				list.Add(new MemberInfo(MemberName.CurrentGroupExprValue, Token.Object));
				list.Add(new MemberInfo(MemberName.SaveGroupExprValues, Token.Boolean));
				list.Add(new MemberInfo(MemberName.SortFilterExpressionScopeInfoIndices, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Int32));
				list.Add(new MemberInfo(MemberName.BuiltinSortOverridden, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Boolean));
				list.Add(new MemberInfo(MemberName.IsDetailGroup, Token.Boolean));
				list.Add(new MemberInfo(MemberName.DetailUserSortTargetInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeUserSortTargetInfo));
				list.Add(new MemberInfo(MemberName.DetailRows, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupRootObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObj, list);
			}
			return m_declaration;
		}

		public override void SetReference(IReference selfRef)
		{
			base.SetReference(selfRef);
			m_hierarchyRoot = (RuntimeGroupRootObjReference)selfRef;
		}
	}
}

using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal abstract class ReportHierarchyNode : IDOwner, IRunningValueHolder, IPersistable, ICustomPropertiesHolder, IIndexedInCollection, IRIFReportScope, IInstancePath, IGloballyReferenceable, IGlobalIDOwner, IStaticReferenceable, IRIFDataScope, IRIFReportDataScope
	{
		protected bool m_isColumn;

		protected int m_originalScopeID = -1;

		protected int m_level;

		protected Grouping m_grouping;

		protected Sorting m_sorting;

		protected List<ScopeIDType> m_memberGroupAndSortExpressionFlag;

		protected int m_memberCellIndex;

		protected int m_exprHostID = -1;

		protected int m_rowSpan;

		protected int m_colSpan;

		protected bool m_isAutoSubtotal;

		protected List<RunningValueInfo> m_runningValues;

		protected DataValueList m_customProperties;

		[Reference]
		protected DataRegion m_dataRegionDef;

		private int m_indexInCollection = -1;

		private bool m_needToCacheDataRows;

		private List<IInScopeEventSource> m_inScopeEventSources;

		private byte[] m_textboxesInScope;

		private byte[] m_variablesInScope;

		private int m_hierarchyDynamicIndex = -1;

		private int m_hierarchyPathIndex = -1;

		private GroupingList m_hierarchyParentGroups;

		private DataScopeInfo m_dataScopeInfo;

		private int? m_innerDomainScopeCount;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private IMemberNode m_exprHost;

		[NonSerialized]
		private bool? m_hasInnerFilters;

		[NonSerialized]
		protected int m_cellStartIndex = -1;

		[NonSerialized]
		protected int m_cellEndIndex = -1;

		[NonSerialized]
		private List<int> m_cellIndexes;

		[NonSerialized]
		private Dictionary<string, Grouping>[] m_cellScopes;

		[NonSerialized]
		protected AggregatesImpl m_outermostStaticCellRVCol;

		[NonSerialized]
		protected AggregatesImpl[] m_outermostStaticCellScopedRVCollections;

		[NonSerialized]
		protected AggregatesImpl m_cellRVCol;

		[NonSerialized]
		protected AggregatesImpl[] m_cellScopedRVCollections;

		[NonSerialized]
		protected int m_staticRefId = int.MinValue;

		[NonSerialized]
		private int m_currentMemberIndex = -1;

		[NonSerialized]
		private int m_currentDynamicInstanceCount = -1;

		[NonSerialized]
		private IRIFReportDataScope m_parentReportScope;

		[NonSerialized]
		private IReference<IOnDemandScopeInstance> m_currentStreamingScopeInstance;

		[NonSerialized]
		private IReference<IOnDemandScopeInstance> m_cachedNoRowsStreamingScopeInstance;

		[NonSerialized]
		private List<ReportItem> m_groupScopedContentsForProcessing;

		string IRIFDataScope.Name
		{
			get
			{
				if (m_grouping != null)
				{
					return m_grouping.Name;
				}
				return null;
			}
		}

		Microsoft.ReportingServices.ReportProcessing.ObjectType IRIFDataScope.DataScopeObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.Grouping;

		public DataScopeInfo DataScopeInfo => m_dataScopeInfo;

		internal abstract string RdlElementName
		{
			get;
		}

		internal abstract HierarchyNodeList InnerHierarchy
		{
			get;
		}

		internal bool IsColumn
		{
			get
			{
				return m_isColumn;
			}
			set
			{
				m_isColumn = value;
			}
		}

		internal bool IsDomainScope => m_originalScopeID != -1;

		internal int OriginalScopeID
		{
			get
			{
				return m_originalScopeID;
			}
			set
			{
				m_originalScopeID = value;
			}
		}

		internal int Level
		{
			get
			{
				return m_level;
			}
			set
			{
				m_level = value;
			}
		}

		internal Grouping Grouping
		{
			get
			{
				return m_grouping;
			}
			set
			{
				m_grouping = value;
				if (m_grouping != null)
				{
					m_grouping.Owner = this;
				}
			}
		}

		internal Sorting Sorting
		{
			get
			{
				return m_sorting;
			}
			set
			{
				m_sorting = value;
			}
		}

		internal List<ScopeIDType> MemberGroupAndSortExpressionFlag => m_memberGroupAndSortExpressionFlag;

		internal int MemberCellIndex
		{
			get
			{
				return m_memberCellIndex;
			}
			set
			{
				m_memberCellIndex = value;
			}
		}

		internal int ExprHostID
		{
			get
			{
				return m_exprHostID;
			}
			set
			{
				m_exprHostID = value;
			}
		}

		internal int RowSpan
		{
			get
			{
				return m_rowSpan;
			}
			set
			{
				m_rowSpan = value;
			}
		}

		internal int ColSpan
		{
			get
			{
				return m_colSpan;
			}
			set
			{
				m_colSpan = value;
			}
		}

		internal bool IsAutoSubtotal
		{
			get
			{
				return m_isAutoSubtotal;
			}
			set
			{
				m_isAutoSubtotal = value;
			}
		}

		DataValueList ICustomPropertiesHolder.CustomProperties => m_customProperties;

		IInstancePath ICustomPropertiesHolder.InstancePath => this;

		internal DataValueList CustomProperties
		{
			get
			{
				return m_customProperties;
			}
			set
			{
				m_customProperties = value;
			}
		}

		internal DataRegion DataRegionDef
		{
			get
			{
				return m_dataRegionDef;
			}
			set
			{
				m_dataRegionDef = value;
			}
		}

		internal List<RunningValueInfo> RunningValues
		{
			get
			{
				return m_runningValues;
			}
			set
			{
				m_runningValues = value;
			}
		}

		internal bool IsStatic => m_grouping == null;

		internal bool IsLeaf
		{
			get
			{
				if (InnerHierarchy != null)
				{
					return InnerHierarchy.Count == 0;
				}
				return true;
			}
		}

		internal bool IsInnermostDynamicMember
		{
			get
			{
				if (InnerHierarchy != null)
				{
					return InnerHierarchy.DynamicMembersAtScope.Count == 0;
				}
				return true;
			}
		}

		internal virtual bool IsTablixMember => false;

		internal int CurrentMemberIndex
		{
			get
			{
				return m_currentMemberIndex;
			}
			set
			{
				m_currentMemberIndex = value;
			}
		}

		internal int InstanceCount
		{
			get
			{
				if (IsStatic)
				{
					return 1;
				}
				return m_currentDynamicInstanceCount;
			}
			set
			{
				Global.Tracer.Assert(!IsStatic, "Cannot set instance count on static tablix member");
				m_currentDynamicInstanceCount = value;
			}
		}

		public int IndexInCollection
		{
			get
			{
				return m_indexInCollection;
			}
			set
			{
				m_indexInCollection = value;
			}
		}

		public IndexedInCollectionType IndexedInCollectionType => IndexedInCollectionType.Member;

		internal bool HasInnerDynamic
		{
			get
			{
				if (InnerHierarchy == null)
				{
					return false;
				}
				return InnerHierarchy.DynamicMembersAtScope.Count != 0;
			}
		}

		internal HierarchyNodeList InnerDynamicMembers
		{
			get
			{
				if (InnerHierarchy == null)
				{
					return null;
				}
				return InnerHierarchy.DynamicMembersAtScope;
			}
		}

		internal HierarchyNodeList InnerStaticMembersInSameScope
		{
			get
			{
				if (InnerHierarchy == null)
				{
					return null;
				}
				return InnerHierarchy.StaticMembersInSameScope;
			}
		}

		internal int CellStartIndex
		{
			get
			{
				if (m_cellStartIndex < 0)
				{
					CalculateDependencies();
				}
				return m_cellStartIndex;
			}
		}

		internal int CellEndIndex
		{
			get
			{
				if (m_cellEndIndex < 0)
				{
					CalculateDependencies();
				}
				return m_cellEndIndex;
			}
		}

		internal bool HasFilters
		{
			get
			{
				if (m_grouping != null)
				{
					return m_grouping.Filters != null;
				}
				return false;
			}
		}

		internal bool HasVariables
		{
			get
			{
				if (m_grouping != null)
				{
					return m_grouping.Variables != null;
				}
				return false;
			}
		}

		internal bool HasInnerFilters
		{
			get
			{
				if (!m_hasInnerFilters.HasValue)
				{
					m_hasInnerFilters = false;
					if (InnerHierarchy != null)
					{
						int count = InnerHierarchy.Count;
						int num = 0;
						while (!m_hasInnerFilters.Value && num < count)
						{
							m_hasInnerFilters = InnerHierarchy[num].HasFilters;
							if (!m_hasInnerFilters.Value)
							{
								m_hasInnerFilters = InnerHierarchy[num].HasInnerFilters;
							}
							num++;
						}
					}
				}
				return m_hasInnerFilters.Value;
			}
			set
			{
				m_hasInnerFilters = value;
			}
		}

		internal AggregatesImpl OutermostStaticCellRVCol
		{
			get
			{
				return m_outermostStaticCellRVCol;
			}
			set
			{
				m_outermostStaticCellRVCol = value;
			}
		}

		internal AggregatesImpl[] OutermostStaticCellScopedRVCollections
		{
			get
			{
				return m_outermostStaticCellScopedRVCollections;
			}
			set
			{
				m_outermostStaticCellScopedRVCollections = value;
			}
		}

		internal AggregatesImpl CellRVCol
		{
			get
			{
				return m_cellRVCol;
			}
			set
			{
				m_cellRVCol = value;
			}
		}

		internal AggregatesImpl[] CellScopedRVCollections
		{
			get
			{
				return m_cellScopedRVCollections;
			}
			set
			{
				m_cellScopedRVCollections = value;
			}
		}

		internal List<IInScopeEventSource> InScopeEventSources => m_inScopeEventSources;

		public IRIFReportDataScope ParentReportScope
		{
			get
			{
				if (m_parentReportScope == null)
				{
					m_parentReportScope = IDOwner.FindReportDataScope(base.ParentInstancePath);
				}
				return m_parentReportScope;
			}
		}

		public bool IsDataIntersectionScope => false;

		public bool IsScope => IsGroup;

		public bool IsGroup => !IsStatic;

		public IReference<IOnDemandScopeInstance> CurrentStreamingScopeInstance => m_currentStreamingScopeInstance;

		public bool IsBoundToStreamingScopeInstance => m_currentStreamingScopeInstance != null;

		internal int HierarchyDynamicIndex
		{
			get
			{
				return m_hierarchyDynamicIndex;
			}
			set
			{
				m_hierarchyDynamicIndex = value;
			}
		}

		internal int HierarchyPathIndex
		{
			get
			{
				return m_hierarchyPathIndex;
			}
			set
			{
				m_hierarchyPathIndex = value;
			}
		}

		internal Dictionary<string, Grouping>[] CellScopes
		{
			get
			{
				return m_cellScopes;
			}
			set
			{
				m_cellScopes = value;
			}
		}

		internal GroupingList HierarchyParentGroups
		{
			get
			{
				return m_hierarchyParentGroups;
			}
			set
			{
				m_hierarchyParentGroups = ((value != null && value.Count == 0) ? null : value);
			}
		}

		internal int InnerDomainScopeCount
		{
			get
			{
				if (!m_innerDomainScopeCount.HasValue)
				{
					if (InnerHierarchy == null)
					{
						m_innerDomainScopeCount = 0;
					}
					else
					{
						m_innerDomainScopeCount = InnerHierarchy.Count - InnerHierarchy.OriginalNodeCount;
					}
				}
				return m_innerDomainScopeCount.Value;
			}
		}

		internal List<ReportItem> GroupScopedContentsForProcessing
		{
			get
			{
				if (m_groupScopedContentsForProcessing == null)
				{
					m_groupScopedContentsForProcessing = ComputeMemberScopedItems();
				}
				return m_groupScopedContentsForProcessing;
			}
		}

		internal virtual List<ReportItem> MemberContentCollection => null;

		bool IRIFReportScope.NeedToCacheDataRows
		{
			get
			{
				return m_needToCacheDataRows;
			}
			set
			{
				if (!m_needToCacheDataRows)
				{
					m_needToCacheDataRows = value;
				}
			}
		}

		private bool HasNaturalGroupAndNaturalSort
		{
			get
			{
				if (m_grouping != null && m_sorting != null && m_grouping.NaturalGroup)
				{
					return m_sorting.NaturalSort;
				}
				return false;
			}
		}

		internal virtual bool IsNonToggleableHiddenMember => false;

		int IStaticReferenceable.ID => m_staticRefId;

		internal ReportHierarchyNode()
		{
		}

		internal ReportHierarchyNode(int id, DataRegion dataRegionDef)
			: base(id)
		{
			m_dataRegionDef = dataRegionDef;
			m_runningValues = new List<RunningValueInfo>();
			m_dataScopeInfo = new DataScopeInfo(id);
		}

		bool IRIFReportScope.VariableInScope(int sequenceIndex)
		{
			return SequenceIndex.GetBit(m_variablesInScope, sequenceIndex, returnValueIfSequenceNull: true);
		}

		bool IRIFReportScope.TextboxInScope(int sequenceIndex)
		{
			return SequenceIndex.GetBit(m_textboxesInScope, sequenceIndex, returnValueIfSequenceNull: true);
		}

		public bool IsSameScope(IRIFReportDataScope candidateScope)
		{
			return DataScopeInfo.IsSameScope(candidateScope.DataScopeInfo);
		}

		public bool IsSameOrChildScopeOf(IRIFReportDataScope candidateScope)
		{
			return DataScopeInfo.IsSameOrChildScope(this, candidateScope);
		}

		public bool IsChildScopeOf(IRIFReportDataScope candidateScope)
		{
			return DataScopeInfo.IsChildScopeOf(this, candidateScope);
		}

		public void ResetAggregates(AggregatesImpl reportOmAggregates)
		{
			if (m_grouping != null)
			{
				m_grouping.ResetAggregates(reportOmAggregates);
				reportOmAggregates.ResetAll(m_runningValues);
				if (m_dataScopeInfo != null)
				{
					m_dataScopeInfo.ResetAggregates(reportOmAggregates);
				}
			}
		}

		public bool HasServerAggregate(string aggregateName)
		{
			return DataScopeInfo.ContainsServerAggregate(m_grouping.Aggregates, aggregateName);
		}

		public void BindToStreamingScopeInstance(IReference<IOnDemandScopeInstance> scopeInstance)
		{
			m_currentStreamingScopeInstance = scopeInstance;
		}

		public void BindToNoRowsScopeInstance(OnDemandProcessingContext odpContext)
		{
			if (m_cachedNoRowsStreamingScopeInstance == null)
			{
				StreamingNoRowsMemberInstance member = new StreamingNoRowsMemberInstance(odpContext, this);
				m_cachedNoRowsStreamingScopeInstance = new SyntheticOnDemandMemberInstanceReference(member);
			}
			m_currentStreamingScopeInstance = m_cachedNoRowsStreamingScopeInstance;
		}

		public void ClearStreamingScopeInstanceBinding()
		{
			m_currentStreamingScopeInstance = null;
		}

		internal Dictionary<string, Grouping> GetScopeNames()
		{
			Dictionary<string, Grouping> dictionary = new Dictionary<string, Grouping>();
			int num = (m_hierarchyParentGroups != null) ? m_hierarchyParentGroups.Count : 0;
			for (int i = 0; i < num; i++)
			{
				dictionary.Add(m_hierarchyParentGroups[i].Name, m_hierarchyParentGroups[i]);
			}
			if (!IsStatic)
			{
				dictionary.Add(m_grouping.Name, m_grouping);
			}
			return dictionary;
		}

		protected virtual List<ReportItem> ComputeMemberScopedItems()
		{
			List<ReportItem> results = null;
			RuntimeRICollection.MergeDataProcessingItems(MemberContentCollection, ref results);
			HierarchyNodeList innerStaticMembersInSameScope = InnerStaticMembersInSameScope;
			if (innerStaticMembersInSameScope != null)
			{
				foreach (ReportHierarchyNode item in innerStaticMembersInSameScope)
				{
					RuntimeRICollection.MergeDataProcessingItems(item.MemberContentCollection, ref results);
				}
				return results;
			}
			return results;
		}

		void IRIFReportScope.AddInScopeTextBox(TextBox textbox)
		{
			AddInScopeTextBox(textbox);
		}

		protected virtual void AddInScopeTextBox(TextBox textbox)
		{
		}

		void IRIFReportScope.ResetTextBoxImpls(OnDemandProcessingContext context)
		{
			ResetTextBoxImpls(context);
		}

		internal virtual void ResetTextBoxImpls(OnDemandProcessingContext context)
		{
		}

		void IRIFReportScope.AddInScopeEventSource(IInScopeEventSource eventSource)
		{
			if (m_inScopeEventSources == null)
			{
				m_inScopeEventSources = new List<IInScopeEventSource>();
			}
			m_inScopeEventSources.Add(eventSource);
		}

		internal virtual void TraverseMemberScopes(IRIFScopeVisitor visitor)
		{
		}

		internal virtual bool InnerInitialize(InitializationContext context, bool restrictive)
		{
			bool flag = false;
			if (InnerHierarchy != null)
			{
				bool handledCellContents = context.HandledCellContents;
				context.HandledCellContents = false;
				foreach (ReportHierarchyNode item in InnerHierarchy)
				{
					flag |= item.Initialize(context, restrictive: false);
				}
				context.HandledCellContents = handledCellContents;
			}
			else
			{
				context.MemberCellIndex++;
			}
			return flag;
		}

		internal virtual bool Initialize(InitializationContext context)
		{
			return Initialize(context, restrictive: true);
		}

		internal virtual bool Initialize(InitializationContext context, bool restrictive)
		{
			bool suspendErrors = context.ErrorContext.SuspendErrors;
			context.ErrorContext.SuspendErrors |= m_isAutoSubtotal;
			bool flag = false;
			DataGroupStart(context.ExprHostBuilder);
			bool flag2 = false;
			if (m_grouping != null)
			{
				context.SetIndexInCollection(this);
				if (m_grouping.Variables != null)
				{
					context.RegisterGroupWithVariables(this);
					context.RegisterVariables(m_grouping.Variables);
				}
				m_variablesInScope = context.GetCurrentReferencableVariables();
				flag = true;
				if ((context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDetail) != 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidDetailDataGrouping, Severity.Error, context.ObjectType, context.ObjectName, "Grouping");
				}
				else
				{
					context.Location |= Microsoft.ReportingServices.ReportPublishing.LocationFlags.InGrouping;
					if (m_grouping.IsDetail)
					{
						context.Location |= Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDetail;
					}
					context.RegisterGroupingScope(this);
					flag2 = true;
					m_dataScopeInfo.ValidateScopeRulesForIdc(context, this);
					if (m_grouping.DomainScope != null && !context.IsAncestor(this, m_grouping.DomainScope))
					{
						if (m_grouping.IsClone)
						{
							if (Global.Tracer.TraceVerbose)
							{
								Global.Tracer.Trace(TraceLevel.Verbose, "The grouping '{3}' in the {0} '{1}' has invalid {2} '{4}'. Domain Scope is allowed only if it is an ancestor scope.", m_dataRegionDef.ObjectType, m_dataRegionDef.Name, "DomainScope", m_grouping.Name.MarkAsModelInfo(), m_grouping.DomainScope.MarkAsPrivate());
							}
							m_grouping.DomainScope = null;
							m_grouping.ScopeIDForDomainScope = -1;
						}
						else
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidGroupingDomainScopeNotAncestor, Severity.Error, m_dataRegionDef.ObjectType, m_dataRegionDef.Name, "DomainScope", m_grouping.Name.MarkAsModelInfo(), m_grouping.DomainScope.MarkAsPrivate());
						}
					}
					_ = context.ObjectType;
					_ = context.ObjectName;
					context.ObjectType = Microsoft.ReportingServices.ReportProcessing.ObjectType.Grouping;
					context.ObjectName = m_grouping.Name;
					context.ValidateScopeRulesForNaturalGroup(this);
					context.ValidateScopeRulesForNaturalSort(this);
					if (HasNaturalGroupAndNaturalSort && !ListUtils.IsSubset(m_sorting.SortExpressions, m_grouping.GroupExpressions, RdlExpressionComparer.Instance))
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsIncompatibleNaturalSortAndNaturalGroup, Severity.Error, m_dataRegionDef.ObjectType, m_dataRegionDef.Name, "Group", m_grouping.Name.MarkAsModelInfo());
					}
				}
				m_grouping.Initialize(context);
				if (m_sorting != null)
				{
					m_sorting.Initialize(context);
				}
				InitializeMemberGroupAndSortExpressionFlags();
			}
			if (m_dataScopeInfo != null)
			{
				m_dataScopeInfo.Initialize(context, this);
			}
			if (m_customProperties != null)
			{
				m_customProperties.Initialize(null, context);
			}
			m_memberCellIndex = context.MemberCellIndex;
			flag |= InnerInitialize(context, restrictive);
			if (m_grouping != null)
			{
				if (flag2)
				{
					context.UnRegisterGroupingScope(this);
				}
				if (m_grouping.Variables != null)
				{
					context.UnregisterVariables(m_grouping.Variables);
				}
			}
			m_exprHostID = DataGroupEnd(context.ExprHostBuilder);
			context.ErrorContext.SuspendErrors = suspendErrors;
			return flag;
		}

		private void InitializeMemberGroupAndSortExpressionFlags()
		{
			int capacity = (m_sorting == null) ? m_grouping.GroupExpressions.Count : m_sorting.SortExpressions.Count;
			m_memberGroupAndSortExpressionFlag = new List<ScopeIDType>(capacity);
			if (m_sorting != null)
			{
				for (int i = 0; i < m_sorting.SortExpressions.Count; i++)
				{
					if (ListUtils.Contains(m_grouping.GroupExpressions, m_sorting.SortExpressions[i], RdlExpressionComparer.Instance))
					{
						m_memberGroupAndSortExpressionFlag.Add(ScopeIDType.SortGroup);
					}
					else
					{
						m_memberGroupAndSortExpressionFlag.Add(ScopeIDType.SortValues);
					}
				}
			}
			for (int j = 0; j < m_grouping.GroupExpressions.Count; j++)
			{
				if (m_sorting == null || !ListUtils.Contains(m_sorting.SortExpressions, m_grouping.GroupExpressions[j], RdlExpressionComparer.Instance))
				{
					m_memberGroupAndSortExpressionFlag.Add(ScopeIDType.GroupValues);
				}
			}
		}

		internal virtual bool PreInitializeDataMember(InitializationContext context)
		{
			return false;
		}

		internal virtual void PostInitializeDataMember(InitializationContext context, bool registeredVisibility)
		{
			if (m_grouping != null)
			{
				if (m_grouping.IsAtomic(context) || context.EvaluateAtomicityCondition(m_dataScopeInfo.HasAggregatesOrRunningValues, this, AtomicityReason.Aggregates) || context.EvaluateAtomicityCondition(HasFilters, this, AtomicityReason.Filters) || context.EvaluateAtomicityCondition(m_sorting != null && !m_sorting.NaturalSort, this, AtomicityReason.NonNaturalSorts))
				{
					context.FoundAtomicScope(this);
				}
				else if (context.EvaluateAtomicityCondition(context.HasMultiplePeerChildScopes(this), this, AtomicityReason.PeerChildScopes))
				{
					m_dataScopeInfo.IsDecomposable = true;
					context.FoundAtomicScope(this);
				}
				else
				{
					m_dataScopeInfo.IsDecomposable = true;
				}
			}
		}

		internal void CaptureReferencableTextboxes(InitializationContext context)
		{
			m_textboxesInScope = context.GetCurrentReferencableTextboxes();
		}

		protected abstract void DataGroupStart(Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder builder);

		protected abstract int DataGroupEnd(Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder builder);

		List<RunningValueInfo> IRunningValueHolder.GetRunningValueList()
		{
			return m_runningValues;
		}

		void IRunningValueHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(m_runningValues != null, "(null != m_runningValues)");
			if (m_runningValues.Count == 0)
			{
				m_runningValues.Clear();
			}
		}

		internal virtual void InitializeRVDirectionDependentItems(InitializationContext context)
		{
		}

		internal virtual void DetermineGroupingExprValueCount(InitializationContext context, int groupingExprCount)
		{
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			return PublishClone(context, null, isSubtotal: false);
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context, DataRegion newContainingRegion)
		{
			return PublishClone(context, newContainingRegion, isSubtotal: false);
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context, DataRegion newContainingRegion, bool isSubtotal)
		{
			ReportHierarchyNode reportHierarchyNode = (ReportHierarchyNode)base.PublishClone(context);
			reportHierarchyNode.m_dataScopeInfo = m_dataScopeInfo.PublishClone(context, reportHierarchyNode.ID);
			context.AddRunningValueHolder(reportHierarchyNode);
			if (isSubtotal)
			{
				reportHierarchyNode.m_grouping = null;
				reportHierarchyNode.m_sorting = null;
			}
			else
			{
				if (m_grouping != null)
				{
					reportHierarchyNode.m_grouping = (Grouping)m_grouping.PublishClone(context, reportHierarchyNode);
				}
				if (m_sorting != null)
				{
					reportHierarchyNode.m_sorting = (Sorting)m_sorting.PublishClone(context);
				}
			}
			if (m_customProperties != null)
			{
				reportHierarchyNode.m_customProperties = new DataValueList(m_customProperties.Count);
				foreach (DataValue customProperty in m_customProperties)
				{
					reportHierarchyNode.m_customProperties.Add(customProperty.PublishClone(context));
				}
			}
			if (newContainingRegion != null)
			{
				reportHierarchyNode.m_dataRegionDef = newContainingRegion;
			}
			return reportHierarchyNode;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.IsColumn, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Level, Token.Int32));
			list.Add(new MemberInfo(MemberName.Grouping, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Grouping));
			list.Add(new MemberInfo(MemberName.Sorting, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Sorting));
			list.Add(new MemberInfo(MemberName.MemberCellIndex, Token.Int32));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.RowSpan, Token.Int32));
			list.Add(new MemberInfo(MemberName.ColSpan, Token.Int32));
			list.Add(new MemberInfo(MemberName.AutoSubtotal, Token.Boolean));
			list.Add(new MemberInfo(MemberName.RunningValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RunningValueInfo));
			list.Add(new MemberInfo(MemberName.CustomProperties, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataValue));
			list.Add(new MemberInfo(MemberName.DataRegionDef, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion, Token.Reference));
			list.Add(new MemberInfo(MemberName.IndexInCollection, Token.Int32));
			list.Add(new MemberInfo(MemberName.NeedToCacheDataRows, Token.Boolean));
			list.Add(new MemberInfo(MemberName.InScopeEventSources, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IInScopeEventSource));
			list.Add(new MemberInfo(MemberName.HierarchyDynamicIndex, Token.Int32));
			list.Add(new MemberInfo(MemberName.HierarchyPathIndex, Token.Int32));
			list.Add(new MemberInfo(MemberName.HierarchyParentGroups, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Grouping));
			list.Add(new MemberInfo(MemberName.TextboxesInScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.VariablesInScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.DataScopeInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataScopeInfo));
			list.Add(new MemberInfo(MemberName.OriginalScopeID, Token.Int32));
			list.Add(new MemberInfo(MemberName.InnerDomainScopeCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.MemberGroupAndSortExpressionFlag, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportHierarchyNode, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.IsColumn:
					writer.Write(m_isColumn);
					break;
				case MemberName.OriginalScopeID:
					writer.Write(m_originalScopeID);
					break;
				case MemberName.Level:
					writer.Write(m_level);
					break;
				case MemberName.Grouping:
					writer.Write(m_grouping);
					break;
				case MemberName.Sorting:
					writer.Write(m_sorting);
					break;
				case MemberName.MemberCellIndex:
					writer.Write(m_memberCellIndex);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.RowSpan:
					writer.Write(m_rowSpan);
					break;
				case MemberName.ColSpan:
					writer.Write(m_colSpan);
					break;
				case MemberName.AutoSubtotal:
					writer.Write(m_isAutoSubtotal);
					break;
				case MemberName.RunningValues:
					writer.Write(m_runningValues);
					break;
				case MemberName.CustomProperties:
					writer.Write(m_customProperties);
					break;
				case MemberName.DataRegionDef:
					writer.WriteReference(m_dataRegionDef);
					break;
				case MemberName.IndexInCollection:
					writer.Write(m_indexInCollection);
					break;
				case MemberName.NeedToCacheDataRows:
					writer.Write(m_needToCacheDataRows);
					break;
				case MemberName.InScopeEventSources:
					writer.WriteListOfReferences(m_inScopeEventSources);
					break;
				case MemberName.HierarchyDynamicIndex:
					writer.Write(m_hierarchyDynamicIndex);
					break;
				case MemberName.HierarchyPathIndex:
					writer.Write(m_hierarchyPathIndex);
					break;
				case MemberName.HierarchyParentGroups:
					writer.WriteListOfReferences(m_hierarchyParentGroups);
					break;
				case MemberName.TextboxesInScope:
					writer.Write(m_textboxesInScope);
					break;
				case MemberName.VariablesInScope:
					writer.Write(m_variablesInScope);
					break;
				case MemberName.DataScopeInfo:
					writer.Write(m_dataScopeInfo);
					break;
				case MemberName.InnerDomainScopeCount:
					writer.Write(InnerDomainScopeCount);
					break;
				case MemberName.MemberGroupAndSortExpressionFlag:
					writer.WriteListOfPrimitives(m_memberGroupAndSortExpressionFlag);
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
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.IsColumn:
					m_isColumn = reader.ReadBoolean();
					break;
				case MemberName.OriginalScopeID:
					m_originalScopeID = reader.ReadInt32();
					break;
				case MemberName.Level:
					m_level = reader.ReadInt32();
					break;
				case MemberName.Grouping:
					Grouping = (Grouping)reader.ReadRIFObject();
					break;
				case MemberName.Sorting:
					m_sorting = (Sorting)reader.ReadRIFObject();
					break;
				case MemberName.MemberCellIndex:
					m_memberCellIndex = reader.ReadInt32();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.RowSpan:
					m_rowSpan = reader.ReadInt32();
					break;
				case MemberName.ColSpan:
					m_colSpan = reader.ReadInt32();
					break;
				case MemberName.AutoSubtotal:
					m_isAutoSubtotal = reader.ReadBoolean();
					break;
				case MemberName.RunningValues:
					m_runningValues = reader.ReadGenericListOfRIFObjects<RunningValueInfo>();
					break;
				case MemberName.CustomProperties:
					m_customProperties = reader.ReadListOfRIFObjects<DataValueList>();
					break;
				case MemberName.DataRegionDef:
					m_dataRegionDef = reader.ReadReference<DataRegion>(this);
					break;
				case MemberName.IndexInCollection:
					m_indexInCollection = reader.ReadInt32();
					break;
				case MemberName.NeedToCacheDataRows:
					m_needToCacheDataRows = reader.ReadBoolean();
					break;
				case MemberName.InScopeEventSources:
					m_inScopeEventSources = reader.ReadGenericListOfReferences<IInScopeEventSource>(this);
					break;
				case MemberName.HierarchyDynamicIndex:
					m_hierarchyDynamicIndex = reader.ReadInt32();
					break;
				case MemberName.HierarchyPathIndex:
					m_hierarchyPathIndex = reader.ReadInt32();
					break;
				case MemberName.HierarchyParentGroups:
					m_hierarchyParentGroups = reader.ReadListOfReferences<GroupingList, Grouping>(this);
					break;
				case MemberName.TextboxesInScope:
					m_textboxesInScope = reader.ReadByteArray();
					break;
				case MemberName.VariablesInScope:
					m_variablesInScope = reader.ReadByteArray();
					break;
				case MemberName.DataScopeInfo:
					m_dataScopeInfo = (DataScopeInfo)reader.ReadRIFObject();
					break;
				case MemberName.InnerDomainScopeCount:
					m_innerDomainScopeCount = reader.ReadInt32();
					break;
				case MemberName.MemberGroupAndSortExpressionFlag:
					m_memberGroupAndSortExpressionFlag = reader.ReadListOfPrimitives<ScopeIDType>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item2 in value)
			{
				switch (item2.MemberName)
				{
				case MemberName.DataRegionDef:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item2.RefID));
					Global.Tracer.Assert(((ReportItem)referenceableItems[item2.RefID]).IsDataRegion);
					Global.Tracer.Assert(m_dataRegionDef != (DataRegion)referenceableItems[item2.RefID]);
					m_dataRegionDef = (DataRegion)referenceableItems[item2.RefID];
					break;
				case MemberName.InScopeEventSources:
				{
					referenceableItems.TryGetValue(item2.RefID, out IReferenceable value2);
					IInScopeEventSource item = (IInScopeEventSource)value2;
					if (m_inScopeEventSources == null)
					{
						m_inScopeEventSources = new List<IInScopeEventSource>();
					}
					m_inScopeEventSources.Add(item);
					break;
				}
				case MemberName.HierarchyParentGroups:
					if (m_hierarchyParentGroups == null)
					{
						m_hierarchyParentGroups = new GroupingList();
					}
					if (item2.RefID != -2)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item2.RefID));
						Global.Tracer.Assert(referenceableItems[item2.RefID] is Grouping);
						Global.Tracer.Assert(!m_hierarchyParentGroups.Contains((Grouping)referenceableItems[item2.RefID]));
						m_hierarchyParentGroups.Add((Grouping)referenceableItems[item2.RefID]);
					}
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportHierarchyNode;
		}

		internal abstract void SetExprHost(IMemberNode memberExprHost, ObjectModelImpl reportObjectModel);

		protected void MemberNodeSetExprHost(IMemberNode exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null, "(null != exprHost)");
			m_exprHost = exprHost;
			if (m_exprHost.GroupHost != null)
			{
				Global.Tracer.Assert(m_grouping != null, "(null != m_grouping)");
				m_grouping.SetExprHost(m_exprHost.GroupHost, reportObjectModel);
			}
			if (m_exprHost.SortHost != null)
			{
				Global.Tracer.Assert(m_sorting != null, "(null != m_sorting)");
				m_sorting.SetExprHost(m_exprHost.SortHost, reportObjectModel);
			}
			if (m_exprHost.CustomPropertyHostsRemotable != null)
			{
				Global.Tracer.Assert(m_customProperties != null, "(null != m_customProperties)");
				m_customProperties.SetExprHost(m_exprHost.CustomPropertyHostsRemotable, reportObjectModel);
			}
			if (m_dataScopeInfo != null && m_dataScopeInfo.JoinInfo != null && m_exprHost.JoinConditionExprHostsRemotable != null)
			{
				m_dataScopeInfo.JoinInfo.SetJoinConditionExprHost(m_exprHost.JoinConditionExprHostsRemotable, reportObjectModel);
			}
		}

		internal abstract void MemberContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions);

		private void CalculateDependencies()
		{
			m_cellStartIndex = (m_cellEndIndex = m_memberCellIndex);
			if (InnerHierarchy != null)
			{
				GetCellIndexes(InnerStaticMembersInSameScope, ref m_cellStartIndex, ref m_cellEndIndex);
			}
		}

		internal List<int> GetCellIndexes()
		{
			if (m_cellIndexes == null)
			{
				if (InnerStaticMembersInSameScope != null && InnerStaticMembersInSameScope.Count != 0 && InnerStaticMembersInSameScope.LeafCellIndexes != null)
				{
					m_cellIndexes = InnerStaticMembersInSameScope.LeafCellIndexes;
				}
				else
				{
					List<int> list = new List<int>(1);
					list.Add(m_memberCellIndex);
					m_cellIndexes = list;
				}
			}
			return m_cellIndexes;
		}

		private static void GetCellIndexes(HierarchyNodeList innerStaticMemberList, ref int cellStartIndex, ref int cellEndIndex)
		{
			if (innerStaticMemberList == null)
			{
				return;
			}
			foreach (ReportHierarchyNode innerStaticMember in innerStaticMemberList)
			{
				if (innerStaticMember.InnerHierarchy == null)
				{
					cellStartIndex = Math.Min(cellStartIndex, innerStaticMember.MemberCellIndex);
					cellEndIndex = Math.Max(cellEndIndex, innerStaticMember.MemberCellIndex);
				}
			}
		}

		internal void ResetInstancePathCascade()
		{
			if (m_grouping == null)
			{
				return;
			}
			base.InstancePathItem.ResetContext();
			HierarchyNodeList innerDynamicMembers = InnerDynamicMembers;
			if (innerDynamicMembers != null)
			{
				for (int i = 0; i < innerDynamicMembers.Count; i++)
				{
					innerDynamicMembers[i].InstancePathItem.ResetContext();
				}
			}
		}

		internal virtual void MoveNextForUserSort(OnDemandProcessingContext odpContext)
		{
			if (m_grouping != null)
			{
				base.InstancePathItem.MoveNext();
			}
		}

		internal void SetUserSortDetailRowIndex(OnDemandProcessingContext odpContext)
		{
			if (m_grouping != null && m_grouping.IsDetail)
			{
				int rowIndex = odpContext.ReportObjectModel.FieldsImpl.GetRowIndex();
				if (m_isColumn)
				{
					m_dataRegionDef.CurrentColDetailIndex = rowIndex;
				}
				else
				{
					m_dataRegionDef.CurrentRowDetailIndex = rowIndex;
				}
			}
		}

		internal virtual void SetMemberInstances(IList<DataRegionMemberInstance> memberInstances)
		{
		}

		internal virtual void SetRecursiveParentIndex(int parentInstanceIndex)
		{
		}

		internal virtual void SetInstanceHasRecursiveChildren(bool? hasRecursiveChildren)
		{
		}

		protected override InstancePathItem CreateInstancePathItem()
		{
			if (IsStatic)
			{
				return new InstancePathItem();
			}
			if (IsColumn)
			{
				return new InstancePathItem(InstancePathItemType.ColumnMemberInstanceIndex, IndexInCollection);
			}
			return new InstancePathItem(InstancePathItemType.RowMemberInstanceIndex, IndexInCollection);
		}

		void IStaticReferenceable.SetID(int id)
		{
			m_staticRefId = id;
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IStaticReferenceable.GetObjectType()
		{
			return GetObjectType();
		}
	}
}

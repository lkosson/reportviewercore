using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal abstract class DataRegion : ReportItem, IPageBreakOwner, IAggregateHolder, IRunningValueHolder, ISortFilterScope, IPersistable, IReferenceable, IIndexedInCollection, IRIFReportScope, IInstancePath, IGloballyReferenceable, IGlobalIDOwner, IRIFDataScope, IDomainScopeMemberCreator, IRIFReportDataScope
	{
		internal enum ProcessingInnerGroupings
		{
			Column,
			Row
		}

		protected string m_dataSetName;

		protected ExpressionInfo m_noRowsMessage;

		protected int m_columnCount;

		protected int m_rowCount;

		protected List<int> m_repeatSiblings;

		protected ProcessingInnerGroupings m_processingInnerGrouping;

		protected Sorting m_sorting;

		protected List<Filter> m_filters;

		protected List<DataAggregateInfo> m_aggregates;

		protected List<DataAggregateInfo> m_postSortAggregates;

		protected List<RunningValueInfo> m_runningValues;

		protected List<DataAggregateInfo> m_cellAggregates;

		protected List<DataAggregateInfo> m_cellPostSortAggregates;

		protected List<RunningValueInfo> m_cellRunningValues;

		protected List<ExpressionInfo> m_userSortExpressions;

		private byte[] m_textboxesInScope;

		private byte[] m_variablesInScope;

		private bool m_needToCacheDataRows;

		private List<IInScopeEventSource> m_inScopeEventSources;

		protected InScopeSortFilterHashtable m_detailSortFiltersInScope;

		protected int m_indexInCollection = -1;

		protected int m_outerGroupingMaximumDynamicLevel;

		protected int m_outerGroupingDynamicMemberCount;

		protected int m_outerGroupingDynamicPathCount;

		protected int m_innerGroupingMaximumDynamicLevel;

		protected int m_innerGroupingDynamicMemberCount;

		protected int m_innerGroupingDynamicPathCount;

		protected PageBreak m_pageBreak;

		protected ExpressionInfo m_pageName;

		protected DataScopeInfo m_dataScopeInfo;

		private int? m_rowDomainScopeCount;

		private int? m_colDomainScopeCount;

		private bool m_isMatrixIDC;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private bool m_rowScopeFound;

		[NonSerialized]
		private bool m_columnScopeFound;

		[NonSerialized]
		private bool m_hasDynamicColumnMember;

		[NonSerialized]
		private bool m_hasDynamicRowMember;

		[NonSerialized]
		private InitializationContext.ScopeChainInfo m_scopeChainInfo;

		[NonSerialized]
		protected DataSet m_cachedDataSet;

		[NonSerialized]
		protected PageBreakStates m_pagebreakState;

		[NonSerialized]
		protected RuntimeDataRegionObjReference m_runtimeDataRegionObj;

		[NonSerialized]
		protected List<int> m_outermostStaticColumnIndexes;

		[NonSerialized]
		protected List<int> m_outermostStaticRowIndexes;

		[NonSerialized]
		protected int m_currentCellInnerIndex;

		[NonSerialized]
		protected int m_sequentialColMemberInstanceIndex;

		[NonSerialized]
		protected int m_sequentialRowMemberInstanceIndex;

		[NonSerialized]
		protected Hashtable m_scopeNames;

		[NonSerialized]
		protected bool m_inTablixCell;

		[NonSerialized]
		protected bool[] m_isSortFilterTarget;

		[NonSerialized]
		protected bool[] m_isSortFilterExpressionScope;

		[NonSerialized]
		protected int[] m_sortFilterSourceDetailScopeInfo;

		[NonSerialized]
		protected int m_currentColDetailIndex = -1;

		[NonSerialized]
		protected int m_currentRowDetailIndex = -1;

		[NonSerialized]
		protected bool m_noRows;

		[NonSerialized]
		protected bool m_processCellRunningValues;

		[NonSerialized]
		protected bool m_processOutermostStaticCellRunningValues;

		[NonSerialized]
		private bool m_inOutermostStaticCells;

		[NonSerialized]
		protected DataRegionInstance m_currentDataRegionInstance;

		[NonSerialized]
		protected AggregateRowInfo m_dataTablixAggregateRowInfo;

		[NonSerialized]
		protected AggregateRowInfo[] m_outerGroupingAggregateRowInfo;

		[NonSerialized]
		protected int[] m_outerGroupingIndexes;

		[NonSerialized]
		protected IReference<RuntimeDataTablixGroupRootObj>[] m_currentOuterGroupRootObjs;

		[NonSerialized]
		protected IReference<RuntimeDataTablixGroupRootObj> m_currentOuterGroupRoot;

		[NonSerialized]
		private bool m_populatedParentReportScope;

		[NonSerialized]
		private IRIFReportDataScope m_parentReportScope;

		[NonSerialized]
		private IReference<IOnDemandScopeInstance> m_currentStreamingScopeInstance;

		[NonSerialized]
		private IReference<IOnDemandScopeInstance> m_cachedNoRowsStreamingScopeInstance;

		[NonSerialized]
		private List<ReportItem> m_dataRegionScopedItemsForDataProcessing;

		string IRIFDataScope.Name => base.Name;

		Microsoft.ReportingServices.ReportProcessing.ObjectType IRIFDataScope.DataScopeObjectType => ObjectType;

		internal bool IsMatrixIDC
		{
			get
			{
				return m_isMatrixIDC;
			}
			set
			{
				m_isMatrixIDC = value;
			}
		}

		internal override bool IsDataRegion => true;

		internal abstract HierarchyNodeList ColumnMembers
		{
			get;
		}

		internal abstract HierarchyNodeList RowMembers
		{
			get;
		}

		internal HierarchyNodeList OuterMembers
		{
			get
			{
				if (m_processingInnerGrouping == ProcessingInnerGroupings.Column)
				{
					return RowMembers;
				}
				return ColumnMembers;
			}
		}

		internal HierarchyNodeList InnerMembers
		{
			get
			{
				if (m_processingInnerGrouping == ProcessingInnerGroupings.Column)
				{
					return ColumnMembers;
				}
				return RowMembers;
			}
		}

		internal abstract RowList Rows
		{
			get;
		}

		internal string DataSetName
		{
			get
			{
				return m_dataSetName;
			}
			set
			{
				m_dataSetName = value;
			}
		}

		internal bool NoRows
		{
			get
			{
				return m_noRows;
			}
			set
			{
				m_noRows = value;
			}
		}

		internal ExpressionInfo NoRowsMessage
		{
			get
			{
				return m_noRowsMessage;
			}
			set
			{
				m_noRowsMessage = value;
			}
		}

		internal int ColumnCount
		{
			get
			{
				return m_columnCount;
			}
			set
			{
				m_columnCount = value;
			}
		}

		internal int RowCount
		{
			get
			{
				return m_rowCount;
			}
			set
			{
				m_rowCount = value;
			}
		}

		internal ProcessingInnerGroupings ProcessingInnerGrouping
		{
			get
			{
				return m_processingInnerGrouping;
			}
			set
			{
				m_processingInnerGrouping = value;
			}
		}

		internal List<int> RepeatSiblings
		{
			get
			{
				return m_repeatSiblings;
			}
			set
			{
				m_repeatSiblings = value;
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

		internal List<Filter> Filters
		{
			get
			{
				return m_filters;
			}
			set
			{
				m_filters = value;
			}
		}

		internal bool HasFilters
		{
			get
			{
				if (m_filters != null)
				{
					return m_filters.Count > 0;
				}
				return false;
			}
		}

		internal List<DataAggregateInfo> Aggregates
		{
			get
			{
				return m_aggregates;
			}
			set
			{
				m_aggregates = value;
			}
		}

		internal List<DataAggregateInfo> PostSortAggregates
		{
			get
			{
				return m_postSortAggregates;
			}
			set
			{
				m_postSortAggregates = value;
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

		internal List<DataAggregateInfo> CellAggregates
		{
			get
			{
				return m_cellAggregates;
			}
			set
			{
				m_cellAggregates = value;
			}
		}

		internal List<DataAggregateInfo> CellPostSortAggregates
		{
			get
			{
				return m_cellPostSortAggregates;
			}
			set
			{
				m_cellPostSortAggregates = value;
			}
		}

		internal List<RunningValueInfo> CellRunningValues
		{
			get
			{
				return m_cellRunningValues;
			}
			set
			{
				m_cellRunningValues = value;
			}
		}

		internal List<ExpressionInfo> UserSortExpressions
		{
			get
			{
				return m_userSortExpressions;
			}
			set
			{
				m_userSortExpressions = value;
			}
		}

		internal InScopeSortFilterHashtable DetailSortFiltersInScope
		{
			get
			{
				return m_detailSortFiltersInScope;
			}
			set
			{
				m_detailSortFiltersInScope = value;
			}
		}

		internal Hashtable ScopeNames
		{
			get
			{
				return m_scopeNames;
			}
			set
			{
				m_scopeNames = value;
			}
		}

		public IRIFReportDataScope ParentReportScope
		{
			get
			{
				if (!m_populatedParentReportScope)
				{
					m_parentReportScope = IDOwner.FindReportDataScope(base.ParentInstancePath);
					m_populatedParentReportScope = true;
				}
				return m_parentReportScope;
			}
		}

		public bool IsDataIntersectionScope => false;

		public bool IsScope => IsDataRegion;

		public bool IsGroup => false;

		public virtual bool IsColumnGroupingSwitched => false;

		public IReference<IOnDemandScopeInstance> CurrentStreamingScopeInstance => m_currentStreamingScopeInstance;

		public bool IsBoundToStreamingScopeInstance => m_currentStreamingScopeInstance != null;

		internal RuntimeDataRegionObjReference RuntimeDataRegionObj
		{
			get
			{
				return m_runtimeDataRegionObj;
			}
			set
			{
				m_runtimeDataRegionObj = value;
			}
		}

		internal List<int> OutermostStaticColumnIndexes
		{
			get
			{
				return m_outermostStaticColumnIndexes;
			}
			set
			{
				m_outermostStaticColumnIndexes = value;
			}
		}

		internal List<int> OutermostStaticRowIndexes
		{
			get
			{
				return m_outermostStaticRowIndexes;
			}
			set
			{
				m_outermostStaticRowIndexes = value;
			}
		}

		internal int CurrentCellInnerIndex => m_currentCellInnerIndex;

		internal IReference<RuntimeDataTablixGroupRootObj> CurrentOuterGroupRoot
		{
			get
			{
				return m_currentOuterGroupRoot;
			}
			set
			{
				m_currentOuterGroupRoot = value;
			}
		}

		internal IReference<RuntimeDataTablixGroupRootObj>[] CurrentOuterGroupRootObjs
		{
			get
			{
				return m_currentOuterGroupRootObjs;
			}
			set
			{
				m_currentOuterGroupRootObjs = value;
			}
		}

		internal int[] OuterGroupingIndexes => m_outerGroupingIndexes;

		internal bool InTablixCell
		{
			get
			{
				return m_inTablixCell;
			}
			set
			{
				m_inTablixCell = value;
			}
		}

		internal bool[] IsSortFilterTarget
		{
			get
			{
				return m_isSortFilterTarget;
			}
			set
			{
				m_isSortFilterTarget = value;
			}
		}

		internal bool[] IsSortFilterExpressionScope
		{
			get
			{
				return m_isSortFilterExpressionScope;
			}
			set
			{
				m_isSortFilterExpressionScope = value;
			}
		}

		internal int[] SortFilterSourceDetailScopeInfo
		{
			get
			{
				return m_sortFilterSourceDetailScopeInfo;
			}
			set
			{
				m_sortFilterSourceDetailScopeInfo = value;
			}
		}

		internal int CurrentColDetailIndex
		{
			get
			{
				return m_currentColDetailIndex;
			}
			set
			{
				m_currentColDetailIndex = value;
			}
		}

		internal int CurrentRowDetailIndex
		{
			get
			{
				return m_currentRowDetailIndex;
			}
			set
			{
				m_currentRowDetailIndex = value;
			}
		}

		internal bool ProcessCellRunningValues
		{
			get
			{
				return m_processCellRunningValues;
			}
			set
			{
				m_processCellRunningValues = value;
			}
		}

		internal bool ProcessOutermostStaticCellRunningValues
		{
			get
			{
				return m_processOutermostStaticCellRunningValues;
			}
			set
			{
				m_processOutermostStaticCellRunningValues = value;
			}
		}

		internal bool InOutermostStaticCells
		{
			get
			{
				return m_inOutermostStaticCells;
			}
			set
			{
				m_inOutermostStaticCells = value;
			}
		}

		int ISortFilterScope.ID => m_ID;

		string ISortFilterScope.ScopeName => m_name;

		bool[] ISortFilterScope.IsSortFilterTarget
		{
			get
			{
				return m_isSortFilterTarget;
			}
			set
			{
				m_isSortFilterTarget = value;
			}
		}

		bool[] ISortFilterScope.IsSortFilterExpressionScope
		{
			get
			{
				return m_isSortFilterExpressionScope;
			}
			set
			{
				m_isSortFilterExpressionScope = value;
			}
		}

		List<ExpressionInfo> ISortFilterScope.UserSortExpressions
		{
			get
			{
				return m_userSortExpressions;
			}
			set
			{
				m_userSortExpressions = value;
			}
		}

		IndexedExprHost ISortFilterScope.UserSortExpressionsHost => UserSortExpressionsHost;

		protected abstract IndexedExprHost UserSortExpressionsHost
		{
			get;
		}

		internal bool ColumnScopeFound
		{
			get
			{
				return m_columnScopeFound;
			}
			set
			{
				m_columnScopeFound = value;
			}
		}

		internal bool RowScopeFound
		{
			get
			{
				return m_rowScopeFound;
			}
			set
			{
				m_rowScopeFound = value;
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

		public IndexedInCollectionType IndexedInCollectionType => IndexedInCollectionType.DataRegion;

		internal DataRegionInstance CurrentDataRegionInstance
		{
			get
			{
				return m_currentDataRegionInstance;
			}
			set
			{
				m_currentDataRegionInstance = value;
			}
		}

		internal List<IInScopeEventSource> InScopeEventSources => m_inScopeEventSources;

		internal int OuterGroupingMaximumDynamicLevel => m_outerGroupingMaximumDynamicLevel;

		internal int OuterGroupingDynamicMemberCount => m_outerGroupingDynamicMemberCount;

		internal int OuterGroupingDynamicPathCount => m_outerGroupingDynamicPathCount;

		internal InitializationContext.ScopeChainInfo ScopeChainInfo
		{
			get
			{
				return m_scopeChainInfo;
			}
			set
			{
				m_scopeChainInfo = value;
			}
		}

		internal int InnerGroupingMaximumDynamicLevel => m_innerGroupingMaximumDynamicLevel;

		internal int InnerGroupingDynamicMemberCount => m_innerGroupingDynamicMemberCount;

		internal int InnerGroupingDynamicPathCount => m_innerGroupingDynamicPathCount;

		internal int RowDomainScopeCount
		{
			get
			{
				if (RowMembers == null)
				{
					m_rowDomainScopeCount = 0;
				}
				else if (!m_rowDomainScopeCount.HasValue)
				{
					m_rowDomainScopeCount = RowMembers.Count - RowMembers.OriginalNodeCount;
				}
				return m_rowDomainScopeCount.Value;
			}
		}

		internal int ColumnDomainScopeCount
		{
			get
			{
				if (ColumnMembers == null)
				{
					m_colDomainScopeCount = 0;
				}
				else if (!m_colDomainScopeCount.HasValue)
				{
					m_colDomainScopeCount = ColumnMembers.Count - ColumnMembers.OriginalNodeCount;
				}
				return m_colDomainScopeCount.Value;
			}
		}

		internal List<ReportItem> DataRegionScopedItemsForDataProcessing
		{
			get
			{
				if (m_dataRegionScopedItemsForDataProcessing == null)
				{
					m_dataRegionScopedItemsForDataProcessing = ComputeDataRegionScopedItemsForDataProcessing();
				}
				return m_dataRegionScopedItemsForDataProcessing;
			}
		}

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

		public DataScopeInfo DataScopeInfo => m_dataScopeInfo;

		internal ExpressionInfo PageName
		{
			get
			{
				return m_pageName;
			}
			set
			{
				m_pageName = value;
			}
		}

		internal PageBreak PageBreak
		{
			get
			{
				return m_pageBreak;
			}
			set
			{
				m_pageBreak = value;
			}
		}

		PageBreak IPageBreakOwner.PageBreak
		{
			get
			{
				return m_pageBreak;
			}
			set
			{
				m_pageBreak = value;
			}
		}

		Microsoft.ReportingServices.ReportProcessing.ObjectType IPageBreakOwner.ObjectType => ObjectType;

		string IPageBreakOwner.ObjectName => m_name;

		IInstancePath IPageBreakOwner.InstancePath => this;

		protected DataRegion(ReportItem parent)
			: base(parent)
		{
		}

		protected DataRegion(int id, ReportItem parent)
			: base(id, parent)
		{
			m_aggregates = new List<DataAggregateInfo>();
			m_postSortAggregates = new List<DataAggregateInfo>();
			m_runningValues = new List<RunningValueInfo>();
			m_cellRunningValues = new List<RunningValueInfo>();
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
			reportOmAggregates.ResetAll(m_aggregates);
			reportOmAggregates.ResetAll(m_postSortAggregates);
			reportOmAggregates.ResetAll(m_runningValues);
			if (m_dataScopeInfo != null)
			{
				m_dataScopeInfo.ResetAggregates(reportOmAggregates);
			}
		}

		public bool HasServerAggregate(string aggregateName)
		{
			return DataScopeInfo.ContainsServerAggregate(m_aggregates, aggregateName);
		}

		public void BindToStreamingScopeInstance(IReference<IOnDemandScopeInstance> scopeInstance)
		{
			m_currentStreamingScopeInstance = scopeInstance;
		}

		public void BindToNoRowsScopeInstance(OnDemandProcessingContext odpContext)
		{
			if (m_cachedNoRowsStreamingScopeInstance == null)
			{
				StreamingNoRowsDataRegionInstance memberOwner = new StreamingNoRowsDataRegionInstance(odpContext, this);
				m_cachedNoRowsStreamingScopeInstance = new SyntheticOnDemandMemberOwnerInstanceReference(memberOwner);
			}
			m_currentStreamingScopeInstance = m_cachedNoRowsStreamingScopeInstance;
		}

		public void ClearStreamingScopeInstanceBinding()
		{
			m_currentStreamingScopeInstance = null;
		}

		protected virtual List<ReportItem> ComputeDataRegionScopedItemsForDataProcessing()
		{
			List<ReportItem> results = null;
			if (OutermostStaticRowIndexes != null && OutermostStaticColumnIndexes != null)
			{
				foreach (int outermostStaticRowIndex in OutermostStaticRowIndexes)
				{
					foreach (int outermostStaticColumnIndex in OutermostStaticColumnIndexes)
					{
						MergeDataProcessingItems(Rows[outermostStaticRowIndex].Cells[outermostStaticColumnIndex], ref results);
					}
				}
			}
			if (OuterMembers != null)
			{
				MergeDataProcessingItems(OuterMembers.StaticMembersInSameScope, ref results);
			}
			if (InnerMembers != null)
			{
				MergeDataProcessingItems(InnerMembers.StaticMembersInSameScope, ref results);
			}
			return results;
		}

		private static void MergeDataProcessingItems(HierarchyNodeList staticMembers, ref List<ReportItem> results)
		{
			if (staticMembers != null)
			{
				for (int i = 0; i < staticMembers.Count; i++)
				{
					RuntimeRICollection.MergeDataProcessingItems(staticMembers[i].MemberContentCollection, ref results);
				}
			}
		}

		protected static void MergeDataProcessingItems(Cell rifCell, ref List<ReportItem> results)
		{
			if (rifCell != null)
			{
				RuntimeRICollection.MergeDataProcessingItems(rifCell.CellContentCollection, ref results);
			}
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

		public virtual void CreateDomainScopeMember(ReportHierarchyNode parentNode, Grouping grouping, AutomaticSubtotalContext context)
		{
			ReportHierarchyNode reportHierarchyNode = CreateHierarchyNode(context.GenerateID());
			reportHierarchyNode.Grouping = grouping.CloneForDomainScope(context, reportHierarchyNode);
			bool isColumn = parentNode?.IsColumn ?? false;
			HierarchyNodeList hierarchyNodeList = (parentNode != null) ? parentNode.InnerHierarchy : RowMembers;
			if (hierarchyNodeList != null)
			{
				hierarchyNodeList.Add(reportHierarchyNode);
				reportHierarchyNode.IsColumn = isColumn;
				CreateDomainScopeRowsAndCells(context, reportHierarchyNode);
			}
		}

		protected virtual void CreateDomainScopeRowsAndCells(AutomaticSubtotalContext context, ReportHierarchyNode member)
		{
			if (!member.IsColumn)
			{
				Row row = CreateRow(context.GenerateID(), ColumnCount);
				for (int i = 0; i < ColumnCount; i++)
				{
					row.Cells.Add(CreateCell(context.GenerateID(), -1, i));
				}
				Rows.Insert(RowMembers.GetMemberIndex(member), row);
				RowCount++;
			}
			else
			{
				int memberIndex = ColumnMembers.GetMemberIndex(member);
				for (int j = 0; j < RowCount; j++)
				{
					Rows[j].Cells.Insert(memberIndex, CreateCell(context.GenerateID(), j, -1));
				}
				ColumnCount++;
			}
		}

		protected virtual ReportHierarchyNode CreateHierarchyNode(int id)
		{
			return null;
		}

		protected virtual Row CreateRow(int id, int columnCount)
		{
			return null;
		}

		protected virtual Cell CreateCell(int id, int rowIndex, int colIndex)
		{
			return null;
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.IsDataRegionScopedCell = true;
			base.Initialize(context);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context);
			}
			m_dataScopeInfo.ValidateScopeRulesForIdc(context, this);
			if (context.PublishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.PeerGroups) && context.HasPeerGroups(this))
			{
				string propertyName = "TablixMembers";
				if (ObjectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.DataShape)
				{
					propertyName = "DataShapeMembers";
				}
				else if (ObjectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart)
				{
					propertyName = "ChartMembers";
				}
				else if (ObjectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.CustomReportItem)
				{
					propertyName = "DataMembers";
				}
				else if (ObjectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.Map)
				{
					propertyName = "MapMember";
				}
				else if (ObjectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel)
				{
					propertyName = "GaugeMember";
				}
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidPeerGroupsNotSupported, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
			}
			if ((context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) == 0)
			{
				return false;
			}
			if (IsDataRegion)
			{
				m_dataScopeInfo.Initialize(context, this);
			}
			context.InitializeAbsolutePosition(this);
			context.UpdateTopLeftDataRegion(this);
			context.InAutoSubtotalClone = false;
			if (m_pageBreak != null)
			{
				m_pageBreak.Initialize(context);
			}
			if (m_pageName != null)
			{
				m_pageName.Initialize("PageName", context);
				context.ExprHostBuilder.PageName(m_pageName);
			}
			if (m_sorting != null)
			{
				m_sorting.Initialize(context);
			}
			if (m_filters != null)
			{
				for (int i = 0; i < m_filters.Count; i++)
				{
					m_filters[i].Initialize(context);
				}
			}
			if (m_noRowsMessage != null)
			{
				m_noRowsMessage.Initialize("NoRows", context);
				context.ExprHostBuilder.GenericNoRows(m_noRowsMessage);
			}
			if (m_userSortExpressions != null)
			{
				context.ExprHostBuilder.UserSortExpressionsStart();
				for (int j = 0; j < m_userSortExpressions.Count; j++)
				{
					ExpressionInfo expression = m_userSortExpressions[j];
					context.ExprHostBuilder.UserSortExpression(expression);
				}
				context.ExprHostBuilder.UserSortExpressionsEnd();
			}
			context.RegisterRunningValues(m_runningValues, m_dataScopeInfo.RunningValuesOfAggregates);
			context.IsTopLevelCellContents = false;
			InitializeCorner(context);
			context.ResetMemberAndCellIndexInCollectionTable();
			context.Location &= ~Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegionCellTopLevelItem;
			bool flag = InitializeRows(context);
			if (ValidateInnerStructure(context))
			{
				context.Location |= Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegionGroupHeader;
				bool num = InitializeMembers(context);
				context.Location &= ~Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegionGroupHeader;
				if (num && flag)
				{
					InitializeData(context);
					m_outerGroupingMaximumDynamicLevel = GetMaximumDynamicLevelAndAssignHierarchyIndexes(OuterMembers, 0, ref m_outerGroupingDynamicMemberCount, ref m_outerGroupingDynamicPathCount);
					m_innerGroupingMaximumDynamicLevel = GetMaximumDynamicLevelAndAssignHierarchyIndexes(InnerMembers, 0, ref m_innerGroupingDynamicMemberCount, ref m_innerGroupingDynamicPathCount);
				}
			}
			context.UnRegisterRunningValues(m_runningValues, m_dataScopeInfo.RunningValuesOfAggregates);
			if (IsDataRegion)
			{
				if (context.EvaluateAtomicityCondition(m_sorting != null && !m_sorting.NaturalSort, this, AtomicityReason.Sorts) || context.EvaluateAtomicityCondition(m_filters != null, this, AtomicityReason.Filters) || context.EvaluateAtomicityCondition(HasAggregatesForAtomicityCheck(), this, AtomicityReason.Aggregates) || context.EvaluateAtomicityCondition(context.HasMultiplePeerChildScopes(this), this, AtomicityReason.PeerChildScopes))
				{
					context.FoundAtomicScope(this);
				}
				else
				{
					m_dataScopeInfo.IsDecomposable = true;
				}
			}
			return false;
		}

		private bool HasAggregatesForAtomicityCheck()
		{
			if (!DataScopeInfo.HasNonServerAggregates(m_aggregates) && !DataScopeInfo.HasAggregates(m_postSortAggregates) && !DataScopeInfo.HasAggregates(m_runningValues))
			{
				return m_dataScopeInfo.HasAggregatesOrRunningValues;
			}
			return true;
		}

		private int GetMaximumDynamicLevelAndAssignHierarchyIndexes(HierarchyNodeList members, int parentDynamicLevels, ref int hierarchyDynamicIndex, ref int hierarchyPathIndex)
		{
			if (members == null)
			{
				return parentDynamicLevels;
			}
			int count = members.Count;
			int num = parentDynamicLevels;
			for (int i = 0; i < count; i++)
			{
				int val = parentDynamicLevels;
				ReportHierarchyNode reportHierarchyNode = members[i];
				if (!reportHierarchyNode.IsStatic)
				{
					reportHierarchyNode.HierarchyDynamicIndex = hierarchyDynamicIndex++;
					if (reportHierarchyNode.HasInnerDynamic)
					{
						reportHierarchyNode.HierarchyPathIndex = hierarchyPathIndex;
						val = GetMaximumDynamicLevelAndAssignHierarchyIndexes(reportHierarchyNode.InnerDynamicMembers, parentDynamicLevels + 1, ref hierarchyDynamicIndex, ref hierarchyPathIndex);
					}
					else
					{
						reportHierarchyNode.HierarchyPathIndex = hierarchyPathIndex++;
						val = parentDynamicLevels + 1;
					}
				}
				else if (reportHierarchyNode.HasInnerDynamic)
				{
					val = GetMaximumDynamicLevelAndAssignHierarchyIndexes(reportHierarchyNode.InnerDynamicMembers, parentDynamicLevels, ref hierarchyDynamicIndex, ref hierarchyPathIndex);
				}
				num = Math.Max(num, val);
			}
			return num;
		}

		protected GroupingList GenerateUserSortGroupingList(bool rowIsInnerGrouping)
		{
			GroupingList groupingList = new GroupingList();
			HierarchyNodeList members = rowIsInnerGrouping ? RowMembers : ColumnMembers;
			AddGroupsToList(members, groupingList);
			members = (rowIsInnerGrouping ? ColumnMembers : RowMembers);
			AddGroupsToList(members, groupingList);
			return groupingList;
		}

		private void AddGroupsToList(HierarchyNodeList members, GroupingList groups)
		{
			foreach (ReportHierarchyNode member in members)
			{
				if (member.Grouping != null)
				{
					groups.Add(member.Grouping);
				}
				if (member.InnerHierarchy != null)
				{
					AddGroupsToList(member.InnerHierarchy, groups);
				}
			}
		}

		protected virtual bool InitializeRows(InitializationContext context)
		{
			bool result = true;
			if (((ColumnMembers == null || RowMembers == null) && Rows != null) || (ColumnMembers != null && RowMembers != null && Rows == null) || (Rows != null && Rows.Count != m_rowCount))
			{
				context.ErrorContext.Register((context.ObjectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart) ? ProcessingErrorCode.rsWrongNumberOfChartSeries : ProcessingErrorCode.rsWrongNumberOfDataRows, Severity.Error, context.ObjectType, context.ObjectName, m_rowCount.ToString(CultureInfo.InvariantCulture));
				return false;
			}
			if (Rows != null)
			{
				for (int i = 0; i < Rows.Count; i++)
				{
					Row row = Rows[i];
					if (row == null || row.Cells == null || row.Cells.Count != m_columnCount)
					{
						context.ErrorContext.Register((context.ObjectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.CustomReportItem) ? ProcessingErrorCode.rsWrongNumberOfDataCellsInDataRow : ProcessingErrorCode.rsWrongNumberOfChartDataPointsInSeries, Severity.Error, context.ObjectType, context.ObjectName, (context.ObjectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.CustomReportItem) ? "DataCell" : "ChartDataPoint", i.ToString(CultureInfo.CurrentCulture));
						result = false;
					}
					row.Initialize(context);
				}
			}
			return result;
		}

		protected virtual void InitializeCorner(InitializationContext context)
		{
		}

		protected abstract bool ValidateInnerStructure(InitializationContext context);

		protected virtual bool InitializeMembers(InitializationContext context)
		{
			bool flag = true;
			if (m_rowCount != 0 && m_columnCount != 0 && (Rows == null || Rows.Count == 0))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsMissingDataCells, Severity.Error, context.ObjectType, context.ObjectName, "DataRows");
				flag = false;
			}
			context.Location |= Microsoft.ReportingServices.ReportPublishing.LocationFlags.InTablixColumnHierarchy;
			flag &= InitializeColumnMembers(context);
			context.ResetMemberAndCellIndexInCollectionTable();
			context.Location &= ~Microsoft.ReportingServices.ReportPublishing.LocationFlags.InTablixColumnHierarchy;
			context.Location |= Microsoft.ReportingServices.ReportPublishing.LocationFlags.InTablixRowHierarchy;
			flag &= InitializeRowMembers(context);
			context.ResetMemberAndCellIndexInCollectionTable();
			context.Location &= ~Microsoft.ReportingServices.ReportPublishing.LocationFlags.InTablixRowHierarchy;
			return flag;
		}

		protected virtual bool InitializeColumnMembers(InitializationContext context)
		{
			HierarchyNodeList columnMembers = ColumnMembers;
			context.MemberCellIndex = 0;
			if (columnMembers == null || columnMembers.Count == 0)
			{
				return false;
			}
			foreach (ReportHierarchyNode item in columnMembers)
			{
				context.InAutoSubtotalClone = item.IsAutoSubtotal;
				m_hasDynamicColumnMember |= item.Initialize(context);
			}
			if (columnMembers.Count == 1 && columnMembers[0].IsStatic)
			{
				context.SpecialTransferRunningValues(columnMembers[0].RunningValues, columnMembers[0].DataScopeInfo.RunningValuesOfAggregates);
			}
			return true;
		}

		protected virtual bool InitializeRowMembers(InitializationContext context)
		{
			HierarchyNodeList rowMembers = RowMembers;
			context.MemberCellIndex = 0;
			if (rowMembers == null || rowMembers.Count == 0)
			{
				return false;
			}
			foreach (ReportHierarchyNode item in rowMembers)
			{
				m_hasDynamicRowMember |= item.Initialize(context);
			}
			if (rowMembers.Count == 1 && rowMembers[0].IsStatic)
			{
				context.SpecialTransferRunningValues(rowMembers[0].RunningValues, rowMembers[0].DataScopeInfo.RunningValuesOfAggregates);
			}
			return true;
		}

		protected virtual void InitializeData(InitializationContext context)
		{
			m_textboxesInScope = context.GetCurrentReferencableTextboxes();
			m_variablesInScope = context.GetCurrentReferencableVariables();
			if (context.ObjectType != Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart)
			{
				context.Location |= Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegionCellTopLevelItem;
			}
			context.TablixName = m_name;
			DataRegion dataRegion = context.RegisterDataRegionCellScope(this, m_columnCount == 1 && ColumnMembers[0].Grouping == null, m_aggregates, m_postSortAggregates);
			int index = 0;
			for (int i = 0; i < RowMembers.Count; i++)
			{
				InitializeDataRows(ref index, RowMembers[i], context);
			}
			if (context.IsRunningValueDirectionColumn || (!m_hasDynamicRowMember && m_hasDynamicColumnMember))
			{
				m_processingInnerGrouping = ProcessingInnerGroupings.Row;
			}
			if (IsColumnGroupingSwitched && m_hasDynamicColumnMember)
			{
				m_processingInnerGrouping = ProcessingInnerGroupings.Row;
			}
			context.UnRegisterTablixCellScope(dataRegion);
		}

		protected void InitializeDataRows(ref int index, ReportHierarchyNode member, InitializationContext context)
		{
			member.HierarchyParentGroups = context.GetContainingScopesInCurrentDataRegion();
			bool suspendErrors = context.ErrorContext.SuspendErrors;
			bool inRecursiveHierarchyRows = context.InRecursiveHierarchyRows;
			context.ErrorContext.SuspendErrors |= member.IsAutoSubtotal;
			context.InAutoSubtotalClone = member.IsAutoSubtotal;
			bool registeredVisibility = member.PreInitializeDataMember(context);
			member.CaptureReferencableTextboxes(context);
			if (member.Grouping != null)
			{
				context.Location |= Microsoft.ReportingServices.ReportPublishing.LocationFlags.InGrouping;
				if (member.Grouping.IsDetail)
				{
					context.Location |= Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDetail;
				}
				context.IsDataRegionScopedCell = false;
				if (member.Grouping.Variables != null)
				{
					context.RegisterVariables(member.Grouping.Variables);
				}
				context.RegisterGroupingScopeForDataRegionCell(member);
				context.InRecursiveHierarchyRows = (member.Grouping.Parent != null);
			}
			else if (member.IsNonToggleableHiddenMember)
			{
				context.Location |= Microsoft.ReportingServices.ReportPublishing.LocationFlags.InNonToggleableHiddenStaticTablixMember;
			}
			if (member.InnerHierarchy == null)
			{
				InitializeDataColumns(member.ID, index, context);
				index++;
			}
			else
			{
				HierarchyNodeList innerHierarchy = member.InnerHierarchy;
				for (int i = 0; i < innerHierarchy.Count; i++)
				{
					InitializeDataRows(ref index, innerHierarchy[i], context);
				}
			}
			member.PostInitializeDataMember(context, registeredVisibility);
			if (member.Grouping != null)
			{
				context.UnRegisterGroupingScopeForDataRegionCell(member);
				if (member.Grouping.Variables != null)
				{
					context.UnregisterVariables(member.Grouping.Variables);
				}
			}
			context.InRecursiveHierarchyRows = inRecursiveHierarchyRows;
			context.ErrorContext.SuspendErrors = suspendErrors;
		}

		protected virtual void InitializeDataColumns(int parentRowID, int rowIndex, InitializationContext context)
		{
			int columnIndex = 0;
			for (int i = 0; i < ColumnMembers.Count; i++)
			{
				InitializeDataColumns(parentRowID, rowIndex, ref columnIndex, ColumnMembers[i], context);
			}
		}

		protected virtual void InitializeDataColumns(int parentRowID, int rowIndex, ref int columnIndex, ReportHierarchyNode member, InitializationContext context)
		{
			member.HierarchyParentGroups = context.GetContainingScopesInCurrentDataRegion();
			bool suspendErrors = context.ErrorContext.SuspendErrors;
			bool inRecursiveHierarchyColumns = context.InRecursiveHierarchyColumns;
			context.ErrorContext.SuspendErrors |= member.IsAutoSubtotal;
			context.InAutoSubtotalClone = member.IsAutoSubtotal;
			bool registeredVisibility = member.PreInitializeDataMember(context);
			member.CaptureReferencableTextboxes(context);
			if (member.Grouping != null)
			{
				context.Location |= Microsoft.ReportingServices.ReportPublishing.LocationFlags.InGrouping;
				if (member.Grouping.IsDetail)
				{
					context.Location |= Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDetail;
				}
				context.IsDataRegionScopedCell = false;
				if (member.Grouping.Variables != null)
				{
					context.RegisterVariables(member.Grouping.Variables);
				}
				context.RegisterGroupingScopeForDataRegionCell(member);
				context.InRecursiveHierarchyColumns = (member.Grouping.Parent != null);
			}
			else if (member.IsNonToggleableHiddenMember)
			{
				context.Location |= Microsoft.ReportingServices.ReportPublishing.LocationFlags.InNonToggleableHiddenStaticTablixMember;
			}
			if (member.InnerHierarchy == null)
			{
				context.Location |= Microsoft.ReportingServices.ReportPublishing.LocationFlags.InTablixCell;
				if (context.CellHasDynamicRowsAndColumns)
				{
					context.Location |= Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDynamicTablixCell;
				}
				if (Rows[rowIndex].Cells != null && rowIndex < Rows.Count && columnIndex < Rows[rowIndex].Cells.Count)
				{
					Cell cell = Rows[rowIndex].Cells[columnIndex];
					cell.Initialize(parentRowID, member.ID, rowIndex, columnIndex, context);
					if ((context.ObjectType != Microsoft.ReportingServices.ReportProcessing.ObjectType.Tablix || !context.HasUserSorts) && !context.IsDataRegionScopedCell)
					{
						CopyCellAggregates(cell);
					}
				}
				columnIndex++;
			}
			else
			{
				HierarchyNodeList innerHierarchy = member.InnerHierarchy;
				for (int i = 0; i < innerHierarchy.Count; i++)
				{
					InitializeDataColumns(parentRowID, rowIndex, ref columnIndex, innerHierarchy[i], context);
				}
			}
			member.PostInitializeDataMember(context, registeredVisibility);
			if (member.Grouping != null)
			{
				context.UnRegisterGroupingScopeForDataRegionCell(member);
				if (member.Grouping.Variables != null)
				{
					context.UnregisterVariables(member.Grouping.Variables);
				}
			}
			context.InRecursiveHierarchyColumns = inRecursiveHierarchyColumns;
			context.ErrorContext.SuspendErrors = suspendErrors;
		}

		internal override void InitializeRVDirectionDependentItems(InitializationContext context)
		{
			if (IsDataRegion && context.RegisterDataRegion(this))
			{
				DataRegion dataRegion = null;
				context.IsDataRegionScopedCell = true;
				context.Location |= (Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataSet | Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegion);
				context.ObjectType = ObjectType;
				context.ObjectName = base.Name;
				context.RegisterRunningValues(m_runningValues, m_dataScopeInfo.RunningValuesOfAggregates);
				InitializeRVDirectionDependentItemsInCorner(context);
				dataRegion = context.RegisterDataRegionCellScope(this, m_columnCount == 1 && ColumnMembers[0].Grouping == null, m_aggregates, m_postSortAggregates);
				context.Location &= ~Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegionCellTopLevelItem;
				InitializeRVDirectionDependentItems(context, traverseInner: false);
				InitializeRVDirectionDependentItems(context, traverseInner: true);
				int outerIndex = 0;
				int innerIndex = 0;
				InitializeRVDirectionDependentItems(ref outerIndex, ref innerIndex, context, traverseInner: false, initializeCells: true);
				context.ProcessUserSortScopes(m_name);
				context.EventSourcesWithDetailSortExpressionInitialize(m_name);
				context.UnRegisterRunningValues(m_runningValues, m_dataScopeInfo.RunningValuesOfAggregates);
				context.UnRegisterTablixCellScope(dataRegion);
				context.UnRegisterDataRegion(this);
			}
		}

		private void InitializeRVDirectionDependentItems(InitializationContext context, bool traverseInner)
		{
			int outerIndex = 0;
			int innerIndex = 0;
			InitializeRVDirectionDependentItems(ref outerIndex, ref innerIndex, context, traverseInner, initializeCells: false);
		}

		private void InitializeRVDirectionDependentItems(ref int outerIndex, ref int innerIndex, InitializationContext context, bool traverseInner, bool initializeCells)
		{
			HierarchyNodeList hierarchyNodeList = (m_processingInnerGrouping == ProcessingInnerGroupings.Column == traverseInner) ? ColumnMembers : RowMembers;
			for (int i = 0; i < hierarchyNodeList.Count; i++)
			{
				InitializeRVDirectionDependentItems(ref outerIndex, ref innerIndex, hierarchyNodeList[i], context, traverseInner, initializeCells);
			}
		}

		private void InitializeRVDirectionDependentItems(ref int outerIndex, ref int innerIndex, ReportHierarchyNode member, InitializationContext context, bool traverseInner, bool initializeCells)
		{
			member.HierarchyParentGroups = context.GetContainingScopesInCurrentDataRegion();
			if (member.Grouping != null)
			{
				context.ObjectType = Microsoft.ReportingServices.ReportProcessing.ObjectType.Grouping;
				context.ObjectName = member.Grouping.Name;
				context.IsDataRegionScopedCell = false;
				context.Location |= Microsoft.ReportingServices.ReportPublishing.LocationFlags.InGrouping;
				List<ExpressionInfo> groupExpressions = member.Grouping.GroupExpressions;
				if (groupExpressions == null || groupExpressions.Count == 0)
				{
					context.Location |= Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDetail;
				}
				context.RegisterGroupingScopeForDataRegionCell(member);
				if (member.Grouping.Variables != null)
				{
					context.RegisterVariables(member.Grouping.Variables);
				}
			}
			if (!initializeCells)
			{
				member.InitializeRVDirectionDependentItems(context);
			}
			if (member.InnerHierarchy == null)
			{
				if (initializeCells)
				{
					if (traverseInner)
					{
						InitializeRVDirectionDependentItems(outerIndex, innerIndex, context);
						innerIndex++;
					}
					else
					{
						innerIndex = 0;
						InitializeRVDirectionDependentItems(ref outerIndex, ref innerIndex, context, traverseInner: true, initializeCells);
						outerIndex++;
					}
				}
			}
			else
			{
				HierarchyNodeList innerHierarchy = member.InnerHierarchy;
				for (int i = 0; i < innerHierarchy.Count; i++)
				{
					InitializeRVDirectionDependentItems(ref outerIndex, ref innerIndex, innerHierarchy[i], context, traverseInner, initializeCells);
				}
			}
			if (member.Grouping != null)
			{
				if (initializeCells)
				{
					context.ProcessUserSortScopes(member.Grouping.Name);
					context.EventSourcesWithDetailSortExpressionInitialize(member.Grouping.Name);
				}
				context.UnRegisterGroupingScopeForDataRegionCell(member);
				if (member.Grouping.Variables != null)
				{
					context.UnregisterVariables(member.Grouping.Variables);
				}
			}
		}

		protected virtual void InitializeRVDirectionDependentItemsInCorner(InitializationContext context)
		{
		}

		protected virtual void InitializeRVDirectionDependentItems(int outerIndex, int innerIndex, InitializationContext context)
		{
		}

		internal override void DetermineGroupingExprValueCount(InitializationContext context, int groupingExprCount)
		{
			DetermineGroupingExprValueCountInCorner(context, groupingExprCount);
			int outerIndex = 0;
			int innerIndex = 0;
			DetermineGroupingExprValueCount(ref outerIndex, ref innerIndex, context, traverseInner: false, groupingExprCount);
		}

		private void DetermineGroupingExprValueCount(ref int outerIndex, ref int innerIndex, InitializationContext context, bool traverseInner, int groupingExprCount)
		{
			HierarchyNodeList hierarchyNodeList = (m_processingInnerGrouping == ProcessingInnerGroupings.Column == traverseInner) ? ColumnMembers : RowMembers;
			for (int i = 0; i < hierarchyNodeList.Count; i++)
			{
				DetermineGroupingExprValueCount(ref outerIndex, ref innerIndex, hierarchyNodeList[i], context, traverseInner, groupingExprCount);
			}
		}

		private void DetermineGroupingExprValueCount(ref int outerIndex, ref int innerIndex, ReportHierarchyNode member, InitializationContext context, bool traverseInner, int groupingExprCount)
		{
			if (member.Grouping != null)
			{
				List<ExpressionInfo> groupExpressions = member.Grouping.GroupExpressions;
				if (groupExpressions != null)
				{
					groupingExprCount += groupExpressions.Count;
				}
				context.AddGroupingExprCountForGroup(member.Grouping.Name, groupingExprCount);
			}
			member.DetermineGroupingExprValueCount(context, groupingExprCount);
			if (member.InnerHierarchy == null)
			{
				if (traverseInner)
				{
					DetermineGroupingExprValueCount(outerIndex, innerIndex, context, groupingExprCount);
					innerIndex++;
				}
				else
				{
					innerIndex = 0;
					DetermineGroupingExprValueCount(ref outerIndex, ref innerIndex, context, traverseInner: true, groupingExprCount);
					outerIndex++;
				}
			}
			else
			{
				HierarchyNodeList innerHierarchy = member.InnerHierarchy;
				for (int i = 0; i < innerHierarchy.Count; i++)
				{
					DetermineGroupingExprValueCount(ref outerIndex, ref innerIndex, innerHierarchy[i], context, traverseInner, groupingExprCount);
				}
			}
		}

		protected virtual void DetermineGroupingExprValueCountInCorner(InitializationContext context, int groupingExprCount)
		{
		}

		protected virtual void DetermineGroupingExprValueCount(int outerIndex, int innerIndex, InitializationContext context, int groupingExprCount)
		{
		}

		protected void CopyCellAggregates(Cell cell)
		{
			CopyCellAggregates(cell.Aggregates, ref m_cellAggregates);
			CopyCellAggregates(cell.PostSortAggregates, ref m_cellPostSortAggregates);
			CopyCellAggregates(cell.RunningValues, ref m_cellRunningValues);
		}

		private void CopyCellAggregates<AggregateType>(List<AggregateType> aggregates, ref List<AggregateType> dataRegionCellAggregates) where AggregateType : DataAggregateInfo, new()
		{
			if (aggregates != null && aggregates.Count != 0)
			{
				if (dataRegionCellAggregates == null)
				{
					dataRegionCellAggregates = new List<AggregateType>();
				}
				dataRegionCellAggregates.AddRange(aggregates);
			}
		}

		internal DataSet GetDataSet(Report reportDefinition)
		{
			if (m_cachedDataSet == null)
			{
				Global.Tracer.Assert(reportDefinition != null, "(null != reportDefinition)");
				if (m_dataScopeInfo != null && m_dataScopeInfo.DataSet != null)
				{
					m_cachedDataSet = m_dataScopeInfo.DataSet;
				}
				else if (m_dataSetName == null)
				{
					m_dataSetName = reportDefinition.FirstDataSet.Name;
					m_cachedDataSet = reportDefinition.FirstDataSet;
				}
				else
				{
					m_cachedDataSet = reportDefinition.MappingNameToDataSet[m_dataSetName];
				}
			}
			return m_cachedDataSet;
		}

		List<DataAggregateInfo> IAggregateHolder.GetAggregateList()
		{
			return m_aggregates;
		}

		List<DataAggregateInfo> IAggregateHolder.GetPostSortAggregateList()
		{
			return m_postSortAggregates;
		}

		void IAggregateHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(m_aggregates != null, "(null != m_aggregates)");
			if (m_aggregates.Count == 0)
			{
				m_aggregates = null;
			}
			Global.Tracer.Assert(m_postSortAggregates != null, "(null != m_postSortAggregates)");
			if (m_postSortAggregates.Count == 0)
			{
				m_postSortAggregates = null;
			}
		}

		List<RunningValueInfo> IRunningValueHolder.GetRunningValueList()
		{
			return m_runningValues;
		}

		void IRunningValueHolder.ClearIfEmpty()
		{
			if (m_runningValues != null && m_runningValues.Count == 0)
			{
				m_runningValues = null;
			}
			if (m_cellRunningValues != null && m_cellRunningValues.Count == 0)
			{
				m_cellRunningValues = null;
			}
		}

		internal void ConvertCellAggregatesToIndexes()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
			Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
			if (m_cellAggregates != null)
			{
				GenerateAggregateIndexMapping(m_cellAggregates, dictionary);
			}
			if (m_cellPostSortAggregates != null)
			{
				GenerateAggregateIndexMapping(m_cellPostSortAggregates, dictionary2);
			}
			if (m_cellRunningValues != null)
			{
				GenerateAggregateIndexMapping(m_cellRunningValues, dictionary3);
			}
			for (int i = 0; i < m_rowCount && Rows.Count > i; i++)
			{
				Row row = Rows[i];
				for (int j = 0; j < m_columnCount; j++)
				{
					if (row.Cells == null)
					{
						break;
					}
					if (row.Cells.Count <= j)
					{
						break;
					}
					row.Cells[j]?.GenerateAggregateIndexes(dictionary, dictionary2, dictionary3);
				}
			}
		}

		private static void GenerateAggregateIndexMapping<AggregateType>(List<AggregateType> cellAggregates, Dictionary<string, int> aggregateIndexes) where AggregateType : DataAggregateInfo
		{
			int count = cellAggregates.Count;
			for (int i = 0; i < count; i++)
			{
				AggregateType val = cellAggregates[i];
				aggregateIndexes.Add(val.Name, i);
				int num = (val.DuplicateNames != null) ? val.DuplicateNames.Count : 0;
				for (int j = 0; j < num; j++)
				{
					string key = val.DuplicateNames[j];
					if (!aggregateIndexes.ContainsKey(key))
					{
						aggregateIndexes.Add(key, i);
					}
				}
			}
		}

		protected override InstancePathItem CreateInstancePathItem()
		{
			if (IsDataRegion)
			{
				return new InstancePathItem(InstancePathItemType.DataRegion, IndexInCollection);
			}
			return new InstancePathItem();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			DataRegion dataRegion = (DataRegion)base.PublishClone(context);
			dataRegion.m_dataScopeInfo = m_dataScopeInfo.PublishClone(context, dataRegion.ID);
			context.CurrentDataRegionClone = dataRegion;
			context.AddAggregateHolder(dataRegion);
			context.AddRunningValueHolder(dataRegion);
			if (m_dataSetName != null)
			{
				dataRegion.m_dataSetName = (string)m_dataSetName.Clone();
			}
			context.RegisterClonedScopeName(m_name, dataRegion.m_name);
			context.AddSortTarget(dataRegion.m_name, dataRegion);
			if (m_noRowsMessage != null)
			{
				dataRegion.m_noRowsMessage = (ExpressionInfo)m_noRowsMessage.PublishClone(context);
			}
			if (m_repeatSiblings != null)
			{
				dataRegion.m_repeatSiblings = new List<int>(m_repeatSiblings.Count);
				foreach (int repeatSibling in m_repeatSiblings)
				{
					dataRegion.m_repeatSiblings.Add(repeatSibling);
				}
			}
			if (m_sorting != null)
			{
				dataRegion.m_sorting = (Sorting)m_sorting.PublishClone(context);
			}
			if (m_filters != null)
			{
				dataRegion.m_filters = new List<Filter>(m_filters.Count);
				foreach (Filter filter in m_filters)
				{
					dataRegion.m_filters.Add((Filter)filter.PublishClone(context));
				}
			}
			if (m_pageBreak != null)
			{
				dataRegion.m_pageBreak = (PageBreak)m_pageBreak.PublishClone(context);
			}
			if (m_pageName != null)
			{
				dataRegion.m_pageName = (ExpressionInfo)m_pageName.PublishClone(context);
			}
			if (m_detailSortFiltersInScope != null)
			{
				dataRegion.m_detailSortFiltersInScope = new InScopeSortFilterHashtable(m_detailSortFiltersInScope.Count);
				{
					foreach (DictionaryEntry item in m_detailSortFiltersInScope)
					{
						List<int> obj = (List<int>)item.Value;
						List<int> list = new List<int>(obj.Count);
						foreach (int item2 in obj)
						{
							list.Add(item2);
						}
						dataRegion.m_detailSortFiltersInScope.Add(item.Key, list);
					}
					return dataRegion;
				}
			}
			return dataRegion;
		}

		internal override void TraverseScopes(IRIFScopeVisitor visitor)
		{
			if (IsDataRegion)
			{
				visitor.PreVisit(this);
			}
			TraverseDataRegionLevelScopes(visitor);
			TraverseMembers(visitor, RowMembers);
			TraverseMembers(visitor, ColumnMembers);
			int rowCellIndex = 0;
			int colCellIndex = 0;
			TraverseScopes(visitor, RowMembers, ref rowCellIndex, ref colCellIndex);
			if (IsDataRegion)
			{
				visitor.PostVisit(this);
			}
		}

		private void TraverseMembers(IRIFScopeVisitor visitor, HierarchyNodeList members)
		{
			if (members == null)
			{
				return;
			}
			foreach (ReportHierarchyNode member in members)
			{
				TraversMembers(visitor, member);
			}
		}

		private void TraversMembers(IRIFScopeVisitor visitor, ReportHierarchyNode member)
		{
			if (!member.IsStatic)
			{
				visitor.PreVisit(member);
			}
			member.TraverseMemberScopes(visitor);
			TraverseMembers(visitor, member.InnerHierarchy);
			if (!member.IsStatic)
			{
				visitor.PostVisit(member);
			}
		}

		protected virtual void TraverseDataRegionLevelScopes(IRIFScopeVisitor visitor)
		{
		}

		private void TraverseScopes(IRIFScopeVisitor visitor, HierarchyNodeList members, ref int rowCellIndex, ref int colCellIndex)
		{
			if (members == null)
			{
				return;
			}
			foreach (ReportHierarchyNode member in members)
			{
				TraverseScopes(visitor, member, ref rowCellIndex, ref colCellIndex);
			}
		}

		private void TraverseScopes(IRIFScopeVisitor visitor, ReportHierarchyNode member, ref int rowCellIndex, ref int colCellIndex)
		{
			if (member == null)
			{
				return;
			}
			if (!member.IsStatic)
			{
				visitor.PreVisit(member);
			}
			if (member.InnerHierarchy == null || member.InnerHierarchy.Count == 0)
			{
				if (member.IsColumn)
				{
					RowList rows = Rows;
					if (rows != null && rows.Count > rowCellIndex)
					{
						Row row = rows[rowCellIndex];
						if (row != null && row.Cells != null && row.Cells.Count > colCellIndex)
						{
							Cell cell = row.Cells[colCellIndex];
							TraverseScopes(visitor, cell, rowCellIndex, colCellIndex);
						}
					}
					colCellIndex++;
				}
				else
				{
					colCellIndex = 0;
					TraverseScopes(visitor, ColumnMembers, ref rowCellIndex, ref colCellIndex);
					rowCellIndex++;
				}
			}
			else
			{
				TraverseScopes(visitor, member.InnerHierarchy, ref rowCellIndex, ref colCellIndex);
			}
			if (!member.IsStatic)
			{
				visitor.PostVisit(member);
			}
		}

		protected void TraverseScopes(IRIFScopeVisitor visitor, Cell cell, int rowIndex, int colIndex)
		{
			cell?.TraverseScopes(visitor, rowIndex, colIndex);
		}

		protected void BuildAndSetupAxisScopeTreeForAutoSubtotals(ref AutomaticSubtotalContext context, ReportHierarchyNode member)
		{
			int memberCellIndex = context.StartIndex;
			FindClonedScopesForAutoSubtotals(register: false, member, context.ScopeNamesToClone, ref memberCellIndex);
		}

		private void FindClonedScopesForAutoSubtotals(bool register, ReportHierarchyNode member, Dictionary<string, IRIFDataScope> scopesToClone, ref int memberCellIndex)
		{
			if (member == null)
			{
				return;
			}
			if (!member.IsStatic && register)
			{
				scopesToClone.Add(member.Grouping.Name, member);
			}
			TablixMember tablixMember = member as TablixMember;
			if (tablixMember != null && tablixMember.TablixHeader != null)
			{
				TablixHeader tablixHeader = tablixMember.TablixHeader;
				FindClonedScopesForAutoSubtotals(tablixHeader.CellContents, scopesToClone);
				FindClonedScopesForAutoSubtotals(tablixHeader.AltCellContents, scopesToClone);
			}
			if (member.InnerHierarchy == null || member.InnerHierarchy.Count == 0)
			{
				RowList rows = Rows;
				if (rows != null && ObjectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.Tablix)
				{
					if (member.IsColumn)
					{
						foreach (Row item in rows)
						{
							if (item != null && item.Cells != null && item.Cells.Count > memberCellIndex)
							{
								FindClonedScopesForAutoSubtotals((TablixCellBase)item.Cells[memberCellIndex], scopesToClone);
							}
						}
					}
					else if (rows.Count > memberCellIndex)
					{
						Row row2 = rows[memberCellIndex];
						if (row2 != null && row2.Cells != null)
						{
							foreach (TablixCellBase cell in row2.Cells)
							{
								FindClonedScopesForAutoSubtotals(cell, scopesToClone);
							}
						}
					}
				}
				memberCellIndex++;
			}
			else
			{
				FindClonedScopesForAutoSubtotals(register, member.InnerHierarchy, scopesToClone, ref memberCellIndex);
			}
		}

		private void FindClonedScopesForAutoSubtotals(TablixCellBase cell, Dictionary<string, IRIFDataScope> scopesToClone)
		{
			if (cell != null)
			{
				FindClonedScopesForAutoSubtotals(cell.CellContents, scopesToClone);
				FindClonedScopesForAutoSubtotals(cell.AltCellContents, scopesToClone);
			}
		}

		private void FindClonedScopesForAutoSubtotals(ReportItem item, Dictionary<string, IRIFDataScope> scopesToClone)
		{
			if (item == null)
			{
				return;
			}
			switch (item.ObjectType)
			{
			case Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel:
			case Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart:
			case Microsoft.ReportingServices.ReportProcessing.ObjectType.CustomReportItem:
			case Microsoft.ReportingServices.ReportProcessing.ObjectType.Tablix:
			{
				DataRegion dataRegion = (DataRegion)item;
				scopesToClone.Add(dataRegion.Name, dataRegion);
				int memberCellIndex = 0;
				dataRegion.FindClonedScopesForAutoSubtotals(register: true, dataRegion.OuterMembers, scopesToClone, ref memberCellIndex);
				int memberCellIndex2 = 0;
				dataRegion.FindClonedScopesForAutoSubtotals(register: true, dataRegion.InnerMembers, scopesToClone, ref memberCellIndex2);
				Tablix tablix = dataRegion as Tablix;
				if (tablix == null || tablix.Corner == null)
				{
					break;
				}
				foreach (List<TablixCornerCell> item2 in tablix.Corner)
				{
					if (item2 == null)
					{
						continue;
					}
					foreach (TablixCornerCell item3 in item2)
					{
						dataRegion.FindClonedScopesForAutoSubtotals(item3, scopesToClone);
					}
				}
				break;
			}
			case Microsoft.ReportingServices.ReportProcessing.ObjectType.Map:
			{
				Map map = (Map)item;
				if (map.MapDataRegions == null)
				{
					break;
				}
				foreach (MapDataRegion mapDataRegion in map.MapDataRegions)
				{
					FindClonedScopesForAutoSubtotals(mapDataRegion, scopesToClone);
				}
				break;
			}
			case Microsoft.ReportingServices.ReportProcessing.ObjectType.Rectangle:
			{
				Rectangle rectangle = (Rectangle)item;
				if (rectangle.ReportItems == null)
				{
					break;
				}
				foreach (ReportItem reportItem in rectangle.ReportItems)
				{
					FindClonedScopesForAutoSubtotals(reportItem, scopesToClone);
				}
				break;
			}
			}
		}

		private void FindClonedScopesForAutoSubtotals(bool register, HierarchyNodeList members, Dictionary<string, IRIFDataScope> scopesToClone, ref int memberCellIndex)
		{
			if (members == null)
			{
				return;
			}
			foreach (ReportHierarchyNode member in members)
			{
				FindClonedScopesForAutoSubtotals(register, member, scopesToClone, ref memberCellIndex);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataSetName, Token.String));
			list.Add(new MemberInfo(MemberName.NoRowsMessage, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ColumnCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.RowCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.ProcessingInnerGrouping, Token.Enum));
			list.Add(new MemberInfo(MemberName.RepeatSiblings, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
			list.Add(new MemberInfo(MemberName.Sorting, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Sorting));
			list.Add(new MemberInfo(MemberName.Filters, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Filter));
			list.Add(new MemberInfo(MemberName.Aggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			list.Add(new MemberInfo(MemberName.PostSortAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			list.Add(new MemberInfo(MemberName.RunningValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RunningValueInfo));
			list.Add(new MemberInfo(MemberName.CellAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			list.Add(new MemberInfo(MemberName.CellPostSortAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			list.Add(new MemberInfo(MemberName.CellRunningValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RunningValueInfo));
			list.Add(new MemberInfo(MemberName.UserSortExpressions, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DetailSortFiltersInScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Int32PrimitiveListHashtable));
			list.Add(new ReadOnlyMemberInfo(MemberName.PageBreakLocation, Token.Enum));
			list.Add(new MemberInfo(MemberName.IndexInCollection, Token.Int32));
			list.Add(new MemberInfo(MemberName.NeedToCacheDataRows, Token.Boolean));
			list.Add(new MemberInfo(MemberName.InScopeEventSources, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IInScopeEventSource));
			list.Add(new MemberInfo(MemberName.OuterGroupingMaximumDynamicLevel, Token.Int32));
			list.Add(new MemberInfo(MemberName.OuterGroupingDynamicMemberCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.OuterGroupingDynamicPathCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.InnerGroupingMaximumDynamicLevel, Token.Int32));
			list.Add(new MemberInfo(MemberName.InnerGroupingDynamicMemberCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.InnerGroupingDynamicPathCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.TextboxesInScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.VariablesInScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.PageBreak, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PageBreak));
			list.Add(new MemberInfo(MemberName.PageName, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DataScopeInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataScopeInfo));
			list.Add(new MemberInfo(MemberName.RowDomainScopeCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.ColumnDomainScopeCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.IsMatrixIDC, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DataSetName:
					writer.Write(m_dataSetName);
					break;
				case MemberName.NoRowsMessage:
					writer.Write(m_noRowsMessage);
					break;
				case MemberName.ColumnCount:
					writer.Write(m_columnCount);
					break;
				case MemberName.RowCount:
					writer.Write(m_rowCount);
					break;
				case MemberName.ProcessingInnerGrouping:
					writer.WriteEnum((int)m_processingInnerGrouping);
					break;
				case MemberName.RepeatSiblings:
					writer.WriteListOfPrimitives(m_repeatSiblings);
					break;
				case MemberName.Sorting:
					writer.Write(m_sorting);
					break;
				case MemberName.Filters:
					writer.Write(m_filters);
					break;
				case MemberName.Aggregates:
					writer.Write(m_aggregates);
					break;
				case MemberName.PostSortAggregates:
					writer.Write(m_postSortAggregates);
					break;
				case MemberName.RunningValues:
					writer.Write(m_runningValues);
					break;
				case MemberName.CellAggregates:
					writer.Write(m_cellAggregates);
					break;
				case MemberName.CellPostSortAggregates:
					writer.Write(m_cellPostSortAggregates);
					break;
				case MemberName.CellRunningValues:
					writer.Write(m_cellRunningValues);
					break;
				case MemberName.UserSortExpressions:
					writer.Write(m_userSortExpressions);
					break;
				case MemberName.DetailSortFiltersInScope:
					writer.WriteInt32PrimitiveListHashtable<int>(m_detailSortFiltersInScope);
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
				case MemberName.OuterGroupingMaximumDynamicLevel:
					writer.Write(m_outerGroupingMaximumDynamicLevel);
					break;
				case MemberName.OuterGroupingDynamicMemberCount:
					writer.Write(m_outerGroupingDynamicMemberCount);
					break;
				case MemberName.OuterGroupingDynamicPathCount:
					writer.Write(m_outerGroupingDynamicPathCount);
					break;
				case MemberName.InnerGroupingMaximumDynamicLevel:
					writer.Write(m_innerGroupingMaximumDynamicLevel);
					break;
				case MemberName.InnerGroupingDynamicMemberCount:
					writer.Write(m_innerGroupingDynamicMemberCount);
					break;
				case MemberName.InnerGroupingDynamicPathCount:
					writer.Write(m_innerGroupingDynamicPathCount);
					break;
				case MemberName.TextboxesInScope:
					writer.Write(m_textboxesInScope);
					break;
				case MemberName.VariablesInScope:
					writer.Write(m_variablesInScope);
					break;
				case MemberName.PageBreak:
					writer.Write(m_pageBreak);
					break;
				case MemberName.PageName:
					writer.Write(m_pageName);
					break;
				case MemberName.DataScopeInfo:
					writer.Write(m_dataScopeInfo);
					break;
				case MemberName.RowDomainScopeCount:
					writer.Write(RowDomainScopeCount);
					break;
				case MemberName.ColumnDomainScopeCount:
					writer.Write(ColumnDomainScopeCount);
					break;
				case MemberName.IsMatrixIDC:
					writer.Write(m_isMatrixIDC);
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
				case MemberName.DataSetName:
					m_dataSetName = reader.ReadString();
					break;
				case MemberName.NoRowsMessage:
					m_noRowsMessage = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ColumnCount:
					m_columnCount = reader.ReadInt32();
					break;
				case MemberName.RowCount:
					m_rowCount = reader.ReadInt32();
					break;
				case MemberName.ProcessingInnerGrouping:
					m_processingInnerGrouping = (ProcessingInnerGroupings)reader.ReadEnum();
					break;
				case MemberName.RepeatSiblings:
					m_repeatSiblings = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.Sorting:
					m_sorting = (Sorting)reader.ReadRIFObject();
					break;
				case MemberName.Filters:
					m_filters = reader.ReadGenericListOfRIFObjects<Filter>();
					break;
				case MemberName.Aggregates:
					m_aggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				case MemberName.PostSortAggregates:
					m_postSortAggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				case MemberName.RunningValues:
					m_runningValues = reader.ReadGenericListOfRIFObjects<RunningValueInfo>();
					break;
				case MemberName.CellAggregates:
					m_cellAggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				case MemberName.CellPostSortAggregates:
					m_cellPostSortAggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				case MemberName.CellRunningValues:
					m_cellRunningValues = reader.ReadGenericListOfRIFObjects<RunningValueInfo>();
					break;
				case MemberName.UserSortExpressions:
					m_userSortExpressions = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.DetailSortFiltersInScope:
					m_detailSortFiltersInScope = reader.ReadInt32PrimitiveListHashtable<InScopeSortFilterHashtable, int>();
					break;
				case MemberName.PageBreakLocation:
					m_pageBreak = new PageBreak();
					m_pageBreak.BreakLocation = (PageBreakLocation)reader.ReadEnum();
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
				case MemberName.OuterGroupingMaximumDynamicLevel:
					m_outerGroupingMaximumDynamicLevel = reader.ReadInt32();
					break;
				case MemberName.OuterGroupingDynamicMemberCount:
					m_outerGroupingDynamicMemberCount = reader.ReadInt32();
					break;
				case MemberName.OuterGroupingDynamicPathCount:
					m_outerGroupingDynamicPathCount = reader.ReadInt32();
					break;
				case MemberName.InnerGroupingMaximumDynamicLevel:
					m_innerGroupingMaximumDynamicLevel = reader.ReadInt32();
					break;
				case MemberName.InnerGroupingDynamicMemberCount:
					m_innerGroupingDynamicMemberCount = reader.ReadInt32();
					break;
				case MemberName.InnerGroupingDynamicPathCount:
					m_innerGroupingDynamicPathCount = reader.ReadInt32();
					break;
				case MemberName.TextboxesInScope:
					m_textboxesInScope = reader.ReadByteArray();
					break;
				case MemberName.VariablesInScope:
					m_variablesInScope = reader.ReadByteArray();
					break;
				case MemberName.PageBreak:
					m_pageBreak = (PageBreak)reader.ReadRIFObject();
					break;
				case MemberName.PageName:
					m_pageName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DataScopeInfo:
					m_dataScopeInfo = (DataScopeInfo)reader.ReadRIFObject();
					break;
				case MemberName.RowDomainScopeCount:
					m_rowDomainScopeCount = reader.ReadInt32();
					break;
				case MemberName.ColumnDomainScopeCount:
					m_colDomainScopeCount = reader.ReadInt32();
					break;
				case MemberName.IsMatrixIDC:
					m_isMatrixIDC = reader.ReadBoolean();
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
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item2 in value)
			{
				MemberName memberName = item2.MemberName;
				if (memberName == MemberName.InScopeEventSources)
				{
					referenceableItems.TryGetValue(item2.RefID, out IReferenceable value2);
					IInScopeEventSource item = (IInScopeEventSource)value2;
					if (m_inScopeEventSources == null)
					{
						m_inScopeEventSources = new List<IInScopeEventSource>();
					}
					m_inScopeEventSources.Add(item);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion;
		}

		internal abstract object EvaluateNoRowsMessageExpression();

		internal string EvaluateNoRowsMessage(Microsoft.ReportingServices.OnDemandReportRendering.DataRegionInstance romInstance, OnDemandProcessingContext odpContext)
		{
			odpContext.SetupContext(this, romInstance);
			return odpContext.ReportRuntime.EvaluateDataRegionNoRowsExpression(this, ObjectType, m_name, "NoRowsMessage");
		}

		internal string EvaluatePageName(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateDataRegionPageNameExpression(this, m_pageName, ObjectType, base.Name);
		}

		protected void DataRegionSetExprHost(ReportItemExprHost exprHost, SortExprHost sortExprHost, IList<FilterExprHost> FilterHostsRemotable, IndexedExprHost UserSortExpressionsHost, PageBreakExprHost pageBreakExprHost, IList<JoinConditionExprHost> joinConditionExprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null, "(exprHost != null)");
			ReportItemSetExprHost(exprHost, reportObjectModel);
			if (sortExprHost != null)
			{
				Global.Tracer.Assert(m_sorting != null, "(null != m_sorting)");
				m_sorting.SetExprHost(sortExprHost, reportObjectModel);
			}
			if (FilterHostsRemotable != null)
			{
				Global.Tracer.Assert(m_filters != null, "(m_filters != null)");
				int count = m_filters.Count;
				for (int i = 0; i < count; i++)
				{
					m_filters[i].SetExprHost(FilterHostsRemotable, reportObjectModel);
				}
			}
			UserSortExpressionsHost?.SetReportObjectModel(reportObjectModel);
			if (m_pageBreak != null && pageBreakExprHost != null)
			{
				m_pageBreak.SetExprHost(pageBreakExprHost, reportObjectModel);
			}
			if (m_dataScopeInfo != null && m_dataScopeInfo.JoinInfo != null && joinConditionExprHost != null)
			{
				m_dataScopeInfo.JoinInfo.SetJoinConditionExprHost(joinConditionExprHost, reportObjectModel);
			}
		}

		internal abstract void DataRegionContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions);

		internal void SaveOuterGroupingAggregateRowInfo(int dynamicIndex, OnDemandProcessingContext odpContext)
		{
			Global.Tracer.Assert(m_outerGroupingAggregateRowInfo != null, "(null != m_outerGroupingAggregateRowInfo)");
			if (m_outerGroupingAggregateRowInfo[dynamicIndex] == null)
			{
				m_outerGroupingAggregateRowInfo[dynamicIndex] = new AggregateRowInfo();
			}
			m_outerGroupingAggregateRowInfo[dynamicIndex].SaveAggregateInfo(odpContext);
		}

		internal void SetDataTablixAggregateRowInfo(AggregateRowInfo aggregateRowInfo)
		{
			m_dataTablixAggregateRowInfo = aggregateRowInfo;
		}

		internal void SetCellAggregateRowInfo(int dynamicIndex, OnDemandProcessingContext odpContext)
		{
			Global.Tracer.Assert(m_outerGroupingAggregateRowInfo != null && m_dataTablixAggregateRowInfo != null, "(null != m_outerGroupingAggregateRowInfo && null != m_dataTablixAggregateRowInfo)");
			m_dataTablixAggregateRowInfo.CombineAggregateInfo(odpContext, m_outerGroupingAggregateRowInfo[dynamicIndex]);
		}

		internal void ResetInstancePathCascade()
		{
			int num = (RowMembers != null) ? RowMembers.Count : 0;
			for (int i = 0; i < num; i++)
			{
				RowMembers[i].ResetInstancePathCascade();
			}
			num = ((ColumnMembers != null) ? ColumnMembers.Count : 0);
			for (int j = 0; j < num; j++)
			{
				ColumnMembers[j].ResetInstancePathCascade();
			}
		}

		internal void ResetInstanceIndexes()
		{
			m_currentCellInnerIndex = 0;
			m_sequentialColMemberInstanceIndex = 0;
			m_sequentialRowMemberInstanceIndex = 0;
			m_outerGroupingIndexes = new int[OuterGroupingDynamicMemberCount];
			m_currentOuterGroupRootObjs = new IReference<RuntimeDataTablixGroupRootObj>[OuterGroupingDynamicMemberCount];
			m_outerGroupingAggregateRowInfo = new AggregateRowInfo[OuterGroupingDynamicMemberCount];
			m_currentOuterGroupRoot = null;
		}

		internal void UpdateOuterGroupingIndexes(IReference<RuntimeDataTablixGroupRootObj> groupRoot, int groupLeafIndex)
		{
			int hierarchyDynamicIndex = groupRoot.Value().HierarchyDef.HierarchyDynamicIndex;
			m_currentOuterGroupRootObjs[hierarchyDynamicIndex] = groupRoot;
			m_outerGroupingIndexes[hierarchyDynamicIndex] = groupLeafIndex;
		}

		internal void ResetOuterGroupingIndexesForOuterPeerGroup(int index)
		{
			for (int i = index; i < OuterGroupingDynamicMemberCount; i++)
			{
				m_currentOuterGroupRootObjs[i] = null;
				m_outerGroupingIndexes[i] = 0;
			}
		}

		internal void ResetOuterGroupingAggregateRowInfo()
		{
			Global.Tracer.Assert(m_outerGroupingAggregateRowInfo != null, "(null != m_outerGroupingAggregateRowInfo)");
			for (int i = 0; i < m_outerGroupingAggregateRowInfo.Length; i++)
			{
				m_outerGroupingAggregateRowInfo[i] = null;
			}
		}

		internal int AddMemberInstance(bool isColumn)
		{
			if (isColumn)
			{
				return ++m_sequentialColMemberInstanceIndex;
			}
			return m_sequentialRowMemberInstanceIndex;
		}

		internal void AddCell()
		{
			m_currentCellInnerIndex++;
		}

		internal void NewOuterCells()
		{
			if (0 < m_currentCellInnerIndex)
			{
				m_currentCellInnerIndex = 0;
			}
		}

		internal void ResetTopLevelDynamicMemberInstanceCount()
		{
			ResetTopLevelDynamicMemberInstanceCount(RowMembers);
			ResetTopLevelDynamicMemberInstanceCount(ColumnMembers);
		}

		private void ResetTopLevelDynamicMemberInstanceCount(HierarchyNodeList topLevelMembers)
		{
			if (topLevelMembers == null)
			{
				return;
			}
			int count = topLevelMembers.Count;
			for (int i = 0; i < count; i++)
			{
				if (!topLevelMembers[i].IsStatic)
				{
					topLevelMembers[i].InstanceCount = 0;
				}
			}
		}
	}
}

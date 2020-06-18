using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal abstract class Cell : IDOwner, IAggregateHolder, IRunningValueHolder, IPersistable, IIndexedInCollection, IRIFReportScope, IInstancePath, IGloballyReferenceable, IGlobalIDOwner, IRIFDataScope, IRIFReportIntersectionScope, IRIFReportDataScope
	{
		protected int m_exprHostID = -1;

		protected int m_parentRowID = -1;

		protected int m_parentColumnID = -1;

		protected int m_indexInCollection = -1;

		protected bool m_hasInnerGroupTreeHierarchy;

		[Reference]
		protected DataRegion m_dataRegionDef;

		protected List<int> m_aggregateIndexes;

		protected List<int> m_postSortAggregateIndexes;

		protected List<int> m_runningValueIndexes;

		private bool m_needToCacheDataRows;

		private byte[] m_textboxesInScope;

		private byte[] m_variablesInScope;

		private List<IInScopeEventSource> m_inScopeEventSources;

		protected bool m_inDynamicRowAndColumnContext;

		protected DataScopeInfo m_dataScopeInfo;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		protected IDOwner m_parentColumnIDOwner;

		[NonSerialized]
		protected List<DataAggregateInfo> m_aggregates;

		[NonSerialized]
		protected List<DataAggregateInfo> m_postSortAggregates;

		[NonSerialized]
		protected List<RunningValueInfo> m_runningValues;

		[NonSerialized]
		protected DataScopeInfo m_canonicalDataScopeInfo;

		[NonSerialized]
		private IRIFReportDataScope m_parentReportScope;

		[NonSerialized]
		private IRIFReportDataScope m_parentColumnReportScope;

		[NonSerialized]
		private IReference<IOnDemandScopeInstance> m_currentStreamingScopeInstance;

		[NonSerialized]
		private SyntheticTriangulatedCellReference m_cachedSyntheticCellReference;

		[NonSerialized]
		private IReference<IOnDemandScopeInstance> m_cachedNoRowsStreamingScopeInstance;

		string IRIFDataScope.Name => null;

		public DataScopeInfo DataScopeInfo => m_dataScopeInfo;

		public abstract Microsoft.ReportingServices.ReportProcessing.ObjectType DataScopeObjectType
		{
			get;
		}

		internal DataScopeInfo CanonicalDataScopeInfo
		{
			get
			{
				return m_canonicalDataScopeInfo;
			}
			set
			{
				m_canonicalDataScopeInfo = value;
			}
		}

		internal int ExpressionHostID
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

		internal int ParentRowMemberID => m_parentRowID;

		internal int ParentColumnMemberID => m_parentColumnID;

		internal DataRegion DataRegionDef => m_dataRegionDef;

		internal List<int> AggregateIndexes => m_aggregateIndexes;

		internal List<int> PostSortAggregateIndexes => m_postSortAggregateIndexes;

		internal List<int> RunningValueIndexes => m_runningValueIndexes;

		internal bool HasInnerGroupTreeHierarchy => m_hasInnerGroupTreeHierarchy;

		internal bool SimpleGroupTreeCell
		{
			get
			{
				if (m_hasInnerGroupTreeHierarchy || m_aggregateIndexes != null || m_postSortAggregateIndexes != null || m_runningValueIndexes != null || (m_dataScopeInfo != null && m_dataScopeInfo.HasAggregatesOrRunningValues) || m_dataRegionDef.IsMatrixIDC)
				{
					return false;
				}
				return true;
			}
		}

		internal List<DataAggregateInfo> Aggregates => m_aggregates;

		internal List<DataAggregateInfo> PostSortAggregates => m_postSortAggregates;

		internal List<RunningValueInfo> RunningValues => m_runningValues;

		public override List<InstancePathItem> InstancePath
		{
			get
			{
				if (m_cachedInstancePath == null)
				{
					if (m_parentColumnIDOwner == null)
					{
						return base.InstancePath;
					}
					m_cachedInstancePath = InstancePathItem.CombineRowColPath(base.InstancePath, m_parentColumnIDOwner.InstancePath);
					m_cachedInstancePath.Add(base.InstancePathItem);
				}
				return m_cachedInstancePath;
			}
		}

		protected virtual bool IsDataRegionBodyCell => false;

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

		public IndexedInCollectionType IndexedInCollectionType => IndexedInCollectionType.Cell;

		internal List<IInScopeEventSource> InScopeEventSources => m_inScopeEventSources;

		internal bool InDynamicRowAndColumnContext => m_inDynamicRowAndColumnContext;

		internal virtual List<ReportItem> CellContentCollection => null;

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

		public bool IsDataIntersectionScope => InDynamicRowAndColumnContext;

		public bool IsScope
		{
			get
			{
				if (!IsDataIntersectionScope)
				{
					if (m_dataScopeInfo != null)
					{
						return m_dataScopeInfo.NeedsIDC;
					}
					return false;
				}
				return true;
			}
		}

		public bool IsGroup => false;

		public bool IsColumnOuterGrouping => DataRegionDef.ProcessingInnerGrouping == DataRegion.ProcessingInnerGroupings.Row;

		public IRIFReportDataScope ParentReportScope
		{
			get
			{
				if (IsDataIntersectionScope)
				{
					return null;
				}
				if (m_parentReportScope == null)
				{
					IRIFReportDataScope parentReportScope = IDOwner.FindReportDataScope(base.ParentInstancePath);
					IRIFReportDataScope iRIFReportDataScope = IDOwner.FindReportDataScope(m_parentColumnIDOwner);
					if (iRIFReportDataScope is ReportHierarchyNode)
					{
						m_parentReportScope = iRIFReportDataScope;
					}
					else
					{
						m_parentReportScope = parentReportScope;
					}
				}
				return m_parentReportScope;
			}
		}

		public IRIFReportDataScope ParentRowReportScope
		{
			get
			{
				if (IsDataIntersectionScope)
				{
					if (m_parentReportScope == null)
					{
						m_parentReportScope = IDOwner.FindReportDataScope(base.ParentInstancePath);
					}
					return m_parentReportScope;
				}
				return null;
			}
		}

		public IRIFReportDataScope ParentColumnReportScope
		{
			get
			{
				if (IsDataIntersectionScope)
				{
					if (m_parentColumnReportScope == null)
					{
						m_parentColumnReportScope = IDOwner.FindReportDataScope(m_parentColumnIDOwner);
					}
					return m_parentColumnReportScope;
				}
				return null;
			}
		}

		public IReference<IOnDemandScopeInstance> CurrentStreamingScopeInstance => m_currentStreamingScopeInstance;

		public bool IsBoundToStreamingScopeInstance => m_currentStreamingScopeInstance != null;

		protected abstract Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode ExprHostDataRegionMode
		{
			get;
		}

		internal Cell()
		{
		}

		internal Cell(int id, DataRegion dataRegion)
			: base(id)
		{
			m_dataRegionDef = dataRegion;
			m_aggregates = new List<DataAggregateInfo>();
			m_postSortAggregates = new List<DataAggregateInfo>();
			m_runningValues = new List<RunningValueInfo>();
			m_dataScopeInfo = new DataScopeInfo(id);
		}

		protected override InstancePathItem CreateInstancePathItem()
		{
			return new InstancePathItem(InstancePathItemType.Cell, IndexInCollection);
		}

		bool IRIFReportScope.VariableInScope(int sequenceIndex)
		{
			return SequenceIndex.GetBit(m_variablesInScope, sequenceIndex, returnValueIfSequenceNull: true);
		}

		bool IRIFReportScope.TextboxInScope(int sequenceIndex)
		{
			return SequenceIndex.GetBit(m_textboxesInScope, sequenceIndex, returnValueIfSequenceNull: true);
		}

		void IRIFReportScope.ResetTextBoxImpls(OnDemandProcessingContext context)
		{
		}

		void IRIFReportScope.AddInScopeTextBox(TextBox textbox)
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

		public bool IsSameOrChildScopeOf(IRIFReportDataScope candidateScope)
		{
			return DataScopeInfo.IsSameOrChildScope(this, candidateScope);
		}

		public bool IsChildScopeOf(IRIFReportDataScope candidateScope)
		{
			return DataScopeInfo.IsChildScopeOf(this, candidateScope);
		}

		public void BindToStreamingScopeInstance(IReference<IOnDemandMemberInstance> parentRowScopeInstance, IReference<IOnDemandMemberInstance> parentColumnScopeInstance)
		{
			if (m_cachedSyntheticCellReference == null)
			{
				m_cachedSyntheticCellReference = new SyntheticTriangulatedCellReference(parentRowScopeInstance, parentColumnScopeInstance);
			}
			else
			{
				m_cachedSyntheticCellReference.UpdateGroupLeafReferences(parentRowScopeInstance, parentColumnScopeInstance);
			}
			m_currentStreamingScopeInstance = m_cachedSyntheticCellReference;
		}

		public void ResetAggregates(AggregatesImpl reportOmAggregates)
		{
			ResetAggregates(reportOmAggregates, m_dataRegionDef.CellAggregates, m_aggregateIndexes);
			ResetAggregates(reportOmAggregates, m_dataRegionDef.CellPostSortAggregates, m_postSortAggregateIndexes);
			ResetAggregates(reportOmAggregates, m_dataRegionDef.CellRunningValues, m_runningValueIndexes);
			if (m_dataScopeInfo != null)
			{
				m_dataScopeInfo.ResetAggregates(reportOmAggregates);
			}
		}

		private void ResetAggregates<T>(AggregatesImpl reportOmAggregates, List<T> aggregateDefs, List<int> aggregateIndices) where T : DataAggregateInfo
		{
			if (aggregateDefs != null && aggregateIndices != null)
			{
				for (int i = 0; i < aggregateIndices.Count; i++)
				{
					int index = aggregateIndices[i];
					reportOmAggregates.Reset(aggregateDefs[index]);
				}
			}
		}

		public bool HasServerAggregate(string aggregateName)
		{
			if (m_aggregateIndexes == null)
			{
				return false;
			}
			foreach (int aggregateIndex in m_aggregateIndexes)
			{
				if (DataScopeInfo.IsTargetServerAggregate(m_dataRegionDef.CellAggregates[aggregateIndex], aggregateName))
				{
					return true;
				}
			}
			return false;
		}

		public void BindToStreamingScopeInstance(IReference<IOnDemandScopeInstance> scopeInstance)
		{
			m_currentStreamingScopeInstance = scopeInstance;
		}

		public void BindToNoRowsScopeInstance(OnDemandProcessingContext odpContext)
		{
			if (m_cachedNoRowsStreamingScopeInstance == null)
			{
				StreamingNoRowsCellInstance scopeInstance = new StreamingNoRowsCellInstance(odpContext, this);
				m_cachedNoRowsStreamingScopeInstance = new SyntheticOnDemandScopeInstanceReference(scopeInstance);
			}
			m_currentStreamingScopeInstance = m_cachedNoRowsStreamingScopeInstance;
		}

		public void ClearStreamingScopeInstanceBinding()
		{
			m_currentStreamingScopeInstance = null;
		}

		internal void TraverseScopes(IRIFScopeVisitor visitor, int rowIndex, int colIndex)
		{
			visitor.PreVisit(this, rowIndex, colIndex);
			TraverseNestedScopes(visitor);
			visitor.PostVisit(this, rowIndex, colIndex);
		}

		protected virtual void TraverseNestedScopes(IRIFScopeVisitor visitor)
		{
		}

		List<DataAggregateInfo> IAggregateHolder.GetAggregateList()
		{
			return m_aggregates;
		}

		List<DataAggregateInfo> IAggregateHolder.GetPostSortAggregateList()
		{
			return m_postSortAggregates;
		}

		List<RunningValueInfo> IRunningValueHolder.GetRunningValueList()
		{
			return m_runningValues;
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

		void IRunningValueHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(m_runningValues != null, "(null != m_runningValues)");
			if (m_runningValues.Count == 0)
			{
				m_runningValues = null;
			}
		}

		internal void GenerateAggregateIndexes(Dictionary<string, int> aggregateIndexMapping, Dictionary<string, int> postSortAggregateIndexMapping, Dictionary<string, int> runningValueIndexMapping)
		{
			if (m_aggregates != null)
			{
				GenerateAggregateIndexes(m_aggregates, aggregateIndexMapping, ref m_aggregateIndexes);
			}
			if (m_postSortAggregates != null)
			{
				GenerateAggregateIndexes(m_postSortAggregates, postSortAggregateIndexMapping, ref m_postSortAggregateIndexes);
			}
			if (m_runningValues != null)
			{
				GenerateAggregateIndexes(m_runningValues, runningValueIndexMapping, ref m_runningValueIndexes);
			}
		}

		private static void GenerateAggregateIndexes<AggregateType>(List<AggregateType> cellAggregates, Dictionary<string, int> aggregateIndexMapping, ref List<int> aggregateIndexes) where AggregateType : DataAggregateInfo
		{
			int count = cellAggregates.Count;
			if (count == 0)
			{
				return;
			}
			aggregateIndexes = new List<int>();
			for (int i = 0; i < count; i++)
			{
				AggregateType val = cellAggregates[i];
				if (aggregateIndexMapping.TryGetValue(val.Name, out int value))
				{
					aggregateIndexes.Add(value);
				}
			}
		}

		internal static bool ContainsInnerGroupTreeHierarchy(ReportItem cellContents)
		{
			return cellContents?.IsOrContainsDataRegionOrSubReport() ?? false;
		}

		internal void Initialize(int parentRowID, int parentColumnID, int rowindex, int colIndex, InitializationContext context)
		{
			bool num = IsDataRegionBodyCell && context.IsDataRegionCellScope;
			if (num)
			{
				context.RegisterIndividualCellScope(this);
				m_inDynamicRowAndColumnContext = context.IsDataRegionCellScope;
				if (DataScopeInfo.JoinInfo != null && DataScopeInfo.JoinInfo is IntersectJoinInfo)
				{
					m_dataRegionDef.IsMatrixIDC = true;
				}
			}
			else
			{
				context.RegisterNonScopeCell(this);
			}
			m_textboxesInScope = context.GetCurrentReferencableTextboxes();
			m_variablesInScope = context.GetCurrentReferencableVariables();
			m_parentRowID = parentRowID;
			m_parentColumnID = parentColumnID;
			context.SetIndexInCollection(this);
			StartExprHost(context);
			if (m_dataScopeInfo != null)
			{
				m_dataScopeInfo.Initialize(context, this);
			}
			InternalInitialize(parentRowID, parentColumnID, rowindex, colIndex, context);
			EndExprHost(context);
			m_dataScopeInfo.ValidateScopeRulesForIdc(context, this);
			if (context.EvaluateAtomicityCondition(HasAggregatesForAtomicityCheck(), this, AtomicityReason.Aggregates) || context.EvaluateAtomicityCondition(context.HasMultiplePeerChildScopes(this), this, AtomicityReason.PeerChildScopes))
			{
				context.FoundAtomicScope(this);
			}
			else
			{
				m_dataScopeInfo.IsDecomposable = true;
			}
			if (num)
			{
				context.UnRegisterIndividualCellScope(this);
			}
			else
			{
				context.UnRegisterNonScopeCell(this);
			}
		}

		internal abstract void InternalInitialize(int parentRowID, int parentColumnID, int rowindex, int colIndex, InitializationContext context);

		protected virtual void StartExprHost(InitializationContext context)
		{
			context.ExprHostBuilder.DataCellStart(ExprHostDataRegionMode);
		}

		protected virtual void EndExprHost(InitializationContext context)
		{
			m_exprHostID = context.ExprHostBuilder.DataCellEnd(ExprHostDataRegionMode);
		}

		private bool HasAggregatesForAtomicityCheck()
		{
			if (!DataScopeInfo.HasNonServerAggregates(m_aggregates) && !DataScopeInfo.HasAggregates(m_postSortAggregates) && !DataScopeInfo.HasAggregates(m_runningValues))
			{
				return m_dataScopeInfo.HasAggregatesOrRunningValues;
			}
			return true;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			Cell cell = (Cell)base.PublishClone(context);
			cell.m_aggregates = new List<DataAggregateInfo>();
			cell.m_postSortAggregates = new List<DataAggregateInfo>();
			cell.m_runningValues = new List<RunningValueInfo>();
			cell.m_dataScopeInfo = m_dataScopeInfo.PublishClone(context, cell.ID);
			context.AddAggregateHolder(cell);
			context.AddRunningValueHolder(cell);
			if (context.CurrentDataRegionClone != null)
			{
				cell.m_dataRegionDef = context.CurrentDataRegionClone;
			}
			return cell;
		}

		internal void BaseSetExprHost(CellExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			if (m_dataScopeInfo != null && m_dataScopeInfo.JoinInfo != null && exprHost.JoinConditionExprHostsRemotable != null)
			{
				m_dataScopeInfo.JoinInfo.SetJoinConditionExprHost(exprHost.JoinConditionExprHostsRemotable, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ParentRowID, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, Token.Reference));
			list.Add(new MemberInfo(MemberName.ParentColumnID, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, Token.Reference));
			list.Add(new MemberInfo(MemberName.IndexInCollection, Token.Int32));
			list.Add(new MemberInfo(MemberName.HasInnerGroupTreeHierarchy, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DataRegionDef, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion, Token.Reference));
			list.Add(new MemberInfo(MemberName.AggregateIndexes, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
			list.Add(new MemberInfo(MemberName.PostSortAggregateIndexes, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
			list.Add(new MemberInfo(MemberName.RunningValueIndexes, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
			list.Add(new MemberInfo(MemberName.NeedToCacheDataRows, Token.Boolean));
			list.Add(new MemberInfo(MemberName.InScopeEventSources, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IInScopeEventSource));
			list.Add(new MemberInfo(MemberName.InDynamicRowAndColumnContext, Token.Boolean));
			list.Add(new MemberInfo(MemberName.TextboxesInScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.VariablesInScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.DataScopeInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataScopeInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Cell, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.ParentRowID:
					writer.WriteReferenceID(m_parentRowID);
					break;
				case MemberName.ParentColumnID:
					writer.WriteReferenceID(m_parentColumnID);
					break;
				case MemberName.IndexInCollection:
					writer.Write(m_indexInCollection);
					break;
				case MemberName.HasInnerGroupTreeHierarchy:
					writer.Write(m_hasInnerGroupTreeHierarchy);
					break;
				case MemberName.DataRegionDef:
					Global.Tracer.Assert(m_dataRegionDef != null, "(null != m_dataRegionDef)");
					writer.WriteReference(m_dataRegionDef);
					break;
				case MemberName.AggregateIndexes:
					writer.WriteListOfPrimitives(m_aggregateIndexes);
					break;
				case MemberName.PostSortAggregateIndexes:
					writer.WriteListOfPrimitives(m_postSortAggregateIndexes);
					break;
				case MemberName.RunningValueIndexes:
					writer.WriteListOfPrimitives(m_runningValueIndexes);
					break;
				case MemberName.NeedToCacheDataRows:
					writer.Write(m_needToCacheDataRows);
					break;
				case MemberName.InScopeEventSources:
					writer.WriteListOfReferences(m_inScopeEventSources);
					break;
				case MemberName.InDynamicRowAndColumnContext:
					writer.Write(m_inDynamicRowAndColumnContext);
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
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.ParentRowID:
					m_parentIDOwner = reader.ReadReference<IDOwner>(this);
					if (m_parentIDOwner != null)
					{
						m_parentRowID = m_parentIDOwner.ID;
					}
					break;
				case MemberName.ParentColumnID:
					m_parentColumnIDOwner = reader.ReadReference<IDOwner>(this);
					if (m_parentColumnIDOwner != null)
					{
						m_parentColumnID = m_parentColumnIDOwner.ID;
					}
					break;
				case MemberName.IndexInCollection:
					m_indexInCollection = reader.ReadInt32();
					break;
				case MemberName.HasInnerGroupTreeHierarchy:
					m_hasInnerGroupTreeHierarchy = reader.ReadBoolean();
					break;
				case MemberName.DataRegionDef:
					m_dataRegionDef = reader.ReadReference<DataRegion>(this);
					break;
				case MemberName.AggregateIndexes:
					m_aggregateIndexes = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.PostSortAggregateIndexes:
					m_postSortAggregateIndexes = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.RunningValueIndexes:
					m_runningValueIndexes = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.NeedToCacheDataRows:
					m_needToCacheDataRows = reader.ReadBoolean();
					break;
				case MemberName.InScopeEventSources:
					m_inScopeEventSources = reader.ReadGenericListOfReferences<IInScopeEventSource>(this);
					break;
				case MemberName.InDynamicRowAndColumnContext:
					m_inDynamicRowAndColumnContext = reader.ReadBoolean();
					break;
				case MemberName.TextboxesInScope:
					m_textboxesInScope = reader.ReadByteArray();
					break;
				case MemberName.VariablesInScope:
					m_variablesInScope = reader.ReadByteArray();
					break;
				case MemberName.DataScopeInfo:
					m_dataScopeInfo = reader.ReadRIFObject<DataScopeInfo>();
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
				case MemberName.ParentRowID:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item2.RefID), "ParentRowID");
					m_parentIDOwner = (IDOwner)referenceableItems[item2.RefID];
					m_parentRowID = item2.RefID;
					break;
				case MemberName.ParentColumnID:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item2.RefID), "ParentColumnID");
					m_parentColumnIDOwner = (IDOwner)referenceableItems[item2.RefID];
					m_parentColumnID = item2.RefID;
					break;
				case MemberName.DataRegionDef:
				{
					referenceableItems.TryGetValue(item2.RefID, out IReferenceable value3);
					Global.Tracer.Assert(value3 != null && ((ReportItem)value3).IsDataRegion, "DataRegionDef");
					m_dataRegionDef = (DataRegion)value3;
					break;
				}
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
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Cell;
		}
	}
}

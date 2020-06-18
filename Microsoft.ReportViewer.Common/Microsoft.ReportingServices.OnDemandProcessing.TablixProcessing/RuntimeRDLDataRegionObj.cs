using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeRDLDataRegionObj : RuntimeDataRegionObj, IHierarchyObj, IStorable, IPersistable, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner, IDataRowSortOwner, IDataRowHolder, IDataCorrelation
	{
		[StaticReference]
		protected Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion m_dataRegionDef;

		protected IReference<IScope> m_outerScope;

		protected DataFieldRow m_firstRow;

		protected bool m_firstRowIsAggregate;

		[StaticReference]
		protected Filters m_filters;

		protected List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_nonCustomAggregates;

		protected BucketedDataAggregateObjs m_aggregatesOfAggregates;

		protected List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_customAggregates;

		protected DataActions m_dataAction;

		protected DataActions m_outerDataAction;

		protected List<string> m_runningValues;

		protected List<string> m_previousValues;

		protected Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] m_runningValueValues;

		protected Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] m_runningValueOfAggregateValues;

		protected List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_postSortAggregates;

		protected BucketedDataAggregateObjs m_postSortAggregatesOfAggregates;

		protected ScalableList<DataFieldRow> m_dataRows;

		protected DataActions m_innerDataAction;

		protected RuntimeUserSortTargetInfo m_userSortTargetInfo;

		protected int[] m_sortFilterExpressionScopeInfoIndices;

		protected bool m_inDataRowSortPhase;

		protected BTree m_sortedDataRowTree;

		protected RuntimeExpressionInfo m_dataRowSortExpression;

		protected bool m_hasProcessedAggregateRow;

		private static Declaration m_declaration = GetDeclaration();

		protected override IReference<IScope> OuterScope => m_outerScope;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion DataRegionDef => m_dataRegionDef;

		protected override string ScopeName => m_dataRegionDef.Name;

		internal override bool TargetForNonDetailSort
		{
			get
			{
				if (m_userSortTargetInfo != null && m_userSortTargetInfo.TargetForNonDetailSort)
				{
					return true;
				}
				return m_outerScope.Value().TargetForNonDetailSort;
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

		internal override IRIFReportScope RIFReportScope => m_dataRegionDef;

		IReference<IHierarchyObj> IHierarchyObj.HierarchyRoot => (IReference<IHierarchyObj>)m_selfReference;

		OnDemandProcessingContext IHierarchyObj.OdpContext => m_odpContext;

		BTree IHierarchyObj.SortTree
		{
			get
			{
				if (m_userSortTargetInfo != null)
				{
					return m_userSortTargetInfo.SortTree;
				}
				return null;
			}
		}

		int IHierarchyObj.ExpressionIndex => 0;

		List<int> IHierarchyObj.SortFilterInfoIndices
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

		bool IHierarchyObj.IsDetail => false;

		bool IHierarchyObj.InDataRowSortPhase => m_inDataRowSortPhase;

		Microsoft.ReportingServices.ReportIntermediateFormat.Sorting IDataRowSortOwner.SortingDef => m_dataRegionDef.Sorting;

		OnDemandProcessingContext IDataRowSortOwner.OdpContext => m_odpContext;

		public override int Size => base.Size + ItemSizes.SizeOf(m_outerScope) + ItemSizes.SizeOf(m_firstRow) + 1 + ItemSizes.ReferenceSize + ItemSizes.SizeOf(m_nonCustomAggregates) + ItemSizes.SizeOf(m_customAggregates) + 4 + 4 + ItemSizes.SizeOf(m_runningValues) + ItemSizes.SizeOf(m_runningValueValues) + ItemSizes.SizeOf(m_runningValueOfAggregateValues) + ItemSizes.SizeOf(m_postSortAggregates) + ItemSizes.SizeOf(m_dataRows) + 4 + ItemSizes.SizeOf(m_userSortTargetInfo) + ItemSizes.SizeOf(m_sortFilterExpressionScopeInfoIndices) + 1 + ItemSizes.SizeOf(m_sortedDataRowTree) + ItemSizes.SizeOf(m_dataRowSortExpression) + ItemSizes.SizeOf(m_aggregatesOfAggregates) + ItemSizes.SizeOf(m_postSortAggregatesOfAggregates) + 1;

		internal RuntimeRDLDataRegionObj()
		{
		}

		internal RuntimeRDLDataRegionObj(IReference<IScope> outerScope, Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, ref DataActions dataAction, OnDemandProcessingContext odpContext, bool onePassProcess, List<Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo> runningValues, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, int level)
			: base(odpContext, objectType, level)
		{
			m_dataRegionDef = dataRegionDef;
			m_outerScope = outerScope;
			RuntimeDataRegionObj.CreateAggregates(m_odpContext, dataRegionDef.Aggregates, ref m_nonCustomAggregates, ref m_customAggregates);
			if (dataRegionDef.DataScopeInfo != null)
			{
				RuntimeDataRegionObj.CreateAggregates(m_odpContext, dataRegionDef.DataScopeInfo.AggregatesOfAggregates, ref m_aggregatesOfAggregates);
			}
			if (dataRegionDef.Filters != null)
			{
				m_filters = new Filters(Filters.FilterTypes.DataRegionFilter, (IReference<Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>)base.SelfReference, dataRegionDef.Filters, dataRegionDef.ObjectType, dataRegionDef.Name, m_odpContext, level + 1);
				return;
			}
			m_outerDataAction = dataAction;
			m_dataAction = dataAction;
			dataAction = DataActions.None;
		}

		internal override bool IsTargetForSort(int index, bool detailSort)
		{
			if (m_userSortTargetInfo != null && m_userSortTargetInfo.IsTargetForSort(index, detailSort))
			{
				return true;
			}
			return m_outerScope.Value().IsTargetForSort(index, detailSort);
		}

		IHierarchyObj IHierarchyObj.CreateHierarchyObjForSortTree()
		{
			if (m_inDataRowSortPhase)
			{
				return new RuntimeDataRowSortHierarchyObj(this, base.Depth + 1);
			}
			return new RuntimeSortHierarchyObj(this, m_depth + 1);
		}

		ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
		{
			if (m_inDataRowSortPhase)
			{
				m_odpContext.ErrorContext.Register(ProcessingErrorCode.rsComparisonError, Severity.Error, m_dataRegionDef.ObjectType, m_dataRegionDef.Name, propertyName);
				return m_odpContext.ErrorContext.Messages;
			}
			return m_odpContext.RegisterComparisonErrorForSortFilterEvent(propertyName);
		}

		void IHierarchyObj.NextRow(IHierarchyObj owner)
		{
			Global.Tracer.Assert(condition: false);
		}

		void IHierarchyObj.Traverse(ProcessingStages operation, ITraversalContext traversalContext)
		{
			Global.Tracer.Assert(condition: false);
		}

		void IHierarchyObj.ReadRow()
		{
			ReadRow(DataActions.UserSort, null);
		}

		void IHierarchyObj.ProcessUserSort()
		{
			m_odpContext.ProcessUserSortForTarget((RuntimeRDLDataRegionObjReference)base.SelfReference, ref m_dataRows, m_userSortTargetInfo.TargetForNonDetailSort);
			m_dataAction &= ~DataActions.UserSort;
			if (m_userSortTargetInfo.TargetForNonDetailSort)
			{
				m_userSortTargetInfo.ResetTargetForNonDetailSort();
				m_userSortTargetInfo.EnterProcessUserSortPhase(m_odpContext);
				DataActions innerDataAction = m_innerDataAction;
				ConstructRuntimeStructure(ref innerDataAction, m_odpContext.ReportDefinition.MergeOnePass);
				if (m_dataAction != 0)
				{
					m_dataRows = new ScalableList<DataFieldRow>(m_depth, m_odpContext.TablixProcessingScalabilityCache);
				}
				ScopeFinishSorting(ref m_firstRow, m_userSortTargetInfo);
				m_userSortTargetInfo.LeaveProcessUserSortPhase(m_odpContext);
			}
		}

		void IHierarchyObj.MarkSortInfoProcessed(List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo)
		{
			if (m_userSortTargetInfo != null)
			{
				m_userSortTargetInfo.MarkSortInfoProcessed(runtimeSortFilterInfo, (IReference<IHierarchyObj>)base.SelfReference);
			}
		}

		void IHierarchyObj.AddSortInfoIndex(int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
		{
			if (m_userSortTargetInfo != null)
			{
				m_userSortTargetInfo.AddSortInfoIndex(sortInfoIndex, sortInfo);
			}
		}

		protected abstract void ConstructRuntimeStructure(ref DataActions innerDataAction, bool onePassProcess);

		protected DataActions HandleSortFilterEvent()
		{
			DataActions result = DataActions.None;
			if (m_odpContext.IsSortFilterTarget(DataRegionDef.IsSortFilterTarget, m_outerScope, (RuntimeRDLDataRegionObjReference)base.SelfReference, ref m_userSortTargetInfo) && m_userSortTargetInfo.TargetForNonDetailSort)
			{
				result = DataActions.UserSort;
			}
			m_odpContext.RegisterSortFilterExpressionScope(m_outerScope, base.SelfReference, DataRegionDef.IsSortFilterExpressionScope);
			return result;
		}

		internal override bool TargetScopeMatched(int index, bool detailSort)
		{
			return m_outerScope.Value().TargetScopeMatched(index, detailSort);
		}

		internal override void GetScopeValues(IReference<IHierarchyObj> targetScopeObj, List<object>[] scopeValues, ref int index)
		{
			if (targetScopeObj == null || this != targetScopeObj.Value())
			{
				m_outerScope.Value().GetScopeValues(targetScopeObj, scopeValues, ref index);
			}
		}

		internal override void NextRow()
		{
			if (m_dataRegionDef.DataScopeInfo == null || !m_dataRegionDef.DataScopeInfo.NeedsIDC)
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
			if (m_odpContext.ReportObjectModel.FieldsImpl.AggregationFieldCount == 0)
			{
				m_hasProcessedAggregateRow = true;
				RuntimeDataRegionObj.UpdateAggregates(m_odpContext, m_customAggregates, updateAndSetup: false);
			}
			if (m_odpContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
			{
				ScopeNextAggregateRow(m_userSortTargetInfo);
				return false;
			}
			NextNonAggregateRow();
			return true;
		}

		private void NextNonAggregateRow()
		{
			bool flag = true;
			if (m_filters != null)
			{
				flag = m_filters.PassFilters(new DataFieldRow(m_odpContext.ReportObjectModel.FieldsImpl, getAndSave: false));
			}
			if (flag)
			{
				((Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner)this).PostFilterNextRow();
			}
		}

		void Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner.PostFilterNextRow()
		{
			if (m_inDataRowSortPhase)
			{
				object keyValue = EvaluateDataRowSortExpression(m_dataRowSortExpression);
				m_sortedDataRowTree.NextRow(keyValue, this);
			}
			else
			{
				((IDataRowSortOwner)this).PostDataRowSortNextRow();
			}
		}

		public object EvaluateDataRowSortExpression(RuntimeExpressionInfo sortExpression)
		{
			return m_odpContext.ReportRuntime.EvaluateRuntimeExpression(sortExpression, base.ObjectType, m_dataRegionDef.Name, "Sort");
		}

		void IDataRowSortOwner.PostDataRowSortNextRow()
		{
			RuntimeDataRegionObj.CommonFirstRow(m_odpContext, ref m_firstRowIsAggregate, ref m_firstRow);
			ScopeNextNonAggregateRow(m_nonCustomAggregates, m_dataRows);
		}

		void IDataRowSortOwner.DataRowSortTraverse()
		{
			try
			{
				ITraversalContext traversalContext = new DataRowSortOwnerTraversalContext(this);
				m_sortedDataRowTree.Traverse(ProcessingStages.Grouping, m_dataRowSortExpression.Direction, traversalContext);
			}
			finally
			{
				m_inDataRowSortPhase = false;
				m_sortedDataRowTree.Dispose();
				m_sortedDataRowTree = null;
				m_dataRowSortExpression = null;
			}
		}

		internal override bool SortAndFilter(AggregateUpdateContext aggContext)
		{
			if ((SecondPassOperations.FilteringOrAggregatesOrDomainScope & m_odpContext.SecondPassOperation) != 0 && m_dataRows != null && (m_outerDataAction & DataActions.RecursiveAggregates) != 0)
			{
				ReadRows(DataActions.RecursiveAggregates, null);
				ReleaseDataRows(DataActions.RecursiveAggregates, ref m_dataAction, ref m_dataRows);
			}
			return true;
		}

		public void ReadRows(DataActions action, ITraversalContext context)
		{
			for (int i = 0; i < m_dataRows.Count; i++)
			{
				m_dataRows[i].SetFields(m_odpContext.ReportObjectModel.FieldsImpl);
				ReadRow(action, context);
			}
		}

		protected void SetupEnvironment(List<Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo> runningValues)
		{
			if (m_dataRegionDef.DataScopeInfo != null && m_dataRegionDef.DataScopeInfo.DataSet != null && m_dataRegionDef.DataScopeInfo.DataSet.DataSetCore.FieldsContext != null)
			{
				m_odpContext.ReportObjectModel.RestoreFields(m_dataRegionDef.DataScopeInfo.DataSet.DataSetCore.FieldsContext);
			}
			SetupEnvironment(m_nonCustomAggregates, m_customAggregates, m_firstRow);
			SetupAggregates(m_postSortAggregates);
			SetupRunningValues(runningValues, m_runningValueValues);
			SetupAggregates(m_aggregatesOfAggregates);
			SetupAggregates(m_postSortAggregatesOfAggregates);
			if (m_dataRegionDef.DataScopeInfo != null)
			{
				SetupRunningValues(m_dataRegionDef.DataScopeInfo.RunningValuesOfAggregates, m_runningValueOfAggregateValues);
			}
		}

		internal override bool InScope(string scope)
		{
			return DataRegionInScope(DataRegionDef, scope);
		}

		protected override int GetRecursiveLevel(string scope)
		{
			return DataRegionRecursiveLevel(DataRegionDef, scope);
		}

		protected override void GetGroupNameValuePairs(Dictionary<string, object> pairs)
		{
			DataRegionGetGroupNameValuePairs(DataRegionDef, pairs);
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
				case MemberName.DataRegionDef:
				{
					int value2 = scalabilityCache.StoreStaticReference(m_dataRegionDef);
					writer.Write(value2);
					break;
				}
				case MemberName.OuterScope:
					writer.Write(m_outerScope);
					break;
				case MemberName.FirstRow:
					writer.Write(m_firstRow);
					break;
				case MemberName.FirstRowIsAggregate:
					writer.Write(m_firstRowIsAggregate);
					break;
				case MemberName.Filters:
				{
					int value = scalabilityCache.StoreStaticReference(m_filters);
					writer.Write(value);
					break;
				}
				case MemberName.NonCustomAggregates:
					writer.Write(m_nonCustomAggregates);
					break;
				case MemberName.CustomAggregates:
					writer.Write(m_customAggregates);
					break;
				case MemberName.DataAction:
					writer.WriteEnum((int)m_dataAction);
					break;
				case MemberName.OuterDataAction:
					writer.WriteEnum((int)m_outerDataAction);
					break;
				case MemberName.RunningValues:
					writer.WriteListOfPrimitives(m_runningValues);
					break;
				case MemberName.PreviousValues:
					writer.WriteListOfPrimitives(m_previousValues);
					break;
				case MemberName.RunningValueValues:
					writer.Write(m_runningValueValues);
					break;
				case MemberName.RunningValueOfAggregateValues:
					writer.Write(m_runningValueOfAggregateValues);
					break;
				case MemberName.PostSortAggregates:
					writer.Write(m_postSortAggregates);
					break;
				case MemberName.DataRows:
					writer.Write(m_dataRows);
					break;
				case MemberName.InnerDataAction:
					writer.WriteEnum((int)m_innerDataAction);
					break;
				case MemberName.UserSortTargetInfo:
					writer.Write(m_userSortTargetInfo);
					break;
				case MemberName.SortFilterExpressionScopeInfoIndices:
					writer.Write(m_sortFilterExpressionScopeInfoIndices);
					break;
				case MemberName.InDataRowSortPhase:
					writer.Write(m_inDataRowSortPhase);
					break;
				case MemberName.SortedDataRowTree:
					writer.Write(m_sortedDataRowTree);
					break;
				case MemberName.DataRowSortExpression:
					writer.Write(m_dataRowSortExpression);
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
				case MemberName.DataRegionDef:
				{
					int id2 = reader.ReadInt32();
					m_dataRegionDef = (Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion)scalabilityCache.FetchStaticReference(id2);
					break;
				}
				case MemberName.OuterScope:
					m_outerScope = (IReference<IScope>)reader.ReadRIFObject();
					break;
				case MemberName.FirstRow:
					m_firstRow = (DataFieldRow)reader.ReadRIFObject();
					break;
				case MemberName.FirstRowIsAggregate:
					m_firstRowIsAggregate = reader.ReadBoolean();
					break;
				case MemberName.Filters:
				{
					int id = reader.ReadInt32();
					m_filters = (Filters)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.NonCustomAggregates:
					m_nonCustomAggregates = reader.ReadListOfRIFObjects<List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.CustomAggregates:
					m_customAggregates = reader.ReadListOfRIFObjects<List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.DataAction:
					m_dataAction = (DataActions)reader.ReadEnum();
					break;
				case MemberName.OuterDataAction:
					m_outerDataAction = (DataActions)reader.ReadEnum();
					break;
				case MemberName.RunningValues:
					m_runningValues = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.PreviousValues:
					m_previousValues = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.RunningValueValues:
					m_runningValueValues = reader.ReadArrayOfRIFObjects<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult>();
					break;
				case MemberName.RunningValueOfAggregateValues:
					m_runningValueOfAggregateValues = reader.ReadArrayOfRIFObjects<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult>();
					break;
				case MemberName.PostSortAggregates:
					m_postSortAggregates = reader.ReadListOfRIFObjects<List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.DataRows:
					m_dataRows = reader.ReadRIFObject<ScalableList<DataFieldRow>>();
					break;
				case MemberName.InnerDataAction:
					m_innerDataAction = (DataActions)reader.ReadEnum();
					break;
				case MemberName.UserSortTargetInfo:
					m_userSortTargetInfo = (RuntimeUserSortTargetInfo)reader.ReadRIFObject();
					break;
				case MemberName.SortFilterExpressionScopeInfoIndices:
					m_sortFilterExpressionScopeInfoIndices = reader.ReadInt32Array();
					break;
				case MemberName.InDataRowSortPhase:
					m_inDataRowSortPhase = reader.ReadBoolean();
					break;
				case MemberName.SortedDataRowTree:
					m_sortedDataRowTree = (BTree)reader.ReadRIFObject();
					break;
				case MemberName.DataRowSortExpression:
					m_dataRowSortExpression = (RuntimeExpressionInfo)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRDLDataRegionObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.DataRegionDef, Token.Int32));
				list.Add(new MemberInfo(MemberName.OuterScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IScopeReference));
				list.Add(new MemberInfo(MemberName.FirstRow, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow));
				list.Add(new MemberInfo(MemberName.FirstRowIsAggregate, Token.Boolean));
				list.Add(new MemberInfo(MemberName.Filters, Token.Int32));
				list.Add(new MemberInfo(MemberName.NonCustomAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.CustomAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.DataAction, Token.Enum));
				list.Add(new MemberInfo(MemberName.OuterDataAction, Token.Enum));
				list.Add(new MemberInfo(MemberName.RunningValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
				list.Add(new MemberInfo(MemberName.PreviousValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
				list.Add(new MemberInfo(MemberName.RunningValueValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjResult));
				list.Add(new MemberInfo(MemberName.PostSortAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.DataRows, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
				list.Add(new MemberInfo(MemberName.InnerDataAction, Token.Enum));
				list.Add(new MemberInfo(MemberName.UserSortTargetInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeUserSortTargetInfo));
				list.Add(new MemberInfo(MemberName.SortFilterExpressionScopeInfoIndices, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Int32));
				list.Add(new MemberInfo(MemberName.InDataRowSortPhase, Token.Boolean));
				list.Add(new MemberInfo(MemberName.SortedDataRowTree, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTree));
				list.Add(new MemberInfo(MemberName.DataRowSortExpression, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeExpressionInfo));
				list.Add(new MemberInfo(MemberName.AggregatesOfAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BucketedDataAggregateObjs));
				list.Add(new MemberInfo(MemberName.PostSortAggregatesOfAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BucketedDataAggregateObjs));
				list.Add(new MemberInfo(MemberName.RunningValueOfAggregateValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjResult));
				list.Add(new MemberInfo(MemberName.HasProcessedAggregateRow, Token.Boolean));
				m_declaration = new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRDLDataRegionObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObj, list);
			}
			return m_declaration;
		}
	}
}

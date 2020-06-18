using Microsoft.ReportingServices.Common;
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
	internal abstract class RuntimeDataRegionObj : IScope, IStorable, IPersistable, ISelfReferential
	{
		[StaticReference]
		protected OnDemandProcessingContext m_odpContext;

		protected Microsoft.ReportingServices.ReportProcessing.ObjectType m_objectType;

		[NonSerialized]
		protected RuntimeDataRegionObjReference m_selfReference;

		protected int m_depth;

		private static readonly Declaration m_declaration = GetDeclaration();

		public RuntimeDataRegionObjReference SelfReference => m_selfReference;

		internal OnDemandProcessingContext OdpContext => m_odpContext;

		protected abstract IReference<IScope> OuterScope
		{
			get;
		}

		protected virtual string ScopeName => null;

		internal virtual bool TargetForNonDetailSort
		{
			get
			{
				if (OuterScope != null)
				{
					return OuterScope.Value().TargetForNonDetailSort;
				}
				return false;
			}
		}

		protected virtual int[] SortFilterExpressionScopeInfoIndices
		{
			get
			{
				Global.Tracer.Assert(condition: false);
				return null;
			}
		}

		internal Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => m_objectType;

		public int Depth => m_depth;

		internal virtual IRIFReportScope RIFReportScope => null;

		bool IScope.TargetForNonDetailSort => TargetForNonDetailSort;

		int[] IScope.SortFilterExpressionScopeInfoIndices => SortFilterExpressionScopeInfoIndices;

		IRIFReportScope IScope.RIFReportScope => RIFReportScope;

		public virtual int Size => 8 + ItemSizes.SizeOf(m_selfReference);

		protected RuntimeDataRegionObj()
		{
		}

		protected RuntimeDataRegionObj(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, int depth)
		{
			m_odpContext = odpContext;
			m_objectType = objectType;
			m_depth = depth;
			m_odpContext.TablixProcessingScalabilityCache.AllocateAndPin(this, m_depth);
		}

		internal virtual bool IsTargetForSort(int index, bool detailSort)
		{
			if (OuterScope != null)
			{
				return OuterScope.Value().IsTargetForSort(index, detailSort);
			}
			return false;
		}

		internal abstract void NextRow();

		internal abstract bool SortAndFilter(AggregateUpdateContext aggContext);

		internal abstract void CalculateRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext);

		public abstract void SetupEnvironment();

		internal abstract void CalculatePreviousAggregates();

		public abstract void UpdateAggregates(AggregateUpdateContext context);

		bool IScope.IsTargetForSort(int index, bool detailSort)
		{
			return IsTargetForSort(index, detailSort);
		}

		void IScope.CalculatePreviousAggregates()
		{
			CalculatePreviousAggregates();
		}

		bool IScope.InScope(string scope)
		{
			return InScope(scope);
		}

		IReference<IScope> IScope.GetOuterScope(bool includeSubReportContainingScope)
		{
			return OuterScope;
		}

		string IScope.GetScopeName()
		{
			return ScopeName;
		}

		int IScope.RecursiveLevel(string scope)
		{
			return GetRecursiveLevel(scope);
		}

		bool IScope.TargetScopeMatched(int index, bool detailSort)
		{
			return TargetScopeMatched(index, detailSort);
		}

		void IScope.GetScopeValues(IReference<IHierarchyObj> targetScopeObj, List<object>[] scopeValues, ref int index)
		{
			GetScopeValues(targetScopeObj, scopeValues, ref index);
		}

		void IScope.GetGroupNameValuePairs(Dictionary<string, object> pairs)
		{
			GetGroupNameValuePairs(pairs);
		}

		internal static bool UpdateAggregatesAtScope(AggregateUpdateContext aggContext, IDataRowHolder scope, DataScopeInfo scopeInfo, AggregateUpdateFlags updateFlags, bool needsSetupEnvironment)
		{
			return aggContext.UpdateAggregates(scopeInfo, scope, updateFlags, needsSetupEnvironment);
		}

		internal static void AggregatesOfAggregatesEnd(IScope scopeObj, AggregateUpdateContext aggContext, AggregateUpdateQueue workQueue, DataScopeInfo dataScopeInfo, BucketedDataAggregateObjs aggregatesOfAggregates, bool updateAggsIfNeeded)
		{
			if (dataScopeInfo == null)
			{
				return;
			}
			if (updateAggsIfNeeded)
			{
				while (aggContext.AdvanceQueue(workQueue))
				{
					scopeObj.UpdateAggregates(aggContext);
				}
			}
			aggContext.RestoreOriginalState(workQueue);
			if (aggContext.Mode == AggregateMode.Aggregates && dataScopeInfo.NeedsSeparateAofAPass && updateAggsIfNeeded)
			{
				scopeObj.UpdateAggregates(aggContext);
			}
		}

		internal static AggregateUpdateQueue AggregateOfAggregatesStart(AggregateUpdateContext aggContext, IDataRowHolder scope, DataScopeInfo dataScopeInfo, BucketedDataAggregateObjs aggregatesOfAggregates, AggregateUpdateFlags updateFlags, bool needsSetupEnvironment)
		{
			if (dataScopeInfo == null)
			{
				return null;
			}
			AggregateUpdateQueue result = null;
			if (aggContext.Mode == AggregateMode.Aggregates)
			{
				if (dataScopeInfo.NeedsSeparateAofAPass)
				{
					result = aggContext.ReplaceAggregatesToUpdate(aggregatesOfAggregates);
				}
				else
				{
					result = aggContext.RegisterAggregatesToUpdate(aggregatesOfAggregates);
					if (updateFlags != 0)
					{
						UpdateAggregatesAtScope(aggContext, scope, dataScopeInfo, updateFlags, needsSetupEnvironment);
					}
				}
			}
			else if (aggContext.Mode == AggregateMode.PostSortAggregates)
			{
				result = aggContext.RegisterAggregatesToUpdate(aggregatesOfAggregates);
				result = aggContext.RegisterRunningValuesToUpdate(result, dataScopeInfo.RunningValuesOfAggregates);
				if (updateFlags != 0)
				{
					UpdateAggregatesAtScope(aggContext, scope, dataScopeInfo, updateFlags, needsSetupEnvironment);
				}
			}
			else
			{
				Global.Tracer.Assert(condition: false, "Unknown AggregateMode for AggregateOfAggregatesStart");
			}
			return result;
		}

		internal static void AddAggregate(ref List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> aggregates, Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregate)
		{
			if (aggregates == null)
			{
				aggregates = new List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj>();
			}
			aggregates.Add(aggregate);
		}

		internal static void CreateAggregates(OnDemandProcessingContext odpContext, List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> aggDefs, ref List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> nonCustomAggregates, ref List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> customAggregates)
		{
			if (aggDefs == null || 0 >= aggDefs.Count)
			{
				return;
			}
			for (int i = 0; i < aggDefs.Count; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregate = new Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj(aggDefs[i], odpContext);
				if (Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Aggregate == aggDefs[i].AggregateType)
				{
					AddAggregate(ref customAggregates, aggregate);
				}
				else
				{
					AddAggregate(ref nonCustomAggregates, aggregate);
				}
			}
		}

		internal static void CreateAggregates(OnDemandProcessingContext odpContext, List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> aggDefs, List<int> aggregateIndexes, ref List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> nonCustomAggregates, ref List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> customAggregates)
		{
			if (aggregateIndexes == null || 0 >= aggregateIndexes.Count)
			{
				return;
			}
			for (int i = 0; i < aggregateIndexes.Count; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo dataAggregateInfo = aggDefs[aggregateIndexes[i]];
				Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregate = new Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj(dataAggregateInfo, odpContext);
				if (Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Aggregate == dataAggregateInfo.AggregateType)
				{
					AddAggregate(ref customAggregates, aggregate);
				}
				else
				{
					AddAggregate(ref nonCustomAggregates, aggregate);
				}
			}
		}

		internal static void CreateAggregates<AggregateType>(OnDemandProcessingContext odpContext, List<AggregateType> aggDefs, ref List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> aggregates) where AggregateType : Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo
		{
			if (aggDefs != null && 0 < aggDefs.Count)
			{
				for (int i = 0; i < aggDefs.Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregate = new Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj(aggDefs[i], odpContext);
					AddAggregate(ref aggregates, aggregate);
				}
			}
		}

		internal static void CreateAggregates<AggregateType>(OnDemandProcessingContext odpContext, BucketedAggregatesCollection<AggregateType> aggDefs, ref BucketedDataAggregateObjs aggregates) where AggregateType : Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo
		{
			if (aggDefs == null || aggDefs.IsEmpty)
			{
				return;
			}
			if (aggregates == null)
			{
				aggregates = new BucketedDataAggregateObjs();
			}
			foreach (AggregateBucket<AggregateType> bucket in aggDefs.Buckets)
			{
				foreach (AggregateType aggregate in bucket.Aggregates)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj item = new Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj(aggregate, odpContext);
					aggregates.GetOrCreateBucket(bucket.Level).Aggregates.Add(item);
				}
			}
		}

		internal static void CreateAggregates<AggregateType>(OnDemandProcessingContext odpContext, List<AggregateType> aggDefs, List<int> aggregateIndexes, ref List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> aggregates) where AggregateType : Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo
		{
			if (aggregateIndexes != null && 0 < aggregateIndexes.Count)
			{
				for (int i = 0; i < aggregateIndexes.Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregate = new Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj(aggDefs[aggregateIndexes[i]], odpContext);
					AddAggregate(ref aggregates, aggregate);
				}
			}
		}

		internal static void UpdateAggregates(OnDemandProcessingContext odpContext, List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> aggregates, bool updateAndSetup)
		{
			if (aggregates == null)
			{
				return;
			}
			for (int i = 0; i < aggregates.Count; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj dataAggregateObj = aggregates[i];
				dataAggregateObj.Update();
				if (updateAndSetup)
				{
					odpContext.ReportObjectModel.AggregatesImpl.Set(dataAggregateObj.Name, dataAggregateObj.AggregateDef, dataAggregateObj.DuplicateNames, dataAggregateObj.AggregateResult());
				}
			}
		}

		protected void SetupAggregates(List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> aggregates)
		{
			if (aggregates != null)
			{
				for (int i = 0; i < aggregates.Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj dataAggregateObj = aggregates[i];
					m_odpContext.ReportObjectModel.AggregatesImpl.Set(dataAggregateObj.Name, dataAggregateObj.AggregateDef, dataAggregateObj.DuplicateNames, dataAggregateObj.AggregateResult());
				}
			}
		}

		protected void SetupAggregates(BucketedDataAggregateObjs aggregates)
		{
			SetupAggregates(m_odpContext, aggregates);
		}

		internal static void SetupAggregates(OnDemandProcessingContext odpContext, BucketedDataAggregateObjs aggregates)
		{
			if (aggregates == null)
			{
				return;
			}
			foreach (Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregate in aggregates)
			{
				odpContext.ReportObjectModel.AggregatesImpl.Set(aggregate.Name, aggregate.AggregateDef, aggregate.DuplicateNames, aggregate.AggregateResult());
			}
		}

		protected void SetupNewDataSet(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			m_odpContext.EnsureRuntimeEnvironmentForDataSet(dataSet, noRows: false);
		}

		protected void SetupEnvironment(List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> nonCustomAggregates, List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> customAggregates, DataFieldRow dataRow)
		{
			SetupAggregates(nonCustomAggregates);
			SetupAggregates(customAggregates);
			SetupFields(dataRow);
			m_odpContext.ReportRuntime.CurrentScope = this;
		}

		protected void SetupFields(DataFieldRow dataRow)
		{
			if (dataRow == null)
			{
				m_odpContext.ReportObjectModel.CreateNoRows();
			}
			else
			{
				dataRow.SetFields(m_odpContext.ReportObjectModel.FieldsImpl);
			}
		}

		protected void SetupRunningValues(List<Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo> rvDefs, Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] rvValues)
		{
			int startIndex = 0;
			SetupRunningValues(m_odpContext, ref startIndex, rvDefs, rvValues);
		}

		private static void SetupRunningValues(OnDemandProcessingContext odpContext, ref int startIndex, List<Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo> rvDefs, Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] rvValues)
		{
			if (rvDefs != null && rvValues != null)
			{
				AggregatesImpl aggregatesImpl = odpContext.ReportObjectModel.AggregatesImpl;
				for (int i = 0; i < rvDefs.Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValueInfo = rvDefs[i];
					aggregatesImpl.Set(runningValueInfo.Name, runningValueInfo, runningValueInfo.DuplicateNames, rvValues[startIndex + i]);
				}
				startIndex += rvDefs.Count;
			}
		}

		protected static IOnDemandMemberInstanceReference GetFirstMemberInstance(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode rifMember, IReference<RuntimeMemberObj>[] memberCol)
		{
			IOnDemandMemberInstanceReference result = null;
			RuntimeDataTablixGroupRootObjReference groupRoot = GetGroupRoot(rifMember, memberCol);
			using (groupRoot.PinValue())
			{
				RuntimeGroupLeafObjReference firstChild = groupRoot.Value().FirstChild;
				if (firstChild != null)
				{
					return (IOnDemandMemberInstanceReference)firstChild;
				}
				return result;
			}
		}

		protected static RuntimeDataTablixGroupRootObjReference GetGroupRoot(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode rifMember, IReference<RuntimeMemberObj>[] memberCol)
		{
			Global.Tracer.Assert(!rifMember.IsStatic, "Cannot GetGroupRoot of a static member");
			return memberCol[rifMember.IndexInCollection].Value().GroupRoot;
		}

		internal static long AssignScopeInstanceNumber(DataScopeInfo dataScopeInfo)
		{
			return dataScopeInfo?.AssignScopeInstanceNumber() ?? 0;
		}

		public abstract void ReadRow(DataActions dataAction, ITraversalContext context);

		internal abstract bool InScope(string scope);

		protected Hashtable GetScopeNames(RuntimeDataRegionObjReference currentScope, string targetScope, out bool inScope)
		{
			inScope = false;
			Hashtable hashtable = new Hashtable();
			IScope scope = null;
			for (IReference<IScope> reference = currentScope; reference != null; reference = scope.GetOuterScope(includeSubReportContainingScope: false))
			{
				scope = reference.Value();
				string scopeName = scope.GetScopeName();
				if (scopeName != null)
				{
					if (!inScope && scopeName.Equals(targetScope))
					{
						inScope = true;
					}
					Microsoft.ReportingServices.ReportIntermediateFormat.Grouping value = null;
					if (scope is RuntimeGroupLeafObj)
					{
						value = ((RuntimeGroupLeafObj)scope).GroupingDef;
					}
					hashtable.Add(scopeName, value);
				}
				else if (scope is RuntimeTablixCell && !inScope)
				{
					inScope = scope.InScope(targetScope);
				}
			}
			return hashtable;
		}

		protected Hashtable GetScopeNames(RuntimeDataRegionObjReference currentScope, string targetScope, out int level)
		{
			level = -1;
			Hashtable hashtable = new Hashtable();
			IScope scope = null;
			for (IReference<IScope> reference = currentScope; reference != null; reference = scope.GetOuterScope(includeSubReportContainingScope: false))
			{
				scope = reference.Value();
				string scopeName = scope.GetScopeName();
				if (scopeName != null)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = null;
					if (scope is RuntimeGroupLeafObj)
					{
						grouping = ((RuntimeGroupLeafObj)scope).GroupingDef;
						if (-1 == level && scopeName.Equals(targetScope))
						{
							level = grouping.RecursiveLevel;
						}
					}
					hashtable.Add(scopeName, grouping);
				}
				else if (scope is RuntimeTablixCell && -1 == level)
				{
					level = scope.RecursiveLevel(targetScope);
				}
			}
			return hashtable;
		}

		protected Hashtable GetScopeNames(RuntimeDataRegionObjReference currentScope, Dictionary<string, object> nameValuePairs)
		{
			Hashtable hashtable = new Hashtable();
			IScope scope = null;
			for (IReference<IScope> reference = currentScope; reference != null; reference = scope.GetOuterScope(includeSubReportContainingScope: false))
			{
				scope = reference.Value();
				string scopeName = scope.GetScopeName();
				if (scopeName != null)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = null;
					if (scope is RuntimeGroupLeafObj)
					{
						grouping = ((RuntimeGroupLeafObj)scope).GroupingDef;
						AddGroupNameValuePair(m_odpContext, grouping, nameValuePairs);
					}
					hashtable.Add(scopeName, grouping);
				}
				else if (scope is RuntimeTablixCell)
				{
					scope.GetGroupNameValuePairs(nameValuePairs);
				}
			}
			return hashtable;
		}

		internal static void AddGroupNameValuePair(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping, Dictionary<string, object> nameValuePairs)
		{
			if (grouping == null)
			{
				return;
			}
			Global.Tracer.Assert(grouping.GroupExpressions != null && 0 < grouping.GroupExpressions.Count);
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = grouping.GroupExpressions[0];
			if (expressionInfo.Type != Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Field)
			{
				return;
			}
			try
			{
				FieldImpl fieldImpl = odpContext.ReportObjectModel.FieldsImpl[expressionInfo.IntValue];
				if (fieldImpl.FieldDef != null)
				{
					object value = fieldImpl.Value;
					if (!nameValuePairs.ContainsKey(fieldImpl.FieldDef.DataField))
					{
						nameValuePairs.Add(fieldImpl.FieldDef.DataField, (value is DBNull) ? null : value);
					}
				}
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
			}
		}

		protected bool DataRegionInScope(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, string scope)
		{
			if (dataRegionDef.ScopeNames == null)
			{
				dataRegionDef.ScopeNames = GetScopeNames(SelfReference, scope, out bool inScope);
				return inScope;
			}
			return dataRegionDef.ScopeNames.Contains(scope);
		}

		protected virtual int GetRecursiveLevel(string scope)
		{
			return -1;
		}

		protected int DataRegionRecursiveLevel(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, string scope)
		{
			if (scope == null)
			{
				return -1;
			}
			if (dataRegionDef.ScopeNames == null)
			{
				dataRegionDef.ScopeNames = GetScopeNames(SelfReference, scope, out int level);
				return level;
			}
			return (dataRegionDef.ScopeNames[scope] as Microsoft.ReportingServices.ReportIntermediateFormat.Grouping)?.RecursiveLevel ?? (-1);
		}

		protected void DataRegionGetGroupNameValuePairs(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, Dictionary<string, object> nameValuePairs)
		{
			if (dataRegionDef.ScopeNames == null)
			{
				dataRegionDef.ScopeNames = GetScopeNames(SelfReference, nameValuePairs);
				return;
			}
			IEnumerator enumerator = dataRegionDef.ScopeNames.Values.GetEnumerator();
			while (enumerator.MoveNext())
			{
				AddGroupNameValuePair(m_odpContext, enumerator.Current as Microsoft.ReportingServices.ReportIntermediateFormat.Grouping, nameValuePairs);
			}
		}

		protected void ScopeNextNonAggregateRow(List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> aggregates, ScalableList<DataFieldRow> dataRows)
		{
			UpdateAggregates(m_odpContext, aggregates, updateAndSetup: true);
			CommonNextRow(dataRows);
		}

		internal static void CommonFirstRow(OnDemandProcessingContext odpContext, ref bool firstRowIsAggregate, ref DataFieldRow firstRow)
		{
			if (firstRowIsAggregate || firstRow == null)
			{
				firstRow = new DataFieldRow(odpContext.ReportObjectModel.FieldsImpl, getAndSave: true);
				firstRowIsAggregate = odpContext.ReportObjectModel.FieldsImpl.IsAggregateRow;
			}
		}

		protected void CommonNextRow(ScalableList<DataFieldRow> dataRows)
		{
			if (dataRows != null)
			{
				RuntimeDataTablixObj.SaveData(dataRows, m_odpContext);
			}
			SendToInner();
		}

		protected virtual void SendToInner()
		{
			Global.Tracer.Assert(condition: false);
		}

		protected void ScopeNextAggregateRow(RuntimeUserSortTargetInfo sortTargetInfo)
		{
			if (sortTargetInfo != null)
			{
				if (sortTargetInfo.AggregateRows == null)
				{
					sortTargetInfo.AggregateRows = new List<AggregateRow>();
				}
				AggregateRow item = new AggregateRow(m_odpContext.ReportObjectModel.FieldsImpl, getAndSave: true);
				sortTargetInfo.AggregateRows.Add(item);
				if (!sortTargetInfo.TargetForNonDetailSort)
				{
					return;
				}
			}
			SendToInner();
		}

		protected void ScopeFinishSorting(ref DataFieldRow firstRow, RuntimeUserSortTargetInfo sortTargetInfo)
		{
			Global.Tracer.Assert(sortTargetInfo != null, "(null != sortTargetInfo)");
			firstRow = null;
			sortTargetInfo.SortTree.Traverse(ProcessingStages.UserSortFilter, ascending: true, null);
			sortTargetInfo.SortTree.Dispose();
			sortTargetInfo.SortTree = null;
			if (sortTargetInfo.AggregateRows != null)
			{
				for (int i = 0; i < sortTargetInfo.AggregateRows.Count; i++)
				{
					sortTargetInfo.AggregateRows[i].SetFields(m_odpContext.ReportObjectModel.FieldsImpl);
					SendToInner();
				}
				sortTargetInfo.AggregateRows = null;
			}
		}

		internal virtual bool TargetScopeMatched(int index, bool detailSort)
		{
			Global.Tracer.Assert(condition: false);
			return false;
		}

		internal virtual void GetScopeValues(IReference<IHierarchyObj> targetScopeObj, List<object>[] scopeValues, ref int index)
		{
			Global.Tracer.Assert(condition: false);
		}

		protected void ReleaseDataRows(DataActions finishedDataAction, ref DataActions dataAction, ref ScalableList<DataFieldRow> dataRows)
		{
			dataAction &= ~finishedDataAction;
			if (dataAction == DataActions.None)
			{
				dataRows.Clear();
				dataRows = null;
			}
		}

		protected void DetailHandleSortFilterEvent(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, IReference<IScope> outerScope, bool isColumnAxis, int rowIndex)
		{
			using (outerScope.PinValue())
			{
				IScope scope = outerScope.Value();
				List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo = m_odpContext.RuntimeSortFilterInfo;
				if (runtimeSortFilterInfo == null || dataRegionDef.SortFilterSourceDetailScopeInfo == null || scope.TargetForNonDetailSort)
				{
					return;
				}
				int count = runtimeSortFilterInfo.Count;
				for (int i = 0; i < count; i++)
				{
					IReference<RuntimeSortFilterEventInfo> reference = runtimeSortFilterInfo[i];
					using (reference.PinValue())
					{
						RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
						if (runtimeSortFilterEventInfo.EventSource.ContainingScopes == null || 0 >= runtimeSortFilterEventInfo.EventSource.ContainingScopes.Count || -1 == dataRegionDef.SortFilterSourceDetailScopeInfo[i] || !scope.TargetScopeMatched(i, detailSort: false) || m_odpContext.ReportObjectModel.FieldsImpl.GetRowIndex() != dataRegionDef.SortFilterSourceDetailScopeInfo[i])
						{
							continue;
						}
						if (runtimeSortFilterEventInfo.EventSource.ContainingScopes.LastEntry == null)
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem parent = runtimeSortFilterEventInfo.EventSource.Parent;
							if (runtimeSortFilterEventInfo.EventSource.IsSubReportTopLevelScope)
							{
								while (parent != null && !(parent is Microsoft.ReportingServices.ReportIntermediateFormat.SubReport))
								{
									parent = parent.Parent;
								}
								Global.Tracer.Assert(parent is Microsoft.ReportingServices.ReportIntermediateFormat.SubReport, "(parent is SubReport)");
								parent = parent.Parent;
							}
							if (parent == dataRegionDef)
							{
								runtimeSortFilterEventInfo.SetEventSourceScope(isColumnAxis, SelfReference, rowIndex);
							}
						}
						runtimeSortFilterEventInfo.AddDetailScopeInfo(isColumnAxis, SelfReference, rowIndex);
					}
				}
			}
		}

		protected void DetailGetScopeValues(IReference<IScope> outerScope, IReference<IHierarchyObj> targetScopeObj, List<object>[] scopeValues, ref int index)
		{
			Global.Tracer.Assert(targetScopeObj == null, "(null == targetScopeObj)");
			outerScope.Value().GetScopeValues(targetScopeObj, scopeValues, ref index);
			Global.Tracer.Assert(index < scopeValues.Length, "(index < scopeValues.Length)");
			List<object> list = new List<object>(1);
			list.Add(m_odpContext.ReportObjectModel.FieldsImpl.GetRowIndex());
			scopeValues[index++] = list;
		}

		protected bool DetailTargetScopeMatched(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, IReference<IScope> outerScope, bool isColumnAxis, int index)
		{
			if (m_odpContext.RuntimeSortFilterInfo != null)
			{
				IReference<RuntimeSortFilterEventInfo> reference = m_odpContext.RuntimeSortFilterInfo[index];
				using (reference.PinValue())
				{
					RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
					if (runtimeSortFilterEventInfo != null)
					{
						List<IReference<RuntimeDataRegionObj>> list = null;
						List<int> list2 = null;
						int num = -1;
						if (isColumnAxis)
						{
							list = runtimeSortFilterEventInfo.DetailColScopes;
							list2 = runtimeSortFilterEventInfo.DetailColScopeIndices;
							num = dataRegionDef.CurrentColDetailIndex;
						}
						else
						{
							list = runtimeSortFilterEventInfo.DetailRowScopes;
							list2 = runtimeSortFilterEventInfo.DetailRowScopeIndices;
							num = dataRegionDef.CurrentRowDetailIndex;
						}
						if (list != null)
						{
							for (int i = 0; i < list.Count; i++)
							{
								if (SelfReference.Equals(list[i]) && num == list2[i])
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		protected virtual void GetGroupNameValuePairs(Dictionary<string, object> pairs)
		{
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.OdpContext:
				{
					int value = scalabilityCache.StoreStaticReference(m_odpContext);
					writer.Write(value);
					break;
				}
				case MemberName.ObjectType:
					writer.WriteEnum((int)m_objectType);
					break;
				case MemberName.Depth:
					writer.Write(m_depth);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.OdpContext:
				{
					int id = reader.ReadInt32();
					m_odpContext = (OnDemandProcessingContext)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.ObjectType:
					m_objectType = (Microsoft.ReportingServices.ReportProcessing.ObjectType)reader.ReadEnum();
					break;
				case MemberName.Depth:
					m_depth = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public virtual Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObj;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.OdpContext, Token.Int32));
				list.Add(new MemberInfo(MemberName.ObjectType, Token.Enum));
				list.Add(new MemberInfo(MemberName.Depth, Token.Int32));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}

		public virtual void SetReference(IReference selfRef)
		{
			m_selfReference = (RuntimeDataRegionObjReference)selfRef;
		}
	}
}

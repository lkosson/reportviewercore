using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal class DataScopeInfo : IPersistable
	{
		private bool m_aggregatesSpanGroupFilter;

		private bool m_hasAggregatesToUpdateAtRowScope;

		private BucketedDataAggregateInfos m_aggregatesOfAggregates;

		private BucketedDataAggregateInfos m_postSortAggregatesOfAggregates;

		private List<RunningValueInfo> m_runningValuesOfAggregates;

		private int m_scopeID;

		private int m_dataPipelineID = -1;

		[Reference]
		private DataSet m_dataSet;

		private bool m_isDecomposable;

		private JoinInfo m_joinInfo;

		private List<int> m_groupingFieldIndicesForServerAggregates;

		[NonSerialized]
		private string m_dataSetName;

		[NonSerialized]
		private long m_lastScopeInstanceNumber;

		[NonSerialized]
		internal const int DataPipelineIDUnassigned = -1;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal bool AggregatesSpanGroupFilter
		{
			get
			{
				return m_aggregatesSpanGroupFilter;
			}
			set
			{
				if (!m_aggregatesSpanGroupFilter)
				{
					m_aggregatesSpanGroupFilter = value;
				}
			}
		}

		internal bool HasAggregatesToUpdateAtRowScope
		{
			get
			{
				return m_hasAggregatesToUpdateAtRowScope;
			}
			set
			{
				if (!m_hasAggregatesToUpdateAtRowScope)
				{
					m_hasAggregatesToUpdateAtRowScope = value;
				}
			}
		}

		internal bool NeedsSeparateAofAPass => AggregatesSpanGroupFilter;

		internal BucketedDataAggregateInfos AggregatesOfAggregates
		{
			get
			{
				return m_aggregatesOfAggregates;
			}
			set
			{
				m_aggregatesOfAggregates = value;
			}
		}

		internal BucketedDataAggregateInfos PostSortAggregatesOfAggregates
		{
			get
			{
				return m_postSortAggregatesOfAggregates;
			}
			set
			{
				m_postSortAggregatesOfAggregates = value;
			}
		}

		internal List<RunningValueInfo> RunningValuesOfAggregates
		{
			get
			{
				return m_runningValuesOfAggregates;
			}
			set
			{
				m_runningValuesOfAggregates = value;
			}
		}

		internal int ScopeID
		{
			get
			{
				return m_scopeID;
			}
			set
			{
				m_scopeID = value;
			}
		}

		internal DataSet DataSet => m_dataSet;

		internal int DataPipelineID
		{
			get
			{
				return m_dataPipelineID;
			}
			set
			{
				m_dataPipelineID = value;
			}
		}

		internal bool HasAggregatesOrRunningValues
		{
			get
			{
				if (!HasAggregates(m_aggregatesOfAggregates) && !HasAggregates(m_postSortAggregatesOfAggregates))
				{
					return HasRunningValues;
				}
				return true;
			}
		}

		internal bool HasRunningValues => m_runningValuesOfAggregates.Count > 0;

		internal bool IsDecomposable
		{
			get
			{
				return m_isDecomposable;
			}
			set
			{
				m_isDecomposable = value;
			}
		}

		internal bool NeedsIDC => m_joinInfo != null;

		internal JoinInfo JoinInfo => m_joinInfo;

		internal List<int> GroupingFieldIndicesForServerAggregates => m_groupingFieldIndicesForServerAggregates;

		public DataScopeInfo()
		{
		}

		public DataScopeInfo(int scopeId)
		{
			m_scopeID = scopeId;
			m_runningValuesOfAggregates = new List<RunningValueInfo>();
			m_aggregatesOfAggregates = new BucketedDataAggregateInfos();
			m_postSortAggregatesOfAggregates = new BucketedDataAggregateInfos();
		}

		internal void ApplyGroupingFieldsForServerAggregates(FieldsImpl fields)
		{
			if (m_groupingFieldIndicesForServerAggregates != null)
			{
				for (int i = 0; i < m_groupingFieldIndicesForServerAggregates.Count; i++)
				{
					fields.ConsumeAggregationField(m_groupingFieldIndicesForServerAggregates[i]);
				}
			}
		}

		internal void SetRelationship(string dataSetName, IdcRelationship relationship)
		{
			m_dataSetName = dataSetName;
			if (relationship != null)
			{
				m_joinInfo = new LinearJoinInfo(relationship);
			}
		}

		internal void SetRelationship(string dataSetName, List<IdcRelationship> relationships)
		{
			m_dataSetName = dataSetName;
			if (relationships != null)
			{
				m_joinInfo = new IntersectJoinInfo(relationships);
			}
		}

		internal void ValidateScopeRulesForIdc(InitializationContext context, IRIFDataScope dataScope)
		{
			if (m_dataSet != null && NeedsIDC)
			{
				m_joinInfo.ValidateScopeRulesForIdcNaturalJoin(context, dataScope);
				context.EnsureDataSetUsedOnceForIdcUnderTopDataRegion(m_dataSet, dataScope);
			}
		}

		internal void ValidateDataSetBindingAndRelationships(ScopeTree scopeTree, IRIFReportDataScope scope, ErrorContext errorContext)
		{
			if (m_dataSet != null)
			{
				return;
			}
			ParentDataSetContainer parentDataSetContainer = DetermineParentDataSets(scopeTree, scope);
			if (m_dataSetName == null)
			{
				BindToParentDataSet(scopeTree, scope, errorContext, parentDataSetContainer);
			}
			else
			{
				BindToNamedDataSet(scopeTree, scope, errorContext, parentDataSetContainer);
			}
			if (m_dataSet == null)
			{
				return;
			}
			if (scopeTree.GetParentDataRegion(scope) == null && m_joinInfo != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidRelationshipTopLevelDataRegion, Severity.Error, scope.DataScopeObjectType, scope.Name, "Relationship");
				m_joinInfo = null;
			}
			if (parentDataSetContainer != null && m_joinInfo == null)
			{
				if (parentDataSetContainer.Count == 1)
				{
					m_joinInfo = new LinearJoinInfo();
				}
				else
				{
					m_joinInfo = new IntersectJoinInfo();
				}
			}
			if (m_joinInfo != null)
			{
				if (!m_joinInfo.ValidateRelationships(scopeTree, errorContext, m_dataSet, parentDataSetContainer, scope))
				{
					m_joinInfo = null;
				}
				if (m_joinInfo == null && m_dataSetName != null && m_dataSet != null && !DataSet.AreEqualById(parentDataSetContainer.ParentDataSet, m_dataSet))
				{
					UpdateDataSet(parentDataSetContainer.ParentDataSet, scope);
				}
			}
		}

		private void BindToNamedDataSet(ScopeTree scopeTree, IRIFReportDataScope scope, ErrorContext errorContext, ParentDataSetContainer parentDataSets)
		{
			DataSet dataSet = scopeTree.GetDataSet(m_dataSetName);
			if (dataSet == null)
			{
				DataRegion parentDataRegion = scopeTree.GetParentDataRegion(scope);
				if (parentDataSets != null && parentDataSets.Count == 1 && scope is DataRegion && parentDataRegion != null)
				{
					UpdateDataSet(parentDataSets.ParentDataSet, scope);
					m_dataSetName = parentDataSets.ParentDataSet.Name;
					return;
				}
				errorContext.Register(ProcessingErrorCode.rsInvalidDataSetName, Severity.Error, scope.DataScopeObjectType, scope.Name, "DataSetName", m_dataSetName.MarkAsPrivate());
				if (parentDataSets != null)
				{
					UpdateDataSet(parentDataSets.ParentDataSet, scope);
				}
			}
			else
			{
				UpdateDataSet(dataSet, scope);
			}
		}

		private void BindToParentDataSet(ScopeTree scopeTree, IRIFReportDataScope scope, ErrorContext errorContext, ParentDataSetContainer parentDataSets)
		{
			if (parentDataSets == null)
			{
				if (scopeTree.GetParentDataRegion(scope) == null)
				{
					if (scopeTree.Report.MappingDataSetIndexToDataSet.Count == 0)
					{
						errorContext.Register(ProcessingErrorCode.rsDataRegionWithoutDataSet, Severity.Error, scope.DataScopeObjectType, scope.Name, null);
					}
					else
					{
						errorContext.Register(ProcessingErrorCode.rsMissingDataSetName, Severity.Error, scope.DataScopeObjectType, scope.Name, "DataSetName");
					}
				}
			}
			else if (parentDataSets.Count > 1 && !parentDataSets.AreAllSameDataSet())
			{
				DataRegion parentDataRegion = scopeTree.GetParentDataRegion(scope);
				IRIFDataScope parentRowScopeForIntersection = scopeTree.GetParentRowScopeForIntersection(scope);
				IRIFDataScope parentColumnScopeForIntersection = scopeTree.GetParentColumnScopeForIntersection(scope);
				errorContext.Register(ProcessingErrorCode.rsMissingIntersectionDataSetName, Severity.Error, scope.DataScopeObjectType, parentDataRegion.Name, "DataSetName", parentDataRegion.ObjectType.ToString(), parentRowScopeForIntersection.Name, parentColumnScopeForIntersection.Name);
			}
			else
			{
				UpdateDataSet(parentDataSets.ParentDataSet, scope);
			}
		}

		private static ParentDataSetContainer DetermineParentDataSets(ScopeTree scopeTree, IRIFReportDataScope scope)
		{
			if (scopeTree.IsIntersectionScope(scope))
			{
				IRIFDataScope parentRowScopeForIntersection = scopeTree.GetParentRowScopeForIntersection(scope);
				return new ParentDataSetContainer(columnParentDataSet: scopeTree.GetParentColumnScopeForIntersection(scope).DataScopeInfo.DataSet, rowParentDataSet: parentRowScopeForIntersection.DataScopeInfo.DataSet);
			}
			IRIFDataScope parentScope = scopeTree.GetParentScope(scope);
			DataSet dataSet2 = (parentScope != null) ? parentScope.DataScopeInfo.DataSet : scopeTree.GetDefaultTopLevelDataSet();
			if (dataSet2 == null)
			{
				return null;
			}
			return new ParentDataSetContainer(dataSet2);
		}

		private void UpdateDataSet(DataSet targetDataSet, IRIFDataScope scope)
		{
			m_dataSet = targetDataSet;
			DataRegion dataRegion = scope as DataRegion;
			if (dataRegion != null && m_dataSet != null)
			{
				dataRegion.DataSetName = m_dataSet.Name;
			}
		}

		internal void ClearAggregatesIfEmpty()
		{
			Global.Tracer.Assert(m_aggregatesOfAggregates != null, "(null != m_aggregatesOfAggregates)");
			if (m_aggregatesOfAggregates.IsEmpty)
			{
				m_aggregatesOfAggregates = null;
			}
			Global.Tracer.Assert(m_postSortAggregatesOfAggregates != null, "(null != m_postSortAggregatesOfAggregates)");
			if (m_postSortAggregatesOfAggregates.IsEmpty)
			{
				m_postSortAggregatesOfAggregates = null;
			}
		}

		internal void ClearRunningValuesIfEmpty()
		{
			Global.Tracer.Assert(m_runningValuesOfAggregates != null, "(null != m_runningValuesOfAggregates)");
			if (m_runningValuesOfAggregates.Count == 0)
			{
				m_runningValuesOfAggregates.Clear();
			}
		}

		public void MergeFrom(DataScopeInfo otherScope)
		{
			m_aggregatesSpanGroupFilter |= otherScope.m_aggregatesSpanGroupFilter;
			m_hasAggregatesToUpdateAtRowScope |= otherScope.m_hasAggregatesToUpdateAtRowScope;
			m_runningValuesOfAggregates.AddRange(otherScope.m_runningValuesOfAggregates);
			m_aggregatesOfAggregates.MergeFrom(otherScope.m_aggregatesOfAggregates);
			m_postSortAggregatesOfAggregates.MergeFrom(otherScope.m_postSortAggregatesOfAggregates);
		}

		internal DataScopeInfo PublishClone(AutomaticSubtotalContext context, int scopeID)
		{
			DataScopeInfo dataScopeInfo = new DataScopeInfo(scopeID);
			dataScopeInfo.m_dataSetName = m_dataSetName;
			if (m_joinInfo != null)
			{
				dataScopeInfo.m_joinInfo = m_joinInfo.PublishClone(context);
			}
			return dataScopeInfo;
		}

		internal void Initialize(InitializationContext context, IRIFDataScope scope)
		{
			if (m_joinInfo != null)
			{
				m_joinInfo.Initialize(context);
				InjectAggregateIndicatorFieldJoinConditions(context, scope);
			}
			CaptureGroupingFieldsForServerAggregates(context, scope);
		}

		private void CaptureGroupingFieldsForServerAggregates(InitializationContext context, IRIFDataScope scope)
		{
			if (m_groupingFieldIndicesForServerAggregates != null)
			{
				return;
			}
			m_groupingFieldIndicesForServerAggregates = new List<int>();
			if (m_dataSet == null)
			{
				return;
			}
			ScopeTree scopeTree = context.ScopeTree;
			if (scopeTree.IsIntersectionScope(scope))
			{
				AddGroupingFieldIndicesFromParentScope(context, scopeTree.GetParentRowScopeForIntersection(scope));
				AddGroupingFieldIndicesFromParentScope(context, scopeTree.GetParentColumnScopeForIntersection(scope));
				return;
			}
			IRIFDataScope parentScope = scopeTree.GetParentScope(scope);
			if (parentScope != null)
			{
				AddGroupingFieldIndicesFromParentScope(context, parentScope);
			}
			ReportHierarchyNode reportHierarchyNode = scope as ReportHierarchyNode;
			if (reportHierarchyNode == null || reportHierarchyNode.IsStatic || reportHierarchyNode.Grouping.IsDetail)
			{
				return;
			}
			foreach (ExpressionInfo groupExpression in reportHierarchyNode.Grouping.GroupExpressions)
			{
				if (groupExpression.Type == ExpressionInfo.Types.Field)
				{
					m_groupingFieldIndicesForServerAggregates.Add(groupExpression.FieldIndex);
				}
			}
		}

		private void AddGroupingFieldIndicesFromParentScope(InitializationContext context, IRIFDataScope parentScope)
		{
			Global.Tracer.Assert(parentScope.DataScopeInfo.GroupingFieldIndicesForServerAggregates != null, "Grouping fields for parent should have been captured first.");
			if (DataSet.AreEqualById(parentScope.DataScopeInfo.DataSet, m_dataSet))
			{
				m_groupingFieldIndicesForServerAggregates.AddRange(parentScope.DataScopeInfo.GroupingFieldIndicesForServerAggregates);
			}
			else if (m_joinInfo == null)
			{
				Global.Tracer.Assert(context.ErrorContext.HasError, "Missing expected error.");
			}
			else
			{
				m_joinInfo.AddMappedFieldIndices(parentScope.DataScopeInfo.GroupingFieldIndicesForServerAggregates, parentScope.DataScopeInfo.DataSet, m_dataSet, m_groupingFieldIndicesForServerAggregates);
			}
		}

		private void InjectAggregateIndicatorFieldJoinConditions(InitializationContext context, IRIFDataScope scope)
		{
			if (m_dataSet == null || m_joinInfo.Relationships == null)
			{
				return;
			}
			_ = context.PublishingContext.PublishingContextKind;
			if (context.PublishingContext.PublishingContextKind == PublishingContextKind.DataShape || scope.DataScopeObjectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.Grouping)
			{
				return;
			}
			int count = m_dataSet.Fields.Count;
			foreach (IdcRelationship relationship in m_joinInfo.Relationships)
			{
				if (relationship.IsCrossJoin)
				{
					continue;
				}
				for (int i = 0; i < count; i++)
				{
					Field field = m_dataSet.Fields[i];
					if (field.AggregateIndicatorFieldIndex >= 0)
					{
						Field field2 = m_dataSet.Fields[field.AggregateIndicatorFieldIndex];
						if (!field2.IsCalculatedField)
						{
							relationship.InsertAggregateIndicatorJoinCondition(field, i, field2, field.AggregateIndicatorFieldIndex, context);
						}
					}
				}
			}
		}

		internal long AssignScopeInstanceNumber()
		{
			m_lastScopeInstanceNumber++;
			return m_lastScopeInstanceNumber;
		}

		internal bool IsLastScopeInstanceNumber(long scopeInstanceNumber)
		{
			return m_lastScopeInstanceNumber == scopeInstanceNumber;
		}

		internal void ResetAggregates(AggregatesImpl reportOmAggregates)
		{
			reportOmAggregates.ResetAll(m_aggregatesOfAggregates);
			reportOmAggregates.ResetAll(m_postSortAggregatesOfAggregates);
			reportOmAggregates.ResetAll(m_runningValuesOfAggregates);
		}

		internal bool IsSameScope(DataScopeInfo candidateScopeInfo)
		{
			return m_scopeID == candidateScopeInfo.ScopeID;
		}

		internal static bool IsSameOrChildScope(IRIFReportDataScope childCandidate, IRIFReportDataScope parentCandidate)
		{
			while (childCandidate != null)
			{
				if (childCandidate.DataScopeInfo.ScopeID == parentCandidate.DataScopeInfo.ScopeID)
				{
					return true;
				}
				if (childCandidate.IsDataIntersectionScope)
				{
					IRIFReportIntersectionScope iRIFReportIntersectionScope = (IRIFReportIntersectionScope)childCandidate;
					if (!IsSameOrChildScope(iRIFReportIntersectionScope.ParentRowReportScope, parentCandidate))
					{
						return IsSameOrChildScope(iRIFReportIntersectionScope.ParentColumnReportScope, parentCandidate);
					}
					return true;
				}
				childCandidate = childCandidate.ParentReportScope;
			}
			return false;
		}

		internal static bool IsChildScopeOf(IRIFReportDataScope childCandidate, IRIFReportDataScope parentCandidate)
		{
			if (!childCandidate.DataScopeInfo.IsSameScope(parentCandidate.DataScopeInfo))
			{
				return IsSameOrChildScope(childCandidate, parentCandidate);
			}
			return false;
		}

		internal static bool HasDecomposableAncestorWithNonLatestInstanceBinding(IRIFReportDataScope candidate)
		{
			while (candidate != null)
			{
				if (candidate.IsScope && candidate.DataScopeInfo.IsDecomposable && candidate.IsBoundToStreamingScopeInstance && !candidate.CurrentStreamingScopeInstance.Value().IsMostRecentlyCreatedScopeInstance)
				{
					return true;
				}
				if (candidate.IsDataIntersectionScope)
				{
					IRIFReportIntersectionScope iRIFReportIntersectionScope = (IRIFReportIntersectionScope)candidate;
					if (!HasDecomposableAncestorWithNonLatestInstanceBinding(iRIFReportIntersectionScope.ParentRowReportScope))
					{
						return HasDecomposableAncestorWithNonLatestInstanceBinding(iRIFReportIntersectionScope.ParentColumnReportScope);
					}
					return true;
				}
				candidate = candidate.ParentReportScope;
			}
			return false;
		}

		internal static bool TryGetInnermostParentScopeRelatedToTargetDataSet(DataSet targetDataSet, IRIFReportDataScope candidate, out IRIFReportDataScope targetScope)
		{
			while (candidate != null)
			{
				if (targetDataSet.HasDefaultRelationship(candidate.DataScopeInfo.DataSet))
				{
					targetScope = candidate;
					return true;
				}
				if (candidate.IsDataIntersectionScope)
				{
					IRIFReportIntersectionScope iRIFReportIntersectionScope = (IRIFReportIntersectionScope)candidate;
					if (!TryGetInnermostParentScopeRelatedToTargetDataSet(targetDataSet, iRIFReportIntersectionScope.ParentRowReportScope, out targetScope))
					{
						return TryGetInnermostParentScopeRelatedToTargetDataSet(targetDataSet, iRIFReportIntersectionScope.ParentColumnReportScope, out targetScope);
					}
					return true;
				}
				candidate = candidate.ParentReportScope;
			}
			targetScope = null;
			return false;
		}

		internal static bool HasAggregates<T>(List<T> aggs) where T : DataAggregateInfo
		{
			if (aggs != null)
			{
				return aggs.Count > 0;
			}
			return false;
		}

		internal static bool HasNonServerAggregates<T>(List<T> aggs) where T : DataAggregateInfo
		{
			return aggs?.Any((T agg) => agg.AggregateType != DataAggregateInfo.AggregateTypes.Aggregate) ?? false;
		}

		internal static bool HasAggregates<T>(BucketedAggregatesCollection<T> aggs) where T : IPersistable
		{
			if (aggs != null)
			{
				return !aggs.IsEmpty;
			}
			return false;
		}

		internal static bool ContainsServerAggregate<T>(List<T> aggs, string aggregateName) where T : DataAggregateInfo
		{
			return aggs?.Any((T agg) => IsTargetServerAggregate(agg, aggregateName)) ?? false;
		}

		internal static bool IsTargetServerAggregate(DataAggregateInfo agg, string aggregateName)
		{
			if (agg.AggregateType == DataAggregateInfo.AggregateTypes.Aggregate)
			{
				if (!string.Equals(agg.Name, aggregateName, StringComparison.Ordinal))
				{
					if (agg.DuplicateNames != null)
					{
						return ListUtils.ContainsWithOrdinalComparer(aggregateName, agg.DuplicateNames);
					}
					return false;
				}
				return true;
			}
			return false;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.AggregatesSpanGroupFilter, Token.Boolean));
			list.Add(new MemberInfo(MemberName.AggregatesOfAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BucketedDataAggregateInfos));
			list.Add(new MemberInfo(MemberName.PostSortAggregatesOfAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BucketedDataAggregateInfos));
			list.Add(new MemberInfo(MemberName.RunningValuesOfAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RunningValueInfo));
			list.Add(new MemberInfo(MemberName.ScopeID, Token.Int32));
			list.Add(new MemberInfo(MemberName.HasAggregatesToUpdateAtRowScope, Token.Boolean));
			list.Add(new MemberInfo(MemberName.IsDecomposable, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DataSet, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSet, Token.Reference));
			list.Add(new MemberInfo(MemberName.JoinInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.JoinInfo));
			list.Add(new MemberInfo(MemberName.DataPipelineID, Token.Int32));
			list.Add(new MemberInfo(MemberName.GroupingFieldIndicesForServerAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataScopeInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.AggregatesSpanGroupFilter:
					writer.Write(m_aggregatesSpanGroupFilter);
					break;
				case MemberName.AggregatesOfAggregates:
					writer.Write(m_aggregatesOfAggregates);
					break;
				case MemberName.PostSortAggregatesOfAggregates:
					writer.Write(m_postSortAggregatesOfAggregates);
					break;
				case MemberName.RunningValuesOfAggregates:
					writer.Write(m_runningValuesOfAggregates);
					break;
				case MemberName.ScopeID:
					writer.Write(m_scopeID);
					break;
				case MemberName.HasAggregatesToUpdateAtRowScope:
					writer.Write(m_hasAggregatesToUpdateAtRowScope);
					break;
				case MemberName.IsDecomposable:
					writer.Write(m_isDecomposable);
					break;
				case MemberName.DataSet:
					writer.WriteReference(m_dataSet);
					break;
				case MemberName.JoinInfo:
					writer.Write(m_joinInfo);
					break;
				case MemberName.DataPipelineID:
					writer.Write(m_dataPipelineID);
					break;
				case MemberName.GroupingFieldIndicesForServerAggregates:
					writer.WriteListOfPrimitives(m_groupingFieldIndicesForServerAggregates);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.AggregatesSpanGroupFilter:
					m_aggregatesSpanGroupFilter = reader.ReadBoolean();
					break;
				case MemberName.AggregatesOfAggregates:
					m_aggregatesOfAggregates = (BucketedDataAggregateInfos)reader.ReadRIFObject();
					break;
				case MemberName.PostSortAggregatesOfAggregates:
					m_postSortAggregatesOfAggregates = (BucketedDataAggregateInfos)reader.ReadRIFObject();
					break;
				case MemberName.RunningValuesOfAggregates:
					m_runningValuesOfAggregates = reader.ReadGenericListOfRIFObjects<RunningValueInfo>();
					break;
				case MemberName.ScopeID:
					m_scopeID = reader.ReadInt32();
					break;
				case MemberName.HasAggregatesToUpdateAtRowScope:
					m_hasAggregatesToUpdateAtRowScope = reader.ReadBoolean();
					break;
				case MemberName.IsDecomposable:
					m_isDecomposable = reader.ReadBoolean();
					break;
				case MemberName.DataSet:
					m_dataSet = reader.ReadReference<DataSet>(this);
					break;
				case MemberName.JoinInfo:
					m_joinInfo = (JoinInfo)reader.ReadRIFObject();
					break;
				case MemberName.DataPipelineID:
					m_dataPipelineID = reader.ReadInt32();
					break;
				case MemberName.GroupingFieldIndicesForServerAggregates:
					m_groupingFieldIndicesForServerAggregates = reader.ReadListOfPrimitives<int>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.DataSet)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					Global.Tracer.Assert(referenceableItems[item.RefID] is DataSet);
					Global.Tracer.Assert(m_dataSet != (DataSet)referenceableItems[item.RefID]);
					m_dataSet = (DataSet)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataScopeInfo;
		}
	}
}

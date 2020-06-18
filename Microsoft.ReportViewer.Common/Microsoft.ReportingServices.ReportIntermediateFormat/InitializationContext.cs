using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal struct InitializationContext
	{
		private enum GroupingType
		{
			Normal,
			TablixRow,
			TablixColumn
		}

		private sealed class VisibilityContainmentInfo
		{
			internal IVisibilityOwner ContainingVisibility;

			internal IVisibilityOwner ContainingRowVisibility;

			internal IVisibilityOwner ContainingColumnVisibility;

			internal VisibilityContainmentInfo()
			{
			}
		}

		private sealed class ScopeInfo
		{
			private bool m_isTopLevelScope;

			private bool m_allowCustomAggregates;

			private List<DataAggregateInfo> m_aggregates;

			private List<DataAggregateInfo> m_postSortAggregates;

			private List<DataAggregateInfo> m_recursiveAggregates;

			private bool m_duplicateScope;

			private Grouping m_groupingScope;

			private DataRegion m_dataRegionScope;

			private DataSet m_dataSetScope;

			private IRIFReportScope m_reportScope;

			internal bool AllowCustomAggregates => m_allowCustomAggregates;

			internal List<DataAggregateInfo> Aggregates => m_aggregates;

			internal List<DataAggregateInfo> PostSortAggregates => m_postSortAggregates;

			internal List<DataAggregateInfo> RecursiveAggregates => m_recursiveAggregates;

			internal IRIFReportScope ReportScope => m_reportScope;

			internal Grouping GroupingScope => m_groupingScope;

			internal DataRegion DataRegionScope => m_dataRegionScope;

			internal DataSet DataSetScope => m_dataSetScope;

			internal IRIFDataScope DataScope
			{
				get
				{
					if (m_groupingScope != null)
					{
						return m_groupingScope.Owner;
					}
					if (m_dataRegionScope != null)
					{
						return m_dataRegionScope;
					}
					if (m_dataSetScope != null)
					{
						return m_dataSetScope;
					}
					return m_reportScope as IRIFDataScope;
				}
			}

			internal DataScopeInfo DataScopeInfo => DataScope?.DataScopeInfo;

			internal bool IsTopLevelScope => m_isTopLevelScope;

			internal bool IsDuplicateScope => m_duplicateScope;

			internal ScopeInfo(bool allowCustomAggregates, List<DataAggregateInfo> aggregates, IRIFReportScope reportScope)
				: this(allowCustomAggregates, aggregates, null, null, null, reportScope)
			{
				m_isTopLevelScope = true;
			}

			internal ScopeInfo(bool allowCustomAggregates, List<DataAggregateInfo> aggregates, List<DataAggregateInfo> postSortAggregates, IRIFReportScope reportScope)
				: this(allowCustomAggregates, aggregates, postSortAggregates, null, null, reportScope)
			{
			}

			internal ScopeInfo(bool allowCustomAggregates, List<DataAggregateInfo> aggregates, List<DataAggregateInfo> postSortAggregates, DataRegion dataRegion)
				: this(allowCustomAggregates, aggregates, postSortAggregates, null, null, dataRegion)
			{
				m_dataRegionScope = dataRegion;
			}

			internal ScopeInfo(bool allowCustomAggregates, List<DataAggregateInfo> aggregates, List<DataAggregateInfo> postSortAggregates, DataSet dataset, bool duplicateScope)
				: this(allowCustomAggregates, aggregates, postSortAggregates, dataset)
			{
				m_duplicateScope = duplicateScope;
			}

			internal ScopeInfo(bool allowCustomAggregates, List<DataAggregateInfo> aggregates, List<DataAggregateInfo> postSortAggregates, DataSet dataset)
				: this(allowCustomAggregates, aggregates, postSortAggregates, null, null, null)
			{
				m_dataSetScope = dataset;
			}

			internal ScopeInfo(bool allowCustomAggregates, List<DataAggregateInfo> aggregates, List<DataAggregateInfo> postSortAggregates, List<DataAggregateInfo> recursiveAggregates, Grouping groupingScope, IRIFReportScope reportScope)
			{
				m_allowCustomAggregates = allowCustomAggregates;
				m_aggregates = aggregates;
				m_postSortAggregates = postSortAggregates;
				m_recursiveAggregates = recursiveAggregates;
				m_reportScope = reportScope;
				m_groupingScope = groupingScope;
			}
		}

		internal class HashtableKeyComparer : IEqualityComparer<Hashtable>
		{
			private static HashtableKeyComparer m_Instance;

			internal static HashtableKeyComparer Instance
			{
				get
				{
					if (m_Instance == null)
					{
						m_Instance = new HashtableKeyComparer();
					}
					return m_Instance;
				}
			}

			private HashtableKeyComparer()
			{
			}

			public bool Equals(Hashtable x, Hashtable y)
			{
				if (x != y)
				{
					if (x.Count != y.Count)
					{
						return false;
					}
					foreach (object key in x.Keys)
					{
						if (!y.ContainsKey(key))
						{
							return false;
						}
					}
				}
				return true;
			}

			public int GetHashCode(Hashtable obj)
			{
				int num = obj.Count;
				foreach (object key in obj.Keys)
				{
					num ^= key.GetHashCode();
				}
				return num;
			}
		}

		private sealed class AxisGroupingScopesForRunningValues
		{
			private int m_columnGroupExprCount;

			private int m_rowGroupExprCount;

			private int m_previousDRsColumnGroupExprCount;

			private int m_previousDRsRowGroupExprCount;

			private int m_rowDetailCount;

			private int m_colDetailCount;

			private int m_previousDRsColumnDetailCount;

			private int m_previousDRsRowDetailCount;

			internal bool InCurrentDataRegionDynamicRow
			{
				get
				{
					if (m_rowGroupExprCount <= m_previousDRsRowGroupExprCount)
					{
						return m_rowDetailCount > m_previousDRsRowDetailCount;
					}
					return true;
				}
			}

			internal bool InCurrentDataRegionDynamicColumn
			{
				get
				{
					if (m_columnGroupExprCount <= m_previousDRsColumnGroupExprCount)
					{
						return m_colDetailCount > m_previousDRsColumnDetailCount;
					}
					return true;
				}
			}

			internal AxisGroupingScopesForRunningValues()
			{
			}

			internal AxisGroupingScopesForRunningValues(AxisGroupingScopesForRunningValues axisGroupingScopesForRunningValues)
			{
				m_columnGroupExprCount = axisGroupingScopesForRunningValues.m_columnGroupExprCount;
				m_rowGroupExprCount = axisGroupingScopesForRunningValues.m_rowGroupExprCount;
				m_colDetailCount = axisGroupingScopesForRunningValues.m_colDetailCount;
				m_rowDetailCount = axisGroupingScopesForRunningValues.m_rowDetailCount;
				SetCountsForDataRegion();
			}

			internal void RegisterColumnGrouping(Grouping grouping)
			{
				if (!grouping.IsDetail)
				{
					m_columnGroupExprCount += grouping.GroupExpressions.Count;
				}
				else
				{
					m_colDetailCount++;
				}
			}

			internal void RegisterRowGrouping(Grouping grouping)
			{
				if (!grouping.IsDetail)
				{
					m_rowGroupExprCount += grouping.GroupExpressions.Count;
				}
				else
				{
					m_rowDetailCount++;
				}
			}

			internal void UnregisterColumnGrouping(Grouping grouping)
			{
				if (!grouping.IsDetail)
				{
					m_columnGroupExprCount -= grouping.GroupExpressions.Count;
				}
				else
				{
					m_colDetailCount--;
				}
			}

			internal void UnregisterRowGrouping(Grouping grouping)
			{
				if (!grouping.IsDetail)
				{
					m_rowGroupExprCount -= grouping.GroupExpressions.Count;
				}
				else
				{
					m_rowDetailCount--;
				}
			}

			private void SetCountsForDataRegion()
			{
				m_previousDRsColumnGroupExprCount = m_columnGroupExprCount;
				m_previousDRsRowGroupExprCount = m_rowGroupExprCount;
				m_previousDRsColumnDetailCount = m_colDetailCount;
				m_previousDRsRowDetailCount = m_rowDetailCount;
			}
		}

		private sealed class GroupingScopesForTablix
		{
			private Microsoft.ReportingServices.ReportProcessing.ObjectType m_containerType;

			private DataRegion m_containerScope;

			private Hashtable m_rowScopes;

			private Hashtable m_columnScopes;

			private FunctionalList<Grouping> m_rowScopeStack;

			private FunctionalList<Grouping> m_columnScopeStack;

			internal bool CurrentRowScopeIsDetail
			{
				get
				{
					if (m_rowScopeStack.Count != 0)
					{
						return m_rowScopeStack.First.IsDetail;
					}
					return true;
				}
			}

			internal bool CurrentColumnScopeIsDetail
			{
				get
				{
					if (m_columnScopeStack.Count != 0)
					{
						return m_columnScopeStack.First.IsDetail;
					}
					return true;
				}
			}

			internal string CurrentRowScopeName
			{
				get
				{
					if (m_rowScopeStack.Count != 0)
					{
						return m_rowScopeStack.First.Name;
					}
					return null;
				}
			}

			internal string CurrentColumnScopeName
			{
				get
				{
					if (m_columnScopeStack.Count != 0)
					{
						return m_columnScopeStack.First.Name;
					}
					return null;
				}
			}

			internal DataRegion ContainerScope => m_containerScope;

			internal string ContainerName => m_containerScope.Name;

			internal bool IsRunningValueDirectionColumn => m_containerScope.ColumnScopeFound;

			internal GroupingScopesForTablix(bool forceRows, Microsoft.ReportingServices.ReportProcessing.ObjectType containerType, DataRegion containerScope)
			{
				containerScope.RowScopeFound = forceRows;
				containerScope.ColumnScopeFound = false;
				m_containerType = containerType;
				m_rowScopes = new Hashtable();
				m_columnScopes = new Hashtable();
				m_rowScopeStack = FunctionalList<Grouping>.Empty;
				m_columnScopeStack = FunctionalList<Grouping>.Empty;
				m_containerScope = containerScope;
			}

			internal ScopeChainInfo GetScopeChainInfo()
			{
				return new ScopeChainInfo(m_containerScope, m_rowScopeStack, m_columnScopeStack);
			}

			internal DataRegion SetContainerScope(DataRegion dataRegion)
			{
				DataRegion containerScope = m_containerScope;
				m_containerScope = dataRegion;
				return containerScope;
			}

			internal void RegisterRowGrouping(Grouping grouping)
			{
				if (!m_rowScopes.ContainsKey(grouping.Name))
				{
					m_rowScopes[grouping.Name] = null;
					m_rowScopeStack = m_rowScopeStack.Add(grouping);
				}
			}

			internal void UnRegisterRowGrouping(string groupName)
			{
				m_rowScopes.Remove(groupName);
				m_rowScopeStack = m_rowScopeStack.Rest;
			}

			internal void RegisterColumnGrouping(Grouping grouping)
			{
				if (!m_columnScopes.ContainsKey(grouping.Name))
				{
					m_columnScopes[grouping.Name] = null;
					m_columnScopeStack = m_columnScopeStack.Add(grouping);
				}
			}

			internal void UnRegisterColumnGrouping(string groupName)
			{
				m_columnScopes.Remove(groupName);
				m_columnScopeStack = m_columnScopeStack.Rest;
			}

			private ProcessingErrorCode getErrorCode()
			{
				switch (m_containerType)
				{
				case Microsoft.ReportingServices.ReportProcessing.ObjectType.Tablix:
					return ProcessingErrorCode.rsConflictingRunningValueScopesInTablix;
				case Microsoft.ReportingServices.ReportProcessing.ObjectType.CustomReportItem:
					return ProcessingErrorCode.rsConflictingRunningValueScopesInTablix;
				case Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart:
					return ProcessingErrorCode.rsConflictingRunningValueScopesInTablix;
				default:
					Global.Tracer.Assert(condition: false);
					return ProcessingErrorCode.rsConflictingRunningValueScopesInTablix;
				}
			}

			internal bool HasRowColScopeConflict(string textboxSortActionScope, string sortTargetScope, out bool bothGroups)
			{
				bothGroups = false;
				if (textboxSortActionScope != null && sortTargetScope != null)
				{
					if (m_rowScopes.ContainsKey(textboxSortActionScope))
					{
						if (m_rowScopes.ContainsKey(sortTargetScope))
						{
							bothGroups = true;
							return false;
						}
						if (m_columnScopes.ContainsKey(sortTargetScope))
						{
							bothGroups = true;
							return true;
						}
					}
					else if (m_columnScopes.ContainsKey(textboxSortActionScope))
					{
						if (m_columnScopes.ContainsKey(sortTargetScope))
						{
							bothGroups = true;
							return false;
						}
						if (m_rowScopes.ContainsKey(sortTargetScope))
						{
							bothGroups = true;
							return true;
						}
					}
				}
				return false;
			}

			internal bool ContainsScope(string scope)
			{
				return ContainsScope(scope, null, checkConflictingScope: false, null);
			}

			internal bool ContainsScope(string scope, ErrorContext errorContext, bool checkConflictingScope, Hashtable groupingScopes)
			{
				Global.Tracer.Assert(scope != null, "(null != scope)");
				if (m_rowScopes.ContainsKey(scope))
				{
					if (checkConflictingScope)
					{
						DataRegion dataRegionDef = ((ScopeInfo)groupingScopes[scope]).GroupingScope.Owner.DataRegionDef;
						if (dataRegionDef.ColumnScopeFound)
						{
							errorContext.Register(getErrorCode(), Severity.Error, dataRegionDef.ObjectType, ContainerName, null);
						}
						dataRegionDef.RowScopeFound = true;
					}
					return true;
				}
				if (m_columnScopes.ContainsKey(scope))
				{
					if (checkConflictingScope)
					{
						DataRegion dataRegionDef2 = ((ScopeInfo)groupingScopes[scope]).GroupingScope.Owner.DataRegionDef;
						if (dataRegionDef2.RowScopeFound)
						{
							errorContext.Register(getErrorCode(), Severity.Error, dataRegionDef2.ObjectType, ContainerName, null);
						}
						dataRegionDef2.ColumnScopeFound = true;
					}
					return true;
				}
				return false;
			}
		}

		private sealed class LookupDestinationCompactionTable : Dictionary<string, int>
		{
			internal LookupDestinationCompactionTable()
			{
			}
		}

		internal sealed class ScopeChainInfo
		{
			private DataRegion m_containingDataRegion;

			private FunctionalList<Grouping> m_rowGroupList;

			private FunctionalList<Grouping> m_columnGroupList;

			internal ScopeChainInfo(DataRegion containingDataRegion, FunctionalList<Grouping> rowGroupList, FunctionalList<Grouping> columnGroupList)
			{
				m_containingDataRegion = containingDataRegion;
				m_rowGroupList = rowGroupList;
				m_columnGroupList = columnGroupList;
			}

			internal Grouping GetInnermostGrouping()
			{
				if (m_containingDataRegion.ProcessingInnerGrouping == DataRegion.ProcessingInnerGroupings.Column)
				{
					if (!m_columnGroupList.IsEmpty() && m_columnGroupList.First.Owner.DataRegionDef == m_containingDataRegion)
					{
						return m_columnGroupList.First;
					}
					if (!m_rowGroupList.IsEmpty() && m_rowGroupList.First.Owner.DataRegionDef == m_containingDataRegion)
					{
						return m_rowGroupList.First;
					}
				}
				else
				{
					if (!m_rowGroupList.IsEmpty() && m_rowGroupList.First.Owner.DataRegionDef == m_containingDataRegion)
					{
						return m_rowGroupList.First;
					}
					if (!m_columnGroupList.IsEmpty() && m_columnGroupList.First.Owner.DataRegionDef == m_containingDataRegion)
					{
						return m_columnGroupList.First;
					}
				}
				return m_containingDataRegion.ScopeChainInfo?.GetInnermostGrouping();
			}

			internal GroupingList GetGroupingList()
			{
				GroupingList groupingList = new GroupingList();
				AddAllGroups(groupingList);
				groupingList.Reverse();
				return groupingList;
			}

			private void AddAllGroups(GroupingList groups)
			{
				AddGroupsForContainingDataRegion(groups);
				m_containingDataRegion.ScopeChainInfo?.AddAllGroups(groups);
			}

			internal GroupingList GetGroupingListForContainingDataRegion()
			{
				GroupingList groupingList = new GroupingList();
				AddGroupsForContainingDataRegion(groupingList);
				groupingList.Reverse();
				return groupingList;
			}

			internal GroupingList GetGroupsFromCurrentTablixAxisToGrouping(Grouping fromGroup)
			{
				Grouping innermostGrouping = GetInnermostGrouping();
				if (innermostGrouping != null)
				{
					return GetGroupsFromCurrentTablixAxisToGrouping(innermostGrouping.Owner.DataRegionDef, innermostGrouping.Owner.IsColumn, fromGroup);
				}
				return null;
			}

			internal GroupingList GetGroupsFromCurrentTablixAxisToGrouping(DataRegion dataRegion, bool isColumn, Grouping fromGroup)
			{
				if (dataRegion == m_containingDataRegion)
				{
					GroupingList groupingList = new GroupingList();
					if (isColumn)
					{
						AddGroupsToList(m_columnGroupList, groupingList, fromGroup);
					}
					else
					{
						AddGroupsToList(m_rowGroupList, groupingList, fromGroup);
					}
					if (fromGroup == null || fromGroup.Owner.DataRegionDef != m_containingDataRegion)
					{
						m_containingDataRegion.ScopeChainInfo?.GetGroupsFromCurrentTablixAxisToGrouping(groupingList, fromGroup);
					}
					if (groupingList.Count > 0)
					{
						groupingList.Reverse();
						return groupingList;
					}
					return null;
				}
				return m_containingDataRegion.ScopeChainInfo?.GetGroupsFromCurrentTablixAxisToGrouping(dataRegion, isColumn, fromGroup);
			}

			internal void GetGroupsFromCurrentTablixAxisToGrouping(GroupingList groups, Grouping fromGroup)
			{
				bool flag = m_containingDataRegion.ProcessingInnerGrouping == DataRegion.ProcessingInnerGroupings.Column;
				bool flag2 = false;
				bool flag3 = false;
				if (fromGroup != null && fromGroup.Owner.DataRegionDef == m_containingDataRegion)
				{
					flag2 = true;
					if (flag == fromGroup.Owner.IsColumn)
					{
						flag3 = true;
					}
				}
				if (flag)
				{
					if (!flag3)
					{
						AddGroupsToList(m_rowGroupList, groups, fromGroup);
					}
					AddGroupsToList(m_columnGroupList, groups, fromGroup);
				}
				else
				{
					if (!flag3)
					{
						AddGroupsToList(m_columnGroupList, groups, fromGroup);
					}
					AddGroupsToList(m_rowGroupList, groups, fromGroup);
				}
				if (!flag2)
				{
					m_containingDataRegion.ScopeChainInfo?.GetGroupsFromCurrentTablixAxisToGrouping(groups, fromGroup);
				}
			}

			private void AddGroupsForContainingDataRegion(GroupingList groups)
			{
				if (m_containingDataRegion.ProcessingInnerGrouping == DataRegion.ProcessingInnerGroupings.Column)
				{
					AddGroupsToList(m_rowGroupList, groups, null);
					AddGroupsToList(m_columnGroupList, groups, null);
				}
				else
				{
					AddGroupsToList(m_columnGroupList, groups, null);
					AddGroupsToList(m_rowGroupList, groups, null);
				}
			}

			private void AddGroupsToList(FunctionalList<Grouping> groupings, GroupingList containingGroups, Grouping fromGroup)
			{
				while (!groupings.IsEmpty())
				{
					Grouping first = groupings.First;
					groupings = groupings.Rest;
					if (first.Owner.DataRegionDef == m_containingDataRegion && first != fromGroup)
					{
						containingGroups.Add(first);
						continue;
					}
					break;
				}
			}

			internal bool IsSameOrChildScope(string parentScope, string childScope)
			{
				return IsSameOrChildScope(parentScope, childScope, foundChild: false);
			}

			private bool IsSameOrChildScope(string parentScope, string childScope, bool foundChild)
			{
				bool num = m_containingDataRegion.ProcessingInnerGrouping == DataRegion.ProcessingInnerGroupings.Column;
				bool foundParent = false;
				if (num)
				{
					foundChild = IsSameOrChildScope(m_rowGroupList, parentScope, childScope, foundChild, out foundParent);
					if (!foundParent)
					{
						foundChild = IsSameOrChildScope(m_columnGroupList, parentScope, childScope, foundChild, out foundParent);
					}
				}
				else
				{
					foundChild = IsSameOrChildScope(m_columnGroupList, parentScope, childScope, foundChild, out foundParent);
					if (!foundParent)
					{
						foundChild = IsSameOrChildScope(m_rowGroupList, parentScope, childScope, foundChild, out foundParent);
					}
				}
				if (!foundParent)
				{
					ScopeChainInfo scopeChainInfo = m_containingDataRegion.ScopeChainInfo;
					if (scopeChainInfo != null)
					{
						return scopeChainInfo.IsSameOrChildScope(parentScope, childScope, foundChild);
					}
				}
				return foundChild && foundParent;
			}

			private bool IsSameOrChildScope(FunctionalList<Grouping> groupings, string parentScope, string childScope, bool foundChild, out bool foundParent)
			{
				foundParent = false;
				while (!groupings.IsEmpty())
				{
					Grouping first = groupings.First;
					groupings = groupings.Rest;
					if (first.Owner.DataRegionDef != m_containingDataRegion)
					{
						break;
					}
					if (first.Name == childScope)
					{
						foundChild = true;
					}
					if (first.Name == parentScope)
					{
						foundParent = true;
						break;
					}
				}
				return foundChild;
			}
		}

		private PublishingContextBase m_publishingContext;

		private ICatalogItemContext m_reportContext;

		private Microsoft.ReportingServices.ReportPublishing.LocationFlags m_location;

		private Microsoft.ReportingServices.ReportProcessing.ObjectType m_objectType;

		private string m_objectName;

		private string m_tablixName;

		private Dictionary<string, ImageInfo> m_embeddedImages;

		private ErrorContext m_errorContext;

		private Hashtable m_parameters;

		private ArrayList m_dynamicParameters;

		private Hashtable m_dataSetQueryInfo;

		private Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder m_exprHostBuilder;

		private Report m_report;

		private Dictionary<string, Variable> m_variablesInScope;

		private byte[] m_referencableTextboxes;

		private byte[] m_referencableTextboxesInSection;

		private byte[] m_referencableVariables;

		private string m_outerGroupName;

		private string m_currentGroupName;

		private string m_currentDataRegionName;

		private List<RunningValueInfo> m_runningValues;

		private List<RunningValueInfo> m_runningValuesOfAggregates;

		private Hashtable m_groupingScopesForRunningValues;

		private GroupingScopesForTablix m_groupingScopesForRunningValuesInTablix;

		private Hashtable m_dataregionScopesForRunningValues;

		private AxisGroupingScopesForRunningValues m_axisGroupingScopesForRunningValues;

		private Dictionary<string, int> m_groupingExprCountAtScope;

		private bool m_hasFilters;

		private ScopeInfo m_currentScope;

		private ScopeInfo m_outermostDataregionScope;

		private Hashtable m_groupingScopes;

		private Hashtable m_dataregionScopes;

		private Hashtable m_datasetScopes;

		private ScopeTree m_scopeTree;

		private Dictionary<int, IRIFDataScope> m_dataSetsForNonStructuralIdc;

		private Dictionary<int, IRIFDataScope> m_dataSetsForIdcInNestedDR;

		private Dictionary<int, IRIFDataScope> m_dataSetsForIdc;

		private int m_numberOfDataSets;

		private string m_oneDataSetName;

		private FunctionalList<DataSet> m_activeDataSets;

		private FunctionalList<ScopeInfo> m_activeScopeInfos;

		private Hashtable m_fieldNameMap;

		private Dictionary<string, List<DataRegion>> m_dataSetNameToDataRegionsMap;

		private Dictionary<string, LookupDestinationCompactionTable> m_lookupCompactionTable;

		private StringDictionary m_dataSources;

		private Dictionary<string, Pair<ReportItem, int>> m_reportItemsInScope;

		private Dictionary<string, ReportItem> m_reportItemsInSection;

		private Holder<bool> m_handledCellContents;

		private CultureInfo m_reportLanguage;

		private bool m_reportDataElementStyleAttribute;

		private bool m_hasUserSortPeerScopes;

		private bool m_hasUserSorts;

		private Dictionary<string, List<IInScopeEventSource>> m_userSortExpressionScopes;

		private Dictionary<string, List<IInScopeEventSource>> m_userSortEventSources;

		private Hashtable m_peerScopes;

		private int m_lastPeerScopeId;

		private Dictionary<string, ISortFilterScope> m_reportScopes;

		private Hashtable m_reportScopeDatasets;

		private bool m_initializingUserSorts;

		private List<IInScopeEventSource> m_detailSortExpressionScopeEventSources;

		private IList<Pair<double, int>> m_columnHeaderLevelSizeList;

		private IList<Pair<double, int>> m_rowHeaderLevelSizeList;

		private bool m_hasPreviousAggregates;

		private bool m_inAutoSubtotalClone;

		private List<VisibilityToggleInfo> m_visibilityToggleInfos;

		private Dictionary<string, ToggleItemInfo> m_toggleItems;

		private Stack<VisibilityContainmentInfo> m_visibilityContainmentInfos;

		private bool m_isTopLevelCellContents;

		private bool m_isDataRegionScopedCell;

		private bool m_inRecursiveHierarchyRows;

		private bool m_inRecursiveHierarchyColumns;

		private Holder<int> m_memberCellIndex;

		private Dictionary<Hashtable, int> m_indexInCollectionTableForDataRegions;

		private Dictionary<Hashtable, int> m_indexInCollectionTableForSubReports;

		private Dictionary<Hashtable, int> m_indexInCollectionTable;

		private Dictionary<string, List<ExpressionInfo>> m_naturalGroupExpressionsByDataSetName;

		private Dictionary<string, List<ExpressionInfo>> m_naturalSortExpressionsByDataSetName;

		private double m_currentAbsoluteTop;

		private double m_currentAbsoluteLeft;

		internal ICatalogItemContext ReportContext => m_reportContext;

		internal Microsoft.ReportingServices.ReportPublishing.LocationFlags Location
		{
			get
			{
				return m_location;
			}
			set
			{
				m_location = value;
			}
		}

		internal Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return m_objectType;
			}
			set
			{
				m_objectType = value;
			}
		}

		internal string ObjectName
		{
			get
			{
				return m_objectName;
			}
			set
			{
				m_objectName = value;
			}
		}

		internal bool IsTopLevelCellContents
		{
			get
			{
				return m_isTopLevelCellContents;
			}
			set
			{
				m_isTopLevelCellContents = value;
			}
		}

		internal bool HasUserSorts => m_hasUserSorts;

		internal bool InitializingUserSorts
		{
			get
			{
				return m_initializingUserSorts;
			}
			set
			{
				m_initializingUserSorts = value;
			}
		}

		internal bool IsDataRegionCellScope
		{
			get
			{
				if (m_axisGroupingScopesForRunningValues.InCurrentDataRegionDynamicRow)
				{
					return m_axisGroupingScopesForRunningValues.InCurrentDataRegionDynamicColumn;
				}
				return false;
			}
		}

		internal bool CellHasDynamicRowsAndColumns
		{
			get
			{
				if (m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName != null)
				{
					return m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName != null;
				}
				return false;
			}
		}

		internal bool IsDataRegionScopedCell
		{
			get
			{
				return m_isDataRegionScopedCell;
			}
			set
			{
				m_isDataRegionScopedCell = value;
			}
		}

		internal bool ReportDataElementStyleAttribute
		{
			get
			{
				return m_reportDataElementStyleAttribute;
			}
			set
			{
				m_reportDataElementStyleAttribute = value;
			}
		}

		internal string TablixName
		{
			get
			{
				return m_tablixName;
			}
			set
			{
				m_tablixName = value;
			}
		}

		internal Dictionary<string, ImageInfo> EmbeddedImages => m_embeddedImages;

		internal ErrorContext ErrorContext => m_errorContext;

		internal Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder ExprHostBuilder => m_exprHostBuilder;

		internal bool MergeOnePass => m_report.MergeOnePass;

		internal CultureInfo ReportLanguage => m_reportLanguage;

		internal IList<Pair<double, int>> ColumnHeaderLevelSizeList
		{
			get
			{
				return m_columnHeaderLevelSizeList;
			}
			set
			{
				m_columnHeaderLevelSizeList = value;
			}
		}

		internal IList<Pair<double, int>> RowHeaderLevelSizeList
		{
			get
			{
				return m_rowHeaderLevelSizeList;
			}
			set
			{
				m_rowHeaderLevelSizeList = value;
			}
		}

		internal bool InAutoSubtotalClone
		{
			get
			{
				return m_inAutoSubtotalClone;
			}
			set
			{
				m_inAutoSubtotalClone = value;
			}
		}

		internal int MemberCellIndex
		{
			get
			{
				return m_memberCellIndex.Value;
			}
			set
			{
				m_memberCellIndex.Value = value;
			}
		}

		internal double CurrentAbsoluteTop => m_currentAbsoluteTop;

		internal double CurrentAbsoluteLeft => m_currentAbsoluteLeft;

		internal bool HasPreviousAggregates => m_hasPreviousAggregates;

		internal Dictionary<string, int> GroupingExprCountAtScope => m_groupingExprCountAtScope;

		internal bool IsRunningValueDirectionColumn
		{
			get
			{
				if (m_groupingScopesForRunningValuesInTablix != null)
				{
					return m_groupingScopesForRunningValuesInTablix.IsRunningValueDirectionColumn;
				}
				return true;
			}
		}

		internal bool HasLookups => m_lookupCompactionTable.Count > 0;

		internal Report Report => m_report;

		internal ScopeTree ScopeTree => m_scopeTree;

		internal bool HandledCellContents
		{
			get
			{
				return m_handledCellContents.Value;
			}
			set
			{
				m_handledCellContents.Value = value;
			}
		}

		internal bool InRecursiveHierarchyColumns
		{
			get
			{
				return m_inRecursiveHierarchyColumns;
			}
			set
			{
				m_inRecursiveHierarchyColumns = value;
			}
		}

		internal bool InRecursiveHierarchyRows
		{
			get
			{
				return m_inRecursiveHierarchyRows;
			}
			set
			{
				m_inRecursiveHierarchyRows = value;
			}
		}

		internal PublishingContextBase PublishingContext => m_publishingContext;

		internal InitializationContext(ICatalogItemContext reportContext, List<DataSet> datasets, ErrorContext errorContext, Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder exprHostBuilder, Report artificialReportContainerForCodeGeneration, Dictionary<string, ISortFilterScope> reportScopes, PublishingContextBase publishingContext)
			: this(reportContext, hasFilters: false, null, datasets, null, null, errorContext, exprHostBuilder, artificialReportContainerForCodeGeneration, null, reportScopes, hasUserSortPeerScopes: false, hasUserSort: false, 0, 0, 0, publishingContext, new ScopeTree(), isSharedDataSetContext: true)
		{
		}

		internal InitializationContext(ICatalogItemContext reportContext, bool hasFilters, StringDictionary dataSources, List<DataSet> dataSets, ArrayList dynamicParameters, Hashtable dataSetQueryInfo, ErrorContext errorContext, Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder exprHostBuilder, Report report, CultureInfo reportLanguage, Dictionary<string, ISortFilterScope> reportScopes, bool hasUserSortPeerScopes, bool hasUserSort, int dataRegionCount, int textboxCount, int variableCount, PublishingContextBase publishingContext, ScopeTree scopeTree, bool isSharedDataSetContext)
		{
			Global.Tracer.Assert(errorContext != null, "(null != errorContext)");
			Global.Tracer.Assert(reportContext != null, "(null != reportContext)");
			m_publishingContext = publishingContext;
			m_reportContext = reportContext;
			m_location = Microsoft.ReportingServices.ReportPublishing.LocationFlags.None;
			m_objectType = Microsoft.ReportingServices.ReportProcessing.ObjectType.Report;
			m_objectName = null;
			m_tablixName = null;
			m_embeddedImages = report.EmbeddedImages;
			m_errorContext = errorContext;
			m_parameters = null;
			m_dynamicParameters = dynamicParameters;
			m_dataSetQueryInfo = dataSetQueryInfo;
			m_exprHostBuilder = exprHostBuilder;
			m_dataSources = dataSources;
			m_rowHeaderLevelSizeList = null;
			m_columnHeaderLevelSizeList = null;
			m_report = report;
			m_reportLanguage = reportLanguage;
			m_outerGroupName = null;
			m_currentGroupName = null;
			m_currentDataRegionName = null;
			m_runningValues = null;
			m_runningValuesOfAggregates = null;
			m_groupingScopesForRunningValues = new Hashtable();
			m_groupingScopesForRunningValuesInTablix = null;
			m_dataregionScopesForRunningValues = new Hashtable();
			m_scopeTree = scopeTree;
			m_hasFilters = hasFilters;
			m_currentScope = null;
			m_outermostDataregionScope = null;
			m_groupingScopes = new Hashtable();
			m_dataregionScopes = new Hashtable();
			m_datasetScopes = new Hashtable();
			m_numberOfDataSets = 0;
			m_oneDataSetName = null;
			m_activeDataSets = FunctionalList<DataSet>.Empty;
			m_activeScopeInfos = FunctionalList<ScopeInfo>.Empty;
			m_fieldNameMap = new Hashtable();
			m_dataSetNameToDataRegionsMap = new Dictionary<string, List<DataRegion>>();
			m_isTopLevelCellContents = false;
			m_isDataRegionScopedCell = false;
			if (dataSets != null)
			{
				m_numberOfDataSets = dataSets.Count;
				m_oneDataSetName = ((1 == dataSets.Count) ? dataSets[0].Name : null);
				for (int i = 0; i < dataSets.Count; i++)
				{
					DataSet dataSet = dataSets[i];
					bool flag = m_datasetScopes.ContainsKey(dataSets[i].Name);
					m_datasetScopes[dataSets[i].Name] = new ScopeInfo(allowCustomAggregates: true, dataSets[i].Aggregates, dataSets[i].PostSortAggregates, dataSets[i], flag);
					Hashtable hashtable = new Hashtable();
					if (dataSet.Fields != null)
					{
						for (int j = 0; j < dataSet.Fields.Count; j++)
						{
							hashtable[dataSet.Fields[j].Name] = j;
						}
					}
					m_fieldNameMap[dataSet.Name] = hashtable;
					if (!flag)
					{
						m_dataSetNameToDataRegionsMap.Add(dataSet.Name, dataSet.DataRegions);
					}
				}
			}
			if (report != null && report.Parameters != null)
			{
				m_parameters = new Hashtable();
				for (int k = 0; k < report.Parameters.Count; k++)
				{
					ParameterDef parameterDef = report.Parameters[k];
					if (parameterDef != null && !m_parameters.ContainsKey(parameterDef.Name))
					{
						m_parameters.Add(parameterDef.Name, parameterDef);
					}
				}
			}
			m_reportItemsInScope = new Dictionary<string, Pair<ReportItem, int>>();
			m_reportItemsInSection = new Dictionary<string, ReportItem>();
			m_variablesInScope = new Dictionary<string, Variable>();
			m_referencableVariables = new byte[(variableCount >> 3) + 1];
			m_referencableTextboxes = new byte[(textboxCount >> 3) + 1];
			m_referencableTextboxesInSection = new byte[(textboxCount >> 3) + 1];
			m_reportDataElementStyleAttribute = true;
			m_hasUserSorts = hasUserSort;
			m_hasUserSortPeerScopes = hasUserSortPeerScopes;
			m_lastPeerScopeId = 0;
			m_reportScopes = reportScopes;
			m_initializingUserSorts = false;
			if (hasUserSort || m_report.HasSubReports)
			{
				m_userSortExpressionScopes = new Dictionary<string, List<IInScopeEventSource>>();
				m_userSortEventSources = new Dictionary<string, List<IInScopeEventSource>>();
				m_peerScopes = (hasUserSortPeerScopes ? new Hashtable() : null);
				m_reportScopeDatasets = new Hashtable();
				m_detailSortExpressionScopeEventSources = new List<IInScopeEventSource>();
			}
			else
			{
				m_userSortExpressionScopes = null;
				m_userSortEventSources = null;
				m_peerScopes = null;
				m_reportScopeDatasets = null;
				m_detailSortExpressionScopeEventSources = null;
			}
			m_inAutoSubtotalClone = false;
			m_visibilityToggleInfos = new List<VisibilityToggleInfo>();
			m_toggleItems = new Dictionary<string, ToggleItemInfo>();
			m_visibilityContainmentInfos = new Stack<VisibilityContainmentInfo>();
			m_memberCellIndex = null;
			m_indexInCollectionTableForDataRegions = new Dictionary<Hashtable, int>(HashtableKeyComparer.Instance);
			m_indexInCollectionTableForSubReports = new Dictionary<Hashtable, int>(HashtableKeyComparer.Instance);
			m_indexInCollectionTable = null;
			m_currentAbsoluteTop = 0.0;
			m_currentAbsoluteLeft = 0.0;
			m_hasPreviousAggregates = report.HasPreviousAggregates;
			m_axisGroupingScopesForRunningValues = null;
			m_groupingExprCountAtScope = new Dictionary<string, int>();
			m_lookupCompactionTable = new Dictionary<string, LookupDestinationCompactionTable>();
			m_handledCellContents = new Holder<bool>();
			m_handledCellContents.Value = false;
			m_inRecursiveHierarchyColumns = false;
			m_inRecursiveHierarchyRows = false;
			m_naturalGroupExpressionsByDataSetName = new Dictionary<string, List<ExpressionInfo>>();
			m_naturalSortExpressionsByDataSetName = new Dictionary<string, List<ExpressionInfo>>();
			m_dataSetsForIdcInNestedDR = new Dictionary<int, IRIFDataScope>();
			m_dataSetsForIdc = new Dictionary<int, IRIFDataScope>();
			m_dataSetsForNonStructuralIdc = new Dictionary<int, IRIFDataScope>();
		}

		internal void RSDRegisterDataSetParameters(DataSetCore sharedDataset)
		{
			m_parameters = new Hashtable();
			if (sharedDataset == null || sharedDataset.Query == null || sharedDataset.Query.Parameters == null)
			{
				return;
			}
			int count = sharedDataset.Query.Parameters.Count;
			for (int i = 0; i < count; i++)
			{
				DataSetParameterValue dataSetParameterValue = sharedDataset.Query.Parameters[i] as DataSetParameterValue;
				if (dataSetParameterValue != null && dataSetParameterValue.UniqueName != null && !m_parameters.ContainsKey(dataSetParameterValue.UniqueName))
				{
					m_parameters.Add(dataSetParameterValue.UniqueName, null);
				}
			}
		}

		internal void ValidateScopeRulesForNaturalGroup(ReportHierarchyNode member)
		{
			if (member.Grouping == null || !member.Grouping.NaturalGroup)
			{
				return;
			}
			ErrorContext errorContext = m_errorContext;
			List<ExpressionInfo> groupExpressions = new List<ExpressionInfo>();
			ScopeTree.DirectedScopeTreeVisitor validator = delegate(IRIFDataScope scope)
			{
				ReportHierarchyNode reportHierarchyNode = scope as ReportHierarchyNode;
				if (reportHierarchyNode != null && DataSet.AreEqualById(member.DataScopeInfo.DataSet, reportHierarchyNode.DataScopeInfo.DataSet))
				{
					if (!reportHierarchyNode.Grouping.NaturalGroup)
					{
						errorContext.Register(ProcessingErrorCode.rsInvalidGroupingContainerNotNaturalGroup, Severity.Warning, member.DataRegionDef.ObjectType, member.DataRegionDef.Name, "Grouping", member.Grouping.Name.MarkAsModelInfo(), reportHierarchyNode.Grouping.Name.MarkAsModelInfo());
						member.Grouping.NaturalGroup = false;
						return false;
					}
					AppendExpressions(groupExpressions, reportHierarchyNode.Grouping.GroupExpressions);
				}
				return true;
			};
			ValidateNaturalGroupOrSortRulesForScopeTree(m_naturalGroupExpressionsByDataSetName, member, groupExpressions, validator, ProcessingErrorCode.rsConflictingNaturalGroupRequirements, "NaturalGroup");
		}

		internal void ValidateScopeRulesForNaturalSort(ReportHierarchyNode member)
		{
			if (member.Grouping == null || member.Sorting == null || !member.Sorting.NaturalSort)
			{
				return;
			}
			ErrorContext errorContext = m_errorContext;
			List<ExpressionInfo> sortExpressions = new List<ExpressionInfo>();
			ScopeTree.DirectedScopeTreeVisitor validator = delegate(IRIFDataScope scope)
			{
				Sorting sorting = null;
				string plainString = null;
				ReportHierarchyNode reportHierarchyNode = scope as ReportHierarchyNode;
				DataRegion dataRegion = scope as DataRegion;
				if (reportHierarchyNode != null)
				{
					sorting = reportHierarchyNode.Sorting;
					plainString = reportHierarchyNode.Grouping.Name;
				}
				else if (dataRegion != null)
				{
					sorting = dataRegion.Sorting;
					plainString = dataRegion.Name;
				}
				if (sorting != null && member.DataScopeInfo.DataSet.ID == scope.DataScopeInfo.DataSet.ID)
				{
					if (!sorting.NaturalSort)
					{
						errorContext.Register(ProcessingErrorCode.rsInvalidSortingContainerNotNaturalSort, Severity.Warning, member.DataRegionDef.ObjectType, member.DataRegionDef.Name, "SortExpressions", member.Grouping.Name.MarkAsModelInfo(), plainString.MarkAsPrivate());
						member.Sorting.NaturalSort = false;
						return false;
					}
					AppendExpressions(sortExpressions, reportHierarchyNode.Sorting.SortExpressions);
				}
				return true;
			};
			ValidateNaturalGroupOrSortRulesForScopeTree(m_naturalSortExpressionsByDataSetName, member, sortExpressions, validator, ProcessingErrorCode.rsConflictingNaturalSortRequirements, "NaturalSort");
		}

		internal void ValidateScopeRulesForIdcNaturalJoin(IRIFDataScope startScope)
		{
			ErrorContext errorContext = m_errorContext;
			ScopeTree.DirectedScopeTreeVisitor visitor = delegate(IRIFDataScope scope)
			{
				if (scope.DataScopeInfo != null && scope.DataScopeInfo.JoinInfo != null)
				{
					scope.DataScopeInfo.JoinInfo.CheckContainerJoinForNaturalJoin(startScope, errorContext, scope);
				}
				ReportHierarchyNode reportHierarchyNode = scope as ReportHierarchyNode;
				if (reportHierarchyNode != null && !reportHierarchyNode.Grouping.NaturalGroup)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidRelationshipGroupingContainerNotNaturalGroup, Severity.Error, startScope.DataScopeObjectType, startScope.Name, "Grouping", scope.Name);
					return false;
				}
				return true;
			};
			m_scopeTree.Traverse(visitor, startScope);
		}

		private void ValidateNaturalGroupOrSortRulesForScopeTree(Dictionary<string, List<ExpressionInfo>> expressionsByDataSetName, ReportHierarchyNode member, List<ExpressionInfo> expressions, ScopeTree.DirectedScopeTreeVisitor validator, ProcessingErrorCode conflictingNaturalRequirementErrorCode, string naturalElementName)
		{
			if (!m_scopeTree.Traverse(validator, member))
			{
				return;
			}
			string currentDataSetName = GetCurrentDataSetName();
			if (currentDataSetName == null)
			{
				return;
			}
			if (expressionsByDataSetName.TryGetValue(currentDataSetName, out List<ExpressionInfo> value))
			{
				if (!ListUtils.AreSameOrSuffix(expressions, value, RdlExpressionComparer.Instance))
				{
					m_errorContext.Register(conflictingNaturalRequirementErrorCode, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.DataSet, currentDataSetName, naturalElementName);
				}
				if (value.Count > expressions.Count)
				{
					expressions = value;
				}
			}
			expressionsByDataSetName[currentDataSetName] = expressions;
		}

		private static void AppendExpressions(List<ExpressionInfo> allExpressions, List<ExpressionInfo> localExpressions)
		{
			if (localExpressions != null)
			{
				allExpressions.AddRange(localExpressions);
			}
		}

		internal void EnsureDataSetUsedOnceForIdcUnderTopDataRegion(DataSet dataSet, IRIFDataScope currentScope)
		{
			if (currentScope.DataScopeInfo.NeedsIDC && !IsErrorForDuplicateIdcDataSet(m_dataSetsForIdcInNestedDR, dataSet, currentScope, ProcessingErrorCode.rsInvalidRelationshipDataSetUsedMoreThanOnce))
			{
				IsErrorForDuplicateIdcDataSet(m_dataSetsForIdc, dataSet, currentScope, ProcessingErrorCode.rsInvalidRelationshipDataSet);
			}
		}

		private bool IsErrorForDuplicateIdcDataSet(Dictionary<int, IRIFDataScope> mappingDataSetIdToFirstIdcScope, DataSet dataSet, IRIFDataScope currentScope, ProcessingErrorCode errorCode)
		{
			if (mappingDataSetIdToFirstIdcScope.TryGetValue(dataSet.ID, out IRIFDataScope value))
			{
				if (!IsOtherParentOfCurrentIntersection(value, currentScope) && !IsOtherSameIntersectionScope(value, currentScope))
				{
					m_errorContext.Register(errorCode, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.DataSet, dataSet.Name.MarkAsPrivate(), currentScope.DataScopeObjectType.ToString(), currentScope.Name, value.DataScopeObjectType.ToString(), value.Name);
				}
				return true;
			}
			mappingDataSetIdToFirstIdcScope.Add(dataSet.ID, currentScope);
			return false;
		}

		private bool IsOtherParentOfCurrentIntersection(IRIFDataScope otherScope, IRIFDataScope currentScope)
		{
			if (m_scopeTree.IsIntersectionScope(currentScope))
			{
				if (!m_scopeTree.IsSameOrParentScope(otherScope, m_scopeTree.GetParentRowScopeForIntersection(currentScope)))
				{
					return m_scopeTree.IsSameOrParentScope(otherScope, m_scopeTree.GetParentColumnScopeForIntersection(currentScope));
				}
				return true;
			}
			return false;
		}

		private bool IsOtherSameIntersectionScope(IRIFDataScope otherScope, IRIFDataScope currentScope)
		{
			if (m_scopeTree.IsIntersectionScope(currentScope) && m_scopeTree.IsIntersectionScope(otherScope) && ScopeTree.SameScope(m_scopeTree.GetParentRowScopeForIntersection(currentScope), m_scopeTree.GetParentRowScopeForIntersection(otherScope)))
			{
				return ScopeTree.SameScope(m_scopeTree.GetParentColumnScopeForIntersection(currentScope), m_scopeTree.GetParentColumnScopeForIntersection(otherScope));
			}
			return false;
		}

		private string GetCurrentDataSetName()
		{
			return m_activeDataSets.First?.Name;
		}

		private int GetCurrentDataSetIndex()
		{
			return m_activeDataSets.First?.IndexInCollection ?? (-1);
		}

		private void RegisterDataSetScope(DataSet dataSet, List<DataAggregateInfo> scopeAggregates, List<DataAggregateInfo> scopePostSortAggregates, int datasetIndexInCollection)
		{
			Global.Tracer.Assert(dataSet != null);
			Global.Tracer.Assert(scopeAggregates != null);
			Global.Tracer.Assert(scopePostSortAggregates != null);
			m_currentScope = new ScopeInfo(allowCustomAggregates: true, scopeAggregates, scopePostSortAggregates, dataSet);
			if (m_initializingUserSorts && !m_reportScopeDatasets.ContainsKey(dataSet.Name))
			{
				m_reportScopeDatasets.Add(dataSet.Name, dataSet);
			}
		}

		private void UnRegisterDataSetScope(string scopeName)
		{
			Global.Tracer.Assert(scopeName != null);
			m_currentScope = null;
		}

		private void RegisterDataRegionScope(DataRegion dataRegion)
		{
			Global.Tracer.Assert(dataRegion.Name != null);
			Global.Tracer.Assert(dataRegion.Aggregates != null);
			Global.Tracer.Assert(dataRegion.PostSortAggregates != null);
			m_currentDataRegionName = dataRegion.Name;
			m_dataregionScopesForRunningValues[dataRegion.Name] = m_currentGroupName;
			if (!m_initializingUserSorts)
			{
				SetIndexInCollection(dataRegion);
				m_memberCellIndex = new Holder<int>();
				m_indexInCollectionTableForDataRegions = new Dictionary<Hashtable, int>(HashtableKeyComparer.Instance);
				m_indexInCollectionTableForSubReports = new Dictionary<Hashtable, int>(HashtableKeyComparer.Instance);
				m_indexInCollectionTable = new Dictionary<Hashtable, int>(HashtableKeyComparer.Instance);
			}
			else
			{
				m_reportScopeDatasets.Add(dataRegion.Name, GetDataSet());
			}
			m_currentScope = CreateScopeInfo(dataRegion, m_currentScope == null || m_currentScope.AllowCustomAggregates);
			if (m_axisGroupingScopesForRunningValues != null)
			{
				m_axisGroupingScopesForRunningValues = new AxisGroupingScopesForRunningValues(m_axisGroupingScopesForRunningValues);
			}
			else
			{
				m_axisGroupingScopesForRunningValues = new AxisGroupingScopesForRunningValues();
			}
			if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) == 0)
			{
				m_outermostDataregionScope = m_currentScope;
			}
			m_dataregionScopes[dataRegion.Name] = m_currentScope;
		}

		private ScopeInfo CreateScopeInfo(DataRegion dataRegion, bool allowCustomAggregates)
		{
			return new ScopeInfo(allowCustomAggregates, dataRegion.Aggregates, dataRegion.PostSortAggregates, dataRegion);
		}

		private void UnRegisterDataRegionScope(DataRegion dataRegion)
		{
			string name = dataRegion.Name;
			Global.Tracer.Assert(name != null);
			m_currentDataRegionName = null;
			m_dataregionScopesForRunningValues.Remove(name);
			m_currentScope = null;
			if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) == 0)
			{
				m_outermostDataregionScope = null;
			}
			m_dataregionScopes.Remove(name);
			m_axisGroupingScopesForRunningValues = null;
		}

		internal void RegisterGroupingScope(ReportHierarchyNode member)
		{
			RegisterGroupingScope(member, forTablixCell: false);
		}

		private void RegisterGroupingScope(ReportHierarchyNode member, bool forTablixCell)
		{
			if (forTablixCell)
			{
				Global.Tracer.Assert(m_groupingScopesForRunningValuesInTablix != null, "(null != m_groupingScopesForRunningValuesInTablix)");
				Global.Tracer.Assert(m_groupingScopesForRunningValues != null, "(null != m_groupingScopesForRunningValues)");
			}
			DataSet dataSet = member.DataScopeInfo.DataSet;
			if (m_activeDataSets.First != dataSet)
			{
				RegisterDataSet(dataSet);
			}
			Grouping grouping = member.Grouping;
			m_outerGroupName = m_currentGroupName;
			m_currentGroupName = grouping.Name;
			string name = grouping.Name;
			m_groupingScopesForRunningValues[name] = null;
			if (member.IsColumn)
			{
				if (forTablixCell)
				{
					m_groupingScopesForRunningValuesInTablix.RegisterColumnGrouping(grouping);
				}
				m_axisGroupingScopesForRunningValues.RegisterColumnGrouping(grouping);
			}
			else
			{
				if (forTablixCell)
				{
					m_groupingScopesForRunningValuesInTablix.RegisterRowGrouping(grouping);
				}
				m_axisGroupingScopesForRunningValues.RegisterRowGrouping(grouping);
			}
			m_currentScope = CreateScopeInfo(member);
			m_groupingScopes[name] = m_currentScope;
			if (forTablixCell && m_initializingUserSorts && !m_reportScopeDatasets.ContainsKey(name))
			{
				m_reportScopeDatasets.Add(name, GetDataSet());
			}
			RegisterRunningValues(member.RunningValues, member.DataScopeInfo.RunningValuesOfAggregates);
		}

		private ScopeInfo CreateScopeInfo(ReportHierarchyNode member)
		{
			return CreateScopeInfo(member, (m_currentScope == null) ? member.Grouping.SimpleGroupExpressions : (member.Grouping.SimpleGroupExpressions && m_currentScope.AllowCustomAggregates));
		}

		private ScopeInfo CreateScopeInfo(ReportHierarchyNode member, bool allowCustomAggregates)
		{
			return new ScopeInfo(allowCustomAggregates, member.Grouping.Aggregates, member.Grouping.PostSortAggregates, member.Grouping.RecursiveAggregates, member.Grouping, member);
		}

		internal void UnRegisterGroupingScope(ReportHierarchyNode member)
		{
			UnRegisterGroupingScope(member, forTablixCell: false);
		}

		private void UnRegisterGroupingScope(ReportHierarchyNode member, bool forTablixCell)
		{
			if (forTablixCell)
			{
				Global.Tracer.Assert(m_groupingScopesForRunningValuesInTablix != null, "(null != m_groupingScopesForRunningValuesInTablix)");
				Global.Tracer.Assert(m_groupingScopesForRunningValues != null, "(null != m_groupingScopesForRunningValues)");
			}
			Grouping grouping = member.Grouping;
			string name = grouping.Name;
			m_outerGroupName = null;
			m_currentGroupName = null;
			m_groupingScopesForRunningValues.Remove(name);
			if (member.IsColumn)
			{
				if (forTablixCell)
				{
					m_groupingScopesForRunningValuesInTablix.UnRegisterColumnGrouping(name);
				}
				m_axisGroupingScopesForRunningValues.UnregisterColumnGrouping(grouping);
			}
			else
			{
				if (forTablixCell)
				{
					m_groupingScopesForRunningValuesInTablix.UnRegisterRowGrouping(name);
				}
				m_axisGroupingScopesForRunningValues.UnregisterRowGrouping(grouping);
			}
			m_currentScope = null;
			m_groupingScopes.Remove(name);
			UnRegisterRunningValues(member.RunningValues, member.DataScopeInfo.RunningValuesOfAggregates);
			DataSet dataSet = member.DataScopeInfo.DataSet;
			UnRegisterDataSet(dataSet);
		}

		internal void ValidateHideDuplicateScope(string hideDuplicateScope, ReportItem reportItem)
		{
			if (hideDuplicateScope == null)
			{
				return;
			}
			bool flag = true;
			ScopeInfo scopeInfo = null;
			if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDetail) == 0 && hideDuplicateScope.Equals(m_currentGroupName))
			{
				flag = false;
			}
			else if (m_groupingScopes.Contains(hideDuplicateScope))
			{
				scopeInfo = (ScopeInfo)m_groupingScopes[hideDuplicateScope];
			}
			else if (!m_datasetScopes.ContainsKey(hideDuplicateScope))
			{
				flag = false;
			}
			if (flag)
			{
				if (scopeInfo != null)
				{
					Global.Tracer.Assert(scopeInfo.GroupingScope != null, "(null != scope.GroupingScope)");
					scopeInfo.GroupingScope.AddReportItemWithHideDuplicates(reportItem);
				}
			}
			else
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidHideDuplicateScope, Severity.Error, m_objectType, m_objectName, "HideDuplicates", hideDuplicateScope);
			}
		}

		internal void RegisterGroupingScopeForDataRegionCell(ReportHierarchyNode member)
		{
			RegisterGroupingScope(member, forTablixCell: true);
		}

		internal void UnRegisterGroupingScopeForDataRegionCell(ReportHierarchyNode member)
		{
			UnRegisterGroupingScope(member, forTablixCell: true);
		}

		internal void RegisterIndividualCellScope(Cell cell)
		{
			DataSet dataSet = cell.DataScopeInfo.DataSet;
			RegisterDataSet(dataSet);
			m_currentScope = new ScopeInfo(m_currentScope == null || m_currentScope.AllowCustomAggregates, cell.Aggregates, cell.PostSortAggregates, cell);
			m_runningValues = cell.RunningValues;
			m_runningValuesOfAggregates = cell.DataScopeInfo.RunningValuesOfAggregates;
			IRIFDataScope canonicalCellScope = m_scopeTree.GetCanonicalCellScope(cell);
			if (canonicalCellScope != null && cell != canonicalCellScope)
			{
				cell.DataScopeInfo.ScopeID = canonicalCellScope.DataScopeInfo.ScopeID;
			}
		}

		internal void UnRegisterIndividualCellScope(Cell cell)
		{
			UnRegisterCell(cell);
		}

		private void UnRegisterCell(Cell cell)
		{
			UnRegisterNonScopeCell(cell);
		}

		internal void RegisterNonScopeCell(Cell cell)
		{
			DataSet dataSet = cell.DataScopeInfo.DataSet;
			RegisterDataSet(dataSet);
		}

		internal void UnRegisterNonScopeCell(Cell cell)
		{
			DataSet dataSet = cell.DataScopeInfo.DataSet;
			UnRegisterDataSet(dataSet);
		}

		internal void FoundAtomicScope(IRIFDataScope scope)
		{
			MarkChildScopesWithAtomicParent(scope);
		}

		private void MarkChildScopesWithAtomicParent(IRIFDataScope scope)
		{
			foreach (IRIFDataScope childScope in m_scopeTree.GetChildScopes(scope))
			{
				if (childScope.DataScopeInfo.IsDecomposable)
				{
					childScope.DataScopeInfo.IsDecomposable = false;
					MarkChildScopesWithAtomicParent(childScope);
				}
			}
		}

		internal bool HasMultiplePeerChildScopes(IRIFDataScope scope)
		{
			int[] dataSetGroupBindingCounts = new int[m_report.MappingDataSetIndexToDataSet.Count];
			foreach (IRIFDataScope childScope in m_scopeTree.GetChildScopes(scope))
			{
				if (IsOrHasSecondGroupBoundToDataSet(childScope, dataSetGroupBindingCounts))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsOrHasSecondGroupBoundToDataSet(IRIFDataScope scope, int[] dataSetGroupBindingCounts)
		{
			if (scope is ReportHierarchyNode)
			{
				DataSet dataSet = scope.DataScopeInfo.DataSet;
				if (dataSet == null)
				{
					return false;
				}
				int num = dataSetGroupBindingCounts[dataSet.IndexInCollection];
				num++;
				dataSetGroupBindingCounts[dataSet.IndexInCollection] = num;
				return num >= 2;
			}
			foreach (IRIFDataScope childScope in m_scopeTree.GetChildScopes(scope))
			{
				if (IsOrHasSecondGroupBoundToDataSet(childScope, dataSetGroupBindingCounts))
				{
					return true;
				}
			}
			return false;
		}

		internal bool EvaluateAtomicityCondition(bool isAtomic, IRIFDataScope scope, AtomicityReason reason)
		{
			if (isAtomic && m_publishingContext.TraceAtomicScopes)
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "Report {0} contains a scope '{1}' which uses or contains {2}.  This may prevent optimizations from being applied to parent or child scopes.", m_reportContext.ItemPathAsString.MarkAsPrivate(), m_scopeTree.GetScopeName(scope), reason.ToString());
			}
			return isAtomic;
		}

		internal bool IsAncestor(ReportHierarchyNode child, string parentName)
		{
			if (!m_reportScopes.TryGetValue(parentName, out ISortFilterScope value))
			{
				return false;
			}
			IRIFDataScope iRIFDataScope = (!(value is Grouping)) ? ((IRIFDataScope)value) : ((Grouping)value).Owner;
			if (iRIFDataScope != child)
			{
				return m_scopeTree.IsSameOrParentScope(iRIFDataScope, child);
			}
			return false;
		}

		internal DataRegion RegisterDataRegionCellScope(DataRegion dataRegion, bool forceRows, List<DataAggregateInfo> scopeAggregates, List<DataAggregateInfo> scopePostSortAggregates)
		{
			Global.Tracer.Assert(scopeAggregates != null, "(null != scopeAggregates)");
			Global.Tracer.Assert(scopePostSortAggregates != null, "(null != scopePostSortAggregates)");
			DataRegion result = null;
			if (m_groupingScopesForRunningValuesInTablix == null)
			{
				m_groupingScopesForRunningValuesInTablix = new GroupingScopesForTablix(forceRows, m_objectType, dataRegion);
			}
			else
			{
				dataRegion.ScopeChainInfo = m_groupingScopesForRunningValuesInTablix.GetScopeChainInfo();
				result = m_groupingScopesForRunningValuesInTablix.SetContainerScope(dataRegion);
			}
			m_currentScope = new ScopeInfo(m_currentScope == null || m_currentScope.AllowCustomAggregates, scopeAggregates, scopePostSortAggregates, dataRegion);
			return result;
		}

		internal void UnRegisterTablixCellScope(DataRegion dataRegion)
		{
			Global.Tracer.Assert(m_groupingScopesForRunningValuesInTablix != null, "(null != m_groupingScopesForRunningValuesInTablix)");
			if (dataRegion == null)
			{
				m_groupingScopesForRunningValuesInTablix = null;
			}
			else
			{
				m_groupingScopesForRunningValuesInTablix.SetContainerScope(dataRegion);
			}
		}

		internal void ResetMemberAndCellIndexInCollectionTable()
		{
			m_indexInCollectionTable.Clear();
		}

		internal void SetIndexInCollection(IIndexedInCollection indexedInCollection)
		{
			Dictionary<Hashtable, int> dictionary;
			switch (indexedInCollection.IndexedInCollectionType)
			{
			case IndexedInCollectionType.DataRegion:
				dictionary = m_indexInCollectionTableForDataRegions;
				break;
			case IndexedInCollectionType.SubReport:
				dictionary = m_indexInCollectionTableForSubReports;
				break;
			default:
				dictionary = m_indexInCollectionTable;
				break;
			}
			Hashtable key = (Hashtable)m_groupingScopes.Clone();
			if (dictionary.TryGetValue(key, out int value))
			{
				value = (dictionary[key] = value + 1);
			}
			else
			{
				value = 0;
				dictionary.Add(key, value);
			}
			indexedInCollection.IndexInCollection = value;
		}

		internal void RegisterPageSectionScope(Page rifPage, List<DataAggregateInfo> scopeAggregates)
		{
			Global.Tracer.Assert(scopeAggregates != null, "(null != scopeAggregates)");
			m_currentScope = new ScopeInfo(allowCustomAggregates: false, scopeAggregates, rifPage);
		}

		internal void UnRegisterPageSectionScope()
		{
			m_currentScope = null;
		}

		internal void RegisterRunningValues(List<RunningValueInfo> runningValues, List<RunningValueInfo> runningValuesOfAggregates)
		{
			Global.Tracer.Assert(runningValues != null, "(runningValues != null)");
			Global.Tracer.Assert(runningValuesOfAggregates != null, "(runningValuesOfAggregates != null)");
			m_runningValues = runningValues;
			m_runningValuesOfAggregates = runningValuesOfAggregates;
		}

		internal void RegisterRunningValues(string groupName, List<RunningValueInfo> runningValues, List<RunningValueInfo> runningValuesOfAggregates)
		{
			Global.Tracer.Assert(runningValues != null, "(runningValues != null)");
			Global.Tracer.Assert(runningValuesOfAggregates != null, "(runningValuesOfAggregates != null)");
			m_groupingScopesForRunningValues[groupName] = null;
			m_runningValues = runningValues;
			m_runningValuesOfAggregates = runningValuesOfAggregates;
		}

		internal void UnRegisterRunningValues(List<RunningValueInfo> runningValues, List<RunningValueInfo> runningValuesOfAggregates)
		{
			Global.Tracer.Assert(runningValues != null, "(runningValues != null)");
			Global.Tracer.Assert(runningValuesOfAggregates != null, "(runningValuesOfAggregates != null)");
			Global.Tracer.Assert(m_runningValues == runningValues);
			Global.Tracer.Assert(m_runningValuesOfAggregates == runningValuesOfAggregates);
			m_runningValues = null;
			m_runningValuesOfAggregates = null;
		}

		internal void TransferGroupExpressionRowNumbers(List<RunningValueInfo> rowNumbers)
		{
			if (rowNumbers == null)
			{
				return;
			}
			for (int num = rowNumbers.Count - 1; num >= 0; num--)
			{
				Global.Tracer.Assert((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InGrouping) != 0, "(0 != (m_location & LocationFlags.InGrouping))");
				RunningValueInfo runningValueInfo = rowNumbers[num];
				Global.Tracer.Assert(runningValueInfo != null, "(null != rowNumber)");
				string scope = runningValueInfo.Scope;
				bool flag = true;
				ScopeInfo scopeInfo = null;
				if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDynamicTablixCell) != 0)
				{
					flag = false;
				}
				else if (scope == null)
				{
					if (m_outerGroupName != null)
					{
						flag = false;
					}
					else
					{
						scopeInfo = m_outermostDataregionScope;
					}
				}
				else if (m_outerGroupName == scope)
				{
					Global.Tracer.Assert(m_outerGroupName != null, "(null != m_outerGroupName)");
					scopeInfo = (ScopeInfo)m_groupingScopes[m_outerGroupName];
				}
				else if (m_currentDataRegionName == scope)
				{
					Global.Tracer.Assert(m_currentDataRegionName != null, "(null != m_currentDataRegionName)");
					scopeInfo = (ScopeInfo)m_dataregionScopes[m_currentDataRegionName];
				}
				else
				{
					flag = false;
				}
				if (!flag)
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidGroupExpressionScope, Severity.Error, m_objectType, m_objectName, "GroupExpression");
				}
				else if (scopeInfo != null)
				{
					Global.Tracer.Assert(scopeInfo.Aggregates != null, "(null != destinationScope.Aggregates)");
					scopeInfo.Aggregates.Add(runningValueInfo);
				}
				rowNumbers.RemoveAt(num);
			}
		}

		internal void TransferRunningValues(List<RunningValueInfo> runningValues, string propertyName)
		{
			TransferRunningValues(runningValues, m_objectType, m_objectName, propertyName);
		}

		internal void TransferRunningValues(List<RunningValueInfo> runningValues, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (runningValues == null || (m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
			{
				return;
			}
			for (int num = runningValues.Count - 1; num >= 0; num--)
			{
				RunningValueInfo runningValueInfo = runningValues[num];
				Global.Tracer.Assert(runningValueInfo != null, "(null != runningValue)");
				bool flag = runningValueInfo.AggregateType == DataAggregateInfo.AggregateTypes.Previous;
				string scope = runningValueInfo.Scope;
				bool flag2 = true;
				string text = null;
				ScopeInfo scopeInfo = null;
				List<DataAggregateInfo> list = null;
				List<RunningValueInfo> list2 = null;
				if (scope == null)
				{
					if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) == 0)
					{
						flag2 = false;
					}
					else if (flag && (m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegionCellTopLevelItem) != 0)
					{
						if (runningValueInfo.HasDirectFieldReferences)
						{
							if (m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeIsDetail && m_groupingScopesForRunningValuesInTablix.CurrentRowScopeIsDetail)
							{
								text = GetDataSetName();
								list2 = GetActiveRunningValuesCollection(runningValueInfo.IsAggregateOfAggregate, flag);
								runningValueInfo.IsScopedInEvaluationScope = true;
							}
							else
							{
								flag2 = false;
							}
						}
						else
						{
							text = GetDataSetName();
							list2 = GetActiveRunningValuesCollection(runningValueInfo.IsAggregateOfAggregate, flag);
							if (m_groupingScopesForRunningValuesInTablix.IsRunningValueDirectionColumn && m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName != null)
							{
								runningValueInfo.Scope = m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName;
								if (m_groupingScopesForRunningValuesInTablix.CurrentRowScopeIsDetail)
								{
									runningValueInfo.IsScopedInEvaluationScope = true;
								}
							}
							else if (m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName != null)
							{
								runningValueInfo.Scope = m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName;
								if (m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeIsDetail)
								{
									runningValueInfo.IsScopedInEvaluationScope = true;
								}
							}
						}
					}
					else if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InGrouping) != 0 || (m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDetail) != 0)
					{
						text = GetDataSetName();
						list2 = GetActiveRunningValuesCollection(runningValueInfo.IsAggregateOfAggregate, flag);
						if (flag)
						{
							runningValueInfo.Scope = m_currentGroupName;
							runningValueInfo.IsScopedInEvaluationScope = true;
						}
					}
					else
					{
						text = GetDataSetName();
						if (text != null)
						{
							scopeInfo = (ScopeInfo)m_datasetScopes[text];
							Global.Tracer.Assert(scopeInfo != null, "(null != destinationScope)");
							list = scopeInfo.Aggregates;
						}
					}
				}
				else if (m_groupingScopesForRunningValuesInTablix != null && m_groupingScopesForRunningValuesInTablix.ContainsScope(scope, m_errorContext, checkConflictingScope: true, m_groupingScopes))
				{
					text = GetDataSetName();
					list2 = GetActiveRunningValuesCollection(runningValueInfo.IsAggregateOfAggregate, flag);
					Global.Tracer.Assert((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InTablixCell) != 0 || (m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegionCellTopLevelItem) != 0);
					if (flag)
					{
						if (runningValueInfo.HasDirectFieldReferences && (!m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeIsDetail || !m_groupingScopesForRunningValuesInTablix.CurrentRowScopeIsDetail))
						{
							flag2 = false;
						}
						else if (((ScopeInfo)m_groupingScopes[scope]).GroupingScope.Owner.IsColumn)
						{
							if (m_groupingScopesForRunningValuesInTablix.CurrentRowScopeIsDetail && scope == m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName)
							{
								runningValueInfo.IsScopedInEvaluationScope = true;
							}
						}
						else if (m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeIsDetail && scope == m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName)
						{
							runningValueInfo.IsScopedInEvaluationScope = true;
						}
					}
				}
				else if (m_groupingScopesForRunningValues.ContainsKey(scope))
				{
					Global.Tracer.Assert((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InGrouping) != 0, "(0 != (m_location & LocationFlags.InGrouping))");
					if (flag && runningValueInfo.HasDirectFieldReferences && m_groupingScopesForRunningValuesInTablix != null && (!m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeIsDetail || !m_groupingScopesForRunningValuesInTablix.CurrentRowScopeIsDetail))
					{
						flag2 = false;
					}
					else if (m_groupingScopesForRunningValuesInTablix == null || m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeIsDetail || m_groupingScopesForRunningValuesInTablix.CurrentRowScopeIsDetail)
					{
						text = GetDataSetName();
						list2 = GetActiveRunningValuesCollection(runningValueInfo.IsAggregateOfAggregate, flag);
					}
					else
					{
						flag2 = false;
					}
				}
				else if (m_dataregionScopesForRunningValues.ContainsKey(scope))
				{
					Global.Tracer.Assert((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) != 0, "(0 != (m_location & LocationFlags.InDataRegion))");
					runningValueInfo.Scope = (string)m_dataregionScopesForRunningValues[scope];
					if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InGrouping) != 0 || (m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDetail) != 0 || flag)
					{
						text = GetDataSetName();
						list2 = GetActiveRunningValuesCollection(runningValueInfo.IsAggregateOfAggregate, flag);
						if (flag && runningValueInfo.Scope == null)
						{
							runningValueInfo = null;
							list2 = null;
						}
					}
					else
					{
						text = GetDataSetName();
						if (text != null)
						{
							scopeInfo = (ScopeInfo)m_datasetScopes[text];
							Global.Tracer.Assert(scopeInfo != null, "(null != destinationScope)");
							list = scopeInfo.Aggregates;
						}
					}
				}
				else if (m_datasetScopes.ContainsKey(scope))
				{
					if (flag)
					{
						runningValueInfo = null;
						list2 = null;
					}
					else if (((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InGrouping) != 0 || (m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDetail) != 0) && scope == GetDataSetName())
					{
						if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InTablixCell) != 0 && IsDataRegionCellScope)
						{
							flag2 = false;
						}
						else
						{
							text = scope;
							runningValueInfo.Scope = null;
							list2 = GetActiveRunningValuesCollection(runningValueInfo.IsAggregateOfAggregate, flag);
							runningValueInfo.IsScopedInEvaluationScope = true;
						}
					}
					else
					{
						text = scope;
						scopeInfo = (ScopeInfo)m_datasetScopes[scope];
						Global.Tracer.Assert(scopeInfo != null, "(null != destinationScope)");
						list = scopeInfo.Aggregates;
					}
				}
				else
				{
					flag2 = false;
				}
				if (!flag2)
				{
					if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDynamicTablixCell) != 0)
					{
						if (DataAggregateInfo.AggregateTypes.Previous == runningValueInfo.AggregateType)
						{
							m_errorContext.Register(ProcessingErrorCode.rsInvalidPreviousAggregateInTablixCell, Severity.Error, objectType, objectName, propertyName, m_tablixName);
						}
						else
						{
							m_errorContext.Register(ProcessingErrorCode.rsInvalidScopeInTablix, Severity.Error, objectType, objectName, propertyName, m_tablixName);
						}
					}
					else
					{
						m_errorContext.Register(ProcessingErrorCode.rsInvalidAggregateScope, Severity.Error, objectType, objectName, propertyName);
					}
				}
				else if (runningValueInfo != null)
				{
					if (scopeInfo == null)
					{
						scopeInfo = (ScopeInfo)m_datasetScopes[GetCurrentDataSetName()];
					}
					runningValueInfo.DataSetIndexInCollection = scopeInfo.DataSetScope.IndexInCollection;
					RegisterDataSetLevelAggregateOrLookup(runningValueInfo.DataSetIndexInCollection);
					runningValueInfo.Initialize(this, text, objectType, objectName, propertyName);
					if (list != null)
					{
						list.Add(runningValueInfo);
					}
					else if (list2 != null)
					{
						Global.Tracer.Assert(runningValues != list2);
						if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) != 0 && !m_isDataRegionScopedCell)
						{
							string text2 = "";
							if (m_axisGroupingScopesForRunningValues.InCurrentDataRegionDynamicRow)
							{
								text2 = m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName;
							}
							if (m_axisGroupingScopesForRunningValues.InCurrentDataRegionDynamicColumn)
							{
								text2 = text2 + "." + m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName;
							}
							runningValueInfo.EvaluationScopeName = text2;
						}
						else if (m_currentScope.DataRegionScope != null)
						{
							runningValueInfo.EvaluationScopeName = m_currentScope.DataRegionScope.Name;
						}
						list2.Add(runningValueInfo);
						if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) != 0 && (!flag || runningValueInfo.HasDirectFieldReferences) && (m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) != 0 && !m_isDataRegionScopedCell)
						{
							if (m_axisGroupingScopesForRunningValues.InCurrentDataRegionDynamicRow)
							{
								((ScopeInfo)m_groupingScopes[m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName]).ReportScope.NeedToCacheDataRows = true;
							}
							if (m_axisGroupingScopesForRunningValues.InCurrentDataRegionDynamicColumn)
							{
								((ScopeInfo)m_groupingScopes[m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName]).ReportScope.NeedToCacheDataRows = true;
							}
							if (m_currentScope.ReportScope != null)
							{
								m_currentScope.ReportScope.NeedToCacheDataRows = true;
							}
						}
					}
					StoreAggregateScopeAndLocationInfo(runningValueInfo, m_currentScope, objectType, objectName, propertyName);
				}
				runningValues.RemoveAt(num);
			}
		}

		private List<RunningValueInfo> GetActiveRunningValuesCollection(bool isAggregateOfAggregates, bool isPrevious)
		{
			if (!isPrevious && isAggregateOfAggregates)
			{
				return m_runningValuesOfAggregates;
			}
			return m_runningValues;
		}

		private void StoreAggregateScopeAndLocationInfo(DataAggregateInfo aggregate, ScopeInfo destinationScope, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (destinationScope != null)
			{
				aggregate.EvaluationScope = destinationScope.DataScope;
			}
			aggregate.PublishingInfo.ObjectType = objectType;
			aggregate.PublishingInfo.ObjectName = objectName;
			aggregate.PublishingInfo.PropertyName = propertyName;
		}

		internal void SpecialTransferRunningValues(List<RunningValueInfo> runningValues, List<RunningValueInfo> runningValuesOfAggregates)
		{
			TransferRunningValueCollection(runningValues, m_runningValues);
			TransferRunningValueCollection(runningValuesOfAggregates, m_runningValuesOfAggregates);
		}

		private void TransferRunningValueCollection(List<RunningValueInfo> runningValues, List<RunningValueInfo> destRunningValues)
		{
			if (runningValues != null)
			{
				Global.Tracer.Assert(destRunningValues != null, "(null != m_runningValues)");
				for (int num = runningValues.Count - 1; num >= 0; num--)
				{
					Global.Tracer.Assert(runningValues != destRunningValues);
					destRunningValues.Add(runningValues[num]);
					runningValues.RemoveAt(num);
				}
			}
		}

		internal void TransferLookups(List<LookupInfo> lookups, string propertyName)
		{
			TransferLookups(lookups, m_objectType, m_objectName, propertyName, GetDataSetName());
		}

		internal void TransferLookups(List<LookupInfo> lookups, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, string dataSetName)
		{
			if (lookups == null)
			{
				return;
			}
			for (int num = lookups.Count - 1; num >= 0; num--)
			{
				LookupInfo lookupInfo = lookups[num];
				Global.Tracer.Assert(lookupInfo != null, "(null != lookup)");
				LookupDestinationInfo destinationInfo = lookupInfo.DestinationInfo;
				Global.Tracer.Assert(destinationInfo != null, "(null != destinationInfo)");
				string scope = destinationInfo.Scope;
				if (string.IsNullOrEmpty(scope) || !m_datasetScopes.ContainsKey(scope))
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidLookupScope, Severity.Error, objectType, objectName, propertyName);
				}
				else
				{
					lookupInfo.Initialize(this, scope, objectType, objectName, propertyName);
					DataSet dataSetScope = ((ScopeInfo)m_datasetScopes[scope]).DataSetScope;
					if (!m_lookupCompactionTable.TryGetValue(scope, out LookupDestinationCompactionTable value))
					{
						value = new LookupDestinationCompactionTable();
						m_lookupCompactionTable[scope] = value;
					}
					destinationInfo.UsedInSameDataSetTablixProcessing = string.Equals(scope, dataSetName, StringComparison.Ordinal);
					string originalText = destinationInfo.DestinationExpr.OriginalText;
					if (!value.TryGetValue(originalText, out int value2))
					{
						if (dataSetScope.LookupDestinationInfos == null)
						{
							dataSetScope.LookupDestinationInfos = new List<LookupDestinationInfo>();
						}
						destinationInfo.Initialize(this, scope, objectType, objectName, propertyName);
						value2 = (destinationInfo.IndexInCollection = dataSetScope.LookupDestinationInfos.Count);
						dataSetScope.LookupDestinationInfos.Add(destinationInfo);
						value[originalText] = value2;
					}
					else
					{
						LookupDestinationInfo lookupDestinationInfo = dataSetScope.LookupDestinationInfos[value2];
						lookupDestinationInfo.IsMultiValue |= destinationInfo.IsMultiValue;
						lookupDestinationInfo.UsedInSameDataSetTablixProcessing |= destinationInfo.UsedInSameDataSetTablixProcessing;
					}
					lookupInfo.DestinationIndexInCollection = value2;
					lookupInfo.DestinationInfo = null;
					lookupInfo.DataSetIndexInCollection = dataSetScope.IndexInCollection;
					if (dataSetScope.Lookups == null)
					{
						dataSetScope.Lookups = new List<LookupInfo>();
					}
					dataSetScope.Lookups.Add(lookupInfo);
					RegisterDataSetLevelAggregateOrLookup(dataSetScope.IndexInCollection);
				}
				lookups.RemoveAt(num);
			}
		}

		internal void TransferAggregates(List<DataAggregateInfo> aggregates, string propertyName)
		{
			TransferAggregates(aggregates, m_objectType, m_objectName, propertyName, isInAggregate: false);
		}

		internal void TransferNestedAggregates(List<DataAggregateInfo> aggregates, string propertyName)
		{
			TransferAggregates(aggregates, m_objectType, m_objectName, propertyName, isInAggregate: true);
		}

		private void TransferAggregates(List<DataAggregateInfo> aggregates, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, bool isInAggregate)
		{
			if (aggregates == null)
			{
				return;
			}
			for (int num = aggregates.Count - 1; num >= 0; num--)
			{
				DataAggregateInfo dataAggregateInfo = aggregates[num];
				Global.Tracer.Assert(dataAggregateInfo != null, "(null != aggregate)");
				if (m_hasFilters && DataAggregateInfo.AggregateTypes.Aggregate == dataAggregateInfo.AggregateType)
				{
					m_errorContext.Register(ProcessingErrorCode.rsCustomAggregateAndFilter, Severity.Error, objectType, objectName, propertyName);
				}
				string scope;
				bool scope2 = dataAggregateInfo.GetScope(out scope);
				bool flag = true;
				string text = null;
				ScopeInfo scopeInfo = null;
				bool flag2 = false;
				if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) == 0 && m_numberOfDataSets == 0)
				{
					flag = false;
					flag2 = true;
				}
				else if (!scope2)
				{
					text = GetDataSetName();
					if (Microsoft.ReportingServices.ReportPublishing.LocationFlags.None == m_location)
					{
						if (1 != m_numberOfDataSets)
						{
							flag = false;
							m_errorContext.Register(ProcessingErrorCode.rsMissingAggregateScope, Severity.Error, objectType, objectName, propertyName);
						}
						else if (text != null)
						{
							scopeInfo = (ScopeInfo)m_datasetScopes[text];
						}
					}
					else if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
					{
						if (dataAggregateInfo.Expressions != null && dataAggregateInfo.Expressions.Length != 0)
						{
							ExpressionInfo expressionInfo = dataAggregateInfo.Expressions[0];
							Global.Tracer.Assert(expressionInfo != null, "(null != paramExpr)");
							if (expressionInfo.HasAnyFieldReferences)
							{
								flag = false;
								m_errorContext.Register(ProcessingErrorCode.rsMissingAggregateScopeInPageSection, Severity.Error, objectType, objectName, propertyName);
							}
							else
							{
								scopeInfo = m_currentScope;
							}
						}
					}
					else
					{
						Global.Tracer.Assert((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataSet) != 0);
						scopeInfo = m_currentScope;
					}
					if (scopeInfo != null && scopeInfo.DataSetScope != null)
					{
						scopeInfo.DataSetScope.UsedInAggregates = true;
						dataAggregateInfo.DataSetIndexInCollection = scopeInfo.DataSetScope.IndexInCollection;
						RegisterDataSetLevelAggregateOrLookup(dataAggregateInfo.DataSetIndexInCollection);
					}
				}
				else if (scope == null)
				{
					flag = false;
				}
				else if (m_groupingScopes.ContainsKey(scope))
				{
					Global.Tracer.Assert((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InGrouping) != 0, "(0 != (m_location & LocationFlags.InGrouping))");
					text = GetDataSetName();
					scopeInfo = (ScopeInfo)m_groupingScopes[scope];
				}
				else if (m_dataregionScopes.ContainsKey(scope))
				{
					Global.Tracer.Assert((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) != 0, "(0 != (m_location & LocationFlags.InDataRegion))");
					text = GetDataSetName();
					scopeInfo = (ScopeInfo)m_dataregionScopes[scope];
				}
				else if (m_datasetScopes.ContainsKey(scope))
				{
					if (isInAggregate)
					{
						flag = false;
						m_errorContext.Register(ProcessingErrorCode.rsInvalidNestedDataSetAggregate, Severity.Error, objectType, objectName, propertyName);
					}
					else if (dataAggregateInfo.IsAggregateOfAggregate)
					{
						flag = false;
						m_errorContext.Register(ProcessingErrorCode.rsDataSetAggregateOfAggregates, Severity.Error, objectType, objectName, propertyName);
					}
					if (flag && (m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0 && dataAggregateInfo.Expressions != null && dataAggregateInfo.Expressions.Length != 0)
					{
						ExpressionInfo expressionInfo2 = dataAggregateInfo.Expressions[0];
						Global.Tracer.Assert(expressionInfo2 != null, "(null != paramExpr)");
						List<string> referencedReportItems = expressionInfo2.ReferencedReportItems;
						if (referencedReportItems != null && referencedReportItems.Count > 0)
						{
							flag = false;
							m_errorContext.Register(ProcessingErrorCode.rsReportItemInScopedAggregate, Severity.Error, objectType, objectName, propertyName, referencedReportItems[0]);
						}
						else if (expressionInfo2.ReferencedOverallPageGlobals)
						{
							flag = false;
							m_errorContext.Register(ProcessingErrorCode.rsOverallPageNumberInScopedAggregate, Severity.Error, objectType, objectName, propertyName);
						}
						else if (expressionInfo2.ReferencedPageGlobals)
						{
							flag = false;
							m_errorContext.Register(ProcessingErrorCode.rsPageNumberInScopedAggregate, Severity.Error, objectType, objectName, propertyName);
						}
					}
					if (flag)
					{
						text = scope;
						scopeInfo = (ScopeInfo)m_datasetScopes[scope];
						if (!scopeInfo.IsDuplicateScope)
						{
							scopeInfo.DataSetScope.UsedInAggregates = true;
							dataAggregateInfo.DataSetIndexInCollection = scopeInfo.DataSetScope.IndexInCollection;
							RegisterDataSetLevelAggregateOrLookup(dataAggregateInfo.DataSetIndexInCollection);
						}
					}
				}
				else if (isInAggregate)
				{
					if (!m_reportScopes.TryGetValue(scope, out ISortFilterScope value))
					{
						flag = false;
						flag2 = true;
					}
					else if (value is Grouping)
					{
						ReportHierarchyNode owner = ((Grouping)value).Owner;
						text = GetDataSetName();
						scopeInfo = CreateScopeInfo(owner, allowCustomAggregates: false);
					}
					else if (value is DataRegion)
					{
						text = GetDataSetName();
						scopeInfo = CreateScopeInfo((DataRegion)value, allowCustomAggregates: false);
					}
					else
					{
						flag = false;
						flag2 = true;
					}
				}
				else
				{
					flag = false;
					flag2 = true;
				}
				if (flag2)
				{
					ProcessingErrorCode code = (!isInAggregate) ? ProcessingErrorCode.rsInvalidAggregateScope : ProcessingErrorCode.rsInvalidNestedAggregateScope;
					m_errorContext.Register(code, Severity.Error, objectType, objectName, propertyName);
				}
				if (flag && scopeInfo != null)
				{
					if (scopeInfo.DataSetScope == null && dataAggregateInfo.DataSetIndexInCollection < 0)
					{
						dataAggregateInfo.DataSetIndexInCollection = GetCurrentDataSetIndex();
					}
					if (DataAggregateInfo.AggregateTypes.Aggregate == dataAggregateInfo.AggregateType && !scopeInfo.AllowCustomAggregates)
					{
						ProcessingErrorCode code2 = (!isInAggregate) ? ProcessingErrorCode.rsInvalidCustomAggregateScope : ProcessingErrorCode.rsNestedCustomAggregate;
						m_errorContext.Register(code2, Severity.Error, objectType, objectName, propertyName);
					}
					if (dataAggregateInfo.Expressions != null)
					{
						for (int i = 0; i < dataAggregateInfo.Expressions.Length; i++)
						{
							Global.Tracer.Assert(dataAggregateInfo.Expressions[i] != null, "(null != aggregate.Expressions[j])");
							dataAggregateInfo.Expressions[i].AggregateInitialize(text, objectType, objectName, propertyName, this);
						}
					}
					if (DataAggregateInfo.AggregateTypes.Aggregate == dataAggregateInfo.AggregateType)
					{
						DataSet dataSet = GetDataSet(text);
						if (dataSet != null)
						{
							dataSet.HasScopeWithCustomAggregates = true;
							if (dataSet.InterpretSubtotalsAsDetails == DataSet.TriState.Auto)
							{
								dataSet.InterpretSubtotalsAsDetails = DataSet.TriState.False;
							}
						}
					}
					List<DataAggregateInfo> list;
					if (dataAggregateInfo.Recursive)
					{
						list = ((scopeInfo.GroupingScope != null && scopeInfo.GroupingScope.Parent != null) ? scopeInfo.RecursiveAggregates : scopeInfo.Aggregates);
					}
					else if (dataAggregateInfo.IsAggregateOfAggregate && scopeInfo.DataScopeInfo != null)
					{
						AggregateBucket<DataAggregateInfo> aggregateBucket = (!dataAggregateInfo.IsPostSortAggregate()) ? scopeInfo.DataScopeInfo.AggregatesOfAggregates.GetOrCreateBucket(dataAggregateInfo.PublishingInfo.AggregateOfAggregatesLevel) : scopeInfo.DataScopeInfo.PostSortAggregatesOfAggregates.GetOrCreateBucket(0);
						list = aggregateBucket.Aggregates;
					}
					else if (scopeInfo.PostSortAggregates != null && dataAggregateInfo.IsPostSortAggregate())
					{
						list = scopeInfo.PostSortAggregates;
						if (scopeInfo.ReportScope != null)
						{
							scopeInfo.ReportScope.NeedToCacheDataRows = true;
						}
						if (m_groupingScopesForRunningValuesInTablix != null)
						{
							if (m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName != null)
							{
								((ScopeInfo)m_groupingScopes[m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName]).ReportScope.NeedToCacheDataRows = true;
							}
							if (m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName != null)
							{
								((ScopeInfo)m_groupingScopes[m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName]).ReportScope.NeedToCacheDataRows = true;
							}
						}
					}
					else
					{
						list = scopeInfo.Aggregates;
					}
					Global.Tracer.Assert(list != null, "(null != destinationAggregates)");
					Global.Tracer.Assert(aggregates != list);
					if (!scope2 && (m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) != 0)
					{
						if (!m_isDataRegionScopedCell)
						{
							string text2 = "";
							if (m_axisGroupingScopesForRunningValues.InCurrentDataRegionDynamicRow)
							{
								text2 = m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName;
							}
							if (m_axisGroupingScopesForRunningValues.InCurrentDataRegionDynamicColumn)
							{
								text2 = text2 + "." + m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName;
							}
							dataAggregateInfo.EvaluationScopeName = text2;
						}
						else if (m_currentScope.DataRegionScope != null)
						{
							dataAggregateInfo.EvaluationScopeName = m_currentScope.DataRegionScope.Name;
						}
					}
					list.Add(dataAggregateInfo);
					StoreAggregateScopeAndLocationInfo(dataAggregateInfo, scopeInfo, objectType, objectName, propertyName);
				}
				aggregates.RemoveAt(num);
			}
		}

		internal void RegisterReportSection(ReportSection sectionDef)
		{
			m_currentScope = new ScopeInfo(allowCustomAggregates: false, null, sectionDef);
			m_reportItemsInSection = new Dictionary<string, ReportItem>();
			m_referencableTextboxesInSection = new byte[m_referencableTextboxesInSection.Length];
		}

		internal void UnRegisterReportSection()
		{
			m_currentScope = null;
			m_reportItemsInSection = null;
		}

		internal void InitializeParameters(List<ParameterDef> parameters, List<DataSet> dataSetList)
		{
			if (m_dynamicParameters == null || m_dynamicParameters.Count == 0)
			{
				return;
			}
			Hashtable hashtable = new Hashtable();
			Microsoft.ReportingServices.ReportPublishing.DynamicParameter dynamicParameter = null;
			int i = 0;
			for (int j = 0; j < m_dynamicParameters.Count; j++)
			{
				for (dynamicParameter = (Microsoft.ReportingServices.ReportPublishing.DynamicParameter)m_dynamicParameters[j]; i < dynamicParameter.Index; i++)
				{
					hashtable.Add(parameters[i].Name, i);
				}
				InitializeParameter(parameters[dynamicParameter.Index], dynamicParameter, hashtable, dataSetList);
			}
		}

		private void InitializeParameter(ParameterDef parameter, Microsoft.ReportingServices.ReportPublishing.DynamicParameter dynamicParameter, Hashtable dependencies, List<DataSet> dataSetList)
		{
			Global.Tracer.Assert(dynamicParameter != null, "(null != dynamicParameter)");
			Microsoft.ReportingServices.ReportPublishing.DataSetReference dataSetReference = null;
			bool isComplex = dynamicParameter.IsComplex;
			dataSetReference = dynamicParameter.ValidValueDataSet;
			if (dataSetReference != null)
			{
				InitializeParameterDataSource(parameter, dataSetReference, isDefault: false, dependencies, ref isComplex, dataSetList);
			}
			dataSetReference = dynamicParameter.DefaultDataSet;
			if (dataSetReference != null)
			{
				InitializeParameterDataSource(parameter, dataSetReference, isDefault: true, dependencies, ref isComplex, dataSetList);
			}
		}

		private void InitializeParameterDataSource(ParameterDef parameter, Microsoft.ReportingServices.ReportPublishing.DataSetReference dataSetRef, bool isDefault, Hashtable dependencies, ref bool isComplex, List<DataSet> dataSetList)
		{
			ParameterDataSource parameterDataSource = null;
			PublishingDataSetInfo publishingDataSetInfo = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			publishingDataSetInfo = (PublishingDataSetInfo)m_dataSetQueryInfo[dataSetRef.DataSet];
			if (publishingDataSetInfo == null)
			{
				if (isDefault)
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidDefaultValueDataSetReference, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.ReportParameter, parameter.Name, "DataSetReference", dataSetRef.DataSet.MarkAsPrivate());
				}
				else
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidValidValuesDataSetReference, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.ReportParameter, parameter.Name, "DataSetReference", dataSetRef.DataSet.MarkAsPrivate());
				}
			}
			else
			{
				DataSet dataSet = dataSetList[publishingDataSetInfo.DataSetDefIndex];
				if (!dataSet.UsedInAggregates && !dataSet.HasLookups && !dataSet.UsedOnlyInParametersSet)
				{
					m_dataSetNameToDataRegionsMap.TryGetValue(dataSetRef.DataSet, out List<DataRegion> value);
					if (value == null || value.Count == 0)
					{
						dataSet.UsedOnlyInParameters = true;
					}
				}
				parameterDataSource = new ParameterDataSource(publishingDataSetInfo.DataSourceIndex, publishingDataSetInfo.DataSetIndex);
				Hashtable hashtable = (Hashtable)m_fieldNameMap[dataSetRef.DataSet];
				if (hashtable != null)
				{
					if (hashtable.ContainsKey(dataSetRef.ValueAlias))
					{
						parameterDataSource.ValueFieldIndex = (int)hashtable[dataSetRef.ValueAlias];
						if (parameterDataSource.ValueFieldIndex >= publishingDataSetInfo.CalculatedFieldIndex)
						{
							flag3 = true;
						}
						flag = true;
					}
					if (dataSetRef.LabelAlias != null)
					{
						if (hashtable.ContainsKey(dataSetRef.LabelAlias))
						{
							parameterDataSource.LabelFieldIndex = (int)hashtable[dataSetRef.LabelAlias];
							if (parameterDataSource.LabelFieldIndex >= publishingDataSetInfo.CalculatedFieldIndex)
							{
								flag3 = true;
							}
							flag2 = true;
						}
					}
					else
					{
						flag2 = true;
					}
				}
				else if (dataSetRef.LabelAlias == null)
				{
					flag2 = true;
				}
				if (!flag)
				{
					ErrorContext.Register(ProcessingErrorCode.rsInvalidDataSetReferenceField, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.ReportParameter, parameter.Name, "Field", dataSetRef.ValueAlias.MarkAsPrivate(), dataSetRef.DataSet.MarkAsPrivate());
				}
				if (!flag2)
				{
					ErrorContext.Register(ProcessingErrorCode.rsInvalidDataSetReferenceField, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.ReportParameter, parameter.Name, "Field", dataSetRef.LabelAlias.MarkAsPrivate(), dataSetRef.DataSet.MarkAsPrivate());
				}
				if (!isComplex)
				{
					if (publishingDataSetInfo.IsComplex || flag3)
					{
						isComplex = true;
						parameter.Dependencies = (Hashtable)dependencies.Clone();
					}
					else if (publishingDataSetInfo.ParameterNames != null && publishingDataSetInfo.ParameterNames.Count != 0)
					{
						Hashtable hashtable2 = parameter.Dependencies;
						if (hashtable2 == null)
						{
							hashtable2 = (parameter.Dependencies = new Hashtable());
						}
						foreach (string key in publishingDataSetInfo.ParameterNames.Keys)
						{
							if (dependencies.ContainsKey(key))
							{
								if (!hashtable2.ContainsKey(key))
								{
									hashtable2.Add(key, dependencies[key]);
								}
							}
							else
							{
								ErrorContext.Register(ProcessingErrorCode.rsInvalidReportParameterDependency, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.ReportParameter, parameter.Name, "DataSetReference", key.MarkAsPrivate());
							}
						}
					}
				}
			}
			if (isDefault)
			{
				parameter.DefaultDataSource = parameterDataSource;
			}
			else
			{
				parameter.ValidValuesDataSource = parameterDataSource;
			}
		}

		internal bool ValidateSliderLabelData(Tablix tablix, LabelData labelData)
		{
			if (labelData == null)
			{
				return true;
			}
			DataSet dataSet = null;
			dataSet = ((labelData.DataSetName != null) ? GetDataSet(labelData.DataSetName) : GetDataSet());
			if (dataSet != null)
			{
				labelData.DataSetName = dataSet.Name;
				bool flag = true;
				if (labelData.Label != null)
				{
					flag &= ValidateSliderDataFieldReference(labelData.Label, "Label", tablix.ObjectType, tablix.Name, dataSet);
				}
				if (labelData.KeyFields != null)
				{
					foreach (string keyField in labelData.KeyFields)
					{
						flag &= ValidateSliderDataFieldReference(keyField, "Key", tablix.ObjectType, tablix.Name, dataSet);
					}
					return flag;
				}
				return flag;
			}
			ErrorContext.Register(ProcessingErrorCode.rsInvalidSliderDataSetReference, Severity.Error, tablix.ObjectType, tablix.Name, null, labelData.DataSetName.MarkAsPrivate());
			return false;
		}

		private bool ValidateSliderDataFieldReference(string fieldName, string propertyName, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, DataSet dataSet)
		{
			Hashtable hashtable = (Hashtable)m_fieldNameMap[dataSet.Name];
			if (hashtable == null || !hashtable.ContainsKey(fieldName))
			{
				ErrorContext.Register(ProcessingErrorCode.rsInvalidSliderDataSetReferenceField, Severity.Error, objectType, objectName, propertyName, fieldName.MarkAsModelInfo(), dataSet.Name.MarkAsPrivate());
				return false;
			}
			return true;
		}

		internal void RegisterDataSetLevelAggregateOrLookup(int referencedDataSetIndex)
		{
			if (GetCurrentDataSetIndex() == referencedDataSetIndex)
			{
				return;
			}
			if (-1 == GetCurrentDataSetIndex())
			{
				m_report.SetDatasetDependency(referencedDataSetIndex, referencedDataSetIndex, clearDependency: false);
				return;
			}
			DataSet dataSet = GetDataSet();
			if (dataSet != null && dataSet.DataSource != null)
			{
				m_report.SetDatasetDependency(GetCurrentDataSetIndex(), referencedDataSetIndex, clearDependency: false);
			}
		}

		internal DataSet GetParentDataSet()
		{
			return m_activeDataSets.First;
		}

		internal bool RegisterDataRegion(DataRegion dataRegion)
		{
			_ = m_activeDataSets.First;
			DataSet dataSet = dataRegion.DataScopeInfo.DataSet;
			if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) == 0)
			{
				m_dataSetsForIdcInNestedDR.Clear();
				if (!m_initializingUserSorts)
				{
					if (m_report.TopLevelDataRegions == null)
					{
						m_report.TopLevelDataRegions = new List<DataRegion>();
					}
					m_report.TopLevelDataRegions.Add(dataRegion);
				}
				if (dataRegion.DataScopeInfo.DataSet == null)
				{
					return false;
				}
				if (dataSet != null && !m_initializingUserSorts)
				{
					m_dataSetNameToDataRegionsMap.TryGetValue(dataSet.Name, out List<DataRegion> value);
					Global.Tracer.Assert(value != null, "(null != dataRegions)");
					value.Add(dataRegion);
				}
			}
			RegisterDataSet(dataSet);
			RegisterDataRegionScope(dataRegion);
			return true;
		}

		internal void UnRegisterDataRegion(DataRegion dataRegion)
		{
			DataSet dataSet = dataRegion.DataScopeInfo.DataSet;
			UnRegisterDataSet(dataSet);
			UnRegisterDataRegionScope(dataRegion);
		}

		internal void RegisterDataSet(DataSet dataSet)
		{
			if (dataSet != null)
			{
				DataSet first = m_activeDataSets.First;
				m_activeDataSets = m_activeDataSets.Add(dataSet);
				m_activeScopeInfos = m_activeScopeInfos.Add(m_currentScope);
				if (first != dataSet)
				{
					ScopeInfo scopeInfo = (ScopeInfo)m_datasetScopes[dataSet.Name];
					Global.Tracer.Assert(scopeInfo != null, "(null != dataSetScope)");
					RegisterDataSetScope(dataSet, scopeInfo.Aggregates, scopeInfo.PostSortAggregates, dataSet.IndexInCollection);
				}
			}
		}

		internal void UnRegisterDataSet(DataSet dataSet)
		{
			if (dataSet != null)
			{
				DataSet first = m_activeDataSets.First;
				m_activeDataSets = m_activeDataSets.Rest;
				DataSet first2 = m_activeDataSets.First;
				if (first != first2)
				{
					UnRegisterDataSetScope(dataSet.Name);
				}
				m_currentScope = m_activeScopeInfos.First;
				m_activeScopeInfos = m_activeScopeInfos.Rest;
			}
		}

		private string GetDataSetName()
		{
			if (m_numberOfDataSets == 0)
			{
				return null;
			}
			if (1 == m_numberOfDataSets)
			{
				Global.Tracer.Assert(m_oneDataSetName != null);
				return m_oneDataSetName;
			}
			Global.Tracer.Assert(1 < m_numberOfDataSets);
			return GetCurrentDataSetName();
		}

		private DataSet GetDataSet()
		{
			string dataSetName = GetDataSetName();
			return GetDataSet(dataSetName);
		}

		private DataSet GetDataSet(string dataSetName)
		{
			DataSet dataSet = null;
			if (m_numberOfDataSets > 0)
			{
				Global.Tracer.Assert(dataSetName != null, "DataSet name must not be null");
				if (!m_reportScopes.ContainsKey(dataSetName))
				{
					return null;
				}
				dataSet = (m_reportScopes[dataSetName] as DataSet);
				Global.Tracer.Assert(dataSet != null, "DataSet {0} not found", dataSetName);
			}
			return dataSet;
		}

		internal void SetDataSetHasSubReports()
		{
			DataSet dataSet = GetDataSet();
			if (dataSet != null)
			{
				dataSet.HasSubReports = true;
			}
		}

		internal DataRegion GetCurrentDataRegion()
		{
			if (m_currentDataRegionName == null)
			{
				return null;
			}
			Global.Tracer.Assert(m_dataregionScopes.ContainsKey(m_currentDataRegionName));
			return ((ScopeInfo)m_dataregionScopes[m_currentDataRegionName]).DataRegionScope;
		}

		private bool ValidateDataSetNameForTopLevelDataRegion(string dataSetName, bool registerError)
		{
			bool flag = true;
			if (m_numberOfDataSets == 0)
			{
				flag = (dataSetName == null);
			}
			else if (1 == m_numberOfDataSets)
			{
				if (dataSetName == null)
				{
					dataSetName = m_oneDataSetName;
					flag = true;
				}
				else
				{
					flag = m_fieldNameMap.ContainsKey(dataSetName);
				}
			}
			else
			{
				Global.Tracer.Assert(1 < m_numberOfDataSets);
				flag = (dataSetName != null && m_fieldNameMap.ContainsKey(dataSetName));
			}
			if (!flag && registerError)
			{
				if (dataSetName == null)
				{
					m_errorContext.Register(ProcessingErrorCode.rsMissingDataSetName, Severity.Error, m_objectType, m_objectName, "DataSetName");
				}
				else
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidDataSetName, Severity.Error, m_objectType, m_objectName, "DataSetName", dataSetName.MarkAsPrivate());
				}
			}
			return flag;
		}

		internal void CheckFieldReferences(List<string> fieldNames, string propertyName)
		{
			InternalCheckFieldReferences(fieldNames, GetDataSetName(), m_objectType, m_objectName, propertyName);
		}

		internal void AggregateCheckFieldReferences(List<string> fieldNames, string dataSetName, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			InternalCheckFieldReferences(fieldNames, dataSetName, objectType, objectName, propertyName);
		}

		private void InternalCheckFieldReferences(List<string> fieldNames, string dataSetName, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (fieldNames != null)
			{
				for (int i = 0; i < fieldNames.Count; i++)
				{
					CheckFieldReference(fieldNames[i], dataSetName, objectType, objectName, propertyName);
				}
			}
		}

		private void CheckFieldReference(string fieldName, string dataSetName, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			Hashtable hashtable = null;
			if (dataSetName != null)
			{
				hashtable = (Hashtable)m_fieldNameMap[dataSetName];
			}
			if (m_numberOfDataSets == 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsFieldReference, Severity.Error, objectType, objectName, propertyName, fieldName.MarkAsModelInfo());
				return;
			}
			Global.Tracer.Assert(1 <= m_numberOfDataSets, "Expected 1 or more data sets");
			if (dataSetName != null)
			{
				DataSet dataSet = GetDataSet(dataSetName);
				if (dataSet != null && !dataSet.UsedOnlyInParametersSet && (m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataSet) == 0)
				{
					dataSet.UsedOnlyInParameters = false;
				}
			}
			if (hashtable == null && m_numberOfDataSets > 1 && (m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataSet) == 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsFieldReferenceAmbiguous, Severity.Error, objectType, objectName, propertyName, fieldName.MarkAsModelInfo());
			}
			else if (hashtable == null || !hashtable.ContainsKey(fieldName))
			{
				m_errorContext.Register(ProcessingErrorCode.rsFieldReference, Severity.Error, objectType, objectName, propertyName, fieldName.MarkAsModelInfo());
			}
		}

		internal void FillInFieldIndex(ExpressionInfo exprInfo)
		{
			InternalFillInFieldIndex(exprInfo, GetDataSetName());
		}

		internal void FillInFieldIndex(ExpressionInfo exprInfo, string dataSetName)
		{
			InternalFillInFieldIndex(exprInfo, dataSetName);
		}

		private void InternalFillInFieldIndex(ExpressionInfo exprInfo, string dataSetName)
		{
			if (exprInfo != null && exprInfo.Type == ExpressionInfo.Types.Field && dataSetName != null)
			{
				exprInfo.IntValue = GetIndexForDataSetField(dataSetName, exprInfo.StringValue);
			}
		}

		private int GetIndexForDataSetField(string dataSetName, string fieldName)
		{
			Hashtable hashtable = (Hashtable)m_fieldNameMap[dataSetName];
			int result = -1;
			if (hashtable != null && hashtable.ContainsKey(fieldName))
			{
				result = (int)hashtable[fieldName];
			}
			return result;
		}

		internal void FillInTokenIndex(ExpressionInfo exprInfo)
		{
			if (exprInfo == null || exprInfo.Type != ExpressionInfo.Types.Token)
			{
				return;
			}
			string stringValue = exprInfo.StringValue;
			if (stringValue != null)
			{
				DataSet dataSet = GetDataSet(stringValue);
				if (dataSet != null)
				{
					exprInfo.IntValue = dataSet.ID;
				}
			}
		}

		internal void CheckDataSetReference(List<string> referencedDataSets, string propertyName)
		{
			InternalCheckDataSetReference(referencedDataSets, m_objectType, m_objectName, propertyName);
		}

		internal void AggregateCheckDataSetReference(List<string> referencedDataSets, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			InternalCheckDataSetReference(referencedDataSets, objectType, objectName, propertyName);
		}

		private void InternalCheckDataSetReference(List<string> dataSetNames, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (dataSetNames == null || (m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
			{
				return;
			}
			for (int i = 0; i < dataSetNames.Count; i++)
			{
				DataSet dataSet = m_scopeTree.GetDataSet(dataSetNames[i]);
				if (dataSet == null)
				{
					m_errorContext.Register(ProcessingErrorCode.rsDataSetReference, Severity.Error, objectType, objectName, propertyName, dataSetNames[i].MarkAsPrivate());
				}
				else if (dataSet.IsReferenceToSharedDataSet)
				{
					dataSet.UsedOnlyInParameters = false;
				}
			}
		}

		internal int ResolveScopedFieldReferenceToIndex(string scopeName, string fieldName)
		{
			DataSet targetDataSetForScopeReference = GetTargetDataSetForScopeReference(scopeName);
			if (targetDataSetForScopeReference != null)
			{
				return GetIndexForDataSetField(targetDataSetForScopeReference.Name, fieldName);
			}
			return -1;
		}

		internal void CheckScopeReferences(List<ScopeReference> referencedScopes, string propertyName)
		{
			if (referencedScopes == null || referencedScopes.Count == 0)
			{
				return;
			}
			if (m_currentScope == null || m_currentScope.DataScope == null || m_currentScope.DataScope is DataSet)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidScopeCollectionReference, Severity.Error, m_objectType, m_objectName, propertyName);
				return;
			}
			IRIFDataScope dataScope = m_currentScope.DataScope;
			foreach (ScopeReference referencedScope in referencedScopes)
			{
				IRIFDataScope sourceScope = null;
				DataSet targetDataSet = GetTargetDataSetForScopeReference(referencedScope.ScopeName);
				if (targetDataSet != null)
				{
					ScopeTree.DirectedScopeTreeVisitor visitor = delegate(IRIFDataScope scope)
					{
						if (scope.DataScopeInfo != null && scope.DataScopeInfo.DataSet != null && targetDataSet.HasDefaultRelationship(scope.DataScopeInfo.DataSet))
						{
							sourceScope = scope;
							return false;
						}
						return true;
					};
					m_scopeTree.Traverse(visitor, dataScope);
				}
				if (sourceScope != null)
				{
					if (referencedScope.HasFieldName)
					{
						CheckFieldReference(referencedScope.FieldName, targetDataSet.Name, m_objectType, m_objectName, propertyName);
					}
					if (m_dataSetsForNonStructuralIdc.TryGetValue(targetDataSet.IndexInCollection, out IRIFDataScope value))
					{
						if (!sourceScope.DataScopeInfo.IsSameScope(value.DataScopeInfo))
						{
							m_errorContext.Register(ProcessingErrorCode.rsScopeReferenceUsesDataSetMoreThanOnce, Severity.Error, m_objectType, m_objectName, propertyName, targetDataSet.Name.MarkAsPrivate());
						}
					}
					else
					{
						m_dataSetsForNonStructuralIdc.Add(targetDataSet.IndexInCollection, sourceScope);
					}
				}
				else
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidScopeReference, Severity.Error, m_objectType, m_objectName, propertyName, referencedScope.ScopeName);
				}
			}
		}

		private DataSet GetTargetDataSetForScopeReference(string scopeName)
		{
			return m_scopeTree.GetDataSet(scopeName);
		}

		internal void CheckDataSourceReference(List<string> referencedDataSources, string propertyName)
		{
			InternalCheckDataSourceReference(referencedDataSources, m_objectType, m_objectName, propertyName);
		}

		internal void AggregateCheckDataSourceReference(List<string> referencedDataSources, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			InternalCheckDataSourceReference(referencedDataSources, objectType, objectName, propertyName);
		}

		private void InternalCheckDataSourceReference(List<string> dataSourceNames, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (dataSourceNames == null || (m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
			{
				return;
			}
			for (int i = 0; i < dataSourceNames.Count; i++)
			{
				if (!m_dataSources.ContainsKey(dataSourceNames[i]))
				{
					m_errorContext.Register(ProcessingErrorCode.rsDataSourceReference, Severity.Error, objectType, objectName, propertyName, dataSourceNames[i].MarkAsPrivate());
				}
			}
		}

		internal void RegisterGroupWithVariables(ReportHierarchyNode node)
		{
			if (node.Grouping != null && node.Grouping.Variables != null)
			{
				m_report.AddGroupWithVariables(node);
			}
		}

		internal void RegisterVariables(List<Variable> variables)
		{
			foreach (Variable variable in variables)
			{
				RegisterVariable(variable);
			}
		}

		internal void UnregisterVariables(List<Variable> variables)
		{
			foreach (Variable variable in variables)
			{
				UnregisterVariable(variable);
			}
		}

		internal void RegisterVariable(Variable variable)
		{
			if (!m_variablesInScope.ContainsKey(variable.Name))
			{
				m_variablesInScope.Add(variable.Name, variable);
				SequenceIndex.SetBit(ref m_referencableVariables, variable.SequenceID);
			}
		}

		internal void UnregisterVariable(Variable variable)
		{
			m_variablesInScope.Remove(variable.Name);
			SequenceIndex.ClearBit(ref m_referencableVariables, variable.SequenceID);
		}

		internal void CheckVariableReferences(List<string> referencedVariables, string propertyName)
		{
			InternalCheckVariableReferences(referencedVariables, m_objectType, m_objectName, propertyName);
		}

		internal void AggregateCheckVariableReferences(List<string> referencedVariables, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			InternalCheckVariableReferences(referencedVariables, objectType, objectName, propertyName);
		}

		private void InternalCheckVariableReferences(List<string> referencedVariables, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (referencedVariables == null || m_inAutoSubtotalClone)
			{
				return;
			}
			for (int i = 0; i < referencedVariables.Count; i++)
			{
				if (!m_variablesInScope.ContainsKey(referencedVariables[i]))
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidVariableReference, Severity.Error, objectType, objectName, propertyName, "Variable", referencedVariables[i].MarkAsPrivate());
				}
			}
		}

		internal void RegisterTextBoxInScope(TextBox textbox)
		{
			if (m_currentDataRegionName != null)
			{
				if (m_groupingScopesForRunningValuesInTablix != null && m_groupingScopesForRunningValuesInTablix.ContainerName.Equals(m_currentDataRegionName))
				{
					string currentColumnScopeName = m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName;
					string currentRowScopeName = m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName;
					if (currentColumnScopeName != null || currentRowScopeName != null)
					{
						if (currentColumnScopeName != null)
						{
							((ScopeInfo)m_groupingScopes[currentColumnScopeName]).ReportScope.AddInScopeTextBox(textbox);
						}
						if (currentRowScopeName != null)
						{
							((ScopeInfo)m_groupingScopes[currentRowScopeName]).ReportScope.AddInScopeTextBox(textbox);
						}
					}
					else
					{
						((IRIFReportScope)m_groupingScopesForRunningValuesInTablix.ContainerScope).AddInScopeTextBox(textbox);
					}
				}
				else
				{
					m_currentScope.ReportScope.AddInScopeTextBox(textbox);
				}
			}
			else
			{
				Global.Tracer.Assert(m_currentScope != null, "Top level scope should have been setup as either Page or ReportSection");
				m_currentScope.ReportScope.AddInScopeTextBox(textbox);
			}
		}

		internal void RegisterReportItem(ReportItem reportItem)
		{
			if (reportItem != null)
			{
				if (!m_reportItemsInScope.TryGetValue(reportItem.Name, out Pair<ReportItem, int> value))
				{
					value = new Pair<ReportItem, int>(reportItem, 1);
				}
				else
				{
					value.Second++;
				}
				m_reportItemsInScope[reportItem.Name] = value;
				m_reportItemsInSection[reportItem.Name] = reportItem;
				switch (reportItem.ObjectType)
				{
				case Microsoft.ReportingServices.ReportProcessing.ObjectType.Rectangle:
					RegisterReportItems(((Rectangle)reportItem).ReportItems);
					break;
				case Microsoft.ReportingServices.ReportProcessing.ObjectType.Tablix:
					RegisterReportItem((Tablix)reportItem);
					break;
				case Microsoft.ReportingServices.ReportProcessing.ObjectType.Textbox:
				{
					int sequenceID = ((TextBox)reportItem).SequenceID;
					SequenceIndex.SetBit(ref m_referencableTextboxes, sequenceID);
					SequenceIndex.SetBit(ref m_referencableTextboxesInSection, sequenceID);
					break;
				}
				}
			}
		}

		internal void RegisterReportItems(ReportItemCollection reportItems)
		{
			for (int i = 0; i < reportItems.Count; i++)
			{
				RegisterReportItem(reportItems[i]);
			}
		}

		private void RegisterReportItems(List<List<TablixCornerCell>> corner)
		{
			if (corner == null)
			{
				return;
			}
			foreach (List<TablixCornerCell> item in corner)
			{
				foreach (TablixCornerCell item2 in item)
				{
					RegisterReportItems(item2);
				}
			}
		}

		internal void RegisterReportItems(TablixRowList rows)
		{
			foreach (TablixRow row in rows)
			{
				foreach (TablixCell tablixCell in row.TablixCells)
				{
					RegisterReportItems(tablixCell);
				}
			}
		}

		private void RegisterReportItems(TablixCellBase cell)
		{
			if (cell.CellContents != null)
			{
				RegisterReportItem(cell.CellContents);
				if (cell.AltCellContents != null)
				{
					RegisterReportItem(cell.AltCellContents);
				}
			}
		}

		private void RegisterReportItem(Tablix tablix)
		{
			RegisterReportItems(tablix.Corner);
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			int index = 0;
			RegisterStaticMemberReportItems(tablix.TablixRowMembers, register: true, list, ref index);
			index = 0;
			RegisterStaticMemberReportItems(tablix.TablixColumnMembers, register: true, list2, ref index);
			if (tablix.TablixRows == null)
			{
				return;
			}
			foreach (int item in list)
			{
				if (item >= tablix.TablixRows.Count)
				{
					continue;
				}
				TablixRow tablixRow = tablix.TablixRows[item];
				foreach (int item2 in list2)
				{
					if (item2 < tablixRow.TablixCells.Count)
					{
						TablixCell cell = tablixRow.TablixCells[item2];
						RegisterReportItems(cell);
					}
				}
			}
		}

		private void RegisterStaticMemberReportItems(TablixMemberList members, bool register, List<int> indexes, ref int index)
		{
			foreach (TablixMember member in members)
			{
				if (member.Grouping == null)
				{
					RegisterMemberReportItems(member, register, registerStatic: true, indexes, ref index);
				}
				else
				{
					RegisterMemberReportItems(member, register: false, registerStatic: true, indexes, ref index);
				}
			}
		}

		internal void RegisterMemberReportItems(TablixMember member, bool firstPass, bool restrictive)
		{
			int index = 0;
			RegisterMemberReportItems(member, register: true, registerStatic: false, null, ref index);
			if (firstPass)
			{
				HandleCellContents(member, register: true, restrictive);
			}
		}

		internal void RegisterMemberReportItems(TablixMember member, bool firstPass)
		{
			RegisterMemberReportItems(member, firstPass, restrictive: true);
		}

		private void HandleCellContents(TablixMember member, bool register)
		{
			HandleCellContents(member, register, restrictive: true);
		}

		private void HandleCellContents(TablixMember member, bool register, bool restrictive)
		{
			Tablix tablix = (Tablix)member.DataRegionDef;
			int num = 0;
			int num2 = tablix.RowCount - 1;
			int num3 = 0;
			int num4 = tablix.ColumnCount - 1;
			if (restrictive)
			{
				if (member.IsColumn)
				{
					num3 = member.CellStartIndex;
					num4 = member.CellEndIndex;
				}
				else
				{
					num = member.CellStartIndex;
					num2 = member.CellEndIndex;
				}
			}
			if (!restrictive && m_handledCellContents.Value)
			{
				if (register)
				{
					return;
				}
				if (member.IsColumn)
				{
					if (member.CellStartIndex < num4 && member.CellEndIndex < num4)
					{
						return;
					}
				}
				else if (member.CellStartIndex < num2 && member.CellEndIndex < num2)
				{
					return;
				}
			}
			if (tablix.TablixRows == null)
			{
				return;
			}
			for (int i = num; i <= num2; i++)
			{
				if (i >= tablix.TablixRows.Count)
				{
					continue;
				}
				TablixRow tablixRow = tablix.TablixRows[i];
				for (int j = num3; j <= num4; j++)
				{
					if (j >= tablixRow.TablixCells.Count)
					{
						continue;
					}
					TablixCell tablixCell = tablixRow.TablixCells[j];
					if (tablixCell != null)
					{
						if (register)
						{
							RegisterReportItems(tablixCell);
						}
						else
						{
							UnRegisterReportItems(tablixCell);
						}
					}
				}
			}
			if (!restrictive)
			{
				m_handledCellContents.Value = (register ? true : false);
			}
		}

		private void RegisterMemberReportItems(TablixMember member, bool register, bool registerStatic, List<int> indexes, ref int index)
		{
			if (register && (registerStatic || member.Grouping != null) && member.TablixHeader != null && member.TablixHeader.CellContents != null)
			{
				RegisterReportItem(member.TablixHeader.CellContents);
				if (member.TablixHeader.AltCellContents != null)
				{
					RegisterReportItem(member.TablixHeader.AltCellContents);
				}
			}
			if (member.SubMembers != null)
			{
				RegisterStaticMemberReportItems(member.SubMembers, register, indexes, ref index);
			}
			else if (indexes != null)
			{
				if (register && member.Grouping == null)
				{
					indexes.Add(index);
				}
				index++;
			}
		}

		internal void UnRegisterReportItem(ReportItem reportItem)
		{
			if (reportItem == null)
			{
				return;
			}
			Pair<ReportItem, int> value = m_reportItemsInScope[reportItem.Name];
			value.Second--;
			bool flag = value.Second == 0;
			if (flag)
			{
				m_reportItemsInScope.Remove(reportItem.Name);
			}
			else
			{
				m_reportItemsInScope[reportItem.Name] = value;
			}
			switch (reportItem.ObjectType)
			{
			case Microsoft.ReportingServices.ReportProcessing.ObjectType.Rectangle:
				UnRegisterReportItems(((Rectangle)reportItem).ReportItems);
				break;
			case Microsoft.ReportingServices.ReportProcessing.ObjectType.Tablix:
				UnRegisterReportItem((Tablix)reportItem);
				break;
			case Microsoft.ReportingServices.ReportProcessing.ObjectType.Textbox:
				if (flag)
				{
					SequenceIndex.ClearBit(ref m_referencableTextboxes, ((TextBox)reportItem).SequenceID);
				}
				break;
			}
		}

		private void UnRegisterReportItems(List<List<TablixCornerCell>> corner)
		{
			if (corner == null)
			{
				return;
			}
			foreach (List<TablixCornerCell> item in corner)
			{
				foreach (TablixCornerCell item2 in item)
				{
					UnRegisterReportItems(item2);
				}
			}
		}

		internal void UnRegisterReportItems(TablixRowList rows)
		{
			foreach (TablixRow row in rows)
			{
				foreach (TablixCell tablixCell in row.TablixCells)
				{
					UnRegisterReportItems(tablixCell);
				}
			}
		}

		private void UnRegisterReportItems(TablixCellBase cell)
		{
			if (cell.CellContents != null)
			{
				UnRegisterReportItem(cell.CellContents);
				if (cell.AltCellContents != null)
				{
					UnRegisterReportItem(cell.AltCellContents);
				}
			}
		}

		internal void UnRegisterReportItem(Tablix tablix)
		{
			UnRegisterReportItems(tablix.Corner);
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			int index = 0;
			UnRegisterStaticMemberReportItems(tablix.TablixRowMembers, unregister: true, list, ref index);
			index = 0;
			UnRegisterStaticMemberReportItems(tablix.TablixColumnMembers, unregister: true, list2, ref index);
			if (tablix.TablixRows == null)
			{
				return;
			}
			foreach (int item in list)
			{
				if (item >= tablix.TablixRows.Count)
				{
					continue;
				}
				TablixRow tablixRow = tablix.TablixRows[item];
				foreach (int item2 in list2)
				{
					if (item2 < tablixRow.TablixCells.Count)
					{
						TablixCell cell = tablixRow.TablixCells[item2];
						UnRegisterReportItems(cell);
					}
				}
			}
		}

		private void UnRegisterStaticMemberReportItems(TablixMemberList members, bool unregister, List<int> indexes, ref int index)
		{
			foreach (TablixMember member in members)
			{
				if (member.Grouping == null)
				{
					UnRegisterMemberReportItems(member, unregister, unregisterStatic: true, indexes, ref index);
				}
				else
				{
					UnRegisterMemberReportItems(member, unregister: false, unregisterStatic: true, indexes, ref index);
				}
			}
		}

		internal void UnRegisterMemberReportItems(TablixMember member, bool firstPass)
		{
			UnRegisterMemberReportItems(member, firstPass, restrictive: true);
		}

		internal void UnRegisterMemberReportItems(TablixMember member, bool firstPass, bool restrictive)
		{
			int index = 0;
			UnRegisterMemberReportItems(member, unregister: true, unregisterStatic: false, null, ref index);
			if (firstPass)
			{
				HandleCellContents(member, register: false, restrictive);
			}
		}

		private void UnRegisterMemberReportItems(TablixMember member, bool unregister, bool unregisterStatic, List<int> indexes, ref int index)
		{
			if (unregister && (unregisterStatic || member.Grouping != null) && member.TablixHeader != null && member.TablixHeader.CellContents != null)
			{
				UnRegisterReportItem(member.TablixHeader.CellContents);
				if (member.TablixHeader.AltCellContents != null)
				{
					UnRegisterReportItem(member.TablixHeader.AltCellContents);
				}
			}
			if (member.SubMembers != null)
			{
				UnRegisterStaticMemberReportItems(member.SubMembers, unregister, indexes, ref index);
			}
			else if (indexes != null)
			{
				if (unregister && member.Grouping == null)
				{
					indexes.Add(index);
				}
				index++;
			}
		}

		internal void UnRegisterReportItems(ReportItemCollection reportItems)
		{
			Global.Tracer.Assert(reportItems != null);
			for (int i = 0; i < reportItems.Count; i++)
			{
				UnRegisterReportItem(reportItems[i]);
			}
		}

		internal void CheckReportItemReferences(List<string> referencedReportItems, string propertyName)
		{
			InternalCheckReportItemReferences(referencedReportItems, m_objectType, m_objectName, propertyName);
		}

		internal void AggregateCheckReportItemReferences(List<string> referencedReportItems, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			InternalCheckReportItemReferences(referencedReportItems, objectType, objectName, propertyName);
		}

		private void InternalCheckReportItemReferences(List<string> referencedReportItems, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (referencedReportItems == null)
			{
				return;
			}
			if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
			{
				for (int i = 0; i < referencedReportItems.Count; i++)
				{
					if (!m_reportItemsInSection.ContainsKey(referencedReportItems[i]))
					{
						m_errorContext.Register(ProcessingErrorCode.rsReportItemReferenceInPageSection, Severity.Error, objectType, objectName, propertyName, referencedReportItems[i]);
					}
				}
				return;
			}
			for (int j = 0; j < referencedReportItems.Count; j++)
			{
				if (!m_reportItemsInScope.ContainsKey(referencedReportItems[j]))
				{
					m_errorContext.Register(ProcessingErrorCode.rsReportItemReference, Severity.Error, objectType, objectName, propertyName, referencedReportItems[j]);
				}
			}
		}

		internal byte[] GetCurrentReferencableVariables()
		{
			if (m_referencableVariables == null)
			{
				return null;
			}
			return m_referencableVariables.Clone() as byte[];
		}

		internal byte[] GetCurrentReferencableTextboxes()
		{
			if (m_referencableTextboxes == null)
			{
				return null;
			}
			return m_referencableTextboxes.Clone() as byte[];
		}

		internal byte[] GetCurrentReferencableTextboxesInSection()
		{
			if (m_referencableTextboxesInSection == null)
			{
				return null;
			}
			return m_referencableTextboxesInSection.Clone() as byte[];
		}

		internal void CheckReportParameterReferences(List<string> referencedParameters, string propertyName)
		{
			InternalCheckReportParameterReferences(referencedParameters, m_objectType, m_objectName, propertyName);
		}

		private void InternalCheckReportParameterReferences(List<string> referencedParameters, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (referencedParameters == null)
			{
				return;
			}
			for (int i = 0; i < referencedParameters.Count; i++)
			{
				if (m_parameters == null || !m_parameters.ContainsKey(referencedParameters[i]))
				{
					m_errorContext.Register(ProcessingErrorCode.rsParameterReference, Severity.Error, objectType, objectName, propertyName, referencedParameters[i].MarkAsPrivate());
				}
			}
		}

		internal VisibilityToggleInfo RegisterVisibilityToggle(Visibility visibility)
		{
			if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
			{
				return null;
			}
			VisibilityToggleInfo visibilityToggleInfo = null;
			if (visibility.IsToggleReceiver)
			{
				visibilityToggleInfo = new VisibilityToggleInfo();
				visibilityToggleInfo.ObjectName = m_objectName;
				visibilityToggleInfo.ObjectType = m_objectType;
				visibilityToggleInfo.Visibility = visibility;
				visibilityToggleInfo.GroupName = m_currentGroupName;
				visibilityToggleInfo.GroupingSet = (Hashtable)m_groupingScopes.Clone();
				m_visibilityToggleInfos.Add(visibilityToggleInfo);
			}
			return visibilityToggleInfo;
		}

		internal bool RegisterVisibility(Visibility visibility, IVisibilityOwner owner)
		{
			if (NeedVisibilityLink(visibility, owner, out IVisibilityOwner outerContainer, out IVisibilityOwner outerRowContainer, out IVisibilityOwner outerColumnContainer))
			{
				VisibilityContainmentInfo visibilityContainmentInfo = new VisibilityContainmentInfo();
				if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InTablixColumnHierarchy) != 0 && outerColumnContainer != null)
				{
					owner.ContainingDynamicVisibility = outerColumnContainer;
				}
				else if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InTablixRowHierarchy) != 0 && outerRowContainer != null)
				{
					owner.ContainingDynamicVisibility = outerRowContainer;
				}
				else if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InTablixCell) != 0 && (outerColumnContainer != null || outerRowContainer != null))
				{
					owner.ContainingDynamicColumnVisibility = outerColumnContainer;
					owner.ContainingDynamicRowVisibility = outerRowContainer;
				}
				else if (outerContainer != null)
				{
					owner.ContainingDynamicVisibility = outerContainer;
				}
				if (owner.GetObjectType() == Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixMember)
				{
					TablixMember tablixMember = (TablixMember)owner;
					if (!tablixMember.IsAutoSubtotal)
					{
						if (tablixMember.IsColumn)
						{
							visibilityContainmentInfo.ContainingColumnVisibility = owner;
							visibilityContainmentInfo.ContainingRowVisibility = outerRowContainer;
						}
						else
						{
							visibilityContainmentInfo.ContainingRowVisibility = owner;
						}
					}
				}
				else
				{
					visibilityContainmentInfo.ContainingVisibility = owner;
				}
				m_visibilityContainmentInfos.Push(visibilityContainmentInfo);
				return true;
			}
			return false;
		}

		internal void UnRegisterVisibility(Visibility visibility, IVisibilityOwner owner)
		{
			m_visibilityContainmentInfos.Pop();
		}

		internal bool NeedVisibilityLink(Visibility visibility, IVisibilityOwner owner, out IVisibilityOwner outerContainer, out IVisibilityOwner outerRowContainer, out IVisibilityOwner outerColumnContainer)
		{
			outerContainer = null;
			outerRowContainer = null;
			outerColumnContainer = null;
			if (m_visibilityContainmentInfos.Count > 0)
			{
				VisibilityContainmentInfo visibilityContainmentInfo = m_visibilityContainmentInfos.Peek();
				outerContainer = visibilityContainmentInfo.ContainingVisibility;
				outerRowContainer = visibilityContainmentInfo.ContainingRowVisibility;
				outerColumnContainer = visibilityContainmentInfo.ContainingColumnVisibility;
			}
			bool flag = outerRowContainer != null || outerColumnContainer != null;
			if (!(owner.GetObjectType() == Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Tablix && flag) && owner.GetObjectType() != Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox)
			{
				if (visibility != null)
				{
					if (!visibility.IsConditional)
					{
						return visibility.IsToggleReceiver;
					}
					return true;
				}
				return false;
			}
			return true;
		}

		internal void RegisterToggleItem(TextBox textbox)
		{
			ToggleItemInfo toggleItemInfo = new ToggleItemInfo();
			toggleItemInfo.Textbox = textbox;
			toggleItemInfo.GroupName = m_currentGroupName;
			toggleItemInfo.GroupingSet = (Hashtable)m_groupingScopes.Clone();
			m_toggleItems.Add(textbox.Name, toggleItemInfo);
		}

		internal void ValidateToggleItems()
		{
			foreach (VisibilityToggleInfo visibilityToggleInfo in m_visibilityToggleInfos)
			{
				bool flag = false;
				ToggleItemInfo toggleItemInfo = null;
				if (m_toggleItems.ContainsKey(visibilityToggleInfo.Visibility.Toggle))
				{
					toggleItemInfo = m_toggleItems[visibilityToggleInfo.Visibility.Toggle];
					Hashtable groupingSet = toggleItemInfo.GroupingSet;
					Hashtable groupingSet2 = visibilityToggleInfo.GroupingSet;
					flag = ContainsSubsetOfKeys(groupingSet2, groupingSet);
					ScopeInfo scopeInfo = null;
					if (visibilityToggleInfo.GroupName != null)
					{
						scopeInfo = (ScopeInfo)groupingSet2[visibilityToggleInfo.GroupName];
					}
					if (!flag || (visibilityToggleInfo.IsTablixMember && scopeInfo != null && scopeInfo.GroupingScope.Parent != null && IsTargetVisibilityOnContainmentChain(toggleItemInfo.Textbox, visibilityToggleInfo.Visibility)))
					{
						if (visibilityToggleInfo.GroupName != null && toggleItemInfo.GroupName != null && visibilityToggleInfo.GroupName == toggleItemInfo.GroupName)
						{
							Global.Tracer.Assert(groupingSet.Contains(toggleItemInfo.GroupName), "(toggleItemGroupingSet.Contains(toggleItem.GroupName))");
							ScopeInfo scopeInfo2 = (ScopeInfo)groupingSet[toggleItemInfo.GroupName];
							if (scopeInfo2.GroupingScope.Parent != null)
							{
								flag = true;
								TextBox textbox = toggleItemInfo.Textbox;
								textbox.RecursiveSender = true;
								textbox.RecursiveMember = (TablixMember)scopeInfo2.ReportScope;
								Visibility visibility = visibilityToggleInfo.Visibility;
								visibility.RecursiveReceiver = true;
								if (scopeInfo != null)
								{
									visibility.RecursiveMember = (TablixMember)scopeInfo.ReportScope;
								}
							}
						}
					}
					else
					{
						for (ReportItem parent = toggleItemInfo.Textbox.Parent; parent != null; parent = parent.Parent)
						{
							if (parent.Visibility == visibilityToggleInfo.Visibility)
							{
								flag = false;
								break;
							}
						}
					}
				}
				if (flag)
				{
					TextBox textbox2 = toggleItemInfo.Textbox;
					textbox2.IsToggle = true;
					if (!visibilityToggleInfo.Visibility.RecursiveReceiver)
					{
						textbox2.HasNonRecursiveSender = true;
					}
					visibilityToggleInfo.Visibility.ToggleSender = textbox2;
					ReportItem reportItem = textbox2;
					do
					{
						reportItem.Computed = true;
						reportItem = reportItem.Parent;
					}
					while (reportItem is Rectangle);
				}
				else if (visibilityToggleInfo.Visibility.IsClone)
				{
					visibilityToggleInfo.Visibility.Toggle = null;
				}
				else
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidToggleItem, Severity.Error, visibilityToggleInfo.ObjectType, visibilityToggleInfo.ObjectName, "Item", visibilityToggleInfo.Visibility.Toggle);
				}
			}
			m_toggleItems.Clear();
			m_visibilityToggleInfos.Clear();
		}

		private bool IsTargetVisibilityOnContainmentChain(IVisibilityOwner visibilityOwner, Visibility targetVisibility)
		{
			if (visibilityOwner.Visibility == targetVisibility)
			{
				return true;
			}
			if (visibilityOwner.ContainingDynamicVisibility != null)
			{
				return IsTargetVisibilityOnContainmentChain(visibilityOwner.ContainingDynamicVisibility, targetVisibility);
			}
			bool flag = false;
			if (visibilityOwner.ContainingDynamicRowVisibility != null)
			{
				flag = IsTargetVisibilityOnContainmentChain(visibilityOwner.ContainingDynamicRowVisibility, targetVisibility);
			}
			if (!flag && visibilityOwner.ContainingDynamicColumnVisibility != null)
			{
				flag = IsTargetVisibilityOnContainmentChain(visibilityOwner.ContainingDynamicColumnVisibility, targetVisibility);
			}
			return flag;
		}

		private bool ContainsSubsetOfKeys(Hashtable set, Hashtable subSet)
		{
			int num = 0;
			foreach (object key in subSet.Keys)
			{
				if (!set.ContainsKey(key))
				{
					return false;
				}
				num++;
			}
			return num == subSet.Keys.Count;
		}

		internal void ValidateHeaderSize(double size, int startLevel, int span, bool isColumnHierarchy, int cellIndex)
		{
			double second = Math.Round(GetHeaderSize(isColumnHierarchy, startLevel, span), 4);
			double first = Math.Round(size, 4);
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareDoubles(first, second) != 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidTablixHeaderSize, Severity.Error, m_objectType, m_objectName, isColumnHierarchy ? "TablixColumnHierarchy" : "TablixRowHierarchy", "TablixHeader.Size", (cellIndex + 1).ToString(CultureInfo.InvariantCulture.NumberFormat), second.ToString(CultureInfo.InvariantCulture.NumberFormat), first.ToString(CultureInfo.InvariantCulture.NumberFormat), isColumnHierarchy ? "TablixColumn" : "TablixRow");
			}
		}

		internal double GetTotalHeaderSize(bool isColumnHierarchy, int span)
		{
			return GetHeaderSize(isColumnHierarchy, 0, span);
		}

		internal double GetHeaderSize(bool isColumnHierarchy, int startLevel, int span)
		{
			IList<Pair<double, int>> headerLevelSizeList;
			if (isColumnHierarchy)
			{
				Global.Tracer.Assert(m_columnHeaderLevelSizeList != null, "(m_columnHeaderLevelSizeList != null)");
				headerLevelSizeList = m_columnHeaderLevelSizeList;
			}
			else
			{
				Global.Tracer.Assert(m_rowHeaderLevelSizeList != null, "(m_rowHeaderLevelSizeList != null)");
				headerLevelSizeList = m_rowHeaderLevelSizeList;
			}
			return GetHeaderSize(headerLevelSizeList, startLevel, span);
		}

		internal double GetHeaderSize(IList<Pair<double, int>> headerLevelSizeList, int startingLevel, int spans)
		{
			int level = startingLevel + spans;
			startingLevel = FindEntryForLevel(headerLevelSizeList, startingLevel);
			level = FindEntryForLevel(headerLevelSizeList, level);
			return headerLevelSizeList[level].First - headerLevelSizeList[startingLevel].First;
		}

		private int FindEntryForLevel(IList<Pair<double, int>> headerLevelSizeList, int level)
		{
			if (level > 0)
			{
				int num = -1;
				for (int i = 0; i < headerLevelSizeList.Count; i++)
				{
					num += headerLevelSizeList[i].Second + 1;
					if (num >= level)
					{
						return i;
					}
				}
			}
			return 0;
		}

		internal double ValidateSize(string size, string propertyName)
		{
			Microsoft.ReportingServices.ReportPublishing.PublishingValidator.ValidateSize(size, m_objectType, m_objectName, propertyName, restrictMaxValue: true, m_errorContext, out double sizeInMM, out string _);
			return sizeInMM;
		}

		internal double ValidateSize(ref string size, string propertyName)
		{
			return ValidateSize(ref size, restrictMaxValue: true, propertyName);
		}

		internal double ValidateSize(ref string size, bool restrictMaxValue, string propertyName)
		{
			Microsoft.ReportingServices.ReportPublishing.PublishingValidator.ValidateSize(size, m_objectType, m_objectName, propertyName, restrictMaxValue, m_errorContext, out double sizeInMM, out string roundSize);
			size = roundSize;
			return sizeInMM;
		}

		internal void CheckInternationalSettings(Dictionary<string, AttributeInfo> styleAttributes)
		{
			if (styleAttributes == null || styleAttributes.Count == 0)
			{
				return;
			}
			CultureInfo culture = null;
			if (!styleAttributes.TryGetValue("Language", out AttributeInfo value))
			{
				culture = m_reportLanguage;
			}
			else if (!value.IsExpression)
			{
				Microsoft.ReportingServices.ReportPublishing.PublishingValidator.ValidateLanguage(value.Value, ObjectType, ObjectName, "Language", ErrorContext, out culture);
			}
			if (culture != null && styleAttributes.TryGetValue("Calendar", out AttributeInfo value2) && !value2.IsExpression)
			{
				Microsoft.ReportingServices.ReportPublishing.PublishingValidator.ValidateCalendar(culture, value2.Value, ObjectType, ObjectName, "Calendar", ErrorContext);
			}
			if (styleAttributes.TryGetValue("NumeralLanguage", out value))
			{
				if (value.IsExpression)
				{
					culture = null;
				}
				else
				{
					Microsoft.ReportingServices.ReportPublishing.PublishingValidator.ValidateLanguage(value.Value, ObjectType, ObjectName, "NumeralLanguage", ErrorContext, out culture);
				}
			}
			if (culture != null && styleAttributes.TryGetValue("NumeralVariant", out AttributeInfo value3) && !value3.IsExpression)
			{
				Microsoft.ReportingServices.ReportPublishing.PublishingValidator.ValidateNumeralVariant(culture, value3.IntValue, ObjectType, ObjectName, "NumeralVariant", ErrorContext);
			}
			if (styleAttributes.TryGetValue("CurrencyLanguage", out value) && !value.IsExpression)
			{
				Microsoft.ReportingServices.ReportPublishing.PublishingValidator.ValidateLanguage(value.Value, ObjectType, ObjectName, "CurrencyLanguage", ErrorContext, out culture);
			}
		}

		internal string GetCurrentScopeName()
		{
			Global.Tracer.Assert(m_currentScope != null, "Missing ScopeInfo for current scope.");
			if (m_currentScope.IsTopLevelScope)
			{
				return "0_ReportScope";
			}
			if (m_currentScope.GroupingScope != null)
			{
				return m_currentScope.GroupingScope.Name;
			}
			if (!m_isDataRegionScopedCell)
			{
				if (!m_groupingScopesForRunningValuesInTablix.IsRunningValueDirectionColumn)
				{
					if (m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName != null)
					{
						return m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName;
					}
					if (m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName != null)
					{
						return m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName;
					}
				}
				else
				{
					if (m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName != null)
					{
						return m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName;
					}
					if (m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName != null)
					{
						return m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName;
					}
				}
			}
			return m_currentDataRegionName;
		}

		internal bool IsScope(string scope)
		{
			if (scope == null)
			{
				return false;
			}
			return m_reportScopes.ContainsKey(scope);
		}

		internal bool IsAncestorScope(string targetScope)
		{
			string dataSetName = GetDataSetName();
			if ((dataSetName == null || Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(dataSetName, targetScope, ignoreCase: false) != 0) && (m_dataregionScopes == null || !m_dataregionScopes.ContainsKey(targetScope)))
			{
				if (m_groupingScopesForRunningValuesInTablix != null)
				{
					return m_groupingScopesForRunningValuesInTablix.ContainsScope(targetScope);
				}
				return false;
			}
			return true;
		}

		internal bool IsSameOrChildScope(string parentScope, string childScope)
		{
			if (parentScope == childScope)
			{
				return true;
			}
			if (m_datasetScopes.ContainsKey(parentScope))
			{
				if (((ScopeInfo)m_datasetScopes[parentScope]).DataSetScope == m_reportScopeDatasets[childScope])
				{
					return true;
				}
			}
			else if (m_dataregionScopes.ContainsKey(parentScope))
			{
				ReportItem reportItem = null;
				ReportItem reportItem2 = null;
				if (m_dataregionScopes.ContainsKey(childScope))
				{
					reportItem2 = ((ScopeInfo)m_dataregionScopes[childScope]).DataRegionScope;
					reportItem = ((ScopeInfo)m_dataregionScopes[parentScope]).DataRegionScope;
				}
				else if (m_groupingScopes.ContainsKey(childScope))
				{
					reportItem = ((ScopeInfo)m_dataregionScopes[parentScope]).DataRegionScope;
					reportItem2 = ((ScopeInfo)m_groupingScopes[childScope]).GroupingScope.Owner.DataRegionDef;
					if (reportItem2 == reportItem)
					{
						return true;
					}
				}
				while (reportItem2.Parent != null)
				{
					if (reportItem2.Parent == reportItem)
					{
						return true;
					}
					reportItem2 = reportItem2.Parent;
				}
			}
			else if (m_groupingScopes.ContainsKey(parentScope))
			{
				if (m_dataregionScopes.ContainsKey(childScope))
				{
					ReportItem reportItem3 = ((ScopeInfo)m_dataregionScopes[childScope]).DataRegionScope;
					ReportItem dataRegionDef = ((ScopeInfo)m_groupingScopes[parentScope]).GroupingScope.Owner.DataRegionDef;
					while (reportItem3.Parent != null)
					{
						if (reportItem3.Parent == dataRegionDef)
						{
							return true;
						}
						reportItem3 = reportItem3.Parent;
					}
				}
				else if (m_groupingScopes.ContainsKey(childScope))
				{
					return GetScopeChainInfo().IsSameOrChildScope(parentScope, childScope);
				}
			}
			return false;
		}

		internal bool IsCurrentScope(string targetScope)
		{
			return targetScope == GetCurrentScopeName();
		}

		internal bool HasPeerGroups(DataRegion dataRegion)
		{
			if (HasPeerGroups(dataRegion.RowMembers, out bool hasGroup))
			{
				return true;
			}
			return HasPeerGroups(dataRegion.ColumnMembers, out hasGroup);
		}

		private bool HasPeerGroups(HierarchyNodeList nodes, out bool hasGroup)
		{
			hasGroup = false;
			if (nodes != null)
			{
				foreach (ReportHierarchyNode node in nodes)
				{
					bool hasGroup2 = false;
					if (node.InnerHierarchy != null && HasPeerGroups(node.InnerHierarchy, out hasGroup2))
					{
						hasGroup = true;
						return true;
					}
					if (node.IsGroup || hasGroup2)
					{
						if (hasGroup)
						{
							return true;
						}
						hasGroup = true;
					}
				}
			}
			return false;
		}

		internal bool IsPeerScope(string targetScope)
		{
			if (!m_hasUserSortPeerScopes)
			{
				return false;
			}
			string currentScopeName = GetCurrentScopeName();
			Global.Tracer.Assert(currentScopeName != null && m_peerScopes != null, "(null != currentScope && null != m_peerScopes)");
			object obj = m_peerScopes[currentScopeName];
			int num = 0;
			int num2 = 0;
			if (obj == null)
			{
				return false;
			}
			num = (int)obj;
			obj = m_peerScopes[targetScope];
			if (obj == null)
			{
				return false;
			}
			num2 = (int)obj;
			return num == num2;
		}

		internal bool IsReportTopLevelScope()
		{
			if (m_currentScope != null)
			{
				return m_currentScope.IsTopLevelScope;
			}
			return true;
		}

		internal ISortFilterScope GetSortFilterScope()
		{
			return GetSortFilterScope(GetCurrentScopeName());
		}

		internal ISortFilterScope GetSortFilterScope(string scopeName)
		{
			Global.Tracer.Assert(scopeName != null && "0_ReportScope" != scopeName && m_reportScopes.ContainsKey(scopeName));
			return m_reportScopes[scopeName];
		}

		internal void RegisterPeerScopes(ReportItemCollection reportItems)
		{
			RegisterPeerScopes(reportItems, ++m_lastPeerScopeId, traverse: true);
		}

		private void RegisterPeerScopes(TablixMemberList members, int scopeID)
		{
			if (members == null)
			{
				return;
			}
			foreach (TablixMember member in members)
			{
				if (member.Grouping != null)
				{
					continue;
				}
				if (member.TablixHeader != null && member.TablixHeader.CellContents != null)
				{
					RegisterPeerScope(member.TablixHeader.CellContents, scopeID, traverse: false);
					if (member.TablixHeader.AltCellContents != null)
					{
						RegisterPeerScope(member.TablixHeader.AltCellContents, scopeID, traverse: false);
					}
				}
				if (member.SubMembers != null)
				{
					RegisterPeerScopes(member.SubMembers, scopeID);
				}
			}
		}

		private void RegisterPeerScopes(List<List<TablixCornerCell>> cornerCells, int scopeID)
		{
			if (cornerCells == null)
			{
				return;
			}
			foreach (List<TablixCornerCell> cornerCell in cornerCells)
			{
				foreach (TablixCornerCell item in cornerCell)
				{
					if (item.CellContents != null)
					{
						RegisterPeerScope(item.CellContents, scopeID, traverse: false);
						if (item.AltCellContents != null)
						{
							RegisterPeerScope(item.AltCellContents, scopeID, traverse: false);
						}
					}
				}
			}
		}

		private void RegisterPeerScopes(ReportItemCollection reportItems, int scopeID, bool traverse)
		{
			if (reportItems == null || !m_hasUserSortPeerScopes)
			{
				return;
			}
			string currentScopeName = GetCurrentScopeName();
			if (!m_peerScopes.ContainsKey(currentScopeName))
			{
				InternalRegisterPeerScopes(reportItems, scopeID, traverse);
				if (!m_peerScopes.ContainsKey(currentScopeName))
				{
					m_peerScopes.Add(currentScopeName, scopeID);
				}
			}
		}

		private void InternalRegisterPeerScopes(ReportItemCollection reportItems, int scopeID, bool traverse)
		{
			if (reportItems != null)
			{
				int count = reportItems.Count;
				for (int i = 0; i < count; i++)
				{
					ReportItem item = reportItems[i];
					RegisterPeerScope(item, scopeID, traverse);
				}
			}
		}

		private void RegisterPeerScope(ReportItem item, int scopeID, bool traverse)
		{
			if (item is Rectangle)
			{
				InternalRegisterPeerScopes(((Rectangle)item).ReportItems, scopeID, traverse);
			}
			else if (item.IsDataRegion && !m_peerScopes.ContainsKey(item.Name))
			{
				m_peerScopes.Add(item.Name, scopeID);
			}
			if (traverse && item is Tablix)
			{
				RegisterPeerScopes(((Tablix)item).Corner, scopeID);
				RegisterPeerScopes(((Tablix)item).TablixColumnMembers, scopeID);
				RegisterPeerScopes(((Tablix)item).TablixRowMembers, scopeID);
			}
		}

		private void RegisterUserSortInnerScope(IInScopeEventSource eventSource)
		{
			string sortExpressionScopeString = eventSource.UserSort.SortExpressionScopeString;
			if (m_groupingScopes.ContainsKey(sortExpressionScopeString) && ((ScopeInfo)m_groupingScopes[sortExpressionScopeString]).GroupingScope.IsDetail)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidSortExpressionScope, Severity.Error, eventSource.ObjectType, eventSource.Name, "SortExpressionScope", sortExpressionScopeString);
				eventSource.UserSort.SortExpressionScopeString = null;
				return;
			}
			if (m_userSortExpressionScopes.TryGetValue(sortExpressionScopeString, out List<IInScopeEventSource> value))
			{
				value.Add(eventSource);
				return;
			}
			ISortFilterScope value2 = null;
			if (m_reportScopes.TryGetValue(sortExpressionScopeString, out value2) && value2 is Grouping && ((Grouping)value2).DomainScope != null)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidSortExpressionScopeDomainScope, Severity.Error, eventSource.ObjectType, eventSource.Name, "SortExpressionScope", sortExpressionScopeString);
			}
			value = new List<IInScopeEventSource>();
			value.Add(eventSource);
			m_userSortExpressionScopes.Add(sortExpressionScopeString, value);
		}

		private void UnregisterUserSortInnerScope(string sortExpressionScopeString, IInScopeEventSource eventSource)
		{
			if (m_userSortExpressionScopes.TryGetValue(sortExpressionScopeString, out List<IInScopeEventSource> value))
			{
				value.Remove(eventSource);
			}
		}

		internal void ProcessUserSortScopes(string scopeName)
		{
			if (!m_hasUserSorts)
			{
				return;
			}
			if (m_userSortExpressionScopes.TryGetValue(scopeName, out List<IInScopeEventSource> value))
			{
				for (int num = value.Count - 1; num >= 0; num--)
				{
					IInScopeEventSource inScopeEventSource = value[num];
					Global.Tracer.Assert(inScopeEventSource.UserSort != null, "(null != eventSource.UserSort)");
					if (m_groupingScopes.ContainsKey(scopeName) && ((ScopeInfo)m_groupingScopes[scopeName]).GroupingScope.IsDetail)
					{
						m_errorContext.Register(ProcessingErrorCode.rsInvalidSortExpressionScope, Severity.Error, inScopeEventSource.ObjectType, inScopeEventSource.Name, "SortExpressionScope", scopeName);
						inScopeEventSource.UserSort.SortExpressionScopeString = null;
					}
					else
					{
						inScopeEventSource.ScopeChainInfo = GetScopeChainInfo();
						inScopeEventSource.UserSort.SortExpressionScope = GetSortFilterScope(inScopeEventSource.UserSort.SortExpressionScopeString);
						InitializeSortExpression(inScopeEventSource, needsExplicitAggregateScope: false);
					}
					value.RemoveAt(num);
				}
				m_userSortExpressionScopes.Remove(scopeName);
			}
			if (!m_userSortEventSources.TryGetValue(scopeName, out value))
			{
				return;
			}
			for (int num2 = value.Count - 1; num2 >= 0; num2--)
			{
				IInScopeEventSource inScopeEventSource2 = value[num2];
				Global.Tracer.Assert(inScopeEventSource2.UserSort != null, "(null != eventSource.UserSort)");
				if (inScopeEventSource2.UserSort.SortTarget != null)
				{
					inScopeEventSource2.UserSort.DataSet = (DataSet)m_reportScopeDatasets[inScopeEventSource2.UserSort.SortTarget.ScopeName];
					if (inScopeEventSource2.UserSort.SortExpressionScopeString != null)
					{
						if (inScopeEventSource2.UserSort.SortExpressionScope != null)
						{
							if (m_reportScopeDatasets[inScopeEventSource2.UserSort.SortExpressionScope.ScopeName] != inScopeEventSource2.UserSort.DataSet)
							{
								m_errorContext.Register(ProcessingErrorCode.rsInvalidExpressionScopeDataSet, Severity.Error, inScopeEventSource2.ObjectType, inScopeEventSource2.Name, "SortExpressionScope", inScopeEventSource2.UserSort.SortExpressionScope.ScopeName, "SortTarget");
							}
							else
							{
								ScopeChainInfo scopeChainInfo = GetScopeChainInfo();
								Grouping fromGroup = null;
								if (scopeChainInfo != null)
								{
									fromGroup = scopeChainInfo.GetInnermostGrouping();
								}
								if (inScopeEventSource2.ScopeChainInfo != null)
								{
									inScopeEventSource2.UserSort.GroupsInSortTarget = inScopeEventSource2.ScopeChainInfo.GetGroupsFromCurrentTablixAxisToGrouping(fromGroup);
								}
								if (inScopeEventSource2.ContainingScopes != null)
								{
									int num3 = inScopeEventSource2.ContainingScopes.Count - 1;
									if (0 <= num3 && string.CompareOrdinal(inScopeEventSource2.UserSort.SortExpressionScopeString, inScopeEventSource2.ContainingScopes[num3].Name) == 0)
									{
										m_errorContext.Register(ProcessingErrorCode.rsIneffectiveSortExpressionScope, Severity.Warning, inScopeEventSource2.ObjectType, inScopeEventSource2.Name, "SortExpressionScope", inScopeEventSource2.UserSort.SortExpressionScopeString);
									}
								}
							}
						}
						else
						{
							m_errorContext.Register(ProcessingErrorCode.rsInvalidExpressionScope, Severity.Error, inScopeEventSource2.ObjectType, inScopeEventSource2.Name, "SortExpressionScope", inScopeEventSource2.UserSort.SortExpressionScopeString);
						}
					}
					if (!m_errorContext.HasError)
					{
						AddToScopeSortFilterList(inScopeEventSource2);
					}
				}
				value.RemoveAt(num2);
			}
			m_userSortEventSources.Remove(scopeName);
		}

		internal void RegisterSortEventSource(IInScopeEventSource eventSource)
		{
			if (m_hasUserSorts && eventSource != null && eventSource.UserSort != null)
			{
				if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegionCellTopLevelItem) != 0)
				{
					eventSource.IsTablixCellScope = IsDataRegionCellScope;
				}
				string sortExpressionScopeString = eventSource.UserSort.SortExpressionScopeString;
				if (sortExpressionScopeString != null && IsScope(sortExpressionScopeString))
				{
					RegisterUserSortInnerScope(eventSource);
				}
				string sortTargetString = eventSource.UserSort.SortTargetString;
				if (sortTargetString != null && IsScope(sortTargetString))
				{
					RegisterUserSortWithSortTarget(eventSource);
				}
			}
		}

		internal void ProcessSortEventSource(IInScopeEventSource eventSource)
		{
			if (!m_initializingUserSorts || !m_hasUserSorts || eventSource == null || eventSource.UserSort == null)
			{
				return;
			}
			AddEventSourceToScope(eventSource);
			GroupingList containingScopes = GetContainingScopes();
			for (int i = 0; i < containingScopes.Count; i++)
			{
				containingScopes[i].SaveGroupExprValues = true;
			}
			if ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDetail) != 0)
			{
				containingScopes.Add(null);
				SetDataSetDetailUserSortFilter();
			}
			eventSource.ContainingScopes = containingScopes;
			string sortExpressionScopeString = eventSource.UserSort.SortExpressionScopeString;
			if (sortExpressionScopeString == null)
			{
				EventSourceWithDetailSortExpressionAdd(eventSource);
			}
			else if (IsScope(sortExpressionScopeString))
			{
				if (IsCurrentScope(sortExpressionScopeString))
				{
					eventSource.UserSort.SortExpressionScope = GetSortFilterScope(sortExpressionScopeString);
					eventSource.ScopeChainInfo = GetScopeChainInfo();
					InitializeSortExpression(eventSource, needsExplicitAggregateScope: false);
					UnregisterUserSortInnerScope(sortExpressionScopeString, eventSource);
				}
				else if (IsAncestorScope(sortExpressionScopeString))
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidExpressionScope, Severity.Error, m_objectType, m_objectName, "SortExpressionScope", sortExpressionScopeString);
					UnregisterUserSortInnerScope(sortExpressionScopeString, eventSource);
				}
				else if (!m_scopeTree.IsSameOrProperParentScope(m_currentScope.DataScope, m_scopeTree.GetScopeByName(sortExpressionScopeString)))
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidExpressionScope, Severity.Warning, eventSource.ObjectType, eventSource.Name, "SortExpressionScope", sortExpressionScopeString);
				}
			}
			else
			{
				m_errorContext.Register(ProcessingErrorCode.rsNonExistingScope, Severity.Error, m_objectType, m_objectName, "SortExpressionScope", sortExpressionScopeString);
			}
			string sortTargetString = eventSource.UserSort.SortTargetString;
			if (sortTargetString != null)
			{
				if (IsScope(sortTargetString))
				{
					if (!IsCurrentScope(sortTargetString) && !IsAncestorScope(sortTargetString) && !IsPeerScope(sortTargetString))
					{
						m_errorContext.Register(ProcessingErrorCode.rsInvalidTargetScope, Severity.Error, m_objectType, m_objectName, "SortTarget", sortTargetString);
						UnregisterUserSortWithSortTarget(sortTargetString, eventSource);
						return;
					}
					bool bothGroups = false;
					if (m_groupingScopesForRunningValuesInTablix != null && (m_groupingScopesForRunningValuesInTablix.HasRowColScopeConflict(eventSource.Scope, sortTargetString, out bothGroups) || (bothGroups && ((ScopeInfo)m_groupingScopes[eventSource.Scope]).GroupingScope.Owner.DataRegionDef != ((ScopeInfo)m_groupingScopes[sortTargetString]).GroupingScope.Owner.DataRegionDef)))
					{
						m_errorContext.Register(ProcessingErrorCode.rsInvalidTargetScope, Severity.Error, m_objectType, m_objectName, "SortTarget", sortTargetString);
						UnregisterUserSortWithSortTarget(sortTargetString, eventSource);
					}
				}
				else
				{
					m_errorContext.Register(ProcessingErrorCode.rsNonExistingScope, Severity.Error, m_objectType, m_objectName, "SortTarget", sortTargetString);
				}
			}
			else if (IsReportTopLevelScope())
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidOmittedTargetScope, Severity.Error, m_objectType, m_objectName, "SortTarget");
			}
			else
			{
				RegisterUserSortWithSortTarget(eventSource);
			}
		}

		private void RegisterUserSortWithSortTarget(IInScopeEventSource eventSource)
		{
			string text = eventSource.UserSort.SortTargetString;
			if (text == null)
			{
				text = GetCurrentScopeName();
			}
			if (m_userSortEventSources.TryGetValue(text, out List<IInScopeEventSource> value))
			{
				Global.Tracer.Assert(!value.Contains(eventSource), "(false == registeredEventSources.Contains(eventSource))");
				value.Add(eventSource);
			}
			else
			{
				value = new List<IInScopeEventSource>();
				value.Add(eventSource);
				m_userSortEventSources.Add(text, value);
			}
		}

		private void UnregisterUserSortWithSortTarget(string sortTarget, IInScopeEventSource eventSource)
		{
			if (m_userSortEventSources.TryGetValue(sortTarget, out List<IInScopeEventSource> value))
			{
				Global.Tracer.Assert(value.Contains(eventSource), "(registeredEventSources.Contains(eventSource))");
				value.Remove(eventSource);
			}
		}

		internal GroupingList GetContainingScopesInCurrentDataRegion()
		{
			return GetScopeChainInfo()?.GetGroupingListForContainingDataRegion();
		}

		internal GroupingList GetContainingScopes()
		{
			ScopeChainInfo scopeChainInfo = GetScopeChainInfo();
			if (scopeChainInfo != null)
			{
				return scopeChainInfo.GetGroupingList();
			}
			return new GroupingList();
		}

		private ScopeChainInfo GetScopeChainInfo()
		{
			if (m_groupingScopesForRunningValuesInTablix != null)
			{
				return m_groupingScopesForRunningValuesInTablix.GetScopeChainInfo();
			}
			return null;
		}

		private void AddEventSourceToScope(IInScopeEventSource eventSource)
		{
			eventSource.Scope = GetCurrentScopeName();
			IRIFReportScope iRIFReportScope = ((m_location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) == 0) ? m_report : (eventSource.IsTablixCellScope ? m_currentScope.ReportScope : ((m_currentScope.DataRegionScope == null && m_currentScope.GroupingScope == null) ? ((ScopeInfo)m_groupingScopes[eventSource.Scope]).ReportScope : m_currentScope.ReportScope));
			iRIFReportScope.AddInScopeEventSource(eventSource);
			m_report.AddEventSource(eventSource);
		}

		private void InitializeSortExpression(IInScopeEventSource eventSource, bool needsExplicitAggregateScope)
		{
			EndUserSort userSort = eventSource.UserSort;
			if (userSort == null || userSort.SortExpression == null)
			{
				return;
			}
			bool flag = true;
			if (needsExplicitAggregateScope && userSort.SortExpression.Aggregates != null)
			{
				int count = userSort.SortExpression.Aggregates.Count;
				for (int i = 0; i < count; i++)
				{
					if (!userSort.SortExpression.Aggregates[i].GetScope(out string _))
					{
						flag = false;
						m_errorContext.Register(ProcessingErrorCode.rsInvalidOmittedExpressionScope, Severity.Error, m_objectType, m_objectName, "SortExpression", "SortExpressionScope");
					}
				}
			}
			if (flag)
			{
				userSort.SortExpression.Initialize("SortExpression", this);
			}
		}

		private void AddToScopeSortFilterList(IInScopeEventSource eventSource)
		{
			List<int> peerSortFilters = eventSource.GetPeerSortFilters(create: true);
			Global.Tracer.Assert(peerSortFilters != null, "(null != peerSorts)");
			peerSortFilters.Add(eventSource.ID);
		}

		internal void SetDataSetDetailUserSortFilter()
		{
			DataSet dataSet = GetDataSet();
			if (dataSet != null)
			{
				dataSet.HasDetailUserSortFilter = true;
			}
		}

		private void EventSourceWithDetailSortExpressionAdd(IInScopeEventSource eventSource)
		{
			Global.Tracer.Assert(m_detailSortExpressionScopeEventSources != null, "(null != m_detailSortExpressionScopeEventSources)");
			m_detailSortExpressionScopeEventSources.Add(eventSource);
		}

		internal void EventSourcesWithDetailSortExpressionInitialize(string sortExpressionScope)
		{
			if (!m_hasUserSorts)
			{
				return;
			}
			Global.Tracer.Assert(m_detailSortExpressionScopeEventSources != null, "(null != m_detailSortExpressionScopeEventSources)");
			int count = m_detailSortExpressionScopeEventSources.Count;
			if (count == 0)
			{
				return;
			}
			for (int i = 0; i < count; i++)
			{
				IInScopeEventSource inScopeEventSource = m_detailSortExpressionScopeEventSources[i];
				inScopeEventSource.UserSort.SortExpressionScope = null;
				InitializeSortExpression(inScopeEventSource, needsExplicitAggregateScope: true);
				if (sortExpressionScope != null && inScopeEventSource.ContainingScopes != null)
				{
					int num = inScopeEventSource.ContainingScopes.Count - 1;
					if (0 <= num && inScopeEventSource.ContainingScopes[num] == null)
					{
						m_errorContext.Register(ProcessingErrorCode.rsIneffectiveSortExpressionScope, Severity.Warning, inScopeEventSource.ObjectType, inScopeEventSource.Name, "SortExpressionScope", sortExpressionScope);
					}
				}
			}
			m_detailSortExpressionScopeEventSources.Clear();
		}

		internal void InitializeAbsolutePosition(ReportItem reportItem)
		{
			m_currentAbsoluteTop += reportItem.AbsoluteTopValue;
			m_currentAbsoluteLeft += reportItem.AbsoluteLeftValue;
		}

		internal void UpdateTopLeftDataRegion(DataRegion dataRegion)
		{
			m_report.UpdateTopLeftDataRegion(this, dataRegion);
		}

		internal void AddGroupingExprCountForGroup(string scope, int groupingExprCount)
		{
			if (!m_groupingExprCountAtScope.ContainsKey(scope))
			{
				m_groupingExprCountAtScope.Add(scope, groupingExprCount);
			}
		}

		internal void EnforceRdlSandboxContentRestrictions(CodeClass codeClass)
		{
		}

		internal void EnforceRdlSandboxContentRestrictions(ExpressionInfo expression, string propertyName)
		{
			EnforceRdlSandboxContentRestrictions(expression, ObjectType, ObjectName, propertyName);
		}

		internal void EnforceRdlSandboxContentRestrictions(ExpressionInfo expression, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
		}
	}
}

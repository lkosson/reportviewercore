using Microsoft.ReportingServices.Diagnostics;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal struct InitializationContext
	{
		private enum GroupingType
		{
			Normal,
			MatrixRow,
			MatrixColumn
		}

		private sealed class ScopeInfo
		{
			private bool m_allowCustomAggregates;

			private DataAggregateInfoList m_aggregates;

			private DataAggregateInfoList m_postSortAggregates;

			private DataAggregateInfoList m_recursiveAggregates;

			private Grouping m_groupingScope;

			private DataRegion m_dataRegionScope;

			private DataSet m_dataSetScope;

			internal bool AllowCustomAggregates => m_allowCustomAggregates;

			internal DataAggregateInfoList Aggregates => m_aggregates;

			internal DataAggregateInfoList PostSortAggregates => m_postSortAggregates;

			internal DataAggregateInfoList RecursiveAggregates => m_recursiveAggregates;

			internal Grouping GroupingScope => m_groupingScope;

			internal DataSet DataSetScope => m_dataSetScope;

			internal ScopeInfo(bool allowCustomAggregates, DataAggregateInfoList aggregates)
			{
				m_allowCustomAggregates = allowCustomAggregates;
				m_aggregates = aggregates;
			}

			internal ScopeInfo(bool allowCustomAggregates, DataAggregateInfoList aggregates, DataAggregateInfoList postSortAggregates)
			{
				m_allowCustomAggregates = allowCustomAggregates;
				m_aggregates = aggregates;
				m_postSortAggregates = postSortAggregates;
			}

			internal ScopeInfo(bool allowCustomAggregates, DataAggregateInfoList aggregates, DataAggregateInfoList postSortAggregates, DataRegion dataRegion)
			{
				m_allowCustomAggregates = allowCustomAggregates;
				m_aggregates = aggregates;
				m_postSortAggregates = postSortAggregates;
				m_dataRegionScope = dataRegion;
			}

			internal ScopeInfo(bool allowCustomAggregates, DataAggregateInfoList aggregates, DataAggregateInfoList postSortAggregates, DataSet dataset)
			{
				m_allowCustomAggregates = allowCustomAggregates;
				m_aggregates = aggregates;
				m_postSortAggregates = postSortAggregates;
				m_dataSetScope = dataset;
			}

			internal ScopeInfo(bool allowCustomAggregates, DataAggregateInfoList aggregates, DataAggregateInfoList postSortAggregates, DataAggregateInfoList recursiveAggregates, Grouping groupingScope)
			{
				m_allowCustomAggregates = allowCustomAggregates;
				m_aggregates = aggregates;
				m_postSortAggregates = postSortAggregates;
				m_recursiveAggregates = recursiveAggregates;
				m_groupingScope = groupingScope;
			}
		}

		private sealed class GroupingScopesForTablix
		{
			private bool m_rowScopeFound;

			private bool m_columnScopeFound;

			private ObjectType m_containerType;

			private string m_containerName;

			private Hashtable m_rowScopes;

			private Hashtable m_columnScopes;

			internal GroupingScopesForTablix(bool forceRows, ObjectType containerType, string containerName)
			{
				m_rowScopeFound = forceRows;
				m_columnScopeFound = false;
				m_containerType = containerType;
				m_containerName = containerName;
				m_rowScopes = new Hashtable();
				m_columnScopes = new Hashtable();
			}

			internal void RegisterRowGrouping(string groupName)
			{
				Global.Tracer.Assert(groupName != null);
				m_rowScopes[groupName] = null;
			}

			internal void UnRegisterRowGrouping(string groupName)
			{
				Global.Tracer.Assert(groupName != null);
				m_rowScopes.Remove(groupName);
			}

			internal void RegisterColumnGrouping(string groupName)
			{
				Global.Tracer.Assert(groupName != null);
				m_columnScopes[groupName] = null;
			}

			internal void UnRegisterColumnGrouping(string groupName)
			{
				Global.Tracer.Assert(groupName != null);
				m_columnScopes.Remove(groupName);
			}

			private ProcessingErrorCode getErrorCode()
			{
				switch (m_containerType)
				{
				case ObjectType.Matrix:
					return ProcessingErrorCode.rsConflictingRunningValueScopesInMatrix;
				case ObjectType.CustomReportItem:
					return ProcessingErrorCode.rsConflictingRunningValueScopesInTablix;
				case ObjectType.Chart:
					return ProcessingErrorCode.rsConflictingRunningValueScopesInTablix;
				default:
					Global.Tracer.Assert(condition: false, string.Empty);
					return ProcessingErrorCode.rsConflictingRunningValueScopesInMatrix;
				}
			}

			internal bool HasRowColScopeConflict(string textboxSortActionScope, string sortTargetScope, bool sortExpressionScopeIsColumnScope)
			{
				if (HasRowColScopeConflict(textboxSortActionScope, sortExpressionScopeIsColumnScope))
				{
					return true;
				}
				if (sortTargetScope != null)
				{
					return HasRowColScopeConflict(sortTargetScope, sortExpressionScopeIsColumnScope);
				}
				return false;
			}

			private bool HasRowColScopeConflict(string scope, bool sortExpressionScopeIsColumnScope)
			{
				if (sortExpressionScopeIsColumnScope || !m_columnScopes.ContainsKey(scope))
				{
					if (sortExpressionScopeIsColumnScope)
					{
						return m_rowScopes.ContainsKey(scope);
					}
					return false;
				}
				return true;
			}

			internal bool ContainsScope(string scope, ErrorContext errorContext, bool checkConflictingScope)
			{
				Global.Tracer.Assert(scope != null, "(null != scope)");
				if (m_rowScopes.ContainsKey(scope))
				{
					if (checkConflictingScope)
					{
						if (m_columnScopeFound)
						{
							errorContext.Register(getErrorCode(), Severity.Error, m_containerType, m_containerName, null);
						}
						m_rowScopeFound = true;
					}
					return true;
				}
				if (m_columnScopes.ContainsKey(scope))
				{
					if (checkConflictingScope)
					{
						if (m_rowScopeFound)
						{
							errorContext.Register(getErrorCode(), Severity.Error, m_containerType, m_containerName, null);
						}
						m_columnScopeFound = true;
					}
					return true;
				}
				return false;
			}

			internal bool IsRunningValueDirectionColumn()
			{
				return m_columnScopeFound;
			}
		}

		private ICatalogItemContext m_reportContext;

		private LocationFlags m_location;

		private ObjectType m_objectType;

		private string m_objectName;

		private ObjectType m_detailObjectType;

		private string m_matrixName;

		private EmbeddedImageHashtable m_embeddedImages;

		private ImageStreamNames m_imageStreamNames;

		private ErrorContext m_errorContext;

		private Hashtable m_parameters;

		private ArrayList m_dynamicParameters;

		private Hashtable m_dataSetQueryInfo;

		private ExprHostBuilder m_exprHostBuilder;

		private Report m_report;

		private StringList m_aggregateEscalateScopes;

		private Hashtable m_aggregateRewriteScopes;

		private Hashtable m_aggregateRewriteMap;

		private int m_dataRegionCount;

		private string m_outerGroupName;

		private string m_currentGroupName;

		private string m_currentDataregionName;

		private RunningValueInfoList m_runningValues;

		private Hashtable m_groupingScopesForRunningValues;

		private GroupingScopesForTablix m_groupingScopesForRunningValuesInTablix;

		private Hashtable m_dataregionScopesForRunningValues;

		private bool m_hasFilters;

		private ScopeInfo m_currentScope;

		private ScopeInfo m_outermostDataregionScope;

		private Hashtable m_groupingScopes;

		private Hashtable m_dataregionScopes;

		private Hashtable m_datasetScopes;

		private int m_numberOfDataSets;

		private string m_oneDataSetName;

		private string m_currentDataSetName;

		private Hashtable m_fieldNameMap;

		private Hashtable m_dataSetNameToDataRegionsMap;

		private StringDictionary m_dataSources;

		private Hashtable m_reportItemsInScope;

		private Hashtable m_toggleItemInfos;

		private bool m_registerReceiver;

		private CultureInfo m_reportLanguage;

		private bool m_reportDataElementStyleAttribute;

		private bool m_tableColumnVisible;

		private bool m_hasUserSortPeerScopes;

		private Hashtable m_userSortExpressionScopes;

		private Hashtable m_userSortTextboxes;

		private Hashtable m_peerScopes;

		private int m_lastPeerScopeId;

		private Hashtable m_reportScopes;

		private Hashtable m_reportScopeDatasetIDs;

		private GroupingList m_groupingList;

		private Hashtable m_reportGroupingLists;

		private Hashtable m_scopesInMatrixCells;

		private StringList m_parentMatrixList;

		private TextBoxList m_reportSortFilterTextboxes;

		private TextBoxList m_detailSortExpressionScopeTextboxes;

		internal ICatalogItemContext ReportContext => m_reportContext;

		internal LocationFlags Location
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

		internal ObjectType ObjectType
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

		internal bool TableColumnVisible
		{
			get
			{
				return m_tableColumnVisible;
			}
			set
			{
				m_tableColumnVisible = value;
			}
		}

		internal ObjectType DetailObjectType
		{
			set
			{
				m_detailObjectType = value;
			}
		}

		internal string MatrixName
		{
			get
			{
				return m_matrixName;
			}
			set
			{
				m_matrixName = value;
			}
		}

		internal EmbeddedImageHashtable EmbeddedImages => m_embeddedImages;

		internal ImageStreamNames ImageStreamNames => m_imageStreamNames;

		internal ErrorContext ErrorContext => m_errorContext;

		internal bool RegisterHiddenReceiver
		{
			get
			{
				return m_registerReceiver;
			}
			set
			{
				m_registerReceiver = value;
			}
		}

		internal ExprHostBuilder ExprHostBuilder => m_exprHostBuilder;

		internal bool MergeOnePass => m_report.MergeOnePass;

		internal int DataRegionCount => m_dataRegionCount;

		internal CultureInfo ReportLanguage => m_reportLanguage;

		internal StringList AggregateEscalateScopes
		{
			get
			{
				return m_aggregateEscalateScopes;
			}
			set
			{
				m_aggregateEscalateScopes = value;
			}
		}

		internal Hashtable AggregateRewriteScopes
		{
			get
			{
				return m_aggregateRewriteScopes;
			}
			set
			{
				m_aggregateRewriteScopes = value;
			}
		}

		internal Hashtable AggregateRewriteMap
		{
			get
			{
				return m_aggregateRewriteMap;
			}
			set
			{
				m_aggregateRewriteMap = value;
			}
		}

		internal InitializationContext(ICatalogItemContext reportContext, bool hasFilters, StringDictionary dataSources, DataSetList dataSets, ArrayList dynamicParameters, Hashtable dataSetQueryInfo, ErrorContext errorContext, ExprHostBuilder exprHostBuilder, Report report, CultureInfo reportLanguage, Hashtable reportScopes, bool hasUserSortPeerScopes, int dataRegionCount)
		{
			Global.Tracer.Assert(dataSets != null, "(null != dataSets)");
			Global.Tracer.Assert(errorContext != null, "(null != errorContext)");
			m_reportContext = reportContext;
			m_location = LocationFlags.None;
			m_objectType = ObjectType.Report;
			m_objectName = null;
			m_detailObjectType = ObjectType.Report;
			m_matrixName = null;
			m_embeddedImages = report.EmbeddedImages;
			m_imageStreamNames = report.ImageStreamNames;
			m_errorContext = errorContext;
			m_parameters = null;
			m_dynamicParameters = dynamicParameters;
			m_dataSetQueryInfo = dataSetQueryInfo;
			m_registerReceiver = true;
			m_exprHostBuilder = exprHostBuilder;
			m_dataSources = dataSources;
			m_report = report;
			m_aggregateEscalateScopes = null;
			m_aggregateRewriteScopes = null;
			m_aggregateRewriteMap = null;
			m_reportLanguage = reportLanguage;
			m_outerGroupName = null;
			m_currentGroupName = null;
			m_currentDataregionName = null;
			m_runningValues = null;
			m_groupingScopesForRunningValues = new Hashtable();
			m_groupingScopesForRunningValuesInTablix = null;
			m_dataregionScopesForRunningValues = new Hashtable();
			m_hasFilters = hasFilters;
			m_currentScope = null;
			m_outermostDataregionScope = null;
			m_groupingScopes = new Hashtable();
			m_dataregionScopes = new Hashtable();
			m_datasetScopes = new Hashtable();
			for (int i = 0; i < dataSets.Count; i++)
			{
				m_datasetScopes[dataSets[i].Name] = new ScopeInfo(allowCustomAggregates: true, dataSets[i].Aggregates, dataSets[i].PostSortAggregates, dataSets[i]);
			}
			m_numberOfDataSets = dataSets.Count;
			m_oneDataSetName = ((1 == dataSets.Count) ? dataSets[0].Name : null);
			m_currentDataSetName = null;
			m_fieldNameMap = new Hashtable();
			m_dataSetNameToDataRegionsMap = new Hashtable();
			bool flag = false;
			if (m_dynamicParameters != null && m_dynamicParameters.Count > 0)
			{
				flag = true;
			}
			for (int j = 0; j < dataSets.Count; j++)
			{
				DataSet dataSet = dataSets[j];
				Global.Tracer.Assert(dataSet != null, "(null != dataSet)");
				Global.Tracer.Assert(dataSet.Query != null, "(null != dataSet.Query)");
				bool flag2 = false;
				if (report.DataSources != null)
				{
					for (int k = 0; k < report.DataSources.Count; k++)
					{
						if (dataSet.Query.DataSourceName == report.DataSources[k].Name)
						{
							flag2 = true;
							if (report.DataSources[k].DataSets == null)
							{
								report.DataSources[k].DataSets = new DataSetList();
							}
							if (flag)
							{
								YukonDataSetInfo yukonDataSetInfo = (YukonDataSetInfo)dataSetQueryInfo[dataSet.Name];
								Global.Tracer.Assert(yukonDataSetInfo != null, "(null != dataSetInfo)");
								yukonDataSetInfo.DataSourceIndex = k;
								yukonDataSetInfo.DataSetIndex = report.DataSources[k].DataSets.Count;
								yukonDataSetInfo.MergeFlagsFromDataSource(report.DataSources[k].IsComplex, report.DataSources[k].ParameterNames);
							}
							report.DataSources[k].DataSets.Add(dataSet);
							break;
						}
					}
				}
				if (!flag2)
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidDataSourceReference, Severity.Error, dataSet.ObjectType, dataSet.Name, "DataSourceName", dataSet.Query.DataSourceName);
				}
				Hashtable hashtable = new Hashtable();
				if (dataSet.Fields != null)
				{
					for (int l = 0; l < dataSet.Fields.Count; l++)
					{
						hashtable[dataSet.Fields[l].Name] = l;
					}
				}
				m_fieldNameMap[dataSet.Name] = hashtable;
				m_dataSetNameToDataRegionsMap[dataSet.Name] = dataSet.DataRegions;
			}
			if (report.Parameters != null)
			{
				m_parameters = new Hashtable();
				for (int m = 0; m < report.Parameters.Count; m++)
				{
					ParameterDef parameterDef = report.Parameters[m];
					if (parameterDef != null)
					{
						try
						{
							m_parameters.Add(parameterDef.Name, parameterDef);
						}
						catch
						{
						}
					}
				}
			}
			m_reportItemsInScope = new Hashtable();
			m_toggleItemInfos = new Hashtable();
			m_reportDataElementStyleAttribute = true;
			m_tableColumnVisible = true;
			m_hasUserSortPeerScopes = hasUserSortPeerScopes;
			m_userSortExpressionScopes = new Hashtable();
			m_userSortTextboxes = new Hashtable();
			m_peerScopes = (hasUserSortPeerScopes ? new Hashtable() : null);
			m_lastPeerScopeId = 0;
			m_reportScopes = reportScopes;
			m_reportScopeDatasetIDs = new Hashtable();
			m_groupingList = new GroupingList();
			m_scopesInMatrixCells = new Hashtable();
			m_parentMatrixList = new StringList();
			m_reportSortFilterTextboxes = new TextBoxList();
			m_detailSortExpressionScopeTextboxes = new TextBoxList();
			m_reportGroupingLists = new Hashtable();
			m_reportGroupingLists.Add("0_ReportScope", m_groupingList.Clone());
			m_dataRegionCount = dataRegionCount;
		}

		private void RegisterDataSetScope(string scopeName, DataAggregateInfoList scopeAggregates, DataAggregateInfoList scopePostSortAggregates)
		{
			Global.Tracer.Assert(scopeName != null, "(null != scopeName)");
			Global.Tracer.Assert(scopeAggregates != null, "(null != scopeAggregates)");
			Global.Tracer.Assert(scopePostSortAggregates != null, "(null != scopePostSortAggregates)");
			m_currentScope = new ScopeInfo(allowCustomAggregates: true, scopeAggregates, scopePostSortAggregates);
			if (!m_reportGroupingLists.ContainsKey(scopeName))
			{
				m_reportGroupingLists.Add(scopeName, GetGroupingList());
				m_reportScopeDatasetIDs.Add(scopeName, GetDataSetID());
			}
		}

		private void UnRegisterDataSetScope(string scopeName)
		{
			Global.Tracer.Assert(scopeName != null, "(null != scopeName)");
			m_currentScope = null;
		}

		private void RegisterDataRegionScope(DataRegion dataRegion)
		{
			Global.Tracer.Assert(dataRegion.Name != null, "(null != dataRegion.Name)");
			Global.Tracer.Assert(dataRegion.Aggregates != null, "(null != dataRegion.Aggregates)");
			Global.Tracer.Assert(dataRegion.PostSortAggregates != null, "(null != dataRegion.PostSortAggregates)");
			m_currentDataregionName = dataRegion.Name;
			m_dataregionScopesForRunningValues[dataRegion.Name] = m_currentGroupName;
			ScopeInfo scopeInfo = m_currentScope = new ScopeInfo(m_currentScope == null || m_currentScope.AllowCustomAggregates, dataRegion.Aggregates, dataRegion.PostSortAggregates, dataRegion);
			if ((m_location & LocationFlags.InDataRegion) == 0)
			{
				m_outermostDataregionScope = scopeInfo;
			}
			m_dataregionScopes[dataRegion.Name] = scopeInfo;
			if (dataRegion is Matrix)
			{
				m_parentMatrixList.Add(dataRegion.Name);
			}
			if (!m_reportGroupingLists.ContainsKey(dataRegion.Name))
			{
				m_reportGroupingLists.Add(dataRegion.Name, GetGroupingList());
				m_reportScopeDatasetIDs.Add(dataRegion.Name, GetDataSetID());
			}
			if ((LocationFlags)0 < (m_location & LocationFlags.InMatrixCell))
			{
				RegisterScopeInMatrixCell(m_matrixName, dataRegion.Name, registerMatrixCellScope: false);
			}
			ProcessUserSortInnerScope(dataRegion.Name, isMatrixGroup: false, isMatrixColumnGroup: false);
		}

		private void UnRegisterDataRegionScope(string scopeName)
		{
			Global.Tracer.Assert(scopeName != null, "(null != scopeName)");
			m_currentDataregionName = null;
			m_dataregionScopesForRunningValues.Remove(scopeName);
			m_currentScope = null;
			if ((m_location & LocationFlags.InDataRegion) == 0)
			{
				m_outermostDataregionScope = null;
			}
			m_dataregionScopes.Remove(scopeName);
			int count = m_parentMatrixList.Count;
			if (0 < count && ReportProcessing.CompareWithInvariantCulture(m_parentMatrixList[count - 1], scopeName, ignoreCase: false) == 0)
			{
				m_parentMatrixList.RemoveAt(count - 1);
			}
			ValidateUserSortInnerScope(scopeName);
			TextboxesWithDetailSortExpressionInitialize();
		}

		internal void RegisterGroupingScope(string scopeName, bool simpleGroupExpressions, DataAggregateInfoList scopeAggregates, DataAggregateInfoList scopePostSortAggregates, DataAggregateInfoList scopeRecursiveAggregates, Grouping groupingScope)
		{
			RegisterGroupingScope(scopeName, simpleGroupExpressions, scopeAggregates, scopePostSortAggregates, scopeRecursiveAggregates, groupingScope, isMatrixGrouping: false);
		}

		internal void RegisterGroupingScope(string scopeName, bool simpleGroupExpressions, DataAggregateInfoList scopeAggregates, DataAggregateInfoList scopePostSortAggregates, DataAggregateInfoList scopeRecursiveAggregates, Grouping groupingScope, bool isMatrixGrouping)
		{
			Global.Tracer.Assert(scopeName != null);
			Global.Tracer.Assert(scopeAggregates != null);
			Global.Tracer.Assert(scopePostSortAggregates != null);
			Global.Tracer.Assert(scopeRecursiveAggregates != null);
			m_outerGroupName = m_currentGroupName;
			m_currentGroupName = scopeName;
			m_groupingScopesForRunningValues[scopeName] = null;
			ScopeInfo value = m_currentScope = new ScopeInfo((m_currentScope == null) ? simpleGroupExpressions : (simpleGroupExpressions && m_currentScope.AllowCustomAggregates), scopeAggregates, scopePostSortAggregates, scopeRecursiveAggregates, groupingScope);
			m_groupingScopes[scopeName] = value;
			m_groupingList.Add(groupingScope);
			if (!m_reportGroupingLists.ContainsKey(scopeName))
			{
				m_reportGroupingLists.Add(scopeName, GetGroupingList());
				m_reportScopeDatasetIDs.Add(scopeName, GetDataSetID());
				if ((LocationFlags)0 < (m_location & LocationFlags.InMatrixCell))
				{
					RegisterScopeInMatrixCell(m_matrixName, scopeName, registerMatrixCellScope: false);
				}
			}
			if (!isMatrixGrouping)
			{
				ProcessUserSortInnerScope(scopeName, isMatrixGroup: false, isMatrixColumnGroup: false);
			}
		}

		internal void UnRegisterGroupingScope(string scopeName)
		{
			UnRegisterGroupingScope(scopeName, isMatrixGrouping: false);
		}

		internal void UnRegisterGroupingScope(string scopeName, bool isMatrixGrouping)
		{
			Global.Tracer.Assert(scopeName != null);
			m_outerGroupName = null;
			m_currentGroupName = null;
			m_groupingScopesForRunningValues.Remove(scopeName);
			m_currentScope = null;
			m_groupingScopes.Remove(scopeName);
			Global.Tracer.Assert(0 < m_groupingList.Count, "(0 < m_groupingList.Count)");
			m_groupingList.RemoveAt(m_groupingList.Count - 1);
			if (!isMatrixGrouping)
			{
				ValidateUserSortInnerScope(scopeName);
				TextboxesWithDetailSortExpressionInitialize();
			}
		}

		internal void ValidateHideDuplicateScope(string hideDuplicateScope, ReportItem reportItem)
		{
			if (hideDuplicateScope == null)
			{
				return;
			}
			bool flag = true;
			ScopeInfo scopeInfo = null;
			if ((m_location & LocationFlags.InDetail) == 0 && hideDuplicateScope.Equals(m_currentGroupName))
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

		internal void RegisterGroupingScopeForTablixCell(string scopeName, bool column, bool simpleGroupExpressions, DataAggregateInfoList scopeAggregates, DataAggregateInfoList scopePostSortAggregates, DataAggregateInfoList scopeRecursiveAggregates, Grouping groupingScope)
		{
			Global.Tracer.Assert(scopeName != null);
			Global.Tracer.Assert(scopeAggregates != null);
			Global.Tracer.Assert(scopePostSortAggregates != null);
			Global.Tracer.Assert(scopeRecursiveAggregates != null);
			if (column)
			{
				m_groupingScopesForRunningValuesInTablix.RegisterColumnGrouping(scopeName);
			}
			else
			{
				m_groupingScopesForRunningValuesInTablix.RegisterRowGrouping(scopeName);
			}
			ScopeInfo value = new ScopeInfo((m_currentScope == null) ? simpleGroupExpressions : (simpleGroupExpressions && m_currentScope.AllowCustomAggregates), scopeAggregates, scopePostSortAggregates, scopeRecursiveAggregates, groupingScope);
			m_groupingScopes[scopeName] = value;
		}

		internal void UnRegisterGroupingScopeForTablixCell(string scopeName, bool column)
		{
			Global.Tracer.Assert(scopeName != null);
			if (column)
			{
				m_groupingScopesForRunningValuesInTablix.UnRegisterColumnGrouping(scopeName);
			}
			else
			{
				m_groupingScopesForRunningValuesInTablix.UnRegisterRowGrouping(scopeName);
			}
			m_groupingScopes.Remove(scopeName);
		}

		internal void RegisterTablixCellScope(bool forceRows, DataAggregateInfoList scopeAggregates, DataAggregateInfoList scopePostSortAggregates)
		{
			Global.Tracer.Assert(scopeAggregates != null);
			m_groupingScopesForRunningValues = new Hashtable();
			m_groupingScopesForRunningValuesInTablix = new GroupingScopesForTablix(forceRows, m_objectType, m_objectName);
			m_dataregionScopesForRunningValues = new Hashtable();
			m_currentScope = new ScopeInfo(m_currentScope == null || m_currentScope.AllowCustomAggregates, scopeAggregates, scopePostSortAggregates);
		}

		internal void UnRegisterTablixCellScope()
		{
			m_groupingScopesForRunningValues = null;
			m_groupingScopesForRunningValuesInTablix = null;
			m_dataregionScopesForRunningValues = null;
			m_currentScope = null;
		}

		internal void RegisterPageSectionScope(DataAggregateInfoList scopeAggregates)
		{
			Global.Tracer.Assert(scopeAggregates != null);
			m_currentScope = new ScopeInfo(allowCustomAggregates: false, scopeAggregates);
		}

		internal void UnRegisterPageSectionScope()
		{
			m_currentScope = null;
		}

		internal void RegisterRunningValues(RunningValueInfoList runningValues)
		{
			Global.Tracer.Assert(runningValues != null);
			m_runningValues = runningValues;
		}

		internal void UnRegisterRunningValues(RunningValueInfoList runningValues)
		{
			Global.Tracer.Assert(runningValues != null);
			Global.Tracer.Assert(m_runningValues != null);
			Global.Tracer.Assert(m_runningValues == runningValues);
			m_runningValues = null;
		}

		internal void TransferGroupExpressionRowNumbers(RunningValueInfoList rowNumbers)
		{
			if (rowNumbers == null)
			{
				return;
			}
			for (int num = rowNumbers.Count - 1; num >= 0; num--)
			{
				Global.Tracer.Assert((m_location & LocationFlags.InGrouping) != 0);
				RunningValueInfo runningValueInfo = rowNumbers[num];
				Global.Tracer.Assert(runningValueInfo != null);
				string scope = runningValueInfo.Scope;
				bool flag = true;
				ScopeInfo scopeInfo = null;
				if ((m_location & LocationFlags.InMatrixCell) != 0)
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
				else if (m_currentDataregionName == scope)
				{
					Global.Tracer.Assert(m_currentDataregionName != null, "(null != m_currentDataregionName)");
					scopeInfo = (ScopeInfo)m_dataregionScopes[m_currentDataregionName];
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

		internal bool IsRunningValueDirectionColumn()
		{
			return m_groupingScopesForRunningValuesInTablix.IsRunningValueDirectionColumn();
		}

		internal void TransferRunningValues(RunningValueInfoList runningValues, string propertyName)
		{
			TransferRunningValues(runningValues, m_objectType, m_objectName, propertyName);
		}

		internal void TransferRunningValues(RunningValueInfoList runningValues, ObjectType objectType, string objectName, string propertyName)
		{
			if (runningValues == null || (m_location & LocationFlags.InPageSection) != 0)
			{
				return;
			}
			for (int num = runningValues.Count - 1; num >= 0; num--)
			{
				RunningValueInfo runningValueInfo = runningValues[num];
				Global.Tracer.Assert(runningValueInfo != null, "(null != runningValue)");
				string scope = runningValueInfo.Scope;
				bool flag = true;
				string text = null;
				DataAggregateInfoList dataAggregateInfoList = null;
				RunningValueInfoList runningValueInfoList = null;
				if (scope == null)
				{
					if ((m_location & LocationFlags.InDataRegion) == 0)
					{
						flag = false;
					}
					else if ((m_location & LocationFlags.InMatrixCell) != 0)
					{
						flag = false;
					}
					else if ((m_location & LocationFlags.InGrouping) != 0 || (m_location & LocationFlags.InDetail) != 0)
					{
						text = GetDataSetName();
						runningValueInfoList = m_runningValues;
					}
					else
					{
						text = GetDataSetName();
						if (text != null)
						{
							ScopeInfo scopeInfo = (ScopeInfo)m_datasetScopes[text];
							Global.Tracer.Assert(scopeInfo != null, "(null != destinationScope)");
							dataAggregateInfoList = scopeInfo.Aggregates;
						}
					}
				}
				else if (m_groupingScopesForRunningValuesInTablix != null && m_groupingScopesForRunningValuesInTablix.ContainsScope(scope, m_errorContext, checkConflictingScope: true))
				{
					Global.Tracer.Assert((m_location & LocationFlags.InMatrixCell) != 0);
					text = GetDataSetName();
					runningValueInfoList = m_runningValues;
				}
				else if (m_groupingScopesForRunningValues.ContainsKey(scope))
				{
					Global.Tracer.Assert((m_location & LocationFlags.InGrouping) != 0, "(0 != (m_location & LocationFlags.InGrouping))");
					text = GetDataSetName();
					runningValueInfoList = m_runningValues;
				}
				else if (m_dataregionScopesForRunningValues.ContainsKey(scope))
				{
					Global.Tracer.Assert((m_location & LocationFlags.InDataRegion) != 0, "(0 != (m_location & LocationFlags.InDataRegion))");
					runningValueInfo.Scope = (string)m_dataregionScopesForRunningValues[scope];
					if ((m_location & LocationFlags.InGrouping) != 0 || (m_location & LocationFlags.InDetail) != 0)
					{
						text = GetDataSetName();
						runningValueInfoList = m_runningValues;
					}
					else
					{
						text = GetDataSetName();
						if (text != null)
						{
							ScopeInfo scopeInfo2 = (ScopeInfo)m_datasetScopes[text];
							Global.Tracer.Assert(scopeInfo2 != null, "(null != destinationScope)");
							dataAggregateInfoList = scopeInfo2.Aggregates;
						}
					}
				}
				else if (m_datasetScopes.ContainsKey(scope))
				{
					if (((m_location & LocationFlags.InGrouping) != 0 || (m_location & LocationFlags.InDetail) != 0) && scope == GetDataSetName())
					{
						if ((m_location & LocationFlags.InMatrixCell) != 0)
						{
							flag = false;
						}
						else
						{
							text = scope;
							runningValueInfo.Scope = null;
							runningValueInfoList = m_runningValues;
						}
					}
					else
					{
						text = scope;
						ScopeInfo scopeInfo3 = (ScopeInfo)m_datasetScopes[scope];
						Global.Tracer.Assert(scopeInfo3 != null, "(null != destinationScope)");
						dataAggregateInfoList = scopeInfo3.Aggregates;
					}
				}
				else
				{
					flag = false;
				}
				if (!flag)
				{
					if (!runningValueInfo.SuppressExceptions)
					{
						if ((m_location & LocationFlags.InMatrixCell) != 0)
						{
							if (DataAggregateInfo.AggregateTypes.Previous == runningValueInfo.AggregateType)
							{
								m_errorContext.Register(ProcessingErrorCode.rsInvalidPreviousAggregateInMatrixCell, Severity.Error, objectType, objectName, propertyName, m_matrixName);
							}
							else
							{
								m_errorContext.Register(ProcessingErrorCode.rsInvalidScopeInMatrix, Severity.Error, objectType, objectName, propertyName, m_matrixName);
							}
						}
						else
						{
							m_errorContext.Register(ProcessingErrorCode.rsInvalidAggregateScope, Severity.Error, objectType, objectName, propertyName);
						}
					}
				}
				else
				{
					if (runningValueInfo.Expressions != null)
					{
						for (int i = 0; i < runningValueInfo.Expressions.Length; i++)
						{
							Global.Tracer.Assert(runningValueInfo.Expressions[i] != null, "(null != runningValue.Expressions[j])");
							runningValueInfo.Expressions[i].AggregateInitialize(text, objectType, objectName, propertyName, this);
						}
					}
					if (dataAggregateInfoList != null)
					{
						dataAggregateInfoList.Add(runningValueInfo);
					}
					else if (runningValueInfoList != null)
					{
						Global.Tracer.Assert(runningValues != runningValueInfoList);
						runningValueInfoList.Add(runningValueInfo);
					}
				}
				runningValues.RemoveAt(num);
			}
		}

		internal void SpecialTransferRunningValues(RunningValueInfoList runningValues)
		{
			if (runningValues != null)
			{
				for (int num = runningValues.Count - 1; num >= 0; num--)
				{
					Global.Tracer.Assert(m_runningValues != null, "(null != m_runningValues)");
					Global.Tracer.Assert(runningValues != m_runningValues);
					m_runningValues.Add(runningValues[num]);
					runningValues.RemoveAt(num);
				}
			}
		}

		internal void CopyRunningValues(RunningValueInfoList runningValues, DataAggregateInfoList tablixAggregates)
		{
			Global.Tracer.Assert(runningValues != null);
			Global.Tracer.Assert((m_location & LocationFlags.InMatrixCell) != 0);
			Global.Tracer.Assert(tablixAggregates != null);
			Global.Tracer.Assert(m_groupingScopesForRunningValuesInTablix != null);
			for (int i = 0; i < runningValues.Count; i++)
			{
				RunningValueInfo runningValueInfo = runningValues[i];
				if (runningValueInfo.Scope != null && m_groupingScopesForRunningValuesInTablix.ContainsScope(runningValueInfo.Scope, m_errorContext, checkConflictingScope: false))
				{
					tablixAggregates.Add(runningValueInfo);
				}
			}
		}

		internal void TransferAggregates(DataAggregateInfoList aggregates, string propertyName)
		{
			TransferAggregates(aggregates, m_objectType, m_objectName, propertyName);
		}

		internal void TransferAggregates(DataAggregateInfoList aggregates, ObjectType objectType, string objectName, string propertyName)
		{
			if (aggregates == null)
			{
				return;
			}
			for (int num = aggregates.Count - 1; num >= 0; num--)
			{
				DataAggregateInfo dataAggregateInfo = aggregates[num];
				Global.Tracer.Assert(dataAggregateInfo != null, "(null != aggregate)");
				if (m_hasFilters && DataAggregateInfo.AggregateTypes.Aggregate == dataAggregateInfo.AggregateType && !dataAggregateInfo.SuppressExceptions)
				{
					m_errorContext.Register(ProcessingErrorCode.rsCustomAggregateAndFilter, Severity.Error, objectType, objectName, propertyName);
				}
				string scope;
				bool scope2 = dataAggregateInfo.GetScope(out scope);
				bool flag = true;
				string text = null;
				ScopeInfo scopeInfo = null;
				if ((m_location & LocationFlags.InPageSection) != 0 && scope2)
				{
					flag = false;
					if (!dataAggregateInfo.SuppressExceptions)
					{
						m_errorContext.Register(ProcessingErrorCode.rsScopeInPageSectionExpression, Severity.Error, objectType, objectName, propertyName);
					}
				}
				else if ((m_location & LocationFlags.InPageSection) == 0 && m_numberOfDataSets == 0)
				{
					flag = false;
					if (!dataAggregateInfo.SuppressExceptions)
					{
						m_errorContext.Register(ProcessingErrorCode.rsInvalidAggregateScope, Severity.Error, objectType, objectName, propertyName);
					}
				}
				else if (!scope2)
				{
					text = GetDataSetName();
					if (LocationFlags.None == m_location)
					{
						if (1 != m_numberOfDataSets)
						{
							flag = false;
							if (!dataAggregateInfo.SuppressExceptions)
							{
								m_errorContext.Register(ProcessingErrorCode.rsMissingAggregateScope, Severity.Error, objectType, objectName, propertyName);
							}
						}
						else if (text != null)
						{
							scopeInfo = (ScopeInfo)m_datasetScopes[text];
						}
					}
					else
					{
						Global.Tracer.Assert((m_location & LocationFlags.InDataSet) != 0 || (m_location & LocationFlags.InPageSection) != 0);
						scopeInfo = m_currentScope;
					}
					if (scopeInfo != null && scopeInfo.DataSetScope != null)
					{
						scopeInfo.DataSetScope.UsedInAggregates = true;
					}
				}
				else if (scope == null)
				{
					flag = false;
				}
				else if (m_groupingScopes.ContainsKey(scope))
				{
					Global.Tracer.Assert((m_location & LocationFlags.InGrouping) != 0, "(0 != (m_location & LocationFlags.InGrouping))");
					text = GetDataSetName();
					scopeInfo = (ScopeInfo)m_groupingScopes[scope];
				}
				else if (m_dataregionScopes.ContainsKey(scope))
				{
					Global.Tracer.Assert((m_location & LocationFlags.InDataRegion) != 0, "(0 != (m_location & LocationFlags.InDataRegion))");
					text = GetDataSetName();
					scopeInfo = (ScopeInfo)m_dataregionScopes[scope];
				}
				else if (m_datasetScopes.ContainsKey(scope))
				{
					text = scope;
					scopeInfo = (ScopeInfo)m_datasetScopes[scope];
					scopeInfo.DataSetScope.UsedInAggregates = true;
				}
				else
				{
					flag = false;
					if (!dataAggregateInfo.SuppressExceptions)
					{
						m_errorContext.Register(ProcessingErrorCode.rsInvalidAggregateScope, Severity.Error, objectType, objectName, propertyName);
					}
				}
				if (flag && scopeInfo != null)
				{
					if (DataAggregateInfo.AggregateTypes.Aggregate == dataAggregateInfo.AggregateType && !scopeInfo.AllowCustomAggregates && !dataAggregateInfo.SuppressExceptions)
					{
						m_errorContext.Register(ProcessingErrorCode.rsInvalidCustomAggregateScope, Severity.Error, objectType, objectName, propertyName);
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
						DataSet dataSet = m_reportScopes[text] as DataSet;
						if (dataSet != null && dataSet.InterpretSubtotalsAsDetailsIsAuto)
						{
							dataSet.InterpretSubtotalsAsDetails = false;
						}
					}
					DataAggregateInfoList dataAggregateInfoList = dataAggregateInfo.Recursive ? ((scopeInfo.GroupingScope != null && scopeInfo.GroupingScope.Parent != null) ? scopeInfo.RecursiveAggregates : scopeInfo.Aggregates) : ((scopeInfo.PostSortAggregates == null || !dataAggregateInfo.IsPostSortAggregate()) ? scopeInfo.Aggregates : scopeInfo.PostSortAggregates);
					Global.Tracer.Assert(dataAggregateInfoList != null, "(null != destinationAggregates)");
					Global.Tracer.Assert(aggregates != dataAggregateInfoList);
					dataAggregateInfoList.Add(dataAggregateInfo);
				}
				aggregates.RemoveAt(num);
			}
		}

		internal string EscalateScope(string oldScope)
		{
			if (m_aggregateRewriteScopes != null && m_aggregateRewriteScopes.ContainsKey(oldScope))
			{
				Global.Tracer.Assert(m_aggregateEscalateScopes != null && 1 <= m_aggregateEscalateScopes.Count);
				return m_aggregateEscalateScopes[m_aggregateEscalateScopes.Count - 1];
			}
			return oldScope;
		}

		internal void InitializeParameters(ParameterDefList parameters, DataSetList dataSetList)
		{
			if (m_dynamicParameters == null || m_dynamicParameters.Count == 0)
			{
				return;
			}
			Hashtable hashtable = new Hashtable();
			DynamicParameter dynamicParameter = null;
			int i = 0;
			for (int j = 0; j < m_dynamicParameters.Count; j++)
			{
				for (dynamicParameter = (DynamicParameter)m_dynamicParameters[j]; i < dynamicParameter.Index; i++)
				{
					hashtable.Add(parameters[i].Name, i);
				}
				InitializeParameter(parameters[dynamicParameter.Index], dynamicParameter, hashtable, dataSetList);
			}
		}

		private void InitializeParameter(ParameterDef parameter, DynamicParameter dynamicParameter, Hashtable dependencies, DataSetList dataSetList)
		{
			Global.Tracer.Assert(dynamicParameter != null, "(null != dynamicParameter)");
			DataSetReference dataSetReference = null;
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

		private void InitializeParameterDataSource(ParameterDef parameter, DataSetReference dataSetRef, bool isDefault, Hashtable dependencies, ref bool isComplex, DataSetList dataSetList)
		{
			ParameterDataSource parameterDataSource = null;
			YukonDataSetInfo yukonDataSetInfo = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			yukonDataSetInfo = (YukonDataSetInfo)m_dataSetQueryInfo[dataSetRef.DataSet];
			if (yukonDataSetInfo == null)
			{
				if (isDefault)
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidDefaultValueDataSetReference, Severity.Error, ObjectType.ReportParameter, parameter.Name, "DataSetReference", dataSetRef.DataSet);
				}
				else
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidValidValuesDataSetReference, Severity.Error, ObjectType.ReportParameter, parameter.Name, "DataSetReference", dataSetRef.DataSet);
				}
			}
			else
			{
				DataSet dataSet = dataSetList[yukonDataSetInfo.DataSetDefIndex];
				if (!dataSet.UsedInAggregates)
				{
					DataRegionList dataRegionList = (DataRegionList)m_dataSetNameToDataRegionsMap[dataSetRef.DataSet];
					if (dataRegionList == null || dataRegionList.Count == 0)
					{
						dataSet.UsedOnlyInParameters = true;
					}
				}
				parameterDataSource = new ParameterDataSource(yukonDataSetInfo.DataSourceIndex, yukonDataSetInfo.DataSetIndex);
				Hashtable hashtable = (Hashtable)m_fieldNameMap[dataSetRef.DataSet];
				if (hashtable != null)
				{
					if (hashtable.ContainsKey(dataSetRef.ValueAlias))
					{
						parameterDataSource.ValueFieldIndex = (int)hashtable[dataSetRef.ValueAlias];
						if (parameterDataSource.ValueFieldIndex >= yukonDataSetInfo.CalculatedFieldIndex)
						{
							flag3 = (dataSet.Fields == null || parameterDataSource.ValueFieldIndex > dataSet.Fields.Count || !(dataSet.Fields[parameterDataSource.ValueFieldIndex].Value is ExpressionInfoExtended) || !((ExpressionInfoExtended)dataSet.Fields[parameterDataSource.ValueFieldIndex].Value).IsExtendedSimpleFieldReference);
						}
						flag = true;
					}
					if (dataSetRef.LabelAlias != null)
					{
						if (hashtable.ContainsKey(dataSetRef.LabelAlias))
						{
							parameterDataSource.LabelFieldIndex = (int)hashtable[dataSetRef.LabelAlias];
							if (parameterDataSource.LabelFieldIndex >= yukonDataSetInfo.CalculatedFieldIndex)
							{
								flag3 = (dataSet.Fields == null || parameterDataSource.LabelFieldIndex > dataSet.Fields.Count || !(dataSet.Fields[parameterDataSource.LabelFieldIndex].Value is ExpressionInfoExtended) || !((ExpressionInfoExtended)dataSet.Fields[parameterDataSource.LabelFieldIndex].Value).IsExtendedSimpleFieldReference);
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
					ErrorContext.Register(ProcessingErrorCode.rsInvalidDataSetReferenceField, Severity.Error, ObjectType.ReportParameter, parameter.Name, "Field", dataSetRef.ValueAlias, dataSetRef.DataSet);
				}
				if (!flag2)
				{
					ErrorContext.Register(ProcessingErrorCode.rsInvalidDataSetReferenceField, Severity.Error, ObjectType.ReportParameter, parameter.Name, "Field", dataSetRef.LabelAlias, dataSetRef.DataSet);
				}
				if (!isComplex)
				{
					if (yukonDataSetInfo.IsComplex || flag3)
					{
						isComplex = true;
						parameter.Dependencies = (Hashtable)dependencies.Clone();
					}
					else if (yukonDataSetInfo.ParameterNames != null && yukonDataSetInfo.ParameterNames.Count != 0)
					{
						string text = null;
						Hashtable hashtable2 = parameter.Dependencies;
						if (hashtable2 == null)
						{
							hashtable2 = (parameter.Dependencies = new Hashtable());
						}
						for (int i = 0; i < yukonDataSetInfo.ParameterNames.Count; i++)
						{
							text = yukonDataSetInfo.ParameterNames[i];
							if (dependencies.ContainsKey(text))
							{
								if (!hashtable2.ContainsKey(text))
								{
									hashtable2.Add(text, dependencies[text]);
								}
							}
							else
							{
								ErrorContext.Register(ProcessingErrorCode.rsInvalidReportParameterDependency, Severity.Error, ObjectType.ReportParameter, parameter.Name, "DataSetReference", text);
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

		internal void MergeFieldPropertiesIntoDataset(ExpressionInfo expressionInfo)
		{
			if (expressionInfo.ReferencedFieldProperties != null || expressionInfo.DynamicFieldReferences)
			{
				string dataSetName = GetDataSetName();
				if (dataSetName != null)
				{
					(m_reportScopes[dataSetName] as DataSet)?.MergeFieldProperties(expressionInfo);
				}
			}
		}

		internal void RegisterDataRegion(DataRegion dataRegion)
		{
			if (m_numberOfDataSets == 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsDataRegionWithoutDataSet, Severity.Error, m_objectType, m_objectName, null);
			}
			if ((m_location & LocationFlags.InDetail) != 0 && ObjectType.List == m_detailObjectType)
			{
				m_errorContext.Register(ProcessingErrorCode.rsDataRegionInDetailList, Severity.Error, m_objectType, m_objectName, null);
			}
			if ((m_location & LocationFlags.InDataRegion) == 0)
			{
				ValidateDataSetNameForTopLevelDataRegion(dataRegion.DataSetName);
				string dataSetName = GetDataSetName();
				if (dataSetName != null)
				{
					DataRegionList dataRegionList = (DataRegionList)m_dataSetNameToDataRegionsMap[dataSetName];
					Global.Tracer.Assert(dataRegionList != null, "(null != dataRegions)");
					dataRegionList.Add(dataRegion);
					ScopeInfo scopeInfo = (ScopeInfo)m_datasetScopes[dataSetName];
					Global.Tracer.Assert(scopeInfo != null, "(null != dataSetScope)");
					RegisterDataSetScope(dataSetName, scopeInfo.Aggregates, scopeInfo.PostSortAggregates);
				}
			}
			RegisterDataRegionScope(dataRegion);
		}

		internal void UnRegisterDataRegion(DataRegion dataRegion)
		{
			if ((m_location & LocationFlags.InDataRegion) == 0)
			{
				string dataSetName = GetDataSetName();
				if (dataSetName != null)
				{
					UnRegisterDataSetScope(dataSetName);
				}
			}
			UnRegisterDataRegionScope(dataRegion.Name);
		}

		internal void RegisterDataSet(DataSet dataSet)
		{
			m_currentDataSetName = dataSet.Name;
			RegisterDataSetScope(dataSet.Name, dataSet.Aggregates, dataSet.PostSortAggregates);
		}

		internal void UnRegisterDataSet(DataSet dataSet)
		{
			m_currentDataSetName = null;
			UnRegisterDataSetScope(dataSet.Name);
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
			return m_currentDataSetName;
		}

		private int GetDataSetID()
		{
			string dataSetName = GetDataSetName();
			if (dataSetName == null)
			{
				return -1;
			}
			return (m_reportScopes[dataSetName] as ISortFilterScope)?.ID ?? (-1);
		}

		private void ValidateDataSetNameForTopLevelDataRegion(string dataSetName)
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
				if (dataSetName == null)
				{
					m_errorContext.Register(ProcessingErrorCode.rsMissingDataSetName, Severity.Error, m_objectType, m_objectName, "DataSetName");
				}
				else
				{
					flag = m_fieldNameMap.ContainsKey(dataSetName);
				}
			}
			if (!flag)
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidDataSetName, Severity.Error, m_objectType, m_objectName, "DataSetName", dataSetName);
			}
			else
			{
				m_currentDataSetName = dataSetName;
			}
		}

		internal void CheckFieldReferences(StringList fieldNames, string propertyName)
		{
			InternalCheckFieldReference(fieldNames, GetDataSetName(), m_objectType, m_objectName, propertyName);
		}

		internal void AggregateCheckFieldReferences(StringList fieldNames, string dataSetName, ObjectType objectType, string objectName, string propertyName)
		{
			InternalCheckFieldReference(fieldNames, dataSetName, objectType, objectName, propertyName);
		}

		private void InternalCheckFieldReference(StringList fieldNames, string dataSetName, ObjectType objectType, string objectName, string propertyName)
		{
			if (fieldNames == null || (m_location & LocationFlags.InPageSection) != 0)
			{
				return;
			}
			Hashtable hashtable = null;
			if (dataSetName != null)
			{
				hashtable = (Hashtable)m_fieldNameMap[dataSetName];
			}
			for (int i = 0; i < fieldNames.Count; i++)
			{
				string text = fieldNames[i];
				if (m_numberOfDataSets == 0)
				{
					m_errorContext.Register(ProcessingErrorCode.rsFieldReference, Severity.Error, objectType, objectName, propertyName, text);
					continue;
				}
				Global.Tracer.Assert(1 <= m_numberOfDataSets);
				if (hashtable != null && !hashtable.ContainsKey(text))
				{
					m_errorContext.Register(ProcessingErrorCode.rsFieldReference, Severity.Error, objectType, objectName, propertyName, text);
				}
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
			if (exprInfo != null && exprInfo.Type == ExpressionInfo.Types.Field && (m_location & LocationFlags.InPageSection) == 0 && dataSetName != null)
			{
				Hashtable hashtable = (Hashtable)m_fieldNameMap[dataSetName];
				if (hashtable != null && hashtable.ContainsKey(exprInfo.Value))
				{
					exprInfo.IntValue = (int)hashtable[exprInfo.Value];
				}
			}
		}

		internal void FillInTokenIndex(ExpressionInfo exprInfo)
		{
			if (exprInfo == null || exprInfo.Type != ExpressionInfo.Types.Token)
			{
				return;
			}
			string value = exprInfo.Value;
			if (value != null)
			{
				DataSet dataSet = m_reportScopes[value] as DataSet;
				if (dataSet != null)
				{
					exprInfo.IntValue = dataSet.ID;
				}
			}
		}

		internal void CheckDataSetReference(StringList referencedDataSets, string propertyName)
		{
			InternalCheckDataSetReference(referencedDataSets, m_objectType, m_objectName, propertyName);
		}

		internal void AggregateCheckDataSetReference(StringList referencedDataSets, ObjectType objectType, string objectName, string propertyName)
		{
			InternalCheckDataSetReference(referencedDataSets, objectType, objectName, propertyName);
		}

		private void InternalCheckDataSetReference(StringList dataSetNames, ObjectType objectType, string objectName, string propertyName)
		{
			if (dataSetNames == null || (m_location & LocationFlags.InPageSection) != 0)
			{
				return;
			}
			for (int i = 0; i < dataSetNames.Count; i++)
			{
				if (!m_dataSetNameToDataRegionsMap.ContainsKey(dataSetNames[i]))
				{
					m_errorContext.Register(ProcessingErrorCode.rsDataSetReference, Severity.Error, objectType, objectName, propertyName, dataSetNames[i]);
				}
			}
		}

		internal void CheckDataSourceReference(StringList referencedDataSources, string propertyName)
		{
			InternalCheckDataSourceReference(referencedDataSources, m_objectType, m_objectName, propertyName);
		}

		internal void AggregateCheckDataSourceReference(StringList referencedDataSources, ObjectType objectType, string objectName, string propertyName)
		{
			InternalCheckDataSetReference(referencedDataSources, objectType, objectName, propertyName);
		}

		private void InternalCheckDataSourceReference(StringList dataSourceNames, ObjectType objectType, string objectName, string propertyName)
		{
			if (dataSourceNames == null || (m_location & LocationFlags.InPageSection) != 0)
			{
				return;
			}
			for (int i = 0; i < dataSourceNames.Count; i++)
			{
				if (!m_dataSources.ContainsKey(dataSourceNames[i]))
				{
					m_errorContext.Register(ProcessingErrorCode.rsDataSourceReference, Severity.Error, objectType, objectName, propertyName, dataSourceNames[i]);
				}
			}
		}

		internal int GenerateSubtotalID()
		{
			Global.Tracer.Assert(m_report != null);
			m_report.LastID++;
			return m_report.LastID;
		}

		internal string GenerateAggregateID(string oldAggregateID)
		{
			Global.Tracer.Assert(m_report != null);
			m_report.LastAggregateID++;
			string text = "Aggregate" + m_report.LastAggregateID;
			if (m_aggregateRewriteMap == null)
			{
				m_aggregateRewriteMap = new Hashtable();
			}
			m_aggregateRewriteMap.Add(oldAggregateID, text);
			return text;
		}

		internal void RegisterReportItems(ReportItemCollection reportItems)
		{
			Global.Tracer.Assert(reportItems != null, "(null != reportItems)");
			for (int i = 0; i < reportItems.Count; i++)
			{
				ReportItem reportItem = reportItems[i];
				if (reportItem != null)
				{
					m_reportItemsInScope[reportItem.Name] = reportItem;
					if (reportItem is Rectangle)
					{
						RegisterReportItems(((Rectangle)reportItem).ReportItems);
					}
					if (reportItem is Table)
					{
						((Table)reportItem).RegisterHeaderAndFooter(this);
					}
					if (reportItem is Matrix)
					{
						RegisterReportItems(((Matrix)reportItem).CornerReportItems);
					}
				}
			}
		}

		internal void UnRegisterReportItems(ReportItemCollection reportItems)
		{
			Global.Tracer.Assert(reportItems != null, "(null != reportItems)");
			for (int i = 0; i < reportItems.Count; i++)
			{
				ReportItem reportItem = reportItems[i];
				if (reportItem != null)
				{
					m_reportItemsInScope.Remove(reportItem.Name);
					if (reportItem is Rectangle)
					{
						UnRegisterReportItems(((Rectangle)reportItem).ReportItems);
					}
					if (reportItem is Table)
					{
						((Table)reportItem).UnRegisterHeaderAndFooter(this);
					}
					if (reportItem is Matrix)
					{
						UnRegisterReportItems(((Matrix)reportItem).CornerReportItems);
					}
				}
			}
		}

		internal void CheckReportItemReferences(StringList referencedReportItems, string propertyName)
		{
			InternalCheckReportItemReferences(referencedReportItems, m_objectType, m_objectName, propertyName);
		}

		internal void AggregateCheckReportItemReferences(StringList referencedReportItems, ObjectType objectType, string objectName, string propertyName)
		{
			InternalCheckReportItemReferences(referencedReportItems, objectType, objectName, propertyName);
		}

		private void InternalCheckReportItemReferences(StringList referencedReportItems, ObjectType objectType, string objectName, string propertyName)
		{
			if (referencedReportItems == null || (m_location & LocationFlags.InPageSection) != 0)
			{
				return;
			}
			for (int i = 0; i < referencedReportItems.Count; i++)
			{
				if (!m_reportItemsInScope.ContainsKey(referencedReportItems[i]))
				{
					m_errorContext.Register(ProcessingErrorCode.rsReportItemReference, Severity.Error, objectType, objectName, propertyName, referencedReportItems[i]);
				}
			}
		}

		internal void CheckReportParameterReferences(StringList referencedParameters, string propertyName)
		{
			InternalCheckReportParameterReferences(referencedParameters, m_objectType, m_objectName, propertyName);
		}

		private void InternalCheckReportParameterReferences(StringList referencedParameters, ObjectType objectType, string objectName, string propertyName)
		{
			if (referencedParameters == null)
			{
				return;
			}
			for (int i = 0; i < referencedParameters.Count; i++)
			{
				if (m_parameters == null || !m_parameters.ContainsKey(referencedParameters[i]))
				{
					m_errorContext.Register(ProcessingErrorCode.rsParameterReference, Severity.Error, objectType, objectName, propertyName, referencedParameters[i]);
				}
			}
		}

		internal ToggleItemInfo RegisterReceiver(string senderName, Visibility visibility, bool isContainer)
		{
			if (senderName == null)
			{
				return null;
			}
			if ((m_location & LocationFlags.InPageSection) != 0)
			{
				return null;
			}
			ReportItem reportItem = (ReportItem)m_reportItemsInScope[senderName];
			if (!(reportItem is TextBox))
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidToggleItem, Severity.Error, m_objectType, m_objectName, "Item", senderName);
			}
			else
			{
				((TextBox)reportItem).IsToggle = true;
				do
				{
					reportItem.Computed = true;
					reportItem = reportItem.Parent;
				}
				while (reportItem is Rectangle);
				if (isContainer)
				{
					ToggleItemInfoList toggleItemInfoList = (ToggleItemInfoList)m_toggleItemInfos[senderName];
					if (toggleItemInfoList == null)
					{
						toggleItemInfoList = new ToggleItemInfoList();
						m_toggleItemInfos[senderName] = toggleItemInfoList;
					}
					ToggleItemInfo toggleItemInfo = new ToggleItemInfo();
					toggleItemInfo.ObjectName = m_objectName;
					toggleItemInfo.ObjectType = m_objectType;
					toggleItemInfo.Visibility = visibility;
					toggleItemInfo.GroupName = m_currentGroupName;
					toggleItemInfoList.Add(toggleItemInfo);
					return toggleItemInfo;
				}
			}
			return null;
		}

		internal void UnRegisterReceiver(string senderName, ToggleItemInfo toggleItemInfo)
		{
			Global.Tracer.Assert(toggleItemInfo != null, "(null != toggleItemInfo)");
			((ToggleItemInfoList)m_toggleItemInfos[senderName])?.Remove(toggleItemInfo);
		}

		internal void RegisterSender(TextBox textbox)
		{
			Global.Tracer.Assert(textbox != null);
			ToggleItemInfoList toggleItemInfoList = (ToggleItemInfoList)m_toggleItemInfos[textbox.Name];
			if (toggleItemInfoList == null || 0 >= toggleItemInfoList.Count)
			{
				return;
			}
			bool flag = false;
			ScopeInfo scopeInfo = null;
			if (m_currentGroupName != null)
			{
				scopeInfo = (ScopeInfo)m_groupingScopes[m_currentGroupName];
				Global.Tracer.Assert(scopeInfo != null && scopeInfo.GroupingScope != null);
				if (scopeInfo.GroupingScope.Parent != null)
				{
					flag = true;
				}
			}
			for (int i = 0; i < toggleItemInfoList.Count; i++)
			{
				ToggleItemInfo toggleItemInfo = toggleItemInfoList[i];
				if (flag && toggleItemInfo.GroupName == m_currentGroupName)
				{
					textbox.RecursiveSender = true;
					toggleItemInfo.Visibility.RecursiveReceiver = true;
				}
				else
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidToggleItem, Severity.Error, toggleItemInfo.ObjectType, toggleItemInfo.ObjectName, "Item", textbox.Name);
				}
			}
			m_toggleItemInfos.Remove(textbox.Name);
		}

		internal double ValidateSize(string size, string propertyName)
		{
			PublishingValidator.ValidateSize(size, m_objectType, m_objectName, propertyName, restrictMaxValue: true, m_errorContext, out double sizeInMM, out string _);
			return sizeInMM;
		}

		internal double ValidateSize(ref string size, string propertyName)
		{
			return ValidateSize(ref size, restrictMaxValue: true, propertyName);
		}

		internal double ValidateSize(ref string size, bool restrictMaxValue, string propertyName)
		{
			PublishingValidator.ValidateSize(size, m_objectType, m_objectName, propertyName, restrictMaxValue, m_errorContext, out double sizeInMM, out string roundSize);
			size = roundSize;
			return sizeInMM;
		}

		internal void CheckInternationalSettings(StyleAttributeHashtable styleAttributes)
		{
			if (styleAttributes == null || styleAttributes.Count == 0)
			{
				return;
			}
			CultureInfo culture = null;
			AttributeInfo attributeInfo = styleAttributes["Language"];
			if (attributeInfo == null)
			{
				culture = m_reportLanguage;
			}
			else if (!attributeInfo.IsExpression)
			{
				PublishingValidator.ValidateLanguage(attributeInfo.Value, ObjectType, ObjectName, "Language", ErrorContext, out culture);
			}
			if (culture != null)
			{
				AttributeInfo attributeInfo2 = styleAttributes["Calendar"];
				if (attributeInfo2 != null && !attributeInfo2.IsExpression)
				{
					PublishingValidator.ValidateCalendar(culture, attributeInfo2.Value, ObjectType, ObjectName, "Calendar", ErrorContext);
				}
			}
			attributeInfo = styleAttributes["NumeralLanguage"];
			if (attributeInfo != null)
			{
				if (attributeInfo.IsExpression)
				{
					culture = null;
				}
				else
				{
					PublishingValidator.ValidateLanguage(attributeInfo.Value, ObjectType, ObjectName, "NumeralLanguage", ErrorContext, out culture);
				}
			}
			if (culture != null)
			{
				AttributeInfo attributeInfo3 = styleAttributes["NumeralVariant"];
				if (attributeInfo3 != null && !attributeInfo3.IsExpression)
				{
					PublishingValidator.ValidateNumeralVariant(culture, attributeInfo3.IntValue, ObjectType, ObjectName, "NumeralVariant", ErrorContext);
				}
			}
		}

		internal string GetCurrentScope()
		{
			if (m_currentScope != null)
			{
				if (m_currentScope.GroupingScope != null)
				{
					return m_currentScope.GroupingScope.Name;
				}
				Global.Tracer.Assert(m_currentDataregionName != null, "(null != m_currentDataregionName)");
				return m_currentDataregionName;
			}
			return "0_ReportScope";
		}

		internal bool IsScope(string scope)
		{
			if (scope == null)
			{
				return false;
			}
			return m_reportScopes.ContainsKey(scope);
		}

		internal bool IsAncestorScope(string targetScope, bool inMatrixGrouping, bool checkAllGroupingScopes)
		{
			string dataSetName = GetDataSetName();
			if (dataSetName != null && ReportProcessing.CompareWithInvariantCulture(dataSetName, targetScope, ignoreCase: false) == 0)
			{
				return true;
			}
			if (m_dataregionScopes != null && m_dataregionScopes.ContainsKey(targetScope))
			{
				return true;
			}
			if ((checkAllGroupingScopes || inMatrixGrouping) && m_groupingScopesForRunningValuesInTablix != null && m_groupingScopesForRunningValuesInTablix.ContainsScope(targetScope, null, checkConflictingScope: false))
			{
				return true;
			}
			if ((checkAllGroupingScopes || !inMatrixGrouping) && m_groupingScopesForRunningValues != null && m_groupingScopesForRunningValues.ContainsKey(targetScope))
			{
				return true;
			}
			return false;
		}

		internal bool IsCurrentScope(string targetScope)
		{
			if (m_currentScope != null)
			{
				if (m_currentScope.GroupingScope == null && ReportProcessing.CompareWithInvariantCulture(targetScope, m_currentDataregionName, ignoreCase: false) == 0)
				{
					return true;
				}
				if (m_currentScope.GroupingScope != null && ReportProcessing.CompareWithInvariantCulture(targetScope, m_currentScope.GroupingScope.Name, ignoreCase: false) == 0)
				{
					return true;
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
			string currentScope = GetCurrentScope();
			Global.Tracer.Assert(currentScope != null && m_peerScopes != null, "(null != currentScope && null != m_peerScopes)");
			object obj = m_peerScopes[currentScope];
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
			return m_currentScope == null;
		}

		internal ISortFilterScope GetSortFilterScope()
		{
			return GetSortFilterScope(GetCurrentScope());
		}

		internal ISortFilterScope GetSortFilterScope(string scopeName)
		{
			Global.Tracer.Assert(scopeName != null && "0_ReportScope" != scopeName && m_reportScopes.ContainsKey(scopeName));
			return m_reportScopes[scopeName] as ISortFilterScope;
		}

		internal GroupingList GetGroupingList()
		{
			Global.Tracer.Assert(m_groupingList != null);
			return m_groupingList.Clone();
		}

		internal void RegisterScopeInMatrixCell(string matrixName, string scope, bool registerMatrixCellScope)
		{
			Global.Tracer.Assert(matrixName != null && m_scopesInMatrixCells != null);
			StringList stringList = m_scopesInMatrixCells[matrixName] as StringList;
			if (stringList != null)
			{
				if (!stringList.Contains(scope))
				{
					stringList.Add(scope);
				}
			}
			else
			{
				stringList = new StringList();
				stringList.Add(scope);
				m_scopesInMatrixCells.Add(matrixName, stringList);
			}
			if (registerMatrixCellScope && !m_reportGroupingLists.ContainsKey(scope))
			{
				m_reportGroupingLists.Add(scope, GetGroupingList());
			}
		}

		internal void UpdateScopesInMatrixCells(string matrixName, GroupingList matrixGroups)
		{
			int count = m_groupingList.Count;
			int count2 = m_parentMatrixList.Count;
			Global.Tracer.Assert(1 <= count2 && ReportProcessing.CompareWithInvariantCulture(m_parentMatrixList[count2 - 1], matrixName, ignoreCase: false) == 0);
			string text = null;
			if (1 < count2)
			{
				text = m_parentMatrixList[count2 - 2];
			}
			StringList stringList = m_scopesInMatrixCells[matrixName] as StringList;
			Global.Tracer.Assert(stringList != null);
			int count3 = stringList.Count;
			for (int i = 0; i < count3; i++)
			{
				string text2 = stringList[i];
				Global.Tracer.Assert(text2 != null);
				if (matrixGroups != null)
				{
					GroupingList groupingList = m_reportGroupingLists[text2] as GroupingList;
					Global.Tracer.Assert(groupingList != null && count <= groupingList.Count);
					groupingList.InsertRange(count, matrixGroups);
				}
				if (text != null)
				{
					RegisterScopeInMatrixCell(text, text2, registerMatrixCellScope: false);
				}
			}
			m_scopesInMatrixCells.Remove(matrixName);
		}

		internal void RegisterPeerScopes(ReportItemCollection reportItems)
		{
			RegisterPeerScopes(reportItems, ++m_lastPeerScopeId, traverse: true);
		}

		private void RegisterMatrixPeerScopes(MatrixHeading headings, int scopeID)
		{
			if (headings == null)
			{
				return;
			}
			while (true)
			{
				if (headings != null)
				{
					if (headings.Grouping != null)
					{
						break;
					}
					RegisterPeerScopes(headings.ReportItems, scopeID, traverse: false);
					headings = headings.SubHeading;
					continue;
				}
				return;
			}
			if (headings.Subtotal != null)
			{
				RegisterPeerScopes(headings.Subtotal.ReportItems, scopeID, traverse: false);
			}
		}

		private void RegisterPeerScopes(ReportItemCollection reportItems, int scopeID, bool traverse)
		{
			if (reportItems == null || !m_hasUserSortPeerScopes)
			{
				return;
			}
			string currentScope = GetCurrentScope();
			if (m_peerScopes.ContainsKey(currentScope))
			{
				return;
			}
			int count = reportItems.Count;
			for (int i = 0; i < count; i++)
			{
				ReportItem reportItem = reportItems[i];
				if (reportItem is Rectangle)
				{
					RegisterPeerScopes(((Rectangle)reportItem).ReportItems, scopeID, traverse);
				}
				else if (reportItem is DataRegion && !m_peerScopes.ContainsKey(reportItem.Name))
				{
					m_peerScopes.Add(reportItem.Name, scopeID);
				}
				if (reportItem is CustomReportItem)
				{
					RegisterPeerScopes(((CustomReportItem)reportItem).AltReportItem, scopeID, traverse);
				}
				else
				{
					if (!traverse)
					{
						continue;
					}
					if (reportItem is Matrix)
					{
						RegisterPeerScopes(((Matrix)reportItem).CornerReportItems, scopeID, traverse: false);
						RegisterMatrixPeerScopes(((Matrix)reportItem).Columns, scopeID);
						RegisterMatrixPeerScopes(((Matrix)reportItem).Rows, scopeID);
					}
					else
					{
						if (!(reportItem is Table))
						{
							continue;
						}
						Table table = reportItem as Table;
						if (table.HeaderRows != null)
						{
							int count2 = table.HeaderRows.Count;
							for (int j = 0; j < count2; j++)
							{
								RegisterPeerScopes(table.HeaderRows[j].ReportItems, scopeID, traverse: false);
							}
						}
						if (table.FooterRows != null)
						{
							int count2 = table.FooterRows.Count;
							for (int k = 0; k < count2; k++)
							{
								RegisterPeerScopes(table.FooterRows[k].ReportItems, scopeID, traverse: false);
							}
						}
					}
				}
			}
			if (!m_peerScopes.ContainsKey(currentScope))
			{
				m_peerScopes.Add(currentScope, scopeID);
			}
		}

		internal void RegisterUserSortInnerScope(TextBox textbox)
		{
			Global.Tracer.Assert(textbox.UserSort != null && textbox.UserSort.SortExpressionScopeString != null && m_userSortExpressionScopes != null && m_userSortTextboxes != null);
			string currentScope = GetCurrentScope();
			TextBoxList textBoxList = m_userSortExpressionScopes[textbox.UserSort.SortExpressionScopeString] as TextBoxList;
			if (textBoxList != null)
			{
				if (!textBoxList.Contains(textbox))
				{
					textBoxList.Add(textbox);
				}
			}
			else
			{
				textBoxList = new TextBoxList();
				textBoxList.Add(textbox);
				m_userSortExpressionScopes.Add(textbox.UserSort.SortExpressionScopeString, textBoxList);
			}
			textBoxList = (m_userSortTextboxes[currentScope] as TextBoxList);
			if (textBoxList != null)
			{
				if (!textBoxList.Contains(textbox))
				{
					textBoxList.Add(textbox);
				}
			}
			else
			{
				textBoxList = new TextBoxList();
				textBoxList.Add(textbox);
				m_userSortTextboxes.Add(currentScope, textBoxList);
			}
		}

		internal void ProcessUserSortInnerScope(string scopeName, bool isMatrixGroup, bool isMatrixColumnGroup)
		{
			TextBoxList textBoxList = m_userSortExpressionScopes[scopeName] as TextBoxList;
			if (textBoxList == null)
			{
				return;
			}
			int count = textBoxList.Count;
			for (int i = 0; i < count; i++)
			{
				TextBox textBox = textBoxList[i];
				Global.Tracer.Assert(textBox.UserSort != null, "(null != textbox.UserSort)");
				if (isMatrixGroup && m_groupingScopesForRunningValuesInTablix != null)
				{
					string sortTargetScope = (textBox.UserSort.SortTarget != null) ? textBox.UserSort.SortTarget.ScopeName : null;
					textBox.UserSort.FoundSortExpressionScope = !m_groupingScopesForRunningValuesInTablix.HasRowColScopeConflict(textBox.TextBoxScope, sortTargetScope, isMatrixColumnGroup);
				}
				else
				{
					textBox.UserSort.FoundSortExpressionScope = true;
				}
				textBox.InitializeSortExpression(this, needsExplicitAggregateScope: false);
			}
		}

		internal void ValidateUserSortInnerScope(string scopeName)
		{
			TextBoxList textBoxList = m_userSortTextboxes[scopeName] as TextBoxList;
			if (textBoxList == null)
			{
				return;
			}
			int count = textBoxList.Count;
			for (int i = 0; i < count; i++)
			{
				TextBox textBox = textBoxList[i];
				Global.Tracer.Assert(textBox.UserSort != null, "(null != textbox.UserSort)");
				if (!textBox.UserSort.FoundSortExpressionScope)
				{
					m_errorContext.Register(ProcessingErrorCode.rsInvalidExpressionScope, Severity.Error, textBox.ObjectType, textBox.Name, "SortExpressionScope", textBox.UserSort.SortExpressionScopeString);
				}
				else
				{
					textBox.UserSort.SortExpressionScope = GetSortFilterScope(textBox.UserSort.SortExpressionScopeString);
				}
			}
			m_userSortTextboxes.Remove(scopeName);
		}

		internal void RegisterSortFilterTextbox(TextBox textbox)
		{
			m_reportSortFilterTextboxes.Add(textbox);
		}

		internal void SetDataSetDetailUserSortFilter()
		{
			string dataSetName = GetDataSetName();
			Global.Tracer.Assert(dataSetName != null, "(null != currentDataset)");
			DataSet dataSet = m_reportScopes[dataSetName] as DataSet;
			Global.Tracer.Assert(dataSet != null, "(null != dataset)");
			dataSet.HasDetailUserSortFilter = true;
		}

		internal void CalculateSortFilterGroupingLists()
		{
			int count = m_reportSortFilterTextboxes.Count;
			for (int i = 0; i < count; i++)
			{
				TextBox textBox = m_reportSortFilterTextboxes[i];
				Global.Tracer.Assert(textBox != null && textBox.TextBoxScope != null);
				if (textBox.IsMatrixCellScope)
				{
					textBox.ContainingScopes = (m_reportGroupingLists["0_CellScope" + textBox.TextBoxScope] as GroupingList);
				}
				if (!textBox.IsMatrixCellScope || textBox.ContainingScopes == null)
				{
					textBox.ContainingScopes = (m_reportGroupingLists[textBox.TextBoxScope] as GroupingList);
				}
				Global.Tracer.Assert(textBox.ContainingScopes != null, "(null != textbox.ContainingScopes)");
				for (int j = 0; j < textBox.ContainingScopes.Count; j++)
				{
					textBox.ContainingScopes[j].SaveGroupExprValues = true;
				}
				if (textBox.IsDetailScope)
				{
					textBox.ContainingScopes = textBox.ContainingScopes.Clone();
					textBox.ContainingScopes.Add(null);
				}
				if (textBox.UserSort == null || textBox.UserSort.SortTarget == null)
				{
					continue;
				}
				string scopeName = textBox.UserSort.SortTarget.ScopeName;
				int num = (int)m_reportScopeDatasetIDs[scopeName];
				textBox.UserSort.DataSetID = num;
				if (textBox.UserSort.SortExpressionScope != null)
				{
					string scopeName2 = textBox.UserSort.SortExpressionScope.ScopeName;
					Global.Tracer.Assert(scopeName2 != null && scopeName != null, "(null != esScope && null != stScope)");
					int num2 = (int)m_reportScopeDatasetIDs[scopeName2];
					Global.Tracer.Assert(0 <= num2 && 0 <= num);
					if (num2 == num)
					{
						textBox.UserSort.GroupsInSortTarget = CalculateGroupingDifference(m_reportGroupingLists[scopeName2] as GroupingList, m_reportGroupingLists[scopeName] as GroupingList);
					}
					else
					{
						m_errorContext.Register(ProcessingErrorCode.rsInvalidExpressionScopeDataSet, Severity.Error, textBox.ObjectType, textBox.Name, "SortExpressionScope", textBox.UserSort.SortExpressionScopeString, "SortTarget");
					}
				}
				if (!m_errorContext.HasError)
				{
					textBox.AddToScopeSortFilterList();
				}
			}
			count = ((m_report.SubReports != null) ? m_report.SubReports.Count : 0);
			for (int k = 0; k < count; k++)
			{
				SubReport subReport = m_report.SubReports[k];
				if (subReport.SubReportScope != null)
				{
					if (subReport.IsMatrixCellScope)
					{
						subReport.ContainingScopes = (m_reportGroupingLists["0_CellScope" + subReport.SubReportScope] as GroupingList);
					}
					else
					{
						subReport.ContainingScopes = (m_reportGroupingLists[subReport.SubReportScope] as GroupingList);
					}
					Global.Tracer.Assert(subReport.ContainingScopes != null, "(null != subReport.ContainingScopes)");
					for (int l = 0; l < subReport.ContainingScopes.Count; l++)
					{
						subReport.ContainingScopes[l].SaveGroupExprValues = true;
					}
				}
				if (subReport.IsDetailScope)
				{
					subReport.ContainingScopes = subReport.ContainingScopes.Clone();
					subReport.ContainingScopes.Add(null);
				}
			}
		}

		private GroupingList CalculateGroupingDifference(GroupingList expressionScope, GroupingList targetScope)
		{
			if (expressionScope == null || expressionScope.Count == 0)
			{
				return null;
			}
			if (targetScope == null || targetScope.Count == 0)
			{
				return expressionScope;
			}
			if (expressionScope.Count < targetScope.Count)
			{
				return null;
			}
			GroupingList groupingList = expressionScope.Clone();
			int count = targetScope.Count;
			int num = expressionScope.IndexOf(targetScope[0]);
			if (num < 0)
			{
				return groupingList;
			}
			Global.Tracer.Assert(num + count <= expressionScope.Count, "(startIndex + count <= expressionScope.Count)");
			groupingList.RemoveRange(0, num + 1);
			for (int i = 1; i < count; i++)
			{
				if (expressionScope[num + i] == targetScope[i])
				{
					groupingList.RemoveAt(0);
					continue;
				}
				return groupingList;
			}
			return groupingList;
		}

		internal void TextboxWithDetailSortExpressionAdd(TextBox textbox)
		{
			Global.Tracer.Assert(m_detailSortExpressionScopeTextboxes != null, "(null != m_detailSortExpressionScopeTextboxes)");
			m_detailSortExpressionScopeTextboxes.Add(textbox);
		}

		internal void TextboxesWithDetailSortExpressionInitialize()
		{
			Global.Tracer.Assert(m_detailSortExpressionScopeTextboxes != null, "(null != m_detailSortExpressionScopeTextboxes)");
			int count = m_detailSortExpressionScopeTextboxes.Count;
			if (count != 0)
			{
				for (int i = 0; i < count; i++)
				{
					m_detailSortExpressionScopeTextboxes[i].InitializeSortExpression(this, needsExplicitAggregateScope: true);
				}
				m_detailSortExpressionScopeTextboxes.RemoveRange(0, count);
			}
		}
	}
}

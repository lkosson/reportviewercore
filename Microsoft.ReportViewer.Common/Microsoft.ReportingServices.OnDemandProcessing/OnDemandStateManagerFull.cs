using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Permissions;
using System.Threading;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class OnDemandStateManagerFull : OnDemandStateManager
	{
		private IReportScopeInstance m_lastROMInstance;

		private IInstancePath m_lastRIFObject;

		private IRIFReportScope m_lastTablixProcessingReportScope;

		private List<InstancePathItem> m_lastInstancePath;

		private readonly List<PairObj<string, object>> m_specialLastGroupingValues = new List<PairObj<string, object>>();

		private bool m_lastInScopeResult;

		private int m_lastRecursiveLevel;

		private bool m_inRecursiveRowHierarchy;

		private bool m_inRecursiveColumnHierarchy;

		internal override IReportScopeInstance LastROMInstance => m_lastROMInstance;

		internal override IInstancePath LastRIFObject
		{
			get
			{
				return m_lastRIFObject;
			}
			set
			{
				m_lastRIFObject = value;
			}
		}

		internal override IRIFReportScope LastTablixProcessingReportScope
		{
			get
			{
				return m_lastTablixProcessingReportScope;
			}
			set
			{
				m_lastTablixProcessingReportScope = value;
			}
		}

		internal override QueryRestartInfo QueryRestartInfo => null;

		internal override ExecutedQueryCache ExecutedQueryCache => null;

		public OnDemandStateManagerFull(OnDemandProcessingContext odpContext)
			: base(odpContext)
		{
		}

		internal override ExecutedQueryCache SetupExecutedQueryCache()
		{
			return ExecutedQueryCache;
		}

		internal override Dictionary<string, object> GetCurrentSpecialGroupingValues()
		{
			int num = (m_lastInstancePath != null) ? m_lastInstancePath.Count : 0;
			Dictionary<string, object> dictionary = new Dictionary<string, object>(num, StringComparer.Ordinal);
			for (int i = 0; i < num; i++)
			{
				PairObj<string, object> pairObj = m_specialLastGroupingValues[i];
				if (pairObj != null && !dictionary.ContainsKey(pairObj.First))
				{
					dictionary.Add(pairObj.First, pairObj.Second);
				}
			}
			return dictionary;
		}

		internal override bool CalculateAggregate(string aggregateName)
		{
			OnDemandProcessingContext odpWorkerContextForTablixProcessing = GetOdpWorkerContextForTablixProcessing();
			odpWorkerContextForTablixProcessing.ReportAggregates.TryGetValue(aggregateName, out Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo value);
			if (value == null)
			{
				return false;
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = m_odpContext.ReportDefinition.MappingDataSetIndexToDataSet[value.DataSetIndexInCollection];
			Microsoft.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance = odpWorkerContextForTablixProcessing.GetDataSetInstance(dataSet);
			if (dataSetInstance != null)
			{
				bool flag = odpWorkerContextForTablixProcessing.IsTablixProcessingComplete(dataSet.IndexInCollection);
				if (!flag)
				{
					if (odpWorkerContextForTablixProcessing.IsTablixProcessingMode)
					{
						return false;
					}
					((OnDemandStateManagerFull)odpWorkerContextForTablixProcessing.StateManager).PerformOnDemandTablixProcessingWithContextRestore(dataSet);
				}
				if (flag || m_odpContext.IsPageHeaderFooter)
				{
					dataSetInstance.SetupDataSetLevelAggregates(m_odpContext);
				}
				return true;
			}
			return false;
		}

		internal override bool CalculateLookup(LookupInfo lookup)
		{
			OnDemandProcessingContext odpWorkerContextForTablixProcessing = GetOdpWorkerContextForTablixProcessing();
			Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = m_odpContext.ReportDefinition.MappingDataSetIndexToDataSet[lookup.DataSetIndexInCollection];
			if (odpWorkerContextForTablixProcessing.GetDataSetInstance(dataSet) != null)
			{
				if (!odpWorkerContextForTablixProcessing.IsTablixProcessingComplete(dataSet.IndexInCollection))
				{
					if (odpWorkerContextForTablixProcessing.IsTablixProcessingMode)
					{
						return false;
					}
					((OnDemandStateManagerFull)odpWorkerContextForTablixProcessing.StateManager).PerformOnDemandTablixProcessingWithContextRestore(dataSet);
				}
				return true;
			}
			return false;
		}

		internal override bool PrepareFieldsCollectionForDirectFields()
		{
			if (m_odpContext.IsPageHeaderFooter && m_odpContext.ReportDefinition.DataSetsNotOnlyUsedInParameters == 1)
			{
				OnDemandProcessingContext parentContext = m_odpContext.ParentContext;
				Microsoft.ReportingServices.ReportIntermediateFormat.DataSet firstDataSet = m_odpContext.ReportDefinition.FirstDataSet;
				Microsoft.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance = parentContext.GetDataSetInstance(firstDataSet);
				if (dataSetInstance != null)
				{
					if (!parentContext.IsTablixProcessingComplete(firstDataSet.IndexInCollection))
					{
						((OnDemandStateManagerFull)parentContext.StateManager).PerformOnDemandTablixProcessing(firstDataSet);
					}
					if (!dataSetInstance.NoRows)
					{
						dataSetInstance.SetupEnvironment(m_odpContext, newDataSetDefinition: false);
						parentContext.GetDataChunkReader(firstDataSet.IndexInCollection).ResetCachedStreamOffset();
						return true;
					}
				}
			}
			return false;
		}

		internal override void EvaluateScopedFieldReference(string scopeName, int fieldIndex, ref Microsoft.ReportingServices.RdlExpressions.VariantResult result)
		{
			Global.Tracer.Assert(condition: false, "Scoped field references are not supported in Full ODP mode.");
			throw new NotImplementedException();
		}

		internal override int RecursiveLevel(string scopeName)
		{
			if (m_odpContext.IsTablixProcessingMode)
			{
				return m_odpContext.ReportRuntime.RecursiveLevel(scopeName);
			}
			if (scopeName == null)
			{
				if (m_inRecursiveRowHierarchy && m_inRecursiveColumnHierarchy)
				{
					return 0;
				}
				return m_lastRecursiveLevel;
			}
			m_lastRecursiveLevel = 0;
			SetupObjectModels(OnDemandMode.InScope, needDeepCopyPath: false, -1, scopeName);
			if (m_lastInScopeResult)
			{
				return m_lastRecursiveLevel;
			}
			return 0;
		}

		internal override bool InScope(string scopeName)
		{
			m_lastInScopeResult = false;
			if (m_odpContext.IsTablixProcessingMode)
			{
				if (m_odpContext.ReportRuntime.CurrentScope == null)
				{
					return false;
				}
				return m_odpContext.ReportRuntime.CurrentScope.InScope(scopeName);
			}
			if (m_lastInstancePath != null && scopeName != null)
			{
				SetupObjectModels(OnDemandMode.InScope, needDeepCopyPath: false, -1, scopeName);
			}
			return m_lastInScopeResult;
		}

		internal override void ResetOnDemandState()
		{
			m_lastInstancePath = null;
			m_lastRecursiveLevel = 0;
			m_lastInScopeResult = false;
			m_lastTablixProcessingReportScope = null;
		}

		private bool InScopeCompare(string scope1, string scope2)
		{
			m_lastInScopeResult = (string.CompareOrdinal(scope1, scope2) == 0);
			return m_lastInScopeResult;
		}

		internal override void RestoreContext(IInstancePath originalObject)
		{
			if (originalObject != null && m_odpContext.ReportRuntime.ContextUpdated && !InstancePathItem.IsSameScopePath(originalObject, m_lastRIFObject))
			{
				SetupContext(originalObject, null, -1);
			}
		}

		internal override void SetupContext(IInstancePath rifObject, IReportScopeInstance romInstance)
		{
			SetupContext(rifObject, romInstance, -1);
		}

		internal override void SetupContext(IInstancePath rifObject, IReportScopeInstance romInstance, int moveNextInstanceIndex)
		{
			bool flag = false;
			bool needDeepCopyPath = false;
			if (romInstance == null)
			{
				flag = true;
				m_lastRIFObject = rifObject;
				needDeepCopyPath = true;
			}
			else if (romInstance.IsNewContext || m_lastROMInstance == null || m_lastRIFObject == null || 0 <= moveNextInstanceIndex)
			{
				flag = true;
				romInstance.IsNewContext = false;
				m_lastROMInstance = romInstance;
				m_lastRIFObject = rifObject;
				needDeepCopyPath = true;
			}
			else if (m_lastROMInstance.Equals(romInstance))
			{
				if (!m_lastRIFObject.Equals(rifObject) && (m_lastRIFObject.InstancePathItem.Type == InstancePathItemType.SubReport || rifObject.InstancePathItem.Type == InstancePathItemType.SubReport))
				{
					flag = true;
				}
				m_lastRIFObject = rifObject;
			}
			else if (m_lastRIFObject.Equals(rifObject))
			{
				m_lastROMInstance = romInstance;
			}
			else if (InstancePathItem.IsSamePath(m_lastInstancePath, rifObject.InstancePath))
			{
				m_lastROMInstance = romInstance;
				m_lastRIFObject = rifObject;
			}
			else
			{
				flag = true;
				m_lastROMInstance = romInstance;
				m_lastRIFObject = rifObject;
				needDeepCopyPath = true;
			}
			if (flag)
			{
				SetupObjectModels(OnDemandMode.FullSetup, needDeepCopyPath, moveNextInstanceIndex, null);
				m_odpContext.ReportRuntime.ContextUpdated = true;
			}
		}

		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.ControlThread)]
		private static void UpdateThreadCultureWithAssert(CultureInfo newCulture)
		{
			Thread.CurrentThread.CurrentCulture = newCulture;
		}

		private void SetupObjectModels(OnDemandMode mode, bool needDeepCopyPath, int moveNextInstanceIndex, string scopeName)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.DataRegionInstance dataRegionInstance = null;
			IMemberHierarchy memberHierarchy = null;
			int num = -1;
			ScopeInstance scopeInstance = m_odpContext.CurrentReportInstance;
			List<InstancePathItem> lastInstancePath = m_lastInstancePath;
			List<InstancePathItem> list = null;
			int num2 = 0;
			Microsoft.ReportingServices.ReportIntermediateFormat.Report reportDefinition = m_odpContext.ReportDefinition;
			ObjectModelImpl reportObjectModel = m_odpContext.ReportObjectModel;
			bool flag = false;
			bool flag2 = false;
			int i = 0;
			try
			{
				if (m_lastRIFObject.InstancePath != null)
				{
					list = m_lastRIFObject.InstancePath;
					num2 = list.Count;
				}
				if (mode != OnDemandMode.InScope)
				{
					m_odpContext.EnsureCultureIsSetOnCurrentThread();
				}
				if (mode == OnDemandMode.InScope && 1 == reportDefinition.DataSetsNotOnlyUsedInParameters && InScopeCompare(reportDefinition.FirstDataSet.Name, scopeName))
				{
					return;
				}
				int num3 = 0;
				if (m_odpContext.InSubreport)
				{
					num3 = InstancePathItem.GetParentReportIndex(m_lastRIFObject.InstancePath, m_lastRIFObject.InstancePathItem.Type == InstancePathItemType.SubReport);
				}
				bool identicalPaths;
				int sharedPathIndex = InstancePathItem.GetSharedPathIndex(num3, lastInstancePath, list, reportObjectModel.AllFieldsCleared, out identicalPaths);
				for (int j = m_specialLastGroupingValues.Count; j < num3; j++)
				{
					m_specialLastGroupingValues.Add(null);
				}
				for (int k = num3; k < num2; k++)
				{
					InstancePathItem instancePathItem = list[k];
					bool flag3 = false;
					if (mode != OnDemandMode.InScope)
					{
						flag3 = (k <= sharedPathIndex);
					}
					if (!flag3 && mode == OnDemandMode.FullSetup)
					{
						if (m_specialLastGroupingValues.Count < num2)
						{
							m_specialLastGroupingValues.Add(null);
						}
						else
						{
							m_specialLastGroupingValues[k] = null;
						}
					}
					switch (instancePathItem.Type)
					{
					case InstancePathItemType.SubReport:
					{
						if (scopeInstance.SubreportInstances == null || instancePathItem.IndexInCollection >= scopeInstance.SubreportInstances.Count)
						{
							break;
						}
						IReference<Microsoft.ReportingServices.ReportIntermediateFormat.SubReportInstance> reference = scopeInstance.SubreportInstances[instancePathItem.IndexInCollection];
						using (reference.PinValue())
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.SubReportInstance subReportInstance = reference.Value();
							subReportInstance.SubReportDef.CurrentSubReportInstance = reference;
							if (mode != OnDemandMode.InScope && !subReportInstance.Initialized)
							{
								if (m_odpContext.IsTablixProcessingMode || m_odpContext.IsTopLevelSubReportProcessing)
								{
									return;
								}
								SubReportInitializer.InitializeSubReport(subReportInstance.SubReportDef);
								reference.PinValue();
							}
							Global.Tracer.Assert(k == num2 - 1, "SubReport not last in instance path.");
						}
						break;
					}
					case InstancePathItemType.DataRegion:
						if (scopeInstance is Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance && (scopeInstance.DataRegionInstances == null || scopeInstance.DataRegionInstances.Count <= instancePathItem.IndexInCollection || scopeInstance.DataRegionInstances[instancePathItem.IndexInCollection] == null || scopeInstance.DataRegionInstances[instancePathItem.IndexInCollection].Value() == null))
						{
							Global.Tracer.Assert(instancePathItem.IndexInCollection < reportDefinition.TopLevelDataRegions.Count, "(newItem.IndexInCollection < m_reportDefinition.TopLevelDataRegions.Count)");
							Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = reportDefinition.TopLevelDataRegions[instancePathItem.IndexInCollection].GetDataSet(reportDefinition);
							if (mode == OnDemandMode.InScope && InScopeCompare(dataSet.Name, scopeName))
							{
								return;
							}
							PerformOnDemandTablixProcessing(dataSet);
						}
						scopeInstance = scopeInstance.DataRegionInstances[instancePathItem.IndexInCollection].Value();
						flag = (m_inRecursiveColumnHierarchy = false);
						flag2 = (m_inRecursiveRowHierarchy = false);
						num = -1;
						dataRegionInstance = (scopeInstance as Microsoft.ReportingServices.ReportIntermediateFormat.DataRegionInstance);
						memberHierarchy = dataRegionInstance;
						if (mode == OnDemandMode.InScope && InScopeCompare(dataRegionInstance.DataRegionDef.Name, scopeName))
						{
							return;
						}
						if (dataRegionInstance.DataSetIndexInCollection >= 0 && m_odpContext.CurrentDataSetIndex != dataRegionInstance.DataSetIndexInCollection && mode != OnDemandMode.InScope)
						{
							if (!flag3)
							{
								Microsoft.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance = m_odpContext.CurrentReportInstance.GetDataSetInstance(dataRegionInstance.DataSetIndexInCollection, m_odpContext);
								if (dataSetInstance != null)
								{
									dataSetInstance.SetupEnvironment(m_odpContext, newDataSetDefinition: true);
									i = 0;
								}
							}
							else
							{
								i = k + 1;
							}
						}
						if (mode == OnDemandMode.InScope)
						{
							break;
						}
						if (!flag3)
						{
							dataRegionInstance.SetupEnvironment(m_odpContext);
							i = 0;
							if (dataRegionInstance.NoRows)
							{
								dataRegionInstance.DataRegionDef.NoRows = true;
								dataRegionInstance.DataRegionDef.ResetTopLevelDynamicMemberInstanceCount();
								return;
							}
							dataRegionInstance.DataRegionDef.NoRows = false;
						}
						else
						{
							i = k + 1;
						}
						break;
					case InstancePathItemType.ColumnMemberInstanceIndexTopMost:
						scopeInstance = dataRegionInstance;
						break;
					case InstancePathItemType.Cell:
					{
						if (-1 == num)
						{
							num = 0;
						}
						IList<Microsoft.ReportingServices.ReportIntermediateFormat.DataCellInstance> cellInstances = memberHierarchy.GetCellInstances(num);
						if (cellInstances == null)
						{
							if (flag2 && flag)
							{
								reportObjectModel.ResetFieldValues();
							}
						}
						else
						{
							if (cellInstances.Count <= instancePathItem.IndexInCollection)
							{
								break;
							}
							Microsoft.ReportingServices.ReportIntermediateFormat.DataCellInstance dataCellInstance = cellInstances[instancePathItem.IndexInCollection];
							if (dataCellInstance != null)
							{
								scopeInstance = dataCellInstance;
								if (!flag3)
								{
									dataCellInstance.SetupEnvironment(m_odpContext, m_odpContext.CurrentDataSetIndex);
									i = 0;
								}
								else
								{
									i = k + 1;
								}
							}
						}
						break;
					}
					case InstancePathItemType.None:
						continue;
					}
					if (!instancePathItem.IsDynamicMember)
					{
						continue;
					}
					IList<DataRegionMemberInstance> childMemberInstances = ((IMemberHierarchy)scopeInstance).GetChildMemberInstances(instancePathItem.Type == InstancePathItemType.RowMemberInstanceIndex, instancePathItem.IndexInCollection);
					if (childMemberInstances == null)
					{
						reportObjectModel.ResetFieldValues();
						return;
					}
					int num4 = (k != num2 - 1 || moveNextInstanceIndex < 0 || moveNextInstanceIndex >= childMemberInstances.Count) ? ((instancePathItem.InstanceIndex >= 0) ? instancePathItem.InstanceIndex : 0) : moveNextInstanceIndex;
					if (num4 >= childMemberInstances.Count)
					{
						instancePathItem.ResetContext();
						num4 = 0;
					}
					DataRegionMemberInstance dataRegionMemberInstance = childMemberInstances[num4];
					if (mode == OnDemandMode.FullSetup)
					{
						dataRegionMemberInstance.MemberDef.InstanceCount = childMemberInstances.Count;
						dataRegionMemberInstance.MemberDef.CurrentMemberIndex = num4;
					}
					scopeInstance = dataRegionMemberInstance;
					m_lastRecursiveLevel = dataRegionMemberInstance.RecursiveLevel;
					Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode memberDef = dataRegionMemberInstance.MemberDef;
					if (mode == OnDemandMode.InScope && InScopeCompare(memberDef.Grouping.Name, scopeName))
					{
						return;
					}
					if (instancePathItem.Type == InstancePathItemType.RowMemberInstanceIndex)
					{
						memberHierarchy = dataRegionMemberInstance;
						flag2 = true;
					}
					else
					{
						num = dataRegionMemberInstance.MemberInstanceIndexWithinScopeLevel;
						flag = true;
					}
					if (mode == OnDemandMode.FullSetup && !flag3)
					{
						dataRegionMemberInstance.SetupEnvironment(m_odpContext, m_odpContext.CurrentDataSetIndex);
						i = 0;
						Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = memberDef.Grouping;
						if (grouping.Parent != null)
						{
							if (memberDef.IsColumn)
							{
								m_inRecursiveColumnHierarchy = true;
							}
							else
							{
								m_inRecursiveRowHierarchy = true;
							}
							if (memberDef.IsTablixMember)
							{
								memberDef.SetMemberInstances(childMemberInstances);
								memberDef.SetRecursiveParentIndex(dataRegionMemberInstance.RecursiveParentIndex);
								memberDef.SetInstanceHasRecursiveChildren(dataRegionMemberInstance.HasRecursiveChildren);
							}
						}
						else if (memberDef.IsColumn)
						{
							m_inRecursiveColumnHierarchy = false;
						}
						else
						{
							m_inRecursiveRowHierarchy = false;
						}
						grouping.RecursiveLevel = m_lastRecursiveLevel;
						grouping.SetGroupInstanceExpressionValues(dataRegionMemberInstance.GroupExprValues);
						if (mode != 0 || grouping == null || grouping.GroupExpressions == null || grouping.GroupExpressions.Count <= 0)
						{
							continue;
						}
						Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = grouping.GroupExpressions[0];
						if (expressionInfo.Type != Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Field)
						{
							continue;
						}
						Microsoft.ReportingServices.ReportIntermediateFormat.Field field = memberDef.DataRegionDef.GetDataSet(reportDefinition).Fields[expressionInfo.IntValue];
						if (field.DataField != null)
						{
							string dataField = field.DataField;
							object second = dataRegionMemberInstance.GroupExprValues[0];
							PairObj<string, object> pairObj = m_specialLastGroupingValues[k];
							if (pairObj == null)
							{
								pairObj = new PairObj<string, object>(dataField, second);
								m_specialLastGroupingValues[k] = pairObj;
							}
							else
							{
								pairObj.First = dataField;
								pairObj.Second = second;
							}
						}
					}
					else
					{
						i = k + 1;
					}
				}
				if (mode == OnDemandMode.FullSetup && !identicalPaths && scopeInstance != null && i > 0)
				{
					for (; i < m_lastInstancePath.Count; i++)
					{
						if (m_lastInstancePath[i].IsScope)
						{
							scopeInstance.SetupFields(m_odpContext, m_odpContext.CurrentDataSetIndex);
							break;
						}
					}
				}
				if (mode != 0 || m_odpContext.IsTablixProcessingMode || m_odpContext.CurrentReportInstance == null || dataRegionInstance != null || reportDefinition.DataSetsNotOnlyUsedInParameters != 1)
				{
					return;
				}
				Microsoft.ReportingServices.ReportIntermediateFormat.DataSet firstDataSet = reportDefinition.FirstDataSet;
				Microsoft.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance2 = m_odpContext.CurrentReportInstance.GetDataSetInstance(firstDataSet, m_odpContext);
				if (dataSetInstance2 != null)
				{
					bool flag4 = true;
					if (!m_odpContext.IsTablixProcessingComplete(firstDataSet.IndexInCollection))
					{
						PerformOnDemandTablixProcessing(firstDataSet);
						flag4 = false;
					}
					if (m_odpContext.CurrentOdpDataSetInstance == dataSetInstance2)
					{
						flag4 = false;
					}
					if (flag4)
					{
						dataSetInstance2.SetupEnvironment(m_odpContext, newDataSetDefinition: true);
					}
					else if (!dataSetInstance2.NoRows)
					{
						dataSetInstance2.SetupFields(m_odpContext, dataSetInstance2);
					}
				}
			}
			finally
			{
				if (needDeepCopyPath)
				{
					InstancePathItem.DeepCopyPath(list, ref m_lastInstancePath);
				}
			}
		}

		private void PerformOnDemandTablixProcessing(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			Merge.TablixDataProcessing(m_odpContext, dataSet);
			m_odpContext.ReportObjectModel.ResetFieldValues();
		}

		private void PerformOnDemandTablixProcessingWithContextRestore(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			Global.Tracer.Assert(!m_odpContext.IsTablixProcessingMode, "Nested calls of tablix data processing are not supported");
			IInstancePath lastRIFObject = m_lastRIFObject;
			PerformOnDemandTablixProcessing(dataSet);
			RestoreContext(lastRIFObject);
		}

		internal override IRecordRowReader CreateSequentialDataReader(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, out Microsoft.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance currentReportInstance = m_odpContext.CurrentReportInstance;
			dataSetInstance = currentReportInstance.GetDataSetInstance(dataSet, m_odpContext);
			Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataChunkReader = null;
			if (!dataSetInstance.NoRows)
			{
				dataChunkReader = new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader(dataSetInstance, m_odpContext, dataSetInstance.DataChunkName);
				RegisterDisposableDataReaderOrIdcDataManager(dataChunkReader);
			}
			return dataChunkReader;
		}

		internal override void BindNextMemberInstance(IInstancePath rifObject, IReportScopeInstance romInstance, int moveNextInstanceIndex)
		{
			Global.Tracer.Assert(condition: false, "This method is not valid for this StateManager type.");
			throw new InvalidOperationException("This method is not valid for this StateManager type.");
		}

		internal override bool ShouldStopPipelineAdvance(bool rowAccepted)
		{
			return true;
		}

		internal override void CreatedScopeInstance(IRIFReportDataScope scope)
		{
		}

		internal override bool ProcessOneRow(IRIFReportDataScope scope)
		{
			Global.Tracer.Assert(condition: false, "This method is not valid for this StateManager type.");
			throw new InvalidOperationException("This method is not valid for this StateManager type.");
		}

		internal override bool CheckForPrematureServerAggregate(string aggregateName)
		{
			return false;
		}
	}
}

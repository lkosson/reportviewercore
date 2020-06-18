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
	internal abstract class RuntimeCell : IScope, IStorable, IPersistable, ISelfReferential, IDataRowHolder, IOnDemandScopeInstance
	{
		protected RuntimeDataTablixGroupLeafObjReference m_owner;

		protected int m_outerGroupDynamicIndex;

		protected List<int> m_rowIndexes;

		protected List<int> m_colIndexes;

		protected List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_cellNonCustomAggObjs;

		protected BucketedDataAggregateObjs m_cellAggregatesOfAggregates;

		protected BucketedDataAggregateObjs m_cellPostSortAggregatesOfAggregates;

		protected Cell m_canonicalCellScopeDef;

		protected List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_cellCustomAggObjs;

		protected Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] m_cellAggValueList;

		protected Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[,][] m_runningValueValues;

		protected Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[,][] m_runningValueOfAggregateValues;

		protected ScalableList<DataFieldRow> m_dataRows;

		protected DataActions m_dataAction;

		protected bool m_innermost;

		protected DataFieldRow m_firstRow;

		protected bool m_firstRowIsAggregate;

		private int m_nextCell = -1;

		protected int[] m_sortFilterExpressionScopeInfoIndices;

		private long m_scopeInstanceNumber;

		private bool m_hasProcessedAggregateRow;

		[NonSerialized]
		protected RuntimeCellReference m_selfReference;

		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		internal int NextCell
		{
			get
			{
				return m_nextCell;
			}
			set
			{
				m_nextCell = value;
			}
		}

		public bool IsNoRows => m_firstRow == null;

		public bool IsMostRecentlyCreatedScopeInstance => m_canonicalCellScopeDef.CanonicalDataScopeInfo.IsLastScopeInstanceNumber(m_scopeInstanceNumber);

		public bool HasUnProcessedServerAggregate
		{
			get
			{
				if (m_cellCustomAggObjs != null && m_cellCustomAggObjs.Count > 0)
				{
					return !m_hasProcessedAggregateRow;
				}
				return false;
			}
		}

		bool IScope.TargetForNonDetailSort => m_owner.Value().GetCellTargetForNonDetailSort();

		int[] IScope.SortFilterExpressionScopeInfoIndices
		{
			get
			{
				if (m_sortFilterExpressionScopeInfoIndices == null)
				{
					OnDemandProcessingContext odpContext = m_owner.Value().OdpContext;
					m_sortFilterExpressionScopeInfoIndices = new int[odpContext.RuntimeSortFilterInfo.Count];
					for (int i = 0; i < odpContext.RuntimeSortFilterInfo.Count; i++)
					{
						m_sortFilterExpressionScopeInfoIndices[i] = -1;
					}
				}
				return m_sortFilterExpressionScopeInfoIndices;
			}
		}

		IRIFReportScope IScope.RIFReportScope
		{
			get
			{
				int index = m_rowIndexes[0];
				int index2 = m_colIndexes[0];
				return m_owner.Value().DataRegionDef.Rows[index].Cells[index2];
			}
		}

		int IScope.Depth => m_outerGroupDynamicIndex;

		internal RuntimeCellReference SelfReference => m_selfReference;

		public virtual int Size => ItemSizes.SizeOf(m_owner) + 4 + ItemSizes.SizeOf(m_rowIndexes) + ItemSizes.SizeOf(m_colIndexes) + ItemSizes.SizeOf(m_cellNonCustomAggObjs) + ItemSizes.SizeOf(m_cellCustomAggObjs) + ItemSizes.SizeOf(m_cellAggValueList) + ItemSizes.SizeOf(m_runningValueValues) + ItemSizes.SizeOf(m_runningValueOfAggregateValues) + ItemSizes.SizeOf(m_dataRows) + 1 + ItemSizes.SizeOf(m_firstRow) + 1 + 4 + ItemSizes.SizeOf(m_sortFilterExpressionScopeInfoIndices) + ItemSizes.SizeOf(m_selfReference) + ItemSizes.SizeOf(m_cellAggregatesOfAggregates) + ItemSizes.SizeOf(m_cellPostSortAggregatesOfAggregates) + ItemSizes.ReferenceSize + 4 + 8 + 1;

		protected RuntimeCell()
		{
		}

		internal RuntimeCell(RuntimeDataTablixGroupLeafObjReference owner, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember, bool innermost)
		{
			m_owner = owner;
			m_outerGroupDynamicIndex = outerGroupingMember.HierarchyDynamicIndex;
			m_innermost = innermost;
			m_dataAction = DataActions.None;
			Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = owner.Value().DataRegionDef;
			OnDemandProcessingContext odpContext = owner.Value().OdpContext;
			GetCellIndexes(outerGroupingMember, innerGroupingMember, dataRegionDef, out m_rowIndexes, out m_colIndexes);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			int num = m_rowIndexes.Count * m_colIndexes.Count;
			foreach (int rowIndex in m_rowIndexes)
			{
				foreach (int colIndex in m_colIndexes)
				{
					Cell cell = dataRegionDef.Rows[rowIndex].Cells[colIndex];
					if (cell != null && m_canonicalCellScopeDef == null && cell.DataScopeInfo != null)
					{
						m_canonicalCellScopeDef = cell;
						if (m_canonicalCellScopeDef.CanonicalDataScopeInfo == null)
						{
							if (num == 1)
							{
								m_canonicalCellScopeDef.CanonicalDataScopeInfo = cell.DataScopeInfo;
							}
							else
							{
								flag3 = true;
								m_canonicalCellScopeDef.CanonicalDataScopeInfo = new DataScopeInfo(cell.DataScopeInfo.ScopeID);
							}
						}
					}
					if (cell == null || cell.SimpleGroupTreeCell)
					{
						continue;
					}
					if (cell.AggregateIndexes != null)
					{
						RuntimeDataRegionObj.CreateAggregates(odpContext, dataRegionDef.CellAggregates, cell.AggregateIndexes, ref m_cellNonCustomAggObjs, ref m_cellCustomAggObjs);
					}
					if (cell.DataScopeInfo != null)
					{
						DataScopeInfo dataScopeInfo = cell.DataScopeInfo;
						if (flag3)
						{
							cell.CanonicalDataScopeInfo = m_canonicalCellScopeDef.CanonicalDataScopeInfo;
							m_canonicalCellScopeDef.CanonicalDataScopeInfo.MergeFrom(dataScopeInfo);
						}
						if (dataScopeInfo.AggregatesOfAggregates != null)
						{
							RuntimeDataRegionObj.CreateAggregates(odpContext, dataScopeInfo.AggregatesOfAggregates, ref m_cellAggregatesOfAggregates);
						}
						if (dataScopeInfo.PostSortAggregatesOfAggregates != null)
						{
							RuntimeDataRegionObj.CreateAggregates(odpContext, dataScopeInfo.PostSortAggregatesOfAggregates, ref m_cellPostSortAggregatesOfAggregates);
						}
					}
					if (cell.PostSortAggregateIndexes != null)
					{
						flag = true;
					}
					if (!flag2)
					{
						flag2 = ((IRIFReportScope)cell).NeedToCacheDataRows;
					}
					ConstructCellContents(cell, ref m_dataAction);
				}
			}
			if (flag)
			{
				m_cellAggValueList = new Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[dataRegionDef.CellPostSortAggregates.Count];
				m_dataAction |= DataActions.PostSortAggregates;
			}
			if (!FlagUtils.HasFlag(m_dataAction, DataActions.PostSortAggregates) && dataRegionDef.CellRunningValues != null && flag2)
			{
				m_dataAction |= DataActions.PostSortAggregates;
			}
			HandleSortFilterEvent();
			if (m_canonicalCellScopeDef != null && m_canonicalCellScopeDef.CanonicalDataScopeInfo != null && m_canonicalCellScopeDef.CanonicalDataScopeInfo.HasAggregatesToUpdateAtRowScope)
			{
				m_dataAction |= DataActions.AggregatesOfAggregates;
			}
			if (m_dataAction != 0)
			{
				m_dataRows = new ScalableList<DataFieldRow>(m_outerGroupDynamicIndex + 1, odpContext.TablixProcessingScalabilityCache, 30);
			}
			odpContext.CreatedScopeInstance(m_canonicalCellScopeDef);
			m_scopeInstanceNumber = RuntimeDataRegionObj.AssignScopeInstanceNumber(GetCanonicalDataScopeInfo());
		}

		internal static void GetCellIndexes(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember, Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, out List<int> rowIndexes, out List<int> colIndexes)
		{
			if (innerGroupingMember.IsColumn)
			{
				rowIndexes = ((outerGroupingMember != null) ? outerGroupingMember.GetCellIndexes() : dataRegionDef.OutermostStaticRowIndexes);
				colIndexes = innerGroupingMember.GetCellIndexes();
			}
			else
			{
				rowIndexes = innerGroupingMember.GetCellIndexes();
				colIndexes = ((outerGroupingMember != null) ? outerGroupingMember.GetCellIndexes() : dataRegionDef.OutermostStaticColumnIndexes);
			}
		}

		internal static bool HasOnlySimpleGroupTreeCells(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember, Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef)
		{
			GetCellIndexes(outerGroupingMember, innerGroupingMember, dataRegionDef, out List<int> rowIndexes, out List<int> colIndexes);
			foreach (int item in rowIndexes)
			{
				foreach (int item2 in colIndexes)
				{
					Cell cell = dataRegionDef.Rows[item].Cells[item2];
					if (cell != null && !cell.SimpleGroupTreeCell)
					{
						return false;
					}
				}
			}
			return true;
		}

		protected abstract void ConstructCellContents(Cell cell, ref DataActions dataAction);

		protected abstract void CreateInstanceCellContents(Cell cell, DataCellInstance cellInstance, OnDemandProcessingContext odpContext);

		internal virtual bool NextRow()
		{
			OnDemandProcessingContext odpContext = m_owner.Value().OdpContext;
			RuntimeDataRegionObj.CommonFirstRow(odpContext, ref m_firstRowIsAggregate, ref m_firstRow);
			NextAggregateRow();
			if (!odpContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
			{
				NextNonAggregateRow();
			}
			return true;
		}

		private void NextNonAggregateRow()
		{
			OnDemandProcessingContext odpContext = m_owner.Value().OdpContext;
			RuntimeDataRegionObj.UpdateAggregates(odpContext, m_cellNonCustomAggObjs, updateAndSetup: false);
			if (m_dataRows != null)
			{
				RuntimeDataTablixObj.SaveData(m_dataRows, odpContext);
			}
		}

		private void NextAggregateRow()
		{
			OnDemandProcessingContext odpContext = m_owner.Value().OdpContext;
			FieldsImpl fieldsImpl = odpContext.ReportObjectModel.FieldsImpl;
			if (fieldsImpl.ValidAggregateRow && fieldsImpl.AggregationFieldCount == 0)
			{
				m_hasProcessedAggregateRow = true;
				if (m_cellCustomAggObjs != null)
				{
					RuntimeDataRegionObj.UpdateAggregates(odpContext, m_cellCustomAggObjs, updateAndSetup: false);
				}
			}
		}

		internal void SortAndFilter(AggregateUpdateContext aggContext)
		{
			SetupEnvironment();
			AggregateUpdateQueue workQueue = RuntimeDataRegionObj.AggregateOfAggregatesStart(aggContext, this, GetCanonicalDataScopeInfo(), m_cellAggregatesOfAggregates, AggregateUpdateFlags.Both, needsSetupEnvironment: false);
			TraverseCellContents(ProcessingStages.SortAndFilter, aggContext);
			RuntimeDataRegionObj.AggregatesOfAggregatesEnd(this, aggContext, workQueue, GetCanonicalDataScopeInfo(), m_cellAggregatesOfAggregates, updateAggsIfNeeded: true);
		}

		protected virtual void TraverseCellContents(ProcessingStages operation, AggregateUpdateContext context)
		{
		}

		public void UpdateAggregates(AggregateUpdateContext aggContext)
		{
			SetupEnvironment();
			if (RuntimeDataRegionObj.UpdateAggregatesAtScope(aggContext, this, GetCanonicalDataScopeInfo(), AggregateUpdateFlags.Both, needsSetupEnvironment: false))
			{
				TraverseCellContents(ProcessingStages.UpdateAggregates, aggContext);
			}
		}

		private DataScopeInfo GetCanonicalDataScopeInfo()
		{
			if (m_canonicalCellScopeDef == null)
			{
				return null;
			}
			return m_canonicalCellScopeDef.CanonicalDataScopeInfo;
		}

		protected void HandleSortFilterEvent()
		{
			using (m_owner.PinValue())
			{
				RuntimeDataTablixGroupLeafObj runtimeDataTablixGroupLeafObj = m_owner.Value();
				if (!runtimeDataTablixGroupLeafObj.NeedHandleCellSortFilterEvent())
				{
					return;
				}
				OnDemandProcessingContext odpContext = runtimeDataTablixGroupLeafObj.OdpContext;
				int count = odpContext.RuntimeSortFilterInfo.Count;
				for (int i = 0; i < count; i++)
				{
					IReference<RuntimeSortFilterEventInfo> reference = odpContext.RuntimeSortFilterInfo[i];
					using (reference.PinValue())
					{
						RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
						if (runtimeSortFilterEventInfo.EventSource.IsTablixCellScope)
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem parent = runtimeSortFilterEventInfo.EventSource.Parent;
							while (parent != null && !parent.IsDataRegion)
							{
								parent = parent.Parent;
							}
							if (parent == runtimeDataTablixGroupLeafObj.DataRegionDef && ((IScope)this).TargetScopeMatched(i, detailSort: false) && !runtimeDataTablixGroupLeafObj.GetOwnerDataTablix().Value().TargetForNonDetailSort && !runtimeSortFilterEventInfo.HasEventSourceScope)
							{
								runtimeSortFilterEventInfo.SetEventSourceScope(isColumnAxis: false, SelfReference, -1);
							}
						}
					}
				}
			}
		}

		internal void CalculateRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
			OnDemandProcessingContext odpContext = m_owner.Value().OdpContext;
			if (GetCanonicalDataScopeInfo() != null)
			{
				List<string> runningValuesInGroup = null;
				List<string> previousValuesInGroup = null;
				RuntimeDataTablixObj.AddRunningValues(odpContext, GetCanonicalDataScopeInfo().RunningValuesOfAggregates, ref runningValuesInGroup, ref previousValuesInGroup, groupCol, lastGroup);
			}
			bool flag = m_dataRows != null && FlagUtils.HasFlag(m_dataAction, DataActions.PostSortAggregates);
			AggregateUpdateQueue workQueue = RuntimeDataRegionObj.AggregateOfAggregatesStart(aggContext, this, GetCanonicalDataScopeInfo(), m_cellPostSortAggregatesOfAggregates, flag ? AggregateUpdateFlags.ScopedAggregates : AggregateUpdateFlags.Both, needsSetupEnvironment: true);
			if (flag)
			{
				DataActions dataActions = DataActions.PostSortAggregates;
				if (aggContext.LastScopeNeedsRowAggregateProcessing())
				{
					dataActions |= DataActions.PostSortAggregatesOfAggregates;
				}
				ReadRows(dataActions, aggContext);
				m_dataRows.Clear();
				m_dataRows = null;
			}
			using (m_owner.PinValue())
			{
				RuntimeDataTablixGroupLeafObj runtimeDataTablixGroupLeafObj = m_owner.Value();
				if (m_cellAggValueList != null)
				{
					List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> cellPostSortAggregates = runtimeDataTablixGroupLeafObj.CellPostSortAggregates;
					if (cellPostSortAggregates != null && 0 < cellPostSortAggregates.Count)
					{
						for (int i = 0; i < cellPostSortAggregates.Count; i++)
						{
							m_cellAggValueList[i] = cellPostSortAggregates[i].AggregateResult();
							cellPostSortAggregates[i].Init();
						}
					}
				}
			}
			CalculateInnerRunningValues(groupCol, lastGroup, aggContext);
			RuntimeDataRegionObj.AggregatesOfAggregatesEnd(this, aggContext, workQueue, GetCanonicalDataScopeInfo(), m_cellPostSortAggregatesOfAggregates, updateAggsIfNeeded: true);
			if (odpContext.HasPreviousAggregates)
			{
				CalculatePreviousAggregates();
			}
			DoneReadingRows();
		}

		internal void DoneReadingRows()
		{
			bool flag = m_runningValueValues != null;
			bool flag2 = m_runningValueValues != null;
			Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = m_owner.Value().DataRegionDef;
			List<Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo> cellRunningValues = dataRegionDef.CellRunningValues;
			if (cellRunningValues != null || (m_canonicalCellScopeDef != null && m_canonicalCellScopeDef.CanonicalDataScopeInfo != null && m_canonicalCellScopeDef.CanonicalDataScopeInfo.HasRunningValues))
			{
				AggregatesImpl aggregatesImpl = m_owner.Value().OdpContext.ReportObjectModel.AggregatesImpl;
				RowList rows = dataRegionDef.Rows;
				if (m_runningValueValues == null)
				{
					m_runningValueValues = new Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[m_rowIndexes.Count, m_colIndexes.Count][];
				}
				if (m_runningValueOfAggregateValues == null)
				{
					m_runningValueOfAggregateValues = new Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[m_rowIndexes.Count, m_colIndexes.Count][];
				}
				for (int i = 0; i < m_rowIndexes.Count; i++)
				{
					for (int j = 0; j < m_colIndexes.Count; j++)
					{
						int index = m_rowIndexes[i];
						int index2 = m_colIndexes[j];
						Cell cell = rows[index].Cells[index2];
						List<int> runningValueIndexes = cell.RunningValueIndexes;
						if (runningValueIndexes != null)
						{
							flag = true;
							Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] array = new Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[runningValueIndexes.Count];
							m_runningValueValues[i, j] = array;
							for (int k = 0; k < runningValueIndexes.Count; k++)
							{
								Microsoft.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValueInfo = cellRunningValues[runningValueIndexes[k]];
								Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = aggregatesImpl.GetAggregateObj(runningValueInfo.Name);
								array[k] = aggregateObj.AggregateResult();
							}
						}
						if (cell.DataScopeInfo != null)
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] runningValueValues = null;
							RuntimeRICollection.StoreRunningValues(aggregatesImpl, cell.DataScopeInfo.RunningValuesOfAggregates, ref runningValueValues);
							if (runningValueValues != null)
							{
								flag2 = true;
								m_runningValueOfAggregateValues[i, j] = runningValueValues;
							}
						}
					}
				}
			}
			if (!flag)
			{
				m_runningValueValues = null;
			}
			if (!flag2)
			{
				m_runningValueOfAggregateValues = null;
			}
		}

		protected virtual void CalculateInnerRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
		}

		private void CalculatePreviousAggregates()
		{
			SetupEnvironment();
			((IScope)this).CalculatePreviousAggregates();
		}

		public void ReadRows(DataActions action, ITraversalContext context)
		{
			for (int i = 0; i < m_dataRows.Count; i++)
			{
				m_dataRows[i].SetFields(m_owner.Value().OdpContext.ReportObjectModel.FieldsImpl);
				ReadRow(action, context);
			}
		}

		protected void SetupAggregates(List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj> aggregates, Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] aggValues)
		{
			if (aggregates != null)
			{
				for (int i = 0; i < aggregates.Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj dataAggregateObj = aggregates[i];
					m_owner.Value().OdpContext.ReportObjectModel.AggregatesImpl.Set(dataAggregateObj.Name, dataAggregateObj.AggregateDef, dataAggregateObj.DuplicateNames, (aggValues == null) ? dataAggregateObj.AggregateResult() : aggValues[i]);
				}
			}
		}

		public void SetupEnvironment()
		{
			SetupAggregates(m_cellNonCustomAggObjs, null);
			SetupAggregates(m_cellCustomAggObjs, null);
			RuntimeDataRegionObj.SetupAggregates(m_owner.Value().OdpContext, m_cellAggregatesOfAggregates);
			RuntimeDataRegionObj.SetupAggregates(m_owner.Value().OdpContext, m_cellPostSortAggregatesOfAggregates);
			OnDemandProcessingContext odpContext = m_owner.Value().OdpContext;
			if (m_cellAggValueList != null)
			{
				using (m_owner.PinValue())
				{
					SetupAggregates(m_owner.Value().CellPostSortAggregates, m_cellAggValueList);
				}
			}
			if (m_canonicalCellScopeDef != null && m_canonicalCellScopeDef.DataScopeInfo != null && m_canonicalCellScopeDef.DataScopeInfo.DataSet != null && m_canonicalCellScopeDef.DataScopeInfo.DataSet.DataSetCore.FieldsContext != null)
			{
				odpContext.ReportObjectModel.RestoreFields(m_canonicalCellScopeDef.DataScopeInfo.DataSet.DataSetCore.FieldsContext);
			}
			if (m_firstRow != null)
			{
				m_firstRow.SetFields(odpContext.ReportObjectModel.FieldsImpl);
			}
			else
			{
				odpContext.ReportObjectModel.ResetFieldValues();
			}
			odpContext.ReportRuntime.CurrentScope = this;
		}

		public abstract IOnDemandMemberOwnerInstanceReference GetDataRegionInstance(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion);

		public abstract IReference<IDataCorrelation> GetIdcReceiver(IRIFReportDataScope scope);

		internal virtual void CreateInstance(IMemberHierarchy dataRegionOrRowMemberInstance, int columnMemberSequenceId)
		{
			SetupEnvironment();
			Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = m_owner.Value().DataRegionDef;
			OnDemandProcessingContext odpContext = m_owner.Value().OdpContext;
			for (int i = 0; i < m_rowIndexes.Count; i++)
			{
				for (int j = 0; j < m_colIndexes.Count; j++)
				{
					int index = m_rowIndexes[i];
					int index2 = m_colIndexes[j];
					Cell cell = dataRegionDef.Rows[index].Cells[index2];
					if (cell == null)
					{
						continue;
					}
					DataCellInstance dataCellInstance = null;
					if (cell.SimpleGroupTreeCell)
					{
						if (m_firstRow != null && cell.InDynamicRowAndColumnContext)
						{
							dataCellInstance = DataCellInstance.CreateInstance(dataRegionOrRowMemberInstance, odpContext, cell, m_firstRow.StreamOffset, columnMemberSequenceId);
						}
					}
					else
					{
						dataCellInstance = DataCellInstance.CreateInstance(dataRegionOrRowMemberInstance, odpContext, cell, (m_runningValueValues != null) ? m_runningValueValues[i, j] : null, (m_runningValueOfAggregateValues != null) ? m_runningValueOfAggregateValues[i, j] : null, (m_firstRow != null) ? m_firstRow.StreamOffset : 0, columnMemberSequenceId);
					}
					if (dataCellInstance != null)
					{
						if (!cell.SimpleGroupTreeCell)
						{
							CreateInstanceCellContents(cell, dataCellInstance, odpContext);
						}
						dataCellInstance.InstanceComplete();
					}
					if (cell.InScopeEventSources != null)
					{
						UserSortFilterContext.ProcessEventSources(odpContext, this, cell.InScopeEventSources);
					}
				}
			}
		}

		private Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.Grouping> GetOuterScopes()
		{
			Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.Grouping> dictionary = null;
			Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = m_owner.Value().DataRegionDef;
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode memberDef = m_owner.Value().MemberDef;
			if (memberDef.CellScopes == null)
			{
				memberDef.CellScopes = new Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.Grouping>[dataRegionDef.OuterGroupingDynamicMemberCount];
			}
			dictionary = memberDef.CellScopes[m_outerGroupDynamicIndex];
			if (dictionary == null)
			{
				IReference<RuntimeDataTablixGroupRootObj> reference = dataRegionDef.CurrentOuterGroupRootObjs[m_outerGroupDynamicIndex];
				Global.Tracer.Assert(reference != null, "(null != outerGroupRoot)");
				Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode hierarchyDef = reference.Value().HierarchyDef;
				Global.Tracer.Assert(hierarchyDef != null, "(null != outerGrouping)");
				dictionary = hierarchyDef.GetScopeNames();
				memberDef.CellScopes[m_outerGroupDynamicIndex] = dictionary;
			}
			return dictionary;
		}

		bool IScope.IsTargetForSort(int index, bool detailSort)
		{
			return m_owner.Value().GetCellTargetForSort(index, detailSort);
		}

		string IScope.GetScopeName()
		{
			return null;
		}

		IReference<IScope> IScope.GetOuterScope(bool includeSubReportContainingScope)
		{
			return m_owner;
		}

		public void ReadRow(DataActions dataAction, ITraversalContext context)
		{
			if (FlagUtils.HasFlag(dataAction, DataActions.PostSortAggregatesOfAggregates) || FlagUtils.HasFlag(dataAction, DataActions.AggregatesOfAggregates))
			{
				((AggregateUpdateContext)context).UpdateAggregatesForRow();
			}
			if (FlagUtils.HasFlag(dataAction, DataActions.PostSortAggregates))
			{
				using (m_owner.PinValue())
				{
					m_owner.Value().ReadRow(DataActions.PostSortAggregates, context);
				}
			}
		}

		void IScope.CalculatePreviousAggregates()
		{
			using (m_owner.PinValue())
			{
				m_owner.Value().CalculatePreviousAggregates();
			}
		}

		bool IScope.InScope(string scope)
		{
			if (m_owner.Value().InScope(scope))
			{
				return true;
			}
			Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.Grouping> outerScopes = GetOuterScopes();
			if (outerScopes == null || outerScopes.Count == 0)
			{
				return false;
			}
			return outerScopes.ContainsKey(scope);
		}

		int IScope.RecursiveLevel(string scope)
		{
			if (scope == null)
			{
				return 0;
			}
			int num = ((IScope)m_owner).RecursiveLevel(scope);
			if (-1 != num)
			{
				return num;
			}
			Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.Grouping> outerScopes = GetOuterScopes();
			if (outerScopes == null || outerScopes.Count == 0)
			{
				return -1;
			}
			if (outerScopes.TryGetValue(scope, out Microsoft.ReportingServices.ReportIntermediateFormat.Grouping value))
			{
				return value.RecursiveLevel;
			}
			return -1;
		}

		bool IScope.TargetScopeMatched(int index, bool detailSort)
		{
			if (!m_owner.Value().TargetScopeMatched(index, detailSort))
			{
				return false;
			}
			IDictionaryEnumerator dictionaryEnumerator = GetOuterScopes().GetEnumerator();
			while (dictionaryEnumerator.MoveNext())
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = (Microsoft.ReportingServices.ReportIntermediateFormat.Grouping)dictionaryEnumerator.Value;
				if ((!detailSort || grouping.SortFilterScopeInfo != null) && grouping.SortFilterScopeMatched != null && !grouping.SortFilterScopeMatched[index])
				{
					return false;
				}
			}
			if (detailSort)
			{
				return true;
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = m_owner.Value().DataRegionDef;
			IReference<RuntimeSortFilterEventInfo> reference = m_owner.Value().OdpContext.RuntimeSortFilterInfo[index];
			using (reference.PinValue())
			{
				List<object>[] sortSourceScopeInfo = reference.Value().SortSourceScopeInfo;
				Microsoft.ReportingServices.ReportIntermediateFormat.Grouping groupingDef = m_owner.Value().GroupingDef;
				if (groupingDef.SortFilterScopeIndex != null && -1 != groupingDef.SortFilterScopeIndex[index])
				{
					int num = groupingDef.SortFilterScopeIndex[index] + 1;
					if (!m_innermost)
					{
						int innerGroupingMaximumDynamicLevel = dataRegionDef.InnerGroupingMaximumDynamicLevel;
						int num2 = m_owner.Value().HeadingLevel + 1;
						while (num2 < innerGroupingMaximumDynamicLevel && num < sortSourceScopeInfo.Length)
						{
							if (sortSourceScopeInfo[num] != null)
							{
								return false;
							}
							num2++;
							num++;
						}
					}
				}
				Global.Tracer.Assert(dataRegionDef.CurrentOuterGroupRootObjs[m_outerGroupDynamicIndex] != null, "(null != dataRegionDef.CurrentOuterGroupRootObjs[m_cellLevel])");
				if (m_outerGroupDynamicIndex + 1 < dataRegionDef.OuterGroupingDynamicMemberCount)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode reportHierarchyNode = dataRegionDef.CurrentOuterGroupRootObjs[m_outerGroupDynamicIndex + 1]?.Value().HierarchyDef;
					if (reportHierarchyNode != null && reportHierarchyNode.Grouping.SortFilterScopeIndex != null && -1 != reportHierarchyNode.Grouping.SortFilterScopeIndex[index])
					{
						int outerGroupingMaximumDynamicLevel = dataRegionDef.OuterGroupingMaximumDynamicLevel;
						int num = reportHierarchyNode.Grouping.SortFilterScopeIndex[index];
						int num3 = m_outerGroupDynamicIndex + 1;
						while (num3 < outerGroupingMaximumDynamicLevel && num < sortSourceScopeInfo.Length)
						{
							if (sortSourceScopeInfo[num] != null)
							{
								return false;
							}
							num3++;
							num++;
						}
					}
				}
			}
			return true;
		}

		void IScope.GetScopeValues(IReference<IHierarchyObj> targetScopeObj, List<object>[] scopeValues, ref int index)
		{
			m_owner.Value().GetScopeValues(targetScopeObj, scopeValues, ref index);
			Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = m_owner.Value().DataRegionDef;
			if (!m_innermost)
			{
				int innerGroupingMaximumDynamicLevel = dataRegionDef.InnerGroupingMaximumDynamicLevel;
				for (int i = m_owner.Value().HeadingLevel + 1; i < innerGroupingMaximumDynamicLevel; i++)
				{
					if (index >= scopeValues.Length)
					{
						break;
					}
					scopeValues[index++] = null;
				}
			}
			Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.Grouping> outerScopes = GetOuterScopes();
			IDictionaryEnumerator dictionaryEnumerator = outerScopes.GetEnumerator();
			while (dictionaryEnumerator.MoveNext())
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = (Microsoft.ReportingServices.ReportIntermediateFormat.Grouping)dictionaryEnumerator.Value;
				if (index < scopeValues.Length)
				{
					Global.Tracer.Assert(index < scopeValues.Length, "Inner groupings");
					scopeValues[index++] = grouping.CurrentGroupExpressionValues;
				}
			}
			int outerGroupingMaximumDynamicLevel = dataRegionDef.OuterGroupingMaximumDynamicLevel;
			for (int j = outerScopes.Count; j < outerGroupingMaximumDynamicLevel; j++)
			{
				if (index >= scopeValues.Length)
				{
					break;
				}
				scopeValues[index++] = null;
			}
		}

		void IScope.GetGroupNameValuePairs(Dictionary<string, object> pairs)
		{
			((IScope)m_owner).GetGroupNameValuePairs(pairs);
			Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.Grouping> outerScopes = GetOuterScopes();
			if (outerScopes != null || outerScopes.Count != 0)
			{
				IEnumerator enumerator = outerScopes.Values.GetEnumerator();
				while (enumerator.MoveNext())
				{
					RuntimeDataRegionObj.AddGroupNameValuePair(m_owner.Value().OdpContext, enumerator.Current as Microsoft.ReportingServices.ReportIntermediateFormat.Grouping, pairs);
				}
			}
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Owner:
					writer.Write(m_owner);
					break;
				case MemberName.OuterGroupDynamicIndex:
					writer.Write(m_outerGroupDynamicIndex);
					break;
				case MemberName.RowIndexes:
					writer.WriteListOfPrimitives(m_rowIndexes);
					break;
				case MemberName.ColumnIndexes:
					writer.WriteListOfPrimitives(m_colIndexes);
					break;
				case MemberName.CellNonCustomAggObjs:
					writer.Write(m_cellNonCustomAggObjs);
					break;
				case MemberName.CellCustomAggObjs:
					writer.Write(m_cellCustomAggObjs);
					break;
				case MemberName.CellAggValueList:
					writer.Write(m_cellAggValueList);
					break;
				case MemberName.RunningValueValues:
					writer.Write(m_runningValueValues);
					break;
				case MemberName.RunningValueOfAggregateValues:
					writer.Write(m_runningValueOfAggregateValues);
					break;
				case MemberName.DataRows:
					writer.Write(m_dataRows);
					break;
				case MemberName.Innermost:
					writer.Write(m_innermost);
					break;
				case MemberName.FirstRow:
					writer.Write(m_firstRow);
					break;
				case MemberName.FirstRowIsAggregate:
					writer.Write(m_firstRowIsAggregate);
					break;
				case MemberName.NextCell:
					writer.Write(m_nextCell);
					break;
				case MemberName.SortFilterExpressionScopeInfoIndices:
					writer.Write(m_sortFilterExpressionScopeInfoIndices);
					break;
				case MemberName.AggregatesOfAggregates:
					writer.Write(m_cellAggregatesOfAggregates);
					break;
				case MemberName.PostSortAggregatesOfAggregates:
					writer.Write(m_cellPostSortAggregatesOfAggregates);
					break;
				case MemberName.CanonicalCellScope:
				{
					int value = scalabilityCache.StoreStaticReference(m_canonicalCellScopeDef);
					writer.Write(value);
					break;
				}
				case MemberName.DataAction:
					writer.WriteEnum((int)m_dataAction);
					break;
				case MemberName.ScopeInstanceNumber:
					writer.Write(m_scopeInstanceNumber);
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

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Owner:
					m_owner = (RuntimeDataTablixGroupLeafObjReference)reader.ReadRIFObject();
					break;
				case MemberName.OuterGroupDynamicIndex:
					m_outerGroupDynamicIndex = reader.ReadInt32();
					break;
				case MemberName.RowIndexes:
					m_rowIndexes = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.ColumnIndexes:
					m_colIndexes = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.CellNonCustomAggObjs:
					m_cellNonCustomAggObjs = reader.ReadListOfRIFObjects<List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.CellCustomAggObjs:
					m_cellCustomAggObjs = reader.ReadListOfRIFObjects<List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.CellAggValueList:
					m_cellAggValueList = reader.ReadArrayOfRIFObjects<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult>();
					break;
				case MemberName.RunningValueValues:
					m_runningValueValues = reader.Read2DArrayOfArrayOfRIFObjects<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult>();
					break;
				case MemberName.RunningValueOfAggregateValues:
					m_runningValueOfAggregateValues = reader.Read2DArrayOfArrayOfRIFObjects<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult>();
					break;
				case MemberName.DataRows:
					m_dataRows = reader.ReadRIFObject<ScalableList<DataFieldRow>>();
					break;
				case MemberName.Innermost:
					m_innermost = reader.ReadBoolean();
					break;
				case MemberName.FirstRow:
					m_firstRow = (DataFieldRow)reader.ReadRIFObject();
					break;
				case MemberName.FirstRowIsAggregate:
					m_firstRowIsAggregate = reader.ReadBoolean();
					break;
				case MemberName.NextCell:
					m_nextCell = reader.ReadInt32();
					break;
				case MemberName.SortFilterExpressionScopeInfoIndices:
					m_sortFilterExpressionScopeInfoIndices = reader.ReadInt32Array();
					break;
				case MemberName.AggregatesOfAggregates:
					m_cellAggregatesOfAggregates = (BucketedDataAggregateObjs)reader.ReadRIFObject();
					break;
				case MemberName.PostSortAggregatesOfAggregates:
					m_cellPostSortAggregatesOfAggregates = (BucketedDataAggregateObjs)reader.ReadRIFObject();
					break;
				case MemberName.CanonicalCellScope:
				{
					int id = reader.ReadInt32();
					m_canonicalCellScopeDef = (Cell)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.DataAction:
					m_dataAction = (DataActions)reader.ReadEnum();
					break;
				case MemberName.ScopeInstanceNumber:
					m_scopeInstanceNumber = reader.ReadInt64();
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

		public virtual void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public virtual Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCell;
		}

		internal static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Owner, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObjReference));
				list.Add(new MemberInfo(MemberName.OuterGroupDynamicIndex, Token.Int32));
				list.Add(new MemberInfo(MemberName.RowIndexes, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
				list.Add(new MemberInfo(MemberName.ColumnIndexes, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
				list.Add(new MemberInfo(MemberName.CellNonCustomAggObjs, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.CellCustomAggObjs, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.CellAggValueList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjResult));
				list.Add(new MemberInfo(MemberName.RunningValueValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Array2D, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray));
				list.Add(new MemberInfo(MemberName.DataRows, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
				list.Add(new MemberInfo(MemberName.Innermost, Token.Boolean));
				list.Add(new MemberInfo(MemberName.FirstRow, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow));
				list.Add(new MemberInfo(MemberName.FirstRowIsAggregate, Token.Boolean));
				list.Add(new MemberInfo(MemberName.NextCell, Token.Int32));
				list.Add(new MemberInfo(MemberName.SortFilterExpressionScopeInfoIndices, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Int32));
				list.Add(new MemberInfo(MemberName.AggregatesOfAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BucketedDataAggregateObjs));
				list.Add(new MemberInfo(MemberName.PostSortAggregatesOfAggregates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BucketedDataAggregateObjs));
				list.Add(new MemberInfo(MemberName.CanonicalCellScope, Token.Int32));
				list.Add(new MemberInfo(MemberName.DataAction, Token.Enum));
				list.Add(new MemberInfo(MemberName.RunningValueOfAggregateValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Array2D, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray));
				list.Add(new MemberInfo(MemberName.ScopeInstanceNumber, Token.Int64));
				list.Add(new MemberInfo(MemberName.HasProcessedAggregateRow, Token.Boolean));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCell, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}

		public void SetReference(IReference selfRef)
		{
			m_selfReference = (RuntimeCellReference)selfRef;
		}
	}
}

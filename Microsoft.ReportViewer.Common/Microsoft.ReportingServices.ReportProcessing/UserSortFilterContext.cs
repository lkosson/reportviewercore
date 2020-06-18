using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class UserSortFilterContext
	{
		private TextBox m_currentSortFilterEventSource;

		private RuntimeSortFilterEventInfoList m_runtimeSortFilterInfo;

		private ReportProcessing.IScope m_currentContainingScope;

		private GroupingList m_containingScopes;

		private int m_dataSetID = -1;

		private SubReportList m_detailScopeSubReports;

		private int[] m_inProcessUserSortPhase;

		internal TextBox CurrentSortFilterEventSource
		{
			get
			{
				return m_currentSortFilterEventSource;
			}
			set
			{
				m_currentSortFilterEventSource = value;
			}
		}

		internal RuntimeSortFilterEventInfoList RuntimeSortFilterInfo
		{
			get
			{
				return m_runtimeSortFilterInfo;
			}
			set
			{
				m_runtimeSortFilterInfo = value;
			}
		}

		internal int DataSetID
		{
			get
			{
				return m_dataSetID;
			}
			set
			{
				m_dataSetID = value;
			}
		}

		internal ReportProcessing.IScope CurrentContainingScope
		{
			get
			{
				return m_currentContainingScope;
			}
			set
			{
				m_currentContainingScope = value;
			}
		}

		internal GroupingList ContainingScopes
		{
			get
			{
				return m_containingScopes;
			}
			set
			{
				m_containingScopes = value;
			}
		}

		internal SubReportList DetailScopeSubReports
		{
			get
			{
				return m_detailScopeSubReports;
			}
			set
			{
				m_detailScopeSubReports = value;
			}
		}

		internal UserSortFilterContext()
		{
		}

		internal UserSortFilterContext(UserSortFilterContext copy)
		{
			m_runtimeSortFilterInfo = copy.RuntimeSortFilterInfo;
			m_currentContainingScope = copy.CurrentContainingScope;
			m_containingScopes = copy.ContainingScopes;
			m_dataSetID = copy.DataSetID;
			m_detailScopeSubReports = copy.DetailScopeSubReports;
			m_inProcessUserSortPhase = copy.m_inProcessUserSortPhase;
		}

		internal UserSortFilterContext(UserSortFilterContext parentContext, SubReport subReport)
		{
			m_runtimeSortFilterInfo = parentContext.RuntimeSortFilterInfo;
			m_dataSetID = parentContext.DataSetID;
			m_containingScopes = subReport.ContainingScopes;
			m_detailScopeSubReports = subReport.DetailScopeSubReports;
			m_inProcessUserSortPhase = parentContext.m_inProcessUserSortPhase;
		}

		internal bool PopulateRuntimeSortFilterEventInfo(ReportProcessing.ProcessingContext pc, DataSet myDataSet)
		{
			if (pc.UserSortFilterInfo == null || pc.UserSortFilterInfo.SortInfo == null || pc.OldSortFilterEventInfo == null)
			{
				return false;
			}
			if (m_dataSetID != -1)
			{
				return false;
			}
			m_runtimeSortFilterInfo = null;
			EventInformation.SortEventInfo sortInfo = pc.UserSortFilterInfo.SortInfo;
			for (int i = 0; i < sortInfo.Count; i++)
			{
				int uniqueNameAt = sortInfo.GetUniqueNameAt(i);
				SortFilterEventInfo sortFilterEventInfo = pc.OldSortFilterEventInfo[uniqueNameAt];
				if (sortFilterEventInfo != null && sortFilterEventInfo.EventSource.UserSort != null && sortFilterEventInfo.EventSource.UserSort.DataSetID == myDataSet.ID)
				{
					if (m_runtimeSortFilterInfo == null)
					{
						m_runtimeSortFilterInfo = new RuntimeSortFilterEventInfoList();
					}
					m_runtimeSortFilterInfo.Add(new RuntimeSortFilterEventInfo(sortFilterEventInfo.EventSource, uniqueNameAt, sortInfo.GetSortDirectionAt(i), sortFilterEventInfo.EventSourceScopeInfo));
				}
			}
			if (m_runtimeSortFilterInfo != null)
			{
				int count = m_runtimeSortFilterInfo.Count;
				for (int j = 0; j < count; j++)
				{
					TextBox eventSource = m_runtimeSortFilterInfo[j].EventSource;
					ISortFilterScope sortExpressionScope = eventSource.UserSort.SortExpressionScope;
					if (sortExpressionScope != null)
					{
						sortExpressionScope.IsSortFilterExpressionScope = SetSortFilterInfo(sortExpressionScope.IsSortFilterExpressionScope, count, j);
					}
					ISortFilterScope sortTarget = eventSource.UserSort.SortTarget;
					if (sortTarget != null)
					{
						sortTarget.IsSortFilterTarget = SetSortFilterInfo(sortTarget.IsSortFilterTarget, count, j);
					}
					if (eventSource.ContainingScopes != null && 0 < eventSource.ContainingScopes.Count)
					{
						int num = 0;
						for (int k = 0; k < eventSource.ContainingScopes.Count; k++)
						{
							Grouping grouping = eventSource.ContainingScopes[k];
							VariantList variantList = m_runtimeSortFilterInfo[j].SortSourceScopeInfo[k];
							if (grouping != null)
							{
								if (grouping.SortFilterScopeInfo == null)
								{
									grouping.SortFilterScopeInfo = new VariantList[count];
									for (int l = 0; l < count; l++)
									{
										grouping.SortFilterScopeInfo[l] = null;
									}
									grouping.SortFilterScopeIndex = new int[count];
									for (int m = 0; m < count; m++)
									{
										grouping.SortFilterScopeIndex[m] = -1;
									}
								}
								grouping.SortFilterScopeInfo[j] = variantList;
								grouping.SortFilterScopeIndex[j] = k;
								continue;
							}
							SubReportList detailScopeSubReports = eventSource.UserSort.DetailScopeSubReports;
							ReportItem parent;
							if (detailScopeSubReports != null && num < detailScopeSubReports.Count)
							{
								parent = detailScopeSubReports[num++].Parent;
							}
							else
							{
								Global.Tracer.Assert(k == eventSource.ContainingScopes.Count - 1, "(j == eventSource.ContainingScopes.Count - 1)");
								parent = eventSource.Parent;
							}
							while (parent != null && !(parent is DataRegion))
							{
								parent = parent.Parent;
							}
							Global.Tracer.Assert(parent is DataRegion, "(parent is DataRegion)");
							DataRegion dataRegion = (DataRegion)parent;
							if (dataRegion.SortFilterSourceDetailScopeInfo == null)
							{
								dataRegion.SortFilterSourceDetailScopeInfo = new int[count];
								for (int n = 0; n < count; n++)
								{
									dataRegion.SortFilterSourceDetailScopeInfo[n] = -1;
								}
							}
							Global.Tracer.Assert(variantList != null && 1 == variantList.Count, "(null != scopeValues && 1 == scopeValues.Count)");
							dataRegion.SortFilterSourceDetailScopeInfo[j] = (int)variantList[0];
						}
					}
					GroupingList groupsInSortTarget = eventSource.UserSort.GroupsInSortTarget;
					if (groupsInSortTarget != null)
					{
						for (int num2 = 0; num2 < groupsInSortTarget.Count; num2++)
						{
							groupsInSortTarget[num2].NeedScopeInfoForSortFilterExpression = SetSortFilterInfo(groupsInSortTarget[num2].NeedScopeInfoForSortFilterExpression, count, j);
						}
					}
					IntList peerSortFilters = eventSource.GetPeerSortFilters(create: false);
					if (peerSortFilters == null)
					{
						continue;
					}
					if (m_runtimeSortFilterInfo[j].PeerSortFilters == null)
					{
						m_runtimeSortFilterInfo[j].PeerSortFilters = new Hashtable();
					}
					for (int num3 = 0; num3 < peerSortFilters.Count; num3++)
					{
						if (eventSource.ID != peerSortFilters[num3])
						{
							m_runtimeSortFilterInfo[j].PeerSortFilters.Add(peerSortFilters[num3], null);
						}
					}
				}
			}
			return true;
		}

		private bool[] SetSortFilterInfo(bool[] source, int count, int index)
		{
			bool[] array = source;
			if (array == null)
			{
				array = new bool[count];
				for (int i = 0; i < count; i++)
				{
					array[i] = false;
				}
			}
			array[index] = true;
			return array;
		}

		internal bool IsSortFilterTarget(bool[] isSortFilterTarget, ReportProcessing.IScope outerScope, ReportProcessing.IHierarchyObj target, ref RuntimeUserSortTargetInfo userSortTargetInfo)
		{
			bool result = false;
			if (m_runtimeSortFilterInfo != null && isSortFilterTarget != null && (outerScope == null || !outerScope.TargetForNonDetailSort))
			{
				for (int i = 0; i < m_runtimeSortFilterInfo.Count; i++)
				{
					RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = m_runtimeSortFilterInfo[i];
					if (runtimeSortFilterEventInfo.EventTarget == null && isSortFilterTarget[i] && (outerScope == null || outerScope.TargetScopeMatched(i, detailSort: false)))
					{
						runtimeSortFilterEventInfo.EventTarget = target;
						if (userSortTargetInfo == null)
						{
							userSortTargetInfo = new RuntimeUserSortTargetInfo(target, i, runtimeSortFilterEventInfo);
						}
						else
						{
							userSortTargetInfo.AddSortInfo(target, i, runtimeSortFilterEventInfo);
						}
						result = true;
					}
				}
			}
			return result;
		}

		internal void RegisterSortFilterExpressionScope(ReportProcessing.IScope container, ReportProcessing.RuntimeDataRegionObj scopeObj, bool[] isSortFilterExpressionScope)
		{
			RuntimeSortFilterEventInfoList runtimeSortFilterInfo = m_runtimeSortFilterInfo;
			if (runtimeSortFilterInfo == null || isSortFilterExpressionScope == null || scopeObj == null)
			{
				return;
			}
			VariantList[] array = null;
			for (int i = 0; i < runtimeSortFilterInfo.Count; i++)
			{
				if (isSortFilterExpressionScope[i] && scopeObj.IsTargetForSort(i, detailSort: false) && scopeObj.TargetScopeMatched(i, detailSort: false))
				{
					if (array == null && runtimeSortFilterInfo[i].EventSource.UserSort.GroupsInSortTarget != null)
					{
						int index = 0;
						array = new VariantList[runtimeSortFilterInfo[i].EventSource.UserSort.GroupsInSortTarget.Count];
						scopeObj.GetScopeValues(runtimeSortFilterInfo[i].EventTarget, array, ref index);
					}
					runtimeSortFilterInfo[i].RegisterSortFilterExpressionScope(ref container.SortFilterExpressionScopeInfoIndices[i], scopeObj, array, i);
				}
			}
		}

		internal bool ProcessUserSort(ReportProcessing.ProcessingContext processingContext)
		{
			bool processedAny = false;
			RuntimeSortFilterEventInfoList runtimeSortFilterInfo = m_runtimeSortFilterInfo;
			if (runtimeSortFilterInfo != null)
			{
				bool processed;
				bool canStop;
				do
				{
					processed = false;
					canStop = true;
					ProcessUserSort(processingContext, ref processed, ref canStop, ref processedAny);
				}
				while (processed && !canStop);
			}
			return processedAny;
		}

		private void ProcessUserSort(ReportProcessing.ProcessingContext processingContext, ref bool processed, ref bool canStop, ref bool processedAny)
		{
			RuntimeSortFilterEventInfoList runtimeSortFilterInfo = m_runtimeSortFilterInfo;
			for (int i = 0; i < runtimeSortFilterInfo.Count; i++)
			{
				if (!runtimeSortFilterInfo[i].Processed)
				{
					runtimeSortFilterInfo[i].PrepareForSorting(processingContext);
				}
			}
			for (int j = 0; j < runtimeSortFilterInfo.Count; j++)
			{
				if (!runtimeSortFilterInfo[j].Processed)
				{
					if (runtimeSortFilterInfo[j].ProcessSorting(processingContext))
					{
						processedAny = true;
						processed = true;
					}
					else
					{
						canStop = false;
					}
				}
			}
		}

		internal void ProcessUserSortForTarget(ObjectModelImpl reportObjectModel, ReportRuntime reportRuntime, ReportProcessing.IHierarchyObj target, ref ReportProcessing.DataRowList dataRows, bool targetForNonDetailSort)
		{
			if (targetForNonDetailSort && dataRows != null && 0 < dataRows.Count)
			{
				RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = null;
				IntList sortFilterInfoIndices = target.SortFilterInfoIndices;
				Global.Tracer.Assert(target.SortTree != null, "(null != target.SortTree)");
				if (sortFilterInfoIndices != null)
				{
					runtimeSortFilterEventInfo = m_runtimeSortFilterInfo[sortFilterInfoIndices[0]];
				}
				for (int i = 0; i < dataRows.Count; i++)
				{
					reportObjectModel.FieldsImpl.SetFields(dataRows[i]);
					object keyValue = DBNull.Value;
					if (runtimeSortFilterEventInfo != null)
					{
						keyValue = runtimeSortFilterEventInfo.GetSortOrder(reportRuntime);
					}
					target.SortTree.NextRow(keyValue);
				}
				dataRows = null;
			}
			target.MarkSortInfoProcessed(m_runtimeSortFilterInfo);
		}

		internal void EnterProcessUserSortPhase(int index)
		{
			if (m_inProcessUserSortPhase == null)
			{
				if (m_runtimeSortFilterInfo == null || m_runtimeSortFilterInfo.Count == 0)
				{
					return;
				}
				m_inProcessUserSortPhase = new int[m_runtimeSortFilterInfo.Count];
				for (int i = 0; i < m_runtimeSortFilterInfo.Count; i++)
				{
					m_inProcessUserSortPhase[i] = 0;
				}
			}
			m_inProcessUserSortPhase[index]++;
		}

		internal void LeaveProcessUserSortPhase(int index)
		{
			if (m_inProcessUserSortPhase != null)
			{
				m_inProcessUserSortPhase[index]--;
				Global.Tracer.Assert(0 <= m_inProcessUserSortPhase[index], "(0 <= m_inProcessUserSortPhase[index])");
			}
		}

		internal bool InProcessUserSortPhase(int index)
		{
			if (m_inProcessUserSortPhase == null)
			{
				return false;
			}
			return m_inProcessUserSortPhase[index] > 0;
		}

		internal void UpdateContextFromDataSet(UserSortFilterContext dataSetContext)
		{
			if (-1 == m_dataSetID)
			{
				m_dataSetID = dataSetContext.DataSetID;
				m_runtimeSortFilterInfo = dataSetContext.RuntimeSortFilterInfo;
			}
		}
	}
}

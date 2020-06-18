using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class UserSortFilterContext
	{
		private IInScopeEventSource m_currentSortFilterEventSource;

		private List<IReference<RuntimeSortFilterEventInfo>> m_runtimeSortFilterInfo;

		private IReference<IScope> m_currentContainingScope;

		private Microsoft.ReportingServices.ReportIntermediateFormat.GroupingList m_containingScopes;

		private int m_dataSetGlobalID = -1;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.SubReport> m_detailScopeSubReports;

		private int[] m_inProcessUserSortPhase;

		internal IInScopeEventSource CurrentSortFilterEventSource
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

		internal List<IReference<RuntimeSortFilterEventInfo>> RuntimeSortFilterInfo => m_runtimeSortFilterInfo;

		internal int DataSetGlobalId
		{
			get
			{
				return m_dataSetGlobalID;
			}
			set
			{
				m_dataSetGlobalID = value;
			}
		}

		internal IReference<IScope> CurrentContainingScope
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

		internal Microsoft.ReportingServices.ReportIntermediateFormat.GroupingList ContainingScopes
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

		internal List<Microsoft.ReportingServices.ReportIntermediateFormat.SubReport> DetailScopeSubReports
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

		internal UserSortFilterContext(UserSortFilterContext parentContext, Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport)
		{
			m_runtimeSortFilterInfo = parentContext.RuntimeSortFilterInfo;
			m_dataSetGlobalID = parentContext.DataSetGlobalId;
			m_inProcessUserSortPhase = parentContext.m_inProcessUserSortPhase;
			subReport.UpdateSubReportScopes(parentContext);
			m_containingScopes = subReport.ContainingScopes;
			m_detailScopeSubReports = subReport.DetailScopeSubReports;
		}

		internal List<Microsoft.ReportingServices.ReportIntermediateFormat.SubReport> CloneDetailScopeSubReports()
		{
			if (m_detailScopeSubReports == null)
			{
				return null;
			}
			int count = m_detailScopeSubReports.Count;
			List<Microsoft.ReportingServices.ReportIntermediateFormat.SubReport> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.SubReport>(count);
			for (int i = 0; i < count; i++)
			{
				list.Add(m_detailScopeSubReports[i]);
			}
			return list;
		}

		internal void ResetContextForTopLevelDataSet()
		{
			m_dataSetGlobalID = -1;
			m_currentSortFilterEventSource = null;
			m_runtimeSortFilterInfo = null;
			m_currentContainingScope = null;
			m_containingScopes = null;
			m_inProcessUserSortPhase = null;
		}

		internal void UpdateContextForFirstSubreportInstance(UserSortFilterContext parentContext)
		{
			if (-1 == m_dataSetGlobalID)
			{
				m_dataSetGlobalID = parentContext.DataSetGlobalId;
				m_runtimeSortFilterInfo = parentContext.RuntimeSortFilterInfo;
				m_inProcessUserSortPhase = parentContext.m_inProcessUserSortPhase;
			}
		}

		internal static void ProcessEventSources(OnDemandProcessingContext odpContext, IScope containingScope, List<IInScopeEventSource> inScopeEventSources)
		{
			if (inScopeEventSources == null || inScopeEventSources.Count == 0)
			{
				return;
			}
			foreach (IInScopeEventSource inScopeEventSource in inScopeEventSources)
			{
				if (inScopeEventSource.UserSort == null)
				{
					continue;
				}
				Microsoft.ReportingServices.ReportIntermediateFormat.SortFilterEventInfo sortFilterEventInfo = new Microsoft.ReportingServices.ReportIntermediateFormat.SortFilterEventInfo(inScopeEventSource);
				sortFilterEventInfo.EventSourceScopeInfo = odpContext.GetScopeValues(inScopeEventSource.ContainingScopes, containingScope);
				if (odpContext.TopLevelContext.NewSortFilterEventInfo == null)
				{
					odpContext.TopLevelContext.NewSortFilterEventInfo = new SortFilterEventInfoMap();
				}
				string text = InstancePathItem.GenerateUniqueNameString(inScopeEventSource.ID, inScopeEventSource.InstancePath);
				odpContext.TopLevelContext.NewSortFilterEventInfo.Add(text, sortFilterEventInfo);
				List<IReference<RuntimeSortFilterEventInfo>> list = odpContext.RuntimeSortFilterInfo;
				if (list == null && odpContext.SubReportUniqueName == null)
				{
					list = odpContext.TopLevelContext.ReportRuntimeUserSortFilterInfo;
				}
				if (list == null)
				{
					continue;
				}
				for (int i = 0; i < list.Count; i++)
				{
					IReference<RuntimeSortFilterEventInfo> reference = list[i];
					using (reference.PinValue())
					{
						reference.Value().MatchEventSource(inScopeEventSource, text, containingScope, odpContext);
					}
				}
			}
		}

		internal bool PopulateRuntimeSortFilterEventInfo(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet myDataSet)
		{
			if (odpContext.TopLevelContext.UserSortFilterInfo == null || odpContext.TopLevelContext.UserSortFilterInfo.OdpSortInfo == null || odpContext.TopLevelContext.OldSortFilterEventInfo == null)
			{
				return false;
			}
			if (-1 != m_dataSetGlobalID)
			{
				return false;
			}
			m_runtimeSortFilterInfo = null;
			EventInformation.OdpSortEventInfo odpSortInfo = odpContext.TopLevelContext.UserSortFilterInfo.OdpSortInfo;
			for (int i = 0; i < odpSortInfo.Count; i++)
			{
				string uniqueNameAt = odpSortInfo.GetUniqueNameAt(i);
				Microsoft.ReportingServices.ReportIntermediateFormat.SortFilterEventInfo sortFilterEventInfo = odpContext.TopLevelContext.OldSortFilterEventInfo[uniqueNameAt];
				if (sortFilterEventInfo == null || sortFilterEventInfo.EventSource.UserSort == null)
				{
					continue;
				}
				int num = sortFilterEventInfo.EventSource.UserSort.SubReportDataSetGlobalId;
				if (-1 == num)
				{
					num = sortFilterEventInfo.EventSource.UserSort.DataSet.GlobalID;
				}
				if (num == myDataSet.GlobalID)
				{
					if (m_runtimeSortFilterInfo == null)
					{
						m_runtimeSortFilterInfo = new List<IReference<RuntimeSortFilterEventInfo>>();
					}
					RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = new RuntimeSortFilterEventInfo(sortFilterEventInfo.EventSource, uniqueNameAt, odpSortInfo.GetSortDirectionAt(i), sortFilterEventInfo.EventSourceScopeInfo, odpContext, (m_currentContainingScope == null) ? 1 : m_currentContainingScope.Value().Depth);
					runtimeSortFilterEventInfo.SelfReference.UnPinValue();
					m_runtimeSortFilterInfo.Add(runtimeSortFilterEventInfo.SelfReference);
				}
			}
			if (m_runtimeSortFilterInfo != null)
			{
				int count = m_runtimeSortFilterInfo.Count;
				for (int j = 0; j < count; j++)
				{
					IReference<RuntimeSortFilterEventInfo> reference = m_runtimeSortFilterInfo[j];
					using (reference.PinValue())
					{
						RuntimeSortFilterEventInfo runtimeSortFilterEventInfo2 = reference.Value();
						IInScopeEventSource eventSource = runtimeSortFilterEventInfo2.EventSource;
						Microsoft.ReportingServices.ReportIntermediateFormat.ISortFilterScope sortExpressionScope = eventSource.UserSort.SortExpressionScope;
						if (sortExpressionScope != null)
						{
							sortExpressionScope.IsSortFilterExpressionScope = SetSortFilterInfo(sortExpressionScope.IsSortFilterExpressionScope, count, j);
						}
						Microsoft.ReportingServices.ReportIntermediateFormat.ISortFilterScope sortTarget = eventSource.UserSort.SortTarget;
						if (sortTarget != null)
						{
							sortTarget.IsSortFilterTarget = SetSortFilterInfo(sortTarget.IsSortFilterTarget, count, j);
						}
						if (eventSource.ContainingScopes != null && 0 < eventSource.ContainingScopes.Count)
						{
							int num2 = 0;
							int num3 = 0;
							for (int k = 0; k < eventSource.ContainingScopes.Count; k++)
							{
								Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = eventSource.ContainingScopes[k];
								if (grouping != null && grouping.IsDetail)
								{
									continue;
								}
								List<object> list = runtimeSortFilterEventInfo2.SortSourceScopeInfo[num2];
								if (grouping != null)
								{
									if (grouping.SortFilterScopeInfo == null)
									{
										grouping.SortFilterScopeInfo = new List<object>[count];
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
									grouping.SortFilterScopeInfo[j] = list;
									grouping.SortFilterScopeIndex[j] = num2;
								}
								else
								{
									List<Microsoft.ReportingServices.ReportIntermediateFormat.SubReport> detailScopeSubReports = eventSource.UserSort.DetailScopeSubReports;
									Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem reportItem = (detailScopeSubReports == null || num3 >= detailScopeSubReports.Count) ? eventSource.Parent : detailScopeSubReports[num3++].Parent;
									while (reportItem != null && !reportItem.IsDataRegion)
									{
										reportItem = reportItem.Parent;
									}
									Global.Tracer.Assert(reportItem.IsDataRegion, "(parent.IsDataRegion)");
									Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = (Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion)reportItem;
									if (dataRegion.SortFilterSourceDetailScopeInfo == null)
									{
										dataRegion.SortFilterSourceDetailScopeInfo = new int[count];
										for (int n = 0; n < count; n++)
										{
											dataRegion.SortFilterSourceDetailScopeInfo[n] = -1;
										}
									}
									Global.Tracer.Assert(list != null && 1 == list.Count, "(null != scopeValues && 1 == scopeValues.Count)");
									dataRegion.SortFilterSourceDetailScopeInfo[j] = (int)list[0];
								}
								num2++;
							}
						}
						Microsoft.ReportingServices.ReportIntermediateFormat.GroupingList groupsInSortTarget = eventSource.UserSort.GroupsInSortTarget;
						if (groupsInSortTarget != null)
						{
							for (int num4 = 0; num4 < groupsInSortTarget.Count; num4++)
							{
								groupsInSortTarget[num4].NeedScopeInfoForSortFilterExpression = SetSortFilterInfo(groupsInSortTarget[num4].NeedScopeInfoForSortFilterExpression, count, j);
							}
						}
						List<int> peerSortFilters = eventSource.GetPeerSortFilters(create: false);
						if (peerSortFilters == null || peerSortFilters.Count == 0)
						{
							continue;
						}
						if (runtimeSortFilterEventInfo2.PeerSortFilters == null)
						{
							runtimeSortFilterEventInfo2.PeerSortFilters = new Hashtable();
						}
						for (int num5 = 0; num5 < peerSortFilters.Count; num5++)
						{
							if (eventSource.ID != peerSortFilters[num5])
							{
								runtimeSortFilterEventInfo2.PeerSortFilters.Add(peerSortFilters[num5], null);
							}
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

		internal bool IsSortFilterTarget(bool[] isSortFilterTarget, IReference<IScope> outerScope, IReference<IHierarchyObj> target, ref RuntimeUserSortTargetInfo userSortTargetInfo)
		{
			bool result = false;
			if (m_runtimeSortFilterInfo != null && isSortFilterTarget != null && (outerScope == null || !outerScope.Value().TargetForNonDetailSort))
			{
				for (int i = 0; i < m_runtimeSortFilterInfo.Count; i++)
				{
					IReference<RuntimeSortFilterEventInfo> reference = m_runtimeSortFilterInfo[i];
					using (reference.PinValue())
					{
						RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
						if (isSortFilterTarget[i] && (outerScope == null || outerScope.Value().TargetScopeMatched(i, detailSort: false)))
						{
							runtimeSortFilterEventInfo.EventTarget = target;
							runtimeSortFilterEventInfo.Processed = false;
							if (userSortTargetInfo == null)
							{
								userSortTargetInfo = new RuntimeUserSortTargetInfo(target, i, reference);
							}
							else
							{
								userSortTargetInfo.AddSortInfo(target, i, reference);
							}
							result = true;
						}
					}
				}
			}
			return result;
		}

		internal void RegisterSortFilterExpressionScope(IReference<IScope> containerRef, IReference<RuntimeDataRegionObj> scopeRef, bool[] isSortFilterExpressionScope)
		{
			List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo = m_runtimeSortFilterInfo;
			if (runtimeSortFilterInfo == null || isSortFilterExpressionScope == null || scopeRef == null)
			{
				return;
			}
			List<object>[] array = null;
			using (scopeRef.PinValue())
			{
				RuntimeDataRegionObj runtimeDataRegionObj = scopeRef.Value();
				using (containerRef.PinValue())
				{
					IScope scope = containerRef.Value();
					for (int i = 0; i < runtimeSortFilterInfo.Count; i++)
					{
						if (!isSortFilterExpressionScope[i] || !runtimeDataRegionObj.IsTargetForSort(i, detailSort: false) || !runtimeDataRegionObj.TargetScopeMatched(i, detailSort: false))
						{
							continue;
						}
						IReference<RuntimeSortFilterEventInfo> reference = runtimeSortFilterInfo[i];
						using (reference.PinValue())
						{
							RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
							if (array == null && runtimeSortFilterEventInfo.EventSource.UserSort.GroupsInSortTarget != null)
							{
								int index = 0;
								array = new List<object>[runtimeSortFilterEventInfo.EventSource.UserSort.GroupsInSortTarget.Count];
								runtimeDataRegionObj.GetScopeValues(runtimeSortFilterEventInfo.EventTarget, array, ref index);
							}
							runtimeSortFilterEventInfo.RegisterSortFilterExpressionScope(ref scope.SortFilterExpressionScopeInfoIndices[i], scopeRef, array, i);
						}
					}
				}
			}
		}

		internal bool ProcessUserSort(OnDemandProcessingContext odpContext)
		{
			bool processedAny = false;
			bool processed = false;
			List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo = m_runtimeSortFilterInfo;
			if (runtimeSortFilterInfo != null)
			{
				bool flag;
				bool canStop;
				do
				{
					flag = processed;
					processed = false;
					canStop = true;
					ProcessUserSort(odpContext, ref processed, ref canStop, ref processedAny);
				}
				while (processed || (!canStop && flag));
			}
			return processedAny;
		}

		private void ProcessUserSort(OnDemandProcessingContext odpContext, ref bool processed, ref bool canStop, ref bool processedAny)
		{
			List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo = m_runtimeSortFilterInfo;
			for (int i = 0; i < runtimeSortFilterInfo.Count; i++)
			{
				IReference<RuntimeSortFilterEventInfo> reference = runtimeSortFilterInfo[i];
				using (reference.PinValue())
				{
					RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
					if (!runtimeSortFilterEventInfo.Processed)
					{
						runtimeSortFilterEventInfo.PrepareForSorting(odpContext);
					}
				}
			}
			for (int j = 0; j < runtimeSortFilterInfo.Count; j++)
			{
				IReference<RuntimeSortFilterEventInfo> reference2 = runtimeSortFilterInfo[j];
				using (reference2.PinValue())
				{
					RuntimeSortFilterEventInfo runtimeSortFilterEventInfo2 = reference2.Value();
					if (!runtimeSortFilterEventInfo2.Processed)
					{
						if (runtimeSortFilterEventInfo2.ProcessSorting(odpContext))
						{
							processedAny = true;
							processed = true;
							odpContext.FirstPassPostProcess();
							return;
						}
						canStop = false;
					}
				}
			}
		}

		internal void ProcessUserSortForTarget(ObjectModelImpl reportObjectModel, Microsoft.ReportingServices.RdlExpressions.ReportRuntime reportRuntime, IReference<IHierarchyObj> target, ref ScalableList<DataFieldRow> dataRows, bool targetForNonDetailSort)
		{
			using (target.PinValue())
			{
				IHierarchyObj hierarchyObj = target.Value();
				if (targetForNonDetailSort && dataRows != null && 0 < dataRows.Count)
				{
					IReference<RuntimeSortFilterEventInfo> reference = null;
					try
					{
						List<int> sortFilterInfoIndices = hierarchyObj.SortFilterInfoIndices;
						Global.Tracer.Assert(hierarchyObj.SortTree != null, "(null != targetObj.SortTree)");
						if (sortFilterInfoIndices != null)
						{
							reference = m_runtimeSortFilterInfo[sortFilterInfoIndices[0]];
						}
						RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = null;
						if (reference != null)
						{
							reference.PinValue();
							runtimeSortFilterEventInfo = reference.Value();
						}
						for (int i = 0; i < dataRows.Count; i++)
						{
							dataRows[i].SetFields(reportObjectModel.FieldsImpl);
							object keyValue = DBNull.Value;
							if (runtimeSortFilterEventInfo != null)
							{
								keyValue = runtimeSortFilterEventInfo.GetSortOrder(reportRuntime);
							}
							hierarchyObj.SortTree.NextRow(keyValue, hierarchyObj);
						}
					}
					finally
					{
						reference?.UnPinValue();
					}
					if (dataRows != null)
					{
						dataRows.Dispose();
					}
					dataRows = null;
				}
				hierarchyObj.MarkSortInfoProcessed(m_runtimeSortFilterInfo);
			}
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
	}
}

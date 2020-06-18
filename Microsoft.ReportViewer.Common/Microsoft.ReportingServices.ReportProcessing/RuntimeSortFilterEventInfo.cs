using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class RuntimeSortFilterEventInfo
	{
		private class SortFilterExpressionScopeObj : ReportProcessing.IHierarchyObj
		{
			private RuntimeSortFilterEventInfo m_owner;

			private ReportProcessing.RuntimeDataRegionObjList m_scopeInstances;

			private ArrayList m_scopeValuesList;

			private ReportProcessing.BTreeNode m_sortTree;

			private int m_currentScopeInstanceIndex = -1;

			internal int CurrentScopeInstanceIndex => m_currentScopeInstanceIndex;

			internal bool SortDirection => m_owner.SortDirection;

			ReportProcessing.IHierarchyObj ReportProcessing.IHierarchyObj.HierarchyRoot => this;

			ReportProcessing.ProcessingContext ReportProcessing.IHierarchyObj.ProcessingContext
			{
				get
				{
					Global.Tracer.Assert(0 < m_scopeInstances.Count, "(0 < m_scopeInstances.Count)");
					return m_scopeInstances[0].ProcessingContext;
				}
			}

			ReportProcessing.BTreeNode ReportProcessing.IHierarchyObj.SortTree
			{
				get
				{
					return m_sortTree;
				}
				set
				{
					m_sortTree = value;
				}
			}

			int ReportProcessing.IHierarchyObj.ExpressionIndex => 0;

			IntList ReportProcessing.IHierarchyObj.SortFilterInfoIndices => null;

			bool ReportProcessing.IHierarchyObj.IsDetail => false;

			internal SortFilterExpressionScopeObj(RuntimeSortFilterEventInfo owner)
			{
				m_owner = owner;
				m_scopeInstances = new ReportProcessing.RuntimeDataRegionObjList();
				m_scopeValuesList = new ArrayList();
			}

			ReportProcessing.IHierarchyObj ReportProcessing.IHierarchyObj.CreateHierarchyObj()
			{
				return new SortExpressionScopeInstanceHolder(this);
			}

			ProcessingMessageList ReportProcessing.IHierarchyObj.RegisterComparisonError(string propertyName)
			{
				return ((ReportProcessing.IHierarchyObj)this).ProcessingContext.RegisterComparisonErrorForSortFilterEvent(propertyName);
			}

			void ReportProcessing.IHierarchyObj.NextRow()
			{
				Global.Tracer.Assert(condition: false);
			}

			void ReportProcessing.IHierarchyObj.Traverse(ReportProcessing.ProcessingStages operation)
			{
				if (m_sortTree != null)
				{
					m_sortTree.Traverse(operation, m_owner.SortDirection);
				}
			}

			void ReportProcessing.IHierarchyObj.ReadRow()
			{
				Global.Tracer.Assert(condition: false);
			}

			void ReportProcessing.IHierarchyObj.ProcessUserSort()
			{
				Global.Tracer.Assert(condition: false);
			}

			void ReportProcessing.IHierarchyObj.MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				Global.Tracer.Assert(condition: false);
			}

			void ReportProcessing.IHierarchyObj.AddSortInfoIndex(int sortFilterInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				Global.Tracer.Assert(condition: false);
			}

			internal void RegisterScopeInstance(ReportProcessing.RuntimeDataRegionObj scopeObj, VariantList[] scopeValues)
			{
				m_scopeInstances.Add(scopeObj);
				m_scopeValuesList.Add(scopeValues);
			}

			internal void SortSEScopes(ReportProcessing.ProcessingContext processingContext, TextBox eventSource)
			{
				m_sortTree = new ReportProcessing.BTreeNode(this);
				for (int i = 0; i < m_scopeInstances.Count; i++)
				{
					ReportProcessing.RuntimeDataRegionObj runtimeDataRegionObj = m_scopeInstances[i];
					m_currentScopeInstanceIndex = i;
					runtimeDataRegionObj.SetupEnvironment();
					m_sortTree.NextRow(processingContext.ReportRuntime.EvaluateUserSortExpression(eventSource));
				}
			}

			internal void AddSortOrder(int scopeInstanceIndex, bool incrementCounter)
			{
				m_owner.AddSortOrder((VariantList[])m_scopeValuesList[scopeInstanceIndex], incrementCounter);
			}
		}

		private class SortExpressionScopeInstanceHolder : ReportProcessing.IHierarchyObj
		{
			private SortFilterExpressionScopeObj m_owner;

			private IntList m_scopeInstanceIndices;

			ReportProcessing.IHierarchyObj ReportProcessing.IHierarchyObj.HierarchyRoot => this;

			ReportProcessing.ProcessingContext ReportProcessing.IHierarchyObj.ProcessingContext => ((ReportProcessing.IHierarchyObj)m_owner).ProcessingContext;

			ReportProcessing.BTreeNode ReportProcessing.IHierarchyObj.SortTree
			{
				get
				{
					return null;
				}
				set
				{
					Global.Tracer.Assert(condition: false);
				}
			}

			int ReportProcessing.IHierarchyObj.ExpressionIndex => -1;

			IntList ReportProcessing.IHierarchyObj.SortFilterInfoIndices => null;

			bool ReportProcessing.IHierarchyObj.IsDetail => false;

			internal SortExpressionScopeInstanceHolder(SortFilterExpressionScopeObj owner)
			{
				m_owner = owner;
				m_scopeInstanceIndices = new IntList();
			}

			ReportProcessing.IHierarchyObj ReportProcessing.IHierarchyObj.CreateHierarchyObj()
			{
				Global.Tracer.Assert(condition: false);
				return null;
			}

			ProcessingMessageList ReportProcessing.IHierarchyObj.RegisterComparisonError(string propertyName)
			{
				Global.Tracer.Assert(condition: false);
				return null;
			}

			void ReportProcessing.IHierarchyObj.NextRow()
			{
				m_scopeInstanceIndices.Add(m_owner.CurrentScopeInstanceIndex);
			}

			void ReportProcessing.IHierarchyObj.Traverse(ReportProcessing.ProcessingStages operation)
			{
				if (m_owner.SortDirection)
				{
					for (int i = 0; i < m_scopeInstanceIndices.Count; i++)
					{
						m_owner.AddSortOrder(m_scopeInstanceIndices[i], i == m_scopeInstanceIndices.Count - 1);
					}
					return;
				}
				for (int num = m_scopeInstanceIndices.Count - 1; num >= 0; num--)
				{
					m_owner.AddSortOrder(m_scopeInstanceIndices[num], num == 0);
				}
			}

			void ReportProcessing.IHierarchyObj.ReadRow()
			{
				Global.Tracer.Assert(condition: false);
			}

			void ReportProcessing.IHierarchyObj.ProcessUserSort()
			{
				Global.Tracer.Assert(condition: false);
			}

			void ReportProcessing.IHierarchyObj.MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				Global.Tracer.Assert(condition: false);
			}

			void ReportProcessing.IHierarchyObj.AddSortInfoIndex(int sortFilterInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				Global.Tracer.Assert(condition: false);
			}
		}

		private TextBox m_eventSource;

		private int m_oldUniqueName;

		private VariantList[] m_sortSourceScopeInfo;

		private bool m_sortDirection;

		private ReportProcessing.IScope m_eventSourceScope;

		private int m_eventSourceDetailIndex = -1;

		private ReportProcessing.RuntimeDataRegionObjList m_detailScopes;

		private IntList m_detailScopeIndices;

		private ReportProcessing.IHierarchyObj m_eventTarget;

		private bool m_targetSortFilterInfoAdded;

		private ReportProcessing.RuntimeExpressionInfoList m_groupExpressionsInSortTarget;

		private ArrayList m_sortFilterExpressionScopeObjects;

		private int m_currentSortIndex = 1;

		private int m_currentInstanceIndex;

		private ScopeLookupTable m_sortOrders;

		private bool m_processed;

		private int m_nullScopeCount;

		private int m_newUniqueName = -1;

		private int m_page;

		private Hashtable m_peerSortFilters;

		internal TextBox EventSource => m_eventSource;

		internal ReportProcessing.IScope EventSourceScope
		{
			get
			{
				return m_eventSourceScope;
			}
			set
			{
				m_eventSourceScope = value;
			}
		}

		internal int EventSourceDetailIndex
		{
			get
			{
				return m_eventSourceDetailIndex;
			}
			set
			{
				m_eventSourceDetailIndex = value;
			}
		}

		internal ReportProcessing.RuntimeDataRegionObjList DetailScopes
		{
			get
			{
				return m_detailScopes;
			}
			set
			{
				m_detailScopes = value;
			}
		}

		internal IntList DetailScopeIndices
		{
			get
			{
				return m_detailScopeIndices;
			}
			set
			{
				m_detailScopeIndices = value;
			}
		}

		internal bool SortDirection
		{
			get
			{
				return m_sortDirection;
			}
			set
			{
				m_sortDirection = value;
			}
		}

		internal VariantList[] SortSourceScopeInfo => m_sortSourceScopeInfo;

		internal ReportProcessing.IHierarchyObj EventTarget
		{
			get
			{
				return m_eventTarget;
			}
			set
			{
				m_eventTarget = value;
			}
		}

		internal bool TargetSortFilterInfoAdded
		{
			get
			{
				return m_targetSortFilterInfoAdded;
			}
			set
			{
				m_targetSortFilterInfoAdded = value;
			}
		}

		internal bool Processed
		{
			get
			{
				return m_processed;
			}
			set
			{
				m_processed = value;
			}
		}

		internal int OldUniqueName
		{
			get
			{
				return m_oldUniqueName;
			}
			set
			{
				m_oldUniqueName = value;
			}
		}

		internal int NewUniqueName
		{
			get
			{
				return m_newUniqueName;
			}
			set
			{
				m_newUniqueName = value;
			}
		}

		internal int Page
		{
			get
			{
				return m_page;
			}
			set
			{
				m_page = value;
			}
		}

		internal Hashtable PeerSortFilters
		{
			get
			{
				return m_peerSortFilters;
			}
			set
			{
				m_peerSortFilters = value;
			}
		}

		internal RuntimeSortFilterEventInfo(TextBox eventSource, int oldUniqueName, bool sortDirection, VariantList[] sortSourceScopeInfo)
		{
			Global.Tracer.Assert(eventSource != null && eventSource.UserSort != null, "(null != eventSource && null != eventSource.UserSort)");
			m_eventSource = eventSource;
			m_oldUniqueName = oldUniqueName;
			m_sortDirection = sortDirection;
			m_sortSourceScopeInfo = sortSourceScopeInfo;
		}

		internal void RegisterSortFilterExpressionScope(ref int containerSortFilterExprScopeIndex, ReportProcessing.RuntimeDataRegionObj scopeObj, VariantList[] scopeValues, int sortFilterInfoIndex)
		{
			if (m_eventTarget != null && !m_targetSortFilterInfoAdded)
			{
				m_eventTarget.AddSortInfoIndex(sortFilterInfoIndex, this);
			}
			SortFilterExpressionScopeObj sortFilterExpressionScopeObj;
			if (-1 != containerSortFilterExprScopeIndex)
			{
				sortFilterExpressionScopeObj = (SortFilterExpressionScopeObj)m_sortFilterExpressionScopeObjects[containerSortFilterExprScopeIndex];
			}
			else
			{
				if (m_sortFilterExpressionScopeObjects == null)
				{
					m_sortFilterExpressionScopeObjects = new ArrayList();
				}
				containerSortFilterExprScopeIndex = m_sortFilterExpressionScopeObjects.Count;
				sortFilterExpressionScopeObj = new SortFilterExpressionScopeObj(this);
				m_sortFilterExpressionScopeObjects.Add(sortFilterExpressionScopeObj);
			}
			sortFilterExpressionScopeObj.RegisterScopeInstance(scopeObj, scopeValues);
		}

		internal void PrepareForSorting(ReportProcessing.ProcessingContext processingContext)
		{
			Global.Tracer.Assert(!m_processed, "(!m_processed)");
			if (m_eventTarget == null || m_sortFilterExpressionScopeObjects == null)
			{
				return;
			}
			processingContext.UserSortFilterContext.CurrentSortFilterEventSource = m_eventSource;
			for (int i = 0; i < m_sortFilterExpressionScopeObjects.Count; i++)
			{
				((SortFilterExpressionScopeObj)m_sortFilterExpressionScopeObjects[i]).SortSEScopes(processingContext, m_eventSource);
			}
			GroupingList groupsInSortTarget = m_eventSource.UserSort.GroupsInSortTarget;
			if (groupsInSortTarget != null && 0 < groupsInSortTarget.Count)
			{
				m_groupExpressionsInSortTarget = new ReportProcessing.RuntimeExpressionInfoList();
				for (int j = 0; j < groupsInSortTarget.Count; j++)
				{
					Grouping grouping = groupsInSortTarget[j];
					for (int k = 0; k < grouping.GroupExpressions.Count; k++)
					{
						m_groupExpressionsInSortTarget.Add(new ReportProcessing.RuntimeExpressionInfo(grouping.GroupExpressions, grouping.ExprHost, null, k));
					}
				}
			}
			CollectSortOrders();
		}

		private void CollectSortOrders()
		{
			m_currentSortIndex = 1;
			for (int i = 0; i < m_sortFilterExpressionScopeObjects.Count; i++)
			{
				((ReportProcessing.IHierarchyObj)m_sortFilterExpressionScopeObjects[i]).Traverse(ReportProcessing.ProcessingStages.UserSortFilter);
			}
			m_sortFilterExpressionScopeObjects = null;
		}

		internal bool ProcessSorting(ReportProcessing.ProcessingContext processingContext)
		{
			Global.Tracer.Assert(!m_processed, "(!m_processed)");
			if (m_eventTarget == null)
			{
				return false;
			}
			m_eventTarget.ProcessUserSort();
			m_sortOrders = null;
			return true;
		}

		private void AddSortOrder(VariantList[] scopeValues, bool incrementCounter)
		{
			if (m_sortOrders == null)
			{
				m_sortOrders = new ScopeLookupTable();
			}
			if (scopeValues == null || scopeValues.Length == 0)
			{
				m_sortOrders.Add(m_eventSource.UserSort.GroupsInSortTarget, scopeValues, m_currentSortIndex);
			}
			else
			{
				int num = 0;
				for (int i = 0; i < scopeValues.Length; i++)
				{
					if (scopeValues[i] == null)
					{
						num++;
					}
				}
				if (num >= m_nullScopeCount)
				{
					if (num > m_nullScopeCount)
					{
						m_sortOrders.Clear();
						m_nullScopeCount = num;
					}
					m_sortOrders.Add(m_eventSource.UserSort.GroupsInSortTarget, scopeValues, m_currentSortIndex);
				}
			}
			if (incrementCounter)
			{
				m_currentSortIndex++;
			}
		}

		internal object GetSortOrder(ReportRuntime runtime)
		{
			object obj = null;
			if (m_eventSource.UserSort.SortExpressionScope == null)
			{
				obj = runtime.EvaluateUserSortExpression(m_eventSource);
			}
			else if (m_sortOrders == null)
			{
				obj = null;
			}
			else
			{
				GroupingList groupsInSortTarget = m_eventSource.UserSort.GroupsInSortTarget;
				if (groupsInSortTarget == null || groupsInSortTarget.Count == 0)
				{
					obj = m_sortOrders.LookupTable;
				}
				else
				{
					bool flag = true;
					bool flag2 = false;
					int num = 0;
					Hashtable hashtable = (Hashtable)m_sortOrders.LookupTable;
					int num2 = 0;
					int num3 = 0;
					while (num3 < groupsInSortTarget.Count)
					{
						IEnumerator enumerator = hashtable.Keys.GetEnumerator();
						enumerator.MoveNext();
						num2 = (int)enumerator.Current;
						for (int i = 0; i < num2; i++)
						{
							num += groupsInSortTarget[num3++].GroupExpressions.Count;
						}
						hashtable = (Hashtable)hashtable[num2];
						if (num3 < groupsInSortTarget.Count)
						{
							Grouping grouping = groupsInSortTarget[num3];
							for (int j = 0; j < grouping.GroupExpressions.Count; j++)
							{
								object key = runtime.EvaluateRuntimeExpression(m_groupExpressionsInSortTarget[num], ObjectType.Grouping, grouping.Name, "GroupExpression");
								num++;
								obj = hashtable[key];
								if (obj == null)
								{
									flag = false;
									break;
								}
								if (num < m_groupExpressionsInSortTarget.Count)
								{
									hashtable = (Hashtable)obj;
								}
							}
							num3++;
							if (!flag)
							{
								break;
							}
							continue;
						}
						flag2 = true;
						break;
					}
					if (flag && flag2)
					{
						obj = hashtable[1];
						if (obj == null)
						{
							flag = false;
						}
					}
					if (flag)
					{
						m_currentInstanceIndex = m_currentSortIndex + 1;
					}
					else
					{
						obj = m_currentInstanceIndex;
					}
				}
			}
			if (obj == null)
			{
				obj = DBNull.Value;
			}
			return obj;
		}

		internal void MatchEventSource(TextBox textBox, TextBoxInstance textBoxInstance, ReportProcessing.IScope containingScope, ReportProcessing.ProcessingContext processingContext)
		{
			bool flag = false;
			if (!(containingScope is ReportProcessing.RuntimePivotCell))
			{
				while (containingScope != null && !(containingScope is ReportProcessing.RuntimeGroupLeafObj) && !(containingScope is ReportProcessing.RuntimeDetailObj) && !(containingScope is ReportProcessing.RuntimeOnePassDetailObj))
				{
					containingScope = containingScope.GetOuterScope(includeSubReportContainingScope: true);
				}
			}
			if (containingScope == null)
			{
				if (m_eventSource.ContainingScopes == null || m_eventSource.ContainingScopes.Count == 0)
				{
					flag = true;
				}
			}
			else if (m_eventSourceScope == containingScope)
			{
				flag = true;
				DataRegion dataRegion = null;
				if (containingScope is ReportProcessing.RuntimeDetailObj)
				{
					dataRegion = ((ReportProcessing.RuntimeDetailObj)containingScope).DataRegionDef;
				}
				else if (containingScope is ReportProcessing.RuntimeOnePassDetailObj)
				{
					dataRegion = ((ReportProcessing.RuntimeOnePassDetailObj)containingScope).DataRegionDef;
				}
				if (dataRegion != null && dataRegion.CurrentDetailRowIndex != m_eventSourceDetailIndex)
				{
					flag = false;
				}
			}
			if (flag)
			{
				if (textBox == m_eventSource)
				{
					m_newUniqueName = textBoxInstance.UniqueName;
					m_page = processingContext.Pagination.GetTextBoxStartPage(textBox);
				}
				else if (m_peerSortFilters != null && m_peerSortFilters.Contains(textBox.ID))
				{
					m_peerSortFilters[textBox.ID] = textBoxInstance.UniqueName;
				}
			}
		}
	}
}

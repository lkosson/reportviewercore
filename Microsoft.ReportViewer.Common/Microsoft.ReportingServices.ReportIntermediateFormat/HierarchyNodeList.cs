using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class HierarchyNodeList : ArrayList
	{
		private List<int> m_leafCellIndexes;

		private int m_excludedIndex = -1;

		private List<int> m_leafCellIndexesWithoutExcluded;

		private int? m_domainScopeCount;

		[NonSerialized]
		private HierarchyNodeList m_staticMembersInSameScope;

		[NonSerialized]
		private HierarchyNodeList m_dynamicMembersAtScope;

		[NonSerialized]
		private bool m_hasStaticLeafMembers;

		internal new ReportHierarchyNode this[int index] => base[index] as ReportHierarchyNode;

		internal List<int> LeafCellIndexes
		{
			get
			{
				if (m_leafCellIndexes == null)
				{
					CalculateLeafCellIndexes(this, ref m_leafCellIndexes, -1);
				}
				if (m_leafCellIndexes.Count != 0)
				{
					return m_leafCellIndexes;
				}
				return null;
			}
		}

		internal int OriginalNodeCount => Count - DomainScopeCount;

		private int DomainScopeCount
		{
			get
			{
				if (!m_domainScopeCount.HasValue)
				{
					m_domainScopeCount = 0;
					{
						IEnumerator enumerator = GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								if (((ReportHierarchyNode)enumerator.Current).IsDomainScope)
								{
									m_domainScopeCount++;
								}
							}
						}
						finally
						{
							IDisposable disposable = enumerator as IDisposable;
							if (disposable != null)
							{
								disposable.Dispose();
							}
						}
					}
				}
				return m_domainScopeCount.Value;
			}
		}

		public HierarchyNodeList StaticMembersInSameScope
		{
			get
			{
				if (m_staticMembersInSameScope == null)
				{
					CalculateDependencies();
				}
				return m_staticMembersInSameScope;
			}
		}

		public bool HasStaticLeafMembers
		{
			get
			{
				if (m_staticMembersInSameScope == null)
				{
					CalculateDependencies();
				}
				return m_hasStaticLeafMembers;
			}
		}

		public HierarchyNodeList DynamicMembersAtScope
		{
			get
			{
				if (m_dynamicMembersAtScope == null)
				{
					CalculateDependencies();
				}
				return m_dynamicMembersAtScope;
			}
		}

		internal HierarchyNodeList()
		{
		}

		internal HierarchyNodeList(int capacity)
			: base(capacity)
		{
		}

		public override int Add(object value)
		{
			if (((ReportHierarchyNode)value).IsDomainScope)
			{
				InitializeDomainScopeCount();
			}
			return base.Add(value);
		}

		private void InitializeDomainScopeCount()
		{
			m_domainScopeCount = null;
		}

		internal List<int> GetLeafCellIndexes(int excludedCellIndex)
		{
			if (excludedCellIndex < 0)
			{
				return LeafCellIndexes;
			}
			if (m_leafCellIndexesWithoutExcluded == null)
			{
				m_excludedIndex = excludedCellIndex;
				CalculateLeafCellIndexes(this, ref m_leafCellIndexesWithoutExcluded, excludedCellIndex);
			}
			else if (m_excludedIndex != excludedCellIndex)
			{
				m_excludedIndex = excludedCellIndex;
				m_leafCellIndexesWithoutExcluded = null;
				CalculateLeafCellIndexes(this, ref m_leafCellIndexesWithoutExcluded, excludedCellIndex);
			}
			if (m_leafCellIndexesWithoutExcluded.Count != 0)
			{
				return m_leafCellIndexesWithoutExcluded;
			}
			return null;
		}

		private static void CalculateLeafCellIndexes(HierarchyNodeList nodes, ref List<int> leafCellIndexes, int excludedCellIndex)
		{
			if (leafCellIndexes != null)
			{
				return;
			}
			int count = nodes.Count;
			leafCellIndexes = new List<int>(count);
			for (int i = 0; i < count; i++)
			{
				ReportHierarchyNode reportHierarchyNode = nodes[i];
				if (reportHierarchyNode.InnerHierarchy == null && reportHierarchyNode.MemberCellIndex != excludedCellIndex)
				{
					leafCellIndexes.Add(reportHierarchyNode.MemberCellIndex);
				}
			}
		}

		internal List<ReportHierarchyNode> GetLeafNodes()
		{
			List<ReportHierarchyNode> list = new List<ReportHierarchyNode>();
			FindLeafNodes(list);
			return list;
		}

		private void FindLeafNodes(List<ReportHierarchyNode> leafNodes)
		{
			for (int i = 0; i < base.Count; i++)
			{
				ReportHierarchyNode reportHierarchyNode = this[i];
				HierarchyNodeList innerHierarchy = reportHierarchyNode.InnerHierarchy;
				if (innerHierarchy == null)
				{
					leafNodes.Add(reportHierarchyNode);
				}
				else
				{
					innerHierarchy.FindLeafNodes(leafNodes);
				}
			}
		}

		internal int GetMemberIndex(ReportHierarchyNode node)
		{
			Global.Tracer.Assert(node.InnerHierarchy == null, "GetMemberIndex should not be called for non leaf node");
			int index = -1;
			GetMemberIndex(ref index, node);
			return index;
		}

		private bool GetMemberIndex(ref int index, ReportHierarchyNode node)
		{
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ReportHierarchyNode reportHierarchyNode = (ReportHierarchyNode)enumerator.Current;
					if (reportHierarchyNode.InnerHierarchy == null)
					{
						index++;
						if (node == reportHierarchyNode)
						{
							return true;
						}
					}
					else if (reportHierarchyNode.InnerHierarchy.GetMemberIndex(ref index, node))
					{
						return true;
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return false;
		}

		private void CalculateDependencies()
		{
			m_staticMembersInSameScope = new HierarchyNodeList();
			m_dynamicMembersAtScope = new HierarchyNodeList();
			m_hasStaticLeafMembers = CalculateDependencies(this, m_staticMembersInSameScope, m_dynamicMembersAtScope);
		}

		private static bool CalculateDependencies(HierarchyNodeList members, HierarchyNodeList staticMembersInSameScope, HierarchyNodeList dynamicMembers)
		{
			if (members == null)
			{
				return false;
			}
			bool flag = false;
			_ = members.Count;
			foreach (ReportHierarchyNode member in members)
			{
				if (!member.IsStatic)
				{
					dynamicMembers.Add(member);
					continue;
				}
				staticMembersInSameScope.Add(member);
				flag = (member.InnerHierarchy == null || (flag | CalculateDependencies(member.InnerHierarchy, staticMembersInSameScope, dynamicMembers)));
			}
			return flag;
		}
	}
}

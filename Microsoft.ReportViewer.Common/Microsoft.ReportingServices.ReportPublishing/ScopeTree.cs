using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal class ScopeTree
	{
		internal delegate void ScopeTreeVisitor(IRIFDataScope scope);

		internal delegate bool DirectedScopeTreeVisitor(IRIFDataScope scope);

		private Dictionary<IRIFDataScope, ScopeTreeNode> m_scopes;

		private Dictionary<string, ScopeTreeNode> m_scopesByName;

		private Dictionary<string, Dictionary<string, ScopeTreeNode>> m_canonicalCellScopes;

		private FunctionalList<ScopeTreeNode> m_dataRegionScopes;

		private FunctionalList<ScopeTreeNode> m_activeScopes;

		private FunctionalList<ScopeTreeNode> m_activeRowScopes;

		private FunctionalList<ScopeTreeNode> m_activeColumnScopes;

		private Microsoft.ReportingServices.ReportIntermediateFormat.Report m_report;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.Report Report => m_report;

		internal ScopeTree()
		{
			m_scopes = new Dictionary<IRIFDataScope, ScopeTreeNode>();
			m_scopesByName = new Dictionary<string, ScopeTreeNode>(StringComparer.Ordinal);
			m_dataRegionScopes = FunctionalList<ScopeTreeNode>.Empty;
			m_activeScopes = FunctionalList<ScopeTreeNode>.Empty;
			m_activeRowScopes = FunctionalList<ScopeTreeNode>.Empty;
			m_activeColumnScopes = FunctionalList<ScopeTreeNode>.Empty;
			m_canonicalCellScopes = new Dictionary<string, Dictionary<string, ScopeTreeNode>>();
		}

		internal ScopeTree(Microsoft.ReportingServices.ReportIntermediateFormat.Report report)
			: this()
		{
			m_report = report;
		}

		internal static bool SameScope(IRIFDataScope scope1, IRIFDataScope scope2)
		{
			return scope1 == scope2;
		}

		internal static bool SameScope(IRIFDataScope scope1, string scope2)
		{
			if (scope1 == null && scope2 == null)
			{
				return true;
			}
			if (scope1 == null || scope2 == null)
			{
				return false;
			}
			return SameScope(scope1.Name, scope2);
		}

		internal static bool SameScope(string scope1, string scope2)
		{
			if (scope1 == null && scope2 == null)
			{
				return true;
			}
			if (scope1 == null || scope2 == null)
			{
				return false;
			}
			return string.CompareOrdinal(scope1, scope2) == 0;
		}

		internal string FindAncestorScopeName(string scopeName, int ancestorLevel)
		{
			if (m_scopesByName.TryGetValue(scopeName, out ScopeTreeNode value))
			{
				SubScopeNode subScopeNode = value as SubScopeNode;
				if (subScopeNode != null)
				{
					SubScopeNode subScopeNode2 = subScopeNode;
					for (int i = 0; i < ancestorLevel; i++)
					{
						if (subScopeNode2.Scope is Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion)
						{
							break;
						}
						subScopeNode = (subScopeNode.ParentScope as SubScopeNode);
						if (subScopeNode == null)
						{
							break;
						}
						subScopeNode2 = subScopeNode;
					}
					return subScopeNode2.ScopeName;
				}
			}
			return null;
		}

		internal int MeasureScopeDistance(string innerScopeName, string outerScopeName)
		{
			if (m_scopesByName.TryGetValue(innerScopeName, out ScopeTreeNode value))
			{
				SubScopeNode subScopeNode = value as SubScopeNode;
				if (subScopeNode != null)
				{
					int num = 0;
					SubScopeNode subScopeNode2 = subScopeNode;
					while (!string.Equals(subScopeNode2.ScopeName, outerScopeName, StringComparison.Ordinal))
					{
						subScopeNode = (subScopeNode.ParentScope as SubScopeNode);
						if (subScopeNode == null)
						{
							return -1;
						}
						subScopeNode2 = subScopeNode;
						num++;
					}
					return num;
				}
			}
			return -1;
		}

		internal bool IsSameOrProperParentScope(IRIFDataScope outerScope, IRIFDataScope innerScope)
		{
			if (m_scopes.TryGetValue(innerScope, out ScopeTreeNode value))
			{
				return value.IsSameOrParentScope(outerScope, isProperParent: true);
			}
			return false;
		}

		internal bool IsSameOrParentScope(IRIFDataScope outerScope, IRIFDataScope innerScope)
		{
			if (m_scopes.TryGetValue(innerScope, out ScopeTreeNode value))
			{
				return value.IsSameOrParentScope(outerScope, isProperParent: false);
			}
			return false;
		}

		internal bool IsParentScope(IRIFDataScope outerScope, IRIFDataScope innerScope)
		{
			if (outerScope == innerScope)
			{
				return false;
			}
			if (m_scopes.TryGetValue(innerScope, out ScopeTreeNode value))
			{
				return value.IsSameOrParentScope(outerScope, isProperParent: false);
			}
			return false;
		}

		internal IEnumerable<IRIFDataScope> GetChildScopes(IRIFDataScope parentScope)
		{
			return GetScopeNodeOrAssert(parentScope).ChildScopes;
		}

		internal bool IsIntersectionScope(IRIFDataScope scope)
		{
			return GetScopeNodeOrAssert(scope) is IntersectScopeNode;
		}

		internal IRIFDataScope GetParentScope(IRIFDataScope scope)
		{
			return GetSubScopeNodeOrAssert(scope).ParentScope?.Scope;
		}

		internal IRIFDataScope GetParentRowScopeForIntersection(IRIFDataScope intersectScope)
		{
			return GetIntersectScopeNodeOrAssert(intersectScope).ParentRowScope.Scope;
		}

		internal IRIFDataScope GetParentColumnScopeForIntersection(IRIFDataScope intersectScope)
		{
			return GetIntersectScopeNodeOrAssert(intersectScope).ParentColumnScope.Scope;
		}

		internal void Traverse(ScopeTreeVisitor visitor, IRIFDataScope outerScope, IRIFDataScope innerScope, bool visitOuterScope)
		{
			GetScopeNodeOrAssert(innerScope).Traverse(visitor, outerScope, visitOuterScope);
		}

		internal bool Traverse(DirectedScopeTreeVisitor visitor, IRIFDataScope startScope)
		{
			return GetScopeNodeOrAssert(startScope).Traverse(visitor);
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion GetParentDataRegion(IRIFDataScope scope)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion parentDataRegion = null;
			DirectedScopeTreeVisitor visitor = delegate(IRIFDataScope candidate)
			{
				if (candidate != scope)
				{
					parentDataRegion = (candidate as Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion);
				}
				return parentDataRegion == null;
			};
			Traverse(visitor, scope);
			return parentDataRegion;
		}

		private SubScopeNode GetSubScopeNodeOrAssert(IRIFDataScope scope)
		{
			SubScopeNode subScopeNode = GetScopeNodeOrAssert(scope) as SubScopeNode;
			Global.Tracer.Assert(subScopeNode != null, "Specified scope was not a SubScope");
			return subScopeNode;
		}

		private IntersectScopeNode GetIntersectScopeNodeOrAssert(IRIFDataScope scope)
		{
			IntersectScopeNode intersectScopeNode = GetScopeNodeOrAssert(scope) as IntersectScopeNode;
			Global.Tracer.Assert(intersectScopeNode != null, "Specified scope was not an IntersectScopeNode ");
			return intersectScopeNode;
		}

		private ScopeTreeNode GetScopeNodeOrAssert(IRIFDataScope scope)
		{
			if (m_scopes.TryGetValue(scope, out ScopeTreeNode value))
			{
				return value;
			}
			Global.Tracer.Assert(false, "Could not find scope in tree: {0}", scope);
			throw new InvalidOperationException();
		}

		internal void RegisterGrouping(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode member)
		{
			if (member.IsColumn)
			{
				AddGroupScope(member, ref m_activeColumnScopes);
			}
			else
			{
				AddGroupScope(member, ref m_activeRowScopes);
			}
		}

		private void AddGroupScope(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode member, ref FunctionalList<ScopeTreeNode> axisScopes)
		{
			if (!m_scopes.TryGetValue(member, out ScopeTreeNode value))
			{
				value = (HasScope(axisScopes) ? new SubScopeNode(member, m_activeScopes.First) : new SubScopeNode(member, m_dataRegionScopes.First));
			}
			AddScope(value);
			axisScopes = axisScopes.Add(value);
		}

		internal void UnRegisterGrouping(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode member)
		{
			if (member.IsColumn)
			{
				RemoveGroupScope(ref m_activeColumnScopes);
			}
			else
			{
				RemoveGroupScope(ref m_activeRowScopes);
			}
		}

		private void RemoveGroupScope(ref FunctionalList<ScopeTreeNode> axisScopes)
		{
			axisScopes = axisScopes.Rest;
			m_activeScopes = m_activeScopes.Rest;
		}

		internal void RegisterDataRegion(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion)
		{
			if (!m_scopes.TryGetValue(dataRegion, out ScopeTreeNode value))
			{
				value = new SubScopeNode(dataRegion, m_activeScopes.First);
			}
			AddScope(value);
			m_dataRegionScopes = m_dataRegionScopes.Add(value);
			m_activeRowScopes = m_activeRowScopes.Add(null);
			m_activeColumnScopes = m_activeColumnScopes.Add(null);
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.DataSet GetDataSet(IRIFDataScope dataScope, string dataSetName)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = null;
			if (dataScope != null && dataScope.DataScopeInfo != null)
			{
				dataSet = dataScope.DataScopeInfo.DataSet;
			}
			if (dataSet != null)
			{
				return dataSet;
			}
			if (dataSetName == null)
			{
				return null;
			}
			return GetDataSet(dataSetName);
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.DataSet GetDataSet(string dataSetName)
		{
			if (string.IsNullOrEmpty(dataSetName))
			{
				return null;
			}
			m_report.MappingNameToDataSet.TryGetValue(dataSetName, out Microsoft.ReportingServices.ReportIntermediateFormat.DataSet value);
			return value;
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.DataSet GetDefaultTopLevelDataSet()
		{
			if (m_report.OneDataSetName != null)
			{
				return GetDataSet(m_report.OneDataSetName);
			}
			return null;
		}

		internal void UnRegisterDataRegion(Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion)
		{
			m_activeScopes = m_activeScopes.Rest;
			m_activeRowScopes = m_activeRowScopes.Rest;
			m_activeColumnScopes = m_activeColumnScopes.Rest;
			m_dataRegionScopes = m_dataRegionScopes.Rest;
		}

		internal IRIFDataScope RegisterCell(IRIFDataScope cell)
		{
			if (!m_scopes.TryGetValue(cell, out ScopeTreeNode value))
			{
				if (HasScope(m_activeRowScopes) && HasScope(m_activeColumnScopes))
				{
					ScopeTreeNode first = m_activeRowScopes.First;
					ScopeTreeNode first2 = m_activeColumnScopes.First;
					if (!TryGetCanonicalCellScope(first, first2, out value))
					{
						value = new IntersectScopeNode(cell, first, first2);
						AddCanonicalCellScope(first, first2, value);
					}
					((IntersectScopeNode)value).AddCell(cell);
				}
				else
				{
					value = new SubScopeNode(cell, m_activeScopes.First);
				}
			}
			AddScope(value, cell);
			m_activeRowScopes = m_activeRowScopes.Add(null);
			m_activeColumnScopes = m_activeColumnScopes.Add(null);
			return value.Scope;
		}

		internal void UnRegisterCell(IRIFDataScope cell)
		{
			m_activeScopes = m_activeScopes.Rest;
			m_activeRowScopes = m_activeRowScopes.Rest;
			m_activeColumnScopes = m_activeColumnScopes.Rest;
		}

		internal IRIFDataScope GetCanonicalCellScope(IRIFDataScope cell)
		{
			if (!m_scopes.TryGetValue(cell, out ScopeTreeNode value))
			{
				Global.Tracer.Assert(condition: false, "GetCanonicalCellScope must not be called for a cell outside the ScopeTree.");
			}
			return value.Scope;
		}

		private bool TryGetCanonicalCellScope(ScopeTreeNode rowScope, ScopeTreeNode colScope, out ScopeTreeNode canonicalCellScope)
		{
			if (m_canonicalCellScopes.TryGetValue(rowScope.Scope.Name, out Dictionary<string, ScopeTreeNode> value) && value.TryGetValue(colScope.Scope.Name, out canonicalCellScope))
			{
				return true;
			}
			canonicalCellScope = null;
			return false;
		}

		private void AddCanonicalCellScope(ScopeTreeNode rowScope, ScopeTreeNode colScope, ScopeTreeNode cellScope)
		{
			if (!m_canonicalCellScopes.TryGetValue(rowScope.Scope.Name, out Dictionary<string, ScopeTreeNode> value))
			{
				value = new Dictionary<string, ScopeTreeNode>();
				m_canonicalCellScopes.Add(rowScope.Scope.Name, value);
			}
			value[colScope.Scope.Name] = cellScope;
		}

		private bool HasScope(FunctionalList<ScopeTreeNode> list)
		{
			if (!list.IsEmpty())
			{
				return list.First != null;
			}
			return false;
		}

		private void AddScope(ScopeTreeNode scopeNode, IRIFDataScope scope)
		{
			m_activeScopes = m_activeScopes.Add(scopeNode);
			m_scopes[scope] = scopeNode;
			if (!string.IsNullOrEmpty(scopeNode.ScopeName))
			{
				m_scopesByName[scopeNode.ScopeName] = scopeNode;
			}
		}

		private void AddScope(ScopeTreeNode scopeNode)
		{
			AddScope(scopeNode, scopeNode.Scope);
		}

		internal string GetScopeName(IRIFDataScope scope)
		{
			string text = null;
			if (m_scopes.TryGetValue(scope, out ScopeTreeNode value))
			{
				return value.ScopeName;
			}
			return scope.Name;
		}

		internal IRIFDataScope GetScopeByName(string scopeName)
		{
			if (m_scopesByName.TryGetValue(scopeName, out ScopeTreeNode value))
			{
				return value.Scope;
			}
			return null;
		}
	}
}

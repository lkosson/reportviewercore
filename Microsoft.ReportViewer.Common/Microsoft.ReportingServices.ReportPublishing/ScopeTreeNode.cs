using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal abstract class ScopeTreeNode
	{
		protected readonly IRIFDataScope m_scope;

		private readonly List<IRIFDataScope> m_childScopes = new List<IRIFDataScope>();

		internal IRIFDataScope Scope => m_scope;

		internal List<IRIFDataScope> ChildScopes => m_childScopes;

		internal abstract string ScopeName
		{
			get;
		}

		internal ScopeTreeNode(IRIFDataScope scope)
		{
			m_scope = scope;
		}

		internal void AddChildScope(IRIFDataScope child)
		{
			m_childScopes.Add(child);
		}

		internal abstract bool IsSameOrParentScope(IRIFDataScope parentScope, bool isProperParent);

		internal abstract void Traverse(ScopeTree.ScopeTreeVisitor visitor, IRIFDataScope outerScope, bool visitOuterScope);

		internal abstract bool Traverse(ScopeTree.DirectedScopeTreeVisitor visitor);

		protected static bool TraverseNode(ScopeTree.DirectedScopeTreeVisitor visitor, ScopeTreeNode node)
		{
			return node?.Traverse(visitor) ?? true;
		}
	}
}

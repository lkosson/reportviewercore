using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal class SubScopeNode : ScopeTreeNode
	{
		private readonly ScopeTreeNode m_parentScope;

		internal ScopeTreeNode ParentScope => m_parentScope;

		internal override string ScopeName => m_scope.Name;

		internal SubScopeNode(IRIFDataScope scope, ScopeTreeNode parentScope)
			: base(scope)
		{
			m_parentScope = parentScope;
			if (m_parentScope != null)
			{
				m_parentScope.AddChildScope(scope);
			}
		}

		internal override bool IsSameOrParentScope(IRIFDataScope parentScope, bool isProperParent)
		{
			if (parentScope == base.Scope)
			{
				return true;
			}
			if (m_parentScope == null)
			{
				return false;
			}
			return m_parentScope.IsSameOrParentScope(parentScope, isProperParent);
		}

		internal override void Traverse(ScopeTree.ScopeTreeVisitor visitor, IRIFDataScope outerScope, bool visitOuterScope)
		{
			bool flag = outerScope == base.Scope;
			if (visitOuterScope || !flag)
			{
				visitor(base.Scope);
			}
			if (!flag && m_parentScope != null)
			{
				m_parentScope.Traverse(visitor, outerScope, visitOuterScope);
			}
		}

		internal override bool Traverse(ScopeTree.DirectedScopeTreeVisitor visitor)
		{
			if (visitor(base.Scope))
			{
				return ScopeTreeNode.TraverseNode(visitor, m_parentScope);
			}
			return false;
		}
	}
}

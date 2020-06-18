using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal class IntersectScopeNode : ScopeTreeNode
	{
		private readonly ScopeTreeNode m_parentRowScope;

		private readonly ScopeTreeNode m_parentColumnScope;

		private readonly List<IRIFDataScope> m_peerDataCells;

		internal ScopeTreeNode ParentRowScope => m_parentRowScope;

		internal ScopeTreeNode ParentColumnScope => m_parentColumnScope;

		internal override string ScopeName
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder("(");
				stringBuilder.Append(m_parentRowScope.ScopeName);
				stringBuilder.Append(".");
				stringBuilder.Append(m_parentColumnScope.ScopeName);
				stringBuilder.Append(")");
				return stringBuilder.ToString();
			}
		}

		internal IntersectScopeNode(IRIFDataScope scope, ScopeTreeNode parentRowScope, ScopeTreeNode parentColScope)
			: base(scope)
		{
			m_parentRowScope = parentRowScope;
			if (m_parentRowScope != null)
			{
				m_parentRowScope.AddChildScope(scope);
			}
			m_parentColumnScope = parentColScope;
			if (m_parentColumnScope != null)
			{
				m_parentColumnScope.AddChildScope(scope);
			}
			m_peerDataCells = new List<IRIFDataScope>();
		}

		internal override bool IsSameOrParentScope(IRIFDataScope parentScope, bool isProperParent)
		{
			if (HasCell(parentScope))
			{
				return true;
			}
			if (m_parentRowScope == null || m_parentColumnScope == null)
			{
				return false;
			}
			bool flag = m_parentRowScope.IsSameOrParentScope(parentScope, isProperParent);
			bool flag2 = m_parentColumnScope.IsSameOrParentScope(parentScope, isProperParent);
			if (!isProperParent)
			{
				return flag || flag2;
			}
			return flag && flag2;
		}

		internal override void Traverse(ScopeTree.ScopeTreeVisitor visitor, IRIFDataScope outerScope, bool visitOuterScope)
		{
			bool flag = HasCell(outerScope);
			if (visitOuterScope || !flag)
			{
				TraverseDefinitionCells(visitor);
			}
			if (!flag)
			{
				if (m_parentRowScope != null)
				{
					m_parentRowScope.Traverse(visitor, outerScope, visitOuterScope);
				}
				if (m_parentColumnScope != null)
				{
					m_parentColumnScope.Traverse(visitor, outerScope, visitOuterScope);
				}
			}
		}

		internal override bool Traverse(ScopeTree.DirectedScopeTreeVisitor visitor)
		{
			if (TraverseDefinitionCells(visitor) && ScopeTreeNode.TraverseNode(visitor, m_parentRowScope))
			{
				return ScopeTreeNode.TraverseNode(visitor, m_parentColumnScope);
			}
			return false;
		}

		private void TraverseDefinitionCells(ScopeTree.ScopeTreeVisitor visitor)
		{
			visitor(base.Scope);
			foreach (IRIFDataScope peerDataCell in m_peerDataCells)
			{
				visitor(peerDataCell);
			}
		}

		private bool TraverseDefinitionCells(ScopeTree.DirectedScopeTreeVisitor visitor)
		{
			if (!visitor(base.Scope))
			{
				return false;
			}
			foreach (IRIFDataScope peerDataCell in m_peerDataCells)
			{
				if (!visitor(peerDataCell))
				{
					return false;
				}
			}
			return true;
		}

		internal void AddCell(IRIFDataScope cell)
		{
			if (!HasCell(cell))
			{
				m_peerDataCells.Add(cell);
			}
		}

		internal bool HasCell(IRIFDataScope cell)
		{
			if (cell == null)
			{
				return false;
			}
			if (cell == base.Scope)
			{
				return true;
			}
			foreach (IRIFDataScope peerDataCell in m_peerDataCells)
			{
				if (ScopeTree.SameScope(cell, peerDataCell))
				{
					return true;
				}
			}
			return false;
		}
	}
}

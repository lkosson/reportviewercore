using Microsoft.ReportingServices.ReportPublishing;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal class ScopeTreeBuilder : IRIFScopeVisitor
	{
		protected ScopeTree m_tree;

		internal ScopeTree Tree => m_tree;

		protected ScopeTreeBuilder(Report report)
		{
			m_tree = new ScopeTree(report);
		}

		public static ScopeTree BuildScopeTree(Report report)
		{
			ScopeTreeBuilder scopeTreeBuilder = new ScopeTreeBuilder(report);
			report.TraverseScopes(scopeTreeBuilder);
			return scopeTreeBuilder.Tree;
		}

		public virtual void PreVisit(DataRegion dataRegion)
		{
			m_tree.RegisterDataRegion(dataRegion);
		}

		public virtual void PostVisit(DataRegion dataRegion)
		{
			m_tree.UnRegisterDataRegion(dataRegion);
		}

		public virtual void PreVisit(ReportHierarchyNode member)
		{
			m_tree.RegisterGrouping(member);
		}

		public virtual void PostVisit(ReportHierarchyNode member)
		{
			m_tree.UnRegisterGrouping(member);
		}

		public virtual void PreVisit(Cell cell, int rowIndex, int colIndex)
		{
			m_tree.RegisterCell(cell);
		}

		public virtual void PostVisit(Cell cell, int rowIndex, int colIndex)
		{
			m_tree.UnRegisterCell(cell);
		}
	}
}

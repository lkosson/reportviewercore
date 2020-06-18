using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class ScopeTreeBuilderForDataScopeDataSet : ScopeTreeBuilder
	{
		private readonly ErrorContext m_errorContext;

		private int m_nextDataPipelineId;

		public static ScopeTree BuildScopeTree(Report report, ErrorContext errorContext)
		{
			ScopeTreeBuilderForDataScopeDataSet scopeTreeBuilderForDataScopeDataSet = new ScopeTreeBuilderForDataScopeDataSet(report, errorContext);
			report.TraverseScopes(scopeTreeBuilderForDataScopeDataSet);
			report.DataPipelineCount = scopeTreeBuilderForDataScopeDataSet.m_nextDataPipelineId;
			return scopeTreeBuilderForDataScopeDataSet.Tree;
		}

		private ScopeTreeBuilderForDataScopeDataSet(Report report, ErrorContext errorContext)
			: base(report)
		{
			m_errorContext = errorContext;
			report.BindAndValidateDataSetDefaultRelationships(m_errorContext);
			m_nextDataPipelineId = report.DataSetCount;
		}

		public override void PreVisit(DataRegion dataRegion)
		{
			base.PreVisit(dataRegion);
			SetDataSetForScope(dataRegion);
		}

		private void SetDataSetForScope(IRIFReportDataScope scope)
		{
			if (scope != null && scope.DataScopeInfo != null)
			{
				scope.DataScopeInfo.ValidateDataSetBindingAndRelationships(m_tree, scope, m_errorContext);
				DetermineDataPipelineID(scope);
			}
		}

		private void DetermineDataPipelineID(IRIFReportDataScope scope)
		{
			if (scope.DataScopeInfo.DataSet == null)
			{
				return;
			}
			DataSet dataSet = scope.DataScopeInfo.DataSet;
			int dataPipelineID;
			if (!scope.DataScopeInfo.NeedsIDC)
			{
				dataPipelineID = (((!m_tree.IsIntersectionScope(scope)) ? m_tree.GetParentScope(scope) : m_tree.GetParentRowScopeForIntersection(scope))?.DataScopeInfo.DataPipelineID ?? dataSet.IndexInCollection);
			}
			else if (m_tree.IsIntersectionScope(scope))
			{
				if (DataSet.AreEqualById(dataSet, m_tree.GetParentRowScopeForIntersection(scope).DataScopeInfo.DataSet) || DataSet.AreEqualById(dataSet, m_tree.GetParentColumnScopeForIntersection(scope).DataScopeInfo.DataSet))
				{
					IRIFDataScope canonicalCellScope = m_tree.GetCanonicalCellScope(scope);
					if (ScopeTree.SameScope(scope, canonicalCellScope))
					{
						dataPipelineID = m_nextDataPipelineId;
						m_nextDataPipelineId++;
					}
					else
					{
						dataPipelineID = canonicalCellScope.DataScopeInfo.DataPipelineID;
					}
				}
				else
				{
					dataPipelineID = dataSet.IndexInCollection;
				}
			}
			else
			{
				dataPipelineID = dataSet.IndexInCollection;
			}
			scope.DataScopeInfo.DataPipelineID = dataPipelineID;
		}

		public override void PreVisit(ReportHierarchyNode member)
		{
			base.PreVisit(member);
			SetDataSetForScope(member);
		}

		public override void PreVisit(Cell cell, int rowIndex, int colIndex)
		{
			base.PreVisit(cell, rowIndex, colIndex);
			SetDataSetForScope(cell);
		}
	}
}

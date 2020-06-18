namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MatrixMemberInfoCache
	{
		private int m_startIndex;

		private int[] m_cellIndexes;

		private MatrixMemberInfoCache[] m_children;

		internal bool IsOptimizedNode => m_startIndex >= 0;

		internal MatrixMemberInfoCache[] Children => m_children;

		internal MatrixMemberInfoCache(int startIndex, int length)
		{
			m_startIndex = startIndex;
			if (!IsOptimizedNode)
			{
				m_cellIndexes = new int[length];
				m_children = new MatrixMemberInfoCache[length];
				for (int i = 0; i < length; i++)
				{
					m_cellIndexes[i] = -1;
				}
			}
		}

		internal int GetCellIndex(ShimMatrixMember member)
		{
			if (IsOptimizedNode)
			{
				return m_startIndex + member.AdjustedRenderCollectionIndex;
			}
			int adjustedRenderCollectionIndex = member.AdjustedRenderCollectionIndex;
			if (m_cellIndexes[adjustedRenderCollectionIndex] < 0)
			{
				m_cellIndexes[adjustedRenderCollectionIndex] = member.CurrentRenderMatrixMember.CachedMemberCellIndex;
			}
			return m_cellIndexes[adjustedRenderCollectionIndex];
		}
	}
}

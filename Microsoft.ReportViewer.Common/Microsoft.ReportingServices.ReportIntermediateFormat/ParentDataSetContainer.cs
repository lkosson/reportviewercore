namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class ParentDataSetContainer
	{
		private readonly DataSet m_rowParentDataSet;

		private readonly DataSet m_columnParentDataSet;

		private readonly int m_count;

		public int Count => m_count;

		public DataSet ParentDataSet => m_rowParentDataSet;

		public DataSet RowParentDataSet => m_rowParentDataSet;

		public DataSet ColumnParentDataSet => m_columnParentDataSet;

		public ParentDataSetContainer(DataSet parentDataSet)
		{
			m_rowParentDataSet = parentDataSet;
			m_columnParentDataSet = null;
			m_count = 1;
		}

		public ParentDataSetContainer(DataSet rowParentDataSet, DataSet columnParentDataSet)
		{
			m_rowParentDataSet = rowParentDataSet;
			m_columnParentDataSet = columnParentDataSet;
			m_count = 2;
		}

		public bool AreAllSameDataSet()
		{
			if (Count == 1)
			{
				return true;
			}
			return DataSet.AreEqualById(m_rowParentDataSet, m_columnParentDataSet);
		}
	}
}

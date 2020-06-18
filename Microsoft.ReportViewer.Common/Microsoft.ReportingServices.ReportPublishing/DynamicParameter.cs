namespace Microsoft.ReportingServices.ReportPublishing
{
	internal sealed class DynamicParameter
	{
		private DataSetReference m_validValueDataSet;

		private DataSetReference m_defaultDataSet;

		private int m_index;

		private bool m_isComplex;

		internal DataSetReference ValidValueDataSet => m_validValueDataSet;

		internal DataSetReference DefaultDataSet => m_defaultDataSet;

		internal int Index => m_index;

		internal bool IsComplex => m_isComplex;

		internal DynamicParameter(DataSetReference validValueDataSet, DataSetReference defaultDataSet, int index, bool isComplex)
		{
			m_validValueDataSet = validValueDataSet;
			m_defaultDataSet = defaultDataSet;
			m_index = index;
			m_isComplex = isComplex;
		}
	}
}

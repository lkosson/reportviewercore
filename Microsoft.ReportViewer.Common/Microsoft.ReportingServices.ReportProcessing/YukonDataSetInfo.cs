namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class YukonDataSetInfo
	{
		private StringList m_parameterNames;

		private bool m_isComplex;

		private int m_dataSetDefIndex = -1;

		private int m_dataSourceIndex = -1;

		private int m_dataSetIndex = -1;

		private int m_calculatedFieldIndex;

		internal int DataSourceIndex
		{
			get
			{
				return m_dataSourceIndex;
			}
			set
			{
				m_dataSourceIndex = value;
			}
		}

		internal int DataSetIndex
		{
			get
			{
				return m_dataSetIndex;
			}
			set
			{
				m_dataSetIndex = value;
			}
		}

		internal int DataSetDefIndex => m_dataSetDefIndex;

		internal bool IsComplex => m_isComplex;

		internal StringList ParameterNames => m_parameterNames;

		internal int CalculatedFieldIndex
		{
			get
			{
				return m_calculatedFieldIndex;
			}
			set
			{
				m_calculatedFieldIndex = value;
			}
		}

		internal YukonDataSetInfo(int index, bool isComplex, StringList parameterNames)
		{
			m_dataSetDefIndex = index;
			m_isComplex = isComplex;
			m_parameterNames = parameterNames;
		}

		internal void MergeFlagsFromDataSource(bool isComplex, StringList datasourceParameterNames)
		{
			m_isComplex |= isComplex;
			if (m_parameterNames == null)
			{
				m_parameterNames = datasourceParameterNames;
			}
			else if (datasourceParameterNames != null)
			{
				m_parameterNames.InsertRange(0, datasourceParameterNames);
			}
		}
	}
}

using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal sealed class PublishingDataSetInfo
	{
		private string m_dataSetName;

		private Dictionary<string, bool> m_parameterNames;

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

		internal string DataSetName => m_dataSetName;

		internal int DataSetDefIndex => m_dataSetDefIndex;

		internal bool IsComplex => m_isComplex;

		internal Dictionary<string, bool> ParameterNames => m_parameterNames;

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

		internal PublishingDataSetInfo(string dataSetName, int dataSetDefIndex, bool isComplex, Dictionary<string, bool> parameterNames)
		{
			m_dataSetName = dataSetName;
			m_dataSetDefIndex = dataSetDefIndex;
			m_isComplex = isComplex;
			m_parameterNames = parameterNames;
		}

		internal void MergeFlagsFromDataSource(bool isComplex, Dictionary<string, bool> datasourceParameterNames)
		{
			m_isComplex |= isComplex;
			if (datasourceParameterNames == null)
			{
				return;
			}
			foreach (string key in datasourceParameterNames.Keys)
			{
				m_parameterNames[key] = true;
			}
		}
	}
}

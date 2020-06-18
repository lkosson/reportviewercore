using Microsoft.ReportingServices.DataProcessing;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Reporting
{
	internal class DataSetProcessingParameters : IDataParameterCollection, IEnumerable
	{
		private List<IDataParameter> m_list;

		public DataSetProcessingParameters()
		{
			m_list = new List<IDataParameter>();
		}

		public int Add(IDataParameter parameter)
		{
			m_list.Add(parameter);
			return m_list.Count;
		}

		public IEnumerator GetEnumerator()
		{
			return m_list.GetEnumerator();
		}
	}
}

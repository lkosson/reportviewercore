using Microsoft.ReportingServices.DataProcessing;
using System.Data;

namespace Microsoft.ReportingServices.DataExtensions
{
	internal class ParameterMultiValueWrapper : ParameterWrapper, IDataMultiValueParameter, Microsoft.ReportingServices.DataProcessing.IDataParameter
	{
		private object[] m_values;

		public virtual object[] Values
		{
			get
			{
				return m_values;
			}
			set
			{
				m_values = value;
			}
		}

		public ParameterMultiValueWrapper(System.Data.IDataParameter param)
			: base(param)
		{
		}
	}
}

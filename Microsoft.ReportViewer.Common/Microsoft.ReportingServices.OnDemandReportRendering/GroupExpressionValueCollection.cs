using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class GroupExpressionValueCollection
	{
		private object[] m_values;

		public object this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				return m_values[index];
			}
		}

		public int Count
		{
			get
			{
				if (m_values != null)
				{
					return m_values.Length;
				}
				return 0;
			}
		}

		internal GroupExpressionValueCollection()
		{
		}

		internal void UpdateValues(object exprValue)
		{
			m_values = new object[1];
			m_values[0] = exprValue;
		}

		internal void UpdateValues(object[] exprValues)
		{
			m_values = exprValues;
		}
	}
}

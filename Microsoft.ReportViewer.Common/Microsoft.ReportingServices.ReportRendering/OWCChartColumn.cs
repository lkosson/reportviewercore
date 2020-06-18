using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class OWCChartColumn
	{
		private ChartColumn m_chartColumnDef;

		private ArrayList m_data;

		public string Name => m_chartColumnDef.Name;

		public object this[int index]
		{
			get
			{
				if (0 <= index && index < m_data.Count)
				{
					if (m_chartColumnDef.Value.Type == ExpressionInfo.Types.Constant)
					{
						return m_chartColumnDef.Value.Value;
					}
					if (m_data != null)
					{
						return m_data[index];
					}
					return null;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
			}
		}

		public int Count
		{
			get
			{
				if (m_data != null)
				{
					return m_data.Count;
				}
				return 0;
			}
		}

		internal OWCChartColumn(ChartColumn chartColumnDef, ArrayList columnData)
		{
			m_chartColumnDef = chartColumnDef;
			m_data = columnData;
		}
	}
}

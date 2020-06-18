using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ChartSeriesDataPoints
	{
		private int m_count;

		private ChartDataPoint[] m_seriesCells;

		internal ChartDataPoint this[int index]
		{
			get
			{
				if (index < 0 || index >= m_count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, m_count);
				}
				if (m_seriesCells != null)
				{
					return m_seriesCells[index];
				}
				return null;
			}
			set
			{
				if (index < 0 || index >= m_count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, m_count);
				}
				if (m_seriesCells == null)
				{
					m_seriesCells = new ChartDataPoint[m_count];
				}
				m_seriesCells[index] = value;
			}
		}

		internal ChartSeriesDataPoints(int count)
		{
			m_count = count;
		}
	}
}

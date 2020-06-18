using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ChartDataPointCollection
	{
		private Chart m_owner;

		private int m_categoryCount;

		private int m_seriesCount;

		private ChartDataPoint m_firstDataPoint;

		private ChartSeriesDataPoints m_firstCategoryDataPoints;

		private ChartSeriesDataPoints m_firstSeriesDataPoints;

		private ChartSeriesDataPoints[] m_dataPoints;

		public ChartDataPoint this[int series, int category]
		{
			get
			{
				if (series < 0 || series >= m_seriesCount)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, series, 0, m_seriesCount);
				}
				if (category < 0 || category >= m_categoryCount)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, category, 0, m_categoryCount);
				}
				ChartDataPoint chartDataPoint = null;
				if (series == 0 && category == 0)
				{
					chartDataPoint = m_firstDataPoint;
				}
				else if (series == 0)
				{
					if (m_firstSeriesDataPoints != null)
					{
						chartDataPoint = m_firstSeriesDataPoints[category - 1];
					}
				}
				else if (category == 0)
				{
					if (m_firstCategoryDataPoints != null)
					{
						chartDataPoint = m_firstCategoryDataPoints[series - 1];
					}
				}
				else if (m_dataPoints != null && m_dataPoints[series - 1] != null)
				{
					chartDataPoint = m_dataPoints[series - 1][category - 1];
				}
				if (chartDataPoint == null)
				{
					chartDataPoint = new ChartDataPoint(m_owner, series, category);
					if (m_owner.RenderingContext.CacheState)
					{
						if (series == 0 && category == 0)
						{
							m_firstDataPoint = chartDataPoint;
						}
						else if (series == 0)
						{
							if (m_firstSeriesDataPoints == null)
							{
								m_firstSeriesDataPoints = new ChartSeriesDataPoints(m_categoryCount - 1);
							}
							m_firstSeriesDataPoints[category - 1] = chartDataPoint;
						}
						else if (category == 0)
						{
							if (m_firstCategoryDataPoints == null)
							{
								m_firstCategoryDataPoints = new ChartSeriesDataPoints(m_seriesCount - 1);
							}
							m_firstCategoryDataPoints[series - 1] = chartDataPoint;
						}
						else
						{
							if (m_dataPoints == null)
							{
								m_dataPoints = new ChartSeriesDataPoints[m_seriesCount - 1];
							}
							if (m_dataPoints[series - 1] == null)
							{
								m_dataPoints[series - 1] = new ChartSeriesDataPoints(m_categoryCount - 1);
							}
							m_dataPoints[series - 1][category - 1] = chartDataPoint;
						}
					}
				}
				return chartDataPoint;
			}
		}

		public int Count => m_seriesCount * m_categoryCount;

		public int SeriesCount => m_seriesCount;

		public int CategoryCount => m_categoryCount;

		internal ChartDataPointCollection(Chart owner, int seriesCount, int categoryCount)
		{
			m_owner = owner;
			m_seriesCount = seriesCount;
			m_categoryCount = categoryCount;
		}
	}
}

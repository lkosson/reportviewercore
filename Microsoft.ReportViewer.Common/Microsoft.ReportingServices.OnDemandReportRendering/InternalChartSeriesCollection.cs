using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalChartSeriesCollection : ChartSeriesCollection
	{
		private ChartSeriesList m_seriesDefs;

		public override ChartSeries this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				if (m_chartSeriesCollection == null)
				{
					m_chartSeriesCollection = new ChartSeries[Count];
				}
				if (m_chartSeriesCollection[index] == null)
				{
					m_chartSeriesCollection[index] = new InternalChartSeries(m_owner, index, m_seriesDefs[index]);
				}
				return m_chartSeriesCollection[index];
			}
		}

		public override int Count => m_seriesDefs.Count;

		internal InternalChartSeriesCollection(Chart owner, ChartSeriesList seriesDefs)
			: base(owner)
		{
			m_seriesDefs = seriesDefs;
		}

		internal InternalChartSeries GetByName(string seriesName)
		{
			for (int i = 0; i < Count; i++)
			{
				InternalChartSeries internalChartSeries = (InternalChartSeries)this[i];
				if (Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(seriesName, internalChartSeries.Name, ignoreCase: false) == 0)
				{
					return internalChartSeries;
				}
			}
			return null;
		}
	}
}

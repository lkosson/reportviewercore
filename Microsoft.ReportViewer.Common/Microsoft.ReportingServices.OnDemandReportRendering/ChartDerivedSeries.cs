using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartDerivedSeries : ChartObjectCollectionItem<BaseInstance>
	{
		private Chart m_chart;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries m_chartDerivedSeriesDef;

		private ChartSeries m_series;

		private ChartFormulaParameterCollection m_chartFormulaParameters;

		private InternalChartSeries m_sourceSeries;

		public ChartSeries Series
		{
			get
			{
				if (m_series == null && !m_chart.IsOldSnapshot && m_chartDerivedSeriesDef.Series != null)
				{
					m_series = new InternalChartSeries(this);
				}
				return m_series;
			}
		}

		public ChartFormulaParameterCollection FormulaParameters
		{
			get
			{
				if (m_chartFormulaParameters == null && !m_chart.IsOldSnapshot && ChartDerivedSeriesDef.FormulaParameters != null)
				{
					m_chartFormulaParameters = new ChartFormulaParameterCollection(this, m_chart);
				}
				return m_chartFormulaParameters;
			}
		}

		public string SourceChartSeriesName
		{
			get
			{
				if (m_chart.IsOldSnapshot)
				{
					return null;
				}
				return m_chartDerivedSeriesDef.SourceChartSeriesName;
			}
		}

		internal InternalChartSeries SourceSeries
		{
			get
			{
				if (m_sourceSeries == null)
				{
					m_sourceSeries = ((InternalChartSeriesCollection)m_chart.ChartData.SeriesCollection).GetByName(SourceChartSeriesName);
				}
				return m_sourceSeries;
			}
		}

		public ChartSeriesFormula DerivedSeriesFormula => m_chartDerivedSeriesDef.DerivedSeriesFormula;

		internal IReportScope ReportScope => SourceSeries.ReportScope;

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries ChartDerivedSeriesDef => m_chartDerivedSeriesDef;

		internal ChartDerivedSeries(Microsoft.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries chartDerivedSeriesDef, Chart chart)
		{
			m_chartDerivedSeriesDef = chartDerivedSeriesDef;
			m_chart = chart;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_series != null)
			{
				m_series.SetNewContext();
			}
			if (m_chartFormulaParameters != null)
			{
				m_chartFormulaParameters.SetNewContext();
			}
		}
	}
}

using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartFormulaParameterCollection : ChartObjectCollectionBase<ChartFormulaParameter, ChartFormulaParameterInstance>
	{
		private Chart m_chart;

		private ChartDerivedSeries m_derivedSeries;

		public ChartFormulaParameter this[string name]
		{
			get
			{
				if (!m_chart.IsOldSnapshot)
				{
					for (int i = 0; i < Count; i++)
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter chartFormulaParameter = m_derivedSeries.ChartDerivedSeriesDef.FormulaParameters[i];
						if (string.CompareOrdinal(name, chartFormulaParameter.FormulaParameterName) == 0)
						{
							return base[i];
						}
					}
				}
				return null;
			}
		}

		public override int Count
		{
			get
			{
				if (m_chart.IsOldSnapshot)
				{
					return 0;
				}
				return m_derivedSeries.ChartDerivedSeriesDef.FormulaParameters.Count;
			}
		}

		internal ChartFormulaParameterCollection(ChartDerivedSeries derivedSeries, Chart chart)
		{
			m_derivedSeries = derivedSeries;
			m_chart = chart;
		}

		protected override ChartFormulaParameter CreateChartObject(int index)
		{
			if (m_chart.IsOldSnapshot)
			{
				return null;
			}
			return new ChartFormulaParameter(m_derivedSeries, m_derivedSeries.ChartDerivedSeriesDef.FormulaParameters[index], m_chart);
		}
	}
}

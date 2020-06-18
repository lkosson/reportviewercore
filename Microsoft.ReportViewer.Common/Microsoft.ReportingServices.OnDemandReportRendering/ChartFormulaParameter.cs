using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartFormulaParameter : ChartObjectCollectionItem<ChartFormulaParameterInstance>
	{
		private Chart m_chart;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter m_chartFormulaParameterDef;

		private ReportVariantProperty m_value;

		private ChartDerivedSeries m_chartDerivedSeries;

		public string Name
		{
			get
			{
				if (m_chart.IsOldSnapshot)
				{
					return null;
				}
				return m_chartFormulaParameterDef.FormulaParameterName;
			}
		}

		public ReportVariantProperty Value
		{
			get
			{
				if (m_value == null && !m_chart.IsOldSnapshot && m_chartFormulaParameterDef.Value != null)
				{
					m_value = new ReportVariantProperty(m_chartFormulaParameterDef.Value);
				}
				return m_value;
			}
		}

		public string Source
		{
			get
			{
				if (m_chart.IsOldSnapshot)
				{
					return null;
				}
				if (m_chartFormulaParameterDef.Source != null)
				{
					return m_chartFormulaParameterDef.Source;
				}
				return null;
			}
		}

		internal IReportScope ReportScope => m_chartDerivedSeries.ReportScope;

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter ChartFormulaParameterDef => m_chartFormulaParameterDef;

		public ChartFormulaParameterInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartFormulaParameterInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartFormulaParameter(ChartDerivedSeries chartDerivedSeries, Microsoft.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter chartFormulaParameterDef, Chart chart)
		{
			m_chartDerivedSeries = chartDerivedSeries;
			m_chartFormulaParameterDef = chartFormulaParameterDef;
			m_chart = chart;
		}
	}
}

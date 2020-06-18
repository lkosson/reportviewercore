using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartCustomPaletteColor : ChartObjectCollectionItem<ChartCustomPaletteColorInstance>
	{
		private Chart m_chart;

		private ReportColorProperty m_color;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartCustomPaletteColor m_chartCustomPaletteColorDef;

		public ReportColorProperty Color
		{
			get
			{
				if (m_color == null && !m_chart.IsOldSnapshot && m_chartCustomPaletteColorDef.Color != null)
				{
					ExpressionInfo color = m_chartCustomPaletteColorDef.Color;
					if (color != null)
					{
						m_color = new ReportColorProperty(color.IsExpression, color.OriginalText, color.IsExpression ? null : new ReportColor(color.StringValue.Trim(), allowTransparency: true), color.IsExpression ? new ReportColor("", System.Drawing.Color.Empty, parsed: true) : null);
					}
				}
				return m_color;
			}
		}

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartCustomPaletteColor ChartCustomPaletteColorDef => m_chartCustomPaletteColorDef;

		public ChartCustomPaletteColorInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartCustomPaletteColorInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartCustomPaletteColor(Microsoft.ReportingServices.ReportIntermediateFormat.ChartCustomPaletteColor chartCustomPaletteColorDef, Chart chart)
		{
			m_chart = chart;
			m_chartCustomPaletteColorDef = chartCustomPaletteColorDef;
		}
	}
}

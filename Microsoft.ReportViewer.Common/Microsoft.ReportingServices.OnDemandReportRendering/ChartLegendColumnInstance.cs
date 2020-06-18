namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendColumnInstance : BaseInstance
	{
		private ChartLegendColumn m_chartLegendColumnDef;

		private StyleInstance m_style;

		private ChartColumnType? m_columnType;

		private string m_value;

		private string m_toolTip;

		private ReportSize m_minimumWidth;

		private ReportSize m_maximumWidth;

		private int? m_seriesSymbolWidth;

		private int? m_seriesSymbolHeight;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_chartLegendColumnDef, m_chartLegendColumnDef.ChartDef, m_chartLegendColumnDef.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public ChartColumnType ColumnType
		{
			get
			{
				if (!m_columnType.HasValue && !m_chartLegendColumnDef.ChartDef.IsOldSnapshot)
				{
					m_columnType = m_chartLegendColumnDef.ChartLegendColumnDef.EvaluateColumnType(ReportScopeInstance, m_chartLegendColumnDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_columnType.Value;
			}
		}

		public string Value
		{
			get
			{
				if (m_value == null && !m_chartLegendColumnDef.ChartDef.IsOldSnapshot)
				{
					m_value = m_chartLegendColumnDef.ChartLegendColumnDef.EvaluateValue(ReportScopeInstance, m_chartLegendColumnDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (m_toolTip == null && !m_chartLegendColumnDef.ChartDef.IsOldSnapshot)
				{
					m_toolTip = m_chartLegendColumnDef.ChartLegendColumnDef.EvaluateToolTip(ReportScopeInstance, m_chartLegendColumnDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_toolTip;
			}
		}

		public ReportSize MinimumWidth
		{
			get
			{
				if (m_minimumWidth == null && !m_chartLegendColumnDef.ChartDef.IsOldSnapshot)
				{
					m_minimumWidth = new ReportSize(m_chartLegendColumnDef.ChartLegendColumnDef.EvaluateMinimumWidth(ReportScopeInstance, m_chartLegendColumnDef.ChartDef.RenderingContext.OdpContext));
				}
				return m_minimumWidth;
			}
		}

		public ReportSize MaximumWidth
		{
			get
			{
				if (m_maximumWidth == null && !m_chartLegendColumnDef.ChartDef.IsOldSnapshot)
				{
					m_maximumWidth = new ReportSize(m_chartLegendColumnDef.ChartLegendColumnDef.EvaluateMaximumWidth(ReportScopeInstance, m_chartLegendColumnDef.ChartDef.RenderingContext.OdpContext));
				}
				return m_maximumWidth;
			}
		}

		public int SeriesSymbolWidth
		{
			get
			{
				if (!m_seriesSymbolWidth.HasValue && !m_chartLegendColumnDef.ChartDef.IsOldSnapshot)
				{
					m_seriesSymbolWidth = m_chartLegendColumnDef.ChartLegendColumnDef.EvaluateSeriesSymbolWidth(ReportScopeInstance, m_chartLegendColumnDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_seriesSymbolWidth.Value;
			}
		}

		public int SeriesSymbolHeight
		{
			get
			{
				if (!m_seriesSymbolHeight.HasValue && !m_chartLegendColumnDef.ChartDef.IsOldSnapshot)
				{
					m_seriesSymbolHeight = m_chartLegendColumnDef.ChartLegendColumnDef.EvaluateSeriesSymbolHeight(ReportScopeInstance, m_chartLegendColumnDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_seriesSymbolHeight.Value;
			}
		}

		internal ChartLegendColumnInstance(ChartLegendColumn chartLegendColumnDef)
			: base(chartLegendColumnDef.ChartDef)
		{
			m_chartLegendColumnDef = chartLegendColumnDef;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_columnType = null;
			m_value = null;
			m_toolTip = null;
			m_minimumWidth = null;
			m_maximumWidth = null;
			m_seriesSymbolWidth = null;
			m_seriesSymbolHeight = null;
		}
	}
}

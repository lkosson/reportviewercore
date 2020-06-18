namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartSeriesInstance : BaseInstance
	{
		private InternalChartSeries m_chartSeriesDef;

		private StyleInstance m_style;

		private ChartSeriesType? m_type;

		private ChartSeriesSubtype? m_subtype;

		private string m_legendName;

		private string m_legendText;

		private bool? m_hideInLegend;

		private string m_chartAreaName;

		private string m_valueAxisName;

		private string m_categoryAxisName;

		private string m_toolTip;

		private bool? m_hidden;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_chartSeriesDef, m_chartSeriesDef.ChartDef, m_chartSeriesDef.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public ChartSeriesType Type
		{
			get
			{
				if (!m_type.HasValue)
				{
					if (m_chartSeriesDef.ChartDef.IsOldSnapshot)
					{
						m_type = m_chartSeriesDef.Type.Value;
					}
					else
					{
						m_type = m_chartSeriesDef.ChartSeriesDef.EvaluateType(ReportScopeInstance, m_chartSeriesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return m_type.Value;
			}
		}

		public ChartSeriesSubtype Subtype
		{
			get
			{
				if (!m_subtype.HasValue)
				{
					if (m_chartSeriesDef.ChartDef.IsOldSnapshot)
					{
						m_subtype = m_chartSeriesDef.Subtype.Value;
					}
					else
					{
						m_subtype = m_chartSeriesDef.ChartSeriesDef.EvaluateSubtype(ReportScopeInstance, m_chartSeriesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return m_subtype.Value;
			}
		}

		public string LegendName
		{
			get
			{
				if (m_legendName == null && !m_chartSeriesDef.ChartDef.IsOldSnapshot)
				{
					m_legendName = m_chartSeriesDef.ChartSeriesDef.EvaluateLegendName(ReportScopeInstance, m_chartSeriesDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_legendName;
			}
		}

		internal string LegendText
		{
			get
			{
				if (m_legendText == null && !m_chartSeriesDef.ChartDef.IsOldSnapshot)
				{
					m_legendText = m_chartSeriesDef.ChartSeriesDef.EvaluateLegendText(ReportScopeInstance, m_chartSeriesDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_legendText;
			}
		}

		internal bool HideInLegend
		{
			get
			{
				if (!m_hideInLegend.HasValue && !m_chartSeriesDef.ChartDef.IsOldSnapshot)
				{
					m_hideInLegend = m_chartSeriesDef.ChartSeriesDef.EvaluateHideInLegend(ReportScopeInstance, m_chartSeriesDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_hideInLegend.Value;
			}
		}

		public string ChartAreaName
		{
			get
			{
				if (m_chartAreaName == null && !m_chartSeriesDef.ChartDef.IsOldSnapshot)
				{
					m_chartAreaName = m_chartSeriesDef.ChartSeriesDef.EvaluateChartAreaName(ReportScopeInstance, m_chartSeriesDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_chartAreaName;
			}
		}

		public string ValueAxisName
		{
			get
			{
				if (m_valueAxisName == null && !m_chartSeriesDef.ChartDef.IsOldSnapshot)
				{
					m_valueAxisName = m_chartSeriesDef.ChartSeriesDef.EvaluateValueAxisName(ReportScopeInstance, m_chartSeriesDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_valueAxisName;
			}
		}

		public string CategoryAxisName
		{
			get
			{
				if (m_categoryAxisName == null && !m_chartSeriesDef.ChartDef.IsOldSnapshot)
				{
					m_categoryAxisName = m_chartSeriesDef.ChartSeriesDef.EvaluateCategoryAxisName(ReportScopeInstance, m_chartSeriesDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_categoryAxisName;
			}
		}

		internal string ToolTip
		{
			get
			{
				if (m_toolTip == null)
				{
					m_toolTip = m_chartSeriesDef.ChartSeriesDef.EvaluateToolTip(ReportScopeInstance, m_chartSeriesDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_toolTip;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!m_hidden.HasValue)
				{
					m_hidden = m_chartSeriesDef.ChartSeriesDef.EvaluateHidden(ReportScopeInstance, m_chartSeriesDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_hidden.Value;
			}
		}

		internal ChartSeriesInstance(InternalChartSeries chartSeriesDef)
			: base(chartSeriesDef.ReportScope)
		{
			m_chartSeriesDef = chartSeriesDef;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_type = null;
			m_subtype = null;
			m_legendName = null;
			m_legendText = null;
			m_hideInLegend = null;
			m_chartAreaName = null;
			m_valueAxisName = null;
			m_categoryAxisName = null;
			m_toolTip = null;
			m_hidden = null;
		}
	}
}

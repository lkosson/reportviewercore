namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartMarkerInstance : BaseInstance
	{
		private ChartMarker m_markerDef;

		private StyleInstance m_style;

		private ReportSize m_size;

		private ChartMarkerTypes? m_type;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_markerDef, m_markerDef.ReportScope, m_markerDef.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public ReportSize Size
		{
			get
			{
				if (m_size == null && !m_markerDef.ChartDef.IsOldSnapshot)
				{
					m_size = new ReportSize(m_markerDef.MarkerDef.EvaluateChartMarkerSize(ReportScopeInstance, m_markerDef.ChartDef.RenderingContext.OdpContext));
				}
				return m_size;
			}
		}

		public ChartMarkerTypes Type
		{
			get
			{
				if (!m_type.HasValue && !m_markerDef.ChartDef.IsOldSnapshot)
				{
					m_type = m_markerDef.MarkerDef.EvaluateChartMarkerType(ReportScopeInstance, m_markerDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_type.Value;
			}
		}

		internal ChartMarkerInstance(ChartMarker markerDef)
			: base(markerDef.ReportScope)
		{
			m_markerDef = markerDef;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_type = null;
			m_size = null;
		}
	}
}

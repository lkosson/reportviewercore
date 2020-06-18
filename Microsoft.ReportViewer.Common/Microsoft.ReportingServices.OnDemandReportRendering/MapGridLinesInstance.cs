namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapGridLinesInstance : BaseInstance
	{
		private MapGridLines m_defObject;

		private StyleInstance m_style;

		private bool? m_hidden;

		private double? m_interval;

		private bool? m_showLabels;

		private MapLabelPosition? m_labelPosition;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_defObject, m_defObject.MapDef.ReportScope, m_defObject.MapDef.RenderingContext);
				}
				return m_style;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!m_hidden.HasValue)
				{
					m_hidden = m_defObject.MapGridLinesDef.EvaluateHidden(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_hidden.Value;
			}
		}

		public double Interval
		{
			get
			{
				if (!m_interval.HasValue)
				{
					m_interval = m_defObject.MapGridLinesDef.EvaluateInterval(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_interval.Value;
			}
		}

		public bool ShowLabels
		{
			get
			{
				if (!m_showLabels.HasValue)
				{
					m_showLabels = m_defObject.MapGridLinesDef.EvaluateShowLabels(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_showLabels.Value;
			}
		}

		public MapLabelPosition LabelPosition
		{
			get
			{
				if (!m_labelPosition.HasValue)
				{
					m_labelPosition = m_defObject.MapGridLinesDef.EvaluateLabelPosition(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_labelPosition.Value;
			}
		}

		internal MapGridLinesInstance(MapGridLines defObject)
			: base(defObject.MapDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_hidden = null;
			m_interval = null;
			m_showLabels = null;
			m_labelPosition = null;
		}
	}
}

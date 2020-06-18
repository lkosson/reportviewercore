namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class GaugePointerInstance : BaseInstance
	{
		private GaugePointer m_defObject;

		private StyleInstance m_style;

		private GaugeBarStarts? m_barStart;

		private double? m_distanceFromScale;

		private double? m_markerLength;

		private GaugeMarkerStyles? m_markerStyle;

		private GaugePointerPlacements? m_placement;

		private bool? m_snappingEnabled;

		private double? m_snappingInterval;

		private string m_toolTip;

		private bool? m_hidden;

		private double? m_width;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_defObject, m_defObject.GaugePanelDef, m_defObject.GaugePanelDef.RenderingContext);
				}
				return m_style;
			}
		}

		public GaugeBarStarts BarStart
		{
			get
			{
				if (!m_barStart.HasValue)
				{
					m_barStart = m_defObject.GaugePointerDef.EvaluateBarStart(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_barStart.Value;
			}
		}

		public double DistanceFromScale
		{
			get
			{
				if (!m_distanceFromScale.HasValue)
				{
					m_distanceFromScale = m_defObject.GaugePointerDef.EvaluateDistanceFromScale(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_distanceFromScale.Value;
			}
		}

		public double MarkerLength
		{
			get
			{
				if (!m_markerLength.HasValue)
				{
					m_markerLength = m_defObject.GaugePointerDef.EvaluateMarkerLength(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_markerLength.Value;
			}
		}

		public GaugeMarkerStyles MarkerStyle
		{
			get
			{
				if (!m_markerStyle.HasValue)
				{
					m_markerStyle = m_defObject.GaugePointerDef.EvaluateMarkerStyle(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_markerStyle.Value;
			}
		}

		public GaugePointerPlacements Placement
		{
			get
			{
				if (!m_placement.HasValue)
				{
					m_placement = m_defObject.GaugePointerDef.EvaluatePlacement(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_placement.Value;
			}
		}

		public bool SnappingEnabled
		{
			get
			{
				if (!m_snappingEnabled.HasValue)
				{
					m_snappingEnabled = m_defObject.GaugePointerDef.EvaluateSnappingEnabled(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_snappingEnabled.Value;
			}
		}

		public double SnappingInterval
		{
			get
			{
				if (!m_snappingInterval.HasValue)
				{
					m_snappingInterval = m_defObject.GaugePointerDef.EvaluateSnappingInterval(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_snappingInterval.Value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (m_toolTip == null)
				{
					m_toolTip = m_defObject.GaugePointerDef.EvaluateToolTip(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
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
					m_hidden = m_defObject.GaugePointerDef.EvaluateHidden(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_hidden.Value;
			}
		}

		public double Width
		{
			get
			{
				if (!m_width.HasValue)
				{
					m_width = m_defObject.GaugePointerDef.EvaluateWidth(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_width.Value;
			}
		}

		internal GaugePointerInstance(GaugePointer defObject)
			: base(defObject.GaugePanelDef)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_barStart = null;
			m_distanceFromScale = null;
			m_markerLength = null;
			m_markerStyle = null;
			m_placement = null;
			m_snappingEnabled = null;
			m_snappingInterval = null;
			m_toolTip = null;
			m_hidden = null;
			m_width = null;
		}
	}
}

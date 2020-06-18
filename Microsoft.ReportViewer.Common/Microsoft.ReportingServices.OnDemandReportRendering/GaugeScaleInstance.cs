namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class GaugeScaleInstance : BaseInstance
	{
		private GaugeScale m_defObject;

		private StyleInstance m_style;

		private double? m_interval;

		private double? m_intervalOffset;

		private bool? m_logarithmic;

		private double? m_logarithmicBase;

		private double? m_multiplier;

		private bool? m_reversed;

		private bool? m_tickMarksOnTop;

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

		public double Interval
		{
			get
			{
				if (!m_interval.HasValue)
				{
					m_interval = m_defObject.GaugeScaleDef.EvaluateInterval(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_interval.Value;
			}
		}

		public double IntervalOffset
		{
			get
			{
				if (!m_intervalOffset.HasValue)
				{
					m_intervalOffset = m_defObject.GaugeScaleDef.EvaluateIntervalOffset(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_intervalOffset.Value;
			}
		}

		public bool Logarithmic
		{
			get
			{
				if (!m_logarithmic.HasValue)
				{
					m_logarithmic = m_defObject.GaugeScaleDef.EvaluateLogarithmic(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_logarithmic.Value;
			}
		}

		public double LogarithmicBase
		{
			get
			{
				if (!m_logarithmicBase.HasValue)
				{
					m_logarithmicBase = m_defObject.GaugeScaleDef.EvaluateLogarithmicBase(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_logarithmicBase.Value;
			}
		}

		public double Multiplier
		{
			get
			{
				if (!m_multiplier.HasValue)
				{
					m_multiplier = m_defObject.GaugeScaleDef.EvaluateMultiplier(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_multiplier.Value;
			}
		}

		public bool Reversed
		{
			get
			{
				if (!m_reversed.HasValue)
				{
					m_reversed = m_defObject.GaugeScaleDef.EvaluateReversed(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_reversed.Value;
			}
		}

		public bool TickMarksOnTop
		{
			get
			{
				if (!m_tickMarksOnTop.HasValue)
				{
					m_tickMarksOnTop = m_defObject.GaugeScaleDef.EvaluateTickMarksOnTop(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_tickMarksOnTop.Value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (m_toolTip == null)
				{
					m_toolTip = m_defObject.GaugeScaleDef.EvaluateToolTip(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
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
					m_hidden = m_defObject.GaugeScaleDef.EvaluateHidden(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
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
					m_width = m_defObject.GaugeScaleDef.EvaluateWidth(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_width.Value;
			}
		}

		internal GaugeScaleInstance(GaugeScale defObject)
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
			m_interval = null;
			m_intervalOffset = null;
			m_logarithmic = null;
			m_logarithmicBase = null;
			m_multiplier = null;
			m_reversed = null;
			m_tickMarksOnTop = null;
			m_toolTip = null;
			m_hidden = null;
			m_width = null;
		}
	}
}

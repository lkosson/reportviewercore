namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ThermometerInstance : BaseInstance
	{
		private Thermometer m_defObject;

		private StyleInstance m_style;

		private double? m_bulbOffset;

		private double? m_bulbSize;

		private GaugeThermometerStyles? m_thermometerStyle;

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

		public double BulbOffset
		{
			get
			{
				if (!m_bulbOffset.HasValue)
				{
					m_bulbOffset = m_defObject.ThermometerDef.EvaluateBulbOffset(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_bulbOffset.Value;
			}
		}

		public double BulbSize
		{
			get
			{
				if (!m_bulbSize.HasValue)
				{
					m_bulbSize = m_defObject.ThermometerDef.EvaluateBulbSize(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_bulbSize.Value;
			}
		}

		public GaugeThermometerStyles ThermometerStyle
		{
			get
			{
				if (!m_thermometerStyle.HasValue)
				{
					m_thermometerStyle = m_defObject.ThermometerDef.EvaluateThermometerStyle(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_thermometerStyle.Value;
			}
		}

		internal ThermometerInstance(Thermometer defObject)
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
			m_bulbOffset = null;
			m_bulbSize = null;
			m_thermometerStyle = null;
		}
	}
}

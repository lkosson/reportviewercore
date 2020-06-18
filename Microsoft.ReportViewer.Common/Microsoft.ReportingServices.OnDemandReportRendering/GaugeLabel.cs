using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeLabel : GaugePanelItem
	{
		private ReportStringProperty m_text;

		private ReportDoubleProperty m_angle;

		private ReportEnumProperty<GaugeResizeModes> m_resizeMode;

		private ReportSizeProperty m_textShadowOffset;

		private ReportBoolProperty m_useFontPercent;

		public ReportStringProperty Text
		{
			get
			{
				if (m_text == null && GaugeLabelDef.Text != null)
				{
					m_text = new ReportStringProperty(GaugeLabelDef.Text);
				}
				return m_text;
			}
		}

		public ReportDoubleProperty Angle
		{
			get
			{
				if (m_angle == null && GaugeLabelDef.Angle != null)
				{
					m_angle = new ReportDoubleProperty(GaugeLabelDef.Angle);
				}
				return m_angle;
			}
		}

		public ReportEnumProperty<GaugeResizeModes> ResizeMode
		{
			get
			{
				if (m_resizeMode == null && GaugeLabelDef.ResizeMode != null)
				{
					m_resizeMode = new ReportEnumProperty<GaugeResizeModes>(GaugeLabelDef.ResizeMode.IsExpression, GaugeLabelDef.ResizeMode.OriginalText, EnumTranslator.TranslateGaugeResizeModes(GaugeLabelDef.ResizeMode.StringValue, null));
				}
				return m_resizeMode;
			}
		}

		public ReportSizeProperty TextShadowOffset
		{
			get
			{
				if (m_textShadowOffset == null && GaugeLabelDef.TextShadowOffset != null)
				{
					m_textShadowOffset = new ReportSizeProperty(GaugeLabelDef.TextShadowOffset);
				}
				return m_textShadowOffset;
			}
		}

		public ReportBoolProperty UseFontPercent
		{
			get
			{
				if (m_useFontPercent == null && GaugeLabelDef.UseFontPercent != null)
				{
					m_useFontPercent = new ReportBoolProperty(GaugeLabelDef.UseFontPercent);
				}
				return m_useFontPercent;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.GaugeLabel GaugeLabelDef => (Microsoft.ReportingServices.ReportIntermediateFormat.GaugeLabel)m_defObject;

		public new GaugeLabelInstance Instance => (GaugeLabelInstance)GetInstance();

		internal GaugeLabel(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeLabel defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
			m_defObject = defObject;
			m_gaugePanel = gaugePanel;
		}

		internal override BaseInstance GetInstance()
		{
			if (m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new GaugeLabelInstance(this);
			}
			return m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}

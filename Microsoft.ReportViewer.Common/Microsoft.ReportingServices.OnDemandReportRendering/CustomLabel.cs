using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class CustomLabel : GaugePanelObjectCollectionItem, IROMStyleDefinitionContainer
	{
		private GaugePanel m_gaugePanel;

		private Microsoft.ReportingServices.ReportIntermediateFormat.CustomLabel m_defObject;

		private Style m_style;

		private ReportStringProperty m_text;

		private ReportBoolProperty m_allowUpsideDown;

		private ReportDoubleProperty m_distanceFromScale;

		private ReportDoubleProperty m_fontAngle;

		private ReportEnumProperty<GaugeLabelPlacements> m_placement;

		private ReportBoolProperty m_rotateLabel;

		private TickMarkStyle m_tickMarkStyle;

		private ReportDoubleProperty m_value;

		private ReportBoolProperty m_hidden;

		private ReportBoolProperty m_useFontPercent;

		public Style Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new Style(m_gaugePanel, m_gaugePanel, m_defObject, m_gaugePanel.RenderingContext);
				}
				return m_style;
			}
		}

		public string Name => m_defObject.Name;

		public ReportStringProperty Text
		{
			get
			{
				if (m_text == null && m_defObject.Text != null)
				{
					m_text = new ReportStringProperty(m_defObject.Text);
				}
				return m_text;
			}
		}

		public ReportBoolProperty AllowUpsideDown
		{
			get
			{
				if (m_allowUpsideDown == null && m_defObject.AllowUpsideDown != null)
				{
					m_allowUpsideDown = new ReportBoolProperty(m_defObject.AllowUpsideDown);
				}
				return m_allowUpsideDown;
			}
		}

		public ReportDoubleProperty DistanceFromScale
		{
			get
			{
				if (m_distanceFromScale == null && m_defObject.DistanceFromScale != null)
				{
					m_distanceFromScale = new ReportDoubleProperty(m_defObject.DistanceFromScale);
				}
				return m_distanceFromScale;
			}
		}

		public ReportDoubleProperty FontAngle
		{
			get
			{
				if (m_fontAngle == null && m_defObject.FontAngle != null)
				{
					m_fontAngle = new ReportDoubleProperty(m_defObject.FontAngle);
				}
				return m_fontAngle;
			}
		}

		public ReportEnumProperty<GaugeLabelPlacements> Placement
		{
			get
			{
				if (m_placement == null && m_defObject.Placement != null)
				{
					m_placement = new ReportEnumProperty<GaugeLabelPlacements>(m_defObject.Placement.IsExpression, m_defObject.Placement.OriginalText, EnumTranslator.TranslateGaugeLabelPlacements(m_defObject.Placement.StringValue, null));
				}
				return m_placement;
			}
		}

		public ReportBoolProperty RotateLabel
		{
			get
			{
				if (m_rotateLabel == null && m_defObject.RotateLabel != null)
				{
					m_rotateLabel = new ReportBoolProperty(m_defObject.RotateLabel);
				}
				return m_rotateLabel;
			}
		}

		public TickMarkStyle TickMarkStyle
		{
			get
			{
				if (m_tickMarkStyle == null && m_defObject.TickMarkStyle != null)
				{
					m_tickMarkStyle = new TickMarkStyle(m_defObject.TickMarkStyle, m_gaugePanel);
				}
				return m_tickMarkStyle;
			}
		}

		public ReportDoubleProperty Value
		{
			get
			{
				if (m_value == null && m_defObject.Value != null)
				{
					m_value = new ReportDoubleProperty(m_defObject.Value);
				}
				return m_value;
			}
		}

		public ReportBoolProperty Hidden
		{
			get
			{
				if (m_hidden == null && m_defObject.Hidden != null)
				{
					m_hidden = new ReportBoolProperty(m_defObject.Hidden);
				}
				return m_hidden;
			}
		}

		public ReportBoolProperty UseFontPercent
		{
			get
			{
				if (m_useFontPercent == null && m_defObject.UseFontPercent != null)
				{
					m_useFontPercent = new ReportBoolProperty(m_defObject.UseFontPercent);
				}
				return m_useFontPercent;
			}
		}

		internal GaugePanel GaugePanelDef => m_gaugePanel;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.CustomLabel CustomLabelDef => m_defObject;

		public CustomLabelInstance Instance
		{
			get
			{
				if (m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new CustomLabelInstance(this);
				}
				return (CustomLabelInstance)m_instance;
			}
		}

		internal CustomLabel(Microsoft.ReportingServices.ReportIntermediateFormat.CustomLabel defObject, GaugePanel gaugePanel)
		{
			m_defObject = defObject;
			m_gaugePanel = gaugePanel;
		}

		internal override void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			if (m_tickMarkStyle != null)
			{
				m_tickMarkStyle.SetNewContext();
			}
		}
	}
}

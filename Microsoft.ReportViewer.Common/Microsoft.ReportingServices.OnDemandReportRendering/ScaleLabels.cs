using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ScaleLabels : IROMStyleDefinitionContainer
	{
		private GaugePanel m_gaugePanel;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ScaleLabels m_defObject;

		private ScaleLabelsInstance m_instance;

		private Style m_style;

		private ReportDoubleProperty m_interval;

		private ReportDoubleProperty m_intervalOffset;

		private ReportBoolProperty m_allowUpsideDown;

		private ReportDoubleProperty m_distanceFromScale;

		private ReportDoubleProperty m_fontAngle;

		private ReportEnumProperty<GaugeLabelPlacements> m_placement;

		private ReportBoolProperty m_rotateLabels;

		private ReportBoolProperty m_showEndLabels;

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

		public ReportDoubleProperty Interval
		{
			get
			{
				if (m_interval == null && m_defObject.Interval != null)
				{
					m_interval = new ReportDoubleProperty(m_defObject.Interval);
				}
				return m_interval;
			}
		}

		public ReportDoubleProperty IntervalOffset
		{
			get
			{
				if (m_intervalOffset == null && m_defObject.IntervalOffset != null)
				{
					m_intervalOffset = new ReportDoubleProperty(m_defObject.IntervalOffset);
				}
				return m_intervalOffset;
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

		public ReportBoolProperty RotateLabels
		{
			get
			{
				if (m_rotateLabels == null && m_defObject.RotateLabels != null)
				{
					m_rotateLabels = new ReportBoolProperty(m_defObject.RotateLabels);
				}
				return m_rotateLabels;
			}
		}

		public ReportBoolProperty ShowEndLabels
		{
			get
			{
				if (m_showEndLabels == null && m_defObject.ShowEndLabels != null)
				{
					m_showEndLabels = new ReportBoolProperty(m_defObject.ShowEndLabels);
				}
				return m_showEndLabels;
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

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ScaleLabels ScaleLabelsDef => m_defObject;

		public ScaleLabelsInstance Instance
		{
			get
			{
				if (m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ScaleLabelsInstance(this);
				}
				return m_instance;
			}
		}

		internal ScaleLabels(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleLabels defObject, GaugePanel gaugePanel)
		{
			m_defObject = defObject;
			m_gaugePanel = gaugePanel;
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
		}
	}
}

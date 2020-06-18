using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class StateIndicator : GaugePanelItem
	{
		private GaugeInputValue m_gaugeInputValue;

		private ReportEnumProperty<GaugeStateIndicatorStyles> m_indicatorStyle;

		private IndicatorImage m_indicatorImage;

		private ReportDoubleProperty m_scaleFactor;

		private IndicatorStateCollection m_indicatorStates;

		private ReportEnumProperty<GaugeResizeModes> m_resizeMode;

		private ReportDoubleProperty m_angle;

		private ReportEnumProperty<GaugeTransformationType> m_transformationType;

		private GaugeInputValue m_maximumValue;

		private GaugeInputValue m_minimumValue;

		private string m_compiledStateName;

		public GaugeInputValue GaugeInputValue
		{
			get
			{
				if (m_gaugeInputValue == null && StateIndicatorDef.GaugeInputValue != null)
				{
					m_gaugeInputValue = new GaugeInputValue(StateIndicatorDef.GaugeInputValue, m_gaugePanel);
				}
				return m_gaugeInputValue;
			}
		}

		public ReportEnumProperty<GaugeTransformationType> TransformationType
		{
			get
			{
				if (m_transformationType == null && StateIndicatorDef.TransformationType != null)
				{
					m_transformationType = new ReportEnumProperty<GaugeTransformationType>(StateIndicatorDef.TransformationType.IsExpression, StateIndicatorDef.TransformationType.OriginalText, EnumTranslator.TranslateGaugeTransformationType(StateIndicatorDef.TransformationType.StringValue, null));
				}
				return m_transformationType;
			}
		}

		public string TransformationScope => StateIndicatorDef.TransformationScope;

		public GaugeInputValue MaximumValue
		{
			get
			{
				if (m_maximumValue == null && StateIndicatorDef.MaximumValue != null)
				{
					m_maximumValue = new GaugeInputValue(StateIndicatorDef.MaximumValue, m_gaugePanel);
				}
				return m_maximumValue;
			}
		}

		public GaugeInputValue MinimumValue
		{
			get
			{
				if (m_minimumValue == null && StateIndicatorDef.MinimumValue != null)
				{
					m_minimumValue = new GaugeInputValue(StateIndicatorDef.MinimumValue, m_gaugePanel);
				}
				return m_minimumValue;
			}
		}

		public ReportEnumProperty<GaugeStateIndicatorStyles> IndicatorStyle
		{
			get
			{
				if (m_indicatorStyle == null && StateIndicatorDef.IndicatorStyle != null)
				{
					m_indicatorStyle = new ReportEnumProperty<GaugeStateIndicatorStyles>(StateIndicatorDef.IndicatorStyle.IsExpression, StateIndicatorDef.IndicatorStyle.OriginalText, EnumTranslator.TranslateGaugeStateIndicatorStyles(StateIndicatorDef.IndicatorStyle.StringValue, null));
				}
				return m_indicatorStyle;
			}
		}

		public IndicatorImage IndicatorImage
		{
			get
			{
				if (m_indicatorImage == null && StateIndicatorDef.IndicatorImage != null)
				{
					m_indicatorImage = new IndicatorImage(StateIndicatorDef.IndicatorImage, m_gaugePanel);
				}
				return m_indicatorImage;
			}
		}

		public ReportDoubleProperty ScaleFactor
		{
			get
			{
				if (m_scaleFactor == null && StateIndicatorDef.ScaleFactor != null)
				{
					m_scaleFactor = new ReportDoubleProperty(StateIndicatorDef.ScaleFactor);
				}
				return m_scaleFactor;
			}
		}

		public IndicatorStateCollection IndicatorStates
		{
			get
			{
				if (m_indicatorStates == null && StateIndicatorDef.IndicatorStates != null)
				{
					m_indicatorStates = new IndicatorStateCollection(this, m_gaugePanel);
				}
				return m_indicatorStates;
			}
		}

		public ReportEnumProperty<GaugeResizeModes> ResizeMode
		{
			get
			{
				if (m_resizeMode == null && StateIndicatorDef.ResizeMode != null)
				{
					m_resizeMode = new ReportEnumProperty<GaugeResizeModes>(StateIndicatorDef.ResizeMode.IsExpression, StateIndicatorDef.ResizeMode.OriginalText, EnumTranslator.TranslateGaugeResizeModes(StateIndicatorDef.ResizeMode.StringValue, null));
				}
				return m_resizeMode;
			}
		}

		public ReportDoubleProperty Angle
		{
			get
			{
				if (m_angle == null && StateIndicatorDef.Angle != null)
				{
					m_angle = new ReportDoubleProperty(StateIndicatorDef.Angle);
				}
				return m_angle;
			}
		}

		public string StateDataElementName => StateIndicatorDef.StateDataElementName;

		public DataElementOutputTypes StateDataElementOutput => StateIndicatorDef.StateDataElementOutput;

		public string CompiledStateName
		{
			get
			{
				m_gaugePanel.ProcessCompiledInstances();
				return m_compiledStateName;
			}
			set
			{
				m_compiledStateName = value;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator StateIndicatorDef => (Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator)m_defObject;

		public new StateIndicatorInstance Instance => (StateIndicatorInstance)GetInstance();

		internal StateIndicator(Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
		}

		internal override BaseInstance GetInstance()
		{
			if (m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new StateIndicatorInstance(this);
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
			if (m_gaugeInputValue != null)
			{
				m_gaugeInputValue.SetNewContext();
			}
			if (m_indicatorImage != null)
			{
				m_indicatorImage.SetNewContext();
			}
			if (m_indicatorStates != null)
			{
				m_indicatorStates.SetNewContext();
			}
			if (m_maximumValue != null)
			{
				m_maximumValue.SetNewContext();
			}
			if (m_minimumValue != null)
			{
				m_minimumValue.SetNewContext();
			}
		}
	}
}

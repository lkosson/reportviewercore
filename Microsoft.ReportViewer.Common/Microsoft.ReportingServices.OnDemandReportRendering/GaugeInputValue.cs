using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeInputValue
	{
		private GaugePanel m_gaugePanel;

		private Microsoft.ReportingServices.ReportIntermediateFormat.GaugeInputValue m_defObject;

		private GaugeInputValueInstance m_instance;

		private CompiledGaugeInputValueInstance m_compiledInstance;

		private ReportVariantProperty m_value;

		private ReportEnumProperty<GaugeInputValueFormulas> m_formula;

		private ReportDoubleProperty m_minPercent;

		private ReportDoubleProperty m_maxPercent;

		private ReportDoubleProperty m_multiplier;

		private ReportDoubleProperty m_addConstant;

		public ReportVariantProperty Value
		{
			get
			{
				if (m_value == null && m_defObject.Value != null)
				{
					m_value = new ReportVariantProperty(m_defObject.Value);
				}
				return m_value;
			}
		}

		public ReportEnumProperty<GaugeInputValueFormulas> Formula
		{
			get
			{
				if (m_formula == null && m_defObject.Formula != null)
				{
					m_formula = new ReportEnumProperty<GaugeInputValueFormulas>(m_defObject.Formula.IsExpression, m_defObject.Formula.OriginalText, EnumTranslator.TranslateGaugeInputValueFormulas(m_defObject.Formula.StringValue, null));
				}
				return m_formula;
			}
		}

		public ReportDoubleProperty MinPercent
		{
			get
			{
				if (m_minPercent == null && m_defObject.MinPercent != null)
				{
					m_minPercent = new ReportDoubleProperty(m_defObject.MinPercent);
				}
				return m_minPercent;
			}
		}

		public ReportDoubleProperty MaxPercent
		{
			get
			{
				if (m_maxPercent == null && m_defObject.MaxPercent != null)
				{
					m_maxPercent = new ReportDoubleProperty(m_defObject.MaxPercent);
				}
				return m_maxPercent;
			}
		}

		public ReportDoubleProperty Multiplier
		{
			get
			{
				if (m_multiplier == null && m_defObject.Multiplier != null)
				{
					m_multiplier = new ReportDoubleProperty(m_defObject.Multiplier);
				}
				return m_multiplier;
			}
		}

		public ReportDoubleProperty AddConstant
		{
			get
			{
				if (m_addConstant == null && m_defObject.AddConstant != null)
				{
					m_addConstant = new ReportDoubleProperty(m_defObject.AddConstant);
				}
				return m_addConstant;
			}
		}

		public string DataElementName => m_defObject.DataElementName;

		public DataElementOutputTypes DataElementOutput => m_defObject.DataElementOutput;

		internal GaugePanel GaugePanelDef => m_gaugePanel;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.GaugeInputValue GaugeInputValueDef => m_defObject;

		public GaugeInputValueInstance Instance
		{
			get
			{
				if (m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new GaugeInputValueInstance(this);
				}
				return m_instance;
			}
		}

		public CompiledGaugeInputValueInstance CompiledInstance
		{
			get
			{
				GaugePanelDef.ProcessCompiledInstances();
				return m_compiledInstance;
			}
			internal set
			{
				m_compiledInstance = value;
			}
		}

		internal GaugeInputValue(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeInputValue defObject, GaugePanel gaugePanel)
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
		}
	}
}

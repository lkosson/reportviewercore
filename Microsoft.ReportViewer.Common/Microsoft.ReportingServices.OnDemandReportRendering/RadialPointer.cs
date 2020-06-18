using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class RadialPointer : GaugePointer
	{
		private ReportEnumProperty<RadialPointerTypes> m_type;

		private PointerCap m_pointerCap;

		private ReportEnumProperty<RadialPointerNeedleStyles> m_needleStyle;

		public ReportEnumProperty<RadialPointerTypes> Type
		{
			get
			{
				if (m_type == null && RadialPointerDef.Type != null)
				{
					m_type = new ReportEnumProperty<RadialPointerTypes>(RadialPointerDef.Type.IsExpression, RadialPointerDef.Type.OriginalText, EnumTranslator.TranslateRadialPointerTypes(RadialPointerDef.Type.StringValue, null));
				}
				return m_type;
			}
		}

		public PointerCap PointerCap
		{
			get
			{
				if (m_pointerCap == null && RadialPointerDef.PointerCap != null)
				{
					m_pointerCap = new PointerCap(RadialPointerDef.PointerCap, m_gaugePanel);
				}
				return m_pointerCap;
			}
		}

		public ReportEnumProperty<RadialPointerNeedleStyles> NeedleStyle
		{
			get
			{
				if (m_needleStyle == null && RadialPointerDef.NeedleStyle != null)
				{
					m_needleStyle = new ReportEnumProperty<RadialPointerNeedleStyles>(RadialPointerDef.NeedleStyle.IsExpression, RadialPointerDef.NeedleStyle.OriginalText, EnumTranslator.TranslateRadialPointerNeedleStyles(RadialPointerDef.NeedleStyle.StringValue, null));
				}
				return m_needleStyle;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.RadialPointer RadialPointerDef => (Microsoft.ReportingServices.ReportIntermediateFormat.RadialPointer)m_defObject;

		public new RadialPointerInstance Instance => (RadialPointerInstance)GetInstance();

		internal RadialPointer(Microsoft.ReportingServices.ReportIntermediateFormat.RadialPointer defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
			m_defObject = defObject;
			m_gaugePanel = gaugePanel;
		}

		internal override GaugePointerInstance GetInstance()
		{
			if (m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new RadialPointerInstance(this);
			}
			return (GaugePointerInstance)m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_pointerCap != null)
			{
				m_pointerCap.SetNewContext();
			}
		}
	}
}

using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class StateIndicatorInstance : GaugePanelItemInstance
	{
		private StateIndicator m_defObject;

		private GaugeStateIndicatorStyles? m_indicatorStyle;

		private double? m_scaleFactor;

		private GaugeResizeModes? m_resizeMode;

		private double? m_angle;

		private GaugeTransformationType? m_transformationType;

		public GaugeStateIndicatorStyles IndicatorStyle
		{
			get
			{
				if (!m_indicatorStyle.HasValue)
				{
					m_indicatorStyle = ((Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator)m_defObject.GaugePanelItemDef).EvaluateIndicatorStyle(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_indicatorStyle.Value;
			}
		}

		public double ScaleFactor
		{
			get
			{
				if (!m_scaleFactor.HasValue)
				{
					m_scaleFactor = ((Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator)m_defObject.GaugePanelItemDef).EvaluateScaleFactor(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_scaleFactor.Value;
			}
		}

		public GaugeResizeModes ResizeMode
		{
			get
			{
				if (!m_resizeMode.HasValue)
				{
					m_resizeMode = ((Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator)m_defObject.GaugePanelItemDef).EvaluateResizeMode(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_resizeMode.Value;
			}
		}

		public double Angle
		{
			get
			{
				if (!m_angle.HasValue)
				{
					m_angle = ((Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator)m_defObject.GaugePanelItemDef).EvaluateAngle(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_angle.Value;
			}
		}

		public GaugeTransformationType TransformationType
		{
			get
			{
				if (!m_transformationType.HasValue)
				{
					m_transformationType = ((Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator)m_defObject.GaugePanelItemDef).EvaluateTransformationType(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_transformationType.Value;
			}
		}

		internal StateIndicatorInstance(StateIndicator defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_indicatorStyle = null;
			m_scaleFactor = null;
			m_resizeMode = null;
			m_angle = null;
			m_transformationType = null;
		}
	}
}

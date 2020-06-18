using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeLabelInstance : GaugePanelItemInstance
	{
		private GaugeLabel m_defObject;

		private VariantResult? m_textResult;

		private string m_formattedText;

		private double? m_angle;

		private GaugeResizeModes? m_resizeMode;

		private ReportSize m_textShadowOffset;

		private bool? m_useFontPercent;

		public string Text
		{
			get
			{
				if (m_formattedText == null)
				{
					EnsureTextEvaluated();
					m_formattedText = RifGaugeLabel.FormatText(m_textResult.Value, OdpContext);
				}
				return m_formattedText;
			}
		}

		internal object OriginalValue
		{
			get
			{
				EnsureTextEvaluated();
				return m_textResult.Value.Value;
			}
		}

		internal TypeCode TypeCode
		{
			get
			{
				EnsureTextEvaluated();
				return m_textResult.Value.TypeCode;
			}
		}

		public double Angle
		{
			get
			{
				if (!m_angle.HasValue)
				{
					m_angle = RifGaugeLabel.EvaluateAngle(ReportScopeInstance, OdpContext);
				}
				return m_angle.Value;
			}
		}

		public GaugeResizeModes ResizeMode
		{
			get
			{
				if (!m_resizeMode.HasValue)
				{
					m_resizeMode = RifGaugeLabel.EvaluateResizeMode(ReportScopeInstance, OdpContext);
				}
				return m_resizeMode.Value;
			}
		}

		public ReportSize TextShadowOffset
		{
			get
			{
				if (m_textShadowOffset == null)
				{
					m_textShadowOffset = new ReportSize(RifGaugeLabel.EvaluateTextShadowOffset(ReportScopeInstance, OdpContext));
				}
				return m_textShadowOffset;
			}
		}

		public bool UseFontPercent
		{
			get
			{
				if (!m_useFontPercent.HasValue)
				{
					m_useFontPercent = RifGaugeLabel.EvaluateUseFontPercent(ReportScopeInstance, OdpContext);
				}
				return m_useFontPercent.Value;
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.GaugeLabel RifGaugeLabel => (Microsoft.ReportingServices.ReportIntermediateFormat.GaugeLabel)m_defObject.GaugePanelItemDef;

		private OnDemandProcessingContext OdpContext => m_defObject.GaugePanelDef.RenderingContext.OdpContext;

		internal GaugeLabelInstance(GaugeLabel defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		private void EnsureTextEvaluated()
		{
			if (!m_textResult.HasValue)
			{
				m_textResult = RifGaugeLabel.EvaluateText(ReportScopeInstance, OdpContext);
			}
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_textResult = null;
			m_formattedText = null;
			m_angle = null;
			m_resizeMode = null;
			m_textShadowOffset = null;
			m_useFontPercent = null;
		}
	}
}

using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class IndicatorImage : BaseGaugeImage
	{
		private ReportColorProperty m_hueColor;

		private ReportDoubleProperty m_transparency;

		public ReportColorProperty HueColor
		{
			get
			{
				if (m_hueColor == null && IndicatorImageDef.HueColor != null)
				{
					ExpressionInfo hueColor = IndicatorImageDef.HueColor;
					if (hueColor != null)
					{
						m_hueColor = new ReportColorProperty(hueColor.IsExpression, IndicatorImageDef.HueColor.OriginalText, hueColor.IsExpression ? null : new ReportColor(hueColor.StringValue.Trim(), allowTransparency: true), hueColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
					}
				}
				return m_hueColor;
			}
		}

		public ReportDoubleProperty Transparency
		{
			get
			{
				if (m_transparency == null && IndicatorImageDef.Transparency != null)
				{
					m_transparency = new ReportDoubleProperty(IndicatorImageDef.Transparency);
				}
				return m_transparency;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorImage IndicatorImageDef => (Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorImage)m_defObject;

		public new IndicatorImageInstance Instance => (IndicatorImageInstance)GetInstance();

		internal IndicatorImage(Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorImage defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
		}

		internal override BaseGaugeImageInstance GetInstance()
		{
			if (m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new IndicatorImageInstance(this);
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

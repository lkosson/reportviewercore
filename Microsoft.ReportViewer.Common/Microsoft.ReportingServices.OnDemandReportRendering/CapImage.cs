using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class CapImage : BaseGaugeImage
	{
		private ReportColorProperty m_hueColor;

		private ReportSizeProperty m_offsetX;

		private ReportSizeProperty m_offsetY;

		public ReportColorProperty HueColor
		{
			get
			{
				if (m_hueColor == null && CapImageDef.HueColor != null)
				{
					ExpressionInfo hueColor = CapImageDef.HueColor;
					if (hueColor != null)
					{
						m_hueColor = new ReportColorProperty(hueColor.IsExpression, hueColor.OriginalText, hueColor.IsExpression ? null : new ReportColor(hueColor.StringValue.Trim(), allowTransparency: true), hueColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
					}
				}
				return m_hueColor;
			}
		}

		public ReportSizeProperty OffsetX
		{
			get
			{
				if (m_offsetX == null && CapImageDef.OffsetX != null)
				{
					m_offsetX = new ReportSizeProperty(CapImageDef.OffsetX);
				}
				return m_offsetX;
			}
		}

		public ReportSizeProperty OffsetY
		{
			get
			{
				if (m_offsetY == null && CapImageDef.OffsetY != null)
				{
					m_offsetY = new ReportSizeProperty(CapImageDef.OffsetY);
				}
				return m_offsetY;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.CapImage CapImageDef => (Microsoft.ReportingServices.ReportIntermediateFormat.CapImage)m_defObject;

		public new CapImageInstance Instance => (CapImageInstance)GetInstance();

		internal CapImage(Microsoft.ReportingServices.ReportIntermediateFormat.CapImage defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
			m_defObject = defObject;
			m_gaugePanel = gaugePanel;
		}

		internal override BaseGaugeImageInstance GetInstance()
		{
			if (m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new CapImageInstance(this);
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

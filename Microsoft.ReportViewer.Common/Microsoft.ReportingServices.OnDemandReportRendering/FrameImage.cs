using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class FrameImage : BaseGaugeImage
	{
		private ReportColorProperty m_hueColor;

		private ReportDoubleProperty m_transparency;

		private ReportBoolProperty m_clipImage;

		public ReportColorProperty HueColor
		{
			get
			{
				if (m_hueColor == null && FrameImageDef.HueColor != null)
				{
					ExpressionInfo hueColor = FrameImageDef.HueColor;
					if (hueColor != null)
					{
						m_hueColor = new ReportColorProperty(hueColor.IsExpression, hueColor.OriginalText, hueColor.IsExpression ? null : new ReportColor(hueColor.StringValue.Trim(), allowTransparency: true), hueColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
					}
				}
				return m_hueColor;
			}
		}

		public ReportDoubleProperty Transparency
		{
			get
			{
				if (m_transparency == null && FrameImageDef.Transparency != null)
				{
					m_transparency = new ReportDoubleProperty(FrameImageDef.Transparency);
				}
				return m_transparency;
			}
		}

		public ReportBoolProperty ClipImage
		{
			get
			{
				if (m_clipImage == null && FrameImageDef.ClipImage != null)
				{
					m_clipImage = new ReportBoolProperty(FrameImageDef.ClipImage);
				}
				return m_clipImage;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.FrameImage FrameImageDef => (Microsoft.ReportingServices.ReportIntermediateFormat.FrameImage)m_defObject;

		public new FrameImageInstance Instance => (FrameImageInstance)GetInstance();

		internal FrameImage(Microsoft.ReportingServices.ReportIntermediateFormat.FrameImage defObject, GaugePanel gaugePanel)
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
				m_instance = new FrameImageInstance(this);
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

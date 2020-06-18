using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class PointerImage : BaseGaugeImage
	{
		private ReportColorProperty m_hueColor;

		private ReportDoubleProperty m_transparency;

		private ReportSizeProperty m_offsetX;

		private ReportSizeProperty m_offsetY;

		public ReportColorProperty HueColor
		{
			get
			{
				if (m_hueColor == null && PointerImageDef.HueColor != null)
				{
					ExpressionInfo hueColor = PointerImageDef.HueColor;
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
				if (m_transparency == null && PointerImageDef.Transparency != null)
				{
					m_transparency = new ReportDoubleProperty(PointerImageDef.Transparency);
				}
				return m_transparency;
			}
		}

		public ReportSizeProperty OffsetX
		{
			get
			{
				if (m_offsetX == null && PointerImageDef.OffsetX != null)
				{
					m_offsetX = new ReportSizeProperty(PointerImageDef.OffsetX);
				}
				return m_offsetX;
			}
		}

		public ReportSizeProperty OffsetY
		{
			get
			{
				if (m_offsetY == null && PointerImageDef.OffsetY != null)
				{
					m_offsetY = new ReportSizeProperty(PointerImageDef.OffsetY);
				}
				return m_offsetY;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.PointerImage PointerImageDef => (Microsoft.ReportingServices.ReportIntermediateFormat.PointerImage)m_defObject;

		public new PointerImageInstance Instance => (PointerImageInstance)GetInstance();

		internal PointerImage(Microsoft.ReportingServices.ReportIntermediateFormat.PointerImage defObject, GaugePanel gaugePanel)
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
				m_instance = new PointerImageInstance(this);
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

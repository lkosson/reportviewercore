using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class Gauge : GaugePanelItem
	{
		private BackFrame m_backFrame;

		private ReportBoolProperty m_clipContent;

		private TopImage m_topImage;

		private ReportDoubleProperty m_aspectRatio;

		public BackFrame BackFrame
		{
			get
			{
				if (m_backFrame == null && GaugeDef.BackFrame != null)
				{
					m_backFrame = new BackFrame(GaugeDef.BackFrame, m_gaugePanel);
				}
				return m_backFrame;
			}
		}

		public ReportBoolProperty ClipContent
		{
			get
			{
				if (m_clipContent == null && GaugeDef.ClipContent != null)
				{
					m_clipContent = new ReportBoolProperty(GaugeDef.ClipContent);
				}
				return m_clipContent;
			}
		}

		public TopImage TopImage
		{
			get
			{
				if (m_topImage == null && GaugeDef.TopImage != null)
				{
					m_topImage = new TopImage(GaugeDef.TopImage, m_gaugePanel);
				}
				return m_topImage;
			}
		}

		public ReportDoubleProperty AspectRatio
		{
			get
			{
				if (m_aspectRatio == null && GaugeDef.AspectRatio != null)
				{
					m_aspectRatio = new ReportDoubleProperty(GaugeDef.AspectRatio);
				}
				return m_aspectRatio;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.Gauge GaugeDef => (Microsoft.ReportingServices.ReportIntermediateFormat.Gauge)m_defObject;

		public new GaugeInstance Instance => (GaugeInstance)GetInstance();

		internal Gauge(Microsoft.ReportingServices.ReportIntermediateFormat.Gauge defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
			m_defObject = defObject;
			m_gaugePanel = gaugePanel;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_backFrame != null)
			{
				m_backFrame.SetNewContext();
			}
			if (m_topImage != null)
			{
				m_topImage.SetNewContext();
			}
		}
	}
}

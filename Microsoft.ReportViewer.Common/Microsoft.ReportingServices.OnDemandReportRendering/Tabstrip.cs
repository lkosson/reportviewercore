using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class Tabstrip : Navigation
	{
		private NavigationItem m_navigationItem;

		private Slider m_slider;

		public NavigationItem NavigationItem
		{
			get
			{
				if (m_navigationItem == null && RIFTabstrip.NavigationItem != null)
				{
					m_navigationItem = new NavigationItem(RIFTabstrip.NavigationItem);
				}
				return m_navigationItem;
			}
		}

		public Slider Slider
		{
			get
			{
				if (m_slider == null && RIFTabstrip.Slider != null)
				{
					m_slider = new Slider(RIFTabstrip.Slider);
				}
				return m_slider;
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Tabstrip RIFTabstrip => m_navigation as Microsoft.ReportingServices.ReportIntermediateFormat.Tabstrip;

		internal Tabstrip(Microsoft.ReportingServices.ReportIntermediateFormat.BandLayoutOptions bandLayout)
			: base(bandLayout)
		{
		}
	}
}

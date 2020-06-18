using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class PlayAxis : Navigation
	{
		public Slider Slider
		{
			get
			{
				if (RIFPlayAxis.Slider == null)
				{
					return null;
				}
				return new Slider(RIFPlayAxis.Slider);
			}
		}

		public DockingOption DockingOption => RIFPlayAxis.DockingOption;

		private Microsoft.ReportingServices.ReportIntermediateFormat.PlayAxis RIFPlayAxis => m_navigation as Microsoft.ReportingServices.ReportIntermediateFormat.PlayAxis;

		internal PlayAxis(Microsoft.ReportingServices.ReportIntermediateFormat.BandLayoutOptions bandLayout)
			: base(bandLayout)
		{
		}
	}
}

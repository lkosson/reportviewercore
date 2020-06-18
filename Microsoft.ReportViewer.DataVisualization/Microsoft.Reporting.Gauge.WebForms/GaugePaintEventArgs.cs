using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class GaugePaintEventArgs : EventArgs
	{
		private GaugeContainer gauge;

		private GaugeGraphics graphics;

		internal GaugeContainer Gauge => gauge;

		public GaugeGraphics Graphics => graphics;

		internal GaugePaintEventArgs(GaugeContainer gauge, GaugeGraphics graphics)
		{
			this.gauge = gauge;
			this.graphics = graphics;
		}
	}
}

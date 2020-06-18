using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class ImageSmoothingState
	{
		public GaugeGraphics g;

		private SmoothingMode oldSmoothingMode;

		private CompositingQuality compositingQuality;

		private InterpolationMode oldInterpolationMode;

		public ImageSmoothingState(GaugeGraphics g)
		{
			this.g = g;
		}

		public void Set()
		{
			oldSmoothingMode = g.Graphics.SmoothingMode;
			compositingQuality = g.Graphics.CompositingQuality;
			oldInterpolationMode = g.Graphics.InterpolationMode;
			g.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			g.Graphics.CompositingQuality = CompositingQuality.HighQuality;
			g.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
		}

		public void Restore()
		{
			g.Graphics.SmoothingMode = oldSmoothingMode;
			g.Graphics.CompositingQuality = compositingQuality;
			g.Graphics.InterpolationMode = oldInterpolationMode;
		}
	}
}

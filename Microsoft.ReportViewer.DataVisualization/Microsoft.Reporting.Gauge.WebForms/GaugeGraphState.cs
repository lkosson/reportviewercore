using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class GaugeGraphState
	{
		internal GraphicsState state;

		internal float width;

		internal float height;

		internal GaugeGraphState(GraphicsState state, float width, float height)
		{
			this.state = state;
			this.width = width;
			this.height = height;
		}
	}
}

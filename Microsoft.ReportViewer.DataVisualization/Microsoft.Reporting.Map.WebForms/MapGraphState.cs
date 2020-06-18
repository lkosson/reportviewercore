using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class MapGraphState
	{
		internal GraphicsState state;

		internal int width;

		internal int height;

		internal MapGraphState(GraphicsState state, int width, int height)
		{
			this.state = state;
			this.width = width;
			this.height = height;
		}
	}
}

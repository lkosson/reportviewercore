using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Map.WebForms
{
	internal struct GridLine
	{
		public GridType GridType;

		public PointF[] Points;

		public GraphicsPath Path;

		public RectangleF LabelRect;

		public double Coordinate;

		public PointF[] SelectionMarkerPositions;

		public void Dispose()
		{
			if (Path != null)
			{
				Path.Dispose();
			}
		}
	}
}

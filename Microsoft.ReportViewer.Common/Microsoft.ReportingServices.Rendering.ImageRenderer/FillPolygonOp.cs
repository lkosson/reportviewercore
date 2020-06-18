using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class FillPolygonOp : Operation
	{
		internal Color Color;

		internal PointF[] Polygon;

		internal FillPolygonOp(Color color, PointF[] polygon)
		{
			Color = color;
			Polygon = polygon;
		}

		internal override void Perform(WriterBase writer)
		{
			writer.FillPolygon(Color, Polygon);
		}
	}
}

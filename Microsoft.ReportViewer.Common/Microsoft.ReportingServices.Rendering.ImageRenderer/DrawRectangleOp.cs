using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class DrawRectangleOp : Operation
	{
		internal float Width;

		internal RPLFormat.BorderStyles Style;

		internal Color Color;

		internal RectangleF Rectangle;

		internal DrawRectangleOp(Color color, float width, RPLFormat.BorderStyles style, RectangleF rectangle)
		{
			Color = color;
			Width = width;
			Style = style;
			Rectangle = rectangle;
		}

		internal override void Perform(WriterBase writer)
		{
			writer.DrawRectangle(Color, Width, Style, Rectangle);
		}
	}
}

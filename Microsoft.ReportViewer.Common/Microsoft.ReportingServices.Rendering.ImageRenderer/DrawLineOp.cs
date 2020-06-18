using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class DrawLineOp : Operation
	{
		internal float Width;

		internal RPLFormat.BorderStyles Style;

		internal Color Color;

		internal float X1;

		internal float Y1;

		internal float X2;

		internal float Y2;

		internal DrawLineOp(Color color, float width, RPLFormat.BorderStyles style, float x1, float y1, float x2, float y2)
		{
			Color = color;
			Width = width;
			Style = style;
			X1 = x1;
			Y1 = y1;
			X2 = x2;
			Y2 = y2;
		}

		internal override void Perform(WriterBase writer)
		{
			writer.DrawLine(Color, Width, Style, X1, Y1, X2, Y2);
		}
	}
}

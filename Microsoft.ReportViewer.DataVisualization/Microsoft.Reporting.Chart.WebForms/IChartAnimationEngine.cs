using System.Drawing;
using System.Drawing.Imaging;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal interface IChartAnimationEngine
	{
		void DrawLineA(Pen pen, ColorA color, PointA point1, PointA point2);

		void DrawEllipseA(Pen pen, ColorA color, RectangleA rect);

		void DrawPolygonA(Pen pen, ColorA color, PointA[] points);

		void DrawRectangleA(Pen pen, ColorA color, RectangleA rect);

		void DrawStringA(string s, Font font, Brush brush, StringFormat format, ColorA colorA, RectangleA rectA);

		void DrawStringA(string s, Font font, Brush brush, StringFormat format, ColorA colorA, PointA pointA);

		void DrawImageA(Image image, RectangleA destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs);

		void FillRectangleA(Brush brush, ColorA color, ColorA secondColor, RectangleA rect);

		void FillEllipseA(Brush brush, ColorA color, ColorA secondColor, RectangleA rect);

		void FillPolygonA(Brush brush, ColorA color, ColorA secondColor, PointA[] points);
	}
}

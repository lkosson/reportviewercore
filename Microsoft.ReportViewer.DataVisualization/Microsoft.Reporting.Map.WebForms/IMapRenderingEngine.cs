using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace Microsoft.Reporting.Map.WebForms
{
	internal interface IMapRenderingEngine
	{
		Matrix Transform
		{
			get;
			set;
		}

		SmoothingMode SmoothingMode
		{
			get;
			set;
		}

		TextRenderingHint TextRenderingHint
		{
			get;
			set;
		}

		Region Clip
		{
			get;
			set;
		}

		Graphics Graphics
		{
			get;
			set;
		}

		bool IsClipEmpty
		{
			get;
		}

		void DrawLine(Pen pen, PointF pt1, PointF pt2);

		void DrawLine(Pen pen, float x1, float y1, float x2, float y2);

		void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr);

		void DrawEllipse(Pen pen, float x, float y, float width, float height);

		void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension);

		void DrawRectangle(Pen pen, int x, int y, int width, int height);

		void DrawPolygon(Pen pen, PointF[] points);

		void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format);

		void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format);

		void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs);

		void DrawRectangle(Pen pen, float x, float y, float width, float height);

		void DrawPath(Pen pen, GraphicsPath path);

		void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle);

		void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle);

		void DrawImage(Image image, RectangleF rect);

		void DrawEllipse(Pen pen, RectangleF rect);

		void DrawLines(Pen pen, PointF[] points);

		void FillEllipse(Brush brush, RectangleF rect);

		void FillPath(Brush brush, GraphicsPath path);

		void FillPath(Brush brush, GraphicsPath path, float angle, bool useBrushOffset, bool circularFill);

		void FillRegion(Brush brush, Region region);

		void FillRectangle(Brush brush, RectangleF rect);

		void FillRectangle(Brush brush, float x, float y, float width, float height);

		void FillPolygon(Brush brush, PointF[] points);

		void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle);

		SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat);

		SizeF MeasureString(string text, Font font);

		GraphicsState Save();

		void Restore(GraphicsState gstate);

		void ResetClip();

		void SetClip(RectangleF rect);

		void SetClip(GraphicsPath path, CombineMode combineMode);

		void TranslateTransform(float dx, float dy);

		void BeginSelection(string hRef, string title);

		void EndSelection();
	}
}

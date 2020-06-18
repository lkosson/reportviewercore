using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class RenderingEngine : IMapRenderingEngine
	{
		internal bool shadowDrawingMode;

		private RenderingType activeRenderingType;

		private GdiGraphics gdiGraphics = new GdiGraphics();

		private string documentTitle = string.Empty;

		internal IMapRenderingEngine RenderingObject
		{
			get
			{
				if (activeRenderingType == RenderingType.Gdi)
				{
					return gdiGraphics;
				}
				return null;
			}
		}

		internal RenderingType ActiveRenderingType
		{
			get
			{
				return activeRenderingType;
			}
			set
			{
				activeRenderingType = value;
			}
		}

		public TextRenderingHint TextRenderingHint
		{
			get
			{
				return RenderingObject.TextRenderingHint;
			}
			set
			{
				RenderingObject.TextRenderingHint = value;
			}
		}

		public Matrix Transform
		{
			get
			{
				return RenderingObject.Transform;
			}
			set
			{
				RenderingObject.Transform = value;
			}
		}

		public SmoothingMode SmoothingMode
		{
			get
			{
				return RenderingObject.SmoothingMode;
			}
			set
			{
				RenderingObject.SmoothingMode = value;
			}
		}

		public Region Clip
		{
			get
			{
				return RenderingObject.Clip;
			}
			set
			{
				RenderingObject.Clip = value;
			}
		}

		public bool IsClipEmpty => RenderingObject.IsClipEmpty;

		public Graphics Graphics
		{
			get
			{
				return RenderingObject.Graphics;
			}
			set
			{
				RenderingObject.Graphics = value;
			}
		}

		public void DrawLine(Pen pen, PointF pt1, PointF pt2)
		{
			RenderingObject.DrawLine(pen, pt1, pt2);
		}

		public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
		{
			RenderingObject.DrawLine(pen, x1, y1, x2, y2);
		}

		public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)
		{
			RenderingObject.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr);
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			RenderingObject.DrawEllipse(pen, x, y, width, height);
		}

		public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
		{
			RenderingObject.DrawCurve(pen, points, offset, numberOfSegments, tension);
		}

		public void DrawRectangle(Pen pen, int x, int y, int width, int height)
		{
			RenderingObject.DrawRectangle(pen, x, y, width, height);
		}

		public void DrawPolygon(Pen pen, PointF[] points)
		{
			RenderingObject.DrawPolygon(pen, points);
		}

		public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
		{
			RenderingObject.DrawString(s, font, brush, layoutRectangle, format);
		}

		public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
		{
			RenderingObject.DrawString(s, font, brush, point, format);
		}

		public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
		{
			RenderingObject.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs);
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			RenderingObject.DrawRectangle(pen, x, y, width, height);
		}

		public void DrawPath(Pen pen, GraphicsPath path)
		{
			RenderingObject.DrawPath(pen, path);
		}

		public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			RenderingObject.DrawPie(pen, x, y, width, height, startAngle, sweepAngle);
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			RenderingObject.DrawArc(pen, x, y, width, height, startAngle, sweepAngle);
		}

		public void DrawImage(Image image, RectangleF rect)
		{
			RenderingObject.DrawImage(image, rect);
		}

		public void DrawEllipse(Pen pen, RectangleF rect)
		{
			RenderingObject.DrawEllipse(pen, rect);
		}

		public void DrawLines(Pen pen, PointF[] points)
		{
			RenderingObject.DrawLines(pen, points);
		}

		public void FillEllipse(Brush brush, RectangleF rect)
		{
			RenderingObject.FillEllipse(brush, rect);
		}

		public void FillPath(Brush brush, GraphicsPath path)
		{
			RenderingObject.FillPath(brush, path);
		}

		public void FillPath(Brush brush, GraphicsPath path, float angle, bool useBrushOffset, bool circularFill)
		{
			RenderingObject.FillPath(brush, path, angle, useBrushOffset, circularFill);
		}

		public void FillRegion(Brush brush, Region region)
		{
			RenderingObject.FillRegion(brush, region);
		}

		public void FillRectangle(Brush brush, RectangleF rect)
		{
			RenderingObject.FillRectangle(brush, rect);
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			RenderingObject.FillRectangle(brush, x, y, width, height);
		}

		public void FillPolygon(Brush brush, PointF[] points)
		{
			RenderingObject.FillPolygon(brush, points);
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			RenderingObject.FillPie(brush, x, y, width, height, startAngle, sweepAngle);
		}

		public virtual void Close()
		{
		}

		public void StartHotRegion(object obj)
		{
			IImageMapProvider imageMapProvider = obj as IImageMapProvider;
			if (imageMapProvider != null)
			{
				RenderingObject.BeginSelection(imageMapProvider.GetHref(), imageMapProvider.GetToolTip());
			}
		}

		public void EndHotRegion()
		{
			RenderingObject.EndSelection();
		}

		public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
		{
			return RenderingObject.MeasureString(text, font, layoutArea, stringFormat);
		}

		public SizeF MeasureString(string text, Font font)
		{
			return RenderingObject.MeasureString(text, font);
		}

		public GraphicsState Save()
		{
			return RenderingObject.Save();
		}

		public void Restore(GraphicsState gstate)
		{
			RenderingObject.Restore(gstate);
		}

		public void ResetClip()
		{
			RenderingObject.ResetClip();
		}

		public void SetClip(RectangleF rect)
		{
			RenderingObject.SetClip(rect);
		}

		public void SetTitle(string title)
		{
			if (activeRenderingType == RenderingType.Svg)
			{
				documentTitle = title;
			}
		}

		public void SetClip(GraphicsPath path, CombineMode combineMode)
		{
			RenderingObject.SetClip(path, combineMode);
		}

		public void TranslateTransform(float dx, float dy)
		{
			RenderingObject.TranslateTransform(dx, dy);
		}

		public void BeginSelection(string hRef, string title)
		{
			RenderingObject.BeginSelection(hRef, title);
		}

		public void EndSelection()
		{
			RenderingObject.EndSelection();
		}
	}
}

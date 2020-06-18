using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace Microsoft.Reporting.Chart.WebForms.Svg
{
	internal class SvgChartGraphics : SvgRendering, IChartRenderingEngine
	{
		private Graphics graphics;

		public new Matrix Transform
		{
			get
			{
				return graphics.Transform;
			}
			set
			{
				chartMatrix = value;
				graphics.Transform = value;
			}
		}

		public SmoothingMode SmoothingMode
		{
			get
			{
				return graphics.SmoothingMode;
			}
			set
			{
				graphics.SmoothingMode = value;
				SetSmoothingMode(value == SmoothingMode.AntiAlias, shape: true);
			}
		}

		public bool IsClipEmpty => graphics.IsClipEmpty;

		public Region Clip
		{
			get
			{
				return graphics.Clip;
			}
			set
			{
				graphics.Clip = value;
			}
		}

		public Graphics Graphics
		{
			get
			{
				return graphics;
			}
			set
			{
				graphics = value;
			}
		}

		public TextRenderingHint TextRenderingHint
		{
			get
			{
				return graphics.TextRenderingHint;
			}
			set
			{
				graphics.TextRenderingHint = value;
				SetSmoothingMode(value == TextRenderingHint.AntiAlias || value == TextRenderingHint.SystemDefault || value == TextRenderingHint.ClearTypeGridFit, shape: false);
			}
		}

		public SvgChartGraphics(CommonElements common)
		{
		}

		public void DrawLine(Pen pen, PointF pt1, PointF pt2)
		{
			base.Pen = pen;
			DrawLine(pt1, pt2);
		}

		public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
		{
			DrawLine(pen, new PointF(x1, y1), new PointF(x2, y2));
		}

		public new void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)
		{
			base.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr);
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			base.Pen = pen;
			DrawEllipse(new RectangleF(x, y, width, height));
		}

		public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
		{
			base.Pen = pen;
			DrawCurve(points, offset, numberOfSegments, tension);
		}

		public void DrawRectangle(Pen pen, int x, int y, int width, int height)
		{
			DrawRectangle(pen, (float)x, (float)y, (float)width, (float)height);
		}

		public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
		{
			chartFont = font;
			base.Brush = brush;
			chartStringFormat = format;
			DrawString(s, layoutRectangle);
		}

		public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
		{
			chartFont = font;
			base.Brush = brush;
			if (format.LineAlignment != 0)
			{
				SizeF sizeF = MeasureString(s, font);
				sizeF.Height *= 0.8f;
				if (format.LineAlignment == StringAlignment.Center)
				{
					point.Y += sizeF.Height / 2f;
				}
				else if (format.LineAlignment == StringAlignment.Far)
				{
					point.Y += sizeF.Height;
				}
			}
			chartStringFormat = format;
			DrawString(s, point);
		}

		public new void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
		{
			base.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs);
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			base.Pen = pen;
			DrawRectangle(new RectangleF(x, y, width, height));
		}

		public void DrawPath(Pen pen, GraphicsPath path)
		{
			base.Pen = pen;
			DrawPath(path);
		}

		public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			base.Pen = pen;
			DrawPie(new RectangleF(x, y, width, height), startAngle, sweepAngle);
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			base.Pen = pen;
			DrawArc(new RectangleF(x, y, width, height), startAngle, sweepAngle);
		}

		public new void DrawImage(Image image, RectangleF rect)
		{
			base.DrawImage(image, rect);
		}

		public void DrawEllipse(Pen pen, RectangleF rect)
		{
			base.Pen = pen;
			DrawEllipse(rect);
		}

		public void DrawLines(Pen pen, PointF[] points)
		{
			base.Pen = pen;
			DrawLines(points);
		}

		public void FillEllipse(Brush brush, RectangleF rect)
		{
			base.Brush = brush;
			FillEllipse(rect);
		}

		public void FillPath(Brush brush, GraphicsPath path)
		{
			base.Brush = brush;
			FillPath(path);
		}

		public void FillRegion(Brush brush, Region region)
		{
		}

		public void FillRectangle(Brush brush, RectangleF rect)
		{
			base.Brush = brush;
			if (brush is TextureBrush)
			{
				FillTexturedRectangle((TextureBrush)brush, rect);
			}
			else
			{
				FillRectangle(rect);
			}
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			base.Brush = brush;
			if (brush is TextureBrush)
			{
				FillTexturedRectangle((TextureBrush)brush, new RectangleF(x, y, width, height));
			}
			else
			{
				FillRectangle(new RectangleF(x, y, width, height));
			}
		}

		public void FillPolygon(Brush brush, PointF[] points)
		{
			base.Brush = brush;
			FillPolygon(points);
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			base.Brush = brush;
			FillPie(new RectangleF(x, y, width, height), startAngle, sweepAngle);
		}

		public new void SetClip(RectangleF rect)
		{
			base.SetClip(rect);
			graphics.SetClip(rect);
		}

		public new void ResetClip()
		{
			base.ResetClip();
			graphics.ResetClip();
		}

		public void SetClip(GraphicsPath path, CombineMode combineMode)
		{
		}

		public void TranslateTransform(float dx, float dy)
		{
			graphics.TranslateTransform(dx, dy);
		}

		public void SetGradient(Color firstColor, Color secondColor, GradientType gradientType)
		{
			chartSvgGradientType = (SvgGradientType)Enum.Parse(typeof(SvgGradientType), gradientType.ToString());
			chartBrushColor = firstColor;
			chartBrushSecondColor = secondColor;
		}

		public void DrawPolygon(Pen pen, PointF[] points)
		{
			base.Pen = pen;
			DrawPolygon(points);
		}

		public GraphicsState Save()
		{
			return graphics.Save();
		}

		public void Restore(GraphicsState gstate)
		{
			graphics.Restore(gstate);
			Transform = graphics.Transform;
		}

		public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
		{
			return graphics.MeasureString(text, font, layoutArea, stringFormat);
		}

		public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted, out int linesFilled)
		{
			return graphics.MeasureString(text, font, layoutArea, stringFormat, out charactersFitted, out linesFilled);
		}

		public SizeF MeasureString(string text, Font font)
		{
			return graphics.MeasureString(text, font);
		}

		public void BeginSelection(string hRef, string title)
		{
			BeginSvgSelection(hRef, title);
		}

		public void EndSelection()
		{
			EndSvgSelection();
		}
	}
}

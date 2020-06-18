using Microsoft.Reporting.Chart.WebForms.Svg;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Xml;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class ChartRenderingEngine : IChartRenderingEngine
	{
		internal bool shadowDrawingMode;

		private RenderingType activeRenderingType;

		private SvgChartGraphics svgGraphics;

		private GdiGraphics gdiGraphics = new GdiGraphics();

		private string documentTitle = string.Empty;

		internal IChartRenderingEngine RenderingObject
		{
			get
			{
				if (activeRenderingType == RenderingType.Gdi)
				{
					return gdiGraphics;
				}
				return svgGraphics;
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
			ChartGraphics chartGraphics = this as ChartGraphics;
			if (chartGraphics == null || !chartGraphics.IsMetafile)
			{
				RenderingObject.DrawCurve(pen, points, offset, numberOfSegments, tension);
				return;
			}
			PointF[] array = null;
			if (offset == 0 && numberOfSegments == points.Length - 1)
			{
				RenderingObject.DrawCurve(pen, points, offset, numberOfSegments, tension);
				return;
			}
			if (offset == 0 && numberOfSegments < points.Length - 1)
			{
				array = new PointF[numberOfSegments + 2];
				for (int i = 0; i < numberOfSegments + 2; i++)
				{
					array[i] = points[i];
				}
			}
			else if (offset > 0 && offset + numberOfSegments == points.Length - 1)
			{
				array = new PointF[numberOfSegments + 2];
				for (int j = 0; j < numberOfSegments + 2; j++)
				{
					array[j] = points[offset + j - 1];
				}
				offset = 1;
			}
			else if (offset > 0 && offset + numberOfSegments < points.Length - 1)
			{
				array = new PointF[numberOfSegments + 3];
				for (int k = 0; k < numberOfSegments + 3; k++)
				{
					array[k] = points[offset + k - 1];
				}
				offset = 1;
			}
			RenderingObject.DrawCurve(pen, array, offset, numberOfSegments, tension);
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
			if (path != null && path.PointCount != 0)
			{
				RenderingObject.DrawPath(pen, path);
			}
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
			if (path != null && path.PointCount != 0)
			{
				RenderingObject.FillPath(brush, path);
			}
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

		public void Open(XmlTextWriter svgWriter, Size pictureSize, SvgOpenParameters extraParameters)
		{
			if (activeRenderingType == RenderingType.Svg)
			{
				svgGraphics = new SvgChartGraphics(((ChartGraphics)this).common);
				svgGraphics.SetTitle(documentTitle);
				svgGraphics.Open(svgWriter, pictureSize, extraParameters);
			}
		}

		public void SetGradient(Color firstColor, Color secondColor, GradientType gradientType)
		{
			if (activeRenderingType == RenderingType.Svg)
			{
				svgGraphics.SetGradient(firstColor, secondColor, gradientType);
			}
		}

		public void Close()
		{
			if (activeRenderingType == RenderingType.Svg)
			{
				svgGraphics.Close();
			}
		}

		internal void StartHotRegion(string href, string title)
		{
			RenderingObject.BeginSelection(href, title);
		}

		internal void StartHotRegion(DataPoint point)
		{
			StartHotRegion(point, labelRegion: false);
		}

		internal void StartHotRegion(DataPoint point, bool labelRegion)
		{
			string empty = string.Empty;
			string text = labelRegion ? point.LabelToolTip : point.ToolTip;
			empty = (labelRegion ? point.LabelHref : point.Href);
			if (empty.Length > 0 || text.Length > 0)
			{
				RenderingObject.BeginSelection(point.ReplaceKeywords(empty), point.ReplaceKeywords(text));
			}
		}

		internal void EndHotRegion()
		{
			RenderingObject.EndSelection();
		}

		public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
		{
			return RenderingObject.MeasureString(text, font, layoutArea, stringFormat);
		}

		public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted, out int linesFilled)
		{
			return RenderingObject.MeasureString(text, font, layoutArea, stringFormat, out charactersFitted, out linesFilled);
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

using Microsoft.ReportingServices.Rendering.ImageRenderer;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;

namespace Microsoft.Reporting.WinForms
{
	internal class GdiWriter : WriterBase
	{
		private System.Drawing.Graphics m_graphics;

		internal System.Drawing.Graphics Graphics
		{
			get
			{
				return m_graphics;
			}
			set
			{
				m_graphics = value;
			}
		}

		internal GdiWriter()
			: base(null, null, disposeRenderer: false, null)
		{
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		~GdiWriter()
		{
			Dispose(disposing: false);
		}

		internal override void DrawLine(Color color, float size, RPLFormat.BorderStyles style, float x1, float y1, float x2, float y2)
		{
			Pen pen = new Pen(color, size);
			pen.DashStyle = RenderingItem.TranslateBorderStyle(style);
			Graphics.DrawLine(pen, x1, y1, x2, y2);
		}

		internal override void DrawRectangle(Color color, float size, RPLFormat.BorderStyles style, RectangleF rectangle)
		{
			Pen pen = new Pen(color, size);
			pen.DashStyle = RenderingItem.TranslateBorderStyle(style);
			Graphics.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		internal override void FillPolygon(Color color, PointF[] polygon)
		{
			Brush brush = new SolidBrush(color);
			Graphics.FillPolygon(brush, polygon);
		}

		internal override float ConvertToMillimeters(int pixels)
		{
			return SharedRenderer.ConvertToMillimeters(pixels, 96f);
		}

		internal override int ConvertToPixels(float mm)
		{
			return SharedRenderer.ConvertToPixels(mm, 96f);
		}
	}
}

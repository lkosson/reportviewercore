using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Svg
{
	internal class ChartParameters : SvgParameters
	{
		internal Color chartBrushColor;

		internal Color chartBrushSecondColor;

		internal Matrix chartMatrix;

		internal Font chartFont;

		internal StringFormat chartStringFormat;

		internal SvgGradientType chartSvgGradientType;

		internal Size chartPictureSize;

		internal WrapMode imageWrapMode;

		private SvgFillType svgFillType;

		private Color chartPenColor;

		private float chartPenWidth;

		private SvgDashStyle chartDashStyle = SvgDashStyle.Solid;

		private FillMode tempFillMode;

		private Brush brush;

		private Pen pen;

		protected override Color BrushColor => chartBrushColor;

		public Brush Brush
		{
			get
			{
				return brush;
			}
			set
			{
				brush = value;
				SetBrush();
			}
		}

		public Pen Pen
		{
			get
			{
				return pen;
			}
			set
			{
				pen = value;
				SetPen();
			}
		}

		protected override Color BrushSecondColor => chartBrushSecondColor;

		protected override SvgGradientType GradientType => chartSvgGradientType;

		protected override SvgFillType FillType => svgFillType;

		protected override Color PenColor => chartPenColor;

		protected override double PenWidth => chartPenWidth;

		protected override SvgDashStyle DashStyle => chartDashStyle;

		protected override Matrix Transform => chartMatrix;

		protected override Font Font => chartFont;

		protected override StringFormat StringFormat => chartStringFormat;

		public override Size PictureSize
		{
			get
			{
				return chartPictureSize;
			}
			set
			{
				chartPictureSize = value;
			}
		}

		protected override FillMode FillMode
		{
			get
			{
				return tempFillMode;
			}
			set
			{
				tempFillMode = value;
			}
		}

		protected override SvgLineCapStyle SvgLineCap
		{
			get
			{
				if (pen.StartCap == LineCap.Flat || pen.StartCap == LineCap.NoAnchor)
				{
					return SvgLineCapStyle.Butt;
				}
				if (pen.StartCap == LineCap.Round || pen.StartCap == LineCap.RoundAnchor)
				{
					return SvgLineCapStyle.Round;
				}
				return SvgLineCapStyle.Square;
			}
		}

		protected override SvgImageWrapMode ImageWrapMode => (SvgImageWrapMode)imageWrapMode;

		protected override Color TextColor => chartBrushColor;

		protected string ToUSString(float number)
		{
			return number.ToString(CultureInfo.InvariantCulture);
		}

		protected string ToUSString(double number)
		{
			return number.ToString(CultureInfo.InvariantCulture);
		}

		protected override string GetX(double x)
		{
			return ToUSString(x);
		}

		protected override string GetY(double y)
		{
			return ToUSString(y);
		}

		protected override string GetX(PointF point)
		{
			return ToUSString(point.X);
		}

		protected override string GetX(RectangleF rectangle)
		{
			return ToUSString(rectangle.X);
		}

		protected override string GetWidth(RectangleF rectangle)
		{
			return ToUSString(rectangle.Width);
		}

		protected override string GetHeight(RectangleF rectangle)
		{
			return ToUSString(rectangle.Height);
		}

		protected override string GetY(PointF point)
		{
			return ToUSString(point.Y);
		}

		protected override string GetY(RectangleF rectangle)
		{
			return ToUSString(rectangle.Y);
		}

		private void SetBrush()
		{
			if (brush is SolidBrush)
			{
				chartBrushColor = ((SolidBrush)brush).Color;
				svgFillType = SvgFillType.Solid;
			}
			else if (brush is LinearGradientBrush)
			{
				chartBrushColor = ((LinearGradientBrush)brush).LinearColors[0];
				chartBrushSecondColor = ((LinearGradientBrush)brush).LinearColors[1];
				svgFillType = SvgFillType.Gradient;
			}
			else if (brush is HatchBrush)
			{
				chartBrushColor = ((HatchBrush)brush).BackgroundColor;
				svgFillType = SvgFillType.Solid;
			}
		}

		private void SetPen()
		{
			if (pen != null)
			{
				chartPenColor = pen.Color;
				chartPenWidth = pen.Width;
				switch (pen.DashStyle)
				{
				case System.Drawing.Drawing2D.DashStyle.Custom:
					chartDashStyle = SvgDashStyle.Custom;
					break;
				case System.Drawing.Drawing2D.DashStyle.Dash:
					chartDashStyle = SvgDashStyle.Dash;
					break;
				case System.Drawing.Drawing2D.DashStyle.DashDot:
					chartDashStyle = SvgDashStyle.DashDot;
					break;
				case System.Drawing.Drawing2D.DashStyle.DashDotDot:
					chartDashStyle = SvgDashStyle.DashDotDot;
					break;
				case System.Drawing.Drawing2D.DashStyle.Dot:
					chartDashStyle = SvgDashStyle.Dot;
					break;
				case System.Drawing.Drawing2D.DashStyle.Solid:
					chartDashStyle = SvgDashStyle.Solid;
					break;
				}
			}
		}
	}
}

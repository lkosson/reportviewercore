using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class MapParameters : SvgParameters
	{
		internal Color mapBrushColor;

		internal Color mapBrushSecondColor;

		internal Matrix mapMatrix;

		internal Font mapFont;

		internal StringFormat mapStringFormat;

		internal SvgGradientType mapSvgGradientType;

		internal Size mapPictureSize;

		internal WrapMode imageWrapMode;

		private SvgFillType svgFillType;

		private Color mapPenColor;

		private float mapPenWidth;

		private SvgDashStyle mapDashStyle = SvgDashStyle.Solid;

		private FillMode tempFillMode;

		private Brush brush;

		private Pen pen;

		protected override Color BrushColor => mapBrushColor;

		public Brush Brush
		{
			set
			{
				brush = value;
				SetBrush();
			}
		}

		public Pen Pen
		{
			set
			{
				pen = value;
				SetPen();
			}
		}

		protected override Color BrushSecondColor => mapBrushSecondColor;

		protected override SvgGradientType GradientType => mapSvgGradientType;

		protected override SvgFillType FillType => svgFillType;

		protected override Color PenColor => mapPenColor;

		protected override double PenWidth => mapPenWidth;

		protected override SvgDashStyle DashStyle => mapDashStyle;

		protected override Matrix Transform => mapMatrix;

		protected override Font Font => mapFont;

		protected override StringFormat StringFormat => mapStringFormat;

		public override Size PictureSize
		{
			get
			{
				return mapPictureSize;
			}
			set
			{
				mapPictureSize = value;
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

		protected override Color TextColor => mapBrushColor;

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
				mapBrushColor = ((SolidBrush)brush).Color;
				svgFillType = SvgFillType.Solid;
			}
			else if (brush is LinearGradientBrush)
			{
				mapBrushColor = ((LinearGradientBrush)brush).LinearColors[0];
				mapBrushSecondColor = ((LinearGradientBrush)brush).LinearColors[1];
				svgFillType = SvgFillType.Gradient;
			}
			else if (brush is HatchBrush)
			{
				mapBrushColor = ((HatchBrush)brush).BackgroundColor;
				svgFillType = SvgFillType.Solid;
			}
		}

		private void SetPen()
		{
			if (pen != null)
			{
				mapPenColor = pen.Color;
				mapPenWidth = pen.Width;
				switch (pen.DashStyle)
				{
				case System.Drawing.Drawing2D.DashStyle.Custom:
					mapDashStyle = SvgDashStyle.Custom;
					break;
				case System.Drawing.Drawing2D.DashStyle.Dash:
					mapDashStyle = SvgDashStyle.Dash;
					break;
				case System.Drawing.Drawing2D.DashStyle.DashDot:
					mapDashStyle = SvgDashStyle.DashDot;
					break;
				case System.Drawing.Drawing2D.DashStyle.DashDotDot:
					mapDashStyle = SvgDashStyle.DashDotDot;
					break;
				case System.Drawing.Drawing2D.DashStyle.Dot:
					mapDashStyle = SvgDashStyle.Dot;
					break;
				case System.Drawing.Drawing2D.DashStyle.Solid:
					mapDashStyle = SvgDashStyle.Solid;
					break;
				}
			}
		}
	}
}

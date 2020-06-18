using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeArrowAnnotation_ArrowAnnotation")]
	internal class ArrowAnnotation : Annotation
	{
		private ArrowStyle arrowStyle;

		private int arrowSize = 5;

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ArrowStyle.Simple)]
		[SRDescription("DescriptionAttributeArrowAnnotation_ArrowStyle")]
		[ParenthesizePropertyName(true)]
		public virtual ArrowStyle ArrowStyle
		{
			get
			{
				return arrowStyle;
			}
			set
			{
				arrowStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(5)]
		[SRDescription("DescriptionAttributeArrowAnnotation_ArrowSize")]
		[ParenthesizePropertyName(true)]
		public virtual int ArrowSize
		{
			get
			{
				return arrowSize;
			}
			set
			{
				if (arrowSize <= 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAnnotationArrowSizeIsZero);
				}
				if (arrowSize > 100)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAnnotationArrowSizeMustBeLessThen100);
				}
				arrowSize = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAnchor")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(typeof(ContentAlignment), "TopLeft")]
		[SRDescription("DescriptionAttributeAnchorAlignment")]
		public override ContentAlignment AnchorAlignment
		{
			get
			{
				return base.AnchorAlignment;
			}
			set
			{
				base.AnchorAlignment = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAnnotationType4")]
		public override string AnnotationType => "Arrow";

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(SelectionPointsStyle.Rectangle)]
		[ParenthesizePropertyName(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeSelectionPointsStyle3")]
		internal override SelectionPointsStyle SelectionPointsStyle => SelectionPointsStyle.TwoPoints;

		public ArrowAnnotation()
		{
			base.AnchorAlignment = ContentAlignment.TopLeft;
		}

		internal override void Paint(Chart chart, ChartGraphics graphics)
		{
			Chart = chart;
			PointF location = PointF.Empty;
			PointF anchorLocation = PointF.Empty;
			SizeF size = SizeF.Empty;
			GetRelativePosition(out location, out size, out anchorLocation);
			PointF pointF = new PointF(location.X + size.Width, location.Y + size.Height);
			RectangleF rectangleF = new RectangleF(location, new SizeF(pointF.X - location.X, pointF.Y - location.Y));
			if (!float.IsNaN(location.X) && !float.IsNaN(location.Y) && !float.IsNaN(pointF.X) && !float.IsNaN(pointF.Y))
			{
				GraphicsPath arrowPath = GetArrowPath(graphics, rectangleF);
				if (Chart.chartPicture.common.ProcessModePaint)
				{
					graphics.DrawPathAbs(arrowPath, BackColor.IsEmpty ? Color.White : BackColor, BackHatchStyle, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, BackGradientType, BackGradientEndColor, LineColor, LineWidth, LineStyle, PenAlignment.Center, ShadowOffset, ShadowColor);
				}
				if (Chart.chartPicture.common.ProcessModeRegions)
				{
					Chart.chartPicture.common.HotRegionsList.AddHotRegion(graphics, arrowPath, relativePath: false, ReplaceKeywords(ToolTip), ReplaceKeywords(Href), ReplaceKeywords(MapAreaAttributes), this, ChartElementType.Annotation);
				}
				PaintSelectionHandles(graphics, rectangleF, null);
			}
		}

		private GraphicsPath GetArrowPath(ChartGraphics graphics, RectangleF position)
		{
			RectangleF absoluteRectangle = graphics.GetAbsoluteRectangle(position);
			PointF location = absoluteRectangle.Location;
			PointF pointF = new PointF(absoluteRectangle.Right, absoluteRectangle.Bottom);
			float num = pointF.X - location.X;
			float num2 = pointF.Y - location.Y;
			float num3 = (float)Math.Sqrt(num * num + num2 * num2);
			GraphicsPath graphicsPath = new GraphicsPath();
			PointF[] array = null;
			float num4 = 2.1f;
			if (ArrowStyle == ArrowStyle.Simple)
			{
				array = new PointF[7]
				{
					location,
					new PointF(location.X + (float)ArrowSize * num4, location.Y - (float)ArrowSize * num4),
					new PointF(location.X + (float)ArrowSize * num4, location.Y - (float)ArrowSize),
					new PointF(location.X + num3, location.Y - (float)ArrowSize),
					new PointF(location.X + num3, location.Y + (float)ArrowSize),
					new PointF(location.X + (float)ArrowSize * num4, location.Y + (float)ArrowSize),
					new PointF(location.X + (float)ArrowSize * num4, location.Y + (float)ArrowSize * num4)
				};
			}
			else if (ArrowStyle == ArrowStyle.DoubleArrow)
			{
				array = new PointF[10]
				{
					location,
					new PointF(location.X + (float)ArrowSize * num4, location.Y - (float)ArrowSize * num4),
					new PointF(location.X + (float)ArrowSize * num4, location.Y - (float)ArrowSize),
					new PointF(location.X + num3 - (float)ArrowSize * num4, location.Y - (float)ArrowSize),
					new PointF(location.X + num3 - (float)ArrowSize * num4, location.Y - (float)ArrowSize * num4),
					new PointF(location.X + num3, location.Y),
					new PointF(location.X + num3 - (float)ArrowSize * num4, location.Y + (float)ArrowSize * num4),
					new PointF(location.X + num3 - (float)ArrowSize * num4, location.Y + (float)ArrowSize),
					new PointF(location.X + (float)ArrowSize * num4, location.Y + (float)ArrowSize),
					new PointF(location.X + (float)ArrowSize * num4, location.Y + (float)ArrowSize * num4)
				};
			}
			else
			{
				if (ArrowStyle != ArrowStyle.Tailed)
				{
					throw new InvalidOperationException(SR.ExceptionAnnotationArrowStyleUnknown);
				}
				float num5 = 2.1f;
				array = new PointF[8]
				{
					location,
					new PointF(location.X + (float)ArrowSize * num4, location.Y - (float)ArrowSize * num4),
					new PointF(location.X + (float)ArrowSize * num4, location.Y - (float)ArrowSize),
					new PointF(location.X + num3, location.Y - (float)ArrowSize * num5),
					new PointF(location.X + num3 - (float)ArrowSize * num5, location.Y),
					new PointF(location.X + num3, location.Y + (float)ArrowSize * num5),
					new PointF(location.X + (float)ArrowSize * num4, location.Y + (float)ArrowSize),
					new PointF(location.X + (float)ArrowSize * num4, location.Y + (float)ArrowSize * num4)
				};
			}
			graphicsPath.AddLines(array);
			graphicsPath.CloseAllFigures();
			float num6 = (float)(Math.Atan(num2 / num) * 180.0 / Math.PI);
			if (num < 0f)
			{
				num6 += 180f;
			}
			using (Matrix matrix = new Matrix())
			{
				matrix.RotateAt(num6, location);
				graphicsPath.Transform(matrix);
				return graphicsPath;
			}
		}
	}
}

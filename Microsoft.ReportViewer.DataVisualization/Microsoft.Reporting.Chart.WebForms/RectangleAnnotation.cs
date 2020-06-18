using Microsoft.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeRectangleAnnotation_RectangleAnnotation")]
	internal class RectangleAnnotation : TextAnnotation
	{
		internal bool isRectVisible = true;

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLineColor")]
		public override Color LineColor
		{
			get
			{
				return base.LineColor;
			}
			set
			{
				base.LineColor = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeLineWidth")]
		public override int LineWidth
		{
			get
			{
				return base.LineWidth;
			}
			set
			{
				base.LineWidth = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(true)]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeLineStyle4")]
		public override ChartDashStyle LineStyle
		{
			get
			{
				return base.LineStyle;
			}
			set
			{
				base.LineStyle = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeBackColor8")]
		[NotifyParentProperty(true)]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(true)]
		[DefaultValue(ChartHatchStyle.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackHatchStyle5")]
		public override ChartHatchStyle BackHatchStyle
		{
			get
			{
				return base.BackHatchStyle;
			}
			set
			{
				base.BackHatchStyle = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(true)]
		[DefaultValue(GradientType.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackGradientType12")]
		public override GradientType BackGradientType
		{
			get
			{
				return base.BackGradientType;
			}
			set
			{
				base.BackGradientType = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(true)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackGradientEndColor8")]
		public override Color BackGradientEndColor
		{
			get
			{
				return base.BackGradientEndColor;
			}
			set
			{
				base.BackGradientEndColor = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAnnotationType4")]
		public override string AnnotationType => "Rectangle";

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(SelectionPointsStyle.Rectangle)]
		[ParenthesizePropertyName(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeSelectionPointsStyle3")]
		internal override SelectionPointsStyle SelectionPointsStyle => SelectionPointsStyle.Rectangle;

		internal override void Paint(Chart chart, ChartGraphics graphics)
		{
			Chart = chart;
			PointF location = PointF.Empty;
			PointF anchorLocation = PointF.Empty;
			SizeF size = SizeF.Empty;
			GetRelativePosition(out location, out size, out anchorLocation);
			PointF pointF = new PointF(location.X + size.Width, location.Y + size.Height);
			RectangleF rectangleF = new RectangleF(location, new SizeF(pointF.X - location.X, pointF.Y - location.Y));
			RectangleF rectF = new RectangleF(rectangleF.Location, rectangleF.Size);
			if (rectF.Width < 0f)
			{
				rectF.X = rectF.Right;
				rectF.Width = 0f - rectF.Width;
			}
			if (rectF.Height < 0f)
			{
				rectF.Y = rectF.Bottom;
				rectF.Height = 0f - rectF.Height;
			}
			if (!float.IsNaN(rectF.X) && !float.IsNaN(rectF.Y) && !float.IsNaN(rectF.Right) && !float.IsNaN(rectF.Bottom))
			{
				if (isRectVisible && Chart.chartPicture.common.ProcessModePaint)
				{
					graphics.FillRectangleRel(rectF, BackColor, BackHatchStyle, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, BackGradientType, BackGradientEndColor, LineColor, LineWidth, LineStyle, ShadowColor, ShadowOffset, PenAlignment.Center, isEllipse, 1, circle3D: false);
				}
				base.Paint(chart, graphics);
			}
		}
	}
}

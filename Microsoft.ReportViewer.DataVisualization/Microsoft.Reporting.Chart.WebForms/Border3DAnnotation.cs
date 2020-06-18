using Microsoft.Reporting.Chart.WebForms.Borders3D;
using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeBorder3DAnnotation_Border3DAnnotation")]
	internal class Border3DAnnotation : RectangleAnnotation
	{
		private BorderSkinAttributes borderSkin = new BorderSkinAttributes();

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAnnotationType4")]
		public override string AnnotationType => "Border3D";

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeBorderSkinAttributes")]
		[NotifyParentProperty(true)]
		[TypeConverter(typeof(LegendConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BorderSkinAttributes BorderSkin
		{
			get
			{
				return borderSkin;
			}
			set
			{
				borderSkin = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Browsable(false)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeChart")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		internal override Chart Chart
		{
			get
			{
				return base.Chart;
			}
			set
			{
				base.Chart = value;
				if (value != null)
				{
					borderSkin.serviceContainer = value.serviceContainer;
				}
			}
		}

		public Border3DAnnotation()
		{
			isRectVisible = false;
			borderSkin.PageColor = Color.Transparent;
			borderSkin.SkinStyle = BorderSkinStyle.Raised;
			lineColor = Color.Empty;
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
			RectangleF rectangleF2 = new RectangleF(rectangleF.Location, rectangleF.Size);
			if (rectangleF2.Width < 0f)
			{
				rectangleF2.X = rectangleF2.Right;
				rectangleF2.Width = 0f - rectangleF2.Width;
			}
			if (rectangleF2.Height < 0f)
			{
				rectangleF2.Y = rectangleF2.Bottom;
				rectangleF2.Height = 0f - rectangleF2.Height;
			}
			if (float.IsNaN(rectangleF2.X) || float.IsNaN(rectangleF2.Y) || float.IsNaN(rectangleF2.Right) || float.IsNaN(rectangleF2.Bottom))
			{
				return;
			}
			if (Chart.chartPicture.common.ProcessModePaint)
			{
				RectangleF absoluteRectangle = graphics.GetAbsoluteRectangle(rectangleF2);
				if (absoluteRectangle.Width > 30f && absoluteRectangle.Height > 30f)
				{
					graphics.Draw3DBorderRel(borderSkin, rectangleF2, BackColor, BackHatchStyle, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, BackGradientType, BackGradientEndColor, LineColor, LineWidth, LineStyle);
				}
			}
			base.Paint(chart, graphics);
		}

		internal override RectangleF GetTextSpacing(out bool annotationRelative)
		{
			annotationRelative = false;
			RectangleF rectangleF = new RectangleF(3f, 3f, 3f, 3f);
			if (GetGraphics() != null)
			{
				rectangleF = GetGraphics().GetRelativeRectangle(rectangleF);
			}
			if (borderSkin.SkinStyle != 0 && GetGraphics() != null && Chart != null && Chart.chartPicture != null && Chart.chartPicture.common != null)
			{
				IBorderType borderType = Chart.chartPicture.common.BorderTypeRegistry.GetBorderType(borderSkin.SkinStyle.ToString());
				if (borderType != null)
				{
					RectangleF areasRect = new RectangleF(0f, 0f, 100f, 100f);
					borderType.AdjustAreasPosition(GetGraphics(), ref areasRect);
					rectangleF = new RectangleF(areasRect.X + 1f, areasRect.Y + 1f, 100f - areasRect.Right + 2f, 100f - areasRect.Bottom + 2f);
				}
			}
			return rectangleF;
		}
	}
}

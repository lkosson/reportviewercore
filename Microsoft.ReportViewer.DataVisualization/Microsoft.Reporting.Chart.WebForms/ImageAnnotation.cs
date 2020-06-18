using Microsoft.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeImageAnnotation_ImageAnnotation")]
	internal class ImageAnnotation : Annotation
	{
		private string imageName = string.Empty;

		private ChartImageWrapMode imageMode = ChartImageWrapMode.Scaled;

		private Color imageTransparentColor = Color.Empty;

		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeImageAnnotation_Image")]
		public virtual string Image
		{
			get
			{
				return imageName;
			}
			set
			{
				imageName = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[DefaultValue(ChartImageWrapMode.Scaled)]
		[SRDescription("DescriptionAttributeImageAnnotation_ImageMode")]
		public ChartImageWrapMode ImageMode
		{
			get
			{
				return imageMode;
			}
			set
			{
				imageMode = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeImageAnnotation_ImageTransparentColor")]
		public Color ImageTransparentColor
		{
			get
			{
				return imageTransparentColor;
			}
			set
			{
				imageTransparentColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeImage")]
		[DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
		[SRDescription("DescriptionAttributeImageAnnotation_Alignment")]
		public override ContentAlignment Alignment
		{
			get
			{
				return base.Alignment;
			}
			set
			{
				base.Alignment = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAnnotationType4")]
		public override string AnnotationType => "Image";

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(SelectionPointsStyle.Rectangle)]
		[ParenthesizePropertyName(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeSelectionPointsStyle3")]
		internal override SelectionPointsStyle SelectionPointsStyle => SelectionPointsStyle.Rectangle;

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeImageAnnotation_LineWidth")]
		public override Color TextColor
		{
			get
			{
				return base.TextColor;
			}
			set
			{
				base.TextColor = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeImageAnnotation_LineWidth")]
		public override Font TextFont
		{
			get
			{
				return base.TextFont;
			}
			set
			{
				base.TextFont = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(TextStyle), "Default")]
		[SRDescription("DescriptionAttributeImageAnnotation_LineWidth")]
		public override TextStyle TextStyle
		{
			get
			{
				return base.TextStyle;
			}
			set
			{
				base.TextStyle = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeImageAnnotation_LineWidth")]
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
		[Browsable(false)]
		[DefaultValue(ChartHatchStyle.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeImageAnnotation_LineWidth")]
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
		[Browsable(false)]
		[DefaultValue(GradientType.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeImageAnnotation_LineWidth")]
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
		[Browsable(false)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeImageAnnotation_LineWidth")]
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

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeImageAnnotation_LineWidth")]
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
		[DefaultValue(1)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeImageAnnotation_LineWidth")]
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
		[Browsable(false)]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeLineStyle6")]
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

		internal override void Paint(Chart chart, ChartGraphics graphics)
		{
			Chart = chart;
			PointF location = PointF.Empty;
			PointF anchorLocation = PointF.Empty;
			SizeF size = SizeF.Empty;
			GetRelativePosition(out location, out size, out anchorLocation);
			PointF pointF = new PointF(location.X + size.Width, location.Y + size.Height);
			RectangleF rect = new RectangleF(location, new SizeF(pointF.X - location.X, pointF.Y - location.Y));
			RectangleF rectangleF = new RectangleF(rect.Location, rect.Size);
			if (rectangleF.Width < 0f)
			{
				rectangleF.X = rectangleF.Right;
				rectangleF.Width = 0f - rectangleF.Width;
			}
			if (rectangleF.Height < 0f)
			{
				rectangleF.Y = rectangleF.Bottom;
				rectangleF.Height = 0f - rectangleF.Height;
			}
			if (float.IsNaN(rectangleF.X) || float.IsNaN(rectangleF.Y) || float.IsNaN(rectangleF.Right) || float.IsNaN(rectangleF.Bottom))
			{
				return;
			}
			if (Chart.chartPicture.common.ProcessModePaint)
			{
				if (imageName.Length == 0 && Chart.IsDesignMode())
				{
					graphics.FillRectangleRel(rectangleF, BackColor, BackHatchStyle, imageName, imageMode, imageTransparentColor, GetImageAlignment(Alignment), BackGradientType, BackGradientEndColor, LineColor, LineWidth, LineStyle, ShadowColor, ShadowOffset, PenAlignment.Center);
					using (Brush brush = new SolidBrush(TextColor))
					{
						StringFormat stringFormat = new StringFormat(StringFormat.GenericTypographic);
						stringFormat.Alignment = StringAlignment.Center;
						stringFormat.LineAlignment = StringAlignment.Center;
						stringFormat.FormatFlags = StringFormatFlags.LineLimit;
						stringFormat.Trimming = StringTrimming.EllipsisCharacter;
						graphics.DrawStringRel("(no image)", TextFont, brush, rectangleF, stringFormat);
					}
				}
				else
				{
					graphics.FillRectangleRel(rectangleF, Color.Transparent, BackHatchStyle, imageName, imageMode, imageTransparentColor, GetImageAlignment(Alignment), BackGradientType, Color.Transparent, Color.Transparent, 0, LineStyle, ShadowColor, ShadowOffset, PenAlignment.Center);
				}
			}
			if (Chart.chartPicture.common.ProcessModeRegions)
			{
				Chart.chartPicture.common.HotRegionsList.AddHotRegion(graphics, rectangleF, ReplaceKeywords(ToolTip), ReplaceKeywords(Href), ReplaceKeywords(MapAreaAttributes), this, ChartElementType.Annotation, string.Empty);
			}
			PaintSelectionHandles(graphics, rect, null);
		}

		private ChartImageAlign GetImageAlignment(ContentAlignment alignment)
		{
			switch (alignment)
			{
			case ContentAlignment.TopLeft:
				return ChartImageAlign.TopLeft;
			case ContentAlignment.TopCenter:
				return ChartImageAlign.Top;
			case ContentAlignment.TopRight:
				return ChartImageAlign.TopRight;
			case ContentAlignment.MiddleRight:
				return ChartImageAlign.Right;
			case ContentAlignment.BottomRight:
				return ChartImageAlign.BottomRight;
			case ContentAlignment.BottomCenter:
				return ChartImageAlign.Bottom;
			case ContentAlignment.BottomLeft:
				return ChartImageAlign.BottomLeft;
			case ContentAlignment.MiddleLeft:
				return ChartImageAlign.Left;
			default:
				return ChartImageAlign.Center;
			}
		}

		internal override RectangleF GetContentPosition()
		{
			if (Image.Length > 0)
			{
				try
				{
					if (Chart != null)
					{
						ImageLoader imageLoader = (ImageLoader)Chart.serviceContainer.GetService(typeof(ImageLoader));
						if (imageLoader != null)
						{
							ChartGraphics graphics = GetGraphics();
							if (graphics != null)
							{
								SizeF size = default(SizeF);
								if (imageLoader.GetAdjustedImageSize(Image, graphics.Graphics, ref size))
								{
									SizeF relativeSize = graphics.GetRelativeSize(size);
									return new RectangleF(float.NaN, float.NaN, relativeSize.Width, relativeSize.Height);
								}
							}
						}
					}
				}
				catch
				{
				}
			}
			return new RectangleF(float.NaN, float.NaN, float.NaN, float.NaN);
		}
	}
}

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(MapImageConverter))]
	internal class MapImage : DockablePanel, IToolTipProvider, IImageMapProvider
	{
		private ResizeMode resizeMode = ResizeMode.AutoFit;

		private string image = "";

		private Color imageTransColor = Color.Empty;

		private float shadowOffset = 1f;

		private float angle;

		private float transparency;

		private object mapAreaTag;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapImage_ToolTip")]
		[Localizable(true)]
		[DefaultValue("")]
		public override string ToolTip
		{
			get
			{
				return base.ToolTip;
			}
			set
			{
				base.ToolTip = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapImage_Href")]
		[Localizable(true)]
		[DefaultValue("")]
		public sealed override string Href
		{
			get
			{
				return base.Href;
			}
			set
			{
				base.Href = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapImage_MapAreaAttributes")]
		[DefaultValue("")]
		public override string MapAreaAttributes
		{
			get
			{
				return base.MapAreaAttributes;
			}
			set
			{
				base.MapAreaAttributes = value;
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeMapImage_Name")]
		[Browsable(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[SerializationVisibility(SerializationVisibility.Attribute)]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapImage_Visible")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(true)]
		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				base.Visible = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeMapImage_ResizeMode")]
		[DefaultValue(ResizeMode.AutoFit)]
		public ResizeMode ResizeMode
		{
			get
			{
				return resizeMode;
			}
			set
			{
				resizeMode = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeMapImage_Image")]
		[DefaultValue("")]
		public string Image
		{
			get
			{
				return image;
			}
			set
			{
				image = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeMapImage_ImageTransColor")]
		[DefaultValue(typeof(Color), "")]
		public Color ImageTransColor
		{
			get
			{
				return imageTransColor;
			}
			set
			{
				imageTransColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapImage_ShadowOffset")]
		[NotifyParentProperty(true)]
		[DefaultValue(1f)]
		public float ShadowOffset
		{
			get
			{
				return shadowOffset;
			}
			set
			{
				if (value < -100f || value > 100f)
				{
					throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
				}
				shadowOffset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Position")]
		[SRDescription("DescriptionAttributeMapImage_Angle")]
		[DefaultValue(0f)]
		public float Angle
		{
			get
			{
				return angle;
			}
			set
			{
				if (value > 360f || value < 0f)
				{
					throw new ArgumentOutOfRangeException(SR.out_of_range(0.0, 360.0));
				}
				angle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeMapImage_Transparency")]
		[DefaultValue(0f)]
		public float Transparency
		{
			get
			{
				return transparency;
			}
			set
			{
				if (value > 100f || value < 0f)
				{
					throw new ArgumentOutOfRangeException(SR.out_of_range(0.0, 100.0));
				}
				transparency = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapImage_BorderStyle")]
		[NotifyParentProperty(true)]
		[DefaultValue(MapDashStyle.Solid)]
		public override MapDashStyle BorderStyle
		{
			get
			{
				return base.BorderStyle;
			}
			set
			{
				base.BorderStyle = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
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

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
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

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public override Color BackSecondaryColor
		{
			get
			{
				return base.BackSecondaryColor;
			}
			set
			{
				base.BackSecondaryColor = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public override MapHatchStyle BackHatchStyle
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

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public override int BackShadowOffset
		{
			get
			{
				return base.BackShadowOffset;
			}
			set
			{
				base.BackShadowOffset = value;
			}
		}

		internal Position Position => new Position(Location, Size, ContentAlignment.TopLeft);

		object IImageMapProvider.Tag
		{
			get
			{
				return mapAreaTag;
			}
			set
			{
				mapAreaTag = value;
			}
		}

		public MapImage()
			: this(null)
		{
		}

		internal MapImage(CommonElements common)
			: base(common)
		{
			Location = new MapLocation(this, 20f, 20f);
			Location.DefaultValues = true;
			Size.DefaultValues = true;
			BorderStyle = MapDashStyle.Solid;
			Visible = true;
		}

		public override string ToString()
		{
			return Name;
		}

		internal override bool ShouldRenderBackground()
		{
			return false;
		}

		internal override void Render(MapGraphics g)
		{
			g.StartHotRegion(this);
			MapDashStyle mapDashStyle = BorderStyle;
			if (!string.IsNullOrEmpty(Image))
			{
				ImageSmoothingState imageSmoothingState = new ImageSmoothingState(g);
				imageSmoothingState.Set();
				DrawImage(g, image, drawShadow: true);
				DrawImage(g, image, drawShadow: false);
				imageSmoothingState.Restore();
			}
			else
			{
				string text = "No image.";
				Font font = new Font("Microsoft Sans Serif", 8.25f);
				SizeF sizeF = g.MeasureString(text, font);
				StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;
				RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
				PointF absolutePoint = g.GetAbsolutePoint(new PointF(50f, 50f));
				new RectangleF(absolutePoint.X, absolutePoint.Y, 0f, 0f).Inflate(sizeF.Width / 2f, sizeF.Height / 2f);
				using (Brush brush = new SolidBrush(Color.Gray))
				{
					g.DrawString(text, font, brush, absoluteRectangle, stringFormat);
				}
				mapDashStyle = MapDashStyle.Solid;
			}
			if (mapDashStyle != 0 && BorderColor != Color.Transparent && BorderWidth != 0)
			{
				using (GraphicsPath path = GetPath(g))
				{
					using (Pen pen = GetPen())
					{
						AntiAliasing antiAliasing = g.AntiAliasing;
						if (Angle % 90f == 0f)
						{
							g.AntiAliasing = AntiAliasing.None;
						}
						g.DrawPath(pen, path);
						g.AntiAliasing = antiAliasing;
					}
				}
			}
			g.EndHotRegion();
		}

		internal void DrawImage(MapGraphics g, string imageName, bool drawShadow)
		{
			if (drawShadow && ShadowOffset == 0f)
			{
				return;
			}
			Image image = Common.ImageLoader.LoadImage(imageName);
			if (image.Width == 0 || image.Height == 0)
			{
				return;
			}
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			Rectangle empty = Rectangle.Empty;
			if (ResizeMode == ResizeMode.AutoFit)
			{
				empty = new Rectangle((int)absoluteRectangle.X, (int)absoluteRectangle.Y, (int)absoluteRectangle.Width, (int)absoluteRectangle.Height);
			}
			else
			{
				empty = new Rectangle(0, 0, image.Width, image.Height);
				PointF absolutePoint = g.GetAbsolutePoint(new PointF(50f, 50f));
				empty.X = (int)(absolutePoint.X - (float)(empty.Size.Width / 2));
				empty.Y = (int)(absolutePoint.Y - (float)(empty.Size.Height / 2));
			}
			ImageAttributes imageAttributes = new ImageAttributes();
			if (ImageTransColor != Color.Empty)
			{
				imageAttributes.SetColorKey(ImageTransColor, ImageTransColor, ColorAdjustType.Default);
			}
			float num = (100f - Transparency) / 100f;
			float num2 = Common.MapCore.ShadowIntensity / 100f;
			if (drawShadow)
			{
				ColorMatrix colorMatrix = new ColorMatrix();
				colorMatrix.Matrix00 = 0f;
				colorMatrix.Matrix11 = 0f;
				colorMatrix.Matrix22 = 0f;
				colorMatrix.Matrix33 = num2 * num;
				imageAttributes.SetColorMatrix(colorMatrix);
			}
			else if (Transparency > 0f)
			{
				ColorMatrix colorMatrix2 = new ColorMatrix();
				colorMatrix2.Matrix33 = num;
				imageAttributes.SetColorMatrix(colorMatrix2);
			}
			if (Angle != 0f)
			{
				PointF point = new PointF(absoluteRectangle.X + absoluteRectangle.Width / 2f, absoluteRectangle.Y + absoluteRectangle.Height / 2f);
				Matrix transform = g.Transform;
				Matrix matrix = g.Transform.Clone();
				float offsetX = matrix.OffsetX;
				float offsetY = matrix.OffsetY;
				point.X += offsetX;
				point.Y += offsetY;
				matrix.RotateAt(Angle, point, MatrixOrder.Append);
				if (drawShadow)
				{
					matrix.Translate(ShadowOffset, ShadowOffset, MatrixOrder.Append);
				}
				g.Transform = matrix;
				g.DrawImage(image, empty, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				g.Transform = transform;
			}
			else
			{
				if (drawShadow)
				{
					empty.X += (int)ShadowOffset;
					empty.Y += (int)ShadowOffset;
				}
				g.DrawImage(image, empty, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
			}
			if (drawShadow)
			{
				return;
			}
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				graphicsPath.AddRectangle(empty);
				if (Angle != 0f)
				{
					PointF point2 = new PointF(absoluteRectangle.X + absoluteRectangle.Width / 2f, absoluteRectangle.Y + absoluteRectangle.Height / 2f);
					using (Matrix matrix2 = new Matrix())
					{
						matrix2.RotateAt(Angle, point2, MatrixOrder.Append);
						graphicsPath.Transform(matrix2);
					}
				}
				Common.MapCore.HotRegionList.SetHotRegion(g, this, graphicsPath);
			}
		}

		internal GraphicsPath GetPath(MapGraphics g)
		{
			if (!IsVisible())
			{
				return null;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			graphicsPath.AddRectangle(absoluteRectangle);
			if (Angle != 0f)
			{
				PointF point = new PointF(absoluteRectangle.X + absoluteRectangle.Width / 2f, absoluteRectangle.Y + absoluteRectangle.Height / 2f);
				using (Matrix matrix = new Matrix())
				{
					matrix.RotateAt(Angle, point);
					graphicsPath.Transform(matrix);
					return graphicsPath;
				}
			}
			return graphicsPath;
		}

		internal Pen GetPen()
		{
			if (BorderWidth <= 0)
			{
				return null;
			}
			_ = BorderColor;
			_ = BorderWidth;
			_ = BorderStyle;
			return new Pen(BorderColor, BorderWidth)
			{
				DashStyle = MapGraphics.GetPenStyle(BorderStyle),
				Alignment = PenAlignment.Center
			};
		}

		internal override object GetDefaultPropertyValue(string prop, object currentValue)
		{
			object obj = null;
			switch (prop)
			{
			case "Size":
				return new MapSize(null, 100f, 100f);
			case "Dock":
				return PanelDockStyle.None;
			case "DockAlignment":
				return DockAlignment.Center;
			case "BackColor":
				return Color.Empty;
			case "BackGradientType":
				return GradientType.None;
			case "BorderWidth":
				return 0;
			default:
				return base.GetDefaultPropertyValue(prop, currentValue);
			}
		}

		string IToolTipProvider.GetToolTip()
		{
			if (Common != null && Common.MapCore != null)
			{
				return Common.MapCore.ResolveAllKeywords(ToolTip, this);
			}
			return ToolTip;
		}

		string IImageMapProvider.GetToolTip()
		{
			return ((IToolTipProvider)this).GetToolTip();
		}

		string IImageMapProvider.GetHref()
		{
			if (Common != null && Common.MapCore != null)
			{
				return Common.MapCore.ResolveAllKeywords(Href, this);
			}
			return Href;
		}

		string IImageMapProvider.GetMapAreaAttributes()
		{
			if (Common != null && Common.MapCore != null)
			{
				return Common.MapCore.ResolveAllKeywords(MapAreaAttributes, this);
			}
			return MapAreaAttributes;
		}
	}
}

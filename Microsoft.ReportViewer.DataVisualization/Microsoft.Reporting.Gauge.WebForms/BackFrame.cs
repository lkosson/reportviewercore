using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(BackFrameConverter))]
	internal class BackFrame : GaugeObject
	{
		private XamlRenderer xamlRenderer;

		private BackFrameStyle style;

		private BackFrameShape shape;

		private float frameWidth = 8f;

		private Color frameColor = Color.Gainsboro;

		private GradientType frameGradientType = GradientType.DiagonalLeft;

		private Color frameGradientEndColor = Color.Gray;

		private GaugeHatchStyle frameHatchStyle;

		private string image = "";

		private Color imageTransColor = Color.Empty;

		private Color imageHueColor = Color.Empty;

		private Color borderColor = Color.Black;

		private GaugeDashStyle borderStyle;

		private int borderWidth = 1;

		private Color backColor = Color.Silver;

		private GradientType backGradientType = GradientType.DiagonalLeft;

		private Color backGradientEndColor = Color.Gray;

		private GaugeHatchStyle backHatchStyle;

		private float shadowOffset;

		private GlassEffect glassEffect;

		private bool clipImage;

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_FrameStyle")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[NotifyParentProperty(true)]
		[ParenthesizePropertyName(true)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Obsolete("This property is obsolete in Dundas Gauge 2.0. FrameStyle is supposed to be used instead.")]
		public BackFrameStyle Style
		{
			get
			{
				return style;
			}
			set
			{
				style = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_FrameStyle")]
		[NotifyParentProperty(true)]
		[ParenthesizePropertyName(true)]
		public BackFrameStyle FrameStyle
		{
			get
			{
				return style;
			}
			set
			{
				style = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_FrameShape")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[RefreshProperties(RefreshProperties.All)]
		[NotifyParentProperty(true)]
		[ParenthesizePropertyName(true)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Obsolete("This property is obsolete in Dundas Gauge 2.0. FrameShape is supposed to be used instead.")]
		public BackFrameShape Shape
		{
			get
			{
				return shape;
			}
			set
			{
				shape = value;
				ResetCachedXamlRenderer();
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_FrameShape")]
		[RefreshProperties(RefreshProperties.All)]
		[NotifyParentProperty(true)]
		[ParenthesizePropertyName(true)]
		public BackFrameShape FrameShape
		{
			get
			{
				return shape;
			}
			set
			{
				shape = value;
				ResetCachedXamlRenderer();
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_FrameWidth")]
		[NotifyParentProperty(true)]
		[ValidateBound(0.0, 50.0)]
		[DefaultValue(8f)]
		public float FrameWidth
		{
			get
			{
				return frameWidth;
			}
			set
			{
				if (value < 0f || value > 50f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 50));
				}
				frameWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_FrameColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Gainsboro")]
		public Color FrameColor
		{
			get
			{
				return frameColor;
			}
			set
			{
				frameColor = value;
				ResetCachedXamlRenderer();
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_FrameGradientType")]
		[NotifyParentProperty(true)]
		[DefaultValue(GradientType.DiagonalLeft)]
		public GradientType FrameGradientType
		{
			get
			{
				return frameGradientType;
			}
			set
			{
				frameGradientType = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_FrameGradientEndColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Gray")]
		public Color FrameGradientEndColor
		{
			get
			{
				return frameGradientEndColor;
			}
			set
			{
				frameGradientEndColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_FrameHatchStyle")]
		[NotifyParentProperty(true)]
		[DefaultValue(GaugeHatchStyle.None)]
		public GaugeHatchStyle FrameHatchStyle
		{
			get
			{
				return frameHatchStyle;
			}
			set
			{
				frameHatchStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_Image")]
		[NotifyParentProperty(true)]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_ImageTransColor")]
		[NotifyParentProperty(true)]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_ImageHueColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "")]
		public Color ImageHueColor
		{
			get
			{
				return imageHueColor;
			}
			set
			{
				imageHueColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_BorderColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Black")]
		public Color BorderColor
		{
			get
			{
				return borderColor;
			}
			set
			{
				borderColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_BorderStyle")]
		[NotifyParentProperty(true)]
		[DefaultValue(GaugeDashStyle.NotSet)]
		public GaugeDashStyle BorderStyle
		{
			get
			{
				return borderStyle;
			}
			set
			{
				borderStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_BorderWidth")]
		[NotifyParentProperty(true)]
		[DefaultValue(1)]
		public int BorderWidth
		{
			get
			{
				return borderWidth;
			}
			set
			{
				if (value < 0 || value > 100)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
				}
				borderWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_BackColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Silver")]
		public Color BackColor
		{
			get
			{
				return backColor;
			}
			set
			{
				backColor = value;
				ResetCachedXamlRenderer();
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_BackGradientType")]
		[NotifyParentProperty(true)]
		[DefaultValue(GradientType.DiagonalLeft)]
		public GradientType BackGradientType
		{
			get
			{
				return backGradientType;
			}
			set
			{
				backGradientType = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_BackGradientEndColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Gray")]
		public Color BackGradientEndColor
		{
			get
			{
				return backGradientEndColor;
			}
			set
			{
				backGradientEndColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_BackHatchStyle")]
		[NotifyParentProperty(true)]
		[DefaultValue(GaugeHatchStyle.None)]
		public GaugeHatchStyle BackHatchStyle
		{
			get
			{
				return backHatchStyle;
			}
			set
			{
				backHatchStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeShadowOffset4")]
		[NotifyParentProperty(true)]
		[ValidateBound(-5.0, 5.0)]
		[DefaultValue(0f)]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
				}
				shadowOffset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_GlassEffect")]
		[NotifyParentProperty(true)]
		[DefaultValue(GlassEffect.None)]
		public GlassEffect GlassEffect
		{
			get
			{
				return glassEffect;
			}
			set
			{
				glassEffect = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_ClipImage")]
		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		public bool ClipImage
		{
			get
			{
				return clipImage;
			}
			set
			{
				clipImage = value;
				Invalidate();
			}
		}

		public BackFrame()
			: this(null)
		{
		}

		public BackFrame(object parent)
			: base(parent)
		{
		}

		public BackFrame(object parent, BackFrameStyle style, BackFrameShape shape)
			: this(parent)
		{
			this.style = style;
			this.shape = shape;
		}

		protected bool ShouldSerializeStyle()
		{
			return false;
		}

		protected bool ShouldSerializeShape()
		{
			return false;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected void ResetFrameStyle()
		{
			if (Parent is GaugeCore)
			{
				FrameStyle = BackFrameStyle.None;
			}
			else if (Parent is GaugeBase)
			{
				FrameStyle = BackFrameStyle.Edged;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected void ResetFrameShape()
		{
			if (Parent is LinearGauge || Parent is GaugeCore)
			{
				FrameShape = BackFrameShape.Rectangular;
			}
			else if (Parent is CircularGauge)
			{
				FrameShape = BackFrameShape.Circular;
			}
		}

		internal XamlRenderer GetCachedXamlRenderer(GaugeGraphics g)
		{
			if (xamlRenderer != null)
			{
				return xamlRenderer;
			}
			BackFrameStyle backFrameStyle = FrameStyle;
			if (backFrameStyle == BackFrameStyle.None)
			{
				backFrameStyle = BackFrameStyle.Edged;
			}
			xamlRenderer = new XamlRenderer(FrameShape.ToString() + "." + backFrameStyle.ToString() + ".xaml");
			xamlRenderer.AllowPathGradientTransform = false;
			RectangleF frameRectangle = GetFrameRectangle(g);
			Color[] layerHues = new Color[2]
			{
				FrameColor,
				BackColor
			};
			xamlRenderer.ParseXaml(frameRectangle, layerHues);
			return xamlRenderer;
		}

		internal void ResetCachedXamlRenderer()
		{
			if (xamlRenderer != null)
			{
				xamlRenderer.Dispose();
				xamlRenderer = null;
			}
		}

		internal GraphicsPath GetFramePath(GaugeGraphics g, float shrinkBy)
		{
			RectangleF frameRectangle = GetFrameRectangle(g);
			float absoluteDimension = g.GetAbsoluteDimension(shrinkBy);
			frameRectangle.Inflate(0f - absoluteDimension, 0f - absoluteDimension);
			if (shrinkBy > 0f)
			{
				frameRectangle.Inflate(1f, 1f);
			}
			if (FrameShape == BackFrameShape.Circular)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddEllipse(frameRectangle);
				return graphicsPath;
			}
			if (FrameShape == BackFrameShape.AutoShape)
			{
				GraphicsPath graphicsPath2 = new GraphicsPath();
				if (Parent is CircularGauge)
				{
					CircularGauge circularGauge = (CircularGauge)Parent;
					if (circularGauge.Scales.Count == 0)
					{
						graphicsPath2.AddEllipse(frameRectangle);
					}
					else
					{
						BuildCircularGaugeAutoFrame(g, graphicsPath2, circularGauge, shrinkBy);
					}
				}
				else
				{
					graphicsPath2.AddRectangle(frameRectangle);
				}
				return graphicsPath2;
			}
			if (FrameShape != BackFrameShape.RoundedRectangular)
			{
				if (FrameShape == BackFrameShape.Rectangular)
				{
					GraphicsPath graphicsPath3 = new GraphicsPath();
					graphicsPath3.AddRectangle(frameRectangle);
					return graphicsPath3;
				}
				GraphicsPath graphicsPath4 = new GraphicsPath();
				graphicsPath4.FillMode = FillMode.Winding;
				XamlRenderer cachedXamlRenderer = GetCachedXamlRenderer(g);
				graphicsPath4.AddPath(cachedXamlRenderer.Layers[0].Paths[0], connect: false);
				return graphicsPath4;
			}
			float num = (!(frameRectangle.Width > frameRectangle.Height)) ? frameRectangle.Width : frameRectangle.Height;
			float num2 = num / 8f;
			float[] array = new float[10];
			for (int i = 0; i < 10; i++)
			{
				array[i] = num2;
			}
			return g.CreateRoundedRectPath(frameRectangle, array);
		}

		internal GraphicsPath GetBackPath(GaugeGraphics g)
		{
			if (FrameShape == BackFrameShape.AutoShape)
			{
				return GetFramePath(g, 0f);
			}
			if (IsCustomXamlFrame())
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.FillMode = FillMode.Winding;
				XamlRenderer cachedXamlRenderer = GetCachedXamlRenderer(g);
				graphicsPath.AddPath(cachedXamlRenderer.Layers[2].Paths[0], connect: false);
				return graphicsPath;
			}
			return GetFramePath(g, FrameWidth);
		}

		private void BuildCircularGaugeAutoFrame(GaugeGraphics g, GraphicsPath path, CircularGauge gauge, float shrinkBy)
		{
			float startAngle = gauge.Scales[0].StartAngle;
			float num = gauge.Scales[0].StartAngle + gauge.Scales[0].SweepAngle;
			float radius = gauge.Scales[0].Radius;
			float num2 = gauge.Scales[0].GetLargestRadius(g);
			if (gauge.Scales.Count > 1)
			{
				for (int i = 1; i < gauge.Scales.Count; i++)
				{
					if (startAngle > gauge.Scales[i].StartAngle)
					{
						startAngle = gauge.Scales[i].StartAngle;
					}
					if (num < gauge.Scales[i].StartAngle + gauge.Scales[i].SweepAngle)
					{
						num = gauge.Scales[i].StartAngle + gauge.Scales[i].SweepAngle;
					}
					if (radius < gauge.Scales[i].Radius)
					{
						radius = gauge.Scales[i].Radius;
					}
					float largestRadius = gauge.Scales[i].GetLargestRadius(g);
					if (num2 < largestRadius)
					{
						num2 = largestRadius;
					}
				}
			}
			float num3 = 0f;
			foreach (CircularPointer pointer in gauge.Pointers)
			{
				if (pointer.Visible && pointer.Type == CircularPointerType.Needle)
				{
					float num4 = pointer.CapWidth / 2f * pointer.GetScale().Radius / 100f;
					if (pointer.CapVisible && num4 > num3)
					{
						num3 = num4;
					}
					float num5 = pointer.GetNeedleTailLength() * pointer.GetScale().Radius / 100f;
					if (num5 > num3)
					{
						num3 = num5;
					}
				}
			}
			foreach (Knob knob in gauge.Knobs)
			{
				if (knob.Visible)
				{
					float num6 = knob.Width * knob.GetScale().Radius / 100f;
					if (num6 > num3)
					{
						num3 = num6;
					}
				}
			}
			num3 = g.GetAbsoluteDimension(num3);
			float absoluteDimension = g.GetAbsoluteDimension(radius / 5f);
			float absoluteDimension2 = g.GetAbsoluteDimension(FrameWidth * radius / 100f);
			float absoluteDimension3 = g.GetAbsoluteDimension(shrinkBy * radius / 100f);
			absoluteDimension += absoluteDimension2;
			absoluteDimension -= absoluteDimension3;
			float num7 = num - startAngle;
			PointF absolutePoint = g.GetAbsolutePoint(gauge.PivotPoint);
			float absoluteDimension4 = g.GetAbsoluteDimension(num2);
			float num8 = startAngle * (float)Math.PI / 180f;
			float num9 = (360f - startAngle - num7) * (float)Math.PI / 180f;
			PointF pointF = default(PointF);
			pointF.X = absolutePoint.X - absoluteDimension4 * (float)Math.Sin(num8);
			pointF.Y = absolutePoint.Y + absoluteDimension4 * (float)Math.Cos(num8);
			PointF pointF2 = default(PointF);
			pointF2.X = absolutePoint.X + absoluteDimension4 * (float)Math.Sin(num9);
			pointF2.Y = absolutePoint.Y + absoluteDimension4 * (float)Math.Cos(num9);
			RectangleF rect = new RectangleF(pointF.X, pointF.Y, 0f, 0f);
			rect.Inflate(absoluteDimension, absoluteDimension);
			RectangleF rect2 = new RectangleF(pointF2.X, pointF2.Y, 0f, 0f);
			rect2.Inflate(absoluteDimension, absoluteDimension);
			RectangleF rect3 = new RectangleF(absolutePoint.X, absolutePoint.Y, 0f, 0f);
			rect3.Inflate(absoluteDimension + num3, absoluteDimension + num3);
			RectangleF rect4 = new RectangleF(absolutePoint.X, absolutePoint.Y, 0f, 0f);
			rect4.Inflate(absoluteDimension4 + absoluteDimension, absoluteDimension4 + absoluteDimension);
			if (num7 < 270f)
			{
				path.AddArc(rect, startAngle + 270f + 90f, 90f);
				path.AddArc(rect4, startAngle + 90f, num7);
				path.AddArc(rect2, startAngle + num7 + 90f, 90f);
				path.AddArc(rect3, startAngle + num7 + 90f + 45f, 360f - num7 - 90f);
			}
			else if (num7 >= 320f)
			{
				path.AddEllipse(rect4);
			}
			else
			{
				float num10 = 90f - (360f - num7) / 2f;
				path.AddArc(rect, startAngle + 270f + 90f + num10, 90f - num10);
				path.AddArc(rect4, startAngle + 90f, num7);
				path.AddArc(rect2, startAngle + num7 + 90f, 90f - num10);
			}
			path.CloseFigure();
		}

		internal static float GetXamlFrameAspectRatio(BackFrameShape shape)
		{
			float result = 1f;
			if (shape >= (BackFrameShape)2000 && shape <= (BackFrameShape)2199)
			{
				result = 1.48275864f;
			}
			else if (shape >= (BackFrameShape)2200 && shape <= (BackFrameShape)2399)
			{
				result = 0.6744186f;
			}
			return result;
		}

		internal GraphicsPath GetShadowPath(GaugeGraphics g)
		{
			GraphicsPath framePath = GetFramePath(g, 0f);
			using (Matrix matrix = new Matrix())
			{
				matrix.Translate(ShadowOffset, ShadowOffset);
				framePath.Transform(matrix);
				return framePath;
			}
		}

		internal void DrawFrameImage(GaugeGraphics g)
		{
			GraphicsPath graphicsPath = null;
			Pen pen = null;
			Region region = null;
			try
			{
				graphicsPath = GetFramePath(g, 0f);
				RectangleF frameRectangle = GetFrameRectangle(g);
				Region clip = null;
				if (ClipImage)
				{
					RenderShadow(g);
					region = new Region(graphicsPath);
					clip = g.Clip;
					g.Clip = region;
				}
				else if (ShadowOffset != 0f)
				{
					using (Brush brush = g.GetShadowBrush())
					{
						RectangleF rect = frameRectangle;
						rect.Offset(ShadowOffset, ShadowOffset);
						g.FillRectangle(brush, rect);
					}
				}
				ImageAttributes imageAttributes = new ImageAttributes();
				if (ImageTransColor != Color.Empty)
				{
					imageAttributes.SetColorKey(ImageTransColor, ImageTransColor, ColorAdjustType.Default);
				}
				Image image = Common.ImageLoader.LoadImage(Image);
				Rectangle destRect = new Rectangle((int)Math.Round(frameRectangle.X), (int)Math.Round(frameRectangle.Y), (int)Math.Round(frameRectangle.Width), (int)Math.Round(frameRectangle.Height));
				if (!ImageHueColor.IsEmpty)
				{
					Color color = g.TransformHueColor(ImageHueColor);
					ColorMatrix colorMatrix = new ColorMatrix();
					colorMatrix.Matrix00 = (float)(int)color.R / 255f;
					colorMatrix.Matrix11 = (float)(int)color.G / 255f;
					colorMatrix.Matrix22 = (float)(int)color.B / 255f;
					imageAttributes.SetColorMatrix(colorMatrix);
				}
				ImageSmoothingState imageSmoothingState = new ImageSmoothingState(g);
				imageSmoothingState.Set();
				g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				imageSmoothingState.Restore();
				if (ClipImage)
				{
					g.Clip = clip;
				}
				if (BorderWidth > 0 && BorderStyle != 0)
				{
					pen = new Pen(BorderColor, BorderWidth);
					pen.DashStyle = g.GetPenStyle(BorderStyle);
					pen.Alignment = PenAlignment.Center;
					if (ClipImage)
					{
						g.DrawPath(pen, graphicsPath);
					}
					else
					{
						g.DrawRectangle(pen, frameRectangle.X, frameRectangle.Y, frameRectangle.Width, frameRectangle.Height);
					}
				}
			}
			finally
			{
				graphicsPath?.Dispose();
				pen?.Dispose();
				region?.Dispose();
			}
		}

		internal bool IsCustomXamlFrame()
		{
			if (FrameShape == BackFrameShape.Circular || FrameShape == BackFrameShape.Rectangular || FrameShape == BackFrameShape.RoundedRectangular || FrameShape == BackFrameShape.AutoShape)
			{
				return false;
			}
			return true;
		}

		internal void RenderFrame(GaugeGraphics g)
		{
			if (Image.Length != 0)
			{
				Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceStartRenderingImageFrame);
				DrawFrameImage(g);
				Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceImageFrameRenderingComplete);
			}
			else
			{
				if (FrameStyle == BackFrameStyle.None)
				{
					return;
				}
				Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceStartRenderingFrame);
				ResetCachedXamlRenderer();
				if (IsCustomXamlFrame())
				{
					RenderShadow(g);
					XamlRenderer cachedXamlRenderer = GetCachedXamlRenderer(g);
					cachedXamlRenderer.Layers[0].Render(g);
					cachedXamlRenderer.Layers[1].Render(g);
				}
				else
				{
					RenderShadow(g);
					Brush brush = null;
					GraphicsPath graphicsPath = null;
					GraphicsPath graphicsPath2 = null;
					GraphicsPath graphicsPath3 = null;
					Brush brush2 = null;
					Brush brush3 = null;
					Pen pen = null;
					Pen pen2 = null;
					try
					{
						graphicsPath = GetFramePath(g, 0f);
						graphicsPath3 = GetFramePath(g, FrameWidth);
						brush = GetBrush(g, graphicsPath3.GetBounds(), backHatchStyle, BackGradientType, BackColor, BackGradientEndColor, frame: false, 0f);
						g.FillPath(brush, graphicsPath3, 0f, useBrushOffset: false, FrameShape == BackFrameShape.Circular);
						if (FrameStyle == BackFrameStyle.Simple)
						{
							using (GraphicsPath graphicsPath4 = new GraphicsPath())
							{
								graphicsPath4.AddPath(graphicsPath, connect: false);
								graphicsPath4.AddPath(graphicsPath3, connect: false);
								graphicsPath4.CloseFigure();
								brush2 = GetBrush(g, graphicsPath4.GetBounds(), FrameHatchStyle, FrameGradientType, FrameColor, FrameGradientEndColor, frame: true, frameWidth);
								pen = new Pen(brush, 2f);
								g.DrawPath(pen, graphicsPath3);
								g.FillPath(brush2, graphicsPath4, 0f, useBrushOffset: false, FrameShape == BackFrameShape.Circular);
							}
						}
						else if (FrameStyle == BackFrameStyle.Edged)
						{
							float num = FrameWidth * 0.7f;
							using (GraphicsPath addingPath = GetFramePath(g, num))
							{
								using (GraphicsPath graphicsPath5 = new GraphicsPath())
								{
									using (GraphicsPath graphicsPath6 = new GraphicsPath())
									{
										graphicsPath5.AddPath(graphicsPath, connect: false);
										graphicsPath5.AddPath(addingPath, connect: false);
										graphicsPath6.AddPath(addingPath, connect: false);
										graphicsPath6.AddPath(graphicsPath3, connect: false);
										brush2 = GetBrush(g, graphicsPath5.GetBounds(), FrameHatchStyle, FrameGradientType, FrameColor, FrameGradientEndColor, frame: true, num);
										g.FillPath(brush2, graphicsPath5, 0f, useBrushOffset: false, FrameShape == BackFrameShape.Circular);
										brush3 = ((FrameGradientType != 0 || FrameHatchStyle != 0) ? GetBrush(g, graphicsPath6.GetBounds(), FrameHatchStyle, FrameGradientType, FrameGradientEndColor, FrameColor, frame: true, frameWidth - num) : GetBrush(g, graphicsPath6.GetBounds(), FrameHatchStyle, FrameGradientType, FrameColor, FrameColor, frame: true, frameWidth - num));
										if (FrameWidth > 0f)
										{
											pen = new Pen(brush3, 2f);
											g.DrawPath(pen, graphicsPath6);
										}
										g.FillPath(brush3, graphicsPath6, 0f, useBrushOffset: false, FrameShape == BackFrameShape.Circular);
									}
								}
							}
						}
						if (BorderWidth > 0 && BorderStyle != 0)
						{
							pen2 = new Pen(BorderColor, BorderWidth);
							pen2.DashStyle = g.GetPenStyle(BorderStyle);
							pen2.Alignment = PenAlignment.Center;
							g.DrawPath(pen2, graphicsPath);
						}
					}
					finally
					{
						brush?.Dispose();
						graphicsPath?.Dispose();
						graphicsPath2?.Dispose();
						brush2?.Dispose();
						brush3?.Dispose();
						pen?.Dispose();
						pen2?.Dispose();
					}
				}
				Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceFrameRenderingComplete);
			}
		}

		internal void RenderShadow(GaugeGraphics g)
		{
			if (ShadowOffset == 0f)
			{
				return;
			}
			using (GraphicsPath path = GetShadowPath(g))
			{
				using (Brush brush = g.GetShadowBrush())
				{
					g.FillPath(brush, path);
				}
			}
		}

		internal Brush GetBrush(GaugeGraphics g, RectangleF rect, GaugeHatchStyle hatchStyle, GradientType gradientType, Color fillColor, Color gradientEndColor, bool frame, float frameWidth)
		{
			Brush brush = null;
			if (hatchStyle != 0)
			{
				brush = GaugeGraphics.GetHatchBrush(hatchStyle, fillColor, gradientEndColor);
			}
			else if (gradientType != 0)
			{
				if (FrameShape == BackFrameShape.Circular && gradientType == GradientType.DiagonalLeft)
				{
					brush = g.GetGradientBrush(rect, fillColor, gradientEndColor, GradientType.LeftRight);
					Matrix matrix = new Matrix();
					matrix.RotateAt(45f, new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f));
					((LinearGradientBrush)brush).Transform = matrix;
				}
				else if (FrameShape == BackFrameShape.Circular && gradientType == GradientType.DiagonalRight)
				{
					brush = g.GetGradientBrush(rect, fillColor, gradientEndColor, GradientType.TopBottom);
					Matrix matrix2 = new Matrix();
					matrix2.RotateAt(135f, new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f));
					((LinearGradientBrush)brush).Transform = matrix2;
				}
				else if (gradientType == GradientType.Center)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					if (FrameShape == BackFrameShape.Circular)
					{
						graphicsPath.AddArc(rect.X, rect.Y, rect.Width, rect.Height, 0f, 360f);
					}
					else
					{
						graphicsPath.AddRectangle(rect);
					}
					PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
					pathGradientBrush.CenterColor = fillColor;
					pathGradientBrush.CenterPoint = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
					pathGradientBrush.SurroundColors = new Color[1]
					{
						gradientEndColor
					};
					if (frame)
					{
						pathGradientBrush.FocusScales = new PointF((rect.Width - frameWidth * 2f) / rect.Width, (rect.Height - frameWidth * 2f) / rect.Height);
					}
					brush = pathGradientBrush;
				}
				else
				{
					brush = g.GetGradientBrush(rect, fillColor, gradientEndColor, gradientType);
				}
			}
			else
			{
				brush = new SolidBrush(fillColor);
			}
			return brush;
		}

		internal RectangleF GetFrameRectangle(GaugeGraphics g)
		{
			RectangleF result;
			if (Parent is GaugeCore)
			{
				result = new RectangleF(0f, 0f, ((GaugeCore)Parent).GetWidth() - 1, ((GaugeCore)Parent).GetHeight() - 1);
			}
			else if ((FrameShape == BackFrameShape.Rectangular || FrameShape == BackFrameShape.RoundedRectangular) && Parent is CircularGauge)
			{
				CircularGauge circularGauge = (CircularGauge)Parent;
				if (circularGauge.ParentObject != null)
				{
					result = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
					if (circularGauge.Position.Rectangle.Width > 0f && circularGauge.Position.Rectangle.Height > 0f)
					{
						if (!double.IsNaN(circularGauge.AspectRatio))
						{
							if (circularGauge.AspectRatio >= 1f)
							{
								float width = result.Width;
								result.Width = result.Height * circularGauge.AspectRatio;
								result.X += (width - result.Width) / 2f;
							}
							else
							{
								float height = result.Height;
								result.Height = result.Width / circularGauge.AspectRatio;
								result.Y += (height - result.Height) / 2f;
							}
						}
						else
						{
							float num = circularGauge.Position.Rectangle.Width / circularGauge.Position.Rectangle.Height;
							if (circularGauge.Position.Rectangle.Width > circularGauge.Position.Rectangle.Height)
							{
								float num2 = result.Height * num;
								result.X += (result.Width - num2) / 2f;
								result.Width = num2;
							}
							else
							{
								float num3 = result.Width / num;
								result.Y += (result.Height - num3) / 2f;
								result.Height = num3;
							}
						}
					}
				}
				else
				{
					result = circularGauge.absoluteRect;
					if (!double.IsNaN(circularGauge.AspectRatio))
					{
						if (result.Width > result.Height * circularGauge.AspectRatio)
						{
							float width2 = result.Width;
							result.Width = result.Height * circularGauge.AspectRatio;
							result.X += (width2 - result.Width) / 2f;
						}
						else
						{
							float height2 = result.Height;
							result.Height = result.Width / circularGauge.AspectRatio;
							result.Y += (height2 - result.Height) / 2f;
						}
					}
					PointF empty = PointF.Empty;
					empty.X = 0f - g.Graphics.Transform.OffsetX + g.InitialOffset.X;
					empty.Y = 0f - g.Graphics.Transform.OffsetY + g.InitialOffset.Y;
					result.Offset(empty.X, empty.Y);
				}
			}
			else
			{
				result = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			}
			if (FrameShape == BackFrameShape.Circular || IsCustomXamlFrame())
			{
				if (result.Width > result.Height)
				{
					result.X += (result.Width - result.Height) / 2f;
					result.Width = result.Height;
				}
				else if (result.Height > result.Width)
				{
					result.Y += (result.Height - result.Width) / 2f;
					result.Height = result.Width;
				}
			}
			float xamlFrameAspectRatio = GetXamlFrameAspectRatio(Shape);
			if (xamlFrameAspectRatio > 1f)
			{
				float num4 = result.Height * xamlFrameAspectRatio;
				result.X += (result.Width - num4) / 2f;
				result.Width = num4;
			}
			else if (xamlFrameAspectRatio < 1f)
			{
				float num5 = result.Width / xamlFrameAspectRatio;
				result.Y += (result.Height - num5) / 2f;
				result.Height = num5;
			}
			if (Parent is GaugeCore)
			{
				if (ShadowOffset < 0f)
				{
					result.X -= ShadowOffset;
					result.Y -= ShadowOffset;
					result.Width += ShadowOffset;
					result.Height += ShadowOffset;
				}
				else if (ShadowOffset > 0f)
				{
					result.Width -= ShadowOffset;
					result.Height -= ShadowOffset;
				}
			}
			return result;
		}

		internal void RenderGlassEffect(GaugeGraphics g)
		{
			if (GlassEffect == GlassEffect.None || FrameStyle == BackFrameStyle.None)
			{
				return;
			}
			if (IsCustomXamlFrame())
			{
				GetCachedXamlRenderer(g).Layers[2].Render(g);
				return;
			}
			RectangleF bounds;
			using (GraphicsPath graphicsPath = GetFramePath(g, FrameWidth - 1f))
			{
				bounds = graphicsPath.GetBounds();
				using (Brush brush = new LinearGradientBrush(bounds, Color.FromArgb(15, Color.Black), Color.FromArgb(128, Color.Black), LinearGradientMode.ForwardDiagonal))
				{
					if (bounds.Height > 0f && bounds.Width > 0f)
					{
						g.FillPath(brush, graphicsPath);
					}
				}
			}
			if (FrameShape == BackFrameShape.Rectangular || FrameShape == BackFrameShape.RoundedRectangular)
			{
				_ = g.GetAbsoluteSize(new SizeF(8f, 0f)).Width;
				GraphicsPath graphicsPath2 = new GraphicsPath();
				float absoluteDimension = g.GetAbsoluteDimension(30f);
				float absoluteDimension2 = g.GetAbsoluteDimension(10f);
				float absoluteDimension3 = g.GetAbsoluteDimension(50f);
				float absoluteDimension4 = g.GetAbsoluteDimension(5f);
				g.GetAbsoluteDimension(30f);
				g.GetAbsoluteDimension(5f);
				PointF[] points = new PointF[4]
				{
					new PointF(bounds.X, bounds.Y + absoluteDimension),
					new PointF(bounds.X + absoluteDimension, bounds.Y),
					new PointF(bounds.X + absoluteDimension + absoluteDimension2, bounds.Y),
					new PointF(bounds.X, bounds.Y + absoluteDimension + absoluteDimension2)
				};
				PointF[] points2 = new PointF[4]
				{
					new PointF(bounds.X, bounds.Y + absoluteDimension3),
					new PointF(bounds.X + absoluteDimension3, bounds.Y),
					new PointF(bounds.X + absoluteDimension3 + absoluteDimension4, bounds.Y),
					new PointF(bounds.X, bounds.Y + absoluteDimension3 + absoluteDimension4)
				};
				graphicsPath2.AddPolygon(points);
				graphicsPath2.AddPolygon(points2);
				Brush brush2 = new SolidBrush(Color.FromArgb(148, Color.White));
				g.FillPath(brush2, graphicsPath2);
			}
			else if ((FrameShape == BackFrameShape.Circular || FrameShape == BackFrameShape.AutoShape) && GlassEffect == GlassEffect.Simple)
			{
				float absoluteDimension5 = g.GetAbsoluteDimension(15f);
				bounds.X += absoluteDimension5;
				bounds.Y += absoluteDimension5;
				bounds.Width -= absoluteDimension5 * 2f;
				bounds.Height -= absoluteDimension5 * 2f;
				GraphicsPath circularRangePath = g.GetCircularRangePath(g.GetRelativeRectangle(bounds), 226f, 30f, 6f, 6f, Placement.Inside);
				GraphicsPath circularRangePath2 = g.GetCircularRangePath(g.GetRelativeRectangle(bounds), 224f, -30f, 6f, 6f, Placement.Inside);
				Brush brush3 = new SolidBrush(Color.FromArgb(200, Color.White));
				g.FillPath(brush3, circularRangePath);
				g.FillPath(brush3, circularRangePath2);
			}
		}
	}
}

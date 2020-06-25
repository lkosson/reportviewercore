using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class LinearPointer : MapObject, IToolTipProvider
	{
		private double position;

		internal bool dragging;

		private LinearPointerType type;

		private Placement placement = Placement.Outside;

		private float width = 20f;

		private MarkerStyle markerStyle = MarkerStyle.Triangle;

		private float markerLength = 20f;

		private MapCursor cursor = MapCursor.Default;

		private float distanceFromScale;

		private string image = "";

		private Color imageTransColor = Color.Empty;

		private Point imageOrigin = Point.Empty;

		private double val;

		private bool snappingEnabled;

		private double snappingInterval;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private bool interactive = true;

		private bool visible = true;

		private float shadowOffset = 2f;

		private Color borderColor = Color.DarkGray;

		private MapDashStyle borderStyle;

		private int borderWidth = 1;

		private Color fillColor = Color.White;

		private Color fillSecondaryColor = Color.Red;

		private MapHatchStyle fillHatchStyle;

		private GradientType fillGradientType = GradientType.DiagonalLeft;

		[SRCategory("CategoryAttribute_TypeSpecific")]
		[SRDescription("DescriptionAttributeLinearPointer_Type")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(LinearPointerType.Marker)]
		public LinearPointerType Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearPointer_Placement")]
		[DefaultValue(Placement.Outside)]
		public Placement Placement
		{
			get
			{
				return placement;
			}
			set
			{
				placement = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearPointer_Width")]
		[DefaultValue(20f)]
		public float Width
		{
			get
			{
				return width;
			}
			set
			{
				if (value < 0f || value > 200f)
				{
					throw new ArgumentException(SR.must_in_range(0.0, 200.0));
				}
				width = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_TypeSpecific")]
		[SRDescription("DescriptionAttributeLinearPointer_MarkerStyle")]
		[DefaultValue(MarkerStyle.Triangle)]
		public MarkerStyle MarkerStyle
		{
			get
			{
				return markerStyle;
			}
			set
			{
				markerStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_TypeSpecific")]
		[SRDescription("DescriptionAttributeLinearPointer_MarkerLength")]
		[DefaultValue(20f)]
		public float MarkerLength
		{
			get
			{
				return markerLength;
			}
			set
			{
				if (value < 0f || value > 200f)
				{
					throw new ArgumentException(SR.must_in_range(0.0, 200.0));
				}
				markerLength = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearPointer_Cursor")]
		[DefaultValue(MapCursor.Default)]
		public MapCursor Cursor
		{
			get
			{
				return cursor;
			}
			set
			{
				cursor = value;
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearPointer_DistanceFromScale")]
		[DefaultValue(0f)]
		public virtual float DistanceFromScale
		{
			get
			{
				return distanceFromScale;
			}
			set
			{
				if (value < -100f || value > 100f)
				{
					throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
				}
				distanceFromScale = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeLinearPointer_Image")]
		[DefaultValue("")]
		public virtual string Image
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
		[SRDescription("DescriptionAttributeLinearPointer_ImageTransColor")]
		[DefaultValue(typeof(Color), "")]
		public virtual Color ImageTransColor
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

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeLinearPointer_ImageOrigin")]
		[TypeConverter(typeof(EmptyPointConverter))]
		[DefaultValue(typeof(Point), "0, 0")]
		public virtual Point ImageOrigin
		{
			get
			{
				return imageOrigin;
			}
			set
			{
				imageOrigin = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeLinearPointer_Value")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.Repaint)]
		[DefaultValue(double.NaN)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public virtual double Value
		{
			get
			{
				return val;
			}
			set
			{
				LinearScale scale = GetScale();
				val = Math.Max(scale.Minimum, Math.Min(value, scale.Maximum));
				Position = val;
				if (!dragging)
				{
					GetGauge().InternalZoomLevelChanged();
				}
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearPointer_SnappingEnabled")]
		[DefaultValue(false)]
		public virtual bool SnappingEnabled
		{
			get
			{
				return snappingEnabled;
			}
			set
			{
				snappingEnabled = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearPointer_SnappingInterval")]
		[DefaultValue(0.0)]
		public double SnappingInterval
		{
			get
			{
				return snappingInterval;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(SR.ExceptionValueCannotBeNegative);
				}
				snappingInterval = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearPointer_ToolTip")]
		[Localizable(true)]
		[DefaultValue("")]
		public virtual string ToolTip
		{
			get
			{
				return toolTip;
			}
			set
			{
				toolTip = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearPointer_Href")]
		[Localizable(true)]
		[DefaultValue("")]
		public string Href
		{
			get
			{
				return href;
			}
			set
			{
				href = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearPointer_MapAreaAttributes")]
		[DefaultValue("")]
		public string MapAreaAttributes
		{
			get
			{
				return mapAreaAttributes;
			}
			set
			{
				mapAreaAttributes = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearPointer_Interactive")]
		[DefaultValue(true)]
		public virtual bool Interactive
		{
			get
			{
				return interactive;
			}
			set
			{
				interactive = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearPointer_Visible")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(true)]
		public virtual bool Visible
		{
			get
			{
				return visible;
			}
			set
			{
				visible = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearPointer_ShadowOffset")]
		[DefaultValue(2f)]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearPointer_BorderColor")]
		[DefaultValue(typeof(Color), "DarkGray")]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearPointer_BorderStyle")]
		[DefaultValue(MapDashStyle.None)]
		public MapDashStyle BorderStyle
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearPointer_BorderWidth")]
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
					throw new ArgumentException(SR.must_in_range(0.0, 100.0));
				}
				borderWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearPointer_FillColor")]
		[DefaultValue(typeof(Color), "White")]
		public virtual Color FillColor
		{
			get
			{
				return fillColor;
			}
			set
			{
				fillColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearPointer_FillSecondaryColor")]
		[DefaultValue(typeof(Color), "Red")]
		public virtual Color FillSecondaryColor
		{
			get
			{
				return fillSecondaryColor;
			}
			set
			{
				fillSecondaryColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearPointer_FillHatchStyle")]
		[DefaultValue(MapHatchStyle.None)]
		public virtual MapHatchStyle FillHatchStyle
		{
			get
			{
				return fillHatchStyle;
			}
			set
			{
				fillHatchStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearPointer_FillGradientType")]
		[DefaultValue(GradientType.DiagonalLeft)]
		public virtual GradientType FillGradientType
		{
			get
			{
				return fillGradientType;
			}
			set
			{
				fillGradientType = value;
				Invalidate();
			}
		}

		internal double Position
		{
			get
			{
				if (double.IsNaN(position))
				{
					return GetScale().GetValueLimit(position);
				}
				return position;
			}
			set
			{
				position = value;
				Invalidate();
			}
		}

		internal override CommonElements Common
		{
			get
			{
				return base.Common;
			}
			set
			{
				base.Common = value;
			}
		}

		public LinearPointer()
			: this(null)
		{
		}

		public LinearPointer(object parent)
			: base(parent)
		{
			markerStyle = MarkerStyle.Triangle;
			markerLength = 20f;
			width = 20f;
			fillGradientType = GradientType.DiagonalLeft;
		}

		internal void Render(MapGraphics g)
		{
			if (Common == null || !Visible || GetScale() == null)
			{
				return;
			}
			g.StartHotRegion(this);
			if (!string.IsNullOrEmpty(Image))
			{
				DrawImage(g, drawShadow: false);
				g.EndHotRegion();
				return;
			}
			Pen pen = new Pen(BorderColor, BorderWidth);
			pen.DashStyle = MapGraphics.GetPenStyle(BorderStyle);
			if (pen.DashStyle != 0)
			{
				pen.Alignment = PenAlignment.Center;
			}
			MarkerStyleAttrib markerStyleAttrib = GetMarkerStyleAttrib(g);
			try
			{
				if (markerStyleAttrib.path != null)
				{
					bool circularFill = (MarkerStyle == MarkerStyle.Circle) ? true : false;
					g.FillPath(markerStyleAttrib.brush, markerStyleAttrib.path, 0f, useBrushOffset: true, circularFill);
				}
				if (BorderWidth > 0 && markerStyleAttrib.path != null)
				{
					g.DrawPath(pen, markerStyleAttrib.path);
				}
			}
			catch (Exception)
			{
				markerStyleAttrib.Dispose();
			}
			if (markerStyleAttrib.path != null)
			{
				Common.MapCore.HotRegionList.SetHotRegion(g, this, markerStyleAttrib.path);
			}
			g.EndHotRegion();
		}

		internal void DrawImage(MapGraphics g, bool drawShadow)
		{
			if (!Visible || (drawShadow && ShadowOffset == 0f))
			{
				return;
			}
			Image image = Common.ImageLoader.LoadImage(Image);
			if (image.Width == 0 || image.Height == 0)
			{
				return;
			}
			Point point = new Point(ImageOrigin.X, ImageOrigin.Y);
			if (point.X == 0 && point.Y == 0)
			{
				point.X = image.Width / 2;
				point.Y = image.Height / 2;
			}
			float absoluteDimension = g.GetAbsoluteDimension(Width);
			float absoluteDimension2 = g.GetAbsoluteDimension(MarkerLength);
			float num = absoluteDimension / (float)image.Width;
			float num2 = absoluteDimension2 / (float)image.Height;
			float num3 = CalculateMarkerDistance();
			float positionFromValue = GetScale().GetPositionFromValue(Position);
			PointF pointF = Point.Empty;
			pointF = ((GetGauge().GetOrientation() != 0) ? g.GetAbsolutePoint(new PointF(num3, positionFromValue)) : g.GetAbsolutePoint(new PointF(positionFromValue, num3)));
			Rectangle rectangle = new Rectangle((int)(pointF.X - (float)point.X * num) + 1, (int)(pointF.Y - (float)point.Y * num2) + 1, (int)((float)image.Width * num) + 1, (int)((float)image.Height * num2) + 1);
			ImageAttributes imageAttributes = new ImageAttributes();
			if (ImageTransColor != Color.Empty)
			{
				imageAttributes.SetColorKey(ImageTransColor, ImageTransColor, ColorAdjustType.Default);
			}
			if (drawShadow)
			{
				ColorMatrix colorMatrix = new ColorMatrix();
				colorMatrix.Matrix00 = 0f;
				colorMatrix.Matrix11 = 0f;
				colorMatrix.Matrix22 = 0f;
				colorMatrix.Matrix33 = Common.MapCore.ShadowIntensity / 100f;
				imageAttributes.SetColorMatrix(colorMatrix);
			}
			g.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
			if (!drawShadow)
			{
				using (GraphicsPath graphicsPath = new GraphicsPath())
				{
					graphicsPath.AddRectangle(rectangle);
					Common.MapCore.HotRegionList.SetHotRegion(g, this, graphicsPath);
				}
			}
		}

		internal MarkerStyleAttrib GetMarkerStyleAttrib(MapGraphics g)
		{
			MarkerStyleAttrib markerStyleAttrib = new MarkerStyleAttrib();
			if (!string.IsNullOrEmpty(Image))
			{
				return markerStyleAttrib;
			}
			float absoluteDimension = g.GetAbsoluteDimension(MarkerLength);
			float absoluteDimension2 = g.GetAbsoluteDimension(Width);
			markerStyleAttrib.path = g.CreateMarker(new PointF(0f, 0f), absoluteDimension2, absoluteDimension, MarkerStyle);
			float num = 0f;
			if (Placement == Placement.Cross || Placement == Placement.Inside)
			{
				num += 180f;
			}
			if (GetGauge().GetOrientation() == Orientation.Vertical)
			{
				num += 270f;
			}
			if (num > 0f)
			{
				using (Matrix matrix = new Matrix())
				{
					matrix.Rotate(num);
					markerStyleAttrib.path.Transform(matrix);
				}
			}
			float num2 = CalculateMarkerDistance();
			LinearScale scale = GetScale();
			float positionFromValue = scale.GetPositionFromValue(scale.GetValueLimit(Position));
			PointF pointF = Point.Empty;
			pointF = ((GetGauge().GetOrientation() != 0) ? g.GetAbsolutePoint(new PointF(num2, positionFromValue)) : g.GetAbsolutePoint(new PointF(positionFromValue, num2)));
			markerStyleAttrib.brush = g.GetMarkerBrush(markerStyleAttrib.path, MarkerStyle, pointF, 0f, FillColor, FillGradientType, FillSecondaryColor, FillHatchStyle);
			using (Matrix matrix2 = new Matrix())
			{
				matrix2.Translate(pointF.X, pointF.Y, MatrixOrder.Append);
				markerStyleAttrib.path.Transform(matrix2);
				return markerStyleAttrib;
			}
		}

		internal float CalculateMarkerDistance()
		{
			if (Placement == Placement.Cross)
			{
				return GetScale().Position - DistanceFromScale;
			}
			if (Placement == Placement.Inside)
			{
				return GetScale().Position - GetScale().Width / 2f - DistanceFromScale - MarkerLength / 2f;
			}
			return GetScale().Position + GetScale().Width / 2f + DistanceFromScale + MarkerLength / 2f;
		}

		internal GraphicsPath GetPointerPath(MapGraphics g)
		{
			if (!Visible)
			{
				return null;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			LinearScale scale = GetScale();
			scale.GetPositionFromValue(scale.GetValueLimit(Position));
			if (Type == LinearPointerType.Marker)
			{
				MarkerStyleAttrib markerStyleAttrib = GetMarkerStyleAttrib(g);
				if (markerStyleAttrib.path != null)
				{
					graphicsPath.AddPath(markerStyleAttrib.path, connect: false);
				}
			}
			return graphicsPath;
		}

		internal GraphicsPath GetShadowPath(MapGraphics g)
		{
			if (ShadowOffset == 0f || GetScale() == null)
			{
				return null;
			}
			if (!string.IsNullOrEmpty(Image))
			{
				DrawImage(g, drawShadow: true);
			}
			GraphicsPath pointerPath = GetPointerPath(g);
			if (pointerPath == null || pointerPath.PointCount == 0)
			{
				return null;
			}
			using (Matrix matrix = new Matrix())
			{
				matrix.Translate(ShadowOffset, ShadowOffset);
				pointerPath.Transform(matrix);
				return pointerPath;
			}
		}

		public ZoomPanel GetGauge()
		{
			return (ZoomPanel)Parent;
		}

		internal LinearScale GetScale()
		{
			return GetGauge().Scale;
		}

		internal virtual void DragTo(int x, int y, PointF refPoint, bool dragging)
		{
			LinearScale scale = GetScale();
			this.dragging = dragging;
			double value = scale.GetValue(refPoint, new PointF(x, y));
			value = scale.GetValueLimit(value, SnappingEnabled, SnappingInterval);
			if (Common != null)
			{
				Value = value;
			}
		}

		internal virtual void RenderShadow(MapGraphics g)
		{
		}

		internal override void BeginInit()
		{
			base.BeginInit();
		}

		internal override void EndInit()
		{
			base.EndInit();
		}

		protected override void OnDispose()
		{
			base.OnDispose();
		}

		string IToolTipProvider.GetToolTip()
		{
			return ToolTip;
		}
	}
}

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class Viewport : Panel
	{
		private bool autoSize = true;

		private int contentSize;

		private int contentAutoFitMargin = 10;

		private bool enablePanning;

		private int minimumZoom = 20;

		private int maximumZoom = 20000;

		private float zoom = 100f;

		private ViewCenter viewCenter;

		private bool optimizeForPanning;

		private bool loadTilesAsynchronously;

		private bool queryVirtualEarthAsynchronously;

		private string errorMessage = string.Empty;

		public override int BorderWidth
		{
			get
			{
				return base.BorderWidth;
			}
			set
			{
				if (base.BorderWidth != value)
				{
					base.BorderWidth = value;
					InvalidateAndLayout();
				}
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				base.Visible = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public override int ZOrder
		{
			get
			{
				return int.MinValue;
			}
			set
			{
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeViewport_AutoSize")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(true)]
		public bool AutoSize
		{
			get
			{
				return autoSize;
			}
			set
			{
				if (autoSize != value)
				{
					autoSize = value;
					Location.Docked = AutoSize;
					Size.AutoSize = AutoSize;
					InvalidateAndLayout();
				}
			}
		}

		[SRCategory("CategoryAttribute_MapContent")]
		[SRDescription("DescriptionAttributeViewport_ContentSize")]
		[TypeConverter(typeof(IntAutoFitConverter))]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(0)]
		public int ContentSize
		{
			get
			{
				return contentSize;
			}
			set
			{
				contentSize = value;
				GetMapCore()?.InvalidateCachedPaths();
				InvalidateDistanceScalePanel();
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_MapContent")]
		[SRDescription("DescriptionAttributeViewport_ContentAutoFitMargin")]
		[NotifyParentProperty(true)]
		[DefaultValue(10)]
		public int ContentAutoFitMargin
		{
			get
			{
				return contentAutoFitMargin;
			}
			set
			{
				contentAutoFitMargin = value;
				GetMapCore()?.InvalidateCachedPaths();
				InvalidateDistanceScalePanel();
				InvalidateViewport();
			}
		}

		[DefaultValue(0)]
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

		[Browsable(false)]
		[SRCategory("CategoryAttribute_Interactivity")]
		[SRDescription("DescriptionAttributeViewport_EnablePanning")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(false)]
		public bool EnablePanning
		{
			get
			{
				return enablePanning;
			}
			set
			{
				enablePanning = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Zooming")]
		[SRDescription("DescriptionAttributeViewport_MinimumZoom")]
		[NotifyParentProperty(true)]
		[DefaultValue(20)]
		public int MinimumZoom
		{
			get
			{
				return minimumZoom;
			}
			set
			{
				if (minimumZoom != value)
				{
					MapCore mapCore = GetMapCore();
					minimumZoom = value;
					if (mapCore != null && mapCore.ZoomPanel != null)
					{
						mapCore.ZoomPanel.UpdateZoomRange();
					}
					Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_Zooming")]
		[SRDescription("DescriptionAttributeViewport_MaximumZoom")]
		[NotifyParentProperty(true)]
		[DefaultValue(20000)]
		public int MaximumZoom
		{
			get
			{
				return maximumZoom;
			}
			set
			{
				if (maximumZoom != value)
				{
					if (value > 5000000)
					{
						throw new ArgumentException(SR.ExceptionMaximumZoomtooLarge(5000000), "MaximumZoom");
					}
					MapCore mapCore = GetMapCore();
					maximumZoom = value;
					if (mapCore != null && mapCore.ZoomPanel != null)
					{
						mapCore.ZoomPanel.UpdateZoomRange();
					}
					Invalidate();
				}
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttribute_Zooming")]
		[SRDescription("DescriptionAttributeViewport_Zoom")]
		[NotifyParentProperty(true)]
		[DefaultValue(100f)]
		public float Zoom
		{
			get
			{
				return zoom;
			}
			set
			{
				zoom = value;
				if (zoom > (float)MaximumZoom)
				{
					zoom = MaximumZoom;
				}
				else if (zoom < (float)MinimumZoom)
				{
					zoom = MinimumZoom;
				}
				MapCore mapCore = GetMapCore();
				if (mapCore != null && mapCore.ZoomPanel != null)
				{
					mapCore.ZoomPanel.ZoomLevel = value;
				}
				MapCore mapCore2 = GetMapCore();
				if (mapCore2 != null)
				{
					mapCore2.EnsureContentIsVisible();
					mapCore2.InvalidateCachedPaths();
				}
				InvalidateDistanceScalePanel();
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_View")]
		[SRDescription("DescriptionAttributeViewport_ViewCenter")]
		[TypeConverter(typeof(ViewCenterConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		public ViewCenter ViewCenter
		{
			get
			{
				return viewCenter;
			}
			set
			{
				viewCenter = value;
				viewCenter.Parent = this;
				InvalidateDistanceScalePanel();
				InvalidateViewport(invalidateGridSections: false);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttribute_Interactivity")]
		[SRDescription("DescriptionAttributeViewport_OptimizeForPanning")]
		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		public bool OptimizeForPanning
		{
			get
			{
				return optimizeForPanning;
			}
			set
			{
				optimizeForPanning = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Interactivity")]
		[SRDescription("DescriptionAttributeViewport_LoadTilesAsynchronously")]
		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		public bool LoadTilesAsynchronously
		{
			get
			{
				return loadTilesAsynchronously;
			}
			set
			{
				loadTilesAsynchronously = value;
			}
		}

		[SRCategory("CategoryAttribute_Interactivity")]
		[SRDescription("DescriptionAttributeViewport_QueryVirtualEarthAsynchronously")]
		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		public bool QueryVirtualEarthAsynchronously
		{
			get
			{
				return queryVirtualEarthAsynchronously;
			}
			set
			{
				queryVirtualEarthAsynchronously = value;
			}
		}

		[SRCategory("CategoryAttribute_MapContent")]
		[SRDescription("DescriptionAttributeViewport_ErrorMessage")]
		[DefaultValue("")]
		public string ErrorMessage
		{
			get
			{
				return errorMessage;
			}
			set
			{
				errorMessage = value;
				InvalidateViewport(invalidateGridSections: false);
			}
		}

		public Viewport()
			: this(null)
		{
		}

		internal Viewport(CommonElements common)
			: base(common)
		{
			Name = "Viewport";
			ViewCenter = new ViewCenter(this, 50f, 50f);
			BackShadowOffset = 0;
			Visible = true;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected bool ShouldSerializeViewCenter()
		{
			if (ViewCenter.X == 50f)
			{
				return ViewCenter.Y != 50f;
			}
			return true;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected void ResetViewCenter()
		{
			ViewCenter.X = 50f;
			ViewCenter.Y = 50f;
		}

		public PointF GetViewOrigin()
		{
			return new PointF(ViewCenter.X - 50f, ViewCenter.Y - 50f);
		}

		internal SizeF GetContentSizeInPixels()
		{
			SizeF result = default(SizeF);
			float num = (float)GetMapCore().CalculateAspectRatio();
			if (ContentSize == 0)
			{
				SizeF absoluteSize = GetAbsoluteSize();
				absoluteSize.Width -= BorderWidth * 2 + ContentAutoFitMargin * 2;
				absoluteSize.Height -= BorderWidth * 2 + ContentAutoFitMargin * 2;
				result.Width = absoluteSize.Width;
				result.Height = result.Width / num;
				if (absoluteSize.Height < result.Height)
				{
					result.Height = absoluteSize.Height;
					result.Width = result.Height * num;
				}
				result.Width = Math.Max(result.Width, 0f);
				result.Height = Math.Max(result.Height, 0f);
			}
			else
			{
				result.Width = ContentSize;
				result.Height = result.Width / num;
			}
			return result;
		}

		internal PointF GetContentOffsetInPixels()
		{
			SizeF contentSizeInPixels = GetContentSizeInPixels();
			PointF pointF = new PointF(ViewCenter.X, ViewCenter.Y);
			pointF.X *= contentSizeInPixels.Width / 100f;
			pointF.Y *= contentSizeInPixels.Height / 100f;
			pointF.X *= Zoom / 100f;
			pointF.Y *= Zoom / 100f;
			PointF locationInPixels = GetLocationInPixels();
			SizeF absoluteSize = GetAbsoluteSize();
			PointF pointF2 = new PointF(locationInPixels.X + absoluteSize.Width / 2f, locationInPixels.Y + absoluteSize.Height / 2f);
			return new PointF((int)(pointF2.X - pointF.X), (int)(pointF2.Y - pointF.Y));
		}

		internal void SetContentOffsetInPixels(PointF contentOffset)
		{
			PointF locationInPixels = GetLocationInPixels();
			SizeF absoluteSize = GetAbsoluteSize();
			PointF pointF = new PointF(locationInPixels.X + absoluteSize.Width / 2f, locationInPixels.Y + absoluteSize.Height / 2f);
			PointF pointF2 = new PointF(pointF.X - contentOffset.X, pointF.Y - contentOffset.Y);
			SizeF contentSizeInPixels = GetContentSizeInPixels();
			pointF2.X /= Zoom / 100f;
			pointF2.Y /= Zoom / 100f;
			pointF2.X /= contentSizeInPixels.Width / 100f;
			pointF2.Y /= contentSizeInPixels.Height / 100f;
			ViewCenter.X = pointF2.X;
			ViewCenter.Y = pointF2.Y;
		}

		public double GetGroundResolutionAtEquator()
		{
			double num = (GetMapCore().MaximumPoint.X - GetMapCore().MinimumPoint.X) / 360.0 * 2.0 * Math.PI * 6378137.0;
			float num2 = GetContentSizeInPixels().Width * Zoom / 100f;
			return num / (double)num2;
		}

		internal double GetGeographicResolutionAtEquator()
		{
			double num = GetMapCore().MaximumPoint.X - GetMapCore().MinimumPoint.X;
			float num2 = GetContentSizeInPixels().Width * Zoom / 100f;
			Projection projection = Common.MapCore.Projection;
			if (projection == Projection.Orthographic)
			{
				return num / (double)num2 / 2.0;
			}
			return num / (double)num2;
		}

		internal override void RenderBorder(MapGraphics g)
		{
			AntiAliasing antiAliasing = g.AntiAliasing;
			g.AntiAliasing = AntiAliasing.None;
			RectangleF rectangleF = new RectangleF(GetAbsoluteLocation(), GetAbsoluteSize());
			rectangleF.X = (float)Math.Round(rectangleF.X);
			rectangleF.Y = (float)Math.Round(rectangleF.Y);
			rectangleF.Width = (float)Math.Round(rectangleF.Width);
			rectangleF.Height = (float)Math.Round(rectangleF.Height);
			if (!(rectangleF.Width > 0f) || !(rectangleF.Height > 0f))
			{
				return;
			}
			try
			{
				if (BorderWidth <= 0 || BorderColor.IsEmpty || BorderStyle == MapDashStyle.None)
				{
					return;
				}
				using (Pen pen = new Pen(BorderColor, BorderWidth))
				{
					pen.DashStyle = MapGraphics.GetPenStyle(BorderStyle);
					pen.Alignment = PenAlignment.Inset;
					if (BorderWidth == 1)
					{
						rectangleF.Width -= 1f;
						rectangleF.Height -= 1f;
					}
					g.DrawRectangle(pen, rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height);
				}
			}
			finally
			{
				g.AntiAliasing = antiAliasing;
			}
		}

		internal override void Render(MapGraphics g)
		{
			AntiAliasing antiAliasing = g.AntiAliasing;
			g.AntiAliasing = AntiAliasing.None;
			RectangleF rectangleF = new RectangleF(GetAbsoluteLocation(), GetAbsoluteSize());
			rectangleF.X = (float)Math.Round(rectangleF.X);
			rectangleF.Y = (float)Math.Round(rectangleF.Y);
			rectangleF.Width = (float)Math.Round(rectangleF.Width);
			rectangleF.Height = (float)Math.Round(rectangleF.Height);
			if (!(rectangleF.Width > 0f) || !(rectangleF.Height > 0f))
			{
				return;
			}
			try
			{
				if (BackShadowOffset != 0)
				{
					RectangleF rect = rectangleF;
					rect.Offset(BackShadowOffset, BackShadowOffset);
					g.FillRectangle(g.GetShadowBrush(), rect);
				}
				using (Brush brush = g.CreateBrush(rectangleF, BackColor, BackHatchStyle, "", MapImageWrapMode.Unscaled, Color.Empty, MapImageAlign.Center, BackGradientType, BackSecondaryColor))
				{
					g.FillRectangle(brush, rectangleF);
				}
			}
			finally
			{
				g.AntiAliasing = antiAliasing;
			}
		}

		internal override void SizeLocationChanged(SizeLocationChangeInfo info)
		{
			base.SizeLocationChanged(info);
			switch (info)
			{
			case SizeLocationChangeInfo.Location:
				Location.Docked = AutoSize;
				break;
			case SizeLocationChangeInfo.Size:
				Size.AutoSize = AutoSize;
				break;
			}
			GetMapCore()?.InvalidateCachedPaths();
			InvalidateAndLayout();
		}

		internal override object GetDefaultPropertyValue(string prop, object currentValue)
		{
			object obj = null;
			switch (prop)
			{
			case "Margins":
				return new PanelMargins(0, 0, 0, 0);
			case "Location":
				return new MapLocation(null, 0f, 0f);
			case "Size":
				return new MapSize(null, 100f, 100f);
			case "SizeUnit":
				return CoordinateUnit.Percent;
			case "BackColor":
				return Color.White;
			case "BorderColor":
				return Color.DarkGray;
			case "BorderWidth":
				return 0;
			case "BackGradientType":
				return GradientType.None;
			default:
				return base.GetDefaultPropertyValue(prop, currentValue);
			}
		}
	}
}

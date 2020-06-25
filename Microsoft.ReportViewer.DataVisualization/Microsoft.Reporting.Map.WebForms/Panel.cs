using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class Panel : NamedElement, IToolTipProvider, IZOrderedObject, ISelectable, IDefaultValueProvider, IImageMapProvider
	{
		private const int DefaultMarginsAllValues = 5;

		private int zOrder;

		private PanelMargins margins;

		private MapLocation location;

		private CoordinateUnit locationUnit = CoordinateUnit.Percent;

		private MapSize size;

		private CoordinateUnit sizeUnit = CoordinateUnit.Percent;

		private bool visible;

		private Color borderColor;

		private MapDashStyle borderStyle = MapDashStyle.Solid;

		private int borderWidth;

		private Color backColor;

		private GradientType backGradientType;

		private Color backSecondaryColor = Color.Empty;

		private MapHatchStyle backHatchStyle;

		private int backShadowOffset = 2;

		private bool selected;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private object mapAreaTag;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[TypeConverter(typeof(StringConverter))]
		[DefaultValue(null)]
		public override object Tag
		{
			get
			{
				return base.Tag;
			}
			set
			{
				base.Tag = value;
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributePanel_ZOrder")]
		[DefaultValue(0)]
		public virtual int ZOrder
		{
			get
			{
				return zOrder;
			}
			set
			{
				if (zOrder != value)
				{
					zOrder = value;
					Invalidate();
					SizeLocationChanged(SizeLocationChangeInfo.ZOrder);
				}
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributePanel_Margins")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelMargins Margins
		{
			get
			{
				return margins;
			}
			set
			{
				if (margins == null || !margins.Equals(value))
				{
					margins = value;
					margins.Owner = this;
					Invalidate();
				}
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributePanel_Location")]
		[TypeConverter(typeof(LocationConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual MapLocation Location
		{
			get
			{
				return location;
			}
			set
			{
				location = value;
				location.Parent = this;
				Invalidate();
				SizeLocationChanged(SizeLocationChangeInfo.Location);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributePanel_LocationUnit")]
		public virtual CoordinateUnit LocationUnit
		{
			get
			{
				return locationUnit;
			}
			set
			{
				locationUnit = value;
				Invalidate();
				SizeLocationChanged(SizeLocationChangeInfo.LocationUnit);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributePanel_Size")]
		[TypeConverter(typeof(SizeConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual MapSize Size
		{
			get
			{
				return size;
			}
			set
			{
				size = value;
				size.Parent = this;
				Invalidate();
				SizeLocationChanged(SizeLocationChangeInfo.Size);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributePanel_SizeUnit")]
		public virtual CoordinateUnit SizeUnit
		{
			get
			{
				return sizeUnit;
			}
			set
			{
				sizeUnit = value;
				Invalidate();
				SizeLocationChanged(SizeLocationChangeInfo.SizeUnit);
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePanel_Visible")]
		[ParenthesizePropertyName(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(false)]
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
		[SRDescription("DescriptionAttributePanel_BorderColor")]
		[NotifyParentProperty(true)]
		public virtual Color BorderColor
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
		[SRDescription("DescriptionAttributePanel_BorderStyle")]
		[NotifyParentProperty(true)]
		[DefaultValue(MapDashStyle.Solid)]
		public virtual MapDashStyle BorderStyle
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
		[SRDescription("DescriptionAttributePanel_BorderWidth")]
		[NotifyParentProperty(true)]
		public virtual int BorderWidth
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
		[SRDescription("DescriptionAttributePanel_BackColor")]
		[NotifyParentProperty(true)]
		public virtual Color BackColor
		{
			get
			{
				return backColor;
			}
			set
			{
				backColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePanel_BackGradientType")]
		[NotifyParentProperty(true)]
		public virtual GradientType BackGradientType
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePanel_BackSecondaryColor")]
		[NotifyParentProperty(true)]
		public virtual Color BackSecondaryColor
		{
			get
			{
				return backSecondaryColor;
			}
			set
			{
				backSecondaryColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePanel_BackHatchStyle")]
		[NotifyParentProperty(true)]
		public virtual MapHatchStyle BackHatchStyle
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePanel_BackShadowOffset")]
		[NotifyParentProperty(true)]
		[DefaultValue(2)]
		public virtual int BackShadowOffset
		{
			get
			{
				return backShadowOffset;
			}
			set
			{
				if (value < -100 || value > 100)
				{
					throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
				}
				backShadowOffset = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributePanel_Selected")]
		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		public bool Selected
		{
			get
			{
				return selected;
			}
			set
			{
				selected = value;
				Invalidate();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributePanel_ToolTip")]
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
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributePanel_Href")]
		[Localizable(true)]
		[DefaultValue("")]
		public virtual string Href
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

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributePanel_MapAreaAttributes")]
		[DefaultValue("")]
		public virtual string MapAreaAttributes
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

		protected bool ShouldSerializeMargins()
		{
			PanelMargins panelMargins = (PanelMargins)GetDefaultPropertyValue("Margins", Margins);
			if (Margins.Top == panelMargins.Top && Margins.Bottom == panelMargins.Bottom && Margins.Left == panelMargins.Left && Margins.Right == panelMargins.Right)
			{
				return false;
			}
			return true;
		}

		protected void ResetMargins()
		{
			PanelMargins panelMargins = (PanelMargins)GetDefaultPropertyValue("Margins", Margins);
			margins.Top = panelMargins.Top;
			margins.Bottom = panelMargins.Bottom;
			margins.Left = panelMargins.Left;
			margins.Right = panelMargins.Right;
		}

		protected void ResetLocation()
		{
			MapLocation mapLocation = (MapLocation)GetDefaultPropertyValue("Location", Location);
			Location.X = mapLocation.X;
			Location.Y = mapLocation.Y;
		}

		protected bool ShouldSerializeLocation()
		{
			MapLocation mapLocation = (MapLocation)GetDefaultPropertyValue("Location", Location);
			if (Location.X == mapLocation.X)
			{
				return Location.Y != mapLocation.Y;
			}
			return true;
		}

		protected void ResetLocationUnit()
		{
			LocationUnit = (CoordinateUnit)GetDefaultPropertyValue("LocationUnit", LocationUnit);
		}

		protected bool ShouldSerializeLocationUnit()
		{
			return !LocationUnit.Equals(GetDefaultPropertyValue("LocationUnit", LocationUnit));
		}

		protected void ResetSize()
		{
			MapSize mapSize = (MapSize)GetDefaultPropertyValue("Size", Size);
			Size.Width = mapSize.Width;
			Size.Height = mapSize.Height;
		}

		protected bool ShouldSerializeSize()
		{
			MapSize mapSize = (MapSize)GetDefaultPropertyValue("Size", Size);
			if (Size.Width == mapSize.Width)
			{
				return Size.Height != mapSize.Height;
			}
			return true;
		}

		protected void ResetSizeUnit()
		{
			SizeUnit = (CoordinateUnit)GetDefaultPropertyValue("SizeUnit", SizeUnit);
		}

		protected bool ShouldSerializeSizeUnit()
		{
			return !SizeUnit.Equals(GetDefaultPropertyValue("SizeUnit", SizeUnit));
		}

		protected void ResetBorderColor()
		{
			Color color2 = BorderColor = (Color)GetDefaultPropertyValue("BorderColor", BorderColor);
		}

		protected bool ShouldSerializeBorderColor()
		{
			Color right = (Color)GetDefaultPropertyValue("BorderColor", BorderColor);
			return BorderColor != right;
		}

		protected void ResetBorderWidth()
		{
			int num2 = BorderWidth = (int)GetDefaultPropertyValue("BorderWidth", BorderWidth);
		}

		protected bool ShouldSerializeBorderWidth()
		{
			int num = (int)GetDefaultPropertyValue("BorderWidth", BorderWidth);
			return BorderWidth != num;
		}

		protected void ResetBackColor()
		{
			Color color2 = BackColor = (Color)GetDefaultPropertyValue("BackColor", BackColor);
		}

		protected bool ShouldSerializeBackColor()
		{
			Color right = (Color)GetDefaultPropertyValue("BackColor", BackColor);
			return BackColor != right;
		}

		protected void ResetBackGradientType()
		{
			GradientType gradientType2 = BackGradientType = (GradientType)GetDefaultPropertyValue("BackGradientType", BackGradientType);
		}

		protected bool ShouldSerializeBackGradientType()
		{
			GradientType gradientType = (GradientType)GetDefaultPropertyValue("BackGradientType", BackGradientType);
			return BackGradientType != gradientType;
		}

		protected void ResetBackSecondaryColor()
		{
			Color color2 = BackSecondaryColor = (Color)GetDefaultPropertyValue("BackSecondaryColor", BackSecondaryColor);
		}

		protected bool ShouldSerializeBackSecondaryColor()
		{
			Color right = (Color)GetDefaultPropertyValue("BackSecondaryColor", BackSecondaryColor);
			return BackSecondaryColor != right;
		}

		protected void ResetBackHatchStyle()
		{
			MapHatchStyle mapHatchStyle2 = BackHatchStyle = (MapHatchStyle)GetDefaultPropertyValue("BackHatchStyle", BackHatchStyle);
		}

		protected bool ShouldSerializeBackHatchStyle()
		{
			MapHatchStyle mapHatchStyle = (MapHatchStyle)GetDefaultPropertyValue("BackHatchStyle", BackHatchStyle);
			return BackHatchStyle != mapHatchStyle;
		}

		public Panel()
			: this(null)
		{
		}

		internal Panel(CommonElements common)
			: base(common)
		{
			Margins = new PanelMargins((PanelMargins)GetDefaultPropertyValue("Margins", null));
			Location = new MapLocation((MapLocation)GetDefaultPropertyValue("Location", null));
			LocationUnit = (CoordinateUnit)GetDefaultPropertyValue("LocationUnit", null);
			Size = new MapSize((MapSize)GetDefaultPropertyValue("Size", null));
			SizeUnit = (CoordinateUnit)GetDefaultPropertyValue("SizeUnit", null);
			BackColor = (Color)GetDefaultPropertyValue("BackColor", null);
			BorderColor = (Color)GetDefaultPropertyValue("BorderColor", null);
			BorderWidth = (int)GetDefaultPropertyValue("BorderWidth", null);
			BackGradientType = (GradientType)GetDefaultPropertyValue("BackGradientType", null);
			BackHatchStyle = (MapHatchStyle)GetDefaultPropertyValue("BackHatchStyle", null);
			BackSecondaryColor = (Color)GetDefaultPropertyValue("BackSecondaryColor", null);
		}

		public SizeF GetSizeInPixels()
		{
			SizeF result = default(SizeF);
			if (SizeUnit == CoordinateUnit.Pixel)
			{
				result.Width = Size.Width;
				result.Height = Size.Height;
			}
			else
			{
				MapCore mapCore = GetMapCore();
				if (mapCore == null)
				{
					return result;
				}
				result.Width = (float)mapCore.Width * Size.Width / 100f;
				result.Height = (float)mapCore.Height * Size.Height / 100f;
			}
			return result;
		}

		public RectangleF GetBoundsInPixels()
		{
			return new RectangleF(GetLocationInPixels(), GetSizeInPixels());
		}

		public void SetLocationInPixels(PointF location)
		{
			MapCore mapCore = GetMapCore();
			if (mapCore != null)
			{
				if (LocationUnit == CoordinateUnit.Pixel)
				{
					Location = new MapLocation(this, location.X, location.Y);
					return;
				}
				float num = 100f * location.X / (float)(mapCore.Width - 1);
				num = ((!float.IsNaN(num)) ? num : 0f);
				float num2 = 100f * location.Y / (float)(mapCore.Height - 1);
				num2 = ((!float.IsNaN(num2)) ? num2 : 0f);
				Location = new MapLocation(this, num, num2);
			}
		}

		public void SetSizeInPixels(SizeF size)
		{
			MapCore mapCore = GetMapCore();
			if (mapCore != null)
			{
				if (SizeUnit == CoordinateUnit.Pixel)
				{
					Size = new MapSize(this, size.Width, size.Height);
					return;
				}
				float num = 100f * size.Width / (float)mapCore.Width;
				num = ((!float.IsNaN(num) && !float.IsInfinity(num)) ? num : 0f);
				float num2 = 100f * size.Height / (float)mapCore.Height;
				num2 = ((!float.IsNaN(num2) && !float.IsInfinity(num2)) ? num2 : 0f);
				Size = new MapSize(this, num, num2);
			}
		}

		public void SetBoundsInPixels(RectangleF bounds)
		{
			SetLocationInPixels(bounds.Location);
			SetSizeInPixels(bounds.Size);
		}

		public PointF GetLocationInPixels()
		{
			PointF result = default(PointF);
			if (LocationUnit == CoordinateUnit.Pixel)
			{
				result.X = Location.X;
				result.Y = Location.Y;
			}
			else
			{
				MapCore mapCore = GetMapCore();
				if (mapCore == null)
				{
					return result;
				}
				result.X = (float)(mapCore.Width - 1) * Location.X / 100f;
				result.Y = (float)(mapCore.Height - 1) * Location.Y / 100f;
			}
			return result;
		}

		public virtual RectangleF GetBoundRect(MapGraphics g)
		{
			RectangleF result = default(RectangleF);
			if (LocationUnit == CoordinateUnit.Percent)
			{
				result.Location = Location;
			}
			else
			{
				result.Location = g.GetRelativePoint(Location);
			}
			if (SizeUnit == CoordinateUnit.Percent)
			{
				result.Size = Size;
			}
			else
			{
				result.Size = g.GetRelativeSize(Size);
			}
			return result;
		}

		internal virtual GraphicsPath GetHotRegionPath(MapGraphics g)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			RectangleF rect = new RectangleF(GetAbsoluteLocation(), GetAbsoluteSize());
			graphicsPath.AddRectangle(rect);
			return graphicsPath;
		}

		internal void SizeChanged(MapSize size)
		{
			SizeLocationChanged(SizeLocationChangeInfo.Size);
		}

		internal virtual void LocationChanged(MapLocation size)
		{
			SizeLocationChanged(SizeLocationChangeInfo.Location);
		}

		internal virtual void Render(MapGraphics g)
		{
		}

		internal virtual bool ShouldRenderBackground()
		{
			return true;
		}

		internal virtual void RenderBackground(MapGraphics g)
		{
			AntiAliasing antiAliasing = g.AntiAliasing;
			g.AntiAliasing = AntiAliasing.None;
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			absoluteRectangle.X = (float)Math.Round(absoluteRectangle.X);
			absoluteRectangle.Y = (float)Math.Round(absoluteRectangle.Y);
			absoluteRectangle.Width = (float)Math.Round(absoluteRectangle.Width);
			absoluteRectangle.Height = (float)Math.Round(absoluteRectangle.Height);
			try
			{
				if (BackShadowOffset != 0)
				{
					RectangleF rect = absoluteRectangle;
					rect.Offset(BackShadowOffset, BackShadowOffset);
					g.FillRectangle(g.GetShadowBrush(), rect);
				}
				if (IsMakeTransparentRequired())
				{
					using (Brush brush = new SolidBrush(GetColorForMakeTransparent()))
					{
						g.FillRectangle(brush, absoluteRectangle);
					}
				}
				using (Brush brush2 = g.CreateBrush(absoluteRectangle, BackColor, BackHatchStyle, "", MapImageWrapMode.Unscaled, Color.Empty, MapImageAlign.Center, BackGradientType, BackSecondaryColor))
				{
					g.FillRectangle(brush2, absoluteRectangle);
				}
			}
			finally
			{
				g.AntiAliasing = antiAliasing;
			}
		}

		internal virtual void RenderBorder(MapGraphics g)
		{
			AntiAliasing antiAliasing = g.AntiAliasing;
			g.AntiAliasing = AntiAliasing.None;
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			absoluteRectangle.X = (float)Math.Round(absoluteRectangle.X);
			absoluteRectangle.Y = (float)Math.Round(absoluteRectangle.Y);
			absoluteRectangle.Width = (float)Math.Round(absoluteRectangle.Width);
			absoluteRectangle.Height = (float)Math.Round(absoluteRectangle.Height);
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
						absoluteRectangle.Width -= 1f;
						absoluteRectangle.Height -= 1f;
					}
					g.DrawRectangle(pen, absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height);
				}
			}
			finally
			{
				g.AntiAliasing = antiAliasing;
			}
		}

		internal virtual void SizeLocationChanged(SizeLocationChangeInfo info)
		{
		}

		internal void RenderPanel(MapGraphics g)
		{
			if (!IsVisible())
			{
				return;
			}
			try
			{
				RectangleF relativeRectangle = g.GetRelativeRectangle(Margins.AdjustRectangle(GetBoundsInPixels()));
				g.CreateDrawRegion(relativeRectangle);
				SizeF absoluteSize = g.GetAbsoluteSize(new SizeF(100f, 100f));
				if (absoluteSize.Width < 1f || absoluteSize.Height < 1f)
				{
					return;
				}
				if (ShouldRenderBackground() && GetMapCore().RenderingMode != RenderingMode.ZoomThumb)
				{
					RenderBackground(g);
					RenderBorder(g);
				}
				if (BorderWidth > 0 && ShouldRenderBackground())
				{
					try
					{
						RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
						absoluteRectangle.Inflate(-BorderWidth, -BorderWidth);
						absoluteRectangle.Width = Math.Max(2f, absoluteRectangle.Width);
						absoluteRectangle.Height = Math.Max(2f, absoluteRectangle.Height);
						g.CreateDrawRegion(g.GetRelativeRectangle(absoluteRectangle));
						Render(g);
					}
					finally
					{
						g.RestoreDrawRegion();
					}
				}
				else
				{
					Render(g);
				}
			}
			finally
			{
				g.RestoreDrawRegion();
			}
		}

		internal virtual bool IsRenderVisible(MapGraphics g, RectangleF clipRect)
		{
			if (!IsVisible())
			{
				return false;
			}
			return clipRect.IntersectsWith(GetBoundsInPixels());
		}

		internal PointF GetAbsoluteLocation()
		{
			PointF locationInPixels = GetLocationInPixels();
			locationInPixels.X += Margins.Left;
			locationInPixels.Y += Margins.Top;
			return locationInPixels;
		}

		internal SizeF GetAbsoluteSize()
		{
			SizeF sizeInPixels = GetSizeInPixels();
			sizeInPixels.Width -= Margins.Left + Margins.Right;
			sizeInPixels.Height -= Margins.Top + Margins.Bottom;
			return sizeInPixels;
		}

		internal virtual object GetDefaultPropertyValue(string prop, object currentValue)
		{
			object result = null;
			switch (prop)
			{
			case "Margins":
				result = new PanelMargins(5, 5, 5, 5);
				break;
			case "Location":
				result = new MapLocation(null, 0f, 0f);
				break;
			case "LocationUnit":
				result = CoordinateUnit.Percent;
				break;
			case "Size":
				result = new MapSize(null, 200f, 100f);
				break;
			case "SizeUnit":
				result = CoordinateUnit.Pixel;
				break;
			case "BackColor":
				result = Color.White;
				break;
			case "BorderColor":
				result = Color.DarkGray;
				break;
			case "BorderWidth":
				result = 1;
				break;
			case "BackGradientType":
				result = GradientType.DiagonalLeft;
				break;
			case "BackHatchStyle":
				result = MapHatchStyle.None;
				break;
			case "BackSecondaryColor":
				result = Color.FromArgb(230, 230, 230);
				break;
			}
			return result;
		}

		internal virtual bool IsVisible()
		{
			return Visible;
		}

		internal bool IsMakeTransparentRequired()
		{
			if (GetMapCore().RenderingMode != RenderingMode.SinglePanel)
			{
				return false;
			}
			if ((this is Legend || this is ColorSwatchPanel || this is DistanceScalePanel || this is MapLabel) && ((BackColor.A == 0 && BackGradientType == GradientType.None) || (BackColor.A == 0 && BackSecondaryColor.A == 0)))
			{
				return true;
			}
			return false;
		}

		internal Color GetColorForMakeTransparent()
		{
			MapCore mapCore = GetMapCore();
			RectangleF rectangleF = new RectangleF(mapCore.Viewport.GetLocationInPixels(), mapCore.Viewport.GetSizeInPixels());
			RectangleF rect = new RectangleF(mapCore.Viewport.GetLocationInPixels(), mapCore.Viewport.GetSizeInPixels());
			if (rectangleF.Contains(rect) && mapCore.Viewport.BackColor.A != 0)
			{
				return Color.FromArgb(255, mapCore.Viewport.BackColor);
			}
			return Color.FromArgb(255, mapCore.MapControl.BackColor);
		}

		protected MapCore GetMapCore()
		{
			if (Common != null)
			{
				return Common.MapCore;
			}
			return null;
		}

		int IZOrderedObject.GetZOrder()
		{
			return ZOrder;
		}

		void ISelectable.DrawSelection(MapGraphics g, RectangleF clipRect, bool designTimeSelection)
		{
			MapCore mapCore = GetMapCore();
			if (mapCore != null && IsVisible())
			{
				RectangleF selectionRectangle = ((ISelectable)this).GetSelectionRectangle(g, clipRect);
				if (!selectionRectangle.IsEmpty)
				{
					g.DrawSelection(selectionRectangle, designTimeSelection, mapCore.SelectionBorderColor, mapCore.SelectionMarkerColor);
				}
			}
		}

		public virtual RectangleF GetSelectionRectangle(MapGraphics g, RectangleF clipRect)
		{
			return new RectangleF(GetAbsoluteLocation(), GetAbsoluteSize());
		}

		bool ISelectable.IsSelected()
		{
			return Selected;
		}

		bool ISelectable.IsVisible()
		{
			return IsVisible();
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

		object IDefaultValueProvider.GetDefaultValue(string prop, object currentValue)
		{
			return GetDefaultPropertyValue(prop, currentValue);
		}
	}
}

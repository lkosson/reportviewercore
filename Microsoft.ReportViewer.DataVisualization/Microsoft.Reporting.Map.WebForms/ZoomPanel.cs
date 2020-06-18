using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class ZoomPanel : DockablePanel, IToolTipProvider
	{
		private const double MaxScaleValue = 100.0;

		private PanelButton zoomInButton;

		private PanelButton zoomOutButton;

		private bool internalZoomChange;

		private bool fixThumbPoition;

		private ZoomPanelStyle panelStyle = ZoomPanelStyle.CircularButtons;

		private Orientation orientation = Orientation.Auto;

		private ZoomType zoomType = ZoomType.Exponential;

		private bool zoomButtonsVisible = true;

		private Color symbolColor = Color.LightGray;

		private Color symbolBorderColor = Color.DimGray;

		private Color buttonBorderColor = Color.DarkGray;

		private Color buttonColor = Color.White;

		private Color thumbBorderColor = Color.Gray;

		private Color thumbColor = Color.White;

		private Color sliderBarBorderColor = Color.Silver;

		private Color sliderBarColor = Color.White;

		private Color tickBorderColor = Color.DarkGray;

		private Color tickColor = Color.White;

		private int tickCount = 10;

		private bool snapToTickMarks;

		private LinearScale scale;

		private LinearPointer pointer;

		private SizeF absoluteSize = SizeF.Empty;

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeZoomPanel_PanelStyle")]
		[DefaultValue(ZoomPanelStyle.CircularButtons)]
		public ZoomPanelStyle PanelStyle
		{
			get
			{
				return panelStyle;
			}
			set
			{
				if (panelStyle != value)
				{
					panelStyle = value;
					ApplyStyle();
					Invalidate();
				}
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeZoomPanel_Orientation")]
		[DefaultValue(Orientation.Auto)]
		public Orientation Orientation
		{
			get
			{
				return orientation;
			}
			set
			{
				if (GetOrientation() != value && value != Orientation.Auto)
				{
					MapSize mapSize2 = Size = new MapSize(this, Size.Height, Size.Width);
				}
				orientation = value;
				AdjustAutoOrientationForDocking(Dock);
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeZoomPanel_Reversed")]
		[DefaultValue(false)]
		public bool Reversed
		{
			get
			{
				return Scale.Reversed;
			}
			set
			{
				Scale.Reversed = value;
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeZoomPanel_ZoomType")]
		[DefaultValue(ZoomType.Exponential)]
		public ZoomType ZoomType
		{
			get
			{
				return zoomType;
			}
			set
			{
				if (zoomType != value)
				{
					zoomType = value;
					if (Common != null && Common.MapCore != null)
					{
						Common.MapCore.Viewport.Zoom = (float)GetZoomLevelFromPointerPosition(Pointer.Value);
					}
				}
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeZoomPanel_Dock")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(PanelDockStyle.Left)]
		public override PanelDockStyle Dock
		{
			get
			{
				return base.Dock;
			}
			set
			{
				AdjustAutoOrientationForDocking(value);
				base.Dock = value;
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeZoomPanel_ZoomButtonsVisible")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(true)]
		public bool ZoomButtonsVisible
		{
			get
			{
				return zoomButtonsVisible;
			}
			set
			{
				zoomButtonsVisible = value;
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeZoomPanel_SymbolColor")]
		[DefaultValue(typeof(Color), "LightGray")]
		public Color SymbolColor
		{
			get
			{
				return symbolColor;
			}
			set
			{
				symbolColor = value;
				ApplyColors();
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeZoomPanel_SymbolBorderColor")]
		[DefaultValue(typeof(Color), "DimGray")]
		public Color SymbolBorderColor
		{
			get
			{
				return symbolBorderColor;
			}
			set
			{
				symbolBorderColor = value;
				ApplyColors();
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeZoomPanel_ButtonBorderColor")]
		[DefaultValue(typeof(Color), "DarkGray")]
		public Color ButtonBorderColor
		{
			get
			{
				return buttonBorderColor;
			}
			set
			{
				buttonBorderColor = value;
				ApplyColors();
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeZoomPanel_ButtonColor")]
		[DefaultValue(typeof(Color), "White")]
		public Color ButtonColor
		{
			get
			{
				return buttonColor;
			}
			set
			{
				buttonColor = value;
				ApplyColors();
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeZoomPanel_ThumbBorderColor")]
		[DefaultValue(typeof(Color), "Gray")]
		public Color ThumbBorderColor
		{
			get
			{
				return thumbBorderColor;
			}
			set
			{
				thumbBorderColor = value;
				ApplyColors();
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeZoomPanel_ThumbColor")]
		[DefaultValue(typeof(Color), "White")]
		public Color ThumbColor
		{
			get
			{
				return thumbColor;
			}
			set
			{
				thumbColor = value;
				ApplyColors();
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeZoomPanel_SliderBarBorderColor")]
		[DefaultValue(typeof(Color), "Silver")]
		public Color SliderBarBorderColor
		{
			get
			{
				return sliderBarBorderColor;
			}
			set
			{
				sliderBarBorderColor = value;
				ApplyColors();
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeZoomPanel_SliderBarColor")]
		[DefaultValue(typeof(Color), "White")]
		public Color SliderBarColor
		{
			get
			{
				return sliderBarColor;
			}
			set
			{
				sliderBarColor = value;
				ApplyColors();
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeZoomPanel_TickBorderColor")]
		[DefaultValue(typeof(Color), "DarkGray")]
		public Color TickBorderColor
		{
			get
			{
				return tickBorderColor;
			}
			set
			{
				tickBorderColor = value;
				ApplyColors();
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeZoomPanel_TickColor")]
		[DefaultValue(typeof(Color), "White")]
		public Color TickColor
		{
			get
			{
				return tickColor;
			}
			set
			{
				tickColor = value;
				ApplyColors();
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeZoomPanel_TickCount")]
		[DefaultValue(10)]
		public int TickCount
		{
			get
			{
				return tickCount;
			}
			set
			{
				if (value < 2)
				{
					throw new ArgumentOutOfRangeException(SR.ticknumber_out_of_range);
				}
				if (tickCount != value)
				{
					tickCount = value;
					double tickMarksInterval = GetTickMarksInterval(value);
					Scale.MinorTickMark.Interval = tickMarksInterval;
					Pointer.SnappingInterval = tickMarksInterval;
				}
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeZoomPanel_SnapToTickMarks")]
		[DefaultValue(false)]
		public bool SnapToTickMarks
		{
			get
			{
				return snapToTickMarks;
			}
			set
			{
				snapToTickMarks = value;
				Pointer.SnappingEnabled = value;
			}
		}

		public override string ToolTip
		{
			get
			{
				return base.ToolTip;
			}
			set
			{
				base.ToolTip = value;
				Scale.ToolTip = value;
				Pointer.ToolTip = value;
			}
		}

		internal LinearScale Scale => scale;

		internal LinearPointer Pointer
		{
			get
			{
				return pointer;
			}
			set
			{
				pointer = value;
			}
		}

		internal SizeF AbsoluteSize
		{
			get
			{
				return absoluteSize;
			}
			set
			{
				absoluteSize = value;
			}
		}

		internal Position Position => new Position(Location, Size, ContentAlignment.TopLeft);

		internal double MinimumZoom
		{
			get
			{
				if (Common != null && Common.MapCore != null)
				{
					return Common.MapCore.Viewport.MinimumZoom;
				}
				return 50.0;
			}
		}

		internal double MaximumZoom
		{
			get
			{
				if (Common != null && Common.MapCore != null)
				{
					return Common.MapCore.Viewport.MaximumZoom;
				}
				return 1000.0;
			}
		}

		internal double ZoomLevel
		{
			get
			{
				if (Common != null && Common.MapCore != null)
				{
					return Common.MapCore.Viewport.Zoom;
				}
				return 100.0;
			}
			set
			{
				if (!internalZoomChange)
				{
					try
					{
						fixThumbPoition = true;
						double pointerPositionFromZoomLevel = GetPointerPositionFromZoomLevel(value);
						Pointer.Value = pointerPositionFromZoomLevel;
					}
					finally
					{
						fixThumbPoition = false;
					}
				}
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
				if (Scale != null)
				{
					Scale.Common = value;
				}
				if (Pointer != null)
				{
					Pointer.Common = value;
				}
			}
		}

		public ZoomPanel()
			: this(null)
		{
		}

		internal ZoomPanel(CommonElements common)
			: base(common)
		{
			Name = "ZoomPanel";
			scale = new LinearScale(this);
			pointer = new LinearPointer(this);
			MapCore mapCore = GetMapCore();
			Scale.LabelStyle.Visible = false;
			Scale.MajorTickMark.Visible = false;
			Scale.MinorTickMark.Length = 30f;
			Scale.MinorTickMark.Width = 10f;
			Scale.MinorTickMark.EnableGradient = false;
			Scale.FillGradientType = GradientType.None;
			Scale.FillHatchStyle = MapHatchStyle.None;
			Scale.ShadowOffset = 0f;
			Scale.BorderWidth = 1;
			Scale.Width = 15f;
			Scale.Minimum = 0.0;
			Scale.Maximum = 100.00000000001;
			double tickMarksInterval = GetTickMarksInterval(TickCount);
			Scale.MinorTickMark.Interval = tickMarksInterval;
			Pointer.Placement = Placement.Cross;
			if (mapCore != null && mapCore.Viewport != null)
			{
				Pointer.Position = mapCore.Viewport.Zoom;
			}
			Pointer.SnappingEnabled = true;
			Pointer.SnappingInterval = tickMarksInterval;
			Pointer.FillGradientType = GradientType.None;
			Pointer.FillHatchStyle = MapHatchStyle.None;
			Pointer.ShadowOffset = 0f;
			if (mapCore != null && mapCore.Viewport != null)
			{
				ZoomLevel = mapCore.Viewport.Zoom;
			}
			zoomInButton = new PanelButton(this, PanelButtonType.ZoomButton, PanelButtonStyle.RoundedRectangle, zoomButtonClickHandler);
			zoomOutButton = new PanelButton(this, PanelButtonType.ZoomOut, PanelButtonStyle.RoundedRectangle, zoomButtonClickHandler);
			ApplyStyle();
			ApplyColors();
		}

		public float GetThumbPosition()
		{
			return (float)Pointer.Value;
		}

		public void SetThumbPosition(float thumbPosition)
		{
			thumbPosition = Math.Min(100f, thumbPosition);
			thumbPosition = Math.Max(0f, thumbPosition);
			Pointer.Value = thumbPosition;
		}

		internal override void BeginInit()
		{
			base.BeginInit();
			Scale.BeginInit();
			Pointer.BeginInit();
		}

		internal override void EndInit()
		{
			base.EndInit();
			Scale.EndInit();
			Pointer.EndInit();
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			Scale.Dispose();
			Pointer.Dispose();
		}

		internal Orientation GetOrientation()
		{
			if (Orientation == Orientation.Auto)
			{
				RectangleF boundsInPixels = GetBoundsInPixels();
				if (boundsInPixels.Width < boundsInPixels.Height)
				{
					return Orientation.Vertical;
				}
				return Orientation.Horizontal;
			}
			return Orientation;
		}

		internal bool GetReversed()
		{
			return Reversed;
		}

		internal void InternalZoomLevelChanged()
		{
			try
			{
				internalZoomChange = true;
				MapCore mapCore = GetMapCore();
				if (mapCore != null && mapCore.Viewport != null && !fixThumbPoition)
				{
					mapCore.Viewport.Zoom = (float)GetZoomLevelFromPointerPosition(Pointer.Value);
				}
			}
			finally
			{
				internalZoomChange = false;
			}
		}

		internal void UpdateZoomRange()
		{
			if (Common != null && Common.MapCore != null)
			{
				try
				{
					fixThumbPoition = true;
					Pointer.Value = GetPointerPositionFromZoomLevel(Common.MapCore.Viewport.Zoom);
				}
				finally
				{
					fixThumbPoition = false;
				}
			}
		}

		internal double GetSnappingInterval()
		{
			if (Pointer.SnappingEnabled)
			{
				return Pointer.SnappingInterval;
			}
			return 0.25;
		}

		internal double GetNextZoomLevel(double currentZoom, int zoomLevels, bool zoomIn)
		{
			double pointerPositionFromZoomLevel = GetPointerPositionFromZoomLevel(currentZoom);
			pointerPositionFromZoomLevel += (zoomIn ? ((double)zoomLevels * GetSnappingInterval()) : ((double)(-zoomLevels) * GetSnappingInterval()));
			pointerPositionFromZoomLevel = Scale.GetValueLimit(pointerPositionFromZoomLevel, Pointer.SnappingEnabled, Pointer.SnappingInterval);
			return GetZoomLevelFromPointerPosition(pointerPositionFromZoomLevel);
		}

		private void ApplyStyle()
		{
			switch (PanelStyle)
			{
			case ZoomPanelStyle.RectangularButtons:
				zoomInButton.Style = PanelButtonStyle.RoundedRectangle;
				zoomOutButton.Style = PanelButtonStyle.RoundedRectangle;
				Pointer.MarkerStyle = MarkerStyle.Rectangle;
				Pointer.MarkerLength = 60f;
				Pointer.Width = 30f;
				break;
			case ZoomPanelStyle.CircularButtons:
				zoomInButton.Style = PanelButtonStyle.Circle;
				zoomOutButton.Style = PanelButtonStyle.Circle;
				Pointer.MarkerStyle = MarkerStyle.Circle;
				Pointer.MarkerLength = 60f;
				Pointer.Width = 30f;
				break;
			}
		}

		private void ApplyColors()
		{
			zoomInButton.BackColor = ButtonColor;
			zoomInButton.BorderColor = ButtonBorderColor;
			zoomInButton.SymbolColor = SymbolColor;
			zoomInButton.SymbolBorderColor = SymbolBorderColor;
			zoomOutButton.BackColor = ButtonColor;
			zoomOutButton.BorderColor = ButtonBorderColor;
			zoomOutButton.SymbolColor = SymbolColor;
			zoomOutButton.SymbolBorderColor = SymbolBorderColor;
			Pointer.FillColor = ThumbColor;
			Pointer.BorderColor = ThumbBorderColor;
			Scale.BorderColor = SliderBarBorderColor;
			Scale.FillColor = SliderBarColor;
			Scale.MinorTickMark.BorderColor = TickBorderColor;
			Scale.MinorTickMark.FillColor = TickColor;
		}

		private void zoomButtonClickHandler(object sender, EventArgs e)
		{
			if (zoomInButton == sender)
			{
				double value = Pointer.Value + GetSnappingInterval();
				value = Scale.GetValueLimit(value, Pointer.SnappingEnabled, Pointer.SnappingInterval);
				Pointer.Value = value;
			}
			else if (zoomOutButton == sender)
			{
				double value2 = Pointer.Value - GetSnappingInterval();
				value2 = Scale.GetValueLimit(value2, Pointer.SnappingEnabled, Pointer.SnappingInterval);
				Pointer.Value = value2;
			}
		}

		private void AdjustAutoOrientationForDocking(PanelDockStyle dockStyle)
		{
			if (Orientation == Orientation.Auto && ((GetOrientation() == Orientation.Vertical && (dockStyle == PanelDockStyle.Bottom || dockStyle == PanelDockStyle.Top)) || (GetOrientation() == Orientation.Horizontal && (dockStyle == PanelDockStyle.Left || dockStyle == PanelDockStyle.Right))))
			{
				MapSize size = new MapSize(this, Size.Height, Size.Width);
				PanelMargins panelMargins2 = base.Margins = new PanelMargins(base.Margins.Bottom, base.Margins.Right, base.Margins.Top, base.Margins.Left);
				Size = size;
			}
		}

		internal double GetZoomLevelFromPointerPosition(double pos)
		{
			double minimumZoom = MinimumZoom;
			double num = 0.0;
			if (ZoomType == ZoomType.Quadratic)
			{
				return (MaximumZoom - minimumZoom) / 10000.0 * pos * pos + minimumZoom;
			}
			if (ZoomType == ZoomType.Exponential)
			{
				_ = (MaximumZoom - minimumZoom) / 10000.0;
				return minimumZoom * Math.Pow(MaximumZoom / minimumZoom, pos / 100.0);
			}
			return (MaximumZoom - minimumZoom) / 100.0 * pos + minimumZoom;
		}

		internal double GetPointerPositionFromZoomLevel(double zoom)
		{
			double minimumZoom = MinimumZoom;
			double num = 0.0;
			if (ZoomType == ZoomType.Quadratic)
			{
				double num2 = (MaximumZoom - minimumZoom) / 10000.0;
				return Math.Sqrt((zoom - minimumZoom) / num2);
			}
			if (ZoomType == ZoomType.Exponential)
			{
				double num3 = Math.Log(MaximumZoom / minimumZoom) / 100.0;
				return Math.Log(zoom / minimumZoom) / num3;
			}
			double num4 = (MaximumZoom - minimumZoom) / 100.0;
			return (zoom - minimumZoom) / num4;
		}

		private double GetTickMarksInterval(int tickNumber)
		{
			return 100.0 / (double)(tickNumber - 1) - double.Epsilon;
		}

		internal float[] GetPossibleZoomLevels(float currentZoom)
		{
			if (!SnapToTickMarks)
			{
				return null;
			}
			currentZoom *= 100f;
			currentZoom = (float)Math.Round(currentZoom);
			currentZoom /= 100f;
			bool flag = true;
			ArrayList arrayList = new ArrayList();
			double num = 0.0;
			double num2 = 100.0 / (double)(TickCount - 1);
			for (int i = 0; i < TickCount; i++)
			{
				float num3 = (float)GetZoomLevelFromPointerPosition(num);
				num += num2;
				num3 *= 100f;
				num3 = (float)Math.Round(num3);
				num3 /= 100f;
				if (num3 == currentZoom)
				{
					flag = false;
				}
				arrayList.Add(num3);
			}
			if (flag)
			{
				arrayList.Insert(0, currentZoom);
			}
			return (float[])arrayList.ToArray(typeof(float));
		}

		internal void RenderStaticElements(MapGraphics g)
		{
			g.StartHotRegion(this);
			g.EndHotRegion();
			RenderStaticShadows(g);
			Scale.RenderStaticElements(g);
			if (ZoomButtonsVisible)
			{
				RenderButton(g, zoomInButton);
				RenderButton(g, zoomOutButton);
			}
		}

		private void AdjustScaleSize(MapGraphics g)
		{
			float markerLength = Pointer.MarkerLength;
			float width = Pointer.Width;
			float num = 0f;
			if (GetOrientation() == Orientation.Horizontal)
			{
				markerLength = g.GetAbsoluteY(markerLength);
				width = g.GetAbsoluteY(width);
				num = (g.GetAbsoluteY(100f) - markerLength) / 2f;
			}
			else
			{
				markerLength = g.GetAbsoluteX(markerLength);
				width = g.GetAbsoluteX(width);
				num = (g.GetAbsoluteX(100f) - markerLength) / 2f;
			}
			float num2 = 2f + width / 2f;
			if (ZoomButtonsVisible)
			{
				num2 += markerLength + num;
			}
			num2 = ((GetOrientation() != 0) ? g.GetRelativeY(num2) : g.GetRelativeX(num2));
			num2 = Math.Min(Math.Max(0f, num2), 100f);
			Scale.StartMargin = num2;
			Scale.EndMargin = num2;
		}

		internal void RenderDynamicElements(MapGraphics g)
		{
			RenderDynamicShadows(g);
			Pointer.Render(g);
		}

		internal void RenderDynamicShadows(MapGraphics g)
		{
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				GraphicsPath shadowPath = Pointer.GetShadowPath(g);
				if (shadowPath != null)
				{
					graphicsPath.AddPath(shadowPath, connect: false);
				}
				graphicsPath.FillMode = FillMode.Winding;
				using (Brush brush = g.GetShadowBrush())
				{
					g.FillPath(brush, graphicsPath);
				}
			}
		}

		internal void RenderStaticShadows(MapGraphics g)
		{
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				GraphicsPath shadowPath = Scale.GetShadowPath();
				if (shadowPath != null)
				{
					graphicsPath.AddPath(shadowPath, connect: false);
				}
				graphicsPath.FillMode = FillMode.Winding;
				using (Brush brush = g.GetShadowBrush())
				{
					g.FillPath(brush, graphicsPath);
				}
			}
		}

		private void RenderButton(MapGraphics g, PanelButton button)
		{
			float markerLength = Pointer.MarkerLength;
			markerLength += 5f;
			float num = 0f;
			float num2 = 0f;
			float num3;
			PointF relative;
			if (GetOrientation() == Orientation.Horizontal)
			{
				float relativeY = (100f - markerLength) / 2f;
				relativeY = g.GetRelativeX(g.GetAbsoluteY(relativeY));
				markerLength = g.GetRelativeX(g.GetAbsoluteY(markerLength));
				num3 = g.GetAbsoluteX(markerLength);
				relative = new PointF(markerLength / 2f + relativeY, 50f);
				if ((button.Type == PanelButtonType.ZoomOut && GetReversed()) || (button.Type == PanelButtonType.ZoomButton && !GetReversed()))
				{
					num = 100f - markerLength - 2f * relativeY;
				}
			}
			else
			{
				float relativeX = (100f - markerLength) / 2f;
				relativeX = g.GetRelativeY(g.GetAbsoluteX(relativeX));
				markerLength = g.GetRelativeY(g.GetAbsoluteX(markerLength));
				num3 = g.GetAbsoluteY(markerLength);
				relative = new PointF(50f, markerLength / 2f + relativeX);
				if ((button.Type == PanelButtonType.ZoomOut && !GetReversed()) || (button.Type == PanelButtonType.ZoomButton && GetReversed()))
				{
					num2 = 100f - markerLength - 2f * relativeX;
				}
			}
			relative.X += num;
			relative.Y += num2;
			relative = g.GetAbsolutePoint(relative);
			RectangleF absolute = new RectangleF(relative.X, relative.Y, 0f, 0f);
			absolute.Inflate(num3 / 2f, num3 / 2f);
			button.Bounds = g.GetRelativeRectangle(absolute);
			button.Render(g);
		}

		internal override void Render(MapGraphics g)
		{
			AbsoluteSize = g.GetAbsoluteSize(Size);
			AdjustScaleSize(g);
			switch (GetMapCore().RenderingMode)
			{
			case RenderingMode.ZoomThumb:
				RenderDynamicElements(g);
				break;
			case RenderingMode.SinglePanel:
				base.Render(g);
				RenderStaticElements(g);
				break;
			default:
				base.Render(g);
				RenderStaticElements(g);
				RenderDynamicElements(g);
				break;
			}
		}

		internal override object GetDefaultPropertyValue(string prop, object currentValue)
		{
			object obj = null;
			if (!(prop == "Size"))
			{
				if (prop == "Dock")
				{
					return PanelDockStyle.Left;
				}
				return base.GetDefaultPropertyValue(prop, currentValue);
			}
			return new MapSize(null, 40f, 200f);
		}
	}
}

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class PointerBase : NamedElement, IToolTipProvider, IPointerProvider, IImageMapProvider
	{
		private double position;

		private DataAttributes data;

		internal bool dragging;

		private string scaleName = "Default";

		private float distanceFromScale;

		private string image = "";

		private Color imageTransColor = Color.Empty;

		private float imageTransparency;

		private Color imageHueColor = Color.Empty;

		private Point imageOrigin = Point.Empty;

		private bool snappingEnabled;

		private double snappingInterval;

		private bool dampeningEnabled;

		private double dampeningSweepTime = 1.0;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private bool interactive;

		private bool visible = true;

		private BarStyle barStyle;

		private BarStart barStart = BarStart.ScaleStart;

		private float shadowOffset = 2f;

		private Color borderColor = Color.Black;

		private GaugeDashStyle borderStyle;

		private int borderWidth = 1;

		private Color fillColor = Color.White;

		private Color fillGradientEndColor = Color.Red;

		private GaugeHatchStyle fillHatchStyle;

		private GradientType fillGradientType = GradientType.DiagonalLeft;

		private MarkerStyle markerStyle;

		private float markerLength;

		private float width;

		private GaugeCursor cursor = GaugeCursor.Default;

		private object imageMapProviderTag;

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeName9")]
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

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeScaleName4")]
		[DefaultValue("Default")]
		[TypeConverter(typeof(ScaleSourceConverter))]
		public virtual string ScaleName
		{
			get
			{
				return scaleName;
			}
			set
			{
				scaleName = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeDistanceFromScale3")]
		[ValidateBound(0.0, 100.0)]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
				}
				distanceFromScale = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeImage7")]
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

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeImageTransColor5")]
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

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeImageTransparency5")]
		[DefaultValue(typeof(float), "0")]
		public virtual float ImageTransparency
		{
			get
			{
				return imageTransparency;
			}
			set
			{
				if (value < 0f || value > 100f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 100));
				}
				imageTransparency = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeImageHueColor")]
		[DefaultValue(typeof(Color), "")]
		public virtual Color ImageHueColor
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

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeCircularPointer_CapImageOrigin")]
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

		[Browsable(false)]
		[SRCategory("CategoryData")]
		[SRDescription("DescriptionAttributeValue6")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.Repaint)]
		[DefaultValue(double.NaN)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public virtual double Value
		{
			get
			{
				return data.Value;
			}
			set
			{
				if (data.Value != value)
				{
					data.Value = value;
				}
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryData")]
		[SRDescription("DescriptionAttributeValueSource")]
		[TypeConverter(typeof(ValueSourceConverter))]
		[RefreshProperties(RefreshProperties.Repaint)]
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		public virtual string ValueSource
		{
			get
			{
				return data.ValueSource;
			}
			set
			{
				data.ValueSource = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeSnappingEnabled4")]
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
				((IPointerProvider)this).DataValueChanged(initialize: true);
				Invalidate();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeSnappingInterval")]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionNegativeValue"));
				}
				snappingInterval = value;
				((IPointerProvider)this).DataValueChanged(initialize: true);
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeDampeningEnabled3")]
		[DefaultValue(false)]
		public virtual bool DampeningEnabled
		{
			get
			{
				return dampeningEnabled;
			}
			set
			{
				dampeningEnabled = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeDampeningSweepTime")]
		[DefaultValue(1.0)]
		public double DampeningSweepTime
		{
			get
			{
				return dampeningSweepTime;
			}
			set
			{
				dampeningSweepTime = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeToolTip5")]
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

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeHref10")]
		[Localizable(true)]
		[Browsable(false)]
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

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeMapAreaAttributes4")]
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

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeInteractive3")]
		[DefaultValue(false)]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeVisible7")]
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

		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributeBarStyle")]
		[DefaultValue(BarStyle.Style1)]
		internal BarStyle BarStyle
		{
			get
			{
				return barStyle;
			}
			set
			{
				barStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributeBarStart")]
		[DefaultValue(BarStart.ScaleStart)]
		public virtual BarStart BarStart
		{
			get
			{
				return barStart;
			}
			set
			{
				barStart = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeShadowOffset")]
		[ValidateBound(-5.0, 5.0)]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
				}
				shadowOffset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBorderColor5")]
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
		[SRDescription("DescriptionAttributeBorderStyle3")]
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
		[SRDescription("DescriptionAttributeBorderWidth9")]
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
		[SRDescription("DescriptionAttributeFillColor3")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFillGradientEndColor")]
		[DefaultValue(typeof(Color), "Red")]
		public virtual Color FillGradientEndColor
		{
			get
			{
				return fillGradientEndColor;
			}
			set
			{
				fillGradientEndColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFillHatchStyle3")]
		[DefaultValue(GaugeHatchStyle.None)]
		public virtual GaugeHatchStyle FillHatchStyle
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFillGradientType6")]
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

		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributePointerBase_MarkerStyle")]
		public virtual MarkerStyle MarkerStyle
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

		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributePointerBase_MarkerLength")]
		public virtual float MarkerLength
		{
			get
			{
				return markerLength;
			}
			set
			{
				markerLength = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributePointerBase_Width")]
		public virtual float Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeCursor")]
		[DefaultValue(GaugeCursor.Default)]
		public virtual GaugeCursor Cursor
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

		internal double Position
		{
			get
			{
				if (double.IsNaN(position))
				{
					return GetScaleBase().GetValueLimit(position);
				}
				return position;
			}
			set
			{
				position = value;
				if (dragging)
				{
					IValueProvider provider = ((IValueConsumer)Data).GetProvider();
					if (provider is InputValue && ((InputValue)provider).IntState != 0)
					{
						((InputValue)provider).Value = value;
					}
					if (!DampeningEnabled)
					{
						dragging = false;
					}
				}
				Refresh();
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
				data.Common = value;
			}
		}

		internal DataAttributes Data => data;

		object IImageMapProvider.Tag
		{
			get
			{
				return imageMapProviderTag;
			}
			set
			{
				imageMapProviderTag = value;
			}
		}

		double IPointerProvider.Position
		{
			get
			{
				return Position;
			}
			set
			{
				Position = value;
			}
		}

		public PointerBase()
		{
			data = new DataAttributes(this);
		}

		protected PointerBase(MarkerStyle markerStyle, float markerLength, float width, GradientType fillGradientType)
			: this()
		{
			this.markerStyle = markerStyle;
			this.markerLength = markerLength;
			this.width = width;
			this.fillGradientType = fillGradientType;
		}

		protected PointerBase(MarkerStyle markerStyle, float markerLength, float width, GradientType fillGradientType, Color fillColor, Color fillGradientEndColor, bool interactive)
			: this(markerStyle, markerLength, width, fillGradientType)
		{
			this.fillColor = fillColor;
			this.fillGradientEndColor = fillGradientEndColor;
			this.interactive = interactive;
		}

		internal virtual void DragTo(int x, int y, PointF refPoint)
		{
			ScaleBase scaleBase = GetScaleBase();
			double value = scaleBase.GetValue(refPoint, new PointF(x, y));
			value = scaleBase.GetValueLimit(value, SnappingEnabled, SnappingInterval);
			PointerPositionChangeEventArgs pointerPositionChangeEventArgs = new PointerPositionChangeEventArgs(value, DateTime.Now, Name, playbackMode: false);
			if (Common != null)
			{
				Common.GaugeContainer.OnPointerPositionChange(this, pointerPositionChangeEventArgs);
				if (pointerPositionChangeEventArgs.Accept)
				{
					dragging = true;
					Value = value;
				}
			}
		}

		internal virtual void Render(GaugeGraphics g)
		{
		}

		internal virtual void RenderShadow(GaugeGraphics g)
		{
		}

		internal GaugeBase GetGaugeBase()
		{
			if (Common == null)
			{
				return null;
			}
			return (GaugeBase)Collection.ParentElement;
		}

		internal ScaleBase GetScaleBase()
		{
			if (Common == null)
			{
				return null;
			}
			if (scaleName == string.Empty)
			{
				return null;
			}
			GaugeBase gaugeBase = GetGaugeBase();
			NamedCollection namedCollection = null;
			if (gaugeBase is CircularGauge)
			{
				namedCollection = ((CircularGauge)gaugeBase).Scales;
			}
			if (gaugeBase is LinearGauge)
			{
				namedCollection = ((LinearGauge)gaugeBase).Scales;
			}
			if (namedCollection == null)
			{
				return null;
			}
			return (ScaleBase)namedCollection.GetByName(scaleName);
		}

		internal override void BeginInit()
		{
			base.BeginInit();
			data.BeginInit();
		}

		internal override void EndInit()
		{
			base.EndInit();
			data.EndInit();
		}

		internal override void OnAdded()
		{
			base.OnAdded();
			data.ReconnectData(exact: true);
			scaleName = GetGaugeBase().GetDefaultScaleName(scaleName);
		}

		internal override void ReconnectData(bool exact)
		{
			data.ReconnectData(exact);
		}

		protected override void OnDispose()
		{
			Data.Dispose();
			base.OnDispose();
		}

		internal override void Notify(MessageType msg, NamedElement element, object param)
		{
			base.Notify(msg, element, param);
			switch (msg)
			{
			case MessageType.NamedElementRemove:
				if (element is ScaleBase)
				{
					ScaleBase scaleBase2 = (ScaleBase)element;
					if (scaleBase2.Name == scaleName && scaleBase2.ParentElement == ParentElement && scaleName != "Default")
					{
						scaleName = string.Empty;
					}
				}
				break;
			case MessageType.NamedElementRename:
				if (element is ScaleBase)
				{
					ScaleBase scaleBase = (ScaleBase)element;
					if (scaleBase.Name == scaleName && scaleBase.ParentElement == ParentElement)
					{
						scaleName = (string)param;
					}
				}
				break;
			}
			Data.Notify(msg, element, param);
		}

		string IToolTipProvider.GetToolTip(HitTestResult ht)
		{
			if (Common != null && Common.GaugeCore != null)
			{
				return Common.GaugeCore.ResolveAllKeywords(ToolTip, this);
			}
			return ToolTip;
		}

		string IImageMapProvider.GetToolTip()
		{
			return ((IToolTipProvider)this).GetToolTip((HitTestResult)null);
		}

		string IImageMapProvider.GetHref()
		{
			if (Common != null && Common.GaugeCore != null)
			{
				return Common.GaugeCore.ResolveAllKeywords(Href, this);
			}
			return Href;
		}

		string IImageMapProvider.GetMapAreaAttributes()
		{
			if (Common != null && Common.GaugeCore != null)
			{
				return Common.GaugeCore.ResolveAllKeywords(MapAreaAttributes, this);
			}
			return MapAreaAttributes;
		}

		void IPointerProvider.DataValueChanged(bool initialize)
		{
			if (!initialize)
			{
				GetGaugeBase()?.PointerValueChanged(this);
			}
			ScaleBase scaleBase = GetScaleBase();
			if (scaleBase != null)
			{
				double valueLimit = scaleBase.GetValueLimit(data.Value, snappingEnabled, snappingInterval);
				if (initialize || !dampeningEnabled || !Data.StartDampening(valueLimit, scaleBase.MinimumLog, scaleBase.Maximum, dampeningSweepTime, Common.GaugeCore.RefreshRate))
				{
					Position = valueLimit;
				}
			}
		}
	}
}

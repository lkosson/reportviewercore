using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal abstract class RangeBase : Range, IToolTipProvider, IImageMapProvider
	{
		private Placement placement;

		private float distanceFromScale = 10f;

		private string scaleName = "Default";

		private Color inRangeTickMarkColor = Color.Empty;

		private Color inRangeLabelColor = Color.Empty;

		private Color inRangeBarPointerColor = Color.Empty;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private bool visible = true;

		private Color borderColor = Color.Black;

		private float shadowOffset;

		private GaugeDashStyle borderStyle = GaugeDashStyle.Solid;

		private int borderWidth = 1;

		private Color fillColor = Color.Lime;

		private RangeGradientType fillGradientType = RangeGradientType.StartToEnd;

		private Color fillGradientEndColor = Color.Red;

		private GaugeHatchStyle fillHatchStyle;

		private bool selected;

		private float startWidth;

		private float endWidth;

		private object imageMapProviderTag;

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeName13")]
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

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeRangeBase_Placement")]
		public virtual Placement Placement
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

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeRangeBase_DistanceFromScale")]
		[ValidateBound(0.0, 100.0)]
		[DefaultValue(10f)]
		public virtual float DistanceFromScale
		{
			get
			{
				return distanceFromScale;
			}
			set
			{
				distanceFromScale = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeScaleName3")]
		[DefaultValue("Default")]
		[TypeConverter(typeof(ScaleSourceConverter))]
		public string ScaleName
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

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeInRangeTickMarkColor")]
		[DefaultValue(typeof(Color), "")]
		public Color InRangeTickMarkColor
		{
			get
			{
				return inRangeTickMarkColor;
			}
			set
			{
				inRangeTickMarkColor = value;
				base.Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeInRangeLabelColor")]
		[DefaultValue(typeof(Color), "")]
		public Color InRangeLabelColor
		{
			get
			{
				return inRangeLabelColor;
			}
			set
			{
				inRangeLabelColor = value;
				base.Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeInRangeBarPointerColor")]
		[DefaultValue(typeof(Color), "")]
		public Color InRangeBarPointerColor
		{
			get
			{
				return inRangeBarPointerColor;
			}
			set
			{
				inRangeBarPointerColor = value;
				base.Invalidate();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeToolTip9")]
		[Localizable(true)]
		[DefaultValue("")]
		public string ToolTip
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

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeHref8")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeVisible9")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(true)]
		public bool Visible
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBorderColor4")]
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
		[SRDescription("DescriptionAttributeShadowOffset4")]
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
		[SRDescription("DescriptionAttributeBorderStyle8")]
		[DefaultValue(GaugeDashStyle.Solid)]
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
		[SRDescription("DescriptionAttributeBorderWidth5")]
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
		[SRDescription("DescriptionAttributeFillColor8")]
		[DefaultValue(typeof(Color), "Lime")]
		public Color FillColor
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
		[SRDescription("DescriptionAttributeFillGradientType5")]
		[DefaultValue(RangeGradientType.StartToEnd)]
		public RangeGradientType FillGradientType
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFillGradientEndColor5")]
		[DefaultValue(typeof(Color), "Red")]
		public Color FillGradientEndColor
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
		[SRDescription("DescriptionAttributeFillHatchStyle7")]
		[DefaultValue(GaugeHatchStyle.None)]
		public GaugeHatchStyle FillHatchStyle
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

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeInRangeTimeout")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(0.0)]
		public override double InRangeTimeout
		{
			get
			{
				return base.InRangeTimeout;
			}
			set
			{
				base.InRangeTimeout = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeInRangeTimeoutType")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(PeriodType.Seconds)]
		public override PeriodType InRangeTimeoutType
		{
			get
			{
				return base.InRangeTimeoutType;
			}
			set
			{
				base.InRangeTimeoutType = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeSelected4")]
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

		public virtual float StartWidth
		{
			get
			{
				return startWidth;
			}
			set
			{
				startWidth = value;
				Invalidate();
			}
		}

		public virtual float EndWidth
		{
			get
			{
				return endWidth;
			}
			set
			{
				endWidth = value;
				Invalidate();
			}
		}

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

		internal abstract void Render(GaugeGraphics g);

		internal GaugeBase GetGaugeBase()
		{
			if (Common == null)
			{
				return null;
			}
			return (GaugeBase)Collection.ParentElement;
		}

		internal override void OnAdded()
		{
			base.OnAdded();
			scaleName = GetGaugeBase().GetDefaultScaleName(scaleName);
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
	}
}

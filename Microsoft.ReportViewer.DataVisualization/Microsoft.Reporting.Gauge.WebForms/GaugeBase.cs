using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal abstract class GaugeBase : NamedElement, IRenderable, IToolTipProvider, IImageMapProvider
	{
		private string parent = string.Empty;

		private NamedElement parentSystem;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private GaugeLocation location;

		private GaugeSize size;

		private int zOrder;

		private bool visible = true;

		private BackFrame frame;

		private bool clipContent = true;

		private bool selected;

		private string topImage = "";

		private Color topImageTransColor = Color.Empty;

		private Color topImageHueColor = Color.Empty;

		private float aspectRatio = float.NaN;

		private object imagMapProviderTag;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeParent5")]
		[TypeConverter(typeof(ParentSourceConverter))]
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		public string Parent
		{
			get
			{
				return parent;
			}
			set
			{
				string text = parent;
				if (value == "(none)")
				{
					value = string.Empty;
				}
				parent = value;
				try
				{
					ConnectToParent(exact: true);
				}
				catch
				{
					parent = text;
					throw;
				}
				Invalidate();
			}
		}

		[Bindable(false)]
		[Browsable(false)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeParentObject")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public NamedElement ParentObject => parentSystem;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeName3")]
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

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeToolTip8")]
		[Localizable(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
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

		[Browsable(false)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeHref3")]
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

		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeLocation5")]
		[TypeConverter(typeof(LocationConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[ValidateBound(100.0, 100.0)]
		public GaugeLocation Location
		{
			get
			{
				return location;
			}
			set
			{
				if (location.X != value.X || location.Y != value.Y)
				{
					RemoveAutoLayout();
				}
				location = value;
				location.Parent = this;
				Invalidate();
			}
		}

		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeSize5")]
		[TypeConverter(typeof(SizeConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[ValidateBound(100.0, 100.0)]
		public GaugeSize Size
		{
			get
			{
				return size;
			}
			set
			{
				if (size.Width != value.Width || size.Height != value.Height)
				{
					RemoveAutoLayout();
				}
				size = value;
				size.Parent = this;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeZOrder4")]
		[DefaultValue(0)]
		public int ZOrder
		{
			get
			{
				return zOrder;
			}
			set
			{
				zOrder = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeVisible3")]
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
		[SRDescription("DescriptionAttributeBackFrame")]
		[NotifyParentProperty(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BackFrame BackFrame
		{
			get
			{
				return frame;
			}
			set
			{
				frame = value;
				frame.Parent = this;
				Invalidate();
			}
		}

		protected BackFrame Frame
		{
			get
			{
				return frame;
			}
			set
			{
				frame = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeClipContent")]
		[DefaultValue(true)]
		public bool ClipContent
		{
			get
			{
				return clipContent;
			}
			set
			{
				clipContent = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeSelected7")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeTopImage4")]
		[DefaultValue("")]
		public string TopImage
		{
			get
			{
				return topImage;
			}
			set
			{
				topImage = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeTopImageTransColor")]
		[DefaultValue(typeof(Color), "")]
		public Color TopImageTransColor
		{
			get
			{
				return topImageTransColor;
			}
			set
			{
				topImageTransColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeTopImageHueColor")]
		[DefaultValue(typeof(Color), "")]
		public Color TopImageHueColor
		{
			get
			{
				return topImageHueColor;
			}
			set
			{
				topImageHueColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeGauge_AspectRatio")]
		[TypeConverter(typeof(FloatAutoValueConverter))]
		[DefaultValue(float.NaN)]
		public float AspectRatio
		{
			get
			{
				return aspectRatio;
			}
			set
			{
				if (value < 0f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange_min_open", 0));
				}
				if (value == 0f)
				{
					aspectRatio = float.NaN;
				}
				else
				{
					aspectRatio = value;
				}
				Invalidate();
			}
		}

		internal Position Position => new Position(Location, Size, ContentAlignment.TopLeft);

		object IImageMapProvider.Tag
		{
			get
			{
				return imagMapProviderTag;
			}
			set
			{
				imagMapProviderTag = value;
			}
		}

		protected GaugeBase()
		{
			location = new GaugeLocation(this, 0f, 0f);
			size = new GaugeSize(this, 100f, 100f);
			location.DefaultValues = true;
			size.DefaultValues = true;
		}

		private void RemoveAutoLayout()
		{
			if (Parent == string.Empty && Common != null && Common.GaugeContainer != null && Common.GaugeContainer.AutoLayout)
			{
				Common.GaugeContainer.AutoLayout = false;
			}
		}

		internal abstract IEnumerable GetRanges();

		internal string GetDefaultScaleName(string scaleName)
		{
			NamedCollection namedCollection = null;
			if (this is CircularGauge)
			{
				namedCollection = ((CircularGauge)this).Scales;
			}
			if (this is LinearGauge)
			{
				namedCollection = ((LinearGauge)this).Scales;
			}
			if (namedCollection.GetIndex(scaleName) != -1)
			{
				return scaleName;
			}
			if (scaleName == "Default" && namedCollection.Count > 0)
			{
				return namedCollection.GetByIndex(0).Name;
			}
			return scaleName;
		}

		private void ConnectToParent(bool exact)
		{
			if (Common != null && !Common.GaugeCore.isInitializing)
			{
				if (parent == string.Empty)
				{
					parentSystem = null;
					return;
				}
				Common.ObjectLinker.IsParentElementValid(this, this, exact);
				parentSystem = Common.ObjectLinker.GetElement(parent);
			}
		}

		internal override void OnAdded()
		{
			base.OnAdded();
			ConnectToParent(exact: true);
		}

		internal override void EndInit()
		{
			base.EndInit();
			ConnectToParent(exact: true);
		}

		internal abstract RectangleF GetAspectRatioBounds();

		internal virtual RectangleF GetBoundRect(GaugeGraphics g)
		{
			return Position.Rectangle;
		}

		internal virtual void RenderStaticElements(GaugeGraphics g)
		{
		}

		internal virtual void RenderDynamicElements(GaugeGraphics g)
		{
		}

		void IRenderable.RenderStaticElements(GaugeGraphics g)
		{
			RenderStaticElements(g);
		}

		void IRenderable.RenderDynamicElements(GaugeGraphics g)
		{
			RenderDynamicElements(g);
		}

		int IRenderable.GetZOrder()
		{
			return ZOrder;
		}

		RectangleF IRenderable.GetBoundRect(GaugeGraphics g)
		{
			return GetBoundRect(g);
		}

		object IRenderable.GetParentRenderable()
		{
			return parentSystem;
		}

		string IRenderable.GetParentRenderableName()
		{
			return parent;
		}

		internal abstract void PointerValueChanged(PointerBase sender);

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

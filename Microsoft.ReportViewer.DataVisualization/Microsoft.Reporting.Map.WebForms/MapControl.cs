using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net.Cache;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Reporting.Map.WebForms
{
	[DisplayName("Map")]
	[Description("Map for ASP.NET is a fully managed .NET control that lets you add fantastic looking maps to your applications with ease.")]
	[ToolboxBitmap(typeof(MapControl), "Map.ico")]
	internal class MapControl : IDisposable
	{
		internal const string ResKeyFormat = "#MapControlResKey#{0}#";

		private const string jsFilename = "DundasMap8.js";

		private const string imagemapExt = ".imagemap.txt";

		internal MapCore mapCore;

		internal string webFormDocumentURL = "";

		internal string applicationDocumentURL = "";

		internal static ITypeDescriptorContext controlCurrentContext = null;

		internal bool sessionExpired;

		internal bool generatingCachedContent;

		internal static string productID = "DG-WC";

		private bool doNotDispose;

		private bool isCallback;

		private Color backColor = Color.White;

		private bool enabled = true;

		public FormatNumberHandler FormatNumberHandler;

		private int width = 500;

		private int height = 375;

		private string resourceKey = "";

		private bool autoRunWizard;

		[SRCategory("CategoryAttribute_Groups")]
		[SRDescription("DescriptionAttributeMapControl_GroupFields")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FieldCollection GroupFields => mapCore.GroupFields;

		[SRCategory("CategoryAttribute_Paths")]
		[SRDescription("DescriptionAttributeMapControl_PathFields")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FieldCollection PathFields => mapCore.PathFields;

		[SRCategory("CategoryAttribute_Shapes")]
		[SRDescription("DescriptionAttributeMapControl_ShapeFields")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FieldCollection ShapeFields => mapCore.ShapeFields;

		[SRCategory("CategoryAttribute_Symbols")]
		[SRDescription("DescriptionAttributeMapControl_SymbolFields")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FieldCollection SymbolFields => mapCore.SymbolFields;

		[SRCategory("CategoryAttribute_Groups")]
		[SRDescription("DescriptionAttributeMapControl_GroupRules")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GroupRuleCollection GroupRules => mapCore.GroupRules;

		[SRCategory("CategoryAttribute_Shapes")]
		[SRDescription("DescriptionAttributeMapControl_ShapeRules")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ShapeRuleCollection ShapeRules => mapCore.ShapeRules;

		[SRCategory("CategoryAttribute_Paths")]
		[SRDescription("DescriptionAttributeMapControl_PathRules")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PathRuleCollection PathRules => mapCore.PathRules;

		[SRCategory("CategoryAttribute_Symbols")]
		[SRDescription("DescriptionAttributeMapControl_SymbolRules")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SymbolRuleCollection SymbolRules => mapCore.SymbolRules;

		[SRCategory("CategoryAttribute_Groups")]
		[SRDescription("DescriptionAttributeMapControl_Groups")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GroupCollection Groups => mapCore.Groups;

		[SRCategory("CategoryAttribute_Layers")]
		[SRDescription("DescriptionAttributeMapControl_Layers")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LayerCollection Layers => mapCore.Layers;

		[SRCategory("CategoryAttribute_Shapes")]
		[SRDescription("DescriptionAttributeMapControl_Shapes")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ShapeCollection Shapes => mapCore.Shapes;

		[SRCategory("CategoryAttribute_Paths")]
		[SRDescription("DescriptionAttributeMapControl_Paths")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PathCollection Paths => mapCore.Paths;

		[SRCategory("CategoryAttribute_Symbols")]
		[SRDescription("DescriptionAttributeMapControl_Shapes")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SymbolCollection Symbols => mapCore.Symbols;

		[SRCategory("CategoryAttribute_Panels")]
		[SRDescription("DescriptionAttributeMapControl_Images")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MapImageCollection Images => mapCore.Images;

		[SRCategory("CategoryAttribute_Panels")]
		[SRDescription("DescriptionAttributeMapControl_Labels")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MapLabelCollection Labels => mapCore.Labels;

		[Browsable(false)]
		[SRCategory("CategoryAttribute_MapControl")]
		[SRDescription("DescriptionAttributeMapControl_NamedImages")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public NamedImageCollection NamedImages => mapCore.NamedImages;

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeMapControl_DataBindingRules")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataBindingRulesCollection DataBindingRules => mapCore.DataBindingRules;

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeMapControl_MapAreas")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MapAreaCollection MapAreas => mapCore.MapAreas;

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeMapControl_ImageMapEnabled")]
		[DefaultValue(true)]
		public bool ImageMapEnabled
		{
			get
			{
				return mapCore.ImageMapEnabled;
			}
			set
			{
				mapCore.ImageMapEnabled = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapControl_ShadowIntensity")]
		[DefaultValue(25f)]
		public float ShadowIntensity
		{
			get
			{
				return mapCore.ShadowIntensity;
			}
			set
			{
				if (value > 100f || value < 0f)
				{
					throw new ArgumentException(SR.must_in_range(0.0, 100.0));
				}
				mapCore.ShadowIntensity = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapControl_AntiAliasing")]
		[DefaultValue(typeof(AntiAliasing), "All")]
		public AntiAliasing AntiAliasing
		{
			get
			{
				return mapCore.AntiAliasing;
			}
			set
			{
				mapCore.AntiAliasing = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapControl_TextAntiAliasingQuality")]
		[DefaultValue(TextAntiAliasingQuality.High)]
		public TextAntiAliasingQuality TextAntiAliasingQuality
		{
			get
			{
				return mapCore.TextAntiAliasingQuality;
			}
			set
			{
				mapCore.TextAntiAliasingQuality = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public MapSerializer Serializer => mapCore.Serializer;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public CallbackManager CallbackManager => mapCore.CallbackManager;

		[SRCategory("CategoryAttribute_ViewState")]
		[DefaultValue(SerializationContent.All)]
		[SRDescription("DescriptionAttributeMapControl_ViewStateContent")]
		public SerializationContent ViewStateContent
		{
			get
			{
				return mapCore.ViewStateContent;
			}
			set
			{
				mapCore.ViewStateContent = value;
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[DefaultValue(ImageType.Png)]
		[SRDescription("DescriptionAttributeMapControl_ImageType")]
		public ImageType ImageType
		{
			get
			{
				return mapCore.ImageType;
			}
			set
			{
				mapCore.ImageType = value;
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeMapControl_Compression")]
		public int Compression
		{
			get
			{
				return mapCore.Compression;
			}
			set
			{
				mapCore.Compression = value;
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeMapControl_ImageUrl")]
		[DefaultValue("TempFiles/MapPic_#SEQ(300,3)")]
		public string ImageUrl
		{
			get
			{
				return mapCore.ImageUrl;
			}
			set
			{
				if (value.IndexOf("#SEQ", StringComparison.Ordinal) > 0)
				{
					CheckImageURLSeqFormat(value);
				}
				mapCore.ImageUrl = value;
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeMapControl_RenderType")]
		[DefaultValue(RenderType.InteractiveImage)]
		public RenderType RenderType
		{
			get
			{
				return mapCore.RenderType;
			}
			set
			{
				mapCore.RenderType = value;
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeMapControl_ControlPersistence")]
		[DefaultValue(ControlPersistence.SessionState)]
		public ControlPersistence ControlPersistence
		{
			get
			{
				return mapCore.ControlPersistence;
			}
			set
			{
				mapCore.ControlPersistence = value;
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeMapControl_RenderingImageUrl")]
		[DefaultValue("")]
		public string RenderingImageUrl
		{
			get
			{
				return mapCore.RenderingImageUrl;
			}
			set
			{
				mapCore.RenderingImageUrl = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public bool IsCallback => isCallback;

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeMapControl_TagAttributes")]
		[DefaultValue("")]
		public string TagAttributes
		{
			get
			{
				return mapCore.TagAttributes;
			}
			set
			{
				mapCore.TagAttributes = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapControl_BorderLineColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "DarkGray")]
		public Color BorderLineColor
		{
			get
			{
				return mapCore.BorderLineColor;
			}
			set
			{
				mapCore.BorderLineColor = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapControl_BorderLineStyle")]
		[NotifyParentProperty(true)]
		[DefaultValue(MapDashStyle.Solid)]
		public MapDashStyle BorderLineStyle
		{
			get
			{
				return mapCore.BorderLineStyle;
			}
			set
			{
				mapCore.BorderLineStyle = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapControl_BorderLineWidth")]
		[NotifyParentProperty(true)]
		[DefaultValue(0)]
		public int BorderLineWidth
		{
			get
			{
				return mapCore.BorderLineWidth;
			}
			set
			{
				mapCore.BorderLineWidth = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(FrameStyle.None)]
		[SRDescription("DescriptionAttributeMapControl_Frame")]
		[NotifyParentProperty(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Frame Frame
		{
			get
			{
				return mapCore.Frame;
			}
			set
			{
				mapCore.Frame = value;
			}
		}

		[SRCategory("CategoryAttribute_Panels")]
		[SRDescription("DescriptionAttributeMapControl_Viewport")]
		[TypeConverter(typeof(DockablePanelConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Viewport Viewport
		{
			get
			{
				return mapCore.Viewport;
			}
			set
			{
				mapCore.Viewport = value;
			}
		}

		[SRCategory("CategoryAttribute_Panels")]
		[SRDescription("DescriptionAttributeMapControl_ZoomPanel")]
		[TypeConverter(typeof(DockablePanelConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ZoomPanel ZoomPanel
		{
			get
			{
				return mapCore.ZoomPanel;
			}
			set
			{
				mapCore.ZoomPanel = value;
			}
		}

		[SRCategory("CategoryAttribute_Panels")]
		[SRDescription("DescriptionAttributeMapControl_NavigationPanel")]
		[TypeConverter(typeof(DockablePanelConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavigationPanel NavigationPanel
		{
			get
			{
				return mapCore.NavigationPanel;
			}
			set
			{
				mapCore.NavigationPanel = value;
			}
		}

		[SRCategory("CategoryAttribute_Panels")]
		[SRDescription("DescriptionAttributeMapControl_DistanceScalePanel")]
		[TypeConverter(typeof(DockablePanelConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DistanceScalePanel DistanceScalePanel
		{
			get
			{
				return mapCore.DistanceScalePanel;
			}
			set
			{
				mapCore.DistanceScalePanel = value;
			}
		}

		[SRCategory("CategoryAttribute_Panels")]
		[SRDescription("DescriptionAttributeMapControl_ColorSwatchPanel")]
		[TypeConverter(typeof(DockablePanelConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColorSwatchPanel ColorSwatchPanel
		{
			get
			{
				return mapCore.ColorSwatchPanel;
			}
			set
			{
				mapCore.ColorSwatchPanel = value;
			}
		}

		[SRCategory("CategoryAttribute_Panels")]
		[SRDescription("DescriptionAttributeMapControl_Legends")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LegendCollection Legends => mapCore.Legends;

		[SRCategory("CategoryAttribute_ParallelsAndMeridians")]
		[SRDescription("DescriptionAttributeMapControl_Parallels")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridAttributes Parallels
		{
			get
			{
				return mapCore.Parallels;
			}
			set
			{
				mapCore.Parallels = value;
			}
		}

		[SRCategory("CategoryAttribute_ParallelsAndMeridians")]
		[SRDescription("DescriptionAttributeMapControl_Meridians")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridAttributes Meridians
		{
			get
			{
				return mapCore.Meridians;
			}
			set
			{
				mapCore.Meridians = value;
			}
		}

		[SRCategory("CategoryAttribute_ParallelsAndMeridians")]
		[SRDescription("DescriptionAttributeMapControl_GridUnderContent")]
		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		public bool GridUnderContent
		{
			get
			{
				return mapCore.GridUnderContent;
			}
			set
			{
				mapCore.GridUnderContent = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapControl_Projection")]
		[DefaultValue(typeof(Projection), "Equirectangular")]
		public Projection Projection
		{
			get
			{
				return mapCore.Projection;
			}
			set
			{
				mapCore.Projection = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapControl_SelectionMarkerColor")]
		[DefaultValue(typeof(Color), "LightBlue")]
		public Color SelectionMarkerColor
		{
			get
			{
				return mapCore.SelectionMarkerColor;
			}
			set
			{
				mapCore.SelectionMarkerColor = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapControl_SelectionBorderColor")]
		[DefaultValue(typeof(Color), "Black")]
		public Color SelectionBorderColor
		{
			get
			{
				return mapCore.SelectionBorderColor;
			}
			set
			{
				mapCore.SelectionBorderColor = value;
			}
		}

		[DefaultValue(typeof(Color), "White")]
		public Color BackColor
		{
			get
			{
				return backColor;
			}
			set
			{
				backColor = value;
			}
		}

		[DefaultValue(true)]
		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapControl_BackGradientType")]
		[DefaultValue(GradientType.None)]
		public GradientType BackGradientType
		{
			get
			{
				return mapCore.BackGradientType;
			}
			set
			{
				mapCore.BackGradientType = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapControl_BackSecondaryColor")]
		[DefaultValue(typeof(Color), "")]
		public Color BackSecondaryColor
		{
			get
			{
				return mapCore.BackSecondaryColor;
			}
			set
			{
				mapCore.BackSecondaryColor = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapControl_BackHatchStyle")]
		[DefaultValue(MapHatchStyle.None)]
		public MapHatchStyle BackHatchStyle
		{
			get
			{
				return mapCore.BackHatchStyle;
			}
			set
			{
				mapCore.BackHatchStyle = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapControl_MapLimits")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MapLimits MapLimits
		{
			get
			{
				return mapCore.MapLimits;
			}
			set
			{
				mapCore.MapLimits = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapControl_AutoLimitsIgnoreSymbols")]
		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		public bool AutoLimitsIgnoreSymbols
		{
			get
			{
				return mapCore.AutoLimitsIgnoreSymbols;
			}
			set
			{
				mapCore.AutoLimitsIgnoreSymbols = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapControl_ProjectionCenter")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ProjectionCenter ProjectionCenter
		{
			get
			{
				return mapCore.ProjectionCenter;
			}
			set
			{
				mapCore.ProjectionCenter = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapControl_GeographyMode")]
		[DefaultValue(true)]
		public bool GeographyMode
		{
			get
			{
				return mapCore.GeographyMode;
			}
			set
			{
				mapCore.GeographyMode = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapControl_TileServerTimeout")]
		[DefaultValue(10000)]
		public int TileServerTimeout
		{
			get
			{
				return mapCore.TileServerTimeout;
			}
			set
			{
				mapCore.TileServerTimeout = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapControl_TileServerMaxConnections")]
		[DefaultValue(2)]
		public int TileServerMaxConnections
		{
			get
			{
				return mapCore.TileServerMaxConnections;
			}
			set
			{
				mapCore.TileServerMaxConnections = value;
			}
		}

		[SRCategory("CategoryAttribute_VirtualEarth")]
		[SRDescription("DescriptionAttributeMapControl_TileCulture")]
		[DefaultValue(typeof(CultureInfo), "Invariant Language (Invariant Country)")]
		public CultureInfo TileCulture
		{
			get
			{
				return mapCore.TileCulture;
			}
			set
			{
				mapCore.TileCulture = value;
			}
		}

		[SRCategory("CategoryAttribute_VirtualEarth")]
		[SRDescription("DescriptionAttributeMapControl_TileCacheLevel")]
		[DefaultValue(RequestCacheLevel.Default)]
		public RequestCacheLevel TileCacheLevel
		{
			get
			{
				return mapCore.TileCacheLevel;
			}
			set
			{
				mapCore.TileCacheLevel = value;
			}
		}

		[SRCategory("CategoryAttribute_VirtualEarth")]
		[SRDescription("DescriptionAttributeMapControl_TileServerAppId")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public string TileServerAppId
		{
			set
			{
				mapCore.TileServerAppId = value;
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeMapControl_ContentCachingEnabled")]
		[DefaultValue(false)]
		public bool ContentCachingEnabled
		{
			get
			{
				return mapCore.ContentCachingEnabled;
			}
			set
			{
				mapCore.ContentCachingEnabled = value;
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeMapControl_ContentCachingTimeout")]
		[DefaultValue(0.0)]
		public double ContentCachingTimeout
		{
			get
			{
				return mapCore.ContentCachingTimeout;
			}
			set
			{
				mapCore.ContentCachingTimeout = value;
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[DefaultValue(500)]
		[SRDescription("DescriptionAttributeMapControl_Width")]
		public int Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
				if (mapCore.Viewport.ContentSize == 0)
				{
					mapCore.ResetCachedPaths();
				}
				mapCore.InvalidateAndLayout();
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[DefaultValue(375)]
		[SRDescription("DescriptionAttributeMapControl_Height")]
		public int Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
				if (mapCore.Viewport.ContentSize == 0)
				{
					mapCore.ResetCachedPaths();
				}
				mapCore.InvalidateAndLayout();
				Invalidate();
			}
		}

		internal ISelectable SelectedDesignTimeElement
		{
			get
			{
				return mapCore.SelectedDesignTimeElement;
			}
			set
			{
				mapCore.SelectedDesignTimeElement = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string ResourceKey
		{
			get
			{
				return resourceKey;
			}
			set
			{
				resourceKey = value;
			}
		}

		[DefaultValue(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public bool InternalAutoRunWizard
		{
			get
			{
				return autoRunWizard;
			}
			set
			{
				autoRunWizard = value;
			}
		}

		[Description("Called just before to databind the control. This event will be called once for each databinding object in the DataBindingRules collection.")]
		[SRCategory("CategoryAttribute_Data")]
		public event DataBindEventHandler BeforeDataBind;

		[Description("Called right after databinding the control. This event will be called once for each databinding object in the DataBindingRules collection.")]
		[SRCategory("CategoryAttribute_Data")]
		public event DataBindEventHandler AfterDataBind;

		internal event EventHandler BeforeApplyingRules;

		[Category("Action")]
		[Description("Fires when all defined rules for the control are applied.")]
		public event EventHandler AllRulesApplied;

		[Description("Called just before a map element's background is painted.")]
		[SRCategory("CategoryAttribute_Appearance")]
		public event MapPaintEvent PrePaint;

		[Description("Called just after a map element's background is painted.")]
		[SRCategory("CategoryAttribute_Appearance")]
		public event MapPaintEvent PostPaint;

		[Description("Called after a map element is added to a collection.")]
		[SRCategory("CategoryAttribute_Behavior")]
		public event ElementEvent ElementAdded;

		[Description("Called after a map element is removed from a collection.")]
		[SRCategory("CategoryAttribute_Behavior")]
		public event ElementEvent ElementRemoved;

		[Category("Action")]
		[Description("Fires when the user clicks on the control.")]
		public event ClickEvent Click;

		[Category("Action")]
		[Description("Fires during a user callback.")]
		public event CallbackEvent Callback;

		public MapControl()
		{
			mapCore = new MapCore(this);
			mapCore.BeginInit();
			BackColor = Color.White;
			Width = 500;
			Height = 375;
			BackColor = Color.White;
		}

		~MapControl()
		{
			doNotDispose = false;
			Dispose();
		}

		public void Dispose()
		{
			if (mapCore != null)
			{
				mapCore.Dispose();
				mapCore = null;
			}
		}

		public HitTestResult HitTest(int x, int y)
		{
			return mapCore.HitTest(x, y, new Type[0], returnMultipleElements: false)[0];
		}

		public HitTestResult HitTest(int x, int y, Type objectType)
		{
			return mapCore.HitTest(x, y, new Type[1]
			{
				objectType
			}, returnMultipleElements: false)[0];
		}

		public HitTestResult HitTest(int x, int y, Type[] objectTypes)
		{
			return mapCore.HitTest(x, y, objectTypes, returnMultipleElements: false)[0];
		}

		public HitTestResult[] HitTest(int x, int y, Type[] objectTypes, bool returnMultipleElements)
		{
			return mapCore.HitTest(x, y, objectTypes, returnMultipleElements);
		}

		public void SaveAsImage(string fileName, MapImageFormat format)
		{
			SaveAsImage(fileName, format, Compression);
		}

		public void SaveAsImage(string fileName, MapImageFormat format, int compression)
		{
			mapCore.SaveTo(fileName, format, compression, null, zoomThumbOnly: false);
		}

		public void SaveAsImage(Stream stream, MapImageFormat format)
		{
			SaveAsImage(stream, format, Compression);
		}

		public void SaveAsImage(Stream stream, MapImageFormat format, int compression)
		{
			mapCore.SaveTo(stream, format, compression, null, zoomThumbOnly: false);
		}

		public void SaveAsImage(Stream stream)
		{
			MapImageFormat imageFormat = (MapImageFormat)Enum.Parse(typeof(MapImageFormat), ImageType.ToString(CultureInfo.CurrentCulture), ignoreCase: true);
			mapCore.SaveTo(stream, imageFormat, Compression, null, zoomThumbOnly: false);
		}

		public void SaveAsImage(string fileName)
		{
			MapImageFormat imageFormat = (MapImageFormat)Enum.Parse(typeof(MapImageFormat), ImageType.ToString(CultureInfo.CurrentCulture), ignoreCase: true);
			mapCore.SaveTo(fileName, imageFormat, Compression, null, zoomThumbOnly: false);
		}

		public void DataBindShapes(object dataSource, string dataMember, string bindingField)
		{
			mapCore.DataBindShapes(dataSource, dataMember, bindingField);
		}

		public void DataBindGroups(object dataSource, string dataMember, string bindingField)
		{
			mapCore.DataBindGroups(dataSource, dataMember, bindingField);
		}

		public void DataBindPaths(object dataSource, string dataMember, string bindingField)
		{
			mapCore.DataBindPaths(dataSource, dataMember, bindingField);
		}

		public void DataBindSymbols(object dataSource, string dataMember, string bindingField, string category, string parentShapeField)
		{
			mapCore.DataBindSymbols(dataSource, dataMember, bindingField, category, parentShapeField, string.Empty, string.Empty);
		}

		public void DataBindSymbols(object dataSource, string dataMember, string bindingField, string category, string xCoordinateField, string yCoordinateField)
		{
			mapCore.DataBindSymbols(dataSource, dataMember, bindingField, category, string.Empty, xCoordinateField, yCoordinateField);
		}

		public void PrintPaint(Graphics g, Rectangle position)
		{
			mapCore.PrintPaint(g, position);
		}

		public void LoadFromShapeFile(string fileName, string nameColumn, bool importData)
		{
			if (nameColumn == null)
			{
				nameColumn = "";
			}
			mapCore.LoadFromShapeFile(fileName, nameColumn, importData);
		}

		public void LoadFromSQLServerSpatial(string connectionString, string sqlStatement, string nameColumn, string spatialColumn, BasicMapElements mapElementsToLoad, bool importAllData, string layer)
		{
			if (nameColumn == null)
			{
				nameColumn = string.Empty;
			}
			if (layer == null)
			{
				layer = string.Empty;
			}
			mapCore.LoadFromSpatial(connectionString, sqlStatement, nameColumn, spatialColumn, mapElementsToLoad, importAllData, layer);
		}

		public void LoadFromSQLServerSpatial(DataTable dataTable, string nameColumn, string spatialColumn, BasicMapElements mapElementsToLoad, bool importAllData, string layer)
		{
			if (nameColumn == null)
			{
				nameColumn = string.Empty;
			}
			if (layer == null)
			{
				layer = string.Empty;
			}
			mapCore.LoadFromSpatial(dataTable, nameColumn, spatialColumn, mapElementsToLoad, importAllData, layer);
		}

		public void LoadFromSQLServerSpatial(DataTable dataTable, string nameColumn, string spatialColumn, BasicMapElements mapElementsToLoad, string[] additionalColumnsToImport, string layer)
		{
			if (nameColumn == null)
			{
				nameColumn = string.Empty;
			}
			if (layer == null)
			{
				layer = string.Empty;
			}
			if (additionalColumnsToImport == null)
			{
				additionalColumnsToImport = new string[0];
			}
			mapCore.LoadFromSpatial(dataTable, nameColumn, spatialColumn, mapElementsToLoad, additionalColumnsToImport, layer);
		}

		public void Simplify(float factor)
		{
			mapCore.Simplify(factor);
		}

		public PointF PixelsToPercents(PointF pointInPixels)
		{
			return mapCore.PixelsToPercents(pointInPixels);
		}

		public PointF PercentsToPixels(PointF pointInPercents)
		{
			return mapCore.PercentsToPixels(pointInPercents);
		}

		public SizeF PixelsToPercents(SizeF sizeInPixels)
		{
			return mapCore.PixelsToPercents(sizeInPixels);
		}

		public SizeF PercentsToPixels(SizeF sizeInPercents)
		{
			return mapCore.PercentsToPixels(sizeInPercents);
		}

		public MapPoint PixelsToGeographic(PointF pointInPixels)
		{
			return mapCore.PixelsToGeographic(pointInPixels);
		}

		public PointF GeographicToPixels(MapPoint pointOnMap)
		{
			return mapCore.GeographicToPixels(pointOnMap);
		}

		public MapPoint PercentsToGeographic(MapPoint pointInPercents)
		{
			return mapCore.PercentsToGeographic(pointInPercents);
		}

		public MapPoint GeographicToPercents(MapPoint pointOnMap)
		{
			Point3D point3D = mapCore.GeographicToPercents(pointOnMap);
			return new MapPoint(point3D.X, point3D.Y);
		}

		public double MeasureDistance(MapPoint point1, MapPoint point2)
		{
			return mapCore.MeasureDistance(point1, point2);
		}

		public void CenterView(MapPoint pointOnMap)
		{
			mapCore.CenterView(pointOnMap);
		}

		public void CreateGroups(string shapeFieldName)
		{
			mapCore.CreateGroups(shapeFieldName);
		}

		public void ApplyRules()
		{
			mapCore.ApplyAllRules();
		}

		public ArrayList Find(string searchFor, bool ignoreCase, bool exactSearch)
		{
			return mapCore.Find(searchFor, ignoreCase, exactSearch);
		}

		protected internal virtual void OnBeforeDataBind(DataBindEventArgs e)
		{
			if (this.BeforeDataBind != null)
			{
				this.BeforeDataBind(this, e);
			}
		}

		protected internal virtual void OnAfterDataBind(DataBindEventArgs e)
		{
			if (this.AfterDataBind != null)
			{
				this.AfterDataBind(this, e);
			}
		}

		internal void OnBeforeApplyingRules(EventArgs e)
		{
			if (this.BeforeApplyingRules != null)
			{
				this.BeforeApplyingRules(this, e);
			}
		}

		internal void OnAllRulesApplied(EventArgs e)
		{
			if (this.AllRulesApplied != null)
			{
				this.AllRulesApplied(this, e);
			}
		}

		internal string GetHtmlColor(Color color, bool allowTransparent)
		{
			if (color.A == 0)
			{
				if (allowTransparent)
				{
					return "transparent";
				}
				color = Color.White;
			}
			else if (color.A < byte.MaxValue)
			{
				color = Color.FromArgb(color.R, color.G, color.B);
			}
			return string.Format(CultureInfo.InvariantCulture, "rgb({0}, {1}, {2})", color.R, color.G, color.B);
		}

		internal string GetHtmlBorderStyle(Panel panel)
		{
			string empty = string.Empty;
			if (panel.BorderWidth == 0)
			{
				return empty;
			}
			empty = ((panel.BorderStyle == MapDashStyle.Dash || panel.BorderStyle == MapDashStyle.DashDot || panel.BorderStyle == MapDashStyle.DashDotDot) ? (empty + "border-style: dashed;") : ((panel.BorderStyle == MapDashStyle.Dot) ? (empty + "border-style: dotted;") : ((panel.BorderStyle != MapDashStyle.Solid) ? (empty + "border-style: none;") : (empty + "border-style: solid;"))));
			empty = empty + "border-width: " + panel.BorderWidth.ToString(CultureInfo.CurrentCulture) + "px;";
			return empty + "border-color: " + GetHtmlColor(panel.BorderColor, allowTransparent: true) + ";";
		}

		internal string GetPanelHref(Panel panel)
		{
			if (panel.Href == "")
			{
				return "";
			}
			return "onclick=\"window.location='" + panel.Href + "';\" ";
		}

		internal string GetPanelHrefStyle(Panel panel)
		{
			if (panel.Href == "")
			{
				return "";
			}
			return "cursor: pointer; ";
		}

		internal string GetPanelImageUrl(string imageUrl, string panelName)
		{
			int startIndex = imageUrl.LastIndexOf('.');
			return imageUrl.Insert(startIndex, panelName);
		}

		internal string GetUrl(string imageUrl, string filename)
		{
			int num = imageUrl.LastIndexOf('/');
			if (num == -1)
			{
				return filename;
			}
			return imageUrl.Substring(0, num + 1) + filename;
		}

		internal void SaveFiles(string fullImagePath)
		{
			string directoryName = System.IO.Path.GetDirectoryName(fullImagePath);
			if (!new DirectoryInfo(directoryName).Exists)
			{
				Directory.CreateDirectory(directoryName);
			}
			string text = directoryName + "\\DundasMap8.js";
			if (!new FileInfo(text).Exists)
			{
				Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(MapControl).Namespace + ".DundasMap.js");
				byte[] array = new byte[manifestResourceStream.Length];
				manifestResourceStream.Read(array, 0, array.Length);
				string value = ObfuscateJavaScript(Encoding.UTF8.GetString(array));
				try
				{
					StreamWriter streamWriter = File.CreateText(text);
					streamWriter.Write(value);
					streamWriter.Close();
				}
				catch
				{
				}
			}
			if (RenderType != RenderType.InteractiveImage)
			{
				if (IsImageCached(fullImagePath) == 0L)
				{
					try
					{
						SaveAsImage(fullImagePath);
					}
					catch
					{
					}
				}
				return;
			}
			if (IsImageCached(fullImagePath) == 0L)
			{
				try
				{
					mapCore.RenderingMode = RenderingMode.Background;
					SaveAsImage(fullImagePath);
					mapCore.RenderingMode = RenderingMode.All;
				}
				catch
				{
				}
			}
			Panel[] sortedPanels = mapCore.GetSortedPanels();
			foreach (Panel panel in sortedPanels)
			{
				if (panel is Viewport || !panel.Visible)
				{
					continue;
				}
				string panelImageUrl = GetPanelImageUrl(fullImagePath, panel.Name);
				if (IsImageCached(panelImageUrl) == 0L)
				{
					try
					{
						mapCore.SavePanelAsImage(panel, panelImageUrl, zoomThumbOnly: false);
					}
					catch
					{
					}
				}
				if (!(panel is ZoomPanel))
				{
					continue;
				}
				string panelImageUrl2 = GetPanelImageUrl(fullImagePath, panel.Name + "Thumb");
				if (IsImageCached(panelImageUrl2) == 0L)
				{
					try
					{
						mapCore.SavePanelAsImage(panel, panelImageUrl2, zoomThumbOnly: true);
					}
					catch
					{
					}
				}
			}
			string panelImageUrl3 = GetPanelImageUrl(fullImagePath, "Background");
			if (IsImageCached(panelImageUrl3) == 0L)
			{
				try
				{
					Bitmap bitmap = new Bitmap(1, 1);
					bitmap.SetPixel(0, 0, mapCore.GetGridSectionBackColor());
					bitmap.Save(panelImageUrl3, ImageFormat.Png);
				}
				catch
				{
				}
			}
			string text2 = directoryName + "\\Empty.gif";
			if (!new FileInfo(text2).Exists)
			{
				Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(MapControl).Namespace + ".Empty.gif");
				byte[] array2 = new byte[manifestResourceStream2.Length];
				manifestResourceStream2.Read(array2, 0, array2.Length);
				try
				{
					FileStream fileStream = File.Create(text2);
					fileStream.Write(array2, 0, array2.Length);
					fileStream.Close();
				}
				catch
				{
				}
			}
			if (!(RenderingImageUrl == ""))
			{
				return;
			}
			string text3 = directoryName + "\\Rendering.gif";
			if (!new FileInfo(text3).Exists)
			{
				Stream manifestResourceStream3 = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(MapControl).Namespace + ".Rendering.gif");
				byte[] array3 = new byte[manifestResourceStream3.Length];
				manifestResourceStream3.Read(array3, 0, array3.Length);
				try
				{
					FileStream fileStream2 = File.Create(text3);
					fileStream2.Write(array3, 0, array3.Length);
					fileStream2.Close();
				}
				catch
				{
				}
			}
		}

		private string ObfuscateJavaScript(string text)
		{
			return Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(text, "\\/\\/.*\\n", ""), "[\\t\\v\\f]", ""), "\\x20?([\\+\\-\\*\\/\\=\\?\\:])\\x20?", "$1"), "(if|for|while)(\\([^\\)]+\\))[\\n\\r]+", "$1$2"), "(for\\()\\x20*(.+;)\\x20*(.+;)\\x20*(.+)\\x20*(\\))", "$1$2$3$4$5'"), ";{2,}", ";").Replace("  ", "").Replace("\r\n{", "{")
				.Replace("\r\n}", "}")
				.Replace("{\r\n", "{")
				.Replace("}\r\n", "}")
				.Replace("\r\n\r\n", "\r\n")
				.Replace(";\r\n", ";")
				.Replace("\r\nvar", "var");
		}

		private bool SaveGridSectionImageMap(string fullImageMapPath)
		{
			bool result = false;
			MapAreaCollection mapAreasFromHotRegionList = mapCore.GetMapAreasFromHotRegionList();
			if (mapAreasFromHotRegionList.Count > 0)
			{
				try
				{
					StreamWriter streamWriter = File.CreateText(fullImageMapPath);
					foreach (MapArea item in mapAreasFromHotRegionList)
					{
						string tag = item.GetTag();
						if (tag.Length > 0)
						{
							streamWriter.Write(tag);
							result = true;
						}
					}
					streamWriter.Close();
					return result;
				}
				catch
				{
					return false;
				}
			}
			return result;
		}

		private long GetLastWriteTime(string fullImagePath)
		{
			return new FileInfo(fullImagePath).LastWriteTime.Ticks;
		}

		private long IsImageCached(string fullImagePath)
		{
			if (!ContentCachingEnabled || IsDesignMode() || generatingCachedContent)
			{
				return 0L;
			}
			FileInfo fileInfo = new FileInfo(fullImagePath);
			if (!fileInfo.Exists)
			{
				return 0L;
			}
			if (ContentCachingTimeout == 0.0)
			{
				return fileInfo.LastWriteTime.Ticks;
			}
			if ((DateTime.Now - fileInfo.LastWriteTime).TotalMinutes < ContentCachingTimeout)
			{
				return fileInfo.LastWriteTime.Ticks;
			}
			return 0L;
		}

		private long IsFileCached(string fullImagePath, string fileExtension)
		{
			if (IsImageCached(fullImagePath) == 0L)
			{
				return 0L;
			}
			return IsImageCached(fullImagePath + fileExtension);
		}

		private Point[] DecodeGridSectionIndexes(string gridXParam, string gridYParam)
		{
			char[] array = new char[1]
			{
				';'
			};
			gridXParam = gridXParam.TrimEnd(array);
			gridYParam = gridYParam.TrimEnd(array);
			string[] array2 = gridXParam.Split(array);
			string[] array3 = gridYParam.Split(array);
			Point[] array4 = new Point[array2.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array4[i].X = int.Parse(array2[i], CultureInfo.InvariantCulture);
				array4[i].Y = int.Parse(array3[i], CultureInfo.InvariantCulture);
			}
			return array4;
		}

		internal void Invalidate()
		{
		}

		internal void Refresh()
		{
		}

		private void CheckImageURLSeqFormat(string imageURL)
		{
			int num = imageURL.IndexOf("#SEQ", StringComparison.Ordinal);
			num += 4;
			if (imageURL[num] != '(')
			{
				throw new ArgumentException("Invalid image URL format. #SEQ formatter must be followed by (300,3), where 300 is a max sequence number and 3 is an image file time to live.", "ImageURL");
			}
			int num2 = imageURL.IndexOf(')', 1);
			if (num2 < 0)
			{
				throw new ArgumentException("Invalid image URL format. #SEQ formatter must be followed by (300,3), where 300 is a max sequence number and 3 is an image file time to live.", "ImageURL");
			}
			string[] array = imageURL.Substring(num + 1, num2 - num - 1).Split(',');
			if (array == null || array.Length != 2)
			{
				throw new ArgumentException("Invalid image URL format. #SEQ formatter must be followed by (300,3), where 300 is a max sequence number and 3 is an image file time to live.", "ImageURL");
			}
			string[] array2 = array;
			foreach (string text in array2)
			{
				for (int j = 0; j < text.Length; j++)
				{
					if (!char.IsDigit(text[j]))
					{
						throw new ArgumentException("Invalid image URL format. #SEQ formatter must be followed by (300,3), where 300 is a max sequence number and 3 is an image file time to live.", "ImageURL");
					}
				}
			}
		}

		internal bool IsDesignMode()
		{
			return false;
		}

		internal void OnPrePaint(object sender, MapPaintEventArgs e)
		{
			if (this.PrePaint != null)
			{
				this.PrePaint(sender, e);
			}
		}

		internal void OnPostPaint(object sender, MapPaintEventArgs e)
		{
			if (this.PostPaint != null)
			{
				this.PostPaint(sender, e);
			}
		}

		internal void OnElementAdded(object sender, ElementEventArgs e)
		{
			if (this.ElementAdded != null)
			{
				this.ElementAdded(sender, e);
			}
		}

		internal void OnElementRemoved(object sender, ElementEventArgs e)
		{
			if (this.ElementRemoved != null)
			{
				this.ElementRemoved(sender, e);
			}
		}

		internal void OnClick(object sender, ClickEventArgs e)
		{
			MapControl mapControl = this;
			if (sender is MapControl && sender != this)
			{
				mapControl = (sender as MapControl);
			}
			if (mapControl.Click != null)
			{
				mapControl.Click(sender, e);
			}
		}

		internal void OnCallback(object sender, CallbackEventArgs e)
		{
			MapControl mapControl = this;
			if (sender is MapControl && sender != this)
			{
				mapControl = (sender as MapControl);
			}
			if (mapControl.Callback != null)
			{
				mapControl.Callback(sender, e);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This method is obsolete in VS2005 map control. The method doesn't do anything and is\tpresent for backward compatibility only")]
		public void LoadResourceData(Type rootType, string shapeResourceKey, string pathsResourceKey)
		{
		}
	}
}

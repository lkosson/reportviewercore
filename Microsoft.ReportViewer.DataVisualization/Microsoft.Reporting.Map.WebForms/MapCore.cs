using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class MapCore : NamedElement, IDisposable
	{
		private struct PanelPair
		{
			private DockablePanel Panel1;

			private DockablePanel Panel2;

			public PanelPair(DockablePanel panel1, DockablePanel panel2)
			{
				Panel1 = panel1;
				Panel2 = panel2;
			}
		}

		internal class GridSection : IDisposable
		{
			public Point Origin;

			public BufferBitmap Bitmap;

			public HotRegionList HotRegions;

			public bool Dirty;

			public void Dispose()
			{
				if (HotRegions != null)
				{
					foreach (HotRegion item in HotRegions.List)
					{
						item.DoNotDispose = false;
					}
					HotRegions.Clear();
					HotRegions = null;
				}
				if (Bitmap != null)
				{
					Bitmap.Dispose();
					Bitmap = null;
				}
				GC.SuppressFinalize(this);
			}
		}

		private const double ProjectionLatitudeLimit = 89.0;

		private const double MercatorProjectionLatitudeLimit = 85.05112878;

		private const float VisibleContentMargin = 20f;

		internal const int MaximumResolvedKeywordLength = 80;

		private const int BufferSize = 4096;

		internal const string DefaultRuleLegendTest = "#FROMVALUE{N0} - #TOVALUE{N0}";

		internal const string NoneToken = "(none)";

		internal const string AllToken = "(all)";

		internal const string NameToken = "(Name)";

		private const double ClippingMargin = 1E-10;

		private static string[] _geoPoliticalBlockListLocales;

		private static Hashtable _geoPoliticalBlockHashtableLocales;

		private BorderTypeRegistry borderTypeRegistry;

		internal bool silentPaint;

		internal string loadedBuildNumber = string.Empty;

		internal bool skipPaint;

		private MapAreaCollection mapAreas;

		private Panel[] sortedPanels;

		private BufferBitmap bufferBitmap;

		private ImageLoader imageLoader;

		internal bool dirtyFlag;

		internal bool disableInvalidate;

		internal bool isInitializing;

		internal bool boundToDataSource;

		private bool invalidatingDataBind;

		private bool dataBinding;

		private bool isPrinting;

		private Size printSize = new Size(0, 0);

		internal ServiceContainer serviceContainer;

		private bool rulesDirty = true;

		private bool applyingRules;

		private bool cachedBoundsDirty = true;

		private bool updatingCachedBounds;

		private bool cachedPathsDirty = true;

		private bool resetingCachedPaths;

		private bool childSymbolsDirty = true;

		private bool resettingChildSymbols;

		private bool gridSectionsDirty = true;

		private bool recreatingGridSections;

		private int bufferedGridSectionCount;

		private MapBounds cachedBoundsAfterProjection;

		private Dictionary<RectangleF, RectangleF> cachedGeographicClipRectangles = new Dictionary<RectangleF, RectangleF>();

		private long openTileRequestCount;

		private EventWaitHandle requestsCompletedEvent = new EventWaitHandle(initialState: false, EventResetMode.ManualReset);

		private string[] userLocales;

		private FieldCollection groupFields;

		private FieldCollection shapeFields;

		private FieldCollection pathFields;

		private FieldCollection symbolFields;

		private GroupRuleCollection groupRules;

		private ShapeRuleCollection shapeRules;

		private PathRuleCollection pathRules;

		private SymbolRuleCollection symbolRules;

		private GroupCollection groups;

		private LayerCollection layers;

		private ShapeCollection shapes;

		private PathCollection paths;

		private SymbolCollection symbols;

		private NamedImageCollection namedImages;

		private MapImageCollection images;

		private MapLabelCollection labels;

		private LegendCollection legends;

		private DataBindingRulesCollection dataBindingRules;

		private Viewport viewport;

		private ZoomPanel zoomPanel;

		private NavigationPanel navigationPanel;

		private DistanceScalePanel distanceScalePanel;

		private ColorSwatchPanel colorSwatchPanel;

		private GridAttributes parallels;

		private GridAttributes meridians;

		private bool gridUnderContent;

		private AntiAliasing antiAliasing = AntiAliasing.All;

		private TextAntiAliasingQuality textAntiAliasingQuality = TextAntiAliasingQuality.High;

		private float shadowIntensity = 25f;

		private GradientType backGradientType;

		private Color backSecondaryColor = Color.Empty;

		private MapHatchStyle backHatchStyle;

		private Frame frame;

		private Projection projection;

		private Color borderLineColor = Color.DarkGray;

		private MapDashStyle borderLineStyle = MapDashStyle.Solid;

		private int borderLineWidth;

		private Color selectionMarkerColor = Color.LightBlue;

		private Color selectionBorderColor = Color.Black;

		private MapLimits mapLimits;

		private bool autoLimitsIgnoreSymbols;

		private ProjectionCenter projectionCenter;

		private bool geographyMode = true;

		private int tileServerTimeout = 10000;

		private int tileServerMaxConnections = 2;

		private CultureInfo tileCulture = CultureInfo.InvariantCulture;

		private RequestCacheLevel tileCacheLevel;

		private string tileServerAppId = "ArHDYkG4iBBgRo5ZKlvBPAXjj3tA13t5WMpr60l7m23-SSyZJoHsVVe2IecRKN88";

		private ImageType imageType = ImageType.Png;

		private int compression;

		private string mapImageUrl = "TempFiles/MapPic_#SEQ(300,3)";

		private bool imageMapEnabled = true;

		private RenderType renderType = RenderType.InteractiveImage;

		private ControlPersistence controlPersistence = ControlPersistence.SessionState;

		private string renderingImageUrl = "";

		private SerializationContent viewStateContent = SerializationContent.All;

		private string tagAttributes = "";

		private bool contentCachingEnabled;

		private double contentCachingTimeout;

		private object dataSource;

		private MapSerializer serializer;

		private CallbackManager callbackManager;

		private MapControl parent;

		private HotRegionList hotRegionList;

		private bool serializing;

		private MapPoint mapCenterPoint = new MapPoint(0.0, 0.0);

		private MapPoint minimumPoint = new MapPoint(-180.0, -90.0);

		private MapPoint maximumPoint = new MapPoint(180.0, 90.0);

		private RectangleF mapDockBounds = RectangleF.Empty;

		private ISelectable selectedDesignTimeElement;

		private bool useRSAccessibilityNames;

		private bool doPanelLayout = true;

		private RenderingMode renderingMode;

		private Panel panelToRender;

		private int singleGridSectionX;

		private int singleGridSectionY;

		private Hashtable tileImagesCache;

		private Image tileWaitImage;

		private bool uppercaseFieldKeywords = true;

		private int maxSpatialPointCount = int.MaxValue;

		private int maxSpatialElementCount = int.MaxValue;

		private double CurrentLatitudeLimit = 89.0;

		private int CurrentSrid = int.MaxValue;

		internal LoadTilesHandler LoadTilesHandler;

		internal SaveTilesHandler SaveTilesHandler;

		public string licenseData = "";

		private double[,] A = new double[18, 4]
		{
			{
				-2.49E-06,
				0.0,
				-0.0001753,
				0.8487
			},
			{
				2.5E-07,
				-3.74E-05,
				-0.00036231,
				0.84751182
			},
			{
				-1.21E-06,
				-3.371E-05,
				-0.00071788,
				0.84479598
			},
			{
				3.22E-06,
				-5.18E-05,
				-0.00114546,
				0.840213
			},
			{
				-4.88E-06,
				-3.5E-06,
				-0.00142198,
				0.83359314
			},
			{
				2E-08,
				-7.677E-05,
				-0.00182334,
				0.8257851
			},
			{
				1.4E-06,
				-7.643E-05,
				-0.00258932,
				0.814752
			},
			{
				-2.22E-06,
				-5.546E-05,
				-0.00324874,
				0.80006949
			},
			{
				4.08E-06,
				-8.875E-05,
				-0.00396977,
				0.78216192
			},
			{
				-4.61E-06,
				-2.748E-05,
				-0.00455092,
				0.76060494
			},
			{
				2.82E-06,
				-9.667E-05,
				-0.00517168,
				0.73658673
			},
			{
				7.9E-07,
				-5.432E-05,
				-0.00592662,
				0.7086645
			},
			{
				8.2E-07,
				-4.252E-05,
				-0.0064108,
				0.67777182
			},
			{
				-2.03E-06,
				-3.021E-05,
				-0.00677444,
				0.64475739
			},
			{
				-6.94E-06,
				-6.071E-05,
				-0.00722903,
				0.60987582
			},
			{
				1.487E-05,
				-0.00016487,
				-0.00835697,
				0.57134484
			},
			{
				1.059E-05,
				5.822E-05,
				-0.00889021,
				0.52729731
			},
			{
				-1.448E-05,
				0.00021714,
				-0.0075134,
				0.48562614
			}
		};

		private double[,] B = new double[18, 4]
		{
			{
				-0.0,
				0.0,
				0.01676852,
				0.0
			},
			{
				0.0,
				-0.0,
				0.01676851,
				0.0838426
			},
			{
				-0.0,
				1E-08,
				0.01676854,
				0.1676852
			},
			{
				1E-08,
				-3E-08,
				0.01676845,
				0.2515278
			},
			{
				-3E-08,
				1E-07,
				0.0167688,
				0.3353704
			},
			{
				1.1E-07,
				-3.6E-07,
				0.01676749,
				0.419213
			},
			{
				-4.2E-07,
				1.34E-06,
				0.01677238,
				0.5030556
			},
			{
				-5.9E-07,
				-4.99E-06,
				0.01675411,
				0.5868982
			},
			{
				-4.7E-07,
				-1.383E-05,
				0.01666002,
				0.67047034
			},
			{
				-7.9E-07,
				-2.084E-05,
				0.01648669,
				0.75336633
			},
			{
				-7.1E-07,
				-3.264E-05,
				0.01621931,
				0.83518048
			},
			{
				-6.8E-07,
				-4.335E-05,
				0.0158394,
				0.91537187
			},
			{
				-8.7E-07,
				-5.362E-05,
				0.01535457,
				0.99339958
			},
			{
				-1.23E-06,
				-6.673E-05,
				0.01475283,
				1.06872269
			},
			{
				-7E-07,
				-8.515E-05,
				0.01399341,
				1.14066505
			},
			{
				-8.94E-06,
				-9.571E-05,
				0.01308909,
				1.20841528
			},
			{
				-1.547E-05,
				-0.00022979,
				0.01146158,
				1.27035062
			},
			{
				3.079E-05,
				-0.00046184,
				0.00800345,
				1.31998003
			}
		};

		private double[,] Phi = new double[18, 5]
		{
			{
				0.0,
				0.00144326,
				0.0,
				59.63554505,
				0.0
			},
			{
				5.0,
				-0.00721629,
				0.00036302,
				59.63557549,
				0.0838426
			},
			{
				10.0,
				0.0274219,
				-0.00145208,
				59.63548418,
				0.1676852
			},
			{
				15.0,
				-0.10247131,
				0.00544529,
				59.63581898,
				0.2515278
			},
			{
				20.0,
				0.38246336,
				-0.02032909,
				59.63457108,
				0.3353704
			},
			{
				25.0,
				-1.42738212,
				0.07587108,
				59.63922787,
				0.419213
			},
			{
				30.0,
				5.32706511,
				-0.28315521,
				59.62184863,
				0.5030556
			},
			{
				35.0,
				7.66388284,
				1.05674976,
				59.6867088,
				0.5868982
			},
			{
				40.0,
				6.65735123,
				2.97821103,
				60.02391911,
				0.67047034
			},
			{
				45.0,
				11.96458831,
				4.63381419,
				60.65492548,
				0.75336633
			},
			{
				50.0,
				14.04901782,
				7.57043206,
				61.65340551,
				0.83518048
			},
			{
				55.0,
				14.25180947,
				10.95026286,
				63.13860578,
				0.91537187
			},
			{
				60.0,
				34.69303182,
				14.28637103,
				65.10776253,
				0.99339958
			},
			{
				65.0,
				11.1754653,
				22.12593219,
				67.85045045,
				1.06872269
			},
			{
				70.0,
				202.71534041,
				24.53790023,
				71.20755668,
				1.14066505
			},
			{
				75.0,
				-173.6636948,
				65.73993304,
				77.32390065,
				1.20841528
			},
			{
				80.0,
				6340.38872653,
				33.47217309,
				83.46863618,
				1.27035062
			},
			{
				85.0,
				-10081.29471342,
				977.4814281,
				133.64166694,
				1.31998003
			}
		};

		private static double RadsInDegree;

		private static double DegreesInRad;

		private static double Cos35;

		private static double Eckert1Constant;

		private static double Eckert3ConstantA;

		private static double Eckert3ConstantB;

		private static double Eckert3ConstantC;

		private GridSection[,] gridSections;

		private int gridSectionsXCount;

		private int gridSectionsYCount;

		private int gridSectionsInViewportXCount;

		private int gridSectionsInViewportYCount;

		private Point[] gridSectionsArray;

		private Size gridSectionSize = Size.Empty;

		private Point gridSectionsOffset = Point.Empty;

		private int suspendUpdatesCount;

		private bool autoUpdates = true;

		public FieldCollection GroupFields => groupFields;

		public FieldCollection ShapeFields => shapeFields;

		public FieldCollection PathFields => pathFields;

		public FieldCollection SymbolFields => symbolFields;

		public GroupRuleCollection GroupRules => groupRules;

		public ShapeRuleCollection ShapeRules => shapeRules;

		public PathRuleCollection PathRules => pathRules;

		public SymbolRuleCollection SymbolRules => symbolRules;

		public GroupCollection Groups => groups;

		public LayerCollection Layers => layers;

		public ShapeCollection Shapes => shapes;

		public PathCollection Paths => paths;

		public SymbolCollection Symbols => symbols;

		public NamedImageCollection NamedImages => namedImages;

		public MapImageCollection Images => images;

		public MapLabelCollection Labels => labels;

		public LegendCollection Legends => legends;

		public DataBindingRulesCollection DataBindingRules => dataBindingRules;

		public MapAreaCollection MapAreas => mapAreas;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Viewport Viewport
		{
			get
			{
				return viewport;
			}
			set
			{
				viewport = value;
				viewport.Common = Common;
				ZoomPanel.UpdateZoomRange();
				ZoomPanel.ZoomLevel = viewport.Zoom;
				Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ZoomPanel ZoomPanel
		{
			get
			{
				return zoomPanel;
			}
			set
			{
				zoomPanel = value;
				zoomPanel.Common = Common;
				ZoomPanel.UpdateZoomRange();
				ZoomPanel.ZoomLevel = viewport.Zoom;
				Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavigationPanel NavigationPanel
		{
			get
			{
				return navigationPanel;
			}
			set
			{
				navigationPanel = value;
				navigationPanel.Common = Common;
				Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DistanceScalePanel DistanceScalePanel
		{
			get
			{
				return distanceScalePanel;
			}
			set
			{
				distanceScalePanel = value;
				distanceScalePanel.Common = Common;
				Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColorSwatchPanel ColorSwatchPanel
		{
			get
			{
				return colorSwatchPanel;
			}
			set
			{
				colorSwatchPanel = value;
				colorSwatchPanel.Common = Common;
				Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridAttributes Parallels
		{
			get
			{
				return parallels;
			}
			set
			{
				parallels = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridAttributes Meridians
		{
			get
			{
				return meridians;
			}
			set
			{
				meridians = value;
			}
		}

		[DefaultValue(false)]
		public bool GridUnderContent
		{
			get
			{
				return gridUnderContent;
			}
			set
			{
				gridUnderContent = value;
				InvalidateViewport();
			}
		}

		[DefaultValue(typeof(AntiAliasing), "All")]
		public AntiAliasing AntiAliasing
		{
			get
			{
				return antiAliasing;
			}
			set
			{
				antiAliasing = value;
				InvalidateViewport();
				Invalidate();
			}
		}

		[DefaultValue(TextAntiAliasingQuality.High)]
		public TextAntiAliasingQuality TextAntiAliasingQuality
		{
			get
			{
				return textAntiAliasingQuality;
			}
			set
			{
				textAntiAliasingQuality = value;
				InvalidateViewport();
				Invalidate();
			}
		}

		[DefaultValue(25f)]
		public float ShadowIntensity
		{
			get
			{
				return shadowIntensity;
			}
			set
			{
				shadowIntensity = value;
				InvalidateViewport();
				Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "White")]
		public Color BackColor
		{
			get
			{
				return MapControl.BackColor;
			}
			set
			{
				MapControl.BackColor = value;
				InvalidateViewport();
				Invalidate();
			}
		}

		[DefaultValue(GradientType.None)]
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

		[DefaultValue(typeof(Color), "")]
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

		[DefaultValue(MapHatchStyle.None)]
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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Frame Frame
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

		[SerializationVisibility(SerializationVisibility.Hidden)]
		public int Width
		{
			get
			{
				return GetWidth();
			}
			set
			{
				if (isPrinting)
				{
					if (Viewport.ContentSize == 0)
					{
						InvalidateCachedPaths();
						InvalidateGridSections();
					}
					InvalidateAndLayout();
					printSize.Width = value;
				}
				else
				{
					MapControl.Width = value;
				}
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		public int Height
		{
			get
			{
				return GetHeight();
			}
			set
			{
				if (isPrinting)
				{
					if (Viewport.ContentSize == 0)
					{
						InvalidateCachedPaths();
						InvalidateGridSections();
					}
					InvalidateAndLayout();
					printSize.Height = value;
				}
				else
				{
					MapControl.Height = value;
				}
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public override string Name
		{
			get
			{
				return "MapControl";
			}
			set
			{
			}
		}

		[SRDescription("DescriptionAttributeMapCore_BuildNumber")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue("")]
		public string BuildNumber
		{
			get
			{
				string text = string.Empty;
				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				if (executingAssembly != null)
				{
					text = executingAssembly.FullName.ToUpper(CultureInfo.CurrentCulture);
					int num = text.IndexOf("VERSION=", StringComparison.Ordinal);
					if (num >= 0)
					{
						text = text.Substring(num + 8);
					}
					num = text.IndexOf(",", StringComparison.Ordinal);
					if (num >= 0)
					{
						text = text.Substring(0, num);
					}
				}
				return text;
			}
			set
			{
				loadedBuildNumber = value;
			}
		}

		[SRDescription("DescriptionAttributeMapCore_ControlType")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue("")]
		public string ControlType
		{
			get
			{
				return "DundasWebMap";
			}
			set
			{
			}
		}

		[DefaultValue(Projection.Equirectangular)]
		public Projection Projection
		{
			get
			{
				return projection;
			}
			set
			{
				projection = value;
				InvalidateCachedBounds();
				InvalidateCachedPaths();
				ResetCachedBoundsAfterProjection();
				InvalidateDistanceScalePanel();
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapCore_BorderLineColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "DarkGray")]
		public Color BorderLineColor
		{
			get
			{
				return borderLineColor;
			}
			set
			{
				borderLineColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapCore_BorderLineStyle")]
		[NotifyParentProperty(true)]
		[DefaultValue(MapDashStyle.Solid)]
		public MapDashStyle BorderLineStyle
		{
			get
			{
				return borderLineStyle;
			}
			set
			{
				borderLineStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapCore_BorderLineWidth")]
		[NotifyParentProperty(true)]
		[DefaultValue(0)]
		public int BorderLineWidth
		{
			get
			{
				return borderLineWidth;
			}
			set
			{
				if (value < 0 || value > 100)
				{
					throw new ArgumentException(SR.must_in_range(0.0, 100.0));
				}
				borderLineWidth = value;
				InvalidateAndLayout();
			}
		}

		[DefaultValue(typeof(Color), "LightBlue")]
		public Color SelectionMarkerColor
		{
			get
			{
				return selectionMarkerColor;
			}
			set
			{
				selectionMarkerColor = value;
				InvalidateViewport();
				Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "Black")]
		public Color SelectionBorderColor
		{
			get
			{
				return selectionBorderColor;
			}
			set
			{
				selectionBorderColor = value;
				InvalidateViewport();
				Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MapLimits MapLimits
		{
			get
			{
				return mapLimits;
			}
			set
			{
				mapLimits = value;
				mapLimits.Parent = this;
				InvalidateCachedPaths();
				ResetCachedBoundsAfterProjection();
				InvalidateDistanceScalePanel();
				InvalidateViewport();
			}
		}

		[DefaultValue(false)]
		public bool AutoLimitsIgnoreSymbols
		{
			get
			{
				return autoLimitsIgnoreSymbols;
			}
			set
			{
				autoLimitsIgnoreSymbols = value;
				InvalidateCachedPaths();
				ResetCachedBoundsAfterProjection();
				InvalidateDistanceScalePanel();
				InvalidateViewport();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ProjectionCenter ProjectionCenter
		{
			get
			{
				return projectionCenter;
			}
			set
			{
				projectionCenter = value;
				projectionCenter.Parent = this;
				InvalidateCachedPaths();
				ResetCachedBoundsAfterProjection();
				InvalidateDistanceScalePanel();
				InvalidateViewport();
			}
		}

		[DefaultValue(true)]
		public bool GeographyMode
		{
			get
			{
				return geographyMode;
			}
			set
			{
				geographyMode = value;
				InvalidateCachedBounds();
				InvalidateCachedPaths();
				ResetCachedBoundsAfterProjection();
				InvalidateDistanceScalePanel();
				InvalidateViewport();
			}
		}

		[DefaultValue(10000)]
		public int TileServerTimeout
		{
			get
			{
				return tileServerTimeout;
			}
			set
			{
				tileServerTimeout = value;
			}
		}

		[DefaultValue(2)]
		public int TileServerMaxConnections
		{
			get
			{
				return tileServerMaxConnections;
			}
			set
			{
				tileServerMaxConnections = value;
			}
		}

		[DefaultValue(typeof(CultureInfo), "Invariant Language (Invariant Country)")]
		public CultureInfo TileCulture
		{
			get
			{
				return tileCulture;
			}
			set
			{
				if (tileCulture != value)
				{
					tileCulture = value;
					InvalidateViewport();
				}
			}
		}

		[DefaultValue(RequestCacheLevel.Default)]
		public RequestCacheLevel TileCacheLevel
		{
			get
			{
				return tileCacheLevel;
			}
			set
			{
				if (tileCacheLevel != value)
				{
					tileCacheLevel = value;
					InvalidateViewport();
				}
			}
		}

		[DefaultValue("ArHDYkG4iBBgRo5ZKlvBPAXjj3tA13t5WMpr60l7m23-SSyZJoHsVVe2IecRKN88")]
		internal string TileServerAppId
		{
			get
			{
				return tileServerAppId;
			}
			set
			{
				if (tileServerAppId != value)
				{
					tileServerAppId = value;
					InvalidateViewport();
				}
			}
		}

		[DefaultValue(ImageType.Png)]
		public ImageType ImageType
		{
			get
			{
				return imageType;
			}
			set
			{
				imageType = value;
			}
		}

		[DefaultValue(0)]
		public int Compression
		{
			get
			{
				return compression;
			}
			set
			{
				compression = value;
			}
		}

		[DefaultValue("TempFiles/MapPic_#SEQ(300,3)")]
		public string ImageUrl
		{
			get
			{
				return mapImageUrl;
			}
			set
			{
				mapImageUrl = value;
			}
		}

		[DefaultValue(true)]
		public bool ImageMapEnabled
		{
			get
			{
				return imageMapEnabled;
			}
			set
			{
				imageMapEnabled = value;
			}
		}

		[DefaultValue(RenderType.InteractiveImage)]
		public RenderType RenderType
		{
			get
			{
				return renderType;
			}
			set
			{
				renderType = value;
			}
		}

		[DefaultValue(ControlPersistence.SessionState)]
		public ControlPersistence ControlPersistence
		{
			get
			{
				return controlPersistence;
			}
			set
			{
				controlPersistence = value;
			}
		}

		[DefaultValue("")]
		public string RenderingImageUrl
		{
			get
			{
				return renderingImageUrl;
			}
			set
			{
				renderingImageUrl = value;
			}
		}

		[DefaultValue(SerializationContent.All)]
		public SerializationContent ViewStateContent
		{
			get
			{
				return viewStateContent;
			}
			set
			{
				viewStateContent = value;
			}
		}

		[DefaultValue("")]
		public string TagAttributes
		{
			get
			{
				return tagAttributes;
			}
			set
			{
				tagAttributes = value;
			}
		}

		[DefaultValue(false)]
		public bool ContentCachingEnabled
		{
			get
			{
				return contentCachingEnabled;
			}
			set
			{
				contentCachingEnabled = value;
			}
		}

		[DefaultValue(0.0)]
		public double ContentCachingTimeout
		{
			get
			{
				return contentCachingTimeout;
			}
			set
			{
				contentCachingTimeout = value;
			}
		}

		internal object DataSource
		{
			get
			{
				return dataSource;
			}
			set
			{
				if (!DataBindingHelper.IsValidDataSource(value))
				{
					throw new ArgumentException(SR.not_supported_DataSource(value.GetType().Name));
				}
				dataSource = value;
				InvalidateDataBinding();
				Invalidate();
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		internal MapSerializer Serializer => serializer;

		[SerializationVisibility(SerializationVisibility.Hidden)]
		internal CallbackManager CallbackManager => callbackManager;

		internal MapControl MapControl => parent;

		internal HotRegionList HotRegionList
		{
			get
			{
				if (hotRegionList == null)
				{
					hotRegionList = new HotRegionList(this);
				}
				return hotRegionList;
			}
		}

		internal bool Serializing
		{
			get
			{
				return serializing;
			}
			set
			{
				serializing = value;
			}
		}

		internal MapPoint MapCenterPoint
		{
			get
			{
				MapPoint result = mapCenterPoint;
				if (!ProjectionCenter.IsXNaN())
				{
					result.X = ProjectionCenter.X;
				}
				if (!ProjectionCenter.IsYNaN())
				{
					result.Y = ProjectionCenter.Y;
				}
				return result;
			}
			set
			{
				mapCenterPoint = value;
				ResetCachedBoundsAfterProjection();
			}
		}

		internal MapPoint MinimumPoint
		{
			get
			{
				MapPoint result = minimumPoint;
				if (!MapLimits.IsMinimumXNaN())
				{
					result.X = MapLimits.MinimumX;
				}
				if (!MapLimits.IsMinimumYNaN())
				{
					result.Y = MapLimits.MinimumY;
				}
				return result;
			}
			set
			{
				minimumPoint = value;
				ResetCachedBoundsAfterProjection();
			}
		}

		internal MapPoint MaximumPoint
		{
			get
			{
				MapPoint result = maximumPoint;
				if (!MapLimits.IsMaximumXNaN())
				{
					result.X = MapLimits.MaximumX;
				}
				if (!MapLimits.IsMaximumYNaN())
				{
					result.Y = MapLimits.MaximumY;
				}
				return result;
			}
			set
			{
				maximumPoint = value;
				ResetCachedBoundsAfterProjection();
			}
		}

		internal RectangleF MapDockBounds
		{
			get
			{
				return mapDockBounds;
			}
			set
			{
				if (mapDockBounds != value)
				{
					mapDockBounds = value;
					InvalidateAndLayout();
				}
			}
		}

		internal ISelectable SelectedDesignTimeElement
		{
			get
			{
				NamedElement namedElement = selectedDesignTimeElement as NamedElement;
				if (namedElement != null && namedElement.Common == null)
				{
					selectedDesignTimeElement = null;
				}
				return selectedDesignTimeElement;
			}
			set
			{
				selectedDesignTimeElement = value;
				InvalidateViewport();
				Invalidate();
			}
		}

		internal bool UseRSAccessibilityNames
		{
			get
			{
				return useRSAccessibilityNames;
			}
			set
			{
				useRSAccessibilityNames = value;
				InvalidateViewport();
				Invalidate();
			}
		}

		internal bool DoPanelLayout
		{
			get
			{
				return doPanelLayout;
			}
			set
			{
				doPanelLayout = value;
			}
		}

		internal RenderingMode RenderingMode
		{
			get
			{
				return renderingMode;
			}
			set
			{
				renderingMode = value;
			}
		}

		internal Panel PanelToRender
		{
			get
			{
				return panelToRender;
			}
			set
			{
				panelToRender = value;
			}
		}

		internal int SingleGridSectionX
		{
			get
			{
				return singleGridSectionX;
			}
			set
			{
				singleGridSectionX = value;
			}
		}

		internal int SingleGridSectionY
		{
			get
			{
				return singleGridSectionY;
			}
			set
			{
				singleGridSectionY = value;
			}
		}

		internal Hashtable TileImagesCache
		{
			get
			{
				return tileImagesCache;
			}
			set
			{
				tileImagesCache = value;
			}
		}

		private Image TileWaitImage
		{
			get
			{
				if (tileWaitImage == null)
				{
					tileWaitImage = new Bitmap(256, 256);
					Utils.SetImageCustomProperty(tileWaitImage, CustomPropertyTag.ImageError, SR.DownloadingTile);
				}
				return tileWaitImage;
			}
		}

		internal bool UppercaseFieldKeywords
		{
			get
			{
				return uppercaseFieldKeywords;
			}
			set
			{
				uppercaseFieldKeywords = value;
			}
		}

		internal int MaxSpatialPointCount
		{
			get
			{
				return maxSpatialPointCount;
			}
			set
			{
				maxSpatialPointCount = value;
			}
		}

		internal int MaxSpatialElementCount
		{
			get
			{
				return maxSpatialElementCount;
			}
			set
			{
				maxSpatialElementCount = value;
			}
		}

		internal bool InvokeRequired => false;

		internal GridSection[,] GridSections
		{
			get
			{
				return gridSections;
			}
			set
			{
				gridSections = value;
			}
		}

		internal int GridSectionsXCount
		{
			get
			{
				return gridSectionsXCount;
			}
			set
			{
				gridSectionsXCount = value;
			}
		}

		internal int GridSectionsYCount
		{
			get
			{
				return gridSectionsYCount;
			}
			set
			{
				gridSectionsYCount = value;
			}
		}

		internal int GridSectionsInViewportXCount
		{
			get
			{
				return gridSectionsInViewportXCount;
			}
			set
			{
				gridSectionsInViewportXCount = value;
			}
		}

		internal int GridSectionsInViewportYCount
		{
			get
			{
				return gridSectionsInViewportYCount;
			}
			set
			{
				gridSectionsInViewportYCount = value;
			}
		}

		internal Point[] GridSectionsArray
		{
			get
			{
				return gridSectionsArray;
			}
			set
			{
				gridSectionsArray = value;
			}
		}

		internal Size GridSectionSize
		{
			get
			{
				return gridSectionSize;
			}
			set
			{
				gridSectionSize = value;
			}
		}

		internal Point GridSectionsOffset
		{
			get
			{
				return gridSectionsOffset;
			}
			set
			{
				gridSectionsOffset = value;
			}
		}

		internal bool IsSuspended => suspendUpdatesCount > 0;

		internal bool AutoUpdates
		{
			get
			{
				return autoUpdates;
			}
			set
			{
				autoUpdates = value;
			}
		}

		static MapCore()
		{
			_geoPoliticalBlockListLocales = new string[21]
			{
				"ES-AR",
				"ZH-CN",
				"ZH-HK",
				"AS-IN",
				"BN-IN",
				"EN-IN",
				"KOK-IN",
				"MR-IN",
				"HI-IN",
				"PA-IN",
				"GU-IN",
				"OR-IN",
				"TA-IN",
				"TE-IN",
				"KN-IN",
				"ML-IN",
				"SA-IN",
				"KO-KR",
				"ZH-MO",
				"AR-MA",
				"ES-VE"
			};
			_geoPoliticalBlockHashtableLocales = new Hashtable(StringComparer.OrdinalIgnoreCase);
			RadsInDegree = Math.PI / 180.0;
			DegreesInRad = 180.0 / Math.PI;
			Cos35 = Math.Cos(Math.PI * 7.0 / 36.0);
			Eckert1Constant = 2.0 * Math.Sqrt(0.21220659078919379);
			Eckert3ConstantA = 2.0 / Math.Sqrt(22.43597501544853);
			Eckert3ConstantB = 4.0 / Math.Sqrt(22.43597501544853);
			Eckert3ConstantC = Math.Sqrt(22.43597501544853);
			for (int i = 0; i < _geoPoliticalBlockListLocales.Length; i++)
			{
				_geoPoliticalBlockHashtableLocales.Add(_geoPoliticalBlockListLocales[i], new object());
			}
		}

		public MapCore()
			: this(null)
		{
		}

		internal MapCore(MapControl parent)
		{
			this.parent = parent;
			serviceContainer = new ServiceContainer();
			serviceContainer.AddService(typeof(MapCore), this);
			serviceContainer.AddService(typeof(MapControl), this.parent);
			borderTypeRegistry = new BorderTypeRegistry(serviceContainer);
			serviceContainer.AddService(borderTypeRegistry.GetType(), borderTypeRegistry);
			common = new CommonElements(serviceContainer);
			shapeFields = new FieldCollection(this, common);
			pathFields = new FieldCollection(this, common);
			symbolFields = new FieldCollection(this, common);
			groupFields = new FieldCollection(this, common);
			shapes = new ShapeCollection(this, common);
			paths = new PathCollection(this, common);
			symbols = new SymbolCollection(this, common);
			shapeRules = new ShapeRuleCollection(this, common);
			pathRules = new PathRuleCollection(this, common);
			groupRules = new GroupRuleCollection(this, common);
			symbolRules = new SymbolRuleCollection(this, common);
			images = new MapImageCollection(this, common);
			labels = new MapLabelCollection(this, common);
			groups = new GroupCollection(this, common);
			layers = new LayerCollection(this, common);
			dataBindingRules = new DataBindingRulesCollection(this, common);
			legends = new LegendCollection(this, common);
			viewport = new Viewport(common);
			parallels = new GridAttributes(this, parallels: true);
			meridians = new GridAttributes(this, parallels: false);
			zoomPanel = new ZoomPanel(common);
			navigationPanel = new NavigationPanel(common);
			colorSwatchPanel = new ColorSwatchPanel(common);
			distanceScalePanel = new DistanceScalePanel(common);
			serializer = new MapSerializer(serviceContainer);
			frame = new Frame(this);
			mapLimits = new MapLimits(this);
			projectionCenter = new ProjectionCenter(this);
			borderTypeRegistry.Register("Emboss", typeof(EmbossBorder));
			borderTypeRegistry.Register("Raised", typeof(RaisedBorder));
			borderTypeRegistry.Register("Sunken", typeof(SunkenBorder));
			borderTypeRegistry.Register("FrameThin1", typeof(FrameThin1Border));
			borderTypeRegistry.Register("FrameThin2", typeof(FrameThin2Border));
			borderTypeRegistry.Register("FrameThin3", typeof(FrameThin3Border));
			borderTypeRegistry.Register("FrameThin4", typeof(FrameThin4Border));
			borderTypeRegistry.Register("FrameThin5", typeof(FrameThin5Border));
			borderTypeRegistry.Register("FrameThin6", typeof(FrameThin6Border));
			borderTypeRegistry.Register("FrameTitle1", typeof(FrameTitle1Border));
			borderTypeRegistry.Register("FrameTitle2", typeof(FrameTitle2Border));
			borderTypeRegistry.Register("FrameTitle3", typeof(FrameTitle3Border));
			borderTypeRegistry.Register("FrameTitle4", typeof(FrameTitle4Border));
			borderTypeRegistry.Register("FrameTitle5", typeof(FrameTitle5Border));
			borderTypeRegistry.Register("FrameTitle6", typeof(FrameTitle6Border));
			borderTypeRegistry.Register("FrameTitle7", typeof(FrameTitle7Border));
			borderTypeRegistry.Register("FrameTitle8", typeof(FrameTitle8Border));
			mapAreas = new MapAreaCollection();
			callbackManager = new CallbackManager(this);
			namedImages = new NamedImageCollection(this, common);
			imageLoader = new ImageLoader(serviceContainer);
			tileImagesCache = new Hashtable(new CaseInsensitiveHashCodeProvider(CultureInfo.InvariantCulture), StringComparer.OrdinalIgnoreCase);
			serviceContainer.AddService(typeof(ImageLoader), imageLoader);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				DisposeGridSections();
				NamedCollection[] renderCollections = GetRenderCollections();
				for (int i = 0; i < renderCollections.Length; i++)
				{
					renderCollections[i].Dispose();
				}
				if (viewport != null)
				{
					viewport.Dispose();
				}
				if (parallels != null)
				{
					parallels.Dispose();
				}
				if (meridians != null)
				{
					meridians.Dispose();
				}
				if (zoomPanel != null)
				{
					zoomPanel.Dispose();
				}
				if (navigationPanel != null)
				{
					navigationPanel.Dispose();
				}
				if (colorSwatchPanel != null)
				{
					colorSwatchPanel.Dispose();
				}
				if (distanceScalePanel != null)
				{
					distanceScalePanel.Dispose();
				}
				if (frame != null)
				{
					frame.Dispose();
				}
				if (hotRegionList != null)
				{
					hotRegionList.Clear();
				}
				if (bufferBitmap != null)
				{
					bufferBitmap.Dispose();
				}
				if (namedImages != null)
				{
					namedImages.Dispose();
				}
				if (imageLoader != null)
				{
					imageLoader.Dispose();
				}
				if (tileImagesCache != null)
				{
					foreach (DictionaryEntry item in tileImagesCache)
					{
						if (item.Value != null)
						{
							((Image)item.Value).Dispose();
						}
					}
					tileImagesCache = null;
				}
			}
			base.Dispose(disposing);
		}

		private static Type DetermineElementTypeFromGeometryType(string geometryType)
		{
			switch (geometryType)
			{
			case "MultiPolygon":
			case "Polygon":
			case "CurvePolygon":
			case "FullGlobe":
				return typeof(Shape);
			case "MultiLineString":
			case "LineString":
			case "CircularString":
			case "CompoundCurve":
				return typeof(Path);
			case "MultiPoint":
			case "Point":
				return typeof(Symbol);
			default:
				return null;
			}
		}

		internal static BasicMapElements? DetermineMapElementsFromSpatial(DataTable spatialTable, string spatialColumn)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			if (num > 0 && num >= num2 && num >= num3)
			{
				return BasicMapElements.Shapes;
			}
			if (num2 > 0 && num2 >= num3)
			{
				return BasicMapElements.Paths;
			}
			if (num3 > 0)
			{
				return BasicMapElements.Symbols;
			}
			return null;
		}

		internal void LoadFromSpatial(string connectionString, string sqlStatement, string nameColumn, string spatialColumn, BasicMapElements mapElementsToLoad, bool importAllData, string layer)
		{
			DataTable dataTable = null;
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				sqlConnection.Open();
				using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
				{
					using (SqlCommand selectCommand = new SqlCommand(sqlStatement, sqlConnection))
					{
						sqlDataAdapter.SelectCommand = selectCommand;
						DataSet dataSet = new DataSet();
						dataSet.Locale = CultureInfo.CurrentCulture;
						sqlDataAdapter.Fill(dataSet);
						dataTable = dataSet.Tables[0];
					}
				}
			}
			LoadFromSpatial(dataTable, nameColumn, spatialColumn, mapElementsToLoad, importAllData, layer);
		}

		internal void LoadFromSpatial(DataTable dataTable, string nameColumn, string spatialColumn, BasicMapElements mapElementsToLoad, bool importAllData, string layer)
		{
			if (!importAllData)
			{
				LoadFromSpatial(dataTable, nameColumn, spatialColumn, mapElementsToLoad, null, null, layer, string.Empty);
				return;
			}
			string[] array = new string[dataTable.Columns.Count];
			ColumnImportMode[] array2 = new ColumnImportMode[dataTable.Columns.Count];
			for (int i = 0; i < dataTable.Columns.Count; i++)
			{
				array[i] = dataTable.Columns[i].ColumnName;
				array2[i] = ColumnImportMode.FirstValue;
			}
			LoadFromSpatial(dataTable, nameColumn, spatialColumn, mapElementsToLoad, array, array2, layer, string.Empty);
		}

		internal void LoadFromSpatial(DataTable dataTable, string nameColumn, string spatialColumn, BasicMapElements mapElementsToLoad, string[] additionalColumnsToImport, string layer)
		{
			ColumnImportMode[] array = new ColumnImportMode[additionalColumnsToImport.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = ColumnImportMode.FirstValue;
			}
			LoadFromSpatial(dataTable, nameColumn, spatialColumn, mapElementsToLoad, additionalColumnsToImport, array, layer, string.Empty);
		}

		internal void LoadFromSpatial(DataTable spatialTable, string nameColumn, string spatialColumn, BasicMapElements mapElementsToLoad, string[] columnsToImport, ColumnImportMode[] importModes, string layer, string category)
		{
			switch (mapElementsToLoad)
			{
			case BasicMapElements.Shapes:
				CreateShapesFromSpatial(spatialTable, nameColumn, spatialColumn, columnsToImport, importModes, layer, category);
				break;
			case BasicMapElements.Paths:
				CreatePathsFromSpatial(spatialTable, nameColumn, spatialColumn, columnsToImport, importModes, layer, category);
				break;
			case BasicMapElements.Symbols:
				CreateSymbolsFromSpatial(spatialTable, nameColumn, spatialColumn, columnsToImport, layer, category);
				break;
			}
			InvalidateCachedBounds();
			InvalidateCachedPaths();
			InvalidateRules();
			InvalidateDataBinding();
		}

		private void CreateShapesFromSpatial(DataTable spatialTable, string nameColumn, string spatialColumn, string[] columnsToImport, ColumnImportMode[] importModes, string layer, string category)
		{
			if (columnsToImport != null)
			{
				string[] array = columnsToImport;
				foreach (string text in array)
				{
					if (ShapeFields.GetByName(text) == null && text != spatialColumn)
					{
						ShapeFields.Add(text).Type = spatialTable.Columns[text].DataType;
					}
				}
			}
			foreach (DataRow row in spatialTable.Rows)
			{
				string name = "Shape" + (Shapes.Count + 1).ToString(CultureInfo.CurrentCulture);
				if (!string.IsNullOrEmpty(nameColumn))
				{
					try
					{
						name = row[nameColumn].ToString();
					}
					catch
					{
						name = "(null)";
					}
				}
				Shape shape = (Shape)Shapes.GetByName(name);
				bool flag = false;
				if (shape == null)
				{
					Shapes.SuppressAddedAndRemovedEvents = true;
					shape = Shapes.Add(name);
					flag = true;
				}
				if (columnsToImport != null)
				{
					string[] array = columnsToImport;
					foreach (string text2 in array)
					{
						if (text2 != spatialColumn)
						{
							try
							{
								shape[text2] = row[text2];
							}
							catch
							{
							}
						}
					}
				}
				if (flag && (shape.ShapeData.Points == null || shape.ShapeData.Points.Length == 0))
				{
					Shapes.Remove(shape);
					if (Shapes.SuppressAddedAndRemovedEvents)
					{
						Shapes.SuppressAddedAndRemovedEvents = false;
					}
					continue;
				}
				shape.Layer = layer;
				shape.Category = category;
				if (Shapes.SuppressAddedAndRemovedEvents)
				{
					Common.InvokeElementAdded(shape);
					Shapes.SuppressAddedAndRemovedEvents = false;
				}
			}
		}

		private void CreatePathsFromSpatial(DataTable spatialTable, string nameColumn, string spatialColumn, string[] columnsToImport, ColumnImportMode[] importModes, string layer, string category)
		{
			if (columnsToImport != null)
			{
				string[] array = columnsToImport;
				foreach (string text in array)
				{
					if (PathFields.GetByName(text) == null && text != spatialColumn)
					{
						PathFields.Add(text).Type = spatialTable.Columns[text].DataType;
					}
				}
			}
			foreach (DataRow row in spatialTable.Rows)
			{
				string name = "Path" + (Paths.Count + 1).ToString(CultureInfo.CurrentCulture);
				if (!string.IsNullOrEmpty(nameColumn))
				{
					try
					{
						name = row[nameColumn].ToString();
					}
					catch
					{
						name = "(null)";
					}
				}
				Path path = (Path)Paths.GetByName(name);
				bool flag = false;
				if (path == null)
				{
					Paths.SuppressAddedAndRemovedEvents = true;
					path = Paths.Add(name);
					flag = true;
				}
				if (columnsToImport != null)
				{
					string[] array = columnsToImport;
					foreach (string text2 in array)
					{
						if (text2 != spatialColumn)
						{
							try
							{
								path[text2] = row[text2];
							}
							catch
							{
							}
						}
					}
				}
				if (flag && (path.PathData.Points == null || path.PathData.Points.Length == 0))
				{
					Paths.Remove(path);
					if (Paths.SuppressAddedAndRemovedEvents)
					{
						Paths.SuppressAddedAndRemovedEvents = false;
					}
					continue;
				}
				path.Layer = layer;
				path.Category = category;
				if (Paths.SuppressAddedAndRemovedEvents)
				{
					Common.InvokeElementAdded(path);
					Paths.SuppressAddedAndRemovedEvents = false;
				}
			}
		}

		private void CreateSymbolsFromSpatial(DataTable spatialTable, string nameColumn, string spatialColumn, string[] columnsToImport, string layer, string category)
		{
			if (columnsToImport != null)
			{
				string[] array = columnsToImport;
				foreach (string text in array)
				{
					if (SymbolFields.GetByName(text) == null && text != spatialColumn)
					{
						SymbolFields.Add(text).Type = spatialTable.Columns[text].DataType;
					}
				}
			}
			foreach (DataRow row in spatialTable.Rows)
			{
				string name = "Symbol" + (Symbols.Count + 1).ToString(CultureInfo.CurrentCulture);
				if (!string.IsNullOrEmpty(nameColumn))
				{
					try
					{
						name = row[nameColumn].ToString();
					}
					catch
					{
						name = "(null)";
					}
				}
				Symbol symbol = (Symbol)Symbols.GetByName(name);
				bool flag = false;
				if (symbol == null)
				{
					Symbols.SuppressAddedAndRemovedEvents = true;
					symbol = Symbols.Add(name);
					flag = true;
				}
				if (columnsToImport != null)
				{
					string[] array = columnsToImport;
					foreach (string text2 in array)
					{
						if (text2 != spatialColumn)
						{
							try
							{
								symbol[text2] = row[text2];
							}
							catch
							{
							}
						}
					}
				}
				bool flag2 = false;
				if (!flag2 && flag)
				{
					Symbols.Remove(symbol);
					if (Symbols.SuppressAddedAndRemovedEvents)
					{
						Symbols.SuppressAddedAndRemovedEvents = false;
					}
					continue;
				}
				symbol.Layer = layer;
				symbol.Category = category;
				if (Symbols.SuppressAddedAndRemovedEvents)
				{
					Common.InvokeElementAdded(symbol);
					Symbols.SuppressAddedAndRemovedEvents = false;
				}
			}
		}

		internal SpatialLoadResult LoadFromShapeFileStreams(Stream shpStream, Stream dbfStream, string[] columnsToImport, string[] destinationFields, string layer, string category)
		{
			ShapeFileReader shapeFileReader = new ShapeFileReader();
			shapeFileReader.ShpStream = shpStream;
			shapeFileReader.DbfStream = dbfStream;
			shapeFileReader.Load();
			ColumnImportMode[] array = null;
			if (columnsToImport != null)
			{
				array = new ColumnImportMode[columnsToImport.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = ColumnImportMode.FirstValue;
				}
			}
			return LoadFromShapeReader(shapeFileReader, string.Empty, columnsToImport, destinationFields, array, layer, category);
		}

		internal SpatialLoadResult LoadFromShapeFileStreams(Stream shpStream, Stream dbfStream, string layer, string category)
		{
			ShapeFileReader shapeFileReader = new ShapeFileReader();
			shapeFileReader.ShpStream = shpStream;
			shapeFileReader.DbfStream = dbfStream;
			shapeFileReader.Load();
			string[] array = new string[shapeFileReader.Table.Columns.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = shapeFileReader.Table.Columns[i].ColumnName;
			}
			ColumnImportMode[] array2 = null;
			if (array != null)
			{
				array2 = new ColumnImportMode[array.Length];
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = ColumnImportMode.FirstValue;
				}
			}
			return LoadFromShapeReader(shapeFileReader, string.Empty, array, array, array2, layer, category);
		}

		internal void LoadFromShapeFile(string fileName, string nameColumn, bool importData)
		{
			ShapeFileReader shapeFileReader = new ShapeFileReader();
			shapeFileReader.FileName = fileName;
			shapeFileReader.Load();
			if (!importData)
			{
				LoadFromShapeReader(shapeFileReader, nameColumn, null, null);
				return;
			}
			string[] array = new string[shapeFileReader.Table.Columns.Count];
			ColumnImportMode[] array2 = new ColumnImportMode[shapeFileReader.Table.Columns.Count];
			for (int i = 0; i < shapeFileReader.Table.Columns.Count; i++)
			{
				array[i] = shapeFileReader.Table.Columns[i].ColumnName;
				array2[i] = ColumnImportMode.FirstValue;
			}
			LoadFromShapeReader(shapeFileReader, nameColumn, array, array2);
		}

		internal void LoadFromShapeFile(string fileName, string nameColumn, string[] columnsToImport, ColumnImportMode[] importModes)
		{
			ShapeFileReader shapeFileReader = new ShapeFileReader();
			shapeFileReader.FileName = fileName;
			shapeFileReader.Load();
			LoadFromShapeReader(shapeFileReader, nameColumn, columnsToImport, importModes);
		}

		internal void LoadFromShapeReader(ShapeFileReader shapeReader, string nameColumn, string[] columnsToImport, ColumnImportMode[] importModes)
		{
			LoadFromShapeReader(shapeReader, nameColumn, columnsToImport, columnsToImport, importModes, string.Empty, string.Empty);
		}

		internal SpatialLoadResult LoadFromShapeReader(ShapeFileReader shapeReader, string nameColumn, string[] columnsToImport, string[] destinationFields, ColumnImportMode[] importModes, string layer, string category)
		{
			SpatialLoadResult result = SpatialLoadResult.AllSpatialElementsLoaded;
			if (shapeReader.ShapeType == ShapeType.Polygon)
			{
				result = CreateShapes(shapeReader, nameColumn, columnsToImport, destinationFields, importModes, layer, category);
			}
			else if (shapeReader.ShapeType == ShapeType.PolyLine)
			{
				result = CreatePaths(shapeReader, nameColumn, columnsToImport, destinationFields, importModes, layer, category);
			}
			else if (shapeReader.ShapeType == ShapeType.Point || shapeReader.ShapeType == ShapeType.MultiPoint)
			{
				result = CreateSymbols(shapeReader, nameColumn, columnsToImport, destinationFields, layer, category);
			}
			InvalidateCachedBounds();
			InvalidateCachedPaths();
			InvalidateRules();
			InvalidateDataBinding();
			return result;
		}

		private SpatialLoadResult CreateShapes(ShapeFileReader shapeReader, string nameColumn, string[] columnsToImport, string[] destinationFields, ColumnImportMode[] importModes, string layer, string category)
		{
			if (columnsToImport != null)
			{
				for (int i = 0; i < columnsToImport.Length; i++)
				{
					if (ShapeFields.GetByName(destinationFields[i]) == null)
					{
						ShapeFields.Add(destinationFields[i]).Type = shapeReader.Table.Columns[columnsToImport[i]].DataType;
					}
				}
			}
			int num = Shapes.Count;
			int num2 = 0;
			foreach (PolyLine polygon in shapeReader.Polygons)
			{
				if (GetSpatialElementCount() + 1 > MaxSpatialElementCount)
				{
					return SpatialLoadResult.MaxSpatialElementCountReached;
				}
				if (GetSpatialPointCount() + polygon.Points.Length > MaxSpatialPointCount)
				{
					return SpatialLoadResult.MaxSpatialPointCountReached;
				}
				int num3 = num + 1;
				num = num3;
				string name = "Shape" + num3.ToString(CultureInfo.CurrentCulture);
				if (!string.IsNullOrEmpty(nameColumn))
				{
					try
					{
						name = shapeReader.Table.Rows[num2][nameColumn].ToString();
					}
					catch
					{
						name = "(null)";
					}
				}
				Shape shape = (Shape)Shapes.GetByName(name);
				if (shape == null)
				{
					Shapes.SuppressAddedAndRemovedEvents = true;
					shape = Shapes.Add(name);
				}
				if (columnsToImport != null)
				{
					for (int j = 0; j < columnsToImport.Length; j++)
					{
						try
						{
							shape[destinationFields[j]] = shapeReader.Table.Rows[num2][columnsToImport[j]];
						}
						catch
						{
						}
					}
				}
				MapPoint[] array = new MapPoint[polygon.NumPoints];
				for (int k = 0; k < polygon.NumPoints; k++)
				{
					if (polygon.Points[k].Y < shapeReader.YMin)
					{
						polygon.Points[k].Y = shapeReader.YMin;
					}
					if (polygon.Points[k].Y > shapeReader.YMax)
					{
						polygon.Points[k].Y = shapeReader.YMax;
					}
					array[k].X = polygon.Points[k].X;
					array[k].Y = polygon.Points[k].Y;
				}
				ShapeSegment[] array2 = new ShapeSegment[polygon.NumParts];
				for (int l = 0; l < polygon.NumParts; l++)
				{
					array2[l].Type = SegmentType.Polygon;
					if (l + 1 < polygon.Parts.Length)
					{
						array2[l].Length = polygon.Parts[l + 1] - polygon.Parts[l];
					}
					else
					{
						array2[l].Length = polygon.NumPoints - polygon.Parts[l];
					}
				}
				shape.AddSegments(array, array2);
				shape.Layer = layer;
				shape.Category = category;
				if (Shapes.SuppressAddedAndRemovedEvents)
				{
					Common.InvokeElementAdded(shape);
					Shapes.SuppressAddedAndRemovedEvents = false;
				}
				num2++;
			}
			return SpatialLoadResult.AllSpatialElementsLoaded;
		}

		private SpatialLoadResult CreatePaths(ShapeFileReader shapeReader, string nameColumn, string[] columnsToImport, string[] destinationFields, ColumnImportMode[] importModes, string layer, string category)
		{
			if (columnsToImport != null)
			{
				for (int i = 0; i < columnsToImport.Length; i++)
				{
					if (PathFields.GetByName(destinationFields[i]) == null)
					{
						PathFields.Add(destinationFields[i]).Type = shapeReader.Table.Columns[columnsToImport[i]].DataType;
					}
				}
			}
			int num = Paths.Count;
			int num2 = 0;
			foreach (PolyLine polyLine in shapeReader.PolyLines)
			{
				if (GetSpatialElementCount() + 1 > MaxSpatialElementCount)
				{
					return SpatialLoadResult.MaxSpatialElementCountReached;
				}
				if (GetSpatialPointCount() + polyLine.Points.Length > MaxSpatialPointCount)
				{
					return SpatialLoadResult.MaxSpatialPointCountReached;
				}
				int num3 = num + 1;
				num = num3;
				string name = "Path" + num3.ToString(CultureInfo.CurrentCulture);
				if (!string.IsNullOrEmpty(nameColumn))
				{
					try
					{
						name = shapeReader.Table.Rows[num2][nameColumn].ToString();
					}
					catch
					{
						name = "(null)";
					}
				}
				Path path = (Path)Paths.GetByName(name);
				if (path == null)
				{
					Paths.SuppressAddedAndRemovedEvents = true;
					path = Paths.Add(name);
				}
				if (columnsToImport != null)
				{
					for (int j = 0; j < columnsToImport.Length; j++)
					{
						try
						{
							path[destinationFields[j]] = shapeReader.Table.Rows[num2][columnsToImport[j]];
						}
						catch
						{
						}
					}
				}
				MapPoint[] array = new MapPoint[polyLine.NumPoints];
				for (int k = 0; k < polyLine.NumPoints; k++)
				{
					if (polyLine.Points[k].Y < shapeReader.YMin)
					{
						polyLine.Points[k].Y = shapeReader.YMin;
					}
					if (polyLine.Points[k].Y > shapeReader.YMax)
					{
						polyLine.Points[k].Y = shapeReader.YMax;
					}
					array[k].X = polyLine.Points[k].X;
					array[k].Y = polyLine.Points[k].Y;
				}
				PathSegment[] array2 = new PathSegment[polyLine.NumParts];
				for (int l = 0; l < polyLine.NumParts; l++)
				{
					array2[l].Type = SegmentType.Polygon;
					if (l + 1 < polyLine.Parts.Length)
					{
						array2[l].Length = polyLine.Parts[l + 1] - polyLine.Parts[l];
					}
					else
					{
						array2[l].Length = polyLine.NumPoints - polyLine.Parts[l];
					}
				}
				path.AddSegments(array, array2);
				path.Layer = layer;
				path.Category = category;
				if (Paths.SuppressAddedAndRemovedEvents)
				{
					Common.InvokeElementAdded(path);
					Paths.SuppressAddedAndRemovedEvents = false;
				}
				num2++;
			}
			return SpatialLoadResult.AllSpatialElementsLoaded;
		}

		private SpatialLoadResult CreateSymbols(ShapeFileReader shapeReader, string nameColumn, string[] columnsToImport, string[] destinationFields, string layer, string category)
		{
			if (columnsToImport != null)
			{
				for (int i = 0; i < columnsToImport.Length; i++)
				{
					if (SymbolFields.GetByName(destinationFields[i]) == null)
					{
						SymbolFields.Add(destinationFields[i]).Type = shapeReader.Table.Columns[columnsToImport[i]].DataType;
					}
				}
			}
			int num = Symbols.Count;
			int num2 = 0;
			foreach (ShapePoint point in shapeReader.Points)
			{
				if (GetSpatialElementCount() + 1 > MaxSpatialElementCount)
				{
					return SpatialLoadResult.MaxSpatialElementCountReached;
				}
				if (GetSpatialPointCount() + 1 > MaxSpatialPointCount)
				{
					return SpatialLoadResult.MaxSpatialPointCountReached;
				}
				int num3 = num + 1;
				num = num3;
				string text = "Symbol" + num3.ToString(CultureInfo.CurrentCulture);
				if (!string.IsNullOrEmpty(nameColumn))
				{
					try
					{
						text = shapeReader.Table.Rows[num2][nameColumn].ToString();
					}
					catch
					{
						text = "(null)";
					}
				}
				Symbol symbol = null;
				Symbols.SuppressAddedAndRemovedEvents = true;
				int num4 = 0;
				while (symbol == null)
				{
					try
					{
						symbol = ((num4 != 0) ? Symbols.Add(text + num4.ToString(CultureInfo.CurrentCulture)) : Symbols.Add(text));
					}
					catch
					{
						num4++;
					}
				}
				if (columnsToImport != null)
				{
					for (int j = 0; j < columnsToImport.Length; j++)
					{
						try
						{
							symbol[destinationFields[j]] = shapeReader.Table.Rows[num2][columnsToImport[j]];
						}
						catch
						{
						}
					}
				}
				symbol.X = point.X;
				symbol.Y = point.Y;
				symbol.Layer = layer;
				symbol.Category = category;
				if (Symbols.SuppressAddedAndRemovedEvents)
				{
					Common.InvokeElementAdded(symbol);
					Symbols.SuppressAddedAndRemovedEvents = false;
				}
				num2++;
			}
			num2 = 0;
			foreach (MultiPoint multiPoint in shapeReader.MultiPoints)
			{
				if (GetSpatialElementCount() + 1 > MaxSpatialElementCount)
				{
					return SpatialLoadResult.MaxSpatialElementCountReached;
				}
				if (GetSpatialPointCount() + multiPoint.NumPoints > MaxSpatialPointCount)
				{
					return SpatialLoadResult.MaxSpatialPointCountReached;
				}
				string text2 = "Symbol" + num++.ToString(CultureInfo.CurrentCulture);
				if (!string.IsNullOrEmpty(nameColumn))
				{
					try
					{
						text2 = shapeReader.Table.Rows[num2][nameColumn].ToString();
					}
					catch
					{
						text2 = "(null)";
					}
				}
				Symbol symbol2 = null;
				Symbols.SuppressAddedAndRemovedEvents = true;
				int num5 = 0;
				while (symbol2 == null)
				{
					try
					{
						symbol2 = ((num5 != 0) ? Symbols.Add(text2 + num5.ToString(CultureInfo.CurrentCulture)) : Symbols.Add(text2));
					}
					catch
					{
						num5++;
					}
				}
				if (columnsToImport != null)
				{
					for (int k = 0; k < columnsToImport.Length; k++)
					{
						try
						{
							symbol2[destinationFields[k]] = shapeReader.Table.Rows[num2][columnsToImport[k]];
						}
						catch
						{
						}
					}
				}
				symbol2.X = multiPoint.Points[0].X;
				symbol2.Y = multiPoint.Points[0].Y;
				symbol2.Layer = layer;
				symbol2.Category = category;
				if (Symbols.SuppressAddedAndRemovedEvents)
				{
					Common.InvokeElementAdded(symbol2);
					Symbols.SuppressAddedAndRemovedEvents = false;
				}
				num2++;
			}
			return SpatialLoadResult.AllSpatialElementsLoaded;
		}

		public void Simplify(float factor)
		{
			double resolution = (MaximumPoint.X - MinimumPoint.X) / 360.0 * (double)factor / 100.0 * 2.0;
			new MapSimplifier().Simplify(this, resolution);
			InvalidateCachedPaths();
			InvalidateViewport();
		}

		internal void InvalidateCachedBounds()
		{
			if (!updatingCachedBounds)
			{
				cachedBoundsDirty = true;
			}
			InvalidateCachedPaths();
		}

		internal void UpdateCachedBounds()
		{
			if (!cachedBoundsDirty)
			{
				return;
			}
			updatingCachedBounds = true;
			cachedBoundsDirty = false;
			List<Shape> boundDeterminingShapes = GetBoundDeterminingShapes();
			List<Path> boundDeterminingPaths = GetBoundDeterminingPaths();
			List<Symbol> boundDeterminingSymbols = GetBoundDeterminingSymbols();
			double num = double.PositiveInfinity;
			double num2 = double.PositiveInfinity;
			double num3 = double.NegativeInfinity;
			double num4 = double.NegativeInfinity;
			foreach (Shape item in boundDeterminingShapes)
			{
				item.ShapeData.UpdateStoredParameters();
				num = Math.Min(num, item.ShapeData.MinimumExtent.X + item.OffsetInt.X);
				num2 = Math.Min(num2, item.ShapeData.MinimumExtent.Y + item.OffsetInt.Y);
				num3 = Math.Max(num3, item.ShapeData.MaximumExtent.X + item.OffsetInt.X);
				num4 = Math.Max(num4, item.ShapeData.MaximumExtent.Y + item.OffsetInt.Y);
			}
			foreach (Path item2 in boundDeterminingPaths)
			{
				item2.PathData.UpdateStoredParameters();
				num = Math.Min(num, item2.PathData.MinimumExtent.X + item2.OffsetInt.X);
				num2 = Math.Min(num2, item2.PathData.MinimumExtent.Y + item2.OffsetInt.Y);
				num3 = Math.Max(num3, item2.PathData.MaximumExtent.X + item2.OffsetInt.X);
				num4 = Math.Max(num4, item2.PathData.MaximumExtent.Y + item2.OffsetInt.Y);
			}
			foreach (Symbol item3 in boundDeterminingSymbols)
			{
				item3.SymbolData.UpdateStoredParameters();
				num = Math.Min(num, item3.SymbolData.MinimumExtent.X + item3.Offset.X);
				num2 = Math.Min(num2, item3.SymbolData.MinimumExtent.Y + item3.Offset.Y);
				num3 = Math.Max(num3, item3.SymbolData.MaximumExtent.X + item3.Offset.X);
				num4 = Math.Max(num4, item3.SymbolData.MaximumExtent.Y + item3.Offset.Y);
			}
			if (num == double.PositiveInfinity || num2 == double.PositiveInfinity || num3 == double.NegativeInfinity || num4 == double.NegativeInfinity)
			{
				MapBounds defaultEmptyBounds = GetDefaultEmptyBounds();
				MinimumPoint = defaultEmptyBounds.MinimumPoint;
				MaximumPoint = defaultEmptyBounds.MaximumPoint;
				MapCenterPoint = new MapPoint(0.0, 0.0);
			}
			else
			{
				double num5 = num3 - num;
				if (num5 < 1E-14)
				{
					num5 = ((!(Math.Abs(num3) < 1E-14)) ? (Math.Abs(num3) / 10.0) : 2.0);
					num -= num5 / 2.0;
					num3 += num5 / 2.0;
				}
				double num6 = num4 - num2;
				if (num6 < 1E-14)
				{
					num6 = ((!(Math.Abs(num4) < 1E-14)) ? (Math.Abs(num4) / 10.0) : 2.0);
					num2 -= num6 / 2.0;
					num4 += num6 / 2.0;
				}
				double latitudeLimit = GetLatitudeLimit();
				if (GeographyMode)
				{
					if (num4 < 0.0 - latitudeLimit)
					{
						num4 = 0.0 - latitudeLimit + num5;
						num6 = num4 - num2;
					}
					else if (num2 > latitudeLimit)
					{
						num2 = latitudeLimit - num5;
						num6 = num4 - num2;
					}
				}
				MapBounds boundsAfterProjection = GetBoundsAfterProjection(num, num2, num3, num4);
				double num7 = boundsAfterProjection.MaximumPoint.X - boundsAfterProjection.MinimumPoint.X;
				double num8 = boundsAfterProjection.MaximumPoint.Y - boundsAfterProjection.MinimumPoint.Y;
				if (num5 > num6 * 3.0 && num7 > num8 * 3.0)
				{
					double num9 = num6;
					num6 = num5 / 3.0;
					double num10 = (num6 - num9) / 2.0;
					num2 -= num10;
					num4 += num10;
				}
				else if (num6 > num5 * 3.0 && num8 > num7 * 3.0)
				{
					double num11 = num5;
					num5 = num6 / 3.0;
					double num12 = (num5 - num11) / 2.0;
					num -= num12;
					num3 += num12;
				}
				double num13 = num5 * 0.1;
				double num14 = num6 * 0.1;
				num -= num13;
				num2 -= num14;
				num3 += num13;
				num4 += num14;
				if (GeographyMode)
				{
					if (num2 < -89.0 && num2 > -90.1 && num4 < 90.0 && num4 > 81.0)
					{
						num4 = 0.0 - num2;
					}
					num = Math.Max(num, -180.0);
					num2 = Math.Max(num2, 0.0 - latitudeLimit);
					num3 = Math.Min(num3, 180.0);
					num4 = Math.Min(num4, latitudeLimit);
				}
				MinimumPoint = new MapPoint(num, num2);
				MaximumPoint = new MapPoint(num3, num4);
				MapCenterPoint = new MapPoint((MaximumPoint.X + MinimumPoint.X) / 2.0, (MaximumPoint.Y + MinimumPoint.Y) / 2.0);
			}
			updatingCachedBounds = false;
		}

		internal MapBounds DetermineSpatialElementsBounds()
		{
			new MapBounds(new MapPoint(double.PositiveInfinity, double.PositiveInfinity), new MapPoint(double.NegativeInfinity, double.NegativeInfinity));
			List<Shape> boundDeterminingShapes = GetBoundDeterminingShapes();
			List<Path> boundDeterminingPaths = GetBoundDeterminingPaths();
			List<Symbol> boundDeterminingSymbols = GetBoundDeterminingSymbols();
			double num = double.PositiveInfinity;
			double num2 = double.PositiveInfinity;
			double num3 = double.NegativeInfinity;
			double num4 = double.NegativeInfinity;
			foreach (Shape item in boundDeterminingShapes)
			{
				item.ShapeData.UpdateStoredParameters();
				num = Math.Min(num, item.ShapeData.MinimumExtent.X + item.OffsetInt.X);
				num2 = Math.Min(num2, item.ShapeData.MinimumExtent.Y + item.OffsetInt.Y);
				num3 = Math.Max(num3, item.ShapeData.MaximumExtent.X + item.OffsetInt.X);
				num4 = Math.Max(num4, item.ShapeData.MaximumExtent.Y + item.OffsetInt.Y);
			}
			foreach (Path item2 in boundDeterminingPaths)
			{
				item2.PathData.UpdateStoredParameters();
				num = Math.Min(num, item2.PathData.MinimumExtent.X + item2.OffsetInt.X);
				num2 = Math.Min(num2, item2.PathData.MinimumExtent.Y + item2.OffsetInt.Y);
				num3 = Math.Max(num3, item2.PathData.MaximumExtent.X + item2.OffsetInt.X);
				num4 = Math.Max(num4, item2.PathData.MaximumExtent.Y + item2.OffsetInt.Y);
			}
			foreach (Symbol item3 in boundDeterminingSymbols)
			{
				item3.SymbolData.UpdateStoredParameters();
				num = Math.Min(num, item3.SymbolData.MinimumExtent.X + item3.Offset.X);
				num2 = Math.Min(num2, item3.SymbolData.MinimumExtent.Y + item3.Offset.Y);
				num3 = Math.Max(num3, item3.SymbolData.MaximumExtent.X + item3.Offset.X);
				num4 = Math.Max(num4, item3.SymbolData.MaximumExtent.Y + item3.Offset.Y);
			}
			if (num == double.PositiveInfinity || num2 == double.PositiveInfinity || num3 == double.NegativeInfinity || num4 == double.NegativeInfinity)
			{
				return null;
			}
			return new MapBounds(new MapPoint(num, num2), new MapPoint(num3, num4));
		}

		private List<Shape> GetBoundDeterminingShapes()
		{
			List<Shape> list = new List<Shape>();
			foreach (Shape shape in Shapes)
			{
				ILayerElement layerElement = shape;
				if ((layerElement.BelongsToAllLayers || !layerElement.BelongsToLayer || (layerElement.BelongsToLayer && (Layers[layerElement.Layer].Visibility == LayerVisibility.Shown || Layers[layerElement.Layer].Visibility == LayerVisibility.ZoomBased))) && shape.ShapeData.Points != null && shape.ShapeData.Points.Length != 0)
				{
					list.Add(shape);
				}
			}
			return list;
		}

		private List<Path> GetBoundDeterminingPaths()
		{
			List<Path> list = new List<Path>();
			foreach (Path path in Paths)
			{
				ILayerElement layerElement = path;
				if ((layerElement.BelongsToAllLayers || !layerElement.BelongsToLayer || (layerElement.BelongsToLayer && (Layers[layerElement.Layer].Visibility == LayerVisibility.Shown || Layers[layerElement.Layer].Visibility == LayerVisibility.ZoomBased))) && path.PathData.Points != null && path.PathData.Points.Length != 0)
				{
					list.Add(path);
				}
			}
			return list;
		}

		private List<Symbol> GetBoundDeterminingSymbols()
		{
			List<Symbol> list = new List<Symbol>();
			if (GeographyMode && AutoLimitsIgnoreSymbols)
			{
				return list;
			}
			foreach (Symbol symbol in Symbols)
			{
				ILayerElement layerElement = symbol;
				if ((layerElement.BelongsToAllLayers || !layerElement.BelongsToLayer || (layerElement.BelongsToLayer && (Layers[layerElement.Layer].Visibility == LayerVisibility.Shown || Layers[layerElement.Layer].Visibility == LayerVisibility.ZoomBased))) && symbol.ParentShape == "(none)" && symbol.SymbolData.Points != null && symbol.SymbolData.Points.Length != 0)
				{
					list.Add(symbol);
				}
			}
			return list;
		}

		private double GetLatitudeLimit()
		{
			if (Projection == Projection.Equirectangular)
			{
				return 90.0;
			}
			if (Projection == Projection.Mercator)
			{
				return 85.05112878;
			}
			return 89.0;
		}

		private MapBounds GetDefaultEmptyBounds()
		{
			MapPoint mapPoint;
			MapPoint mapPoint2;
			if (!GeographyMode)
			{
				mapPoint = new MapPoint(-10.0, -10.0);
				mapPoint2 = new MapPoint(10.0, 10.0);
			}
			else
			{
				double latitudeLimit = GetLatitudeLimit();
				mapPoint = new MapPoint(-180.0, 0.0 - latitudeLimit);
				mapPoint2 = new MapPoint(180.0, latitudeLimit);
			}
			return new MapBounds(mapPoint, mapPoint2);
		}

		private string[,] DetermineTileUrls(Layer layer, int levelOfDetail, int xStartIndex, int yStartIndex, int horizontalCount, int verticalCount)
		{
			string[,] array = new string[horizontalCount, verticalCount];
			string text = layer.TileImageUriFormat.Replace("{subdomain}", "{0}");
			text = text.Replace("{quadkey}", "{1}");
			text = text.Replace("{culture}", "{2}");
			text = text.Replace("{token}", "{3}");
			for (int i = 0; i < verticalCount; i++)
			{
				for (int j = 0; j < horizontalCount; j++)
				{
					string text2 = VirtualEarthTileSystem.TileXYToQuadKey(j + xStartIndex, i + yStartIndex, levelOfDetail);
					int num = int.Parse(text2[text2.Length - 1].ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
					string text3 = layer.TileImageUriSubdomains[num % layer.TileImageUriSubdomains.Length];
					array[j, i] = string.Format(CultureInfo.InvariantCulture, text, text3, text2, TileCulture.IetfLanguageTag, TileServerAppId);
				}
			}
			return array;
		}

		private Rectangle[,] DetermineTileRectangles(int horizontalCount, int verticalCount, int xOffset, int yOffset, int stretchedTileLength)
		{
			Rectangle[,] array = new Rectangle[horizontalCount, verticalCount];
			for (int i = 0; i < verticalCount; i++)
			{
				for (int j = 0; j < horizontalCount; j++)
				{
					array[j, i] = new Rectangle(xOffset + j * stretchedTileLength, yOffset + i * stretchedTileLength, stretchedTileLength, stretchedTileLength);
				}
			}
			return array;
		}

		private Image[,] LoadTileImages(string[,] tileImageUrls, Rectangle[,] tileRectangles, Layer layer)
		{
			int num = tileImageUrls.GetUpperBound(0) + 1;
			int num2 = tileImageUrls.GetUpperBound(1) + 1;
			Image[,] array = new Image[num, num2];
			openTileRequestCount = 0L;
			requestsCompletedEvent.Reset();
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					if (TileImagesCache.Contains(tileImageUrls[j, i]))
					{
						array[j, i] = (Image)TileImagesCache[tileImageUrls[j, i]];
						continue;
					}
					if (Viewport.LoadTilesAsynchronously)
					{
						array[j, i] = TileWaitImage;
						lock (TileImagesCache)
						{
							TileImagesCache[tileImageUrls[j, i]] = TileWaitImage;
						}
					}
					else
					{
						Interlocked.Increment(ref openTileRequestCount);
					}
					try
					{
						HttpWebRequest obj = (HttpWebRequest)WebRequest.Create(tileImageUrls[j, i]);
						obj.KeepAlive = true;
						obj.CachePolicy = new RequestCachePolicy(TileCacheLevel);
						TileRequestState state = new TileRequestState(obj, tileImageUrls[j, i], array, j, i, tileRectangles[j, i], layer, this);
						ThreadPool.RegisterWaitForSingleObject(obj.BeginGetResponse(WebResponseCallback, state).AsyncWaitHandle, TimeoutCallback, state, TileServerTimeout, executeOnlyOnce: true);
					}
					catch
					{
						if (!Viewport.LoadTilesAsynchronously)
						{
							Interlocked.Decrement(ref openTileRequestCount);
						}
					}
				}
			}
			if (!Viewport.LoadTilesAsynchronously)
			{
				while (Interlocked.Read(ref openTileRequestCount) > 0)
				{
					requestsCompletedEvent.WaitOne(10, exitContext: false);
				}
			}
			return array;
		}

		private void TimeoutCallback(object state, bool timedOut)
		{
			if (timedOut)
			{
				TileRequestState obj = (TileRequestState)state;
				obj.Timeout = true;
				obj.Request.Abort();
			}
		}

		private void WebResponseCallback(IAsyncResult asyncResult)
		{
			TileRequestState tileRequestState = (TileRequestState)asyncResult.AsyncState;
			Image image = null;
			try
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)tileRequestState.Request.EndGetResponse(asyncResult);
				using (Stream stream = httpWebResponse.GetResponseStream())
				{
					MemoryStream memoryStream = new MemoryStream();
					byte[] buffer = new byte[4096];
					int count;
					while ((count = stream.Read(buffer, 0, 4096)) > 0)
					{
						memoryStream.Write(buffer, 0, count);
					}
					image = Image.FromStream(memoryStream);
					Utils.SetImageCustomProperty(image, CustomPropertyTag.ImageProviders, tileRequestState.Layer.GetAttributionStrings());
				}
				httpWebResponse.Close();
			}
			catch (Exception ex)
			{
				image = new Bitmap(256, 256);
				if (tileRequestState.Timeout)
				{
					Utils.SetImageCustomProperty(image, CustomPropertyTag.ImageError, SR.TileServerRequestTimeout(tileRequestState.MapCore.TileServerTimeout));
				}
				else
				{
					Utils.SetImageCustomProperty(image, CustomPropertyTag.ImageError, ex.Message);
				}
			}
			lock (tileRequestState.TileImages)
			{
				tileRequestState.TileImages[tileRequestState.X, tileRequestState.Y] = image;
			}
			if (tileRequestState.MapCore.Viewport.LoadTilesAsynchronously)
			{
				lock (tileRequestState.MapCore.TileImagesCache)
				{
					TileImagesCache[tileRequestState.Url] = image;
				}
				if (UseGridSectionRendering())
				{
					InvalidateGridSections(tileRequestState.Rectangle);
				}
				PointF contentOffsetInPixels = tileRequestState.MapCore.Viewport.GetContentOffsetInPixels();
				tileRequestState.Rectangle.Offset((int)(contentOffsetInPixels.X + (float)Viewport.Margins.Left), (int)(contentOffsetInPixels.Y + (float)Viewport.Margins.Top));
				if (tileRequestState.Rectangle.Width != 256)
				{
					tileRequestState.Rectangle.Inflate(3, 3);
				}
			}
			if (!tileRequestState.MapCore.Viewport.LoadTilesAsynchronously)
			{
				if (Interlocked.Read(ref openTileRequestCount) > 0)
				{
					Interlocked.Decrement(ref openTileRequestCount);
				}
				if (Interlocked.Read(ref openTileRequestCount) == 0L)
				{
					requestsCompletedEvent.Set();
				}
			}
		}

		public void SetUserLocales(string[] userLocales)
		{
			this.userLocales = userLocales;
		}

		private bool IsLocaleAllowedToAccessBing()
		{
			if (userLocales != null)
			{
				string[] array = userLocales;
				foreach (string key in array)
				{
					if (_geoPoliticalBlockHashtableLocales.ContainsKey(key))
					{
						return false;
					}
				}
				return true;
			}
			return !_geoPoliticalBlockHashtableLocales.ContainsKey(CultureInfo.CurrentCulture.Name);
		}

		private void RenderTiles(MapGraphics g, Layer layer, RectangleF clipRect)
		{
			if (!IsLocaleAllowedToAccessBing())
			{
				RenderTileImageError(g.Graphics, SR.Map_BackgroundNotAvailable, Rectangle.Round(clipRect));
				return;
			}
			if (!layer.IsVirtualEarthServiceQueried())
			{
				if (Viewport.LoadTilesAsynchronously || (Viewport.QueryVirtualEarthAsynchronously && string.IsNullOrEmpty(layer.TileError)))
				{
					layer.QueryVirtualEarthService(asyncQuery: true);
					return;
				}
				if (!layer.QueryVirtualEarthService(asyncQuery: false))
				{
					return;
				}
			}
			PointF location = GeographicToContent(new MapPoint(-180.0, 90.0));
			PointF pointF = GeographicToContent(new MapPoint(180.0, -90.0));
			RectangleF rect = new RectangleF(location, new SizeF(pointF.X - location.X, pointF.Y - location.Y));
			clipRect.Intersect(rect);
			if (clipRect.IsEmpty)
			{
				return;
			}
			GraphicsState gstate = g.Save();
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				graphicsPath.AddRectangle(clipRect);
				g.SetClip(graphicsPath, CombineMode.Replace);
			}
			double num = VirtualEarthTileSystem.LevelOfDetail(Viewport.GetGroundResolutionAtEquator());
			int num2 = Math.Max((int)num, 1);
			double num3 = Math.Pow(2.0, num - (double)num2);
			double num4 = 256.0 * num3;
			int num5 = (int)Math.Round(num4);
			if (num5 == 512)
			{
				num4 = 256.0;
				num5 = 256;
				num3 = 1.0;
				num += 1.0;
				num2++;
			}
			VirtualEarthTileSystem.LongLatToPixelXY(MinimumPoint.X, MaximumPoint.Y, num, out double pixelX, out double pixelY);
			pixelX = Math.Round(pixelX);
			pixelY = Math.Round(pixelY);
			double num6 = pixelX + (double)clipRect.X;
			double num7 = pixelY + (double)clipRect.Y;
			int num8 = (int)Math.Max(num6 / num4, 0.0);
			int num9 = (int)Math.Max(num7 / num4, 0.0);
			double num10 = (double)num8 * num4;
			double num11 = (double)num9 * num4;
			float num12 = (float)(num10 - num6) + clipRect.X;
			float num13 = (float)(num11 - num7) + clipRect.Y;
			int num14 = (int)Math.Ceiling((double)(clipRect.Width + clipRect.X - num12) / num4);
			int num15 = (int)Math.Ceiling((double)(clipRect.Height + clipRect.Y - num13) / num4);
			float num16 = 1f;
			if (layer.Transparency > 0f)
			{
				num16 = (100f - layer.Transparency) / 100f;
			}
			ImageAttributes imageAttributes = null;
			if (num16 < 1f)
			{
				ColorMatrix colorMatrix = new ColorMatrix();
				colorMatrix.Matrix33 = num16;
				imageAttributes = new ImageAttributes();
				imageAttributes.SetColorMatrix(colorMatrix);
			}
			Bitmap bitmap = null;
			Graphics graphics = null;
			if (num5 > 256)
			{
				bitmap = new Bitmap(256 * num14, 256 * num15);
				graphics = Graphics.FromImage(bitmap);
			}
			string[,] array = DetermineTileUrls(layer, num2, num8, num9, num14, num15);
			Rectangle[,] tileRectangles = DetermineTileRectangles(num14, num15, (int)num12, (int)num13, num5);
			Image[,] array2 = null;
			if (LoadTilesHandler != null)
			{
				array2 = LoadTilesHandler(layer, array);
			}
			if (array2 == null)
			{
				array2 = LoadTileImages(array, tileRectangles, layer);
			}
			if (SaveTilesHandler != null)
			{
				SaveTilesHandler(layer, array, array2);
			}
			if (!Viewport.LoadTilesAsynchronously)
			{
				for (int i = 0; i < num15; i++)
				{
					for (int j = 0; j < num14; j++)
					{
						TileImagesCache[array[j, i]] = array2[j, i];
					}
				}
			}
			for (int k = 0; k < num15; k++)
			{
				for (int l = 0; l < num14; l++)
				{
					Image image = array2[l, k];
					if (image == null)
					{
						continue;
					}
					if (graphics != null)
					{
						Rectangle rect2 = new Rectangle(l * 256, k * 256, 256, 256);
						graphics.DrawImageUnscaledAndClipped(image, rect2);
						continue;
					}
					Rectangle rectangle = new Rectangle(l * num5, k * num5, num5, num5);
					rectangle.Offset((int)num12, (int)num13);
					if (imageAttributes != null)
					{
						g.Graphics.DrawImage(image, rectangle, 0, 0, 256, 256, GraphicsUnit.Pixel, imageAttributes);
					}
					else if (num5 < 256)
					{
						g.Graphics.DrawImage(image, rectangle);
					}
					else
					{
						g.Graphics.DrawImageUnscaledAndClipped(image, rectangle);
					}
				}
			}
			if (bitmap != null)
			{
				Rectangle rectangle2 = new Rectangle((int)num12, (int)num13, num14 * num5, num15 * num5);
				if (imageAttributes != null)
				{
					g.Graphics.DrawImage(bitmap, rectangle2, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imageAttributes);
				}
				else
				{
					g.Graphics.DrawImageUnscaledAndClipped(bitmap, rectangle2);
				}
			}
			for (int m = 0; m < num15; m++)
			{
				for (int n = 0; n < num14; n++)
				{
					if (array2[n, m] != null)
					{
						string imageCustomProperty = Utils.GetImageCustomProperty(array2[n, m], CustomPropertyTag.ImageError);
						if (!string.IsNullOrEmpty(imageCustomProperty))
						{
							Rectangle rect3 = new Rectangle(n * num5, m * num5, num5, num5);
							rect3.Offset((int)num12, (int)num13);
							RenderTileImageError(g.Graphics, imageCustomProperty, rect3);
						}
					}
				}
			}
			g.Restore(gstate);
		}

		internal void Paint(Graphics g)
		{
			Paint(g, RenderingType.Gdi, null, buffered: false);
		}

		internal void Paint(Graphics gdiGraph, RenderingType renderingType, Stream stream, bool buffered)
		{
			if (skipPaint)
			{
				return;
			}
			disableInvalidate = true;
			MapGraphics mapGraphics = null;
			try
			{
				if (AutoUpdates)
				{
					AutoDataBind(forceBinding: false);
					UpdateCachedBounds();
					ApplyAllRules();
					ResetCachedPaths();
					ResetChildSymbols();
				}
				mapGraphics = GetGraphics(renderingType, gdiGraph, stream);
				LayoutPanels(mapGraphics);
				if (RenderingMode == RenderingMode.GridSections)
				{
					RenderOneGridSection(mapGraphics, SingleGridSectionX, SingleGridSectionY);
				}
				else if (RenderingMode == RenderingMode.SinglePanel || RenderingMode == RenderingMode.ZoomThumb)
				{
					RenderOnePanel(mapGraphics);
				}
				else if (RenderingMode == RenderingMode.Background)
				{
					RenderFrame(mapGraphics);
				}
				else if (buffered)
				{
					RenderElementsBufered(mapGraphics);
				}
				else
				{
					RenderElements(mapGraphics);
				}
			}
			finally
			{
				disableInvalidate = false;
				mapGraphics?.Close();
			}
			dirtyFlag = false;
		}

		internal void PrintPaint(Graphics g, Rectangle position)
		{
			Notify(MessageType.PrepareSnapShot, this, null);
			GraphicsState gstate = g.Save();
			try
			{
				isPrinting = true;
				Width = position.Width;
				Height = position.Height;
				g.TranslateTransform(position.X, position.Y);
				Paint(g, RenderingType.Gdi, null, buffered: false);
			}
			finally
			{
				g.Restore(gstate);
				isPrinting = false;
			}
		}

		internal BufferBitmap InitBitmap(BufferBitmap bmp)
		{
			if (bmp == null)
			{
				bmp = new BufferBitmap();
			}
			else if (dirtyFlag)
			{
				bmp.Invalidate();
			}
			if (RenderingMode == RenderingMode.GridSections)
			{
				bmp.Size = new Size(GridSectionSize.Width, GridSectionSize.Height);
			}
			else if (RenderingMode == RenderingMode.SinglePanel || RenderingMode == RenderingMode.ZoomThumb)
			{
				SizeF absoluteSize = PanelToRender.GetAbsoluteSize();
				bmp.Size = new Size((int)Math.Round(absoluteSize.Width), (int)Math.Round(absoluteSize.Height));
			}
			else
			{
				bmp.Size = new Size(GetWidth(), GetHeight());
			}
			bmp.Graphics.SmoothingMode = GetSmootingMode();
			bmp.Graphics.TextRenderingHint = GetTextRenderingHint();
			bmp.Graphics.TextContrast = 2;
			return bmp;
		}

		private SmoothingMode GetSmootingMode()
		{
			if ((MapControl.AntiAliasing & AntiAliasing.Graphics) == AntiAliasing.Graphics)
			{
				return SmoothingMode.HighQuality;
			}
			return SmoothingMode.HighSpeed;
		}

		private TextRenderingHint GetTextRenderingHint()
		{
			TextRenderingHint textRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
			if ((MapControl.AntiAliasing & AntiAliasing.Text) == AntiAliasing.Text)
			{
				textRenderingHint = TextRenderingHint.ClearTypeGridFit;
				if (TextAntiAliasingQuality == TextAntiAliasingQuality.Normal)
				{
					textRenderingHint = TextRenderingHint.AntiAlias;
				}
				else if (TextAntiAliasingQuality == TextAntiAliasingQuality.SystemDefault)
				{
					textRenderingHint = TextRenderingHint.SystemDefault;
				}
			}
			else
			{
				textRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
			}
			return textRenderingHint;
		}

		internal MapGraphics GetGraphics(RenderingType renderingType, Graphics g, Stream outputStream)
		{
			MapGraphics mapGraphics = new MapGraphics(Common);
			Common.Height = GetHeight();
			Common.Width = GetWidth();
			mapGraphics.SetPictureSize(Common.Width, Common.Height);
			mapGraphics.ActiveRenderingType = renderingType;
			mapGraphics.Graphics = g;
			mapGraphics.AntiAliasing = Common.MapCore.AntiAliasing;
			mapGraphics.SmoothingMode = GetSmootingMode();
			mapGraphics.TextRenderingHint = GetTextRenderingHint();
			return mapGraphics;
		}

		internal Panel[] GetSortedPanels()
		{
			if (sortedPanels == null || dirtyFlag)
			{
				ArrayList arrayList = new ArrayList();
				arrayList.Add(Viewport);
				arrayList.AddRange(Labels);
				arrayList.AddRange(Legends);
				arrayList.AddRange(Images);
				arrayList.Add(ColorSwatchPanel);
				arrayList.Add(DistanceScalePanel);
				arrayList.Add(NavigationPanel);
				arrayList.Add(ZoomPanel);
				arrayList.Sort(new ZOrderSort(arrayList));
				sortedPanels = (Panel[])arrayList.ToArray(typeof(Panel));
			}
			return sortedPanels;
		}

		private string GetTileLayerAttributions()
		{
			Hashtable hashtable = new Hashtable();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Layer layer in Layers)
			{
				if (!layer.Visible || layer.TileSystem == TileSystem.None)
				{
					continue;
				}
				string[] array = layer.GetAttributionStrings().Split('|');
				foreach (string text in array)
				{
					if (!hashtable.ContainsKey(text))
					{
						hashtable.Add(text, null);
						stringBuilder.Append(text);
						stringBuilder.Append(", ");
					}
				}
			}
			return stringBuilder.ToString().Trim(',', ' ');
		}

		private void RenderTileLayerAttributions(MapGraphics g)
		{
			string tileLayerAttributions = GetTileLayerAttributions();
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Far;
			RectangleF layoutRectangle = new RectangleF(Viewport.GetAbsoluteLocation(), Viewport.GetAbsoluteSize());
			using (Font font = new Font("Tahoma", 7f))
			{
				layoutRectangle.Offset(1f, 1f);
				g.DrawStringAbs(tileLayerAttributions, font, Brushes.Black, layoutRectangle, stringFormat);
				layoutRectangle.Offset(-1f, -1f);
				using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.White)))
				{
					g.DrawStringAbs(tileLayerAttributions, font, brush, layoutRectangle, stringFormat);
				}
			}
		}

		private string GetTileLayerError()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Layer layer in Layers)
			{
				if (layer.Visible && layer.TileSystem != 0)
				{
					lock (layer.TileError)
					{
						stringBuilder.Append(layer.TileError + "\n");
					}
				}
			}
			return stringBuilder.ToString().Trim();
		}

		internal static void RenderTileImageError(Graphics graphics, string error, Rectangle rect)
		{
			rect.Width++;
			rect.Height++;
			graphics.FillRectangle(Brushes.White, rect);
			rect.Inflate(-10, -10);
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			using (Font font = new Font("Tahoma", 10f))
			{
				graphics.MeasureString(error, font, rect.Width, stringFormat);
				graphics.DrawString(error, font, Brushes.Black, rect, stringFormat);
			}
		}

		internal void RenderErrorMessage(MapGraphics g)
		{
			string tileLayerError = GetTileLayerError();
			if (string.IsNullOrEmpty(Viewport.ErrorMessage) && string.IsNullOrEmpty(tileLayerError))
			{
				return;
			}
			string text = Viewport.ErrorMessage + "\n" + tileLayerError;
			text = text.Trim();
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Near;
			stringFormat.LineAlignment = StringAlignment.Near;
			RectangleF layoutRectangle = new RectangleF(Viewport.GetAbsoluteLocation(), Viewport.GetAbsoluteSize());
			using (Font font = new Font("Tahoma", 10f))
			{
				layoutRectangle.Offset(1f, 1f);
				g.DrawStringAbs(text, font, Brushes.Black, layoutRectangle, stringFormat);
				layoutRectangle.Offset(-1f, -1f);
				using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.White)))
				{
					g.DrawStringAbs(text, font, brush, layoutRectangle, stringFormat);
				}
			}
		}

		private bool IsGridSectionsArraySquare()
		{
			int num = 9999999;
			int num2 = -9999999;
			int num3 = 9999999;
			int num4 = -9999999;
			Point[] array = GridSectionsArray;
			for (int i = 0; i < array.Length; i++)
			{
				Point point = array[i];
				num = Math.Min(num, point.X);
				num2 = Math.Max(num2, point.X);
				num3 = Math.Min(num3, point.Y);
				num4 = Math.Max(num4, point.Y);
			}
			int num5 = num2 - num + 1;
			int num6 = num4 - num3 + 1;
			if (GridSectionsArray.Length == GridSectionsInViewportXCount * GridSectionsInViewportYCount && num5 == GridSectionsInViewportXCount && num6 == GridSectionsInViewportYCount)
			{
				return true;
			}
			return false;
		}

		internal void RenderElements(MapGraphics g)
		{
			HotRegionList.Clear();
			HotRegionList.ScaleFactorX = g.ScaleFactorX;
			HotRegionList.ScaleFactorY = g.ScaleFactorY;
			RenderFrame(g);
			using (GraphicsPath graphicsPath = Viewport.GetHotRegionPath(g))
			{
				HotRegionList.SetHotRegion(g, Viewport, graphicsPath);
			}
			Viewport.Render(g);
			GraphicsState gstate = g.Save();
			try
			{
				RectangleF rect = new RectangleF(Viewport.GetAbsoluteLocation(), Viewport.GetAbsoluteSize());
				rect.X = (float)Math.Round(rect.X);
				rect.Y = (float)Math.Round(rect.Y);
				rect.Width = (float)Math.Round(rect.Width);
				rect.Height = (float)Math.Round(rect.Height);
				if (rect.Width > 0f && rect.Height > 0f)
				{
					g.Graphics.IntersectClip(rect);
					Common.InvokePrePaint(Viewport);
					if (UseGridSectionRendering())
					{
						RenderGridSections(g);
					}
					else
					{
						RenderContentElements(g, new PointF(0f, 0f), HotRegionList);
					}
					if (!GridUnderContent)
					{
						RenderGrid(g, Point.Empty);
					}
					RenderErrorMessage(g);
					RenderTileLayerAttributions(g);
					Common.InvokePostPaint(Viewport);
				}
			}
			finally
			{
				g.Restore(gstate);
			}
			Viewport.RenderBorder(g);
			RenderPanels(g);
		}

		internal void RenderLayer(MapGraphics g, Layer layer, bool allLayers, RectangleF clipRect, HotRegionList hotRegions, Hashtable visibleLabels)
		{
			if (layer != null && layer.TileSystem != 0 && Projection == Projection.Mercator)
			{
				RenderTiles(g, layer, clipRect);
			}
			ArrayList arrayList = new ArrayList();
			foreach (IContentElement group in Groups)
			{
				if (group.IsVisible(g, layer, allLayers, clipRect))
				{
					arrayList.Add(group);
					if (layer == null || layer.LabelVisible)
					{
						visibleLabels.Add(group, group);
					}
				}
				foreach (IContentElement shape in ((Group)group).Shapes)
				{
					if (shape.IsVisible(g, layer, allLayers, clipRect))
					{
						arrayList.Add(shape);
						if (layer == null || layer.LabelVisible)
						{
							visibleLabels.Add(shape, shape);
						}
					}
					((Shape)shape).ArrangeChildSymbols(g);
					foreach (IContentElement symbol in ((Shape)shape).Symbols)
					{
						if (symbol.IsVisible(g, layer, allLayers, clipRect))
						{
							arrayList.Add(symbol);
							if (layer == null || layer.LabelVisible)
							{
								visibleLabels.Add(symbol, symbol);
							}
						}
					}
				}
			}
			foreach (IContentElement shape2 in Shapes)
			{
				if (((Shape)shape2).ParentGroupObject != null)
				{
					continue;
				}
				if (shape2.IsVisible(g, layer, allLayers, clipRect))
				{
					arrayList.Add(shape2);
					if (layer == null || layer.LabelVisible)
					{
						visibleLabels.Add(shape2, shape2);
					}
				}
				((Shape)shape2).ArrangeChildSymbols(g);
				foreach (IContentElement symbol2 in ((Shape)shape2).Symbols)
				{
					if (symbol2.IsVisible(g, layer, allLayers, clipRect))
					{
						arrayList.Add(symbol2);
						if (layer == null || layer.LabelVisible)
						{
							visibleLabels.Add(symbol2, symbol2);
						}
					}
				}
			}
			foreach (IContentElement path in Paths)
			{
				if (path.IsVisible(g, layer, allLayers, clipRect))
				{
					arrayList.Add(path);
					if (layer == null || layer.LabelVisible)
					{
						visibleLabels.Add(path, path);
					}
				}
			}
			foreach (IContentElement symbol3 in Symbols)
			{
				if (((Symbol)symbol3).ParentShapeObject == null && symbol3.IsVisible(g, layer, allLayers, clipRect))
				{
					arrayList.Add(symbol3);
					if (layer == null || layer.LabelVisible)
					{
						visibleLabels.Add(symbol3, symbol3);
					}
				}
			}
			foreach (IContentElement item in arrayList)
			{
				item.RenderShadow(g);
			}
			foreach (IContentElement item2 in arrayList)
			{
				common.InvokePrePaint((NamedElement)item2);
				item2.RenderBack(g, hotRegions);
			}
			foreach (IContentElement item3 in arrayList)
			{
				item3.RenderFront(g, hotRegions);
			}
		}

		internal void RenderContentElements(MapGraphics g, PointF gridSectionOffset, HotRegionList hotRegions)
		{
			if (GridUnderContent)
			{
				RenderGrid(g, gridSectionOffset);
			}
			try
			{
				g.CreateContentDrawRegion(Viewport, gridSectionOffset);
				RectangleF bounds = g.Clip.GetBounds(g.Graphics);
				ResetGeographicClipRectangles();
				Hashtable hashtable = new Hashtable();
				RenderLayer(g, null, allLayers: false, bounds, hotRegions, hashtable);
				if (Layers.HasVisibleLayer())
				{
					foreach (Layer layer in Layers)
					{
						if (layer.Visible)
						{
							RenderLayer(g, layer, allLayers: false, bounds, hotRegions, hashtable);
						}
					}
					RenderLayer(g, null, allLayers: true, bounds, hotRegions, hashtable);
				}
				foreach (IContentElement key in hashtable.Keys)
				{
					key.RenderText(g, hotRegions);
					Common.InvokePostPaint((NamedElement)key);
				}
				RenderSelectedContentElements(g, bounds);
			}
			finally
			{
				g.RestoreDrawRegion();
			}
		}

		private void RenderOnePanel(MapGraphics g)
		{
			SizeF absoluteSize = PanelToRender.GetAbsoluteSize();
			if ((double)absoluteSize.Width < 1.0 || (double)absoluteSize.Height < 1.0)
			{
				return;
			}
			try
			{
				using (GraphicsPath graphicsPath = PanelToRender.GetHotRegionPath(g))
				{
					HotRegionList.SetHotRegion(g, PanelToRender, graphicsPath);
				}
				RectangleF rect = new RectangleF((float)(-PanelToRender.Margins.Left) / (float)GetWidth() * 100f, (float)(-PanelToRender.Margins.Top) / (float)GetHeight() * 100f, absoluteSize.Width / (float)GetWidth() * 100f, absoluteSize.Height / (float)GetHeight() * 100f);
				g.CreateDrawRegion(rect);
				Common.InvokePrePaint(PanelToRender);
				PanelToRender.RenderPanel(g);
				Common.InvokePostPaint(PanelToRender);
			}
			finally
			{
				g.RestoreDrawRegion();
			}
		}

		internal void RenderPanels(MapGraphics g)
		{
			RectangleF bounds = g.Clip.GetBounds(g.Graphics);
			Panel[] array = GetSortedPanels();
			foreach (Panel panel in array)
			{
				if (panel is Viewport)
				{
					continue;
				}
				RectangleF boundRect = panel.GetBoundRect(g);
				SizeF absoluteSize = g.GetAbsoluteSize(boundRect.Size);
				if ((double)absoluteSize.Width < 1.0 || (double)absoluteSize.Height < 1.0 || !panel.IsRenderVisible(g, bounds))
				{
					continue;
				}
				try
				{
					using (GraphicsPath graphicsPath = panel.GetHotRegionPath(g))
					{
						HotRegionList.SetHotRegion(g, panel, graphicsPath);
					}
					g.CreateDrawRegion(boundRect);
					Common.InvokePrePaint(panel);
					panel.RenderPanel(g);
					Common.InvokePostPaint(panel);
				}
				finally
				{
					g.RestoreDrawRegion();
				}
			}
			RenderSelectedPanels(g, bounds);
		}

		private void RenderFrame(MapGraphics g)
		{
			if (Frame.FrameStyle != 0)
			{
				g.FillRectangleAbs(new RectangleF(0f, 0f, Width, Height), Frame.PageColor, MapHatchStyle.None, "", MapImageWrapMode.Tile, Color.Empty, MapImageAlign.Center, GradientType.None, Color.Empty, Frame.PageColor, 1, MapDashStyle.Solid, PenAlignment.Inset);
				g.Draw3DBorderAbs(Frame, new RectangleF(0f, 0f, GetWidth(), GetHeight()), BackColor, BackHatchStyle, "", MapImageWrapMode.Unscaled, Color.Empty, MapImageAlign.Center, BackGradientType, BackSecondaryColor, BorderLineColor, BorderLineWidth, BorderLineStyle);
				return;
			}
			AntiAliasing antiAliasing = g.AntiAliasing;
			g.AntiAliasing = AntiAliasing.None;
			try
			{
				RectangleF rect = new RectangleF(0f, 0f, Width, Height);
				g.FillRectangleAbs(rect, BackColor, BackHatchStyle, "", MapImageWrapMode.Unscaled, Color.Empty, MapImageAlign.Center, BackGradientType, BackSecondaryColor, Color.Empty, 0, MapDashStyle.Solid, PenAlignment.Inset);
				if (BorderLineWidth <= 0 || BorderLineColor.IsEmpty || BorderLineStyle == MapDashStyle.None)
				{
					return;
				}
				using (Pen pen = new Pen(BorderLineColor, BorderLineWidth))
				{
					pen.DashStyle = MapGraphics.GetPenStyle(BorderLineStyle);
					pen.Alignment = PenAlignment.Inset;
					if (BorderLineWidth == 1)
					{
						rect.Width -= 1f;
						rect.Height -= 1f;
					}
					g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
				}
			}
			finally
			{
				g.AntiAliasing = antiAliasing;
			}
		}

		internal void RenderElementsBufered(MapGraphics g)
		{
			if (dirtyFlag || bufferBitmap == null || bufferBitmap.Graphics.ClipBounds != RectangleF.Union(g.Graphics.ClipBounds, bufferBitmap.Graphics.ClipBounds))
			{
				bufferBitmap = InitBitmap(bufferBitmap);
				Graphics graphics = g.Graphics;
				try
				{
					g.Graphics = bufferBitmap.Graphics;
					g.Graphics.SetClip(graphics);
					RenderElements(g);
				}
				finally
				{
					g.Graphics = graphics;
				}
			}
			if (!MapControl.Enabled)
			{
				ImageAttributes imageAttributes = new ImageAttributes();
				ColorMatrix colorMatrix = new ColorMatrix();
				if (!MapControl.Enabled)
				{
					colorMatrix.Matrix00 = 0.3f;
					colorMatrix.Matrix01 = 0.3f;
					colorMatrix.Matrix02 = 0.3f;
					colorMatrix.Matrix10 = 0.3f;
					colorMatrix.Matrix11 = 0.3f;
					colorMatrix.Matrix12 = 0.3f;
					colorMatrix.Matrix20 = 0.3f;
					colorMatrix.Matrix21 = 0.3f;
					colorMatrix.Matrix22 = 0.3f;
				}
				imageAttributes.SetColorMatrix(colorMatrix);
				g.Graphics.DrawImage(bufferBitmap.Bitmap, Rectangle.Round(g.Graphics.VisibleClipBounds), g.Graphics.VisibleClipBounds.X, g.Graphics.VisibleClipBounds.Y, g.Graphics.VisibleClipBounds.Width, g.Graphics.VisibleClipBounds.Height, GraphicsUnit.Pixel, imageAttributes);
			}
			else
			{
				g.Graphics.DrawImage(bufferBitmap.Bitmap, g.Graphics.VisibleClipBounds, g.Graphics.VisibleClipBounds, GraphicsUnit.Pixel);
			}
		}

		internal void RenderSelectedContentElements(MapGraphics g, RectangleF clipRect)
		{
			ISelectable[] selectedContentElements = GetSelectedContentElements();
			for (int i = 0; i < selectedContentElements.Length; i++)
			{
				selectedContentElements[i].DrawSelection(g, clipRect, designTimeSelection: false);
			}
			if (SelectedDesignTimeElement is IContentElement)
			{
				SelectedDesignTimeElement.DrawSelection(g, clipRect, designTimeSelection: true);
			}
			if (SelectedDesignTimeElement is Layer)
			{
				SelectedDesignTimeElement.DrawSelection(g, clipRect, designTimeSelection: true);
			}
		}

		internal void RenderSelectedPanels(MapGraphics g, RectangleF clipRect)
		{
			ISelectable[] selectedPanels = GetSelectedPanels();
			for (int i = 0; i < selectedPanels.Length; i++)
			{
				selectedPanels[i].DrawSelection(g, clipRect, designTimeSelection: false);
			}
			if (SelectedDesignTimeElement is Panel)
			{
				SelectedDesignTimeElement.DrawSelection(g, clipRect, designTimeSelection: true);
			}
		}

		internal ISelectable[] GetSelectedContentElements()
		{
			ArrayList arrayList = new ArrayList();
			foreach (ISelectable group in Groups)
			{
				if (group.IsSelected())
				{
					arrayList.Add(group);
				}
			}
			foreach (ISelectable shape in Shapes)
			{
				if (shape.IsSelected())
				{
					arrayList.Add(shape);
				}
			}
			foreach (ISelectable path in Paths)
			{
				if (path.IsSelected())
				{
					arrayList.Add(path);
				}
			}
			foreach (ISelectable symbol in Symbols)
			{
				if (symbol.IsSelected())
				{
					arrayList.Add(symbol);
				}
			}
			return (ISelectable[])arrayList.ToArray(typeof(ISelectable));
		}

		internal ISelectable[] GetSelectedPanels()
		{
			ArrayList arrayList = new ArrayList();
			Panel[] array = GetSortedPanels();
			foreach (ISelectable selectable in array)
			{
				if (selectable.IsSelected())
				{
					arrayList.Add(selectable);
				}
			}
			return (ISelectable[])arrayList.ToArray(typeof(ISelectable));
		}

		internal double PixelsToKilometers(float pixels)
		{
			PointF absoluteLocation = Viewport.GetAbsoluteLocation();
			SizeF absoluteSize = Viewport.GetAbsoluteSize();
			PointF pointInPixels = new PointF(absoluteLocation.X + absoluteSize.Width / 2f, absoluteLocation.Y + absoluteSize.Height / 2f);
			PointF pointInPixels2 = new PointF(pointInPixels.X + pixels, pointInPixels.Y);
			MapPoint point = PixelsToGeographic(pointInPixels);
			MapPoint point2 = PixelsToGeographic(pointInPixels2);
			return MeasureDistance(point, point2);
		}

		internal double MeasureDistance(MapPoint point1, MapPoint point2)
		{
			point1.X *= Math.PI / 180.0;
			point1.Y *= Math.PI / 180.0;
			point2.X *= Math.PI / 180.0;
			point2.Y *= Math.PI / 180.0;
			double num = point2.X - point1.X;
			double num2 = point2.Y - point1.Y;
			double num3 = Math.Sin(num2 / 2.0) * Math.Sin(num2 / 2.0) + Math.Cos(point2.Y) * Math.Cos(point1.Y) * Math.Sin(num / 2.0) * Math.Sin(num / 2.0);
			double num4 = 2.0 * Math.Atan2(Math.Sqrt(num3), Math.Sqrt(1.0 - num3));
			return 6378.137 * num4;
		}

		internal double GetMinimumAbsoluteLatitude()
		{
			return GetMinimumAbsoluteLatitude(MinimumPoint.Y, MaximumPoint.Y);
		}

		internal double GetMinimumAbsoluteLatitude(double minY, double maxY)
		{
			if (minY <= 0.0 && maxY >= 0.0)
			{
				return 0.0;
			}
			if (minY <= 0.0 && maxY <= 0.0)
			{
				return maxY;
			}
			return minY;
		}

		internal double LimitValue(double value, double lowerLimit, double upperLimit)
		{
			if (value < lowerLimit)
			{
				return lowerLimit;
			}
			if (value > upperLimit)
			{
				return upperLimit;
			}
			return value;
		}

		internal Point3D ApplyProjection(MapPoint mapPoint)
		{
			return ApplyProjection(mapPoint.X, mapPoint.Y);
		}

		internal Point3D ApplyProjection(double longitude, double latitude)
		{
			double num = longitude;
			double num2 = latitude;
			double z = 1.0;
			Projection projection = Projection;
			if (projection != Projection.Orthographic)
			{
				num -= MapCenterPoint.X;
			}
			if (projection != 0)
			{
				num = LimitValue(num, -180.0, 180.0);
				num2 = LimitValue(num2, -89.0, 89.0);
			}
			switch (projection)
			{
			case Projection.Mercator:
			{
				num2 = LimitValue(num2, -85.05112878, 85.05112878);
				double num8 = num2 * RadsInDegree;
				num2 = Math.Log(Math.Tan(num8) + 1.0 / Math.Cos(num8));
				num2 *= DegreesInRad;
				break;
			}
			case Projection.Wagner3:
			{
				double num9 = num * RadsInDegree;
				double num10 = num2 * RadsInDegree;
				num = Math.Cos(0.0) / Math.Cos(0.0) * num9 * Math.Cos(2.0 * num10 / 3.0);
				num *= DegreesInRad;
				break;
			}
			case Projection.Fahey:
			{
				double num25 = num * RadsInDegree;
				double num26 = Math.Tan(num2 * RadsInDegree / 2.0);
				num = num25 * Cos35 * Math.Sqrt(1.0 - num26 * num26);
				num2 = (1.0 + Cos35) * num26;
				num *= DegreesInRad;
				num2 *= DegreesInRad;
				break;
			}
			case Projection.Eckert1:
			{
				double num23 = num * RadsInDegree;
				double num24 = num2 * RadsInDegree;
				num = Eckert1Constant * num23 * (1.0 - Math.Abs(num24) / Math.PI);
				num2 = Eckert1Constant * num24;
				num *= DegreesInRad;
				num2 *= DegreesInRad;
				break;
			}
			case Projection.Eckert3:
			{
				double num21 = num * RadsInDegree;
				double num22 = num2 * RadsInDegree;
				num = Eckert3ConstantA * num21 * (1.0 + Math.Sqrt(1.0 - 4.0 * (num22 / Math.PI) * (num22 / Math.PI)));
				num2 = Eckert3ConstantB * num22;
				num *= DegreesInRad;
				num2 *= DegreesInRad;
				break;
			}
			case Projection.HammerAitoff:
			{
				double num18 = num * RadsInDegree;
				double num19 = num2 * RadsInDegree;
				double num20 = Math.Sqrt(1.0 + Math.Cos(num19) * Math.Cos(num18 / 2.0));
				num = 2.0 * Math.Cos(num19) * Math.Sin(num18 / 2.0) / num20;
				num2 = Math.Sin(num19) / num20;
				num *= DegreesInRad;
				num2 *= DegreesInRad;
				break;
			}
			case Projection.Robinson:
			{
				double num11 = Math.Abs(num2);
				int num12 = (int)(num11 / 5.0);
				double num13 = num11 - Phi[num12, 0];
				double num14 = num13 * num13;
				double num15 = num14 * num13;
				double num16 = A[num12, 0] * num15 + A[num12, 1] * num14 + A[num12, 2] * num13 + A[num12, 3];
				double num17 = B[num12, 0] * num15 + B[num12, 1] * num14 + B[num12, 2] * num13 + B[num12, 3];
				num *= num16;
				num2 = num17 * (double)Math.Sign(num2) * DegreesInRad;
				break;
			}
			case Projection.Orthographic:
			{
				double xValueRad = num * RadsInDegree;
				double yValueRad = num2 * RadsInDegree;
				Point3D globePoint = GetGlobePoint(xValueRad, yValueRad);
				globePoint = RotateAndTilt(globePoint, MapCenterPoint.X, MapCenterPoint.Y);
				num = globePoint.X;
				num2 = globePoint.Y;
				z = globePoint.Z;
				num *= DegreesInRad;
				num2 *= DegreesInRad;
				break;
			}
			case Projection.Bonne:
			{
				double num3 = num * RadsInDegree;
				double num4 = num2 * RadsInDegree;
				double num5 = MapCenterPoint.Y;
				if (Math.Abs(num5) < 0.001)
				{
					num5 = 0.001;
				}
				num5 *= RadsInDegree;
				double num6 = 1.0 / Math.Tan(num5) + num5 - num4;
				double num7 = num3 * Math.Cos(num4) / num6;
				num = num6 * Math.Sin(num7);
				num2 = 1.0 / Math.Tan(num5) - num6 * Math.Cos(num7);
				num *= DegreesInRad;
				num2 *= DegreesInRad;
				break;
			}
			}
			return new Point3D(num, num2, z);
		}

		private Point3D RotateAndTilt(Point3D point, double longitude, double latitude)
		{
			longitude = longitude * Math.PI / 180.0;
			latitude = latitude * Math.PI / 180.0;
			double num = Math.Sin(longitude);
			double num2 = Math.Cos(longitude);
			double num3 = Math.Sin(latitude);
			double num4 = Math.Cos(latitude);
			Matrix3x3 matrix3x = new Matrix3x3();
			matrix3x.Elements[0, 0] = num2;
			matrix3x.Elements[2, 0] = num;
			matrix3x.Elements[0, 2] = 0.0 - num;
			matrix3x.Elements[2, 2] = num2;
			Matrix3x3 matrix3x2 = new Matrix3x3();
			matrix3x2.Elements[1, 1] = num4;
			matrix3x2.Elements[2, 1] = num3;
			matrix3x2.Elements[1, 2] = 0.0 - num3;
			matrix3x2.Elements[2, 2] = num4;
			return (matrix3x2 * matrix3x).TransformPoint(point);
		}

		private Point3D GetGlobePoint(double xValueRad, double yValueRad)
		{
			double num = Math.Cos(yValueRad);
			Point3D result = default(Point3D);
			result.X = num * Math.Sin(xValueRad);
			result.Y = Math.Sin(yValueRad);
			result.Z = num * Math.Cos(xValueRad);
			return result;
		}

		internal MapPoint InverseProjection(double projectedX, double projectedY)
		{
			double num = projectedX;
			double num2 = projectedY;
			Projection projection = Projection;
			if (projection != 0)
			{
				MapBounds boundsAfterProjection = GetBoundsAfterProjection();
				if (num < boundsAfterProjection.MinimumPoint.X)
				{
					num = boundsAfterProjection.MinimumPoint.X;
				}
				else if (num > boundsAfterProjection.MaximumPoint.X)
				{
					num = boundsAfterProjection.MaximumPoint.X;
				}
				if (num2 < boundsAfterProjection.MinimumPoint.Y)
				{
					num2 = boundsAfterProjection.MinimumPoint.Y;
				}
				else if (num2 > boundsAfterProjection.MaximumPoint.Y)
				{
					num2 = boundsAfterProjection.MaximumPoint.Y;
				}
			}
			switch (projection)
			{
			case Projection.Mercator:
				num2 = Math.Atan(Math.Sinh(num2 * RadsInDegree));
				num2 *= DegreesInRad;
				break;
			case Projection.Wagner3:
			{
				double num34 = num * RadsInDegree;
				double num35 = num2 * RadsInDegree;
				num = num34 / Math.Cos(2.0 * num35 / 3.0);
				num *= DegreesInRad;
				break;
			}
			case Projection.Fahey:
			{
				double num32 = num * RadsInDegree;
				double num33 = num2 * RadsInDegree;
				num2 = 2.0 * Math.Atan(num33 / (1.0 + Cos35));
				num = num32 / (Cos35 * Math.Sqrt(1.0 - Math.Tan(num2 / 2.0) * Math.Tan(num2 / 2.0)));
				num *= DegreesInRad;
				num2 *= DegreesInRad;
				break;
			}
			case Projection.Eckert1:
			{
				double num31 = num * RadsInDegree;
				num2 = num2 * RadsInDegree / Eckert1Constant;
				num = (0.0 - num31) * Math.PI / (Eckert1Constant * (Math.Abs(num2) - Math.PI));
				num *= DegreesInRad;
				num2 *= DegreesInRad;
				break;
			}
			case Projection.Eckert3:
			{
				double num8 = num * RadsInDegree;
				double num9 = num2 * RadsInDegree;
				num = num8 * Eckert3ConstantC * Math.PI / (Math.PI * 2.0 + Math.Sqrt(39.478417604357432 - 4.0 * num9 * num9 * Math.PI - num9 * num9 * Math.PI * Math.PI));
				num2 = 0.25 * num9 * Eckert3ConstantC;
				num *= DegreesInRad;
				num2 *= DegreesInRad;
				break;
			}
			case Projection.HammerAitoff:
			{
				double num10 = num * RadsInDegree;
				double num11 = num2 * RadsInDegree;
				double num12 = Math.Sqrt(1.0 - Math.Pow(Math.Sqrt(2.0) / 4.0 * num10, 2.0) - Math.Pow(Math.Sqrt(2.0) / 2.0 * num11, 2.0));
				num = 2.0 * Math.Atan(Math.Sqrt(2.0) * num12 * num10 / (2.0 * (2.0 * num12 * num12 - 1.0)));
				num2 = Math.Asin(Math.Sqrt(2.0) * num11 * num12);
				num *= DegreesInRad;
				num2 *= DegreesInRad;
				break;
			}
			case Projection.Robinson:
			{
				double num22 = Math.Abs(num2) * RadsInDegree;
				int i;
				for (i = 0; i < 18; i++)
				{
					if (Phi[i, 4] > num22)
					{
						i--;
						break;
					}
				}
				if (i == 18)
				{
					i--;
				}
				double num23 = num22 - Phi[i, 4];
				double num24 = num23 * num23;
				double num25 = num24 * num23;
				double num26 = Phi[i, 1] * num25 + Phi[i, 2] * num24 + Phi[i, 3] * num23 + Phi[i, 0];
				double num27 = num26 - Phi[i, 0];
				double num28 = num27 * num27;
				double num29 = num28 * num27;
				double num30 = A[i, 0] * num29 + A[i, 1] * num28 + A[i, 2] * num27 + A[i, 3];
				num /= num30;
				num2 = num26 * (double)Math.Sign(num2);
				break;
			}
			case Projection.Orthographic:
			{
				double num13 = num * RadsInDegree;
				double num14 = num2 * RadsInDegree;
				double num15 = MapCenterPoint.Y * RadsInDegree;
				double num16 = Math.Sqrt(num13 * num13 + num14 * num14);
				double num17 = Math.Asin(num16);
				double num18 = Math.Sin(num17);
				double num19 = Math.Cos(num17);
				double num20 = Math.Sin(num15);
				double num21 = Math.Cos(num15);
				num = Math.Atan2(num13 * num18, num16 * num21 * num19 - num14 * num20 * num18);
				num2 = Math.Asin(num19 * num20 + num14 * num18 * num21 / num16);
				num *= DegreesInRad;
				num2 *= DegreesInRad;
				break;
			}
			case Projection.Bonne:
			{
				double num3 = num * RadsInDegree;
				double num4 = num2 * RadsInDegree;
				double num5 = MapCenterPoint.Y;
				if (Math.Abs(num5) < 0.001)
				{
					num5 = 0.001;
				}
				num5 *= RadsInDegree;
				double num6 = 1.0 / Math.Tan(num5);
				double num7 = Math.Sqrt(num3 * num3 + (num6 - num4) * (num6 - num4));
				num7 *= (double)Math.Sign(num5);
				num2 = num6 + num5 - num7;
				num = num7 / Math.Cos(num2) * Math.Atan(num3 / (num6 - num4));
				num *= DegreesInRad;
				num2 *= DegreesInRad;
				break;
			}
			}
			num += MapCenterPoint.X;
			if (num < MinimumPoint.X)
			{
				num = MinimumPoint.X;
			}
			else if (num > MaximumPoint.X)
			{
				num = MaximumPoint.X;
			}
			if (num2 < MinimumPoint.Y)
			{
				num2 = MinimumPoint.Y;
			}
			else if (num2 > MaximumPoint.Y)
			{
				num2 = MaximumPoint.Y;
			}
			return new MapPoint(num, num2);
		}

		internal double CalculateAspectRatio()
		{
			MapBounds boundsAfterProjection = GetBoundsAfterProjection();
			double num = Math.Abs(boundsAfterProjection.MaximumPoint.X - boundsAfterProjection.MinimumPoint.X);
			double num2 = Math.Abs(boundsAfterProjection.MaximumPoint.Y - boundsAfterProjection.MinimumPoint.Y);
			if (num2 > 1E-07)
			{
				return num / num2;
			}
			return 1.0;
		}

		internal void ResetCachedBoundsAfterProjection()
		{
			cachedBoundsAfterProjection = null;
		}

		internal MapBounds GetBoundsAfterProjection()
		{
			if (cachedBoundsAfterProjection != null)
			{
				return cachedBoundsAfterProjection;
			}
			cachedBoundsAfterProjection = GetBoundsAfterProjection(MinimumPoint.X, MinimumPoint.Y, MaximumPoint.X, MaximumPoint.Y);
			return cachedBoundsAfterProjection;
		}

		internal MapBounds GetBoundsAfterProjection(double minX, double minY, double maxX, double maxY)
		{
			Point3D point3D = ApplyProjection(minX, minY);
			Point3D point3D2 = ApplyProjection(maxX, maxY);
			if (Projection != 0 && Projection != Projection.Mercator)
			{
				List<Point3D> list = new List<Point3D>();
				double num = (maxX - minX) / 250.0;
				if (minX + num == minX)
				{
					num = double.MaxValue;
				}
				if (Projection == Projection.Bonne || Projection == Projection.Orthographic)
				{
					for (double num2 = minX; num2 <= maxX; num2 += num)
					{
						list.Add(ApplyProjection(num2, minY));
						list.Add(ApplyProjection(num2, maxY));
					}
				}
				num = (maxY - minY) / 250.0;
				if (minY + num == minY)
				{
					num = double.MaxValue;
				}
				for (double num3 = minY; num3 <= maxY; num3 += num)
				{
					list.Add(ApplyProjection(minX, num3));
					list.Add(ApplyProjection(maxX, num3));
				}
				list.Add(ApplyProjection(maxX, maxY));
				double minimumAbsoluteLatitude = GetMinimumAbsoluteLatitude(minY, maxY);
				list.Add(ApplyProjection(minX, minimumAbsoluteLatitude));
				list.Add(ApplyProjection(maxX, minimumAbsoluteLatitude));
				list.Add(ApplyProjection(MapCenterPoint.X, minY));
				list.Add(ApplyProjection(MapCenterPoint.X, maxY));
				list.Add(ApplyProjection(minX, MapCenterPoint.Y));
				list.Add(ApplyProjection(maxX, MapCenterPoint.Y));
				if (Projection == Projection.Orthographic)
				{
					if (minX < -90.0 && maxX > -90.0)
					{
						list.Add(ApplyProjection(-90.0, minimumAbsoluteLatitude));
					}
					if (minX < 90.0 && maxX > 90.0)
					{
						list.Add(ApplyProjection(90.0, minimumAbsoluteLatitude));
					}
				}
				foreach (Point3D item in list)
				{
					if (item.Z >= 0.0)
					{
						point3D.X = Math.Min(point3D.X, item.X);
						point3D.Y = Math.Min(point3D.Y, item.Y);
						point3D2.X = Math.Max(point3D2.X, item.X);
						point3D2.Y = Math.Max(point3D2.Y, item.Y);
					}
				}
			}
			return new MapBounds(new MapPoint(point3D.X, point3D.Y), new MapPoint(point3D2.X, point3D2.Y));
		}

		internal RectangleF GetGeographicClipRectangle(RectangleF clipRectangle)
		{
			if (!cachedGeographicClipRectangles.ContainsKey(clipRectangle))
			{
				Projection projection = Projection;
				double step = ((uint)projection > 1u) ? 5.0 : 1000000.0;
				IEnumerable<PointF> points = Utils.DensifyPoints(Utils.GetRectangePoints(clipRectangle), step);
				cachedGeographicClipRectangles[clipRectangle] = GetGeographicRectangle(points);
			}
			return cachedGeographicClipRectangles[clipRectangle];
		}

		internal void ResetGeographicClipRectangles()
		{
			cachedGeographicClipRectangles.Clear();
		}

		internal RectangleF GetGeographicRectangle(IEnumerable<PointF> points)
		{
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			float num4 = float.MinValue;
			foreach (PointF point in points)
			{
				MapPoint mapPoint = ContentToGeographic(point);
				if (!double.IsNaN(mapPoint.X))
				{
					num = Math.Min(num, (float)mapPoint.X);
					num2 = Math.Min(num2, (float)mapPoint.Y);
					num3 = Math.Max(num3, (float)mapPoint.X);
					num4 = Math.Max(num4, (float)mapPoint.Y);
					continue;
				}
				num = (float)minimumPoint.X;
				num2 = (float)minimumPoint.Y;
				num3 = (float)maximumPoint.X;
				num4 = (float)maximumPoint.Y;
				break;
			}
			RectangleF result = RectangleF.Empty;
			if (num != float.MinValue)
			{
				result = new RectangleF(num, num2, num3 - num, num4 - num2);
			}
			return result;
		}

		internal void PanBy(Point delta)
		{
			PointF contentOffsetInPixels = Viewport.GetContentOffsetInPixels();
			contentOffsetInPixels.X += delta.X;
			contentOffsetInPixels.Y += delta.Y;
			SizeF sizeInPixels = Viewport.GetSizeInPixels();
			contentOffsetInPixels.X = Math.Min(contentOffsetInPixels.X, sizeInPixels.Width - 20f);
			contentOffsetInPixels.Y = Math.Min(contentOffsetInPixels.Y, sizeInPixels.Height - 20f);
			SizeF contentSizeInPixels = Viewport.GetContentSizeInPixels();
			contentSizeInPixels.Height *= Viewport.Zoom / 100f;
			contentSizeInPixels.Width *= Viewport.Zoom / 100f;
			contentOffsetInPixels.X = Math.Max(contentOffsetInPixels.X, 0f - contentSizeInPixels.Width + 20f);
			contentOffsetInPixels.Y = Math.Max(contentOffsetInPixels.Y, 0f - contentSizeInPixels.Height + 20f);
			Viewport.SetContentOffsetInPixels(contentOffsetInPixels);
		}

		internal void EnsureContentIsVisible()
		{
			PointF contentOffsetInPixels = Viewport.GetContentOffsetInPixels();
			PointF right = contentOffsetInPixels;
			SizeF sizeInPixels = Viewport.GetSizeInPixels();
			if (contentOffsetInPixels.X > sizeInPixels.Width - 20f)
			{
				contentOffsetInPixels.X = sizeInPixels.Width - 20f;
			}
			if (contentOffsetInPixels.Y > sizeInPixels.Height - 20f)
			{
				contentOffsetInPixels.Y = sizeInPixels.Height - 20f;
			}
			SizeF contentSizeInPixels = Viewport.GetContentSizeInPixels();
			contentSizeInPixels.Height *= Viewport.Zoom / 100f;
			contentSizeInPixels.Width *= Viewport.Zoom / 100f;
			if (contentOffsetInPixels.X < 0f - contentSizeInPixels.Width + 20f)
			{
				contentOffsetInPixels.X = 0f - contentSizeInPixels.Width + 20f;
			}
			if (contentOffsetInPixels.Y < 0f - contentSizeInPixels.Height + 20f)
			{
				contentOffsetInPixels.Y = 0f - contentSizeInPixels.Height + 20f;
			}
			if (contentOffsetInPixels != right)
			{
				Viewport.SetContentOffsetInPixels(contentOffsetInPixels);
			}
		}

		internal void CenterView(MapPoint pointOnMap)
		{
			PointF pointF = GeographicToPercents(pointOnMap).ToPointF();
			Viewport.ViewCenter.X = pointF.X;
			Viewport.ViewCenter.Y = pointF.Y;
		}

		internal void Scroll(ScrollDirection direction, double scrollStep)
		{
			_ = (PointF)Viewport.ViewCenter;
			SizeF sizeInPixels = Viewport.GetSizeInPixels();
			sizeInPixels.Width *= (float)(scrollStep / 100.0);
			sizeInPixels.Height *= (float)(scrollStep / 100.0);
			Size size = Size.Round(sizeInPixels);
			Size size2 = default(Size);
			if ((direction & ScrollDirection.North) != 0)
			{
				size2.Height = size.Height;
			}
			if ((direction & ScrollDirection.South) != 0)
			{
				size2.Height = -size.Height;
			}
			if ((direction & ScrollDirection.East) != 0)
			{
				size2.Width = -size.Width;
			}
			if ((direction & ScrollDirection.West) != 0)
			{
				size2.Width = size.Width;
			}
			PanBy(new Point(size2.Width, size2.Height));
		}

		private void RenderGrid(MapGraphics g, PointF gridSectionOffset)
		{
			try
			{
				g.CreateContentDrawRegion(Viewport, gridSectionOffset);
				GraphicsState graphicsState = null;
				bool flag = Parallels.Visible && Parallels.LineColor != Color.Transparent && Parallels.Interval != 0.0 && Parallels.LineWidth != 0;
				bool flag2 = Meridians.Visible && Meridians.LineColor != Color.Transparent && Meridians.Interval != 0.0 && Meridians.LineWidth != 0;
				if (flag || flag2)
				{
					graphicsState = g.Save();
				}
				RectangleF bounds = g.Clip.GetBounds(g.Graphics);
				if (flag)
				{
					Parallels.GridLines = GetParallels(g);
					if (DrawParallelLabels())
					{
						GraphicsPath graphicsPath = new GraphicsPath();
						graphicsPath.FillMode = FillMode.Winding;
						GridLine[] gridLines = Parallels.GridLines;
						for (int i = 0; i < gridLines.Length; i++)
						{
							GridLine gridLine = gridLines[i];
							RectangleF labelRect = gridLine.LabelRect;
							if (!labelRect.IsEmpty && bounds.IntersectsWith(gridLine.LabelRect))
							{
								graphicsPath.AddRectangle(gridLine.LabelRect);
							}
						}
						g.SetClip(graphicsPath, CombineMode.Exclude);
					}
				}
				if (flag2)
				{
					Meridians.GridLines = GetMeridians(g, Parallels.GridLines);
					if (DrawMeridianLabels())
					{
						GraphicsPath graphicsPath2 = new GraphicsPath();
						graphicsPath2.FillMode = FillMode.Winding;
						GridLine[] gridLines = Meridians.GridLines;
						for (int i = 0; i < gridLines.Length; i++)
						{
							GridLine gridLine2 = gridLines[i];
							RectangleF labelRect = gridLine2.LabelRect;
							if (!labelRect.IsEmpty && bounds.IntersectsWith(gridLine2.LabelRect))
							{
								graphicsPath2.AddRectangle(gridLine2.LabelRect);
							}
						}
						g.SetClip(graphicsPath2, CombineMode.Exclude);
					}
				}
				if (flag)
				{
					RenderParallels(g, Parallels.GridLines);
				}
				if (flag2)
				{
					RenderMeridians(g, Meridians.GridLines);
				}
				if (graphicsState != null)
				{
					g.Restore(graphicsState);
				}
				if (SelectedDesignTimeElement is GridAttributes)
				{
					SelectedDesignTimeElement.DrawSelection(g, RectangleF.Empty, designTimeSelection: true);
				}
			}
			finally
			{
				g.RestoreDrawRegion();
			}
		}

		internal bool DrawParallelLabels()
		{
			if (UseGridSectionRendering() && GridUnderContent)
			{
				return false;
			}
			return Parallels.ShowLabels;
		}

		internal bool DrawMeridianLabels()
		{
			if (UseGridSectionRendering() && GridUnderContent)
			{
				return false;
			}
			return Meridians.ShowLabels;
		}

		internal void RenderParallels(MapGraphics g, GridLine[] parallels)
		{
			AntiAliasing antiAliasing = g.AntiAliasing;
			if (!AreParallelsCurved())
			{
				g.AntiAliasing = AntiAliasing.None;
			}
			g.StartHotRegion(Parallels);
			GraphicsPath[] array = new GraphicsPath[parallels.Length];
			using (Pen pen = Parallels.GetPen())
			{
				int num = 0;
				for (int i = 0; i < parallels.Length; i++)
				{
					GridLine gridLine = parallels[i];
					if (gridLine.Path != null)
					{
						g.DrawPath(pen, gridLine.Path);
						array[num++] = gridLine.Path;
					}
				}
			}
			HotRegionList.SetHotRegion(g, Parallels, array);
			g.EndHotRegion();
			g.AntiAliasing = antiAliasing;
		}

		internal void RenderMeridians(MapGraphics g, GridLine[] meridians)
		{
			AntiAliasing antiAliasing = g.AntiAliasing;
			if (!AreMeridiansCurved() && Projection != Projection.Eckert1)
			{
				g.AntiAliasing = AntiAliasing.None;
			}
			g.StartHotRegion(Meridians);
			GraphicsPath[] array = new GraphicsPath[meridians.Length];
			using (Pen pen = Meridians.GetPen())
			{
				int num = 0;
				for (int i = 0; i < meridians.Length; i++)
				{
					GridLine gridLine = meridians[i];
					if (gridLine.Path != null)
					{
						g.DrawPath(pen, gridLine.Path);
						array[num++] = gridLine.Path;
					}
				}
			}
			HotRegionList.SetHotRegion(g, Meridians, array);
			g.EndHotRegion();
			g.AntiAliasing = antiAliasing;
		}

		internal GridLine[] GetParallels(MapGraphics g)
		{
			bool flag = AreParallelsCurved();
			int num = (!flag) ? 10 : 50;
			double[] parallelPositions = GetParallelPositions();
			GridLine[] array = new GridLine[parallelPositions.Length];
			for (int i = 0; i < parallelPositions.Length; i++)
			{
				PointF[] array2 = new PointF[num];
				double num2 = Math.Abs(MaximumPoint.X - MinimumPoint.X) / (double)(num - 1);
				double num3 = MinimumPoint.X;
				for (int j = 0; j < num; j++)
				{
					array2[j] = GeographicToPercents(num3, parallelPositions[i]).ToPointF();
					array2[j] = g.GetAbsolutePoint(array2[j]);
					num3 += num2;
				}
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.StartFigure();
				if (flag)
				{
					graphicsPath.AddCurve(array2);
				}
				else
				{
					graphicsPath.AddLines(array2);
				}
				array[i].GridType = GridType.Parallel;
				array[i].Points = array2;
				array[i].Path = graphicsPath;
				array[i].Coordinate = parallelPositions[i];
			}
			CalculateSelectionMarkerPositions(g, array);
			if (DrawParallelLabels())
			{
				string[] array3 = new string[parallelPositions.Length];
				SizeF[] array4 = new SizeF[parallelPositions.Length];
				float num4 = 0f;
				for (int k = 0; k < parallelPositions.Length; k++)
				{
					array3[k] = FormatParallelLabel(parallelPositions[k]);
					array4[k] = g.MeasureString(array3[k], Parallels.Font);
					num4 = Math.Max(num4, array4[k].Width);
				}
				MapPoint mapPoint = CalculateIdealLabelPointForParallels(g, num4);
				for (int l = 0; l < parallelPositions.Length; l++)
				{
					StringFormat stringFormat = new StringFormat();
					stringFormat.Alignment = StringAlignment.Center;
					stringFormat.LineAlignment = StringAlignment.Center;
					MapPoint mapPoint2 = new MapPoint(mapPoint.X, parallelPositions[l]);
					PointF absolutePoint = g.GetAbsolutePoint(GeographicToPercents(mapPoint2).ToPointF());
					RectangleF rectangleF = new RectangleF(absolutePoint.X, absolutePoint.Y, 0f, 0f);
					rectangleF.Inflate(array4[l].Width / 2f, array4[l].Height / 2f);
					bool flag2 = false;
					for (int num5 = l - 1; num5 >= 0; num5--)
					{
						if (RectangleF.Intersect(array[num5].LabelRect, rectangleF) != RectangleF.Empty)
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						array[l].LabelRect = rectangleF;
						using (Brush brush = new SolidBrush(Parallels.LabelColor))
						{
							g.DrawString(array3[l], Parallels.Font, brush, absolutePoint, stringFormat);
						}
					}
				}
			}
			return array;
		}

		private string FormatParallelLabel(double value)
		{
			if (MapControl.FormatNumberHandler != null)
			{
				return MapControl.FormatNumberHandler(MapControl, value, Parallels.LabelFormatString);
			}
			return value.ToString(Parallels.LabelFormatString, CultureInfo.CurrentCulture);
		}

		private string FormatMeridianLabel(double value)
		{
			if (MapControl.FormatNumberHandler != null)
			{
				return MapControl.FormatNumberHandler(MapControl, value, Meridians.LabelFormatString);
			}
			return value.ToString(Meridians.LabelFormatString, CultureInfo.CurrentCulture);
		}

		private void CalculateSelectionMarkerPositions(MapGraphics g, GridLine[] gridLines)
		{
			if (gridLines.Length != 0)
			{
				RectangleF pixelsRect = new RectangleF(Viewport.GetAbsoluteLocation(), Viewport.GetAbsoluteSize());
				pixelsRect.Inflate(-3f / g.ScaleFactorX, -3f / g.ScaleFactorY);
				pixelsRect.Width -= 1f;
				pixelsRect.Height -= 1f;
				pixelsRect = PixelsToContent(pixelsRect);
				for (int i = 0; i < gridLines.Length; i++)
				{
					gridLines[i].SelectionMarkerPositions = FindAllIntersectingPoints(gridLines[i], pixelsRect);
				}
			}
		}

		private float CalculateYIntersect(PointF point1, PointF point2, float xIntersect)
		{
			if (point1.X == point2.X)
			{
				return point1.X;
			}
			return (point2.Y - point1.Y) / (point2.X - point1.X) * (xIntersect - point1.X) + point1.Y;
		}

		private float CalculateXIntersect(PointF point1, PointF point2, float yIntersect)
		{
			return CalculateYIntersect(new PointF(point1.Y, point1.X), new PointF(point2.Y, point2.X), yIntersect);
		}

		private PointF[] FindAllIntersectingPoints(GridLine gridLine, RectangleF rect)
		{
			Hashtable hashtable = new Hashtable();
			if (gridLine.GridType == GridType.Parallel)
			{
				FindIntersectingPoints(hashtable, gridLine.Points, rect.Left, rect.Top, rect.Left, rect.Bottom);
				FindIntersectingPoints(hashtable, gridLine.Points, rect.Right, rect.Top, rect.Right, rect.Bottom);
				FindIntersectingPoints(hashtable, gridLine.Points, rect.Left, rect.Top, rect.Right, rect.Top);
				FindIntersectingPoints(hashtable, gridLine.Points, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
			}
			else
			{
				FindIntersectingPoints(hashtable, gridLine.Points, rect.Left, rect.Top, rect.Right, rect.Top);
				FindIntersectingPoints(hashtable, gridLine.Points, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
				FindIntersectingPoints(hashtable, gridLine.Points, rect.Left, rect.Top, rect.Left, rect.Bottom);
				FindIntersectingPoints(hashtable, gridLine.Points, rect.Right, rect.Top, rect.Right, rect.Bottom);
			}
			if (rect.Contains(gridLine.Points[0]))
			{
				hashtable[gridLine.Points[0]] = null;
			}
			if (rect.Contains(gridLine.Points[gridLine.Points.Length - 1]))
			{
				hashtable[gridLine.Points[gridLine.Points.Length - 1]] = null;
			}
			PointF[] array = new PointF[hashtable.Keys.Count];
			int num = 0;
			foreach (PointF key in hashtable.Keys)
			{
				array[num++] = key;
			}
			return array;
		}

		private void FindIntersectingPoints(Hashtable resultSet, PointF[] points, float x1, float y1, float x2, float y2)
		{
			if (x1 == x2)
			{
				for (int i = 0; i < points.Length - 1; i++)
				{
					if ((points[i].X <= x1 && x1 <= points[i + 1].X) || (points[i + 1].X <= x1 && x1 <= points[i].X))
					{
						float num = CalculateYIntersect(points[i], points[i + 1], x1);
						if (y1 <= num && num <= y2)
						{
							resultSet[new PointF(x1, num)] = null;
						}
					}
				}
				return;
			}
			if (y1 == y2)
			{
				for (int j = 0; j < points.Length - 1; j++)
				{
					if ((points[j].Y <= y1 && y1 <= points[j + 1].Y) || (points[j + 1].Y <= y1 && y1 <= points[j].Y))
					{
						float num2 = CalculateXIntersect(points[j], points[j + 1], y1);
						if (x1 <= num2 && num2 <= x2)
						{
							resultSet[new PointF(num2, y1)] = null;
						}
					}
				}
				return;
			}
			throw new NotSupportedException();
		}

		private MapPoint CalculateIdealLabelPointForParallels(MapGraphics g, float labelWidth)
		{
			PointF empty = PointF.Empty;
			if (RenderingMode == RenderingMode.GridSections)
			{
				if (Parallels.LabelPosition == LabelPosition.Near)
				{
					empty.X = 0f;
				}
				else if (Parallels.LabelPosition == LabelPosition.OneQuarter)
				{
					empty.X = 25f;
				}
				else if (Parallels.LabelPosition == LabelPosition.Center)
				{
					empty.X = 50f;
				}
				else if (Parallels.LabelPosition == LabelPosition.ThreeQuarters)
				{
					empty.X = 75f;
				}
				else if (Parallels.LabelPosition == LabelPosition.Far)
				{
					empty.X = 100f;
				}
				empty.Y = 50f;
				return PercentsToGeographic(empty.X, empty.Y);
			}
			RectangleF rectangleF = new RectangleF(Viewport.GetAbsoluteLocation(), Viewport.GetAbsoluteSize());
			float num = 8f;
			if (Parallels.LabelPosition == LabelPosition.Near)
			{
				empty.X = rectangleF.X + labelWidth / 2f + num;
			}
			else if (Parallels.LabelPosition == LabelPosition.OneQuarter)
			{
				empty.X = rectangleF.X + rectangleF.Width * 0.25f;
			}
			else if (Parallels.LabelPosition == LabelPosition.Center)
			{
				empty.X = rectangleF.X + rectangleF.Width * 0.5f;
			}
			else if (Parallels.LabelPosition == LabelPosition.ThreeQuarters)
			{
				empty.X = rectangleF.X + rectangleF.Width * 0.75f;
			}
			else if (Parallels.LabelPosition == LabelPosition.Far)
			{
				empty.X = rectangleF.X + rectangleF.Width - labelWidth / 2f - num;
			}
			empty.Y = rectangleF.Y + rectangleF.Height / 2f;
			empty = PixelsToContent(empty);
			PointF relativePoint = g.GetRelativePoint(empty);
			return PercentsToGeographic(relativePoint.X, relativePoint.Y);
		}

		private MapPoint CalculateIdealLabelPointForMeridians(MapGraphics g, float labelHeight)
		{
			PointF empty = PointF.Empty;
			if (RenderingMode == RenderingMode.GridSections)
			{
				if (Meridians.LabelPosition == LabelPosition.Near)
				{
					empty.Y = 0f;
				}
				else if (Meridians.LabelPosition == LabelPosition.OneQuarter)
				{
					empty.Y = 25f;
				}
				else if (Meridians.LabelPosition == LabelPosition.Center)
				{
					empty.Y = 50f;
				}
				else if (Meridians.LabelPosition == LabelPosition.ThreeQuarters)
				{
					empty.Y = 75f;
				}
				else if (Meridians.LabelPosition == LabelPosition.Far)
				{
					empty.Y = 100f;
				}
				empty.X = 50f;
				return PercentsToGeographic(empty.X, empty.Y);
			}
			RectangleF rectangleF = new RectangleF(Viewport.GetAbsoluteLocation(), Viewport.GetAbsoluteSize());
			float num = 8f;
			if (Meridians.LabelPosition == LabelPosition.Near)
			{
				empty.Y = rectangleF.Y + labelHeight / 2f + num;
			}
			else if (Meridians.LabelPosition == LabelPosition.OneQuarter)
			{
				empty.Y = rectangleF.Y + rectangleF.Height * 0.25f;
			}
			else if (Meridians.LabelPosition == LabelPosition.Center)
			{
				empty.Y = rectangleF.Y + rectangleF.Height * 0.5f;
			}
			else if (Meridians.LabelPosition == LabelPosition.ThreeQuarters)
			{
				empty.Y = rectangleF.Y + rectangleF.Height * 0.75f;
			}
			else if (Meridians.LabelPosition == LabelPosition.Far)
			{
				empty.Y = rectangleF.Y + rectangleF.Height - labelHeight / 2f - num;
			}
			empty.X = rectangleF.X + rectangleF.Width / 2f;
			empty = PixelsToContent(empty);
			PointF relativePoint = g.GetRelativePoint(empty);
			return PercentsToGeographic(relativePoint.X, relativePoint.Y);
		}

		internal GridLine[] GetMeridians(MapGraphics g, GridLine[] parallels)
		{
			bool flag = AreMeridiansCurved();
			int num = (!flag) ? 5 : 50;
			double[] meridianPositions = GetMeridianPositions();
			GridLine[] array = new GridLine[meridianPositions.Length];
			for (int i = 0; i < meridianPositions.Length; i++)
			{
				PointF[] array2 = new PointF[num];
				double num2 = Math.Abs(MaximumPoint.Y - MinimumPoint.Y) / (double)(num - 1);
				double num3 = MinimumPoint.Y;
				for (int j = 0; j < num; j++)
				{
					if (num == 3 && j == 1)
					{
						array2[j] = GeographicToPercents(meridianPositions[i], 0.0).ToPointF();
					}
					else
					{
						array2[j] = GeographicToPercents(meridianPositions[i], num3).ToPointF();
					}
					array2[j] = g.GetAbsolutePoint(array2[j]);
					num3 += num2;
				}
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.StartFigure();
				if (flag)
				{
					graphicsPath.AddCurve(array2);
				}
				else
				{
					graphicsPath.AddLines(array2);
				}
				array[i].GridType = GridType.Meridian;
				array[i].Points = array2;
				array[i].Path = graphicsPath;
				array[i].Coordinate = meridianPositions[i];
			}
			CalculateSelectionMarkerPositions(g, array);
			if (DrawMeridianLabels())
			{
				float height = g.MeasureString("5", Meridians.Font).Height;
				MapPoint mapPoint = CalculateIdealLabelPointForMeridians(g, height);
				for (int k = 0; k < meridianPositions.Length; k++)
				{
					StringFormat stringFormat = new StringFormat();
					stringFormat.Alignment = StringAlignment.Center;
					stringFormat.LineAlignment = StringAlignment.Center;
					MapPoint mapPoint2 = new MapPoint(meridianPositions[k], mapPoint.Y);
					PointF absolutePoint = g.GetAbsolutePoint(GeographicToPercents(mapPoint2).ToPointF());
					string text = FormatMeridianLabel(meridianPositions[k]);
					SizeF sizeF = g.MeasureString(text, Parallels.Font);
					RectangleF rectangleF = new RectangleF(absolutePoint.X, absolutePoint.Y, 0f, 0f);
					rectangleF.Inflate(sizeF.Width / 2f, sizeF.Height / 2f);
					bool flag2 = false;
					if (parallels != null)
					{
						for (int l = 0; l < parallels.Length; l++)
						{
							if (RectangleF.Intersect(parallels[l].LabelRect, rectangleF) != RectangleF.Empty)
							{
								flag2 = true;
								break;
							}
						}
					}
					if (flag2)
					{
						continue;
					}
					bool flag3 = false;
					for (int num4 = k - 1; num4 >= 0; num4--)
					{
						if (RectangleF.Intersect(array[num4].LabelRect, rectangleF) != RectangleF.Empty)
						{
							flag3 = true;
							break;
						}
					}
					if (!flag3)
					{
						array[k].LabelRect = rectangleF;
						using (Brush brush = new SolidBrush(Meridians.LabelColor))
						{
							g.DrawString(text, Meridians.Font, brush, absolutePoint, stringFormat);
						}
					}
				}
			}
			return array;
		}

		private double GetParallelInterval()
		{
			if (!double.IsNaN(Parallels.Interval))
			{
				return Parallels.Interval;
			}
			if ((MaximumPoint.Y == 90.0 && MinimumPoint.Y == -90.0) || (MaximumPoint.Y == 89.0 && MinimumPoint.Y == -89.0) || (MaximumPoint.Y == 85.05112878 && MinimumPoint.Y == -85.05112878))
			{
				return 10.0;
			}
			double num = (MaximumPoint.Y - MinimumPoint.Y) / 20.0;
			int num2 = (int)Math.Log10(num);
			double num3 = Math.Pow(10.0, num2);
			return Math.Ceiling(num / num3) * num3;
		}

		private double GetMeridianInterval()
		{
			if (!double.IsNaN(Meridians.Interval))
			{
				return Meridians.Interval;
			}
			double num = (MaximumPoint.X - MinimumPoint.X) / 20.0;
			int num2 = (int)Math.Log10(num);
			double num3 = Math.Pow(10.0, num2);
			return Math.Ceiling(num / num3) * num3;
		}

		private double[] GetParallelPositions()
		{
			ArrayList arrayList = new ArrayList();
			double num = GetParallelInterval();
			double num2 = MaximumPoint.Y - MinimumPoint.Y;
			if (num2 / num > 1000.0)
			{
				num = num2 / 1000.0;
			}
			if (MinimumPoint.Y < 0.0 && MaximumPoint.Y > 0.0)
			{
				double num3;
				for (num3 = 0.0; num3 <= MaximumPoint.Y; num3 += num)
				{
					num3 = double.Parse(num3.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
					arrayList.Add(num3);
				}
				for (num3 = 0.0 - num; num3 >= MinimumPoint.Y; num3 -= num)
				{
					num3 = double.Parse(num3.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
					arrayList.Add(num3);
				}
			}
			else if (MaximumPoint.Y > 0.0)
			{
				double num4;
				for (num4 = Math.Ceiling(MinimumPoint.Y / num) * num; num4 <= MaximumPoint.Y; num4 += num)
				{
					num4 = double.Parse(num4.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
					arrayList.Add(num4);
				}
			}
			else if (MinimumPoint.Y < 0.0)
			{
				double num5;
				for (num5 = Math.Floor(MaximumPoint.Y / num) * num; num5 >= MinimumPoint.Y; num5 -= num)
				{
					num5 = double.Parse(num5.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
					arrayList.Add(num5);
				}
			}
			arrayList.Sort();
			return (double[])arrayList.ToArray(typeof(double));
		}

		private double[] GetMeridianPositions()
		{
			ArrayList arrayList = new ArrayList();
			double num = GetMeridianInterval();
			double num2 = MaximumPoint.X - MinimumPoint.X;
			if (num2 / num > 2000.0)
			{
				num = num2 / 2000.0;
			}
			if (MinimumPoint.X < 0.0 && MaximumPoint.X > 0.0)
			{
				double num3;
				for (num3 = 0.0; num3 <= MaximumPoint.X; num3 += num)
				{
					num3 = double.Parse(num3.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
					arrayList.Add(num3);
				}
				for (num3 = 0.0 - num; num3 >= MinimumPoint.X; num3 -= num)
				{
					num3 = double.Parse(num3.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
					arrayList.Add(num3);
				}
			}
			else if (MaximumPoint.X > 0.0)
			{
				double num4;
				for (num4 = Math.Ceiling(MinimumPoint.X / num) * num; num4 <= MaximumPoint.X; num4 += num)
				{
					num4 = double.Parse(num4.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
					arrayList.Add(num4);
				}
			}
			else if (MinimumPoint.X < 0.0)
			{
				double num5;
				for (num5 = Math.Floor(MaximumPoint.X / num) * num; num5 >= MinimumPoint.X; num5 -= num)
				{
					num5 = double.Parse(num5.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
					arrayList.Add(num5);
				}
			}
			arrayList.Sort();
			return (double[])arrayList.ToArray(typeof(double));
		}

		internal bool AreParallelsCurved()
		{
			if (Projection == Projection.HammerAitoff || Projection == Projection.Orthographic || Projection == Projection.Bonne)
			{
				return true;
			}
			return false;
		}

		internal bool AreMeridiansCurved()
		{
			if (Projection == Projection.Equirectangular || Projection == Projection.Mercator || Projection == Projection.Eckert1)
			{
				return false;
			}
			return true;
		}

		internal void InvalidateRules()
		{
			if (!applyingRules)
			{
				rulesDirty = true;
			}
		}

		internal void ApplyAllRules()
		{
			if (!rulesDirty)
			{
				return;
			}
			if (Common != null && Common.MapControl != null)
			{
				Common.MapControl.OnBeforeApplyingRules(EventArgs.Empty);
			}
			applyingRules = true;
			rulesDirty = false;
			for (int i = 0; i < ColorSwatchPanel.Colors.Count; i++)
			{
				if (ColorSwatchPanel.Colors[i].AutomaticallyAdded)
				{
					ColorSwatchPanel.Colors.RemoveAt(i--);
				}
			}
			foreach (Legend legend in Legends)
			{
				for (int j = 0; j < legend.Items.Count; j++)
				{
					if (legend.Items[j].AutomaticallyAdded)
					{
						legend.Items.RemoveAt(j--);
					}
				}
			}
			foreach (GroupRule groupRule in GroupRules)
			{
				groupRule.RegenerateColorRanges();
			}
			foreach (ShapeRule shapeRule in ShapeRules)
			{
				shapeRule.RegenerateColorRanges();
			}
			foreach (PathRuleBase pathRule in PathRules)
			{
				pathRule.RegenerateRanges();
			}
			foreach (SymbolRule symbolRule in SymbolRules)
			{
				symbolRule.UpdateAutoRanges();
			}
			foreach (Group group in Groups)
			{
				group.UseInternalProperties = false;
				foreach (GroupRule groupRule2 in GroupRules)
				{
					groupRule2.Apply(group);
				}
			}
			foreach (Shape shape in Shapes)
			{
				shape.UseInternalProperties = false;
				foreach (ShapeRule shapeRule2 in ShapeRules)
				{
					shapeRule2.Apply(shape);
				}
			}
			foreach (Path path in Paths)
			{
				path.UseInternalProperties = false;
				foreach (PathRuleBase pathRule2 in PathRules)
				{
					pathRule2.Apply(path);
				}
			}
			foreach (Symbol symbol in Symbols)
			{
				symbol.UseInternalProperties = false;
				foreach (SymbolRule symbolRule2 in SymbolRules)
				{
					symbolRule2.Apply(symbol);
				}
			}
			applyingRules = false;
			if (Common != null && Common.MapControl != null)
			{
				Common.MapControl.OnAllRulesApplied(EventArgs.Empty);
			}
		}

		internal int GetWidth()
		{
			if (isPrinting)
			{
				return printSize.Width;
			}
			return MapControl.Width;
		}

		internal int GetHeight()
		{
			if (isPrinting)
			{
				return printSize.Height;
			}
			return MapControl.Height;
		}

		internal void ResetAutoValues()
		{
			if (SelectedDesignTimeElement != null)
			{
				SelectedDesignTimeElement = null;
			}
		}

		internal override void Invalidate()
		{
			dirtyFlag = true;
		}

		internal override void Invalidate(RectangleF rect)
		{
			dirtyFlag = true;
		}

		internal override void InvalidateViewport(bool invalidateGridSections)
		{
			if (Viewport != null && !disableInvalidate)
			{
				if (invalidateGridSections)
				{
					InvalidateGridSections();
				}
				RectangleF rect = new RectangleF(Viewport.GetAbsoluteLocation(), Viewport.GetAbsoluteSize());
				Invalidate(rect);
			}
		}

		internal override void InvalidateDistanceScalePanel()
		{
			if (DistanceScalePanel != null)
			{
				RectangleF rect = new RectangleF(DistanceScalePanel.GetAbsoluteLocation(), DistanceScalePanel.GetAbsoluteSize());
				Invalidate(rect);
			}
		}

		internal override void InvalidateAndLayout()
		{
			DoPanelLayout = true;
			Invalidate();
		}

		internal void InvalidateCachedPaths()
		{
			if (!resetingCachedPaths)
			{
				cachedPathsDirty = true;
			}
		}

		internal void ResetCachedPaths()
		{
			if (!cachedPathsDirty)
			{
				return;
			}
			resetingCachedPaths = true;
			cachedPathsDirty = false;
			foreach (Shape shape in Shapes)
			{
				shape.ResetCachedPaths();
			}
			foreach (Path path in Paths)
			{
				path.ResetCachedPaths();
			}
			foreach (Symbol symbol in Symbols)
			{
				symbol.ResetCachedPaths();
			}
			resetingCachedPaths = false;
		}

		internal void OnFontChanged()
		{
		}

		internal NamedCollection[] GetRenderCollections()
		{
			return (NamedCollection[])new ArrayList
			{
				DataBindingRules,
				Symbols,
				SymbolRules,
				SymbolFields,
				Shapes,
				ShapeRules,
				ShapeFields,
				Paths,
				PathRules,
				PathFields,
				Groups,
				GroupRules,
				GroupFields,
				Layers,
				Images,
				Labels,
				Legends
			}.ToArray(typeof(NamedCollection));
		}

		internal override void BeginInit()
		{
			isInitializing = true;
			NamedCollection[] renderCollections = GetRenderCollections();
			for (int i = 0; i < renderCollections.Length; i++)
			{
				renderCollections[i].BeginInit();
			}
		}

		internal override void EndInit()
		{
			skipPaint = false;
			isInitializing = false;
			NamedCollection[] renderCollections = GetRenderCollections();
			for (int i = 0; i < renderCollections.Length; i++)
			{
				renderCollections[i].EndInit();
			}
			LoadFieldsFromBuffers();
			InvalidateCachedBounds();
			InvalidateRules();
			InvalidateCachedPaths();
			InvalidateChildSymbols();
			InvalidateDataBinding();
			InvalidateAndLayout();
			if (AutoUpdates)
			{
				UpdateCachedBounds();
				ResetCachedPaths();
				ResetChildSymbols();
			}
		}

		private void LoadFieldsFromBuffers()
		{
			foreach (Group group in Groups)
			{
				group.FieldDataFromBuffer();
			}
			foreach (Shape shape in Shapes)
			{
				shape.FieldDataFromBuffer();
			}
			foreach (Path path in Paths)
			{
				path.FieldDataFromBuffer();
			}
			foreach (Symbol symbol in Symbols)
			{
				symbol.FieldDataFromBuffer();
			}
		}

		internal override void ReconnectData(bool exact)
		{
			NamedCollection[] renderCollections = GetRenderCollections();
			for (int i = 0; i < renderCollections.Length; i++)
			{
				renderCollections[i].ReconnectData(exact);
			}
		}

		internal override void Notify(MessageType msg, NamedElement element, object param)
		{
			NamedCollection[] renderCollections = GetRenderCollections();
			for (int i = 0; i < renderCollections.Length; i++)
			{
				renderCollections[i].Notify(msg, element, param);
			}
		}

		internal void SaveTo(string fileName, MapImageFormat imageFormat, int compression, Panel panel, bool zoomThumbOnly)
		{
			using (Stream stream = new FileStream(fileName, FileMode.Create))
			{
				SaveTo(stream, imageFormat, compression, panel, zoomThumbOnly);
				stream.Close();
			}
		}

		internal void SaveTo(Stream stream, MapImageFormat imageFormat, int compression, Panel panel, bool zoomThumbOnly)
		{
			if (panel != null)
			{
				if (panel is ZoomPanel && zoomThumbOnly)
				{
					RenderingMode = RenderingMode.ZoomThumb;
				}
				else
				{
					RenderingMode = RenderingMode.SinglePanel;
				}
				PanelToRender = panel;
			}
			bool flag = dirtyFlag;
			dirtyFlag = true;
			Notify(MessageType.PrepareSnapShot, this, null);
			if (imageFormat == MapImageFormat.Emf)
			{
				SaveIntoMetafile(stream);
				dirtyFlag = flag;
				if (panel != null)
				{
					RenderingMode = RenderingMode.All;
					PanelToRender = null;
				}
				return;
			}
			BufferBitmap bufferBitmap = InitBitmap(null);
			RenderingType renderingType = RenderingType.Gdi;
			ImageFormat imageFormat2 = null;
			ImageCodecInfo imageCodecInfo = null;
			EncoderParameter encoderParameter = null;
			EncoderParameters encoderParameters = null;
			string text = imageFormat.ToString(CultureInfo.InvariantCulture);
			imageFormat2 = (ImageFormat)new ImageFormatConverter().ConvertFromString(text);
			Paint(bufferBitmap.Graphics, renderingType, stream, buffered: false);
			if (renderingType == RenderingType.Gdi && compression >= 0 && compression <= 100 && "JPEG,PNG,BMP".IndexOf(text.ToUpperInvariant(), StringComparison.Ordinal) != -1)
			{
				imageCodecInfo = GetEncoderInfo("image/" + text.ToLowerInvariant());
				encoderParameters = new EncoderParameters(1);
				encoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L - (long)compression);
				encoderParameters.Param[0] = encoderParameter;
			}
			if (!MapControl.Enabled)
			{
				ImageAttributes imageAttributes = new ImageAttributes();
				ColorMatrix colorMatrix = new ColorMatrix();
				colorMatrix.Matrix00 = 0.3f;
				colorMatrix.Matrix01 = 0.3f;
				colorMatrix.Matrix02 = 0.3f;
				colorMatrix.Matrix10 = 0.3f;
				colorMatrix.Matrix11 = 0.3f;
				colorMatrix.Matrix12 = 0.3f;
				colorMatrix.Matrix20 = 0.3f;
				colorMatrix.Matrix21 = 0.3f;
				colorMatrix.Matrix22 = 0.3f;
				imageAttributes.SetColorMatrix(colorMatrix);
				Region clip = bufferBitmap.Graphics.Clip;
				bufferBitmap.Graphics.Clip = GetClipRegion();
				bufferBitmap.Graphics.DrawImage(bufferBitmap.Bitmap, new Rectangle(0, 0, GetWidth(), GetHeight()), 0, 0, GetWidth(), GetHeight(), GraphicsUnit.Pixel, imageAttributes);
				bufferBitmap.Graphics.Clip.Dispose();
				bufferBitmap.Graphics.Clip = clip;
			}
			if (RenderingMode == RenderingMode.SinglePanel && PanelToRender.IsMakeTransparentRequired())
			{
				bufferBitmap.Bitmap.MakeTransparent(PanelToRender.GetColorForMakeTransparent());
			}
			if (renderingType == RenderingType.Gdi && imageFormat != MapImageFormat.Emf)
			{
				if (imageCodecInfo == null)
				{
					bufferBitmap.Bitmap.Save(stream, imageFormat2);
				}
				else
				{
					bufferBitmap.Bitmap.Save(stream, imageCodecInfo, encoderParameters);
				}
			}
			dirtyFlag = flag;
			if (panel != null)
			{
				RenderingMode = RenderingMode.All;
				PanelToRender = null;
			}
		}

		private static ImageCodecInfo GetEncoderInfo(string mimeType)
		{
			ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
			for (int i = 0; i < imageEncoders.Length; i++)
			{
				if (imageEncoders[i].MimeType == mimeType)
				{
					return imageEncoders[i];
				}
			}
			return null;
		}

		public void SaveIntoMetafile(Stream imageStream)
		{
			Bitmap bitmap = new Bitmap(GetWidth(), GetHeight());
			Graphics graphics = Graphics.FromImage(bitmap);
			IntPtr hdc = graphics.GetHdc();
			Metafile metafile = new Metafile(imageStream, hdc, new Rectangle(0, 0, GetWidth(), GetHeight()), MetafileFrameUnit.Pixel);
			Graphics graphics2 = Graphics.FromImage(metafile);
			graphics2.SmoothingMode = GetSmootingMode();
			graphics2.TextRenderingHint = GetTextRenderingHint();
			Paint(graphics2, RenderingType.Gdi, imageStream, buffered: false);
			byte[] data = new byte[12]
			{
				68,
				117,
				110,
				100,
				97,
				115,
				32,
				67,
				104,
				97,
				114,
				116
			};
			graphics2.AddMetafileComment(data);
			graphics2.Dispose();
			metafile.Dispose();
			graphics.ReleaseHdc(hdc);
			graphics.Dispose();
			bitmap.Dispose();
		}

		internal void SavePanelAsImage(Panel panel, string fileName, MapImageFormat format, int compression, bool zoomThumbOnly)
		{
			SaveTo(fileName, format, compression, panel, zoomThumbOnly);
		}

		internal void SavePanelAsImage(Panel panel, Stream stream, MapImageFormat format, int compression, bool zoomThumbOnly)
		{
			SaveTo(stream, format, compression, panel, zoomThumbOnly);
		}

		internal void SavePanelAsImage(Panel panel, Stream stream, bool zoomThumbOnly)
		{
			MapImageFormat imageFormat = (MapImageFormat)Enum.Parse(typeof(MapImageFormat), ImageType.ToString(CultureInfo.CurrentCulture), ignoreCase: true);
			SaveTo(stream, imageFormat, Compression, panel, zoomThumbOnly);
		}

		internal void SavePanelAsImage(Panel panel, string fileName, bool zoomThumbOnly)
		{
			MapImageFormat imageFormat = (MapImageFormat)Enum.Parse(typeof(MapImageFormat), ImageType.ToString(CultureInfo.CurrentCulture), ignoreCase: true);
			SaveTo(fileName, imageFormat, Compression, panel, zoomThumbOnly);
		}

		internal int GetSpatialElementCount()
		{
			int num = 0;
			foreach (Symbol symbol in Symbols)
			{
				if (symbol.ParentShape == "(none)")
				{
					num++;
				}
			}
			return Shapes.Count + Paths.Count + num;
		}

		internal int GetSpatialPointCount()
		{
			int num = 0;
			foreach (ISpatialElement path in Paths)
			{
				if (path.Points != null)
				{
					num += path.Points.Length;
				}
			}
			foreach (ISpatialElement path2 in Paths)
			{
				if (path2.Points != null)
				{
					num += path2.Points.Length;
				}
			}
			foreach (Symbol symbol in Symbols)
			{
				if (symbol.ParentShape == "(none)")
				{
					ISpatialElement spatialElement3 = symbol;
					if (spatialElement3.Points != null)
					{
						num += spatialElement3.Points.Length;
					}
				}
			}
			return num;
		}

		private Region GetClipRegion()
		{
			Region region = null;
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				foreach (HotRegion item in HotRegionList.List)
				{
					if (item.SelectedObject is Panel)
					{
						GraphicsPath[] array = item.Paths;
						foreach (GraphicsPath addingPath in array)
						{
							graphicsPath.AddPath(addingPath, connect: false);
						}
					}
				}
				return new Region(graphicsPath);
			}
		}

		internal bool IsDesignMode()
		{
			return MapControl.IsDesignMode();
		}

		internal void DrawException(Graphics graphics, Exception e)
		{
			graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, GetWidth(), GetHeight());
			graphics.DrawRectangle(new Pen(Color.Red, 4f), 0, 0, GetWidth(), GetHeight());
			string text = "Group Exception: " + e.Message;
			string text2 = "";
			if (e.TargetSite != null)
			{
				text2 = string.Concat(text2, "Site: ", e.TargetSite, "\r\n\r\n");
			}
			if (e.StackTrace != string.Empty)
			{
				text2 = text2 + "Stack Trace: \r\n" + e.StackTrace;
			}
			RectangleF layoutRectangle = new RectangleF(3f, 3f, GetWidth() - 6, GetHeight() - 6);
			StringFormat format = new StringFormat();
			SizeF sizeF = graphics.MeasureString(text, new Font("MS Sans Serif", 10f, FontStyle.Bold), (int)layoutRectangle.Width, format);
			graphics.DrawString(text, new Font("MS Sans Serif", 10f, FontStyle.Bold), new SolidBrush(Color.Black), layoutRectangle, format);
			layoutRectangle.Y += sizeF.Height + 5f;
			graphics.DrawString(text2, new Font("MS Sans Serif", 8f), new SolidBrush(Color.Black), layoutRectangle, format);
		}

		internal void PrepareHitTest()
		{
			if (HotRegionList.List.Count == 0)
			{
				using (MemoryStream stream = new MemoryStream())
				{
					SaveTo(stream, MapImageFormat.Bmp, -1, null, zoomThumbOnly: false);
				}
			}
		}

		internal HitTestResult[] HitTest(int x, int y, Type[] objectTypes, bool returnMultipleElements)
		{
			if (objectTypes == null)
			{
				objectTypes = new Type[0];
			}
			ArrayList arrayList = new ArrayList();
			HotRegion[] array = HotRegionList.CheckHotRegions(x, y, objectTypes, needTooltipOnly: false);
			PointF hitTestPoint = new PointF(x, y);
			if (array == null)
			{
				arrayList.Add(new HitTestResult(null, hitTestPoint));
			}
			else if (!returnMultipleElements)
			{
				arrayList.Add(new HitTestResult(array[0], hitTestPoint));
			}
			else
			{
				object obj = null;
				HotRegion[] array2 = array;
				foreach (HotRegion hotRegion in array2)
				{
					if (obj != hotRegion.SelectedObject)
					{
						arrayList.Add(new HitTestResult(hotRegion, hitTestPoint));
						obj = hotRegion.SelectedObject;
					}
				}
			}
			return (HitTestResult[])arrayList.ToArray(typeof(HitTestResult));
		}

		internal HotRegion GetHotRegion(NamedElement element)
		{
			if (element == null)
			{
				element = this;
			}
			if (element is IContentElement || element == this)
			{
				int num = HotRegionList.FindHotRegionOfObject(element);
				if (num != -1)
				{
					return (HotRegion)HotRegionList.List[num];
				}
				throw new ArgumentException(SR.hot_region_error_initialize(element.Name));
			}
			throw new ArgumentException(SR.hot_region_error_support(element.Name));
		}

		internal double GetMaximumSimplificationResolution()
		{
			if (Width == 0 || Height == 0)
			{
				return 0.0;
			}
			double num = MaximumPoint.X - MinimumPoint.X;
			double num2 = MaximumPoint.Y - MinimumPoint.Y;
			double val = num / (double)Width;
			double val2 = num2 / (double)Height;
			return Math.Min(val, val2);
		}

		internal PointF PercentsToPixels(PointF pointInPercents)
		{
			MapGraphics mapGraphics = new MapGraphics(null);
			mapGraphics.SetPictureSize(Width, Height);
			return mapGraphics.GetAbsolutePoint(pointInPercents);
		}

		internal PointF PixelsToPercents(PointF pointInPixels)
		{
			MapGraphics mapGraphics = new MapGraphics(null);
			mapGraphics.SetPictureSize(Width, Height);
			return mapGraphics.GetRelativePoint(pointInPixels);
		}

		internal SizeF PercentsToPixels(SizeF sizeInPercents)
		{
			MapGraphics mapGraphics = new MapGraphics(null);
			mapGraphics.SetPictureSize(Width, Height);
			return mapGraphics.GetAbsoluteSize(sizeInPercents);
		}

		internal SizeF PixelsToPercents(SizeF sizeInPixels)
		{
			MapGraphics mapGraphics = new MapGraphics(null);
			mapGraphics.SetPictureSize(Width, Height);
			return mapGraphics.GetRelativeSize(sizeInPixels);
		}

		internal Point3D GeographicToPercents(MapPoint mapPoint)
		{
			return GeographicToPercents(mapPoint.X, mapPoint.Y);
		}

		internal Point3D GeographicToPercents(double longtitude, double latitude)
		{
			Point3D result = ApplyProjection(longtitude, latitude);
			MapBounds boundsAfterProjection = GetBoundsAfterProjection();
			result.X -= boundsAfterProjection.MinimumPoint.X;
			result.Y -= boundsAfterProjection.MinimumPoint.Y;
			double num = boundsAfterProjection.MaximumPoint.X - boundsAfterProjection.MinimumPoint.X;
			double num2 = boundsAfterProjection.MaximumPoint.Y - boundsAfterProjection.MinimumPoint.Y;
			if (num > 0.0)
			{
				result.X *= 100.0 / num;
			}
			else
			{
				result.X = 0.0;
			}
			if (num2 > 0.0)
			{
				result.Y *= 100.0 / num2;
			}
			else
			{
				result.Y = 0.0;
			}
			result.Y = 100.0 - result.Y;
			return result;
		}

		internal MapPoint PercentsToGeographic(MapPoint point)
		{
			return PercentsToGeographic(point.X, point.Y);
		}

		internal MapPoint PercentsToGeographic(double pointX, double pointY)
		{
			double num = pointX;
			double num2 = pointY;
			num2 = 100.0 - num2;
			MapBounds boundsAfterProjection = GetBoundsAfterProjection();
			num *= (boundsAfterProjection.MaximumPoint.X - boundsAfterProjection.MinimumPoint.X) / 100.0;
			num2 *= (boundsAfterProjection.MaximumPoint.Y - boundsAfterProjection.MinimumPoint.Y) / 100.0;
			num += boundsAfterProjection.MinimumPoint.X;
			num2 += boundsAfterProjection.MinimumPoint.Y;
			return InverseProjection(num, num2);
		}

		internal MapPoint PixelsToGeographic(PointF pointInPixels)
		{
			return ContentToGeographic(PixelsToContent(pointInPixels));
		}

		internal PointF GeographicToPixels(MapPoint pointOnMap)
		{
			return ContentToPixels(GeographicToContent(pointOnMap));
		}

		internal MapPoint ContentToGeographic(PointF contentPoint)
		{
			SizeF contentSizeInPixels = Viewport.GetContentSizeInPixels();
			contentPoint.X *= 100f / contentSizeInPixels.Width;
			contentPoint.Y *= 100f / contentSizeInPixels.Height;
			contentPoint.X *= 100f / Viewport.Zoom;
			contentPoint.Y *= 100f / Viewport.Zoom;
			return PercentsToGeographic(contentPoint.X, contentPoint.Y);
		}

		internal PointF GeographicToContent(MapPoint pointOnMap)
		{
			SizeF contentSizeInPixels = Viewport.GetContentSizeInPixels();
			Point3D point3D = GeographicToPercents(pointOnMap);
			point3D.X *= Viewport.Zoom / 100f;
			point3D.Y *= Viewport.Zoom / 100f;
			point3D.X *= contentSizeInPixels.Width / 100f;
			point3D.Y *= contentSizeInPixels.Height / 100f;
			return point3D.ToPointF();
		}

		internal PointF PixelsToContent(PointF pointInPixels)
		{
			PointF contentOffsetInPixels = Viewport.GetContentOffsetInPixels();
			pointInPixels.X -= contentOffsetInPixels.X + (float)Viewport.Margins.Left;
			pointInPixels.Y -= contentOffsetInPixels.Y + (float)Viewport.Margins.Top;
			return pointInPixels;
		}

		internal RectangleF PixelsToContent(RectangleF pixelsRect)
		{
			PointF contentOffsetInPixels = Viewport.GetContentOffsetInPixels();
			pixelsRect.Offset(0f - (contentOffsetInPixels.X + (float)Viewport.Margins.Left), 0f - (contentOffsetInPixels.Y + (float)Viewport.Margins.Top));
			return pixelsRect;
		}

		internal PointF ContentToPixels(PointF contentPoint)
		{
			PointF contentOffsetInPixels = Viewport.GetContentOffsetInPixels();
			contentPoint.X += contentOffsetInPixels.X + (float)Viewport.Margins.Left;
			contentPoint.Y += contentOffsetInPixels.Y + (float)Viewport.Margins.Top;
			return contentPoint;
		}

		internal RectangleF ContentToPixels(RectangleF contentRectangle)
		{
			PointF pointF = ContentToPixels(contentRectangle.Location);
			PointF pointF2 = ContentToPixels(new PointF(contentRectangle.X + contentRectangle.Width, contentRectangle.Y + contentRectangle.Height));
			return new RectangleF(pointF.X, pointF.Y, pointF2.X - pointF.X, pointF2.Y - pointF.Y);
		}

		internal bool IsContentImageMapRequired()
		{
			if (!ImageMapEnabled)
			{
				return false;
			}
			foreach (IImageMapProvider shape in Shapes)
			{
				string toolTip = shape.GetToolTip();
				string href = shape.GetHref();
				string mapAreaAttributes = shape.GetMapAreaAttributes();
				if (!string.IsNullOrEmpty(toolTip) || !string.IsNullOrEmpty(href) || !string.IsNullOrEmpty(mapAreaAttributes))
				{
					return true;
				}
			}
			foreach (IImageMapProvider path in Paths)
			{
				string toolTip2 = path.GetToolTip();
				string href2 = path.GetHref();
				string mapAreaAttributes2 = path.GetMapAreaAttributes();
				if (!string.IsNullOrEmpty(toolTip2) || !string.IsNullOrEmpty(href2) || !string.IsNullOrEmpty(mapAreaAttributes2))
				{
					return true;
				}
			}
			foreach (IImageMapProvider symbol in Symbols)
			{
				string toolTip3 = symbol.GetToolTip();
				string href3 = symbol.GetHref();
				string mapAreaAttributes3 = symbol.GetMapAreaAttributes();
				if (!string.IsNullOrEmpty(toolTip3) || !string.IsNullOrEmpty(href3) || !string.IsNullOrEmpty(mapAreaAttributes3))
				{
					return true;
				}
			}
			return false;
		}

		internal MapAreaCollection GetMapAreasFromHotRegionList()
		{
			MapAreaCollection mapAreaCollection = new MapAreaCollection();
			for (int num = HotRegionList.List.Count - 1; num >= 0; num--)
			{
				HotRegion hotRegion = (HotRegion)HotRegionList.List[num];
				if (hotRegion.SelectedObject is IImageMapProvider)
				{
					IImageMapProvider obj = (IImageMapProvider)hotRegion.SelectedObject;
					string toolTip = obj.GetToolTip();
					string href = obj.GetHref();
					string mapAreaAttributes = obj.GetMapAreaAttributes();
					if (!string.IsNullOrEmpty(toolTip) || !string.IsNullOrEmpty(href) || !string.IsNullOrEmpty(mapAreaAttributes))
					{
						RectangleF rectangleF = new RectangleF(Point.Empty, GridSectionSize);
						if (hotRegion.SelectedObject is Path)
						{
							for (int i = 0; i < hotRegion.Paths.Length; i++)
							{
								if (hotRegion.Paths[i] == null)
								{
									continue;
								}
								using (Pen pen = ((Path)hotRegion.SelectedObject).GetBorderPen())
								{
									if (pen != null)
									{
										if (pen.Width < 7f)
										{
											pen.Width = 7f;
										}
										GraphicsPath graphicsPath = (GraphicsPath)hotRegion.Paths[i].Clone();
										graphicsPath.Widen(pen);
										graphicsPath.Flatten(null, 1f);
										if (rectangleF.IntersectsWith(graphicsPath.GetBounds()))
										{
											mapAreaCollection.Add(toolTip, href, mapAreaAttributes, graphicsPath);
										}
										graphicsPath.Dispose();
									}
								}
							}
						}
						else if (hotRegion.SelectedObject is Shape)
						{
							for (int j = 0; j < hotRegion.Paths.Length; j++)
							{
								if (hotRegion.Paths[j] == null || !rectangleF.IntersectsWith(hotRegion.Paths[j].GetBounds()))
								{
									continue;
								}
								PointF[] pathPoints = hotRegion.Paths[j].PathPoints;
								if (pathPoints.Length == 0)
								{
									continue;
								}
								int[] array = new int[pathPoints.Length * 2];
								int num2 = 0;
								bool flag = true;
								PointF[] array2 = pathPoints;
								for (int k = 0; k < array2.Length; k++)
								{
									PointF pt = array2[k];
									array[num2++] = (int)Math.Round(pt.X);
									array[num2++] = (int)Math.Round(pt.Y);
									if (flag && rectangleF.Contains(pt))
									{
										flag = false;
									}
								}
								if (flag)
								{
									PointF point = new PointF(rectangleF.Width / 2f, rectangleF.Height / 2f);
									if (hotRegion.Paths[j].IsVisible(point))
									{
										int[] coord = new int[4]
										{
											-1,
											-1,
											GridSectionSize.Width + 1,
											GridSectionSize.Height + 1
										};
										mapAreaCollection.Add(MapAreaShape.Rectangle, toolTip, href, mapAreaAttributes, coord);
									}
								}
								else
								{
									mapAreaCollection.Add(MapAreaShape.Polygon, toolTip, href, mapAreaAttributes, array);
								}
							}
						}
						else if (hotRegion.SelectedObject is IContentElement)
						{
							for (int l = 0; l < hotRegion.Paths.Length; l++)
							{
								if (hotRegion.Paths[l] != null)
								{
									mapAreaCollection.Add(toolTip, href, mapAreaAttributes, hotRegion.Paths[l]);
								}
							}
						}
					}
				}
			}
			return mapAreaCollection;
		}

		internal void PopulateImageMaps()
		{
			for (int num = HotRegionList.List.Count - 1; num >= 0; num--)
			{
				HotRegion hotRegion = (HotRegion)HotRegionList.List[num];
				if (hotRegion.SelectedObject is IImageMapProvider)
				{
					IImageMapProvider imageMapProvider = (IImageMapProvider)hotRegion.SelectedObject;
					string toolTip = imageMapProvider.GetToolTip();
					string href = imageMapProvider.GetHref();
					string mapAreaAttributes = imageMapProvider.GetMapAreaAttributes();
					if (!string.IsNullOrEmpty(toolTip) || !string.IsNullOrEmpty(href) || !string.IsNullOrEmpty(mapAreaAttributes))
					{
						object tag = imageMapProvider.Tag;
						for (int i = 0; i < hotRegion.Paths.Length; i++)
						{
							if (hotRegion.Paths[i] == null)
							{
								continue;
							}
							if (hotRegion.SelectedObject is Path)
							{
								using (Pen pen = ((Path)hotRegion.SelectedObject).GetBorderPen())
								{
									if (pen != null)
									{
										GraphicsPath graphicsPath = (GraphicsPath)hotRegion.Paths[i].Clone();
										graphicsPath.Widen(pen);
										graphicsPath.Flatten(null, 1f);
										MapAreas.Add(toolTip, href, mapAreaAttributes, graphicsPath, tag);
										graphicsPath.Dispose();
									}
								}
							}
							else
							{
								MapAreas.Add(toolTip, href, mapAreaAttributes, hotRegion.Paths[i], tag);
							}
						}
					}
				}
			}
		}

		internal void WriteMapTag(TextWriter output, string mapName)
		{
			output.Write("\r\n<MAP NAME=\"" + mapName + "\">");
			foreach (MapArea mapArea in mapAreas)
			{
				output.Write(mapArea.GetTag());
			}
			output.Write("\r\n</MAP>");
		}

		internal void InvalidateDataBinding()
		{
			if (!dataBinding && !invalidatingDataBind && !isInitializing && !IsSuspended)
			{
				try
				{
					invalidatingDataBind = true;
					ShapeFields.Purge();
					GroupFields.Purge();
					SymbolFields.Purge();
					boundToDataSource = false;
				}
				finally
				{
					invalidatingDataBind = false;
				}
			}
		}

		internal void AutoDataBind(bool forceBinding)
		{
			if (boundToDataSource)
			{
				return;
			}
			try
			{
				dataBinding = true;
				if (!(DataSource != null || forceBinding))
				{
					return;
				}
				foreach (DataBindingRuleBase dataBindingRule in DataBindingRules)
				{
					dataBindingRule.DataBind();
				}
				boundToDataSource = true;
			}
			finally
			{
				dataBinding = false;
			}
		}

		internal void DataBindShapes(object dataSource, string dataMember, string bindingField)
		{
			try
			{
				dataBinding = true;
				ExecuteDataBind(BindingType.Shapes, null, dataSource, dataMember, bindingField);
			}
			finally
			{
				dataBinding = false;
			}
		}

		internal void DataBindGroups(object dataSource, string dataMember, string bindingField)
		{
			try
			{
				dataBinding = true;
				ExecuteDataBind(BindingType.Groups, null, dataSource, dataMember, bindingField);
			}
			finally
			{
				dataBinding = false;
			}
		}

		internal void DataBindPaths(object dataSource, string dataMember, string bindingField)
		{
			try
			{
				dataBinding = true;
				ExecuteDataBind(BindingType.Paths, null, dataSource, dataMember, bindingField);
			}
			finally
			{
				dataBinding = false;
			}
		}

		internal void DataBindSymbols(object dataSource, string dataMember, string bindingField, string category, string parentShapeField, string xCoordinateField, string yCoordinateField)
		{
			try
			{
				if (string.IsNullOrEmpty(parentShapeField) && string.IsNullOrEmpty(xCoordinateField))
				{
					throw new ArgumentException(SR.symboldatabind_bad_loc_arg);
				}
				dataBinding = true;
				ExecuteDataBind(BindingType.Symbols, null, dataSource, dataMember, bindingField, category, parentShapeField, xCoordinateField, yCoordinateField);
			}
			finally
			{
				dataBinding = false;
			}
		}

		internal void ExecuteDataBind(BindingType bindingType, DataBindingRuleBase dataBinding, object dataSource, string dataMember, string bindingField, params string[] auxFields)
		{
			bool closeDataReader = false;
			bool flag = false;
			IDbConnection connection = null;
			bool dummyData = false;
			IEnumerable enumerable = null;
			try
			{
				if (!IsDesignMode() && (dataSource == null || bindingField == null || bindingField == string.Empty))
				{
					throw new ArgumentException(SR.bad_data_src_fields);
				}
				if (dataSource is IDesignTimeDataSource)
				{
					dummyData = true;
				}
				OnBeforeDataBind(new DataBindEventArgs(dataBinding));
				flag = true;
				DataBindingTargetResolver dataBindingTargetResolver = null;
				switch (bindingType)
				{
				case BindingType.Shapes:
					dataBindingTargetResolver = new DataBindingTargetResolver(ShapeFields, Shapes);
					break;
				case BindingType.Groups:
					dataBindingTargetResolver = new DataBindingTargetResolver(GroupFields, Groups);
					break;
				case BindingType.Paths:
					dataBindingTargetResolver = new DataBindingTargetResolver(PathFields, Paths);
					break;
				case BindingType.Symbols:
					if (auxFields.Length != 4)
					{
						if (!IsDesignMode())
						{
							throw new ArgumentException(SR.symbol_databinding_not_enough_params);
						}
						return;
					}
					dataBindingTargetResolver = new DataBindingTargetResolver(SymbolFields, Symbols);
					break;
				}
				ArrayList arrayList = null;
				if (!string.IsNullOrEmpty(bindingField))
				{
					arrayList = DataBindingHelper.GetDataSourceDataFields(dataSource, dataMember, bindingField);
					foreach (DataBindingHelper.DataFieldDescriptor item in arrayList)
					{
						string name = item.Name;
						Type type = Field.ConvertToSupportedType(item.Type);
						if (name != bindingField && !dataBindingTargetResolver.ContainsField(name))
						{
							Field field = new Field();
							field.Name = name;
							field.Type = type;
							field.IsTemporary = true;
							dataBindingTargetResolver.AddField(field);
						}
					}
				}
				else
				{
					arrayList = new ArrayList();
				}
				enumerable = DataBindingHelper.GetDataSourceAsIEnumerable(dataSource, dataMember, out closeDataReader, out connection);
				switch (bindingType)
				{
				case BindingType.Shapes:
				case BindingType.Groups:
				case BindingType.Paths:
					ExecuteFieldDataBind(enumerable, bindingField, arrayList, dataBindingTargetResolver, dummyData);
					break;
				case BindingType.Symbols:
					ExecuteSymbolDataBind(enumerable, bindingField, arrayList, dataBindingTargetResolver, auxFields[0], auxFields[1], auxFields[2], auxFields[3], dummyData);
					break;
				}
			}
			finally
			{
				if (closeDataReader && enumerable != null)
				{
					((IDataReader)enumerable).Close();
				}
				connection?.Close();
				if (flag)
				{
					OnAfterDataBind(new DataBindEventArgs(dataBinding));
				}
				Invalidate();
			}
		}

		private void ExecuteFieldDataBind(IEnumerable dataEnumerator, string bindingField, ArrayList columnDescrs, DataBindingTargetResolver targetResolver, bool dummyData)
		{
			object obj = null;
			int num = 0;
			if (columnDescrs.Count == 0)
			{
				return;
			}
			foreach (object item in dataEnumerator)
			{
				try
				{
					obj = DataBindingHelper.ConvertEnumerationItem(item, bindingField);
				}
				catch
				{
					if (!IsDesignMode())
					{
						throw;
					}
					obj = null;
				}
				NamedElement namedElement = null;
				if (!dummyData)
				{
					namedElement = targetResolver.GetItemById(obj);
				}
				else if (obj != null)
				{
					namedElement = targetResolver.GetItemByIndex(num++);
				}
				if (namedElement == null)
				{
					continue;
				}
				foreach (DataBindingHelper.DataFieldDescriptor columnDescr in columnDescrs)
				{
					object obj4 = null;
					Type type = Field.ConvertToSupportedType(columnDescr.Type);
					string name = columnDescr.Name;
					try
					{
						obj4 = DataBindingHelper.ConvertEnumerationItem(item, name);
					}
					catch
					{
						if (!IsDesignMode())
						{
							throw;
						}
					}
					if (obj4 == null || Convert.IsDBNull(obj4))
					{
						continue;
					}
					obj4 = Field.ConvertToSupportedValue(obj4);
					obj4.GetType();
					Field fieldByName = targetResolver.GetFieldByName(name);
					if (name.ToUpper(CultureInfo.CurrentCulture) == "NAME" || fieldByName == null)
					{
						continue;
					}
					if (fieldByName.Type != type)
					{
						if (!IsDesignMode())
						{
							BindingType bindingType = targetResolver.BindingType;
							throw new InvalidOperationException(SR.field_duplication(name, bindingType.ToString(CultureInfo.CurrentCulture), fieldByName.Type.Name, type.Name));
						}
					}
					else if (!dummyData || fieldByName.IsTemporary)
					{
						targetResolver.SetFieldValue(namedElement, name, obj4);
					}
				}
			}
		}

		private void ExecuteSymbolDataBind(IEnumerable dataEnumerator, string bindingField, ArrayList columnDescrs, DataBindingTargetResolver targetResolver, string category, string parentShapeField, string xCoordinateField, string yCoordinateField, bool dummyData)
		{
			if (string.IsNullOrEmpty(parentShapeField) && string.IsNullOrEmpty(xCoordinateField))
			{
				if (!IsDesignMode())
				{
					throw new ArgumentException(SR.symboldatabind_bad_loc_arg);
				}
				return;
			}
			object obj = null;
			DataBindingTargetResolver dataBindingTargetResolver = null;
			int num = 0;
			if (columnDescrs.Count == 0)
			{
				return;
			}
			if (!string.IsNullOrEmpty(parentShapeField))
			{
				dataBindingTargetResolver = new DataBindingTargetResolver(ShapeFields, Shapes);
			}
			foreach (object item in dataEnumerator)
			{
				try
				{
					obj = DataBindingHelper.ConvertEnumerationItem(item, bindingField);
				}
				catch
				{
					if (!IsDesignMode())
					{
						throw;
					}
					obj = null;
				}
				Symbol symbol = null;
				if (!dummyData)
				{
					symbol = (Symbol)targetResolver.GetItemById(obj);
				}
				else if (obj != null)
				{
					symbol = (Symbol)targetResolver.GetItemByIndex(num++);
				}
				if (symbol == null)
				{
					continue;
				}
				if (!IsDesignMode())
				{
					symbol.Category = category;
				}
				foreach (DataBindingHelper.DataFieldDescriptor columnDescr in columnDescrs)
				{
					object obj3 = null;
					Type type = Field.ConvertToSupportedType(columnDescr.Type);
					string name = columnDescr.Name;
					try
					{
						obj3 = DataBindingHelper.ConvertEnumerationItem(item, name);
					}
					catch
					{
						if (!IsDesignMode())
						{
							throw;
						}
					}
					if (obj3 == null || Convert.IsDBNull(obj3))
					{
						continue;
					}
					if (columnDescr.Name == parentShapeField && !IsDesignMode())
					{
						Shape shape = dataBindingTargetResolver.GetItemById(obj3) as Shape;
						if (shape != null)
						{
							symbol.ParentShape = shape.Name;
						}
					}
					else if (columnDescr.Name == xCoordinateField && !IsDesignMode())
					{
						if (!DataBindingHelper.IsValidAsCoordinateType(columnDescr.Type))
						{
							if (!IsDesignMode())
							{
								throw new NotSupportedException(SR.symbol_coord_type(columnDescr.Type.Name));
							}
							return;
						}
						if (columnDescr.Type == typeof(string))
						{
							if (xCoordinateField != yCoordinateField)
							{
								symbol.X = obj3.ToString();
							}
							else
							{
								symbol.SetCoordinates(obj3.ToString());
							}
							continue;
						}
						try
						{
							symbol.X = Convert.ToDouble(obj3, CultureInfo.CurrentCulture);
						}
						catch (InvalidCastException)
						{
							if (!IsDesignMode())
							{
								throw new NotSupportedException(SR.symbol_coord_type(columnDescr.Type.Name));
							}
							return;
						}
						catch
						{
							if (!IsDesignMode())
							{
								throw;
							}
							return;
						}
						continue;
					}
					if (columnDescr.Name == yCoordinateField && !IsDesignMode())
					{
						if (!DataBindingHelper.IsValidAsCoordinateType(columnDescr.Type))
						{
							if (!IsDesignMode())
							{
								throw new NotSupportedException(SR.symbol_coord_type(columnDescr.Type.Name));
							}
							return;
						}
						if (columnDescr.Type == typeof(string))
						{
							if (xCoordinateField != yCoordinateField)
							{
								symbol.Y = obj3.ToString();
							}
							continue;
						}
						try
						{
							symbol.Y = Convert.ToDouble(obj3, CultureInfo.InvariantCulture);
						}
						catch (InvalidCastException)
						{
							if (!IsDesignMode())
							{
								throw new NotSupportedException(SR.symbol_coord_type(columnDescr.Type.Name));
							}
							return;
						}
						catch
						{
							if (!IsDesignMode())
							{
								throw;
							}
							return;
						}
						continue;
					}
					obj3 = Field.ConvertToSupportedValue(obj3);
					obj3.GetType();
					Field fieldByName = targetResolver.GetFieldByName(name);
					if (name.ToUpper(CultureInfo.CurrentCulture) == "NAME" || fieldByName == null)
					{
						continue;
					}
					if (fieldByName.Type != type)
					{
						if (!IsDesignMode())
						{
							BindingType bindingType = targetResolver.BindingType;
							throw new InvalidOperationException(SR.field_duplication(name, bindingType.ToString(CultureInfo.CurrentCulture), fieldByName.Type.Name, type.Name));
						}
					}
					else if (!dummyData || fieldByName.IsTemporary)
					{
						targetResolver.SetFieldValue(symbol, name, obj3);
					}
				}
			}
		}

		private void OnBeforeDataBind(DataBindEventArgs e)
		{
			MapControl.OnBeforeDataBind(e);
		}

		private void OnAfterDataBind(DataBindEventArgs e)
		{
			MapControl.OnAfterDataBind(e);
		}

		internal string ResolveAllKeywords(string original, NamedElement element)
		{
			if (original.Length == 0)
			{
				return original;
			}
			string text = original;
			text = text.Replace("\\n", "\n");
			if (element is Group)
			{
				SortedList sortedList = new SortedList(new StringLengthReversedComparer());
				foreach (Field groupField in GroupFields)
				{
					sortedList.Add(groupField.GetKeyword(), groupField.Name);
				}
				foreach (DictionaryEntry item in sortedList)
				{
					object val = ((Group)element)[(string)item.Value];
					text = ResolveKeyword(text, (string)item.Key, val);
				}
				text = ResolveKeyword(text, "#NAME", element.Name);
			}
			else if (element is Shape)
			{
				SortedList sortedList2 = new SortedList(new StringLengthReversedComparer());
				foreach (Field shapeField in ShapeFields)
				{
					sortedList2[shapeField.GetKeyword()] = shapeField.Name;
				}
				foreach (DictionaryEntry item2 in sortedList2)
				{
					object val2 = ((Shape)element)[(string)item2.Value];
					text = ResolveKeyword(text, (string)item2.Key, val2);
				}
				text = ResolveKeyword(text, "#NAME", element.Name);
			}
			else if (element is Path)
			{
				SortedList sortedList3 = new SortedList(new StringLengthReversedComparer());
				foreach (Field pathField in PathFields)
				{
					sortedList3.Add(pathField.GetKeyword(), pathField.Name);
				}
				foreach (DictionaryEntry item3 in sortedList3)
				{
					object val3 = ((Path)element)[(string)item3.Value];
					text = ResolveKeyword(text, (string)item3.Key, val3);
				}
				text = ResolveKeyword(text, "#NAME", element.Name);
			}
			else if (element is Symbol)
			{
				SortedList sortedList4 = new SortedList(new StringLengthReversedComparer());
				foreach (Field symbolField in SymbolFields)
				{
					sortedList4.Add(symbolField.GetKeyword(), symbolField.Name);
				}
				foreach (DictionaryEntry item4 in sortedList4)
				{
					object val4 = ((Symbol)element)[(string)item4.Value];
					text = ResolveKeyword(text, (string)item4.Key, val4);
				}
				text = ResolveKeyword(text, "#NAME", element.Name);
			}
			return text;
		}

		internal string ResolveKeyword(string original, string keyword, object val)
		{
			string text = original;
			if (val == null)
			{
				val = "(null)";
			}
			for (int num = text.IndexOf(keyword + "{", StringComparison.Ordinal); num != -1; num = text.IndexOf(keyword + "{", num + 1, StringComparison.Ordinal))
			{
				int num2 = text.IndexOf("{", num, StringComparison.Ordinal);
				int num3 = text.IndexOf("}", num2, StringComparison.Ordinal);
				if (num3 == -1)
				{
					throw new InvalidOperationException(SR.ExceptionInvalidKeywordFormat(text));
				}
				string text2 = text.Substring(num2, num3 - num2 + 1);
				string text3;
				if (MapControl.FormatNumberHandler != null)
				{
					text3 = MapControl.FormatNumberHandler(MapControl, val, text2.Trim('{', '}'));
				}
				else
				{
					string format = text2.Replace("{", "{0:");
					text3 = string.Format(CultureInfo.CurrentCulture, format, val);
				}
				if (text3.Length > 80)
				{
					text3 = text3.Substring(0, 80) + "...";
				}
				text = text.Replace(keyword + text2, text3);
			}
			string text4 = val.ToString();
			if (text4.Length > 80)
			{
				text4 = text4.Substring(0, 80) + "...";
			}
			return text.Replace(keyword, text4);
		}

		internal static bool CheckLicense()
		{
			return true;
		}

		private RectangleF PerformPanelLayout(MapGraphics g, RectangleF bounds, List<DockablePanel> sortedPanels)
		{
			if (sortedPanels.Count == 0)
			{
				return bounds;
			}
			RectangleF unoccupiedArea = bounds;
			float top = bounds.Top;
			float bottom = bounds.Bottom;
			float top2 = bounds.Top;
			float bottom2 = bounds.Bottom;
			float left = bounds.Left;
			float right = bounds.Right;
			float left2 = bounds.Left;
			float right2 = bounds.Right;
			_ = bounds.Top;
			ArrayList arrayList = new ArrayList();
			_ = bounds.Bottom;
			ArrayList arrayList2 = new ArrayList();
			_ = bounds.Left;
			ArrayList arrayList3 = new ArrayList();
			_ = bounds.Right;
			ArrayList arrayList4 = new ArrayList();
			RectangleF unoccupiedArea2 = bounds;
			foreach (DockablePanel sortedPanel in sortedPanels)
			{
				if (sortedPanel is AutoSizePanel)
				{
					((AutoSizePanel)sortedPanel).AdjustAutoSize(g);
				}
				RectangleF rectangleF = MapGraphics.Round(sortedPanel.GetBoundsInPixels());
				bool flag = rectangleF.Width <= rectangleF.Height;
				switch (sortedPanel.Dock)
				{
				case PanelDockStyle.Top:
					if (sortedPanel.DockAlignment == DockAlignment.Near)
					{
						LayoutAlignPanel(sortedPanel, bounds, ref unoccupiedArea, left);
						break;
					}
					if (sortedPanel.DockAlignment == DockAlignment.Far)
					{
						LayoutAlignPanel(sortedPanel, bounds, ref unoccupiedArea, right);
						break;
					}
					if (flag)
					{
						arrayList.Add(sortedPanel);
						break;
					}
					LayoutCenteredPanels(arrayList, horizAlignment: true, bounds, ref unoccupiedArea2);
					arrayList.Clear();
					arrayList.Add(sortedPanel);
					LayoutCenteredPanels(arrayList, horizAlignment: true, bounds, ref unoccupiedArea2);
					arrayList.Clear();
					break;
				case PanelDockStyle.Bottom:
					if (sortedPanel.DockAlignment == DockAlignment.Near)
					{
						LayoutAlignPanel(sortedPanel, bounds, ref unoccupiedArea, left2);
						break;
					}
					if (sortedPanel.DockAlignment == DockAlignment.Far)
					{
						LayoutAlignPanel(sortedPanel, bounds, ref unoccupiedArea, right2);
						break;
					}
					if (flag)
					{
						arrayList2.Add(sortedPanel);
						break;
					}
					LayoutCenteredPanels(arrayList2, horizAlignment: true, bounds, ref unoccupiedArea2);
					arrayList2.Clear();
					arrayList2.Add(sortedPanel);
					LayoutCenteredPanels(arrayList2, horizAlignment: true, bounds, ref unoccupiedArea2);
					arrayList2.Clear();
					break;
				case PanelDockStyle.Left:
					if (sortedPanel.DockAlignment == DockAlignment.Near)
					{
						LayoutAlignPanel(sortedPanel, bounds, ref unoccupiedArea, top);
						break;
					}
					if (sortedPanel.DockAlignment == DockAlignment.Far)
					{
						LayoutAlignPanel(sortedPanel, bounds, ref unoccupiedArea, bottom);
						break;
					}
					if (!flag)
					{
						arrayList3.Add(sortedPanel);
						break;
					}
					LayoutCenteredPanels(arrayList3, horizAlignment: false, bounds, ref unoccupiedArea2);
					arrayList3.Clear();
					arrayList3.Add(sortedPanel);
					LayoutCenteredPanels(arrayList3, horizAlignment: false, bounds, ref unoccupiedArea2);
					arrayList3.Clear();
					break;
				case PanelDockStyle.Right:
					if (sortedPanel.DockAlignment == DockAlignment.Near)
					{
						LayoutAlignPanel(sortedPanel, bounds, ref unoccupiedArea, top2);
						break;
					}
					if (sortedPanel.DockAlignment == DockAlignment.Far)
					{
						LayoutAlignPanel(sortedPanel, bounds, ref unoccupiedArea, bottom2);
						break;
					}
					if (!flag)
					{
						arrayList4.Add(sortedPanel);
						break;
					}
					LayoutCenteredPanels(arrayList4, horizAlignment: false, bounds, ref unoccupiedArea2);
					arrayList4.Clear();
					arrayList4.Add(sortedPanel);
					LayoutCenteredPanels(arrayList4, horizAlignment: false, bounds, ref unoccupiedArea2);
					arrayList4.Clear();
					break;
				}
			}
			LayoutCenteredPanels(arrayList, horizAlignment: true, bounds, ref unoccupiedArea2);
			LayoutCenteredPanels(arrayList2, horizAlignment: true, bounds, ref unoccupiedArea2);
			LayoutCenteredPanels(arrayList3, horizAlignment: false, bounds, ref unoccupiedArea2);
			LayoutCenteredPanels(arrayList4, horizAlignment: false, bounds, ref unoccupiedArea2);
			return RectangleF.Intersect(unoccupiedArea, unoccupiedArea2);
		}

		private RectangleF LayoutAlignPanel(DockablePanel panel, RectangleF layoutBounds, ref RectangleF unoccupiedArea, float position)
		{
			bool flag = panel.Dock == PanelDockStyle.Top || panel.Dock == PanelDockStyle.Bottom;
			RectangleF result = MapGraphics.Round(panel.GetBoundsInPixels());
			switch (panel.Dock)
			{
			case PanelDockStyle.Top:
				result.Y = layoutBounds.Top;
				break;
			case PanelDockStyle.Bottom:
				result.Y = layoutBounds.Bottom - result.Height;
				break;
			case PanelDockStyle.Left:
				result.X = layoutBounds.Left;
				break;
			case PanelDockStyle.Right:
				result.X = layoutBounds.Right - result.Width;
				break;
			}
			switch (panel.DockAlignment)
			{
			case DockAlignment.Near:
			case DockAlignment.Center:
				if (flag)
				{
					result.X = position;
				}
				else
				{
					result.Y = position;
				}
				break;
			case DockAlignment.Far:
				if (flag)
				{
					result.X = position - result.Width;
				}
				else
				{
					result.Y = position - result.Height;
				}
				break;
			}
			panel.SetLocationInPixels(result.Location);
			switch (panel.Dock)
			{
			case PanelDockStyle.Top:
				if (result.Bottom > unoccupiedArea.Top)
				{
					unoccupiedArea.Height -= result.Bottom - unoccupiedArea.Top;
					unoccupiedArea.Y = result.Bottom;
				}
				break;
			case PanelDockStyle.Bottom:
				if (result.Top < unoccupiedArea.Bottom)
				{
					unoccupiedArea.Height -= unoccupiedArea.Bottom - result.Top;
				}
				break;
			case PanelDockStyle.Left:
				if (result.Right > unoccupiedArea.Left)
				{
					unoccupiedArea.Width -= result.Right - unoccupiedArea.Left;
					unoccupiedArea.X = result.Right;
				}
				break;
			case PanelDockStyle.Right:
				if (result.Left < unoccupiedArea.Right)
				{
					unoccupiedArea.Width -= unoccupiedArea.Right - result.Left;
				}
				break;
			}
			return result;
		}

		private void LayoutCenteredPanels(ArrayList panels, bool horizAlignment, RectangleF layoutBounds, ref RectangleF unoccupiedArea)
		{
			if (panels.Count == 0)
			{
				return;
			}
			float num = 0f;
			RectangleF layoutBounds2 = layoutBounds;
			foreach (Panel panel2 in panels)
			{
				RectangleF rectangleF = MapGraphics.Round(panel2.GetBoundsInPixels());
				num += (horizAlignment ? rectangleF.Width : rectangleF.Height);
			}
			float num2 = 0f;
			num2 = ((!horizAlignment) ? (layoutBounds2.Y + (layoutBounds2.Height - num) / 2f) : (layoutBounds2.X + (layoutBounds2.Width - num) / 2f));
			foreach (DockablePanel panel3 in panels)
			{
				num2 = (int)Math.Round(num2);
				LayoutAlignPanel(panel3, layoutBounds2, ref unoccupiedArea, num2);
			}
		}

		internal void LayoutPanels(MapGraphics g)
		{
			if (!DoPanelLayout)
			{
				return;
			}
			MapDockBounds = CalculateMapDockBounds(g);
			try
			{
				if (MapDockBounds.IsEmpty)
				{
					return;
				}
				List<DockablePanel> list = new List<DockablePanel>();
				List<DockablePanel> list2 = new List<DockablePanel>();
				Panel[] array = GetSortedPanels();
				foreach (Panel panel in array)
				{
					DockablePanel dockablePanel = panel as DockablePanel;
					if (dockablePanel == null || !dockablePanel.IsVisible())
					{
						continue;
					}
					if (dockablePanel.Dock != 0)
					{
						if (dockablePanel.DockedInsideViewport)
						{
							list.Add(dockablePanel);
						}
						else
						{
							list2.Add(dockablePanel);
						}
					}
					else if (panel is AutoSizePanel)
					{
						((AutoSizePanel)panel).AdjustAutoSize(g);
					}
				}
				RectangleF rectangleF = MapGraphics.Round(MapDockBounds);
				RectangleF viewportRect = PerformPanelLayout(g, rectangleF, list2);
				SeparateOverlappingPanels(list2, rectangleF, ref viewportRect);
				if (Viewport.AutoSize)
				{
					Viewport.SetBoundsInPixels(viewportRect);
				}
				RectangleF viewportRect2 = MapGraphics.Round(Viewport.GetBoundsInPixels());
				viewportRect2.X += Viewport.Margins.Left;
				viewportRect2.Y += Viewport.Margins.Top;
				viewportRect2.Width -= Viewport.Margins.Left + Viewport.Margins.Right;
				viewportRect2.Height -= Viewport.Margins.Top + Viewport.Margins.Bottom;
				viewportRect2.Inflate(-Viewport.BorderWidth, -Viewport.BorderWidth);
				PerformPanelLayout(g, viewportRect2, list);
				SeparateOverlappingPanels(list, viewportRect2, ref viewportRect2);
			}
			finally
			{
				DoPanelLayout = false;
			}
		}

		internal RectangleF CalculateMapDockBounds(MapGraphics g)
		{
			RectangleF empty = RectangleF.Empty;
			if (Frame.FrameStyle != 0)
			{
				empty = g.GetBorder3DAdjustedRect(Frame);
			}
			else if (BorderLineWidth <= 0)
			{
				empty = new RectangleF(0f, 0f, Width, Height);
			}
			else
			{
				empty = new RectangleF(0f, 0f, Width, Height);
				empty.Inflate(-BorderLineWidth, -BorderLineWidth);
			}
			return empty;
		}

		private void SeparateOverlappingPanels(List<DockablePanel> panels, RectangleF availableSpace, ref RectangleF viewportRect)
		{
			int num = 0;
			int num2 = panels.Count * panels.Count;
			Hashtable hashtable = new Hashtable();
			DockablePanel intersectingPanel;
			DockablePanel panelToMove;
			while (FindFirstOverlappingPanelPair(panels, hashtable, out intersectingPanel, out panelToMove) && num++ < num2)
			{
				if (!MovePanelToAvoidOverlap(panels, hashtable, panelToMove, intersectingPanel, availableSpace, ref viewportRect))
				{
					hashtable.Add(new PanelPair(intersectingPanel, panelToMove), null);
				}
			}
		}

		private bool FindFirstOverlappingPanelPair(List<DockablePanel> panels, Hashtable unavoidableOverlaps, out DockablePanel intersectingPanel, out DockablePanel panelToMove)
		{
			for (int i = 0; i < panels.Count - 1; i++)
			{
				RectangleF rectangleF = MapGraphics.Round(panels[i].GetBoundsInPixels());
				for (int j = i + 1; j < panels.Count; j++)
				{
					RectangleF rect = MapGraphics.Round(panels[j].GetBoundsInPixels());
					if (rectangleF.IntersectsWith(rect) && !unavoidableOverlaps.ContainsKey(new PanelPair(panels[i], panels[j])))
					{
						intersectingPanel = panels[i];
						panelToMove = panels[j];
						return true;
					}
				}
			}
			intersectingPanel = null;
			panelToMove = null;
			return false;
		}

		private bool MovePanelToAvoidOverlap(List<DockablePanel> panels, Hashtable unavoidableOverlaps, DockablePanel panelToMove, DockablePanel intersectingPanel, RectangleF availableSpace, ref RectangleF viewportRect)
		{
			RectangleF rectangleF = MapGraphics.Round(panelToMove.GetBoundsInPixels());
			RectangleF rectangleF2 = MapGraphics.Round(intersectingPanel.GetBoundsInPixels());
			RectangleF newRect = rectangleF;
			if (panelToMove.Dock == PanelDockStyle.Left || panelToMove.Dock == PanelDockStyle.Right)
			{
				newRect.Y = rectangleF2.Y + rectangleF2.Height;
				if (ValidateNewPosition(panels, unavoidableOverlaps, panelToMove, intersectingPanel, newRect, availableSpace, checkPreviousPanels: true))
				{
					SetNewPanelPosition(panelToMove, newRect, ref viewportRect);
					return true;
				}
				newRect = rectangleF;
				newRect.Y = rectangleF2.Y - rectangleF.Height;
				if (ValidateNewPosition(panels, unavoidableOverlaps, panelToMove, intersectingPanel, newRect, availableSpace, checkPreviousPanels: true))
				{
					SetNewPanelPosition(panelToMove, newRect, ref viewportRect);
					return true;
				}
				newRect = rectangleF;
				if (panelToMove.Dock == PanelDockStyle.Left)
				{
					newRect.X = rectangleF2.X + rectangleF2.Width;
				}
				else
				{
					newRect.X = rectangleF2.X - rectangleF.Width;
				}
				if (ValidateNewPosition(panels, unavoidableOverlaps, panelToMove, intersectingPanel, newRect, availableSpace, checkPreviousPanels: false))
				{
					SetNewPanelPosition(panelToMove, newRect, ref viewportRect);
					return true;
				}
			}
			else if (panelToMove.Dock == PanelDockStyle.Top || panelToMove.Dock == PanelDockStyle.Bottom)
			{
				newRect.X = rectangleF2.X + rectangleF2.Width;
				if (ValidateNewPosition(panels, unavoidableOverlaps, panelToMove, intersectingPanel, newRect, availableSpace, checkPreviousPanels: true))
				{
					SetNewPanelPosition(panelToMove, newRect, ref viewportRect);
					return true;
				}
				newRect = rectangleF;
				newRect.X = rectangleF2.X - rectangleF.Width;
				if (ValidateNewPosition(panels, unavoidableOverlaps, panelToMove, intersectingPanel, newRect, availableSpace, checkPreviousPanels: true))
				{
					SetNewPanelPosition(panelToMove, newRect, ref viewportRect);
					return true;
				}
				newRect = rectangleF;
				if (panelToMove.Dock == PanelDockStyle.Top)
				{
					newRect.Y = rectangleF2.Y + rectangleF2.Height;
				}
				else
				{
					newRect.Y = rectangleF2.Y - rectangleF.Height;
				}
				if (ValidateNewPosition(panels, unavoidableOverlaps, panelToMove, intersectingPanel, newRect, availableSpace, checkPreviousPanels: false))
				{
					SetNewPanelPosition(panelToMove, newRect, ref viewportRect);
					return true;
				}
			}
			return false;
		}

		private bool ValidateNewPosition(List<DockablePanel> panels, Hashtable unavoidableOverlaps, DockablePanel panelToMove, DockablePanel intersectingPanel, RectangleF newRect, RectangleF availableSpace, bool checkPreviousPanels)
		{
			if (!MapGraphics.Round(availableSpace).Contains(newRect))
			{
				return false;
			}
			if (checkPreviousPanels)
			{
				foreach (DockablePanel panel in panels)
				{
					if (panel == panelToMove)
					{
						return true;
					}
					if (!unavoidableOverlaps.ContainsKey(new PanelPair(panel, panelToMove)) && newRect.IntersectsWith(MapGraphics.Round(panel.GetBoundsInPixels())))
					{
						return false;
					}
				}
			}
			return true;
		}

		private void SetNewPanelPosition(DockablePanel panelToMove, RectangleF newRect, ref RectangleF viewportRect)
		{
			panelToMove.SetLocationInPixels(newRect.Location);
			RectangleF rectangleF = RectangleF.Intersect(newRect, viewportRect);
			if (panelToMove.Dock == PanelDockStyle.Left || panelToMove.Dock == PanelDockStyle.Right)
			{
				if (rectangleF.Width > 0f)
				{
					float num;
					if (panelToMove.Dock == PanelDockStyle.Left)
					{
						num = newRect.Right - viewportRect.Left;
						viewportRect.X += num;
					}
					else
					{
						num = viewportRect.Right - newRect.Left;
					}
					viewportRect.Width -= num;
					viewportRect.Width = Math.Max(viewportRect.Width, 0f);
				}
			}
			else if (rectangleF.Height > 0f)
			{
				float num2;
				if (panelToMove.Dock == PanelDockStyle.Top)
				{
					num2 = newRect.Bottom - viewportRect.Top;
					viewportRect.Y += num2;
				}
				else
				{
					num2 = viewportRect.Bottom - newRect.Top;
				}
				viewportRect.Height -= num2;
				viewportRect.Height = Math.Max(viewportRect.Height, 0f);
			}
		}

		internal void LoadShapesFromStream(Stream stream)
		{
			stream.Seek(0L, SeekOrigin.Begin);
			foreach (Shape shape in Shapes)
			{
				shape.ShapeData.LoadFromStream(stream);
			}
			InvalidateCachedBounds();
		}

		internal void SaveShapesToStream(Stream stream)
		{
			foreach (Shape shape in Shapes)
			{
				shape.ShapeData.SaveToStream(stream);
			}
		}

		internal void LoadPathsFromStream(Stream stream)
		{
			stream.Seek(0L, SeekOrigin.Begin);
			foreach (Path path in Paths)
			{
				path.PathData.LoadFromStream(stream);
			}
			InvalidateCachedBounds();
		}

		internal void SavePathsToStream(Stream stream)
		{
			foreach (Path path in Paths)
			{
				path.PathData.SaveToStream(stream);
			}
		}

		internal void LoadSymbolsFromStream(Stream stream)
		{
			stream.Seek(0L, SeekOrigin.Begin);
			foreach (Symbol symbol in Symbols)
			{
				symbol.SymbolData.LoadFromStream(stream);
			}
			InvalidateCachedBounds();
		}

		internal void SaveSymbolsToStream(Stream stream)
		{
			foreach (Symbol symbol in Symbols)
			{
				symbol.SymbolData.SaveToStream(stream);
			}
		}

		internal void CreateGroups(string shapeFieldName)
		{
			CreateGroups(shapeFieldName, string.Empty, string.Empty);
		}

		internal void CreateGroups(string shapeFieldName, string layer, string category)
		{
			Field field = (Field)ShapeFields.GetByName(shapeFieldName);
			if (field == null)
			{
				throw new ArgumentException(SR.ExceptionShapeFieldNotFound(shapeFieldName));
			}
			Hashtable hashtable = new Hashtable();
			foreach (Shape shape in Shapes)
			{
				object obj = shape[field.Name];
				if (obj != null)
				{
					string text = Field.ToStringInvariant(obj);
					if (!string.IsNullOrEmpty(text))
					{
						hashtable[text] = 0;
						shape.ParentGroup = text;
					}
				}
			}
			foreach (string key in hashtable.Keys)
			{
				if (Groups.GetByName(key) == null)
				{
					Group group = Groups.Add(key);
					group.Layer = layer;
					group.Category = category;
				}
			}
			InvalidateRules();
			InvalidateDataBinding();
			InvalidateCachedPaths();
			InvalidateCachedBounds();
			InvalidateChildSymbols();
			InvalidateDistanceScalePanel();
			InvalidateGridSections();
		}

		internal void InvalidateChildSymbols()
		{
			if (!resettingChildSymbols)
			{
				childSymbolsDirty = true;
			}
		}

		internal void ResetChildSymbols()
		{
			if (!childSymbolsDirty)
			{
				return;
			}
			resettingChildSymbols = true;
			childSymbolsDirty = false;
			foreach (Shape shape in Shapes)
			{
				shape.Symbols = null;
			}
			resettingChildSymbols = false;
		}

		internal void InvalidateGridSections()
		{
			if (!recreatingGridSections)
			{
				gridSectionsDirty = true;
			}
		}

		internal void DisposeGridSections()
		{
			if (GridSections == null)
			{
				return;
			}
			GridSection[,] array = GridSections;
			int upperBound = array.GetUpperBound(0);
			int upperBound2 = array.GetUpperBound(1);
			for (int i = array.GetLowerBound(0); i <= upperBound; i++)
			{
				for (int j = array.GetLowerBound(1); j <= upperBound2; j++)
				{
					array[i, j]?.Dispose();
				}
			}
			GridSections = null;
		}

		internal void RecreateGridSections()
		{
			if (gridSectionsDirty)
			{
				recreatingGridSections = true;
				gridSectionsDirty = false;
				DisposeGridSections();
				bufferedGridSectionCount = 0;
				DetermineGridSectionSizeAndCount();
				GridSections = new GridSection[GridSectionsXCount, GridSectionsYCount];
				recreatingGridSections = false;
			}
		}

		internal void DetermineGridSectionSizeAndCount()
		{
			SizeF absoluteSize = Viewport.GetAbsoluteSize();
			int val = (int)absoluteSize.Width / 160;
			val = Math.Max(val, 1);
			val = Math.Min(val, 4);
			int val2 = (int)Math.Ceiling(absoluteSize.Width / (float)val);
			val2 = Math.Max(val2, 1);
			int num = (int)Math.Ceiling(absoluteSize.Height / (float)val2);
			if (num == 0)
			{
				num = 1;
			}
			int val3 = (int)Math.Ceiling(absoluteSize.Height / (float)num);
			val3 = Math.Max(val3, 1);
			GridSectionSize = new Size(val2, val3);
			float num2 = Viewport.Zoom / 100f;
			SizeF contentSizeInPixels = Viewport.GetContentSizeInPixels();
			contentSizeInPixels.Width *= num2;
			contentSizeInPixels.Height *= num2;
			GridSectionsXCount = (int)Math.Ceiling(contentSizeInPixels.Width / (float)GridSectionSize.Width) + 2;
			GridSectionsYCount = (int)Math.Ceiling(contentSizeInPixels.Height / (float)GridSectionSize.Height) + 2;
			GridSectionsInViewportXCount = Math.Min(GridSectionsXCount, val);
			GridSectionsInViewportYCount = Math.Min(GridSectionsYCount, num);
			PointF contentOffsetInPixels = Viewport.GetContentOffsetInPixels();
			GridSectionsOffset = new Point(SimpleRound(contentOffsetInPixels.X % (float)GridSectionSize.Width), SimpleRound(contentOffsetInPixels.Y % (float)GridSectionSize.Height));
		}

		internal GridSection[] GetVisibleSections()
		{
			ArrayList arrayList = new ArrayList();
			PointF contentOffsetInPixels = Viewport.GetContentOffsetInPixels();
			contentOffsetInPixels.X -= GridSectionsOffset.X;
			contentOffsetInPixels.Y -= GridSectionsOffset.Y;
			PointF locationInPixels = Viewport.GetLocationInPixels();
			SizeF absoluteSize = Viewport.GetAbsoluteSize();
			int val = (int)Math.Floor((0f - contentOffsetInPixels.X) / (float)GridSectionSize.Width) + 1;
			val = Math.Max(0, val);
			val = Math.Min(GridSectionsXCount - 1, val);
			int val2 = (int)Math.Floor((0f - contentOffsetInPixels.Y) / (float)GridSectionSize.Height) + 1;
			val2 = Math.Max(0, val2);
			val2 = Math.Min(GridSectionsYCount - 1, val2);
			int val3 = (int)Math.Floor((0f - contentOffsetInPixels.X + absoluteSize.Width) / (float)GridSectionSize.Width) + 1;
			val3 = Math.Max(0, val3);
			val3 = Math.Min(GridSectionsXCount - 1, val3);
			int val4 = (int)Math.Floor((0f - contentOffsetInPixels.Y + absoluteSize.Height) / (float)GridSectionSize.Height) + 1;
			val4 = Math.Max(0, val4);
			val4 = Math.Min(GridSectionsYCount - 1, val4);
			for (int i = val; i <= val3; i++)
			{
				for (int j = val2; j <= val4; j++)
				{
					if (GridSections[i, j] == null)
					{
						GridSections[i, j] = new GridSection();
						GridSections[i, j].Origin.X = (i - 1) * GridSectionSize.Width - GridSectionsOffset.X + SimpleRound(locationInPixels.X);
						GridSections[i, j].Origin.Y = (j - 1) * GridSectionSize.Height - GridSectionsOffset.Y + SimpleRound(locationInPixels.Y);
					}
					arrayList.Add(GridSections[i, j]);
				}
			}
			return (GridSection[])arrayList.ToArray(typeof(GridSection));
		}

		private void InvalidateGridSections(Rectangle rectangle)
		{
			for (int i = 0; i < GridSectionsXCount; i++)
			{
				for (int j = 0; j < GridSectionsYCount; j++)
				{
					if (GridSections[i, j] != null)
					{
						ContentToPixels(GridSections[i, j].Origin);
						Rectangle rectangle2 = new Rectangle(GridSections[i, j].Origin.X, GridSections[i, j].Origin.Y, GridSectionSize.Width, GridSectionSize.Height);
						if (rectangle2.IntersectsWith(rectangle))
						{
							GridSections[i, j].Dirty = true;
						}
					}
				}
			}
		}

		internal void RenderOneGridSection(MapGraphics g, int xIndex, int yIndex)
		{
			HotRegionList.Clear();
			Common.InvokePrePaint(Viewport);
			GridSection gridSection = new GridSection();
			gridSection.Origin.X = (xIndex - 1) * GridSectionSize.Width - GridSectionsOffset.X;
			gridSection.Origin.Y = (yIndex - 1) * GridSectionSize.Height - GridSectionsOffset.Y;
			gridSection.HotRegions = new HotRegionList(this);
			PointF gridSectionOffset = ContentToPixels(gridSection.Origin);
			new Rectangle((int)gridSectionOffset.X, (int)gridSectionOffset.Y, GridSectionSize.Width, GridSectionSize.Height);
			Graphics graphics = g.Graphics;
			try
			{
				using (Brush brush = new SolidBrush(GetGridSectionBackColor()))
				{
					g.Graphics.FillRectangle(brush, -1, -1, GridSectionSize.Width + 1, GridSectionSize.Height + 1);
				}
				RectangleF rect = new RectangleF(0f, 0f, GridSectionSize.Width, GridSectionSize.Height);
				g.Graphics.SetClip(rect, CombineMode.Replace);
				RenderContentElements(g, gridSectionOffset, gridSection.HotRegions);
				if (!GridUnderContent)
				{
					RenderGrid(g, gridSectionOffset);
				}
			}
			finally
			{
				g.Graphics = graphics;
			}
			foreach (HotRegion item in gridSection.HotRegions.List)
			{
				if (HotRegionList.FindHotRegionOfObject(item.SelectedObject) == -1)
				{
					item.DoNotDispose = true;
					HotRegionList.List.Add(item);
				}
			}
			Common.InvokePostPaint(Viewport);
		}

		internal static int SimpleRound(float number)
		{
			return (int)Math.Round(number);
		}

		internal void RenderGridSections(MapGraphics g)
		{
			RecreateGridSections();
			GridSection[] visibleSections = GetVisibleSections();
			for (int i = 0; i < visibleSections.Length; i++)
			{
				PointF pointF = ContentToPixels(visibleSections[i].Origin);
				Rectangle rect = new Rectangle(SimpleRound(pointF.X), SimpleRound(pointF.Y), GridSectionSize.Width, GridSectionSize.Height);
				if (visibleSections[i].Dirty)
				{
					visibleSections[i].Dispose();
					visibleSections[i].Dirty = false;
					bufferedGridSectionCount--;
				}
				if (visibleSections[i].Bitmap == null)
				{
					bufferedGridSectionCount++;
					visibleSections[i].Bitmap = new BufferBitmap();
					visibleSections[i].Bitmap.Size = GridSectionSize;
					visibleSections[i].Bitmap.Graphics.SmoothingMode = GetSmootingMode();
					visibleSections[i].Bitmap.Graphics.TextRenderingHint = GetTextRenderingHint();
					visibleSections[i].Bitmap.Graphics.TextContrast = 2;
					visibleSections[i].HotRegions = new HotRegionList(this);
					Graphics graphics = g.Graphics;
					try
					{
						g.Graphics = visibleSections[i].Bitmap.Graphics;
						using (Brush brush = new SolidBrush(GetGridSectionBackColor()))
						{
							g.Graphics.FillRectangle(brush, -1, -1, GridSectionSize.Width + 1, GridSectionSize.Height + 1);
						}
						RectangleF rect2 = new RectangleF(0f, 0f, GridSectionSize.Width, GridSectionSize.Height);
						g.Graphics.SetClip(rect2, CombineMode.Intersect);
						RenderContentElements(g, pointF, visibleSections[i].HotRegions);
					}
					finally
					{
						g.Graphics = graphics;
					}
				}
				foreach (HotRegion item in visibleSections[i].HotRegions.List)
				{
					if (HotRegionList.FindHotRegionOfObject(item.SelectedObject) == -1)
					{
						item.DoNotDispose = true;
						item.OffsetBy(pointF);
						HotRegionList.List.Add(item);
					}
				}
				AntiAliasing antiAliasing = g.AntiAliasing;
				g.AntiAliasing = AntiAliasing.None;
				g.Graphics.DrawImageUnscaledAndClipped(visibleSections[i].Bitmap.Bitmap, rect);
				g.AntiAliasing = antiAliasing;
			}
		}

		internal Color GetGridSectionBackColor()
		{
			if (Viewport.BackColor.A == 0)
			{
				return Color.FromArgb(255, MapControl.BackColor);
			}
			return Viewport.BackColor;
		}

		internal bool UseGridSectionRendering()
		{
			if (Viewport.OptimizeForPanning)
			{
				if (!IsDesignMode() && Viewport.EnablePanning)
				{
					return !IsTileLayerVisible();
				}
				return false;
			}
			return false;
		}

		internal bool IsTileLayerVisible()
		{
			foreach (Layer layer in Layers)
			{
				if (layer.TileSystem != 0 && layer.Visible)
				{
					return true;
				}
			}
			return false;
		}

		internal ArrayList Find(string searchFor, bool ignoreCase, bool exactSearch)
		{
			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(Shapes.Find(searchFor, ignoreCase, exactSearch, uniqueOnlyFields: true));
			arrayList.AddRange(Groups.Find(searchFor, ignoreCase, exactSearch, uniqueOnlyFields: true));
			arrayList.AddRange(Symbols.Find(searchFor, ignoreCase, exactSearch, uniqueOnlyFields: true));
			arrayList.AddRange(Paths.Find(searchFor, ignoreCase, exactSearch, uniqueOnlyFields: true));
			return arrayList;
		}

		internal void SuspendUpdates()
		{
			suspendUpdatesCount++;
			disableInvalidate = true;
			if (suspendUpdatesCount == 1)
			{
				NamedCollection[] renderCollections = GetRenderCollections();
				for (int i = 0; i < renderCollections.Length; i++)
				{
					renderCollections[i].SuspendUpdates();
				}
			}
		}

		internal void ResumeUpdates()
		{
			if (suspendUpdatesCount > 0)
			{
				suspendUpdatesCount--;
			}
			if (suspendUpdatesCount == 0)
			{
				disableInvalidate = false;
				NamedCollection[] renderCollections = GetRenderCollections();
				for (int i = 0; i < renderCollections.Length; i++)
				{
					renderCollections[i].ResumeUpdates();
				}
			}
		}
	}
}

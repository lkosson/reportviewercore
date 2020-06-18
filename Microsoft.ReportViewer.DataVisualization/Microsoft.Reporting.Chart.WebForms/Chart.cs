using Microsoft.Reporting.Chart.WebForms.Borders3D;
using Microsoft.Reporting.Chart.WebForms.ChartTypes;
using Microsoft.Reporting.Chart.WebForms.Data;
using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Formulas;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Microsoft.Reporting.Chart.WebForms
{
	[LicenseProvider(typeof(LicFileLicenseProvider))]
	[DisplayName("Dundas Chart Enterprise")]
	internal class Chart : IDisposable, IChart
	{
		private float imageResolution = 96f;

		private string multiValueSeparator = "\\#\\";

		private Title noDataMessage = new Title("*****  No Data  *****");

		private bool reverseSeriesOrder;

		private bool suppressCodeExceptions = true;

		private string codeException = "";

		internal bool ShowDebugMarkings;

		private ChartTypeRegistry chartTypeRegistry;

		private BorderTypeRegistry borderTypeRegistry;

		private CustomAttributeRegistry customAttributeRegistry;

		private DataManager dataManager;

		internal ChartImage chartPicture;

		private ImageLoader imageLoader;

		internal static ITypeDescriptorContext controlCurrentContext = null;

		internal string webFormDocumentURL = "";

		internal ServiceContainer serviceContainer;

		private EventsManager eventsManager;

		private TraceManager traceManager;

		private NamedImagesCollection namedImages;

		private FormulaRegistry formulaRegistry;

		internal static string productID = "DC-WCE-42";

		private License license;

		private RenderType renderType;

		private string chartImageUrl = "ChartPic_#SEQ(300,3)";

		internal bool serializing;

		internal SerializationStatus serializationStatus;

		private ChartSerializer chartSerializer;

		private string windowsFormsControlURL = string.Empty;

		private string currentChartImageUrl = string.Empty;

		private KeywordsRegistry keywordsRegistry;

		internal static double renderingDpiX = 96.0;

		internal static double renderingDpiY = 96.0;

		private string lastUpdatedDesignTimeHtmlValue = string.Empty;

		public LocalizeTextHandler LocalizeTextHandler;

		public FormatNumberHandler FormatNumberHandler;

		public float ImageResolution
		{
			get
			{
				return imageResolution;
			}
			set
			{
				imageResolution = value;
			}
		}

		[SRDescription("DescriptionAttributeMultiValueSeparator")]
		public string MultiValueSeparator
		{
			get
			{
				return multiValueSeparator;
			}
			set
			{
				multiValueSeparator = value;
			}
		}

		[SRDescription("DescriptionAttributeNoDataMessage")]
		public Title NoDataMessage
		{
			get
			{
				if (noDataMessage != null && noDataMessage.Text == string.Empty)
				{
					noDataMessage.Text = "*****  No Data  *****";
				}
				return noDataMessage;
			}
			set
			{
				noDataMessage = value;
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeReverseSeriesOrder")]
		public bool ReverseSeriesOrder
		{
			get
			{
				return reverseSeriesOrder;
			}
			set
			{
				reverseSeriesOrder = value;
			}
		}

		[SRCategory("CategoryAttributeChart")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeSuppressCodeExceptions")]
		[RefreshProperties(RefreshProperties.All)]
		public bool SuppressCodeExceptions
		{
			get
			{
				return suppressCodeExceptions;
			}
			set
			{
				suppressCodeExceptions = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string CodeException
		{
			get
			{
				return codeException;
			}
			set
			{
				codeException = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(96.0)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public double RenderingDpiY
		{
			get
			{
				return renderingDpiY;
			}
			set
			{
				renderingDpiY = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(96.0)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public double RenderingDpiX
		{
			get
			{
				return renderingDpiX;
			}
			set
			{
				renderingDpiX = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeSuppressExceptions")]
		public bool SuppressExceptions
		{
			get
			{
				return chartPicture.SuppressExceptions;
			}
			set
			{
				chartPicture.SuppressExceptions = value;
			}
		}

		[SRCategory("CategoryAttributeChart")]
		[Bindable(false)]
		[SRDescription("DescriptionAttributeChart_Images")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public NamedImagesCollection Images => namedImages;

		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeChart_RenderType")]
		[DefaultValue(RenderType.ImageTag)]
		internal RenderType RenderType
		{
			get
			{
				return renderType;
			}
			set
			{
				renderType = value;
				if (renderType == RenderType.ImageMap && !MapEnabled)
				{
					MapEnabled = true;
				}
			}
		}

		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeChart_ImageUrl")]
		[DefaultValue("ChartPic_#SEQ(300,3)")]
		public string ImageUrl
		{
			get
			{
				return chartImageUrl;
			}
			set
			{
				if (value.IndexOf("#SEQ", StringComparison.Ordinal) > 0)
				{
					CheckImageURLSeqFormat(value);
				}
				chartImageUrl = value;
			}
		}

		[Category("Appearance")]
		[SRDescription("DescriptionAttribute_RightToLeft")]
		[DefaultValue(RightToLeft.No)]
		public RightToLeft RightToLeft
		{
			get
			{
				return chartPicture.RightToLeft;
			}
			set
			{
				chartPicture.RightToLeft = value;
			}
		}

		[SRCategory("CategoryAttributeChart")]
		[SRDescription("DescriptionAttributeChart_Series")]
		public SeriesCollection Series => dataManager.Series;

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributePalette")]
		[DefaultValue(ChartColorPalette.BrightPastel)]
		public ChartColorPalette Palette
		{
			get
			{
				return dataManager.Palette;
			}
			set
			{
				dataManager.Palette = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SerializationVisibility(SerializationVisibility.Attribute)]
		[SRDescription("DescriptionAttributeChart_PaletteCustomColors")]
		[TypeConverter(typeof(ColorArrayConverter))]
		public Color[] PaletteCustomColors
		{
			get
			{
				return dataManager.PaletteCustomColors;
			}
			set
			{
				dataManager.PaletteCustomColors = value;
			}
		}

		[SRDescription("DescriptionAttributeChart_BuildNumber")]
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
					text = executingAssembly.FullName.ToUpper(CultureInfo.InvariantCulture);
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
			}
		}

		[SRCategory("CategoryAttributeSerializer")]
		[SRDescription("DescriptionAttributeChart_Serializer")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		internal ChartSerializer Serializer => chartSerializer;

		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[DefaultValue(ChartImageType.Png)]
		[SRDescription("DescriptionAttributeChartImageType")]
		[RefreshProperties(RefreshProperties.All)]
		public ChartImageType ImageType
		{
			get
			{
				return chartPicture.ImageType;
			}
			set
			{
				chartPicture.ImageType = value;
			}
		}

		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeChart_Compression")]
		internal int Compression
		{
			get
			{
				return chartPicture.Compression;
			}
			set
			{
				chartPicture.Compression = value;
			}
		}

		[SRCategory("CategoryAttributeMap")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMapEnabled")]
		[DefaultValue(true)]
		internal bool MapEnabled
		{
			get
			{
				return chartPicture.MapEnabled;
			}
			set
			{
				chartPicture.MapEnabled = value;
			}
		}

		[SRCategory("CategoryAttributeMap")]
		[SRDescription("DescriptionAttributeMapAreas")]
		public MapAreasCollection MapAreas => chartPicture.MapAreas;

		[SRCategory("CategoryAttributeImage")]
		[Browsable(false)]
		[Bindable(false)]
		[DefaultValue(typeof(AntiAlias), "On")]
		[SRDescription("DescriptionAttributeAntiAlias")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public AntiAlias AntiAlias
		{
			get
			{
				return chartPicture.AntiAlias;
			}
			set
			{
				chartPicture.AntiAlias = value;
			}
		}

		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[DefaultValue(typeof(AntiAliasingTypes), "All")]
		[SRDescription("DescriptionAttributeAntiAlias")]
		public AntiAliasingTypes AntiAliasing
		{
			get
			{
				return chartPicture.AntiAliasing;
			}
			set
			{
				chartPicture.AntiAliasing = value;
			}
		}

		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[DefaultValue(typeof(TextAntiAliasingQuality), "High")]
		[SRDescription("DescriptionAttributeTextAntiAliasingQuality")]
		public TextAntiAliasingQuality TextAntiAliasingQuality
		{
			get
			{
				return chartPicture.TextAntiAliasingQuality;
			}
			set
			{
				chartPicture.TextAntiAliasingQuality = value;
			}
		}

		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeChart_SoftShadows")]
		public bool SoftShadows
		{
			get
			{
				return chartPicture.SoftShadows;
			}
			set
			{
				chartPicture.SoftShadows = value;
			}
		}

		[SRCategory("CategoryAttributeChart")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeChartAreas")]
		public ChartAreaCollection ChartAreas => chartPicture.ChartAreas;

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "White")]
		[SRDescription("DescriptionAttributeBackColor5")]
		public Color BackColor
		{
			get
			{
				return chartPicture.BackColor;
			}
			set
			{
				chartPicture.BackColor = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(false)]
		[Browsable(false)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeChart_ForeColor")]
		public Color ForeColor
		{
			get
			{
				return Color.Empty;
			}
			set
			{
			}
		}

		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[DefaultValue(300)]
		[SRDescription("DescriptionAttributeHeight3")]
		public int Height
		{
			get
			{
				return chartPicture.Height;
			}
			set
			{
				chartPicture.Height = value;
			}
		}

		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[DefaultValue(300)]
		[SRDescription("DescriptionAttributeWidth")]
		public int Width
		{
			get
			{
				return chartPicture.Width;
			}
			set
			{
				chartPicture.Width = value;
			}
		}

		[SRCategory("CategoryAttributeChart")]
		[Bindable(false)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeLegend")]
		[NotifyParentProperty(true)]
		[TypeConverter(typeof(LegendConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DefaultValue(null)]
		public Legend Legend
		{
			get
			{
				if (serializing)
				{
					return null;
				}
				return chartPicture.Legend;
			}
			set
			{
				chartPicture.Legend = value;
			}
		}

		[SRCategory("CategoryAttributeChart")]
		[SRDescription("DescriptionAttributeLegends")]
		public LegendCollection Legends => chartPicture.Legends;

		[SRCategory("CategoryAttributeChart")]
		[SRDescription("DescriptionAttributeTitles")]
		public TitleCollection Titles => chartPicture.Titles;

		[SRCategory("CategoryAttributeChart")]
		[SRDescription("DescriptionAttributeAnnotations3")]
		public AnnotationCollection Annotations => chartPicture.Annotations;

		[SRCategory("CategoryAttributeData")]
		[SRDescription("DescriptionAttributeDataManipulator")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public DataManipulator DataManipulator => chartPicture.DataManipulator;

		[SRCategory("CategoryAttributeCharttitle")]
		[Bindable(false)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeTitle5")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Title
		{
			get
			{
				return chartPicture.Title;
			}
			set
			{
				chartPicture.Title = value;
			}
		}

		[SRCategory("CategoryAttributeCharttitle")]
		[Bindable(false)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeTitleFontColor")]
		public Color TitleFontColor
		{
			get
			{
				return chartPicture.TitleFontColor;
			}
			set
			{
				chartPicture.TitleFontColor = value;
			}
		}

		[SRCategory("CategoryAttributeCharttitle")]
		[Bindable(false)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeTitleFont4")]
		public Font TitleFont
		{
			get
			{
				return chartPicture.TitleFont;
			}
			set
			{
				chartPicture.TitleFont = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartHatchStyle.None)]
		[SRDescription("DescriptionAttributeBackHatchStyle9")]
		public ChartHatchStyle BackHatchStyle
		{
			get
			{
				return chartPicture.BackHatchStyle;
			}
			set
			{
				chartPicture.BackHatchStyle = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeBackImage3")]
		[NotifyParentProperty(true)]
		public string BackImage
		{
			get
			{
				return chartPicture.BackImage;
			}
			set
			{
				chartPicture.BackImage = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartImageWrapMode.Tile)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackImageMode3")]
		public ChartImageWrapMode BackImageMode
		{
			get
			{
				return chartPicture.BackImageMode;
			}
			set
			{
				chartPicture.BackImageMode = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackImageTransparentColor6")]
		public Color BackImageTransparentColor
		{
			get
			{
				return chartPicture.BackImageTransparentColor;
			}
			set
			{
				chartPicture.BackImageTransparentColor = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartImageAlign.TopLeft)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackImageAlign")]
		public ChartImageAlign BackImageAlign
		{
			get
			{
				return chartPicture.BackImageAlign;
			}
			set
			{
				chartPicture.BackImageAlign = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(GradientType.None)]
		[SRDescription("DescriptionAttributeBackGradientType3")]
		public GradientType BackGradientType
		{
			get
			{
				return chartPicture.BackGradientType;
			}
			set
			{
				chartPicture.BackGradientType = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeBackGradientEndColor4")]
		public Color BackGradientEndColor
		{
			get
			{
				return chartPicture.BackGradientEndColor;
			}
			set
			{
				chartPicture.BackGradientEndColor = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(false)]
		[Browsable(false)]
		[DefaultValue(typeof(Color), "White")]
		[SRDescription("DescriptionAttributeBorderColor")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Color BorderColor
		{
			get
			{
				return chartPicture.BorderColor;
			}
			set
			{
				chartPicture.BorderColor = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(false)]
		[Browsable(false)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeChart_BorderlineWidth")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public int BorderWidth
		{
			get
			{
				return chartPicture.BorderWidth;
			}
			set
			{
				chartPicture.BorderWidth = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(false)]
		[Browsable(false)]
		[DefaultValue(ChartDashStyle.NotSet)]
		[SRDescription("DescriptionAttributeBorderStyle8")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public ChartDashStyle BorderStyle
		{
			get
			{
				return chartPicture.BorderStyle;
			}
			set
			{
				chartPicture.BorderStyle = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "White")]
		[SRDescription("DescriptionAttributeBorderColor")]
		public Color BorderlineColor
		{
			get
			{
				return chartPicture.BorderColor;
			}
			set
			{
				chartPicture.BorderColor = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeChart_BorderlineWidth")]
		public int BorderlineWidth
		{
			get
			{
				return chartPicture.BorderWidth;
			}
			set
			{
				chartPicture.BorderWidth = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartDashStyle.NotSet)]
		[SRDescription("DescriptionAttributeBorderStyle8")]
		public ChartDashStyle BorderlineStyle
		{
			get
			{
				return chartPicture.BorderStyle;
			}
			set
			{
				chartPicture.BorderStyle = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(BorderSkinStyle.None)]
		[SRDescription("DescriptionAttributeBorderSkinAttributes")]
		[NotifyParentProperty(true)]
		[TypeConverter(typeof(LegendConverter))]
		public BorderSkinAttributes BorderSkin
		{
			get
			{
				return chartPicture.BorderSkinAttributes;
			}
			set
			{
				chartPicture.BorderSkinAttributes = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(false)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeChart_Edition")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public ChartEdition Edition => ChartEdition.Enterprise;

		[SRDescription("DescriptionAttributeChartEvent_PrePaint")]
		internal event PaintEventHandler PrePaint;

		[SRDescription("DescriptionAttributeChartEvent_PostPaint")]
		internal event PaintEventHandler PostPaint;

		[SRDescription("DescriptionAttributeChartEvent_BackPaint")]
		[Browsable(false)]
		internal event PaintEventHandler BackPaint;

		[SRDescription("DescriptionAttributeChartEvent_Paint")]
		[Browsable(false)]
		internal event PaintEventHandler Paint;

		[SRDescription("DescriptionAttributeChartEvent_CustomizeMapAreas")]
		internal event CustomizeMapAreasEventHandler CustomizeMapAreas;

		[SRDescription("DescriptionAttributeChartEvent_Customize")]
		internal event CustomizeEventHandler Customize;

		[SRDescription("DescriptionAttributeChartEvent_CustomizeLegend")]
		internal event CustomizeLegendEventHandler CustomizeLegend;

		public Chart()
		{
			serviceContainer = new ServiceContainer();
			eventsManager = new EventsManager(serviceContainer);
			traceManager = new TraceManager(serviceContainer);
			chartTypeRegistry = new ChartTypeRegistry(serviceContainer);
			borderTypeRegistry = new BorderTypeRegistry(serviceContainer);
			customAttributeRegistry = new CustomAttributeRegistry(serviceContainer);
			keywordsRegistry = new KeywordsRegistry(serviceContainer);
			dataManager = new DataManager(serviceContainer);
			imageLoader = new ImageLoader(serviceContainer);
			chartPicture = new ChartImage(serviceContainer);
			chartSerializer = new ChartSerializer(serviceContainer);
			formulaRegistry = new FormulaRegistry(serviceContainer);
			serviceContainer.AddService(typeof(Chart), this);
			serviceContainer.AddService(eventsManager.GetType(), eventsManager);
			serviceContainer.AddService(traceManager.GetType(), traceManager);
			serviceContainer.AddService(chartTypeRegistry.GetType(), chartTypeRegistry);
			serviceContainer.AddService(borderTypeRegistry.GetType(), borderTypeRegistry);
			serviceContainer.AddService(customAttributeRegistry.GetType(), customAttributeRegistry);
			serviceContainer.AddService(dataManager.GetType(), dataManager);
			serviceContainer.AddService(imageLoader.GetType(), imageLoader);
			serviceContainer.AddService(chartPicture.GetType(), chartPicture);
			serviceContainer.AddService(chartSerializer.GetType(), chartSerializer);
			serviceContainer.AddService(formulaRegistry.GetType(), formulaRegistry);
			serviceContainer.AddService(keywordsRegistry.GetType(), keywordsRegistry);
			dataManager.Initialize();
			chartTypeRegistry.Register("Bar", typeof(BarChart));
			chartTypeRegistry.Register("Column", typeof(ColumnChart));
			chartTypeRegistry.Register("Point", typeof(PointChart));
			chartTypeRegistry.Register("Bubble", typeof(BubbleChart));
			chartTypeRegistry.Register("Line", typeof(LineChart));
			chartTypeRegistry.Register("Spline", typeof(SplineChart));
			chartTypeRegistry.Register("StepLine", typeof(StepLineChart));
			chartTypeRegistry.Register("Area", typeof(AreaChart));
			chartTypeRegistry.Register("SplineArea", typeof(SplineAreaChart));
			chartTypeRegistry.Register("StackedArea", typeof(StackedAreaChart));
			chartTypeRegistry.Register("Pie", typeof(PieChart));
			chartTypeRegistry.Register("Stock", typeof(StockChart));
			chartTypeRegistry.Register("Candlestick", typeof(CandleStickChart));
			chartTypeRegistry.Register("Doughnut", typeof(DoughnutChart));
			chartTypeRegistry.Register("StackedBar", typeof(StackedBarChart));
			chartTypeRegistry.Register("StackedColumn", typeof(StackedColumnChart));
			chartTypeRegistry.Register("100%StackedColumn", typeof(HundredPercentStackedColumnChart));
			chartTypeRegistry.Register("100%StackedBar", typeof(HundredPercentStackedBarChart));
			chartTypeRegistry.Register("100%StackedArea", typeof(HundredPercentStackedAreaChart));
			chartTypeRegistry.Register("Range", typeof(RangeChart));
			chartTypeRegistry.Register("SplineRange", typeof(SplineRangeChart));
			chartTypeRegistry.Register("Gantt", typeof(GanttChart));
			chartTypeRegistry.Register("RangeColumn", typeof(RangeColumnChart));
			chartTypeRegistry.Register("ErrorBar", typeof(ErrorBarChart));
			chartTypeRegistry.Register("BoxPlot", typeof(BoxPlotChart));
			chartTypeRegistry.Register("Radar", typeof(RadarChart));
			chartTypeRegistry.Register("Renko", typeof(RenkoChart));
			chartTypeRegistry.Register("ThreeLineBreak", typeof(ThreeLineBreakChart));
			chartTypeRegistry.Register("Kagi", typeof(KagiChart));
			chartTypeRegistry.Register("PointAndFigure", typeof(PointAndFigureChart));
			chartTypeRegistry.Register("Polar", typeof(PolarChart));
			chartTypeRegistry.Register("FastLine", typeof(FastLineChart));
			chartTypeRegistry.Register("Funnel", typeof(FunnelChart));
			chartTypeRegistry.Register("Pyramid", typeof(PyramidChart));
			chartTypeRegistry.Register("FastPoint", typeof(FastPointChart));
			chartTypeRegistry.Register("TreeMap", typeof(TreeMapChart));
			chartTypeRegistry.Register("Sunburst", typeof(SunburstChart));
			formulaRegistry.Register(SR.FormulaNamePriceIndicators, typeof(PriceIndicators));
			formulaRegistry.Register(SR.FormulaNameGeneralTechnicalIndicators, typeof(GeneralTechnicalIndicators));
			formulaRegistry.Register(SR.FormulaNameTechnicalVolumeIndicators, typeof(VolumeIndicators));
			formulaRegistry.Register(SR.FormulaNameOscillator, typeof(Oscillators));
			formulaRegistry.Register(SR.FormulaNameGeneralFormulas, typeof(GeneralFormulas));
			formulaRegistry.Register(SR.FormulaNameTimeSeriesAndForecasting, typeof(TimeSeriesAndForecasting));
			formulaRegistry.Register(SR.FormulaNameStatisticalAnalysis, typeof(StatisticalAnalysis));
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
			namedImages = new NamedImagesCollection(this);
			chartPicture.Initialize(this);
		}

		~Chart()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (imageLoader != null)
			{
				imageLoader.Dispose();
			}
			if (license != null)
			{
				license.Dispose();
				license = null;
			}
			if (serviceContainer != null)
			{
				serviceContainer.Dispose();
			}
			GC.SuppressFinalize(this);
		}

		private void CheckImageURLSeqFormat(string imageURL)
		{
			int num = imageURL.IndexOf("#SEQ", StringComparison.Ordinal);
			num += 4;
			if (imageURL[num] != '(')
			{
				throw new ArgumentException(SR.ExceptionImageUrlInvalidFormat, "imageURL");
			}
			int num2 = imageURL.IndexOf(')', 1);
			if (num2 < 0)
			{
				throw new ArgumentException(SR.ExceptionImageUrlInvalidFormat, "imageURL");
			}
			string[] array = imageURL.Substring(num + 1, num2 - num - 1).Split(',');
			if (array == null || array.Length != 2)
			{
				throw new ArgumentException(SR.ExceptionImageUrlInvalidFormat, "imageURL");
			}
			string[] array2 = array;
			foreach (string text in array2)
			{
				for (int j = 0; j < text.Length; j++)
				{
					if (!char.IsDigit(text[j]))
					{
						throw new ArgumentException(SR.ExceptionImageUrlInvalidFormat, "imageURL");
					}
				}
			}
		}

		private string GetNewSeqImageUrl(string imageUrl)
		{
			int num = 0;
			string text = "";
			int num2 = imageUrl.IndexOf("#SEQ", StringComparison.Ordinal);
			if (num2 < 0)
			{
				throw new ArgumentException(SR.ExceptionImageUrlMissedFormatter, "imageUrl");
			}
			CheckImageURLSeqFormat(imageUrl);
			text = imageUrl.Substring(0, num2);
			num2 += 4;
			int num3 = imageUrl.IndexOf(')', 1);
			text += "{0:D6}";
			text += imageUrl.Substring(num3 + 1);
			string[] array = imageUrl.Substring(num2 + 1, num3 - num2 - 1).Split(',');
			int.Parse(array[0], CultureInfo.InvariantCulture);
			num = int.Parse(array[1], CultureInfo.InvariantCulture);
			int num4 = 1;
			text = string.Format(CultureInfo.InvariantCulture, text, num4);
			if (num > 0)
			{
				CheckChartFileTime(text, num);
			}
			return text;
		}

		private void CheckChartFileTime(string fileName, int imageTimeToLive)
		{
		}

		public void Select(int x, int y, out string series, out int point)
		{
			chartPicture.Select(x, y, out series, out point);
		}

		public HitTestResult HitTest(int x, int y)
		{
			chartPicture.Select(x, y, ChartElementType.Nothing, ignoreTransparent: false, out string series, out int point, out ChartElementType type, out object obj);
			return GetHitTestResult(series, point, type, obj);
		}

		public HitTestResult HitTest(int x, int y, bool ignoreTransparent)
		{
			chartPicture.Select(x, y, ChartElementType.Nothing, ignoreTransparent, out string series, out int point, out ChartElementType type, out object obj);
			return GetHitTestResult(series, point, type, obj);
		}

		public HitTestResult HitTest(int x, int y, ChartElementType requestedElement)
		{
			chartPicture.Select(x, y, requestedElement, ignoreTransparent: false, out string series, out int point, out ChartElementType type, out object obj);
			return GetHitTestResult(series, point, type, obj);
		}

		internal HitTestResult GetHitTestResult(string seriesName, int pointIndex, ChartElementType type, object obj)
		{
			HitTestResult hitTestResult = new HitTestResult();
			if (seriesName.Length > 0)
			{
				hitTestResult.Series = Series[seriesName];
			}
			hitTestResult.Object = obj;
			hitTestResult.PointIndex = pointIndex;
			hitTestResult.ChartElementType = type;
			switch (type)
			{
			case ChartElementType.Axis:
			{
				Axis axis2 = hitTestResult.Axis = (Axis)obj;
				if (axis2 != null)
				{
					hitTestResult.ChartArea = axis2.chartArea;
				}
				break;
			}
			case ChartElementType.DataPoint:
			{
				DataPoint dataPoint = Series[seriesName].Points[pointIndex];
				hitTestResult.Axis = null;
				hitTestResult.ChartArea = ChartAreas[dataPoint.series.ChartArea];
				break;
			}
			case ChartElementType.Gridlines:
			{
				Grid grid = (Grid)obj;
				hitTestResult.Axis = grid.axis;
				if (grid.axis != null)
				{
					hitTestResult.ChartArea = grid.axis.chartArea;
				}
				break;
			}
			case ChartElementType.LegendArea:
				hitTestResult.Axis = null;
				hitTestResult.ChartArea = null;
				break;
			case ChartElementType.LegendItem:
				hitTestResult.Axis = null;
				hitTestResult.ChartArea = null;
				break;
			case ChartElementType.PlottingArea:
			{
				ChartArea chartArea = (ChartArea)obj;
				hitTestResult.Axis = null;
				hitTestResult.ChartArea = chartArea;
				break;
			}
			case ChartElementType.StripLines:
			{
				StripLine stripLine = (StripLine)obj;
				hitTestResult.Axis = stripLine.axis;
				if (stripLine.axis != null)
				{
					hitTestResult.ChartArea = stripLine.axis.chartArea;
				}
				break;
			}
			case ChartElementType.TickMarks:
			{
				TickMark tickMark = (TickMark)obj;
				hitTestResult.Axis = tickMark.axis;
				if (tickMark.axis != null)
				{
					hitTestResult.ChartArea = tickMark.axis.chartArea;
				}
				break;
			}
			case ChartElementType.Title:
				hitTestResult.Axis = null;
				hitTestResult.ChartArea = null;
				break;
			case ChartElementType.SBLargeDecrement:
			{
				AxisScrollBar axisScrollBar = (AxisScrollBar)obj;
				hitTestResult.Axis = axisScrollBar.axis;
				if (axisScrollBar.axis != null)
				{
					hitTestResult.ChartArea = axisScrollBar.axis.chartArea;
				}
				break;
			}
			case ChartElementType.SBLargeIncrement:
			{
				AxisScrollBar axisScrollBar = (AxisScrollBar)obj;
				hitTestResult.Axis = axisScrollBar.axis;
				if (axisScrollBar.axis != null)
				{
					hitTestResult.ChartArea = axisScrollBar.axis.chartArea;
				}
				break;
			}
			case ChartElementType.SBSmallDecrement:
			{
				AxisScrollBar axisScrollBar = (AxisScrollBar)obj;
				hitTestResult.Axis = axisScrollBar.axis;
				if (axisScrollBar.axis != null)
				{
					hitTestResult.ChartArea = axisScrollBar.axis.chartArea;
				}
				break;
			}
			case ChartElementType.SBSmallIncrement:
			{
				AxisScrollBar axisScrollBar = (AxisScrollBar)obj;
				hitTestResult.Axis = axisScrollBar.axis;
				if (axisScrollBar.axis != null)
				{
					hitTestResult.ChartArea = axisScrollBar.axis.chartArea;
				}
				break;
			}
			case ChartElementType.SBThumbTracker:
			{
				AxisScrollBar axisScrollBar = (AxisScrollBar)obj;
				hitTestResult.Axis = axisScrollBar.axis;
				if (axisScrollBar.axis != null)
				{
					hitTestResult.ChartArea = axisScrollBar.axis.chartArea;
				}
				break;
			}
			}
			return hitTestResult;
		}

		public void Save(string imageFileName, ChartImageFormat format)
		{
			FileStream fileStream = new FileStream(imageFileName, FileMode.Create);
			try
			{
				Save(fileStream, format);
			}
			finally
			{
				fileStream.Close();
			}
		}

		public void Save(Stream imageStream, ChartImageFormat format)
		{
			chartPicture.isSavingAsImage = true;
			chartPicture.isPrinting = true;
			try
			{
				if (format == ChartImageFormat.Emf || format == ChartImageFormat.EmfDual || format == ChartImageFormat.EmfPlus)
				{
					EmfType emfType = EmfType.EmfOnly;
					switch (format)
					{
					case ChartImageFormat.EmfDual:
						emfType = EmfType.EmfPlusDual;
						break;
					case ChartImageFormat.EmfPlus:
						emfType = EmfType.EmfPlusOnly;
						break;
					}
					chartPicture.SaveIntoMetafile(imageStream, emfType);
					return;
				}
				Image image = chartPicture.GetImage(ImageResolution);
				ImageFormat format2 = ImageFormat.Png;
				switch (format)
				{
				case ChartImageFormat.Bmp:
					format2 = ImageFormat.Bmp;
					break;
				case ChartImageFormat.Jpeg:
					format2 = ImageFormat.Jpeg;
					break;
				case ChartImageFormat.Png:
					format2 = ImageFormat.Png;
					break;
				case ChartImageFormat.Emf:
					format2 = ImageFormat.Emf;
					break;
				}
				image.Save(imageStream, format2);
				image.Dispose();
			}
			finally
			{
				chartPicture.isSavingAsImage = false;
				chartPicture.isPrinting = false;
			}
		}

		public void Save(Stream imageStream)
		{
			chartPicture.isSavingAsImage = true;
			chartPicture.isPrinting = true;
			try
			{
				if (ImageType == ChartImageType.Emf)
				{
					chartPicture.SaveIntoMetafile(imageStream, EmfType.EmfOnly);
					return;
				}
				Image image = chartPicture.GetImage(ImageResolution);
				ImageCodecInfo imageCodecInfo = null;
				EncoderParameter encoderParameter = null;
				EncoderParameters encoderParameters = new EncoderParameters(1);
				if (ImageType == ChartImageType.Bmp)
				{
					imageCodecInfo = GetEncoderInfo("image/bmp");
				}
				else if (ImageType == ChartImageType.Jpeg)
				{
					imageCodecInfo = GetEncoderInfo("image/jpeg");
				}
				else if (ImageType == ChartImageType.Png)
				{
					imageCodecInfo = GetEncoderInfo("image/png");
				}
				encoderParameter = new EncoderParameter(Encoder.Quality, 100L - (long)Compression);
				encoderParameters.Param[0] = encoderParameter;
				if (imageCodecInfo == null)
				{
					ImageFormat format = (ImageFormat)new ImageFormatConverter().ConvertFromString(ImageType.ToString());
					image.Save(imageStream, format);
				}
				else
				{
					image.Save(imageStream, imageCodecInfo, encoderParameters);
				}
				image.Dispose();
			}
			finally
			{
				chartPicture.isSavingAsImage = false;
				chartPicture.isPrinting = false;
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

		public void RaisePostBackEvent(string eventArgument)
		{
		}

		[SRDescription("DescriptionAttributeChart_OnBackPaint")]
		protected virtual void OnBackPaint(object caller, ChartPaintEventArgs e)
		{
			if (this.BackPaint != null)
			{
				this.BackPaint(caller, e);
			}
			if (this.PrePaint != null)
			{
				this.PrePaint(caller, e);
			}
		}

		[SRDescription("DescriptionAttributeChart_OnPaint")]
		protected virtual void OnPaint(object caller, ChartPaintEventArgs e)
		{
			if (this.Paint != null)
			{
				this.Paint(caller, e);
			}
			if (this.PostPaint != null)
			{
				this.PostPaint(caller, e);
			}
		}

		[SRDescription("DescriptionAttributeChart_OnCustomizeMapAreas")]
		protected virtual void OnCustomizeMapAreas(MapAreasCollection areaItems)
		{
			if (this.CustomizeMapAreas != null)
			{
				this.CustomizeMapAreas(this, new CustomizeMapAreasEventArgs(areaItems));
			}
		}

		[SRDescription("DescriptionAttributeChart_OnCustomize")]
		protected virtual void OnCustomize()
		{
			if (this.Customize != null)
			{
				this.Customize(this, EventArgs.Empty);
			}
		}

		[SRDescription("DescriptionAttributeChart_OnCustomizeLegend")]
		protected virtual void OnCustomizeLegend(LegendItemsCollection legendItems, string legendName)
		{
			if (this.CustomizeLegend != null)
			{
				this.CustomizeLegend(this, new CustomizeLegendEventArgs(legendItems, legendName));
			}
		}

		internal void CallBackPaint(object caller, ChartPaintEventArgs e)
		{
			OnBackPaint(caller, e);
		}

		internal void CallPaint(object caller, ChartPaintEventArgs e)
		{
			OnPaint(caller, e);
		}

		internal void CallCustomizeMapAreas(MapAreasCollection areaItems)
		{
			OnCustomizeMapAreas(areaItems);
		}

		internal void CallCustomize()
		{
			OnCustomize();
		}

		internal void CallCustomizeLegend(LegendItemsCollection legendItems, string legendName)
		{
			OnCustomizeLegend(legendItems, legendName);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetPaletteCustomColors()
		{
			PaletteCustomColors = new Color[0];
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializePaletteCustomColors()
		{
			if (PaletteCustomColors == null || PaletteCustomColors.Length == 0)
			{
				return false;
			}
			return true;
		}

		public void SaveXml(string name)
		{
			try
			{
				Serializer.Save(name);
			}
			catch
			{
			}
		}

		public void ApplyPaletteColors()
		{
			dataManager.ApplyPaletteColors();
			foreach (Series item in Series)
			{
				bool flag = false;
				if (item.Palette != 0 || chartTypeRegistry.GetChartType(item.ChartTypeName).ApplyPaletteColorsToPoints)
				{
					item.ApplyPaletteColors();
				}
			}
		}

		internal bool IsDesignMode()
		{
			return false;
		}

		public void ResetAutoValues()
		{
			foreach (Series item in Series)
			{
				item.ResetAutoValues();
			}
			foreach (ChartArea chartArea in ChartAreas)
			{
				chartArea.ResetAutoValues();
			}
		}

		public void AlignDataPointsByAxisLabel()
		{
			chartPicture.AlignDataPointsByAxisLabel(sortAxisLabels: false, PointsSortOrder.Ascending);
		}

		public void AlignDataPointsByAxisLabel(string series)
		{
			ArrayList arrayList = new ArrayList();
			string[] array = series.Split(',');
			foreach (string text in array)
			{
				arrayList.Add(Series[text.Trim()]);
			}
			chartPicture.AlignDataPointsByAxisLabel(arrayList, sortAxisLabels: false, PointsSortOrder.Ascending);
		}

		public void AlignDataPointsByAxisLabel(string series, PointsSortOrder sortingOrder)
		{
			ArrayList arrayList = new ArrayList();
			string[] array = series.Split(',');
			foreach (string text in array)
			{
				arrayList.Add(Series[text.Trim()]);
			}
			chartPicture.AlignDataPointsByAxisLabel(arrayList, sortAxisLabels: true, sortingOrder);
		}

		public void AlignDataPointsByAxisLabel(PointsSortOrder sortingOrder)
		{
			chartPicture.AlignDataPointsByAxisLabel(sortAxisLabels: true, sortingOrder);
		}

		public object GetService(Type serviceType)
		{
			object result = null;
			if (serviceContainer != null)
			{
				result = serviceContainer.GetService(serviceType);
			}
			return result;
		}
	}
}

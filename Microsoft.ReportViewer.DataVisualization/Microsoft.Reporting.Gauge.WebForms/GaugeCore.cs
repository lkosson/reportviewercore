using Microsoft.Win32;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Timers;
using System.Windows.Forms;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class GaugeCore : NamedElement, IDisposable, ISelectable
	{
		private InputValueCollection inputValues;

		private CircularGaugeCollection circularGauges;

		private LinearGaugeCollection linearGauges;

		private NumericIndicatorCollection numericIndicators;

		private StateIndicatorCollection stateIndicators;

		private GaugeImageCollection images;

		private GaugeLabelCollection labels;

		private NamedImageCollection namedImages;

		internal bool silentPaint;

		internal RenderContent renderContent;

		internal string loadedBuildNumber = string.Empty;

		internal bool hasTransparentBackground;

		private MapAreaCollection mapAreas;

		private IRenderable[] renderableElements;

		private BufferBitmap bmpGauge;

		private BufferBitmap bmpFaces;

		private ImageLoader imageLoader;

		internal bool dirtyFlag;

		internal bool disableInvalidate;

		internal bool layoutFlag;

		internal bool isInitializing;

		internal bool refreshPending;

		internal bool boundToDataSource;

		private bool isPrinting;

		private Size printSize = new Size(0, 0);

		internal ServiceContainer serviceContainer;

		private NamedCollection[] elementCollections;

		private TraceManager traceManager;

		public string licenseData = "";

		private string valueExpression = "";

		private string toolTip = "";

		private string multiValueSeparator = "\\#\\";

		private float imageResolution = 96f;

		private GaugeThemes gaugeTheme = GaugeThemes.Default;

		private bool autoLayout = true;

		private double refreshRate = 30.0;

		private BackFrame frame;

		private string topImage = "";

		private Color topImageTransColor = Color.Empty;

		private Color topImageHueColor = Color.Empty;

		private AntiAliasing antiAliasing = AntiAliasing.All;

		private TextAntiAliasingQuality textAntiAliasingQuality = TextAntiAliasingQuality.High;

		private float shadowIntensity = 25f;

		private RightToLeft rightToLeft;

		private Color selectionMarkerColor = Color.LightBlue;

		private Color selectionBorderColor = Color.Black;

		private Color borderColor = Color.Black;

		private GaugeDashStyle borderStyle;

		private int borderWidth = 1;

		private float realTimeDataInterval = 1f;

		private ImageType imageType = ImageType.Png;

		private int compression;

		private string gaugeImageUrl = "TempFiles/GaugePic_#SEQ(300,3)";

		private bool mapEnabled = true;

		private RenderType renderType;

		private Color transparentColor = Color.Empty;

		private SerializationContent viewStateContent = SerializationContent.All;

		private AutoBool renderAsControl = AutoBool.False;

		private string winControlUrl = "";

		private string loadingDataText = "Loading...";

		private string loadingDataImage = "";

		private string loadingControlImage = "";

		private string tagAttributes = "";

		private object dataSource;

		private GaugeSerializer serializer;

		private CallbackManager callbackManager;

		private GaugeContainer parent;

		private HotRegionList hotRegionList;

		private bool serializing;

		private ISelectable selectedDesignTimeElement;

		private bool savingToMetafile;

		public InputValueCollection Values => inputValues;

		public NamedImageCollection NamedImages => namedImages;

		public CircularGaugeCollection CircularGauges => circularGauges;

		public LinearGaugeCollection LinearGauges => linearGauges;

		public NumericIndicatorCollection NumericIndicators => numericIndicators;

		public StateIndicatorCollection StateIndicators => stateIndicators;

		public GaugeImageCollection Images => images;

		public GaugeLabelCollection Labels => labels;

		public MapAreaCollection MapAreas => mapAreas;

		[DefaultValue("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string ValueExpression
		{
			get
			{
				return valueExpression;
			}
			set
			{
				if (Values.GetByName("Default") == null)
				{
					Values.Add("Default");
				}
				valueExpression = value;
				double result = 0.0;
				if (double.TryParse(value, out result))
				{
					Values["Default"].Value = result;
				}
			}
		}

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

		[DefaultValue("\\#\\")]
		[SRCategory("CategoryGaugeContainer")]
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

		[DefaultValue(96f)]
		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeImageResolution")]
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

		[DefaultValue(GaugeThemes.Default)]
		public GaugeThemes GaugeTheme
		{
			get
			{
				return gaugeTheme;
			}
			set
			{
				gaugeTheme = value;
			}
		}

		[DefaultValue(true)]
		public bool AutoLayout
		{
			get
			{
				return autoLayout;
			}
			set
			{
				autoLayout = value;
				DoAutoLayout();
				Invalidate();
			}
		}

		[DefaultValue(30.0)]
		public double RefreshRate
		{
			get
			{
				return refreshRate;
			}
			set
			{
				refreshRate = value;
			}
		}

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
				Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "White")]
		public Color BackColor
		{
			get
			{
				return GaugeContainer.BackColor;
			}
			set
			{
				GaugeContainer.BackColor = value;
				if (value.A != byte.MaxValue || value == Color.Empty)
				{
					hasTransparentBackground = true;
				}
				else
				{
					hasTransparentBackground = false;
				}
				Invalidate();
			}
		}

		[DefaultValue(RightToLeft.No)]
		public RightToLeft RightToLeft
		{
			get
			{
				return rightToLeft;
			}
			set
			{
				rightToLeft = value;
				Invalidate();
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
					printSize.Width = value;
				}
				else
				{
					GaugeContainer.Width = value;
				}
				Invalidate();
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
					printSize.Height = value;
				}
				else
				{
					GaugeContainer.Height = value;
				}
				Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public override string Name
		{
			get
			{
				return "GaugeContainer";
			}
			set
			{
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeGaugeCore_BuildNumber")]
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
					text = executingAssembly.FullName.ToUpperInvariant();
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

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeGaugeCore_ControlType")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue("")]
		public string ControlType
		{
			get
			{
				return "DundasWebGauge";
			}
			set
			{
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
				Invalidate();
			}
		}

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

		[DefaultValue(1f)]
		public float RealTimeDataInterval
		{
			get
			{
				return realTimeDataInterval;
			}
			set
			{
				realTimeDataInterval = value;
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

		[DefaultValue("TempFiles/GaugePic_#SEQ(300,3)")]
		public string ImageUrl
		{
			get
			{
				return gaugeImageUrl;
			}
			set
			{
				gaugeImageUrl = value;
			}
		}

		[DefaultValue(true)]
		public bool MapEnabled
		{
			get
			{
				return mapEnabled;
			}
			set
			{
				mapEnabled = value;
			}
		}

		[DefaultValue(RenderType.ImageTag)]
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

		[DefaultValue(typeof(Color), "")]
		public Color TransparentColor
		{
			get
			{
				return transparentColor;
			}
			set
			{
				transparentColor = value;
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

		[DefaultValue(AutoBool.False)]
		public AutoBool RenderAsControl
		{
			get
			{
				return renderAsControl;
			}
			set
			{
				renderAsControl = value;
			}
		}

		[DefaultValue("")]
		public string WinControlUrl
		{
			get
			{
				return winControlUrl;
			}
			set
			{
				winControlUrl = value;
			}
		}

		[DefaultValue("Loading...")]
		public string LoadingDataText
		{
			get
			{
				return loadingDataText;
			}
			set
			{
				loadingDataText = value;
			}
		}

		[DefaultValue("")]
		public string LoadingDataImage
		{
			get
			{
				return loadingDataImage;
			}
			set
			{
				loadingDataImage = value;
			}
		}

		[DefaultValue("")]
		public string LoadingControlImage
		{
			get
			{
				return loadingControlImage;
			}
			set
			{
				loadingControlImage = value;
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

		internal object DataSource
		{
			get
			{
				return dataSource;
			}
			set
			{
				dataSource = value;
				boundToDataSource = false;
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		internal GaugeSerializer Serializer => serializer;

		[SerializationVisibility(SerializationVisibility.Hidden)]
		internal CallbackManager CallbackManager => callbackManager;

		internal GaugeContainer GaugeContainer => parent;

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

		internal ISelectable SelectedDesignTimeElement
		{
			get
			{
				return selectedDesignTimeElement;
			}
			set
			{
				selectedDesignTimeElement = value;
				Invalidate();
			}
		}

		internal bool SavingToMetafile
		{
			get
			{
				return savingToMetafile;
			}
			set
			{
				savingToMetafile = value;
			}
		}

		internal bool InvokeRequired => false;

		public GaugeCore()
		{
			parent = null;
			inputValues = new InputValueCollection(this, null);
			circularGauges = new CircularGaugeCollection(this, null);
			linearGauges = new LinearGaugeCollection(this, null);
			numericIndicators = new NumericIndicatorCollection(this, null);
			stateIndicators = new StateIndicatorCollection(this, null);
			images = new GaugeImageCollection(this, null);
			labels = new GaugeLabelCollection(this, null);
			frame = new BackFrame(this, BackFrameStyle.None, BackFrameShape.Rectangular);
			mapAreas = new MapAreaCollection();
			namedImages = new NamedImageCollection(this, common);
		}

		internal GaugeCore(GaugeContainer parent)
		{
			this.parent = parent;
			serviceContainer = new ServiceContainer();
			serviceContainer.AddService(typeof(GaugeCore), this);
			serviceContainer.AddService(typeof(GaugeContainer), parent);
			common = new CommonElements(serviceContainer);
			inputValues = new InputValueCollection(this, common);
			circularGauges = new CircularGaugeCollection(this, common);
			linearGauges = new LinearGaugeCollection(this, common);
			numericIndicators = new NumericIndicatorCollection(this, common);
			stateIndicators = new StateIndicatorCollection(this, common);
			images = new GaugeImageCollection(this, common);
			labels = new GaugeLabelCollection(this, common);
			frame = new BackFrame(this, BackFrameStyle.None, BackFrameShape.Rectangular);
			serializer = new GaugeSerializer(serviceContainer);
			mapAreas = new MapAreaCollection();
			callbackManager = new CallbackManager();
			traceManager = new TraceManager(serviceContainer);
			serviceContainer.AddService(traceManager.GetType(), traceManager);
			namedImages = new NamedImageCollection(this, common);
			imageLoader = new ImageLoader(serviceContainer);
			serviceContainer.AddService(typeof(ImageLoader), imageLoader);
		}

		internal void DoAutoLayout()
		{
			if (!AutoLayout || GetWidth() == 0 || GetHeight() == 0)
			{
				return;
			}
			CircularGauge[] circularAutoLayoutGauges = GetCircularAutoLayoutGauges();
			LinearGauge[] linearAutoLayoutGauges = GetLinearAutoLayoutGauges();
			StateIndicator[] autoLayoutStateIndicators = GetAutoLayoutStateIndicators();
			int num = circularAutoLayoutGauges.Length + linearAutoLayoutGauges.Length + autoLayoutStateIndicators.Length;
			if (num == 0)
			{
				return;
			}
			layoutFlag = true;
			float num2 = (float)circularAutoLayoutGauges.Length / (float)num * 100f;
			float num3 = (float)linearAutoLayoutGauges.Length / (float)num * 100f;
			float num4 = (float)autoLayoutStateIndicators.Length / (float)num * 100f;
			RectangleF empty = RectangleF.Empty;
			RectangleF empty2 = RectangleF.Empty;
			RectangleF empty3 = RectangleF.Empty;
			if (GetWidth() <= GetHeight())
			{
				empty = new RectangleF(0f, 0f, 100f, num2);
				empty2 = new RectangleF(0f, num2, 100f, 100f - num2 - num4);
				empty3 = new RectangleF(0f, num2 + num3, 100f, 100f - num2 - num3);
			}
			else
			{
				empty = new RectangleF(0f, 0f, num2, 100f);
				empty2 = new RectangleF(num2, 0f, 100f - num2 - num4, 100f);
				empty3 = new RectangleF(num2 + num3, 0f, 100f - num2 - num3, 100f);
			}
			if (circularAutoLayoutGauges.Length != 0)
			{
				int num5 = (int)((float)GetWidth() * empty.Width / 100f);
				int num6 = (int)((float)GetHeight() * empty.Height / 100f);
				bool stackHorizontally;
				float num7;
				if (num5 > num6)
				{
					stackHorizontally = true;
					num7 = (float)num6 / (float)num5;
				}
				else
				{
					stackHorizontally = false;
					num7 = (float)num5 / (float)num6;
				}
				if (circularAutoLayoutGauges.Length % 3 == 0 && circularAutoLayoutGauges.Length > 6 && num7 > 1f / (float)(circularAutoLayoutGauges.Length / 3))
				{
					LayoutMatrix(circularAutoLayoutGauges, empty, stackHorizontally, 3);
				}
				else if (circularAutoLayoutGauges.Length % 2 == 0 && circularAutoLayoutGauges.Length > 2 && num7 > 1f / (float)(circularAutoLayoutGauges.Length / 2))
				{
					LayoutMatrix(circularAutoLayoutGauges, empty, stackHorizontally, 2);
				}
				else
				{
					LayoutSingleLine(circularAutoLayoutGauges, empty, stackHorizontally);
				}
			}
			if (linearAutoLayoutGauges.Length != 0)
			{
				float num8 = (float)GetWidth() * empty2.Width / 100f;
				float num9 = (float)GetHeight() * empty2.Height / 100f;
				SplitAutoLayoutGauges(linearAutoLayoutGauges, num8, num9, out LinearGauge[] horizontalGauges, out LinearGauge[] verticalGauges);
				if (horizontalGauges.Length == 0 || verticalGauges.Length == 0)
				{
					bool stackHorizontally2 = ShouldStackHorizontally(linearAutoLayoutGauges, empty2);
					LayoutSingleLine(linearAutoLayoutGauges, empty2, stackHorizontally2);
				}
				else if (num8 > num9)
				{
					RectangleF rectangleF = empty2;
					rectangleF.Width /= 2f;
					RectangleF rectangleF2 = empty2;
					rectangleF2.X = rectangleF.Width;
					rectangleF2.Width -= rectangleF.Width;
					bool stackHorizontally3 = ShouldStackHorizontally(horizontalGauges, rectangleF);
					LayoutSingleLine(horizontalGauges, rectangleF, stackHorizontally3);
					bool flag = ShouldStackHorizontally(verticalGauges, rectangleF2);
					LayoutSingleLine(verticalGauges, rectangleF2, flag);
					float num10 = 0f;
					LinearGauge[] array = verticalGauges;
					foreach (LinearGauge linearGauge in array)
					{
						num10 = ((!flag) ? Math.Max(num10, linearGauge.GetAspectRatioBounds().Width) : (num10 + linearGauge.GetAspectRatioBounds().Width));
					}
					rectangleF.Width = empty2.Width - num10;
					rectangleF2.X = rectangleF.Width;
					rectangleF2.Width = num10;
					LayoutSingleLine(horizontalGauges, rectangleF, stackHorizontally3);
					LayoutSingleLine(verticalGauges, rectangleF2, flag);
				}
				else
				{
					RectangleF rectangleF3 = empty2;
					rectangleF3.Height /= 2f;
					RectangleF rectangleF4 = empty2;
					rectangleF4.Y = rectangleF3.Height;
					rectangleF4.Height -= rectangleF3.Height;
					bool flag2 = ShouldStackHorizontally(horizontalGauges, rectangleF3);
					LayoutSingleLine(horizontalGauges, rectangleF3, flag2);
					bool stackHorizontally4 = ShouldStackHorizontally(verticalGauges, rectangleF4);
					LayoutSingleLine(verticalGauges, rectangleF4, stackHorizontally4);
					float num11 = 0f;
					LinearGauge[] array = horizontalGauges;
					foreach (LinearGauge linearGauge2 in array)
					{
						num11 = (flag2 ? Math.Max(num11, linearGauge2.GetAspectRatioBounds().Height) : (num11 + linearGauge2.GetAspectRatioBounds().Height));
					}
					rectangleF3.Height = num11;
					rectangleF4.Y = num11;
					rectangleF4.Height = empty2.Height - num11;
					LayoutSingleLine(horizontalGauges, rectangleF3, flag2);
					LayoutSingleLine(verticalGauges, rectangleF4, stackHorizontally4);
				}
			}
			if (autoLayoutStateIndicators.Length != 0)
			{
				empty3.Inflate(-4f, -4f);
				int num12 = (int)((float)GetWidth() * empty3.Width / 100f);
				int num13 = (int)((float)GetHeight() * empty3.Height / 100f);
				bool stackHorizontally5 = (num12 > num13) ? true : false;
				LayoutSingleLine(autoLayoutStateIndicators, empty3, stackHorizontally5);
			}
			layoutFlag = false;
		}

		private bool ShouldStackHorizontally(LinearGauge[] linearGauges, RectangleF availableRect)
		{
			float num = (float)GetWidth() * availableRect.Width / 100f;
			float num2 = (float)GetHeight() * availableRect.Height / 100f;
			float horizontalStackAspectRatio = GetHorizontalStackAspectRatio(linearGauges, num, num2);
			float verticalStackAspectRatio = GetVerticalStackAspectRatio(linearGauges, num, num2);
			float num3 = num / num2;
			float num4 = Math.Abs(num3 - horizontalStackAspectRatio);
			float num5 = Math.Abs(num3 - verticalStackAspectRatio);
			return num4 > num5;
		}

		private float GetHorizontalStackAspectRatio(LinearGauge[] horizontalGauges, float pixelWidth, float pixelHeight)
		{
			if (horizontalGauges.Length == 0)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < horizontalGauges.Length; i++)
			{
				num += 1f / GetPreferredAspectRatio(horizontalGauges[i], pixelWidth, pixelHeight);
			}
			return 1f / num;
		}

		private float GetVerticalStackAspectRatio(LinearGauge[] verticalGauges, float pixelWidth, float pixelHeight)
		{
			if (verticalGauges.Length == 0)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < verticalGauges.Length; i++)
			{
				num += GetPreferredAspectRatio(verticalGauges[i], pixelWidth, pixelHeight);
			}
			return num;
		}

		private void SplitAutoLayoutGauges(LinearGauge[] autoLayoutGauges, float pixelWidth, float pixelHeight, out LinearGauge[] horizontalGauges, out LinearGauge[] verticalGauges)
		{
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			foreach (LinearGauge linearGauge in autoLayoutGauges)
			{
				if (GetPreferredAspectRatio(linearGauge, pixelWidth, pixelHeight) >= 1f)
				{
					arrayList.Add(linearGauge);
				}
				else
				{
					arrayList2.Add(linearGauge);
				}
			}
			horizontalGauges = (LinearGauge[])arrayList.ToArray(typeof(LinearGauge));
			verticalGauges = (LinearGauge[])arrayList2.ToArray(typeof(LinearGauge));
		}

		private float GetPreferredAspectRatio(GaugeBase gauge, float pixelWidth, float pixelHeight)
		{
			if (!float.IsNaN(gauge.AspectRatio))
			{
				return gauge.AspectRatio;
			}
			if (gauge is CircularGauge)
			{
				return 1f;
			}
			LinearGauge linearGauge = (LinearGauge)gauge;
			if (linearGauge.Orientation == GaugeOrientation.Horizontal)
			{
				return 5f;
			}
			if (linearGauge.Orientation == GaugeOrientation.Vertical)
			{
				return 0.2f;
			}
			return pixelWidth / pixelHeight;
		}

		internal CircularGauge[] GetCircularAutoLayoutGauges()
		{
			ArrayList arrayList = new ArrayList();
			foreach (CircularGauge circularGauge in CircularGauges)
			{
				if (circularGauge.Parent == string.Empty)
				{
					arrayList.Add(circularGauge);
				}
			}
			return (CircularGauge[])arrayList.ToArray(typeof(CircularGauge));
		}

		internal LinearGauge[] GetLinearAutoLayoutGauges()
		{
			ArrayList arrayList = new ArrayList();
			foreach (LinearGauge linearGauge in LinearGauges)
			{
				if (linearGauge.Parent == string.Empty)
				{
					arrayList.Add(linearGauge);
				}
			}
			return (LinearGauge[])arrayList.ToArray(typeof(LinearGauge));
		}

		internal StateIndicator[] GetAutoLayoutStateIndicators()
		{
			ArrayList arrayList = new ArrayList();
			foreach (StateIndicator stateIndicator in StateIndicators)
			{
				if (stateIndicator.Parent == string.Empty)
				{
					arrayList.Add(stateIndicator);
				}
			}
			return (StateIndicator[])arrayList.ToArray(typeof(StateIndicator));
		}

		private void LayoutSingleLine(GaugeBase[] gauges, RectangleF rect, bool stackHorizontally)
		{
			if (stackHorizontally)
			{
				float num = rect.Width / (float)gauges.Length;
				float num2 = rect.X;
				GaugeBase[] array = gauges;
				foreach (GaugeBase gaugeBase in array)
				{
					gaugeBase.Location.X = num2;
					gaugeBase.Location.Y = rect.Y;
					gaugeBase.Size.Width = num;
					gaugeBase.Size.Height = rect.Height;
					CompensateForShadowAndBorder(gaugeBase);
					num2 += num;
				}
			}
			else
			{
				float num3 = rect.Height / (float)gauges.Length;
				float num4 = rect.Y;
				GaugeBase[] array = gauges;
				foreach (GaugeBase gaugeBase2 in array)
				{
					gaugeBase2.Location.X = rect.X;
					gaugeBase2.Location.Y = num4;
					gaugeBase2.Size.Width = rect.Width;
					gaugeBase2.Size.Height = num3;
					CompensateForShadowAndBorder(gaugeBase2);
					num4 += num3;
				}
			}
		}

		private void CompensateForShadowAndBorder(GaugeBase gauge)
		{
			if (gauge.BackFrame.BorderWidth > 1 && gauge.BackFrame.BorderStyle != 0 && gauge.BackFrame.BorderColor.A != 0)
			{
				float num = (float)gauge.BackFrame.BorderWidth / (float)GetWidth() * 100f;
				float num2 = (float)gauge.BackFrame.BorderWidth / (float)GetHeight() * 100f;
				gauge.Location.X += num / 2f;
				gauge.Location.Y += num2 / 2f;
				gauge.Size.Width -= num;
				gauge.Size.Height -= num2;
			}
			if (gauge.BackFrame.ShadowOffset != 0f)
			{
				float num3 = gauge.BackFrame.ShadowOffset / (float)GetWidth() * 100f;
				float num4 = gauge.BackFrame.ShadowOffset / (float)GetHeight() * 100f;
				if (gauge.BackFrame.ShadowOffset < 0f)
				{
					gauge.Location.X -= num3;
					gauge.Location.Y -= num4;
					gauge.Size.Width += num3;
					gauge.Size.Height += num4;
				}
				else
				{
					gauge.Size.Width -= num3;
					gauge.Size.Height -= num4;
				}
			}
		}

		private void LayoutMatrix(GaugeBase[] gauges, RectangleF rect, bool stackHorizontally, int lineCount)
		{
			if (stackHorizontally)
			{
				float num = rect.Height / (float)lineCount;
				float num2 = rect.Y;
				int num3 = 0;
				for (int i = 0; i < lineCount; i++)
				{
					RectangleF rect2 = new RectangleF(rect.X, num2, rect.Width, num);
					int num4 = gauges.Length / lineCount;
					GaugeBase[] array = new GaugeBase[num4];
					for (int j = 0; j < num4; j++)
					{
						array[j] = gauges[num3++];
					}
					LayoutSingleLine(array, rect2, stackHorizontally);
					num2 += num;
				}
				return;
			}
			float num5 = rect.Width / (float)lineCount;
			float num6 = rect.X;
			int num7 = 0;
			for (int k = 0; k < lineCount; k++)
			{
				RectangleF rect3 = new RectangleF(num6, rect.Y, num5, rect.Height);
				int num8 = gauges.Length / lineCount;
				GaugeBase[] array2 = new GaugeBase[num8];
				for (int l = 0; l < num8; l++)
				{
					array2[l] = gauges[num7++];
				}
				LayoutSingleLine(array2, rect3, stackHorizontally);
				num6 += num5;
			}
		}

		private void LayoutSingleLine(StateIndicator[] indicators, RectangleF rect, bool stackHorizontally)
		{
			if (stackHorizontally)
			{
				float num = rect.Width / (float)indicators.Length;
				float num2 = rect.X;
				StateIndicator[] array = indicators;
				foreach (StateIndicator obj in array)
				{
					obj.Location.X = num2;
					obj.Location.Y = rect.Y;
					obj.Size.Width = num;
					obj.Size.Height = rect.Height;
					num2 += num;
				}
			}
			else
			{
				float num3 = rect.Height / (float)indicators.Length;
				float num4 = rect.Y;
				StateIndicator[] array = indicators;
				foreach (StateIndicator obj2 in array)
				{
					obj2.Location.X = rect.X;
					obj2.Location.Y = num4;
					obj2.Size.Width = rect.Width;
					obj2.Size.Height = num3;
					num4 += num3;
				}
			}
		}

		internal int GetWidth()
		{
			if (isPrinting)
			{
				return printSize.Width;
			}
			return GaugeContainer.Width;
		}

		internal int GetHeight()
		{
			if (isPrinting)
			{
				return printSize.Height;
			}
			return GaugeContainer.Height;
		}

		internal void ResetAutoValues()
		{
			if (selectedDesignTimeElement != null)
			{
				selectedDesignTimeElement = null;
			}
		}

		internal override void Invalidate()
		{
			if (!layoutFlag)
			{
				dirtyFlag = true;
				common.ObjectLinker.Invalidate();
				if (!disableInvalidate)
				{
					Refresh();
				}
			}
		}

		internal void OnFontChanged()
		{
		}

		private void refreshTimer_Elapsed(object source, ElapsedEventArgs e)
		{
			if (refreshPending)
			{
				refreshPending = false;
				GaugeContainer.Invalidate();
			}
		}

		internal override void Refresh()
		{
		}

		internal NamedCollection[] GetRenderCollections()
		{
			if (elementCollections == null)
			{
				elementCollections = new NamedCollection[6];
				elementCollections[0] = CircularGauges;
				elementCollections[1] = LinearGauges;
				elementCollections[2] = Labels;
				elementCollections[3] = NumericIndicators;
				elementCollections[4] = StateIndicators;
				elementCollections[5] = Images;
			}
			return elementCollections;
		}

		internal override void BeginInit()
		{
			isInitializing = true;
			Values.BeginInit();
			NamedCollection[] renderCollections = GetRenderCollections();
			for (int i = 0; i < renderCollections.Length; i++)
			{
				renderCollections[i].BeginInit();
			}
		}

		internal override void EndInit()
		{
			isInitializing = false;
			Values.EndInit();
			NamedCollection[] renderCollections = GetRenderCollections();
			for (int i = 0; i < renderCollections.Length; i++)
			{
				renderCollections[i].EndInit();
			}
		}

		internal override void ReconnectData(bool exact)
		{
			Values.ReconnectData(exact);
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

		private BufferBitmap InitBitmap(BufferBitmap bmp, float dpiX, float dpiY)
		{
			return InitBitmap(bmp, new Size(GetWidth(), GetHeight()), dpiX, dpiY);
		}

		internal BufferBitmap InitBitmap(BufferBitmap bmp, Size size, float dpiX, float dpiY)
		{
			if (bmp == null)
			{
				bmp = new BufferBitmap(dpiX, dpiY);
			}
			else if (dirtyFlag)
			{
				bmp.Invalidate();
			}
			bmp.Size = size;
			bmp.Graphics.SmoothingMode = GetSmootingMode();
			bmp.Graphics.TextRenderingHint = GetTextRenderingHint();
			bmp.Graphics.TextContrast = 2;
			return bmp;
		}

		private SmoothingMode GetSmootingMode()
		{
			if ((GaugeContainer.AntiAliasing & AntiAliasing.Graphics) == AntiAliasing.Graphics)
			{
				return SmoothingMode.HighQuality;
			}
			return SmoothingMode.HighSpeed;
		}

		private TextRenderingHint GetTextRenderingHint()
		{
			TextRenderingHint textRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
			if ((GaugeContainer.AntiAliasing & AntiAliasing.Text) == AntiAliasing.Text)
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

		public GaugeGraphics GetGraphics(RenderingType renderingType, Graphics g, Stream outputStream)
		{
			GaugeGraphics gaugeGraphics = new GaugeGraphics(Common);
			Common.Height = GetHeight();
			Common.Width = GetWidth();
			gaugeGraphics.SetPictureSize(Common.Width, Common.Height);
			gaugeGraphics.ActiveRenderingType = renderingType;
			gaugeGraphics.Graphics = g;
			gaugeGraphics.SmoothingMode = GetSmootingMode();
			gaugeGraphics.TextRenderingHint = GetTextRenderingHint();
			return gaugeGraphics;
		}

		internal IRenderable[] GetGraphElements()
		{
			if (renderableElements == null || dirtyFlag)
			{
				ArrayList arrayList = new ArrayList();
				NamedCollection[] renderCollections = GetRenderCollections();
				foreach (NamedCollection c in renderCollections)
				{
					arrayList.AddRange(c);
				}
				ArrayList collection = (ArrayList)arrayList.Clone();
				arrayList.Sort(new ZOrderSort(collection));
				renderableElements = (IRenderable[])arrayList.ToArray(typeof(IRenderable));
			}
			return renderableElements;
		}

		private void RenderWaterMark(GaugeGraphics g)
		{
		}

		internal void RenderOneDynamicElement(GaugeGraphics g, IRenderable element, bool renderChildrenFirst)
		{
			try
			{
				RectangleF boundRect = element.GetBoundRect(g);
				g.CreateDrawRegion(boundRect);
				boundRect = g.GetAbsoluteRectangle(boundRect);
				float num = (element is GaugeBase) ? 0.1f : 0.01f;
				if (boundRect.Width > num && boundRect.Height > num)
				{
					if (renderChildrenFirst)
					{
						RenderElements(g, element, renderStaticElements: false);
					}
					element.RenderDynamicElements(g);
					if (!renderChildrenFirst)
					{
						RenderElements(g, element, renderStaticElements: false);
					}
					common.InvokePostPaint(element);
				}
			}
			finally
			{
				g.RestoreDrawRegion();
			}
		}

		internal void RenderElements(GaugeGraphics g, IRenderable parentElement, bool renderStaticElements)
		{
			IRenderable[] graphElements = GetGraphElements();
			ArrayList arrayList = new ArrayList();
			IRenderable[] array = graphElements;
			foreach (IRenderable renderable in array)
			{
				if (renderable.GetParentRenderable() == parentElement)
				{
					arrayList.Add(renderable);
				}
			}
			if (arrayList.Count <= 0)
			{
				return;
			}
			arrayList.Sort(new ZOrderSort(arrayList));
			if (renderStaticElements)
			{
				TraceWrite("GaugePaint", SR.TraceStartingRenderingStaticElements);
				for (int j = 0; j < arrayList.Count; j++)
				{
					try
					{
						RectangleF boundRect = ((IRenderable)arrayList[j]).GetBoundRect(g);
						g.CreateDrawRegion(boundRect);
						boundRect = g.GetAbsoluteRectangle(boundRect);
						if ((double)boundRect.Width > 0.1 && (double)boundRect.Height > 0.1)
						{
							((IRenderable)arrayList[j]).RenderStaticElements(g);
							common.InvokePrePaint(arrayList[j]);
							RenderElements(g, (IRenderable)arrayList[j], renderStaticElements);
						}
					}
					finally
					{
						g.RestoreDrawRegion();
					}
				}
				TraceWrite("GaugePaint", SR.TraceFinishedRenderingStaticElements);
				return;
			}
			TraceWrite("GaugePaint", SR.TraceStartingRenderingDynamicElements);
			for (int k = 0; k < arrayList.Count; k++)
			{
				IRenderable renderable2 = (IRenderable)arrayList[k];
				if (renderable2 is NumericIndicator || renderable2 is StateIndicator || renderable2 is GaugeLabel)
				{
					RenderOneDynamicElement(g, renderable2, renderChildrenFirst: false);
				}
			}
			for (int num = arrayList.Count - 1; num >= 0; num--)
			{
				IRenderable renderable3 = (IRenderable)arrayList[num];
				if (!(renderable3 is NumericIndicator) && !(renderable3 is StateIndicator) && !(renderable3 is GaugeLabel))
				{
					RenderOneDynamicElement(g, renderable3, renderChildrenFirst: true);
				}
			}
			TraceWrite("GaugePaint", SR.TraceFinishedRenderingDynamicElements);
		}

		internal void RenderSelection(GaugeGraphics g)
		{
			NamedElement[] selectedElements = GetSelectedElements();
			if (selectedElements != null)
			{
				NamedElement[] array = selectedElements;
				for (int i = 0; i < array.Length; i++)
				{
					(array[i] as ISelectable)?.DrawSelection(g, designTimeSelection: false);
				}
			}
			if (selectedDesignTimeElement != null)
			{
				selectedDesignTimeElement.DrawSelection(g, designTimeSelection: true);
			}
		}

		internal NamedElement[] GetSelectedElements()
		{
			ArrayList arrayList = new ArrayList();
			foreach (CircularGauge circularGauge in CircularGauges)
			{
				if (circularGauge.Selected)
				{
					arrayList.Add(circularGauge);
				}
				foreach (CircularScale scale in circularGauge.Scales)
				{
					if (scale.Selected)
					{
						arrayList.Add(scale);
					}
				}
				foreach (CircularRange range in circularGauge.Ranges)
				{
					if (range.Selected)
					{
						arrayList.Add(range);
					}
				}
				foreach (CircularPointer pointer in circularGauge.Pointers)
				{
					if (pointer.Selected)
					{
						arrayList.Add(pointer);
					}
				}
				foreach (Knob knob in circularGauge.Knobs)
				{
					if (knob.Selected)
					{
						arrayList.Add(knob);
					}
				}
			}
			foreach (LinearGauge linearGauge in LinearGauges)
			{
				if (linearGauge.Selected)
				{
					arrayList.Add(linearGauge);
				}
				foreach (LinearScale scale2 in linearGauge.Scales)
				{
					if (scale2.Selected)
					{
						arrayList.Add(scale2);
					}
				}
				foreach (LinearRange range2 in linearGauge.Ranges)
				{
					if (range2.Selected)
					{
						arrayList.Add(range2);
					}
				}
				foreach (LinearPointer pointer2 in linearGauge.Pointers)
				{
					if (pointer2.Selected)
					{
						arrayList.Add(pointer2);
					}
				}
			}
			foreach (NumericIndicator numericIndicator in NumericIndicators)
			{
				if (numericIndicator.Selected)
				{
					arrayList.Add(numericIndicator);
				}
			}
			foreach (StateIndicator stateIndicator in StateIndicators)
			{
				if (stateIndicator.Selected)
				{
					arrayList.Add(stateIndicator);
				}
			}
			foreach (GaugeImage image in Images)
			{
				if (image.Selected)
				{
					arrayList.Add(image);
				}
			}
			foreach (GaugeLabel label in Labels)
			{
				if (label.Selected)
				{
					arrayList.Add(label);
				}
			}
			if (arrayList.Count == 0)
			{
				return null;
			}
			return (NamedElement[])arrayList.ToArray(typeof(NamedElement));
		}

		internal void RenderTopImage(GaugeGraphics g)
		{
			if (TopImage != "")
			{
				ImageAttributes imageAttributes = new ImageAttributes();
				if (TopImageTransColor != Color.Empty)
				{
					imageAttributes.SetColorKey(TopImageTransColor, TopImageTransColor, ColorAdjustType.Default);
				}
				Image image = Common.ImageLoader.LoadImage(TopImage);
				Rectangle destRect = new Rectangle(0, 0, GetWidth(), GetHeight());
				if (!TopImageHueColor.IsEmpty)
				{
					Color color = g.TransformHueColor(TopImageHueColor);
					ColorMatrix colorMatrix = new ColorMatrix();
					colorMatrix.Matrix00 = (float)(int)color.R / 255f;
					colorMatrix.Matrix11 = (float)(int)color.G / 255f;
					colorMatrix.Matrix22 = (float)(int)color.B / 255f;
					imageAttributes.SetColorMatrix(colorMatrix);
				}
				ImageSmoothingState imageSmoothingState = new ImageSmoothingState(g);
				imageSmoothingState.Set();
				g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				imageSmoothingState.Restore();
			}
		}

		internal void RenderStaticElements(GaugeGraphics g)
		{
			if (renderContent != RenderContent.Dynamic)
			{
				using (Brush brush = new SolidBrush(GaugeContainer.BackColor))
				{
					g.FillRectangle(brush, new Rectangle(new Point(-1, -1), new Size(GetWidth() + 2, GetHeight() + 2)));
				}
				HotRegionList.Clear();
				HotRegionList.SetHotRegion(this);
				BackFrame.RenderFrame(g);
				RenderBorder(g);
				RenderElements(g, null, renderStaticElements: true);
			}
		}

		private void RenderBorder(GaugeGraphics g)
		{
			if (BorderStyle == GaugeDashStyle.NotSet || BorderColor.A == 0 || BorderWidth == 0)
			{
				return;
			}
			using (Pen pen = new Pen(BorderColor, BorderWidth))
			{
				pen.DashStyle = g.GetPenStyle(BorderStyle);
				pen.Alignment = PenAlignment.Inset;
				using (GraphicsPath graphicsPath = new GraphicsPath())
				{
					if (g.Graphics.PageScale > 1f)
					{
						graphicsPath.AddRectangle(new RectangleF(0f, 0f, GetWidth(), GetHeight()));
					}
					else
					{
						graphicsPath.AddRectangle(new RectangleF(0f, 0f, GetWidth() - 1, GetHeight() - 1));
					}
					AntiAliasing antiAliasing = g.AntiAliasing;
					g.AntiAliasing = AntiAliasing.None;
					g.DrawPath(pen, graphicsPath);
					g.AntiAliasing = antiAliasing;
				}
			}
		}

		internal void RenderStaticElementsBufered(GaugeGraphics g)
		{
			if (dirtyFlag || bmpFaces == null)
			{
				bmpFaces = InitBitmap(bmpFaces, g.Graphics.DpiX, g.Graphics.DpiY);
				Graphics graphics = g.Graphics;
				try
				{
					g.Graphics = bmpFaces.Graphics;
					HotRegionList.Clear();
					RenderStaticElements(g);
				}
				finally
				{
					g.Graphics = graphics;
				}
			}
			g.DrawImage(bmpFaces.Bitmap, new Rectangle(new Point(0, 0), bmpFaces.Size));
		}

		internal void RenderDynamicElements(GaugeGraphics g)
		{
			if (renderContent != RenderContent.Static)
			{
				RenderElements(g, null, renderStaticElements: false);
				BackFrame.RenderGlassEffect(g);
				RenderTopImage(g);
				RenderSelection(g);
			}
		}

		internal void Paint(Graphics gdiGraph, RenderingType renderingType, Stream stream, bool buffered)
		{
			TraceWrite("GaugePaint", SR.TraceStartingPaint);
			disableInvalidate = true;
			GaugeGraphics graphics = GetGraphics(renderingType, gdiGraph, stream);
			try
			{
				AutoDataBind(forceBinding: false);
				DoAutoLayout();
				if (buffered)
				{
					RenderStaticElementsBufered(graphics);
				}
				else
				{
					RenderStaticElements(graphics);
				}
				RenderDynamicElements(graphics);
				RenderWaterMark(graphics);
			}
			finally
			{
				disableInvalidate = false;
				graphics.Close();
			}
			dirtyFlag = false;
			TraceWrite("GaugePaint", SR.TracePaintComplete);
		}

		internal void PrintPaint(Graphics g, Rectangle position)
		{
			Notify(MessageType.PrepareSnapShot, this, null);
			printSize = position.Size;
			GraphicsState gstate = g.Save();
			try
			{
				isPrinting = true;
				g.TranslateTransform(position.X, position.Y);
				Paint(g, RenderingType.Gdi, null, buffered: false);
			}
			finally
			{
				g.Restore(gstate);
				isPrinting = false;
			}
		}

		internal void Paint(Graphics g)
		{
			disableInvalidate = true;
			bmpGauge = InitBitmap(bmpGauge, g.DpiX, g.DpiY);
			Paint(bmpGauge.Graphics, RenderingType.Gdi, null, buffered: true);
			if (!GaugeContainer.Enabled)
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
				Region clip = bmpGauge.Graphics.Clip;
				bmpGauge.Graphics.Clip = GetClipRegion();
				bmpGauge.Graphics.DrawImage(bmpGauge.Bitmap, new Rectangle(0, 0, GetWidth(), GetHeight()), 0, 0, GetWidth(), GetHeight(), GraphicsUnit.Pixel, imageAttributes);
				bmpGauge.Graphics.Clip.Dispose();
				bmpGauge.Graphics.Clip = clip;
			}
			g.DrawImageUnscaled(bmpGauge.Bitmap, 0, 0);
			dirtyFlag = false;
		}

		internal void SaveTo(Stream stream, GaugeImageFormat imageFormat, int compression, float dpiX, float dpiY)
		{
			if (isInitializing)
			{
				EndInit();
			}
			bool flag = dirtyFlag;
			dirtyFlag = true;
			Notify(MessageType.PrepareSnapShot, this, null);
			if (imageFormat == GaugeImageFormat.Emf)
			{
				SaveIntoMetafile(stream);
				dirtyFlag = flag;
				return;
			}
			BufferBitmap bufferBitmap = InitBitmap(null, dpiX, dpiY);
			RenderingType renderingType = RenderingType.Gdi;
			ImageFormat imageFormat2 = null;
			ImageCodecInfo imageCodecInfo = null;
			EncoderParameter encoderParameter = null;
			EncoderParameters encoderParameters = null;
			string text = imageFormat.ToString(CultureInfo.InvariantCulture);
			imageFormat2 = (ImageFormat)new ImageFormatConverter().ConvertFromString(text);
			Color color = (!(BackColor != Color.Empty)) ? Color.White : BackColor;
			Pen pen = new Pen(color);
			SmoothingMode smoothingMode = bufferBitmap.Graphics.SmoothingMode;
			bufferBitmap.Graphics.SmoothingMode = SmoothingMode.None;
			bufferBitmap.Graphics.DrawRectangle(pen, 0, 0, bufferBitmap.Size.Width, bufferBitmap.Size.Height);
			bufferBitmap.Graphics.SmoothingMode = smoothingMode;
			pen.Dispose();
			Paint(bufferBitmap.Graphics, renderingType, stream, buffered: false);
			if (renderingType == RenderingType.Gdi && compression >= 0 && compression <= 100 && "jpeg,png,bmp".IndexOf(text, StringComparison.OrdinalIgnoreCase) != -1)
			{
				imageCodecInfo = GetEncoderInfo("image/" + text);
				encoderParameters = new EncoderParameters(1);
				encoderParameter = new EncoderParameter(Encoder.Quality, 100L - (long)compression);
				encoderParameters.Param[0] = encoderParameter;
			}
			if (!GaugeContainer.Enabled)
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
			if (renderingType == RenderingType.Gdi && imageFormat != GaugeImageFormat.Emf)
			{
				if (imageCodecInfo == null)
				{
					bufferBitmap.Bitmap.Save(stream, imageFormat2);
				}
				else
				{
					if (TransparentColor != Color.Empty)
					{
						bufferBitmap.Bitmap.MakeTransparent(TransparentColor);
					}
					bufferBitmap.Bitmap.Save(stream, imageCodecInfo, encoderParameters);
				}
			}
			dirtyFlag = flag;
		}

		internal void SaveTo(Stream stream, GaugeImageFormat imageFormat, float dpiX, float dpiY)
		{
			SaveTo(stream, imageFormat, -1, dpiX, dpiY);
		}

		internal void SaveTo(string fileName, GaugeImageFormat imageFormat, float dpiX, float dpiY)
		{
			SaveTo(fileName, imageFormat, -1, dpiX, dpiY);
		}

		internal void SaveTo(string fileName, GaugeImageFormat imageFormat, int compression, float dpiX, float dpiY)
		{
			using (Stream stream = new FileStream(fileName, FileMode.Create))
			{
				SaveTo(stream, imageFormat, compression, dpiX, dpiY);
			}
		}

		private static ImageCodecInfo GetEncoderInfo(string mimeType)
		{
			ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
			for (int i = 0; i < imageEncoders.Length; i++)
			{
				if (imageEncoders[i].MimeType.Equals(mimeType, StringComparison.OrdinalIgnoreCase))
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
			Metafile metafile = new Metafile(imageStream, hdc, new Rectangle(0, 0, GetWidth(), GetHeight()), MetafileFrameUnit.Pixel, EmfType.EmfPlusOnly);
			Graphics graphics2 = Graphics.FromImage(metafile);
			graphics2.SmoothingMode = GetSmootingMode();
			graphics2.TextRenderingHint = GetTextRenderingHint();
			SavingToMetafile = true;
			Paint(graphics2, RenderingType.Gdi, imageStream, buffered: false);
			SavingToMetafile = false;
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

		private Region GetClipRegion()
		{
			Region region = null;
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				foreach (HotRegion item in HotRegionList.List)
				{
					if (item.SelectedObject is IRenderable)
					{
						GraphicsPath[] paths = item.Paths;
						foreach (GraphicsPath addingPath in paths)
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
			return GaugeContainer.IsDesignMode();
		}

		internal void DrawException(Graphics graphics, Exception e)
		{
			graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, GetWidth(), GetHeight());
			graphics.DrawRectangle(new Pen(Color.Red, 4f), 0, 0, GetWidth(), GetHeight());
			string text = SR.GaugeDesignException + e.Message;
			string text2 = "";
			if (e.TargetSite != null)
			{
				text2 += SR.GaugeDesignSite(e.TargetSite.ToString());
			}
			if (e.StackTrace != string.Empty)
			{
				text2 = text2 + SR.GaugeDesignStack + e.StackTrace;
			}
			RectangleF layoutRectangle = new RectangleF(3f, 3f, GetWidth() - 6, GetHeight() - 6);
			StringFormat format = new StringFormat();
			SizeF sizeF = graphics.MeasureString(text, new Font("Microsoft Sans Serif", 10f, FontStyle.Bold), (int)layoutRectangle.Width, format);
			graphics.DrawString(text, new Font("Microsoft Sans Serif", 10f, FontStyle.Bold), new SolidBrush(Color.Black), layoutRectangle, format);
			layoutRectangle.Y += sizeF.Height + 5f;
			graphics.DrawString(text2, new Font("Microsoft Sans Serif", 8f), new SolidBrush(Color.Black), layoutRectangle, format);
		}

		internal HitTestResult[] HitTest(int x, int y, Type[] objectTypes, bool returnMultipleElements)
		{
			if (objectTypes == null)
			{
				objectTypes = new Type[0];
			}
			ArrayList arrayList = new ArrayList();
			HotRegion[] array = HotRegionList.CheckHotRegions(x, y, objectTypes);
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
			if (element is IRenderable || element == this)
			{
				int num = HotRegionList.LocateObject(element);
				if (num != -1)
				{
					return (HotRegion)HotRegionList.List[num];
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionHotRegionInitialize", element.Name));
			}
			throw new ArgumentException(Utils.SRGetStr("ExceptionHotRegionSupport", element.Name));
		}

		internal void PopulateImageMaps()
		{
			for (int num = HotRegionList.List.Count - 1; num >= 0; num--)
			{
				HotRegion hotRegion = (HotRegion)HotRegionList.List[num];
				if (hotRegion.SelectedObject is IImageMapProvider)
				{
					IImageMapProvider imageMapProvider = (IImageMapProvider)hotRegion.SelectedObject;
					string a = imageMapProvider.GetToolTip();
					string href = imageMapProvider.GetHref();
					string mapAreaAttributes = imageMapProvider.GetMapAreaAttributes();
					if (a != "" || href != "" || mapAreaAttributes != "")
					{
						object tag = imageMapProvider.Tag;
						for (int i = 0; i < hotRegion.Paths.Length; i++)
						{
							if (hotRegion.Paths[i] != null)
							{
								mapAreas.Add(a, href, mapAreaAttributes, hotRegion.Paths[i], tag);
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
			output.Write("\r\n</MAP>\r\n");
		}

		internal void PerformDataBinding(IEnumerable data)
		{
			foreach (InputValue value in Values)
			{
				value.PerformDataBinding(data);
				boundToDataSource = true;
			}
		}

		internal void AutoDataBind(bool forceBinding)
		{
			if (!((!boundToDataSource && !IsDesignMode()) || forceBinding))
			{
				return;
			}
			boundToDataSource = true;
			foreach (InputValue value in Values)
			{
				value.AutoDataBind();
			}
		}

		internal static bool IsValidDataSource(object dataSource)
		{
			if (dataSource is IEnumerable || dataSource is DataSet || dataSource is DataView || dataSource is DataTable || dataSource is OleDbCommand || dataSource is SqlCommand || dataSource is OleDbDataAdapter || dataSource is SqlDataAdapter || dataSource.GetType().GetInterface("IDataSource") != null)
			{
				return true;
			}
			return false;
		}

		internal string ResolveAllKeywords(string original, NamedElement element)
		{
			if (original.Length == 0)
			{
				return original;
			}
			string text = original;
			text = text.Replace("\\n", "\n");
			int num = 0;
			foreach (InputValue value in Values)
			{
				string keyword = "#INPUTVALUE" + num.ToString(CultureInfo.InvariantCulture);
				text = ResolveKeyword(text, keyword, value.Value);
				num++;
			}
			if (element is CircularGauge)
			{
				CircularGauge obj = (CircularGauge)element;
				num = 0;
				foreach (CircularPointer pointer in obj.Pointers)
				{
					string keyword2 = "#POINTERVALUE" + num.ToString(CultureInfo.InvariantCulture);
					text = ResolveKeyword(text, keyword2, pointer.Value);
					num++;
				}
			}
			else if (element is LinearGauge)
			{
				LinearGauge obj2 = (LinearGauge)element;
				num = 0;
				foreach (LinearPointer pointer2 in obj2.Pointers)
				{
					string keyword3 = "#POINTERVALUE" + num.ToString(CultureInfo.InvariantCulture);
					text = ResolveKeyword(text, keyword3, pointer2.Value);
					num++;
				}
			}
			if (element is CircularPointer)
			{
				text = ResolveKeyword(text, "#VALUE", ((CircularPointer)element).Value);
			}
			else if (element is LinearPointer)
			{
				text = ResolveKeyword(text, "#VALUE", ((LinearPointer)element).Value);
			}
			else if (element is NumericIndicator)
			{
				text = ResolveKeyword(text, "#VALUE", ((NumericIndicator)element).Value);
			}
			else if (element is StateIndicator)
			{
				text = ResolveKeyword(text, "#VALUE", ((StateIndicator)element).Value);
			}
			if (element is StateIndicator)
			{
				text = text.Replace("#STATE", ((StateIndicator)element).CurrentState);
			}
			return text;
		}

		internal string ResolveKeyword(string original, string keyword, double val)
		{
			string text = original;
			int num = -1;
			while ((num = text.IndexOf(keyword, StringComparison.Ordinal)) != -1)
			{
				int num2 = num + keyword.Length;
				string format = string.Empty;
				if (text.Length > num2 && text[num2] == '{')
				{
					int num3 = text.IndexOf('}', num2);
					if (num3 == -1)
					{
						throw new InvalidOperationException(Utils.SRGetStr("ExceptionInvalidKeywordFormat", text));
					}
					format = text.Substring(num2, num3 - num2).Trim('{', '}');
					num2 = num3 + 1;
				}
				text = text.Remove(num, num2 - num);
				string empty = string.Empty;
				empty = ((GaugeContainer.FormatNumberHandler == null) ? val.ToString(format, CultureInfo.CurrentCulture) : GaugeContainer.FormatNumberHandler(GaugeContainer, val, format));
				text = text.Insert(num, empty);
			}
			return text;
		}

		internal void TraceWrite(string category, string message)
		{
			if (serviceContainer != null)
			{
				((TraceManager)serviceContainer.GetService(typeof(TraceManager)))?.Write(category, message);
			}
		}

		internal object GetService(Type serviceType)
		{
			object result = null;
			if (serviceContainer != null)
			{
				result = serviceContainer.GetService(serviceType);
			}
			return result;
		}

		protected override void OnDispose()
		{
			if (inputValues != null)
			{
				inputValues.Dispose();
			}
			NamedCollection[] renderCollections = GetRenderCollections();
			for (int i = 0; i < renderCollections.Length; i++)
			{
				renderCollections[i].Dispose();
			}
			if (bmpGauge != null)
			{
				bmpGauge.Dispose();
			}
			if (bmpFaces != null)
			{
				bmpFaces.Dispose();
			}
			if (namedImages != null)
			{
				namedImages.Dispose();
			}
			if (imageLoader != null)
			{
				imageLoader.Dispose();
			}
		}

		internal static bool CheckLicense()
		{
			bool result = false;
			try
			{
				string str = "SOFTWARE\\Dundas Software\\Gauges\\WebControl";
				str += "VS2005";
				string str2 = "Microsoft.Reporting.Gauge.WebForms.GaugeContainer.lic";
				return result;
			}
			catch
			{
				return result;
			}
		}

		void ISelectable.DrawSelection(GaugeGraphics g, bool designTimeSelection)
		{
			g.DrawSelection(new RectangleF(0f, 0f, GetWidth() - 1, GetHeight() - 1), -3f / g.Graphics.PageScale, designTimeSelection, Common.GaugeCore.SelectionBorderColor, Common.GaugeCore.SelectionMarkerColor);
		}
	}
}

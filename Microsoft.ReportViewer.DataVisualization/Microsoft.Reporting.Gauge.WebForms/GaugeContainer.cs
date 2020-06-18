using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[DisplayName("Dundas Gauge")]
	[SRDescription("DescriptionAttributeGaugeContainer_GaugeContainer")]
	internal class GaugeContainer : IDisposable
	{
		private const string smartClientDll = "DundasWinGauge.dll";

		private const string jsFilename = "DundasGauge1.js";

		internal GaugeCore gauge;

		internal string webFormDocumentURL = "";

		internal string applicationDocumentURL = "";

		internal static ITypeDescriptorContext controlCurrentContext = null;

		private string cachedImageUrl = string.Empty;

		internal static string productID = "DG-WC";

		private bool pollServer;

		private Color backColor = Color.White;

		private int width = 320;

		private int height = 240;

		private bool enabled = true;

		public FormatNumberHandler FormatNumberHandler;

		[Category("Data")]
		[SRDescription("DescriptionAttributeGaugeContainer_Values")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public InputValueCollection Values => gauge.Values;

		[Category("Gauge Container")]
		[SRDescription("DescriptionAttributeGaugeContainer_CircularGauges")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CircularGaugeCollection CircularGauges => gauge.CircularGauges;

		[Category("Gauge Container")]
		[SRDescription("DescriptionAttributeGaugeContainer_LinearGauges")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearGaugeCollection LinearGauges => gauge.LinearGauges;

		[Category("Gauge Container")]
		[SRDescription("DescriptionAttributeGaugeContainer_NumericIndicators")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NumericIndicatorCollection NumericIndicators => gauge.NumericIndicators;

		[Category("Gauge Container")]
		[SRDescription("DescriptionAttributeGaugeContainer_StateIndicators")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public StateIndicatorCollection StateIndicators => gauge.StateIndicators;

		[Category("Gauge Container")]
		[SRDescription("DescriptionAttributeGaugeContainer_Images")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GaugeImageCollection Images => gauge.Images;

		[Category("Gauge Container")]
		[SRDescription("DescriptionAttributeGaugeContainer_Labels")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GaugeLabelCollection Labels => gauge.Labels;

		[Browsable(false)]
		[Category("Gauge Container")]
		[SRDescription("DescriptionAttributeGaugeContainer_NamedImages")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public NamedImageCollection NamedImages => gauge.NamedImages;

		[Category("Image")]
		[SRDescription("DescriptionAttributeGaugeContainer_MapAreas")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MapAreaCollection MapAreas => gauge.MapAreas;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public string LicenseData
		{
			get
			{
				return gauge.licenseData;
			}
			set
			{
				gauge.licenseData = value;
			}
		}

		[Description("Separator to be used with the multiple value parameters.")]
		public string MultiValueSeparator
		{
			get
			{
				return gauge.MultiValueSeparator;
			}
			set
			{
				gauge.MultiValueSeparator = value;
			}
		}

		public float ImageResolution
		{
			get
			{
				return gauge.ImageResolution;
			}
			set
			{
				gauge.ImageResolution = value;
			}
		}

		[Category("Gauge Behavior")]
		[SRDescription("DescriptionAttributeGaugeContainer_RefreshRate")]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		[DefaultValue(30.0)]
		public double RefreshRate
		{
			get
			{
				return gauge.RefreshRate;
			}
			set
			{
				if (value < 0.0 || value > 100.0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
				}
				gauge.RefreshRate = value;
			}
		}

		[Category("Image")]
		[SRDescription("DescriptionAttributeGaugeContainer_MapEnabled")]
		[DefaultValue(true)]
		public bool MapEnabled
		{
			get
			{
				return gauge.MapEnabled;
			}
			set
			{
				gauge.MapEnabled = value;
			}
		}

		[Category("Gauge Behavior")]
		[SRDescription("DescriptionAttributeGaugeContainer_AutoLayout")]
		[DefaultValue(true)]
		public bool AutoLayout
		{
			get
			{
				return gauge.AutoLayout;
			}
			set
			{
				gauge.AutoLayout = value;
			}
		}

		[Category("Appearance")]
		[SRDescription("DescriptionAttributeGaugeContainer_ShadowIntensity")]
		[ValidateBound(0.0, 100.0)]
		[DefaultValue(25f)]
		public float ShadowIntensity
		{
			get
			{
				return gauge.ShadowIntensity;
			}
			set
			{
				if (value > 100f || value < 0f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 100));
				}
				gauge.ShadowIntensity = value;
			}
		}

		[Category("Appearance")]
		[SRDescription("DescriptionAttributeGaugeContainer_BackFrame")]
		[NotifyParentProperty(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BackFrame BackFrame
		{
			get
			{
				return gauge.BackFrame;
			}
			set
			{
				gauge.BackFrame = value;
			}
		}

		[Category("Appearance")]
		[SRDescription("DescriptionAttributeGaugeContainer_TopImage")]
		[DefaultValue("")]
		public string TopImage
		{
			get
			{
				return gauge.TopImage;
			}
			set
			{
				gauge.TopImage = value;
			}
		}

		[Category("Appearance")]
		[SRDescription("DescriptionAttributeTopImageTransColor")]
		[DefaultValue(typeof(Color), "")]
		public Color TopImageTransColor
		{
			get
			{
				return gauge.TopImageTransColor;
			}
			set
			{
				gauge.TopImageTransColor = value;
			}
		}

		[Category("Appearance")]
		[SRDescription("DescriptionAttributeTopImageHueColor")]
		[DefaultValue(typeof(Color), "")]
		public Color TopImageHueColor
		{
			get
			{
				return gauge.TopImageHueColor;
			}
			set
			{
				gauge.TopImageHueColor = value;
			}
		}

		[Category("Appearance")]
		[SRDescription("DescriptionAttributeGaugeContainer_AntiAliasing")]
		[DefaultValue(typeof(AntiAliasing), "All")]
		public AntiAliasing AntiAliasing
		{
			get
			{
				return gauge.AntiAliasing;
			}
			set
			{
				gauge.AntiAliasing = value;
			}
		}

		[Category("Appearance")]
		[SRDescription("DescriptionAttributeGaugeContainer_TextAntiAliasingQuality")]
		[DefaultValue(TextAntiAliasingQuality.High)]
		public TextAntiAliasingQuality TextAntiAliasingQuality
		{
			get
			{
				return gauge.TextAntiAliasingQuality;
			}
			set
			{
				gauge.TextAntiAliasingQuality = value;
			}
		}

		[Category("Appearance")]
		[SRDescription("DescriptionAttributeGaugeContainer_RightToLeft")]
		[DefaultValue(RightToLeft.No)]
		public RightToLeft RightToLeft
		{
			get
			{
				return gauge.RightToLeft;
			}
			set
			{
				gauge.RightToLeft = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public GaugeSerializer Serializer => gauge.Serializer;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public CallbackManager CallbackManager => gauge.CallbackManager;

		[Category("Data")]
		[SRDescription("DescriptionAttributeGaugeContainer_RealTimeDataInterval")]
		[DefaultValue(1f)]
		public float RealTimeDataInterval
		{
			get
			{
				return gauge.RealTimeDataInterval;
			}
			set
			{
				if (value < 0f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionNegativeValue"));
				}
				gauge.RealTimeDataInterval = value;
			}
		}

		[Category("ViewState")]
		[DefaultValue(SerializationContent.All)]
		[SRDescription("DescriptionAttributeGaugeContainer_ViewStateContent")]
		public SerializationContent ViewStateContent
		{
			get
			{
				return gauge.ViewStateContent;
			}
			set
			{
				gauge.ViewStateContent = value;
			}
		}

		[Category("Image")]
		[DefaultValue(ImageType.Png)]
		[SRDescription("DescriptionAttributeGaugeContainer_ImageType")]
		public ImageType ImageType
		{
			get
			{
				return gauge.ImageType;
			}
			set
			{
				gauge.ImageType = value;
			}
		}

		[Category("Image")]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeGaugeContainer_Compression")]
		public int Compression
		{
			get
			{
				return gauge.Compression;
			}
			set
			{
				gauge.Compression = value;
			}
		}

		[Category("Image")]
		[SRDescription("DescriptionAttributeGaugeContainer_ImageUrl")]
		[DefaultValue("TempFiles/GaugePic_#SEQ(300,3)")]
		public string ImageUrl
		{
			get
			{
				return gauge.ImageUrl;
			}
			set
			{
				if (value.IndexOf("#SEQ", StringComparison.Ordinal) > 0)
				{
					CheckImageUrlSeqFormat(value);
				}
				gauge.ImageUrl = value;
			}
		}

		[Category("Image")]
		[SRDescription("DescriptionAttributeGaugeContainer_RenderType")]
		[DefaultValue(RenderType.ImageTag)]
		public RenderType RenderType
		{
			get
			{
				return gauge.RenderType;
			}
			set
			{
				gauge.RenderType = value;
			}
		}

		[Category("Image")]
		[SRDescription("DescriptionAttributeGaugeContainer_TransparentColor")]
		[DefaultValue(typeof(Color), "")]
		public Color TransparentColor
		{
			get
			{
				return gauge.TransparentColor;
			}
			set
			{
				gauge.TransparentColor = value;
			}
		}

		[Category("Smart Client")]
		[SRDescription("DescriptionAttributeGaugeContainer_RenderAsControl")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(AutoBool.False)]
		public AutoBool RenderAsControl
		{
			get
			{
				return gauge.RenderAsControl;
			}
			set
			{
				gauge.RenderAsControl = value;
			}
		}

		[Category("Smart Client")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeGaugeContainer_WinControlUrl")]
		public string WinControlUrl
		{
			get
			{
				return gauge.WinControlUrl;
			}
			set
			{
				gauge.WinControlUrl = value;
			}
		}

		[Category("Smart Client")]
		[SRDescription("DescriptionAttributeGaugeContainer_LoadingDataText")]
		[Localizable(true)]
		[DefaultValue("Loading...")]
		public string LoadingDataText
		{
			get
			{
				return gauge.LoadingDataText;
			}
			set
			{
				gauge.LoadingDataText = value;
			}
		}

		[Category("Smart Client")]
		[SRDescription("DescriptionAttributeGaugeContainer_LoadingDataImage")]
		[DefaultValue("")]
		public string LoadingDataImage
		{
			get
			{
				return gauge.LoadingDataImage;
			}
			set
			{
				gauge.LoadingDataImage = value;
			}
		}

		[Category("Smart Client")]
		[SRDescription("DescriptionAttributeGaugeContainer_LoadingDataImage")]
		[DefaultValue("")]
		public string LoadingControlImage
		{
			get
			{
				return gauge.LoadingControlImage;
			}
			set
			{
				gauge.LoadingControlImage = value;
			}
		}

		[Category("Image")]
		[SRDescription("DescriptionAttributeGaugeContainer_TagAttributes")]
		[DefaultValue("")]
		public string TagAttributes
		{
			get
			{
				return gauge.TagAttributes;
			}
			set
			{
				gauge.TagAttributes = value;
			}
		}

		[Category("Appearance")]
		[SRDescription("DescriptionAttributeGaugeContainer_SelectionMarkerColor")]
		[DefaultValue(typeof(Color), "LightBlue")]
		public Color SelectionMarkerColor
		{
			get
			{
				return gauge.SelectionMarkerColor;
			}
			set
			{
				gauge.SelectionMarkerColor = value;
			}
		}

		[Category("Appearance")]
		[SRDescription("DescriptionAttributeGaugeContainer_SelectionBorderColor")]
		[DefaultValue(typeof(Color), "Black")]
		public Color SelectionBorderColor
		{
			get
			{
				return gauge.SelectionBorderColor;
			}
			set
			{
				gauge.SelectionBorderColor = value;
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeContainer_BorderColor")]
		[DefaultValue(typeof(Color), "Black")]
		public Color BorderColor
		{
			get
			{
				return gauge.BorderColor;
			}
			set
			{
				gauge.BorderColor = value;
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeContainer_BorderStyle")]
		[DefaultValue(GaugeDashStyle.NotSet)]
		public GaugeDashStyle BorderStyle
		{
			get
			{
				return gauge.BorderStyle;
			}
			set
			{
				gauge.BorderStyle = value;
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeContainer_BorderWidth")]
		[DefaultValue(1)]
		public int BorderWidth
		{
			get
			{
				return gauge.BorderWidth;
			}
			set
			{
				gauge.BorderWidth = value;
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
				Invalidate();
			}
		}

		[Category("Image")]
		[DefaultValue(320)]
		[SRDescription("DescriptionAttributeGaugeContainer_Width")]
		public int Width
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

		[Category("Image")]
		[DefaultValue(240)]
		[SRDescription("DescriptionAttributeGaugeContainer_Height")]
		public int Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
				Invalidate();
			}
		}

		[Category("Image")]
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
				Invalidate();
			}
		}

		internal ISelectable SelectedDesignTimeElement
		{
			get
			{
				return gauge.SelectedDesignTimeElement;
			}
			set
			{
				gauge.SelectedDesignTimeElement = value;
			}
		}

		[Category("Behavior")]
		[Description("Provides a tooltip to be displayed on the rendered image.")]
		[DefaultValue("")]
		public string ToolTip
		{
			get
			{
				return gauge.ToolTip;
			}
			set
			{
				gauge.ToolTip = value;
			}
		}

		[DefaultValue(false)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool EnableTheming
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DefaultValue("")]
		public string SkinID
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeGaugeContainerEvent_PrePaint")]
		[Category("Appearance")]
		public event GaugePaintEventHandler PrePaint;

		[SRDescription("DescriptionAttributeGaugeContainerEvent_PostPaint")]
		[Category("Appearance")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event GaugePaintEventHandler PostPaint;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeGaugeContainerEvent_RealTimeData")]
		[Category("Data")]
		public event RealTimeDataEventHandler RealTimeData;

		[Category("Action")]
		[SRDescription("DescriptionAttributeGaugeContainerEvent_Callback")]
		public event CallbackEventHandler Callback;

		public GaugeContainer()
		{
			gauge = new GaugeCore(this);
			gauge.BeginInit();
			BackColor = Color.White;
			Width = 320;
			Height = 240;
			BackColor = Color.White;
		}

		~GaugeContainer()
		{
			Dispose();
		}

		public void Dispose()
		{
			gauge.Dispose();
			GC.SuppressFinalize(this);
		}

		private void SaveAsImage(string fileName, GaugeImageFormat format)
		{
			SaveAsImage(fileName, format, Compression);
		}

		private void SaveAsImage(string fileName, GaugeImageFormat format, int compression)
		{
			gauge.SaveTo(fileName, format, compression, ImageResolution, ImageResolution);
		}

		internal void SaveAsImage(Stream stream, GaugeImageFormat format)
		{
			SaveAsImage(stream, format, Compression);
		}

		private void SaveAsImage(Stream stream, GaugeImageFormat format, int compression)
		{
			gauge.SaveTo(stream, format, compression, ImageResolution, ImageResolution);
			if (gauge.MapEnabled)
			{
				gauge.PopulateImageMaps();
			}
		}

		public void SaveAsImage(Stream stream)
		{
			GaugeImageFormat imageFormat = (GaugeImageFormat)Enum.Parse(typeof(GaugeImageFormat), ImageType.ToString(), ignoreCase: true);
			gauge.SaveTo(stream, imageFormat, Compression, ImageResolution, ImageResolution);
		}

		private void SaveAsImage(string fileName)
		{
			GaugeImageFormat imageFormat = (GaugeImageFormat)Enum.Parse(typeof(GaugeImageFormat), ImageType.ToString(), ignoreCase: true);
			gauge.SaveTo(fileName, imageFormat, Compression, ImageResolution, ImageResolution);
		}

		private void PrintPaint(Graphics graphics, Rectangle position)
		{
			gauge.PrintPaint(graphics, position);
		}

		public RectangleF GetAbsoluteRectangle(NamedElement element, RectangleF relativeRect)
		{
			return gauge.GetHotRegion(element).GetAbsoluteRectangle(relativeRect);
		}

		public RectangleF GetRelativeRectangle(NamedElement element, RectangleF absoluteRect)
		{
			return gauge.GetHotRegion(element).GetRelativeRectangle(absoluteRect);
		}

		public PointF GetAbsolutePoint(NamedElement element, PointF relativePoint)
		{
			return gauge.GetHotRegion(element).GetAbsolutePoint(relativePoint);
		}

		public PointF GetRelativePoint(NamedElement element, PointF absolutePoint)
		{
			return gauge.GetHotRegion(element).GetRelativePoint(absolutePoint);
		}

		public SizeF GetAbsoluteSize(NamedElement element, SizeF relativeSize)
		{
			return gauge.GetHotRegion(element).GetAbsoluteSize(relativeSize);
		}

		public SizeF GetRelativeSize(NamedElement element, SizeF absoluteSize)
		{
			return gauge.GetHotRegion(element).GetRelativeSize(absoluteSize);
		}

		public NamedElement[] GetSelectedElements()
		{
			return gauge.GetSelectedElements();
		}

		private string GetImageRefreshScript(string gaugeContainerID, float refreshInterval)
		{
			return ("\n<script language=\"javascript\">\n<!--\nvar GaugeContainer1 = new Object();\nGaugeContainer1.refreshInterval = " + ((int)(refreshInterval * 1000f)).ToString(CultureInfo.InvariantCulture) + ";\nif (navigator.appName == \"Microsoft Internet Explorer\")\n    GaugeContainer1.bufferImage = document.images[\"GaugeContainer1\"].cloneNode();\nelse\n    GaugeContainer1.bufferImage = new Image();\nGaugeContainer1.bufferImage.src = GetNewGaugeContainer1Url();\nUpdateGaugeContainer1();\n\nfunction GetNewGaugeContainer1Url()\n{\n    var now = new Date();\n    if (location.href.indexOf(\"?\") == -1)\n        return location.href + \"?_gaugeContainerID=GaugeContainer1&_time=\" + now.getTime() + now.getMilliseconds();\n    else\n        return location.href + \"&_gaugeContainerID=GaugeContainer1&_time=\" + now.getTime() + now.getMilliseconds();\n}\n\nfunction UpdateGaugeContainer1()\n{\n    var isExplorer = navigator.appName == \"Microsoft Internet Explorer\";\n    var isBufferLoaded;\n    if (isExplorer)\n        isBufferLoaded = GaugeContainer1.bufferImage.readyState == \"complete\";\n    else\n        isBufferLoaded = GaugeContainer1.bufferImage.complete;\n    if (isBufferLoaded)\n    {\n        if (isExplorer)\n        {\n            GaugeContainer1.bufferImage.swapNode(document.images[\"GaugeContainer1\"]);\n            GaugeContainer1.bufferImage = document.images[\"GaugeContainer1\"].cloneNode();\n        }\n        else\n        {\n            document.images[\"GaugeContainer1\"].src = GaugeContainer1.bufferImage.src;\n            GaugeContainer1.bufferImage = new Image();\n        }\n\n        GaugeContainer1.bufferImage.src = GetNewGaugeContainer1Url();\n    }\n\n    setTimeout(\"UpdateGaugeContainer1()\", GaugeContainer1.refreshInterval);\n}\n-->\n</script>\n").Replace("GaugeContainer1", gaugeContainerID);
		}

		private string GetFlashRefreshScript(string gaugeContainerID, float refreshInterval, bool isIE)
		{
			int num = (int)(refreshInterval * 1000f);
			string text = (!isIE) ? ("\n<script language=\"javascript\">\n<!--\nvar GaugeContainer1 = new Object();\nGaugeContainer1.refreshInterval = " + num.ToString(CultureInfo.InvariantCulture) + ";\nGaugeContainer1.bufferImage = new Image();\nGaugeContainer1.bufferImage.src = GetNewGaugeContainer1Url();\nif (document.GaugeContainer1.PercentLoaded() == 100)\n    document.GaugeContainer1.LoadMovie(1, GaugeContainer1.bufferImage.src); \nif (document.GaugeContainer1Buffer.PercentLoaded() == 100)\n    document.GaugeContainer1Buffer.LoadMovie(1, GaugeContainer1.bufferImage.src);\nUpdateGaugeContainer1();\n\nfunction GetNewGaugeContainer1Url()\n{\n    var now = new Date();\n    if (location.href.indexOf(\"?\") == -1)\n        return location.href + \"?___gaugeContainerID=GaugeContainer1&_time=\" + now.getTime() + now.getMilliseconds();\n    else\n        return location.href + \"&___gaugeContainerID=GaugeContainer1&_time=\" + now.getTime() + now.getMilliseconds();\n}\n\nfunction UpdateGaugeContainer1()\n{\n    if (GaugeContainer1.bufferImage.complete)\n    {\n        if (document.GaugeContainer1.width == 0)\n        {\n            document.GaugeContainer1.width = document.GaugeContainer1Buffer.width;\n            document.GaugeContainer1.height = document.GaugeContainer1Buffer.height;\n            document.GaugeContainer1Buffer.width = 0;\n            document.GaugeContainer1Buffer.height = 0;\n            if (document.GaugeContainer1Buffer.PercentLoaded() == 100)\n                document.GaugeContainer1Buffer.LoadMovie(1, GaugeContainer1.bufferImage.src);\n        }\n        else\n        {\n            document.GaugeContainer1Buffer.width = document.GaugeContainer1.width;\n            document.GaugeContainer1Buffer.height = document.GaugeContainer1.height;\n            document.GaugeContainer1.width = 0;\n            document.GaugeContainer1.height = 0;\n            if (document.GaugeContainer1.PercentLoaded() == 100)\n                document.GaugeContainer1.LoadMovie(1, GaugeContainer1.bufferImage.src);\n        }\n    }\n    setTimeout(\"UpdateGaugeContainer1()\", GaugeContainer1.refreshInterval);\n}\n-->\n</script>\n") : ("\n<script language=\"javascript\">\n<!--\nvar GaugeContainer1 = new Object();\nInitializeGaugeContainer1();\n\nfunction InitializeGaugeContainer1()\n{\n    if (!GaugeContainer1.initialized)\n    {\n        if (document.GaugeContainer1.ReadyState == 4 && document._GaugeContainer1.ReadyState == 4)\n        {\n            GaugeContainer1.refreshInterval = " + num.ToString(CultureInfo.InvariantCulture) + ";\n            document.GaugeContainer1.LoadMovie(1, GetNewGaugeContainer1Url());\n            document._GaugeContainer1.LoadMovie(1, GetNewGaugeContainer1Url());\n            document.GaugeContainer1Buffer.Movie = GetNewGaugeContainer1Url();\n            GaugeContainer1.initialized = true;\n            UpdateGaugeContainer1();\n        }\n        else\n        {\n            setTimeout(\"InitializeGaugeContainer1()\", " + num.ToString(CultureInfo.InvariantCulture) + ");\n        }\n    }\n}\n\nfunction GetNewGaugeContainer1Url()\n{\n    var now = new Date();\n    if (location.href.indexOf(\"?\") == -1)\n        return location.href + \"?___gaugeContainerID=GaugeContainer1&_time=\" + now.getTime() + now.getMilliseconds();\n    else\n        return location.href + \"&___gaugeContainerID=GaugeContainer1&_time=\" + now.getTime() + now.getMilliseconds();\n}\n\nfunction UpdateGaugeContainer1()\n{\n    if (document.GaugeContainer1Buffer.ReadyState == 4)\n    {\n        if (document.GaugeContainer1.style.display == \"none\")\n        {\n            document.GaugeContainer1.style.width = document._GaugeContainer1.style.width;\n            document.GaugeContainer1.style.heigth = document._GaugeContainer1.style.heigth;\n            document._GaugeContainer1.style.width = 0;\n            document._GaugeContainer1.style.heigth = 0;\n            document._GaugeContainer1.style.display = \"none\";\n            document.GaugeContainer1.style.display = \"\";\n            document._GaugeContainer1.LoadMovie(1, document.GaugeContainer1Buffer.Movie);\n        }\n        else\n        {\n            document._GaugeContainer1.style.width = document.GaugeContainer1.style.width;\n            document._GaugeContainer1.style.heigth = document.GaugeContainer1.style.heigth;\n            document.GaugeContainer1.style.width = 0;\n            document.GaugeContainer1.style.heigth = 0;\n            document.GaugeContainer1.style.display = \"none\";\n            document._GaugeContainer1.style.display = \"\";\n            document.GaugeContainer1.LoadMovie(1, document.GaugeContainer1Buffer.Movie);\n        }\n        document.GaugeContainer1Buffer.Movie = GetNewGaugeContainer1Url();\n    }\n    setTimeout(\"UpdateGaugeContainer1()\", GaugeContainer1.refreshInterval);\n}\n-->\n</script>\n");
			return text.Replace("GaugeContainer1", gaugeContainerID);
		}

		internal void SaveFiles(string fullImagePath)
		{
			string directoryName = Path.GetDirectoryName(fullImagePath);
			if (!new DirectoryInfo(directoryName).Exists)
			{
				Directory.CreateDirectory(directoryName);
			}
			string text = directoryName + "\\DundasGauge1.js";
			if (!new FileInfo(text).Exists)
			{
				Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(GaugeContainer).Namespace + ".DundasGauge.js");
				byte[] array = new byte[manifestResourceStream.Length];
				manifestResourceStream.Read(array, 0, array.Length);
				string value = ObfuscateJavaScript(Encoding.UTF8.GetString(array));
				StreamWriter streamWriter = File.CreateText(text);
				streamWriter.Write(value);
				streamWriter.Close();
			}
			SaveAsImage(fullImagePath);
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

		internal void Invalidate()
		{
		}

		internal void Refresh()
		{
		}

		private void CheckImageUrlSeqFormat(string imageUrl)
		{
			int num = imageUrl.IndexOf("#SEQ", StringComparison.Ordinal);
			num += 4;
			if (imageUrl[num] != '(')
			{
				throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidImageFormat"), "imageUrl");
			}
			int num2 = imageUrl.IndexOf(')', 1);
			if (num2 < 0)
			{
				throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidImageFormat"), "imageUrl");
			}
			string[] array = imageUrl.Substring(num + 1, num2 - num - 1).Split(',');
			if (array == null || array.Length != 2)
			{
				throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidImageFormat"), "imageUrl");
			}
			string[] array2 = array;
			foreach (string text in array2)
			{
				for (int j = 0; j < text.Length; j++)
				{
					if (!char.IsDigit(text[j]))
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidImageFormat"), "imageUrl");
					}
				}
			}
		}

		internal bool IsDesignMode()
		{
			return false;
		}

		public void SaveXml(string fileName)
		{
			try
			{
				Serializer.Save(fileName);
			}
			catch
			{
			}
		}

		internal void OnPrePaint(object sender, GaugePaintEventArgs e)
		{
			if (this.PrePaint != null)
			{
				this.PrePaint(sender, e);
			}
		}

		internal void OnPostPaint(object sender, GaugePaintEventArgs e)
		{
			if (this.PostPaint != null)
			{
				this.PostPaint(sender, e);
			}
		}

		public void RaisePostBackEvent(string eventArgument)
		{
		}

		internal void OnRealTimeData(object sender, RealTimeDataEventArgs e)
		{
			if (this.RealTimeData != null)
			{
				this.RealTimeData(sender, e);
			}
		}

		internal void OnCallback(object sender, CallbackEventArgs e)
		{
			if (this.Callback != null)
			{
				this.Callback(sender, e);
			}
		}

		internal void OnValueChanged(object sender, ValueChangedEventArgs e)
		{
		}

		internal void OnPlaybackStateChanged(object sender, PlaybackStateChangedEventArgs e)
		{
		}

		internal void OnValueLimitOverflow(object sender, ValueChangedEventArgs e)
		{
		}

		internal void OnValueRateOfChangeExceed(object sender, ValueChangedEventArgs e)
		{
		}

		internal void OnValueRangeEnter(object sender, ValueRangeEventArgs e)
		{
		}

		internal void OnValueRangeLeave(object sender, ValueRangeEventArgs e)
		{
		}

		internal void OnValueRangeTimeOut(object sender, ValueRangeEventArgs e)
		{
		}

		internal void OnValueScaleEnter(object sender, ValueRangeEventArgs e)
		{
		}

		internal void OnValueScaleLeave(object sender, ValueRangeEventArgs e)
		{
		}

		internal void OnPointerPositionChange(object sender, PointerPositionChangeEventArgs e)
		{
		}
	}
}

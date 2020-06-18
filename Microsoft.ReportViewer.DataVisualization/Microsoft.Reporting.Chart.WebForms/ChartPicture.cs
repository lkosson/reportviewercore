using Microsoft.Reporting.Chart.WebForms.Borders3D;
using Microsoft.Reporting.Chart.WebForms.Svg;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class ChartPicture : IServiceProvider
	{
		private bool suppressExceptions;

		internal ChartGraphics chartGraph;

		internal bool backgroundRestored;

		private IServiceContainer serviceContainer;

		private ChartAreaCollection chartAreas;

		internal Legend legend = new Legend();

		private Color titleFontColor = Color.Black;

		internal static FontCache fontCache = new FontCache();

		private Font titleFont = new Font(GetDefaultFontFamilyName(), 8f);

		internal CommonElements common;

		private GradientType backGradientType;

		private Color backGradientEndColor = Color.Empty;

		private Color backColor = Color.White;

		private string backImage = "";

		private ChartImageWrapMode backImageMode;

		private Color backImageTranspColor = Color.Empty;

		private ChartImageAlign backImageAlign;

		private Color borderColor = Color.White;

		private int borderWidth = 1;

		private ChartDashStyle borderStyle;

		private ChartHatchStyle backHatchStyle;

		private AntiAlias antiAlias;

		private AntiAliasingTypes antiAliasing = AntiAliasingTypes.All;

		private TextAntiAliasingQuality textAntiAliasingQuality = TextAntiAliasingQuality.High;

		private bool softShadows = true;

		private int width = 300;

		private int height = 300;

		private DataManipulator dataManipulator = new DataManipulator();

		internal HotRegionsList hotRegionsList;

		private BorderSkinAttributes borderSkin;

		private bool mapEnabled = true;

		private MapAreasCollection mapAreas;

		private LegendCollection legends;

		private TitleCollection titles;

		private AnnotationCollection annotations;

		internal AnnotationSmartLabels annotationSmartLabels = new AnnotationSmartLabels();

		internal bool showWaterMark;

		private RectangleF titlePosition = RectangleF.Empty;

		internal const float elementSpacing = 3f;

		internal const float maxTitleSize = 15f;

		internal float legendMaxAutoSize = 50f;

		internal bool isPrinting;

		internal bool isSavingAsImage;

		internal bool isSelectionMode;

		private static string defaultFontFamilyName = string.Empty;

		private RectangleF chartBorderPosition = RectangleF.Empty;

		private SelectionManager selectorManager;

		private RightToLeft rightToLeft;

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
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeSuppressExceptions")]
		internal bool SuppressExceptions
		{
			get
			{
				return suppressExceptions;
			}
			set
			{
				suppressExceptions = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(BorderSkinStyle.None)]
		[SRDescription("DescriptionAttributeBorderSkinAttributes")]
		public BorderSkinAttributes BorderSkinAttributes
		{
			get
			{
				return borderSkin;
			}
			set
			{
				borderSkin = value;
			}
		}

		[SRCategory("CategoryAttributeMap")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMapEnabled")]
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

		[SRCategory("CategoryAttributeMap")]
		[SRDescription("DescriptionAttributeMapAreas")]
		public MapAreasCollection MapAreas => mapAreas;

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeChartAreas")]
		public ChartAreaCollection ChartAreas => chartAreas;

		[SRCategory("CategoryAttributeChart")]
		[SRDescription("DescriptionAttributeLegends")]
		public LegendCollection Legends => legends;

		[SRCategory("CategoryAttributeCharttitle")]
		[SRDescription("DescriptionAttributeTitles")]
		public TitleCollection Titles => titles;

		[SRCategory("CategoryAttributeChart")]
		[SRDescription("DescriptionAttributeAnnotations3")]
		public AnnotationCollection Annotations => annotations;

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue("White")]
		[SRDescription("DescriptionAttributeBackColor5")]
		public Color BackColor
		{
			get
			{
				return backColor;
			}
			set
			{
				if (!(value == Color.Empty) && value.A == byte.MaxValue)
				{
					_ = (value == Color.Transparent);
				}
				backColor = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue("White")]
		[SRDescription("DescriptionAttributeBorderColor")]
		public Color BorderColor
		{
			get
			{
				return borderColor;
			}
			set
			{
				borderColor = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(300)]
		[SRDescription("DescriptionAttributeWidth")]
		public int Width
		{
			get
			{
				return width;
			}
			set
			{
				if (value < 5)
				{
					throw new ArgumentOutOfRangeException("Width", SR.ExceptionChartWidthLessThen5Pixels);
				}
				width = value;
				common.Width = width;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeLegend")]
		public Legend Legend
		{
			get
			{
				int index = legends.GetIndex("Default");
				if (index >= 0)
				{
					return legends[index];
				}
				return legend;
			}
			set
			{
				value.Name = "Default";
				value.Common = common;
				value.CustomItems.common = common;
				int index = legends.GetIndex("Default");
				if (index >= 0)
				{
					legends[index] = value;
				}
				else
				{
					legends.Insert(0, value);
				}
			}
		}

		[SRCategory("CategoryAttributeData")]
		[SRDescription("DescriptionAttributeDataManipulator")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public DataManipulator DataManipulator => dataManipulator;

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(300)]
		[SRDescription("DescriptionAttributeHeight3")]
		public int Height
		{
			get
			{
				return height;
			}
			set
			{
				if (value < 5)
				{
					throw new ArgumentOutOfRangeException("Height", SR.ExceptionChartHeightLessThen5Pixels);
				}
				height = value;
				common.Height = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeTitle5")]
		public string Title
		{
			get
			{
				Title defaultTitle = GetDefaultTitle(create: false);
				if (defaultTitle != null && (common == null || !common.Chart.serializing))
				{
					return defaultTitle.Text;
				}
				return "";
			}
			set
			{
				Title defaultTitle = GetDefaultTitle(create: true);
				defaultTitle.Text = value;
				defaultTitle.Color = titleFontColor;
				defaultTitle.Font = titleFont;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue("Black")]
		[SRDescription("DescriptionAttributeTitleFontColor")]
		public Color TitleFontColor
		{
			get
			{
				Title defaultTitle = GetDefaultTitle(create: false);
				if (defaultTitle != null && (common == null || !common.Chart.serializing))
				{
					return defaultTitle.Color;
				}
				return Color.Black;
			}
			set
			{
				titleFontColor = value;
				Title defaultTitle = GetDefaultTitle(create: false);
				if (defaultTitle != null)
				{
					defaultTitle.Color = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeTitleFont4")]
		public Font TitleFont
		{
			get
			{
				Title defaultTitle = GetDefaultTitle(create: false);
				if (defaultTitle != null && (common == null || !common.Chart.serializing))
				{
					return defaultTitle.Font;
				}
				return new Font(GetDefaultFontFamilyName(), 8f);
			}
			set
			{
				titleFont = value;
				Title defaultTitle = GetDefaultTitle(create: false);
				if (defaultTitle != null)
				{
					defaultTitle.Font = value;
				}
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartHatchStyle.None)]
		[SRDescription("DescriptionAttributeBackHatchStyle9")]
		public ChartHatchStyle BackHatchStyle
		{
			get
			{
				return backHatchStyle;
			}
			set
			{
				backHatchStyle = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeBackImage16")]
		[NotifyParentProperty(true)]
		public string BackImage
		{
			get
			{
				return backImage;
			}
			set
			{
				backImage = value;
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
				return backImageMode;
			}
			set
			{
				backImageMode = value;
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
				return backImageTranspColor;
			}
			set
			{
				backImageTranspColor = value;
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
				return backImageAlign;
			}
			set
			{
				backImageAlign = value;
			}
		}

		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeSoftShadows3")]
		public bool SoftShadows
		{
			get
			{
				return softShadows;
			}
			set
			{
				softShadows = value;
			}
		}

		[SRCategory("CategoryAttributeImage")]
		[Browsable(false)]
		[Bindable(false)]
		[DefaultValue(typeof(AntiAlias), "On")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeAntiAlias")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public AntiAlias AntiAlias
		{
			get
			{
				return antiAlias;
			}
			set
			{
				antiAlias = value;
				if (antiAlias == AntiAlias.Off)
				{
					AntiAliasing = AntiAliasingTypes.None;
				}
				else if (antiAlias == AntiAlias.On)
				{
					AntiAliasing = AntiAliasingTypes.All;
				}
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
				return antiAliasing;
			}
			set
			{
				antiAliasing = value;
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
				return textAntiAliasingQuality;
			}
			set
			{
				textAntiAliasingQuality = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(GradientType.None)]
		[SRDescription("DescriptionAttributeBackGradientType3")]
		public GradientType BackGradientType
		{
			get
			{
				return backGradientType;
			}
			set
			{
				backGradientType = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeBackGradientEndColor4")]
		public Color BackGradientEndColor
		{
			get
			{
				return backGradientEndColor;
			}
			set
			{
				if (value != Color.Empty && (value.A != byte.MaxValue || value == Color.Transparent))
				{
					throw new ArgumentException(SR.ExceptionChartBackGradientEndColorIsTransparent);
				}
				backGradientEndColor = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeChart_BorderlineWidth")]
		public int BorderWidth
		{
			get
			{
				return borderWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionChartBorderIsNegative);
				}
				borderWidth = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartDashStyle.NotSet)]
		[SRDescription("DescriptionAttributeBorderStyle8")]
		public ChartDashStyle BorderStyle
		{
			get
			{
				return borderStyle;
			}
			set
			{
				borderStyle = value;
			}
		}

		internal SelectionManager SelectorManager => selectorManager;

		internal bool IsSelectorManagerEnabled
		{
			get
			{
				if (selectorManager.Enabled)
				{
					return true;
				}
				return false;
			}
		}

		internal event PaintEventHandler BeforePaint;

		internal event PaintEventHandler AfterPaint;

		private ChartPicture()
		{
		}

		public ChartPicture(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(SR.ExceptionInvalidServiceContainer);
			}
			common = new CommonElements(container);
			chartGraph = new ChartGraphics(common);
			hotRegionsList = new HotRegionsList(common);
			borderSkin = new BorderSkinAttributes(container);
			serviceContainer = container;
			chartAreas = new ChartAreaCollection(common);
			legend.Common = common;
			legend.CustomItems.common = common;
			legend.Position.common = common;
			legends = new LegendCollection(common);
			legend.Name = "Default";
			legends.Add(legend);
			titles = new TitleCollection(serviceContainer);
			annotations = new AnnotationCollection(serviceContainer);
			dataManipulator.Common = common;
			mapAreas = new MapAreasCollection();
			selectorManager = new SelectionManager(serviceContainer);
		}

		internal void Initialize(Chart chart)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(ChartPicture))
			{
				return this;
			}
			throw new ArgumentException(SR.ExceptionChartPictureUnsupportedType(serviceType.ToString()));
		}

		internal void Dispose()
		{
			if (chartGraph != null)
			{
				chartGraph.Dispose();
			}
		}

		public void Select(int x, int y, ChartElementType requestedElement, bool ignoreTransparent, out string series, out int point, out ChartElementType type, out object obj)
		{
			object subObj = null;
			Select(x, y, requestedElement, ignoreTransparent, out series, out point, out type, out obj, out subObj);
		}

		public void Select(int x, int y, ChartElementType requestedElement, bool ignoreTransparent, out string series, out int point, out ChartElementType type, out object obj, out object subObj)
		{
			point = -1;
			series = "";
			obj = null;
			subObj = null;
			type = ChartElementType.Nothing;
			if (Width <= 0 || Height <= 0)
			{
				return;
			}
			common.HotRegionsList.ProcessChartMode |= ProcessMode.HotRegions;
			common.HotRegionsList.hitTestCalled = true;
			if (common.HotRegionsList.List != null)
			{
				common.HotRegionsList.CheckHotRegions(x, y, requestedElement, ignoreTransparent, out series, out point, out type, out obj, out subObj);
				if (common.HotRegionsList.List.Count != 0)
				{
					return;
				}
			}
			isSelectionMode = true;
			if (common.HotRegionsList.List != null)
			{
				common.HotRegionsList.List.Clear();
			}
			else
			{
				common.HotRegionsList.List = new ArrayList();
			}
			Bitmap bitmap = new Bitmap(Width, Height);
			Graphics graphics = Graphics.FromImage(bitmap);
			chartGraph.Graphics = graphics;
			Paint(chartGraph.Graphics, paintTopLevelElementOnly: false);
			bitmap.Dispose();
			isSelectionMode = false;
			common.HotRegionsList.ProcessChartMode |= ProcessMode.HotRegions;
			if (common.HotRegionsList.List != null)
			{
				common.HotRegionsList.CheckHotRegions(x, y, requestedElement, ignoreTransparent, out series, out point, out type, out obj, out subObj);
				_ = common.HotRegionsList.List.Count;
			}
		}

		public void Select(int x, int y, out string series, out int point)
		{
			Select(x, y, ChartElementType.Nothing, ignoreTransparent: false, out series, out point, out ChartElementType _, out object _);
		}

		public void Paint(Graphics graph, bool paintTopLevelElementOnly)
		{
			Paint(graph, paintTopLevelElementOnly, RenderingType.Gdi, null, null, string.Empty, resizable: false, preserveAspectRatio: false);
		}

		internal TextRenderingHint GetTextRenderingHint()
		{
			TextRenderingHint textRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
			if ((AntiAliasing & AntiAliasingTypes.Text) == AntiAliasingTypes.Text)
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

		public void Paint(Graphics graph, bool paintTopLevelElementOnly, RenderingType renderingType, XmlTextWriter svgTextWriter, Stream flashStream, string documentTitle, bool resizable, bool preserveAspectRatio)
		{
			backgroundRestored = false;
			annotationSmartLabels.Reset(common, null);
			if (MapEnabled)
			{
				common.HotRegionsList.ProcessChartMode |= (ProcessMode.Paint | ProcessMode.ImageMaps);
				for (int i = 0; i < MapAreas.Count; i++)
				{
					if (!MapAreas[i].Custom)
					{
						MapAreas.RemoveAt(i);
						i--;
					}
				}
			}
			if (renderingType == RenderingType.Svg)
			{
				chartGraph.ActiveRenderingType = RenderingType.Svg;
				chartGraph.SetTitle(documentTitle);
				chartGraph.Open(svgTextWriter, new Size(width, height), new SvgOpenParameters(IsToolTipsEnabled(), resizable, preserveAspectRatio));
			}
			chartGraph.Graphics = graph;
			common.graph = chartGraph;
			chartGraph.AntiAliasing = antiAliasing;
			chartGraph.softShadows = softShadows;
			chartGraph.TextRenderingHint = GetTextRenderingHint();
			try
			{
				if (!paintTopLevelElementOnly)
				{
					OnBeforePaint(new ChartPaintEventArgs(chartGraph, common, new ElementPosition(0f, 0f, 100f, 100f)));
					_ = 1;
					bool flag = false;
					foreach (ChartArea chartArea7 in chartAreas)
					{
						if (chartArea7.Visible)
						{
							chartArea7.Set3DAnglesAndReverseMode();
							chartArea7.SetTempValues();
							chartArea7.ReCalcInternal();
							if (chartArea7.Area3DStyle.Enable3D && !chartArea7.chartAreaIsCurcular)
							{
								flag = true;
							}
						}
					}
					common.EventsManager.OnCustomize();
					Resize(chartGraph, flag);
					bool flag2 = false;
					foreach (ChartArea chartArea8 in chartAreas)
					{
						if (chartArea8.Area3DStyle.Enable3D && !chartArea8.chartAreaIsCurcular && chartArea8.Visible)
						{
							flag2 = true;
							chartArea8.Estimate3DInterval(chartGraph);
							chartArea8.ReCalcInternal();
						}
					}
					if (flag)
					{
						if (flag2)
						{
							common.EventsManager.OnCustomize();
						}
						Resize(chartGraph);
					}
					if (borderSkin.SkinStyle != 0 && Width > 20 && Height > 20)
					{
						chartGraph.FillRectangleAbs(new RectangleF(0f, 0f, Width - 1, Height - 1), borderSkin.PageColor, ChartHatchStyle.None, "", ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, borderSkin.PageColor, 1, ChartDashStyle.Solid, PenAlignment.Inset);
						if (chartGraph.ActiveRenderingType == RenderingType.Svg)
						{
							Bitmap bitmap = new Bitmap(Width, Height);
							Graphics graphics = Graphics.FromImage(bitmap);
							graphics.SmoothingMode = chartGraph.Graphics.SmoothingMode;
							ChartGraphics chartGraphics = new ChartGraphics(common);
							chartGraphics.Graphics = graphics;
							chartGraphics.Draw3DBorderAbs(borderSkin, chartBorderPosition, BackColor, BackHatchStyle, BackImage, BackImageMode, BackImageTransparentColor, BackImageAlign, BackGradientType, BackGradientEndColor, BorderColor, BorderWidth, BorderStyle);
							chartGraph.DrawImage(bitmap, new RectangleF(0f, 0f, Width - 1, Height - 1));
							chartGraphics.Dispose();
							graphics.Dispose();
							bitmap.Dispose();
						}
						else
						{
							chartGraph.Draw3DBorderAbs(borderSkin, chartBorderPosition, BackColor, BackHatchStyle, BackImage, BackImageMode, BackImageTransparentColor, BackImageAlign, BackGradientType, BackGradientEndColor, BorderColor, BorderWidth, BorderStyle);
						}
					}
					else
					{
						chartGraph.FillRectangleAbs(new RectangleF(0f, 0f, Width - 1, Height - 1), BackColor, BackHatchStyle, BackImage, BackImageMode, BackImageTransparentColor, BackImageAlign, BackGradientType, BackGradientEndColor, BorderColor, BorderWidth, BorderStyle, PenAlignment.Inset);
					}
					common.EventsManager.OnBackPaint(this, new ChartPaintEventArgs(chartGraph, common, new ElementPosition(0f, 0f, 100f, 100f)));
					foreach (ChartArea chartArea9 in chartAreas)
					{
						if (chartArea9.Visible)
						{
							chartArea9.Paint(chartGraph);
						}
					}
					foreach (ChartArea chartArea10 in chartAreas)
					{
						chartArea10.intervalData = double.NaN;
					}
					foreach (Legend legend2 in Legends)
					{
						legend2.Paint(chartGraph);
					}
					foreach (Title title in Titles)
					{
						title.Paint(chartGraph);
					}
					common.EventsManager.OnPaint(this, new ChartPaintEventArgs(chartGraph, common, new ElementPosition(0f, 0f, 100f, 100f)));
				}
				if (common.Chart != null)
				{
					foreach (Series item in common.Chart.Series)
					{
						item.financialMarkers.DrawMarkers(chartGraph, this);
					}
				}
				Annotations.Paint(chartGraph, paintTopLevelElementOnly);
				if (!isSelectionMode)
				{
					foreach (ChartArea chartArea11 in chartAreas)
					{
						if (chartArea11.Visible)
						{
							chartArea11.PaintCursors(chartGraph, paintTopLevelElementOnly);
						}
					}
				}
				foreach (ChartArea chartArea12 in chartAreas)
				{
					if (chartArea12.Visible)
					{
						chartArea12.Restore3DAnglesAndReverseMode();
						chartArea12.GetTempValues();
					}
				}
				if (!isSelectionMode)
				{
					SelectorManager.DrawSelection();
				}
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				OnAfterPaint(new ChartPaintEventArgs(chartGraph, common, new ElementPosition(0f, 0f, 100f, 100f)));
				foreach (ChartArea chartArea13 in chartAreas)
				{
					if (chartArea13.Visible)
					{
						chartArea13.Restore3DAnglesAndReverseMode();
						chartArea13.GetTempValues();
					}
				}
				showWaterMark = !common.Chart.IsDesignMode();
				if (renderingType == RenderingType.Svg)
				{
					chartGraph.Close();
					chartGraph.ActiveRenderingType = RenderingType.Gdi;
				}
			}
		}

		private void DrawTitle(ChartGraphics graph)
		{
			object obj = null;
			DrawTitle(graph, selectionMode: false, 0, 0, out obj);
		}

		private void DrawTitle(ChartGraphics graph, bool selectionMode, int x, int y, out object obj)
		{
			obj = null;
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			if (selectionMode)
			{
				RectangleF absolute = new RectangleF(x - 1, y - 1, 2f, 2f);
				absolute = graph.GetRelativeRectangle(absolute);
				if (titlePosition.IntersectsWith(absolute))
				{
					obj = this;
				}
			}
			else
			{
				graph.DrawStringRel(Title.Replace("\\n", "\n"), TitleFont, new SolidBrush(TitleFontColor), titlePosition, stringFormat);
			}
		}

		protected virtual void OnBeforePaint(ChartPaintEventArgs e)
		{
			if (this.BeforePaint != null)
			{
				this.BeforePaint(this, e);
			}
		}

		protected virtual void OnAfterPaint(ChartPaintEventArgs e)
		{
			if (this.AfterPaint != null)
			{
				this.AfterPaint(this, e);
			}
		}

		public void Resize(ChartGraphics chartGraph)
		{
			Resize(chartGraph, calcAreaPositionOnly: false);
		}

		public void Resize(ChartGraphics chartGraph, bool calcAreaPositionOnly)
		{
			common.Width = width;
			common.Height = height;
			chartGraph.SetPictureSize(width, height);
			RectangleF absolute = new RectangleF(0f, 0f, width - 1, height - 1);
			absolute = chartGraph.GetRelativeRectangle(absolute);
			titlePosition = RectangleF.Empty;
			IBorderType borderType = null;
			bool flag = false;
			if (borderSkin.SkinStyle != 0)
			{
				chartBorderPosition = chartGraph.GetAbsoluteRectangle(absolute);
				borderType = common.BorderTypeRegistry.GetBorderType(borderSkin.SkinStyle.ToString());
				if (borderType != null)
				{
					borderType.Resolution = chartGraph.Graphics.DpiX;
					flag = (borderType.GetTitlePositionInBorder() != RectangleF.Empty);
					titlePosition = chartGraph.GetRelativeRectangle(borderType.GetTitlePositionInBorder());
					titlePosition.Width = absolute.Width - titlePosition.Width;
					borderType.AdjustAreasPosition(chartGraph, ref absolute);
				}
			}
			RectangleF frameTitlePosition = RectangleF.Empty;
			if (flag)
			{
				frameTitlePosition = new RectangleF(titlePosition.Location, titlePosition.Size);
			}
			foreach (Title title in Titles)
			{
				if (title.DockToChartArea == "NotSet" && title.Position.Auto && title.Visible)
				{
					title.CalcTitlePosition(chartGraph, ref absolute, ref frameTitlePosition, 3f);
				}
			}
			Legends.CalcLegendPosition(chartGraph, ref absolute, legendMaxAutoSize, 3f);
			absolute.Width -= 3f;
			absolute.Height -= 3f;
			RectangleF chartAreasRectangle = default(RectangleF);
			int num = 0;
			foreach (ChartArea chartArea4 in chartAreas)
			{
				if (chartArea4.Visible && chartArea4.Position.Auto)
				{
					num++;
				}
			}
			int num2 = (int)Math.Floor(Math.Sqrt(num));
			if (num2 < 1)
			{
				num2 = 1;
			}
			int num3 = (int)Math.Ceiling((float)num / (float)num2);
			int num4 = 0;
			int num5 = 0;
			foreach (ChartArea chartArea5 in chartAreas)
			{
				if (!chartArea5.Visible)
				{
					continue;
				}
				if (chartArea5.Position.Auto)
				{
					chartAreasRectangle.Width = absolute.Width / (float)num2 - 3f;
					chartAreasRectangle.Height = absolute.Height / (float)num3 - 3f;
					chartAreasRectangle.X = absolute.X + (float)num4 * (absolute.Width / (float)num2) + 3f;
					chartAreasRectangle.Y = absolute.Y + (float)num5 * (absolute.Height / (float)num3) + 3f;
					TitleCollection.CalcOutsideTitlePosition(this, chartGraph, chartArea5, ref chartAreasRectangle, 3f);
					Legends.CalcOutsideLegendPosition(chartGraph, chartArea5, ref chartAreasRectangle, legendMaxAutoSize, 3f);
					chartArea5.Position.SetPositionNoAuto(chartAreasRectangle.X, chartAreasRectangle.Y, chartAreasRectangle.Width, chartAreasRectangle.Height);
					num5++;
					if (num5 >= num3)
					{
						num5 = 0;
						num4++;
					}
				}
				else
				{
					RectangleF chartAreasRectangle2 = chartArea5.Position.ToRectangleF();
					TitleCollection.CalcOutsideTitlePosition(this, chartGraph, chartArea5, ref chartAreasRectangle2, 3f);
					Legends.CalcOutsideLegendPosition(chartGraph, chartArea5, ref chartAreasRectangle2, legendMaxAutoSize, 3f);
				}
			}
			AlignChartAreasPosition();
			if (calcAreaPositionOnly)
			{
				return;
			}
			foreach (ChartArea chartArea6 in chartAreas)
			{
				if (chartArea6.Visible)
				{
					chartArea6.Resize(chartGraph);
				}
			}
			AlignChartAreas(AreaAlignTypes.PlotPosition);
			TitleCollection.CalcInsideTitlePosition(this, chartGraph, 3f);
			Legends.CalcInsideLegendPosition(chartGraph, legendMaxAutoSize, 3f);
		}

		internal void ResetMinMaxFromData()
		{
			foreach (ChartArea chartArea in chartAreas)
			{
				if (chartArea.Visible)
				{
					chartArea.ResetMinMaxFromData();
				}
			}
		}

		public void Recalculate()
		{
			foreach (ChartArea chartArea in chartAreas)
			{
				if (chartArea.Visible)
				{
					chartArea.ReCalcInternal();
				}
			}
		}

		private bool IsAreasAlignmentRequired()
		{
			bool result = false;
			foreach (ChartArea chartArea in ChartAreas)
			{
				if (chartArea.Visible && chartArea.AlignWithChartArea != "NotSet")
				{
					result = true;
					try
					{
						_ = ChartAreas[chartArea.AlignWithChartArea];
					}
					catch
					{
						throw new InvalidOperationException(SR.ExceptionChartAreaNameReferenceInvalid(chartArea.Name, chartArea.AlignWithChartArea));
					}
				}
			}
			return result;
		}

		private ArrayList GetAlignedAreasGroup(ChartArea masterArea, AreaAlignTypes type, AreaAlignOrientations orientation)
		{
			ArrayList arrayList = new ArrayList();
			foreach (ChartArea chartArea in ChartAreas)
			{
				if (chartArea.Visible && chartArea.Name != masterArea.Name && chartArea.AlignWithChartArea == masterArea.Name && (chartArea.AlignType & type) == type && (chartArea.AlignOrientation & orientation) == orientation)
				{
					arrayList.Add(chartArea);
				}
			}
			if (arrayList.Count > 0)
			{
				arrayList.Insert(0, masterArea);
			}
			return arrayList;
		}

		internal void AlignChartAreas(AreaAlignTypes type)
		{
			if (!IsAreasAlignmentRequired())
			{
				return;
			}
			foreach (ChartArea chartArea in ChartAreas)
			{
				if (chartArea.Visible)
				{
					ArrayList alignedAreasGroup = GetAlignedAreasGroup(chartArea, type, AreaAlignOrientations.Vertical);
					if (alignedAreasGroup.Count > 0)
					{
						AlignChartAreasPlotPosition(alignedAreasGroup, AreaAlignOrientations.Vertical);
					}
					alignedAreasGroup = GetAlignedAreasGroup(chartArea, type, AreaAlignOrientations.Horizontal);
					if (alignedAreasGroup.Count > 0)
					{
						AlignChartAreasPlotPosition(alignedAreasGroup, AreaAlignOrientations.Horizontal);
					}
				}
			}
		}

		private void AlignChartAreasPlotPosition(ArrayList areasGroup, AreaAlignOrientations orientation)
		{
			RectangleF rectangleF = ((ChartArea)areasGroup[0]).PlotAreaPosition.ToRectangleF();
			foreach (ChartArea item in areasGroup)
			{
				if (item.PlotAreaPosition.X > rectangleF.X)
				{
					rectangleF.X += item.PlotAreaPosition.X - rectangleF.X;
					rectangleF.Width -= item.PlotAreaPosition.X - rectangleF.X;
				}
				if (item.PlotAreaPosition.Y > rectangleF.Y)
				{
					rectangleF.Y += item.PlotAreaPosition.Y - rectangleF.Y;
					rectangleF.Height -= item.PlotAreaPosition.Y - rectangleF.Y;
				}
				if (item.PlotAreaPosition.Right() < rectangleF.Right)
				{
					rectangleF.Width -= rectangleF.Right - item.PlotAreaPosition.Right();
					if (rectangleF.Width < 5f)
					{
						rectangleF.Width = 5f;
					}
				}
				if (item.PlotAreaPosition.Bottom() < rectangleF.Bottom)
				{
					rectangleF.Height -= rectangleF.Bottom - item.PlotAreaPosition.Bottom();
					if (rectangleF.Height < 5f)
					{
						rectangleF.Height = 5f;
					}
				}
			}
			foreach (ChartArea item2 in areasGroup)
			{
				RectangleF rectangleF2 = item2.PlotAreaPosition.ToRectangleF();
				if ((orientation & AreaAlignOrientations.Vertical) == AreaAlignOrientations.Vertical)
				{
					rectangleF2.X = rectangleF.X;
					rectangleF2.Width = rectangleF.Width;
				}
				if ((orientation & AreaAlignOrientations.Horizontal) == AreaAlignOrientations.Horizontal)
				{
					rectangleF2.Y = rectangleF.Y;
					rectangleF2.Height = rectangleF.Height;
				}
				item2.PlotAreaPosition.SetPositionNoAuto(rectangleF2.X, rectangleF2.Y, rectangleF2.Width, rectangleF2.Height);
				rectangleF2.X = (rectangleF2.X - item2.Position.X) / item2.Position.Width * 100f;
				rectangleF2.Y = (rectangleF2.Y - item2.Position.Y) / item2.Position.Height * 100f;
				rectangleF2.Width = rectangleF2.Width / item2.Position.Width * 100f;
				rectangleF2.Height = rectangleF2.Height / item2.Position.Height * 100f;
				item2.InnerPlotPosition.SetPositionNoAuto(rectangleF2.X, rectangleF2.Y, rectangleF2.Width, rectangleF2.Height);
				if ((orientation & AreaAlignOrientations.Vertical) == AreaAlignOrientations.Vertical)
				{
					item2.AxisX2.AdjustLabelFontAtSecondPass(chartGraph, item2.InnerPlotPosition.Auto);
					item2.AxisX.AdjustLabelFontAtSecondPass(chartGraph, item2.InnerPlotPosition.Auto);
				}
				if ((orientation & AreaAlignOrientations.Horizontal) == AreaAlignOrientations.Horizontal)
				{
					item2.AxisY2.AdjustLabelFontAtSecondPass(chartGraph, item2.InnerPlotPosition.Auto);
					item2.AxisY.AdjustLabelFontAtSecondPass(chartGraph, item2.InnerPlotPosition.Auto);
				}
			}
		}

		private void AlignChartAreasPosition()
		{
			if (!IsAreasAlignmentRequired())
			{
				return;
			}
			foreach (ChartArea chartArea3 in ChartAreas)
			{
				if (chartArea3.Visible && chartArea3.AlignWithChartArea != "NotSet" && (chartArea3.AlignType & AreaAlignTypes.Position) == AreaAlignTypes.Position)
				{
					RectangleF rectangleF = chartArea3.Position.ToRectangleF();
					ChartArea chartArea2 = ChartAreas[chartArea3.AlignWithChartArea];
					if ((chartArea3.AlignOrientation & AreaAlignOrientations.Vertical) == AreaAlignOrientations.Vertical)
					{
						rectangleF.X = chartArea2.Position.X;
						rectangleF.Width = chartArea2.Position.Width;
					}
					if ((chartArea3.AlignOrientation & AreaAlignOrientations.Horizontal) == AreaAlignOrientations.Horizontal)
					{
						rectangleF.Y = chartArea2.Position.Y;
						rectangleF.Height = chartArea2.Position.Height;
					}
					chartArea3.Position.SetPositionNoAuto(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height);
				}
			}
		}

		internal void AlignChartAreasCursor(ChartArea changedArea, AreaAlignOrientations orientation, bool selectionChanged)
		{
			if (!IsAreasAlignmentRequired())
			{
				return;
			}
			foreach (ChartArea chartArea3 in ChartAreas)
			{
				if (!chartArea3.Visible)
				{
					continue;
				}
				ArrayList alignedAreasGroup = GetAlignedAreasGroup(chartArea3, AreaAlignTypes.Cursor, orientation);
				if (!alignedAreasGroup.Contains(changedArea))
				{
					continue;
				}
				foreach (ChartArea item in alignedAreasGroup)
				{
					item.alignmentInProcess = true;
					if (orientation == AreaAlignOrientations.Vertical)
					{
						if (selectionChanged)
						{
							item.CursorX.SelectionStart = changedArea.CursorX.SelectionStart;
							item.CursorX.SelectionEnd = changedArea.CursorX.SelectionEnd;
						}
						else
						{
							item.CursorX.Position = changedArea.CursorX.Position;
						}
					}
					if (orientation == AreaAlignOrientations.Horizontal)
					{
						if (selectionChanged)
						{
							item.CursorY.SelectionStart = changedArea.CursorY.SelectionStart;
							item.CursorY.SelectionEnd = changedArea.CursorY.SelectionEnd;
						}
						else
						{
							item.CursorY.Position = changedArea.CursorY.Position;
						}
					}
					item.alignmentInProcess = false;
				}
			}
		}

		internal void AlignChartAreasZoomed(ChartArea changedArea, AreaAlignOrientations orientation, bool disposeBufferBitmap)
		{
			if (!IsAreasAlignmentRequired())
			{
				return;
			}
			foreach (ChartArea chartArea3 in ChartAreas)
			{
				if (!chartArea3.Visible)
				{
					continue;
				}
				ArrayList alignedAreasGroup = GetAlignedAreasGroup(chartArea3, AreaAlignTypes.AxesView, orientation);
				if (!alignedAreasGroup.Contains(changedArea))
				{
					continue;
				}
				foreach (ChartArea item in alignedAreasGroup)
				{
					if (orientation == AreaAlignOrientations.Vertical)
					{
						item.CursorX.SelectionStart = double.NaN;
						item.CursorX.SelectionEnd = double.NaN;
					}
					if (orientation == AreaAlignOrientations.Horizontal)
					{
						item.CursorY.SelectionStart = double.NaN;
						item.CursorY.SelectionEnd = double.NaN;
					}
				}
			}
		}

		internal void AlignChartAreasAxesView(ChartArea changedArea, AreaAlignOrientations orientation)
		{
			if (!IsAreasAlignmentRequired())
			{
				return;
			}
			foreach (ChartArea chartArea3 in ChartAreas)
			{
				if (!chartArea3.Visible)
				{
					continue;
				}
				ArrayList alignedAreasGroup = GetAlignedAreasGroup(chartArea3, AreaAlignTypes.AxesView, orientation);
				if (!alignedAreasGroup.Contains(changedArea))
				{
					continue;
				}
				foreach (ChartArea item in alignedAreasGroup)
				{
					item.alignmentInProcess = true;
					if (orientation == AreaAlignOrientations.Vertical)
					{
						item.AxisX.View.Position = changedArea.AxisX.View.Position;
						item.AxisX.View.Size = changedArea.AxisX.View.Size;
						item.AxisX.View.SizeType = changedArea.AxisX.View.SizeType;
						item.AxisX2.View.Position = changedArea.AxisX2.View.Position;
						item.AxisX2.View.Size = changedArea.AxisX2.View.Size;
						item.AxisX2.View.SizeType = changedArea.AxisX2.View.SizeType;
					}
					if (orientation == AreaAlignOrientations.Horizontal)
					{
						item.AxisY.View.Position = changedArea.AxisY.View.Position;
						item.AxisY.View.Size = changedArea.AxisY.View.Size;
						item.AxisY.View.SizeType = changedArea.AxisY.View.SizeType;
						item.AxisY2.View.Position = changedArea.AxisY2.View.Position;
						item.AxisY2.View.Size = changedArea.AxisY2.View.Size;
						item.AxisY2.View.SizeType = changedArea.AxisY2.View.SizeType;
					}
					item.alignmentInProcess = false;
				}
			}
		}

		internal bool IsRightToLeft()
		{
			return RightToLeft == RightToLeft.Yes;
		}

		internal static string GetDefaultFontFamilyName()
		{
			if (defaultFontFamilyName.Length == 0)
			{
				defaultFontFamilyName = "Microsoft Sans Serif";
				bool flag = false;
				FontFamily[] families = FontFamily.Families;
				for (int i = 0; i < families.Length; i++)
				{
					if (families[i].Name == defaultFontFamilyName)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					try
					{
						defaultFontFamilyName = FontFamily.GenericSansSerif.Name;
					}
					catch
					{
					}
				}
			}
			return defaultFontFamilyName;
		}

		public void LoadTemplate(string name)
		{
			Stream stream = LoadTemplateData(name);
			LoadTemplate(stream);
			stream.Close();
		}

		public void LoadTemplate(Stream stream)
		{
			if (stream == null)
			{
				return;
			}
			ChartSerializer chartSerializer = (ChartSerializer)common.container.GetService(typeof(ChartSerializer));
			if (chartSerializer == null)
			{
				return;
			}
			string serializableContent = chartSerializer.SerializableContent;
			string nonSerializableContent = chartSerializer.NonSerializableContent;
			SerializationFormat format = chartSerializer.Format;
			bool ignoreUnknownXmlAttributes = chartSerializer.IgnoreUnknownXmlAttributes;
			bool templateMode = chartSerializer.TemplateMode;
			chartSerializer.Content = SerializationContents.Appearance;
			chartSerializer.Format = SerializationFormat.Xml;
			chartSerializer.IgnoreUnknownXmlAttributes = true;
			chartSerializer.TemplateMode = true;
			try
			{
				chartSerializer.Load(stream);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(ex.Message);
			}
			finally
			{
				chartSerializer.SerializableContent = serializableContent;
				chartSerializer.NonSerializableContent = nonSerializableContent;
				chartSerializer.Format = format;
				chartSerializer.IgnoreUnknownXmlAttributes = ignoreUnknownXmlAttributes;
				chartSerializer.TemplateMode = templateMode;
			}
		}

		private Stream LoadTemplateData(string url)
		{
			Stream stream = null;
			if (stream == null)
			{
				stream = new FileStream(url, FileMode.Open);
			}
			return stream;
		}

		internal void WriteChartMapTag(TextWriter output, string mapName)
		{
			output.Write("\r\n<map name=\"" + mapName + "\" id=\"" + mapName + "\">");
			MapAreasCollection mapAreasCollection = new MapAreasCollection();
			for (int i = 0; i < mapAreas.Count; i++)
			{
				if (!mapAreas[i].Custom)
				{
					mapAreasCollection.Add(mapAreas[i]);
					mapAreas.RemoveAt(i);
					i--;
				}
			}
			common.EventsManager.OnCustomizeMapAreas(mapAreasCollection);
			foreach (MapArea item in mapAreasCollection)
			{
				item.Custom = false;
				mapAreas.Add(item);
			}
			foreach (MapArea mapArea in mapAreas)
			{
				output.Write(mapArea.GetTag(chartGraph));
			}
			if (mapAreas.Count == 0)
			{
				output.Write("<area shape=\"rect\" coords=\"0,0,0,0\" alt=\"\" />");
			}
			output.Write("\r\n</map>\r\n");
		}

		internal Title GetDefaultTitle(bool create)
		{
			Title title = null;
			foreach (Title title2 in Titles)
			{
				if (title2.Name == "Default Title")
				{
					title = title2;
				}
			}
			if (title == null && create)
			{
				title = new Title();
				title.Name = "Default Title";
				Titles.Insert(0, title);
			}
			return title;
		}

		private bool IsToolTipsEnabled()
		{
			foreach (Series item in common.DataManager.Series)
			{
				if (item.ToolTip.Length > 0)
				{
					return true;
				}
				if (item.LegendToolTip.Length > 0 || item.LabelToolTip.Length > 0)
				{
					return true;
				}
				if (item.IsFastChartType())
				{
					continue;
				}
				foreach (DataPoint point in item.Points)
				{
					if (point.ToolTip.Length > 0)
					{
						return true;
					}
					if (point.LegendToolTip.Length > 0 || point.LabelToolTip.Length > 0)
					{
						return true;
					}
				}
			}
			foreach (Legend legend2 in Legends)
			{
				foreach (LegendItem customItem in legend2.CustomItems)
				{
					if (customItem.ToolTip.Length > 0)
					{
						return true;
					}
				}
			}
			foreach (Title title in Titles)
			{
				if (title.ToolTip.Length > 0)
				{
					return true;
				}
			}
			return false;
		}
	}
}

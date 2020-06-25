using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(StateIndicatorConverter))]
	internal class StateIndicator : NamedElement, IRenderable, IToolTipProvider, IPointerProvider, ISelectable, IImageMapProvider
	{
		private DataAttributes data;

		private double internalValue;

		private StateCollection states;

		private NamedElement parentSystem;

		private string parent = string.Empty;

		private int zOrder;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private StateIndicatorStyle style = StateIndicatorStyle.CircularLed;

		private GaugeLocation location;

		private GaugeSize size;

		private bool visible = true;

		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private Color borderColor = Color.Black;

		private GaugeDashStyle borderStyle;

		private int borderWidth = 1;

		private Color fillColor = Color.LawnGreen;

		private GradientType fillGradientType = GradientType.Center;

		private Color fillGradientEndColor = Color.DarkGreen;

		private GaugeHatchStyle fillHatchStyle;

		private string text = "Text";

		private string image = "";

		private Color imageTransColor = Color.Empty;

		private Color imageHueColor = Color.Empty;

		private float shadowOffset = 1f;

		private bool showBorder;

		private float angle;

		private float scaleFactor = 1f;

		private ResizeMode resizeMode = ResizeMode.AutoFit;

		private float imageTransparency;

		private FontUnit fontUnit = FontUnit.Default;

		private bool selected;

		private bool defaultParent = true;

		private object imageMapProviderTag;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public StateCollection States => states;

		[Bindable(false)]
		[Browsable(false)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeParentObject3")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public NamedElement ParentObject => parentSystem;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeStateIndicator_Parent")]
		[TypeConverter(typeof(ParentSourceConverter))]
		[NotifyParentProperty(true)]
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
				DefaultParent = false;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeStateIndicator_ZOrder")]
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

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeStateIndicator_ToolTip")]
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
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeStateIndicator_Href")]
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

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeStateIndicator_Name")]
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

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeStateIndicator_IndicatorStyle")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[ParenthesizePropertyName(true)]
		[DefaultValue(StateIndicatorStyle.CircularLed)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Obsolete("This property is obsolete in Dundas Gauge 2.0. IndicatorStyle is supposed to be used instead.")]
		public StateIndicatorStyle Style
		{
			get
			{
				return style;
			}
			set
			{
				style = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeStateIndicator_IndicatorStyle")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(StateIndicatorStyle.CircularLed)]
		public StateIndicatorStyle IndicatorStyle
		{
			get
			{
				return style;
			}
			set
			{
				style = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeStateIndicator_Location")]
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
				location = value;
				location.Parent = this;
				Invalidate();
			}
		}

		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeStateIndicator_Size")]
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
				size = value;
				size.Parent = this;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeStateIndicator_Visible")]
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
		[SRDescription("DescriptionAttributeStateIndicator_Font")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt")]
		public Font Font
		{
			get
			{
				return font;
			}
			set
			{
				font = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeStateIndicator_BorderColor")]
		[NotifyParentProperty(true)]
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
		[SRDescription("DescriptionAttributeStateIndicator_BorderStyle")]
		[NotifyParentProperty(true)]
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
		[SRDescription("DescriptionAttributeStateIndicator_BorderWidth")]
		[NotifyParentProperty(true)]
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
		[SRDescription("DescriptionAttributeStateIndicator_FillColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "LawnGreen")]
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
		[SRDescription("DescriptionAttributeStateIndicator_FillGradientType")]
		[NotifyParentProperty(true)]
		[DefaultValue(GradientType.Center)]
		public GradientType FillGradientType
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
		[SRDescription("DescriptionAttributeStateIndicator_FillGradientEndColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "DarkGreen")]
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
		[SRDescription("DescriptionAttributeStateIndicator_FillHatchStyle")]
		[NotifyParentProperty(true)]
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

		[Browsable(false)]
		[SRCategory("CategoryData")]
		[SRDescription("DescriptionAttributeStateIndicator_Value")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.Repaint)]
		[DefaultValue(double.NaN)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public double Value
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
					data.ValueSource = string.Empty;
					Invalidate();
				}
			}
		}

		[SRCategory("CategoryData")]
		[SRDescription("DescriptionAttributeStateIndicator_Visible")]
		[DefaultValue(true)]
		public bool IsPercentBased
		{
			get
			{
				return data.IsPercentBased;
			}
			set
			{
				data.IsPercentBased = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryData")]
		[SRDescription("DescriptionAttributeStateIndicator_Minimum")]
		[DefaultValue(0.0)]
		public double Minimum
		{
			get
			{
				return data.Minimum;
			}
			set
			{
				if (Common != null)
				{
					if (value > data.Maximum || (value == data.Maximum && value != 0.0))
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionMinMax"));
					}
					if (double.IsNaN(value) || double.IsInfinity(value))
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidValue"));
					}
				}
				data.Minimum = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryData")]
		[SRDescription("DescriptionAttributeStateIndicator_Maximum")]
		[DefaultValue(100.0)]
		public double Maximum
		{
			get
			{
				return data.Maximum;
			}
			set
			{
				if (Common != null)
				{
					if (value < data.Minimum || (value == data.Minimum && value != 0.0))
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionMaxMin"));
					}
					if (double.IsNaN(value) || double.IsInfinity(value))
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidValue"));
					}
				}
				data.Maximum = value;
				Invalidate();
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
		public string ValueSource
		{
			get
			{
				return data.ValueSource;
			}
			set
			{
				data.ValueSource = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeStateIndicator_Text")]
		[Localizable(true)]
		[DefaultValue("Text")]
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeStateIndicator_Image")]
		[DefaultValue("")]
		public string Image
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
		[SRDescription("DescriptionAttributeImageTransColor6")]
		[DefaultValue(typeof(Color), "")]
		public Color ImageTransColor
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
		[SRDescription("DescriptionAttributeImageHueColor4")]
		[DefaultValue(typeof(Color), "")]
		public Color ImageHueColor
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

		[SRDescription("DescriptionAttributeStateIndicator_CurrentState")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public string CurrentState
		{
			get
			{
				State currentState = GetCurrentState();
				if (currentState != null)
				{
					return currentState.Name;
				}
				return string.Empty;
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeShadowOffset4")]
		[NotifyParentProperty(true)]
		[ValidateBound(-5.0, 5.0)]
		[DefaultValue(1f)]
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
		[SRDescription("DescriptionAttributeShowBorder")]
		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		public bool ShowBorder
		{
			get
			{
				return showBorder;
			}
			set
			{
				showBorder = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeStateIndicator_Angle")]
		[DefaultValue(0f)]
		[ValidateBound(0.0, 360.0)]
		public float Angle
		{
			get
			{
				return angle;
			}
			set
			{
				if (value > 360f || value < 0f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 360));
				}
				angle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeStateIndicator_ScaleFactor")]
		[DefaultValue(1f)]
		[ValidateBound(0.0, 1.0)]
		public float ScaleFactor
		{
			get
			{
				return scaleFactor;
			}
			set
			{
				if (value > 1f || value < 0f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 1));
				}
				scaleFactor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeStateIndicator_ResizeMode")]
		[DefaultValue(ResizeMode.AutoFit)]
		public ResizeMode ResizeMode
		{
			get
			{
				return resizeMode;
			}
			set
			{
				resizeMode = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeStateIndicator_ImageTransparency")]
		[DefaultValue(0f)]
		[ValidateBound(0.0, 100.0)]
		public float ImageTransparency
		{
			get
			{
				return imageTransparency;
			}
			set
			{
				if (value > 100f || value < 0f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 100));
				}
				imageTransparency = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFontUnit3")]
		[DefaultValue(FontUnit.Default)]
		public FontUnit FontUnit
		{
			get
			{
				return fontUnit;
			}
			set
			{
				fontUnit = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeStateIndicator_Selected")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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

		internal Position Position => new Position(Location, Size, ContentAlignment.TopLeft);

		internal override CommonElements Common
		{
			get
			{
				return base.Common;
			}
			set
			{
				data.Common = value;
				States.Common = value;
				base.Common = value;
			}
		}

		internal DataAttributes Data => data;

		internal double InternalValue
		{
			get
			{
				return internalValue;
			}
			set
			{
				internalValue = value;
				Refresh();
			}
		}

		internal bool DefaultParent
		{
			get
			{
				return defaultParent;
			}
			set
			{
				defaultParent = value;
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

		double IPointerProvider.Position
		{
			get
			{
				return InternalValue;
			}
			set
			{
				InternalValue = value;
			}
		}

		public StateIndicator()
		{
			location = new GaugeLocation(this, 26f, 47f);
			size = new GaugeSize(this, 12f, 6f);
			location.DefaultValues = true;
			size.DefaultValues = true;
			states = new StateCollection(this, common);
			data = new DataAttributes(this);
		}

		protected bool ShouldSerializeStyle()
		{
			return false;
		}

		public double GetValueInPercents()
		{
			return data.GetValueInPercents();
		}

		internal static XamlRenderer CreateXamlRenderer(StateIndicatorStyle style, Color color, RectangleF rect, bool addBorder)
		{
			XamlRenderer xamlRenderer = new XamlRenderer(style.ToString() + ".xaml");
			xamlRenderer.AllowPathGradientTransform = false;
			xamlRenderer.ParseXaml(rect, new Color[5]
			{
				Color.Transparent,
				Color.Transparent,
				color,
				Color.Transparent,
				Color.Transparent
			}, addBorder ? 5 : 4);
			return xamlRenderer;
		}

		internal XamlRenderer GetXamlRenderer(GaugeGraphics g, State currentState)
		{
			GraphicsPath path = GetPath(g, currentState);
			XamlRenderer xamlRenderer = null;
			if (path != null)
			{
				RectangleF squareScaledAbsoluteRectangle = GetSquareScaledAbsoluteRectangle(g, currentState.ScaleFactor, new RectangleF(0f, 0f, 100f, 100f));
				if (squareScaledAbsoluteRectangle.Width == 0f || squareScaledAbsoluteRectangle.Height == 0f)
				{
					return null;
				}
				float num = squareScaledAbsoluteRectangle.Width / 2f;
				RectangleF rect = CalculateXamlMarkerBounds(currentState.IndicatorStyle, new PointF(squareScaledAbsoluteRectangle.X + num, squareScaledAbsoluteRectangle.Y + num), num, num);
				xamlRenderer = CreateXamlRenderer(currentState.IndicatorStyle, currentState.FillColor, rect, showBorder);
				XamlLayer[] layers = xamlRenderer.Layers;
				for (int i = 0; i < layers.Length; i++)
				{
					GraphicsPath[] paths = layers[i].Paths;
					foreach (GraphicsPath graphicsPath in paths)
					{
						if (Angle != 0f)
						{
							using (Matrix matrix = new Matrix())
							{
								matrix.RotateAt(point: new PointF(squareScaledAbsoluteRectangle.X + squareScaledAbsoluteRectangle.Width / 2f, squareScaledAbsoluteRectangle.Y + squareScaledAbsoluteRectangle.Height / 2f), angle: Angle);
								graphicsPath.Transform(matrix);
							}
						}
					}
				}
			}
			return xamlRenderer;
		}

		internal static bool IsXamlMarker(StateIndicatorStyle style)
		{
			if ((uint)(style - 1) <= 3u)
			{
				return false;
			}
			return true;
		}

		internal static RectangleF CalculateXamlMarkerBounds(StateIndicatorStyle markerStyle, PointF centerPoint, float width, float height)
		{
			RectangleF result = RectangleF.Empty;
			if (IsXamlMarker(markerStyle))
			{
				result = new RectangleF(centerPoint.X, centerPoint.Y, 0f, 0f);
				result.Inflate(width, height);
			}
			return result;
		}

		public override string ToString()
		{
			return Name;
		}

		internal GraphicsPath GetPath(GaugeGraphics g, State currentState)
		{
			if (!Visible)
			{
				return null;
			}
			RectangleF empty = RectangleF.Empty;
			GraphicsPath graphicsPath = new GraphicsPath();
			if (IsXamlMarker(IndicatorStyle))
			{
				empty = GetSquareScaledAbsoluteRectangle(g, currentState.ScaleFactor, new RectangleF(0f, 0f, 100f, 100f));
				graphicsPath.AddRectangle(empty);
			}
			else
			{
				empty = GetScaledAbsoluteRectangle(g, currentState.ScaleFactor, new RectangleF(0f, 0f, 100f, 100f));
				if (IndicatorStyle == StateIndicatorStyle.RectangularLed)
				{
					graphicsPath.AddRectangle(empty);
				}
				else if (IndicatorStyle == StateIndicatorStyle.CircularLed)
				{
					if (empty.Width != empty.Height)
					{
						if (empty.Width > empty.Height)
						{
							empty.Offset((empty.Width - empty.Height) / 2f, 0f);
							empty.Width = empty.Height;
						}
						else if (empty.Width < empty.Height)
						{
							empty.Offset(0f, (empty.Height - empty.Width) / 2f);
							empty.Height = empty.Width;
						}
					}
					graphicsPath.AddEllipse(empty);
				}
				else if (IndicatorStyle == StateIndicatorStyle.RoundedRectangularLed)
				{
					float num = (empty.Width < empty.Height) ? (empty.Width / 2f) : (empty.Height / 2f);
					float[] cornerRadius = new float[8]
					{
						num,
						num,
						num,
						num,
						num,
						num,
						num,
						num
					};
					graphicsPath.AddPath(g.CreateRoundedRectPath(empty, cornerRadius), connect: false);
				}
				else if (IndicatorStyle == StateIndicatorStyle.Text)
				{
					Font font;
					string text;
					if (currentState != null)
					{
						text = currentState.Text;
						font = currentState.Font;
					}
					else
					{
						text = Text;
						font = Font;
					}
					if (text.Length == 0)
					{
						return null;
					}
					text = text.Replace("\\n", "\n");
					float emSize;
					if (ResizeMode == ResizeMode.AutoFit)
					{
						SizeF sizeF = g.MeasureString(text, font);
						SizeF absoluteSize = g.GetAbsoluteSize(new SizeF(100f, 100f));
						float num2 = absoluteSize.Width / sizeF.Width;
						float num3 = absoluteSize.Height / sizeF.Height;
						emSize = ((!(num2 < num3)) ? (font.SizeInPoints * num3 * 1.3f) : (font.SizeInPoints * num2 * 1.3f));
					}
					else
					{
						if (FontUnit == FontUnit.Percent)
						{
							g.RestoreDrawRegion();
							emSize = g.GetAbsoluteDimension(font.Size);
							RectangleF boundRect = ((IRenderable)this).GetBoundRect(g);
							g.CreateDrawRegion(boundRect);
						}
						else
						{
							emSize = font.Size;
						}
						emSize *= 1.3f;
					}
					StringFormat stringFormat = new StringFormat();
					stringFormat.Alignment = StringAlignment.Center;
					stringFormat.LineAlignment = StringAlignment.Center;
					graphicsPath.AddString(text, font.FontFamily, (int)font.Style, emSize, empty, stringFormat);
				}
			}
			if (Angle != 0f)
			{
				PointF point = new PointF(empty.X + empty.Width / 2f, empty.Y + empty.Height / 2f);
				using (Matrix matrix = new Matrix())
				{
					matrix.RotateAt(Angle, point);
					graphicsPath.Transform(matrix);
					return graphicsPath;
				}
			}
			return graphicsPath;
		}

		internal Brush GetBrush(GaugeGraphics g, State currentState, RectangleF rect)
		{
			if (!IsCircular())
			{
				rect = GetScaledAbsoluteRectangle(g, currentState.ScaleFactor, new RectangleF(0f, 0f, 100f, 100f));
			}
			rect.Inflate(1f, 1f);
			Brush brush = null;
			Color color;
			Color color2;
			GradientType gradientType;
			GaugeHatchStyle gaugeHatchStyle;
			if (currentState != null)
			{
				color = currentState.FillColor;
				color2 = currentState.FillGradientEndColor;
				gradientType = currentState.FillGradientType;
				gaugeHatchStyle = currentState.FillHatchStyle;
			}
			else
			{
				color = FillColor;
				color2 = FillGradientEndColor;
				gradientType = FillGradientType;
				gaugeHatchStyle = FillHatchStyle;
			}
			if (IndicatorStyle == StateIndicatorStyle.Text)
			{
				gaugeHatchStyle = GaugeHatchStyle.None;
				gradientType = GradientType.None;
			}
			if (gaugeHatchStyle != 0)
			{
				brush = GaugeGraphics.GetHatchBrush(gaugeHatchStyle, color, color2);
			}
			else if (gradientType != 0)
			{
				if (IsCircular() && gradientType == GradientType.DiagonalLeft)
				{
					brush = g.GetGradientBrush(rect, color, color2, GradientType.LeftRight);
					Matrix matrix = new Matrix();
					matrix.RotateAt(45f, new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f));
					((LinearGradientBrush)brush).Transform = matrix;
				}
				else if (IsCircular() && gradientType == GradientType.DiagonalRight)
				{
					brush = g.GetGradientBrush(rect, color, color2, GradientType.TopBottom);
					Matrix matrix2 = new Matrix();
					matrix2.RotateAt(135f, new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f));
					((LinearGradientBrush)brush).Transform = matrix2;
				}
				else if (gradientType == GradientType.Center)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					if (IsCircular())
					{
						graphicsPath.AddArc(rect.X - 1f, rect.Y - 1f, rect.Width + 2f, rect.Height + 2f, 0f, 360f);
					}
					else
					{
						graphicsPath.AddRectangle(rect);
					}
					PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
					pathGradientBrush.CenterColor = color;
					pathGradientBrush.CenterPoint = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
					pathGradientBrush.SurroundColors = new Color[1]
					{
						color2
					};
					brush = pathGradientBrush;
				}
				else
				{
					brush = g.GetGradientBrush(rect, color, color2, gradientType);
				}
				if (!IsCircular() && Angle != 0f)
				{
					PointF pointF = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
					if (brush is LinearGradientBrush)
					{
						((LinearGradientBrush)brush).TranslateTransform(0f - pointF.X, 0f - pointF.Y, MatrixOrder.Append);
						((LinearGradientBrush)brush).RotateTransform(Angle, MatrixOrder.Append);
						((LinearGradientBrush)brush).TranslateTransform(pointF.X, pointF.Y, MatrixOrder.Append);
					}
					else if (brush is PathGradientBrush)
					{
						((PathGradientBrush)brush).TranslateTransform(0f - pointF.X, 0f - pointF.Y, MatrixOrder.Append);
						((PathGradientBrush)brush).RotateTransform(Angle, MatrixOrder.Append);
						((PathGradientBrush)brush).TranslateTransform(pointF.X, pointF.Y, MatrixOrder.Append);
					}
				}
			}
			else
			{
				brush = new SolidBrush(color);
			}
			return brush;
		}

		internal Pen GetPen(GaugeGraphics g, State currentState)
		{
			Color color;
			int num;
			GaugeDashStyle gaugeDashStyle;
			if (currentState != null)
			{
				if (currentState.BorderWidth <= 0 && currentState.BorderStyle != 0)
				{
					return null;
				}
				color = currentState.BorderColor;
				num = currentState.BorderWidth;
				gaugeDashStyle = currentState.BorderStyle;
			}
			else
			{
				if (BorderWidth <= 0 && BorderStyle != 0)
				{
					return null;
				}
				color = BorderColor;
				num = BorderWidth;
				gaugeDashStyle = BorderStyle;
			}
			return new Pen(color, num)
			{
				DashStyle = g.GetPenStyle(gaugeDashStyle),
				Alignment = PenAlignment.Center
			};
		}

		private RectangleF GetSquareScaledAbsoluteRectangle(GaugeGraphics g, float scaleFactor, RectangleF rect)
		{
			RectangleF scaledAbsoluteRectangle = GetScaledAbsoluteRectangle(g, scaleFactor, rect);
			if (scaledAbsoluteRectangle.Width != scaledAbsoluteRectangle.Height)
			{
				if (scaledAbsoluteRectangle.Width > scaledAbsoluteRectangle.Height)
				{
					scaledAbsoluteRectangle.Offset((scaledAbsoluteRectangle.Width - scaledAbsoluteRectangle.Height) / 2f, 0f);
					scaledAbsoluteRectangle.Width = scaledAbsoluteRectangle.Height;
				}
				else if (scaledAbsoluteRectangle.Width < scaledAbsoluteRectangle.Height)
				{
					scaledAbsoluteRectangle.Offset(0f, (scaledAbsoluteRectangle.Height - scaledAbsoluteRectangle.Width) / 2f);
					scaledAbsoluteRectangle.Height = scaledAbsoluteRectangle.Width;
				}
			}
			return scaledAbsoluteRectangle;
		}

		private RectangleF GetScaledAbsoluteRectangle(GaugeGraphics g, float scaleFactor, RectangleF rect)
		{
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			if (scaleFactor != 1f)
			{
				absoluteRectangle.Inflate((absoluteRectangle.Width * scaleFactor - absoluteRectangle.Width) / 2f, (absoluteRectangle.Height * scaleFactor - absoluteRectangle.Height) / 2f);
			}
			return absoluteRectangle;
		}

		internal void DrawImage(GaugeGraphics g, string imageName, float scaleFactor, Color imageTransColor, Color imageHueColor, bool drawShadow)
		{
			if (drawShadow && ShadowOffset == 0f)
			{
				return;
			}
			Image image = Common.ImageLoader.LoadImage(imageName);
			if (image.Width == 0 || image.Height == 0)
			{
				return;
			}
			RectangleF scaledAbsoluteRectangle = GetScaledAbsoluteRectangle(g, scaleFactor, new RectangleF(0f, 0f, 100f, 100f));
			Rectangle empty = Rectangle.Empty;
			if (ResizeMode == ResizeMode.AutoFit)
			{
				empty = new Rectangle((int)scaledAbsoluteRectangle.X, (int)scaledAbsoluteRectangle.Y, (int)scaledAbsoluteRectangle.Width, (int)scaledAbsoluteRectangle.Height);
			}
			else
			{
				empty = new Rectangle(0, 0, image.Width, image.Height);
				PointF absolutePoint = g.GetAbsolutePoint(new PointF(50f, 50f));
				empty.X = (int)(absolutePoint.X - (float)(empty.Size.Width / 2));
				empty.Y = (int)(absolutePoint.Y - (float)(empty.Size.Height / 2));
			}
			ImageAttributes imageAttributes = new ImageAttributes();
			if (ImageTransColor != Color.Empty)
			{
				imageAttributes.SetColorKey(imageTransColor, imageTransColor, ColorAdjustType.Default);
			}
			float num = (100f - ImageTransparency) / 100f;
			float num2 = Common.GaugeCore.ShadowIntensity / 100f;
			if (drawShadow)
			{
				ColorMatrix colorMatrix = new ColorMatrix();
				colorMatrix.Matrix00 = 0f;
				colorMatrix.Matrix11 = 0f;
				colorMatrix.Matrix22 = 0f;
				colorMatrix.Matrix33 = num2 * num;
				imageAttributes.SetColorMatrix(colorMatrix);
			}
			else if (ImageTransparency > 0f)
			{
				ColorMatrix colorMatrix2 = new ColorMatrix();
				colorMatrix2.Matrix33 = num;
				imageAttributes.SetColorMatrix(colorMatrix2);
			}
			if (Angle != 0f)
			{
				PointF point = new PointF(scaledAbsoluteRectangle.X + scaledAbsoluteRectangle.Width / 2f, scaledAbsoluteRectangle.Y + scaledAbsoluteRectangle.Height / 2f);
				Matrix transform = g.Transform;
				Matrix matrix = g.Transform.Clone();
				float offsetX = matrix.OffsetX;
				float offsetY = matrix.OffsetY;
				point.X += offsetX;
				point.Y += offsetY;
				matrix.RotateAt(Angle, point, MatrixOrder.Append);
				if (drawShadow)
				{
					matrix.Translate(ShadowOffset, ShadowOffset, MatrixOrder.Append);
				}
				else if (!imageHueColor.IsEmpty)
				{
					Color color = g.TransformHueColor(imageHueColor);
					ColorMatrix colorMatrix3 = new ColorMatrix();
					colorMatrix3.Matrix00 = (float)(int)color.R / 255f;
					colorMatrix3.Matrix11 = (float)(int)color.G / 255f;
					colorMatrix3.Matrix22 = (float)(int)color.B / 255f;
					imageAttributes.SetColorMatrix(colorMatrix3);
				}
				g.Transform = matrix;
				ImageSmoothingState imageSmoothingState = new ImageSmoothingState(g);
				imageSmoothingState.Set();
				g.DrawImage(image, empty, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				imageSmoothingState.Restore();
				g.Transform = transform;
			}
			else
			{
				if (drawShadow)
				{
					empty.X += (int)ShadowOffset;
					empty.Y += (int)ShadowOffset;
				}
				else if (!imageHueColor.IsEmpty)
				{
					Color color2 = g.TransformHueColor(imageHueColor);
					ColorMatrix colorMatrix4 = new ColorMatrix();
					colorMatrix4.Matrix00 = (float)(int)color2.R / 255f;
					colorMatrix4.Matrix11 = (float)(int)color2.G / 255f;
					colorMatrix4.Matrix22 = (float)(int)color2.B / 255f;
					imageAttributes.SetColorMatrix(colorMatrix4);
				}
				ImageSmoothingState imageSmoothingState2 = new ImageSmoothingState(g);
				imageSmoothingState2.Set();
				g.DrawImage(image, empty, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				imageSmoothingState2.Restore();
			}
			if (drawShadow)
			{
				return;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(empty);
			if (Angle != 0f)
			{
				PointF point2 = new PointF(scaledAbsoluteRectangle.X + scaledAbsoluteRectangle.Width / 2f, scaledAbsoluteRectangle.Y + scaledAbsoluteRectangle.Height / 2f);
				using (Matrix matrix2 = new Matrix())
				{
					matrix2.RotateAt(Angle, point2, MatrixOrder.Append);
					graphicsPath.Transform(matrix2);
				}
			}
			Common.GaugeCore.HotRegionList.SetHotRegion(this, Point.Empty, graphicsPath);
		}

		internal State GetCurrentState()
		{
			foreach (State state in States)
			{
				if (state.GetDataState(Data).IsRangeActive)
				{
					return state;
				}
			}
			return null;
		}

		internal bool IsCircular()
		{
			if (IndicatorStyle == StateIndicatorStyle.CircularLed)
			{
				return true;
			}
			return false;
		}

		internal override void BeginInit()
		{
			base.BeginInit();
			data.BeginInit();
		}

		internal override void EndInit()
		{
			base.EndInit();
			ConnectToParent(exact: true);
			data.EndInit();
		}

		internal override void ReconnectData(bool exact)
		{
			data.ReconnectData(exact);
		}

		protected override void OnDispose()
		{
			States.Dispose();
			Data.Dispose();
			base.OnDispose();
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

		internal override void Notify(MessageType msg, NamedElement element, object param)
		{
			base.Notify(msg, element, param);
			switch (msg)
			{
			case MessageType.NamedElementRemove:
				if (parentSystem == element)
				{
					Parent = string.Empty;
				}
				break;
			case MessageType.NamedElementRename:
				if (parentSystem == element)
				{
					parent = element.GetNameAsParent((string)param);
				}
				break;
			}
			Data.Notify(msg, element, param);
			States.Notify(msg, element, param);
		}

		internal override void OnAdded()
		{
			base.OnAdded();
			ConnectToParent(exact: true);
			data.ReconnectData(exact: true);
		}

		void IRenderable.RenderStaticElements(GaugeGraphics g)
		{
		}

		void IRenderable.RenderDynamicElements(GaugeGraphics g)
		{
			if (!Visible)
			{
				return;
			}
			g.StartHotRegion(this);
			State state = GetCurrentState();
			if (state == null)
			{
				state = new State();
				state.IndicatorStyle = IndicatorStyle;
				state.ImageTransColor = ImageTransColor;
				state.ImageHueColor = ImageHueColor;
				state.Image = Image;
				state.ScaleFactor = ScaleFactor;
				state.FillColor = FillColor;
			}
			GraphicsPath graphicsPath = null;
			Brush brush = null;
			Brush brush2 = null;
			Pen pen = null;
			try
			{
				switch (state.IndicatorStyle)
				{
				case StateIndicatorStyle.None:
					return;
				case StateIndicatorStyle.Image:
					if (state.Image.Length != 0)
					{
						DrawImage(g, state.Image, state.ScaleFactor, state.ImageTransColor, state.ImageHueColor, drawShadow: true);
						DrawImage(g, state.Image, state.ScaleFactor, state.ImageTransColor, state.ImageHueColor, drawShadow: false);
						g.EndHotRegion();
					}
					return;
				case StateIndicatorStyle.Text:
				{
					graphicsPath = GetPath(g, state);
					if (graphicsPath == null)
					{
						return;
					}
					brush = GetBrush(g, state, graphicsPath.GetBounds());
					if (ShadowOffset != 0f)
					{
						using (Matrix matrix2 = new Matrix())
						{
							brush2 = g.GetShadowBrush();
							matrix2.Translate(ShadowOffset, ShadowOffset, MatrixOrder.Append);
							graphicsPath.Transform(matrix2);
							g.FillPath(brush2, graphicsPath);
							matrix2.Reset();
							matrix2.Translate(0f - ShadowOffset, 0f - ShadowOffset, MatrixOrder.Append);
							graphicsPath.Transform(matrix2);
						}
					}
					AntiAliasing antiAliasing = Common.GaugeContainer.AntiAliasing;
					AntiAliasing antiAliasing2 = g.AntiAliasing;
					if (Common.GaugeContainer.AntiAliasing == AntiAliasing.Text)
					{
						antiAliasing = AntiAliasing.Graphics;
					}
					else if (Common.GaugeContainer.AntiAliasing == AntiAliasing.Graphics)
					{
						antiAliasing = AntiAliasing.None;
					}
					g.AntiAliasing = antiAliasing;
					g.FillPath(brush, graphicsPath);
					g.AntiAliasing = antiAliasing2;
					using (GraphicsPath graphicsPath2 = new GraphicsPath())
					{
						graphicsPath2.AddRectangle(graphicsPath.GetBounds());
						Common.GaugeCore.HotRegionList.SetHotRegion(this, Point.Empty, (GraphicsPath)graphicsPath2.Clone());
					}
					return;
				}
				case StateIndicatorStyle.RectangularLed:
				case StateIndicatorStyle.CircularLed:
				case StateIndicatorStyle.RoundedRectangularLed:
					graphicsPath = GetPath(g, state);
					if (graphicsPath == null)
					{
						return;
					}
					brush = GetBrush(g, state, graphicsPath.GetBounds());
					if (ShadowOffset != 0f)
					{
						using (Matrix matrix = new Matrix())
						{
							brush2 = g.GetShadowBrush();
							matrix.Translate(ShadowOffset, ShadowOffset, MatrixOrder.Append);
							graphicsPath.Transform(matrix);
							g.FillPath(brush2, graphicsPath);
							matrix.Reset();
							matrix.Translate(0f - ShadowOffset, 0f - ShadowOffset, MatrixOrder.Append);
							graphicsPath.Transform(matrix);
						}
					}
					g.FillPath(brush, graphicsPath);
					if (state.IndicatorStyle != StateIndicatorStyle.Text && !IsXamlMarker(state.IndicatorStyle))
					{
						pen = GetPen(g, state);
						if (pen != null)
						{
							g.DrawPath(pen, graphicsPath);
						}
					}
					Common.GaugeCore.HotRegionList.SetHotRegion(this, Point.Empty, (GraphicsPath)graphicsPath.Clone());
					return;
				}
				graphicsPath = GetPath(g, state);
				if (graphicsPath == null)
				{
					return;
				}
				XamlRenderer xamlRenderer = GetXamlRenderer(g, state);
				if (xamlRenderer != null)
				{
					for (int i = 0; i < xamlRenderer.Layers.Length; i++)
					{
						if (i == 0)
						{
							if (ShadowOffset != 0f)
							{
								brush2 = g.GetShadowBrush();
								xamlRenderer.Layers[i].SetSingleBrush(brush2);
								xamlRenderer.Layers[i].Render(g);
							}
						}
						else
						{
							xamlRenderer.Layers[i].Render(g);
						}
					}
				}
				Common.GaugeCore.HotRegionList.SetHotRegion(this, Point.Empty, (GraphicsPath)graphicsPath.Clone());
			}
			finally
			{
				graphicsPath?.Dispose();
				brush2?.Dispose();
				brush?.Dispose();
				pen?.Dispose();
				g.EndHotRegion();
			}
		}

		int IRenderable.GetZOrder()
		{
			return ZOrder;
		}

		RectangleF IRenderable.GetBoundRect(GaugeGraphics g)
		{
			return Position.Rectangle;
		}

		object IRenderable.GetParentRenderable()
		{
			return ParentObject;
		}

		string IRenderable.GetParentRenderableName()
		{
			return parent;
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
			foreach (Range state in States)
			{
				state.PointerValueChanged(Data);
			}
			InternalValue = Data.Value;
		}

		void ISelectable.DrawSelection(GaugeGraphics g, bool designTimeSelection)
		{
			Stack stack = new Stack();
			for (NamedElement namedElement = ParentObject; namedElement != null; namedElement = (NamedElement)((IRenderable)namedElement).GetParentRenderable())
			{
				stack.Push(namedElement);
			}
			foreach (IRenderable item in stack)
			{
				g.CreateDrawRegion(item.GetBoundRect(g));
			}
			g.CreateDrawRegion(((IRenderable)this).GetBoundRect(g));
			g.DrawSelection(g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f)), -3f / g.Graphics.PageScale, designTimeSelection, Common.GaugeCore.SelectionBorderColor, Common.GaugeCore.SelectionMarkerColor);
			g.RestoreDrawRegion();
			foreach (IRenderable item2 in stack)
			{
				_ = item2;
				g.RestoreDrawRegion();
			}
		}

		public override object Clone()
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatSerializer binaryFormatSerializer = new BinaryFormatSerializer();
			binaryFormatSerializer.Serialize(this, stream);
			StateIndicator stateIndicator = new StateIndicator();
			binaryFormatSerializer.Deserialize(stateIndicator, stream);
			return stateIndicator;
		}
	}
}

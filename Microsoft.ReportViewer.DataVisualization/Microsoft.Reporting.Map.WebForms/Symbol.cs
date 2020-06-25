using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(SymbolConverter))]
	internal class Symbol : NamedElement, IContentElement, ILayerElement, IToolTipProvider, ISelectable, ISpatialElement, IImageMapProvider
	{
		private GraphicsPath[] cachedPaths;

		private RectangleF[] cachedPathBounds;

		internal Hashtable fields;

		private string fieldDataBuffer = string.Empty;

		internal PointF precalculatedCenterPoint = PointF.Empty;

		private XamlRenderer[] xamlRenderers;

		private SymbolData symbolData;

		private Offset offset;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private TextAlignment textAlignment = TextAlignment.Bottom;

		private bool visible = true;

		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private Color borderColor = Color.DarkGray;

		private MapDashStyle borderStyle = MapDashStyle.Solid;

		private int borderWidth = 1;

		private Color color = Color.Red;

		private Color textColor = Color.Black;

		private GradientType gradientType;

		private Color secondaryColor = Color.Empty;

		private MapHatchStyle hatchStyle;

		private string text = "";

		private int shadowOffset;

		private int textShadowOffset;

		private bool selected;

		private MarkerStyle markerStyle = MarkerStyle.Circle;

		private float width = 7f;

		private float height = 7f;

		private ResizeMode imageResizeMode = ResizeMode.AutoFit;

		private string image = "";

		private Color imageTransColor = Color.Empty;

		private string category = string.Empty;

		private string parentShape = "(none)";

		private Shape parentShapeObject;

		private bool useInternalProperties;

		private Color borderColorInt = Color.DarkGray;

		private MapDashStyle borderStyleInt = MapDashStyle.Solid;

		private int borderWidthInt = 1;

		private Color colorInt = Color.Red;

		private Color secondaryColorInt = Color.Empty;

		private GradientType gradientTypeInt;

		private MapHatchStyle hatchStyleInt;

		private string textInt = "";

		private TextAlignment textAlignmentInt = TextAlignment.Bottom;

		private string toolTipInt = "";

		private Font fontInt = new Font("Microsoft Sans Serif", 8.25f);

		private Color textColorInt = Color.Black;

		private int textShadowOffsetInt;

		private MarkerStyle markerStyleInt = MarkerStyle.Circle;

		private float widthInt = 7f;

		private float heightInt = 7f;

		private int shadowOffsetInt;

		private string imageInt = "";

		private Color imageTransColorInt = Color.Empty;

		private ResizeMode imageResizeModeInt = ResizeMode.AutoFit;

		private bool visibleInt = true;

		private object mapAreaTag;

		private string layer = "(none)";

		private bool belongsToLayer;

		private bool belongsToAllLayers;

		private Layer layerObject;

		MapPoint ISpatialElement.MinimumExtent => SymbolData.MinimumExtent;

		MapPoint ISpatialElement.MaximumExtent => SymbolData.MaximumExtent;

		[SRDescription("DescriptionAttributeSymbol_SymbolData")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public SymbolData SymbolData
		{
			get
			{
				return symbolData;
			}
			set
			{
				symbolData = value;
				ResetCachedPaths();
				InvalidateCachedBounds();
				InvalidateViewport();
			}
		}

		MapPoint[] ISpatialElement.Points => SymbolData.Points;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string EncodedSymbolData
		{
			get
			{
				return SymbolData.SymbolDataToString(SymbolData);
			}
			set
			{
				symbolData = SymbolData.SymbolDataFromString(value);
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue("")]
		public string FieldData
		{
			get
			{
				return FieldDataToString();
			}
			set
			{
				fieldDataBuffer = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeSymbol_Offset")]
		[TypeConverter(typeof(ShapeOffsetConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Offset Offset
		{
			get
			{
				return offset;
			}
			set
			{
				offset = value;
				offset.Parent = this;
				ResetCachedPaths();
				InvalidateCachedBounds();
				InvalidateViewport();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeSymbol_ToolTip")]
		[Localizable(true)]
		[TypeConverter(typeof(KeywordConverter))]
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
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeSymbol_Href")]
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

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeSymbol_MapAreaAttributes")]
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

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSymbol_Name")]
		public sealed override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeSymbol_TextAlignment")]
		[DefaultValue(TextAlignment.Bottom)]
		public TextAlignment TextAlignment
		{
			get
			{
				return textAlignment;
			}
			set
			{
				textAlignment = value;
				ResetCachedPaths();
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeSymbol_Visible")]
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
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeSymbol_Font")]
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
				ResetCachedPaths();
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeSymbol_BorderColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "DarkGray")]
		public Color BorderColor
		{
			get
			{
				return borderColor;
			}
			set
			{
				borderColor = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeSymbol_BorderStyle")]
		[DefaultValue(MapDashStyle.Solid)]
		public MapDashStyle BorderStyle
		{
			get
			{
				return borderStyle;
			}
			set
			{
				borderStyle = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeSymbol_BorderWidth")]
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
					throw new ArgumentException(SR.must_in_range(0.0, 100.0));
				}
				borderWidth = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeSymbol_Color")]
		[DefaultValue(typeof(Color), "Red")]
		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				if (color != value)
				{
					color = value;
					ResetCachedXamlRenderers();
					InvalidateViewport();
				}
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeSymbol_TextColor")]
		[DefaultValue(typeof(Color), "Black")]
		public Color TextColor
		{
			get
			{
				return textColor;
			}
			set
			{
				textColor = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeSymbol_GradientType")]
		[DefaultValue(GradientType.None)]
		public GradientType GradientType
		{
			get
			{
				return gradientType;
			}
			set
			{
				gradientType = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeSymbol_SecondaryColor")]
		[DefaultValue(typeof(Color), "")]
		public Color SecondaryColor
		{
			get
			{
				return secondaryColor;
			}
			set
			{
				secondaryColor = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeSymbol_HatchStyle")]
		[DefaultValue(MapHatchStyle.None)]
		public MapHatchStyle HatchStyle
		{
			get
			{
				return hatchStyle;
			}
			set
			{
				hatchStyle = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeSymbol_Text")]
		[Localizable(true)]
		[TypeConverter(typeof(KeywordConverter))]
		[DefaultValue("")]
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
				ResetCachedPaths();
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeSymbol_ShadowOffset")]
		[NotifyParentProperty(true)]
		[DefaultValue(0)]
		public int ShadowOffset
		{
			get
			{
				return shadowOffset;
			}
			set
			{
				if (value < -100 || value > 100)
				{
					throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
				}
				shadowOffset = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeSymbol_TextShadowOffset")]
		[NotifyParentProperty(true)]
		[DefaultValue(0)]
		public int TextShadowOffset
		{
			get
			{
				return textShadowOffset;
			}
			set
			{
				if (value < -100 || value > 100)
				{
					throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
				}
				textShadowOffset = value;
				InvalidateViewport();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeSymbol_Selected")]
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
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeSymbol_MarkerStyle")]
		[DefaultValue(MarkerStyle.Circle)]
		public MarkerStyle MarkerStyle
		{
			get
			{
				return markerStyle;
			}
			set
			{
				if (markerStyle != value)
				{
					markerStyle = value;
					ResetCachedPaths();
					InvalidateViewport();
				}
			}
		}

		[SRCategory("CategoryAttribute_Size")]
		[SRDescription("DescriptionAttributeSymbol_Width")]
		[DefaultValue(7f)]
		public float Width
		{
			get
			{
				return width;
			}
			set
			{
				if (value < 0f || value > 1000f)
				{
					throw new ArgumentException(SR.must_in_range(0.0, 1000.0));
				}
				width = value;
				ResetCachedPaths();
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Size")]
		[SRDescription("DescriptionAttributeSymbol_Height")]
		[DefaultValue(7f)]
		public float Height
		{
			get
			{
				return height;
			}
			set
			{
				if (value < 0f || value > 1000f)
				{
					throw new ArgumentException(SR.must_in_range(0.0, 1000.0));
				}
				height = value;
				ResetCachedPaths();
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Coordinates")]
		[SRDescription("DescriptionAttributeSymbol_X")]
		[DefaultValue(typeof(MapCoordinate), "0d")]
		public MapCoordinate X
		{
			get
			{
				return new MapCoordinate(SymbolData.Points[0].X);
			}
			set
			{
				SymbolData.Points[0].X = value.ToDouble();
				SymbolData.UpdateStoredParameters();
				InvalidateCachedBounds();
				ResetCachedPaths();
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Coordinates")]
		[SRDescription("DescriptionAttributeSymbol_Y")]
		[DefaultValue(typeof(MapCoordinate), "0d")]
		public MapCoordinate Y
		{
			get
			{
				return new MapCoordinate(SymbolData.Points[0].Y);
			}
			set
			{
				SymbolData.Points[0].Y = value.ToDouble();
				SymbolData.UpdateStoredParameters();
				InvalidateCachedBounds();
				ResetCachedPaths();
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeSymbol_ImageResizeMode")]
		[DefaultValue(ResizeMode.AutoFit)]
		public ResizeMode ImageResizeMode
		{
			get
			{
				return imageResizeMode;
			}
			set
			{
				imageResizeMode = value;
				ResetCachedPaths();
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeSymbol_Image")]
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
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeSymbol_ImageTransColor")]
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
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSymbol_Category")]
		[DefaultValue("")]
		public string Category
		{
			get
			{
				return category;
			}
			set
			{
				category = value;
				InvalidateRules();
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeSymbol_ParentShape")]
		[TypeConverter(typeof(DesignTimeShapeConverter))]
		[DefaultValue("(none)")]
		public string ParentShape
		{
			get
			{
				return parentShape;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					parentShape = "(none)";
				}
				else
				{
					parentShape = value;
				}
				ParentShapeObject = null;
				InvalidateChildSymbols();
				InvalidateCachedBounds();
				InvalidateDistanceScalePanel();
				InvalidateViewport();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object this[string name]
		{
			get
			{
				return fields[name];
			}
			set
			{
				if (value == null || value is DBNull)
				{
					return;
				}
				MapCore mapCore = GetMapCore();
				if (mapCore != null)
				{
					Field field = (Field)mapCore.SymbolFields.GetByName(name);
					if (field == null)
					{
						throw new ArgumentException(SR.ExceptionFieldNameDoesNotExist(name));
					}
					if (!field.Type.IsInstanceOfType(value) && field.Type != Field.ConvertToSupportedType(value.GetType()))
					{
						throw new ArgumentException(SR.ExceptionFieldMustBeOfType(name, field.Type.ToString()));
					}
					field.SetValue(Field.ConvertToSupportedValue(value), fields);
					mapCore.InvalidateRules();
					mapCore.InvalidateDataBinding();
				}
				else
				{
					fields[name] = Field.ConvertToSupportedValue(value);
				}
				InvalidateViewport();
			}
		}

		internal Shape ParentShapeObject
		{
			get
			{
				if (parentShape == "(none)")
				{
					return null;
				}
				if (parentShapeObject == null)
				{
					MapCore mapCore = GetMapCore();
					if (mapCore != null)
					{
						parentShapeObject = (Shape)mapCore.Shapes.GetByName(parentShape);
					}
				}
				return parentShapeObject;
			}
			set
			{
				parentShapeObject = value;
			}
		}

		internal bool UseInternalProperties
		{
			get
			{
				return useInternalProperties;
			}
			set
			{
				useInternalProperties = value;
				InvalidateViewport();
			}
		}

		internal Color BorderColorInt
		{
			get
			{
				if (borderColor == Color.DarkGray && useInternalProperties)
				{
					return borderColorInt;
				}
				return borderColor;
			}
			set
			{
				borderColorInt = value;
				InvalidateViewport();
			}
		}

		internal MapDashStyle BorderStyleInt
		{
			get
			{
				if (borderStyle == MapDashStyle.Solid && useInternalProperties)
				{
					return borderStyleInt;
				}
				return borderStyle;
			}
			set
			{
				borderStyleInt = value;
				InvalidateViewport();
			}
		}

		internal int BorderWidthInt
		{
			get
			{
				if (borderWidth == 1 && useInternalProperties)
				{
					return borderWidthInt;
				}
				return borderWidth;
			}
			set
			{
				borderWidthInt = value;
				InvalidateViewport();
			}
		}

		internal Color ColorInt
		{
			get
			{
				if (color == Color.Red && useInternalProperties)
				{
					return colorInt;
				}
				return color;
			}
			set
			{
				if (colorInt != value)
				{
					colorInt = value;
					ResetCachedXamlRenderers();
					InvalidateViewport();
				}
			}
		}

		internal Color SecondaryColorInt
		{
			get
			{
				if (secondaryColor == Color.Empty && useInternalProperties)
				{
					return secondaryColorInt;
				}
				return secondaryColor;
			}
			set
			{
				secondaryColorInt = value;
				InvalidateViewport();
			}
		}

		internal GradientType GradientTypeInt
		{
			get
			{
				if (gradientType == GradientType.None && useInternalProperties)
				{
					return gradientTypeInt;
				}
				return gradientType;
			}
			set
			{
				gradientTypeInt = value;
				InvalidateViewport();
			}
		}

		internal MapHatchStyle HatchStyleInt
		{
			get
			{
				if (hatchStyle == MapHatchStyle.None && useInternalProperties)
				{
					return hatchStyleInt;
				}
				return hatchStyle;
			}
			set
			{
				hatchStyleInt = value;
				InvalidateViewport();
			}
		}

		internal string TextInt
		{
			get
			{
				if (string.IsNullOrEmpty(text) && useInternalProperties)
				{
					return textInt;
				}
				return text;
			}
			set
			{
				textInt = value;
				ResetCachedPaths();
				InvalidateViewport();
			}
		}

		internal TextAlignment TextAlignmentInt
		{
			get
			{
				if (textAlignment == TextAlignment.Bottom && useInternalProperties)
				{
					return textAlignmentInt;
				}
				return textAlignment;
			}
			set
			{
				textAlignmentInt = value;
				ResetCachedPaths();
				InvalidateViewport();
			}
		}

		internal string ToolTipInt
		{
			get
			{
				if (string.IsNullOrEmpty(toolTip) && useInternalProperties)
				{
					return toolTipInt;
				}
				return toolTip;
			}
			set
			{
				toolTipInt = value;
				InvalidateViewport();
			}
		}

		internal Font FontInt
		{
			get
			{
				if (font == new Font("Microsoft Sans Serif", 8.25f) && useInternalProperties)
				{
					return fontInt;
				}
				return font;
			}
			set
			{
				fontInt = value;
				ResetCachedPaths();
				InvalidateViewport();
			}
		}

		internal Color TextColorInt
		{
			get
			{
				if (textColor == Color.Black && useInternalProperties)
				{
					return textColorInt;
				}
				return textColor;
			}
			set
			{
				textColorInt = value;
				InvalidateViewport();
			}
		}

		internal int TextShadowOffsetInt
		{
			get
			{
				if (textShadowOffset == 0 && useInternalProperties)
				{
					return textShadowOffsetInt;
				}
				return textShadowOffset;
			}
			set
			{
				textShadowOffsetInt = value;
				InvalidateViewport();
			}
		}

		internal MarkerStyle MarkerStyleInt
		{
			get
			{
				if (markerStyle == MarkerStyle.Circle && useInternalProperties)
				{
					return markerStyleInt;
				}
				return markerStyle;
			}
			set
			{
				if (markerStyleInt != value)
				{
					markerStyleInt = value;
					ResetCachedPaths();
					InvalidateViewport();
				}
			}
		}

		internal float WidthInt
		{
			get
			{
				if (width == 7f && useInternalProperties)
				{
					return widthInt;
				}
				return width;
			}
			set
			{
				widthInt = value;
				ResetCachedPaths();
				InvalidateViewport();
			}
		}

		internal float HeightInt
		{
			get
			{
				if (height == 7f && useInternalProperties)
				{
					return heightInt;
				}
				return height;
			}
			set
			{
				heightInt = value;
				ResetCachedPaths();
				InvalidateViewport();
			}
		}

		internal int ShadowOffsetInt
		{
			get
			{
				if (shadowOffset == 0 && useInternalProperties)
				{
					return shadowOffsetInt;
				}
				return shadowOffset;
			}
			set
			{
				shadowOffsetInt = value;
				InvalidateViewport();
			}
		}

		internal string ImageInt
		{
			get
			{
				if (string.IsNullOrEmpty(image) && useInternalProperties)
				{
					return imageInt;
				}
				return image;
			}
			set
			{
				imageInt = value;
				InvalidateViewport();
			}
		}

		internal Color ImageTransColorInt
		{
			get
			{
				if (imageTransColor == Color.Empty && useInternalProperties)
				{
					return imageTransColorInt;
				}
				return imageTransColor;
			}
			set
			{
				imageTransColorInt = value;
				InvalidateViewport();
			}
		}

		internal ResizeMode ImageResizeModeInt
		{
			get
			{
				if (imageResizeMode == ResizeMode.AutoFit && useInternalProperties)
				{
					return imageResizeModeInt;
				}
				return imageResizeMode;
			}
			set
			{
				imageResizeModeInt = value;
				ResetCachedPaths();
				InvalidateViewport();
			}
		}

		internal bool VisibleInt
		{
			get
			{
				if (visible && useInternalProperties)
				{
					return visibleInt;
				}
				return visible;
			}
			set
			{
				visibleInt = value;
				InvalidateViewport();
			}
		}

		object IImageMapProvider.Tag
		{
			get
			{
				return mapAreaTag;
			}
			set
			{
				mapAreaTag = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeSymbol_Layer")]
		[TypeConverter(typeof(DesignTimeLayerConverter))]
		[DefaultValue("(none)")]
		public string Layer
		{
			get
			{
				return layer;
			}
			set
			{
				if (layer != value)
				{
					if (string.IsNullOrEmpty(value) || value == "(none)")
					{
						layer = "(none)";
						belongsToLayer = false;
						belongsToAllLayers = false;
					}
					else if (value == "(all)")
					{
						layer = value;
						belongsToLayer = true;
						belongsToAllLayers = true;
					}
					else
					{
						layer = value;
						belongsToLayer = true;
						belongsToAllLayers = false;
					}
					InvalidateViewport();
				}
			}
		}

		bool ILayerElement.BelongsToLayer => belongsToLayer;

		bool ILayerElement.BelongsToAllLayers => belongsToAllLayers;

		Layer ILayerElement.LayerObject
		{
			get
			{
				if (layerObject != null)
				{
					return layerObject;
				}
				MapCore mapCore = GetMapCore();
				if (belongsToLayer && !belongsToAllLayers && layerObject == null && mapCore != null)
				{
					layerObject = mapCore.Layers[Layer];
				}
				return layerObject;
			}
			set
			{
				layerObject = value;
				if (value != null)
				{
					layer = value.Name;
					belongsToAllLayers = false;
					belongsToLayer = true;
				}
				else
				{
					layer = "(none)";
					belongsToAllLayers = false;
					belongsToLayer = false;
				}
				InvalidateViewport();
			}
		}

		public Symbol()
			: this(null)
		{
		}

		internal Symbol(CommonElements common)
			: base(common)
		{
			symbolData = new SymbolData();
			fields = new Hashtable();
			offset = new Offset(this, 0.0, 0.0);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected bool ShouldSerializeOffset()
		{
			if (Offset.X == 0.0)
			{
				return Offset.Y != 0.0;
			}
			return true;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected void ResetOffset()
		{
			Offset.X = 0.0;
			Offset.Y = 0.0;
		}

		public override string ToString()
		{
			return Name;
		}

		public void AddPoints(MapPoint[] points)
		{
			if (points == null)
			{
				throw new ArgumentException(SR.ExceptionArrayOfPointsCannotBeNull);
			}
			MapPoint[] array = new MapPoint[SymbolData.Points.Length + points.Length];
			Array.Copy(SymbolData.Points, array, SymbolData.Points.Length);
			Array.Copy(points, 0, array, SymbolData.Points.Length, points.Length);
			SymbolData.Points = array;
			SymbolData.UpdateStoredParameters();
			InvalidateChildSymbols();
			InvalidateCachedBounds();
			InvalidateRules();
			InvalidateViewport();
		}

		public void ClearSymbolData()
		{
			X = 0.0;
			Y = 0.0;
			SymbolData.Points = new MapPoint[1]
			{
				new MapPoint(X, Y)
			};
			SymbolData.UpdateStoredParameters();
			InvalidateChildSymbols();
			InvalidateCachedBounds();
			InvalidateRules();
			InvalidateViewport();
		}

		public void SetPoints(MapPoint[] points)
		{
			if (points == null)
			{
				throw new ArgumentException(SR.ExceptionArrayOfPointsCannotBeNull);
			}
			if (points.Length < 1)
			{
				throw new ArgumentException(SR.ExceptionArrayOfPointsMustContainOnePoint);
			}
			SymbolData.Points = points;
			SymbolData.UpdateStoredParameters();
			InvalidateCachedBounds();
			InvalidateRules();
			InvalidateViewport();
		}

		public void SetCoordinates(double longitude, double latitude)
		{
			X = longitude;
			Y = latitude;
		}

		public void SetCoordinates(string longitude, string latitude)
		{
			X = longitude;
			Y = latitude;
		}

		public void SetCoordinates(string latitudeAndLongitude)
		{
			MatchCollection matchCollection = new Regex("[-+]?[0-9]*\\.?[0-9]+").Matches(latitudeAndLongitude);
			if (matchCollection.Count < 2 || matchCollection.Count == 3 || matchCollection.Count == 5 || matchCollection.Count > 6)
			{
				throw new ArgumentException(SR.ExceptionInvalidCoordonateString(latitudeAndLongitude));
			}
			string latitude = latitudeAndLongitude.Substring(0, matchCollection[matchCollection.Count / 2].Index);
			string longitude = latitudeAndLongitude.Substring(matchCollection[matchCollection.Count / 2].Index);
			SetCoordinates(longitude, latitude);
		}

		internal PointF GetCenterPointInContentPixels(MapGraphics g, int pointIndex, out bool visible)
		{
			visible = true;
			if (ParentShape != "(none)")
			{
				return new PointF(precalculatedCenterPoint.X + (float)Offset.X, precalculatedCenterPoint.Y - (float)Offset.Y);
			}
			if (GetMapCore() != null)
			{
				Point3D point3D = GetMapCore().GeographicToPercents(SymbolData.Points[pointIndex].X + Offset.X, SymbolData.Points[pointIndex].Y + Offset.Y);
				visible = (point3D.Z >= 0.0);
				return g.GetAbsolutePoint(point3D.ToPointF());
			}
			return PointF.Empty;
		}

		public PointF GetCenterPointInContentPixels(MapGraphics g, int pointIndex)
		{
			bool flag;
			return GetCenterPointInContentPixels(g, pointIndex, out flag);
		}

		internal MapCore GetMapCore()
		{
			return (MapCore)ParentElement;
		}

		private string FieldDataToString()
		{
			string text = string.Empty;
			MapCore mapCore = GetMapCore();
			if (mapCore != null)
			{
				foreach (Field symbolField in mapCore.SymbolFields)
				{
					if (!symbolField.IsTemporary)
					{
						string text2 = symbolField.FormatValue(fields[symbolField.Name]);
						if (!string.IsNullOrEmpty(text2))
						{
							text = text + XmlConvert.EncodeName(symbolField.Name) + "=" + text2 + "&";
						}
					}
				}
				text = text.TrimEnd('&');
			}
			return text;
		}

		internal void FieldDataFromBuffer()
		{
			if (fieldDataBuffer.Length == 0)
			{
				return;
			}
			MapCore mapCore = GetMapCore();
			if (mapCore != null)
			{
				fields.Clear();
				string[] array = fieldDataBuffer.Split('&');
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split('=');
					string name = XmlConvert.DecodeName(array2[0]);
					string fieldValue = XmlConvert.DecodeName(array2[1]);
					((Field)mapCore.SymbolFields.GetByName(name))?.ParseValue(fieldValue, fields);
				}
				fieldDataBuffer = "";
			}
		}

		internal override void OnAdded()
		{
			base.OnAdded();
		}

		internal override void OnRemove()
		{
			InvalidateChildSymbols();
			InvalidateViewport();
			base.OnRemove();
		}

		protected override void OnDispose()
		{
			ResetCachedPaths();
			SymbolData.Points = null;
			if (fields != null)
			{
				fields.Clear();
			}
			base.OnDispose();
		}

		internal void ApplyPredefinedSymbolAttributes(PredefinedSymbol predefinedSymbol, AffectedSymbolAttributes affectedAttributes)
		{
			UseInternalProperties = true;
			switch (affectedAttributes)
			{
			case AffectedSymbolAttributes.ColorOnly:
				ColorInt = predefinedSymbol.Color;
				return;
			case AffectedSymbolAttributes.MarkerOnly:
				MarkerStyleInt = predefinedSymbol.MarkerStyle;
				ImageInt = predefinedSymbol.Image;
				return;
			case AffectedSymbolAttributes.SizeOnly:
				WidthInt = predefinedSymbol.Width;
				HeightInt = predefinedSymbol.Height;
				return;
			}
			BorderColorInt = predefinedSymbol.BorderColor;
			BorderStyleInt = predefinedSymbol.BorderStyle;
			BorderWidthInt = predefinedSymbol.BorderWidth;
			ColorInt = predefinedSymbol.Color;
			SecondaryColorInt = predefinedSymbol.SecondaryColor;
			GradientTypeInt = predefinedSymbol.GradientType;
			HatchStyleInt = predefinedSymbol.HatchStyle;
			TextInt = predefinedSymbol.Text;
			TextAlignmentInt = predefinedSymbol.TextAlignment;
			ToolTipInt = predefinedSymbol.ToolTip;
			FontInt = predefinedSymbol.Font;
			TextColorInt = predefinedSymbol.TextColor;
			TextShadowOffsetInt = predefinedSymbol.TextShadowOffset;
			MarkerStyleInt = predefinedSymbol.MarkerStyle;
			WidthInt = predefinedSymbol.Width;
			HeightInt = predefinedSymbol.Height;
			ShadowOffsetInt = predefinedSymbol.ShadowOffset;
			ImageInt = predefinedSymbol.Image;
			ImageTransColorInt = predefinedSymbol.ImageTransColor;
			ImageResizeModeInt = predefinedSymbol.ImageResizeMode;
			VisibleInt = predefinedSymbol.Visible;
		}

		internal void InvalidateChildSymbols()
		{
			GetMapCore()?.InvalidateChildSymbols();
		}

		internal float GetWidth()
		{
			return Width;
		}

		internal void InvalidateRules()
		{
			MapCore mapCore = GetMapCore();
			if (mapCore != null)
			{
				mapCore.InvalidateRules();
				mapCore.Invalidate();
			}
		}

		internal static XamlRenderer CreateXamlRenderer(MarkerStyle markerStyle, Color color, RectangleF rect)
		{
			XamlRenderer xamlRenderer = new XamlRenderer(markerStyle.ToString() + ".xaml");
			xamlRenderer.AllowPathGradientTransform = false;
			xamlRenderer.ParseXaml(rect, new Color[1]
			{
				color
			});
			return xamlRenderer;
		}

		internal XamlRenderer[] GetXamlRenderers(MapGraphics g)
		{
			if (xamlRenderers != null)
			{
				return xamlRenderers;
			}
			GraphicsPath[] paths = GetPaths(g);
			XamlRenderer[] array = new XamlRenderer[paths.Length - 1];
			for (int i = 0; i < paths.Length - 1; i++)
			{
				if (paths[i] != null)
				{
					array[i] = CreateXamlRenderer(MarkerStyleInt, ColorInt, cachedPathBounds[i]);
				}
			}
			xamlRenderers = array;
			return xamlRenderers;
		}

		internal void ResetCachedXamlRenderers()
		{
			if (xamlRenderers != null)
			{
				XamlRenderer[] array = xamlRenderers;
				for (int i = 0; i < array.Length; i++)
				{
					array[i]?.Dispose();
				}
				xamlRenderers = null;
			}
		}

		internal static bool IsXamlMarker(MarkerStyle markerStyle)
		{
			if (markerStyle == MarkerStyle.PushPin)
			{
				return true;
			}
			return false;
		}

		internal static RectangleF CalculateXamlMarkerBounds(MarkerStyle markerStyle, PointF centerPoint, float width, float height)
		{
			RectangleF result = RectangleF.Empty;
			if (markerStyle == MarkerStyle.PushPin)
			{
				result = new RectangleF(centerPoint.X, centerPoint.Y, 0f, 0f);
				result.Inflate(width, height);
				result.Offset(0f - width, 0f - height);
			}
			return result;
		}

		public string SaveWKT()
		{
			if (SymbolData.Points.Length == 1)
			{
				return "POINT(" + X.ToDouble().ToString(CultureInfo.InvariantCulture) + " " + Y.ToDouble().ToString(CultureInfo.InvariantCulture) + ")";
			}
			MapPoint[] points = SymbolData.Points;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("MULTIPOINT(");
			for (int i = 0; i < points.Length; i++)
			{
				stringBuilder.Append("(" + points[i].X.ToString(CultureInfo.InvariantCulture) + " " + points[i].Y.ToString(CultureInfo.InvariantCulture) + ")");
				if (i < points.Length - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public byte[] SaveWKB()
		{
			MemoryStream memoryStream = new MemoryStream();
			SaveWKBToStream(memoryStream);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return memoryStream.ToArray();
		}

		private void SaveWKBToStream(Stream stream)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			byte value = (byte)(BitConverter.IsLittleEndian ? 1 : 0);
			if (SymbolData.Points.Length == 1)
			{
				binaryWriter.Write(value);
				binaryWriter.Write(1u);
				binaryWriter.Write(X.ToDouble());
				binaryWriter.Write(Y.ToDouble());
				return;
			}
			MapPoint[] points = SymbolData.Points;
			binaryWriter.Write(value);
			binaryWriter.Write(4u);
			binaryWriter.Write((uint)points.Length);
			for (int i = 0; i < points.Length; i++)
			{
				binaryWriter.Write(value);
				binaryWriter.Write(1u);
				binaryWriter.Write(points[i].X);
				binaryWriter.Write(points[i].Y);
			}
		}

		internal GraphicsPath[] GetPaths(MapGraphics g)
		{
			if (!VisibleInt)
			{
				return null;
			}
			if (cachedPaths != null)
			{
				return cachedPaths;
			}
			GraphicsPath[] array = new GraphicsPath[SymbolData.Points.Length + 1];
			RectangleF[] array2 = new RectangleF[SymbolData.Points.Length + 1];
			for (int i = 0; i < SymbolData.Points.Length; i++)
			{
				bool flag;
				PointF centerPointInContentPixels = GetCenterPointInContentPixels(g, i, out flag);
				if (flag)
				{
					if (ImageInt != string.Empty)
					{
						Image image = Common.ImageLoader.LoadImage(ImageInt);
						RectangleF rectangleF = new RectangleF(centerPointInContentPixels.X, centerPointInContentPixels.Y, 0f, 0f);
						if (ImageResizeModeInt == ResizeMode.AutoFit)
						{
							rectangleF.Inflate(WidthInt / 2f, HeightInt / 2f);
						}
						else
						{
							rectangleF.Inflate((float)image.Width / 2f, (float)image.Height / 2f);
						}
						GraphicsPath graphicsPath = new GraphicsPath();
						graphicsPath.AddRectangle(rectangleF);
						array[i] = graphicsPath;
						array2[i] = rectangleF;
					}
					else if (IsXamlMarker(MarkerStyleInt))
					{
						RectangleF rectangleF2 = CalculateXamlMarkerBounds(MarkerStyleInt, centerPointInContentPixels, WidthInt, HeightInt);
						GraphicsPath graphicsPath2 = new GraphicsPath();
						graphicsPath2.AddRectangle(rectangleF2);
						array[i] = graphicsPath2;
						array2[i] = rectangleF2;
					}
					else
					{
						array2[i] = (array[i] = g.CreateMarker(centerPointInContentPixels, WidthInt, HeightInt, MarkerStyleInt)).GetBounds();
					}
				}
				else
				{
					array[i] = null;
					array2[i] = RectangleF.Empty;
				}
			}
			if (TextInt != string.Empty && IsLabelVisible() && array[0] != null)
			{
				GraphicsPath graphicsPath3 = new GraphicsPath();
				RectangleF labelRect = GetLabelRect(g, array2[0]);
				graphicsPath3.AddRectangle(labelRect);
				array[array.Length - 1] = graphicsPath3;
				array2[array2.Length - 1] = labelRect;
			}
			else
			{
				array[array.Length - 1] = null;
			}
			cachedPaths = array;
			cachedPathBounds = array2;
			return cachedPaths;
		}

		internal void InvalidateCachedBounds()
		{
			GetMapCore()?.InvalidateCachedBounds();
		}

		internal void ResetCachedPaths()
		{
			ResetCachedXamlRenderers();
			if (cachedPaths != null)
			{
				GraphicsPath[] array = cachedPaths;
				for (int i = 0; i < array.Length; i++)
				{
					array[i]?.Dispose();
				}
			}
			cachedPaths = null;
			cachedPathBounds = null;
		}

		private bool IsLabelVisible()
		{
			MapCore mapCore = GetMapCore();
			if (mapCore != null)
			{
				if (((ILayerElement)this).BelongsToLayer)
				{
					if (!((ILayerElement)this).BelongsToAllLayers)
					{
						if (!mapCore.Layers[((ILayerElement)this).Layer].LabelVisible)
						{
							return false;
						}
					}
					else if (!mapCore.Layers.HasVisibleLayer())
					{
						return false;
					}
				}
			}
			return true;
		}

		private RectangleF GetLabelRect(MapGraphics g, RectangleF symbolRect)
		{
			PointF centerPointInContentPixels = GetCenterPointInContentPixels(g, 0);
			string text = (TextInt.IndexOf("#", StringComparison.Ordinal) == -1) ? TextInt : GetMapCore().ResolveAllKeywords(TextInt, this);
			text = text.Replace("\\n", "\n");
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			SizeF sizeF = g.MeasureString(text, FontInt, new SizeF(0f, 0f), stringFormat);
			RectangleF result = new RectangleF(centerPointInContentPixels.X, centerPointInContentPixels.Y, 0f, 0f);
			result.Inflate(sizeF.Width / 2f, sizeF.Height / 2f);
			if (TextAlignmentInt == TextAlignment.Left)
			{
				float num = symbolRect.Width / 2f + result.Width / 2f + 3f;
				result.X -= num;
			}
			else if (TextAlignmentInt == TextAlignment.Right)
			{
				float num2 = symbolRect.Width / 2f + result.Width / 2f + 3f;
				result.X += num2;
			}
			else if (TextAlignmentInt == TextAlignment.Top)
			{
				float num3 = symbolRect.Height / 2f + result.Height / 2f + 3f;
				result.Y -= num3;
			}
			else if (TextAlignmentInt == TextAlignment.Bottom)
			{
				float num4 = symbolRect.Height / 2f + result.Height / 2f + 3f;
				result.Y += num4;
			}
			if (MarkerStyleInt == MarkerStyle.PushPin)
			{
				result.Offset(0f - WidthInt, 0f - HeightInt);
			}
			return result;
		}

		private void RenderText(MapGraphics g)
		{
			GraphicsPath[] paths = GetPaths(g);
			if (string.IsNullOrEmpty(TextInt) || paths.Length == 0)
			{
				return;
			}
			string text = (TextInt.IndexOf("#", StringComparison.Ordinal) == -1) ? TextInt : GetMapCore().ResolveAllKeywords(TextInt, this);
			text = text.Replace("\\n", "\n");
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			RectangleF rectangleF = cachedPathBounds[cachedPathBounds.Length - 1];
			PointF point = new PointF(rectangleF.Left + rectangleF.Width / 2f, rectangleF.Top + rectangleF.Height / 2f);
			if (TextShadowOffsetInt != 0)
			{
				using (Brush brush = g.GetShadowBrush())
				{
					g.DrawString(point: new PointF(point.X + (float)TextShadowOffsetInt, point.Y + (float)TextShadowOffsetInt), s: text, font: FontInt, brush: brush, format: stringFormat);
				}
			}
			using (Brush brush2 = new SolidBrush(TextColorInt))
			{
				g.DrawString(text, FontInt, brush2, point, stringFormat);
			}
		}

		internal bool IsRectangleVisible(MapGraphics g, RectangleF clipRect, MapPoint minExtent, MapPoint maxExtent)
		{
			PointF pointF = GetMapCore().GeographicToPercents(minExtent).ToPointF();
			PointF pointF2 = GetMapCore().GeographicToPercents(maxExtent).ToPointF();
			RectangleF relative = new RectangleF(pointF.X, pointF2.Y, pointF2.X - pointF.X, pointF.Y - pointF2.Y);
			relative = g.GetAbsoluteRectangle(relative);
			return clipRect.IntersectsWith(relative);
		}

		internal Brush GetBackBrush(MapGraphics g, GraphicsPath path)
		{
			RectangleF bounds = path.GetBounds();
			Brush brush = null;
			Color color = ApplyLayerTransparency(ColorInt);
			Color color2 = ApplyLayerTransparency(SecondaryColorInt);
			GradientType gradientType = GradientTypeInt;
			MapHatchStyle mapHatchStyle = HatchStyleInt;
			if (mapHatchStyle != 0)
			{
				return MapGraphics.GetHatchBrush(mapHatchStyle, color, color2);
			}
			if (gradientType != 0)
			{
				return g.GetGradientBrush(bounds, color, color2, gradientType);
			}
			return new SolidBrush(color);
		}

		internal Pen GetPen()
		{
			return new Pen(ApplyLayerTransparency(BorderColorInt), BorderWidthInt)
			{
				DashStyle = MapGraphics.GetPenStyle(BorderStyleInt),
				Alignment = PenAlignment.Center,
				LineJoin = LineJoin.Round
			};
		}

		internal RectangleF[] DrawImage(MapGraphics g, string imageName, bool drawShadow)
		{
			if (drawShadow && ShadowOffsetInt == 0)
			{
				return null;
			}
			Image image = Common.ImageLoader.LoadImage(imageName);
			if (image.Width == 0 || image.Height == 0)
			{
				return null;
			}
			ImageAttributes imageAttributes = new ImageAttributes();
			if (ImageTransColorInt != Color.Empty)
			{
				imageAttributes.SetColorKey(ImageTransColorInt, ImageTransColorInt, ColorAdjustType.Default);
			}
			float num = 1f;
			if (((ILayerElement)this).LayerObject != null && ((ILayerElement)this).LayerObject.Transparency > 0f)
			{
				num = (100f - ((ILayerElement)this).LayerObject.Transparency) / 100f;
			}
			if (drawShadow)
			{
				float num2 = Common.MapCore.ShadowIntensity / 100f;
				ColorMatrix colorMatrix = new ColorMatrix();
				colorMatrix.Matrix00 = 0f;
				colorMatrix.Matrix11 = 0f;
				colorMatrix.Matrix22 = 0f;
				colorMatrix.Matrix33 = num2 * num;
				imageAttributes.SetColorMatrix(colorMatrix);
			}
			else if (num < 1f)
			{
				ColorMatrix colorMatrix2 = new ColorMatrix();
				colorMatrix2.Matrix33 = num;
				imageAttributes.SetColorMatrix(colorMatrix2);
			}
			RectangleF[] array = new RectangleF[SymbolData.Points.Length];
			for (int i = 0; i < SymbolData.Points.Length; i++)
			{
				bool flag;
				PointF centerPointInContentPixels = GetCenterPointInContentPixels(g, i, out flag);
				if (flag)
				{
					RectangleF rectangleF = new RectangleF(centerPointInContentPixels.X, centerPointInContentPixels.Y, 0f, 0f);
					rectangleF.Inflate(WidthInt / 2f, HeightInt / 2f);
					Rectangle empty = Rectangle.Empty;
					if (ImageResizeModeInt == ResizeMode.AutoFit)
					{
						empty = new Rectangle((int)rectangleF.X, (int)rectangleF.Y, (int)rectangleF.Width, (int)rectangleF.Height);
					}
					else
					{
						empty = new Rectangle(0, 0, image.Width, image.Height);
						empty.X = (int)Math.Round(centerPointInContentPixels.X - (float)empty.Size.Width / 2f);
						empty.Y = (int)Math.Round(centerPointInContentPixels.Y - (float)empty.Size.Height / 2f);
					}
					if (drawShadow)
					{
						empty.X += ShadowOffsetInt;
						empty.Y += ShadowOffsetInt;
					}
					g.DrawImage(image, empty, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
					array[i] = new RectangleF(empty.X, empty.Y, empty.Width, empty.Height);
				}
				else
				{
					array[i] = RectangleF.Empty;
				}
			}
			return array;
		}

		private Color ApplyLayerTransparency(Color color)
		{
			if (((ILayerElement)this).LayerObject == null || ((ILayerElement)this).LayerObject.Transparency == 0f)
			{
				return color;
			}
			return Color.FromArgb((int)Math.Round((100f - ((ILayerElement)this).LayerObject.Transparency) / 100f * (float)(int)color.A), color);
		}

		bool IContentElement.IsVisible(MapGraphics g, Layer layer, bool allLayers, RectangleF clipRect)
		{
			if (!VisibleInt)
			{
				return false;
			}
			if (allLayers)
			{
				if (!((ILayerElement)this).BelongsToAllLayers)
				{
					return false;
				}
			}
			else if (layer != null)
			{
				if (!((ILayerElement)this).BelongsToLayer || layer.Name != ((ILayerElement)this).Layer)
				{
					return false;
				}
			}
			else if (((ILayerElement)this).BelongsToAllLayers || ((ILayerElement)this).BelongsToLayer)
			{
				return false;
			}
			GraphicsPath[] paths = GetPaths(g);
			if (paths == null)
			{
				return false;
			}
			for (int i = 0; i < paths.Length; i++)
			{
				if (paths[i] != null)
				{
					RectangleF rect = cachedPathBounds[i];
					if (clipRect.IntersectsWith(rect))
					{
						return true;
					}
				}
			}
			return false;
		}

		void IContentElement.RenderShadow(MapGraphics g)
		{
		}

		void IContentElement.RenderBack(MapGraphics g, HotRegionList hotRegions)
		{
		}

		void IContentElement.RenderFront(MapGraphics g, HotRegionList hotRegions)
		{
			g.StartHotRegion(this);
			if (!string.IsNullOrEmpty(ImageInt))
			{
				ImageSmoothingState imageSmoothingState = new ImageSmoothingState(g);
				imageSmoothingState.Set();
				DrawImage(g, ImageInt, drawShadow: true);
				DrawImage(g, ImageInt, drawShadow: false);
				imageSmoothingState.Restore();
			}
			try
			{
				GraphicsPath[] paths = GetPaths(g);
				if (MarkerStyleInt != 0)
				{
					for (int i = 0; i < paths.Length - 1; i++)
					{
						if (!string.IsNullOrEmpty(ImageInt))
						{
							continue;
						}
						if (IsXamlMarker(MarkerStyleInt))
						{
							XamlRenderer xamlRenderer = GetXamlRenderers(g)[i];
							if (xamlRenderer != null)
							{
								XamlLayer[] layers = xamlRenderer.Layers;
								for (int j = 0; j < layers.Length; j++)
								{
									layers[j].Render(g);
								}
							}
							continue;
						}
						if (ShadowOffsetInt != 0)
						{
							using (Matrix matrix = new Matrix())
							{
								int num = ShadowOffsetInt;
								matrix.Translate(num, num, MatrixOrder.Append);
								paths[i].Transform(matrix);
								using (Brush brush = g.GetShadowBrush())
								{
									g.FillPath(brush, paths[i]);
								}
								matrix.Reset();
								matrix.Translate(-num, -num, MatrixOrder.Append);
								paths[i].Transform(matrix);
							}
						}
						using (Brush brush2 = GetBackBrush(g, paths[i]))
						{
							g.FillPath(brush2, paths[i]);
						}
						if (BorderWidthInt > 0)
						{
							using (GetPen())
							{
								g.DrawPath(GetPen(), paths[i]);
							}
						}
					}
				}
				hotRegions.SetHotRegion(g, this, paths);
			}
			finally
			{
				g.EndHotRegion();
			}
		}

		void IContentElement.RenderText(MapGraphics g, HotRegionList hotRegions)
		{
			RenderText(g);
		}

		RectangleF IContentElement.GetBoundRect(MapGraphics g)
		{
			return new RectangleF(0f, 0f, 100f, 100f);
		}

		string IToolTipProvider.GetToolTip()
		{
			if (Common != null && Common.MapCore != null)
			{
				return Common.MapCore.ResolveAllKeywords(ToolTipInt, this);
			}
			return ToolTipInt;
		}

		string IImageMapProvider.GetToolTip()
		{
			return ((IToolTipProvider)this).GetToolTip();
		}

		string IImageMapProvider.GetHref()
		{
			if (Common != null && Common.MapCore != null)
			{
				return Common.MapCore.ResolveAllKeywords(Href, this);
			}
			return Href;
		}

		string IImageMapProvider.GetMapAreaAttributes()
		{
			if (Common != null && Common.MapCore != null)
			{
				return Common.MapCore.ResolveAllKeywords(MapAreaAttributes, this);
			}
			return MapAreaAttributes;
		}

		void ISelectable.DrawSelection(MapGraphics g, RectangleF clipRect, bool designTimeSelection)
		{
			MapCore mapCore = GetMapCore();
			GraphicsPath[] paths = GetPaths(g);
			if (paths != null && paths.Length != 0)
			{
				RectangleF selectionRectangle = ((ISelectable)this).GetSelectionRectangle(g, clipRect);
				RectangleF rect = selectionRectangle;
				rect.Inflate(6f, 6f);
				if (clipRect.IntersectsWith(rect) && !selectionRectangle.IsEmpty)
				{
					g.DrawSelection(selectionRectangle, designTimeSelection, mapCore.SelectionBorderColor, mapCore.SelectionMarkerColor);
				}
			}
		}

		RectangleF ISelectable.GetSelectionRectangle(MapGraphics g, RectangleF clipRect)
		{
			GraphicsPath[] paths = GetPaths(g);
			RectangleF rectangleF = cachedPathBounds[0];
			for (int i = 0; i < paths.Length; i++)
			{
				if (paths[i] != null)
				{
					RectangleF b = cachedPathBounds[i];
					rectangleF = RectangleF.Union(rectangleF, b);
				}
			}
			return rectangleF;
		}

		bool ISelectable.IsSelected()
		{
			return Selected;
		}

		bool ISelectable.IsVisible()
		{
			return Visible;
		}
	}
}

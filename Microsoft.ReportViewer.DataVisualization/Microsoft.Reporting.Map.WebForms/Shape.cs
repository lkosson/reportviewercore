using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(ShapeConverter))]
	internal class Shape : NamedElement, IContentElement, ILayerElement, IToolTipProvider, ISelectable, ISpatialElement, IImageMapProvider
	{
		private GraphicsPath[] cachedPaths;

		private RectangleF[] cachedPathBounds;

		private RectangleF cachedTextBounds = Rectangle.Empty;

		internal int largestPathIndex;

		internal Hashtable fields;

		private string fieldDataBuffer = string.Empty;

		private ShapeData shapeData;

		private Offset offset;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private ContentAlignment textAlignment = ContentAlignment.MiddleCenter;

		private bool visible = true;

		private TextVisibility textVisibility = TextVisibility.Auto;

		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private Color borderColor = Color.DarkGray;

		private MapDashStyle borderStyle = MapDashStyle.Solid;

		private int borderWidth = 1;

		private Color color = Color.Empty;

		private Color textColor = Color.Black;

		private GradientType gradientType;

		private Color secondaryColor = Color.Empty;

		private MapHatchStyle hatchStyle;

		private string text = "#NAME";

		private int shadowOffset;

		private int textShadowOffset;

		private bool selected;

		private Offset centralPointOffset;

		private string category = string.Empty;

		private string parentGroup = "(none)";

		private int childSymbolMargin;

		private double scaleFactor = 1.0;

		private ArrayList symbols;

		private Group parentGroupObject;

		private bool useInternalProperties;

		private Color borderColorInt = Color.DarkGray;

		private Color colorInt = Color.Empty;

		private GradientType gradientTypeInt;

		private Color secondaryColorInt = Color.Empty;

		private MapHatchStyle hatchStyleInt;

		private string textInt = "#NAME";

		private string toolTipInt = "";

		private object mapAreaTag;

		private string layer = "(none)";

		private bool belongsToLayer;

		private bool belongsToAllLayers;

		private Layer layerObject;

		MapPoint ISpatialElement.MinimumExtent => ShapeData.MinimumExtent;

		MapPoint ISpatialElement.MaximumExtent => ShapeData.MaximumExtent;

		[SRDescription("DescriptionAttributeShape_ShapeData")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public ShapeData ShapeData
		{
			get
			{
				return shapeData;
			}
			set
			{
				shapeData = value;
				ResetCachedPaths();
				InvalidateCachedBounds();
				InvalidateViewport();
			}
		}

		MapPoint[] ISpatialElement.Points => ShapeData.Points;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string EncodedShapeData
		{
			get
			{
				return ShapeData.ShapeDataToString(ShapeData);
			}
			set
			{
				shapeData = ShapeData.ShapeDataFromString(value);
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
		[SRDescription("DescriptionAttributeShape_Offset")]
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
		[SRDescription("DescriptionAttributeShape_ToolTip")]
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

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeShape_Href")]
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

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeShape_MapAreaAttributes")]
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
		[SRDescription("DescriptionAttributeShape_Name")]
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
		[SRDescription("DescriptionAttributeShape_TextAlignment")]
		[DefaultValue(ContentAlignment.MiddleCenter)]
		public ContentAlignment TextAlignment
		{
			get
			{
				return textAlignment;
			}
			set
			{
				textAlignment = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeShape_Visible")]
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
		[SRDescription("DescriptionAttributeShape_TextVisibility")]
		[DefaultValue(TextVisibility.Auto)]
		public TextVisibility TextVisibility
		{
			get
			{
				return textVisibility;
			}
			set
			{
				textVisibility = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeShape_Font")]
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
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeShape_BorderColor")]
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
		[SRDescription("DescriptionAttributeShape_BorderStyle")]
		[NotifyParentProperty(true)]
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
		[SRDescription("DescriptionAttributeShape_BorderWidth")]
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
					throw new ArgumentException(SR.must_in_range(0.0, 100.0));
				}
				borderWidth = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeShape_Color")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "")]
		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeShape_TextColor")]
		[NotifyParentProperty(true)]
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
		[SRDescription("DescriptionAttributeShape_GradientType")]
		[NotifyParentProperty(true)]
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
		[SRDescription("DescriptionAttributeShape_SecondaryColor")]
		[NotifyParentProperty(true)]
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
		[SRDescription("DescriptionAttributeShape_HatchStyle")]
		[NotifyParentProperty(true)]
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
		[SRDescription("DescriptionAttributeShape_Text")]
		[Localizable(true)]
		[TypeConverter(typeof(KeywordConverter))]
		[DefaultValue("#NAME")]
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
				cachedTextBounds = RectangleF.Empty;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeShape_ShadowOffset")]
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
		[SRDescription("DescriptionAttributeShape_TextShadowOffset")]
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
		[SRDescription("DescriptionAttributeShape_Selected")]
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

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeShape_CentralPoint")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MapPoint CentralPoint
		{
			get
			{
				if (ShapeData.Points != null)
				{
					if (ShapeData.Segments.Length == 0 || ShapeData.Points.Length == 0)
					{
						return new MapPoint(0.0, 0.0);
					}
					return ShapeData.Segments[ShapeData.LargestSegmentIndex].PolygonCentroid;
				}
				return new MapPoint(0.0, 0.0);
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeShape_CentralPointOffset")]
		[TypeConverter(typeof(ShapeOffsetConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Offset CentralPointOffset
		{
			get
			{
				return centralPointOffset;
			}
			set
			{
				centralPointOffset = value;
				centralPointOffset.Parent = this;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeShape_Category")]
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
		[SRDescription("DescriptionAttributeShape_ParentGroup")]
		[TypeConverter(typeof(DesignTimeGroupConverter))]
		[DefaultValue("(none)")]
		public string ParentGroup
		{
			get
			{
				return parentGroup;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					parentGroup = "(none)";
				}
				else
				{
					parentGroup = value;
				}
				parentGroupObject = null;
				InvalidateCachedShapesInGroups();
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeShape_ChildSymbolMargin")]
		[DefaultValue(0)]
		public int ChildSymbolMargin
		{
			get
			{
				return childSymbolMargin;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException(SR.ExceptionValueCannotBeNegative);
				}
				childSymbolMargin = value;
				InvalidateChildSymbols();
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeShape_ScaleFactor")]
		[DefaultValue(1.0)]
		public double ScaleFactor
		{
			get
			{
				return scaleFactor;
			}
			set
			{
				if (value < 0.0 || value > 100.0)
				{
					throw new ArgumentException(SR.must_in_range(0.0, 100.0));
				}
				scaleFactor = value;
				ResetCachedPaths();
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
					Field field = (Field)mapCore.ShapeFields.GetByName(name);
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

		internal ArrayList Symbols
		{
			get
			{
				MapCore mapCore = GetMapCore();
				if (mapCore != null && symbols == null)
				{
					symbols = new ArrayList();
					foreach (Symbol symbol in mapCore.Symbols)
					{
						if (symbol.ParentShape == Name)
						{
							symbols.Add(symbol);
						}
					}
				}
				return symbols;
			}
			set
			{
				symbols = value;
			}
		}

		internal Group ParentGroupObject
		{
			get
			{
				if (parentGroup == "(none)")
				{
					return null;
				}
				if (parentGroupObject == null)
				{
					MapCore mapCore = GetMapCore();
					if (mapCore != null)
					{
						parentGroupObject = (Group)mapCore.Groups.GetByName(parentGroup);
					}
				}
				return parentGroupObject;
			}
			set
			{
				parentGroupObject = value;
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
				if (borderColor == Color.DarkGray)
				{
					if (ParentGroupObject != null)
					{
						return ParentGroupObject.BorderColorInt;
					}
					if (useInternalProperties)
					{
						return borderColorInt;
					}
				}
				return borderColor;
			}
			set
			{
				borderColorInt = value;
				InvalidateViewport();
			}
		}

		internal Color ColorInt
		{
			get
			{
				if (color.IsEmpty)
				{
					if (ParentGroupObject != null)
					{
						return ParentGroupObject.ColorInt;
					}
					if (useInternalProperties)
					{
						return colorInt;
					}
				}
				return color;
			}
			set
			{
				colorInt = value;
				InvalidateViewport();
			}
		}

		internal GradientType GradientTypeInt
		{
			get
			{
				if (gradientType == GradientType.None)
				{
					if (ParentGroupObject != null)
					{
						return ParentGroupObject.GradientTypeInt;
					}
					if (useInternalProperties)
					{
						return gradientTypeInt;
					}
				}
				return gradientType;
			}
			set
			{
				gradientTypeInt = value;
				InvalidateViewport();
			}
		}

		internal Color SecondaryColorInt
		{
			get
			{
				if (secondaryColor.IsEmpty)
				{
					if (ParentGroupObject != null)
					{
						return ParentGroupObject.SecondaryColorInt;
					}
					if (useInternalProperties)
					{
						return secondaryColorInt;
					}
				}
				return secondaryColor;
			}
			set
			{
				secondaryColorInt = value;
				InvalidateViewport();
			}
		}

		internal MapHatchStyle HatchStyleInt
		{
			get
			{
				if (hatchStyle == MapHatchStyle.None)
				{
					if (ParentGroupObject != null)
					{
						return ParentGroupObject.HatchStyleInt;
					}
					if (useInternalProperties)
					{
						return hatchStyleInt;
					}
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
				if (text == "#NAME" && useInternalProperties)
				{
					return textInt;
				}
				return text;
			}
			set
			{
				textInt = value;
				cachedTextBounds = RectangleF.Empty;
				InvalidateViewport();
			}
		}

		internal string ToolTipInt
		{
			get
			{
				if (string.IsNullOrEmpty(toolTip))
				{
					if (ParentGroupObject != null)
					{
						return ((IToolTipProvider)ParentGroupObject).GetToolTip();
					}
					if (useInternalProperties)
					{
						return toolTipInt;
					}
				}
				return toolTip;
			}
			set
			{
				toolTipInt = value;
			}
		}

		internal Offset OffsetInt
		{
			get
			{
				if (ParentGroupObject != null)
				{
					return new Offset(this, ParentGroupObject.Offset.X + offset.X, ParentGroupObject.Offset.Y + offset.Y);
				}
				return offset;
			}
		}

		internal MapDashStyle BorderStyleInt
		{
			get
			{
				if (borderStyle == MapDashStyle.Solid && ParentGroupObject != null)
				{
					return ParentGroupObject.BorderStyle;
				}
				return borderStyle;
			}
		}

		internal int BorderWidthInt
		{
			get
			{
				if (borderWidth == 1 && ParentGroupObject != null)
				{
					return ParentGroupObject.BorderWidth;
				}
				return borderWidth;
			}
		}

		internal int ShadowOffsetInt
		{
			get
			{
				if (shadowOffset == 0 && ParentGroupObject != null)
				{
					return ParentGroupObject.ShadowOffset;
				}
				return shadowOffset;
			}
		}

		internal bool VisibleInt
		{
			get
			{
				if (visible && ParentGroupObject != null)
				{
					return ParentGroupObject.Visible;
				}
				return visible;
			}
		}

		internal string HrefInt
		{
			get
			{
				if (string.IsNullOrEmpty(href) && ParentGroupObject != null)
				{
					return ParentGroupObject.Href;
				}
				return href;
			}
		}

		internal string MapAreaAttributesInt
		{
			get
			{
				if (string.IsNullOrEmpty(mapAreaAttributes) && ParentGroupObject != null)
				{
					return ParentGroupObject.MapAreaAttributes;
				}
				return mapAreaAttributes;
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
		[SRDescription("DescriptionAttributeShape_Layer")]
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
					layerObject = null;
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

		public Shape()
			: this(null)
		{
		}

		internal Shape(CommonElements common)
			: base(common)
		{
			shapeData = new ShapeData();
			offset = new Offset(this, 0.0, 0.0);
			centralPointOffset = new Offset(this, 0.0, 0.0);
			fields = new Hashtable();
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected bool ShouldSerializeCentralPointOffset()
		{
			if (CentralPointOffset.X == 0.0)
			{
				return CentralPointOffset.Y != 0.0;
			}
			return true;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected void ResetCentralPointOffset()
		{
			CentralPointOffset.X = 0.0;
			CentralPointOffset.Y = 0.0;
		}

		public override string ToString()
		{
			return Name;
		}

		public void StartNewFigure()
		{
			if (ShapeData.Segments != null && ShapeData.Segments.Length != 0)
			{
				ShapeSegment[] array = ShapeData.Segments;
				Array.Resize(ref array, ShapeData.Segments.Length + 1);
				ShapeData.Segments = array;
				ShapeData.Segments[ShapeData.Segments.Length - 1].Type = SegmentType.StartFigure;
			}
		}

		public void AddSegments(MapPoint[] points, ShapeSegment[] segments)
		{
			if (ShapeData.Points == null || ShapeData.Points.Length == 0)
			{
				ShapeData.Points = points;
			}
			else
			{
				MapPoint[] array = new MapPoint[ShapeData.Points.Length + points.Length];
				Array.Copy(ShapeData.Points, array, ShapeData.Points.Length);
				Array.Copy(points, 0, array, ShapeData.Points.Length, points.Length);
				ShapeData.Points = array;
			}
			if (ShapeData.Segments == null || ShapeData.Segments.Length == 0)
			{
				ShapeData.Segments = segments;
			}
			else
			{
				ShapeSegment[] array2 = new ShapeSegment[ShapeData.Segments.Length + segments.Length];
				Array.Copy(ShapeData.Segments, array2, ShapeData.Segments.Length);
				Array.Copy(segments, 0, array2, ShapeData.Segments.Length, segments.Length);
				ShapeData.Segments = array2;
			}
			ShapeData.UpdateStoredParameters();
			InvalidateCachedBounds();
			InvalidateCachedShapesInGroups();
			InvalidateRules();
			InvalidateViewport();
		}

		public void ClearShapeData()
		{
			ShapeData.Segments = null;
			ShapeData.Points = null;
			ShapeData.UpdateStoredParameters();
			InvalidateCachedBounds();
			InvalidateCachedShapesInGroups();
			InvalidateRules();
			InvalidateViewport();
		}

		public PointF GetCenterPointInContentPixels(MapGraphics g)
		{
			MapCore mapCore = GetMapCore();
			if (mapCore == null)
			{
				return PointF.Empty;
			}
			MapPoint centralPoint = CentralPoint;
			centralPoint.X += OffsetInt.X + CentralPointOffset.X;
			centralPoint.Y += OffsetInt.Y + CentralPointOffset.Y;
			PointF relative = mapCore.GeographicToPercents(centralPoint).ToPointF();
			return g.GetAbsolutePoint(relative);
		}

		public bool IsPointInShape(MapPoint mapPoint)
		{
			bool result = false;
			PointF point = new PointF((float)mapPoint.X, (float)mapPoint.Y);
			GraphicsPath[] geographicGraphicsPaths = GetGeographicGraphicsPaths();
			foreach (GraphicsPath graphicsPath in geographicGraphicsPaths)
			{
				if (graphicsPath.IsVisible(point) || graphicsPath.IsOutlineVisible(point, Pens.Black))
				{
					result = true;
				}
				graphicsPath.Dispose();
			}
			return result;
		}

		internal MapCore GetMapCore()
		{
			return (MapCore)ParentElement;
		}

		private void InvalidateCachedShapesInGroups()
		{
			MapCore mapCore = GetMapCore();
			if (mapCore == null)
			{
				return;
			}
			foreach (Group group in mapCore.Groups)
			{
				group.Shapes = null;
			}
		}

		internal void InvalidateChildSymbols()
		{
			GetMapCore()?.InvalidateChildSymbols();
		}

		private string FieldDataToString()
		{
			string text = string.Empty;
			MapCore mapCore = GetMapCore();
			if (mapCore != null)
			{
				foreach (Field shapeField in mapCore.ShapeFields)
				{
					if (!shapeField.IsTemporary)
					{
						string text2 = shapeField.FormatValue(fields[shapeField.Name]);
						if (!string.IsNullOrEmpty(text2))
						{
							text = text + XmlConvert.EncodeName(shapeField.Name) + "=" + text2 + "&";
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
					((Field)mapCore.ShapeFields.GetByName(name))?.ParseValue(fieldValue, fields);
				}
				fieldDataBuffer = "";
			}
		}

		internal override void OnAdded()
		{
			base.OnAdded();
			InvalidateRules();
		}

		internal override void OnRemove()
		{
			base.OnRemove();
			MapCore mapCore = GetMapCore();
			if (mapCore != null)
			{
				foreach (Symbol symbol in mapCore.Symbols)
				{
					if (symbol.ParentShape == Name)
					{
						symbol.ParentShape = "";
					}
				}
				mapCore.InvalidateRules();
				mapCore.InvalidateDataBinding();
			}
			InvalidateCachedShapesInGroups();
		}

		protected override void OnDispose()
		{
			ResetCachedPaths();
			ShapeData.Points = null;
			ShapeData.Segments = null;
			if (fields != null)
			{
				fields.Clear();
			}
			if (symbols != null)
			{
				symbols.Clear();
			}
			base.OnDispose();
		}

		internal void ApplyCustomColorAttributes(CustomColor customColor)
		{
			UseInternalProperties = true;
			BorderColorInt = customColor.BorderColor;
			ColorInt = customColor.Color;
			SecondaryColorInt = customColor.SecondaryColor;
			GradientTypeInt = customColor.GradientType;
			HatchStyleInt = customColor.HatchStyle;
			TextInt = customColor.Text;
			ToolTipInt = customColor.ToolTip;
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

		internal IEnumerable<RectangleF> GetGeographicRectangles()
		{
			for (int i = 0; i < ShapeData.Segments.Length; i++)
			{
				double verticalOffsetFactor = GetVerticalOffsetFactor();
				ShapeSegment shapeSegment = ShapeData.Segments[i];
				if (shapeSegment.Length > 0)
				{
					MapPoint mapPoint = OffsetAndScaleGeoPoint(shapeSegment.MinimumExtent, verticalOffsetFactor);
					MapPoint mapPoint2 = OffsetAndScaleGeoPoint(shapeSegment.MaximumExtent, verticalOffsetFactor);
					yield return new RectangleF((float)mapPoint.X, (float)mapPoint.Y, (float)(mapPoint2.X - mapPoint.X), (float)(mapPoint2.Y - mapPoint.Y));
				}
			}
		}

		public string SaveWKT()
		{
			if (ShapeData == null || ShapeData.Segments == null || ShapeData.Segments.Length == 0 || ShapeData.Points == null || ShapeData.Points.Length == 0)
			{
				return string.Empty;
			}
			MapPoint[] points = ShapeData.Points;
			ShapeSegment[] segments = ShapeData.Segments;
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("MULTIPOLYGON(");
			int num2 = 0;
			for (int i = 0; i < segments.Length; i++)
			{
				if (segments[i].Type == SegmentType.StartFigure)
				{
					num2++;
					continue;
				}
				if (i > num2)
				{
					stringBuilder.Append(", ");
				}
				if (segments[i].PolygonSignedArea <= 0.0)
				{
					stringBuilder.Append("(");
				}
				stringBuilder.Append("(");
				for (int num3 = num + segments[i].Length - 1; num3 >= num; num3--)
				{
					stringBuilder.Append(points[num3].X.ToString(CultureInfo.InvariantCulture) + " " + points[num3].Y.ToString(CultureInfo.InvariantCulture));
					if (num3 > num)
					{
						stringBuilder.Append(", ");
					}
				}
				stringBuilder.Append(")");
				if (i == segments.Length - 1 || segments[i + 1].PolygonSignedArea <= 0.0)
				{
					stringBuilder.Append(")");
				}
				num += segments[i].Length;
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public byte[] SaveWKB()
		{
			if (ShapeData == null)
			{
				return null;
			}
			MemoryStream memoryStream = new MemoryStream();
			SaveWKBToStream(memoryStream);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return memoryStream.ToArray();
		}

		private void SaveWKBToStream(Stream stream)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			byte value = (byte)(BitConverter.IsLittleEndian ? 1 : 0);
			binaryWriter.Write(value);
			binaryWriter.Write(6u);
			if (ShapeData == null || ShapeData.Segments == null || ShapeData.Segments.Length == 0 || ShapeData.Points == null || ShapeData.Points.Length == 0)
			{
				binaryWriter.Write(0u);
				return;
			}
			MapPoint[] points = ShapeData.Points;
			ShapeSegment[] segments = ShapeData.Segments;
			List<int> list = new List<int>();
			for (int i = 0; i < segments.Length; i++)
			{
				if (segments[i].Type != SegmentType.StartFigure)
				{
					if (segments[i].PolygonSignedArea <= 0.0)
					{
						list.Add(1);
					}
					else if (list.Count > 0)
					{
						list[list.Count - 1]++;
					}
				}
			}
			binaryWriter.Write((uint)list.Count);
			int num = 0;
			int num2 = 0;
			for (int j = 0; j < segments.Length; j++)
			{
				if (segments[j].Type != SegmentType.StartFigure)
				{
					if (segments[j].PolygonSignedArea <= 0.0)
					{
						binaryWriter.Write(value);
						binaryWriter.Write(3u);
						binaryWriter.Write((uint)list[num2]);
						num2++;
					}
					binaryWriter.Write((uint)segments[j].Length);
					for (int num3 = num + segments[j].Length - 1; num3 >= num; num3--)
					{
						binaryWriter.Write(points[num3].X);
						binaryWriter.Write(points[num3].Y);
					}
					num += segments[j].Length;
				}
			}
		}

		private void RenderText(MapGraphics g)
		{
			if (string.IsNullOrEmpty(TextInt) || cachedPaths.Length == 0 || TextVisibility == TextVisibility.Hidden)
			{
				return;
			}
			string text = (TextInt.IndexOf("#", StringComparison.Ordinal) == -1) ? TextInt : GetMapCore().ResolveAllKeywords(TextInt, this);
			text = text.Replace("\\n", "\n");
			StringFormat stringFormat = new StringFormat();
			if (TextAlignment == ContentAlignment.BottomRight)
			{
				stringFormat.Alignment = StringAlignment.Near;
				stringFormat.LineAlignment = StringAlignment.Near;
			}
			else if (TextAlignment == ContentAlignment.BottomCenter)
			{
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Near;
			}
			else if (TextAlignment == ContentAlignment.BottomLeft)
			{
				stringFormat.Alignment = StringAlignment.Far;
				stringFormat.LineAlignment = StringAlignment.Near;
			}
			else if (TextAlignment == ContentAlignment.MiddleRight)
			{
				stringFormat.Alignment = StringAlignment.Near;
				stringFormat.LineAlignment = StringAlignment.Center;
			}
			else if (TextAlignment == ContentAlignment.MiddleCenter)
			{
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;
			}
			else if (TextAlignment == ContentAlignment.MiddleLeft)
			{
				stringFormat.Alignment = StringAlignment.Far;
				stringFormat.LineAlignment = StringAlignment.Center;
			}
			else if (TextAlignment == ContentAlignment.TopRight)
			{
				stringFormat.Alignment = StringAlignment.Near;
				stringFormat.LineAlignment = StringAlignment.Far;
			}
			else if (TextAlignment == ContentAlignment.TopCenter)
			{
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Far;
			}
			else
			{
				stringFormat.Alignment = StringAlignment.Far;
				stringFormat.LineAlignment = StringAlignment.Far;
			}
			SizeF sizeF = g.MeasureString(text, Font, new SizeF(0f, 0f), StringFormat.GenericTypographic);
			if (TextVisibility == TextVisibility.Auto && sizeF.Width > cachedPathBounds[largestPathIndex].Width)
			{
				return;
			}
			PointF centerPointInContentPixels = GetCenterPointInContentPixels(g);
			new RectangleF(centerPointInContentPixels.X, centerPointInContentPixels.Y - 1f, 0f, 0f).Inflate(sizeF.Width / 2f, sizeF.Height / 2f);
			if (TextShadowOffset != 0)
			{
				using (Brush brush = g.GetShadowBrush())
				{
					g.DrawString(point: new PointF(centerPointInContentPixels.X + (float)TextShadowOffset, centerPointInContentPixels.Y + (float)TextShadowOffset), s: text, font: Font, brush: brush, format: stringFormat);
				}
			}
			using (Brush brush2 = new SolidBrush(TextColor))
			{
				g.DrawString(text, Font, brush2, centerPointInContentPixels, stringFormat);
			}
		}

		internal bool IsRectangleVisible(MapGraphics g, RectangleF clipRect, MapPoint minExtent, MapPoint maxExtent)
		{
			MapCore mapCore = GetMapCore();
			PointF pointF = mapCore.GeographicToPercents(minExtent).ToPointF();
			PointF pointF2 = mapCore.GeographicToPercents(maxExtent).ToPointF();
			RectangleF relative = new RectangleF(pointF.X, pointF2.Y, pointF2.X - pointF.X, pointF.Y - pointF2.Y);
			relative = g.GetAbsoluteRectangle(relative);
			return clipRect.IntersectsWith(relative);
		}

		internal RectangleF GetTextBounds(MapGraphics g)
		{
			if (TextInt == string.Empty)
			{
				return RectangleF.Empty;
			}
			if (!cachedTextBounds.IsEmpty)
			{
				return cachedTextBounds;
			}
			string text = (TextInt.IndexOf("#", StringComparison.Ordinal) == -1) ? TextInt : GetMapCore().ResolveAllKeywords(TextInt, this);
			text = text.Replace("\\n", "\n");
			SizeF sizeF = g.MeasureString(text, Font, new SizeF(0f, 0f), StringFormat.GenericTypographic);
			PointF centerPointInContentPixels = GetCenterPointInContentPixels(g);
			cachedTextBounds = new RectangleF(centerPointInContentPixels.X, centerPointInContentPixels.Y, sizeF.Width, sizeF.Height);
			cachedTextBounds.Inflate(sizeF.Width / 2f, sizeF.Height / 2f);
			return cachedTextBounds;
		}

		internal GraphicsPath[] GetPaths(MapGraphics g)
		{
			if (!VisibleInt || ShapeData.Points == null)
			{
				return null;
			}
			if (cachedPaths != null)
			{
				return cachedPaths;
			}
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			int num = 0;
			int index = 0;
			largestPathIndex = 0;
			double geographicResolutionAtEquator = Common.MapCore.Viewport.GetGeographicResolutionAtEquator();
			double num2 = geographicResolutionAtEquator * geographicResolutionAtEquator;
			double verticalOffsetFactor = GetVerticalOffsetFactor();
			for (int i = 0; i < ShapeData.Segments.Length; i++)
			{
				ShapeSegment shapeSegment = ShapeData.Segments[i];
				List<Point3D> list = new List<Point3D>();
				MapPoint pointB = default(MapPoint);
				for (int j = 0; j < shapeSegment.Length; j++)
				{
					MapPoint mapPoint = ShapeData.Points[num];
					mapPoint = OffsetAndScaleGeoPoint(mapPoint, verticalOffsetFactor);
					if (j == 0 || j == shapeSegment.Length - 1 || Utils.GetDistanceSqr(mapPoint, pointB) > num2)
					{
						pointB = mapPoint;
						Point3D point3D = GetMapCore().GeographicToPercents(mapPoint);
						PointF absolutePoint = g.GetAbsolutePoint(point3D.ToPointF());
						Point3D item = new Point3D(absolutePoint.X, absolutePoint.Y, point3D.Z);
						list.Add(item);
					}
					num++;
				}
				PointF[] array = ReducePoints(list.ToArray());
				if (array.Length <= 2)
				{
					continue;
				}
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.StartFigure();
				graphicsPath.AddPolygon(array);
				graphicsPath.CloseFigure();
				graphicsPath.SetMarkers();
				if (ShapeData.MultiPolygonWithHoles || ShapeData.Segments[0].PolygonSignedArea > 0.0)
				{
					if (arrayList.Count == 0 || ShapeData.Segments[i - 1].Type == SegmentType.StartFigure)
					{
						arrayList.Add(graphicsPath);
						arrayList2.Add(graphicsPath.GetBounds());
						index = arrayList.Count - 1;
						if (ShapeData.LargestSegmentIndex == i)
						{
							largestPathIndex = arrayList.Count - 1;
						}
					}
					else
					{
						((GraphicsPath)arrayList[index]).AddPath(graphicsPath, connect: false);
						arrayList2[index] = RectangleF.Union((RectangleF)arrayList2[index], graphicsPath.GetBounds());
					}
				}
				else if (ShapeData.Segments[i].PolygonSignedArea > 0.0 && arrayList.Count > 0)
				{
					((GraphicsPath)arrayList[index]).AddPath(graphicsPath, connect: false);
					arrayList2[index] = RectangleF.Union((RectangleF)arrayList2[index], graphicsPath.GetBounds());
				}
				else
				{
					arrayList.Add(graphicsPath);
					arrayList2.Add(graphicsPath.GetBounds());
					index = arrayList.Count - 1;
					if (ShapeData.LargestSegmentIndex == i)
					{
						largestPathIndex = arrayList.Count - 1;
					}
				}
			}
			cachedPaths = (GraphicsPath[])arrayList.ToArray(typeof(GraphicsPath));
			cachedPathBounds = (RectangleF[])arrayList2.ToArray(typeof(RectangleF));
			return cachedPaths;
		}

		private MapPoint OffsetAndScaleGeoPoint(MapPoint mapPoint, double verticalOffsetFactor)
		{
			if (OffsetInt.Y != 0.0)
			{
				double num = mapPoint.X - CentralPoint.X;
				mapPoint.X = CentralPoint.X + num * verticalOffsetFactor;
			}
			if (ScaleFactor != 1.0)
			{
				double num2 = mapPoint.X - CentralPoint.X;
				double num3 = mapPoint.Y - CentralPoint.Y;
				mapPoint.X = CentralPoint.X + num2 * ScaleFactor;
				mapPoint.Y = CentralPoint.Y + num3 * ScaleFactor;
			}
			mapPoint.X += OffsetInt.X;
			mapPoint.Y += OffsetInt.Y;
			return mapPoint;
		}

		internal GraphicsPath[] GetGeographicGraphicsPaths()
		{
			ArrayList arrayList = new ArrayList();
			int num = 0;
			for (int i = 0; i < ShapeData.Segments.Length; i++)
			{
				PointF[] array = new PointF[ShapeData.Segments[i].Length];
				for (int j = 0; j < array.Length; j++)
				{
					_ = ref ShapeData.Points[num];
					array[j].X = (float)ShapeData.Points[num].X;
					array[j].Y = (float)ShapeData.Points[num].Y;
					num++;
				}
				if (array.Length > 2)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					graphicsPath.StartFigure();
					graphicsPath.AddPolygon(array);
					graphicsPath.CloseFigure();
					graphicsPath.SetMarkers();
					arrayList.Add(graphicsPath);
				}
			}
			return (GraphicsPath[])arrayList.ToArray(typeof(GraphicsPath));
		}

		internal double GetVerticalOffsetFactor()
		{
			MapCore mapCore = GetMapCore();
			if (OffsetInt.Y == 0.0 || mapCore == null || mapCore.Projection == Projection.Equirectangular || mapCore.Projection == Projection.Mercator)
			{
				return 1.0;
			}
			double y = CentralPoint.Y;
			double y2 = y + OffsetInt.Y;
			double num = mapCore.MeasureDistance(new MapPoint(0.0, y), new MapPoint(1.0, y));
			double num2 = mapCore.MeasureDistance(new MapPoint(0.0, y2), new MapPoint(1.0, y2));
			if (num2 > double.Epsilon)
			{
				return num / num2;
			}
			return 0.0;
		}

		internal void ArrangeChildSymbols(MapGraphics g)
		{
			PointF centerPointInContentPixels = GetCenterPointInContentPixels(g);
			float num = 0f;
			foreach (Symbol symbol3 in Symbols)
			{
				num += symbol3.GetWidth() + (float)ChildSymbolMargin;
			}
			if (num > 0f)
			{
				num -= (float)ChildSymbolMargin;
			}
			float num2 = 0f;
			foreach (Symbol symbol4 in Symbols)
			{
				symbol4.precalculatedCenterPoint.X = centerPointInContentPixels.X + num2 - num / 2f + symbol4.GetWidth() / 2f;
				num2 += symbol4.Width + (float)ChildSymbolMargin;
				symbol4.precalculatedCenterPoint.Y = centerPointInContentPixels.Y;
				symbol4.ResetCachedPaths();
			}
		}

		internal void InvalidateCachedBounds()
		{
			GetMapCore()?.InvalidateCachedBounds();
		}

		internal void ResetCachedPaths()
		{
			if (cachedPaths != null)
			{
				GraphicsPath[] array = cachedPaths;
				for (int i = 0; i < array.Length; i++)
				{
					array[i]?.Dispose();
				}
				cachedPaths = null;
			}
			if (cachedPathBounds != null)
			{
				cachedPathBounds = null;
			}
			cachedTextBounds = RectangleF.Empty;
		}

		private PointF[] ReducePoints(Point3D[] points)
		{
			ArrayList arrayList = new ArrayList();
			bool flag = false;
			for (int i = 0; i < points.Length; i++)
			{
				if (!(points[i].Z < 0.0))
				{
					if (!flag)
					{
						arrayList.Add(points[i].ToPointF());
						flag = true;
					}
					else if (Utils.GetDistanceSqr((PointF)arrayList[arrayList.Count - 1], points[i].ToPointF()) > 1f)
					{
						arrayList.Add(points[i].ToPointF());
					}
				}
			}
			return (PointF[])arrayList.ToArray(typeof(PointF));
		}

		internal Brush GetBackBrush(MapGraphics g, GraphicsPath path, RectangleF pathBounds)
		{
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
				return g.GetGradientBrush(pathBounds, color, color2, gradientType);
			}
			return new SolidBrush(color);
		}

		internal Pen GetPen(Brush backBrush)
		{
			Pen pen = null;
			if (BorderWidthInt <= 0)
			{
				pen = new Pen(backBrush);
				pen.Width = 1f;
			}
			else
			{
				pen = new Pen(ApplyLayerTransparency(BorderColorInt), BorderWidthInt);
				pen.DashStyle = MapGraphics.GetPenStyle(BorderStyleInt);
			}
			pen.Alignment = PenAlignment.Center;
			pen.LineJoin = LineJoin.Round;
			return pen;
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
			if (ParentGroupObject != null)
			{
				return ((IContentElement)ParentGroupObject).IsVisible(g, layer, allLayers, clipRect);
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
			if (ShapeData.Points == null || ShapeData.Segments.Length == 0)
			{
				return false;
			}
			if (Selected)
			{
				return true;
			}
			RectangleF geographicClipRectangle = Common.MapCore.GetGeographicClipRectangle(clipRect);
			bool flag = false;
			foreach (RectangleF geographicRectangle in GetGeographicRectangles())
			{
				if (geographicClipRectangle.IntersectsWith(geographicRectangle))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
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
				RectangleF rect = cachedPathBounds[i];
				if (clipRect.IntersectsWith(rect))
				{
					return true;
				}
			}
			if (TextInt != string.Empty && clipRect.IntersectsWith(GetTextBounds(g)))
			{
				return true;
			}
			return false;
		}

		void IContentElement.RenderShadow(MapGraphics g)
		{
			if (!VisibleInt || ShadowOffsetInt == 0)
			{
				return;
			}
			GraphicsPath[] paths = GetPaths(g);
			for (int i = 0; i < paths.Length; i++)
			{
				if (paths[i] != null)
				{
					using (Brush brush = g.GetShadowBrush())
					{
						Matrix matrix = new Matrix();
						int shadowOffsetInt = ShadowOffsetInt;
						matrix.Translate(shadowOffsetInt, shadowOffsetInt, MatrixOrder.Append);
						paths[i].Transform(matrix);
						g.FillPath(brush, paths[i]);
						matrix.Reset();
						matrix.Translate(-shadowOffsetInt, -shadowOffsetInt, MatrixOrder.Append);
						paths[i].Transform(matrix);
					}
				}
			}
		}

		void IContentElement.RenderBack(MapGraphics g, HotRegionList hotRegions)
		{
			if (!VisibleInt)
			{
				return;
			}
			g.StartHotRegion(this);
			try
			{
				GraphicsPath[] paths = GetPaths(g);
				for (int i = 0; i < paths.Length; i++)
				{
					if (paths[i] == null)
					{
						continue;
					}
					using (Brush brush = GetBackBrush(g, paths[i], cachedPathBounds[i]))
					{
						g.FillPath(brush, paths[i]);
						using (Pen pen = GetPen(brush))
						{
							if (pen != null)
							{
								g.DrawPath(pen, paths[i]);
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

		void IContentElement.RenderFront(MapGraphics g, HotRegionList hotRegions)
		{
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
				return Common.MapCore.ResolveAllKeywords(HrefInt, this);
			}
			return HrefInt;
		}

		string IImageMapProvider.GetMapAreaAttributes()
		{
			if (Common != null && Common.MapCore != null)
			{
				return Common.MapCore.ResolveAllKeywords(MapAreaAttributesInt, this);
			}
			return MapAreaAttributesInt;
		}

		void ISelectable.DrawSelection(MapGraphics g, RectangleF clipRect, bool designTimeSelection)
		{
			MapCore mapCore = GetMapCore();
			RectangleF selectionRectangle = ((ISelectable)this).GetSelectionRectangle(g, clipRect);
			RectangleF rect = selectionRectangle;
			rect.Inflate(6f, 6f);
			if (clipRect.IntersectsWith(rect) && !selectionRectangle.IsEmpty)
			{
				g.DrawSelection(selectionRectangle, designTimeSelection, mapCore.SelectionBorderColor, mapCore.SelectionMarkerColor);
				if (mapCore.IsDesignMode())
				{
					PointF centerPointInContentPixels = GetCenterPointInContentPixels(g);
					AntiAliasing antiAliasing = g.AntiAliasing;
					g.AntiAliasing = AntiAliasing.None;
					g.DrawLine(Pens.Red, centerPointInContentPixels.X - 8f, centerPointInContentPixels.Y, centerPointInContentPixels.X + 8f, centerPointInContentPixels.Y);
					g.DrawLine(Pens.Red, centerPointInContentPixels.X, centerPointInContentPixels.Y - 8f, centerPointInContentPixels.X, centerPointInContentPixels.Y + 8f);
					g.AntiAliasing = antiAliasing;
				}
			}
		}

		RectangleF ISelectable.GetSelectionRectangle(MapGraphics g, RectangleF clipRect)
		{
			RectangleF rectangleF = RectangleF.Empty;
			GraphicsPath[] paths = GetPaths(g);
			if (paths == null || paths.Length == 0)
			{
				return RectangleF.Empty;
			}
			RectangleF rectangleF2 = cachedPathBounds[largestPathIndex];
			_ = rectangleF2.Width;
			_ = rectangleF2.Height;
			for (int i = 0; i < paths.Length; i++)
			{
				RectangleF rectangleF3 = cachedPathBounds[i];
				RectangleF rect = rectangleF3;
				rect.Inflate(6f, 6f);
				if (clipRect.IntersectsWith(rect))
				{
					rectangleF = ((!rectangleF.IsEmpty) ? RectangleF.Union(rectangleF, rectangleF3) : rectangleF3);
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
			return VisibleInt;
		}
	}
}

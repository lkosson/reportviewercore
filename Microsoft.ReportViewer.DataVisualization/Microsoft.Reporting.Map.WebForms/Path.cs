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
	[TypeConverter(typeof(PathConverter))]
	internal class Path : NamedElement, IContentElement, ILayerElement, IToolTipProvider, ISelectable, ISpatialElement, IImageMapProvider
	{
		private GraphicsPath[] cachedPaths;

		private RectangleF[] cachedPathBounds;

		private RectangleF cachedUnionRectangle = RectangleF.Empty;

		private GraphicsPath[] cachedLabelPaths;

		private double[] cachedSegmentLengths;

		internal int largestPathIndex;

		internal Hashtable fields;

		private string fieldDataBuffer = string.Empty;

		private PathData pathData;

		private Offset offset;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private bool visible = true;

		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private Color borderColor = Color.DarkGray;

		private MapDashStyle lineStyle = MapDashStyle.Solid;

		private int borderWidth = 1;

		private float width = 5f;

		private Color color = Color.LightSalmon;

		private Color textColor = Color.Black;

		private GradientType gradientType;

		private Color secondaryColor = Color.Empty;

		private MapHatchStyle hatchStyle;

		private string text = "#NAME";

		private int shadowOffset;

		private int textShadowOffset;

		private bool selected;

		private string category = string.Empty;

		private string parentGroup = "(none)";

		private PathLabelPosition labelPosition;

		private Group parentGroupObject;

		private bool useInternalProperties;

		private Color borderColorInt = Color.DarkGray;

		private Color colorInt = Color.LightSalmon;

		private GradientType gradientTypeInt;

		private Color secondaryColorInt = Color.Empty;

		private MapHatchStyle hatchStyleInt;

		private float widthInt = 5f;

		private string textInt = "#NAME";

		private string toolTipInt = "";

		private object mapAreaTag;

		private string layer = "(none)";

		private bool belongsToLayer;

		private bool belongsToAllLayers;

		private Layer layerObject;

		MapPoint ISpatialElement.MinimumExtent => PathData.MinimumExtent;

		MapPoint ISpatialElement.MaximumExtent => PathData.MaximumExtent;

		[SRDescription("DescriptionAttributePath_PathData")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public PathData PathData
		{
			get
			{
				return pathData;
			}
			set
			{
				pathData = value;
				ResetCachedPaths();
				InvalidateCachedBounds();
				InvalidateViewport();
			}
		}

		MapPoint[] ISpatialElement.Points => PathData.Points;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string EncodedPathData
		{
			get
			{
				return PathData.PathDataToString(PathData);
			}
			set
			{
				pathData = PathData.PathDataFromString(value);
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
		[SRDescription("DescriptionAttributePath_Offset")]
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
		[SRDescription("DescriptionAttributePath_ToolTip")]
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
		[SRDescription("DescriptionAttributePath_Href")]
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
		[SRDescription("DescriptionAttributePath_MapAreaAttributes")]
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
		[SRDescription("DescriptionAttributePath_Name")]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePath_Visible")]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePath_Font")]
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
		[SRDescription("DescriptionAttributePath_BorderColor")]
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
		[SRDescription("DescriptionAttributePath_LineStyle")]
		[NotifyParentProperty(true)]
		[DefaultValue(MapDashStyle.Solid)]
		public MapDashStyle LineStyle
		{
			get
			{
				return lineStyle;
			}
			set
			{
				lineStyle = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePath_BorderWidth")]
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
		[SRDescription("DescriptionAttributePath_Width")]
		[NotifyParentProperty(true)]
		[DefaultValue(5f)]
		public float Width
		{
			get
			{
				return width;
			}
			set
			{
				if (value < 0f || value > 100f)
				{
					throw new ArgumentException(SR.must_in_range(0.0, 100.0));
				}
				width = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePath_Color")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "LightSalmon")]
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
		[SRDescription("DescriptionAttributePath_TextColor")]
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
		[SRDescription("DescriptionAttributePath_GradientType")]
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
		[SRDescription("DescriptionAttributePath_SecondaryColor")]
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
		[SRDescription("DescriptionAttributePath_HatchStyle")]
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
		[SRDescription("DescriptionAttributePath_Text")]
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
				ResetCachedPaths();
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePath_ShadowOffset")]
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
		[SRDescription("DescriptionAttributePath_TextShadowOffset")]
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
		[SRDescription("DescriptionAttributePath_Selected")]
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

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributePath_Category")]
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
		[SRDescription("DescriptionAttributePath_ParentGroup")]
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
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePath_LabelPosition")]
		[NotifyParentProperty(true)]
		[DefaultValue(PathLabelPosition.Above)]
		public PathLabelPosition LabelPosition
		{
			get
			{
				return labelPosition;
			}
			set
			{
				labelPosition = value;
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
					Field field = (Field)mapCore.PathFields.GetByName(name);
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
				if (color == Color.LightSalmon)
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

		internal float WidthInt
		{
			get
			{
				if (width == 5f && useInternalProperties)
				{
					return widthInt;
				}
				return width;
			}
			set
			{
				widthInt = value;
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
		[SRDescription("DescriptionAttributePath_Layer")]
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

		public Path()
			: this(null)
		{
		}

		internal Path(CommonElements common)
			: base(common)
		{
			pathData = new PathData();
			offset = new Offset(this, 0.0, 0.0);
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

		public override string ToString()
		{
			return Name;
		}

		public void AddSegments(MapPoint[] points, PathSegment[] segments)
		{
			if (PathData.Points == null || PathData.Points.Length == 0)
			{
				PathData.Points = points;
			}
			else
			{
				MapPoint[] array = new MapPoint[PathData.Points.Length + points.Length];
				Array.Copy(PathData.Points, array, PathData.Points.Length);
				Array.Copy(points, 0, array, PathData.Points.Length, points.Length);
				PathData.Points = array;
			}
			if (PathData.Segments == null || PathData.Segments.Length == 0)
			{
				PathData.Segments = segments;
			}
			else
			{
				PathSegment[] array2 = new PathSegment[PathData.Segments.Length + segments.Length];
				Array.Copy(PathData.Segments, array2, PathData.Segments.Length);
				Array.Copy(segments, 0, array2, PathData.Segments.Length, segments.Length);
				PathData.Segments = array2;
			}
			PathData.UpdateStoredParameters();
			InvalidateCachedBounds();
			InvalidateRules();
			InvalidateViewport();
		}

		public void ClearPathData()
		{
			PathData.Segments = null;
			PathData.Points = null;
			PathData.UpdateStoredParameters();
			InvalidateCachedBounds();
			InvalidateRules();
			InvalidateViewport();
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
				foreach (Field pathField in mapCore.PathFields)
				{
					if (!pathField.IsTemporary)
					{
						string text2 = pathField.FormatValue(fields[pathField.Name]);
						if (!string.IsNullOrEmpty(text2))
						{
							text = text + XmlConvert.EncodeName(pathField.Name) + "=" + text2 + "&";
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
					((Field)mapCore.PathFields.GetByName(name))?.ParseValue(fieldValue, fields);
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
				mapCore.InvalidateRules();
				mapCore.InvalidateDataBinding();
			}
		}

		protected override void OnDispose()
		{
			ResetCachedPaths();
			PathData.Points = null;
			PathData.Segments = null;
			if (fields != null)
			{
				fields.Clear();
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

		internal void ApplyCustomWidthAttributes(CustomWidth customWidth)
		{
			UseInternalProperties = true;
			WidthInt = customWidth.Width;
			TextInt = customWidth.Text;
			ToolTipInt = customWidth.ToolTip;
		}

		internal int GetLongestVisibleSegmentIndex(MapGraphics g)
		{
			int num = -1;
			double num2 = double.NegativeInfinity;
			MapCore mapCore = GetMapCore();
			PointF location = mapCore.PixelsToContent(mapCore.Viewport.GetAbsoluteLocation());
			SizeF sizeInPixels = mapCore.Viewport.GetSizeInPixels();
			RectangleF rectangleF = new RectangleF(location, sizeInPixels);
			GraphicsPath[] paths = GetPaths(g);
			for (int i = 0; i < paths.Length; i++)
			{
				if (rectangleF.Contains(cachedPathBounds[i]) && cachedSegmentLengths[i] > num2)
				{
					num2 = cachedSegmentLengths[i];
					num = i;
				}
			}
			if (num == -1)
			{
				num2 = double.NegativeInfinity;
				for (int j = 0; j < paths.Length; j++)
				{
					if (rectangleF.IntersectsWith(cachedPathBounds[j]) && cachedSegmentLengths[j] > num2)
					{
						num2 = cachedSegmentLengths[j];
						num = j;
					}
				}
			}
			return num;
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

		public string SaveWKT()
		{
			if (PathData == null || PathData.Segments == null || PathData.Segments.Length == 0 || PathData.Points == null || PathData.Points.Length == 0)
			{
				return string.Empty;
			}
			MapPoint[] points = PathData.Points;
			PathSegment[] segments = PathData.Segments;
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("MULTILINESTRING(");
			for (int i = 0; i < segments.Length; i++)
			{
				stringBuilder.Append("(");
				for (int j = num; j < num + segments[i].Length; j++)
				{
					stringBuilder.Append(points[j].X.ToString(CultureInfo.InvariantCulture) + " " + points[j].Y.ToString(CultureInfo.InvariantCulture));
					if (j < num + segments[i].Length - 1)
					{
						stringBuilder.Append(", ");
					}
				}
				stringBuilder.Append(")");
				if (i < segments.Length - 1)
				{
					stringBuilder.Append(", ");
				}
				num += segments[i].Length;
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
			binaryWriter.Write(value);
			binaryWriter.Write(5u);
			if (PathData == null || PathData.Segments == null || PathData.Segments.Length == 0 || PathData.Points == null || PathData.Points.Length == 0)
			{
				binaryWriter.Write(0u);
				return;
			}
			binaryWriter.Write((uint)PathData.Segments.Length);
			MapPoint[] points = PathData.Points;
			PathSegment[] segments = PathData.Segments;
			int num = 0;
			for (int i = 0; i < segments.Length; i++)
			{
				binaryWriter.Write(value);
				binaryWriter.Write(2u);
				binaryWriter.Write((uint)segments[i].Length);
				for (int j = num; j < num + segments[i].Length; j++)
				{
					binaryWriter.Write(points[j].X);
					binaryWriter.Write(points[j].Y);
				}
				num += segments[i].Length;
			}
		}

		private void RenderText(MapGraphics g)
		{
			if (string.IsNullOrEmpty(TextInt) || cachedPaths.Length == 0)
			{
				return;
			}
			string text = (TextInt.IndexOf("#", StringComparison.Ordinal) == -1) ? TextInt : GetMapCore().ResolveAllKeywords(TextInt, this);
			int labelOffset = 0;
			if (LabelPosition == PathLabelPosition.Above)
			{
				labelOffset = (int)Math.Round(WidthInt * 2f + 10f);
			}
			else if (LabelPosition == PathLabelPosition.Below)
			{
				labelOffset = -(int)Math.Round(WidthInt * 2f + 10f);
			}
			text = text.Replace("\\n", "\n");
			text = "   " + text;
			using (Brush brush2 = new SolidBrush(TextColor))
			{
				int longestVisibleSegmentIndex = GetLongestVisibleSegmentIndex(g);
				if (longestVisibleSegmentIndex == -1)
				{
					return;
				}
				GraphicsPath graphicsPath = cachedLabelPaths[longestVisibleSegmentIndex];
				if (graphicsPath == null)
				{
					PointF[] pathPoints = cachedPaths[longestVisibleSegmentIndex].PathPoints;
					BendingText bendingText = new BendingText();
					if (pathPoints.Length > 1)
					{
						graphicsPath = bendingText.CreatePath(Font, pathPoints, text, 0, labelOffset);
						cachedLabelPaths[longestVisibleSegmentIndex] = graphicsPath;
					}
				}
				if (graphicsPath == null)
				{
					return;
				}
				if (TextShadowOffset != 0)
				{
					using (Brush brush = g.GetShadowBrush())
					{
						Matrix matrix = new Matrix();
						matrix.Translate(TextShadowOffset, TextShadowOffset, MatrixOrder.Append);
						graphicsPath.Transform(matrix);
						g.FillPath(brush, graphicsPath);
						matrix.Reset();
						matrix.Translate(-TextShadowOffset, -TextShadowOffset, MatrixOrder.Append);
						graphicsPath.Transform(matrix);
					}
				}
				g.FillPath(brush2, graphicsPath);
			}
		}

		internal bool IsRectangleVisible(MapGraphics g, RectangleF clipRect, MapPoint minExtent, MapPoint maxExtent)
		{
			MapCore mapCore = GetMapCore();
			Point3D point3D = mapCore.GeographicToPercents(minExtent);
			Point3D point3D2 = mapCore.GeographicToPercents(maxExtent);
			RectangleF relative = new RectangleF((float)point3D.X, (float)point3D2.Y, (float)(point3D2.X - point3D.X), (float)(point3D.Y - point3D2.Y));
			relative = g.GetAbsoluteRectangle(relative);
			return clipRect.IntersectsWith(relative);
		}

		internal GraphicsPath[] GetPaths(MapGraphics g)
		{
			if (!Visible || PathData.Points == null)
			{
				return null;
			}
			if (cachedPaths != null)
			{
				return cachedPaths;
			}
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			ArrayList arrayList3 = new ArrayList();
			int num = 0;
			largestPathIndex = 0;
			double geographicResolutionAtEquator = Common.MapCore.Viewport.GetGeographicResolutionAtEquator();
			double num2 = geographicResolutionAtEquator * geographicResolutionAtEquator;
			for (int i = 0; i < PathData.Segments.Length; i++)
			{
				PathSegment pathSegment = PathData.Segments[i];
				List<Point3D> list = new List<Point3D>();
				MapPoint pointB = default(MapPoint);
				for (int j = 0; j < pathSegment.Length; j++)
				{
					MapPoint mapPoint = PathData.Points[num];
					if (j == 0 || j == pathSegment.Length - 1 || Utils.GetDistanceSqr(mapPoint, pointB) > num2)
					{
						pointB = mapPoint;
						mapPoint.X += OffsetInt.X;
						mapPoint.Y += OffsetInt.Y;
						Point3D point3D = GetMapCore().GeographicToPercents(mapPoint);
						PointF absolutePoint = g.GetAbsolutePoint(point3D.ToPointF());
						Point3D item = new Point3D(absolutePoint.X, absolutePoint.Y, point3D.Z);
						list.Add(item);
					}
					num++;
				}
				PointF[] array = ReducePoints(list.ToArray());
				if (array.Length > 1)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					graphicsPath.AddLines(array);
					arrayList.Add(graphicsPath);
					RectangleF bounds = graphicsPath.GetBounds();
					arrayList2.Add(bounds);
					arrayList3.Add(PathData.Segments[i].SegmentLength);
					if (cachedUnionRectangle.IsEmpty)
					{
						cachedUnionRectangle = bounds;
					}
					else
					{
						cachedUnionRectangle = RectangleF.Union(cachedUnionRectangle, bounds);
					}
				}
			}
			cachedUnionRectangle.Inflate(WidthInt / 2f, WidthInt / 2f);
			cachedPaths = (GraphicsPath[])arrayList.ToArray(typeof(GraphicsPath));
			cachedPathBounds = (RectangleF[])arrayList2.ToArray(typeof(RectangleF));
			cachedLabelPaths = new GraphicsPath[cachedPaths.Length];
			cachedSegmentLengths = (double[])arrayList3.ToArray(typeof(double));
			return cachedPaths;
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
			cachedUnionRectangle = RectangleF.Empty;
			if (cachedLabelPaths != null)
			{
				GraphicsPath[] array = cachedLabelPaths;
				for (int i = 0; i < array.Length; i++)
				{
					array[i]?.Dispose();
				}
				cachedLabelPaths = null;
			}
			if (cachedSegmentLengths != null)
			{
				cachedSegmentLengths = null;
			}
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
					else if (GetDistance((PointF)arrayList[arrayList.Count - 1], points[i].ToPointF()) > 1f)
					{
						arrayList.Add(points[i].ToPointF());
					}
				}
			}
			return (PointF[])arrayList.ToArray(typeof(PointF));
		}

		private float GetDistance(PointF pointA, PointF pointB)
		{
			double num = Math.Abs(pointA.X - pointB.X);
			double num2 = Math.Abs(pointA.Y - pointB.Y);
			return (float)Math.Sqrt(num * num + num2 * num2);
		}

		internal static Brush GetBackBrush(MapGraphics g, GraphicsPath path, RectangleF pathBounds, Color fillColor, Color secondaryColor, GradientType gradientType, MapHatchStyle hatchStyle)
		{
			Brush brush = null;
			if (hatchStyle != 0)
			{
				return MapGraphics.GetHatchBrush(hatchStyle, fillColor, secondaryColor);
			}
			if (gradientType != 0)
			{
				return g.GetGradientBrush(pathBounds, fillColor, secondaryColor, gradientType);
			}
			return new SolidBrush(fillColor);
		}

		internal Pen GetBorderPen()
		{
			Pen pen = new Pen(ApplyLayerTransparency(BorderColorInt), WidthInt + (float)(BorderWidthInt * 2));
			pen.Alignment = PenAlignment.Center;
			pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
			pen.LineJoin = LineJoin.Round;
			return pen;
		}

		internal static Pen GetColorPen(Color color, float width, float borderWidth)
		{
			if (width + borderWidth * 2f == 0f)
			{
				return null;
			}
			Pen pen = new Pen(color, width + borderWidth * 2f);
			pen.Alignment = PenAlignment.Center;
			pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
			pen.LineJoin = LineJoin.Round;
			return pen;
		}

		internal static Pen GetFillPen(MapGraphics g, GraphicsPath path, RectangleF pathBounds, float width, MapDashStyle lineStyle, Color fillColor, Color secondaryColor, GradientType gradientType, MapHatchStyle hatchStyle)
		{
			if (width == 0f)
			{
				return null;
			}
			Pen pen = new Pen(GetBackBrush(g, path, pathBounds, fillColor, secondaryColor, gradientType, hatchStyle));
			pen.Width = width;
			pen.DashStyle = MapGraphics.GetPenStyle(lineStyle);
			pen.Alignment = PenAlignment.Center;
			pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
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
			if (!Visible)
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
			if (PathData.Points == null || PathData.Segments.Length == 0)
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
				rect.Inflate(WidthInt / 2f + (float)BorderWidthInt, WidthInt / 2f + (float)BorderWidthInt);
				if (clipRect.IntersectsWith(rect))
				{
					return true;
				}
			}
			return false;
		}

		void IContentElement.RenderShadow(MapGraphics g)
		{
			if (!Visible || ShadowOffset == 0 || (WidthInt == 0f && BorderWidthInt == 0))
			{
				return;
			}
			GraphicsPath[] paths = GetPaths(g);
			foreach (GraphicsPath graphicsPath in paths)
			{
				if (graphicsPath == null)
				{
					continue;
				}
				using (Pen pen = GetColorPen(g.GetShadowColor(), WidthInt, BorderWidthInt))
				{
					if (pen != null)
					{
						Matrix matrix = new Matrix();
						int shadowOffsetInt = ShadowOffsetInt;
						matrix.Translate(shadowOffsetInt, shadowOffsetInt, MatrixOrder.Append);
						graphicsPath.Transform(matrix);
						g.DrawPath(pen, graphicsPath);
						matrix.Reset();
						matrix.Translate(-shadowOffsetInt, -shadowOffsetInt, MatrixOrder.Append);
						graphicsPath.Transform(matrix);
					}
				}
			}
		}

		void IContentElement.RenderBack(MapGraphics g, HotRegionList hotRegions)
		{
			if (!Visible || BorderColorInt == Color.Empty || BorderWidthInt < 1 || WidthInt == 0f)
			{
				return;
			}
			g.StartHotRegion(this);
			Brush brush = null;
			Brush brush2 = null;
			Pen pen = null;
			try
			{
				GraphicsPath[] paths = GetPaths(g);
				foreach (GraphicsPath graphicsPath in paths)
				{
					if (graphicsPath != null)
					{
						pen = GetBorderPen();
						if (pen != null && BorderWidthInt != 0 && BorderColorInt.A != 0)
						{
							g.DrawPath(pen, graphicsPath);
						}
					}
				}
			}
			finally
			{
				brush2?.Dispose();
				brush?.Dispose();
				pen?.Dispose();
				g.EndHotRegion();
			}
		}

		void IContentElement.RenderFront(MapGraphics g, HotRegionList hotRegions)
		{
			if (!Visible || WidthInt == 0f)
			{
				return;
			}
			g.StartHotRegion(this);
			try
			{
				GraphicsPath[] paths = GetPaths(g);
				GraphicsPath[] array = paths;
				foreach (GraphicsPath graphicsPath in array)
				{
					if (graphicsPath == null)
					{
						continue;
					}
					using (Pen pen = GetFillPen(g, graphicsPath, cachedUnionRectangle, WidthInt, LineStyle, ApplyLayerTransparency(ColorInt), ApplyLayerTransparency(SecondaryColorInt), GradientTypeInt, HatchStyleInt))
					{
						if (pen != null)
						{
							g.DrawPath(pen, graphicsPath);
							if (pen.Brush != null)
							{
								pen.Brush.Dispose();
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
			RectangleF selectionRectangle = ((ISelectable)this).GetSelectionRectangle(g, clipRect);
			RectangleF rect = selectionRectangle;
			rect.Inflate(6f, 6f);
			if (clipRect.IntersectsWith(rect) && !selectionRectangle.IsEmpty)
			{
				g.DrawSelection(selectionRectangle, designTimeSelection, mapCore.SelectionBorderColor, mapCore.SelectionMarkerColor);
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
			double num = rectangleF2.Width * rectangleF2.Height;
			for (int i = 0; i < paths.Length; i++)
			{
				RectangleF rectangleF3 = cachedPathBounds[i];
				if (!((double)(rectangleF3.Width * rectangleF3.Height) < num / 20.0))
				{
					RectangleF rect = rectangleF3;
					rect.Inflate(6f, 6f);
					if (clipRect.IntersectsWith(rect))
					{
						rectangleF = ((!rectangleF.IsEmpty) ? RectangleF.Union(rectangleF, rectangleF3) : rectangleF3);
					}
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

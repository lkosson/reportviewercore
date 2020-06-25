using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Xml;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(GroupConverter))]
	internal class Group : NamedElement, IContentElement, ILayerElement, ISelectable, IToolTipProvider
	{
		internal Hashtable fields;

		private string fieldDataBuffer = string.Empty;

		private Offset offset;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private ContentAlignment textAlignment = ContentAlignment.MiddleCenter;

		private bool visible = true;

		private Font font = new Font("Microsoft Sans Serif", 10f);

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

		private ArrayList shapes;

		private bool useInternalProperties;

		private Color borderColorInt = Color.DarkGray;

		private Color colorInt = Color.Empty;

		private GradientType gradientTypeInt;

		private Color secondaryColorInt = Color.Empty;

		private MapHatchStyle hatchStyleInt;

		private string textInt = "#NAME";

		private string toolTipInt = "";

		private string layer = "(none)";

		private bool belongsToLayer;

		private bool belongsToAllLayers;

		private Layer layerObject;

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
		[SRDescription("DescriptionAttributeGroup_Offset")]
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
		[SRDescription("DescriptionAttributeGroup_ToolTip")]
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
		[SRDescription("DescriptionAttributeGroup_Href")]
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
		[SRDescription("DescriptionAttributeGroup_MapAreaAttributes")]
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
		[SRDescription("DescriptionAttributeGroup_Name")]
		public override string Name
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
		[SRDescription("DescriptionAttributeGroup_TextAlignment")]
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
		[SRDescription("DescriptionAttributeGroup_Visible")]
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
		[SRDescription("DescriptionAttributeGroup_Font")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 10pt")]
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
		[SRDescription("DescriptionAttributeGroup_BorderColor")]
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
		[SRDescription("DescriptionAttributeGroup_BorderStyle")]
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
		[SRDescription("DescriptionAttributeGroup_BorderWidth")]
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
		[SRDescription("DescriptionAttributeGroup_Color")]
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
		[SRDescription("DescriptionAttributeGroup_TextColor")]
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
		[SRDescription("DescriptionAttributeGroup_GradientType")]
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
		[SRDescription("DescriptionAttributeGroup_SecondaryColor")]
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
		[SRDescription("DescriptionAttributeGroup_HatchStyle")]
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
		[SRDescription("DescriptionAttributeGroup_Text")]
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
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeGroup_ShadowOffset")]
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
		[SRDescription("DescriptionAttributeGroup_TextShadowOffset")]
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
		[SRDescription("DescriptionAttributeGroup_Selected")]
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
		[SRDescription("DescriptionAttributeGroup_CentralPoint")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MapPoint CentralPoint
		{
			get
			{
				MapPoint result = new MapPoint(0.0, 0.0);
				if (Shapes.Count > 0)
				{
					foreach (Shape shape in Shapes)
					{
						MapPoint centralPoint = shape.CentralPoint;
						centralPoint.X += shape.OffsetInt.X + shape.CentralPointOffset.X;
						centralPoint.Y += shape.OffsetInt.Y + shape.CentralPointOffset.Y;
						result.X += centralPoint.X;
						result.Y += centralPoint.Y;
					}
					result.X /= Shapes.Count;
					result.Y /= Shapes.Count;
				}
				return result;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeGroup_CentralPointOffset")]
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
		[SRDescription("DescriptionAttributeGroup_Category")]
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
					Field field = (Field)mapCore.GroupFields.GetByName(name);
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

		internal ArrayList Shapes
		{
			get
			{
				MapCore mapCore = GetMapCore();
				if (mapCore != null && shapes == null)
				{
					shapes = new ArrayList();
					foreach (Shape shape in mapCore.Shapes)
					{
						if (shape.ParentGroup == Name)
						{
							shapes.Add(shape);
						}
					}
				}
				return shapes;
			}
			set
			{
				shapes = value;
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

		internal Color ColorInt
		{
			get
			{
				if (color.IsEmpty && useInternalProperties)
				{
					return colorInt;
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

		internal Color SecondaryColorInt
		{
			get
			{
				if (secondaryColor.IsEmpty && useInternalProperties)
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

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeGroup_Layer")]
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

		public Group()
			: this(null)
		{
		}

		internal Group(CommonElements common)
			: base(common)
		{
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

		public PointF GetCenterPointInContentPixels(MapGraphics g)
		{
			MapCore mapCore = GetMapCore();
			if (mapCore == null)
			{
				return PointF.Empty;
			}
			MapPoint centralPoint = CentralPoint;
			centralPoint.X += CentralPointOffset.X;
			centralPoint.Y += CentralPointOffset.Y;
			PointF relative = mapCore.GeographicToPercents(centralPoint).ToPointF();
			return g.GetAbsolutePoint(relative);
		}

		internal MapCore GetMapCore()
		{
			return (MapCore)ParentElement;
		}

		internal PointF GetCentralPoint()
		{
			int count = Shapes.Count;
			if (count == 0)
			{
				return PointF.Empty;
			}
			MapPoint mapPoint = new MapPoint(0.0, 0.0);
			foreach (Shape shape in Shapes)
			{
				MapPoint centralPoint = shape.CentralPoint;
				centralPoint.X += shape.OffsetInt.X + shape.CentralPointOffset.X;
				centralPoint.Y += shape.OffsetInt.Y + shape.CentralPointOffset.Y;
				mapPoint.X += centralPoint.X;
				mapPoint.Y += centralPoint.Y;
			}
			mapPoint.X /= count;
			mapPoint.Y /= count;
			return GetMapCore().GeographicToPercents(mapPoint).ToPointF();
		}

		private string FieldDataToString()
		{
			string text = string.Empty;
			MapCore mapCore = GetMapCore();
			if (mapCore != null)
			{
				foreach (Field groupField in mapCore.GroupFields)
				{
					if (!groupField.IsTemporary)
					{
						string text2 = groupField.FormatValue(fields[groupField.Name]);
						if (!string.IsNullOrEmpty(text2))
						{
							text = text + XmlConvert.EncodeName(groupField.Name) + "=" + text2 + "&";
						}
					}
				}
				text = text.TrimEnd('&');
			}
			return text;
		}

		internal void FieldDataFromBuffer()
		{
			if (string.IsNullOrEmpty(fieldDataBuffer))
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
					((Field)mapCore.GroupFields.GetByName(name))?.ParseValue(fieldValue, fields);
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
			if (mapCore == null)
			{
				return;
			}
			foreach (Shape shape in mapCore.Shapes)
			{
				if (shape.ParentGroup == Name)
				{
					shape.ParentGroup = "";
				}
			}
			mapCore.InvalidateCachedPaths();
			mapCore.InvalidateRules();
			mapCore.InvalidateDataBinding();
			mapCore.InvalidateCachedBounds();
			mapCore.InvalidateGridSections();
		}

		protected override void OnDispose()
		{
			ResetCachedPaths();
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

		internal void InvalidateRules()
		{
			MapCore mapCore = GetMapCore();
			if (mapCore != null)
			{
				mapCore.InvalidateRules();
				mapCore.Invalidate();
			}
		}

		private void RenderText(MapGraphics g)
		{
			if (TextInt == string.Empty)
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

		internal GraphicsPath GetPath(MapGraphics g, bool outlineOnly)
		{
			if (Shapes.Count == 0)
			{
				return null;
			}
			if (outlineOnly)
			{
				ArrayList arrayList = new ArrayList();
				foreach (Shape shape in Shapes)
				{
					GraphicsPath[] paths = shape.GetPaths(g);
					arrayList.Add(paths[shape.largestPathIndex]);
				}
				if (arrayList.Count > 0 && !GetMapCore().IsDesignMode())
				{
					return new GraphicsPathOutliner(g.Graphics).GetOutlinePath((GraphicsPath[])arrayList.ToArray(typeof(GraphicsPath)));
				}
				return null;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			foreach (Shape shape2 in Shapes)
			{
				GraphicsPath[] paths2 = shape2.GetPaths(g);
				foreach (GraphicsPath addingPath in paths2)
				{
					graphicsPath.AddPath(addingPath, connect: false);
				}
			}
			return graphicsPath;
		}

		internal void InvalidateCachedBounds()
		{
			GetMapCore()?.InvalidateCachedBounds();
		}

		internal void ResetCachedPaths()
		{
			foreach (Shape shape in Shapes)
			{
				shape.ResetCachedPaths();
			}
		}

		private float GetDistance(PointF pointA, PointF pointB)
		{
			double num = Math.Abs(pointA.X - pointB.X);
			double num2 = Math.Abs(pointA.Y - pointB.Y);
			return (float)Math.Sqrt(num * num + num2 * num2);
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
			if (BorderWidth <= 0)
			{
				return null;
			}
			return new Pen(ApplyLayerTransparency(BorderColorInt), BorderWidth)
			{
				DashStyle = MapGraphics.GetPenStyle(BorderStyle),
				Alignment = PenAlignment.Center,
				LineJoin = LineJoin.Round
			};
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
			return true;
		}

		void IContentElement.RenderShadow(MapGraphics g)
		{
		}

		void IContentElement.RenderBack(MapGraphics g, HotRegionList hotRegions)
		{
		}

		void IContentElement.RenderFront(MapGraphics g, HotRegionList hotRegions)
		{
		}

		void IContentElement.RenderText(MapGraphics g, HotRegionList hotRegions)
		{
			if (Shapes.Count > 0)
			{
				RenderText(g);
			}
		}

		RectangleF IContentElement.GetBoundRect(MapGraphics g)
		{
			return new RectangleF(0f, 0f, 100f, 100f);
		}

		void ISelectable.DrawSelection(MapGraphics g, RectangleF clipRect, bool designTimeSelection)
		{
			if (Shapes.Count == 0)
			{
				return;
			}
			MapCore mapCore = GetMapCore();
			using (GraphicsPath graphicsPath = GetPath(g, outlineOnly: false))
			{
				if (graphicsPath != null)
				{
					RectangleF bounds = graphicsPath.GetBounds();
					RectangleF rect = bounds;
					rect.Inflate(6f, 6f);
					if (clipRect.IntersectsWith(rect) && !bounds.IsEmpty)
					{
						g.DrawSelection(bounds, designTimeSelection, mapCore.SelectionBorderColor, mapCore.SelectionMarkerColor);
						PointF centerPointInContentPixels = GetCenterPointInContentPixels(g);
						AntiAliasing antiAliasing = g.AntiAliasing;
						g.AntiAliasing = AntiAliasing.None;
						g.DrawLine(Pens.Red, centerPointInContentPixels.X - 8f, centerPointInContentPixels.Y, centerPointInContentPixels.X + 8f, centerPointInContentPixels.Y);
						g.DrawLine(Pens.Red, centerPointInContentPixels.X, centerPointInContentPixels.Y - 8f, centerPointInContentPixels.X, centerPointInContentPixels.Y + 8f);
						g.AntiAliasing = antiAliasing;
					}
				}
			}
		}

		RectangleF ISelectable.GetSelectionRectangle(MapGraphics g, RectangleF clipRect)
		{
			return RectangleF.Empty;
		}

		bool ISelectable.IsVisible()
		{
			return Visible;
		}

		bool ISelectable.IsSelected()
		{
			return Selected;
		}

		string IToolTipProvider.GetToolTip()
		{
			if (Common != null && Common.MapCore != null)
			{
				return Common.MapCore.ResolveAllKeywords(ToolTipInt, this);
			}
			return ToolTipInt;
		}
	}
}

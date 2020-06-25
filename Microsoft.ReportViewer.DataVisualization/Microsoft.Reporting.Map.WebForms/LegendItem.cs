using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace Microsoft.Reporting.Map.WebForms
{
	[Description("Map legend item.")]
	[DefaultProperty("Name")]
	internal class LegendItem : NamedElement
	{
		private Color color = Color.Empty;

		private string image = "";

		private string seriesName = "";

		private int seriesPointIndex = -1;

		private string toolTip = "";

		private string href = "";

		private string attributes = "";

		internal LegendItemStyle itemStyle;

		internal Color borderColor = Color.DarkGray;

		internal int borderWidth = 1;

		internal int pathWidth = 3;

		internal MapDashStyle pathLineStyle = MapDashStyle.Solid;

		internal MapDashStyle borderStyle = MapDashStyle.Solid;

		internal int shadowOffset;

		internal Color shadowColor = Color.FromArgb(128, 0, 0, 0);

		internal MarkerStyle markerStyle = MarkerStyle.Circle;

		internal string markerImage = "";

		internal Color markerImageTranspColor = Color.Empty;

		internal Color markerColor = Color.Empty;

		internal Color markerBorderColor = Color.Empty;

		[Bindable(false)]
		[Browsable(false)]
		public Legend Legend;

		private int markerBorderWidth = 1;

		private LegendCellCollection cells;

		private LegendSeparatorType separator;

		private Color separatorColor = Color.Black;

		internal bool clearTempCells;

		private string text = "";

		private MapHatchStyle hatchStyle;

		internal Color imageTranspColor = Color.Empty;

		private GradientType gradientType;

		private Color secondaryColor = Color.Empty;

		private MapImageAlign imageAlign;

		private MapImageWrapMode imageWrapMode;

		private bool visible = true;

		private float markerWidth = 7f;

		private float markerHeight = 7f;

		private MapDashStyle markerBorderStyle = MapDashStyle.Solid;

		private GradientType markerGradientType;

		private Color markerSecondaryColor = Color.Empty;

		private MapHatchStyle markerHatchStyle;

		internal bool automaticallyAdded;

		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegendItem_Text")]
		[NotifyParentProperty(true)]
		public string Text
		{
			get
			{
				if (string.IsNullOrEmpty(text))
				{
					return base.Name;
				}
				return text;
			}
			set
			{
				text = value;
			}
		}

		[Browsable(true)]
		[SRDescription("DescriptionAttributeLegendItem_Name")]
		[SRCategory("CategoryAttribute_Data")]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[SRCategory("CategoryAttribute_Background")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegendItem_Color")]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Background")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegendItem_Image")]
		[DefaultValue("")]
		[NotifyParentProperty(true)]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[DefaultValue(typeof(LegendItemStyle), "Shape")]
		[SRDescription("DescriptionAttributeLegendItem_ItemStyle")]
		[NotifyParentProperty(true)]
		public LegendItemStyle ItemStyle
		{
			get
			{
				return itemStyle;
			}
			set
			{
				itemStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "DarkGray")]
		[SRDescription("DescriptionAttributeLegendItem_BorderColor")]
		[NotifyParentProperty(true)]
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

		[SRCategory("CategoryAttribute_Background")]
		[Bindable(true)]
		[DefaultValue(MapHatchStyle.None)]
		[SRDescription("DescriptionAttributeLegendItem_HatchStyle")]
		[NotifyParentProperty(true)]
		public MapHatchStyle HatchStyle
		{
			get
			{
				return hatchStyle;
			}
			set
			{
				hatchStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Background")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLegendItem_ImageTranspColor")]
		public Color ImageTranspColor
		{
			get
			{
				return imageTranspColor;
			}
			set
			{
				imageTranspColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Background")]
		[Bindable(true)]
		[DefaultValue(GradientType.None)]
		[SRDescription("DescriptionAttributeLegendItem_GradientType")]
		[NotifyParentProperty(true)]
		public GradientType GradientType
		{
			get
			{
				return gradientType;
			}
			set
			{
				gradientType = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Background")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendItem_SecondaryColor")]
		[NotifyParentProperty(true)]
		public Color SecondaryColor
		{
			get
			{
				return secondaryColor;
			}
			set
			{
				if (value != Color.Empty && (value.A != byte.MaxValue || value == Color.Transparent))
				{
					throw new ArgumentException(SR.ExceptionCollorCannotBeTransparent);
				}
				secondaryColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Background")]
		[Bindable(true)]
		[DefaultValue(MapImageAlign.TopLeft)]
		[SRDescription("DescriptionAttributeLegendItem_ImageAlign")]
		[NotifyParentProperty(true)]
		public MapImageAlign ImageAlign
		{
			get
			{
				return imageAlign;
			}
			set
			{
				imageAlign = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Background")]
		[Bindable(true)]
		[DefaultValue(MapImageWrapMode.Tile)]
		[SRDescription("DescriptionAttributeLegendItem_ImageWrapMode")]
		[NotifyParentProperty(true)]
		public MapImageWrapMode ImageWrapMode
		{
			get
			{
				return imageWrapMode;
			}
			set
			{
				imageWrapMode = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeLegendItem_BorderWidth")]
		[NotifyParentProperty(true)]
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
					throw new ArgumentOutOfRangeException("BorderWidth", SR.ExceptionBorderWidthMustBeGreaterThanZero);
				}
				borderWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[DefaultValue(3)]
		[SRDescription("DescriptionAttributeLegendItem_PathWidth")]
		[NotifyParentProperty(true)]
		public int PathWidth
		{
			get
			{
				return pathWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("PathWidth", SR.ExceptionPathWidthMustBeGreaterThanZero);
				}
				pathWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[DefaultValue(MapDashStyle.Solid)]
		[SRDescription("DescriptionAttributeLegendItem_PathLineStyle")]
		[NotifyParentProperty(true)]
		public MapDashStyle PathLineStyle
		{
			get
			{
				return pathLineStyle;
			}
			set
			{
				pathLineStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeLegendItem_Visible")]
		[NotifyParentProperty(true)]
		[ParenthesizePropertyName(true)]
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

		[SRCategory("CategoryAttribute_Marker")]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeLegendItem_MarkerBorderWidth")]
		public int MarkerBorderWidth
		{
			get
			{
				return markerBorderWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("MarkerBorderWidth", SR.ExceptionMarkerBorderMustBeGreaterThanZero);
				}
				markerBorderWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[DefaultValue(MapDashStyle.Solid)]
		[SRDescription("DescriptionAttributeLegendItem_BorderStyle")]
		[NotifyParentProperty(true)]
		public MapDashStyle BorderStyle
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

		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegendItem_ShadowOffset")]
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
				shadowOffset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "128,0,0,0")]
		[SRDescription("DescriptionAttributeLegendItem_ShadowColor")]
		[NotifyParentProperty(true)]
		public Color ShadowColor
		{
			get
			{
				return shadowColor;
			}
			set
			{
				shadowColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Marker")]
		[Bindable(true)]
		[DefaultValue(MarkerStyle.Circle)]
		[SRDescription("DescriptionAttributeLegendItem_MarkerStyle")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public MarkerStyle MarkerStyle
		{
			get
			{
				return markerStyle;
			}
			set
			{
				markerStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Marker")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerWidth")]
		[DefaultValue(7f)]
		public float MarkerWidth
		{
			get
			{
				return markerWidth;
			}
			set
			{
				if (value < 0f || value > 1000f)
				{
					throw new ArgumentException(SR.must_in_range(0.0, 1000.0));
				}
				markerWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Marker")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerHeight")]
		[DefaultValue(7f)]
		public float MarkerHeight
		{
			get
			{
				return markerHeight;
			}
			set
			{
				if (value < 0f || value > 1000f)
				{
					throw new ArgumentException(SR.must_in_range(0.0, 1000.0));
				}
				markerHeight = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Marker")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerImage")]
		[RefreshProperties(RefreshProperties.All)]
		public string MarkerImage
		{
			get
			{
				return markerImage;
			}
			set
			{
				markerImage = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Marker")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerImageTranspColor")]
		[RefreshProperties(RefreshProperties.All)]
		public Color MarkerImageTranspColor
		{
			get
			{
				return markerImageTranspColor;
			}
			set
			{
				markerImageTranspColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Marker")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerColor")]
		[RefreshProperties(RefreshProperties.All)]
		public Color MarkerColor
		{
			get
			{
				return markerColor;
			}
			set
			{
				markerColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Marker")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerBorderColor")]
		[RefreshProperties(RefreshProperties.All)]
		public Color MarkerBorderColor
		{
			get
			{
				return markerBorderColor;
			}
			set
			{
				markerBorderColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Marker")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerBorderStyle")]
		[DefaultValue(MapDashStyle.Solid)]
		public MapDashStyle MarkerBorderStyle
		{
			get
			{
				return markerBorderStyle;
			}
			set
			{
				markerBorderStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Marker")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerGradientType")]
		[DefaultValue(GradientType.None)]
		public GradientType MarkerGradientType
		{
			get
			{
				return markerGradientType;
			}
			set
			{
				markerGradientType = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Marker")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerSecondaryColor")]
		[DefaultValue(typeof(Color), "")]
		public Color MarkerSecondaryColor
		{
			get
			{
				return markerSecondaryColor;
			}
			set
			{
				markerSecondaryColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Marker")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerHatchStyle")]
		[DefaultValue(MapHatchStyle.None)]
		public MapHatchStyle MarkerHatchStyle
		{
			get
			{
				return markerHatchStyle;
			}
			set
			{
				markerHatchStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(typeof(LegendSeparatorType), "None")]
		[SRDescription("DescriptionAttributeLegendItem_Separator")]
		public LegendSeparatorType Separator
		{
			get
			{
				return separator;
			}
			set
			{
				if (value != separator)
				{
					separator = value;
					Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLegendItem_SeparatorColor")]
		public Color SeparatorColor
		{
			get
			{
				return separatorColor;
			}
			set
			{
				if (value != separatorColor)
				{
					separatorColor = value;
					Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLegendItem_Cells")]
		public LegendCellCollection Cells => cells;

		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeLegendItem_AutomaticallyAdded")]
		[DefaultValue(false)]
		public bool AutomaticallyAdded
		{
			get
			{
				return automaticallyAdded;
			}
			set
			{
				automaticallyAdded = value;
			}
		}

		[Browsable(false)]
		[SRDescription("DescriptionAttributeLegendItem_SeriesName")]
		[DefaultValue("")]
		public string SeriesName
		{
			get
			{
				return seriesName;
			}
			set
			{
				seriesName = value;
			}
		}

		[Browsable(false)]
		[SRDescription("DescriptionAttributeLegendItem_SeriesPointIndex")]
		[DefaultValue(-1)]
		public int SeriesPointIndex
		{
			get
			{
				return seriesPointIndex;
			}
			set
			{
				seriesPointIndex = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegendItem_ToolTip")]
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

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_MapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegendItem_Href")]
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
		[SRCategory("CategoryAttribute_MapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegendItem_MapAreaAttributes")]
		[DefaultValue("")]
		public string MapAreaAttributes
		{
			get
			{
				return attributes;
			}
			set
			{
				attributes = value;
			}
		}

		internal override CommonElements Common
		{
			get
			{
				return base.Common;
			}
			set
			{
				base.Common = value;
				if (Cells != null)
				{
					Cells.Common = value;
				}
			}
		}

		public LegendItem()
			: base(null)
		{
			cells = new LegendCellCollection(this, this, Common);
		}

		public LegendItem(string name, Color color, string image)
			: this()
		{
			Name = name;
			this.color = color;
			this.image = image;
		}

		protected bool ShouldSerializeText()
		{
			return !string.IsNullOrEmpty(text);
		}

		internal void AddAutomaticCells(Legend legend, SizeF singleWCharacterSize)
		{
			if (Cells.Count != 0)
			{
				return;
			}
			clearTempCells = true;
			int num = Cells.Add(LegendCellType.Symbol, string.Empty, ContentAlignment.MiddleCenter);
			LegendCell legendCell = Cells[num];
			if (ItemStyle == LegendItemStyle.Symbol)
			{
				if (Symbol.IsXamlMarker(MarkerStyle))
				{
					RectangleF rectangleF = Symbol.CalculateXamlMarkerBounds(MarkerStyle, PointF.Empty, MarkerWidth, MarkerHeight);
					Size empty = Size.Empty;
					empty.Width = (int)Math.Round(100f * rectangleF.Width / singleWCharacterSize.Width);
					empty.Height = (int)Math.Round(100f * rectangleF.Height / singleWCharacterSize.Height);
					legendCell.SymbolSize = empty;
				}
				else
				{
					int width = (int)Math.Round(100f * MarkerWidth / singleWCharacterSize.Width);
					int height = (int)Math.Round(100f * MarkerHeight / singleWCharacterSize.Height);
					legendCell.SymbolSize = new Size(width, height);
				}
			}
			else if (ItemStyle == LegendItemStyle.Path)
			{
				float num2 = 100f * (float)PathWidth / singleWCharacterSize.Height;
				float num3 = 100f * (float)BorderWidth / singleWCharacterSize.Height;
				float num4 = 9.1f;
				if (PathLineStyle == MapDashStyle.Dash)
				{
					num4 = 8.9f;
				}
				else if (PathLineStyle == MapDashStyle.DashDot)
				{
					num4 = 11.5f;
				}
				else if (PathLineStyle == MapDashStyle.DashDotDot)
				{
					num4 = 9.1f;
				}
				else if (PathLineStyle == MapDashStyle.Dot)
				{
					num4 = 9.1f;
				}
				if (PathWidth == 1)
				{
					num4 *= 3f;
				}
				else if (PathWidth == 2)
				{
					num4 *= 2f;
				}
				else if (PathWidth == 3)
				{
					num4 *= 1.5f;
				}
				int width2 = (int)Math.Round(num2 * num4);
				legendCell.SymbolSize = new Size(width2, (int)Math.Round(num2 + num3 * 2f));
			}
			if (num < legend.CellColumns.Count)
			{
				LegendCellColumn legendCellColumn = legend.CellColumns[num];
				legendCell.ToolTip = legendCellColumn.ToolTip;
				legendCell.Href = legendCellColumn.Href;
				legendCell.CellAttributes = legendCellColumn.CellColumnAttributes;
			}
			num = Cells.Add(LegendCellType.Text, "#LEGENDTEXT", ContentAlignment.MiddleLeft);
			legendCell = Cells[num];
			if (num < legend.CellColumns.Count)
			{
				LegendCellColumn legendCellColumn2 = legend.CellColumns[num];
				legendCell.ToolTip = legendCellColumn2.ToolTip;
				legendCell.Href = legendCellColumn2.Href;
				legendCell.CellAttributes = legendCellColumn2.CellColumnAttributes;
			}
		}

		internal SizeF MeasureLegendItem(MapGraphics graph, int fontSizeReducedBy)
		{
			SizeF empty = SizeF.Empty;
			int num = 0;
			foreach (LegendCell cell in Cells)
			{
				if (num > 0)
				{
					num--;
					continue;
				}
				SizeF sizeF = cell.MeasureCell(graph, fontSizeReducedBy, Legend.autofitFont, Legend.singleWCharacterSize);
				if (cell.CellSpan > 1)
				{
					num = cell.CellSpan - 1;
				}
				empty.Width += sizeF.Width;
				empty.Height = Math.Max(empty.Height, sizeF.Height);
			}
			return empty;
		}

		internal string ReplaceKeywords(string strOriginal)
		{
			if (strOriginal.Length == 0)
			{
				return strOriginal;
			}
			return strOriginal.Replace("#LEGENDTEXT", Text);
		}

		internal override void Invalidate()
		{
			if (Legend != null)
			{
				Legend.Invalidate();
			}
		}
	}
}

using Microsoft.Reporting.Chart.WebForms.ChartTypes;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeLegendItem_LegendItem")]
	[DefaultProperty("Name")]
	internal class LegendItem : IMapAreaAttributes
	{
		private string name = "Legend Item";

		private Color color = Color.Empty;

		private string image = "";

		private string seriesName = "";

		private int seriesPointIndex = -1;

		private string toolTip = "";

		private string href = "";

		private object mapAreaTag;

		private string attributes = "";

		internal LegendImageStyle style;

		internal GradientType backGradientType;

		internal Color backGradientEndColor = Color.Empty;

		internal Color backImageTranspColor = Color.Empty;

		internal Color borderColor = Color.Black;

		internal int borderWidth = 1;

		internal ChartDashStyle borderStyle = ChartDashStyle.Solid;

		internal ChartHatchStyle backHatchStyle;

		internal int shadowOffset;

		internal Color shadowColor = Color.FromArgb(128, 0, 0, 0);

		internal string backImage = "";

		internal ChartImageWrapMode backImageMode;

		internal ChartImageAlign backImageAlign;

		internal MarkerStyle markerStyle;

		internal int markerSize = 5;

		internal string markerImage = "";

		internal Color markerImageTranspColor = Color.Empty;

		internal Color markerColor = Color.Empty;

		internal Color markerBorderColor = Color.Empty;

		internal CommonElements common;

		private Legend m_legend;

		private bool enabled = true;

		private int markerBorderWidth = 1;

		private LegendCellCollection cells;

		private LegendSeparatorType separator;

		private Color separatorColor = Color.Black;

		private object tag;

		internal bool clearTempCells;

		[Bindable(false)]
		[Browsable(false)]
		public Legend Legend
		{
			get
			{
				return m_legend;
			}
			set
			{
				m_legend = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegendItem_Name")]
		[NotifyParentProperty(true)]
		[ParenthesizePropertyName(true)]
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
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
				Invalidate(invalidateLegendOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
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
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(LegendImageStyle), "Rectangle")]
		[SRDescription("DescriptionAttributeLegendItem_Style")]
		[ParenthesizePropertyName(true)]
		public LegendImageStyle Style
		{
			get
			{
				return style;
			}
			set
			{
				style = value;
				Invalidate(invalidateLegendOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLegendItem_BorderColor")]
		public Color BorderColor
		{
			get
			{
				return borderColor;
			}
			set
			{
				borderColor = value;
				Invalidate(invalidateLegendOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartHatchStyle.None)]
		[SRDescription("DescriptionAttributeLegendItem_BackHatchStyle")]
		public ChartHatchStyle BackHatchStyle
		{
			get
			{
				return backHatchStyle;
			}
			set
			{
				backHatchStyle = value;
				Invalidate(invalidateLegendOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLegendItem_BackImageTransparentColor")]
		public Color BackImageTransparentColor
		{
			get
			{
				return backImageTranspColor;
			}
			set
			{
				backImageTranspColor = value;
				Invalidate(invalidateLegendOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(GradientType.None)]
		[SRDescription("DescriptionAttributeLegendItem_BackGradientType")]
		public GradientType BackGradientType
		{
			get
			{
				return backGradientType;
			}
			set
			{
				backGradientType = value;
				Invalidate(invalidateLegendOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendItem_BackGradientEndColor")]
		public Color BackGradientEndColor
		{
			get
			{
				return backGradientEndColor;
			}
			set
			{
				backGradientEndColor = value;
				Invalidate(invalidateLegendOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeLegendItem_BorderWidth")]
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
					throw new ArgumentOutOfRangeException("value", SR.ExceptionBorderWidthIsZero);
				}
				borderWidth = value;
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeLegendItem_Enabled")]
		[ParenthesizePropertyName(true)]
		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[SRCategory("CategoryAttributeMarker")]
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
					throw new ArgumentOutOfRangeException("value", SR.ExceptionLegendMarkerBorderWidthIsNegative);
				}
				markerBorderWidth = value;
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeLegendItem_BorderStyle")]
		public ChartDashStyle BorderStyle
		{
			get
			{
				return borderStyle;
			}
			set
			{
				borderStyle = value;
				Invalidate(invalidateLegendOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegendItem_ShadowOffset")]
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
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "128,0,0,0")]
		[SRDescription("DescriptionAttributeLegendItem_ShadowColor")]
		public Color ShadowColor
		{
			get
			{
				return shadowColor;
			}
			set
			{
				shadowColor = value;
				Invalidate(invalidateLegendOnly: true);
			}
		}

		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[DefaultValue(MarkerStyle.None)]
		[SRDescription("DescriptionAttributeLegendItem_MarkerStyle")]
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
				Invalidate(invalidateLegendOnly: true);
			}
		}

		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[DefaultValue(5)]
		[SRDescription("DescriptionAttributeLegendItem_MarkerSize")]
		[RefreshProperties(RefreshProperties.All)]
		public int MarkerSize
		{
			get
			{
				return markerSize;
			}
			set
			{
				markerSize = value;
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[SRCategory("CategoryAttributeMarker")]
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
				Invalidate(invalidateLegendOnly: true);
			}
		}

		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerImageTransparentColor")]
		[RefreshProperties(RefreshProperties.All)]
		public Color MarkerImageTransparentColor
		{
			get
			{
				return markerImageTranspColor;
			}
			set
			{
				markerImageTranspColor = value;
				Invalidate(invalidateLegendOnly: true);
			}
		}

		[SRCategory("CategoryAttributeMarker")]
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
				Invalidate(invalidateLegendOnly: true);
			}
		}

		[SRCategory("CategoryAttributeMarker")]
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
				Invalidate(invalidateLegendOnly: true);
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

		[SRCategory("CategoryAttributeAppearance")]
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
					Invalidate(invalidateLegendOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
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
					Invalidate(invalidateLegendOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeLegendItem_Cells")]
		public LegendCellCollection Cells => cells;

		[SRCategory("CategoryAttributeMisc")]
		[Browsable(false)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeLegendItem_Tag")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public object Tag
		{
			get
			{
				return tag;
			}
			set
			{
				tag = value;
			}
		}

		[SRCategory("CategoryAttributeMapArea")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeToolTip7")]
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

		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeHref7")]
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

		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMapAreaAttributes9")]
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

		object IMapAreaAttributes.Tag
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

		public LegendItem()
		{
			cells = new LegendCellCollection(this);
		}

		public LegendItem(string name, Color color, string image)
		{
			this.name = name;
			this.color = color;
			this.image = image;
			cells = new LegendCellCollection(this);
		}

		internal void AddAutomaticCells(Legend legend)
		{
			if (Cells.Count != 0)
			{
				return;
			}
			if (SeriesName.Length > 0)
			{
				if (legend.CellColumns.Count == 0)
				{
					if (legend.Common != null && legend.Common.ChartPicture.IsRightToLeft())
					{
						Cells.Add(LegendCellType.Text, "#LEGENDTEXT", ContentAlignment.MiddleLeft);
						Cells.Add(LegendCellType.SeriesSymbol, string.Empty, ContentAlignment.MiddleCenter);
					}
					else
					{
						Cells.Add(LegendCellType.SeriesSymbol, string.Empty, ContentAlignment.MiddleCenter);
						Cells.Add(LegendCellType.Text, "#LEGENDTEXT", ContentAlignment.MiddleLeft);
					}
					return;
				}
				foreach (LegendCellColumn cellColumn in legend.CellColumns)
				{
					Cells.Add(cellColumn.CreateNewCell());
				}
			}
			else
			{
				clearTempCells = true;
				Cells.Add(LegendCellType.SeriesSymbol, string.Empty, ContentAlignment.MiddleCenter);
				Cells.Add(LegendCellType.Text, "#LEGENDTEXT", ContentAlignment.MiddleLeft);
			}
		}

		internal void SetAttributes(CommonElements common, Series series)
		{
			IChartType chartType = common.ChartTypeRegistry.GetChartType(series.ChartTypeName);
			style = chartType.GetLegendImageStyle(series);
			seriesName = series.Name;
			shadowOffset = series.ShadowOffset;
			shadowColor = series.ShadowColor;
			bool enable3D = common.Chart.ChartAreas[series.ChartArea].Area3DStyle.Enable3D;
			SetAttributes(series, enable3D);
		}

		internal void SetAttributes(DataPointAttributes attrib, bool area3D)
		{
			borderColor = attrib.BorderColor;
			borderWidth = attrib.BorderWidth;
			borderStyle = attrib.BorderStyle;
			markerStyle = attrib.MarkerStyle;
			markerSize = attrib.MarkerSize;
			markerImage = attrib.MarkerImage;
			markerImageTranspColor = attrib.MarkerImageTransparentColor;
			markerColor = attrib.MarkerColor;
			markerBorderColor = attrib.MarkerBorderColor;
			markerBorderWidth = attrib.MarkerBorderWidth;
			float num = 96f;
			if (common != null)
			{
				num = common.graph.Graphics.DpiX;
			}
			int num2 = (int)Math.Round(2f * num / 96f);
			if (markerBorderWidth > num2)
			{
				markerBorderWidth = num2;
			}
			if (attrib.MarkerBorderWidth <= 0)
			{
				markerBorderColor = Color.Transparent;
			}
			if (style == LegendImageStyle.Line && borderWidth <= (int)Math.Round(num / 96f))
			{
				borderWidth = num2;
			}
			if (!area3D)
			{
				backGradientType = attrib.BackGradientType;
				backGradientEndColor = attrib.BackGradientEndColor;
				backImageTranspColor = attrib.BackImageTransparentColor;
				backImage = attrib.BackImage;
				backImageMode = attrib.BackImageMode;
				backImageAlign = attrib.BackImageAlign;
				backHatchStyle = attrib.BackHatchStyle;
			}
		}

		private void Invalidate(bool invalidateLegendOnly)
		{
		}
	}
}

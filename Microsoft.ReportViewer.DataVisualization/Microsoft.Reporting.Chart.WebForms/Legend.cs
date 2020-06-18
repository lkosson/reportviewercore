using Microsoft.Reporting.Chart.WebForms.ChartTypes;
using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeLegend_Legend")]
	[DefaultProperty("Enabled")]
	internal class Legend : ChartElement
	{
		private ElementPosition position = new ElementPosition();

		private bool enabled = true;

		private LegendStyle legendStyle = LegendStyle.Table;

		private LegendTableStyle legendTableStyle;

		private LegendItemsCollection customLegends;

		private ChartHatchStyle backHatchStyle;

		private string backImage = "";

		private ChartImageWrapMode backImageMode;

		private Color backImageTranspColor = Color.Empty;

		private ChartImageAlign backImageAlign;

		private GradientType backGradientType;

		private Color backGradientEndColor = Color.Empty;

		private Color borderColor = Color.Black;

		private Color backColor = Color.Transparent;

		private int borderWidth = 1;

		private ChartDashStyle borderStyle;

		private Font font = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);

		private Color fontColor = Color.Black;

		private StringAlignment legendAlignment;

		private LegendDocking legendDocking = LegendDocking.Right;

		private int shadowOffset;

		private Color shadowColor = Color.FromArgb(128, 0, 0, 0);

		private bool autoFitText = true;

		private string name = "";

		private string dockToChartArea = "NotSet";

		private bool dockInsideChartArea = true;

		internal LegendItemsCollection legendItems;

		private SizeF sizeLargestItemText = SizeF.Empty;

		private SizeF sizeAverageItemText = SizeF.Empty;

		private SizeF sizeItemImage = SizeF.Empty;

		private int itemColumns;

		private SizeF itemCellSize = SizeF.Empty;

		internal Font autofitFont;

		private bool equallySpacedItems;

		private bool interlacedRows;

		private Color interlacedRowsColor = Color.Empty;

		private Size offset = Size.Empty;

		private float maximumLegendAutoSize = 50f;

		private PointF animationLocationAdjustment = PointF.Empty;

		private int textWrapThreshold = 25;

		private int autoFitFontSizeAdjustment;

		private LegendCellColumnCollection cellColumns;

		private AutoBool reversed;

		private string title = string.Empty;

		private Color titleColor = Color.Black;

		private Color titleBackColor = Color.Empty;

		private Font titleFont = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f, FontStyle.Bold);

		private StringAlignment titleAlignment = StringAlignment.Center;

		private LegendSeparatorType titleSeparator;

		private Color titleSeparatorColor = Color.Black;

		private LegendSeparatorType headerSeparator;

		private Color headerSeparatorColor = Color.Black;

		private LegendSeparatorType itemColumnSeparator;

		private Color itemColumnSeparatorColor = Color.Black;

		private int itemColumnSpacing = 50;

		private int itemColumnSpacingRel;

		private Rectangle titlePosition = Rectangle.Empty;

		private Rectangle headerPosition = Rectangle.Empty;

		private int autoFitMinFontSize = 7;

		private int horizontalSpaceLeft;

		private int verticalSpaceLeft;

		private int[,] subColumnSizes;

		private int[,] cellHeights;

		private int[] numberOfRowsPerColumn;

		private int numberOfLegendItemsToProcess = -1;

		private Rectangle legendItemsAreaPosition = Rectangle.Empty;

		private bool legendItemsTruncated;

		private int truncatedDotsSize = 3;

		private int numberOfCells = -1;

		internal Size singleWCharacterSize = Size.Empty;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegend_Name")]
		[NotifyParentProperty(true)]
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if (value != name && !(name == "Default"))
				{
					if (value == null || value.Length == 0)
					{
						throw new ArgumentException(SR.ExceptionLegendNameIsEmpty);
					}
					if (base.Common != null && base.Common.ChartPicture != null && base.Common.ChartPicture.Legends.GetIndex(value) != -1)
					{
						throw new ArgumentException(SR.ExceptionLegendNameIsNotUnique(value));
					}
					name = value;
				}
			}
		}

		[SRCategory("CategoryAttributeDocking")]
		[Bindable(true)]
		[DefaultValue("NotSet")]
		[SRDescription("DescriptionAttributeLegend_DockToChartArea")]
		[TypeConverter(typeof(LegendAreaNameConverter))]
		[NotifyParentProperty(true)]
		public string DockToChartArea
		{
			get
			{
				return dockToChartArea;
			}
			set
			{
				if (value != dockToChartArea)
				{
					if (value.Length == 0)
					{
						dockToChartArea = "NotSet";
					}
					else
					{
						dockToChartArea = value;
					}
					Invalidate(invalidateLegendOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeDocking")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeLegend_DockInsideChartArea")]
		[NotifyParentProperty(true)]
		public bool DockInsideChartArea
		{
			get
			{
				return dockInsideChartArea;
			}
			set
			{
				if (value != dockInsideChartArea)
				{
					dockInsideChartArea = value;
					Invalidate(invalidateLegendOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegend_Position")]
		[DefaultValue(typeof(ElementPosition), "Auto")]
		[NotifyParentProperty(true)]
		[TypeConverter(typeof(ElementPositionConverter))]
		[SerializationVisibility(SerializationVisibility.Element)]
		public ElementPosition Position
		{
			get
			{
				if (base.Common != null && base.Common.Chart != null && base.Common.Chart.serializationStatus == SerializationStatus.Saving)
				{
					if (position.Auto)
					{
						return new ElementPosition();
					}
					ElementPosition elementPosition = new ElementPosition();
					elementPosition.Auto = true;
					elementPosition.SetPositionNoAuto(position.X, position.Y, position.Width, position.Height);
					return elementPosition;
				}
				return position;
			}
			set
			{
				position = value;
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeLegend_EquallySpacedItems")]
		[NotifyParentProperty(true)]
		public bool EquallySpacedItems
		{
			get
			{
				return equallySpacedItems;
			}
			set
			{
				equallySpacedItems = value;
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeLegend_Enabled")]
		[NotifyParentProperty(true)]
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

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeLegend_AutoFitText")]
		[NotifyParentProperty(true)]
		public bool AutoFitText
		{
			get
			{
				return autoFitText;
			}
			set
			{
				autoFitText = value;
				if (autoFitText)
				{
					if (this.font != null)
					{
						Font font = new Font(this.font.FontFamily, 8f, this.font.Style);
						this.font.Dispose();
						this.font = font;
					}
					else
					{
						this.font = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);
					}
				}
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(LegendStyle.Table)]
		[SRDescription("DescriptionAttributeLegend_LegendStyle")]
		[NotifyParentProperty(true)]
		[ParenthesizePropertyName(true)]
		public LegendStyle LegendStyle
		{
			get
			{
				return legendStyle;
			}
			set
			{
				legendStyle = value;
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(7)]
		[SRDescription("DescriptionAttributeLegend_AutoFitMinFontSize")]
		public int AutoFitMinFontSize
		{
			get
			{
				return autoFitMinFontSize;
			}
			set
			{
				if (value < 5)
				{
					throw new InvalidOperationException(SR.ExceptionLegendAutoFitMinFontSizeInvalid);
				}
				autoFitMinFontSize = value;
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[SRCategory("CategoryAttributeDocking")]
		[DefaultValue(50f)]
		[SRDescription("DescriptionAttributeLegend_MaxAutoSize")]
		public float MaxAutoSize
		{
			get
			{
				return maximumLegendAutoSize;
			}
			set
			{
				if (value < 0f || value > 100f)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionLegendMaximumAutoSizeInvalid);
				}
				maximumLegendAutoSize = value;
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[SRCategory("CategoryAttributeCellColumns")]
		[SRDescription("DescriptionAttributeLegend_CellColumns")]
		public LegendCellColumnCollection CellColumns => cellColumns;

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(LegendTableStyle.Auto)]
		[SRDescription("DescriptionAttributeLegend_TableStyle")]
		[NotifyParentProperty(true)]
		[ParenthesizePropertyName(true)]
		public LegendTableStyle TableStyle
		{
			get
			{
				return legendTableStyle;
			}
			set
			{
				legendTableStyle = value;
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[SRCategory("CategoryAttributeCellColumns")]
		[DefaultValue(typeof(LegendSeparatorType), "None")]
		[SRDescription("DescriptionAttributeLegend_HeaderSeparator")]
		public LegendSeparatorType HeaderSeparator
		{
			get
			{
				return headerSeparator;
			}
			set
			{
				if (value != headerSeparator)
				{
					headerSeparator = value;
					Invalidate(invalidateLegendOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeCellColumns")]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLegend_HeaderSeparatorColor")]
		public Color HeaderSeparatorColor
		{
			get
			{
				return headerSeparatorColor;
			}
			set
			{
				if (value != headerSeparatorColor)
				{
					headerSeparatorColor = value;
					Invalidate(invalidateLegendOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeCellColumns")]
		[DefaultValue(typeof(LegendSeparatorType), "None")]
		[SRDescription("DescriptionAttributeLegend_ItemColumnSeparator")]
		public LegendSeparatorType ItemColumnSeparator
		{
			get
			{
				return itemColumnSeparator;
			}
			set
			{
				if (value != itemColumnSeparator)
				{
					itemColumnSeparator = value;
					Invalidate(invalidateLegendOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeCellColumns")]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLegend_ItemColumnSeparatorColor")]
		public Color ItemColumnSeparatorColor
		{
			get
			{
				return itemColumnSeparatorColor;
			}
			set
			{
				if (value != itemColumnSeparatorColor)
				{
					itemColumnSeparatorColor = value;
					Invalidate(invalidateLegendOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeCellColumns")]
		[DefaultValue(50)]
		[SRDescription("DescriptionAttributeLegend_ItemColumnSpacing")]
		public int ItemColumnSpacing
		{
			get
			{
				return itemColumnSpacing;
			}
			set
			{
				if (value != itemColumnSpacing)
				{
					if (value < 0)
					{
						throw new ArgumentOutOfRangeException("value", SR.ExceptionLegendColumnSpacingInvalid);
					}
					itemColumnSpacing = value;
					Invalidate(invalidateLegendOnly: false);
				}
			}
		}

		[Browsable(false)]
		[DefaultValue(typeof(Color), "Transparent")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegend_BackColor")]
		[NotifyParentProperty(true)]
		public Color BackColor
		{
			get
			{
				return backColor;
			}
			set
			{
				backColor = value;
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[Browsable(false)]
		[DefaultValue(typeof(Color), "Black")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegend_BorderColor")]
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
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartDashStyle.NotSet)]
		[SRDescription("DescriptionAttributeLegend_BorderStyle")]
		[NotifyParentProperty(true)]
		public ChartDashStyle BorderStyle
		{
			get
			{
				return borderStyle;
			}
			set
			{
				borderStyle = value;
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeLegend_BorderWidth")]
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
					throw new ArgumentOutOfRangeException("value", SR.ExceptionLegendBorderWidthIsNegative);
				}
				borderWidth = value;
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeLegendBackImage7")]
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
				Invalidate(invalidateLegendOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartImageWrapMode.Tile)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLegend_BackImageMode")]
		public ChartImageWrapMode BackImageMode
		{
			get
			{
				return backImageMode;
			}
			set
			{
				backImageMode = value;
				Invalidate(invalidateLegendOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLegend_BackImageTransparentColor")]
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
		[DefaultValue(ChartImageAlign.TopLeft)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLegend_BackImageAlign")]
		public ChartImageAlign BackImageAlign
		{
			get
			{
				return backImageAlign;
			}
			set
			{
				backImageAlign = value;
				Invalidate(invalidateLegendOnly: true);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(GradientType.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLegend_BackGradientType")]
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

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLegend_BackGradientEndColor")]
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

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartHatchStyle.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLegend_BackHatchStyle")]
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

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeLegend_Font")]
		[NotifyParentProperty(true)]
		public Font Font
		{
			get
			{
				return font;
			}
			set
			{
				AutoFitText = false;
				font = value;
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLegend_FontColor")]
		[NotifyParentProperty(true)]
		public Color FontColor
		{
			get
			{
				return fontColor;
			}
			set
			{
				fontColor = value;
				Invalidate(invalidateLegendOnly: true);
			}
		}

		[SRCategory("CategoryAttributeDocking")]
		[Bindable(true)]
		[DefaultValue(StringAlignment.Near)]
		[SRDescription("DescriptionAttributeLegend_Alignment")]
		[NotifyParentProperty(true)]
		public StringAlignment Alignment
		{
			get
			{
				return legendAlignment;
			}
			set
			{
				legendAlignment = value;
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[SRCategory("CategoryAttributeDocking")]
		[Bindable(true)]
		[DefaultValue(LegendDocking.Right)]
		[SRDescription("DescriptionAttributeLegend_Docking")]
		[NotifyParentProperty(true)]
		public LegendDocking Docking
		{
			get
			{
				return legendDocking;
			}
			set
			{
				legendDocking = value;
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeLegend_ShadowOffset")]
		[NotifyParentProperty(true)]
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
		[DefaultValue(typeof(Color), "128, 0, 0, 0")]
		[SRDescription("DescriptionAttributeLegend_ShadowColor")]
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
				Invalidate(invalidateLegendOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[Bindable(false)]
		[DefaultValue("NotSet")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLegend_InsideChartArea")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[TypeConverter(typeof(LegendAreaNameConverter))]
		public string InsideChartArea
		{
			get
			{
				if (base.Common != null && base.Common.Chart != null && base.Common.Chart.serializing)
				{
					return "NotSet";
				}
				return DockToChartArea;
			}
			set
			{
				if (value.Length == 0)
				{
					DockToChartArea = "NotSet";
				}
				else
				{
					DockToChartArea = value;
				}
				Invalidate(invalidateLegendOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLegend_CustomItems")]
		public LegendItemsCollection CustomItems => customLegends;

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(25)]
		[SRDescription("DescriptionAttributeLegend_TextWrapThreshold")]
		public int TextWrapThreshold
		{
			get
			{
				return textWrapThreshold;
			}
			set
			{
				if (value != textWrapThreshold)
				{
					if (value < 0)
					{
						throw new ArgumentException(SR.ExceptionTextThresholdIsNegative, "value");
					}
					textWrapThreshold = value;
					Invalidate(invalidateLegendOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(AutoBool.Auto)]
		[SRDescription("DescriptionAttributeLegend_Reversed")]
		public AutoBool Reversed
		{
			get
			{
				return reversed;
			}
			set
			{
				if (value != reversed)
				{
					reversed = value;
					Invalidate(invalidateLegendOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeLegend_InterlacedRows")]
		public bool InterlacedRows
		{
			get
			{
				return interlacedRows;
			}
			set
			{
				if (value != interlacedRows)
				{
					interlacedRows = value;
					Invalidate(invalidateLegendOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegend_InterlacedRowsColor")]
		public Color InterlacedRowsColor
		{
			get
			{
				return interlacedRowsColor;
			}
			set
			{
				if (value != interlacedRowsColor)
				{
					interlacedRowsColor = value;
					Invalidate(invalidateLegendOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeTitle")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeLegend_Title")]
		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				if (value != title)
				{
					title = value;
					Invalidate(invalidateLegendOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeTitle")]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLegend_TitleColor")]
		public Color TitleColor
		{
			get
			{
				return titleColor;
			}
			set
			{
				if (value != titleColor)
				{
					titleColor = value;
					Invalidate(invalidateLegendOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeTitle")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegend_TitleBackColor")]
		public Color TitleBackColor
		{
			get
			{
				return titleBackColor;
			}
			set
			{
				if (value != titleBackColor)
				{
					titleBackColor = value;
					Invalidate(invalidateLegendOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeTitle")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt, style=Bold")]
		[SRDescription("DescriptionAttributeLegend_TitleFont")]
		public Font TitleFont
		{
			get
			{
				return titleFont;
			}
			set
			{
				if (value != titleFont)
				{
					titleFont = value;
					Invalidate(invalidateLegendOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeTitle")]
		[DefaultValue(typeof(StringAlignment), "Center")]
		[SRDescription("DescriptionAttributeLegend_TitleAlignment")]
		public StringAlignment TitleAlignment
		{
			get
			{
				return titleAlignment;
			}
			set
			{
				if (value != titleAlignment)
				{
					titleAlignment = value;
					Invalidate(invalidateLegendOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeTitle")]
		[DefaultValue(typeof(LegendSeparatorType), "None")]
		[SRDescription("DescriptionAttributeLegend_TitleSeparator")]
		public LegendSeparatorType TitleSeparator
		{
			get
			{
				return titleSeparator;
			}
			set
			{
				if (value != titleSeparator)
				{
					titleSeparator = value;
					Invalidate(invalidateLegendOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeTitle")]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLegend_TitleSeparatorColor")]
		public Color TitleSeparatorColor
		{
			get
			{
				return titleSeparatorColor;
			}
			set
			{
				if (value != titleSeparatorColor)
				{
					titleSeparatorColor = value;
					Invalidate(invalidateLegendOnly: false);
				}
			}
		}

		public Legend()
		{
			customLegends = new LegendItemsCollection();
			customLegends.legend = this;
			legendItems = new LegendItemsCollection();
			legendItems.legend = this;
			cellColumns = new LegendCellColumnCollection(this);
		}

		public Legend(CommonElements common)
		{
			base.Common = common;
			customLegends.common = common;
			position.common = common;
			customLegends = new LegendItemsCollection();
			customLegends.legend = this;
			legendItems = new LegendItemsCollection();
			legendItems.legend = this;
			cellColumns = new LegendCellColumnCollection(this);
		}

		public Legend(string name)
		{
			Name = name;
			customLegends = new LegendItemsCollection();
			customLegends.legend = this;
			legendItems = new LegendItemsCollection();
			legendItems.legend = this;
			cellColumns = new LegendCellColumnCollection(this);
		}

		public Legend(CommonElements common, string name)
		{
			Name = name;
			base.Common = common;
			position.common = common;
			customLegends = new LegendItemsCollection();
			customLegends.common = common;
			customLegends.legend = this;
			legendItems = new LegendItemsCollection();
			legendItems.legend = this;
			cellColumns = new LegendCellColumnCollection(this);
		}

		private void RecalcLegendInfo(ChartGraphics chartGraph)
		{
			RectangleF relative = position.ToRectangleF();
			Rectangle rectangle = Rectangle.Round(chartGraph.GetAbsoluteRectangle(relative));
			singleWCharacterSize = chartGraph.MeasureStringAbs("W", Font);
			Size size = chartGraph.MeasureStringAbs("WW", Font);
			singleWCharacterSize.Width = size.Width - singleWCharacterSize.Width;
			offset.Width = (int)Math.Ceiling((float)singleWCharacterSize.Width / 2f);
			offset.Height = (int)Math.Ceiling((float)singleWCharacterSize.Width / 3f);
			itemColumnSpacingRel = (int)((float)singleWCharacterSize.Width * ((float)itemColumnSpacing / 100f));
			if (itemColumnSpacingRel % 2 == 1)
			{
				itemColumnSpacingRel++;
			}
			titlePosition = Rectangle.Empty;
			if (Title.Length > 0)
			{
				Size titleSize = GetTitleSize(chartGraph, rectangle.Size);
				titlePosition = new Rectangle(rectangle.Location.X, rectangle.Location.Y, rectangle.Width, Math.Min(rectangle.Height, titleSize.Height));
				rectangle.Height -= titlePosition.Height;
				titlePosition.Y += GetBorderSize();
			}
			headerPosition = Rectangle.Empty;
			Size empty = Size.Empty;
			foreach (LegendCellColumn cellColumn in CellColumns)
			{
				if (cellColumn.HeaderText.Length > 0)
				{
					empty.Height = Math.Max(val2: GetHeaderSize(chartGraph, cellColumn).Height, val1: empty.Height);
				}
			}
			if (!empty.IsEmpty)
			{
				headerPosition = new Rectangle(rectangle.Location.X + GetBorderSize() + offset.Width, rectangle.Location.Y + titlePosition.Height, rectangle.Width - (GetBorderSize() + offset.Width) * 2, Math.Min(rectangle.Height - titlePosition.Height, empty.Height));
				headerPosition.Height = Math.Max(headerPosition.Height, 0);
				rectangle.Height -= headerPosition.Height;
				headerPosition.Y += GetBorderSize();
			}
			legendItemsAreaPosition = new Rectangle(rectangle.X + offset.Width + GetBorderSize(), rectangle.Y + offset.Height + GetBorderSize() + titlePosition.Height + headerPosition.Height, rectangle.Width - 2 * (offset.Width + GetBorderSize()), rectangle.Height - 2 * (offset.Height + GetBorderSize()));
			GetNumberOfRowsAndColumns(chartGraph, legendItemsAreaPosition.Size, -1, out numberOfRowsPerColumn, out itemColumns, out horizontalSpaceLeft, out verticalSpaceLeft);
			autoFitFontSizeAdjustment = 0;
			legendItemsTruncated = false;
			bool flag = horizontalSpaceLeft >= 0 && verticalSpaceLeft >= 0;
			numberOfLegendItemsToProcess = legendItems.Count;
			int num = 0;
			for (int i = 0; i < itemColumns; i++)
			{
				num += numberOfRowsPerColumn[i];
			}
			if (num < numberOfLegendItemsToProcess)
			{
				flag = false;
			}
			autofitFont = new Font(Font, Font.Style);
			if (!flag)
			{
				do
				{
					if (AutoFitText && Font.Size - (float)autoFitFontSizeAdjustment > (float)autoFitMinFontSize)
					{
						autoFitFontSizeAdjustment++;
						int num2 = (int)Math.Round(Font.Size - (float)autoFitFontSizeAdjustment);
						if (num2 < 1)
						{
							num2 = 1;
						}
						if (autofitFont != null)
						{
							autofitFont.Dispose();
							autofitFont = null;
						}
						autofitFont = new Font(Font.FontFamily, num2, Font.Style, Font.Unit);
						GetNumberOfRowsAndColumns(chartGraph, legendItemsAreaPosition.Size, -1, out numberOfRowsPerColumn, out itemColumns, out horizontalSpaceLeft, out verticalSpaceLeft);
						flag = (horizontalSpaceLeft >= 0 && verticalSpaceLeft >= 0);
						num = 0;
						for (int j = 0; j < itemColumns; j++)
						{
							num += numberOfRowsPerColumn[j];
						}
						if (num < numberOfLegendItemsToProcess)
						{
							flag = false;
						}
						continue;
					}
					if (numberOfLegendItemsToProcess > 2)
					{
						if (itemColumns == 1 && horizontalSpaceLeft < 0 && verticalSpaceLeft >= 0)
						{
							flag = true;
							numberOfLegendItemsToProcess = Math.Min(numberOfLegendItemsToProcess, numberOfRowsPerColumn[0]);
						}
						else if (GetMaximumNumberOfRows() == 1 && verticalSpaceLeft < 0 && horizontalSpaceLeft >= 0)
						{
							flag = true;
							numberOfLegendItemsToProcess = Math.Min(numberOfLegendItemsToProcess, itemColumns);
						}
						else
						{
							if (!legendItemsTruncated)
							{
								legendItemsAreaPosition.Height -= truncatedDotsSize;
							}
							legendItemsTruncated = true;
							numberOfLegendItemsToProcess--;
							GetNumberOfRowsAndColumns(chartGraph, legendItemsAreaPosition.Size, numberOfLegendItemsToProcess, out numberOfRowsPerColumn, out itemColumns);
						}
						if (flag && !legendItemsTruncated && numberOfLegendItemsToProcess < legendItems.Count)
						{
							legendItemsAreaPosition.Height -= truncatedDotsSize;
							legendItemsTruncated = true;
						}
					}
					else
					{
						flag = true;
					}
					if (!flag)
					{
						flag = CheckLegendItemsFit(chartGraph, legendItemsAreaPosition.Size, numberOfLegendItemsToProcess, autoFitFontSizeAdjustment, itemColumns, numberOfRowsPerColumn, out subColumnSizes, out cellHeights, out horizontalSpaceLeft, out verticalSpaceLeft);
					}
				}
				while (!flag);
			}
			Size empty2 = Size.Empty;
			if (verticalSpaceLeft > 0)
			{
				empty2.Height = verticalSpaceLeft / GetMaximumNumberOfRows() / 2;
			}
			if (horizontalSpaceLeft > 0)
			{
				empty2.Width = horizontalSpaceLeft / 2;
			}
			int num3 = 0;
			int num4 = 0;
			if (numberOfLegendItemsToProcess < 0)
			{
				numberOfLegendItemsToProcess = legendItems.Count;
			}
			for (int k = 0; k < numberOfLegendItemsToProcess; k++)
			{
				LegendItem legendItem = legendItems[k];
				int num5;
				for (num5 = 0; num5 < legendItem.Cells.Count; num5++)
				{
					LegendCell legendCell = legendItem.Cells[num5];
					Rectangle cellPosition = GetCellPosition(chartGraph, num3, num4, num5, empty2);
					int num6 = 0;
					if (legendCell.CellSpan > 1)
					{
						for (int l = 1; l < legendCell.CellSpan && num5 + l < legendItem.Cells.Count; l++)
						{
							Rectangle cellPosition2 = GetCellPosition(chartGraph, num3, num4, num5 + l, empty2);
							if (cellPosition.Right < cellPosition2.Right)
							{
								cellPosition.Width += cellPosition2.Right - cellPosition.Right;
							}
							num6++;
							legendItem.Cells[num5 + l].SetCellPosition(chartGraph, num3, num4, Rectangle.Empty, autoFitFontSizeAdjustment, (autofitFont == null) ? Font : autofitFont, singleWCharacterSize);
						}
					}
					cellPosition.Intersect(legendItemsAreaPosition);
					legendCell.SetCellPosition(chartGraph, num3, num4, cellPosition, autoFitFontSizeAdjustment, (autofitFont == null) ? Font : autofitFont, singleWCharacterSize);
					num5 += num6;
				}
				num4++;
				if (num4 >= numberOfRowsPerColumn[num3])
				{
					num3++;
					num4 = 0;
					if (num3 >= itemColumns)
					{
						break;
					}
				}
			}
		}

		private Rectangle GetCellPosition(ChartGraphics chartGraph, int columnIndex, int rowIndex, int cellIndex, Size itemHalfSpacing)
		{
			Rectangle result = legendItemsAreaPosition;
			for (int i = 0; i < rowIndex; i++)
			{
				result.Y += cellHeights[columnIndex, i];
			}
			if (itemHalfSpacing.Height > 0)
			{
				result.Y += itemHalfSpacing.Height * rowIndex * 2 + itemHalfSpacing.Height;
			}
			if (horizontalSpaceLeft > 0)
			{
				result.X += itemHalfSpacing.Width;
			}
			int num = GetNumberOfCells();
			for (int j = 0; j < columnIndex; j++)
			{
				for (int k = 0; k < num; k++)
				{
					result.X += subColumnSizes[j, k];
				}
				result.X += GetSeparatorSize(chartGraph, ItemColumnSeparator).Width;
			}
			for (int l = 0; l < cellIndex; l++)
			{
				result.X += subColumnSizes[columnIndex, l];
			}
			result.Height = cellHeights[columnIndex, rowIndex];
			result.Width = subColumnSizes[columnIndex, cellIndex];
			return result;
		}

		private SizeF GetOptimalSize(ChartGraphics chartGraph, SizeF maxSizeRel)
		{
			offset = Size.Empty;
			itemColumns = 0;
			horizontalSpaceLeft = 0;
			verticalSpaceLeft = 0;
			subColumnSizes = null;
			numberOfRowsPerColumn = null;
			cellHeights = null;
			autofitFont = null;
			autoFitFontSizeAdjustment = 0;
			numberOfCells = -1;
			numberOfLegendItemsToProcess = -1;
			Size empty = Size.Empty;
			SizeF absoluteSize = chartGraph.GetAbsoluteSize(maxSizeRel);
			Size size = new Size((int)absoluteSize.Width, (int)absoluteSize.Height);
			foreach (LegendItem legendItem in legendItems)
			{
				foreach (LegendCell cell in legendItem.Cells)
				{
					cell.ResetCache();
				}
			}
			if (IsEnabled())
			{
				FillLegendItemsCollection();
				base.Common.EventsManager.OnCustomizeLegend(legendItems, Name);
				if (legendItems.Count > 0)
				{
					singleWCharacterSize = chartGraph.MeasureStringAbs("W", Font);
					Size size2 = chartGraph.MeasureStringAbs("WW", Font);
					singleWCharacterSize.Width = size2.Width - singleWCharacterSize.Width;
					offset.Width = (int)Math.Ceiling((float)singleWCharacterSize.Width / 2f);
					offset.Height = (int)Math.Ceiling((float)singleWCharacterSize.Width / 3f);
					itemColumnSpacingRel = (int)((float)singleWCharacterSize.Width * ((float)itemColumnSpacing / 100f));
					if (itemColumnSpacingRel % 2 == 1)
					{
						itemColumnSpacingRel++;
					}
					Size size3 = Size.Empty;
					if (Title.Length > 0)
					{
						size3 = GetTitleSize(chartGraph, size);
					}
					Size empty2 = Size.Empty;
					foreach (LegendCellColumn cellColumn in CellColumns)
					{
						if (cellColumn.HeaderText.Length > 0)
						{
							empty2.Height = Math.Max(val2: GetHeaderSize(chartGraph, cellColumn).Height, val1: empty2.Height);
						}
					}
					Size legendSize = size;
					legendSize.Width -= 2 * (offset.Width + GetBorderSize());
					legendSize.Height -= 2 * (offset.Height + GetBorderSize());
					legendSize.Height -= size3.Height;
					legendSize.Height -= empty2.Height;
					autoFitFontSizeAdjustment = 0;
					autofitFont = new Font(Font, Font.Style);
					int vertSpaceLeft = 0;
					int horSpaceLeft = 0;
					bool flag = AutoFitText;
					bool flag2 = false;
					do
					{
						GetNumberOfRowsAndColumns(chartGraph, legendSize, -1, out numberOfRowsPerColumn, out itemColumns, out horSpaceLeft, out vertSpaceLeft);
						int num = 0;
						for (int i = 0; i < itemColumns; i++)
						{
							num += numberOfRowsPerColumn[i];
						}
						flag2 = (horSpaceLeft >= 0 && vertSpaceLeft >= 0 && num >= legendItems.Count);
						if (!flag || flag2)
						{
							continue;
						}
						if (Font.Size - (float)autoFitFontSizeAdjustment > (float)autoFitMinFontSize)
						{
							autoFitFontSizeAdjustment++;
							int num2 = (int)Math.Round(Font.Size - (float)autoFitFontSizeAdjustment);
							if (num2 < 1)
							{
								num2 = 1;
							}
							if (autofitFont != null)
							{
								autofitFont.Dispose();
								autofitFont = null;
							}
							autofitFont = new Font(Font.FontFamily, num2, Font.Style, Font.Unit);
						}
						else
						{
							flag = false;
						}
					}
					while (flag && !flag2);
					horSpaceLeft -= Math.Min(4, horSpaceLeft);
					vertSpaceLeft -= Math.Min(2, vertSpaceLeft);
					empty.Width = legendSize.Width - horSpaceLeft;
					empty.Width = Math.Max(empty.Width, size3.Width);
					empty.Width += 2 * (offset.Width + GetBorderSize());
					empty.Height = legendSize.Height - vertSpaceLeft + size3.Height + empty2.Height;
					empty.Height += 2 * (offset.Height + GetBorderSize());
					if (horSpaceLeft < 0 || vertSpaceLeft < 0)
					{
						empty.Height += truncatedDotsSize;
					}
					if (empty.Width > size.Width)
					{
						empty.Width = size.Width;
					}
					if (empty.Height > size.Height)
					{
						empty.Height = size.Height;
					}
					if (empty.Width < 0)
					{
						empty.Width = 0;
					}
					if (empty.Height < 0)
					{
						empty.Height = 0;
					}
				}
			}
			return chartGraph.GetRelativeSize(empty);
		}

		internal void CalcLegendPosition(ChartGraphics chartGraph, ref RectangleF chartAreasRectangle, float maxLegendSize, float elementSpacing)
		{
			RectangleF rectangleF = default(RectangleF);
			SizeF maxSizeRel = new SizeF(chartAreasRectangle.Width - 2f * elementSpacing, chartAreasRectangle.Height - 2f * elementSpacing);
			if (DockToChartArea == "NotSet")
			{
				if (Docking == LegendDocking.Top || Docking == LegendDocking.Bottom)
				{
					maxSizeRel.Height = maxSizeRel.Height / 100f * maximumLegendAutoSize;
				}
				else
				{
					maxSizeRel.Width = maxSizeRel.Width / 100f * maximumLegendAutoSize;
				}
			}
			if (maxSizeRel.Width <= 0f || maxSizeRel.Height <= 0f)
			{
				return;
			}
			SizeF optimalSize = GetOptimalSize(chartGraph, maxSizeRel);
			rectangleF.Height = optimalSize.Height;
			rectangleF.Width = optimalSize.Width;
			if (float.IsNaN(optimalSize.Height) || float.IsNaN(optimalSize.Width))
			{
				return;
			}
			if (Docking == LegendDocking.Top)
			{
				rectangleF.Y = chartAreasRectangle.Y + elementSpacing;
				if (Alignment == StringAlignment.Near)
				{
					rectangleF.X = chartAreasRectangle.X + elementSpacing;
				}
				else if (Alignment == StringAlignment.Far)
				{
					rectangleF.X = chartAreasRectangle.Right - optimalSize.Width - elementSpacing;
				}
				else if (Alignment == StringAlignment.Center)
				{
					rectangleF.X = chartAreasRectangle.X + (chartAreasRectangle.Width - optimalSize.Width) / 2f;
				}
				chartAreasRectangle.Height -= rectangleF.Height + elementSpacing;
				chartAreasRectangle.Y = rectangleF.Bottom;
			}
			else if (Docking == LegendDocking.Bottom)
			{
				rectangleF.Y = chartAreasRectangle.Bottom - optimalSize.Height - elementSpacing;
				if (Alignment == StringAlignment.Near)
				{
					rectangleF.X = chartAreasRectangle.X + elementSpacing;
				}
				else if (Alignment == StringAlignment.Far)
				{
					rectangleF.X = chartAreasRectangle.Right - optimalSize.Width - elementSpacing;
				}
				else if (Alignment == StringAlignment.Center)
				{
					rectangleF.X = chartAreasRectangle.X + (chartAreasRectangle.Width - optimalSize.Width) / 2f;
				}
				chartAreasRectangle.Height -= rectangleF.Height + elementSpacing;
			}
			if (Docking == LegendDocking.Left)
			{
				rectangleF.X = chartAreasRectangle.X + elementSpacing;
				if (Alignment == StringAlignment.Near)
				{
					rectangleF.Y = chartAreasRectangle.Y + elementSpacing;
				}
				else if (Alignment == StringAlignment.Far)
				{
					rectangleF.Y = chartAreasRectangle.Bottom - optimalSize.Height - elementSpacing;
				}
				else if (Alignment == StringAlignment.Center)
				{
					rectangleF.Y = chartAreasRectangle.Y + (chartAreasRectangle.Height - optimalSize.Height) / 2f;
				}
				chartAreasRectangle.Width -= rectangleF.Width + elementSpacing;
				chartAreasRectangle.X = rectangleF.Right;
			}
			if (Docking == LegendDocking.Right)
			{
				rectangleF.X = chartAreasRectangle.Right - optimalSize.Width - elementSpacing;
				if (Alignment == StringAlignment.Near)
				{
					rectangleF.Y = chartAreasRectangle.Y + elementSpacing;
				}
				else if (Alignment == StringAlignment.Far)
				{
					rectangleF.Y = chartAreasRectangle.Bottom - optimalSize.Height - elementSpacing;
				}
				else if (Alignment == StringAlignment.Center)
				{
					rectangleF.Y = chartAreasRectangle.Y + (chartAreasRectangle.Height - optimalSize.Height) / 2f;
				}
				chartAreasRectangle.Width -= rectangleF.Width + elementSpacing;
			}
			Position.SetPositionNoAuto(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height);
		}

		private void GetNumberOfRowsAndColumns(ChartGraphics chartGraph, Size legendSize, int numberOfItemsToCheck, out int[] numberOfRowsPerColumn, out int columnNumber)
		{
			int horSpaceLeft = 0;
			int vertSpaceLeft = 0;
			GetNumberOfRowsAndColumns(chartGraph, legendSize, numberOfItemsToCheck, out numberOfRowsPerColumn, out columnNumber, out horSpaceLeft, out vertSpaceLeft);
		}

		private void GetNumberOfRowsAndColumns(ChartGraphics chartGraph, Size legendSize, int numberOfItemsToCheck, out int[] numberOfRowsPerColumn, out int columnNumber, out int horSpaceLeft, out int vertSpaceLeft)
		{
			numberOfRowsPerColumn = null;
			columnNumber = 1;
			horSpaceLeft = 0;
			vertSpaceLeft = 0;
			if (numberOfItemsToCheck < 0)
			{
				numberOfItemsToCheck = legendItems.Count;
			}
			if (LegendStyle == LegendStyle.Column || numberOfItemsToCheck <= 1)
			{
				columnNumber = 1;
				numberOfRowsPerColumn = new int[1]
				{
					numberOfItemsToCheck
				};
			}
			else if (LegendStyle == LegendStyle.Row)
			{
				columnNumber = numberOfItemsToCheck;
				numberOfRowsPerColumn = new int[columnNumber];
				for (int i = 0; i < columnNumber; i++)
				{
					numberOfRowsPerColumn[i] = 1;
				}
			}
			else if (LegendStyle == LegendStyle.Table)
			{
				columnNumber = 1;
				numberOfRowsPerColumn = new int[1]
				{
					1
				};
				switch (GetLegendTableStyle(chartGraph))
				{
				case LegendTableStyle.Tall:
				{
					bool flag4 = false;
					int num6 = 1;
					num6 = 1;
					while (!flag4 && num6 < numberOfItemsToCheck)
					{
						numberOfRowsPerColumn[columnNumber - 1]++;
						if (!CheckLegendItemsFit(chartGraph, legendSize, num6 + 1, autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out subColumnSizes, out cellHeights, out horSpaceLeft, out vertSpaceLeft) && ((columnNumber != 1 && horSpaceLeft >= 0) || vertSpaceLeft <= 0))
						{
							if (numberOfRowsPerColumn[columnNumber - 1] > 1)
							{
								numberOfRowsPerColumn[columnNumber - 1]--;
							}
							int num7 = 0;
							if (horSpaceLeft > 0)
							{
								num7 = (int)Math.Round((double)(legendSize.Width - horSpaceLeft) / (double)columnNumber) / 2;
							}
							if (columnNumber < 50 && horSpaceLeft >= num7)
							{
								columnNumber++;
								int[] array2 = numberOfRowsPerColumn;
								numberOfRowsPerColumn = new int[columnNumber];
								for (int num8 = 0; num8 < array2.Length; num8++)
								{
									numberOfRowsPerColumn[num8] = array2[num8];
								}
								numberOfRowsPerColumn[columnNumber - 1] = 1;
								if (num6 == numberOfItemsToCheck - 1)
								{
									CheckLegendItemsFit(chartGraph, legendSize, num6 + 1, autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out subColumnSizes, out cellHeights, out horSpaceLeft, out vertSpaceLeft);
								}
							}
							else
							{
								flag4 = true;
							}
						}
						num6++;
					}
					if (columnNumber <= 1)
					{
						break;
					}
					bool flag5 = false;
					while (!flag5)
					{
						flag5 = true;
						int num9 = -1;
						for (int num10 = 0; num10 < columnNumber; num10++)
						{
							int num11 = 0;
							for (int num12 = 0; num12 < this.numberOfRowsPerColumn[num10] - 1; num12++)
							{
								num11 += cellHeights[num10, num12];
							}
							num9 = Math.Max(num9, num11);
						}
						int num13 = 0;
						for (int num14 = 0; num14 < columnNumber - 1; num14++)
						{
							if (this.numberOfRowsPerColumn[num14] > 1)
							{
								num13 += cellHeights[num14, this.numberOfRowsPerColumn[num14] - 1];
							}
						}
						if (num13 <= 0 || GetColumnHeight(columnNumber - 1) + num13 > num9)
						{
							continue;
						}
						int num15 = 0;
						for (int num16 = 0; num16 < columnNumber - 1; num16++)
						{
							if (this.numberOfRowsPerColumn[num16] > 1)
							{
								this.numberOfRowsPerColumn[num16]--;
								num15++;
							}
						}
						if (num15 > 0)
						{
							this.numberOfRowsPerColumn[columnNumber - 1] += num15;
							CheckLegendItemsFit(chartGraph, legendSize, num6 + 1, autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out subColumnSizes, out cellHeights, out horSpaceLeft, out vertSpaceLeft);
							flag5 = false;
						}
					}
					break;
				}
				case LegendTableStyle.Wide:
				{
					bool flag = false;
					int num = 1;
					num = 1;
					while (!flag && num < numberOfItemsToCheck)
					{
						columnNumber++;
						int[] array = numberOfRowsPerColumn;
						numberOfRowsPerColumn = new int[columnNumber];
						for (int j = 0; j < array.Length; j++)
						{
							numberOfRowsPerColumn[j] = array[j];
						}
						numberOfRowsPerColumn[columnNumber - 1] = 1;
						bool flag2 = CheckLegendItemsFit(chartGraph, legendSize, num + 1, autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out subColumnSizes, out cellHeights, out horSpaceLeft, out vertSpaceLeft);
						if (!flag2 && ((GetMaximumNumberOfRows(numberOfRowsPerColumn) != 1 && vertSpaceLeft >= 0) || horSpaceLeft <= 0))
						{
							bool flag3 = true;
							while (flag3)
							{
								flag3 = false;
								int num2 = 0;
								if (columnNumber > 1)
								{
									num2 = numberOfRowsPerColumn[columnNumber - 1];
									columnNumber--;
									array = numberOfRowsPerColumn;
									numberOfRowsPerColumn = new int[columnNumber];
									for (int k = 0; k < columnNumber; k++)
									{
										numberOfRowsPerColumn[k] = array[k];
									}
								}
								for (int l = 0; l < num2; l++)
								{
									int num3 = -1;
									int num4 = int.MaxValue;
									for (int m = 0; m < columnNumber; m++)
									{
										int columnHeight = GetColumnHeight(m);
										int num5 = 0;
										if (m < columnNumber - 1)
										{
											num5 = cellHeights[m + 1, 0];
										}
										if (columnHeight < num4 && columnHeight + num5 < legendSize.Height)
										{
											num4 = columnHeight;
											num3 = m;
										}
									}
									if (num3 < 0)
									{
										flag2 = CheckLegendItemsFit(chartGraph, legendSize, num + 1, autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out subColumnSizes, out cellHeights, out horSpaceLeft, out vertSpaceLeft);
										flag = true;
										break;
									}
									numberOfRowsPerColumn[num3]++;
									if (num3 < columnNumber - 1 && numberOfRowsPerColumn[num3 + 1] == 1)
									{
										array = numberOfRowsPerColumn;
										for (int n = num3 + 1; n < array.Length - 1; n++)
										{
											numberOfRowsPerColumn[n] = array[n + 1];
										}
										numberOfRowsPerColumn[columnNumber - 1] = 1;
									}
									flag2 = CheckLegendItemsFit(chartGraph, legendSize, num + 1, autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out subColumnSizes, out cellHeights, out horSpaceLeft, out vertSpaceLeft);
								}
								if (!flag2 && (float)horSpaceLeft < 0f && columnNumber > 1)
								{
									flag3 = true;
								}
							}
						}
						num++;
					}
					break;
				}
				}
			}
			CheckLegendItemsFit(chartGraph, legendSize, -1, autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out subColumnSizes, out cellHeights, out horSpaceLeft, out vertSpaceLeft);
		}

		private int GetColumnHeight(int columnIndex)
		{
			int num = 0;
			for (int i = 0; i < numberOfRowsPerColumn[columnIndex]; i++)
			{
				num += cellHeights[columnIndex, i];
			}
			return num;
		}

		private int GetMaximumNumberOfRows()
		{
			return GetMaximumNumberOfRows(numberOfRowsPerColumn);
		}

		private int GetMaximumNumberOfRows(int[] rowsPerColumn)
		{
			int num = 0;
			if (rowsPerColumn != null)
			{
				for (int i = 0; i < rowsPerColumn.Length; i++)
				{
					num = Math.Max(num, rowsPerColumn[i]);
				}
			}
			return num;
		}

		private bool CheckLegendItemsFit(ChartGraphics graph, Size legendItemsAreaSize, int numberOfItemsToCheck, int fontSizeReducedBy, int numberOfColumns, int[] numberOfRowsPerColumn, out int[,] subColumnSizes, out int[,] cellHeights, out int horizontalSpaceLeft, out int verticalSpaceLeft)
		{
			bool result = true;
			horizontalSpaceLeft = 0;
			verticalSpaceLeft = 0;
			if (numberOfItemsToCheck < 0)
			{
				numberOfItemsToCheck = legendItems.Count;
			}
			int num = GetNumberOfCells();
			int maximumNumberOfRows = GetMaximumNumberOfRows(numberOfRowsPerColumn);
			int[,,] array = new int[numberOfColumns, maximumNumberOfRows, num];
			cellHeights = new int[numberOfColumns, maximumNumberOfRows];
			singleWCharacterSize = graph.MeasureStringAbs("W", (autofitFont == null) ? Font : autofitFont);
			Size size = graph.MeasureStringAbs("WW", (autofitFont == null) ? Font : autofitFont);
			singleWCharacterSize.Width = size.Width - singleWCharacterSize.Width;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < numberOfItemsToCheck; i++)
			{
				LegendItem legendItem = legendItems[i];
				int num4 = 0;
				for (int j = 0; j < legendItem.Cells.Count; j++)
				{
					LegendCell legendCell = legendItem.Cells[j];
					LegendCellColumn legendCellColumn = null;
					if (j < CellColumns.Count)
					{
						legendCellColumn = CellColumns[j];
					}
					if (num4 > 0)
					{
						array[num2, num3, j] = -1;
						num4--;
						continue;
					}
					if (legendCell.CellSpan > 1)
					{
						num4 = legendCell.CellSpan - 1;
					}
					Size size2 = legendCell.MeasureCell(graph, fontSizeReducedBy, (autofitFont == null) ? Font : autofitFont, singleWCharacterSize);
					if (legendCellColumn != null)
					{
						if (legendCellColumn.MinimumWidth >= 0)
						{
							size2.Width = (int)Math.Max(size2.Width, (float)(legendCellColumn.MinimumWidth * singleWCharacterSize.Width) / 100f);
						}
						if (legendCellColumn.MaximumWidth >= 0)
						{
							size2.Width = (int)Math.Min(size2.Width, (float)(legendCellColumn.MaximumWidth * singleWCharacterSize.Width) / 100f);
						}
					}
					array[num2, num3, j] = size2.Width;
					if (j == 0)
					{
						cellHeights[num2, num3] = size2.Height;
					}
					else
					{
						cellHeights[num2, num3] = Math.Max(cellHeights[num2, num3], size2.Height);
					}
				}
				num3++;
				if (num3 < numberOfRowsPerColumn[num2])
				{
					continue;
				}
				num2++;
				num3 = 0;
				if (num2 >= numberOfColumns)
				{
					if (i < numberOfItemsToCheck - 1)
					{
						result = false;
					}
					break;
				}
			}
			subColumnSizes = new int[numberOfColumns, num];
			bool flag = false;
			for (num2 = 0; num2 < numberOfColumns; num2++)
			{
				for (int k = 0; k < num; k++)
				{
					int num5 = 0;
					for (num3 = 0; num3 < numberOfRowsPerColumn[num2]; num3++)
					{
						int num6 = array[num2, num3, k];
						if (num6 < 0)
						{
							flag = true;
						}
						else if (k + 1 >= num || array[num2, num3, k + 1] >= 0)
						{
							num5 = Math.Max(num5, num6);
						}
					}
					subColumnSizes[num2, k] = num5;
				}
			}
			for (num2 = 0; num2 < numberOfColumns; num2++)
			{
				for (int l = 0; l < num; l++)
				{
					if (l >= CellColumns.Count)
					{
						continue;
					}
					LegendCellColumn legendCellColumn2 = CellColumns[l];
					if (legendCellColumn2.HeaderText.Length <= 0)
					{
						continue;
					}
					Size size3 = graph.MeasureStringAbs(legendCellColumn2.HeaderText + "I", legendCellColumn2.HeaderFont);
					if (size3.Width > subColumnSizes[num2, l])
					{
						subColumnSizes[num2, l] = size3.Width;
						if (legendCellColumn2.MinimumWidth >= 0)
						{
							subColumnSizes[num2, l] = (int)Math.Max(subColumnSizes[num2, l], (float)(legendCellColumn2.MinimumWidth * singleWCharacterSize.Width) / 100f);
						}
						if (legendCellColumn2.MaximumWidth >= 0)
						{
							subColumnSizes[num2, l] = (int)Math.Min(subColumnSizes[num2, l], (float)(legendCellColumn2.MaximumWidth * singleWCharacterSize.Width) / 100f);
						}
					}
				}
			}
			if (flag)
			{
				for (num2 = 0; num2 < numberOfColumns; num2++)
				{
					for (int m = 0; m < num; m++)
					{
						for (num3 = 0; num3 < numberOfRowsPerColumn[num2]; num3++)
						{
							int num7 = array[num2, num3, m];
							int n;
							for (n = 0; m + n + 1 < num && array[num2, num3, m + n + 1] < 0; n++)
							{
							}
							if (n > 0)
							{
								int num8 = 0;
								for (int num9 = 0; num9 <= n; num9++)
								{
									num8 += subColumnSizes[num2, m + num9];
								}
								if (num7 > num8)
								{
									subColumnSizes[num2, m + n] += num7 - num8;
								}
							}
						}
					}
				}
			}
			if (EquallySpacedItems)
			{
				for (int num10 = 0; num10 < num; num10++)
				{
					int num11 = 0;
					for (num2 = 0; num2 < numberOfColumns; num2++)
					{
						num11 = Math.Max(num11, subColumnSizes[num2, num10]);
					}
					for (num2 = 0; num2 < numberOfColumns; num2++)
					{
						subColumnSizes[num2, num10] = num11;
					}
				}
			}
			int num12 = 0;
			int num13 = 0;
			for (num2 = 0; num2 < numberOfColumns; num2++)
			{
				for (int num14 = 0; num14 < num; num14++)
				{
					num12 += subColumnSizes[num2, num14];
				}
				if (num2 < numberOfColumns - 1)
				{
					num13 += GetSeparatorSize(graph, ItemColumnSeparator).Width;
				}
			}
			int num15 = 0;
			for (num2 = 0; num2 < numberOfColumns; num2++)
			{
				int num16 = 0;
				for (num3 = 0; num3 < numberOfRowsPerColumn[num2]; num3++)
				{
					num16 += cellHeights[num2, num3];
				}
				num15 = Math.Max(num15, num16);
			}
			horizontalSpaceLeft = legendItemsAreaSize.Width - num12 - num13;
			if (horizontalSpaceLeft < 0)
			{
				result = false;
			}
			verticalSpaceLeft = legendItemsAreaSize.Height - num15;
			if (verticalSpaceLeft < 0)
			{
				result = false;
			}
			return result;
		}

		private int GetNumberOfCells()
		{
			if (numberOfCells < 0)
			{
				numberOfCells = CellColumns.Count;
				foreach (LegendItem legendItem in legendItems)
				{
					numberOfCells = Math.Max(numberOfCells, legendItem.Cells.Count);
				}
			}
			return numberOfCells;
		}

		private void FillLegendItemsCollection()
		{
			legendItems.Clear();
			foreach (Series item in base.Common.DataManager.Series)
			{
				try
				{
					_ = base.Common.ChartPicture.Legends[item.Legend];
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionLegendReferencedInSeriesNotFound(item.Name, item.Legend));
				}
			}
			bool flag = false;
			foreach (Series item2 in base.Common.DataManager.Series)
			{
				if (base.Common.ChartPicture.Legends[item2.Legend] != this || item2.ChartArea.Length <= 0)
				{
					continue;
				}
				bool flag2 = false;
				foreach (ChartArea chartArea in base.Common.ChartPicture.ChartAreas)
				{
					if (chartArea.Name == item2.ChartArea)
					{
						flag2 = true;
						break;
					}
				}
				if (!(item2.IsVisible() && flag2))
				{
					continue;
				}
				IChartType chartType = base.Common.ChartTypeRegistry.GetChartType(item2.ChartTypeName);
				if (Reversed == AutoBool.Auto && (item2.ChartType == SeriesChartType.StackedArea || item2.ChartType == SeriesChartType.StackedArea100 || item2.ChartType == SeriesChartType.Pyramid || item2.ChartType == SeriesChartType.StackedColumn || item2.ChartType == SeriesChartType.StackedColumn100))
				{
					flag = true;
				}
				if (chartType.DataPointsInLegend)
				{
					bool flag3 = false;
					foreach (DataPoint point in item2.Points)
					{
						if (point.XValue != 0.0)
						{
							flag3 = true;
							break;
						}
					}
					int num = 0;
					foreach (DataPoint point2 in item2.Points)
					{
						if (point2.Empty)
						{
							num++;
							continue;
						}
						if (!point2.ShowInLegend)
						{
							num++;
							continue;
						}
						LegendItem legendItem = new LegendItem(point2.Label, point2.Color, "");
						bool enable3D = base.Common.Chart.ChartAreas[item2.ChartArea].Area3DStyle.Enable3D;
						legendItem.SetAttributes(base.Common, item2);
						legendItem.SetAttributes(point2, enable3D);
						legendItem.ToolTip = point2.ReplaceKeywords(point2.LegendToolTip);
						legendItem.MapAreaAttributes = point2.ReplaceKeywords(point2.LegendMapAreaAttributes);
						legendItem.Href = point2.ReplaceKeywords(point2.LegendHref);
						((IMapAreaAttributes)legendItem).Tag = point2.LegendTag;
						legendItem.Name = point2.ReplaceKeywords(point2.LegendText);
						legendItem.SeriesPointIndex = num++;
						if (legendItem.Name.Length == 0)
						{
							legendItem.Name = point2.ReplaceKeywords((point2.Label.Length > 0) ? point2.Label : point2.AxisLabel);
						}
						if (legendItem.Name.Length == 0 && flag3)
						{
							legendItem.Name = ValueConverter.FormatValue(item2.chart, this, point2.XValue, "", point2.series.XValueType, ChartElementType.LegendItem);
						}
						if (legendItem.Name.Length == 0)
						{
							legendItem.Name = "Point " + num;
						}
						legendItem.AddAutomaticCells(this);
						foreach (LegendCell cell in legendItem.Cells)
						{
							if (cell.Text.Length > 0)
							{
								cell.Text = cell.Text.Replace("#LEGENDTEXT", legendItem.Name);
								cell.Text = point2.ReplaceKeywords(cell.Text);
								cell.ToolTip = point2.ReplaceKeywords(cell.ToolTip);
								cell.Href = point2.ReplaceKeywords(cell.Href);
								cell.MapAreaAttributes = point2.ReplaceKeywords(cell.MapAreaAttributes);
							}
						}
						if (item2.chart != null && item2.chart.LocalizeTextHandler != null)
						{
							legendItem.Name = item2.chart.LocalizeTextHandler(this, legendItem.Name, point2.ElementId, ChartElementType.LegendItem);
						}
						legendItems.Add(legendItem);
					}
				}
				else
				{
					if (!item2.ShowInLegend)
					{
						continue;
					}
					LegendItem legendItem2 = new LegendItem(item2.Name, item2.Color, "");
					legendItem2.SetAttributes(base.Common, item2);
					legendItem2.ToolTip = item2.ReplaceKeywords(item2.LegendToolTip);
					legendItem2.Href = item2.ReplaceKeywords(item2.LegendHref);
					legendItem2.MapAreaAttributes = item2.ReplaceKeywords(item2.LegendMapAreaAttributes);
					((IMapAreaAttributes)legendItem2).Tag = item2.LegendTag;
					if (item2.LegendText.Length > 0)
					{
						legendItem2.Name = item2.ReplaceKeywords(item2.LegendText);
					}
					legendItem2.AddAutomaticCells(this);
					foreach (LegendCell cell2 in legendItem2.Cells)
					{
						if (cell2.Text.Length > 0)
						{
							cell2.Text = cell2.Text.Replace("#LEGENDTEXT", legendItem2.Name);
							cell2.Text = item2.ReplaceKeywords(cell2.Text);
							cell2.ToolTip = item2.ReplaceKeywords(cell2.ToolTip);
							cell2.Href = item2.ReplaceKeywords(cell2.Href);
							cell2.MapAreaAttributes = item2.ReplaceKeywords(cell2.MapAreaAttributes);
						}
					}
					if (item2.chart != null && item2.chart.LocalizeTextHandler != null)
					{
						legendItem2.Name = item2.chart.LocalizeTextHandler(this, legendItem2.Name, item2.ElementId, ChartElementType.LegendItem);
					}
					legendItems.Add(legendItem2);
				}
			}
			if (Reversed == AutoBool.True || (Reversed == AutoBool.Auto && flag))
			{
				legendItems.Reverse();
			}
			foreach (LegendItem customLegend in customLegends)
			{
				if (customLegend.Enabled)
				{
					legendItems.Add(customLegend);
				}
			}
			if (legendItems.Count == 0 && base.Common != null && base.Common.Chart != null && base.Common.Chart.IsDesignMode())
			{
				LegendItem legendItem4 = new LegendItem(Name + " - " + SR.DescriptionTypeEmpty, Color.White, "");
				legendItem4.Style = LegendImageStyle.Line;
				legendItems.Add(legendItem4);
			}
			foreach (LegendItem legendItem5 in legendItems)
			{
				legendItem5.AddAutomaticCells(this);
			}
		}

		internal void Paint(ChartGraphics chartGraph)
		{
			offset = Size.Empty;
			itemColumns = 0;
			horizontalSpaceLeft = 0;
			verticalSpaceLeft = 0;
			subColumnSizes = null;
			numberOfRowsPerColumn = null;
			cellHeights = null;
			autofitFont = null;
			autoFitFontSizeAdjustment = 0;
			numberOfCells = -1;
			numberOfLegendItemsToProcess = -1;
			if (!IsEnabled() || (MaxAutoSize == 0f && Position.Auto))
			{
				return;
			}
			base.Common.TraceWrite("ChartPainting", SR.TraceMessageBeginDrawingChartLegend);
			FillLegendItemsCollection();
			foreach (LegendItem legendItem3 in legendItems)
			{
				foreach (LegendCell cell in legendItem3.Cells)
				{
					cell.ResetCache();
				}
			}
			base.Common.EventsManager.OnCustomizeLegend(legendItems, Name);
			if (legendItems.Count == 0)
			{
				return;
			}
			RecalcLegendInfo(chartGraph);
			animationLocationAdjustment = PointF.Empty;
			if (base.Common.ProcessModePaint)
			{
				chartGraph.StartAnimation();
				chartGraph.FillRectangleRel(chartGraph.GetRelativeRectangle(Rectangle.Round(chartGraph.GetAbsoluteRectangle(Position.ToRectangleF()))), BackColor, BackHatchStyle, BackImage, BackImageMode, BackImageTransparentColor, BackImageAlign, BackGradientType, BackGradientEndColor, BorderColor, GetBorderSize(), BorderStyle, ShadowColor, ShadowOffset, PenAlignment.Inset);
				chartGraph.StopAnimation();
				base.Common.EventsManager.OnBackPaint(this, new ChartPaintEventArgs(chartGraph, base.Common, Position));
			}
			if (base.Common.ProcessModeRegions)
			{
				SelectLegendBackground(chartGraph);
			}
			DrawLegendHeader(chartGraph);
			DrawLegendTitle(chartGraph);
			if (base.Common.ProcessModeRegions && !titlePosition.IsEmpty)
			{
				base.Common.HotRegionsList.AddHotRegion(chartGraph.GetRelativeRectangle(titlePosition), this, ChartElementType.LegendTitle, relativeCoordinates: true);
			}
			if (numberOfLegendItemsToProcess < 0)
			{
				numberOfLegendItemsToProcess = legendItems.Count;
			}
			for (int i = 0; i < numberOfLegendItemsToProcess; i++)
			{
				LegendItem legendItem = legendItems[i];
				for (int j = 0; j < legendItem.Cells.Count; j++)
				{
					legendItem.Cells[j].Paint(chartGraph, autoFitFontSizeAdjustment, autofitFont, singleWCharacterSize, animationLocationAdjustment);
				}
				if (legendItem.Separator == LegendSeparatorType.None || legendItem.Cells.Count <= 0)
				{
					continue;
				}
				Rectangle empty = Rectangle.Empty;
				empty.X = legendItem.Cells[0].cellPosition.Left;
				int num = 0;
				for (int num2 = legendItem.Cells.Count - 1; num2 >= 0; num2--)
				{
					num = legendItem.Cells[num2].cellPosition.Right;
					if (num > 0)
					{
						break;
					}
				}
				empty.Width = num - empty.X;
				empty.Y = legendItem.Cells[0].cellPosition.Bottom;
				empty.Height = GetSeparatorSize(chartGraph, legendItem.Separator).Height;
				empty.Intersect(legendItemsAreaPosition);
				DrawSeparator(chartGraph, legendItem.Separator, legendItem.SeparatorColor, horizontal: true, empty);
			}
			if (ItemColumnSeparator != 0)
			{
				Rectangle rectangle = Rectangle.Round(chartGraph.GetAbsoluteRectangle(Position.ToRectangleF()));
				rectangle.Y += GetBorderSize() + titlePosition.Height;
				rectangle.Height -= 2 * GetBorderSize() + titlePosition.Height;
				rectangle.X += GetBorderSize() + offset.Width;
				rectangle.Width = GetSeparatorSize(chartGraph, ItemColumnSeparator).Width;
				if (horizontalSpaceLeft > 0)
				{
					rectangle.X += horizontalSpaceLeft / 2;
				}
				if (rectangle.Width > 0 && rectangle.Height > 0)
				{
					for (int k = 0; k < itemColumns; k++)
					{
						int num3 = GetNumberOfCells();
						for (int l = 0; l < num3; l++)
						{
							rectangle.X += subColumnSizes[k, l];
						}
						if (k < itemColumns - 1)
						{
							DrawSeparator(chartGraph, ItemColumnSeparator, ItemColumnSeparatorColor, horizontal: false, rectangle);
						}
						rectangle.X += rectangle.Width;
					}
				}
			}
			if (legendItemsTruncated && legendItemsAreaPosition.Height > truncatedDotsSize / 2)
			{
				chartGraph.StartAnimation();
				int num4 = 3;
				int val = legendItemsAreaPosition.Width / 3 / num4;
				val = Math.Min(val, 10);
				PointF absolute = new PointF((float)(legendItemsAreaPosition.X + legendItemsAreaPosition.Width / 2) - (float)val * (float)Math.Floor((float)num4 / 2f), legendItemsAreaPosition.Bottom + (truncatedDotsSize + offset.Height) / 2);
				for (int m = 0; m < num4; m++)
				{
					chartGraph.DrawMarkerRel(chartGraph.GetRelativePoint(absolute), MarkerStyle.Circle, truncatedDotsSize, FontColor, Color.Empty, 0, string.Empty, Color.Empty, 0, Color.Empty, RectangleF.Empty);
					absolute.X += val;
				}
				chartGraph.StopAnimation();
			}
			if (base.Common.ProcessModePaint)
			{
				base.Common.EventsManager.OnPaint(this, new ChartPaintEventArgs(chartGraph, base.Common, Position));
			}
			base.Common.TraceWrite("ChartPainting", SR.TraceMessageEndDrawingChartLegend);
			foreach (LegendItem legendItem4 in legendItems)
			{
				if (legendItem4.clearTempCells)
				{
					legendItem4.clearTempCells = false;
					legendItem4.Cells.Clear();
				}
			}
		}

		private Size GetTitleSize(ChartGraphics chartGraph, Size titleMaxSize)
		{
			Size result = Size.Empty;
			if (Title.Length > 0)
			{
				titleMaxSize.Width -= GetBorderSize() * 2 + offset.Width;
				result = chartGraph.MeasureStringAbs(Title.Replace("\\n", "\n"), TitleFont, titleMaxSize, new StringFormat());
				result.Height += offset.Height;
				result.Width += offset.Width;
				result.Height += GetSeparatorSize(chartGraph, TitleSeparator).Height;
			}
			return result;
		}

		private Size GetHeaderSize(ChartGraphics chartGraph, LegendCellColumn legendColumn)
		{
			Size result = Size.Empty;
			if (legendColumn.HeaderText.Length > 0)
			{
				result = chartGraph.MeasureStringAbs(legendColumn.HeaderText.Replace("\\n", "\n") + "I", legendColumn.HeaderFont);
				result.Height += offset.Height;
				result.Width += offset.Width;
				result.Height += GetSeparatorSize(chartGraph, HeaderSeparator).Height;
			}
			return result;
		}

		private void DrawLegendHeader(ChartGraphics chartGraph)
		{
			if (headerPosition.IsEmpty || headerPosition.Width <= 0 || headerPosition.Height <= 0)
			{
				return;
			}
			int num = -1;
			bool flag = false;
			Rectangle rect = Rectangle.Round(chartGraph.GetAbsoluteRectangle(Position.ToRectangleF()));
			rect.Y += GetBorderSize();
			rect.Height -= 2 * (offset.Height + GetBorderSize());
			rect.X += GetBorderSize();
			rect.Width -= 2 * GetBorderSize();
			if (GetBorderSize() > 0)
			{
				rect.Height++;
				rect.Width++;
			}
			bool flag2 = false;
			for (int i = 0; i < CellColumns.Count; i++)
			{
				if (!CellColumns[i].HeaderBackColor.IsEmpty)
				{
					flag2 = true;
				}
			}
			for (int j = 0; j < itemColumns; j++)
			{
				int num2 = 0;
				int num3 = 0;
				int length = subColumnSizes.GetLength(1);
				for (int k = 0; k < length; k++)
				{
					Rectangle rectangle = headerPosition;
					if (horizontalSpaceLeft > 0)
					{
						rectangle.X += (int)((float)horizontalSpaceLeft / 2f);
					}
					if (num != -1)
					{
						rectangle.X = num;
					}
					rectangle.Width = subColumnSizes[j, k];
					num = rectangle.Right;
					if (k == 0)
					{
						num2 = rectangle.Left;
					}
					num3 += rectangle.Width;
					rectangle.Intersect(rect);
					if (rectangle.Width <= 0 || rectangle.Height <= 0)
					{
						continue;
					}
					Rectangle r = rectangle;
					if (titlePosition.Height <= 0)
					{
						r.Y -= offset.Height;
						r.Height += offset.Height;
					}
					if ((itemColumns == 1 && flag2) || ItemColumnSeparator != 0)
					{
						if (j == 0 && k == 0)
						{
							int x = rect.X;
							num3 += num2 - x;
							num2 = x;
							r.Width += r.X - rect.X;
							r.X = x;
						}
						if (j == itemColumns - 1 && k == length - 1)
						{
							num3 += rect.Right - r.Right + 1;
							r.Width += rect.Right - r.Right + 1;
						}
						if (j != 0 && k == 0)
						{
							num3 += itemColumnSpacingRel / 2;
							num2 -= itemColumnSpacingRel / 2;
							r.Width += itemColumnSpacingRel / 2;
							r.X -= itemColumnSpacingRel / 2;
						}
						if (j != itemColumns - 1 && k == length - 1)
						{
							num3 += itemColumnSpacingRel / 2;
							r.Width += itemColumnSpacingRel / 2;
						}
					}
					if (k >= CellColumns.Count)
					{
						continue;
					}
					LegendCellColumn legendCellColumn = CellColumns[k];
					if (!legendCellColumn.HeaderBackColor.IsEmpty)
					{
						flag = true;
						chartGraph.StartAnimation();
						if (r.Right > rect.Right)
						{
							r.Width -= rect.Right - r.Right;
						}
						if (r.X < rect.X)
						{
							r.X += rect.X - r.X;
							r.Width -= rect.X - r.X;
						}
						r.Intersect(rect);
						chartGraph.FillRectangleRel(chartGraph.GetRelativeRectangle(r), legendCellColumn.HeaderBackColor, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, Color.Empty, 0, ChartDashStyle.NotSet, Color.Empty, 0, PenAlignment.Inset);
						chartGraph.StopAnimation();
					}
					using (SolidBrush brush = new SolidBrush(legendCellColumn.HeaderColor))
					{
						StringFormat stringFormat = new StringFormat();
						stringFormat.Alignment = legendCellColumn.HeaderTextAlignment;
						stringFormat.LineAlignment = StringAlignment.Center;
						stringFormat.FormatFlags = StringFormatFlags.LineLimit;
						stringFormat.Trimming = StringTrimming.EllipsisCharacter;
						chartGraph.StartAnimation();
						chartGraph.DrawStringRel(legendCellColumn.HeaderText, legendCellColumn.HeaderFont, brush, chartGraph.GetRelativeRectangle(rectangle), stringFormat);
						chartGraph.StopAnimation();
					}
				}
				Rectangle rectangle2 = headerPosition;
				rectangle2.X = num2;
				rectangle2.Width = num3;
				if (HeaderSeparator == LegendSeparatorType.Line || HeaderSeparator == LegendSeparatorType.DoubleLine)
				{
					rect.Width--;
				}
				rectangle2.Intersect(rect);
				DrawSeparator(chartGraph, HeaderSeparator, HeaderSeparatorColor, horizontal: true, rectangle2);
				num += GetSeparatorSize(chartGraph, ItemColumnSeparator).Width;
			}
			if (flag)
			{
				chartGraph.StartAnimation();
				chartGraph.FillRectangleRel(chartGraph.GetRelativeRectangle(Rectangle.Round(chartGraph.GetAbsoluteRectangle(Position.ToRectangleF()))), Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, BorderColor, GetBorderSize(), BorderStyle, Color.Empty, 0, PenAlignment.Inset);
				chartGraph.StopAnimation();
			}
			if (base.Common.ProcessModeRegions && !headerPosition.IsEmpty)
			{
				base.Common.HotRegionsList.AddHotRegion(chartGraph.GetRelativeRectangle(headerPosition), this, ChartElementType.LegendHeader, relativeCoordinates: true);
			}
		}

		private void DrawLegendTitle(ChartGraphics chartGraph)
		{
			if (Title.Length > 0 && !titlePosition.IsEmpty)
			{
				Rectangle rect = Rectangle.Round(chartGraph.GetAbsoluteRectangle(Position.ToRectangleF()));
				rect.Y += GetBorderSize();
				rect.Height -= 2 * GetBorderSize();
				rect.X += GetBorderSize();
				rect.Width -= 2 * GetBorderSize();
				if (GetBorderSize() > 0)
				{
					rect.Height++;
					rect.Width++;
				}
				if (!TitleBackColor.IsEmpty)
				{
					chartGraph.StartAnimation();
					Rectangle r = titlePosition;
					r.Intersect(rect);
					chartGraph.FillRectangleRel(chartGraph.GetRelativeRectangle(r), TitleBackColor, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, Color.Empty, 0, ChartDashStyle.NotSet, Color.Empty, 0, PenAlignment.Inset);
					chartGraph.StopAnimation();
				}
				using (SolidBrush brush = new SolidBrush(TitleColor))
				{
					StringFormat stringFormat = new StringFormat();
					stringFormat.Alignment = TitleAlignment;
					Rectangle r2 = titlePosition;
					r2.Y += offset.Height;
					r2.X += offset.Width;
					r2.X += GetBorderSize();
					r2.Width -= GetBorderSize() * 2 + offset.Width;
					chartGraph.StartAnimation();
					r2.Intersect(rect);
					chartGraph.DrawStringRel(Title.Replace("\\n", "\n"), TitleFont, brush, chartGraph.GetRelativeRectangle(r2), stringFormat);
					chartGraph.StopAnimation();
				}
				Rectangle rectangle = titlePosition;
				if (TitleSeparator == LegendSeparatorType.Line || TitleSeparator == LegendSeparatorType.DoubleLine)
				{
					rect.Width--;
				}
				rectangle.Intersect(rect);
				DrawSeparator(chartGraph, TitleSeparator, TitleSeparatorColor, horizontal: true, rectangle);
				if (!TitleBackColor.IsEmpty || TitleSeparator != 0)
				{
					chartGraph.StartAnimation();
					chartGraph.FillRectangleRel(chartGraph.GetRelativeRectangle(Rectangle.Round(chartGraph.GetAbsoluteRectangle(Position.ToRectangleF()))), Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, BorderColor, GetBorderSize(), BorderStyle, Color.Empty, 0, PenAlignment.Inset);
					chartGraph.StopAnimation();
				}
			}
		}

		internal Size GetSeparatorSize(ChartGraphics chartGraph, LegendSeparatorType separatorType)
		{
			Size empty = Size.Empty;
			switch (separatorType)
			{
			case LegendSeparatorType.None:
				empty = Size.Empty;
				break;
			case LegendSeparatorType.Line:
				empty = new Size(1, 1);
				break;
			case LegendSeparatorType.DashLine:
				empty = new Size(1, 1);
				break;
			case LegendSeparatorType.DotLine:
				empty = new Size(1, 1);
				break;
			case LegendSeparatorType.ThickLine:
				empty = new Size(2, 2);
				break;
			case LegendSeparatorType.DoubleLine:
				empty = new Size(3, 3);
				break;
			case LegendSeparatorType.GradientLine:
				empty = new Size(1, 1);
				break;
			case LegendSeparatorType.ThickGradientLine:
				empty = new Size(2, 2);
				break;
			default:
				throw new InvalidOperationException(SR.ExceptionLegendSeparatorTypeUnknown(separatorType.ToString()));
			}
			empty.Width += itemColumnSpacingRel;
			return empty;
		}

		private void DrawSeparator(ChartGraphics chartGraph, LegendSeparatorType separatorType, Color color, bool horizontal, Rectangle position)
		{
			SmoothingMode smoothingMode = chartGraph.SmoothingMode;
			chartGraph.SmoothingMode = SmoothingMode.None;
			RectangleF rectangleF = position;
			if (!horizontal)
			{
				rectangleF.X += (int)((float)itemColumnSpacingRel / 2f);
				rectangleF.Width -= itemColumnSpacingRel;
			}
			chartGraph.StartAnimation();
			switch (separatorType)
			{
			case LegendSeparatorType.Line:
				if (horizontal)
				{
					chartGraph.DrawLineAbs(color, 1, ChartDashStyle.Solid, new PointF(rectangleF.Left, rectangleF.Bottom - 1f), new PointF(rectangleF.Right, rectangleF.Bottom - 1f));
				}
				else
				{
					chartGraph.DrawLineAbs(color, 1, ChartDashStyle.Solid, new PointF(rectangleF.Right - 1f, rectangleF.Top), new PointF(rectangleF.Right - 1f, rectangleF.Bottom));
				}
				break;
			case LegendSeparatorType.DashLine:
				if (horizontal)
				{
					chartGraph.DrawLineAbs(color, 1, ChartDashStyle.Dash, new PointF(rectangleF.Left, rectangleF.Bottom - 1f), new PointF(rectangleF.Right, rectangleF.Bottom - 1f));
				}
				else
				{
					chartGraph.DrawLineAbs(color, 1, ChartDashStyle.Dash, new PointF(rectangleF.Right - 1f, rectangleF.Top), new PointF(rectangleF.Right - 1f, rectangleF.Bottom));
				}
				break;
			case LegendSeparatorType.DotLine:
				if (horizontal)
				{
					chartGraph.DrawLineAbs(color, 1, ChartDashStyle.Dot, new PointF(rectangleF.Left, rectangleF.Bottom - 1f), new PointF(rectangleF.Right, rectangleF.Bottom - 1f));
				}
				else
				{
					chartGraph.DrawLineAbs(color, 1, ChartDashStyle.Dot, new PointF(rectangleF.Right - 1f, rectangleF.Top), new PointF(rectangleF.Right - 1f, rectangleF.Bottom));
				}
				break;
			case LegendSeparatorType.ThickLine:
				if (horizontal)
				{
					chartGraph.DrawLineAbs(color, 2, ChartDashStyle.Solid, new PointF(rectangleF.Left, rectangleF.Bottom - 1f), new PointF(rectangleF.Right, rectangleF.Bottom - 1f));
				}
				else
				{
					chartGraph.DrawLineAbs(color, 2, ChartDashStyle.Solid, new PointF(rectangleF.Right - 1f, rectangleF.Top), new PointF(rectangleF.Right - 1f, rectangleF.Bottom));
				}
				break;
			case LegendSeparatorType.DoubleLine:
				if (horizontal)
				{
					chartGraph.DrawLineAbs(color, 1, ChartDashStyle.Solid, new PointF(rectangleF.Left, rectangleF.Bottom - 3f), new PointF(rectangleF.Right, rectangleF.Bottom - 3f));
					chartGraph.DrawLineAbs(color, 1, ChartDashStyle.Solid, new PointF(rectangleF.Left, rectangleF.Bottom - 1f), new PointF(rectangleF.Right, rectangleF.Bottom - 1f));
				}
				else
				{
					chartGraph.DrawLineAbs(color, 1, ChartDashStyle.Solid, new PointF(rectangleF.Right - 3f, rectangleF.Top), new PointF(rectangleF.Right - 3f, rectangleF.Bottom));
					chartGraph.DrawLineAbs(color, 1, ChartDashStyle.Solid, new PointF(rectangleF.Right - 1f, rectangleF.Top), new PointF(rectangleF.Right - 1f, rectangleF.Bottom));
				}
				break;
			case LegendSeparatorType.GradientLine:
				if (horizontal)
				{
					chartGraph.FillRectangleAbs(new RectangleF(rectangleF.Left, rectangleF.Bottom - 1f, rectangleF.Width, 0f), Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.VerticalCenter, color, Color.Empty, 0, ChartDashStyle.NotSet, PenAlignment.Inset);
				}
				else
				{
					chartGraph.FillRectangleAbs(new RectangleF(rectangleF.Right - 1f, rectangleF.Top, 0f, rectangleF.Height), Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.HorizontalCenter, color, Color.Empty, 0, ChartDashStyle.NotSet, PenAlignment.Inset);
				}
				break;
			case LegendSeparatorType.ThickGradientLine:
				if (horizontal)
				{
					chartGraph.FillRectangleAbs(new RectangleF(rectangleF.Left, rectangleF.Bottom - 2f, rectangleF.Width, 1f), Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.VerticalCenter, color, Color.Empty, 0, ChartDashStyle.NotSet, PenAlignment.Inset);
				}
				else
				{
					chartGraph.FillRectangleAbs(new RectangleF(rectangleF.Right - 2f, rectangleF.Top, 1f, rectangleF.Height), Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.HorizontalCenter, color, Color.Empty, 0, ChartDashStyle.NotSet, PenAlignment.Inset);
				}
				break;
			}
			chartGraph.StopAnimation();
			chartGraph.SmoothingMode = smoothingMode;
		}

		private int GetBorderSize()
		{
			if (BorderWidth > 0 && BorderStyle != 0 && !BorderColor.IsEmpty && BorderColor != Color.Transparent)
			{
				return BorderWidth;
			}
			return 0;
		}

		private LegendTableStyle GetLegendTableStyle(ChartGraphics chartGraph)
		{
			LegendTableStyle tableStyle = TableStyle;
			if (TableStyle == LegendTableStyle.Auto)
			{
				if (Position.Auto)
				{
					if (Docking == LegendDocking.Left || Docking == LegendDocking.Right)
					{
						return LegendTableStyle.Tall;
					}
					return LegendTableStyle.Wide;
				}
				SizeF size = chartGraph.GetAbsoluteRectangle(Position.ToRectangleF()).Size;
				if (size.Width < size.Height)
				{
					return LegendTableStyle.Tall;
				}
				return LegendTableStyle.Wide;
			}
			return tableStyle;
		}

		internal bool IsEnabled()
		{
			if (Enabled)
			{
				if (DockToChartArea.Length > 0 && base.Common != null && base.Common.ChartPicture != null && base.Common.ChartPicture.ChartAreas.GetIndex(DockToChartArea) >= 0 && !base.Common.ChartPicture.ChartAreas[DockToChartArea].Visible)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		internal void Invalidate(bool invalidateLegendOnly)
		{
		}

		internal void SelectLegendBackground(ChartGraphics chartGraph)
		{
			base.Common.HotRegionsList.AddHotRegion(Position.ToRectangleF(), this, ChartElementType.LegendArea, relativeCoordinates: true);
		}
	}
}

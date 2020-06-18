using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	[Description("Legend style, position, custom elements and other properties.")]
	[DefaultProperty("Enabled")]
	[TypeConverter(typeof(LegendConverter))]
	internal class Legend : AutoSizePanel, IToolTipProvider
	{
		internal class LegendConverter : AutoSizePanelConverter
		{
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				if (destinationType == typeof(InstanceDescriptor))
				{
					return true;
				}
				return base.CanConvertTo(context, destinationType);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				if (destinationType == typeof(InstanceDescriptor))
				{
					return new InstanceDescriptor(typeof(Legend).GetConstructor(Type.EmptyTypes), null, isComplete: false);
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}

		private const int DummyItemsCount = 5;

		private LegendTableStyle legendTableStyle;

		private bool autoFitText = true;

		private TempLegendItemsCollection legendItems;

		private SizeF sizeLargestItemText = SizeF.Empty;

		private SizeF sizeAverageItemText = SizeF.Empty;

		private SizeF sizeItemImage = SizeF.Empty;

		private int itemColumns;

		private SizeF itemCellSize = SizeF.Empty;

		internal Font autofitFont;

		private bool interlacedRows;

		private Color interlacedRowsColor = Color.Empty;

		private Size offset = System.Drawing.Size.Empty;

		private int textWrapThreshold = 25;

		private int autoFitFontSizeAdjustment;

		private LegendCellColumnCollection cellColumns;

		private AutoBool reversed;

		private string title = "Legend Title";

		private Color titleColor = Color.Black;

		private Color titleBackColor = Color.Empty;

		private StringAlignment titleAlignment = StringAlignment.Center;

		private LegendSeparatorType titleSeparator = LegendSeparatorType.GradientLine;

		private Color titleSeparatorColor = Color.Gray;

		private LegendSeparatorType headerSeparator;

		private Color headerSeparatorColor = Color.Black;

		private LegendSeparatorType itemColumnSeparator;

		private Color itemColumnSeparatorColor = Color.DarkGray;

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

		internal Size singleWCharacterSize = System.Drawing.Size.Empty;

		private bool showSelectedTitle;

		private bool equallySpacedItems;

		private LegendStyle legendStyle = LegendStyle.Table;

		private Font font = new Font("Microsoft Sans Serif", 8f);

		private bool disposeFont = true;

		private Color textColor = Color.Black;

		private LegendItemsCollection customLegends;

		private Font titleFont = new Font("Microsoft Sans Serif", 9.75f);

		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		[Browsable(false)]
		internal bool ShowSelectedTitle
		{
			get
			{
				return showSelectedTitle;
			}
			set
			{
				showSelectedTitle = value;
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		[Browsable(false)]
		internal RectangleF TitleSelectionRectangle
		{
			get
			{
				RectangleF result = titlePosition;
				result.Offset(GetAbsoluteLocation());
				result.Inflate(-GetBorderSize(), -GetBorderSize());
				result.Inflate(-offset.Width, -offset.Height);
				return result;
			}
		}

		[Browsable(true)]
		[NotifyParentProperty(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[SerializationVisibility(SerializationVisibility.Attribute)]
		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeLegend_Name")]
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

		[SRCategory("CategoryAttribute_Behavior")]
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
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
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
						if (disposeFont)
						{
							this.font.Dispose();
						}
						this.font = font;
						disposeFont = true;
					}
					else
					{
						this.font = new Font("Microsoft Sans Serif", 8f);
						disposeFont = true;
					}
				}
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[Bindable(true)]
		[DefaultValue(LegendStyle.Table)]
		[SRDescription("DescriptionAttributeLegend_LegendStyle")]
		[NotifyParentProperty(true)]
		public LegendStyle LegendStyle
		{
			get
			{
				return legendStyle;
			}
			set
			{
				legendStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[Bindable(true)]
		[DefaultValue(LegendTableStyle.Auto)]
		[SRDescription("DescriptionAttributeLegend_TableStyle")]
		[NotifyParentProperty(true)]
		public LegendTableStyle TableStyle
		{
			get
			{
				return legendTableStyle;
			}
			set
			{
				legendTableStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
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
					throw new InvalidOperationException(SR.ExceptionAutoFitMinFontSizeMinValue);
				}
				autoFitMinFontSize = value;
				Invalidate();
			}
		}

		[DefaultValue(true)]
		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				base.Visible = value;
			}
		}

		[SRCategory("CategoryAttribute_CellColumns")]
		[SRDescription("DescriptionAttributeLegend_CellColumns")]
		public LegendCellColumnCollection CellColumns => cellColumns;

		[SRCategory("CategoryAttribute_CellColumns")]
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
					Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_CellColumns")]
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
					Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_CellColumns")]
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
					Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_CellColumns")]
		[DefaultValue(typeof(Color), "DarkGray")]
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
					Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_CellColumns")]
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
						throw new ArgumentOutOfRangeException("TableColumnSpacing", SR.ExceptionLegendTableColumnSpacingTooSmall);
					}
					itemColumnSpacing = value;
					Invalidate();
				}
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttribute_Appearance")]
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
				if (disposeFont)
				{
					font.Dispose();
				}
				font = value;
				disposeFont = false;
				Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLegend_TextColor")]
		[NotifyParentProperty(true)]
		public Color TextColor
		{
			get
			{
				return textColor;
			}
			set
			{
				textColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLegend_Items")]
		public LegendItemsCollection Items => customLegends;

		[SRCategory("CategoryAttribute_Behavior")]
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
						throw new ArgumentException(SR.ExceptionTextThresholdValueTooSmall, "TextWrapThreshold");
					}
					textWrapThreshold = value;
					Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(AutoBool.Auto)]
		[Description("Indicates that all legend items are shown in reversed order. This property only affects legend items automatically added and has no effect on custom legend items.")]
		private AutoBool Reversed
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
					Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
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
					Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
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
					Invalidate();
				}
			}
		}

		internal override bool IsEmpty
		{
			get
			{
				if (Common != null && Common.MapCore.IsDesignMode())
				{
					return false;
				}
				return Items.Count == 0;
			}
		}

		[SRCategory("CategoryAttribute_Title")]
		[DefaultValue("Legend Title")]
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
					Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_Title")]
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
					Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_Title")]
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
					Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_Title")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 9.75pt")]
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
					Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_Title")]
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
					Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_Title")]
		[DefaultValue(typeof(LegendSeparatorType), "GradientLine")]
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
					Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_Title")]
		[DefaultValue(typeof(Color), "Gray")]
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
					Invalidate();
				}
			}
		}

		internal override string DefaultName => "Legend";

		internal override CommonElements Common
		{
			get
			{
				return base.Common;
			}
			set
			{
				base.Common = value;
				if (Items != null)
				{
					Items.Common = value;
				}
				if (CellColumns != null)
				{
					CellColumns.Common = value;
				}
			}
		}

		public Legend()
			: this((CommonElements)null)
		{
		}

		internal Legend(CommonElements common)
			: base(common)
		{
			customLegends = new LegendItemsCollection(this, common);
			customLegends.Legend = this;
			legendItems = new TempLegendItemsCollection();
			cellColumns = new LegendCellColumnCollection(this, this, common);
			Visible = true;
		}

		public Legend(string name)
			: this((CommonElements)null)
		{
			Name = name;
		}

		public override RectangleF GetSelectionRectangle(MapGraphics g, RectangleF clipRect)
		{
			RectangleF result = base.GetSelectionRectangle(g, clipRect);
			if (ShowSelectedTitle && !TitleSelectionRectangle.IsEmpty)
			{
				result = TitleSelectionRectangle;
			}
			return result;
		}

		private void RecalcLegendInfo(MapGraphics g)
		{
			RectangleF relative = new RectangleF(0f, 0f, 100f, 100f);
			Rectangle rectangle = Rectangle.Round(g.GetAbsoluteRectangle(relative));
			offset.Width = (int)Math.Ceiling((float)singleWCharacterSize.Width / 2f);
			offset.Height = (int)Math.Ceiling((float)singleWCharacterSize.Width / 3f);
			itemColumnSpacingRel = (int)((float)singleWCharacterSize.Width * ((float)itemColumnSpacing / 100f));
			if (itemColumnSpacingRel % 2 == 1)
			{
				itemColumnSpacingRel++;
			}
			titlePosition = Rectangle.Empty;
			if (!string.IsNullOrEmpty(Title))
			{
				Size titleSize = GetTitleSize(g, rectangle.Size);
				titlePosition = new Rectangle(rectangle.Location.X, rectangle.Location.Y, rectangle.Width, Math.Min(rectangle.Height, titleSize.Height));
				rectangle.Height -= titlePosition.Height;
				titlePosition.Y += GetBorderSize();
			}
			headerPosition = Rectangle.Empty;
			Size empty = System.Drawing.Size.Empty;
			foreach (LegendCellColumn cellColumn in CellColumns)
			{
				if (!string.IsNullOrEmpty(cellColumn.HeaderText))
				{
					empty.Height = Math.Max(val2: GetHeaderSize(g, cellColumn).Height, val1: empty.Height);
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
			GetNumberOfRowsAndColumns(g, legendItemsAreaPosition.Size, -1, out numberOfRowsPerColumn, out itemColumns, out horizontalSpaceLeft, out verticalSpaceLeft);
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
			while (!flag && AutoFitText && Font.Size - (float)autoFitFontSizeAdjustment > (float)autoFitMinFontSize)
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
				GetNumberOfRowsAndColumns(g, legendItemsAreaPosition.Size, -1, out numberOfRowsPerColumn, out itemColumns, out horizontalSpaceLeft, out verticalSpaceLeft);
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
			}
			int num3 = 0;
			int num4 = numberOfLegendItemsToProcess;
			while (!flag)
			{
				if (numberOfLegendItemsToProcess <= 2)
				{
					flag = true;
					break;
				}
				if (num4 - num3 <= 1)
				{
					flag = true;
					if (numberOfLegendItemsToProcess != num3)
					{
						numberOfLegendItemsToProcess = num3;
						TryLayoutLegendItems(g);
					}
					break;
				}
				if (itemColumns == 1 && horizontalSpaceLeft < 0 && verticalSpaceLeft >= 0)
				{
					flag = true;
					numberOfLegendItemsToProcess = Math.Min(numberOfLegendItemsToProcess, numberOfRowsPerColumn[0]);
					break;
				}
				if (GetMaximumNumberOfRows() == 1 && verticalSpaceLeft < 0 && horizontalSpaceLeft >= 0)
				{
					flag = true;
					numberOfLegendItemsToProcess = Math.Min(numberOfLegendItemsToProcess, itemColumns);
					break;
				}
				if (!legendItemsTruncated)
				{
					legendItemsTruncated = true;
					legendItemsAreaPosition.Height -= truncatedDotsSize;
				}
				numberOfLegendItemsToProcess = (num4 + num3) / 2;
				if (TryLayoutLegendItems(g))
				{
					num3 = numberOfLegendItemsToProcess;
				}
				else
				{
					num4 = numberOfLegendItemsToProcess;
				}
			}
			Size empty2 = System.Drawing.Size.Empty;
			if (verticalSpaceLeft > 0)
			{
				empty2.Height = verticalSpaceLeft / GetMaximumNumberOfRows() / 2;
			}
			if (horizontalSpaceLeft > 0)
			{
				empty2.Width = horizontalSpaceLeft / 2;
			}
			int num5 = 0;
			int num6 = 0;
			if (numberOfLegendItemsToProcess < 0)
			{
				numberOfLegendItemsToProcess = legendItems.Count;
			}
			for (int k = 0; k < numberOfLegendItemsToProcess; k++)
			{
				LegendItem legendItem = legendItems[k];
				int num7;
				for (num7 = 0; num7 < legendItem.Cells.Count; num7++)
				{
					LegendCell legendCell = legendItem.Cells[num7];
					Rectangle cellPosition = GetCellPosition(g, num5, num6, num7, empty2);
					int num8 = 0;
					if (legendCell.CellSpan > 1)
					{
						for (int l = 1; l < legendCell.CellSpan && num7 + l < legendItem.Cells.Count; l++)
						{
							Rectangle cellPosition2 = GetCellPosition(g, num5, num6, num7 + l, empty2);
							if (cellPosition.Right < cellPosition2.Right)
							{
								cellPosition.Width += cellPosition2.Right - cellPosition.Right;
							}
							num8++;
							legendItem.Cells[num7 + l].SetCellPosition(g, num5, num6, Rectangle.Empty, autoFitFontSizeAdjustment, (autofitFont == null) ? Font : autofitFont, singleWCharacterSize);
						}
					}
					cellPosition.Intersect(legendItemsAreaPosition);
					legendCell.SetCellPosition(g, num5, num6, cellPosition, autoFitFontSizeAdjustment, (autofitFont == null) ? Font : autofitFont, singleWCharacterSize);
					num7 += num8;
				}
				num6++;
				if (num6 >= numberOfRowsPerColumn[num5])
				{
					num5++;
					num6 = 0;
					if (num5 >= itemColumns)
					{
						break;
					}
				}
			}
		}

		private bool TryLayoutLegendItems(MapGraphics g)
		{
			bool result = false;
			for (int i = 0; i < 2; i++)
			{
				result = GetNumberOfRowsAndColumns(g, legendItemsAreaPosition.Size, numberOfLegendItemsToProcess, out numberOfRowsPerColumn, out itemColumns, out horizontalSpaceLeft, out verticalSpaceLeft);
			}
			return result;
		}

		protected override SizeF CalculateUndockedAutoSize(SizeF size)
		{
			switch (LegendStyle)
			{
			case LegendStyle.Column:
				size.Width = size.Width * MaxAutoSize / 100f;
				break;
			case LegendStyle.Row:
				size.Height = size.Height * MaxAutoSize / 100f;
				break;
			case LegendStyle.Table:
				switch (TableStyle)
				{
				case LegendTableStyle.Tall:
					size.Width = size.Width * MaxAutoSize / 100f;
					break;
				case LegendTableStyle.Wide:
					size.Height = size.Height * MaxAutoSize / 100f;
					break;
				case LegendTableStyle.Auto:
					size = base.CalculateUndockedAutoSize(size);
					break;
				}
				break;
			}
			return size;
		}

		private Rectangle GetCellPosition(MapGraphics g, int columnIndex, int rowIndex, int cellIndex, Size itemHalfSpacing)
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
				result.X += GetSeparatorSize(g, ItemColumnSeparator).Width;
			}
			for (int l = 0; l < cellIndex; l++)
			{
				result.X += subColumnSizes[columnIndex, l];
			}
			result.Height = cellHeights[columnIndex, rowIndex];
			result.Width = subColumnSizes[columnIndex, cellIndex];
			return result;
		}

		internal override SizeF GetOptimalSize(MapGraphics g, SizeF maxSizeAbs)
		{
			offset = System.Drawing.Size.Empty;
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
			Size empty = System.Drawing.Size.Empty;
			Size size = new Size((int)maxSizeAbs.Width, (int)maxSizeAbs.Height);
			foreach (LegendItem legendItem in legendItems)
			{
				foreach (LegendCell cell in legendItem.Cells)
				{
					cell.ResetCache();
				}
			}
			if (IsVisible())
			{
				singleWCharacterSize = g.MeasureStringAbs("W", Font);
				Size size2 = g.MeasureStringAbs("WW", Font);
				singleWCharacterSize.Width = size2.Width - singleWCharacterSize.Width;
				FillLegendItemsCollection(singleWCharacterSize);
				if (legendItems.Count > 0)
				{
					offset.Width = (int)Math.Ceiling((float)singleWCharacterSize.Width / 2f);
					offset.Height = (int)Math.Ceiling((float)singleWCharacterSize.Width / 3f);
					itemColumnSpacingRel = (int)((float)singleWCharacterSize.Width * ((float)itemColumnSpacing / 100f));
					if (itemColumnSpacingRel % 2 == 1)
					{
						itemColumnSpacingRel++;
					}
					Size size3 = System.Drawing.Size.Empty;
					if (!string.IsNullOrEmpty(Title))
					{
						size3 = GetTitleSize(g, size);
					}
					Size empty2 = System.Drawing.Size.Empty;
					foreach (LegendCellColumn cellColumn in CellColumns)
					{
						if (!string.IsNullOrEmpty(cellColumn.HeaderText))
						{
							empty2.Height = Math.Max(val2: GetHeaderSize(g, cellColumn).Height, val1: empty2.Height);
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
						GetNumberOfRowsAndColumns(g, legendSize, -1, out numberOfRowsPerColumn, out itemColumns, out horSpaceLeft, out vertSpaceLeft);
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
					empty.Width += 2 * BorderWidth;
					empty.Height += 2 * BorderWidth;
					if (empty.Width > size.Width)
					{
						empty.Width = size.Width;
					}
					if (empty.Height > size.Height)
					{
						empty.Height = size.Height;
					}
					if (empty.Width < 2)
					{
						empty.Width = 2;
					}
					if (empty.Height < 2)
					{
						empty.Height = 2;
					}
				}
			}
			return empty;
		}

		private bool GetNumberOfRowsAndColumns(MapGraphics g, Size legendSize, int numberOfItemsToCheck, out int[] numberOfRowsPerColumn, out int columnNumber)
		{
			int horSpaceLeft = 0;
			int vertSpaceLeft = 0;
			return GetNumberOfRowsAndColumns(g, legendSize, numberOfItemsToCheck, out numberOfRowsPerColumn, out columnNumber, out horSpaceLeft, out vertSpaceLeft);
		}

		private bool GetNumberOfRowsAndColumns(MapGraphics g, Size legendSize, int numberOfItemsToCheck, out int[] numberOfRowsPerColumn, out int columnNumber, out int horSpaceLeft, out int vertSpaceLeft)
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
				switch (GetLegendTableStyle(g))
				{
				case LegendTableStyle.Tall:
				{
					bool flag4 = false;
					int num6 = 1;
					num6 = 1;
					while (!flag4 && num6 < numberOfItemsToCheck)
					{
						numberOfRowsPerColumn[columnNumber - 1]++;
						if (!CheckLegendItemsFit(g, legendSize, num6 + 1, autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out subColumnSizes, out cellHeights, out horSpaceLeft, out vertSpaceLeft) && ((columnNumber != 1 && horSpaceLeft >= 0) || vertSpaceLeft <= 0))
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
									CheckLegendItemsFit(g, legendSize, num6 + 1, autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out subColumnSizes, out cellHeights, out horSpaceLeft, out vertSpaceLeft);
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
							CheckLegendItemsFit(g, legendSize, num6 + 1, autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out subColumnSizes, out cellHeights, out horSpaceLeft, out vertSpaceLeft);
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
						bool flag2 = CheckLegendItemsFit(g, legendSize, num + 1, autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out subColumnSizes, out cellHeights, out horSpaceLeft, out vertSpaceLeft);
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
										flag2 = CheckLegendItemsFit(g, legendSize, num + 1, autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out subColumnSizes, out cellHeights, out horSpaceLeft, out vertSpaceLeft);
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
									flag2 = CheckLegendItemsFit(g, legendSize, num + 1, autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out subColumnSizes, out cellHeights, out horSpaceLeft, out vertSpaceLeft);
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
			return CheckLegendItemsFit(g, legendSize, numberOfItemsToCheck, autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out subColumnSizes, out cellHeights, out horSpaceLeft, out vertSpaceLeft);
		}

		private int GetHighestColumnIndex()
		{
			int result = 0;
			int num = -1;
			for (int i = 0; i < itemColumns; i++)
			{
				int num2 = 0;
				for (int j = 0; j < numberOfRowsPerColumn[i]; j++)
				{
					num2 += cellHeights[i, j];
				}
				if (num2 > num)
				{
					num = num2;
					result = i;
				}
			}
			return result;
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

		private bool CheckLegendItemsFit(MapGraphics graph, Size legendItemsAreaSize, int numberOfItemsToCheck, int fontSizeReducedBy, int numberOfColumns, int[] numberOfRowsPerColumn, out int[,] subColumnSizes, out int[,] cellHeights, out int horizontalSpaceLeft, out int verticalSpaceLeft)
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
					if (string.IsNullOrEmpty(legendCellColumn2.HeaderText))
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

		private void FillLegendItemsCollection(SizeF singleWCharacterSize)
		{
			legendItems.Clear();
			foreach (LegendItem customLegend in customLegends)
			{
				if (customLegend.Visible)
				{
					legendItems.Add(customLegend);
				}
			}
			if (legendItems.Count == 0 && Common != null && Common.MapControl != null && Common.MapControl.IsDesignMode())
			{
				Color[] array = new ColorGenerator().GenerateColors(MapColorPalette.Light, 5);
				for (int i = 0; i < array.Length; i++)
				{
					LegendItem legendItem2 = new LegendItem(string.Format(CultureInfo.CurrentCulture, "LegendItem{0}", i + 1), array[i], "");
					legendItem2.Legend = this;
					legendItem2.Common = Common;
					legendItem2.ItemStyle = LegendItemStyle.Shape;
					legendItems.Add(legendItem2);
				}
			}
			foreach (LegendItem legendItem3 in legendItems)
			{
				legendItem3.AddAutomaticCells(this, singleWCharacterSize);
			}
		}

		internal override void Render(MapGraphics g)
		{
			base.Render(g);
			RenderLegend(g);
		}

		private void RenderLegend(MapGraphics g)
		{
			offset = System.Drawing.Size.Empty;
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
			RectangleF relative = new RectangleF(0f, 0f, 100f, 100f);
			singleWCharacterSize = g.MeasureStringAbs("W", Font);
			Size size = g.MeasureStringAbs("WW", Font);
			singleWCharacterSize.Width = size.Width - singleWCharacterSize.Width;
			FillLegendItemsCollection(singleWCharacterSize);
			foreach (LegendItem legendItem3 in legendItems)
			{
				foreach (LegendCell cell in legendItem3.Cells)
				{
					cell.ResetCache();
				}
			}
			if (legendItems.Count == 0)
			{
				return;
			}
			RecalcLegendInfo(g);
			DrawLegendHeader(g);
			DrawLegendTitle(g);
			if (numberOfLegendItemsToProcess < 0)
			{
				numberOfLegendItemsToProcess = legendItems.Count;
			}
			for (int i = 0; i < numberOfLegendItemsToProcess; i++)
			{
				LegendItem legendItem = legendItems[i];
				for (int j = 0; j < legendItem.Cells.Count; j++)
				{
					legendItem.Cells[j].Paint(g, autoFitFontSizeAdjustment, autofitFont, singleWCharacterSize);
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
				empty.Height = GetSeparatorSize(g, legendItem.Separator).Height;
				empty.Intersect(legendItemsAreaPosition);
				DrawSeparator(g, legendItem.Separator, legendItem.SeparatorColor, horizontal: true, empty);
			}
			if (ItemColumnSeparator != 0)
			{
				Rectangle position = Rectangle.Round(g.GetAbsoluteRectangle(relative));
				position.Y += GetBorderSize() + titlePosition.Height;
				position.Height -= 2 * GetBorderSize() + titlePosition.Height;
				position.X += GetBorderSize() + offset.Width;
				position.Width = GetSeparatorSize(g, ItemColumnSeparator).Width;
				if (horizontalSpaceLeft > 0)
				{
					position.X += horizontalSpaceLeft / 2;
				}
				if (position.Width > 0 && position.Height > 0)
				{
					for (int k = 0; k < itemColumns; k++)
					{
						int num3 = GetNumberOfCells();
						for (int l = 0; l < num3; l++)
						{
							position.X += subColumnSizes[k, l];
						}
						if (k < itemColumns - 1)
						{
							DrawSeparator(g, ItemColumnSeparator, ItemColumnSeparatorColor, horizontal: false, position);
						}
						position.X += position.Width;
					}
				}
			}
			if (legendItemsTruncated && legendItemsAreaPosition.Height > truncatedDotsSize / 2)
			{
				int num4 = 3;
				int val = legendItemsAreaPosition.Width / 3 / num4;
				val = Math.Min(val, 10);
				PointF absolute = new PointF((float)(legendItemsAreaPosition.X + legendItemsAreaPosition.Width / 2) - (float)val * (float)Math.Floor((float)num4 / 2f), legendItemsAreaPosition.Bottom + (truncatedDotsSize + offset.Height) / 2);
				for (int m = 0; m < num4; m++)
				{
					g.DrawMarkerRel(g.GetRelativePoint(absolute), MarkerStyle.Circle, truncatedDotsSize, TextColor, GradientType.None, MapHatchStyle.None, Color.Empty, MapDashStyle.Solid, Color.Empty, 0, string.Empty, Color.Empty, 0, Color.Empty, RectangleF.Empty);
					absolute.X += val;
				}
			}
			_ = Common.ProcessModePaint;
			foreach (LegendItem legendItem4 in legendItems)
			{
				if (legendItem4.clearTempCells)
				{
					legendItem4.clearTempCells = false;
					legendItem4.Cells.Clear();
				}
			}
		}

		private Size GetTitleSize(MapGraphics chartGraph, Size titleMaxSize)
		{
			Size result = System.Drawing.Size.Empty;
			if (!string.IsNullOrEmpty(Title))
			{
				titleMaxSize.Width -= GetBorderSize() * 2 + offset.Width;
				result = chartGraph.MeasureStringAbs(Title.Replace("\\n", "\n"), TitleFont, titleMaxSize, new StringFormat());
				result.Height += offset.Height;
				result.Width += offset.Width;
				result.Height += GetSeparatorSize(chartGraph, TitleSeparator).Height;
			}
			return result;
		}

		private Size GetHeaderSize(MapGraphics chartGraph, LegendCellColumn legendColumn)
		{
			Size result = System.Drawing.Size.Empty;
			if (!string.IsNullOrEmpty(legendColumn.HeaderText))
			{
				result = chartGraph.MeasureStringAbs(legendColumn.HeaderText.Replace("\\n", "\n") + "I", legendColumn.HeaderFont);
				result.Height += offset.Height;
				result.Width += offset.Width;
				result.Height += GetSeparatorSize(chartGraph, HeaderSeparator).Height;
			}
			return result;
		}

		private void DrawLegendHeader(MapGraphics g)
		{
			if (headerPosition.IsEmpty || headerPosition.Width <= 0 || headerPosition.Height <= 0)
			{
				return;
			}
			int num = -1;
			RectangleF relative = new RectangleF(0f, 0f, 100f, 100f);
			Rectangle rect = Rectangle.Round(g.GetAbsoluteRectangle(relative));
			rect.Y += GetBorderSize();
			rect.Height -= 2 * (offset.Height + GetBorderSize());
			rect.X += GetBorderSize();
			rect.Width -= 2 * GetBorderSize();
			if (GetBorderSize() > 0)
			{
				rect.Height++;
				rect.Width++;
			}
			bool flag = false;
			for (int i = 0; i < CellColumns.Count; i++)
			{
				if (!CellColumns[i].HeaderBackColor.IsEmpty)
				{
					flag = true;
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
					if ((itemColumns == 1 && flag) || ItemColumnSeparator != 0)
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
						g.FillRectangleRel(g.GetRelativeRectangle(r), legendCellColumn.HeaderBackColor, MapHatchStyle.None, string.Empty, MapImageWrapMode.Tile, Color.Empty, MapImageAlign.Center, GradientType.None, Color.Empty, Color.Empty, 0, MapDashStyle.None, Color.Empty, 0, PenAlignment.Inset);
					}
					using (SolidBrush brush = new SolidBrush(legendCellColumn.HeaderColor))
					{
						StringFormat stringFormat = new StringFormat();
						stringFormat.Alignment = legendCellColumn.HeaderTextAlignment;
						stringFormat.LineAlignment = StringAlignment.Center;
						stringFormat.FormatFlags = StringFormatFlags.LineLimit;
						stringFormat.Trimming = StringTrimming.EllipsisCharacter;
						g.DrawStringRel(legendCellColumn.HeaderText, legendCellColumn.HeaderFont, brush, g.GetRelativeRectangle(rectangle), stringFormat);
					}
				}
				Rectangle position = headerPosition;
				position.X = num2;
				position.Width = num3;
				if (HeaderSeparator == LegendSeparatorType.Line || HeaderSeparator == LegendSeparatorType.DoubleLine)
				{
					rect.Width--;
				}
				position.Intersect(rect);
				DrawSeparator(g, HeaderSeparator, HeaderSeparatorColor, horizontal: true, position);
				num += GetSeparatorSize(g, ItemColumnSeparator).Width;
			}
		}

		private void DrawLegendTitle(MapGraphics g)
		{
			if (!string.IsNullOrEmpty(Title) && !titlePosition.IsEmpty)
			{
				RectangleF relative = new RectangleF(0f, 0f, 100f, 100f);
				Rectangle rect = Rectangle.Round(g.GetAbsoluteRectangle(relative));
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
					Rectangle r = titlePosition;
					r.Intersect(rect);
					g.FillRectangleRel(g.GetRelativeRectangle(r), TitleBackColor, MapHatchStyle.None, string.Empty, MapImageWrapMode.Tile, Color.Empty, MapImageAlign.Center, GradientType.None, Color.Empty, Color.Empty, 0, MapDashStyle.None, Color.Empty, 0, PenAlignment.Inset);
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
					r2.Intersect(rect);
					g.DrawStringRel(Title.Replace("\\n", "\n"), TitleFont, brush, g.GetRelativeRectangle(r2), stringFormat);
				}
				Rectangle position = titlePosition;
				if (TitleSeparator == LegendSeparatorType.Line || TitleSeparator == LegendSeparatorType.DoubleLine)
				{
					rect.Width--;
				}
				position.Intersect(rect);
				DrawSeparator(g, TitleSeparator, TitleSeparatorColor, horizontal: true, position);
			}
		}

		internal Size GetSeparatorSize(MapGraphics chartGraph, LegendSeparatorType separatorType)
		{
			Size empty = System.Drawing.Size.Empty;
			switch (separatorType)
			{
			case LegendSeparatorType.None:
				empty = System.Drawing.Size.Empty;
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
				throw new InvalidOperationException("Unknown legend separator type '" + separatorType.ToString(CultureInfo.CurrentCulture) + "'.");
			}
			empty.Width += itemColumnSpacingRel;
			return empty;
		}

		private void DrawSeparator(MapGraphics chartGraph, LegendSeparatorType separatorType, Color color, bool horizontal, Rectangle position)
		{
			SmoothingMode smoothingMode = chartGraph.SmoothingMode;
			chartGraph.SmoothingMode = SmoothingMode.None;
			RectangleF rectangleF = position;
			if (!horizontal)
			{
				rectangleF.X += (int)((float)itemColumnSpacingRel / 2f);
				rectangleF.Width -= itemColumnSpacingRel;
			}
			switch (separatorType)
			{
			case LegendSeparatorType.Line:
				if (horizontal)
				{
					chartGraph.DrawLineAbs(color, 1, MapDashStyle.Solid, new PointF(rectangleF.Left, rectangleF.Bottom - 1f), new PointF(rectangleF.Right, rectangleF.Bottom - 1f));
				}
				else
				{
					chartGraph.DrawLineAbs(color, 1, MapDashStyle.Solid, new PointF(rectangleF.Right - 1f, rectangleF.Top), new PointF(rectangleF.Right - 1f, rectangleF.Bottom));
				}
				break;
			case LegendSeparatorType.DashLine:
				if (horizontal)
				{
					chartGraph.DrawLineAbs(color, 1, MapDashStyle.Dash, new PointF(rectangleF.Left, rectangleF.Bottom - 1f), new PointF(rectangleF.Right, rectangleF.Bottom - 1f));
				}
				else
				{
					chartGraph.DrawLineAbs(color, 1, MapDashStyle.Dash, new PointF(rectangleF.Right - 1f, rectangleF.Top), new PointF(rectangleF.Right - 1f, rectangleF.Bottom));
				}
				break;
			case LegendSeparatorType.DotLine:
				if (horizontal)
				{
					chartGraph.DrawLineAbs(color, 1, MapDashStyle.Dot, new PointF(rectangleF.Left, rectangleF.Bottom - 1f), new PointF(rectangleF.Right, rectangleF.Bottom - 1f));
				}
				else
				{
					chartGraph.DrawLineAbs(color, 1, MapDashStyle.Dot, new PointF(rectangleF.Right - 1f, rectangleF.Top), new PointF(rectangleF.Right - 1f, rectangleF.Bottom));
				}
				break;
			case LegendSeparatorType.ThickLine:
				if (horizontal)
				{
					chartGraph.DrawLineAbs(color, 2, MapDashStyle.Solid, new PointF(rectangleF.Left, rectangleF.Bottom - 1f), new PointF(rectangleF.Right, rectangleF.Bottom - 1f));
				}
				else
				{
					chartGraph.DrawLineAbs(color, 2, MapDashStyle.Solid, new PointF(rectangleF.Right - 1f, rectangleF.Top), new PointF(rectangleF.Right - 1f, rectangleF.Bottom));
				}
				break;
			case LegendSeparatorType.DoubleLine:
				if (horizontal)
				{
					chartGraph.DrawLineAbs(color, 1, MapDashStyle.Solid, new PointF(rectangleF.Left, rectangleF.Bottom - 3f), new PointF(rectangleF.Right, rectangleF.Bottom - 3f));
					chartGraph.DrawLineAbs(color, 1, MapDashStyle.Solid, new PointF(rectangleF.Left, rectangleF.Bottom - 1f), new PointF(rectangleF.Right, rectangleF.Bottom - 1f));
				}
				else
				{
					chartGraph.DrawLineAbs(color, 1, MapDashStyle.Solid, new PointF(rectangleF.Right - 3f, rectangleF.Top), new PointF(rectangleF.Right - 3f, rectangleF.Bottom));
					chartGraph.DrawLineAbs(color, 1, MapDashStyle.Solid, new PointF(rectangleF.Right - 1f, rectangleF.Top), new PointF(rectangleF.Right - 1f, rectangleF.Bottom));
				}
				break;
			case LegendSeparatorType.GradientLine:
				if (horizontal)
				{
					chartGraph.FillRectangleAbs(new RectangleF(rectangleF.Left, rectangleF.Bottom - 1f, rectangleF.Width, 0f), Color.Transparent, MapHatchStyle.None, string.Empty, MapImageWrapMode.Tile, Color.Empty, MapImageAlign.Center, GradientType.VerticalCenter, color, Color.Empty, 0, MapDashStyle.None, PenAlignment.Inset);
				}
				else
				{
					chartGraph.FillRectangleAbs(new RectangleF(rectangleF.Right - 1f, rectangleF.Top, 0f, rectangleF.Height), Color.Transparent, MapHatchStyle.None, string.Empty, MapImageWrapMode.Tile, Color.Empty, MapImageAlign.Center, GradientType.HorizontalCenter, color, Color.Empty, 0, MapDashStyle.None, PenAlignment.Inset);
				}
				break;
			case LegendSeparatorType.ThickGradientLine:
				if (horizontal)
				{
					chartGraph.FillRectangleAbs(new RectangleF(rectangleF.Left, rectangleF.Bottom - 2f, rectangleF.Width, 1f), Color.Transparent, MapHatchStyle.None, string.Empty, MapImageWrapMode.Tile, Color.Empty, MapImageAlign.Center, GradientType.VerticalCenter, color, Color.Empty, 0, MapDashStyle.None, PenAlignment.Inset);
				}
				else
				{
					chartGraph.FillRectangleAbs(new RectangleF(rectangleF.Right - 2f, rectangleF.Top, 1f, rectangleF.Height), Color.Transparent, MapHatchStyle.None, string.Empty, MapImageWrapMode.Tile, Color.Empty, MapImageAlign.Center, GradientType.HorizontalCenter, color, Color.Empty, 0, MapDashStyle.None, PenAlignment.Inset);
				}
				break;
			}
			chartGraph.SmoothingMode = smoothingMode;
		}

		private int GetBorderSize()
		{
			return 0;
		}

		private LegendTableStyle GetLegendTableStyle(MapGraphics g)
		{
			LegendTableStyle tableStyle = TableStyle;
			if (TableStyle == LegendTableStyle.Auto)
			{
				if (Dock != 0)
				{
					if (Dock == PanelDockStyle.Left || Dock == PanelDockStyle.Right)
					{
						return LegendTableStyle.Tall;
					}
					return LegendTableStyle.Wide;
				}
				SizeF size = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f)).Size;
				if (size.Width < size.Height)
				{
					return LegendTableStyle.Tall;
				}
				return LegendTableStyle.Wide;
			}
			return tableStyle;
		}

		internal override bool IsRenderVisible(MapGraphics g, RectangleF clipRect)
		{
			if (base.AutoSize && MaxAutoSize == 0f)
			{
				return false;
			}
			return base.IsRenderVisible(g, clipRect);
		}

		internal override object GetDefaultPropertyValue(string prop, object currentValue)
		{
			object obj = null;
			switch (prop)
			{
			case "Dock":
				return PanelDockStyle.Right;
			case "DockAlignment":
				return DockAlignment.Near;
			case "SizeUnit":
				return CoordinateUnit.Percent;
			case "BackColor":
				return Color.White;
			case "BorderColor":
				return Color.DarkGray;
			default:
				return base.GetDefaultPropertyValue(prop, currentValue);
			}
		}

		internal override void Invalidate()
		{
			InvalidateAndLayout();
		}
	}
}

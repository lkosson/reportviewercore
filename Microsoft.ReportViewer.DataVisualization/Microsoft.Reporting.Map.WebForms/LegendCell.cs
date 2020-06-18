using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	[Description("Represents a cell of a legend item.")]
	internal class LegendCell : NamedElement, IToolTipProvider, IImageMapProvider
	{
		private Legend legend;

		private LegendItem legendItem;

		private LegendCellType cellType;

		private string text = string.Empty;

		private Color textColor = Color.Empty;

		private Color backColor = Color.Empty;

		private Font font;

		private string image = string.Empty;

		private Color imageTranspColor = Color.Empty;

		private Size imageSize = Size.Empty;

		private Size symbolSize = new Size(200, 70);

		private ContentAlignment alignment = ContentAlignment.MiddleCenter;

		private int cellSpan = 1;

		private string toolTip = string.Empty;

		private Margins margins = new Margins(0, 0, 15, 15);

		private int rowIndex = -1;

		private int columnIndex = -1;

		internal Rectangle cellPosition = Rectangle.Empty;

		internal Rectangle cellPositionWithMargins = Rectangle.Empty;

		private Size cachedCellSize = Size.Empty;

		private int cachedCellSizeFontReducedBy;

		private string href = string.Empty;

		private string cellAttributes = string.Empty;

		private object mapAreaTag;

		[Browsable(true)]
		[SRDescription("DescriptionAttributeLegendCell_Name")]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(LegendCellType.Text)]
		[SRDescription("DescriptionAttributeLegendCell_CellType")]
		[ParenthesizePropertyName(true)]
		public virtual LegendCellType CellType
		{
			get
			{
				return cellType;
			}
			set
			{
				cellType = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeLegendCell_Text")]
		public virtual string Text
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

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendCell_TextColor")]
		public virtual Color TextColor
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

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendCell_BackColor")]
		public virtual Color BackColor
		{
			get
			{
				return backColor;
			}
			set
			{
				backColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeLegendCell_Font")]
		public virtual Font Font
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

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeLegendCell_Image")]
		public virtual string Image
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
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendCell_ImageTranspColor")]
		public virtual Color ImageTranspColor
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

		[SRCategory("CategoryAttribute_Layout")]
		[DefaultValue(typeof(Size), "0, 0")]
		[TypeConverter(typeof(SizeEmptyValueConverter))]
		[SRDescription("DescriptionAttributeLegendCell_ImageSize")]
		public virtual Size ImageSize
		{
			get
			{
				return imageSize;
			}
			set
			{
				if (value.Width < 0 || value.Height < 0)
				{
					throw new ArgumentException("Cell image width and height cannot be negative numbers.", "ImageSize");
				}
				imageSize = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[DefaultValue(typeof(Size), "200, 70")]
		[SRDescription("DescriptionAttributeLegendCell_SymbolSize")]
		public virtual Size SymbolSize
		{
			get
			{
				return symbolSize;
			}
			set
			{
				if (value.Width < 0 || value.Height < 0)
				{
					throw new ArgumentException("Cell symbol width and height cannot be negative numbers.", "SeriesSymbolSize");
				}
				symbolSize = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[DefaultValue(ContentAlignment.MiddleCenter)]
		[SRDescription("DescriptionAttributeLegendCell_Alignment")]
		public virtual ContentAlignment Alignment
		{
			get
			{
				return alignment;
			}
			set
			{
				alignment = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeLegendCell_CellSpan")]
		public virtual int CellSpan
		{
			get
			{
				return cellSpan;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentException("Cell span must be equal or more than one.", "CellSpan");
				}
				cellSpan = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[DefaultValue(typeof(Margins), "0,0,15,15")]
		[SRDescription("DescriptionAttributeLegendCell_Margins")]
		[SerializationVisibility(SerializationVisibility.Attribute)]
		[NotifyParentProperty(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Margins Margins
		{
			get
			{
				return margins;
			}
			set
			{
				margins = value;
				Invalidate();
				if (GetLegend() != null)
				{
					margins.Common = GetLegend().Common;
				}
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLegendCell_ToolTip")]
		[DefaultValue("")]
		public virtual string ToolTip
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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public LegendItem LegendItem => legendItem;

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLegendCell_Href")]
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
		[SRDescription("DescriptionAttributeLegendCell_CellAttributes")]
		[DefaultValue("")]
		public virtual string CellAttributes
		{
			get
			{
				return cellAttributes;
			}
			set
			{
				cellAttributes = value;
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

		internal override string DefaultName => "Cell";

		internal override CommonElements Common
		{
			get
			{
				return base.Common;
			}
			set
			{
				base.Common = value;
				if (margins != null)
				{
					margins.Common = value;
				}
			}
		}

		public LegendCell()
			: base(null)
		{
			Intitialize(LegendCellType.Text, string.Empty, ContentAlignment.MiddleCenter);
		}

		public LegendCell(string text)
			: base(null)
		{
			Intitialize(LegendCellType.Text, text, ContentAlignment.MiddleCenter);
		}

		public LegendCell(LegendCellType cellType, string text)
			: base(null)
		{
			Intitialize(cellType, text, ContentAlignment.MiddleCenter);
		}

		public LegendCell(LegendCellType cellType, string text, ContentAlignment alignment)
			: base(null)
		{
			Intitialize(cellType, text, alignment);
		}

		private void Intitialize(LegendCellType cellType, string text, ContentAlignment alignment)
		{
			this.cellType = cellType;
			if (this.cellType == LegendCellType.Image)
			{
				image = text;
			}
			else
			{
				this.text = text;
			}
			this.alignment = alignment;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected bool ShouldSerializeMargins()
		{
			if (margins.Top == 0 && margins.Bottom == 0 && margins.Left == 15 && margins.Right == 15)
			{
				return false;
			}
			return true;
		}

		internal void ResetCache()
		{
			cachedCellSize = Size.Empty;
			cachedCellSizeFontReducedBy = 0;
		}

		internal void SetCellPosition(MapGraphics g, int columnIndex, int rowIndex, Rectangle position, int fontSizeReducedBy, Font legendAutoFont, Size singleWCharacterSize)
		{
			cellPosition = position;
			cellPositionWithMargins = position;
			this.rowIndex = rowIndex;
			this.columnIndex = columnIndex;
			cellPosition.X += (int)((float)(Margins.Left * singleWCharacterSize.Width) / 100f);
			cellPosition.Y += (int)((float)(Margins.Top * singleWCharacterSize.Height) / 100f);
			cellPosition.Width -= (int)((float)(Margins.Left * singleWCharacterSize.Width) / 100f) + (int)((float)(Margins.Right * singleWCharacterSize.Width) / 100f);
			cellPosition.Height -= (int)((float)(Margins.Top * singleWCharacterSize.Height) / 100f) + (int)((float)(Margins.Bottom * singleWCharacterSize.Height) / 100f);
			if (GetLegend() != null && legendItem != null && legendItem.Separator != 0)
			{
				cellPosition.Height -= GetLegend().GetSeparatorSize(g, legendItem.Separator).Height;
			}
		}

		internal Size MeasureCell(MapGraphics g, int fontSizeReducedBy, Font legendAutoFont, Size singleWCharacterSize)
		{
			if (cachedCellSizeFontReducedBy == fontSizeReducedBy && !cachedCellSize.IsEmpty)
			{
				return cachedCellSize;
			}
			Size result = Size.Empty;
			bool disposeFont = false;
			Font cellFont = GetCellFont(legendAutoFont, fontSizeReducedBy, out disposeFont);
			if (CellType == LegendCellType.Symbol)
			{
				result.Width = (int)((float)(Math.Abs(SymbolSize.Width) * singleWCharacterSize.Width) / 100f);
				result.Height = (int)((float)(Math.Abs(SymbolSize.Height) * singleWCharacterSize.Height) / 100f);
				result.Width = (int)Math.Round((double)result.Width * 1.1);
				result.Height = (int)Math.Round((double)result.Height * 1.25);
			}
			else if (CellType == LegendCellType.Image)
			{
				if (ImageSize.IsEmpty && !string.IsNullOrEmpty(Image))
				{
					Image image = GetLegend().Common.ImageLoader.LoadImage(Image);
					result.Width = image.Width;
					result.Height = image.Height;
				}
				else
				{
					result.Width = (int)((float)(Math.Abs(ImageSize.Width) * singleWCharacterSize.Width) / 100f);
					result.Height = (int)((float)(Math.Abs(ImageSize.Height) * singleWCharacterSize.Height) / 100f);
				}
			}
			else
			{
				if (CellType != 0)
				{
					throw new InvalidOperationException("Unknown Legend Cell Type: " + CellType.ToString(CultureInfo.CurrentCulture));
				}
				string cellText = GetCellText();
				result = g.MeasureStringAbs(cellText + "I", cellFont);
			}
			result.Width += (int)((float)((Margins.Left + Margins.Right) * singleWCharacterSize.Width) / 100f);
			result.Height += (int)((float)((Margins.Top + Margins.Bottom) * singleWCharacterSize.Height) / 100f);
			if (GetLegend() != null && legendItem != null && legendItem.Separator != 0)
			{
				result.Height += GetLegend().GetSeparatorSize(g, legendItem.Separator).Height;
			}
			if (disposeFont)
			{
				cellFont.Dispose();
				cellFont = null;
			}
			cachedCellSize = result;
			cachedCellSizeFontReducedBy = fontSizeReducedBy;
			return result;
		}

		private Color GetCellBackColor()
		{
			Color result = BackColor;
			if (BackColor.IsEmpty && GetLegend() != null)
			{
				if (legendItem != null)
				{
					int num = legendItem.Cells.IndexOf(this);
					if (num >= 0 && num < GetLegend().CellColumns.Count && !GetLegend().CellColumns[num].BackColor.IsEmpty)
					{
						result = GetLegend().CellColumns[num].BackColor;
					}
				}
				if (result.IsEmpty && GetLegend().InterlacedRows && rowIndex % 2 != 0)
				{
					result = ((!GetLegend().InterlacedRowsColor.IsEmpty) ? GetLegend().InterlacedRowsColor : ((GetLegend().BackColor == Color.Empty) ? Color.LightGray : ((!(GetLegend().BackColor == Color.Transparent)) ? MapGraphics.GetGradientColor(GetLegend().BackColor, Color.Black, 0.2) : ((!(GetLegend().Common.MapCore.BackColor != Color.Transparent) || !(GetLegend().Common.MapCore.BackColor != Color.Black)) ? Color.LightGray : MapGraphics.GetGradientColor(GetLegend().Common.MapCore.BackColor, Color.Black, 0.2)))));
				}
			}
			return result;
		}

		private Font GetCellFont(Font legendAutoFont, int fontSizeReducedBy, out bool disposeFont)
		{
			Font font = Font;
			disposeFont = false;
			if (font == null && GetLegend() != null)
			{
				if (legendItem != null)
				{
					int num = legendItem.Cells.IndexOf(this);
					if (num >= 0 && num < GetLegend().CellColumns.Count && GetLegend().CellColumns[num].Font != null)
					{
						font = GetLegend().CellColumns[num].Font;
					}
				}
				if (font == null)
				{
					return legendAutoFont;
				}
			}
			if (font != null && fontSizeReducedBy != 0)
			{
				disposeFont = true;
				int num2 = (int)Math.Round(font.Size - (float)fontSizeReducedBy);
				if (num2 < 1)
				{
					num2 = 1;
				}
				font = new Font(font.FontFamily, num2, font.Style, font.Unit);
			}
			return font;
		}

		private string GetCellText()
		{
			string text = Text.Replace("\\n", "\n");
			text = ((legendItem == null) ? text.Replace("#LEGENDTEXT", "") : text.Replace("#LEGENDTEXT", legendItem.Text));
			if (GetLegend() != null)
			{
				int textWrapThreshold = GetLegend().TextWrapThreshold;
				if (textWrapThreshold > 0 && text.Length > textWrapThreshold)
				{
					int num = 0;
					for (int i = 0; i < text.Length; i++)
					{
						if (text[i] == '\n')
						{
							num = 0;
							continue;
						}
						num++;
						if (char.IsWhiteSpace(text, i) && num >= textWrapThreshold)
						{
							num = 0;
							text = text.Substring(0, i) + "\n" + text.Substring(i + 1).TrimStart();
						}
					}
				}
			}
			return text;
		}

		private Color GetCellTextColor()
		{
			if (!TextColor.IsEmpty)
			{
				return TextColor;
			}
			if (GetLegend() != null)
			{
				if (legendItem != null)
				{
					int num = legendItem.Cells.IndexOf(this);
					if (num >= 0 && num < GetLegend().CellColumns.Count && !GetLegend().CellColumns[num].TextColor.IsEmpty)
					{
						return GetLegend().CellColumns[num].TextColor;
					}
				}
				return GetLegend().TextColor;
			}
			return Color.Black;
		}

		internal override void Invalidate()
		{
			if (legend != null)
			{
				legend.Invalidate();
			}
		}

		internal void SetContainingLegend(Legend legend, LegendItem legendItem)
		{
			this.legend = legend;
			this.legendItem = legendItem;
			if (this.legend != null)
			{
				margins.Common = this.legend.Common;
			}
		}

		public virtual Legend GetLegend()
		{
			if (legend != null)
			{
				return legend;
			}
			if (legendItem != null)
			{
				return legendItem.Legend;
			}
			return null;
		}

		public virtual LegendItem GetLegendItem()
		{
			return legendItem;
		}

		internal void Paint(MapGraphics chartGraph, int fontSizeReducedBy, Font legendAutoFont, Size singleWCharacterSize)
		{
			if (cellPosition.Width <= 0 || cellPosition.Height <= 0 || !GetLegend().Common.ProcessModePaint)
			{
				return;
			}
			Color cellBackColor = GetCellBackColor();
			if (!cellBackColor.IsEmpty)
			{
				chartGraph.FillRectangleRel(chartGraph.GetRelativeRectangle(cellPositionWithMargins), cellBackColor, MapHatchStyle.None, string.Empty, MapImageWrapMode.Tile, Color.Empty, MapImageAlign.Center, GradientType.None, Color.Empty, Color.Empty, 0, MapDashStyle.None, Color.Empty, 0, PenAlignment.Inset);
			}
			if (GetLegend().Common.ProcessModePaint)
			{
				switch (CellType)
				{
				case LegendCellType.Text:
					PaintCellText(chartGraph, fontSizeReducedBy, legendAutoFont, singleWCharacterSize);
					break;
				case LegendCellType.Image:
					PaintCellImage(chartGraph, fontSizeReducedBy, legendAutoFont, singleWCharacterSize);
					break;
				case LegendCellType.Symbol:
					PaintCellSeriesSymbol(chartGraph, fontSizeReducedBy, legendAutoFont, singleWCharacterSize);
					break;
				default:
					throw new InvalidOperationException("Unknown legend cell type: '" + CellType.ToString(CultureInfo.CurrentCulture) + "'.");
				}
			}
		}

		private void PaintCellText(MapGraphics g, int fontSizeReducedBy, Font legendAutoFont, Size singleWCharacterSize)
		{
			bool disposeFont = false;
			Font cellFont = GetCellFont(legendAutoFont, fontSizeReducedBy, out disposeFont);
			g.StartHotRegion(this);
			using (SolidBrush brush = new SolidBrush(GetCellTextColor()))
			{
				StringFormat stringFormat = new StringFormat(StringFormat.GenericDefault);
				stringFormat.FormatFlags = StringFormatFlags.LineLimit;
				stringFormat.Trimming = StringTrimming.EllipsisCharacter;
				stringFormat.Alignment = StringAlignment.Center;
				if (Alignment == ContentAlignment.BottomLeft || Alignment == ContentAlignment.MiddleLeft || Alignment == ContentAlignment.TopLeft)
				{
					stringFormat.Alignment = StringAlignment.Near;
				}
				else if (Alignment == ContentAlignment.BottomRight || Alignment == ContentAlignment.MiddleRight || Alignment == ContentAlignment.TopRight)
				{
					stringFormat.Alignment = StringAlignment.Far;
				}
				stringFormat.LineAlignment = StringAlignment.Center;
				if (Alignment == ContentAlignment.BottomCenter || Alignment == ContentAlignment.BottomLeft || Alignment == ContentAlignment.BottomRight)
				{
					stringFormat.LineAlignment = StringAlignment.Far;
				}
				else if (Alignment == ContentAlignment.TopCenter || Alignment == ContentAlignment.TopLeft || Alignment == ContentAlignment.TopRight)
				{
					stringFormat.LineAlignment = StringAlignment.Near;
				}
				SizeF sizeF = g.MeasureStringAbs(GetCellText(), cellFont, new SizeF(10000f, 10000f), stringFormat);
				if (sizeF.Height > (float)cellPosition.Height && (stringFormat.FormatFlags & StringFormatFlags.LineLimit) != 0)
				{
					stringFormat.FormatFlags ^= StringFormatFlags.LineLimit;
				}
				else if (sizeF.Height < (float)cellPosition.Height && (stringFormat.FormatFlags & StringFormatFlags.LineLimit) == 0)
				{
					stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
				}
				g.DrawStringRel(GetCellText(), cellFont, brush, g.GetRelativeRectangle(cellPosition), stringFormat);
			}
			g.EndHotRegion();
			if (disposeFont)
			{
				cellFont.Dispose();
				cellFont = null;
			}
		}

		private void PaintCellImage(MapGraphics chartGraph, int fontSizeReducedBy, Font legendAutoFont, Size singleWCharacterSize)
		{
			if (string.IsNullOrEmpty(Image))
			{
				return;
			}
			Rectangle empty = Rectangle.Empty;
			Image image = GetLegend().Common.ImageLoader.LoadImage(Image);
			empty.Width = image.Size.Width;
			empty.Height = image.Size.Height;
			Rectangle rectangle = cellPosition;
			rectangle.Width = empty.Width;
			rectangle.Height = empty.Height;
			if (!ImageSize.IsEmpty)
			{
				if (ImageSize.Width > 0)
				{
					int num = (int)((float)(ImageSize.Width * singleWCharacterSize.Width) / 100f);
					if (num > cellPosition.Width)
					{
						num = cellPosition.Width;
					}
					rectangle.Width = num;
				}
				if (ImageSize.Height > 0)
				{
					int num2 = (int)((float)(ImageSize.Height * singleWCharacterSize.Height) / 100f);
					if (num2 > cellPosition.Height)
					{
						num2 = cellPosition.Height;
					}
					rectangle.Height = num2;
				}
			}
			float num3 = 1f;
			if (empty.Height > rectangle.Height)
			{
				num3 = (float)empty.Height / (float)rectangle.Height;
			}
			if (empty.Width > rectangle.Width)
			{
				num3 = Math.Max(num3, (float)empty.Width / (float)rectangle.Width);
			}
			empty.Height = (int)((float)empty.Height / num3);
			empty.Width = (int)((float)empty.Width / num3);
			empty.X = (int)((float)cellPosition.X + (float)cellPosition.Width / 2f - (float)empty.Width / 2f);
			empty.Y = (int)((float)cellPosition.Y + (float)cellPosition.Height / 2f - (float)empty.Height / 2f);
			if (Alignment == ContentAlignment.BottomLeft || Alignment == ContentAlignment.MiddleLeft || Alignment == ContentAlignment.TopLeft)
			{
				empty.X = cellPosition.X;
			}
			else if (Alignment == ContentAlignment.BottomRight || Alignment == ContentAlignment.MiddleRight || Alignment == ContentAlignment.TopRight)
			{
				empty.X = cellPosition.Right - empty.Width;
			}
			if (Alignment == ContentAlignment.BottomCenter || Alignment == ContentAlignment.BottomLeft || Alignment == ContentAlignment.BottomRight)
			{
				empty.Y = cellPosition.Bottom - empty.Height;
			}
			else if (Alignment == ContentAlignment.TopCenter || Alignment == ContentAlignment.TopLeft || Alignment == ContentAlignment.TopRight)
			{
				empty.Y = cellPosition.Y;
			}
			ImageAttributes imageAttributes = new ImageAttributes();
			if (ImageTranspColor != Color.Empty)
			{
				imageAttributes.SetColorKey(ImageTranspColor, ImageTranspColor, ColorAdjustType.Default);
			}
			SmoothingMode smoothingMode = chartGraph.SmoothingMode;
			CompositingQuality compositingQuality = chartGraph.Graphics.CompositingQuality;
			InterpolationMode interpolationMode = chartGraph.Graphics.InterpolationMode;
			chartGraph.SmoothingMode = SmoothingMode.AntiAlias;
			chartGraph.Graphics.CompositingQuality = CompositingQuality.HighQuality;
			chartGraph.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			chartGraph.DrawImage(image, empty, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
			chartGraph.SmoothingMode = smoothingMode;
			chartGraph.Graphics.CompositingQuality = compositingQuality;
			chartGraph.Graphics.InterpolationMode = interpolationMode;
		}

		private void PaintCellSeriesSymbol(MapGraphics g, int fontSizeReducedBy, Font legendAutoFont, SizeF singleWCharacterSize)
		{
			Rectangle r = cellPosition;
			if (SymbolSize.Width >= 0)
			{
				int num = (int)((float)SymbolSize.Width * singleWCharacterSize.Width / 100f);
				if (num > cellPosition.Width)
				{
					num = cellPosition.Width;
				}
				r.Width = num;
			}
			if (SymbolSize.Height >= 0)
			{
				int num2 = (int)((float)SymbolSize.Height * singleWCharacterSize.Height / 100f);
				if (num2 > cellPosition.Height)
				{
					num2 = cellPosition.Height;
				}
				r.Height = num2;
			}
			if (r.Height <= 0 || r.Width <= 0)
			{
				return;
			}
			r.X = (int)((float)cellPosition.X + (float)cellPosition.Width / 2f - (float)r.Width / 2f);
			r.Y = (int)((float)cellPosition.Y + (float)cellPosition.Height / 2f - (float)r.Height / 2f);
			if (Alignment == ContentAlignment.BottomLeft || Alignment == ContentAlignment.MiddleLeft || Alignment == ContentAlignment.TopLeft)
			{
				r.X = cellPosition.X;
			}
			else if (Alignment == ContentAlignment.BottomRight || Alignment == ContentAlignment.MiddleRight || Alignment == ContentAlignment.TopRight)
			{
				r.X = cellPosition.Right - r.Width;
			}
			if (Alignment == ContentAlignment.BottomCenter || Alignment == ContentAlignment.BottomLeft || Alignment == ContentAlignment.BottomRight)
			{
				r.Y = cellPosition.Bottom - r.Height;
			}
			else if (Alignment == ContentAlignment.TopCenter || Alignment == ContentAlignment.TopLeft || Alignment == ContentAlignment.TopRight)
			{
				r.Y = cellPosition.Y;
			}
			g.StartHotRegion(this);
			if (!string.IsNullOrEmpty(legendItem.Image))
			{
				Rectangle empty = Rectangle.Empty;
				Image image = GetLegend().Common.ImageLoader.LoadImage(legendItem.Image);
				empty.Width = image.Size.Width;
				empty.Height = image.Size.Height;
				float num3 = 1f;
				if (empty.Height > r.Height)
				{
					num3 = (float)empty.Height / (float)r.Height;
				}
				if (empty.Width > r.Width)
				{
					num3 = Math.Max(num3, (float)empty.Width / (float)r.Width);
				}
				empty.Height = (int)((float)empty.Height / num3);
				empty.Width = (int)((float)empty.Width / num3);
				empty.X = (int)((float)r.X + (float)r.Width / 2f - (float)empty.Width / 2f);
				empty.Y = (int)((float)r.Y + (float)r.Height / 2f - (float)empty.Height / 2f);
				ImageAttributes imageAttributes = new ImageAttributes();
				if (legendItem.ImageTranspColor != Color.Empty)
				{
					imageAttributes.SetColorKey(legendItem.ImageTranspColor, legendItem.ImageTranspColor, ColorAdjustType.Default);
				}
				g.DrawImage(image, empty, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
			}
			else if (legendItem.ItemStyle == LegendItemStyle.Shape)
			{
				g.FillRectangleRel(g.GetRelativeRectangle(r), legendItem.Color, legendItem.HatchStyle, legendItem.Image, legendItem.ImageWrapMode, legendItem.ImageTranspColor, legendItem.ImageAlign, legendItem.GradientType, legendItem.SecondaryColor, legendItem.borderColor, (legendItem.BorderWidth > 2) ? 2 : legendItem.BorderWidth, legendItem.BorderStyle, legendItem.ShadowColor, (legendItem.ShadowOffset > 3) ? 3 : legendItem.ShadowOffset, PenAlignment.Center);
			}
			else if (legendItem.ItemStyle == LegendItemStyle.Path)
			{
				Point location = r.Location;
				location.Y = (int)Math.Round((float)r.Y + (float)r.Height / 2f);
				Point pt = new Point(r.Right, location.Y);
				int num4 = (int)Math.Round((float)legendItem.PathWidth / 2f);
				location.X += num4;
				pt.X -= num4;
				SmoothingMode smoothingMode = g.SmoothingMode;
				if (legendItem.PathWidth < 2 && legendItem.BorderWidth < 2)
				{
					g.SmoothingMode = SmoothingMode.None;
				}
				using (GraphicsPath graphicsPath = new GraphicsPath())
				{
					graphicsPath.AddLine(location, pt);
					int num5 = (legendItem.shadowOffset > 3) ? 3 : legendItem.shadowOffset;
					if (num5 > 0)
					{
						using (Pen pen = Path.GetColorPen(g.GetShadowColor(), legendItem.PathWidth, legendItem.BorderWidth))
						{
							if (pen != null)
							{
								Matrix matrix = new Matrix();
								matrix.Translate(num5, num5, MatrixOrder.Append);
								graphicsPath.Transform(matrix);
								g.DrawPath(pen, graphicsPath);
								matrix.Reset();
								matrix.Translate(-num5, -num5, MatrixOrder.Append);
								graphicsPath.Transform(matrix);
							}
						}
					}
					if (legendItem.BorderWidth > 0)
					{
						using (Pen pen2 = Path.GetColorPen(legendItem.BorderColor, legendItem.PathWidth, legendItem.BorderWidth))
						{
							if (pen2 != null)
							{
								g.DrawPath(pen2, graphicsPath);
							}
						}
					}
					RectangleF bounds = graphicsPath.GetBounds();
					bounds.Inflate((float)legendItem.PathWidth / 2f, (float)legendItem.PathWidth / 2f);
					using (Pen pen3 = Path.GetFillPen(g, graphicsPath, bounds, legendItem.PathWidth, legendItem.PathLineStyle, legendItem.Color, legendItem.SecondaryColor, legendItem.GradientType, legendItem.HatchStyle))
					{
						if (pen3 != null)
						{
							g.DrawPath(pen3, graphicsPath);
							if (pen3.Brush != null)
							{
								pen3.Brush.Dispose();
							}
						}
					}
				}
				g.SmoothingMode = smoothingMode;
			}
			else if (legendItem.ItemStyle == LegendItemStyle.Symbol)
			{
				MarkerStyle markerStyle = legendItem.markerStyle;
				if (markerStyle != 0 || !string.IsNullOrEmpty(legendItem.markerImage))
				{
					int num6 = Math.Min(r.Width, r.Height);
					int num7 = (legendItem.MarkerBorderWidth > 3) ? 3 : legendItem.MarkerBorderWidth;
					if (num7 > 0)
					{
						num6 -= num7;
						if (num6 < 1)
						{
							num6 = 1;
						}
					}
					Point point = default(Point);
					point.X = (int)((float)r.X + (float)r.Width / 2f);
					point.Y = (int)((float)r.Y + (float)r.Height / 2f);
					Rectangle empty2 = Rectangle.Empty;
					if (!string.IsNullOrEmpty(legendItem.markerImage))
					{
						Image image2 = GetLegend().Common.ImageLoader.LoadImage(legendItem.markerImage);
						empty2.Width = image2.Size.Width;
						empty2.Height = image2.Size.Height;
						float num8 = 1f;
						if (empty2.Height > r.Height)
						{
							num8 = (float)empty2.Height / (float)r.Height;
						}
						if (empty2.Width > r.Width)
						{
							num8 = Math.Max(num8, (float)empty2.Width / (float)r.Width);
						}
						empty2.Height = (int)((float)empty2.Height / num8);
						empty2.Width = (int)((float)empty2.Width / num8);
					}
					Color color = (legendItem.markerColor == Color.Empty) ? legendItem.Color : legendItem.markerColor;
					if (Symbol.IsXamlMarker(markerStyle))
					{
						RectangleF rect = r;
						if (rect.Width > rect.Height)
						{
							rect.X += (rect.Width - rect.Height) / 2f;
							rect.Width = rect.Height;
						}
						else if (rect.Height > rect.Width)
						{
							rect.Y += (rect.Height - rect.Width) / 2f;
							rect.Height = rect.Width;
						}
						using (XamlRenderer xamlRenderer = Symbol.CreateXamlRenderer(markerStyle, color, rect))
						{
							XamlLayer[] layers = xamlRenderer.Layers;
							for (int i = 0; i < layers.Length; i++)
							{
								layers[i].Render(g);
							}
						}
					}
					else
					{
						PointF absolute = new PointF(point.X, point.Y);
						if ((double)(num6 % 2) != 0.0)
						{
							absolute.X -= 0.5f;
							absolute.Y -= 0.5f;
						}
						g.DrawMarkerRel(g.GetRelativePoint(absolute), markerStyle, num6, color, legendItem.MarkerGradientType, legendItem.MarkerHatchStyle, legendItem.MarkerSecondaryColor, legendItem.MarkerBorderStyle, (legendItem.markerBorderColor == Color.Empty) ? legendItem.borderColor : legendItem.markerBorderColor, num7, legendItem.markerImage, legendItem.markerImageTranspColor, (legendItem.shadowOffset > 3) ? 3 : legendItem.shadowOffset, legendItem.shadowColor, empty2);
					}
				}
			}
			g.EndHotRegion();
		}

		string IImageMapProvider.GetToolTip()
		{
			return ((IToolTipProvider)this).GetToolTip();
		}

		string IImageMapProvider.GetHref()
		{
			if (!string.IsNullOrEmpty(href))
			{
				return Href;
			}
			if (legendItem != null)
			{
				return legendItem.Href;
			}
			return string.Empty;
		}

		string IImageMapProvider.GetMapAreaAttributes()
		{
			if (CellAttributes.Length > 0)
			{
				return CellAttributes;
			}
			if (legendItem != null)
			{
				return legendItem.MapAreaAttributes;
			}
			return string.Empty;
		}

		string IToolTipProvider.GetToolTip()
		{
			if (!string.IsNullOrEmpty(ToolTip))
			{
				return ToolTip;
			}
			if (legendItem != null)
			{
				return legendItem.ToolTip;
			}
			return string.Empty;
		}
	}
}

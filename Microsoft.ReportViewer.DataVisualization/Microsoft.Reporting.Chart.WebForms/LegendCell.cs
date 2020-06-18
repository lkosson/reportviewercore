using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeLegendCell_LegendCell")]
	internal class LegendCell : IMapAreaAttributes
	{
		private Legend legend;

		private LegendItem legendItem;

		private string name = string.Empty;

		private LegendCellType cellType;

		private string text = string.Empty;

		private Color textColor = Color.Empty;

		private Color backColor = Color.Empty;

		private Font font;

		private string image = string.Empty;

		private Color imageTranspColor = Color.Empty;

		private Size imageSize = Size.Empty;

		private Size seriesSymbolSize = new Size(200, 70);

		private ContentAlignment alignment = ContentAlignment.MiddleCenter;

		private int cellSpan = 1;

		private string toolTip = string.Empty;

		private Margins margins = new Margins(0, 0, 15, 15);

		private string href = string.Empty;

		private int rowIndex = -1;

		private int columnIndex = -1;

		private string mapAreaAttribute = string.Empty;

		internal Rectangle cellPosition = Rectangle.Empty;

		internal Rectangle cellPositionWithMargins = Rectangle.Empty;

		private Size cachedCellSize = Size.Empty;

		private int cachedCellSizeFontReducedBy;

		private object mapAreaTag;

		[SRCategory("CategoryAttributeMisc")]
		[SRDescription("DescriptionAttributeLegendCell_Name")]
		public virtual string Name
		{
			get
			{
				return name;
			}
			set
			{
				if (!(name != value))
				{
					return;
				}
				if (legendItem != null)
				{
					foreach (LegendCell cell in legendItem.Cells)
					{
						if (cell.Name == value)
						{
							throw new ArgumentException(SR.ExceptionLegendCellNameAlreadyExistsInCollection(value));
						}
					}
				}
				if (value == null || value.Length == 0)
				{
					throw new ArgumentException(SR.ExceptionLegendCellNameIsEmpty);
				}
				name = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
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

		[SRCategory("CategoryAttributeAppearance")]
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

		[SRCategory("CategoryAttributeAppearance")]
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

		[SRCategory("CategoryAttributeAppearance")]
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

		[SRCategory("CategoryAttributeAppearance")]
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

		[SRCategory("CategoryAttributeAppearance")]
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

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendCell_ImageTransparentColor")]
		public virtual Color ImageTransparentColor
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

		[SRCategory("CategoryAttributeLayout")]
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
					throw new ArgumentException(SR.ExceptionLegendCellImageSizeIsNegative, "value");
				}
				imageSize = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeLayout")]
		[DefaultValue(typeof(Size), "200, 70")]
		[SRDescription("DescriptionAttributeLegendCell_SeriesSymbolSize")]
		public virtual Size SeriesSymbolSize
		{
			get
			{
				return seriesSymbolSize;
			}
			set
			{
				if (value.Width < 0 || value.Height < 0)
				{
					throw new ArgumentException(SR.ExceptionLegendCellSeriesSymbolSizeIsNegative, "value");
				}
				seriesSymbolSize = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeLayout")]
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

		[SRCategory("CategoryAttributeLayout")]
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
					throw new ArgumentException(SR.ExceptionLegendCellSpanIsLessThenOne, "value");
				}
				cellSpan = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeLayout")]
		[DefaultValue(typeof(Margins), "0,0,15,15")]
		[SRDescription("DescriptionAttributeLegendCell_Margins")]
		[SerializationVisibility(SerializationVisibility.Attribute)]
		[NotifyParentProperty(true)]
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

		[SRCategory("CategoryAttributeMapArea")]
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

		[SRCategory("CategoryAttributeMapArea")]
		[SRDescription("DescriptionAttributeLegendCell_Href")]
		[DefaultValue("")]
		public virtual string Href
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
		[SRDescription("DescriptionAttributeLegendCell_MapAreaAttributes")]
		[DefaultValue("")]
		public virtual string MapAreaAttributes
		{
			get
			{
				return mapAreaAttribute;
			}
			set
			{
				mapAreaAttribute = value;
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

		string IMapAreaAttributes.ToolTip
		{
			get
			{
				return ToolTip;
			}
			set
			{
				ToolTip = value;
			}
		}

		string IMapAreaAttributes.Href
		{
			get
			{
				return Href;
			}
			set
			{
				Href = value;
			}
		}

		string IMapAreaAttributes.MapAreaAttributes
		{
			get
			{
				return MapAreaAttributes;
			}
			set
			{
				MapAreaAttributes = value;
			}
		}

		public LegendCell()
		{
			Intitialize(LegendCellType.Text, string.Empty, ContentAlignment.MiddleCenter);
		}

		public LegendCell(string text)
		{
			Intitialize(LegendCellType.Text, text, ContentAlignment.MiddleCenter);
		}

		public LegendCell(LegendCellType cellType, string text)
		{
			Intitialize(cellType, text, ContentAlignment.MiddleCenter);
		}

		public LegendCell(LegendCellType cellType, string text, ContentAlignment alignment)
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
		public bool ShouldSerializeMargins()
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

		internal void SetCellPosition(ChartGraphics graph, int columnIndex, int rowIndex, Rectangle position, int fontSizeReducedBy, Font legendAutoFont, Size singleWCharacterSize)
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
				cellPosition.Height -= GetLegend().GetSeparatorSize(graph, legendItem.Separator).Height;
			}
		}

		internal Size MeasureCell(ChartGraphics graph, int fontSizeReducedBy, Font legendAutoFont, Size singleWCharacterSize)
		{
			if (cachedCellSizeFontReducedBy == fontSizeReducedBy && !cachedCellSize.IsEmpty)
			{
				return cachedCellSize;
			}
			Size result = Size.Empty;
			bool disposeFont = false;
			Font cellFont = GetCellFont(legendAutoFont, fontSizeReducedBy, out disposeFont);
			if (CellType == LegendCellType.SeriesSymbol)
			{
				result.Width = (int)((float)(Math.Abs(SeriesSymbolSize.Width) * singleWCharacterSize.Width) / 100f);
				result.Height = (int)((float)(Math.Abs(SeriesSymbolSize.Height) * singleWCharacterSize.Height) / 100f);
			}
			else if (CellType == LegendCellType.Image)
			{
				if (ImageSize.IsEmpty && Image.Length > 0)
				{
					SizeF size = default(SizeF);
					if (GetLegend().Common.ImageLoader.GetAdjustedImageSize(Image, graph.Graphics, ref size))
					{
						result.Width = (int)size.Width;
						result.Height = (int)size.Height;
					}
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
					throw new InvalidOperationException(SR.ExceptionLegendCellTypeUnknown(CellType.ToString()));
				}
				string cellText = GetCellText();
				result = graph.MeasureStringAbs(cellText + "I", cellFont);
			}
			result.Width += (int)((float)((Margins.Left + Margins.Right) * singleWCharacterSize.Width) / 100f);
			result.Height += (int)((float)((Margins.Top + Margins.Bottom) * singleWCharacterSize.Height) / 100f);
			if (GetLegend() != null && legendItem != null && legendItem.Separator != 0)
			{
				result.Height += GetLegend().GetSeparatorSize(graph, legendItem.Separator).Height;
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
					result = ((!GetLegend().InterlacedRowsColor.IsEmpty) ? GetLegend().InterlacedRowsColor : ((GetLegend().BackColor == Color.Empty) ? Color.LightGray : ((!(GetLegend().BackColor == Color.Transparent)) ? ChartGraphics.GetGradientColor(GetLegend().BackColor, Color.Black, 0.2) : ((!(GetLegend().Common.Chart.BackColor != Color.Transparent) || !(GetLegend().Common.Chart.BackColor != Color.Black)) ? Color.LightGray : ChartGraphics.GetGradientColor(GetLegend().Common.Chart.BackColor, Color.Black, 0.2)))));
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

		private string GetCellToolTip()
		{
			if (ToolTip.Length > 0)
			{
				return ToolTip;
			}
			if (legendItem != null)
			{
				return legendItem.ToolTip;
			}
			return string.Empty;
		}

		private string GetCellHref()
		{
			if (href.Length > 0)
			{
				return href;
			}
			if (legendItem != null)
			{
				return legendItem.Href;
			}
			return string.Empty;
		}

		private string GetCellMapAreaAttributes()
		{
			if (mapAreaAttribute.Length > 0)
			{
				return mapAreaAttribute;
			}
			if (legendItem != null)
			{
				return legendItem.MapAreaAttributes;
			}
			return string.Empty;
		}

		private string GetCellText()
		{
			string text = Text.Replace("\\n", "\n");
			text = ((legendItem == null) ? text.Replace("#LEGENDTEXT", "") : text.Replace("#LEGENDTEXT", legendItem.Name));
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
				return GetLegend().FontColor;
			}
			return Color.Black;
		}

		protected void Invalidate()
		{
			if (legend != null)
			{
				legend.Invalidate(invalidateLegendOnly: false);
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

		internal void Paint(ChartGraphics chartGraph, int fontSizeReducedBy, Font legendAutoFont, Size singleWCharacterSize, PointF animationLocationAdjustment)
		{
			if (cellPosition.Width <= 0 || cellPosition.Height <= 0)
			{
				return;
			}
			if (GetLegend().Common.ProcessModePaint)
			{
				Color cellBackColor = GetCellBackColor();
				RectangleF relativeRectangle = chartGraph.GetRelativeRectangle(cellPositionWithMargins);
				if (!cellBackColor.IsEmpty)
				{
					chartGraph.FillRectangleRel(relativeRectangle, cellBackColor, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, Color.Empty, 0, ChartDashStyle.NotSet, Color.Empty, 0, PenAlignment.Inset);
				}
				GetLegend().Common.EventsManager.OnBackPaint(this, new ChartPaintEventArgs(chartGraph, GetLegend().Common, new ElementPosition(relativeRectangle.X, relativeRectangle.Y, relativeRectangle.Width, relativeRectangle.Height)));
				switch (CellType)
				{
				case LegendCellType.Text:
					PaintCellText(chartGraph, fontSizeReducedBy, legendAutoFont, singleWCharacterSize, animationLocationAdjustment);
					break;
				case LegendCellType.Image:
					PaintCellImage(chartGraph, fontSizeReducedBy, legendAutoFont, singleWCharacterSize, animationLocationAdjustment);
					break;
				case LegendCellType.SeriesSymbol:
					PaintCellSeriesSymbol(chartGraph, fontSizeReducedBy, legendAutoFont, singleWCharacterSize, animationLocationAdjustment);
					break;
				default:
					throw new InvalidOperationException(SR.ExceptionLegendCellTypeUnknown(CellType.ToString()));
				}
				GetLegend().Common.EventsManager.OnPaint(this, new ChartPaintEventArgs(chartGraph, GetLegend().Common, new ElementPosition(relativeRectangle.X, relativeRectangle.Y, relativeRectangle.Width, relativeRectangle.Height)));
			}
			if (GetLegend().Common.ProcessModeRegions)
			{
				GetLegend().Common.HotRegionsList.AddHotRegion(chartGraph, chartGraph.GetRelativeRectangle(cellPositionWithMargins), GetCellToolTip(), GetCellHref(), GetCellMapAreaAttributes(), legendItem, this, ChartElementType.LegendItem, legendItem.SeriesName);
			}
		}

		private void PaintCellText(ChartGraphics chartGraph, int fontSizeReducedBy, Font legendAutoFont, Size singleWCharacterSize, PointF animationLocationAdjustment)
		{
			bool disposeFont = false;
			Font cellFont = GetCellFont(legendAutoFont, fontSizeReducedBy, out disposeFont);
			chartGraph.StartHotRegion(GetCellHref(), GetCellToolTip());
			chartGraph.StartAnimation();
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
				SizeF sizeF = chartGraph.MeasureStringAbs(GetCellText(), cellFont, new SizeF(10000f, 10000f), stringFormat);
				if (sizeF.Height > (float)cellPosition.Height && (stringFormat.FormatFlags & StringFormatFlags.LineLimit) != 0)
				{
					stringFormat.FormatFlags ^= StringFormatFlags.LineLimit;
				}
				else if (sizeF.Height < (float)cellPosition.Height && (stringFormat.FormatFlags & StringFormatFlags.LineLimit) == 0)
				{
					stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
				}
				chartGraph.DrawStringRel(GetCellText(), cellFont, brush, chartGraph.GetRelativeRectangle(cellPosition), stringFormat);
			}
			chartGraph.StopAnimation();
			chartGraph.EndHotRegion();
			if (disposeFont)
			{
				cellFont.Dispose();
				cellFont = null;
			}
		}

		private void PaintCellImage(ChartGraphics chartGraph, int fontSizeReducedBy, Font legendAutoFont, Size singleWCharacterSize, PointF animationLocationAdjustment)
		{
			if (Image.Length <= 0)
			{
				return;
			}
			Rectangle empty = Rectangle.Empty;
			Image image = GetLegend().Common.ImageLoader.LoadImage(Image);
			SizeF size = default(SizeF);
			ImageLoader.GetAdjustedImageSize(image, chartGraph.Graphics, ref size);
			empty.Width = (int)size.Width;
			empty.Height = (int)size.Height;
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
			if (ImageTransparentColor != Color.Empty)
			{
				imageAttributes.SetColorKey(ImageTransparentColor, ImageTransparentColor, ColorAdjustType.Default);
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

		private void PaintCellSeriesSymbol(ChartGraphics chartGraph, int fontSizeReducedBy, Font legendAutoFont, SizeF singleWCharacterSize, PointF animationLocationAdjustment)
		{
			Rectangle r = cellPosition;
			if (SeriesSymbolSize.Width >= 0)
			{
				int num = (int)((float)SeriesSymbolSize.Width * singleWCharacterSize.Width / 100f);
				if (num > cellPosition.Width)
				{
					num = cellPosition.Width;
				}
				r.Width = num;
			}
			if (SeriesSymbolSize.Height >= 0)
			{
				int num2 = (int)((float)SeriesSymbolSize.Height * singleWCharacterSize.Height / 100f);
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
			chartGraph.StartHotRegion(GetCellHref(), GetCellToolTip());
			chartGraph.StartAnimation();
			if (legendItem.Image.Length > 0)
			{
				Rectangle empty = Rectangle.Empty;
				Image image = GetLegend().Common.ImageLoader.LoadImage(legendItem.Image);
				if (image != null)
				{
					SizeF size = default(SizeF);
					ImageLoader.GetAdjustedImageSize(image, chartGraph.Graphics, ref size);
					empty.Width = (int)size.Width;
					empty.Height = (int)size.Height;
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
					if (legendItem.BackImageTransparentColor != Color.Empty)
					{
						imageAttributes.SetColorKey(legendItem.BackImageTransparentColor, legendItem.BackImageTransparentColor, ColorAdjustType.Default);
					}
					chartGraph.DrawImage(image, empty, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				}
			}
			else
			{
				int num4 = (int)Math.Round(3f * chartGraph.Graphics.DpiX / 96f);
				int num5 = (int)Math.Round(3f * chartGraph.Graphics.DpiX / 96f);
				if (legendItem.Style == LegendImageStyle.Rectangle)
				{
					int num6 = (int)Math.Round(2f * chartGraph.Graphics.DpiX / 96f);
					chartGraph.FillRectangleRel(chartGraph.GetRelativeRectangle(r), legendItem.Color, legendItem.BackHatchStyle, legendItem.Image, legendItem.backImageMode, legendItem.BackImageTransparentColor, legendItem.backImageAlign, legendItem.backGradientType, legendItem.backGradientEndColor, legendItem.borderColor, (legendItem.BorderWidth > num6) ? num6 : legendItem.BorderWidth, legendItem.BorderStyle, legendItem.ShadowColor, (legendItem.ShadowOffset > num4) ? num4 : legendItem.ShadowOffset, PenAlignment.Inset);
				}
				if (legendItem.Style == LegendImageStyle.Line)
				{
					Point p = default(Point);
					p.X = r.X;
					p.Y = (int)((float)r.Y + (float)r.Height / 2f);
					Point p2 = default(Point);
					p2.Y = p.Y;
					p2.X = r.Right;
					SmoothingMode smoothingMode = chartGraph.SmoothingMode;
					chartGraph.SmoothingMode = SmoothingMode.None;
					chartGraph.DrawLineRel(legendItem.Color, (legendItem.borderWidth > num5) ? num5 : legendItem.borderWidth, legendItem.borderStyle, chartGraph.GetRelativePoint(p), chartGraph.GetRelativePoint(p2), legendItem.shadowColor, (legendItem.shadowOffset > num4) ? num4 : legendItem.shadowOffset);
					chartGraph.SmoothingMode = smoothingMode;
				}
				if (legendItem.Style == LegendImageStyle.Marker || legendItem.Style == LegendImageStyle.Line)
				{
					MarkerStyle markerStyle = legendItem.markerStyle;
					if (legendItem.style == LegendImageStyle.Marker)
					{
						markerStyle = ((legendItem.markerStyle == MarkerStyle.None) ? MarkerStyle.Circle : legendItem.markerStyle);
					}
					if (markerStyle != 0 || legendItem.markerImage.Length > 0)
					{
						int num7 = Math.Min(r.Width, r.Height);
						num7 = (int)Math.Min(legendItem.markerSize, (legendItem.style == LegendImageStyle.Line) ? (2f * ((float)num7 / 3f)) : ((float)num7));
						int num8 = (legendItem.MarkerBorderWidth > num5) ? num5 : legendItem.MarkerBorderWidth;
						if (num8 > 0)
						{
							num7 -= num8;
							if (num7 < 1)
							{
								num7 = 1;
							}
						}
						Point point = default(Point);
						point.X = (int)((float)r.X + (float)r.Width / 2f);
						point.Y = (int)((float)r.Y + (float)r.Height / 2f);
						Rectangle empty2 = Rectangle.Empty;
						if (legendItem.markerImage.Length > 0)
						{
							Image obj = GetLegend().Common.ImageLoader.LoadImage(legendItem.markerImage);
							SizeF size2 = default(SizeF);
							ImageLoader.GetAdjustedImageSize(obj, chartGraph.Graphics, ref size2);
							empty2.Width = (int)size2.Width;
							empty2.Height = (int)size2.Height;
							float num9 = 1f;
							if (empty2.Height > r.Height)
							{
								num9 = (float)empty2.Height / (float)r.Height;
							}
							if (empty2.Width > r.Width)
							{
								num9 = Math.Max(num9, (float)empty2.Width / (float)r.Width);
							}
							empty2.Height = (int)((float)empty2.Height / num9);
							empty2.Width = (int)((float)empty2.Width / num9);
						}
						PointF absolute = new PointF(point.X, point.Y);
						if ((double)(num7 % 2) != 0.0)
						{
							absolute.X -= 0.5f;
							absolute.Y -= 0.5f;
						}
						chartGraph.DrawMarkerRel(chartGraph.GetRelativePoint(absolute), markerStyle, num7, (legendItem.markerColor == Color.Empty) ? legendItem.Color : legendItem.markerColor, (legendItem.markerBorderColor == Color.Empty) ? legendItem.borderColor : legendItem.markerBorderColor, num8, legendItem.markerImage, legendItem.markerImageTranspColor, (legendItem.shadowOffset > num4) ? num4 : legendItem.shadowOffset, legendItem.shadowColor, empty2);
					}
				}
			}
			chartGraph.StopAnimation();
			chartGraph.EndHotRegion();
		}
	}
}

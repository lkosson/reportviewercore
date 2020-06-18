using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeLegendCellColumn_LegendCellColumn")]
	internal class LegendCellColumn
	{
		private Legend legend;

		private string name = string.Empty;

		private LegendCellColumnType columnType;

		private string text = "#LEGENDTEXT";

		private Color textColor = Color.Empty;

		private Color backColor = Color.Empty;

		private Font font;

		private Size seriesSymbolSize = new Size(200, 70);

		private ContentAlignment alignment = ContentAlignment.MiddleCenter;

		private string toolTip = string.Empty;

		private Margins margins = new Margins(0, 0, 15, 15);

		private string href = string.Empty;

		private string mapAreaAttribute = string.Empty;

		private string headerText = string.Empty;

		private StringAlignment headerTextAlignment = StringAlignment.Center;

		private Color headerColor = Color.Black;

		private Color headerBackColor = Color.Empty;

		private Font headerFont = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f, FontStyle.Bold);

		private int minimumCellWidth = -1;

		private int maximumCellWidth = -1;

		[SRCategory("CategoryAttributeMisc")]
		[SRDescription("DescriptionAttributeLegendCellColumn_Name")]
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
				if (legend != null)
				{
					foreach (LegendCellColumn cellColumn in legend.CellColumns)
					{
						if (cellColumn.Name == value)
						{
							throw new ArgumentException(SR.ExceptionLegendColumnAlreadyExistsInCollection(value));
						}
					}
				}
				if (value == null || value.Length == 0)
				{
					throw new ArgumentException(SR.ExceptionLegendColumnIsEmpty);
				}
				name = value;
			}
		}

		[SRCategory("CategoryAttributeSeriesItems")]
		[DefaultValue(LegendCellColumnType.Text)]
		[SRDescription("DescriptionAttributeLegendCellColumn_ColumnType")]
		[ParenthesizePropertyName(true)]
		public virtual LegendCellColumnType ColumnType
		{
			get
			{
				return columnType;
			}
			set
			{
				columnType = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeSeriesItems")]
		[DefaultValue("#LEGENDTEXT")]
		[SRDescription("DescriptionAttributeLegendCellColumn_Text")]
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

		[SRCategory("CategoryAttributeSeriesItems")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendCellColumn_TextColor")]
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

		[SRCategory("CategoryAttributeSeriesItems")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendCellColumn_BackColor")]
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

		[SRCategory("CategoryAttributeSeriesItems")]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeLegendCellColumn_Font")]
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

		[SRCategory("CategoryAttributeSeriesItems")]
		[DefaultValue(typeof(Size), "200, 70")]
		[SRDescription("DescriptionAttributeLegendCellColumn_SeriesSymbolSize")]
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
					throw new ArgumentException(SR.ExceptionSeriesSymbolSizeIsNegative, "value");
				}
				seriesSymbolSize = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeSeriesItems")]
		[DefaultValue(ContentAlignment.MiddleCenter)]
		[SRDescription("DescriptionAttributeLegendCellColumn_Alignment")]
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

		[SRCategory("CategoryAttributeSeriesItems")]
		[DefaultValue(typeof(Margins), "0,0,15,15")]
		[SRDescription("DescriptionAttributeLegendCellColumn_Margins")]
		[SerializationVisibility(SerializationVisibility.Attribute)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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

		[SRCategory("CategoryAttributeSeriesItems")]
		[SRDescription("DescriptionAttributeLegendCellColumn_ToolTip")]
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

		[SRCategory("CategoryAttributeSeriesItems")]
		[SRDescription("DescriptionAttributeLegendCellColumn_Href")]
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

		[SRCategory("CategoryAttributeSeriesItems")]
		[SRDescription("DescriptionAttributeLegendCellColumn_MapAreaAttributes")]
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

		[SRCategory("CategoryAttributeHeader")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeLegendCellColumn_HeaderText")]
		public virtual string HeaderText
		{
			get
			{
				return headerText;
			}
			set
			{
				headerText = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeHeader")]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLegendCellColumn_HeaderColor")]
		public virtual Color HeaderColor
		{
			get
			{
				return headerColor;
			}
			set
			{
				headerColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeHeader")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendCellColumn_HeaderBackColor")]
		public virtual Color HeaderBackColor
		{
			get
			{
				return headerBackColor;
			}
			set
			{
				headerBackColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeHeader")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt, style=Bold")]
		[SRDescription("DescriptionAttributeLegendCellColumn_HeaderFont")]
		public virtual Font HeaderFont
		{
			get
			{
				return headerFont;
			}
			set
			{
				headerFont = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeHeader")]
		[DefaultValue(typeof(StringAlignment), "Center")]
		[SRDescription("DescriptionAttributeLegendCellColumn_HeaderTextAlignment")]
		public StringAlignment HeaderTextAlignment
		{
			get
			{
				return headerTextAlignment;
			}
			set
			{
				if (value != headerTextAlignment)
				{
					headerTextAlignment = value;
					Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttributeSize")]
		[DefaultValue(-1)]
		[TypeConverter(typeof(IntNanValueConverter))]
		[SRDescription("DescriptionAttributeLegendCellColumn_MinimumWidth")]
		public virtual int MinimumWidth
		{
			get
			{
				return minimumCellWidth;
			}
			set
			{
				if (value < -1)
				{
					throw new ArgumentException(SR.ExceptionMinimumCellWidthIsWrong, "value");
				}
				minimumCellWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeSize")]
		[DefaultValue(-1)]
		[TypeConverter(typeof(IntNanValueConverter))]
		[SRDescription("DescriptionAttributeLegendCellColumn_MaximumWidth")]
		public virtual int MaximumWidth
		{
			get
			{
				return maximumCellWidth;
			}
			set
			{
				if (value < -1)
				{
					throw new ArgumentException(SR.ExceptionMaximumCellWidthIsWrong, "value");
				}
				maximumCellWidth = value;
				Invalidate();
			}
		}

		public LegendCellColumn()
			: this(string.Empty, LegendCellColumnType.Text, "#LEGENDTEXT", ContentAlignment.MiddleCenter)
		{
		}

		public LegendCellColumn(string headerText, LegendCellColumnType columnType, string text)
			: this(headerText, columnType, text, ContentAlignment.MiddleCenter)
		{
		}

		public LegendCellColumn(string headerText, LegendCellColumnType columnType, string text, ContentAlignment alignment)
		{
			this.headerText = headerText;
			this.columnType = columnType;
			this.text = text;
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

		internal LegendCell CreateNewCell()
		{
			return new LegendCell
			{
				CellType = ((ColumnType == LegendCellColumnType.SeriesSymbol) ? LegendCellType.SeriesSymbol : LegendCellType.Text),
				Text = Text,
				ToolTip = ToolTip,
				Href = Href,
				MapAreaAttributes = MapAreaAttributes,
				SeriesSymbolSize = SeriesSymbolSize,
				Alignment = Alignment,
				Margins = new Margins(Margins.Top, Margins.Bottom, Margins.Left, Margins.Right)
			};
		}

		protected void Invalidate()
		{
			if (legend != null)
			{
				legend.Invalidate(invalidateLegendOnly: false);
			}
		}

		public virtual Legend GetLegend()
		{
			return legend;
		}

		internal void SetContainingLegend(Legend legend)
		{
			this.legend = legend;
			if (this.legend != null)
			{
				margins.Common = this.legend.Common;
			}
		}
	}
}

using System;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	[Description("Represents a column in the map legend.")]
	internal class LegendCellColumn : NamedElement, IToolTipProvider, IImageMapProvider
	{
		private Legend legend;

		private LegendCellColumnType columnType;

		private string text = "#LEGENDTEXT";

		private Color textColor = Color.Empty;

		private Color backColor = Color.Empty;

		private Font font;

		private Size seriesSymbolSize = new Size(200, 70);

		private ContentAlignment alignment = ContentAlignment.MiddleCenter;

		private Margins margins = new Margins(0, 0, 15, 15);

		private string headerText = string.Empty;

		private StringAlignment headerTextAlignment = StringAlignment.Center;

		private Color headerColor = Color.Black;

		private Color headerBackColor = Color.Empty;

		private int minimumCellWidth = -1;

		private int maximumCellWidth = -1;

		private string toolTip = string.Empty;

		private string href = string.Empty;

		private string cellColumnAttributes = string.Empty;

		private Font headerFont = new Font("Microsoft Sans Serif", 8f);

		private object mapAreaTag;

		[Category("Series Items")]
		[DefaultValue(LegendCellColumnType.Text)]
		[Description("Legend column type of the items automatically generated.")]
		[ParenthesizePropertyName(true)]
		internal virtual LegendCellColumnType ColumnType
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

		[Category("Series Items")]
		[DefaultValue("#LEGENDTEXT")]
		[Description("Legend column text of the items automatically generated. Set ColumnType to Text to use this property.")]
		internal virtual string Text
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

		[Category("Series Items")]
		[DefaultValue(typeof(Color), "")]
		[Description("Legend column text color.")]
		internal virtual Color TextColor
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

		[Category("Series Items")]
		[DefaultValue(typeof(Color), "")]
		[Description("Legend column back color.")]
		internal virtual Color BackColor
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

		[Category("Series Items")]
		[DefaultValue(null)]
		[Description("Legend column text font.")]
		internal virtual Font Font
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

		[Category("Series Items")]
		[DefaultValue(typeof(Size), "200, 70")]
		[Description("Legend column symbol size (as a percentage of legend font size). This is only applicable to items that are automatically generated.")]
		internal virtual Size SeriesSymbolSize
		{
			get
			{
				return seriesSymbolSize;
			}
			set
			{
				if (value.Width < 0 || value.Height < 0)
				{
					throw new ArgumentException("Column symbol width and height cannot be a negative number.", "SeriesSymbolSize");
				}
				seriesSymbolSize = value;
				Invalidate();
			}
		}

		[Category("Series Items")]
		[DefaultValue(ContentAlignment.MiddleCenter)]
		[Description("Legend column content alignment of the items automatically generated.")]
		internal virtual ContentAlignment Alignment
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

		[Category("Series Items")]
		[DefaultValue(typeof(Margins), "0,0,15,15")]
		[Description("Legend column margins (as a percentage of legend font size).  This is only applicable to items that are automatically generated.")]
		[SerializationVisibility(SerializationVisibility.Attribute)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		internal virtual Margins Margins
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

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLegendCellColumn_Href")]
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
		[SRDescription("DescriptionAttributeLegendCellColumn_CellColumnAttributes")]
		[DefaultValue("")]
		public virtual string CellColumnAttributes
		{
			get
			{
				return cellColumnAttributes;
			}
			set
			{
				cellColumnAttributes = value;
			}
		}

		[SRCategory("CategoryAttribute_Header")]
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

		[SRCategory("CategoryAttribute_Header")]
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

		[SRCategory("CategoryAttribute_Header")]
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

		[SRCategory("CategoryAttribute_Header")]
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

		[SRCategory("CategoryAttribute_Header")]
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

		[SRCategory("CategoryAttribute_Size")]
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
					throw new ArgumentException("Column minimum width cannot be less than -1.", "MinimumCellWidth");
				}
				minimumCellWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Size")]
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
					throw new ArgumentException("Column maximum width cannot be less than -1.", "MaximumWidth");
				}
				maximumCellWidth = value;
				Invalidate();
			}
		}

		[Browsable(true)]
		[SRDescription("DescriptionAttributeLegendCellColumn_Name")]
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

		internal override string DefaultName => "Column";

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

		public LegendCellColumn()
			: base(null)
		{
			Intitialize(string.Empty, LegendCellColumnType.Text, "#LEGENDTEXT", ContentAlignment.MiddleCenter);
		}

		public LegendCellColumn(string headerText)
			: base(null)
		{
			Intitialize(headerText, LegendCellColumnType.Text, "#LEGENDTEXT", ContentAlignment.MiddleCenter);
		}

		public LegendCellColumn(string name, string headerText)
			: base(null)
		{
			Intitialize(headerText, LegendCellColumnType.Text, "#LEGENDTEXT", ContentAlignment.MiddleCenter);
			Name = name;
		}

		public LegendCellColumn(string name, string headerText, ContentAlignment alignment)
			: base(null)
		{
			Intitialize(headerText, LegendCellColumnType.Text, "#LEGENDTEXT", alignment);
			Name = name;
		}

		private void Intitialize(string headerText, LegendCellColumnType columnType, string text, ContentAlignment alignment)
		{
			this.headerText = headerText;
			ColumnType = columnType;
			Text = text;
			Alignment = alignment;
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

		internal LegendCell CreateNewCell()
		{
			return new LegendCell
			{
				CellType = ((ColumnType == LegendCellColumnType.Symbol) ? LegendCellType.Symbol : LegendCellType.Text),
				Text = Text,
				ToolTip = ToolTip,
				Href = Href,
				CellAttributes = CellColumnAttributes,
				SymbolSize = SeriesSymbolSize,
				Alignment = Alignment,
				Margins = new Margins(Margins.Top, Margins.Bottom, Margins.Left, Margins.Right)
			};
		}

		internal override void Invalidate()
		{
			if (legend != null)
			{
				legend.Invalidate();
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

		string IToolTipProvider.GetToolTip()
		{
			return ToolTip;
		}

		string IImageMapProvider.GetToolTip()
		{
			return ((IToolTipProvider)this).GetToolTip();
		}

		string IImageMapProvider.GetHref()
		{
			return Href;
		}

		string IImageMapProvider.GetMapAreaAttributes()
		{
			return CellColumnAttributes;
		}
	}
}

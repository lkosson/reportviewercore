using System;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class GridAttributes : MapObject, ISelectable
	{
		private GridLine[] gridLines;

		private bool visible = true;

		private Color lineColor = Color.FromArgb(128, 128, 128, 255);

		private int lineWidth = 1;

		private double interval = double.NaN;

		private MapDashStyle lineStyle = MapDashStyle.Solid;

		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private bool showLabels = true;

		private Color labelColor = Color.FromArgb(192, 128, 128, 255);

		private LabelPosition labelPosition;

		private string labelFormatString = "#°E;#°W;0°";

		internal GridLine[] GridLines
		{
			get
			{
				return gridLines;
			}
			set
			{
				gridLines = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeGridAttributes_Visible")]
		[ParenthesizePropertyName(true)]
		[NotifyParentProperty(true)]
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
		[SRDescription("DescriptionAttributeGridAttributes_LineColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "128, 128, 128, 255")]
		public Color LineColor
		{
			get
			{
				return lineColor;
			}
			set
			{
				lineColor = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeGridAttributes_LineWidth")]
		[NotifyParentProperty(true)]
		[DefaultValue(1)]
		public int LineWidth
		{
			get
			{
				return lineWidth;
			}
			set
			{
				lineWidth = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Interval")]
		[SRDescription("DescriptionAttributeGridAttributes_Interval")]
		[NotifyParentProperty(true)]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[DefaultValue(double.NaN)]
		public double Interval
		{
			get
			{
				return interval;
			}
			set
			{
				if (value <= 0.0)
				{
					throw new ArgumentException(SR.ExceptionIntervalGraterThanZero);
				}
				interval = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeGridAttributes_LineStyle")]
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
		[SRDescription("DescriptionAttributeGridAttributes_Font")]
		[NotifyParentProperty(true)]
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
		[SRDescription("DescriptionAttributeGridAttributes_ShowLabels")]
		[NotifyParentProperty(true)]
		[DefaultValue(true)]
		public bool ShowLabels
		{
			get
			{
				return showLabels;
			}
			set
			{
				showLabels = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeGridAttributes_LabelColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "192, 128, 128, 255")]
		public Color LabelColor
		{
			get
			{
				return labelColor;
			}
			set
			{
				labelColor = value;
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeGridAttributes_LabelPosition")]
		[NotifyParentProperty(true)]
		[DefaultValue(LabelPosition.Near)]
		public LabelPosition LabelPosition
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeGridAttributes_LabelFormatString")]
		[DefaultValue("#°E;#°W;0°")]
		public string LabelFormatString
		{
			get
			{
				return labelFormatString;
			}
			set
			{
				labelFormatString = value;
				InvalidateViewport();
			}
		}

		public GridAttributes()
			: this(null, parallels: true)
		{
		}

		public GridAttributes(object parent, bool parallels)
			: base(parent)
		{
			if (parallels)
			{
				LabelFormatString = "#°N;#°S;0°";
			}
		}

		internal MapCore GetMapCore()
		{
			return (MapCore)Parent;
		}

		internal Pen GetPen()
		{
			return new Pen(LineColor, LineWidth)
			{
				Width = LineWidth,
				DashStyle = MapGraphics.GetPenStyle(LineStyle)
			};
		}

		void ISelectable.DrawSelection(MapGraphics g, RectangleF clipRect, bool designTimeSelection)
		{
			MapCore mapCore = GetMapCore();
			GridLine[] array = GridLines;
			for (int i = 0; i < array.Length; i++)
			{
				GridLine gridLine = array[i];
				g.DrawSelectionMarkers(gridLine.SelectionMarkerPositions, designTimeSelection, mapCore.SelectionBorderColor, mapCore.SelectionMarkerColor);
			}
		}

		bool ISelectable.IsSelected()
		{
			return false;
		}

		bool ISelectable.IsVisible()
		{
			return true;
		}

		RectangleF ISelectable.GetSelectionRectangle(MapGraphics g, RectangleF clipRect)
		{
			return RectangleF.Empty;
		}
	}
}

using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeCustomLabel_CustomLabel")]
	[DefaultProperty("Text")]
	internal class CustomLabel
	{
		internal Axis axis;

		private string name = "Custom Label";

		private double from;

		private double to;

		private string text = "";

		private LabelMark labelMark;

		private Color textColor = Color.Empty;

		private Color markColor = Color.Empty;

		private int labelRowIndex;

		private GridTicks gridTick;

		internal bool customLabel = true;

		private object tag;

		private string image = string.Empty;

		private Color imageTranspColor = Color.Empty;

		private string tooltip = string.Empty;

		private string imageHref = string.Empty;

		private string imageMapAreaAttributes = string.Empty;

		private string href = string.Empty;

		private string mapAreaAttributes = string.Empty;

		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeCustomLabel_ToolTip")]
		[DefaultValue("")]
		public string ToolTip
		{
			get
			{
				return tooltip;
			}
			set
			{
				tooltip = value;
			}
		}

		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeCustomLabel_Href")]
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
		[SRDescription("DescriptionAttributeCustomLabel_MapAreaAttributes")]
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

		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeCustomLabel_ImageHref")]
		[DefaultValue("")]
		public string ImageHref
		{
			get
			{
				return imageHref;
			}
			set
			{
				imageHref = value;
			}
		}

		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeCustomLabel_ImageMapAreaAttributes")]
		[DefaultValue("")]
		public string ImageMapAreaAttributes
		{
			get
			{
				return imageMapAreaAttributes;
			}
			set
			{
				imageMapAreaAttributes = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[SRDescription("DescriptionAttributeCustomLabel_Tag")]
		[DefaultValue(null)]
		[Browsable(false)]
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

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeCustomLabel_Image")]
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

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeCustomLabel_ImageTransparentColor")]
		public Color ImageTransparentColor
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

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeCustomLabel_Name")]
		[DefaultValue("Custom Label")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DesignOnly(true)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(GridTicks.None)]
		[SRDescription("DescriptionAttributeCustomLabel_GridTicks")]
		public GridTicks GridTicks
		{
			get
			{
				return gridTick;
			}
			set
			{
				gridTick = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeCustomLabel_From")]
		[TypeConverter(typeof(AxisLabelDateValueConverter))]
		public double From
		{
			get
			{
				return from;
			}
			set
			{
				from = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeCustomLabel_To")]
		[TypeConverter(typeof(AxisLabelDateValueConverter))]
		public double To
		{
			get
			{
				return to;
			}
			set
			{
				to = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeCustomLabel_Text")]
		public string Text
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
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeCustomLabel_TextColor")]
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

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeCustomLabel_MarkColor")]
		[NotifyParentProperty(true)]
		public Color MarkColor
		{
			get
			{
				return markColor;
			}
			set
			{
				markColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[Bindable(false)]
		[DefaultValue(LabelRow.First)]
		[SRDescription("DescriptionAttributeCustomLabel_Row")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public LabelRow Row
		{
			get
			{
				if (axis != null && axis.chart != null && axis.chart.serializing)
				{
					return LabelRow.First;
				}
				if (labelRowIndex != 0)
				{
					return LabelRow.Second;
				}
				return LabelRow.First;
			}
			set
			{
				if (labelRowIndex == 0)
				{
					labelRowIndex = ((value != 0) ? 1 : 0);
					Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeCustomLabel_RowIndex")]
		public int RowIndex
		{
			get
			{
				return labelRowIndex;
			}
			set
			{
				if (value < 0)
				{
					throw new InvalidOperationException(SR.ExceptionAxisLabelRowIndexIsNegative);
				}
				labelRowIndex = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(LabelMark.None)]
		[SRDescription("DescriptionAttributeCustomLabel_LabelMark")]
		public LabelMark LabelMark
		{
			get
			{
				return labelMark;
			}
			set
			{
				labelMark = value;
				Invalidate();
			}
		}

		public CustomLabel()
		{
		}

		public CustomLabel(double fromPosition, double toPosition, string text, int labelRow, LabelMark mark)
		{
			from = fromPosition;
			to = toPosition;
			this.text = text;
			RowIndex = labelRow;
			labelMark = mark;
			gridTick = GridTicks.None;
		}

		public CustomLabel(double fromPosition, double toPosition, string text, int labelRow, LabelMark mark, GridTicks gridTick)
		{
			from = fromPosition;
			to = toPosition;
			this.text = text;
			RowIndex = labelRow;
			labelMark = mark;
			this.gridTick = gridTick;
		}

		public CustomLabel(double fromPosition, double toPosition, string text, LabelRow row, LabelMark mark)
		{
			from = fromPosition;
			to = toPosition;
			this.text = text;
			RowIndex = ((row != 0) ? 1 : 0);
			labelMark = mark;
			gridTick = GridTicks.None;
		}

		public CustomLabel(double fromPosition, double toPosition, string text, LabelRow row, LabelMark mark, GridTicks gridTick)
		{
			from = fromPosition;
			to = toPosition;
			this.text = text;
			RowIndex = ((row != 0) ? 1 : 0);
			labelMark = mark;
			this.gridTick = gridTick;
		}

		public CustomLabel Clone()
		{
			return new CustomLabel
			{
				From = From,
				To = To,
				Text = Text,
				TextColor = TextColor,
				MarkColor = MarkColor,
				RowIndex = RowIndex,
				LabelMark = LabelMark,
				GridTicks = GridTicks,
				ToolTip = ToolTip,
				Tag = Tag,
				Image = Image,
				ImageTransparentColor = ImageTransparentColor,
				Href = Href,
				MapAreaAttributes = MapAreaAttributes,
				ImageHref = ImageHref,
				ImageMapAreaAttributes = ImageMapAreaAttributes
			};
		}

		public Axis GetAxis()
		{
			return axis;
		}

		private void Invalidate()
		{
		}
	}
}

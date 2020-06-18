using Microsoft.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class FinancialMarker
	{
		private FinancialMarkerType markerType;

		private int firstPointIndex;

		private int secondPointIndex;

		private int firstYIndex;

		private int secondYIndex;

		private Color lineColor = Color.Gray;

		private int lineWidth = 1;

		private Color textColor = Color.Black;

		private Font textFont = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);

		private ChartDashStyle lineStyle = ChartDashStyle.Solid;

		[Bindable(true)]
		[DefaultValue(typeof(FinancialMarkerType), "FibonacciArcs")]
		[SRDescription("DescriptionAttributeFinancialMarker_MarkerType")]
		[ParenthesizePropertyName(true)]
		public FinancialMarkerType MarkerType
		{
			get
			{
				return markerType;
			}
			set
			{
				markerType = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(false)]
		[Browsable(false)]
		[DefaultValue("FinancialMarker")]
		[SRDescription("DescriptionAttributeFinancialMarker_Name")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public string Name => "FinancialMarker";

		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeFinancialMarker_FirstPointIndex")]
		[ParenthesizePropertyName(true)]
		public int FirstPointIndex
		{
			get
			{
				return firstPointIndex;
			}
			set
			{
				firstPointIndex = value;
			}
		}

		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeFinancialMarker_SecondPointIndex")]
		[ParenthesizePropertyName(true)]
		public int SecondPointIndex
		{
			get
			{
				return secondPointIndex;
			}
			set
			{
				secondPointIndex = value;
			}
		}

		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeFinancialMarker_FirstYIndex")]
		public int FirstYIndex
		{
			get
			{
				return firstYIndex;
			}
			set
			{
				firstYIndex = value;
			}
		}

		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeFinancialMarker_SecondYIndex")]
		public int SecondYIndex
		{
			get
			{
				return secondYIndex;
			}
			set
			{
				secondYIndex = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Gray")]
		[SRDescription("DescriptionAttributeFinancialMarker_LineColor")]
		public Color LineColor
		{
			get
			{
				return lineColor;
			}
			set
			{
				lineColor = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeFinancialMarker_TextColor")]
		public Color TextColor
		{
			get
			{
				return textColor;
			}
			set
			{
				textColor = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeFinancialMarker_Font")]
		public Font Font
		{
			get
			{
				return textFont;
			}
			set
			{
				textFont = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeFinancialMarker_LineWidth")]
		public int LineWidth
		{
			get
			{
				return lineWidth;
			}
			set
			{
				lineWidth = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeFinancialMarker_LineStyle")]
		public ChartDashStyle LineStyle
		{
			get
			{
				return lineStyle;
			}
			set
			{
				lineStyle = value;
			}
		}

		public FinancialMarker()
		{
		}

		public FinancialMarker(FinancialMarkerType markerType, int firstPointIndex, int secondPointIndex, int firstYIndex, int secondYIndex, Color lineColor, int lineWidth, Color textColor, Font textFont)
		{
			this.markerType = markerType;
			this.firstPointIndex = firstPointIndex;
			this.secondPointIndex = secondPointIndex;
			this.firstYIndex = firstYIndex;
			this.secondYIndex = secondYIndex;
			this.lineColor = lineColor;
			this.lineWidth = lineWidth;
			this.textColor = textColor;
			this.textFont = textFont;
		}
	}
}

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(StateConverter))]
	internal class State : Range
	{
		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private Color borderColor = Color.Black;

		private GaugeDashStyle borderStyle;

		private int borderWidth = 1;

		private Color fillColor = Color.Red;

		private GradientType fillGradientType = GradientType.Center;

		private Color fillGradientEndColor = Color.DarkRed;

		private GaugeHatchStyle fillHatchStyle;

		private string text = "Text";

		private StateIndicatorStyle style = StateIndicatorStyle.CircularLed;

		private float scaleFactor = 1f;

		private string image = "";

		private Color imageTransColor = Color.Empty;

		private Color imageHueColor = Color.Empty;

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeState_Name")]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeState_EndValue")]
		[DefaultValue(70.0)]
		public override double StartValue
		{
			get
			{
				return base.StartValue;
			}
			set
			{
				base.StartValue = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeState_EndValue")]
		[DefaultValue(100.0)]
		public override double EndValue
		{
			get
			{
				return base.EndValue;
			}
			set
			{
				base.EndValue = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeState_TriggerDelay")]
		[DefaultValue(0.0)]
		internal double TriggerDelay
		{
			get
			{
				return base.InRangeTimeout;
			}
			set
			{
				base.InRangeTimeout = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeState_TriggerDelayType")]
		[DefaultValue(PeriodType.Seconds)]
		internal PeriodType TriggerDelayType
		{
			get
			{
				return base.InRangeTimeoutType;
			}
			set
			{
				base.InRangeTimeoutType = value;
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeState_Font")]
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
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeState_BorderColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Black")]
		public Color BorderColor
		{
			get
			{
				return borderColor;
			}
			set
			{
				borderColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeState_BorderStyle")]
		[NotifyParentProperty(true)]
		[DefaultValue(GaugeDashStyle.NotSet)]
		public GaugeDashStyle BorderStyle
		{
			get
			{
				return borderStyle;
			}
			set
			{
				borderStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeState_BorderWidth")]
		[NotifyParentProperty(true)]
		[DefaultValue(1)]
		public int BorderWidth
		{
			get
			{
				return borderWidth;
			}
			set
			{
				if (value < 0 || value > 100)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
				}
				borderWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeState_FillColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Red")]
		public Color FillColor
		{
			get
			{
				return fillColor;
			}
			set
			{
				fillColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeState_FillGradientType")]
		[NotifyParentProperty(true)]
		[DefaultValue(GradientType.Center)]
		public GradientType FillGradientType
		{
			get
			{
				return fillGradientType;
			}
			set
			{
				fillGradientType = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFillGradientEndColor5")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "DarkRed")]
		public Color FillGradientEndColor
		{
			get
			{
				return fillGradientEndColor;
			}
			set
			{
				fillGradientEndColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeState_FillHatchStyle")]
		[NotifyParentProperty(true)]
		[DefaultValue(GaugeHatchStyle.None)]
		public GaugeHatchStyle FillHatchStyle
		{
			get
			{
				return fillHatchStyle;
			}
			set
			{
				fillHatchStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeState_Text")]
		[Localizable(true)]
		[DefaultValue("Text")]
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

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeStateIndicator_IndicatorStyle")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(StateIndicatorStyle.CircularLed)]
		public StateIndicatorStyle IndicatorStyle
		{
			get
			{
				return style;
			}
			set
			{
				style = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeStateIndicator_ScaleFactor")]
		[DefaultValue(1f)]
		[ValidateBound(0.0, 1.0)]
		public float ScaleFactor
		{
			get
			{
				return scaleFactor;
			}
			set
			{
				if (value > 1f || value < 0f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 1));
				}
				scaleFactor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeState_Image")]
		[DefaultValue("")]
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

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeImageTransColor6")]
		[DefaultValue(typeof(Color), "")]
		public Color ImageTransColor
		{
			get
			{
				return imageTransColor;
			}
			set
			{
				imageTransColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeImageHueColor4")]
		[DefaultValue(typeof(Color), "")]
		public Color ImageHueColor
		{
			get
			{
				return imageHueColor;
			}
			set
			{
				imageHueColor = value;
				Invalidate();
			}
		}

		public override string ToString()
		{
			return Name;
		}

		internal override void OnAdded()
		{
			((IValueConsumer)((StateIndicator)ParentElement).Data).Refresh();
			base.OnAdded();
		}

		internal override void OnValueRangeTimeOut(object sender, ValueRangeEventArgs e)
		{
			base.OnValueRangeTimeOut(sender, e);
			((StateIndicator)ParentElement).Refresh();
		}
	}
}

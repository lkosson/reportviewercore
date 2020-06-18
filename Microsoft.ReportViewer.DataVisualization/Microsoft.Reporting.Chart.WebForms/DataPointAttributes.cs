using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeDataPointAttributes_DataPointAttributes")]
	[DefaultProperty("Label")]
	[TypeConverter(typeof(NoNameExpandableObjectConverter))]
	internal class DataPointAttributes : IMapAreaAttributes, ICustomTypeDescriptor
	{
		internal IServiceContainer serviceContainer;

		internal bool pointAttributes = true;

		internal Series series;

		internal Hashtable attributes = new Hashtable();

		internal static ColorConverter colorConverter = new ColorConverter();

		internal static FontConverter fontConverter = new FontConverter();

		internal bool tempColorIsSet;

		internal CustomAttributes customAttributes;

		internal bool emptyPoint;

		private object tag;

		private object mapAreaTag;

		private object mapAreaLegendTag;

		private object mapAreaLabelTag;

		public string this[int index]
		{
			get
			{
				int num = 0;
				foreach (object key in attributes.Keys)
				{
					if (num == index)
					{
						if (key is string)
						{
							return (string)key;
						}
						if (key is int)
						{
							return Enum.GetName(typeof(CommonAttributes), key);
						}
						return key.ToString();
					}
					num++;
				}
				throw new IndexOutOfRangeException();
			}
		}

		public string this[string name]
		{
			get
			{
				if (!IsAttributeSet(name) && pointAttributes)
				{
					if (emptyPoint)
					{
						return (string)series.EmptyPointStyle.attributes[name];
					}
					return (string)series.attributes[name];
				}
				return (string)attributes[name];
			}
			set
			{
				attributes[name] = value;
				Invalidate(invalidateLegend: true);
			}
		}

		[DefaultValue("")]
		[SRCategory("CategoryAttributeLabel")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLabel")]
		public virtual string Label
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.Label))
					{
						return (string)GetAttributeObject(CommonAttributes.Label);
					}
					if (IsSerializing())
					{
						return "";
					}
					if (emptyPoint)
					{
						return (string)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.Label);
					}
					return series.label;
				}
				return series.label;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.Label, value);
				}
				else
				{
					series.label = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeAxisLabel")]
		[DefaultValue("")]
		public virtual string AxisLabel
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.AxisLabel))
					{
						return (string)GetAttributeObject(CommonAttributes.AxisLabel);
					}
					if (IsSerializing())
					{
						return "";
					}
					if (emptyPoint)
					{
						return (string)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.AxisLabel);
					}
					return series.axisLabel;
				}
				return series.axisLabel;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.AxisLabel, value);
				}
				else
				{
					series.axisLabel = value;
				}
				if (value.Length > 0 && series != null)
				{
					series.noLabelsInPoints = false;
				}
				Invalidate(invalidateLegend: false);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeLabel")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLabelFormat")]
		[DefaultValue("")]
		public string LabelFormat
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.LabelFormat))
					{
						return (string)GetAttributeObject(CommonAttributes.LabelFormat);
					}
					if (IsSerializing())
					{
						return "";
					}
					if (emptyPoint)
					{
						return (string)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LabelFormat);
					}
					return series.labelFormat;
				}
				return series.labelFormat;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.LabelFormat, value);
				}
				else
				{
					series.labelFormat = value;
				}
				Invalidate(invalidateLegend: false);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeLabel")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeShowLabelAsValue")]
		[DefaultValue(false)]
		public bool ShowLabelAsValue
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.ShowLabelAsValue))
					{
						return (bool)GetAttributeObject(CommonAttributes.ShowLabelAsValue);
					}
					if (IsSerializing())
					{
						return false;
					}
					if (emptyPoint)
					{
						return (bool)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.ShowLabelAsValue);
					}
					return series.showLabelAsValue;
				}
				return series.showLabelAsValue;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.ShowLabelAsValue, value);
				}
				else
				{
					series.showLabelAsValue = value;
				}
				Invalidate(invalidateLegend: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeColor4")]
		[DefaultValue(typeof(Color), "")]
		public Color Color
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.Color))
					{
						return (Color)GetAttributeObject(CommonAttributes.Color);
					}
					if (IsSerializing())
					{
						return Color.Empty;
					}
					if (emptyPoint)
					{
						return (Color)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.Color);
					}
					return series.color;
				}
				return series.color;
			}
			set
			{
				tempColorIsSet = false;
				if (value == Color.Empty && pointAttributes)
				{
					DeleteAttribute(CommonAttributes.Color);
					return;
				}
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.Color, value);
				}
				else
				{
					series.color = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBorderColor9")]
		[DefaultValue(typeof(Color), "")]
		public Color BorderColor
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.BorderColor))
					{
						return (Color)GetAttributeObject(CommonAttributes.BorderColor);
					}
					if (IsSerializing())
					{
						return Color.Empty;
					}
					if (emptyPoint)
					{
						return (Color)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.BorderColor);
					}
					return series.borderColor;
				}
				return series.borderColor;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.BorderColor, value);
				}
				else
				{
					series.borderColor = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBorderStyle3")]
		[DefaultValue(ChartDashStyle.Solid)]
		public ChartDashStyle BorderStyle
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.BorderDashStyle))
					{
						return (ChartDashStyle)GetAttributeObject(CommonAttributes.BorderDashStyle);
					}
					if (IsSerializing())
					{
						return ChartDashStyle.Solid;
					}
					if (emptyPoint)
					{
						return (ChartDashStyle)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.BorderDashStyle);
					}
					return series.borderStyle;
				}
				return series.borderStyle;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.BorderDashStyle, value);
				}
				else
				{
					series.borderStyle = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBorderWidth8")]
		[DefaultValue(1)]
		public int BorderWidth
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.BorderWidth))
					{
						return (int)GetAttributeObject(CommonAttributes.BorderWidth);
					}
					if (IsSerializing())
					{
						return 1;
					}
					if (emptyPoint)
					{
						return (int)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.BorderWidth);
					}
					return series.borderWidth;
				}
				return series.borderWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionBorderWidthIsNotPositive);
				}
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.BorderWidth, value);
				}
				else
				{
					series.borderWidth = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBackImage10")]
		[DefaultValue("")]
		public string BackImage
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.BackImage))
					{
						return (string)GetAttributeObject(CommonAttributes.BackImage);
					}
					if (IsSerializing())
					{
						return "";
					}
					if (emptyPoint)
					{
						return (string)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.BackImage);
					}
					return series.backImage;
				}
				return series.backImage;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.BackImage, value);
				}
				else
				{
					series.backImage = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBackImageMode4")]
		[DefaultValue(ChartImageWrapMode.Tile)]
		public ChartImageWrapMode BackImageMode
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.BackImageMode))
					{
						return (ChartImageWrapMode)GetAttributeObject(CommonAttributes.BackImageMode);
					}
					if (IsSerializing())
					{
						return ChartImageWrapMode.Tile;
					}
					if (emptyPoint)
					{
						return (ChartImageWrapMode)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.BackImageMode);
					}
					return series.backImageMode;
				}
				return series.backImageMode;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.BackImageMode, value);
				}
				else
				{
					series.backImageMode = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackImageTransparentColor")]
		[DefaultValue(typeof(Color), "")]
		public Color BackImageTransparentColor
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.BackImageTransparentColor))
					{
						return (Color)GetAttributeObject(CommonAttributes.BackImageTransparentColor);
					}
					if (IsSerializing())
					{
						return Color.Empty;
					}
					if (emptyPoint)
					{
						return (Color)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.BackImageTransparentColor);
					}
					return series.backImageTranspColor;
				}
				return series.backImageTranspColor;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.BackImageTransparentColor, value);
				}
				else
				{
					series.backImageTranspColor = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackImageAlign")]
		[DefaultValue(ChartImageAlign.TopLeft)]
		public ChartImageAlign BackImageAlign
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.BackImageAlign))
					{
						return (ChartImageAlign)GetAttributeObject(CommonAttributes.BackImageAlign);
					}
					if (IsSerializing())
					{
						return ChartImageAlign.TopLeft;
					}
					if (emptyPoint)
					{
						return (ChartImageAlign)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.BackImageAlign);
					}
					return series.backImageAlign;
				}
				return series.backImageAlign;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.BackImageAlign, value);
				}
				else
				{
					series.backImageAlign = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBackGradientType4")]
		[DefaultValue(GradientType.None)]
		public GradientType BackGradientType
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.BackGradientType))
					{
						return (GradientType)GetAttributeObject(CommonAttributes.BackGradientType);
					}
					if (IsSerializing())
					{
						return GradientType.None;
					}
					if (emptyPoint)
					{
						return (GradientType)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.BackGradientType);
					}
					return series.backGradientType;
				}
				return series.backGradientType;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.BackGradientType, value);
				}
				else
				{
					series.backGradientType = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBackGradientEndColor7")]
		[DefaultValue(typeof(Color), "")]
		public Color BackGradientEndColor
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.BackGradientEndColor))
					{
						return (Color)GetAttributeObject(CommonAttributes.BackGradientEndColor);
					}
					if (IsSerializing())
					{
						return Color.Empty;
					}
					if (emptyPoint)
					{
						return (Color)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.BackGradientEndColor);
					}
					return series.backGradientEndColor;
				}
				return series.backGradientEndColor;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.BackGradientEndColor, value);
				}
				else
				{
					series.backGradientEndColor = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBackHatchStyle9")]
		[DefaultValue(ChartHatchStyle.None)]
		public ChartHatchStyle BackHatchStyle
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.BackHatchStyle))
					{
						return (ChartHatchStyle)GetAttributeObject(CommonAttributes.BackHatchStyle);
					}
					if (IsSerializing())
					{
						return ChartHatchStyle.None;
					}
					if (emptyPoint)
					{
						return (ChartHatchStyle)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.BackHatchStyle);
					}
					return series.backHatchStyle;
				}
				return series.backHatchStyle;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.BackHatchStyle, value);
				}
				else
				{
					series.backHatchStyle = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeLabelAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeFont")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		public Font Font
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.Font))
					{
						return (Font)GetAttributeObject(CommonAttributes.Font);
					}
					if (IsSerializing())
					{
						return new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);
					}
					if (emptyPoint)
					{
						return (Font)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.Font);
					}
					return series.font;
				}
				return series.font;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.Font, value);
				}
				else
				{
					series.font = value;
				}
				Invalidate(invalidateLegend: false);
			}
		}

		[SRCategory("CategoryAttributeLabelAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeFontColor")]
		[DefaultValue(typeof(Color), "Black")]
		public Color FontColor
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.FontColor))
					{
						return (Color)GetAttributeObject(CommonAttributes.FontColor);
					}
					if (IsSerializing())
					{
						return Color.Black;
					}
					if (emptyPoint)
					{
						return (Color)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.FontColor);
					}
					return series.fontColor;
				}
				return series.fontColor;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.FontColor, value);
				}
				else
				{
					series.fontColor = value;
				}
				Invalidate(invalidateLegend: false);
			}
		}

		[SRCategory("CategoryAttributeLabelAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeFontAngle3")]
		[DefaultValue(0)]
		public int FontAngle
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.FontAngle))
					{
						return (int)GetAttributeObject(CommonAttributes.FontAngle);
					}
					if (IsSerializing())
					{
						return 0;
					}
					if (emptyPoint)
					{
						return (int)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.FontAngle);
					}
					return series.fontAngle;
				}
				return series.fontAngle;
			}
			set
			{
				if (value < -90 || value > 90)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAngleRangeInvalid);
				}
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.FontAngle, value);
				}
				else
				{
					series.fontAngle = value;
				}
				Invalidate(invalidateLegend: false);
			}
		}

		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMarkerStyle4")]
		[DefaultValue(MarkerStyle.None)]
		[RefreshProperties(RefreshProperties.All)]
		public MarkerStyle MarkerStyle
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.MarkerStyle))
					{
						return (MarkerStyle)GetAttributeObject(CommonAttributes.MarkerStyle);
					}
					if (IsSerializing())
					{
						return MarkerStyle.None;
					}
					if (emptyPoint)
					{
						return (MarkerStyle)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.MarkerStyle);
					}
					return series.markerStyle;
				}
				return series.markerStyle;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.MarkerStyle, value);
				}
				else
				{
					series.markerStyle = value;
				}
				if (this is Series)
				{
					((Series)this).tempMarkerStyleIsSet = false;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMarkerSize")]
		[DefaultValue(5)]
		[RefreshProperties(RefreshProperties.All)]
		public int MarkerSize
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.MarkerSize))
					{
						return (int)GetAttributeObject(CommonAttributes.MarkerSize);
					}
					if (IsSerializing())
					{
						return 5;
					}
					if (emptyPoint)
					{
						return (int)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.MarkerSize);
					}
					return series.markerSize;
				}
				return series.markerSize;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.MarkerSize, value);
				}
				else
				{
					series.markerSize = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMarkerImage10")]
		[DefaultValue("")]
		[RefreshProperties(RefreshProperties.All)]
		public string MarkerImage
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.MarkerImage))
					{
						return (string)GetAttributeObject(CommonAttributes.MarkerImage);
					}
					if (IsSerializing())
					{
						return "";
					}
					if (emptyPoint)
					{
						return (string)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.MarkerImage);
					}
					return series.markerImage;
				}
				return series.markerImage;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.MarkerImage, value);
				}
				else
				{
					series.markerImage = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMarkerImageTransparentColor3")]
		[DefaultValue(typeof(Color), "")]
		[RefreshProperties(RefreshProperties.All)]
		public Color MarkerImageTransparentColor
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.MarkerImageTransparentColor))
					{
						return (Color)GetAttributeObject(CommonAttributes.MarkerImageTransparentColor);
					}
					if (IsSerializing())
					{
						return Color.Empty;
					}
					if (emptyPoint)
					{
						return (Color)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.MarkerImageTransparentColor);
					}
					return series.markerImageTranspColor;
				}
				return series.markerImageTranspColor;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.MarkerImageTransparentColor, value);
				}
				else
				{
					series.markerImageTranspColor = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMarkerColor3")]
		[DefaultValue(typeof(Color), "")]
		[RefreshProperties(RefreshProperties.All)]
		public Color MarkerColor
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.MarkerColor))
					{
						return (Color)GetAttributeObject(CommonAttributes.MarkerColor);
					}
					if (IsSerializing())
					{
						return Color.Empty;
					}
					if (emptyPoint)
					{
						return (Color)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.MarkerColor);
					}
					return series.markerColor;
				}
				return series.markerColor;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.MarkerColor, value);
				}
				else
				{
					series.markerColor = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMarkerBorderColor")]
		[DefaultValue(typeof(Color), "")]
		[RefreshProperties(RefreshProperties.All)]
		public Color MarkerBorderColor
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.MarkerBorderColor))
					{
						return (Color)GetAttributeObject(CommonAttributes.MarkerBorderColor);
					}
					if (IsSerializing())
					{
						return Color.Empty;
					}
					if (emptyPoint)
					{
						return (Color)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.MarkerBorderColor);
					}
					return series.markerBorderColor;
				}
				return series.markerBorderColor;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.MarkerBorderColor, value);
				}
				else
				{
					series.markerBorderColor = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMarkerBorderWidth3")]
		[DefaultValue(1)]
		public int MarkerBorderWidth
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.MarkerBorderWidth))
					{
						return (int)GetAttributeObject(CommonAttributes.MarkerBorderWidth);
					}
					if (IsSerializing())
					{
						return 1;
					}
					if (emptyPoint)
					{
						return (int)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.MarkerBorderWidth);
					}
					return series.markerBorderWidth;
				}
				return series.markerBorderWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionBorderWidthIsNotPositive);
				}
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.MarkerBorderWidth, value);
				}
				else
				{
					series.markerBorderWidth = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(false)]
		[SRDescription("DescriptionAttributeCustomAttributesExtended")]
		[DefaultValue(null)]
		[RefreshProperties(RefreshProperties.All)]
		[NotifyParentProperty(true)]
		[DesignOnly(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public CustomAttributes CustomAttributesExtended
		{
			get
			{
				return customAttributes;
			}
			set
			{
				customAttributes = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeCustomAttributesExtended")]
		[DefaultValue("")]
		public string CustomAttributes
		{
			get
			{
				string text = "";
				string[] names = Enum.GetNames(typeof(CommonAttributes));
				for (int num = attributes.Count - 1; num >= 0; num--)
				{
					if (this[num] != null)
					{
						string text2 = this[num];
						bool flag = true;
						string[] array = names;
						foreach (string strB in array)
						{
							if (string.Compare(text2, strB, StringComparison.OrdinalIgnoreCase) == 0)
							{
								flag = false;
								break;
							}
						}
						if (flag && attributes[text2] != null)
						{
							if (text.Length > 0)
							{
								text += ", ";
							}
							string text3 = attributes[text2].ToString().Replace(",", "\\,");
							text3 = text3.Replace("=", "\\=");
							text = text + text2 + "=" + text3;
						}
					}
				}
				return text;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				Hashtable hashtable = new Hashtable();
				foreach (object value2 in Enum.GetValues(typeof(CommonAttributes)))
				{
					if (IsAttributeSet((CommonAttributes)value2))
					{
						hashtable[(int)value2] = attributes[(int)value2];
					}
				}
				if (value.Length > 0)
				{
					value = value.Replace("\\,", "\\x45");
					value = value.Replace("\\=", "\\x46");
					string[] array = value.Split(',');
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split('=');
						if (array2.Length != 2)
						{
							throw new FormatException(SR.ExceptionAttributeInvalidFormat);
						}
						array2[0] = array2[0].Trim();
						array2[1] = array2[1].Trim();
						if (array2[0].Length == 0)
						{
							throw new FormatException(SR.ExceptionAttributeInvalidFormat);
						}
						foreach (object key in hashtable.Keys)
						{
							if (key is string && string.Compare((string)key, array2[0], StringComparison.OrdinalIgnoreCase) == 0)
							{
								throw new FormatException(SR.ExceptionAttributeNameIsNotUnique(array2[0]));
							}
						}
						string text = array2[1].Replace("\\x45", ",");
						hashtable[array2[0]] = text.Replace("\\x46", "=");
					}
				}
				attributes = hashtable;
				Invalidate(invalidateLegend: true);
			}
		}

		[Browsable(false)]
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

		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeToolTip7")]
		[DefaultValue("")]
		public string ToolTip
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.ToolTip))
					{
						return (string)GetAttributeObject(CommonAttributes.ToolTip);
					}
					if (IsSerializing())
					{
						return "";
					}
					if (emptyPoint)
					{
						return (string)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.ToolTip);
					}
					return series.toolTip;
				}
				return series.toolTip;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.ToolTip, value);
				}
				else
				{
					series.toolTip = value;
				}
			}
		}

		object IMapAreaAttributes.Tag
		{
			get
			{
				if (pointAttributes)
				{
					if (mapAreaTag == null && series != null)
					{
						if (emptyPoint)
						{
							return series.EmptyPointStyle.mapAreaTag;
						}
						return series.mapAreaTag;
					}
					return mapAreaTag;
				}
				return series.mapAreaTag;
			}
			set
			{
				mapAreaTag = value;
			}
		}

		public object LegendTag
		{
			get
			{
				if (pointAttributes)
				{
					if (mapAreaLegendTag == null && series != null)
					{
						return series.mapAreaLegendTag;
					}
					return mapAreaLegendTag;
				}
				return series.mapAreaLegendTag;
			}
			set
			{
				mapAreaLegendTag = value;
			}
		}

		public object LabelTag
		{
			get
			{
				if (pointAttributes)
				{
					if (mapAreaLabelTag == null && series != null)
					{
						if (emptyPoint)
						{
							return series.EmptyPointStyle.mapAreaLabelTag;
						}
						return series.mapAreaLabelTag;
					}
					return mapAreaLabelTag;
				}
				return series.mapAreaLabelTag;
			}
			set
			{
				mapAreaLabelTag = value;
			}
		}

		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeHref7")]
		[DefaultValue("")]
		public string Href
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.Href))
					{
						return (string)GetAttributeObject(CommonAttributes.Href);
					}
					if (IsSerializing())
					{
						return "";
					}
					if (emptyPoint)
					{
						return (string)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.Href);
					}
					return series.href;
				}
				return series.href;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.Href, value);
				}
				else
				{
					series.href = value;
				}
			}
		}

		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMapAreaAttributes9")]
		[DefaultValue("")]
		public string MapAreaAttributes
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.MapAreaAttributes))
					{
						return (string)GetAttributeObject(CommonAttributes.MapAreaAttributes);
					}
					if (IsSerializing())
					{
						return "";
					}
					if (emptyPoint)
					{
						return (string)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.MapAreaAttributes);
					}
					return series.mapAreaAttributes;
				}
				return series.mapAreaAttributes;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.MapAreaAttributes, value);
				}
				else
				{
					series.mapAreaAttributes = value;
				}
			}
		}

		[SRCategory("CategoryAttributeLegend")]
		[Browsable(false)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeShowInLegend")]
		[DefaultValue(true)]
		public bool ShowInLegend
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.ShowInLegend))
					{
						return (bool)GetAttributeObject(CommonAttributes.ShowInLegend);
					}
					if (IsSerializing())
					{
						return true;
					}
					if (emptyPoint)
					{
						return (bool)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.ShowInLegend);
					}
					return series.showInLegend;
				}
				return series.showInLegend;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.ShowInLegend, value);
				}
				else
				{
					series.showInLegend = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeLegend")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegendText")]
		[DefaultValue("")]
		public string LegendText
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.LegendText))
					{
						return (string)GetAttributeObject(CommonAttributes.LegendText);
					}
					if (IsSerializing())
					{
						return "";
					}
					if (emptyPoint)
					{
						return (string)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LegendText);
					}
					return series.legendText;
				}
				return series.legendText;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.LegendText, value);
				}
				else
				{
					series.legendText = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeLegend")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegendToolTip")]
		[DefaultValue("")]
		public string LegendToolTip
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.LegendToolTip))
					{
						return (string)GetAttributeObject(CommonAttributes.LegendToolTip);
					}
					if (IsSerializing())
					{
						return "";
					}
					if (emptyPoint)
					{
						return (string)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LegendToolTip);
					}
					return series.legendToolTip;
				}
				return series.legendToolTip;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.LegendToolTip, value);
				}
				else
				{
					series.legendToolTip = value;
				}
			}
		}

		[SRCategory("CategoryAttributeLabelAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLabelBackColor")]
		[DefaultValue(typeof(Color), "")]
		public Color LabelBackColor
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.LabelBackColor))
					{
						return (Color)GetAttributeObject(CommonAttributes.LabelBackColor);
					}
					if (IsSerializing())
					{
						return Color.Empty;
					}
					if (emptyPoint)
					{
						return (Color)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LabelBackColor);
					}
					return series.labelBackColor;
				}
				return series.labelBackColor;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.LabelBackColor, value);
				}
				else
				{
					series.labelBackColor = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeLabelAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLabelBorderColor")]
		[DefaultValue(typeof(Color), "")]
		public Color LabelBorderColor
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.LabelBorderColor))
					{
						return (Color)GetAttributeObject(CommonAttributes.LabelBorderColor);
					}
					if (IsSerializing())
					{
						return Color.Empty;
					}
					if (emptyPoint)
					{
						return (Color)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LabelBorderColor);
					}
					return series.labelBorderColor;
				}
				return series.labelBorderColor;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.LabelBorderColor, value);
				}
				else
				{
					series.labelBorderColor = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeLabelAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLabelBorderStyle")]
		[DefaultValue(ChartDashStyle.Solid)]
		public ChartDashStyle LabelBorderStyle
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.LabelBorderDashStyle))
					{
						return (ChartDashStyle)GetAttributeObject(CommonAttributes.LabelBorderDashStyle);
					}
					if (IsSerializing())
					{
						return ChartDashStyle.Solid;
					}
					if (emptyPoint)
					{
						return (ChartDashStyle)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LabelBorderDashStyle);
					}
					return series.labelBorderStyle;
				}
				return series.labelBorderStyle;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.LabelBorderDashStyle, value);
				}
				else
				{
					series.labelBorderStyle = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeLabelAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLabelBorderWidth")]
		[DefaultValue(1)]
		public int LabelBorderWidth
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.LabelBorderWidth))
					{
						return (int)GetAttributeObject(CommonAttributes.LabelBorderWidth);
					}
					if (IsSerializing())
					{
						return 1;
					}
					if (emptyPoint)
					{
						return (int)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LabelBorderWidth);
					}
					return series.labelBorderWidth;
				}
				return series.labelBorderWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionLabelBorderIsNotPositive);
				}
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.LabelBorderWidth, value);
				}
				else
				{
					series.labelBorderWidth = value;
				}
				Invalidate(invalidateLegend: true);
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeLabel")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLabelToolTip")]
		[DefaultValue("")]
		public string LabelToolTip
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.LabelToolTip))
					{
						return (string)GetAttributeObject(CommonAttributes.LabelToolTip);
					}
					if (IsSerializing())
					{
						return "";
					}
					if (emptyPoint)
					{
						return (string)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LabelToolTip);
					}
					return series.labelToolTip;
				}
				return series.labelToolTip;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.LabelToolTip, value);
				}
				else
				{
					series.labelToolTip = value;
				}
			}
		}

		[SRCategory("CategoryAttributeLegend")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegendHref")]
		[DefaultValue("")]
		public string LegendHref
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.LegendHref))
					{
						return (string)GetAttributeObject(CommonAttributes.LegendHref);
					}
					if (IsSerializing())
					{
						return "";
					}
					if (emptyPoint)
					{
						return (string)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LegendHref);
					}
					return series.legendHref;
				}
				return series.legendHref;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.LegendHref, value);
				}
				else
				{
					series.legendHref = value;
				}
			}
		}

		[SRCategory("CategoryAttributeLegend")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegendMapAreaAttributes")]
		[DefaultValue("")]
		public string LegendMapAreaAttributes
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.LegendMapAreaAttributes))
					{
						return (string)GetAttributeObject(CommonAttributes.LegendMapAreaAttributes);
					}
					if (IsSerializing())
					{
						return "";
					}
					if (emptyPoint)
					{
						return (string)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LegendMapAreaAttributes);
					}
					return series.legendMapAreaAttributes;
				}
				return series.legendMapAreaAttributes;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.LegendMapAreaAttributes, value);
				}
				else
				{
					series.legendMapAreaAttributes = value;
				}
			}
		}

		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLabelHref")]
		[DefaultValue("")]
		public string LabelHref
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.LabelHref))
					{
						return (string)GetAttributeObject(CommonAttributes.LabelHref);
					}
					if (IsSerializing())
					{
						return "";
					}
					if (emptyPoint)
					{
						return (string)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LabelHref);
					}
					return series.labelHref;
				}
				return series.labelHref;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.LabelHref, value);
				}
				else
				{
					series.labelHref = value;
				}
			}
		}

		[SRCategory("CategoryAttributeLabel")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLabelMapAreaAttributes")]
		[DefaultValue("")]
		public string LabelMapAreaAttributes
		{
			get
			{
				if (pointAttributes)
				{
					if (attributes.Count != 0 && IsAttributeSet(CommonAttributes.LabelMapAreaAttributes))
					{
						return (string)GetAttributeObject(CommonAttributes.LabelMapAreaAttributes);
					}
					if (IsSerializing())
					{
						return "";
					}
					if (emptyPoint)
					{
						return (string)series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LabelMapAreaAttributes);
					}
					return series.labelMapAreaAttributes;
				}
				return series.labelMapAreaAttributes;
			}
			set
			{
				if (pointAttributes)
				{
					SetAttributeObject(CommonAttributes.LabelMapAreaAttributes, value);
				}
				else
				{
					series.labelMapAreaAttributes = value;
				}
			}
		}

		[Bindable(false)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeEmptyX")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public bool EmptyX
		{
			get
			{
				if (IsAttributeSet("EmptyX"))
				{
					object attributeObject = GetAttributeObject(CommonAttributes.EmptyX);
					if (attributeObject is bool)
					{
						return (bool)attributeObject;
					}
					if (attributeObject is string)
					{
						return bool.Parse((string)attributeObject);
					}
				}
				return false;
			}
			set
			{
				SetAttributeObject(CommonAttributes.EmptyX, value);
			}
		}

		[Bindable(false)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeElementId")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public int ElementId
		{
			get
			{
				if (IsAttributeSet(CommonAttributes.ElementID))
				{
					object attributeObject = GetAttributeObject(CommonAttributes.ElementID);
					if (attributeObject is int)
					{
						return (int)attributeObject;
					}
					if (attributeObject is string)
					{
						return int.Parse((string)attributeObject, CultureInfo.InvariantCulture);
					}
				}
				return 0;
			}
			set
			{
				SetAttributeObject(CommonAttributes.ElementID, value);
			}
		}

		public DataPointAttributes()
		{
			series = null;
			customAttributes = new CustomAttributes(this);
		}

		public DataPointAttributes(Series series, bool pointAttributes)
		{
			this.series = series;
			this.pointAttributes = pointAttributes;
			customAttributes = new CustomAttributes(this);
		}

		public virtual bool IsAttributeSet(string name)
		{
			return attributes.ContainsKey(name);
		}

		internal bool IsAttributeSet(CommonAttributes attrib)
		{
			return attributes.ContainsKey((int)attrib);
		}

		public virtual void DeleteAttribute(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(SR.ExceptionAttributeNameIsEmpty);
			}
			string[] names = Enum.GetNames(typeof(CommonAttributes));
			foreach (string text in names)
			{
				if (name == text)
				{
					DeleteAttribute((CommonAttributes)Enum.Parse(typeof(CommonAttributes), text));
				}
			}
			attributes.Remove(name);
		}

		internal void DeleteAttribute(CommonAttributes attrib)
		{
			if (!pointAttributes)
			{
				throw new ArgumentException(SR.ExceptionAttributeUnableToDelete);
			}
			attributes.Remove((int)attrib);
		}

		public virtual string GetAttribute(string name)
		{
			if (!IsAttributeSet(name) && pointAttributes)
			{
				bool flag = false;
				if (series.chart == null && series.serviceContainer != null)
				{
					series.chart = (Chart)series.serviceContainer.GetService(typeof(Chart));
				}
				if (series.chart != null && series.chart.serializing)
				{
					flag = true;
				}
				if (!flag)
				{
					if (emptyPoint)
					{
						return (string)series.EmptyPointStyle.attributes[name];
					}
					return (string)series.attributes[name];
				}
				return Series.defaultAttributes[name];
			}
			return (string)attributes[name];
		}

		internal bool IsSerializing()
		{
			if (series == null)
			{
				return true;
			}
			if (series.chart == null)
			{
				if (series.serviceContainer != null)
				{
					series.chart = (Chart)series.serviceContainer.GetService(typeof(Chart));
					if (series.chart != null)
					{
						return series.chart.serializing;
					}
				}
				return false;
			}
			return series.chart.serializing;
		}

		internal object GetAttributeObject(CommonAttributes attrib)
		{
			if (!pointAttributes || series == null)
			{
				return attributes[(int)attrib];
			}
			if (attributes.Count == 0 || !IsAttributeSet(attrib))
			{
				bool flag = false;
				if (series.chart == null)
				{
					if (series.serviceContainer != null)
					{
						series.chart = (Chart)series.serviceContainer.GetService(typeof(Chart));
						if (series.chart != null)
						{
							flag = series.chart.serializing;
						}
					}
				}
				else
				{
					flag = series.chart.serializing;
				}
				if (!flag)
				{
					if (emptyPoint)
					{
						return series.EmptyPointStyle.attributes[(int)attrib];
					}
					return series.attributes[(int)attrib];
				}
				return Series.defaultAttributes.attributes[(int)attrib];
			}
			return attributes[(int)attrib];
		}

		public virtual void SetAttribute(string name, string attributeValue)
		{
			attributes[name] = attributeValue;
		}

		internal void SetAttributeObject(CommonAttributes attrib, object attributeValue)
		{
			attributes[(int)attrib] = attributeValue;
		}

		public virtual void SetDefault(bool clearAll)
		{
			if (!pointAttributes)
			{
				if (clearAll)
				{
					attributes.Clear();
				}
				if (!IsAttributeSet(CommonAttributes.ToolTip))
				{
					SetAttributeObject(CommonAttributes.ToolTip, "");
				}
				if (!IsAttributeSet(CommonAttributes.LegendToolTip))
				{
					SetAttributeObject(CommonAttributes.LegendToolTip, "");
				}
				if (!IsAttributeSet(CommonAttributes.Color))
				{
					SetAttributeObject(CommonAttributes.Color, Color.Empty);
				}
				if (!IsAttributeSet(CommonAttributes.ShowLabelAsValue))
				{
					SetAttributeObject(CommonAttributes.ShowLabelAsValue, false);
				}
				if (!IsAttributeSet(CommonAttributes.MarkerStyle))
				{
					SetAttributeObject(CommonAttributes.MarkerStyle, MarkerStyle.None);
				}
				if (!IsAttributeSet(CommonAttributes.MarkerSize))
				{
					SetAttributeObject(CommonAttributes.MarkerSize, 5);
				}
				if (!IsAttributeSet(CommonAttributes.MarkerImage))
				{
					SetAttributeObject(CommonAttributes.MarkerImage, "");
				}
				if (!IsAttributeSet(CommonAttributes.Label))
				{
					SetAttributeObject(CommonAttributes.Label, "");
				}
				if (!IsAttributeSet(CommonAttributes.BorderWidth))
				{
					SetAttributeObject(CommonAttributes.BorderWidth, 1);
				}
				if (!IsAttributeSet(CommonAttributes.BorderDashStyle))
				{
					SetAttributeObject(CommonAttributes.BorderDashStyle, ChartDashStyle.Solid);
				}
				if (!IsAttributeSet(CommonAttributes.AxisLabel))
				{
					SetAttributeObject(CommonAttributes.AxisLabel, "");
				}
				if (!IsAttributeSet(CommonAttributes.LabelFormat))
				{
					SetAttributeObject(CommonAttributes.LabelFormat, "");
				}
				if (!IsAttributeSet(CommonAttributes.BorderColor))
				{
					SetAttributeObject(CommonAttributes.BorderColor, Color.Empty);
				}
				if (!IsAttributeSet(CommonAttributes.BackImage))
				{
					SetAttributeObject(CommonAttributes.BackImage, "");
				}
				if (!IsAttributeSet(CommonAttributes.BackImageMode))
				{
					SetAttributeObject(CommonAttributes.BackImageMode, ChartImageWrapMode.Tile);
				}
				if (!IsAttributeSet(CommonAttributes.BackImageAlign))
				{
					SetAttributeObject(CommonAttributes.BackImageAlign, ChartImageAlign.TopLeft);
				}
				if (!IsAttributeSet(CommonAttributes.BackImageTransparentColor))
				{
					SetAttributeObject(CommonAttributes.BackImageTransparentColor, Color.Empty);
				}
				if (!IsAttributeSet(CommonAttributes.BackGradientType))
				{
					SetAttributeObject(CommonAttributes.BackGradientType, GradientType.None);
				}
				if (!IsAttributeSet(CommonAttributes.BackGradientEndColor))
				{
					SetAttributeObject(CommonAttributes.BackGradientEndColor, Color.Empty);
				}
				if (!IsAttributeSet(CommonAttributes.BackHatchStyle))
				{
					SetAttributeObject(CommonAttributes.BackHatchStyle, ChartHatchStyle.None);
				}
				if (!IsAttributeSet(CommonAttributes.Font))
				{
					SetAttributeObject(CommonAttributes.Font, new Font(ChartPicture.GetDefaultFontFamilyName(), 8f));
				}
				if (!IsAttributeSet(CommonAttributes.FontColor))
				{
					SetAttributeObject(CommonAttributes.FontColor, Color.Black);
				}
				if (!IsAttributeSet(CommonAttributes.FontAngle))
				{
					SetAttributeObject(CommonAttributes.FontAngle, 0);
				}
				if (!IsAttributeSet(CommonAttributes.MarkerImageTransparentColor))
				{
					SetAttributeObject(CommonAttributes.MarkerImageTransparentColor, Color.Empty);
				}
				if (!IsAttributeSet(CommonAttributes.MarkerColor))
				{
					SetAttributeObject(CommonAttributes.MarkerColor, Color.Empty);
				}
				if (!IsAttributeSet(CommonAttributes.MarkerBorderColor))
				{
					SetAttributeObject(CommonAttributes.MarkerBorderColor, Color.Empty);
				}
				if (!IsAttributeSet(CommonAttributes.MarkerBorderWidth))
				{
					SetAttributeObject(CommonAttributes.MarkerBorderWidth, 1);
				}
				if (!IsAttributeSet(CommonAttributes.MapAreaAttributes))
				{
					SetAttributeObject(CommonAttributes.MapAreaAttributes, "");
				}
				if (!IsAttributeSet(CommonAttributes.LabelToolTip))
				{
					SetAttributeObject(CommonAttributes.LabelToolTip, "");
				}
				if (!IsAttributeSet(CommonAttributes.LabelHref))
				{
					SetAttributeObject(CommonAttributes.LabelHref, "");
				}
				if (!IsAttributeSet(CommonAttributes.LabelMapAreaAttributes))
				{
					SetAttributeObject(CommonAttributes.LabelMapAreaAttributes, "");
				}
				if (!IsAttributeSet(CommonAttributes.LabelBackColor))
				{
					SetAttributeObject(CommonAttributes.LabelBackColor, Color.Empty);
				}
				if (!IsAttributeSet(CommonAttributes.LabelBorderWidth))
				{
					SetAttributeObject(CommonAttributes.LabelBorderWidth, 1);
				}
				if (!IsAttributeSet(CommonAttributes.LabelBorderDashStyle))
				{
					SetAttributeObject(CommonAttributes.LabelBorderDashStyle, ChartDashStyle.Solid);
				}
				if (!IsAttributeSet(CommonAttributes.LabelBorderColor))
				{
					SetAttributeObject(CommonAttributes.LabelBorderColor, Color.Empty);
				}
				if (!IsAttributeSet(CommonAttributes.MapAreaID))
				{
					SetAttributeObject(CommonAttributes.MapAreaID, 0);
				}
				if (!IsAttributeSet(CommonAttributes.ElementID))
				{
					SetAttributeObject(CommonAttributes.ElementID, 0);
				}
				if (!IsAttributeSet(CommonAttributes.EmptyX))
				{
					SetAttributeObject(CommonAttributes.EmptyX, false);
				}
				if (!IsAttributeSet(CommonAttributes.Href))
				{
					SetAttributeObject(CommonAttributes.Href, "");
				}
				if (!IsAttributeSet(CommonAttributes.LegendHref))
				{
					SetAttributeObject(CommonAttributes.LegendHref, "");
				}
				if (!IsAttributeSet(CommonAttributes.LegendText))
				{
					SetAttributeObject(CommonAttributes.LegendText, "");
				}
				if (!IsAttributeSet(CommonAttributes.LegendMapAreaAttributes))
				{
					SetAttributeObject(CommonAttributes.LegendMapAreaAttributes, "");
				}
				if (!IsAttributeSet(CommonAttributes.ShowInLegend))
				{
					SetAttributeObject(CommonAttributes.ShowInLegend, true);
				}
			}
			else
			{
				attributes.Clear();
			}
		}

		internal void Invalidate(bool invalidateLegend)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public PropertyDescriptorCollection GetProperties()
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this, noCustomTypeDesc: true);
			Series series = (this is Series) ? ((Series)this) : this.series;
			if (series != null && series.chart != null)
			{
				PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
				{
					foreach (PropertyDescriptor item in properties)
					{
						if (item.Name == "CustomAttributesEx")
						{
							DynamicPropertyDescriptor value = new DynamicPropertyDescriptor(item, "CustomAttributes");
							propertyDescriptorCollection.Add(value);
						}
						else
						{
							propertyDescriptorCollection.Add(item);
						}
					}
					return propertyDescriptorCollection;
				}
			}
			return properties;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return GetProperties();
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, noCustomTypeDesc: true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, noCustomTypeDesc: true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, noCustomTypeDesc: true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, noCustomTypeDesc: true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, noCustomTypeDesc: true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, noCustomTypeDesc: true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, noCustomTypeDesc: true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, noCustomTypeDesc: true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, noCustomTypeDesc: true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		public DataPointAttributes CloneAttributes()
		{
			DataPointAttributes dataPointAttributes = new DataPointAttributes();
			dataPointAttributes.pointAttributes = pointAttributes;
			dataPointAttributes.emptyPoint = emptyPoint;
			foreach (object key in attributes.Keys)
			{
				dataPointAttributes.attributes.Add(key, attributes[key]);
			}
			return dataPointAttributes;
		}
	}
}

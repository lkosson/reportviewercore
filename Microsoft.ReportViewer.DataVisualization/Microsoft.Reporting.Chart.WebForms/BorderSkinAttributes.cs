using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	[DefaultProperty("SkinStyle")]
	[SRDescription("DescriptionAttributeBorderSkinAttributes_BorderSkinAttributes")]
	internal class BorderSkinAttributes
	{
		internal IServiceContainer serviceContainer;

		private Color pageColor = Color.White;

		private BorderSkinStyle skinStyle;

		private GradientType backGradientType;

		private Color backGradientEndColor = Color.Empty;

		private Color backColor = Color.Gray;

		private string backImage = "";

		private ChartImageWrapMode backImageMode;

		private Color backImageTranspColor = Color.Empty;

		private ChartImageAlign backImageAlign;

		private Color borderColor = Color.Black;

		private int borderWidth = 1;

		private ChartDashStyle borderStyle;

		private ChartHatchStyle backHatchStyle;

		internal object ownerElement;

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "White")]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_PageColor")]
		public Color PageColor
		{
			get
			{
				return pageColor;
			}
			set
			{
				pageColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(BorderSkinStyle.None)]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_SkinStyle")]
		[ParenthesizePropertyName(true)]
		public BorderSkinStyle SkinStyle
		{
			get
			{
				return skinStyle;
			}
			set
			{
				skinStyle = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Gray")]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBackColor")]
		public Color FrameBackColor
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

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBorderColor")]
		public Color FrameBorderColor
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

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(ChartHatchStyle.None)]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBackHatchStyle")]
		public ChartHatchStyle FrameBackHatchStyle
		{
			get
			{
				return backHatchStyle;
			}
			set
			{
				backHatchStyle = value;
				Invalidate();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBackImage")]
		public string FrameBackImage
		{
			get
			{
				return backImage;
			}
			set
			{
				backImage = value;
				Invalidate();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(ChartImageWrapMode.Tile)]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBackImageMode")]
		public ChartImageWrapMode FrameBackImageMode
		{
			get
			{
				return backImageMode;
			}
			set
			{
				backImageMode = value;
				Invalidate();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBackImageTransparentColor")]
		public Color FrameBackImageTransparentColor
		{
			get
			{
				return backImageTranspColor;
			}
			set
			{
				backImageTranspColor = value;
				Invalidate();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(ChartImageAlign.TopLeft)]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBackImageAlign")]
		public ChartImageAlign FrameBackImageAlign
		{
			get
			{
				return backImageAlign;
			}
			set
			{
				backImageAlign = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(GradientType.None)]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBackGradientType")]
		public GradientType FrameBackGradientType
		{
			get
			{
				return backGradientType;
			}
			set
			{
				backGradientType = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBackGradientEndColor")]
		public Color FrameBackGradientEndColor
		{
			get
			{
				return backGradientEndColor;
			}
			set
			{
				backGradientEndColor = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBorderWidth")]
		public int FrameBorderWidth
		{
			get
			{
				return borderWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionBorderWidthIsNotPositive);
				}
				borderWidth = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(ChartDashStyle.NotSet)]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBorderStyle")]
		public ChartDashStyle FrameBorderStyle
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

		public BorderSkinAttributes()
		{
		}

		internal BorderSkinAttributes(IServiceContainer container)
		{
			serviceContainer = container;
		}

		private void Invalidate()
		{
		}
	}
}

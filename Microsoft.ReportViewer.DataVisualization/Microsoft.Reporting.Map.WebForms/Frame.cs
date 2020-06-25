using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(FrameAttributesConverter))]
	[DefaultProperty("FrameStyle")]
	[Description("Drawing attributes for the 3D frames.")]
	internal class Frame : MapObject
	{
		private Color pageColor = Color.White;

		private FrameStyle frameStyle;

		private GradientType backGradientType;

		private Color backSecondaryColor = Color.Empty;

		private Color backColor = Color.Gray;

		private string backImage = "";

		private MapImageWrapMode backImageMode;

		private Color backImageTranspColor = Color.Empty;

		private MapImageAlign backImageAlign;

		private Color borderColor = Color.DarkGray;

		private int borderWidth = 1;

		private MapDashStyle borderStyle;

		private MapHatchStyle backHatchStyle;

		internal object ownerElement;

		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "White")]
		[SRDescription("DescriptionAttributeFrame_PageColor")]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(FrameStyle.None)]
		[SRDescription("DescriptionAttributeFrame_FrameStyle")]
		[ParenthesizePropertyName(true)]
		public FrameStyle FrameStyle
		{
			get
			{
				return frameStyle;
			}
			set
			{
				frameStyle = value;
				InvalidateAndLayout();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Gray")]
		[SRDescription("DescriptionAttributeFrame_BackColor")]
		public Color BackColor
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
		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "DarkGray")]
		[SRDescription("DescriptionAttributeFrame_BorderColor")]
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

		[Browsable(false)]
		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(MapHatchStyle.None)]
		[SRDescription("DescriptionAttributeFrame_BackHatchStyle")]
		public MapHatchStyle BackHatchStyle
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
		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeFrame_BackImage")]
		public string BackImage
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
		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(MapImageWrapMode.Tile)]
		[SRDescription("DescriptionAttributeFrame_BackImageMode")]
		public MapImageWrapMode BackImageMode
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
		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeFrame_BackImageTranspColor")]
		public Color BackImageTranspColor
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
		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(MapImageAlign.TopLeft)]
		[SRDescription("DescriptionAttributeFrame_BackImageAlign")]
		public MapImageAlign BackImageAlign
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

		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(GradientType.None)]
		[SRDescription("DescriptionAttributeFrame_BackGradientType")]
		public GradientType BackGradientType
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
		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeFrame_BackSecondaryColor")]
		public Color BackSecondaryColor
		{
			get
			{
				return backSecondaryColor;
			}
			set
			{
				backSecondaryColor = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeFrame_BorderWidth")]
		public int BorderWidth
		{
			get
			{
				return borderWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("BorderWidth", SR.ExceptionBorderWidthMustBeGreaterThanZero);
				}
				borderWidth = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(MapDashStyle.None)]
		[SRDescription("DescriptionAttributeFrame_BorderStyle")]
		public MapDashStyle BorderStyle
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

		public Frame()
			: this(null)
		{
		}

		public Frame(object parent)
			: base(parent)
		{
		}

		internal MapCore GetMapCore()
		{
			return (MapCore)Parent;
		}

		internal override void Invalidate()
		{
			GetMapCore()?.Invalidate();
		}

		internal void InvalidateAndLayout()
		{
			GetMapCore()?.InvalidateAndLayout();
		}

		internal bool ShouldRenderReadOnly()
		{
			if (FrameStyle != 0 && FrameStyle != FrameStyle.Raised && FrameStyle != FrameStyle.Sunken)
			{
				return FrameStyle == FrameStyle.Emboss;
			}
			return true;
		}
	}
}

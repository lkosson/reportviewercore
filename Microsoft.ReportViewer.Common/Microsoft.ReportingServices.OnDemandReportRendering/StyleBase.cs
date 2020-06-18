using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class StyleBase
	{
		public const int StyleAttributeCount = 51;

		protected const string cBorderColor = "BorderColor";

		protected const string cBorderColorLeft = "BorderColorLeft";

		protected const string cBorderColorRight = "BorderColorRight";

		protected const string cBorderColorTop = "BorderColorTop";

		protected const string cBorderColorBottom = "BorderColorBottom";

		protected const string cBorderStyle = "BorderStyle";

		protected const string cBorderStyleLeft = "BorderStyleLeft";

		protected const string cBorderStyleRight = "BorderStyleRight";

		protected const string cBorderStyleTop = "BorderStyleTop";

		protected const string cBorderStyleBottom = "BorderStyleBottom";

		protected const string cBorderWidth = "BorderWidth";

		protected const string cBorderWidthLeft = "BorderWidthLeft";

		protected const string cBorderWidthRight = "BorderWidthRight";

		protected const string cBorderWidthTop = "BorderWidthTop";

		protected const string cBorderWidthBottom = "BorderWidthBottom";

		protected const string cBackgroundImage = "BackgroundImage";

		protected const string cBackgroundImageSource = "BackgroundImageSource";

		protected const string cBackgroundImageValue = "BackgroundImageValue";

		protected const string cBackgroundImageMIMEType = "BackgroundImageMIMEType";

		protected const string cBackgroundColor = "BackgroundColor";

		protected const string cBackgroundGradientEndColor = "BackgroundGradientEndColor";

		protected const string cBackgroundGradientType = "BackgroundGradientType";

		protected const string cBackgroundRepeat = "BackgroundRepeat";

		protected const string cFontStyle = "FontStyle";

		protected const string cFontFamily = "FontFamily";

		protected const string cFontSize = "FontSize";

		protected const string cFontWeight = "FontWeight";

		protected const string cFormat = "Format";

		protected const string cTextDecoration = "TextDecoration";

		protected const string cTextAlign = "TextAlign";

		protected const string cVerticalAlign = "VerticalAlign";

		protected const string cColor = "Color";

		protected const string cPaddingLeft = "PaddingLeft";

		protected const string cPaddingRight = "PaddingRight";

		protected const string cPaddingTop = "PaddingTop";

		protected const string cPaddingBottom = "PaddingBottom";

		protected const string cLineHeight = "LineHeight";

		protected const string cDirection = "Direction";

		protected const string cWritingMode = "WritingMode";

		protected const string cLanguage = "Language";

		protected const string cUnicodeBiDi = "UnicodeBiDi";

		protected const string cCalendar = "Calendar";

		protected const string cCurrencyLanguage = "CurrencyLanguage";

		protected const string cNumeralLanguage = "NumeralLanguage";

		protected const string cNumeralVariant = "NumeralVariant";

		protected const string cTextEffect = "TextEffect";

		protected const string cBackgroundHatchType = "BackgroundHatchType";

		protected const string cShadowColor = "ShadowColor";

		protected const string cShadowOffset = "ShadowOffset";

		protected const string cPosition = "Position";

		protected const string cTransparentColor = "TransparentColor";

		internal RenderingContext m_renderingContext;

		protected List<StyleAttributeNames> m_sharedStyles;

		protected List<StyleAttributeNames> m_nonSharedStyles;

		internal static IEnumerable<StyleAttributeNames> StyleNames
		{
			get
			{
				int i = 0;
				while (i < 51)
				{
					yield return (StyleAttributeNames)i;
					int num = i + 1;
					i = num;
				}
			}
		}

		public abstract ReportProperty this[StyleAttributeNames style]
		{
			get;
		}

		public abstract List<StyleAttributeNames> SharedStyleAttributes
		{
			get;
		}

		public abstract List<StyleAttributeNames> NonSharedStyleAttributes
		{
			get;
		}

		public abstract BackgroundImage BackgroundImage
		{
			get;
		}

		public abstract Border Border
		{
			get;
		}

		public abstract Border TopBorder
		{
			get;
		}

		public abstract Border LeftBorder
		{
			get;
		}

		public abstract Border RightBorder
		{
			get;
		}

		public abstract Border BottomBorder
		{
			get;
		}

		public abstract ReportColorProperty BackgroundGradientEndColor
		{
			get;
		}

		public abstract ReportColorProperty BackgroundColor
		{
			get;
		}

		public abstract ReportColorProperty Color
		{
			get;
		}

		public abstract ReportEnumProperty<FontStyles> FontStyle
		{
			get;
		}

		public abstract ReportStringProperty FontFamily
		{
			get;
		}

		public abstract ReportEnumProperty<FontWeights> FontWeight
		{
			get;
		}

		public abstract ReportStringProperty Format
		{
			get;
		}

		public abstract ReportEnumProperty<TextDecorations> TextDecoration
		{
			get;
		}

		public abstract ReportEnumProperty<TextAlignments> TextAlign
		{
			get;
		}

		public abstract ReportEnumProperty<VerticalAlignments> VerticalAlign
		{
			get;
		}

		public abstract ReportEnumProperty<Directions> Direction
		{
			get;
		}

		public abstract ReportEnumProperty<WritingModes> WritingMode
		{
			get;
		}

		public abstract ReportStringProperty Language
		{
			get;
		}

		public abstract ReportEnumProperty<UnicodeBiDiTypes> UnicodeBiDi
		{
			get;
		}

		public abstract ReportEnumProperty<Calendars> Calendar
		{
			get;
		}

		public abstract ReportStringProperty CurrencyLanguage
		{
			get;
		}

		public abstract ReportStringProperty NumeralLanguage
		{
			get;
		}

		public abstract ReportEnumProperty<BackgroundGradients> BackgroundGradientType
		{
			get;
		}

		public abstract ReportSizeProperty FontSize
		{
			get;
		}

		public abstract ReportSizeProperty PaddingLeft
		{
			get;
		}

		public abstract ReportSizeProperty PaddingRight
		{
			get;
		}

		public abstract ReportSizeProperty PaddingTop
		{
			get;
		}

		public abstract ReportSizeProperty PaddingBottom
		{
			get;
		}

		public abstract ReportSizeProperty LineHeight
		{
			get;
		}

		public abstract ReportIntProperty NumeralVariant
		{
			get;
		}

		public abstract ReportEnumProperty<TextEffects> TextEffect
		{
			get;
		}

		public abstract ReportEnumProperty<BackgroundHatchTypes> BackgroundHatchType
		{
			get;
		}

		public abstract ReportColorProperty ShadowColor
		{
			get;
		}

		public abstract ReportSizeProperty ShadowOffset
		{
			get;
		}

		internal RenderingContext RenderingContext => m_renderingContext;

		internal StyleBase(RenderingContext context)
		{
			m_renderingContext = context;
		}
	}
}

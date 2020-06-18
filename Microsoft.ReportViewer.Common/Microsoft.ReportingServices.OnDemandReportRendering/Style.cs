using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using Microsoft.ReportingServices.ReportRendering;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class Style : StyleBase
	{
		internal sealed class StyleDefaults
		{
			private Hashtable m_nameMap;

			private string[] m_keyCollection;

			private object[] m_valueCollection;

			internal object this[int index] => m_valueCollection[index];

			internal object this[string styleName] => m_valueCollection[(int)m_nameMap[styleName]];

			internal StyleDefaults(bool isLine, string defaultFontFamily)
			{
				m_nameMap = new Hashtable(51);
				m_keyCollection = new string[51];
				m_valueCollection = new object[51];
				int num = 0;
				m_nameMap["BorderColor"] = num;
				m_keyCollection[num] = "BorderColor";
				m_valueCollection[num++] = new ReportColor("Black", System.Drawing.Color.Empty, parsed: false);
				m_nameMap["BorderColorTop"] = num;
				m_keyCollection[num] = "BorderColorTop";
				m_valueCollection[num++] = null;
				m_nameMap["BorderColorLeft"] = num;
				m_keyCollection[num] = "BorderColorLeft";
				m_valueCollection[num++] = null;
				m_nameMap["BorderColorRight"] = num;
				m_keyCollection[num] = "BorderColorRight";
				m_valueCollection[num++] = null;
				m_nameMap["BorderColorBottom"] = num;
				m_keyCollection[num] = "BorderColorBottom";
				m_valueCollection[num++] = null;
				m_nameMap["BorderStyle"] = num;
				m_keyCollection[num] = "BorderStyle";
				if (!isLine)
				{
					m_valueCollection[num++] = "None";
				}
				else
				{
					m_valueCollection[num++] = "Solid";
				}
				m_nameMap["BorderStyleTop"] = num;
				m_keyCollection[num] = "BorderStyleTop";
				m_valueCollection[num++] = null;
				m_nameMap["BorderStyleLeft"] = num;
				m_keyCollection[num] = "BorderStyleLeft";
				m_valueCollection[num++] = null;
				m_nameMap["BorderStyleRight"] = num;
				m_keyCollection[num] = "BorderStyleRight";
				m_valueCollection[num++] = null;
				m_nameMap["BorderStyleBottom"] = num;
				m_keyCollection[num] = "BorderStyleBottom";
				m_valueCollection[num++] = null;
				m_nameMap["BorderWidth"] = num;
				m_keyCollection[num] = "BorderWidth";
				m_valueCollection[num++] = new ReportSize("1pt");
				m_nameMap["BorderWidthTop"] = num;
				m_keyCollection[num] = "BorderWidthTop";
				m_valueCollection[num++] = null;
				m_nameMap["BorderWidthLeft"] = num;
				m_keyCollection[num] = "BorderWidthLeft";
				m_valueCollection[num++] = null;
				m_nameMap["BorderWidthRight"] = num;
				m_keyCollection[num] = "BorderWidthRight";
				m_valueCollection[num++] = null;
				m_nameMap["BorderWidthBottom"] = num;
				m_keyCollection[num] = "BorderWidthBottom";
				m_valueCollection[num++] = null;
				m_nameMap["BackgroundColor"] = num;
				m_keyCollection[num] = "BackgroundColor";
				m_valueCollection[num++] = new ReportColor("Transparent", System.Drawing.Color.Empty, parsed: true);
				m_nameMap["BackgroundGradientType"] = num;
				m_keyCollection[num] = "BackgroundGradientType";
				m_valueCollection[num++] = BackgroundGradients.None;
				m_nameMap["BackgroundGradientEndColor"] = num;
				m_keyCollection[num] = "BackgroundGradientEndColor";
				m_valueCollection[num++] = new ReportColor("Transparent", System.Drawing.Color.Empty, parsed: true);
				m_nameMap["BackgroundImage"] = num;
				m_keyCollection[num] = "BackgroundImage";
				m_valueCollection[num++] = null;
				m_nameMap["BackgroundRepeat"] = num;
				m_keyCollection[num] = "BackgroundRepeat";
				m_valueCollection[num++] = "Repeat";
				m_nameMap["FontStyle"] = num;
				m_keyCollection[num] = "FontStyle";
				m_valueCollection[num++] = "Normal";
				m_nameMap["FontFamily"] = num;
				m_keyCollection[num] = "FontFamily";
				m_valueCollection[num++] = (defaultFontFamily ?? "Arial");
				m_nameMap["FontSize"] = num;
				m_keyCollection[num] = "FontSize";
				m_valueCollection[num++] = new ReportSize("10pt");
				m_nameMap["FontWeight"] = num;
				m_keyCollection[num] = "FontWeight";
				m_valueCollection[num++] = "Normal";
				m_nameMap["Format"] = num;
				m_keyCollection[num] = "Format";
				m_valueCollection[num++] = null;
				m_nameMap["TextDecoration"] = num;
				m_keyCollection[num] = "TextDecoration";
				m_valueCollection[num++] = "None";
				m_nameMap["TextAlign"] = num;
				m_keyCollection[num] = "TextAlign";
				m_valueCollection[num++] = "General";
				m_nameMap["VerticalAlign"] = num;
				m_keyCollection[num] = "VerticalAlign";
				m_valueCollection[num++] = "Top";
				m_nameMap["Color"] = num;
				m_keyCollection[num] = "Color";
				m_valueCollection[num++] = new ReportColor("Black", System.Drawing.Color.Empty, parsed: false);
				m_nameMap["PaddingLeft"] = num;
				m_keyCollection[num] = "PaddingLeft";
				m_valueCollection[num++] = new ReportSize("0pt", 0.0);
				m_nameMap["PaddingRight"] = num;
				m_keyCollection[num] = "PaddingRight";
				m_valueCollection[num++] = new ReportSize("0pt", 0.0);
				m_nameMap["PaddingTop"] = num;
				m_keyCollection[num] = "PaddingTop";
				m_valueCollection[num++] = new ReportSize("0pt", 0.0);
				m_nameMap["PaddingBottom"] = num;
				m_keyCollection[num] = "PaddingBottom";
				m_valueCollection[num++] = new ReportSize("0pt", 0.0);
				m_nameMap["LineHeight"] = num;
				m_keyCollection[num] = "LineHeight";
				m_valueCollection[num++] = null;
				m_nameMap["Direction"] = num;
				m_keyCollection[num] = "Direction";
				m_valueCollection[num++] = "LTR";
				m_nameMap["WritingMode"] = num;
				m_keyCollection[num] = "WritingMode";
				m_valueCollection[num++] = "lr-tb";
				m_nameMap["Language"] = num;
				m_keyCollection[num] = "Language";
				m_valueCollection[num++] = null;
				m_nameMap["UnicodeBiDi"] = num;
				m_keyCollection[num] = "UnicodeBiDi";
				m_valueCollection[num++] = "Normal";
				m_nameMap["Calendar"] = num;
				m_keyCollection[num] = "Calendar";
				m_valueCollection[num++] = "Gregorian";
				m_nameMap["NumeralLanguage"] = num;
				m_keyCollection[num] = "NumeralLanguage";
				m_valueCollection[num++] = null;
				m_nameMap["NumeralVariant"] = num;
				m_keyCollection[num] = "NumeralVariant";
				m_valueCollection[num++] = 1;
				m_nameMap["TextEffect"] = num;
				m_keyCollection[num] = "TextEffect";
				m_valueCollection[num++] = "None";
				m_nameMap["BackgroundHatchType"] = num;
				m_keyCollection[num] = "BackgroundHatchType";
				m_valueCollection[num++] = "None";
				m_nameMap["ShadowColor"] = num;
				m_keyCollection[num] = "ShadowColor";
				m_valueCollection[num++] = new ReportColor("Transparent", System.Drawing.Color.Empty, parsed: true);
				m_nameMap["ShadowOffset"] = num;
				m_keyCollection[num] = "ShadowOffset";
				m_valueCollection[num++] = new ReportSize("0pt", 0.0);
				m_nameMap["Position"] = num;
				m_keyCollection[num] = "Position";
				m_valueCollection[num++] = "Center";
				m_nameMap["TransparentColor"] = num;
				m_keyCollection[num] = "TransparentColor";
				m_valueCollection[num++] = new ReportColor("Transparent", System.Drawing.Color.Empty, parsed: true);
				m_nameMap["BackgroundImageSource"] = num;
				m_keyCollection[num] = "BackgroundImageSource";
				m_valueCollection[num++] = Microsoft.ReportingServices.ReportRendering.Image.SourceType.External;
				m_nameMap["BackgroundImageValue"] = num;
				m_keyCollection[num] = "BackgroundImageValue";
				m_valueCollection[num++] = null;
				m_nameMap["BackgroundImageMIMEType"] = num;
				m_keyCollection[num] = "BackgroundImageMIMEType";
				m_valueCollection[num++] = null;
				m_nameMap["CurrencyLanguage"] = num;
				m_keyCollection[num] = "CurrencyLanguage";
				m_valueCollection[num++] = null;
				Global.Tracer.Assert(51 == num, "(Style.StyleAttributeCount == index)");
			}

			internal string GetName(int index)
			{
				return m_keyCollection[index];
			}
		}

		private bool m_isOldSnapshot;

		private IStyleContainer m_iStyleContainer;

		private Microsoft.ReportingServices.ReportRendering.ReportItem m_renderReportItem;

		private Microsoft.ReportingServices.ReportRendering.Style m_cachedRenderStyle;

		private bool m_isLineBorderStyle;

		private StyleDefaults m_styleDefaults;

		private BackgroundImage m_backgroundImage;

		private Border m_border;

		private Border m_topBorder;

		private Border m_rightBorder;

		private Border m_bottomBorder;

		private Border m_leftBorder;

		private StyleDefaults m_normalStyleDefaults;

		private StyleDefaults m_lineStyleDefaults;

		private IReportScope m_reportScope;

		private ReportElement m_reportElement;

		private Microsoft.ReportingServices.ReportProcessing.Style m_styleDef;

		private bool m_isDynamicImageStyle;

		private object[] m_styleValues;

		private ReportProperty[] m_cachedReportProperties = new ReportProperty[51];

		private bool m_disallowBorderTransparencyOnDynamicImage;

		internal static FontStyles DefaultEnumFontStyle = FontStyles.Normal;

		internal static FontWeights DefaultEnumFontWeight = FontWeights.Normal;

		internal static TextDecorations DefaultEnumTextDecoration = TextDecorations.None;

		internal static TextAlignments DefaultEnumTextAlignment = TextAlignments.General;

		internal static VerticalAlignments DefaultEnumVerticalAlignment = VerticalAlignments.Top;

		internal static Directions DefaultEnumDirection = Directions.LTR;

		internal static WritingModes DefaultEnumWritingMode = WritingModes.Horizontal;

		internal static UnicodeBiDiTypes DefaultEnumUnicodeBiDiType = UnicodeBiDiTypes.Normal;

		internal static Calendars DefaultEnumCalendar = Calendars.Default;

		internal static BackgroundGradients DefaultEnumBackgroundGradient = BackgroundGradients.None;

		internal static BackgroundRepeatTypes DefaultEnumBackgroundRepeatType = BackgroundRepeatTypes.Repeat;

		internal IReportScope ReportScope => m_reportScope;

		internal ReportElement ReportElement => m_reportElement;

		public override ReportProperty this[StyleAttributeNames style] => GetReportProperty(style);

		public override List<StyleAttributeNames> SharedStyleAttributes
		{
			get
			{
				if (m_sharedStyles == null)
				{
					PopulateCollections();
				}
				return m_sharedStyles;
			}
		}

		public override List<StyleAttributeNames> NonSharedStyleAttributes
		{
			get
			{
				if (m_nonSharedStyles == null)
				{
					PopulateCollections();
				}
				return m_nonSharedStyles;
			}
		}

		public override BackgroundImage BackgroundImage
		{
			get
			{
				if (m_backgroundImage == null)
				{
					m_backgroundImage = (GetReportProperty(StyleAttributeNames.BackgroundImage) as BackgroundImage);
				}
				return m_backgroundImage;
			}
		}

		public override Border Border
		{
			get
			{
				if (m_border == null)
				{
					m_border = new Border(this, Border.Position.Default, m_isLineBorderStyle);
				}
				return m_border;
			}
		}

		public override Border TopBorder
		{
			get
			{
				if (m_topBorder == null && HasBorderProperties(Border.Position.Top))
				{
					m_topBorder = new Border(this, Border.Position.Top, defaultSolidBorderStyle: false);
				}
				return m_topBorder;
			}
		}

		public override Border RightBorder
		{
			get
			{
				if (m_rightBorder == null && HasBorderProperties(Border.Position.Right))
				{
					m_rightBorder = new Border(this, Border.Position.Right, defaultSolidBorderStyle: false);
				}
				return m_rightBorder;
			}
		}

		public override Border BottomBorder
		{
			get
			{
				if (m_bottomBorder == null && HasBorderProperties(Border.Position.Bottom))
				{
					m_bottomBorder = new Border(this, Border.Position.Bottom, defaultSolidBorderStyle: false);
				}
				return m_bottomBorder;
			}
		}

		public override Border LeftBorder
		{
			get
			{
				if (m_leftBorder == null && HasBorderProperties(Border.Position.Left))
				{
					m_leftBorder = new Border(this, Border.Position.Left, defaultSolidBorderStyle: false);
				}
				return m_leftBorder;
			}
		}

		public override ReportColorProperty BackgroundGradientEndColor => GetReportProperty(StyleAttributeNames.BackgroundGradientEndColor) as ReportColorProperty;

		public override ReportColorProperty BackgroundColor => GetReportProperty(StyleAttributeNames.BackgroundColor) as ReportColorProperty;

		public override ReportColorProperty Color => GetReportProperty(StyleAttributeNames.Color) as ReportColorProperty;

		public override ReportEnumProperty<FontStyles> FontStyle => GetReportProperty(StyleAttributeNames.FontStyle) as ReportEnumProperty<FontStyles>;

		public override ReportStringProperty FontFamily => GetReportProperty(StyleAttributeNames.FontFamily) as ReportStringProperty;

		public override ReportEnumProperty<FontWeights> FontWeight => GetReportProperty(StyleAttributeNames.FontWeight) as ReportEnumProperty<FontWeights>;

		public override ReportStringProperty Format => GetReportProperty(StyleAttributeNames.Format) as ReportStringProperty;

		public override ReportEnumProperty<TextDecorations> TextDecoration => GetReportProperty(StyleAttributeNames.TextDecoration) as ReportEnumProperty<TextDecorations>;

		public override ReportEnumProperty<TextAlignments> TextAlign => GetReportProperty(StyleAttributeNames.TextAlign) as ReportEnumProperty<TextAlignments>;

		public override ReportEnumProperty<VerticalAlignments> VerticalAlign => GetReportProperty(StyleAttributeNames.VerticalAlign) as ReportEnumProperty<VerticalAlignments>;

		public override ReportEnumProperty<Directions> Direction => GetReportProperty(StyleAttributeNames.Direction) as ReportEnumProperty<Directions>;

		public override ReportEnumProperty<WritingModes> WritingMode => GetReportProperty(StyleAttributeNames.WritingMode) as ReportEnumProperty<WritingModes>;

		public override ReportStringProperty Language => GetReportProperty(StyleAttributeNames.Language) as ReportStringProperty;

		public override ReportEnumProperty<UnicodeBiDiTypes> UnicodeBiDi => GetReportProperty(StyleAttributeNames.UnicodeBiDi) as ReportEnumProperty<UnicodeBiDiTypes>;

		public override ReportEnumProperty<Calendars> Calendar => GetReportProperty(StyleAttributeNames.Calendar) as ReportEnumProperty<Calendars>;

		public override ReportStringProperty CurrencyLanguage => GetReportProperty(StyleAttributeNames.CurrencyLanguage) as ReportStringProperty;

		public override ReportStringProperty NumeralLanguage => GetReportProperty(StyleAttributeNames.NumeralLanguage) as ReportStringProperty;

		public override ReportEnumProperty<BackgroundGradients> BackgroundGradientType => GetReportProperty(StyleAttributeNames.BackgroundGradientType) as ReportEnumProperty<BackgroundGradients>;

		public override ReportSizeProperty FontSize => GetReportProperty(StyleAttributeNames.FontSize) as ReportSizeProperty;

		public override ReportSizeProperty PaddingLeft => GetReportProperty(StyleAttributeNames.PaddingLeft) as ReportSizeProperty;

		public override ReportSizeProperty PaddingRight => GetReportProperty(StyleAttributeNames.PaddingRight) as ReportSizeProperty;

		public override ReportSizeProperty PaddingTop => GetReportProperty(StyleAttributeNames.PaddingTop) as ReportSizeProperty;

		public override ReportSizeProperty PaddingBottom => GetReportProperty(StyleAttributeNames.PaddingBottom) as ReportSizeProperty;

		public override ReportSizeProperty LineHeight => GetReportProperty(StyleAttributeNames.LineHeight) as ReportSizeProperty;

		public override ReportIntProperty NumeralVariant => GetReportProperty(StyleAttributeNames.NumeralVariant) as ReportIntProperty;

		public override ReportEnumProperty<TextEffects> TextEffect => GetReportProperty(StyleAttributeNames.TextEffect) as ReportEnumProperty<TextEffects>;

		public override ReportEnumProperty<BackgroundHatchTypes> BackgroundHatchType => GetReportProperty(StyleAttributeNames.BackgroundHatchType) as ReportEnumProperty<BackgroundHatchTypes>;

		public override ReportColorProperty ShadowColor => GetReportProperty(StyleAttributeNames.ShadowColor) as ReportColorProperty;

		public override ReportSizeProperty ShadowOffset => GetReportProperty(StyleAttributeNames.ShadowOffset) as ReportSizeProperty;

		internal bool IsOldSnapshot => m_isOldSnapshot;

		internal bool IsDynamicImageStyle => m_isDynamicImageStyle;

		internal Microsoft.ReportingServices.ReportRendering.Style CachedRenderStyle => m_cachedRenderStyle;

		internal IStyleContainer StyleContainer => m_iStyleContainer;

		internal StyleDefaults NormalStyleDefaults => m_normalStyleDefaults;

		internal StyleDefaults LineStyleDefaults => m_lineStyleDefaults;

		internal Style(ReportElement reportElement, IReportScope reportScope, IStyleContainer styleContainer, RenderingContext renderingContext)
			: base(renderingContext)
		{
			m_reportElement = reportElement;
			m_lineStyleDefaults = new StyleDefaults(isLine: true, GetDefaultFontFamily(renderingContext));
			m_normalStyleDefaults = new StyleDefaults(isLine: false, GetDefaultFontFamily(renderingContext));
			m_reportScope = reportScope;
			m_iStyleContainer = styleContainer;
			m_isOldSnapshot = false;
			switch (styleContainer.ObjectType)
			{
			case ObjectType.Line:
				m_isLineBorderStyle = true;
				m_styleDefaults = LineStyleDefaults;
				break;
			case ObjectType.Chart:
			{
				Chart chart = reportElement as Chart;
				m_disallowBorderTransparencyOnDynamicImage = (chart != null);
				m_isDynamicImageStyle = true;
				m_styleDefaults = NormalStyleDefaults;
				break;
			}
			case ObjectType.GaugePanel:
			{
				GaugePanel gaugePanel = reportElement as GaugePanel;
				m_disallowBorderTransparencyOnDynamicImage = (gaugePanel != null);
				m_isDynamicImageStyle = true;
				m_styleDefaults = NormalStyleDefaults;
				break;
			}
			case ObjectType.Map:
			{
				Map map = reportElement as Map;
				m_disallowBorderTransparencyOnDynamicImage = (map != null);
				m_isDynamicImageStyle = true;
				m_styleDefaults = NormalStyleDefaults;
				break;
			}
			default:
				m_styleDefaults = NormalStyleDefaults;
				break;
			}
		}

		internal Style(Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem, RenderingContext renderingContext, bool useRenderStyle)
			: base(renderingContext)
		{
			m_isOldSnapshot = true;
			m_renderReportItem = renderReportItem;
			m_lineStyleDefaults = new StyleDefaults(isLine: true, null);
			m_normalStyleDefaults = new StyleDefaults(isLine: false, null);
			if (useRenderStyle)
			{
				m_cachedRenderStyle = renderReportItem.Style;
			}
			if (renderReportItem is Microsoft.ReportingServices.ReportRendering.Line)
			{
				m_isLineBorderStyle = true;
				m_styleDefaults = LineStyleDefaults;
			}
			else
			{
				m_styleDefaults = NormalStyleDefaults;
			}
		}

		internal Style(Microsoft.ReportingServices.ReportProcessing.Style styleDefinition, object[] styleValues, RenderingContext renderingContext)
			: base(renderingContext)
		{
			m_isOldSnapshot = true;
			m_isDynamicImageStyle = true;
			m_lineStyleDefaults = new StyleDefaults(isLine: true, GetDefaultFontFamily(renderingContext));
			m_normalStyleDefaults = new StyleDefaults(isLine: false, GetDefaultFontFamily(renderingContext));
			m_styleDef = styleDefinition;
			m_styleValues = styleValues;
			m_styleDefaults = NormalStyleDefaults;
		}

		internal Style(ReportElement reportElement, RenderingContext renderingContext)
			: base(renderingContext)
		{
			m_isOldSnapshot = true;
			m_reportElement = reportElement;
			m_lineStyleDefaults = new StyleDefaults(isLine: true, GetDefaultFontFamily(renderingContext));
			m_normalStyleDefaults = new StyleDefaults(isLine: false, GetDefaultFontFamily(renderingContext));
			m_styleDefaults = NormalStyleDefaults;
			m_reportScope = reportElement.ReportScope;
		}

		internal void UpdateStyleCache(Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			if (renderReportItem != null)
			{
				m_renderReportItem = renderReportItem;
				m_cachedRenderStyle = renderReportItem.Style;
			}
		}

		internal void UpdateStyleCache(object[] styleValues)
		{
			m_styleValues = styleValues;
		}

		internal void SetNewContext()
		{
			if (m_backgroundImage != null && m_backgroundImage.Instance != null)
			{
				m_backgroundImage.Instance.SetNewContext();
			}
			if (m_border != null && m_border.GetInstance() != null)
			{
				m_border.GetInstance().SetNewContext();
			}
			if (m_topBorder != null && m_topBorder.GetInstance() != null)
			{
				m_topBorder.GetInstance().SetNewContext();
			}
			if (m_rightBorder != null && m_rightBorder.GetInstance() != null)
			{
				m_rightBorder.GetInstance().SetNewContext();
			}
			if (m_bottomBorder != null && m_bottomBorder.GetInstance() != null)
			{
				m_bottomBorder.GetInstance().SetNewContext();
			}
			if (m_leftBorder != null && m_leftBorder.GetInstance() != null)
			{
				m_leftBorder.GetInstance().SetNewContext();
			}
		}

		internal ReportColor EvaluateInstanceReportColor(StyleAttributeNames style)
		{
			ReportColor reportColor = null;
			if (m_isOldSnapshot)
			{
				if (m_isDynamicImageStyle)
				{
					if (m_styleDef != null)
					{
						string styleStringFromEnum = GetStyleStringFromEnum(style);
						Microsoft.ReportingServices.ReportProcessing.AttributeInfo attributeInfo = m_styleDef.StyleAttributes[styleStringFromEnum];
						if (attributeInfo != null)
						{
							string text = null;
							text = ((!attributeInfo.IsExpression) ? attributeInfo.Value : (m_styleValues[attributeInfo.IntValue] as string));
							if (text != null)
							{
								reportColor = new ReportColor(text, allowTransparency: false);
							}
						}
					}
				}
				else if (IsAvailableStyle(style))
				{
					Microsoft.ReportingServices.ReportRendering.ReportColor reportColor2 = null;
					if (m_cachedRenderStyle != null)
					{
						reportColor2 = (m_cachedRenderStyle[GetStyleStringFromEnum(style)] as Microsoft.ReportingServices.ReportRendering.ReportColor);
					}
					if (reportColor2 != null)
					{
						reportColor = new ReportColor(reportColor2);
					}
				}
			}
			else if (m_iStyleContainer.StyleClass != null)
			{
				m_renderingContext.OdpContext.SetupContext(m_iStyleContainer.InstancePath, m_reportScope.ReportScopeInstance);
				string text2 = m_iStyleContainer.StyleClass.EvaluateStyle(m_iStyleContainer.ObjectType, m_iStyleContainer.Name, (Microsoft.ReportingServices.ReportIntermediateFormat.Style.StyleId)style, m_renderingContext.OdpContext) as string;
				if (text2 != null)
				{
					if (m_disallowBorderTransparencyOnDynamicImage)
					{
						if ((uint)style <= 4u)
						{
							if (!ReportColor.TryParse(text2, out reportColor))
							{
								m_renderingContext.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidColor, Severity.Warning, m_iStyleContainer.ObjectType, m_iStyleContainer.Name, GetStyleStringFromEnum(style), text2);
							}
						}
						else
						{
							reportColor = new ReportColor(text2, m_isDynamicImageStyle);
						}
					}
					else
					{
						reportColor = new ReportColor(text2, m_isDynamicImageStyle);
					}
				}
			}
			return reportColor;
		}

		internal ReportSize EvaluateInstanceReportSize(StyleAttributeNames style)
		{
			ReportSize result = null;
			if (m_isOldSnapshot)
			{
				if (m_isDynamicImageStyle)
				{
					if (m_styleDef != null)
					{
						string styleStringFromEnum = GetStyleStringFromEnum(style);
						Microsoft.ReportingServices.ReportProcessing.AttributeInfo attributeInfo = m_styleDef.StyleAttributes[styleStringFromEnum];
						if (attributeInfo != null)
						{
							object obj = null;
							obj = ((!attributeInfo.IsExpression) ? attributeInfo.Value : m_styleValues[attributeInfo.IntValue]);
							if (obj != null)
							{
								result = new ReportSize(obj as string);
							}
						}
					}
				}
				else if (IsAvailableStyle(style))
				{
					Microsoft.ReportingServices.ReportRendering.ReportSize reportSize = null;
					if (m_cachedRenderStyle != null)
					{
						reportSize = (m_cachedRenderStyle[GetStyleStringFromEnum(style)] as Microsoft.ReportingServices.ReportRendering.ReportSize);
					}
					if (reportSize != null)
					{
						result = new ReportSize(reportSize);
					}
				}
			}
			else if (m_iStyleContainer.StyleClass != null)
			{
				m_renderingContext.OdpContext.SetupContext(m_iStyleContainer.InstancePath, m_reportScope.ReportScopeInstance);
				string text = m_iStyleContainer.StyleClass.EvaluateStyle(m_iStyleContainer.ObjectType, m_iStyleContainer.Name, (Microsoft.ReportingServices.ReportIntermediateFormat.Style.StyleId)style, m_renderingContext.OdpContext) as string;
				if (text != null)
				{
					result = new ReportSize(text);
				}
			}
			return result;
		}

		internal string EvaluateInstanceStyleString(StyleAttributeNames style)
		{
			string result = null;
			if (m_isOldSnapshot)
			{
				if (m_isDynamicImageStyle)
				{
					if (m_styleDef != null)
					{
						string styleStringFromEnum = GetStyleStringFromEnum(style);
						Microsoft.ReportingServices.ReportProcessing.AttributeInfo attributeInfo = m_styleDef.StyleAttributes[styleStringFromEnum];
						if (attributeInfo != null)
						{
							result = ((!attributeInfo.IsExpression) ? attributeInfo.Value : (m_styleValues[attributeInfo.IntValue] as string));
						}
					}
				}
				else if (IsAvailableStyle(style) && m_cachedRenderStyle != null)
				{
					result = (m_cachedRenderStyle[GetStyleStringFromEnum(style)] as string);
				}
			}
			else if (m_iStyleContainer.StyleClass != null)
			{
				result = EvaluateInstanceStyleString((Microsoft.ReportingServices.ReportIntermediateFormat.Style.StyleId)style);
			}
			return result;
		}

		internal string EvaluateInstanceStyleString(Microsoft.ReportingServices.ReportIntermediateFormat.Style.StyleId style)
		{
			m_renderingContext.OdpContext.SetupContext(m_iStyleContainer.InstancePath, m_reportScope.ReportScopeInstance);
			return m_iStyleContainer.StyleClass.EvaluateStyle(m_iStyleContainer.ObjectType, m_iStyleContainer.Name, style, m_renderingContext.OdpContext) as string;
		}

		internal int EvaluateInstanceStyleInt(StyleAttributeNames style, int defaultValue)
		{
			object obj = null;
			if (m_isOldSnapshot)
			{
				if (m_isDynamicImageStyle)
				{
					if (m_styleDef != null)
					{
						string styleStringFromEnum = GetStyleStringFromEnum(style);
						Microsoft.ReportingServices.ReportProcessing.AttributeInfo attributeInfo = m_styleDef.StyleAttributes[styleStringFromEnum];
						if (attributeInfo != null)
						{
							obj = ((!attributeInfo.IsExpression) ? ((object)attributeInfo.IntValue) : m_styleValues[attributeInfo.IntValue]);
						}
					}
				}
				else if (IsAvailableStyle(style) && m_cachedRenderStyle != null)
				{
					obj = m_cachedRenderStyle[GetStyleStringFromEnum(style)];
				}
			}
			else if (m_iStyleContainer.StyleClass != null)
			{
				obj = EvaluateInstanceStyleInt((Microsoft.ReportingServices.ReportIntermediateFormat.Style.StyleId)style);
			}
			if (obj != null && obj is int)
			{
				return (int)obj;
			}
			return defaultValue;
		}

		private object EvaluateInstanceStyleInt(Microsoft.ReportingServices.ReportIntermediateFormat.Style.StyleId style)
		{
			m_renderingContext.OdpContext.SetupContext(m_iStyleContainer.InstancePath, m_reportScope.ReportScopeInstance);
			return m_iStyleContainer.StyleClass.EvaluateStyle(m_iStyleContainer.ObjectType, m_iStyleContainer.Name, style, m_renderingContext.OdpContext);
		}

		internal int EvaluateInstanceStyleEnum(StyleAttributeNames style)
		{
			return EvaluateInstanceStyleEnum(style, 1);
		}

		internal int EvaluateInstanceStyleEnum(StyleAttributeNames style, int styleDefaultValueIfNull)
		{
			int? num = null;
			if (m_isOldSnapshot)
			{
				if (m_isDynamicImageStyle)
				{
					if (m_styleDef != null)
					{
						string styleStringFromEnum = GetStyleStringFromEnum(style);
						Microsoft.ReportingServices.ReportProcessing.AttributeInfo attributeInfo = m_styleDef.StyleAttributes[styleStringFromEnum];
						if (attributeInfo != null)
						{
							string text = null;
							text = ((!attributeInfo.IsExpression) ? attributeInfo.Value : (m_styleValues[attributeInfo.IntValue] as string));
							if (text != null)
							{
								num = StyleTranslator.TranslateStyle(style, text, null, m_isDynamicImageStyle);
							}
						}
					}
				}
				else if (IsAvailableStyle(style) && m_cachedRenderStyle != null)
				{
					string text2 = m_cachedRenderStyle[GetStyleStringFromEnum(style)] as string;
					if (text2 != null)
					{
						num = StyleTranslator.TranslateStyle(style, text2, null, m_isDynamicImageStyle);
					}
				}
			}
			else if (m_iStyleContainer.StyleClass != null)
			{
				num = EvaluateInstanceStyleEnum((Microsoft.ReportingServices.ReportIntermediateFormat.Style.StyleId)style);
			}
			if (!num.HasValue)
			{
				return styleDefaultValueIfNull;
			}
			return num.Value;
		}

		internal int? EvaluateInstanceStyleEnum(Microsoft.ReportingServices.ReportIntermediateFormat.Style.StyleId style)
		{
			int? result = null;
			m_renderingContext.OdpContext.SetupContext(m_iStyleContainer.InstancePath, m_reportScope.ReportScopeInstance);
			object obj = m_iStyleContainer.StyleClass.EvaluateStyle(m_iStyleContainer.ObjectType, m_iStyleContainer.Name, style, m_renderingContext.OdpContext);
			if (obj != null)
			{
				string text = obj as string;
				result = ((text != null) ? new int?(StyleTranslator.TranslateStyle((StyleAttributeNames)style, text, null, m_isDynamicImageStyle)) : new int?((int)obj));
				if (Microsoft.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(m_iStyleContainer.ObjectType) && !Microsoft.ReportingServices.ReportPublishing.Validator.IsDynamicImageSubElement(m_iStyleContainer) && (result.Value == 6 || result.Value == 7))
				{
					result = 3;
				}
			}
			return result;
		}

		internal object EvaluateInstanceStyleVariant(Microsoft.ReportingServices.ReportIntermediateFormat.Style.StyleId style)
		{
			m_renderingContext.OdpContext.SetupContext(m_iStyleContainer.InstancePath, m_reportScope.ReportScopeInstance);
			return m_iStyleContainer.StyleClass.EvaluateStyle(m_iStyleContainer.ObjectType, m_iStyleContainer.Name, style, m_renderingContext.OdpContext);
		}

		internal void ConstructStyleDefinition()
		{
			Global.Tracer.Assert(ReportElement != null, "(ReportElement != null)");
			Global.Tracer.Assert(ReportElement is ReportItem, "(ReportElement is ReportItem)");
			Global.Tracer.Assert(ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Definition, "(ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Definition)");
			Global.Tracer.Assert(ReportElement.ReportItemDef.StyleClass == null, "(ReportElement.ReportItemDef.StyleClass == null)");
			ReportElement.ReportItemDef.StyleClass = new Microsoft.ReportingServices.ReportIntermediateFormat.Style(Microsoft.ReportingServices.ReportIntermediateFormat.ConstructionPhase.Publishing);
			ReportElement.ReportItemDef.StyleClass.InitializeForCRIGeneratedReportItem();
			Border.ConstructBorderDefinition();
			TopBorder.ConstructBorderDefinition();
			BottomBorder.ConstructBorderDefinition();
			LeftBorder.ConstructBorderDefinition();
			RightBorder.ConstructBorderDefinition();
			StyleInstance style = ((ReportItem)ReportElement).Instance.Style;
			Global.Tracer.Assert(!BackgroundColor.IsExpression, "(!this.BackgroundColor.IsExpression)");
			if (style.IsBackgroundColorAssigned)
			{
				string value = (style.BackgroundColor != null) ? style.BackgroundColor.ToString() : null;
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.BackgroundColor), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.BackgroundColor), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!BackgroundGradientEndColor.IsExpression, "(!this.BackgroundGradientEndColor.IsExpression)");
			if (style.IsBackgroundGradientEndColorAssigned)
			{
				string value2 = (style.BackgroundGradientEndColor != null) ? style.BackgroundGradientEndColor.ToString() : null;
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.BackgroundGradientEndColor), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value2));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.BackgroundGradientEndColor), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!Color.IsExpression, "(!this.Color.IsExpression)");
			if (style.IsColorAssigned)
			{
				string value3 = (style.Color != null) ? style.Color.ToString() : null;
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.Color), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value3));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.Color), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!FontStyle.IsExpression, "(!this.FontStyle.IsExpression)");
			if (style.IsFontStyleAssigned)
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.FontStyle), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.FontStyle.ToString()));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.FontStyle), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!FontFamily.IsExpression, "(!this.FontFamily.IsExpression)");
			if (style.IsFontFamilyAssigned)
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.FontFamily), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.FontFamily));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.FontFamily), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!FontWeight.IsExpression, "(!this.FontWeight.IsExpression)");
			if (style.IsFontWeightAssigned)
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.FontWeight), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.FontWeight.ToString()));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.FontWeight), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!Format.IsExpression, "(!this.Format.IsExpression)");
			if (style.IsFormatAssigned)
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.Format), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.Format));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.Format), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!TextDecoration.IsExpression, "(!this.TextDecoration.IsExpression)");
			if (style.IsTextDecorationAssigned)
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.TextDecoration), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.TextDecoration.ToString()));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.TextDecoration), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!TextAlign.IsExpression, "(!this.TextAlign.IsExpression)");
			if (style.IsTextAlignAssigned)
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.TextAlign), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.TextAlign.ToString()));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.TextAlign), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!VerticalAlign.IsExpression, "(!this.VerticalAlign.IsExpression)");
			if (style.IsVerticalAlignAssigned)
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.VerticalAlign), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.VerticalAlign.ToString()));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.VerticalAlign), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!Direction.IsExpression, "(!this.Direction.IsExpression)");
			if (style.IsDirectionAssigned)
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.Direction), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.Direction.ToString()));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.Direction), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!WritingMode.IsExpression, "(!this.WritingMode.IsExpression)");
			if (style.IsWritingModeAssigned)
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.WritingMode), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.WritingMode.ToString()));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.WritingMode), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!Language.IsExpression, "(!this.Language.IsExpression)");
			if (style.IsLanguageAssigned)
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.Language), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.Language));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.Language), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!UnicodeBiDi.IsExpression, "(!this.UnicodeBiDi.IsExpression)");
			if (style.IsUnicodeBiDiAssigned)
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.UnicodeBiDi), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.UnicodeBiDi.ToString()));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.UnicodeBiDi), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!Calendar.IsExpression, "(!this.Calendar.IsExpression)");
			if (style.IsCalendarAssigned)
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.Calendar), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.Calendar.ToString()));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.Calendar), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!CurrencyLanguage.IsExpression, "(!this.CurrencyLanguage.IsExpression)");
			if (style.IsCurrencyLanguageAssigned)
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.CurrencyLanguage), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.CurrencyLanguage));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.CurrencyLanguage), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!NumeralLanguage.IsExpression, "(!this.NumeralLanguage.IsExpression)");
			if (style.IsNumeralLanguageAssigned)
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.NumeralLanguage), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.NumeralLanguage));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.NumeralLanguage), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!BackgroundGradientType.IsExpression, "(!this.BackgroundGradientType.IsExpression)");
			if (style.IsBackgroundGradientTypeAssigned)
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.BackgroundGradientType), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.BackgroundGradientType.ToString()));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.BackgroundGradientType), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!FontSize.IsExpression, "(!this.FontSize.IsExpression)");
			if (style.IsFontSizeAssigned)
			{
				string value4 = (style.FontSize != null) ? style.FontSize.ToString() : null;
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.FontSize), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value4));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.FontSize), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!PaddingLeft.IsExpression, "(!this.PaddingLeft.IsExpression)");
			if (style.IsPaddingLeftAssigned)
			{
				string value5 = (style.PaddingLeft != null) ? style.PaddingLeft.ToString() : null;
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.PaddingLeft), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value5));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.PaddingLeft), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!PaddingRight.IsExpression, "(!this.PaddingRight.IsExpression)");
			if (style.IsPaddingRightAssigned)
			{
				string value6 = (style.PaddingRight != null) ? style.PaddingRight.ToString() : null;
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.PaddingRight), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value6));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.PaddingRight), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!PaddingTop.IsExpression, "(!this.PaddingTop.IsExpression)");
			if (style.IsPaddingTopAssigned)
			{
				string value7 = (style.PaddingTop != null) ? style.PaddingTop.ToString() : null;
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.PaddingTop), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value7));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.PaddingTop), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!PaddingBottom.IsExpression, "(!this.PaddingBottom.IsExpression)");
			if (style.IsPaddingBottomAssigned)
			{
				string value8 = (style.PaddingBottom != null) ? style.PaddingBottom.ToString() : null;
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.PaddingBottom), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value8));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.PaddingBottom), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!LineHeight.IsExpression, "(!this.LineHeight.IsExpression)");
			if (style.IsLineHeightAssigned)
			{
				string value9 = (style.LineHeight != null) ? style.LineHeight.ToString() : null;
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.LineHeight), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value9));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.LineHeight), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!NumeralVariant.IsExpression, "(!this.NumeralVariant.IsExpression)");
			if (style.IsNumeralVariantAssigned)
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.NumeralVariant), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.NumeralVariant));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.NumeralVariant), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!TextEffect.IsExpression, "(!this.TextEffect.IsExpression)");
			if (style.IsTextEffectAssigned)
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.TextEffect), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.TextEffect.ToString()));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.TextEffect), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!BackgroundHatchType.IsExpression, "(!this.BackgroundHatchType.IsExpression)");
			if (style.IsBackgroundHatchTypeAssigned)
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.BackgroundHatchType), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.BackgroundHatchType.ToString()));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.BackgroundHatchType), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!ShadowColor.IsExpression, "(!this.ShadowColor.IsExpression)");
			if (style.IsShadowColorAssigned)
			{
				string value10 = (style.Color != null) ? style.ShadowColor.ToString() : null;
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.ShadowColor), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value10));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.ShadowColor), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!ShadowOffset.IsExpression, "(!this.ShadowOffset.IsExpression)");
			if (style.IsShadowOffsetAssigned)
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.ShadowOffset), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.ShadowOffset.ToString()));
			}
			else
			{
				m_iStyleContainer.StyleClass.AddAttribute(GetStyleStringFromEnum(StyleAttributeNames.ShadowOffset), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			foreach (StyleAttributeNames styleName in StyleBase.StyleNames)
			{
				string styleStringFromEnum = GetStyleStringFromEnum(styleName);
				if (!m_iStyleContainer.StyleClass.GetAttributeInfo(styleStringFromEnum, out Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo styleAttribute))
				{
					m_iStyleContainer.StyleClass.AddAttribute(styleStringFromEnum, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
				}
				else if (!styleAttribute.IsExpression && styleAttribute.Value == null)
				{
					m_iStyleContainer.StyleClass.StyleAttributes.Remove(styleStringFromEnum);
				}
			}
			m_sharedStyles = null;
			m_nonSharedStyles = null;
		}

		private bool HasBorderProperties(Border.Position position)
		{
			if (position == Border.Position.Default)
			{
				return true;
			}
			if (m_isOldSnapshot)
			{
				if (!IsAvailableStyle(StyleAttributeNames.BorderColor))
				{
					return false;
				}
				if (m_cachedRenderStyle == null && m_styleDef == null)
				{
					return false;
				}
				string text = null;
				switch (position)
				{
				case Border.Position.Top:
					text = "BorderStyleTop";
					break;
				case Border.Position.Right:
					text = "BorderStyleRight";
					break;
				case Border.Position.Bottom:
					text = "BorderStyleBottom";
					break;
				case Border.Position.Left:
					text = "BorderStyleLeft";
					break;
				}
				if (m_isDynamicImageStyle)
				{
					if (m_styleDef.StyleAttributes[text] != null)
					{
						return true;
					}
				}
				else if (m_cachedRenderStyle.GetStyleDefinition(text) != null)
				{
					return true;
				}
				switch (position)
				{
				case Border.Position.Top:
					text = "BorderColorTop";
					break;
				case Border.Position.Right:
					text = "BorderColorRight";
					break;
				case Border.Position.Bottom:
					text = "BorderColorBottom";
					break;
				case Border.Position.Left:
					text = "BorderColorLeft";
					break;
				}
				if (m_isDynamicImageStyle)
				{
					if (m_styleDef.StyleAttributes[text] != null)
					{
						return true;
					}
				}
				else if (m_cachedRenderStyle.GetStyleDefinition(text) != null)
				{
					return true;
				}
				switch (position)
				{
				case Border.Position.Top:
					text = "BorderWidthTop";
					break;
				case Border.Position.Right:
					text = "BorderWidthRight";
					break;
				case Border.Position.Bottom:
					text = "BorderWidthBottom";
					break;
				case Border.Position.Left:
					text = "BorderWidthLeft";
					break;
				}
				if (m_isDynamicImageStyle)
				{
					if (m_styleDef.StyleAttributes[text] != null)
					{
						return true;
					}
				}
				else if (m_cachedRenderStyle.GetStyleDefinition(text) != null)
				{
					return true;
				}
				return false;
			}
			return true;
		}

		private static string GetDefaultFontFamily(RenderingContext renderingContext)
		{
			if (renderingContext.OdpContext != null && renderingContext.OdpContext.ReportDefinition != null)
			{
				return renderingContext.OdpContext.ReportDefinition.DefaultFontFamily;
			}
			return null;
		}

		private void PopulateCollections()
		{
			if (m_isOldSnapshot)
			{
				if (m_cachedRenderStyle == null && m_styleDef == null)
				{
					return;
				}
				m_sharedStyles = new List<StyleAttributeNames>();
				m_nonSharedStyles = new List<StyleAttributeNames>();
				foreach (StyleAttributeNames styleName in StyleBase.StyleNames)
				{
					bool isExpressionBased;
					if (StyleAttributeNames.BackgroundImage != styleName)
					{
						Microsoft.ReportingServices.ReportProcessing.AttributeInfo attributeInfo = null;
						if (IsAvailableStyle(styleName))
						{
							attributeInfo = ((!m_isDynamicImageStyle) ? m_cachedRenderStyle.GetStyleDefinition(GetStyleStringFromEnum(styleName)) : m_styleDef.StyleAttributes[GetStyleStringFromEnum(styleName)]);
						}
						if (attributeInfo != null)
						{
							if (attributeInfo.IsExpression)
							{
								m_nonSharedStyles.Add(styleName);
							}
							else
							{
								m_sharedStyles.Add(styleName);
							}
						}
					}
					else if (!m_isDynamicImageStyle && m_cachedRenderStyle.HasBackgroundImage(out isExpressionBased))
					{
						if (isExpressionBased)
						{
							m_nonSharedStyles.Add(styleName);
						}
						else
						{
							m_sharedStyles.Add(styleName);
						}
					}
				}
				return;
			}
			m_sharedStyles = new List<StyleAttributeNames>();
			m_nonSharedStyles = new List<StyleAttributeNames>();
			if (m_iStyleContainer == null || m_iStyleContainer.StyleClass == null || m_iStyleContainer.StyleClass.StyleAttributes == null)
			{
				return;
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo styleAttribute = null;
			foreach (StyleAttributeNames styleName2 in StyleBase.StyleNames)
			{
				string styleAttributeName = (StyleAttributeNames.BackgroundImage == styleName2) ? "BackgroundImageValue" : GetStyleStringFromEnum(styleName2);
				if (m_iStyleContainer.StyleClass.GetAttributeInfo(styleAttributeName, out styleAttribute))
				{
					if (styleAttribute.IsExpression)
					{
						m_nonSharedStyles.Add(styleName2);
					}
					else
					{
						m_sharedStyles.Add(styleName2);
					}
				}
			}
		}

		internal string GetStyleStringFromEnum(StyleAttributeNames style)
		{
			switch (style)
			{
			case StyleAttributeNames.BackgroundColor:
				return "BackgroundColor";
			case StyleAttributeNames.Color:
				return "Color";
			case StyleAttributeNames.FontStyle:
				return "FontStyle";
			case StyleAttributeNames.FontFamily:
				return "FontFamily";
			case StyleAttributeNames.FontWeight:
				return "FontWeight";
			case StyleAttributeNames.FontSize:
				return "FontSize";
			case StyleAttributeNames.Format:
				return "Format";
			case StyleAttributeNames.BackgroundImage:
				return "BackgroundImage";
			case StyleAttributeNames.BorderColor:
				return "BorderColor";
			case StyleAttributeNames.BorderColorBottom:
				return "BorderColorBottom";
			case StyleAttributeNames.BorderColorLeft:
				return "BorderColorLeft";
			case StyleAttributeNames.BorderColorRight:
				return "BorderColorRight";
			case StyleAttributeNames.BorderColorTop:
				return "BorderColorTop";
			case StyleAttributeNames.BackgroundGradientEndColor:
				return "BackgroundGradientEndColor";
			case StyleAttributeNames.BorderStyle:
				return "BorderStyle";
			case StyleAttributeNames.BorderStyleTop:
				return "BorderStyleTop";
			case StyleAttributeNames.BorderStyleLeft:
				return "BorderStyleLeft";
			case StyleAttributeNames.BorderStyleRight:
				return "BorderStyleRight";
			case StyleAttributeNames.BorderStyleBottom:
				return "BorderStyleBottom";
			case StyleAttributeNames.TextDecoration:
				return "TextDecoration";
			case StyleAttributeNames.TextAlign:
				return "TextAlign";
			case StyleAttributeNames.VerticalAlign:
				return "VerticalAlign";
			case StyleAttributeNames.Direction:
				return "Direction";
			case StyleAttributeNames.WritingMode:
				return "WritingMode";
			case StyleAttributeNames.Language:
				return "Language";
			case StyleAttributeNames.UnicodeBiDi:
				return "UnicodeBiDi";
			case StyleAttributeNames.Calendar:
				return "Calendar";
			case StyleAttributeNames.CurrencyLanguage:
				return "CurrencyLanguage";
			case StyleAttributeNames.NumeralLanguage:
				return "NumeralLanguage";
			case StyleAttributeNames.BackgroundGradientType:
				return "BackgroundGradientType";
			case StyleAttributeNames.BorderWidth:
				return "BorderWidth";
			case StyleAttributeNames.BorderWidthTop:
				return "BorderWidthTop";
			case StyleAttributeNames.BorderWidthLeft:
				return "BorderWidthLeft";
			case StyleAttributeNames.BorderWidthRight:
				return "BorderWidthRight";
			case StyleAttributeNames.BorderWidthBottom:
				return "BorderWidthBottom";
			case StyleAttributeNames.PaddingLeft:
				return "PaddingLeft";
			case StyleAttributeNames.PaddingRight:
				return "PaddingRight";
			case StyleAttributeNames.PaddingTop:
				return "PaddingTop";
			case StyleAttributeNames.PaddingBottom:
				return "PaddingBottom";
			case StyleAttributeNames.LineHeight:
				return "LineHeight";
			case StyleAttributeNames.NumeralVariant:
				return "NumeralVariant";
			case StyleAttributeNames.TextEffect:
				return "TextEffect";
			case StyleAttributeNames.BackgroundHatchType:
				return "BackgroundHatchType";
			case StyleAttributeNames.ShadowColor:
				return "ShadowColor";
			case StyleAttributeNames.ShadowOffset:
				return "ShadowOffset";
			case StyleAttributeNames.BackgroundImageRepeat:
				return "BackgroundRepeat";
			case StyleAttributeNames.BackgroundImageSource:
				return "BackgroundImageSource";
			case StyleAttributeNames.BackgroundImageValue:
				return "BackgroundImageValue";
			case StyleAttributeNames.BackgroundImageMimeType:
				return "BackgroundImageMIMEType";
			case StyleAttributeNames.Position:
				return "Position";
			case StyleAttributeNames.TransparentColor:
				return "TransparentColor";
			default:
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
		}

		private ReportProperty GetReportProperty(StyleAttributeNames styleName)
		{
			ReportProperty reportProperty = null;
			if (styleName >= StyleAttributeNames.Count)
			{
				return null;
			}
			if (m_cachedReportProperties[(int)styleName] != null)
			{
				reportProperty = m_cachedReportProperties[(int)styleName];
			}
			else
			{
				reportProperty = ((!m_isOldSnapshot) ? GetOdpReportProperty(styleName) : ((!m_isDynamicImageStyle) ? GetOldSnapshotReportProperty(styleName, m_cachedRenderStyle) : GetOldSnapshotReportProperty(styleName, m_styleDef)));
				if (ReportElement == null || ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Definition)
				{
					m_cachedReportProperties[(int)styleName] = reportProperty;
				}
			}
			return reportProperty;
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo GetAttributeInfo(string styleNameString, out string expressionString)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo styleAttribute = null;
			expressionString = null;
			if (m_iStyleContainer.StyleClass != null && m_iStyleContainer.StyleClass.GetAttributeInfo(styleNameString, out styleAttribute))
			{
				if (styleAttribute.IsExpression)
				{
					expressionString = m_iStyleContainer.StyleClass.ExpressionList[styleAttribute.IntValue].OriginalText;
				}
				else
				{
					expressionString = styleAttribute.Value;
				}
			}
			return styleAttribute;
		}

		private ReportProperty GetOdpReportProperty(StyleAttributeNames styleName)
		{
			string text = null;
			text = ((styleName != StyleAttributeNames.BackgroundImage) ? GetStyleStringFromEnum(styleName) : "BackgroundImageValue");
			string text2 = null;
			string expressionString = null;
			Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = GetAttributeInfo(text, out expressionString);
			switch (styleName)
			{
			case StyleAttributeNames.BackgroundImage:
				if (attributeInfo != null)
				{
					return new BackgroundImage(attributeInfo.IsExpression, expressionString, this);
				}
				break;
			case StyleAttributeNames.BorderColor:
			case StyleAttributeNames.BorderColorTop:
			case StyleAttributeNames.BorderColorLeft:
			case StyleAttributeNames.BorderColorRight:
			case StyleAttributeNames.BorderColorBottom:
			case StyleAttributeNames.BackgroundColor:
			case StyleAttributeNames.Color:
			case StyleAttributeNames.BackgroundGradientEndColor:
			case StyleAttributeNames.ShadowColor:
			{
				ReportColor reportColor = null;
				if (!m_isDynamicImageStyle || styleName != StyleAttributeNames.Color)
				{
					reportColor = (m_styleDefaults[text] as ReportColor);
				}
				if (attributeInfo == null)
				{
					return new ReportColorProperty(isExpression: false, null, reportColor, reportColor);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportColorProperty(attributeInfo.IsExpression, expressionString, attributeInfo.IsExpression ? null : new ReportColor(text2, m_isDynamicImageStyle), reportColor);
			}
			case StyleAttributeNames.FontFamily:
			case StyleAttributeNames.Format:
			case StyleAttributeNames.Language:
			case StyleAttributeNames.NumeralLanguage:
			case StyleAttributeNames.CurrencyLanguage:
				if (attributeInfo == null)
				{
					return new ReportStringProperty(isExpression: false, null, m_styleDefaults[text] as string);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportStringProperty(attributeInfo.IsExpression, expressionString, text2, attributeInfo.IsExpression ? (m_styleDefaults[text] as string) : null);
			case StyleAttributeNames.BorderStyle:
			{
				BorderStyles borderStyles = (!m_isLineBorderStyle) ? BorderStyles.None : BorderStyles.Solid;
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<BorderStyles>(isExpression: false, null, borderStyles, borderStyles);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<BorderStyles>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateBorderStyle(text2, null), borderStyles);
			}
			case StyleAttributeNames.BorderStyleTop:
			case StyleAttributeNames.BorderStyleLeft:
			case StyleAttributeNames.BorderStyleRight:
			case StyleAttributeNames.BorderStyleBottom:
				if (attributeInfo == null)
				{
					return null;
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<BorderStyles>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateBorderStyle(text2, null), BorderStyles.None);
			case StyleAttributeNames.FontStyle:
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<FontStyles>(DefaultEnumFontStyle);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<FontStyles>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateFontStyle(text2, null), DefaultEnumFontStyle);
			case StyleAttributeNames.FontWeight:
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<FontWeights>(DefaultEnumFontWeight);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<FontWeights>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateFontWeight(text2, null), DefaultEnumFontWeight);
			case StyleAttributeNames.TextDecoration:
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<TextDecorations>(DefaultEnumTextDecoration);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<TextDecorations>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateTextDecoration(text2, null), DefaultEnumTextDecoration);
			case StyleAttributeNames.TextAlign:
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<TextAlignments>(DefaultEnumTextAlignment);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<TextAlignments>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateTextAlign(text2, null), DefaultEnumTextAlignment);
			case StyleAttributeNames.VerticalAlign:
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<VerticalAlignments>(DefaultEnumVerticalAlignment);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<VerticalAlignments>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateVerticalAlign(text2, null), DefaultEnumVerticalAlignment);
			case StyleAttributeNames.Direction:
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<Directions>(DefaultEnumDirection);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<Directions>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateDirection(text2, null), DefaultEnumDirection);
			case StyleAttributeNames.WritingMode:
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<WritingModes>(DefaultEnumWritingMode);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<WritingModes>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateWritingMode(text2, null), DefaultEnumWritingMode);
			case StyleAttributeNames.UnicodeBiDi:
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<UnicodeBiDiTypes>(DefaultEnumUnicodeBiDiType);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<UnicodeBiDiTypes>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateUnicodeBiDi(text2, null), DefaultEnumUnicodeBiDiType);
			case StyleAttributeNames.Calendar:
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<Calendars>(DefaultEnumCalendar);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<Calendars>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateCalendar(text2, null), DefaultEnumCalendar);
			case StyleAttributeNames.BackgroundGradientType:
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<BackgroundGradients>(DefaultEnumBackgroundGradient);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<BackgroundGradients>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateBackgroundGradientType(text2, null), DefaultEnumBackgroundGradient);
			case StyleAttributeNames.BorderWidth:
			case StyleAttributeNames.FontSize:
			case StyleAttributeNames.PaddingLeft:
			case StyleAttributeNames.PaddingRight:
			case StyleAttributeNames.PaddingTop:
			case StyleAttributeNames.PaddingBottom:
			case StyleAttributeNames.LineHeight:
			case StyleAttributeNames.ShadowOffset:
				if (attributeInfo == null)
				{
					return new ReportSizeProperty(isExpression: false, null, m_styleDefaults[text] as ReportSize);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportSizeProperty(attributeInfo.IsExpression, expressionString, attributeInfo.IsExpression ? null : new ReportSize(text2, allowNegative: false), attributeInfo.IsExpression ? (m_styleDefaults[text] as ReportSize) : null);
			case StyleAttributeNames.BorderWidthTop:
			case StyleAttributeNames.BorderWidthLeft:
			case StyleAttributeNames.BorderWidthRight:
			case StyleAttributeNames.BorderWidthBottom:
				if (attributeInfo == null)
				{
					return null;
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportSizeProperty(attributeInfo.IsExpression, expressionString, attributeInfo.IsExpression ? null : new ReportSize(text2, allowNegative: false), null);
			case StyleAttributeNames.NumeralVariant:
			{
				int num = (int)m_styleDefaults[text];
				if (attributeInfo == null)
				{
					return new ReportIntProperty(isExpression: false, null, num, num);
				}
				if (!attributeInfo.IsExpression)
				{
					num = attributeInfo.IntValue;
				}
				return new ReportIntProperty(attributeInfo.IsExpression, expressionString, num, num);
			}
			case StyleAttributeNames.TextEffect:
			{
				TextEffects defaultValue2 = StyleTranslator.TranslateTextEffect(null, null, m_isDynamicImageStyle);
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<TextEffects>(isExpression: false, null, TextEffects.Default, defaultValue2);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<TextEffects>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateTextEffect(text2, null, m_isDynamicImageStyle), defaultValue2);
			}
			case StyleAttributeNames.BackgroundHatchType:
			{
				BackgroundHatchTypes defaultValue = StyleTranslator.TranslateBackgroundHatchType(null, null, m_isDynamicImageStyle);
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<BackgroundHatchTypes>(isExpression: false, null, BackgroundHatchTypes.Default, defaultValue);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<BackgroundHatchTypes>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateBackgroundHatchType(text2, null, m_isDynamicImageStyle), defaultValue);
			}
			}
			return null;
		}

		private ReportProperty GetOldSnapshotReportProperty(StyleAttributeNames styleName, Microsoft.ReportingServices.ReportRendering.Style style)
		{
			Microsoft.ReportingServices.ReportProcessing.AttributeInfo styleDefinition = null;
			string styleStringFromEnum = GetStyleStringFromEnum(styleName);
			string expressionString = null;
			if (style != null && styleName != StyleAttributeNames.BackgroundImage)
			{
				styleDefinition = style.GetStyleDefinition(styleStringFromEnum, out expressionString);
			}
			return GetOldSnapshotReportProperty(styleDefinition, expressionString, styleName, styleStringFromEnum, style);
		}

		private ReportProperty GetOldSnapshotReportProperty(StyleAttributeNames styleName, Microsoft.ReportingServices.ReportProcessing.Style style)
		{
			Microsoft.ReportingServices.ReportProcessing.AttributeInfo attributeInfo = null;
			string expressionString = null;
			string styleStringFromEnum = GetStyleStringFromEnum(styleName);
			if (style != null)
			{
				attributeInfo = m_styleDef.StyleAttributes[styleStringFromEnum];
				if (attributeInfo.IsExpression)
				{
					expressionString = m_styleDef.ExpressionList[attributeInfo.IntValue].OriginalText;
				}
			}
			return GetOldSnapshotReportProperty(attributeInfo, expressionString, styleName, styleStringFromEnum, null);
		}

		private ReportProperty GetOldSnapshotReportProperty(Microsoft.ReportingServices.ReportProcessing.AttributeInfo styleDefinition, string expressionString, StyleAttributeNames styleName, string styleNameString, Microsoft.ReportingServices.ReportRendering.Style style)
		{
			if (!IsAvailableStyle(styleName))
			{
				styleDefinition = null;
			}
			switch (styleName)
			{
			case StyleAttributeNames.BackgroundImage:
				if (IsAvailableStyle(styleName))
				{
					Microsoft.ReportingServices.ReportRendering.BackgroundImage backgroundImage = null;
					if (style != null)
					{
						backgroundImage = (style[styleNameString] as Microsoft.ReportingServices.ReportRendering.BackgroundImage);
					}
					if (backgroundImage != null)
					{
						return new BackgroundImage(isExpression: true, expressionString, style, this);
					}
				}
				break;
			case StyleAttributeNames.BorderColor:
			case StyleAttributeNames.BorderColorTop:
			case StyleAttributeNames.BorderColorLeft:
			case StyleAttributeNames.BorderColorRight:
			case StyleAttributeNames.BorderColorBottom:
			case StyleAttributeNames.BackgroundColor:
			case StyleAttributeNames.Color:
			case StyleAttributeNames.BackgroundGradientEndColor:
			case StyleAttributeNames.ShadowColor:
			{
				ReportColor reportColor = null;
				if (!m_isDynamicImageStyle || styleName != StyleAttributeNames.Color)
				{
					reportColor = (m_styleDefaults[styleNameString] as ReportColor);
				}
				if (styleDefinition == null)
				{
					return new ReportColorProperty(isExpression: false, null, reportColor, reportColor);
				}
				return new ReportColorProperty(styleDefinition.IsExpression, expressionString, styleDefinition.IsExpression ? null : new ReportColor(styleDefinition.Value, m_isDynamicImageStyle), reportColor);
			}
			case StyleAttributeNames.FontFamily:
			case StyleAttributeNames.Format:
			case StyleAttributeNames.Language:
			case StyleAttributeNames.NumeralLanguage:
			case StyleAttributeNames.CurrencyLanguage:
				if (styleDefinition == null)
				{
					return new ReportStringProperty(isExpression: false, null, m_styleDefaults[styleNameString] as string);
				}
				return new ReportStringProperty(styleDefinition.IsExpression, expressionString, styleDefinition.Value, styleDefinition.IsExpression ? (m_styleDefaults[styleNameString] as string) : null);
			case StyleAttributeNames.BorderStyle:
			{
				BorderStyles borderStyles = (!m_isLineBorderStyle) ? BorderStyles.None : BorderStyles.Solid;
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<BorderStyles>(isExpression: false, null, borderStyles, borderStyles);
				}
				return new ReportEnumProperty<BorderStyles>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateBorderStyle(styleDefinition.Value, null), borderStyles);
			}
			case StyleAttributeNames.BorderStyleTop:
			case StyleAttributeNames.BorderStyleLeft:
			case StyleAttributeNames.BorderStyleRight:
			case StyleAttributeNames.BorderStyleBottom:
				if (styleDefinition == null)
				{
					return null;
				}
				return new ReportEnumProperty<BorderStyles>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateBorderStyle(styleDefinition.Value, null), BorderStyles.None);
			case StyleAttributeNames.FontStyle:
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<FontStyles>(DefaultEnumFontStyle);
				}
				return new ReportEnumProperty<FontStyles>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateFontStyle(styleDefinition.Value, null), DefaultEnumFontStyle);
			case StyleAttributeNames.FontWeight:
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<FontWeights>(DefaultEnumFontWeight);
				}
				return new ReportEnumProperty<FontWeights>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateFontWeight(styleDefinition.Value, null), DefaultEnumFontWeight);
			case StyleAttributeNames.TextDecoration:
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<TextDecorations>(DefaultEnumTextDecoration);
				}
				return new ReportEnumProperty<TextDecorations>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateTextDecoration(styleDefinition.Value, null), DefaultEnumTextDecoration);
			case StyleAttributeNames.TextAlign:
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<TextAlignments>(DefaultEnumTextAlignment);
				}
				return new ReportEnumProperty<TextAlignments>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateTextAlign(styleDefinition.Value, null), DefaultEnumTextAlignment);
			case StyleAttributeNames.VerticalAlign:
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<VerticalAlignments>(DefaultEnumVerticalAlignment);
				}
				return new ReportEnumProperty<VerticalAlignments>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateVerticalAlign(styleDefinition.Value, null), DefaultEnumVerticalAlignment);
			case StyleAttributeNames.Direction:
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<Directions>(DefaultEnumDirection);
				}
				return new ReportEnumProperty<Directions>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateDirection(styleDefinition.Value, null), DefaultEnumDirection);
			case StyleAttributeNames.WritingMode:
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<WritingModes>(DefaultEnumWritingMode);
				}
				return new ReportEnumProperty<WritingModes>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateWritingMode(styleDefinition.Value, null), DefaultEnumWritingMode);
			case StyleAttributeNames.UnicodeBiDi:
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<UnicodeBiDiTypes>(DefaultEnumUnicodeBiDiType);
				}
				return new ReportEnumProperty<UnicodeBiDiTypes>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateUnicodeBiDi(styleDefinition.Value, null), DefaultEnumUnicodeBiDiType);
			case StyleAttributeNames.Calendar:
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<Calendars>(DefaultEnumCalendar);
				}
				return new ReportEnumProperty<Calendars>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateCalendar(styleDefinition.Value, null), DefaultEnumCalendar);
			case StyleAttributeNames.BackgroundGradientType:
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<BackgroundGradients>(DefaultEnumBackgroundGradient);
				}
				return new ReportEnumProperty<BackgroundGradients>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateBackgroundGradientType(styleDefinition.Value, null), DefaultEnumBackgroundGradient);
			case StyleAttributeNames.BorderWidth:
			case StyleAttributeNames.FontSize:
			case StyleAttributeNames.PaddingLeft:
			case StyleAttributeNames.PaddingRight:
			case StyleAttributeNames.PaddingTop:
			case StyleAttributeNames.PaddingBottom:
			case StyleAttributeNames.LineHeight:
			case StyleAttributeNames.ShadowOffset:
				if (styleDefinition == null)
				{
					return new ReportSizeProperty(isExpression: false, null, m_styleDefaults[styleNameString] as ReportSize);
				}
				return new ReportSizeProperty(styleDefinition.IsExpression, expressionString, styleDefinition.IsExpression ? null : new ReportSize(styleDefinition.Value, allowNegative: false), styleDefinition.IsExpression ? (m_styleDefaults[styleNameString] as ReportSize) : null);
			case StyleAttributeNames.BorderWidthTop:
			case StyleAttributeNames.BorderWidthLeft:
			case StyleAttributeNames.BorderWidthRight:
			case StyleAttributeNames.BorderWidthBottom:
				if (styleDefinition == null)
				{
					return null;
				}
				return new ReportSizeProperty(styleDefinition.IsExpression, expressionString, styleDefinition.IsExpression ? null : new ReportSize(styleDefinition.Value, allowNegative: false), null);
			case StyleAttributeNames.NumeralVariant:
			{
				int num = (int)m_styleDefaults[styleNameString];
				if (styleDefinition == null)
				{
					return new ReportIntProperty(isExpression: false, null, num, num);
				}
				return new ReportIntProperty(styleDefinition.IsExpression, expressionString, styleDefinition.IntValue, num);
			}
			case StyleAttributeNames.TextEffect:
			{
				TextEffects defaultValue2 = StyleTranslator.TranslateTextEffect(null, null, m_isDynamicImageStyle);
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<TextEffects>(isExpression: false, null, TextEffects.Default, defaultValue2);
				}
				return new ReportEnumProperty<TextEffects>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateTextEffect(styleDefinition.Value, null, m_isDynamicImageStyle), defaultValue2);
			}
			case StyleAttributeNames.BackgroundHatchType:
			{
				BackgroundHatchTypes defaultValue = StyleTranslator.TranslateBackgroundHatchType(null, null, m_isDynamicImageStyle);
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<BackgroundHatchTypes>(isExpression: false, null, BackgroundHatchTypes.Default, defaultValue);
				}
				return new ReportEnumProperty<BackgroundHatchTypes>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateBackgroundHatchType(styleDefinition.Value, null, m_isDynamicImageStyle), defaultValue);
			}
			}
			return null;
		}

		protected virtual bool IsAvailableStyle(StyleAttributeNames styleName)
		{
			return true;
		}
	}
}

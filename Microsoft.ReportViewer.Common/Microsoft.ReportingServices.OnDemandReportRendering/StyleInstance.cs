using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	[SkipStaticValidation]
	internal class StyleInstance : StyleBaseInstance, IPersistable
	{
		private Style m_styleDefinition;

		protected ReportColor m_backgroundColor;

		protected ReportColor m_backgroundGradientEndColor;

		protected ReportColor m_color;

		protected FontStyles? m_fontStyle;

		protected string m_fontFamily;

		protected FontWeights? m_fontWeight;

		protected string m_format;

		protected TextDecorations? m_textDecoration;

		protected TextAlignments? m_textAlign;

		protected VerticalAlignments? m_verticalAlign;

		protected Directions? m_direction;

		protected WritingModes? m_writingMode;

		protected string m_language;

		protected UnicodeBiDiTypes? m_unicodeBiDi;

		protected Calendars? m_calendar;

		protected string m_currencyLanguage;

		protected string m_numeralLanguage;

		protected BackgroundGradients? m_backgroundGradientType;

		protected ReportSize m_fontSize;

		protected ReportSize m_paddingLeft;

		protected ReportSize m_paddingRight;

		protected ReportSize m_paddingTop;

		protected ReportSize m_paddingBottom;

		protected ReportSize m_lineHeight;

		protected int m_numeralVariant;

		protected TextEffects? m_textEffect;

		protected BackgroundHatchTypes? m_backgroundHatchType;

		protected ReportColor m_shadowColor;

		protected ReportSize m_shadowOffset;

		protected Dictionary<StyleAttributeNames, bool> m_assignedValues;

		private static readonly Declaration m_Declaration = GetDeclaration();

		public override List<StyleAttributeNames> StyleAttributes => m_styleDefinition.NonSharedStyleAttributes;

		public override object this[StyleAttributeNames style]
		{
			get
			{
				Border border = null;
				switch (style)
				{
				case StyleAttributeNames.BorderColor:
					return m_styleDefinition.Border.Instance.Color;
				case StyleAttributeNames.BorderColorTop:
					return m_styleDefinition.TopBorder?.Instance.Color;
				case StyleAttributeNames.BorderColorLeft:
					return m_styleDefinition.LeftBorder?.Instance.Color;
				case StyleAttributeNames.BorderColorRight:
					return m_styleDefinition.RightBorder?.Instance.Color;
				case StyleAttributeNames.BorderColorBottom:
					return m_styleDefinition.BottomBorder?.Instance.Color;
				case StyleAttributeNames.BorderStyle:
					return m_styleDefinition.Border.Instance.Style;
				case StyleAttributeNames.BorderStyleTop:
					border = m_styleDefinition.TopBorder;
					if (border != null)
					{
						return border.Instance.Style;
					}
					return null;
				case StyleAttributeNames.BorderStyleLeft:
					border = m_styleDefinition.LeftBorder;
					if (border != null)
					{
						return border.Instance.Style;
					}
					return null;
				case StyleAttributeNames.BorderStyleRight:
					border = m_styleDefinition.RightBorder;
					if (border != null)
					{
						return border.Instance.Style;
					}
					return null;
				case StyleAttributeNames.BorderStyleBottom:
					border = m_styleDefinition.BottomBorder;
					if (border != null)
					{
						return border.Instance.Style;
					}
					return null;
				case StyleAttributeNames.BorderWidth:
					return m_styleDefinition.Border.Instance.Width;
				case StyleAttributeNames.BorderWidthTop:
					return m_styleDefinition.TopBorder?.Instance.Width;
				case StyleAttributeNames.BorderWidthLeft:
					return m_styleDefinition.LeftBorder?.Instance.Width;
				case StyleAttributeNames.BorderWidthRight:
					return m_styleDefinition.RightBorder?.Instance.Width;
				case StyleAttributeNames.BorderWidthBottom:
					return m_styleDefinition.BottomBorder?.Instance.Width;
				case StyleAttributeNames.BackgroundColor:
					return BackgroundColor;
				case StyleAttributeNames.BackgroundGradientEndColor:
					return BackgroundGradientEndColor;
				case StyleAttributeNames.BackgroundGradientType:
					return BackgroundGradientType;
				case StyleAttributeNames.Calendar:
					return Calendar;
				case StyleAttributeNames.Color:
					return Color;
				case StyleAttributeNames.CurrencyLanguage:
					return CurrencyLanguage;
				case StyleAttributeNames.Direction:
					return Direction;
				case StyleAttributeNames.FontFamily:
					return FontFamily;
				case StyleAttributeNames.FontSize:
					return FontSize;
				case StyleAttributeNames.FontStyle:
					return FontStyle;
				case StyleAttributeNames.FontWeight:
					return FontWeight;
				case StyleAttributeNames.Format:
					return Format;
				case StyleAttributeNames.Language:
					return Language;
				case StyleAttributeNames.LineHeight:
					return LineHeight;
				case StyleAttributeNames.NumeralLanguage:
					return NumeralLanguage;
				case StyleAttributeNames.NumeralVariant:
					return NumeralVariant;
				case StyleAttributeNames.PaddingBottom:
					return PaddingBottom;
				case StyleAttributeNames.PaddingLeft:
					return PaddingLeft;
				case StyleAttributeNames.PaddingRight:
					return PaddingRight;
				case StyleAttributeNames.PaddingTop:
					return PaddingTop;
				case StyleAttributeNames.TextAlign:
					return TextAlign;
				case StyleAttributeNames.TextDecoration:
					return TextDecoration;
				case StyleAttributeNames.UnicodeBiDi:
					return UnicodeBiDi;
				case StyleAttributeNames.VerticalAlign:
					return VerticalAlign;
				case StyleAttributeNames.WritingMode:
					return WritingMode;
				case StyleAttributeNames.TextEffect:
					return TextEffect;
				case StyleAttributeNames.BackgroundHatchType:
					return BackgroundHatchType;
				case StyleAttributeNames.ShadowColor:
					return ShadowColor;
				case StyleAttributeNames.ShadowOffset:
					return ShadowOffset;
				default:
					return null;
				}
			}
		}

		public override ReportColor BackgroundGradientEndColor
		{
			get
			{
				if (m_backgroundGradientEndColor == null)
				{
					m_backgroundGradientEndColor = m_styleDefinition.EvaluateInstanceReportColor(StyleAttributeNames.BackgroundGradientEndColor);
				}
				return m_backgroundGradientEndColor;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.BackgroundGradientEndColor.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.BackgroundGradientEndColor);
				m_backgroundGradientEndColor = value;
			}
		}

		internal bool IsBackgroundGradientEndColorAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.BackgroundGradientEndColor);
			}
		}

		public override ReportColor Color
		{
			get
			{
				if (m_color == null)
				{
					m_color = m_styleDefinition.EvaluateInstanceReportColor(StyleAttributeNames.Color);
				}
				return m_color;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.Color.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.Color);
				m_color = value;
			}
		}

		internal bool IsColorAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.Color);
			}
		}

		public override ReportColor BackgroundColor
		{
			get
			{
				if (m_backgroundColor == null)
				{
					m_backgroundColor = m_styleDefinition.EvaluateInstanceReportColor(StyleAttributeNames.BackgroundColor);
				}
				return m_backgroundColor;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.BackgroundColor.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.BackgroundColor);
				m_backgroundColor = value;
			}
		}

		internal bool IsBackgroundColorAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.BackgroundColor);
			}
		}

		public override FontStyles FontStyle
		{
			get
			{
				if (!m_fontStyle.HasValue)
				{
					m_fontStyle = (FontStyles)m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.FontStyle);
				}
				return m_fontStyle.Value;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.FontStyle.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.FontStyle);
				m_fontStyle = value;
			}
		}

		internal bool IsFontStyleAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.FontStyle);
			}
		}

		public override string FontFamily
		{
			get
			{
				if (m_fontFamily == null)
				{
					m_fontFamily = m_styleDefinition.EvaluateInstanceStyleString(StyleAttributeNames.FontFamily);
				}
				return m_fontFamily;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.FontFamily.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.FontFamily);
				m_fontFamily = value;
			}
		}

		internal bool IsFontFamilyAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.FontFamily);
			}
		}

		public override FontWeights FontWeight
		{
			get
			{
				if (!m_fontWeight.HasValue)
				{
					m_fontWeight = (FontWeights)m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.FontWeight);
				}
				return m_fontWeight.Value;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.FontWeight.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.FontWeight);
				m_fontWeight = value;
			}
		}

		internal bool IsFontWeightAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.FontWeight);
			}
		}

		public override string Format
		{
			get
			{
				if (m_format == null)
				{
					m_format = m_styleDefinition.EvaluateInstanceStyleString(StyleAttributeNames.Format);
				}
				return m_format;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.Format.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.Format);
				m_format = value;
			}
		}

		internal bool IsFormatAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.Format);
			}
		}

		public override TextDecorations TextDecoration
		{
			get
			{
				if (!m_textDecoration.HasValue)
				{
					m_textDecoration = (TextDecorations)m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.TextDecoration);
				}
				return m_textDecoration.Value;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.TextDecoration.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.TextDecoration);
				m_textDecoration = value;
			}
		}

		internal bool IsTextDecorationAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.TextDecoration);
			}
		}

		public override TextAlignments TextAlign
		{
			get
			{
				if (!m_textAlign.HasValue)
				{
					m_textAlign = (TextAlignments)m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.TextAlign);
				}
				return m_textAlign.Value;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.TextAlign.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.TextAlign);
				m_textAlign = value;
			}
		}

		internal bool IsTextAlignAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.TextAlign);
			}
		}

		public override VerticalAlignments VerticalAlign
		{
			get
			{
				if (!m_verticalAlign.HasValue)
				{
					m_verticalAlign = (VerticalAlignments)m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.VerticalAlign);
				}
				return m_verticalAlign.Value;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.VerticalAlign.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.VerticalAlign);
				m_verticalAlign = value;
			}
		}

		internal bool IsVerticalAlignAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.VerticalAlign);
			}
		}

		public override Directions Direction
		{
			get
			{
				if (!m_direction.HasValue)
				{
					m_direction = (Directions)m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.Direction);
				}
				return m_direction.Value;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.Direction.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.Direction);
				m_direction = value;
			}
		}

		internal bool IsDirectionAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.Direction);
			}
		}

		public override WritingModes WritingMode
		{
			get
			{
				if (!m_writingMode.HasValue)
				{
					m_writingMode = (WritingModes)m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.WritingMode);
				}
				return m_writingMode.Value;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.WritingMode.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.WritingMode);
				m_writingMode = value;
			}
		}

		internal bool IsWritingModeAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.WritingMode);
			}
		}

		public override string Language
		{
			get
			{
				if (m_language == null)
				{
					m_language = m_styleDefinition.EvaluateInstanceStyleString(StyleAttributeNames.Language);
				}
				return m_language;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.Language.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.Language);
				m_language = value;
			}
		}

		internal bool IsLanguageAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.Language);
			}
		}

		public override UnicodeBiDiTypes UnicodeBiDi
		{
			get
			{
				if (!m_unicodeBiDi.HasValue)
				{
					m_unicodeBiDi = (UnicodeBiDiTypes)m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.UnicodeBiDi);
				}
				return m_unicodeBiDi.Value;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.UnicodeBiDi.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.UnicodeBiDi);
				m_unicodeBiDi = value;
			}
		}

		internal bool IsUnicodeBiDiAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.UnicodeBiDi);
			}
		}

		public override Calendars Calendar
		{
			get
			{
				if (!m_calendar.HasValue)
				{
					m_calendar = (Calendars)m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.Calendar);
				}
				return m_calendar.Value;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.Calendar.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.Calendar);
				m_calendar = value;
			}
		}

		internal bool IsCalendarAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.Calendar);
			}
		}

		public override string CurrencyLanguage
		{
			get
			{
				if (m_currencyLanguage == null)
				{
					m_currencyLanguage = m_styleDefinition.EvaluateInstanceStyleString(StyleAttributeNames.CurrencyLanguage);
				}
				return m_currencyLanguage;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.CurrencyLanguage.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.CurrencyLanguage);
				m_currencyLanguage = value;
			}
		}

		internal bool IsCurrencyLanguageAssigned
		{
			get
			{
				if (m_currencyLanguage == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.CurrencyLanguage);
			}
		}

		public override string NumeralLanguage
		{
			get
			{
				if (m_numeralLanguage == null)
				{
					m_numeralLanguage = m_styleDefinition.EvaluateInstanceStyleString(StyleAttributeNames.NumeralLanguage);
				}
				return m_numeralLanguage;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.NumeralLanguage.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.NumeralLanguage);
				m_numeralLanguage = value;
			}
		}

		internal bool IsNumeralLanguageAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.NumeralLanguage);
			}
		}

		public override BackgroundGradients BackgroundGradientType
		{
			get
			{
				if (!m_backgroundGradientType.HasValue)
				{
					m_backgroundGradientType = (BackgroundGradients)m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.BackgroundGradientType);
				}
				return m_backgroundGradientType.Value;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.BackgroundGradientType.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.BackgroundGradientType);
				m_backgroundGradientType = value;
			}
		}

		internal bool IsBackgroundGradientTypeAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.BackgroundGradientType);
			}
		}

		public override ReportSize FontSize
		{
			get
			{
				if (m_fontSize == null)
				{
					m_fontSize = m_styleDefinition.EvaluateInstanceReportSize(StyleAttributeNames.FontSize);
				}
				return m_fontSize;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.FontSize.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.FontSize);
				m_fontSize = value;
			}
		}

		internal bool IsFontSizeAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.FontSize);
			}
		}

		public override ReportSize PaddingLeft
		{
			get
			{
				if (m_paddingLeft == null)
				{
					m_paddingLeft = m_styleDefinition.EvaluateInstanceReportSize(StyleAttributeNames.PaddingLeft);
				}
				return m_paddingLeft;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.PaddingLeft.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.PaddingLeft);
				m_paddingLeft = value;
			}
		}

		internal bool IsPaddingLeftAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.PaddingLeft);
			}
		}

		public override ReportSize PaddingRight
		{
			get
			{
				if (m_paddingRight == null)
				{
					m_paddingRight = m_styleDefinition.EvaluateInstanceReportSize(StyleAttributeNames.PaddingRight);
				}
				return m_paddingRight;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.PaddingRight.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.PaddingRight);
				m_paddingRight = value;
			}
		}

		internal bool IsPaddingRightAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.PaddingRight);
			}
		}

		public override ReportSize PaddingTop
		{
			get
			{
				if (m_paddingTop == null)
				{
					m_paddingTop = m_styleDefinition.EvaluateInstanceReportSize(StyleAttributeNames.PaddingTop);
				}
				return m_paddingTop;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.PaddingTop.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.PaddingTop);
				m_paddingTop = value;
			}
		}

		internal bool IsPaddingTopAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.PaddingTop);
			}
		}

		public override ReportSize PaddingBottom
		{
			get
			{
				if (m_paddingBottom == null)
				{
					m_paddingBottom = m_styleDefinition.EvaluateInstanceReportSize(StyleAttributeNames.PaddingBottom);
				}
				return m_paddingBottom;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.PaddingBottom.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.PaddingBottom);
				m_paddingBottom = value;
			}
		}

		internal bool IsPaddingBottomAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.PaddingBottom);
			}
		}

		public override ReportSize LineHeight
		{
			get
			{
				if (m_lineHeight == null)
				{
					m_lineHeight = m_styleDefinition.EvaluateInstanceReportSize(StyleAttributeNames.LineHeight);
				}
				return m_lineHeight;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.LineHeight.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.LineHeight);
				m_lineHeight = value;
			}
		}

		internal bool IsLineHeightAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.LineHeight);
			}
		}

		public override int NumeralVariant
		{
			get
			{
				if (m_numeralVariant == -1)
				{
					m_numeralVariant = m_styleDefinition.EvaluateInstanceStyleInt(StyleAttributeNames.NumeralVariant, 1);
				}
				return m_numeralVariant;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.NumeralVariant.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.NumeralVariant);
				m_numeralVariant = value;
			}
		}

		internal bool IsNumeralVariantAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.NumeralVariant);
			}
		}

		public override TextEffects TextEffect
		{
			get
			{
				if (!m_textEffect.HasValue)
				{
					m_textEffect = (TextEffects)m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.TextEffect);
				}
				return m_textEffect.Value;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.NumeralVariant.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.TextEffect);
				m_textEffect = value;
			}
		}

		internal bool IsTextEffectAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.TextEffect);
			}
		}

		public override BackgroundHatchTypes BackgroundHatchType
		{
			get
			{
				if (!m_backgroundHatchType.HasValue)
				{
					m_backgroundHatchType = (BackgroundHatchTypes)m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.BackgroundHatchType);
				}
				return m_backgroundHatchType.Value;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.NumeralVariant.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.BackgroundHatchType);
				m_backgroundHatchType = value;
			}
		}

		internal bool IsBackgroundHatchTypeAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.BackgroundHatchType);
			}
		}

		public override ReportColor ShadowColor
		{
			get
			{
				if (m_shadowColor == null)
				{
					m_shadowColor = m_styleDefinition.EvaluateInstanceReportColor(StyleAttributeNames.ShadowColor);
				}
				return m_shadowColor;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.NumeralVariant.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.ShadowColor);
				m_shadowColor = value;
			}
		}

		internal bool IsShadowColorAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.ShadowColor);
			}
		}

		public override ReportSize ShadowOffset
		{
			get
			{
				if (m_shadowOffset == null)
				{
					m_shadowOffset = m_styleDefinition.EvaluateInstanceReportSize(StyleAttributeNames.ShadowOffset);
				}
				return m_shadowOffset;
			}
			set
			{
				if (m_styleDefinition.ReportElement == null || m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_styleDefinition.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_styleDefinition.NumeralVariant.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				AssignedValueTo(StyleAttributeNames.ShadowColor);
				m_shadowOffset = value;
			}
		}

		internal bool IsShadowOffsetAssigned
		{
			get
			{
				if (m_assignedValues == null)
				{
					return false;
				}
				return m_assignedValues.ContainsKey(StyleAttributeNames.ShadowOffset);
			}
		}

		internal StyleInstance(IROMStyleDefinitionContainer styleDefinitionContainer, IReportScope reportScope, RenderingContext context)
			: base(context, reportScope)
		{
			m_styleDefinition = styleDefinitionContainer.Style;
		}

		protected override void ResetInstanceCache()
		{
			m_backgroundColor = null;
			m_backgroundGradientEndColor = null;
			m_color = null;
			m_fontStyle = null;
			m_fontFamily = null;
			m_fontWeight = null;
			m_format = null;
			m_textDecoration = null;
			m_textAlign = null;
			m_verticalAlign = null;
			m_direction = null;
			m_writingMode = null;
			m_language = null;
			m_unicodeBiDi = null;
			m_calendar = null;
			m_currencyLanguage = null;
			m_numeralLanguage = null;
			m_backgroundGradientType = null;
			m_fontSize = null;
			m_paddingLeft = null;
			m_paddingRight = null;
			m_paddingTop = null;
			m_paddingBottom = null;
			m_lineHeight = null;
			m_numeralVariant = -1;
			m_textEffect = null;
			m_backgroundHatchType = null;
			m_shadowColor = null;
			m_shadowOffset = null;
			m_assignedValues = null;
		}

		private void AssignedValueTo(StyleAttributeNames styleName)
		{
			if (m_assignedValues == null)
			{
				m_assignedValues = new Dictionary<StyleAttributeNames, bool>();
			}
			if (!m_assignedValues.ContainsKey(styleName))
			{
				m_assignedValues.Add(styleName, value: true);
			}
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			GetStyleDynamicValues(out List<int> styles, out List<Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo> values);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.StyleAttributes:
					writer.WriteListOfPrimitives(styles);
					break;
				case MemberName.StyleAttributeValues:
					writer.Write(values);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			List<int> styles = null;
			List<Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo> values = null;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.StyleAttributes:
					styles = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.StyleAttributeValues:
					values = reader.ReadListOfRIFObjects<List<Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo>>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
			SetStyleDynamicValues(styles, values);
		}

		void IPersistable.ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StyleInstance;
		}

		private static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.StyleAttributes, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
			list.Add(new MemberInfo(MemberName.StyleAttributeValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AttributeInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StyleInstance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		private void GetStyleDynamicValues(out List<int> styles, out List<Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo> values)
		{
			styles = new List<int>();
			values = new List<Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo>();
			m_styleDefinition.Border.Instance.GetAssignedDynamicValues(styles, values);
			m_styleDefinition.TopBorder.Instance.GetAssignedDynamicValues(styles, values);
			m_styleDefinition.BottomBorder.Instance.GetAssignedDynamicValues(styles, values);
			m_styleDefinition.LeftBorder.Instance.GetAssignedDynamicValues(styles, values);
			m_styleDefinition.RightBorder.Instance.GetAssignedDynamicValues(styles, values);
			if (IsBackgroundColorAssigned && m_styleDefinition.BackgroundColor.IsExpression)
			{
				styles.Add(15);
				values.Add(CreateAttrInfo(m_backgroundColor));
			}
			if (IsBackgroundGradientEndColorAssigned && m_styleDefinition.BackgroundGradientEndColor.IsExpression)
			{
				styles.Add(38);
				values.Add(CreateAttrInfo(m_backgroundGradientEndColor));
			}
			if (IsColorAssigned && m_styleDefinition.Color.IsExpression)
			{
				styles.Add(24);
				values.Add(CreateAttrInfo(m_color));
			}
			if (IsFontStyleAssigned && m_styleDefinition.FontStyle.IsExpression)
			{
				styles.Add(16);
				values.Add(CreateAttrInfo((int)m_fontStyle.Value));
			}
			if (IsFontFamilyAssigned && m_styleDefinition.FontFamily.IsExpression)
			{
				styles.Add(17);
				values.Add(CreateAttrInfo(m_fontFamily));
			}
			if (IsFontWeightAssigned && m_styleDefinition.FontWeight.IsExpression)
			{
				styles.Add(19);
				values.Add(CreateAttrInfo((int)m_fontWeight.Value));
			}
			if (IsFormatAssigned && m_styleDefinition.Format.IsExpression)
			{
				styles.Add(20);
				values.Add(CreateAttrInfo(m_format));
			}
			if (IsTextDecorationAssigned && m_styleDefinition.TextDecoration.IsExpression)
			{
				styles.Add(21);
				values.Add(CreateAttrInfo((int)m_textDecoration.Value));
			}
			if (IsTextAlignAssigned && m_styleDefinition.TextAlign.IsExpression)
			{
				styles.Add(22);
				values.Add(CreateAttrInfo((int)m_textAlign.Value));
			}
			if (IsVerticalAlignAssigned && m_styleDefinition.VerticalAlign.IsExpression)
			{
				styles.Add(23);
				values.Add(CreateAttrInfo((int)m_verticalAlign.Value));
			}
			if (IsDirectionAssigned && m_styleDefinition.Direction.IsExpression)
			{
				styles.Add(30);
				values.Add(CreateAttrInfo((int)m_direction.Value));
			}
			if (IsWritingModeAssigned && m_styleDefinition.WritingMode.IsExpression)
			{
				styles.Add(31);
				values.Add(CreateAttrInfo((int)m_writingMode.Value));
			}
			if (IsLanguageAssigned && m_styleDefinition.Language.IsExpression)
			{
				styles.Add(32);
				values.Add(CreateAttrInfo(m_language));
			}
			if (IsUnicodeBiDiAssigned && m_styleDefinition.UnicodeBiDi.IsExpression)
			{
				styles.Add(33);
				values.Add(CreateAttrInfo((int)m_unicodeBiDi.Value));
			}
			if (IsCalendarAssigned && m_styleDefinition.Calendar.IsExpression)
			{
				styles.Add(34);
				values.Add(CreateAttrInfo((int)m_calendar.Value));
			}
			if (IsCurrencyLanguageAssigned && m_styleDefinition.CurrencyLanguage.IsExpression)
			{
				styles.Add(50);
				values.Add(CreateAttrInfo(m_currencyLanguage));
			}
			if (IsNumeralLanguageAssigned && m_styleDefinition.NumeralLanguage.IsExpression)
			{
				styles.Add(35);
				values.Add(CreateAttrInfo(m_numeralLanguage));
			}
			if (IsBackgroundGradientTypeAssigned && m_styleDefinition.BackgroundGradientType.IsExpression)
			{
				styles.Add(37);
				values.Add(CreateAttrInfo((int)m_backgroundGradientType.Value));
			}
			if (IsFontSizeAssigned && m_styleDefinition.FontSize.IsExpression)
			{
				styles.Add(18);
				values.Add(CreateAttrInfo(m_fontSize));
			}
			if (IsPaddingLeftAssigned && m_styleDefinition.PaddingLeft.IsExpression)
			{
				styles.Add(25);
				values.Add(CreateAttrInfo(m_paddingLeft));
			}
			if (IsPaddingRightAssigned && m_styleDefinition.PaddingRight.IsExpression)
			{
				styles.Add(26);
				values.Add(CreateAttrInfo(m_paddingRight));
			}
			if (IsPaddingTopAssigned && m_styleDefinition.PaddingTop.IsExpression)
			{
				styles.Add(27);
				values.Add(CreateAttrInfo(m_paddingTop));
			}
			if (IsPaddingBottomAssigned && m_styleDefinition.PaddingBottom.IsExpression)
			{
				styles.Add(28);
				values.Add(CreateAttrInfo(m_paddingBottom));
			}
			if (IsLineHeightAssigned && m_styleDefinition.LineHeight.IsExpression)
			{
				styles.Add(29);
				values.Add(CreateAttrInfo(m_lineHeight));
			}
			if (IsNumeralVariantAssigned && m_styleDefinition.NumeralVariant.IsExpression)
			{
				styles.Add(36);
				values.Add(CreateAttrInfo(m_numeralVariant));
			}
			Global.Tracer.Assert(styles.Count == values.Count, "styles.Count == values.Count");
		}

		internal static Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo CreateAttrInfo(ReportColor reportColor)
		{
			return new Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo
			{
				IsExpression = true,
				Value = reportColor?.ToString()
			};
		}

		internal static Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo CreateAttrInfo(ReportSize reportSize)
		{
			return new Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo
			{
				IsExpression = true,
				Value = reportSize?.ToString()
			};
		}

		internal static Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo CreateAttrInfo(string strValue)
		{
			return new Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo
			{
				IsExpression = true,
				Value = strValue?.ToString()
			};
		}

		internal static Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo CreateAttrInfo(int intValue)
		{
			return new Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo
			{
				IsExpression = true,
				IntValue = intValue
			};
		}

		private void SetStyleDynamicValues(List<int> styles, List<Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo> values)
		{
			if (styles == null && values == null)
			{
				return;
			}
			Global.Tracer.Assert(styles != null && values != null && styles.Count == values.Count, "styles != null && values != null && styles.Count == values.Count");
			for (int i = 0; i < styles.Count; i++)
			{
				StyleAttributeNames styleAttributeNames = (StyleAttributeNames)styles[i];
				Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = values[i];
				switch (styleAttributeNames)
				{
				case StyleAttributeNames.BorderColor:
					m_styleDefinition.Border.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Color, attributeInfo, m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderColorTop:
					m_styleDefinition.TopBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Color, attributeInfo, m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderColorLeft:
					m_styleDefinition.LeftBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Color, attributeInfo, m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderColorRight:
					m_styleDefinition.RightBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Color, attributeInfo, m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderColorBottom:
					m_styleDefinition.BottomBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Color, attributeInfo, m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderStyle:
					m_styleDefinition.Border.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Style, attributeInfo, m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderStyleTop:
					m_styleDefinition.TopBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Style, attributeInfo, m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderStyleLeft:
					m_styleDefinition.LeftBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Style, attributeInfo, m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderStyleRight:
					m_styleDefinition.RightBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Style, attributeInfo, m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderStyleBottom:
					m_styleDefinition.BottomBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Style, attributeInfo, m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderWidth:
					m_styleDefinition.Border.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Width, attributeInfo, m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderWidthTop:
					m_styleDefinition.TopBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Width, attributeInfo, m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderWidthLeft:
					m_styleDefinition.LeftBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Width, attributeInfo, m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderWidthRight:
					m_styleDefinition.RightBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Width, attributeInfo, m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderWidthBottom:
					m_styleDefinition.BottomBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Width, attributeInfo, m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BackgroundColor:
					m_backgroundColor = new ReportColor(attributeInfo.Value, m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BackgroundGradientEndColor:
					m_backgroundGradientEndColor = new ReportColor(attributeInfo.Value, m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BackgroundGradientType:
					m_backgroundGradientType = (BackgroundGradients)attributeInfo.IntValue;
					break;
				case StyleAttributeNames.Calendar:
					m_calendar = (Calendars)attributeInfo.IntValue;
					break;
				case StyleAttributeNames.Color:
					m_color = new ReportColor(attributeInfo.Value, m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.Direction:
					m_direction = (Directions)attributeInfo.IntValue;
					break;
				case StyleAttributeNames.FontFamily:
					m_fontFamily = attributeInfo.Value;
					break;
				case StyleAttributeNames.FontSize:
					m_fontSize = new ReportSize(attributeInfo.Value);
					break;
				case StyleAttributeNames.FontStyle:
					m_fontStyle = (FontStyles)attributeInfo.IntValue;
					break;
				case StyleAttributeNames.FontWeight:
					m_fontWeight = (FontWeights)attributeInfo.IntValue;
					break;
				case StyleAttributeNames.Format:
					m_format = attributeInfo.Value;
					break;
				case StyleAttributeNames.Language:
					m_language = attributeInfo.Value;
					break;
				case StyleAttributeNames.LineHeight:
					m_lineHeight = new ReportSize(attributeInfo.Value);
					break;
				case StyleAttributeNames.CurrencyLanguage:
					m_currencyLanguage = attributeInfo.Value;
					break;
				case StyleAttributeNames.NumeralLanguage:
					m_numeralLanguage = attributeInfo.Value;
					break;
				case StyleAttributeNames.NumeralVariant:
					m_numeralVariant = attributeInfo.IntValue;
					break;
				case StyleAttributeNames.PaddingBottom:
					m_paddingBottom = new ReportSize(attributeInfo.Value);
					break;
				case StyleAttributeNames.PaddingLeft:
					m_paddingLeft = new ReportSize(attributeInfo.Value);
					break;
				case StyleAttributeNames.PaddingRight:
					m_paddingRight = new ReportSize(attributeInfo.Value);
					break;
				case StyleAttributeNames.PaddingTop:
					m_paddingTop = new ReportSize(attributeInfo.Value);
					break;
				case StyleAttributeNames.TextAlign:
					m_textAlign = (TextAlignments)attributeInfo.IntValue;
					break;
				case StyleAttributeNames.TextDecoration:
					m_textDecoration = (TextDecorations)attributeInfo.IntValue;
					break;
				case StyleAttributeNames.UnicodeBiDi:
					m_unicodeBiDi = (UnicodeBiDiTypes)attributeInfo.IntValue;
					break;
				case StyleAttributeNames.VerticalAlign:
					m_verticalAlign = (VerticalAlignments)attributeInfo.IntValue;
					break;
				case StyleAttributeNames.WritingMode:
					m_writingMode = (WritingModes)attributeInfo.IntValue;
					break;
				}
			}
		}
	}
}

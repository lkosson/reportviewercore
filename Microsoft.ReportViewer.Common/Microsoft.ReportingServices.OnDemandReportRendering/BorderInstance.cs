using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class BorderInstance : BaseInstance
	{
		internal enum BorderStyleProperty
		{
			Color,
			Style,
			Width
		}

		private Border m_owner;

		private BorderStyles m_defaultStyleValueIfExpressionNull;

		private ReportColor m_color;

		private BorderStyles m_style;

		private ReportSize m_width;

		private bool m_colorEvaluated;

		private bool m_colorAssigned;

		private bool m_styleEvaluated;

		private bool m_styleAssigned;

		private bool m_widthEvaluated;

		private bool m_widthAssigned;

		public ReportColor Color
		{
			get
			{
				if (!m_colorEvaluated)
				{
					m_colorEvaluated = true;
					m_color = m_owner.Owner.EvaluateInstanceReportColor(m_owner.ColorAttrName);
				}
				return m_color;
			}
			set
			{
				if (m_owner.Owner.ReportElement == null || m_owner.Owner.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_owner.Owner.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_owner.Color.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				m_colorEvaluated = true;
				m_colorAssigned = true;
				m_color = value;
			}
		}

		internal bool IsColorAssigned => m_colorAssigned;

		public BorderStyles Style
		{
			get
			{
				if (!m_styleEvaluated)
				{
					m_styleEvaluated = true;
					m_style = (BorderStyles)m_owner.Owner.EvaluateInstanceStyleEnum(m_owner.StyleAttrName, (int)m_defaultStyleValueIfExpressionNull);
					if (m_style == BorderStyles.Default && m_owner.BorderPosition == Border.Position.Default)
					{
						m_style = m_defaultStyleValueIfExpressionNull;
					}
				}
				return m_style;
			}
			set
			{
				if (m_owner.Owner.ReportElement == null || m_owner.Owner.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_owner.Owner.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_owner.Style.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				string text = value.ToString();
				if (!Microsoft.ReportingServices.ReportPublishing.Validator.ValidateBorderStyle(text, m_owner.BorderPosition == Border.Position.Default, m_owner.Owner.StyleContainer.ObjectType, Microsoft.ReportingServices.ReportPublishing.Validator.IsDynamicImageSubElement(m_owner.Owner.StyleContainer), out string validatedStyle))
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				m_styleEvaluated = true;
				m_styleAssigned = true;
				if (validatedStyle != text)
				{
					m_style = StyleTranslator.TranslateBorderStyle(validatedStyle, null);
				}
				else
				{
					m_style = value;
				}
			}
		}

		internal bool IsStyleAssigned => m_styleAssigned;

		public ReportSize Width
		{
			get
			{
				if (!m_widthEvaluated)
				{
					m_widthEvaluated = true;
					m_width = m_owner.Owner.EvaluateInstanceReportSize(m_owner.WidthAttrName);
				}
				return m_width;
			}
			set
			{
				if (m_owner.Owner.ReportElement == null || m_owner.Owner.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_owner.Owner.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_owner.Width.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				m_widthEvaluated = true;
				m_widthAssigned = true;
				m_width = value;
			}
		}

		internal bool IsWidthAssigned => m_widthAssigned;

		internal BorderInstance(Border owner, IReportScope reportScope, BorderStyles defaultStyleValueIfExpressionNull)
			: base(reportScope)
		{
			m_owner = owner;
			m_defaultStyleValueIfExpressionNull = defaultStyleValueIfExpressionNull;
		}

		protected override void ResetInstanceCache()
		{
			m_colorEvaluated = false;
			m_colorAssigned = false;
			m_color = null;
			m_styleEvaluated = false;
			m_styleAssigned = false;
			m_widthEvaluated = false;
			m_widthAssigned = false;
			m_width = null;
		}

		internal void GetAssignedDynamicValues(List<int> styles, List<Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo> values)
		{
			if (m_colorAssigned && m_owner.Color.IsExpression)
			{
				styles.Add((int)m_owner.ColorAttrName);
				values.Add(StyleInstance.CreateAttrInfo(m_color));
			}
			if (m_styleAssigned && m_owner.Style.IsExpression)
			{
				styles.Add((int)m_owner.StyleAttrName);
				values.Add(StyleInstance.CreateAttrInfo((int)m_style));
			}
			if (m_widthAssigned && m_owner.Width.IsExpression)
			{
				styles.Add((int)m_owner.WidthAttrName);
				values.Add(StyleInstance.CreateAttrInfo(m_width));
			}
		}

		internal void SetAssignedDynamicValue(BorderStyleProperty prop, Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo value, bool allowTransparency)
		{
			switch (prop)
			{
			case BorderStyleProperty.Color:
				m_colorEvaluated = true;
				m_color = new ReportColor(value.Value, allowTransparency);
				break;
			case BorderStyleProperty.Style:
				m_styleEvaluated = true;
				m_style = (BorderStyles)value.IntValue;
				break;
			case BorderStyleProperty.Width:
				m_widthEvaluated = true;
				m_width = new ReportSize(value.Value);
				break;
			}
		}
	}
}

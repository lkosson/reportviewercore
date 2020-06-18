using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class Border
	{
		internal enum Position
		{
			Default,
			Top,
			Left,
			Right,
			Bottom
		}

		private Style m_owner;

		private Position m_position;

		private bool m_defaultSolidBorderStyle;

		private BorderInstance m_instance;

		public ReportColorProperty Color => m_owner[ColorAttrName] as ReportColorProperty;

		public ReportEnumProperty<BorderStyles> Style => m_owner[StyleAttrName] as ReportEnumProperty<BorderStyles>;

		public ReportSizeProperty Width => m_owner[WidthAttrName] as ReportSizeProperty;

		public BorderInstance Instance
		{
			get
			{
				GetInstance();
				(Owner.ReportElement as ReportItem)?.CriEvaluateInstance();
				return m_instance;
			}
		}

		internal Style Owner => m_owner;

		internal Position BorderPosition => m_position;

		internal StyleAttributeNames ColorAttrName
		{
			get
			{
				switch (m_position)
				{
				case Position.Default:
					return StyleAttributeNames.BorderColor;
				case Position.Top:
					return StyleAttributeNames.BorderColorTop;
				case Position.Right:
					return StyleAttributeNames.BorderColorRight;
				case Position.Bottom:
					return StyleAttributeNames.BorderColorBottom;
				case Position.Left:
					return StyleAttributeNames.BorderColorLeft;
				default:
					Global.Tracer.Assert(condition: false);
					return StyleAttributeNames.BorderColor;
				}
			}
		}

		internal StyleAttributeNames StyleAttrName
		{
			get
			{
				switch (m_position)
				{
				case Position.Default:
					return StyleAttributeNames.BorderStyle;
				case Position.Top:
					return StyleAttributeNames.BorderStyleTop;
				case Position.Right:
					return StyleAttributeNames.BorderStyleRight;
				case Position.Bottom:
					return StyleAttributeNames.BorderStyleBottom;
				case Position.Left:
					return StyleAttributeNames.BorderStyleLeft;
				default:
					Global.Tracer.Assert(condition: false);
					return StyleAttributeNames.BorderColor;
				}
			}
		}

		internal StyleAttributeNames WidthAttrName
		{
			get
			{
				switch (m_position)
				{
				case Position.Default:
					return StyleAttributeNames.BorderWidth;
				case Position.Top:
					return StyleAttributeNames.BorderWidthTop;
				case Position.Right:
					return StyleAttributeNames.BorderWidthRight;
				case Position.Bottom:
					return StyleAttributeNames.BorderWidthBottom;
				case Position.Left:
					return StyleAttributeNames.BorderWidthLeft;
				default:
					Global.Tracer.Assert(condition: false);
					return StyleAttributeNames.BorderColor;
				}
			}
		}

		internal Border(Style owner, Position position, bool defaultSolidBorderStyle)
		{
			m_owner = owner;
			m_position = position;
			m_defaultSolidBorderStyle = defaultSolidBorderStyle;
		}

		internal BorderInstance GetInstance()
		{
			if (m_owner.m_renderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				BorderStyles defaultStyleValueIfExpressionNull = (!m_defaultSolidBorderStyle) ? BorderStyles.None : BorderStyles.Solid;
				if (m_owner.IsOldSnapshot)
				{
					m_instance = new BorderInstance(this, null, defaultStyleValueIfExpressionNull);
				}
				else
				{
					m_instance = new BorderInstance(this, m_owner.ReportScope, defaultStyleValueIfExpressionNull);
				}
			}
			return m_instance;
		}

		internal void ConstructBorderDefinition()
		{
			Global.Tracer.Assert(m_owner.ReportElement != null, "(m_owner.ReportElement != null)");
			Global.Tracer.Assert(m_owner.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Definition, "(m_owner.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Definition)");
			Global.Tracer.Assert(m_owner.StyleContainer.StyleClass != null, "(m_owner.StyleContainer.StyleClass != null)");
			Global.Tracer.Assert(!Color.IsExpression, "(!this.Color.IsExpression)");
			if (Instance.IsColorAssigned)
			{
				string value = (Instance.Color != null) ? Instance.Color.ToString() : null;
				m_owner.StyleContainer.StyleClass.AddAttribute(m_owner.GetStyleStringFromEnum(ColorAttrName), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value));
			}
			else
			{
				m_owner.StyleContainer.StyleClass.AddAttribute(m_owner.GetStyleStringFromEnum(ColorAttrName), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(Style == null || !Style.IsExpression, "(this.Style == null || !this.Style.IsExpression)");
			if (Instance.IsStyleAssigned)
			{
				m_owner.StyleContainer.StyleClass.AddAttribute(m_owner.GetStyleStringFromEnum(StyleAttrName), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(Instance.Style.ToString()));
			}
			else
			{
				m_owner.StyleContainer.StyleClass.AddAttribute(m_owner.GetStyleStringFromEnum(StyleAttrName), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(Width == null || !Width.IsExpression, "(this.Width == null || !this.Width.IsExpression)");
			if (Instance.IsWidthAssigned)
			{
				string value2 = (Instance.Width != null) ? Instance.Width.ToString() : null;
				m_owner.StyleContainer.StyleClass.AddAttribute(m_owner.GetStyleStringFromEnum(WidthAttrName), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value2));
			}
			else
			{
				m_owner.StyleContainer.StyleClass.AddAttribute(m_owner.GetStyleStringFromEnum(WidthAttrName), Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
		}
	}
}

using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimTextRun : TextRun
	{
		public override string ID => base.TextBox.ID + "xL";

		public override Style Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new TextRunFilteredStyle(RenderReportItem, base.RenderingContext, UseRenderStyle);
				}
				return m_style;
			}
		}

		public override ReportStringProperty Value
		{
			get
			{
				if (m_value == null)
				{
					Microsoft.ReportingServices.ReportProcessing.TextBox textBox = (Microsoft.ReportingServices.ReportProcessing.TextBox)RenderReportItem.ReportItemDef;
					m_value = new ReportStringProperty(textBox.Value, textBox.Formula);
				}
				return m_value;
			}
		}

		public override TypeCode SharedTypeCode => ((Microsoft.ReportingServices.ReportRendering.TextBox)base.TextBox.RenderReportItem).SharedTypeCode;

		public override ReportEnumProperty<MarkupType> MarkupType
		{
			get
			{
				if (m_markupType == null)
				{
					m_markupType = new ReportEnumProperty<MarkupType>(Microsoft.ReportingServices.OnDemandReportRendering.MarkupType.None);
				}
				return m_markupType;
			}
		}

		public override bool FormattedValueExpressionBased
		{
			get
			{
				if (!m_formattedValueExpressionBased.HasValue)
				{
					Microsoft.ReportingServices.ReportProcessing.TextBox textBox = (Microsoft.ReportingServices.ReportProcessing.TextBox)RenderReportItem.ReportItemDef;
					if (textBox.Value != null)
					{
						m_formattedValueExpressionBased = textBox.Value.IsExpression;
					}
					else
					{
						m_formattedValueExpressionBased = false;
					}
				}
				return m_formattedValueExpressionBased.Value;
			}
		}

		public override TextRunInstance Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new ShimTextRunInstance(this, (TextBoxInstance)base.TextBox.Instance);
				}
				return m_instance;
			}
		}

		internal ShimTextRun(Paragraph paragraph, RenderingContext renderingContext)
			: base(paragraph, renderingContext)
		{
		}

		internal override void UpdateRenderReportItem(Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			if (m_style != null)
			{
				m_style.UpdateStyleCache(renderReportItem);
			}
		}
	}
}

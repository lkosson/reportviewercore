using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimParagraph : Paragraph
	{
		public override string ID => base.TextBox.ID + "xK";

		public override Style Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new ParagraphFilteredStyle(RenderReportItem, base.RenderingContext, UseRenderStyle);
				}
				return m_style;
			}
		}

		public override ReportEnumProperty<ListStyle> ListStyle
		{
			get
			{
				if (m_listStyle == null)
				{
					m_listStyle = new ReportEnumProperty<ListStyle>(Microsoft.ReportingServices.OnDemandReportRendering.ListStyle.None);
				}
				return m_listStyle;
			}
		}

		public override ReportIntProperty ListLevel
		{
			get
			{
				if (m_listLevel == null)
				{
					m_listLevel = new ReportIntProperty(0);
				}
				return m_listLevel;
			}
		}

		public override ParagraphInstance Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new ShimParagraphInstance(this);
				}
				return m_instance;
			}
		}

		internal ShimParagraph(TextBox textBox, RenderingContext renderingContext)
			: base(textBox, renderingContext)
		{
		}

		internal override void UpdateRenderReportItem(Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			base.UpdateRenderReportItem(renderReportItem);
			if (m_style != null)
			{
				m_style.UpdateStyleCache(renderReportItem);
			}
		}
	}
}

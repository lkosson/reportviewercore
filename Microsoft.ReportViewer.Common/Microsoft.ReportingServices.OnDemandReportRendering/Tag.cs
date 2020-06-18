using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class Tag
	{
		private readonly Image m_image;

		private readonly ExpressionInfo m_expression;

		private ReportVariantProperty m_value;

		private TagInstance m_instance;

		public ReportVariantProperty Value
		{
			get
			{
				if (m_value == null)
				{
					m_value = new ReportVariantProperty(m_expression);
				}
				return m_value;
			}
		}

		public TagInstance Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new TagInstance(this);
				}
				return m_instance;
			}
		}

		internal Image Image => m_image;

		internal ExpressionInfo Expression => m_expression;

		internal Tag(Image image, ExpressionInfo expression)
		{
			m_image = image;
			m_expression = expression;
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.ResetInstanceCache();
			}
		}
	}
}

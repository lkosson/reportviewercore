using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class PageBreakInstance : BaseInstance
	{
		private bool? m_resetPageNumber;

		private bool? m_disabled;

		private PageBreak m_pageBreakDef;

		public bool Disabled
		{
			get
			{
				if (!m_disabled.HasValue)
				{
					if (m_pageBreakDef.IsOldSnapshot)
					{
						m_disabled = false;
					}
					else
					{
						ExpressionInfo disabled = m_pageBreakDef.PageBreakDef.Disabled;
						if (disabled != null)
						{
							if (disabled.IsExpression)
							{
								m_disabled = m_pageBreakDef.PageBreakDef.EvaluateDisabled(ReportScopeInstance, m_pageBreakDef.RenderingContext.OdpContext, m_pageBreakDef.PageBreakOwner);
							}
							else
							{
								m_disabled = disabled.BoolValue;
							}
						}
						else
						{
							m_disabled = false;
						}
					}
				}
				return m_disabled.Value;
			}
		}

		public bool ResetPageNumber
		{
			get
			{
				if (!m_resetPageNumber.HasValue)
				{
					if (m_pageBreakDef.IsOldSnapshot)
					{
						m_resetPageNumber = false;
					}
					else
					{
						ExpressionInfo resetPageNumber = m_pageBreakDef.PageBreakDef.ResetPageNumber;
						if (resetPageNumber != null)
						{
							if (resetPageNumber.IsExpression)
							{
								m_resetPageNumber = m_pageBreakDef.PageBreakDef.EvaluateResetPageNumber(ReportScopeInstance, m_pageBreakDef.RenderingContext.OdpContext, m_pageBreakDef.PageBreakOwner);
							}
							else
							{
								m_resetPageNumber = resetPageNumber.BoolValue;
							}
						}
						else
						{
							m_resetPageNumber = false;
						}
					}
				}
				return m_resetPageNumber.Value;
			}
		}

		internal PageBreakInstance(IReportScope reportScope, PageBreak pageBreakDef)
			: base(reportScope)
		{
			m_pageBreakDef = pageBreakDef;
		}

		protected override void ResetInstanceCache()
		{
			m_resetPageNumber = null;
			m_disabled = null;
		}
	}
}

using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportItemVisibilityInstance : VisibilityInstance
	{
		private ReportItem m_reportItem;

		public override bool CurrentlyHidden
		{
			get
			{
				if (!m_cachedCurrentlyHidden)
				{
					m_cachedCurrentlyHidden = true;
					if (m_reportItem.IsOldSnapshot)
					{
						m_currentlyHiddenValue = m_reportItem.RenderReportItem.Hidden;
					}
					else
					{
						m_currentlyHiddenValue = m_reportItem.ReportItemDef.ComputeHidden(m_reportItem.RenderingContext, ToggleCascadeDirection.None);
					}
				}
				return m_currentlyHiddenValue;
			}
		}

		public override bool StartHidden
		{
			get
			{
				if (!m_cachedStartHidden)
				{
					m_cachedStartHidden = true;
					if (m_reportItem.IsOldSnapshot)
					{
						m_startHiddenValue = m_reportItem.RenderReportItem.Hidden;
					}
					else
					{
						m_startHiddenValue = m_reportItem.ReportItemDef.ComputeStartHidden(m_reportItem.RenderingContext);
					}
				}
				return m_startHiddenValue;
			}
		}

		internal ReportItemVisibilityInstance(ReportItem reportitem)
			: base(reportitem.ReportScope)
		{
			m_reportItem = reportitem;
		}
	}
}

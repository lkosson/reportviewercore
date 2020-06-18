using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class PageInstance : ReportElementInstance, IReportScopeInstance
	{
		private bool m_isNewContext;

		public string UniqueName
		{
			get
			{
				if (m_reportElementDef.IsOldSnapshot)
				{
					return PageDefinition.RenderReport.UniqueName + "xP";
				}
				Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection reportSection = (Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection)PageDefinition.ReportItemDef;
				return InstancePathItem.GenerateUniqueNameString(reportSection.ID, reportSection.InstancePath) + "xP";
			}
		}

		public override StyleInstance Style
		{
			get
			{
				Page pageDefinition = PageDefinition;
				if (pageDefinition.ShouldUseFirstSection)
				{
					return pageDefinition.FirstSectionPage.Instance.Style;
				}
				return base.Style;
			}
		}

		internal Page PageDefinition => (Page)m_reportElementDef;

		IReportScope IReportScopeInstance.ReportScope => PageDefinition;

		bool IReportScopeInstance.IsNewContext
		{
			get
			{
				return m_isNewContext;
			}
			set
			{
				m_isNewContext = value;
			}
		}

		internal PageInstance(Page pageDef)
			: base(pageDef)
		{
		}
	}
}

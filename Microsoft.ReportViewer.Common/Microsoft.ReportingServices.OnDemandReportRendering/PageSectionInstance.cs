using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class PageSectionInstance : ReportElementInstance
	{
		public string UniqueName
		{
			get
			{
				if (m_reportElementDef.IsOldSnapshot)
				{
					return PageSectionDefinition.PageDefinition.Instance.UniqueName + PageSectionDefinition.RenderReportItem.UniqueName;
				}
				string str = PageSectionDefinition.IsHeader ? "xH" : "xF";
				Microsoft.ReportingServices.ReportIntermediateFormat.PageSection pageSection = (Microsoft.ReportingServices.ReportIntermediateFormat.PageSection)PageSectionDefinition.ReportItemDef;
				return InstancePathItem.GenerateUniqueNameString(pageSection.ID, pageSection.InstancePath) + str;
			}
		}

		internal PageSection PageSectionDefinition => (PageSection)m_reportElementDef;

		internal PageSectionInstance(PageSection pageSectionDef)
			: base(pageSectionDef)
		{
		}
	}
}

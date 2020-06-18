using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportSectionInstance : BaseInstance, IReportScopeInstance
	{
		private bool m_isNewContext;

		internal ReportSection SectionDef => (ReportSection)m_reportScope;

		IReportScope IReportScopeInstance.ReportScope => m_reportScope;

		public string UniqueName
		{
			get
			{
				if (SectionDef.IsOldSnapshot)
				{
					return SectionDef.Report.RenderReport.UniqueName + "xE";
				}
				Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection sectionDef = SectionDef.SectionDef;
				return InstancePathItem.GenerateUniqueNameString(sectionDef.ID, sectionDef.InstancePath);
			}
		}

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

		internal ReportSectionInstance(ReportSection sectionDef)
			: base(sectionDef)
		{
		}

		protected override void ResetInstanceCache()
		{
		}

		internal override void SetNewContext()
		{
			m_isNewContext = true;
			base.SetNewContext();
		}
	}
}

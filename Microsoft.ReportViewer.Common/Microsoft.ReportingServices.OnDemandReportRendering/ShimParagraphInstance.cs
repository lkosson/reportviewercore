using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimParagraphInstance : ParagraphInstance
	{
		public override string UniqueName
		{
			get
			{
				if (m_uniqueName == null)
				{
					Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem = m_reportElementDef.RenderReportItem;
					m_uniqueName = renderReportItem.ID + "x0i" + renderReportItem.UniqueName;
				}
				return m_uniqueName;
			}
		}

		public override bool IsCompiled => false;

		internal ShimParagraphInstance(Paragraph paragraphDef)
			: base(paragraphDef)
		{
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
		}
	}
}

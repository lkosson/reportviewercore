using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class CustomReportItemInstance : ReportItemInstance
	{
		public bool NoRows
		{
			get
			{
				if (m_reportElementDef.IsOldSnapshot)
				{
					return ((CustomReportItem)m_reportElementDef).RenderCri.CustomData.NoRows;
				}
				m_reportElementDef.RenderingContext.OdpContext.SetupContext(m_reportElementDef.ReportItemDef, ReportScopeInstance);
				return ((Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion)m_reportElementDef.ReportItemDef).NoRows;
			}
		}

		internal CustomReportItemInstance(CustomReportItem reportItemDef)
			: base(reportItemDef)
		{
		}
	}
}

using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class SubReportInstance : ReportItemInstance
	{
		private string m_noRowsMessageExpressionResult;

		public bool ProcessedWithError => SubReportDefinition.ProcessedWithError;

		public SubReportErrorCodes ErrorCode => SubReportDefinition.ErrorCode;

		public string ErrorMessage => SubReportDefinition.ErrorMessage;

		public string NoRowsMessage
		{
			get
			{
				if (m_noRowsMessageExpressionResult == null)
				{
					if (SubReportDefinition.IsOldSnapshot)
					{
						m_noRowsMessageExpressionResult = ((Microsoft.ReportingServices.ReportRendering.SubReport)SubReportDefinition.RenderReportItem).NoRowMessage;
					}
					else if (!SubReportDefinition.ProcessedWithError)
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport = (Microsoft.ReportingServices.ReportIntermediateFormat.SubReport)SubReportDefinition.ReportItemDef;
						m_noRowsMessageExpressionResult = subReport.EvaulateNoRowMessage(ReportScopeInstance, m_reportElementDef.RenderingContext.OdpContext);
					}
				}
				return m_noRowsMessageExpressionResult;
			}
		}

		public bool NoRows => SubReportDefinition.NoRows;

		private SubReport SubReportDefinition => m_reportElementDef as SubReport;

		internal SubReportInstance(SubReport reportItemDef)
			: base(reportItemDef)
		{
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_noRowsMessageExpressionResult = null;
		}
	}
}

using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class SubReport : ReportItem
	{
		private Report m_report;

		private bool m_processedWithError;

		public Report Report => m_report;

		public bool ProcessedWithError => m_processedWithError;

		public bool NoRows
		{
			get
			{
				if (m_processedWithError)
				{
					return false;
				}
				Global.Tracer.Assert(m_report != null);
				if (m_report.ReportInstance != null)
				{
					return m_report.InstanceInfo.NoRows;
				}
				return true;
			}
		}

		public string NoRowMessage
		{
			get
			{
				ExpressionInfo noRows = ((Microsoft.ReportingServices.ReportProcessing.SubReport)base.ReportItemDef).NoRows;
				if (noRows != null)
				{
					if (ExpressionInfo.Types.Constant == noRows.Type)
					{
						return noRows.Value;
					}
					if (base.InstanceInfo != null)
					{
						return ((SubReportInstanceInfo)base.InstanceInfo).NoRows;
					}
				}
				return null;
			}
		}

		internal SubReport(int intUniqueName, Microsoft.ReportingServices.ReportProcessing.SubReport reportItemDef, SubReportInstance reportItemInstance, RenderingContext renderingContext, Report innerReport, bool processedWithError)
			: base(null, intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
			m_report = innerReport;
			m_processedWithError = processedWithError;
		}

		internal override bool Search(SearchContext searchContext)
		{
			if (base.SkipSearch || NoRows || ProcessedWithError)
			{
				return false;
			}
			return Report?.Body.Search(searchContext) ?? false;
		}
	}
}

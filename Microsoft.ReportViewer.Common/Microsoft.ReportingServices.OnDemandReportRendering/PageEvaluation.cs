namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class PageEvaluation
	{
		protected int m_currentPageNumber = 1;

		protected int m_totalPages = 1;

		protected int m_overallTotalPages = 1;

		protected int m_currentOverallPageNumber = 1;

		protected Report m_romReport;

		protected string m_pageName;

		protected PageEvaluation(Report report)
		{
			m_romReport = report;
		}

		internal virtual void Reset(ReportSection section, int newPageNumber, int newTotalPages, int newOverallPageNumber, int newOverallTotalPages)
		{
			m_currentPageNumber = newPageNumber;
			m_totalPages = newTotalPages;
			m_currentOverallPageNumber = newOverallPageNumber;
			m_overallTotalPages = newOverallTotalPages;
		}

		internal virtual void SetPageName(string pageName)
		{
			m_pageName = pageName;
		}

		internal abstract void Add(string textboxName, object textboxValue);

		internal abstract void UpdatePageSections(ReportSection section);
	}
}

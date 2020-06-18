using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class PageCollection
	{
		private PaginationInfo m_paginationDef;

		private Report m_report;

		public int TotalCount
		{
			get
			{
				return m_paginationDef.TotalPageNumber;
			}
			set
			{
				m_paginationDef.TotalPageNumber = value;
			}
		}

		public Page this[int pageNumber]
		{
			get
			{
				if (0 > pageNumber || pageNumber >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, pageNumber, 0, Count);
				}
				Page page = m_paginationDef[pageNumber];
				if (page != null && m_report != null)
				{
					if (page.PageSectionHeader == null)
					{
						page.PageSectionHeader = GetHeader(page.HeaderInstance);
					}
					if (page.PageSectionFooter == null)
					{
						page.PageSectionFooter = GetFooter(page.FooterInstance);
					}
				}
				return page;
			}
			set
			{
				if (0 > pageNumber || pageNumber >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, pageNumber, 0, Count);
				}
				m_paginationDef[pageNumber] = value;
			}
		}

		public int Count => m_paginationDef.CurrentPageCount;

		internal PageCollection(PaginationInfo paginationDef, Report report)
		{
			m_paginationDef = paginationDef;
			m_report = report;
		}

		public void Add(Page page)
		{
			m_paginationDef.AddPage(page);
		}

		public void Clear()
		{
			m_paginationDef.Clear();
		}

		public void Insert(int index, Page page)
		{
			m_paginationDef.InsertPage(index, page);
		}

		public void RemoveAt(int index)
		{
			m_paginationDef.RemovePage(index);
		}

		internal PageSection GetHeader(PageSectionInstance headerInstance)
		{
			PageSection result = null;
			Microsoft.ReportingServices.ReportProcessing.Report reportDef = m_report.ReportDef;
			if (reportDef != null)
			{
				if (!reportDef.PageHeaderEvaluation)
				{
					result = m_report.PageHeader;
				}
				else if (reportDef.PageHeader != null && headerInstance != null)
				{
					string text = headerInstance.PageNumber + "ph";
					RenderingContext renderingContext = new RenderingContext(m_report.RenderingContext, text);
					result = new PageSection(text, reportDef.PageHeader, headerInstance, m_report, renderingContext, pageDef: false);
				}
			}
			return result;
		}

		internal PageSection GetFooter(PageSectionInstance footerInstance)
		{
			PageSection result = null;
			Microsoft.ReportingServices.ReportProcessing.Report reportDef = m_report.ReportDef;
			if (reportDef != null)
			{
				if (!reportDef.PageFooterEvaluation)
				{
					result = m_report.PageFooter;
				}
				else if (reportDef.PageFooter != null && footerInstance != null)
				{
					string text = footerInstance.PageNumber + "pf";
					RenderingContext renderingContext = new RenderingContext(m_report.RenderingContext, text);
					result = new PageSection(text, reportDef.PageFooter, footerInstance, m_report, renderingContext, pageDef: false);
				}
			}
			return result;
		}
	}
}

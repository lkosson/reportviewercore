using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportSection : IDefinitionPath, IReportScope
	{
		private Microsoft.ReportingServices.ReportRendering.Report m_renderReport;

		private Report m_reportDef;

		private int m_sectionIndex;

		private ReportSectionInstance m_instance;

		private Page m_page;

		private Body m_body;

		private string m_definitionPath;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection m_sectionDef;

		private ReportItemsImpl m_bodyItemsForHeadFoot;

		private ReportItemsImpl m_pageSectionItemsForHeadFoot;

		private Dictionary<string, AggregatesImpl> m_pageAggregatesOverReportItems = new Dictionary<string, AggregatesImpl>();

		public string Name
		{
			get
			{
				if (IsOldSnapshot)
				{
					return "ReportSection0";
				}
				return m_sectionDef.Name;
			}
		}

		public Body Body
		{
			get
			{
				if (m_body == null)
				{
					if (IsOldSnapshot)
					{
						m_body = new Body(this, m_reportDef.SubreportInSubtotal, m_renderReport, m_reportDef.RenderingContext);
					}
					else
					{
						m_body = new Body(this, this, m_sectionDef, m_reportDef.RenderingContext);
					}
				}
				return m_body;
			}
		}

		public ReportSize Width
		{
			get
			{
				if (IsOldSnapshot)
				{
					return new ReportSize(m_renderReport.Width);
				}
				if (m_sectionDef.WidthForRendering == null)
				{
					m_sectionDef.WidthForRendering = new ReportSize(m_sectionDef.Width, m_sectionDef.WidthValue);
				}
				return m_sectionDef.WidthForRendering;
			}
		}

		public Page Page
		{
			get
			{
				if (m_page == null)
				{
					if (m_reportDef.IsOldSnapshot)
					{
						m_page = new Page(this, m_reportDef.RenderReport, m_reportDef.RenderingContext, this);
					}
					else
					{
						m_page = new Page(this, m_reportDef.RenderingContext, this);
					}
				}
				return m_page;
			}
		}

		public string DataElementName
		{
			get
			{
				if (IsOldSnapshot)
				{
					return string.Empty;
				}
				return m_sectionDef.DataElementName;
			}
		}

		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (IsOldSnapshot)
				{
					return DataElementOutputTypes.ContentsOnly;
				}
				return m_sectionDef.DataElementOutput;
			}
		}

		public ReportSectionInstance Instance
		{
			get
			{
				if (m_reportDef.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ReportSectionInstance(this);
				}
				return m_instance;
			}
		}

		public string ID
		{
			get
			{
				if (IsOldSnapshot)
				{
					return m_renderReport.Body.ID + "xE";
				}
				return m_sectionDef.RenderingModelID;
			}
		}

		internal bool IsOldSnapshot => m_reportDef.IsOldSnapshot;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection SectionDef => m_sectionDef;

		internal ReportItemsImpl BodyItemsForHeadFoot
		{
			get
			{
				return m_bodyItemsForHeadFoot;
			}
			set
			{
				m_bodyItemsForHeadFoot = value;
			}
		}

		internal ReportItemsImpl PageSectionItemsForHeadFoot
		{
			get
			{
				return m_pageSectionItemsForHeadFoot;
			}
			set
			{
				m_pageSectionItemsForHeadFoot = value;
			}
		}

		internal Dictionary<string, AggregatesImpl> PageAggregatesOverReportItems
		{
			get
			{
				return m_pageAggregatesOverReportItems;
			}
			set
			{
				m_pageAggregatesOverReportItems = value;
			}
		}

		internal Report Report => m_reportDef;

		internal int SectionIndex => m_sectionIndex;

		public bool NeedsTotalPages
		{
			get
			{
				if (!NeedsOverallTotalPages)
				{
					return NeedsPageBreakTotalPages;
				}
				return true;
			}
		}

		public bool NeedsOverallTotalPages
		{
			get
			{
				if (IsOldSnapshot)
				{
					return m_renderReport.NeedsHeaderFooterEvaluation;
				}
				return m_sectionDef.NeedsOverallTotalPages;
			}
		}

		public bool NeedsPageBreakTotalPages
		{
			get
			{
				if (IsOldSnapshot)
				{
					return false;
				}
				return m_sectionDef.NeedsPageBreakTotalPages;
			}
		}

		public bool NeedsReportItemsOnPage
		{
			get
			{
				if (IsOldSnapshot)
				{
					return m_renderReport.NeedsHeaderFooterEvaluation;
				}
				return m_sectionDef.NeedsReportItemsOnPage;
			}
		}

		public string DefinitionPath => m_definitionPath;

		public IDefinitionPath ParentDefinitionPath => m_reportDef;

		IReportScopeInstance IReportScope.ReportScopeInstance => Instance;

		IRIFReportScope IReportScope.RIFReportScope => m_sectionDef;

		internal ReportSection(Report reportDef, Microsoft.ReportingServices.ReportRendering.Report renderReport, int indexInCollection)
			: this(reportDef, indexInCollection)
		{
			m_renderReport = renderReport;
		}

		internal ReportSection(Report reportDef, Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection sectionDef, int indexInCollection)
			: this(reportDef, indexInCollection)
		{
			m_sectionDef = sectionDef;
		}

		private ReportSection(Report reportDef, int indexInCollection)
		{
			m_reportDef = reportDef;
			m_sectionIndex = indexInCollection;
			m_definitionPath = DefinitionPathConstants.GetCollectionDefinitionPath(reportDef, indexInCollection);
		}

		internal void UpdateSubReportContents(Microsoft.ReportingServices.ReportRendering.Report newRenderSubreport)
		{
			m_renderReport = newRenderSubreport;
			if (m_body != null)
			{
				m_body.UpdateSubReportContents(m_renderReport);
			}
			if (m_page != null)
			{
				m_page.UpdateSubReportContents(m_renderReport);
			}
		}

		internal void SetNewContext()
		{
			if (m_body != null)
			{
				m_body.SetNewContext();
			}
			if (m_page != null)
			{
				m_page.SetNewContext();
			}
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}

		public void GetPageSections()
		{
			m_reportDef.PageEvaluation?.UpdatePageSections(this);
		}

		public void SetPage(int pageNumber, int totalPages, int overallPageNumber, int overallTotalPages)
		{
			m_reportDef.PageEvaluation?.Reset(this, pageNumber, totalPages, overallPageNumber, overallTotalPages);
		}

		public void SetPage(int pageNumber, int totalPages)
		{
			SetPage(pageNumber, totalPages, pageNumber, totalPages);
		}

		public void SetPageName(string pageName)
		{
			m_reportDef.PageEvaluation?.SetPageName(pageName);
		}
	}
}

using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class PageSection : ReportElement
	{
		private bool m_isHeader;

		private ReportSize m_height;

		private ReportItemCollection m_reportItems;

		private PageSectionInstance m_instance;

		internal const string PageHeaderUniqueNamePrefix = "ph";

		internal const string PageFooterUniqueNamePrefix = "pf";

		public override string ID
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderReportItem.ID;
				}
				return m_reportItemDef.RenderingModelID;
			}
		}

		public override string DefinitionPath => m_parentDefinitionPath.DefinitionPath + (m_isHeader ? "xH" : "xF");

		internal Page PageDefinition => (Page)m_parentDefinitionPath;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.PageSection RifPageSection => (Microsoft.ReportingServices.ReportIntermediateFormat.PageSection)m_reportItemDef;

		internal Microsoft.ReportingServices.ReportRendering.PageSection RenderPageSection => (Microsoft.ReportingServices.ReportRendering.PageSection)m_renderReportItem;

		internal bool IsHeader => m_isHeader;

		public ReportSize Height
		{
			get
			{
				if (m_height == null)
				{
					if (m_isOldSnapshot)
					{
						m_height = new ReportSize(RenderPageSection.Height);
					}
					else
					{
						m_height = new ReportSize(RifPageSection.Height);
					}
				}
				return m_height;
			}
		}

		public bool PrintOnFirstPage
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return RenderPageSection.PrintOnFirstPage;
				}
				return RifPageSection.PrintOnFirstPage;
			}
		}

		public bool PrintOnLastPage
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return RenderPageSection.PrintOnLastPage;
				}
				return RifPageSection.PrintOnLastPage;
			}
		}

		public ReportItemCollection ReportItemCollection
		{
			get
			{
				if (m_reportItems == null)
				{
					if (m_isOldSnapshot)
					{
						m_reportItems = new ReportItemCollection(this, inSubtotal: false, RenderPageSection.ReportItemCollection, m_renderingContext);
					}
					else
					{
						m_reportItems = new ReportItemCollection(ReportScope, this, RifPageSection.ReportItems, m_renderingContext);
					}
				}
				return m_reportItems;
			}
		}

		public bool PrintBetweenSections
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return false;
				}
				return RifPageSection.PrintBetweenSections;
			}
		}

		internal override string InstanceUniqueName
		{
			get
			{
				if (Instance != null)
				{
					return Instance.UniqueName;
				}
				return null;
			}
		}

		internal override ReportElementInstance ReportElementInstance => Instance;

		public new PageSectionInstance Instance
		{
			get
			{
				if (base.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new PageSectionInstance(this);
				}
				return m_instance;
			}
		}

		internal PageSection(IReportScope reportScope, IDefinitionPath parentDefinitionPath, bool isHeader, Microsoft.ReportingServices.ReportIntermediateFormat.PageSection pageSectionDef, RenderingContext renderingContext)
			: base(reportScope, parentDefinitionPath, pageSectionDef, renderingContext)
		{
			m_isHeader = isHeader;
		}

		internal PageSection(IDefinitionPath parentDefinitionPath, bool isHeader, Microsoft.ReportingServices.ReportRendering.PageSection renderPageSection, RenderingContext renderingContext)
			: base(parentDefinitionPath, renderPageSection, renderingContext)
		{
			m_isHeader = isHeader;
		}

		internal void UpdatePageSection(Microsoft.ReportingServices.ReportRendering.PageSection renderPageSection)
		{
			m_renderReportItem = renderPageSection;
			if (m_reportItems != null)
			{
				m_reportItems.UpdateRenderReportItem(renderPageSection.ReportItemCollection);
			}
		}

		internal override void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			base.SetNewContext();
		}

		internal override void SetNewContextChildren()
		{
			if (m_reportItems != null)
			{
				m_reportItems.SetNewContext();
			}
		}
	}
}

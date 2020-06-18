using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class Page : ReportElement, IReportScope
	{
		private PageInstance m_instance;

		private Microsoft.ReportingServices.ReportIntermediateFormat.Page m_pageDef;

		private Microsoft.ReportingServices.ReportRendering.Report m_renderReport;

		private ReportSection m_reportSection;

		private PageSection m_pageHeader;

		private PageSection m_pageFooter;

		public override string ID
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderReport.ReportDef.ID.ToString(CultureInfo.InvariantCulture) + "xP";
				}
				return m_pageDef.RenderingModelID;
			}
		}

		public override string DefinitionPath => ((base.ParentDefinitionPath.DefinitionPath != null) ? base.ParentDefinitionPath.DefinitionPath : "") + "xP";

		public PageSection PageHeader
		{
			get
			{
				if (m_pageHeader == null && !m_renderingContext.IsSubReportContext)
				{
					if (m_isOldSnapshot && m_renderReport.PageHeader != null)
					{
						m_pageHeader = new PageSection(this, isHeader: true, m_renderReport.PageHeader, m_reportSection.Report.HeaderFooterRenderingContext);
					}
					else if (!m_isOldSnapshot && m_pageDef.PageHeader != null)
					{
						m_pageHeader = new PageSection(this, this, isHeader: true, m_pageDef.PageHeader, m_reportSection.Report.HeaderFooterRenderingContext);
					}
				}
				return m_pageHeader;
			}
		}

		public PageSection PageFooter
		{
			get
			{
				if (m_pageFooter == null && !m_renderingContext.IsSubReportContext)
				{
					if (m_isOldSnapshot && m_renderReport.PageFooter != null)
					{
						m_pageFooter = new PageSection(this, isHeader: false, m_renderReport.PageFooter, m_reportSection.Report.HeaderFooterRenderingContext);
					}
					else if (!m_isOldSnapshot && m_pageDef.PageFooter != null)
					{
						m_pageFooter = new PageSection(this, this, isHeader: false, m_pageDef.PageFooter, m_reportSection.Report.HeaderFooterRenderingContext);
					}
				}
				return m_pageFooter;
			}
		}

		public ReportSize PageHeight
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return new ReportSize(m_renderReport.PageHeight);
				}
				if (ShouldUseFirstSection)
				{
					return FirstSectionPage.PageHeight;
				}
				if (m_pageDef.PageHeightForRendering == null)
				{
					m_pageDef.PageHeightForRendering = new ReportSize(m_pageDef.PageHeight, m_pageDef.PageHeightValue);
				}
				return m_pageDef.PageHeightForRendering;
			}
		}

		public ReportSize PageWidth
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return new ReportSize(m_renderReport.PageWidth);
				}
				if (ShouldUseFirstSection)
				{
					return FirstSectionPage.PageWidth;
				}
				if (m_pageDef.PageWidthForRendering == null)
				{
					m_pageDef.PageWidthForRendering = new ReportSize(m_pageDef.PageWidth, m_pageDef.PageWidthValue);
				}
				return m_pageDef.PageWidthForRendering;
			}
		}

		public ReportSize InteractiveHeight
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return new ReportSize(m_renderReport.ReportDef.InteractiveHeight, m_renderReport.ReportDef.InteractiveHeightValue);
				}
				if (ShouldUseFirstSection)
				{
					return FirstSectionPage.InteractiveHeight;
				}
				if (m_pageDef.InteractiveHeightForRendering == null)
				{
					m_pageDef.InteractiveHeightForRendering = new ReportSize(m_pageDef.InteractiveHeight, m_pageDef.InteractiveHeightValue);
				}
				return m_pageDef.InteractiveHeightForRendering;
			}
		}

		public ReportSize InteractiveWidth
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return new ReportSize(m_renderReport.ReportDef.InteractiveWidth, m_renderReport.ReportDef.InteractiveWidthValue);
				}
				if (ShouldUseFirstSection)
				{
					return FirstSectionPage.InteractiveWidth;
				}
				if (m_pageDef.InteractiveWidthForRendering == null)
				{
					m_pageDef.InteractiveWidthForRendering = new ReportSize(m_pageDef.InteractiveWidth, m_pageDef.InteractiveWidthValue);
				}
				return m_pageDef.InteractiveWidthForRendering;
			}
		}

		public ReportSize LeftMargin
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return new ReportSize(m_renderReport.LeftMargin);
				}
				if (ShouldUseFirstSection)
				{
					return FirstSectionPage.LeftMargin;
				}
				if (m_pageDef.LeftMarginForRendering == null)
				{
					m_pageDef.LeftMarginForRendering = new ReportSize(m_pageDef.LeftMargin, m_pageDef.LeftMarginValue);
				}
				return m_pageDef.LeftMarginForRendering;
			}
		}

		public ReportSize RightMargin
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return new ReportSize(m_renderReport.RightMargin);
				}
				if (ShouldUseFirstSection)
				{
					return FirstSectionPage.RightMargin;
				}
				if (m_pageDef.RightMarginForRendering == null)
				{
					m_pageDef.RightMarginForRendering = new ReportSize(m_pageDef.RightMargin, m_pageDef.RightMarginValue);
				}
				return m_pageDef.RightMarginForRendering;
			}
		}

		public ReportSize TopMargin
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return new ReportSize(m_renderReport.TopMargin);
				}
				if (ShouldUseFirstSection)
				{
					return FirstSectionPage.TopMargin;
				}
				if (m_pageDef.TopMarginForRendering == null)
				{
					m_pageDef.TopMarginForRendering = new ReportSize(m_pageDef.TopMargin, m_pageDef.TopMarginValue);
				}
				return m_pageDef.TopMarginForRendering;
			}
		}

		public ReportSize BottomMargin
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return new ReportSize(m_renderReport.BottomMargin);
				}
				if (ShouldUseFirstSection)
				{
					return FirstSectionPage.BottomMargin;
				}
				if (m_pageDef.BottomMarginForRendering == null)
				{
					m_pageDef.BottomMarginForRendering = new ReportSize(m_pageDef.BottomMargin, m_pageDef.BottomMarginValue);
				}
				return m_pageDef.BottomMarginForRendering;
			}
		}

		internal override bool UseRenderStyle => !m_renderReport.BodyHasBorderStyles;

		internal override IStyleContainer StyleContainer => m_pageDef;

		public int Columns
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderReport.Columns;
				}
				return m_pageDef.Columns;
			}
		}

		public ReportSize ColumnSpacing
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return new ReportSize(m_renderReport.ColumnSpacing);
				}
				if (m_pageDef.ColumnSpacingForRendering == null)
				{
					m_pageDef.ColumnSpacingForRendering = new ReportSize(m_pageDef.ColumnSpacing, m_pageDef.ColumnSpacingValue);
				}
				return m_pageDef.ColumnSpacingForRendering;
			}
		}

		public override Style Style
		{
			get
			{
				if (ShouldUseFirstSection)
				{
					return FirstSectionPage.Style;
				}
				return base.Style;
			}
		}

		internal Microsoft.ReportingServices.ReportRendering.Report RenderReport
		{
			get
			{
				if (!m_isOldSnapshot)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return m_renderReport;
			}
		}

		internal override Microsoft.ReportingServices.ReportRendering.ReportItem RenderReportItem
		{
			get
			{
				if (!m_isOldSnapshot)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return m_renderReport.Body;
			}
		}

		internal Page FirstSectionPage => m_reportSection.Report.FirstSection.Page;

		internal bool ShouldUseFirstSection => m_reportSection.SectionIndex > 0;

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

		public new PageInstance Instance
		{
			get
			{
				if (base.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new PageInstance(this);
				}
				return m_instance;
			}
		}

		internal override IReportScope ReportScope => this;

		IReportScopeInstance IReportScope.ReportScopeInstance => Instance;

		IRIFReportScope IReportScope.RIFReportScope => m_pageDef;

		internal Page(IDefinitionPath parentDefinitionPath, RenderingContext renderingContext, ReportSection reportSection)
			: base(null, parentDefinitionPath, reportSection.SectionDef, renderingContext)
		{
			m_isOldSnapshot = false;
			m_pageDef = reportSection.SectionDef.Page;
			m_reportSection = reportSection;
		}

		internal Page(IDefinitionPath parentDefinitionPath, Microsoft.ReportingServices.ReportRendering.Report renderReport, RenderingContext renderingContext, ReportSection reportSection)
			: base(parentDefinitionPath, renderingContext)
		{
			m_isOldSnapshot = true;
			m_renderReport = renderReport;
			m_reportSection = reportSection;
		}

		internal void UpdateWithCurrentPageSections(Microsoft.ReportingServices.ReportRendering.PageSection header, Microsoft.ReportingServices.ReportRendering.PageSection footer)
		{
			if (header != null)
			{
				PageHeader.UpdatePageSection(header);
			}
			if (footer != null)
			{
				PageFooter.UpdatePageSection(footer);
			}
		}

		internal void UpdateSubReportContents(Microsoft.ReportingServices.ReportRendering.Report newRenderSubreport)
		{
			m_renderReport = newRenderSubreport;
			UpdateWithCurrentPageSections(m_renderReport.PageHeader, m_renderReport.PageFooter);
		}

		internal override void SetNewContextChildren()
		{
			if (m_pageHeader != null)
			{
				m_pageHeader.SetNewContext();
			}
			if (m_pageFooter != null)
			{
				m_pageFooter.SetNewContext();
			}
		}
	}
}

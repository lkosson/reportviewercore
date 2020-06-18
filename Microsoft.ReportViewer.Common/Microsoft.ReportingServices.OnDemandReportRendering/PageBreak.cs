using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class PageBreak
	{
		private ReportBoolProperty m_resetPageNumber;

		private ReportBoolProperty m_disabled;

		private RenderingContext m_renderingContext;

		private IReportScope m_reportScope;

		private IPageBreakOwner m_pageBreakOwner;

		private Microsoft.ReportingServices.ReportIntermediateFormat.PageBreak m_pageBreakDef;

		private PageBreakLocation m_pageBreaklocation;

		private bool m_isOldSnapshotOrStaticMember;

		private PageBreakInstance m_pageBreakInstance;

		public PageBreakLocation BreakLocation
		{
			get
			{
				if (m_isOldSnapshotOrStaticMember)
				{
					return m_pageBreaklocation;
				}
				return m_pageBreakDef.BreakLocation;
			}
		}

		public ReportBoolProperty Disabled
		{
			get
			{
				if (m_disabled == null)
				{
					if (m_isOldSnapshotOrStaticMember)
					{
						m_disabled = new ReportBoolProperty();
					}
					else
					{
						m_disabled = new ReportBoolProperty(m_pageBreakDef.Disabled);
					}
				}
				return m_disabled;
			}
		}

		public ReportBoolProperty ResetPageNumber
		{
			get
			{
				if (m_resetPageNumber == null)
				{
					if (m_isOldSnapshotOrStaticMember)
					{
						m_resetPageNumber = new ReportBoolProperty();
					}
					else
					{
						m_resetPageNumber = new ReportBoolProperty(m_pageBreakDef.ResetPageNumber);
					}
				}
				return m_resetPageNumber;
			}
		}

		public PageBreakInstance Instance
		{
			get
			{
				if (m_renderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_pageBreakInstance == null)
				{
					m_pageBreakInstance = new PageBreakInstance(m_reportScope, this);
				}
				return m_pageBreakInstance;
			}
		}

		internal bool IsOldSnapshot => m_isOldSnapshotOrStaticMember;

		internal IReportScope ReportScope => m_reportScope;

		internal RenderingContext RenderingContext => m_renderingContext;

		internal IPageBreakOwner PageBreakOwner => m_pageBreakOwner;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.PageBreak PageBreakDef => m_pageBreakDef;

		internal bool HasEnabledInstance
		{
			get
			{
				PageBreakInstance instance = Instance;
				if (instance != null)
				{
					return !instance.Disabled;
				}
				return false;
			}
		}

		internal PageBreak(RenderingContext renderingContext, IReportScope reportScope, IPageBreakOwner pageBreakOwner)
		{
			m_renderingContext = renderingContext;
			m_reportScope = reportScope;
			m_pageBreakOwner = pageBreakOwner;
			m_pageBreakDef = m_pageBreakOwner.PageBreak;
			if (m_pageBreakDef == null)
			{
				m_pageBreakDef = new Microsoft.ReportingServices.ReportIntermediateFormat.PageBreak();
			}
			m_isOldSnapshotOrStaticMember = false;
		}

		internal PageBreak(RenderingContext renderingContext, IReportScope reportScope, PageBreakLocation pageBreaklocation)
		{
			m_renderingContext = renderingContext;
			m_reportScope = reportScope;
			m_pageBreaklocation = pageBreaklocation;
			m_isOldSnapshotOrStaticMember = true;
		}

		internal void SetNewContext()
		{
			if (m_pageBreakInstance != null)
			{
				m_pageBreakInstance.SetNewContext();
			}
		}
	}
}

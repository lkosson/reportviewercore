using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class Body : ReportElement
	{
		private Microsoft.ReportingServices.ReportRendering.Report m_renderReport;

		private ReportItemCollection m_reportItems;

		private BodyInstance m_instance;

		private bool m_subreportInSubtotal;

		internal override bool UseRenderStyle => m_renderReport.BodyHasBorderStyles;

		public override string ID
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderReport.Body.ID;
				}
				if (m_renderingContext.IsSubReportContext)
				{
					return SectionDef.RenderingModelID + "xS";
				}
				return SectionDef.RenderingModelID + "xB";
			}
		}

		public override string DefinitionPath => string.Concat(str1: (!m_renderingContext.IsSubReportContext) ? "xB" : "xS", str0: m_parentDefinitionPath.DefinitionPath);

		public ReportItemCollection ReportItemCollection
		{
			get
			{
				if (m_reportItems == null)
				{
					if (m_isOldSnapshot)
					{
						m_reportItems = new ReportItemCollection(m_parentDefinitionPath, m_subreportInSubtotal, m_renderReport.Body.ReportItemCollection, m_renderingContext);
					}
					else
					{
						m_reportItems = new ReportItemCollection(ReportScope, m_parentDefinitionPath, SectionDef.ReportItems, m_renderingContext);
					}
				}
				return m_reportItems;
			}
		}

		public ReportSize Height
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return new ReportSize(m_renderReport.Body.Height);
				}
				Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection sectionDef = SectionDef;
				if (sectionDef.HeightForRendering == null)
				{
					sectionDef.HeightForRendering = new ReportSize(sectionDef.Height, sectionDef.HeightValue);
				}
				return sectionDef.HeightForRendering;
			}
		}

		internal override Microsoft.ReportingServices.ReportRendering.ReportItem RenderReportItem => m_renderReport.Body;

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

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection SectionDef => (Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection)m_reportItemDef;

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

		public new BodyInstance Instance
		{
			get
			{
				if (base.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new BodyInstance(this);
				}
				return m_instance;
			}
		}

		internal Body(IReportScope reportScope, IDefinitionPath parentDefinitionPath, Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection sectionDef, RenderingContext renderingContext)
			: base(reportScope, parentDefinitionPath, sectionDef, renderingContext)
		{
		}

		internal Body(IDefinitionPath parentDefinitionPath, bool subreportInSubtotal, Microsoft.ReportingServices.ReportRendering.Report renderReport, RenderingContext renderingContext)
			: base(parentDefinitionPath, renderingContext)
		{
			m_isOldSnapshot = true;
			m_subreportInSubtotal = subreportInSubtotal;
			m_renderReport = renderReport;
			m_renderingContext = renderingContext;
		}

		internal void UpdateSubReportContents(Microsoft.ReportingServices.ReportRendering.Report newRenderSubreport)
		{
			m_renderReport = newRenderSubreport;
			if (m_reportItems != null)
			{
				m_reportItems.UpdateRenderReportItem(m_renderReport.Body.ReportItemCollection);
			}
		}

		internal override void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (SectionDef != null)
			{
				SectionDef.ResetTextBoxImpls(m_renderingContext.OdpContext);
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

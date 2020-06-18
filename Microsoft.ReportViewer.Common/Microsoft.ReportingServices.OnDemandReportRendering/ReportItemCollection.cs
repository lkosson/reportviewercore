using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportItemCollection : ReportElementCollectionBase<ReportItem>
	{
		private IDefinitionPath m_parentDefinitionPath;

		private bool m_isOldSnapshot;

		private bool m_inSubtotal;

		private ReportItem[] m_reportItems;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ReportItemCollection m_reportItemColDef;

		private Microsoft.ReportingServices.ReportRendering.ReportItemCollection m_renderReportItemCollection;

		private RenderingContext m_renderingContext;

		private IReportScope m_reportScope;

		public override ReportItem this[int index] => GetItem(index).ExposeAs(m_renderingContext);

		public override int Count
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (m_renderReportItemCollection == null)
					{
						return 0;
					}
					return m_renderReportItemCollection.Count;
				}
				if (m_reportItemColDef.ROMIndexMap != null)
				{
					return m_reportItemColDef.ROMIndexMap.Count;
				}
				return m_reportItemColDef.Count;
			}
		}

		internal bool IsOldSnapshot => m_isOldSnapshot;

		internal Microsoft.ReportingServices.ReportRendering.ReportItemCollection RenderReportItemCollection
		{
			get
			{
				if (!m_isOldSnapshot)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return m_renderReportItemCollection;
			}
		}

		internal ReportItemCollection(IReportScope reportScope, IDefinitionPath parentDefinitionPath, Microsoft.ReportingServices.ReportIntermediateFormat.ReportItemCollection reportItemColDef, RenderingContext renderingContext)
		{
			m_reportScope = reportScope;
			m_parentDefinitionPath = parentDefinitionPath;
			m_isOldSnapshot = false;
			m_reportItemColDef = reportItemColDef;
			m_renderingContext = renderingContext;
		}

		internal ReportItemCollection(IDefinitionPath parentDefinitionPath, bool inSubtotal, Microsoft.ReportingServices.ReportRendering.ReportItemCollection renderReportItemCollection, RenderingContext renderingContext)
		{
			m_parentDefinitionPath = parentDefinitionPath;
			m_isOldSnapshot = true;
			m_inSubtotal = inSubtotal;
			m_renderReportItemCollection = renderReportItemCollection;
			m_renderingContext = renderingContext;
		}

		internal void UpdateRenderReportItem(Microsoft.ReportingServices.ReportRendering.ReportItemCollection renderReportItemCollection)
		{
			if (!m_isOldSnapshot)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			if (renderReportItemCollection != null)
			{
				m_renderReportItemCollection = renderReportItemCollection;
			}
			if (m_reportItems == null)
			{
				return;
			}
			for (int i = 0; i < m_reportItems.Length; i++)
			{
				if (m_reportItems[i] != null)
				{
					m_reportItems[i].UpdateRenderReportItem(renderReportItemCollection[i]);
				}
			}
		}

		internal void SetNewContext()
		{
			for (int i = 0; i < Count; i++)
			{
				GetItem(i).SetNewContext();
			}
		}

		private ReportItem GetItem(int index)
		{
			if (0 > index || index >= Count)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
			}
			ReportItem reportItem = null;
			if (m_reportItems == null)
			{
				m_reportItems = new ReportItem[Count];
			}
			reportItem = m_reportItems[index];
			if (reportItem == null)
			{
				if (m_isOldSnapshot)
				{
					reportItem = (m_reportItems[index] = ReportItem.CreateShim(m_parentDefinitionPath, index, m_inSubtotal, m_renderReportItemCollection[index], m_renderingContext));
				}
				else
				{
					int num = (m_reportItemColDef.ROMIndexMap == null) ? index : m_reportItemColDef.ROMIndexMap[index];
					reportItem = (m_reportItems[index] = ReportItem.CreateItem(m_reportScope, m_parentDefinitionPath, num, m_reportItemColDef[num], m_renderingContext));
				}
			}
			return reportItem;
		}
	}
}

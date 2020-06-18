using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class CellContents
	{
		private IDefinitionPath m_ownerPath;

		private bool m_isOldSnapshot;

		private bool m_inSubtotal;

		private RenderingContext m_renderingContext;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem m_cellReportItem;

		private Microsoft.ReportingServices.ReportRendering.ReportItem m_renderReportItem;

		private ReportItem m_reportItem;

		private int m_colSpan = 1;

		private int m_rowSpan = 1;

		private IReportScope m_reportScope;

		private double m_sizeDelta;

		private bool m_isColumn;

		public ReportItem ReportItem
		{
			get
			{
				if (m_reportItem == null)
				{
					if (m_isOldSnapshot)
					{
						m_reportItem = ReportItem.CreateShim(m_ownerPath, 0, m_inSubtotal, m_renderReportItem, m_renderingContext);
						if (m_sizeDelta > 0.0)
						{
							if (m_isColumn)
							{
								m_reportItem.SetCachedWidth(m_sizeDelta);
							}
							else
							{
								m_reportItem.SetCachedHeight(m_sizeDelta);
							}
						}
					}
					else if (m_cellReportItem != null)
					{
						m_reportItem = ReportItem.CreateItem(m_reportScope, m_ownerPath, 0, m_cellReportItem, m_renderingContext);
					}
				}
				if (m_reportItem != null)
				{
					return m_reportItem.ExposeAs(m_renderingContext);
				}
				return null;
			}
		}

		public int ColSpan => m_colSpan;

		public int RowSpan => m_rowSpan;

		internal CellContents(IReportScope reportScope, IDefinitionPath ownerPath, Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem cellReportItem, int rowSpan, int colSpan, RenderingContext renderingContext)
		{
			m_reportScope = reportScope;
			m_rowSpan = rowSpan;
			m_colSpan = colSpan;
			m_ownerPath = ownerPath;
			m_isOldSnapshot = false;
			m_cellReportItem = cellReportItem;
			m_renderingContext = renderingContext;
		}

		internal CellContents(IDefinitionPath ownerPath, bool inSubtotal, Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem, int rowSpan, int colSpan, RenderingContext renderingContext)
		{
			m_rowSpan = rowSpan;
			m_colSpan = colSpan;
			m_ownerPath = ownerPath;
			m_isOldSnapshot = true;
			m_inSubtotal = inSubtotal;
			m_renderReportItem = renderReportItem;
			m_renderingContext = renderingContext;
		}

		internal CellContents(IDefinitionPath ownerPath, bool inSubtotal, Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem, int rowSpan, int colSpan, RenderingContext renderingContext, double sizeDelta, bool isColumn)
		{
			m_rowSpan = rowSpan;
			m_colSpan = colSpan;
			m_ownerPath = ownerPath;
			m_isOldSnapshot = true;
			m_inSubtotal = inSubtotal;
			m_renderReportItem = renderReportItem;
			m_renderingContext = renderingContext;
			m_sizeDelta = sizeDelta;
			m_isColumn = isColumn;
		}

		internal CellContents(Rectangle rectangle, int rowSpan, int colSpan, RenderingContext renderingContext)
		{
			m_rowSpan = rowSpan;
			m_colSpan = colSpan;
			m_ownerPath = rectangle;
			m_reportItem = rectangle;
			m_renderingContext = renderingContext;
			m_isOldSnapshot = true;
		}

		internal void SetNewContext()
		{
			if (m_reportItem != null)
			{
				m_reportItem.SetNewContext();
			}
		}

		internal void UpdateRenderReportItem(Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			if (renderReportItem != null)
			{
				m_renderReportItem = renderReportItem;
			}
			if (m_reportItem != null)
			{
				m_reportItem.UpdateRenderReportItem(m_renderReportItem);
			}
		}
	}
}

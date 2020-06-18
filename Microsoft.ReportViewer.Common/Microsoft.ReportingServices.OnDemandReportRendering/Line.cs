using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class Line : ReportItem
	{
		public bool Slant
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return ((Microsoft.ReportingServices.ReportRendering.Line)m_renderReportItem).Slant;
				}
				return ((Microsoft.ReportingServices.ReportIntermediateFormat.Line)m_reportItemDef).LineSlant;
			}
		}

		internal Line(IReportScope reportScope, IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, Microsoft.ReportingServices.ReportIntermediateFormat.Line reportItemDef, RenderingContext renderingContext)
			: base(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
		}

		internal Line(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, Microsoft.ReportingServices.ReportRendering.Line renderLine, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, renderLine, renderingContext)
		{
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (m_instance == null)
			{
				m_instance = new LineInstance(this);
			}
			return m_instance;
		}
	}
}

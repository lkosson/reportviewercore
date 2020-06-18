using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalChartMemberCollection : ChartMemberCollection
	{
		private ChartMember m_parent;

		private ChartMemberList m_memberDefs;

		public override ChartMember this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				if (m_children == null)
				{
					m_children = new DataRegionMember[Count];
				}
				ChartMember chartMember = (ChartMember)m_children[index];
				if (chartMember == null)
				{
					IReportScope reportScope = (m_parent != null) ? m_parent.ReportScope : m_owner.ReportScope;
					chartMember = (ChartMember)(m_children[index] = new InternalChartMember(reportScope, this, base.OwnerChart, m_parent, m_memberDefs[index], index));
				}
				return chartMember;
			}
		}

		public override int Count => m_memberDefs.OriginalNodeCount;

		internal InternalChartMemberCollection(IDefinitionPath parentDefinitionPath, Chart owner, ChartMember parent, ChartMemberList memberDefs)
			: base(parentDefinitionPath, owner)
		{
			m_parent = parent;
			m_memberDefs = memberDefs;
		}
	}
}

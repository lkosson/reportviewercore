using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalTablixMemberCollection : TablixMemberCollection
	{
		private TablixMember m_parent;

		private TablixMemberList m_memberDefs;

		public override TablixMember this[int index]
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
				TablixMember tablixMember = (TablixMember)m_children[index];
				if (tablixMember == null)
				{
					IReportScope reportScope = (m_parent != null) ? m_parent.ReportScope : m_owner.ReportScope;
					tablixMember = (TablixMember)(m_children[index] = new InternalTablixMember(reportScope, this, base.OwnerTablix, m_parent, m_memberDefs[index], index));
				}
				return tablixMember;
			}
		}

		public override int Count => m_memberDefs.OriginalNodeCount;

		internal TablixMemberList MemberDefs => m_memberDefs;

		internal InternalTablixMemberCollection(IDefinitionPath parentDefinitionPath, Tablix owner, TablixMember parent, TablixMemberList memberDefs)
			: base(parentDefinitionPath, owner)
		{
			m_parent = parent;
			m_memberDefs = memberDefs;
		}
	}
}

using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalDataMemberCollection : DataMemberCollection
	{
		private DataMember m_parent;

		private DataMemberList m_memberDefs;

		public override DataMember this[int index]
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
				DataMember dataMember = (DataMember)m_children[index];
				if (dataMember == null)
				{
					IReportScope reportScope = (m_parent != null) ? m_parent.ReportScope : m_owner.ReportScope;
					dataMember = (DataMember)(m_children[index] = new InternalDataMember(reportScope, this, base.OwnerCri, m_parent, m_memberDefs[index], index));
				}
				return dataMember;
			}
		}

		public override int Count => m_memberDefs.OriginalNodeCount;

		internal InternalDataMemberCollection(IDefinitionPath parentDefinitionPath, CustomReportItem owner, DataMember parent, DataMemberList memberDefs)
			: base(parentDefinitionPath, owner)
		{
			m_parent = parent;
			m_memberDefs = memberDefs;
		}
	}
}

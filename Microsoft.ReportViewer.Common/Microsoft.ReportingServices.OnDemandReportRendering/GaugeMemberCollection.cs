using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeMemberCollection : DataRegionMemberCollection<GaugeMember>
	{
		private GaugeMember m_parent;

		private GaugeMemberList m_memberDefs;

		public override string DefinitionPath
		{
			get
			{
				if (m_parentDefinitionPath is GaugeMember)
				{
					return m_parentDefinitionPath.DefinitionPath + "xM";
				}
				return m_parentDefinitionPath.DefinitionPath;
			}
		}

		internal GaugePanel OwnerGaugePanel => m_owner as GaugePanel;

		public override GaugeMember this[int index]
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
				GaugeMember gaugeMember = (GaugeMember)m_children[index];
				if (gaugeMember == null)
				{
					IReportScope reportScope = (m_parent != null) ? m_parent.ReportScope : m_owner.ReportScope;
					gaugeMember = (GaugeMember)(m_children[index] = new GaugeMember(reportScope, this, OwnerGaugePanel, m_parent, m_memberDefs[index]));
				}
				return gaugeMember;
			}
		}

		public override int Count => 1;

		internal GaugeMemberCollection(IDefinitionPath parentDefinitionPath, GaugePanel owner)
			: base(parentDefinitionPath, (ReportItem)owner)
		{
		}

		internal GaugeMemberCollection(IDefinitionPath parentDefinitionPath, GaugePanel owner, GaugeMember parent, GaugeMemberList memberDefs)
			: base(parentDefinitionPath, (ReportItem)owner)
		{
			m_parent = parent;
			m_memberDefs = memberDefs;
		}
	}
}

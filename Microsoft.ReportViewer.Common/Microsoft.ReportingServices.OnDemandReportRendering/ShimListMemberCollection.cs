using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimListMemberCollection : ShimMemberCollection
	{
		private ShimRenderGroups m_renderGroups;

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
					tablixMember = (TablixMember)(m_children[index] = new ShimListMember(this, base.OwnerTablix, m_renderGroups, index, m_isColumnGroup));
				}
				return tablixMember;
			}
		}

		public override int Count => 1;

		internal ShimListMemberCollection(IDefinitionPath parentDefinitionPath, Tablix owner)
			: base(parentDefinitionPath, owner, isColumnGroup: true)
		{
		}

		internal ShimListMemberCollection(IDefinitionPath parentDefinitionPath, Tablix owner, ListContentCollection renderListContents)
			: base(parentDefinitionPath, owner, isColumnGroup: false)
		{
			m_renderGroups = new ShimRenderGroups(renderListContents);
		}

		internal void UpdateContext(ListContentCollection renderListContents)
		{
			m_renderGroups = new ShimRenderGroups(renderListContents);
			if (m_children != null)
			{
				((ShimListMember)m_children[0]).ResetContext(m_renderGroups);
			}
		}
	}
}

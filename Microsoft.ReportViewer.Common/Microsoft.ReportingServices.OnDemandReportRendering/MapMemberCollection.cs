using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapMemberCollection : DataRegionMemberCollection<MapMember>
	{
		private MapMember m_parent;

		private MapMemberList m_memberDefs;

		public override string DefinitionPath
		{
			get
			{
				if (m_parentDefinitionPath is MapMember)
				{
					return m_parentDefinitionPath.DefinitionPath + "xM";
				}
				return m_parentDefinitionPath.DefinitionPath;
			}
		}

		internal MapDataRegion OwnerMapDataRegion => m_owner as MapDataRegion;

		public override MapMember this[int index]
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
				MapMember mapMember = (MapMember)m_children[index];
				if (mapMember == null)
				{
					IReportScope reportScope = (m_parent != null) ? m_parent.ReportScope : m_owner.ReportScope;
					mapMember = (MapMember)(m_children[index] = new MapMember(reportScope, this, OwnerMapDataRegion, m_parent, m_memberDefs[index]));
				}
				return mapMember;
			}
		}

		public override int Count => 1;

		internal MapMemberCollection(IDefinitionPath parentDefinitionPath, MapDataRegion owner)
			: base(parentDefinitionPath, (ReportItem)owner)
		{
		}

		internal MapMemberCollection(IDefinitionPath parentDefinitionPath, MapDataRegion owner, MapMember parent, MapMemberList memberDefs)
			: base(parentDefinitionPath, (ReportItem)owner)
		{
			m_parent = parent;
			m_memberDefs = memberDefs;
		}
	}
}

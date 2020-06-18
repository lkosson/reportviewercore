using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class DataHierarchy : MemberHierarchy<DataMember>
	{
		private CustomReportItem OwnerCri => m_owner as CustomReportItem;

		public DataMemberCollection MemberCollection
		{
			get
			{
				if (m_members == null)
				{
					if (OwnerCri.IsOldSnapshot)
					{
						OwnerCri.ResetMemberCellDefinitionIndex(0);
						DataGroupingCollection definitionGroups = m_isColumn ? OwnerCri.RenderCri.CustomData.DataColumnGroupings : OwnerCri.RenderCri.CustomData.DataRowGroupings;
						m_members = new ShimDataMemberCollection(this, OwnerCri, m_isColumn, null, definitionGroups);
					}
					else
					{
						DataMemberList memberDefs = m_isColumn ? OwnerCri.CriDef.DataColumnMembers : OwnerCri.CriDef.DataRowMembers;
						m_members = new InternalDataMemberCollection(this, OwnerCri, null, memberDefs);
					}
				}
				return (DataMemberCollection)m_members;
			}
		}

		internal DataHierarchy(CustomReportItem owner, bool isColumn)
			: base((ReportItem)owner, isColumn)
		{
		}

		internal override void ResetContext()
		{
			if (m_members != null)
			{
				((ShimDataMemberCollection)m_members).UpdateContext();
			}
		}
	}
}

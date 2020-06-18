namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class MapMemberInstance : BaseInstance
	{
		protected MapDataRegion m_owner;

		protected MapMember m_memberDef;

		internal MapMemberInstance(MapDataRegion owner, MapMember memberDef)
			: base(memberDef.ReportScope)
		{
			m_owner = owner;
			m_memberDef = memberDef;
		}

		protected override void ResetInstanceCache()
		{
		}
	}
}

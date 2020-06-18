namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class DataMemberInstance : BaseInstance
	{
		protected CustomReportItem m_owner;

		protected DataMember m_memberDef;

		internal DataMemberInstance(CustomReportItem owner, DataMember memberDef)
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

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class GaugeMemberInstance : BaseInstance
	{
		protected GaugePanel m_owner;

		protected GaugeMember m_memberDef;

		internal GaugeMemberInstance(GaugePanel owner, GaugeMember memberDef)
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

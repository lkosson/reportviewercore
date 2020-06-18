namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal class MemberReference
	{
		private MemberName m_memberName;

		private int m_refID;

		internal MemberName MemberName => m_memberName;

		internal int RefID => m_refID;

		internal MemberReference(MemberName memberName, int refID)
		{
			m_memberName = memberName;
			m_refID = refID;
		}
	}
}

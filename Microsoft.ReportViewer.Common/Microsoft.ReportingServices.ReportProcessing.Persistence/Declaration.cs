namespace Microsoft.ReportingServices.ReportProcessing.Persistence
{
	internal sealed class Declaration
	{
		private ObjectType m_baseType;

		private MemberInfoList m_members;

		internal ObjectType BaseType => m_baseType;

		internal MemberInfoList Members => m_members;

		internal Declaration(ObjectType baseType, MemberInfoList members)
		{
			m_baseType = baseType;
			Global.Tracer.Assert(members != null);
			m_members = members;
		}
	}
}

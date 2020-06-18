namespace Microsoft.ReportingServices.ReportProcessing.Persistence
{
	internal sealed class MemberInfo
	{
		private MemberName m_memberName;

		private Token m_token;

		private ObjectType m_objectType;

		internal MemberName MemberName
		{
			get
			{
				return m_memberName;
			}
			set
			{
				m_memberName = value;
			}
		}

		internal Token Token => m_token;

		internal ObjectType ObjectType => m_objectType;

		internal MemberInfo(MemberName memberName, Token token)
		{
			m_memberName = memberName;
			m_token = token;
			m_objectType = ObjectType.None;
		}

		internal MemberInfo(MemberName memberName, ObjectType objectType)
		{
			m_memberName = memberName;
			m_token = Token.Object;
			m_objectType = objectType;
		}

		internal MemberInfo(MemberName memberName, Token token, ObjectType objectType)
		{
			m_memberName = memberName;
			m_token = token;
			m_objectType = objectType;
		}

		internal static bool Equals(MemberInfo a, MemberInfo b)
		{
			if (a == null || b == null)
			{
				return false;
			}
			if (a.MemberName == b.MemberName && a.Token == b.Token)
			{
				return a.ObjectType == b.ObjectType;
			}
			return false;
		}
	}
}

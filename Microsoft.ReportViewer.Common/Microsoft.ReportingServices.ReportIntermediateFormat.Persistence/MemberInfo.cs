namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal class MemberInfo
	{
		private MemberName m_name;

		private Token m_token = Token.Object;

		private ObjectType m_type = ObjectType.None;

		private ObjectType m_containedType = ObjectType.None;

		private Lifetime m_lifetime = Lifetime.Unspecified;

		internal MemberName MemberName => m_name;

		internal Token Token => m_token;

		internal ObjectType ObjectType => m_type;

		internal ObjectType ContainedType => m_containedType;

		internal Lifetime Lifetime => m_lifetime;

		internal MemberInfo(MemberName name, Token token)
		{
			m_name = name;
			m_token = token;
		}

		internal MemberInfo(MemberName name, Token token, Lifetime lifetime)
		{
			m_name = name;
			m_token = token;
			m_lifetime = lifetime;
		}

		internal MemberInfo(MemberName name, ObjectType type)
		{
			m_name = name;
			m_type = type;
		}

		internal MemberInfo(MemberName name, ObjectType type, Lifetime lifetime)
		{
			m_name = name;
			m_type = type;
			m_lifetime = lifetime;
		}

		internal MemberInfo(MemberName name, ObjectType type, ObjectType containedType)
		{
			m_name = name;
			m_type = type;
			m_containedType = containedType;
		}

		internal MemberInfo(MemberName name, ObjectType type, ObjectType containedType, Lifetime lifetime)
		{
			m_name = name;
			m_type = type;
			m_containedType = containedType;
			m_lifetime = lifetime;
		}

		internal MemberInfo(MemberName name, ObjectType type, Token token)
		{
			m_name = name;
			m_token = token;
			m_type = type;
		}

		internal MemberInfo(MemberName name, ObjectType type, Token token, Lifetime lifetime)
		{
			m_name = name;
			m_token = token;
			m_type = type;
			m_lifetime = lifetime;
		}

		internal MemberInfo(MemberName name, ObjectType type, Token token, ObjectType containedType)
		{
			m_name = name;
			m_token = token;
			m_type = type;
			m_containedType = containedType;
		}

		internal MemberInfo(MemberName name, ObjectType type, Token token, ObjectType containedType, Lifetime lifetime)
		{
			m_name = name;
			m_token = token;
			m_type = type;
			m_containedType = containedType;
			m_lifetime = lifetime;
		}

		internal virtual bool IsWrittenForCompatVersion(int compatVersion)
		{
			return m_lifetime.IncludesVersion(compatVersion);
		}

		public override int GetHashCode()
		{
			return (int)m_name ^ (int)((uint)m_token << 8) ^ ((int)m_type << 16) ^ ((int)m_containedType << 24);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is MemberInfo))
			{
				return false;
			}
			return Equals((MemberInfo)obj);
		}

		internal bool Equals(MemberInfo otherMember)
		{
			if (otherMember != null && m_name == otherMember.m_name && m_token == otherMember.m_token && m_type == otherMember.m_type && m_containedType == otherMember.m_containedType)
			{
				return true;
			}
			return false;
		}
	}
}

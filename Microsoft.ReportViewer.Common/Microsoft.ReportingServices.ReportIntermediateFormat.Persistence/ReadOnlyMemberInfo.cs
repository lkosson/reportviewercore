namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal class ReadOnlyMemberInfo : MemberInfo
	{
		internal ReadOnlyMemberInfo(MemberName name, Token token)
			: base(name, token)
		{
		}

		internal ReadOnlyMemberInfo(MemberName name, ObjectType type)
			: base(name, type)
		{
		}

		internal ReadOnlyMemberInfo(MemberName name, ObjectType type, ObjectType containedType)
			: base(name, type, containedType)
		{
		}

		internal ReadOnlyMemberInfo(MemberName name, ObjectType type, Token token)
			: base(name, type, token)
		{
		}

		internal ReadOnlyMemberInfo(MemberName name, ObjectType type, Token token, ObjectType containedType)
			: base(name, type, token, containedType)
		{
		}

		internal override bool IsWrittenForCompatVersion(int compatVersion)
		{
			return false;
		}
	}
}

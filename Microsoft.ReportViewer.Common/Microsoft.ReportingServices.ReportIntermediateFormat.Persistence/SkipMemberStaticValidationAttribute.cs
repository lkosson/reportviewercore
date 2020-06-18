using System;

namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	internal sealed class SkipMemberStaticValidationAttribute : Attribute
	{
		private MemberName m_member;

		public MemberName Member => m_member;

		internal SkipMemberStaticValidationAttribute(MemberName member)
		{
			m_member = member;
		}
	}
}

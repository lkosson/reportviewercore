using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing.Persistence
{
	internal sealed class MemberInfoList : ArrayList
	{
		internal new MemberInfo this[int index] => (MemberInfo)base[index];

		internal MemberInfoList()
		{
		}

		internal MemberInfoList(int capacity)
			: base(capacity)
		{
		}
	}
}

using System;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class TablixMemberList : HierarchyNodeList
	{
		internal new TablixMember this[int index] => (TablixMember)base[index];

		public TablixMemberList()
		{
		}

		internal TablixMemberList(int capacity)
			: base(capacity)
		{
		}
	}
}

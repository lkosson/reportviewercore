using System;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class DataMemberList : HierarchyNodeList
	{
		internal new DataMember this[int index] => (DataMember)base[index];

		public DataMemberList()
		{
		}

		internal DataMemberList(int capacity)
			: base(capacity)
		{
		}
	}
}

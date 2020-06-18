using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class GroupingList : ArrayList
	{
		internal new Grouping this[int index] => (Grouping)base[index];

		internal Grouping LastEntry
		{
			get
			{
				if (Count == 0)
				{
					return null;
				}
				return this[Count - 1];
			}
		}

		internal GroupingList()
		{
		}

		internal GroupingList(int capacity)
			: base(capacity)
		{
		}

		internal new GroupingList Clone()
		{
			int count = Count;
			GroupingList groupingList = new GroupingList(count);
			for (int i = 0; i < count; i++)
			{
				groupingList.Add(this[i]);
			}
			return groupingList;
		}
	}
}

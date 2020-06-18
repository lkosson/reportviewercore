using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class SortedReportItemIndexList : ArrayList
	{
		internal new int this[int index] => (int)base[index];

		internal SortedReportItemIndexList()
		{
		}

		internal SortedReportItemIndexList(int capacity)
			: base(capacity)
		{
		}

		public void Add(ReportItemList collection, int collectionIndex, bool sortVertically)
		{
			Global.Tracer.Assert(collection != null);
			ReportItem reportItem = collection[collectionIndex];
			int num = 0;
			while (num < base.Count)
			{
				if (sortVertically && reportItem.AbsoluteTopValue > collection[this[num]].AbsoluteTopValue)
				{
					num++;
					continue;
				}
				if (sortVertically || !(reportItem.AbsoluteLeftValue > collection[this[num]].AbsoluteLeftValue))
				{
					break;
				}
				num++;
			}
			base.Insert(num, collectionIndex);
		}
	}
}

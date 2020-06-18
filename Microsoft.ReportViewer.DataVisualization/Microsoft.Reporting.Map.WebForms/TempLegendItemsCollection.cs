using System.Collections;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class TempLegendItemsCollection : CollectionBase
	{
		public LegendItem this[int index]
		{
			get
			{
				return (LegendItem)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		public int Add(LegendItem item)
		{
			return base.List.Add(item);
		}
	}
}

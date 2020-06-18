using System.Collections;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class ZOrderSort : IComparer
	{
		private ArrayList collection;

		public ZOrderSort(ArrayList collection)
		{
			this.collection = collection;
		}

		int IComparer.Compare(object x, object y)
		{
			int result = 0;
			if (x is IZOrderedObject && y is IZOrderedObject)
			{
				int num = ((IZOrderedObject)x).GetZOrder();
				int num2 = ((IZOrderedObject)y).GetZOrder();
				if (num == num2)
				{
					num = collection.IndexOf(x);
					num2 = collection.IndexOf(y);
				}
				if (num > num2)
				{
					result = 1;
				}
				else if (num < num2)
				{
					result = -1;
				}
			}
			return result;
		}
	}
}

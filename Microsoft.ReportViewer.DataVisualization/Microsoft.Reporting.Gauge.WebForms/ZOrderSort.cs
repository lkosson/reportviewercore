using System.Collections;

namespace Microsoft.Reporting.Gauge.WebForms
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
			if (x is IRenderable && y is IRenderable)
			{
				int num = ((IRenderable)x).GetZOrder();
				int num2 = ((IRenderable)y).GetZOrder();
				if (num == 0)
				{
					num = collection.IndexOf(x);
				}
				else if (num > 0)
				{
					num += collection.Count;
				}
				if (num2 == 0)
				{
					num2 = collection.IndexOf(y);
				}
				else if (num2 > 0)
				{
					num2 += collection.Count;
				}
				result = num - num2;
			}
			return result;
		}
	}
}

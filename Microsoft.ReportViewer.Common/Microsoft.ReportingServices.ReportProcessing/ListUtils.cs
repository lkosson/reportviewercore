using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal class ListUtils
	{
		public static void AdjustLength<T>(List<T> instances, int indexInCollection) where T : class
		{
			for (int i = instances.Count; i <= indexInCollection; i++)
			{
				instances.Add(null);
			}
		}

		public static bool ContainsWithOrdinalComparer(string item, List<string> list)
		{
			if (item == null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] == null)
					{
						return true;
					}
				}
				return false;
			}
			for (int j = 0; j < list.Count; j++)
			{
				if (string.CompareOrdinal(list[j], item) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public static bool AreSameOrSuffix<T>(List<T> list1, List<T> list2, IComparer<T> comparer)
		{
			if (list1 == null || list2 == null)
			{
				return list1 == list2;
			}
			int num = Math.Min(list1.Count, list2.Count);
			int num2 = list1.Count - num;
			int num3 = list2.Count - num;
			for (int i = 0; i < num; i++)
			{
				T x = list1[num2 + i];
				T y = list2[num3 + i];
				if (comparer.Compare(x, y) != 0)
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsSubSequenceAllowExtras<T>(List<T> super, List<T> subCandidate, IComparer<T> comparer)
		{
			if (super == null || subCandidate == null)
			{
				return super == subCandidate;
			}
			if (subCandidate.Count > super.Count)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < super.Count; i++)
			{
				if (num >= subCandidate.Count)
				{
					break;
				}
				T x = super[i];
				T y = subCandidate[num];
				if (comparer.Compare(x, y) == 0)
				{
					num++;
				}
			}
			return num == subCandidate.Count;
		}

		public static bool Contains<T>(List<T> list, T item, IComparer<T> comparer)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (comparer.Compare(list[i], item) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsSubset<T>(List<T> super, List<T> subCandidate, IComparer<T> comparer)
		{
			for (int i = 0; i < subCandidate.Count; i++)
			{
				if (!Contains(super, subCandidate[i], comparer))
				{
					return false;
				}
			}
			return true;
		}
	}
}

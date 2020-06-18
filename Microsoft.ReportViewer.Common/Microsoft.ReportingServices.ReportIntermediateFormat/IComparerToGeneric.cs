using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal class IComparerToGeneric<T> : IComparer<T>
	{
		private IComparer m_comparer;

		internal IComparerToGeneric(IComparer comparer)
		{
			m_comparer = comparer;
		}

		public int Compare(T x, T y)
		{
			return m_comparer.Compare(x, y);
		}
	}
}

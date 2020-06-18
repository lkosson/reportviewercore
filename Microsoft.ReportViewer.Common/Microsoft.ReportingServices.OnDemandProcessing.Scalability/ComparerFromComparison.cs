using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class ComparerFromComparison<T> : IComparer
	{
		private Comparison<T> m_comparison;

		internal ComparerFromComparison(Comparison<T> comparison)
		{
			m_comparison = comparison;
		}

		public int Compare(object x, object y)
		{
			if (!(x is T) || !(y is T))
			{
				Global.Tracer.Assert(condition: false, "Cannot compare other types than the comparison's types");
			}
			return m_comparison((T)x, (T)y);
		}
	}
}

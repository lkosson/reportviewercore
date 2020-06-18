using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Common
{
	internal interface IDataComparer : IEqualityComparer, IEqualityComparer<object>, IComparer, IComparer<object>
	{
		int Compare(object x, object y, bool extendedTypeComparisons);

		int Compare(object x, object y, bool throwExceptionOnComparisonFailure, bool extendedTypeComparisons, out bool validComparisonResult);
	}
}

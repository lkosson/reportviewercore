using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Common
{
	internal static class EnumeratorMapping
	{
		public static IEnumerable<U> Map<T, U>(IEnumerable<T> source, Converter<T, U> mapFunc)
		{
			foreach (T item in source)
			{
				yield return mapFunc(item);
			}
		}
	}
}

using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal class EqualityComparers
	{
		private class ObjectTypeEqualityComparer : IEqualityComparer<ObjectType>
		{
			internal ObjectTypeEqualityComparer()
			{
			}

			public bool Equals(ObjectType x, ObjectType y)
			{
				return x == y;
			}

			public int GetHashCode(ObjectType obj)
			{
				return (int)obj;
			}
		}

		private class StringEqualityComparer : IEqualityComparer<string>
		{
			internal StringEqualityComparer()
			{
			}

			public bool Equals(string str1, string str2)
			{
				return string.Equals(str1, str2, StringComparison.Ordinal);
			}

			public int GetHashCode(string str)
			{
				return str.GetHashCode();
			}
		}

		internal class Int32EqualityComparer : IEqualityComparer<int>, IComparer<int>
		{
			internal Int32EqualityComparer()
			{
			}

			public bool Equals(int x, int y)
			{
				return x == y;
			}

			public int GetHashCode(int obj)
			{
				return obj;
			}

			public int Compare(int x, int y)
			{
				return x - y;
			}
		}

		internal class ReversedInt32EqualityComparer : IEqualityComparer<int>, IComparer<int>
		{
			internal ReversedInt32EqualityComparer()
			{
			}

			public bool Equals(int x, int y)
			{
				return x == y;
			}

			public int GetHashCode(int obj)
			{
				return obj;
			}

			public int Compare(int x, int y)
			{
				return y - x;
			}
		}

		internal class Int64EqualityComparer : IEqualityComparer<long>, IComparer<long>
		{
			internal Int64EqualityComparer()
			{
			}

			public bool Equals(long x, long y)
			{
				return x == y;
			}

			public int GetHashCode(long obj)
			{
				return (int)obj;
			}

			public int Compare(long x, long y)
			{
				if (x < y)
				{
					return -1;
				}
				if (x > y)
				{
					return 1;
				}
				return 0;
			}
		}

		internal static readonly IEqualityComparer<ObjectType> ObjectTypeComparerInstance = new ObjectTypeEqualityComparer();

		internal static readonly Int32EqualityComparer Int32ComparerInstance = new Int32EqualityComparer();

		internal static readonly ReversedInt32EqualityComparer ReversedInt32ComparerInstance = new ReversedInt32EqualityComparer();

		internal static readonly Int64EqualityComparer Int64ComparerInstance = new Int64EqualityComparer();

		internal static readonly IEqualityComparer<string> StringComparerInstance = new StringEqualityComparer();
	}
}

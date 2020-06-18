using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Int64List : ArrayList
	{
		internal new long this[int index]
		{
			get
			{
				return (long)base[index];
			}
			set
			{
				base[index] = value;
			}
		}

		internal Int64List()
		{
		}

		internal Int64List(int capacity)
			: base(capacity)
		{
		}
	}
}

using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class BoolList : ArrayList
	{
		internal new bool this[int index]
		{
			get
			{
				return (bool)base[index];
			}
			set
			{
				base[index] = value;
			}
		}

		internal BoolList()
		{
		}

		internal BoolList(int capacity)
			: base(capacity)
		{
		}
	}
}

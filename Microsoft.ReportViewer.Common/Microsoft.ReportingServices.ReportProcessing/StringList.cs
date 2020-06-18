using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class StringList : ArrayList
	{
		internal new string this[int index]
		{
			get
			{
				return (string)base[index];
			}
			set
			{
				base[index] = value;
			}
		}

		internal StringList()
		{
		}

		internal StringList(int capacity)
			: base(capacity)
		{
		}
	}
}

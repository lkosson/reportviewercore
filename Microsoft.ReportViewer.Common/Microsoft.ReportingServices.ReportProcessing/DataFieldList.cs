using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataFieldList : ArrayList
	{
		internal new Field this[int index] => (Field)base[index];

		internal DataFieldList()
		{
		}

		internal DataFieldList(int capacity)
			: base(capacity)
		{
		}
	}
}

using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class RowList : ArrayList
	{
		internal new Row this[int index] => (Row)base[index];

		internal RowList()
		{
		}

		internal RowList(int capacity)
			: base(capacity)
		{
		}
	}
}

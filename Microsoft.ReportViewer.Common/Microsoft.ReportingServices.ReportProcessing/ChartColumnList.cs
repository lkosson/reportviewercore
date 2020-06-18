using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartColumnList : ArrayList
	{
		internal new ChartColumn this[int index] => (ChartColumn)base[index];

		internal ChartColumnList()
		{
		}

		internal ChartColumnList(int capacity)
			: base(capacity)
		{
		}
	}
}

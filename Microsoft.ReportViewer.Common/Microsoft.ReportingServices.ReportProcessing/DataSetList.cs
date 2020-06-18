using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataSetList : ArrayList
	{
		internal new DataSet this[int index] => (DataSet)base[index];

		internal DataSetList()
		{
		}

		internal DataSetList(int capacity)
			: base(capacity)
		{
		}
	}
}

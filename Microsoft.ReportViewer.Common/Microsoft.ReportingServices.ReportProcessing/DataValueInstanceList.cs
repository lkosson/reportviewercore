using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataValueInstanceList : ArrayList
	{
		internal new DataValueInstance this[int index] => (DataValueInstance)base[index];

		internal DataValueInstanceList()
		{
		}

		internal DataValueInstanceList(int capacity)
			: base(capacity)
		{
		}
	}
}

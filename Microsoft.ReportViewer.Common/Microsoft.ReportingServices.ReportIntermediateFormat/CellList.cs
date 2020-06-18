using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class CellList : ArrayList
	{
		internal new Cell this[int index] => (Cell)base[index];

		internal CellList()
		{
		}

		internal CellList(int capacity)
			: base(capacity)
		{
		}
	}
}

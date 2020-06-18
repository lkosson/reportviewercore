using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ActionItemInstanceList : ArrayList
	{
		internal new ActionItemInstance this[int index] => (ActionItemInstance)base[index];

		internal ActionItemInstanceList()
		{
		}

		internal ActionItemInstanceList(int capacity)
			: base(capacity)
		{
		}
	}
}

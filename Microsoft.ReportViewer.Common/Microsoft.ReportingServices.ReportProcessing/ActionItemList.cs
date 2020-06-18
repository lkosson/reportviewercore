using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ActionItemList : ArrayList
	{
		internal new ActionItem this[int index] => (ActionItem)base[index];

		internal ActionItemList()
		{
		}

		internal ActionItemList(int capacity)
			: base(capacity)
		{
		}
	}
}

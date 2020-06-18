using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class IntList : ArrayList
	{
		internal new int this[int index]
		{
			get
			{
				return (int)base[index];
			}
			set
			{
				base[index] = value;
			}
		}

		internal IntList()
		{
		}

		internal IntList(int capacity)
			: base(capacity)
		{
		}

		internal void CopyTo(IntList target)
		{
			if (target != null)
			{
				target.Clear();
				for (int i = 0; i < Count; i++)
				{
					target.Add(this[i]);
				}
			}
		}
	}
}

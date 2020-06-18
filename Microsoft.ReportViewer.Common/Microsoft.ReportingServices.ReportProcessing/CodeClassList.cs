using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CodeClassList : ArrayList
	{
		internal new CodeClass this[int index] => (CodeClass)base[index];

		internal CodeClassList()
		{
		}

		internal CodeClassList(int capacity)
			: base(capacity)
		{
		}
	}
}

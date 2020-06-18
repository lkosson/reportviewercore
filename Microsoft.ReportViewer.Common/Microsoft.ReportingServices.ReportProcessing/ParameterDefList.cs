using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ParameterDefList : ArrayList
	{
		internal new ParameterDef this[int index] => (ParameterDef)base[index];

		public ParameterDefList()
		{
		}

		internal ParameterDefList(int capacity)
			: base(capacity)
		{
		}
	}
}

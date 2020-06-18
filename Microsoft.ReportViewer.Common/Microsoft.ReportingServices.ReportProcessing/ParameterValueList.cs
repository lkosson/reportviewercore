using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ParameterValueList : ArrayList
	{
		internal new ParameterValue this[int index] => (ParameterValue)base[index];

		internal ParameterValueList()
		{
		}

		internal ParameterValueList(int capacity)
			: base(capacity)
		{
		}
	}
}

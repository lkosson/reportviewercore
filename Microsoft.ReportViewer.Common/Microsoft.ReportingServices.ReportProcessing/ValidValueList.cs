using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ValidValueList : ArrayList
	{
		public new ValidValue this[int index] => (ValidValue)base[index];

		public ValidValueList()
		{
		}

		public ValidValueList(int capacity)
			: base(capacity)
		{
		}
	}
}

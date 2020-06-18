using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class VariantList : ArrayList
	{
		internal VariantList()
		{
		}

		internal VariantList(int capacity)
			: base(capacity)
		{
		}
	}
}

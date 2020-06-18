using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal abstract class TablixHeadingList : ArrayList
	{
		internal new TablixHeading this[int index] => (TablixHeading)base[index];

		internal TablixHeadingList()
		{
		}

		internal TablixHeadingList(int capacity)
			: base(capacity)
		{
		}

		internal abstract TablixHeadingList InnerHeadings();
	}
}

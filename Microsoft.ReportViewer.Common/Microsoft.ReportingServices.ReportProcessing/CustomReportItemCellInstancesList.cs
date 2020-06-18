using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CustomReportItemCellInstancesList : ArrayList
	{
		internal new CustomReportItemCellInstanceList this[int index] => (CustomReportItemCellInstanceList)base[index];

		internal CustomReportItemCellInstancesList()
		{
		}

		internal CustomReportItemCellInstancesList(int capacity)
			: base(capacity)
		{
		}
	}
}

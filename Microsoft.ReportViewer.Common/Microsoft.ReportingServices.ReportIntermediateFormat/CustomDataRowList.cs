using System;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class CustomDataRowList : RowList
	{
		internal new CustomDataRow this[int index] => (CustomDataRow)base[index];

		public CustomDataRowList()
		{
		}

		internal CustomDataRowList(int capacity)
			: base(capacity)
		{
		}
	}
}

using System;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class TablixRowList : RowList
	{
		internal new TablixRow this[int index] => (TablixRow)base[index];

		public TablixRowList()
		{
		}

		internal TablixRowList(int capacity)
			: base(capacity)
		{
		}
	}
}

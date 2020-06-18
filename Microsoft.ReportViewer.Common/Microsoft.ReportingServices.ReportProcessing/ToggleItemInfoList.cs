using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class ToggleItemInfoList : ArrayList
	{
		internal new ToggleItemInfo this[int index] => (ToggleItemInfo)base[index];
	}
}

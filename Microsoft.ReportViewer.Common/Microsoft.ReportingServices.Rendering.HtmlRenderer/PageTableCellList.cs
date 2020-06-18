using System;
using System.Collections;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	[Serializable]
	internal sealed class PageTableCellList : ArrayList
	{
		internal new PageTableCell this[int index] => (PageTableCell)base[index];
	}
}

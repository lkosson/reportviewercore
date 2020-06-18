using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal class Border
	{
		internal int RowZIndex;

		internal int ColumnZIndex;

		internal int RowIndex;

		internal int ColumnIndex;

		internal bool CompareRowFirst = true;

		internal List<Operation> Operations;
	}
}

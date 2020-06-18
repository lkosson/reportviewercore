using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixCellInstanceList : ArrayList
	{
		internal new MatrixCellInstance this[int index] => (MatrixCellInstance)base[index];

		internal MatrixCellInstanceList()
		{
		}

		internal MatrixCellInstanceList(int capacity)
			: base(capacity)
		{
		}
	}
}

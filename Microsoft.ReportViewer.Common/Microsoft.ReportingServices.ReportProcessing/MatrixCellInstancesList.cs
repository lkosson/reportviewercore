using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixCellInstancesList : ArrayList
	{
		internal new MatrixCellInstanceList this[int index] => (MatrixCellInstanceList)base[index];

		internal MatrixCellInstancesList()
		{
		}

		internal MatrixCellInstancesList(int capacity)
			: base(capacity)
		{
		}
	}
}

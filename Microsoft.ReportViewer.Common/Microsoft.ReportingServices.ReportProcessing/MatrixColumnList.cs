using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixColumnList : ArrayList
	{
		internal new MatrixColumn this[int index] => (MatrixColumn)base[index];

		internal MatrixColumnList()
		{
		}

		internal MatrixColumnList(int capacity)
			: base(capacity)
		{
		}
	}
}

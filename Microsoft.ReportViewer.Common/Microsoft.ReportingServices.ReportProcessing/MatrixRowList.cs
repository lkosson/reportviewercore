using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixRowList : ArrayList
	{
		internal new MatrixRow this[int index] => (MatrixRow)base[index];

		internal MatrixRowList()
		{
		}

		internal MatrixRowList(int capacity)
			: base(capacity)
		{
		}
	}
}

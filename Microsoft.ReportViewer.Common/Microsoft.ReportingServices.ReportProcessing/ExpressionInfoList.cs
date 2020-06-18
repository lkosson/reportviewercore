using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ExpressionInfoList : ArrayList
	{
		internal new ExpressionInfo this[int index] => (ExpressionInfo)base[index];

		internal ExpressionInfoList()
		{
		}

		internal ExpressionInfoList(int capacity)
			: base(capacity)
		{
		}
	}
}

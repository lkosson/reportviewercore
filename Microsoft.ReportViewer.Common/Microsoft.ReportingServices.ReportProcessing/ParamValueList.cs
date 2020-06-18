using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class ParamValueList : ArrayList
	{
		internal new ParamValue this[int index] => (ParamValue)base[index];

		internal ParamValueList()
		{
		}

		internal ParamValueList(int capacity)
			: base(capacity)
		{
		}
	}
}

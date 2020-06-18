using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class DataAggregateObjResultsList : ArrayList
	{
		internal new DataAggregateObjResult[] this[int index] => (DataAggregateObjResult[])base[index];
	}
}

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal interface IAggregateHolder
	{
		DataAggregateInfoList[] GetAggregateLists();

		DataAggregateInfoList[] GetPostSortAggregateLists();

		void ClearIfEmpty();
	}
}

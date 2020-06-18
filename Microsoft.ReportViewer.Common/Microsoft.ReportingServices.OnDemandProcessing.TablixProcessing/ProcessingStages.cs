namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal enum ProcessingStages
	{
		Grouping = 1,
		SortAndFilter,
		PreparePeerGroupRunningValues,
		RunningValues,
		UserSortFilter,
		UpdateAggregates,
		CreateGroupTree
	}
}

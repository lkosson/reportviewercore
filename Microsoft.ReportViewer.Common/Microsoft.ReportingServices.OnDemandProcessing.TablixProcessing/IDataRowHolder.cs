namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal interface IDataRowHolder
	{
		void ReadRows(DataActions action, ITraversalContext context);

		void UpdateAggregates(AggregateUpdateContext context);

		void SetupEnvironment();
	}
}

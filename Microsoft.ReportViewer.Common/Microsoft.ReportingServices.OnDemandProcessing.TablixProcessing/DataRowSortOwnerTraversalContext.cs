namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal class DataRowSortOwnerTraversalContext : ITraversalContext
	{
		private IDataRowSortOwner m_sortOwner;

		internal IDataRowSortOwner SortOwner => m_sortOwner;

		internal DataRowSortOwnerTraversalContext(IDataRowSortOwner sortOwner)
		{
			m_sortOwner = sortOwner;
		}
	}
}

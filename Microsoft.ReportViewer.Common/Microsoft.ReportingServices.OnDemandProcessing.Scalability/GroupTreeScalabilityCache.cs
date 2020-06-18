namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class GroupTreeScalabilityCache : PartitionedTreeScalabilityCache
	{
		private const long CacheExpansionIntervalMs = 2000L;

		private const double CacheExpansionRatio = 0.3;

		private const long MinReservedMemoryBytes = 2097152L;

		public override ScalabilityCacheType CacheType => ScalabilityCacheType.GroupTree;

		internal GroupTreeScalabilityCache(TreePartitionManager partitionManager, IStorage storage)
			: base(partitionManager, storage, 2000L, 0.3, 2097152L)
		{
		}
	}
}

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class LookupScalabilityCache : PartitionedTreeScalabilityCache
	{
		private const long CacheExpansionIntervalMs = 2000L;

		private const double CacheExpansionRatio = 0.3;

		private const long MinReservedMemoryBytes = 2097152L;

		public override ScalabilityCacheType CacheType => ScalabilityCacheType.Lookup;

		internal LookupScalabilityCache(TreePartitionManager partitionManager, IStorage storage)
			: base(partitionManager, storage, 2000L, 0.3, 2097152L)
		{
		}

		internal override BaseReference TransferTo(BaseReference reference)
		{
			IStorable storable = reference.InternalValue();
			BaseReference baseReference = AllocateAndPin(storable, ItemSizes.SizeOf(storable));
			(storable as ITransferable)?.TransferTo(this);
			baseReference.UnPinValue();
			reference.ScalabilityCache.Free(reference);
			return baseReference;
		}
	}
}

using Microsoft.ReportingServices.Interfaces;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal static class ScalabilityUtils
	{
		public static IScalabilityCache CreateCacheForTransientAllocations(CreateAndRegisterStream createStreamCallback, string streamNamePrefix, IScalabilityObjectCreator objectCreator, IReferenceCreator referenceCreator, ComponentType componentType, int minReservedMemoryMB)
		{
			int rifCompatVersion = 0;
			ISpaceManager spaceManager = new PromoteLocalitySpaceManager(52428800L);
			RIFStorage storage = new RIFStorage(new CreateAndRegisterStreamHandler(streamNamePrefix + "_Data", createStreamCallback), 4096, 200, 500, spaceManager, objectCreator, referenceCreator, null, fromExistingStream: false, rifCompatVersion);
			IIndexStrategy indexStrategy = new IndexTable(new CreateAndRegisterStreamHandler(streamNamePrefix + "_Index", createStreamCallback), 1024, 100);
			return new ScalabilityCache(storage, indexStrategy, componentType, minReservedMemoryMB * 1048576);
		}
	}
}

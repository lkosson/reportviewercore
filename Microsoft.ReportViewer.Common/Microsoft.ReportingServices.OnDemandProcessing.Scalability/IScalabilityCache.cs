using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IScalabilityCache : PersistenceHelper, IDisposable
	{
		long SerializationDurationMs
		{
			get;
		}

		long DeserializationDurationMs
		{
			get;
		}

		long ScalabilityDurationMs
		{
			get;
		}

		long PeakMemoryUsageKBytes
		{
			get;
		}

		long CacheSizeKBytes
		{
			get;
		}

		ComponentType OwnerComponent
		{
			get;
		}

		IStorage Storage
		{
			get;
		}

		ScalabilityCacheType CacheType
		{
			get;
		}

		IReference<T> Allocate<T>(T obj, int priority) where T : IStorable;

		IReference<T> Allocate<T>(T obj, int priority, int initialSize) where T : IStorable;

		IReference<T> AllocateAndPin<T>(T obj, int priority) where T : IStorable;

		IReference<T> AllocateAndPin<T>(T obj, int priority, int initialSize) where T : IStorable;

		IReference<T> GenerateFixedReference<T>(T obj) where T : IStorable;

		void Close();

		int StoreStaticReference(object item);

		object FetchStaticReference(int id);

		IReference PoolReference(IReference reference);
	}
}

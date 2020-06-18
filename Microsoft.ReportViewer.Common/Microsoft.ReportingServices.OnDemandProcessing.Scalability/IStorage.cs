using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IStorage : IDisposable
	{
		long StreamSize
		{
			get;
		}

		IScalabilityCache ScalabilityCache
		{
			get;
			set;
		}

		IReferenceCreator ReferenceCreator
		{
			get;
		}

		bool FreezeAllocations
		{
			get;
			set;
		}

		IPersistable Retrieve(long offset, out long persistedSize);

		IPersistable Retrieve(long offset);

		T Retrieve<T>(long offset, out long persistedSize) where T : IPersistable, new();

		long Allocate(IPersistable obj);

		void Free(long offset, int size);

		long Update(IPersistable obj, long offset, long oldPersistedSize);

		void Close();

		void Flush();

		void TraceStats();
	}
}

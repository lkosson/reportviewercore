using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal abstract class Reference<T> : BaseReference, IReference<T>, IReference, IStorable, IPersistable, IDisposable where T : IStorable
	{
		internal Reference()
		{
		}

		T IReference<T>.Value()
		{
			return (T)InternalValue();
		}
	}
}

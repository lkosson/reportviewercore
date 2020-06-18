using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IDecumulator<T> : IEnumerator<T>, IDisposable, IEnumerator
	{
		void RemoveCurrent();
	}
}

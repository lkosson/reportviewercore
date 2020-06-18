using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal abstract class IScopeReference : Reference<IScope>
	{
		internal IScopeReference()
		{
		}

		[DebuggerStepThrough]
		public IScope Value()
		{
			return (IScope)InternalValue();
		}
	}
}

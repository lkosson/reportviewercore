using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class ScopeInstanceReference : Reference<ScopeInstance>
	{
		internal ScopeInstanceReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.ScopeInstanceReference;
		}

		public ScopeInstance Value()
		{
			return (ScopeInstance)InternalValue();
		}
	}
}

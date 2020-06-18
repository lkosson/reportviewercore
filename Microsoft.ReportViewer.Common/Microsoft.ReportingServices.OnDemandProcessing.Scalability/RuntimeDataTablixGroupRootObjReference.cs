using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeDataTablixGroupRootObjReference : RuntimeGroupRootObjReference, IReference<RuntimeDataTablixGroupRootObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeDataTablixGroupRootObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeDataTablixGroupRootObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeDataTablixGroupRootObj Value()
		{
			return (RuntimeDataTablixGroupRootObj)InternalValue();
		}
	}
}

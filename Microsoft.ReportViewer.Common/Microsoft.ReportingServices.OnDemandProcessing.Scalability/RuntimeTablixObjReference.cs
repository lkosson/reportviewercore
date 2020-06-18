using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeTablixObjReference : RuntimeDataTablixObjReference, IReference<RuntimeTablixObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeTablixObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeTablixObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeTablixObj Value()
		{
			return (RuntimeTablixObj)InternalValue();
		}
	}
}

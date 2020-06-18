using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeDetailObjReference : RuntimeHierarchyObjReference, IReference<RuntimeDetailObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeDetailObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeDetailObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeDetailObj Value()
		{
			return (RuntimeDetailObj)InternalValue();
		}
	}
}

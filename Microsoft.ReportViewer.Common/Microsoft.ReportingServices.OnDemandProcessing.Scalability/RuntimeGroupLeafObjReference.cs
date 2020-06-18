using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeGroupLeafObjReference : RuntimeGroupObjReference, IReference<RuntimeGroupLeafObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeGroupLeafObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeGroupLeafObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeGroupLeafObj Value()
		{
			return (RuntimeGroupLeafObj)InternalValue();
		}
	}
}

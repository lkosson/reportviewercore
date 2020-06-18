using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeDataTablixMemberObjReference : RuntimeMemberObjReference, IReference<RuntimeDataTablixMemberObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeDataTablixMemberObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeDataTablixMemberObjReference;
		}

		[DebuggerStepThrough]
		public RuntimeDataTablixMemberObj Value()
		{
			return (RuntimeDataTablixMemberObj)InternalValue();
		}
	}
}

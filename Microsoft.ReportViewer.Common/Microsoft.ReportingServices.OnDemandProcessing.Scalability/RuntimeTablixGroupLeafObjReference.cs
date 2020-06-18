using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeTablixGroupLeafObjReference : RuntimeDataTablixGroupLeafObjReference, IReference<RuntimeTablixGroupLeafObj>, IReference, IStorable, IPersistable, IReference<ISortDataHolder>
	{
		internal RuntimeTablixGroupLeafObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeTablixGroupLeafObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeTablixGroupLeafObj Value()
		{
			return (RuntimeTablixGroupLeafObj)InternalValue();
		}

		[DebuggerStepThrough]
		ISortDataHolder IReference<ISortDataHolder>.Value()
		{
			return (ISortDataHolder)InternalValue();
		}
	}
}

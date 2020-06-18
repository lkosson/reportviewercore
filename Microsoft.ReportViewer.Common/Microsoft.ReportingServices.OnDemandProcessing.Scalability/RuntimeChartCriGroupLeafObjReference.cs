using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeChartCriGroupLeafObjReference : RuntimeDataTablixGroupLeafObjReference, IReference<RuntimeChartCriGroupLeafObj>, IReference, IStorable, IPersistable, IReference<ISortDataHolder>
	{
		internal RuntimeChartCriGroupLeafObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeChartCriGroupLeafObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeChartCriGroupLeafObj Value()
		{
			return (RuntimeChartCriGroupLeafObj)InternalValue();
		}

		[DebuggerStepThrough]
		ISortDataHolder IReference<ISortDataHolder>.Value()
		{
			return (ISortDataHolder)InternalValue();
		}
	}
}

using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeMapDataRegionObjReference : RuntimeChartCriObjReference, IReference<RuntimeMapDataRegionObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeMapDataRegionObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeMapDataRegionObjReference;
		}

		public new RuntimeMapDataRegionObj Value()
		{
			return (RuntimeMapDataRegionObj)InternalValue();
		}
	}
}

using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeChartCriObjReference : RuntimeDataTablixObjReference, IReference<RuntimeChartCriObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeChartCriObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeChartCriObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeChartCriObj Value()
		{
			return (RuntimeChartCriObj)InternalValue();
		}
	}
}

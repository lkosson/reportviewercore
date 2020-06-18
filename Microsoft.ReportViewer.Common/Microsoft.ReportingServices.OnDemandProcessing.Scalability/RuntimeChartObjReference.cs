using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeChartObjReference : RuntimeChartCriObjReference, IReference<RuntimeChartObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeChartObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeChartObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeChartObj Value()
		{
			return (RuntimeChartObj)InternalValue();
		}
	}
}

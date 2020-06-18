using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeGaugePanelObjReference : RuntimeChartCriObjReference, IReference<RuntimeGaugePanelObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeGaugePanelObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeGaugePanelObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeGaugePanelObj Value()
		{
			return (RuntimeGaugePanelObj)InternalValue();
		}
	}
}

using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeTablixCellReference : RuntimeCellReference, IReference<RuntimeTablixCell>, IReference, IStorable, IPersistable
	{
		internal RuntimeTablixCellReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeTablixCellReference;
		}

		[DebuggerStepThrough]
		public new RuntimeTablixCell Value()
		{
			return (RuntimeTablixCell)InternalValue();
		}
	}
}

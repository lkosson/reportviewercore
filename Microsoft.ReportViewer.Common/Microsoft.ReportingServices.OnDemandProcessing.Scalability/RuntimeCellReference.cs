using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeCellReference : IScopeReference, IReference<RuntimeCell>, IReference, IStorable, IPersistable, IReference<IOnDemandScopeInstance>
	{
		internal RuntimeCellReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeCellReference;
		}

		[DebuggerStepThrough]
		public new RuntimeCell Value()
		{
			return (RuntimeCell)InternalValue();
		}

		[DebuggerStepThrough]
		IOnDemandScopeInstance IReference<IOnDemandScopeInstance>.Value()
		{
			return (IOnDemandScopeInstance)InternalValue();
		}
	}
}

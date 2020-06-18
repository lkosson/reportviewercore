using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class SortExpressionScopeInstanceHolderReference : Reference<IHierarchyObj>, IReference<RuntimeSortFilterEventInfo.SortExpressionScopeInstanceHolder>, IReference, IStorable, IPersistable
	{
		internal SortExpressionScopeInstanceHolderReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.SortExpressionScopeInstanceHolderReference;
		}

		[DebuggerStepThrough]
		public RuntimeSortFilterEventInfo.SortExpressionScopeInstanceHolder Value()
		{
			return (RuntimeSortFilterEventInfo.SortExpressionScopeInstanceHolder)InternalValue();
		}
	}
}

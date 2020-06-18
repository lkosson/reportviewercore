using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class SortFilterExpressionScopeObjReference : Reference<IHierarchyObj>, IReference<RuntimeSortFilterEventInfo.SortFilterExpressionScopeObj>, IReference, IStorable, IPersistable
	{
		internal SortFilterExpressionScopeObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.SortFilterExpressionScopeObjReference;
		}

		[DebuggerStepThrough]
		public RuntimeSortFilterEventInfo.SortFilterExpressionScopeObj Value()
		{
			return (RuntimeSortFilterEventInfo.SortFilterExpressionScopeObj)InternalValue();
		}
	}
}

using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeSortHierarchyObjReference : Reference<IHierarchyObj>, IReference<RuntimeSortHierarchyObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeSortHierarchyObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeSortHierarchyObjReference;
		}

		[DebuggerStepThrough]
		public RuntimeSortHierarchyObj Value()
		{
			return (RuntimeSortHierarchyObj)InternalValue();
		}
	}
}

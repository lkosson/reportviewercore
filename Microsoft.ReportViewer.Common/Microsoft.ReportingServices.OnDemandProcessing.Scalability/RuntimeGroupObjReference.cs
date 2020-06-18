using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeGroupObjReference : RuntimeHierarchyObjReference, IReference<RuntimeGroupObj>, IReference, IStorable, IPersistable, IReference<Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>
	{
		internal RuntimeGroupObjReference()
		{
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeGroupObj Value()
		{
			return (RuntimeGroupObj)InternalValue();
		}

		Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner IReference<Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>.Value()
		{
			return (Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner)InternalValue();
		}
	}
}

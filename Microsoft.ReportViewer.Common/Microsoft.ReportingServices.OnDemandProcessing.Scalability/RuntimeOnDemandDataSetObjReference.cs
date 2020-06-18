using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeOnDemandDataSetObjReference : IScopeReference, IReference<IHierarchyObj>, IReference, IStorable, IPersistable, IReference<RuntimeOnDemandDataSetObj>, IReference<Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>
	{
		internal RuntimeOnDemandDataSetObjReference()
		{
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeOnDemandDataSetObjReference;
		}

		[DebuggerStepThrough]
		IHierarchyObj IReference<IHierarchyObj>.Value()
		{
			return (IHierarchyObj)InternalValue();
		}

		[DebuggerStepThrough]
		public new RuntimeOnDemandDataSetObj Value()
		{
			return (RuntimeOnDemandDataSetObj)InternalValue();
		}

		[DebuggerStepThrough]
		Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner IReference<Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>.Value()
		{
			return (Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner)InternalValue();
		}
	}
}

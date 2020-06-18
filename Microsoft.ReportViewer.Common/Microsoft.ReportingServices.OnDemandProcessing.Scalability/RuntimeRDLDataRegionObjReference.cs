using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeRDLDataRegionObjReference : RuntimeDataRegionObjReference, IReference<IHierarchyObj>, IReference, IStorable, IPersistable, IReference<RuntimeRDLDataRegionObj>, IReference<IDataRowSortOwner>, IReference<Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>, IReference<IDataCorrelation>
	{
		internal RuntimeRDLDataRegionObjReference()
		{
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRDLDataRegionObjReference;
		}

		[DebuggerStepThrough]
		IHierarchyObj IReference<IHierarchyObj>.Value()
		{
			return (IHierarchyObj)InternalValue();
		}

		[DebuggerStepThrough]
		public new RuntimeRDLDataRegionObj Value()
		{
			return (RuntimeRDLDataRegionObj)InternalValue();
		}

		[DebuggerStepThrough]
		IDataRowSortOwner IReference<IDataRowSortOwner>.Value()
		{
			return (IDataRowSortOwner)InternalValue();
		}

		[DebuggerStepThrough]
		Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner IReference<Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>.Value()
		{
			return (Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner)InternalValue();
		}

		[DebuggerStepThrough]
		IDataCorrelation IReference<IDataCorrelation>.Value()
		{
			return (IDataCorrelation)InternalValue();
		}
	}
}

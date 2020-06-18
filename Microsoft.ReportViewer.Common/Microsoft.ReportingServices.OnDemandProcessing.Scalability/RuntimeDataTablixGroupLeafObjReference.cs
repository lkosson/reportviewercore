using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeDataTablixGroupLeafObjReference : RuntimeGroupLeafObjReference, IReference<RuntimeDataTablixGroupLeafObj>, IReference, IStorable, IPersistable, IOnDemandMemberInstanceReference, IOnDemandMemberOwnerInstanceReference, IReference<IOnDemandScopeInstance>, IReference<IOnDemandMemberOwnerInstance>, IReference<IOnDemandMemberInstance>
	{
		internal RuntimeDataTablixGroupLeafObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeDataTablixGroupLeafObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeDataTablixGroupLeafObj Value()
		{
			return (RuntimeDataTablixGroupLeafObj)InternalValue();
		}

		[DebuggerStepThrough]
		IOnDemandScopeInstance IReference<IOnDemandScopeInstance>.Value()
		{
			return (IOnDemandScopeInstance)InternalValue();
		}

		[DebuggerStepThrough]
		IOnDemandMemberOwnerInstance IReference<IOnDemandMemberOwnerInstance>.Value()
		{
			return (IOnDemandMemberOwnerInstance)InternalValue();
		}

		[DebuggerStepThrough]
		IOnDemandMemberInstance IReference<IOnDemandMemberInstance>.Value()
		{
			return (IOnDemandMemberInstance)InternalValue();
		}
	}
}

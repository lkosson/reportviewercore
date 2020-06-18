using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class SyntheticOnDemandMemberOwnerInstanceReference : SyntheticOnDemandScopeInstanceReference, IOnDemandMemberOwnerInstanceReference, IReference<IOnDemandScopeInstance>, IReference, IStorable, IPersistable, IReference<IOnDemandMemberOwnerInstance>
	{
		public SyntheticOnDemandMemberOwnerInstanceReference(IOnDemandMemberOwnerInstance memberOwner)
			: base(memberOwner)
		{
		}

		IOnDemandMemberOwnerInstance IReference<IOnDemandMemberOwnerInstance>.Value()
		{
			return (IOnDemandMemberOwnerInstance)Value();
		}
	}
}

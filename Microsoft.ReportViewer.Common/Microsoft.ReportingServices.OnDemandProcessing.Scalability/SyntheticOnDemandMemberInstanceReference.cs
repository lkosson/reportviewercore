using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class SyntheticOnDemandMemberInstanceReference : SyntheticOnDemandScopeInstanceReference, IOnDemandMemberInstanceReference, IOnDemandMemberOwnerInstanceReference, IReference<IOnDemandScopeInstance>, IReference, IStorable, IPersistable, IReference<IOnDemandMemberOwnerInstance>, IReference<IOnDemandMemberInstance>
	{
		public SyntheticOnDemandMemberInstanceReference(IOnDemandMemberInstance member)
			: base(member)
		{
		}

		IOnDemandMemberOwnerInstance IReference<IOnDemandMemberOwnerInstance>.Value()
		{
			return (IOnDemandMemberOwnerInstance)Value();
		}

		IOnDemandMemberInstance IReference<IOnDemandMemberInstance>.Value()
		{
			return (IOnDemandMemberInstance)Value();
		}
	}
}

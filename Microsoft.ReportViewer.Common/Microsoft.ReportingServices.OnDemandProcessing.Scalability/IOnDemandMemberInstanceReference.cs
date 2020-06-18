using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IOnDemandMemberInstanceReference : IOnDemandMemberOwnerInstanceReference, IReference<IOnDemandScopeInstance>, IReference, IStorable, IPersistable, IReference<IOnDemandMemberOwnerInstance>, IReference<IOnDemandMemberInstance>
	{
	}
}

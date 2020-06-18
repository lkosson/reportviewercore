using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IOnDemandMemberOwnerInstanceReference : IReference<IOnDemandScopeInstance>, IReference, IStorable, IPersistable, IReference<IOnDemandMemberOwnerInstance>
	{
	}
}

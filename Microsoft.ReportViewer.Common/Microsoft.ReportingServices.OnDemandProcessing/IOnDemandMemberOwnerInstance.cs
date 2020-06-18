using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal interface IOnDemandMemberOwnerInstance : IOnDemandScopeInstance, IStorable, IPersistable
	{
		IOnDemandMemberInstanceReference GetFirstMemberInstance(ReportHierarchyNode rifMember);
	}
}

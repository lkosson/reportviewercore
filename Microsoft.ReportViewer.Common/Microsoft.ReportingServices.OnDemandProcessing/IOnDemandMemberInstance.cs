using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal interface IOnDemandMemberInstance : IOnDemandMemberOwnerInstance, IOnDemandScopeInstance, IStorable, IPersistable
	{
		List<object> GroupExprValues
		{
			get;
		}

		IOnDemandMemberInstanceReference GetNextMemberInstance();

		IOnDemandScopeInstance GetCellInstance(IOnDemandMemberInstanceReference outerGroupInstanceRef, out IReference<IOnDemandScopeInstance> cellRef);
	}
}

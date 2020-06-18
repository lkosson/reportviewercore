using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal class CreateInstancesTraversalContext : ITraversalContext
	{
		private ScopeInstance m_parentInstance;

		private IReference<RuntimeMemberObj>[] m_innerMembers;

		private IReference<RuntimeDataTablixGroupLeafObj> m_innerGroupLeafRef;

		internal ScopeInstance ParentInstance => m_parentInstance;

		internal IReference<RuntimeMemberObj>[] InnerMembers => m_innerMembers;

		internal IReference<RuntimeDataTablixGroupLeafObj> InnerGroupLeafRef => m_innerGroupLeafRef;

		internal CreateInstancesTraversalContext(ScopeInstance parentInstance, IReference<RuntimeMemberObj>[] innerMembers, IReference<RuntimeDataTablixGroupLeafObj> innerGroupLeafRef)
		{
			m_parentInstance = parentInstance;
			m_innerMembers = innerMembers;
			m_innerGroupLeafRef = innerGroupLeafRef;
		}
	}
}

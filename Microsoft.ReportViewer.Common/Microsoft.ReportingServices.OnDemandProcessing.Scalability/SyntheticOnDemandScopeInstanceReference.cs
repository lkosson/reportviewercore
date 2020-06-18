using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class SyntheticOnDemandScopeInstanceReference : SyntheticReferenceBase<IOnDemandScopeInstance>
	{
		private readonly IOnDemandScopeInstance m_value;

		public SyntheticOnDemandScopeInstanceReference(IOnDemandScopeInstance scopeInstance)
		{
			m_value = scopeInstance;
		}

		public override IOnDemandScopeInstance Value()
		{
			return m_value;
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.SyntheticOnDemandScopeInstanceReference;
		}
	}
}

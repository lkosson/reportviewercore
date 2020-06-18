namespace Microsoft.ReportingServices.DataExtensions
{
	internal sealed class SecureStoreLookup
	{
		public enum LookupContextOptions
		{
			AuthenticatedUser,
			Unattended
		}

		public readonly string m_targetApplicationId;

		public readonly LookupContextOptions m_lookUpContext;

		public LookupContextOptions LookupContext => m_lookUpContext;

		public string TargetApplicationId => m_targetApplicationId;

		internal SecureStoreLookup(LookupContextOptions lookupContext, string targetApplicationId)
		{
			m_lookUpContext = lookupContext;
			m_targetApplicationId = targetApplicationId;
		}
	}
}

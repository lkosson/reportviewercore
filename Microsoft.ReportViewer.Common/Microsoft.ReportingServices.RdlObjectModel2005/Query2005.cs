using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel2005.Upgrade;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class Query2005 : Query, IUpgradeable
	{
		public Query2005()
		{
		}

		public Query2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public void Upgrade(UpgradeImpl2005 upgrader)
		{
			upgrader.UpgradeQuery(this);
		}
	}
}

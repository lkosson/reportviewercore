namespace Microsoft.ReportingServices.RdlObjectModel2005.Upgrade
{
	internal interface IUpgradeable
	{
		void Upgrade(UpgradeImpl2005 upgrader);
	}
}

namespace Microsoft.ReportingServices.RdlObjectModel2010.Upgrade
{
	internal interface IUpgradeable2010
	{
		void Upgrade(UpgradeImpl2010 upgrader);
	}
}

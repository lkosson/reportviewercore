using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel2010.Upgrade;

namespace Microsoft.ReportingServices.RdlObjectModel2010
{
	internal class StateIndicator2010 : StateIndicator, IUpgradeable2010
	{
		public void Upgrade(UpgradeImpl2010 upgrader)
		{
			if (base.Style != null)
			{
				base.Style.Border = new Border();
				base.Style.Border.Style = BorderStyles.Solid;
			}
		}
	}
}

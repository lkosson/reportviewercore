using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel2005.Upgrade;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal interface IReportItem2005 : IUpgradeable
	{
		Action Action
		{
			get;
			set;
		}
	}
}

using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel2005.Upgrade;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal interface IPageBreakLocation2005 : IUpgradeable
	{
		bool PageBreakAtStart
		{
			get;
			set;
		}

		bool PageBreakAtEnd
		{
			get;
			set;
		}

		PageBreak PageBreak
		{
			get;
			set;
		}
	}
}

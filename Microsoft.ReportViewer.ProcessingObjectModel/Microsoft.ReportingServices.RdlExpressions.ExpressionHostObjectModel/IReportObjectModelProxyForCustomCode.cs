using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public interface IReportObjectModelProxyForCustomCode
	{
		Parameters Parameters
		{
			get;
		}

		Globals Globals
		{
			get;
		}

		User User
		{
			get;
		}

		Variables Variables
		{
			get;
		}
	}
}

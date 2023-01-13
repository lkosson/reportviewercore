using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
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
	}
}

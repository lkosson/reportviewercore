using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Specialized;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface IInteractivityPaginationModule
	{
		int ProcessFindStringEvent(Report report, int totalPages, int startPage, int endPage, string findValue);

		int ProcessBookmarkNavigationEvent(Report report, int totalPages, string bookmarkId, out string uniqueName);

		int ProcessUserSortEvent(Report report, string textbox, ref int numberOfPages, ref PaginationMode paginationMode);

		string ProcessDrillthroughEvent(Report report, int totalPages, string drillthroughId, out NameValueCollection parameters);

		int ProcessDocumentMapNavigationEvent(Report report, string documentMapId);
	}
}

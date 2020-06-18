using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Specialized;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal class SPBInteractivityProcessing : IInteractivityPaginationModule
	{
		public int ProcessFindStringEvent(Microsoft.ReportingServices.OnDemandReportRendering.Report report, int totalPages, int startPage, int endPage, string findValue)
		{
			if (findValue == null || startPage <= 0 || endPage <= 0)
			{
				return 0;
			}
			int num = 0;
			using (SPBProcessing sPBProcessing = new SPBProcessing(report, totalPages, needTotalPages: true))
			{
				sPBProcessing.CanTracePagination = true;
				sPBProcessing.SetContext(new SPBContext(0, 0, addToggledItems: false));
				return sPBProcessing.FindString(startPage, endPage, findValue);
			}
		}

		public int ProcessUserSortEvent(Microsoft.ReportingServices.OnDemandReportRendering.Report report, string textbox, ref int numberOfPages, ref PaginationMode paginationMode)
		{
			if (textbox == null)
			{
				return 0;
			}
			int num = 0;
			using (SPBProcessing sPBProcessing = new SPBProcessing(report, 0, needTotalPages: false))
			{
				sPBProcessing.CanTracePagination = true;
				sPBProcessing.SetContext(new SPBContext(0, 0, addToggledItems: true));
				return sPBProcessing.FindUserSort(textbox, ref numberOfPages, ref paginationMode);
			}
		}

		public int ProcessBookmarkNavigationEvent(Microsoft.ReportingServices.OnDemandReportRendering.Report report, int totalPages, string bookmarkId, out string uniqueName)
		{
			uniqueName = null;
			if (!report.HasBookmarks)
			{
				return 0;
			}
			if (bookmarkId == null)
			{
				return 0;
			}
			int lastPageCollected = 0;
			bool reportDone = false;
			int num = InteractivityChunks.FindBoomark(report, bookmarkId, ref uniqueName, ref lastPageCollected, ref reportDone);
			if (!reportDone && num == 0)
			{
				using (SPBProcessing sPBProcessing = new SPBProcessing(report, totalPages, needTotalPages: true))
				{
					sPBProcessing.CanTracePagination = true;
					sPBProcessing.SetContext(new SPBContext(0, 0, addToggledItems: true));
					return sPBProcessing.FindBookmark(bookmarkId, lastPageCollected, ref uniqueName);
				}
			}
			return num;
		}

		public string ProcessDrillthroughEvent(Microsoft.ReportingServices.OnDemandReportRendering.Report report, int totalPages, string drillthroughId, out NameValueCollection parameters)
		{
			parameters = null;
			if (drillthroughId == null)
			{
				return null;
			}
			int lastPageCollected = 0;
			string text = null;
			using (SPBProcessing sPBProcessing = new SPBProcessing(report, totalPages, needTotalPages: true))
			{
				sPBProcessing.CanTracePagination = true;
				sPBProcessing.SetContext(new SPBContext(0, 0, addToggledItems: true));
				return sPBProcessing.FindDrillthrough(drillthroughId, lastPageCollected, out parameters);
			}
		}

		public int ProcessDocumentMapNavigationEvent(Microsoft.ReportingServices.OnDemandReportRendering.Report report, string documentMapId)
		{
			if (!report.HasDocumentMap)
			{
				return 0;
			}
			if (documentMapId == null)
			{
				return 0;
			}
			int lastPageCollected = 0;
			bool reportDone = false;
			int num = InteractivityChunks.FindDocumentMapLabel(report, documentMapId, ref lastPageCollected, ref reportDone);
			if (!reportDone && num == 0)
			{
				using (SPBProcessing sPBProcessing = new SPBProcessing(report, 0, needTotalPages: false))
				{
					sPBProcessing.CanTracePagination = true;
					sPBProcessing.SetContext(new SPBContext(0, 0, addToggledItems: true));
					return sPBProcessing.FindDocumentMap(documentMapId, lastPageCollected);
				}
			}
			return num;
		}
	}
}

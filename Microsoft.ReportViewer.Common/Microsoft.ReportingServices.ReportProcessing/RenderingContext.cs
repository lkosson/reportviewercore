using Microsoft.ReportingServices.Diagnostics;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class RenderingContext
	{
		private ICatalogItemContext m_reportContext;

		private string m_reportDescription;

		private EventInformation m_eventInfo;

		internal ReportProcessing.GetReportChunk m_getReportChunkCallback;

		internal ReportProcessing.GetChunkMimeType m_getChunkMimeType;

		private ReportProcessing.StoreServerParameters m_storeServerParameters;

		private UserProfileState m_allowUserProfileState;

		private ReportRuntimeSetup m_reportRuntimeSetup;

		private PaginationMode m_clientPaginationMode;

		private int m_previousTotalPages;

		internal string Format => m_reportContext.RSRequestParameters.FormatParamValue;

		internal Uri ReportUri
		{
			get
			{
				if (string.IsNullOrEmpty(m_reportContext.HostRootUri))
				{
					return null;
				}
				return new Uri(new CatalogItemUrlBuilder(m_reportContext).ToString());
			}
		}

		internal string ShowHideToggle => m_reportContext.RSRequestParameters.ShowHideToggleParamValue;

		internal ICatalogItemContext ReportContext => m_reportContext;

		internal string ReportDescription => m_reportDescription;

		internal EventInformation EventInfo
		{
			get
			{
				return m_eventInfo;
			}
			set
			{
				m_eventInfo = value;
			}
		}

		internal ReportProcessing.StoreServerParameters StoreServerParametersCallback => m_storeServerParameters;

		internal UserProfileState AllowUserProfileState => m_allowUserProfileState;

		internal ReportRuntimeSetup ReportRuntimeSetup => m_reportRuntimeSetup;

		internal PaginationMode ClientPaginationMode => m_clientPaginationMode;

		internal int PreviousTotalPages => m_previousTotalPages;

		internal RenderingContext(ICatalogItemContext reportContext, string reportDescription, EventInformation eventInfo, ReportRuntimeSetup reportRuntimeSetup, ReportProcessing.StoreServerParameters storeServerParameters, UserProfileState allowUserProfileState, PaginationMode clientPaginationMode, int previousTotalPages)
		{
			Global.Tracer.Assert(reportContext != null, "(null != reportContext)");
			m_reportContext = reportContext;
			m_reportDescription = reportDescription;
			m_eventInfo = eventInfo;
			m_storeServerParameters = storeServerParameters;
			m_allowUserProfileState = allowUserProfileState;
			m_reportRuntimeSetup = reportRuntimeSetup;
			m_clientPaginationMode = clientPaginationMode;
			m_previousTotalPages = previousTotalPages;
		}

		internal Hashtable GetRenderProperties(bool reprocessSnapshot)
		{
			Hashtable hashtable = new Hashtable(4);
			if (reprocessSnapshot)
			{
				hashtable.Add("ClientPaginationMode", m_clientPaginationMode);
				hashtable.Add("PreviousTotalPages", 0);
			}
			else
			{
				hashtable.Add("ClientPaginationMode", m_clientPaginationMode);
				hashtable.Add("PreviousTotalPages", m_previousTotalPages);
			}
			return hashtable;
		}
	}
}

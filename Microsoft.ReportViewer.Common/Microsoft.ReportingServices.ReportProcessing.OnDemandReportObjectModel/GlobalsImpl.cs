using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class GlobalsImpl : Globals
	{
		private string m_reportName;

		private int m_pageNumber;

		private int m_totalPages;

		private int m_overallPageNumber;

		private int m_overallTotalPages;

		private DateTime m_executionTime;

		private string m_reportServerUrl;

		private string m_reportFolder;

		private RenderFormat m_renderFormat;

		private string m_pageName;

		public override object this[string key]
		{
			get
			{
				switch (key)
				{
				case "ReportName":
					return m_reportName;
				case "PageNumber":
					return m_pageNumber;
				case "TotalPages":
					return m_totalPages;
				case "OverallPageNumber":
					return m_overallPageNumber;
				case "OverallTotalPages":
					return m_overallTotalPages;
				case "ExecutionTime":
					return m_executionTime;
				case "ReportServerUrl":
					return m_reportServerUrl;
				case "ReportFolder":
					return m_reportFolder;
				case "RenderFormat":
					return m_renderFormat;
				case "PageName":
					return m_pageName;
				default:
					throw new ReportProcessingException_NonExistingGlobalReference(key);
				}
			}
		}

		public override string ReportName => m_reportName;

		public override int PageNumber => m_pageNumber;

		public override int TotalPages => m_totalPages;

		public override int OverallPageNumber => m_overallPageNumber;

		public override int OverallTotalPages => m_overallTotalPages;

		public override DateTime ExecutionTime => m_executionTime;

		public override string ReportServerUrl => m_reportServerUrl;

		public override string ReportFolder => m_reportFolder;

		public override RenderFormat RenderFormat => m_renderFormat;

		public override string PageName => m_pageName;

		internal GlobalsImpl(OnDemandProcessingContext odpContext)
		{
			m_reportName = odpContext.ReportContext.ItemName;
			m_executionTime = odpContext.ExecutionTime;
			m_reportServerUrl = odpContext.ReportContext.HostRootUri;
			m_reportFolder = odpContext.ReportFolder;
			m_pageNumber = 1;
			m_totalPages = 1;
			m_overallPageNumber = 1;
			m_overallTotalPages = 1;
			m_pageName = null;
			m_renderFormat = new RenderFormat(new RenderFormatImpl(odpContext));
		}

		internal GlobalsImpl(string reportName, int pageNumber, int totalPages, int overallPageNumber, int overallTotalPages, DateTime executionTime, string reportServerUrl, string reportFolder, string pageName, OnDemandProcessingContext odpContext)
		{
			m_reportName = reportName;
			m_pageNumber = pageNumber;
			m_totalPages = totalPages;
			m_overallPageNumber = overallPageNumber;
			m_overallTotalPages = overallTotalPages;
			m_executionTime = executionTime;
			m_reportServerUrl = reportServerUrl;
			m_reportFolder = reportFolder;
			m_pageName = pageName;
			m_renderFormat = new RenderFormat(new RenderFormatImpl(odpContext));
		}

		internal void SetPageNumbers(int pageNumber, int totalPages, int overallPageNumber, int overallTotalPages)
		{
			m_pageNumber = pageNumber;
			m_totalPages = totalPages;
			m_overallPageNumber = overallPageNumber;
			m_overallTotalPages = overallTotalPages;
		}

		internal void SetPageName(string pageName)
		{
			m_pageName = pageName;
		}
	}
}

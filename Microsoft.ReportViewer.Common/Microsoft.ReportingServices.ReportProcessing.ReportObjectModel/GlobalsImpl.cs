using System;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	internal sealed class GlobalsImpl : Globals
	{
		private string m_reportName;

		private int m_pageNumber;

		private int m_totalPages;

		private DateTime m_executionTime;

		private string m_reportServerUrl;

		private string m_reportFolder;

		internal const string Name = "Globals";

		internal const string FullName = "Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.Globals";

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
				case "ExecutionTime":
					return m_executionTime;
				case "ReportServerUrl":
					return m_reportServerUrl;
				case "ReportFolder":
					return m_reportFolder;
				case "RenderFormat":
					return new NotSupportedException();
				default:
					throw new ArgumentOutOfRangeException("key");
				}
			}
		}

		public override string ReportName => m_reportName;

		public override int PageNumber => m_pageNumber;

		public override int TotalPages => m_totalPages;

		public override int OverallPageNumber => m_pageNumber;

		public override int OverallTotalPages => m_totalPages;

		public override DateTime ExecutionTime => m_executionTime;

		public override string ReportServerUrl => m_reportServerUrl;

		public override string ReportFolder => m_reportFolder;

		public override string PageName => null;

		public override RenderFormat RenderFormat
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		internal GlobalsImpl(string reportName, DateTime executionTime, string reportServerUrl, string reportFolder)
		{
			m_reportName = reportName;
			m_pageNumber = 1;
			m_totalPages = 1;
			m_executionTime = executionTime;
			m_reportServerUrl = reportServerUrl;
			m_reportFolder = reportFolder;
		}

		internal GlobalsImpl(string reportName, int pageNumber, int totalPages, DateTime executionTime, string reportServerUrl, string reportFolder)
		{
			m_reportName = reportName;
			m_pageNumber = pageNumber;
			m_totalPages = totalPages;
			m_executionTime = executionTime;
			m_reportServerUrl = reportServerUrl;
			m_reportFolder = reportFolder;
		}

		internal void SetPageNumber(int pageNumber)
		{
			m_pageNumber = pageNumber;
		}

		internal void SetPageNumbers(int pageNumber, int totalPages)
		{
			m_pageNumber = pageNumber;
			m_totalPages = totalPages;
		}
	}
}

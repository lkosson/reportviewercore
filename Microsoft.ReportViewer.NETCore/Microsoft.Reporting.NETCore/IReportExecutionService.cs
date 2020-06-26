using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;

namespace Microsoft.Reporting.NETCore
{
	internal interface IReportExecutionService
	{
		int Timeout
		{
			set;
		}

		string BearerToken
		{
			get;
			set;
		}

		ExecutionInfo GetExecutionInfo();

		ExecutionInfo ResetExecution();

		ExecutionInfo LoadReport(string report, string historyId);

		ExecutionInfo LoadReportDefinition(byte[] definition);

		DocumentMapNode GetDocumentMap(string rootLabel);

		RenderingExtension[] ListRenderingExtensions();

		ExecutionInfo SetExecutionCredentials(IEnumerable<DataSourceCredentials> credentials);

		ExecutionInfo SetExecutionParameters(IEnumerable<ReportParameter> parameters, string parameterLanguage);

		byte[] Render(string format, string deviceInfo, PageCountMode paginationMode, out string extension, out string mimeType, out string encoding, out Warning[] warnings, out string[] streamIds);

		void Render(AbortState abortState, string reportPath, string executionId, string historyId, string format, XmlNodeList deviceInfo, NameValueCollection urlAccessParameters, Stream reportStream, out string mimeType, out string fileNameExtension);

		byte[] RenderStream(string format, string streamId, string deviceInfo, out string encoding, out string mimeType);

		void DeliverReportItem(string format, string deviceInfo, ExtensionSettings extensionSettings, string description, string eventType, string matchData);

		int FindString(int startPage, int endPage, string findValue);

		void ToggleItem(string toggleId);

		int NavigateBookmark(string bookmarkId, out string uniqueName);

		int NavigateDocumentMap(string documentMapId);

		ExecutionInfo LoadDrillthroughTarget(string drillthroughId);

		int Sort(string sortItem, SortOrder direction, bool clear, PageCountMode paginationMode, out string reportItem, out ExecutionInfo executionInfo, out int numPages);

		void SetExecutionId(string executionId);

		string GetServerVersion();
	}
}

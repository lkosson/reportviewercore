using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.Text;

namespace Microsoft.Reporting.NETCore.Internal.Soap.ReportingServices2005.Execution
{
	partial class ReportExecutionServiceSoapClient
	{
		public TrustedUserHeader TrustedUserHeaderValue { get; set; }
		public ServerInfoHeader ServerInfoHeaderValue { get; set; }
		public ExecutionHeader ExecutionHeaderValue { get; set; }

		public ICredentials Credentials
		{
			get => new NetworkCredential(ClientCredentials.Windows.ClientCredential.UserName, ClientCredentials.Windows.ClientCredential.Password);

			set
			{
				var cred = value.GetCredential(null, null);
				ClientCredentials.Windows.ClientCredential.UserName = cred.UserName;
				ClientCredentials.Windows.ClientCredential.Password = cred.Password;
				ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Delegation;
				var binding = (BasicHttpBinding)Endpoint.Binding;
				binding.Security.Mode = Endpoint.Address.Uri.Scheme == "https" ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.TransportCredentialOnly;
				binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
			}
		}

		public string Url {  get { return Endpoint.Address.Uri.ToString(); } set { Endpoint.Address = new System.ServiceModel.EndpointAddress(value); } }
		public int Timeout { get { return (int)Endpoint.Binding.OpenTimeout.TotalMilliseconds; } set { var ts = TimeSpan.FromMilliseconds(value); Endpoint.Binding.ReceiveTimeout = ts; Endpoint.Binding.SendTimeout = ts; Endpoint.Binding.OpenTimeout = ts; } }

		public ReportExecutionServiceSoapClient()
			: this(EndpointConfiguration.ReportExecutionServiceSoap)
		{
		}

		public string[] ListSecureMethods()
		{
			ServerInfoHeaderValue = ListSecureMethods(TrustedUserHeaderValue, out var result);
			return result;
		}

		public ExecutionInfo LoadReport(string Report, string HistoryID)
		{
			ExecutionHeaderValue = LoadReport(TrustedUserHeaderValue, Report, HistoryID, out var serverInfoHeader, out var executionInfo);
			return executionInfo;
		}

		public ExecutionInfo LoadReport2(string Report, string HistoryID)
		{
			ExecutionHeaderValue = LoadReport2(TrustedUserHeaderValue, Report, HistoryID, out var serverInfoHeader, out var executionInfo);
			return executionInfo;
		}

		public ExecutionInfo LoadReport3(string Report, string HistoryID)
		{
			ExecutionHeaderValue = LoadReport3(TrustedUserHeaderValue, Report, HistoryID, out var serverInfoHeader, out var executionInfo);
			return executionInfo;
		}

		public ExecutionInfo LoadReportDefinition(byte[] Definition, out Warning[] warnings)
		{
			ExecutionHeaderValue = LoadReportDefinition(TrustedUserHeaderValue, Definition, out var serverInfoHeader, out var executionInfo, out warnings);
			return executionInfo;
		}

		public ExecutionInfo LoadReportDefinition2(byte[] Definition, out Warning[] warnings)
		{
			ExecutionHeaderValue = LoadReportDefinition2(TrustedUserHeaderValue, Definition, out var serverInfoHeader, out var executionInfo, out warnings);
			return executionInfo;
		}

		public ExecutionInfo LoadReportDefinition3(byte[] Definition, out Warning[] warnings)
		{
			ExecutionHeaderValue = LoadReportDefinition3(TrustedUserHeaderValue, Definition, out var serverInfoHeader, out var executionInfo, out warnings);
			return executionInfo;
		}

		public ExecutionInfo SetExecutionCredentials(DataSourceCredentials[] Credentials)
		{
			ServerInfoHeaderValue = SetExecutionCredentials(ExecutionHeaderValue, TrustedUserHeaderValue, Credentials, out var executionInfo);
			return executionInfo;
		}

		public ExecutionInfo SetExecutionCredentials2(DataSourceCredentials[] Credentials)
		{
			ServerInfoHeaderValue = SetExecutionCredentials2(ExecutionHeaderValue, TrustedUserHeaderValue, Credentials, out var executionInfo);
			return executionInfo;
		}

		public ExecutionInfo SetExecutionCredentials3(DataSourceCredentials[] Credentials)
		{
			ServerInfoHeaderValue = SetExecutionCredentials3(ExecutionHeaderValue, TrustedUserHeaderValue, Credentials, out var executionInfo);
			return executionInfo;
		}

		public ExecutionInfo SetExecutionParameters(ParameterValue[] Parameters, string ParameterLanguage)
		{
			ServerInfoHeaderValue = SetExecutionParameters(ExecutionHeaderValue, TrustedUserHeaderValue, Parameters, ParameterLanguage, out var executionInfo);
			return executionInfo;
		}

		public ExecutionInfo SetExecutionParameters2(ParameterValue[] Parameters, string ParameterLanguage)
		{
			ServerInfoHeaderValue = SetExecutionParameters2(ExecutionHeaderValue, TrustedUserHeaderValue, Parameters, ParameterLanguage, out var executionInfo);
			return executionInfo;
		}

		public ExecutionInfo SetExecutionParameters3(ParameterValue[] Parameters, string ParameterLanguage)
		{
			ServerInfoHeaderValue = SetExecutionParameters3(ExecutionHeaderValue, TrustedUserHeaderValue, Parameters, ParameterLanguage, out var executionInfo);
			return executionInfo;
		}

		public ExecutionInfo ResetExecution()
		{
			ServerInfoHeaderValue = ResetExecution(ExecutionHeaderValue, TrustedUserHeaderValue, out var executionInfo);
			return executionInfo;
		}

		public ExecutionInfo ResetExecution2()
		{
			ServerInfoHeaderValue = ResetExecution2(ExecutionHeaderValue, TrustedUserHeaderValue, out var executionInfo);
			return executionInfo;
		}

		public ExecutionInfo ResetExecution3()
		{
			ServerInfoHeaderValue = ResetExecution3(ExecutionHeaderValue, TrustedUserHeaderValue, out var executionInfo);
			return executionInfo;

		}

		public byte[] Render(string Format, string DeviceInfo, out string Extension, out string MimeType, out string Encoding, out Warning[] Warnings, out string[] StreamIds)
		{
			ServerInfoHeaderValue = Render(ExecutionHeaderValue, TrustedUserHeaderValue, Format, DeviceInfo, out var result, out Extension, out MimeType, out Encoding, out Warnings, out StreamIds);
			return result;
		}

		public byte[] Render2(string Format, string DeviceInfo, PageCountMode PaginationMode, out string Extension, out string MimeType, out string Encoding, out Warning[] Warnings, out string[] StreamIds)
		{
			ServerInfoHeaderValue = Render2(ExecutionHeaderValue, TrustedUserHeaderValue, Format, DeviceInfo, PaginationMode, out var result, out Extension, out MimeType, out Encoding, out Warnings, out StreamIds);
			return result;
		}

		public byte[] RenderStream(string Format, string StreamID, string DeviceInfo, out string Encoding, out string MimeType)
		{
			ServerInfoHeaderValue = RenderStream(ExecutionHeaderValue, TrustedUserHeaderValue, Format, StreamID, DeviceInfo, out var result, out Encoding, out MimeType);
			return result;
		}

		public void DeliverReportItem(string Format, string DeviceInfo, ExtensionSettings ExtensionSettings, string Description, string EventType, string MatchData)
		{
			ServerInfoHeaderValue = DeliverReportItem(ExecutionHeaderValue, TrustedUserHeaderValue, Format, DeviceInfo, ExtensionSettings, Description, EventType, MatchData);
		}

		public ExecutionInfo GetExecutionInfo()
		{
			ServerInfoHeaderValue = GetExecutionInfo(ExecutionHeaderValue, TrustedUserHeaderValue, out var executionInfo);
			return executionInfo;
		}

		public ExecutionInfo GetExecutionInfo2()
		{
			ServerInfoHeaderValue = GetExecutionInfo2(ExecutionHeaderValue, TrustedUserHeaderValue, out var executionInfo);
			return executionInfo;
		}

		public ExecutionInfo GetExecutionInfo3()
		{
			ServerInfoHeaderValue = GetExecutionInfo3(ExecutionHeaderValue, TrustedUserHeaderValue, out var executionInfo);
			return executionInfo;
		}

		public DocumentMapNode GetDocumentMap()
		{
			ServerInfoHeaderValue = GetDocumentMap(ExecutionHeaderValue, TrustedUserHeaderValue, out var documentMapNode);
			return documentMapNode;
		}

		public ExecutionInfo LoadDrillthroughTarget(string DrillthroughID)
		{
			var executionHeader = ExecutionHeaderValue;
			ServerInfoHeaderValue = LoadDrillthroughTarget(ref executionHeader, TrustedUserHeaderValue, DrillthroughID, out var executionInfo);
			ExecutionHeaderValue = executionHeader;
			return executionInfo;
		}

		public ExecutionInfo LoadDrillthroughTarget2(string DrillthroughID)
		{
			var executionHeader = ExecutionHeaderValue;
			ServerInfoHeaderValue = LoadDrillthroughTarget2(ref executionHeader, TrustedUserHeaderValue, DrillthroughID, out var executionInfo);
			ExecutionHeaderValue = executionHeader;
			return executionInfo;
		}

		public ExecutionInfo LoadDrillthroughTarget3(string DrillthroughID)
		{
			var executionHeader = ExecutionHeaderValue;
			ServerInfoHeaderValue = LoadDrillthroughTarget3(ref executionHeader, TrustedUserHeaderValue, DrillthroughID, out var executionInfo);
			ExecutionHeaderValue = executionHeader;
			return executionInfo;
		}

		public bool ToggleItem(string ToggleID)
		{
			ServerInfoHeaderValue = ToggleItem(ExecutionHeaderValue, TrustedUserHeaderValue, ToggleID, out var found);
			return found;
		}

		public int NavigateDocumentMap(string DocMapID)
		{
			ServerInfoHeaderValue = NavigateDocumentMap(ExecutionHeaderValue, TrustedUserHeaderValue, DocMapID, out var pageNumber);
			return pageNumber;
		}

		public int NavigateBookmark(string BookmarkID, out string UniqueName)
		{
			ServerInfoHeaderValue = NavigateBookmark(ExecutionHeaderValue, TrustedUserHeaderValue, BookmarkID, out var pageNumber, out UniqueName);
			return pageNumber;
		}

		public int Sort(string SortItem, SortDirectionEnum Direction, bool Clear, out string ReportItem, out int NumPages)
		{
			ServerInfoHeaderValue = Sort(ExecutionHeaderValue, TrustedUserHeaderValue, SortItem, Direction, Clear, out var pageNumber, out ReportItem, out NumPages);
			return pageNumber;
		}

		public int Sort2(string SortItem, SortDirectionEnum Direction, bool Clear, PageCountMode PaginationMode, out string ReportItem, out ExecutionInfo2 ExecutionInfo)
		{
			ServerInfoHeaderValue = Sort2(ExecutionHeaderValue, TrustedUserHeaderValue, SortItem, Direction, Clear, PaginationMode, out var pageNumber, out ReportItem, out ExecutionInfo);
			return pageNumber;
		}

		public int FindString(int startPage, int endPage, string findValue)
		{
			ServerInfoHeaderValue = FindString(ExecutionHeaderValue, TrustedUserHeaderValue, startPage, endPage, findValue, out var pageNumber);
			return pageNumber;
		}

		public byte[] GetRenderResource(string Format, string DeviceInfo, out string MimeType)
		{
			ServerInfoHeaderValue = GetRenderResource(TrustedUserHeaderValue, Format, DeviceInfo, out var result, out MimeType);
			return result;
		}

		public Extension[] ListRenderingExtensions()
		{
			ServerInfoHeaderValue = ListRenderingExtensions(TrustedUserHeaderValue, out var extensions);
			return extensions;
		}
	}
}

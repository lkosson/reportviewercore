using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.Text;

namespace Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution
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
				binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
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
			ListSecureMethods(TrustedUserHeaderValue, out var result);
			return result;
		}

		public ExecutionInfo LoadReport(string Report, string HistoryID)
		{
			throw new NotImplementedException();
		}

		public ExecutionInfo LoadReport2(string Report, string HistoryID)
		{
			throw new NotImplementedException();
		}

		public ExecutionInfo LoadReport3(string Report, string HistoryID)
		{
			throw new NotImplementedException();
		}

		public ExecutionInfo LoadReportDefinition(byte[] Definition, out Warning[] warnings)
		{
			throw new NotImplementedException();
		}

		public ExecutionInfo LoadReportDefinition2(byte[] Definition, out Warning[] warnings)
		{
			throw new NotImplementedException();
		}

		public ExecutionInfo LoadReportDefinition3(byte[] Definition, out Warning[] warnings)
		{
			throw new NotImplementedException();
		}

		public ExecutionInfo SetExecutionCredentials(DataSourceCredentials[] Credentials)
		{
			throw new NotImplementedException();
		}

		public ExecutionInfo SetExecutionCredentials2(DataSourceCredentials[] Credentials)
		{
			throw new NotImplementedException();
		}

		public ExecutionInfo SetExecutionCredentials3(DataSourceCredentials[] Credentials)
		{
			throw new NotImplementedException();
		}

		public ExecutionInfo SetExecutionParameters(ParameterValue[] Parameters, string ParameterLanguage)
		{
			throw new NotImplementedException();
		}

		public ExecutionInfo SetExecutionParameters2(ParameterValue[] Parameters, string ParameterLanguage)
		{
			throw new NotImplementedException();
		}

		public ExecutionInfo SetExecutionParameters3(ParameterValue[] Parameters, string ParameterLanguage)
		{
			throw new NotImplementedException();
		}

		public ExecutionInfo ResetExecution()
		{
			throw new NotImplementedException();
		}

		public ExecutionInfo ResetExecution2()
		{
			throw new NotImplementedException();
		}

		public ExecutionInfo ResetExecution3()
		{
			throw new NotImplementedException();
		}

		public byte[] Render(string Format, string DeviceInfo, out string Extension, out string MimeType, out string Encoding, out Warning[] Warnings, out string[] StreamIds)
		{
			throw new NotImplementedException();
		}

		public byte[] Render2(string Format, string DeviceInfo, PageCountMode PaginationMode, out string Extension, out string MimeType, out string Encoding, out Warning[] Warnings, out string[] StreamIds)
		{
			throw new NotImplementedException();
		}

		public byte[] RenderStream(string Format, string StreamID, string DeviceInfo, out string Encoding, out string MimeType)
		{
			throw new NotImplementedException();
		}

		public void DeliverReportItem(string Format, string DeviceInfo, ExtensionSettings ExtensionSettings, string Description, string EventType, string MatchData)
		{
			throw new NotImplementedException();
		}

		public ExecutionInfo GetExecutionInfo()
		{
			throw new NotImplementedException();
		}

		public ExecutionInfo GetExecutionInfo2()
		{
			throw new NotImplementedException();
		}

		public ExecutionInfo GetExecutionInfo3()
		{
			throw new NotImplementedException();
		}

		public DocumentMapNode GetDocumentMap()
		{
			throw new NotImplementedException();
		}

		public ExecutionInfo LoadDrillthroughTarget(string DrillthroughID)
		{
			throw new NotImplementedException();
		}

		public ExecutionInfo LoadDrillthroughTarget2(string DrillthroughID)
		{
			throw new NotImplementedException();
		}

		public ExecutionInfo LoadDrillthroughTarget3(string DrillthroughID)
		{
			throw new NotImplementedException();
		}

		public bool ToggleItem(string ToggleID)
		{
			throw new NotImplementedException();
		}

		public int NavigateDocumentMap(string DocMapID)
		{
			throw new NotImplementedException();
		}

		public int NavigateBookmark(string BookmarkID, out string UniqueName)
		{
			throw new NotImplementedException();
		}

		public int Sort(string SortItem, SortDirectionEnum Direction, bool Clear, out string ReportItem, out int NumPages)
		{
			throw new NotImplementedException();
		}

		public int Sort2(string SortItem, SortDirectionEnum Direction, bool Clear, PageCountMode PaginationMode, out string ReportItem, out ExecutionInfo2 ExecutionInfo)
		{
			throw new NotImplementedException();
		}

		public int FindString(int startPage, int endPage, string findValue)
		{
			throw new NotImplementedException();
		}

		public byte[] GetRenderResource(string Format, string DeviceInfo, out string MimeType)
		{
			throw new NotImplementedException();
		}

		public Extension[] ListRenderingExtensions()
		{
			throw new NotImplementedException();
		}
	}
}

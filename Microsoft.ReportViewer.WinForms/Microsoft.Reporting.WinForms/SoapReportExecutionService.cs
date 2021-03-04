using Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Xml;

namespace Microsoft.Reporting.WinForms
{
	internal class SoapReportExecutionService : IReportExecutionService
	{
		private sealed class ServerReportSoapProxy : RSExecutionConnection
		{
			private System.Security.Principal.WindowsIdentity m_impersonationUser;

			private IEnumerable<string> m_headers;

			private IEnumerable<Cookie> m_cookies;

			public Cookie FormsAuthCookie;

			public string BearerToken
			{
				get;
				set;
			}

			public ServerReportSoapProxy(System.Security.Principal.WindowsIdentity impersonationUser, string reportServerLocation, IEnumerable<string> headers, IEnumerable<Cookie> cookies, EndpointVersion version)
				: base(reportServerLocation, version)
			{
				m_impersonationUser = impersonationUser;
				m_headers = headers;
				m_cookies = cookies;
			}
/*
			protected override WebRequest GetWebRequest(Uri uri)
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)base.GetWebRequest(uri);
				if (!string.IsNullOrEmpty(BearerToken))
				{
					httpWebRequest.Headers.Add("Authorization", $"Bearer {BearerToken}");
				}
				WebRequestHelper.SetRequestHeaders(httpWebRequest, FormsAuthCookie, m_headers, m_cookies);
				return httpWebRequest;
			}

			protected override WebResponse GetWebResponse(WebRequest request)
			{
				using (new ServerImpersonationContext(m_impersonationUser))
				{
					HttpWebResponse httpWebResponse = (HttpWebResponse)base.GetWebResponse(request);
					string text = httpWebResponse.Headers["RSAuthenticationHeader"];
					if (text != null)
					{
						Cookie cookie = httpWebResponse.Cookies[text];
						if (cookie != null)
						{
							FormsAuthCookie = cookie;
						}
					}
					return httpWebResponse;
				}
			}
*/
			protected override void OnSoapException(FaultException e)
			{
				SoapVersionMismatchException.ThrowIfVersionMismatch(e, "ReportExecution2005.asmx", CommonStrings.UnsupportedReportServerError, includeInnerException: false);
				base.OnSoapException(e);
				throw ReportServerException.FromException(e);
			}
		}

		private System.Security.Principal.WindowsIdentity m_impersonationUser;

		private Uri m_reportServerUrl;

		private IReportServerCredentials m_reportServerCredentials;

		private TrustedUserHeader m_trustedUserHeader;

		private IEnumerable<string> m_headers;

		private IEnumerable<Cookie> m_cookies;

		private int m_timeout;

		private ServerReportSoapProxy m_service;

		private const EndpointVersion EndpointVersion = default;

		private const int BufferedReadSize = 81920;

		private ICredentials ServerNetworkCredentials
		{
			get
			{
				if (m_reportServerCredentials != null)
				{
					ICredentials networkCredentials = m_reportServerCredentials.NetworkCredentials;
					if (networkCredentials != null)
					{
						return networkCredentials;
					}
				}
				return DefaultCredentials;
			}
		}

		private ICredentials DefaultCredentials
		{
			[EnvironmentPermission(SecurityAction.Assert, Read = "USERNAME")]
			get
			{
				return CredentialCache.DefaultCredentials;
			}
		}

		public string BearerToken
		{
			get;
			set;
		}

		private ServerReportSoapProxy Service
		{
			get
			{
				if (m_service == null)
				{
					using (MonitoredScope.New("SoapReportExecutionService.Service - proxy creation"))
					{
						ServerReportSoapProxy serverReportSoapProxy = new ServerReportSoapProxy(m_impersonationUser, m_reportServerUrl.ToString(), m_headers, m_cookies, EndpointVersion.Automatic);
						serverReportSoapProxy.Credentials = ServerNetworkCredentials;
						serverReportSoapProxy.Timeout = m_timeout;
						if (m_trustedUserHeader != null)
						{
							serverReportSoapProxy.TrustedUserHeaderValue = m_trustedUserHeader;
						}
						if (m_reportServerCredentials != null && m_reportServerCredentials.GetFormsCredentials(out Cookie authCookie, out string userName, out string password, out string authority))
						{
							if (authCookie != null)
							{
								serverReportSoapProxy.FormsAuthCookie = authCookie;
							}
							else
							{
								serverReportSoapProxy.LogonUser(userName, password, authority);
							}
						}
						if (!string.IsNullOrEmpty(BearerToken))
						{
							serverReportSoapProxy.BearerToken = BearerToken;
						}
						m_service = serverReportSoapProxy;
					}
				}
				return m_service;
			}
		}

		public int Timeout
		{
			set
			{
				m_timeout = value;
				if (m_service != null)
				{
					m_service.Timeout = value;
				}
			}
		}

		private int ServerMajorVersion
		{
			get
			{
				int result = 0;
				string serverVersion = GetServerVersion();
				if (!string.IsNullOrEmpty(serverVersion))
				{
					int num = serverVersion.IndexOf(".", StringComparison.Ordinal);
					if (num > 0 && !int.TryParse(serverVersion.Substring(0, num), NumberStyles.None, CultureInfo.InvariantCulture, out result))
					{
						result = 0;
					}
				}
				return result;
			}
		}

		public SoapReportExecutionService(System.Security.Principal.WindowsIdentity impersonationUser, Uri reportServerUrl, IReportServerCredentials reportServerCredentials, TrustedUserHeader trustedUserHeader, IEnumerable<string> headers, IEnumerable<Cookie> cookies, int timeout)
		{
			m_impersonationUser = impersonationUser;
			m_reportServerUrl = reportServerUrl;
			m_reportServerCredentials = reportServerCredentials;
			m_trustedUserHeader = trustedUserHeader;
			m_headers = headers;
			m_cookies = cookies;
			m_timeout = timeout;
		}

		public ExecutionInfo GetExecutionInfo()
		{
			return FromSoapExecutionInfo(Service.GetExecutionInfo());
		}

		public ExecutionInfo ResetExecution()
		{
			return FromSoapExecutionInfo(Service.ResetExecution());
		}

		public ExecutionInfo LoadReport(string report, string historyId)
		{
			return FromSoapExecutionInfo(Service.LoadReport(report, historyId));
		}

		public ExecutionInfo LoadReportDefinition(byte[] definition)
		{
			Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution.Warning[] warnings;
			return FromSoapExecutionInfo(Service.LoadReportDefinition(definition, out warnings));
		}

		public DocumentMapNode GetDocumentMap(string rootLabel)
		{
			return DocumentMapNode.CreateTree(Service.GetDocumentMap(), rootLabel);
		}

		public RenderingExtension[] ListRenderingExtensions()
		{
			return RenderingExtension.FromSoapExtensions(Service.ListRenderingExtensions());
		}

		public ExecutionInfo SetExecutionCredentials(IEnumerable<DataSourceCredentials> credentials)
		{
			List<Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution.DataSourceCredentials> list = new List<Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution.DataSourceCredentials>();
			foreach (DataSourceCredentials credential in credentials)
			{
				if (credential == null)
				{
					throw new ArgumentNullException("credentials");
				}
				list.Add(credential.ToSoapCredentials());
			}
			return FromSoapExecutionInfo(Service.SetExecutionCredentials(list.ToArray()));
		}

		public ExecutionInfo SetExecutionParameters(IEnumerable<ReportParameter> parameters, string parameterLanguage)
		{
			List<ParameterValue> list = new List<ParameterValue>();
			foreach (ReportParameter parameter in parameters)
			{
				StringEnumerator enumerator2 = parameter.Values.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						string current2 = enumerator2.Current;
						ParameterValue parameterValue = new ParameterValue();
						parameterValue.Name = parameter.Name;
						parameterValue.Value = current2;
						list.Add(parameterValue);
					}
				}
				finally
				{
					(enumerator2 as IDisposable)?.Dispose();
				}
			}
			return FromSoapExecutionInfo(Service.SetExecutionParameters(list.ToArray(), parameterLanguage));
		}

		public byte[] Render(string format, string deviceInfo, PageCountMode paginationMode, out string extension, out string mimeType, out string encoding, out Warning[] warnings, out string[] streamIds)
		{
			Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution.PageCountMode paginationMode2 = SoapPageCountFromViewerAPI(paginationMode);
			Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution.Warning[] Warnings;
			byte[] result = Service.Render(format, deviceInfo, paginationMode2, out extension, out mimeType, out encoding, out Warnings, out streamIds);
			warnings = Warning.FromSoapWarnings(Warnings);
			return result;
		}

		public void Render(AbortState abortState, string reportPath, string executionId, string historyId, string format, XmlNodeList deviceInfo, NameValueCollection urlAccessParameters, Stream reportStream, out string mimeType, out string fileNameExtension)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}?{1}&rs:SessionID={2}&rs:command=Render&rs:Format={3}", Service.UrlForRender, UrlUtil.UrlEncode(reportPath), UrlUtil.UrlEncode(executionId), UrlUtil.UrlEncode(format));
			if (!string.IsNullOrEmpty(historyId))
			{
				stringBuilder.Append("&rs:snapshot=");
				stringBuilder.Append(UrlUtil.UrlEncode(historyId));
			}
			if (deviceInfo != null)
			{
				foreach (XmlNode item in deviceInfo)
				{
					stringBuilder.Append("&rc:");
					stringBuilder.Append(UrlUtil.UrlEncode(item.Name));
					stringBuilder.Append("=");
					stringBuilder.Append(UrlUtil.UrlEncode(item.InnerText));
				}
			}
			stringBuilder.Append("&rc:Toolbar=false&rs:ErrorResponseAsXml=true&rs:AllowNewSessions=false");
			if (urlAccessParameters != null)
			{
				foreach (string key in urlAccessParameters.Keys)
				{
					stringBuilder.Append("&");
					stringBuilder.Append(UrlUtil.UrlEncode(key));
					stringBuilder.Append("=");
					stringBuilder.Append(UrlUtil.UrlEncode(urlAccessParameters[key]));
				}
			}
			ServerUrlRequest(abortState, stringBuilder.ToString(), reportStream, out mimeType, out fileNameExtension);
		}

		public byte[] RenderStream(string format, string streamId, string deviceInfo, out string encoding, out string mimeType)
		{
			return Service.RenderStream(format, streamId, deviceInfo, out encoding, out mimeType);
		}

		public void DeliverReportItem(string format, string deviceInfo, ExtensionSettings extensionSettings, string description, string eventType, string matchData)
		{
			Service.DeliverReportItem(format, deviceInfo, extensionSettings.ConvertSettings(), description, eventType, matchData);
		}

		public int FindString(int startPage, int endPage, string findValue)
		{
			return Service.FindString(startPage, endPage, findValue);
		}

		public void ToggleItem(string toggleId)
		{
			Service.ToggleItem(toggleId);
		}

		public int NavigateBookmark(string bookmarkId, out string uniqueName)
		{
			return Service.NavigateBookmark(bookmarkId, out uniqueName);
		}

		public int NavigateDocumentMap(string documentMapId)
		{
			return Service.NavigateDocumentMap(documentMapId);
		}

		public ExecutionInfo LoadDrillthroughTarget(string drillthroughId)
		{
			return FromSoapExecutionInfo(Service.LoadDrillthroughTarget(drillthroughId));
		}

		public int Sort(string sortItem, SortOrder direction, bool clear, PageCountMode paginationMode, out string reportItem, out ExecutionInfo executionInfo, out int numPages)
		{
			Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution.PageCountMode paginationMode2 = SoapPageCountFromViewerAPI(paginationMode);
			SortDirectionEnum direction2 = (direction == SortOrder.Ascending) ? SortDirectionEnum.Ascending : SortDirectionEnum.Descending;
			Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution.ExecutionInfo ExecutionInfo;
			int result = Service.Sort(sortItem, direction2, clear, paginationMode2, out reportItem, out ExecutionInfo, out numPages);
			executionInfo = FromSoapExecutionInfo(ExecutionInfo);
			return result;
		}

		public void SetExecutionId(string executionId)
		{
			if (executionId != null)
			{
				Service.ExecutionHeaderValue = new ExecutionHeader();
				Service.ExecutionHeaderValue.ExecutionID = executionId;
			}
			else
			{
				Service.ExecutionHeaderValue = null;
			}
		}

		public string GetServerVersion()
		{
			if (Service.ServerInfoHeaderValue == null)
			{
				Service.ValidateConnection();
			}
			return Service.ServerInfoHeaderValue.ReportServerVersionNumber;
		}

		private static ExecutionInfo FromSoapExecutionInfo(Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution.ExecutionInfo soapExecutionInfo)
		{
			if (soapExecutionInfo == null)
			{
				return null;
			}
			ReportParameterInfoCollection reportParameterInfoCollection = null;
			Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution.ReportParameter[] parameters = soapExecutionInfo.Parameters;
			if (parameters != null)
			{
				ReportParameterInfo[] array = new ReportParameterInfo[parameters.Length];
				for (int i = 0; i < parameters.Length; i++)
				{
					array[i] = SoapParameterToReportParameterInfo(parameters[i]);
				}
				reportParameterInfoCollection = new ReportParameterInfoCollection(array);
			}
			else
			{
				reportParameterInfoCollection = new ReportParameterInfoCollection();
			}
			PageCountMode pageCountMode = PageCountMode.Actual;
			ExecutionInfo2 executionInfo = soapExecutionInfo as ExecutionInfo2;
			if (executionInfo != null && executionInfo.PageCountMode == Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution.PageCountMode.Estimate)
			{
				pageCountMode = PageCountMode.Estimate;
			}
			ParametersPaneLayout parametersPaneLayout = null;
			ExecutionInfo3 executionInfo2 = soapExecutionInfo as ExecutionInfo3;
			if (executionInfo2 != null && executionInfo2.ParametersLayout != null && parameters != null)
			{
				parametersPaneLayout = new ParametersPaneLayout();
				SoapExecutionInfoToParametersLayout(parametersPaneLayout, executionInfo2, reportParameterInfoCollection);
			}
			ReportPageSettings pageSettings = new ReportPageSettings(soapExecutionInfo.ReportPageSettings.PaperSize.Height, soapExecutionInfo.ReportPageSettings.PaperSize.Width, soapExecutionInfo.ReportPageSettings.Margins.Left, soapExecutionInfo.ReportPageSettings.Margins.Right, soapExecutionInfo.ReportPageSettings.Margins.Top, soapExecutionInfo.ReportPageSettings.Margins.Bottom);
			return new ExecutionInfo(soapExecutionInfo.ExecutionID, soapExecutionInfo.HistoryID, soapExecutionInfo.ReportPath, soapExecutionInfo.NumPages, soapExecutionInfo.HasDocumentMap, soapExecutionInfo.AutoRefreshInterval, soapExecutionInfo.CredentialsRequired, soapExecutionInfo.ParametersRequired, soapExecutionInfo.HasSnapshot, soapExecutionInfo.NeedsProcessing, soapExecutionInfo.ExpirationDateTime, soapExecutionInfo.AllowQueryExecution, pageCountMode, ReportDataSourceInfoCollection.FromSoapDataSourcePrompts(soapExecutionInfo.DataSourcePrompts), reportParameterInfoCollection, pageSettings, parametersPaneLayout);
		}

		private static void SoapExecutionInfoToParametersLayout(ParametersPaneLayout paramPaneLayout, ExecutionInfo3 soapExecInfo3, ReportParameterInfoCollection paramInfoCollection)
		{
			if (soapExecInfo3.ParametersLayout.CellDefinitions != null)
			{
				int num = soapExecInfo3.ParametersLayout.CellDefinitions.Length;
				GridLayoutCellDefinition[] array = new GridLayoutCellDefinition[num];
				for (int i = 0; i < num; i++)
				{
					GridLayoutCellDefinition gridLayoutCellDefinition = new GridLayoutCellDefinition();
					gridLayoutCellDefinition.Column = soapExecInfo3.ParametersLayout.CellDefinitions[i].ColumnsIndex;
					gridLayoutCellDefinition.Row = soapExecInfo3.ParametersLayout.CellDefinitions[i].RowIndex;
					gridLayoutCellDefinition.ParameterName = soapExecInfo3.ParametersLayout.CellDefinitions[i].ParameterName;
					array[i] = gridLayoutCellDefinition;
				}
				GridLayoutDefinition gridLayoutDefinition = paramPaneLayout.GridLayoutDefinition = new GridLayoutDefinition(new GridLayoutCellDefinitionCollection(array), soapExecInfo3.ParametersLayout.NumberOfRows, soapExecInfo3.ParametersLayout.NumberOfColumns, paramInfoCollection);
			}
		}

		private static ReportParameterInfo SoapParameterToReportParameterInfo(Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution.ReportParameter soapParam)
		{
			string[] array = null;
			if (soapParam.DefaultValues != null)
			{
				array = new string[soapParam.DefaultValues.Length];
				for (int i = 0; i < soapParam.DefaultValues.Length; i++)
				{
					array[i] = soapParam.DefaultValues[i];
				}
			}
			List<ValidValue> list = null;
			if (soapParam.ValidValues != null)
			{
				list = new List<ValidValue>(soapParam.ValidValues.Length);
				Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution.ValidValue[] validValues = soapParam.ValidValues;
				foreach (Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution.ValidValue validValue in validValues)
				{
					list.Add(new ValidValue(validValue.Label, validValue.Value));
				}
			}
			return new ReportParameterInfo(soapParam.Name, (ParameterDataType)Enum.Parse(typeof(ParameterDataType), soapParam.Type.ToString()), soapParam.Nullable, soapParam.AllowBlank, soapParam.MultiValue, soapParam.QueryParameterSpecified && soapParam.QueryParameter, soapParam.Prompt, soapParam.PromptUser, soapParam.DefaultValuesQueryBasedSpecified && soapParam.DefaultValuesQueryBased, soapParam.ValidValuesQueryBasedSpecified && soapParam.ValidValuesQueryBased, null, array, list, soapParam.Dependencies, (ParameterState)Enum.Parse(typeof(ParameterState), soapParam.State.ToString()));
		}

		private static Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution.PageCountMode SoapPageCountFromViewerAPI(PageCountMode pageCountMode)
		{
			if (pageCountMode == PageCountMode.Actual)
			{
				return Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution.PageCountMode.Actual;
			}
			return Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution.PageCountMode.Estimate;
		}

		private void ServerUrlRequest(AbortState abortState, string url, Stream outputStream, out string mimeType, out string fileNameExtension)
		{
			byte[] userToken = null;
			string userName = string.Empty;
			if (m_trustedUserHeader != null)
			{
				userName = m_trustedUserHeader.UserName;
				userToken = m_trustedUserHeader.UserToken;
			}
			HttpWebRequest serverUrlAccessObject = WebRequestHelper.GetServerUrlAccessObject(url, m_timeout, ServerNetworkCredentials, Service.FormsAuthCookie, m_headers, m_cookies, userName, BearerToken, userToken);
			if (abortState != null && !abortState.RegisterAbortableRequest(serverUrlAccessObject))
			{
				throw new OperationCanceledException();
			}
			try
			{
				using (new ServerImpersonationContext(m_impersonationUser))
				{
					WebResponse response = serverUrlAccessObject.GetResponse();
					mimeType = response.Headers["Content-Type"];
					fileNameExtension = response.Headers["FileExtension"];
					Stream responseStream = response.GetResponseStream();
					if (responseStream == null)
					{
						return;
					}
					using (responseStream)
					{
						byte[] array = new byte[81920];
						int count;
						while ((count = responseStream.Read(array, 0, array.Length)) > 0)
						{
							outputStream.Write(array, 0, count);
						}
					}
				}
			}
			catch (Exception e)
			{
				throw WebRequestHelper.ExceptionFromWebResponse(e);
			}
		}
	}
}

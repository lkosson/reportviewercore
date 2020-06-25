using Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Xml;

namespace Microsoft.Reporting.WinForms
{
	[Serializable]
	public sealed class ServerReport : Report, ISerializable
	{
		private const string ParamServerSession = "ReportSession";

		private Uri m_serverUrl = new Uri("http://localhost/reportserver");

		private IReportServerCredentials m_serverCredentials;

		private ReportServerCredentials m_serverCredentialsImpl;

		private System.Security.Principal.WindowsIdentity m_serverIdentity;

		private ReportViewerHeaderCollection m_headers;

		private ReportViewerCookieCollection m_cookies;

		private string m_reportPath = string.Empty;

		private string m_historyID = string.Empty;

		private string m_executionID;

		private int m_timeOut = 600000;

		private List<int> m_hiddenParameters = new List<int>();

		private IReportExecutionService m_service;

		private ExecutionInfo m_executionInfo;

		private TrustedUserHeader m_trustedUserHeader;

		private RenderingExtension[] m_renderingExtensions;

		private AbortState m_abortState = new AbortState();

		internal TrustedUserHeader TrustedUserHeaderValue
		{
			get
			{
				return m_trustedUserHeader;
			}
			set
			{
				m_trustedUserHeader = value;
				m_service = null;
			}
		}

		[SRDescription("ReportServerUrlDesc")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Uri), "http://localhost/reportserver")]
		public Uri ReportServerUrl
		{
			get
			{
				return m_serverUrl;
			}
			set
			{
				lock (m_syncObject)
				{
					if (value == null)
					{
						throw new ArgumentNullException("value");
					}
					if (base.IsDrillthroughReport)
					{
						throw new InvalidOperationException();
					}
					if (Uri.Compare(m_serverUrl, value, UriComponents.AbsoluteUri, UriFormat.UriEscaped, StringComparison.Ordinal) != 0)
					{
						m_serverUrl = value;
						m_service = null;
						ClearServerSpecificInfo();
					}
				}
			}
		}

		public string BearerToken
		{
			get;
			set;
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ReportServerCredentials ReportServerCredentials => m_serverCredentialsImpl;

		[SRDescription("ServerTimeoutDesc")]
		[NotifyParentProperty(true)]
		[DefaultValue(600000)]
		public int Timeout
		{
			get
			{
				return m_timeOut;
			}
			set
			{
				lock (m_syncObject)
				{
					if (base.IsDrillthroughReport)
					{
						throw new InvalidOperationException();
					}
					m_timeOut = value;
					if (m_service != null)
					{
						m_service.Timeout = value;
					}
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ReportViewerHeaderCollection Headers => m_headers;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ReportViewerCookieCollection Cookies => m_cookies;

		[SRDescription("ServerReportPathDesc")]
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		public string ReportPath
		{
			get
			{
				return m_reportPath;
			}
			set
			{
				lock (m_syncObject)
				{
					if (value == null)
					{
						throw new ArgumentNullException("value");
					}
					if (base.IsDrillthroughReport)
					{
						throw new InvalidOperationException();
					}
					if (string.Compare(m_reportPath, value, StringComparison.OrdinalIgnoreCase) != 0)
					{
						ClearSession();
						m_reportPath = value;
					}
				}
			}
		}

		[SRDescription("HistoryIdDesc")]
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		public string HistoryId
		{
			get
			{
				return m_historyID;
			}
			set
			{
				lock (m_syncObject)
				{
					if (base.IsDrillthroughReport)
					{
						throw new InvalidOperationException();
					}
					m_historyID = value;
					Refresh();
				}
			}
		}

		internal override string DisplayNameForUse
		{
			get
			{
				lock (m_syncObject)
				{
					if (string.IsNullOrEmpty(base.DisplayName))
					{
						string text = RetrieveReportNameFromPath(ReportPath);
						if (string.IsNullOrEmpty(text))
						{
							text = CommonStrings.Report;
						}
						return text;
					}
					return base.DisplayName;
				}
			}
		}

		internal bool HasExecutionId
		{
			get
			{
				lock (m_syncObject)
				{
					return !string.IsNullOrEmpty(m_executionID);
				}
			}
		}

		internal override bool CanSelfCancel => true;

		private PageCountMode PageCountMode
		{
			get
			{
				lock (m_syncObject)
				{
					if (m_executionInfo == null)
					{
						return PageCountMode.Estimate;
					}
					if (m_executionInfo.NumPages == 0)
					{
						return PageCountMode.Estimate;
					}
					return m_executionInfo.PageCountMode;
				}
			}
		}

		internal override bool HasDocMap
		{
			get
			{
				lock (m_syncObject)
				{
					if (m_executionInfo == null)
					{
						return false;
					}
					return m_executionInfo.HasDocumentMap;
				}
			}
		}

		internal override int AutoRefreshInterval
		{
			get
			{
				lock (m_syncObject)
				{
					EnsureExecutionSession();
					return m_executionInfo.AutoRefreshInterval;
				}
			}
		}

		internal override bool IsReadyForConnection
		{
			get
			{
				lock (m_syncObject)
				{
					return !string.IsNullOrEmpty(m_reportPath) || m_executionID != null;
				}
			}
		}

		internal override bool IsPreparedReportReadyForRendering
		{
			get
			{
				if (!m_executionInfo.CredentialsRequired)
				{
					return !m_executionInfo.ParametersRequired;
				}
				return false;
			}
		}

		private bool IsReadyForProcessingPostTasks
		{
			get
			{
				EnsureExecutionSession();
				if (m_executionInfo.HasSnapshot)
				{
					return !m_executionInfo.NeedsProcessing;
				}
				return false;
			}
		}

		private IReportExecutionService Service
		{
			get
			{
				if (m_service == null)
				{
					m_service = CreateExecutionService();
					m_service.BearerToken = BearerToken;
					ApplyExecutionIdToService(m_service);
				}
				return m_service;
			}
		}

		internal event EventHandler ExecutionIDChanged;

		public ServerReport()
		{
			m_headers = new ReportViewerHeaderCollection(m_syncObject);
			m_cookies = new ReportViewerCookieCollection(m_syncObject);
			m_serverCredentialsImpl = new ReportServerCredentials(m_syncObject);
			m_serverCredentialsImpl.Change += delegate
			{
				OnCredentialsChanged(m_serverCredentials);
			};
			m_serverCredentials = m_serverCredentialsImpl;
		}

		internal ServerReport(ServerReport original)
			: this()
		{
			ReportServerUrl = new Uri(original.ReportServerUrl.ToString());
			Timeout = original.Timeout;
			foreach (string header in original.Headers)
			{
				Headers.Add(header);
			}
			foreach (Cookie cooky in original.Cookies)
			{
				Cookies.Add(cooky);
			}
			ReportServerCredentials.CopyFrom(original.ReportServerCredentials);
		}

		private ServerReport(ServerReport parentReport, ExecutionInfo executionInfo)
			: this(parentReport)
		{
			m_reportPath = executionInfo.ReportPath;
			m_executionID = executionInfo.ExecutionID;
			m_executionInfo = executionInfo;
			m_trustedUserHeader = parentReport.TrustedUserHeaderValue;
			base.DrillthroughDepth = parentReport.DrillthroughDepth + 1;
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		internal ServerReport(SerializationInfo info, StreamingContext context)
			: this()
		{
			m_serverUrl = (Uri)info.GetValue("ServerURL", typeof(Uri));
			m_timeOut = info.GetInt32("TimeOut");
			m_headers = (ReportViewerHeaderCollection)info.GetValue("Headers", typeof(ReportViewerHeaderCollection));
			m_cookies = (ReportViewerCookieCollection)info.GetValue("Cookies", typeof(ReportViewerCookieCollection));
			m_headers.SetSyncObject(m_syncObject);
			m_cookies.SetSyncObject(m_syncObject);
			IReportServerCredentials credentials = (IReportServerCredentials)info.GetValue("Credentials", typeof(IReportServerCredentials));
			OnCredentialsChanged(credentials);
			object value = info.GetValue("ViewStateValues", typeof(object[]));
			LoadViewState(value);
		}

		[SecurityCritical]
		[SecurityTreatAsSafe]
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			lock (m_syncObject)
			{
				info.AddValue("ServerURL", m_serverUrl);
				info.AddValue("TimeOut", m_timeOut);
				info.AddValue("Credentials", m_serverCredentials, typeof(IReportServerCredentials));
				info.AddValue("Headers", m_headers);
				info.AddValue("Cookies", m_cookies);
				info.AddValue("ViewStateValues", SaveViewState());
			}
		}

		private void OnExecutionIDChanged()
		{
			if (this.ExecutionIDChanged != null)
			{
				this.ExecutionIDChanged(this, EventArgs.Empty);
			}
		}

		internal object SaveViewState()
		{
			lock (m_syncObject)
			{
				return new object[4]
				{
					m_executionID,
					m_hiddenParameters.ToArray(),
					base.DisplayName,
					base.DrillthroughDepth
				};
			}
		}

		internal void LoadViewState(object viewStateObj)
		{
			lock (m_syncObject)
			{
				object[] array = (object[])viewStateObj;
				m_executionID = (string)array[0];
				m_hiddenParameters.AddRange((int[])array[1]);
				base.DisplayName = (string)array[2];
				base.DrillthroughDepth = (int)array[3];
				if (m_executionID != null)
				{
					EnsureExecutionSession();
					m_historyID = m_executionInfo.HistoryID;
					if (m_executionInfo.ReportPath == null)
					{
						m_reportPath = "";
					}
					else
					{
						m_reportPath = m_executionInfo.ReportPath;
					}
				}
				OnExecutionIDChanged();
			}
		}

		private void OnCredentialsChanged(IReportServerCredentials credentials)
		{
			if (credentials != null)
			{
				m_serverIdentity = credentials.ImpersonationUser;
			}
			else
			{
				m_serverIdentity = null;
			}
			m_serverCredentials = credentials;
			m_service = null;
			ClearSession();
		}

		private static string RetrieveReportNameFromPath(string reportPath)
		{
			string result = null;
			if (!string.IsNullOrEmpty(reportPath) && reportPath.IndexOfAny(Path.GetInvalidPathChars()) < 0)
			{
				try
				{
					result = Path.GetFileName(reportPath);
					return result;
				}
				catch (ArgumentException)
				{
					return result;
				}
			}
			return result;
		}

		internal DateTime GetExecutionSessionExpiration()
		{
			lock (m_syncObject)
			{
				EnsureExecutionSession();
				return m_executionInfo.ExpirationDateTime;
			}
		}

		public bool IsQueryExecutionAllowed()
		{
			lock (m_syncObject)
			{
				EnsureExecutionSession();
				return m_executionInfo.AllowQueryExecution;
			}
		}

		public override ReportParameterInfoCollection GetParameters()
		{
			lock (m_syncObject)
			{
				EnsureExecutionSession();
				ReportParameterInfoCollection parameters = m_executionInfo.Parameters;
				if (parameters != null)
				{
					for (int i = 0; i < parameters.Count; i++)
					{
						parameters[i].Visible = !m_hiddenParameters.Contains(i);
					}
				}
				return parameters;
			}
		}

		internal override ParametersPaneLayout GetParametersPaneLayout()
		{
			lock (m_syncObject)
			{
				EnsureExecutionSession();
				return m_executionInfo.ParametersPaneLayout;
			}
		}

		public override void SetParameters(IEnumerable<ReportParameter> parameters)
		{
			lock (m_syncObject)
			{
				if (parameters == null)
				{
					throw new ArgumentNullException("parameters");
				}
				EnsureExecutionSession();
				Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
				foreach (ReportParameter parameter in parameters)
				{
					if (parameter == null || parameter.Name == null)
					{
						throw new ArgumentNullException("parameters");
					}
					int indexForParameter = GetIndexForParameter(parameter.Name);
					if (dictionary.ContainsKey(indexForParameter))
					{
						throw new ArgumentException(CommonStrings.ParameterSpecifiedMultipleTimes(parameter.Name));
					}
					dictionary.Add(indexForParameter, parameter.Visible);
				}
				m_executionInfo = Service.SetExecutionParameters(parameters, Thread.CurrentThread.CurrentCulture.Name);
				foreach (int key in dictionary.Keys)
				{
					if (dictionary[key])
					{
						m_hiddenParameters.Remove(key);
					}
					else if (!m_hiddenParameters.Contains(key))
					{
						m_hiddenParameters.Add(key);
					}
				}
				OnChange(isRefreshOnly: false);
			}
		}

		public override ReportPageSettings GetDefaultPageSettings()
		{
			lock (m_syncObject)
			{
				EnsureExecutionSession();
				return m_executionInfo.ReportPageSettings;
			}
		}

		public void SetDataSourceCredentials(IEnumerable<DataSourceCredentials> credentials)
		{
			lock (m_syncObject)
			{
				if (credentials == null)
				{
					throw new ArgumentNullException("credentials");
				}
				EnsureExecutionSession();
				m_executionInfo = Service.SetExecutionCredentials(credentials);
				OnChange(isRefreshOnly: false);
			}
		}

		public void SetExecutionId(string executionId)
		{
			SetExecutionId(executionId, fullReportLoad: true);
		}

		internal void SetExecutionId(string executionId, bool fullReportLoad)
		{
			lock (m_syncObject)
			{
				if (executionId == null)
				{
					throw new ArgumentNullException("executionId");
				}
				if (executionId.Length == 0)
				{
					throw new ArgumentOutOfRangeException("executionId");
				}
				if (base.IsDrillthroughReport)
				{
					throw new InvalidOperationException();
				}
				ClearSession(doRefresh: false);
				m_executionID = executionId;
				ApplyExecutionIdToService(m_service);
				OnExecutionIDChanged();
				if (fullReportLoad)
				{
					EnsureExecutionSession();
					m_reportPath = m_executionInfo.ReportPath;
					m_historyID = m_executionInfo.HistoryID;
				}
				OnChange(isRefreshOnly: false);
			}
		}

		public string GetExecutionId()
		{
			lock (m_syncObject)
			{
				EnsureExecutionSession();
				return m_executionID;
			}
		}

		internal override void SetCancelState(bool shouldCancel)
		{
			if (shouldCancel)
			{
				m_abortState.AbortRequest();
			}
			else
			{
				m_abortState.ClearPendingAbort();
			}
		}

		public Stream Render(string format, string deviceInfo, NameValueCollection urlAccessParameters, out string mimeType, out string fileNameExtension)
		{
			lock (m_syncObject)
			{
				MemoryStream memoryStream = new MemoryStream();
				Render(format, deviceInfo, urlAccessParameters, memoryStream, out mimeType, out fileNameExtension);
				memoryStream.Position = 0L;
				return memoryStream;
			}
		}

		public void Render(string format, string deviceInfo, NameValueCollection urlAccessParameters, Stream reportStream, out string mimeType, out string fileNameExtension)
		{
			InternalRender(isAbortable: false, format, deviceInfo, urlAccessParameters, reportStream, out mimeType, out fileNameExtension);
		}

		internal void InternalRender(bool isAbortable, string format, string deviceInfo, NameValueCollection urlAccessParameters, Stream reportStream, out string mimeType, out string fileNameExtension)
		{
			lock (m_syncObject)
			{
				EnsureExecutionSession();
				XmlNodeList deviceInfo2 = null;
				if (!string.IsNullOrEmpty(deviceInfo))
				{
					XmlDocument xmlDocument = new XmlDocument();
					XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
					xmlReaderSettings.CheckCharacters = false;
					XmlReader reader = XmlReader.Create(new StringReader(deviceInfo), xmlReaderSettings);
					xmlDocument.Load(reader);
					if (xmlDocument.DocumentElement != null && xmlDocument.DocumentElement.ChildNodes != null)
					{
						deviceInfo2 = xmlDocument.DocumentElement.ChildNodes;
					}
				}
				Service.Render(isAbortable ? m_abortState : null, ReportPath, m_executionID, HistoryId, format, deviceInfo2, urlAccessParameters, reportStream, out mimeType, out fileNameExtension);
				UpdatedExecutionInfoIfNecessary();
			}
		}

		public override byte[] Render(string format, string deviceInfo, PageCountMode pageCountMode, out string mimeType, out string encoding, out string fileNameExtension, out string[] streams, out Warning[] warnings)
		{
			lock (m_syncObject)
			{
				EnsureExecutionSession();
				byte[] result = Service.Render(format, deviceInfo, pageCountMode, out fileNameExtension, out mimeType, out encoding, out warnings, out streams);
				UpdatedExecutionInfoIfNecessary();
				return result;
			}
		}

		private void UpdatedExecutionInfoIfNecessary()
		{
			if (!m_executionInfo.HasSnapshot || m_executionInfo.NeedsProcessing || PageCountMode != 0)
			{
				m_executionInfo = Service.GetExecutionInfo();
			}
		}

		public byte[] RenderStream(string format, string streamId, string deviceInfo, out string mimeType, out string encoding)
		{
			lock (m_syncObject)
			{
				if (!PrepareForRender())
				{
					throw new InvalidOperationException(CommonStrings.ReportNotReady);
				}
				return InternalRenderStream(format, streamId, deviceInfo, out mimeType, out encoding);
			}
		}

		internal override byte[] InternalRenderStream(string format, string streamId, string deviceInfo, out string mimeType, out string encoding)
		{
			lock (m_syncObject)
			{
				return Service.RenderStream(format, streamId, deviceInfo, out encoding, out mimeType);
			}
		}

		internal void DeliverReportItem(string format, string deviceInfo, ExtensionSettings extensionSettings, string description, string eventType, string matchData)
		{
			lock (m_syncObject)
			{
				if (!PrepareForRender())
				{
					throw new InvalidOperationException(CommonStrings.ReportNotReady);
				}
				InternalDeliverReportItem(format, deviceInfo, extensionSettings, description, eventType, matchData);
			}
		}

		internal override void InternalDeliverReportItem(string format, string deviceInfo, ExtensionSettings extensionSettings, string description, string eventType, string matchData)
		{
			lock (m_syncObject)
			{
				Service.DeliverReportItem(format, deviceInfo, extensionSettings, description, eventType, matchData);
			}
		}

		public override void LoadReportDefinition(TextReader report)
		{
			lock (m_syncObject)
			{
				if (report == null)
				{
					throw new ArgumentNullException("report");
				}
				if (base.IsDrillthroughReport)
				{
					throw new InvalidOperationException();
				}
				string s = report.ReadToEnd();
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				m_executionInfo = Service.LoadReportDefinition(bytes);
				m_executionID = m_executionInfo.ExecutionID;
				OnExecutionIDChanged();
				m_reportPath = "";
				Refresh();
			}
		}

		internal override int PerformSearch(string searchText, int startPage, int endPage)
		{
			lock (m_syncObject)
			{
				if (!IsReadyForProcessingPostTasks)
				{
					throw new InvalidOperationException(CommonStrings.ReportNotReady);
				}
				return Service.FindString(startPage, endPage, searchText);
			}
		}

		internal override void PerformToggle(string toggleId)
		{
			lock (m_syncObject)
			{
				if (!IsReadyForProcessingPostTasks)
				{
					throw new InvalidOperationException(CommonStrings.ReportNotReady);
				}
				Service.ToggleItem(toggleId);
			}
		}

		internal override int PerformBookmarkNavigation(string bookmarkId, out string uniqueName)
		{
			lock (m_syncObject)
			{
				if (!IsReadyForProcessingPostTasks)
				{
					throw new InvalidOperationException(CommonStrings.ReportNotReady);
				}
				return Service.NavigateBookmark(bookmarkId, out uniqueName);
			}
		}

		internal override int PerformDocumentMapNavigation(string documentMapId)
		{
			lock (m_syncObject)
			{
				if (!IsReadyForProcessingPostTasks)
				{
					throw new InvalidOperationException(CommonStrings.ReportNotReady);
				}
				return Service.NavigateDocumentMap(documentMapId);
			}
		}

		internal override Report PerformDrillthrough(string drillthroughId, out string reportPath)
		{
			lock (m_syncObject)
			{
				if (!IsReadyForProcessingPostTasks)
				{
					throw new InvalidOperationException(CommonStrings.ReportNotReady);
				}
				ExecutionInfo executionInfo = Service.LoadDrillthroughTarget(drillthroughId);
				Service.SetExecutionId(m_executionID);
				reportPath = executionInfo.ReportPath;
				return new ServerReport(this, executionInfo);
			}
		}

		internal override int PerformSort(string sortId, SortOrder sortDirection, bool clearSort, PageCountMode pageCountMode, out string uniqueName)
		{
			lock (m_syncObject)
			{
				if (!IsReadyForProcessingPostTasks)
				{
					throw new InvalidOperationException(CommonStrings.ReportNotReady);
				}
				ExecutionInfo executionInfo;
				int numPages;
				int result = Service.Sort(sortId, sortDirection, clearSort, pageCountMode, out uniqueName, out executionInfo, out numPages);
				Service.SetExecutionId(m_executionID);
				if (executionInfo == null)
				{
					m_executionInfo.NumPages = numPages;
				}
				else
				{
					m_executionInfo = executionInfo;
				}
				return result;
			}
		}

		internal void TouchSession()
		{
			lock (m_syncObject)
			{
				if (IsReadyForConnection)
				{
					Service.GetExecutionInfo();
				}
			}
		}

		public override int GetTotalPages(out PageCountMode pageCountMode)
		{
			lock (m_syncObject)
			{
				if (m_executionInfo == null)
				{
					pageCountMode = PageCountMode.Estimate;
					return 0;
				}
				pageCountMode = PageCountMode;
				return m_executionInfo.NumPages;
			}
		}

		internal override DocumentMapNode GetDocumentMap(string rootLabel)
		{
			lock (m_syncObject)
			{
				if (!IsReadyForProcessingPostTasks)
				{
					throw new InvalidOperationException(CommonStrings.ReportNotReady);
				}
				if (!m_executionInfo.HasDocumentMap)
				{
					return null;
				}
				return Service.GetDocumentMap(rootLabel);
			}
		}

		public override RenderingExtension[] ListRenderingExtensions()
		{
			lock (m_syncObject)
			{
				if (m_renderingExtensions == null)
				{
					m_renderingExtensions = Service.ListRenderingExtensions();
				}
				return m_renderingExtensions;
			}
		}

		public override void Refresh()
		{
			lock (m_syncObject)
			{
				if (IsReadyForConnection && m_executionID != null)
				{
					m_executionInfo = Service.ResetExecution();
					OnChange(isRefreshOnly: true);
				}
			}
		}

		public ReportDataSourceInfoCollection GetDataSources()
		{
			bool allCredentialsSet;
			return GetDataSources(out allCredentialsSet);
		}

		public ReportDataSourceInfoCollection GetDataSources(out bool allCredentialsSet)
		{
			lock (m_syncObject)
			{
				EnsureExecutionSession();
				allCredentialsSet = !m_executionInfo.CredentialsRequired;
				return m_executionInfo.DataSourcePrompts;
			}
		}

		public string GetServerVersion()
		{
			lock (m_syncObject)
			{
				return Service.GetServerVersion();
			}
		}

		private void ApplyExecutionIdToService(IReportExecutionService service)
		{
			service?.SetExecutionId(m_executionID);
		}

		internal override void EnsureExecutionSession()
		{
			if (m_executionInfo != null)
			{
				return;
			}
			if (m_executionID == null)
			{
				string text = HistoryId;
				if (text != null && text.Length == 0)
				{
					text = null;
				}
				string reportPath = ReportPath;
				if (reportPath == null || reportPath.Length == 0)
				{
					throw new MissingReportSourceException();
				}
				if (string.IsNullOrEmpty(ReportServerUrl.ToString()))
				{
					throw new MissingReportServerUrlException();
				}
				m_executionInfo = Service.LoadReport(reportPath, text);
				m_executionID = m_executionInfo.ExecutionID;
				OnExecutionIDChanged();
			}
			else
			{
				Service.SetExecutionId(m_executionID);
				m_executionInfo = Service.GetExecutionInfo();
			}
		}

		private void ClearSession()
		{
			ClearSession(doRefresh: true);
		}

		private void ClearSession(bool doRefresh)
		{
			m_executionID = null;
			OnExecutionIDChanged();
			m_executionInfo = null;
			m_hiddenParameters.Clear();
			if (doRefresh)
			{
				Refresh();
			}
			OnChange(isRefreshOnly: false);
		}

		private void ClearServerSpecificInfo()
		{
			m_renderingExtensions = null;
			ClearSession();
		}

		private int GetIndexForParameter(string parameterName)
		{
			int num = -1;
			for (int i = 0; i < m_executionInfo.Parameters.Count; i++)
			{
				if (string.Compare(m_executionInfo.Parameters[i].Name, parameterName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					num = i;
					break;
				}
			}
			if (num == -1 && string.Compare(parameterName, "rs:StoredParametersID", StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new ArgumentException(CommonStrings.ParameterNotFound(parameterName));
			}
			return num;
		}

		private IReportExecutionService CreateExecutionService()
		{
			return new SoapReportExecutionService(m_serverIdentity, m_serverUrl, m_serverCredentials, m_trustedUserHeader, Headers, Cookies, m_timeOut);
		}
	}
}

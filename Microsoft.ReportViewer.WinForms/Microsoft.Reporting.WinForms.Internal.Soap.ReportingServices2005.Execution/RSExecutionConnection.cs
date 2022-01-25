using Microsoft.ReportingServices.Diagnostics;
using Microsoft.SqlServer.ReportingServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution
{
	internal class RSExecutionConnection : ReportExecutionServiceSoapClient
	{
		[Serializable]
		internal sealed class MissingEndpointException : Exception
		{
			public MissingEndpointException(Exception inner)
				: base(SoapExceptionStrings.MissingEndpoint, inner)
			{
			}

			private MissingEndpointException(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
			}

			public static void ThrowIfEndpointMissing(WebException e)
			{
				if (e.Status == WebExceptionStatus.ProtocolError && e.Response != null)
				{
                    if (e.Response is HttpWebResponse httpWebResponse && httpWebResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new MissingEndpointException(e);
                    }
                }
			}
		}

		[Serializable]
		internal sealed class SoapVersionMismatchException : Exception
		{
			private SoapVersionMismatchException(string message, Exception inner)
				: base(message, inner)
			{
			}

			private SoapVersionMismatchException(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
			}

			public static void ThrowIfVersionMismatch(FaultException e, string expectedEndpoint, string message, bool includeInnerException)
			{
				if (IsVersionMismatch(e, expectedEndpoint))
				{
					if (includeInnerException)
					{
						throw new SoapVersionMismatchException(message, e);
					}
					throw new SoapVersionMismatchException(message, null);
				}
			}

			public static bool IsVersionMismatch(FaultException e, string expectedEndpoint)
			{
				if (e.Code.IsSenderFault)
				{
					var fault = e.CreateMessageFault();
					return !fault.Actor.EndsWith(expectedEndpoint, StringComparison.OrdinalIgnoreCase);
				}
				return false;
			}
		}

		private sealed class SecureMethodsList : Dictionary<string, object>
		{
			public SecureMethodsList()
				: base((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase)
			{
			}
		}

		private static class ProxyMethodInvocation
		{
			internal static TReturn Execute<TReturn>(RSExecutionConnection connection, ProxyMethod<TReturn> method)
			{
				return Execute(connection, method, null);
			}

			internal static TReturn Execute<TReturn>(RSExecutionConnection connection, ProxyMethod<TReturn> initialMethod, ProxyMethod<TReturn> retryMethod)
			{
				using (MonitoredScope.NewConcat("ProxyMethodInvocation.Execute - Method : ", initialMethod.MethodName))
				{
					if (connection == null)
					{
						throw new ArgumentNullException(nameof(connection));
					}
					if (initialMethod == null)
					{
						throw new ArgumentNullException(nameof(initialMethod));
					}
					ProxyMethod<TReturn>[] array = (retryMethod != null && !connection.CanUseKatmaiMethods) ? new ProxyMethod<TReturn>[1]
					{
						retryMethod
					} : ((retryMethod != null) ? new ProxyMethod<TReturn>[2]
					{
						initialMethod,
						retryMethod
					} : new ProxyMethod<TReturn>[1]
					{
						initialMethod
					});
					for (int i = 0; i < array.Length; i++)
					{
						ProxyMethod<TReturn> proxyMethod = array[i];
						try
						{
							if (!string.IsNullOrEmpty(proxyMethod.MethodName))
							{
								connection.SetConnectionSSLForMethod(proxyMethod.MethodName);
							}
							return proxyMethod.Method();
						}
						catch (FaultException e)
						{
							if (i < array.Length - 1 && connection.CheckForDownlevelRetry(e))
							{
								connection.MarkAsFailedUsingKatmai();
								continue;
							}
							connection.OnSoapException(e);
							throw;
						}
						catch (WebException e2)
						{
							MissingEndpointException.ThrowIfEndpointMissing(e2);
							throw;
						}
						catch (InvalidOperationException inner)
						{
							throw new MissingEndpointException(inner);
						}
					}
					throw new InvalidOperationException("Failed to execute method");
				}
			}

			internal static TReturn Execute<TReturn>(RSExecutionConnection connection, ProxyMethod<TReturn> sql16Method, ProxyMethod<TReturn> katmaiMethod, ProxyMethod<TReturn> yukonMethod)
			{
				using (MonitoredScope.NewConcat("ProxyMethodInvocation.Execute - Method : ", katmaiMethod.MethodName))
				{
					if (connection == null)
					{
						throw new ArgumentNullException(nameof(connection));
					}
					if (katmaiMethod == null)
					{
                        throw new ArgumentNullException(nameof(katmaiMethod));
					}
					bool flag = yukonMethod != null;
					bool flag2 = katmaiMethod != null;
					ProxyMethod<TReturn>[] array = (flag && !connection.CanUseKatmaiMethods && !connection.CanUseSql16Methods) ? new ProxyMethod<TReturn>[1]
					{
						yukonMethod
					} : ((!(!flag && flag2) || connection.CanUseSql16Methods) ? new ProxyMethod<TReturn>[3]
					{
						sql16Method,
						katmaiMethod,
						yukonMethod
					} : new ProxyMethod<TReturn>[1]
					{
						katmaiMethod
					});
					for (int i = 0; i < array.Length; i++)
					{
						ProxyMethod<TReturn> proxyMethod = array[i];
						try
						{
							if (!string.IsNullOrEmpty(proxyMethod.MethodName))
							{
								connection.SetConnectionSSLForMethod(proxyMethod.MethodName);
							}
							return proxyMethod.Method();
						}
						catch (FaultException e)
						{
							if (i < array.Length - 1 && connection.CheckForDownlevelRetry(e))
							{
								if (connection.m_endpointVersion == EndpointVersion.Katmai)
								{
									connection.MarkAsFailedUsingKatmai();
								}
								else if (connection.m_endpointVersion == EndpointVersion.Sql16)
								{
									connection.MarkAsFailedUsingSql16();
								}
								continue;
							}
							connection.OnSoapException(e);
							throw;
						}
						catch (WebException e2)
						{
							MissingEndpointException.ThrowIfEndpointMissing(e2);
							throw;
						}
						catch (InvalidOperationException inner)
						{
							throw new MissingEndpointException(inner);
						}
					}
					throw new InvalidOperationException("Failed to execute method");
				}
			}
		}

		private sealed class ProxyMethod<TReturn>
		{
			internal delegate TReturn ProxyMethodCallback();

			private readonly ProxyMethodCallback m_method;

			private readonly string m_methodName;

			internal ProxyMethodCallback Method
			{
				[DebuggerStepThrough]
				get
				{
					return m_method;
				}
			}

			internal string MethodName
			{
				[DebuggerStepThrough]
				get
				{
					return m_methodName;
				}
			}

			internal ProxyMethod(string methodName, ProxyMethodCallback method)
			{
                m_methodName = methodName;
				m_method = method ?? throw new ArgumentNullException(nameof(method));
			}
		}

		internal const string SoapEndpoint = "ReportExecution2005.asmx";

		private string m_secureServerUrl;

		private string m_nonsecureServerUrl;

		private bool m_currentlyUsingSSL;

		private bool m_alwaysUseSSL;

		private bool m_failedUsingKatmai;

		private bool m_failedUsingSql16;

		private readonly EndpointVersion m_endpointVersion;

		private SecureMethodsList m_secureMethods;

		internal string UrlForRender => GetServerURL(IsSecureMethod("UrlRender"));

        private bool CanUseKatmaiMethods => m_endpointVersion switch
        {
			EndpointVersion.Yukon => false,
			EndpointVersion.Katmai => true,
			EndpointVersion.Automatic => !m_failedUsingKatmai,
            EndpointVersion.Sql16 => false,
            _ => false,
        };

        private bool CanUseSql16Methods
		{
			get
			{
                return m_endpointVersion switch
                {
                    EndpointVersion.Yukon => false,
                    EndpointVersion.Katmai => false,
                    EndpointVersion.Sql16 => true,
                    EndpointVersion.Automatic => !m_failedUsingSql16,
                    _ => false,
                };
            }
		}

		public RSExecutionConnection(string reportServerLocation, EndpointVersion version)
		{
			InitializeReportServerUrl(reportServerLocation);
			m_endpointVersion = version;
		}

		public void ValidateConnection()
		{
			try
			{
				IsSecureMethod("");
				if (base.ServerInfoHeaderValue == null)
				{
					ListSecureMethods();
				}
			}
			catch (FaultException e)
			{
				OnSoapException(e);
				throw;
			}
			catch (WebException e2)
			{
				MissingEndpointException.ThrowIfEndpointMissing(e2);
				throw;
			}
			catch (InvalidOperationException inner)
			{
				throw new MissingEndpointException(inner);
			}
		}

		private void SetConnectionSSLForMethod(string methodname)
		{
			SetConnectionSSL(IsSecureMethod(methodname));
		}

		private void SetConnectionSSL(bool useSSL)
		{
			if (m_currentlyUsingSSL != useSSL)
			{
				m_currentlyUsingSSL = useSSL;
				base.Url = GetSoapURL(m_currentlyUsingSSL);
			}
		}

		private void InitializeReportServerUrl(string reportServerLocation)
		{
			if (reportServerLocation != null)
			{
				m_secureMethods = null;
				UriBuilder uriBuilder = new(reportServerLocation);
				if (string.Compare(uriBuilder.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) == 0)
				{
					m_alwaysUseSSL = true;
					m_nonsecureServerUrl = null;
					m_secureServerUrl = uriBuilder.Uri.AbsoluteUri;
					m_currentlyUsingSSL = true;
				}
				else
				{
					m_alwaysUseSSL = false;
					m_nonsecureServerUrl = uriBuilder.Uri.AbsoluteUri;
					uriBuilder.Port = -1;
					uriBuilder.Scheme = Uri.UriSchemeHttps;
					m_secureServerUrl = uriBuilder.Uri.AbsoluteUri;
					m_currentlyUsingSSL = false;
				}
				base.Url = GetSoapURL(m_currentlyUsingSSL);
			}
		}

		internal string GetSoapURL(bool useSSL)
		{
			return GetServerURL(useSSL) + "/ReportExecution2005.asmx";
		}

		internal string GetServerURL(bool useSSL)
		{
			if (useSSL)
			{
				return m_secureServerUrl;
			}
			return m_nonsecureServerUrl;
		}
		/*
		protected override WebRequest GetWebRequest(Uri uri)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)base.GetWebRequest(uri);
			if (5 == Environment.OSVersion.Version.Major && m_unsafeHeaderServerIsIIS5 && httpWebRequest.Credentials == CredentialCache.DefaultCredentials)
			{
				httpWebRequest.UnsafeAuthenticatedConnectionSharing = true;
				httpWebRequest.ConnectionGroupName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
			}
			return httpWebRequest;
		}

		protected override WebResponse GetWebResponse(WebRequest request)
		{
			WebResponse webResponse = base.GetWebResponse(request);
			if (string.Compare(webResponse.Headers["Server"], "Microsoft-IIS/5.0", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(webResponse.Headers["Server"], "Microsoft-IIS/5.1", StringComparison.OrdinalIgnoreCase) == 0)
			{
				m_unsafeHeaderServerIsIIS5 = true;
			}
			return webResponse;
		}
		*/
		protected virtual void OnSoapException(FaultException e)
		{
			SoapVersionMismatchException.ThrowIfVersionMismatch(e, "ReportExecution2005.asmx", SoapExceptionStrings.VersionMismatch, includeInnerException: true);
		}

		private string[] GetSecureMethods()
		{
			try
			{
				SetConnectionSSL(m_alwaysUseSSL);
				return ListSecureMethods();
			}
			catch (Exception ex)
			{
				if (m_alwaysUseSSL)
				{
                    throw ex;
				}
				m_alwaysUseSSL = true;
				SetConnectionSSL(useSSL: true);
				try
				{
					return ListSecureMethods();
				}
				catch
				{
					m_alwaysUseSSL = false;
                    if (ex is WebException ex2)
                    {
                        if (ex2.Response is HttpWebResponse httpWebResponse && httpWebResponse.StatusCode == HttpStatusCode.Forbidden)
                        {
                            throw;
                        }
                    }
                    throw ex;
				}
			}
		}

		private bool IsSecureMethod(string methodname)
		{
			if (m_alwaysUseSSL)
			{
				return true;
			}
			if (m_secureMethods == null)
			{
				string[] secureMethods = GetSecureMethods();
				if (m_alwaysUseSSL)
				{
					return true;
				}
				m_secureMethods = new SecureMethodsList();
				string[] array = secureMethods;
				foreach (string key in array)
				{
					m_secureMethods.Add(key, null);
				}
			}
			return m_secureMethods.ContainsKey(methodname);
		}

		private bool CheckForDownlevelRetry(FaultException e)
		{
            return m_endpointVersion switch
            {
                EndpointVersion.Yukon or EndpointVersion.Katmai or EndpointVersion.Automatic => SoapVersionMismatchException.IsVersionMismatch(e, "ReportExecution2005.asmx"),
                EndpointVersion.Sql16 => false,
                _ => false,
            };
        }

		private void MarkAsFailedUsingKatmai()
		{
			m_failedUsingKatmai = true;
		}

		private void MarkAsFailedUsingSql16()
		{
			m_failedUsingSql16 = true;
		}

		public new ExecutionInfo LoadReport(string Report, string HistoryID)
		{
			ProxyMethod<ExecutionInfo> sql16Method = new("LoadReport3", () => LoadReport3(Report, HistoryID));
			ProxyMethod<ExecutionInfo> katmaiMethod = new("LoadReport2", () => LoadReport2(Report, HistoryID));
			ProxyMethod<ExecutionInfo> yukonMethod = new("LoadReport", () => base.LoadReport(Report, HistoryID));
			return ProxyMethodInvocation.Execute(this, sql16Method, katmaiMethod, yukonMethod);
		}

		public new ExecutionInfo LoadReportDefinition(byte[] Definition, out Warning[] warnings)
		{
			Warning[] w = null;
			ProxyMethod<ExecutionInfo> sql16Method = new("LoadReportDefinition3", () => LoadReportDefinition3(Definition, out w));
			ProxyMethod<ExecutionInfo> katmaiMethod = new("LoadReportDefinition2", () => LoadReportDefinition2(Definition, out w));
			ProxyMethod<ExecutionInfo> yukonMethod = new("LoadReportDefinition", () => base.LoadReportDefinition(Definition, out w));
			ExecutionInfo result = ProxyMethodInvocation.Execute(this, sql16Method, katmaiMethod, yukonMethod);
			warnings = w;
			return result;
		}

		public new ExecutionInfo SetExecutionCredentials(DataSourceCredentials[] Credentials)
		{
			ProxyMethod<ExecutionInfo> sql16Method = new("SetExecutionCredentials3", () => SetExecutionCredentials3(Credentials));
			ProxyMethod<ExecutionInfo> katmaiMethod = new("SetExecutionCredentials2", () => SetExecutionCredentials2(Credentials));
			ProxyMethod<ExecutionInfo> yukonMethod = new("SetExecutionCredentials", () => base.SetExecutionCredentials(Credentials));
			return ProxyMethodInvocation.Execute(this, sql16Method, katmaiMethod, yukonMethod);
		}

		public new ExecutionInfo SetExecutionParameters(ParameterValue[] Parameters, string ParameterLanguage)
		{
			ProxyMethod<ExecutionInfo> sql16Method = new("SetExecutionParameters3", () => SetExecutionParameters3(Parameters, ParameterLanguage));
			ProxyMethod<ExecutionInfo> katmaiMethod = new("SetExecutionParameters2", () => SetExecutionParameters2(Parameters, ParameterLanguage));
			ProxyMethod<ExecutionInfo> yukonMethod = new("SetExecutionParameters", () => base.SetExecutionParameters(Parameters, ParameterLanguage));
			return ProxyMethodInvocation.Execute(this, sql16Method, katmaiMethod, yukonMethod);
		}

		public new ExecutionInfo ResetExecution()
		{
			ProxyMethod<ExecutionInfo> sql16Method = new("ResetExecution3", () => ResetExecution3());
			ProxyMethod<ExecutionInfo> katmaiMethod = new("ResetExecution2", () => ResetExecution2());
			ProxyMethod<ExecutionInfo> yukonMethod = new("ResetExecution", () => base.ResetExecution());
			return ProxyMethodInvocation.Execute(this, sql16Method, katmaiMethod, yukonMethod);
		}

		public new byte[] Render(string Format, string DeviceInfo, out string Extension, out string MimeType, out string Encoding, out Warning[] Warnings, out string[] StreamIds)
		{
			return Render(Format, DeviceInfo, PageCountMode.Actual, out Extension, out MimeType, out Encoding, out Warnings, out StreamIds);
		}

		public byte[] Render(string Format, string DeviceInfo, PageCountMode PaginationMode, out string Extension, out string MimeType, out string Encoding, out Warning[] Warnings, out string[] StreamIds)
		{
			string ext = null;
			string mime = null;
			string enc = null;
			Warning[] w = null;
			string[] sids = null;
			ProxyMethod<byte[]> initialMethod = new("Render2", () => Render2(Format, DeviceInfo, PaginationMode, out ext, out mime, out enc, out w, out sids));
			ProxyMethod<byte[]> retryMethod = new("Render", () => base.Render(Format, DeviceInfo, out ext, out mime, out enc, out w, out sids));
			byte[] result = ProxyMethodInvocation.Execute(this, initialMethod, retryMethod);
			Extension = ext;
			MimeType = mime;
			Encoding = enc;
			Warnings = w;
			StreamIds = sids;
			return result;
		}

		public new byte[] RenderStream(string Format, string StreamID, string DeviceInfo, out string Encoding, out string MimeType)
		{
			string enc = null;
			string mime = null;
			ProxyMethod<byte[]> method = new("RenderStream", () => base.RenderStream(Format, StreamID, DeviceInfo, out enc, out mime));
			byte[] result = ProxyMethodInvocation.Execute(this, method);
			Encoding = enc;
			MimeType = mime;
			return result;
		}

		public new void DeliverReportItem(string Format, string DeviceInfo, ExtensionSettings ExtensionSettings, string Description, string EventType, string MatchData)
		{
			ProxyMethod<int> method = new("DeliverReportItem", delegate
			{
				base.DeliverReportItem(Format, DeviceInfo, ExtensionSettings, Description, EventType, MatchData);
				return 0;
			});
			ProxyMethodInvocation.Execute(this, method);
		}

		public new ExecutionInfo GetExecutionInfo()
		{
			ProxyMethod<ExecutionInfo> sql16Method = new("GetExecutionInfo3", () => GetExecutionInfo3());
			ProxyMethod<ExecutionInfo> katmaiMethod = new("GetExecutionInfo2", () => GetExecutionInfo2());
			ProxyMethod<ExecutionInfo> yukonMethod = new("GetExecutionInfo", () => base.GetExecutionInfo());
			return ProxyMethodInvocation.Execute(this, sql16Method, katmaiMethod, yukonMethod);
		}

		public new DocumentMapNode GetDocumentMap()
		{
			ProxyMethod<DocumentMapNode> method = new("GetDocumentMap", () => base.GetDocumentMap());
			return ProxyMethodInvocation.Execute(this, method);
		}

		public new ExecutionInfo LoadDrillthroughTarget(string DrillthroughID)
		{
			ProxyMethod<ExecutionInfo> sql16Method = new("LoadDrillthroughTarget3", () => LoadDrillthroughTarget3(DrillthroughID));
			ProxyMethod<ExecutionInfo> katmaiMethod = new("LoadDrillthroughTarget2", () => LoadDrillthroughTarget2(DrillthroughID));
			ProxyMethod<ExecutionInfo> yukonMethod = new("LoadDrillthroughTarget", () => base.LoadDrillthroughTarget(DrillthroughID));
			return ProxyMethodInvocation.Execute(this, sql16Method, katmaiMethod, yukonMethod);
		}

		public new bool ToggleItem(string ToggleID)
		{
			ProxyMethod<bool> method = new("ToggleItem", () => base.ToggleItem(ToggleID));
			return ProxyMethodInvocation.Execute(this, method);
		}

		public new int NavigateDocumentMap(string DocMapID)
		{
			ProxyMethod<int> method = new("NavigateDocumentMap", () => base.NavigateDocumentMap(DocMapID));
			return ProxyMethodInvocation.Execute(this, method);
		}

		public new int NavigateBookmark(string BookmarkID, out string UniqueName)
		{
			string name = null;
			ProxyMethod<int> method = new("NavigateBookmark", () => base.NavigateBookmark(BookmarkID, out name));
			int result = ProxyMethodInvocation.Execute(this, method);
			UniqueName = name;
			return result;
		}

		public new int Sort(string SortItem, SortDirectionEnum Direction, bool Clear, out string ReportItem, out int NumPages)
		{
			string rptItem = null;
			int nPages = 0;
			ProxyMethod<int> method = new("Sort", () => base.Sort(SortItem, Direction, Clear, out rptItem, out nPages));
			int result = ProxyMethodInvocation.Execute(this, method);
			ReportItem = rptItem;
			NumPages = nPages;
			return result;
		}

		public int Sort(string SortItem, SortDirectionEnum Direction, bool Clear, PageCountMode PaginationMode, out string ReportItem, out ExecutionInfo ExecutionInfo, out int NumPages)
		{
			string rptItem = null;
			int nPages = 0;
			ExecutionInfo2 execInfo = null;
			ProxyMethod<int> initialMethod = new("Sort2", delegate
            {
                int result2 = Sort2(SortItem, Direction, Clear, PaginationMode, out rptItem, out execInfo);
                if (execInfo != null)
                {
                    nPages = execInfo.NumPages;
                }
                return result2;
            });
			ProxyMethod<int> retryMethod = new("Sort", () => base.Sort(SortItem, Direction, Clear, out rptItem, out nPages));
			int result = ProxyMethodInvocation.Execute(this, initialMethod, retryMethod);
			ExecutionInfo = execInfo;
			NumPages = nPages;
			ReportItem = rptItem;
			return result;
		}

		public new int FindString(int startPage, int endPage, string findValue)
		{
			ProxyMethod<int> method = new("FindString", () => base.FindString(startPage, endPage, findValue));
			return ProxyMethodInvocation.Execute(this, method);
		}

		public new byte[] GetRenderResource(string Format, string DeviceInfo, out string MimeType)
		{
			string mimeType = null;
			ProxyMethod<byte[]> method = new("GetRenderResource", () => base.GetRenderResource(Format, DeviceInfo, out mimeType));
			byte[] result = ProxyMethodInvocation.Execute(this, method);
			MimeType = mimeType;
			return result;
		}

		public new Extension[] ListRenderingExtensions()
		{
			ProxyMethod<Extension[]> method = new("ListRenderingExtensions", () => base.ListRenderingExtensions());
			return ProxyMethodInvocation.Execute(this, method);
		}

		public new void LogonUser(string userName, string password, string authority)
		{
			ProxyMethod<int> method = new(null, delegate
			{
				base.LogonUser(userName, password, authority);
				return 0;
			});
			ProxyMethodInvocation.Execute(this, method);
		}

		public new void Logoff()
		{
			ProxyMethod<int> method = new("Logoff", delegate
            {
                base.Logoff();
                return 0;
            });
			ProxyMethodInvocation.Execute(this, method);
		}
    }
}

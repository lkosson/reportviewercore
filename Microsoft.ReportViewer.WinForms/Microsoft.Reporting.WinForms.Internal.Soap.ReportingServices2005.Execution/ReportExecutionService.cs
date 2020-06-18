using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution
{
	[GeneratedCode("wsdl", "2.0.50727.42")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[WebServiceBinding(Name = "ReportExecutionServiceSoap", Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
	[XmlInclude(typeof(ParameterValueOrFieldReference))]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ToolboxItem(false)]
	public class ReportExecutionService : SoapHttpClientProtocol
	{
		private TrustedUserHeader trustedUserHeaderValueField;

		private ServerInfoHeader serverInfoHeaderValueField;

		private SendOrPostCallback ListSecureMethodsOperationCompleted;

		private ExecutionHeader executionHeaderValueField;

		private SendOrPostCallback LoadReportOperationCompleted;

		private SendOrPostCallback LoadReport3OperationCompleted;

		private SendOrPostCallback LoadReport2OperationCompleted;

		private SendOrPostCallback LoadReportDefinitionOperationCompleted;

		private SendOrPostCallback LoadReportDefinition2OperationCompleted;

		private SendOrPostCallback LoadReportDefinition3OperationCompleted;

		private SendOrPostCallback SetExecutionCredentialsOperationCompleted;

		private SendOrPostCallback SetExecutionCredentials2OperationCompleted;

		private SendOrPostCallback SetExecutionCredentials3OperationCompleted;

		private SendOrPostCallback SetExecutionParametersOperationCompleted;

		private SendOrPostCallback SetExecutionParameters2OperationCompleted;

		private SendOrPostCallback SetExecutionParameters3OperationCompleted;

		private SendOrPostCallback ResetExecutionOperationCompleted;

		private SendOrPostCallback ResetExecution2OperationCompleted;

		private SendOrPostCallback ResetExecution3OperationCompleted;

		private SendOrPostCallback RenderOperationCompleted;

		private SendOrPostCallback Render2OperationCompleted;

		private SendOrPostCallback DeliverReportItemOperationCompleted;

		private SendOrPostCallback RenderStreamOperationCompleted;

		private SendOrPostCallback GetExecutionInfoOperationCompleted;

		private SendOrPostCallback GetExecutionInfo2OperationCompleted;

		private SendOrPostCallback GetExecutionInfo3OperationCompleted;

		private SendOrPostCallback GetDocumentMapOperationCompleted;

		private SendOrPostCallback LoadDrillthroughTargetOperationCompleted;

		private SendOrPostCallback LoadDrillthroughTarget2OperationCompleted;

		private SendOrPostCallback LoadDrillthroughTarget3OperationCompleted;

		private SendOrPostCallback ToggleItemOperationCompleted;

		private SendOrPostCallback NavigateDocumentMapOperationCompleted;

		private SendOrPostCallback NavigateBookmarkOperationCompleted;

		private SendOrPostCallback FindStringOperationCompleted;

		private SendOrPostCallback SortOperationCompleted;

		private SendOrPostCallback Sort2OperationCompleted;

		private SendOrPostCallback Sort3OperationCompleted;

		private SendOrPostCallback GetRenderResourceOperationCompleted;

		private SendOrPostCallback ListRenderingExtensionsOperationCompleted;

		private SendOrPostCallback LogonUserOperationCompleted;

		private SendOrPostCallback LogoffOperationCompleted;

		public TrustedUserHeader TrustedUserHeaderValue
		{
			get
			{
				return trustedUserHeaderValueField;
			}
			set
			{
				trustedUserHeaderValueField = value;
			}
		}

		public ServerInfoHeader ServerInfoHeaderValue
		{
			get
			{
				return serverInfoHeaderValueField;
			}
			set
			{
				serverInfoHeaderValueField = value;
			}
		}

		public ExecutionHeader ExecutionHeaderValue
		{
			get
			{
				return executionHeaderValueField;
			}
			set
			{
				executionHeaderValueField = value;
			}
		}

		public event ListSecureMethodsCompletedEventHandler ListSecureMethodsCompleted;

		public event LoadReportCompletedEventHandler LoadReportCompleted;

		public event LoadReport3CompletedEventHandler LoadReport3Completed;

		public event LoadReport2CompletedEventHandler LoadReport2Completed;

		public event LoadReportDefinitionCompletedEventHandler LoadReportDefinitionCompleted;

		public event LoadReportDefinition2CompletedEventHandler LoadReportDefinition2Completed;

		public event LoadReportDefinition3CompletedEventHandler LoadReportDefinition3Completed;

		public event SetExecutionCredentialsCompletedEventHandler SetExecutionCredentialsCompleted;

		public event SetExecutionCredentials2CompletedEventHandler SetExecutionCredentials2Completed;

		public event SetExecutionCredentials3CompletedEventHandler SetExecutionCredentials3Completed;

		public event SetExecutionParametersCompletedEventHandler SetExecutionParametersCompleted;

		public event SetExecutionParameters2CompletedEventHandler SetExecutionParameters2Completed;

		public event SetExecutionParameters3CompletedEventHandler SetExecutionParameters3Completed;

		public event ResetExecutionCompletedEventHandler ResetExecutionCompleted;

		public event ResetExecution2CompletedEventHandler ResetExecution2Completed;

		public event ResetExecution3CompletedEventHandler ResetExecution3Completed;

		public event RenderCompletedEventHandler RenderCompleted;

		public event Render2CompletedEventHandler Render2Completed;

		public event DeliverReportItemCompletedEventHandler DeliverReportItemCompleted;

		public event RenderStreamCompletedEventHandler RenderStreamCompleted;

		public event GetExecutionInfoCompletedEventHandler GetExecutionInfoCompleted;

		public event GetExecutionInfo2CompletedEventHandler GetExecutionInfo2Completed;

		public event GetExecutionInfo3CompletedEventHandler GetExecutionInfo3Completed;

		public event GetDocumentMapCompletedEventHandler GetDocumentMapCompleted;

		public event LoadDrillthroughTargetCompletedEventHandler LoadDrillthroughTargetCompleted;

		public event LoadDrillthroughTarget2CompletedEventHandler LoadDrillthroughTarget2Completed;

		public event LoadDrillthroughTarget3CompletedEventHandler LoadDrillthroughTarget3Completed;

		public event ToggleItemCompletedEventHandler ToggleItemCompleted;

		public event NavigateDocumentMapCompletedEventHandler NavigateDocumentMapCompleted;

		public event NavigateBookmarkCompletedEventHandler NavigateBookmarkCompleted;

		public event FindStringCompletedEventHandler FindStringCompleted;

		public event SortCompletedEventHandler SortCompleted;

		public event Sort2CompletedEventHandler Sort2Completed;

		public event Sort3CompletedEventHandler Sort3Completed;

		public event GetRenderResourceCompletedEventHandler GetRenderResourceCompleted;

		public event ListRenderingExtensionsCompletedEventHandler ListRenderingExtensionsCompleted;

		public event LogonUserCompletedEventHandler LogonUserCompleted;

		public event LogoffCompletedEventHandler LogoffCompleted;

		public ReportExecutionService()
		{
			base.Url = "http://localhost/ReportServer/ReportExecution2005.asmx";
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/ListSecureMethods", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string[] ListSecureMethods()
		{
			return (string[])Invoke("ListSecureMethods", new object[0])[0];
		}

		protected new object[] Invoke(string methodName, object[] parameters)
		{
			return base.Invoke(methodName, parameters);
		}

		public IAsyncResult BeginListSecureMethods(AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("ListSecureMethods", new object[0], callback, asyncState);
		}

		public string[] EndListSecureMethods(IAsyncResult asyncResult)
		{
			return (string[])EndInvoke(asyncResult)[0];
		}

		public void ListSecureMethodsAsync()
		{
			ListSecureMethodsAsync(null);
		}

		public void ListSecureMethodsAsync(object userState)
		{
			if (ListSecureMethodsOperationCompleted == null)
			{
				ListSecureMethodsOperationCompleted = OnListSecureMethodsOperationCompleted;
			}
			InvokeAsync("ListSecureMethods", new object[0], ListSecureMethodsOperationCompleted, userState);
		}

		private void OnListSecureMethodsOperationCompleted(object arg)
		{
			if (this.ListSecureMethodsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ListSecureMethodsCompleted(this, new ListSecureMethodsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadReport", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("executionInfo")]
		public ExecutionInfo LoadReport(string Report, string HistoryID)
		{
			return (ExecutionInfo)Invoke("LoadReport", new object[2]
			{
				Report,
				HistoryID
			})[0];
		}

		public IAsyncResult BeginLoadReport(string Report, string HistoryID, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("LoadReport", new object[2]
			{
				Report,
				HistoryID
			}, callback, asyncState);
		}

		public ExecutionInfo EndLoadReport(IAsyncResult asyncResult)
		{
			return (ExecutionInfo)EndInvoke(asyncResult)[0];
		}

		public void LoadReportAsync(string Report, string HistoryID)
		{
			LoadReportAsync(Report, HistoryID, null);
		}

		public void LoadReportAsync(string Report, string HistoryID, object userState)
		{
			if (LoadReportOperationCompleted == null)
			{
				LoadReportOperationCompleted = OnLoadReportOperationCompleted;
			}
			InvokeAsync("LoadReport", new object[2]
			{
				Report,
				HistoryID
			}, LoadReportOperationCompleted, userState);
		}

		private void OnLoadReportOperationCompleted(object arg)
		{
			if (this.LoadReportCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.LoadReportCompleted(this, new LoadReportCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadReport3", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("executionInfo")]
		public ExecutionInfo3 LoadReport3(string Report, string HistoryID)
		{
			return (ExecutionInfo3)Invoke("LoadReport3", new object[2]
			{
				Report,
				HistoryID
			})[0];
		}

		public IAsyncResult BeginLoadReport3(string Report, string HistoryID, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("LoadReport3", new object[2]
			{
				Report,
				HistoryID
			}, callback, asyncState);
		}

		public ExecutionInfo3 EndLoadReport3(IAsyncResult asyncResult)
		{
			return (ExecutionInfo3)EndInvoke(asyncResult)[0];
		}

		public void LoadReport3Async(string Report, string HistoryID)
		{
			LoadReport3Async(Report, HistoryID, null);
		}

		public void LoadReport3Async(string Report, string HistoryID, object userState)
		{
			if (LoadReport3OperationCompleted == null)
			{
				LoadReport3OperationCompleted = OnLoadReport3OperationCompleted;
			}
			InvokeAsync("LoadReport3", new object[2]
			{
				Report,
				HistoryID
			}, LoadReport3OperationCompleted, userState);
		}

		private void OnLoadReport3OperationCompleted(object arg)
		{
			if (this.LoadReport3Completed != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.LoadReport3Completed(this, new LoadReport3CompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadReport2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("executionInfo")]
		public ExecutionInfo2 LoadReport2(string Report, string HistoryID)
		{
			return (ExecutionInfo2)Invoke("LoadReport2", new object[2]
			{
				Report,
				HistoryID
			})[0];
		}

		public IAsyncResult BeginLoadReport2(string Report, string HistoryID, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("LoadReport2", new object[2]
			{
				Report,
				HistoryID
			}, callback, asyncState);
		}

		public ExecutionInfo2 EndLoadReport2(IAsyncResult asyncResult)
		{
			return (ExecutionInfo2)EndInvoke(asyncResult)[0];
		}

		public void LoadReport2Async(string Report, string HistoryID)
		{
			LoadReport2Async(Report, HistoryID, null);
		}

		public void LoadReport2Async(string Report, string HistoryID, object userState)
		{
			if (LoadReport2OperationCompleted == null)
			{
				LoadReport2OperationCompleted = OnLoadReport2OperationCompleted;
			}
			InvokeAsync("LoadReport2", new object[2]
			{
				Report,
				HistoryID
			}, LoadReport2OperationCompleted, userState);
		}

		private void OnLoadReport2OperationCompleted(object arg)
		{
			if (this.LoadReport2Completed != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.LoadReport2Completed(this, new LoadReport2CompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadReportDefinition", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("executionInfo")]
		public ExecutionInfo LoadReportDefinition([XmlElement(DataType = "base64Binary")] byte[] Definition, out Warning[] warnings)
		{
			object[] array = Invoke("LoadReportDefinition", new object[1]
			{
				Definition
			});
			warnings = (Warning[])array[1];
			return (ExecutionInfo)array[0];
		}

		public IAsyncResult BeginLoadReportDefinition(byte[] Definition, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("LoadReportDefinition", new object[1]
			{
				Definition
			}, callback, asyncState);
		}

		public ExecutionInfo EndLoadReportDefinition(IAsyncResult asyncResult, out Warning[] warnings)
		{
			object[] array = EndInvoke(asyncResult);
			warnings = (Warning[])array[1];
			return (ExecutionInfo)array[0];
		}

		public void LoadReportDefinitionAsync(byte[] Definition)
		{
			LoadReportDefinitionAsync(Definition, null);
		}

		public void LoadReportDefinitionAsync(byte[] Definition, object userState)
		{
			if (LoadReportDefinitionOperationCompleted == null)
			{
				LoadReportDefinitionOperationCompleted = OnLoadReportDefinitionOperationCompleted;
			}
			InvokeAsync("LoadReportDefinition", new object[1]
			{
				Definition
			}, LoadReportDefinitionOperationCompleted, userState);
		}

		private void OnLoadReportDefinitionOperationCompleted(object arg)
		{
			if (this.LoadReportDefinitionCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.LoadReportDefinitionCompleted(this, new LoadReportDefinitionCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadReportDefinition2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("executionInfo")]
		public ExecutionInfo2 LoadReportDefinition2([XmlElement(DataType = "base64Binary")] byte[] Definition, out Warning[] warnings)
		{
			object[] array = Invoke("LoadReportDefinition2", new object[1]
			{
				Definition
			});
			warnings = (Warning[])array[1];
			return (ExecutionInfo2)array[0];
		}

		public IAsyncResult BeginLoadReportDefinition2(byte[] Definition, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("LoadReportDefinition2", new object[1]
			{
				Definition
			}, callback, asyncState);
		}

		public ExecutionInfo2 EndLoadReportDefinition2(IAsyncResult asyncResult, out Warning[] warnings)
		{
			object[] array = EndInvoke(asyncResult);
			warnings = (Warning[])array[1];
			return (ExecutionInfo2)array[0];
		}

		public void LoadReportDefinition2Async(byte[] Definition)
		{
			LoadReportDefinition2Async(Definition, null);
		}

		public void LoadReportDefinition2Async(byte[] Definition, object userState)
		{
			if (LoadReportDefinition2OperationCompleted == null)
			{
				LoadReportDefinition2OperationCompleted = OnLoadReportDefinition2OperationCompleted;
			}
			InvokeAsync("LoadReportDefinition2", new object[1]
			{
				Definition
			}, LoadReportDefinition2OperationCompleted, userState);
		}

		private void OnLoadReportDefinition2OperationCompleted(object arg)
		{
			if (this.LoadReportDefinition2Completed != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.LoadReportDefinition2Completed(this, new LoadReportDefinition2CompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadReportDefinition3", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("executionInfo")]
		public ExecutionInfo3 LoadReportDefinition3([XmlElement(DataType = "base64Binary")] byte[] Definition, out Warning[] warnings)
		{
			object[] array = Invoke("LoadReportDefinition3", new object[1]
			{
				Definition
			});
			warnings = (Warning[])array[1];
			return (ExecutionInfo3)array[0];
		}

		public IAsyncResult BeginLoadReportDefinition3(byte[] Definition, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("LoadReportDefinition3", new object[1]
			{
				Definition
			}, callback, asyncState);
		}

		public ExecutionInfo3 EndLoadReportDefinition3(IAsyncResult asyncResult, out Warning[] warnings)
		{
			object[] array = EndInvoke(asyncResult);
			warnings = (Warning[])array[1];
			return (ExecutionInfo3)array[0];
		}

		public void LoadReportDefinition3Async(byte[] Definition)
		{
			LoadReportDefinition3Async(Definition, null);
		}

		public void LoadReportDefinition3Async(byte[] Definition, object userState)
		{
			if (LoadReportDefinition3OperationCompleted == null)
			{
				LoadReportDefinition3OperationCompleted = OnLoadReportDefinition3OperationCompleted;
			}
			InvokeAsync("LoadReportDefinition3", new object[1]
			{
				Definition
			}, LoadReportDefinition3OperationCompleted, userState);
		}

		private void OnLoadReportDefinition3OperationCompleted(object arg)
		{
			if (this.LoadReportDefinition3Completed != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.LoadReportDefinition3Completed(this, new LoadReportDefinition3CompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/SetExecutionCredentials", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("executionInfo")]
		public ExecutionInfo SetExecutionCredentials(DataSourceCredentials[] Credentials)
		{
			return (ExecutionInfo)Invoke("SetExecutionCredentials", new object[1]
			{
				Credentials
			})[0];
		}

		public IAsyncResult BeginSetExecutionCredentials(DataSourceCredentials[] Credentials, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("SetExecutionCredentials", new object[1]
			{
				Credentials
			}, callback, asyncState);
		}

		public ExecutionInfo EndSetExecutionCredentials(IAsyncResult asyncResult)
		{
			return (ExecutionInfo)EndInvoke(asyncResult)[0];
		}

		public void SetExecutionCredentialsAsync(DataSourceCredentials[] Credentials)
		{
			SetExecutionCredentialsAsync(Credentials, null);
		}

		public void SetExecutionCredentialsAsync(DataSourceCredentials[] Credentials, object userState)
		{
			if (SetExecutionCredentialsOperationCompleted == null)
			{
				SetExecutionCredentialsOperationCompleted = OnSetExecutionCredentialsOperationCompleted;
			}
			InvokeAsync("SetExecutionCredentials", new object[1]
			{
				Credentials
			}, SetExecutionCredentialsOperationCompleted, userState);
		}

		private void OnSetExecutionCredentialsOperationCompleted(object arg)
		{
			if (this.SetExecutionCredentialsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetExecutionCredentialsCompleted(this, new SetExecutionCredentialsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/SetExecutionCredentials2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("executionInfo")]
		public ExecutionInfo2 SetExecutionCredentials2(DataSourceCredentials[] Credentials)
		{
			return (ExecutionInfo2)Invoke("SetExecutionCredentials2", new object[1]
			{
				Credentials
			})[0];
		}

		public IAsyncResult BeginSetExecutionCredentials2(DataSourceCredentials[] Credentials, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("SetExecutionCredentials2", new object[1]
			{
				Credentials
			}, callback, asyncState);
		}

		public ExecutionInfo2 EndSetExecutionCredentials2(IAsyncResult asyncResult)
		{
			return (ExecutionInfo2)EndInvoke(asyncResult)[0];
		}

		public void SetExecutionCredentials2Async(DataSourceCredentials[] Credentials)
		{
			SetExecutionCredentials2Async(Credentials, null);
		}

		public void SetExecutionCredentials2Async(DataSourceCredentials[] Credentials, object userState)
		{
			if (SetExecutionCredentials2OperationCompleted == null)
			{
				SetExecutionCredentials2OperationCompleted = OnSetExecutionCredentials2OperationCompleted;
			}
			InvokeAsync("SetExecutionCredentials2", new object[1]
			{
				Credentials
			}, SetExecutionCredentials2OperationCompleted, userState);
		}

		private void OnSetExecutionCredentials2OperationCompleted(object arg)
		{
			if (this.SetExecutionCredentials2Completed != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetExecutionCredentials2Completed(this, new SetExecutionCredentials2CompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/SetExecutionCredentials3", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("executionInfo")]
		public ExecutionInfo3 SetExecutionCredentials3(DataSourceCredentials[] Credentials)
		{
			return (ExecutionInfo3)Invoke("SetExecutionCredentials3", new object[1]
			{
				Credentials
			})[0];
		}

		public IAsyncResult BeginSetExecutionCredentials3(DataSourceCredentials[] Credentials, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("SetExecutionCredentials3", new object[1]
			{
				Credentials
			}, callback, asyncState);
		}

		public ExecutionInfo3 EndSetExecutionCredentials3(IAsyncResult asyncResult)
		{
			return (ExecutionInfo3)EndInvoke(asyncResult)[0];
		}

		public void SetExecutionCredentials3Async(DataSourceCredentials[] Credentials)
		{
			SetExecutionCredentials3Async(Credentials, null);
		}

		public void SetExecutionCredentials3Async(DataSourceCredentials[] Credentials, object userState)
		{
			if (SetExecutionCredentials3OperationCompleted == null)
			{
				SetExecutionCredentials3OperationCompleted = OnSetExecutionCredentials3OperationCompleted;
			}
			InvokeAsync("SetExecutionCredentials3", new object[1]
			{
				Credentials
			}, SetExecutionCredentials3OperationCompleted, userState);
		}

		private void OnSetExecutionCredentials3OperationCompleted(object arg)
		{
			if (this.SetExecutionCredentials3Completed != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetExecutionCredentials3Completed(this, new SetExecutionCredentials3CompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/SetExecutionParameters", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("executionInfo")]
		public ExecutionInfo SetExecutionParameters(ParameterValue[] Parameters, string ParameterLanguage)
		{
			return (ExecutionInfo)Invoke("SetExecutionParameters", new object[2]
			{
				Parameters,
				ParameterLanguage
			})[0];
		}

		public IAsyncResult BeginSetExecutionParameters(ParameterValue[] Parameters, string ParameterLanguage, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("SetExecutionParameters", new object[2]
			{
				Parameters,
				ParameterLanguage
			}, callback, asyncState);
		}

		public ExecutionInfo EndSetExecutionParameters(IAsyncResult asyncResult)
		{
			return (ExecutionInfo)EndInvoke(asyncResult)[0];
		}

		public void SetExecutionParametersAsync(ParameterValue[] Parameters, string ParameterLanguage)
		{
			SetExecutionParametersAsync(Parameters, ParameterLanguage, null);
		}

		public void SetExecutionParametersAsync(ParameterValue[] Parameters, string ParameterLanguage, object userState)
		{
			if (SetExecutionParametersOperationCompleted == null)
			{
				SetExecutionParametersOperationCompleted = OnSetExecutionParametersOperationCompleted;
			}
			InvokeAsync("SetExecutionParameters", new object[2]
			{
				Parameters,
				ParameterLanguage
			}, SetExecutionParametersOperationCompleted, userState);
		}

		private void OnSetExecutionParametersOperationCompleted(object arg)
		{
			if (this.SetExecutionParametersCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetExecutionParametersCompleted(this, new SetExecutionParametersCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/SetExecutionParameters2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("executionInfo")]
		public ExecutionInfo2 SetExecutionParameters2(ParameterValue[] Parameters, string ParameterLanguage)
		{
			return (ExecutionInfo2)Invoke("SetExecutionParameters2", new object[2]
			{
				Parameters,
				ParameterLanguage
			})[0];
		}

		public IAsyncResult BeginSetExecutionParameters2(ParameterValue[] Parameters, string ParameterLanguage, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("SetExecutionParameters2", new object[2]
			{
				Parameters,
				ParameterLanguage
			}, callback, asyncState);
		}

		public ExecutionInfo2 EndSetExecutionParameters2(IAsyncResult asyncResult)
		{
			return (ExecutionInfo2)EndInvoke(asyncResult)[0];
		}

		public void SetExecutionParameters2Async(ParameterValue[] Parameters, string ParameterLanguage)
		{
			SetExecutionParameters2Async(Parameters, ParameterLanguage, null);
		}

		public void SetExecutionParameters2Async(ParameterValue[] Parameters, string ParameterLanguage, object userState)
		{
			if (SetExecutionParameters2OperationCompleted == null)
			{
				SetExecutionParameters2OperationCompleted = OnSetExecutionParameters2OperationCompleted;
			}
			InvokeAsync("SetExecutionParameters2", new object[2]
			{
				Parameters,
				ParameterLanguage
			}, SetExecutionParameters2OperationCompleted, userState);
		}

		private void OnSetExecutionParameters2OperationCompleted(object arg)
		{
			if (this.SetExecutionParameters2Completed != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetExecutionParameters2Completed(this, new SetExecutionParameters2CompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/SetExecutionParameters3", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("executionInfo")]
		public ExecutionInfo3 SetExecutionParameters3(ParameterValue[] Parameters, string ParameterLanguage)
		{
			return (ExecutionInfo3)Invoke("SetExecutionParameters3", new object[2]
			{
				Parameters,
				ParameterLanguage
			})[0];
		}

		public IAsyncResult BeginSetExecutionParameters3(ParameterValue[] Parameters, string ParameterLanguage, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("SetExecutionParameters3", new object[2]
			{
				Parameters,
				ParameterLanguage
			}, callback, asyncState);
		}

		public ExecutionInfo3 EndSetExecutionParameters3(IAsyncResult asyncResult)
		{
			return (ExecutionInfo3)EndInvoke(asyncResult)[0];
		}

		public void SetExecutionParameters3Async(ParameterValue[] Parameters, string ParameterLanguage)
		{
			SetExecutionParameters3Async(Parameters, ParameterLanguage, null);
		}

		public void SetExecutionParameters3Async(ParameterValue[] Parameters, string ParameterLanguage, object userState)
		{
			if (SetExecutionParameters3OperationCompleted == null)
			{
				SetExecutionParameters3OperationCompleted = OnSetExecutionParameters3OperationCompleted;
			}
			InvokeAsync("SetExecutionParameters3", new object[2]
			{
				Parameters,
				ParameterLanguage
			}, SetExecutionParameters3OperationCompleted, userState);
		}

		private void OnSetExecutionParameters3OperationCompleted(object arg)
		{
			if (this.SetExecutionParameters3Completed != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetExecutionParameters3Completed(this, new SetExecutionParameters3CompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/ResetExecution", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("executionInfo")]
		public ExecutionInfo ResetExecution()
		{
			return (ExecutionInfo)Invoke("ResetExecution", new object[0])[0];
		}

		public IAsyncResult BeginResetExecution(AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("ResetExecution", new object[0], callback, asyncState);
		}

		public ExecutionInfo EndResetExecution(IAsyncResult asyncResult)
		{
			return (ExecutionInfo)EndInvoke(asyncResult)[0];
		}

		public void ResetExecutionAsync()
		{
			ResetExecutionAsync(null);
		}

		public void ResetExecutionAsync(object userState)
		{
			if (ResetExecutionOperationCompleted == null)
			{
				ResetExecutionOperationCompleted = OnResetExecutionOperationCompleted;
			}
			InvokeAsync("ResetExecution", new object[0], ResetExecutionOperationCompleted, userState);
		}

		private void OnResetExecutionOperationCompleted(object arg)
		{
			if (this.ResetExecutionCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ResetExecutionCompleted(this, new ResetExecutionCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/ResetExecution2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("executionInfo")]
		public ExecutionInfo2 ResetExecution2()
		{
			return (ExecutionInfo2)Invoke("ResetExecution2", new object[0])[0];
		}

		public IAsyncResult BeginResetExecution2(AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("ResetExecution2", new object[0], callback, asyncState);
		}

		public ExecutionInfo2 EndResetExecution2(IAsyncResult asyncResult)
		{
			return (ExecutionInfo2)EndInvoke(asyncResult)[0];
		}

		public void ResetExecution2Async()
		{
			ResetExecution2Async(null);
		}

		public void ResetExecution2Async(object userState)
		{
			if (ResetExecution2OperationCompleted == null)
			{
				ResetExecution2OperationCompleted = OnResetExecution2OperationCompleted;
			}
			InvokeAsync("ResetExecution2", new object[0], ResetExecution2OperationCompleted, userState);
		}

		private void OnResetExecution2OperationCompleted(object arg)
		{
			if (this.ResetExecution2Completed != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ResetExecution2Completed(this, new ResetExecution2CompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/ResetExecution3", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("executionInfo")]
		public ExecutionInfo3 ResetExecution3()
		{
			return (ExecutionInfo3)Invoke("ResetExecution3", new object[0])[0];
		}

		public IAsyncResult BeginResetExecution3(AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("ResetExecution3", new object[0], callback, asyncState);
		}

		public ExecutionInfo3 EndResetExecution3(IAsyncResult asyncResult)
		{
			return (ExecutionInfo3)EndInvoke(asyncResult)[0];
		}

		public void ResetExecution3Async()
		{
			ResetExecution3Async(null);
		}

		public void ResetExecution3Async(object userState)
		{
			if (ResetExecution3OperationCompleted == null)
			{
				ResetExecution3OperationCompleted = OnResetExecution3OperationCompleted;
			}
			InvokeAsync("ResetExecution3", new object[0], ResetExecution3OperationCompleted, userState);
		}

		private void OnResetExecution3OperationCompleted(object arg)
		{
			if (this.ResetExecution3Completed != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ResetExecution3Completed(this, new ResetExecution3CompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/Render", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("Result", DataType = "base64Binary")]
		public byte[] Render(string Format, string DeviceInfo, out string Extension, out string MimeType, out string Encoding, out Warning[] Warnings, out string[] StreamIds)
		{
			object[] array = Invoke("Render", new object[2]
			{
				Format,
				DeviceInfo
			});
			Extension = (string)array[1];
			MimeType = (string)array[2];
			Encoding = (string)array[3];
			Warnings = (Warning[])array[4];
			StreamIds = (string[])array[5];
			return (byte[])array[0];
		}

		public IAsyncResult BeginRender(string Format, string DeviceInfo, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("Render", new object[2]
			{
				Format,
				DeviceInfo
			}, callback, asyncState);
		}

		public byte[] EndRender(IAsyncResult asyncResult, out string Extension, out string MimeType, out string Encoding, out Warning[] Warnings, out string[] StreamIds)
		{
			object[] array = EndInvoke(asyncResult);
			Extension = (string)array[1];
			MimeType = (string)array[2];
			Encoding = (string)array[3];
			Warnings = (Warning[])array[4];
			StreamIds = (string[])array[5];
			return (byte[])array[0];
		}

		public void RenderAsync(string Format, string DeviceInfo)
		{
			RenderAsync(Format, DeviceInfo, null);
		}

		public void RenderAsync(string Format, string DeviceInfo, object userState)
		{
			if (RenderOperationCompleted == null)
			{
				RenderOperationCompleted = OnRenderOperationCompleted;
			}
			InvokeAsync("Render", new object[2]
			{
				Format,
				DeviceInfo
			}, RenderOperationCompleted, userState);
		}

		private void OnRenderOperationCompleted(object arg)
		{
			if (this.RenderCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.RenderCompleted(this, new RenderCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/Render2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("Result", DataType = "base64Binary")]
		public byte[] Render2(string Format, string DeviceInfo, PageCountMode PaginationMode, out string Extension, out string MimeType, out string Encoding, out Warning[] Warnings, out string[] StreamIds)
		{
			object[] array = Invoke("Render2", new object[3]
			{
				Format,
				DeviceInfo,
				PaginationMode
			});
			Extension = (string)array[1];
			MimeType = (string)array[2];
			Encoding = (string)array[3];
			Warnings = (Warning[])array[4];
			StreamIds = (string[])array[5];
			return (byte[])array[0];
		}

		public IAsyncResult BeginRender2(string Format, string DeviceInfo, PageCountMode PaginationMode, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("Render2", new object[3]
			{
				Format,
				DeviceInfo,
				PaginationMode
			}, callback, asyncState);
		}

		public byte[] EndRender2(IAsyncResult asyncResult, out string Extension, out string MimeType, out string Encoding, out Warning[] Warnings, out string[] StreamIds)
		{
			object[] array = EndInvoke(asyncResult);
			Extension = (string)array[1];
			MimeType = (string)array[2];
			Encoding = (string)array[3];
			Warnings = (Warning[])array[4];
			StreamIds = (string[])array[5];
			return (byte[])array[0];
		}

		public void Render2Async(string Format, string DeviceInfo, PageCountMode PaginationMode)
		{
			Render2Async(Format, DeviceInfo, PaginationMode, null);
		}

		public void Render2Async(string Format, string DeviceInfo, PageCountMode PaginationMode, object userState)
		{
			if (Render2OperationCompleted == null)
			{
				Render2OperationCompleted = OnRender2OperationCompleted;
			}
			InvokeAsync("Render2", new object[3]
			{
				Format,
				DeviceInfo,
				PaginationMode
			}, Render2OperationCompleted, userState);
		}

		private void OnRender2OperationCompleted(object arg)
		{
			if (this.Render2Completed != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.Render2Completed(this, new Render2CompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/DeliverReportItem", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void DeliverReportItem(string Format, string DeviceInfo, ExtensionSettings ExtensionSettings, string Description, string EventType, string MatchData)
		{
			Invoke("DeliverReportItem", new object[6]
			{
				Format,
				DeviceInfo,
				ExtensionSettings,
				Description,
				EventType,
				MatchData
			});
		}

		public IAsyncResult BeginDeliverReportItem(string Format, string DeviceInfo, ExtensionSettings ExtensionSettings, string Description, string EventType, string MatchData, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("DeliverReportItem", new object[6]
			{
				Format,
				DeviceInfo,
				ExtensionSettings,
				Description,
				EventType,
				MatchData
			}, callback, asyncState);
		}

		public void EndDeliverReportItem(IAsyncResult asyncResult)
		{
			EndInvoke(asyncResult);
		}

		public void DeliverReportItemAsync(string Format, string DeviceInfo, string Report, ExtensionSettings ExtensionSettings, string Description, string EventType, string MatchData)
		{
			DeliverReportItemAsync(Format, DeviceInfo, ExtensionSettings, Description, EventType, MatchData, null);
		}

		public void DeliverReportItemAsync(string Format, string DeviceInfo, ExtensionSettings ExtensionSettings, string Description, string EventType, string MatchData, object userState)
		{
			if (DeliverReportItemOperationCompleted == null)
			{
				DeliverReportItemOperationCompleted = OnDeliverReportItemOperationCompleted;
			}
			InvokeAsync("DeliverReportItem", new object[6]
			{
				Format,
				DeviceInfo,
				ExtensionSettings,
				Description,
				EventType,
				MatchData
			}, DeliverReportItemOperationCompleted, userState);
		}

		private void OnDeliverReportItemOperationCompleted(object arg)
		{
			if (this.DeliverReportItemCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.DeliverReportItemCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/RenderStream", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("Result", DataType = "base64Binary")]
		public byte[] RenderStream(string Format, string StreamID, string DeviceInfo, out string Encoding, out string MimeType)
		{
			object[] array = Invoke("RenderStream", new object[3]
			{
				Format,
				StreamID,
				DeviceInfo
			});
			Encoding = (string)array[1];
			MimeType = (string)array[2];
			return (byte[])array[0];
		}

		public IAsyncResult BeginRenderStream(string Format, string StreamID, string DeviceInfo, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("RenderStream", new object[3]
			{
				Format,
				StreamID,
				DeviceInfo
			}, callback, asyncState);
		}

		public byte[] EndRenderStream(IAsyncResult asyncResult, out string Encoding, out string MimeType)
		{
			object[] array = EndInvoke(asyncResult);
			Encoding = (string)array[1];
			MimeType = (string)array[2];
			return (byte[])array[0];
		}

		public void RenderStreamAsync(string Format, string StreamID, string DeviceInfo)
		{
			RenderStreamAsync(Format, StreamID, DeviceInfo, null);
		}

		public void RenderStreamAsync(string Format, string StreamID, string DeviceInfo, object userState)
		{
			if (RenderStreamOperationCompleted == null)
			{
				RenderStreamOperationCompleted = OnRenderStreamOperationCompleted;
			}
			InvokeAsync("RenderStream", new object[3]
			{
				Format,
				StreamID,
				DeviceInfo
			}, RenderStreamOperationCompleted, userState);
		}

		private void OnRenderStreamOperationCompleted(object arg)
		{
			if (this.RenderStreamCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.RenderStreamCompleted(this, new RenderStreamCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/GetExecutionInfo", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("executionInfo")]
		public ExecutionInfo GetExecutionInfo()
		{
			return (ExecutionInfo)Invoke("GetExecutionInfo", new object[0])[0];
		}

		public IAsyncResult BeginGetExecutionInfo(AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("GetExecutionInfo", new object[0], callback, asyncState);
		}

		public ExecutionInfo EndGetExecutionInfo(IAsyncResult asyncResult)
		{
			return (ExecutionInfo)EndInvoke(asyncResult)[0];
		}

		public void GetExecutionInfoAsync()
		{
			GetExecutionInfoAsync(null);
		}

		public void GetExecutionInfoAsync(object userState)
		{
			if (GetExecutionInfoOperationCompleted == null)
			{
				GetExecutionInfoOperationCompleted = OnGetExecutionInfoOperationCompleted;
			}
			InvokeAsync("GetExecutionInfo", new object[0], GetExecutionInfoOperationCompleted, userState);
		}

		private void OnGetExecutionInfoOperationCompleted(object arg)
		{
			if (this.GetExecutionInfoCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetExecutionInfoCompleted(this, new GetExecutionInfoCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/GetExecutionInfo2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("executionInfo")]
		public ExecutionInfo2 GetExecutionInfo2()
		{
			return (ExecutionInfo2)Invoke("GetExecutionInfo2", new object[0])[0];
		}

		public IAsyncResult BeginGetExecutionInfo2(AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("GetExecutionInfo2", new object[0], callback, asyncState);
		}

		public ExecutionInfo2 EndGetExecutionInfo2(IAsyncResult asyncResult)
		{
			return (ExecutionInfo2)EndInvoke(asyncResult)[0];
		}

		public void GetExecutionInfo2Async()
		{
			GetExecutionInfo2Async(null);
		}

		public void GetExecutionInfo2Async(object userState)
		{
			if (GetExecutionInfo2OperationCompleted == null)
			{
				GetExecutionInfo2OperationCompleted = OnGetExecutionInfo2OperationCompleted;
			}
			InvokeAsync("GetExecutionInfo2", new object[0], GetExecutionInfo2OperationCompleted, userState);
		}

		private void OnGetExecutionInfo2OperationCompleted(object arg)
		{
			if (this.GetExecutionInfo2Completed != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetExecutionInfo2Completed(this, new GetExecutionInfo2CompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/GetExecutionInfo3", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("executionInfo")]
		public ExecutionInfo3 GetExecutionInfo3()
		{
			return (ExecutionInfo3)Invoke("GetExecutionInfo3", new object[0])[0];
		}

		public IAsyncResult BeginGetExecutionInfo3(AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("GetExecutionInfo3", new object[0], callback, asyncState);
		}

		public ExecutionInfo3 EndGetExecutionInfo3(IAsyncResult asyncResult)
		{
			return (ExecutionInfo3)EndInvoke(asyncResult)[0];
		}

		public void GetExecutionInfo3Async()
		{
			GetExecutionInfo3Async(null);
		}

		public void GetExecutionInfo3Async(object userState)
		{
			if (GetExecutionInfo3OperationCompleted == null)
			{
				GetExecutionInfo3OperationCompleted = OnGetExecutionInfo3OperationCompleted;
			}
			InvokeAsync("GetExecutionInfo3", new object[0], GetExecutionInfo3OperationCompleted, userState);
		}

		private void OnGetExecutionInfo3OperationCompleted(object arg)
		{
			if (this.GetExecutionInfo3Completed != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetExecutionInfo3Completed(this, new GetExecutionInfo3CompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/GetDocumentMap", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("result")]
		public DocumentMapNode GetDocumentMap()
		{
			return (DocumentMapNode)Invoke("GetDocumentMap", new object[0])[0];
		}

		public IAsyncResult BeginGetDocumentMap(AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("GetDocumentMap", new object[0], callback, asyncState);
		}

		public DocumentMapNode EndGetDocumentMap(IAsyncResult asyncResult)
		{
			return (DocumentMapNode)EndInvoke(asyncResult)[0];
		}

		public void GetDocumentMapAsync()
		{
			GetDocumentMapAsync(null);
		}

		public void GetDocumentMapAsync(object userState)
		{
			if (GetDocumentMapOperationCompleted == null)
			{
				GetDocumentMapOperationCompleted = OnGetDocumentMapOperationCompleted;
			}
			InvokeAsync("GetDocumentMap", new object[0], GetDocumentMapOperationCompleted, userState);
		}

		private void OnGetDocumentMapOperationCompleted(object arg)
		{
			if (this.GetDocumentMapCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetDocumentMapCompleted(this, new GetDocumentMapCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.InOut)]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadDrillthroughTarget", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("ExecutionInfo")]
		public ExecutionInfo LoadDrillthroughTarget(string DrillthroughID)
		{
			return (ExecutionInfo)Invoke("LoadDrillthroughTarget", new object[1]
			{
				DrillthroughID
			})[0];
		}

		public IAsyncResult BeginLoadDrillthroughTarget(string DrillthroughID, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("LoadDrillthroughTarget", new object[1]
			{
				DrillthroughID
			}, callback, asyncState);
		}

		public ExecutionInfo EndLoadDrillthroughTarget(IAsyncResult asyncResult)
		{
			return (ExecutionInfo)EndInvoke(asyncResult)[0];
		}

		public void LoadDrillthroughTargetAsync(string DrillthroughID)
		{
			LoadDrillthroughTargetAsync(DrillthroughID, null);
		}

		public void LoadDrillthroughTargetAsync(string DrillthroughID, object userState)
		{
			if (LoadDrillthroughTargetOperationCompleted == null)
			{
				LoadDrillthroughTargetOperationCompleted = OnLoadDrillthroughTargetOperationCompleted;
			}
			InvokeAsync("LoadDrillthroughTarget", new object[1]
			{
				DrillthroughID
			}, LoadDrillthroughTargetOperationCompleted, userState);
		}

		private void OnLoadDrillthroughTargetOperationCompleted(object arg)
		{
			if (this.LoadDrillthroughTargetCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.LoadDrillthroughTargetCompleted(this, new LoadDrillthroughTargetCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.InOut)]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadDrillthroughTarget2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("ExecutionInfo")]
		public ExecutionInfo2 LoadDrillthroughTarget2(string DrillthroughID)
		{
			return (ExecutionInfo2)Invoke("LoadDrillthroughTarget2", new object[1]
			{
				DrillthroughID
			})[0];
		}

		public IAsyncResult BeginLoadDrillthroughTarget2(string DrillthroughID, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("LoadDrillthroughTarget2", new object[1]
			{
				DrillthroughID
			}, callback, asyncState);
		}

		public ExecutionInfo2 EndLoadDrillthroughTarget2(IAsyncResult asyncResult)
		{
			return (ExecutionInfo2)EndInvoke(asyncResult)[0];
		}

		public void LoadDrillthroughTarget2Async(string DrillthroughID)
		{
			LoadDrillthroughTarget2Async(DrillthroughID, null);
		}

		public void LoadDrillthroughTarget2Async(string DrillthroughID, object userState)
		{
			if (LoadDrillthroughTarget2OperationCompleted == null)
			{
				LoadDrillthroughTarget2OperationCompleted = OnLoadDrillthroughTarget2OperationCompleted;
			}
			InvokeAsync("LoadDrillthroughTarget2", new object[1]
			{
				DrillthroughID
			}, LoadDrillthroughTarget2OperationCompleted, userState);
		}

		private void OnLoadDrillthroughTarget2OperationCompleted(object arg)
		{
			if (this.LoadDrillthroughTarget2Completed != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.LoadDrillthroughTarget2Completed(this, new LoadDrillthroughTarget2CompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue", Direction = SoapHeaderDirection.InOut)]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LoadDrillthroughTarget3", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("ExecutionInfo")]
		public ExecutionInfo3 LoadDrillthroughTarget3(string DrillthroughID)
		{
			return (ExecutionInfo3)Invoke("LoadDrillthroughTarget3", new object[1]
			{
				DrillthroughID
			})[0];
		}

		public IAsyncResult BeginLoadDrillthroughTarget3(string DrillthroughID, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("LoadDrillthroughTarget3", new object[1]
			{
				DrillthroughID
			}, callback, asyncState);
		}

		public ExecutionInfo3 EndLoadDrillthroughTarget3(IAsyncResult asyncResult)
		{
			return (ExecutionInfo3)EndInvoke(asyncResult)[0];
		}

		public void LoadDrillthroughTarget3Async(string DrillthroughID)
		{
			LoadDrillthroughTarget3Async(DrillthroughID, null);
		}

		public void LoadDrillthroughTarget3Async(string DrillthroughID, object userState)
		{
			if (LoadDrillthroughTarget3OperationCompleted == null)
			{
				LoadDrillthroughTarget3OperationCompleted = OnLoadDrillthroughTarget3OperationCompleted;
			}
			InvokeAsync("LoadDrillthroughTarget3", new object[1]
			{
				DrillthroughID
			}, LoadDrillthroughTarget3OperationCompleted, userState);
		}

		private void OnLoadDrillthroughTarget3OperationCompleted(object arg)
		{
			if (this.LoadDrillthroughTarget3Completed != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.LoadDrillthroughTarget3Completed(this, new LoadDrillthroughTarget3CompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/ToggleItem", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("Found")]
		public bool ToggleItem(string ToggleID)
		{
			return (bool)Invoke("ToggleItem", new object[1]
			{
				ToggleID
			})[0];
		}

		public IAsyncResult BeginToggleItem(string ToggleID, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("ToggleItem", new object[1]
			{
				ToggleID
			}, callback, asyncState);
		}

		public bool EndToggleItem(IAsyncResult asyncResult)
		{
			return (bool)EndInvoke(asyncResult)[0];
		}

		public void ToggleItemAsync(string ToggleID)
		{
			ToggleItemAsync(ToggleID, null);
		}

		public void ToggleItemAsync(string ToggleID, object userState)
		{
			if (ToggleItemOperationCompleted == null)
			{
				ToggleItemOperationCompleted = OnToggleItemOperationCompleted;
			}
			InvokeAsync("ToggleItem", new object[1]
			{
				ToggleID
			}, ToggleItemOperationCompleted, userState);
		}

		private void OnToggleItemOperationCompleted(object arg)
		{
			if (this.ToggleItemCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ToggleItemCompleted(this, new ToggleItemCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/NavigateDocumentMap", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("PageNumber")]
		public int NavigateDocumentMap(string DocMapID)
		{
			return (int)Invoke("NavigateDocumentMap", new object[1]
			{
				DocMapID
			})[0];
		}

		public IAsyncResult BeginNavigateDocumentMap(string DocMapID, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("NavigateDocumentMap", new object[1]
			{
				DocMapID
			}, callback, asyncState);
		}

		public int EndNavigateDocumentMap(IAsyncResult asyncResult)
		{
			return (int)EndInvoke(asyncResult)[0];
		}

		public void NavigateDocumentMapAsync(string DocMapID)
		{
			NavigateDocumentMapAsync(DocMapID, null);
		}

		public void NavigateDocumentMapAsync(string DocMapID, object userState)
		{
			if (NavigateDocumentMapOperationCompleted == null)
			{
				NavigateDocumentMapOperationCompleted = OnNavigateDocumentMapOperationCompleted;
			}
			InvokeAsync("NavigateDocumentMap", new object[1]
			{
				DocMapID
			}, NavigateDocumentMapOperationCompleted, userState);
		}

		private void OnNavigateDocumentMapOperationCompleted(object arg)
		{
			if (this.NavigateDocumentMapCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.NavigateDocumentMapCompleted(this, new NavigateDocumentMapCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/NavigateBookmark", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("PageNumber")]
		public int NavigateBookmark(string BookmarkID, out string UniqueName)
		{
			object[] array = Invoke("NavigateBookmark", new object[1]
			{
				BookmarkID
			});
			UniqueName = (string)array[1];
			return (int)array[0];
		}

		public IAsyncResult BeginNavigateBookmark(string BookmarkID, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("NavigateBookmark", new object[1]
			{
				BookmarkID
			}, callback, asyncState);
		}

		public int EndNavigateBookmark(IAsyncResult asyncResult, out string UniqueName)
		{
			object[] array = EndInvoke(asyncResult);
			UniqueName = (string)array[1];
			return (int)array[0];
		}

		public void NavigateBookmarkAsync(string BookmarkID)
		{
			NavigateBookmarkAsync(BookmarkID, null);
		}

		public void NavigateBookmarkAsync(string BookmarkID, object userState)
		{
			if (NavigateBookmarkOperationCompleted == null)
			{
				NavigateBookmarkOperationCompleted = OnNavigateBookmarkOperationCompleted;
			}
			InvokeAsync("NavigateBookmark", new object[1]
			{
				BookmarkID
			}, NavigateBookmarkOperationCompleted, userState);
		}

		private void OnNavigateBookmarkOperationCompleted(object arg)
		{
			if (this.NavigateBookmarkCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.NavigateBookmarkCompleted(this, new NavigateBookmarkCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/FindString", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("PageNumber")]
		public int FindString(int StartPage, int EndPage, string FindValue)
		{
			return (int)Invoke("FindString", new object[3]
			{
				StartPage,
				EndPage,
				FindValue
			})[0];
		}

		public IAsyncResult BeginFindString(int StartPage, int EndPage, string FindValue, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("FindString", new object[3]
			{
				StartPage,
				EndPage,
				FindValue
			}, callback, asyncState);
		}

		public int EndFindString(IAsyncResult asyncResult)
		{
			return (int)EndInvoke(asyncResult)[0];
		}

		public void FindStringAsync(int StartPage, int EndPage, string FindValue)
		{
			FindStringAsync(StartPage, EndPage, FindValue, null);
		}

		public void FindStringAsync(int StartPage, int EndPage, string FindValue, object userState)
		{
			if (FindStringOperationCompleted == null)
			{
				FindStringOperationCompleted = OnFindStringOperationCompleted;
			}
			InvokeAsync("FindString", new object[3]
			{
				StartPage,
				EndPage,
				FindValue
			}, FindStringOperationCompleted, userState);
		}

		private void OnFindStringOperationCompleted(object arg)
		{
			if (this.FindStringCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.FindStringCompleted(this, new FindStringCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/Sort", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("PageNumber")]
		public int Sort(string SortItem, SortDirectionEnum Direction, bool Clear, out string ReportItem, out int NumPages)
		{
			object[] array = Invoke("Sort", new object[3]
			{
				SortItem,
				Direction,
				Clear
			});
			ReportItem = (string)array[1];
			NumPages = (int)array[2];
			return (int)array[0];
		}

		public IAsyncResult BeginSort(string SortItem, SortDirectionEnum Direction, bool Clear, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("Sort", new object[3]
			{
				SortItem,
				Direction,
				Clear
			}, callback, asyncState);
		}

		public int EndSort(IAsyncResult asyncResult, out string ReportItem, out int NumPages)
		{
			object[] array = EndInvoke(asyncResult);
			ReportItem = (string)array[1];
			NumPages = (int)array[2];
			return (int)array[0];
		}

		public void SortAsync(string SortItem, SortDirectionEnum Direction, bool Clear)
		{
			SortAsync(SortItem, Direction, Clear, null);
		}

		public void SortAsync(string SortItem, SortDirectionEnum Direction, bool Clear, object userState)
		{
			if (SortOperationCompleted == null)
			{
				SortOperationCompleted = OnSortOperationCompleted;
			}
			InvokeAsync("Sort", new object[3]
			{
				SortItem,
				Direction,
				Clear
			}, SortOperationCompleted, userState);
		}

		private void OnSortOperationCompleted(object arg)
		{
			if (this.SortCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SortCompleted(this, new SortCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/Sort2", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("PageNumber")]
		public int Sort2(string SortItem, SortDirectionEnum Direction, bool Clear, PageCountMode PaginationMode, out string ReportItem, out ExecutionInfo2 ExecutionInfo)
		{
			object[] array = Invoke("Sort2", new object[4]
			{
				SortItem,
				Direction,
				Clear,
				PaginationMode
			});
			ReportItem = (string)array[1];
			ExecutionInfo = (ExecutionInfo2)array[2];
			return (int)array[0];
		}

		public IAsyncResult BeginSort2(string SortItem, SortDirectionEnum Direction, bool Clear, PageCountMode PaginationMode, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("Sort2", new object[4]
			{
				SortItem,
				Direction,
				Clear,
				PaginationMode
			}, callback, asyncState);
		}

		public int EndSort2(IAsyncResult asyncResult, out string ReportItem, out ExecutionInfo2 ExecutionInfo)
		{
			object[] array = EndInvoke(asyncResult);
			ReportItem = (string)array[1];
			ExecutionInfo = (ExecutionInfo2)array[2];
			return (int)array[0];
		}

		public void Sort2Async(string SortItem, SortDirectionEnum Direction, bool Clear, PageCountMode PaginationMode)
		{
			Sort2Async(SortItem, Direction, Clear, PaginationMode, null);
		}

		public void Sort2Async(string SortItem, SortDirectionEnum Direction, bool Clear, PageCountMode PaginationMode, object userState)
		{
			if (Sort2OperationCompleted == null)
			{
				Sort2OperationCompleted = OnSort2OperationCompleted;
			}
			InvokeAsync("Sort2", new object[4]
			{
				SortItem,
				Direction,
				Clear,
				PaginationMode
			}, Sort2OperationCompleted, userState);
		}

		private void OnSort2OperationCompleted(object arg)
		{
			if (this.Sort2Completed != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.Sort2Completed(this, new Sort2CompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExecutionHeaderValue")]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/Sort3", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("PageNumber")]
		public int Sort3(string SortItem, SortDirectionEnum Direction, bool Clear, PageCountMode PaginationMode, out string ReportItem, out ExecutionInfo3 ExecutionInfo)
		{
			object[] array = Invoke("Sort3", new object[4]
			{
				SortItem,
				Direction,
				Clear,
				PaginationMode
			});
			ReportItem = (string)array[1];
			ExecutionInfo = (ExecutionInfo3)array[2];
			return (int)array[0];
		}

		public IAsyncResult BeginSort3(string SortItem, SortDirectionEnum Direction, bool Clear, PageCountMode PaginationMode, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("Sort3", new object[4]
			{
				SortItem,
				Direction,
				Clear,
				PaginationMode
			}, callback, asyncState);
		}

		public int EndSort3(IAsyncResult asyncResult, out string ReportItem, out ExecutionInfo3 ExecutionInfo)
		{
			object[] array = EndInvoke(asyncResult);
			ReportItem = (string)array[1];
			ExecutionInfo = (ExecutionInfo3)array[2];
			return (int)array[0];
		}

		public void Sort3Async(string SortItem, SortDirectionEnum Direction, bool Clear, PageCountMode PaginationMode)
		{
			Sort3Async(SortItem, Direction, Clear, PaginationMode, null);
		}

		public void Sort3Async(string SortItem, SortDirectionEnum Direction, bool Clear, PageCountMode PaginationMode, object userState)
		{
			if (Sort3OperationCompleted == null)
			{
				Sort3OperationCompleted = OnSort3OperationCompleted;
			}
			InvokeAsync("Sort3", new object[4]
			{
				SortItem,
				Direction,
				Clear,
				PaginationMode
			}, Sort3OperationCompleted, userState);
		}

		private void OnSort3OperationCompleted(object arg)
		{
			if (this.Sort3Completed != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.Sort3Completed(this, new Sort3CompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/GetRenderResource", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("Result", DataType = "base64Binary")]
		public byte[] GetRenderResource(string Format, string DeviceInfo, out string MimeType)
		{
			object[] array = Invoke("GetRenderResource", new object[2]
			{
				Format,
				DeviceInfo
			});
			MimeType = (string)array[1];
			return (byte[])array[0];
		}

		public IAsyncResult BeginGetRenderResource(string Format, string DeviceInfo, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("GetRenderResource", new object[2]
			{
				Format,
				DeviceInfo
			}, callback, asyncState);
		}

		public byte[] EndGetRenderResource(IAsyncResult asyncResult, out string MimeType)
		{
			object[] array = EndInvoke(asyncResult);
			MimeType = (string)array[1];
			return (byte[])array[0];
		}

		public void GetRenderResourceAsync(string Format, string DeviceInfo)
		{
			GetRenderResourceAsync(Format, DeviceInfo, null);
		}

		public void GetRenderResourceAsync(string Format, string DeviceInfo, object userState)
		{
			if (GetRenderResourceOperationCompleted == null)
			{
				GetRenderResourceOperationCompleted = OnGetRenderResourceOperationCompleted;
			}
			InvokeAsync("GetRenderResource", new object[2]
			{
				Format,
				DeviceInfo
			}, GetRenderResourceOperationCompleted, userState);
		}

		private void OnGetRenderResourceOperationCompleted(object arg)
		{
			if (this.GetRenderResourceCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetRenderResourceCompleted(this, new GetRenderResourceCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("TrustedUserHeaderValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/ListRenderingExtensions", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlArray("Extensions")]
		public Extension[] ListRenderingExtensions()
		{
			return (Extension[])Invoke("ListRenderingExtensions", new object[0])[0];
		}

		public IAsyncResult BeginListRenderingExtensions(AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("ListRenderingExtensions", new object[0], callback, asyncState);
		}

		public Extension[] EndListRenderingExtensions(IAsyncResult asyncResult)
		{
			return (Extension[])EndInvoke(asyncResult)[0];
		}

		public void ListRenderingExtensionsAsync()
		{
			ListRenderingExtensionsAsync(null);
		}

		public void ListRenderingExtensionsAsync(object userState)
		{
			if (ListRenderingExtensionsOperationCompleted == null)
			{
				ListRenderingExtensionsOperationCompleted = OnListRenderingExtensionsOperationCompleted;
			}
			InvokeAsync("ListRenderingExtensions", new object[0], ListRenderingExtensionsOperationCompleted, userState);
		}

		private void OnListRenderingExtensionsOperationCompleted(object arg)
		{
			if (this.ListRenderingExtensionsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ListRenderingExtensionsCompleted(this, new ListRenderingExtensionsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/LogonUser", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void LogonUser(string userName, string password, string authority)
		{
			Invoke("LogonUser", new object[3]
			{
				userName,
				password,
				authority
			});
		}

		public IAsyncResult BeginLogonUser(string userName, string password, string authority, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("LogonUser", new object[3]
			{
				userName,
				password,
				authority
			}, callback, asyncState);
		}

		public void EndLogonUser(IAsyncResult asyncResult)
		{
			EndInvoke(asyncResult);
		}

		public void LogonUserAsync(string userName, string password, string authority)
		{
			LogonUserAsync(userName, password, authority, null);
		}

		public void LogonUserAsync(string userName, string password, string authority, object userState)
		{
			if (LogonUserOperationCompleted == null)
			{
				LogonUserOperationCompleted = OnLogonUserOperationCompleted;
			}
			InvokeAsync("LogonUser", new object[3]
			{
				userName,
				password,
				authority
			}, LogonUserOperationCompleted, userState);
		}

		private void OnLogonUserOperationCompleted(object arg)
		{
			if (this.LogonUserCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.LogonUserCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerInfoHeaderValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/Logoff", RequestNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", ResponseNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void Logoff()
		{
			Invoke("Logoff", new object[0]);
		}

		public IAsyncResult BeginLogoff(AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("Logoff", new object[0], callback, asyncState);
		}

		public void EndLogoff(IAsyncResult asyncResult)
		{
			EndInvoke(asyncResult);
		}

		public void LogoffAsync()
		{
			LogoffAsync(null);
		}

		public void LogoffAsync(object userState)
		{
			if (LogoffOperationCompleted == null)
			{
				LogoffOperationCompleted = OnLogoffOperationCompleted;
			}
			InvokeAsync("Logoff", new object[0], LogoffOperationCompleted, userState);
		}

		private void OnLogoffOperationCompleted(object arg)
		{
			if (this.LogoffCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.LogoffCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		public new void CancelAsync(object userState)
		{
			base.CancelAsync(userState);
		}
	}
}

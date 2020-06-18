using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	internal sealed class RSTrace
	{
		internal enum TraceComponents
		{
			Library,
			ConfigManager,
			WebServer,
			NtService,
			Session,
			BufferedResponse,
			RunningRequests,
			DbPolling,
			Notification,
			Provider,
			Schedule,
			Subscription,
			Security,
			ServiceController,
			DbCleanup,
			Cache,
			Chunks,
			ExtensionFactory,
			RunningJobs,
			Processing,
			ReportRendering,
			HtmlViewer,
			DataExtension,
			EmailExtension,
			ImageRenderer,
			ExcelRenderer,
			PreviewServer,
			ResourceUtilities,
			ReportPreview,
			UI,
			Crypto,
			SemanticModelGenerator,
			SemanticQueryEngine,
			AppDomainManager,
			HttpRuntime,
			WcfRuntime,
			AlertingRuntime,
			UndoManager,
			DataManager,
			DataStructureManager,
			Controls,
			PowerView,
			QueryDesign,
			MonitoredScope,
			CloudReportServer,
			ExecutionLog,
			DataShapeQueryTranslation,
			InfoNav,
			ReportServerWebApp,
			Thread
		}

		private class DefaultRSTraceInternal : IRSTraceInternal
		{
			public string TraceDirectory => string.Empty;

			public string CurrentTraceFilePath => string.Empty;

			public bool BufferOutput
			{
				get
				{
					return false;
				}
				set
				{
				}
			}

			public bool IsTraceInitialized => true;

			public void ClearBuffer()
			{
			}

			public void WriteBuffer()
			{
			}

			public string GetDefaultTraceLevel()
			{
				return "0";
			}

			public void Trace(string componentName, string message)
			{
			}

			public void Trace(string componentName, string format, params object[] arg)
			{
			}

			public void Trace(TraceLevel traceLevel, string componentName, string message)
			{
			}

			public void TraceWithDetails(TraceLevel traceLevel, string componentName, string message, string details)
			{
			}

			public void Trace(TraceLevel traceLevel, string componentName, string format, params object[] arg)
			{
			}

			public void TraceException(TraceLevel traceLevel, string componentName, string message)
			{
			}

			public void TraceWithNoEventLog(TraceLevel traceLevel, string componentName, string format, params object[] arg)
			{
			}

			public void Fail(string componentName)
			{
				throw new InvalidOperationException(componentName);
			}

			public void Fail(string componentName, string message)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "({0}): {1}", componentName, message));
			}

			public bool GetTraceLevel(string componentName, out TraceLevel componentTraceLevel)
			{
				componentTraceLevel = TraceLevel.Off;
				return false;
			}
		}

		public sealed class WriteOnce
		{
			private Hashtable m_traceWriteOnce = new Hashtable();

			private static Hashtable s_traceWriteOnce = new Hashtable();

			public bool TraceWritten(string text)
			{
				if (text != null && !m_traceWriteOnce.Contains(text))
				{
					m_traceWriteOnce.Add(text, null);
					lock (s_traceWriteOnce.SyncRoot)
					{
						if (!s_traceWriteOnce.Contains(text))
						{
							s_traceWriteOnce.Add(text, null);
							return false;
						}
					}
				}
				return true;
			}
		}

		private static RSTrace m_CryptoTrace;

		private static RSTrace m_ResourceUtilTrace;

		private static RSTrace m_CatalogTrace;

		private static RSTrace m_ConfigManagerTrace;

		private static RSTrace m_WebServerTrace;

		private static RSTrace m_WcfRuntimeTrace;

		private static RSTrace m_AlertingRuntimeTrace;

		private static RSTrace m_NtServiceTrace;

		private static RSTrace m_SessionTrace;

		private static RSTrace m_BufRespTrace;

		private static RSTrace m_RunningRequestsTrace;

		private static RSTrace m_DbPollingTrace;

		private static RSTrace m_NotificationTrace;

		private static RSTrace m_ProviderTrace;

		private static RSTrace m_ScheduleTrace;

		private static RSTrace m_SubscriptionTrace;

		private static RSTrace m_SecurityTrace;

		private static RSTrace m_ServiceControllerTrace;

		private static RSTrace m_CleanupTrace;

		private static RSTrace m_CacheTrace;

		private static RSTrace m_ChunkTrace;

		private static RSTrace m_ExtTrace;

		private static RSTrace m_RunningJobsTrace;

		private static RSTrace m_ProcessingTrace;

		private static RSTrace m_RenderingTrace;

		private static RSTrace m_ViewerTrace;

		private static RSTrace m_DataExtTrace;

		private static RSTrace m_RSWebAppTracer;

		private static RSTrace m_EmailExtensionTrace;

		private static RSTrace m_ImageRendererTrace;

		private static RSTrace m_ExcelRendererTrace;

		private static RSTrace m_PreviewServerTrace;

		private static RSTrace m_ReportPreviewTrace;

		private static RSTrace m_UITrace;

		private static RSTrace m_SMGTrace;

		private static RSTrace m_SQETrace;

		private static RSTrace m_AppDomainManagerTrace;

		private static RSTrace m_HttpRuntimeTrace;

		private static RSTrace m_UndoManagerTrace;

		private static RSTrace m_DataManagerTrace;

		private static RSTrace m_DataStructureManagerTrace;

		private static RSTrace m_QueryDesignTrace;

		private static RSTrace m_ControlsTrace;

		private static RSTrace m_ClientEventTracer;

		private static RSTrace m_ThreadTracer;

		private static RSTrace m_MonitoredScope;

		private static RSTrace m_dsqtTracer;

		private static RSTrace m_infoNavTracer;

		private readonly string m_ComponentName;

		private TraceLevel m_componentTraceLevel;

		private const string m_allComponents = "all";

		private static IRSTraceInternal m_traceInternal = new DefaultRSTraceInternal();

		private static IRSTraceInternalWithDynamicLevel m_alternateTraceInternal;

		private static TraceSwitch m_rsTraceSwitch;

		public static RSTrace CryptoTrace
		{
			get
			{
				RSTrace cryptoTrace = m_CryptoTrace;
				if (cryptoTrace == null)
				{
					cryptoTrace = new RSTrace(TraceComponents.Crypto.ToString());
					Interlocked.CompareExchange(ref m_CryptoTrace, cryptoTrace, null);
				}
				return m_CryptoTrace;
			}
		}

		public static RSTrace ResourceUtilTrace
		{
			get
			{
				RSTrace resourceUtilTrace = m_ResourceUtilTrace;
				if (resourceUtilTrace == null)
				{
					resourceUtilTrace = new RSTrace(TraceComponents.ResourceUtilities.ToString());
					Interlocked.CompareExchange(ref m_ResourceUtilTrace, resourceUtilTrace, null);
				}
				return m_ResourceUtilTrace;
			}
		}

		public static RSTrace CatalogTrace
		{
			get
			{
				RSTrace catalogTrace = m_CatalogTrace;
				if (catalogTrace == null)
				{
					catalogTrace = new RSTrace(TraceComponents.Library.ToString());
					Interlocked.CompareExchange(ref m_CatalogTrace, catalogTrace, null);
				}
				return m_CatalogTrace;
			}
		}

		public static RSTrace ConfigManagerTracer
		{
			get
			{
				RSTrace configManagerTrace = m_ConfigManagerTrace;
				if (configManagerTrace == null)
				{
					configManagerTrace = new RSTrace(TraceComponents.ConfigManager.ToString());
					Interlocked.CompareExchange(ref m_ConfigManagerTrace, configManagerTrace, null);
				}
				return m_ConfigManagerTrace;
			}
		}

		public static RSTrace WebServerTracer
		{
			get
			{
				RSTrace webServerTrace = m_WebServerTrace;
				if (webServerTrace == null)
				{
					webServerTrace = new RSTrace(TraceComponents.WebServer.ToString());
					Interlocked.CompareExchange(ref m_WebServerTrace, webServerTrace, null);
				}
				return m_WebServerTrace;
			}
		}

		public static RSTrace WcfRuntimeTracer
		{
			get
			{
				RSTrace wcfRuntimeTrace = m_WcfRuntimeTrace;
				if (wcfRuntimeTrace == null)
				{
					wcfRuntimeTrace = new RSTrace(TraceComponents.WcfRuntime.ToString());
					Interlocked.CompareExchange(ref m_WcfRuntimeTrace, wcfRuntimeTrace, null);
				}
				return m_WcfRuntimeTrace;
			}
		}

		public static RSTrace AlertingRuntimeTracer
		{
			get
			{
				RSTrace alertingRuntimeTrace = m_AlertingRuntimeTrace;
				if (alertingRuntimeTrace == null)
				{
					alertingRuntimeTrace = new RSTrace(TraceComponents.AlertingRuntime.ToString());
					Interlocked.CompareExchange(ref m_AlertingRuntimeTrace, alertingRuntimeTrace, null);
				}
				return m_AlertingRuntimeTrace;
			}
		}

		public static RSTrace NtServiceTracer
		{
			get
			{
				RSTrace ntServiceTrace = m_NtServiceTrace;
				if (ntServiceTrace == null)
				{
					ntServiceTrace = new RSTrace(TraceComponents.NtService.ToString());
					Interlocked.CompareExchange(ref m_NtServiceTrace, ntServiceTrace, null);
				}
				return m_NtServiceTrace;
			}
		}

		public static RSTrace SessionTrace
		{
			get
			{
				RSTrace sessionTrace = m_SessionTrace;
				if (sessionTrace == null)
				{
					sessionTrace = new RSTrace(TraceComponents.Session.ToString());
					Interlocked.CompareExchange(ref m_SessionTrace, sessionTrace, null);
				}
				return m_SessionTrace;
			}
		}

		public static RSTrace BufferedResponseTracer
		{
			get
			{
				RSTrace bufRespTrace = m_BufRespTrace;
				if (bufRespTrace == null)
				{
					bufRespTrace = new RSTrace(TraceComponents.BufferedResponse.ToString());
					Interlocked.CompareExchange(ref m_BufRespTrace, bufRespTrace, null);
				}
				return m_BufRespTrace;
			}
		}

		public static RSTrace RunningRequestsTracer
		{
			get
			{
				RSTrace runningRequestsTrace = m_RunningRequestsTrace;
				if (runningRequestsTrace == null)
				{
					runningRequestsTrace = new RSTrace(TraceComponents.RunningRequests.ToString());
					Interlocked.CompareExchange(ref m_RunningRequestsTrace, runningRequestsTrace, null);
				}
				return m_RunningRequestsTrace;
			}
		}

		public static RSTrace DbPollingTracer
		{
			get
			{
				RSTrace dbPollingTrace = m_DbPollingTrace;
				if (dbPollingTrace == null)
				{
					dbPollingTrace = new RSTrace(TraceComponents.DbPolling.ToString());
					Interlocked.CompareExchange(ref m_DbPollingTrace, dbPollingTrace, null);
				}
				return m_DbPollingTrace;
			}
		}

		public static RSTrace NotificationTracer
		{
			get
			{
				RSTrace notificationTrace = m_NotificationTrace;
				if (notificationTrace == null)
				{
					notificationTrace = new RSTrace(TraceComponents.Notification.ToString());
					Interlocked.CompareExchange(ref m_NotificationTrace, notificationTrace, null);
				}
				return m_NotificationTrace;
			}
		}

		public static RSTrace ProviderTracer
		{
			get
			{
				RSTrace providerTrace = m_ProviderTrace;
				if (providerTrace == null)
				{
					providerTrace = new RSTrace(TraceComponents.Provider.ToString());
					Interlocked.CompareExchange(ref m_ProviderTrace, providerTrace, null);
				}
				return m_ProviderTrace;
			}
		}

		public static RSTrace ScheduleTracer
		{
			get
			{
				RSTrace scheduleTrace = m_ScheduleTrace;
				if (scheduleTrace == null)
				{
					scheduleTrace = new RSTrace(TraceComponents.Schedule.ToString());
					Interlocked.CompareExchange(ref m_ScheduleTrace, scheduleTrace, null);
				}
				return m_ScheduleTrace;
			}
		}

		public static RSTrace SubscriptionTracer
		{
			get
			{
				RSTrace subscriptionTrace = m_SubscriptionTrace;
				if (subscriptionTrace == null)
				{
					subscriptionTrace = new RSTrace(TraceComponents.Subscription.ToString());
					Interlocked.CompareExchange(ref m_SubscriptionTrace, subscriptionTrace, null);
				}
				return m_SubscriptionTrace;
			}
		}

		public static RSTrace SecurityTracer
		{
			get
			{
				RSTrace securityTrace = m_SecurityTrace;
				if (securityTrace == null)
				{
					securityTrace = new RSTrace(TraceComponents.Security.ToString());
					Interlocked.CompareExchange(ref m_SecurityTrace, securityTrace, null);
				}
				return m_SecurityTrace;
			}
		}

		public static RSTrace ServiceControllerTracer
		{
			get
			{
				RSTrace serviceControllerTrace = m_ServiceControllerTrace;
				if (serviceControllerTrace == null)
				{
					serviceControllerTrace = new RSTrace(TraceComponents.ServiceController.ToString());
					Interlocked.CompareExchange(ref m_ServiceControllerTrace, serviceControllerTrace, null);
				}
				return m_ServiceControllerTrace;
			}
		}

		public static RSTrace CleanupTracer
		{
			get
			{
				RSTrace cleanupTrace = m_CleanupTrace;
				if (cleanupTrace == null)
				{
					cleanupTrace = new RSTrace(TraceComponents.DbCleanup.ToString());
					Interlocked.CompareExchange(ref m_CleanupTrace, cleanupTrace, null);
				}
				return m_CleanupTrace;
			}
		}

		public static RSTrace CacheTracer
		{
			get
			{
				RSTrace cacheTrace = m_CacheTrace;
				if (cacheTrace == null)
				{
					cacheTrace = new RSTrace(TraceComponents.Cache.ToString());
					Interlocked.CompareExchange(ref m_CacheTrace, cacheTrace, null);
				}
				return m_CacheTrace;
			}
		}

		public static RSTrace ChunkTracer
		{
			get
			{
				RSTrace chunkTrace = m_ChunkTrace;
				if (chunkTrace == null)
				{
					chunkTrace = new RSTrace(TraceComponents.Chunks.ToString());
					Interlocked.CompareExchange(ref m_ChunkTrace, chunkTrace, null);
				}
				return m_ChunkTrace;
			}
		}

		public static RSTrace ExtensionFactoryTracer
		{
			get
			{
				RSTrace extTrace = m_ExtTrace;
				if (extTrace == null)
				{
					extTrace = new RSTrace(TraceComponents.ExtensionFactory.ToString());
					Interlocked.CompareExchange(ref m_ExtTrace, extTrace, null);
				}
				return m_ExtTrace;
			}
		}

		public static RSTrace RunningJobsTrace
		{
			get
			{
				RSTrace runningJobsTrace = m_RunningJobsTrace;
				if (runningJobsTrace == null)
				{
					runningJobsTrace = new RSTrace(TraceComponents.RunningJobs.ToString());
					Interlocked.CompareExchange(ref m_RunningJobsTrace, runningJobsTrace, null);
				}
				return m_RunningJobsTrace;
			}
		}

		public static RSTrace ProcessingTracer
		{
			get
			{
				RSTrace processingTrace = m_ProcessingTrace;
				if (processingTrace == null)
				{
					processingTrace = new RSTrace(TraceComponents.Processing.ToString());
					Interlocked.CompareExchange(ref m_ProcessingTrace, processingTrace, null);
				}
				return m_ProcessingTrace;
			}
		}

		public static RSTrace RenderingTracer
		{
			get
			{
				RSTrace renderingTrace = m_RenderingTrace;
				if (renderingTrace == null)
				{
					renderingTrace = new RSTrace(TraceComponents.ReportRendering.ToString());
					Interlocked.CompareExchange(ref m_RenderingTrace, renderingTrace, null);
				}
				return m_RenderingTrace;
			}
		}

		public static RSTrace ViewerTracer
		{
			get
			{
				RSTrace viewerTrace = m_ViewerTrace;
				if (viewerTrace == null)
				{
					viewerTrace = new RSTrace(TraceComponents.ConfigManager.ToString());
					Interlocked.CompareExchange(ref m_ViewerTrace, viewerTrace, null);
				}
				return m_ViewerTrace;
			}
		}

		public static RSTrace DataExtensionTracer
		{
			get
			{
				RSTrace dataExtTrace = m_DataExtTrace;
				if (dataExtTrace == null)
				{
					dataExtTrace = new RSTrace(TraceComponents.DataExtension.ToString());
					Interlocked.CompareExchange(ref m_DataExtTrace, dataExtTrace, null);
				}
				return m_DataExtTrace;
			}
		}

		public static RSTrace RSWebAppTracer
		{
			get
			{
				RSTrace rSWebAppTracer = m_RSWebAppTracer;
				if (rSWebAppTracer == null)
				{
					rSWebAppTracer = new RSTrace(TraceComponents.ReportServerWebApp.ToString());
					Interlocked.CompareExchange(ref m_RSWebAppTracer, rSWebAppTracer, null);
				}
				return m_RSWebAppTracer;
			}
		}

		public static RSTrace EmailExtensionTracer
		{
			get
			{
				RSTrace emailExtensionTrace = m_EmailExtensionTrace;
				if (emailExtensionTrace == null)
				{
					emailExtensionTrace = new RSTrace(TraceComponents.EmailExtension.ToString());
					Interlocked.CompareExchange(ref m_EmailExtensionTrace, emailExtensionTrace, null);
				}
				return m_EmailExtensionTrace;
			}
		}

		public static RSTrace ImageRendererTracer
		{
			get
			{
				RSTrace imageRendererTrace = m_ImageRendererTrace;
				if (imageRendererTrace == null)
				{
					imageRendererTrace = new RSTrace(TraceComponents.ImageRenderer.ToString());
					Interlocked.CompareExchange(ref m_ImageRendererTrace, imageRendererTrace, null);
				}
				return m_ImageRendererTrace;
			}
		}

		public static RSTrace ExcelRendererTracer
		{
			get
			{
				RSTrace excelRendererTrace = m_ExcelRendererTrace;
				if (excelRendererTrace == null)
				{
					excelRendererTrace = new RSTrace(TraceComponents.ExcelRenderer.ToString());
					Interlocked.CompareExchange(ref m_ExcelRendererTrace, excelRendererTrace, null);
				}
				return m_ExcelRendererTrace;
			}
		}

		public static RSTrace PreviewServerTracer
		{
			get
			{
				RSTrace previewServerTrace = m_PreviewServerTrace;
				if (previewServerTrace == null)
				{
					previewServerTrace = new RSTrace(TraceComponents.PreviewServer.ToString());
					Interlocked.CompareExchange(ref m_PreviewServerTrace, previewServerTrace, null);
				}
				return m_PreviewServerTrace;
			}
		}

		public static RSTrace ReportPreviewTracer
		{
			get
			{
				RSTrace reportPreviewTrace = m_ReportPreviewTrace;
				if (reportPreviewTrace == null)
				{
					reportPreviewTrace = new RSTrace(TraceComponents.ReportPreview.ToString());
					Interlocked.CompareExchange(ref m_ReportPreviewTrace, reportPreviewTrace, null);
				}
				return m_ReportPreviewTrace;
			}
		}

		public static RSTrace UITracer
		{
			get
			{
				RSTrace uITrace = m_UITrace;
				if (uITrace == null)
				{
					uITrace = new RSTrace(TraceComponents.UI.ToString());
					Interlocked.CompareExchange(ref m_UITrace, uITrace, null);
				}
				return m_UITrace;
			}
		}

		public static RSTrace SMGTracer
		{
			get
			{
				RSTrace sMGTrace = m_SMGTrace;
				if (sMGTrace == null)
				{
					sMGTrace = new RSTrace(TraceComponents.SemanticModelGenerator.ToString());
					Interlocked.CompareExchange(ref m_SMGTrace, sMGTrace, null);
				}
				return m_SMGTrace;
			}
		}

		public static RSTrace SQETracer
		{
			get
			{
				RSTrace sQETrace = m_SQETrace;
				if (sQETrace == null)
				{
					sQETrace = new RSTrace(TraceComponents.SemanticQueryEngine.ToString());
					Interlocked.CompareExchange(ref m_SQETrace, sQETrace, null);
				}
				return m_SQETrace;
			}
		}

		public static RSTrace AppDomainManagerTracer
		{
			get
			{
				RSTrace appDomainManagerTrace = m_AppDomainManagerTrace;
				if (appDomainManagerTrace == null)
				{
					appDomainManagerTrace = new RSTrace(TraceComponents.AppDomainManager.ToString());
					Interlocked.CompareExchange(ref m_AppDomainManagerTrace, appDomainManagerTrace, null);
				}
				return m_AppDomainManagerTrace;
			}
		}

		public static RSTrace HttpRuntimeTracer
		{
			get
			{
				RSTrace httpRuntimeTrace = m_HttpRuntimeTrace;
				if (httpRuntimeTrace == null)
				{
					httpRuntimeTrace = new RSTrace(TraceComponents.HttpRuntime.ToString());
					Interlocked.CompareExchange(ref m_HttpRuntimeTrace, httpRuntimeTrace, null);
				}
				return m_HttpRuntimeTrace;
			}
		}

		public static RSTrace UndoManager
		{
			get
			{
				RSTrace undoManagerTrace = m_UndoManagerTrace;
				if (undoManagerTrace == null)
				{
					undoManagerTrace = new RSTrace(TraceComponents.UndoManager.ToString());
					Interlocked.CompareExchange(ref m_UndoManagerTrace, undoManagerTrace, null);
				}
				return m_UndoManagerTrace;
			}
		}

		public static RSTrace DataManager
		{
			get
			{
				RSTrace dataManagerTrace = m_DataManagerTrace;
				if (dataManagerTrace == null)
				{
					dataManagerTrace = new RSTrace(TraceComponents.DataManager.ToString());
					Interlocked.CompareExchange(ref m_DataManagerTrace, dataManagerTrace, null);
				}
				return m_DataManagerTrace;
			}
		}

		public static RSTrace DataStructureManager
		{
			get
			{
				RSTrace dataStructureManagerTrace = m_DataStructureManagerTrace;
				if (dataStructureManagerTrace == null)
				{
					dataStructureManagerTrace = new RSTrace(TraceComponents.DataStructureManager.ToString());
					Interlocked.CompareExchange(ref m_DataStructureManagerTrace, dataStructureManagerTrace, null);
				}
				return m_DataStructureManagerTrace;
			}
		}

		public static RSTrace QueryDesign
		{
			get
			{
				RSTrace queryDesignTrace = m_QueryDesignTrace;
				if (queryDesignTrace == null)
				{
					queryDesignTrace = new RSTrace(TraceComponents.QueryDesign.ToString());
					Interlocked.CompareExchange(ref m_QueryDesignTrace, queryDesignTrace, null);
				}
				return m_QueryDesignTrace;
			}
		}

		public static RSTrace Controls
		{
			get
			{
				RSTrace controlsTrace = m_ControlsTrace;
				if (controlsTrace == null)
				{
					controlsTrace = new RSTrace(TraceComponents.Controls.ToString());
					Interlocked.CompareExchange(ref m_ControlsTrace, controlsTrace, null);
				}
				return m_ControlsTrace;
			}
		}

		public static RSTrace ClientEventTracer
		{
			get
			{
				RSTrace clientEventTracer = m_ClientEventTracer;
				if (clientEventTracer == null)
				{
					clientEventTracer = new RSTrace(TraceComponents.PowerView.ToString());
					Interlocked.CompareExchange(ref m_ClientEventTracer, clientEventTracer, null);
				}
				return m_ClientEventTracer;
			}
		}

		public static RSTrace ThreadTracer
		{
			get
			{
				RSTrace threadTracer = m_ThreadTracer;
				if (threadTracer == null)
				{
					threadTracer = new RSTrace(TraceComponents.Thread.ToString());
					Interlocked.CompareExchange(ref m_ThreadTracer, threadTracer, null);
				}
				return m_ThreadTracer;
			}
		}

		public static RSTrace MonitoredScope
		{
			get
			{
				RSTrace monitoredScope = m_MonitoredScope;
				if (monitoredScope == null)
				{
					monitoredScope = new RSTrace(TraceComponents.MonitoredScope.ToString());
					Interlocked.CompareExchange(ref m_MonitoredScope, monitoredScope, null);
				}
				return m_MonitoredScope;
			}
		}

		public static RSTrace DsqtTracer
		{
			get
			{
				RSTrace dsqtTracer = m_dsqtTracer;
				if (dsqtTracer == null)
				{
					dsqtTracer = new RSTrace(TraceComponents.DataShapeQueryTranslation.ToString());
					Interlocked.CompareExchange(ref m_dsqtTracer, dsqtTracer, null);
				}
				return m_dsqtTracer;
			}
		}

		public static RSTrace InfoNavTracer
		{
			get
			{
				RSTrace infoNavTracer = m_infoNavTracer;
				if (infoNavTracer == null)
				{
					infoNavTracer = new RSTrace(TraceComponents.InfoNav.ToString());
					Interlocked.CompareExchange(ref m_infoNavTracer, infoNavTracer, null);
				}
				return m_infoNavTracer;
			}
		}

		public bool TraceInfo => IsTraceLevelEnabled(TraceLevel.Info);

		public bool TraceError => IsTraceLevelEnabled(TraceLevel.Error);

		public bool TraceWarning => IsTraceLevelEnabled(TraceLevel.Warning);

		public bool TraceVerbose => IsTraceLevelEnabled(TraceLevel.Verbose);

		internal static bool IsTraceInitialized => m_traceInternal.IsTraceInitialized;

		internal string TraceFileName => m_traceInternal.CurrentTraceFilePath;

		public string TraceDirectory => m_traceInternal.TraceDirectory;

		public bool BufferOutput
		{
			get
			{
				return m_traceInternal.BufferOutput;
			}
			set
			{
				m_traceInternal.BufferOutput = value;
			}
		}

		public TraceSwitch RSTraceSwitch
		{
			get
			{
				if (m_rsTraceSwitch == null)
				{
					lock (this)
					{
						string defaultTraceLevel = m_traceInternal.GetDefaultTraceLevel();
						m_rsTraceSwitch = new TraceSwitch("DefaultTraceSwitch", "Default Trace Switch", defaultTraceLevel);
					}
				}
				return m_rsTraceSwitch;
			}
			set
			{
				lock (this)
				{
					m_rsTraceSwitch = value;
				}
			}
		}

		internal RSTrace(string componentName)
		{
			m_ComponentName = componentName.ToLowerInvariant();
			m_componentTraceLevel = GetTraceLevel(m_ComponentName);
		}

		public void Trace(string message)
		{
			m_traceInternal.Trace(m_ComponentName, message);
			if (m_alternateTraceInternal != null)
			{
				m_alternateTraceInternal.Trace(m_ComponentName, message);
			}
		}

		public void Trace(TraceLevel traceLevel, string message)
		{
			m_traceInternal.Trace(traceLevel, m_ComponentName, message);
			if (m_alternateTraceInternal != null)
			{
				m_alternateTraceInternal.Trace(traceLevel, m_ComponentName, message);
			}
		}

		public void TraceWithDetails(TraceLevel traceLevel, string message, string details)
		{
			m_traceInternal.TraceWithDetails(traceLevel, m_ComponentName, message, details);
			if (m_alternateTraceInternal != null)
			{
				m_alternateTraceInternal.TraceWithDetails(traceLevel, m_ComponentName, message, details);
			}
		}

		public void TraceException(TraceLevel traceLevel, string message)
		{
			m_traceInternal.TraceException(traceLevel, m_ComponentName, message);
			if (m_alternateTraceInternal != null)
			{
				m_alternateTraceInternal.TraceException(traceLevel, m_ComponentName, message);
			}
		}

		public void Trace(string format, params object[] arg)
		{
			m_traceInternal.Trace(m_ComponentName, format, arg);
			if (m_alternateTraceInternal != null)
			{
				m_alternateTraceInternal.Trace(m_ComponentName, format, arg);
			}
		}

		public void Trace(TraceLevel traceLevel, string format, params object[] arg)
		{
			m_traceInternal.Trace(traceLevel, m_ComponentName, format, arg);
			if (m_alternateTraceInternal != null)
			{
				m_alternateTraceInternal.Trace(traceLevel, m_ComponentName, format, arg);
			}
		}

		public void TraceException(TraceLevel traceLevel, string format, params object[] arg)
		{
			if (IsTraceLevelEnabled(traceLevel))
			{
				string message = string.Format(CultureInfo.InvariantCulture, format, arg);
				m_traceInternal.TraceException(traceLevel, m_ComponentName, message);
				if (m_alternateTraceInternal != null)
				{
					m_alternateTraceInternal.TraceException(traceLevel, m_ComponentName, message);
				}
			}
		}

		public void TraceWithNoEventLog(TraceLevel traceLevel, string format, params object[] arg)
		{
			m_traceInternal.TraceWithNoEventLog(traceLevel, m_ComponentName, format, arg);
			if (m_alternateTraceInternal != null)
			{
				m_alternateTraceInternal.TraceWithNoEventLog(traceLevel, m_ComponentName, format, arg);
			}
		}

		public void Assert(bool condition)
		{
			if (condition)
			{
				return;
			}
			if (m_alternateTraceInternal != null)
			{
				try
				{
					m_alternateTraceInternal.Fail(m_ComponentName);
				}
				catch
				{
					m_traceInternal.Fail(m_ComponentName);
				}
			}
			m_traceInternal.Fail(m_ComponentName);
		}

		public void Assert(bool condition, string message)
		{
			if (condition)
			{
				return;
			}
			if (m_alternateTraceInternal != null)
			{
				try
				{
					m_alternateTraceInternal.Fail(m_ComponentName, message);
				}
				catch
				{
					m_traceInternal.Fail(m_ComponentName, message);
				}
			}
			m_traceInternal.Fail(m_ComponentName, message);
		}

		public void Assert(bool condition, string message, params object[] args)
		{
			if (!condition)
			{
				Assert(condition, string.Format(CultureInfo.InvariantCulture, message, args));
			}
		}

		[Conditional("DEBUG")]
		public void DebugAssert(bool condition)
		{
			Assert(condition);
		}

		[Conditional("DEBUG")]
		public void DebugAssert(string message)
		{
			Assert(condition: false, message);
		}

		[Conditional("DEBUG")]
		public void DebugAssert(bool condition, string message)
		{
			Assert(condition, message);
		}

		[Conditional("DEBUG")]
		public void DebugAssert(bool condition, string message, object arg1)
		{
			Assert(condition, message, arg1);
		}

		private static TraceLevel GetTraceLevel(string componentName)
		{
			TraceLevel componentTraceLevel = TraceLevel.Error;
			if (m_traceInternal != null && !m_traceInternal.GetTraceLevel(componentName, out componentTraceLevel) && !m_traceInternal.GetTraceLevel("all", out componentTraceLevel) && int.TryParse(m_traceInternal.GetDefaultTraceLevel(), NumberStyles.None, CultureInfo.InvariantCulture, out int result) && result >= 0 && result <= 3)
			{
				componentTraceLevel = (TraceLevel)result;
			}
			return componentTraceLevel;
		}

		public bool IsTraceLevelEnabled(TraceLevel level)
		{
			if (m_traceInternal is IRSTraceInternalWithDynamicLevel && m_traceInternal.GetTraceLevel(m_ComponentName, out TraceLevel componentTraceLevel))
			{
				m_componentTraceLevel = componentTraceLevel;
			}
			if (level <= m_componentTraceLevel)
			{
				return true;
			}
			if (m_alternateTraceInternal != null && m_alternateTraceInternal.GetTraceLevel(m_ComponentName, out TraceLevel componentTraceLevel2))
			{
				return level <= componentTraceLevel2;
			}
			return false;
		}

		public void ClearBuffer()
		{
			m_traceInternal.ClearBuffer();
		}

		public void WriteBuffer()
		{
			m_traceInternal.WriteBuffer();
		}

		public static void SetTrace(IRSTraceInternal trace)
		{
			if (trace == null)
			{
				throw new ArgumentNullException("trace");
			}
			m_traceInternal = trace;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		internal static IRSTraceInternal GetTrace()
		{
			return m_traceInternal;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		internal static IRSTraceInternal GetAlternateTrace()
		{
			return m_alternateTraceInternal;
		}

		public static void SetAlternateTrace(IRSTraceInternalWithDynamicLevel alternateTrace)
		{
			m_alternateTraceInternal = alternateTrace;
		}
	}
}

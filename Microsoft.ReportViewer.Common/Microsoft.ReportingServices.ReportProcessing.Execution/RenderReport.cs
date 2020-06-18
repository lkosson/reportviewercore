using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading;

namespace Microsoft.ReportingServices.ReportProcessing.Execution
{
	internal abstract class RenderReport
	{
		private readonly ProcessingContext m_publicProcessingContext;

		private readonly RenderingContext m_publicRenderingContext;

		protected abstract ProcessingEngine RunningProcessingEngine
		{
			get;
		}

		protected virtual bool IsSnapshotReprocessing => false;

		internal ProcessingContext PublicProcessingContext => m_publicProcessingContext;

		internal RenderingContext PublicRenderingContext => m_publicRenderingContext;

		protected string ReportName => PublicProcessingContext.ReportContext.ItemName;

		protected NameValueCollection RenderingParameters => PublicRenderingContext.ReportContext.RSRequestParameters.RenderingParameters;

		public RenderReport(ProcessingContext pc, RenderingContext rc)
		{
			m_publicProcessingContext = pc;
			m_publicRenderingContext = rc;
		}

		public OnDemandProcessingResult Execute(IRenderingExtension newRenderer)
		{
			ExecutionLogContext executionLogContext = new ExecutionLogContext(m_publicProcessingContext.JobContext);
			executionLogContext.StartProcessingTimer();
			ProcessingErrorContext processingErrorContext = new ProcessingErrorContext();
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				bool eventInfoChanged = false;
				bool renderingInfoChanged = false;
				UserProfileState userProfileState = UserProfileState.None;
				Hashtable renderProperties = PublicRenderingContext.GetRenderProperties(IsSnapshotReprocessing);
				NameValueCollection reportServerParameters = FormServerParameterCollection(PublicRenderingContext.ReportContext.RSRequestParameters.CatalogParameters);
				PrepareForExecution();
				ProcessReport(processingErrorContext, executionLogContext, ref userProfileState);
				Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext = null;
				try
				{
					Microsoft.ReportingServices.OnDemandReportRendering.Report report = PrepareROM(out odpRenderingContext);
					executionLogContext.StartRenderingTimer();
					renderingInfoChanged = InvokeRenderer(newRenderer, report, reportServerParameters, RenderingParameters, PublicRenderingContext.ReportContext.RSRequestParameters.BrowserCapabilities, ref renderProperties, PublicProcessingContext.CreateStreamCallback);
					UpdateServerTotalPages(newRenderer, ref renderProperties);
					UpdateEventInfo(odpRenderingContext, PublicRenderingContext, ref eventInfoChanged);
				}
				catch (ReportProcessing.DataCacheUnavailableException)
				{
					throw;
				}
				catch (ReportRenderingException rex)
				{
					ReportProcessing.HandleRenderingException(rex);
				}
				catch (RSException)
				{
					throw;
				}
				catch (Exception ex3)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(ex3))
					{
						throw;
					}
					throw new UnhandledReportRenderingException(ex3);
				}
				finally
				{
					FinallyBlockForProcessingAndRendering(odpRenderingContext, executionLogContext);
				}
				CleanupSuccessfulProcessing(processingErrorContext);
				return ConstructProcessingResult(eventInfoChanged, renderProperties, processingErrorContext, userProfileState, renderingInfoChanged, executionLogContext);
			}
			catch (ReportProcessing.DataCacheUnavailableException)
			{
				throw;
			}
			catch (RSException)
			{
				CleanupForException();
				throw;
			}
			catch (Exception innerException)
			{
				CleanupForException();
				throw new ReportProcessingException(innerException, processingErrorContext.Messages);
			}
			finally
			{
				FinalCleanup();
				ReportProcessing.UpdateHostingEnvironment(processingErrorContext, PublicProcessingContext.ReportContext, executionLogContext, RunningProcessingEngine, PublicProcessingContext.JobContext);
				if (currentCulture != null)
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
				}
			}
		}

		protected virtual void CleanupSuccessfulProcessing(ProcessingErrorContext errorContext)
		{
		}

		protected virtual bool InvokeRenderer(IRenderingExtension renderer, Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			return renderer.Render(report, reportServerParameters, deviceInfo, clientCapabilities, ref renderProperties, createAndRegisterStream);
		}

		protected abstract void ProcessReport(ProcessingErrorContext errorContext, ExecutionLogContext executionLogContext, ref UserProfileState userProfileState);

		protected abstract void PrepareForExecution();

		protected abstract Microsoft.ReportingServices.OnDemandReportRendering.Report PrepareROM(out Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext);

		protected abstract OnDemandProcessingResult ConstructProcessingResult(bool eventInfoChanged, Hashtable renderProperties, ProcessingErrorContext errorContext, UserProfileState userProfileState, bool renderingInfoChanged, ExecutionLogContext executionLogContext);

		protected abstract void FinalCleanup();

		protected virtual void CleanupForException()
		{
		}

		private void UpdateEventInfo(Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext, RenderingContext rc, ref bool eventInfoChanged)
		{
			UpdateEventInfoInSnapshot();
			eventInfoChanged |= odpRenderingContext.EventInfoChanged;
			if (eventInfoChanged)
			{
				rc.EventInfo = odpRenderingContext.EventInfo;
			}
		}

		protected virtual void UpdateEventInfoInSnapshot()
		{
		}

		private void FinallyBlockForProcessingAndRendering(Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext, ExecutionLogContext executionLogContext)
		{
			odpRenderingContext?.CloseRenderingChunkManager();
			executionLogContext.StopRenderingTimer();
		}

		protected int GetNumberOfPages(Hashtable renderProperties)
		{
			int result = 0;
			if (renderProperties != null)
			{
				object obj = renderProperties["TotalPages"];
				if (obj != null && obj is int)
				{
					result = (int)obj;
				}
			}
			return result;
		}

		protected PaginationMode GetUpdatedPaginationMode(Hashtable renderProperties, PaginationMode clientPaginationMode)
		{
			try
			{
				if (renderProperties != null)
				{
					object obj = renderProperties["UpdatedPaginationMode"];
					if (obj != null)
					{
						return (PaginationMode)obj;
					}
				}
			}
			catch (InvalidCastException)
			{
			}
			return PaginationMode.Estimate;
		}

		protected void UpdateServerTotalPages(IRenderingExtension renderer, ref Hashtable renderProperties)
		{
			if (renderProperties != null && renderer != null && !(renderer is ITotalPages))
			{
				renderProperties["TotalPages"] = 0;
				renderProperties["UpdatedPaginationMode"] = PaginationMode.Estimate;
			}
		}

		private NameValueCollection FormServerParameterCollection(NameValueCollection serverParams)
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			if (serverParams == null)
			{
				return nameValueCollection;
			}
			CheckAndAddServerParam(serverParams, "Command", nameValueCollection);
			CheckAndAddServerParam(serverParams, "Format", nameValueCollection);
			CheckAndAddServerParam(serverParams, "SessionID", nameValueCollection);
			CheckAndAddServerParam(serverParams, "ShowHideToggle", nameValueCollection);
			CheckAndAddServerParam(serverParams, "ImageID", nameValueCollection);
			CheckAndAddServerParam(serverParams, "Snapshot", nameValueCollection);
			return nameValueCollection;
		}

		private void CheckAndAddServerParam(NameValueCollection src, string paramName, NameValueCollection dest)
		{
			string[] values = src.GetValues(paramName);
			if (values != null && values.Length != 0)
			{
				dest.Add(paramName, values[0]);
			}
		}

		protected void ValidateReportParameters()
		{
			bool satisfiable = true;
			if (!PublicProcessingContext.Parameters.ValuesAreValid(out satisfiable, throwOnUnsatisfiable: true))
			{
				throw new ReportProcessingException(ErrorCode.rsParametersNotSpecified);
			}
		}
	}
}

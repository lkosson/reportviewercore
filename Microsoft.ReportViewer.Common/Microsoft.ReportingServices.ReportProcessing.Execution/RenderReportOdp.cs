using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing.Execution
{
	internal abstract class RenderReportOdp : RenderReport
	{
		protected OnDemandProcessingContext m_odpContext;

		protected Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot m_odpReportSnapshot;

		private readonly IConfiguration m_configuration;

		protected OnDemandProcessingContext OdpContext => m_odpContext;

		protected override ProcessingEngine RunningProcessingEngine => ProcessingEngine.OnDemandEngine;

		protected IConfiguration Configuration => m_configuration;

		public RenderReportOdp(ProcessingContext pc, RenderingContext rc, IConfiguration configuration)
			: base(pc, rc)
		{
			m_configuration = configuration;
		}

		protected override Microsoft.ReportingServices.OnDemandReportRendering.Report PrepareROM(out Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext)
		{
			odpRenderingContext = new Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext(base.PublicRenderingContext.Format, m_odpReportSnapshot, base.PublicRenderingContext.EventInfo, m_odpContext);
			return new Microsoft.ReportingServices.OnDemandReportRendering.Report(m_odpReportSnapshot.Report, m_odpReportSnapshot.ReportInstance, odpRenderingContext, base.ReportName, base.PublicRenderingContext.ReportDescription);
		}

		protected override void CleanupSuccessfulProcessing(ProcessingErrorContext errorContext)
		{
			ReportProcessing.CleanupOnDemandProcessing(m_odpContext, resetGroupTreeStorage: true);
		}

		protected override OnDemandProcessingResult ConstructProcessingResult(bool eventInfoChanged, Hashtable renderProperties, ProcessingErrorContext errorContext, UserProfileState userProfileState, bool renderingInfoChanged, ExecutionLogContext executionLogContext)
		{
			return new FullOnDemandProcessingResult(m_odpReportSnapshot, m_odpContext.OdpMetadata.OdpChunkManager, m_odpContext.OdpMetadata.SnapshotHasChanged, base.PublicProcessingContext.ChunkFactory, base.PublicProcessingContext.Parameters, m_odpReportSnapshot.Report.EvaluateAutoRefresh(null, m_odpContext), GetNumberOfPages(renderProperties), errorContext.Messages, eventInfoChanged, base.PublicRenderingContext.EventInfo, GetUpdatedPaginationMode(renderProperties, base.PublicRenderingContext.ClientPaginationMode), base.PublicProcessingContext.ChunkFactory.ReportProcessingFlags, m_odpContext.HasUserProfileState, executionLogContext);
		}

		protected override void FinalCleanup()
		{
			if (m_odpContext != null)
			{
				m_odpContext.FreeAllResources();
			}
		}

		protected override void CleanupForException()
		{
			ReportProcessing.RequestErrorGroupTreeCleanup(m_odpContext);
		}

		protected override void UpdateEventInfoInSnapshot()
		{
			Global.Tracer.Assert(m_odpReportSnapshot != null, "Snapshot must exist for ODP Engine");
			if (m_odpContext.NewSortFilterEventInfo != null && m_odpContext.NewSortFilterEventInfo.Count > 0)
			{
				m_odpReportSnapshot.SortFilterEventInfo = m_odpContext.NewSortFilterEventInfo;
			}
			else
			{
				m_odpReportSnapshot.SortFilterEventInfo = null;
			}
		}
	}
}

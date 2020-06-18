using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing.Execution
{
	internal abstract class RenderReportYukon : RenderReport
	{
		protected ChunkManager.RenderingChunkManager m_chunkManager;

		protected ReportProcessing.ProcessingContext m_context;

		protected Microsoft.ReportingServices.ReportRendering.RenderingContext m_renderingContext;

		protected ReportSnapshot m_reportSnapshot;

		private readonly ReportProcessing m_processing;

		protected override ProcessingEngine RunningProcessingEngine => ProcessingEngine.YukonEngine;

		protected ReportProcessing Processing => m_processing;

		protected Uri ReportUri => base.PublicRenderingContext.ReportUri;

		public RenderReportYukon(ProcessingContext pc, RenderingContext rc, ReportProcessing processing)
			: base(pc, rc)
		{
			m_processing = processing;
		}

		protected override Microsoft.ReportingServices.OnDemandReportRendering.Report PrepareROM(out Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext)
		{
			odpRenderingContext = new Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext(base.PublicRenderingContext.Format, m_reportSnapshot, base.PublicProcessingContext.ChunkFactory, base.PublicRenderingContext.EventInfo);
			return new Microsoft.ReportingServices.OnDemandReportRendering.Report(m_reportSnapshot.Report, m_reportSnapshot.ReportInstance, m_renderingContext, odpRenderingContext, base.ReportName, base.PublicRenderingContext.ReportDescription);
		}

		protected override void FinalCleanup()
		{
			if (m_chunkManager != null)
			{
				m_chunkManager.Close();
			}
		}

		protected override OnDemandProcessingResult ConstructProcessingResult(bool eventInfoChanged, Hashtable renderProperties, ProcessingErrorContext errorContext, UserProfileState userProfileState, bool renderingInfoChanged, ExecutionLogContext executionLogContext)
		{
			return new YukonProcessingResult(m_reportSnapshot, m_context.ChunkManager, base.PublicProcessingContext.ChunkFactory, base.PublicProcessingContext.Parameters, m_reportSnapshot.Report.AutoRefresh, GetNumberOfPages(renderProperties), errorContext.Messages, renderingInfoChanged: true, m_renderingContext.RenderingInfoManager, eventInfoChanged, base.PublicRenderingContext.EventInfo, GetUpdatedPaginationMode(renderProperties, base.PublicRenderingContext.ClientPaginationMode), base.PublicProcessingContext.ChunkFactory.ReportProcessingFlags, userProfileState | m_renderingContext.UsedUserProfileState, executionLogContext);
		}
	}
}

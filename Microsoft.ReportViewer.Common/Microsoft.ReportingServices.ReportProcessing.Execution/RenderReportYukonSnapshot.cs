using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportRendering;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing.Execution
{
	internal class RenderReportYukonSnapshot : RenderReportYukon
	{
		protected virtual bool IsRenderStream => false;

		public RenderReportYukonSnapshot(ProcessingContext pc, RenderingContext rc, ReportProcessing processing)
			: base(pc, rc, processing)
		{
		}

		protected override void ProcessReport(ProcessingErrorContext errorContext, ExecutionLogContext executionLogContext, ref UserProfileState userProfileState)
		{
			ChunkFactoryAdapter @object = new ChunkFactoryAdapter(base.PublicProcessingContext.ChunkFactory);
			m_reportSnapshot = ReportProcessing.DeserializeReportSnapshot(@object.GetReportChunk, @object.CreateReportChunk, base.PublicProcessingContext.GetResourceCallback, base.PublicRenderingContext, base.PublicProcessingContext.DataProtection, out Hashtable instanceObjects, out Hashtable definitionObjects, out IntermediateFormatReader.State declarationsRead, out bool isOldSnapshot);
			Global.Tracer.Assert(m_reportSnapshot != null, "(null != reportSnapshot)");
			Global.Tracer.Assert(m_reportSnapshot.Report != null, "(null != reportSnapshot.Report)");
			Global.Tracer.Assert(m_reportSnapshot.ReportInstance != null, "(null != reportSnapshot.ReportInstance)");
			m_chunkManager = new ChunkManager.RenderingChunkManager(@object.GetReportChunk, instanceObjects, definitionObjects, declarationsRead, m_reportSnapshot.Report.IntermediateFormatVersion);
			base.Processing.ProcessShowHideToggle(base.PublicRenderingContext.ShowHideToggle, m_reportSnapshot, base.PublicRenderingContext.EventInfo, m_chunkManager, out bool showHideInformationChanged, out EventInformation newOverrideInformation);
			if (showHideInformationChanged)
			{
				base.PublicRenderingContext.EventInfo = newOverrideInformation;
			}
			m_renderingContext = new Microsoft.ReportingServices.ReportRendering.RenderingContext(retrieveRenderingInfo: IsRenderStream || !isOldSnapshot, reportSnapshot: m_reportSnapshot, rendererID: base.PublicRenderingContext.Format, executionTime: m_reportSnapshot.ExecutionTime, embeddedImages: m_reportSnapshot.Report.EmbeddedImages, imageStreamNames: m_reportSnapshot.Report.ImageStreamNames, eventInfo: base.PublicRenderingContext.EventInfo, reportContext: base.PublicRenderingContext.ReportContext, contextUri: base.PublicRenderingContext.ReportUri, reportParameters: base.RenderingParameters, getChunkCallback: @object.GetReportChunk, chunkManager: m_chunkManager, getResourceCallback: base.PublicProcessingContext.GetResourceCallback, getChunkMimeType: @object.GetChunkMimeType, storeServerParameters: base.PublicRenderingContext.StoreServerParametersCallback, allowUserProfileState: base.PublicRenderingContext.AllowUserProfileState, reportRuntimeSetup: base.PublicRenderingContext.ReportRuntimeSetup, jobContext: base.PublicProcessingContext.JobContext, dataProtection: base.PublicProcessingContext.DataProtection);
		}

		protected override void PrepareForExecution()
		{
		}

		protected override void CleanupSuccessfulProcessing(ProcessingErrorContext errorContext)
		{
			if (!IsRenderStream)
			{
				errorContext.Combine(m_reportSnapshot.Warnings);
			}
		}

		protected override OnDemandProcessingResult ConstructProcessingResult(bool eventInfoChanged, Hashtable renderProperties, ProcessingErrorContext errorContext, UserProfileState userProfileState, bool renderingInfoChanged, ExecutionLogContext executionLogContext)
		{
			ReportInstanceInfo reportInstanceInfo = (ReportInstanceInfo)m_reportSnapshot.ReportInstance.GetInstanceInfo(m_renderingContext.ChunkManager);
			return new YukonProcessingResult(renderingInfoChanged, base.PublicProcessingContext.ChunkFactory, m_reportSnapshot.HasShowHide, m_renderingContext.RenderingInfoManager, eventInfoChanged, base.PublicRenderingContext.EventInfo, reportInstanceInfo.Parameters, errorContext.Messages, m_reportSnapshot.Report.AutoRefresh, GetNumberOfPages(renderProperties), GetUpdatedPaginationMode(renderProperties, base.PublicRenderingContext.ClientPaginationMode), base.PublicProcessingContext.ChunkFactory.ReportProcessingFlags, m_renderingContext.UsedUserProfileState, executionLogContext);
		}
	}
}

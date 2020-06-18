using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing.Execution
{
	internal class RenderReportYukonReprocessSnapshot : RenderReportYukon
	{
		private readonly IChunkFactory m_originalSnapshotChunks;

		protected override bool IsSnapshotReprocessing => true;

		public RenderReportYukonReprocessSnapshot(ProcessingContext pc, RenderingContext rc, ReportProcessing processing, IChunkFactory originalSnapshotChunks)
			: base(pc, rc, processing)
		{
			m_originalSnapshotChunks = originalSnapshotChunks;
		}

		protected override void ProcessReport(ProcessingErrorContext errorContext, ExecutionLogContext executionLogContext, ref UserProfileState userProfileState)
		{
			ChunkFactoryAdapter @object = new ChunkFactoryAdapter(base.PublicProcessingContext.ChunkFactory);
			ChunkFactoryAdapter object2 = new ChunkFactoryAdapter(m_originalSnapshotChunks);
			Hashtable definitionObjects = null;
			DateTime executionTime;
			Report report = ReportProcessing.DeserializeReportFromSnapshot(object2.GetReportChunk, out executionTime, out definitionObjects);
			m_reportSnapshot = base.Processing.ProcessReport(report, base.PublicProcessingContext, snapshotProcessing: true, processWithCachedData: false, object2.GetReportChunk, errorContext, executionTime, null, out m_context, out userProfileState);
			Global.Tracer.Assert(m_context != null, "(null != context)");
			m_chunkManager = new ChunkManager.RenderingChunkManager(@object.GetReportChunk, null, definitionObjects, null, report.IntermediateFormatVersion);
			m_renderingContext = new Microsoft.ReportingServices.ReportRendering.RenderingContext(m_reportSnapshot, base.PublicRenderingContext.Format, executionTime, report.EmbeddedImages, report.ImageStreamNames, base.PublicRenderingContext.EventInfo, base.PublicProcessingContext.ReportContext, base.ReportUri, null, @object.GetReportChunk, m_chunkManager, base.PublicProcessingContext.GetResourceCallback, @object.GetChunkMimeType, base.PublicRenderingContext.StoreServerParametersCallback, retrieveRenderingInfo: false, base.PublicProcessingContext.AllowUserProfileState, base.PublicRenderingContext.ReportRuntimeSetup, base.PublicProcessingContext.JobContext, base.PublicProcessingContext.DataProtection);
		}

		protected override void PrepareForExecution()
		{
			ValidateReportParameters();
		}
	}
}

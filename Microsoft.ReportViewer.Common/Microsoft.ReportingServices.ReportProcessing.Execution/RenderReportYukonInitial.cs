using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing.Execution
{
	internal class RenderReportYukonInitial : RenderReportYukon
	{
		private readonly DateTime m_executionTimeStamp;

		private readonly IChunkFactory m_yukonCompiledDefinition;

		public RenderReportYukonInitial(ProcessingContext pc, RenderingContext rc, DateTime executionTimeStamp, ReportProcessing processing, IChunkFactory yukonCompiledDefinition)
			: base(pc, rc, processing)
		{
			m_executionTimeStamp = executionTimeStamp;
			m_yukonCompiledDefinition = yukonCompiledDefinition;
		}

		protected override void PrepareForExecution()
		{
			ValidateReportParameters();
			ReportProcessing.CheckReportCredentialsAndConnectionUserDependency(base.PublicProcessingContext);
		}

		protected override void ProcessReport(ProcessingErrorContext errorContext, ExecutionLogContext executionLogContext, ref UserProfileState userProfileState)
		{
			ChunkFactoryAdapter @object = new ChunkFactoryAdapter(m_yukonCompiledDefinition);
			ChunkFactoryAdapter object2 = new ChunkFactoryAdapter(base.PublicProcessingContext.ChunkFactory);
			Hashtable definitionObjects = null;
			Report report = ReportProcessing.DeserializeReport(@object.GetReportChunk, out definitionObjects);
			m_reportSnapshot = base.Processing.ProcessReport(report, base.PublicProcessingContext, snapshotProcessing: false, processWithCachedData: false, object2.GetReportChunk, errorContext, m_executionTimeStamp, null, out m_context, out userProfileState);
			Global.Tracer.Assert(m_context != null, "(null != m_context)");
			executionLogContext.AddLegacyDataProcessingTime(m_context.DataProcessingDurationMs);
			m_chunkManager = new ChunkManager.RenderingChunkManager(object2.GetReportChunk, null, definitionObjects, null, report.IntermediateFormatVersion);
			m_renderingContext = new Microsoft.ReportingServices.ReportRendering.RenderingContext(m_reportSnapshot, base.PublicRenderingContext.Format, m_executionTimeStamp, report.EmbeddedImages, report.ImageStreamNames, base.PublicRenderingContext.EventInfo, base.PublicProcessingContext.ReportContext, base.ReportUri, base.RenderingParameters, object2.GetReportChunk, m_chunkManager, base.PublicProcessingContext.GetResourceCallback, object2.GetChunkMimeType, base.PublicRenderingContext.StoreServerParametersCallback, retrieveRenderingInfo: false, base.PublicProcessingContext.AllowUserProfileState, base.PublicRenderingContext.ReportRuntimeSetup, base.PublicProcessingContext.JobContext, base.PublicProcessingContext.DataProtection);
		}
	}
}

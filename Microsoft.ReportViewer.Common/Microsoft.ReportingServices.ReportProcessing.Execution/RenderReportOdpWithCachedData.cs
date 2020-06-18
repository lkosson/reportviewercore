using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing.Execution
{
	internal class RenderReportOdpWithCachedData : RenderReportOdp
	{
		private readonly DateTime m_executionTimeStamp;

		private readonly IChunkFactory m_dataCacheChunks;

		public RenderReportOdpWithCachedData(ProcessingContext pc, RenderingContext rc, DateTime executionTimeStamp, IConfiguration configuration, IChunkFactory dataCacheChunks)
			: base(pc, rc, configuration)
		{
			m_executionTimeStamp = executionTimeStamp;
			m_dataCacheChunks = dataCacheChunks;
		}

		protected override void ProcessReport(ProcessingErrorContext errorContext, ExecutionLogContext executionLogContext, ref UserProfileState userProfileState)
		{
			OnDemandMetadata onDemandMetadata = null;
			GlobalIDOwnerCollection globalIDOwnerCollection = new GlobalIDOwnerCollection();
			onDemandMetadata = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.DeserializeOnDemandMetadata(m_dataCacheChunks, globalIDOwnerCollection);
			globalIDOwnerCollection = new GlobalIDOwnerCollection();
			Microsoft.ReportingServices.ReportIntermediateFormat.Report report = ReportProcessing.DeserializeKatmaiReport(base.PublicProcessingContext.ChunkFactory, keepReferences: false, globalIDOwnerCollection);
			ProcessReportOdpWithCachedData processReportOdpWithCachedData = new ProcessReportOdpWithCachedData(base.Configuration, base.PublicProcessingContext, report, errorContext, base.PublicRenderingContext.StoreServerParametersCallback, globalIDOwnerCollection, executionLogContext, m_executionTimeStamp, onDemandMetadata);
			m_odpReportSnapshot = processReportOdpWithCachedData.Execute(out m_odpContext);
		}

		protected override void PrepareForExecution()
		{
			ValidateReportParameters();
		}
	}
}

using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.ReportProcessing.Execution
{
	internal class RenderReportOdpReprocessSnapshot : RenderReportOdp
	{
		private readonly IChunkFactory m_originalSnapshotChunks;

		protected override bool IsSnapshotReprocessing => true;

		public RenderReportOdpReprocessSnapshot(ProcessingContext pc, RenderingContext rc, IConfiguration configuration, IChunkFactory originalSnapshotChunks)
			: base(pc, rc, configuration)
		{
			m_originalSnapshotChunks = originalSnapshotChunks;
		}

		protected override void ProcessReport(ProcessingErrorContext errorContext, ExecutionLogContext executionLogContext, ref UserProfileState userProfileState)
		{
			OnDemandMetadata odpMetadata = null;
			Microsoft.ReportingServices.ReportIntermediateFormat.Report report;
			GlobalIDOwnerCollection globalIDOwnerCollection = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.DeserializeOdpReportSnapshot(base.PublicProcessingContext, m_originalSnapshotChunks, errorContext, fetchSubreports: true, deserializeGroupTree: false, base.Configuration, ref odpMetadata, out report);
			ProcessReportOdpSnapshotReprocessing processReportOdpSnapshotReprocessing = new ProcessReportOdpSnapshotReprocessing(base.Configuration, base.PublicProcessingContext, report, errorContext, base.PublicRenderingContext.StoreServerParametersCallback, globalIDOwnerCollection, executionLogContext, odpMetadata);
			m_odpReportSnapshot = processReportOdpSnapshotReprocessing.Execute(out m_odpContext);
		}

		protected override void PrepareForExecution()
		{
			ValidateReportParameters();
		}
	}
}

using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.ReportProcessing.Execution
{
	internal class RenderReportOdpSnapshot : RenderReportOdp
	{
		protected virtual bool IsRenderStream => false;

		public RenderReportOdpSnapshot(ProcessingContext pc, RenderingContext rc, IConfiguration configuration)
			: base(pc, rc, configuration)
		{
		}

		protected override void PrepareForExecution()
		{
			ReportProcessing.ProcessOdpToggleEvent(base.PublicRenderingContext.ShowHideToggle, base.PublicProcessingContext.ChunkFactory, base.PublicRenderingContext.EventInfo, out EventInformation newShowHideInfo, out bool showHideInfoChanged);
			if (showHideInfoChanged)
			{
				base.PublicRenderingContext.EventInfo = newShowHideInfo;
			}
		}

		protected override void ProcessReport(ProcessingErrorContext errorContext, ExecutionLogContext executionLogContext, ref UserProfileState userProfileState)
		{
			OnDemandMetadata odpMetadata = null;
			Microsoft.ReportingServices.ReportIntermediateFormat.Report report;
			GlobalIDOwnerCollection globalIDOwnerCollection = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.DeserializeOdpReportSnapshot(base.PublicProcessingContext, null, errorContext, fetchSubreports: true, deserializeGroupTree: true, base.Configuration, ref odpMetadata, out report);
			m_odpReportSnapshot = odpMetadata.ReportSnapshot;
			new ProcessReportOdpSnapshot(base.Configuration, base.PublicProcessingContext, report, errorContext, base.PublicRenderingContext.StoreServerParametersCallback, globalIDOwnerCollection, executionLogContext, odpMetadata).Execute(out m_odpContext);
		}

		protected override void CleanupSuccessfulProcessing(ProcessingErrorContext errorContext)
		{
			if (!IsRenderStream)
			{
				errorContext.Combine(m_odpReportSnapshot.Warnings);
			}
			base.CleanupSuccessfulProcessing(errorContext);
		}
	}
}

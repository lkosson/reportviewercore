using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.ReportProcessing.Execution
{
	internal class ProcessReportOdpSnapshot : ProcessReportOdp
	{
		private readonly OnDemandMetadata m_odpMetadataFromSnapshot;

		protected OnDemandMetadata OdpMetadataFromSnapshot => m_odpMetadataFromSnapshot;

		protected override bool SnapshotProcessing => true;

		protected override bool ReprocessSnapshot => false;

		protected override bool ProcessWithCachedData => false;

		protected override OnDemandProcessingContext.Mode OnDemandProcessingMode => OnDemandProcessingContext.Mode.Full;

		public ProcessReportOdpSnapshot(IConfiguration configuration, ProcessingContext pc, Microsoft.ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, ReportProcessing.StoreServerParameters storeServerParameters, GlobalIDOwnerCollection globalIDOwnerCollection, ExecutionLogContext executionLogContext, OnDemandMetadata odpMetadataFromSnapshot)
			: base(configuration, pc, report, errorContext, storeServerParameters, globalIDOwnerCollection, executionLogContext)
		{
			Global.Tracer.Assert(odpMetadataFromSnapshot != null, "Must provide existing metadata when processing an existing snapshot");
			Global.Tracer.Assert(odpMetadataFromSnapshot.OdpChunkManager != null && odpMetadataFromSnapshot.ReportSnapshot != null, "Must provide chunk manager and ReportSnapshot when processing an existing snapshot");
			m_odpMetadataFromSnapshot = odpMetadataFromSnapshot;
		}

		protected override OnDemandMetadata PrepareMetadata()
		{
			Global.Tracer.Assert(m_odpMetadataFromSnapshot.ReportInstance != null, "Processing an existing snapshot with no ReportInstance");
			return m_odpMetadataFromSnapshot;
		}

		protected override void SetupReportLanguage(Merge odpMerge, Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			odpMerge.EvaluateReportLanguage(reportInstance, reportInstance.Language);
		}

		protected override void PreProcessSnapshot(OnDemandProcessingContext odpContext, Merge odpMerge, Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot)
		{
			SetupInitialOdpState(odpContext, reportInstance, reportSnapshot);
			if (base.ReportDefinition.HasSubReports)
			{
				SubReportInitializer.InitializeSubReportOdpContext(base.ReportDefinition, odpContext);
				SubReportInitializer.InitializeSubReports(base.ReportDefinition, reportInstance, odpContext, inDataRegion: false, fromCreateSubReportInstance: false);
			}
			PreProcessTablices(odpContext, reportSnapshot);
			reportInstance.CalculateAndStoreReportVariables(odpContext);
			odpContext.OdpMetadata.SetUpdatedVariableValues(odpContext, reportInstance);
		}

		protected virtual void PreProcessTablices(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot)
		{
		}
	}
}

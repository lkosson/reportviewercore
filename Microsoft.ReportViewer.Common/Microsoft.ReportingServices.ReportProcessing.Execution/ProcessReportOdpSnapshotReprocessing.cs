using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.ReportProcessing.Execution
{
	internal class ProcessReportOdpSnapshotReprocessing : ProcessReportOdpSnapshot
	{
		protected override bool ReprocessSnapshot => true;

		public ProcessReportOdpSnapshotReprocessing(IConfiguration configuration, ProcessingContext pc, Microsoft.ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, ReportProcessing.StoreServerParameters storeServerParameters, GlobalIDOwnerCollection globalIDOwnerCollection, ExecutionLogContext executionLogContext, OnDemandMetadata odpMetadataFromSnapshot)
			: base(configuration, pc, report, errorContext, storeServerParameters, globalIDOwnerCollection, executionLogContext, odpMetadataFromSnapshot)
		{
		}

		protected override OnDemandMetadata PrepareMetadata()
		{
			return new OnDemandMetadata(base.OdpMetadataFromSnapshot, base.ReportDefinition)
			{
				ReportSnapshot = new Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot(base.ReportDefinition, base.PublicProcessingContext.ReportContext.ItemName, base.PublicProcessingContext.Parameters, base.PublicProcessingContext.RequestUserName, base.OdpMetadataFromSnapshot.ReportSnapshot.ExecutionTime, base.PublicProcessingContext.ReportContext.HostRootUri, base.PublicProcessingContext.ReportContext.ParentPath, base.PublicProcessingContext.UserLanguage.Name)
			};
		}

		protected override void SetupReportLanguage(Merge odpMerge, Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			odpMerge.EvaluateReportLanguage(reportInstance, null);
		}
	}
}

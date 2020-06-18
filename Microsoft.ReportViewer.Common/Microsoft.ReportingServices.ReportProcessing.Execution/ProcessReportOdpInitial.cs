using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing.Execution
{
	internal class ProcessReportOdpInitial : ProcessReportOdp
	{
		private readonly DateTime m_executionTime;

		protected override bool SnapshotProcessing => false;

		protected override bool ReprocessSnapshot => false;

		protected override bool ProcessWithCachedData => false;

		protected override OnDemandProcessingContext.Mode OnDemandProcessingMode => OnDemandProcessingContext.Mode.Full;

		public ProcessReportOdpInitial(IConfiguration configuration, ProcessingContext pc, Microsoft.ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, ReportProcessing.StoreServerParameters storeServerParameters, GlobalIDOwnerCollection globalIDOwnerCollection, ExecutionLogContext executionLogContext, DateTime executionTime)
			: base(configuration, pc, report, errorContext, storeServerParameters, globalIDOwnerCollection, executionLogContext)
		{
			m_executionTime = executionTime;
		}

		protected override OnDemandMetadata PrepareMetadata()
		{
			OnDemandMetadata onDemandMetadata = new OnDemandMetadata(base.ReportDefinition);
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot2 = onDemandMetadata.ReportSnapshot = new Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot(base.ReportDefinition, base.PublicProcessingContext.ReportContext.ItemName, base.PublicProcessingContext.Parameters, base.PublicProcessingContext.RequestUserName, m_executionTime, base.PublicProcessingContext.ReportContext.HostRootUri, base.PublicProcessingContext.ReportContext.ParentPath, base.PublicProcessingContext.UserLanguage.Name);
			return onDemandMetadata;
		}

		protected override void SetupReportLanguage(Merge odpMerge, Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			odpMerge.EvaluateReportLanguage(reportInstance, null);
		}

		protected override void PreProcessSnapshot(OnDemandProcessingContext odpContext, Merge odpMerge, Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot)
		{
			if (base.ReportDefinition.HasSubReports)
			{
				ReportProcessing.FetchSubReports(base.ReportDefinition, odpContext.ChunkFactory, odpContext.ErrorContext, odpContext.OdpMetadata, odpContext.ReportContext, odpContext.SubReportCallback, 0, odpContext.SnapshotProcessing, odpContext.ProcessWithCachedData, base.GlobalIDOwnerCollection, base.PublicProcessingContext.QueryParameters);
				SubReportInitializer.InitializeSubReportOdpContext(base.ReportDefinition, odpContext);
			}
			odpMerge.FetchData(reportInstance, mergeTransaction: false);
			reportInstance.CalculateAndStoreReportVariables(odpContext);
			if (base.ReportDefinition.HasSubReports)
			{
				SubReportInitializer.InitializeSubReports(base.ReportDefinition, reportInstance, odpContext, inDataRegion: false, fromCreateSubReportInstance: false);
			}
			SetupInitialOdpState(odpContext, reportInstance, reportSnapshot);
			if (base.ReportDefinition.HasSubReports || (!base.ReportDefinition.DeferVariableEvaluation && base.ReportDefinition.HasVariables))
			{
				Merge.PreProcessTablixes(base.ReportDefinition, odpContext, onlyWithSubReports: true);
			}
		}
	}
}

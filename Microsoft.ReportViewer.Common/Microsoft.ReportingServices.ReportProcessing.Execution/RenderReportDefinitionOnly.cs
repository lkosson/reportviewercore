using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing.Execution
{
	internal class RenderReportDefinitionOnly : RenderReportOdpInitial
	{
		public RenderReportDefinitionOnly(ProcessingContext pc, RenderingContext rc, DateTime executionTimeStamp, IConfiguration configuration)
			: base(pc, rc, executionTimeStamp, configuration)
		{
		}

		protected override void CleanupForException()
		{
		}

		protected override void FinalCleanup()
		{
		}

		protected override void PrepareForExecution()
		{
			ValidateReportParameters();
			ReportProcessing.CheckReportCredentialsAndConnectionUserDependency(base.PublicProcessingContext);
		}

		protected override void UpdateEventInfoInSnapshot()
		{
		}

		protected override void ProcessReport(ProcessingErrorContext errorContext, ExecutionLogContext executionLogContext, ref UserProfileState userProfileState)
		{
			GlobalIDOwnerCollection globalIDOwnerCollection;
			Microsoft.ReportingServices.ReportIntermediateFormat.Report reportDefinition = GetReportDefinition(out globalIDOwnerCollection);
			ProcessReportDefinitionOnly processReportDefinitionOnly = new ProcessReportDefinitionOnly(base.Configuration, base.PublicProcessingContext, reportDefinition, errorContext, base.PublicRenderingContext.StoreServerParametersCallback, globalIDOwnerCollection, executionLogContext, base.ExecutionTimeStamp);
			m_odpReportSnapshot = processReportDefinitionOnly.Execute(out m_odpContext);
		}

		protected override OnDemandProcessingResult ConstructProcessingResult(bool eventInfoChanged, Hashtable renderProperties, ProcessingErrorContext errorContext, UserProfileState userProfileState, bool renderingInfoChanged, ExecutionLogContext executionLogContext)
		{
			return new DefinitionOnlyOnDemandProcessingResult(m_odpReportSnapshot, m_odpContext.OdpMetadata.OdpChunkManager, m_odpContext.OdpMetadata.SnapshotHasChanged, base.PublicProcessingContext.ChunkFactory, base.PublicProcessingContext.Parameters, 0, GetNumberOfPages(renderProperties), errorContext.Messages, eventInfoChanged, base.PublicRenderingContext.EventInfo, GetUpdatedPaginationMode(renderProperties, base.PublicRenderingContext.ClientPaginationMode), base.PublicProcessingContext.ChunkFactory.ReportProcessingFlags, m_odpContext.HasUserProfileState, executionLogContext);
		}

		protected override Microsoft.ReportingServices.OnDemandReportRendering.Report PrepareROM(out Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext)
		{
			odpRenderingContext = new Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext(base.PublicRenderingContext.Format, m_odpReportSnapshot, base.PublicRenderingContext.EventInfo, m_odpContext);
			odpRenderingContext.InstanceAccessDisallowed = true;
			return new Microsoft.ReportingServices.OnDemandReportRendering.Report(m_odpReportSnapshot.Report, odpRenderingContext, base.ReportName, base.PublicRenderingContext.ReportDescription);
		}
	}
}

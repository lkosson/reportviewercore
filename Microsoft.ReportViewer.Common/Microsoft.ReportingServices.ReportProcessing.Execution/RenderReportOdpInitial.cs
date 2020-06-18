using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing.Execution
{
	internal class RenderReportOdpInitial : RenderReportOdp
	{
		private readonly DateTime m_executionTimeStamp;

		protected DateTime ExecutionTimeStamp => m_executionTimeStamp;

		public RenderReportOdpInitial(ProcessingContext pc, RenderingContext rc, DateTime executionTimeStamp, IConfiguration configuration)
			: base(pc, rc, configuration)
		{
			m_executionTimeStamp = executionTimeStamp;
		}

		protected override void PrepareForExecution()
		{
			ValidateReportParameters();
			ReportProcessing.CheckReportCredentialsAndConnectionUserDependency(base.PublicProcessingContext);
		}

		protected override void ProcessReport(ProcessingErrorContext errorContext, ExecutionLogContext executionLogContext, ref UserProfileState userProfileState)
		{
			GlobalIDOwnerCollection globalIDOwnerCollection;
			Microsoft.ReportingServices.ReportIntermediateFormat.Report reportDefinition = GetReportDefinition(out globalIDOwnerCollection);
			ProcessReportOdpInitial processReportOdpInitial = new ProcessReportOdpInitial(base.Configuration, base.PublicProcessingContext, reportDefinition, errorContext, base.PublicRenderingContext.StoreServerParametersCallback, globalIDOwnerCollection, executionLogContext, m_executionTimeStamp);
			m_odpReportSnapshot = processReportOdpInitial.Execute(out m_odpContext);
		}

		protected Microsoft.ReportingServices.ReportIntermediateFormat.Report GetReportDefinition(out GlobalIDOwnerCollection globalIDOwnerCollection)
		{
			globalIDOwnerCollection = new GlobalIDOwnerCollection();
			return ReportProcessing.DeserializeKatmaiReport(base.PublicProcessingContext.ChunkFactory, keepReferences: false, globalIDOwnerCollection);
		}
	}
}

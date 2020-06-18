using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing.Execution
{
	internal sealed class ProcessDataShape : ProcessReportOdpStreaming
	{
		private readonly bool m_useParallelQueryExecution;

		public ProcessDataShape(IConfiguration configuration, ProcessingContext pc, Microsoft.ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, ReportProcessing.StoreServerParameters storeServerParameters, GlobalIDOwnerCollection globalIDOwnerCollection, ExecutionLogContext executionLogContext, DateTime executionTime, IAbortHelper abortHelper, bool useParallelQueryExecution)
			: base(configuration, pc, report, errorContext, storeServerParameters, globalIDOwnerCollection, executionLogContext, executionTime, abortHelper)
		{
			m_useParallelQueryExecution = useParallelQueryExecution;
		}

		protected override void PreProcessSnapshot(OnDemandProcessingContext odpContext, Merge odpMerge, Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot)
		{
			ParallelPreloadQueries(odpContext);
			SetupInitialOdpState(odpContext, reportInstance, reportSnapshot);
		}

		private void ParallelPreloadQueries(OnDemandProcessingContext odpContext)
		{
			if (m_useParallelQueryExecution)
			{
				_ = odpContext.JobContext;
			}
		}
	}
}

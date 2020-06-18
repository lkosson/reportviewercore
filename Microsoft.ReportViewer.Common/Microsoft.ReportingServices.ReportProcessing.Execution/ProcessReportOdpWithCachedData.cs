using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing.Execution
{
	internal sealed class ProcessReportOdpWithCachedData : ProcessReportOdpInitial
	{
		private readonly OnDemandMetadata m_odpMetadataFromDataCache;

		protected override bool ProcessWithCachedData => true;

		public ProcessReportOdpWithCachedData(IConfiguration configuration, ProcessingContext pc, Microsoft.ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, ReportProcessing.StoreServerParameters storeServerParameters, GlobalIDOwnerCollection globalIDOwnerCollection, ExecutionLogContext executionLogContext, DateTime executionTime, OnDemandMetadata odpMetadataFromDataCache)
			: base(configuration, pc, report, errorContext, storeServerParameters, globalIDOwnerCollection, executionLogContext, executionTime)
		{
			Global.Tracer.Assert(odpMetadataFromDataCache != null, "Must provide existing metadata to process with cached data");
			m_odpMetadataFromDataCache = odpMetadataFromDataCache;
		}

		protected override OnDemandMetadata PrepareMetadata()
		{
			OnDemandMetadata onDemandMetadata = base.PrepareMetadata();
			onDemandMetadata.PrepareForCachedDataProcessing(m_odpMetadataFromDataCache);
			return onDemandMetadata;
		}
	}
}

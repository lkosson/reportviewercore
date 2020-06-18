using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing.Execution
{
	internal class RenderReportOdpLiveAndCacheData : RenderReportOdpInitial
	{
		private IChunkFactory m_metaDataChunkFactory;

		public RenderReportOdpLiveAndCacheData(ProcessingContext pc, RenderingContext rc, DateTime executionTimeStamp, IConfiguration configuration, IChunkFactory metaDataChunkFactory)
			: base(pc, rc, executionTimeStamp, configuration)
		{
			Global.Tracer.Assert(metaDataChunkFactory != null, "Must supply a IChunkFactory to store the cached data");
			m_metaDataChunkFactory = metaDataChunkFactory;
		}

		protected override void CleanupSuccessfulProcessing(ProcessingErrorContext errorContext)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.SerializeMetadata(m_metaDataChunkFactory, base.OdpContext.OdpMetadata, base.OdpContext.GetActiveCompatibilityVersion(), base.OdpContext.ProhibitSerializableValues);
			base.CleanupSuccessfulProcessing(errorContext);
		}
	}
}

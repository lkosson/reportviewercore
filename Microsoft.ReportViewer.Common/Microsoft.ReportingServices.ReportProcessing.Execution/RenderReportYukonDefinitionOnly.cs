using Microsoft.ReportingServices.OnDemandReportRendering;
using System;

namespace Microsoft.ReportingServices.ReportProcessing.Execution
{
	internal sealed class RenderReportYukonDefinitionOnly : RenderReportYukonInitial
	{
		public RenderReportYukonDefinitionOnly(ProcessingContext pc, RenderingContext rc, DateTime executionTimeStamp, ReportProcessing processing, IChunkFactory yukonCompiledDefinition)
			: base(pc, rc, executionTimeStamp, processing, yukonCompiledDefinition)
		{
		}

		protected override Microsoft.ReportingServices.OnDemandReportRendering.Report PrepareROM(out Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext)
		{
			odpRenderingContext = new Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext(base.PublicRenderingContext.Format, m_reportSnapshot, base.PublicProcessingContext.ChunkFactory, base.PublicRenderingContext.EventInfo);
			odpRenderingContext.InstanceAccessDisallowed = true;
			return new Microsoft.ReportingServices.OnDemandReportRendering.Report(m_reportSnapshot.Report, m_reportSnapshot.ReportInstance, m_renderingContext, odpRenderingContext, base.ReportName, base.PublicRenderingContext.ReportDescription);
		}
	}
}

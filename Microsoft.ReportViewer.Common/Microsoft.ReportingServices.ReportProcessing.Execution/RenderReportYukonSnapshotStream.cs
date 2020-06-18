using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using System.Collections;
using System.Collections.Specialized;

namespace Microsoft.ReportingServices.ReportProcessing.Execution
{
	internal sealed class RenderReportYukonSnapshotStream : RenderReportYukonSnapshot
	{
		private readonly string m_streamName;

		protected override bool IsRenderStream => true;

		public RenderReportYukonSnapshotStream(ProcessingContext pc, RenderingContext rc, ReportProcessing processing, string streamName)
			: base(pc, rc, processing)
		{
			m_streamName = streamName;
		}

		protected override bool InvokeRenderer(IRenderingExtension renderer, Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			return renderer.RenderStream(m_streamName, report, reportServerParameters, deviceInfo, clientCapabilities, ref renderProperties, createAndRegisterStream);
		}
	}
}

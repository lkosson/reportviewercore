using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal static class DataSegmentRendererFactory
	{
		internal static IDataSegmentRenderer CreateDataSegmentRenderer()
		{
			return (IDataSegmentRenderer)Activator.CreateInstance("Microsoft.ReportingServices.DataSegmentRendering", "Microsoft.ReportingServices.Rendering.DataSegmentRenderer.DataSegmentRenderer").Unwrap();
		}
	}
}

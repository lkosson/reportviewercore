using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class SelectiveRenderingCannotFindReportItemException : ReportRenderingException
	{
		internal SelectiveRenderingCannotFindReportItemException(string name)
			: base(HPBRes.ReportItemCannotBeFound(name), unexpected: false)
		{
		}
	}
}

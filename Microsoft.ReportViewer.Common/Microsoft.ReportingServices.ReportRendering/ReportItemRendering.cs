using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ReportItemRendering : MemberBase
	{
		internal RenderingContext m_renderingContext;

		internal Microsoft.ReportingServices.ReportProcessing.ReportItem m_reportItemDef;

		internal ReportItemInstance m_reportItemInstance;

		internal ReportItemInstanceInfo m_reportItemInstanceInfo;

		internal MatrixHeadingInstance m_headingInstance;

		internal ReportItemRendering()
			: base(isCustomControl: false)
		{
		}
	}
}

using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class ParagraphImpl : Paragraph
	{
		private TextRunsImpl m_textRuns;

		public override TextRuns TextRuns => m_textRuns;

		internal ParagraphImpl(Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph paragraphDef, Microsoft.ReportingServices.RdlExpressions.ReportRuntime reportRT, IErrorContext iErrorContext, IScope scope)
		{
			m_textRuns = new TextRunsImpl(paragraphDef, reportRT, iErrorContext, scope);
		}

		internal void Reset()
		{
			m_textRuns.Reset();
		}
	}
}

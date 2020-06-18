using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class TextRunExprHost : StyleExprHost
	{
		public ActionInfoExprHost ActionInfoHost;

		private TextRun m_textRun;

		public object Value => m_textRun.Value;

		public virtual object LabelExpr => null;

		public virtual object ValueExpr => null;

		public virtual object ToolTipExpr => null;

		public virtual object MarkupTypeExpr => null;

		internal void SetTextRun(TextRun textRun)
		{
			m_textRun = textRun;
		}
	}
}

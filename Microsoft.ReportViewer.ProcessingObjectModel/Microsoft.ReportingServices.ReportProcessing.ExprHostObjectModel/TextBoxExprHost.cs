using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;

namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class TextBoxExprHost : ReportItemExprHost
	{
		public const string ValueName = "Value";

		private ReportItem m_textBox;

		public object Value => m_textBox.Value;

		internal ReportItem ReportObjectModelTextBox => m_textBox;

		public virtual object ValueExpr => null;

		public virtual object ToggleImageInitialStateExpr => null;

		internal void SetTextBox(ReportItem textBox)
		{
			m_textBox = textBox;
		}
	}
}

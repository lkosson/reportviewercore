using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class TextBoxExprHost : ReportItemExprHost
	{
		[CLSCompliant(false)]
		protected IList<ParagraphExprHost> m_paragraphHostsRemotable;

		public const string ValueName = "Value";

		private ReportItem m_textBox;

		internal IList<ParagraphExprHost> ParagraphHostsRemotable => m_paragraphHostsRemotable;

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

using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ParagraphExprHost : StyleExprHost
	{
		[CLSCompliant(false)]
		protected IList<TextRunExprHost> m_textRunHostsRemotable;

		internal IList<TextRunExprHost> TextRunHostsRemotable => m_textRunHostsRemotable;

		public virtual object LeftIndentExpr => null;

		public virtual object RightIndentExpr => null;

		public virtual object HangingIndentExpr => null;

		public virtual object SpaceBeforeExpr => null;

		public virtual object SpaceAfterExpr => null;

		public virtual object ListStyleExpr => null;

		public virtual object ListLevelExpr => null;
	}
}

using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ReportItemExprHost : StyleExprHost, IVisibilityHiddenExprHost
	{
		public ActionInfoExprHost ActionInfoHost;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

		public PageBreakExprHost PageBreakExprHost;

		internal IList<DataValueExprHost> CustomPropertyHostsRemotable => m_customPropertyHostsRemotable;

		public virtual object LabelExpr => null;

		public virtual object BookmarkExpr => null;

		public virtual object ToolTipExpr => null;

		public virtual object VisibilityHiddenExpr => null;

		public virtual object PageNameExpr => null;
	}
}

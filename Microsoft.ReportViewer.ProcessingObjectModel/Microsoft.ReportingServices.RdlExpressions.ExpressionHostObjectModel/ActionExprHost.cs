using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ActionExprHost : ReportObjectModelProxy
	{
		[CLSCompliant(false)]
		protected IList<ParamExprHost> m_drillThroughParameterHostsRemotable;

		public virtual object HyperlinkExpr => null;

		public virtual object DrillThroughReportNameExpr => null;

		internal IList<ParamExprHost> DrillThroughParameterHostsRemotable => m_drillThroughParameterHostsRemotable;

		public virtual object DrillThroughBookmarkLinkExpr => null;

		public virtual object BookmarkLinkExpr => null;

		public virtual object LabelExpr => null;
	}
}

using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class ActionExprHost : ReportObjectModelProxy
	{
		protected ParamExprHost[] DrillThroughParameterHosts;

		[CLSCompliant(false)]
		protected IList<ParamExprHost> m_drillThroughParameterHostsRemotable;

		public virtual object HyperlinkExpr => null;

		public virtual object DrillThroughReportNameExpr => null;

		internal IList<ParamExprHost> DrillThroughParameterHostsRemotable
		{
			get
			{
				if (m_drillThroughParameterHostsRemotable == null && DrillThroughParameterHosts != null)
				{
					m_drillThroughParameterHostsRemotable = new RemoteArrayWrapper<ParamExprHost>(DrillThroughParameterHosts);
				}
				return m_drillThroughParameterHostsRemotable;
			}
		}

		public virtual object DrillThroughBookmarkLinkExpr => null;

		public virtual object BookmarkLinkExpr => null;

		public virtual object LabelExpr => null;
	}
}

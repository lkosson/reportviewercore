using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class ReportItemExprHost : StyleExprHost, IVisibilityHiddenExprHost
	{
		public ActionExprHost ActionHost;

		public ActionInfoExprHost ActionInfoHost;

		protected DataValueExprHost[] CustomPropertyHosts;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

		internal IList<DataValueExprHost> CustomPropertyHostsRemotable
		{
			get
			{
				if (m_customPropertyHostsRemotable == null && CustomPropertyHosts != null)
				{
					m_customPropertyHostsRemotable = new RemoteArrayWrapper<DataValueExprHost>(CustomPropertyHosts);
				}
				return m_customPropertyHostsRemotable;
			}
		}

		public virtual object LabelExpr => null;

		public virtual object BookmarkExpr => null;

		public virtual object ToolTipExpr => null;

		public virtual object VisibilityHiddenExpr => null;
	}
}

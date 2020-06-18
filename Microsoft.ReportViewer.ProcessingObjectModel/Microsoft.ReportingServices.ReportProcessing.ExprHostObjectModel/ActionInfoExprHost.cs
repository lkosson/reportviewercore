using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class ActionInfoExprHost : StyleExprHost
	{
		protected ActionExprHost[] ActionItemHosts;

		[CLSCompliant(false)]
		protected IList<ActionExprHost> m_actionItemHostsRemotable;

		internal IList<ActionExprHost> ActionItemHostsRemotable
		{
			get
			{
				if (m_actionItemHostsRemotable == null && ActionItemHosts != null)
				{
					m_actionItemHostsRemotable = new RemoteArrayWrapper<ActionExprHost>(ActionItemHosts);
				}
				return m_actionItemHostsRemotable;
			}
		}
	}
}

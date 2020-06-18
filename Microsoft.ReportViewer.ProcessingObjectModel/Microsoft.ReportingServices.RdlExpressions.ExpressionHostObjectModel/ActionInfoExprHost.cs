using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ActionInfoExprHost : StyleExprHost
	{
		[CLSCompliant(false)]
		protected IList<ActionExprHost> m_actionItemHostsRemotable;

		internal IList<ActionExprHost> ActionItemHostsRemotable => m_actionItemHostsRemotable;
	}
}

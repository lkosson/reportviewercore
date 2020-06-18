using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class LinearScaleExprHost : GaugeScaleExprHost
	{
		[CLSCompliant(false)]
		protected IList<LinearPointerExprHost> m_linearPointersHostsRemotable;

		internal IList<LinearPointerExprHost> LinearPointersHostsRemotable => m_linearPointersHostsRemotable;

		public virtual object StartMarginExpr => null;

		public virtual object EndMarginExpr => null;
	}
}

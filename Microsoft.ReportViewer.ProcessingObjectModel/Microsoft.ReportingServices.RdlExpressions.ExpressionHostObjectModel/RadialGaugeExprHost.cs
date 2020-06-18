using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class RadialGaugeExprHost : GaugeExprHost
	{
		[CLSCompliant(false)]
		protected IList<RadialScaleExprHost> m_radialScalesHostsRemotable;

		internal IList<RadialScaleExprHost> RadialScalesHostsRemotable => m_radialScalesHostsRemotable;

		public virtual object PivotXExpr => null;

		public virtual object PivotYExpr => null;
	}
}

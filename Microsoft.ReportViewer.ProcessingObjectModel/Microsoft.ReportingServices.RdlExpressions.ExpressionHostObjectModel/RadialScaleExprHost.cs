using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class RadialScaleExprHost : GaugeScaleExprHost
	{
		[CLSCompliant(false)]
		protected IList<RadialPointerExprHost> m_radialPointersHostsRemotable;

		internal IList<RadialPointerExprHost> RadialPointersHostsRemotable => m_radialPointersHostsRemotable;

		public virtual object RadiusExpr => null;

		public virtual object StartAngleExpr => null;

		public virtual object SweepAngleExpr => null;
	}
}

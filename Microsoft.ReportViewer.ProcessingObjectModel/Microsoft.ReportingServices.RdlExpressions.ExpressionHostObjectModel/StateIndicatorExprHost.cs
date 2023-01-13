using System;
using System.Collections.Generic;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class StateIndicatorExprHost : GaugePanelItemExprHost
	{
		public GaugeInputValueExprHost GaugeInputValueHost;

		public IndicatorImageExprHost IndicatorImageHost;

		[CLSCompliant(false)]
		protected IList<IndicatorStateExprHost> m_indicatorStatesHostsRemotable;

		public GaugeInputValueExprHost MaximumValueHost;

		public GaugeInputValueExprHost MinimumValueHost;

		public virtual object IndicatorStyleExpr => null;

		public virtual object ScaleFactorExpr => null;

		[CLSCompliant(false)]
		public IList<IndicatorStateExprHost> IndicatorStatesHostsRemotable
		{
			get
			{
				return m_indicatorStatesHostsRemotable;
			}
		}

		public virtual object ResizeModeExpr => null;

		public virtual object AngleExpr => null;

		public virtual object TransformationTypeExpr => null;
	}
}

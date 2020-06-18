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
			[StrongNameIdentityPermission(SecurityAction.LinkDemand, PublicKey = "0024000004800000940000000602000000240000525341310004000001000100272736ad6e5f9586bac2d531eabc3acc666c2f8ec879fa94f8f7b0327d2ff2ed523448f83c3d5c5dd2dfc7bc99c5286b2c125117bf5cbe242b9d41750732b2bdffe649c6efb8e5526d526fdd130095ecdb7bf210809c6cdad8824faa9ac0310ac3cba2aa0523567b2dfa7fe250b30facbd62d4ec99b94ac47c7d3b28f1f6e4c8")]
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

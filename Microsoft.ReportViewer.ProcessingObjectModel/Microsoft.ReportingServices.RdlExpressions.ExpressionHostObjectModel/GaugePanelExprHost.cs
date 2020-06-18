using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GaugePanelExprHost : DataRegionExprHost<GaugeMemberExprHost, GaugeCellExprHost>
	{
		[CLSCompliant(false)]
		protected IList<LinearGaugeExprHost> m_linearGaugesHostsRemotable;

		[CLSCompliant(false)]
		protected IList<RadialGaugeExprHost> m_radialGaugesHostsRemotable;

		[CLSCompliant(false)]
		protected IList<NumericIndicatorExprHost> m_numericIndicatorsHostsRemotable;

		[CLSCompliant(false)]
		protected IList<StateIndicatorExprHost> m_stateIndicatorsHostsRemotable;

		[CLSCompliant(false)]
		protected IList<GaugeImageExprHost> m_gaugeImagesHostsRemotable;

		[CLSCompliant(false)]
		protected IList<GaugeLabelExprHost> m_gaugeLabelsHostsRemotable;

		public BackFrameExprHost BackFrameHost;

		public TopImageExprHost TopImageHost;

		internal IList<LinearGaugeExprHost> LinearGaugesHostsRemotable => m_linearGaugesHostsRemotable;

		internal IList<RadialGaugeExprHost> RadialGaugesHostsRemotable => m_radialGaugesHostsRemotable;

		internal IList<NumericIndicatorExprHost> NumericIndicatorsHostsRemotable => m_numericIndicatorsHostsRemotable;

		internal IList<StateIndicatorExprHost> StateIndicatorsHostsRemotable => m_stateIndicatorsHostsRemotable;

		internal IList<GaugeImageExprHost> GaugeImagesHostsRemotable => m_gaugeImagesHostsRemotable;

		internal IList<GaugeLabelExprHost> GaugeLabelsHostsRemotable => m_gaugeLabelsHostsRemotable;

		public virtual object AntiAliasingExpr => null;

		public virtual object AutoLayoutExpr => null;

		public virtual object ShadowIntensityExpr => null;

		public virtual object TextAntiAliasingQualityExpr => null;
	}
}

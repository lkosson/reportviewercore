using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GaugeCellExprHost : DataCellExprHost
	{
		[CLSCompliant(false)]
		protected IList<GaugeInputValueExprHost> m_gaugeInputValueHostsRemotable;

		internal IList<GaugeInputValueExprHost> GaugeInputValueHostsRemotable => m_gaugeInputValueHostsRemotable;
	}
}

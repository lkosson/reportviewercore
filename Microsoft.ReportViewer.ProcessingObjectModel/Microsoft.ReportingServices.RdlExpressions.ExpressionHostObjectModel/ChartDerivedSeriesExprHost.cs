using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartDerivedSeriesExprHost : StyleExprHost
	{
		public ChartSeriesExprHost ChartSeriesHost;

		[CLSCompliant(false)]
		protected IList<ChartFormulaParameterExprHost> m_formulaParametersHostsRemotable;

		internal IList<ChartFormulaParameterExprHost> ChartFormulaParametersHostsRemotable => m_formulaParametersHostsRemotable;
	}
}

using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class SubreportExprHost : ReportItemExprHost
	{
		[CLSCompliant(false)]
		protected IList<ParamExprHost> m_parameterHostsRemotable;

		public virtual object NoRowsExpr => null;

		internal IList<ParamExprHost> ParameterHostsRemotable => m_parameterHostsRemotable;
	}
}

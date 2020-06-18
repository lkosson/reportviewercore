using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class TablixExprHost : DataRegionExprHost<TablixMemberExprHost, TablixCellExprHost>
	{
		[CLSCompliant(false)]
		protected IList<TablixCellExprHost> m_cornerCellHostsRemotable;

		public virtual object TopMarginExpr => null;

		public virtual object BottomMarginExpr => null;

		public virtual object LeftMarginExpr => null;

		public virtual object RightMarginExpr => null;

		internal IList<TablixCellExprHost> CornerCellHostsRemotable => m_cornerCellHostsRemotable;
	}
}

using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class DataRegionExprHost : ReportItemExprHost
	{
		protected FilterExprHost[] FilterHosts;

		[CLSCompliant(false)]
		protected IList<FilterExprHost> m_filterHostsRemotable;

		public IndexedExprHost UserSortExpressionsHost;

		public virtual object NoRowsExpr => null;

		internal IList<FilterExprHost> FilterHostsRemotable
		{
			get
			{
				if (m_filterHostsRemotable == null && FilterHosts != null)
				{
					m_filterHostsRemotable = new RemoteArrayWrapper<FilterExprHost>(FilterHosts);
				}
				return m_filterHostsRemotable;
			}
		}
	}
}

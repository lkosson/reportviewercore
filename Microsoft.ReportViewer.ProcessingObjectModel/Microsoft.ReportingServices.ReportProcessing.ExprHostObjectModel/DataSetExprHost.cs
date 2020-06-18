using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class DataSetExprHost : ReportObjectModelProxy
	{
		protected CalcFieldExprHost[] FieldHosts;

		[CLSCompliant(false)]
		protected IList<CalcFieldExprHost> m_fieldHostsRemotable;

		public IndexedExprHost QueryParametersHost;

		protected FilterExprHost[] FilterHosts;

		[CLSCompliant(false)]
		protected IList<FilterExprHost> m_filterHostsRemotable;

		public IndexedExprHost UserSortExpressionsHost;

		internal IList<CalcFieldExprHost> FieldHostsRemotable
		{
			get
			{
				if (m_fieldHostsRemotable == null && FieldHosts != null)
				{
					m_fieldHostsRemotable = new RemoteArrayWrapper<CalcFieldExprHost>(FieldHosts);
				}
				return m_fieldHostsRemotable;
			}
		}

		public virtual object QueryCommandTextExpr => null;

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

using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class GroupingExprHost : IndexedExprHost
	{
		protected FilterExprHost[] FilterHosts;

		[CLSCompliant(false)]
		protected IList<FilterExprHost> m_filterHostsRemotable;

		public IndexedExprHost ParentExpressionsHost;

		protected DataValueExprHost[] CustomPropertyHosts;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

		public IndexedExprHost UserSortExpressionsHost;

		public virtual object LabelExpr => null;

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

		internal IList<DataValueExprHost> CustomPropertyHostsRemotable
		{
			get
			{
				if (m_customPropertyHostsRemotable == null && CustomPropertyHosts != null)
				{
					m_customPropertyHostsRemotable = new RemoteArrayWrapper<DataValueExprHost>(CustomPropertyHosts);
				}
				return m_customPropertyHostsRemotable;
			}
		}
	}
}

using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class DataGroupingExprHost : ReportObjectModelProxy
	{
		public GroupingExprHost GroupingHost;

		public SortingExprHost SortingHost;

		protected DataValueExprHost[] CustomPropertyHosts;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

		protected DataGroupingExprHost[] DataGroupingHosts;

		[CLSCompliant(false)]
		protected IList<DataGroupingExprHost> m_dataGroupingHostsRemotable;

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

		internal IList<DataGroupingExprHost> DataGroupingHostsRemotable
		{
			get
			{
				if (m_dataGroupingHostsRemotable == null && DataGroupingHosts != null)
				{
					m_dataGroupingHostsRemotable = new RemoteArrayWrapper<DataGroupingExprHost>(DataGroupingHosts);
				}
				return m_dataGroupingHostsRemotable;
			}
		}
	}
}

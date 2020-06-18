using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class CustomReportItemExprHost : DataRegionExprHost
	{
		protected DataGroupingExprHost[] DataGroupingHosts;

		[CLSCompliant(false)]
		protected IList<DataGroupingExprHost> m_dataGroupingHostsRemotable;

		protected DataCellExprHost[] DataCellHosts;

		[CLSCompliant(false)]
		protected IList<DataCellExprHost> m_dataCellHostsRemotable;

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

		internal IList<DataCellExprHost> DataCellHostsRemotable
		{
			get
			{
				if (m_dataCellHostsRemotable == null && DataCellHosts != null)
				{
					m_dataCellHostsRemotable = new RemoteArrayWrapper<DataCellExprHost>(DataCellHosts);
				}
				return m_dataCellHostsRemotable;
			}
		}
	}
}

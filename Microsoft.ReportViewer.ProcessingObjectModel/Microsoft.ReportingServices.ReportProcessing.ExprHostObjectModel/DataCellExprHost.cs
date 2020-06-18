using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class DataCellExprHost : ReportObjectModelProxy
	{
		protected DataValueExprHost[] DataValueHosts;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_dataValueHostsRemotable;

		internal IList<DataValueExprHost> DataValueHostsRemotable
		{
			get
			{
				if (m_dataValueHostsRemotable == null && DataValueHosts != null)
				{
					m_dataValueHostsRemotable = new RemoteArrayWrapper<DataValueExprHost>(DataValueHosts);
				}
				return m_dataValueHostsRemotable;
			}
		}
	}
}

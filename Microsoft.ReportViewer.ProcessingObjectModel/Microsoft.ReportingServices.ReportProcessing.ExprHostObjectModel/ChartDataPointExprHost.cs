using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class ChartDataPointExprHost : IndexedExprHost
	{
		public StyleExprHost DataLabelStyleHost;

		public ActionExprHost ActionHost;

		public StyleExprHost StyleHost;

		public StyleExprHost MarkerStyleHost;

		public ActionInfoExprHost ActionInfoHost;

		protected DataValueExprHost[] CustomPropertyHosts;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

		public virtual object DataLabelValueExpr => null;

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

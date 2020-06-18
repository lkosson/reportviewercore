using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class AxisExprHost : StyleExprHost
	{
		public ChartTitleExprHost TitleHost;

		public StyleExprHost MajorGridLinesHost;

		public StyleExprHost MinorGridLinesHost;

		protected DataValueExprHost[] CustomPropertyHosts;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

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

		public virtual object AxisMinExpr => null;

		public virtual object AxisMaxExpr => null;

		public virtual object AxisCrossAtExpr => null;

		public virtual object AxisMajorIntervalExpr => null;

		public virtual object AxisMinorIntervalExpr => null;
	}
}

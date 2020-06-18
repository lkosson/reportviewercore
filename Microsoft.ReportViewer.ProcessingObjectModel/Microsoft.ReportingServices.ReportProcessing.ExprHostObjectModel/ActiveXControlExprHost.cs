using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class ActiveXControlExprHost : ReportItemExprHost
	{
		protected ParamExprHost[] ParameterHosts;

		[CLSCompliant(false)]
		protected IList<ParamExprHost> m_parameterHostsRemotable;

		internal IList<ParamExprHost> ParameterHostsRemotable
		{
			get
			{
				if (m_parameterHostsRemotable == null && ParameterHosts != null)
				{
					m_parameterHostsRemotable = new RemoteArrayWrapper<ParamExprHost>(ParameterHosts);
				}
				return m_parameterHostsRemotable;
			}
		}
	}
}

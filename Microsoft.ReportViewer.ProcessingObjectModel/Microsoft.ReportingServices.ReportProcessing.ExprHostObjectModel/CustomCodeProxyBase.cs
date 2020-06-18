namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class CustomCodeProxyBase
	{
		private IReportObjectModelProxyForCustomCode m_reportObjectModel;

		protected IReportObjectModelProxyForCustomCode Report => m_reportObjectModel;

		protected CustomCodeProxyBase(IReportObjectModelProxyForCustomCode reportObjectModel)
		{
			m_reportObjectModel = reportObjectModel;
		}

		protected virtual void OnInit()
		{
		}

		internal void CallOnInit()
		{
			OnInit();
		}
	}
}

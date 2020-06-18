namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class BaseInstance
	{
		internal IReportScope m_reportScope;

		internal virtual IReportScopeInstance ReportScopeInstance => m_reportScope.ReportScopeInstance;

		internal BaseInstance(IReportScope reportScope)
		{
			m_reportScope = reportScope;
		}

		internal virtual void SetNewContext()
		{
			ResetInstanceCache();
		}

		protected abstract void ResetInstanceCache();
	}
}

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class VisibilityInstance : BaseInstance
	{
		protected bool m_cachedStartHidden;

		protected bool m_startHiddenValue;

		protected bool m_cachedCurrentlyHidden;

		protected bool m_currentlyHiddenValue;

		public abstract bool CurrentlyHidden
		{
			get;
		}

		public abstract bool StartHidden
		{
			get;
		}

		internal VisibilityInstance(IReportScope reportScope)
			: base(reportScope)
		{
		}

		protected override void ResetInstanceCache()
		{
			m_cachedStartHidden = false;
			m_cachedCurrentlyHidden = false;
		}
	}
}

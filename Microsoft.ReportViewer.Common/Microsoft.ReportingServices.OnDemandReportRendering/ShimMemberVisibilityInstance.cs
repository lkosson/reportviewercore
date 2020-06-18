namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimMemberVisibilityInstance : VisibilityInstance
	{
		private ShimMemberVisibility m_owner;

		public override bool CurrentlyHidden
		{
			get
			{
				if (!m_cachedCurrentlyHidden)
				{
					m_cachedCurrentlyHidden = true;
					m_currentlyHiddenValue = m_owner.GetInstanceHidden();
				}
				return m_currentlyHiddenValue;
			}
		}

		public override bool StartHidden
		{
			get
			{
				if (!m_cachedStartHidden)
				{
					m_cachedStartHidden = true;
					m_startHiddenValue = m_owner.GetInstanceStartHidden();
				}
				return m_startHiddenValue;
			}
		}

		internal ShimMemberVisibilityInstance(ShimMemberVisibility owner)
			: base(null)
		{
			m_owner = owner;
		}
	}
}

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class TablixMemberInstance : BaseInstance
	{
		protected Tablix m_owner;

		protected TablixMember m_memberDef;

		protected VisibilityInstance m_visibility;

		public virtual VisibilityInstance Visibility
		{
			get
			{
				if (m_visibility == null && m_memberDef.Visibility != null)
				{
					if (m_owner.IsOldSnapshot)
					{
						m_visibility = new ShimMemberVisibilityInstance((ShimMemberVisibility)m_memberDef.Visibility);
					}
					else
					{
						m_visibility = new InternalTablixMemberVisibilityInstance((InternalTablixMember)m_memberDef);
					}
				}
				return m_visibility;
			}
		}

		internal TablixMemberInstance(Tablix owner, TablixMember memberDef)
			: base(memberDef.ReportScope)
		{
			m_owner = owner;
			m_memberDef = memberDef;
		}

		protected override void ResetInstanceCache()
		{
			if (m_visibility != null)
			{
				m_visibility.SetNewContext();
			}
		}
	}
}

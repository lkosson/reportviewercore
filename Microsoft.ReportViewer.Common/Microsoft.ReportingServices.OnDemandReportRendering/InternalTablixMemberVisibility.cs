namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalTablixMemberVisibility : Visibility
	{
		private InternalTablixMember m_owner;

		public override ReportBoolProperty Hidden
		{
			get
			{
				if (m_startHidden == null)
				{
					m_startHidden = Visibility.GetStartHidden(m_owner.MemberDefinition.Visibility);
				}
				return m_startHidden;
			}
		}

		public override string ToggleItem
		{
			get
			{
				if (m_owner.MemberDefinition.Visibility != null)
				{
					return m_owner.MemberDefinition.Visibility.Toggle;
				}
				return null;
			}
		}

		public override SharedHiddenState HiddenState => Visibility.GetHiddenState(m_owner.MemberDefinition.Visibility);

		public override bool RecursiveToggleReceiver
		{
			get
			{
				if (m_owner.MemberDefinition.Visibility != null)
				{
					return m_owner.MemberDefinition.Visibility.RecursiveReceiver;
				}
				return false;
			}
		}

		public InternalTablixMemberVisibility(InternalTablixMember owner)
		{
			m_owner = owner;
		}
	}
}

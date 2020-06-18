using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalTablixMemberVisibilityInstance : VisibilityInstance
	{
		private InternalTablixMember m_owner;

		public override bool CurrentlyHidden
		{
			get
			{
				if (!m_cachedCurrentlyHidden)
				{
					m_cachedCurrentlyHidden = true;
					Microsoft.ReportingServices.ReportIntermediateFormat.TablixMember memberDefinition = m_owner.MemberDefinition;
					ToggleCascadeDirection direction = (!memberDefinition.IsColumn) ? ToggleCascadeDirection.Row : ToggleCascadeDirection.Column;
					m_currentlyHiddenValue = memberDefinition.ComputeHidden(m_owner.OwnerTablix.RenderingContext, direction);
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
					if (m_owner.MemberDefinition.Visibility == null || m_owner.MemberDefinition.Visibility.Hidden == null)
					{
						m_startHiddenValue = false;
					}
					else
					{
						m_startHiddenValue = m_owner.MemberDefinition.ComputeStartHidden(m_owner.OwnerTablix.RenderingContext);
					}
				}
				return m_startHiddenValue;
			}
		}

		internal InternalTablixMemberVisibilityInstance(InternalTablixMember owner)
			: base(owner.ReportScope)
		{
			m_owner = owner;
		}
	}
}

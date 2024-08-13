namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal class GaugePanelState : MemberState
{
	private int m_walkGaugePanelCounter;

	private string m_gaugePanelDynamicMemberPath;

	internal override bool IsDynamic => true;

	internal override string ActiveDynamicMemberPath => m_gaugePanelDynamicMemberPath;

	internal GaugePanelState(int id, string gaugePanelPath)
		: base(null, id)
	{
		m_gaugePanelDynamicMemberPath = MemberState.AddMemberToDynamicPath(m_gaugePanelDynamicMemberPath, gaugePanelPath);
	}

	internal override bool AdvanceDynamicMembers()
	{
		if (m_walkGaugePanelCounter + 1 <= 1)
		{
			m_walkGaugePanelCounter++;
			return true;
		}
		return false;
	}

	internal override bool ResetDynamicMembers()
	{
		m_walkGaugePanelCounter = 0;
		m_hasRows = AdvanceDynamicMembers();
		return m_hasRows;
	}

	internal override bool ResetAllMembers(bool atomHeaderInstanceWalk)
	{
		return ResetDynamicMembers();
	}
}

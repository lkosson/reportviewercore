namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ActionProcessing : MemberBase
	{
		internal string m_label;

		internal string m_action;

		internal ActionProcessing()
			: base(isCustomControl: true)
		{
		}

		internal ActionProcessing DeepClone()
		{
			ActionProcessing actionProcessing = new ActionProcessing();
			if (m_label != null)
			{
				actionProcessing.m_label = string.Copy(m_label);
			}
			if (m_action != null)
			{
				actionProcessing.m_action = string.Copy(m_action);
			}
			return actionProcessing;
		}
	}
}

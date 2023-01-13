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
			actionProcessing.m_label = m_label;
			actionProcessing.m_action = m_action;
			return actionProcessing;
		}
	}
}

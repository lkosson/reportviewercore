using System.Collections;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ActionCollectionProcessing : MemberBase
	{
		internal ArrayList m_actions;

		internal ActionCollectionProcessing()
			: base(isCustomControl: true)
		{
		}

		internal ActionCollectionProcessing DeepClone()
		{
			if (m_actions == null || m_actions.Count == 0)
			{
				return null;
			}
			ActionCollectionProcessing actionCollectionProcessing = new ActionCollectionProcessing();
			int count = m_actions.Count;
			actionCollectionProcessing.m_actions = new ArrayList();
			for (int i = 0; i < count; i++)
			{
				actionCollectionProcessing.m_actions.Add(((Action)m_actions[i]).DeepClone());
			}
			return actionCollectionProcessing;
		}
	}
}

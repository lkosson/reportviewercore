using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ActionCollection : ReportElementCollectionBase<Action>
	{
		private List<Action> m_list;

		public override Action this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				return m_list[index];
			}
		}

		public override int Count => m_list.Count;

		internal ActionCollection(ActionInfo actionInfo, List<Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem> actions)
		{
			int count = actions.Count;
			m_list = new List<Action>(count);
			for (int i = 0; i < count; i++)
			{
				m_list.Add(new Action(actionInfo, actions[i], i));
			}
		}

		internal ActionCollection(ActionInfo actionInfo, Microsoft.ReportingServices.ReportRendering.ActionCollection actions)
		{
			int count = actions.Count;
			m_list = new List<Action>(count);
			for (int i = 0; i < count; i++)
			{
				m_list.Add(new Action(actionInfo, actions[i]));
			}
		}

		internal Action Add(ActionInfo owner, Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem actionItem)
		{
			Action action = new Action(owner, actionItem, m_list.Count);
			m_list.Add(action);
			return action;
		}

		internal void Update(Microsoft.ReportingServices.ReportRendering.ActionInfo newCollection)
		{
			int count = m_list.Count;
			for (int i = 0; i < count; i++)
			{
				m_list[i].Update((newCollection != null && newCollection.Actions != null) ? newCollection.Actions[i] : null);
			}
		}

		internal void SetNewContext()
		{
			if (m_list != null)
			{
				for (int i = 0; i < m_list.Count; i++)
				{
					m_list[i].SetNewContext();
				}
			}
		}

		internal void ConstructActionDefinitions()
		{
			foreach (Action item in m_list)
			{
				item.ConstructActionDefinition();
			}
		}
	}
}

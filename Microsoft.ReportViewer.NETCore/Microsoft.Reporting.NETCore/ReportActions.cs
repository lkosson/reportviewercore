using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.Reporting.NETCore
{
	internal sealed class ReportActions : IEnumerable<Action>, IEnumerable
	{
		private List<Action> m_actionList = new List<Action>();

		public int Count => m_actionList.Count;

		public List<Action> Actions
		{
			set
			{
				m_actionList = value;
			}
		}

		public Action this[int i]
		{
			get
			{
				if (i < 0 || i >= m_actionList.Count)
				{
					return null;
				}
				return m_actionList[i];
			}
		}

		public IEnumerator<Action> GetEnumerator()
		{
			return m_actionList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public Point GetActionPoint(ActionType actionType, string actionId)
		{
			if (m_actionList == null || actionType == ActionType.None || actionId == null)
			{
				return Point.Empty;
			}
			Action action = null;
			for (int i = 0; i < m_actionList.Count; i++)
			{
				action = m_actionList[i];
				if (actionId == action.Id)
				{
					return new Point(action.Position.Left, action.Position.Top);
				}
			}
			return Point.Empty;
		}
	}
}

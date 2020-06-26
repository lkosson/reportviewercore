using System.Drawing;

namespace Microsoft.Reporting.NETCore
{
	internal sealed class PostRenderArgs
	{
		private bool m_isDifferentReport;

		private bool m_isPartialRendering;

		private ActionType m_postActionType;

		private string m_actionID;

		private Point m_preActionScrollPosition;

		public bool IsDifferentReport => m_isDifferentReport;

		public bool IsPartialRendering => m_isPartialRendering;

		public string ActionID => m_actionID;

		public ActionType PostActionType => m_postActionType;

		public Point PreActionScrollPosition
		{
			get
			{
				return m_preActionScrollPosition;
			}
			set
			{
				m_preActionScrollPosition = new Point(-value.X, -value.Y);
			}
		}

		public PostRenderArgs(bool isDifferentReport, bool isPartialRendering)
		{
			m_isDifferentReport = isDifferentReport;
			m_isPartialRendering = isPartialRendering;
			m_postActionType = ActionType.None;
			m_actionID = null;
		}

		public PostRenderArgs(bool isDifferentReport, bool isPartialRendering, Point preActionScrollPosition)
			: this(isDifferentReport, isPartialRendering)
		{
			PreActionScrollPosition = preActionScrollPosition;
		}

		public PostRenderArgs(ActionType postActionType, string actionID)
		{
			if (postActionType == ActionType.Sort)
			{
				m_isDifferentReport = true;
			}
			else
			{
				m_isDifferentReport = false;
			}
			m_isPartialRendering = false;
			m_postActionType = postActionType;
			m_actionID = actionID;
		}

		public PostRenderArgs(ActionType postActionType, string actionID, Point preActionScrollPosition)
			: this(postActionType, actionID)
		{
			PreActionScrollPosition = preActionScrollPosition;
		}
	}
}

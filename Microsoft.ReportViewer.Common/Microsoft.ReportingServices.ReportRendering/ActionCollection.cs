using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ActionCollection
	{
		private MemberBase m_members;

		public Action this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				Action action = null;
				if (IsCustomControl)
				{
					action = (Processing.m_actions[index] as Action);
				}
				else
				{
					if (Rendering.m_actions != null)
					{
						action = Rendering.m_actions[index];
					}
					if (action == null)
					{
						ActionItem actionItem = Rendering.m_actionList[index];
						ActionItemInstance actionItemInstance = null;
						if (Rendering.m_actionInstanceList != null && actionItem.ComputedIndex >= 0)
						{
							actionItemInstance = Rendering.m_actionInstanceList[actionItem.ComputedIndex];
						}
						string drillthroughId = Rendering.m_ownerUniqueName + ":" + index.ToString(CultureInfo.InvariantCulture);
						action = new Action(actionItem, actionItemInstance, drillthroughId, Rendering.m_renderingContext);
						if (Rendering.m_renderingContext.CacheState)
						{
							if (Rendering.m_actions == null)
							{
								Rendering.m_actions = new Action[Count];
							}
							Rendering.m_actions[index] = action;
						}
					}
				}
				return action;
			}
		}

		public int Count
		{
			get
			{
				if (IsCustomControl)
				{
					return Processing.m_actions.Count;
				}
				return Rendering.m_actionList.Count;
			}
		}

		private bool IsCustomControl => m_members.IsCustomControl;

		private ActionCollectionRendering Rendering
		{
			get
			{
				Global.Tracer.Assert(!m_members.IsCustomControl);
				ActionCollectionRendering actionCollectionRendering = m_members as ActionCollectionRendering;
				if (actionCollectionRendering == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return actionCollectionRendering;
			}
		}

		private ActionCollectionProcessing Processing
		{
			get
			{
				Global.Tracer.Assert(m_members.IsCustomControl);
				ActionCollectionProcessing actionCollectionProcessing = m_members as ActionCollectionProcessing;
				if (actionCollectionProcessing == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return actionCollectionProcessing;
			}
		}

		public ActionCollection()
		{
			m_members = new ActionCollectionProcessing();
			Global.Tracer.Assert(IsCustomControl);
			Processing.m_actions = new ArrayList();
		}

		internal ActionCollection(ActionItemList actionItemList, ActionItemInstanceList actionItemInstanceList, string ownerUniqueName, RenderingContext renderingContext)
		{
			m_members = new ActionCollectionRendering();
			Global.Tracer.Assert(!IsCustomControl);
			Rendering.m_actionList = actionItemList;
			Rendering.m_actionInstanceList = actionItemInstanceList;
			Rendering.m_renderingContext = renderingContext;
			Rendering.m_ownerUniqueName = ownerUniqueName;
		}

		public void Add(Action action)
		{
			if (!IsCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			if (action == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "action");
			}
			int count = Processing.m_actions.Count;
			if (2 <= count)
			{
				if (action.Label == null)
				{
					throw new ReportRenderingException(ErrorCode.rrInvalidActionLabel);
				}
			}
			else if (1 == count)
			{
				if (action.Label == null)
				{
					throw new ReportRenderingException(ErrorCode.rrInvalidActionLabel);
				}
				if (((Action)Processing.m_actions[0]).Label == null)
				{
					throw new ReportRenderingException(ErrorCode.rrInvalidActionLabel);
				}
			}
			Processing.m_actions.Add(action);
		}

		internal ActionCollection DeepClone()
		{
			if (!IsCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			ActionCollection actionCollection = new ActionCollection();
			Global.Tracer.Assert(m_members != null && m_members is ActionCollectionProcessing);
			actionCollection.m_members = Processing.DeepClone();
			return actionCollection;
		}
	}
}

using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.Reporting.NETCore
{
	internal abstract class RenderingElement : RenderingElementBase
	{
		protected RPLElementProps m_instanceProperties;

		protected RPLElementPropsDef m_definitionProperties;

		protected List<Action> m_actions = new List<Action>();

		internal virtual RPLElementProps InstanceProperties => m_instanceProperties;

		internal virtual RPLElementPropsDef DefinitionProperties => m_definitionProperties;

		internal IList<Action> Actions => m_actions;

		protected virtual void Initialize(RPLElement rplElement)
		{
			m_instanceProperties = rplElement.ElementProps;
			m_definitionProperties = m_instanceProperties.Definition;
		}

		internal virtual void ProcessRenderingElementContent(RPLElement rplElement, GdiContext context, RectangleF bounds)
		{
		}

		internal virtual void DrawContent(GdiContext context)
		{
		}

		internal bool ProcessActions(GdiContext context, string uniqueName, RPLActionInfo actionInfo, RectangleF position)
		{
			if (actionInfo != null && actionInfo.Actions != null && actionInfo.Actions.Length != 0)
			{
				RPLAction rPLAction = actionInfo.Actions[0];
				if (!string.IsNullOrEmpty(rPLAction.Hyperlink))
				{
					RegisterAction(context, new HyperLinkAction(uniqueName, rPLAction.Label, position, rPLAction.Hyperlink));
				}
				else if (!string.IsNullOrEmpty(rPLAction.DrillthroughId))
				{
					RegisterAction(context, new DrillthroughAction(uniqueName, rPLAction.Label, position, rPLAction.DrillthroughId));
				}
				else if (!string.IsNullOrEmpty(rPLAction.BookmarkLink))
				{
					RegisterAction(context, new BookmarkLinkAction(uniqueName, rPLAction.Label, position, rPLAction.BookmarkLink));
				}
				return true;
			}
			return false;
		}

		protected void RegisterAction(GdiContext context, Action action)
		{
			if (context.FirstDraw)
			{
				context.RenderingReport.Actions.Add(action);
				m_actions.Add(action);
			}
		}
	}
}

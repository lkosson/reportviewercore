using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class Action
	{
		private ReportStringProperty m_label;

		private ReportStringProperty m_bookmark;

		private ReportUrlProperty m_hyperlink;

		private ActionInstance m_instance;

		private ActionDrillthrough m_drillthrough;

		private Microsoft.ReportingServices.ReportRendering.Action m_renderAction;

		private ActionInfo m_owner;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem m_actionItemDef;

		private int m_index = -1;

		public ReportStringProperty Label
		{
			get
			{
				if (m_label == null)
				{
					if (IsOldSnapshot)
					{
						if (m_renderAction.ActionDefinition.Label != null)
						{
							m_label = new ReportStringProperty(m_renderAction.ActionDefinition.Label);
						}
					}
					else if (m_actionItemDef.Label != null)
					{
						m_label = new ReportStringProperty(m_actionItemDef.Label);
					}
				}
				return m_label;
			}
		}

		public ReportStringProperty BookmarkLink
		{
			get
			{
				if (m_bookmark == null)
				{
					if (IsOldSnapshot)
					{
						if (m_renderAction.ActionDefinition.BookmarkLink != null)
						{
							m_bookmark = new ReportStringProperty(m_renderAction.ActionDefinition.BookmarkLink);
						}
					}
					else if (m_actionItemDef.BookmarkLink != null)
					{
						m_bookmark = new ReportStringProperty(m_actionItemDef.BookmarkLink);
					}
				}
				return m_bookmark;
			}
		}

		public ReportUrlProperty Hyperlink
		{
			get
			{
				if (m_hyperlink == null)
				{
					if (IsOldSnapshot)
					{
						if (m_renderAction.ActionDefinition.HyperLinkURL != null)
						{
							m_hyperlink = new ReportUrlProperty(m_renderAction.ActionDefinition.HyperLinkURL.IsExpression, m_renderAction.ActionDefinition.HyperLinkURL.OriginalText, m_renderAction.ActionDefinition.HyperLinkURL.IsExpression ? null : new ReportUrl(m_renderAction.HyperLinkURL));
						}
					}
					else if (m_actionItemDef.HyperLinkURL != null)
					{
						ReportUrl reportUrl = null;
						Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo hyperLinkURL = m_actionItemDef.HyperLinkURL;
						if (!hyperLinkURL.IsExpression)
						{
							reportUrl = ReportUrl.BuildHyperlinkUrl(m_owner.RenderingContext, m_owner.ObjectType, m_owner.ObjectName, "Hyperlink", m_owner.RenderingContext.OdpContext.ReportContext, hyperLinkURL.StringValue);
						}
						m_hyperlink = new ReportUrlProperty(hyperLinkURL.IsExpression, hyperLinkURL.OriginalText, reportUrl);
					}
				}
				return m_hyperlink;
			}
		}

		public ActionDrillthrough Drillthrough
		{
			get
			{
				if (m_drillthrough == null)
				{
					if (IsOldSnapshot)
					{
						if (m_renderAction.ActionDefinition.DrillthroughReportName != null)
						{
							m_drillthrough = new ActionDrillthrough(m_owner, m_renderAction);
						}
					}
					else if (m_actionItemDef.DrillthroughReportName != null)
					{
						m_drillthrough = new ActionDrillthrough(m_owner, m_actionItemDef, m_index);
					}
				}
				return m_drillthrough;
			}
		}

		private bool IsOldSnapshot => m_owner.IsOldSnapshot;

		public ActionInstance Instance
		{
			get
			{
				if (m_owner.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					if (IsOldSnapshot)
					{
						m_instance = new ActionInstance(m_renderAction);
					}
					else
					{
						m_instance = new ActionInstance(m_owner.ReportScope, this);
					}
				}
				(m_owner.ReportElementOwner as ReportItem)?.CriEvaluateInstance();
				return m_instance;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem ActionItemDef => m_actionItemDef;

		internal ActionInfo Owner => m_owner;

		internal Action(ActionInfo owner, Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem actionItemDef, int index)
		{
			m_owner = owner;
			m_actionItemDef = actionItemDef;
			m_index = index;
		}

		internal Action(ActionInfo owner, Microsoft.ReportingServices.ReportRendering.Action renderAction)
		{
			m_owner = owner;
			m_renderAction = renderAction;
		}

		internal void Update(Microsoft.ReportingServices.ReportRendering.Action newAction)
		{
			if (m_instance != null)
			{
				m_instance.Update(newAction);
			}
			if (m_drillthrough != null)
			{
				m_drillthrough.Update(newAction);
			}
			if (newAction != null)
			{
				m_renderAction = newAction;
			}
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_drillthrough != null)
			{
				m_drillthrough.SetNewContext();
			}
		}

		internal void ConstructActionDefinition()
		{
			ActionInstance instance = Instance;
			Global.Tracer.Assert(instance != null);
			if (instance.Label != null)
			{
				m_actionItemDef.Label = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(instance.Label);
			}
			else
			{
				m_actionItemDef.Label = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			m_label = null;
			if (BookmarkLink != null)
			{
				if (instance.BookmarkLink != null)
				{
					m_actionItemDef.BookmarkLink = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(instance.BookmarkLink);
				}
				else
				{
					m_actionItemDef.BookmarkLink = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
				}
				m_bookmark = null;
			}
			if (Hyperlink != null)
			{
				if (instance.HyperlinkText != null)
				{
					m_actionItemDef.HyperLinkURL = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(instance.HyperlinkText);
				}
				else
				{
					m_actionItemDef.HyperLinkURL = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
				}
				m_hyperlink = null;
			}
			if (Drillthrough != null)
			{
				Drillthrough.ConstructDrillthoughDefinition();
			}
		}
	}
}

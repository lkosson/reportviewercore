using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class ActionInfo
	{
		private ActionCollection m_collection;

		private Microsoft.ReportingServices.ReportRendering.ActionInfo m_renderAction;

		private Microsoft.ReportingServices.ReportIntermediateFormat.Action m_actionDef;

		private bool m_isOldSnapshot;

		private IReportScope m_reportScope;

		private IInstancePath m_instancePath;

		private ReportElement m_reportElementOwner;

		private ObjectType m_objectType;

		private string m_objectName;

		private bool m_dynamic;

		protected bool m_chartConstruction;

		private RenderingContext m_renderingContext;

		private IROMActionOwner m_romActionOwner;

		public ActionCollection Actions
		{
			get
			{
				InitActions();
				return m_collection;
			}
		}

		internal bool IsOldSnapshot => m_isOldSnapshot;

		internal IReportScope ReportScope => m_reportScope;

		internal RenderingContext RenderingContext => m_renderingContext;

		internal IInstancePath InstancePath => m_instancePath;

		internal ReportElement ReportElementOwner => m_reportElementOwner;

		internal ObjectType ObjectType => m_objectType;

		internal string ObjectName => m_objectName;

		internal bool IsDynamic
		{
			get
			{
				return m_dynamic;
			}
			set
			{
				m_dynamic = value;
			}
		}

		internal bool IsChartConstruction => m_chartConstruction;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.Action ActionDef
		{
			get
			{
				return m_actionDef;
			}
			set
			{
				m_actionDef = value;
			}
		}

		internal IROMActionOwner ROMActionOwner => m_romActionOwner;

		internal ActionInfo(RenderingContext renderingContext, IReportScope reportScope, Microsoft.ReportingServices.ReportIntermediateFormat.Action actionDef, IInstancePath instancePath, ReportElement reportElementOwner, ObjectType objectType, string objectName, IROMActionOwner romActionOwner)
		{
			m_renderingContext = renderingContext;
			m_reportScope = reportScope;
			m_actionDef = actionDef;
			m_isOldSnapshot = false;
			m_instancePath = instancePath;
			m_reportElementOwner = reportElementOwner;
			m_objectType = objectType;
			m_objectName = objectName;
			m_romActionOwner = romActionOwner;
		}

		internal ActionInfo(RenderingContext renderingContext, Microsoft.ReportingServices.ReportRendering.ActionInfo renderAction)
		{
			m_renderingContext = renderingContext;
			m_renderAction = renderAction;
			m_isOldSnapshot = true;
		}

		public Action CreateHyperlinkAction()
		{
			AssertValidCreateActionContext();
			InitActions();
			if (Actions.Count > 0)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem actionItem = new Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem();
			actionItem.HyperLinkURL = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			m_actionDef.ActionItems.Add(actionItem);
			return Actions.Add(this, actionItem);
		}

		public Action CreateBookmarkLinkAction()
		{
			AssertValidCreateActionContext();
			InitActions();
			if (Actions.Count > 0)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem actionItem = new Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem();
			actionItem.BookmarkLink = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			m_actionDef.ActionItems.Add(actionItem);
			return Actions.Add(this, actionItem);
		}

		public Action CreateDrillthroughAction()
		{
			AssertValidCreateActionContext();
			InitActions();
			if (Actions.Count > 0)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem actionItem = new Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem();
			actionItem.DrillthroughReportName = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			m_actionDef.ActionItems.Add(actionItem);
			return Actions.Add(this, actionItem);
		}

		private void AssertValidCreateActionContext()
		{
			if (!m_chartConstruction)
			{
				if (m_dynamic && m_reportElementOwner.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance)
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWritebackDynamicAction);
				}
				if (!m_dynamic && m_reportElementOwner.CriGenerationPhase != ReportElement.CriGenerationPhases.Definition)
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWritebackDynamicAction);
				}
			}
		}

		internal void Update(Microsoft.ReportingServices.ReportRendering.ActionInfo newActionInfo)
		{
			m_collection.Update(newActionInfo);
		}

		internal virtual void SetNewContext()
		{
			if (m_collection != null)
			{
				m_collection.SetNewContext();
			}
		}

		internal bool ConstructActionDefinition()
		{
			if (m_collection == null || m_collection.Count == 0)
			{
				return false;
			}
			m_collection.ConstructActionDefinitions();
			return true;
		}

		private void InitActions()
		{
			if (m_collection == null)
			{
				if (IsOldSnapshot)
				{
					m_collection = new ActionCollection(this, m_renderAction.Actions);
				}
				else
				{
					m_collection = new ActionCollection(this, m_actionDef.ActionItems);
				}
			}
		}
	}
}

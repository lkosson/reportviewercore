using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ActionDrillthrough
	{
		private ReportStringProperty m_reportName;

		private ParameterCollection m_parameters;

		private ActionDrillthroughInstance m_instance;

		private Microsoft.ReportingServices.ReportRendering.Action m_renderAction;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem m_actionItemDef;

		private ActionInfo m_owner;

		private int m_index = -1;

		public ReportStringProperty ReportName
		{
			get
			{
				if (m_reportName == null)
				{
					if (IsOldSnapshot)
					{
						m_reportName = new ReportStringProperty(m_renderAction.ActionDefinition.DrillthroughReportName);
					}
					else
					{
						m_reportName = new ReportStringProperty(m_actionItemDef.DrillthroughReportName);
					}
				}
				return m_reportName;
			}
		}

		public ParameterCollection Parameters
		{
			get
			{
				if (m_parameters == null)
				{
					if (IsOldSnapshot)
					{
						NameValueCollection drillthroughParameters = m_renderAction.DrillthroughParameters;
						if (drillthroughParameters != null)
						{
							m_parameters = new ParameterCollection(this, drillthroughParameters, m_renderAction.DrillthroughParameterNameObjectCollection, m_renderAction.DrillthroughParameterValueList, m_renderAction.ActionInstance);
						}
					}
					else if (m_actionItemDef.DrillthroughParameters != null)
					{
						m_parameters = new ParameterCollection(this, m_actionItemDef.DrillthroughParameters);
					}
				}
				return m_parameters;
			}
		}

		private bool IsOldSnapshot => m_owner.IsOldSnapshot;

		public ActionDrillthroughInstance Instance
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
						m_instance = new ActionDrillthroughInstance(m_renderAction);
					}
					else
					{
						m_instance = new ActionDrillthroughInstance(m_owner.ReportScope, this, m_index);
					}
				}
				(m_owner.ReportElementOwner as ReportItem)?.CriEvaluateInstance();
				return m_instance;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem ActionItemDef => m_actionItemDef;

		internal ActionInfo Owner => m_owner;

		internal ICatalogItemContext PathResolutionContext => m_owner.RenderingContext.OdpContext.TopLevelContext.ReportContext;

		internal ActionDrillthrough(ActionInfo owner, Microsoft.ReportingServices.ReportIntermediateFormat.ActionItem actionItemDef, int index)
		{
			m_owner = owner;
			m_actionItemDef = actionItemDef;
			m_index = index;
		}

		internal ActionDrillthrough(ActionInfo owner, Microsoft.ReportingServices.ReportRendering.Action renderAction)
		{
			m_owner = owner;
			m_renderAction = renderAction;
		}

		public void RegisterDrillthroughAction()
		{
			string drillthroughID = Instance.DrillthroughID;
			if (drillthroughID != null)
			{
				m_owner.RenderingContext.AddDrillthroughAction(drillthroughID, Instance.ReportName, (Parameters != null) ? Parameters.ParametersNameObjectCollection : null);
			}
		}

		public Parameter CreateParameter(string name)
		{
			if (!m_owner.IsChartConstruction)
			{
				if (m_owner.IsDynamic && m_owner.ReportElementOwner.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance)
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWritebackDynamicAction);
				}
				if (!m_owner.IsDynamic && m_owner.ReportElementOwner.CriGenerationPhase != ReportElement.CriGenerationPhases.Definition)
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWritebackNonDynamicAction);
				}
			}
			if (Parameters == null)
			{
				m_actionItemDef.DrillthroughParameters = new List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue>();
				Global.Tracer.Assert(Parameters != null, "(Parameters != null)");
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue parameterValue = new Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue();
			parameterValue.Name = name;
			m_actionItemDef.DrillthroughParameters.Add(parameterValue);
			if (!m_owner.IsChartConstruction && m_owner.ReportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance)
			{
				parameterValue.Value = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			return Parameters.Add(this, parameterValue);
		}

		internal void Update(Microsoft.ReportingServices.ReportRendering.Action newAction)
		{
			if (m_instance != null)
			{
				m_instance.Update(newAction);
			}
			if (newAction != null)
			{
				m_renderAction = newAction;
			}
			if (m_parameters != null)
			{
				m_parameters.Update(m_renderAction.DrillthroughParameters, m_renderAction.DrillthroughParameterNameObjectCollection, m_renderAction.ActionInstance);
			}
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_parameters != null)
			{
				m_parameters.SetNewContext();
			}
		}

		internal void ConstructDrillthoughDefinition()
		{
			ActionDrillthroughInstance instance = Instance;
			Global.Tracer.Assert(instance != null, "(instance != null)");
			if (instance.ReportName != null)
			{
				m_actionItemDef.DrillthroughReportName = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(instance.ReportName);
			}
			else
			{
				m_actionItemDef.DrillthroughReportName = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			m_reportName = null;
			if (m_parameters != null)
			{
				m_parameters.ConstructParameterDefinitions();
			}
		}
	}
}

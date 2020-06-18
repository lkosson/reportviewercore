using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class Parameter
	{
		private string m_name;

		private ReportVariantProperty m_value;

		private ReportBoolProperty m_omit;

		private ParameterInstance m_instance;

		private ActionDrillthrough m_actionDef;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue m_parameterDef;

		public string Name => m_name;

		public ReportVariantProperty Value
		{
			get
			{
				if (m_value == null)
				{
					m_value = new ReportVariantProperty(m_parameterDef.Value);
				}
				return m_value;
			}
		}

		public ReportBoolProperty Omit
		{
			get
			{
				if (m_omit == null)
				{
					m_omit = new ReportBoolProperty(m_parameterDef.Omit);
				}
				return m_omit;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue ParameterDef => m_parameterDef;

		internal ActionDrillthrough ActionDef => m_actionDef;

		public ParameterInstance Instance
		{
			get
			{
				if (m_actionDef.Owner.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				return m_instance;
			}
		}

		internal Parameter(ActionDrillthrough actionDef, Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue parameterDef)
		{
			m_name = parameterDef.Name;
			m_actionDef = actionDef;
			m_parameterDef = parameterDef;
			m_instance = new ParameterInstance(this);
		}

		internal Parameter(ActionDrillthrough actionDef, Microsoft.ReportingServices.ReportProcessing.ParameterValue parameterDef, ActionItemInstance actionInstance, int index)
		{
			m_name = parameterDef.Name;
			m_value = new ReportVariantProperty(parameterDef.Value);
			m_omit = new ReportBoolProperty(parameterDef.Omit);
			m_actionDef = actionDef;
			m_instance = new ParameterInstance(actionInstance, index);
		}

		internal void Update(ActionItemInstance actionInstance, int index)
		{
			if (m_instance != null)
			{
				m_instance.Update(actionInstance, index);
			}
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}

		internal void ConstructParameterDefinition()
		{
			ParameterInstance instance = Instance;
			Global.Tracer.Assert(instance != null);
			if (instance.Value != null)
			{
				m_parameterDef.Value = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression((string)instance.Value);
			}
			else
			{
				m_parameterDef.Value = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			m_value = null;
			if (instance.IsOmitAssined)
			{
				m_parameterDef.Omit = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(instance.Omit);
			}
			else
			{
				m_parameterDef.Omit = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			m_omit = null;
		}
	}
}

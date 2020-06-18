using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportParameter
	{
		private Microsoft.ReportingServices.ReportProcessing.ParameterDef m_renderParam;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef m_paramDef;

		private ReportParameterInstance m_paramInstance;

		private ReportStringProperty m_prompt;

		private bool m_validInstance;

		private OnDemandProcessingContext m_odpContext;

		public string Name
		{
			get
			{
				if (IsOldSnapshot)
				{
					return m_renderParam.Name;
				}
				return m_paramDef.Name;
			}
		}

		public TypeCode DataType
		{
			get
			{
				if (IsOldSnapshot)
				{
					return (TypeCode)m_renderParam.DataType;
				}
				return (TypeCode)m_paramDef.DataType;
			}
		}

		public bool Nullable
		{
			get
			{
				if (IsOldSnapshot)
				{
					return m_renderParam.Nullable;
				}
				return m_paramDef.Nullable;
			}
		}

		public bool MultiValue
		{
			get
			{
				if (IsOldSnapshot)
				{
					return m_renderParam.MultiValue;
				}
				return m_paramDef.MultiValue;
			}
		}

		public bool AllowBlank
		{
			get
			{
				if (IsOldSnapshot)
				{
					return m_renderParam.AllowBlank;
				}
				return m_paramDef.AllowBlank;
			}
		}

		public ReportStringProperty Prompt
		{
			get
			{
				if (m_prompt == null)
				{
					if (IsOldSnapshot)
					{
						m_prompt = new ReportStringProperty(isExpression: false, m_renderParam.Prompt, m_renderParam.Prompt);
					}
					else
					{
						m_prompt = new ReportStringProperty(m_paramDef.PromptExpression);
					}
				}
				return m_prompt;
			}
		}

		public bool UsedInQuery
		{
			get
			{
				if (IsOldSnapshot)
				{
					return m_renderParam.UsedInQuery;
				}
				return m_paramDef.UsedInQuery;
			}
		}

		public ReportParameterInstance Instance
		{
			get
			{
				if (!m_validInstance)
				{
					return null;
				}
				if (IsOldSnapshot)
				{
					return m_paramInstance;
				}
				if (m_paramInstance == null)
				{
					m_paramInstance = new ReportParameterInstance(this);
				}
				return m_paramInstance;
			}
		}

		internal bool IsOldSnapshot => m_renderParam != null;

		internal OnDemandProcessingContext OdpContext => m_odpContext;

		internal ReportParameter(Microsoft.ReportingServices.ReportProcessing.ParameterDef renderParam)
		{
			m_renderParam = renderParam;
		}

		internal ReportParameter(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef paramDef)
		{
			m_paramDef = paramDef;
			m_odpContext = odpContext;
		}

		internal void SetNewContext(bool validInstance)
		{
			m_validInstance = validInstance;
			if (m_paramInstance != null)
			{
				m_paramInstance.SetNewContext();
			}
		}

		internal void UpdateRenderReportItem(Microsoft.ReportingServices.ReportRendering.ReportParameter paramValue)
		{
			if (paramValue == null)
			{
				m_validInstance = false;
				return;
			}
			m_validInstance = true;
			if (m_paramInstance == null)
			{
				m_paramInstance = new ReportParameterInstance(this, paramValue);
			}
			else
			{
				m_paramInstance.UpdateRenderReportItem(paramValue);
			}
		}
	}
}

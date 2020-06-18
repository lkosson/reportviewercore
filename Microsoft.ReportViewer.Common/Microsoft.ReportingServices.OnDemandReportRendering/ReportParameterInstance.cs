using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections.ObjectModel;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportParameterInstance
	{
		private ReportParameter m_paramDef;

		private Microsoft.ReportingServices.ReportRendering.ReportParameter m_renderParamValue;

		private ParameterImpl m_paramValue;

		public string Prompt
		{
			get
			{
				if (IsOldSnapshot)
				{
					return m_renderParamValue.Prompt;
				}
				return ReportOMParam.Prompt;
			}
		}

		public object Value
		{
			get
			{
				if (IsOldSnapshot)
				{
					return m_renderParamValue.Value;
				}
				object[] values = ReportOMParam.GetValues();
				if (values == null || values.Length == 0)
				{
					return null;
				}
				return values[0];
			}
		}

		public ReadOnlyCollection<object> Values
		{
			get
			{
				if (IsOldSnapshot)
				{
					return Array.AsReadOnly(m_renderParamValue.Values);
				}
				object[] values = ReportOMParam.GetValues();
				if (values == null || values.Length == 0)
				{
					return null;
				}
				return Array.AsReadOnly(values);
			}
		}

		public string Label
		{
			get
			{
				string[] array = (!IsOldSnapshot) ? ReportOMParam.GetLabels() : m_renderParamValue.UnderlyingParam.Labels;
				if (array == null || array.Length == 0)
				{
					return null;
				}
				return array[0];
			}
		}

		public ReadOnlyCollection<string> Labels
		{
			get
			{
				string[] array = (!IsOldSnapshot) ? ReportOMParam.GetLabels() : m_renderParamValue.UnderlyingParam.Labels;
				if (array == null || array.Length == 0)
				{
					return null;
				}
				return Array.AsReadOnly(array);
			}
		}

		internal bool IsOldSnapshot => m_renderParamValue != null;

		internal ParameterImpl ReportOMParam
		{
			get
			{
				if (m_paramValue == null)
				{
					ParametersImpl parametersImpl = m_paramDef.OdpContext.ReportObjectModel.ParametersImpl;
					m_paramValue = (ParameterImpl)parametersImpl[m_paramDef.Name];
				}
				return m_paramValue;
			}
		}

		internal ReportParameterInstance(ReportParameter paramDef)
		{
			m_paramDef = paramDef;
		}

		internal ReportParameterInstance(ReportParameter paramDef, Microsoft.ReportingServices.ReportRendering.ReportParameter paramValue)
		{
			m_paramDef = paramDef;
			m_renderParamValue = paramValue;
		}

		internal void SetNewContext()
		{
			m_paramValue = null;
		}

		internal void UpdateRenderReportItem(Microsoft.ReportingServices.ReportRendering.ReportParameter paramValue)
		{
			m_renderParamValue = paramValue;
		}
	}
}

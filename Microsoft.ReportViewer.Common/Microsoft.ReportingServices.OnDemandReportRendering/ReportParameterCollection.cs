using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportParameterCollection : ReportElementCollectionBase<ReportParameter>
	{
		private List<ReportParameter> m_parameters;

		private Dictionary<string, ReportParameter> m_parametersByName;

		private NameValueCollection m_reportParameters;

		public ReportParameter this[string name]
		{
			get
			{
				if (m_parametersByName == null)
				{
					m_parametersByName = new Dictionary<string, ReportParameter>(m_parameters.Count);
					for (int i = 0; i < m_parameters.Count; i++)
					{
						ReportParameter reportParameter = m_parameters[i];
						m_parametersByName.Add(reportParameter.Name, reportParameter);
					}
				}
				return m_parametersByName[name];
			}
		}

		public override ReportParameter this[int index] => m_parameters[index];

		public override int Count => m_parameters.Count;

		internal NameValueCollection ToNameValueCollection
		{
			get
			{
				if (m_reportParameters == null && m_parameters != null)
				{
					int count = m_parameters.Count;
					m_reportParameters = new NameValueCollection(count);
					for (int i = 0; i < count; i++)
					{
						ReportParameter reportParameter = m_parameters[i];
						ReportParameterInstance instance = reportParameter.Instance;
						if (instance != null && instance.Values != null)
						{
							int count2 = instance.Values.Count;
							for (int j = 0; j < count2; j++)
							{
								m_reportParameters.Add(reportParameter.Name, Formatter.FormatWithInvariantCulture(instance.Values[j]));
							}
						}
					}
					if (count > 0)
					{
						m_reportParameters.Add("rs:ParameterLanguage", "");
					}
				}
				return m_reportParameters;
			}
		}

		internal ReportParameterCollection(ParameterDefList parameterDefs, Microsoft.ReportingServices.ReportRendering.ReportParameterCollection paramValues)
		{
			m_parameters = new List<ReportParameter>(parameterDefs.Count);
			for (int i = 0; i < parameterDefs.Count; i++)
			{
				if (parameterDefs[i].PromptUser)
				{
					m_parameters.Add(new ReportParameter(parameterDefs[i]));
				}
			}
			UpdateRenderReportItem(paramValues);
		}

		internal ReportParameterCollection(OnDemandProcessingContext odpContext, List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef> parameterDefs, bool validInstance)
		{
			m_parameters = new List<ReportParameter>(parameterDefs.Count);
			for (int i = 0; i < parameterDefs.Count; i++)
			{
				if (parameterDefs[i].PromptUser)
				{
					m_parameters.Add(new ReportParameter(odpContext, parameterDefs[i]));
				}
			}
			SetNewContext(validInstance);
		}

		internal void SetNewContext(bool validInstance)
		{
			for (int i = 0; i < m_parameters.Count; i++)
			{
				m_parameters[i].SetNewContext(validInstance);
			}
		}

		internal void UpdateRenderReportItem(Microsoft.ReportingServices.ReportRendering.ReportParameterCollection paramValues)
		{
			int count = m_parameters.Count;
			if (paramValues != null && paramValues.Count != count)
			{
				paramValues = null;
			}
			for (int i = 0; i < count; i++)
			{
				if (paramValues == null)
				{
					m_parameters[i].UpdateRenderReportItem(null);
				}
				else
				{
					m_parameters[i].UpdateRenderReportItem(paramValues[i]);
				}
			}
		}
	}
}

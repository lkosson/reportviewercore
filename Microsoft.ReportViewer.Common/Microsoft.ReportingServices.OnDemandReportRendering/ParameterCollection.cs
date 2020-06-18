using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ParameterCollection : ReportElementCollectionBase<Parameter>
	{
		private bool m_isOldSnapshot;

		private List<Parameter> m_list;

		private NameValueCollection m_drillthroughParameters;

		private DrillthroughParameters m_parametersNameObjectCollection;

		private ActionDrillthrough m_actionDef;

		public NameValueCollection ToNameValueCollection
		{
			get
			{
				if (!m_isOldSnapshot && m_drillthroughParameters == null && m_list != null)
				{
					m_drillthroughParameters = ConvertToNameValueCollection(forDrillthroughEvent: false, out bool[] sharedParams);
					if (0 < m_drillthroughParameters.Count)
					{
						m_drillthroughParameters.Add("rs:ParameterLanguage", "");
					}
					bool replaced = false;
					Microsoft.ReportingServices.ReportProcessing.ReportProcessing.StoreServerParameters storeServerParameters = m_actionDef.Owner.RenderingContext.OdpContext.StoreServerParameters;
					if (storeServerParameters != null)
					{
						string reportName = m_actionDef.Instance.ReportName;
						ICatalogItemContext subreportContext = m_actionDef.PathResolutionContext.GetSubreportContext(reportName);
						m_drillthroughParameters = storeServerParameters(subreportContext, m_drillthroughParameters, sharedParams, out replaced);
					}
				}
				return m_drillthroughParameters;
			}
		}

		public override Parameter this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				return m_list[index];
			}
		}

		public override int Count => m_list.Count;

		internal DrillthroughParameters ParametersNameObjectCollection
		{
			get
			{
				if (!m_isOldSnapshot && m_parametersNameObjectCollection == null && m_list != null)
				{
					int count = m_list.Count;
					m_parametersNameObjectCollection = new DrillthroughParameters(count);
					for (int i = 0; i < count; i++)
					{
						Parameter parameter = m_list[i];
						ParameterInstance instance = parameter.Instance;
						if (!instance.Omit)
						{
							m_parametersNameObjectCollection.Add(parameter.Name, instance.Value);
						}
					}
				}
				return m_parametersNameObjectCollection;
			}
		}

		internal ParameterCollection(ActionDrillthrough actionDef, List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue> parameters)
		{
			m_isOldSnapshot = false;
			m_actionDef = actionDef;
			if (parameters == null)
			{
				m_list = new List<Parameter>();
				return;
			}
			int count = parameters.Count;
			m_list = new List<Parameter>(count);
			for (int i = 0; i < count; i++)
			{
				m_list.Add(new Parameter(actionDef, parameters[i]));
			}
		}

		internal ParameterCollection(ActionDrillthrough actionDef, NameValueCollection drillthroughParameters, DrillthroughParameters parametersNameObjectCollection, ParameterValueList parameters, ActionItemInstance actionInstance)
		{
			m_isOldSnapshot = true;
			m_actionDef = actionDef;
			m_drillthroughParameters = drillthroughParameters;
			m_parametersNameObjectCollection = parametersNameObjectCollection;
			if (parameters == null)
			{
				m_list = new List<Parameter>();
				return;
			}
			int count = parameters.Count;
			m_list = new List<Parameter>(count);
			for (int i = 0; i < count; i++)
			{
				m_list.Add(new Parameter(actionDef, parameters[i], actionInstance, i));
			}
		}

		internal NameValueCollection ToNameValueCollectionForDrillthroughEvent()
		{
			bool[] sharedParams;
			return ConvertToNameValueCollection(forDrillthroughEvent: true, out sharedParams);
		}

		private NameValueCollection ConvertToNameValueCollection(bool forDrillthroughEvent, out bool[] sharedParams)
		{
			int count = m_list.Count;
			NameValueCollection nameValueCollection = new NameValueCollection(count);
			sharedParams = new bool[count];
			for (int i = 0; i < count; i++)
			{
				Parameter parameter = m_list[i];
				ParameterInstance instance = parameter.Instance;
				Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue parameterDef = parameter.ParameterDef;
				object obj = null;
				if (parameterDef.Value != null && parameterDef.Value.Type == Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Token)
				{
					sharedParams[i] = true;
				}
				else
				{
					sharedParams[i] = false;
				}
				if (instance.Omit)
				{
					continue;
				}
				obj = instance.Value;
				if (obj == null)
				{
					nameValueCollection.Add(parameter.Name, null);
					continue;
				}
				object[] array = obj as object[];
				if (array != null)
				{
					for (int j = 0; j < array.Length; j++)
					{
						nameValueCollection.Add(parameter.Name, ConvertValueToString(array[j], forDrillthroughEvent));
					}
				}
				else
				{
					nameValueCollection.Add(parameter.Name, ConvertValueToString(obj, forDrillthroughEvent));
				}
			}
			return nameValueCollection;
		}

		private string ConvertValueToString(object value, bool forDrillthroughEvent)
		{
			if (forDrillthroughEvent)
			{
				return Formatter.FormatWithClientCulture(value);
			}
			return Formatter.FormatWithInvariantCulture(value);
		}

		internal Parameter Add(ActionDrillthrough owner, Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue paramDef)
		{
			Parameter parameter = new Parameter(owner, paramDef);
			m_list.Add(parameter);
			return parameter;
		}

		internal void Update(NameValueCollection drillthroughParameters, DrillthroughParameters nameObjectCollection, ActionItemInstance actionInstance)
		{
			int count = m_list.Count;
			for (int i = 0; i < count; i++)
			{
				m_list[i].Update(actionInstance, i);
			}
			m_parametersNameObjectCollection = nameObjectCollection;
			m_drillthroughParameters = drillthroughParameters;
			m_parametersNameObjectCollection = nameObjectCollection;
		}

		internal void SetNewContext()
		{
			if (!m_isOldSnapshot)
			{
				m_drillthroughParameters = null;
				m_parametersNameObjectCollection = null;
			}
			if (m_list != null)
			{
				for (int i = 0; i < m_list.Count; i++)
				{
					m_list[i].SetNewContext();
				}
			}
		}

		internal void ConstructParameterDefinitions()
		{
			foreach (Parameter item in m_list)
			{
				item.ConstructParameterDefinition();
			}
		}
	}
}

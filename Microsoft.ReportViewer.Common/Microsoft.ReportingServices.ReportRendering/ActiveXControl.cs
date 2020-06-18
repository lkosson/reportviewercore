using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Specialized;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ActiveXControl : ReportItem
	{
		public sealed class Parameter
		{
			private string m_name;

			private object m_value;

			public string Name => m_name;

			public object Value => m_value;

			internal Parameter(string name, object value)
			{
				m_name = name;
				m_value = value;
			}
		}

		public sealed class ParameterCollection : NameObjectCollectionBase
		{
			public Parameter this[int index] => (Parameter)BaseGet(index);

			public Parameter this[string name] => (Parameter)BaseGet(name);

			internal ParameterCollection()
			{
			}

			internal void Add(Parameter parameter)
			{
				BaseAdd(parameter.Name, parameter);
			}
		}

		private ParameterCollection m_parameters;

		private ReportUrl m_codeBase;

		public string ClassID => ((Microsoft.ReportingServices.ReportProcessing.ActiveXControl)base.ReportItemDef).ClassID;

		public ReportUrl CodeBase
		{
			get
			{
				string codeBase = ((Microsoft.ReportingServices.ReportProcessing.ActiveXControl)base.ReportItemDef).CodeBase;
				if (codeBase == null)
				{
					return null;
				}
				ReportUrl reportUrl = m_codeBase;
				if (m_codeBase == null)
				{
					reportUrl = new ReportUrl(base.RenderingContext, codeBase);
					if (base.RenderingContext.CacheState)
					{
						m_codeBase = reportUrl;
					}
				}
				return reportUrl;
			}
		}

		public ParameterCollection Parameters
		{
			get
			{
				ParameterCollection parameterCollection = m_parameters;
				if (m_parameters == null)
				{
					Microsoft.ReportingServices.ReportProcessing.ActiveXControl activeXControl = (Microsoft.ReportingServices.ReportProcessing.ActiveXControl)base.ReportItemDef;
					if (activeXControl.Parameters != null && activeXControl.Parameters.Count > 0)
					{
						parameterCollection = new ParameterCollection();
						for (int i = 0; i < activeXControl.Parameters.Count; i++)
						{
							ParameterValue parameterValue = activeXControl.Parameters[i];
							parameterCollection.Add(new Parameter(value: (parameterValue.Value.Type != ExpressionInfo.Types.Constant) ? ((base.ReportItemInstance != null) ? ((ActiveXControlInstanceInfo)base.InstanceInfo).ParameterValues[i] : null) : parameterValue.Value.Value, name: parameterValue.Name));
						}
						if (base.RenderingContext.CacheState)
						{
							m_parameters = parameterCollection;
						}
					}
				}
				return parameterCollection;
			}
		}

		internal ActiveXControl(string uniqueName, int intUniqueName, Microsoft.ReportingServices.ReportProcessing.ActiveXControl reportItemDef, ActiveXControlInstance reportItemInstance, RenderingContext renderingContext)
			: base(uniqueName, intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
		}
	}
}
